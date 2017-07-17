// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IG.Forms
{
    public partial class HashAllControl : UserControl
    {
        public HashAllControl()
        {
            InitializeComponent();
        }

        private void tabControl1_KeyUp(object sender, KeyEventArgs e)
        {
            hashControl1.KeyUpHandler(sender, e);
        }

        private void hashControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
