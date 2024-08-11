using ChargingControllerApp.Models;

namespace ChargingControllerApp.Services.Contracts
{
	public interface IDataExtractionService
	{
		LaptopDataModel GetBasicBatteryData();

	}
}
