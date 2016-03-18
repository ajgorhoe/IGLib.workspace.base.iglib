using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using IG.Lib;
using IG.Forms;

namespace IG.Forms
{ 



    /// <summary>Reporting system that in particular utilizes forms.
    /// Beside the main delegates for assembling location and message strings, this class has three additional delegates
    /// for each kind of reporting (i.e. reporting with console consform, message box, fading message or console)</summary>
    /// <remarks>The Reporting class objects can be extended with appropriate delegates to use forms, while ReporterForms itself
    /// contains implementation of methods that utilize several standard forms suitable for reporting.</remarks>
    public class ReporterForms :  ReporterBase // IG.Lib.Reporter
    {


        #region Initialization 

        // Constructors:

        /// <summary>Constructor. Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        public ReporterForms()
        // $A Igor Oct08;
        {
            InitBegin();
            InitEnd();
        }


        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Delegates that are not specified (have a null reference) are set to default values.</summary>
        /// <param name="obj">Auxiliary object that will be passed to error reporting delegates when called in local methods.</param>
        /// <param name="reportdelegate">Delegates that is called to launc an error report.
        /// Methods of this class will pass to this class the auxiliary object, location strings assembled by the location assembling delegate,
        /// and error message string assembled by the error message delegate.</param>
        /// <param name="locationdelegate">Delegate that is called to assemble the error location string. 
        /// The Auxiliary object this.Obj will be internally passed to this delegate any time it is called.</param>
        /// <param name="messagedelegate">Delegate that is called to assemble the error message (without decorations).
        /// The Auxiliary object this.Obj will be internally passed to this delegate any time it is called.</param>
        /// <param name="reservereportdelegate">Delegate that is called to report exceptions that occur within error reporting
        /// methods. In particular, this must ne as bullet proof as possible.</param>
        public ReporterForms(object obj,
            ReportDelegate reportdelegate,
            ReportLocationDelegate locationdelegate,
            ReportMessageDelegate messagedelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                InitBegin();  // Remove all delegate functions.
                this.Obj = obj;
                this.ReportDlg = reportdelegate;
                this.ReportLocationDlg = locationdelegate;
                this.ReportMessageDlg = messagedelegate;
                ReserveReportErrorDlg = reservereportdelegate;
                //Initialize delegates that were not provided with default values:
                InitEnd();  // Assign default valse to the delegates that were not set.
            }
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates that are not specified are set to default values.</summary>
        public ReporterForms(object obj,
            ReportDelegate reportdelegate,
            ReportLocationDelegate locationdelegate,
            ReportMessageDelegate messagedelegate)
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                Init(obj, reportdelegate, locationdelegate, messagedelegate, null /* reservereportdelegate */ );
            }
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and the delegate to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates for assembling the error location string and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        public ReporterForms(object obj,
            ReportDelegate reportdelegate)
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                Init(obj, reportdelegate, null /* locationdelegate */, null /* messagedelegate */, null /* reservereportdelegate */ );
            }
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Delegates for assembling the error locatin and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        /// <param name="obj">Auxiliary object that will be passed to error reporting delegates when called in local methods.</param>
        /// <param name="reportdelegate">Delegates that is called to launc an error report.
        /// Methods of this class will pass to this class the auxiliary object, location strings assembled by the location assembling delegate,
        /// and error message string assembled by the error message delegate.</param>
        /// <param name="reservereportdelegate">Delegate that is called to report exceptions that occur within error reporting
        /// methods. In particular, this must ne as bullet proof as possible.</param>
        public ReporterForms(object obj,
            ReportDelegate reportdelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                Init(obj, reportdelegate, null /* locationdelegate */, null /* messagedelegate */,
                    reservereportdelegate);
            }
        }
        


        // Setting default error reporting behavior:

        /// <summary>Sets the error reporting delegate to the default value.
        /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected override void SetDefaultReportDlg()
        // $A Igor Oct08;
        {
            ReportDlg = null;  //  new ReportDelegate(DefaultReportForms);
        }

        /// <summary>Sets the error location assembling delegate to the default value.
        /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected override void SetDefaultReportLocationDlg()
        // $A Igor Oct08;
        {
             ReportLocationDlg = new ReportLocationDelegate(DefaultLocationString);
        }

        /// <summary>Sets the error message assembling delegate to the default value.
        /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected override void SetDefaultReportMessageDlg()
        // $A Igor Oct08;
        {
            ReportMessageDlg = new ReportMessageDelegate(DefaultMessageString);
        }

        /// <summary>Sets the reserve error reporting delegate to the default value.
        /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected override void SetDefaultReserveReportErrorDlg()
        // $A Igor Oct08;
        {
             ReserveReportErrorDlg = new ReserveReportErrorDelegate(DefaultReserveReportErrorForms);
        }


        #endregion  // Initialization



        #region Erorr_Reporting_Global

        // Global error reporter:

        // TODO: consider whether initialization is nexessary here (maybe it is better to allow only explicit initialization).
        private static ReporterForms _Global = new ReporterForms();
        private static bool _GlobalInitialized = false;

        public new static bool GlobalInitialized
        {
            get { return _GlobalInitialized; }
        }

        /// <summary>Gets the global reporter object.
        /// This is typically used for configuring the global reporter.</summary>
        public static new ReporterForms Global
        // $A Igor Oct08;
        {
            get
            {
                if (_Global == null)
                {
                    if (!_GlobalInitialized)
                    {
                        _Global = new ReporterForms();
                        _GlobalInitialized = true;
                        _Global.IsGlobal = true;
                    }
                    if (_Global == null)
                    {
                        // Default reserve error reporting method is used because we do not have a
                        // reporter object reference here:
                        DefaultReserveReportError(null, ReportType.Error, "Reporter.Global", "Global reporting system is not initialized.", null, null);
                    }
                }
                return _Global;
            }
            protected set { _Global = value; }
        }

        #endregion Erorr_Reporting_Global


        #region Default_General_Methods

        // Default function function for reserve error reporting (called if an exception is thrown in an error reporting function):

        private static int NumShownDialog = 0, MaxShownDialog = 3;

        /// <summary>Default function for reserve error reporting (called if an exception is thrown in an error reporting function):</summary>
        /// <param name="reporter">Reporter object whre the method can get additional information.</param>
        /// <param name="messagetype">Level of the message (Error, Warning,Info, etc.)</param>
        /// <param name="location">Location string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="message">Error message string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="ex">Original exception that was being reported when the error reporting function threw an exception.</param>
        /// <param name="ex1">Exception thrown by the error reporting function.</param>
        public static void DefaultReserveReportErrorForms(ReporterBase reporter, ReportType messagetype,
                string location, string message, Exception ex, Exception ex1)
        // $A Igor Oct08;
        {
            string msg = "";
            // Assemble the message that will beshown in the reserve error report:
            try
            {
                msg = DefaultReserveReportMessage(reporter, messagetype,
                        location, message, ex, ex1);
            }
            catch { }
            // Show the message in several ways:
            try
            {
                // Output the message to a console (if the application has it):
                Console.Write(msg);
            }
            catch { }
            try
            {
                // Show the message as a fading message:
                FadingMessage fm = new FadingMessage(false /* launch */);
                fm.BackGroundColor = Color.Pink;
                fm.FadeColor = Color.LightYellow;
                fm.ForeGroundColorTitle = Color.Blue;
                fm.ForeGroundColorMsg = Color.Black;
                fm.ShowTime = 8000;
                fm.FadingTimePortion = 0.5;
                string title = "ERROR in the reporting system!";
                fm.MsgTitle = title;
                fm.MsgText = msg;
                fm.Launch(true /* parallelThread */);
            }
            catch{}
            try
            {
                // Show the message in the global console consform:
                ConsoleForm cons = UtilForms.AppConsole;
                if (cons != null)
                {
                    cons.Write(msg);
                }
            }
            catch{}
            try
            {
                // Show the message in a message box (but only a couple of times):
                ++NumShownDialog;
                if (NumShownDialog <= MaxShownDialog || MaxShownDialog == 0)
                {
                    string title = "ERROR in the reporting system", text = "";
                    text += msg;
                    if (NumShownDialog == MaxShownDialog)
                        text += Environment.NewLine + Environment.NewLine + 
                                "Showing further messages of this kind in a dialog box will be omitted." + Environment.NewLine;
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBoxIcon icon = MessageBoxIcon.Error;
                    DialogResult dlgres = MessageBox.Show(text, title, buttons, icon);
                }
            }
            catch{}
        }


        #endregion // Default_General_Methods


        #region Reporting
        // ACTUAL REPORTING METHODS:

        // private object _lockobj = new Object();     // enables thread locking

        //public override object lockobj
        //{
        //    get { return _lockobj; }
        //}



        #region Reporting_Basic
        // BASIC REPORTING METHODS:

        // Last resort error reporting function (bulletproof, called if an exception is thrown inside a reporting function):

        /// <summary>Used to report errors within reporting functions.
        /// Designed to be bullet proof in order to ensure that improper behavior of the reporting system does not remain unnoticed.
        /// Overrides the method of the base class in order to show the message in forms rather than just in a console.</summary>
        /// <param name="messagetype"></param>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="ex1"></param>
        protected override void ReserveReportErrorSpecific(ReportType messagetype, string location,
                    string message, Exception ex, Exception ex1)
        // $A Igor Oct08;
        {
            try
            {
                lock (lockobj)
                {
                    if (ReserveReportErrorDlg != null)
                    {
                        ReserveReportErrorDlg(this, messagetype, location, message, ex, ex1);
                    }
                    else
                        DefaultReserveReportErrorForms(this, messagetype, location, message, ex, ex1);
                }
            }
            catch (Exception)
            {
                DefaultReserveReportErrorForms(this, messagetype, location, message, ex, ex1);
            }
        }

        // OTHER REPORTING METHODS (for all kinds of reports):
        // The basic reporting method is overridden while all s methods are inherited from the base class
        // and call that basic method, which is overridden in this class.

        /// <summary>Basic method for reporting by this class (i.e., reporting by forms).</summary>
        /// <param name="reporter">Reporter object where additional data can be found.</param>
        /// <param name="messagetype">The type of the report (e.g. Error, Warning, etc.).</param>
        /// <param name="location">User-provided description of error location.</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception thrown when error occurred.</param>
        public override void Report(ReportType messagetype, string location, string message, Exception ex)
        // $A Igor Oct08;
        {
            if (message == null)
                message = "";
            try
            {
                lock (lockobj)
                {
                    try
                    {
                        if (ReportDlg != null)
                        {
                            // First report by the main delegate if it is initialized:
                            string locationstring = "", messagestring = "";
                            if (ReportLocationDlg != null)
                                locationstring = ReportLocationDlg(this, messagetype, location, ex);
                            else
                            {
                                // No delegate for assembling location string:
                                if (!string.IsNullOrEmpty(location))
                                    locationstring += location;
                            }
                            if (ReportMessageDlg != null)
                                messagestring = ReportMessageDlg(this, messagetype, message, ex);
                            else
                            {
                                // No delegate for assembling message string:
                                if (!string.IsNullOrEmpty(messagestring))
                                {
                                    messagestring += message;
                                    if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                                            messagestring += " Details: ";
                                }
                                if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                                        messagestring += ex.Message;
                            }
                            if (ReportDlg != null)
                                ReportDlg(this, messagetype, locationstring, messagestring);
                            else
                                throw new Exception("Main reporting delegate is not specified.");
                        }
                    }
                    catch (Exception ex1)
                    {
                        this.ReserveReportError(ReportType.Error, location,
                            "Error in Report. " + message, ex, ex1);
                    }
                    if (this.UseConsoleForm)
                    {
                        try
                        {
                            this.Report_ConsoleForm(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Report_ConsoleForm. " + message, ex, ex1);
                        }
                    }
                    if (this.UseFadeMessage)
                    {
                        try
                        {
                            this.Report_FadeMessage(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Report_FadeMessage. " + message, ex, ex1);
                        }
                    }
                    if (this.UseConsole)
                    {
                        try
                        {
                            this.Report_Console(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Report_Console. " + message, ex, ex1);
                        }
                    }
                    if (this.UseMessageBox)
                    {
                        try
                        {
                            this.Report_MessageBox(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Report_MessageBox. " + message, ex, ex1);
                        }
                    }
                    if (ThrowTestException)
                    {
                        // Throw a test exception:
                        throw new Exception("Test exception thrown by the reporter (after reporting has been performted).");
                    }
                }  // lock
            }
            catch (Exception ex1)
            {
                ThrowTestException = false;  // re-set, such that a test exception is thrown only once
                DefaultReserveReportErrorForms(null, ReportType.Error, location, message, ex, ex1);
            }

        }


        #endregion  // Reporting_Basic



        #region Reporting_Specific


        #region Reporting_Console_Form

        // REPORTING VIA CONSOLE FORM:

        private bool _UseConsoleForm = false;

        /// <summary>Gets or sets the flag specifying whether reporting using a console form is performed or not.</summary>
        public bool UseConsoleForm { get { return _UseConsoleForm; } set { _UseConsoleForm = value; } }

        //Data: 

        /// <summary>Console consform used for reporting.
        /// If null then global console consform is used (this is the matter of delegate methods).</summary>
        public ConsoleForm ConsoleForm = null;

        // Delegates with default values:

        /// <summary>Delegate that performs error reporting via console consform.
        /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
        /// assemble error message. Then it uses both to assemble the final decorated error message and launches
        /// it in its own way.</summary>
        public ReportDelegate ReportDlgConsoleForm = new ReportDelegate(DefaultReport_ConsoleForm);
        
        /// <summary>Delegate that assembles the error location string for reporting via console consform.</summary>
        public ReportLocationDelegate ReportLocationDlgConsoleForm = new ReportLocationDelegate(DefaultReportLocation_ConsoleForm);

        /// <summary>Delegate that assembles the eror message string for reporting via console consform.</summary>
        public ReportMessageDelegate ReportMessageDlgConsoleForm = new ReportMessageDelegate(DefaultReportMessage_ConsoleForm);

        // Default delegate methods for reporting via console consform:

        /// <summary>Delegate for launching a report via console form.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_ConsoleForm(ReporterBase reporter, ReportType messagetype,
            string location, string message)
        {
            ReporterForms rep=reporter as ReporterForms;
            ConsoleForm consform = null;
            if (rep != null)
            {
                consform = rep.ConsoleForm;
                if (consform != null)
                    if (consform.IsDisposed)
                        consform = null;
            }
            if (consform == null)
            {
                consform = UtilForms.AppConsole;
                if (consform != null)
                    if (consform.IsDisposed)
                        consform = null;
            }
            if (consform==null)
                throw new Exception("Console form to be used for rporting is not specified or is not in active state.");
            // This uses the ConsoleForm class' own reporting method.
            consform.Report(messagetype, location, message);
        }

        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_ConsoleForm(ReporterBase reporter, ReportType messagetype,
                string location, Exception ex)
        {
            return DefaultLocationString(reporter, messagetype, location, ex);
        }

        /// <summary>Delegate for assembling a report message for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="basicmessage">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Message string that can be used in a report.</returns>
        protected static string DefaultReportMessage_ConsoleForm(ReporterBase reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }

        // Methods for reporting via console consform:

        /// <summary>Launches a report via console consform.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_ConsoleForm(ReportType messagetype, string location, string message, Exception ex)
        {
            string locationstring = "", messagestring = "";
            if (ReportLocationDlgConsoleForm != null)
                locationstring = ReportLocationDlgConsoleForm(this, messagetype, location, ex);
            else if (ReportLocationDlg != null)
                locationstring = ReportLocationDlg(this, messagetype, location, ex);
            else
            {
                // No delegate for assembling location string:
                if (!string.IsNullOrEmpty(location))
                    locationstring += location;
            }
            if (ReportMessageDlgConsoleForm != null)
                messagestring = ReportMessageDlgConsoleForm(this, messagetype, message, ex);
            else if (ReportMessageDlg != null)
                messagestring = ReportMessageDlg(this, messagetype, message, ex);
            else
            {
                // No delegate for assembling message string:
                if (!string.IsNullOrEmpty(messagestring))
                {
                    messagestring += message;
                    if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                            messagestring += " Details: ";
                }
                if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                        messagestring += ex.Message;
            }
            if (ReportDlgConsoleForm != null)
                ReportDlgConsoleForm(this, messagetype, locationstring, messagestring);
            else
                throw new Exception("Console form reporting delegate is not specified.");
        }


        #endregion Reporting_Console_Form


        #region Reporting_Fading_Message
        // REPORTING BY A FADING MESSAGE:

        private bool _UseFadeMessage = false;

        /// <summary>Gets or sets the flag specifying whether reporting using a fadinfg messagebox is performed or not.</summary>
        public bool UseFadeMessage { get { return _UseFadeMessage; } set { _UseFadeMessage = value; } }

        //Data: 

            /// <summary>Whether the message is launched as a topmost window, so that clicking on other windows 
            /// will not hide them as they are displayed in front of windows with current focus.</summary>
        public bool FadeMessageIsTopmostWindow = true;

        /// <summary>Whether eventual fading messages are launched at mouse cursot.</summary>
        public bool FadeMessageLaunchAtMouseCursor = false;

        /// <summary>Whether fading messages are launched in the current thread.
        /// <para>When false, each fading message is launched in a separate thread, i.e. a new thread is created
        /// for each message. This consumes system resources (but will usually not be noticeable on current systems),
        /// but the big advantage is that messages are responsive when GUI thread is blocked.</para>
        /// <para>One of the advantages of launching a message in the same GUI thread is thar message content
        /// can be copied to clipboard through fading message' context menu (right-click). This can not be done 
        /// with messages launched in parallel thread due to clipboard limitations.</para>
        /// <para>Messages are launched in the same thread only when this thread is a GUI thread with a message loop.
        /// Currently, the thread must also be the main GUI thread (it is possible that this will not change in
        /// the future because other GUI threads can exit in any time, which would abort the fading message).</para></summary>
        public bool LaunchInSameThread = true;

        /// <summary>Fading message visibility time in milliseconds.</summary>
        public int FadeMessageShowtime = 5000;

        /// <summary>Portion of visibility time when the message is fading.</summary>
        public double FadeMessageFadingTimePortion = 0.25;

        /// <summary>Active background color of the fading message.</summary>
        public Color FadeMessageBackColor = Color.Orange;

        /// <summary>Active background color of the fading message for Info messages.</summary>
        public Color FadeMessageBackColorInfo = Color.Beige;

        /// <summary>Active background color of the fading message for Info messages.</summary>
        public Color FadeMessageBackColorWarning = Color.Orchid;

        /// <summary>Active background color of the fading message for Info messages.</summary>
        public Color FadeMessageBackColorError = Color.Tomato;



        /// <summary>Final (fade) background color of the fading message.</summary>
        public Color FadeMessageBackColorFinal = Color.DarkGray;

        /// <summary>Text color of the fading message.</summary>
        public Color FadeMessageForeColor = Color.Blue;


        // Delegates with default values:

        /// <summary>Delegate that performs error reporting via fading message.
        /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
        /// assemble error message. Then it uses both to assemble the final decorated error message and launches
        /// it in its own way.</summary>
        public ReportDelegate ReportDlgFadeMessage = new ReportDelegate(DefaultReport_FadeMessage);

        /// <summary>Delegate that assembles the error location string for reporting via fading message.</summary>
        public ReportLocationDelegate ReportLocationDlgFadeMessage = new ReportLocationDelegate(DefaultReportLocation_FadeMessage);

        /// <summary>Delegate that assembles the eror message string for reporting via fading message.</summary>
        public ReportMessageDelegate ReportMessageDlgFadeMessage = new ReportMessageDelegate(DefaultReportMessage_FadeMessage);

        // Default delegate methods for reporting via fading message:

        /// <summary>Delegat for launching a report via fading message.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_FadeMessage(ReporterBase reporter, ReportType messagetype,
            string location, string message)
        {
            ReporterForms rep = reporter as ReporterForms;
            if (rep == null)
                throw new ArgumentException("The reporter argument is not specified or is invalid (possible up-casting failure).");
            FadingMessage fm = new FadingMessage(false);
            if (rep != null)
            {
                fm.BackGroundColor = rep.FadeMessageBackColor;
                fm.FadeColor = rep.FadeMessageBackColorFinal;
                fm.ForeGroundColorMsg = fm.ForeGroundColorTitle = rep.FadeMessageForeColor;
                fm.ShowTime = rep.FadeMessageShowtime;
                fm.FadingTimePortion = rep.FadeMessageFadingTimePortion;
                fm.IsTopMostWindow = rep.FadeMessageIsTopmostWindow;
                fm.LaunchedAtMouseCursor = rep.FadeMessageLaunchAtMouseCursor;
            }
            string title = "", text = "";
            switch (messagetype)
            {
                case ReportType.Error:
                    fm.BackGroundColor = rep.FadeMessageBackColorError;
                    title = "  ERROR! ";
                    fm.IsTopMostWindow = true;
                    break;
                case ReportType.Warning:
                    fm.BackGroundColor = rep.FadeMessageBackColorWarning;
                    title = "  Warning!";
                    fm.IsTopMostWindow = true;
                    break;
                default: 
                    fm.BackGroundColor = rep.FadeMessageBackColorInfo;
                    title = "Info:";
                    break;
            }
            if (!string.IsNullOrEmpty(location))
                text += location + ": " + Environment.NewLine;
            if (!string.IsNullOrEmpty(message))
                text += message;
            fm.MsgTitle = title;
            fm.MsgText = text;
            // fm.ShowThread(title, text);

            bool launchInParallelThread = !rep.LaunchInSameThread;
            if (!launchInParallelThread)
            {
                // If this is not the main GUI thread then te message will not be launched in the same thread:
                if (!UtilForms.IsMainGuiThread())
                    launchInParallelThread = true;
            }
            fm.Launch(launchInParallelThread);
        }

        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_FadeMessage(ReporterBase reporter, ReportType messagetype,
                string location, Exception ex)
        {
            return DefaultLocationString(reporter, messagetype, location, ex);
        }

        /// <summary>Delegate for assembling a report message for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="basicmessage">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Message string that can be used in a report.</returns>
        protected static string DefaultReportMessage_FadeMessage(ReporterBase reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }

        // Methods for reporting via fading message:

        /// <summary>Launches a report via fading message.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_FadeMessage(ReportType messagetype, string location, string message, Exception ex)
        {
            string locationstring = "", messagestring = "";
            if (ReportLocationDlgFadeMessage != null)
                locationstring = ReportLocationDlgFadeMessage(this, messagetype, location, ex);
            else if (ReportLocationDlg != null)
                locationstring = ReportLocationDlg(this, messagetype, location, ex);
            else
            {
                // No delegate for assembling location string:
                if (!string.IsNullOrEmpty(location))
                    locationstring += location;
            }
            if (ReportMessageDlgFadeMessage != null)
                messagestring = ReportMessageDlgFadeMessage(this, messagetype, message, ex);
            else if (ReportMessageDlg != null)
                messagestring = ReportMessageDlg(this, messagetype, message, ex);
            else
            {
                // No delegate for assembling message string:
                if (!string.IsNullOrEmpty(messagestring))
                {
                    messagestring += message;
                    if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                            messagestring += " Details: ";
                }
                if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                        messagestring += ex.Message;
            }
            if (ReportDlgFadeMessage != null)
                ReportDlgFadeMessage(this, messagetype, locationstring, messagestring);
            else
                throw new Exception("Fading message reporting delegate is not specified.");
        }

        #endregion  // Reporting_Fading_Message


        #region Reporting_Console
        // REPORTING BY A CONSOLE:

        private bool _UseConsole = false;

        /// <summary>Gets or sets the flag specifying whether reporting using a system console is performed or not.</summary>
        public bool UseConsole { get { return _UseConsole; } set { _UseConsole = value; } }

        //Data: 

        // Delegates with default values:

        /// <summary>Delegate that performs error reporting via console.
        /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
        /// assemble error message. Then it uses both to assemble the final decorated error message and launches
        /// it in its own way.</summary>
        public ReportDelegate ReportDlgConsole = new ReportDelegate(DefaultReport_Console);

        /// <summary>Delegate that assembles the error location string for reporting via console.</summary>
        public ReportLocationDelegate ReportLocationDlgConsole = new ReportLocationDelegate(DefaultReportLocation_Console);

        /// <summary>Delegate that assembles the eror message string for reporting via console.</summary>
        public ReportMessageDelegate ReportMessageDlgConsole = new ReportMessageDelegate(DefaultReportMessage_Console);

        // Default delegate methods for reporting via console:

        /// <summary>Delegat for launching a report via console.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_Console(ReporterBase reporter, ReportType messagetype,
            string location, string message)
        {
            ReporterForms rep = reporter as ReporterForms;
            if (rep == null)
                throw new ArgumentException("The reporter argument is not specified or is invalid (possible up-casting failure).");
            DefaultReportConsole(rep, messagetype, location, message);
        }

        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_Console(ReporterBase reporter, ReportType messagetype,
                string location, Exception ex)
        {
            return DefaultLocationString(reporter, messagetype, location, ex);
        }


        /// <summary>Delegate for assembling a report message for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="basicmessage">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Message string that can be used in a report.</returns>
        protected static string DefaultReportMessage_Console(ReporterBase reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }

        // Methods for reporting via console:

        /// <summary>Launches a report via console.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_Console(ReportType messagetype, string location, string message, Exception ex)
        {
            string locationstring = "", messagestring = "";
            if (ReportLocationDlgConsole != null)
                locationstring = ReportLocationDlgConsole(this, messagetype, location, ex);
            else if (ReportLocationDlg != null)
                locationstring = ReportLocationDlg(this, messagetype, location, ex);
            else
            {
                // No delegate for assembling location string:
                if (!string.IsNullOrEmpty(location))
                    locationstring += location;
            }
            if (ReportMessageDlgConsole != null)
                messagestring = ReportMessageDlgConsole(this, messagetype, message, ex);
            else if (ReportMessageDlg != null)
                messagestring = ReportMessageDlg(this, messagetype, message, ex);
            else
            {
                // No delegate for assembling message string:
                if (!string.IsNullOrEmpty(messagestring))
                {
                    messagestring += message;
                    if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                            messagestring += " Details: ";
                }
                if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                        messagestring += ex.Message;
            }
            if (ReportDlgConsole != null)
                ReportDlgConsole(this, messagetype, locationstring, messagestring);
            else
                throw new Exception("Console form reporting delegate is not specified.");
        }

        #endregion Reporting_Console


        #region Reporting_Message_Box
        //  REPORTING BY A MESSAGE BOX:

        private bool _UseMessageBox = false;

        /// <summary>Gets or sets the flag specifying whether reporting using a message box is performed or not.</summary>
        public bool UseMessageBox { get { return _UseMessageBox; } set { _UseMessageBox = value; } }



        //Data: 

        // Delegates with default values:

        /// <summary>Delegate that performs error reporting via message box.
        /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
        /// assemble error message. Then it uses both to assemble the final decorated error message and launches
        /// it in its own way.</summary>
        public ReportDelegate ReportDlgMessageBox = new ReportDelegate(DefaultReport_MessageBox);

        /// <summary>Delegate that assembles the error location string for reporting via message box.</summary>
        public ReportLocationDelegate ReportLocationDlgMessageBox = new ReportLocationDelegate(DefaultReportLocation_MessageBox);

        /// <summary>Delegate that assembles the eror message string for reporting via message box.</summary>
        public ReportMessageDelegate ReportMessageDlgMessageBox = new ReportMessageDelegate(DefaultReportMessage_MessageBox);

        // Default delegate methods for reporting via message box:

        /// <summary>Delegat for launching a report via message box.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_MessageBox(ReporterBase reporter, ReportType messagetype,
            string location, string message)
        {
            ReporterForms rep = reporter as ReporterForms;
            if (rep == null)
                throw new ArgumentException("The reporter argument is not specified or is invalid (possible up-casting failure).");
            string title = "", text = "";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Exclamation;

            switch (messagetype)
            {
                case ReportType.Error:
                    title = "  ERROR! ";
                    icon = MessageBoxIcon.Error;
                    break;
                case ReportType.Warning:
                    title = "  Warning!";
                    icon = MessageBoxIcon.Warning;
                    break;
                default:
                    title = "Info:";
                    icon = MessageBoxIcon.Information;
                    break;
            }
            if (!string.IsNullOrEmpty(location))
                text += location + ": " + Environment.NewLine;
            if (!string.IsNullOrEmpty(message))
                text += message;
            DialogResult dlgres = MessageBox.Show(text, title, buttons, icon);
        }

        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_MessageBox(ReporterBase reporter, ReportType messagetype,
                string location, Exception ex)
        {
            return DefaultLocationString(reporter, messagetype, location, ex);
        }

        /// <summary>Delegate for assembling a report message for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="basicmessage">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Message string that can be used in a report.</returns>
        protected static string DefaultReportMessage_MessageBox(ReporterBase reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }

        // Methods for reporting via message box:

        /// <summary>Launches a report via message box.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_MessageBox(ReportType messagetype, string location, string message, Exception ex)
        {
            string locationstring = "", messagestring = "";
            if (ReportLocationDlgMessageBox != null)
                locationstring = ReportLocationDlgMessageBox(this, messagetype, location, ex);
            else if (ReportLocationDlg != null)
                locationstring = ReportLocationDlg(this, messagetype, location, ex);
            else
            {
                // No delegate for assembling location string:
                if (!string.IsNullOrEmpty(location))
                    locationstring += location;
            }
            if (ReportMessageDlgMessageBox != null)
                messagestring = ReportMessageDlgMessageBox(this, messagetype, message, ex);
            else if (ReportMessageDlg != null)
                messagestring = ReportMessageDlg(this, messagetype, message, ex);
            else
            {
                // No delegate for assembling message string:
                if (!string.IsNullOrEmpty(messagestring))
                {
                    messagestring += message;
                    if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                            messagestring += " Details: ";
                }
                if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                        messagestring += ex.Message;
            }
            if (ReportDlgMessageBox != null)
                ReportDlgMessageBox(this, messagetype, locationstring, messagestring);
            else
                throw new Exception("Message box reporting delegate is not specified.");
        }

        #endregion   // Reporting_Message_Box


        #endregion  // Reporting_Specific


        #endregion   // Reporting



    }   // Class ReporterForms

}  // namespace IG.Forms
