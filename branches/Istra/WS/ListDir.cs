using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Istra.WS {
	/// <summary>Возвращает содержимое заданной директории</summary>
	public class ListDir : Istra.WS.WebService {

		private string dirPath;
		private string contentPath;

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			dirPath = Request["dir"];
			contentPath = Istra.SiteSettings.Current.RootDir + @"\" + dirPath;

			try {
				string[] dirs = Directory.GetDirectories(contentPath);
				string[] files = Directory.GetFiles(contentPath);

				writer.Write(@"{{""dir"":""{0}"",""directories"":[", dirPath);
				bool first = true;
				foreach (string dir in dirs) {
					if (first) first = false; else writer.Write(",");
					writer.Write(@"""{0}""", FormatPath(dir));
				}
				writer.Write(@"],""files"":[");
				first = true;
				foreach (string filePath in files) {
					if (first) first = false; else writer.Write(",");
					writer.Write(@"""{0}""", FormatPath(filePath));
				}
				writer.Write(@"]}");
			}
			catch (Exception err) {
				WriteError("Ошибка чтения директории", err, writer);
			}

		}

		private string FormatPath(string path) {
			return path.Replace(contentPath+@"\", string.Empty)
				.Replace(@"\", "/");
		}
	}
}
