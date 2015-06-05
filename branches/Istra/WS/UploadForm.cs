using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
				string fileDir = Request["filePath"];
				HttpPostedFile uploadedFile = Request.Files["uploadedFile"];

				fileName = BuildFileName(fileName, uploadedFile);

				FileOperationsUtility.Upload(fileName, fileDir, uploadedFile);

				WriteSuccess(null);

			}
		}
		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
		}

		/// <summary>Формирует имя файла для сохранения на диск</summary>
		/// <param name="fileName">имя, заданное пользователем</param>
		/// <param name="uploadedFile">загруженный файл</param>
		/// <returns></returns>
		private string BuildFileName(string fileName, HttpPostedFile uploadedFile) {
			string[] nameParts = fileName.Split(".".ToCharArray());
			string[] fileParts = uploadedFile.FileName.Split(".".ToCharArray());
			string name = string.Empty,
					ext = string.Empty;
			if (nameParts.Length < 2) ext = fileParts[fileParts.Length - 1];
			if (nameParts[0].Length > 0)
				name = nameParts[0];
			else
				name = reCyr.Replace(fileParts[0], "z");
			return name + "." + ext;
		}

		Regex reCyr = new Regex(@"[а-я]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
