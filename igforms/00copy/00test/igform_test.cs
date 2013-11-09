using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Text;
using System.IO;
using System.Xml;

using IG.Forms;





namespace IG.Forms
{

    public class TestIgforms
    {
        [STAThread]
        public static void Main()
        {

            if (0 != 0)
            {
                // Test of error reporting: 
                try { throw (new Exception()); }
                catch (Exception ex) { IGForm.ReportError(ex); }
            }

            //FadeMessage fm = new FadeMessage();
            //fm.ShowThread();

            new FadeMessage("Fade message, 8 s", 8000);
            new FadeMessage("Fade message, 2 s", 2000);
            new FadeMessage("Fade message, 1 s", 1000);

            for (int i=1;i<=0;++i)
            {
                new FadeMessage("Fade message "+i.ToString()+", 8 s",8000,0.5);
            }

            if (0 != 1)
            {
                // Application.Run(new XMLTreeView());
                XMLTreeView tv = new XMLTreeView();
                tv.ShowInTaskbar = true;     // Override the default setting

                tv.ShowDialog();
            }

            //tv.ShowDialog();

        }
    }

} // nmespace IG.Forms




