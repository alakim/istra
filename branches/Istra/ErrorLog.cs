using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using System.Web;

namespace Istra {
	/// <summary>Протокол ошибок</summary>
	/// <remarks>Используется сохранение протокола в директорию (а не в БД) как более универсальное (предполагается, что не всякий проект может использовать БД).</remarks>
	public static class ErrorLog {

		/// <summary>Сохраняет сообщение об ошибке</summary>
		/// <param name="err">исключение</param>
		public static void WriteError(Exception err) {
			DateTime date = DateTime.Now;
			XmlDocument xError = new XmlDocument();
			StringWriter wrt = new StringWriter();
			wrt.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
			wrt.WriteLine(@"<error date=""{0}"">", XmlUtility.FormatDateTime(date));
			wrt.WriteLine("<Message>{0}</Message>", XmlUtility.FormatString(err.Message));
			wrt.WriteLine("<Source>{0}</Source>", XmlUtility.FormatString(err.Source));
			wrt.WriteLine("<StackTrace>{0}</StackTrace>", XmlUtility.FormatString(err.StackTrace));
			wrt.WriteLine("<TargetSite>{0}</TargetSite>", XmlUtility.FormatString(err.TargetSite.ToString()));
			wrt.WriteLine("</error>");
			xError.InnerXml = wrt.ToString();

			SqlConnection myConn = new SqlConnection("server=(local);database=pubs;Trusted_Connection=yes");
			SqlCommand myCmd = new SqlCommand("select * from jobs", myConn);

			string path = CreateDir(date);
			
			StringWriter wrtPath = new StringWriter();
			wrtPath.Write(path);
			wrtPath.Write(@"\");
			wrtPath.Write(Guid.NewGuid().ToString("N"));
			wrtPath.Write(@".xml");
			xError.Save(wrtPath.ToString());
		}

		/// <summary>Возвращает текущий протокол ошибок</summary>
		public static XmlDocument GetProtocol() {
			XmlDocument xProtocol = new XmlDocument();
			xProtocol.LoadXml("<ErrorLog/>");
			string logPath = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.LogDir;
			if (!Directory.Exists(logPath)) return xProtocol;

			AddDirectory(logPath, xProtocol);

			return xProtocol;
		}


		/// <summary>Добавляет в документ описание директории</summary>
		/// <param name="dirPath">путь к директории</param>
		/// <param name="doc">целевой XML-документ</param>
		private static void AddDirectory(string dirPath, XmlDocument doc) {
			foreach (string filePath in Directory.GetFiles(dirPath)) {
				LoadFile(filePath, doc);
			}
			foreach (string cDir in Directory.GetDirectories(dirPath)) {
				AddDirectory(cDir, doc);
			}
		}


		/// <summary>Загружает содержимое файла в целевой XML-элемент</summary>
		/// <param name="filePath">путь к файлу</param>
		/// <param name="doc">целевой XML-документ</param>
		private static void LoadFile(string filePath, XmlDocument doc) {
			XmlDocument d1 = new XmlDocument();
			d1.Load(filePath);
			//string xml = reXmlDef.Replace(d1.InnerXml, string.Empty);
			//target.InnerXml = xml;
			doc.DocumentElement.InnerXml += d1.DocumentElement.OuterXml;
		}

		private static Regex reXmlDef = new Regex(@"<\?xml[^\?]+\?>\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);




		/// <summary>Удаляет данные протокола ошибок</summary>
		public static void Clear() {
			string logPath = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.LogDir;
			Directory.Delete(logPath, true);
			Directory.CreateDirectory(logPath);
		}

		/// <summary>Создает директорию для сохранения ошибок за текущую дату</summary>
		private static string CreateDir(DateTime date) {
			string logPath = SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.LogDir;
			if (!Directory.Exists(logPath))
				Directory.CreateDirectory(logPath);
			string yearPath = logPath + @"\" + date.Year;
			if (!Directory.Exists(yearPath))
				Directory.CreateDirectory(yearPath);
			string monthPath = yearPath + @"\" + date.Month;
			if (!Directory.Exists(monthPath))
				Directory.CreateDirectory(monthPath);
			string dayPath = monthPath + @"\" + date.Day;
			if (!Directory.Exists(dayPath))
				Directory.CreateDirectory(dayPath);
			string hourPath = dayPath + @"\" + date.Hour;
			if (!Directory.Exists(hourPath))
				Directory.CreateDirectory(hourPath);
			return hourPath;
		}
	}

	/// <summary>Возвращает текущий протокол ошибок</summary>
	public class ErrorLogQuery : IQuery {

		/// <summary>Добавляет результат запроса к заданному документу</summary>
		/// <param name="doc">целевой документ</param>
		/// <param name="xQuery">XML-описание данного запроса</param>
		/// <param name="context">контекст веб-приложения</param>
		public void Apply(XmlDocument doc, XmlElement xQuery, HttpContext context) {
			XmlElement requestRoot = doc.CreateElement("ErrorLog");
			xQuery.ParentNode.InsertAfter(requestRoot, xQuery);
			xQuery.ParentNode.RemoveChild(xQuery);

			requestRoot.InnerXml = ErrorLog.GetProtocol().InnerXml;
		}


	}
}
