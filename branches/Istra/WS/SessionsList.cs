using System;
using System.Collections.Generic;
using System.Text;

namespace Istra.WS {
	/// <summary>Возвращает данные открытых сессий</summary>
	public class SessionsList : Istra.WS.WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			IUserSessionManager manager = SiteSettings.Current.GetSessionManager();

			try {
				List<SessionData> sessions = manager.GetSessions();
				writer.Write(@"{""sessions"":[");
				bool first = true;
				foreach (SessionData data in sessions) {
					if (first) first = false; else writer.Write(",");
					writer.Write(
						@"{{""id"":""{0}"",""userID"":""{1}"",""userName"":""{2}"",""lastAccess"":""{3}""}}",
						data.SessionID, data.UserID, data.UserName, data.LastAccess
					);
				}
				writer.Write("]");

				writer.Write(@",""time"":""{0}""", JsonUtility.FormatDate(DateTime.Now));

				writer.Write(@",""timeout"":{0}", manager.SessionTimeout);

				writer.Write("}");
			}
			catch (Exception err) {
				WriteError("Ошибка получения списка сессий", err, writer);
			}
		}
	}
}
