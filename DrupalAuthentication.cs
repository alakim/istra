using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Istra.Drupal {
	/// <summary>Утилита для аутентификации пользователей сайтов на базе Drupal</summary>
	public static class DrupalAuthentication {

		/// <summary>Возвращает хэш заданного пароля</summary>
		/// <param name="password">пароль</param>
		public static string HashPassword(string password) {
			string hash = user_hash_password(password);
			return hash;
		}

		
		public static bool CheckPassword(string password, string hashedPass) {
			string stored_hash;
			if (hashedPass.Substring(0, 2) == "U$") {
				stored_hash = hashedPass.Substring(1);
				//*** password = md5(password);
				HMACMD5 md5 = new HMACMD5();
				md5.ComputeHash(Encoding.ASCII.GetBytes(password));
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


		//**** public static bool Test1() { //Тест проходит
		//**** 	byte[] bytes = new byte[]{113, 10, 244, 37, 149, 146, 84, 70, 139, 91, 15, 44, 157, 141, 63, 48, 178, 238, 212, 4, 134, 158, 144, 165, 237, 199, 4, 161, 218, 7, 196, 111, 145, 152, 49, 49, 31, 135, 187, 162, 157, 193, 60, 248, 44, 65, 196, 234, 40, 219, 183, 48, 244, 8, 139, 95, 191, 80, 117, 195, 11, 175, 114, 89};
		//**** 	string expected = @"ld.xZIdYINoWPx.9RqsDk6fvIHUVS0NdhTA/Vex/2zKYM4HATQsiWqNkwUD9/FgucgxhkED09ypjEJrk9weQN/";
		//**** 	string res = _password_base64_encode(bytes, 64);
		//**** 	return res == expected;
		//**** }


		private static object variable_get(string name) {
			return variable_get(name, null);
		}

		private static object variable_get(string name, object defaultVal) {
			if (conf.ContainsKey(name)) return conf[name];
			return defaultVal;
		}

		private static string user_hash_password(string password) {
			return user_hash_password(password, 0);
		}
		private static string user_hash_password(string password, int count_log2) {
			if (count_log2==0) {
				count_log2 = (int)variable_get("password_count_log2", DRUPAL_HASH_COUNT);
			}
			string salt = _password_generate_salt(count_log2);
			if (salt.Length != saltLength) throw new ApplicationException("Bad salt length");
			return _password_crypt("sha512", password, salt);
		}

		private static string _password_generate_salt(int count_log2) {
			string output = "$S$";
			count_log2 = _password_enforce_log2_boundaries(count_log2);
			string itoa64 = _password_itoa64();
			output += itoa64[count_log2];
			output += _password_base64_encode(drupal_random_bytes(6), 6);
			return output;
		}

		private static int _password_get_count_log2(string setting) {
			string itoa64 = _password_itoa64();
			return itoa64.IndexOf(setting[3]); // return strpos($itoa64, $setting[3]);
		}

		private static string _password_itoa64() {
			return "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		}

		private static byte ord(byte[] str, int pos){
			//return Encoding.ASCII.GetBytes(str.Substring(pos, 1))[0];
			return str[pos];
		}

		/// <summary>Преобразует массив в строку в кодировке base64</summary>
		/// <param name="input"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		private static string _password_base64_encode(byte[] input, int count) {
#if STD_BASE64
			return Convert.ToBase64String(input, 0, count);
#else
			// *****************************************************************************
			// ****** Протестировано - см. Test1. Соответствует оригиналу в Drupal *********
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
#endif
		}

		private static int _password_enforce_log2_boundaries(int count_log2) {
			if (count_log2 < DRUPAL_MIN_HASH_COUNT) {return DRUPAL_MIN_HASH_COUNT;}
			else if (count_log2 > DRUPAL_MAX_HASH_COUNT) {return DRUPAL_MAX_HASH_COUNT;}
			return (int) count_log2;
		}

		// private static string _hash(string algo, string str, bool rawOutput){
		// 	if(rawOutput) return _hash(algo, str);
		// 	else return Encoding.ASCII.GetString(_hash(algo, str));
		// }

		private static string hashToString(byte[] hash) {
			return Encoding.ASCII.GetString(hash);
		}

		private static byte[] _hash(string algo, string str){
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

		private static string _password_crypt(string algo, string password, string setting) {
			setting = setting.Substring(0, saltLength); //substr(setting, 0, 12);

			if (setting[0] != '$' || setting[2] != '$') throw new ApplicationException("Bad Password Setting"); //{return false;}
	
			int count_log2 = _password_get_count_log2(setting);
			if (count_log2 < DRUPAL_MIN_HASH_COUNT || count_log2 > DRUPAL_MAX_HASH_COUNT) throw new ApplicationException("Bad Password Count"); //{return FALSE;}
	
			string salt = setting.Substring(4, 8); //substr($setting, 4, 8);
			if (salt.Length != 8) throw new ApplicationException("Bad Salt"); //return FALSE;
	
			int count = 1 << count_log2;
			byte[] hash = _hash(algo, salt + password);
			do {
				hash = _hash(algo, hash + password);
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



		private static byte[] drupal_random_bytes(int count)  {
			return Encoding.ASCII.GetBytes(RandomString(count));
		}

		private static string RandomString(int size){
			StringBuilder builder = new StringBuilder();
			Random random = new Random();
			char ch;
			for (int i = 0; i < size; i++){
				ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));                 
				builder.Append(ch);
			}
			return builder.ToString();
		}

		private static Dictionary<string, object> conf = new Dictionary<string,object>();

		private static int DRUPAL_HASH_COUNT = 15;
		private static int DRUPAL_MIN_HASH_COUNT = 7;
		private static int DRUPAL_MAX_HASH_COUNT = 30;
		private static int DRUPAL_HASH_LENGTH = 55;
	}
}
