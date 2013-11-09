using IG.ReporterMsg;
using IG.ReporterMsgForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace UnitTests_ReporterMsg {


    /// <summary>
    ///This is a test class for ReporterTest and is intended
    ///to contain all ReporterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReporterTest {

        private TestContext testContextInstance;
        private static string path;
        private static StreamReader sr;
        private static Random random;
        private static int numReportTypes;
        private static int numFirstRepType;

        //variables, needed for each test
        private static string message;
        private static string actualString;
        private static ReportType messagetype;
        private static string errorlocation;
        private static string errormessage;
        private static bool expected;
        private static bool actual;

        private static int numStartedTests;


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) {
            path = Directory.GetCurrentDirectory();
            path = path.Remove(path.LastIndexOf("\\"));
            path = path.Remove(path.LastIndexOf("\\"));

            numReportTypes = Enum.GetNames(typeof(ReportType)).Length + 1;
            numFirstRepType = (int)Enum.GetValues(typeof(ReportType)).GetValue(0);

            numStartedTests = 0;
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize() {
            random = new Random();

            numStartedTests++;

            message = "";
            actualString = "";
            messagetype = ReportType.Undefined;
            errorlocation = "---errorlocation---";
            errormessage = "---errormessage---";
            expected = true;
            actual = true;
        }

        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for DefaultReportStringConsole
        ///</summary>
        [TestMethod()]
        public void DefaultReportStringConsoleTest() {

            ReporterConsole reporter = new ReporterConsole();
            reporter.UseConsole = false;

            for (int i = numFirstRepType; i < numReportTypes; i++) {
                messagetype = (ReportType)i;
                actualString = Reporter.DefaultReportStringConsole(reporter, messagetype, errorlocation, errormessage);

                //check if atttibutes are in the string, depending on the message type
                if ((message = checkActualString(messagetype, errorlocation, errormessage, actualString)).Length != 0) {
                    message += "Error occured during message type = " + messagetype.ToString() + ".\r\n";
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }
            }
            Assert.AreEqual(expected, actual, message);
        }


        //check if atttibutes are in the string, depending on the message type
        private static string checkActualString(ReportType messagetype, string errorlocation, string errormessage, string actualString) {
            string message = "";
            actualString = actualString.ToLowerInvariant();

            if (!actualString.Contains(errorlocation) || !actualString.Contains(errormessage)) {
                message = "Actual string doesn't contain either error location or error message.\r\n";
            }

            switch (messagetype) {
                case ReportType.Error:
                    if (!actualString.Contains("error"))
                        message += "Actual string doesn't contain right report type.\r\n";
                    break;
                case ReportType.Warning:
                    if (!actualString.Contains("warning"))
                        message += "Actual string doesn't contain right report type.\r\n";
                    break;
                default:
                    if (!actualString.Contains("info"))
                        message += "Actual string doesn't contain right report type.\r\n";
                    break;
            }

            return message;
        }

        /// <summary>
        ///A test for DefaultReportStringConsoleBas
        ///</summary>
        [TestMethod()]
        public void DefaultReportStringConsoleBasTest() {

            ReporterConsole reporter = new ReporterConsole();
            reporter.UseConsole = false;


            //random time stamp
            bool timestamp = true;
            if (random.Next(100) < 50)
                timestamp = false;

            for (int i = numFirstRepType; i < numReportTypes; i++) {
                messagetype = (ReportType)i;
                actualString = Reporter.DefaultReportStringConsoleBas(reporter, messagetype, errorlocation, errormessage, timestamp);

                //check if atttibutes are in the string, depending on the message type
                if ((message = checkActualString(messagetype, errorlocation, errormessage, actualString)).Length != 0) {
                    message += "Error occured during message type = " + messagetype.ToString() +
                               " and time stamp = " + timestamp.ToString() + ".\r\n";
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }
            }
            Assert.AreEqual(expected, actual, message);
        }

        /// <summary>
        ///A test for DefaultReportStringConsoleTimeStamp
        ///</summary>
        [TestMethod()]
        public void DefaultReportStringConsoleTimeStampTest() {

            ReporterConsole reporter = new ReporterConsole();
            reporter.UseConsole = false;

            for (int i = numFirstRepType; i < numReportTypes; i++) {
                messagetype = (ReportType)i;
                actualString = Reporter.DefaultReportStringConsoleTimeStamp(reporter, messagetype, errorlocation, errormessage);

                //check if atttibutes are in the string, depending on the message type
                if ((message = checkActualString(messagetype, errorlocation, errormessage, actualString)).Length != 0) {
                    message += "Error occured during message type = " + messagetype.ToString();
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }
            }
            Assert.AreEqual(expected, actual, message);
        }

        /// <summary>
        ///A test for DefaultReportStringMessageBox
        ///</summary>
        [TestMethod()]
        public void DefaultReportStringMessageBoxTest() {

            ReporterConsoleMsgbox reporter = new ReporterConsoleMsgbox();
            reporter.UseConsole = false;
            reporter.UseMessageBox = false;

            for (int i = numFirstRepType; i < numReportTypes; i++) {
                messagetype = (ReportType)i;
                actualString = Reporter.DefaultReportStringMessageBox(reporter, messagetype, errorlocation, errormessage);

                //check if atttibutes are in the string, depending on the message type
                if ((message = checkActualString(messagetype, errorlocation, errormessage, actualString)).Length != 0) {
                    message += "Error occured during message type = " + messagetype.ToString();
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }
            }
            Assert.AreEqual(expected, actual, message);
        }


        /// <summary>
        ///A test for DefaultReserveReportMessage
        ///</summary>
        [TestMethod()]
        public void DefaultReserveReportMessageTest() {

            ReporterConsoleMsgbox reporter = new ReporterConsoleMsgbox();
            reporter.UseConsole = false;
            reporter.UseMessageBox = false;

            try {
                throw new Exception("Test exception 1");
            } catch (Exception ex1) {

                try {
                    throw new Exception("Test exception 2");
                } catch (Exception ex2) {

                    for (int i = numFirstRepType; i < numReportTypes; i++) {
                        messagetype = (ReportType)i;
                        actualString = Reporter.DefaultReserveReportMessage(reporter, messagetype, errorlocation, errormessage, ex1, ex2);

                        //check if atttibutes are in the string, depending on the message type
                        if (!actualString.Contains(errorlocation) || !actualString.Contains(errormessage) ||
                            !actualString.Contains(messagetype.ToString()) || !actualString.Contains(ex1.Message) ||
                            !actualString.Contains(ex2.Message)) {
                            message = "Actual string doesn't contain error location, error message or the right message type.";
                            message += "Error occured during message type = " + messagetype.ToString();
                            actual = false;
                            Assert.AreEqual(expected, actual, message);

                        }
                    }
                    Assert.AreEqual(expected, actual, message);
                }
            }

        }


        /// <summary>
        ///Another test for DefaultReserveReportMessage checking also what happens if reporter throws test exception
        ///</summary>
        [TestMethod()]
        public void DefaultReserveReportMessageTest1() {

            ReporterConsoleMsgbox reporter = new ReporterConsoleMsgbox();
            reporter.UseConsole = false;
            reporter.UseMessageBox = false;
            reporter.UseTrace = false;
            reporter.ThrowTestException = true;

            reporter.ReportingLevel = ReportLevel.Error;
            reporter.UseTextLogger = false;
            reporter.UseTextWriter = true;
            reporter.TextWriterFlushing = false;
            string fileName = path + "//throwTestException.txt";
            reporter.SetTextWriter(fileName, false);

            string testExceptionLocation = "---LocationX---";
            string testExceptionMessage = "---MessageX---";


            for (int i = numFirstRepType; i < numReportTypes; i++) {
                messagetype = (ReportType)i;

                reporter.ReportError(testExceptionLocation, testExceptionMessage);
                reporter.RemoveTextWriters();

                //check if atttibutes are in the file
                sr = new StreamReader(fileName);
                actualString = sr.ReadToEnd();
                sr.Close();

                int ind1, ind2;
                if ((ind1 = actualString.IndexOf(testExceptionMessage)) != -1 && (ind2 = actualString.IndexOf(testExceptionLocation)) != -1) {
                    actualString = actualString.Remove(ind1, testExceptionMessage.Length);
                    actualString = actualString.Remove(ind2, testExceptionLocation.Length);
                }

                //TODO: v reported stringu sploh ni exception location-a
                if (!actualString.Contains(testExceptionLocation) || !actualString.Contains(testExceptionMessage) ||
                    !actualString.Contains(messagetype.ToString()) || !actualString.Contains("TEST INTERNAL EXCEPTION")) {
                    message += "Actual string doesn't contain test exception's error location, error message or error type.\r\n";
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }
            }

        }


        /// <summary>
        ///A test for Report 
        ///</summary>
        [TestMethod()]
        public void ReportTest() {

            //text writer
            Exception exc = new Exception("Test exception message.");
            ReporterConsoleMsgbox reporter = new ReporterConsoleMsgbox();

            String reportedString, reportedString1;

            for (int i = numFirstRepType; i < numReportTypes; i++) {
                messagetype = (ReportType)i;

                setReporter(reporter, messagetype, true);

                reporter.Report(messagetype, errorlocation, errormessage, exc);

                //close all writers
                reporter.RemoveTextWriters();

                //read report.txt & report2.txt, compare them and check if they contain all of the attributes
                string fileName = path + "\\" + numStartedTests.ToString() + "_reporter";
                sr = new StreamReader(fileName + ".txt");
                reportedString = sr.ReadToEnd().ToLower();
                sr.Close();
                sr = new StreamReader(fileName + "2.txt");
                reportedString1 = sr.ReadToEnd().ToLower();
                sr.Close();

                if ((message = checkReport(errorlocation, errormessage, exc, reportedString, reporter)).Length != 0) {
                    actual = false;
                }

                if (reportedString != reportedString1) {
                    actual = false;
                    message += "First and third text file are not equal.\r\n";
                    message += "Error occured during message type = " + messagetype.ToString();
                }

                Assert.AreEqual(expected, actual, message);

            }// for end

            
            //text logger            
            for (int i = numFirstRepType; i < numReportTypes; i++) {
                messagetype = (ReportType)i;

                setReporter(reporter, messagetype, false);

                reporter.Report(messagetype, errorlocation, errormessage, exc);

                //close all text loggers
                reporter.RemoveTextLoggers();

                //read report.txt & report2.txt, compare them and check if they contain all of the attributes
                string fileName = path + "\\" + numStartedTests.ToString() + "_logger";
                sr = new StreamReader(fileName + ".txt");
                reportedString = sr.ReadToEnd();
                sr.Close();
                sr = new StreamReader(fileName + "2.txt");
                reportedString1 = sr.ReadToEnd();
                sr.Close();

                if ((message = checkReport(errorlocation, errormessage, exc, reportedString, reporter)).Length != 0) {
                    actual = false;
                }
                if (reportedString != reportedString1) {
                    actual = false;
                    message += "First and third log file are not equal.\r\n";
                }

                Assert.AreEqual(expected, actual, message);
            }
            Assert.AreEqual(expected, actual, message);

        }


        private string checkExceptionMsg(string actualString, Exception ex) {
            string message = "";

            StackTrace trace = new StackTrace(ex, true);
            string functionname = trace.GetFrame(0).GetMethod().Name;
            string filename = trace.GetFrame(0).GetFileName();
            int line = trace.GetFrame(0).GetFileLineNumber();
            int column = trace.GetFrame(0).GetFileColumnNumber();

            if (!actualString.Contains(functionname))
                message += "Actual string doesn't contain function name in the exception.\r\n";
            if (!actualString.Contains(filename))
                message += "Actual string doesn't contain file name in the exception.\r\n";
            if (!actualString.Contains(line.ToString()))
                message += "Actual string doesn't contain line number in the exception.\r\n";
            if (!actualString.Contains(column.ToString()))
                message += "Actual string doesn't contain column number in the exception.\r\n";


            return message;
        }

        private string checkReport(string errorlocation, string errormessage, Exception exc, string reportedString, ReporterConsoleMsgbox reporter) {
            string message = "";

            reportedString = reportedString.ToLower();

            if (!reportedString.Contains(errorlocation) || !reportedString.Contains(errormessage)) {
                message = "Actual string doesn't contain either error location or error message.\r\n";
            }

            //TODO: exceptione je se treba postimat...
            if (reporter.ReportingLevel.ToString() == "Error" || reporter.ReportingLevel.ToString() == "Warning" ||
                reporter.LoggingLevel.ToString() == "Error" || reporter.LoggingLevel.ToString() == "Warning") {

                //message += checkExceptionMsg(reportedString, exc);
                if (!reportedString.Contains(exc.Message.ToLower())) {
                    actual = false;
                    message += "Reported string doesn't contain exception message.\r\n";
                }

            }

            return message;
        }

        private static void setReporter(ReporterConsoleMsgbox reporter, ReportType messagetype, bool useWriter) {

            ReportLevel repLevel;
            if (messagetype.ToString() == "Error")
                repLevel = ReportLevel.Error;
            else if (messagetype.ToString() == "Warning")
                repLevel = ReportLevel.Warning;
            else if (messagetype.ToString() == "Info")
                repLevel = ReportLevel.Info;
            else
                repLevel = ReportLevel.Verbose;


            reporter.UseConsole = false;
            reporter.UseMessageBox = false;
            reporter.UseTrace = false;

            string fileName = "";

            if (useWriter) {
                reporter.ReportingLevel = repLevel;
                reporter.LoggingLevel = repLevel;
                reporter.UseTextLogger = false;
                reporter.UseTextWriter = true;
                fileName = path + "\\" + numStartedTests.ToString() + "_reporter";
                reporter.TextWriterFlushing = false;
                reporter.SetTextWriter(fileName + ".txt", false);
                reporter.AddTextWriter(fileName + "1.txt", false);
                reporter.AddTextWriter(fileName + "2.txt", false);
            }
            else {
                reporter.LoggingLevel = repLevel;
                reporter.ReportingLevel = repLevel;
                reporter.UseTextLogger = true;
                reporter.UseTextWriter = false;
                fileName = path + "\\" + numStartedTests.ToString() + "_logger";
                reporter.TextLoggerFlushing = false;
                reporter.SetTextLogger(fileName + ".txt", false, false);
                reporter.AddTextLogger(fileName + "1.txt", false, false);
                reporter.AddTextLogger(fileName + "2.txt", false, false);

            }
        }

        /// <summary>
        ///A test for ReportError
        ///</summary>
        [TestMethod()]
        public void ReportErrorTest() {

            //text writer
            Exception exc = new Exception("Test exception message.");
            ReporterConsoleMsgbox reporter = new ReporterConsoleMsgbox();
            messagetype = ReportType.Error;

            String reportedString, reportedString1;

            setReporter(reporter, messagetype, true);

            reporter.ReportError(errorlocation, errormessage, exc);

            //close all writers
            reporter.RemoveTextWriters();

            //read report.txt & report2.txt, compare them and check if they contain all of the attributes
            string fileName = path + "\\" + numStartedTests.ToString() + "_reporter";
            sr = new StreamReader(fileName + ".txt");
            reportedString = sr.ReadToEnd().ToLower();
            sr.Close();
            sr = new StreamReader(fileName + "2.txt");
            reportedString1 = sr.ReadToEnd().ToLower();
            sr.Close();

            if ((message = checkReport(errorlocation, errormessage, exc, reportedString, reporter)).Length != 0) {
                actual = false;
            }

            if (reportedString != reportedString1) {
                actual = false;
                message += "First and third text file are not equal.\r\n";
                message += "Error occured during message type = " + messagetype.ToString() + ".\r\n";
            }
            Assert.AreEqual(expected, actual, message);


            //text logger
            setReporter(reporter, messagetype, false);

            reporter.ReportError(errorlocation, errormessage, exc);

            //close all text loggers
            reporter.RemoveTextLoggers();

            //read report.txt & report2.txt, compare them and check if they contain all of the attributes
            fileName = path + "\\" + numStartedTests.ToString() + "_logger";
            sr = new StreamReader(fileName + ".txt");
            reportedString = sr.ReadToEnd();
            sr.Close();
            sr = new StreamReader(fileName + "2.txt");
            reportedString1 = sr.ReadToEnd();
            sr.Close();

            if ((message = checkReport(errorlocation, errormessage, exc, reportedString, reporter)).Length != 0) {
                actual = false;
            }
            if (reportedString != reportedString1) {
                actual = false;
                message += "First and third log file are not equal.\r\n";
            }

            Assert.AreEqual(expected, actual, message);
        }

        /// <summary>
        ///A test for ReportInfo
        ///</summary>
        [TestMethod()]
        public void ReportInfoTest() {

            //text writer
            ReporterConsoleMsgbox reporter = new ReporterConsoleMsgbox();
            Exception exc = new Exception("Test exception message.");
            messagetype = ReportType.Info;

            String reportedString, reportedString1;

            setReporter(reporter, messagetype, true);

            reporter.ReportInfo(errorlocation, errormessage);

            //close all writers
            reporter.RemoveTextWriters();

            //read report.txt & report2.txt, compare them and check if they contain all of the attributes
            string fileName = path + "\\" + numStartedTests.ToString() + "_reporter";
            sr = new StreamReader(fileName + ".txt");
            //TODO: v reported stringu sploh ni error locationa
            reportedString = sr.ReadToEnd().ToLower();
            sr.Close();
            sr = new StreamReader(fileName + "2.txt");
            reportedString1 = sr.ReadToEnd().ToLower();
            sr.Close();

            if ((message = checkReport(errorlocation, errormessage, exc, reportedString, reporter)).Length != 0) {
                actual = false;
            }

            if (reportedString != reportedString1) {
                actual = false;
                message += "First and third text file are not equal.\r\n";
                message += "Error occured during message type = " + messagetype.ToString() + ".\r\n";
            }
            Assert.AreEqual(expected, actual, message);


            //text logger
            setReporter(reporter, messagetype, false);

            reporter.ReportInfo(errorlocation, errormessage);

            //close all text loggers
            reporter.RemoveTextLoggers();

            //read report.txt & report2.txt, compare them and check if they contain all of the attributes
            fileName = path + "\\" + numStartedTests.ToString() + "_logger";
            sr = new StreamReader(fileName + ".txt");
            reportedString = sr.ReadToEnd();
            sr.Close();
            sr = new StreamReader(fileName + "2.txt");
            reportedString1 = sr.ReadToEnd();
            sr.Close();

            if ((message = checkReport(errorlocation, errormessage, exc, reportedString, reporter)).Length != 0) {
                actual = false;
            }
            if (reportedString != reportedString1) {
                actual = false;
                message += "First and third log file are not equal.\r\n";
            }

            Assert.AreEqual(expected, actual, message);

        }

        /// <summary>
        ///A test for ReportWarning
        ///</summary>
        [TestMethod()]
        public void ReportWarningTest() {

            //text writer
            ReporterConsoleMsgbox reporter = new ReporterConsoleMsgbox();
            Exception exc = new Exception("Test exception message.");
            messagetype = ReportType.Info;

            String reportedString, reportedString1;

            setReporter(reporter, messagetype, true);

            reporter.ReportWarning(errorlocation, errormessage, exc);

            //close all writers
            reporter.RemoveTextWriters();

            //read report.txt & report2.txt, compare them and check if they contain all of the attributes
            string fileName = path + "\\" + numStartedTests.ToString() + "_reporter";
            sr = new StreamReader(fileName + ".txt");
            reportedString = sr.ReadToEnd().ToLower();
            sr.Close();
            sr = new StreamReader(fileName + "2.txt");
            reportedString1 = sr.ReadToEnd().ToLower();
            sr.Close();

            if ((message = checkReport(errorlocation, errormessage, exc, reportedString, reporter)).Length != 0) {
                actual = false;
            }

            if (reportedString != reportedString1) {
                actual = false;
                message += "First and third text file are not equal.\r\n";
                message += "Error occured during message type = " + messagetype.ToString() + ".\r\n";
            }
            Assert.AreEqual(expected, actual, message);


            //text logger
            setReporter(reporter, messagetype, false);

            reporter.ReportWarning(errorlocation, errormessage, exc);

            //close all text loggers
            reporter.RemoveTextLoggers();

            //read report.txt & report2.txt, compare them and check if they contain all of the attributes
            fileName = path + "\\" + numStartedTests.ToString() + "_logger";
            sr = new StreamReader(fileName + ".txt");
            reportedString = sr.ReadToEnd();
            sr.Close();
            sr = new StreamReader(fileName + "2.txt");
            reportedString1 = sr.ReadToEnd();
            sr.Close();

            if ((message = checkReport(errorlocation, errormessage, exc, reportedString, reporter)).Length != 0) {
                actual = false;
            }
            if (reportedString != reportedString1) {
                actual = false;
                message += "First and third log file are not equal.\r\n";
            }

            Assert.AreEqual(expected, actual, message);
        }

        /// <summary>
        ///A test for ReviseException
        ///</summary>
        [TestMethod()]
        public void ReviseExceptionTest() {

            bool actual = true, expected = true;
            Exception actualException;
            string message = "";

            string messageaddition = "---messageaddition---";
            Type newtype = typeof(SystemException);

            try {
                throw new ApplicationException("Test exception 1");
            } catch (ApplicationException ex) {

                // check keepmessage
                bool keepmessage = true;

                bool sametype = true;
                bool oldasinner = true;
                actualException = Reporter.ReviseException(ex, messageaddition, newtype, sametype, keepmessage, oldasinner);
                if((message = checkExceptions(ex, actualException, messageaddition, newtype, sametype, keepmessage, oldasinner)).Length != 0) {
                    actual = false;                
                    Assert.AreEqual(expected, actual, message);
                }

                // check keepmessage
                keepmessage = false;

                sametype = true;
                oldasinner = true;
                actualException = Reporter.ReviseException(ex, messageaddition, newtype, sametype, keepmessage, oldasinner);
                if ((message = checkExceptions(ex, actualException, messageaddition, newtype, sametype, keepmessage, oldasinner)).Length != 0) {
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }

                // check sametype
                sametype = true;

                keepmessage = true;
                oldasinner = true;
                actualException = Reporter.ReviseException(ex, messageaddition, newtype, sametype, keepmessage, oldasinner);
                if ((message = checkExceptions(ex, actualException, messageaddition, newtype, sametype, keepmessage, oldasinner)).Length != 0) {
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }

                // check sametype
                sametype = false;

                keepmessage = true;
                oldasinner = true;
                actualException = Reporter.ReviseException(ex, messageaddition, newtype, sametype, keepmessage, oldasinner);
                if ((message = checkExceptions(ex, actualException, messageaddition, newtype, sametype, keepmessage, oldasinner)).Length != 0) {
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }

                // check oldasinner
                oldasinner = true;

                sametype = true;
                keepmessage = true;
                actualException = Reporter.ReviseException(ex, messageaddition, newtype, sametype, keepmessage, oldasinner);
                if ((message = checkExceptions(ex, actualException, messageaddition, newtype, sametype, keepmessage, oldasinner)).Length != 0) {
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }

                // check oldasinner
                oldasinner = false;

                sametype = true;
                keepmessage = true;
                actualException = Reporter.ReviseException(ex, messageaddition, newtype, sametype, keepmessage, oldasinner);
                if ((message = checkExceptions(ex, actualException, messageaddition, newtype, sametype, keepmessage, oldasinner)).Length != 0) {
                    actual = false;
                    Assert.AreEqual(expected, actual, message);
                }

            }

            Assert.AreEqual(expected, actual, message);

        }

        private string checkExceptions(ApplicationException ex, Exception actualException, string messageaddition, Type newtype, bool sametype, bool keepmessage, bool oldasinner) {
            string message = "";
            
            //check keep message
            if (keepmessage) {
                if (!actualException.Message.Contains(messageaddition) || !actualException.Message.Contains(ex.Message))
                    message = "New exception doesn't contain message addition or original exception's message.\r\n";
            }
            else {
                if (!actualException.Message.Contains(messageaddition))
                    message = "New exception doesn't contain message addition.\r\n";
            }
            
            //check type
            if (sametype) {
                if (!typeof(ApplicationException).Equals(actualException.GetType()))
                    message += "Exceptions are not of the same type.\r\n";
            }
            else {
                if(!newtype.Equals(actualException.GetType()))
                    message += "New exception is not of the specified new type.\r\n";
            }
            
            //check old as inner
            if (oldasinner) {
                if (actualException.InnerException != ex)
                    message += "New exception doesn't contain the old one as its inner exception.\r\n";
            }
            
            return message;
        }
    }
}