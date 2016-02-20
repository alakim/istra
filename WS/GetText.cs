using System;
using System.Collections.Generic;
using System.Text;

namespace Istra.WS {
	/// <summary>Загружает текстовые-данные</summary>
	public class GetText : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string docFile = Request["doc"];

			string content = FileOperationsUtility.GetText(docFile);
			writer.Write(content);
		}
	}
}
