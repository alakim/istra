using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace istraview {
	public class TestNewsQuery : Istra.IQuery {
		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			XmlElement newsRoot = doc.CreateElement("news");
			xQuery.ParentNode.InsertAfter(newsRoot, xQuery);
			xQuery.ParentNode.RemoveChild(xQuery);

			for (int i = 0; i < 5; i++) {
				XmlElement msg = doc.CreateElement("message");
				Istra.XmlUtility.AddAttribute(doc, msg, "date", "2015.03." + (10 + i));
				Istra.XmlUtility.AddAttribute(doc, msg, "title", "Новость №" + (i + 1));
				newsRoot.AppendChild(msg);
			}
		}




	}
}