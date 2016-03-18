using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Istra.Json {
	/// <summary>Утилита для форматирования JSON</summary>
	public class Json {
		/// <summary>Выполняет сериализацию строки</summary>
		/// <param name="str">исходная строка</param>
		public static string Serialize(string str) {
			return Serialize(str, false);
		}

		/// <summary>Выполняет сериализацию строки</summary>
		/// <param name="str">исходная строка</param>
		/// <param name="rawValue">предписывает возвращать значение без окружающих кавычек</param>
		public static string Serialize(string str, bool rawValue) {
			if (str == null) str = string.Empty;
			return (rawValue ? string.Empty : "\"") + str.Replace("\\", "\\\\")
				.Replace("\n", "\\n")
				.Replace("\r", "\\r")
				.Replace("\t", "\\t")
				.Replace("\"", "\\\"")
				//.Replace("\'", "\\\'")
				+ (rawValue ? string.Empty : "\"");
		}

		/// <summary>Выражение для удаления лишних нулей</summary>
		private static Regex reNulls = new Regex(@"\.0+$", RegexOptions.Compiled); 

		/// <summary>Выполняет сериализацию числа</summary>
		/// <param name="d">исходное число</param>
		public static string Serialize(decimal d) {
			string ss = d.ToString("F").Replace(',', '.');
			return reNulls.Replace(ss, string.Empty);
		}

		/// <summary>Выполняет сериализацию числа</summary>
		/// <param name="d">исходное число</param>
		public static string Serialize(decimal? d) {
			if (d == null) return "0";
			return Serialize((decimal)d);
		}

		/// <summary>Выполняет сериализацию целого числа</summary>
		/// <param name="d">исходное число</param>
		public static string Serialize(int d) {
			return d.ToString();
		}

		/// <summary>Выполняет сериализацию логического значения</summary>
		/// <param name="b">логическое значение</param>
		public static string Serialize(bool b) {
			return b ? "true" : "false";
		}

		/// <summary>Выполняет сериализацию списка</summary>
		/// <param name="lst">список</param>
		public static string Serialize(List<object> lst) {
			StringWriter wrt = new StringWriter();
			wrt.Write("[");
			bool first = true;
			foreach (object o in lst) {
				if (o == null) continue;
				if (first) first = false; else wrt.Write(",");
				wrt.Write(Serialize(o));
			}
			wrt.Write("]");
			return wrt.ToString();
		}

		/// <summary>Выполняет сериализацию словаря</summary>
		/// <param name="dict">исходный словарь</param>
		public static string Serialize(Dictionary<string, object> dict) {
			StringWriter wrt = new StringWriter();
			wrt.Write("{");
			bool first = true;
			foreach (string key in dict.Keys) {
				object o = dict[key];
				if (o == null) continue;
				if (first) first = false; else wrt.Write(",");
				wrt.Write(@"""{0}"":{1}", key, Serialize(o));
			}
			wrt.Write("}");
			return wrt.ToString();
		}

		/// <summary>Выполняет сериализацию произвольного значения</summary>
		public static string Serialize(object o) {
			if (o is string) return Serialize((string)o);
			else if (o is decimal) return Serialize((decimal)o);
			else if (o is decimal?) return Serialize((decimal?)o);
			else if (o is int) return Serialize((int)o);
			else if (o is bool) return Serialize((bool)o);
			else if (o is IJsonSerializable) return ((IJsonSerializable)o).Serialize();
			else return Serialize(o.ToString());
		}
	}

	/// <summary>Объект, сериализуемый в JSON</summary>
	public interface IJsonSerializable {
		/// <summary>Выполняет сериализацию</summary>
		string Serialize();
	}

	/// <summary>Массив JSON</summary>
	public class JsonArray : IJsonSerializable {
		/// <summary>Выполняет сериализацию</summary>
		public string Serialize() {
			return Json.Serialize(array);
		}

		/// <summary>Возвращает размер массива</summary>
		public int Count { get { return array.Count; } }

		/// <summary>Добавляет элемент в массив</summary>
		/// <param name="o">элемент</param>
		public void Add(object o) {
			array.Add(o);
		}

		/// <summary>Добавляет новый объект, и возвращает его</summary>
		public JsonObject AddNewObject() {
			JsonObject jo = new JsonObject();
			array.Add(jo);
			return jo;
		}

		private List<object> array = new List<object>();
	}

	/// <summary>Объект JSON</summary>
	public class JsonObject : IJsonSerializable {

		/// <summary>Выполняет сериализацию</summary>
		public string Serialize() {
			return Json.Serialize(dict);
		}

		/// <summary>Возвращает значение атрибута</summary>
		/// <param name="name">имя атрибута</param>
		public object Get(string name) {
			return dict[name];
		}

		/// <summary>Добавляет атрибут в структуру</summary>
		/// <param name="name">имя атрибута</param>
		/// <param name="o">элемент</param>
		public void Add(string name, object o) {
			dict.Add(name, o);
		}

		/// <summary>Добавляет новый массив в качестве атрибута, и возвращает его</summary>
		/// <param name="name">имя атрибута</param>
		public JsonArray AddNewArray(string name) {
			JsonArray res = new JsonArray();
			dict.Add(name, res);
			return res;
		}

		private Dictionary<string, object> dict = new Dictionary<string, object>();
	}
}
