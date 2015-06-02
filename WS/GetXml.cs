﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Istra.WS {
	/// <summary>Загружает XML-данные</summary>
	public class GetXml : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string docFile = Request["doc"];
			string filePath = Istra.SiteSettings.Current.RootDir + docFile;

			string xml = File.ReadAllText(filePath);
			writer.Write(xml);
		}
	}
}