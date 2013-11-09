// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace IG.Plot2d
{

    /// <summary>Template for stand-alone graph windows.</summary>
    /// $A Igor Sept09;
    public partial class GraphWindowTemplate : Form
    {
        public GraphWindowTemplate()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
