using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.SQLite.Manager
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Security.Cryptography;
	using System.Text;

	public class AesEncryption
	{
		private static byte[] s_key = Encoding.UTF8.GetBytes("key+");
		private static byte[] s_iv = Encoding.UTF8.GetBytes("key+");

		public static string FromBytes(byte[] bytes)
		{
			return System.Text.Encoding.ASCII.GetString(bytes);
		}
		public static byte[] GenerateIV()
		{
			byte[] iv;
			using (Aes aes = Aes.Create())
			{
				aes.GenerateIV();
				iv = aes.IV;
			}
			return s_iv = iv;
		}

		public static byte[] GenerateKey()
		{
			byte[] key;
			using (Aes aes = Aes.Create())
			{
				aes.GenerateKey();
				key = aes.Key;
			}
			return s_key = key;
		}

		public static string Encrypt(string plainText)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = s_key;
				aes.IV = s_iv;

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(plainText);
						}
						return Convert.ToBase64String(msEncrypt.ToArray());
					}
				}
			}
		}

		public static string Decrypt(string cipherText)
		{
			cipherText = cipherText.Replace(" ", "+");
			byte[] cipherBytes = Convert.FromBase64String(cipherText);

			using (Aes aes = Aes.Create())
			{
				aes.Key = s_key;
				aes.IV = s_iv;

				ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

				using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{
							return srDecrypt.ReadToEnd();
						}
					}
				}
			}
		}
	}
}
