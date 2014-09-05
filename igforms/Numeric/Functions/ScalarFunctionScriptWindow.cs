// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IG.Lib;
using IG.Num;

namespace IG.Forms
{

    /// <summary>Window for definition of scalar functions by user defined expressions (through script loader).</summary>
    /// $A Igor Jun14;
    public partial class ScalarFunctionScriptWindow : Form
    {
        public ScalarFunctionScriptWindow()
        {
            InitializeComponent();
        }
    }
}
