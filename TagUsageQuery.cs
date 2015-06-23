using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web;
using System.IO;

namespace Istra {

	/// <summary>Выводит данные об использовании XML-тегов</summary>
	public class TagUsageQuery : IQuery {

		private static Regex reXmlFile = new Regex(@"\.xml$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private string contentPath;

		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="xQuery">XML-описание данного запроса</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			XmlNodeList auth = doc.SelectNodes(@"//AuthDialog/authenticated");
			if (auth.Count < 1) {
				xQuery.ParentNode.RemoveChild(xQuery);
				return;
			}

			XmlElement requestRoot = doc.CreateElement("TagUsage");
			xQuery.ParentNode.InsertAfter(requestRoot, xQuery);
			xQuery.ParentNode.RemoveChild(xQuery);


			contentPath = Istra.SiteSettings.Current.RootDir + @"/" + Istra.SiteSettings.Current.ContentDir + @"/";
			string pagesPath = contentPath + @"pages/";

			Dictionary<string, Dictionary<string, bool>> tagsRegistry = new Dictionary<string, Dictionary<string, bool>>();
			CollectTags(contentPath, tagsRegistry, true);
			//CollectTags(pagesPath, tagsRegistry);
			CollectFromFile(Istra.SiteSettings.Current.RootDir + @"/users.xml", tagsRegistry);

			StringWriter wrt = new StringWriter();
			wrt.Write("{");
			bool first = true;
			foreach (string tagNm in tagsRegistry.Keys) {
				if (first) first = false; else wrt.Write(",");
				wrt.Write(@"""{0}"":""", tagNm);
				Dictionary<string, bool> filesDict = tagsRegistry[tagNm];
				bool firstFile = true;
				foreach (string file in filesDict.Keys) {
					if (!filesDict[file]) continue;
					if (firstFile) firstFile = false; else wrt.Write(";");
					wrt.Write(JsonUtility.PrepareString(file, true));
				}
				wrt.Write(@"""");
			}
			wrt.Write("}");
			requestRoot.InnerText = wrt.ToString();
		}

		/// <summary>Собирает данные о тегах из заданной директории и ее поддиректорий</summary>
		/// <param name="dir">сканируемая директория</param>
		/// <param name="tagsRegistry">реестр найденных тегов</param>
		private void CollectTags(string dir, Dictionary<string, Dictionary<string, bool>> tagsRegistry) {
			CollectTags(dir, tagsRegistry, true);
		}

		/// <summary>Собирает данные о тегах из заданной директории</summary>
		/// <param name="dir">сканируемая директория</param>
		/// <param name="tagsRegistry">реестр найденных тегов</param>
		/// <param name="recursive">признак рекурсивного сканирования поддиректорий</param>
		private void CollectTags(string dir, Dictionary<string, Dictionary<string, bool>> tagsRegistry, bool recursive) {

			foreach (string file in Directory.GetFiles(dir)) {
				if (!reXmlFile.IsMatch(file)) continue;
				try {
					CollectFromFile(file, tagsRegistry);
				}
				catch (Exception) {
				}
			}
			
			if (!recursive) return;

			foreach (string subDir in Directory.GetDirectories(dir)) {
				CollectTags(subDir, tagsRegistry);
			}
		}

		/// <summary>Собирает данные о тегах из заданного файла</summary>
		/// <param name="file">сканируемый файл</param>
		/// <param name="tagsRegistry">реестр найденных тегов</param>
		private void CollectFromFile(string file, Dictionary<string, Dictionary<string, bool>> tagsRegistry) {
			XmlDocument xDoc = new XmlDocument();
			xDoc.Load(file);
			XmlNodeList tags = xDoc.SelectNodes("//*");
			foreach (XmlNode nd in tags) {
				if (!(nd is XmlElement)) continue;
				XmlElement tag = (XmlElement)nd;
				if (!tagsRegistry.ContainsKey(tag.Name)) tagsRegistry[tag.Name] = new Dictionary<string, bool>();
				Dictionary<string, bool> docs = tagsRegistry[tag.Name];
				string relFile = file.Replace(SiteSettings.Current.RootDir, string.Empty).Replace(@"\", "/");
				docs[relFile] = true;
			}
		}

	}
}
