using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Istra {
	/// <summary>Утилита для работы с XML-документами</summary>
	public class XmlUtility {
		public static void AddAttribute(XmlDocument doc, XmlElement node, string name, string val) {
			XmlAttribute att = doc.CreateAttribute(name);
			att.Value = val;
			node.Attributes.Append(att);
		}
	}
}
