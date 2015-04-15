using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Xml;

namespace Istra {
	/// <summary>Получает данные, расположенные в файлах заданной директории</summary>
	public class DirectoryDataSource : DataSource {
		/// <summary>Конструктор</summary>
		/// <param name="def">данные определения источника</param>
		public DirectoryDataSource(DataSourceDefinition def) {
			cachedFile = def.CachedFile;
			rootDir = def.Attributes["rootDir"];
			if (rootDir == null || rootDir.Length < 1) throw new ApplicationException("DirectoryDataSource construction error. Root directory name expected.");
			xsltName = def.Attributes["xsltName"];
			if (xsltName == null || xsltName.Length < 1) throw new ApplicationException("DirectoryDataSource construction error. XSLT name expected.");
			
		}
		
		/// <summary>Формирует кэшированный файл данных</summary>
		public override bool Build(HttpContext context) {
			if (!base.Build(context)) return false;

			XmlDocument doc = new XmlDocument();
			string contentPath = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir;
			AddDirectory(contentPath, contentPath + @"\" + rootDir, doc);

			Dictionary<string, string> trSettings = new Dictionary<string, string>();
			using (StreamWriter wrt = OpenFileStream()) {
				XsltProcessor xslt = new XsltProcessor(context);
				xslt.TransformDocument(doc, @"\" + SiteSettings.Current.XsltDir + @"\" + xsltName, trSettings, wrt);
			}
			return true;
		}

		private void AddDirectory(string basePath, string dirPath, XmlDocument doc) {
			AddDirectory(basePath, dirPath, doc, null);
		}

		private void AddDirectory(string basePath, string dirPath, XmlDocument doc, XmlElement root) {
			string dirName = dirPath.Replace(basePath + @"\", string.Empty);
			XmlElement dir = doc.CreateElement("directory");
			XmlUtility.AddAttribute(doc, dir, "name", dirName);
			if (root == null) doc.AppendChild(dir); else root.AppendChild(dir);

			foreach (string filePath in Directory.GetFiles(dirPath)) {
				string fileName = filePath.Replace(dirPath + @"\", string.Empty);
				XmlElement el = doc.CreateElement("file");
				XmlUtility.AddAttribute(doc, el, "name", fileName);
				dir.AppendChild(el);
			}
			foreach (string cDir in Directory.GetDirectories(dirPath)) {
				AddDirectory(dirPath, cDir, doc, dir);
			}
		}

		private string rootDir;
		private string xsltName;
	}
}
