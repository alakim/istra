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
				writer.Write(@"{""error"":""Authentication required!""}");
				return false;
			}
			return true;
		}
	}
}
