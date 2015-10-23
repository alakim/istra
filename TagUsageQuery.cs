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

			TagsRegistry tagsRegistry = new TagsRegistry(Istra.SiteSettings.Current.RootDir + @"/" + Istra.SiteSettings.Current.ContentDir + @"/");

			StringWriter wrt = new StringWriter();
			wrt.Write("{");
			bool first = true;
			foreach (string tagNm in tagsRegistry.Tags) {
				if (first) first = false; else wrt.Write(",");
				wrt.Write(@"""{0}"":""", tagNm);
				bool firstFile = true;
				foreach (string file in tagsRegistry.GetFiles(tagNm)) {
					if (firstFile) firstFile = false; else wrt.Write(";");
					wrt.Write(JsonUtility.PrepareString(file, true));
				}
				wrt.Write(@"""");
			}
			wrt.Write("}");
			requestRoot.InnerText = wrt.ToString();
		}

	}

	/// <summary>Контрол для размещения на странице</summary>
	public class TagUsageControl : System.Web.UI.Control {
		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			TagsRegistry tagsRegistry = new TagsRegistry(Istra.SiteSettings.Current.RootDir + @"/" + Istra.SiteSettings.Current.ContentDir + @"/");
			writer.Write(@"<div class=""tagUsageDiagram"">{");
			bool first = true;
			foreach (string tagNm in tagsRegistry.Tags) {
				if (first) first = false; else writer.Write(",");
				writer.Write(@"""{0}"":""", tagNm);
				bool firstFile = true;
				foreach (string file in tagsRegistry.GetFiles(tagNm)) {
					if (firstFile) firstFile = false; else writer.Write(";");
					writer.Write(JsonUtility.PrepareString(file, true));
				}
				writer.Write(@"""");
			}
			writer.Write("}</div>");
		}
	}

	/// <summary>Реестр тегов</summary>
	public class TagsRegistry {

		/// <summary>Конструктор</summary>
		public TagsRegistry(string contentPath) {
			this.registry = BuildRegistry(contentPath);
			this.keys = new List<string>();
			foreach (string k in registry.Keys) {
				keys.Add(k);
			}
		}

		/// <summary>Возвращает коллекцию имен тегов</summary>
		public List<string> Tags { get { return keys; } }

		/// <summary>Возвращает список файлов, использующих заданные тег</summary>
		/// <param name="tag">имя тега</param>
		public List<string> GetFiles(string tag) {
			List<string> files = new List<string>();
			Dictionary<string, bool> filesDict = registry[tag];
			foreach (string file in filesDict.Keys) {
				if (!filesDict[file]) continue;
				files.Add(file);
			}
			return files;
		}

		private Dictionary<string, Dictionary<string, bool>> registry;
		private List<string> keys;

		/// <summary>Формирует реестр тегов</summary>
		private static Dictionary<string, Dictionary<string, bool>> BuildRegistry(string contentPath) {
			Dictionary<string, Dictionary<string, bool>> tagsRegistry = new Dictionary<string, Dictionary<string, bool>>();
			TagsRegistry.CollectTags(contentPath, tagsRegistry, true);
			TagsRegistry.CollectFromFile(Istra.SiteSettings.Current.RootDir + @"/users.xml", tagsRegistry);
			return tagsRegistry;
		}

		private static Regex reXmlFile = new Regex(@"\.xml$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>Собирает данные о тегах из заданной директории и ее поддиректорий</summary>
		/// <param name="dir">сканируемая директория</param>
		/// <param name="tagsRegistry">реестр найденных тегов</param>
		private static void CollectTags(string dir, Dictionary<string, Dictionary<string, bool>> tagsRegistry) {
			TagsRegistry.CollectTags(dir, tagsRegistry, true);
		}

		/// <summary>Собирает данные о тегах из заданной директории</summary>
		/// <param name="dir">сканируемая директория</param>
		/// <param name="tagsRegistry">реестр найденных тегов</param>
		/// <param name="recursive">признак рекурсивного сканирования поддиректорий</param>
		private static void CollectTags(string dir, Dictionary<string, Dictionary<string, bool>> tagsRegistry, bool recursive) {

			foreach (string file in Directory.GetFiles(dir)) {
				if (!reXmlFile.IsMatch(file)) continue;
				try {
					TagsRegistry.CollectFromFile(file, tagsRegistry);
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
		private static void CollectFromFile(string file, Dictionary<string, Dictionary<string, bool>> tagsRegistry) {
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
