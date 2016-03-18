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
    public partial class VectorFunctionScriptForm : Form 
    {
        public VectorFunctionScriptForm()
        {
            InitializeComponent();

            // IMPORTANT: This will re-set the values set by users in case they were overridden by 
            // setting some other property such as num. parameters or num. functions.
            MainControl.InitialScalarFunctionDefinitionStrings = MainControl.InitialScalarFunctionDefinitionStrings;
            MainControl.SynchronizeData(true, true);
            MainControl.HasUnsavedChanges = false;
            MainControl.HasChildUnsavedChanges = false;
        }
        VectorFunctionScriptControl MainControl
        { get { return vectorFunctionScriptControl1; } }


    }
}
