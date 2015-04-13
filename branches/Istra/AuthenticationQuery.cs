using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Istra {
	public class AuthenticationQuery : IQuery {

		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			if (context.Session["sessionID"]!=null) {
			}
			else {
				XmlNode attTarget = xQuery.Attributes.GetNamedItem("target");
				if (attTarget == null) {
					XmlUtility.WriteError(doc, "AuthenticationQuery/@target attribute requred");
					return;
				}

				XmlElement dialogRoot = doc.CreateElement("AuthDialog");
				xQuery.ParentNode.InsertAfter(dialogRoot, xQuery);
				xQuery.ParentNode.RemoveChild(xQuery);
				XmlUtility.AddAttribute(doc, dialogRoot, "target", attTarget.Value);
			}
			// XmlElement sessionRoot = doc.CreateElement("session");
			// xQuery.ParentNode.InsertAfter(sessionRoot, xQuery);
			// xQuery.ParentNode.RemoveChild(xQuery);
			// 
			// foreach (string key in context.Session.Keys) {
			// 	XmlElement param = doc.CreateElement("param");
			// 	XmlUtility.AddAttribute(doc, param, "name", key);
			// 	XmlUtility.AddAttribute(doc, param, "value", context.Session[key].ToString());
			// 	sessionRoot.AppendChild(param);
			// }
		}
	}
}
