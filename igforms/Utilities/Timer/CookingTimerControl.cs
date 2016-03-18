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
    public partial class CookingTimerControl : UserControl
    {
        public CookingTimerControl()
        {
            InitializeComponent();
        }

        private void flowImage_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboTimes_Click(object sender, EventArgs e)
        {
            comboTimes.DroppedDown = true;
        }

        private void comboTimes_Leave(object sender, EventArgs e)
        {
            comboTimes.DroppedDown = false;
        }
    }
}
