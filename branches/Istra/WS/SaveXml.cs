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

			string filePath = Istra.SiteSettings.Current.RootDir + docFile;
			content = JsonUtility.RestoreXmlMarkup(content);

			try {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(content);
			}
			catch (Exception err) {
				WriteError("Ошибка синтаксического разбора документа", err, writer);
				return;
			}

			try {
				File.WriteAllText(filePath, content);
				writer.Write(@"{""success"":true}");
			}
			catch (Exception err) {
				WriteError("Ошибка сохранения документа", err, writer);
			}
		}

		/// <summary>Выводит сообщение об ошибке</summary>
		/// <param name="title">заголовок сообщения</param>
		/// <param name="err">исключение</param>
		/// <param name="writer">компонент вывода</param>
		private static void WriteError(string title, Exception err, System.Web.UI.HtmlTextWriter writer) {
			writer.Write(@"{{""error"":""{0}: \n{1}""}}", title, JsonUtility.PrepareString(err.Message, true));
		}
	}
}
