using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using System.Xml;
using System.Xml.Xsl;

namespace Istra {
	/// <summary>Веб-страница</summary>
	public class WebPage : System.Web.UI.Page {
		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			PageUtil.BuildMenu();

			string pageNm = Request["p"];
			if (pageNm == null || pageNm.Length < 1) pageNm = "about";

			string html = PageUtil.BuildPage(pageNm);
			writer.Write(html);
		}
	}


	/// <summary>Формирует страницы сайта</summary>
	public class PageUtil {
		/// <summary>Директори приложения</summary>
		private const string rootDir = @"C:\Users\Alexandr.Akimov\Documents\Visual Studio 2012\Projects\istraview\istraview";
		/// <summary>Директория для размещения конетента</summary>
		private const string contentDir = "content";
		/// <summary>Директория для размещения кэша</summary>
		private const string cacheDir = "cache";
		/// <summary>Директория для размещения XSLT-преобразований</summary>
		private const string xsltDir = @"istra\xslt";

		public static void BuildMenu() {
			string filePath = rootDir + @"\" + cacheDir + @"\menu.xml";
			File.Delete(filePath);

			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings["contentFolder"] = rootDir + @"\" + contentDir;

			StreamWriter wrt = new StreamWriter(filePath);
			TransformDocument(@"\content\toc.xml", @"\" + xsltDir + @"\menu.xslt", settings, wrt);
			wrt.Close();
		}


		private static Regex reHeader = new Regex(@"<html[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>Формирует HTML-код веб-страницы</summary>
		/// <param name="pageName">имя веб-страницы</param>
		public static string BuildPage(string pageName) {
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
		private static void BuildPage(string pageName, TextWriter tWriter) {
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings["contentFolder"] = rootDir + @"\" + contentDir;
			settings["cacheFolder"] = rootDir + @"\" + cacheDir;
			settings["jsFolder"] = "/js";
			
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
		private static void TransformDocument(string srcPath, string xsltPath, TextWriter tWriter) {
			TransformDocument(srcPath, xsltPath, null, tWriter);
		}

		/// <summary>Преобразует XML-документ</summary>
		/// <param name="srcPath">исходный документ</param>
		/// <param name="xsltPath">XSLT-преобразование</param>
		/// <param name="settings">дополнительные настройки преобразования</param>
		/// <param name="tWriter">компонент вывода данных</param>
		private static void TransformDocument(string srcPath, string xsltPath, Dictionary<string, string> settings, TextWriter tWriter) {
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
			xslt.Transform(xmlDoc, xArg, tWriter);
		}
	
	}
}