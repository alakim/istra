using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace istraview {
	public class TestNewsQuery : Istra.IQuery {
		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, HttpContext context) {
			XmlElement newsRoot = doc.CreateElement("news");
			doc.DocumentElement.AppendChild(newsRoot);

			for (int i = 0; i < 5; i++) {
				XmlElement msg = doc.CreateElement("message");
				AddAttribute(doc, msg, "date", "2015.03."+(10+i));
				AddAttribute(doc, msg, "title", "Новость №"+(i+1));
				newsRoot.AppendChild(msg);
			}
		}



		private void AddAttribute(XmlDocument doc, XmlElement node, string name, string val) {
			XmlAttribute att = doc.CreateAttribute(name);
			att.Value = val;
			node.Attributes.Append(att);
		}

	}
}