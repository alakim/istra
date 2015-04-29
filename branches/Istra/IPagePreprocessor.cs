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
}
