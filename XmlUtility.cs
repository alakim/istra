using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Specialized;
using System.IO;

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

		/// <summary>Возвращает значение атрибута или null, если атрибут не задан.</summary>
		/// <param name="el">XML-элемент</param>
		/// <param name="name">имя атрибута</param>
		public static string GetAttribute(XmlNode el, string name) {
			return GetAttribute((XmlElement)el, name);
		}

		/// <summary>Возвращает значение атрибута или null, если атрибут не задан.</summary>
		/// <param name="el">XML-элемент</param>
		/// <param name="name">имя атрибута</param>
		public static string GetAttribute(XmlElement el, string name) {
			XmlNode ndAtt = el.Attributes.GetNamedItem(name);
			if (ndAtt == null) return null;
			return ndAtt.Value;
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

		/// <summary>Форматирует представление даты и времени</summary>
		/// <param name="dt">временная метка</param>
		public static string FormatDateTime(DateTime dt) {
			return FormatDateTime(dt, false);
		}

		/// <summary>Форматирует представление даты и времени</summary>
		/// <param name="dt">временная метка</param>
		/// <param name="dateOnly">выводить только дату</param>
		public static string FormatDateTime(DateTime dt, bool dateOnly) {
			StringWriter wrt = new StringWriter();
			wrt.Write(dt.Year);
			wrt.Write("-");
			wrt.Write(dt.Month < 10? "0" + dt.Month : dt.Month.ToString());
			wrt.Write("-");
			wrt.Write(dt.Day < 10 ? "0" + dt.Day : dt.Day.ToString());
			if (!dateOnly) {
				wrt.Write("T");
				wrt.Write(dt.Hour < 10 ? "0" + dt.Hour : dt.Hour.ToString());
				wrt.Write(":");
				wrt.Write(dt.Minute < 10 ? "0" + dt.Minute : dt.Minute.ToString());
				wrt.Write(":");
				wrt.Write(dt.Second < 10 ? "0" + dt.Second : dt.Second.ToString());
			}
			return wrt.ToString();
		}

		private static Regex reDate1 = new Regex(@"(\d\d?)[.\-\/](\d\d?)[.\-\/](\d\d\d\d)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex reDate2 = new Regex(@"(\d\d\d\d)[.\-\/](\d\d?)[.\-\/](\d\d?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>Приводит формат даты к единому стилю</summary>
		/// <param name="nodes">узлы, содержащие даты</param>
		public static void FormatDates(XmlNodeList nodes) {
			foreach (XmlNode nd in nodes) {
				string date = nd.Value;
				Match mt = reDate1.Match(date);
				if (mt.Success) {
					nd.Value = mt.Groups[3].Value + "-" + mt.Groups[2].Value + "-" + mt.Groups[1].Value;
				}
				else {
					mt = reDate2.Match(date);
					if (mt.Success) {
						nd.Value = mt.Groups[1].Value + "-" + mt.Groups[2].Value + "-" + mt.Groups[3].Value;
					}
					else {

					}
				}

			}
		}
		/// <summary>Форматирует строку</summary>
		/// <param name="str">исходная строка</param>
		public static string FormatString(string str) {
			return str.Replace("&", "&amp;")
					.Replace("<", "&lt;")
					.Replace(">", "&gt;")
					.Replace("\"", "&quot;")
					.Replace("\'", "&amp;");
		}

		/// <summary>Пространство имен для XML-элементов</summary>
		public const string IstraNamespace = "http://www.istra.com/cms";
	}
}
