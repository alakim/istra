using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Istra {
	/// <summary>Интерфейс запросов, выполняемых со страницы</summary>
	public interface IQuery {
		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="context">контекст веб-приложения</param>
		void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context);
	}
}
