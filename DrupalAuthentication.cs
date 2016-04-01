using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Istra.Drupal {
	/// <summary>Утилита для аутентификации пользователей сайтов на базе Drupal</summary>
	public class DrupalAuthentication {

		public DrupalAuthentication() {
		}

		/// <summary>Конструктор</summary>
		/// <param name="randomMode">включает (true) режим генерации случайных строк при кодировании. По умолчанию включен.</param>
		public DrupalAuthentication(bool randomMode) {
			this.randomMode = randomMode;
		}

		/// <summary>включает (true) режим генерации случайных строк при кодировании. По умолчанию включен.</summary>
		private bool randomMode = true;

		/// <summary>Возвращает хэш заданного пароля</summary>
		/// <param name="password">пароль</param>
		public string HashPassword(string password) {
			string hash = user_hash_password(password);
			return hash;
		}

		
		public bool CheckPassword(string password, string hashedPass) {
			string stored_hash;
			if (hashedPass.Substring(0, 2) == "U$") {
				stored_hash = hashedPass.Substring(1);
				//*** password = md5(password);
				HMACMD5 md5 = new HMACMD5();
				byte[] bPass = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
				password = Encoding.ASCII.GetString(bPass);
			}
			else {
				stored_hash = hashedPass;
			}
			string type = stored_hash.Substring(0, 3);
			string hash;
			switch (type) {
				case "$S$":
					hash = _password_crypt("sha512", password, stored_hash);
					break;
				case "$H$":
				case "$P$":
					hash = _password_crypt("md5", password, stored_hash);
					break;
				default:
					throw new ApplicationException("Hashing algorythm not supported.");
			}
			return (hash!=null && stored_hash == hash);
		}


		public bool Test_base64_encode() { //Тест проходит
			byte[] bytes = new byte[]{113, 10, 244, 37, 149, 146, 84, 70, 139, 91, 15, 44, 157, 141, 63, 48, 178, 238, 212, 4, 134, 158, 144, 165, 237, 199, 4, 161, 218, 7, 196, 111, 145, 152, 49, 49, 31, 135, 187, 162, 157, 193, 60, 248, 44, 65, 196, 234, 40, 219, 183, 48, 244, 8, 139, 95, 191, 80, 117, 195, 11, 175, 114, 89};
			string expected = @"ld.xZIdYINoWPx.9RqsDk6fvIHUVS0NdhTA/Vex/2zKYM4HATQsiWqNkwUD9/FgucgxhkED09ypjEJrk9weQN/"; // то, что генерирует Drupal
			string res = _password_base64_encode(bytes, 64);
			return res == expected;
		}


		private object variable_get(string name) {
			return variable_get(name, null);
		}

		private object variable_get(string name, object defaultVal) {
			if (conf.ContainsKey(name)) return conf[name];
			return defaultVal;
		}

		private string user_hash_password(string password) {
			return user_hash_password(password, 0);
		}
		private string user_hash_password(string password, int count_log2) {
			if (count_log2==0) {
				count_log2 = (int)variable_get("password_count_log2", DRUPAL_HASH_COUNT);
			}
			string salt = _password_generate_salt(count_log2);
			if (salt.Length != saltLength) throw new ApplicationException("Bad salt length");
			return _password_crypt("sha512", password, salt);
		}

		private string _password_generate_salt(int count_log2) {
			if (randomMode) {
				string output = "$S$";
				count_log2 = _password_enforce_log2_boundaries(count_log2);
				string itoa64 = _password_itoa64();
				output += itoa64[count_log2];
				output += _password_base64_encode(drupal_random_bytes(6), 6);
				return output;
			}
			else
				return "$S$DDpIJJVIH";
		}

		private int _password_get_count_log2(string setting) {
			string itoa64 = _password_itoa64();
			return itoa64.IndexOf(setting[3]); // return strpos($itoa64, $setting[3]);
		}

		private string _password_itoa64() {
			return "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		}

		private byte ord(byte[] str, int pos){
			//return Encoding.ASCII.GetBytes(str.Substring(pos, 1))[0];
			return str[pos];
		}

		/// <summary>Преобразует массив в строку в кодировке base64</summary>
		/// <param name="input"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		private string _password_base64_encode(byte[] input, int count) {
			//*********** return Convert.ToBase64String(input, 0, count); // Возможное стандартное решение

			// *****************************************************************************
			// ****** Протестировано - см. Test_base64_encode. Соответствует оригиналу в Drupal *********
			// *****************************************************************************
			string output = string.Empty;
			int i = 0;
			string itoa64 = _password_itoa64();
			int mask = 0x3f; // шестибитная маска
			do {
				int _value = ord(input, i++);
				output += itoa64[_value & mask];

				if (i < count) { _value |= (int)(ord(input, i) << 8); }
				output += itoa64[(_value >> 6) & mask];

				if (i++ >= count) {break;}

				if (i < count) { _value |= (int)(ord(input, i) << 16); }
				output += itoa64[(_value >> 12) & mask];

				if (i++ >= count) {break;}
				output += itoa64[(_value >> 18) & mask];

			} while (i < count);
			return output;
		}

		private int _password_enforce_log2_boundaries(int count_log2) {
			if (count_log2 < DRUPAL_MIN_HASH_COUNT) {return DRUPAL_MIN_HASH_COUNT;}
			else if (count_log2 > DRUPAL_MAX_HASH_COUNT) {return DRUPAL_MAX_HASH_COUNT;}
			return (int) count_log2;
		}

		// private static string _hash(string algo, string str, bool rawOutput){
		// 	if(rawOutput) return _hash(algo, str);
		// 	else return Encoding.ASCII.GetString(_hash(algo, str));
		// }

		private string hashToString(byte[] hash) {
			return Encoding.ASCII.GetString(hash);
		}

		private byte[] _hash(string algo, string str){
			byte[] bytes;
			switch(algo){
				case "md5":
					HMACMD5 md5 = new HMACMD5();
					bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
					break;
				case "sha512":
					HMACSHA512 sha512 = new HMACSHA512();
					bytes = sha512.ComputeHash(Encoding.ASCII.GetBytes(str));
					break;
				default:
					throw new ApplicationException("Hash algorythm not supported");
			}
			return bytes;
		}
		
		/// <summary>Стандартная и ожидаемая длина соли</summary>
		private const int saltLength = 12;

		private string _password_crypt(string algo, string password, string setting) {
			setting = setting.Substring(0, saltLength); //substr(setting, 0, 12);

			if (setting[0] != '$' || setting[2] != '$') throw new ApplicationException("Bad Password Setting"); //{return false;}
	
			int count_log2 = _password_get_count_log2(setting);
			if (count_log2 < DRUPAL_MIN_HASH_COUNT || count_log2 > DRUPAL_MAX_HASH_COUNT) throw new ApplicationException("Bad Password Count"); //{return FALSE;}
	
			string salt = setting.Substring(4, 8); //substr($setting, 4, 8);
			if (salt.Length != 8) throw new ApplicationException("Bad Salt"); //return FALSE;
	
			int count = randomMode? 1 << count_log2 : 1;

			byte[] hash = _hash(algo, salt + password);
			do {
				hash = _hash(algo, hashToString(hash) + password);
			} while (--count>0);

			int len = hash.Length;
			string b64 = _password_base64_encode(hash, len);
			//b64 = b64.Substring(0, 85); // заплатка
			string output =  setting + b64;

			double dd = (8 * len) / 6.0;
			int expected = saltLength + (int)Math.Ceiling(dd);
			if (output.Length == expected)
				return output.Substring(0, DRUPAL_HASH_LENGTH);
			else throw new ApplicationException("Unexpected output length.");
		}



		private byte[] drupal_random_bytes(int count){
			return Encoding.ASCII.GetBytes(RandomString(count));
		}

		private string RandomString(int size){
			StringBuilder builder = new StringBuilder();
			if (randomMode) {
				Random random = new Random();
				char ch;
				for (int i = 0; i < size; i++){
					ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));                 
					builder.Append(ch);
				}
			}
			else {
				for (int i = 0; i < size; i++) { // для тестирования
					builder.Append("X");
				}
			}
			return builder.ToString();
		}

		private Dictionary<string, object> conf = new Dictionary<string,object>();

		private const int DRUPAL_HASH_COUNT = 15;
		private const int DRUPAL_MIN_HASH_COUNT = 7;
		private const int DRUPAL_MAX_HASH_COUNT = 30;
		private const int DRUPAL_HASH_LENGTH = 55;
	}
}
