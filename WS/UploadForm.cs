using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Istra.WS {
	/// <summary>Загружает файлы на сервер</summary>
	public class UploadForm : WebService {

		protected override void OnLoadComplete(EventArgs e) {
			base.OnLoadComplete(e);
			if (this.IsPostBack) {
				FindControl("UploadForm").Visible = false;

				if (!CheckSession(null)) return;

				string fileName = Request["fileName"];
				string filePath = Request["filePath"];
				HttpPostedFile uploadedFile = Request.Files["uploadedFile"];

				// string docFile = Request["doc"];
				// 
				// string content = FileOperationsUtility.GetText(docFile);
				// writer.Write(content);

				WriteSuccess(null);

			}
		}
		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
		}

		/// <summary>Выводит сообщение об ошибке</summary>
		/// <param name="title">заголовок сообщения</param>
		/// <param name="writer">компонент вывода</param>
		public override void WriteError(string title, System.Web.UI.HtmlTextWriter writer) {
			Label label =  (Label)FindControl("lblError");
			if (label != null) {
				label.Text = title;
				label.Parent.Visible = true;
			}
		}

		/// <summary>Выводит сообщение об успешном завершенни операции</summary>
		public override void WriteSuccess(System.Web.UI.HtmlTextWriter writer) {
			FindControl("divSuccess").Visible = true;
		}
	}
}
