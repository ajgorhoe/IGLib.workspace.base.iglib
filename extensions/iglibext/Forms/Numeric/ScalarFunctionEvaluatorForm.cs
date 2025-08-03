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

    /// <summary>Control for test evaluation of scalar functions.
    /// <para>User can define input parameter values and evaluate the function at those parameters.</para></summary>
    /// $A Igor Jul14;
    public partial class ScalarFunctionEvaluatorForm : Form
    {
        public ScalarFunctionEvaluatorForm()
        {
            InitializeComponent();
        }

        /// <summary>Returns this form's main control.</summary>
        public ScalarFunctionEvaluatorControl MainControl
        {
            get { return scalarFunctionEvaluatorControl1; }
        }
        
        /// <summary>Constructs the control, with starting data specified.</summary>
        /// <param name="function">Scalar function.</param>
        public ScalarFunctionEvaluatorForm(IScalarFunction function, string[] parameterNames, double[] parameterValues)
            : this()
        {
            this.ScalarFunction = function;
            this.ParameterNames = parameterNames;
            this.ParameterValues = parameterValues;
        }


        /// <summary>Scalar function that is evaluated by the current control.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IVectorFunction VectorFunction
        {
            get { return scalarFunctionEvaluatorControl1.VectorFunction; }
            set { scalarFunctionEvaluatorControl1.VectorFunction = value; }
        }


        /// <summary>Scalar function that is evaluated by the current control.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IScalarFunction ScalarFunction
        {
            get { return scalarFunctionEvaluatorControl1.ScalarFunction; }
            set { scalarFunctionEvaluatorControl1.ScalarFunction = value; }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string[] ParameterNames
        {
            get { return scalarFunctionEvaluatorControl1.ParameterNames; }
            set
            { scalarFunctionEvaluatorControl1.ParameterNames = value; }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double[] ParameterValues
        {
            get { return scalarFunctionEvaluatorControl1.ParameterValues; }
            set { scalarFunctionEvaluatorControl1.ParameterValues = value; }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NumParameters
        {
            get
            {
                return scalarFunctionEvaluatorControl1.NumParameters;
            }
            set
            {
                scalarFunctionEvaluatorControl1.NumParameters = value;
            }
        }


    }
}
