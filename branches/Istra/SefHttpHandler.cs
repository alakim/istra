using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;

namespace Istra {
	/// <summary>ОБработчик ЧПУ</summary>
	class SefHttpHandler : IHttpHandler {
		public bool IsReusable { get { return true; } }

		private static Regex reUrl = new Regex(@"([^/.]+)\.html", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		public void ProcessRequest(HttpContext context) {
			string url = context.Request.Path;
			Match mt = reUrl.Match(url);
			if (mt.Success) {
				string newUrl = SefSettings.Current.DefaultPage + (mt.Groups[1].Value.ToLower()=="default"?string.Empty:("?p="+ mt.Groups[1].Value));
				context.Server.Transfer(newUrl);
			}
		}

	}

	/// <summary>Настройки режима ЧПУ</summary>
	public class SefSettings {

		/// <summary>Возвращает текущие настройки</summary>
		public static SefSettings Current {
			get {
				if (instance == null)
					instance = new SefSettings();
				return instance;
			}
		}

		private static SefSettings instance;

		/// <summary>Конструктор</summary>
		private SefSettings() {
			NameValueCollection settings = (NameValueCollection)ConfigurationManager.GetSection("Istra/SEF");
			this.defaultPage = settings["defaultPage"];
		}

		/// <summary>Страница по умолчанию</summary>
		public string DefaultPage { get { return defaultPage; } }

		private string defaultPage;
	}

	/// <summary>Обработчик настроек конфигурации</summary>
	public class SefConfigHandler : IConfigurationSectionHandler {
		/// <summary>Реализация интерфейса</summary>
		public object Create(object parent, object configContext, XmlNode section) {
			NameValueCollection res = new NameValueCollection();
			foreach (XmlNode ch in section.ChildNodes) {
				string key = ch.Attributes.GetNamedItem("key").Value;
				string val = ch.Attributes.GetNamedItem("value").Value;
				res[key] = val;
			}
			return res;
		}
	}
}
