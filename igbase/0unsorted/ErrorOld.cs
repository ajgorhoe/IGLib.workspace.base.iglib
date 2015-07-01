// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/


// Remark: Create objects of a specific type whose type is known only at runtime:
//      Activator.CreateInstance(Type, Args)
// (this can be used when re-throwing an error - if we want to keep teh error type but change the message.)


            /*******************************************/
            /*                                         */
            /*    UNIFIED EXCEPTION HANDLING SYSTEM    */
            /*                                         */
            /*******************************************/



using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace IG.Lib.Old
{

   ///// <summary>Defines the type of a report.</summary>
   // public enum ReportType {Error = 0, Warning, Info}

   // /// <summary>Defines the level of output when launching reports.</summary>
   // public enum ReportLevel { Error, Warning, Info, Verbose }


   // // Delegate types:

   // /// <summary>Reports an error.</summary>
   // /// <param name="reporter">Reference to the reporter class where all s necessary data is fond.
   // /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
   // /// <param name="errorlocation">User-defined string containing (sometimes supplemental to the Exception object) specification of error location
   // /// (e.g. a class or module name)</param>
   // /// <param name="errormessage">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
   // public delegate void ReportDelegate(Reporter reporter, ReportType messagetype,
   //         string errorlocation, string errormessage);

   // /// <summary>Assembles the error location desctiption.</summary>
   // /// <param name="reporter">Reference to the reporter class where all s necessary data is fond.
   // /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
   // /// <param name="location">User-provided string containing (sometimes supplemental to the Exception object) specification of error location
   // /// (e.g. a class or module name)</param>
   // /// <param name="ex">Exception to be reported.</param>
   // /// <param name="DebugMode">A flag indicating whether the debug mode reporting (usually more verbose) is performed.</param>
   // /// <returns>A string describing error location.</returns>
   // public delegate string ReportLocationDelegate(Reporter reporter, ReportType messagetype,
   //             string location, Exception ex);

   // /// <summary>Assembles error description (without any decoration, this is added by talling methods).</summary>
   // /// <param name="reporter">Reference to the reporter class where all s necessary data is fond.
   // /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
   // /// <param name="basicmessage">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
   // /// <param name="ex">Exception to be reported.</param>
   // /// <param name="DebugMode">>A flag indicating whether the debug mode reporting (usually more verbose) is performed.</param>
   // /// <returns>Error description string (error message).</returns>
   // public delegate string ReportMessageDelegate(Reporter reporter, ReportType messagetype,
   //             string basicmessage, Exception ex);

   // /// <summary>Reports errors occurred in error reporting methods when exceptions are thrown within them.
   // /// Methods assigned to these delegates must be bullet proof. They must report the original error (being reported when an
   // /// exception occurred) as well as the exception that occurred uin the error reporting method.</summary>
   // /// <param name="reporter">Reference to the reporter class where all s necessary data is fond.
   // /// In particular, the Obj member contains a user-set object reference used by the delegate functions.
   // /// This object should be used with special care within reserve reporting functions because it may be corrupted.</param>
   // /// <param name="location">User-provided string containing (sometimes supplemental to the Exception object) specification of error location
   // /// (e.g. a class or module name)</param>
   // /// <param name="message">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
   // /// <param name="ex">Original exception that was being reported.</param>
   // /// <param name="ex1">Exception that was thrwn by the error reporting function.</param>
   // public delegate void ReserveReportErrorDelegate(Reporter reporter, ReportType messagetype,
   //             string location, string message, Exception ex, Exception ex1);


    #region Types

    // WARNING: do not change values of these two enumerations because the values must correspond!

    /// <summary>Defines the type of a report.</summary>
    public enum ReportType { Error = 1, Warning, Info, Undefined }

    /// <summary>Defines the level of output when launching reports.</summary>
    public enum ReportLevel { Off = 0, Error, Warning, Info, Verbose }


    // Delegate types:

    /// <summary>Reports an error.</summary>
    /// <param name="reporter">Reference to the reporter class where all s necessary data is fond.
    /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
    /// <param name="errorlocation">User-defined string containing (sometimes supplemental to the Exception object) specification of error location
    /// (e.g. a class or module name)</param>
    /// <param name="errormessage">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
    public delegate void ReportDelegate(Reporter reporter, ReportType messagetype,
            string errorlocation, string errormessage);

    /// <summary>Assembles the error location desctiption.</summary>
    /// <param name="reporter">Reference to the reporter class where all s necessary data is fond.
    /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
    /// <param name="location">User-provided string containing (sometimes supplemental to the Exception object) specification of error location
    /// (e.g. a class or module name)</param>
    /// <param name="ex">Exception to be reported.</param>
    /// <param name="DebugMode">A flag indicating whether the debug mode reporting (usually more verbose) is performed.</param>
    /// <returns>A string describing error location.</returns>
    public delegate string ReportLocationDelegate(Reporter reporter, ReportType messagetype,
                string location, Exception ex);

    /// <summary>Assembles error description (without any decoration, this is added by talling methods).</summary>
    /// <param name="reporter">Reference to the reporter class where all s necessary data is fond.
    /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
    /// <param name="basicmessage">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
    /// <param name="ex">Exception to be reported.</param>
    /// <param name="DebugMode">>A flag indicating whether the debug mode reporting (usually more verbose) is performed.</param>
    /// <returns>Error description string (error message).</returns>
    public delegate string ReportMessageDelegate(Reporter reporter, ReportType messagetype,
                string basicmessage, Exception ex);

    /// <summary>Reports errors occurred in error reporting methods when exceptions are thrown within them.
    /// Methods assigned to these delegates must be bullet proof. They must report the original error (being reported when an
    /// exception occurred) as well as the exception that occurred uin the error reporting method.</summary>
    /// <param name="reporter">Reference to the reporter class where all s necessary data is fond.
    /// In particular, the Obj member contains a user-set object reference used by the delegate functions.
    /// This object should be used with special care within reserve reporting functions because it may be corrupted.</param>
    /// <param name="location">User-provided string containing (sometimes supplemental to the Exception object) specification of error location
    /// (e.g. a class or module name)</param>
    /// <param name="message">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
    /// <param name="ex">Original exception that was being reported.</param>
    /// <param name="ex1">Exception that was thrwn by the error reporting function.</param>
    public delegate void ReserveReportErrorDelegate(Reporter reporter, ReportType messagetype,
                string location, string message, Exception ex, Exception ex1);


    #endregion  // Types



    /// <summary>Interface from which all reporters inherit.</summary>
    /// $A Igor Aug08;
    public interface IReporter
    {

        #region Initialization

        // Default initialization


        /// <summary>Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        void Init();

        // Setting specific error reporting behavior:

        /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
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
        void Init(object obj,
           ReportDelegate reportdelegate,
           ReportLocationDelegate locationdelegate,
           ReportMessageDelegate messagedelegate,
           ReserveReportErrorDelegate reservereportdelegate);

        /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates that are not specified are set to default values.</summary>
        void Init(object obj,
           ReportDelegate reportdelegate,
           ReportLocationDelegate locationdelegate,
           ReportMessageDelegate messagedelegate);

        /// <summary>Initializes the error reporter by the specified auxiliary object and the delegate to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates for assembling the error location string and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        void Init(object obj,
           ReportDelegate reportdelegate);

        /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Delegates for assembling the error locatin and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        /// <param name="obj">Auxiliary object that will be passed to error reporting delegates when called in local methods.</param>
        /// <param name="reportdelegate">Delegates that is called to launc an error report.
        /// Methods of this class will pass to this class the auxiliary object, location strings assembled by the location assembling delegate,
        /// and error message string assembled by the error message delegate.</param>
        /// <param name="reservereportdelegate">Delegate that is called to report exceptions that occur within error reporting
        /// methods. In particular, this must ne as bullet proof as possible.</param>
        void Init(object obj,
           ReportDelegate reportdelegate,
           ReserveReportErrorDelegate reservereportdelegate);


        #endregion // Initialization

        #region Reporter_Data


        // Levels of various kinds of reporting (User interface reporting, logging and tracing):

        /// <summary>Resets the various kinds of reporting levels to default values.</summary>
        void ResetLevels();

        /// <summary>Gets or sets level of output for reporting (console output, message boxes, etc.).</summary>
        ReportLevel ReportingLevel { get; set; }

        /// <summary>Gets or sets level of output for logging (writing to log files).</summary>
        ReportLevel LoggingLevel { get; set; }

        /// <summary>Gets or sets trace level (for external trace systems).</summary>
        ReportLevel TracingLevel { get; set; }

        // Auxiliary properties & indexers dealing with levels


        /// <summary>Returns a boolean value indicating whether errors are treated by the reporter in its current state.</summary>
        bool TreatError { get; }

        /// <summary>Returns a boolean value indicating whether warnings are treated by the reporter in its current state.</summary>
        bool TreatWarning { get; }


        /// <summary>Returns a boolean value indicating whether info messages are treated by the reporter in its current state.</summary>
        bool TreatInfo { get; }

        /// <summary>Returns a boolean value indicating whether undefined messages with the lowest priority are treated by the reporter in its current state.</summary>
        bool TreatUndefined { get; }



        /// <summary>Gets the level with the lowesst priority (out of reporting, logging and tracing level), or sets all levels 
        /// to the assigned level.</summary>
        ReportLevel Level { get; set; }

        /// <summary>Indexer that returns true if messages of a given level are treated by the reporter (either by the
        /// reporting, logging or tracing part, according to the corresponding level states), or false if not (i.e. if
        /// all the levels are set lover than the index). 
        /// In the case of assignment, when true is assigned, all levels that are set lower than the are set to index.
        /// Assignment with false should be used only exteptionally; but in this case all levels that are set higher or 
        /// equal than the index are set one level below.</summary>
        /// <param name="level">Level for which we are interested whether it is treated.</param>
        /// <returns>True if a specific level (stated as index) is treated, false if not (i.e. if all of the reporting,
        /// logging )</returns>
        bool this[ReportLevel level] { get; set; }


        #region TraceSwitch

        /// <summary>Returns the System.Diagnostics.EventLogEntryType value corresponding to the given ReportType.
        /// Remark: FailureAudit and SuccessAudit can not be generated because they don't have representation in ReportType.</summary>
        /// <param name="level">ReportType value to be converted.</param>
        /// <returns>Converted value of type EventLogEntryType.</returns>
        EventLogEntryType ReportType2EventLogEntryType(ReportType rt);

        /// <summary>Returns the ReportType value corresponding to the given System.Diagnostics.EventLogEntryType.
        /// Remark: FailureAudit and SuccessAudit do not have representation in ReportType and are mapped to Error and Warning, respectively.</summary>
        /// <param name="level">EventLogEntryType value to be converted.</param>
        /// <returns>Converted value of type ReportType.</returns>
        ReportType EventLogEntryType2ReportType(EventLogEntryType el);

        // Properties of the type TraceSwitch that are synchronized with the levels of reporting of different types
        // (i.e. reporting, logging and tracing). These properties are not in use if not explicitly accessed, therefore
        // they just contribute to the Reporter'result API for those users used to deal with the TraceSwitch class.
        // It is not yet clear whether this feature will be kept in the futre.

        /// <summary>Returns the System.Diagnostics.TraceLevel value corresponding to the given ReportLevel.</summary>
        /// <param name="level">ReportLevel value to be converted.</param>
        /// <returns>Converted value of type TraceLevel.</returns>
        TraceLevel ReportLevel2TraceLevel(ReportLevel level);

        /// <summary>Returns the ReportLevel value corresponding to the given System.Diagnostics.TraceLevel.</summary>
        /// <param name="level">TraceLevel value to be converted.</param>
        /// <returns>Converted value of type ReportLevel.</returns>
        ReportLevel TraceLevel2ReportLevel(TraceLevel tl);


        /// <summary>Gets or sets the TraceSwitch that is synchronized with ReportingLevel.
        /// IMPORTANT: State of the object obtained by get accessor must not be changed unless the object is 
        /// assigned back by the set accessor. If one wants that this TraceSwitch assumes values specified by the configuration
        /// file, a corresponding Traceswitch must be created and assigned to this property (otherwise the TraceSwitch will be
        /// synchronized with the ReportingLevel, which will override its settings).</summary>
        TraceSwitch ReportingSwitch { get; set; }

        /// <summary>Gets or sets the TraceSwitch that is synchronized with LoggingLevel.
        /// IMPORTANT: State of the object obtained by get accessor must not be changed unless the object is 
        /// assigned back by the set accessor. If one wants that this TraceSwitch assumes values specified by the configuration
        /// file, a corresponding Traceswitch must be created and assigned to this property (otherwise the TraceSwitch will be
        /// synchronized with the LoggingLevel, which will override its settings).</summary>
        TraceSwitch LoggingSwitch { get; set; }

        /// <summary>Gets or sets the TraceSwitch that is synchronized with TracingLevel.
        /// IMPORTANT: State of the object obtained by get accessor must not be changed unless the object is 
        /// assigned back by the set accessor. If one wants that this TraceSwitch assumes values specified by the configuration
        /// file, a corresponding Traceswitch must be created and assigned to this property (otherwise the TraceSwitch will be
        /// synchronized with the TracingLevel, which will override its settings).</summary>
        TraceSwitch TracingSwitch { get; set; }


        #endregion  // TraceSwitch




        /// <summary>Auxiliary object used by the delegates that perform error reporting.
        /// The object is used to provide additional information used in error reporting, or to provide objects that
        /// perform some actions in error reporting tasks, or both. 
        /// It is left entirely to error reporting delegates to interpret the object'result contents.</summary>
        object Obj { get; set; }

        /// <summary>Object used for locking.</summary>
        object lockobj { get; }

        #endregion  // Reporter_Data


        #region Reporting
        // ACTUAL REPORTING METHDS; Specific methods are not included in the interface.

        // GENERAL REPORTING METHODS (for all kinds of reports):

        /// <summary>Basic reporting method (overloaded). Launches an error report, a warning report or s kind of report/message.
        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        /// are obtained from the class' instance.</summary>
        /// <param name="messagetype">The type of the report (e.g. Error, Warning, etc.).</param>
        /// <param name="location">User-provided description of error location.</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception thrown when error occurred.</param>
        void Report(ReportType messagetype, string location, string message, Exception ex);

        // Overloaded general reporting methods (different combinations of parameters passed):

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        void Report(ReportType messagetype, string message, Exception ex);

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        /// <param name="location">User-provided description of error location.</param>
        void Report(ReportType messagetype, Exception ex, string location);

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        void Report(ReportType messagetype, Exception ex);

        /// <summary>Launches a report.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        void Report(ReportType messagetype, string location, string message);

        /// <summary>Launches a report.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="message">User provided message included in the report.</param>
        void Report(ReportType messagetype, string message);


        // ERROR REPORTING FUNCTIONS:


        /// <summary>Basic error reporting method (overloaded).
        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        /// are obtained from the class' instance.</summary>
        /// <param name="location">User-provided description of error location.</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception thrown when error occurred.</param>
        void ReportError(string location, string message, Exception ex);

        // Overloaded general reporting methods (different combinations of parameters passed):

        /// <summary>Launches an error report.</summary>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        void ReportError(string message, Exception ex);

        /// <summary>Launches an error report.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        /// <param name="location">User-provided description of error location.</param>
        void ReportError(Exception ex, string location);

        /// <summary>Launches an error report. Predominantly for error and warning reports.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        void ReportError(Exception ex);

        /// <summary>Launches an error report.</summary>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        void ReportError(string location, string message);

        /// <summary>Launches an error report.</summary>
        /// <param name="message">User provided message included in the report.</param>
        void ReportError(string message);



        // WARNING LAUNCHING FUNCTIONS:


        /// <summary>Basic warning reporting method (overloaded).
        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        /// are obtained from the class' instance.</summary>
        /// <param name="location">User-provided description of error location.</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception thrown when error occurred.</param>
        void ReportWarning(string location, string message, Exception ex);

        // Overloaded general warning methods (different combinations of parameters passed):

        /// <summary>Launches a warning report.</summary>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        void ReportWarning(string message, Exception ex);

        /// <summary>Launches a warning report.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        /// <param name="location">User-provided description of error location.</param>
        void ReportWarning(Exception ex, string location);

        /// <summary>Launches a warning report. Predominantly for error and warning reports.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        void ReportWarning(Exception ex);

        /// <summary>Launches a warning report.</summary>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        void ReportWarning(string location, string message);

        /// <summary>Launches a warning report.</summary>
        /// <param name="message">User provided message included in the report.</param>
        void ReportWarning(string message);



        // INFO LAUNCHING FUNCTIONS:

        /// <summary>Launches an info.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        void ReportInfo(Exception ex);

        /// <summary>Launches an info.</summary>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        void ReportInfo(string location, string message);

        /// <summary>Launches an info.</summary>
        /// <param name="message">User provided message included in the report.</param>
        void ReportInfo(string message);

        #endregion // Reporting


    }   // interface IReporter





    /// <summary>Base IG class for reporting, tracing and logging; provides a global reporter and a basis for creation 
    /// of local reporters.
    /// This class is identical to the IGLib class (copied directly). IN EFA, refer to the derived class Reporter!</summary>
    public class Reporter : IReporter
    // $A Igor Oct08;
    {

        #region Erorr_Reporting_Global

        // Global error reporter of this class:
        // In derived classes, this block should be repeated, only with classs name of _Global and Global set to the
        // current (derived) class and with keyword new in property declarations.

        // TODO: consider whether initialization is necessary here (maybe it is better to allow only explicit initialization).
        private static Reporter _Global = null;
        private static bool _GlobalInitialized = false;

        public static bool GlobalInitialized
        {
            get { return _GlobalInitialized; }
        }

        /// <summary>Indicates whwther the current reporter is used as a global reporter or not.
        /// This flag is set when the global reporter is initialized.</summary>
        protected bool IsGlobal = false;

        /// <summary>Gets the global reporter object.
        /// This is typically used for configuring the global reporter.</summary>
        public static Reporter Global
        // $A Igor Oct08;
        {
            get
            {
                if (_Global == null)
                {
                    if (!_GlobalInitialized)
                    {
                        _Global = new Reporter();
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
            protected set { _Global = value; if (value != null) _GlobalInitialized = true; }
        }

        #endregion Erorr_Reporting_Global


        #region Error_Reporting_Definition

        // Replaceable methods implemented through delagates:

        /// <summary>Delegate that performs error reporting.
        /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
        /// assemble error message. Then it uses both to assemble the final decorated error message and launches
        /// it in its own way.</summary>
        public ReportDelegate ReportDlg = null;  // new ReportDelegate(DefaultReportConsole);

        /// <summary>Delegate that assembles the error location string.</summary>
        public ReportLocationDelegate ReportLocationDlg = null;  //new ReportLocationDelegate(DefaultLocationString);

        /// <summary>Delegate that assembles the eror message string.</summary>
        public ReportMessageDelegate ReportMessageDlg = null;  // new ReportMessageDelegate(DefaultMessageString);

        /// <summary>Delegate that reports an error when the error reporting function throws an exception.
        /// Functions assigned to this delegate should really be BULLETPROOF. 
        /// It is highly recommended to do reporting in small steps enclosed in try/catch blocks and especially to
        /// use the error reporting object very carefully (because it may not be initialized properly, which can
        /// also be the reason that the error reporting function crashes).
        /// It is higly recommended to first call the DefaultReserveReportError() within the function assigned to this delegate,
        /// or at least to use the DefaultReserveReportMessage() method for assembly of the message shown.</summary>
        public ReserveReportErrorDelegate ReserveReportErrorDlg = null;  // new ReserveReportErrorDelegate(DefaultReserveReportError);

        #region Initialization

        #region Constructors

        // Constructors - this region will be literally included in derived classes, except in special cases):

        /// <summary>Constructor. Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        public Reporter()
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
        public Reporter(object obj,
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
        public Reporter(object obj,
            ReportDelegate reportdelegate,
            ReportLocationDelegate locationdelegate,
            ReportMessageDelegate messagedelegate)
        // $A Igor Oct08;
        {
            Init(obj, reportdelegate, locationdelegate, messagedelegate);
        }

        /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and the delegate to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates for assembling the error location string and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        public Reporter(object obj, ReportDelegate reportdelegate)
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
        public Reporter(object obj,
            ReportDelegate reportdelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
            Init(obj, reportdelegate, reservereportdelegate);
        }

        #endregion  // Constructors


        #region Initialization_Overridden

        // Setting default error reporting behavior, these methods should be overridden in derived classes:

        /// <summary>Sets the error reporting delegate to the default value.
        /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected virtual void SetDefaultReportDlg()
        // $A Igor Oct08;
        {
            ReportDlg = new ReportDelegate(DefaultReportConsole);
        }

        /// <summary>Sets the error location assembling delegate to the default value.
        /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected virtual void SetDefaultReportLocationDlg()
        // $A Igor Oct08;
        {
            ReportLocationDlg = new ReportLocationDelegate(DefaultLocationString);
        }

        /// <summary>Sets the error message assembling delegate to the default value.
        /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected virtual void SetDefaultReportMessageDlg()
        // $A Igor Oct08;
        {
            ReportMessageDlg = new ReportMessageDelegate(DefaultMessageString);
        }

        /// <summary>Sets the reserve error reporting delegate to the default value.
        /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected virtual void SetDefaultReserveReportErrorDlg()
        // $A Igor Oct08;
        {
            ReserveReportErrorDlg = new ReserveReportErrorDelegate(DefaultReserveReportError);
        }


        /// <summary>Initial part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected virtual void InitBegin()
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                Obj = null;
                ReportDlg = null;
                ReportLocationDlg = null;
                ReportMessageDlg = null;
                ReserveReportErrorDlg = null;
            }
        }

        /// <summary>Finalizing part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected virtual void InitEnd()
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                if (ReportDlg == null)
                    SetDefaultReportDlg();
                if (ReportLocationDlg == null)
                    SetDefaultReportLocationDlg();
                if (ReportMessageDlg == null)
                    SetDefaultReportMessageDlg();
                if (ReserveReportErrorDlg == null)
                    SetDefaultReserveReportErrorDlg();
            }
        }

        #endregion  // Initialization_Overridden


        #region Initialization_Overloaded

        // Initialization functions (overloaded) to be called in constructors:

        /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
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
        public void Init(object obj,
            ReportDelegate reportdelegate,
            ReportLocationDelegate locationdelegate,
            ReportMessageDelegate messagedelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                InitBegin();  // Initial part of initialization that can  be overridden by derived classes
                this.Obj = obj;
                this.ReportDlg = reportdelegate;
                this.ReportLocationDlg = locationdelegate;
                this.ReportMessageDlg = messagedelegate;
                this.ReserveReportErrorDlg = reservereportdelegate;
                //Initialize delegates that were not provided with default values:
                InitEnd(); // final part of initialization that can be overridden 
            }
        }

        /// <summary>Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        public void Init()
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                Init(null /* obj */, null /* reportdelegate */, null /* locationdelegate */, null /* messagedelegate */,
                    null /* reservereportdelegate */ );
            }
        }

        /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates that are not specified are set to default values.</summary>
        public void Init(object obj,
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

        /// <summary>Initializes the error reporter by the specified auxiliary object and the delegate to perform error reporting tasks.
        /// Reserve error reporting delegate is initialized to a default value.
        /// Delegates for assembling the error location string and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        public void Init(object obj,
            ReportDelegate reportdelegate)
        // $A Igor Oct08;
        {
            lock (lockobj)
            {
                Init(obj, reportdelegate, null /* locationdelegate */, null /* messagedelegate */, null /* reservereportdelegate */ );
            }
        }

        /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
        /// Delegates for assembling the error locatin and error message string are set to their default values, 
        /// which are adapted to console-like eror reporting systems.</summary>
        /// <param name="obj">Auxiliary object that will be passed to error reporting delegates when called in local methods.</param>
        /// <param name="reportdelegate">Delegates that is called to launc an error report.
        /// Methods of this class will pass to this class the auxiliary object, location strings assembled by the location assembling delegate,
        /// and error message string assembled by the error message delegate.</param>
        /// <param name="reservereportdelegate">Delegate that is called to report exceptions that occur within error reporting
        /// methods. In particular, this must ne as bullet proof as possible.</param>
        public void Init(object obj,
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

        #endregion  // Initialization_Overloaded


        #endregion  // Initialization

        #endregion // Error_Reporting_Definition


        #region Reporter_Data


        /// <summary>Indicates that reporting suitable for debugging mode should be performed.
        /// A standard flag that can be used by the delegate functions.</summary>
        public bool DebugMode = false;


        // Levels of various kinds of reporting (User interface reporting, logging and tracing):

        protected ReportLevel
            _ReportingLevel = ReportLevel.Warning,
            _LoggingLevel = ReportLevel.Warning,
            _TracingLevel = ReportLevel.Info;


        /// <summary>Resets the various kinds of reporting levels to default values.</summary>
        public virtual void ResetLevels()
        {
            ReportingLevel = ReportLevel.Warning;
            LoggingLevel = ReportLevel.Warning;
            TracingLevel = ReportLevel.Info;
        }

        /// <summary>Gets or sets level of output for reporting (console output, message boxes, etc.).</summary>
        public virtual ReportLevel ReportingLevel
        {
            get { return _ReportingLevel; }
            set { _ReportingLevel = value; if (_ReportingSwitch != null) SyncTraceSwitchWithReportLevel(_ReportingLevel, _ReportingSwitch); }
        }

        /// <summary>Gets or sets level of output for logging (writing to log files).</summary>
        public virtual ReportLevel LoggingLevel
        {
            get { return _LoggingLevel; }
            set { _LoggingLevel = value; if (_LoggingSwitch != null) SyncTraceSwitchWithReportLevel(_LoggingLevel, _LoggingSwitch); }
        }

        /// <summary>Gets or sets trace level (for external trace systems).</summary>
        public virtual ReportLevel TracingLevel
        {
            get { return _TracingLevel; }
            set { _TracingLevel = value; if (_TracingSwitch != null) SyncTraceSwitchWithReportLevel(_TracingLevel, _TracingSwitch); }
        }

        // Auxiliary properties & indexers dealing with levels

        /// <summary>Returns a boolean value indicating whether errors are treated by the reporter in its current state.</summary>
        public bool TreatError
        { get { return (ReportingLevel >= ReportLevel.Error || LoggingLevel >= ReportLevel.Error || TracingLevel >= ReportLevel.Error); } }

        /// <summary>Returns a boolean value indicating whether warnings are treated by the reporter in its current state.</summary>
        public bool TreatWarning
        { get { return (ReportingLevel >= ReportLevel.Warning || LoggingLevel >= ReportLevel.Warning || TracingLevel >= ReportLevel.Warning); } }

        /// <summary>Returns a boolean value indicating whether info messages are treated by the reporter in its current state.</summary>
        public bool TreatInfo
        { get { return (ReportingLevel >= ReportLevel.Info || LoggingLevel >= ReportLevel.Info || TracingLevel >= ReportLevel.Info); } }

        /// <summary>Returns a boolean value indicating whether undefined messages with the lowest priority are treated by the reporter in its current state.</summary>
        public bool TreatUndefined
        { get { return (ReportingLevel >= ReportLevel.Verbose || LoggingLevel >= ReportLevel.Verbose || TracingLevel >= ReportLevel.Verbose); } }


        /// <summary>Returns true if the report of a given type will be launched at the given reporting level, and false if not.</summary>
        /// <param name="reptype">Type of the report for which information is returned.</param>
        /// <param name="replevel">True if reports of the specific type are launched, false if not.</param>
        public static bool DoLaunch(ReportType reptype, ReportLevel replevel)
        {
            switch (reptype)
            {
                case ReportType.Error:
                    if (replevel >= ReportLevel.Error)
                        return true;
                    else
                        return false;
                case ReportType.Warning:
                    if (replevel >= ReportLevel.Warning)
                        return true;
                    else
                        return false;
                case ReportType.Info:
                    if (replevel >= ReportLevel.Info)
                        return true;
                    else
                        return false;
                case ReportType.Undefined:
                    if (replevel >= ReportLevel.Verbose)
                        return true;
                    else
                        return false;
                default:
                    return true;
            }
        }

        /// <summary>Returns true if the report of a given type should be shown by user interface (according to ReportingLevel),
        /// and false if not.</summary>
        /// <param name="reptype">Type of the report for which information is returned.</param>
        /// <returns>True if reports of the specific type are launched, false if not.</returns>
        public bool DoReporting(ReportType reptype)
        {
            return DoLaunch(reptype, ReportingLevel);
        }

        /// <summary>Returns true if the report of a given type should be logged in log files (according to ReportingLevel),
        /// and false if not.</summary>
        /// <param name="reptype">Type of the report for which information is returned.</param>
        /// <returns>True if reports of the specific type are launched, false if not.</returns>
        public bool DoLogging(ReportType reptype)
        {
            return DoLaunch(reptype, LoggingLevel);
        }

        /// <summary>Returns true if the report of a given type should traced (according to ReportingLevel),
        /// and false if not.</summary>
        /// <param name="reptype">Type of the report for which information is returned.</param>
        /// <returns>True if reports of the specific type are launched, false if not.</returns>
        public bool DoTracing(ReportType reptype)
        {
            return DoLaunch(reptype, TracingLevel);
        }




        /// <summary>Gets the level with the lowesst priority (out of reporting, logging and tracing level), or sets all levels 
        /// to the assigned level.</summary>
        public ReportLevel Level
        {
            get
            {
                ReportLevel ret = ReportingLevel;
                if (LoggingLevel > ret)
                    ret = LoggingLevel;
                if (TracingLevel > ret)
                    ret = TracingLevel;
                return ret;
            }
            set
            {
                ReportingLevel = LoggingLevel = TracingLevel = value;
            }
        }

        /// <summary>Indexer that returns true if messages of a given level are treated by the reporter (either by the
        /// reporting, logging or tracing part, according to the corresponding level states), or false if not (i.e. if
        /// all the levels are set lover than the index). 
        /// In the case of assignment, when true is assigned, all levels that are set lower than the are set to index.
        /// Assignment with false should be used only exteptionally; but in this case all levels that are set higher or 
        /// equal than the index are set one level below.</summary>
        /// <param name="level">Level for which we are interested whether it is treated.</param>
        /// <returns>True if a specific level (stated as index) is treated, false if not (i.e. if all of the reporting,
        /// logging )</returns>
        public virtual bool this[ReportLevel level]
        {
            get
            {
                return (ReportingLevel >= level || LoggingLevel >= level || TracingLevel >= level);
            }
            set
            {
                if (value)
                {
                    if (ReportingLevel < level)
                        ReportingLevel = level;
                    if (LoggingLevel < level)
                        LoggingLevel = level;
                    if (TracingLevel < level)
                        TracingLevel = level;
                }
                else  // value = false
                {
                    ReportLevel level1 = ReportLevel.Off;
                    try
                    {
                        if (level > ReportLevel.Off)
                            level1 = (ReportLevel)((int)level - 1);
                    }
                    catch { }
                    if (ReportingLevel > level1)
                        ReportingLevel = (level1);

                    if (LoggingLevel > level1)
                        LoggingLevel = level1;
                    if (TracingLevel > level1)
                        TracingLevel = level1;
                }
            }
        }



        #region TraceSwitch

        // Properties of the type TraceSwitch that are synchronized with the levels of reporting of different types
        // (i.e. reporting, logging and tracing). These properties are not in use if not explicitly accessed, therefore
        // they just contribute to the Reporter'result API for those users used to deal with the TraceSwitch class.
        // It is not yet clear whether this feature will be kept in the futre.


        /// <summary>Returns the System.Diagnostics.EventLogEntryType value corresponding to the given ReportType.
        /// Remark: FailureAudit and SuccessAudit can not be generated because they don't have representation in ReportType.</summary>
        /// <param name="level">ReportType value to be converted.</param>
        /// <returns>Converted value of type EventLogEntryType.</returns>
        public virtual EventLogEntryType ReportType2EventLogEntryType(ReportType rt)
        {
            switch (rt)
            {
                case ReportType.Error:
                    return EventLogEntryType.Error;
                case ReportType.Warning:
                    return EventLogEntryType.Warning;
                case ReportType.Info:
                    return EventLogEntryType.Information;
                case ReportType.Undefined:
                    return EventLogEntryType.Error;
                //case ReportType.:
                //    return EventLogEntryType.FailureAudit;
                //case ReportType.:
                //    return EventLogEntryType.SuccessAudit;
                default:
                    return EventLogEntryType.Error;
            }
        }

        /// <summary>Returns the ReportType value corresponding to the given System.Diagnostics.EventLogEntryType.
        /// Remark: FailureAudit and SuccessAudit do not have representation in ReportType and are mapped to Error and Warning, respectively.</summary>
        /// <param name="level">EventLogEntryType value to be converted.</param>
        /// <returns>Converted value of type ReportType.</returns>
        public virtual ReportType EventLogEntryType2ReportType(EventLogEntryType el)
        {
            switch (el)
            {
                case EventLogEntryType.Error:
                    return ReportType.Error;
                case EventLogEntryType.Warning:
                    return ReportType.Warning;
                case EventLogEntryType.Information:
                    return ReportType.Info;
                case EventLogEntryType.FailureAudit:
                    return ReportType.Error;
                case EventLogEntryType.SuccessAudit:
                    return ReportType.Warning;
                default:
                    return ReportType.Error;
            }
        }

        /// <summary>Returns the System.Diagnostics.TraceLevel value corresponding to the given ReportLevel.</summary>
        /// <param name="level">ReportLevel value to be converted.</param>
        /// <returns>Converted value of type TraceLevel.</returns>
        public virtual TraceLevel ReportLevel2TraceLevel(ReportLevel level)
        {
            switch (level)
            {
                case ReportLevel.Off:
                    return TraceLevel.Off;
                case ReportLevel.Error:
                    return TraceLevel.Error;
                case ReportLevel.Warning:
                    return TraceLevel.Warning;
                case ReportLevel.Info:
                    return TraceLevel.Info;
                case ReportLevel.Verbose:
                    return TraceLevel.Verbose;
                default:
                    return TraceLevel.Off;
            }

        }

        /// <summary>Returns the ReportLevel value corresponding to the given System.Diagnostics.TraceLevel.</summary>
        /// <param name="level">TraceLevel value to be converted.</param>
        /// <returns>Converted value of type ReportLevel.</returns>
        public virtual ReportLevel TraceLevel2ReportLevel(TraceLevel tl)
        {
            switch (tl)
            {
                case TraceLevel.Off:
                    return ReportLevel.Off;
                case TraceLevel.Error:
                    return ReportLevel.Error;
                case TraceLevel.Warning:
                    return ReportLevel.Warning;
                case TraceLevel.Info:
                    return ReportLevel.Info;
                case TraceLevel.Verbose:
                    return ReportLevel.Verbose;
                default:
                    return ReportLevel.Off;
            }
        }

        /// <summary>Synchronizes the value of ReportLevel enumeration variable with the state of a TraceSwitch variable. 
        /// Enumeration is synchronized according to the Level property of the switch.</summary>
        /// <param name="tswitch">A trace switch that the ReportLevel enumeration variable will be synchronized with. 
        /// If it is null or it contains a level that can not be represented by the ReportLevel enumeration then nothing happens.</param>
        /// <param name="level">The ReportLevel variable that is synchronized; declared as ref</param>
        protected void SyncReportLevelWithTraceSwitch(TraceSwitch tswitch, ref ReportLevel level)
        {
            if (tswitch != null)
                level = TraceLevel2ReportLevel(tswitch.Level);
        }

        /// <summary>Synchronizes the state of a TraceSwitch object with the value of the ReportLevel enumeration.</summary>
        /// <param name="level">Value of the ReportLevel enumeration that TraceSwitch will be synchronized with</param>
        /// <param name="tswitch">TraceSwitch taht is synchronized.</param>
        protected void SyncTraceSwitchWithReportLevel(ReportLevel level, TraceSwitch tswitch)
        {
            if (tswitch != null)
                tswitch.Level = ReportLevel2TraceLevel(level);
        }



        protected TraceSwitch
            _ReportingSwitch = null,
            _LoggingSwitch = null, // new TraceSwitch("Logging", "Synchronized with the logging level of the reporter."),
            _TracingSwitch = null;  // TraceSwitch("Tracing", "Synchronized with the tracing level of the reporter.");

        /// <summary>Gets or sets the TraceSwitch that is synchronized with ReportingLevel.
        /// IMPORTANT: State of the object obtained by get accessor must not be changed unless the object is 
        /// assigned back by the set accessor. If one wants that this TraceSwitch assumes values specified by the configuration
        /// file, a corresponding Traceswitch must be created and assigned to this property (otherwise the TraceSwitch will be
        /// synchronized with the ReportingLevel, which will override its settings).</summary>
        public TraceSwitch ReportingSwitch
        {
            get
            {
                if (_ReportingSwitch == null)
                {
                    _ReportingSwitch = new TraceSwitch("Reporting", "Synchronized with the ReportingLevel of the reporter.");
                }
                // Before returning this object, synchronize it with the ReportingLevel:
                SyncTraceSwitchWithReportLevel(ReportingLevel, _ReportingSwitch);
                return _ReportingSwitch;
            }
            set
            {
                _ReportingSwitch = value;
                SyncReportLevelWithTraceSwitch(_ReportingSwitch, ref _ReportingLevel);
            }
        }

        /// <summary>Gets or sets the TraceSwitch that is synchronized with LoggingLevel.
        /// IMPORTANT: State of the object obtained by get accessor must not be changed unless the object is 
        /// assigned back by the set accessor. If one wants that this TraceSwitch assumes values specified by the configuration
        /// file, a corresponding Traceswitch must be created and assigned to this property (otherwise the TraceSwitch will be
        /// synchronized with the LoggingLevel, which will override its settings).</summary>
        public TraceSwitch LoggingSwitch
        {
            get
            {
                if (_LoggingSwitch == null)
                {
                    _LoggingSwitch = new TraceSwitch("Logging", "Synchronized with the LoggingLevel of the reporter.");
                }
                // Before returning this object, synchronize it with the LoggingLevel:
                SyncTraceSwitchWithReportLevel(LoggingLevel, _LoggingSwitch);
                return _LoggingSwitch;
            }
            set
            {
                _LoggingSwitch = value;
                SyncReportLevelWithTraceSwitch(_LoggingSwitch, ref _LoggingLevel);
            }
        }

        /// <summary>Gets or sets the TraceSwitch that is synchronized with TracingLevel.
        /// IMPORTANT: State of the object obtained by get accessor must not be changed unless the object is 
        /// assigned back by the set accessor. If one wants that this TraceSwitch assumes values specified by the configuration
        /// file, a corresponding Traceswitch must be created and assigned to this property (otherwise the TraceSwitch will be
        /// synchronized with the TracingLevel, which will override its settings).</summary>
        public TraceSwitch TracingSwitch
        {
            get
            {
                if (_TracingSwitch == null)
                {
                    _TracingSwitch = new TraceSwitch("Tracing", "Synchronized with the TracingLevel of the reporter.");
                }
                // Before returning this object, synchronize it with the TracingLevel:
                SyncTraceSwitchWithReportLevel(TracingLevel, _TracingSwitch);
                return _TracingSwitch;
            }
            set
            {
                _TracingSwitch = value;
                SyncReportLevelWithTraceSwitch(_TracingSwitch, ref _TracingLevel);
            }
        }

        #endregion TraceSwitch




        /// <summary>If true then the basic reporting function will throw an exception.
        /// This is intended predominantly for testing how the reporter behaves in case of internal errors.
        /// When the exception is thrown, the value is set back to false. If we want an exception to be
        /// thrown again then the value must be set to true once again.</summary>
        public bool ThrowTestException { get { return _ThrowTestException; } set { _ThrowTestException = value; } }
        bool _ThrowTestException = false;

        /// <summary>Auxiliary object used by the delegates that perform error reporting.
        /// The object is used to provide additional information used in error reporting, or to provide objects that
        /// perform some actions in error reporting tasks, or both. 
        /// It is left entirely to error reporting delegates to interpret the object'result contents.</summary>
        public object Obj { get { return _obj; } set { _obj = value; } }
        private object _obj = null;

        protected object _lockobj = new Object();     // enables thread locking

        /// <summary>Object used for locking.</summary>
        public virtual object lockobj
        {
            get { return _lockobj; }
        }

        #endregion  // Reporter_Data


        #region Reporting
        // ACTUAL REPORTING METHDS

        #region Reporting_General

        // ACTUAL REPORTING METHODS (utilizing delegates):

        // Last resort error reporting function (bulletproof, called if an exception is thrown inside a reporting function):

        /// <summary>Used to report errors within reporting functions.
        /// Designed to be bullet proof in order to ensure that improper behavior of the reporting system does not remain unnoticed.</summary>
        /// <param name="messagetype"></param>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="ex1"></param>
        protected virtual void ReserveReportError(ReportType messagetype, string location,
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
                        DefaultReserveReportError(this, messagetype, location, message, ex, ex1);
                }
            }
            catch (Exception)
            {
                DefaultReserveReportError(this, messagetype, location, message, ex, ex1);
            }
        }

        // GENERAL REPORTING METHODS (for all kinds of reports):

        /// <summary>Basic reporting method (overloaded). Launches an error report, a warning report or s kind of report/message.
        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        /// are obtained from the class' instance.</summary>
        /// <param name="messagetype">The type of the report (e.g. Error, Warning, etc.).</param>
        /// <param name="location">User-provided description of error location.</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception thrown when error occurred.</param>
        public virtual void Report(ReportType messagetype, string location, string message, Exception ex)
        // $A Igor Oct08;
        {
            string locationstr = "", messagestr = "";
            try
            {
                lock (lockobj)
                {
                    try
                    {
                        if (ReportLocationDlg != null)
                            locationstr = ReportLocationDlg(this, messagetype, location, ex);
                        if (ReportMessageDlg != null)
                            messagestr = ReportMessageDlg(this, messagetype, message, ex);
                        if (ReportDlg != null)
                            ReportDlg(this, messagetype, locationstr, messagestr);
                    }
                    catch (Exception ex1)
                    {
                        this.ReserveReportError(ReportType.Error, location,
                            "Error in Report. " + message, ex, ex1);
                    }
                    if (this.UseTextWriter)
                    {
                        try
                        {
                            this.Report_TextWriter(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Report_ConsoleForm. " + message, ex, ex1);
                        }
                    }


                    if (ThrowTestException)
                    {
                        // Throw a test exception:
                        throw new Exception("Test exception thrown by the reporter (after reporting has been performted).");
                    }
                }
            }
            catch (Exception ex1)
            {
                ThrowTestException = false;  // re-set, such that a test exception is thrown only once
                DefaultReserveReportError(null, ReportType.Error, location, message, ex, ex1);
            }
        }

        // Overloaded general reporting methods (different combinations of parameters passed):

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public void Report(ReportType messagetype, string message, Exception ex)
        // $A Igor Oct08;
        {
            Report(messagetype, null /* location */, message, ex);
        }

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        /// <param name="location">User-provided description of error location.</param>
        public void Report(ReportType messagetype, Exception ex, string location)
        // $A Igor Oct08;
        {
            Report(messagetype, location, null /* message */, ex);
        }

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public void Report(ReportType messagetype, Exception ex)
        // $A Igor Oct08;
        {
            Report(messagetype, null /* location */ , null /* message */, ex);
        }

        /// <summary>Launches a report.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        public void Report(ReportType messagetype, string location, string message)
        // $A Igor Oct08;
        {
            Report(messagetype, location, message, null /* ex */ );
        }

        /// <summary>Launches a report.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="message">User provided message included in the report.</param>
        public void Report(ReportType messagetype, string message)
        // $A Igor Oct08;
        {
            Report(messagetype, null /* location */, message, null /* ex */ );
        }

        #region Reporting_Types

        // ERROR REPORTING FUNCTIONS:


        /// <summary>Basic error reporting method (overloaded).
        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        /// are obtained from the class' instance.</summary>
        /// <param name="location">User-provided description of error location.</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception thrown when error occurred.</param>
        public void ReportError(string location, string message, Exception ex)
        // $A Igor Oct08;
        {
            Report(ReportType.Error, location, message, ex);
        }

        // Overloaded general reporting methods (different combinations of parameters passed):

        /// <summary>Launches an error report.</summary>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public void ReportError(string message, Exception ex)
        // $A Igor Oct08;
        {
            Report(ReportType.Error, null /* location */, message, ex);
        }

        /// <summary>Launches an error report.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        /// <param name="location">User-provided description of error location.</param>
        public void ReportError(Exception ex, string location)
        // $A Igor Oct08;
        {
            Report(ReportType.Error, location, null /* message */, ex);
        }

        /// <summary>Launches an error report. Predominantly for error and warning reports.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public void ReportError(Exception ex)
        // $A Igor Oct08;
        {
            Report(ReportType.Error, null /* location */ , null /* message */, ex);
        }

        /// <summary>Launches an error report.</summary>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        public void ReportError(string location, string message)
        // $A Igor Oct08;
        {
            Report(ReportType.Error, location, message, null /* ex */ );
        }

        /// <summary>Launches an error report.</summary>
        /// <param name="message">User provided message included in the report.</param>
        public void ReportError(string message)
        // $A Igor Oct08;
        {
            Report(ReportType.Error, null /* location */, message, null /* ex */ );
        }



        // WARNING LAUNCHING FUNCTIONS:


        /// <summary>Basic warning reporting method (overloaded).
        /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        /// are obtained from the class' instance.</summary>
        /// <param name="location">User-provided description of error location.</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception thrown when error occurred.</param>
        public void ReportWarning(string location, string message, Exception ex)
        // $A Igor Oct08;
        {
            Report(ReportType.Warning, location, message, ex);
        }

        // Overloaded general warning methods (different combinations of parameters passed):

        /// <summary>Launches a warning report.</summary>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public void ReportWarning(string message, Exception ex)
        // $A Igor Oct08;
        {
            Report(ReportType.Warning, null /* location */, message, ex);
        }

        /// <summary>Launches a warning report.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        /// <param name="location">User-provided description of error location.</param>
        public void ReportWarning(Exception ex, string location)
        // $A Igor Oct08;
        {
            Report(ReportType.Warning, location, null /* message */, ex);
        }

        /// <summary>Launches a warning report. Predominantly for error and warning reports.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public void ReportWarning(Exception ex)
        // $A Igor Oct08;
        {
            Report(ReportType.Warning, null /* location */ , null /* message */, ex);
        }

        /// <summary>Launches a warning report.</summary>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        public void ReportWarning(string location, string message)
        // $A Igor Oct08;
        {
            Report(ReportType.Warning, location, message, null /* ex */ );
        }

        /// <summary>Launches a warning report.</summary>
        /// <param name="message">User provided message included in the report.</param>
        public void ReportWarning(string message)
        // $A Igor Oct08;
        {
            Report(ReportType.Warning, null /* location */, message, null /* ex */ );
        }



        // INFO LAUNCHING FUNCTIONS:


        /// <summary>Launches an info.</summary>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public void ReportInfo(Exception ex)
        // $A Igor Oct08;
        {
            Report(ReportType.Info, null /* location */ , null /* message */, ex);
        }

        /// <summary>Launches an info.</summary>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        public void ReportInfo(string location, string message)
        // $A Igor Oct08;
        {
            Report(ReportType.Info, location, message, null /* ex */ );
        }

        /// <summary>Launches an info.</summary>
        /// <param name="message">User provided message included in the report.</param>
        public void ReportInfo(string message)
        {
            Report(ReportType.Info, null /* location */, message, null /* ex */ );
        }

        #endregion  // Reporting_Types

        #endregion   // Reporting_General


        #region Reporting_Specific



        #region Reporting_TextWriter

        //Data & its manipulation: 

        private bool _UseTextWriter = false;

        /// <summary>Gets or sets the flag specifying whether reporting using a text writer is performed or not.</summary>
        public bool UseTextWriter { get { return _UseTextWriter; } set { _UseTextWriter = value; } }


        private List<TextWriter> Writers = new List<TextWriter>();

        private List<String> FileNames = new List<String>();

        private TextWriter Writer = null;
        private bool DisposeWriter = false;  // specifies that writer must be disposed when it is changed.

        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Writer to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetWriter(TextWriter writer)
        {
            bool ret = false;
            try
            {
                if (DisposeWriter)
                    if (Writer != null)
                        Writer.Dispose();
                Writer = null;
            }
            catch { }
            DisposeWriter = false;
            Writer = writer;
            if (Writer != null)
                ret = true;
            return ret;
        }


        /// <summary>Creates a TextWriter upon the stream and sets it as the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetWriter(Stream stream)
        {
            bool ret = false;
            try
            {
                if (DisposeWriter)
                    if (Writer != null)
                        Writer.Dispose();
                Writer = null;
            }
            catch { }
            DisposeWriter = true;  // must be disposed when not used any more
            TextWriter writer = new StreamWriter(stream);
            Writer = writer;
            if (Writer != null)
                ret = true;
            return ret;
        }



        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetWriter(string filename)
        {
            bool ret = false;
            try
            {
                if (DisposeWriter)
                    if (Writer != null)
                        Writer.Dispose();
                Writer = null;
            }
            catch { }
            DisposeWriter = true;  // must be disposed when not used any more
            TextWriter writer = new StreamWriter(filename, true  /* append */ );
            Writer = writer;
            if (Writer != null)
                ret = true;
            return ret;
        }

        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Stream to which reporting will be performed.</param>
        /// <param name="overwrite">If true then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetWriter(string filename, bool overwrite)
        {
            bool ret = false;
            try
            {
                if (DisposeWriter)
                    if (Writer != null)
                        Writer.Dispose();
                Writer = null;
            }
            catch { }
            DisposeWriter = true;  // must be disposed when not used any more
            TextWriter writer = new StreamWriter(filename, !overwrite);
            Writer = writer;
            if (Writer != null)
                ret = true;
            return ret;
        }


        // Delegates with default values:

        /// <summary>Delegate that performs reporting (actually logging) via text writer.</summary>
        public ReportDelegate ReportDlgTextWriter = new ReportDelegate(DefaultReport_TextWriter);

        /// <summary>Delegate that assembles the location string for reporting via console form.</summary>
        public ReportLocationDelegate ReportLocationDlgTextWriter = new ReportLocationDelegate(DefaultReportLocation_TextWriter);

        /// <summary>Delegate that assembles the message string for reporting via text writer.</summary>
        public ReportMessageDelegate ReportMessageDlgTextWriter = new ReportMessageDelegate(DefaultReportMessage_TextWriter);


        // Default delegate methods for reporting via console form:

        /// <summary>Writes the message msg to all output streams and files registered with the reporter.</summary>
        /// <param name="reporter">Reporter used for reporting, containing information about output streams and files.</param>
        /// <param name="msg">String to be written to output streams and files.</param>
        /// <returns>Number of failures (0 if the message could be output to all streams and files specified).</returns>
        protected static int WriteMessage(Reporter reporter, string msg)
        {
            int numwritten = 0, numerrors = 0;
            try
            {
                // Write to the basic output stream:
                if (reporter.Writer != null)
                {
                    ++numerrors;
                    reporter.Writer.Write(msg);
                    --numerrors;
                    ++numwritten;
                }
            }
            catch { }
            try
            {
                if (reporter.Writers != null)
                    for (int i = 0; i < reporter.Writers.Count; ++i)
                    {
                        TextWriter writer = reporter.Writers[i];
                        if (writer != null)
                        {
                            try
                            {
                                ++numerrors;
                                writer.Write(msg);
                                --numerrors;
                                ++numwritten;
                            }
                            catch { }
                        }
                    }
            }
            catch { }
            try
            {
                if (reporter.FileNames != null)
                    for (int i = 0; i < reporter.FileNames.Count; ++i)
                    {
                        string filename = reporter.FileNames[i];
                        if (filename != null)
                        {
                            try
                            {
                                ++numerrors;
                                FileInfo fi = new FileInfo(filename);
                                TextWriter writer = new StreamWriter(filename, true /* append */ );
                                writer.Write(msg);
                                --numerrors;
                                ++numwritten;
                            }
                            catch { }
                        }
                    }
            }
            catch { }
            //if (numerrors > 0)
            //{
            //    throw new Exception("Errors occurred during outpur of the report. Number of failures: " + numerrors + ".");
            //}
            return numerrors;
        }

        /// <summary>Default delegate for launching reports (actually logging reports) via text writer.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_TextWriter(Reporter reporter, ReportType messagetype,
            string location, string message)
        {
            if (reporter == null)
                throw new Exception("The reporter object containing auxiliary data is not specified.");
            // Assemble the string that is written to the streams:
            string msg = DefaultReportStringConsoleTimeStamp(reporter, messagetype,
                     location, message);
            // Write the string to all registered streams and files:
            int numerrors = WriteMessage(reporter, msg);
            if (numerrors > 0)
            {
                throw new Exception("Errors occurred during outpur of the report. Number of failures: " + numerrors + ".");
            }
        }


        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_TextWriter(Reporter reporter, ReportType messagetype,
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
        protected static string DefaultReportMessage_TextWriter(Reporter reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }

        // Methods for reporting via text writer:

        /// <summary>Launches a report via console form.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_TextWriter(ReportType messagetype, string location, string message, Exception ex)
        {
            lock (lockobj)
            {
                string locationstring = "", messagestring = "";
                if (ReportLocationDlgTextWriter != null)
                    locationstring = ReportLocationDlgTextWriter(this, messagetype, location, ex);
                else if (ReportLocationDlg != null)
                    locationstring = ReportLocationDlg(this, messagetype, location, ex);
                else
                {
                    // No delegate for assembling location string:
                    if (!string.IsNullOrEmpty(location))
                        locationstring += location;
                }
                if (ReportMessageDlgTextWriter != null)
                    messagestring = ReportMessageDlgTextWriter(this, messagetype, message, ex);
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
                if (ReportDlgTextWriter != null)
                    ReportDlgTextWriter(this, messagetype, locationstring, messagestring);
                else
                    throw new Exception("Stream reporting (logging) delegate is not specified.");
            }
        }


        #endregion   // Reporting_TextWriter


        #endregion  // Reporting_Specific


        #endregion   //  Reporting


        #region Error_Default_methods  // Auxiliary methods, default methods to assign to delegates, etc.

        // A SET OF PRE-DEFINEd (DEFAULT) METHODS FOR ASSIGNMENT TO DELEGATES: 

        // FOR REPORTING BY MESSAGE BOXES:


        /// <summary>Auxiliary method that composes the complete message, including decoration, for reports launched in a message box.</summary>
        /// <param name="reporter">Reporter object used for reporting.</param>
        /// <param name="messagetype">Level of the message that is reported (Error, Warning, ...)</param>
        /// <param name="errorlocation">User description of location that caused the report.</param>
        /// <param name="errormessage">User defined message.</param>
        /// <returns>String that is output by the report.</returns>
        public static string DefaultReportStringMessageBox(Reporter reporter, ReportType messagetype,
                    string errorlocation, string errormessage)
        // $A Igor Oct08;
        {
            string msg = "";
            switch (messagetype)
            {
                case ReportType.Error:
                    if (!string.IsNullOrEmpty(errorlocation))
                        msg += ">>>> ERROR in " + errorlocation + ": ";
                    else
                        msg += ">>>> ERROR: ";
                    break;
                case ReportType.Warning:
                    if (!string.IsNullOrEmpty(errorlocation))
                        msg += ">>>> Warning in " + errorlocation + ": ";
                    else
                        msg += ">>>> Warning: ";
                    break;
                default:
                    if (!string.IsNullOrEmpty(errorlocation))
                        msg += ">>>> Info from " + errorlocation + ": ";
                    else
                        msg += ">>>> Info: ";
                    break;
            }
            msg += Environment.NewLine;
            msg += errormessage;  // message desctibing the error
            msg += Environment.NewLine;
            return msg;
        }

        // FOR REPORTING IN A CONSOLE:

        /// <summary>Auxiliary method that composes the complete message, including decoration, for reports launched in consoles.</summary>
        /// <param name="reporter">Reporter object used for reporting.</param>
        /// <param name="messagetype">Level of the message that is reported (Error, Warning, ...)</param>
        /// <param name="errorlocation">User description of location that caused the report.</param>
        /// <param name="errormessage">User defined message.</param>
        /// <returns>String that is output by the report.</returns>
        public static string DefaultReportStringConsole(Reporter reporter, ReportType messagetype,
                    string errorlocation, string errormessage)
        // $A Igor Oct08 Nov08;
        {
            return DefaultReportStringConsoleBas(reporter, messagetype, errorlocation, errormessage, false);
        }

        /// <summary>Auxiliary method that composes the complete message, including decoration with a TIMESTAMP, 
        /// for reports launched in consoles.</summary>
        /// <param name="reporter">Reporter object used for reporting.</param>
        /// <param name="messagetype">Level of the message that is reported (Error, Warning, ...)</param>
        /// <param name="errorlocation">User description of location that caused the report.</param>
        /// <param name="errormessage">User defined message.</param>
        /// <returns>String that is output by the report.</returns>
        public static string DefaultReportStringConsoleTimeStamp(Reporter reporter, ReportType messagetype,
                    string errorlocation, string errormessage)
        // $A Igor Nov08;
        {
            return DefaultReportStringConsoleBas(reporter, messagetype, errorlocation, errormessage, true);
        }

        /// <summary>Base method for DefaultReportStringConsole and DefaultReportStringConsoleTimeStamp.</summary>
        /// <param name="reporter">Reporter object used for reporting.</param>
        /// <param name="messagetype">Level of the message that is reported (Error, Warning, ...)</param>
        /// <param name="errorlocation">User description of location that caused the report.</param>
        /// <param name="errormessage">User defined message.</param>
        /// <param name="timestamp">Specifies whether to include a time stamp in the message.</param>
        /// <returns>String that is output by the report.</returns>
        public static string DefaultReportStringConsoleBas(Reporter reporter, ReportType messagetype,
                    string errorlocation, string errormessage, bool timestamp)
        // $A Igor Nov08;
        {
            string msg = "";
            msg += Environment.NewLine;
            msg += Environment.NewLine;
            switch (messagetype)
            {
                case ReportType.Error:
                    if (!string.IsNullOrEmpty(errorlocation))
                        msg += "==== ERROR in " + errorlocation + ": ==";
                    else
                        msg += "==== ERROR: ================";
                    break;
                case ReportType.Warning:
                    if (!string.IsNullOrEmpty(errorlocation))
                        msg += "==== Warning in " + errorlocation + ": ==";
                    else
                        msg += "==== Warning: ================";
                    break;
                default:
                    if (!string.IsNullOrEmpty(errorlocation))
                        msg += "==== Info from " + errorlocation + ": ==";
                    else
                        msg += "==== Info: ================";
                    break;
            }
            msg += Environment.NewLine;
            msg += errormessage;  // message desctibing the error
            msg += Environment.NewLine;
            msg += "====";
            // Add a time stamp if applicable:
            if (timestamp)
                msg += DateTime.Now.ToString("  yyyy-MM-dd, HH:mm:ss.FF  ==");
            msg += Environment.NewLine;
            msg += Environment.NewLine;
            return msg;
        }


        /// <summary>Default delegate for reporting.
        /// For parameter descriptions, see ReportDlg.</summary>
        public static void DefaultReportConsole(Reporter reporter, ReportType messagetype,
                    string errorlocation, string errormessage)
        // $A Igor Oct08;
        {
            string msg = DefaultReportStringConsoleTimeStamp(reporter, messagetype,
                     errorlocation, errormessage);
            Console.Write(msg);
        }

        /// <summary>Default delegate for assembly of the location string when reporting on consoles.
        /// For parameter descriptions, see ReportMessageDlg.</summary>
        public static string DefaultLocationString(Reporter reporter, ReportType messagetype,
                    string location, Exception ex)
        // $A Igor Oct08;
        {
            try
            {
                if (string.IsNullOrEmpty(ex.Message))
                    return location;
                else
                {
                    //if (!reporter.DebugMode)
                    //    return location;   // data on ex not taken into account
                    //else
                    return ErrorLocationString0(location, ex);
                }
            }
            catch { }
            return null;
        }


        /// <summary>Default delegate for message assembly of the message string when reporting on consoles.
        /// For parameter descriptions, see ReportMessageDlg.</summary>
        public static string DefaultMessageString(Reporter reporter, ReportType messagetype,
                    string basicmessage, Exception ex)
        // $A Igor Oct08;
        {
            string ret = "";
            try
            {
                string exmessage = null;
                if (ex != null)
                    exmessage = ex.Message;
                if (string.IsNullOrEmpty(basicmessage))
                {
                    if (string.IsNullOrEmpty(exmessage))
                        ret = "<< Unknown error. >>";
                    else
                        ret += exmessage;
                }
                else
                {
                    if (string.IsNullOrEmpty(exmessage))
                        ret += basicmessage;
                    else
                        ret += basicmessage + Environment.NewLine + " Details: " + exmessage;
                }
            }
            catch { }
            return ret;
        }




        // Default function for reserve error reporting (called if an exception is thrown in an error reporting function):

        /// <summary>Default function function for assembling reserve error reporting message.
        /// This is put outside the DefaultReserveReportError() method such that the same assembly
        /// method can be used in different systems. 
        /// The method is considered bulletproof.</param>
        /// <param name="messagetype">Level of the message (Error, Warning,Info, etc.)</param>
        /// <param name="location">Location string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="message">Error message string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="ex">Original exception that was being reported when the error reporting function threw an exception.</param>
        /// <param name="ex1">Exception thrown by the error reporting function.</param>
        public static string DefaultReserveReportMessage(Reporter reporter, ReportType messagetype,
                string location, string message, Exception ex, Exception ex1)
        {
            string msg = "";
            try
            {

                try
                {
                    msg = Environment.NewLine + Environment.NewLine
                        + "************************************************************************"
                        + Environment.NewLine
                        + "ERROR IN REPORTING PROCEDURES." + Environment.NewLine;
                }
                catch { }
                try
                {
                    msg += "Error Description: " + Environment.NewLine + "  " + ex1.Message + Environment.NewLine;
                }
                catch { }
                try
                {
                    if (location != null || message != null || ex != null)
                    {
                        msg += "Original Report: " + Environment.NewLine;
                        if (!string.IsNullOrEmpty(location))
                            msg += "Location: " + location + Environment.NewLine;
                        if (message != null)
                            msg += "Message: " + message + Environment.NewLine;
                        if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                                msg += "Exception's message: " + ex.Message + Environment.NewLine;
                        msg += "Report type: " + messagetype.ToString();
                        if (ex1 != null)
                            if (!string.IsNullOrEmpty(ex1.Message))
                            {
                                msg += Environment.NewLine;
                                msg += "Exception thrown within the reporting system: ";
                                msg += Environment.NewLine + "  ";
                                msg += ex1.Message;
                            }
                    }
                }
                catch { }
                try
                {
                    msg += Environment.NewLine + DateTime.Now.ToString("<< yyyy-MM-dd, HH:mm:ss.FF >>");

                    msg += Environment.NewLine
                        + "************************************************************************"
                        + Environment.NewLine + Environment.NewLine;
                }
                catch { }
            }
            catch (Exception)
            {
                // Assembly of error message itself generated an error, use the last resort assembly procedure:
                try
                {
                    msg += Environment.NewLine + Environment.NewLine
                        + "== CRITICAL ERROR OCCURRED IN THE REPORTING SYSTEM: == " + Environment.NewLine +
                        "Assembly of message for reserve error report has failed. Using last resort procedure."
                        + Environment.NewLine + Environment.NewLine;
                }
                catch { }
                try
                {
                    if (!string.IsNullOrEmpty(location))
                        msg += "User provided location of the original report: " + Environment.NewLine +
                                "  " + location + Environment.NewLine;
                    if (!string.IsNullOrEmpty(message))
                        msg += "User provided description of the original report: " + Environment.NewLine +
                                "  " + message + Environment.NewLine;
                    if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
                            msg += "Original exception description: " + Environment.NewLine +
                                    "  " + ex.Message + Environment.NewLine;
                    if (ex1 != null) if (!string.IsNullOrEmpty(ex1.Message))
                            msg += Environment.NewLine + "Exception raised in the assembly method: " + Environment.NewLine +
                                    "  " + ex1.Message + Environment.NewLine;
                    msg += Environment.NewLine;
                }
                catch { }
            }
            return msg;
        }

        /// <summary>Default function function for reserve error reporting (called if an exception is thrown in an error reporting function).
        /// Writes a report to the application'result standard console (if defined).</summary>
        /// <param name="reporter">Reporter object whre the method can get additional information.</param>
        /// <param name="messagetype">Level of the message (Error, Warning,Info, etc.)</param>
        /// <param name="location">Location string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="message">Error message string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="ex">Original exception that was being reported when the error reporting function threw an exception.</param>
        /// <param name="ex1">Exception thrown by the error reporting function.</param>
        public static void DefaultReserveReportError(Reporter reporter, ReportType messagetype,
                string location, string message, Exception ex, Exception ex1)
        // $A Igor Oct08;
        {
            string msg = "";
            try
            {
                // Create reserve error report'result message:
                msg = DefaultReserveReportMessage(reporter, messagetype,
                        location, message, ex, ex1);
            }
            catch { }
            try
            {
                // Output the message to a console:
                Console.Write(msg);
            }
            catch { }
            try
            {
                // Output the message to output streams and files registered::
                int numerrors = WriteMessage(reporter, msg);
            }
            catch { }
        }

        #endregion Error_Default_methods  // Default methods to assign to delegates.


        #region Error_Auxiliary  // Auxiliary functions, default functions to assign to delegates, etc.

        /// <summary>Returns location string derived from ex, which includes information about the location where error occurred,
        /// specified by the source file name, function and line and column numbers.</summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ErrorLocationString0(Exception ex)
        // $A Igor Oct08;
        {

            string locationstr = "", functionname = "", filename = "";
            int line = -1, column = -1;
            StackTrace trace = null;
            try
            {
                try
                {
                    // Extract info about error location:
                    trace = new StackTrace(ex, true);
                    functionname = trace.GetFrame(0).GetMethod().Name;
                    filename = trace.GetFrame(0).GetFileName();
                    line = trace.GetFrame(0).GetFileLineNumber();
                    column = trace.GetFrame(0).GetFileColumnNumber();
                }
                catch { }
                locationstr += functionname;
                if (line > 0 && column >= 0 || !string.IsNullOrEmpty(filename))
                {
                    if (!string.IsNullOrEmpty(locationstr))
                        locationstr += " ";
                    locationstr += "<";
                    if (!string.IsNullOrEmpty(filename))
                        locationstr += Path.GetFileName(filename);
                    if (line > 0 && column >= 0)
                    {
                        locationstr += "[" + line.ToString() + "," + column.ToString() + "]";
                    }
                    locationstr += ">";
                }
            }
            catch { }
            return locationstr;
        }

        /// <summary>Returns Error location string derived from ex, which includes information about location of
        /// error occurrence and is prepended by additional location information (such as class name)</summary>
        /// <param name="location"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ErrorLocationString0(string location, Exception ex)
        // $A Igor Oct08;
        {
            string ret = "<< Unknown location >>",
                exstr = null;
            try
            {
                if (ex != null)
                    exstr = ErrorLocationString0(ex);
                if (string.IsNullOrEmpty(exstr))
                {
                    if (!string.IsNullOrEmpty(location))
                        ret = location;
                }
                else
                {
                    if (string.IsNullOrEmpty(location))
                        ret = exstr;
                    else
                        ret = location + "." + exstr;
                }
            }
            catch { }
            return ret;
        }

        #endregion Error_Auxiliary  // Auxiliary methods, default methods to assign to delegates, etc.


        //#region Reporting_Static

        //// GLOBAL GENERAL REPORTING METHODS (for all kinds of reports):
        //// Since there should be only one global reporter per process, these methods don't need to be defined in
        //// derived classes, but one must ensure that global is set to the right class.

        //// Lock for the global reporter:
        //private static object lockobjglobal = new object();

        ///// <summary>Basic global reporting method (overloaded). Launches an error report, a warning report or s kind of report/message.
        ///// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        ///// are obtained from the class' instance.</summary>
        ///// <param name="messagetype">The type of the report (e.g. Error, Warning, etc.).</param>
        ///// <param name="location">User-provided description of error location.</param>
        ///// <param name="message">User-provided description of error.</param>
        ///// <param name="ex">Exception thrown when error occurred.</param>
        //public static void ReportS(ReportType messagetype, string location, string message, Exception ex)
        //// $A Igor Oct08;
        //{
        //    string locationstr = "", messagestr = "";
        //    try
        //    {
        //        lock (lockobjglobal)
        //        {
        //            Reporter rep = Global;
        //            if (rep != null)
        //            {
        //                if (rep.ReportLocationDlg != null)
        //                    locationstr = rep.ReportLocationDlg(rep, messagetype, location, ex);
        //                if (rep.ReportMessageDlg != null)
        //                    messagestr = rep.ReportMessageDlg(rep, messagetype, message, ex);
        //                if (rep.ReportDlg != null)
        //                    rep.ReportDlg(rep, messagetype, locationstr, messagestr);
        //            }
        //        }
        //    }
        //    catch (Exception ex1)
        //    {
        //        Reporter.DefaultReserveReportError(null, ReportType.Error, location, message, ex, ex1);
        //    }
        //}

        //// Overloaded general reporting methods (different combinations of parameters passed):

        ///// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        ///// <param name="message">User-provided description of error.</param>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        //public static void ReportS(ReportType messagetype, string message, Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(messagetype, null /* location */, message, ex);
        //}

        ///// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        ///// <param name="location">User-provided description of error location.</param>
        //public static void ReportS(ReportType messagetype, Exception ex, string location)
        //// $A Igor Oct08;
        //{
        //    ReportS(messagetype, location, null /* message */, ex);
        //}

        ///// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        //public static void ReportS(ReportType messagetype, Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(messagetype, null /* location */ , null /* message */, ex);
        //}

        ///// <summary>Launches a report.</summary>
        ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        ///// <param name="location">User provided description of the location where report was triggered.</param>
        ///// <param name="message">User provided message included in the report.</param>
        //public static void ReportS(ReportType messagetype, string location, string message)
        //// $A Igor Oct08;
        //{
        //    ReportS(messagetype, location, message, null /* ex */ );
        //}

        ///// <summary>Launches a report.</summary>
        ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        ///// <param name="message">User provided message included in the report.</param>
        //public static void ReportS(ReportType messagetype, string message)
        //// $A Igor Oct08;
        //{
        //    ReportS(messagetype, null /* location */, message, null /* ex */ );
        //}


        //// GLOBAL ERROR REPORTING FUNCTIONS:


        ///// <summary>Basic error reporting method (overloaded).
        ///// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        ///// are obtained from the class' instance.</summary>
        ///// <param name="location">User-provided description of error location.</param>
        ///// <param name="message">User-provided description of error.</param>
        ///// <param name="ex">Exception thrown when error occurred.</param>
        //public static void ReportErrorS(string location, string message, Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Error, location, message, ex);
        //}

        //// Overloaded general reporting methods (different combinations of parameters passed):

        ///// <summary>Launches an error report.</summary>
        ///// <param name="message">User-provided description of error.</param>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        //public static void ReportErrorS(string message, Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Error, null /* location */, message, ex);
        //}

        ///// <summary>Launches an error report.</summary>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        ///// <param name="location">User-provided description of error location.</param>
        //public static void ReportErrorS(Exception ex, string location)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Error, location, null /* message */, ex);
        //}

        ///// <summary>Launches an error report. Predominantly for error and warning reports.</summary>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        //public static void ReportErrorS(Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Error, null /* location */ , null /* message */, ex);
        //}

        ///// <summary>Launches an error report.</summary>
        ///// <param name="location">User provided description of the location where report was triggered.</param>
        ///// <param name="message">User provided message included in the report.</param>
        //public static void ReportErrorS(string location, string message)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Error, location, message, null /* ex */ );
        //}

        ///// <summary>Launches an error report.</summary>
        ///// <param name="message">User provided message included in the report.</param>
        //public static void ReportErrorS(string message)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Error, null /* location */, message, null /* ex */ );
        //}



        //// GLOBAL WARNING LAUNCHING FUNCTIONS:


        ///// <summary>Basic warning reporting method (overloaded).
        ///// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
        ///// are obtained from the class' instance.</summary>
        ///// <param name="location">User-provided description of error location.</param>
        ///// <param name="message">User-provided description of error.</param>
        ///// <param name="ex">Exception thrown when error occurred.</param>
        //public static void ReportWarningS(string location, string message, Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Warning, location, message, ex);
        //}

        //// Overloaded general reporting methods (different combinations of parameters passed):

        ///// <summary>Launches a warning report.</summary>
        ///// <param name="message">User-provided description of error.</param>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        //public static void ReportWarningS(string message, Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Warning, null /* location */, message, ex);
        //}

        ///// <summary>Launches a warning report.</summary>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        ///// <param name="location">User-provided description of error location.</param>
        //public static void ReportWarningS(Exception ex, string location)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Warning, location, null /* message */, ex);
        //}

        ///// <summary>Launches a warning report. Predominantly for error and warning reports.</summary>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        //public static void ReportWarningS(Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Warning, null /* location */ , null /* message */, ex);
        //}

        ///// <summary>Launches a warning report.</summary>
        ///// <param name="location">User provided description of the location where report was triggered.</param>
        ///// <param name="message">User provided message included in the report.</param>
        //public static void ReportWarningS(string location, string message)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Warning, location, message, null /* ex */ );
        //}

        ///// <summary>Launches a warning report.</summary>
        ///// <param name="message">User provided message included in the report.</param>
        //public static void ReportWarningS(string message)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Warning, null /* location */, message, null /* ex */ );
        //}



        //// GLOBAL INFO LAUNCHING FUNCTIONS:


        ///// <summary>Launches an info.</summary>
        ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        //public static void ReportInfoS(Exception ex)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Info, null /* location */ , null /* message */, ex);
        //}

        ///// <summary>Launches an info.</summary>
        ///// <param name="location">User provided description of the location where report was triggered.</param>
        ///// <param name="message">User provided message included in the report.</param>
        //public static void ReportInfoS(string location, string message)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Info, location, message, null /* ex */ );
        //}

        ///// <summary>Launches an info.</summary>
        ///// <param name="message">User provided message included in the report.</param>
        //public static void ReportInfoS(string message)
        //// $A Igor Oct08;
        //{
        //    ReportS(ReportType.Info, null /* location */, message, null /* ex */ );
        //}


        //#endregion  // Reporting_Static



        #region Error_Reporter_Old

        // GLOBAL ERROR REPORTER:

        //static private Reporter _Global = null;

        //public static Reporter Global
        //{
        //    get { return _Global; }
        //    set { _Global = value; }
        //}


        //// Error Message composition:

        //public virtual string ErrorMessage(string message)
        //{
        //    if (string.IsNullOrEmpty(message))
        //        return "<< Unknown >>";
        //    else return message;
        //}

        //public virtual string ErrorMessage(string location, string message)
        //{
        //    if (string.IsNullOrEmpty(location))
        //        return message;
        //    else
        //        return location + ": " + message;
        //}

        //public virtual string ErrorMessage(Exception ex)
        //{
        //    if (ex == null)
        //        return ErrorMessage((string)null);
        //    else
        //        return ex.Message;
        //}

        //public virtual string ErrorMessage(string location, Exception ex)
        //{
        //    return ErrorMessage(location,ErrorMessage(ex));
        //}


        //// ERROR LOCATION STRINGS:

        //public static string ErrorLocationString(Exception ex)
        //{

        //    string locationstr = "", functionname = "", filename = "";
        //    int line = -1, column = -1;
        //    StackTrace trace = null;
        //    try
        //    {
        //        try
        //        {
        //            // Extract info about error location:
        //            trace = new StackTrace(ex, true);
        //            functionname = trace.GetFrame(0).GetMethod().Name;
        //            filename = trace.GetFrame(0).GetFileName();
        //            line = trace.GetFrame(0).GetFileLineNumber();
        //            column = trace.GetFrame(0).GetFileColumnNumber();
        //        }
        //        catch (Exception) { }
        //        locationstr += functionname;
        //        if (!string.IsNullOrEmpty(locationstr))
        //            locationstr+=" ";
        //        locationstr +=
        //            "< " + Path.GetFileName(filename) +
        //            " (" + line.ToString() +
        //            ", " + column.ToString() +
        //            ") >";
        //    }
        //    catch { }
        //    return locationstr;
        //}

        //public static string ErrorLocationString(string location, Exception ex)
        //{
        //    string ReturnedString="<< Unknown >>",
        //        exstr=null;
        //    try
        //    {
        //        if (ex!=null)
        //            exstr=ErrorLocationString(ex);
        //        if (string.IsNullOrEmpty(exstr))
        //        {
        //            if (!string.IsNullOrEmpty(location))
        //                ReturnedString = location;
        //        } else
        //        {
        //            if (string.IsNullOrEmpty(location))
        //                ReturnedString = exstr;
        //            else
        //                ReturnedString = location + "." + exstr;
        //        }
        //    }
        //    catch { }
        //    return ReturnedString;
        //}

        //public static string ErrorLocationString(string location)
        //{
        //    if (string.IsNullOrEmpty(location))
        //        return "<< Unknown >>";
        //    else
        //        return location;
        //}


        //// TODO: In class provided error reporting procedures, take into account the reporting messagetype of this class!
        //// Also take into account whether debug mode is set, etc.

        //static void ConsoleReportError(ReportType messagetype,string location,string message)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine();
        //    if (!string.IsNullOrEmpty(location))
        //    {
        //        Console.WriteLine("==== ERROR in " + location + ": ==");
        //    } else
        //        Console.WriteLine("==== ERROR: ================");
        //    Console.WriteLine(message);
        //    Console.WriteLine("====");
        //    Console.WriteLine();
        //    Console.WriteLine();
        //}

        //static void ConsoleReportError(ReportType messagetype,string location,Exception ex)
        //{
        //    string locationstr = "";
        //    if (ex != null || !string.IsNullOrEmpty(location))
        //        locationstr = ErrorLocationString(location, ex);
        //    ConsoleReportError(messagetype, location, ex.Message);
        //}


        #endregion Error_Reporter_Old



        #region Testing

        static void Test()
        {
        }

        #endregion  // Testing


    }  //  class Reporter



    ///// <summary>Utilities for error reporting and a global error reporter.</summary>
    //public class Reporter_Old
    //// $A Igor Oct08;
    //{

    //    #region Erorr_Reporting_Global

    //    // Global error reporter:

    //    // TODO: consider whether initialization is nexessary here (maybe it is better to allow only explicit initialization).
    //    private static Reporter _Global = new Reporter();

    //    /// <summary>Gets the global reporter object.
    //    /// This is typically used for configuring the global reporter.</summary>
    //    public static Reporter Global
    //    // $A Igor Oct08;
    //    {
    //        get 
    //        {
    //            if (_Global == null)
    //            {
    //                // Default reserve error reporting method is used because we do not have a
    //                // reporter object reference here:
    //                DefaultReserveReportError(null, ReportType.Error, "Reporter.Global", "Global reporting system is not initialized.", null, null);
    //            }
    //            return _Global; 
    //        }
    //        protected set { _Global = value; }
    //    }

    //    #endregion Erorr_Reporting_Global


    //    #region Error_Reporting_Definition

    //    // Replaceable methods implemented through delagates:
        
    //    /// <summary>Delegate that performs error reporting.
    //    /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
    //    /// assemble error message. Then it uses both to assemble the final decorated error message and launches
    //    /// it in its own way.</summary>
    //    public ReportDelegate ReportDlg = new ReportDelegate(DefaultReportConsole);
        
    //    /// <summary>Delegate that assembles the error location string.</summary>
    //    public ReportLocationDelegate ReportLocationDlg = new ReportLocationDelegate(DefaultLocationString);

    //    /// <summary>Delegate that assembles the eror message string.</summary>
    //    public ReportMessageDelegate ReportMessageDlg = new ReportMessageDelegate(DefaultMessageString);

    //    /// <summary>Delegate that reports an error when the error reporting function throws an exception.
    //    /// Functions assigned to this delegate should really be BULLETPROOF. 
    //    /// It is highly recommended to do reporting in small steps enclosed in try/catch blocks and especially to
    //    /// use the error reporting object very carefully (because it may not be initialized properly, which can
    //    /// also be the reason that the error reporting function crashes).
    //    /// It is higly recommended to first call the DefaultReserveReportError() within the function assigned to this delegate,
    //    /// or at least to use the DefaultReserveReportMessage() method for assembly of the message shown.</summary>
    //    public ReserveReportErrorDelegate ReserveReportErrorDlg = new ReserveReportErrorDelegate(DefaultReserveReportError);

    //    #region Initialization

    //    // Constructors:

    //    /// <summary>Constructor. Initializes all error reporting delegates to default values and sets auxliary object to null.
    //    /// Auxiliary object Obj is set to null.</summary>
    //    public Reporter()
    //    // $A Igor Oct08;
    //    {
    //        Init();
    //    }


    //    /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
    //    /// Delegates that are not specified are set to default values.</summary>
    //    /// <param name="obj">Auxiliary object that will be passed to error reporting delegates when called in local methods.</param>
    //    /// <param name="reportdelegate">Delegates that is called to launc an error report.
    //    /// Methods of this class will pass to this class the auxiliary object, location strings assembled by the location assembling delegate,
    //    /// and error message string assembled by the error message delegate.</param>
    //    /// <param name="locationdelegate">Delegate that is called to assemble the error location string. 
    //    /// The Auxiliary object this.Obj will be internally passed to this delegate any time it is called.</param>
    //    /// <param name="messagedelegate">Delegate that is called to assemble the error message (without decorations).
    //    /// The Auxiliary object this.Obj will be internally passed to this delegate any time it is called.</param>
    //    /// <param name="reservereportdelegate">Delegate that is called to report exceptions that occur within error reporting
    //    /// methods. In particular, this must ne as bullet proof as possible.</param>
    //    public Reporter(object obj,
    //        ReportDelegate reportdelegate,
    //        ReportLocationDelegate locationdelegate,
    //        ReportMessageDelegate messagedelegate,
    //        ReserveReportErrorDelegate reservereportdelegate)
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            this.Obj = obj;
    //            this.ReportDlg = reportdelegate;
    //            this.ReportLocationDlg = locationdelegate;
    //            this.ReportMessageDlg = messagedelegate;
    //            ReserveReportErrorDlg = reservereportdelegate;
    //            //Initialize delegates that were not provided with default values:
    //            InitEnd();
    //        }
    //    }

    //    /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
    //    /// Reserve error reporting delegate is initialized to a default value.
    //    /// Delegates that are not specified are set to default values.</summary>
    //    public Reporter(object obj,
    //        ReportDelegate reportdelegate,
    //        ReportLocationDelegate locationdelegate,
    //        ReportMessageDelegate messagedelegate)
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            Init(obj, reportdelegate, locationdelegate, messagedelegate, null /* reservereportdelegate */ );
    //        }
    //    }

    //    /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and the delegate to perform error reporting tasks.
    //    /// Reserve error reporting delegate is initialized to a default value.
    //    /// Delegates for assembling the error location string and error message string are set to their default values, 
    //    /// which are adapted to console-like eror reporting systems.</summary>
    //    public Reporter(object obj,
    //        ReportDelegate reportdelegate)
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            Init(obj, reportdelegate, null /* locationdelegate */, null /* messagedelegate */, null /* reservereportdelegate */ );
    //        }
    //    }

    //    /// <summary>Constructor. Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
    //    /// Delegates for assembling the error locatin and error message string are set to their default values, 
    //    /// which are adapted to console-like eror reporting systems.</summary>
    //    /// <param name="obj">Auxiliary object that will be passed to error reporting delegates when called in local methods.</param>
    //    /// <param name="reportdelegate">Delegates that is called to launc an error report.
    //    /// Methods of this class will pass to this class the auxiliary object, location strings assembled by the location assembling delegate,
    //    /// and error message string assembled by the error message delegate.</param>
    //    /// <param name="reservereportdelegate">Delegate that is called to report exceptions that occur within error reporting
    //    /// methods. In particular, this must ne as bullet proof as possible.</param>
    //    public Reporter(object obj,
    //        ReportDelegate reportdelegate,
    //        ReserveReportErrorDelegate reservereportdelegate)
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            Init(obj, reportdelegate, null /* locationdelegate */, null /* messagedelegate */,
    //                reservereportdelegate);
    //        }
    //    }
        




    //    // Setting default error reporting behavior:

    //    /// <summary>Sets the error reporting delegate to the default value.
    //    /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
    //    protected virtual void SetDefaultReportDlg()
    //    // $A Igor Oct08;
    //    {
    //        ReportDlg = new ReportDelegate(DefaultReportConsole);
    //    }

    //    /// <summary>Sets the error location assembling delegate to the default value.
    //    /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
    //    protected virtual void SetDefaultReportLocationDlg()
    //    // $A Igor Oct08;
    //    {
    //         ReportLocationDlg = new ReportLocationDelegate(DefaultLocationString);
    //    }

    //    /// <summary>Sets the error message assembling delegate to the default value.
    //    /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
    //    protected virtual void SetDefaultReportMessageDlg()
    //    // $A Igor Oct08;
    //    {
    //        ReportMessageDlg = new ReportMessageDelegate(DefaultMessageString);
    //    }

    //    /// <summary>Sets the reserve error reporting delegate to the default value.
    //    /// The default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
    //    protected virtual void SetDefaultReserveReportErrorDlg()
    //    // $A Igor Oct08;
    //    {
    //         ReserveReportErrorDlg = new ReserveReportErrorDelegate(DefaultReserveReportError);
    //    }

    //    /// <summary>Initializes the missing error reporting delegates to default values.
    //    /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
    //    protected virtual void InitEnd()
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            if (ReportDlg==null)
    //                SetDefaultReportDlg();
    //            if (ReportLocationDlg==null)
    //                SetDefaultReportLocationDlg();
    //            if (ReportMessageDlg==null)
    //                SetDefaultReportMessageDlg();
    //            if (ReserveReportErrorDlg==null)
    //                SetDefaultReserveReportErrorDlg();
    //        }
    //    }

    //    /// <summary>Initializes all error reporting delegates to null.
    //    /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
    //    protected virtual void InitBegin()
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            ReportDlg = null;
    //            ReportLocationDlg=null;
    //            ReportMessageDlg=null;
    //            ReserveReportErrorDlg=null;
    //        }
    //    }


    //    /// <summary>Initializes all error reporting delegates to default values and sets auxliary object to null.
    //    /// Auxiliary object Obj is set to null.</summary>
    //    public virtual void Init()
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            Obj = null;
    //            SetDefaultReportDlg();
    //            SetDefaultReportLocationDlg();
    //            SetDefaultReportMessageDlg();
    //            SetDefaultReserveReportErrorDlg();
    //        }
    //    }

    //    // Setting specific error reporting behavior:

    //    /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
    //    /// Delegates that are not specified are set to default values.</summary>
    //    /// <param name="obj">Auxiliary object that will be passed to error reporting delegates when called in local methods.</param>
    //    /// <param name="reportdelegate">Delegates that is called to launc an error report.
    //    /// Methods of this class will pass to this class the auxiliary object, location strings assembled by the location assembling delegate,
    //    /// and error message string assembled by the error message delegate.</param>
    //    /// <param name="locationdelegate">Delegate that is called to assemble the error location string. 
    //    /// The Auxiliary object this.Obj will be internally passed to this delegate any time it is called.</param>
    //    /// <param name="messagedelegate">Delegate that is called to assemble the error message (without decorations).
    //    /// The Auxiliary object this.Obj will be internally passed to this delegate any time it is called.</param>
    //    /// <param name="reservereportdelegate">Delegate that is called to report exceptions that occur within error reporting
    //    /// methods. In particular, this must ne as bullet proof as possible.</param>
    //    public virtual void Init(object obj,
    //        ReportDelegate reportdelegate,
    //        ReportLocationDelegate locationdelegate,
    //        ReportMessageDelegate messagedelegate,
    //        ReserveReportErrorDelegate reservereportdelegate)
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            this.Obj = obj;
    //            this.ReportDlg = reportdelegate;
    //            this.ReportLocationDlg = locationdelegate;
    //            this.ReportMessageDlg = messagedelegate;
    //            ReserveReportErrorDlg = reservereportdelegate;
    //            //Initialize delegates that were not provided with default values:
    //            InitEnd();
    //        }
    //    }

    //    /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
    //    /// Reserve error reporting delegate is initialized to a default value.
    //    /// Delegates that are not specified are set to default values.</summary>
    //    public virtual void Init(object obj,
    //        ReportDelegate reportdelegate,
    //        ReportLocationDelegate locationdelegate,
    //        ReportMessageDelegate messagedelegate)
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            Init(obj, reportdelegate, locationdelegate, messagedelegate, null /* reservereportdelegate */ );
    //        }
    //    }

    //    /// <summary>Initializes the error reporter by the specified auxiliary object and the delegate to perform error reporting tasks.
    //    /// Reserve error reporting delegate is initialized to a default value.
    //    /// Delegates for assembling the error location string and error message string are set to their default values, 
    //    /// which are adapted to console-like eror reporting systems.</summary>
    //    public virtual void Init(object obj,
    //        ReportDelegate reportdelegate)
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            Init(obj, reportdelegate, null /* locationdelegate */, null /* messagedelegate */,null /* reservereportdelegate */ );
    //        }
    //    }

    //    /// <summary>Initializes the error reporter by the specified auxiliary object and delegates used to perform error reporting tasks.
    //    /// Delegates for assembling the error locatin and error message string are set to their default values, 
    //    /// which are adapted to console-like eror reporting systems.</summary>
    //    /// <param name="obj">Auxiliary object that will be passed to error reporting delegates when called in local methods.</param>
    //    /// <param name="reportdelegate">Delegates that is called to launc an error report.
    //    /// Methods of this class will pass to this class the auxiliary object, location strings assembled by the location assembling delegate,
    //    /// and error message string assembled by the error message delegate.</param>
    //    /// <param name="reservereportdelegate">Delegate that is called to report exceptions that occur within error reporting
    //    /// methods. In particular, this must ne as bullet proof as possible.</param>
    //    public virtual void Init(object obj,
    //        ReportDelegate reportdelegate,
    //        ReserveReportErrorDelegate reservereportdelegate)
    //    // $A Igor Oct08;
    //    {
    //        lock (lockobj)
    //        {
    //            Init(obj, reportdelegate, null /* locationdelegate */, null /* messagedelegate */, 
    //                reservereportdelegate );
    //        }
    //    }

        

    //    #endregion  // Initialization

    //    #endregion // Error_Reporting_Definition


    //    #region Reporter_Data


    //    /// <summary>Indicates that reporting suitable for debugging mode should be performed.
    //    /// A standard flag that can be used by the delegate functions.</summary>
    //    public bool DebugMode = false;

    //    /// <summary>Level of output generated by the reporting system.</summary>
    //    public ReportLevel
    //        ReportingLevel = ReportLevel.Warning,
    //        LoggingLevel = ReportLevel.Info,
    //        TraceLevel = ReportLevel.Warning;

    //    /// <summary>If true then the basic reporting function will throw an exception.
    //    /// This is intended predominantly for testing how the reporter behaves in case of internal errors.
    //    /// When the exception is thrown, the value is set back to false. If we want an exception to be
    //    /// thrown again then the value must be set to true once again.</summary>
    //    public bool ThrowTestException = false;

    //    /// <summary>Auxiliary object used by the delegates that perform error reporting.
    //    /// The object is used to provide additional information used in error reporting, or to provide objects that
    //    /// perform some actions in error reporting tasks, or both. 
    //    /// It is left entirely to error reporting delegates to interpret the object'result contents.</summary>
    //    public object Obj = null;

    //    private object _lockobj = new Object();     // enables thread locking

    //    public virtual object lockobj
    //    {
    //        get { return _lockobj; }
    //    }




    //    #endregion  // Reporter_Data


    //    #region Error_Default_methods  // Auxiliary methods, default methods to assign to delegates, etc.

    //    // A SET OF PRE-DEFINET (DEFAULT) METHODS FOR ASSIGNMENT TO DELEGATES: 

    //    // REPORTING BY MESSAGE BOXES:


    //    /// <summary>Auxiliary method that composes the complete message, including decoration, for reports launched in a message box.</summary>
    //    /// <param name="reporter">Reporter object used for reporting.</param>
    //    /// <param name="messagetype">Level of the message that is reported (Error, Warning, ...)</param>
    //    /// <param name="errorlocation">User description of location that caused the report.</param>
    //    /// <param name="errormessage">User defined message.</param>
    //    /// <returns>String that is output by the report.</returns>
    //    public static string DefaultReportStringMessageBox(Reporter reporter, ReportType messagetype,
    //                string errorlocation, string errormessage)
    //    // $A Igor Oct08;
    //    {
    //        string msg = "";
    //        switch (messagetype)
    //        {
    //            case ReportType.Error:
    //                if (!string.IsNullOrEmpty(errorlocation))
    //                    msg += ">>>> ERROR in " + errorlocation + ": ";
    //                else
    //                    msg += ">>>> ERROR: ";
    //                break;
    //            case ReportType.Warning:
    //                if (!string.IsNullOrEmpty(errorlocation))
    //                    msg += ">>>> Warning in " + errorlocation + ": ";
    //                else
    //                    msg += ">>>> Warning: ";
    //                break;
    //            default:
    //                if (!string.IsNullOrEmpty(errorlocation))
    //                    msg += ">>>> Info from " + errorlocation + ": ";
    //                else
    //                    msg += ">>>> Info: ";
    //                break;
    //        }
    //        msg += Environment.NewLine;
    //        msg += errormessage;  // message desctibing the error
    //        msg += Environment.NewLine;
    //        return msg;
    //    }

    //    // REPORTING IN A CONSOLE:

    //    /// <summary>Auxiliary method that composes the complete message, including decoration, for reports launched in consoles.</summary>
    //    /// <param name="reporter">Reporter object used for reporting.</param>
    //    /// <param name="messagetype">Level of the message that is reported (Error, Warning, ...)</param>
    //    /// <param name="errorlocation">User description of location that caused the report.</param>
    //    /// <param name="errormessage">User defined message.</param>
    //    /// <returns>String that is output by the report.</returns>
    //    public static string DefaultReportStringConsole(Reporter reporter, ReportType messagetype,
    //                string errorlocation, string errormessage)
    //    // $A Igor Oct08 Nov08;
    //    {
    //        return DefaultReportStringConsoleBas(reporter, messagetype, errorlocation, errormessage, false);
    //    }

    //    /// <summary>Auxiliary method that composes the complete message, including decoration with a TIMESTAMP, 
    //    /// for reports launched in consoles.</summary>
    //    /// <param name="reporter">Reporter object used for reporting.</param>
    //    /// <param name="messagetype">Level of the message that is reported (Error, Warning, ...)</param>
    //    /// <param name="errorlocation">User description of location that caused the report.</param>
    //    /// <param name="errormessage">User defined message.</param>
    //    /// <returns>String that is output by the report.</returns>
    //    public static string DefaultReportStringConsoleTimeStamp(Reporter reporter, ReportType messagetype,
    //                string errorlocation, string errormessage)
    //    // $A Igor Nov08;
    //    {
    //        return DefaultReportStringConsoleBas(reporter, messagetype, errorlocation, errormessage, true);
    //    }

    //    /// <summary>Base method for DefaultReportStringConsole and DefaultReportStringConsoleTimeStamp.</summary>
    //    /// <param name="reporter">Reporter object used for reporting.</param>
    //    /// <param name="messagetype">Level of the message that is reported (Error, Warning, ...)</param>
    //    /// <param name="errorlocation">User description of location that caused the report.</param>
    //    /// <param name="errormessage">User defined message.</param>
    //    /// <param name="timestamp">Specifies whether to include a time stamp in the message.</param>
    //    /// <returns>String that is output by the report.</returns>
    //    public static string DefaultReportStringConsoleBas(Reporter reporter, ReportType messagetype,
    //                string errorlocation, string errormessage, bool timestamp)
    //    // $A Igor Nov08;
    //    {
    //        string msg = "";
    //        msg += Environment.NewLine;
    //        msg += Environment.NewLine;
    //        switch (messagetype)
    //        {
    //            case ReportType.Error:
    //                if (!string.IsNullOrEmpty(errorlocation))
    //                    msg += "==== ERROR in " + errorlocation + ": ==";
    //                else
    //                    msg += "==== ERROR: ================";
    //                break;
    //            case ReportType.Warning:
    //                if (!string.IsNullOrEmpty(errorlocation))
    //                    msg += "==== Warning in " + errorlocation + ": ==";
    //                else
    //                    msg += "==== Warning: ================";
    //                break;
    //            default:
    //                if (!string.IsNullOrEmpty(errorlocation))
    //                    msg += "==== Info from " + errorlocation + ": ==";
    //                else
    //                    msg += "==== Info: ================";
    //                break;
    //        }
    //        msg += Environment.NewLine;
    //        msg += errormessage;  // message desctibing the error
    //        msg += Environment.NewLine;
    //        msg += "====";
    //        // Add a time stamp if applicable:
    //        if (timestamp)
    //            msg += DateTime.Now.ToString("  yyyy-MM-dd, HH:mm:ss.FF  ==");
    //        msg += Environment.NewLine;
    //        msg += Environment.NewLine;
    //        return msg;
    //    }


    //    /// <summary>Default delegate for reporting.
    //    /// For parameter descriptions, see ReportDlg.</summary>
    //    public static void DefaultReportConsole(Reporter reporter, ReportType messagetype,
    //                string errorlocation, string errormessage)
    //    // $A Igor Oct08;
    //    {
    //        string msg = DefaultReportStringConsoleTimeStamp(reporter, messagetype,
    //                 errorlocation, errormessage);
    //        Console.Write(msg);
    //    }

    //    /// <summary>Default delegate for assembly of the location string when reporting on consoles.
    //    /// For parameter descriptions, see ReportMessageDlg.</summary>
    //    public static string DefaultLocationString(Reporter reporter, ReportType messagetype,
    //                string location, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        try
    //        {
    //            if (string.IsNullOrEmpty(ex.Message))
    //                return location;
    //            else
    //            {
    //                //if (!reporter.DebugMode)
    //                //    return location;   // data on ex not taken into account
    //                //else
    //                    return ErrorLocationString0(location, ex);
    //            }
    //        }
    //        catch { }
    //        return null;
    //    }


    //    /// <summary>Default delegate for message assembly of the message string when reporting on consoles.
    //    /// For parameter descriptions, see ReportMessageDlg.</summary>
    //    public static string DefaultMessageString(Reporter reporter, ReportType messagetype,
    //                string basicmessage, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        string ReturnedString = "";
    //        try
    //        {
    //            string exmessage = null;
    //            if (ex != null)
    //                exmessage = ex.Message;
    //            if (string.IsNullOrEmpty(basicmessage))
    //            {
    //                if (string.IsNullOrEmpty(exmessage))
    //                    ReturnedString = "<< Unknown error. >>";
    //                else
    //                    ReturnedString += exmessage;
    //            }
    //            else
    //            {
    //                if (string.IsNullOrEmpty(exmessage))
    //                    ReturnedString += basicmessage;
    //                else
    //                    ReturnedString += basicmessage + Environment.NewLine + " Details: " + exmessage;
    //            }
    //        }
    //        catch { }
    //        return ReturnedString;
    //    }




    //    // Default function function for reserve error reporting (called if an exception is thrown in an error reporting function):

    //    /// <summary>Default function function for assembling reserve error reporting message.
    //    /// This is put outside the DefaultReserveReportError() method such that the same assembly
    //    /// method can be used in different systems. 
    //    /// The method is considered bulletproof.</param>
    //    /// <param name="messagetype">Level of the message (Error, Warning,Info, etc.)</param>
    //    /// <param name="location">Location string as passed to the error reporting function that has thrown an exception.</param>
    //    /// <param name="message">Error message string as passed to the error reporting function that has thrown an exception.</param>
    //    /// <param name="ex">Original exception that was being reported when the error reporting function threw an exception.</param>
    //    /// <param name="ex1">Exception thrown by the error reporting function.</param>
    //    public static string DefaultReserveReportMessage(Reporter reporter, ReportType messagetype,
    //            string location, string message, Exception ex, Exception ex1)
    //    {
    //        string msg="";
    //        try
    //        {

    //            try
    //            {
    //                msg = Environment.NewLine + Environment.NewLine 
    //                    + "************************************************************************"
    //                    + Environment.NewLine
    //                    + "ERROR IN REPORTING PROCEDURES." + Environment.NewLine;
    //            }
    //            catch { }
    //            try
    //            {
    //                msg += "Error Description: " + Environment.NewLine + "  " + ex1.Message + Environment.NewLine;
    //            }
    //            catch { }
    //            try
    //            {
    //                if (location != null || message != null || ex != null)
    //                {
    //                    msg += "Original Report: " + Environment.NewLine;
    //                    if (!string.IsNullOrEmpty(location))
    //                        msg += "Location: " + location + Environment.NewLine;
    //                    if (message != null)
    //                        msg += "Message: " + message + Environment.NewLine;
    //                    if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
    //                        msg += "Exception'result message: " + ex.Message + Environment.NewLine;
    //                    msg += "Report type: " + messagetype.ToString();
    //                    if (ex1!=null)
    //                        if (!string.IsNullOrEmpty(ex1.Message))
    //                        {
    //                            msg += Environment.NewLine;
    //                            msg += "Exception thrown within the reporting system: ";
    //                            msg += Environment.NewLine + "  ";
    //                            msg += ex1.Message;
    //                        }
    //                }
    //            }
    //            catch { }
    //            try
    //            {
    //                msg += Environment.NewLine + DateTime.Now.ToString("<< yyyy-MM-dd, HH:mm:ss.FF >>");

    //                msg += Environment.NewLine
    //                    + "************************************************************************"
    //                    + Environment.NewLine + Environment.NewLine;
    //            }
    //            catch { }
    //        }
    //        catch(Exception)
    //        {
    //            // Assembly of error message itself generated an error, use the last resort assembly procedure:
    //            try
    //            {
    //                msg+=Environment.NewLine + Environment.NewLine
    //                    + "== CRITICAL ERROR OCCURRED IN THE REPORTING SYSTEM: == " + Environment.NewLine +
    //                    "Assembly of message for reserve error report has failed. Using last resort procedure."
    //                    +Environment.NewLine + Environment.NewLine;
    //            }
    //            catch { }
    //            try
    //            {
    //                if (!string.IsNullOrEmpty(location))
    //                    msg+="User provided location of the original report: " + Environment.NewLine + 
    //                            "  " + location + Environment.NewLine;
    //                if (!string.IsNullOrEmpty(message))
    //                    msg+="User provided description of the original report: " + Environment.NewLine + 
    //                            "  " + message + Environment.NewLine;
    //                if (ex!=null) if (!string.IsNullOrEmpty(ex.Message))
    //                    msg+="Original exception description: " + Environment.NewLine + 
    //                            "  " + ex.Message + Environment.NewLine;
    //                if (ex1!=null) if (!string.IsNullOrEmpty(ex1.Message))
    //                    msg+= Environment.NewLine + "Exception raised in the assembly method: " + Environment.NewLine + 
    //                            "  " + ex1.Message + Environment.NewLine;
    //                msg+=Environment.NewLine;
    //            }
    //            catch {  }
    //        }
    //        return msg;
    //    }

    //    /// <summary>Default function function for reserve error reporting (called if an exception is thrown in an error reporting function).
    //    /// Writes a report to the application'result standard console (if defined).</summary>
    //    /// <param name="reporter">Reporter object whre the method can get additional information.</param>
    //    /// <param name="messagetype">Level of the message (Error, Warning,Info, etc.)</param>
    //    /// <param name="location">Location string as passed to the error reporting function that has thrown an exception.</param>
    //    /// <param name="message">Error message string as passed to the error reporting function that has thrown an exception.</param>
    //    /// <param name="ex">Original exception that was being reported when the error reporting function threw an exception.</param>
    //    /// <param name="ex1">Exception thrown by the error reporting function.</param>
    //    public static void DefaultReserveReportError(Reporter reporter, ReportType messagetype,
    //            string location, string message, Exception ex, Exception ex1)
    //    // $A Igor Oct08;
    //    {
    //        string msg = "";
    //        try
    //        {
    //            // Create reserve error report'result message:
    //            msg = DefaultReserveReportMessage(reporter, messagetype,
    //                    location, message, ex, ex1);
    //        }
    //        catch {  }
    //        try
    //        {
    //            // Output the message to a console:
    //            Console.Write(msg);
    //        }
    //        catch {  }
    //        try
    //        {
    //            // Output the message to output streams and files registered::
    //            int numerrors = WriteMessage(reporter, msg);
    //        }
    //        catch {  }
    //    }

    //    #endregion Error_Default_methods  // Default methods to assign to delegates.


    //    #region Error_Auxiliary  // Auxiliary functions, default functions to assign to delegates, etc.

    //    /// <summary>Returns location string derived from ex, which includes information about the location where error occurred,
    //    /// specified by the source file name, function and line and column numbers.</summary>
    //    /// <param name="ex"></param>
    //    /// <returns></returns>
    //    public static string ErrorLocationString0(Exception ex)
    //    // $A Igor Oct08;
    //    {

    //        string locationstr = "", functionname = "", filename = "";
    //        int line = -1, column = -1;
    //        StackTrace trace = null;
    //        try
    //        {
    //            try
    //            {
    //                // Extract info about error location:
    //                trace = new StackTrace(ex, true);
    //                functionname = trace.GetFrame(0).GetMethod().Name;
    //                filename = trace.GetFrame(0).GetFileName();
    //                line = trace.GetFrame(0).GetFileLineNumber();
    //                column = trace.GetFrame(0).GetFileColumnNumber();
    //            }
    //            catch { }
    //            locationstr += functionname;
    //            if (line > 0 && column >= 0 || !string.IsNullOrEmpty(filename))
    //            {
    //                if (!string.IsNullOrEmpty(locationstr))
    //                    locationstr += " ";
    //                locationstr += "<";
    //                if (!string.IsNullOrEmpty(filename))
    //                    locationstr += Path.GetFileName(filename);
    //                if (line > 0 && column >= 0)
    //                {
    //                    locationstr += "[" + line.ToString() + "," + column.ToString() + "]";
    //                }
    //                locationstr += ">";
    //            }
    //        }
    //        catch { }
    //        return locationstr;
    //    }

    //    /// <summary>Returns Error location string derived from ex, which includes information about location of
    //    /// error occurrence and is prepended by additional location information (such as class name)</summary>
    //    /// <param name="location"></param>
    //    /// <param name="ex"></param>
    //    /// <returns></returns>
    //    public static string ErrorLocationString0(string location, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        string ReturnedString = "<< Unknown location >>",
    //            exstr = null;
    //        try
    //        {
    //            if (ex != null)
    //                exstr = ErrorLocationString0(ex);
    //            if (string.IsNullOrEmpty(exstr))
    //            {
    //                if (!string.IsNullOrEmpty(location))
    //                    ReturnedString = location;
    //            }
    //            else
    //            {
    //                if (string.IsNullOrEmpty(location))
    //                    ReturnedString = exstr;
    //                else
    //                    ReturnedString = location + "." + exstr;
    //            }
    //        }
    //        catch { }
    //        return ReturnedString;
    //    }

    //    #endregion Error_Auxiliary  // Auxiliary methods, default methods to assign to delegates, etc.


    //    //#region Reporting_Static

    //    //// GLOBAL GENERAL REPORTING METHODS (for all kinds of reports):

    //    //// Lock for the global reporter:
    //    //private static object lockobjglobal=new object();

    //    ///// <summary>Basic global reporting method (overloaded). Launches an error report, a warning report or s kind of report/message.
    //    ///// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
    //    ///// are obtained from the class' instance.</summary>
    //    ///// <param name="messagetype">The type of the report (e.g. Error, Warning, etc.).</param>
    //    ///// <param name="location">User-provided description of error location.</param>
    //    ///// <param name="message">User-provided description of error.</param>
    //    ///// <param name="ex">Exception thrown when error occurred.</param>
    //    //public static void ReportS(ReportType messagetype, string location, string message, Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    string locationstr = "", messagestr = "";
    //    //    try
    //    //    {
    //    //            lock (lockobjglobal)
    //    //            {
    //    //                Reporter rep = Global;
    //    //                if (rep != null)
    //    //                {
    //    //                    if (rep.ReportLocationDlg != null)
    //    //                        locationstr = rep.ReportLocationDlg(rep, messagetype, location, ex);
    //    //                    if (rep.ReportMessageDlg != null)
    //    //                        messagestr = rep.ReportMessageDlg(rep, messagetype, message, ex);
    //    //                    if (rep.ReportDlg != null)
    //    //                        rep.ReportDlg(rep, messagetype, locationstr, messagestr);
    //    //                }                    }
    //    //    }
    //    //    catch (Exception ex1)
    //    //    {
    //    //        Reporter.DefaultReserveReportError(null,ReportType.Error, location, message, ex, ex1);
    //    //    }
    //    //}

    //    //// Overloaded general reporting methods (different combinations of parameters passed):

    //    ///// <summary>Launches a report. Predominantly for error and warning reports.</summary>
    //    ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    ///// <param name="message">User-provided description of error.</param>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    //public static void ReportS(ReportType messagetype, string message, Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(messagetype, null /* location */, message, ex);
    //    //}

    //    ///// <summary>Launches a report. Predominantly for error and warning reports.</summary>
    //    ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    ///// <param name="location">User-provided description of error location.</param>
    //    //public static void ReportS(ReportType messagetype, Exception ex, string location)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(messagetype, location, null /* message */, ex);
    //    //}

    //    ///// <summary>Launches a report. Predominantly for error and warning reports.</summary>
    //    ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    //public static void ReportS(ReportType messagetype, Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(messagetype, null /* location */ , null /* message */, ex);
    //    //}

    //    ///// <summary>Launches a report.</summary>
    //    ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    ///// <param name="location">User provided description of the location where report was triggered.</param>
    //    ///// <param name="message">User provided message included in the report.</param>
    //    //public static void ReportS(ReportType messagetype, string location, string message)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(messagetype, location, message, null /* ex */ );
    //    //}

    //    ///// <summary>Launches a report.</summary>
    //    ///// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    ///// <param name="message">User provided message included in the report.</param>
    //    //public static void ReportS(ReportType messagetype, string message)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(messagetype, null /* location */, message, null /* ex */ );
    //    //}


    //    //// GLOBAL ERROR REPORTING FUNCTIONS:


    //    ///// <summary>Basic error reporting method (overloaded).
    //    ///// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
    //    ///// are obtained from the class' instance.</summary>
    //    ///// <param name="location">User-provided description of error location.</param>
    //    ///// <param name="message">User-provided description of error.</param>
    //    ///// <param name="ex">Exception thrown when error occurred.</param>
    //    //public static void ReportErrorS(string location, string message, Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Error, location, message, ex);
    //    //}

    //    //// Overloaded general reporting methods (different combinations of parameters passed):

    //    ///// <summary>Launches an error report.</summary>
    //    ///// <param name="message">User-provided description of error.</param>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    //public static void ReportErrorS(string message, Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Error, null /* location */, message, ex);
    //    //}

    //    ///// <summary>Launches an error report.</summary>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    ///// <param name="location">User-provided description of error location.</param>
    //    //public static void ReportErrorS(Exception ex, string location)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Error, location, null /* message */, ex);
    //    //}

    //    ///// <summary>Launches an error report. Predominantly for error and warning reports.</summary>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    //public static void ReportErrorS(Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Error, null /* location */ , null /* message */, ex);
    //    //}

    //    ///// <summary>Launches an error report.</summary>
    //    ///// <param name="location">User provided description of the location where report was triggered.</param>
    //    ///// <param name="message">User provided message included in the report.</param>
    //    //public static void ReportErrorS(string location, string message)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Error, location, message, null /* ex */ );
    //    //}

    //    ///// <summary>Launches an error report.</summary>
    //    ///// <param name="message">User provided message included in the report.</param>
    //    //public static void ReportErrorS(string message)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Error, null /* location */, message, null /* ex */ );
    //    //}



    //    //// GLOBAL WARNING LAUNCHING FUNCTIONS:


    //    ///// <summary>Basic warning reporting method (overloaded).
    //    ///// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
    //    ///// are obtained from the class' instance.</summary>
    //    ///// <param name="location">User-provided description of error location.</param>
    //    ///// <param name="message">User-provided description of error.</param>
    //    ///// <param name="ex">Exception thrown when error occurred.</param>
    //    //public static void ReportWarningS(string location, string message, Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Warning, location, message, ex);
    //    //}

    //    //// Overloaded general reporting methods (different combinations of parameters passed):

    //    ///// <summary>Launches a warning report.</summary>
    //    ///// <param name="message">User-provided description of error.</param>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    //public static void ReportWarningS(string message, Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Warning, null /* location */, message, ex);
    //    //}

    //    ///// <summary>Launches a warning report.</summary>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    ///// <param name="location">User-provided description of error location.</param>
    //    //public static void ReportWarningS(Exception ex, string location)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Warning, location, null /* message */, ex);
    //    //}

    //    ///// <summary>Launches a warning report. Predominantly for error and warning reports.</summary>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    //public static void ReportWarningS(Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Warning, null /* location */ , null /* message */, ex);
    //    //}

    //    ///// <summary>Launches a warning report.</summary>
    //    ///// <param name="location">User provided description of the location where report was triggered.</param>
    //    ///// <param name="message">User provided message included in the report.</param>
    //    //public static void ReportWarningS(string location, string message)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Warning, location, message, null /* ex */ );
    //    //}

    //    ///// <summary>Launches a warning report.</summary>
    //    ///// <param name="message">User provided message included in the report.</param>
    //    //public static void ReportWarningS(string message)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Warning, null /* location */, message, null /* ex */ );
    //    //}



    //    //// GLOBAL INFO LAUNCHING FUNCTIONS:


    //    ///// <summary>Launches an info.</summary>
    //    ///// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    //public static void ReportInfoS(Exception ex)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Info, null /* location */ , null /* message */, ex);
    //    //}

    //    ///// <summary>Launches an info.</summary>
    //    ///// <param name="location">User provided description of the location where report was triggered.</param>
    //    ///// <param name="message">User provided message included in the report.</param>
    //    //public static void ReportInfoS(string location, string message)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Info, location, message, null /* ex */ );
    //    //}

    //    ///// <summary>Launches an info.</summary>
    //    ///// <param name="message">User provided message included in the report.</param>
    //    //public static void ReportInfoS(string message)
    //    //// $A Igor Oct08;
    //    //{
    //    //    ReportS(ReportType.Info, null /* location */, message, null /* ex */ );
    //    //}


    //    //#endregion  // Reporting_Static



    //    #region Error_Reporter_Old

    //    // GLOBAL ERROR REPORTER:

    //    //static private Reporter _Global = null;

    //    //public static Reporter Global
    //    //{
    //    //    get { return _Global; }
    //    //    set { _Global = value; }
    //    //}


    //    //// Error Message composition:

    //    //public virtual string ErrorMessage(string message)
    //    //{
    //    //    if (string.IsNullOrEmpty(message))
    //    //        return "<< Unknown >>";
    //    //    else return message;
    //    //}

    //    //public virtual string ErrorMessage(string location, string message)
    //    //{
    //    //    if (string.IsNullOrEmpty(location))
    //    //        return message;
    //    //    else
    //    //        return location + ": " + message;
    //    //}

    //    //public virtual string ErrorMessage(Exception ex)
    //    //{
    //    //    if (ex == null)
    //    //        return ErrorMessage((string)null);
    //    //    else
    //    //        return ex.Message;
    //    //}

    //    //public virtual string ErrorMessage(string location, Exception ex)
    //    //{
    //    //    return ErrorMessage(location,ErrorMessage(ex));
    //    //}


    //    //// ERROR LOCATION STRINGS:

    //    //public static string ErrorLocationString(Exception ex)
    //    //{

    //    //    string locationstr = "", functionname = "", filename = "";
    //    //    int line = -1, column = -1;
    //    //    StackTrace trace = null;
    //    //    try
    //    //    {
    //    //        try
    //    //        {
    //    //            // Extract info about error location:
    //    //            trace = new StackTrace(ex, true);
    //    //            functionname = trace.GetFrame(0).GetMethod().Name;
    //    //            filename = trace.GetFrame(0).GetFileName();
    //    //            line = trace.GetFrame(0).GetFileLineNumber();
    //    //            column = trace.GetFrame(0).GetFileColumnNumber();
    //    //        }
    //    //        catch (Exception) { }
    //    //        locationstr += functionname;
    //    //        if (!string.IsNullOrEmpty(locationstr))
    //    //            locationstr+=" ";
    //    //        locationstr +=
    //    //            "< " + Path.GetFileName(filename) +
    //    //            " (" + line.ToString() +
    //    //            ", " + column.ToString() +
    //    //            ") >";
    //    //    }
    //    //    catch { }
    //    //    return locationstr;
    //    //}

    //    //public static string ErrorLocationString(string location, Exception ex)
    //    //{
    //    //    string ReturnedString="<< Unknown >>",
    //    //        exstr=null;
    //    //    try
    //    //    {
    //    //        if (ex!=null)
    //    //            exstr=ErrorLocationString(ex);
    //    //        if (string.IsNullOrEmpty(exstr))
    //    //        {
    //    //            if (!string.IsNullOrEmpty(location))
    //    //                ReturnedString = location;
    //    //        } else
    //    //        {
    //    //            if (string.IsNullOrEmpty(location))
    //    //                ReturnedString = exstr;
    //    //            else
    //    //                ReturnedString = location + "." + exstr;
    //    //        }
    //    //    }
    //    //    catch { }
    //    //    return ReturnedString;
    //    //}

    //    //public static string ErrorLocationString(string location)
    //    //{
    //    //    if (string.IsNullOrEmpty(location))
    //    //        return "<< Unknown >>";
    //    //    else
    //    //        return location;
    //    //}


    //    //// TODO: In class provided error reporting procedures, take into account the reporting messagetype of this class!
    //    //// Also take into account whether debug mode is set, etc.

    //    //static void ConsoleReportError(ReportType messagetype,string location,string message)
    //    //{
    //    //    Console.WriteLine();
    //    //    Console.WriteLine();
    //    //    if (!string.IsNullOrEmpty(location))
    //    //    {
    //    //        Console.WriteLine("==== ERROR in " + location + ": ==");
    //    //    } else
    //    //        Console.WriteLine("==== ERROR: ================");
    //    //    Console.WriteLine(message);
    //    //    Console.WriteLine("====");
    //    //    Console.WriteLine();
    //    //    Console.WriteLine();
    //    //}

    //    //static void ConsoleReportError(ReportType messagetype,string location,Exception ex)
    //    //{
    //    //    string locationstr = "";
    //    //    if (ex != null || !string.IsNullOrEmpty(location))
    //    //        locationstr = ErrorLocationString(location, ex);
    //    //    ConsoleReportError(messagetype, location, ex.Message);
    //    //}


    //    #endregion Error_Reporter_Old


    //    static void Test()
    //    {
    //    }

    //    #region Reporting
    //    // ACTUAL REPORTING METHDS

    //    #region Reporting_Basic

    //    // ACTUAL REPORTING METHODS (utilizing delegates):

    //    // Last resort error reporting function (bulletproof, called if an exception is thrown inside a reporting function):

    //    /// <summary>Used to report errors within reporting functions.
    //    /// Designed to be bullet proof in order to ensure that improper behavior of the reporting system does not remain unnoticed.</summary>
    //    /// <param name="messagetype"></param>
    //    /// <param name="location"></param>
    //    /// <param name="message"></param>
    //    /// <param name="ex"></param>
    //    /// <param name="ex1"></param>
    //    protected virtual void ReserveReportError(ReportType messagetype, string location,
    //                string message, Exception ex, Exception ex1)
    //    // $A Igor Oct08;
    //    {
    //        try
    //        {
    //            lock (lockobj)
    //            {
    //                if (ReserveReportErrorDlg != null)
    //                {
    //                    ReserveReportErrorDlg(this, messagetype, location, message, ex, ex1);
    //                }
    //                else
    //                    DefaultReserveReportError(this, messagetype, location, message, ex, ex1);
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            DefaultReserveReportError(this, messagetype, location, message, ex, ex1);
    //        }
    //    }

    //    // GENERAL REPORTING METHODS (for all kinds of reports):

    //    /// <summary>Basic reporting method (overloaded). Launches an error report, a warning report or s kind of report/message.
    //    /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
    //    /// are obtained from the class' instance.</summary>
    //    /// <param name="messagetype">The type of the report (e.g. Error, Warning, etc.).</param>
    //    /// <param name="location">User-provided description of error location.</param>
    //    /// <param name="message">User-provided description of error.</param>
    //    /// <param name="ex">Exception thrown when error occurred.</param>
    //    public virtual void Report(ReportType messagetype, string location, string message, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        string locationstr = "", messagestr = "";
    //        try
    //        {
    //            lock (lockobj)
    //            {
    //                try
    //                {
    //                    if (ReportLocationDlg != null)
    //                        locationstr = ReportLocationDlg(this, messagetype, location, ex);
    //                    if (ReportMessageDlg != null)
    //                        messagestr = ReportMessageDlg(this, messagetype, message, ex);
    //                    if (ReportDlg != null)
    //                        ReportDlg(this, messagetype, locationstr, messagestr);
    //                }
    //                catch (Exception ex1)
    //                {
    //                    this.ReserveReportError(ReportType.Error, location,
    //                        "Error in Report. " + message, ex, ex1);
    //                }
    //                if (this.UseTextWriter)
    //                {
    //                    try
    //                    {
    //                        this.Report_TextWriter(messagetype, location, message, ex);
    //                    }
    //                    catch (Exception ex1)
    //                    {
    //                        this.ReserveReportError(ReportType.Error, location,
    //                            "Error in Report_ConsoleForm. " + message, ex, ex1);
    //                    }
    //                }
                    
                    
    //                if (ThrowTestException)
    //                {
    //                    // Throw a test exception:
    //                    throw new Exception("Test exception thrown by the reporter (after reporting has been performted).");
    //                }
    //            }
    //        }
    //        catch (Exception ex1)
    //        {
    //            ThrowTestException = false;  // re-set, such that a test exception is thrown only once
    //            DefaultReserveReportError(null, ReportType.Error, location, message, ex, ex1);
    //        }
    //    }

    //    // Overloaded general reporting methods (different combinations of parameters passed):

    //    /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
    //    /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    /// <param name="message">User-provided description of error.</param>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    public void Report(ReportType messagetype, string message, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(messagetype, null /* location */, message, ex);
    //    }

    //    /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
    //    /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    /// <param name="location">User-provided description of error location.</param>
    //    public void Report(ReportType messagetype, Exception ex, string location)
    //    // $A Igor Oct08;
    //    {
    //        Report(messagetype, location, null /* message */, ex);
    //    }

    //    /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
    //    /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    public void Report(ReportType messagetype, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(messagetype, null /* location */ , null /* message */, ex);
    //    }

    //    /// <summary>Launches a report.</summary>
    //    /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    /// <param name="location">User provided description of the location where report was triggered.</param>
    //    /// <param name="message">User provided message included in the report.</param>
    //    public void Report(ReportType messagetype, string location, string message)
    //    // $A Igor Oct08;
    //    {
    //        Report(messagetype, location, message, null /* ex */ );
    //    }

    //    /// <summary>Launches a report.</summary>
    //    /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
    //    /// <param name="message">User provided message included in the report.</param>
    //    public void Report(ReportType messagetype, string message)
    //    // $A Igor Oct08;
    //    {
    //        Report(messagetype, null /* location */, message, null /* ex */ );
    //    }


    //    // ERROR REPORTING FUNCTIONS:


    //    /// <summary>Basic error reporting method (overloaded).
    //    /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
    //    /// are obtained from the class' instance.</summary>
    //    /// <param name="location">User-provided description of error location.</param>
    //    /// <param name="message">User-provided description of error.</param>
    //    /// <param name="ex">Exception thrown when error occurred.</param>
    //    public virtual void ReportError(string location, string message, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Error, location, message, ex);
    //    }

    //    // Overloaded general reporting methods (different combinations of parameters passed):

    //    /// <summary>Launches an error report.</summary>
    //    /// <param name="message">User-provided description of error.</param>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    public void ReportError(string message, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Error, null /* location */, message, ex);
    //    }

    //    /// <summary>Launches an error report.</summary>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    /// <param name="location">User-provided description of error location.</param>
    //    public void ReportError(Exception ex, string location)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Error, location, null /* message */, ex);
    //    }

    //    /// <summary>Launches an error report. Predominantly for error and warning reports.</summary>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    public void ReportError(Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Error, null /* location */ , null /* message */, ex);
    //    }

    //    /// <summary>Launches an error report.</summary>
    //    /// <param name="location">User provided description of the location where report was triggered.</param>
    //    /// <param name="message">User provided message included in the report.</param>
    //    public void ReportError(string location, string message)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Error, location, message, null /* ex */ );
    //    }

    //    /// <summary>Launches an error report.</summary>
    //    /// <param name="message">User provided message included in the report.</param>
    //    public void ReportError(string message)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Error, null /* location */, message, null /* ex */ );
    //    }



    //    // WARNING LAUNCHING FUNCTIONS:


    //    /// <summary>Basic warning reporting method (overloaded).
    //    /// Supplemental data (such as objects necessary to launch visualize the report or operation typeflags)
    //    /// are obtained from the class' instance.</summary>
    //    /// <param name="location">User-provided description of error location.</param>
    //    /// <param name="message">User-provided description of error.</param>
    //    /// <param name="ex">Exception thrown when error occurred.</param>
    //    public virtual void ReportWarning(string location, string message, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Warning, location, message, ex);
    //    }

    //    // Overloaded general reporting methods (different combinations of parameters passed):

    //    /// <summary>Launches a warning report.</summary>
    //    /// <param name="message">User-provided description of error.</param>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    public void ReportWarning(string message, Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Warning, null /* location */, message, ex);
    //    }

    //    /// <summary>Launches a warning report.</summary>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    /// <param name="location">User-provided description of error location.</param>
    //    public void ReportWarning(Exception ex, string location)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Warning, location, null /* message */, ex);
    //    }

    //    /// <summary>Launches a warning report. Predominantly for error and warning reports.</summary>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    public void ReportWarning(Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Warning, null /* location */ , null /* message */, ex);
    //    }

    //    /// <summary>Launches a warning report.</summary>
    //    /// <param name="location">User provided description of the location where report was triggered.</param>
    //    /// <param name="message">User provided message included in the report.</param>
    //    public void ReportWarning(string location, string message)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Warning, location, message, null /* ex */ );
    //    }

    //    /// <summary>Launches a warning report.</summary>
    //    /// <param name="message">User provided message included in the report.</param>
    //    public void ReportWarning(string message)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Warning, null /* location */, message, null /* ex */ );
    //    }



    //    // INFO LAUNCHING FUNCTIONS:


    //    /// <summary>Launches an info.</summary>
    //    /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
    //    public void ReportInfo(Exception ex)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Info, null /* location */ , null /* message */, ex);
    //    }

    //    /// <summary>Launches an info.</summary>
    //    /// <param name="location">User provided description of the location where report was triggered.</param>
    //    /// <param name="message">User provided message included in the report.</param>
    //    public void ReportInfo(string location, string message)
    //    // $A Igor Oct08;
    //    {
    //        Report(ReportType.Info, location, message, null /* ex */ );
    //    }

    //    /// <summary>Launches an info.</summary>
    //    /// <param name="message">User provided message included in the report.</param>
    //    public void ReportInfo(string message)
    //    {
    //        Report(ReportType.Info, null /* location */, message, null /* ex */ );
    //    }

    //    #endregion   // Reporting_Basic


    //    #region Reporting_Specific



    //    #region Reporting_TextWriter


    //    //Data & its manipulation: 

    //    private bool _UseTextWriter = false;

        ///// <summary>Gets or sets the flag specifying whether reporting using a text writer is performed or not.</summary>
        //public bool UseTextWriter { get { return _UseTextWriter; } set { _UseTextWriter = value; } }

    //    private List<TextWriter> Writers = new List<TextWriter>();

    //    private List<String> FileNames = new List<String>();

    //    private TextWriter Writer = null;
    //    private bool DisposeWriter = false;  // specifies that writer must be disposed when it is changed.

    //    /// <summary>Sets the text writer to which reporting is also performed.</summary>
    //    /// <param name="writer">Writer to which reporting will be performed.</param>
    //    /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
    //    bool SetWriter(TextWriter writer)
    //    {
    //        bool ReturnedString=false;
    //        try
    //        {
    //            if (DisposeWriter)
    //                if (Writer!=null)
    //                    Writer.Dispose();
    //            Writer=null;
    //        }
    //        catch {  }
    //        DisposeWriter=false;
    //        Writer=writer;
    //        if (Writer!=null)
    //            ReturnedString = true;
    //        return ReturnedString;
    //    }


    //    /// <summary>Creates a TextWriter upon the stream and sets it as the text writer to which reporting is also performed.</summary>
    //    /// <param name="writer">Stream to which reporting will be performed.</param>
    //    /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
    //    bool SetWriter(Stream stream)
    //    {
    //        bool ReturnedString=false;
    //         try
    //        {
    //            if (DisposeWriter)
    //                if (Writer!=null)
    //                    Writer.Dispose();
    //            Writer=null;
    //        }
    //        catch {  }
    //        DisposeWriter = true;  // must be disposed when not used any more
    //        TextWriter writer = new StreamWriter(stream);
    //        Writer=writer;
    //        if (Writer!=null)
    //            ReturnedString = true;
    //        return ReturnedString;
    //    }

        

    //    /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.</summary>
    //    /// <param name="writer">Stream to which reporting will be performed.</param>
    //    /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
    //    bool SetWriter(string filename)
    //    {
    //        bool ReturnedString=false;
    //         try
    //        {
    //            if (DisposeWriter)
    //                if (Writer!=null)
    //                    Writer.Dispose();
    //            Writer=null;
    //        }
    //        catch {  }
    //        DisposeWriter = true;  // must be disposed when not used any more
    //        TextWriter writer = new StreamWriter(filename,true  /* append */ );
    //        Writer=writer;
    //        if (Writer!=null)
    //            ReturnedString = true;
    //        return ReturnedString;
    //    }
        
    //    /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.</summary>
    //    /// <param name="writer">Stream to which reporting will be performed.</param>
    //    /// <param name="overwrite">If true then eventual existing contents of the file are overwritten. Otherwise,
    //    /// new text is appended at the end of the file.</param>
    //    /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
    //    bool SetWriter(string filename, bool overwrite)
    //    {
    //        bool ReturnedString=false;
    //         try
    //        {
    //            if (DisposeWriter)
    //                if (Writer!=null)
    //                    Writer.Dispose();
    //            Writer=null;
    //        }
    //        catch {  }
    //        DisposeWriter = true;  // must be disposed when not used any more
    //        TextWriter writer = new StreamWriter(filename, !overwrite);
    //        Writer=writer;
    //        if (Writer!=null)
    //            ReturnedString = true;
    //        return ReturnedString;
    //    }


    //    // Delegates with default values:

    //    /// <summary>Delegate that performs reporting (actually logging) via text writer.</summary>
    //    public ReportDelegate ReportDlgTextWriter = new ReportDelegate(DefaultReport_TextWriter);

    //    /// <summary>Delegate that assembles the location string for reporting via console form.</summary>
    //    public ReportLocationDelegate ReportLocationDlgTextWriter = new ReportLocationDelegate(DefaultReportLocation_TextWriter);

    //    /// <summary>Delegate that assembles the message string for reporting via text writer.</summary>
    //    public ReportMessageDelegate ReportMessageDlgTextWriter = new ReportMessageDelegate(DefaultReportMessage_TextWriter);


    //    // Default delegate methods for reporting via console form:

    //    /// <summary>Writes the message msg to all output streams and files registered with the reporter.</summary>
    //    /// <param name="reporter">Reporter used for reporting, containing information about output streams and files.</param>
    //    /// <param name="msg">String to be written to output streams and files.</param>
    //    /// <returns>Number of failures (0 if the message could be output to all streams and files specified).</returns>
    //    protected static int WriteMessage(Reporter reporter, string msg)
    //    {
    //        int numwritten = 0, numerrors = 0;
    //        try
    //        {
    //            // Write to the basic output stream:
    //            if (reporter.Writer != null)
    //            {
    //                ++numerrors;
    //                reporter.Writer.Write(msg);
    //                --numerrors;
    //                ++numwritten;
    //            }
    //        }
    //        catch { }
    //        try
    //        {
    //            if (reporter.Writers != null)
    //                for (int i = 0; i < reporter.Writers.Count; ++i)
    //                {
    //                    TextWriter writer = reporter.Writers[i];
    //                    if (writer != null)
    //                    {
    //                        try
    //                        {
    //                            ++numerrors;
    //                            writer.Write(msg);
    //                            --numerrors;
    //                            ++numwritten;
    //                        }
    //                        catch { }
    //                    }
    //                }
    //        }
    //        catch { }
    //        try
    //        {
    //            if (reporter.FileNames != null)
    //                for (int i = 0; i < reporter.FileNames.Count; ++i)
    //                {
    //                    string filename = reporter.FileNames[i];
    //                    if (filename != null)
    //                    {
    //                        try
    //                        {
    //                            ++numerrors;
    //                            FileInfo fi = new FileInfo(filename);
    //                            TextWriter writer = new StreamWriter(filename, true /* append */ );
    //                            writer.Write(msg);
    //                            --numerrors;
    //                            ++numwritten;
    //                        }
    //                        catch { }
    //                    }
    //                }
    //        }
    //        catch { }
    //        //if (numerrors > 0)
    //        //{
    //        //    throw new Exception("Errors occurred during outpur of the report. Number of failures: " + numerrors + ".");
    //        //}
    //        return numerrors;
    //    }

    //    /// <summary>Default delegate for launching reports (actually logging reports) via text writer.</summary>
    //    /// <param name="reporter">Reporter object where additional information can be found.</param>
    //    /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
    //    /// <param name="location">Short string desctribing location where report was triggered.</param>
    //    /// <param name="message">Message of the report.</param>
    //    protected static void DefaultReport_TextWriter(Reporter reporter, ReportType messagetype,
    //        string location, string message)
    //    {
    //        if (reporter == null)
    //            throw new Exception("The reporter object containing auxiliary data is not specified.");
    //        // Assemble the string that is written to the streams:
    //        string msg = DefaultReportStringConsoleTimeStamp(reporter, messagetype,
    //                 location, message);
    //        // Write the string to all registered streams and files:
    //        int numerrors=WriteMessage(reporter, msg);
    //        if (numerrors > 0)
    //        {
    //            throw new Exception("Errors occurred during outpur of the report. Number of failures: " + numerrors + ".");
    //        }
    //    }


    //    /// <summary>Delegate for assembling a location string for this kind of report.</summary>
    //    /// <param name="reporter">Reporter object where additional information can be found.</param>
    //    /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
    //    /// <param name="location">User provided location string.</param>
    //    /// <param name="ex">Exception that triggered reporting.</param>
    //    /// <returns>Location string that can be used in a report.</returns>
    //    protected static string DefaultReportLocation_TextWriter(Reporter reporter, ReportType messagetype,
    //            string location, Exception ex)
    //    {
    //        return DefaultLocationString(reporter, messagetype, location, ex);
    //    }

    //    /// <summary>Delegate for assembling a report message for this kind of report.</summary>
    //    /// <param name="reporter">Reporter object where additional information can be found.</param>
    //    /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
    //    /// <param name="basicmessage">User provided message string.</param>
    //    /// <param name="ex">Exception that triggered reporting.</param>
    //    /// <returns>Message string that can be used in a report.</returns>
    //    protected static string DefaultReportMessage_TextWriter(Reporter reporter, ReportType messagetype,
    //            string basicmessage, Exception ex)
    //    {
    //        return DefaultMessageString(reporter, messagetype, basicmessage, ex);
    //    }

    //    // Methods for reporting via text writer:

    //    /// <summary>Launches a report via console form.
    //    /// Report is launched by using special delegates for this kind of reporting.
    //    /// If the corresponding delegates for error location and message are not specified then general delegates
    //    /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
    //    /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
    //    /// <param name="location">User provided location string.</param>
    //    /// <param name="message">User provided message string.</param>
    //    /// <param name="ex">Exception that triggered reporting.</param>
    //    protected virtual void Report_TextWriter(ReportType messagetype, string location, string message, Exception ex)
    //    {
    //        lock (lockobj)
    //        {
    //            string locationstring = "", messagestring = "";
    //            if (ReportLocationDlgTextWriter != null)
    //                locationstring = ReportLocationDlgTextWriter(this, messagetype, location, ex);
    //            else if (ReportLocationDlg != null)
    //                locationstring = ReportLocationDlg(this, messagetype, location, ex);
    //            else
    //            {
    //                // No delegate for assembling location string:
    //                if (!string.IsNullOrEmpty(location))
    //                    locationstring += location;
    //            }
    //            if (ReportMessageDlgTextWriter != null)
    //                messagestring = ReportMessageDlgTextWriter(this, messagetype, message, ex);
    //            else if (ReportMessageDlg != null)
    //                messagestring = ReportMessageDlg(this, messagetype, message, ex);
    //            else
    //            {
    //                // No delegate for assembling message string:
    //                if (!string.IsNullOrEmpty(messagestring))
    //                {
    //                    messagestring += message;
    //                    if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
    //                            messagestring += " Details: ";
    //                }
    //                if (ex != null) if (!string.IsNullOrEmpty(ex.Message))
    //                        messagestring += ex.Message;
    //            }
    //            if (ReportDlgTextWriter != null)
    //                ReportDlgTextWriter(this, messagetype, locationstring, messagestring);
    //            else
    //                throw new Exception("Stream reporting (logging) delegate is not specified.");
    //        }
    //    }


    //    #endregion   // Reporting_TextWriter


    //    #endregion  // Reporting_Specific


    //    #endregion   //  Reporting

    //}  //  class Reporter_Old




}  // namespace IG.Lib
