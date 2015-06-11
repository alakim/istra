using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Istra {
	/// <summary>ОБработчик для запрета доступа</summary>
	class AccessDeniedHandler : IHttpHandler {
		public bool IsReusable { get { return true; } }

		public void ProcessRequest(HttpContext context) {
			context.Response.Redirect("/default.aspx", true);
		}

	}
}
