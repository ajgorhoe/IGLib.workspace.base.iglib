// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

using IG.Lib;
using IG.Num;
using System.Windows.Forms;

namespace IG.Forms
{

    /// <summary>Control for editing input or output data definitions (only one of these two at a time).
    /// <para>Data definitions are contained in an object of type <see cref="InputOutputDataDefiniton"/>.</para></summary>
    /// $A Igor Mar16;
    public partial class InpuOrOutputtDataDefinitionControl : UserControl
    {

        public InpuOrOutputtDataDefinitionControl()
        {
            GridChangeEventsDisabled = true;
            try
            {
                InitializeComponent();

                // Defines types of data for data grid columns, to prevent invalid cast exceptions:
                // These exceptions can be thrown when value type definitions disappear from the GUI designer generated code.
                InitDataGridProperties();

                grpOuter.Text = Title;

                UpdateInputOrOutput(this.IsInputData);


            }
            finally
            {
                GridChangeEventsDisabled = false;
            }
        }


        #region Data


        private InputOutputDataDefiniton _dataDefinition;

        /// <summary>Data about input and output quantities of the manipulated functions or response.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public InputOutputDataDefiniton DataDefinition
        {
            get {
                return _dataDefinition;
            }
            set {
                if (!object.ReferenceEquals(value, _dataDefinition))
                {
                    this.HasUnsavedChanges = false;
                    InputOutputDataDefiniton previous = _dataDefinition;
                    _dataDefinition = value;
                    SetInputLength(_dataDefinition.InputLength);
                    SetOutputLength(_dataDefinition.OutputLength);
                    CopyDataToGrid();
                    OnDefinitionObjectChanged(previous, _dataDefinition);
                    indicatorLight1.SetOk();
                    this.HasUnsavedChanges = false;
                }
            }
        }


        /// <summary>Number of the corresponding elements in the data definition.
        /// <para>If <see cref="IsInputData"/> then this refers to the number of input elements, otherwise to the number o output elements.</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NumElements
        {
            get
            {
                if (IsInputData)
                    return InputLength;
                else
                    return OutputLength;
            }
            protected set
            {
                SetNumElements(value, false /* internalCall */);
            }
        }

        /// <summary>Sets the number of input or output elements (property <see cref="NumElements"/>), dependent on whether
        /// the current control is for input or output data definition.
        /// <para>Internally, this method should be used instead of the property, because it does not throw on certain situations.</para>
        /// <para>Internally, the method should be called without arguments.</para></summary>
        /// <param name="newValue">Value to be assigned to the property.</param>
        /// <param name="internalCall">Specifies whether the method was called internally. Default is true.</param>
        protected void SetNumElements(int newValue, bool internalCall = true)
        {
            if (IsInputData)
                SetInputLength(newValue, internalCall);
            else
                SetOutputLength(newValue, internalCall); ;
        }

        protected int _inputLength;

        /// <summary>Number of input parameters or data elements.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual int InputLength
        {
            get { return _inputLength; }
            set
            {
                SetInputLength(value, false /* internalCall */);
            }
        }


        /// <summary>Sets the number of input elements (property <see cref="InputLength"/>).
        /// <para>Internally, this method should be used instead of the property, because it does not throw on certain situations.</para>
        /// <para>Internally, the method should be called without arguments.</para></summary>
        /// <remarks>Outside this class, one should assign the property directyl, because that provides additional error checks.
        /// <para>Exception to the rule is in GUI controls and other classes that handle two controls of this kind at the same time,
        /// and setting the property can follow complex flow of ewents, so it is difficult to guarantee the right order of operations
        /// that will not trigger exceptions in various consistency checks.</para></remarks>
        /// <param name="newValue">Value to be assigned to the property.</param>
        /// <param name="internalCall">Specifies whetherr the method was called internally. Default is true.</param>
        public void SetInputLength(int newValue, bool internalCall = true)
        {
            if (newValue != _inputLength)
            {
                if (IsOutputData)
                {
                    // If this control is for output data, we can not set input length that is different from what data definiton object defines:
                    if (!internalCall && DataDefinition != null)
                    {
                        if (newValue != DataDefinition.InputLength)
                            throw new InvalidOperationException("Can not change the number of input elements, since this control is for editing output elements.");
                    }
                }
                int previous = _inputLength;
                _inputLength = newValue;
                if (IsInputData)
                {
                    if (DataDefinition == null)
                    {
                        if (_inputLength > 0)
                        {
                            HasUnsavedChanges = true;
                            DataDefinition = new InputOutputDataDefiniton();
                        }
                    }
                    if (_inputLength > 0 && DataDefinition == null)
                        DataDefinition = new InputOutputDataDefiniton();
                    if (DataDefinition != null)
                    {
                        if (DataDefinition.InputLength != _inputLength)
                        {
                            HasUnsavedChanges = true;
                            while (DataDefinition.InputLength > _inputLength)
                            {
                                DataDefinition.InputElementList.RemoveAt(DataDefinition.InputLength - 1);
                            }
                            while (DataDefinition.InputLength < _inputLength)
                                DataDefinition.AddInputElement(CreateInputElement(DataDefinition.InputLength));
                            try
                            {
                                GridChangeEventsDisabled = true;
                                while (dataGridView1.RowCount > _inputLength)
                                    dataGridView1.Rows.RemoveAt(dataGridView1.RowCount - 1);
                                while (dataGridView1.RowCount < _inputLength)
                                {
                                    dataGridView1.Rows.Add(new object[dataGridView1.ColumnCount]);
                                    CopyDataToGridRow(dataGridView1.RowCount - 1);
                                }
                            }
                            finally
                            {
                                GridChangeEventsDisabled = false;
                            }
                        }
                    }
                    OnNumElementsChanged(previous, _inputLength);
                }
            }
        }  // SetInputLength(...)


        protected int _outputLength;

        /// <summary>Number of output parameters or data elements.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual int OutputLength
        {  
            get { return _outputLength; }
            set
            {
                SetOutputLength(value, false /* internalCall */);
            }
        }


        /// <summary>Sets the number of output elements (property <see cref="OutputLength"/>).
        /// <para>Internally, this method should be used instead of the property, because it does not throw on certain situations.</para>
        /// <para>Internally, the method should be called without arguments.</para></summary>
        /// <remarks>Outside this class, one should assign the property directyl, because that provides additional error checks.
        /// <para>Exception to the rule is in GUI controls and other classes that handle two controls of this kind at the same time,
        /// and setting the property can follow complex flow of ewents, so it is difficult to guarantee the right order of operations
        /// that will not trigger exceptions in various consistency checks.</para></remarks>
        /// <param name="newValue">Value to be assigned to the property.</param>
        /// <param name="internalCall">Specifies whether the method was called internally. Default is true.</param>
        public void SetOutputLength(int newValue, bool internalCall = true)
        {
            if (newValue != _outputLength)
            {
                if (IsInputData)
                {
                    // If this control is for input data, we can not set output length that is different for what data definiton object defines:
                    if (!internalCall && DataDefinition != null)
                    {
                        if (newValue != DataDefinition.OutputLength)
                            throw new InvalidOperationException("Can not change the number of output elements, since this control is for editing input elements.");
                    }
                }
                int previous = _outputLength;
                _outputLength = newValue;
                if (IsOutputData)
                {
                    if (DataDefinition == null)
                    {
                        if (_outputLength > 0)
                        {
                            HasUnsavedChanges = true;
                            DataDefinition = new InputOutputDataDefiniton();
                        }
                    }

                    if (_outputLength > 0 && DataDefinition == null)
                        DataDefinition = new InputOutputDataDefiniton();
                    if (DataDefinition != null)
                    {
                        if (DataDefinition.OutputLength != _outputLength)
                        {
                            HasUnsavedChanges = true;
                            while (DataDefinition.OutputLength > _outputLength)
                            {
                                DataDefinition.OutputElementList.RemoveAt(DataDefinition.OutputLength - 1);
                            }
                            while (DataDefinition.OutputLength < _outputLength)
                                DataDefinition.AddOutputElement(CreateOutputElement(DataDefinition.OutputLength));
                            try
                            {
                                GridChangeEventsDisabled = true;
                                while (dataGridView1.RowCount > _outputLength)
                                    dataGridView1.Rows.RemoveAt(dataGridView1.RowCount - 1);
                                while (dataGridView1.RowCount < _outputLength)
                                {
                                    dataGridView1.Rows.Add(new object[dataGridView1.ColumnCount]);
                                    CopyDataToGridRow(dataGridView1.RowCount - 1);
                                }
                            }
                            finally
                            {
                                GridChangeEventsDisabled = false;
                            }
                        }
                    }
                    OnNumElementsChanged(previous, _outputLength);
                }
            }
        }  // SetOutputLength(...)


        private string _titleInputData = "Input Data";

        /// <summary>Control's title (displayed as group box text) in case of input data editing.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleInputData
        {
            get { return _titleInputData; }
            set { _titleInputData = value; }
        }

        private string _titleOutputData = "Output Data";

        /// <summary>Control's title (displayed as group box text) in case of output data editing.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleOutputData
        {
            get { return _titleOutputData; }
            set { _titleOutputData = value; }
        }



        /// <summary>Gets text of the title label shown on top left of the form.</summary>
        public virtual string Title
        {
            get
            {
                if (IsInputData)
                    return TitleInputData;
                else
                    return TitleOutputData;
            }
        }


        /// <summary>Actual title that is shown on the top of control, in the <see cref="lblMainTitle"/> label, includes
        /// eventual marks for unsaved changes.</summary>
        public string TitleWithChangedSign
        {
            get
            {
                if (HasUnsavedChanges)
                    return Title + " (*) ";
                else
                    return Title;
            }
        }




        #region Data.Behavior


        private bool _hasUnsavedChanges = false;

        /// <summary>Whether the current function definition has unsaved changes.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool HasUnsavedChanges
        {
            get { return _hasUnsavedChanges; }
            set
            {
                if (value != _hasUnsavedChanges)
                {
                    _hasUnsavedChanges = value;
                    grpOuter.Text = TitleWithChangedSign;
                    if (!_hasUnsavedChanges)
                    {
                        if (DataDefinition == null)
                            indicatorLight1.SetOff();
                        else
                            indicatorLight1.SetOk();
                    } else
                    {
                        indicatorLight1.SetBusy();
                    }
                }
            }
        }

        private bool _isDimentionChangeAllowed = true;

        /// <summary>Indicates whether user can change dimensions (number of input/output data elements).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsDimentionChangeAllowed
        {
            get { return _isDimentionChangeAllowed; }
            set
            {
                if (value != _isDimentionChangeAllowed)
                {
                    _isDimentionChangeAllowed = value;
                }
            }
        }


        private bool _useLegalVariableNames = true;

        /// <summary>Specifies whether legal variable names (according to rules in programming languages such as C++, C# or Java) should be enforced for naming input and utput elements.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseLegalVariableNames
        {
            get { return _useLegalVariableNames; }
            set { _useLegalVariableNames = value; }
        }

        protected bool _isInputData = true;

        /// <summary>Indicates whether this form is used to edit input (when true) or output (when false) data elements.
        /// <para>Setting the property will change behavior.</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual bool IsInputData
        {
            get { return _isInputData; }
            set
            {
                if (value != _isInputData)
                {
                    _isInputData = value;
                    UpdateInputOrOutput(_isInputData);
                }
            }
        }

        /// <summary>Indicates whether this form is used to edit input (when false) or output (when true) data elements.
        /// <para>This property depends on <see cref="IsInputData"/> property and is just its negation.</para></summary>
        protected bool IsOutputData
        { get { return !IsInputData; } set { IsInputData = !value; } }


        /// <summary>Prepares control for processing either input or output data.
        /// <para>When calling this method, its body is executed regardles of whether it has been called with the same parameters before.
        /// This is different from <see cref="IsInputData"/> setter which has effect only when value is actually changed.</para></summary>
        /// <param name="isInput"></param>
        public void UpdateInputOrOutput(bool isInput)
        {
            grpOuter.Text = TitleWithChangedSign;

            IsInputData = isInput;
            if (isInput)
            {
                this.columnId.Visible = true;
                this.columnName.Visible = true;
                this.columnMin.Visible = true;
                this.columnMax.Visible = true;
                this.columnDefault.Visible = true;
                this.columnNumPoints.Visible = true;
                this.columnTitle.Visible = true;
                this.columnDescription.Visible = true;
            } else
            {
                //InputElementDefinition inp = new InputElementDefinition(2);
                //OutputElementDefinition el = new OutputElementDefinition(2);
                //var aa = el.MinimalValue;
                //var bb = inp.DefaultValue; bb = inp.

                this.columnId.Visible = true;
                this.columnName.Visible = true;
                this.columnMin.Visible = true;
                this.columnMax.Visible = true;
                this.columnDefault.Visible = false;  // property not present in output elements
                this.columnNumPoints.Visible = false;  // property not present in output elements
                this.columnTitle.Visible = true;
                this.columnDescription.Visible = true;
            }
        }
        

        #endregion Data.Behavior


        #endregion Data


        #region GridOperation


        private bool _gridChangeEventsDdisabled = false;

        /// <summary>Flag for disabling events raised when grid data is changed. Default is false.
        /// <para>This should be set to true in functions where grid data is changed programatically.</para></summary>
        protected bool GridChangeEventsDisabled
        {
            get { return _gridChangeEventsDdisabled; }
            set { _gridChangeEventsDdisabled = value; }
        }


        /// <summary>Initializes additional properties of the data grid view that were not set in designer.</summary>
        /// <remarks>This is currently not used because the relevant proprties are set in the designer's code.
        /// <para>We will keep the method for the case it becomes useful (e.g. if EinForms GUI designer would tend to override property values when 
        /// changing the form in the designer).</para></remarks>
        protected virtual void InitDataGridProperties()
        {
            this.columnId.ValueType = typeof(int);
            this.columnName.ValueType = typeof(string);
            this.columnMin.ValueType = typeof(double);
            this.columnMax.ValueType = typeof(double);
            this.columnDefault.ValueType = typeof(double);
            this.columnNumPoints.ValueType = typeof(int);
            this.columnTitle.ValueType = typeof(string);
            this.columnDescription.ValueType = typeof(string);
        }

        int _columnIndexId = -1;

        /// <summary>Returns column index of the ID field.</summary>
        public int ColumnIndexId
        { get { if (_columnIndexId < 0) _columnIndexId = dataGridView1.Columns[columnId.Name].Index; return _columnIndexId; } }

        int _columnIndexName = -1;

        /// <summary>Returns column index of the Name field.</summary>
        public int ColumnIndexName
        { get { if (_columnIndexName < 0) _columnIndexName = dataGridView1.Columns[columnName.Name].Index; return _columnIndexName; } }

        int _columnIndexMin = -1;

        /// <summary>Returns column index of the Min field.</summary>
        public int ColumnIndexMin
        { get { if (_columnIndexMin < 0) _columnIndexMin = dataGridView1.Columns[columnMin.Name].Index; return _columnIndexMin; } }

        int _columnIndexMax = -1;

        /// <summary>Returns column index of the Max field.</summary>
        public int ColumnIndexMax
        { get { if (_columnIndexMax < 0) _columnIndexMax = dataGridView1.Columns[columnMax.Name].Index; return _columnIndexMax; } }

        int _columnIndexDefault = -1;

        /// <summary>Returns column index of the Default field.</summary>
        public int ColumnIndexDefault
        { get { if (_columnIndexDefault < 0) _columnIndexDefault = dataGridView1.Columns[columnDefault.Name].Index; return _columnIndexDefault; } }

        int _columnIndexNumPoints = -1;

        /// <summary>Returns column index of the Num field.</summary>
        public int ColumnIndexNumPoints
        { get { if (_columnIndexNumPoints < 0) _columnIndexNumPoints = dataGridView1.Columns[columnNumPoints.Name].Index; return _columnIndexNumPoints; } }

        int _columnIndexTitle = -1;

        /// <summary>Returns column index of the Title field.</summary>
        public int ColumnIndexTitle
        { get { if (_columnIndexTitle < 0) _columnIndexTitle = dataGridView1.Columns[columnTitle.Name].Index; return _columnIndexTitle; } }

        int _columnIndexDescription = -1;

        /// <summary>Returns column index of the Description field.</summary>
        public int ColumnIndexDescription
        { get { if (_columnIndexDescription < 0) _columnIndexDescription = dataGridView1.Columns[columnDescription.Name].Index; return _columnIndexDescription; } }




        /// <summary>Returns the <see cref="DataGridView"/> control that is used for setting values
        /// of the input parameters.</summary>
        /// <returns></returns>
        public DataGridView GetGridControl()
        {
            return dataGridView1;
        }



        //protected int _colNumValue = 2;

        ///// <summary>Number of DataGrid Column that contains the value.</summary>
        //public int ColNumValue
        //{
        //    get { return _colNumValue; }
        //    protected set { _colNumValue = value; }
        //}

        /// <summary>Returns a suitable name for input data element with the specified index.</summary>
        /// <param name="elementIndex">Index of the input data element whose name is generated.</param>
        public string CreateInputElementName(int elementIndex)
        { return InputOutputDataDefiniton.CreateInputElementName(elementIndex, DataDefinition, UseLegalVariableNames); }

        /// <summary>Returns a suitable name for output data element with the specified index.</summary>
        /// <param name="elementIndex">Index of the output data element whose name is generated.</param>
        public string CreateOutputElementName(int elementIndex)
        { return InputOutputDataDefiniton.CreateOutputElementName(elementIndex, DataDefinition, UseLegalVariableNames); }

        /// <summary>Creates and returns a new input data element definition.</summary>
        /// <param name="elementIndex">Index of data elment.</param>
        public InputElementDefinition CreateInputElement(int elementIndex)
        {
            string elementName = CreateInputElementName(elementIndex);
            InputElementDefinition ret = new InputElementDefinition(elementIndex, elementName);
            ret.Title = string.Copy(elementName);
            ret.Description = "Input element No. " + elementIndex;
            return ret;
        }

        /// <summary>Creates and returns a new output data element definition.</summary>
        /// <param name="elementIndex">Index of data elment.</param>
        public OutputElementDefinition CreateOutputElement(int elementIndex)
        {
            string elementName = CreateOutputElementName(elementIndex);
            OutputElementDefinition ret = new OutputElementDefinition(elementIndex, elementName);
            ret.Title = string.Copy(elementName);
            ret.Description = "Output element No. " + elementIndex;
            return ret;
        }


        /// <summary>Copies data from the specified row of the data grid view to data definition object.</summary>
        /// <param name="whichRow">Index of row that is copied.</param>
        public void CopyGridRowToData(int whichRow)
        {
            int numRows = dataGridView1.RowCount;
            if (whichRow < 0 || whichRow >= numRows)
                throw new ArgumentException("Grid view row " + whichRow + " is out of range, should be between 0 and " + (numRows - 1).ToString());
            if (whichRow >= NumElements)
            {
                if (IsInputData)
                    throw new ArgumentException("Grid view row number" + whichRow + " should be smaller than the number of input elements " + NumElements + ".");
                else
                    throw new ArgumentException("Grid view row number" + whichRow + " should be smaller than the number of output elements " + NumElements + ".");
            }
            DataGridViewRow row = dataGridView1.Rows[whichRow];
            InputOutputElementDefinition element = null;
            if (IsInputData)
            {
                InputElementDefinition el = DataDefinition.GetInputElement(whichRow);
                element = el;
                if (el == null)
                    throw new InvalidOperationException("Input data element definition is not specified (null reference) for element " + whichRow + ".");

                // Properties that are specific to input data elements:
                if (row.Cells[ColumnIndexDefault].Value != null)
                {
                    el.DefaultValue = (double)row.Cells[ColumnIndexDefault].Value;
                    el.DefaultValueDefined = true;
                } else
                {
                    el.DefaultValue = 0.0;
                    el.DefaultValueDefined = false;
                }
                if (row.Cells[ColumnIndexNumPoints].Value != null)
                {
                    el.NumSamplingPoints = (int)row.Cells[ColumnIndexNumPoints].Value;
                }
                else
                    el.NumSamplingPoints = 0;

            } else
            {
                OutputElementDefinition el = DataDefinition.GetOutputElement(whichRow);
                element = el;
                if (el == null)
                    throw new InvalidOperationException("Input data element definition is not specified (null reference) for element " + whichRow + ".");
            }
            // Properties that are common to input and output elements:
            element.ElementIndex = whichRow;
            element.Name = (string)row.Cells[ColumnIndexName].Value;
            if (string.IsNullOrEmpty(element.Name))
            {
                if (IsInputData)
                    element.Name = this.CreateInputElementName(whichRow);
                else
                    element.Name = this.CreateOutputElementName(whichRow);
                row.Cells[ColumnIndexName].Value = element.Name;
            }
            element.Description = (string)row.Cells[ColumnIndexDescription].Value;
            element.Title = (string)row.Cells[ColumnIndexTitle].Value;

            element.BoundsDefined = false;
            if (row.Cells[ColumnIndexMin].Value != null)
            {
                element.MinimalValue = (double)row.Cells[ColumnIndexMin].Value;
                element.BoundsDefined = true;
            }
            else
                element.MinimalValue = double.MinValue;
            if (row.Cells[ColumnIndexMax].Value != null)
            {
                element.MaximalValue = (double)row.Cells[ColumnIndexMax].Value;
                element.BoundsDefined = true;
            }
            else
                element.MaximalValue = double.MaxValue;
            // Note unsaved changes and rase the appropriate event:
            HasUnsavedChanges = true;
            OnElementDataChanged();
        }


        /// <summary>Copies data to the specified row of the data grid view from data definition object <see cref="DataDefinition"/>.</summary>
        /// <param name="whichRow">Index of row that is copied.</param>
        public void CopyDataToGridRow(int whichRow)
        {
            GridChangeEventsDisabled = true;
            try
            {
                int numRows = dataGridView1.RowCount;
                if (whichRow < 0 || whichRow >= numRows)
                    throw new ArgumentException("Grid view row " + whichRow + " is out of range, should be between 0 and " + (numRows - 1).ToString());
                if (whichRow >= NumElements)
                {
                    if (IsInputData)
                        throw new ArgumentException("Grid view row number" + whichRow + " should be smaller than the number of input elements " + NumElements + ".");
                    else
                        throw new ArgumentException("Grid view row number" + whichRow + " should be smaller than the number of output elements " + NumElements + ".");
                }
                DataGridViewRow row = dataGridView1.Rows[whichRow];
                InputOutputElementDefinition element = null;
                if (IsInputData)
                {
                    InputElementDefinition el = DataDefinition.GetInputElement(whichRow);
                    element = el;
                    if (el == null)
                        throw new InvalidOperationException("Input data element definition is not specified (null reference) for element " + whichRow + ".");

                    if (el.DefaultValueDefined)
                    {
                        row.Cells[ColumnIndexDefault].Value = el.DefaultValue;
                    } else
                    {
                        row.Cells[ColumnIndexDefault].Value = null;
                    }
                    row.Cells[ColumnIndexNumPoints].Value = el.NumSamplingPoints;
                }
                else
                {
                    OutputElementDefinition el = DataDefinition.GetOutputElement(whichRow);
                    element = el;
                    if (el == null)
                        throw new InvalidOperationException("Output data element definition is not specified (null reference) for element " + whichRow + ".");

                }

                // element.ElementIndex = whichRow;
                row.Cells[ColumnIndexId].Value = whichRow;

                if (string.IsNullOrEmpty(element.Name))
                {
                    if (IsInputData)
                        element.Name = this.CreateInputElementName(whichRow);
                    else
                        element.Name = this.CreateOutputElementName(whichRow);
                }
                row.Cells[ColumnIndexName].Value = element.Name;
                row.Cells[ColumnIndexDescription].Value = element.Description;
                row.Cells[ColumnIndexTitle].Value = element.Title;

                if (element.BoundsDefined)
                {
                    row.Cells[ColumnIndexMin].Value = element.MinimalValue;
                    row.Cells[ColumnIndexMax].Value = element.MaximalValue;
                } else
                {
                    row.Cells[ColumnIndexMin].Value = null;
                    row.Cells[ColumnIndexMax].Value = null;
                }
            }
            finally
            {
                GridChangeEventsDisabled = false;
            }
        }

        /// <summary>Copies data from the <see cref="DataDefinition"/> object to the internal data grid view control.</summary>
        public void CopyDataToGrid()
        {
            GridChangeEventsDisabled = true;
            try
            {
                if (DataDefinition != null)
                {

                    // Control is for editing Input data definitions: 
                    dataGridView1.RowCount = 0;
                    dataGridView1.Refresh();
                    int numRows = NumElements; // number of input or output elements, dependent on which kind of data is represented by this control
                    for (int rowNum = 0; rowNum < numRows; ++rowNum)
                    {

                        int rowNumReturned = dataGridView1.Rows.Add(new object[dataGridView1.ColumnCount]);
                        DataGridViewRow currentRow = dataGridView1.Rows[rowNumReturned];
                        if (rowNumReturned != rowNum)
                            throw new InvalidOperationException("Wrong row number returned when adding a new row: " + rowNumReturned + ", sould be " + rowNum + ".");


                        //DataGridViewRowCollection allRows = dataGridView1.Rows;
                        //int currentNumRows = allRows.Count;
                        //// DataGridViewRow currentRow = allRows[rowNum];
                        //int currentRowNumElements = currentRow.Cells.Count;
                        //DataGridViewCell el0 = currentRow.Cells[0];
                        //DataGridViewCell el1 = currentRow.Cells[1];
                        //DataGridViewCell el2 = currentRow.Cells[2];
                        //DataGridViewCell el3 = currentRow.Cells[3];
                        //DataGridViewCell el4 = currentRow.Cells[4];
                        //if (false)
                        //{
                        //    DataGridViewCell element0 = dataGridView1[rowNum, 0];
                        //    DataGridViewCell element1 = dataGridView1[rowNum, 1];
                        //    DataGridViewCell element2 = dataGridView1[rowNum, 2];
                        //    DataGridViewCell element3 = dataGridView1[rowNum, 3];
                        //    DataGridViewCell element4 = dataGridView1[rowNum, 4];
                        //}

                        //object val0 = el0.Value;
                        //object val1 = el1.Value;
                        //object val2 = el2.Value;
                        //object val3 = el3.Value;
                        //object val4 = el4.Value;

                        //var type0 = (val0 == null ? null : val0.GetType().FullName);
                        //var type1 = (val1 == null ? null : val1.GetType().FullName);
                        //var type2 = (val2 == null ? null : val2.GetType().FullName);
                        //var type3 = (val3 == null ? null : val3.GetType().FullName);
                        //var type4 = (val4 == null ? null : val4.GetType().FullName);

                        //var cellType0 = el0.ValueType;
                        //var cellType1 = el1.ValueType;
                        //var cellType2 = el2.ValueType;
                        //var cellType3 = el3.ValueType;
                        //var cellType4 = el4.ValueType;


                        // Populate grid row by copying the appropriate data from data definition object:
                        CopyDataToGridRow(rowNum);

                    }
                    dataGridView1.Refresh();
                }
            }
            finally
            {
                GridChangeEventsDisabled = false;
            }
        }


        /// <summary>Clears contents of all DataGridView cells.</summary>
        public void ClearCells()
        {
            DataGridViewRowCollection rows = dataGridView1.Rows;
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
            dataGridView1.Refresh();
        }

        /// <summary>Removes all the data from the DataGridView.</summary>
        public void ClearDataGrid()
        {
            dataGridView1.RowCount = 0;
            dataGridView1.Refresh();
        }



        //public void ResetToDefault()
        //{
        //    CopyDataToGrid();
        //    DataGridViewRowCollection rows = dGridInputParam.Rows;
        //    for (int i = 0; i < DataDefinition.InputLength; ++i)
        //    {
        //        if (i < rows.Count)
        //        {
        //            DataGridViewRow row = rows[i];
        //            InputElementDefinition def = DataDefinition.InputElementList[i];
        //            double value = 0;
        //            if (def.DefaultValueDefined)
        //            {
        //                value = def.DefaultValue;
        //            }
        //            else if (def.BoundsDefined)
        //            {
        //                value = 0.5 * (def.MinimalValue + def.MaximalValue);
        //            }
        //            row.Cells[ColNumValue].Tag = value;
        //        }
        //    }
        //}

        //public void ResetToCenter()
        //{
        //    CopyDataToGrid();
        //    DataGridViewRowCollection rows = dGridInputParam.Rows;
        //    for (int i = 0; i < DataDefinition.InputLength; ++i)
        //    {
        //        if (i < rows.Count)
        //        {
        //            DataGridViewRow row = rows[i];
        //            InputElementDefinition def = DataDefinition.InputElementList[i];
        //            double value = 0;
        //            if (def.BoundsDefined)
        //            {
        //                value = 0.5 * (def.MinimalValue + def.MaximalValue);
        //            } else if (def.DefaultValueDefined)
        //            {
        //                value = def.DefaultValue;
        //            }
        //            row.Cells[ColNumValue].Tag = value;
        //        }
        //    }
        //}




        /// <summary>Current number of values, otained form the DataGridView.</summary>
        public int NumValues
        {
            get { return dataGridView1.RowCount; }
        }


        protected Vector _values;


        ///// <summary>Vector of current values of input parameters as defined by the DadaGridView.</summary>
        //public IVector Values
        //{
        //    get {
        //        if (NumValues <= 0)
        //            _values = null;
        //        else
        //        {
        //            _values = new Vector(NumValues);
        //            for (int i = 0; i < _values.Length; ++i)
        //            {
        //                if (i < dGridInputParam.RowCount)
        //                {
        //                    DataGridViewRow row = dGridInputParam.Rows[i];
        //                    _values[i] = double.Parse(row.Cells[ColNumValue].Value.ToString());
        //                }
        //            }
        //        }
        //        return _values;
        //    }
        //    protected set
        //    {
        //        if (value == null)
        //        {
        //            if (NumValues > 0)
        //                throw new ArgumentException("Vector of values to be set is not specified (null argument).");
        //        } else
        //        {
        //            if (value.Length != NumValues)
        //            {
        //                throw new ArgumentException("Vector of values is of wrong dimension, " + value.Length
        //                    + " instead of " + NumValues + ".");
        //            }
        //            else
        //            {
        //                if (_values.Length != value.Length)
        //                    _values = new Vector(value.Length);
        //                Vector.CopyPlain(value, _values);
        //                for (int i = 0; i < value.Length; ++i)
        //                {
        //                    if (i<dGridInputParam.RowCount)
        //                    {
        //                        DataGridViewRow row = dGridInputParam.Rows[i];
        //                        row.Cells[ColNumValue].Tag = value[i];
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


        ///// <summary>Sets the values of input parameters on the DataViewGrid to the elements of the specified vector.</summary>
        ///// <param name="values">Vectors defining the values to be set.</param>
        //public void SetValues(IVector values)
        //{ this.Values = values; }


        #endregion GridOperation


        #region Events


        /// <summary>Occurs when data definition object is changed (i.e., when object reference changes), meaning
        /// that whole data definition object is replaced.</summary>
        public event EventHandler<ValueChangeEventArgs<InputOutputDataDefiniton>> DefinitionObjectChanged;

        /// <summary>Raises the <see cref="DefinitionObjectChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldDataDefinition">Old data definition object.</param>
        /// <param name="newDataDefinition">New data definition object.</param>
        protected void OnDefinitionObjectChanged(InputOutputDataDefiniton oldDataDefinition, InputOutputDataDefiniton newDataDefinition)
        {
            if (DefinitionObjectChanged != null)
                DefinitionObjectChanged(this, new ValueChangeEventArgs<InputOutputDataDefiniton>(oldDataDefinition, newDataDefinition));
            indicatorLight1.BlinkSpecial(Color.LightBlue, 2);
        }


        /// <summary>Occurs when number of data elements (<see cref="InputLength"/> or <see cref="OutputLength"/>, dependent on <see cref="IsInputData"/> property) is changed.</summary>
        public event EventHandler<IndexChangeEventArgs> NumElementsChanged;

        /// <summary>Raises the <see cref="NumElementsChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldLength">Old length.</param>
        /// <param name="newLength">New length.</param>
        protected void OnNumElementsChanged(int oldLength, int newLength)
        {
            if (NumElementsChanged != null)
                NumElementsChanged(this, new IndexChangeEventArgs(oldLength, newLength));
            indicatorLight1.BlinkSpecial(Color.Magenta, 3);
        }


        /// <summary>Occurs when some element property is changed.</summary>
        public event EventHandler ElementDataChanged;

        /// <summary>Raises the <see cref="ElementDataChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        protected void OnElementDataChanged()
        {
            if (ElementDataChanged != null)
                ElementDataChanged(this, new EventArgs());
            indicatorLight1.BlinkSpecial(Color.Yellow, 2);
        }
        


        #endregion Events


        public virtual string CreateSummary()
        {
            StringBuilder sb = new StringBuilder();

            if (IsInputData)
                sb.AppendLine("This control handles INPUT data definitions.");
            else
                sb.AppendLine("This control handles OUTPUT data definitions.");
            if (IsInputData && NumElements != InputLength || !IsInputData && NumElements != OutputLength)
                sb.AppendLine("  ERROR: Number of data elements is inconsistent.");
            sb.AppendLine("  Input dimension = " + InputLength + ", output dimension = " + OutputLength
                + ", number of data elements: " + NumElements);

            if (NumElementsChanged == null)
                sb.AppendLine("  NumElementsChanged: no handlers set.");
            else
            {
                int numHandlers = NumElementsChanged.GetInvocationList().Count();
                sb.AppendLine("  NumElementsChanged: " + numHandlers + " handler(s).");
            }

            if (DefinitionObjectChanged == null)
                sb.AppendLine("  DefinitionObjectChanged: no handlers set.");
            else
            {
                int numHandlers = DefinitionObjectChanged.GetInvocationList().Count();
                sb.AppendLine("  DefinitionObjectChanged: " + numHandlers + " handler(s).");
            }

            if (ElementDataChanged == null)
                sb.AppendLine("  DataChanged: no handlers set.");
            else
            {
                int numHandlers = ElementDataChanged.GetInvocationList().Count();
                sb.AppendLine("  DataChanged: " + numHandlers + " handler(s).");
            }

            return sb.ToString();
        }


        public void ShowContextMenuControl(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuControl.Show(new Point(e.X, e.Y));
            }
        }

        private void InputDataDefinitionControl_MouseClick(object sender, MouseEventArgs e)
        {
            ShowContextMenuControl(sender, e);
        }

        private void dGridInputParam_MouseClick(object sender, MouseEventArgs e)
        {
            ShowContextMenuControl(sender, e);
        }

        private void btnResetDefault_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            //ResetToDefault();
        }

        private void btnResetCenter_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            // ResetToCenter();
        }

        private void resetToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            //ResetToDefault();
        }

        private void resetToCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            // ResetToCenter();
        }

        private void dGridInputParam_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!GridChangeEventsDisabled)
            {
                btnResetDefault.Enabled = false;
                CopyGridRowToData(e.RowIndex);
                Util.SleepSeconds(0.02);
                btnResetDefault.Enabled = true;
            }
        }



    }
}
