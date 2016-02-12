using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Istra {
	/// <summary>Менеджер представлений сайта</summary>
	public class ViewManager {

		/// <summary>Закрытый конструктор</summary>
		protected ViewManager() {
			settings = (List<ViewSettings>)ConfigurationManager.GetSection("Istra/Views");
			foreach(ViewSettings vs in settings){
				if (vs.IsDefaultView) {
					defaultView = vs;
					break;
				}
			}
		}

		/// <summary>Экземпляр менеджера</summary>
		public static ViewManager Instance { get { 
			if(instance==null)
				instance = new ViewManager();
			return instance; 
		} }

		/// <summary>Возвращает представление по заданному идентификатору</summary>
		/// <param name="id">идентификатор представления</param>
		public ViewSettings GetView(int id){
			foreach (ViewSettings vs in settings) {
				if (vs.ID == id) return vs;
			}
			throw new ApplicationException("View #"+id+" does not exist.");
		}

		private static ViewManager instance = null;
		private List<ViewSettings> settings;
		private ViewSettings defaultView = null;

		/// <summary>Обработчик конфигурации представлений</summary>
		public class ConfigHandler : IConfigurationSectionHandler {
			/// <summary>Реализация интерфейса</summary>
			public object Create(object parent, object configContext, XmlNode section) {
				List<ViewSettings> res = new List<ViewSettings>();
				foreach(XmlNode nd in section.SelectNodes("view")) {
					res.Add(new ViewSettings((XmlElement)nd));
				}
				return res;
			}
		}
	}

	/// <summary>Настройки представления</summary>
	public class ViewSettings {
		/// <summary>Конструктор</summary>
		/// <param name="xml">XML-определение настроек</param>
		public ViewSettings(XmlElement xml) {
			xsltDir = XmlUtility.GetAttribute(xml, "xsltDir");
			id = Int32.Parse(XmlUtility.GetAttribute(xml, "id"));
			isDefaultView = XmlUtility.GetAttribute(xml, "default").ToLower() == "true";
		}

		/// <summary>Идентификатор представления</summary>
		public int ID { get { return id; } }
		/// <summary>Директория для размещения XSLT-шаблонов</summary>
		public string XsltDir { get { return xsltDir; } }
		/// <summary>Указывает, что это представление по умолчанию</summary>
		public bool IsDefaultView { get { return isDefaultView; } }

		/// <summary>Директория для размещения XSLT-шаблонов</summary>
		private string xsltDir;
		/// <summary>Идентификатор представления</summary>
		private int id;
		/// <summary>Указывает, что это представление по умолчанию</summary>
		private bool isDefaultView = false;
	}

}
