using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;
using System.Data.SqlClient;
using System.IO;

namespace Istra {
	/// <summary>Менеджер авторизованного скачивания файлов</summary>
	public class DownloadManager {

		/// <summary>Конструктор</summary>
		public DownloadManager() {
			// using (SqlConnection conn = new SqlConnection(DownloadSettings.Current.DbConn)) {
			// 	conn.Open();
			// 
			// 	// if (!CheckTable(conn))
			// 	// 	BuildTable(conn);
			// }
			
		}

		/// <summary>Открывает сессию получения файла, возвращает идентификатор сессии</summary>
		/// <param name="url">размещение скачиваемого файла</param>
		public string OpenSession(string url) {
			throw new NotImplementedException();
		}


		private static bool CheckTable(SqlConnection conn) {
			SqlCommand cmd = new SqlCommand(
				string.Format(@"select count(*) from dbo.sysobjects where name = '{0}'", DownloadSettings.Current.TableName),
				conn
			);
			int count = (int)cmd.ExecuteScalar();
			return count == 1;
		}

		private static void BuildTable(SqlConnection conn) {
			StringWriter sqlCmd = new StringWriter();
			sqlCmd.Write(@"CREATE TABLE {0} (", DownloadSettings.Current.TableName);
			sqlCmd.Write(@"ID Uniqueidentifier NOT NULL");
			sqlCmd.Write(@", Url varchar(512) NOT NULL");
			sqlCmd.Write(@", LastAccess DateTime NOT NULL");
			sqlCmd.Write(@", PRIMARY KEY(ID)");
			sqlCmd.Write(@")");
			SqlCommand cmd = new SqlCommand(sqlCmd.ToString(), conn);
			cmd.ExecuteNonQuery();
		}


		/// <summary>Настройки загрузки</summary>
		public class DownloadSettings {

			/// <summary>Возвращает текущие настройки</summary>
			public static DownloadSettings Current {
				get {
					if (instance == null)
						instance = new DownloadSettings();
					return instance;
				}
			}

			/// <summary>Строка подключения к БД</summary>
			public string DbConn { get { return dbConn; } }
			/// <summary>Имя таблицы БД</summary>
			public string TableName { get { return tableName; } }
			/// <summary>Таймаут сессии доступа к файлу (сек). Если отрицательный, сессии не закрываются</summary>
			public int Timeout { get { return timeout; } }

			/// <summary>Закрытый конструктор</summary>
			private DownloadSettings() {
				NameValueCollection settings = (NameValueCollection)ConfigurationManager.GetSection("Istra/Download");
				if (settings == null) return;
				foreach (string key in settings.AllKeys) {
					switch (key) {
						case "db": this.dbConn = settings["db"]; break;
						case "tableName": this.tableName = settings["tableName"]; break;
						case "timeout": this.timeout = Int32.Parse(settings["timeout"]); break;
						default: break;
					}
				}
			}

			private static DownloadSettings instance;

			/// <summary>Строка подключения к БД</summary>
			private string dbConn = null;
			/// <summary>Имя таблицы БД</summary>
			private string tableName = null;
			/// <summary>Таймаут сессии (сек) доступа к файлу. Если отрицательный, сессии не закрываются</summary>
			private int timeout = 600;

		}


		/// <summary>Обработчик конфигурации настроек сессии</summary>
		public class ConfigHandler : IConfigurationSectionHandler {
			/// <summary>Реализация интерфейса</summary>
			public object Create(object parent, object configContext, XmlNode section) {
				NameValueCollection res = new NameValueCollection();
				foreach(XmlAttribute att in section.Attributes){
					res[att.Name] = att.Value;
				}
				return res;
			}
		}

	}
}
