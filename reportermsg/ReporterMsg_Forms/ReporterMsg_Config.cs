using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

using IG.Lib;


namespace IG.Forms
{
	
    /// <summary>Reporter configuration.</summary>
	public class ReporterConf : System.Windows.Forms.Form
    //$A Igor Oct08;
    {

        #region Initialization

        private ReporterConf() { }  // prevent calling argument-less constructor



        public ReporterConf(ReporterBase rf)
        {
            SpeechVisible = false;  // speech settings sub-panel will not be visible
            InitializeComponent();
            CurrentReporter = rf;
            if (CurrentReporter == null)
                throw new ArgumentException("Forms Reporter to be configured is not specified (null reference).");
        }

        #endregion  // Initialization

        #region ReporterData

        ReporterBase _r = null;
        private GroupBox GrpTextWriter;
        private CheckBox chkTextWriterAppend;
        private Panel pnlTextWriter;
        private RadioButton rbTextWriterOn;
        private RadioButton rbTextWriterOff;
        private TextBox txtTextWriter;
        private Label lblTextWriter;
        private NumericUpDown txtLogIndentSpacing;
        private Label lblLogIndentIncrement;
        private NumericUpDown txtLogIndentInitial;
        private Label lblLogIndentInitial;
        private CheckBox ChkTraceVerbose;
        private CheckBox chkLogVerbose;
        private Label lblTextWriterNum;
        private Label lblLogNum;
        private Button btnTextWriter;
        private Button btnLogFile;
        private GroupBox grpSpeech;
        private Panel pnlSpeechSwitch;
        private RadioButton rbSpeechOn;
        private RadioButton rbSpeechOff;
        private ComboBox cmbSpeechLevelSignal;
        private ComboBox cmbSpeechLevelMessage;
        private Label lblSpeechLevelSignal;
        private Label lblSpeechLevelMessage;

        /// <summary>Gets or sets the reporter for reporting internal errors of this class.
        /// This is different from the reporting that is being tested.</summary>
        public ReporterBase R
        {
            get  
            {
                // With this statement, we separate the internal reporter from the one that is being tested / configured:
                if (_r == null)
                    _r = new ReporterConsoleMsgbox();
                return _r;
            }
            set { _r = value; }
        }

        public enum enReporterType { Basic, Console, Msgbox, ConsoleMsgbox }

        public enReporterType ReporterType;

        /// <summary>Applies the reporter type as specified by the form's state.</summary>
        protected virtual void ApplyGuiReporterType()
        {
            if (rbBasic.Checked)
                ReporterType = enReporterType.Basic;
            else if (rbConsole.Checked)
                ReporterType = enReporterType.Console;
            else if (rbMessageBox.Checked)
                ReporterType = enReporterType.Msgbox;
            else if (rbConsoleMessageBox.Checked)
                ReporterType = enReporterType.ConsoleMsgbox;
            else
                ReporterType = enReporterType.ConsoleMsgbox;
        }

        /// <summary>Sets the current reporter of this calss according to the reporter type.</summary>
        /// <param name="type">Specifies what type of reporter should be set.</param>
        protected virtual void SetReporter(enReporterType type)
        {
            switch (type)
            {
                case enReporterType.Basic:
                    _reporter = ReporterBase.Global;
                    break;
                case enReporterType.Console:
                    _reporter = ReporterConsole.Global;
                    break;
                case enReporterType.Msgbox:
                    _reporter = ReporterMsgbox.Global;
                    break;
                case enReporterType.ConsoleMsgbox:
                    // $$ Exclusion of speech library at this point
                    // _reporter = ReporterConsoleMsgboxSpeech.Global;
                    _reporter = ReporterConsoleMsgbox.Global;
                    break;
                default:
                    // $$ Exclusion of speech library at this point
                    // _reporter = ReporterConsoleMsgboxSpeech.Global;
                    _reporter = ReporterConsoleMsgbox.Global;
                    break;
            }
        }

        /// <summary>Sets the current reporter of this calss according to the private variable ReporterType.</summary>
        protected virtual void SetReporter()
        {
            ApplyGuiReporterType();
            SetReporter(ReporterType);
        }


        // Auxiliary data to check whether a file has already been set:
        static IReporter reptw = null, reptl = null;
        static string filetw = null, filetl = null;

        /// <summary>Applies the settings of the user interface.</summary>
        protected void ApplyGuiSettings()
        {
            try
            {
                ApplyGuiReporterType();
                SetReporter();
                IReporter rep = CurrentReporter;
                if (rep == null)
                    throw new NullReferenceException("The current reporter can not be located.");
                try
                {
                    // Check whether the current reporter supports reporting via console:
                    IReporterConsole repConsole = rep as IReporterConsole;
                    if (repConsole == null)
                    {
                        if (rbConsoleOn.Checked)
                        {
                            // Reportinf via console is required, but the current reporter does not support it:
                            rbConsoleOn.Checked = false; rbConsoleOff.Checked = true;
                            throw new InvalidOperationException("The current reporter does not support reporting via console. GUI state has been modified appropriately.");
                        }
                    } else
                    {
                        // Reporting via console is supported:
                        if (rbConsoleOn.Checked)
                        {
                            // Reporting via console is required, adjust the reporter state:
                            repConsole.UseConsole = true;
                        }  else
                            repConsole.UseConsole = false;  // reporting in this way not required
                    }
                }
                catch (Exception ex1)
                {
                    R.ReportWarning("Console-related settings" /* location */, ex1);
                }
                try
                {
                    // Check whether the current reporter supports reporting via message box:
                    IReporterMessageBox repMessageBox = rep as IReporterMessageBox;
                    if (repMessageBox == null)
                    {
                        if (rbMessageBoxOn.Checked)
                        {
                            // Reporting via message box is required, but the current reporter does not support it:
                            rbMessageBoxOn.Checked = false; rbMessageBoxOff.Checked = true;
                            throw new InvalidOperationException("The current reporter does not support reporting via message box. GUI state has been modified appropriately.");
                        }
                    }
                    else
                    {
                        // Reporting via meassage boxes is supported:
                        if (rbMessageBoxOn.Checked)
                        {
                            // Reporting via message box is required, adjust the reporter state:
                            repMessageBox.UseMessageBox = true;
                        }
                        else
                            repMessageBox.UseMessageBox = false;  // reporting in this way not required
                    }
                }
                catch (Exception ex1)
                {
                    R.ReportWarning("Message box-related settings" /* location */, ex1);
                }
                try
                {
                    // Check whether the current reporter supports tracing TextWriters:
                    IReporterTextWriter repTextWriter = rep as IReporterTextWriter;
                    if (repTextWriter == null)
                    {
                        if (rbTextWriterOn.Checked)
                        {
                            // Reporting via text file is required, but the current reporter does not support it:
                            rbTextWriterOn.Checked = false; rbTextWriterOff.Checked = true;
                            throw new InvalidOperationException("The current reporter does not support reporting via TextWriters. GUI state has been modified appropriately.");
                        }
                    }
                    else
                    {
                        // Reporting via TextWriter is supported:
                        if (rbTextWriterOn.Checked)
                        {
                            // Reporting via text file is required, adjust the reporter state:
                            repTextWriter.UseTextWriter = true;
                            if (reptw != rep || filetw != txtTextWriter.Text)
                            {
                                // Required outpur file has not yet been set for rhe current reporter, remember data and set it:
                                reptw = rep;
                                filetw = txtTextWriter.Text;
                                repTextWriter.SetTextWriter(System.Environment.ExpandEnvironmentVariables(txtTextWriter.Text),
                                    chkTextWriterAppend.Checked);
                            }
                            lblTextWriterNum.Text = "Number of text writers: " + repTextWriter.TextWriterNumWriters();
                        }
                        else
                            repTextWriter.UseTextWriter = false;  // reporting in this way not required
                    }
                }
                catch (Exception ex1)
                {
                    R.ReportWarning("Text file reporting related settings" /* location */, ex1);
                }
                try
                {
                    // Check whether the current reporter supports logging to files:
                    IReporterTextLogger repTextLogger = rep as IReporterTextLogger;
                    if (repTextLogger == null)
                    {
                        if (rbLogOn.Checked)
                        {
                            // Reporting via log file is required, but the current reporter does not support it:
                            rbLogOn.Checked = false; rbLogOff.Checked = true;
                            throw new InvalidOperationException("The current reporter does not support logging. GUI state has been modified appropriately.");
                        }
                    }
                    else
                    {
                        // Reporting via logging is supported:
                        if (rbLogOn.Checked)
                        {
                            // Reporting via log file is required, adjust the reporter state:
                            repTextLogger.UseTextLogger = true;
                            repTextLogger.TextLoggerIndentInitial = int.Parse(txtLogIndentInitial.Text);
                            repTextLogger.TextLoggerIndentSpacing = int.Parse(txtLogIndentSpacing.Text);
                            if (reptl != rep || filetl != txtLogFile.Text)
                            {
                                // Required outpur file has not yet been set for the current reporter, remember data and set it:
                                reptl = rep;
                                filetl = txtLogFile.Text;
                                repTextLogger.SetTextLogger(System.Environment.ExpandEnvironmentVariables(txtLogFile.Text),
                                chkLogAppend.Checked);
                            }
                            lblLogNum.Text = "Number of text writers: " + repTextLogger.TextLoggerNumWriters();
                        }
                        else
                            repTextLogger.UseTextLogger = false;  // reporting in this way not required
                    }
                }
                catch (Exception ex1)
                {
                    R.ReportWarning("Text file logging related settings" /* location */, ex1);
                }
                try
                {
                    // Check whether the current reporter supports tracing:
                    IReporterTrace repTrace = rep as IReporterTrace;
                    if (repTrace == null)
                    {
                        if (rbTraceOn.Checked)
                        {
                            // Reporting via trace mechanism is required, but the current reporter does not support it:
                            rbTraceOn.Checked = false; rbTraceOff.Checked = true;
                            throw new InvalidOperationException("The current reporter does not support tracing. GUI state has been modified appropriately.");
                        }
                    }  else
                    {
                        // Reporting via trace mechanism is supported:
                        if (rbTraceOn.Checked)
                        {
                            // Reporting via trace mechanism is required, adjust the reporter state:
                            repTrace.UseTrace = true;
                            if (chkTraceToConsole.Checked)
                                SetTraceConsole(true);
                            else
                                SetTraceConsole(false);
                        }
                        else
                            repTrace.UseTrace = false;  // reporting in this way not required
                    }
                }
                catch (Exception ex1)
                {
                    R.ReportWarning("Tracing related settings" /* location */, ex1);
                }

                try
                {
                    // Check whether the current reporter supports speech:
                    IReporterSpeech repSpeech = rep as IReporterSpeech;
                    if (repSpeech == null)
                    {
                        if (rbSpeechOn.Checked)
                        {
                            // Reporting via speech is required, but the current reporter does not support it:
                            rbSpeechOn.Checked = false; rbSpeechOff.Checked = true;
                            throw new InvalidOperationException("The current reporter does not support speech. GUI state has been modified appropriately.");
                        }
                    }
                    else
                    {
                        // Reporting via speech is supported:
                        if (rbSpeechOn.Checked)
                        {
                            // Reporting via speech mechanism is required, adjust the reporter state:
                            repSpeech.UseSpeech = true;
                            switch (cmbSpeechLevelSignal.SelectedIndex)
                            {
                                case 0:
                                    repSpeech.SpeechLevelSignal = ReportLevel.Off;
                                    break;
                                case 1:
                                    repSpeech.SpeechLevelSignal = ReportLevel.Error;
                                    break;
                                case 2:
                                    repSpeech.SpeechLevelSignal = ReportLevel.Warning;
                                    break;
                                case 3:
                                    repSpeech.SpeechLevelSignal = ReportLevel.Info;
                                    break;
                                case 4:
                                    repSpeech.SpeechLevelSignal = ReportLevel.Verbose;
                                    break;
                                default:
                                    repSpeech.SpeechLevelSignal = ReportLevel.Warning;
                                    break;
                            }
                            switch (cmbSpeechLevelMessage.SelectedIndex)
                            {
                                case 0:
                                    repSpeech.SpeechLevelMessage = ReportLevel.Off;
                                    break;
                                case 1:
                                    repSpeech.SpeechLevelMessage = ReportLevel.Error;
                                    break;
                                case 2:
                                    repSpeech.SpeechLevelMessage = ReportLevel.Warning;
                                    break;
                                case 3:
                                    repSpeech.SpeechLevelMessage = ReportLevel.Info;
                                    break;
                                case 4:
                                    repSpeech.SpeechLevelMessage = ReportLevel.Verbose;
                                    break;
                                default:
                                    repSpeech.SpeechLevelMessage = ReportLevel.Off;
                                    break;
                            }
                        }
                        else
                            repSpeech.UseSpeech = false;  // reporting in this way not required
                    }
                }
                catch (Exception ex1)
                {
                    R.ReportWarning("Speech related settings" /* location */, ex1);
                }


                // Trace Trace Trace


            }
            catch (Exception ex)
            {
                R.ReportError("ReporterConf.ApplyGUISettings", ex);
            }
        }



        /// <summary>A stream that redirects its input to the console.</summary>
        protected class ConsoleWritingStream : Stream
        {

            /// <summary>Redirects output to the system's console.</summary>
            public override void Write(byte[] array, int offset, int count)
            {
                string instr = System.Text.UnicodeEncoding.Unicode.GetString(array, offset, count);
                string outstr = "Trace >" + instr;
                Console.WriteLine(outstr);
            }

            // Implementation of the Stream's abstract properties & methods:

            public override bool CanRead { get { return false; } }  // the stream will be used only for writing

            public override bool CanSeek { get { return false; } }

            public override bool CanWrite { get { return true; } }


            public override long Position
            {
                get { throw new NotImplementedException("ConsoleWritingStream.Position.get not implemented."); }
                set { throw new NotImplementedException("ConsoleWritingStream.Position.set not implemented."); }
            }

            public override long Length
            {
                get { throw new NotImplementedException("ConsoleWritingStream.Length not implemented."); }
            }

            public override void Flush() { throw new NotImplementedException("ConsoleWritingStream.Flush not implemented."); }

            public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException("ConsoleWritingStream.Seek not implemented."); }

            public override void SetLength(long value) { throw new NotImplementedException("ConsoleWritingStream.SetLength not implemented."); }

            public override int Read(byte[] buffer, int offset, int count) { throw new NotImplementedException("ConsoleWritingStream.read not implemented."); }
        }


        private TextWriterTraceListener ConsoleListener = null;

        /// <summary>Installs or removes the trace listener that echoes the trace output to a console.</summary>
        /// <param name="doset">If true then the appropriate trace listener is added, else it is removed.</param>
        void SetTraceConsole(bool doset)
        {
            try
            {
                // Remove the listener first if it is already installed:
                Trace.Listeners.Remove(ConsoleListener);
            }
            catch { }
            if (doset)
            {
                if (ConsoleListener == null)
                    ConsoleListener = new TextWriterTraceListener( new ConsoleWritingStream() );
                try
                {
                    Trace.Listeners.Add(ConsoleListener);
                }
                catch (Exception ex1)
                {
                    R.ReportError("Trace listener for console output.",ex1);
                }
            }
        }


        
        /// <summary>Gets or sets the current reporter.</summary>
        protected ReporterBase CurrentReporter
        {
            get
            {
                if (_reporter == null)
                    ApplyGuiSettings();
                return _reporter;
            }
            set { _reporter = value; }
        }

        protected string[] reporters = new string[] {"Basic Reporter","Forms Reporter"};



        protected ReporterBase _reporter = null;

        #endregion  // ReporterData


        #region auxiliary_definitions

        delegate void VoidDelegate();  /// Reference to a function without arguments & return value.
        public delegate void FormDelegate(Form f);    // Reference to a function with a Form argument.
        public delegate void ControlDelegate(Control ct);    // Reference to a function with a Control argument.

        private void RecursiveControlDelegate(Control frm, ControlDelegate fd)
        // Recursively executes a dlegate of type (vod(Control)) on frm and all its children.
        {
            try
            {
                foreach (Control child in frm.Controls)
                {
                    try { RecursiveControlDelegate(child, fd); }
                    catch (Exception e) { Exception ee = e; }
                }
                // After executing the delegate on all children of the consform, execute it on the consform itself:
                if (frm.InvokeRequired)
                {
                    frm.Invoke(fd, new Object[] { frm });
                }
                else
                    fd(frm);
            }
            catch (Exception e)
            {
                R.ReportError(e);
            }
        }

        void ReportNonimplemented(string method, Exception ex)
        // Launching report in the case when a CurrentReporter is used that doesn't implement the requested functionality.
        // $A Igor jan09;
        {
            R.ReportError(method, "Funcionality not implemented, please choose a different reporter.", ex);
        }


        #endregion  // auxiliary_definitions


        #region Control_Definitions

        private GroupBox grpReporters;
        private Panel pnlReporters;
        private RadioButton rbBasic;
        private RadioButton rbConsole;
        private RadioButton rbConsoleMessageBox;
        private RadioButton rbMessageBox;
        private GroupBox grpLog;
        private Panel pnlLogFile;
        private RadioButton rbLogOn;
        private RadioButton rbLogOff;
        private GroupBox grpTrace;
        private Panel pnlTrace;
        private RadioButton rbTraceOn;
        private RadioButton rbTraceOff;
        private CheckBox chkTraceToConsole;
        private CheckBox chkLogAppend;
        private TextBox txtLogFile;
        private Label lblLogFile;

        private bool bgthread = false;
        private GroupBox grpConsole;
        private Panel pnlConsoleSwitch;
        private RadioButton rbConsoleOn;
        private RadioButton rbConsoleOff;
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

        private CheckBox chkThrowException;

        #endregion  // Control_Definitions


        public bool IsBackground
        // $A Igor Jul08; 
        {
            set { bgthread = value; if (formthread != null) formthread.IsBackground = bgthread; }
            get { return bgthread; }
        }


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.grpConsole = new System.Windows.Forms.GroupBox();
            this.pnlConsoleSwitch = new System.Windows.Forms.Panel();
            this.rbConsoleOn = new System.Windows.Forms.RadioButton();
            this.rbConsoleOff = new System.Windows.Forms.RadioButton();
            this.grpMessageBox = new System.Windows.Forms.GroupBox();
            this.pnlMessageBoxSwitch = new System.Windows.Forms.Panel();
            this.rbMessageBoxOn = new System.Windows.Forms.RadioButton();
            this.rbMessageBoxOff = new System.Windows.Forms.RadioButton();
            this.grpTest = new System.Windows.Forms.GroupBox();
            this.chkLogVerbose = new System.Windows.Forms.CheckBox();
            this.ChkTraceVerbose = new System.Windows.Forms.CheckBox();
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
            this.grpReporters = new System.Windows.Forms.GroupBox();
            this.pnlReporters = new System.Windows.Forms.Panel();
            this.rbBasic = new System.Windows.Forms.RadioButton();
            this.rbConsoleMessageBox = new System.Windows.Forms.RadioButton();
            this.rbMessageBox = new System.Windows.Forms.RadioButton();
            this.rbConsole = new System.Windows.Forms.RadioButton();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.btnLogFile = new System.Windows.Forms.Button();
            this.lblLogNum = new System.Windows.Forms.Label();
            this.txtLogIndentInitial = new System.Windows.Forms.NumericUpDown();
            this.txtLogIndentSpacing = new System.Windows.Forms.NumericUpDown();
            this.chkLogAppend = new System.Windows.Forms.CheckBox();
            this.pnlLogFile = new System.Windows.Forms.Panel();
            this.rbLogOn = new System.Windows.Forms.RadioButton();
            this.rbLogOff = new System.Windows.Forms.RadioButton();
            this.txtLogFile = new System.Windows.Forms.TextBox();
            this.lblLogIndentInitial = new System.Windows.Forms.Label();
            this.lblLogIndentIncrement = new System.Windows.Forms.Label();
            this.lblLogFile = new System.Windows.Forms.Label();
            this.grpTrace = new System.Windows.Forms.GroupBox();
            this.chkTraceToConsole = new System.Windows.Forms.CheckBox();
            this.pnlTrace = new System.Windows.Forms.Panel();
            this.rbTraceOn = new System.Windows.Forms.RadioButton();
            this.rbTraceOff = new System.Windows.Forms.RadioButton();
            this.GrpTextWriter = new System.Windows.Forms.GroupBox();
            this.btnTextWriter = new System.Windows.Forms.Button();
            this.lblTextWriterNum = new System.Windows.Forms.Label();
            this.txtTextWriter = new System.Windows.Forms.TextBox();
            this.chkTextWriterAppend = new System.Windows.Forms.CheckBox();
            this.pnlTextWriter = new System.Windows.Forms.Panel();
            this.rbTextWriterOn = new System.Windows.Forms.RadioButton();
            this.rbTextWriterOff = new System.Windows.Forms.RadioButton();
            this.lblTextWriter = new System.Windows.Forms.Label();
            this.grpSpeech = new System.Windows.Forms.GroupBox();
            this.pnlSpeechSwitch = new System.Windows.Forms.Panel();
            this.rbSpeechOn = new System.Windows.Forms.RadioButton();
            this.rbSpeechOff = new System.Windows.Forms.RadioButton();
            this.cmbSpeechLevelSignal = new System.Windows.Forms.ComboBox();
            this.cmbSpeechLevelMessage = new System.Windows.Forms.ComboBox();
            this.lblSpeechLevelSignal = new System.Windows.Forms.Label();
            this.lblSpeechLevelMessage = new System.Windows.Forms.Label();
            this.grpConsole.SuspendLayout();
            this.pnlConsoleSwitch.SuspendLayout();
            this.grpMessageBox.SuspendLayout();
            this.pnlMessageBoxSwitch.SuspendLayout();
            this.grpTest.SuspendLayout();
            this.grpOutputLevel.SuspendLayout();
            this.grpReportType.SuspendLayout();
            this.grpMultipleReports.SuspendLayout();
            this.PnlStatus.SuspendLayout();
            this.grpReporters.SuspendLayout();
            this.pnlReporters.SuspendLayout();
            this.grpLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogIndentInitial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogIndentSpacing)).BeginInit();
            this.pnlLogFile.SuspendLayout();
            this.grpTrace.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.GrpTextWriter.SuspendLayout();
            this.pnlTextWriter.SuspendLayout();
            this.grpSpeech.SuspendLayout();
            this.pnlSpeechSwitch.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConsole
            // 
            this.grpConsole.Controls.Add(this.pnlConsoleSwitch);
            this.grpConsole.Location = new System.Drawing.Point(5, 91);
            this.grpConsole.Name = "grpConsole";
            this.grpConsole.Size = new System.Drawing.Size(115, 57);
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
            // grpMessageBox
            // 
            this.grpMessageBox.Controls.Add(this.pnlMessageBoxSwitch);
            this.grpMessageBox.Location = new System.Drawing.Point(126, 94);
            this.grpMessageBox.Name = "grpMessageBox";
            this.grpMessageBox.Size = new System.Drawing.Size(115, 54);
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
            this.rbMessageBoxOn.Location = new System.Drawing.Point(3, 5);
            this.rbMessageBoxOn.Name = "rbMessageBoxOn";
            this.rbMessageBoxOn.Size = new System.Drawing.Size(39, 17);
            this.rbMessageBoxOn.TabIndex = 4;
            this.rbMessageBoxOn.Text = "On";
            this.rbMessageBoxOn.UseVisualStyleBackColor = true;
            this.rbMessageBoxOn.CheckedChanged += new System.EventHandler(this.rbMessageBoxOn_CheckedChanged);
            // 
            // rbMessageBoxOff
            // 
            this.rbMessageBoxOff.AutoSize = true;
            this.rbMessageBoxOff.Checked = true;
            this.rbMessageBoxOff.Location = new System.Drawing.Point(48, 5);
            this.rbMessageBoxOff.Name = "rbMessageBoxOff";
            this.rbMessageBoxOff.Size = new System.Drawing.Size(39, 17);
            this.rbMessageBoxOff.TabIndex = 4;
            this.rbMessageBoxOff.TabStop = true;
            this.rbMessageBoxOff.Text = "Off";
            this.rbMessageBoxOff.UseVisualStyleBackColor = true;
            // 
            // grpTest
            // 
            this.grpTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTest.Controls.Add(this.chkLogVerbose);
            this.grpTest.Controls.Add(this.ChkTraceVerbose);
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
            this.grpTest.Location = new System.Drawing.Point(7, 408);
            this.grpTest.Name = "grpTest";
            this.grpTest.Size = new System.Drawing.Size(865, 307);
            this.grpTest.TabIndex = 11;
            this.grpTest.TabStop = false;
            this.grpTest.Text = "Test Launch";
            // 
            // chkLogVerbose
            // 
            this.chkLogVerbose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkLogVerbose.AutoSize = true;
            this.chkLogVerbose.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkLogVerbose.Location = new System.Drawing.Point(547, 232);
            this.chkLogVerbose.Name = "chkLogVerbose";
            this.chkLogVerbose.Size = new System.Drawing.Size(212, 17);
            this.chkLogVerbose.TabIndex = 3;
            this.chkLogVerbose.Text = "Verbose logging (overrides output level)";
            this.chkLogVerbose.UseVisualStyleBackColor = true;
            // 
            // ChkTraceVerbose
            // 
            this.ChkTraceVerbose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkTraceVerbose.AutoSize = true;
            this.ChkTraceVerbose.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ChkTraceVerbose.Location = new System.Drawing.Point(547, 255);
            this.ChkTraceVerbose.Name = "ChkTraceVerbose";
            this.ChkTraceVerbose.Size = new System.Drawing.Size(210, 17);
            this.ChkTraceVerbose.TabIndex = 3;
            this.ChkTraceVerbose.Text = "Verbose tracing (overrides output level)";
            this.ChkTraceVerbose.UseVisualStyleBackColor = true;
            // 
            // chkThrowException
            // 
            this.chkThrowException.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkThrowException.AutoSize = true;
            this.chkThrowException.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.chkThrowException.Location = new System.Drawing.Point(547, 293);
            this.chkThrowException.Name = "chkThrowException";
            this.chkThrowException.Size = new System.Drawing.Size(204, 17);
            this.chkThrowException.TabIndex = 3;
            this.chkThrowException.Text = "Throw internal exception (testing only)";
            this.chkThrowException.UseVisualStyleBackColor = true;
            // 
            // grpOutputLevel
            // 
            this.grpOutputLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOutputLevel.Controls.Add(this.rbOutVerbose);
            this.grpOutputLevel.Controls.Add(this.rbOutInfo);
            this.grpOutputLevel.Controls.Add(this.rbOutWarning);
            this.grpOutputLevel.Controls.Add(this.rbOutError);
            this.grpOutputLevel.Location = new System.Drawing.Point(693, 136);
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
            this.grpReportType.Location = new System.Drawing.Point(547, 136);
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
            this.btnLaunch.Location = new System.Drawing.Point(368, 11);
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
            this.grpMultipleReports.Location = new System.Drawing.Point(547, 40);
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
            this.txtExMessage.Size = new System.Drawing.Size(515, 137);
            this.txtExMessage.TabIndex = 1;
            this.txtExMessage.Text = "Internal exception message.\r\nInternal exception message, second line.";
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
            this.txtMessage.Size = new System.Drawing.Size(511, 62);
            this.txtMessage.TabIndex = 1;
            this.txtMessage.Text = "Message text, first line.\r\nMessage text, second line.";
            // 
            // txtLocation
            // 
            this.txtLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocation.Location = new System.Drawing.Point(8, 40);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(511, 20);
            this.txtLocation.TabIndex = 1;
            this.txtLocation.Text = "User defined location";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.lblDescription.ForeColor = System.Drawing.Color.Green;
            this.lblDescription.Location = new System.Drawing.Point(16, 45);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(299, 32);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "This panel is used to configure how error, warning \r\nand information messages are" +
                " shown or logged.";
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
            this.PnlStatus.Location = new System.Drawing.Point(4, 721);
            this.PnlStatus.Name = "PnlStatus";
            this.PnlStatus.Size = new System.Drawing.Size(869, 30);
            this.PnlStatus.TabIndex = 10;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.AutoSize = true;
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CancelBtn.Location = new System.Drawing.Point(810, 5);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(5);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(50, 24);
            this.CancelBtn.TabIndex = 2;
            this.CancelBtn.Text = "Close";
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
            // grpReporters
            // 
            this.grpReporters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpReporters.Controls.Add(this.pnlReporters);
            this.grpReporters.Location = new System.Drawing.Point(346, 12);
            this.grpReporters.Name = "grpReporters";
            this.grpReporters.Size = new System.Drawing.Size(527, 73);
            this.grpReporters.TabIndex = 14;
            this.grpReporters.TabStop = false;
            this.grpReporters.Text = "Reporter type";
            // 
            // pnlReporters
            // 
            this.pnlReporters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlReporters.Controls.Add(this.rbBasic);
            this.pnlReporters.Controls.Add(this.rbConsoleMessageBox);
            this.pnlReporters.Controls.Add(this.rbMessageBox);
            this.pnlReporters.Controls.Add(this.rbConsole);
            this.pnlReporters.Location = new System.Drawing.Point(10, 15);
            this.pnlReporters.Name = "pnlReporters";
            this.pnlReporters.Size = new System.Drawing.Size(311, 52);
            this.pnlReporters.TabIndex = 0;
            // 
            // rbBasic
            // 
            this.rbBasic.AutoSize = true;
            this.rbBasic.Location = new System.Drawing.Point(3, 5);
            this.rbBasic.Name = "rbBasic";
            this.rbBasic.Size = new System.Drawing.Size(146, 17);
            this.rbBasic.TabIndex = 4;
            this.rbBasic.Text = "Basic (no console or GUI)";
            this.rbBasic.UseVisualStyleBackColor = true;
            this.rbBasic.CheckedChanged += new System.EventHandler(this.rbBasic_CheckedChanged);
            // 
            // rbConsoleMessageBox
            // 
            this.rbConsoleMessageBox.AutoSize = true;
            this.rbConsoleMessageBox.Checked = true;
            this.rbConsoleMessageBox.Location = new System.Drawing.Point(155, 28);
            this.rbConsoleMessageBox.Name = "rbConsoleMessageBox";
            this.rbConsoleMessageBox.Size = new System.Drawing.Size(139, 17);
            this.rbConsoleMessageBox.TabIndex = 4;
            this.rbConsoleMessageBox.TabStop = true;
            this.rbConsoleMessageBox.Text = "Console && Message Box";
            this.rbConsoleMessageBox.UseVisualStyleBackColor = true;
            this.rbConsoleMessageBox.CheckedChanged += new System.EventHandler(this.rbConsoleMessageBox_CheckedChanged);
            // 
            // rbMessageBox
            // 
            this.rbMessageBox.AutoSize = true;
            this.rbMessageBox.Location = new System.Drawing.Point(155, 5);
            this.rbMessageBox.Name = "rbMessageBox";
            this.rbMessageBox.Size = new System.Drawing.Size(89, 17);
            this.rbMessageBox.TabIndex = 4;
            this.rbMessageBox.Text = "Message Box";
            this.rbMessageBox.UseVisualStyleBackColor = true;
            this.rbMessageBox.CheckedChanged += new System.EventHandler(this.rbMessageBox_CheckedChanged);
            // 
            // rbConsole
            // 
            this.rbConsole.AutoSize = true;
            this.rbConsole.Location = new System.Drawing.Point(3, 30);
            this.rbConsole.Name = "rbConsole";
            this.rbConsole.Size = new System.Drawing.Size(63, 17);
            this.rbConsole.TabIndex = 4;
            this.rbConsole.Text = "Console";
            this.rbConsole.UseVisualStyleBackColor = true;
            this.rbConsole.CheckedChanged += new System.EventHandler(this.rbConsole_CheckedChanged);
            // 
            // grpLog
            // 
            this.grpLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLog.Controls.Add(this.btnLogFile);
            this.grpLog.Controls.Add(this.lblLogNum);
            this.grpLog.Controls.Add(this.txtLogIndentInitial);
            this.grpLog.Controls.Add(this.txtLogIndentSpacing);
            this.grpLog.Controls.Add(this.chkLogAppend);
            this.grpLog.Controls.Add(this.pnlLogFile);
            this.grpLog.Controls.Add(this.txtLogFile);
            this.grpLog.Controls.Add(this.lblLogIndentInitial);
            this.grpLog.Controls.Add(this.lblLogIndentIncrement);
            this.grpLog.Controls.Add(this.lblLogFile);
            this.grpLog.Location = new System.Drawing.Point(5, 277);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(865, 62);
            this.grpLog.TabIndex = 13;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log file (one line indented messages)";
            // 
            // btnLogFile
            // 
            this.btnLogFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnLogFile.Location = new System.Drawing.Point(110, 34);
            this.btnLogFile.Name = "btnLogFile";
            this.btnLogFile.Size = new System.Drawing.Size(36, 22);
            this.btnLogFile.TabIndex = 5;
            this.btnLogFile.Text = "...";
            this.btnLogFile.UseVisualStyleBackColor = true;
            this.btnLogFile.Click += new System.EventHandler(this.btnLogFile_Click);
            // 
            // lblLogNum
            // 
            this.lblLogNum.AutoSize = true;
            this.lblLogNum.ForeColor = System.Drawing.Color.Gray;
            this.lblLogNum.Location = new System.Drawing.Point(743, 0);
            this.lblLogNum.Name = "lblLogNum";
            this.lblLogNum.Size = new System.Drawing.Size(118, 13);
            this.lblLogNum.TabIndex = 4;
            this.lblLogNum.Text = "Number of text writers: -";
            // 
            // txtLogIndentInitial
            // 
            this.txtLogIndentInitial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLogIndentInitial.Location = new System.Drawing.Point(583, 13);
            this.txtLogIndentInitial.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtLogIndentInitial.Name = "txtLogIndentInitial";
            this.txtLogIndentInitial.Size = new System.Drawing.Size(37, 20);
            this.txtLogIndentInitial.TabIndex = 4;
            this.txtLogIndentInitial.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtLogIndentInitial.Leave += new System.EventHandler(this.txtLogIndentInitial_Leave);
            // 
            // txtLogIndentSpacing
            // 
            this.txtLogIndentSpacing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLogIndentSpacing.Location = new System.Drawing.Point(424, 13);
            this.txtLogIndentSpacing.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtLogIndentSpacing.Name = "txtLogIndentSpacing";
            this.txtLogIndentSpacing.Size = new System.Drawing.Size(37, 20);
            this.txtLogIndentSpacing.TabIndex = 4;
            this.txtLogIndentSpacing.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.txtLogIndentSpacing.Leave += new System.EventHandler(this.txtLogIndentSpacing_Leave);
            // 
            // chkLogAppend
            // 
            this.chkLogAppend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLogAppend.AutoSize = true;
            this.chkLogAppend.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkLogAppend.Location = new System.Drawing.Point(203, 19);
            this.chkLogAppend.Name = "chkLogAppend";
            this.chkLogAppend.Size = new System.Drawing.Size(79, 17);
            this.chkLogAppend.TabIndex = 3;
            this.chkLogAppend.Text = "Append file";
            this.chkLogAppend.UseVisualStyleBackColor = true;
            this.chkLogAppend.CheckedChanged += new System.EventHandler(this.chkLogAppend_CheckedChanged);
            // 
            // pnlLogFile
            // 
            this.pnlLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlLogFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLogFile.Controls.Add(this.rbLogOn);
            this.pnlLogFile.Controls.Add(this.rbLogOff);
            this.pnlLogFile.Location = new System.Drawing.Point(10, 24);
            this.pnlLogFile.Name = "pnlLogFile";
            this.pnlLogFile.Size = new System.Drawing.Size(92, 27);
            this.pnlLogFile.TabIndex = 0;
            // 
            // rbLogOn
            // 
            this.rbLogOn.AutoSize = true;
            this.rbLogOn.Location = new System.Drawing.Point(3, 5);
            this.rbLogOn.Name = "rbLogOn";
            this.rbLogOn.Size = new System.Drawing.Size(39, 17);
            this.rbLogOn.TabIndex = 4;
            this.rbLogOn.Text = "On";
            this.rbLogOn.UseVisualStyleBackColor = true;
            this.rbLogOn.CheckedChanged += new System.EventHandler(this.rbLogOn_CheckedChanged);
            // 
            // rbLogOff
            // 
            this.rbLogOff.AutoSize = true;
            this.rbLogOff.Checked = true;
            this.rbLogOff.Location = new System.Drawing.Point(48, 5);
            this.rbLogOff.Name = "rbLogOff";
            this.rbLogOff.Size = new System.Drawing.Size(39, 17);
            this.rbLogOff.TabIndex = 4;
            this.rbLogOff.TabStop = true;
            this.rbLogOff.Text = "Off";
            this.rbLogOff.UseVisualStyleBackColor = true;
            // 
            // txtLogFile
            // 
            this.txtLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLogFile.Location = new System.Drawing.Point(152, 36);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.Size = new System.Drawing.Size(735, 20);
            this.txtLogFile.TabIndex = 1;
            this.txtLogFile.Text = "%temp%\\log.txt";
            this.txtLogFile.Leave += new System.EventHandler(this.txtLogFile_Leave);
            // 
            // lblLogIndentInitial
            // 
            this.lblLogIndentInitial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLogIndentInitial.AutoSize = true;
            this.lblLogIndentInitial.Location = new System.Drawing.Point(488, 15);
            this.lblLogIndentInitial.Name = "lblLogIndentInitial";
            this.lblLogIndentInitial.Size = new System.Drawing.Size(89, 13);
            this.lblLogIndentInitial.TabIndex = 2;
            this.lblLogIndentInitial.Text = "Initial indentation:";
            // 
            // lblLogIndentIncrement
            // 
            this.lblLogIndentIncrement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLogIndentIncrement.AutoSize = true;
            this.lblLogIndentIncrement.Location = new System.Drawing.Point(306, 15);
            this.lblLogIndentIncrement.Name = "lblLogIndentIncrement";
            this.lblLogIndentIncrement.Size = new System.Drawing.Size(112, 13);
            this.lblLogIndentIncrement.TabIndex = 2;
            this.lblLogIndentIncrement.Text = "Indentation increment:";
            // 
            // lblLogFile
            // 
            this.lblLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLogFile.AutoSize = true;
            this.lblLogFile.Location = new System.Drawing.Point(106, 20);
            this.lblLogFile.Name = "lblLogFile";
            this.lblLogFile.Size = new System.Drawing.Size(44, 13);
            this.lblLogFile.TabIndex = 2;
            this.lblLogFile.Text = "Log file:";
            // 
            // grpTrace
            // 
            this.grpTrace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTrace.Controls.Add(this.chkTraceToConsole);
            this.grpTrace.Controls.Add(this.pnlTrace);
            this.grpTrace.Location = new System.Drawing.Point(5, 345);
            this.grpTrace.Name = "grpTrace";
            this.grpTrace.Size = new System.Drawing.Size(865, 57);
            this.grpTrace.TabIndex = 13;
            this.grpTrace.TabStop = false;
            this.grpTrace.Text = "Trace";
            // 
            // chkTraceToConsole
            // 
            this.chkTraceToConsole.AutoSize = true;
            this.chkTraceToConsole.Checked = true;
            this.chkTraceToConsole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTraceToConsole.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkTraceToConsole.Location = new System.Drawing.Point(109, 29);
            this.chkTraceToConsole.Name = "chkTraceToConsole";
            this.chkTraceToConsole.Size = new System.Drawing.Size(156, 17);
            this.chkTraceToConsole.TabIndex = 3;
            this.chkTraceToConsole.Text = "Log trace output to console";
            this.chkTraceToConsole.UseVisualStyleBackColor = true;
            this.chkTraceToConsole.CheckedChanged += new System.EventHandler(this.chkTraceToConsole_CheckedChanged);
            // 
            // pnlTrace
            // 
            this.pnlTrace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTrace.Controls.Add(this.rbTraceOn);
            this.pnlTrace.Controls.Add(this.rbTraceOff);
            this.pnlTrace.Location = new System.Drawing.Point(10, 19);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(92, 27);
            this.pnlTrace.TabIndex = 0;
            // 
            // rbTraceOn
            // 
            this.rbTraceOn.AutoSize = true;
            this.rbTraceOn.Location = new System.Drawing.Point(3, 5);
            this.rbTraceOn.Name = "rbTraceOn";
            this.rbTraceOn.Size = new System.Drawing.Size(39, 17);
            this.rbTraceOn.TabIndex = 4;
            this.rbTraceOn.Text = "On";
            this.rbTraceOn.UseVisualStyleBackColor = true;
            this.rbTraceOn.CheckedChanged += new System.EventHandler(this.rbTraceOn_CheckedChanged);
            // 
            // rbTraceOff
            // 
            this.rbTraceOff.AutoSize = true;
            this.rbTraceOff.Checked = true;
            this.rbTraceOff.Location = new System.Drawing.Point(48, 5);
            this.rbTraceOff.Name = "rbTraceOff";
            this.rbTraceOff.Size = new System.Drawing.Size(39, 17);
            this.rbTraceOff.TabIndex = 4;
            this.rbTraceOff.TabStop = true;
            this.rbTraceOff.Text = "Off";
            this.rbTraceOff.UseVisualStyleBackColor = true;
            // 
            // GrpTextWriter
            // 
            this.GrpTextWriter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.GrpTextWriter.Controls.Add(this.btnTextWriter);
            this.GrpTextWriter.Controls.Add(this.lblTextWriterNum);
            this.GrpTextWriter.Controls.Add(this.txtTextWriter);
            this.GrpTextWriter.Controls.Add(this.chkTextWriterAppend);
            this.GrpTextWriter.Controls.Add(this.pnlTextWriter);
            this.GrpTextWriter.Controls.Add(this.lblTextWriter);
            this.GrpTextWriter.Location = new System.Drawing.Point(5, 214);
            this.GrpTextWriter.Name = "GrpTextWriter";
            this.GrpTextWriter.Size = new System.Drawing.Size(865, 57);
            this.GrpTextWriter.TabIndex = 13;
            this.GrpTextWriter.TabStop = false;
            this.GrpTextWriter.Text = "Text file (messages written in long decorated form)";
            // 
            // btnTextWriter
            // 
            this.btnTextWriter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnTextWriter.Location = new System.Drawing.Point(110, 29);
            this.btnTextWriter.Name = "btnTextWriter";
            this.btnTextWriter.Size = new System.Drawing.Size(36, 22);
            this.btnTextWriter.TabIndex = 5;
            this.btnTextWriter.Text = "...";
            this.btnTextWriter.UseVisualStyleBackColor = true;
            this.btnTextWriter.Click += new System.EventHandler(this.btnTextWriter_Click);
            // 
            // lblTextWriterNum
            // 
            this.lblTextWriterNum.AutoSize = true;
            this.lblTextWriterNum.ForeColor = System.Drawing.Color.Gray;
            this.lblTextWriterNum.Location = new System.Drawing.Point(743, 0);
            this.lblTextWriterNum.Name = "lblTextWriterNum";
            this.lblTextWriterNum.Size = new System.Drawing.Size(118, 13);
            this.lblTextWriterNum.TabIndex = 4;
            this.lblTextWriterNum.Text = "Number of text writers: -";
            // 
            // txtTextWriter
            // 
            this.txtTextWriter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTextWriter.Location = new System.Drawing.Point(152, 31);
            this.txtTextWriter.Name = "txtTextWriter";
            this.txtTextWriter.Size = new System.Drawing.Size(707, 20);
            this.txtTextWriter.TabIndex = 1;
            this.txtTextWriter.Text = "%temp%\\reporter.txt";
            this.txtTextWriter.Leave += new System.EventHandler(this.txtTextWriter_Leave);
            // 
            // chkTextWriterAppend
            // 
            this.chkTextWriterAppend.AutoSize = true;
            this.chkTextWriterAppend.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkTextWriterAppend.Location = new System.Drawing.Point(201, 15);
            this.chkTextWriterAppend.Name = "chkTextWriterAppend";
            this.chkTextWriterAppend.Size = new System.Drawing.Size(79, 17);
            this.chkTextWriterAppend.TabIndex = 3;
            this.chkTextWriterAppend.Text = "Append file";
            this.chkTextWriterAppend.UseVisualStyleBackColor = true;
            this.chkTextWriterAppend.CheckedChanged += new System.EventHandler(this.chkTextWriterAppend_CheckedChanged);
            // 
            // pnlTextWriter
            // 
            this.pnlTextWriter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTextWriter.Controls.Add(this.rbTextWriterOn);
            this.pnlTextWriter.Controls.Add(this.rbTextWriterOff);
            this.pnlTextWriter.Location = new System.Drawing.Point(10, 19);
            this.pnlTextWriter.Name = "pnlTextWriter";
            this.pnlTextWriter.Size = new System.Drawing.Size(92, 27);
            this.pnlTextWriter.TabIndex = 0;
            // 
            // rbTextWriterOn
            // 
            this.rbTextWriterOn.AutoSize = true;
            this.rbTextWriterOn.Location = new System.Drawing.Point(3, 5);
            this.rbTextWriterOn.Name = "rbTextWriterOn";
            this.rbTextWriterOn.Size = new System.Drawing.Size(39, 17);
            this.rbTextWriterOn.TabIndex = 4;
            this.rbTextWriterOn.Text = "On";
            this.rbTextWriterOn.UseVisualStyleBackColor = true;
            this.rbTextWriterOn.CheckedChanged += new System.EventHandler(this.rbTextWriterOn_CheckedChanged);
            // 
            // rbTextWriterOff
            // 
            this.rbTextWriterOff.AutoSize = true;
            this.rbTextWriterOff.Checked = true;
            this.rbTextWriterOff.Location = new System.Drawing.Point(48, 5);
            this.rbTextWriterOff.Name = "rbTextWriterOff";
            this.rbTextWriterOff.Size = new System.Drawing.Size(39, 17);
            this.rbTextWriterOff.TabIndex = 4;
            this.rbTextWriterOff.TabStop = true;
            this.rbTextWriterOff.Text = "Off";
            this.rbTextWriterOff.UseVisualStyleBackColor = true;
            // 
            // lblTextWriter
            // 
            this.lblTextWriter.AutoSize = true;
            this.lblTextWriter.Location = new System.Drawing.Point(106, 15);
            this.lblTextWriter.Name = "lblTextWriter";
            this.lblTextWriter.Size = new System.Drawing.Size(80, 13);
            this.lblTextWriter.TabIndex = 2;
            this.lblTextWriter.Text = "Text output file:";
            // 
            // grpSpeech
            // 
            this.grpSpeech.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSpeech.Controls.Add(this.cmbSpeechLevelMessage);
            this.grpSpeech.Controls.Add(this.cmbSpeechLevelSignal);
            this.grpSpeech.Controls.Add(this.pnlSpeechSwitch);
            this.grpSpeech.Controls.Add(this.lblSpeechLevelMessage);
            this.grpSpeech.Controls.Add(this.lblSpeechLevelSignal);
            this.grpSpeech.Location = new System.Drawing.Point(4, 154);
            this.grpSpeech.Name = "grpSpeech";
            this.grpSpeech.Size = new System.Drawing.Size(866, 54);
            this.grpSpeech.TabIndex = 15;
            this.grpSpeech.TabStop = false;
            this.grpSpeech.Text = "Speech";
            // 
            // pnlSpeechSwitch
            // 
            this.pnlSpeechSwitch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSpeechSwitch.Controls.Add(this.rbSpeechOn);
            this.pnlSpeechSwitch.Controls.Add(this.rbSpeechOff);
            this.pnlSpeechSwitch.Location = new System.Drawing.Point(13, 19);
            this.pnlSpeechSwitch.Name = "pnlSpeechSwitch";
            this.pnlSpeechSwitch.Size = new System.Drawing.Size(92, 27);
            this.pnlSpeechSwitch.TabIndex = 0;
            // 
            // rbSpeechOn
            // 
            this.rbSpeechOn.AutoSize = true;
            this.rbSpeechOn.Location = new System.Drawing.Point(3, 5);
            this.rbSpeechOn.Name = "rbSpeechOn";
            this.rbSpeechOn.Size = new System.Drawing.Size(39, 17);
            this.rbSpeechOn.TabIndex = 4;
            this.rbSpeechOn.Text = "On";
            this.rbSpeechOn.UseVisualStyleBackColor = true;
            this.rbSpeechOn.CheckedChanged += new System.EventHandler(this.rbMessageBoxOn_CheckedChanged);
            // 
            // rbSpeechOff
            // 
            this.rbSpeechOff.AutoSize = true;
            this.rbSpeechOff.Checked = true;
            this.rbSpeechOff.Location = new System.Drawing.Point(48, 5);
            this.rbSpeechOff.Name = "rbSpeechOff";
            this.rbSpeechOff.Size = new System.Drawing.Size(39, 17);
            this.rbSpeechOff.TabIndex = 4;
            this.rbSpeechOff.TabStop = true;
            this.rbSpeechOff.Text = "Off";
            this.rbSpeechOff.UseVisualStyleBackColor = true;
            // 
            // cmbSpeechLevelSignal
            // 
            this.cmbSpeechLevelSignal.FormattingEnabled = true;
            this.cmbSpeechLevelSignal.Items.AddRange(new object[] {
            "Off",
            "Error",
            "Warning",
            "Info",
            "Verbose"});
            this.cmbSpeechLevelSignal.Location = new System.Drawing.Point(111, 27);
            this.cmbSpeechLevelSignal.Name = "cmbSpeechLevelSignal";
            this.cmbSpeechLevelSignal.Size = new System.Drawing.Size(121, 21);
            this.cmbSpeechLevelSignal.TabIndex = 1;
            this.cmbSpeechLevelSignal.TextChanged += new System.EventHandler(this.cmbSpeechLevelSignal_TextChanged);
            // 
            // cmbSpeechLevelMessage
            // 
            this.cmbSpeechLevelMessage.DisplayMember = "(none)";
            this.cmbSpeechLevelMessage.FormattingEnabled = true;
            this.cmbSpeechLevelMessage.Items.AddRange(new object[] {
            "Off",
            "Error",
            "Warning",
            "Info",
            "Verbose"});
            this.cmbSpeechLevelMessage.Location = new System.Drawing.Point(265, 27);
            this.cmbSpeechLevelMessage.Name = "cmbSpeechLevelMessage";
            this.cmbSpeechLevelMessage.Size = new System.Drawing.Size(121, 21);
            this.cmbSpeechLevelMessage.TabIndex = 1;
            this.cmbSpeechLevelMessage.TextChanged += new System.EventHandler(this.cmbSpeechLevelMessage_TextChanged);
            // 
            // lblSpeechLevelSignal
            // 
            this.lblSpeechLevelSignal.AutoSize = true;
            this.lblSpeechLevelSignal.Location = new System.Drawing.Point(111, 11);
            this.lblSpeechLevelSignal.Name = "lblSpeechLevelSignal";
            this.lblSpeechLevelSignal.Size = new System.Drawing.Size(122, 13);
            this.lblSpeechLevelSignal.TabIndex = 2;
            this.lblSpeechLevelSignal.Text = "Reporting level - signals:";
            // 
            // lblSpeechLevelMessage
            // 
            this.lblSpeechLevelMessage.AutoSize = true;
            this.lblSpeechLevelMessage.Location = new System.Drawing.Point(262, 11);
            this.lblSpeechLevelMessage.Name = "lblSpeechLevelMessage";
            this.lblSpeechLevelMessage.Size = new System.Drawing.Size(137, 13);
            this.lblSpeechLevelMessage.TabIndex = 2;
            this.lblSpeechLevelMessage.Text = "Reporting level - messages:";
            // 
            // ReporterConf
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(877, 755);
            this.Controls.Add(this.grpTrace);
            this.Controls.Add(this.grpSpeech);
            this.Controls.Add(this.grpMessageBox);
            this.Controls.Add(this.GrpTextWriter);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpConsole);
            this.Controls.Add(this.grpReporters);
            this.Controls.Add(this.grpTest);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.PnlStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(350, 400);
            this.Name = "ReporterConf";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Text = "Reporter configuration utility";
            this.Load += new System.EventHandler(this.ReporterConf_Load);
            this.grpConsole.ResumeLayout(false);
            this.pnlConsoleSwitch.ResumeLayout(false);
            this.pnlConsoleSwitch.PerformLayout();
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
            this.grpReporters.ResumeLayout(false);
            this.pnlReporters.ResumeLayout(false);
            this.pnlReporters.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogIndentInitial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogIndentSpacing)).EndInit();
            this.pnlLogFile.ResumeLayout(false);
            this.pnlLogFile.PerformLayout();
            this.grpTrace.ResumeLayout(false);
            this.grpTrace.PerformLayout();
            this.pnlTrace.ResumeLayout(false);
            this.pnlTrace.PerformLayout();
            this.GrpTextWriter.ResumeLayout(false);
            this.GrpTextWriter.PerformLayout();
            this.pnlTextWriter.ResumeLayout(false);
            this.pnlTextWriter.PerformLayout();
            this.grpSpeech.ResumeLayout(false);
            this.grpSpeech.PerformLayout();
            this.pnlSpeechSwitch.ResumeLayout(false);
            this.pnlSpeechSwitch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion  // Windows Form Designer generated code



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
            catch (Exception ex) { R.ReportError("ReporterConf", null, ex); }
        }


        /// <summary>Specifies whether the sub-panel for speech settings is visible or not.</summary>
        protected bool SpeechVisible = false;

        private void ReporterConf_Load(object sender, System.EventArgs args)
        // $A Igor Jul08; 
        {
            try
            {
                // Set common event handlers recursively (move the consform with a mouse & close with button 3):
                if (SpeechVisible)
                    grpSpeech.Visible = true;
                else
                    grpSpeech.Visible = false;
            }
            catch (Exception ex) 
            {
                R.ReportError("ReporterConf", null, ex);
            }
            try
            {
                // Set common event handlers recursively (move the consform with a mouse & close with button 3):
                RecursiveControlDelegate(this, new ControlDelegate(SetCommonEvents));
            }
            catch (Exception ex) 
            {
                R.ReportError("ReporterConf", null, ex);
            }
            finally
            {
                try
                {
                    ApplyGuiSettings();
                }
                catch (Exception e) { R.ReportError(e, "\nError inside ReporterConf.Load"); }
            }
            cmbSpeechLevelSignal.SelectedIndex = 2;
            cmbSpeechLevelMessage.SelectedIndex = 0;
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
                }
            }
            catch (Exception ex) { R.ReportError("ReporterConf", null, ex);  }
        }



        #region Mouse_Control

        // Event handlers FadeMessage_MouseMove and ReporterConf_MouseDown enable dragging of messagebox
        // without a frame.
        int m_PrevX;
        int m_PrevY;
        private void ReporterConf_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            Left = Left + (e.X - m_PrevX);
            Top = Top + (e.Y - m_PrevY);
        }

        private void ReporterConf_MouseDown(object sender, MouseEventArgs e)
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
                    // This will enable to kill the window by 
                    // holding Ctrl and clicking the mouse button 3
                    f.MouseClick += new MouseEventHandler(this.ReportConfig_MouseClick);
                }
                catch (Exception e) { R.ReportError(e); }
                try
                {
                    //// This will allow dragging the window by mouse:
                    //f.MouseDown += new MouseEventHandler(this.ReporterConf_MouseDown);
                    //f.MouseMove += new MouseEventHandler(this.ReporterConf_MouseMove);
                }
                catch (Exception e) { Exception ee = e; }
            }
        }


        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        #endregion  // Mouse_Control



        #region Actions

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentReporter == null)
                    throw new Exception("Reporter to be configured is not specified.");
                ReportType type = ReportType.Error;
                if (rbError.Checked)
                    type = ReportType.Error;
                else if (rbWarning.Checked)
                    type = ReportType.Warning;
                else if (rbInfo.Checked)
                    type = ReportType.Info;
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

                // Set the appropriate things on the CurrentReporter:
                CurrentReporter.ReportingLevel = level;
                CurrentReporter.LoggingLevel = level;
                CurrentReporter.TracingLevel = level;
                if (chkLogVerbose.Checked)
                    CurrentReporter.LoggingLevel = ReportLevel.Verbose;
                if (ChkTraceVerbose.Checked)
                    CurrentReporter.TracingLevel = ReportLevel.Verbose;

                if (chkThrowException.Checked)
                    CurrentReporter.ThrowTestException = chkThrowException.Checked;

                // Launch error report(s):
                int numlaunches;
                if (chkMultiple.Checked)
                    numlaunches = int.Parse(txtNumLaunches.Text);
                else
                    numlaunches = 1;
                for (int i = 1; i <= numlaunches; ++i)
                {
                    string loc;
                    if (chkMultiple.Checked)
                        loc = location + "_" + i.ToString();
                    else
                        loc = location;
                    CurrentReporter.Report(type, loc, message, exrep);
                    if (chkMultiple.Checked && i!=numlaunches)
                        Thread.Sleep(int.Parse(txtDelay.Text));
                }
            }
            catch (Exception ex) { R.ReportError("ReporterConf", null, ex); }
        }

        #endregion Actions



        #region GUI

        // Remark: Although most of the event handlers in this region of code just call ApplyGuiSettings(),
        // each of them is implemented separately in order to prepare grounds for extensions.

        #region GUI_ReporterType

        // After changing the reporter type:

        private void rbBasic_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }


        private void rbConsole_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        private void rbMessageBox_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        private void rbConsoleMessageBox_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        #endregion  // GUI_ReporterType


        #region GUI_TextWriter

        private void rbTextWriterOn_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        private void chkTextWriterAppend_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        /// <summary>Checks whether the directory containing the file exists, and applies the output file.</summary>
        private void txtTextWriter_Leave(object sender, EventArgs e)
        {
            try
            {
                string path = System.Environment.ExpandEnvironmentVariables(txtTextWriter.Text);
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    txtTextWriter.BackColor = Color.Orange;
                else
                {
                    txtTextWriter.BackColor = Color.White;
                    ApplyGuiSettings();
                }
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
            }
        }


        #endregion  // GUI_TextWriter

        #region GUI_LogWriter

        private void rbLogOn_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        private void chkLogAppend_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        /// <summary>Checks whether the directory containing the file exists, and applies the output file.</summary>
        private void txtLogFile_Leave(object sender, EventArgs e)
        {
            try
            {
                string path = System.Environment.ExpandEnvironmentVariables(txtLogFile.Text);
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    txtLogFile.BackColor = Color.Orange;
                else
                {
                    txtLogFile.BackColor = Color.White;
                    ApplyGuiSettings();
                }
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
            }
        }

        private void txtLogIndentSpacing_Leave(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        private void txtLogIndentInitial_Leave(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        #endregion  // GUI_LogWriter

        #region GUI_Trace

        private void rbTraceOn_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        private void chkTraceToConsole_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        #endregion  // GUI_Trace

        #region GUI_Console

        private void rbConsoleOn_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyGuiSettings();
            }
            catch (Exception ex)
            {
                R.ReportError("rbConsoleOn_CheckedChanged", "Inconsistent user action (reporter type should be changed): ", ex);
            }
        }

        #endregion   //  GUI_Console

        #region GUI_MessageBox

        private void rbMessageBoxOn_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyGuiSettings();
            }
            catch (Exception ex)
            {
                R.ReportError("rbMessageBoxOn_CheckedChanged", ex);
            }
        }

        #endregion   //  GUI_MessageBox

        private void btnTextWriter_Click(object sender, EventArgs e)
        {
            try
            {
                string currentfile = Environment.ExpandEnvironmentVariables(txtTextWriter.Text);
                string currentdir = Path.GetDirectoryName(currentfile);
                FileDialog filedlg = new OpenFileDialog();

                filedlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                filedlg.FilterIndex = 2;
                // filedlg.RestoreDirectory = true ;
                filedlg.InitialDirectory = currentdir;
                filedlg.FileName = currentfile;
                if (filedlg.ShowDialog() == DialogResult.OK)
                {
                    txtTextWriter.Text = filedlg.FileName;
                    ApplyGuiSettings();
                }
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
            }
        }

      #endregion  // GUI

        private void btnLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                string currentfile = Environment.ExpandEnvironmentVariables(txtLogFile.Text);
                string currentdir = Path.GetDirectoryName(currentfile);
                FileDialog filedlg = new OpenFileDialog();

                filedlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                filedlg.FilterIndex = 2;
                // filedlg.RestoreDirectory = true ;
                filedlg.InitialDirectory = currentdir;
                filedlg.FileName = currentfile;
                if (filedlg.ShowDialog() == DialogResult.OK)
                {
                    txtTextWriter.Text = filedlg.FileName;
                    ApplyGuiSettings();
                }
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
            }
        }


        private void cmbSpeechLevelSignal_TextChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }

        private void cmbSpeechLevelMessage_TextChanged(object sender, EventArgs e)
        {
            ApplyGuiSettings();
        }


    }  // Class ConsoleForm

}
