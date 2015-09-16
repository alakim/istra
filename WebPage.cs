﻿using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;
using System.Threading;

namespace Istra {
	/// <summary>Веб-страница</summary>
	public class WebPage : System.Web.UI.Page {

		/// <summary>Ключ запроса для выдачи документа без преобразования</summary>
		public static string RawKeyName = "raw";

		private static Regex reFile = new Regex(@"[^\.\/]+\.aspx$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			SiteSettings.Key = reFile.Replace(Request.FilePath, string.Empty);

			DataSource.RefreshSources(Context);

			string pageNm = Request["p"];
			if (pageNm == null || pageNm.Length < 1) pageNm = SiteSettings.Current.DefaultPage;

			string html = BuildPage(pageNm);
			writer.Write(html);
		}


		private static Regex reHeader = new Regex(@"<html[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>Формирует HTML-код веб-страницы</summary>
		/// <param name="pageName">имя веб-страницы</param>
		public string BuildPage(string pageName) {
			StringBuilder sb = new StringBuilder();
			StringWriter wrt = new StringWriter(sb);

			BuildPage(pageName, wrt);

			string html = sb.ToString();
			html = reHeader.Replace(html, "<!DOCTYPE html>\n<html>");
			return html;
		}

		/// <summary>Возвращает признак доступа с мобильного устройства</summary>
		private bool IsMobileBrowser() {
			const string mobCookieName = "istra_mobile";
			HttpCookie ccc = Request.Cookies[mobCookieName];
			if (ccc != null) return ccc.Value == "1";
			if (Request.Browser.IsMobileDevice) return true;

			string userAgent = Request.ServerVariables["HTTP_USER_AGENT"];

			Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			if (userAgent!=null && (b.IsMatch(userAgent) || (userAgent.Length>4 && v.IsMatch(userAgent.Substring(0, 4))))) return true;

			if (SiteSettings.Current.MobileDetection != null && SiteSettings.Current.MobileDetection.Length > 0) {
				Regex reMob = new Regex(SiteSettings.Current.MobileDetection, RegexOptions.IgnoreCase | RegexOptions.Compiled);
				return userAgent!=null && reMob.IsMatch(userAgent);
			}
			return false;
		}


		/// <summary>Формирует HTML-код веб-страницы</summary>
		/// <param name="pageName">имя веб-страницы</param>
		/// <param name="tWriter">компонент вывода данных</param>
		private void BuildPage(string pageName, TextWriter tWriter) {
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings["contentFolder"] = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir;
			settings["cacheFolder"] = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.CacheDir;
			settings["jsFolder"] = "/js";
			settings["cssFolder"] = "/";

			Mutex m = new Mutex();
			if (m.WaitOne(SiteSettings.mutexTimeout, false)) {
				try {
					// int x = 0;
					// int y = 5 / x;
					XmlDocument xmlDoc = new XmlDocument();
					try {
						string filePath = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir + 
							@"\pages\" + pageName + ".xml";
						if (File.Exists(filePath))
							xmlDoc.Load(filePath);
						else {
							if (SefSettings.Current.LogMissingPages)
								ErrorLog.WriteError(new ApplicationException("Missing content file " + pageName + ".xml"));
							//this.Context.Server.Transfer(SefSettings.Current.DefaultPage + "/?p=" + SiteSettings.Current.DefaultPage);
							this.Context.Response.Redirect(SefSettings.Current.DefaultPage + "/?p=" + SiteSettings.Current.DefaultPage);
							return;
						}
					}
					catch (Exception err) {
						//if(SefSettings.Current.LogMissingPages)
						ErrorLog.WriteError(err);
						// xmlDoc.Load(SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir + 
						// 	@"\pages\" + SiteSettings.Current.DefaultPage + ".xml");
						this.Context.Response.Redirect(SefSettings.Current.DefaultPage+"/?p="+SiteSettings.Current.DefaultPage);
					}

					XmlUtility.AddAttribute(xmlDoc, xmlDoc.DocumentElement, "pageName", pageName);
					XmlUtility.AddAttribute(xmlDoc, xmlDoc.DocumentElement, "mobileMode", IsMobileBrowser() ? "true" : "false");
					XmlUtility.AddAttribute(xmlDoc, xmlDoc.DocumentElement, "userAgent", Request.ServerVariables["HTTP_USER_AGENT"]);

					if (SiteSettings.Current.Preprocessor != null)
						SiteSettings.Current.Preprocessor.Process(xmlDoc, Context);

					XmlNode attProprocessor = xmlDoc.DocumentElement.Attributes.GetNamedItem("preprocessor");
					if (attProprocessor != null) {
						Type t = Type.GetType(attProprocessor.Value);
						ConstructorInfo cInf = t.GetConstructor(new Type[0] { });
						IPagePreprocessor preprocessor = (IPagePreprocessor)cInf.Invoke(new object[0] { });
						preprocessor.Process(xmlDoc, Context);
					}

					XsltProcessor xslt = new XsltProcessor(Context);
					xslt.RawMode = SiteSettings.Current.AllowRawOutput && (Request[RawKeyName] != null);

					xslt.TransformDocument(
						xmlDoc,
						@"\" + SiteSettings.Current.XsltDir + @"\article.xslt",
						settings,
						tWriter
					);
				}
				catch (Exception err) {
					ErrorLog.WriteError(err);
				}
				finally {
					m.ReleaseMutex();
				}
			}
		}

	}

}