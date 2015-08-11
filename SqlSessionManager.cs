using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Istra {
	/// <summary>Менеджер сессий, хранимых в БД</summary>
	public class SqlSessionManager : IUserSessionManager {

		/// <summary>Конструктор</summary>
		/// <param name="connString">строка соединения с БД</param>
		/// <param name="tableName">имя таблицы БД для хранения сессий</param>
		/// <param name="sessionTimeout">таймаут пользовательской сессии (сек)</param>
		public SqlSessionManager(SessionsSettings settings) {
			this.connString = settings.DbConn;
			this.tableName = settings.TableName;
			this.sessionTimeout = settings.Timeout;
		}

		/// <summary>Строка соединения с БД</summary>
		private string connString;
		/// <summary>Имя таблицы БД для хранения сессий</summary>
		private string tableName;
		/// <summary>Таймаут пользовательской сессии (сек)</summary>
		private int sessionTimeout;

		/// <summary>Открывает сессию</summary>
		/// <param name="userID">идентификатор пользователя</param>
		/// <param name="userName">отображаемое имя пользователя</param>
		/// <returns>идентификатор сессии</returns>
		public string Open(string userID, string userName) {
			using (SqlConnection conn = new SqlConnection(connString)) {
				conn.Open();
				if (!CheckTable(conn))
					BuildTable(conn);
				Guid sessionID = Guid.NewGuid();
				SqlCommand cmd = new SqlCommand(string.Format(
					@"INSERT INTO {4} (ID, UserID, UserName, LastAccess) VALUES ('{{{0}}}', '{{{1}}}', '{2}', '{3}')",
					sessionID, userID, userName, DateTime.Now, tableName
				), conn);
				cmd.ExecuteNonQuery();
				return sessionID.ToString("N");
			}
		}

		/// <summary>Проверяет сессию</summary>
		/// <param name="sessionID">идентификатор сессии</param>
		public bool Check(string sessionID) {
			using (SqlConnection conn = new SqlConnection(connString)) {
				conn.Open();
				if (!CheckTable(conn)) return false;
				CloseOldSessions(conn);
				Guid sID;
				string sssID;
				try {
					sID = new Guid(sessionID);
					sssID = (sID).ToString("D");
				}
				catch (Exception err) {
					throw new ApplicationException("Bad session ID '" + sessionID + "'", err);
				}
				SqlCommand cmd = new SqlCommand(string.Format(
					@"select LastAccess from {0} where id='{{{1}}}'",
					tableName, sssID
				), conn);
				object dt = cmd.ExecuteScalar();
				if (dt == null || dt == DBNull.Value) return false;
				DateTime lastAccess = (DateTime)dt;
				if (sessionTimeout > 0 && DateTime.Now > lastAccess.AddSeconds(sessionTimeout)) {
					Close(sessionID, conn);
					return false;
				}
				else {
					SqlCommand cmdProlong = new SqlCommand(string.Format(
						@"UPDATE {0} SET LastAccess = '{1}' WHERE ID = '{{{2}}}'",
						tableName, DateTime.Now, sssID
					), conn);
					cmdProlong.ExecuteNonQuery();
				}
				return true;
			}
		}

		/// <summary>Закрывает старые версии</summary>
		public void CloseOldSessions(SqlConnection conn) {
			if (sessionTimeout < 0) return;
			SqlCommand cmd = new SqlCommand(string.Format(
				@"DELETE FROM {0} WHERE LastAccess < '{1}'",
				tableName, DateTime.Now.AddSeconds(-sessionTimeout)
			), conn);
			cmd.ExecuteNonQuery();
		}

		/// <summary>Закрывает сессию</summary>
		public void Close(object sessionID) {
			if (sessionID == null) return;
			using (SqlConnection conn = new SqlConnection(connString)) {
				conn.Open();
				Close(sessionID.ToString(), conn);
			}
		}

		/// <summary>Возвращает идентификатор пользователя</summary>
		/// <param name="sessionID">идентификатор сессии</param>
		public string GetUserID(string sessionID) {
			using (SqlConnection conn = new SqlConnection(connString)) {
				conn.Open();
				Guid sID;
				string sssID;
				try {
					sID = new Guid(sessionID);
					sssID = (sID).ToString("D");
				}
				catch (Exception err) {
					throw new ApplicationException("Bad session ID '"+sessionID+"'", err);
				}
				SqlCommand cmd = new SqlCommand(string.Format(
					@"select UserID from {0} where id='{{{1}}}'",
					tableName, sssID
				), conn);
				object dt = cmd.ExecuteScalar();
				if (dt == null || dt == DBNull.Value) return string.Empty;
				return dt.ToString();
			}
		}


		/// <summary>Возвращает список открытых сессий</summary>
		public List<SessionData> GetSessions() {
			List<SessionData> res = new List<SessionData>();
			using (SqlConnection conn = new SqlConnection(connString)) {
				conn.Open();
				SqlCommand cmd = new SqlCommand(string.Format(
					@"select * from {0}",
					tableName
				), conn);
				using (SqlDataAdapter adp = new SqlDataAdapter(cmd)) {
					DataTable dt = new DataTable();
					adp.Fill(dt);

					foreach (DataRow row in dt.Rows) {
						SessionData data = new SessionData(
							(Guid)row["ID"],
							(Guid)row["UserID"],
							(string)row["UserName"],
							(DateTime)row["LastAccess"]
						);
						res.Add(data);
					}
				}
			}
			return res;
		}


		/// <summary>Таймаут пользовательской сессии (сек)</summary>
		public int SessionTimeout {
			get {
				return sessionTimeout;
			}
		}



		/// <summary>Закрывает сессию</summary>
		private void Close(string sessionID, SqlConnection conn) {
			Guid sID = new Guid(sessionID);
			string sssID = sID.ToString("D");
			SqlCommand cmd = new SqlCommand(string.Format(
				@"DELETE FROM {0} WHERE ID='{{{1}}}'",
				tableName, sssID
			), conn);
			cmd.ExecuteNonQuery();
		}

		private bool CheckTable(SqlConnection conn) {
			SqlCommand cmd = new SqlCommand(
				string.Format(@"select count(*) from dbo.sysobjects where name = '{0}'", tableName),
				conn
			);
			int count = (int)cmd.ExecuteScalar();
			return count == 1;
		}

		private void BuildTable(SqlConnection conn) {
			StringWriter sqlCmd = new StringWriter();
			sqlCmd.Write(@"CREATE TABLE {0} (", tableName);
			sqlCmd.Write(@"ID Uniqueidentifier NOT NULL");
			sqlCmd.Write(@", UserID Uniqueidentifier NOT NULL");
			sqlCmd.Write(@", UserName varchar(255) NOT NULL");
			sqlCmd.Write(@", LastAccess DateTime NOT NULL");
			sqlCmd.Write(@", PRIMARY KEY(ID)");
			sqlCmd.Write(@")");
			SqlCommand cmd = new SqlCommand(sqlCmd.ToString(), conn);
			cmd.ExecuteNonQuery();
		}


	}
}
