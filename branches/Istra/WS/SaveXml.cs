using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Istra.WS {
	public class SaveXml : WebService {

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
				FileOperationsUtility.SaveXml(docFile, content, this);
				WriteSuccess(writer);
			}
			catch (Exception err) {
				WriteError("Ошибка сохранения документа", err, writer);
			}
		}


	}
}
