using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Istra {
	/// <summary>Препроцессор контента веб-страниц</summary>
	public interface IPagePreprocessor {
		/// <summary>Выполняет обработку страницы</summary>
		/// <param name="pageDoc">контент страницы</param>
		void Process(XmlDocument pageDoc);
	}

	/// <summary>Выполняет единообразное форматирование всех атрибутов @date</summary>
	public class DateFormatter : IPagePreprocessor {
		/// <summary>Выполняет обработку страницы</summary>
		/// <param name="pageDoc">контент страницы</param>
		public void Process(XmlDocument pageDoc) {
			XmlUtility.FormatDates(pageDoc.SelectNodes(@"//@date"));
		}
	}
}
