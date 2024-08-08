using ChargingControllerApp.Enums;
using ChargingControllerApp.Utils.Contracts;
using Guna.UI2.WinForms;
using System.Media;

namespace ChargingControllerApp.Utils
{
	public class UIElementsDisplayHelper : IUIElementsDisplayHelper
	{
		private SmartChargingStates lastChargingState= SmartChargingStates.Deactivated;

        public Form MainForm { get; private set; }
		public PictureBox ServerConnectedImg { get; private set; }
		public PictureBox ServerDisconnectedImg { get; }
		public Guna2CircleProgressBar ServerConnectionLoading { get; private set; }
		public Guna2TextBox ServerIpInput { get; }
		public Guna2TextBox ServerTokentTB { get; private set; }
		public Guna2Button ConnectToServerBn { get; private set; }

		public Guna2ComboBox ModeSelectorCB { get; private set; }
		public Guna2TrackBar BatteryMaxSlider { get; private set; }
		public Guna2TrackBar BatteryMinSlider { get; private set; }

		public Guna2TabControl Guna2TabControl1 { get; private set; }
		public Guna2TextBox StatusMessageTB { get; private set; }
		public Guna2HtmlToolTip Guna2HtmlToolTip3 { get; private set; }
		public Control ModeInfoToolTip { get; private set; }

		public System.Windows.Forms.Timer MainTimer { get; private set; }

		public Guna2PictureBox NotChargingImg { get; private set; }
		public Guna2PictureBox DischargingImg { get; private set; }
		public Guna2PictureBox ErrorChargingImg { get; private set; }
		public PictureBox ChargingImg { get; private set; }

		public UIElementsDisplayHelper(
			Form mainForm,
			PictureBox serverConnectedImg,
			PictureBox serverDisconnectedImg,
			Guna2CircleProgressBar serverConnectionLoading,
			Guna2TextBox serverIpInput,
			Guna2TextBox serverTokentTB,
			Guna2Button connectToServerBn,
			Guna2ComboBox modeSelectorCB,
			Guna2TrackBar batteryMaxSlider,
			Guna2TrackBar batteryMinSlider,
			Guna2TabControl guna2TabControl1,
			Guna2TextBox statusMessageTB,
			Guna2HtmlToolTip guna2HtmlToolTip3,
			Control modeInfoToolTip,
			System.Windows.Forms.Timer mainTimer,
			Guna2PictureBox notChargingImg,
			Guna2PictureBox dischargingImg,
			Guna2PictureBox errorChargingImg,
			PictureBox chargingImg
		)
		{
			MainForm = mainForm;
			MainForm.WindowState = FormWindowState.Minimized;
			MainForm.ShowInTaskbar = false;
			MainForm.Visible = false;

			ServerConnectedImg = serverConnectedImg;
			ServerDisconnectedImg = serverDisconnectedImg;
			ServerConnectionLoading = serverConnectionLoading;
			ServerIpInput = serverIpInput;
			ServerTokentTB = serverTokentTB;
			ConnectToServerBn = connectToServerBn;
			ModeSelectorCB = modeSelectorCB;
			BatteryMaxSlider = batteryMaxSlider;
			BatteryMinSlider = batteryMinSlider;
			Guna2TabControl1 = guna2TabControl1;
			StatusMessageTB = statusMessageTB;
			Guna2HtmlToolTip3 = guna2HtmlToolTip3;
			ModeInfoToolTip = modeInfoToolTip;
			MainTimer = mainTimer;
			NotChargingImg = notChargingImg;
			DischargingImg = dischargingImg;
			ErrorChargingImg = errorChargingImg;
			ChargingImg = chargingImg;
		}

		public void HideApp()
		{
			MainForm.WindowState = FormWindowState.Minimized;
			MainForm.ShowInTaskbar = false;
			MainForm.Visible = false;
		}

		public void ShowApp()
		{
			MainForm.WindowState = FormWindowState.Normal;
			MainForm.ShowInTaskbar = false;
			MainForm.Visible = true;
		}

		public void DisplayServerConnectionState(bool isConnected)
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
				DisableServerConnectionInput();
            }
		}

		public void DisplayServerConnectionStateLoading(bool show)
		{
			ServerConnectionLoading.Visible = show;
		}

		public void DisableServerConnectionInput()
		{
			ServerIpInput.Enabled = false;
			ServerTokentTB.Enabled = false;


			ConnectToServerBn.Text = "Already Connected";
			ConnectToServerBn.Enabled = false;
		}

		public void EnableServerConnectionInput()
		{
			ServerIpInput.Enabled = true;
			ServerTokentTB.Enabled = true;

			ConnectToServerBn.Text = "Connect";
			ConnectToServerBn.Enabled = true;
		}

		public void DisableBatteryInputs()
		{
			ModeSelectorCB.Enabled = false;
			BatteryMaxSlider.Enabled = false;
			BatteryMinSlider.Enabled = false;
		}

		public void EnableBatteryInputs()
		{
			ModeSelectorCB.Enabled = true;
			BatteryMaxSlider.Enabled = true;
			BatteryMinSlider.Enabled = true;
		}

		public void DisplayServerConnectionStateLost(string message)
		{
			DisplayServerConnectionState(false);
			ShowApp();
			Guna2TabControl1.SelectTab(1);
			EnableServerConnectionInput();
			DisplayErrorStatus(message);
			DisableBatteryInputs();

			MainTimer.Stop();
		}

		public void DisplayErrorStatus(string message)
		{
			StatusMessageTB.Text = message;
			StatusMessageTB.DisabledState.FillColor = Color.FromArgb(255, 189, 191);
		}

		public void DisplayOkStatus(string message)
		{
			StatusMessageTB.Text = message;
			StatusMessageTB.DisabledState.FillColor = Color.FromArgb(218, 254, 225);
		}

		public void SelectMode(ChargingModes mode)
		{
			ModeSelectorCB.SelectedIndex = (int)mode;
			switch (mode)
			{
				case ChargingModes.BestBatteryLifeCharging:
					EnableBatteryInputs();
					Guna2HtmlToolTip3.SetToolTip(ModeInfoToolTip, "Note: In this mode the battery will continuously be charged to the specified % and when this level is reached the charging will be stopped until the battery % drops to the specified minimum level. (this is repeated)");
					break;

				case ChargingModes.FixedLevelCharging:
					ModeSelectorCB.Enabled = true;
					BatteryMaxSlider.Enabled = true;
					BatteryMinSlider.Enabled = false;
					Guna2HtmlToolTip3.SetToolTip(ModeInfoToolTip, "Note: The battery will be charged to a constant level.");
					break;

				case ChargingModes.Off:
					ModeSelectorCB.Enabled = true;
					BatteryMinSlider.Enabled = false;
					BatteryMaxSlider.Enabled = false;
					Guna2HtmlToolTip3.SetToolTip(ModeInfoToolTip, "Note: The smart charging is stopped");
					break;
			}
		}

		public void DisplayChargingState(SmartChargingStates status)
		{
            if (status==lastChargingState)
            {
				return;
            }

            NotChargingImg.Visible = false;
			DischargingImg.Visible = false;
			ErrorChargingImg.Visible = false;
			ChargingImg.Visible = false;

			switch (status)
			{
				case SmartChargingStates.Activated:
					ChargingImg.Visible = true;
                    SoundPlayer soundConnected = new SoundPlayer(Path.GetFullPath(@"Resourses\message-incoming-2-199577.wav"));
                    soundConnected.Play();

                    break;
				case SmartChargingStates.Deactivated:
					NotChargingImg.Visible = true;
					break;
				case SmartChargingStates.FailedToStartCharging:
					ErrorChargingImg.Visible = true;
					SoundPlayer soundDisconnect= new SoundPlayer(Path.GetFullPath(@"Resourses\bottle-205353.wav"));
					soundDisconnect.Play();
					
					break;
				case SmartChargingStates.WaitingToDischarge:
					DischargingImg.Visible = true;
					break;
			}

            lastChargingState= status;
        }
	}
}
