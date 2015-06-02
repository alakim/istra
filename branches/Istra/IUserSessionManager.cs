using System;
using System.Collections.Generic;
using System.Text;

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
		/// <summary>Открывает сессию</summary>
		/// <param name="userID">идентификатор пользователя</param>
		/// <param name="userName">отображаемое имя пользователя</param>
		/// <returns>идентификатор сессии</returns>
		public string Open(string userID, string userName) {
			return Guid.NewGuid().ToString("N");
		}

		
		/// <summary>Проверяет сессию</summary>
		/// <param name="sessionID">идентификатор сессии</param>
		public bool Check(string sessionID) {
			return true;
		}

		
		/// <summary>Закрывает сессию</summary>
		public void Close(object sessionID) {
		}
	}
}
