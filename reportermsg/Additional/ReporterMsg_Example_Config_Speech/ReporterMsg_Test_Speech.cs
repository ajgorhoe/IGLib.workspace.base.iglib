


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Threading;
using System.Diagnostics;



using IG.Lib;
using IG.Forms;


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

                //Reporter R = ReporterConsoleMsgboxSpeech.Global;
                //R.ReportError("This is test error report launched just to test functioning of the reporter.");

                ReporterBase R = new ReporterConsole();
                Application.Run(new ReporterConfSpeech(R));
            }
            catch (Exception ex)
            {
                ReporterConsoleMsgbox.Global.ReportError(ex);
            }
        } // Main()

   
 
    }
}
