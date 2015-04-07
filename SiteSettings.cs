using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

namespace Istra {
	/// <summary>Настройки сайта</summary>
	class SiteSettings {

		/// <summary>Возвращает текущие настройки</summary>
		public static SiteSettings Current {
			get {
				if (instance == null)
					instance = new SiteSettings();
				return instance;
			}
		}

		/// <summary>Директори приложения</summary>
		public string RootDir{get{return rootDir;}}
		/// <summary>Директория для размещения конетента</summary>
		public string ContentDir { get { return contentDir; } }
		/// <summary>Директория для размещения кэша</summary>
		public string CacheDir { get { return cacheDir; } }
		/// <summary>Директория для размещения XSLT-преобразований</summary>
		public string XsltDir { get { return xsltDir; } }
		/// <summary>Интервал кэширования по умолчанию (сек)</summary>
		public int CacheTime { get { return cacheTime; } }
		/// <summary>Страница по умолчанию</summary>
		public string DefaultPage { get { return defaultPage; } }
		/// <summary>Настройки источников данных</summary>
		public DataSourceDefinition[] Sources { get { return sources; } }

		/// <summary>Закрытый конструктор</summary>
		private SiteSettings() {
			NameValueCollection settings = (NameValueCollection)ConfigurationManager.GetSection("Istra/XsltSettings");
			rootDir = settings["rootDir"].ToString();
			contentDir = settings["contentDir"].ToString();
			cacheDir = settings["cacheDir"].ToString();
			xsltDir = settings["xsltDir"].ToString();
			cacheTime = Int32.Parse(settings["cacheTime"]);
			defaultPage = settings["defaultPage"].ToString();

			sources = (DataSourceDefinition[])ConfigurationManager.GetSection("Istra/DataSources");


		}

		/// <summary>Директори приложения</summary>
		private string rootDir;
		/// <summary>Директория для размещения конетента</summary>
		private string contentDir;
		/// <summary>Директория для размещения кэша</summary>
		private string cacheDir;
		/// <summary>Директория для размещения XSLT-преобразований</summary>
		private string xsltDir;
		/// <summary>Интервал кэширования по умолчанию (сек)</summary>
		private int cacheTime;
		/// <summary>Страница по умолчанию</summary>
		private string defaultPage;
		/// <summary>Настройки источников данных</summary>
		private DataSourceDefinition[] sources;

		/// <summary>Текущие настройки</summary>
		private static SiteSettings instance;

	}
}
