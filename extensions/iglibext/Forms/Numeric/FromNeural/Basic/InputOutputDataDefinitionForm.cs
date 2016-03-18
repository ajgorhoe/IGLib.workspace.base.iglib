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
    public partial class InputOutputDataDefinitionForm : Form
    {
        public InputOutputDataDefinitionForm()
        {
            InitializeComponent();
            // Two lines below are necessary in order to override eventual mismatches following
            // from setting properties in GUI designer:
            MainControl.InputDataControl.IsInputData = true;
            MainControl.OutputDataControl.IsInputData = false;
        }

        public InputOutputDataDefinitionControl MainControl { get { return inputOutputDataDefinitionCntrol1; } }

    }
}
