using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace istraview {
	public class DemoPagePreprocessor : Istra.IPagePreprocessor {
		/// <summary>Выполняет обработку страницы</summary>
		/// <param name="pageDoc">контент страницы</param>
		public void Process(XmlDocument pageDoc) {
			XmlNodeList paragraphs = pageDoc.SelectNodes(@"//p");
			foreach (XmlNode par in paragraphs) {
				par.InnerXml = "[modified by DemoPagePreprocessor] " + par.InnerXml;
			}
		}
	}
}