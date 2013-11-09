using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

using SpeechLib;


namespace IG.ReporterMsgForms
{


    #region Interfaces

    // IReporterSpeech is defined in ReporterMsg_Forms.cs!

    #endregion Interfaces

 
    #region Base_Classes

    // Mainly abstract classes that contain implementation of significant portions of code, which will prevent
    // code duplication.


    /// <summary>Base class for reporter classes that contain either reporting via system console, via message box, 
    /// via speech, or any combination thereof.</summary>
    /// <remarks>Code common to all classes is put here. This is to reduce duplication of code as much as possible, while
    /// at the same time achieving pure behavior, similar as with ReporterConsoleMsgbox_Base.
    /// Interfaces are intended to clearly distinguish between various functionality supported.</remarks>
    public abstract class ReporterConsoleMsgboxSpeech_Base : ReporterConsoleMsgbox_Base, IReporterSpeech
    {


        #region Reporter_ReadConfiguration
        // Reading of reporter settings from configuration files

        protected const string
            KeyUseSpeech = "UseSpeech";

        /// <summary>Reads settings for a specified named group of reporters from the application configuration file.</summary>
        /// <param name="groupname">Name of the group of reporters for which the settings apply.</param>
        protected override void ReadAppSettingsBasic(string groupname)
        // $A Igor Feb09;
        {
            lock (lockobj)
            {
                try
                {
                    // Attention: below must be a call to ReadAppSettingsBasic() and not a call to ReadAppSettings(),
                    // since otherwise infinite recursion is caused.
                    base.ReadAppSettingsBasic(groupname);
                    bool assigned, b = false;
                    GetAppSetting(groupname, KeyUseSpeech, ref b, out assigned);
                    if (assigned) UseSpeech = b;
                }
                catch (Exception ex)
                {
                    ReserveReportError(ReportType.Error, "Reading configuration", null, null, ex);
                }
            }
        }


        #endregion // Reporter_ReadConfiguration



        #region Reporting_General

        // GENERAL REPORTING METHODS (for all kinds of reports):

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
                    // Call specific methods from this class:
                    if (UseSpeech && DoReporting(messagetype))
                    {
                        // Speech is called first because other methods (such as message boxes) can block until user interaction.
                        try
                        {
                            this.Report_Speech(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Reporter.Report_Speech. " + message, ex, ex1);
                        }
                    }
                    try
                    {
                        // Call reporting form a base class (this includes calling reporting delegates
                        // and inherited specific methods):
                        base.Report(messagetype, location, message, ex);
                    }
                    catch (Exception ex1)
                    {
                        this.ReserveReportError(ReportType.Error, location,
                            "Error in Reporter.base.Report. " + message, ex, ex1);
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


        #region Reporting_Speech
        //  REPORTING BY SPEECH:

        private bool _UseSpeech = false;

        /// <summary>Gets or sets the flag specifying whether reporting using a message box is performed or not.</summary>
        public virtual bool UseSpeech { get { return _UseSpeech; } set { _UseSpeech = value; } }


        //Data: 

        private ReportLevel
            _SpeechLevelSignal = ReportLevel.Warning,
            _SpeechLevelMessage = ReportLevel.Off;

        /// <summary>Gets or sets the reporting level for speaking out signals (such as "Warning", "Error", "Information", etc.)</summary>
        public virtual ReportLevel SpeechLevelSignal
        {
            get { return _SpeechLevelSignal; }
            set
            {
                _SpeechLevelSignal = value;
                if (_SpeechLevelMessage > _SpeechLevelSignal)
                    _SpeechLevelMessage = _SpeechLevelSignal;
            }
        }


        /// <summary>Gets or sets the reporting level for speaking out message text.</summary>
        public virtual ReportLevel SpeechLevelMessage
        {
            get { return _SpeechLevelMessage; }
            set
            {
                _SpeechLevelMessage = value;
                if (_SpeechLevelSignal < _SpeechLevelMessage)
                    _SpeechLevelSignal = _SpeechLevelMessage;
            }
        }
        

        private static SpVoice _voice = null; 
        private static object _voiceLock = new object();
        private static int _numSpeaking = 0;


        public static object VoiceLock { get { return _voiceLock; } }

        public static SpVoice Voice
        {
            get { lock (_voiceLock) { return _voice; }  }
            set { lock (_voiceLock) { _voice = value; }  }
        }


        /// <summary>Speaks out a specified text. The caller can specify that articulation of only one text can be
        /// requested simultaneously.</summary>
        /// <param name="text">Text to be articulated.</param>
        /// <param name="only_one">If true then only one text can be articulated at a given time. If the function is
        /// called during articulation of another text then it exits without triggering articulation.</param>
        public static void Articulate(string text, bool only_one)
        {
            if (only_one && _numSpeaking > 0)
                return;
            Articulate(text);
        }

        /// <summary>Speeks a specified text. If functino is called from multiple threads simultaneously then calls are
        /// serialized.</summary>
        /// <param name="text">Text to be spoken.</param>
        public static void Articulate(string text)
        {
            ++_numSpeaking;
            lock(VoiceLock)
            {
                try
                {
                    if (Voice == null)
                        Voice = new SpVoice();
                    Voice.Speak(text, SpeechVoiceSpeakFlags.SVSFDefault);
                }
                catch {  }
            }
            --_numSpeaking;
        }



        // Delegates with default values:

        /// <summary>Delegate that performs error reporting via speech.
        /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
        /// assemble error message. Then it uses both to assemble the final decorated error message and launches
        /// it in its own way.</summary>
        public ReportDelegate ReportDlgSpeech = new ReportDelegate(DefaultReport_Speech);

        /// <summary>Delegate that assembles the error location string for reporting via speech.</summary>
        public ReportLocationDelegate ReportLocationDlgSpeech = new ReportLocationDelegate(DefaultReportLocation_Speech);

        /// <summary>Delegate that assembles the eror message string for reporting via speech.</summary>
        public ReportMessageDelegate ReportMessageDlgSpeech = new ReportMessageDelegate(DefaultReportMessage_Speech);

        // Default delegate methods for reporting via speech:


        /// <summary>Delegat for launching a report via speech.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_Speech(Reporter reporter, ReportType messagetype,
            string location, string message)
        {
            ReporterConsoleMsgboxSpeech_Base rep = reporter as ReporterConsoleMsgboxSpeech_Base;
            if (rep == null)
                throw new ArgumentException("The reporter argument is not specified or is invalid (possible up-casting failure).");
            string text = "";
            bool speakmessage = false;
            switch (messagetype)
            {
                case ReportType.Error:
                    if (rep.SpeechLevelSignal >= ReportLevel.Error)
                        text += "ERROR!! " + Environment.NewLine;
                    if (rep.SpeechLevelMessage >= ReportLevel.Error)
                        speakmessage = true;
                    break;
                case ReportType.Warning:
                    if (rep.SpeechLevelSignal >= ReportLevel.Warning)
                        text += "Warning! " + Environment.NewLine;
                    if (rep.SpeechLevelMessage >= ReportLevel.Warning)
                        speakmessage = true;
                    break;
                case ReportType.Info:
                    if (rep.SpeechLevelSignal >= ReportLevel.Info)
                        text += "Information: " + Environment.NewLine;
                    if (rep.SpeechLevelMessage >= ReportLevel.Info)
                        speakmessage = true;
                    break;
                default:
                    if (rep.SpeechLevelSignal >= ReportLevel.Verbose)
                        text += "Message: " + Environment.NewLine;
                    if (rep.SpeechLevelMessage >= ReportLevel.Verbose)
                        speakmessage = true;
                    break;
            }
            if (speakmessage)
            {
                //if (!string.IsNullOrEmpty(location))
                //    text += "In " + location + ": " + Environment.NewLine;
                if (!string.IsNullOrEmpty(message))
                    text += message + Environment.NewLine + "...";
            }
            if (!string.IsNullOrEmpty(text))
            {
                ArticulateText = text;
                Thread ArtThread = new Thread(new ParameterizedThreadStart(ThreadArticulate));
                ArtThread.Start(text);
                // Articulate(text,true);
            }
        }




        private static string ArticulateText = "";

        private static void ThreadArticulate(object txt)
        {
            Articulate(txt as string, true);
        }



        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_Speech(Reporter reporter, ReportType messagetype,
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
        protected static string DefaultReportMessage_Speech(Reporter reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }

        // Methods for reporting via speech:

        /// <summary>Launches a report via speech.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_Speech(ReportType messagetype, string location, string message, Exception ex)
        {
            string locationstring = "", messagestring = "";
            if (ReportLocationDlgSpeech != null)
                locationstring = ReportLocationDlgSpeech(this, messagetype, location, ex);
            else if (ReportLocationDlg != null)
                locationstring = ReportLocationDlg(this, messagetype, location, ex);
            else
            {
                // No delegate for assembling location string:
                if (!string.IsNullOrEmpty(location))
                    locationstring += location;
            }
            if (ReportMessageDlgSpeech != null)
                messagestring = ReportMessageDlgSpeech(this, messagetype, message, ex);
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
            if (ReportDlgSpeech != null)
                ReportDlgSpeech(this, messagetype, locationstring, messagestring);
            else
                throw new Exception("speech reporting delegate is not specified.");
        }

        #endregion   // Reporting_Speech


    } // class ReporterConsoleMsgboxSpeech_Base


    #endregion Base_Classes



    #region Concrete_Classes

    public class ReporterConsoleMsgboxSpeech : ReporterConsoleMsgboxSpeech_Base,
                        IReporterConsole, IReporterMessageBox, IReporterSpeech
    {


        #region Initialization

        #region Constructors

        // Constructors - this region will be literally included in derived classes, except in special cases):

        /// <summary>Constructor. Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        public ReporterConsoleMsgboxSpeech()
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
        public ReporterConsoleMsgboxSpeech(object obj,
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
        public ReporterConsoleMsgboxSpeech(object obj,
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
        public ReporterConsoleMsgboxSpeech(object obj, ReportDelegate reportdelegate)
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
        public ReporterConsoleMsgboxSpeech(object obj,
            ReportDelegate reportdelegate,
            ReserveReportErrorDelegate reservereportdelegate)
        // $A Igor Oct08;
        {
                Init(obj, reportdelegate, reservereportdelegate);
        }

        #endregion  // Constructors


        #region Initialization_Overridden

        // Setting default error reporting behavior, these methods can be overridden in derived classes:

        /// <summary>Initial part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected override void InitBegin()
        // $A Igor Oct08;
        {
            base.InitBegin();
        }

        /// <summary>Finalizing part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected override void InitEnd()
        // $A Igor Oct08;
        {
            base.InitEnd();
            UseMessageBox = true;
            UseConsole = true;
            UseSpeech = true;
            UseTextLogger = true;
            UseTextWriter = true;
            UseTrace = false;
        }

        #endregion  // Initialization_Overridden

        #endregion  // Initialization



        #region Reporting_Global

        // Global error reporter of this class:
        // In derived classes, this block should be repeated, only with classs name of _Global and Global set to the
        // current (derived) class and with keyword new in property declarations.

        private static ReporterConsoleMsgboxSpeech _Global = null;
        private static bool _GlobalInitialized = false;
        private static object GlobalLock = new object();

        public static new bool GlobalInitialized
        {
            get { return _GlobalInitialized; }
        }

        /// <summary>Gets the global reporter object.
        /// This is typically used for configuring the global reporter.</summary>
        public static new ReporterConsoleMsgboxSpeech Global
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
                            _Global = new ReporterConsoleMsgboxSpeech();
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

    }  // class ReporterConsoleMsgboxSpeech

    #endregion  // Concrete_Classes


}