using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Istra {
	/// <summary>Страница, возвращающая уменьшенную версию изображения</summary>
	public class ThumbnailPage : System.Web.UI.Page {
		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			Response.Clear();
			try {
				string imageRef = Request.Params["i"];
				string sW = Request.Params["w"];
				int w = sW == null ? 0 : Int32.Parse(sW);
				string sH = Request.Params["h"];
				int h = sH == null ? 0 : Int32.Parse(sH);

				string path = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir + @"\" + imageRef;
				Image img = Image.FromFile(path);
				float rate = (float)img.Height / img.Width;
				
				if (w == 0 && h == 0) {
					w = 200;
					h = (int)Math.Round(w * rate);
				}
				else if (w == 0) {
					w = (int)Math.Round(h / rate);
				}
				else if (h == 0) {
					h = (int)Math.Round(w * rate);
				}
				else {
					//*** float rateX = (float)img.Width / w;
					//*** float rateY = (float)img.Height / h;
					//*** if (rateX > rateY) {
					//*** 	rate = rateX;
					//*** }
					//*** else {
					//*** 	rate = rateY;
					//*** }
				}
				
				Image thumb = img.GetThumbnailImage(w, h, null, IntPtr.Zero);
				string mimeType = img.RawFormat == ImageFormat.Gif ? "image/gif"
					: img.RawFormat == ImageFormat.Jpeg ? "image/jpeg"
					: img.RawFormat == ImageFormat.Png ? "image/png"
					: "image/jpeg";

				Response.AddHeader("Content-Type", mimeType);
				thumb.Save(Response.OutputStream, img.RawFormat);
			}
			catch(Exception err){
				if (Request["trace"] == "1") {
					throw err;
				}
				else {
					Image i = Image.FromFile(SiteSettings.Current.RootDir + @"\istra\img\error_icon.gif");
					Response.AddHeader("Content-Type", "image/gif");
					i.Save(Response.OutputStream, ImageFormat.Gif);
				}
			}
		}
	}
}
