using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Istra {
	/// <summary>Приводит все даты к единому формату</summary>
	public class DateFormatPostprocessor : IPostprocessor {

		private static Regex reDate1 = new Regex(@"(\d\d?)[.\-\/](\d\d?)[.\-\/](\d\d\d\d)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static Regex reDate2 = new Regex(@"(\d\d\d\d)[.\-\/](\d\d?)[.\-\/](\d\d?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>Выполняет преобразование</summary>
		/// <param name="xDoc">обрабатываемый XML-документ</param>
		public void Process(XmlDocument xDoc) {
			XmlNodeList dates = xDoc.SelectNodes(@"//@date");

			foreach (XmlNode nd in dates) {
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

	}
}
