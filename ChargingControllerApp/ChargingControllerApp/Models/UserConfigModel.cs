using ChargingControllerApp.Enums;

namespace ChargingControllerApp.Models
{
	public class UserConfigModel
	{

		public ChargingModes ChargingMode { get; set; }

		private int maxSliderValue;
		public int MaxSliderValue
		{
			get => maxSliderValue; set
			{
				if (value < 50 || value > 90)
				{
					throw new ArgumentOutOfRangeException("MaxSliderValue");
				}
				maxSliderValue = value;
			}
		}

		private int minSliderValue;
		public int MinSliderValue
		{
			get => minSliderValue; set
			{
				if (value < 40 || value > 80)
				{
					throw new ArgumentOutOfRangeException("minSliderValue");
				}
				minSliderValue = value;
			}
		}
	}
}
