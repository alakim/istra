using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Reflection;

namespace Istra {
	/// <summary>Менеджер пользовательских сессий</summary>
	public interface IUserSessionManager {
		/// <summary>Открывает сессию</summary>
		/// <param name="userID">идентификатор пользователя</param>
		/// <param name="userName">отображаемое имя пользователя</param>
		/// <returns>идентификатор сессии</returns>
		string Open(string userID, string userName);

		
		/// <summary>Проверяет сессию</summary>
		/// <param name="sessionID">идентификатор сессии</param>
		bool Check(string sessionID);

		
		/// <summary>Закрывает сессию</summary>
		void Close(object sessionID);
	}

	/// <summary>Менеджер-заглушка для случая отсутствия поддержки сессий</summary>
	public class EmptySessionManager : IUserSessionManager {
		/// <summary>Псевдо-идентификатор сессии</summary>
		private const string pseudoSessionID = "emptysession";

		public EmptySessionManager(SessionsSettings settings) {
		}

		/// <summary>Открывает сессию</summary>
		/// <param name="userID">идентификатор пользователя</param>
		/// <param name="userName">отображаемое имя пользователя</param>
		/// <returns>идентификатор сессии</returns>
		public string Open(string userID, string userName) {
			return pseudoSessionID;
		}

		
		/// <summary>Проверяет сессию</summary>
		/// <param name="sessionID">идентификатор сессии</param>
		public bool Check(string sessionID) {
			return sessionID == pseudoSessionID;
		}

		
		/// <summary>Закрывает сессию</summary>
		public void Close(object sessionID) {
		}
	}

		/// <summary>Настройки управления сессиями</summary>
	public class SessionsSettings {
		/// <summary>Конструктор</summary>
		public SessionsSettings() {
			NameValueCollection settings = (NameValueCollection)ConfigurationManager.GetSection("Istra/UserSessions");
			if (settings == null) return;

			if (settings["manager"] != null)
				this.className = settings["class"];
			if (settings["db"] != null)
				this.dbConn = settings["db"];
			if (settings["tableName"] != null)
				this.tableName = settings["tableName"];
			if (settings["timeout"] != null)
				this.timeout = Int32.Parse(settings["timeout"]);
		}

		/// <summary>Возвращает экземпляр менеджера сессий</summary>
		/// <returns></returns>
		public IUserSessionManager GetSessionManager() {
			if (className == null || className.Length == 0)
				return new EmptySessionManager(this);
			Type t = Type.GetType(className);
			ConstructorInfo cInf = t.GetConstructor(new Type[1] {typeof(SessionsSettings)});
			return (IUserSessionManager)cInf.Invoke(new object[1] {this});
		}

		/// <summary>Строка подключения к БД</summary>
		public string DbConn { get { return dbConn; } }
		/// <summary>Имя таблицы БД</summary>
		public string TableName { get { return tableName; } }
		/// <summary>Таймаут сессии (сек)</summary>
		public int Timeout { get { return timeout; } }

		/// <summary>Имя класса менеджера сессии</summary>
		private string className = null;
		/// <summary>Строка подключения к БД</summary>
		private string dbConn = null;
		/// <summary>Имя таблицы БД</summary>
		private string tableName = null;
		/// <summary>Таймаут сессии (сек)</summary>
		private int timeout = 600;

	}

	/// <summary>Обработчик конфигурации настроек сессии</summary>
	public class SessionsConfigHandler : IConfigurationSectionHandler {
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
