using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;


namespace IG.Forms
{
	/// <summary>
	/// Summary description for ConsoleForm.
	/// </summary>
	public class ConsoleForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button CancelBtn;
        private Panel TitlePanel;
        private Panel MessagePanel;
        private Panel ControlPanel;
        private Label TitleLbl;
        private Label MessageLbl;
        // private IContainer components;
        private RichTextBox OutputRichText;
        private Panel OutputPanel;
        private Label StatusLbl;
		/// <summary>
		/// Required designer variable.
		/// </summary>


        private void baseFadeMessage()
        {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
        }

        public ConsoleForm()
		{
			baseFadeMessage();
		}


        public ConsoleForm(string msgtext)
        /// Shows a fading message in a new thread, with message text equal to mshtext and without a title;
        {  baseFadeMessage(); ShowThread(msgtext);  }

        public ConsoleForm(string msgtext, int showtime)
        /// Shows a fading message in a new thread, with message text equal to  msgtext and with specified showing time in ms.
        { baseFadeMessage(); ShowThread(msgtext, showtime); }

        public ConsoleForm(string msgtext, int showtime, double fadeportion)
        /// Shows a fading message in a new thread, with message text equal to  msgtext and with specified showing time in ms and fading time portion.
        { baseFadeMessage(); ShowThread(msgtext, showtime, fadeportion); }


        public ConsoleForm(string title, string msgtext)
        /// Shows the form in a new thread, with a title and with message text equal to msgtext.
        { baseFadeMessage(); ShowThread(title, msgtext); }

        public ConsoleForm(string title, string msgtext, int showtime)
        /// Shows a fading message in a new thread, with a title and with message text equal to  msgtext, with specified showing time in ms.
        { ShowTime = showtime; ShowThread(title, msgtext, showtime); }

        public ConsoleForm(string title, string msgtext, int showtime, double fadeportion)
        /// Shows a fading message in a new thread, with a title and with message text equal to  msgtext, with specified showing time in ms and fading time portion.
        { baseFadeMessage(); ShowThread(title, msgtext, showtime,fadeportion); }



		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.CancelBtn = new System.Windows.Forms.Button();
            this.TitlePanel = new System.Windows.Forms.Panel();
            this.TitleLbl = new System.Windows.Forms.Label();
            this.MessagePanel = new System.Windows.Forms.Panel();
            this.MessageLbl = new System.Windows.Forms.Label();
            this.ControlPanel = new System.Windows.Forms.Panel();
            this.StatusLbl = new System.Windows.Forms.Label();
            this.OutputRichText = new System.Windows.Forms.RichTextBox();
            this.OutputPanel = new System.Windows.Forms.Panel();
            this.TitlePanel.SuspendLayout();
            this.MessagePanel.SuspendLayout();
            this.ControlPanel.SuspendLayout();
            this.OutputPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.AutoSize = true;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CancelBtn.Location = new System.Drawing.Point(619, 1);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(5);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(50, 24);
            this.CancelBtn.TabIndex = 2;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // TitlePanel
            // 
            this.TitlePanel.AutoSize = true;
            this.TitlePanel.Controls.Add(this.TitleLbl);
            this.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitlePanel.ForeColor = System.Drawing.Color.Blue;
            this.TitlePanel.Location = new System.Drawing.Point(4, 4);
            this.TitlePanel.Margin = new System.Windows.Forms.Padding(0);
            this.TitlePanel.Name = "TitlePanel";
            this.TitlePanel.Size = new System.Drawing.Size(676, 38);
            this.TitlePanel.TabIndex = 3;
            // 
            // TitleLbl
            // 
            this.TitleLbl.AutoSize = true;
            this.TitleLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TitleLbl.Location = new System.Drawing.Point(3, 4);
            this.TitleLbl.Name = "TitleLbl";
            this.TitleLbl.Padding = new System.Windows.Forms.Padding(5);
            this.TitleLbl.Size = new System.Drawing.Size(152, 34);
            this.TitleLbl.TabIndex = 0;
            this.TitleLbl.Text = "Console Form";
            // 
            // MessagePanel
            // 
            this.MessagePanel.AutoSize = true;
            this.MessagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MessagePanel.Controls.Add(this.MessageLbl);
            this.MessagePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.MessagePanel.Location = new System.Drawing.Point(4, 42);
            this.MessagePanel.Margin = new System.Windows.Forms.Padding(5);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.Padding = new System.Windows.Forms.Padding(5);
            this.MessagePanel.Size = new System.Drawing.Size(676, 33);
            this.MessagePanel.TabIndex = 3;
            // 
            // MessageLbl
            // 
            this.MessageLbl.AutoSize = true;
            this.MessageLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MessageLbl.Location = new System.Drawing.Point(5, 5);
            this.MessageLbl.Margin = new System.Windows.Forms.Padding(0);
            this.MessageLbl.Name = "MessageLbl";
            this.MessageLbl.Padding = new System.Windows.Forms.Padding(3);
            this.MessageLbl.Size = new System.Drawing.Size(52, 23);
            this.MessageLbl.TabIndex = 0;
            this.MessageLbl.Text = "label2";
            // 
            // ControlPanel
            // 
            this.ControlPanel.AutoSize = true;
            this.ControlPanel.Controls.Add(this.StatusLbl);
            this.ControlPanel.Controls.Add(this.CancelBtn);
            this.ControlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ControlPanel.Location = new System.Drawing.Point(4, 675);
            this.ControlPanel.Name = "ControlPanel";
            this.ControlPanel.Size = new System.Drawing.Size(676, 30);
            this.ControlPanel.TabIndex = 3;
            // 
            // StatusLbl
            // 
            this.StatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusLbl.AutoSize = true;
            this.StatusLbl.Location = new System.Drawing.Point(5, 12);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(35, 13);
            this.StatusLbl.TabIndex = 3;
            this.StatusLbl.Text = "label3";
            // 
            // OutputRichText
            // 
            this.OutputRichText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputRichText.Location = new System.Drawing.Point(0, 0);
            this.OutputRichText.Name = "OutputRichText";
            this.OutputRichText.Size = new System.Drawing.Size(676, 600);
            this.OutputRichText.TabIndex = 4;
            this.OutputRichText.Text = "";
            // 
            // OutputPanel
            // 
            this.OutputPanel.Controls.Add(this.OutputRichText);
            this.OutputPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputPanel.Location = new System.Drawing.Point(4, 75);
            this.OutputPanel.Name = "OutputPanel";
            this.OutputPanel.Size = new System.Drawing.Size(676, 600);
            this.OutputPanel.TabIndex = 5;
            // 
            // ConsoleForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(684, 709);
            this.Controls.Add(this.OutputPanel);
            this.Controls.Add(this.ControlPanel);
            this.Controls.Add(this.MessagePanel);
            this.Controls.Add(this.TitlePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "ConsoleForm";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Fademessage";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ConsoleForm_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FadeMessage_MouseClick);
            this.TitlePanel.ResumeLayout(false);
            this.TitlePanel.PerformLayout();
            this.MessagePanel.ResumeLayout(false);
            this.MessagePanel.PerformLayout();
            this.ControlPanel.ResumeLayout(false);
            this.ControlPanel.PerformLayout();
            this.OutputPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion



        // Default properties:
        private static int defaultShowtime = 3000;
        private static double defaultFadingTimePortion = 0.3;

        // Properties:
        private int vShowTime = defaultShowtime;
        private double vFadingTimePortion = defaultFadingTimePortion;

        public int ShowTime
        {
            set { if (value < 500) value = 500; vShowTime = value; }
            get { return vShowTime; }
        }
        public double FadingTimePortion
        {
            set { if (value < 0) value = 0; if (value > 1) value = 1; vFadingTimePortion = value; }
            get { return vFadingTimePortion; }
        }

        public string MsgTitle = null, MsgText = null;







        //string text;
        //int showtime;
        //double fadingportion;

        
        
        private Thread formthread = null, manipulationthread = null;
        


        private void ManipulationThreadFunc()
        {
            try
            {
                formthread = new Thread(new ThreadStart(FormThreadFunc));
                formthread.Start();
                Thread.Sleep(ShowTime);
                formthread.Abort();

            }
            catch { }
        }

        public Color BackGroundColor = Color.LightYellow,
            FadeColor = Color.Blue;



        [STAThread]
        private void FormThreadFunc()
        {

            try
            {
                this.TitleLbl.Text = MsgTitle;
                this.MessageLbl.Text = MsgText;
                this.StatusLbl.Text = "";
                IGForm.SetBackColorRec(this,BackGroundColor);
                if ( 0 != 1 )
                {
                    this.StatusLbl.Text = "<< Output Console >>";
                }
                this.ShowDialog( );
            }
            catch (Exception) { }
        }




        private void ConsoleForm_Load(object sender, System.EventArgs e)
		{
		
		}



        private void CancelBtn_Click(object sender, EventArgs e)
        {
            CloseDialog();
        }



        // Public methods:

        public void HideDialog()
        {
            this.Visible = false;
        }


        public void CloseDialog()
        {
            try
            {
                HideDialog();
                Dispose();
            }
            catch { }
        }


        public void ShowThread()
        /// Shows a fading message in a new thread.
        {
            /// this.ShowDialog();
            // this.Visible = true;

            manipulationthread = new Thread(new ThreadStart(ManipulationThreadFunc));
            manipulationthread.Start();
        }

        public void ShowThread(string text)
        /// Shows a fading message in a new thread, with message text equal to mshtext and without a title;
        {
            if (text != null) if (text.Length > 0) MsgText = text;
            MsgTitle = null;
            ShowThread();
        }

        public void ShowThread(string msgtext, int showtime)
        /// Shows a fading message in a new thread, with message text equal to  msgtext and with specified showing time in ms.
        { ShowTime = showtime; ShowThread(msgtext); }

        public void ShowThread(string msgtext, int showtime, double fadeportion)
        /// Shows a fading message in a new thread, with message text equal to  msgtext and with specified showing time in ms and fading time portion.
        { ShowTime = showtime; FadingTimePortion = fadeportion; ShowThread(msgtext); }


        public void ShowThread(string title, string text)
        /// Shows the form in a new thread, with a title and with message text equal to msgtext.
        {
            if (title != null) if (title.Length>0) MsgTitle = title;
            if (text != null) if (text.Length > 0) MsgText = text;
            ShowThread();
        }

        public void ShowThread(string title, string msgtext, int showtime)
        /// Shows a fading message in a new thread, with a title and with message text equal to  msgtext, with specified showing time in ms.
        { ShowTime = showtime; ShowThread(msgtext); }

        public void ShowThread(string title, string msgtext, int showtime, double fadeportion)
        /// Shows a fading message in a new thread, with a title and with message text equal to  msgtext, with specified showing time in ms and fading time portion.
        { ShowTime = showtime; FadingTimePortion = fadeportion; ShowThread(msgtext); }


        private void FadeMessage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CloseDialog();
            }
        }



        
   
      
	}
}
