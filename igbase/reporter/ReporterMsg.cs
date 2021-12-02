using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Configuration;


namespace IG.Lib
{

    /*  Reporter
    TODO:
     * Enumerations:
         If the reporter class is adopted by PADO, then it would make sense to unify the enumeration types such
       as ReportType and ReportLevel. One option is that these enumerations are replaced by PADO's enumerations.
       Another option is that Reporter basic classes are used form IGlib (to avoid duplication) and PASO usese
       enumerations from the Reporter class. This optiion would be the best from the point that it would eliminate
       the need for two parallel development threads, but would imply that PADO should adopt IGlib as its part. 
       If PADO adopted IGLib then then further development of the Reporter's base classes in IGlib could be more 
       easily performed by taking into account specific needs of PADO. 
         If classes that were developed in parallel are merged at a later time, the replacement of the corresponding
       enumerations can be easily done because mebers will be kept the same.
     
    QUESTIONS:
     * For use with PADO:
        Synchronization of tracing may be a problem. Synchronization with settings in the configuration file can be
      achieved in such a way that PadoVariables.PadoTraceSwitch is assigned to the TracingSwitch property, in the
      initialization block of the Reporter.Global get accessor, which will in turn synchronize the TracingLevel
      property. 
        On the other hand, synchronization should also be achieved for setting the tracing level via the Pado
      console. We should take care somehow that each time the PadoVariables.PadoTraceSwitch is changed, it is also
      assigned to the Reporter.Global. Synchronization can not be put in the Reporter.Global accessor itself (except
      in the initialization part) because
    
    WISH LIST (the necessity of individual items is to be discussed):
     * Implement automatic timestamps in messages and a switch to enable or disable them (disabled by default)!
     * What would we do if trace listeners would enable definition of levels for each individual trace listener?
        -- see wish list in http://www.codeproject.com/KB/dotnet/customnettracelisteners.aspx
     * Also enabe switching the timestamp for each individual message
     * 
    */

    #region Types

    // WARNING: do not change values of these two enumerations because the values must correspond!

    /// <summary>Defines the type of a report.</summary>
    public enum ReportType { Error = 1, Warning, Info, Undefined }

#if !NETFRAMEWORK
    /// <summary>Specifies the event type of an event log entry.</summary>
    public enum EventLogEntryType
    {

        /// <summary>An error event. This indicates a significant problem the user should know about;
        //     usually a loss of functionality or data.</summary>
        Error = 1,


        /// <summary>A warning event. This indicates a problem that is not immediately significant,
        //     but that may signify conditions that could cause future problems.</summary>
        Warning = 2,
        //
        // Summary:
        //     An information event. This indicates a significant, successful operation.

        /// <summary>An information event. This indicates a significant, successful operation.</summary>
        Information = 4,

        /// <summary>A success audit event. This indicates a security event that occurs when an audited
        //     access attempt is successful; for example, logging on successfully.</summary>
        SuccessAudit = 8,

        /// <summary>A failure audit event. This indicates a security event that occurs when an audited
        //     access attempt fails; for example, a failed attempt to open a file.</summary>
        FailureAudit = 0x10

    }
#endif

    /// <summary>Defines the level of output when launching reports.</summary>
    public enum ReportLevel { Off = 0, Error, Warning, Info, Verbose }

    public enum ReportSource { Ignore, Unknown, Server, Client, WebService, UserService, ClientOrServer }


    // Delegate types:

    /// <summary>Launches a message (report) about some event.</summary>
    /// <param name="reporter">Reference to the reporter class where all other necessary data is fond.
    /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
    /// <param name="messagetype">Type of the message to be launched.</param>
    /// <param name="errorlocation">User-defined string containing (sometimes supplemental to the Exception object) specification of error location
    /// (e.g. a class or module name)</param>
    /// <param name="errormessage">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
    public delegate void ReportDelegate(ReporterBase reporter, ReportType messagetype,
            string errorlocation, string errormessage);

    /// <summary>Assembles the error location desctiption.</summary>
    /// <param name="reporter">Reference to the reporter class where all other necessary data is fond.
    /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
    /// <param name="messagetype">Type of the message to be launched.</param>
    /// <param name="location">User-provided string containing (sometimes supplemental to the Exception object) specification of error location
    /// (e.g. a class or module name)</param>
    /// <param name="ex">Exception to be reported.</param>
    /// <returns>A string describing error location.</returns>
    public delegate string ReportLocationDelegate(ReporterBase reporter, ReportType messagetype,
                string location, Exception ex);

    /// <summary>Assembles error description (without any decoration, this is added by talling methods).</summary>
    /// <param name="reporter">Reference to the reporter class where all other necessary data is fond.
    /// <param name="messagetype">Type of the message to be launched.</param>
    /// In particular, the Obj member contains a user-set object reference used by the delegate functions.</param>
    /// <param name="basicmessage">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
    /// <param name="ex">Exception to be reported.</param>
    /// <returns>Error description string (error message).</returns>
    public delegate string ReportMessageDelegate(ReporterBase reporter, ReportType messagetype,
                string basicmessage, Exception ex);

    /// <summary>Reports errors occurred in error reporting methods when exceptions are thrown within them.
    /// Methods assigned to these delegates must be bullet proof. They must report the original error (being reported when an
    /// exception occurred) as well as the exception that occurred uin the error reporting method.</summary>
    /// <param name="reporter">Reference to the reporter class where all other necessary data is fond.
    /// In particular, the Obj member contains a user-set object reference used by the delegate functions.
    /// This object should be used with special care within reserve reporting functions because it may be corrupted.</param>
    /// <param name="messagetype">Type of the message to be launched.</param>
    /// <param name="location">User-provided string containing (sometimes supplemental to the Exception object) specification of error location
    /// (e.g. a class or module name)</param>
    /// <param name="message">User-provided string containing (sometimes supplemental to the Exception object) error description.</param>
    /// <param name="ex">Original exception that was being reported.</param>
    /// <param name="ex1">Exception that was thrwn by the error reporting function.</param>
    public delegate void ReserveReportErrorDelegate(ReporterBase reporter, ReportType messagetype,
                string location, string message, Exception ex, Exception ex1);


#endregion  // Types


#region Interfaces



    /// <summary>Interface from which majority of reporters inherit.
    /// Includes generic reporting functionality plus tracinf plus reportinf to files.</summary>
    public interface IReporter : IDisposable, IReporterBase, IReporterTextWriter, IReporterTextLogger, IReporterTrace
    {
    }

    /// <summary>Reporters that utilize system's trace utility.</summary>
    public interface IReporterTrace : IReporterBase
    {
#region Reporting_Trace

        bool UseTrace { get; set; }

#endregion
    }

    /// <summary>Reporters that utilize writing messages to files.
    /// Messages are typically formatted as multi-line messages with distinctive markup.
    /// For one-line possibly indented messages, IReporterTextLogger should be used.</summary>
    public interface IReporterTextWriter : IReporterBase
    {
#region Reporting_TextWriter

        /// <summary>Specifies whether or not TextWriter(s) are used by the Reporter to log messages.</summary>
        bool UseTextWriter { get; set; }

        /// <summary>Gets or sets the introduction string that is written before logging to a TextWriter begins.
        /// If this is not specified then the reporter composes its own introduction string, eventually using 
        /// programname (when defined).</summary>
        string TextWriterIntroText { get; set; }

        /// <summary>String denoting the name of the program or other entity that uses the Reporter for logging.
        /// When introtext is not specified, this name is used in the introduction text composed by the reporter.</summary>
        string TextWriterProgramName { get; set; }

        /// <summary>Specifies whether introduction text is written before logging of messages begins or not.</summary>
        bool TextWriterWriteIntro { get; set; }



        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(TextWriter writer);

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(TextWriter writer, bool writeintro);

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(TextWriter writer, bool writeintro, bool disposewriter);

        /// <summary>Creates a TextWriter upon the stream and sets it as the text writer to which reporting is also performed.</summary>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(Stream stream);

        /// <summary>Creates a TextWriter upon the stream and sets it as the basic TextWriter to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(Stream stream, bool writeintro);

        /// <summary>Creates a TextWriter upon the stream and sets it as the basic TextWriter to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(Stream stream, bool writeintro, bool disposewriter);

        /// <summary>Creates a TextWriter upon a file and sets it as the basic TextWriter to which reporting is also performed.
        /// The file is overwritten.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(string filename);

        /// <summary>Creates a TextWriter upon a file and sets it as the basic TextWriter to which reporting is also performed,
        /// where the caller specifies either to overwrite the file or to append to it.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(string filename, bool append);

        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(string filename, bool append, bool writeintro);

        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextWriter(string filename, bool append, bool writeintro, bool disposewriter);


        // Functionality for having multiple TextWriters:

        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(TextWriter writer);

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(TextWriter writer, bool writeintro);

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(TextWriter writer, bool writeintro, bool disposewriter);

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed.</summary>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(Stream stream);

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(Stream stream, bool writeintro);

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(Stream stream, bool writeintro, bool disposewriter);

        /// <summary>Creates a TextWriter from the file name and adds it to the list of TextWriters on which
        /// reporting is also performed.
        /// The file is overwritten.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(string filename);

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies either to overwrite the file or to append to it.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(string filename, bool append);

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(string filename, bool append, bool writeintro);

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextWriter(string filename, bool append, bool writeintro, bool disposewriter);

        /// <summary>Removes all text writers from the TextWriter subsystem.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if al text writers were successfully removed, false if there were problems.</returns>
        bool RemoveTextWriters();

        /// <summary>Removes the default text writer from the TextWriter subsystem.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed)</returns>
        bool RemoveTextWriter();

        /// <summary>Removes the first object from Writers that contains the specified TextWriter.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        bool RemoveTextWriter(TextWriter writer);

        /// <summary>Removes the first object from Writers whose TextWriter has been created form the specified stream.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        bool RemoveTextWriter(Stream stream);

        /// <summary>Removes the first object from Writers whose TextWriter has been created form the file with the specified name.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        bool RemoveTextWriter(string filename);


#region TextWriter_Custom_Operations

        // This provides support for performing custom operations on TextWriter's output streams. 
        // These are operations taht are not in the scope of commonly supported by teh Repoirter, and
        // enable, for example, to write custom text at specific points.

        /// <summary>Returns a list of all text writers that are currently used by the reporter's text writer.
        /// Only text writers that are actually writable are included.</summary>
        /// <returns>List of text writers that are currently used by the reporter's text writer
        /// (only those that are actually writable are included).</returns>
        List<TextWriter> TextWriterWriters();

        /// <summary>Returns a list of all text writers that are currently used by the reporter's text writer.
        /// Warning: Beware of thread safety issues! 
        /// Blocks of code where the returned list is used should be enclosed in lock(reporter.lockobj){...} block
        /// (where reporter is the object through which this method was called).</summary>
        /// <param name="writableonly">If true then only those text writers are listed that are actually writable.
        /// If false then all text writers are listed.</param>
        /// <returns>List of text writers that are currently used by the reporter's text writer.</returns>
        List<TextWriter> TextWriterWriters(bool writableonly);


        /// <summary>Returns the current number of text writers used by the reporter's text logging module.
        /// Only text writers that are actually writable are counted.</summary>
        /// <returns>The current number of text writers used by the reporter's text logging module 
        /// (only those that are actually writable are counted).</returns>
        int TextWriterNumWriters();

        /// <summary>Returns the current number of TextWriters used by the reporter's text logging module.</summary>
        /// <param name="writableonly">If true then only those text writers are counted that are actually writable.
        /// If false then all text writers are returned.</param>
        /// <returns>The current number of text writers used by the reporter's text logging module.</returns>
        int TextWriterNumWriters(bool writableonly);

        /// <summary>Flushes all text writers of the Writer's TextWriter subsystem.</summary>
        /// <returns>Number of writers that has actually been flushed.</returns>
        int TextWriterFlush();

        /// <summary>Writes a string to all text writers of the Writer's TextWriter subsystem.</summary>
        /// <param name="str">String to be written.</param>
        /// <returns>Number of writers that the string has actually been written to.</returns>
        int TextWriterWrite(string str);

        /// <summary>Similar to TextWriterWrite(), except that a newline is added at the end of the string.</summary>
        int TextWriterWriteLine(string str);

#endregion  // TextWriter_Custom_Operations

#endregion  // Reporting_TextWriter
    }  // interface IReporterTextWriter


    /// <summary>Reporters that utilize logging messages to files.
    /// IReporterTextLogger typically outputs (to a file) messages in one-line format with possibility to define indentation,
    /// while IReporterTextWriter typically outputs multi-line messages formatted for increased visibility. </summary>
    public interface IReporterTextLogger : IReporterBase
    {
#region Reporting_TextLogger

        /// <summary>Specifies whether or not TextLogger(s) are used by the Reporter to log messages.</summary>
        bool UseTextLogger { get; set; }


        // Indentation parameters:

        /// <summary>Gets or sets number fo initial indentation charactyers.</summary>
        int TextLoggerIndentInitial { get; set; }

        /// <summary>Gets or sets the number of indentation characters written per indentation level.</summary>
        int TextLoggerIndentSpacing { get; set; }

        /// <summary>Gets or sets the indentation character.</summary>
        char TextLoggerIndentCharacter { get; set; }


        // Introduction string:

        /// <summary>Gets or sets the introduction string that is written before logging to a TextLogger begins.
        /// If this is not specified then the reporter composes its own introduction string, eventually using 
        /// programname (when defined).</summary>
        string TextLoggerIntroText { get; set; }

        /// <summary>String denoting the name of the program or other entity that uses the Reporter for logging.
        /// When introtext is not specified, this name is used in the introduction text composed by the reporter.</summary>
        string TextLoggerProgramName { get; set; }

        /// <summary>Specifies whether introduction text is written before logging of messages begins or not.</summary>
        bool TextLoggerWriteIntro { get; set; }

        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(TextWriter writer);

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(TextWriter writer, bool writeintro);

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextLogger should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(TextWriter writer, bool writeintro, bool disposewriter);

        /// <summary>Creates a TextLogger upon the stream and sets it as the text writer to which reporting is also performed.</summary>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(Stream stream);

        /// <summary>Creates a TextLogger upon the stream and sets it as the basic TextLogger to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(Stream stream, bool writeintro);

        /// <summary>Creates a TextLogger upon the stream and sets it as the basic TextLogger to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextLogger should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(Stream stream, bool writeintro, bool disposewriter);

        /// <summary>Creates a TextLogger upon a file and sets it as the basic TextLogger to which reporting is also performed.
        /// The file is overwritten.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(string filename);

        /// <summary>Creates a TextLogger upon a file and sets it as the basic TextLogger to which reporting is also performed,
        /// where the caller specifies either to overwrite the file or to append to it.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(string filename, bool append);

        /// <summary>Creates a TextLogger upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(string filename, bool append, bool writeintro);

        /// <summary>Creates a TextLogger upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextLogger should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool SetTextLogger(string filename, bool append, bool writeintro, bool disposewriter);


        // Functionality for having multiple TextLoggers:

        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(TextWriter writer);

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(TextWriter writer, bool writeintro);

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextLogger should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(TextWriter writer, bool writeintro, bool disposewriter);

        /// <summary>Creates a TextLogger from the stream and adds it to the list of TextLoggers on which
        /// reporting is also performed.</summary>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(Stream stream);

        /// <summary>Creates a TextLogger from the stream and adds it to the list of TextLoggers on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(Stream stream, bool writeintro);

        /// <summary>Creates a TextLogger from the stream and adds it to the list of TextLoggers on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextLogger should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(Stream stream, bool writeintro, bool disposewriter);

        /// <summary>Creates a TextLogger from the file name and adds it to the list of TextLoggers on which
        /// reporting is also performed.
        /// The file is overwritten.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(string filename);

        /// <summary>Creates a TextLogger from the stream and adds it to the list of TextLoggers on which
        /// reporting is also performed. The caller specifies either to overwrite the file or to append to it.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(string filename, bool append);

        /// <summary>Creates a TextLogger from the stream and adds it to the list of TextLoggers on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(string filename, bool append, bool writeintro);

        /// <summary>Creates a TextLogger from the stream and adds it to the list of TextLoggers on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextLogger should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        bool AddTextLogger(string filename, bool append, bool writeintro, bool disposewriter);

        /// <summary>Removes all text writers from the TextLogger subsystem.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if al text writers were successfully removed, false if there were problems.</returns>
        bool RemoveTextLoggers();

        /// <summary>Removes the default text writer from the TextLogger subsystem.
        /// If appropriate, the corresponding txt writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed)</returns>
        bool RemoveTextLogger();

        /// <summary>Removes the first object from Loggers that contains the specified TextWriter.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        bool RemoveTextLogger(TextWriter writer);

        /// <summary>Removes the first object from Loggers whose TextWriter has been created form the specified stream.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        bool RemoveTextLogger(Stream stream);

        /// <summary>Removes the first object from Loggers whose TextWriter has been created form the file with the specified name.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        bool RemoveTextLogger(string filename);


#region TextLogger_Custom_Operations

        // This provides support for performing custom operations on TextLogger's output streams. 
        // These are operations taht are not in the scope of commonly supported by teh Repoirter, and
        // enable, for example, to write custom text at specific points.

        /// <summary>Returns a list of all text writers that are currently used by the reporter's text logger.
        /// Only text writers that are actually writable are included.</summary>
        /// <returns>List of text writers that are currently used by the reporter's text logger
        /// (only those that are actually writable are included).</returns>
        List<TextWriter> TextLoggerWriters();

        /// <summary>Returns a list of all text writers that are currently used by the reporter's text logger.
        /// Warning: Beware of thread safety issues! 
        /// Blocks of code where the returned list is used should be enclosed in lock(reporter.lockobj){...} block
        /// (where reporter is the object through which this method was called).</summary>
        /// <param name="writableonly">If true then only those text writers are listed that are actually writable.
        /// If false then all text writers are listed.</param>
        /// <returns>List of text writers that are currently used by the reporter's text logger.</returns>
        List<TextWriter> TextLoggerWriters(bool writableonly);


        /// <summary>Returns the current number of text writers used by the reporter's text logging module.
        /// Only text writers that are actually writable are counted.</summary>
        /// <returns>The current number of text writers used by the reporter's text logging module 
        /// (only those that are actually writable are counted).</returns>
        int TextLoggerNumWriters();

        /// <summary>Returns the current number of TextWriters used by the reporter's text logging module.</summary>
        /// <param name="writableonly">If true then only those text writers are counted that are actually writable.
        /// If false then all text writers are returned.</param>
        /// <returns>The current number of text writers used by the reporter's text logging module.</returns>
        int TextLoggerNumWriters(bool writableonly);

        /// <summary>Flushes all text writers of the Writer's TextLogger subsystem.</summary>
        /// <returns>Number of writers that has actually been flushed.</returns>
        int TextLoggerFlush();

        /// <summary>Writes a string to all text writers of the Writer's TextLogger subsystem.</summary>
        /// <param name="str">String to be written.</param>
        /// <returns>Number of writers that the string has actually been written to.</returns>
        int TextLoggerWrite(string str);


        /// <summary>Similar to TextLoggerWrite(), except that a newline is added at the end of the string.</summary>
        int TextLoggerWriteLine(string str);

#endregion  // TextLogger_Custom_Operations


#endregion  // Reporting_TextLogger
    }  // interface IReporterTextLogger


    /// <summary>Interface from which all reporters inherit.</summary>
    public interface IReporterBase 
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


#region Reporter_ReadConfiguration


        /// <summary>Reads settings for a specified named group of reporters from the application configuration file.</summary>
        /// <param name="groupname">Name of the group of reporters for which the settings apply.</param>
         void ReadAppSettings(string groupname);
        
        /// <summary>Reads common reporter settings (i.e. settings that are not assigned for any named group) 
        /// from the application configuration file.</summary>
         void ReadAppSettings();

        /// <summary>Reads settings for a specified named group of reporters from the application configuration file.</summary>
        /// <param name="groupname">Name of the group of reporters for which the settings apply.</param>
        /// <param name="onlyonce">If true then settings belonging to the specified group are read only once, 
        /// otherwise the settings for this group can be read several times (this is seldom used).</param>
        void ReadAppSettings(string groupname, bool onlyonce);

        /// <summary>Returns a flag that tells whether general configuration settings (not belonging to any group)
        /// have already been read for this reporter.</summary>
        /// <returns>true if settings with the specified name have already been read for teh current reporter, false otherwise.</returns>
        bool AppSettingsRead();

        /// <summary>Returns a flag that tells whether configuration settings with a given group name have already
        /// been read for this reporter.</summary>
        /// <param name="groupname">Name of the settings group, null and "" are treated teh same (denoting the genraal group).</param>
        /// <returns>true if settings with the specified name have already been read for teh current reporter, false otherwise.</returns>
        bool AppSettingsRead(string groupname);


        /// <summary>Gets or sets the flag that specifies whether a warning message is launched when reading of application
        /// settings is attempted more than once for the same named group of settings.</summary>
        bool AppSettingsWarnings { get; set; }

#endregion // Reporter_ReadConfiguration


#region Reporter_Data

        /// <summary>Indicates whether the current reporter is used as a global reporter or not.</summary>
        /// <remarks>This flag is set when the global reporter is initialized.</remarks>
         bool IsGlobal { get; }


         // Indentation for one-line reports:

        /// <summary>Gets or sets the current indentation level for on-line output.
        /// This should normally be done by calling IncreaseDepth() or DecreaseDepth().</summary>
         int Depth { get; set; }

        /// <summary>Increases indentation level by 1.</summary>
        /// <returns>Current indentation level.</returns>
        void IncreaseDepth();

       /// <summary>Increases indentation level by the specified number of levels (can be 0 or negative).</summary>
        /// <param name="numlevels">Number of levels by which indentation is increased.</param>
        /// <returns>Current intentation level.</returns>
        void IncreaseDepth(int numlevels);

        /// <summary>Decreases indentation level by 1.</summary>
        /// <returns>Current indentation level.</returns>
        void DecreaseDepth();

        /// <summary>Decreases indentation level by the specified number of levels (can be 0 or negative).</summary>
        /// <param name="numlevels">Number of levels by which indentation is decreased.</param>
        /// <returns>Current intentation level.</returns>
        void DecreaseDepth(int numlevels);


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


        /// <summary>Returns true if the report of a given type should be shown by user interface (according to ReportingLevel),
        /// and false if not.</summary>
        /// <param name="reptype">Type of the report for which information is returned.</param>
        /// <returns>True if reports of the specific type are launched, false if not.</returns>
        bool DoReporting(ReportType reptype);

        /// <summary>Returns true if the report of a given type should be logged in log files (according to ReportingLevel),
        /// and false if not.</summary>
        /// <param name="reptype">Type of the report for which information is returned.</param>
        /// <returns>True if reports of the specific type are launched, false if not.</returns>
        bool DoLogging(ReportType reptype);

        /// <summary>Returns true if the report of a given type should traced (according to ReportingLevel),
        /// and false if not.</summary>
        /// <param name="reptype">Type of the report for which information is returned.</param>
        /// <returns>True if reports of the specific type are launched, false if not.</returns>
        bool DoTracing(ReportType reptype);


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

        ///// <summary>Returns the System.Diagnostics.EventLogEntryType value corresponding to the given ReportType.
        ///// Remark: FailureAudit and SuccessAudit can not be generated because they don't have representation in ReportType.</summary>
        ///// <param name="level">ReportType value to be converted.</param>
        ///// <returns>Converted value of type EventLogEntryType.</returns>
        //EventLogEntryType ReportType2EventLogEntryType(ReportType rt);

        ///// <summary>Returns the ReportType value corresponding to the given System.Diagnostics.EventLogEntryType.
        ///// Remark: FailureAudit and SuccessAudit do not have representation in ReportType and are mapped to Error and Warning, respectively.</summary>
        ///// <param name="level">EventLogEntryType value to be converted.</param>
        ///// <returns>Converted value of type ReportType.</returns>
        //ReportType EventLogEntryType2ReportType(EventLogEntryType el);

        //// Properties of the type TraceSwitch that are synchronized with the levels of reporting of different types
        //// (i.e. reporting, logging and tracing). These properties are not in use if not explicitly accessed, therefore
        //// they just contribute to the Reporter's API for those users used to deal with the TraceSwitch class.
        //// It is not yet clear whether this feature will be kept in the futre.

        ///// <summary>Returns the System.Diagnostics.TraceLevel value corresponding to the given ReportLevel.</summary>
        ///// <param name="level">ReportLevel value to be converted.</param>
        ///// <returns>Converted value of type TraceLevel.</returns>
        //TraceLevel ReportLevel2TraceLevel(ReportLevel level);

        ///// <summary>Returns the ReportLevel value corresponding to the given System.Diagnostics.TraceLevel.</summary>
        ///// <param name="level">TraceLevel value to be converted.</param>
        ///// <returns>Converted value of type ReportLevel.</returns>
        //ReportLevel TraceLevel2ReportLevel(TraceLevel tl);


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
         /// It is left entirely to error reporting delegates to interpret the object's contents.</summary>
        object Obj { get; set; } 
 
        /// <summary>Object used for locking.</summary>
        object lockobj { get; }

#endregion  // Reporter_Data


#region Reporting
        // ACTUAL REPORTING METHDS; Specific methods are not included in this interface.

#region Reporting_General
        // GENERAL REPORTING METHODS (for all kinds of reports):

        /// <summary>Basic reporting method (overloaded). Launches an error report, a warning report or other kind of report/message.
        /// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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
        /// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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
        /// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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

#endregion  // Reporting_General

        // IReporterBase does not include specific reporting utilities.

#endregion // Reporting

    }   // interface IReporter


#endregion  // Interfaces





    /// <summary>Base IGlib class for reporting, tracing and logging; provides a global reporter and a basis for creation 
    /// of local reporters.
    /// This class is identical to the IGLib class (copied directly). IN EFA, refer to the derived class Reporter!</summary>
    public class ReporterBase : IReporter
    // $A Igor Oct08;
    {

#region Error_Reporting_Definition

        // Replaceable methods implemented through delagates:

        /// <summary>Delegate that performs error reporting.
        /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
        /// assemble error message. Then it uses both to assemble the final decorated error message and launches
        /// it in its own way.</summary>
        public ReportDelegate ReportDlg = null;  // new ReportDelegate(DefaultReportConsole);

        /// <summary>Delegate that assembles the error location string.</summary>
        public ReportLocationDelegate ReportLocationDlg = null;  // new ReportLocationDelegate(DefaultLocationString);

        /// <summary>Delegate that assembles the eror message string.</summary>
        public ReportMessageDelegate ReportMessageDlg = null;  //  = new ReportMessageDelegate(DefaultMessageString);

        /// <summary>Delegate that reports an error when the error reporting function throws an exception.
        /// Functions assigned to this delegate should really be BULLETPROOF. If not assigned then the internal
        /// method of the corresponding class is called.
        /// It is highly recommended to do reporting in small steps enclosed in try/catch blocks and especially to
        /// use the error reporting object very carefully (because it may not be initialized properly, which can
        /// also be the reason that the error reporting function crashes).
        /// It is higly recommended to first call the DefaultReserveReportError() within the function assigned to this delegate,
        /// or at least to use the DefaultReserveReportMessage() method for assembly of the message shown.</summary>
        public ReserveReportErrorDelegate ReserveReportErrorDlg = null; // new ReserveReportErrorDelegate(DefaultReserveReportError);

#region Initialization

#region Constructors

        // Constructors - this region will be literally included in derived classes, except in special cases):

        /// <summary>Constructor. Initializes all error reporting delegates to default values and sets auxliary object to null.
        /// Auxiliary object Obj is set to null.</summary>
        public ReporterBase()
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
        public ReporterBase(object obj,
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
        public ReporterBase(object obj,
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
        public ReporterBase(object obj, ReportDelegate reportdelegate)
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
        public ReporterBase(object obj,
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
        protected virtual void SetDefaultReportDlg()
        // $A Igor Oct08;
        {
            ReportDlg = null; // By default, there is no reporting through delegates. // new ReportDelegate(DefaultReportConsole);  
        }

        /// <summary>Sets the error location assembling delegate to the default value.
        /// This default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected virtual void SetDefaultReportLocationDlg()
        // $A Igor Oct08;
        {
            ReportLocationDlg = new ReportLocationDelegate(DefaultLocationString);
        }

        /// <summary>Sets the error message assembling delegate to the default value.
        /// This default delegate does not utilize the auxiliary object (and can be mixed with the delegates that do.)</summary>
        protected virtual void SetDefaultReportMessageDlg()
        // $A Igor Oct08;
        {
            ReportMessageDlg = new ReportMessageDelegate(DefaultMessageString);
        }

        /// <summary>Sets the reserve error reporting delegate to the default value.</summary>
        protected virtual void SetDefaultReserveReportErrorDlg()
        // $A Igor Oct08;
        {
            ReserveReportErrorDlg = null; // new ReserveReportErrorDelegate(DefaultReserveReportError);
        }


        /// <summary>Initial part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected virtual void InitBegin()
        // $A Igor Oct08;
        {
            try
            {
                Obj = null;
                ReportDlg = null;
                ReportLocationDlg = null;
                ReportMessageDlg = null;
                ReserveReportErrorDlg = null;
            }
            catch { }
        }

        /// <summary>Finalizing part of initialization.
        /// Auxiliary object is not affected because default delegates do not utilize it.</summary>
        protected virtual void InitEnd()
        // $A Igor Oct08;
        {
            try
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
            catch { }
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
                this.ReportMessageDlg = null; // messagedelegate;
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


#region Finalization


         ~ReporterBase()
         {
           Dispose(false);
         }
       
         private bool isDisposed = false, dispLocked = false;
       
          public void Dispose() 
          {
             Dispose(true);
             GC.SuppressFinalize(this); 
          }
          protected virtual void Dispose(bool disposing)
          {
              if (!dispLocked)
              {
                  dispLocked = true;
                  if (!isDisposed)
                  {
                      if (disposing)
                      {
                          // Code to dispose the managed resources 
                          // held by the class
                      }
                  }
                  // Code to dispose the unmanaged resources 
                  // held by the class

                  try
                  {
                      try
                      {
                          TextWriterFlush();
                      }
                      catch { }
                      try 
                      {
                          RemoveTextWriters();
                      }
                      catch { }
                      try
                      {
                          TextLoggerFlush();
                      }
                      catch { }
                      try 
                      {
                          RemoveTextLoggers();
                      }
                      catch { }
                  }
                  finally
                  {
                  }


                  isDisposed = true;
                  dispLocked = false;
              }
          }





       
#endregion  // Finalization


#region Erorr_Reporting_Global

        // Global error reporter of this class:
        // In derived classes, this block should be repeated, only with classs name of _Global and Global set to the
        // current (derived) class and with keyword new in property declarations.

        private static ReporterBase _Global = null; 
        private static bool _GlobalInitialized = false;
        private static object GlobalLock = new object();

        /// <summary>Gets the value indicating whether the global reporter of this class has already been initialized.</summary>
        public static bool GlobalInitialized
        {
            get { return _GlobalInitialized; }
        }

        /// <summary>Indicates whether a global Reporter (or its derived class) should read settings from the
        /// application configuration file when initialized (default is true).</summary>
        public static bool ReadGlobalAppSettings = true;

        /// <summary>Gets the global reporter object.
        /// This is typically used for configuring the global reporter.</summary>
        public static ReporterBase Global
        // $A Igor Oct08;
        {
            get
            {
                    if (_Global == null)
                    {
                        lock (GlobalLock)  // we need a lock only in initialization; otherwise global reporter is immutable
                        {
                            if (_Global== null && !_GlobalInitialized)
                            {
                                _Global = new ReporterBase();
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

        /// <summary>Takes an existing exception and returns a modified exception based on it.</summary>
        /// <param name="ex">Exception whose revised version is created.</param>
        /// <param name="messageaddition">String that is prepended to the old message (when <paramref name="keepmessage"/> is true)
        /// or forms the new message of the exception (when <paramref name="keepmessage"/> is false)</param>
        /// <param name="newtype">The type of the new exception.
        /// Applies when <paramref name="sametype"/> is false and the specified type is valid, i.e. represents Exception or a derived type.
        /// Also applies when <paramref name="ex"/> is null and the specified type is valid.</param>
        /// <param name="sametype">If true then the an exception of the same type as the original one is created.</param>
        /// <param name="keepmessage">If true then the original message is contained in the revised exception
        /// (it is simply appended to <paramref name="messageaddition"/>)</param>
        /// <param name="oldasinner">If true then the original exception is included as inner exception.</param>
        /// <returns>The revised exception (the original exception modified according to parameters).</returns>
        public static Exception ReviseException(Exception ex, string messageaddition, Type newtype,
                bool sametype, bool keepmessage, bool oldasinner)
        {
            Exception ret;
            Type extype = typeof(Exception);  // Default type of the revised exception is simply Exception
            if (sametype)
            {
                // We will create exception of the same type as the original one:
                if (ex != null)
                    extype = ex.GetType();
                else if (newtype!=null)  // however, if exception is not specified, take the specified type if valid
                    if (newtype.IsSubclassOf(typeof(Exception)))
                        extype = newtype;
            } else
            {
                // Type of the revised exception is not the same as that of the old one; set the type to the specified one if valid:
                if (newtype != null)
                    if (newtype.IsSubclassOf(typeof(Exception)))
                        extype = newtype;
            }
            string message, oldmessage = (ex==null?"":ex.Message);
            if (messageaddition == null)
                messageaddition = "";
            if (keepmessage)
                message = messageaddition + oldmessage;
            else
                message = messageaddition;
            object[] args;
            if (oldasinner)
            {
                args = new object[2];
                args[0] = message;
                args[1] = ex;
            } else
            {
                args = new object[1];
                args[0] = message;
            }
            // ReturnedString = Activator.CreateInstance(extype,AppArguments) as Exception;  // $$ Verjetno Nejčeva koda, to je treba preveriti 
            ret = (Exception)Activator.CreateInstance(extype,args);   // Verjetno moja originalna koda - preveriti, kaj ostane not.    
   
            return (ret);
        }

        // Specified old exception and message addition (or replacement):

        /// <summary>Takes an existing exception and returns a modified exception based on it.
        /// The returned exception is of the same type as the original one.
        /// Original message is appended to <paramref name="messageaddition"/>.
        /// The created exception does not have an inner exception.</summary>
        /// <param name="ex">Exception whose revised version is created.</param>
        /// <param name="messageaddition">String that is prepended to the old message.</param>
        public static Exception ReviseException(Exception ex, string messageaddition)
        {
            return ReviseException(
                ex, 
                messageaddition, 
                null   /* newtype */ ,
                true   /* sametype */ , 
                true   /* keepmessage */ , 
                false   /* oldasinner */   );
        }

        /// <summary>Takes an existing exception and returns a modified exception based on it.
        /// The returned exception is of the same type as the original one.
        /// The created exception does not have an inner exception.</summary>
        /// <param name="ex">Exception whose revised version is created.</param>
        /// <param name="messageaddition">String that is prepended to the old message (when <paramref name="keepmessage"/> is true)
        /// or forms the new message of the exception (when <paramref name="keepmessage"/> is false)</param>
        /// <param name="keepmessage">If false then the original message is not kept.</param>
        public static Exception ReviseException(Exception ex, string messageaddition, bool keepmessage)
        {
            if (string.IsNullOrEmpty(messageaddition) && !keepmessage)
                keepmessage = true;
            return ReviseException(
                ex, 
                messageaddition, 
                null   /* newtype */ ,
                true   /* sametype */ ,
                keepmessage   /* keepmessage */ ,
                false   /* oldasinner */   );
        }


        /// <summary>Takes an existing exception and returns a modified exception based on it.
        /// The returned exception is of the same type as the original one.</summary>
        /// <param name="ex">Exception whose revised version is created.</param>
        /// <param name="messageaddition">String that is prepended to the old message (when <paramref name="keepmessage"/> is true)
        /// or forms the new message of the exception (when <paramref name="keepmessage"/> is false)</param>
        /// <param name="keepmessage">If false then the original message is not kept.</param>
        /// <param name="oldasinner">If true then the original original exception becomes the inner exception of the returned one.</param>
        public static Exception ReviseException(Exception ex, string messageaddition, bool keepmessage, bool oldasinner)
        {
            if (string.IsNullOrEmpty(messageaddition) && !keepmessage)
                keepmessage = true;
            return ReviseException(
                ex, 
                messageaddition, 
                null   /* newtype */ ,
                true   /* sametype */ ,
                keepmessage   /* keepmessage */ ,
                oldasinner   /* oldasinner */   );
        }



        // Specified old exception, message addition (or replacement), and type:

        /// <summary>Takes an existing exception and returns a modified exception based on it.
        /// Original message is appended to <paramref name="messageaddition"/>.
        /// The created exception does not have an inner exception.</summary>
        /// <param name="ex">Exception whose revised version is created.</param>
        /// <param name="messageaddition">String that is prepended to the old message
        /// or forms the new message of the exception.</param>
        /// <param name="newtype">Type of the returned exception. If the parameter is null then the type will be Exception.</param>
        public static Exception ReviseException(Exception ex, string messageaddition, Type newtype)
        {
            return ReviseException(
                ex, 
                messageaddition,
                newtype   /* newtype */ ,
                false   /* sametype */ , 
                true   /* keepmessage */ , 
                false   /* oldasinner */   );
        }

        /// <summary>Takes an existing exception and returns a modified exception based on it.
        /// The created exception does not have an inner exception.</summary>
        /// <param name="ex">Exception whose revised version is created.</param>
        /// <param name="messageaddition">String that is prepended to the old message (when <paramref name="keepmessage"/> is true)
        /// or forms the new message of the exception (when <paramref name="keepmessage"/> is false)</param>
        /// <param name="newtype">Type of the returned exception. If the parameter is null then the type will be Exception.</param>
        /// <param name="keepmessage">If false then the original message is not kept.</param>
        public static Exception ReviseException(Exception ex, string messageaddition, Type newtype, bool keepmessage)
        {
            if (string.IsNullOrEmpty(messageaddition) && !keepmessage)
                keepmessage = true;
            return ReviseException(
                ex, 
                messageaddition, 
                newtype,
                false   /* sametype */ ,
                keepmessage   /* keepmessage */ ,
                false   /* oldasinner */   );
        }

        /// <summary>Takes an existing exception and returns a modified exception based on it.</summary>
        /// <param name="ex">Exception whose revised version is created.</param>
        /// <param name="messageaddition">String that is prepended to the old message (when <paramref name="keepmessage"/> is true)
        /// or forms the new message of the exception (when <paramref name="keepmessage"/> is false)</param>
        /// <param name="newtype">Type of the returned exception. If the parameter is null then the type will be Exception.</param>
        /// <param name="keepmessage">If false then the original message is not kept.</param>
        /// <param name="oldasinner">If true then the original original exception becomes the inner exception of the returned one.</param>
        public static Exception ReviseException(Exception ex, string messageaddition, Type newtype, bool keepmessage, bool oldasinner)
        {
            if (string.IsNullOrEmpty(messageaddition) && !keepmessage)
                keepmessage = true;
            return ReviseException(
                ex, 
                messageaddition, 
                newtype,
                false   /* sametype */ ,
                keepmessage   /* keepmessage */ ,
                oldasinner   /* oldasinner */   );
        }


#endregion Erorr_Reporting_Global



#region Reporter_ReadConfiguration
        // Reading of reporter settings from configuration files

#region Reporter_ReadConfiguration_General


        // Reading values of different types from the application configuration files:

        /// <summary>Reads a string value from the application configuration file (e.g. app.config) and assigns it to the specified variable.</summary>
        /// <param name="key">Key in the application configuration file.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned.</param>
        /// <param name="assigned">Output parameter that indicates whether the value has been assigned or not.</param>
        public static void GetAppSetting(string key, ref string value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            string str = GetAppSettingsValue(key);
            if (!string.IsNullOrEmpty(str))
                assigned = true;
        }

        /// <summary>Reads a character value from the application configuration file and assigns it to the specified variable.</summary>
        /// <param name="key">Key in the application configuration file.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned.</param>
        /// <param name="assigned">Output parameter that indicates whether the value has been assigned or not.</param>
        /// <exception cref="T:System.FormatException">Thrown when value is not in the right format.</exception>
        public static void GetAppSetting(string key, ref char value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            // Read the string value corresponding to the key (if defined):
            string str = GetAppSettingsValue(key);
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length != 1)
                    throw new FormatException("A single character should be provided.");
                value = str[0];
                assigned = true;
            }
        }


        /// <summary>Reads a list of characters from the application configuration file and assigns it to the specified variable.</summary>
        /// <param name="key">Key in the application configuration file.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned.</param>
        /// <param name="assigned">Output parameter that indicates whether the value has been assigned or not.</param>
        /// <exception cref="T:System.FormatException">Thrown when value is not in the right format.</exception>
        public static void GetAppSetting(string key, ref List<char> value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            // Read the string value corresponding to the key (if defined):
            string str = GetAppSettingsValue(key);
            if (!string.IsNullOrEmpty(str))
            {
                if (value == null)
                    value = new List<char>();
                value.Clear();
                for (int i = 0; i < str.Length; ++i)
                {
                    value.Add(str[i]);
                }
                assigned = true;
            }
        }


        /// <summary>Reads an integer value from the application configuration file and assigns it to the specified variable.</summary>
        /// <param name="key">Key in the application configuration file.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned.</param>
        /// <param name="assigned">Output parameter that indicates whether the value has been assigned or not.</param>
        /// <exception cref="T:System.FormatException">Thrown when value is not in the right format.</exception>
        /// <exception cref="T:System.OverFlowException"></exception>
        public static void GetAppSetting(string key, ref int value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            // Read the string value corresponding to the key (if defined):
            string str = GetAppSettingsValue(key);
            if (!string.IsNullOrEmpty(str))
            {
                // The key is defined in the settings file, convert it to the appropriate type:
                value = int.Parse(str); // this will throw exception if the format is wrong
                assigned = true;
            }
        }


        /// <summary>Reads a floating point value from the application configuration file and assigns it to the specified variable.</summary>
        /// <param name="key">Key in the application configuration file.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned.</param>
        /// <param name="assigned">Output parameter that indicates whether the value has been assigned or not.</param>
        /// <exception cref="T:System.FormatException">Thrown when value is not in the right format.</exception>
        /// <exception cref="T:System.OverFlowException"></exception>
        public static void GetAppSetting(string key, ref double value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
                // Read the string value corresponding to the key (if defined):
                string str = GetAppSettingsValue(key);
                if (!string.IsNullOrEmpty(str))
                {
                    // The key is defined in the settings file, convert it to the appropriate type:
                    value = double.Parse(str); // this will throw exception if the format is wrong
                    assigned = true;
                }
        }


        /// <summary>Reads a boolean value from the application configuration file and assigns it to the specified variable. 
        /// Numerical representation is allowed (0 for false, non-zero for true)</summary>
        /// <param name="key">Key in the application configuration file.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned.</param>
        /// <param name="assigned">Output parameter that indicates whether the value has been assigned or not.</param>
        /// <exception cref="T:System.FormatException">Thrown when value is not in the right format.</exception>
        public static void GetAppSetting(string key, ref bool value, out bool assigned)
        // $A Igor Feb09;
        {
            GetAppSetting(key, ref value, out assigned, true /* otherformatsallowed */);
        }


        /// <summary>Reads a boolean value from the application configuration file and assigns it to the specified variable.</summary>
        /// <param name="key">Key in the application configuration file.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned.</param>
        /// <param name="assigned">Output parameter that indicates whether the value has been assigned or not.</param>
        /// <param name="otherformatsallowed">If true then boolean type can be specified as number - 0 for false and non-zero for true.</param>
        /// <exception cref="T:System.FormatException">Thrown when value is not in the right format.</exception>
        public static void GetAppSetting(string key, ref bool value, out bool assigned, bool otherformatsallowed)
        // $A Igor Feb09;
        {
            assigned = false;
            // Read the string value corresponding to the key (if defined):
            string str = GetAppSettingsValue(key);
            if (!string.IsNullOrEmpty(str))
            {
                str = str.ToLower();
                // The key is defined in the settings file, convert it to boolean:
                if (otherformatsallowed)
                {
                    try
                    {
                        value = bool.Parse(str);
                        assigned = true;
                    }
                    catch { }
                    if (!assigned)
                    {
                        if (str == "y")
                            value = true;
                        else if (str == "n")
                            value = false;
                        else if (str == "yes")
                            value = true;
                        else if (str == "no")
                            value = false;
                        else if (str == "on")
                            value = true;
                        else if (str == "off")
                            value = false;
                        else
                        {
                            // Direct conversion failed, 
                            double numval = double.Parse(str);  // this will throw exception if the format is wrong
                            if (numval == 0.0)
                                value = false;
                            else
                                value = true;
                        }
                        assigned = true;
                    }
                } else
                {
                    value = bool.Parse(str); // this will throw exception if the format is wrong
                    assigned = true;
                }
            }
        }


        /// <summary>Returns configuration value corresponding to the configuration key in AppSettings (specified by 
        /// <paramref name="key"/>)</summary>
        /// <param name="key">The key for which the value is retrieved from AppSettings.</param>
        /// <exception cref="FrameworkDependencyException">Thrown when code is not compiled against the .NET full framework.</exception>
        public static string GetAppSettingsValue(string key)
        {
#if NETFRAMEWORK
            return ConfigurationManager.AppSettings.Get(key);
#else
            throw new FrameworkDependencyException($"Coulld not read the value for key {key} from the ConfigurationManager.");
#endif

        }


        /// <summary>Reads an integer value from the application configuration file and assigns it to the specified variable.</summary>
        /// <param name="key">Key in the application configuration file.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned.</param>
        /// <param name="assigned">Output parameter that indicates whether the value has been assigned or not.</param>
        public static void GetAppSetting(string key, ref ReportLevel value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            // Read the string value corresponding to the key (if defined):
            string str = GetAppSettingsValue(key);
            if (!string.IsNullOrEmpty(str))
            {
                // The key is defined in the settings file, convert it to the appropriate type:
                if (!string.IsNullOrEmpty(str))
                {
                    string strlowercase = str.ToLower();
                    if (strlowercase == "off")
                        value = ReportLevel.Off;
                    else if (strlowercase == "error")
                        value = ReportLevel.Error;
                    else if (strlowercase == "warning")
                        value = ReportLevel.Warning;
                    else if (strlowercase == "info")
                        value = ReportLevel.Info;
                    else if (strlowercase == "verbose")
                        value = ReportLevel.Verbose;
                    else
                        throw new InvalidCastException("Can not convert the string \"" + str + "\" to the ReporterLevel enumeration.");
                    assigned = true;
                }
            }
        }


        // Formation of keys for the configuration files:

        protected const string KeyPrefix = "ReporterMsg";

        /// <summary>Returns a full keyroot of a specific configuration item with a reporter name specified.</summary>
        /// <param name="reportername">User - defined name of a reporter where settings are set. 
        /// If null then the key will apply to all reporters.</param>
        /// <param name="keyroot">Reduced keyroot name (without decorations).</param>
        /// <returns>The composed as it appears in the application configuration file.</returns>
        protected static string FullKey(string reportername, string keyroot)
        // $A Igor feb09;
        {
            if (string.IsNullOrEmpty(keyroot))
                throw new ArgumentNullException(keyroot, reportername);
            if (string.IsNullOrEmpty(reportername))
                return KeyPrefix + "_" + keyroot;
            else
                return KeyPrefix + "_" + reportername + "_" + keyroot;
        }

        // Reading of values adapted for reporter classes:

        /// <summary>Reads a string value from the application configuration file (e.g. app.config) and assigns it to the specified parameter
        /// of the reporter.</summary>
        /// <param name="reportername">User - defined name of a reporter where settings are set. 
        /// If null then the key will apply to all reporters.</param>
        /// <param name="keyroot">Agreed name of the parameter to be assigned.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned in the case that it is defined.
        /// If the given key is not defined in the configuration file then the variable is not affected.</param>
        /// <param name="assigned">Output parameter, tells whether the value has been assigned.</param>
        protected void GetAppSetting(string reportername, string keyroot, ref string value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            try
            {
                GetAppSetting(FullKey(reportername, keyroot), ref value, out assigned);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, null, 
                    "Wrong format of reporter configuration parameter \"" + keyroot + "\" (string).", null, ex);
            }
        }

        /// <summary>Reads a character value from the application configuration file (e.g. app.config) and assigns it to the specified parameter
        /// of the reporter.</summary>
        /// <param name="reportername">User - defined name of a reporter where settings are set. 
        /// If null then the key will apply to all reporters.</param>
        /// <param name="keyroot">Agreed name of the parameter to be assigned.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned in the case that it is defined.
        /// If the given key is not defined in the configuration file then the variable is not affected.</param>
        /// <param name="assigned">Output parameter, tells whether the value has been assigned.</param>
        protected void GetAppSetting(string reportername, string keyroot, ref char value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            try
            {
                GetAppSetting(FullKey(reportername, keyroot), ref value, out assigned);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, null,
                    "Wrong format of reporter configuration parameter \"" + keyroot + "\" (list of characters).", null, ex);
            }
        }

        /// <summary>Reads a list of characters from the application configuration file (e.g. app.config) and assigns it to the specified parameter
        /// of the reporter.</summary>
        /// <param name="reportername">User - defined name of a reporter where settings are set. 
        /// If null then the key will apply to all reporters.</param>
        /// <param name="keyroot">Agreed name of the parameter to be assigned.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned in the case that it is defined.
        /// If the given key is not defined in the configuration file then the variable is not affected.</param>
        /// <param name="assigned">Output parameter, tells whether the value has been assigned.</param>
        protected void GetAppSetting(string reportername, string keyroot, ref List<char> value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            try
            {
                GetAppSetting(FullKey(reportername, keyroot), ref value, out assigned);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, null,
                    "Wrong format of reporter configuration parameter \"" + keyroot + "\" (characters).", null, ex);
            }
        }

        /// <summary>Reads an integer value from the application configuration file (e.g. app.config) and assigns it to the specified parameter
        /// of the reporter.</summary>
        /// <param name="reportername">User - defined name of a reporter where settings are set. 
        /// If null then the key will apply to all reporters.</param>
        /// <param name="keyroot">Agreed name of the parameter to be assigned.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned in the case that it is defined.
        /// If the given key is not defined in the configuration file then the variable is not affected.</param>
        /// <param name="assigned">Output parameter, tells whether the value has been assigned.</param>
        protected void GetAppSetting(string reportername, string keyroot, ref int value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            try
            {
                GetAppSetting(FullKey(reportername, keyroot), ref value, out assigned);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, null,
                    "Wrong format of reporter configuration parameter \"" + keyroot + "\" (integer).", null, ex);
            }
        }

        /// <summary>Reads a numeric value from the application configuration file (e.g. app.config) and assigns it to the specified parameter
        /// of the reporter.</summary>
        /// <param name="reportername">User - defined name of a reporter where settings are set. 
        /// If null then the key will apply to all reporters.</param>
        /// <param name="keyroot">Agreed name of the parameter to be assigned.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned in the case that it is defined.
        /// If the given key is not defined in the configuration file then the variable is not affected.</param>
        /// <param name="assigned">Output parameter, tells whether the value has been assigned.</param>
        protected void GetAppSetting(string reportername, string keyroot, ref double value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            try
            {
                GetAppSetting(FullKey(reportername, keyroot), ref value, out assigned);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, null,
                    "Wrong format of reporter configuration parameter \"" + keyroot + "\" (numeric).", null, ex);
            }
        }

        /// <summary>Reads a boolean value from the application configuration file (e.g. app.config) and assigns it to the specified parameter
        /// of the reporter.</summary>
        /// <param name="reportername">User - defined name of a reporter where settings are set. 
        /// If null then the key will apply to all reporters.</param>
        /// <param name="keyroot">Agreed name of the parameter to be assigned.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned in the case that it is defined.
        /// If the given key is not defined in the configuration file then the variable is not affected.</param>
        /// <param name="assigned">Output parameter, tells whether the value has been assigned.</param>
        protected void GetAppSetting(string reportername, string keyroot, ref bool value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            try
            {
                GetAppSetting(FullKey(reportername, keyroot), ref value, out assigned);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, null,
                    "Wrong format of reporter configuration parameter \"" + keyroot + "\" (boolean).", null, ex);
            }
        }

        /// <summary>Reads a ReportLevel enumeration value from the application configuration file (e.g. app.config) and assigns it to the specified parameter
        /// of the reporter.</summary>
        /// <param name="reportername">User - defined name of a reporter where settings are set. 
        /// If null then the key will apply to all reporters.</param>
        /// <param name="keyroot">Agreed name of the parameter to be assigned.</param>
        /// <param name="value">Reference to the variable to which the read-in value is assigned in the case that it is defined.
        /// If the given key is not defined in the configuration file then the variable is not affected.</param>
        /// <param name="assigned">Output parameter, tells whether the value has been assigned.</param>
        protected void GetAppSetting(string reportername, string keyroot, ref ReportLevel value, out bool assigned)
        // $A Igor Feb09;
        {
            assigned = false;
            try
            {
                GetAppSetting(FullKey(reportername, keyroot), ref value, out assigned);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, null,
                    "Wrong format of reporter configuration parameter \"" + keyroot + "\" (ReportLevel enumeration).", null, ex);
            }
        }


        // Maintaining a list of setting groups that ahve already be read for the specified textwriter:


        private List<String> readconf = null;

        /// <summary>Returns a flag that tells whether configuration settings with a given group name have already
        /// been read for this reporter.</summary>
        /// <param name="groupname">Name of the settings group, null and "" are treated teh same (denoting the genraal group).</param>
        /// <returns>true if settings with the specified name have already been read for teh current reporter, false otherwise.</returns>
        public bool AppSettingsRead(string groupname)
        {
            bool ret = false;
            if (groupname == null)
                groupname = "";
            lock (lockobj)
            {
                try
                {
                    if (readconf != null)
                    {
                        ret = readconf.Contains(groupname);
                    }
                }
                catch (Exception ex)
                {
                    ReserveReportError(ReportType.Error, null, "Can not check whether a group of settings named \"" + groupname +
                        "\" has been read.", null, ex);
                }
            }
            return ret;
        }

        
        /// <summary>Returns a flag that tells whether general configuration settings (not belonging to any group)
        /// have already been read for this reporter.</summary>
        /// <returns>true if settings with the specified name have already been read for teh current reporter, false otherwise.</returns>
        public bool AppSettingsRead()
        {
            return AppSettingsRead("");
        }

        private bool _AppSettingsWarnings = true;

        /// <summary>Gets or sets the flag that specifies whether a warning message is launched when reading of application
        /// settings is attempted more than once for the same named group of settings.</summary>
        public bool AppSettingsWarnings
        {
            get { return _AppSettingsWarnings; }
            set { _AppSettingsWarnings = value; }
        }

        /// <summary>Launches an internal warning that the configuration settings belonging tot he specified group
        /// have already been read for this reporter.
        /// The warning is launched only if the settings have actually been read and if warnings of this type are
        /// switched on.</summary>
        /// <param name="groupname">Name of the settings group.</param>
        protected void AppSettingsReadWarning(string groupname)
        {
            if (AppSettingsRead(groupname) && AppSettingsWarnings)
            {
                string message;
                if (string.IsNullOrEmpty(groupname))
                    message = "Settings for the general group have already been read for the current reporter.";
                else
                    message = "Settings belonging to the group named \"" + groupname + 
                        "\" have already been read for the current reporter.";
                ReserveReportError(ReportType.Warning, "Reporter configuration settings", message, null, null);
            }
        }

        /// <summary>Marks a specified group of settings as been read for the current reporter.</summary>
        /// <param name="groupname">Name of the settings group, null and "" are treated teh same (denoting the genraal group).</param>
        protected void MarkAppSettingsRead(string groupname)
        {
            if (groupname == null)
                groupname = "";
            lock (lockobj)
            {
                try
                {
                    if (!AppSettingsRead(groupname))
                    {
                        if (readconf == null)
                            readconf = new List<string>();
                        readconf.Add(groupname);
                    }
                }
                catch (Exception ex)
                {
                    ReserveReportError(ReportType.Error, null, "Can not mark the group of settings named \"" + groupname +
                        "\" as read.", null, ex);
                };
            }
        }



#endregion Reporter_ReadConfiguration_General

        // Key roots for various settings that are recognized:
        protected const string
            KeyLevelOn = "LevelOn", // if set then it specifies the minimal level for reporting, logging and tracing altogether
            KeyLevelOff = "LevelOff", // if set then it specifies that the maximal level for reporting, logging and tracing altogether is below the specified level
            KeyReportingLevel = "ReportingLevel", // level of reporting
            KeyLoggingLevel = "LoggingLevel",  // level of logging
            KeyTracingLevel = "TracingLevel",  // level of tracing
            
            KeyUseTextWriter = "UseTextWriter", // specifies whether or not the TextWriter is used in reporting
            KeyUseTextLogger = "UseTextLogger", // specifies whether or not the TextLogger is used in reporting
            KeyUseTrace = "UseTrace",  // specifies whether or not Trace is used in reporting

            KeyTextWriterAppend = "TextWriterAppend", // specifies whether append mode is used for text writer files open via 
            // configuration settings (this is not valid for files installed by function calls like AddTextWriter())
            KeyTextWriterFile = "TextWriterFile",  // specifies the output file that is installed on the text writer when
            // configuration is read
            KeyTextWriterFile1 = "TextWriterFile1",  // specifies an output file that is added to the text writer when
            // configuration is read
            KeyTextWriterWriteIntro = "TextWriterWriteIntro",  // specifies whether introductory text is written to text writers when they are set
            KeyTextWriterProgramName = "TextWriterProgramName",  // specifies the program name used in text writers' introductory text
            KeyTextWriterIntroText = "TextWriterIntroText", // specifies textwriters' introductory text as a whole

            KeyTextLoggerAppend = "TextLoggerAppend", // specifies whether append mode is used for text logger files open via 
            // configuration settings (this is not valid for files installed by function calls like AddTextLogger())
            KeyTextLoggerFile = "TextLoggerFile",  // specifies the output file that is installed on the text logger when
            // configuration is read
            KeyTextLoggerFile1 = "TextLoggerFile1",  // specifies an output file that is added to the text logger when
            // configuration is read
            KeyTextLoggerWriteIntro = "TextLoggerWriteIntro",  // specifies whether introductory text is written to text loggers when they are set
            KeyTextLoggerProgramName = "TextLoggerProgramName",  // specifies the program name used in text loggers' introductory text
            KeyTextLoggerIntroText = "TextLoggerIntroText", // specifies text loggers' introductory text as a whole
            KeyTextLoggerIndentInitial = "TextLoggerIndentInitial", // specifies text loggers' initial (zero depth) indentation
            KeyTextLoggerIndentSpacing = "TextLoggerIndentSpacing", // specifies text loggers' number of characters used in indentation
            KeyTextLoggerIndentCharacter = "TextLoggerIndentCharacter", // specifies text loggers' indentation character

            KeyAppSettingsWarnings = "AppSettingsWarnings"; // specifies whether warnings are launched when attempting to read
                // the same group of settings several times for a given reporter


        // Auxiliary settings for opening TextLogger or TextWriter files form via application settings file:
        private bool TextWriterAppend = false;
        private bool TextLoggerAppend = false;

        /// <summary>Reads settings for a specified named group of reporters from the application configuration file.
        /// In the derived classes, this method should be overridden by the method that calls the base class' method.</summary>
        /// <param name="groupname">Name of the group of reporters for which the settings apply.</param>
        /// <remarks>Locking and try blocks are not needed in this method because they appear in the calling methods.</remarks>
        protected virtual void ReadAppSettingsBasic(string groupname)
        // $A Igor Feb09;
        {
            bool assigned, boolval = false;
            ReportLevel level = ReportLevel.Warning;
            int intval = 0;
            string stringval = null;
            char charval = ' ';

            GetAppSetting(groupname, KeyLevelOn, ref level, out assigned);
            if (assigned) this[level] = true;
            GetAppSetting(groupname, KeyLevelOff, ref level, out assigned);
            if (assigned) this[level] = false;
            GetAppSetting(groupname, KeyReportingLevel, ref level, out assigned);
            if (assigned) ReportingLevel = level;
            GetAppSetting(groupname, KeyLoggingLevel, ref level, out assigned);
            if (assigned) LoggingLevel = level;
            GetAppSetting(groupname, KeyTracingLevel, ref level, out assigned);
            if (assigned) TracingLevel = level;

            GetAppSetting(groupname, KeyUseTextWriter, ref boolval, out assigned);
            if (assigned) UseTextWriter = boolval;
            GetAppSetting(groupname, KeyUseTextLogger, ref boolval, out assigned);
            if (assigned) UseTextLogger = boolval;
            GetAppSetting(groupname, KeyUseTrace, ref boolval, out assigned);
            if (assigned) UseTrace = boolval;

            GetAppSetting(groupname, KeyTextWriterAppend, ref boolval, out assigned);
            if (assigned) TextWriterAppend = boolval;
            GetAppSetting(groupname, KeyTextWriterFile, ref stringval, out assigned);
            if (assigned) SetTextWriter(stringval, TextWriterAppend);
            GetAppSetting(groupname, KeyTextWriterFile1, ref stringval, out assigned);
            if (assigned && !string.IsNullOrEmpty(stringval)) AddTextWriter(stringval, TextWriterAppend) ;
            GetAppSetting(groupname, KeyTextWriterWriteIntro, ref boolval, out assigned);
            if (assigned) TextWriterWriteIntro = boolval;
            GetAppSetting(groupname, KeyTextWriterProgramName, ref stringval, out assigned);
            if (assigned) TextWriterProgramName = stringval;
            GetAppSetting(groupname, KeyTextWriterIntroText, ref stringval, out assigned);
            if (assigned) TextWriterIntroText = stringval;

            GetAppSetting(groupname, KeyTextLoggerAppend, ref boolval, out assigned);
            if (assigned) TextLoggerAppend = boolval;
            GetAppSetting(groupname, KeyTextLoggerFile, ref stringval, out assigned);
            if (assigned) SetTextLogger(stringval, TextLoggerAppend);
            GetAppSetting(groupname, KeyTextLoggerFile1, ref stringval, out assigned);
            if (assigned && !string.IsNullOrEmpty(stringval)) AddTextLogger(stringval, TextLoggerAppend);
            GetAppSetting(groupname, KeyTextLoggerWriteIntro, ref boolval, out assigned);
            if (assigned) TextLoggerWriteIntro = boolval;
            GetAppSetting(groupname, KeyTextLoggerProgramName, ref stringval, out assigned);
            if (assigned) TextLoggerProgramName = stringval;
            GetAppSetting(groupname, KeyTextLoggerIntroText, ref stringval, out assigned);
            if (assigned) TextLoggerIntroText = stringval;

            GetAppSetting(groupname, KeyTextLoggerIndentInitial, ref intval, out assigned);
            if (assigned) TextLoggerIndentInitial = intval;
            GetAppSetting(groupname, KeyTextLoggerIndentSpacing, ref intval, out assigned);
            if (assigned) TextLoggerIndentSpacing = intval;
            GetAppSetting(groupname, KeyTextLoggerIndentCharacter, ref charval, out assigned);
            if (assigned) TextLoggerIndentCharacter = charval;

            
            GetAppSetting(groupname, KeyAppSettingsWarnings, ref boolval, out assigned);
            if (assigned) AppSettingsWarnings = boolval;
        }


        /// <summary>Reads settings for a specified named group of reporters from the application configuration file.</summary>
        /// <param name="groupname">Name of the group of reporters for which the settings apply.</param>
        /// <param name="onlyonce">If true then settings belonging to the specified group are read only once.</param>
        public void ReadAppSettings(string groupname, bool onlyonce)
        // $A Igor Feb09;
        {
            lock (lockobj)
            {
                try
                {
                    if (onlyonce && AppSettingsRead(groupname))
                        AppSettingsReadWarning(groupname);
                    else
                    {
                        ReadAppSettingsBasic(groupname);
                        MarkAppSettingsRead(groupname);
                    }
                }
                catch (Exception ex)
                {
                    ReserveReportError(ReportType.Error, "Reading configuration", null, null, ex);
                }
            }
        }

        /// <summary>Reads settings for a specified named group of reporters from the application configuration file.</summary>
        /// <param name="groupname">Name of the group of reporters for which the settings apply.</param>
        // $A Igor Feb09;
        public void ReadAppSettings(string groupname)
        {
            ReadAppSettings(groupname, true);
        }


        /// <summary>Reads common reporter settings (i.e. settings that are not assigned for any named group) 
        /// from the application configuration file.</summary>
        public void ReadAppSettings()
        // $A Igor Feb09;
        {
            ReadAppSettings(null, true);
        }

#endregion // Reporter_ReadConfiguration


#region Reporter_Data


        /// <summary>Indicates that reporting suitable for debugging mode should be performed.
        /// A standard flag that can be used by the delegate functions.</summary>
        public bool DebugMode = false;


        private bool _IsGlobal = false;

        /// <summary>Indicates whether the current reporter is used as a global reporter or not.</summary>
        /// <remarks>This flag is set when the global reporter is initialized.</remarks>
        public bool IsGlobal
        {
            get { return _IsGlobal; }
            protected set { _IsGlobal = value; }
        }


        // Indentation for one-line reports:

        private int _Depth = 0, _IndentSpacing = 2, _InitialIndent=0;
        private char _IndentCharacter = ' ';


        private static int numErrIndentLevel = 0, maxErrIndentLevel = 3;

        /// <summary>Gets or sets the current indentation level for on-line output.
        /// This should normally be done by calling IncreaseDepth() or DecreaseDepth().</summary>
        public int Depth
        {
            get { return _Depth; }
            set
            {
                _Depth = value;
                if (_Depth < 0)
                {
                    ++numErrIndentLevel;
                    if (numErrIndentLevel <= maxErrIndentLevel)
                    {
                        string msg = "Indentation level is set to a negative value (" + _Depth.ToString() + "), re-set to 0.";
                        if (numErrIndentLevel == maxErrIndentLevel)
                            msg += Environment.NewLine + "Further messages of this king will be omitted.";
                        ReserveReportError(ReportType.Error, "Reporter.IndentLevel.set()", msg, null, null);
                    }
                    _Depth = 0;
                }
            }
        }

        private static int numErrInitialIndent = 0, maxErrInitialIndent = 3;


        /// <summary>Gets or sets number fo initial indentation charactyers.</summary>
        public int TextLoggerIndentInitial
        {
            get { return _InitialIndent; }
            set
            {
                _InitialIndent = value;
                if (_InitialIndent < 0)
                {
                    ++numErrInitialIndent;
                    if (numErrInitialIndent <= maxErrInitialIndent)
                    {
                        string msg = "Initial indentation is set to a negative value ("
                            + _InitialIndent.ToString() + "), re-set to 0.";
                        if (numErrInitialIndent == maxErrInitialIndent)
                            msg += Environment.NewLine + "Further messages of this king will be omitted.";
                        ReserveReportError(ReportType.Error, "Reporter.IndentChars.set()", msg, null, null);
                    }
                    _InitialIndent = 0;
                }
            }
        }

        private static int numErrIndentSpacing = 0, maxErrIndentSpacing =  3;


        /// <summary>Gets or sets the number of indentation characters written per indentation level.</summary>
        public int TextLoggerIndentSpacing
        {
            get { return _IndentSpacing; }
            set
            {
                _IndentSpacing = value;
                if (_IndentSpacing < 0)
                {
                    ++ numErrIndentSpacing;
                    if (numErrIndentSpacing <= maxErrIndentSpacing)
                    {
                        string msg = "Number of indentation characters per level is set to a negative value (" 
                            + _IndentSpacing.ToString() + "), re-set to 0.";
                        if (numErrIndentSpacing == maxErrIndentSpacing)
                            msg+= Environment.NewLine + "Further messages of this king will be omitted.";
                        ReserveReportError(ReportType.Error, "Reporter.IndentChars.set()", msg, null, null);
                    }
                    _IndentSpacing = 0;
                }
            }
        }

        
        // private static int numErrIndentCharacter = 0, maxErrIndentCharacter = 1;

        /// <summary>Gets or sets the indentation character.</summary>
        public char TextLoggerIndentCharacter
        {
            get { return _IndentCharacter; }
            set
            {
                _IndentCharacter = value;
            }
        }

        /// <summary>Increases indentation level by 1.</summary>
        /// <returns>Current indentation level.</returns>
        public void IncreaseDepth()
        {
            ++Depth;

        }

        /// <summary>Increases indentation level by the specified number of levels (can be 0 or negative).</summary>
        /// <param name="numlevels">Number of levels by which indentation is increased.</param>
        /// <returns>Current intentation level.</returns>
        public void IncreaseDepth(int numlevels)
        {
            Depth += numlevels;
        }

        /// <summary>Decreases indentation level by 1.</summary>
        /// <returns>Current indentation level.</returns>
        public void DecreaseDepth()
        {
            --Depth;
        }

        /// <summary>Decreases indentation level by the specified number of levels (can be 0 or negative).</summary>
        /// <param name="numlevels">Number of levels by which indentation is decreased.</param>
        /// <returns>Current intentation level.</returns>
        public void DecreaseDepth(int numlevels)
        {
            Depth -= numlevels;
        }



        // Levels of various kinds of reporting (User interface reporting, logging and tracing):

        private const ReportLevel
            DefaultReportingLevel = ReportLevel.Warning,
            DefaultLoggingLevel = ReportLevel.Warning,
            DefaultTracingLevel = ReportLevel.Info;

        private ReportLevel
            _ReportingLevel = DefaultReportingLevel,
            _LoggingLevel   = DefaultLoggingLevel,
            _TracingLevel   = DefaultTracingLevel;

        private bool 
            _TreatError     = DefaultReportingLevel >= ReportLevel.Error || DefaultLoggingLevel >= ReportLevel.Error || 
                    DefaultTracingLevel>=ReportLevel.Error,
            _TreatWarning   = DefaultReportingLevel >= ReportLevel.Warning || DefaultLoggingLevel >= ReportLevel.Warning || 
                    DefaultTracingLevel >= ReportLevel.Warning,
            _TreatInfo      = DefaultReportingLevel >= ReportLevel.Info || DefaultLoggingLevel >= ReportLevel.Info || 
                    DefaultTracingLevel >= ReportLevel.Info,
            _TreatUndefined = DefaultReportingLevel >= ReportLevel.Verbose || DefaultLoggingLevel >= ReportLevel.Verbose || DefaultTracingLevel >= ReportLevel.Verbose;

        private void SyncLevels()
        {
            _TreatError     = _ReportingLevel >= ReportLevel.Error || _LoggingLevel >= ReportLevel.Error || 
                    _TracingLevel>=ReportLevel.Error;
            _TreatWarning   = _ReportingLevel >= ReportLevel.Warning || _LoggingLevel >= ReportLevel.Warning || 
                    _TracingLevel >= ReportLevel.Warning;
            _TreatInfo = _ReportingLevel >= ReportLevel.Info || _LoggingLevel >= ReportLevel.Info || 
                    _TracingLevel >= ReportLevel.Info;
            _TreatUndefined = _ReportingLevel >= ReportLevel.Verbose || _LoggingLevel >= ReportLevel.Verbose || 
                    _TracingLevel >= ReportLevel.Verbose;
        }

        /// <summary>Resets the various kinds of reporting levels to default values.</summary>
        public void ResetLevels()
        {
            ReportingLevel = ReportLevel.Warning;
            LoggingLevel = ReportLevel.Warning;
            TracingLevel = ReportLevel.Info;
        }

        /// <summary>Gets or sets level of output for reporting (console output, message boxes, etc.).</summary>
        public ReportLevel ReportingLevel 
        { 
            get { return _ReportingLevel; }
            set 
            { 
                _ReportingLevel = value;
                SyncLevels();  // update dependent flags used in properties
                if (_ReportingSwitch != null) SyncTraceSwitchWithReportLevel(_ReportingLevel, _ReportingSwitch); 
            }
        }

        /// <summary>Gets or sets level of output for logging (writing to log files).</summary>
        public ReportLevel LoggingLevel 
        { 
            get { return _LoggingLevel; }
            set 
            { 
                _LoggingLevel = value;
                SyncLevels();  // update dependent flags used in properties
                if (_LoggingSwitch != null) SyncTraceSwitchWithReportLevel(_LoggingLevel, _LoggingSwitch); 
            }
        }

        /// <summary>Gets or sets trace level (for external trace systems).</summary>
        public ReportLevel TracingLevel 
        { 
            get { return _TracingLevel; } 
            set 
            { 
                _TracingLevel = value;
                SyncLevels();  // update dependent flags used in properties
                if (_TracingSwitch != null) SyncTraceSwitchWithReportLevel(_TracingLevel, _TracingSwitch); 
            } 
        }

        // Auxiliary properties & indexers dealing with levels

        /// <summary>Returns a boolean value indicating whether errors are treated by the reporter in its current state.</summary>
        public bool TreatError 
        { get { return _TreatError; } }

        /// <summary>Returns a boolean value indicating whether warnings are treated by the reporter in its current state.</summary>
        public bool TreatWarning 
        { get { return _TreatWarning; } }

        /// <summary>Returns a boolean value indicating whether info messages are treated by the reporter in its current state.</summary>
        public bool TreatInfo 
        { get { return _TreatInfo;  }  }

        /// <summary>Returns a boolean value indicating whether undefined messages with the lowest priority are treated by the reporter in its current state.</summary>
        public bool TreatUndefined
        { get { return _TreatUndefined; } }

        
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
        public ReportLevel Level {
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
                            level1 = (ReportLevel) ((int) level - 1);
                    }
                    catch {  }
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
// they just contribute to the Reporter's API for those users used to deal with the TraceSwitch class.
// It is not yet clear whether this feature will be kept in the futre.


#if !NETFRAMEWORK
    private double DDD;
#endif

        /// <summary>Returns the System.Diagnostics.EventLogEntryType value corresponding to the given ReportType.
        /// Remark: FailureAudit and SuccessAudit can not be generated because they don't have representation in ReportType.</summary>
        /// <param name="rt">ReportType value to be converted.</param>
        /// <returns>Converted value of type EventLogEntryType.</returns>
        public static EventLogEntryType ReportType2EventLogEntryType(ReportType rt)
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
        /// <param name="el">EventLogEntryType value to be converted.</param>
        /// <returns>Converted value of type ReportType.</returns>
        public static ReportType EventLogEntryType2ReportType(EventLogEntryType el)
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
        public static TraceLevel ReportLevel2TraceLevel(ReportLevel level)
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
        /// <param name="tl">TraceLevel value to be converted.</param>
        /// <returns>Converted value of type ReportLevel.</returns>
        public static ReportLevel TraceLevel2ReportLevel(TraceLevel tl)
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
            if (tswitch!=null)
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
        /// It is left entirely to error reporting delegates to interpret the object's contents.</summary>
        public object Obj { get { return _obj; } set { _obj=value; } } 
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

        /// <summary>Reportinf of internal reporter errors for the specific reporter class (overridden in derived classes).
        /// This method is called for internal error reports when the delegate for this job is not defined.</summary>
        /// <param name="messagetype"></param>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="ex1"></param>
        protected virtual void ReserveReportErrorSpecific(ReportType messagetype, string location,
                    string message, Exception ex, Exception ex1)
        {
            DefaultReserveReportError(this, messagetype, location, message, ex, ex1);
        }

        /// <summary>Used to report internal errors of the reporter.
        /// Designed to be bullet proof in order to ensure that improper behavior of the reporting system does not remain unnoticed.</summary>
        /// <param name="messagetype"></param>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="ex1"></param>
        protected void ReserveReportError(ReportType messagetype, string location,
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
                        ReserveReportErrorSpecific(messagetype, location, message, ex, ex1);
                }
            }
            catch (Exception)
            {
                ReserveReportErrorSpecific(messagetype, location, message, ex, ex1);
            }
        }

        // GENERAL REPORTING METHODS (for all kinds of reports):

        /// <summary>Basic reporting method (overloaded). Launches an error report, a warning report or other kind of report/message.
        /// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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
                        if (DoReporting(messagetype))
                        {
                            if (ReportLocationDlg != null)
                                locationstr = ReportLocationDlg(this, messagetype, location, ex);
                            if (ReportMessageDlg != null)
                                messagestr = ReportMessageDlg(this, messagetype, message, ex);
                            if (ReportDlg != null)
                                ReportDlg(this, messagetype, locationstr, messagestr);
                        }
                    }
                    catch (Exception ex1)
                    {
                        this.ReserveReportError(ReportType.Error, location,
                            "Error in Reporter.Report. " + message, ex, ex1);
                    }
                    if (UseTextWriter && DoReporting(messagetype))
                    {
                        try
                        {
                            this.Report_TextWriter(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Reporter.Report_TextWriter. " + message, ex, ex1);
                        }
                    }
                    if (UseTextLogger && DoLogging(messagetype))
                    {
                        try
                        {
                            this.Report_TextLogger(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Reporter.Report_TextLogger. " + message, ex, ex1);
                        }
                    }
                    if (UseTrace && DoTracing(messagetype))
                    {
                        try
                        {
                            this.Report_Trace(messagetype, location, message, ex);
                        }
                        catch (Exception ex1)
                        {
                            this.ReserveReportError(ReportType.Error, location,
                                "Error in Reporter.Report_Trace. " + message, ex, ex1);
                        }
                    }

                    if (ThrowTestException)
                    {
                        // Throw a test exception (please leave the capitalized part of the message unchanged!):
                        throw new Exception("TEST INTERNAL EXCEPTION thrown by the reporter (after reporting has been performted).");
                    }
                }
            }
            catch (Exception ex1)
            {
                ThrowTestException = false;  // re-set, such that a test exception is thrown only once
                ReserveReportError(ReportType.Error, location, message, ex, ex1);
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
        /// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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
        /// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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



#region Reporting_TextWriter_TextLogger
        // Functionality that is common for both TextWriter and TextLogger


        /// <summary>Writes to a textwriter the introduction text that is usually printed before logging starts.</summary>
        /// <param name="tw">The TextWriter that the text is written to.</param>
        /// <param name="introtext">Introduction text that is printed (if null then text will be composed); additional newlines are printed.</param>
        /// <param name="programname">Name of the program or other entity that uses the reporter.</param>
        protected virtual void PrintIntro(TextWriter tw, string introtext, string programname)
        // $A Igor Dec08;
        {
            if (!string.IsNullOrEmpty(introtext))
            {
                tw.WriteLine(Environment.NewLine + Environment.NewLine
                    + introtext
                    + Environment.NewLine + Environment.NewLine);
            }
            else if (!string.IsNullOrEmpty(programname))
            {
                tw.WriteLine(Environment.NewLine + Environment.NewLine
                    + "Logging messages from " + programname
                    + ", starting at " + DateTime.Now.ToString() + ":"
                    + Environment.NewLine + Environment.NewLine);
            }
            else
            {
                tw.WriteLine(Environment.NewLine + Environment.NewLine
                    + "Logging messages from reporter"
                    + ", starting at " + DateTime.Now.ToString() + ":"
                    + Environment.NewLine + Environment.NewLine);
            }
            tw.Flush();
        }

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="tw">Textwriter that is updated.</param>
        /// <param name="disptw">Reference to the dispose flag that is also remembered.</param>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        // $A Igor Dec08;
        protected bool SetTW(ref TextWriter tw, ref bool disptw, TextWriter writer, bool writeintro, bool disposewriter)
        {
            bool ret = false;
            try
            {
                if (disptw)
                    if (tw != null)
                        tw.Dispose();
                tw = null;
            }
            catch { }
            tw = writer;
            disptw = disposewriter;
            if (tw != null)
            {
                ret = true;
                try
                {
                    if (writeintro)
                        TextWriterPrintIntro(writer);
                }
                catch { }
            }
            return ret;
        }


        /// <summary>Creates a TextWriter upon the stream and sets it as the basic TextWriter to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="tw">Textwriter that is updated.</param>
        /// <param name="disptw">Reference to the dispose flag that is also remembered.</param>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        protected bool SetTW(ref TextWriter tw, ref bool disptw, Stream stream, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            bool ret = false;
            try
            {
                if (disptw)
                    if (tw != null)
                        tw.Dispose();
                tw = null;
            }
            catch { }
            try
            {
                TextWriter writer = null;
                if (stream!=null)
                    writer = new StreamWriter(stream);
                ret = SetTW(ref tw, ref disptw, writer, writeintro, disposewriter);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, "Reporter.SetTW()", "Could not create a TextWriter upon a stream.", ex, null);
            }
            return ret;
        }

        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="tw">Textwriter that is updated.</param>
        /// <param name="disptw">Reference to the dispose flag that is also remembered.</param>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTW(ref TextWriter tw, ref bool disptw, string filename, bool append, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            bool ret = false;
            try
            {
                if (disptw)
                    if (tw != null)
                        tw.Dispose();
                tw = null;
            }
            catch { }
            try
            {
                TextWriter writer = null;
                if (!string.IsNullOrEmpty(filename))
                    writer = new StreamWriter(filename, append);
                ret = SetTW(ref tw, ref disptw, writer, writeintro, disposewriter);
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, "Reporter.SetTW", "Could not open a file for "
                    + (append ? "appending." : "writing."), ex, null);
            }
            return ret;
        }


        /// <summary>A class for storing TextWriters and some data associated with them (such as the name of 
        /// the file from which a TextWriter was created), which enables searching on basis of this data.</summary>
        protected class TWClass
        {
            public TextWriter Writer = null;
            private bool DisposeWriter = false;

            // Fields that memorize original data and enable searching for the specific writer:
            private string _filename = null;
            private Stream _stream = null;

            public string filename { get { return _filename; } protected set { _filename = value; } }
            public Stream stream { get { return _stream; } protected set { _stream = value; } }

            // Constructor:

            private ReporterBase rep = null;

            private TWClass() { }  // hide default constructor (force specifying the Reporter)

            /// <summary>Public constructor, requires the Reporter object on which this object is installed.</summary>
            /// <param name="R">Reporter object on which the created instance is installed.</param>
            public TWClass(ReporterBase R)
            {
                if (R == null)
                    throw new ArgumentNullException("R","The Parent Reporter object is not specified.");
                rep = R;
            }

            public void ClearWriter()
            {
                try
                {
                    if (DisposeWriter && Writer != null)
                    {
                        Writer.Close();
                        Writer = null;
                    }
                }
                catch { }
                DisposeWriter = false;
                filename = null;
                stream = null;
            }

            /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
            /// property by the 'writeintro' argument.</summary>
            /// <param name="writer">Textwriter to which reporting will be performed.</param>
            /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
            /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
            /// is used, taking into account the introtext and programname properties.</param>
            /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
            /// Default is false when specifying a textwriter.</param>
            /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
            public bool SetWriter(TextWriter writer, bool writeintro, bool disposewriter)
            {
                bool ret = false;
                try
                {
                    if (DisposeWriter && Writer != null)
                        Writer.Close();
                    Writer = null;
                }
                catch { }
                Writer = writer;
                DisposeWriter = disposewriter;
                if (Writer != null)
                {
                    ret = true;
                    try
                    {
                        if (writeintro)
                            rep.TextWriterPrintIntro(writer);
                    }
                    catch { }
                }
                return ret;
            }


            /// <summary>Creates a TextWriter upon the stream and sets it as the basic TextWriter to which reporting is 
            /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
            /// <param name="stream">Textwriter to which reporting will be performed.</param>
            /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
            /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
            /// is used, taking into account the introtext and programname properties.</param>
            /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
            /// Default is false when specifying a stream.</param>
            /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
            public bool SetWriter(Stream stream, bool writeintro, bool disposewriter)
            // $A Igor Dec08;
            {
                bool ret = false;
                this.stream = stream;
                try
                {
                    TextWriter writer = new StreamWriter(stream);
                    ret = SetWriter(writer, writeintro, disposewriter);
                }
                catch (Exception ex)
                {
                    SetWriter((TextWriter) null, writeintro, disposewriter);
                    if (rep!=null)
                        rep.ReserveReportError(ReportType.Error, "Reporter.SetWriter()", "Could not create a TextWriter upon a stream.", ex, null);
                }
                return ret;
            }

            /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.
            /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
            /// written before reporting to the file begins.</summary>
            /// <param name="filename">Name of the file to which reporting will be performed.</param>
            /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
            /// new text is appended at the end of the file.</param>
            /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
            /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
            /// is used, taking into account the introtext and programname properties.</param>
            /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
            /// Default is true when specifying a file name.</param>
            /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
            public bool SetWriter(string filename, bool append, bool writeintro, bool disposewriter)
            // $A Igor Dec08;
            {
                bool ret = false;
                this.filename = filename;
                try
                {
                    TextWriter writer = new StreamWriter(filename, append);
                    ret = SetWriter(writer, writeintro, disposewriter);
                    DisposeWriter = disposewriter;  // must be disposed when not used any more
                }
                catch (Exception ex)
                {
                    SetWriter((TextWriter) null, writeintro, disposewriter);
                    rep.ReserveReportError(ReportType.Error, "Reporter.SetWriter", "Could not open a file for "
                        + (append ? "appending." : "writing."), ex, null);
                }
                return ret;
            }

        }  // class TWClass


        /// <summary>Adds another TextWriter to which reporting will also be performed.</summary>
        /// <param name="lwriters">List on which a TextWriter is added.</param>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        protected bool AddTW(ref List<TWClass> lwriters, TextWriter writer, bool writeintro, bool disposewriter)
        {
            bool ret = false;
            try
            {
                TWClass tw = new TWClass(this);
                if (tw != null)
                {
                    if (FindTW(lwriters, writer) > 0)
                        ReserveReportError(ReportType.Warning, "Adding text writer", "Could add a text writer because it is already on the list.", null, null);
                    else
                    {
                        ret = tw.SetWriter(writer, writeintro, disposewriter);
                        if (ret)
                            lwriters.Add(tw);
                    }
                }
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, "Reporter.AddTextWriter", "Could add a textwriter.", ex, null);
            }
            return ret;
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="lwriters">List on which a TextWriter is added.</param>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        protected bool AddTW(ref List<TWClass> lwriters, Stream stream, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
            }

            bool ret = false;
            try
            {
                TWClass tw = new TWClass(this);
                if (tw != null)
                {
                    if (FindTW(lwriters, stream) > 0)
                        ReserveReportError(ReportType.Warning, "Adding text writer", "Could add a stream because it is already on the list.", null, null);
                    else
                    {
                        ret = tw.SetWriter(stream, writeintro, disposewriter);
                        if (ret)
                            lwriters.Add(tw);
                    }
                }
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, "Reporter.AddWriter", "Could add a textwriter.", ex, null);
            }
            return ret;
        }

        /// <summary>Creates a TextWriter from a specific file and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is written before reporting to the file begins.</summary>
        /// <param name="lwriters">List on which a TextWriter is added.</param>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        protected bool AddTW(ref List<TWClass> lwriters, string filename, bool append, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            bool ret = false;
            try
            {
                TWClass tw = new TWClass(this);
                if (tw != null)
                {
                    if (FindTW(lwriters, filename) > 0)
                        ReserveReportError(ReportType.Warning, "Adding text writer", "Could add a file because it is already on the list.", null, null);
                    else
                    {
                        ret = tw.SetWriter(filename, append, writeintro, disposewriter);
                        if (ret)
                            lwriters.Add(tw);
                    }
                }
            }
            catch (Exception ex)
            {
                ReserveReportError(ReportType.Error, "Reporter.AddWriter", "Could add a textwriter.", ex, null);
            }
            return ret;
        }


        /// <summary>Returns the first object on the list that contains a specific TextWriter.</summary>
        private int FindTW(List<TWClass> lwriters, TextWriter writer)
        // $A Igor Dec08;
        {
            int ret = -1;
            try
            {
                if (lwriters != null)
                {
                    int num = lwriters.Count;
                    for (int i = 0; i < num; ++i)
                    {
                        TWClass tw = lwriters[i];
                        if (tw.Writer == writer)
                            return i;
                    }
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Returns the first object on the list whose TextWriter has been created from the specific Stream.</summary>
        private int FindTW(List<TWClass> lwriters, Stream stream)
        // $A Igor Dec08;
        {
            int ret = -1;
            try
            {
                if (lwriters != null)
                {
                    int num = lwriters.Count;
                    for (int i = 0; i < num; ++i)
                    {
                        TWClass tw = lwriters[i];
                        if (tw.stream == stream)
                            return i;
                    }
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Returns the first object on the list whose TextWriter has been created from a file with the specific name.</summary>
        private int FindTW(List<TWClass> lwriters, string filename)
        // $A Igor Dec08;
        {
            int ret = -1;
            try
            {
                if (lwriters != null)
                {
                    int num = lwriters.Count;
                    for (int i = 0; i < num; ++i)
                    {
                        TWClass tw = lwriters[i];
                        if (tw.filename == filename)
                            return i;
                    }
                }
            }
            catch { }
            return ret;
        }


        /// <summary>Removes the first object from a list that contains the specified TextWriter.</summary>
        /// <returns>true if the appropriate text writer was found and successfully removed.</returns>
        protected bool RemoveTW(List<TWClass> lwriters, TextWriter writer)
        // $A Igor Dec08;
        {
            bool ret = false;
            try
            {
                int index;
                index = FindTW(lwriters, writer);
                if (index >= 0)
                {
                    TWClass tw = lwriters[index];
                    if (tw != null)
                        tw.ClearWriter();
                    lwriters.RemoveAt(index);
                    ret = true;
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Removes the first object from a list whose TextWriter has been created form the specified stream.</summary>
        protected bool RemoveTW(List<TWClass> lwriters, Stream stream)
        // $A Igor Dec08;
        {
            bool ret = false;
            try
            {
                int index;
                index = FindTW(lwriters, stream);
                if (index >= 0)
                {
                    TWClass tw = lwriters[index];
                    if (tw != null)
                        tw.ClearWriter();
                    lwriters.RemoveAt(index);
                    ret = true;
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Removes the first object from a list whose TextWriter has been created form the file with the specified name.</summary>
        protected bool RemoveTW(List<TWClass> lwriters, string filename)
        // $A Igor Dec08;
        {
            bool ret = false;
            try
            {
                int index;
                index = FindTW(lwriters, filename);
                if (index >= 0)
                {
                    TWClass tw = lwriters[index];
                    if (tw != null)
                        tw.ClearWriter();
                    lwriters.RemoveAt(index);
                    ret = true;
                }
            }
            catch { }
            return ret;
        }



#endregion // Reporting_TextWriter_TextLogger


#region Reporting_TextWriter

        //Data & its manipulation: 

        private bool _UseTextWriter = true;

        /// <summary>Gets or sets the flag specifying whether reporting using a text writer is performed or not.</summary>
        public bool UseTextWriter { get { return _UseTextWriter; } set { _UseTextWriter = value; } }

        private bool _TextWriterFlushing = false;

        /// <summary>Gets or sets the flag that tells whether or not the text writers are flushed after 
        /// every message that is reported through them.</summary>
        public bool TextWriterFlushing { get { return _TextWriterFlushing; } set { _TextWriterFlushing = value; } }

        // Variables that hold the TextWriter subsystem's text writers and their state:
        private TextWriter Writer = null;
        private bool DisposeWriter = false;  // specifies that writer must be disposed when it is changed.

        private List<TWClass> Writers = new List<TWClass>();

        // Automatic introductory text when a text writer is added:

        private string 
            _TextWriterIntroText = null,
            _TextWriterProgramName = null;
        private bool _TextWriterWriteIntro = true;

        /// <summary>Gets or sets the introduction string that is written before logging to a TextWriter begins.
        /// If this is not specified then the reporter composes its own introduction string, eventually using 
        /// programname (when defined).</summary>
        public string TextWriterIntroText   { get { return _TextWriterIntroText; } set { _TextWriterIntroText = value; } }

        /// <summary>String denoting the name of the program or other entity that uses the Reporter for logging.
        /// When introtext is not specified, this name is used in the introduction text composed by the reporter.</summary>
        public string TextWriterProgramName { get { return _TextWriterProgramName;  } set { _TextWriterProgramName = value; } }

        /// <summary>Specifies whether introduction text is written before logging of messages begins or not.</summary>
        public bool TextWriterWriteIntro  { get { return _TextWriterWriteIntro; } set { _TextWriterWriteIntro = value; } }

        /// <summary>Writes to a textwriter the introduction text that is usually printed before logging starts.</summary>
        /// <param name="tw">The TextWriter that the text is written to.</param>
        protected virtual void TextWriterPrintIntro(TextWriter tw)
        {
            PrintIntro(tw, TextWriterIntroText, TextWriterProgramName);
        }

        // Manipulation of subsystem's  text writers:

        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(TextWriter writer)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextWriter(writer, TextWriterWriteIntro);
            }
        }

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(TextWriter writer, bool writeintro)
        {
            lock (lockobj)
            {
                return SetTextWriter(writer, writeintro, false /* disptw */);
            }
        }


        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(TextWriter writer, bool writeintro, bool disposewriter)
        {
            lock (lockobj)
            {
                return SetTW(ref Writer, ref DisposeWriter, writer, writeintro, disposewriter);
            }
        }


        /// <summary>Creates a TextWriter upon the stream and sets it as the text writer to which reporting is also performed.</summary>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(Stream stream)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextWriter(stream, TextWriterWriteIntro);
            }
        }

        /// <summary>Creates a TextWriter upon the stream and sets it as the basic TextWriter to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(Stream stream, bool writeintro)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextWriter(stream, writeintro, false /* disptw */);
            }
        }


        /// <summary>Creates a TextWriter upon the stream and sets it as the basic TextWriter to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(Stream stream, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock(lockobj)
            {
                return SetTW(ref Writer, ref DisposeWriter, stream, writeintro, disposewriter);
            }
        }


        /// <summary>Creates a TextWriter upon a file and sets it as the basic TextWriter to which reporting is also performed.
        /// The file is overwritten.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(string filename)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextWriter(filename, false /* no appending */, TextWriterWriteIntro);
            }
        }

        /// <summary>Creates a TextWriter upon a file and sets it as the basic TextWriter to which reporting is also performed,
        /// where the caller specifies either to overwrite the file or to append to it.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(string filename, bool append)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextWriter(filename, append, TextWriterWriteIntro);
            }
        }

        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(string filename, bool append, bool writeintro)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextWriter(filename, append, writeintro, true /* disptw */);
            }
        }

        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextWriter(string filename, bool append, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock(lockobj)
            {
                return SetTW(ref Writer, ref DisposeWriter, filename, append, writeintro, disposewriter);
            }
        }


        // Functionality for defining multiple TextWriters:

        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(TextWriter writer)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTextWriter(writer, TextWriterWriteIntro);
            }
        }

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(TextWriter writer, bool writeintro)
        {
            lock (lockobj)
            {
                return AddTextWriter(writer, writeintro, false /* disptw */);
            }
        }


        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(TextWriter writer, bool writeintro, bool disposewriter)
        {
            lock (lockobj)
            {
                return AddTW(ref Writers /* lwriters */, writer, writeintro, disposewriter);
            }
        }



        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed.</summary>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(Stream stream)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTextWriter(stream, TextWriterWriteIntro);
            }
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(Stream stream, bool writeintro)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTextWriter(stream, writeintro, false /* disptw */);
            }
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(Stream stream, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTW(ref Writers /* lwriters */, stream, writeintro, disposewriter);
            }
        }

        /// <summary>Creates a TextWriter from the file name and adds it to the list of TextWriters on which
        /// reporting is also performed.
        /// The file is overwritten.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(string filename)
        // $A Igor Dec08;
        {
            return AddTextWriter(filename, false /* no appending */, TextWriterWriteIntro);
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies either to overwrite the file or to append to it.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(string filename, bool append)
        // $A Igor Dec08;
        {
            return AddTextWriter(filename, append, TextWriterWriteIntro);
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(string filename, bool append, bool writeintro)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTextWriter(filename, append, writeintro, true /* disptw */);
            }
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextWriter(string filename, bool append, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTW(ref Writers, filename, append, writeintro, disposewriter);
            }
        }


        /// <summary>Removes all text writers from the TextWriter subsystem.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if al text writers were successfully removed, false if there were problems.</returns>
        public bool RemoveTextWriters()
        {
            bool ret = true;
            lock (lockobj)
            {
                // Remove the default text writer:
                try
                {
                    bool doremove = false, wasremoved=false;
                    if (Writer != null && DisposeWriter)
                        doremove = true;
                    wasremoved = RemoveTextWriter();
                    if (doremove && !wasremoved)
                        ret = false;
                }
                catch { }
                if (Writers != null)
                {
                    // Remove text writers from the list:
                    int i = Writers.Count - 1;
                    while (i >= 0)
                    {
                        bool failed = true;
                        try
                        {
                            TWClass tw = Writers[i];
                            if (tw != null)
                                tw.ClearWriter();
                            failed = false;
                            Writers.RemoveAt(i);
                        }
                        catch { }
                        if (failed)
                            ret = false;
                        --i;
                    }
                }
            }
            return ret;
        }


        /// <summary>Removes the default text writer from the TextWriter subsystem.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed)</returns>
        public bool RemoveTextWriter()
        {
            bool ret = true;
            lock (lockobj)
            {
                try
                {
                    if (Writer == null)
                        ret = false;
                    else if (DisposeWriter)
                    {
                        ret = false;
                        Writer.Close(); // Close() will call Dispose()
                        Writer = null;
                        ret = true;
                    }
                }
                catch { }
            }
            return ret;
        }


        /// <summary>Removes the first object from Writers that contains the specified TextWriter.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        public bool RemoveTextWriter(TextWriter writer)
        // $A Igor feb09;
        {
            lock (lockobj)
            {
                return RemoveTW(Writers, writer);
            }
        }

        /// <summary>Removes the first object from Writers whose TextWriter has been created form the specified stream.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        public bool RemoveTextWriter(Stream stream)
        // $A Igor feb09;
        {
            lock (lockobj)
            {
                return RemoveTW(Writers, stream);
            }
        }

        /// <summary>Removes the first object from Writers whose TextWriter has been created form the file with the specified name.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        public bool RemoveTextWriter(string filename)
        // $A Igor feb09;
        {
            lock (lockobj)
            {
                return RemoveTW(Writers, filename);
            }
        }


#region TextWriter_Custom_Operations

        // This provides support for performing custom operations on TextWriter's output streams. 
        // These are operations taht are not in the scope of commonly supported by teh Repoirter, and
        // enable, for example, to write custom text at specific points.

        /// <summary>Returns a list of all text writers that are currently used by the reporter's text writer.
        /// Only text writers that are actually writable are included.</summary>
        /// <returns>List of text writers that are currently used by the reporter's text writer
        /// (only those that are actually writable are included).</returns>
        public List<TextWriter> TextWriterWriters()
        // $A Igor Feb09;
        {
            return TextWriterWriters(true);
        }

        /// <summary>Returns a list of all text writers that are currently used by the reporter's text writer.
        /// Warning: Beware of thread safety issues! 
        /// Blocks of code where the returned list is used should be enclosed in lock(reporter.lockobj){...} block
        /// (where reporter is the object through which this method was called).</summary>
        /// <param name="writableonly">If true then only those text writers are listed that are actually writable.
        /// If false then all text writers are listed.</param>
        /// <returns>List of text writers that are currently used by the reporter's text writer.</returns>
        public List<TextWriter> TextWriterWriters(bool writableonly)
        // $A Igor Feb09;
        {
            lock (lockobj)
            {
                List<TextWriter> ret = new List<TextWriter>();
                try
                {
                    TextWriter tw = Writer;
                    if (tw != null)
                    {
                        if (!writableonly)
                            ret.Add(tw);
                        else
                        {
                            try
                            {
                                tw.Write("");
                                ret.Add(tw); // add writer to the list only if test operation does not throw an exception.
                            }
                            catch { }
                        }
                    }
                    for (int i = 0; i < Writers.Count; ++i)
                    {
                        try
                        {
                            tw = Writers[i].Writer;
                            if (!writableonly)
                                ret.Add(tw);
                            else
                            {
                                tw.Write("");
                                ret.Add(tw); // add writer to the list only if test operation does not throw an exception.
                            }
                        }
                        catch { }
                    }
                }
                catch { }
                return ret;
            }
        }


        /// <summary>Returns the current number of text writers used by the reporter's text logging module.
        /// Only text writers that are actually writable are counted.</summary>
        /// <returns>The current number of text writers used by the reporter's text logging module 
        /// (only those that are actually writable are counted).</returns>
        public int TextWriterNumWriters()
        // $A Igor Feb09;
        {
            return TextWriterNumWriters(true);
        }

        /// <summary>Returns the current number of TextWriters used by the reporter's text logging module.</summary>
        /// <param name="writableonly">If true then only those text writers are counted that are actually writable.
        /// If false then all text writers are returned.</param>
        /// <returns>The current number of text writers used by the reporter's text logging module.</returns>
        public int TextWriterNumWriters(bool writableonly)
        // $A Igor Feb09;
        {
            int ret = 0;
            lock (lockobj)
            {
                try
                {
                    TextWriter tw = Writer;
                    if (tw != null)
                    {
                        if (!writableonly)
                            ++ret;
                        else
                        {
                            try
                            {
                                tw.Write("");
                                ++ret; // count this writer only if test operation does not throw an exception
                            }
                            catch { }
                        }
                    }
                    for (int i = 0; i < Writers.Count; ++i)
                    {
                        try
                        {
                            tw = Writers[i].Writer;
                            if (!writableonly)
                                ++ret;
                            else
                            {
                                tw.Write("");
                                ++ret; // count this writer only if test operation does not throw an exception
                            }
                        }
                        catch { }
                    }
                }
                catch { }
                return ret;
            }
        }


        /// <summary>Flushes all text writers of the Writer's TextWriter subsystem.</summary>
        /// <returns>Number of writers that has actually been flushed.</returns>
        public int TextWriterFlush()
        // $A Igor Feb09;
        {
            int ret = 0;
            lock (lockobj)
            {
                List<TextWriter> writers = TextWriterWriters();
                bool doflushing = TextWriterFlushing;
                if (writers != null)
                    for (int i = 0; i < writers.Count; ++i)
                    {
                        try
                        {
                            TextWriter writer = writers[i];
                            if (writer != null)
                            {
                                writer.Flush();
                                ++ret;
                            }
                        }
                        catch { }
                    }
            }
            return ret;
        }

        /// <summary>Writes a string to all text writers of the Writer's TextWriter subsystem.</summary>
        /// <param name="str">String to be written.</param>
        /// <returns>Number of writers that the string has actually been written to.</returns>
        public int TextWriterWrite(string str)
        // $A Igor Feb09;
        {
            lock (lockobj)
            {
                int numwritten = 0, numerrors = 0;
                bool doflushing = TextWriterFlushing;
                try
                {
                    // Write to the basic output stream:
                    if (Writer != null)
                    {
                        ++numerrors;
                        Writer.Write(str);
                        if (doflushing)
                            Writer.Flush();
                        --numerrors;
                        ++numwritten;
                    }
                }
                catch { }
                try
                {
                    if (Writers != null)
                        for (int i = 0; i < Writers.Count; ++i)
                        {
                            try
                            {
                                TextWriter writer = Writers[i].Writer;
                                if (writer != null)
                                {
                                    ++numerrors;
                                    writer.Write(str);
                                    if (doflushing)
                                        writer.Flush();
                                    --numerrors;
                                    ++numwritten;
                                }
                            }
                            catch { }
                        }
                }
                catch { }
                if (numerrors > 0)
                    throw new Exception("TextWriter: Writing to " + numerrors.ToString() + " output streams failed.");
                return numwritten;
            }  // lock
        }


        /// <summary>Similar to TextWriterWrite(), except that a newline is added at the end of the string.</summary>
        public int TextWriterWriteLine(string str)
        // $A Igor Feb09;
        {
            return TextWriterWrite(str + Environment.NewLine);
        }

#endregion  // TextWriter_Custom_Operations


        // Delegates with default values:

        /// <summary>Delegate that performs reporting (actually logging) via text writer.</summary>
        public ReportDelegate ReportDlgTextWriter = new ReportDelegate(DefaultReport_TextWriter);

        /// <summary>Delegate that assembles the location string for reporting via TextWriter.</summary>
        public ReportLocationDelegate ReportLocationDlgTextWriter = new ReportLocationDelegate(DefaultReportLocation_TextWriter);

        /// <summary>Delegate that assembles the message string for reporting via text writer.</summary>
        public ReportMessageDelegate ReportMessageDlgTextWriter = new ReportMessageDelegate(DefaultReportMessage_TextWriter);


        // Default delegate methods for reporting via TextWriter:


        /// <summary>Default delegate for launching reports (actually logging reports) via text writer.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_TextWriter(ReporterBase reporter, ReportType messagetype,
            string location, string message)
        // $A Igor Dec08;
        {
            if (reporter == null)
                throw new Exception("The reporter object containing auxiliary data is not specified.");
            // Assemble the string that is written to the streams:
            string msg = DefaultReportStringConsoleTimeStamp(reporter, messagetype,
                     location, message);
            // Write the string to all registered streams and files:
            int numwritten = reporter.TextWriterWriteLine(msg);
        }


        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_TextWriter(ReporterBase reporter, ReportType messagetype,
                string location, Exception ex)
        // $A Igor Dec08;
        {
            return DefaultLocationString(reporter, messagetype, location, ex);
        }

        /// <summary>Delegate for assembling a report message for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="basicmessage">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Message string that can be used in a report.</returns>
        protected static string DefaultReportMessage_TextWriter(ReporterBase reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        // $A Igor Dec08;
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }



        // Methods for reporting via text writer:

        /// <summary>Launches a report via text writers.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_TextWriter(ReportType messagetype, string location, string message, Exception ex)
        // $A Igor Dec08;
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





#region Reporting_TextLogger

        //Data & its manipulation: 

        private bool _UseTextLogger = false;

        /// <summary>Gets or sets the flag specifying whether reporting using a text writer is performed or not.</summary>
        public bool UseTextLogger { get { return _UseTextLogger; } set { _UseTextLogger = value; } }

        private bool _TextLoggerFlushing = true;

        /// <summary>Gets or sets the flag that tells whether or not the text writers are flushed after 
        /// every message that is reported through them.</summary>
        public bool TextLoggerFlushing { get { return _TextLoggerFlushing; } set { _TextLoggerFlushing = value; } }


        // Variables that hold the TextLogger subsystem's text writers and their state:
        private TextWriter Logger = null;
        private bool DisposeLogger = false;  // specifies that writer must be disposed when it is changed.

        private List<TWClass> Loggers = new List<TWClass>();

        // Automatic introductory text when a text writer is added:

        private string
            _TextLoggerIntroText = null,
            _TextLoggerProgramName = null;
        private bool _TextLoggerWriteIntro = true;

        /// <summary>Gets or sets the introduction string that is written before logging to a TextWriter begins.
        /// If this is not specified then the reporter composes its own introduction string, eventually using 
        /// programname (when defined).</summary>
        public string TextLoggerIntroText { get { return _TextLoggerIntroText; } set { _TextLoggerIntroText = value; } }

        /// <summary>String denoting the name of the program or other entity that uses the Reporter for logging.
        /// When introtext is not specified, this name is used in the introduction text composed by the reporter.</summary>
        public string TextLoggerProgramName { get { return _TextLoggerProgramName; } set { _TextLoggerProgramName = value; } }

        /// <summary>Specifies whether introduction text is written before logging of messages begins or not.</summary>
        public bool TextLoggerWriteIntro { get { return _TextLoggerWriteIntro; } set { _TextLoggerWriteIntro = value; } }

        /// <summary>Writes to a textwriter the introduction text that is usually printed before logging starts.</summary>
        /// <param name="tw">The TextWriter that the text is written to.</param>
        protected virtual void TextLoggerPrintIntro(TextWriter tw)
        {
            PrintIntro(tw, TextLoggerIntroText, TextLoggerProgramName);
        }


        // Manipulation of subsystem's  text writers:

        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(TextWriter writer)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextLogger(writer, TextLoggerWriteIntro);
            }
        }

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(TextWriter writer, bool writeintro)
        {
            lock (lockobj)
            {
                return SetTextLogger(writer, writeintro, false /* disptw */);
            }
        }


        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(TextWriter writer, bool writeintro, bool disposewriter)
        {
            lock (lockobj)
            {
                return SetTW(ref Logger, ref DisposeLogger, writer, writeintro, disposewriter);
            }
        }


        /// <summary>Creates a TextWriter upon the stream and sets it as the text writer to which reporting is also performed.</summary>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(Stream stream)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextLogger(stream, TextLoggerWriteIntro);
            }
        }

        /// <summary>Creates a TextWriter upon the stream and sets it as the basic TextWriter to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(Stream stream, bool writeintro)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextLogger(stream, writeintro, false /* disptw */);
            }
        }



        /// <summary>Creates a TextWriter upon the stream and sets it as the basic TextWriter to which reporting is 
        /// performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(Stream stream, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTW(ref Logger, ref DisposeLogger, stream, writeintro, disposewriter);
            }
        }


        /// <summary>Creates a TextWriter upon a file and sets it as the basic TextWriter to which reporting is also performed.
        /// The file is overwritten.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(string filename)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextLogger(filename, false /* no appending */, TextLoggerWriteIntro);
            }
        }

        /// <summary>Creates a TextWriter upon a file and sets it as the basic TextWriter to which reporting is also performed,
        /// where the caller specifies either to overwrite the file or to append to it.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(string filename, bool append)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextLogger(filename, append, TextLoggerWriteIntro);
            }
        }

        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(string filename, bool append, bool writeintro)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTextLogger(filename, append, writeintro, true /* disptw */);
            }
        }

        /// <summary>Creates a TextWriter upon a file and sets it as the text writer to which reporting is also performed.
        /// The caller specifies whether to overwrite the file or to append to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool SetTextLogger(string filename, bool append, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return SetTW(ref Logger, ref DisposeLogger, filename, append, writeintro, disposewriter);
            }
        }



        // Functionality for defining multiple TextWriters:

        /// <summary>Sets the text writer to which reporting is also performed.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(TextWriter writer)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTextLogger(writer, TextLoggerWriteIntro);
            }
        }

        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(TextWriter writer, bool writeintro)
        {
            lock (lockobj)
            {
                return AddTextLogger(writer, writeintro, false /* disptw */);
            }
        }


        /// <summary>Sets the basic text writer to which reporting is performed, but overrides the writeintro
        /// property by the 'writeintro' argument.</summary>
        /// <param name="writer">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a textwriter.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(TextWriter writer, bool writeintro, bool disposewriter)
        {
            lock (lockobj)
            {
                return AddTW(ref Loggers /* lwriters */, writer, writeintro, disposewriter);
            }
        }



        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed.</summary>
        /// <param name="stream">Stream to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(Stream stream)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTextLogger(stream, TextLoggerWriteIntro);
            }
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(Stream stream, bool writeintro)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTextLogger(stream, writeintro, false /* disptw */);
            }
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. Overrides the writeintro property by the 'writeintro' argument.</summary>
        /// <param name="stream">Textwriter to which reporting will be performed.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is false when specifying a stream.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(Stream stream, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTW(ref Loggers /* lwriters */, stream, writeintro, disposewriter);
            }
        }

        /// <summary>Creates a TextWriter from the file name and adds it to the list of TextWriters on which
        /// reporting is also performed.
        /// The file is overwritten.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(string filename)
        // $A Igor Dec08;
        {
            return AddTextLogger(filename, false /* no appending */, TextLoggerWriteIntro);
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies either to overwrite the file or to append to it.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(string filename, bool append)
        // $A Igor Dec08;
        {
            return AddTextLogger(filename, append, TextLoggerWriteIntro);
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is
        /// written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(string filename, bool append, bool writeintro)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTextLogger(filename, append, writeintro, true /* disptw */);
            }
        }

        /// <summary>Creates a TextWriter from the stream and adds it to the list of TextWriters on which
        /// reporting is also performed. The caller specifies whether to overwrite the file or to append 
        /// to it, and whether the introductory text is written before reporting to the file begins.</summary>
        /// <param name="filename">Name of the file to which reporting will be performed.</param>
        /// <param name="append">If false then eventual existing contents of the file are overwritten. Otherwise,
        /// new text is appended at the end of the file.</param>
        /// <param name="writeintro">Overrides the class' writeintro property (if true then introductory text is
        /// printed, regardless of the value of the property). If the introductory text is printed then the standard method
        /// is used, taking into account the introtext and programname properties.</param>
        /// <param name="disposewriter">Indicates whether the TextWriter should be disposed when not used any more.
        /// Default is true when specifying a file name.</param>
        /// <returns>True if a new writer has been successfully set and is ready to use, false otherwise.</returns>
        public bool AddTextLogger(string filename, bool append, bool writeintro, bool disposewriter)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                return AddTW(ref Loggers, filename, append, writeintro, disposewriter);
            }
        }



        /// <summary>Removes all text writers from the TextLogger subsystem.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if al text writers were successfully removed, false if there were problems.</returns>
        public bool RemoveTextLoggers()
        {
            bool ret = true;
            lock (lockobj)
            {
                // Remove the default text writer:
                try
                {
                    bool doremove = false, wasremoved = false;
                    if (Logger != null && DisposeLogger)
                        doremove = true;
                    wasremoved = RemoveTextLogger();
                    if (doremove && !wasremoved)
                        ret = false;
                }
                catch { }
                if (Loggers != null)
                {
                    // Remove text writers from the list:
                    int i = Loggers.Count - 1;
                    while (i >= 0)
                    {
                        bool failed = true;
                        try
                        {
                            TWClass tw = Loggers[i];
                            if (tw != null)
                                tw.ClearWriter();
                            failed = false;
                            Loggers.RemoveAt(i);
                        }
                        catch { }
                        if (failed)
                            ret = false;
                        --i;
                    }
                }
            }
            return ret;
        }


        /// <summary>Removes the default text writer from the TextLogger subsystem.
        /// If appropriate, the corresponding txt writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed)</returns>
        public bool RemoveTextLogger()
        {
            bool ret = true;
            lock (lockobj)
            {
                try
                {
                    if (Logger == null)
                        ret = false;
                    else if (DisposeLogger)
                    {
                        ret = false;
                        Logger.Close(); // Close() will call Dispose()
                        Logger = null;
                        ret = true;
                    }
                }
                catch { }
            }
            return ret;
        }


        /// <summary>Removes the first object from Loggers that contains the specified TextWriter.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        public bool RemoveTextLogger(TextWriter writer)
        // $A Igor feb09;
        {
            lock (lockobj)
            {
                return RemoveTW(Loggers, writer);
            }

        }

        /// <summary>Removes the first object from Loggers whose TextWriter has been created form the specified stream.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        public bool RemoveTextLogger(Stream stream)
        // $A Igor feb09;
        {
            lock (lockobj)
            {
                return RemoveTW(Loggers, stream);
            }
        }

        /// <summary>Removes the first object from Loggers whose TextWriter has been created form the file with the specified name.
        /// If appropriate, the corresponding text writer is closed.</summary>
        /// <returns>true if the text writer was successfully removed, false otherwise (also if there is no writer installed
        /// corresponding the argument)</returns>
        public bool RemoveTextLogger(string filename)
        // $A Igor feb09;
        {
            lock (lockobj)
            {
                return RemoveTW(Loggers, filename);
            }
        }


#region TextLogger_Custom_Operations

        // This provides support for performing custom operations on TextLogger's output streams. 
        // These are operations taht are not in the scope of commonly supported by teh Repoirter, and
        // enable, for example, to write custom text at specific points.

        /// <summary>Returns a list of all text writers that are currently used by the reporter's text logger.
        /// Only text writers that are actually writable are included.</summary>
        /// <returns>List of text writers that are currently used by the reporter's text logger
        /// (only those that are actually writable are included).</returns>
        public List<TextWriter> TextLoggerWriters()
        // $A Igor Feb09;
        {
            return TextLoggerWriters(true);
        }

        /// <summary>Returns a list of all text writers that are currently used by the reporter's text logger.
        /// Warning: Beware of thread safety issues! 
        /// Blocks of code where the returned list is used should be enclosed in lock(reporter.lockobj){...} block
        /// (where reporter is the object through which this method was called).</summary>
        /// <param name="writableonly">If true then only those text writers are listed that are actually writable.
        /// If false then all text writers are listed.</param>
        /// <returns>List of text writers that are currently used by the reporter's text logger.</returns>
        public List<TextWriter> TextLoggerWriters(bool writableonly)
        // $A Igor Feb09;
        {
            lock (lockobj)
            {
                List<TextWriter> ret = new List<TextWriter>();
                try
                {
                    TextWriter tw = Logger;
                    if (tw != null)
                    {
                        if (!writableonly)
                            ret.Add(tw);
                        else
                        {
                            try
                            {
                                tw.Write("");
                                ret.Add(tw); // add writer to the list only if test operation does not throw an exception.
                            }
                            catch { }
                        }
                    }
                    for (int i = 0; i < Loggers.Count; ++i)
                    {
                        try
                        {
                            tw = Loggers[i].Writer;
                            if (!writableonly)
                                ret.Add(tw);
                            else
                            {
                                tw.Write("");
                                ret.Add(tw); // add writer to the list only if test operation does not throw an exception.
                            }
                        }
                        catch { }
                    }
                }
                catch { }
                return ret;
            }
        }


        /// <summary>Returns the current number of text writers used by the reporter's text logging module.
        /// Only text writers that are actually writable are counted.</summary>
        /// <returns>The current number of text writers used by the reporter's text logging module 
        /// (only those that are actually writable are counted).</returns>
        public int TextLoggerNumWriters()
        // $A Igor Feb09;
        {
            return TextLoggerNumWriters(true);
        }

        /// <summary>Returns the current number of TextWriters used by the reporter's text logging module.</summary>
        /// <param name="writableonly">If true then only those text writers are counted that are actually writable.
        /// If false then all text writers are returned.</param>
        /// <returns>The current number of text writers used by the reporter's text logging module.</returns>
        public int TextLoggerNumWriters(bool writableonly)
        // $A Igor Feb09;
        {
            int ret = 0;
            lock (lockobj)
            {
                try
                {
                    TextWriter tw = Logger;
                    if (tw != null)
                    {
                        if (!writableonly)
                            ++ret;
                        else
                        {
                            try
                            {
                                tw.Write("");
                                ++ret; // count this writer only if test operation does not throw an exception
                            }
                            catch { }
                        }
                    }
                    for (int i = 0; i < Loggers.Count; ++i)
                    {
                        try
                        {
                            tw = Loggers[i].Writer;
                            if (!writableonly)
                                ++ret;
                            else
                            {
                                tw.Write("");
                                ++ret; // count this writer only if test operation does not throw an exception
                            }
                        }
                        catch { }
                    }
                }
                catch { }
                return ret;
            }
        }


        /// <summary>Flushes all text writers of the Writer's TextLogger subsystem.</summary>
        /// <returns>Number of writers that has actually been flushed.</returns>
        public int TextLoggerFlush()
        // $A Igor Feb09;
        {
            int ret = 0;
            lock (lockobj)
            {
                List<TextWriter> writers = TextLoggerWriters();
                bool doflushing = TextLoggerFlushing;
                if (writers != null)
                    for (int i = 0; i < writers.Count; ++i)
                    {
                        try
                        {
                            TextWriter writer = writers[i];
                            if (writer != null)
                            {
                                writer.Flush();
                                ++ret;
                            }
                        }
                        catch { }
                    }
            }
            return ret;
        }

        /// <summary>Writes a string to all text writers of the Writer's TextLogger subsystem.</summary>
        /// <param name="str">String to be written.</param>
        /// <returns>Number of writers that the string has actually been written to.</returns>
        public int TextLoggerWrite(string str)
        // $A Igor Feb09;
        {
            lock (lockobj)
            {
                int numwritten = 0, numerrors = 0;
                bool doflushing = TextLoggerFlushing;
                try
                {
                    // Write to the basic output stream:
                    if (Logger != null)
                    {
                        ++numerrors;
                        Logger.Write(str);
                        if (doflushing)
                            Logger.Flush();
                        --numerrors;
                        ++numwritten;
                    }
                }
                catch { }
                try
                {
                    if (Loggers != null)
                        for (int i = 0; i < Loggers.Count; ++i)
                        {
                            try
                            {
                                TextWriter writer = Loggers[i].Writer;
                                if (writer != null)
                                {
                                    ++numerrors;
                                    writer.Write(str);
                                    if (doflushing)
                                        writer.Flush();
                                    --numerrors;
                                    ++numwritten;
                                }
                            }
                            catch { }
                        }
                }
                catch { }
                if (numerrors > 0)
                    throw new Exception("TextLogger: Writing to " + numerrors.ToString() + " output streams failed.");
                return numwritten;
            }  // lock
        }


        /// <summary>Similar to TextLoggerWrite(), except that a newline is added at the end of the string.</summary>
        public int TextLoggerWriteLine(string str)
        // $A Igor Feb09;
        {
            return TextLoggerWrite(str + Environment.NewLine);
        }

#endregion  // TextLogger_Custom_Operations

        // Delegates with default values:

        /// <summary>Delegate that performs reporting (actually logging) via text writer.</summary>
        public ReportDelegate ReportDlgTextLogger = new ReportDelegate(DefaultReport_TextLogger);

        /// <summary>Delegate that assembles the location string for reporting via text logger.</summary>
        public ReportLocationDelegate ReportLocationDlgTextLogger = new ReportLocationDelegate(DefaultReportLocation_TextLogger);

        /// <summary>Delegate that assembles the message string for reporting via text writer.</summary>
        public ReportMessageDelegate ReportMessageDlgTextLogger = new ReportMessageDelegate(DefaultReportMessage_TextLogger);


        // Default delegate methods for reporting via TextLogger:

        /// <summary>Formats a one-line message for tracing output.</summary>
        /// <param name="depth">Indentation level.</param>
        /// <param name="initialindent">Initial indentation.</param>
        /// <param name="indentincrement">Indentation spacing.</param>
        /// <param name="indentchar">Indentation character.</param>
        /// <param name="type">Type of the message.</param>
        /// <param name="location">String denoting location of the message.</param>
        /// <param name="message">Message to be output.</param>
        /// <param name="source">Indication of message source.</param>
        /// <returns>String taht is output as the one-line message.</returns>
        static public string FormatLogMsgDefault(int depth, int initialindent, int indentincrement, char indentchar,
                    ReportType type, string location, string message, ReportSource source)
        {
            string msg = "";
            int nspaces = initialindent + depth * indentincrement;
            if (!string.IsNullOrEmpty(location))
                location = location.Replace(Environment.NewLine, _newLineReplacement);
            if (!string.IsNullOrEmpty(message))
                message = message.Replace(Environment.NewLine, _newLineReplacement);
            for (int i = 1; i <= nspaces; ++i)
                msg += indentchar;
            switch (type)
            {
                case ReportType.Error:
                    if (string.IsNullOrEmpty(location))
                        msg += "ERROR: ";
                    else
                        msg += "ERROR in " + location + ": ";
                    break;
                case ReportType.Warning:
                    if (string.IsNullOrEmpty(location))
                        msg += "Warning: ";
                    else
                        msg += "Warning in " + location + ": ";
                    break;
                case ReportType.Info:
                    if (string.IsNullOrEmpty(location))
                        msg += "Info: ";
                    else
                        msg += "Info from " + location + ": ";
                    break;
                default:
                    if (string.IsNullOrEmpty(location))
                        msg += "Message: ";
                    else
                        msg += "Message from " + location + ": ";
                    break;
            }
            if (!string.IsNullOrEmpty(message))
                msg += message;
            msg += "  | Source: " + source.ToString();
            return msg;
        }


        /// <summary>Default delegate for launching reports (actually logging reports) via text writer.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_TextLogger(ReporterBase reporter, ReportType messagetype,
            string location, string message)
        // $A Igor Dec08;
        {
            if (reporter == null)
                throw new Exception("The reporter object containing auxiliary data is not specified.");
            // Assemble the string that is written to the streams:
           string msg = FormatLogMsgDefault(reporter.Depth, reporter.TextLoggerIndentInitial, reporter.TextLoggerIndentSpacing, reporter.TextLoggerIndentCharacter,
                    messagetype,
                    location, message, reporter.MsgSource);
            // Write the string to all registered streams and files:
            int numwritten = reporter.TextLoggerWriteLine(msg);
        }


        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_TextLogger(ReporterBase reporter, ReportType messagetype,
                string location, Exception ex)
        // $A Igor Dec08;
        {
            return DefaultLocationString(reporter, messagetype, location, ex);
        }

        /// <summary>Delegate for assembling a report message for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="basicmessage">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Message string that can be used in a report.</returns>
        protected static string DefaultReportMessage_TextLogger(ReporterBase reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        // $A Igor Dec08;
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }



        // Methods for reporting via text writer:

        /// <summary>Launches a report via text logger.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_TextLogger(ReportType messagetype, string location, string message, Exception ex)
        // $A Igor Dec08;
        {
            lock (lockobj)
            {
                string locationstring = "", messagestring = "";
                if (ReportLocationDlgTextLogger != null)
                    locationstring = ReportLocationDlgTextLogger(this, messagetype, location, ex);
                else if (ReportLocationDlg != null)
                    locationstring = ReportLocationDlg(this, messagetype, location, ex);
                else
                {
                    // No delegate for assembling location string:
                    if (!string.IsNullOrEmpty(location))
                        locationstring += location;
                }
                if (ReportMessageDlgTextLogger != null)
                    messagestring = ReportMessageDlgTextLogger(this, messagetype, message, ex);
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
                if (ReportDlgTextLogger != null)
                    ReportDlgTextLogger(this, messagetype, locationstring, messagestring);
                else
                    throw new Exception("Stream reporting (logging) delegate is not specified.");
            }
        }


#endregion   // Reporting_TextLogger





#region Reporting_Trace

        // REPORTING BY Trace:

        // TODO: This part should be refined and tested!

        private bool _UseTrace = false;

        /// <summary>Gets or sets the flag specifying whether reporting using the pado trace is performed or not.</summary>
        public bool UseTrace { get { return _UseTrace; } set { _UseTrace = value; } }

        //Data: 

        /// <summary>Indicates the source of the message (such as client, server, web service, etc.)</summary>
        public ReportSource MsgSource = ReportSource.Unknown;


        // Delegates with default values:

        /// <summary>Delegate that performs reporting (logging) via Pado trace mechanism.
        /// It calls delegates ReportDlg to assemble error location information and ReportMessageDlg to 
        /// assemble error message. Then it uses both to assemble the final decorated error message and launches
        /// it in its own way.</summary>
        public ReportDelegate ReportDlgTrace = new ReportDelegate(DefaultReport_Trace);

        /// <summary>Delegate that assembles the error location string for reporting (logging) via Pado trace mechanism.</summary>
        public ReportLocationDelegate ReportLocationDlgTrace = new ReportLocationDelegate(DefaultReportLocation_Trace);

        /// <summary>Delegate that assembles the eror message string for reporting (logging) via Pado trace mechanism.</summary>
        public ReportMessageDelegate ReportMessageDlgTrace = new ReportMessageDelegate(DefaultReportMessage_Trace);

        // Default delegate methods for reporting via Pado trace mechanism:

        static private string _newLineReplacement = " ... ";

        /// <summary>Formats a one-line message for tracing output.</summary>
        /// <param name="depth">Indentation level.</param>
        /// <param name="initialindent">Initial indent.</param>
        /// <param name="indentincrement">Indentation spacing.</param>
        /// <param name="indentchar">Indentation character.</param>
        /// <param name="type">Type of the message.</param>
        /// <param name="location">String denoting location of the message.</param>
        /// <param name="message">Message to be output.</param>
        /// <param name="source">Indication of message source.</param>
        /// <returns>String taht is output as the one-line message.</returns>
        static public string FormatTraceMsgDefault(int depth, int initialindent, int indentincrement, char indentchar, 
                    ReportType type, string location, string message, ReportSource source)
        {
            string msg = "";
            int nspaces = initialindent + depth * indentincrement;
            if (!string.IsNullOrEmpty(location))
                location = location.Replace(Environment.NewLine, _newLineReplacement);
            if (!string.IsNullOrEmpty(message))
                message = message.Replace(Environment.NewLine, _newLineReplacement);
            for (int i = 1; i <= nspaces; ++i)
                msg += indentchar;
            switch (type)
            {
                case ReportType.Error:
                    if (string.IsNullOrEmpty(location))
                        msg += "ERROR: ";
                    else
                        msg += "ERROR in " + location + ": ";
                    break;
                case ReportType.Warning:
                    if (string.IsNullOrEmpty(location))
                        msg += "Warning: ";
                    else
                        msg += "Warning in " + location + ": ";
                    break;
                case ReportType.Info:
                    if (string.IsNullOrEmpty(location))
                        msg += "Info: ";
                    else
                        msg += "Info from " + location + ": ";
                    break;
                default:
                    if (string.IsNullOrEmpty(location))
                        msg += "Message: ";
                    else
                        msg += "Message from " + location + ": ";
                    break;
            }
            if (!string.IsNullOrEmpty(message))
                msg += message;
            msg += "  | Source: " + source.ToString();
            return msg;
        }

        /// <summary>Delegat for launching a report via Trace.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">Short string desctribing location where report was triggered.</param>
        /// <param name="message">Message of the report.</param>
        protected static void DefaultReport_Trace(ReporterBase reporter, ReportType messagetype,
            string location, string message)
        {
            //ReporterPado rep = reporter as ReporterPado;
            //if (rep == null)
            //    throw new ArgumentException("The reporter argument is not specified or is invalid (possible up-casting failure).");
            Trace.WriteLine(
                FormatTraceMsgDefault(reporter.Depth, reporter.TextLoggerIndentInitial, reporter.TextLoggerIndentSpacing, reporter.TextLoggerIndentCharacter,
                    messagetype,
                    location, message, reporter.MsgSource));
        }

        /// <summary>Delegate for assembling a location string for this kind of report.</summary>
        /// <param name="reporter">Reporter object where additional information can be found.</param>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        /// <returns>Location string that can be used in a report.</returns>
        protected static string DefaultReportLocation_Trace(ReporterBase reporter, ReportType messagetype,
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
        protected static string DefaultReportMessage_Trace(ReporterBase reporter, ReportType messagetype,
                string basicmessage, Exception ex)
        {
            return DefaultMessageString(reporter, messagetype, basicmessage, ex);
        }

        // Methods for reporting (logging) via Trace mechanism:

        /// <summary>Launches a report via Trace.
        /// Report is launched by using special delegates for this kind of reporting.
        /// If the corresponding delegates for error location and message are not specified then general delegates
        /// are used for this purporse, or location and message string are simple assembled by this function.</summary>
        /// <param name="messagetype">Type of the report(Error, Warning, Info...).</param>
        /// <param name="location">User provided location string.</param>
        /// <param name="message">User provided message string.</param>
        /// <param name="ex">Exception that triggered reporting.</param>
        protected virtual void Report_Trace(ReportType messagetype, string location, string message, Exception ex)
        {
            string locationstring = "", messagestring = "";
            if (ReportLocationDlgTrace != null)
                locationstring = ReportLocationDlgTrace(this, messagetype, location, ex);
            else if (ReportLocationDlg != null)
                locationstring = ReportLocationDlg(this, messagetype, location, ex);
            else
            {
                // No delegate for assembling location string:
                if (!string.IsNullOrEmpty(location))
                    locationstring += location;
            }
            if (ReportMessageDlgTrace != null)
                messagestring = ReportMessageDlgTrace(this, messagetype, message, ex);
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
            if (ReportDlgTrace != null)
                ReportDlgTrace(this, messagetype, locationstring, messagestring);
            else
                throw new Exception("Trace form reporting delegate is not specified.");
        }


#endregion  // Reporting_Trace


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
        public static string DefaultReportStringMessageBox(ReporterBase reporter, ReportType messagetype,
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
        public static string DefaultReportStringConsole(ReporterBase reporter, ReportType messagetype,
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
        public static string DefaultReportStringConsoleTimeStamp(ReporterBase reporter, ReportType messagetype,
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
        public static string DefaultReportStringConsoleBas(ReporterBase reporter, ReportType messagetype,
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
        public static void DefaultReportConsole(ReporterBase reporter, ReportType messagetype,
                    string errorlocation, string errormessage)
        // $A Igor Oct08;
        {
            string msg = DefaultReportStringConsoleTimeStamp(reporter, messagetype,
                     errorlocation, errormessage);
            Console.Write(msg);
        }

        /// <summary>Default delegate for assembly of the location string when reporting on consoles.
        /// For parameter descriptions, see ReportMessageDlg.</summary>
        public static string DefaultLocationString(ReporterBase reporter, ReportType messagetype,
                    string location, Exception ex)
        // $A Igor Oct08;
        {
            try
            {
                if (ex == null || string.IsNullOrEmpty(ex.Message))
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
        public static string DefaultMessageString(ReporterBase reporter, ReportType messagetype,
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
        /// The method is considered bulletproof.</summary>
        /// <param name="reporter">Rporter to which message is passed.</param>
        /// <param name="messagetype">Level of the message (Error, Warning,Info, etc.)</param>
        /// <param name="location">Location string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="message">Error message string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="ex">Original exception that was being reported when the error reporting function threw an exception.</param>
        /// <param name="ex1">Exception thrown by the error reporting function.</param>
        public static string DefaultReserveReportMessage(ReporterBase reporter, ReportType messagetype,
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
                        + "ERROR IN THE REPORTING PROCEDURE." + Environment.NewLine;
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

        /// <summary>Default function function for reserve error reporting (called if an exception is thrown in
        /// a reporting function).
        /// Writes a report to the application's standard console (if defined) and Reporter's text writers and 
        /// loggers (when defined).
        /// Writing is unconditional (e.g., even if reporter.UseTextWriter = false or reporter.ReportingLevel = Off,
        /// a message is written to the TextWriter's output streams).</summary>
        /// <param name="reporter">Reporter object whre the method can get additional information.</param>
        /// <param name="messagetype">Level of the message (Error, Warning,Info, etc.)</param>
        /// <param name="location">Location string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="message">Error message string as passed to the error reporting function that has thrown an exception.</param>
        /// <param name="ex">Original exception that was being reported when the error reporting function threw an exception.</param>
        /// <param name="ex1">Exception thrown by the error reporting function.</param>
        public static void DefaultReserveReportError(ReporterBase reporter, ReportType messagetype,
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


        /// <summary>Returns an error string that concisely describes information contained in the specific exception
        /// (including location where exception was thrown, and its message).</summary>
        /// <param name="e">Exception from which information is extracted and represented as string.</param>
        public static string GetErorString(Exception e)
        {
            string errstr = "", functionname = null, filename = null;
            int line = -1, column = -1;
            System.Diagnostics.StackTrace trace = null;
            try
            {
                // Extract info about error location:
                trace = new System.Diagnostics.StackTrace(e, true);
                functionname = trace.GetFrame(0).GetMethod().Name;
                filename = trace.GetFrame(0).GetFileName();
                line = trace.GetFrame(0).GetFileLineNumber();
                column = trace.GetFrame(0).GetFileColumnNumber();
                errstr += "Error in " + functionname +
                    "\n  < " + filename +
                    " (" + line.ToString() +
                    ", " + column.ToString() +
                    ") >: \n\n";
                errstr += e.Message;
            }
            catch { }
            return errstr;
        }



#endregion Error_Auxiliary  // Auxiliary methods, default methods to assign to delegates, etc.


        //#region Reporting_Static

        //// GLOBAL GENERAL REPORTING METHODS (for all kinds of reports):
        //// Since there should be only one global reporter per process, these methods don't need to be defined in
        //// derived classes, but one must ensure that global is set to the right class.

        //// Lock for the global reporter:
        //private static object lockobjglobal = new object();

        ///// <summary>Basic global reporting method (overloaded). Launches an error report, a warning report or other kind of report/message.
        ///// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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
        //        Reporter.ReserveReportError(ReportType.Error, location, message, ex, ex1);
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
        ///// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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
        ///// Supplemental data (such as objects necessary to launch visualize the report or operation flags)
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





}
