using ChargingControllerApp.Enums;
using ChargingControllerApp.Models;
using ChargingControllerApp.Services;
using ChargingControllerApp.Services.Contracts;
using ChargingControllerApp.Utils;
using ChargingControllerApp.Utils.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Text.Json;

namespace ChargingControllerApp
{
	public partial class MainForm : Form
	{
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;

		private readonly IDataExtractionService _extractionService = new DataExtractionService();
		private readonly IDataManagerService _dataManagerService = new DataManagerService();
		private readonly IUIElementsDisplayHelper _uiHelper;

		private IStatusSenderService? _statusSenderService;

		private ChargingModes currentChargingMode = 0;
		private int maxPercentage = 80;
		private int minPercentage = 60;


		public MainForm()
		{
			InitializeComponent();

			_uiHelper = new UIElementsDisplayHelper(
				this,
				ServerConnectedImg,
				ServerDisconnectedImg,
				serverConnectionLoading,
				serverIpInput,
				serverTokentTB,
				connectToServerBn,
				modeSelectorCB,
				batteryMaxSlider,
				batteryMinSlider,
				guna2TabControl1,
				statusMessageTB,
				guna2HtmlToolTip3,
				ModeInfoToolTip,
				mainTimer,
				NotChargingImg,
				DischargingImg,
				ErrorChargingImg,
				ChargingImg
			);

			StartPosition = FormStartPosition.Manual;
			if (Screen.PrimaryScreen != null)
			{
				Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
				int x = workingArea.Right - Width;
				int y = workingArea.Bottom - Height;
				Location = new Point(x, y);
			}

			FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

			textOverflowTimer.Start();

			LoadUserConfig();

			if (File.Exists("serverInfo.txt") && File.Exists("token.txt"))
			{
				_uiHelper.DisplayErrorStatus("Trying to connect....");

				_uiHelper.DisableServerConnectionInput();

				_uiHelper.EnableBatteryInputs();

				try
				{
					_statusSenderService = new StatusSenderService(_extractionService, _dataManagerService);
				}
				catch (Exception ex)
				{
					_uiHelper.DisplayServerConnectionStateLost(ex.Message);
					return;
				}

				mainTimer.Start();
			}
		}

		private async void MainTimer_Tick(object sender, EventArgs e)
		{
			mainTimer.Stop();
			if (_statusSenderService == null)
			{
				_uiHelper.DisplayServerConnectionStateLost("Failed to send request to server");
				return;
			}

			try
			{
				var response = await _statusSenderService.SendLaptopData(currentChargingMode, minPercentage, maxPercentage);

				if (response == null)
				{
					_uiHelper.DisplayErrorStatus("Could not parse response!");
					return;
				}

				_uiHelper.DisplayChargingState(response.SmartChargingStatus);
				_uiHelper.DisplayServerConnectionState(true);

				if (response.OverrideActive)
				{
					_uiHelper.DisplayErrorStatus("Override activated. Controls disabled!");
					_uiHelper.DisableBatteryInputs();

					mainTimer.Start();
					return;
				}
				else
				{
					_uiHelper.SelectMode((ChargingModes)modeSelectorCB.SelectedIndex);
				}

				if (response.IsError)
				{
					_uiHelper.DisplayErrorStatus(response.ResponseMessage);
					_uiHelper.ShowApp();
				}
				else
				{
					_uiHelper.DisplayOkStatus(response.ResponseMessage);
				}
			}
			catch (Exception ex)
			{
				_uiHelper.DisplayServerConnectionStateLost("The request was not successful - " + ex.Message);
			}

			mainTimer.Start();
		}

		protected override void WndProc(ref Message m)
		{
			// Ignore the WM_NCLBUTTONDOWN message to prevent moving the form
			if (m.Msg == WM_NCLBUTTONDOWN && (int)m.WParam == HTCAPTION)
			{
				return;
			}
			base.WndProc(ref m);
		}

		private void notifyIcon_MouseDoubleClick(object Sender, EventArgs e)
		{
			_uiHelper.ShowApp();
			Activate();
		}

		private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			_uiHelper.HideApp();
		}

		private void exitToolStripMenuItem_Click(object? sender, EventArgs e)
		{
			DialogResult dialog = MessageBox.Show("If you exit the app you will lose control over the LPM System! Do you still want to exit?", "Alert!", MessageBoxButtons.YesNo);

			if (dialog == DialogResult.Yes)
			{
				Environment.Exit(1);
			}
		}

		private void guna2Button2_Click(object sender, EventArgs e)
		{
			Hide();
		}

		private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
		{
			int value = batteryMaxSlider.Value;
			int snapInterval = 5;
			int remainder = value % snapInterval;

			if (remainder != 0)
			{
				if (remainder >= snapInterval / 2)
				{
					value = value + (snapInterval - remainder);
				}
				else
				{
					value = value - remainder;
				}

				batteryMaxSlider.Value = value;
			}

			MaxBatteryLabel.Text = value.ToString() + "%";

			maxPercentage = value;

			SaveUserConfig();
		}

		private void batteryMinSlider_Scroll(object sender, ScrollEventArgs e)
		{
			int value = batteryMinSlider.Value;
			int snapInterval = 5;
			int remainder = value % snapInterval;

			if (remainder != 0)
			{
				if (remainder >= snapInterval / 2)
				{
					value = value + (snapInterval - remainder);
				}
				else
				{
					value = value - remainder;
				}

				batteryMinSlider.Value = value;
			}

			MinBatteryLabel.Text = value.ToString() + "%";

			minPercentage = value;

			SaveUserConfig();
		}

		private void guna2TextBox3_TextChanged(object sender, EventArgs e)
		{
			if (!ValidationHelper.IsIpValid(serverIpInput.Text))
			{
				ipErrorLabel.Text = "A local ip in the format 111.111.111.111 expected!";
				connectToServerBn.Enabled = false;

				return;
			}

			ipErrorLabel.Text = "";
			connectToServerBn.Enabled = true;
		}

		private void textOverflowTimer_Tick(object sender, EventArgs e)
		{
			if (statusMessageTB.Text.Length > 40)
			{
				string? newStr = new string(statusMessageTB.Text.Skip(1).Append(statusMessageTB.Text[0]).ToArray());

				statusMessageTB.Text = newStr;
			}
		}

		private async void ConnectToServerBn_Click(object sender, EventArgs e)
		{
			_uiHelper.DisplayServerConnectionStateLoading(true);
			_dataManagerService.SaveEncryptedToken(serverTokentTB.Text);
			_dataManagerService.SaveServerIp(serverIpInput.Text);

			_statusSenderService = new StatusSenderService(_extractionService, _dataManagerService);

			var result = await _statusSenderService.CheckStatus();

			_uiHelper.DisplayServerConnectionStateLoading(false);
			_uiHelper.DisplayServerConnectionState(result);

			if (result)
			{
				_uiHelper.DisableServerConnectionInput();

				_uiHelper.EnableBatteryInputs();

				mainTimer.Start();

				_uiHelper.DisplayOkStatus("Connected");
			}
			else
			{
				_uiHelper.DisplayErrorStatus("Could not reach server...");
			}
		}

		private void modeSelectorCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			currentChargingMode = (ChargingModes)modeSelectorCB.SelectedIndex;

			_uiHelper.SelectMode(currentChargingMode);

			SaveUserConfig();
		}

		private void SaveUserConfig()
		{
			var userConfig = new UserConfigModel()
			{
				ChargingMode = (ChargingModes)modeSelectorCB.SelectedIndex,
				MinSliderValue = minPercentage,
				MaxSliderValue = maxPercentage,
			};

			string jsonString = JsonSerializer.Serialize(userConfig);

			BsonDocument document = BsonDocument.Parse(jsonString);

			File.WriteAllBytes("userconfig.dat", document.ToBson());
		}

		private void LoadUserConfig()
		{
			if (!File.Exists("userconfig.dat"))
			{
				return;
			}

			byte[] bsonData = File.ReadAllBytes("userconfig.dat");

			UserConfigModel? model = BsonSerializer.Deserialize<UserConfigModel>(bsonData);

			if (model is null)
			{
				return;
			}

			minPercentage = model.MinSliderValue;
			batteryMinSlider.Value = model.MinSliderValue;
			MinBatteryLabel.Text = model.MinSliderValue.ToString() + "%";

			maxPercentage = model.MaxSliderValue;
			batteryMaxSlider.Value = model.MaxSliderValue;
			MaxBatteryLabel.Text = model.MaxSliderValue.ToString() + "%";

			_uiHelper.SelectMode(model.ChargingMode);
		}
	}
}
