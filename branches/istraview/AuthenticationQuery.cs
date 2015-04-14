using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace istraview {
	public class AuthenticationQuery : Istra.IQuery {

		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			if (context.Request["login"] != null) {
				context.Session["sessionID"] = context.Request["login"] + ":" + context.Request["password"];
			}
			if (context.Session["sessionID"] != null) {
				XmlElement sessionRoot = doc.CreateElement("session");
				xQuery.ParentNode.InsertAfter(sessionRoot, xQuery);
				xQuery.ParentNode.RemoveChild(xQuery);
				Istra.XmlUtility.AddAttribute(doc, sessionRoot, "sessionID", context.Session["sessionID"].ToString());
			}
			else {
				XmlNode attTarget = xQuery.Attributes.GetNamedItem("target");
				if (attTarget == null) {
					Istra.XmlUtility.WriteError(doc, "AuthenticationQuery/@target attribute requred");
					return;
				}

				XmlElement dialogRoot = doc.CreateElement("AuthDialog");
				xQuery.ParentNode.InsertAfter(dialogRoot, xQuery);
				xQuery.ParentNode.RemoveChild(xQuery);
				Istra.XmlUtility.AddAttribute(doc, dialogRoot, "target", attTarget.Value);
			}

		}
	}
}