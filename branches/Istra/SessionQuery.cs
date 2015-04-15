using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Collections.Specialized;

namespace Istra {
	/// <summary>Возвращает данные текущей сессии</summary>
	/// <remarks>
	///		Атрибуты в XML-описании данного запроса могут задавать мэппинг имен параметров сессии во внутренние имена.
	///		Если мэппинг задан, то выводятся только определенные в нем параметры, 
	///		если не задат - выводятся все параметры сессии.
	/// </remarks>
	public class SessionQuery : IQuery {

		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="xQuery">XML-описание данного запроса</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			StringDictionary mapping = XmlUtility.GetAttributes(xQuery, "type");

			XmlElement sessionRoot = doc.CreateElement("session");
			xQuery.ParentNode.InsertAfter(sessionRoot, xQuery);
			xQuery.ParentNode.RemoveChild(xQuery);

			foreach (string key in context.Session.Keys) {
				if (mapping.Keys.Count == 0)
					XmlUtility.AddAttribute(doc, sessionRoot, key, GetValue(context, key));
				else if (mapping.ContainsKey(key))
					XmlUtility.AddAttribute(doc, sessionRoot, mapping[key], GetValue(context, key));

			}
		}

		/// <summary>Возвращает значение параметра сессии в виде строки</summary>
		/// <param name="context">контекст веб-приложения</param>
		/// <param name="key">имя параметра сессии</param>
		private static string GetValue(HttpContext context, string key) {
			if (context.Session[key] != null) return context.Session[key].ToString();
			return string.Empty;
		}

	}
}
