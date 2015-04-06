using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Istra {
	/// <summary>Меню сайта</summary>
	class MenuDataSource : DataSource {
		/// <summary>Конструктор</summary>
		public MenuDataSource() {
			cachedFile = "menu.xml";
		}

		/// <summary>Формирует кэшированный файл данных</summary>
		public override bool Build() {
			if (!base.Build()) return false;

			Dictionary<string, string> trSettings = new Dictionary<string, string>();

			trSettings["contentFolder"] = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir;

			using (StreamWriter wrt = OpenFileStream()) {
				XsltProcessor xslt = new XsltProcessor();
				xslt.TransformDocument(@"\" + SiteSettings.Current.ContentDir + @"\toc.xml", @"\" + SiteSettings.Current.XsltDir + @"\menu.xslt", trSettings, wrt);
			}
			return true;
		}

	}
}
