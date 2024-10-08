﻿using System.Net;
using System.Text.RegularExpressions;

namespace ChargingControllerApp.Utils
{
	public static class ValidationHelper
	{
		public static bool IsIpValid(string ip)
		{
			string regPattern = @"\d{3}\.\d{3}\.\d{1,3}\.\d{1,3}";

			Regex reg = new(regPattern);

			if (!reg.IsMatch(ip) || !IPAddress.TryParse(ip, out IPAddress? parsedIp))
			{
				return false;
			}

			byte[] bytes = parsedIp.GetAddressBytes();
			if (bytes.Length != 4) // Only IPv4
			{
				return false;
			}

			// Check for local IP ranges
			return
				(bytes[0] == 10) || // 10.0.0.0 to 10.255.255.255
				(bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) || // 172.16.0.0 to 172.31.255.255
				(bytes[0] == 192 && bytes[1] == 168); // 192.168.0.0 to 192.168.255.255
		}
	}
}
