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
    public partial class ScalarFunctionEvaluatorWindow : Form
    {
        public ScalarFunctionEvaluatorWindow()
        {
            InitializeComponent();
        }

        
        /// <summary>Constructs the control, with starting data specified.</summary>
        /// <param name="function">Scalar function.</param>
        public ScalarFunctionEvaluatorWindow(IScalarFunction function, string[] parameterNames, double[] parameterValues)
            : this()
        {
            this.Function = function;
            this.ParameterNames = parameterNames;
            this.ParameterValues = parameterValues;
        }



        /// <summary>Scalar function that is evaluated by the current control.</summary>
        public IScalarFunction Function
        {
            get { return scalarFunctionEvaluatorControl1.Function; }
            set { scalarFunctionEvaluatorControl1.Function = value; }
        }


        public string[] ParameterNames
        {
            get { return scalarFunctionEvaluatorControl1.ParameterNames; }
            set
            { scalarFunctionEvaluatorControl1.ParameterNames = value; }
        }

        
        public double[] ParameterValues
        {
            get { return scalarFunctionEvaluatorControl1.ParameterValues; }
            set { scalarFunctionEvaluatorControl1.ParameterValues = value; }
        }


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
