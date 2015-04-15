using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.Specialized;

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


		/// <summary>Возвращает таблицу атрибутов заданного XML-элемента</summary>
		/// <param name="el">XML-элемент</param>
		public static StringDictionary GetAttributes(XmlElement el) {
			return GetAttributes(el, null);
		}

		/// <summary>Возвращает таблицу атрибутов заданного XML-элемента</summary>
		/// <param name="el">XML-элемент</param>
		/// <param name="exclusions">имена атрибутов, исключаемых из коллекции (перечисляются через запятую)</param>
		public static StringDictionary GetAttributes(XmlElement el, string exclusions) {
			Dictionary<string, bool> dExclusions = new Dictionary<string, bool>();
			if (exclusions != null) {
				foreach(string k in exclusions.Split(",".ToCharArray())){
					dExclusions[k] = true;
				}
			}
			StringDictionary res = new StringDictionary();
			foreach (XmlNode att in el.Attributes) {
				if(!dExclusions.ContainsKey(att.Name))
					res[att.Name] = att.Value;
			}
			return res;
		}

		/// <summary>Добавляет к документу сообщение об ошибке</summary>
		/// <param name="doc">XML-документ</param>
		/// <param name="message">сообщение об ошибке</param>
		public static void WriteError(XmlDocument doc, string message){
			XmlElement err = doc.CreateElement("error");
			doc.DocumentElement.AppendChild(err);
			err.InnerText = message;
		}

		/// <summary>Пространство имен для XML-элементов</summary>
		public const string IstraNamespace = "http://www.istra.com/cms";
	}
}
