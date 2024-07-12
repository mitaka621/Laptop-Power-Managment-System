using ChargingControllerApp.Models;
using ChargingControllerApp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ChargingControllerApp.Services
{
	public class DataExtractionService : IDataExtractionService
	{
		public LaptopDataModel GetBasicBatteryData()
		{
			PowerStatus powerStatus = SystemInformation.PowerStatus;

			return new LaptopDataModel()
			{
				BatteryPercentage = (int)powerStatus.BatteryLifePercent * 100,
				IsCharging = powerStatus.PowerLineStatus == PowerLineStatus.Online,
				CurrentDateTime= DateTime.Now,
			};
		}

		public int GetDesignedBatteryCapacity()
		{
			return GetBatteryData("DesignCapacity");
		}

		public int GetFullChargeCapacity()
		{
			return GetBatteryData("FullChargeCapacity");
		}

		private int GetBatteryData(string propertyName)
		{
			int capacity = -1;
			try
			{
				ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery");

				foreach (ManagementObject queryObj in searcher.Get())
				{
					capacity = Convert.ToInt32(queryObj[propertyName]);
				}
			}
			catch (Exception)
			{
				return -1;
			}

			return capacity;
		}
	}
}
