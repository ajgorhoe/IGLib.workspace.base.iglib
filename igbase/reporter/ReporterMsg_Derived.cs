using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;


namespace IG.Lib
{



    #region Interfaces

    public interface IReporterConsole : IReporter
    {
        bool UseConsole { get; set; }
    }

    #endregion  // Interfaces



    #region Base_Classes

    // Classes that contain implementation of significant portions of code, which will prevent
    // code duplication.


    /// <summary>Base class for reporter classes that contain either reporting via system console, 
    /// reporting via message box, or both.</summary>
    /// <remarks>Code common to all classes is put here. Dhis is to reduce duplication of code as much as possible, while
    /// at the same time achieving pure behavior: a class intended for reporting via message boxes but not via console
    /// does not enable reporting in a console; the class intended for reporting only via message box does not enable
    /// reporting via co nsole; but still there can be a class that enables both, with only small portions of code
    /// duplicated form both classes.
    /// Interfaces are intended to clearly distinguish between various functionality supported.</remarks>
    public abstract class ReporterConsole_Base : ReporterBase, IReporterConsole
    {

        #region Reporting_General

        // ACTUAL REPORTING METHODS (utilizing delegates if defined):

        // ReserveReportErrorSpecific is not overridden (just taken from the base class).

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
                        ReserveReportError(ReportType.Error, location,
                            "Error in Reporter.base.Report. " + message, ex, ex1);
                    }
                    // Then, call specific methods from this class:
                    if (UseConsole && DoReporting(messagetype))
                    {
                        try
                        {
                            this.Report_Console(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            ReserveReportError(ReportType.Error, location,
                                "Error in Reporter.Report_Console. " + message, ex, ex1);
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


        #region Reporting_Console

        /// <summary>Gets or sets the flag specifying whether reporting using the system console is performed or not.</summary>
        virtual public bool UseConsole { get; set; }

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
            //ReporterPado rep = reporter as ReporterPado;
            //if (rep == null)
            //    throw new ArgumentException("The reporter argument is not specified or is invalid (possible up-casting failure).");
            DefaultReportConsole(reporter, messagetype, location, message);
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


        #endregion  // Reporting_Console

    }  // class ReporterConsole_Base : Reporter 



    #endregion  // Base_Classes



    #region Concrete_Classes

    public class ReporterConsole : ReporterConsole_Base, IReporterConsole
    {

        #region Initialization

        #region Constructors

        // Constructors - this region will be literally included in derived classes, except in special cases):

        /// <summary>Constructor. Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        public ReporterConsole()
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
        public ReporterConsole(object obj,
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
        public ReporterConsole(object obj,
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
        public ReporterConsole(object obj, ReportDelegate reportdelegate)
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
        public ReporterConsole(object obj,
            ReportDelegate reportdelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
                Init(obj, reportdelegate, reservereportdelegate);
        }

        #endregion  // Constructors

        #region Initialization_Overridden

        // Setting default error reporting behavior, these methods should be overridden in derived classes:

        /// <summary>Sets the error reporting delegate to the default value.</summary>
        protected override void SetDefaultReportDlg()
        // $A Igor Oct08;
        {
            ReportDlg = null; // By default, there is no reporting through delegates. // new ReportDelegate(DefaultReportConsole);  
        }

        /// <summary>Sets the error location assembling delegate to the default value.
        /// This default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected override void SetDefaultReportLocationDlg()
        // $A Igor Oct08;
        {
            ReportLocationDlg = new ReportLocationDelegate(DefaultLocationString);
        }

        /// <summary>Sets the error message assembling delegate to the default value.
        /// This default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected override void SetDefaultReportMessageDlg()
        // $A Igor Oct08;
        {
            ReportMessageDlg = new ReportMessageDelegate(DefaultMessageString);
        }

        /// <summary>Sets the reserve error reporting delegate to the default value.</summary>
        protected override void SetDefaultReserveReportErrorDlg()
        // $A Igor Oct08;
        {
            ReserveReportErrorDlg = null; // new ReserveReportErrorDelegate(DefaultReserveReportError);
        }


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

        private static ReporterConsole _Global = null;
        private static bool _GlobalInitialized = false;
        private static object GlobalLock = new object();


        public static new bool GlobalInitialized
        {
            get { return _GlobalInitialized; }
        }

        /// <summary>Gets the global reporter object.
        /// This is typically used for configuring the global reporter.</summary>
        public static new ReporterConsole Global
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
                            _Global = new ReporterConsole();
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
            base.ReadAppSettingsBasic(groupname);
            bool assigned, boolval = false;
            GetAppSetting(groupname, KeyUseConsole, ref boolval, out assigned);
            if (assigned) UseConsole = boolval;
         }

        #endregion // Reporter_ReadConfiguration


        #region Reporting_Console
        // REPORTING BY A CONSOLE:

        private bool _UseConsole = true;

        /// <summary>Gets or sets the flag specifying whether reporting using the system console is performed or not.</summary>
        public override bool UseConsole { get { return _UseConsole; } set { _UseConsole = value; } }

        #endregion  // Reporting_Console


    }  // class ReporterConsole



    #endregion  // Concrete_Classes


}  // namespace IG.Lib


