using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IG.Forms
{
    public partial class BrowserSimpleForm : Form
    {
        public BrowserSimpleForm()
        {
            InitializeComponent();
            this.ActiveControl = browserForm;
        }

        private void BrowserSimpleWindow_VisibleChanged(object sender, EventArgs e)
        {
            this.ActiveControl = browserForm;
        }

        #region Operation

        public void OpenLocation(string location)
        {
            browserForm.OpenLocation(location);
        }

        #endregion Operation

        #region Examples


        [STAThread]
        static public void BrowserMain()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BrowserSimpleForm());
        }

        private void MenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }



        public static void Example()
        {
            BrowserSimpleForm.BrowserMain();
        }


        #endregion Examples

    }



}
