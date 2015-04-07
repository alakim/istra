﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;

namespace Istra {
	/// <summary>Кэшируемый источник данных</summary>
	public abstract class DataSource {

		/// <summary>Имя файла в кэше</summary>
		protected string cachedFile;

		/// <summary>Формирует кэшированный файл данных</summary>
		public virtual bool Build(HttpContext context) {
			DateTime time = File.GetCreationTime(FilePath);
			TimeSpan diff = DateTime.Now - time;
			double sec = diff.TotalSeconds;
			if (context.Request["clearcache"]==null && sec < SiteSettings.Current.CacheTime)
				return false;
			
			ClearCache();
			return true;
		}

		public static void RefreshSources(HttpContext context) {
			foreach (DataSourceDefinition def in SiteSettings.Current.Sources) {
				DataSource dSrc = def.CreateDataSource();
				dSrc.Build(context);
			}
		}

		protected string FilePath {
			get {
				return SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.CacheDir + @"\" + cachedFile;
			}
		}

		/// <summary>Удаляет кэшированные данные</summary>
		protected void ClearCache() {
			File.Delete(FilePath);
		}

		/// <summary>Открывает поток вывода кэшированных данных</summary>
		protected StreamWriter OpenFileStream() {
			return new StreamWriter(FilePath);
		}
	}

	/// <summary>Определение источника данных</summary>
	public class DataSourceDefinition {

		/// <summary>Файл кэшированных данных</summary>
		public string CachedFile { get { return cachedFile; } }
		/// <summary>Дополнительные атрибуты</summary>
		public NameValueCollection Attributes { get { return attributes; } }

		/// <summary>Конструктор</summary>
		/// <param name="type">тип источника</param>
		/// <param name="cachedFile">имя кэшерованного файла</param>
		public DataSourceDefinition(string type, string cachedFile, NameValueCollection attributes) {
			this.cachedFile = cachedFile;
			this.type = type;
			this.attributes = attributes;
		}

		/// <summary>Создает экземпляр источника</summary>
		public DataSource CreateDataSource() {
			Type t = Type.GetType(this.type);
			ConstructorInfo cInf = t.GetConstructor(new Type[1]{Type.GetType("Istra.DataSourceDefinition")});
			DataSource dSrc = (DataSource)cInf.Invoke(new object[1]{this});
			return dSrc;
		}

		private string type;
		private string cachedFile;
		private NameValueCollection attributes;
	}

	/// <summary>Обработчик настроек конфигурации</summary>
	public class DataSourcesConfigHandler : IConfigurationSectionHandler {
		/// <summary>Реализация интерфейса</summary>
		public object Create(object parent, object configContext, XmlNode section) {
			DataSourceDefinition[] res = new DataSourceDefinition[section.ChildNodes.Count];
			for(int i=0; i<section.ChildNodes.Count; i++){
				XmlNode dSrc = section.ChildNodes[i];
				XmlNode attType = dSrc.Attributes.GetNamedItem("type");
				if (attType == null) throw new ApplicationException("Data Source configuration error: attribute \"type\" expectted");
				XmlNode attCachedFile = dSrc.Attributes.GetNamedItem("cachedFile");
				if (attCachedFile == null) throw new ApplicationException("Data Source configuration error: attribute \"cachedFile\" expectted");

				NameValueCollection attributes = new NameValueCollection();
				foreach (XmlNode attr in dSrc.Attributes) {
					attributes[attr.Name] = attr.Value;
				}
				
				DataSourceDefinition def = new DataSourceDefinition(attType.Value, attCachedFile.Value, attributes);
				res[i] = def;
			}
			return res;
		}
	}
}