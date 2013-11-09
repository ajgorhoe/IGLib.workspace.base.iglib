using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace IG.Forms
{
	/// <summary>
	/// Summary description for FaidMessage1.
	/// </summary>
    /// 



    public class IGForm : System.Windows.Forms.Form
    {
        /// Common tools for Windows forms


        static int numerrors=0, maxerrors=5;

        private static void basReportError(string errstr)
        {
            ++numerrors;
            if (numerrors <= maxerrors)
            {
                FadeMessage message = new FadeMessage();
                message.BackGroundColor = Color.FromArgb(255, 224, 192);
                message.MsgTitle = "Error";
                message.MsgText = errstr;
                if (numerrors == maxerrors)
                    message.MsgText += "\n\r\n\rFurther error reports of this time will be omitted due to the current settings.\n\r";
                message.ShowTime = 6000;
                message.FadingTimePortion = 0.3;
                message.ShowThread();
            }
        }

        static public void ReportError(string errstr)
        /// Reports an error (including logging, etc., dependent on the current settings).
        {
            basReportError(errstr);
        }

        static public void ReportError(Exception e)
        /// Reports an error (including logging, etc., dependent on the current settings).
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
                basReportError(errstr);
            }
            catch { }
        }


        

        static public void SetBackColorRec(Form win, Color col)
        /// Recursively sets background color for a window form and all its children.
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


        static public void SetForeColorRec(Form win, Color col)
        /// Recursively sets foreground color for a window form and all its children.
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

    }  // class IGForm



}
