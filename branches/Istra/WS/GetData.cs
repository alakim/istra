using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Istra.WS {
	public class GetData : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string docFile = Request["doc"];
			string query = Request["query"];


			string filePath = Istra.SiteSettings.Current.RootDir + docFile;
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);

			bool first = true;
			writer.Write("[");
			foreach (XmlNode nd in doc.SelectNodes(query)) {
				if (first) first = false; else writer.Write(",");
				writer.Write(JsonUtility.Serialize(nd));
			}
			writer.Write("]");

			writer.Close();
		}
	}
}
