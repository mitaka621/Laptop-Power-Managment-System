﻿using ChargingControllerApp.Enums;
using ChargingControllerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargingControllerApp.Services.Contracts
{
	public interface IStatusSenderService
	{
		Task<ResponseMessageModel?> SendLaptopData(ChargingModes currentChargingMode, int minPercentage, int maxPercentage);

		Task<bool> CheckStatus();
	}
}
