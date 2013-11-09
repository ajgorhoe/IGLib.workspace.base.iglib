


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Threading;
using System.Diagnostics;



using IG.ReporterMsg;
using IG.ReporterMsgForms;


namespace TestSqlXml
{
    static class Reporter_Test
    {

        /// <summary>The main entry point for the application.</summary>
        [STAThread]
        static void Main()
        {
            try
            {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ReporterConf(new Reporter()));
            }
            catch (Exception ex)
            {
                ReporterConsoleMsgbox.Global.ReportError(ex);
            }
        } // Main()

   
 
    }
}
