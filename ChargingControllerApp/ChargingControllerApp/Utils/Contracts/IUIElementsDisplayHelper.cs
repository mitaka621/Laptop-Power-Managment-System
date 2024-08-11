using ChargingControllerApp.Enums;
using Guna.UI2.WinForms;

namespace ChargingControllerApp.Utils.Contracts
{

	public interface IUIElementsDisplayHelper
	{
		Form MainForm { get; }

		PictureBox ServerConnectedImg { get; }
		PictureBox ServerDisconnectedImg { get; }
		Guna2CircleProgressBar ServerConnectionLoading { get; }
		Guna2TextBox ServerIpInput { get; }
		Guna2TextBox ServerTokentTB { get; }
		Guna2Button ConnectToServerBn { get; }

		Guna2ComboBox ModeSelectorCB { get; }
		Guna2TrackBar BatteryMaxSlider { get; }
		Guna2TrackBar BatteryMinSlider { get; }

		Guna2TabControl Guna2TabControl1 { get; }
		Guna2TextBox StatusMessageTB { get; }
		Guna2HtmlToolTip Guna2HtmlToolTip3 { get; }
		Control ModeInfoToolTip { get; }
		Guna2HtmlLabel MessageLabel { get; }

		System.Windows.Forms.Timer MainTimer { get; }

		Guna2PictureBox NotChargingImg { get; }
		Guna2PictureBox DischargingImg { get; }
		Guna2PictureBox ErrorChargingImg { get; }
		PictureBox ChargingImg { get; }

		void HideApp();
		void ShowApp();
		void DisplayServerConnectionState(bool isConnected);
		void DisplayServerConnectionStateLoading(bool show);
		void DisableServerConnectionInput();
		void EnableServerConnectionInput();
		void DisableBatteryInputs();
		void EnableBatteryInputs();
		void DisplayServerConnectionStateLost(string message);
		void DisplayErrorStatus(string message);
		void DisplayOkStatus(string message);
		void SelectMode(ChargingModes mode);
		void DisplayChargingState(SmartChargingStates status);
		string GetMessage();
		void WriteMessage(string message);
	}

}
