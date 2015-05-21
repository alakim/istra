using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Reflection;
using System.IO;

namespace Istra {
	/// <summary>Выводит текст руководства по архитектуре Istra</summary>
	public class IstraManualQuery : IQuery {
		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="xQuery">XML-описание данного запроса</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			

			XmlElement requestRoot = doc.CreateElement("IstraManual");
			xQuery.ParentNode.InsertAfter(requestRoot, xQuery);
			xQuery.ParentNode.RemoveChild(xQuery);

			Assembly thisAsm = Assembly.GetExecutingAssembly();
			Stream stm = thisAsm.GetManifestResourceStream("Istra.manual.xml");
			StreamReader rdr = new StreamReader(stm);
			string xml = rdr.ReadToEnd();
			XmlDocument manual = new XmlDocument();
			manual.LoadXml(xml);
			AddDataSources(thisAsm, manual);
			AddQueries(thisAsm, manual);

			requestRoot.InnerXml = manual.DocumentElement.InnerXml;

			XmlUtility.AddAttribute(doc, requestRoot, "version", thisAsm.GetName().Version.ToString());
		}

		/// <summary>Формирует список доступных источников данных</summary>
		/// <param name="thisAsm">сборка</param>
		/// <param name="manual">документ руководства</param>
		private static void AddDataSources(Assembly thisAsm, XmlDocument manual) {
			XmlElement elSources = (XmlElement)manual.SelectSingleNode("//AvailableSources");
			Type tDS = thisAsm.GetType("Istra.DataSource");
			Type[] types = thisAsm.GetTypes();
			foreach (Type t in types) {
				if (t.BaseType == tDS) {
					XmlElement elDS = manual.CreateElement("type");
					elSources.AppendChild(elDS);
					XmlUtility.AddAttribute(manual, elDS, "name", t.Namespace + "." + t.Name);
				}
			}
		}

		/// <summary>Формирует список доступных запросов</summary>
		/// <param name="thisAsm">сборка</param>
		/// <param name="manual">документ руководства</param>
		private static void AddQueries(Assembly thisAsm, XmlDocument manual) {
			XmlElement elSources = (XmlElement)manual.SelectSingleNode("//AvailableQueries");
			Type tDS = thisAsm.GetType("Istra.IQuery");
			Type[] types = thisAsm.GetTypes();
			foreach (Type t in types) {
				if (t.GetInterface("Istra.IQuery") != null) { 
					XmlElement elDS = manual.CreateElement("type");
					elSources.AppendChild(elDS);
					XmlUtility.AddAttribute(manual, elDS, "name", t.Namespace+"."+t.Name);
				}
			}
		}


	}
}
