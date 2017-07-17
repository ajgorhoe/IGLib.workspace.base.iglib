// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

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
    public partial class HashAboutForm : Form
    {
        public HashAboutForm()
        {
            InitializeComponent();
        }

        public HashAboutControl MainControl
        {
            get { return this.hashFormAbout1; }
        }

    }
}
