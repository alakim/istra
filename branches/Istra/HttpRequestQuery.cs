using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Collections.Specialized;

namespace Istra {
	/// <summary>Возвращает данные текущего HTTP-запроса</summary>
	/// <remarks>Атрибуты в XML-описании данного запроса могут задавать мэппинг имен параметров запроса во внутренние имена.
	///		Если мэппинг задан, то выводятся только определенные в нем параметры, если не задан - выводятся все параметры запроса.
	/// </remarks>
	public class HttpRequestQuery : IQuery {

		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="xQuery">XML-описание данного запроса</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			StringDictionary mapping = XmlUtility.GetAttributes(xQuery, "type");

			XmlElement requestRoot = doc.CreateElement("request");
			xQuery.ParentNode.InsertAfter(requestRoot, xQuery);
			xQuery.ParentNode.RemoveChild(xQuery);

			foreach (string key in context.Request.Params.Keys) {
				if(mapping.Keys.Count==0)
					XmlUtility.AddAttribute(doc, requestRoot, key, context.Request[key]);
				else if (mapping.ContainsKey(key))
					XmlUtility.AddAttribute(doc, requestRoot, mapping[key], context.Request[key]);
			}
		}


	}
}
