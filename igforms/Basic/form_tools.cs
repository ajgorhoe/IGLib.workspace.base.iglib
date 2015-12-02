// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/



            /*****************************/
            /*                           */
            /*    MISCELLANEOUS TOOLS    */
            /*                           */
            /*****************************/


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using System.Runtime.InteropServices;
using System.Drawing.Printing;
using System.Drawing.Imaging;


namespace IG.Forms
{
	/// <summary>
	/// Summary description for FaidMessage1.
	/// </summary>
    /// 

    delegate void VoidDelegate();  /// Reference to a function without arguments & return value.
    public delegate void FormDelegate(Form f);    // Reference to a function with a Form argument.
    public delegate void ControlDelegate(Control ct);    // Reference to a function with a Control argument.

    

    /// <summary>Various forms utilities.</summary>
    public class UtilForms //: System.Windows.Forms.Form
    {
        /// Common tools for Windows forms

        //
        // Global console:
        //

        private static ConsoleForm
            ApplicationConsole = null, // holds the application console
            auxcons;



        #region APPLICATION_CONSOLE

        //*******************************************
        // Application-messagelevel input and output:
        //*******************************************

        // TODO: Implement also input operations!


        private static object 
            InnerAppConsLock = new object(),
            OuterAppConsLock = new object();



        private static bool ConsoleReady()
        {
            bool ret = false;
            try
            {
                if (InnerAppConsLock == null)
                    throw (new Exception("IGForm.ConsoleReady: Can not acquire the inner application console lock."));
                else
                    lock(InnerAppConsLock)
                    {
                        if (ApplicationConsole != null)
                        {
                            if (!ApplicationConsole.IsDisposed && !ApplicationConsole.Disposing)
                                ret = true;
                            if (!ret)
                            {
                                // Give some time in the case that console is just being created 
                                // in another thread:
                                Thread.Sleep(40);
                                if (ApplicationConsole != null)
                                    if (!ApplicationConsole.IsDisposed && !ApplicationConsole.Disposing)
                                        ret = true;
                            }
                        }
                    }
            }
            catch (Exception e) { UtilForms.ReserveReportError(e); }
            return ret;
        }

        public static ConsoleForm Console
        {
            get
            {
                if (!UtilForms.ConsoleReady())
                {
                    try 
                    {
                        // Try to create the console:
                        if (InnerAppConsLock == null)
                            throw (new Exception("IGForm.ConsoleReady: Can not acquire the inner application console lock."));
                        else
                            lock(InnerAppConsLock)
                            {
                                UtilForms.ApplicationConsole = new ConsoleForm("Application console");
                            }
                    }
                    catch(Exception e)
                    {
                        UtilForms.ReserveReportError(e,"Problem with creating the application console.");
                    }
                }
                if (!ConsoleReady())
                {
                    UtilForms.ReserveReportError("IGForm.Console.get: Applicatin console could not be created.");
                }
                return UtilForms.ApplicationConsole;
            }
            set
            {
                if (ConsoleReady())
                {
                    UtilForms.ReserveReportWarning("IGForm.Console.set: Applicatin console is replaced by another console form.");
                    UtilForms.ApplicationConsole.CloseForm();
                };
                UtilForms.ApplicationConsole = value;
            }
        }


        

        //
        // Re-implementation of common meethods provided by the ConsoleForm class for global calls 
        // not specifying the static Console variable:

        public static void Write(string str)
        {
            try
            {
                auxcons = UtilForms.Console;
                if (UtilForms.ConsoleReady())
                    auxcons.Write(str);
                else
                    UtilForms.ReserveWrite(str);
            }
            catch (Exception ex)
            {
                UtilForms.ReserveReportError(ex);
            }
        }

        public static void Write(bool block, string str)
        { 
            try
            {
                auxcons = UtilForms.Console;
                if (UtilForms.ConsoleReady())
                    auxcons.Write(block,str);
                else
                    UtilForms.ReserveWrite(str);
            }
            catch (Exception ex)
            {
                UtilForms.ReserveReportError(ex);
            }
        }


        public static void WriteLine(string str)
        { 
            try
            {
                auxcons = UtilForms.Console;
                if (UtilForms.ConsoleReady())
                    auxcons.WriteLine(str);
                else
                    UtilForms.ReserveWrite(str);
            }
            catch (Exception ex)
            {
                UtilForms.ReserveReportError(ex);
            }
        }

        public static void WriteLine(bool block,string str)
        { 
            try
            {
                auxcons = UtilForms.Console;
                if (UtilForms.ConsoleReady())
                    auxcons.WriteLine(block, str);
                else
                    UtilForms.ReserveWrite(str);
            }
            catch (Exception ex)
            {
                UtilForms.ReserveReportError(ex);
            }
        }

        //*********************************************
        // Overloaded methods for Write and WriteLine:

        public static void Write(Char arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(Boolean arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(Char[] arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }
        }

        public static void Write(Decimal arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }
        }

        public static void Write(Double arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(Int32 arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(Int64 arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(Object arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(Single arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(UInt32 arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(UInt64 arg) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); Write(sw.ToString()); }  }

        public static void Write(String arg, Object arg1) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1); Write(sw.ToString()); }  }

        public static void Write(String arg, Object[] arg1) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1); Write(sw.ToString()); }  }

        public static void Write(Char[] arg, Int32 arg1, Int32 arg2) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1, arg2); Write(sw.ToString()); }  }

        public static void Write(String arg, Object arg1, Object arg2) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1, arg2); Write(sw.ToString()); }  }

        public static void Write(String arg, Object arg1, Object arg2, Object arg3) {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1, arg2, arg3); Write(sw.ToString()); }  }

        // Overloaded methods for WriteLine:

        public static void WriteLine(Char arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Boolean arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Char[] arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Decimal arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Double arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Int32 arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Int64 arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Object arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Single arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(UInt32 arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(UInt64 arg)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(String arg, Object arg1)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(String arg, Object[] arg1)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(Char[] arg, Int32 arg1, Int32 arg2)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1, arg2); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(String arg, Object arg1, Object arg2)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1, arg2); WriteLine(sw.ToString()); }
        }

        public static void WriteLine(String arg, Object arg1, Object arg2, Object arg3)
        {
            using (StringWriter sw = new StringWriter())
            { sw.Write(arg, arg1, arg2, arg3); WriteLine(sw.ToString()); }
        }




        //*************************************
        // Reserve application-messagelevel input and output:
        //*************************************

        static int NumResWrErr = 0, MaxResWrErr = 4;

        private static void ReserveWrite(string str)
        // Reserve Write replacement, tries to output in an alternative way.
        {
            try
            {
                FadingMessage fm = new FadingMessage();
                fm.MsgTitle = "Application console replacement";
                fm.MsgText = str;
                fm.BackGroundColor = Color.White;
                fm.ShowThread(str, 6000);
            }
            catch (Exception ex)
            {
                ++NumResWrErr;
                if (NumResWrErr<MaxResWrErr)
                    UtilForms.ReserveReportError(ex);
            }
            try
            {
                Console.Write(str);
            }
            catch (Exception ex1)
            {
                ++NumResWrErr;
                if (NumResWrErr < MaxResWrErr)
                    UtilForms.ReserveReportError(ex1);
            }
        }

        private static void ReserveWriteLine(string str)
        // Reserve WriteLine replacement, tries to output in an alternative way.
        {
            ReserveWrite(str + System.Environment.NewLine);
        }


        #endregion    // APPLICATION_CONSOLE


        #region ERROR_REPORTING


        //******************
        // Error reporting:
        //******************

        


        static public void ReportError(string errstr)
        /// Reports an error (including logging, etc., dependent on the current settings).
        {
            try
            {
                ConsoleForm globalconsole = null;
                globalconsole = UtilForms.Console;  // Create a new console if not yet created
                if ( UtilForms.ConsoleReady() )
                {
                    globalconsole.ReportError0(errstr);
                } else
                {
                    UtilForms.ReserveReportError(errstr);
                }
            }
            catch (Exception e)
            {
                UtilForms.ReserveReportError(e,"\nIn IGForm.ReportError; ERROR to be reported:\n"
                    +errstr);
            }
        }


        static public string GetErorString(Exception e)
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

        static public void ReportError(Exception e)
        /// Reports an error (including logging, etc., dependent on the current settings).
        {
            try {  ReportError(GetErorString(e)); }
            catch (Exception e1)
            { 
                UtilForms.ReserveReportError(e,"\nAdditional error in IGForm.ReportError:\n"+e1.Message); 
            }
        }

        static public void ReportError(Exception e, string additional)
        /// Reports an error (including logging, etc., dependent on the current settings).
        /// An additonal string is appended to error report.
        {
            string errstr = null;
            try {
                errstr = GetErorString(e) + System.Environment.NewLine
                    + additional
                    + System.Environment.NewLine;
                ReportError( errstr ); 
            }
            catch (Exception ex) 
            {
                try
                {
                    ReserveReportError(e, additional+
                        "\nAdditional error in IGForm.ReportError:\n" + ex.Message);
                } 
                catch { }
            }
        }

        //TODO: Implement reporting of warnings (now it'result the same as reporting erorrs)
        // This should first be done i the ConsoleForm class and then here.
        
        static public void ReporWarning(Exception e)
        {
            ReportError(e);
        }

        static public void ReportWarning(string errstr)
        {
            ReportError(errstr);
        }

        static public void ReportWarning(Exception e, string additional)
        {
            ReportError(e, additional);
        }

        //***********************************
        // Reserve error reporting functions
        //***********************************

        //Used when the application console can not be initialized.

        private static void ReserveReportError(string errorstr)
        // Utility for reporting errors when normal reporting is not possible.
        {
            string str = null;
            try
            {
                str = System.Environment.NewLine +
                      System.Environment.NewLine +
                      "ERROR:" + System.Environment.NewLine +
                      errorstr +
                      System.Environment.NewLine + System.Environment.NewLine;
            }
            catch (Exception e)
            {
                str = errorstr;
                Exception eee = e;
            }
            try
            {
                FadingMessage fm = new FadingMessage();
                fm.MsgTitle = "Error in ConsoleForm";
                fm.MsgText = str;
                fm.BackGroundColor = Color.Orange;
                fm.ShowThread(str, 8000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n" + str + "\n");
                //Console.WriteLine("Additional error occurred when reporting this error:\d2" + ex.Message
                //    +"\d2\d2");
                Exception ex1 = ex;
            }
        }

        private static void ReserveReportError(Exception e)
        // Reports an error when it can not be reported in the usual way.
        {
            string errstr = null;
            try
            {
                errstr = UtilForms.GetErorString(e);
                UtilForms.ReserveReportError(errstr);
            }
            catch (Exception ee)
            {
                try
                {
                    errstr = e.Message;
                    UtilForms.ReserveReportError(errstr + "\nAdditional error:\n" + ee.Message);
                }
                catch (Exception ex) { Exception eee = ex; }
            }
        }

        private static void ReserveReportError(Exception e, string additional)
        // Reports an error when it can not be reported in the usual way.
        {
            string errstr = null;
            try
            {
                errstr = UtilForms.GetErorString(e) + System.Environment.NewLine
                    + additional
                    + System.Environment.NewLine;
                UtilForms.ReserveReportError(errstr);
            }
            catch (Exception ee)
            {
                try
                {
                    errstr = e.Message + "\n" + additional + "\nAdditional error:\n" + ee.Message;
                    UtilForms.ReserveReportError(errstr);
                }
                catch (Exception ex) { UtilForms.ReserveReportError(ex); }

            }
        }

        //**************************
        // Reserve warning reports:
        //**************************


        private static void ReserveReportWarning(string Warningstr)
        // Utility for reporting warnings when normal reporting is not possible.
        {
            string str = null;
            try
            {
                str = System.Environment.NewLine +
                      System.Environment.NewLine +
                      "Warning:" + System.Environment.NewLine +
                      Warningstr +
                      System.Environment.NewLine + System.Environment.NewLine;
            }
            catch (Exception e)
            {
                str = Warningstr;
                Exception eee = e;
            }
            try
            {
                FadingMessage fm = new FadingMessage();
                fm.MsgTitle = "Warning in ConsoleForm";
                fm.MsgText = str;
                // fm.BackGroundColor = Color.Orange;
                fm.ShowThread(str, 4000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n" + str + "\n");
                Exception ex1 = ex;
            }
        }

        private static void ReserveReportWarning(Exception e)
        // Reports a warning when it can not be reported in the usual way.
        {
            string errstr = null;
            try
            {
                errstr = UtilForms.GetErorString(e);
                UtilForms.ReserveReportWarning(errstr);
            }
            catch (Exception ee)
            {
                try
                {
                    errstr = e.Message;
                    UtilForms.ReserveReportWarning(errstr + "\nAdditional error:\n" + ee.Message);
                }
                catch (Exception ex) { Exception eee = ex; }
            }
        }

        private static void ReserveReportWarning(Exception e, string additional)
        // Reports a warning when it can not be reported in the usual way.
        {
            string errstr = null;
            try
            {
                errstr = UtilForms.GetErorString(e) + System.Environment.NewLine
                    + additional
                    + System.Environment.NewLine;
                UtilForms.ReserveReportWarning(errstr);
            }
            catch (Exception ee)
            {
                try
                {
                    errstr = e.Message + "\n" + additional + "\nAdditional error:\n" + ee.Message;
                    UtilForms.ReserveReportWarning(errstr);
                }
                catch (Exception ex) { UtilForms.ReserveReportWarning(ex); }

            }
        }

        #endregion   // ERROR_REPORTING


        #region EVENTS


        /// <summary>Sends the specified keystorkes to the specified control.
        /// <para>Examples: "A", "B", "F1", "{ENTER}", "{DELETE}", "{END}", </para>
        /// <para>"^AB" - hold Ctrl while A is pressed, then press B </para>
        /// <para>"+(AB)" (hold Shift while A and B are pressed),</para>
        /// <para>"%{ENTER}" (hold Alt while Enter is pressed).</para>
        /// </summary>
        /// <param name="targetControl">Target control to which the keystrokes are sent.</param>
        /// <param name="keyCode">A string specifying keys or sequence thereof to be sent to the control.</param>
        /// <remarks><para>List of codes accepted:</para>
        /// <para> http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx </para></remarks>
        public static void GenerateKeyPress(Control targetControl, string keyCode)
        {
            targetControl.Focus();
            SendKeys.SendWait(keyCode);
        }


        public void GenerateKeyPressTest1(Control targetControl, string keyCode)
        {
            // Control target = this.VtkRenderWindowControl;

            // REFERENCEs: WindowsBase (for Key), PresentationCore (for Keyboard, IInputElement), 

            //System.Windows.Input.Key key = System.Windows.Input.Key.Insert;                    // Key to send
            //System.Windows.IInputElement target = System.Windows.Input.Keyboard.FocusedElement;    // Target element
            //System.Windows.RoutedEvent routedEvent = System.Windows.Input.Keyboard.KeyDownEvent; // Event to send

            //var key = System.Windows.Input.Key.Insert;                    // Key to send
            //var target = System.Windows.Input.Keyboard.FocusedElement;    // Target element
            //var routedEvent = System.Windows.Input.Keyboard.KeyDownEvent; // Event to send


            //target.RaiseEvent(
            //  new KeyEventArgs(
            //    System.Windows.Input.Keyboard.PrimaryDevice,
            //    System.Windows.PresentationSource.FromVisual(target),
            //    0,
            //    key) { RoutedEvent = routedEvent }
            //);


            //// Just a small snippet of code:
            //Control a = null;
            //System.Windows.IInputElement target1 = a as System.Windows.IInputElement;
            //if (a != null)
            //{
            //}


        }

        #endregion EVENTS


        #region GENERAL_UTILITIES


        //********************************
            // General Window Forms Utilities
        //********************************

        #region PrintAndSave

        /// <summary>Prints a control by a printer.</summary>
        /// <param name="frm">Control to be printed.</param>
        public static void PrintControl(Control frm)
        {
            if (frm == null)
                throw new ArgumentException("The form to be printed is not specified (null argument).");
            Bitmap bitmap = new Bitmap(frm.Width, frm.Height);
            frm.DrawToBitmap(bitmap, new Rectangle(0, 0, frm.Width, frm.Height));
            PrintDocument docPrinter = new PrintDocument();
            docPrinter.PrintPage += (sender, eventArgs) => eventArgs.Graphics.DrawImage(bitmap, 100, 100);
            docPrinter.Print();
        }


        /// <summary>Saves the specified control to the specified file in the JPG format (common extension .jpg).
        /// <para>The file is overwritten if it already exists.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        public static void SaveControlJpeg(Control frm, string filePath)
        {
            SaveControl(frm, filePath, ImageFormat.Jpeg /* format */, true /* canOverwriteExistent */);
        }

        /// <summary>Saves the specified control to the specified file in the JPG format (common extension .jpg).
        /// <para><paramref name="canOverwriteExistent"/> specifies whether existent files can be overritten.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        /// <param name="canOverwriteExistent">Whether the method can override existent files.
        /// <para>If false and <paramref name="inputFilePath"/> specifies an existent file then exception is thrown.</para></param>
        public static void SaveControlJpeg(Control frm, string filePath, bool canOverwriteExistent)
        {
            SaveControl(frm, filePath, ImageFormat.Jpeg /* format */, canOverwriteExistent);
        }

        /// <summary>Saves the specified control to the specified file in the GIF format (common extension .gif).
        /// <para>The file is overwritten if it already exists.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        public static void SaveControlGif(Control frm, string filePath)
        {
            SaveControl(frm, filePath, ImageFormat.Gif /* format */, true /* canOverwriteExistent */);
        }

        /// <summary>Saves the specified control to the specified file in the GIF format (common extension .gif).
        /// <para><paramref name="canOverwriteExistent"/> specifies whether existent files can be overritten.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        /// <param name="canOverwriteExistent">Whether the method can override existent files.
        /// <para>If false and <paramref name="inputFilePath"/> specifies an existent file then exception is thrown.</para></param>
        public static void SaveControlGif(Control frm, string filePath, bool canOverwriteExistent)
        {
            SaveControl(frm, filePath, ImageFormat.Gif /* format */, canOverwriteExistent);
        }


        /// <summary>Saves the specified control to the specified file in the BMP format (common extension .bmp).
        /// <para>The file is overwritten if it already exists.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        public static void SaveControlBmp(Control frm, string filePath)
        {
            SaveControl(frm, filePath, ImageFormat.Bmp /* format */, true /* canOverwriteExistent */);
        }

        /// <summary>Saves the specified control to the specified file in the BMP format (common extension .bmp).
        /// <para><paramref name="canOverwriteExistent"/> specifies whether existent files can be overritten.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        /// <param name="canOverwriteExistent">Whether the method can override existent files.
        /// <para>If false and <paramref name="inputFilePath"/> specifies an existent file then exception is thrown.</para></param>
        public static void SaveControlBmp(Control frm, string filePath, bool canOverwriteExistent)
        {
            SaveControl(frm, filePath, ImageFormat.Bmp /* format */, canOverwriteExistent);
        }
        
        /// <summary>Saves the specified control to the specified file in a specified bitmap format.
        /// <para>The file is overwritten if it already exists.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        /// <param name="format">Image format.</param>
        public static void SaveControl(Control frm, string filePath, ImageFormat format)
        {
            SaveControl(frm, filePath, format, true /* canOverwriteExistent */);
        }

        /// <summary>Saves the specified control to the specified file in a specified bitmap format.
        /// <para><paramref name="canOverwriteExistent"/> specifies whether existent files can be overritten.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        /// <param name="format">Image format.</param>
        /// <param name="canOverwriteExistent">Whether the method can override existent files.
        /// <para>If false and <paramref name="inputFilePath"/> specifies an existent file then exception is thrown.</para></param>
        public static void SaveControl(Control frm, string filePath, ImageFormat format, bool canOverwriteExistent)
        {
            Bitmap bitmap = new Bitmap(frm.Width, frm.Height);
            frm.DrawToBitmap(bitmap, new Rectangle(0, 0, frm.Width, frm.Height));
            if (!canOverwriteExistent && File.Exists(filePath))
            {
                throw new InvalidOleVariantTypeException("Can not save the image, file already exists. "
                    + Environment.NewLine + "  File path: " + filePath + " ");
            }
            else
            {
                bitmap.Save(filePath, format);
            }
        }



        // SAVE WITH FILE DIALOG:

        
        /// <summary>Saves the specified control to a file in the JPG format (common extension .jpg). 
        /// The file is chosen by the user via a file dialog that is launched.
        /// <para>The file is overwritten if it already exists.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="initialDirectoryPath">Initial directory opened in file dialog. Set to current directory if not specified.</param>
        /// <returns>Path to the file to which the control image was saved, or null if it was not saved.</returns>
        public static string SaveControlFileDialogJpeg(Control frm, string initialDirectoryPath)
        {
            return SaveControlFileDialog(frm, initialDirectoryPath, ".jpg" /* extension */, ImageFormat.Jpeg /* format */);
        }

        /// <summary>Saves the specified control to the specified file in the JPEG format (common extension .jpg).
        /// The file is chosen by the user via a file dialog that is launched.
        /// <para><paramref name="canOverwriteExistent"/> specifies whether existent files can be overritten.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="initialDirectoryPath">Initial directory opened in file dialog. Set to current directory if not specified.</param>
        /// <param name="canOverwriteExistent">Whether the method can override existent files.
        /// <para>If false and an existent file is selected then exception is thrown.</para></param>
        /// <returns>Path to the file to which the control image was saved, or null if it was not saved.</returns>
        public static string SaveControlFileDialogJpeg(Control frm, string initialDirectoryPath, bool canOverwriteExistent)
        {
            return SaveControlFileDialog(frm, initialDirectoryPath, ".jpg" /* extension */, ImageFormat.Jpeg /* format */, canOverwriteExistent);
        }


        /// <summary>Saves the specified control to a file in the GIF format (common extension .gif). 
        /// The file is chosen by the user via a file dialog that is launched.
        /// <para>The file is overwritten if it already exists.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="initialDirectoryPath">Initial directory opened in file dialog. Set to current directory if not specified.</param>
        /// <returns>Path to the file to which the control image was saved, or null if it was not saved.</returns>
        public static string SaveControlFileDialogGif(Control frm, string initialDirectoryPath)
        {
            return SaveControlFileDialog(frm, initialDirectoryPath, ".gif" /* extension */, ImageFormat.Gif /* format */);
        }

        /// <summary>Saves the specified control to the specified file in the GIF format (common extension .gif).
        /// The file is chosen by the user via a file dialog that is launched.
        /// <para><paramref name="canOverwriteExistent"/> specifies whether existent files can be overritten.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="initialDirectoryPath">Initial directory opened in file dialog. Set to current directory if not specified.</param>
        /// <param name="canOverwriteExistent">Whether the method can override existent files.
        /// <para>If false and an existent file is selected then exception is thrown.</para></param>
        /// <returns>Path to the file to which the control image was saved, or null if it was not saved.</returns>
        public static string SaveControlFileDialogGif(Control frm, string initialDirectoryPath, bool canOverwriteExistent)
        {
            return SaveControlFileDialog(frm, initialDirectoryPath, ".gif" /* extension */, ImageFormat.Gif /* format */, canOverwriteExistent);
        }


        /// <summary>Saves the specified control to a file in the BMP format (common extension .bmp). 
        /// The file is chosen by the user via a file dialog that is launched.
        /// <para>The file is overwritten if it already exists.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="initialDirectoryPath">Initial directory opened in file dialog. Set to current directory if not specified.</param>
        /// <returns>Path to the file to which the control image was saved, or null if it was not saved.</returns>
        public static string SaveControlFileDialogBmp(Control frm, string initialDirectoryPath)
        {
            return SaveControlFileDialog(frm, initialDirectoryPath, ".bmp" /* extension */, ImageFormat.Bmp /* format */);
        }

        /// <summary>Saves the specified control to the specified file in the BMP format (common extension .bmp).
        /// The file is chosen by the user via a file dialog that is launched.
        /// <para><paramref name="canOverwriteExistent"/> specifies whether existent files can be overritten.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="initialDirectoryPath">Initial directory opened in file dialog. Set to current directory if not specified.</param>
        /// <param name="canOverwriteExistent">Whether the method can override existent files.
        /// <para>If false and <paramref name="inputFilePath"/> specifies an existent file then exception is thrown.</para></param>
        /// <returns>Path to the file to which the control image was saved, or null if it was not saved.</returns>
        public static string SaveControlFileDialogBmp(Control frm, string initialDirectoryPath, bool canOverwriteExistent)
        {
            return SaveControlFileDialog(frm, initialDirectoryPath, ".bmp" /* extension */, ImageFormat.Bmp /* format */, canOverwriteExistent);
        }


        /// <summary>Saves the specified control to a file. The is chosen by the user via a file dialog that is launched.
        /// <para>The file is overwritten if it already exists.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="initialDirectoryPath">Initial directory opened in file dialog. Set to current directory if not specified.</param>
        /// <param name="extension">File extension (e.g. ".bmp").</param>
        /// <param name="format">Image format.</param>
        /// <returns>Path to the file to which the control image was saved, or null if it was not saved.</returns>
        public static string SaveControlFileDialog(Control frm, string initialDirectoryPath, string extension, ImageFormat format)
        {
            return SaveControlFileDialog(frm, initialDirectoryPath, extension, format, true /* canOverwriteExistent */);
        }

        /// <summary>Saves the specified control's image to the specified file in a specified bitmap format.
        /// <para><paramref name="canOverwriteExistent"/> specifies whether existent files can be overritten.</para></summary>
        /// <param name="frm">Control to be printed.</param>
        /// <param name="initialDirectoryPath">Initial directory opened in file dialog. Set to current directory if not specified.</param>
        /// <param name="extension">File extension (e.g. ".bmp").</param>
        /// <param name="inputFilePath">Path of the file where image is saved.</param>
        /// <param name="format">Image format.</param>
        /// <param name="canOverwriteExistent">Whether the method can override existent files.
        /// <para>If false and <paramref name="inputFilePath"/> specifies an existent file then exception is thrown.</para></param>
        /// <returns>Path to the file to which the control image was saved, or null if it was not saved.</returns>
        public static string SaveControlFileDialog(Control frm, string initialDirectoryPath, string extension, ImageFormat format, bool canOverwriteExistent)
        {
            string filePath = null;
            //if (string.IsNullOrEmpty(initialDirectoryPath))
            //{
            //    initialDirectoryPath = Directory.GetCurrentDirectory();
            //    //initialDirectoryPath = Environment.SpecialFolder.MyDocuments;
            //}
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = initialDirectoryPath;
            saveFileDialog1.Filter = "*" + extension +  "|*.*";
            saveFileDialog1.FilterIndex = 1;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialog1.FileName;
                SaveControl(frm, filePath, format, canOverwriteExistent);
                return filePath;
            } else
            {
                Console.WriteLine(Environment.NewLine + "File for saving control has not been selected." + Environment.NewLine+Environment.NewLine);
            }
            return null;
        }



        #endregion PrintAndSave



        public static void BlinkForm(Control frm)
        // BlinkForm with default number of blinks and blink interval
        {
            BlinkForm(frm, 2, 60);
        }

        public static void BlinkForm(Control frm, int numBlinks, int blinkTimeMs)
        {
            Color normal, hl1, hl2;
            normal = frm.BackColor;
            hl1 = Color.Orange;
            hl2 = Color.White;
            try
            {
                frm.Visible = true;
                for (int i = 1; i <= numBlinks; ++i)
                {
                    frm.BackColor = hl1;
                    frm.Refresh();
                    Thread.Sleep(blinkTimeMs);
                    frm.BackColor = hl2;
                    frm.Refresh();
                    if (i <= numBlinks)
                        Thread.Sleep(100);
                }
                frm.BackColor = normal;
                frm.Refresh();
            }
            catch (Exception e)
            {
                UtilForms.ReportError(e);
            }
            finally
            {
                frm.BackColor = normal;
            }
        }


        public static void RecursiveControlDelegate(Control frm, ControlDelegate fd)
        /// Recursively executes a dlegate of type (vod(Control)) on frm and all its children.
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
                UtilForms.ReportError(e);
            }
        }



        static public void SetBackColorRec(Control win, Color col)
        /// Recursively sets background color for a window consform and all its children.
        {
            try
            {
                foreach (Control child in win.Controls)
                {
                    try{ SetBackColorRec((Form) child, col); }
                    catch {  }
                    try{ child.BackColor = col; } 
                    catch (Exception e) { ReportError(e); }
                }
                win.BackColor = col;
            }
            catch (Exception e)
            {
                ReportError(e);
            }
        }


        static public void SetForeColorRec(Control win, Color col)
        /// Recursively sets foreground color for a window consform and all its children.
        {
            try
            {
                foreach (Control child in win.Controls)
                {
                    try { SetForeColorRec((Form)child, col); }
                    catch  { }
                    try { child.ForeColor = col; }
                    catch (Exception e) { ReportError(e); }
                }
                win.ForeColor = col;
            }
            catch (Exception e)
            {
                ReportError(e);
            }
        }

        #endregion GENERAL_UTILITIES


        #region ICON_UTILITIES
        
                /**********************/
                /*                    */
                /*   ICON UTILITIES   */
                /*                    */
                /**********************/

        public Icon FileIcon(string filePath)
        // Returns an icon that is associated with a file on disk.
        {
            Icon ret = null;
            try
            {
                ret = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            }
            catch(Exception ex)
            {
                UtilForms.ReportError(ex);
            }
            return ret;
        }

        internal class WindowsAPI
        {
            [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr ExtractIcon(IntPtr hInstance,
                                                    string strFileName,
                                                    uint uiIconIndex);
        }

        public Icon DllIcon(string DLLPath, int iconIndex)
        // Returns an icon that is embedded in a dll.
        {
            Icon ret = null;
            try
            {
                // Get process handle:
                IntPtr processHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;
                System.IntPtr oPtr = WindowsAPI.ExtractIcon(processHandle, DLLPath, (uint) iconIndex);
                ret = Icon.FromHandle(oPtr);
            }
            catch(Exception ex)
            {
                UtilForms.ReportError(ex);
            }
            return ret;
        }


        /* System icons that can be used:
 
        SystemIcons.Application
        SystemIcons.Asterisk
        SystemIcons.Error
        SystemIcons.Exclamation
        SystemIcons.Hand
        SystemIcons.Information
        SystemIcons.Question
        SystemIcons.Warning
        SystemIcons.WinLogo
        */

        #endregion   // ICON_UTILITIES


        #region EXAMPLES


        public static void ConsoleExample()
        {

            //try 
            //{
            //    string str = "xy";
            //    while (str.Length > 0 && !ApplicationConsle.IsDisposed )
            //    {
            //        str = ApplicationConsle.ReadLine("Input a line of text: "); 
            //        IGForm.Console.WriteLine(
            //                "\n\nResult of ReadLine: \n  \"" + str + "\"\n\n");

            //        ApplicationConsle.ReadString(ref str,"Input a string: ");
            //        IGForm.Console.WriteLine("\nResult of ReadString: \"" + str + "\"\n");

            //        double dval = ApplicationConsle.ReadString("Input a string: ");
            //        IGForm.Console.WriteLine("\nResult of ReadString: \"" + str + "\"\n");
            //    }
            //}
            //catch (Exception e) { IGForm.ReportError(e, "ERROR in READING testing block."); }


            ConsoleForm globalcons = UtilForms.Console;
            // ConsoleForm globalcons = new ConsoleForm("Application console / false");
            globalcons.WriteLine("\n\nBefore SLEEPING for a LONG TIME.\n\n");

            //IGForm.WriteLine("\n\nBefore SLEEPING for a LONG TIME.\n\n");
            // Thread.Sleep(98000);

            Thread.Sleep(1000);


            //int ch = 'r';
            //while (ch != 'x' && !ApplicationConsle.IsDisposed)
            //{
            //    ch=ApplicationConsle.Read("Insert a character ('x' to exit): ");
            //    IGForm.WriteLine("\n\nValue of a character read from input console: " + ((char) ch).ToString());
            //}
            //ApplicationConsle.WriteLine("Finished reading characters.");


            // Thread.Sleep(5000);


            // cf.ShowThread();
            // cf.ShowDialog();
            // cf.ShowDialog();

            //FadeMessage fm = new FadeMessage();
            //fm.ShowThread();




            // MessageBox.Show("Click to continue.");
        }



        public static void ConsoleExample2()
        {
            for (int i = 1; i <= 20; ++i)
            {
                UtilForms.WriteLine(i.ToString() + ": This is written to global console.");
            }
            Thread.Sleep(2000);

            UtilForms.WriteLine("Test of global console.");
            ConsoleForm appcons = UtilForms.Console;


            // Test of error reporting: 
            try { throw (new Exception()); }
            catch (Exception ex) { UtilForms.ReportError(ex); }

            new FadingMessage("Test message", 4000);

            // $$
            UtilForms.WriteLine("Application Console has beeen used for the first Time.");
            UtilForms.ReportError("Error is reported here for TEST.");
            //Thread.Sleep(5000);
            UtilForms.WriteLine("This is test of output after an error has been reported.");
            //Thread.Sleep(2000);

            string sa = UtilForms.Console.ReadLine("Insert a string!");
        }



        #endregion EXAMPLES



    }  // class IGForm



}
