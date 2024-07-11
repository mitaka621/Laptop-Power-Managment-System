namespace ChargingControllerApp
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			notifyIcon = new NotifyIcon(components);
			contextMenuStrip1 = new ContextMenuStrip(components);
			toolStripMenuItem1 = new ToolStripMenuItem();
			exitToolStripMenuItem = new ToolStripMenuItem();
			IpTB = new MaskedTextBox();
			label1 = new Label();
			SetIpBt = new Button();
			contextMenuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// notifyIcon
			// 
			notifyIcon.ContextMenuStrip = contextMenuStrip1;
			notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
			notifyIcon.Text = "notifyIcon";
			notifyIcon.Visible = true;
			notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1 });
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new Size(117, 26);
			// 
			// toolStripMenuItem1
			// 
			toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
			toolStripMenuItem1.Name = "toolStripMenuItem1";
			toolStripMenuItem1.Size = new Size(116, 22);
			toolStripMenuItem1.Text = "Options";
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new Size(93, 22);
			exitToolStripMenuItem.Text = "Exit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// IpTB
			// 
			IpTB.Location = new Point(76, 37);
			IpTB.Mask = "###.###.###.###";
			IpTB.Name = "IpTB";
			IpTB.Size = new Size(100, 23);
			IpTB.TabIndex = 1;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(76, 19);
			label1.Name = "label1";
			label1.Size = new Size(83, 15);
			label1.TabIndex = 2;
			label1.Text = "Server local IP:";
			// 
			// SetIpBt
			// 
			SetIpBt.Location = new Point(182, 36);
			SetIpBt.Name = "SetIpBt";
			SetIpBt.Size = new Size(75, 23);
			SetIpBt.TabIndex = 3;
			SetIpBt.Text = "Set";
			SetIpBt.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(367, 450);
			Controls.Add(SetIpBt);
			Controls.Add(label1);
			Controls.Add(IpTB);
			Name = "MainForm";
			Text = "Form1";
			contextMenuStrip1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private NotifyIcon notifyIcon;
		private ContextMenuStrip contextMenuStrip1;
		private ToolStripMenuItem toolStripMenuItem1;
		private ToolStripMenuItem exitToolStripMenuItem;
		private MaskedTextBox IpTB;
		private Label label1;
		private Button SetIpBt;
	}
}
