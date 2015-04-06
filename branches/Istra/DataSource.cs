using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Collections.Specialized;
using System.Reflection;

namespace Istra {
	/// <summary>Кэшируемый источник данных</summary>
	public abstract class DataSource {

		/// <summary>Имя файла в кэше</summary>
		protected string cachedFile;

		/// <summary>Формирует кэшированный файл данных</summary>
		public virtual void Build() {
			ClearCache();
		}

		public static void RefreshSources() {
			foreach (DataSourceDefinition def in SiteSettings.Current.Sources) {
				DataSource dSrc = def.CreateDataSource();
				dSrc.Build();
			}
		}

		protected string FilePath {
			get {
				return SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.CacheDir + @"\" + cachedFile;
			}
		}

		/// <summary>Удаляет кэшированные данные</summary>
		protected void ClearCache() {
			//SiteSettings sSettings = new SiteSettings();
			//string filePath = sSettings.RootDir + @"\" + sSettings.CacheDir + @"\"+cachedFile;
			File.Delete(FilePath);
		}

		/// <summary>Открывает поток вывода кэшированных данных</summary>
		protected StreamWriter OpenFileStream() {
			return new StreamWriter(FilePath);
		}
	}

	/// <summary>Определение источника данных</summary>
	public class DataSourceDefinition {

		/// <summary>Конструктор</summary>
		/// <param name="type">тип источника</param>
		/// <param name="cachedFile">имя кэшерованного файла</param>
		public DataSourceDefinition(string type, string cachedFile) {
			this.cachedFile = cachedFile;
			this.type = type;
		}

		/// <summary>Создает экземпляр источника</summary>
		public DataSource CreateDataSource() {
			Type t = Type.GetType(this.type);
			ConstructorInfo cInf = t.GetConstructor(new Type[0]);
			DataSource dSrc = (DataSource)cInf.Invoke(new object[0]);
			return dSrc;
		}

		private string type;
		private string cachedFile;
	}

	/// <summary>Обработчик настроек конфигурации</summary>
	public class DataSourcesConfigHandler : IConfigurationSectionHandler {
		/// <summary>Реализация интерфейса</summary>
		public object Create(object parent, object configContext, XmlNode section) {
			DataSourceDefinition[] res = new DataSourceDefinition[section.ChildNodes.Count];
			for(int i=0; i<section.ChildNodes.Count; i++){
			//foreach (XmlNode dSrc in section.ChildNodes) {
				XmlNode dSrc = section.ChildNodes[i];
				XmlNode attType = dSrc.Attributes.GetNamedItem("type");
				if (attType == null) throw new ApplicationException("Data Source configuration error: attribute \"type\" expectted");
				XmlNode attCachedFile = dSrc.Attributes.GetNamedItem("cachedFile");
				if (attCachedFile == null) throw new ApplicationException("Data Source configuration error: attribute \"cachedFile\" expectted");
				
				DataSourceDefinition def = new DataSourceDefinition(attType.Value, attCachedFile.Value);
				res[i] = def;
			}
			return res;
		}
	}
}
