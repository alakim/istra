using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Istra {
	public class JsonUtility {
		public static string Serialize(XmlNode xNd) {
			if (xNd.NodeType == XmlNodeType.Text) return PrepareString(xNd.Value);
			StringWriter wrt = new StringWriter();
			wrt.Write("{");
			wrt.Write(@"""_type"":""{0}""", xNd.Name);
			if (xNd.Attributes != null && xNd.Attributes.Count > 0) {
				wrt.Write(@",""_attr"":{");
				bool first = true;
				foreach (XmlAttribute att in xNd.Attributes) {
					if (first) first = false; else wrt.Write(",");
					wrt.Write(@"""{0}"":{1}", att.Name, PrepareString(att.Value));
				}
				wrt.Write("}");
			}
			if (xNd.ChildNodes != null && xNd.ChildNodes.Count > 0) {
				wrt.Write(@",""_ch"":[");
				bool first = true;
				foreach (XmlNode ch in xNd.ChildNodes) {
					if (first) first = false; else wrt.Write(",");
					wrt.Write(Serialize(ch));
				}
				wrt.Write("]");
			}
			wrt.Write("}");
			return wrt.ToString();
		}

		/// <summary>Подготавливает строку для вставки в JSON</summary>
		/// <param name="str">исходная строка</param>
		public static string PrepareString(string str) {
			return PrepareString(str, false);
		}

		/// <summary>Подготавливает строку для вставки в JSON</summary>
		/// <param name="str">исходная строка</param>
		/// <param name="rawValue">предписывает не заключать строковое значение в кавычки</param>
		public static string PrepareString(string str, bool rawValue) {
			return (rawValue ? string.Empty : "\"") + str.Replace("\\", "\\\\")
				.Replace("\n", "\\n")
				.Replace("\r", "\\r")
				.Replace("\t", "\\t")
				.Replace("\"", "\\\"")
				.Replace("\'", "\\\'")
				+ (rawValue ? string.Empty : "\"");
		}

		/// <summary>Восстанавливает скрытую XML-разметку</summary>
		/// <param name="hiddenXml">код со скрытой XML-разметкой</param>
		public static string RestoreXmlMarkup(string hiddenXml) {
			return hiddenXml.Replace("#[#", "<").Replace("#]#", ">"); ;
		}

		/// <summary>Форматирует строку даты</summary>
		/// <param name="date">дата и время</param>
		public static string FormatDate(DateTime date) {
			StringWriter wrt = new StringWriter();
			wrt.Write(@"{0}.{1}.{2} {3}:{4}:{5}",
				(date.Day < 10 ? "0" : string.Empty) + date.Day,
				(date.Month < 10 ? "0" : string.Empty) + date.Month,
				date.Year,
				(date.Hour < 10 ? "0" : string.Empty) + date.Hour,
				(date.Minute < 10 ? "0" : string.Empty) + date.Minute,
				(date.Second < 10 ? "0" : string.Empty) + date.Second
			);
			return wrt.ToString();

		}
	}
}
