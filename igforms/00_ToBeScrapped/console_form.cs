//// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

//using System;
//using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
//using System.Windows.Forms;
//using System.Threading;
//using System.IO;

//using IG.Lib;

//using IG.Forms;


//// $$$$Excluded


//namespace IG.Forms11
//{
	
//    /// <summary>Console replacement.</summary>
//	public class ConsoleForm : System.Windows.Forms.Form
//    //$A Igor Jul08;
//    {

//        /// Handles a consform that can be use as console output and input window, for logging messages, etc.
//        /// Input and output window are separated. The window can be launched in a separate thread.
//        // $A Igor Jul08; 

//        public enum Styles { Normal, Error, Mark };

//        // public bool IsReady = false; /// Returns true if control has been created and is ready to work with.
//        public bool IsClosable = true;  /// If false then the consform can not be closed or dispoded.
//        public bool WriteInput = true; /// If true then Input messages are repeated in the output console.

//        private bool bgthread = true;
//        public bool IsBackground
//        // $A Igor Jul08; 
//        {
//            set { bgthread = value; if (formthread != null) formthread.IsBackground = bgthread; }
//            get { return bgthread; }
//        }
    
//        private object
//            InputLock = new object(),
//            OutputLock = new object();
        
//        public string Title = "Console";

//        private Color 
//            OBg = Color.White,
//            OFg = Color.Blue,
//            OSelFg = Color.Blue,
//            IBg = Color.White, 
//            IFg = Color.Violet,
              
//            ILblNormalBg=Color.Gray,
//            ILblNormalFg=Color.Black,
//            ILblActiveBg=Color.Yellow,
//            ILblActiveFg=Color.Blue,
//            ILblErrorBg=Color.Red,
//            ILblErrorFg=Color.Blue;
        
//        public Color OutBackColor /// Output console background color.
//        // $A Igor Jul08; 
//        {
//            set { OBg = value; UpdateSettings(); }
//            get { return OBg; }    
//        }
        
//        public Color OutForeColor /// Output console foreground color.
//        // $A Igor Jul08; 
//        {
//            set { OFg = value; UpdateSettings(); }
//            get { return OFg; }    
//        }

//        public Color OutSelectionColor /// Output console foreground color.
//        // $A Igor Jul08; 
//        {
//            set { OSelFg = value; UpdateOutSettings(); }
//            get { return OSelFg; }
//        }


//        // *****************************
//        // Definition of output styles
//        // *****************************

//        public Color
//            NormalOutColor = Color.Blue,
//            ErrorOutColor = Color.Red,
//            MarkOutColor = Color.Violet;
//        private CheckBox chkWrap;


//        private Styles OStyle = Styles.Normal;  // Contains the current value of the output style.

//        /// <summary>Defines the output style of the console.</summary>
//        public Styles Style
//        // $A Igor Jul08; 
//        {
//            get { return OStyle; }
//            set
//            {
//                try
//                {
//                    switch (value)
//                    {
//                        case Styles.Normal:
//                            SetNormalStyle();
//                            break;
//                        case Styles.Error:
//                            SetErrorStyle();
//                            break;
//                        case Styles.Mark:
//                            SetMarkStyle();
//                            break;
//                        default:
//                            Exception e = new Exception("Setting of the following style is not yet implemented: "
//                                + value.ToString());
//                            break;
//                    }
//                }
//                catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.Style.Set"); }
//            }
//        }

//        /// <summary>Sets normal style for console output.</summary>
//        public void SetNormalStyle()
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                OStyle = Styles.Normal;
//                // Insert here all settings related to this style!
//                // ...
//                OutSelectionColor = NormalOutColor;  // This also causes the update!!
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }

//        /// <summary>Sets Error Style for console output.</summary>
//        public void SetErrorStyle()
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                OStyle = Styles.Error;
//                // Insert here all settings related to this style!
//                // ...
//                OutSelectionColor = ErrorOutColor;  // This also causes the update!!
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }

//        /// <summary>Sets marking (emphasis) style for console output.</summary>
//        public void SetMarkStyle()
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                OStyle = Styles.Mark;
//                // Insert here all settings related to this style!
//                // ...
//                OutSelectionColor = MarkOutColor;  // This also causes the update!!
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }


        



//        public Color InBackColor /// Input console background color.
//        // $A Igor Jul08; 
//        {
//            set { IBg = value; UpdateSettings(); }
//            get { return IBg; }    
//        }
        
//        public Color InForeColor /// Input console foreground color.
//        // $A Igor Jul08; 
//        {
//            set { IFg = value; UpdateSettings(); }
//            get { return IFg; }    
//        }


//        private void UpdateOutSettings()
//        /// Updates only those settings of the console consform that influence output operation, in the consform'result thread.
//        /// This is performed separately from UpdateSettings() to enable faster execution of this vital update.
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                if (this != null && !this.IsDisposed && !this.Disposing)
//                {
//                    if (this.InvokeRequired)
//                    {
//                        // Delegate the method when called consform a thread not owning the consform.
//                        VoidDelegate fref = new VoidDelegate(UpdateOutSettings);
//                        this.Invoke(fref);
//                    }
//                    else
//                    {
//                        OutBox.SelectionColor = OutSelectionColor;
//                    }
//                }
//            }
//            catch (Exception e) 
//            {
//                ReserveReportError(e,"Problem in ConsoleForm.UpdateOutSettings"); // Can not use normal error reporting here.
//            }
//        }


//        private void UpdateSettings()
//        /// Updates all settings for the console consform on the consform'result thread.
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                if (this != null && !this.IsDisposed && !this.Disposing)
//                {
//                    if (this.InvokeRequired)
//                    {
//                        // Delegate the method when called consform a thread not owning the consform.
//                        VoidDelegate fref = new VoidDelegate(UpdateSettings);
//                        this.Invoke(fref);
//                    }
//                    else
//                    {
//                        OutBox.BackColor = OBg;
//                        OutBox.ForeColor = OFg;

//                        InBox.BackColor = IBg;
//                        InBox.ForeColor = IFg;
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                ReserveReportError(e,"Problem in ConsoleForm.UpdateSettings"); // Can not use normal error reporting here.
//            }
//            UpdateOutSettings(); 
//        }

        






//        private System.Windows.Forms.Button HideBtn;
//        private Panel TitlePanel;
//        private Panel OutputOuterPnl;
//        private Panel ControlPanel;
//        private Label TitleLbl;
//        // private IContainer vec;
//        private RichTextBox OutBox;
//        private Label OutputLbl;
//        private Panel InputPnl;
//        private RichTextBox InBox;
//        private Label InputLbl;
//        private Label InputMsgLbl;
//        private Button ConfirmInputBtn;
//        private CheckBox InputChk;
//        private Button button1;
//        private Label StatusLbl;
//		/// <summary>
//		/// Required designer variable.
//		/// </summary>


//        private void baseConsoleForm()
//        /// Common initialization part called from all constructors.
//        // $A Igor Jul08; 
//        {
//			InitializeComponent();
//        }

//        public ConsoleForm()
//        /// Argument-less constructor, does not launch the window in a parallel thread.
//        // $A Igor Jul08; 
//        {
//			baseConsoleForm();
//		}


//        public ConsoleForm(string msgtext)
//        /// Shows a console consform in a new thread, with message text equal to mshtext and without a title;
//        // $A Igor Jul08; 
//        { baseConsoleForm(); ShowThread(msgtext); }



//		#region Windows Form Designer generated code
//		/// <summary>
//		/// Required method for Designer support - do not modify
//		/// the contents of this method with the code editor.
//		/// </summary>
//		private void InitializeComponent()
//		{
//            this.HideBtn = new System.Windows.Forms.Button();
//            this.TitlePanel = new System.Windows.Forms.Panel();
//            this.TitleLbl = new System.Windows.Forms.Label();
//            this.OutputOuterPnl = new System.Windows.Forms.Panel();
//            this.OutBox = new System.Windows.Forms.RichTextBox();
//            this.OutputLbl = new System.Windows.Forms.Label();
//            this.ControlPanel = new System.Windows.Forms.Panel();
//            this.InputChk = new System.Windows.Forms.CheckBox();
//            this.button1 = new System.Windows.Forms.Button();
//            this.StatusLbl = new System.Windows.Forms.Label();
//            this.InputPnl = new System.Windows.Forms.Panel();
//            this.InputMsgLbl = new System.Windows.Forms.Label();
//            this.ConfirmInputBtn = new System.Windows.Forms.Button();
//            this.InBox = new System.Windows.Forms.RichTextBox();
//            this.InputLbl = new System.Windows.Forms.Label();
//            this.chkWrap = new System.Windows.Forms.CheckBox();
//            this.TitlePanel.SuspendLayout();
//            this.OutputOuterPnl.SuspendLayout();
//            this.ControlPanel.SuspendLayout();
//            this.InputPnl.SuspendLayout();
//            this.SuspendLayout();
//            // 
//            // HideBtn
//            // 
//            this.HideBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.HideBtn.AutoSize = true;
//            this.HideBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
//            this.HideBtn.Location = new System.Drawing.Point(343, 13);
//            this.HideBtn.Margin = new System.Windows.Forms.Padding(5);
//            this.HideBtn.Name = "HideBtn";
//            this.HideBtn.Size = new System.Drawing.Size(83, 44);
//            this.HideBtn.TabIndex = 2;
//            this.HideBtn.Text = "Hide";
//            this.HideBtn.Click += new System.EventHandler(this.HideBtn_Click);
//            // 
//            // TitlePanel
//            // 
//            this.TitlePanel.AutoSize = true;
//            this.TitlePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
//            this.TitlePanel.Controls.Add(this.TitleLbl);
//            this.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
//            this.TitlePanel.ForeColor = System.Drawing.Color.Blue;
//            this.TitlePanel.Location = new System.Drawing.Point(4, 4);
//            this.TitlePanel.Margin = new System.Windows.Forms.Padding(0);
//            this.TitlePanel.Name = "TitlePanel";
//            this.TitlePanel.Size = new System.Drawing.Size(530, 45);
//            this.TitlePanel.TabIndex = 3;
//            // 
//            // TitleLbl
//            // 
//            this.TitleLbl.AutoSize = true;
//            this.TitleLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
//            this.TitleLbl.Location = new System.Drawing.Point(5, 6);
//            this.TitleLbl.Name = "TitleLbl";
//            this.TitleLbl.Padding = new System.Windows.Forms.Padding(5);
//            this.TitleLbl.Size = new System.Drawing.Size(120, 39);
//            this.TitleLbl.TabIndex = 0;
//            this.TitleLbl.Text = "Console";
//            // 
//            // OutputOuterPnl
//            // 
//            this.OutputOuterPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
//            this.OutputOuterPnl.Controls.Add(this.OutBox);
//            this.OutputOuterPnl.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.OutputOuterPnl.Location = new System.Drawing.Point(4, 80);
//            this.OutputOuterPnl.Margin = new System.Windows.Forms.Padding(5);
//            this.OutputOuterPnl.Name = "OutputOuterPnl";
//            this.OutputOuterPnl.Padding = new System.Windows.Forms.Padding(5);
//            this.OutputOuterPnl.Size = new System.Drawing.Size(530, 173);
//            this.OutputOuterPnl.TabIndex = 3;
//            // 
//            // OutBox
//            // 
//            this.OutBox.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.OutBox.Location = new System.Drawing.Point(5, 5);
//            this.OutBox.Name = "OutBox";
//            this.OutBox.Size = new System.Drawing.Size(520, 163);
//            this.OutBox.TabIndex = 4;
//            this.OutBox.Text = "";
//            this.OutBox.WordWrap = false;
//            // 
//            // OutputLbl
//            // 
//            this.OutputLbl.AutoSize = true;
//            this.OutputLbl.Dock = System.Windows.Forms.DockStyle.Top;
//            this.OutputLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
//            this.OutputLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
//            this.OutputLbl.Location = new System.Drawing.Point(4, 49);
//            this.OutputLbl.Margin = new System.Windows.Forms.Padding(0);
//            this.OutputLbl.Name = "OutputLbl";
//            this.OutputLbl.Padding = new System.Windows.Forms.Padding(3);
//            this.OutputLbl.Size = new System.Drawing.Size(155, 31);
//            this.OutputLbl.TabIndex = 0;
//            this.OutputLbl.Text = "Output Console";
//            // 
//            // ControlPanel
//            // 
//            this.ControlPanel.AutoSize = true;
//            this.ControlPanel.Controls.Add(this.InputChk);
//            this.ControlPanel.Controls.Add(this.button1);
//            this.ControlPanel.Controls.Add(this.HideBtn);
//            this.ControlPanel.Controls.Add(this.StatusLbl);
//            this.ControlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
//            this.ControlPanel.Location = new System.Drawing.Point(4, 528);
//            this.ControlPanel.Name = "ControlPanel";
//            this.ControlPanel.Size = new System.Drawing.Size(530, 64);
//            this.ControlPanel.TabIndex = 3;
//            // 
//            // InputChk
//            // 
//            this.InputChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.InputChk.AutoSize = true;
//            this.InputChk.Checked = true;
//            this.InputChk.CheckState = System.Windows.Forms.CheckState.Checked;
//            this.InputChk.Location = new System.Drawing.Point(196, 33);
//            this.InputChk.Name = "InputChk";
//            this.InputChk.Size = new System.Drawing.Size(116, 24);
//            this.InputChk.TabIndex = 4;
//            this.InputChk.Text = "Show Input";
//            this.InputChk.UseVisualStyleBackColor = true;
//            this.InputChk.CheckedChanged += new System.EventHandler(this.InputChk_CheckedChanged);
//            // 
//            // button1
//            // 
//            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.button1.AutoSize = true;
//            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
//            this.button1.Location = new System.Drawing.Point(427, 13);
//            this.button1.Margin = new System.Windows.Forms.Padding(5);
//            this.button1.Name = "button1";
//            this.button1.Size = new System.Drawing.Size(95, 44);
//            this.button1.TabIndex = 2;
//            this.button1.Text = "Close";
//            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
//            this.button1.Click += new System.EventHandler(this.CloseBtn_Click);
//            // 
//            // StatusLbl
//            // 
//            this.StatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
//            this.StatusLbl.AutoSize = true;
//            this.StatusLbl.Location = new System.Drawing.Point(8, 38);
//            this.StatusLbl.Name = "StatusLbl";
//            this.StatusLbl.Size = new System.Drawing.Size(149, 20);
//            this.StatusLbl.TabIndex = 3;
//            this.StatusLbl.Text = "< Console window >";
//            // 
//            // InputPnl
//            // 
//            this.InputPnl.Controls.Add(this.InputMsgLbl);
//            this.InputPnl.Controls.Add(this.ConfirmInputBtn);
//            this.InputPnl.Controls.Add(this.InBox);
//            this.InputPnl.Controls.Add(this.InputLbl);
//            this.InputPnl.Dock = System.Windows.Forms.DockStyle.Bottom;
//            this.InputPnl.Location = new System.Drawing.Point(4, 253);
//            this.InputPnl.Name = "InputPnl";
//            this.InputPnl.Size = new System.Drawing.Size(530, 275);
//            this.InputPnl.TabIndex = 5;
//            // 
//            // InputMsgLbl
//            // 
//            this.InputMsgLbl.AutoSize = true;
//            this.InputMsgLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
//            this.InputMsgLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic);
//            this.InputMsgLbl.Location = new System.Drawing.Point(0, 45);
//            this.InputMsgLbl.Margin = new System.Windows.Forms.Padding(3);
//            this.InputMsgLbl.Name = "InputMsgLbl";
//            this.InputMsgLbl.Padding = new System.Windows.Forms.Padding(3);
//            this.InputMsgLbl.Size = new System.Drawing.Size(111, 30);
//            this.InputMsgLbl.TabIndex = 0;
//            this.InputMsgLbl.Text = "<< Input >>";
//            // 
//            // ConfirmInputBtn
//            // 
//            this.ConfirmInputBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//            this.ConfirmInputBtn.Location = new System.Drawing.Point(376, 48);
//            this.ConfirmInputBtn.Name = "ConfirmInputBtn";
//            this.ConfirmInputBtn.Size = new System.Drawing.Size(146, 31);
//            this.ConfirmInputBtn.TabIndex = 5;
//            this.ConfirmInputBtn.Text = "Confirm Input";
//            this.ConfirmInputBtn.UseVisualStyleBackColor = true;
//            this.ConfirmInputBtn.Click += new System.EventHandler(this.ConfirmInputBtn_Click);
//            // 
//            // InBox
//            // 
//            this.InBox.Dock = System.Windows.Forms.DockStyle.Bottom;
//            this.InBox.Location = new System.Drawing.Point(0, 88);
//            this.InBox.Name = "InBox";
//            this.InBox.Size = new System.Drawing.Size(530, 187);
//            this.InBox.TabIndex = 4;
//            this.InBox.Text = "";
//            this.InBox.WordWrap = false;
//            this.InBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.InBox_KeyPress);
//            this.InBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.InBox_KeyUp);
//            this.InBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.InBox_PreviewKeyDown);
//            // 
//            // InputLbl
//            // 
//            this.InputLbl.AutoSize = true;
//            this.InputLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
//            this.InputLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
//            this.InputLbl.Location = new System.Drawing.Point(3, 10);
//            this.InputLbl.Margin = new System.Windows.Forms.Padding(0);
//            this.InputLbl.Name = "InputLbl";
//            this.InputLbl.Padding = new System.Windows.Forms.Padding(3);
//            this.InputLbl.Size = new System.Drawing.Size(139, 31);
//            this.InputLbl.TabIndex = 0;
//            this.InputLbl.Text = "Input Console";
//            // 
//            // chkWrap
//            // 
//            this.chkWrap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
//            this.chkWrap.AutoSize = true;
//            this.chkWrap.Location = new System.Drawing.Point(458, 60);
//            this.chkWrap.Name = "chkWrap";
//            this.chkWrap.Size = new System.Drawing.Size(73, 24);
//            this.chkWrap.TabIndex = 6;
//            this.chkWrap.Text = "Wrap";
//            this.chkWrap.UseVisualStyleBackColor = true;
//            this.chkWrap.CheckedChanged += new System.EventHandler(this.chkWrap_CheckedChanged);
//            // 
//            // ConsoleForm
//            // 
//            this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
//            this.ClientSize = new System.Drawing.Size(538, 596);
//            this.Controls.Add(this.chkWrap);
//            this.Controls.Add(this.OutputOuterPnl);
//            this.Controls.Add(this.InputPnl);
//            this.Controls.Add(this.ControlPanel);
//            this.Controls.Add(this.OutputLbl);
//            this.Controls.Add(this.TitlePanel);
//            this.MinimumSize = new System.Drawing.Size(560, 585);
//            this.Name = "ConsoleForm";
//            this.Padding = new System.Windows.Forms.Padding(4);
//            this.Text = "Console";
//            this.TopMost = true;
//            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
//            this.Load += new System.EventHandler(this.ConsoleForm_Load);
//            this.Disposed += new System.EventHandler(this.ConsoleForm_Dispose);
//            this.TitlePanel.ResumeLayout(false);
//            this.TitlePanel.PerformLayout();
//            this.OutputOuterPnl.ResumeLayout(false);
//            this.ControlPanel.ResumeLayout(false);
//            this.ControlPanel.PerformLayout();
//            this.InputPnl.ResumeLayout(false);
//            this.InputPnl.PerformLayout();
//            this.ResumeLayout(false);
//            this.PerformLayout();

//		}
//		#endregion



//        private Thread formthread = null;

//        [STAThread]
//        private void FormThreadFunc()
//        /// Shows the window in a separate thread.
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                this.TitleLbl.Text = Title;
//                this.StatusLbl.Text = "  ";
//                this.ShowDialog( );
//            }
//            catch (Exception e) { UtilForms.ReportError(e,"\nInside ConsoleForm."); }
//        }



//        private void ConsoleForm_Load(object sender, System.EventArgs args)
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                //_Reporter = new Reporter(this, 
//                //    new ReportDelegate(ReportErrorBas),
//                //    new ReserveReportErrorDelegate(ReserveReportErrorBas));
//                this.TitleLbl.Text = Title;
//                this.StatusLbl.Text = "  ";
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.Load"); }
//            finally
//            {
//                try
//                {
//                    UpdateSettings();
//                }
//                catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.Load"); }
//                // IsReady = true;
//            }
//		} 


//        private void ConsoleForm_Dispose(object sender, System.EventArgs e)
//        // $A Igor Jul08; 
//        {
//            try {
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }

//        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                // If the consform is tagged as non-closable, then cancel the closing of the consform:
//                if (!this.IsClosable)
//                {
//                    e.Cancel = true;
//                }
//                else
//                {
//                    // this.IsReady = false;
//                    //if (formthread != null)
//                    //    if (formthread.IsAlive)
//                    ////        formthread.Abort();
//                }
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }


//        private void CloseBtn_Click(object sender, EventArgs e)
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                CloseForm();
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }

//        private void HideBtn_Click(object sender, EventArgs e)
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                HideForm();
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }




//        // Public methods:

//        public void HideForm()
//        // $A Igor Jul08; 
//        {
//            try
//            {
//            this.Visible = false;
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }


//        public void CloseForm()
//        /// Closes the consform by properly (i.e. thread-safe) calling the Close() and Dispose().
//        // $A Igor Jul08; 
//        {
//            try
//            {
//                if (this.InvokeRequired)
//                {
//                    // Delegate the method when called consform a thread not owning the consform.
//                    VoidDelegate fref = new VoidDelegate(CloseForm);
//                    this.Invoke(fref);
//                }
//                else
//                {
//                    // this.IsReady = false;
//                    // Call appropriate system methods to close the consform:
//                    this.Close();
//                    if (!this.IsDisposed)
//                        this.Dispose();
//                    //if (formthread!=null)
//                    //    if (formthread.IsAlive)
//                    //        formthread.Abort();
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }


//        #region Output

//        // **************************
//        // Writing to console output
//        // **************************

//        protected string OutBuf=null;
//        protected bool OutputStarted=false, OutputEnded=false;


//        private void WriteOutput()
//        /// Appends output buffer to the output console.
//        // $A Igor Aug08; 
//        {
//            try
//            {
//                if (this.InvokeRequired)
//                {
//                    // Delegate the method when called consform a thread not owning the consform.
//                    VoidDelegate fref = new VoidDelegate(WriteOutput);
//                    this.Invoke(fref);
//                }
//                else
//                {
//                    OutputStarted=true;
//                    OutBox.AppendText(OutBuf);
//                    OutBox.SelectionStart = OutBox.Text.Length;
//                    OutBox.SelectionLength = 0;
//                    OutBox.ScrollToCaret();
//                    OutBuf=null;
//                    OutputEnded=true;
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }



//        int RecCountWrite = 0;

//        public void Write(bool block,string str)
//        /// Writes a string to the output console. 
//        /// If block=true, then it blocks until the string is actually written.
//        // $A Igor Aug08; 
//        {
//            try
//            {
//                if (this.OutputLock == null)
//                {
//                    throw (new Exception("ConsoleformWrite: Can not acquire the output lock."));
//                }
//                else if (this == null || this.IsDisposed || this.Disposing)
//                {
//                    // ReserveReportError(errorstr);
//                    throw (new Exception("ConsoleForm.Write: Form is being disposed."));
//                }
//                else
//                {
//                    if (RecCountWrite > 0)
//                    {
//                        try
//                        {
//                            new FadingMessage("ConoleForm.Write: Skipped to prevent recursive locking.\nString to write:\n"
//                                + str, 8000);
//                        }
//                        catch (Exception e) 
//                        {
//                            ReserveReportError(e, "ConsoleForm.Write:\nAfter writing skipped to avoid recursive locking.");
//                        }
//                    }
//                    else
//                    {
//                        lock (OutputLock)
//                        {
//                            ++RecCountWrite;
//                            try
//                            {
//                                OutBuf = str;
//                                OutputStarted = OutputEnded = false;
//                                WriteOutput();
//                                if (block)
//                                {
//                                    while (!OutputEnded && !this.IsDisposed)
//                                        Thread.Sleep(5);
//                                }
//                            }
//                            finally
//                            {
//                                --RecCountWrite;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception e) 
//            {
//                string outstr="";
//                if (str!=null)
//                {
//                    int maxlength=100;
//                    if (str.Length<=maxlength)
//                        outstr=str;
//                    else
//                        outstr=str.Substring(0,maxlength);
//                }
//                if (outstr == null)
//                    outstr = "";
//                ReserveReportError(e,"Problem in ConsoleForm.Write.\nCould not write the sring:\n"
//                     + outstr);
//            }
//        }

//        public void Write(string str) { Write(false,str); }  // non-blocking.

//        public void WriteLine(bool block, string str)
//        /// Writes a string to the output console and appends a newline character.
//        // $A Igor Aug08; 
//        { Write(block, str + System.Environment.NewLine); }

//        public void WriteLine(string str) { WriteLine(false, str); }  // non-blocking.


//        #region Output_Overloaded

//        //*********************************************
//        // Overloaded methods for Write and WriteLine:

//        public void Write(Char arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(Boolean arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(Char[] arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(Decimal arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(Double arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(Int32 arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(Int64 arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(Object arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(Single arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(UInt32 arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(UInt64 arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); Write(sw.ToString()); }
//        }

//        public void Write(String arg, Object arg1)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1); Write(sw.ToString()); }
//        }

//        public void Write(String arg, Object[] arg1)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1); Write(sw.ToString()); }
//        }

//        public void Write(Char[] arg, Int32 arg1, Int32 arg2)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1, arg2); Write(sw.ToString()); }
//        }

//        public void Write(String arg, Object arg1, Object arg2)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1, arg2); Write(sw.ToString()); }
//        }

//        public void Write(String arg, Object arg1, Object arg2, Object arg3)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1, arg2, arg3); Write(sw.ToString()); }
//        }


//        // Overloaded methods for WriteLine:

//        public void WriteLine(Char arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Boolean arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Char[] arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Decimal arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Double arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Int32 arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Int64 arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Object arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Single arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(UInt32 arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(UInt64 arg)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(String arg, Object arg1)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(String arg, Object[] arg1)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(Char[] arg, Int32 arg1, Int32 arg2)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1, arg2); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(String arg, Object arg1, Object arg2)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1, arg2); WriteLine(sw.ToString()); }
//        }

//        public void WriteLine(String arg, Object arg1, Object arg2, Object arg3)
//        {
//            using (StringWriter sw = new StringWriter())
//            { sw.Write(arg, arg1, arg2, arg3); WriteLine(sw.ToString()); }
//        }


//        #endregion  // Output_Overloaded

//        #endregion  // Output


//        #region Error_Reporting

//        private object errorlock = new object();
//        private int reccounterror = 0;



//        public void ReportErrorBas(ReporterBase reporter, ReportType messagelevel, string errorlocation, string errormessage)
//        // $A Igor Oct08; 
//        {
//            lock (errorlock)
//            {
//                try
//                {
//                    ++reccounterror;
//                    if (reccounterror > 1)
//                        throw new Exception("Recursive call to ConsoleForm.ReportError() occurred.");
//                    if (reporter == null)
//                        throw new Exception("Reporter of the console is not initialized.");
//                    else
//                    {
//                        ConsoleForm cons = null;
//                        try
//                        {
//                            cons = reporter.Obj as ConsoleForm;
//                        }
//                        catch { }
//                        if (cons == null)
//                            throw new ArgumentException("Argument for error reporting can not be converted to console form.");
//                        if (cons.IsDisposed)
//                            throw new Exception("Console form has been disposed and can nt be used for reporting errors any more.");
//                        string msg = "";
//                        // Assemble the report message:
//                        msg = ReporterBase.DefaultReportStringConsoleTimeStamp(reporter, messagelevel,
//                                errorlocation, errormessage);

//                        //msg += Environment.NewLine;
//                        //msg += Environment.NewLine;
//                        //switch(messagelevel)
//                        //{
//                        //    case ReportType.Error:
//                        //        if (!string.IsNullOrEmpty(errorlocation))
//                        //            msg += "==== ERROR in " + errorlocation + ": ==";
//                        //        else
//                        //            msg += "==== ERROR: ================";
//                        //        break;
//                        //    case ReportType.Warning:
//                        //        if (!string.IsNullOrEmpty(errorlocation))
//                        //            msg += "==== Warning in " + errorlocation + ": ==";
//                        //        else
//                        //            msg += "==== Warning: ================";
//                        //        break;
//                        //    default:
//                        //        if (!string.IsNullOrEmpty(errorlocation))
//                        //            msg += "==== Info from " + errorlocation + ": ==";
//                        //        else
//                        //            msg += "==== Info: ================";
//                        //        break;
//                        //}
//                        //msg += Environment.NewLine;
//                        //msg += errormessage;  // message desctibing the error
//                        //msg += Environment.NewLine;
//                        //msg += "====";
//                        //msg += Environment.NewLine;
//                        //msg += Environment.NewLine;

//                        // Write the report in the console:
//                        Styles CurrentStyle = cons.Style;
//                        switch (messagelevel)
//                        {
//                            case ReportType.Error:
//                                cons.Style = Styles.Error;
//                                break;
//                            case ReportType.Warning:
//                                cons.Style = Styles.Mark;
//                                break;
//                            default:
//                                cons.Style = Styles.Normal;
//                                break;
//                        }
//                        cons.Style = Styles.Error;
//                        cons.Write(msg);
//                        cons.Style = CurrentStyle;
//                    }
//                }
//                catch (Exception ex1)
//                {
//                    ReserveReportErrorBas(reporter, ReportType.Error, errorlocation, errormessage, null, ex1);
//                }
//                finally
//                {
//                    --reccounterror;
//                }
//            }
//        }  // ReportErrorBas

//        public void ReserveReportErrorBas(ReporterBase reporter, ReportType messagelevel, string location, string message, Exception ex, Exception ex1)
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                string msg = "";
//                try
//                {
//                    msg = Environment.NewLine + Environment.NewLine + "*******************************"
//                    + "Error in error reporting procedure." + Environment.NewLine;
//                }
//                catch { }
//                try
//                {
//                    msg += "Error Description: " + Environment.NewLine + "  " + ex1.Message + Environment.NewLine;
//                }
//                catch { }
//                try
//                {
//                    if (location != null || message != null || ex != null)
//                    {
//                        msg += "Original Error: " + Environment.NewLine;
//                        if (location != null)
//                            msg += "Location: " + location + Environment.NewLine;
//                        if (message != null)
//                            msg += "Message: " + message + Environment.NewLine;
//                        if (ex != null)
//                            msg += "Exception's message: " + ex.Message + Environment.NewLine;
//                    }
//                }
//                catch { }
//                try
//                {
//                    msg += "****" + Environment.NewLine + Environment.NewLine;
//                }
//                catch { }
//                try
//                {
//                    Console.Write(msg);
//                }
//                catch { }
//                try
//                {
//                    MessageBox.Show(msg);
//                }
//                catch { }
//            }
//            catch { }
//        }

//        // Error reporter
//        protected ReporterBase _Reporter = null;


//        /// <summary>Gets the reporter used by the current console consform for launching error and warning reports, notices, etc.</summary>
//        public ReporterBase Reporter
//        // $A Igor Oct08;
//        {
//            get 
//            {
//                if (_Reporter == null)
//                {
//                    _Reporter = new ReporterBase(this,
//                            new ReportDelegate(ReportErrorBas),
//                            new ReserveReportErrorDelegate(ReserveReportErrorBas));
//                    if (_Reporter == null)
//                        throw new Exception("Reporter of the console form could not be initialized.");
//                }
//                return _Reporter; 
//            }
//        }

//        // TODO: Consider whether the ReserveReportError() is necessary!
//        private void ReserveReportError(Exception e, string additional)
//        // Reports an error when it can not be reported in the usual way.
//        // $A Igor Oct08; 
//        {
//            ReserveReportErrorBas(null, ReportType.Error, "ConsoleForm", additional, e, null);
//        }

//        public void ReserveReportError(string errorstr)
//        {
//            ReserveReportErrorBas(null, ReportType.Error, "ConsoleForm", errorstr, null, null);
//        }


//        // VARIOUS REPORTING METHODS:


//        // GENERAL LOCAL REPORTING METHODS OF THE PRESENT CONSOLE FORM (for all kinds of reports):


//        /// <summary>Basic reporting method (overloaded). Launches an error report, a warning report or s kind of report/message.
//        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
//        /// are obtained from the class' instance.</summary>
//        /// <param name="messagetype">The type of the report (e.g. Error, Warning, etc.).</param>
//        /// <param name="location">User-provided description of error location.</param>
//        /// <param name="message">User-provided description of error.</param>
//        /// <param name="ex">Exception thrown when error occurred.</param>
//        public virtual void Report(ReportType messagetype, string location, string message, Exception ex)
//        // $A Igor Oct08;
//        {
//            Reporter.Report(messagetype, location, message, ex);
//        }

//        // Overloaded general reporting methods (different combinations of parameters passed):

//        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
//        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
//        /// <param name="message">User-provided description of error.</param>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        public void Report(ReportType messagetype, string message, Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(messagetype, null /* location */, message, ex);
//        }

//        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
//        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        /// <param name="location">User-provided description of error location.</param>
//        public void Report(ReportType messagetype, Exception ex, string location)
//        // $A Igor Oct08;
//        {
//            Report(messagetype, location, null /* message */, ex);
//        }

//        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
//        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        public void Report(ReportType messagetype, Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(messagetype, null /* location */ , null /* message */, ex);
//        }

//        /// <summary>Launches a report.</summary>
//        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
//        /// <param name="location">User provided description of the location where report was triggered.</param>
//        /// <param name="message">User provided message included in the report.</param>
//        public void Report(ReportType messagetype, string location, string message)
//        // $A Igor Oct08;
//        {
//            Report(messagetype, location, message, null /* ex */ );
//        }

//        /// <summary>Launches a report.</summary>
//        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
//        /// <param name="message">User provided message included in the report.</param>
//        public void Report(ReportType messagetype, string message)
//        // $A Igor Oct08;
//        {
//            Report(messagetype, null /* location */, message, null /* ex */ );
//        }


//        // ERROR REPORTING FUNCTIONS:


//        /// <summary>Basic error reporting method (overloaded).
//        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
//        /// are obtained from the class' instance.</summary>
//        /// <param name="location">User-provided description of error location.</param>
//        /// <param name="message">User-provided description of error.</param>
//        /// <param name="ex">Exception thrown when error occurred.</param>
//        public virtual void ReportError(string location, string message, Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Error, location, message, ex);
//        }

//        // Overloaded general reporting methods (different combinations of parameters passed):

//        /// <summary>Launches an error report.</summary>
//        /// <param name="message">User-provided description of error.</param>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        public void ReportError(string message, Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Error, null /* location */, message, ex);
//        }

//        /// <summary>Launches an error report.</summary>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        /// <param name="location">User-provided description of error location.</param>
//        public void ReportError(Exception ex, string location)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Error, location, null /* message */, ex);
//        }

//        /// <summary>Launches an error report. Predominantly for error and warning reports.</summary>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        public void ReportError(Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Error, null /* location */ , null /* message */, ex);
//        }

//        /// <summary>Launches an error report.</summary>
//        /// <param name="location">User provided description of the location where report was triggered.</param>
//        /// <param name="message">User provided message included in the report.</param>
//        public void ReportError(string location, string message)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Error, location, message, null /* ex */ );
//        }

//        /// <summary>Launches an error report.</summary>
//        /// <param name="message">User provided message included in the report.</param>
//        public void ReportError(string message)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Error, null /* location */, message, null /* ex */ );
//        }



//        // WARNING LAUNCHING FUNCTIONS:


//        /// <summary>Basic warning reporting method (overloaded).
//        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
//        /// are obtained from the class' instance.</summary>
//        /// <param name="location">User-provided description of error location.</param>
//        /// <param name="message">User-provided description of error.</param>
//        /// <param name="ex">Exception thrown when error occurred.</param>
//        public virtual void ReportWarning(string location, string message, Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Warning, location, message, ex);
//        }

//        // Overloaded general reporting methods (different combinations of parameters passed):

//        /// <summary>Launches a warning report.</summary>
//        /// <param name="message">User-provided description of error.</param>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        public void ReportWarning(string message, Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Warning, null /* location */, message, ex);
//        }

//        /// <summary>Launches a warning report.</summary>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        /// <param name="location">User-provided description of error location.</param>
//        public void ReportWarning(Exception ex, string location)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Warning, location, null /* message */, ex);
//        }

//        /// <summary>Launches a warning report. Predominantly for error and warning reports.</summary>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        public void ReportWarning(Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Warning, null /* location */ , null /* message */, ex);
//        }

//        /// <summary>Launches a warning report.</summary>
//        /// <param name="location">User provided description of the location where report was triggered.</param>
//        /// <param name="message">User provided message included in the report.</param>
//        public void ReportWarning(string location, string message)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Warning, location, message, null /* ex */ );
//        }

//        /// <summary>Launches a warning report.</summary>
//        /// <param name="message">User provided message included in the report.</param>
//        public void ReportWarning(string message)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Warning, null /* location */, message, null /* ex */ );
//        }



//        // INFO LAUNCHING FUNCTIONS:


//        /// <summary>Launches an info.</summary>
//        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
//        public void ReportInfo(Exception ex)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Info, null /* location */ , null /* message */, ex);
//        }

//        /// <summary>Launches an info.</summary>
//        /// <param name="location">User provided description of the location where report was triggered.</param>
//        /// <param name="message">User provided message included in the report.</param>
//        public void ReportInfo(string location, string message)
//        // $A Igor Oct08;
//        {
//            Report(ReportType.Info, location, message, null /* ex */ );
//        }

//        /// <summary>Launches an info.</summary>
//        /// <param name="message">User provided message included in the report.</param>
//        public void ReportInfo(string message)
//        {
//            Report(ReportType.Info, null /* location */, message, null /* ex */ );
//        }





//        //public void ReportError(string errorstr)
//        ///// ConsoleForm utility for reporting errors (not global).
//        ///// Global error reporting utility uses this method on the global console when options are set in this way.
//        ///// Otherwise, for reporting errors, the global utility should normally be used.
//        //// $A Igor Oct08; 
//        //{
//        //    try
//        //    {
//        //        Reporter.ReportError(errorstr);
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        ReserveReportError(e, "Problem in ConsoleForm.ReportError.");
//        //    }

//        //}

//        //public void ReportError(Exception e)
//        ///// Reports an error (including logging, etc., dependent on the current settings).
//        //// $A Igor Oct08; 
//        //{
//        //    try
//        //    {
//        //        Reporter.ReportError(e);
//        //    }
//        //    catch (Exception ee)
//        //    {
//        //        ReserveReportError(ee, "Problem in ReportError."); // Can not use normal error reporting here.
//        //    }
//        //}

//        //public void ReportError(Exception e, string additional)
//        ///// Reports an error (including logging, etc., dependent on the current settings).
//        ///// An additonal string is appended to error report.
//        //// $A Igor Oct08; 
//        //{
//        //    try
//        //    {
//        //        Reporter.ReportError(null /* location */, additional, e);
//        //    }
//        //    catch (Exception ee)
//        //    {
//        //        ReserveReportError(ee, "Originall error message: " + ee.Message);
//        //    }
//        //}









        
//        #endregion Error_Reporting



//        #region ErrorReporting_Old



//        private int RecCountError = 0;
//        private object ErrorLock00 = new object();
        
//        public void ReportError0(string errorstr)
//        /// ConsoleForm utility for reporting errors (not global).
//        // Global error reporting utility uses this method on the global console when options are set in this way.
//        // Otherwise, for reporting errors, the global utility should normally be used.
//        {
//            try
//            {
//                if (this.ErrorLock00 == null)
//                {
//                    ReserveReportError(errorstr);
//                    throw (new Exception("Can not acquire the error lock."));
//                }
//                else if (this == null || this.IsDisposed || this.Disposing)
//                {
//                    ReserveReportError(errorstr);
//                    throw (new Exception("ConsoleForm.ReportError: Form is being disposed."));
//                }
//                else
//                {
//                    if (RecCountError > 0)
//                    {
//                        try
//                        {
//                            new FadingMessage("ConoleForm.Error: Skipped to prevent recursive locking.\nError message:\n"
//                                + errorstr, 8000);
//                        }
//                        catch (Exception e) 
//                        {
//                            ReserveReportError(e,"In Console.ReportError to prevent recursive locking;\nError Message:\n"
//                                    +errorstr);
//                        }
//                    }
//                    else
//                    {
//                        lock (ErrorLock00)
//                        {
//                            ++RecCountError;
//                            try
//                            {
//                                string str = errorstr;
//                                str = System.Environment.NewLine +
//                                      System.Environment.NewLine + "ERROR:" + System.Environment.NewLine +
//                                      errorstr + System.Environment.NewLine + System.Environment.NewLine;


//                                Styles CurrentStyle = this.Style;
//                                this.Style = Styles.Error;
//                                this.Write(str);
//                                this.Style = CurrentStyle;
//                            }
//                            finally
//                            {
//                                --RecCountError;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                ReserveReportError(e,"Problem in ConsoleForm.ReportError.");
//            }

//        }

//        public void ReportError0(Exception e)
//        /// Reports an error (including logging, etc., dependent on the current settings).
//        {
//            string errstr=null;
//            try { 
//                errstr = UtilForms.GetErorString(e);
//                this.ReportError0( errstr ); 
//            }
//            catch (Exception ee) 
//            {
//                ReserveReportError(ee,"Problem in ReportError."); // Can not use normal error reporting here.
//            }
//        }

//        public void ReportError0(Exception e, string additional)
//        /// Reports an error (including logging, etc., dependent on the current settings).
//        /// An additonal string is appended to error report.
//        {
//            string errstr = null;
//            try
//            {
//                errstr = UtilForms.GetErorString(e) + System.Environment.NewLine
//                    + additional
//                    + System.Environment.NewLine;
//                this.ReportError0( errstr );
//            }
//            catch (Exception ee)
//            {
//                ReserveReportError(ee,"Originall error message: " + ee.Message);
//            }
//        }




//        private void ReserveReportError0(string errorstr)
//        // Utility for reporting errors when normal reporting is not possible.
//        {
//            string str = null;
//            try
//            {
//                str = System.Environment.NewLine +
//                      System.Environment.NewLine + 
//                      "ERROR:" + System.Environment.NewLine +
//                      errorstr + 
//                      System.Environment.NewLine + System.Environment.NewLine;
//            }
//            catch (Exception e)
//            {
//                str=errorstr;
//                Exception eee = e;
//            }
//            try
//            {
//                FadingMessage fm = new FadingMessage();
//                fm.MsgTitle = "Error in ConsoleForm";
//                fm.MsgText = str;
//                fm.BackGroundColor = Color.Orange;
//                fm.ShowThread(str, 8000);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("\n" + str + "\n");
//                //Console.WriteLine("Additional error occurred when reporting this error:\d2" + ex.Message
//                //    +"\d2\d2");
//                Exception ex1 = ex;
//            }
//        }

//        private void ReserveReportError0(Exception e)
//        // Reports an error when it can not be reported in the usual way.
//        {
//            string errstr = null;
//            try
//            {
//                errstr = UtilForms.GetErorString(e);
//                this.ReserveReportError(errstr);
//            }
//            catch (Exception ee)
//            {
//                try
//                {
//                    errstr=e.Message;
//                    this.ReserveReportError(errstr+"\nAdditional error:\n"+ee.Message);
//                }
//                catch (Exception ex) { Exception eee = ex; }
//            }
//        }

//        private void ReserveReportError0(Exception e, string additional)
//        // Reports an error when it can not be reported in the usual way.
//        {
//            string errstr = null;
//            try
//            {
//                errstr = UtilForms.GetErorString(e) + System.Environment.NewLine
//                    + additional
//                    + System.Environment.NewLine;
//                this.ReserveReportError(errstr);
//            }
//            catch (Exception ee)
//            {
//                try
//                {
//                    errstr = e.Message + "\n" + additional + "\nAdditional error:\n" + ee.Message;
//                    this.ReserveReportError(errstr);
//                }
//                catch (Exception ex) { this.ReserveReportError0(ex); }

//            }
//        }

//        #endregion  // ErrorReporting_Old


//        #region Input

//        //**********
//        //   Input
//        //**********

//        public void HideInput()
//        // $A Igor Sept08; 
//        {
//            try
//            {
//                if (this.InvokeRequired)
//                {
//                    // Delegate the method when called consform a thread not owning the consform.
//                    VoidDelegate fref = new VoidDelegate(HideInput);
//                    this.Invoke(fref);
//                }
//                else
//                {
//                    InputChk.Checked=false;
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }

//        public void ShowInput()
//        // $A Igor Sept08; 
//        {
//            try
//            {
//                if (this.InvokeRequired)
//                {
//                    // Delegate the method when called consform a thread not owning the consform.
//                    VoidDelegate fref = new VoidDelegate(HideInput);
//                    this.Invoke(fref);
//                }
//                else
//                {
//                    InputChk.Checked=false;
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }


//        private void PrepareForReading()
//        // $A Igor Sept08; 
//        {
//            try
//            {
//                if (this.InvokeRequired)
//                {
//                    // Delegate the method when called consform a thread not owning the consform.
//                    VoidDelegate fref = new VoidDelegate(PrepareForReading);
//                    this.Invoke(fref);
//                }
//                else
//                {
//                    InBox.Clear();
//                    if (InputResultString != null)
//                    {
//                        InBox.Text = InputResultString;
//                        if (SelectInput)
//                        {
//                            InBox.SelectionStart = 0;
//                            InBox.SelectionLength = InBox.Text.Length;
//                        } else
//                        {
//                            InBox.SelectionStart = InBox.Text.Length;
//                            InBox.SelectionLength = 0;
//                        }
//                    }
//                    InputMsgLbl.BackColor = ILblActiveBg;
//                    InputMsgLbl.ForeColor = ILblActiveFg;
//                    if (InputMessage!=null)
//                    {
//                        InputMsgLbl.Text=InputMessage;
//                    } else
//                        InputMsgLbl.Text="";
//                    DeleteLastNewline = false;
//                    InputChk.Checked=true;
//                    InBox.Enabled=true;
//                    InBox.Focus();
//                    ReadStarted = true;
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }


//        private void FinalizeReading()
//        // $A Igor Sept08; 
//        {
//            try
//            {
//                if (this.InvokeRequired)
//                {
//                    // Delegate the method when called consform a thread not owning the consform.
//                    VoidDelegate fref = new VoidDelegate(FinalizeReading);
//                    this.Invoke(fref);
//                }
//                else
//                {
//                    InputMessage = null;
//                    InBox.Enabled=false;
//                    InputMsgLbl.Text="Last input:";
//                    // InBox.Clear();
//                    InputMsgLbl.BackColor = ILblNormalBg;
//                    InputMsgLbl.ForeColor = ILblNormalFg;
//                    if (WriteInput)
//                    {
//                        // Replicate user input in the output console:
//                        string str;
//                        if (!MultiLineInput && !SingleCharacterInput)
//                            str=InputResultString+System.Environment.NewLine;
//                        else
//                            str=InputResultString;
//                        Styles CurrentStyle = this.Style;
//                        this.Style = Styles.Mark;
//                        if (SingleCharacterInput)
//                            this.Write(str);
//                        else
//                            this.WriteLine(str);
//                        this.Style = CurrentStyle;
//                    }
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//        }


//        private string InputResultString=null;
//        private string InputMessage=null;
//        private bool ReadStarted=false, ReadFinished=false,
//            MultiLineInput = false, SingleCharacterInput = false, DeleteLastNewline = false,
//            SelectInput=false;
//        private int KeyValue = 0;
//        private Keys KeyData = Keys.None, KeyCode=Keys.None;

//        // Culture-invariant parsing of numbers:
//        private IFormatProvider FormatProvider = System.Globalization.CultureInfo.CreateSpecificCulture("");

        
//        VoidDelegate InputConfirmDelegate; // Input confirmation handler 



//        private void WaitResult()
//        /// Waits until result is available (indicating by the flag ReadFinished, which must be set by the 
//        /// ReadDelegate).
//        // $A Igor Oct08; 
//        {
//            int count=0;
//            while (!ReadFinished && !this.IsDisposed)
//            {
//                ++count;
//                if (count<100)
//                    Thread.Sleep(10);
//                else if(count<200)
//                    Thread.Sleep(20);
//                else if (count<300)
//                    Thread.Sleep(40);
//                else if (count<400)
//                    Thread.Sleep(100);
//                else if (count<500)
//                    Thread.Sleep(100);
//                else
//                    Thread.Sleep(100);
//            }
//        }


//        private void RunInputConfirmationDelegate()
//        /// Runs specific function for handling user input consform input console, which grabs the input, eventually
//        /// validates it, and notifies the caller that reading is finished when validation succeeds.
//        // $A Igor Oct08; 
//        {
//            if (ReadStarted && !ReadFinished)
//            {
//                try
//                {
//                    if (InputConfirmDelegate == null)
//                    {
//                        Exception e = new Exception("Input confirmation delegate is not specified.");
//                        throw (e);
//                    }
//                    else
//                        InputConfirmDelegate();
//                }
//                catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm."); }
//            }
//        }




//        //************************************
//        // Reading of strings and characters:
//        //************************************

//        private void InputConfirm_Read()
//        /// This function should be called as event handler for input events (such as <Enter> pressed in the input
//        /// console or button "Confirm Input" pressed).
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (ReadStarted && !ReadFinished)
//                {
//                    InputResultString = InBox.Text;  // This must be performed in ALL input handlers
//                    if (DeleteLastNewline)
//                    {
//                        int length = InputResultString.Length;
//                        if (length > 0 && InputResultString[length - 1] == '\n')
//                            --length;
//                        if (length > 0 && InputResultString[length - 1] == '\r')
//                            --length;
//                        InputResultString = InputResultString.Substring(0, length);
//                    }
//                    ReadFinished = true;  // This must be performed in ALL input handlers, indicates that input was successfully read; 
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.InputConfirm_Read."); }
//        }


//        public string ReadLine()
//        /// Reads a single line string from the input console and returns it.
//        // $A Igor Oct08; 
//        {
//            return ReadLine(null);
//        }

//        public string ReadLine(string Message)
//        /// Reads a single line string from the input console and returns it.
//        /// If Message!=Null then Message is written above the input consform before reading.
//        // $A Igor Oct08; 
//        {
//            string result=null;
//            try
//            {
//                if (this.InputLock == null)
//                {
//                    throw (new Exception("Consoleform.ReadLine: Can not acquire the input lock."));
//                }
//                else if (this == null || this.IsDisposed || this.Disposing)
//                {
//                    // ReserveReportError(errorstr);
//                    throw (new Exception("ConsoleForm.ReadLine: Form is being disposed."));
//                }
//                else
//                    lock (InputLock)
//                    {
//                        if (Message == null)
//                            InputMessage = "Insert a string (<Enter> to confirm): ";
//                        else InputMessage = Message;
//                        InputResultString = null;
//                        // specific - no initialization of input before reading
//                        MultiLineInput = false;  // specific
//                        SingleCharacterInput = false; // specific
//                        SelectInput = false;  // specific
//                        ReadStarted = ReadFinished = false;
//                        InputConfirmDelegate = new VoidDelegate(InputConfirm_Read);  // Specific
//                        PrepareForReading();
//                        WaitResult();
//                        result = InputResultString;  // specific
//                        FinalizeReading();
//                    }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.ReadLine"); }
//            return result;
//        }

//        public void ReadString(ref string str)
//        {
//            ReadString(ref str, null);
//        }

//        public void ReadString(ref string str, string Message)
//        /// Reads a multiple line string from the input console and returns it in OutStr.
//        /// If Message!=Null then Message is written above the input consform before reading.
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (this.InputLock == null)
//                {
//                    throw (new Exception("Consoleform.ReadString: Can not acquire the input lock."));
//                }
//                else if (this == null || this.IsDisposed || this.Disposing)
//                {
//                    // ReserveReportError(errorstr);
//                    throw (new Exception("ConsoleForm.ReadString: Form is being disposed."));
//                }
//                else
//                {
//                    lock (InputLock)
//                    {
//                        if (Message == null)
//                            InputMessage = "Insert a string (<Ctrl-Enter> to confirm): ";
//                        else InputMessage = Message;
//                        InputResultString = str;
//                        // Specific - no initialization of input before reading
//                        MultiLineInput = true;  // specific
//                        SingleCharacterInput = false; // specific
//                        SelectInput = true;  // specific
//                        ReadStarted = ReadFinished = false;
//                        InputConfirmDelegate = new VoidDelegate(InputConfirm_Read);  // Specific
//                        PrepareForReading();
//                        WaitResult();
//                        str = InputResultString;  // specific
//                        FinalizeReading();
//                    }
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.ReadString."); }
//        }



//        public int Read()
//        /// Reads a single character from the input console and returns it as integer.
//        // $A Igor Oct08; 
//        {
//            return Read(null);
//        }
       
//        public int Read(string Message)
//        /// Reads a single character from the input console and returns it as integer.
//        /// If Message!=null then it is written above the input consform before reading starts.
//        // $A Igor Oct08; 
//        {
//            string result = null;
//            try
//            {
//                if (this.InputLock == null)
//                {
//                    throw (new Exception("Consoleform.Read: Can not acquire the input lock."));
//                }
//                else if (this == null || this.IsDisposed || this.Disposing)
//                {
//                    // ReserveReportError(errorstr);
//                    throw (new Exception("ConsoleForm.Read: Form is being disposed."));
//                }
//                else
//                    lock (InputLock)
//                    {
//                        if (Message == null)
//                            InputMessage = "Insert a character: ";
//                        else InputMessage = Message;
//                        InputResultString = null;
//                        // specific - no initialization of input before reading
//                        MultiLineInput = false;  // specific
//                        SingleCharacterInput = true; // specific
//                        SelectInput = false;  // specific
//                        ReadStarted = ReadFinished = false;
//                        InputConfirmDelegate = new VoidDelegate(InputConfirm_Read);  // Specific
//                        PrepareForReading();
//                        WaitResult();
//                        result = InputResultString;  // specific
//                        FinalizeReading();
//                    }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.Read."); }
//            if (result != null) if (result.Length>0)
//                return (int)result[0];
//            return KeyValue;
//        }



//        //*********************
//        // Reading of numbers:
//        //*********************

//        private void InputConfirm_ReadDouble()
//        // This function should be called as event handler for input events (such as <Enter> pressed in the input
//        // console or button "Confirm Input" pressed), for confirmation of floating point numbers input.
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (ReadStarted && !ReadFinished)
//                {
//                    InputResultString = InBox.Text;  // This must be performed in ALL input handlers
//                    if (DeleteLastNewline)
//                    {
//                        int length = InputResultString.Length;
//                        if (length > 0 && InputResultString[length - 1] == '\n')
//                            --length;
//                        if (length > 0 && InputResultString[length - 1] == '\r')
//                            --length;
//                        InputResultString = InputResultString.Substring(0, length);
//                    }

//                    if (InputResultString != null)
//                    {
//                        if (InputResultString.Length > 0)
//                        {
//                            bool err = false;
//                            double val;
//                            try { val = double.Parse(InputResultString, FormatProvider); }
//                            catch { err = true; }
//                            if (!err)
//                                ReadFinished = true;  // No error in parsing, result is valid
//                            else
//                            {
//                                // Error in number format
//                                InBox.SelectionStart = 0;
//                                InBox.SelectionLength = InBox.Text.Length;
//                                InputMsgLbl.BackColor = ILblErrorBg;
//                                InputMsgLbl.ForeColor = ILblErrorFg;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.InputConfirm_ReadDouble."); }
//        }


//        public double ReadDouble(ref double Value)
//        /// Reads a floating point number from the input console and returns it in Value.
//        /// Initial value of the number is set to Value prior to reading.
//        // $A Igor Oct08; 
//        {
//            return ReadDouble(ref Value, null);
//        }

//        public double ReadDouble(ref double Value, string Message)
//        /// Reads a floating point number from the input console and returns it in Value.
//        /// If Message!=null then it is written above the input consform before reading starts.
//        /// Initial value of the number is set to Value prior to reading.
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (this.InputLock == null)
//                {
//                    throw (new Exception("Consoleform.ReadDouble: Can not acquire the input lock."));
//                }
//                else if (this == null || this.IsDisposed || this.Disposing)
//                {
//                    // ReserveReportError(errorstr);
//                    throw (new Exception("ConsoleForm.ReadDouble: Form is being disposed."));
//                }
//                else
//                    lock (InputLock)
//                    {
//                        if (Message == null)
//                            InputMessage = "Insert a number (<Enter> to confirm): ";
//                        else InputMessage = Message;
//                        InputResultString = Value.ToString(FormatProvider);
//                        // Specific - no initialization of input before reading
//                        MultiLineInput = false;  // specific
//                        SingleCharacterInput = false; // specific
//                        SelectInput = true;  // specific
//                        ReadStarted = ReadFinished = false;
//                        InputConfirmDelegate = new VoidDelegate(InputConfirm_ReadDouble);  // Specific
//                        PrepareForReading();
//                        WaitResult();
//                        Value = double.Parse(InputResultString, FormatProvider);  // specific
//                        FinalizeReading();
//                    }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.ReadDouble"); }
//            return Value;
//        }

//        public double ReadDouble()
//        /// Reads a floating point number from the input console and returns it.
//        // $A Igor Oct08; 
//        {
//            double Val = 0.0;
//            ReadDouble(ref Val);
//            return Val;
//        }

//        public double ReadDouble(string Message)
//        /// Reads a floating point number from the input console and returns it.
//        /// If Message!=null then it is written above the input consform before reading starts.
//        // $A Igor Oct08; 
//        {
//            double Val = 0.0;
//            ReadDouble(ref Val, Message);
//            return Val;
//        }



//        private void InputConfirm_ReadLong()
//        /// This function should be called as event handler for input events (such as <Enter> pressed in the input
//        /// console or button "Confirm Input" pressed), for confirmation of floating point numbers input.
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (ReadStarted && !ReadFinished)
//                {
//                    InputResultString = InBox.Text;  // This must be performed in ALL input handlers
//                    if (DeleteLastNewline)
//                    {
//                        int length = InputResultString.Length;
//                        if (length > 0 && InputResultString[length - 1] == '\n')
//                            --length;
//                        if (length > 0 && InputResultString[length - 1] == '\r')
//                            --length;
//                        InputResultString = InputResultString.Substring(0, length);
//                    }

//                    if (InputResultString != null)
//                    {
//                        if (InputResultString.Length > 0)
//                        {
//                            bool err = false;
//                            long val;
//                            try { val = long.Parse(InputResultString, FormatProvider); }
//                            catch { err = true; }
//                            if (!err)
//                                ReadFinished = true;  // No error in parsing, result is valid
//                            else
//                            {
//                                // Error in number format
//                                InBox.SelectionStart = 0;
//                                InBox.SelectionLength = InBox.Text.Length;
//                                InputMsgLbl.BackColor = ILblErrorBg;
//                                InputMsgLbl.ForeColor = ILblErrorFg;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.InputConfirm_ReadLong."); }
//        }


//        public long ReadLong(ref long Value)
//        /// Reads a floating point number from the input console and returns it in Value.
//        /// Initial value of the number is set to Value prior to reading.
//        // $A Igor Oct08; 
//        {
//            return ReadLong(ref Value, null);
//        }

//        public long ReadLong(ref long Value, string Message)
//        /// Reads a floating point number from the input console and returns it in Value.
//        /// If Message!=null then it is written above the input consform before reading starts.
//        /// Initial value of the number is set to Value prior to reading.
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (this.InputLock == null)
//                {
//                    throw (new Exception("Consoleform.ReadLong: Can not acquire the input lock."));
//                }
//                else if (this == null || this.IsDisposed || this.Disposing)
//                {
//                    // ReserveReportError(errorstr);
//                    throw (new Exception("ConsoleForm.ReadLong: Form is being disposed."));
//                }
//                else
//                    lock (InputLock)
//                    {
//                        if (Message == null)
//                            InputMessage = "Insert a number (<Enter> to confirm): ";
//                        else InputMessage = Message;
//                        InputResultString = Value.ToString(FormatProvider);
//                        // Specific - no initialization of input before reading
//                        MultiLineInput = false;  // specific
//                        SingleCharacterInput = false; // specific
//                        SelectInput = true;  // specific
//                        ReadStarted = ReadFinished = false;
//                        InputConfirmDelegate = new VoidDelegate(InputConfirm_ReadLong);  // specific
//                        PrepareForReading();
//                        WaitResult();
//                        Value = long.Parse(InputResultString, FormatProvider);  // specific
//                        FinalizeReading();
//                    }
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "\nInside ConsoleForm.ReadLong."); }
//            return Value;
//        }

//        public long ReadLong()
//        /// Reads a floating point number from the input console and returns it.
//        // $A Igor Oct08; 
//        {
//            long Val = 0;
//            ReadLong(ref Val);
//            return Val;
//        }

//        public long ReadLong(string Message)
//        /// Reads a floating point number from the input console and returns it.
//        /// If Message!=null then it is written above the input consform before reading starts.
//        {
//            long Val = 0;
//            ReadLong(ref Val, Message);
//            return Val;
//        }

//        #endregion   // Input


//        public void ShowThread()
//        /// Shows a fading message in a new thread.
//        // $A Igor Oct08; 
//        {
//            formthread = new Thread(new ThreadStart(FormThreadFunc));
//            formthread.IsBackground = bgthread;
//            formthread.Start();
//        }

//        public void ShowThread(string title)
//        /// Shows a fading message in a new thread, with message text equal to mshtext and without a title;
//        // $A Igor Oct08; 
//        {
//            if (title != null) if (title.Length > 0) Title = title;
//            ShowThread();
//        }



//        private void InputChk_CheckedChanged(object sender, EventArgs e)
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (InputChk.Checked)
//                {
//                    InputPnl.Visible = true;
//                }
//                else
//                {
//                    InputPnl.Visible = false;
//                }
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }

//        private void ConfirmInputBtn_Click(object sender, EventArgs e)
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                DeleteLastNewline = false;
//                RunInputConfirmationDelegate();
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }


//        private void InBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
//        // $A Igor Oct08; 
//        {
//            try
//            {
//            //// Use this for debugging!
//            //string str = "";
//            //str += "\nPreviewKeyDown: ";
//            //if (e.Control) str += " Ctrl ";
//            //if (e.Alt) str += " Alt ";
//            //if (e.Shift) str += " Shift ";
//            //str += " Key Code: " + e.KeyCode;
//            //str += " Key Data: " + e.KeyData;
//            //str += " Key Value: " + e.KeyValue;
//            //str += " Key Modifiers: " + e.Modifiers;
//            //str += " IsInput: " + e.IsInputKey;
//            //IGForm.WriteLine(str);
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }


//        private void InBox_KeyUp(object sender, KeyEventArgs e)
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (e.Control && e.KeyCode == Keys.Enter)
//                {
//                    // IGForm.WriteLine("\nKeyUp Ctrl-Enter");
//                    if (ReadStarted && !ReadFinished)
//                    {
//                        // IGForm.WriteLine("\nKeyUp event is handled with <Ctrl-Enter> presed, confirmation delegate was run here.");
//                        // e.Handled = true;
//                        DeleteLastNewline = true;
//                        RunInputConfirmationDelegate();
//                    }
//                }
//                else if (SingleCharacterInput && ReadStarted && !ReadFinished)
//                {
//                    if (!(e.Alt || e.Control))
//                    {
//                        KeyValue = e.KeyValue;
//                        KeyData = e.KeyData;
//                        KeyCode = e.KeyCode;
//                        DeleteLastNewline = false;
//                        RunInputConfirmationDelegate();
//                    }
//                }
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }


//        private void InBox_KeyPress(object sender, KeyPressEventArgs e)
//        // $A Igor Oct08; 
//        {
//            try
//            {
//                if (e.KeyChar == '\r')
//                {
//                    //IGForm.WriteLine("\nKeyPress <Enter>.");
//                    if (ReadStarted && !ReadFinished && !MultiLineInput)
//                    {
//                        //IGForm.WriteLine("\nKeyPress event is handled, key = <Enter>, confirmation delegate was run here.");
//                        //e.Handled = true;
//                        DeleteLastNewline = true;
//                        RunInputConfirmationDelegate();
//                    }
//                }
//            }
//            catch (Exception ex) { UtilForms.ReportError(ex, "\nInside ConsoleForm."); }
//        }

//        private void chkWrap_CheckedChanged(object sender, EventArgs e)
//        // Toggles wrapping mode for output console.
//        {
//            if (chkWrap.Checked)
//                OutBox.WordWrap = true;
//            else
//                OutBox.WordWrap = false;
//        }


//        public static void Example()
//        {

//            // Console consform - sequential:
//            //ConsoleForm cons1 = new ConsoleForm();
//            //cons1.Title = "Console in a serial thread.";
//            //cons1.ShowDialog();
//            //Thread.Sleep(0);



//            // Console consform in a parallel thread:
//            ConsoleForm cons = new ConsoleForm("Testing console");
//            ConsoleForm cons1 = new ConsoleForm("Second console");


//            double dval = 5.55; ;
//            while (dval != 0 && !cons.IsDisposed)
//            {
//                dval = cons.ReadDouble(ref dval, "Insert a number (0 to exit): ");
//                cons1.WriteLine("\n\nValue of a number read from input console: " + dval.ToString());
//            }


//            Thread.Sleep(4000);

//            for (int i = 1; i <= 5; ++i)
//            {
//                cons.WriteLine(i.ToString() + ".  vrstica");
//                cons1.WriteLine(i.ToString() + ".  vrstica, izpis v cons1");
//            }
//            cons.WriteLine("After for loop.");
//            //Thread.Sleep(1000);
//            cons.WriteLine("After the first sleep after for loop.\n");
//            //Thread.Sleep(2000);



//            try
//            {
//                cons.OutSelectionColor = Color.Brown;
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "ERROR when trying to set OutForeColor."); }

//            // new FadeMessage("AFT SETTIG COL.",2000);



//            // Thread.Sleep(5000);

//            try
//            {
//                cons.WriteLine("\n\nThis line should appear in brown.\n");
//            }
//            catch (Exception e) { UtilForms.ReportError(e, "ERROR when trying to write AFTER SETTING OutForeColor."); }

//            // Thread.Sleep(1000);
//            // new FadeMessage("AFT WRITING with sleep",2000);

//            try
//            {
//                cons.OutSelectionColor = Color.Orange;
//                cons.WriteLine("This line should appear in Orange.");
//                cons.OutSelectionColor = Color.LightGreen;
//                cons.WriteLine("This line should appear in LightGreen.");
//                cons.Style = ConsoleForm.Styles.Normal;
//                cons.WriteLine("\nBack to normal style.");
//                cons.Style = ConsoleForm.Styles.Error;
//                cons.WriteLine("\nThis is in Error style.");
//                cons.SetMarkStyle();
//                cons.WriteLine("\nThis is in Mark Style.");
//                cons.Style = ConsoleForm.Styles.Normal;
//                cons.WriteLine("\nAnd this is again in normal style.");

//                cons.WriteLine("So we are back to normal style now.\n");
//                cons.WriteLine("This should stil appear in normal style.\n");

//            }
//            catch (Exception e) { UtilForms.ReportError(e, "ERROR in STYLES testing block."); }


//        }  // Example()


   



      
//	}  // Class ConsoleForm
//}
