using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.Xsl;
using System.Reflection;
using System.Web;

namespace Istra {
	/// <summary>Выполняет XSLT-преобразование</summary>
	class XsltProcessor {

		/// <summary>Конструктор</summary>
		/// <param name="context">контекст веб-приложения</param>
		public XsltProcessor(HttpContext context) {
			this.context = context;
		}

		/// <summary>Преобразует XML-документ</summary>
		/// <param name="srcPath">исходный документ</param>
		/// <param name="xsltPath">XSLT-преобразование</param>
		/// <param name="tWriter">компонен вывода данных</param>
		public void TransformDocument(string srcPath, string xsltPath, TextWriter tWriter) {
			TransformDocument(srcPath, xsltPath, null, tWriter);
		}

		/// <summary>Преобразует XML-документ</summary>
		/// <param name="srcPath">исходный документ</param>
		/// <param name="xsltPath">XSLT-преобразование</param>
		/// <param name="settings">дополнительные настройки преобразования</param>
		/// <param name="tWriter">компонент вывода данных</param>
		public void TransformDocument(string srcPath, string xsltPath, Dictionary<string, string> settings, TextWriter tWriter) {

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(SiteSettings.Current.RootDir + srcPath);
			TransformDocument(xmlDoc, xsltPath, settings, tWriter);
		}

		/// <summary>Режим выдачи документа без преобразования</summary>
		public bool RawMode = false;

		/// <summary>Преобразует XML-документ</summary>
		/// <param name="xmlDoc">исходный документ</param>
		/// <param name="xsltPath">XSLT-преобразование</param>
		/// <param name="settings">дополнительные настройки преобразования</param>
		/// <param name="tWriter">компонент вывода данных</param>
		public void TransformDocument(XmlDocument xmlDoc, string xsltPath, Dictionary<string, string> settings, TextWriter tWriter) {
			XsltSettings xSettings = new XsltSettings();
			xSettings.EnableDocumentFunction = true;

			XmlElement root = xmlDoc.DocumentElement;

			if (settings != null) {
				foreach (string k in settings.Keys) {
					root.SetAttribute(k, settings[k]);
				}
			}

			XmlNamespaceManager nsmng = new XmlNamespaceManager(xmlDoc.NameTable);
			nsmng.AddNamespace("istra", XmlUtility.IstraNamespace);
			XmlNodeList queries = xmlDoc.SelectNodes("//istra:query", nsmng);
			foreach (XmlElement xQ in queries) {
				string queryType = xQ.Attributes.GetNamedItem("type").Value;
				Type t = Type.GetType(queryType);
				if(t==null) throw new ApplicationException("Class '"+queryType+"' is not defined.");
				ConstructorInfo cInf = t.GetConstructor(new Type[0]);
				IQuery query = (IQuery)cInf.Invoke(new object[0]);
				query.Apply(xmlDoc, xQ, context);
			}

			//XmlWriter xwrt = XmlWriter.Create(tWriter);
			if (RawMode) {
				tWriter.Write(xmlDoc.OuterXml);
			}
			else {
				XslCompiledTransform xslt = new XslCompiledTransform();
				xslt.Load(SiteSettings.Current.RootDir + xsltPath, xSettings, new XmlUrlResolver());

				XsltArgumentList xArg = new XsltArgumentList();
				xslt.Transform(xmlDoc, xArg, tWriter /*xwrt, new UrlResolver()*/);
			}
		}


		/// <summary>Контекст веб-приложения</summary>
		private HttpContext context;
	}
}
