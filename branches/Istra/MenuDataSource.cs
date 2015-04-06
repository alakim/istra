using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Istra {
	/// <summary>Меню сайта</summary>
	class MenuDataSource : DataSource {

		public void Build() {
			SiteSettings sSettings = new SiteSettings();
			string filePath = sSettings.RootDir + @"\" + sSettings.CacheDir + @"\menu.xml";
			File.Delete(filePath);

			Dictionary<string, string> trSettings = new Dictionary<string, string>();
			trSettings["contentFolder"] = sSettings.RootDir + @"\" + sSettings.ContentDir;

			StreamWriter wrt = new StreamWriter(filePath);
			XsltProcessor xslt = new XsltProcessor();
			xslt.TransformDocument(@"\" + sSettings.ContentDir + @"\toc.xml", @"\" + sSettings.XsltDir + @"\menu.xslt", trSettings, wrt);
			wrt.Close();
		}

	}
}
