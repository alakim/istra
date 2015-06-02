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

			AddTypes(thisAsm, manual, "Istra.DataSource", "//AvailableSources", false);
			AddTypes(thisAsm, manual, "Istra.IQuery", "//AvailableQueries", true);
			AddTypes(thisAsm, manual, "Istra.IUserSessionManager", "//AvailableSessionManagers", true);
			AddTypes(thisAsm, manual, "Istra.WS.WebService", "//AvailableWebServices", false);


			requestRoot.InnerXml = manual.DocumentElement.InnerXml;

			XmlUtility.AddAttribute(doc, requestRoot, "version", thisAsm.GetName().Version.ToString());
		}


		/// <summary>Формирует список доступных классов заданного типа</summary>
		/// <param name="thisAsm">сборка</param>
		/// <param name="manual">документ руководства</param>
		/// <param name="typeName">имя типа</param>
		/// <param name="nodePath">путь к узлу документации</param>
		/// <param name="interfaceMode">проверяет реализацию интерфейск</param>
		private static void AddTypes(Assembly thisAsm, XmlDocument manual, string typeName, string nodePath, bool interfaceMode) {
			XmlElement elSources = (XmlElement)manual.SelectSingleNode(nodePath);
			Type tDS = thisAsm.GetType(typeName);
			Type[] types = thisAsm.GetTypes();
			foreach (Type t in types) {
				if((interfaceMode && t.GetInterface(typeName) != null) || (!interfaceMode && t.BaseType == tDS)){
					XmlElement elDS = manual.CreateElement("type");
					elSources.AppendChild(elDS);
					XmlUtility.AddAttribute(manual, elDS, "name", t.Namespace + "." + t.Name);
				}
			}
		}



	}
}
