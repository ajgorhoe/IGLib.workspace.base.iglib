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

    /// <summary>Windows form (<see cref="System.Windows.Forms.Form"/>) that can launch and show arbitrary controls and can create them if necessary.</summary>
    public partial class ControlViewerForm : Form
    {

        /// <summary>Creates a control viewer form with default parameters. Control to be shown can be chosen on the </summary>
        public ControlViewerForm()
        {
            InitializeComponent();
        }

        /// <summary>Creates the control viewer window and assigns the (already created) specified control to be shown in the viewer.</summary>
        /// <param name="launchedControl">Control to be shown in the viewing container of the form.</param>
        public ControlViewerForm(Control launchedControl): this()
        {
            Viewer.DisplayedControl = launchedControl;
        }

        /// <summary>Gets the control within the current form that is responsible for displaying the inspected control.</summary>
        public ControlViewerControl Viewer
        { get { return controlViewerControl1; } }



    }
}
