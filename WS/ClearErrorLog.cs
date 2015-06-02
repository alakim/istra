using System;
using System.Collections.Generic;
using System.Text;

namespace Istra.WS {
	public class ClearErrorLog : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			try {
				Istra.ErrorLog.Clear();
				writer.Write(@"{""success"":true}");
			}
			catch (Exception err) {
				writer.Write(@"{{""error"":{0}}}", JsonUtility.PrepareString(err.Message));
			}
		}

	}
}
