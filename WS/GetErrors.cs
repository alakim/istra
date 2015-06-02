using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Istra.WS {
	/// <summary>Возвращает протокол ошибок</summary>
	public class GetErrors : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			XmlDocument protocol = Istra.ErrorLog.GetProtocol();
			string json = JsonUtility.Serialize(protocol.DocumentElement);
			writer.Write(json);
		}

	}
}
