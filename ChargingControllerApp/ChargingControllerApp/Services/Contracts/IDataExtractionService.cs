using ChargingControllerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargingControllerApp.Services.Contracts
{
	public interface IDataExtractionService
	{
		LaptopDataModel GetBasicBatteryData();

		int GetDesignedBatteryCapacity();

		int GetFullChargeCapacity();
	}
}
