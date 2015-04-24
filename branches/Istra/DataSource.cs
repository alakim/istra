using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Threading;

namespace Istra {
	/// <summary>Кэшируемый источник данных</summary>
	public abstract class DataSource {

		/// <summary>Таймаут для ожидания доступа по мьютексу</summary>
		private const int mutexTimeout = 3000;

		/// <summary>Конструктор</summary>
		protected DataSource(DataSourceDefinition def) {
			if (def.Attributes["postprocessor"] != null) {
				Type t = Type.GetType(def.Attributes["postprocessor"]);
				ConstructorInfo cInf = t.GetConstructor(new Type[0] {});
				this.postprocessor = (IPostprocessor)cInf.Invoke(new object[0] {});
			}
		}

		/// <summary>Имя файла в кэше</summary>
		protected string cachedFile;

		/// <summary>Формирует кэшированный файл данных</summary>
		public virtual bool Build(HttpContext context) {
			DateTime time = File.GetLastWriteTime(FilePath);
			TimeSpan diff = DateTime.Now - time;
			double sec = diff.TotalSeconds;
			if (context.Request["clearcache"]==null && sec < SiteSettings.Current.CacheTime)
				return false;
			
			ClearCache();
			return true;
		}

		/// <summary>Постобработка документа</summary>
		public void PrepareDocument() {
			if (postprocessor != null) {
				XmlDocument xDoc = new XmlDocument();
				xDoc.Load(FilePath);
				postprocessor.Process(xDoc);
				xDoc.Save(FilePath);
			}
		}

		public static void RefreshSources(HttpContext context) {
			Mutex m = new Mutex();
			if(m.WaitOne(mutexTimeout, false)){
				try {
					foreach (DataSourceDefinition def in SiteSettings.Current.Sources) {
						DataSource dSrc = def.CreateDataSource();
						dSrc.Build(context);
					}
				}
				finally {
					m.ReleaseMutex();
				}
			}
		}

		protected string FilePath {
			get {
				return SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.CacheDir + @"\" + cachedFile;
			}
		}

		/// <summary>Удаляет кэшированные данные</summary>
		protected void ClearCache() {
			if (!File.Exists(FilePath)) return;
			File.Delete(FilePath);
		}

		/// <summary>Открывает поток вывода кэшированных данных</summary>
		protected StreamWriter OpenFileStream() {
			return new StreamWriter(FilePath);
		}

		private IPostprocessor postprocessor;
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
