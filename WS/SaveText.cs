using System;
using System.Collections.Generic;
using System.Text;

namespace Istra.WS {

	public class SaveText : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string docFile = Request["doc"];
			string content = Request["content"];


			if (docFile == null) {
				WriteError("Не указан файл для сохранения", writer);
				return;
			}

			docFile = docFile.Replace(@"*", Guid.NewGuid().ToString("N"));


			try {
				FileOperationsUtility.SaveText(docFile, content, this);
				WriteSuccess(writer);
			}
			catch (Exception err) {
				WriteError("Ошибка сохранения документа", err, writer);
			}
		}


	}
}
