using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Xml;
using System.Xml.Xsl;

namespace Istra {
	/// <summary>Выполняет XSLT-преобразование</summary>
	class XsltProcessor {

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
			XsltSettings xSettings = new XsltSettings();
			xSettings.EnableDocumentFunction = true;

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(SiteSettings.Current.RootDir + srcPath);

			XmlElement root = xmlDoc.DocumentElement;

			if (settings != null) {
				foreach (string k in settings.Keys) {
					root.SetAttribute(k, settings[k]);
				}
			}

			XslCompiledTransform xslt = new XslCompiledTransform();
			xslt.Load(SiteSettings.Current.RootDir + xsltPath, xSettings, new XmlUrlResolver());

			XsltArgumentList xArg = new XsltArgumentList();
			XmlWriter xwrt = XmlWriter.Create(tWriter);
			xslt.Transform(xmlDoc, xArg, tWriter /*xwrt, new UrlResolver()*/);
		}

	}
}
