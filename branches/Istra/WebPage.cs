using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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

			XsltProcessor xslt = new XsltProcessor();
			xslt.TransformDocument(
				@"\" + SiteSettings.Current.ContentDir + @"\pages\" + pageName + ".xml",
				@"\" + SiteSettings.Current.XsltDir + @"\article.xslt",
				settings,
				tWriter
			);
		}

	}

}