using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Xml;

namespace Istra {
	/// <summary>Получает данные, расположенные в файлах заданной директории</summary>
	public class DirectoryDataSource : DataSource {
		/// <summary>Конструктор</summary>
		/// <param name="def">данные определения источника</param>
		public DirectoryDataSource(DataSourceDefinition def) :base(def) {
			cachedFile = def.CachedFile;
			rootDir = def.Attributes["rootDir"];
			if (rootDir == null || rootDir.Length < 1) throw new ApplicationException("DirectoryDataSource construction error. Root directory name expected.");
			xsltName = def.Attributes["xsltName"];
			// if (xsltName == null || xsltName.Length < 1) throw new ApplicationException("DirectoryDataSource construction error. XSLT name expected.");
			
		}
		
		/// <summary>Формирует кэшированный файл данных</summary>
		/// <param name="context">контекст веб-приложения</param>
		public override bool Build(HttpContext context) {
			if (!base.Build(context)) return false;

			XmlDocument doc = new XmlDocument();
			string contentPath = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir;
			AddDirectory(contentPath, contentPath + @"\" + rootDir, doc);

			if (xsltName == null || xsltName.Length < 1) {
				doc.Save(FilePath);
			}
			else {
				Dictionary<string, string> trSettings = new Dictionary<string, string>();
				using (StreamWriter wrt = OpenFileStream()) {
					XsltProcessor xslt = new XsltProcessor(context);
					xslt.TransformDocument(doc, @"\" + SiteSettings.Current.XsltDir + @"\" + xsltName, trSettings, wrt);
				}
			}
			PrepareDocument();
			return true;
		}

		/// <summary>Добавляет в документ описание директории</summary>
		/// <param name="basePath">базовый путь</param>
		/// <param name="dirPath">путь к директории</param>
		/// <param name="doc">целевой XML-документ</param>
		private void AddDirectory(string basePath, string dirPath, XmlDocument doc) {
			AddDirectory(basePath, dirPath, doc, null);
		}

		/// <summary>Добавляет в документ описание директории</summary>
		/// <param name="basePath">базовый путь</param>
		/// <param name="dirPath">путь к директории</param>
		/// <param name="doc">целевой XML-документ</param>
		/// <param name="root">корневой XML-элемент</param>
		private void AddDirectory(string basePath, string dirPath, XmlDocument doc, XmlElement root) {
			string dirName = dirPath.Replace(basePath + @"\", string.Empty);
			XmlElement dir = doc.CreateElement("directory");
			XmlUtility.AddAttribute(doc, dir, "name", dirName);
			if (root == null) doc.AppendChild(dir); else root.AppendChild(dir);

			foreach (string filePath in Directory.GetFiles(dirPath)) {
				string fileName = filePath.Replace(dirPath + @"\", string.Empty);
				XmlElement el = doc.CreateElement("file");
				XmlUtility.AddAttribute(doc, el, "name", fileName);
				LoadFile(filePath, doc, el);
				dir.AppendChild(el);
			}
			foreach (string cDir in Directory.GetDirectories(dirPath)) {
				AddDirectory(dirPath, cDir, doc, dir);
			}
		}

		private static Regex reXmlDef = new Regex(@"<\?xml[^\?]+\?>\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>Загружает содержимое файла в целевой XML-элемент</summary>
		/// <param name="filePath">путь к файлу</param>
		/// <param name="doc">целевой XML-документ</param>
		/// <param name="target">целевой XML-элемент</param>
		private void LoadFile(string filePath, XmlDocument doc, XmlElement target) {
			XmlDocument d1 = new XmlDocument();
			d1.Load(filePath);
			string xml = reXmlDef.Replace(d1.InnerXml, string.Empty);
			target.InnerXml = xml;
		}

		private string rootDir;
		private string xsltName;
	}
}
