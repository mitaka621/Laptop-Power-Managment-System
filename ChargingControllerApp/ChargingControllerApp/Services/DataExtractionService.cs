using ChargingControllerApp.Models;
using ChargingControllerApp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargingControllerApp.Services
{
	public class DataExtractionService : IDataExtractionService
	{
		public LaptopDataModel GetData()
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
