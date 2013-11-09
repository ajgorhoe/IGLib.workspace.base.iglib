using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace IG.Forms
{
	/// <summary>
	/// Summary description for FaidMessage1.
	/// </summary>
	public class FaidMessage1 : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button CancelBtn;
        private Panel TitlePanel;
        private Panel MessagePanel;
        private Panel ControlPanel;
        private Label label1;
        private Label label2;
        private Label StatusLbl;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public FaidMessage1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.CancelBtn = new System.Windows.Forms.Button();
            this.TitlePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.MessagePanel = new System.Windows.Forms.Panel();
            this.ControlPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.StatusLbl = new System.Windows.Forms.Label();
            this.TitlePanel.SuspendLayout();
            this.MessagePanel.SuspendLayout();
            this.ControlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            this.CancelBtn.AutoSize = true;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CancelBtn.Location = new System.Drawing.Point(372, 18);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(5);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(50, 24);
            this.CancelBtn.TabIndex = 2;
            this.CancelBtn.Text = "Cancel";
            // 
            // TitlePanel
            // 
            this.TitlePanel.AutoSize = true;
            this.TitlePanel.Controls.Add(this.label1);
            this.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitlePanel.ForeColor = System.Drawing.Color.Blue;
            this.TitlePanel.Location = new System.Drawing.Point(4, 4);
            this.TitlePanel.Margin = new System.Windows.Forms.Padding(10);
            this.TitlePanel.Name = "TitlePanel";
            this.TitlePanel.Padding = new System.Windows.Forms.Padding(5);
            this.TitlePanel.Size = new System.Drawing.Size(438, 48);
            this.TitlePanel.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(5);
            this.label1.Size = new System.Drawing.Size(175, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fading Message";
            // 
            // MessagePanel
            // 
            this.MessagePanel.AutoSize = true;
            this.MessagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MessagePanel.Controls.Add(this.label2);
            this.MessagePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MessagePanel.Location = new System.Drawing.Point(4, 52);
            this.MessagePanel.Margin = new System.Windows.Forms.Padding(5);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.Padding = new System.Windows.Forms.Padding(5);
            this.MessagePanel.Size = new System.Drawing.Size(438, 33);
            this.MessagePanel.TabIndex = 3;
            // 
            // ControlPanel
            // 
            this.ControlPanel.AutoSize = true;
            this.ControlPanel.Controls.Add(this.StatusLbl);
            this.ControlPanel.Controls.Add(this.CancelBtn);
            this.ControlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ControlPanel.Location = new System.Drawing.Point(4, 85);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Size = new System.Drawing.Size(438, 47);
            this.ControlPanel.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(5, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3);
            this.label2.Size = new System.Drawing.Size(52, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "label2";
            // 
            // StatusLbl
            // 
            this.StatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusLbl.AutoSize = true;
            this.StatusLbl.Location = new System.Drawing.Point(5, 29);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(35, 13);
            this.StatusLbl.TabIndex = 3;
            this.StatusLbl.Text = "label3";
            // 
            // FaidMessage1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(446, 142);
            this.Controls.Add(this.ControlPanel);
            this.Controls.Add(this.MessagePanel);
            this.Controls.Add(this.TitlePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FaidMessage1";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FaidMessage1";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FaidMessage1_Load);
            this.TitlePanel.ResumeLayout(false);
            this.TitlePanel.PerformLayout();
            this.MessagePanel.ResumeLayout(false);
            this.MessagePanel.PerformLayout();
            this.ControlPanel.ResumeLayout(false);
            this.ControlPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void FaidMessage1_Load(object sender, System.EventArgs e)
		{
		
		}

      
	}
}
