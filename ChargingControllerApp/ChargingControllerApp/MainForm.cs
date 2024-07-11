using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ChargingControllerApp
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			HideApp();

			FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
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
	}
}
