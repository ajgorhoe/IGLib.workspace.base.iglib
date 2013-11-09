// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using IG.Lib;
using IG.Forms;

namespace IG.Forms
{
	
    /// <summary>Reporter configuration.</summary>
	public class ReporterConf : System.Windows.Forms.Form
    //$A Igor Oct08;
    {

        private ReporterForms _reporter = null;

        protected ReporterForms reporter
        {
            private set { _reporter = value; }
            get { return _reporter; }
        }


        #region Initialization

        private ReporterConf() { }  // prevent calling argument-less constructor

        public ReporterConf(ReporterForms rf)
        {
            InitializeComponent();
            reporter = rf;
            if (reporter == null)
                throw new ArgumentException("Forms Reporter to be configured is not specified (null reference).");
        }


        #endregion  // Initialization


        #region Control_Definitions

        private bool bgthread = false;
        private GroupBox grpFadeMessage;
        private Panel pnlFadingMessageSwitch;
        private RadioButton rbFadeMessageOn;
        private RadioButton rbFadeMessageOff;
        private Label lblFadeBGFade;
        private Button btnFadeBGFinal;
        private Button btnFadeBG;
        private Label lblFadingBG;
        private TextBox txtFadeBGFinal;
        private Label lblFadingPortion;
        private TextBox txtFadeBG;
        private Label lblFadingShowtime;
        private TextBox txtFadePortion;
        private TextBox txtFadeShowtime;
        private GroupBox grpConsole;
        private Panel pnlConsoleSwitch;
        private RadioButton rbConsoleOn;
        private RadioButton rbConsoleOff;
        private GroupBox grpConsoleForm;
        private Panel pnlConsoleFormSwitch;
        private RadioButton rbConsoleFormOn;
        private RadioButton rbConsoleFormOff;
        private GroupBox grpMessageBox;
        private Panel pnlMessageBoxSwitch;
        private RadioButton rbMessageBoxOn;
        private RadioButton rbMessageBoxOff;
        private GroupBox grpTest;
        private GroupBox grpOutputLevel;
        private RadioButton rbOutVerbose;
        private RadioButton rbOutInfo;
        private RadioButton rbOutWarning;
        private RadioButton rbOutError;
        private GroupBox grpReportType;
        private RadioButton rbInfo;
        private RadioButton rbWarning;
        private RadioButton rbError;
        private Button btnLaunch;
        private GroupBox grpMultipleReports;
        private CheckBox chkMultiple;
        private TextBox txtNumLaunches;
        private TextBox txtDelay;
        private Label lblMultipleNum;
        private Label lblDelay;
        private Label label1;
        private Label lblMessage;
        private TextBox txtExMessage;
        private Label lblLocation;
        private TextBox txtMessage;
        private TextBox txtLocation;
        private Label lblDescription;
        private Label lblTitle;
        private Panel PnlStatus;
        private Button CancelBtn;
        private Label StatusLbl;

        #endregion  // Control_Definitions
        private CheckBox chkThrowException;


        public bool IsBackground
        // $A Igor Jul08; 
        {
            set { bgthread = value; if (formthread != null) formthread.IsBackground = bgthread; }
            get { return bgthread; }
        }

        public string Title="";



        // *****************************
        // 
        // *****************************




 


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.grpFadeMessage = new System.Windows.Forms.GroupBox();
            this.pnlFadingMessageSwitch = new System.Windows.Forms.Panel();
            this.rbFadeMessageOn = new System.Windows.Forms.RadioButton();
            this.rbFadeMessageOff = new System.Windows.Forms.RadioButton();
            this.lblFadeBGFade = new System.Windows.Forms.Label();
            this.btnFadeBGFinal = new System.Windows.Forms.Button();
            this.btnFadeBG = new System.Windows.Forms.Button();
            this.lblFadingBG = new System.Windows.Forms.Label();
            this.txtFadeBGFinal = new System.Windows.Forms.TextBox();
            this.lblFadingPortion = new System.Windows.Forms.Label();
            this.txtFadeBG = new System.Windows.Forms.TextBox();
            this.lblFadingShowtime = new System.Windows.Forms.Label();
            this.txtFadePortion = new System.Windows.Forms.TextBox();
            this.txtFadeShowtime = new System.Windows.Forms.TextBox();
            this.grpConsole = new System.Windows.Forms.GroupBox();
            this.pnlConsoleSwitch = new System.Windows.Forms.Panel();
            this.rbConsoleOn = new System.Windows.Forms.RadioButton();
            this.rbConsoleOff = new System.Windows.Forms.RadioButton();
            this.grpConsoleForm = new System.Windows.Forms.GroupBox();
            this.pnlConsoleFormSwitch = new System.Windows.Forms.Panel();
            this.rbConsoleFormOn = new System.Windows.Forms.RadioButton();
            this.rbConsoleFormOff = new System.Windows.Forms.RadioButton();
            this.grpMessageBox = new System.Windows.Forms.GroupBox();
            this.pnlMessageBoxSwitch = new System.Windows.Forms.Panel();
            this.rbMessageBoxOn = new System.Windows.Forms.RadioButton();
            this.rbMessageBoxOff = new System.Windows.Forms.RadioButton();
            this.grpTest = new System.Windows.Forms.GroupBox();
            this.chkThrowException = new System.Windows.Forms.CheckBox();
            this.grpOutputLevel = new System.Windows.Forms.GroupBox();
            this.rbOutVerbose = new System.Windows.Forms.RadioButton();
            this.rbOutInfo = new System.Windows.Forms.RadioButton();
            this.rbOutWarning = new System.Windows.Forms.RadioButton();
            this.rbOutError = new System.Windows.Forms.RadioButton();
            this.grpReportType = new System.Windows.Forms.GroupBox();
            this.rbInfo = new System.Windows.Forms.RadioButton();
            this.rbWarning = new System.Windows.Forms.RadioButton();
            this.rbError = new System.Windows.Forms.RadioButton();
            this.btnLaunch = new System.Windows.Forms.Button();
            this.grpMultipleReports = new System.Windows.Forms.GroupBox();
            this.chkMultiple = new System.Windows.Forms.CheckBox();
            this.txtNumLaunches = new System.Windows.Forms.TextBox();
            this.txtDelay = new System.Windows.Forms.TextBox();
            this.lblMultipleNum = new System.Windows.Forms.Label();
            this.lblDelay = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtExMessage = new System.Windows.Forms.TextBox();
            this.lblLocation = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.PnlStatus = new System.Windows.Forms.Panel();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.StatusLbl = new System.Windows.Forms.Label();
            this.grpFadeMessage.SuspendLayout();
            this.pnlFadingMessageSwitch.SuspendLayout();
            this.grpConsole.SuspendLayout();
            this.pnlConsoleSwitch.SuspendLayout();
            this.grpConsoleForm.SuspendLayout();
            this.pnlConsoleFormSwitch.SuspendLayout();
            this.grpMessageBox.SuspendLayout();
            this.pnlMessageBoxSwitch.SuspendLayout();
            this.grpTest.SuspendLayout();
            this.grpOutputLevel.SuspendLayout();
            this.grpReportType.SuspendLayout();
            this.grpMultipleReports.SuspendLayout();
            this.PnlStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpFadeMessage
            // 
            this.grpFadeMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFadeMessage.Controls.Add(this.pnlFadingMessageSwitch);
            this.grpFadeMessage.Controls.Add(this.lblFadeBGFade);
            this.grpFadeMessage.Controls.Add(this.btnFadeBGFinal);
            this.grpFadeMessage.Controls.Add(this.btnFadeBG);
            this.grpFadeMessage.Controls.Add(this.lblFadingBG);
            this.grpFadeMessage.Controls.Add(this.txtFadeBGFinal);
            this.grpFadeMessage.Controls.Add(this.lblFadingPortion);
            this.grpFadeMessage.Controls.Add(this.txtFadeBG);
            this.grpFadeMessage.Controls.Add(this.lblFadingShowtime);
            this.grpFadeMessage.Controls.Add(this.txtFadePortion);
            this.grpFadeMessage.Controls.Add(this.txtFadeShowtime);
            this.grpFadeMessage.Location = new System.Drawing.Point(5, 205);
            this.grpFadeMessage.Name = "grpFadeMessage";
            this.grpFadeMessage.Size = new System.Drawing.Size(984, 163);
            this.grpFadeMessage.TabIndex = 12;
            this.grpFadeMessage.TabStop = false;
            this.grpFadeMessage.Text = "Fading Message";
            // 
            // pnlFadingMessageSwitch
            // 
            this.pnlFadingMessageSwitch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFadingMessageSwitch.Controls.Add(this.rbFadeMessageOn);
            this.pnlFadingMessageSwitch.Controls.Add(this.rbFadeMessageOff);
            this.pnlFadingMessageSwitch.Location = new System.Drawing.Point(13, 19);
            this.pnlFadingMessageSwitch.Name = "pnlFadingMessageSwitch";
            this.pnlFadingMessageSwitch.Size = new System.Drawing.Size(92, 27);
            this.pnlFadingMessageSwitch.TabIndex = 0;
            // 
            // rbFadeMessageOn
            // 
            this.rbFadeMessageOn.AutoSize = true;
            this.rbFadeMessageOn.Checked = true;
            this.rbFadeMessageOn.Location = new System.Drawing.Point(3, 5);
            this.rbFadeMessageOn.Name = "rbFadeMessageOn";
            this.rbFadeMessageOn.Size = new System.Drawing.Size(39, 17);
            this.rbFadeMessageOn.TabIndex = 4;
            this.rbFadeMessageOn.TabStop = true;
            this.rbFadeMessageOn.Text = "On";
            this.rbFadeMessageOn.UseVisualStyleBackColor = true;
            this.rbFadeMessageOn.CheckedChanged += new System.EventHandler(this.rbFadingMessageOn_CheckedChanged);
            // 
            // rbFadeMessageOff
            // 
            this.rbFadeMessageOff.AutoSize = true;
            this.rbFadeMessageOff.Location = new System.Drawing.Point(48, 5);
            this.rbFadeMessageOff.Name = "rbFadeMessageOff";
            this.rbFadeMessageOff.Size = new System.Drawing.Size(39, 17);
            this.rbFadeMessageOff.TabIndex = 4;
            this.rbFadeMessageOff.Text = "Off";
            this.rbFadeMessageOff.UseVisualStyleBackColor = true;
            // 
            // lblFadeBGFade
            // 
            this.lblFadeBGFade.AutoSize = true;
            this.lblFadeBGFade.Location = new System.Drawing.Point(10, 133);
            this.lblFadeBGFade.Name = "lblFadeBGFade";
            this.lblFadeBGFade.Size = new System.Drawing.Size(92, 13);
            this.lblFadeBGFade.TabIndex = 2;
            this.lblFadeBGFade.Text = "BackGround Final";
            // 
            // btnFadeBGFinal
            // 
            this.btnFadeBGFinal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFadeBGFinal.Location = new System.Drawing.Point(179, 130);
            this.btnFadeBGFinal.Name = "btnFadeBGFinal";
            this.btnFadeBGFinal.Size = new System.Drawing.Size(27, 20);
            this.btnFadeBGFinal.TabIndex = 0;
            this.btnFadeBGFinal.Text = "...";
            this.btnFadeBGFinal.UseVisualStyleBackColor = true;
            this.btnFadeBGFinal.Click += new System.EventHandler(this.btnFadeBGFinal_Click);
            // 
            // btnFadeBG
            // 
            this.btnFadeBG.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFadeBG.Location = new System.Drawing.Point(179, 105);
            this.btnFadeBG.Name = "btnFadeBG";
            this.btnFadeBG.Size = new System.Drawing.Size(27, 20);
            this.btnFadeBG.TabIndex = 0;
            this.btnFadeBG.Text = "...";
            this.btnFadeBG.UseVisualStyleBackColor = true;
            this.btnFadeBG.Click += new System.EventHandler(this.btnFadeBG_Click);
            // 
            // lblFadingBG
            // 
            this.lblFadingBG.AutoSize = true;
            this.lblFadingBG.Location = new System.Drawing.Point(10, 107);
            this.lblFadingBG.Name = "lblFadingBG";
            this.lblFadingBG.Size = new System.Drawing.Size(67, 13);
            this.lblFadingBG.TabIndex = 2;
            this.lblFadingBG.Text = "BackGround";
            // 
            // txtFadeBGFinal
            // 
            this.txtFadeBGFinal.BackColor = System.Drawing.Color.Silver;
            this.txtFadeBGFinal.Enabled = false;
            this.txtFadeBGFinal.Location = new System.Drawing.Point(117, 130);
            this.txtFadeBGFinal.Name = "txtFadeBGFinal";
            this.txtFadeBGFinal.Size = new System.Drawing.Size(56, 20);
            this.txtFadeBGFinal.TabIndex = 1;
            this.txtFadeBGFinal.BackColorChanged += new System.EventHandler(this.txtFadeBGFinal_BackColorChanged);
            // 
            // lblFadingPortion
            // 
            this.lblFadingPortion.AutoSize = true;
            this.lblFadingPortion.Location = new System.Drawing.Point(10, 81);
            this.lblFadingPortion.Name = "lblFadingPortion";
            this.lblFadingPortion.Size = new System.Drawing.Size(99, 13);
            this.lblFadingPortion.TabIndex = 2;
            this.lblFadingPortion.Text = "Fading time portion:";
            // 
            // txtFadeBG
            // 
            this.txtFadeBG.BackColor = System.Drawing.Color.Gold;
            this.txtFadeBG.Enabled = false;
            this.txtFadeBG.Location = new System.Drawing.Point(117, 104);
            this.txtFadeBG.Name = "txtFadeBG";
            this.txtFadeBG.Size = new System.Drawing.Size(56, 20);
            this.txtFadeBG.TabIndex = 1;
            this.txtFadeBG.BackColorChanged += new System.EventHandler(this.txtFadingBG_BackColorChanged);
            // 
            // lblFadingShowtime
            // 
            this.lblFadingShowtime.AutoSize = true;
            this.lblFadingShowtime.Location = new System.Drawing.Point(10, 55);
            this.lblFadingShowtime.Name = "lblFadingShowtime";
            this.lblFadingShowtime.Size = new System.Drawing.Size(81, 13);
            this.lblFadingShowtime.TabIndex = 2;
            this.lblFadingShowtime.Text = "Show time (ms):";
            // 
            // txtFadePortion
            // 
            this.txtFadePortion.Location = new System.Drawing.Point(117, 78);
            this.txtFadePortion.Name = "txtFadePortion";
            this.txtFadePortion.Size = new System.Drawing.Size(56, 20);
            this.txtFadePortion.TabIndex = 1;
            this.txtFadePortion.Text = "0.6";
            this.txtFadePortion.TextChanged += new System.EventHandler(this.txtFadingPortion_TextChanged);
            // 
            // txtFadeShowtime
            // 
            this.txtFadeShowtime.Location = new System.Drawing.Point(117, 52);
            this.txtFadeShowtime.Name = "txtFadeShowtime";
            this.txtFadeShowtime.Size = new System.Drawing.Size(56, 20);
            this.txtFadeShowtime.TabIndex = 1;
            this.txtFadeShowtime.Text = "5000";
            this.txtFadeShowtime.TextChanged += new System.EventHandler(this.txtFadingShowtime_TextChanged);
            // 
            // grpConsole
            // 
            this.grpConsole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConsole.Controls.Add(this.pnlConsoleSwitch);
            this.grpConsole.Location = new System.Drawing.Point(8, 391);
            this.grpConsole.Name = "grpConsole";
            this.grpConsole.Size = new System.Drawing.Size(981, 57);
            this.grpConsole.TabIndex = 13;
            this.grpConsole.TabStop = false;
            this.grpConsole.Text = "Console";
            // 
            // pnlConsoleSwitch
            // 
            this.pnlConsoleSwitch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlConsoleSwitch.Controls.Add(this.rbConsoleOn);
            this.pnlConsoleSwitch.Controls.Add(this.rbConsoleOff);
            this.pnlConsoleSwitch.Location = new System.Drawing.Point(10, 19);
            this.pnlConsoleSwitch.Name = "pnlConsoleSwitch";
            this.pnlConsoleSwitch.Size = new System.Drawing.Size(92, 27);
            this.pnlConsoleSwitch.TabIndex = 0;
            // 
            // rbConsoleOn
            // 
            this.rbConsoleOn.AutoSize = true;
            this.rbConsoleOn.Checked = true;
            this.rbConsoleOn.Location = new System.Drawing.Point(3, 5);
            this.rbConsoleOn.Name = "rbConsoleOn";
            this.rbConsoleOn.Size = new System.Drawing.Size(39, 17);
            this.rbConsoleOn.TabIndex = 4;
            this.rbConsoleOn.TabStop = true;
            this.rbConsoleOn.Text = "On";
            this.rbConsoleOn.UseVisualStyleBackColor = true;
            this.rbConsoleOn.CheckedChanged += new System.EventHandler(this.rbConsoleOn_CheckedChanged);
            // 
            // rbConsoleOff
            // 
            this.rbConsoleOff.AutoSize = true;
            this.rbConsoleOff.Location = new System.Drawing.Point(48, 5);
            this.rbConsoleOff.Name = "rbConsoleOff";
            this.rbConsoleOff.Size = new System.Drawing.Size(39, 17);
            this.rbConsoleOff.TabIndex = 4;
            this.rbConsoleOff.Text = "Off";
            this.rbConsoleOff.UseVisualStyleBackColor = true;
            // 
            // grpConsoleForm
            // 
            this.grpConsoleForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConsoleForm.Controls.Add(this.pnlConsoleFormSwitch);
            this.grpConsoleForm.Location = new System.Drawing.Point(8, 91);
            this.grpConsoleForm.Name = "grpConsoleForm";
            this.grpConsoleForm.Size = new System.Drawing.Size(981, 48);
            this.grpConsoleForm.TabIndex = 14;
            this.grpConsoleForm.TabStop = false;
            this.grpConsoleForm.Text = "Console form";
            // 
            // pnlConsoleFormSwitch
            // 
            this.pnlConsoleFormSwitch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlConsoleFormSwitch.Controls.Add(this.rbConsoleFormOn);
            this.pnlConsoleFormSwitch.Controls.Add(this.rbConsoleFormOff);
            this.pnlConsoleFormSwitch.Location = new System.Drawing.Point(10, 15);
            this.pnlConsoleFormSwitch.Name = "pnlConsoleFormSwitch";
            this.pnlConsoleFormSwitch.Size = new System.Drawing.Size(92, 27);
            this.pnlConsoleFormSwitch.TabIndex = 0;
            // 
            // rbConsoleFormOn
            // 
            this.rbConsoleFormOn.AutoSize = true;
            this.rbConsoleFormOn.Checked = true;
            this.rbConsoleFormOn.Location = new System.Drawing.Point(3, 5);
            this.rbConsoleFormOn.Name = "rbConsoleFormOn";
            this.rbConsoleFormOn.Size = new System.Drawing.Size(39, 17);
            this.rbConsoleFormOn.TabIndex = 4;
            this.rbConsoleFormOn.TabStop = true;
            this.rbConsoleFormOn.Text = "On";
            this.rbConsoleFormOn.UseVisualStyleBackColor = true;
            this.rbConsoleFormOn.CheckedChanged += new System.EventHandler(this.rbConsoleFormOn_CheckedChanged);
            // 
            // rbConsoleFormOff
            // 
            this.rbConsoleFormOff.AutoSize = true;
            this.rbConsoleFormOff.Location = new System.Drawing.Point(48, 5);
            this.rbConsoleFormOff.Name = "rbConsoleFormOff";
            this.rbConsoleFormOff.Size = new System.Drawing.Size(39, 17);
            this.rbConsoleFormOff.TabIndex = 4;
            this.rbConsoleFormOff.Text = "Off";
            this.rbConsoleFormOff.UseVisualStyleBackColor = true;
            // 
            // grpMessageBox
            // 
            this.grpMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMessageBox.Controls.Add(this.pnlMessageBoxSwitch);
            this.grpMessageBox.Location = new System.Drawing.Point(5, 145);
            this.grpMessageBox.Name = "grpMessageBox";
            this.grpMessageBox.Size = new System.Drawing.Size(984, 54);
            this.grpMessageBox.TabIndex = 15;
            this.grpMessageBox.TabStop = false;
            this.grpMessageBox.Text = "Message Box";
            // 
            // pnlMessageBoxSwitch
            // 
            this.pnlMessageBoxSwitch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMessageBoxSwitch.Controls.Add(this.rbMessageBoxOn);
            this.pnlMessageBoxSwitch.Controls.Add(this.rbMessageBoxOff);
            this.pnlMessageBoxSwitch.Location = new System.Drawing.Point(13, 19);
            this.pnlMessageBoxSwitch.Name = "pnlMessageBoxSwitch";
            this.pnlMessageBoxSwitch.Size = new System.Drawing.Size(92, 27);
            this.pnlMessageBoxSwitch.TabIndex = 0;
            // 
            // rbMessageBoxOn
            // 
            this.rbMessageBoxOn.AutoSize = true;
            this.rbMessageBoxOn.Checked = true;
            this.rbMessageBoxOn.Location = new System.Drawing.Point(3, 5);
            this.rbMessageBoxOn.Name = "rbMessageBoxOn";
            this.rbMessageBoxOn.Size = new System.Drawing.Size(39, 17);
            this.rbMessageBoxOn.TabIndex = 4;
            this.rbMessageBoxOn.TabStop = true;
            this.rbMessageBoxOn.Text = "On";
            this.rbMessageBoxOn.UseVisualStyleBackColor = true;
            this.rbMessageBoxOn.CheckedChanged += new System.EventHandler(this.rbMessageBoxOn_CheckedChanged);
            // 
            // rbMessageBoxOff
            // 
            this.rbMessageBoxOff.AutoSize = true;
            this.rbMessageBoxOff.Location = new System.Drawing.Point(48, 5);
            this.rbMessageBoxOff.Name = "rbMessageBoxOff";
            this.rbMessageBoxOff.Size = new System.Drawing.Size(39, 17);
            this.rbMessageBoxOff.TabIndex = 4;
            this.rbMessageBoxOff.Text = "Off";
            this.rbMessageBoxOff.UseVisualStyleBackColor = true;
            // 
            // grpTest
            // 
            this.grpTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTest.Controls.Add(this.chkThrowException);
            this.grpTest.Controls.Add(this.grpOutputLevel);
            this.grpTest.Controls.Add(this.grpReportType);
            this.grpTest.Controls.Add(this.btnLaunch);
            this.grpTest.Controls.Add(this.grpMultipleReports);
            this.grpTest.Controls.Add(this.label1);
            this.grpTest.Controls.Add(this.lblMessage);
            this.grpTest.Controls.Add(this.txtExMessage);
            this.grpTest.Controls.Add(this.lblLocation);
            this.grpTest.Controls.Add(this.txtMessage);
            this.grpTest.Controls.Add(this.txtLocation);
            this.grpTest.Location = new System.Drawing.Point(8, 454);
            this.grpTest.Name = "grpTest";
            this.grpTest.Size = new System.Drawing.Size(981, 259);
            this.grpTest.TabIndex = 11;
            this.grpTest.TabStop = false;
            this.grpTest.Text = "Test Launch";
            // 
            // chkThrowException
            // 
            this.chkThrowException.AutoSize = true;
            this.chkThrowException.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.chkThrowException.Location = new System.Drawing.Point(663, 232);
            this.chkThrowException.Name = "chkThrowException";
            this.chkThrowException.Size = new System.Drawing.Size(157, 17);
            this.chkThrowException.TabIndex = 3;
            this.chkThrowException.Text = "Throw an internal exception";
            this.chkThrowException.UseVisualStyleBackColor = true;
            // 
            // grpOutputLevel
            // 
            this.grpOutputLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOutputLevel.Controls.Add(this.rbOutVerbose);
            this.grpOutputLevel.Controls.Add(this.rbOutInfo);
            this.grpOutputLevel.Controls.Add(this.rbOutWarning);
            this.grpOutputLevel.Controls.Add(this.rbOutError);
            this.grpOutputLevel.Location = new System.Drawing.Point(809, 136);
            this.grpOutputLevel.Name = "grpOutputLevel";
            this.grpOutputLevel.Size = new System.Drawing.Size(158, 90);
            this.grpOutputLevel.TabIndex = 7;
            this.grpOutputLevel.TabStop = false;
            this.grpOutputLevel.Text = "System output level";
            // 
            // rbOutVerbose
            // 
            this.rbOutVerbose.AutoSize = true;
            this.rbOutVerbose.Checked = true;
            this.rbOutVerbose.Location = new System.Drawing.Point(88, 65);
            this.rbOutVerbose.Name = "rbOutVerbose";
            this.rbOutVerbose.Size = new System.Drawing.Size(64, 17);
            this.rbOutVerbose.TabIndex = 4;
            this.rbOutVerbose.TabStop = true;
            this.rbOutVerbose.Text = "Verbose";
            this.rbOutVerbose.UseVisualStyleBackColor = true;
            // 
            // rbOutInfo
            // 
            this.rbOutInfo.AutoSize = true;
            this.rbOutInfo.Location = new System.Drawing.Point(17, 65);
            this.rbOutInfo.Name = "rbOutInfo";
            this.rbOutInfo.Size = new System.Drawing.Size(43, 17);
            this.rbOutInfo.TabIndex = 4;
            this.rbOutInfo.Text = "Info";
            this.rbOutInfo.UseVisualStyleBackColor = true;
            // 
            // rbOutWarning
            // 
            this.rbOutWarning.AutoSize = true;
            this.rbOutWarning.Location = new System.Drawing.Point(17, 42);
            this.rbOutWarning.Name = "rbOutWarning";
            this.rbOutWarning.Size = new System.Drawing.Size(65, 17);
            this.rbOutWarning.TabIndex = 4;
            this.rbOutWarning.Text = "Warning";
            this.rbOutWarning.UseVisualStyleBackColor = true;
            // 
            // rbOutError
            // 
            this.rbOutError.AutoSize = true;
            this.rbOutError.Location = new System.Drawing.Point(17, 19);
            this.rbOutError.Name = "rbOutError";
            this.rbOutError.Size = new System.Drawing.Size(47, 17);
            this.rbOutError.TabIndex = 4;
            this.rbOutError.Text = "Error";
            this.rbOutError.UseVisualStyleBackColor = true;
            // 
            // grpReportType
            // 
            this.grpReportType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpReportType.Controls.Add(this.rbInfo);
            this.grpReportType.Controls.Add(this.rbWarning);
            this.grpReportType.Controls.Add(this.rbError);
            this.grpReportType.Location = new System.Drawing.Point(663, 136);
            this.grpReportType.Name = "grpReportType";
            this.grpReportType.Size = new System.Drawing.Size(122, 90);
            this.grpReportType.TabIndex = 7;
            this.grpReportType.TabStop = false;
            this.grpReportType.Text = "Type of the report";
            // 
            // rbInfo
            // 
            this.rbInfo.AutoSize = true;
            this.rbInfo.Location = new System.Drawing.Point(17, 65);
            this.rbInfo.Name = "rbInfo";
            this.rbInfo.Size = new System.Drawing.Size(43, 17);
            this.rbInfo.TabIndex = 4;
            this.rbInfo.Text = "Info";
            this.rbInfo.UseVisualStyleBackColor = true;
            // 
            // rbWarning
            // 
            this.rbWarning.AutoSize = true;
            this.rbWarning.Location = new System.Drawing.Point(17, 42);
            this.rbWarning.Name = "rbWarning";
            this.rbWarning.Size = new System.Drawing.Size(65, 17);
            this.rbWarning.TabIndex = 4;
            this.rbWarning.Text = "Warning";
            this.rbWarning.UseVisualStyleBackColor = true;
            // 
            // rbError
            // 
            this.rbError.AutoSize = true;
            this.rbError.Checked = true;
            this.rbError.Location = new System.Drawing.Point(17, 19);
            this.rbError.Name = "rbError";
            this.rbError.Size = new System.Drawing.Size(47, 17);
            this.rbError.TabIndex = 4;
            this.rbError.TabStop = true;
            this.rbError.Text = "Error";
            this.rbError.UseVisualStyleBackColor = true;
            // 
            // btnLaunch
            // 
            this.btnLaunch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLaunch.Location = new System.Drawing.Point(484, 11);
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.Size = new System.Drawing.Size(151, 23);
            this.btnLaunch.TabIndex = 0;
            this.btnLaunch.Text = "&Launch the reports";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.btnLaunch_Click);
            // 
            // grpMultipleReports
            // 
            this.grpMultipleReports.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMultipleReports.Controls.Add(this.chkMultiple);
            this.grpMultipleReports.Controls.Add(this.txtNumLaunches);
            this.grpMultipleReports.Controls.Add(this.txtDelay);
            this.grpMultipleReports.Controls.Add(this.lblMultipleNum);
            this.grpMultipleReports.Controls.Add(this.lblDelay);
            this.grpMultipleReports.ForeColor = System.Drawing.Color.Blue;
            this.grpMultipleReports.Location = new System.Drawing.Point(663, 40);
            this.grpMultipleReports.Name = "grpMultipleReports";
            this.grpMultipleReports.Size = new System.Drawing.Size(304, 90);
            this.grpMultipleReports.TabIndex = 7;
            this.grpMultipleReports.TabStop = false;
            this.grpMultipleReports.Text = "Multiple reports";
            // 
            // chkMultiple
            // 
            this.chkMultiple.AutoSize = true;
            this.chkMultiple.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.chkMultiple.Location = new System.Drawing.Point(6, 19);
            this.chkMultiple.Name = "chkMultiple";
            this.chkMultiple.Size = new System.Drawing.Size(135, 17);
            this.chkMultiple.TabIndex = 3;
            this.chkMultiple.Text = "Launch &multiple reports";
            this.chkMultiple.UseVisualStyleBackColor = true;
            // 
            // txtNumLaunches
            // 
            this.txtNumLaunches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNumLaunches.Location = new System.Drawing.Point(191, 36);
            this.txtNumLaunches.Name = "txtNumLaunches";
            this.txtNumLaunches.Size = new System.Drawing.Size(94, 20);
            this.txtNumLaunches.TabIndex = 1;
            this.txtNumLaunches.Text = "10";
            // 
            // txtDelay
            // 
            this.txtDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDelay.Location = new System.Drawing.Point(191, 62);
            this.txtDelay.Name = "txtDelay";
            this.txtDelay.Size = new System.Drawing.Size(94, 20);
            this.txtDelay.TabIndex = 1;
            this.txtDelay.Text = "200";
            // 
            // lblMultipleNum
            // 
            this.lblMultipleNum.AutoSize = true;
            this.lblMultipleNum.Location = new System.Drawing.Point(36, 43);
            this.lblMultipleNum.Name = "lblMultipleNum";
            this.lblMultipleNum.Size = new System.Drawing.Size(105, 13);
            this.lblMultipleNum.TabIndex = 2;
            this.lblMultipleNum.Text = "Number of launches:";
            // 
            // lblDelay
            // 
            this.lblDelay.AutoSize = true;
            this.lblDelay.Location = new System.Drawing.Point(36, 65);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(149, 13);
            this.lblDelay.TabIndex = 2;
            this.lblDelay.Text = "Delay between launches (ms):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Internal exception message:";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(7, 63);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(115, 13);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "User defined message:";
            // 
            // txtExMessage
            // 
            this.txtExMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExMessage.Location = new System.Drawing.Point(4, 169);
            this.txtExMessage.Multiline = true;
            this.txtExMessage.Name = "txtExMessage";
            this.txtExMessage.Size = new System.Drawing.Size(637, 84);
            this.txtExMessage.TabIndex = 1;
            this.txtExMessage.Text = "Exception_Msg line 1\r\nException_Msg line 2";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(5, 24);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(117, 13);
            this.lblLocation.TabIndex = 2;
            this.lblLocation.Text = "User specified location:";
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Location = new System.Drawing.Point(8, 79);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(627, 62);
            this.txtMessage.TabIndex = 1;
            this.txtMessage.Text = "User_Message line 1\r\nUser_Message line 2";
            // 
            // txtLocation
            // 
            this.txtLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocation.Location = new System.Drawing.Point(8, 40);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(627, 20);
            this.txtLocation.TabIndex = 1;
            this.txtLocation.Text = "User_Location";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.lblDescription.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDescription.Location = new System.Drawing.Point(16, 45);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(389, 32);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "This panel is used to configure how error reports, warning reports, \r\ninfo boxes " +
                "and other kinds of reports are launched.";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblTitle.Location = new System.Drawing.Point(46, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(215, 24);
            this.lblTitle.TabIndex = 9;
            this.lblTitle.Text = "Reporter Configuration";
            // 
            // PnlStatus
            // 
            this.PnlStatus.AutoSize = true;
            this.PnlStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PnlStatus.Controls.Add(this.CancelBtn);
            this.PnlStatus.Controls.Add(this.StatusLbl);
            this.PnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PnlStatus.Location = new System.Drawing.Point(4, 719);
            this.PnlStatus.Name = "PnlStatus";
            this.PnlStatus.Size = new System.Drawing.Size(985, 30);
            this.PnlStatus.TabIndex = 10;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.AutoSize = true;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CancelBtn.Location = new System.Drawing.Point(926, 5);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(5);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(50, 24);
            this.CancelBtn.TabIndex = 2;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // StatusLbl
            // 
            this.StatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusLbl.AutoSize = true;
            this.StatusLbl.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLbl.Location = new System.Drawing.Point(5, 10);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(213, 15);
            this.StatusLbl.TabIndex = 3;
            this.StatusLbl.Text = "< Drag to move, Ctrl-right-click to close >";
            // 
            // ReporterConf
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(993, 753);
            this.Controls.Add(this.grpFadeMessage);
            this.Controls.Add(this.grpConsole);
            this.Controls.Add(this.grpConsoleForm);
            this.Controls.Add(this.grpMessageBox);
            this.Controls.Add(this.grpTest);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.PnlStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimumSize = new System.Drawing.Size(350, 400);
            this.Name = "ReporterConf";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Text = "Reporter configuration utility";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ConsoleForm_Load);
            this.Disposed += new System.EventHandler(this.ConsoleForm_Dispose);
            this.grpFadeMessage.ResumeLayout(false);
            this.grpFadeMessage.PerformLayout();
            this.pnlFadingMessageSwitch.ResumeLayout(false);
            this.pnlFadingMessageSwitch.PerformLayout();
            this.grpConsole.ResumeLayout(false);
            this.pnlConsoleSwitch.ResumeLayout(false);
            this.pnlConsoleSwitch.PerformLayout();
            this.grpConsoleForm.ResumeLayout(false);
            this.pnlConsoleFormSwitch.ResumeLayout(false);
            this.pnlConsoleFormSwitch.PerformLayout();
            this.grpMessageBox.ResumeLayout(false);
            this.pnlMessageBoxSwitch.ResumeLayout(false);
            this.pnlMessageBoxSwitch.PerformLayout();
            this.grpTest.ResumeLayout(false);
            this.grpTest.PerformLayout();
            this.grpOutputLevel.ResumeLayout(false);
            this.grpOutputLevel.PerformLayout();
            this.grpReportType.ResumeLayout(false);
            this.grpReportType.PerformLayout();
            this.grpMultipleReports.ResumeLayout(false);
            this.grpMultipleReports.PerformLayout();
            this.PnlStatus.ResumeLayout(false);
            this.PnlStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion



        private Thread formthread = null;

        [STAThread]
        private void FormThreadFunc()
        /// Shows the window in a separate thread.
        // $A Igor Jul08; 
        {
            try
            {
                this.ShowDialog();
            }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }



        private void ConsoleForm_Load(object sender, System.EventArgs args)
        // $A Igor Jul08; 
        {
            try
            {
                // Set common event handlers recursively (move the consform with a mouse & close with button 3):
                UtilForms.RecursiveControlDelegate(this, new ControlDelegate(SetCommonEvents));
            }
            catch (Exception ex) 
            {
                ReporterForms.Global.ReportError("ReporterConf", null, ex);
            }
            finally
            {
                try
                {
                    // Set the state of the configure console according to the state of the reporter 
                    // that is being configured:
                    rbConsoleFormOn.Checked = reporter.UseConsoleForm;
                    rbFadeMessageOn.Checked = reporter.UseFadeMessage;
                    rbMessageBoxOn.Checked = reporter.UseMessageBox;
                    rbConsoleOn.Checked = reporter.UseConsole;
                    // Switch complementary buttons:
                    rbConsoleFormOff.Checked = !rbConsoleFormOn.Checked;
                    rbFadeMessageOff.Checked = !rbFadeMessageOn.Checked;
                    rbMessageBoxOff.Checked = !rbMessageBoxOn.Checked;
                    rbConsoleOff.Checked = !rbConsoleOn.Checked;

                    // Settings for fadingmessage:
                    txtFadeShowtime.Text = reporter.FadeMessageShowtime.ToString();
                    txtFadePortion.Text = reporter.FadeMessageFadingTimePortion.ToString();
                    // txtFadePortion.Text = reporter.FadeMessageFadingTimePortion.ToString(); 
                    txtFadeBG.BackColor = reporter.FadeMessageBackColor;
                    txtFadeBGFinal.BackColor = reporter.FadeMessageBackColorFinal;
                }
                catch (Exception e) { ReporterForms.Global.ReportError(e, "\nError inside ReporterConf.Load"); }
            }
		} 


        private void ConsoleForm_Dispose(object sender, System.EventArgs e)
        // $A Igor Jul08; 
        {
            try 
            {
            }
            catch (Exception ex) {  ReporterForms.Global.ReportError("ReporterConf", null, ex);  }
        }


        private void CloseBtn_Click(object sender, EventArgs e)
        // $A Igor Jul08; 
        {
            try
            {
                CloseForm();
            }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }

        private void HideBtn_Click(object sender, EventArgs e)
        // $A Igor Jul08; 
        {
            try
            {
                HideForm();
            }
            catch (Exception ex) {  ReporterForms.Global.ReportError("ReporterConf", null, ex);  }
        }




        // Public methods:

        public void HideForm()
        // $A Igor Jul08; 
        {
            try
            {
            this.Visible = false;
            }
            catch (Exception ex) {  ReporterForms.Global.ReportError("ReporterConf", null, ex);  }
        }


        public void CloseForm()
        /// Closes the consform by properly (i.e. thread-safe) calling the Close() and Dispose().
        // $A Igor Jul08; 
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
                    // this.IsReady = false;
                    // Call appropriate system methods to close the consform:
                    this.Close();
                    if (!this.IsDisposed)
                        this.Dispose();
                    //if (formthread!=null)
                    //    if (formthread.IsAlive)
                    //        formthread.Abort();
                }
            }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex);  }
        }


        public void ShowThread()
        /// Shows a fading message in a new thread.
        // $A Igor Oct08; 
        {
            formthread = new Thread(new ThreadStart(FormThreadFunc));
            formthread.IsBackground = bgthread;
            formthread.Start();
        }

        public void ShowThread(string title)
        /// Shows a fading message in a new thread, with message text equal to mshtext and without a title;
        // $A Igor Oct08; 
        {
            if (title != null) if (title.Length > 0) Title = title;
            ShowThread();
        }



        #region Mouse_Control

        // Event handlers FadeMessage_MouseMoveand ReportConfig_MouseDown enable dragging of messagebox
        // without a frame.
        int m_PrevX;
        int m_PrevY;
        private void ReportConfig_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            Left = Left + (e.X - m_PrevX);
            Top = Top + (e.Y - m_PrevY);
        }

        private void ReportConfig_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            m_PrevX = e.X;
            m_PrevY = e.Y;
        }


        private void ReportConfig_MouseClick(object sender, MouseEventArgs e)
        /// Enables closing the consform with right-clicking
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


        void SetCommonEvents(Control f)
        // Sets common events for the consform and its sub-controls
        {
            {
                try
                {
                    // This will enable to kill the windowby clicking the mouse button 3
                    f.MouseClick += new MouseEventHandler(this.ReportConfig_MouseClick);
                }
                catch (Exception e) { Exception ee = e; }
                try
                {
                    // This will allow dragging the window by mouse:
                    f.MouseDown += new MouseEventHandler(this.ReportConfig_MouseDown);
                    f.MouseMove += new MouseEventHandler(this.ReportConfig_MouseMove);
                }
                catch (Exception e) { Exception ee = e; }
            }
        }




        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }


        #endregion  // Mouse_Control

        #region GUI

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            try
            {
                if (reporter == null)
                    throw new Exception("Reporter to be configured is not specified.");
                ReportType type=ReportType.Error;
                if (rbError.Checked)
                    type=ReportType.Error;
                else if (rbWarning.Checked)
                    type=ReportType.Warning;
                else if (rbInfo.Checked)
                    type=ReportType.Info;
                ReportLevel level = ReportLevel.Info;
                if (rbOutError.Checked)
                    level = ReportLevel.Error;
                else if (rbOutWarning.Checked)
                    level = ReportLevel.Warning;
                else if (rbOutInfo.Checked)
                    level = ReportLevel.Info;
                else if (rbOutVerbose.Checked)
                    level = ReportLevel.Verbose;

                // txtMessage, txtLocation, txtExMessage ;

                string 
                    location = txtLocation.Text,
                    message = txtMessage.Text;
                Exception exrep = null;
                if (!string.IsNullOrEmpty(txtExMessage.Text))
                    exrep = new Exception(txtExMessage.Text);

                // Set the appropriate things on the reporter:
                reporter.ReportingLevel = level;
                if (chkThrowException.Checked)
                    reporter.ThrowTestException = chkThrowException.Checked;

                // Launch error report(result):
                int numlaunches;
                if (chkMultiple.Checked)
                    numlaunches = int.Parse(txtNumLaunches.Text);
                else
                    numlaunches=1;
                for (int i = 1; i <= numlaunches; ++i)
                {
                    string loc;
                    if (chkMultiple.Checked)
                        loc = location + "_" + i.ToString();
                    else
                        loc = location;
                    reporter.Report(type, loc, message, exrep);
                    if (chkMultiple.Checked)
                        Thread.Sleep(int.Parse(txtDelay.Text));
                }
            }
            catch (Exception ex) {  ReporterForms.Global.ReportError("ReporterConf", null, ex);  }
        }


        #region GUI_ConsoleForm

        private void rbConsoleFormOn_CheckedChanged(object sender, EventArgs e)
        {
            try { reporter.UseConsoleForm = rbConsoleFormOn.Checked; }
            catch (Exception ex) {  ReporterForms.Global.ReportError("ReporterConf", null, ex);  }
        }

        #endregion   //  GUI_ConsoleForm


        #region GUI_MessageBox

        private void rbMessageBoxOn_CheckedChanged(object sender, EventArgs e)
        {
            try { reporter.UseMessageBox = rbMessageBoxOn.Checked; }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }

        #endregion   //  GUI_MessageBox


        #region GUI_FadeMessage

        private void rbFadingMessageOn_CheckedChanged(object sender, EventArgs e)
        {
            try { reporter.UseFadeMessage = rbFadeMessageOn.Checked; }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }

        private void txtFadingShowtime_TextChanged(object sender, EventArgs e)
        {
            try { reporter.FadeMessageShowtime = int.Parse(txtFadeShowtime.Text); }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }

        private void txtFadingPortion_TextChanged(object sender, EventArgs e)
        {
            try { reporter.FadeMessageFadingTimePortion = double.Parse(txtFadePortion.Text); }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }

        // Background color specification:
        private void txtFadingBG_BackColorChanged(object sender, EventArgs e)
        {
            try { reporter.FadeMessageBackColor = txtFadeBG.BackColor;  }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }

        private void btnFadeBG_Click(object sender, EventArgs e)
        {
            try
            {
                ColorDialog colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() != DialogResult.Cancel)
                {
                    txtFadeBG.BackColor = colorDialog.Color;
                }
            }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }

        // Final background color specification:
        private void txtFadeBGFinal_BackColorChanged(object sender, EventArgs e)
        {
            try { reporter.FadeMessageBackColorFinal = txtFadeBGFinal.BackColor; }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }

        private void btnFadeBGFinal_Click(object sender, EventArgs e)
        {
            try
            {
                ColorDialog colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() != DialogResult.Cancel)
                {
                    txtFadeBGFinal.BackColor = colorDialog.Color;
                }
            }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }
        }



        #endregion  // GUI_FadeMessage


        #region GUI_Console

        private void rbConsoleOn_CheckedChanged(object sender, EventArgs e)
        {
            try { reporter.UseConsole = rbConsoleOn.Checked; }
            catch (Exception ex) { ReporterForms.Global.ReportError("ReporterConf", null, ex); }

        }

        #endregion   //  GUI_Console




        #endregion  // GUI






    }  // Class ConsoleForm
}
