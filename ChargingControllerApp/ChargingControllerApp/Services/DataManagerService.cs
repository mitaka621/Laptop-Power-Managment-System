using ChargingControllerApp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChargingControllerApp.Services
{
	public class DataManagerService : IDataManagerService
	{
		public void SaveEncryptedToken(string token, string filePath = "token.txt")
		{
			using (Aes aes = Aes.Create())
			{
				aes.GenerateKey();
				aes.GenerateIV();

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter sw = new StreamWriter(cs))
						{
							sw.Write(token);
						}
					}

					byte[] encryptedToken = ms.ToArray();

					using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
					{
						fs.Write(aes.Key, 0, aes.Key.Length);
						fs.Write(aes.IV, 0, aes.IV.Length);

						fs.Write(encryptedToken, 0, encryptedToken.Length);
					}
				}
			}
		}

		public string ReadEncryptedToken(string filePath = "token.txt")
		{
			using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				using (Aes aes = Aes.Create())
				{
					byte[] key = new byte[32]; // AES-256 key size
					byte[] iv = new byte[16];  // AES block size

					// Read the key and IV from the file
					fs.Read(key, 0, key.Length);
					fs.Read(iv, 0, iv.Length);

					aes.Key = key;
					aes.IV = iv;

					ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

					using (MemoryStream ms = new MemoryStream())
					{
						byte[] buffer = new byte[fs.Length - fs.Position];
						fs.Read(buffer, 0, buffer.Length);

						using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
						{
							cs.Write(buffer, 0, buffer.Length);
						}

						return Encoding.UTF8.GetString(ms.ToArray());
					}
				}
			}
		}

		public string GetServerIp(string filePath = "serverInfo.txt")
		{
			return File.ReadAllText(filePath);
		}

		public void SaveServerIp(string serverIp, string filePath = "serverInfo.txt")
		{
			File.WriteAllText(filePath, serverIp);
		}
	}
}
