using ChargingControllerApp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChargingControllerApp.Services
{
	public class StatusSenderService : IStatusSenderService
	{
		private readonly IDataExtractionService _dataExtractionService;
		private readonly HttpClient _httpClient=new();

        public StatusSenderService(IDataExtractionService dataExtractionService, string serverIp)
        {
			_dataExtractionService=dataExtractionService;

			_httpClient.BaseAddress = new Uri($"http://{serverIp}");
		}

        public async Task<bool> SendLaptopData()
		{
			var laptopData = _dataExtractionService.GetData();

			StringContent content = new(JsonSerializer.Serialize(laptopData), Encoding.UTF8, "application/json");

			var response=await _httpClient.PostAsync("/data",content);

            if (response.StatusCode==HttpStatusCode.OK)
            {
                return true;
            }

			return false;
        }
	}
}
