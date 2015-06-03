using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Istra.WS {
	/// <summary>Утилита операций над файлами</summary>
	static class FileOperationsUtility {
		/// <summary>Удаляет файл</summary>
		/// <param name="file">путь к файлу относительно корневой директории приложения</param>
		public static void DeleteFile(string file) {
			File.Delete(FullPath(file));
		}

		/// <summary>Удаляет директорию</summary>
		/// <param name="dir">путь к директории относительно корневой директории приложения</param>
		public static void DeleteDirectory(string dir) {
			Directory.Delete(FullPath(dir), true);
		}

		/// <summary>Создает файл</summary>
		/// <param name="file">путь к файлу относительно корневой директории приложения</param>
		public static void CreateFile(string file) {
			FileStream stm = File.Create(FullPath(file));
			stm.Close();
		}

		/// <summary>Создает директорию</summary>
		/// <param name="dir">путь к директории относительно корневой директории приложения</param>
		public static void CreateDirectory(string dir) {
			Directory.CreateDirectory(FullPath(dir));
		}

		/// <summary>Перемещает или переименовывает файл</summary>
		/// <param name="file">исходный путь к файлу относительно корневой директории приложения</param>
		/// <param name="target">целевой путь к файлу относительно корневой директории приложения</param>
		/// <param name="directoryMode">указывает, что действия производятся над директорией</param>
		public static void Move(string file, string target, bool directoryMode) {
			if (directoryMode)
				Directory.Move(FullPath(file), FullPath(target));
			else
				File.Move(FullPath(file), FullPath(target));
		}

		/// <summary>Сохраняет текстовый файл</summary>
		/// <param name="file">путь к файлу относительно корневой директории приложения</param>
		/// <param name="content">текстовый контент</param>
		public static void SaveText(string file, string content) {
			string filePath = FullPath(file);
			File.WriteAllText(filePath, content);
		}

		/// <summary>Сохраняет XML-документ с проверкой корректности и сохранением форматирования</summary>
		/// <param name="file">путь к файлу относительно корневой директории приложения</param>
		/// <param name="content">контент в формате XML со скрытой разметкой</param>
		public static void SaveXml(string file, string content) {
			SaveXml(file, content, true, null);
		}

		/// <summary>Сохраняет XML-документ с проверкой корректности без сохранения форматирования</summary>
		/// <param name="file">путь к файлу относительно корневой директории приложения</param>
		/// <param name="content">контент в формате XML со скрытой разметкой</param>
		/// <param name="processor">выполняет предварительную обработку документа (не сохраняет форматирование)</param>
		public static void SaveXml(string file, string content, IPagePreprocessor processor) {
			SaveXml(file, content, false, processor);
		}

		/// <summary>Сохраняет XML-документ с проверкой корректности</summary>
		/// <param name="file">путь к файлу относительно корневой директории приложения</param>
		/// <param name="content">контент в формате XML со скрытой разметкой</param>
		/// <param name="preserveFormatting">флаг сохранения форматирования документа</param>
		/// <param name="processor">выполняет предварительную обработку документа (не сохраняет форматирование)</param>
		public static void SaveXml(string file, string content, bool preserveFormatting, IPagePreprocessor processor) {
			XmlDocument doc;
			string filePath = FullPath(file);
			content = JsonUtility.RestoreXmlMarkup(content);
			try {
				doc = new XmlDocument();
				doc.LoadXml(content);
				if (processor != null)
					processor.Process(doc);
			}
			catch (Exception err) {
				throw new ApplicationException("Ошибка синтаксического разбора документа", err);
			}

			if (preserveFormatting && processor==null)
				File.WriteAllText(filePath, content);
			else
				doc.Save(filePath);
		}

		/// <summary>Возвращает содержимое текстового файла</summary>
		/// <param name="file">путь к файлу относительно корневой директории приложения</param>
		public static string GetText(string file) {
			string filePath = FullPath(file);
			return File.ReadAllText(filePath);
		}


		/// <summary>Возвращает полный путь</summary>
		/// <param name="path">путь относительно корневой директории приложения</param>
		private static string FullPath(string path) {
			if (path[0] != '/') path = '/' + path;
			string fullPath = Istra.SiteSettings.Current.RootDir + path;
			fullPath = fullPath.Replace('/', '\\');
			return fullPath;
		}
	}
}
