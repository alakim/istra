using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Istra.WS {
	/// <summary>Удаляет файл</summary>
	public class DeleteFile : WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string docFile = Request["file"];


			string filePath = Istra.SiteSettings.Current.RootDir + docFile;
			try {
				File.Delete(filePath);
				writer.Write(@"{""success"":true}");
			}
			catch (Exception err) {
				WriteError("Ошибка удаления файла", err, writer);
			}
		}
	}
}
