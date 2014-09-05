// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IG.Lib;
using IG.Num;
using IG.Script;

namespace IG.Forms
{

    /// <summary>Control for definition of scalar functions by user defined expressions (through script loader).</summary>
    /// $A Igor Jun14;
    public partial class ScalarFunctionScriptControl : UserControl
    {

        public ScalarFunctionScriptControl()
        {
            InitializeComponent();
            _bgDefined = txtValue.BackColor;
            FunctionName = "f";
            ParameterNames = new string[] { "x1", "x2" };
            ValueDefinition = "x1 * x1 + 2 * x2 * x2";
            GradientDefinitions = new string[] { "2 * x1", "4 * x2 * Pi" };
            IsGradientDefined = true; // set twice just to make sure that dependencies are executed
            IsGradientDefined = false;
            IsLoaderConsistent = false;
        }

        #region Data

        ScalarFunctionLoader _functionLoader;


        /// <summary>Scalar function loader that is responsible for creation of scalar function objects from data.</summary>
        public ScalarFunctionLoader FunctionLoader
        {
            get
            {
                if (_functionLoader == null)
                {
                    _functionLoader = new ScalarFunctionLoader();
                }
                return _functionLoader;
            }
            protected set
            {
                if (value != _functionLoader)
                {
                    IsLoaderConsistent = false;
                    if (value == null)
                        throw new ArgumentException("Scalar function loader can not be null.");
                    _functionLoader = value;
                }
            }
        }


        public LoadableScalarFunctionBase Function
        {
            get
            {
                if (!IsLoaderConsistent)
                {
                    DataToFunctionLoader();
                }
                return FunctionLoader.Function;
            }
        }

        /// <summary>Copies data of the form to function loader.</summary>
        public void DataToFunctionLoader()
        {
            FunctionLoader.IndependentVariableNames = ParameterNames;
            FunctionLoader.ValueDefinitionString = ValueDefinition;
            if (IsGradientDefined)
            {
                FunctionLoader.GradientDefinitionStrings = GradientDefinitions;
            }
            else
            {
                FunctionLoader.GradientDefinitionStrings = null;
            }
            //FunctionLoader.
        }



        /// <summary>Creates scalar function according to user data by the function loader.</summary>
        protected void CreateScalarFunction()
        {
            DataToFunctionLoader();
            IScalarFunction f = Function;
        }


        protected string GetFunctionSignature()
        {
            return FunctionName + " (" + UtilStr.GetParametersStringPlain(ParameterNames) + ") = ";
        }

        /// <summary>Resets function definition to simply "0". Gradients are redefined accordingly.</summary>
        public void ResetFunctionDefinition()
        {
            int numParam = Dimension;
            ValueDefinition = "0";
            string [] grads = new string[numParam];
            for (int i = 0; i < numParam; ++i)
            {
                grads[i] = "0";
            }
            GradientDefinitions = grads;
        }


        private bool _isLoaderConsistent = false;

        public bool IsLoaderConsistent
        {
            get { return _isLoaderConsistent; }
            protected set { _isLoaderConsistent = value; }
        }

        protected string _functionName;



        public string FunctionName
        {
            get { return _functionName; }
            set
            {
                if (value != _functionName)
                {
                    _functionName = value;
                    IsLoaderConsistent = false;
                    if (value != txtName.Text)
                        txtName.Text = value;
                    txtFunctionSignature.Text = GetFunctionSignature();
                }
            }
        }


        /// <summary>Gets or sets function dimension.</summary>
        public int Dimension
        {
            get {
                if (ParameterNames == null)
                    return 0; 
                else
                    return ParameterNames.Length;
            }
            set { 
                int currentDim = 0;
                if (ParameterNames != null)
                    currentDim = ParameterNames.Length;
                if (value != currentDim)
                {
                    if (value < 1)
                        throw new ArgumentException("Number of function parameters should be greater or equal to 1.");
                    string[] names = new string[value];
                    for (int i = 1; i <= value; ++i)
                    {
                        names[i-1] = "x" + i.ToString();
                    }
                    ParameterNames = names;
                    numDimension.Value = value;
                    ResetFunctionDefinition();
                }
            }
        }

        protected string[] _parameterNames;

        /// <summary>Names of function parameters.</summary>
        public string[] ParameterNames
        {
            get { return _parameterNames; }
            set
            {
                if (!UtilStr.StringArraysEqual(value, _parameterNames))
                {
                    IsLoaderConsistent = false;
                    _parameterNames = value;
                    // Update parameter names in a text box if necessary:
                    if (!UtilStr.StringArraysEqual(value, UtilStr.GetParametersArrayPlain(txtParameterNames.Text)))
                        txtParameterNames.Text = UtilStr.GetParametersStringPlain(value);
                    if (value == null)
                        numDimension.Value = 0;
                    else
                        numDimension.Value = value.Length;
                    txtFunctionSignature.Text = GetFunctionSignature();
                    ResetFunctionDefinition();
                }
            }
        }

        protected string _valueDefinition;

        /// <summary>String that defines function value.</summary>
        public string ValueDefinition
        {
            get { return _valueDefinition; }
            set {
                if (value != _valueDefinition)
                {
                    IsLoaderConsistent = false;
                    _valueDefinition = value;
                    if (value != txtValue.Text)
                        txtValue.Text = value;
                }
            }
        }

        protected bool _isGradientDefined = false;

        Color _bgDefined = Color.White;

        Color _bgUndefined = Color.LightGray;

        public bool IsGradientDefined
        {
            get { return _isGradientDefined; }
            set {
                if (value != _isGradientDefined)
                {
                    IsLoaderConsistent = false;
                    _isGradientDefined = value;
                    chkGradients.Checked = value;
                    if (value == true)
                        txtGradients.BackColor = _bgDefined;
                    else
                        txtGradients.BackColor = _bgUndefined;
                }
            }
        }
        


        protected string[] _gradientDefinitions;

        /// <summary>String that defines function value.</summary>
        public string[] GradientDefinitions
        {
            get { return _gradientDefinitions; }
            set
            {
                if (! UtilStr.StringArraysEqual(value, _gradientDefinitions))
                {
                    IsLoaderConsistent = false;
                    _gradientDefinitions = value;
                    txtGradients.Lines = value;
                }
            }
        }


        #endregion Data


        private void btnCreateFunction_Click(object sender, EventArgs e)
        {
            CreateScalarFunction();
        }

        private void btnValueCalculator_Click(object sender, EventArgs e)
        {
            CreateScalarFunction();
            double[] parameterValues = new double[Dimension];
            for (int i=0; i<Dimension; ++i)
            {
                parameterValues[i] = 0;
            }

            if (false && Dimension == 2)
            {
                double[] param = new double[2];
                for (double i=1; i<= 3; ++i)
                    for (int j = 1; j <= 3; ++j)
                    {
                        param[0] = i;
                        param[1] = j;
                        Console.WriteLine(FunctionName + "(" + i + ", " + j + "} =  " + Function.Value(new Vector(param)) );
                    }
            }

            ScalarFunctionEvaluatorWindow win = new ScalarFunctionEvaluatorWindow(this.Function, ParameterNames, parameterValues);
            win.Show();
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            //DataFromGrid()
            StringBuilder sb = new StringBuilder();
            sb.Append(FunctionName).
                Append(" ( ").
                Append(UtilStr.GetParametersStringPlain(ParameterNames)).
                Append(") = ").Append(Environment.NewLine).Append(ValueDefinition);
            FadingMessage fm = new FadingMessage("Scalar function summay", sb.ToString(), 4000, 0.25);
        }



        private void txtParameterNames_Validated(object sender, EventArgs e)
        {
            ParameterNames = UtilStr.GetParametersArrayPlain(txtParameterNames.Text);
        }

        private void txtName_Validated(object sender, EventArgs e)
        {
            FunctionName = txtName.Text;
        }

        private void numDimension_Validated(object sender, EventArgs e)
        {
            Dimension = (int) numDimension.Value;
        }


        private void txtValue_Validated(object sender, EventArgs e)
        {
            ValueDefinition = txtValue.Text;
        }

        private void txtGradients_Validated(object sender, EventArgs e)
        {
            GradientDefinitions = txtGradients.Lines;
        }

        private void chkGradients_CheckedChanged(object sender, EventArgs e)
        {
            IsGradientDefined = chkGradients.Checked;
        }


    }
}
