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

		/// <summary>Конструктор</summary>
		protected DataSource(DataSourceDefinition def, DataSource parent) {
			this.parent = parent;
			this.dependentSources = new List<DataSource>();
			if(parent!=null) parent.AddDependent(this);
			if (def.Attributes["postprocessor"] != null) {
				Type t = Type.GetType(def.Attributes["postprocessor"]);
				ConstructorInfo cInf = t.GetConstructor(new Type[0] {});
				this.postprocessor = (IPostprocessor)cInf.Invoke(new object[0] {});
			}
			string activePagesList = def.Attributes["activePages"];
			if (activePagesList != null) {
				string[] pNames = activePagesList.Split(",;".ToCharArray());
				activePages = new List<string>(pNames);
			}
		}

		/// <summary>Добавляет зависимый источник</summary>
		/// <param name="dep">источник для добавления</param>
		public void AddDependent(DataSource dep) {
			dependentSources.Add(dep);
		}

		/// <summary>Имя файла в кэше</summary>
		public string CachedFile { get { return cachedFile; } }

		/// <summary>Имя файла в кэше</summary>
		protected string cachedFile;
		/// <summary>Зависимые источники</summary>
		protected List<DataSource> dependentSources;
		/// <summary>Родительский источник, используемый зависимыми источниками</summary>
		protected DataSource parent;

		/// <summary>Формирует кэшированный файл данных</summary>
		public virtual bool Build(HttpContext context) {
			if (context.Request["clearcache"] != null) {
				ClearCache();
				return true;
			}

			string pageNm = context.Request["p"];
			if (pageNm == null) pageNm = "home";
			if (activePages.Count>0 && !activePages.Contains(pageNm)) return false;

			DateTime time = File.GetLastWriteTime(FilePath);
			TimeSpan diff = DateTime.Now - time;
			double sec = diff.TotalSeconds;
			if (sec < SiteSettings.Current.CacheTime)
				return false;
			
			ClearCache();
			return true;
		}

		/// <summary>Формирует кэшированные файлы данных для зависимых источников</summary>
		protected void BuildDependents(HttpContext context) {
			foreach (DataSource src in dependentSources) {
				src.Build(context);
			}
		}

		/// <summary>Страницы, на которых активируется источник</summary>
		protected List<string> activePages = new List<string>();

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
			if(m.WaitOne(SiteSettings.mutexTimeout, false)){
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
				return cachedFile[0]=='/'? SiteSettings.Current.RootDir + @"\" + cachedFile
					: SiteSettings.Current.RootDir + @"\" + SiteSettings.Current.CacheDir + @"\" + cachedFile;
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
		/// <summary>Зависимые источники</summary>
		public List<DataSourceDefinition> DependentSources { get { return dependentSources; } }

		/// <summary>Конструктор</summary>
		/// <param name="type">тип источника</param>
		/// <param name="cachedFile">имя кэшерованного файла</param>
		public DataSourceDefinition(XmlNode dSrc) {
			XmlNode attType = dSrc.Attributes.GetNamedItem("type");
			if (attType == null) throw new ApplicationException("Data Source configuration error: attribute \"type\" expectted");
			this.type = attType.Value;

			XmlNode attCachedFile = dSrc.Attributes.GetNamedItem("cachedFile");
			if (attCachedFile == null) throw new ApplicationException("Data Source configuration error: attribute \"cachedFile\" expectted");
			this.cachedFile = attCachedFile.Value;

			this.attributes = new NameValueCollection();
			foreach (XmlNode attr in dSrc.Attributes) {
				this.attributes[attr.Name] = attr.Value;
			}

			this.dependentSources = new List<DataSourceDefinition>();
			foreach (XmlNode chld in dSrc.ChildNodes) {
				if (chld.Name == "source") {
					DataSourceDefinition chDef = new DataSourceDefinition(chld);
					this.dependentSources.Add(chDef);
				}
			}
		}


		/// <summary>Создает экземпляр источника</summary>
		public DataSource CreateDataSource() {
			return CreateDataSource(null);
		}

		/// <summary>Создает экземпляр источника</summary>
		/// <param name="parent">родительский источник</param>
		public DataSource CreateDataSource(DataSource parent) {
			Type t = Type.GetType(this.type);
			ConstructorInfo cInf = t.GetConstructor(new Type[2]{
				Type.GetType("Istra.DataSourceDefinition"),
				Type.GetType("Istra.DataSource")
			});
			
			DataSource dSrc = (DataSource)cInf.Invoke(new object[2]{this, parent});
			foreach (DataSourceDefinition dep in dependentSources) {
				dep.CreateDataSource(dSrc);
			}
			return dSrc;
		}

		private string type;
		private string cachedFile;
		private NameValueCollection attributes;
		private List<DataSourceDefinition> dependentSources;
	}

	/// <summary>Обработчик настроек конфигурации</summary>
	public class DataSourcesConfigHandler : IConfigurationSectionHandler {
		/// <summary>Реализация интерфейса</summary>
		public object Create(object parent, object configContext, XmlNode section) {
			DataSourceDefinition[] res = new DataSourceDefinition[section.ChildNodes.Count];
			for(int i=0; i<section.ChildNodes.Count; i++){
				res[i] = new DataSourceDefinition(section.ChildNodes[i]);
			}
			return res;
		}
	}
}
