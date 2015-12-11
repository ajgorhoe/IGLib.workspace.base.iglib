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

        /// <summary>Constructs the control, with ANN-based model specified.</summary>
        /// <param name="neuralModel">ANN-based model, containing data definitions and trained neural network.</param>
        public ScalarFunctionEvaluatorControl(IScalarFunction function, string[] parameterNames, double[] parameterValues)
            : this()
        {
            this.Function = function;
            this.ParameterNames = parameterNames;
            this.ParameterValues = parameterValues;
        }

        protected internal ScalarFunctionEvaluatorControl()
        {
            InitializeComponent();
        }

        private IScalarFunction _function;

        /// <summary>Scalar function that is evaluated by the current control.</summary>
        public IScalarFunction Function
        {
            get { return _function; }
            set { _function = value; }
        }


        private string[] _parameterNames;

        /// <summary>Names of function parameters, shown in data grid.</summary>
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
        }

        /// <summary>Gets or sets number of parameters.</summary>
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

        protected void CalculateFunctionValue()
        {
            if (Function == null)
            {
                txtValue.Text = "< Function not defined. >";
                // throw new InvalidOperationException("Function to be evaluated is not defined (null reference).");
            }
            else
            {
                DataFromGrid();
                IVector vec = new Vector(ParameterValues);
                double value = Function.Value(vec);
                txtValue.Text = value.ToString();
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            CalculateFunctionValue();
        }


        private void dGridInputParam_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_gridEvents)
            {
                btnResetDefault.Enabled = false;
                DataFromGrid();
                CalculateFunctionValue();
                Util.SleepSeconds(0.1);
                btnResetDefault.Enabled = true;
            }
        }

        private void resetToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDefaultParameterValues(NumParameters);
            ResetToDefault();
        }



    }
}
