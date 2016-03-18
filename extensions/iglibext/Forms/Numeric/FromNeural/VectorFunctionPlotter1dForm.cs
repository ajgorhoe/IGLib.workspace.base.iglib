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
    public partial class VectorFunctionPlotter1dForm : Form
    {
        public VectorFunctionPlotter1dForm()
        {
            InitializeComponent();
        }

        public VectorFunctionPlotter1d MainControl
        { get { return neuralParametricTest1; } }

        private void neuralParametricTest1_Load(object sender, EventArgs e)
        {

        }
    }
}
