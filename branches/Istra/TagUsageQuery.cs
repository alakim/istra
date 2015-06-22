using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web;
using System.IO;

namespace Istra {

	/// <summary>Выводит текст руководства по архитектуре Istra</summary>
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
			CollectTags(contentPath, tagsRegistry);
			CollectTags(pagesPath, tagsRegistry);

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
					wrt.Write(file);
				}
				wrt.Write(@"""");
			}
			wrt.Write("}");
			requestRoot.InnerText = wrt.ToString();
		}

		private void CollectTags(string dir, Dictionary<string, Dictionary<string, bool>> tagsRegistry) {

			foreach (string file in Directory.GetFiles(dir)) {
				if (!reXmlFile.IsMatch(file)) continue;
				try {
					XmlDocument xDoc = new XmlDocument();
					xDoc.Load(file);
					XmlNodeList tags = xDoc.SelectNodes("//*");
					foreach (XmlNode nd in tags) {
						if (!(nd is XmlElement)) continue;
						XmlElement tag = (XmlElement)nd;
						if (!tagsRegistry.ContainsKey(tag.Name)) tagsRegistry[tag.Name] = new Dictionary<string, bool>();
						Dictionary<string, bool> docs = tagsRegistry[tag.Name];
						string relFile = file.Replace(contentPath, string.Empty);
						docs[relFile] = true;
					}
				}
				catch (Exception) {
				}
			}
		}
	}
}
