using ChargingControllerApp.Enums;
using ChargingControllerApp.Models;

namespace ChargingControllerApp.Services.Contracts
{
	public interface IStatusSenderService
	{
		Task<ResponseMessageModel?> SendLaptopData(ChargingModes currentChargingMode, int minPercentage, int maxPercentage);

		Task<bool> CheckStatus();
	}
}
