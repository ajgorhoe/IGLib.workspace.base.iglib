using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;


using IG.Lib;

namespace IG.Forms
{


    #region Interfaces

    public interface IReporterMessageBox : IReporter
    {
        bool UseMessageBox { get; set; }
    }


    public interface IReporterFadeMessage : IReporter
    {
        bool UseFadeMessage { get; set; }
    }

    public interface IReporterConsoleForm : IReporter
    {
        bool UseConsoleForm { get; set; }
    }

    public interface IReporterSpeech : IReporter
    {
        bool UseSpeech { get; set; }

        /// <summary>Gets or sets the reporting level for speaking out signals (such as "Warning", "Error", "Information", etc.)</summary>
        ReportLevel SpeechLevelSignal { get; set; }

        /// <summary>Gets or sets the reporting level for speaking out message text.</summary>
        ReportLevel SpeechLevelMessage { get; set; }

    }

    #endregion  // Interfaces



    #region Base_Classes

    // Mainly abstract classes that contain implementation of significant portions of code, which will prevent
    // code duplication.


    /// <summary>Base class for reporter classes that contain either reporting via system console, 
    /// reporting via message box, or both.</summary>
    /// <remarks>Code common to all classes is put here. This is to reduce duplication of code as much as possible, while
    /// at the same time achieving pure behavior: a class intended for reporting via message boxes but not via console
    /// does not enable reporting in a console; the class intended for reporting only via message box does not enable
    /// reporting via co nsole; but still there can be a class that enables both, with only small portions of code
    /// duplicated form both classes.
    /// Interfaces are intended to clearly distinguish between various functionality supported.</remarks>
    public abstract class ReporterConsoleMsgbox_Base : ReporterConsole_Base
    {


        #region Reporter_ReadConfiguration
        // Reading of reporter settings from configuration files

        protected const string
            KeyUseMessageBox = "UseMessageBox";

        /// <summary>Reads settings for a specified named group of reporters from the application configuration file.</summary>
        /// <param name="groupname">Name of the group of reporters for which the settings apply.</param>
        protected override void ReadAppSettingsBasic(string groupname)
        // $A Igor Feb09;
        {
            base.ReadAppSettingsBasic(groupname);
            bool assigned, boolval = false;
            GetAppSetting(groupname, KeyUseMessageBox, ref boolval, out assigned);
            if (assigned) UseMessageBox = boolval;
        }

        #endregion // Reporter_ReadConfiguration


        #region Reporting_General

        // GENERAL REPORTING METHODS (for all kinds of reports):


        public static void DefaultReserveReportError(ReporterConsoleMsgbox_Base reporter, ReportType messagetype,
                string location, string message, Exception ex, Exception ex1)
        // $A Igor Oct08;
        {
            string msg = "";
            try
            {
                // Create decorated reserve error report's message:
                msg = DefaultReserveReportMessage(reporter, messagetype,
                        location, message, ex, ex1);
            }
            catch { }
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    msg += Environment.NewLine + Environment.NewLine;
                    msg += "ERROR: " + Environment.NewLine;
                    if (!string.IsNullOrEmpty(location))
                        msg += "Location: " + location + Environment.NewLine;
                    if (!string.IsNullOrEmpty(message))
                        msg += "User provided message: " + message + Environment.NewLine;
                    if (ex!=null)
                        if (!string.IsNullOrEmpty(ex.Message))
                            msg+="Original exception message: " + ex.Message + Environment.NewLine;
                    if (ex1!=null)
                        if (!string.IsNullOrEmpty(ex1.Message))
                            msg+="Internal exception message: " + ex1.Message + Environment.NewLine;
                    msg+=Environment.NewLine + Environment.NewLine;
                }
                catch { }
            }
            try
            {
                // Output the message to a console:
                Console.Write(msg);
            }
            catch { }
            try
            {
                //// Output the message to a message box - this is done only if the reporter uses the message box:
                if (reporter.UseMessageBox) 
                    MessageBox.Show(msg);
            }
            catch { }
            try
            {
                // Output the message to output streams and files registered with Reporter's TextWriter:
                int numerrors = reporter.TextWriterWrite(msg);
            }
            catch { }
            try
            {
                // Output the message to output streams and files registered with Reporter's TextLogger:
                int numerrors = reporter.TextLoggerWrite(msg);
            }
            catch { }
        }

        /// <summary>Reportinf of internal reporter errors for the specific reporter class (overridden in derived classes).
        /// This method is called for internal error reports when the delegate for this job is not defined.</summary>
        /// <param name="messagetype"></param>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="ex1"></param>
        protected override void ReserveReportErrorSpecific(ReportType messagetype, string location,
                    string message, Exception ex, Exception ex1)
        {
            DefaultReserveReportError(this,messagetype, location, message, ex, ex1);
        }

        /// <summary>Basic reporting method (overloaded). Launches an error report, a warning report or other kind of report/message.
        /// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
        /// are obtained from the class' instance.</summary>
        /// <param name="messagetype">The type of the report (e.g. Error, Warning, etc.).</param>
        /// <param name="location">User-provided description of error location.</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception thrown when error occurred.</param>
        public override void Report(ReportType messagetype, string location, string message, Exception ex)
        // $A Igor Oct08;
        {
            try
            {
                lock (lockobj)
                {
                    try
                    {
                        // First, call reporting form a base class (this includes calling reporting delegates
                        // and inherited specific methods):
                        base.Report(messagetype, location, message, ex);
                    }
                    catch (Exception ex1)
                    {
                        this.ReserveReportError(ReportType.Error, location,
                            "Error in Reporter.base.Report. " + message, ex, ex1);
                    }
                    // Then, call specific methods from this class:
                    if (UseMessageBox && DoReporting(messagetype))
                    {
                        try
                        {
                            this.Report_MessageBox(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Reporter.Report_MessageBox. " + message, ex, ex1);
                        }
                    }
                }
            }
            catch (Exception ex1)
            {
                ThrowTestException = false;  // re-set, such that a test exception is thrown only once
                ReserveReportError(ReportType.Error, location, message, ex, ex1);
            }
        }


        #endregion  // Reporting_General


        #region Reporting_MessageBox
        //  REPORTING BY MESSAGE BOX:

        // This subsystem enables launching message boxes when messages are launched, or even reading of message contents.

        /// <summary>Gets or sets the flag specifying whether reporting using a message box is performed or not.</summary>
        public virtual bool UseMessageBox { get; set; }

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
            ReporterConsoleMsgbox_Base rep = reporter as ReporterConsoleMsgbox_Base;
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
                case ReportType.Info:
                    title = "Info:";
                    icon = MessageBoxIcon.Information;
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

        #endregion   // Reporting_MessageBox

    }  // class ReporterConsoleMsgbox_Base


    #endregion  // Base_Classes




    #region Concrete_Classes


    public class ReporterConsoleMsgbox : ReporterConsoleMsgbox_Base, IReporterConsole, IReporterMessageBox
    {


        #region Initialization

        #region Constructors

        // Constructors - this region will be literally included in derived classes, except in special cases):

        /// <summary>Constructor. Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        public ReporterConsoleMsgbox()
        // $A Igor Oct08;
        {
            Init();
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Delegates that are not specified are set to default values.</summary>
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
        public ReporterConsoleMsgbox(object obj,
            ReportDelegate reportdelegate,
            ReportLocationDelegate locationdelegate,
            ReportMessageDelegate messagedelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
            Init(obj, reportdelegate, locationdelegate, messagedelegate, reservereportdelegate);
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates that are not specified are set to default values.</summary>
        public ReporterConsoleMsgbox(object obj,
            ReportDelegate reportdelegate,
            ReportLocationDelegate locationdelegate,
            ReportMessageDelegate messagedelegate)
        // $A Igor Oct08;
        {
                Init(obj, reportdelegate, locationdelegate, messagedelegate );
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and the delegate to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates for assembling the error location string and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        public ReporterConsoleMsgbox(object obj, ReportDelegate reportdelegate)
        // $A Igor Oct08;
        {
                Init(obj, reportdelegate);
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
        public ReporterConsoleMsgbox(object obj,
            ReportDelegate reportdelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
                Init(obj, reportdelegate, reservereportdelegate);
        }

        #endregion  // Constructors


        #region Initialization_Overridden

        // Setting default error reporting behavior, these methods can be overridden in derived classes:

        // Default delegates are the same as for base cls.

        /// <summary>Initial part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected override void InitBegin()
        // $A Igor Oct08;
        {
            try
            {
                base.InitBegin();
            }
            catch { }
        }

        /// <summary>Finalizing part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected override void InitEnd()
        // $A Igor Oct08;
        {
            try
            {
                base.InitEnd();
                UseMessageBox = true;
                UseConsole = true;
                UseTextLogger = true;
                UseTextWriter = true;
                UseTrace = false;
            }
            catch { }
        }

        #endregion  // Initialization_Overridden

        #endregion  // Initialization


        #region Reporting_Global

        // Global error reporter of this class:
        // In derived classes, this block should be repeated, only with classs name of _Global and Global set to the
        // current (derived) class and with keyword new in property declarations.

        private static ReporterConsoleMsgbox _Global = null;
        private static bool _GlobalInitialized = false;
        private static object GlobalLock = new object();

        public static new bool GlobalInitialized
        {
            get { return _GlobalInitialized; }
        }

        /// <summary>Gets the global reporter object.
        /// This is typically used for configuring the global reporter.</summary>
        public static new ReporterConsoleMsgbox Global
        // $A Igor Oct08;
        {
            get
            {
                if (_Global == null)
                {
                    lock (GlobalLock)  // we need a lock only in initialization; otherwise global reporter is immutable
                    {
                        if (_Global == null && !_GlobalInitialized)
                        {
                            _Global = new ReporterConsoleMsgbox();
                            _Global.IsGlobal = true;
                            if (ReadGlobalAppSettings)
                            {
                                _Global.ReadAppSettings();
                                _Global.ReadAppSettings("Global");
                            }
                            _GlobalInitialized = true;
                        }
                        if (_Global == null)
                        {
                            // Default reserve error reporting method is used because we do not have a
                            // reporter object reference here:
                            DefaultReserveReportError(null, ReportType.Error, "Reporter.Global", "Global reporting system is not initialized.", null, null);
                        }
                    }
                }
                return _Global;
            }
            private set
            {
                lock (GlobalLock)
                {
                    _Global = value;
                    if (value != null)
                        _GlobalInitialized = true;
                    else
                        _GlobalInitialized = false;
                }
            }
        }

        #endregion Reporting_Global


    }  //  class ReporterConsoleMsgbox



    public class ReporterMsgbox : ReporterConsoleMsgbox_Base, IReporterMessageBox
    {

        #region Initialization

        #region Constructors

        // Constructors - this region will be literally included in derived classes, except in special cases):

        /// <summary>Constructor. Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        public ReporterMsgbox()
        // $A Igor Oct08;
        {
            Init();
        }


        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Delegates that are not specified are set to default values.</summary>
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
        public ReporterMsgbox(object obj,
            ReportDelegate reportdelegate,
            ReportLocationDelegate locationdelegate,
            ReportMessageDelegate messagedelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
            Init(obj, reportdelegate, locationdelegate, messagedelegate, reservereportdelegate);
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates that are not specified are set to default values.</summary>
        public ReporterMsgbox(object obj,
            ReportDelegate reportdelegate,
            ReportLocationDelegate locationdelegate,
            ReportMessageDelegate messagedelegate)
        // $A Igor Oct08;
        {
                Init(obj, reportdelegate, locationdelegate, messagedelegate );
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and the delegate to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates for assembling the error location string and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        public ReporterMsgbox(object obj, ReportDelegate reportdelegate)
        // $A Igor Oct08;
        {
                Init(obj, reportdelegate);
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
        public ReporterMsgbox(object obj,
            ReportDelegate reportdelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
                Init(obj, reportdelegate, reservereportdelegate);
        }

        #endregion  // Constructors

        #region Initialization_Overridden

        // Setting default error reporting behavior, these methods can be overridden in derived classes:

        // Default delegates are the same as for base cls.

        /// <summary>Initial part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected override void InitBegin()
        // $A Igor Oct08;
        {
            ReportConsoleSet = false;  // switch off reporting when we want to set the console
            try
            {
                base.InitBegin();
            }
            catch { }
        }

        /// <summary>Finalizing part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected override void InitEnd()
        // $A Igor Oct08;
        {
            try
            {
                base.InitEnd();
                UseMessageBox = true;
                UseConsole = false;
                UseTextLogger = true;
                UseTextWriter = true;
                UseTrace = false;
            }
            catch { }
            finally
            {
                ReportConsoleSet = true;  // switch this back on (it was switched off in InitBegin())
            }
        }

        #endregion  // Initialization_Overridden

        #endregion  // Initialization


        #region Reporting_Global

        // Global error reporter of this class:
        // In derived classes, this block should be repeated, only with classs name of _Global and Global set to the
        // current (derived) class and with keyword new in property declarations.

        private static ReporterMsgbox _Global = null;
        private static bool _GlobalInitialized = false;
        private static object GlobalLock = new object();

        public static new bool GlobalInitialized
        {
            get { return _GlobalInitialized; }
        }

        /// <summary>Gets the global reporter object.
        /// This is typically used for configuring the global reporter.</summary>
        public static new ReporterMsgbox Global
        // $A Igor Oct08;
        {
            get
            {
                if (_Global == null)
                {
                    lock (GlobalLock)  // we need a lock only in initialization; otherwise global reporter is immutable
                    {
                        if (_Global == null && !_GlobalInitialized)
                        {
                            _Global = new ReporterMsgbox();
                            _Global.IsGlobal = true;
                            if (ReadGlobalAppSettings)
                            {
                                _Global.ReadAppSettings();
                                _Global.ReadAppSettings("Global");
                            }
                            _GlobalInitialized = true;
                        }
                        if (_Global == null)
                        {
                            // Default reserve error reporting method is used because we do not have a
                            // reporter object reference here:
                            DefaultReserveReportError(null, ReportType.Error, "Reporter.Global", "Global reporting system is not initialized.", null, null);
                        }
                    }
                }
                return _Global;
            }
            private set
            {
                lock (GlobalLock)
                {
                    _Global = value;
                    if (value != null)
                        _GlobalInitialized = true;
                    else
                        _GlobalInitialized = false;
                }
            }
        }

        #endregion Reporting_Global



        #region Reporter_ReadConfiguration
        // Reading of reporter settings from configuration files

        protected const string
            KeyUseConsole = "UseConsole";

        /// <summary>Reads settings for a specified named group of reporters from the application configuration file.</summary>
        /// <param name="groupname">Name of the group of reporters for which the settings apply.</param>
        protected override void ReadAppSettingsBasic(string groupname)
        // $A Igor Feb09;
        {
            try
            {
                ReportConsoleSet = false;  // switch off reporting about attempts to set UseConsole, since this can
                   // happen during readind of configuration, but it is not an error
                base.ReadAppSettingsBasic(groupname);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, "Reading configuration", null, null, ex);
            }
            finally
            {
                ReportConsoleSet = true;
            }
        }

        #endregion // Reporter_ReadConfiguration



        #region Reporting_Console_Annihilated

        private bool ReportConsoleSet = true;

        /// <summary>Console is not used by this class.</summary>
        public override bool UseConsole 
        { 
            get { return false; }
            set
            {
                if (ReportConsoleSet)
                    ReserveReportError(ReportType.Error, "ReporterMsgbox.UseConsole.set", "This class does not implement reporting via console.", null, null);
            }
        }

        #endregion  // Reporting_Console_Annihilated



    }  // class ReporterMsgbox


    #endregion  // Concrete_Classes


}  // namespace IG.Forms


