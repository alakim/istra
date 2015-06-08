using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Istra {
	/// <summary>Провайдер доступа к файлам</summary>
	public interface IAccessProvider {
		/// <summary>Возвращает уровень доступа к заданному файлу</summary>
		/// <param name="filePath">путь к файлу</param>
		AccessLevel GetRights(string filePath);
	}

	/// <summary>Уровень доступа к данным</summary>
	public enum AccessLevel {
		none, read, write
	}

	/// <summary>Провайдер доступа по умолчанию - предоставляет полный доступ всем</summary>
	public class DefaultAccessProvider : IAccessProvider {

		/// <summary>Конструктор</summary>
		/// <param name="userID">идентификтор пользователя</param>
		public DefaultAccessProvider(string userID) {
		}

		/// <summary>Возвращает уровень доступа к заданному файлу</summary>
		/// <param name="filePath">путь к файлу</param>
		public AccessLevel GetRights(string filePath) {
			return AccessLevel.write;
		}
	}


	/// <summary>Провайдер доступа с хранением данных в XML-документе</summary>
	public class XmlAccessProvider : IAccessProvider {
		/// <summary>Файл для хранения настроек</summary>
		private const string fileName = "users.xml";

		/// <summary>Конструктор</summary>
		/// <param name="userID">идентификтор пользователя</param>
		public XmlAccessProvider(string userID) {
			this.userID = userID;
			XmlDocument doc = new XmlDocument();
			doc.Load(SiteSettings.Current.RootDir+@"\"+fileName);
			XmlNodeList nodes = doc.SelectNodes(@"//user[@id='"+userID.ToUpper()+"']");
			if (nodes.Count > 0)
				userSettings = nodes[0];
		}

		/// <summary>Возвращает уровень доступа к заданному файлу</summary>
		/// <param name="filePath">путь к файлу</param>
		public AccessLevel GetRights(string filePath) {
			AccessLevel level = AccessLevel.none;
			filePath = filePath.Replace(SiteSettings.Current.RootDir+@"\", string.Empty)
				.Replace(@"\", "/");
			foreach (XmlNode access in userSettings.SelectNodes("rights/access")) {
				string path = access.Attributes.GetNamedItem("path").Value,
					right = access.Attributes.GetNamedItem("right").Value.ToLower();
				Regex rePath = new Regex(path, RegexOptions.IgnoreCase);
				if (rePath.Match(filePath).Success) {
					level = GetAccessLevel(right);
				}
			}
			return level;
		}

		/// <summary>Возвращает уровень доступа, закодированный в строке</summary>
		/// <param name="rights">строка - код уровня доступа</param>
		private AccessLevel GetAccessLevel(string rights) {
			bool allowRead = false,
				allowWrite = false;
			for (int i = 0; i < rights.Length; i++) {
				switch (rights[i]) {
					case 'r': allowRead = true; break;
					case 'w': allowWrite = true; break;
					default: break;
				}
			}
			return allowWrite ? AccessLevel.write
				: allowRead ? AccessLevel.read
				: AccessLevel.none;
		}

		/// <summary>Идентификатор пользователя</summary>
		private string userID;
		/// <summary>Настройки пользователя</summary>
		private XmlNode userSettings = null;
	}
}
