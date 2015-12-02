// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;


namespace IG.Forms
{

        /// <summary>
        /// Manages a fading message window. 
        /// Windows containing a message are launched in separate threads, closing after a specified time.
        /// </summary>
        /// $A Igor jul08;
	public class FadingMessage : System.Windows.Forms.Form
    {

        /// <summary>Color of the title (if shown in fading message).</summary>
        public Color ForeGroundColorTitle = Color.Blue;

        /// <summary>Color of the message text shown in the fading message.</summary>
        public Color ForeGroundColorMsg = Color.Black;

        /// <summary>Active background color of the fading message.</summary>
        public Color BackGroundColor = Color.LightYellow;

        /// <summary>Final (faded) color of the fading message.</summary>
        public Color FadeColor = Color.DarkGray;
        
        public string MsgTitle = null, MsgText = null;
        
        public bool IsReady=false; /// Returns true if control has been created and is ready to work with.
        protected bool IsClosable = true;  /// If false then the consform can not be closed or dispoded.
        public static int MaxShown = 50; /// Indicates how many messages can be shown simultaneously.
        private static int NumShown = 0; // Number of currently shown messages
        public static bool BlockMaxShown = false;  /// Indicates to block launching new messges when too many are processed.
        public bool BlockMaxShownCurrent = false; /// Instructs just for the current index to block launching the messge when too many are processed.
        private bool Counted = false;  // true indicates that Numshown has been incremented for this instance
        // but not yet decremented 

        #region Controls_Definitions
        private System.Windows.Forms.Timer timer1;
        private Button btnCancel;
        private Label lblStatus;
        private Panel BodyPnl;
        private Label lblMEssage;
        private Label lblTitle;
        private Panel PnlStatus;
        private Label labelBottom;
        private IContainer components;
		/// <summary>
		/// Required designer variable.
        /// </summary>
        #endregion  // Controls_Definitions

        private void baseFadeMessage()
        {
			InitializeComponent();
        }


        public FadingMessage()
        /// Argument-less constructor, does not launch the window in a parallel thread.
		{
			baseFadeMessage();
		}


        public FadingMessage(string msgtext)
        /// Shows a fading message in a new thread, with message text equal to mshtext and without a title;
        {  baseFadeMessage(); ShowThread(msgtext);  }

        public FadingMessage(string msgtext, int showtime)
        /// Shows a fading message in a new thread, with message text equal to  msgtext and with specified showing time in ms.
        { baseFadeMessage(); ShowThread(msgtext, showtime); }

        public FadingMessage(string msgtext, int showtime, double fadeportion)
        /// Shows a fading message in a new thread, with message text equal to  msgtext and with specified showing time in ms and fading time portion.
        { baseFadeMessage(); ShowThread(msgtext, showtime, fadeportion); }


        public FadingMessage(string title, string msgtext)
        /// Shows the consform in a new thread, with a title and with message text equal to msgtext.
        { baseFadeMessage(); ShowThread(title, msgtext); }

        public FadingMessage(string title, string msgtext, int showtime)
        /// Shows a fading message in a new thread, with a title and with message text equal to  msgtext, with specified showing time in ms.
        { baseFadeMessage(); ShowThread(title, msgtext, showtime); }

        public FadingMessage(string title, string msgtext, int showtime, double fadeportion)
        /// Shows a fading message in a new thread, with a title and with message text equal to  msgtext, with specified showing time in ms and fading time portion.
        { baseFadeMessage(); ShowThread(title, msgtext, showtime,fadeportion); }



		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FadingMessage));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.BodyPnl = new System.Windows.Forms.Panel();
            this.lblMEssage = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.PnlStatus = new System.Windows.Forms.Panel();
            this.labelBottom = new System.Windows.Forms.Label();
            this.BodyPnl.SuspendLayout();
            this.PnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCancel.Location = new System.Drawing.Point(308, 3);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(50, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(3, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(140, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "< Ctrl-right-click to close >";
            // 
            // BodyPnl
            // 
            this.BodyPnl.AutoSize = true;
            this.BodyPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BodyPnl.Controls.Add(this.lblMEssage);
            this.BodyPnl.Controls.Add(this.lblTitle);
            this.BodyPnl.Location = new System.Drawing.Point(8, 11);
            this.BodyPnl.Name = "BodyPnl";
            this.BodyPnl.Size = new System.Drawing.Size(349, 153);
            this.BodyPnl.TabIndex = 4;
            // 
            // lblMEssage
            // 
            this.lblMEssage.AutoSize = true;
            this.lblMEssage.Font = new System.Drawing.Font("Times New Roman", 10.5F);
            this.lblMEssage.Location = new System.Drawing.Point(0, 67);
            this.lblMEssage.Margin = new System.Windows.Forms.Padding(0);
            this.lblMEssage.MinimumSize = new System.Drawing.Size(100, 40);
            this.lblMEssage.Name = "lblMEssage";
            this.lblMEssage.Padding = new System.Windows.Forms.Padding(3);
            this.lblMEssage.Size = new System.Drawing.Size(349, 86);
            this.lblMEssage.TabIndex = 0;
            this.lblMEssage.Text = resources.GetString("lblMEssage.Text");
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Blue;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(5);
            this.lblTitle.Size = new System.Drawing.Size(272, 67);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Fading Message Title.\r\nCan be mutiline. Top position of label\r\nis adjusted to tit" +
    "le height.";
            // 
            // PnlStatus
            // 
            this.PnlStatus.AutoSize = true;
            this.PnlStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PnlStatus.Controls.Add(this.btnCancel);
            this.PnlStatus.Controls.Add(this.lblStatus);
            this.PnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlStatus.Location = new System.Drawing.Point(8, 197);
            this.PnlStatus.Name = "PnlStatus";
            this.PnlStatus.Size = new System.Drawing.Size(363, 30);
            this.PnlStatus.TabIndex = 5;
            this.PnlStatus.Paint += new System.Windows.Forms.PaintEventHandler(this.PnlStatus_Paint);
            // 
            // labelBottom
            // 
            this.labelBottom.AutoSize = true;
            this.labelBottom.Location = new System.Drawing.Point(10, 166);
            this.labelBottom.Name = "labelBottom";
            this.labelBottom.Size = new System.Drawing.Size(177, 13);
            this.labelBottom.TabIndex = 6;
            this.labelBottom.Text = "Label for forcing lower window edge";
            // 
            // FadingMessage
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(379, 235);
            this.ControlBox = false;
            this.Controls.Add(this.BodyPnl);
            this.Controls.Add(this.PnlStatus);
            this.Controls.Add(this.labelBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(40, 20);
            this.Name = "FadingMessage";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FadeMessage_FormClosing);
            this.Load += new System.EventHandler(this.FadeMessage_Load);
            this.Disposed += new System.EventHandler(this.FadeMessage_Disposed);
            this.BodyPnl.ResumeLayout(false);
            this.BodyPnl.PerformLayout();
            this.PnlStatus.ResumeLayout(false);
            this.PnlStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        
        // Event handlers FadeMessage_MouseMoveand ReportConfig_MouseDown enable dragging of messagebox
        // without a frame.
        int m_PrevX;
        int m_PrevY;
        private void FadeMessage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            Left = Left + (e.X - m_PrevX);
            Top = Top + (e.Y - m_PrevY);
        }

        private void FadeMessage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            m_PrevX = e.X;
            m_PrevY = e.Y;
        }


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



       
        
        private Thread formthread = null, manipulationthread = null;

        private void ManipulationThreadFunc()
        // A parallel thread that takes care of launching the thread that manages the consform.
        // This thread also takes care that the consform is disposed after the prescribed time elapses.
        {
            int WasShown=NumShown, SkipLimit=(int)((double) MaxShown*1.2);
            if (NumShown > SkipLimit)
            {
                // A limit is exceeded when we start considering skipping some messages:
                Thread.Sleep(200); // give some time to improve the situation
                if (NumShown >= WasShown && 
                            NumShown > (int) (double) SkipLimit*1.1)  // additional chance to recover
                {
                    // The number of open forms has not been decreased, therefore we skip showing the current message:
                    // Undo increment of the number of launched forms because this one is not launched:
                    if ( this.Counted ) { this.Counted=false; --NumShown; }
                    return;
                }
            }
            // Perform some waiting if the number of launching forms exceeds the maximum allowed number:
            int count=0, waitinterval = 200, maxwaittime = 2000;
            while (NumShown >= MaxShown && count * waitinterval <= maxwaittime)
                Thread.Sleep(waitinterval); // Take care that not too many forms are shown in parallel threads
            try
            {
                formthread = new Thread(new ThreadStart(FormThreadFunc));
                formthread.IsBackground = true;
                formthread.Start();
                Thread.Sleep(ShowTime);
                this.CloseForm();
                Thread.Sleep(100);
                // formthread.Abort();
                formthread.Join();
            }
            catch (Exception e) { Exception ee = e; }
        }





        [STAThread]
        private void FormThreadFunc()
        /// Launches the message.
        {
            try
            {
                // Set up timer parameters:
                firsttick = (int)((1.0 - FadingTimePortion) * (double)ShowTime);
                // Set the total number of ticks such that fading is smooth but there are not too much timer intervals:
                tickinterval = mintickinterval;
                totalticks = (int)(FadingTimePortion * (double)ShowTime / (double)tickinterval);
                if (totalticks > maxticks)
                {
                    totalticks = maxticks;
                    tickinterval = (int)(FadingTimePortion * (double)ShowTime / (double)totalticks);
                }
                timer1.Interval = firsttick;
                timer1.Enabled = true;
            }
            catch (Exception) {  }
            finally
            {
                try
                {
                    this.ShowDialog();
                }
                catch (Exception) { }
            }
        }



        // Timer controled fade/out:
        int mintickinterval = 40, maxticks = 50;  // parameters to enable smooth fading
        int numticks = 0, totalticks = 20, firsttick = 1000, tickinterval = 100;
        double fadingfactor = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                ++numticks;
                timer1.Interval = tickinterval; // change time interval after the first tick
                fadingfactor = (double)(numticks - 1) / ((double)totalticks);
                if (fadingfactor >= 1.0)
                    timer1.Enabled = false;
                // Report fading progress (uncomment this only for testing):
                // StatusLbl.Text = "Fading: " + numticks.ToString() + "/" + totalticks.ToString();
                SetFadeLevel(fadingfactor);
            }
            catch { }
        }


        private void SetFadeLevel(double fadefactor)
        {
            Color bkcolor;
            double R, G, B;
            if (fadefactor < 0.0)
                fadefactor = 0.0;
            R = (1.0 - fadefactor) * (double)BackGroundColor.R + fadefactor * FadeColor.R;
            G = (1.0 - fadefactor) * (double)BackGroundColor.G + fadefactor * FadeColor.G;
            B = (1.0 - fadefactor) * (double)BackGroundColor.B + fadefactor * FadeColor.B;
            if (R < 0) R = 0; if (R > 255) R = 255;
            if (G < 0) G = 0; if (G > 255) G = 255;
            if (B < 0) B = 0; if (B > 255) B = 255;
            bkcolor = Color.FromArgb((int)R, (int)G, (int)B);
            this.BackGroundColor = bkcolor;
            UtilForms.SetBackColorRec(this, bkcolor);
        }


        void SetCommonEvents(Control f)
        // Sets common events for the consform and its sub-controls
        {
            {
                try
                {
                    // This will enable to kill the windowby clicking the mouse button 3
                    f.MouseClick += new MouseEventHandler(this.FadeMessage_MouseClick);
                }
                catch (Exception e) { Exception ee = e; }
                try
                {
                    // This will allow dragging the window by mouse:
                    f.MouseDown += new MouseEventHandler(this.FadeMessage_MouseDown);
                    f.MouseMove += new MouseEventHandler(this.FadeMessage_MouseMove);
                }
                catch (Exception e) { Exception ee = e; }
            }
        }

		private void FadeMessage_Load(object sender, System.EventArgs e)
		{
            if (!this.Counted) {  this.Counted = true; ++NumShown; }
            try
            {
                // Set common event handlers recursively:
                UtilForms.RecursiveControlDelegate(this, new ControlDelegate(SetCommonEvents));

                // Set up timer parameters:
                firsttick = (int)((1.0 - FadingTimePortion) * (double)ShowTime);
                // Set the total number of ticks such that fading is smooth but there are not too much timer intervals:
                tickinterval = mintickinterval;
                totalticks = (int)(FadingTimePortion * (double)ShowTime / (double)tickinterval);
                if (totalticks > maxticks)
                {
                    totalticks = maxticks;
                    tickinterval = (int)(FadingTimePortion * (double)ShowTime / (double)totalticks);
                }
                timer1.Interval = firsttick;
                timer1.Enabled = true;
            }
            catch (Exception) {  }
            finally
            {
                try
                {
                    lblTitle.ForeColor = ForeGroundColorTitle;
                    lblMEssage.ForeColor = ForeGroundColorMsg;
                    if (MsgTitle == null)
                        MsgTitle = "";
                    lblTitle.Text = MsgTitle;
                    if (MsgTitle.Length < 1)
                    {
                        lblTitle.Visible = false;
                        // lblMEssage.Location.Y = 4;
                    }
                    
                    this.lblMEssage.Text = MsgText;

                    UtilForms.SetBackColorRec(this, BackGroundColor);
                    //if (0 != 1)
                    //{
                    //    this.StatusLbl.Text = "< Right-Click to close >";
                    //    // report fading parameters in the status line (uncomment this only for testing)
                    //    //this.StatusLbl.Text = "firsttick = " + firsttick.ToString() + "  tickinterval = " + tickinterval
                    //    //    + "  totalticks = " + totalticks.ToString();
                    //}
                    this.ShowDialog();
                }
                catch (Exception) { }
                finally
                {
                    this.IsReady = true;
                    try
                    {
                        lblStatus.Visible = false;  // do not show the status label
                        btnCancel.Visible = false;
                        // PnlStatus.Visible = false;

                        // Ensure proper size of controls:
                        lblMEssage.Top = lblTitle.Location.Y;
                        if (lblTitle.Text!=null && lblTitle.Visible == true)
                            lblMEssage.Top = lblTitle.Location.Y + lblTitle.Size.Height;
                        // Place the distance label such that its bottom edge will be at the same position as the bottom edge of
                        // the bottom Panel:
                        labelBottom.Text="";
                        if (PnlStatus.Visible)
                            labelBottom.Top = BodyPnl.Location.Y + BodyPnl.Size.Height + PnlStatus.Size.Height - labelBottom.Size.Height;
                        else // otherwise we do not need a distance keeping label.
                            labelBottom.Top = BodyPnl.Top;

                        //PnlStatus.Top = BodyPnl.Location.Y + BodyPnl.Size.Height;

                        //PnlStatus.Right = 0;
                        //PnlStatus.Size.Width = BodyPnl.Size.Width ;
                        
                    }
                    catch { }
                }
            }
		}

        private void FadeMessage_Disposed(object sender, System.EventArgs e)
        {
            if (this.Counted) { this.Counted = false; --NumShown; }
            this.IsReady = false;
        }

        private void FadeMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the consform is tagged as non-closable, then cancel the closing of the consform:
            if (!this.IsClosable)
            {
                e.Cancel = true;
            }
            else
            {
                this.IsReady = false;
            }
        }



        private void CancelBtn_Click(object sender, EventArgs e)
        {
            CloseForm();
        }



        // Public methods:


        public void CloseForm()
        /// Closes the FadeMessage by properly (i.e. thread-safe) calling the Close() and Dispose().
        {
            try
            {
                if (this.InvokeRequired)
                {
                    // Delegate the method when called consform a thread not owning the consform.
                    VoidDelegate fref = new VoidDelegate(CloseForm);
                    this.Invoke(fref);
                }
                else
                {
                    this.IsReady = false;
                    // Stop the timer first:
                    if (timer1 != null)
                        timer1.Enabled = false;
                    // Call appropriate system methods to close the consform:
                    this.Close();
                    if (!this.IsDisposed)
                        this.Dispose();
                }
            }
            catch (Exception e) { Exception ee = e; }
            finally
            {
                if (this.Counted) { this.Counted = false; --NumShown; }
            }
        }


        public void ShowThread()
        /// Shows a fading message in a new thread.
        /// It first launches a new thread that launches the thread managing the consform. 
        //  In this way, the calling thread returns quickly and robust manipulation of the consform can be done.
        {
            if (BlockMaxShown || this.BlockMaxShownCurrent)
            {
                // If specified, the calling thread blocks until the number of launched messages gets down to the allowed number.
                while (NumShown >= MaxShown)
                    Thread.Sleep(20);
            }
            else
            {

                // In the launcing thread, perform some waiting if the number of launching forms heavily exceeds the maximum allowed number:
                int count = 0, waitinterval = 10, maxwaittime = 200;
                while (NumShown >= 2 * MaxShown && count * waitinterval <= maxwaittime)
                    Thread.Sleep(waitinterval); // Take care that not too many threads are open
                while (NumShown >= 3 * MaxShown && count * waitinterval <= 1000)
                    Thread.Sleep(waitinterval);
            }
            if (!this.Counted) { this.Counted = true; ++NumShown; }  // add this instance to the counter.
            manipulationthread = new Thread(new ThreadStart(ManipulationThreadFunc));
            manipulationthread.IsBackground = true;
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
        /// Shows the consform in a new thread, with a title and with message text equal to msgtext.
        {
            if (title != null) if (title.Length>0) MsgTitle = title;
            if (text != null) if (text.Length > 0) MsgText = text;
            ShowThread();
        }

        public void ShowThread(string title, string msgtext, int showtime)
        /// Shows a fading message in a new thread, with a title and with message text equal to  msgtext, with specified showing time in ms.
        { ShowTime = showtime; ShowThread(title, msgtext); }

        public void ShowThread(string title, string msgtext, int showtime, double fadeportion)
        /// Shows a fading message in a new thread, with a title and with message text equal to  msgtext, with specified showing time in ms and fading time portion.
        { ShowTime = showtime; FadingTimePortion = fadeportion; ShowThread(title, msgtext); }


        private void FadeMessage_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    bool ControlPressed = false;
                    try
                    {
                        Control SenderControl = sender as Control;
                        if (SenderControl != null)
                        {
                            if (Control.ModifierKeys == Keys.Control)
                                ControlPressed = true;
                        }
                    }
                    catch { }
                    if (ControlPressed)
                        CloseForm();
                }
            }
            catch { }
        }

        private void PnlStatus_Paint(object sender, PaintEventArgs e)
        {

        }


        public static void Example()
        {

            //Creating a fading message in this thread, which must be canceled explicitly (e.g. by pressing mouse button 3:)
            string msgtitle = "FadeMessage; Naslov;                                          Konec Naslova.\nXX\nYY";
            string msgtext = "My label.\nLine 2\nLine 3\nLine 4\nLine 5\n\nLLLLL";
            FadingMessage fm = new FadingMessage(msgtitle, msgtext, 8000);
        }  // Example1()



        public static void Example2()
        {
            // Creating a fading message in this thread, which must be canceled explicitly (e.g. by pressing mouse button 3:)
            FadingMessage fm = new FadingMessage();
            fm.MsgTitle = "Test message";
            fm.MsgText = "My label.";
            fm.ShowDialog();

            // Fading message tests:
            new FadingMessage("Fading message", "Fade message, 4 s  this is a very long fade message in order to see how the window size is adapted\r\nline2\n\rline 3\n\rline4\r\nline5\r\n\r\n\r\n\r\n\r\n\r\nBottomline",
                9000);
            new FadingMessage("Fade message, 4 s", 9000);
            new FadingMessage("Fade message, 1 s", 9000);


            // Test launching many fading messages:
            int averagetime = 5000, deviation = 2000;
            Random rnd = new Random(0);
            for (int i = 1; i <= 800; ++i)
            {
                int showtime = averagetime + rnd.Next(deviation);
                float f = (float)showtime / (float)1000;
                new FadingMessage("Fade message " + i.ToString() + ", " + f.ToString() + " s",
                        averagetime + rnd.Next(deviation),
                        0.5);
            }

        } // FadeMessageExample2()




	}  // class FadeMessage : System.Windows.Forms.Form

}  // namespace IG.Forms
