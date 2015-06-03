using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Istra.WS {
	/// <summary>Выполняет операции над файлами</summary>
	public class FileManager : Istra.WS.WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string operation = Request["oper"];
			string file = Request["file"];
			string target = Request["target"];


			try {
				switch (operation) {
					case "delFile":
						FileOperationsUtility.DeleteFile(file);
						break;
					case "delDir":
						string dir = Request["dir"];
						FileOperationsUtility.DeleteDirectory(dir);
						break;
					case "saveText":
						string text = Request["text"];
						FileOperationsUtility.SaveText(file, text);
						break;
					case "saveXml":
						string xml = Request["xml"];
						FileOperationsUtility.SaveXml(file, xml);
						break;
					case "renameFile":
					case "moveFile":
						FileOperationsUtility.Move(file, target, false);
						break;
					case "renameDir":
					case "moveDir":
						FileOperationsUtility.Move(file, target, true);
						break;
					default:
						throw new NotImplementedException("Операция '" + operation + "' не определена");
				}
				WriteSuccess(writer);
			}
			catch (Exception err) {
				WriteError("Ошибка файл-менеджера", err, writer);
			}
		}
	}
}
