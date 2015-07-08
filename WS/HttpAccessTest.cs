using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Istra.WS {
	/// <summary>Проверяет доступность веб-ресурса</summary>
	public class HttpAccessTest : Istra.WS.WebService {

		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			base.Render(writer);
			if (!CheckSession(writer)) return;

			string url = Request["url"];

			try {
				WebRequest request = WebRequest.Create(url);
				request.Method = "HEAD";
				request.UseDefaultCredentials = true;
				WebResponse responce = request.GetResponse();
				HttpWebResponse hResp = (HttpWebResponse)responce;
				if (hResp.StatusCode == HttpStatusCode.OK)
					WriteSuccess(writer);
				else {
					WriteError("Ресурс недоступен: "+hResp.StatusCode, writer);
				}
			}
			catch (Exception err) {
				WriteError("Ошибка тестирования веб-ресурса", err, writer);
			}
		}
	}

}
