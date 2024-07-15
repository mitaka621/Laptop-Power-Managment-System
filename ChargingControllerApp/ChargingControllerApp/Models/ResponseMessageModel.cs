using ChargingControllerApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChargingControllerApp.Models
{
	public class ResponseMessageModel
	{
        [JsonPropertyName("message")]
        public string ResponseMessage { get; set; }=string.Empty;

		[JsonPropertyName("isError")]
		public bool IsError { get; set; }
		[JsonPropertyName("overrideActive")]
		public bool OverrideActive { get; set; }

		[JsonPropertyName("smartChargingStatus")]
		public SmartChargingStates SmartChargingStatus { get; set; }
	}
}
