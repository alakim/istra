using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Istra.WS {
	public class SaveData : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string docFile = Request["file"];
			string xml = Request["xml"];

			if (docFile == null) {
				WriteError("Не указан файл для сохранения", writer);
				return;
			}

			docFile = docFile.Replace(@"*", Guid.NewGuid().ToString("N"))
				.Replace(@"%", DateTime.Now.Year+@"\"+DateTime.Now.Month+@"\");

			FileOperationsUtility.SaveXml(docFile, xml, this, new DateFormatter());
			WriteSuccess(writer);
		}
	}
}
