using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Istra {
	public class SessionQuery : IQuery {

		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			XmlElement sessionRoot = doc.CreateElement("session");
			doc.DocumentElement.InsertAfter(sessionRoot, xQuery);
			doc.DocumentElement.RemoveChild(xQuery);

			foreach (string key in context.Session.Keys) {
				XmlElement param = doc.CreateElement("param");
				XmlUtility.AddAttribute(doc, param, "name", key);
				XmlUtility.AddAttribute(doc, param, "value", context.Session[key].ToString());
				sessionRoot.AppendChild(param);
			}
		}

	}
}
