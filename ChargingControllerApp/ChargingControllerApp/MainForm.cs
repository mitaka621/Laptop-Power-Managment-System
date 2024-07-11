using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ChargingControllerApp
{
	public partial class MainForm : Form
	{
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;

		public MainForm()
		{
			InitializeComponent();

			StartPosition = FormStartPosition.Manual;

            if (Screen.PrimaryScreen!=null)
            {
				Rectangle workingArea = Screen.PrimaryScreen.Bounds;
				int x = workingArea.Right - Width;
				int y = workingArea.Bottom - Height;
				Location = new Point(x, y);
			}

			MaximizeBox = false;
			MinimizeBox = false;
			HideApp();

			FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
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
