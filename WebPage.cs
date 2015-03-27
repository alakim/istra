using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using System.Xml;
using System.Xml.Xsl;
using System.Configuration;
using System.Collections.Specialized;

namespace Istra {
	/// <summary>Веб-страница</summary>
	public class WebPage : System.Web.UI.Page {
		/// <summary>Директори приложения</summary>
		private string rootDir;
		/// <summary>Директория для размещения конетента</summary>
		private string contentDir;
		/// <summary>Директория для размещения кэша</summary>
		private string cacheDir;
		/// <summary>Директория для размещения XSLT-преобразований</summary>
		private string xsltDir;

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			NameValueCollection settings = (NameValueCollection)ConfigurationManager.GetSection("Istra/XsltSettings");
			rootDir = settings["rootDir"].ToString();
			contentDir = settings["contentDir"].ToString();
			cacheDir = settings["cacheDir"].ToString();
			xsltDir = settings["xsltDir"].ToString();

			BuildMenu();

			string pageNm = Request["p"];
			if (pageNm == null || pageNm.Length < 1) pageNm = "about";

			string html = BuildPage(pageNm);
			writer.Write(html);
		}


		public void BuildMenu() {
			string filePath = rootDir + @"\" + cacheDir + @"\menu.xml";
			File.Delete(filePath);

			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings["contentFolder"] = rootDir + @"\" + contentDir;

			StreamWriter wrt = new StreamWriter(filePath);
			TransformDocument(@"\" + contentDir + @"\toc.xml", @"\" + xsltDir + @"\menu.xslt", settings, wrt);
			wrt.Close();
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
			settings["contentFolder"] = rootDir + @"\" + contentDir;
			settings["cacheFolder"] = rootDir + @"\" + cacheDir;
			settings["jsFolder"] = "/js";
			settings["cssFolder"] = "/";

			TransformDocument(
				@"\" + contentDir + @"\pages\" + pageName + ".xml",
				@"\" + xsltDir + @"\article.xslt",
				settings,
				tWriter
			);
		}

		/// <summary>Преобразует XML-документ</summary>
		/// <param name="srcPath">исходный документ</param>
		/// <param name="xsltPath">XSLT-преобразование</param>
		/// <param name="tWriter">компонен вывода данных</param>
		private void TransformDocument(string srcPath, string xsltPath, TextWriter tWriter) {
			TransformDocument(srcPath, xsltPath, null, tWriter);
		}

		/// <summary>Преобразует XML-документ</summary>
		/// <param name="srcPath">исходный документ</param>
		/// <param name="xsltPath">XSLT-преобразование</param>
		/// <param name="settings">дополнительные настройки преобразования</param>
		/// <param name="tWriter">компонент вывода данных</param>
		private void TransformDocument(string srcPath, string xsltPath, Dictionary<string, string> settings, TextWriter tWriter) {
			XsltSettings xSettings = new XsltSettings();
			xSettings.EnableDocumentFunction = true;

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(rootDir + srcPath);

			XmlElement root = xmlDoc.DocumentElement;

			if (settings != null) {
				foreach (string k in settings.Keys) {
					root.SetAttribute(k, settings[k]);
				}
			}

			XslCompiledTransform xslt = new XslCompiledTransform();
			xslt.Load(rootDir + xsltPath, xSettings, new XmlUrlResolver());

			XsltArgumentList xArg = new XsltArgumentList();
			XmlWriter xwrt = XmlWriter.Create(tWriter);
			xslt.Transform(xmlDoc, xArg, tWriter /*xwrt, new UrlResolver()*/);
		}

	}

}