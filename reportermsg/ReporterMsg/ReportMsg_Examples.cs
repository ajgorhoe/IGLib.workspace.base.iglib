using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace ReportMsg_Examples
{
    class ReportMsg_Examples
    {
        static void Main(string[] args)
        {
            Console.WriteLine("    ***** Code examples ***** " + Environment.NewLine );


            //// Examples from Five Minutes Tutorial:
            //ExBas5minTutorial();

            // Example that demonstrates logging of messages:
            ExTextLogger();
            //// Example taht demonstrates writing of messages to files:
            //ExTextWriter();

            //// Example that shows how to read reporter settings from the application configuration file:
            //ExAppSettings();

        }


        /// <summary>Introductory examples from 5 minutes tutorial.</summary>
        static void ExBas5minTutorial()
        // $A Igor Feb09;
        {
            //ReporterConsoleMsgbox rep = new ReporterConsoleMsgbox();

            // Create a reporter and an exception, launch an error message:
            Console.WriteLine("Creating a reporter and launching a report of type Error...");
            Exception TestExc = new Exception("Test exception message.");
            ReporterConsoleMsgbox Rep = new ReporterConsoleMsgbox();
            Rep.Report(ReportType.Error, "First location", "Test Error", TestExc);
            
            // Switch off reporting via message box:
            Console.WriteLine("Switching off repoorting through console, launching another report using an overloaded method...");
            Rep.UseConsole = false;
            Rep.Report(ReportType.Error, TestExc);

            // Use an overloading method for specific type:
            Console.WriteLine("Lanunch error reports by overloaded specific methods for error reports...");
            Rep.ReportError(TestExc);
            Rep.ReportError("My location", "User provided part of the message.", TestExc);

            //Increase reporting level, launch an information message:
            Console.WriteLine("Launch an information message at increased reoporting level...");
            Rep.ReportingLevel = ReportLevel.Verbose; // least restrictive, all messages are shown 
            Rep.ReportInfo("Info source", "This is an information message.");
            
            // Decrease reporting level, launch an information message: 
            Console.WriteLine("Launch an information message at decreased reoporting level...");
            Rep.ReportingLevel = ReportLevel.Warning; // more restrictive, only warnings and errors are shown 
            Rep.ReportInfo("Info source", "This is an information message launched after decreasing reporting level.");
            
            // Decrease reporting level to Warning, launch a warning message that has sufficient priority 
            // to be launched at this level: 
            Console.WriteLine("Launch a warning message (sufficient priority) at decreased reoporting level...");
            Rep.ReportWarning("Info source", "This is a warning message launched after decreasing reporting level.");





        }  // ExBas5minTutorial





        /// <summary>Introductory examples from 5 minutes tutorial.</summary>
        static void ExTextLogger()
        // $A Igor Feb09;
        {
            //ReporterConsoleMsgbox rep = new ReporterConsoleMsgbox();

            // Create a reporter and an exception, launch an error message:
            Console.WriteLine("Creating a reporter for text writing and logging ...");
            Exception TestExc = new Exception("Test exception message.");
            ReporterConsoleMsgbox Rep = new ReporterConsoleMsgbox();
            Rep.UseConsole = Rep.UseMessageBox = Rep.UseTrace = Rep.UseTextWriter = Rep.UseTextLogger = false;
            Rep.UseConsole = Rep.UseTextLogger = true;

            // Set indentation properties:
            Rep.TextLoggerIndentInitial = 3;
            Rep.TextLoggerIndentSpacing = 2;
            Rep.TextLoggerIndentCharacter = '>';


            Rep.TextLoggerFlushing = true;

            Rep.ReportingLevel = ReportLevel.Verbose;
            Rep.LoggingLevel = ReportLevel.Verbose;

            Rep.SetTextLogger("..\\..\\files\\logger.txt", false);
            Rep.AddTextLogger("..\\..\\files\\logger1.txt", false);
            Rep.AddTextLogger("..\\..\\files\\logger2.txt", true); // In this file, messages will be appended to the old contents


            Rep.Report(ReportType.Error, "First location", "Test Error", TestExc);
            MessageBox.Show("Right after launching the first message (type \"Error\")..." + Environment.NewLine
                + "Now you can observe changes in output files.");
            Rep.TextLoggerFlush();  // Perform flush after the first message, but not after the second

            // Increase depth and change the leading character:
            Rep.IncreaseDepth(2);
            Rep.TextLoggerIndentCharacter = '*';

            Rep.ReportWarning("Second location", "Test Warning", TestExc);
            //MessageBox.Show("Right after launching the second message (type \"Warning\")...");

            Rep.DecreaseDepth();
            Rep.ReportInfo("Location 3", "Test information message.");


            StreamReader sr = new StreamReader("..\\..\\files\\logger.txt");
            string str = sr.ReadToEnd();

            MessageBox.Show(str);



        }




        /// <summary>Introductory examples from 5 minutes tutorial.</summary>
        static void ExTextWriter()
        // $A Igor Feb09;
        {
            //ReporterConsoleMsgbox rep = new ReporterConsoleMsgbox();

            // Create a reporter and an exception, launch an error message:
            Console.WriteLine("Creating a reporter for logging ...");
            Exception TestExc = new Exception("Test exception message.");
            ReporterConsoleMsgbox Rep = new ReporterConsoleMsgbox();
            Rep.UseConsole = Rep.UseMessageBox = Rep.UseTrace = Rep.UseTextWriter = Rep.UseTextLogger = false;
            Rep.UseConsole = Rep.UseTextWriter = true;

            Rep.TextWriterFlushing = false;

            Rep.ReportingLevel = ReportLevel.Verbose;
            Rep.LoggingLevel = ReportLevel.Verbose;

            Rep.SetTextWriter("..\\..\\files\\writer.txt", false);
            Rep.AddTextWriter("..\\..\\files\\writer1.txt", false);
            Rep.AddTextWriter("..\\..\\files\\writer2.txt", true); // In this file, messages will be appended to the old contents


            Rep.Report(ReportType.Error, "First location", "Test Error", TestExc);

            Rep.ReportWarning("Second location", "Test Warning", TestExc);
            Rep.TextWriterFlush();  // Perform flush after the second message, but not after the last one

            Rep.ReportInfo("Location 3", "Test information message.");

        }
            



        /// <summary>Examples of hoe application settings are read from App.config (see this file !).</summary>
        static void ExAppSettings()
        // $A Igor Feb09;
        {
            // Initialize a global reporter and launch a report; since this is a global reporter, general application
            // settings and settings for the "Global" group of reporters wil both apply:
            Console.WriteLine("Creating a global reporter and launching an error message (general and Global settings are read)...");
            Exception TestExc = new Exception("Test exception message.");
            ReporterConsoleMsgbox Rep = ReporterConsoleMsgbox.Global;
            Rep.Report(ReportType.Error, "First location", "Test Error", TestExc);

            // Apply settings from the configuration group "FirstGroup" and launch an error report again.
            // This will disable message boxes, but enable the text logger:
            Console.WriteLine();
            Console.WriteLine("Read settings from the 'FirstGroup' named group and launch error report...");
            Rep.ReadAppSettings("FirstGroup");
            Rep.ReportError("Second location","Test Error after reading settings of the 'FirstGroup'");

            // Apply settings from the configuration group "FirstGroup" and launch an error report again.
            // This will enable the text writer and speech, and change the leading character of the TextLogger:
            Console.WriteLine();
            Console.WriteLine("Read settings from the 'SecondGroup' named group and launch error report...");
            Rep.ReadAppSettings("SecondGroup");
            Rep.ReportError("Third location","Test Error after reading settings of the 'SecondGroup'");

        }




        void Sandbox()
        {
        }




    }  // Main()



}
