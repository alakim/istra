using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using Istra.Json;

namespace Istra.WS {

	/// <summary>Выводит результаты поиска по сайту</summary>
	public class Search : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);

			pattern = Request["s"];
			string root = Request["r"];
			rootDir = Istra.SiteSettings.Current.RootDir + (root!=null?@"\"+root: @"\content\pages");

			JsonArray list = new JsonArray();
			SearchDir(rootDir, list);

			JsonObject res = new JsonObject();
			res.Add("searchString", pattern);
			res.Add("result", list);

			writer.Write(res.Serialize());
		}

		/// <summary>Выполняет поиск в заданной директории</summary>
		/// <param name="path">путь к директории</param>
		private void SearchDir(string path, JsonArray list) {
			foreach (string file in Directory.GetFiles(path)) {
				if (!reXmlFile.IsMatch(file)) continue;
				XmlDocument doc = new XmlDocument();
				doc.Load(file);
				if (doc.DocumentElement.Name != "article") continue;
				if (doc.InnerXml.Contains(pattern)) {
					JsonObject docRef = new JsonObject();
					docRef.Add("ref", file.Replace(rootDir, string.Empty).Replace(@"\", "/"));
					docRef.Add("title", doc.SelectSingleNode(@"//article/@title").Value);
					list.Add(docRef);
				}
			}
			foreach (string dir in Directory.GetDirectories(path)) {
				SearchDir(dir, list);
			}
		}

		private string pattern;
		private string rootDir;

		private static Regex reXmlFile = new Regex(@"\.xml$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
