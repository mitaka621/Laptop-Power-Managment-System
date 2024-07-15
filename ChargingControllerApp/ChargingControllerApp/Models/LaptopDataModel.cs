using ChargingControllerApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChargingControllerApp.Models
{
	public class LaptopDataModel
	{
		private int batteryPercentage;

		public DateTime CurrentDateTime { get; set; }

		public bool IsCharging { get; set; }

		public int BatteryPercentage
		{
			get => batteryPercentage;
			set
			{
                if (value<0||value>100)
                {
					throw new ArgumentException("The battery percentage is invalid");
				}

				batteryPercentage = value;
            }
		}

		public ChargingModes CurrentChargingMode { get; set; }

        public int MaxPercentage { get; set; }

        public int MinPercentage { get; set; }
    }
}
