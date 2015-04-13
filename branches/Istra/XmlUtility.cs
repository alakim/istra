using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Istra {
	/// <summary>Утилита для работы с XML-документами</summary>
	public class XmlUtility {
		/// <summary>Добавляет атрибут к XML-элементу</summary>
		/// <param name="doc">XML-документ</param>
		/// <param name="node">целевой XML-элемент</param>
		/// <param name="name">имя атрибута</param>
		/// <param name="val">значение атрибута</param>
		public static void AddAttribute(XmlDocument doc, XmlElement node, string name, string val) {
			XmlAttribute att = doc.CreateAttribute(name);
			att.Value = val;
			node.Attributes.Append(att);
		}

		/// <summary>Добавляет к документу сообщение об ошибке</summary>
		/// <param name="doc">XML-документ</param>
		/// <param name="message">сообщение об ошибке</param>
		public static void WriteError(XmlDocument doc, string message){
			XmlElement err = doc.CreateElement("error");
			doc.DocumentElement.AppendChild(err);
			err.InnerText = message;
		}
	}
}
