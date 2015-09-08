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
				/// ссылка на картинку
				string imageRef = Request.Params["i"];
				string sW = Request.Params["w"];
				/// требуемая ширина
				int w = sW == null ? 0 : Int32.Parse(sW);
				string sH = Request.Params["h"];
				/// требуемая высота
				int h = sH == null ? 0 : Int32.Parse(sH);
				/// разрешить увеличение картинки
				bool magnify = Request["m"]=="1";
				/// разрешить обрезку картинки
				bool crop = Request["c"] == "1";

				string path = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.ContentDir + @"\" + imageRef;
				Image img = Image.FromFile(path);

				string mimeType = img.RawFormat == ImageFormat.Gif ? "image/gif"
					: img.RawFormat == ImageFormat.Jpeg ? "image/jpeg"
					: img.RawFormat == ImageFormat.Png ? "image/png"
					: "image/jpeg";
				Response.AddHeader("Content-Type", mimeType);

				float rate;
				if (crop) {
					float rateX = w / (float)img.Width;
					float rateY = h / (float)img.Height;
					rate = rateX > rateY? rateX: rateY;

					Image thumb = img.GetThumbnailImage(
						(int)Math.Round(img.Width * rate), 
						(int)Math.Round(img.Height * rate), 
						null, IntPtr.Zero
					);

					Rectangle cropRect = new Rectangle(
						(int)Math.Round((double)(thumb.Width - w) / 2), 
						(int)Math.Round((double)(thumb.Height - h) / 2), 
						w, h
					);
					Bitmap src = thumb as Bitmap;

					Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
					 using(Graphics g = Graphics.FromImage(target)){
					 	g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), 
					 	cropRect,                        
					 	GraphicsUnit.Pixel);
					 }

					target.Save(Response.OutputStream, img.RawFormat);
				}
				else {
					rate = (float)img.Height / img.Width;

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

					if (w < img.Width && h < img.Height || magnify) {
						Image thumb = img.GetThumbnailImage(w, h, null, IntPtr.Zero);
						thumb.Save(Response.OutputStream, img.RawFormat);
					}
					else {
						img.Save(Response.OutputStream, img.RawFormat);
					}
				}
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
