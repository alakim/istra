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
				writer.Write(@"{""error"":""Не указан файл для сохранения""}");
			}

			docFile = docFile.Replace(@"*", Guid.NewGuid().ToString("N"));

			string filePath = Istra.SiteSettings.Current.RootDir + docFile;
			xml = JsonUtility.RestoreXmlMarkup(xml);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlUtility.FormatDates(doc.SelectNodes(@"//@date"));

			doc.Save(filePath);
			writer.Write(@"{""success"":true}");
		}
	}
}
