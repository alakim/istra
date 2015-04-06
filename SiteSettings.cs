using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

namespace Istra {
	/// <summary>Настройки сайта</summary>
	class SiteSettings {

		/// <summary>Директори приложения</summary>
		public string RootDir{get{return rootDir;}}
		/// <summary>Директория для размещения конетента</summary>
		public string ContentDir { get { return contentDir; } }
		/// <summary>Директория для размещения кэша</summary>
		public string CacheDir { get { return cacheDir; } }
		/// <summary>Директория для размещения XSLT-преобразований</summary>
		public string XsltDir { get { return xsltDir; } }

		/// <summary>Конструктор</summary>
		public SiteSettings() {
			NameValueCollection settings = (NameValueCollection)ConfigurationManager.GetSection("Istra/XsltSettings");
			rootDir = settings["rootDir"].ToString();
			contentDir = settings["contentDir"].ToString();
			cacheDir = settings["cacheDir"].ToString();
			xsltDir = settings["xsltDir"].ToString();

		}

		/// <summary>Директори приложения</summary>
		private string rootDir;
		/// <summary>Директория для размещения конетента</summary>
		private string contentDir;
		/// <summary>Директория для размещения кэша</summary>
		private string cacheDir;
		/// <summary>Директория для размещения XSLT-преобразований</summary>
		private string xsltDir;

	}
}
