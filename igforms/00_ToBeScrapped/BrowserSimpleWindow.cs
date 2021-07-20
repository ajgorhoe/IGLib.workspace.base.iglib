using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IG.Forms;

// $$$$Excluded


namespace IG.Forms11
{
    public partial class BrowserSimpleWindow : Form
    {
        public BrowserSimpleWindow()
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
    }
}
