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
using IG.Forms;

// using IG.Neural;

namespace IG.Forms
{

    /// <summary>Control for editing input parameter values.</summary>
    /// $A Igor Apr13 Mar16;
    public partial class InputParametersControl : UserControl //, INeuralModelContainer, INeuralModel
    {

        ///// <summary>Constructs the control, with ANN-based model specified.</summary>
        ///// <param name="neuralModel">ANN-based model, containing data definitions and trained neural network.</param>
        //public NeuralInputControl(INeuralModel neuralModel): this()
        //{
        //    this.NeuralModel = NeuralModel;
        //}

        public InputParametersControl()
        {
            InitializeComponent();
        }



        #region CommonData


        private InputOutputDataDefiniton _dataDefinition;


        /// <summary>Data about input and output quantities of the manipulated functions or response.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public InputOutputDataDefiniton DataDefinition
        {
            get {
                return _dataDefinition;
            }
            set {
                _dataDefinition = value;
                CopyDataToGrid();
            }
        }


        #endregion CommonData



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
            get {
                if (_colNumValue < 0) _colNumValue = dGridInputParam.Columns[columnValue.Name].Index;
                return _colNumValue;
            }
            //protected set {
            //    _colNumValue = value;
            //}
        }



        /// <summary>Initializes contents of the DataGridView.</summary>
        public void CopyDataToGrid()
        {
            if (DataDefinition != null)
            {
                dGridInputParam.RowCount = 0;
                dGridInputParam.Refresh();
                int numRows = DataDefinition.InputLength;
                for (int i = 0; i < numRows; ++i)
                {
                    InputElementDefinition def = DataDefinition.InputElementList[i];
                    double value = 0;
                    if (def.DefaultValueDefined)
                    {
                        value = def.DefaultValue;
                    }
                    else if (def.BoundsDefined)
                    {
                        value = 0.5 * (def.MinimalValue + def.MaximalValue);
                    }
                    string[] values = new string[3];
                    values[0] = def.ElementIndex.ToString();
                    values[1] = def.Title;
                    values[2] = value.ToString();
                    dGridInputParam.Rows.Add(values);
                }
                dGridInputParam.Refresh();
            }
        }


        /// <summary>Clears contents of all DataGridView cells.</summary>
        public void ClearCells()
        {
            DataGridViewRowCollection rows = dGridInputParam.Rows;
            for (int i = 0; i < DataDefinition.InputLength; ++i)
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

        public void ResetToDefault()
        {
            CopyDataToGrid();
            DataGridViewRowCollection rows = dGridInputParam.Rows;
            for (int i = 0; i < DataDefinition.InputLength; ++i)
            {
                if (i < rows.Count)
                {
                    DataGridViewRow row = rows[i];
                    InputElementDefinition def = DataDefinition.InputElementList[i];
                    double value = 0;
                    if (def.DefaultValueDefined)
                    {
                        value = def.DefaultValue;
                    }
                    else if (def.BoundsDefined)
                    {
                        value = 0.5 * (def.MinimalValue + def.MaximalValue);
                    }
                    row.Cells[ColNumValue].Tag = value;
                }
            }
        }

        public void ResetToCenter()
        {
            CopyDataToGrid();
            DataGridViewRowCollection rows = dGridInputParam.Rows;
            for (int i = 0; i < DataDefinition.InputLength; ++i)
            {
                if (i < rows.Count)
                {
                    DataGridViewRow row = rows[i];
                    InputElementDefinition def = DataDefinition.InputElementList[i];
                    double value = 0;
                    if (def.BoundsDefined)
                    {
                        value = 0.5 * (def.MinimalValue + def.MaximalValue);
                    } else if (def.DefaultValueDefined)
                    {
                        value = def.DefaultValue;
                    }
                    row.Cells[ColNumValue].Tag = value;
                }
            }
        }


        /// <summary>Current number of values, otained form the DataGridView.</summary>
        public int NumValues
        {
            get { return dGridInputParam.RowCount; }
        }

        protected Vector _values;


        /// <summary>Vector of current values of input parameters as defined by the DadaGridView.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IVector Values
        {
            get {
                if (NumValues <= 0)
                    _values = null;
                else
                {
                    _values = new Vector(NumValues);
                    for (int i = 0; i < _values.Length; ++i)
                    {
                        if (i < dGridInputParam.RowCount)
                        {
                            DataGridViewRow row = dGridInputParam.Rows[i];
                            _values[i] = double.Parse(row.Cells[ColNumValue].Value.ToString());
                        }
                    }
                }
                return _values;
            }
            protected set
            {
                if (value == null)
                {
                    if (NumValues > 0)
                        throw new ArgumentException("Vector of values to be set is not specified (null argument).");
                } else
                {
                    if (value.Length != NumValues)
                    {
                        throw new ArgumentException("Vector of values is of wrong dimension, " + value.Length
                            + " instead of " + NumValues + ".");
                    }
                    else
                    {
                        if (_values.Length != value.Length)
                            _values = new Vector(value.Length);
                        Vector.CopyPlain(value, _values);
                        for (int i = 0; i < value.Length; ++i)
                        {
                            if (i<dGridInputParam.RowCount)
                            {
                                DataGridViewRow row = dGridInputParam.Rows[i];
                                row.Cells[ColNumValue].Tag = value[i];
                            }
                        }
                    }
                }
            }
        }


        /// <summary>Sets the values of input parameters on the DataViewGrid to the elements of the specified vector.</summary>
        /// <param name="values">Vectors defining the values to be set.</param>
        public void SetValues(IVector values)
        { this.Values = values; }

        #endregion GridOperation


        public void ShowContextMenuControl(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuControl.Show(new Point(e.X, e.Y));
            }
        }

        private void InputParametersControl_MouseClick(object sender, MouseEventArgs e)
        {
            ShowContextMenuControl(sender, e);
        }

        private void dGridInputParam_MouseClick(object sender, MouseEventArgs e)
        {
            ShowContextMenuControl(sender, e);
        }

        private void btnResetDefault_Click(object sender, EventArgs e)
        {
            ResetToDefault();
        }

        private void btnResetCenter_Click(object sender, EventArgs e)
        {
            ResetToCenter();
        }

        private void resetToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetToDefault();
        }

        private void resetToCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetToCenter();
        }

        private void dGridInputParam_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            btnResetDefault.Enabled = false;
            Util.SleepSeconds(0.1);
            btnResetDefault.Enabled = true;
        }



    }
}
