using ChargingControllerApp.Models;
using ChargingControllerApp.Services.Contracts;

namespace ChargingControllerApp.Services
{
	public class DataExtractionService : IDataExtractionService
	{
		public LaptopDataModel GetBasicBatteryData()
		{
			PowerStatus powerStatus = SystemInformation.PowerStatus;

			return new LaptopDataModel()
			{
				BatteryPercentage = (int)(powerStatus.BatteryLifePercent * 100),
				IsCharging = powerStatus.PowerLineStatus == PowerLineStatus.Online,
				CurrentDateTime = DateTime.Now,
			};
		}
	}
}
