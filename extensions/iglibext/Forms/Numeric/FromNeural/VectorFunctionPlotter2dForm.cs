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
    public partial class VectorFunctionPlotter2dForm : Form
    {
        public VectorFunctionPlotter2dForm()
        {
            InitializeComponent();
        }

        public VectorFunctionPlotter2d MainControl
        { get { return neuralParametricTest1; } }

        private void neuralParametricTest1_Load(object sender, EventArgs e)
        {

        }
    }
}
