using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Istra {
	/// <summary>Выполняет финальное преобразование кэша источников данных</summary>
	public interface IPostprocessor {
		/// <summary>Выполняет преобразование</summary>
		/// <param name="xDoc">обрабатываемый XML-документ</param>
		void Process(XmlDocument xDoc);
	}
}
