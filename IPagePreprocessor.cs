using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;

namespace Istra {
	/// <summary>Препроцессор контента веб-страниц</summary>
	public interface IPagePreprocessor {
		/// <summary>Выполняет обработку страницы</summary>
		/// <param name="pageDoc">контент страницы</param>
		void Process(XmlDocument pageDoc);
		/// <summary>Выполняет обработку страницы</summary>
		/// <param name="pageDoc">контент страницы</param>
		/// <param name="context">контекст веб-приложения</param>
		void Process(XmlDocument pageDoc, HttpContext context);
	}

	/// <summary>Выполняет единообразное форматирование всех атрибутов @date</summary>
	public class DateFormatter : IPagePreprocessor {
		/// <summary>Выполняет обработку страницы</summary>
		/// <param name="pageDoc">контент страницы</param>
		public void Process(XmlDocument pageDoc) {
			Process(pageDoc, null);
		}
		/// <summary>Выполняет обработку страницы</summary>
		/// <param name="pageDoc">контент страницы</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Process(XmlDocument pageDoc, HttpContext context) {
			XmlUtility.FormatDates(pageDoc.SelectNodes(@"//@date"));
		}

	}
}
