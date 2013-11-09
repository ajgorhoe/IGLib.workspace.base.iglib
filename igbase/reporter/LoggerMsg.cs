using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Configuration;


namespace IG.Lib
{


    /// <summary>Single record for temporary logging.</summary>
    /// $A Igor Jun10;
    public class LogRecord
    {

        /// <summary>If true then every creation of a new LogRecord is logged to console.</summary>
        public static bool LogToConsole = false;

        #region Construction 

        public LogRecord(ReportType messagetype, string location, string message, Exception ex)
        {
            this.Type = messagetype;
            this.Location = location;
            this.Message = message;
            this.Ex = ex;
            if (LogToConsole)
            {
                Console.WriteLine(Environment.NewLine + ">>>> Logged " + Type.ToString() + ":");
                if (!string.IsNullOrEmpty(Location))
                    Console.WriteLine(location + ": ");
                if (!string.IsNullOrEmpty(Message))
                    Console.WriteLine(Message);
                if (Ex != null)
                {
                    if (!string.IsNullOrEmpty(Ex.Message))
                        Console.WriteLine("Exception details: " + Ex.Message);
                    if (Ex.InnerException != null)
                        if (!string.IsNullOrEmpty(Ex.InnerException.Message))
                            Console.WriteLine("Exception cause: " + Ex.InnerException.Message);
                }
            }
        }

        // Overloaded constructors (different combinations of parameters passed):

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="message">User-provided description of error.</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public LogRecord(ReportType messagetype, string message, Exception ex) : 
                this(messagetype, null /* location */, message, ex)
        {  }

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        /// <param name="location">User-provided description of error location.</param>
        public LogRecord(ReportType messagetype, Exception ex, string location) :
                this(messagetype, location, null /* message */, ex)
        {  }

        /// <summary>Launches a report. Predominantly for error and warning reports.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="ex">Exception that is the cause for launching the report (and from which additional information is extracted).</param>
        public LogRecord(ReportType messagetype, Exception ex) :
            this(messagetype, null /* location */ , null /* message */, ex)
        {  }

        /// <summary>Launches a report.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="location">User provided description of the location where report was triggered.</param>
        /// <param name="message">User provided message included in the report.</param>
        public LogRecord(ReportType messagetype, string location, string message) :
            this(messagetype, location, message, null /* ex */ )
        {  }

        /// <summary>Launches a report.</summary>
        /// <param name="messagetype">Level of the message (Error, Warnind, etc.).</param>
        /// <param name="message">User provided message included in the report.</param>
        public LogRecord(ReportType messagetype, string message) :
            this(messagetype, null /* location */, message, null /* ex */ )
        {  }

        #endregion Construction


        #region Data

        ReportType _type = ReportType.Info;

        /// <summary>Type of the logged message.</summary>
        public ReportType Type { 
            get { return _type; }
            protected set { _type = value; }
        }

        private string _location = null, _message = null;

        /// <summary>Location where message cause occurred.</summary>
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>Message string.</summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private Exception _ex = null;

        /// <summary>Eventual exception that caused the message to be logged.</summary>
        public Exception Ex
        {
            get { return _ex; }
            protected set { _ex = value; }
        }

        #endregion Data

        #region Creation 

        #region CreationGeneral


        /// <summary>Creates and returns a log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord Create(ReportType messagetype, string location, string message, Exception ex)
        {
            return new LogRecord(messagetype, location, message, ex);
        }

        // Overloaded constructors (different combinations of parameters passed):

        /// <summary>Creates and returns a log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord Create(ReportType messagetype, string message, Exception ex)
        {
            return new LogRecord(messagetype, message, ex);
        }

        /// <summary>Creates and returns a log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        public LogRecord Create(ReportType messagetype, Exception ex, string location)
        {
            return new LogRecord(messagetype, ex, location);
        }

        /// <summary>Creates and returns a log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord Create(ReportType messagetype, Exception ex)
        {
            return new LogRecord(messagetype, ex);
        }

        /// <summary>Creates and returns a log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public LogRecord Create(ReportType messagetype, string location, string message)
        {
            return new LogRecord(messagetype, location, message);
        }

        /// <summary>Creates and returns a log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public LogRecord Create(ReportType messagetype, string message)
        {
            return new LogRecord(messagetype, message);
        }

        #endregion CreationGeneral


        #region CreationError

        /// <summary>Creates and returns an error log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord CreateError(string location, string message, Exception ex)
        {
            return Create(ReportType.Error, location, message, ex);
        }

        // Overloaded constructors (different combinations of parameters passed):

        /// <summary>Creates and returns an error log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord CreateError(string message, Exception ex)
        {
            return Create(ReportType.Error, message, ex);
        }

        /// <summary>Creates and returns an error log record initialized according to parameters.</summary>
        /// <param name="ex">Exception that caused creation of log record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        public LogRecord CreateError(Exception ex, string location)
        {
            return Create(ReportType.Error, ex, location);
        }

        /// <summary>Creates and returns an error log record initialized according to parameters.</summary>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord CreateError(Exception ex)
        {
            return Create(ReportType.Error, ex);
        }

        /// <summary>Creates and returns an error log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public LogRecord CreateError(string location, string message)
        {
            return Create(ReportType.Error, location, message);
        }

        /// <summary>Creates and returns an error log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public LogRecord CreateError(string message)
        {
            return Create(ReportType.Error, message);
        }

        #endregion CreationError

        #region CreationWarning

        /// <summary>Creates and returns a warning log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord CreateWarning(string location, string message, Exception ex)
        {
            return Create(ReportType.Warning, location, message, ex);
        }

        // Overloaded constructors (different combinations of parameters passed):

        /// <summary>Creates and returns a warning log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord CreateWarning(string message, Exception ex)
        {
            return Create(ReportType.Warning, message, ex);
        }

        /// <summary>Creates and returns a warning log record initialized according to parameters.</summary>
        /// <param name="ex">Exception that caused creation of log record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        public LogRecord CreateWarning(Exception ex, string location)
        {
            return Create(ReportType.Warning, ex, location);
        }

        /// <summary>Creates and returns a warning log record initialized according to parameters.</summary>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public LogRecord CreateWarning(Exception ex)
        {
            return Create(ReportType.Warning, ex);
        }

        /// <summary>Creates and returns a warning log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public LogRecord CreateWarning(string location, string message)
        {
            return Create(ReportType.Warning, location, message);
        }

        /// <summary>Creates and returns a warning log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public LogRecord CreateWarning(string message)
        {
            return Create(ReportType.Warning, message);
        }


        #endregion CreationWarning

        #region CreationInfo

        /// <summary>Creates and returns an info log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public LogRecord CreateInfo(string location, string message)
        {
            return Create(ReportType.Info, location, message);
        }

        /// <summary>Creates and returns an info log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public LogRecord CreateInfo(string message)
        {
            return Create(ReportType.Info, message);
        }

        #endregion CreationInfo


        #endregion Creation

    }  // class LogRecord

    /// <summary>Temporary logging of errors, warnings and infos for later processing.
    /// It is used to log multiple events in order to be processed (reported or otherwise) later.</summary>
    /// $A Igor Jun10;
    public class Logger
    {

        private List<LogRecord> _logs = new List<LogRecord>();

        /// <summary>Logs that are currently on logger.</summary>
        protected List<LogRecord> Logs
        {
            get { return _logs; }
        }


        #region QueryingReporting

        /// <summary>Removes all logs (if any) currently on the logger.</summary>
        public void Clear()
        {
            Logs.Clear();
        }

        /// <summary>Reports all logs contained in this logger by the specified reporter.</summary>
        /// <param name="reporter">Reporter used to report logged messages.</param>
        public void Report(IReporter reporter)
        {
            if (reporter!=null)
                foreach (LogRecord log in Logs)
                {
                    if (log != null)
                        reporter.Report(log.Type, log.Location, log.Message, log.Ex);
                }
        }

        /// <summary>Reports all logs contained in this logger by the specified reporter,
        /// then clears the logger (removes all logs from it).</summary>
        /// <param name="reporter">Reporter used to report logged messages.</param>
        public void ReportAndClear(IReporter reporter)
        {
            Report(reporter);
            Clear();
        }



        /// <summary>Returns true if logger contains any logs (of any type), false otherwise.</summary>
        public bool HasLogs()
        {
            return Logs.Count > 0;
        }

        /// <summary>Returns true if logger contains any logs of the specified type, false otherwise.</summary>
        public bool HasLogs(ReportType type)
        {
            foreach (LogRecord log in Logs)
            {
                if (log != null)
                    if (log.Type == type)
                        return true;
            }
            return false;
        }

        /// <summary>Returns true if logger contains any error logs false otherwise.</summary>
        public bool HasErrors()
        {
            return HasLogs(ReportType.Error);
        }

        /// <summary>Returns true if logger contains any warning logs false otherwise.</summary>
        public bool HasWarnings()
        {
            return HasLogs(ReportType.Error);
        }

        /// <summary>Returns true if logger contains any info logs false otherwise.</summary>
        public bool HasInfos()
        {
            return HasLogs(ReportType.Info);
        }

        /// <summary>Returns number of logs (of any type) that logger contains.</summary>
        /// <returns></returns>
        public int NumLogs()
        {
            return Logs.Count;
        }

        /// <summary>Returns number of logs of the specified type that logger contains.</summary>
        public int NumLogs(ReportType type)
        {
            int ret = 0;
            foreach (LogRecord log in Logs)
            {
                if (log != null)
                    if (log.Type == type)
                        ++ret;
            }
            return ret;
        }

        /// <summary>Returns number of error logs that logger contains.</summary>
        public int NumErrors()
        {
            return NumLogs(ReportType.Error);
        }

        /// <summary>Returns number of warning logs that logger contains.</summary>
        public int NumWarnings()
        {
            return NumLogs(ReportType.Warning);
        }

        /// <summary>Returns number of info logs that logger contains.</summary>
        public int NumInfos()
        {
            return NumLogs(ReportType.Info);
        }

        /// <summary>Prints an short report corresponding to the specified log record
        /// to the specified StringBuilder. Auxiliary methof used to generate various 
        /// condensed reports without using a separate reporter.</summary>
        /// <param name="log">Log record whose information is printed.</param>
        /// <param name="sb">StringBuilder to which information is printed.</param>
        /// <param name="printDecoration">Whethr a decoration is printed around the report 
        /// (indicates e.g. type of the reported log record - Error, Warning, Info)</param>
        /// <param name="newLineAfter">Whether a newline is printed after the report.</param>
        protected virtual void PrintReport(LogRecord logRecord, StringBuilder sb, bool printDecoration,
            bool newLineAfter)
        {
            if (logRecord==null)
                return;

            if (sb != null)
            {
                if (printDecoration)
                {
                    switch (logRecord.Type)
                    {
                        case ReportType.Error:
                            sb.AppendLine("==== ERROR:");
                            break;
                        case ReportType.Warning:
                            sb.AppendLine("==== Warning:");
                            break;
                        case ReportType.Info:
                            sb.AppendLine("-- Info:");
                            break;
                        default:
                            sb.AppendLine("-- Msg:");
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(logRecord.Location))
                    sb.AppendLine(logRecord.Location + ": ");
                if (!string.IsNullOrEmpty(logRecord.Message))
                    sb.AppendLine(logRecord.Message);
                if (logRecord.Ex != null)
                {
                    if (!string.IsNullOrEmpty(logRecord.Ex.Message))
                        sb.AppendLine("Exception details: " + logRecord.Ex.Message);
                    if (logRecord.Ex.InnerException != null)
                        if (!string.IsNullOrEmpty(logRecord.Ex.InnerException.Message))
                            sb.AppendLine("Exception cause: " + logRecord.Ex.InnerException.Message);
                }
                if (printDecoration)
                {
                        sb.AppendLine("------------");
                }
                if (newLineAfter)
                    sb.AppendLine();
            }
        }


        /// <summary>Returns a stirng that contains reports for all logs contained in the logger.</summary>
        /// <param name="level">Reporting level, logs whose level is greater than prescribed level are not printed.</param>
        /// <param name="printDecorations">Whether decorations are printed or not. Decorations 
        /// outline individual reports and hod information on report types.</param>
        /// <param name="newLinesAfter">Whether newlines are printed after individual reports.</param>
        public string GetReport(ReportLevel level, bool printDecorations, bool newLinesAfter)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogRecord log in Logs)
            {
                if (log!=null)
                    if ((int) log.Type <= (int) level)
                        PrintReport(log, sb, printDecorations, newLinesAfter);
            }
            return sb.ToString();
        }

        /// <summary>Returns a stirng that contains reports for all logs contained in the logger.
        /// Decorations are printed around reports. A newline is printed after each report.</summary>
        /// <param name="level">Reporting level, logs whose level is greater than prescribed level are not printed.</param>
        public string GetReport(ReportLevel level)
        {
            return GetReport(level, true /* printDecorations */, true /* newLinesAfter */);
        }

        /// <summary>Returns a stirng that contains reports for all logs contained in the logger.
        /// Reporting level is Info.
        /// Decorations are printed around reports. A newline is printed after each report.</summary>
        /// <param name="level">Reporting level, logs whose level is greater than prescribed level are not printed.</param>
        public string GetReport()
        {
            return GetReport(ReportLevel.Info, true /* printDecorations */, true /* newLinesAfter */);
        }


        /// <summary>Returns a string that contains reports for all errors contained in the logger.
        /// If there are no logs to be reported then an empty string is returned.</summary>
        /// <param name="printDecorations">Whether decorations are printed or not. Decorations 
        /// outline individual reports and hod information on report types.</param>
        /// <param name="newLinesAfter">Whether newlines are printed after individual reports.</param>
        public string GetErrorsReport(bool printDecorations, bool newLinesAfter)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogRecord log in Logs)
            {
                if (log!=null)
                    if (log.Type==ReportType.Error)
                        PrintReport(log, sb, printDecorations, newLinesAfter);
            }
            return sb.ToString();
        }

        /// <summary>Returns a string that contains reports for all errors contained in the logger.
        /// If there are no logs to be reported then an empty string is returned.</summary>
        public string GetErrorsReport()
        { return GetErrorsReport(false /* printDecorations */, true /* newLinesAfter */ ); }

        /// <summary>Returns a string that contains reports for all warnings contained in the logger.
        /// If there are no logs to be reported then an empty string is returned.</summary>
        /// <param name="printDecorations">Whether decorations are printed or not. Decorations 
        /// outline individual reports and hod information on report types.</param>
        /// <param name="newLinesAfter">Whether newlines are printed after individual reports.</param>
        public string GetWarningsReport(bool printDecorations, bool newLinesAfter)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogRecord log in Logs)
            {
                if (log.Type == ReportType.Warning)
                    PrintReport(log, sb, printDecorations, newLinesAfter);
            }
            return sb.ToString();
        }

        /// <summary>Returns a string that contains reports for all warnings contained in the logger.
        /// If there are no logs to be reported then an empty string is returned.</summary>
        public string GetWarningsReport()
        { return GetWarningsReport(false /* printDecorations */, true /* newLinesAfter */ ); }

        /// <summary>Returns a string that contains reports for all infos contained in the logger.
        /// If there are no logs to be reported then an empty string is returned.</summary>
        /// <param name="printDecorations">Whether decorations are printed or not. Decorations 
        /// outline individual reports and hod information on report types.</param>
        /// <param name="newLinesAfter">Whether newlines are printed after individual reports.</param>
        public string GetInfosReport(bool printDecorations, bool newLinesAfter)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogRecord log in Logs)
            {
                if (log.Type == ReportType.Info)
                    PrintReport(log, sb, printDecorations, newLinesAfter);
            }
            return sb.ToString();
        }

        /// <summary>Returns a string that contains reports for all infos contained in the logger.
        /// If there are no logs to be reported then an empty string is returned.
        /// Decorations are not printed. A newline is printed after each report.</summary>
        public string GetInfosReport()
        { return GetInfosReport(false /* printDecorations */, true /* newLinesAfter */ ); }


        #endregion QueryingReporting


        #region Logging

        #region LoggingGeneral


        /// <summary>Adds a new log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void Log(ReportType messagetype, string location, string message, Exception ex)
        {
            Logs.Add(new LogRecord(messagetype, location, message, ex));
        }

        // Overloaded constructors (different combinations of parameters passed):

        /// <summary>Adds a new log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void Log(ReportType messagetype, string message, Exception ex)
        {
            Logs.Add(new LogRecord(messagetype, message, ex));
        }

        /// <summary>Adds a new log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        public void Log(ReportType messagetype, Exception ex, string location)
        {
            Logs.Add(new LogRecord(messagetype, ex, location));
        }

        /// <summary>Adds a new log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void Log(ReportType messagetype, Exception ex)
        {
            Logs.Add(new LogRecord(messagetype, ex));
        }

        /// <summary>Adds a new log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public void Log(ReportType messagetype, string location, string message)
        {
            Logs.Add(new LogRecord(messagetype, location, message));
        }

        /// <summary>Adds a new log record initialized according to parameters.</summary>
        /// <param name="messagetype">Type of the logged record.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public void Log(ReportType messagetype, string message)
        {
            Logs.Add(new LogRecord(messagetype, message));
        }

        #endregion LoggingGeneral



        #region LoggingError

        /// <summary>Adds a new error log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void LogError(string location, string message, Exception ex)
        {
            Log(ReportType.Error, location, message, ex);
        }

        /// <summary>Adds a new error log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void LogError(string message, Exception ex)
        {
            Log(ReportType.Error, message, ex);
        }

        /// <summary>Adds a new error log record initialized according to parameters.</summary>
        /// <param name="ex">Exception that caused creation of log record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        public void LogError(Exception ex, string location)
        {
            Log(ReportType.Error, ex, location);
        }

        /// <summary>Adds a new error log record initialized according to parameters.</summary>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void LogError(Exception ex)
        {
            Log(ReportType.Error, ex);
        }

        /// <summary>Adds a new error log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public void LogError(string location, string message)
        {
            Log(ReportType.Error, location, message);
        }

        /// <summary>Adds a new error log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public void LogError(string message)
        {
            Log(ReportType.Error, message);
        }


        #endregion LoggingError


        #region LoggingWarning

        /// <summary>Adds a new warning log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void LogWarning(string location, string message, Exception ex)
        {
            Log(ReportType.Warning, location, message, ex);
        }

        /// <summary>Adds a new warning log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void LogWarning(string message, Exception ex)
        {
            Log(ReportType.Warning, message, ex);
        }

        /// <summary>Adds a new warning log record initialized according to parameters.</summary>
        /// <param name="ex">Exception that caused creation of log record.</param>
        /// <param name="location">Description of location where logging occurred.</param>
        public void LogWarning(Exception ex, string location)
        {
            Log(ReportType.Warning, ex, location);
        }

        /// <summary>Adds a new warning log record initialized according to parameters.</summary>
        /// <param name="ex">Exception that caused creation of log record.</param>
        public void LogWarning(Exception ex)
        {
            Log(ReportType.Warning, ex);
        }

        /// <summary>Adds a new warning log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public void LogWarning(string location, string message)
        {
            Log(ReportType.Warning, location, message);
        }

        /// <summary>Adds a new warning log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public void LogWarning(string message)
        {
            Log(ReportType.Warning, message);
        }

        #endregion LoggingWarning


        #region LoggingInfo

        /// <summary>Adds a new info log record initialized according to parameters.</summary>
        /// <param name="location">Description of location where logging occurred.</param>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public void LogInfo(string location, string message)
        {
            Log(ReportType.Info, location, message);
        }

        /// <summary>Adds a new info log record initialized according to parameters.</summary>
        /// <param name="message">Message (or additional explanation) to be logged.</param>
        public void LogInfo(string message)
        {
            Log(ReportType.Info, message);
        }


        #endregion LoggingInfo


        #endregion Logging


    }  // class Logger




}