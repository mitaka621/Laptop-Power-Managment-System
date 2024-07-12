using ChargingControllerApp.Services;
using ChargingControllerApp.Services.Contracts;
using Guna.UI2.WinForms;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ChargingControllerApp
{
	public partial class MainForm : Form
	{
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;

		private readonly IDataExtractionService _extractionService = new DataExtractionService();

		public MainForm()
		{
			InitializeComponent();

			StartPosition = FormStartPosition.Manual;

			if (Screen.PrimaryScreen != null)
			{
				Rectangle workingArea = Screen.PrimaryScreen.Bounds;
				int x = workingArea.Right - Width;
				int y = workingArea.Bottom - Height;
				Location = new Point(x, y);
			}

			MinimizeBox = false;
			HideApp();

			FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

			MainTimer.Start();
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
			if (WindowState == FormWindowState.Minimized)
			{
				ShowApp();
			}
			else
			{
				HideApp();
			}

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

			serverConnectionLoading.Value = percentage;
		}

		private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

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

		private void MainTimer_Tick(object sender, EventArgs e)
		{
			int fullChargeCapacity = _extractionService.GetDesignedBatteryCapacity();
			int designCapacity = _extractionService.GetFullChargeCapacity();
			double wearPercentage = 100 - ((double)fullChargeCapacity / designCapacity * 100);

			initialCapacityLabel.Text = $"{fullChargeCapacity}mWh";
			currentCapacityLabel.Text=$"{designCapacity}mWh";		
			batteryHealthLabel.Text = $"{wearPercentage}%";

			PowerStatus powerStatus = SystemInformation.PowerStatus;

			chargeLevelLabel.Text= $"{powerStatus.BatteryLifePercent*100}%";
			chargeStatusLabel.Text = $"{powerStatus.PowerLineStatus}";
			timeRemainingLabel.Text = $"{powerStatus.BatteryFullLifetime/60}";
		}
	}
}
