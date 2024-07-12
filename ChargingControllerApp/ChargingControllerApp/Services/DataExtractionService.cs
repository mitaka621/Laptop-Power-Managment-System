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
	}
}
