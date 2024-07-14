using ChargingControllerApp.Services;
using ChargingControllerApp.Services.Contracts;
using ChargingControllerApp.Utils;
using Guna.UI2.WinForms;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ChargingControllerApp
{
	public partial class MainForm : Form
	{
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;

		private readonly IDataExtractionService _extractionService = new DataExtractionService();
		private readonly IDataManagerService _dataManagerService = new DataManagerService();

		private IStatusSenderService? _statusSenderService;

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

			if (File.Exists("serverInfo.txt") && File.Exists("token.txt"))
			{
				DisplayErrorStatus("Trying to connect....");

				DisableServerConnectionInput();

				EnableBatteryInputs();

				mainTimer.Start();

				_statusSenderService = new StatusSenderService(_extractionService, _dataManagerService);
			}
			textOverflowTimer.Start();
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
			DialogResult dialog = MessageBox.Show("If you exit the app the charging will stop! Do you still want to exit?", "Alert!", MessageBoxButtons.YesNo);

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
			ShowInTaskbar = true;
			Visible = true;
		}

		private void DisplayServerConnection(bool isConnected)
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

		private void DisplayServerConnectionPercentage(int percentage)
		{
			if (percentage >= 100)
			{
				serverConnectionLoading.Visible = false;
				return;
			}

			if (percentage < 0)
			{
				percentage = 0;
			}

			serverConnectionLoading.Visible = true;

			serverConnectionLoading.Value = percentage;
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
		}

		private async void MainTimer_Tick(object sender, EventArgs e)
		{
			if (_statusSenderService == null)
			{
				DisplayServerConnectionLost("Failed to send request to server");
				return;
			}

			try
			{
				var response = await _statusSenderService.SendLaptopData();

                if (response==null)
                {
					DisplayErrorStatus("Could not parse response!");
					return;
				}

                if (response.OverrideActive)
                {
					DisplayErrorStatus("Override activated. Controls disabled!");
					DisableBatteryInputs();
				}

				switch (response.SmartChargingStatus)
				{
					case Enums.SmartChargingStates.Activated:
						NotChargingImg.Visible = false;
						ChargingImg.Visible = true;
						break;
					case Enums.SmartChargingStates.Deactivated:
						NotChargingImg.Visible = true;
						ChargingImg.Visible= false;
						break;
					case Enums.SmartChargingStates.WaitingToDischarge:
						break;
				}

				DisplayOkStatus(response.ResponseMessage);
            }
			catch (Exception)
			{
				DisplayServerConnectionLost("The request was not successful");
			}

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
			connectToServerBn.Enabled = true;

			connectToServerBn.Text = "Connect";
		}

		private void DisableBatteryInputs()
		{
			modeSelectorCB.Enabled=false;
			batteryMaxSlider.Enabled=false;
			batteryMinSlider.Enabled=false;	
		}

		private void EnableBatteryInputs()
		{
			modeSelectorCB.Enabled = true;
			batteryMaxSlider.Enabled = true;
			batteryMinSlider.Enabled = true;
		}

		private void DisplayServerConnectionLost(string message)
		{
			DisplayServerConnection(false);
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
			DisplayServerConnectionPercentage(30);
			_dataManagerService.SaveEncryptedToken(serverTokentTB.Text);	
			_dataManagerService.SaveServerIp(serverIpInput.Text);

			DisplayServerConnectionPercentage(60);
			_statusSenderService =new StatusSenderService(_extractionService, _dataManagerService);


            var result=await _statusSenderService.CheckStatus();

			DisplayServerConnectionPercentage(100);
			DisplayServerConnection(result);

            if (result)
            {
				DisableServerConnectionInput();
			}
        }
	}
}
