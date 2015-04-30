using System;
using System.Collections.Specialized;
using System.Web;
using System.Xml;
using Istra;

namespace istraview {
	/// <summary>Формирует данные сессии для отладки</summary>
	public class SampleSessionData : IQuery {
		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="xQuery">XML-описание данного запроса</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			xQuery.ParentNode.RemoveChild(xQuery);

			if (context.Session == null) return;

			context.Session["param1"] = "Parameter#1 Value";
			context.Session["param2"] = "Parameter#2 Value";
			context.Session["param3"] = "Parameter#3 Value";
		}

	}
}