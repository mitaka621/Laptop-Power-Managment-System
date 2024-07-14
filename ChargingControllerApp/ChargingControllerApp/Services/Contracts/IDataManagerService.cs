using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargingControllerApp.Services.Contracts
{
	public interface IDataManagerService
	{
		void SaveEncryptedToken(string token, string filePath="token.txt");

		string ReadEncryptedToken(string filePath = "token.txt");

		void SaveServerIp(string serverIp, string filePath = "serverInfo.txt");

		string GetServerIp(string filePath = "serverInfo.txt");
	}
}
