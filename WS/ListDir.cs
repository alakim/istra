using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace Istra.WS {
	/// <summary>Возвращает содержимое заданной директории</summary>
	public class ListDir : Istra.WS.WebService {

		private string dirPath;
		private string contentPath;
		private Dictionary<string, string> fieldsQuery = null;

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			IAccessProvider access = GetAccessProvider();

			dirPath = Request["dir"];
			contentPath = Istra.SiteSettings.Current.RootDir + @"\" + dirPath;
			string sFieldsQuery = Request["fields"];
			if (sFieldsQuery != null && sFieldsQuery.Length > 0) {
				fieldsQuery = new Dictionary<string, string>();
				foreach (string def in sFieldsQuery.Split(";".ToCharArray())) {
					string[] pair = def.Split(":".ToCharArray());
					fieldsQuery[pair[0]] = pair[1];
				}
			}

			try {
				string[] dirs = Directory.GetDirectories(contentPath);
				string[] files = Directory.GetFiles(contentPath);

				AccessLevel dirLevel = access.GetRights(dirPath);

				writer.Write(@"{{""dir"":""{0}"",""readonly"":{1},""directories"":[", dirPath, dirLevel==AccessLevel.write?"false":"true");
				bool first = true;
				foreach (string dir in dirs) {
					AccessLevel aLevel = access.GetRights(dir);
					if (aLevel == AccessLevel.none) continue;

					if (first) first = false; else writer.Write(",");
					if(aLevel==AccessLevel.read)
						writer.Write(@"{{""name"":""{0}"",""readonly"":true}}", FormatPath(dir));
					else
						writer.Write(@"""{0}""", FormatPath(dir));
				}
				writer.Write(@"],""files"":[");
				first = true;
				foreach (string filePath in files) {
					AccessLevel aLevel = access.GetRights(filePath);
					if (aLevel == AccessLevel.none) continue;

					if (first) first = false; else writer.Write(",");
					if (fieldsQuery != null && reXmlFile.Match(filePath).Success)
						WriteWithFields(fieldsQuery, filePath, aLevel, writer);
					else if (aLevel == AccessLevel.read)
						writer.Write(@"{{""name"":""{0}"",""readonly"":true}}", FormatPath(filePath));
					else
						writer.Write(@"""{0}""", FormatPath(filePath));
				}
				writer.Write(@"]}");
			}
			catch (Exception err) {
				WriteError("Ошибка чтения директории", err, writer);
			}

		}

		private static Regex reXmlFile = new Regex(@"\.xml$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>Выводит значения дополнительных полей</summary>
		/// <param name="fieldsQuery"></param>
		/// <param name="filePath"></param>
		/// <param name="writer"></param>
		private void WriteWithFields(Dictionary<string, string> fieldsQuery, string filePath, AccessLevel aLevel, System.Web.UI.HtmlTextWriter writer) {
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);
			writer.Write(@"{{""name"":""{0}"",""readonly"":{1},""fields"":{{", FormatPath(filePath), aLevel==AccessLevel.read?"true":"false");
			bool first = true;
			foreach (string field in fieldsQuery.Keys) {
				XmlNodeList res = doc.SelectNodes(fieldsQuery[field]);
				if (res.Count > 0) {
					string val = res[0].Value;
					if (first) first = false; else writer.Write(",");
					writer.Write(@"""{0}"":{1}", field, JsonUtility.PrepareString(val));
				}
			}
			writer.Write("}}");
		}

		private string FormatPath(string path) {
			return path.Replace(contentPath+@"\", string.Empty)
				.Replace(@"\", "/");
		}
	}
}
