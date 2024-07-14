using ChargingControllerApp.Models;
using ChargingControllerApp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChargingControllerApp.Services
{
	public class StatusSenderService : IStatusSenderService
	{
		private readonly IDataExtractionService _dataExtractionService;
		private readonly IDataManagerService _dataManagerService;
		private readonly HttpClient _httpClient=new();

        public StatusSenderService(IDataExtractionService dataExtractionService, IDataManagerService dataManagerService)
        {
			_dataExtractionService=dataExtractionService;
			_dataManagerService = dataManagerService;

			_httpClient.BaseAddress = new Uri($"http://{_dataManagerService.GetServerIp()}");
			_httpClient.DefaultRequestHeaders.Add("token", _dataManagerService.ReadEncryptedToken());
		}

        public async Task<ResponseMessageModel?> SendLaptopData()
		{
			var laptopData = _dataExtractionService.GetBasicBatteryData();

			StringContent content = new(JsonSerializer.Serialize(laptopData), Encoding.UTF8, "application/json");

			var reponse= await _httpClient.PostAsync("/data",content);
			return JsonSerializer.Deserialize<ResponseMessageModel>(await reponse.Content.ReadAsStringAsync());
        }

		public async Task<bool> CheckStatus()
		{
			try
			{
				var response = await _httpClient.GetAsync("/");

                if (response.StatusCode==HttpStatusCode.OK)
                {
					return true;
                }

				return false;
            }
			catch (Exception)
			{
				return false;
			}			
		}
	}
}
