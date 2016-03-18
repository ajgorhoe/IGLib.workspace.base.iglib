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

    /// <summary>Selection of the output value of interest in an ANN model.</summary>
    /// $A Igor Apr13 March16;
    public partial class OutputValueSelector : UserControl
    {



        public OutputValueSelector()
        {
            InitializeComponent();
        }

        #region CommonData


        private InputOutputDataDefiniton _dataDefinition;


        /// <summary>Data about input and output quantities of the manipulated functions or response.</summary>
        public InputOutputDataDefiniton DataDefinition
        {
            get
            {
                return _dataDefinition;
            }
            set
            {
                if (!object.ReferenceEquals(value, _dataDefinition))
                {
                    _dataDefinition = value;
                    UpdateDataDefinition();
                }
            }
        }


        /// <summary>Initialiyes data after data definition object been set or changed.</summary>
        public void UpdateDataDefinition()
        {
            if (DataDefinition == null)
            {

            }
            else
            {
                NumInputParameters = DataDefinition.InputLength;
                NumOutputValues = DataDefinition.OutputLength;

                int outputLength = DataDefinition.OutputLength;
                for (int i = 0; i < outputLength; ++i)
                {
                    OutputElementDefinition def = DataDefinition.OutputElementList[i];
                    string itemText = def.Title;
                    if (string.IsNullOrEmpty(itemText))
                        itemText = def.Name;
                    if (string.IsNullOrEmpty(itemText))
                        itemText = "Output Value " + i.ToString();
                    comboBoxSelection.Items.Add(itemText);
                }
                SelectedOutputId = 0;
                UpdateSelectedParameterIdDependencies();
            }
        }


        #endregion CommonData



        #region Events

        /// <summary>Occurs when selected output value ID is changed.</summary>
        public event EventHandler<IndexChangeEventArgs> SelectedOutputIdChanged;

        /// <summary>Raises the <see cref="SelectedOutputIdChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldId">Old output element Id.</param>
        /// <param name="newId">New output element Id.</param>
        protected void OnSelectedOutputChanged(int oldId, int newId)
        {
            if (SelectedOutputIdChanged != null)
                SelectedOutputIdChanged(this, new IndexChangeEventArgs(oldId, newId));
        }


        #endregion Events


        #region Data.Operation

        int _numInputParameters = 2;

        /// <summary>Gets number of input parameters.</summary>
        public int NumInputParameters
        {
            get
            {
                if (DataDefinition != null)
                    _numInputParameters = DataDefinition.InputLength;
                return _numInputParameters;
            }
            set
            {
                if (value != NumInputParameters)
                {
                    _numInputParameters = value;
                    if (DataDefinition != null)
                    {
                        if (value != DataDefinition.InputLength)
                            throw new ArgumentException("Number of input parameters can not be set different as specified by definition data.");
                    }
                }
            }
        }

        int _numOutputValues = 2;

        /// <summary>Gets number of output values.</summary>
        public int NumOutputValues
        {
            get
            {
                if (DataDefinition != null)
                    _numOutputValues = DataDefinition.OutputLength;
                return _numOutputValues;
             }
            set
            {
                if (value != NumOutputValues)
                {
                    if (DataDefinition != null)
                    {
                        if (value != DataDefinition.OutputLength)
                            throw new ArgumentException("Number of output values can not be set different as specified by definition ddata.");
                    }
                    _numOutputValues = value;
                }
                // GUI must be updated in any case, even if the value is not different from previous:
                txtValueNum.Minimum = 0;
                txtValueNum.Maximum = NumOutputValues - 1;
            }
        }


        protected int _selectedOutputId = 0;

        /// <summary>Sequential number of the selected parameter as specified by the user.</summary>
        public int SelectedOutputId
        {
            get { return _selectedOutputId; }
            protected set
            {
                if (value != _selectedOutputId)
                {
                    int oldValue = _selectedOutputId;
                    if (value >= NumOutputValues)
                        value = NumOutputValues - 1;
                    if (value < 0)
                        value = 0;
                    
                    if (value != _selectedOutputId)
                    {
                        _selectedOutputId = value;
                        UpdateSelectedParameterIdDependencies();  // update GUI
                        // Trigger the event:
                        OnSelectedOutputChanged(oldValue, value);
                    } else
                    {
                        // GUI update must be done in any case, e.g. to account for rolling back a change due to index out of range:
                        UpdateSelectedParameterIdDependencies();
                    }
                }
            }
        }

        /// <summary>Replacement for the <see cref="SelectedOutputId"/> setter that has a lower access level.</summary>
        /// <param name="id">Sequential number of the output valuee to be selected.</param>
        public void SetSelectedOutputId(int id)
        { SelectedOutputId = id; }

        /// <summary>Updates things that depend on the selected parameter's index.</summary>
        public void UpdateSelectedParameterIdDependencies()
        {
            comboBoxSelection.SelectedIndex = SelectedOutputId;
            txtValueNum.Value = SelectedOutputId;
        }


        #endregion Data.Operation

        private void comboBoxSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedOutputId = comboBoxSelection.SelectedIndex;
        }

        private void txtParamNum_ValueChanged(object sender, EventArgs e)
        {
            this.SelectedOutputId = (int)txtValueNum.Value;
        }


        private void btnDecrease_Click(object sender, EventArgs e)
        {
            SelectedOutputId = SelectedOutputId - 1;
        }

        private void btnIncrease_Click(object sender, EventArgs e)
        {
            SelectedOutputId = SelectedOutputId + 1;
        }


        private void menuSummary_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Number of input parameters: " + NumInputParameters);
            sb.AppendLine("Number of output values: " + NumOutputValues);
            sb.AppendLine();
            sb.AppendLine("Selected output index: " + SelectedOutputId);
            sb.AppendLine();
            if (DataDefinition == null)
                sb.AppendLine("Data definitions are NOT provided.");
            else
                sb.AppendLine("Data definitions are provided.");
            sb.AppendLine();
            sb.AppendLine("Events: ");
            if (SelectedOutputIdChanged == null)
                sb.AppendLine("SelectedOutputIdChanged: no handlers.");
            else
            {
                int numHandlers = SelectedOutputIdChanged.GetInvocationList().Count();
                sb.AppendLine("SelectedOutputIdChanged: " + numHandlers + " handler(s).");
            }
            sb.AppendLine();
            FadingMessage msg = new FadingMessage("Output selector summary", sb.ToString(), 4000, false);
            msg.LaunchedInParallelThread = false;
            msg.Launch(false /* parallel */);
        }


    }
}
