using ChargingControllerApp.Enums;
using ChargingControllerApp.Models;
using ChargingControllerApp.Services.Contracts;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ChargingControllerApp.Services
{
	public class StatusSenderService : IStatusSenderService
	{
		private readonly IDataExtractionService _dataExtractionService;
		private readonly IDataManagerService _dataManagerService;
		private readonly HttpClient _httpClient = new();

		public StatusSenderService(IDataExtractionService dataExtractionService, IDataManagerService dataManagerService)
		{
			_dataExtractionService = dataExtractionService;
			_dataManagerService = dataManagerService;

			_httpClient.BaseAddress = new Uri($"http://{_dataManagerService.GetServerIp()}");
			_httpClient.DefaultRequestHeaders.Add("token", _dataManagerService.ReadEncryptedToken());
		}

		public async Task<ResponseMessageModel?> SendLaptopData(ChargingModes chargingMode, int minPercentage, int maxPercentage)
		{
			var laptopData = _dataExtractionService.GetBasicBatteryData();
			laptopData.CurrentChargingMode = chargingMode;
			laptopData.MinPercentage = minPercentage;
			laptopData.MaxPercentage = maxPercentage;

			StringContent content = new(JsonSerializer.Serialize(laptopData), Encoding.UTF8, "application/json");

			var reponse = await _httpClient.PostAsync("/data", content);
			string body = await reponse.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<ResponseMessageModel>(body);
		}

		public async Task<bool> CheckStatus()
		{
			try
			{
				var response = await _httpClient.GetAsync("/");

				if (response.StatusCode == HttpStatusCode.OK && (await response.Content.ReadAsStringAsync()) == "ESP Power Manager - status OK")
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
