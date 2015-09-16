using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;

namespace Istra {
	/// <summary>Настройки сайта</summary>
	public class SiteSettings {

		/// <summary>Возвращает текущие настройки</summary>
		public static SiteSettings Current {
			get {
				if (!instances.ContainsKey(Key) || instances[Key] == null)
					instances[Key] = new SiteSettings();
				return instances[Key];
			}
		}

		/// <summary>Ключ для получения текущих настроек в данной директории</summary>
		public static string Key = "default";

		/// <summary>Таймаут для ожидания доступа по мьютексу</summary>
		public const int mutexTimeout = 3000;

		/// <summary>Директори приложения</summary>
		public string RootDir{get{return rootDir;}}
		/// <summary>Директория для размещения конетента</summary>
		public string ContentDir { get { return contentDir; } }
		/// <summary>Директория для размещения протокола ошибок</summary>
		public string LogDir { get { return logDir; } }
		/// <summary>Включает протоколирование ошибок типа AbortInternal</summary>
		public bool LogInternalAborts { get { return logInternalAborts; } }
		/// <summary>Разрешает вывод необработанного XML-документа</summary>
		public bool AllowRawOutput { get { return allowRawOutput; } }
		/// <summary>Директория для размещения кэша</summary>
		public string CacheDir { get { return cacheDir; } }
		/// <summary>Директория для размещения XSLT-преобразований</summary>
		public string XsltDir { get { return xsltDir; } }
		/// <summary>Интервал кэширования по умолчанию (сек)</summary>
		public int CacheTime { get { return cacheTime; } }
		/// <summary>Страница по умолчанию</summary>
		public string DefaultPage { get { return defaultPage; } }
		/// <summary>Регулярное выражения для распознавания мобильного устройства</summary>
		public string MobileDetection { get { return mobileDetection; } }
		/// <summary>Препроцессор веб-страницы</summary>
		public IPagePreprocessor Preprocessor { get { return preprocessor; } }

		/// <summary>Настройки источников данных</summary>
		public DataSourceDefinition[] Sources { get { return sources; } }

		/// <summary>Возвращает экземпляр менеджера сессий</summary>
		/// <returns></returns>
		public IUserSessionManager GetSessionManager() {
			return sessionSettings.GetSessionManager();
		}

		/// <summary>Возвращает экземпляр провайдера доступа</summary>
		///<param name="userID">идентификатор пользователя</param>
		public IAccessProvider GetAccessProvider(string userID) {
			return sessionSettings.GetAccessProvider(userID);
		}

		/// <summary>Закрытый конструктор</summary>
		private SiteSettings() {
			NameValueCollection settings = (NameValueCollection)ConfigurationManager.GetSection("Istra/XsltSettings");
			rootDir = settings["rootDir"].ToString();
			contentDir = settings["contentDir"].ToString();
			logDir = settings["logDir"].ToString();
			if(settings["logInternalAborts"]!=null)
				logInternalAborts = settings["logInternalAborts"].ToLower()=="true";
			if (settings["allowRawOutput"] != null)
				allowRawOutput = settings["allowRawOutput"].ToLower() == "true";
			cacheDir = settings["cacheDir"].ToString();
			xsltDir = settings["xsltDir"].ToString();
			cacheTime = Int32.Parse(settings["cacheTime"]);
			defaultPage = settings["defaultPage"].ToString();
			if (settings["mobileDetection"]!=null)
				mobileDetection = settings["mobileDetection"].ToString();
			if (settings["preprocessor"]!=null) {
				Type t = Type.GetType(settings["preprocessor"].ToString());
				ConstructorInfo cInf = t.GetConstructor(new Type[0] { });
				preprocessor = (IPagePreprocessor)cInf.Invoke(new object[0] { });
			}

			sources = (DataSourceDefinition[])ConfigurationManager.GetSection("Istra/DataSources");
			sessionSettings = new SessionsSettings();
		}

		/// <summary>Директори приложения</summary>
		private string rootDir;
		/// <summary>Директория для размещения конетента</summary>
		private string contentDir;
		/// <summary>Директория для размещения протокола ошибок</summary>
		private string logDir;
		/// <summary>Включает протоколирование ошибок типа AbortInternal</summary>
		private bool logInternalAborts = false;
		/// <summary>Разрешает вывод необработанного XML-документа</summary>
		private bool allowRawOutput = false;
		/// <summary>Директория для размещения кэша</summary>
		private string cacheDir;
		/// <summary>Директория для размещения XSLT-преобразований</summary>
		private string xsltDir;
		/// <summary>Интервал кэширования по умолчанию (сек)</summary>
		private int cacheTime;
		/// <summary>Страница по умолчанию</summary>
		private string defaultPage;
		/// <summary>Регулярное выражения для распознавания мобильного устройства</summary>
		private string mobileDetection = null;
		/// <summary>Препроцессор веб-страницы</summary>
		private IPagePreprocessor preprocessor = null;
		/// <summary>Настройки источников данных</summary>
		private DataSourceDefinition[] sources;

		private SessionsSettings sessionSettings;

		/// <summary>Текущие настройки</summary>
		private static Dictionary<string, SiteSettings> instances = new Dictionary<string,SiteSettings>();

	}
}
