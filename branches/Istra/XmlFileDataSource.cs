using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

namespace Istra {
	/// <summary>Получает данные из XML-файла</summary>
	class XmlFileDataSource : DataSource {
		/// <summary>Конструктор</summary>
		public XmlFileDataSource(DataSourceDefinition def) {
			cachedFile = def.CachedFile;
			fileName = def.Attributes["fileName"];
			if (fileName==null || fileName.Length < 1) throw new ApplicationException("XmlFileDataSource construction error. File name expected.");
			xsltName = def.Attributes["xsltName"];
			if (xsltName==null || xsltName.Length < 1) throw new ApplicationException("XmlFileDataSource construction error. XSLT name expected.");
		}

		/// <summary>Формирует кэшированный файл данных</summary>
		public override bool Build(HttpContext context) {
			if (!base.Build(context)) return false;

			Dictionary<string, string> trSettings = new Dictionary<string, string>();

			trSettings["contentFolder"] = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir;

			using (StreamWriter wrt = OpenFileStream()) {
				XsltProcessor xslt = new XsltProcessor();
				xslt.TransformDocument(@"\" + SiteSettings.Current.ContentDir + @"\"+fileName, @"\" + SiteSettings.Current.XsltDir + @"\"+xsltName, trSettings, wrt);
			}
			return true;
		}

		private string fileName;
		private string xsltName;
	}
}
