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

namespace IG.Forms
{


    /// <summary>Control for test evaluation of scalar functions.
    /// <para>User can define input parameter values and evaluate the function at those parameters.</para></summary>
    /// $A Igor Jul14;
    public partial class ScalarFunctionEvaluatorControl : UserControl
    {

        protected internal ScalarFunctionEvaluatorControl()
        {
            InitializeComponent();

            chkImmediateCalculation.Checked = ImmediateCalculation;
            chkTreatAsVector.Checked = TreatScalarAsVectorFunction;
            chkTreatAsVector.Visible = VisibleTreatAsVector;
        }



        /// <summary>Constructs the control, with ANN-based model specified.</summary>
        /// <param name="neuralModel">ANN-based model, containing data definitions and trained neural network.</param>
        public ScalarFunctionEvaluatorControl(IScalarFunction function, string[] parameterNames, double[] parameterValues)
            : this()
        {
            this.ScalarFunction = function;
            this.ParameterNames = parameterNames;
            this.ParameterValues = parameterValues;
        }


        private IScalarFunction _scalarFunction;

        /// <summary>Scalar function that is evaluated by the current control.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IScalarFunction ScalarFunction
        {
            get { return _scalarFunction; }
            set {
                if (value != _scalarFunction)
                { 
                    _scalarFunction = value;
                    if (value != null)
                    {
                        if (TreatScalarAsVectorFunction && value != null)
                        {
                            _vectorFunction = new VectorFunctionFromScalar(new IScalarFunction[] { ScalarFunction });
                        }
                        else
                        {
                            _vectorFunction = null;
                        }
                        if (ImmediateCalculation)
                            CalculateFunctionValue();
                    }
                }
            }
        }


        IVectorFunction _vectorFunction;

        /// <summary>Vector function whose values are calculated.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IVectorFunction VectorFunction
        {
            get { return _vectorFunction; }
            set {
                if (value != _vectorFunction)
                {
                    _vectorFunction = value;
                    if (value != null)
                    {
                        _scalarFunction = null;
                        if (ImmediateCalculation)
                            CalculateFunctionValue();
                    }
                }
            }
        }


        private bool _visibleTreatAsVector = true;

        /// <summary>Whether the check box for treating scalar function as a vector will be visible or not.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool VisibleTreatAsVector
        {
            get { return _visibleTreatAsVector; }
            set
            {
                if (value != _visibleTreatAsVector)
                {
                    _visibleTreatAsVector = value;
                    chkTreatAsVector.Visible = value;
                }
            }
        }


        private bool _treatAsVectorFunction = false;

        /// <summary>If true then scalar function is converted to a vector funciton when set.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool TreatScalarAsVectorFunction
        {
            get { return _treatAsVectorFunction; }
            set {
                if (value != _treatAsVectorFunction)
                {
                    _treatAsVectorFunction = value;
                    if (value)
                    {
                        if (VectorFunction == null && ScalarFunction != null)
                            _vectorFunction = new VectorFunctionFromScalar(new IScalarFunction[] { ScalarFunction });
                    } else
                    {
                        if (VectorFunction != null && ScalarFunction != null)
                            _vectorFunction = null;
                    }
                    if (ImmediateCalculation)
                    {
                        CalculateFunctionValue();
                    }
                    chkTreatAsVector.Checked = value;
                }
            }
        }

        

        protected string _functionDefinition = null;

        /// <summary>Function definition that can be assigned by creator of the window. This will be used when the verification button is pressed.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string FunctionDefinition
        {
            get { return _functionDefinition; }
            set { _functionDefinition = value; }
        }

        private string[] _parameterNames;

        /// <summary>Names of function parameters, shown in data grid.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string[] ParameterNames
        {
            get {
                if (_parameterNames == null)
                {
                    if (_parameterValues != null)
                        SetDefaultParameterNames(_parameterValues.Length);
                }
                return _parameterNames; 
            }
            set
            {
                if (!UtilStr.ArraysEqual(value, _parameterNames))  // value != _parameterNames)
                {
                    if (value != null && _parameterValues != null)
                    {
                        if (value.Length != ParameterValues.Length)
                            ParameterValues = null;
                    }
                    _parameterNames = value;
                    DataToGrid();
                }
            }
        }

        private double[] _parameterValues;

        /// <summary>Valuse of parameters, shown in data grid.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double[] ParameterValues
        {
            get
            {
                if (_parameterValues == null)
                {
                    if (_parameterNames != null)
                        SetDefaultParameterValues(_parameterNames.Length);
                }
                return _parameterValues;
            }
            set
            {
                if (!UtilStr.ArraysEqual(value, _parameterValues))
                {
                    if (value != null && _parameterNames != null)
                        if (value.Length != ParameterNames.Length)
                        {
                            ParameterNames = null;
                        }
                    _parameterValues = value;
                    DataToGrid();
                    txtValue.Text = "";
                }

            }
        }


        /// <summary>Sets parameter names to default.</summary>
        /// <param name="dim">Number of parameters.</param>
        /// <param name="baseName">Optiional base name for parameters.</param>
        protected void SetDefaultParameterNames(int dim, string baseName = "x")
        {
            string[] names = new string[dim];
            for (int i = 0; i < dim; ++i)
            {
                names[i] = baseName + i.ToString();
            }
            ParameterNames = names;
        }

        /// <summary>Sets parameter values to default.</summary>
        /// <param name="dim">Number of parameters.</param>
        /// <param name="defaultValue">Optional default value, if not specified then 0.</param>
        protected void SetDefaultParameterValues(int dim, double defaultValue = 0)
        {
            double[] values = new double[dim];
            for (int i = 0; i < dim; ++i)
            {
                values[i] = defaultValue;
            }
            ParameterValues = values;
            if (ImmediateCalculation)
            {
                CalculateFunctionValue();
            }
        }

        /// <summary>Gets or sets number of parameters.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NumParameters
        {
            get
            {
                if (_parameterNames != null)
                    return _parameterNames.Length;
                if (_parameterValues != null)
                    return _parameterValues.Length;
                return 0;
            }
            set
            {
                bool setNames = false, setVal = false;
                if (_parameterNames == null)
                    setNames = true;
                else if (_parameterNames.Length != value)
                    setNames = true;
                if (_parameterValues == null)
                    setVal = true;
                else if (_parameterValues.Length == 0)
                    setVal = true;
                if (setNames)
                    SetDefaultParameterNames(value);
                if (setVal)
                        SetDefaultParameterValues(value);
            }
        }


        #region GridOperation

        /// <summary>Returns the <see cref="DataGridView"/> control that is used for setting values
        /// of the input parameters.</summary>
        /// <returns></returns>
        public DataGridView GetGridControl()
        {
            return dGridInputParam;
        }

        protected int _colNumValue = 2;

        /// <summary>Number of DataGrid Column that contains the value.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ColNumValue
        {
            get { return _colNumValue; }
            protected set { _colNumValue = value; }
        }


        /// <summary>Initializes contents of the DataGridView.</summary>
        public void InitializeCells()
        {
            dGridInputParam.RowCount = 0;
            dGridInputParam.Refresh();
            int numRows = NumParameters;
            for (int i = 0; i < numRows; ++i)
            {
                string[] values = new string[3];
                values[0] = i.ToString();
                values[1] = ParameterNames[i];
                values[2] = ParameterValues[i].ToString();
                dGridInputParam.Rows.Add(values);
            }
            dGridInputParam.Refresh();
        }

        protected bool _gridEvents = true;

        /// <summary>Copies internal data to grid view.</summary>
        public void DataToGrid()
        {
            try
            {
                _gridEvents = false;
                int numRows = ParameterValues.Length;
                if (dGridInputParam.RowCount != numRows)
                {
                    dGridInputParam.RowCount = 0;
                    dGridInputParam.Refresh();
                }
                for (int i = 0; i < numRows; ++i)
                {
                    string[] values = new string[3];
                    values[0] = i.ToString();
                    values[1] = ParameterNames[i];
                    values[2] = ParameterValues[i].ToString();
                    if (dGridInputParam.RowCount < i + 1)
                        dGridInputParam.Rows.Add(values);
                    else
                    {
                        var row = dGridInputParam.Rows[i];
                        row.SetValues(values);
                    }
                }
                dGridInputParam.Refresh();
            }
            finally
            {
                _gridEvents = true;
            }
        }


        /// <summary>Copies internal data to grid view.</summary>
        public void DataFromGrid()
        {
            int numRows = dGridInputParam.RowCount;
            string[] names = new string[numRows];
            double[] values = new double[numRows];
            for (int i = 0; i < numRows; ++i)
            {
                var row = dGridInputParam.Rows[i];
                names[i] = row.Cells[1].Value.ToString();
                values[i] = UtilStr.ToDouble(row.Cells[2].Value.ToString());
            }
            ParameterValues = values;
            dGridInputParam.Refresh();
        }


        /// <summary>Clears contents of all DataGridView cells.</summary>
        public void ClearCells()
        {
            DataGridViewRowCollection rows = dGridInputParam.Rows;
            for (int i = 0; i < NumParameters; ++i)
            {
                if (i < rows.Count)
                {
                    DataGridViewRow row = rows[i];
                    int numColumns = row.Cells.Count;
                    for (int j = 0; j < numColumns; ++j)
                    {
                        DataGridViewCell cell = row.Cells[j];
                        cell.Value = null;
                    }
                }
            }
            dGridInputParam.Refresh();
        }

        /// <summary>Removes all the data from the DataGridView.</summary>
        public void ClearDataGrid()
        {
            dGridInputParam.RowCount = 0;
            dGridInputParam.Refresh();
        }




        #endregion GridOperation


        /// <summary>Resets cells to default values.</summary>
        public void ResetToDefault()
        {
            InitializeCells();
            DataGridViewRowCollection rows = dGridInputParam.Rows;
            for (int i = 0; i < NumParameters; ++i)
            {
                if (i < rows.Count)
                {
                    DataGridViewRow row = rows[i];
                    double value = 0;
                    row.Cells[ColNumValue].Tag = value;
                }
            }
        }


        public void ShowContextMenuControl(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuControl.Show(new Point(e.X, e.Y));
            }
        }

        private void ScalarFunctionEvaluatorControl_MouseClick(object sender, MouseEventArgs e)
        {
            ShowContextMenuControl(sender, e);
        }

        private void dGridInputParam_MouseClick(object sender, MouseEventArgs e)
        {
            ShowContextMenuControl(sender, e);
        }

        private void btnResetDefault_Click(object sender, EventArgs e)
        {
            SetDefaultParameterValues(NumParameters);
            // ResetToDefault();
        }

        private List<double> _vectorFunctionValues = new List<double>();

        protected void CalculateFunctionValue()
        {
            bool calculated = false;
            DataFromGrid();
            if (ParameterValues != null)
                if (ParameterValues.Length > 0)
                {
                    IVector vec = new Vector(ParameterValues);
                    {
                        if (VectorFunction != null || ScalarFunction != null)
                        {
                            if (VectorFunction != null)
                            {
                                VectorFunction.Value(vec, ref _vectorFunctionValues);
                                Vector val = new Vector(_vectorFunctionValues);
                                txtValue.Text = val.ToString();
                                calculated = true;
                            } else if (ScalarFunction != null)
                            {
                                double val = ScalarFunction.Value(vec);
                                txtValue.Text = val.ToString();
                                calculated = true;
                            }
                        }
                    }
                }
            if (!calculated)
            {
                txtValue.Text = "< Function not defined. >";
                // throw new InvalidOperationException("Function to be evaluated is not defined (null reference).");
            }

        }


        private void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateFunctionValue();
        }


        private bool _immediateCalculation = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ImmediateCalculation
        {
            get { return _immediateCalculation; }
            set
            {
                if (value != _immediateCalculation)
                {
                    _immediateCalculation = value;
                    chkImmediateCalculation.Checked = value;
                }
            }
        }

        private void dGridInputParam_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_gridEvents)
            {
                try
                {
                    btnResetDefault.Enabled = false;
                    DataFromGrid();
                    if (ImmediateCalculation)
                    {
                        CalculateFunctionValue();
                    }
                    Util.SleepSeconds(0.1);
                }
                finally
                {
                    btnResetDefault.Enabled = true;
                }
            }
        }

        private void resetToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDefaultParameterValues(NumParameters);
            ResetToDefault();
        }


        
        /// <summary>Returns a string that contains a summary of the evaluated function.</summary>
        public string CreateFunctionSummary()
        {
            IScalarFunction function = this.ScalarFunction;
            StringBuilder sb = new StringBuilder();
            if (function == null)
                sb.AppendLine("Scalar function is not defined (null reference).");
            else
            {
                sb.AppendLine("Evaluated function: ");
                sb.AppendLine("Type: " + function.GetType().FullName);
                sb.AppendLine("Name: " + function.Name);
                sb.AppendLine("Description: " + function.Description);
                LoadableScalarFunctionBase loadableFunc = function as LoadableScalarFunctionBase;
                if (loadableFunc == null)
                {
                    sb.AppendLine("Function was not compiled from textual definitions.");
                }
                else
                {
                    sb.AppendLine("Compiled function.");
                }
                if (string.IsNullOrEmpty(FunctionDefinition))
                    sb.AppendLine("Definition was not provided.");
                else
                    sb.AppendLine("Definiton: " + Environment.NewLine + FunctionDefinition + Environment.NewLine);
            }
            sb.AppendLine();
            sb.AppendLine("Immediate calculation: " + ImmediateCalculation);
            sb.AppendLine("Treat scalar f. as vector function: " + TreatScalarAsVectorFunction);
            sb.AppendLine("Scalar function defined: " + (ScalarFunction != null));
            sb.AppendLine("Vector function defined: " + (VectorFunction != null));


            return sb.ToString();
        }

        private void btnShowDefinition_Click(object sender, EventArgs e)
        {
            UtilForms.Reporter.ReportInfo(CreateFunctionSummary());
        }

        private void chkImmediateCalculation_CheckedChanged(object sender, EventArgs e)
        {
            ImmediateCalculation = chkImmediateCalculation.Checked;
        }

        private void chkTreatAsVector_CheckedChanged(object sender, EventArgs e)
        {
            TreatScalarAsVectorFunction = chkTreatAsVector.Checked;
        }

        private void btnIdentifyThread_Click(object sender, EventArgs e)
        {
            string threadData = UtilForms.GetCurrentThreadInfo();
            FadingMessage msg = new FadingMessage("Form Thread Information", threadData, 4000, false /* launchImmediately */);
            msg.Launch(false /* inParallelThread */);
        }



    }
}
