using System;
using System.Collections.Generic;
using System.Text;

namespace Istra.WS {
	/// <summary>Принудительно закрывает сессию</summary>
	public class CloseSession : Istra.WS.WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string sessionID = Request["id"];

			IUserSessionManager manager = SiteSettings.Current.GetSessionManager();
			manager.Close(sessionID);

			WriteSuccess(writer);
		}
	}
}
