using System;
using System.Collections.Generic;
using System.Text;

namespace Istra.WS {
	/// <summary>Веб-служба</summary>
	public class WebService : System.Web.UI.Page {
		/// <summary>Вызывает проверку пользовательской сессии</summary>
		/// <param name="writer">компонент вывода</param>
		public bool CheckSession(System.Web.UI.HtmlTextWriter writer) {
			string sessionID = Request["sessionID"];
			IUserSessionManager session = SiteSettings.Current.GetSessionManager();
			if (!session.Check(sessionID)) {
				WriteError("Authentication required!", writer);
				return false;
			}
			return true;
		}

		/// <summary>Возвращает провайдер доступа к данным</summary>
		public IAccessProvider GetAccessProvider() {
			IUserSessionManager session = SiteSettings.Current.GetSessionManager();
			string sessionID = Request["sessionID"];
			string userID = session.GetUserID(sessionID);

			return SiteSettings.Current.GetAccessProvider(userID);
		}

		/// <summary>Выводит сообщение об ошибке</summary>
		/// <param name="title">заголовок сообщения</param>
		/// <param name="err">исключение</param>
		/// <param name="writer">компонент вывода</param>
		public virtual void WriteError(string title, Exception err, System.Web.UI.HtmlTextWriter writer) {
			writer.Write(@"{{""error"":""{0}: \n{1}""}}", title, JsonUtility.PrepareString(err.Message, true));
		}
		/// <summary>Выводит сообщение об ошибке</summary>
		/// <param name="title">заголовок сообщения</param>
		/// <param name="writer">компонент вывода</param>
		public virtual void WriteError(string title, System.Web.UI.HtmlTextWriter writer) {
			writer.Write(@"{{""error"":""{0}""}}", title);
		}

		/// <summary>Выводит сообщение об успешном завершенни операции</summary>
		public virtual void WriteSuccess(System.Web.UI.HtmlTextWriter writer) {
			writer.Write(@"{""success"":true}");
		}
	}
}
