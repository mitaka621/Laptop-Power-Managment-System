using ChargingControllerApp.Enums;
using ChargingControllerApp.Services;
using ChargingControllerApp.Services.Contracts;
using ChargingControllerApp.Utils;

namespace ChargingControllerApp
{
	public partial class MainForm : Form
	{
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;

		private readonly IDataExtractionService _extractionService = new DataExtractionService();
		private readonly IDataManagerService _dataManagerService = new DataManagerService();

		private IStatusSenderService? _statusSenderService;

		private ChargingModes currentChargingMode = 0;
		private int maxPercentage = 80;
		private int minPercentage = 60;

		public MainForm()
		{
			InitializeComponent();

			StartPosition = FormStartPosition.Manual;

			if (Screen.PrimaryScreen != null)
			{
				Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
				int x = workingArea.Right - Width;
				int y = workingArea.Bottom - Height;
				Location = new Point(x, y);
			}

			MinimizeBox = false;
			HideApp();

			FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

			textOverflowTimer.Start();

			if (File.Exists("misc.txt"))
			{
				int savedMode = int.Parse(File.ReadAllText("misc.txt"));

				SelectMode(savedMode);

				modeSelectorCB.SelectedIndex = savedMode;
			}

			if (File.Exists("serverInfo.txt") && File.Exists("token.txt"))
			{
				DisplayErrorStatus("Trying to connect....");

				DisableServerConnectionInput();

				EnableBatteryInputs();

				try
				{
					_statusSenderService = new StatusSenderService(_extractionService, _dataManagerService);
				}
				catch (Exception ex)
				{
					DisplayServerConnectionStateLost(ex.Message);
					return;
				}

				mainTimer.Start();
			}

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
			ShowApp();
			Activate();
		}

		private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			HideApp();
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

		private void HideApp()
		{
			WindowState = FormWindowState.Minimized;
			ShowInTaskbar = false;
			Visible = false;
		}

		private void ShowApp()
		{
			WindowState = FormWindowState.Normal;
			ShowInTaskbar = false;
			Visible = true;
		}

		private void DisplayServerConnectionState(bool isConnected)
		{
			if (isConnected)
			{
				ServerConnectedImg.Visible = true;
				ServerDisconnectedImg.Visible = false;
			}
			else
			{
				ServerConnectedImg.Visible = false;
				ServerDisconnectedImg.Visible = true;
			}
		}

		private void DisplayServerConnectionStateLoading(bool show)
		{
			serverConnectionLoading.Visible = show;
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
		}

		private async void MainTimer_Tick(object sender, EventArgs e)
		{
			mainTimer.Stop();
			if (_statusSenderService == null)
			{
				DisplayServerConnectionStateLost("Failed to send request to server");
				return;
			}

			try
			{
				var response = await _statusSenderService.SendLaptopData(currentChargingMode, minPercentage, maxPercentage);

				if (response == null)
				{
					DisplayErrorStatus("Could not parse response!");
					return;
				}

				DisplayChargingState(response.SmartChargingStatus);
				DisplayServerConnectionState(true);

				if (response.OverrideActive)
				{
					DisplayErrorStatus("Override activated. Controls disabled!");
					DisableBatteryInputs();

					mainTimer.Start();
					return;
				}
				else
				{
					SelectMode(modeSelectorCB.SelectedIndex);
				}

				if (response.IsError)
				{
					DisplayErrorStatus(response.ResponseMessage);
					ShowApp();
				}
				else
				{
					DisplayOkStatus(response.ResponseMessage);
				}
			}
			catch (Exception ex)
			{
				DisplayServerConnectionStateLost("The request was not successful - " + ex.Message);
			}

			mainTimer.Start();
		}

		private void DisableServerConnectionInput()
		{
			serverIpInput.Enabled = false;
			serverTokentTB.Enabled = false;
			connectToServerBn.Enabled = false;

			connectToServerBn.Text = "Already Connected";
		}

		private void EnableServerConnectionInput()
		{
			serverIpInput.Enabled = true;
			serverTokentTB.Enabled = true;

			connectToServerBn.Text = "Connect";
		}

		private void DisableBatteryInputs()
		{
			modeSelectorCB.Enabled = false;
			batteryMaxSlider.Enabled = false;
			batteryMinSlider.Enabled = false;
		}

		private void EnableBatteryInputs()
		{
			modeSelectorCB.Enabled = true;
			batteryMaxSlider.Enabled = true;
			batteryMinSlider.Enabled = true;
		}

		private void DisplayServerConnectionStateLost(string message)
		{
			DisplayServerConnectionState(false);
			ShowApp();
			guna2TabControl1.SelectTab(1);
			EnableServerConnectionInput();
			DisplayErrorStatus(message);
			DisableBatteryInputs();

			mainTimer.Stop();
		}

		private void DisplayErrorStatus(string message)
		{
			statusMessageTB.Text = message;
			statusMessageTB.DisabledState.FillColor = Color.FromArgb(255, 189, 191);
		}

		private void DisplayOkStatus(string message)
		{
			statusMessageTB.Text = message;
			statusMessageTB.DisabledState.FillColor = Color.FromArgb(218, 254, 225);
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
			DisplayServerConnectionStateLoading(true);
			_dataManagerService.SaveEncryptedToken(serverTokentTB.Text);
			_dataManagerService.SaveServerIp(serverIpInput.Text);

			_statusSenderService = new StatusSenderService(_extractionService, _dataManagerService);


			var result = await _statusSenderService.CheckStatus();

			DisplayServerConnectionStateLoading(false);
			DisplayServerConnectionState(result);

			if (result)
			{
				DisableServerConnectionInput();

				EnableBatteryInputs();

				mainTimer.Start();

				DisplayOkStatus("Connected");
			}
			else
			{
				DisplayErrorStatus("Could not reach server...");
			}
		}

		private void modeSelectorCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			File.WriteAllText("misc.txt", modeSelectorCB.SelectedIndex.ToString());
			currentChargingMode = (ChargingModes)modeSelectorCB.SelectedIndex;

			SelectMode(modeSelectorCB.SelectedIndex);
		}

		private void SelectMode(int mode)
		{

			switch (modeSelectorCB.SelectedIndex)
			{
				//best battery life
				case 0:
					EnableBatteryInputs();
					guna2HtmlToolTip3.SetToolTip(ModeInfoToolTip, "Note: In this mode the battery will continuously be charged to the specified % and when this level is reached the charging will be stopped until the battery % drops to the specified minimum level. (this is repeated)");
					break;
				//charge to a given %
				case 1:
					modeSelectorCB.Enabled = true;
					batteryMaxSlider.Enabled = true;
					batteryMinSlider.Enabled = false;
					guna2HtmlToolTip3.SetToolTip(ModeInfoToolTip, "Note: The battery will be charged to a constant level.");
					break;
				//stop charging
				case 2:
					modeSelectorCB.Enabled = true;
					batteryMinSlider.Enabled = false;
					batteryMaxSlider.Enabled = false;
					guna2HtmlToolTip3.SetToolTip(ModeInfoToolTip, "Note: The smart charging is stopped");
					break;
			}
		}

		private void DisplayChargingState(SmartChargingStates status)
		{
			NotChargingImg.Visible = false;
			DischargingImg.Visible = false;
			ErrorChargingImg.Visible = false;
			ChargingImg.Visible = false;

			switch (status)
			{
				case SmartChargingStates.Activated:
					ChargingImg.Visible = true;
					break;
				case SmartChargingStates.Deactivated:
					NotChargingImg.Visible = true;
					break;
				case SmartChargingStates.FailedToStartCharging:
					ErrorChargingImg.Visible = true;
					break;
				case SmartChargingStates.WaitingToDischarge:
					DischargingImg.Visible = true;
					break;
			}
		}
	}
}
