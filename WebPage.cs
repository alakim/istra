using System;
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

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
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
						xmlDoc.Load(SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir + 
							@"\pages\" + pageName + ".xml");
					}
					catch (Exception err) {
						ErrorLog.WriteError(err);
						// xmlDoc.Load(SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir + 
						// 	@"\pages\" + SiteSettings.Current.DefaultPage + ".xml");
						this.Context.Response.Redirect(SefSettings.Current.DefaultPage+"/?p="+SiteSettings.Current.DefaultPage);
					}

					XmlNode attProprocessor = xmlDoc.DocumentElement.Attributes.GetNamedItem("preprocessor");
					if (attProprocessor != null) {
						Type t = Type.GetType(attProprocessor.Value);
						ConstructorInfo cInf = t.GetConstructor(new Type[0] { });
						IPagePreprocessor preprocessor = (IPagePreprocessor)cInf.Invoke(new object[0] { });
						preprocessor.Process(xmlDoc);
					}

					XsltProcessor xslt = new XsltProcessor(Context);
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