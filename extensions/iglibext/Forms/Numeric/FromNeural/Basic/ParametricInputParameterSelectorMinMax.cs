// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/


#define DEBUGTESTING

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

    /// <summary>Selection of parameter to be varied, together with bounds - for parametric tests.</summary>
    /// $A Igor Apr13 March16;
    public partial class InputParameterSelectorMinMax : UserControl
    {


        public InputParameterSelectorMinMax()
        {
            InitializeComponent();
        }


        #region CommonData


        private InputOutputDataDefiniton _dataDefinition;


        /// <summary>Data about input and output quantities of the manipulated functions or response.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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


        /// <summary>Updates data with accordance to data definitions in <see cref="DataDefinition"/>.</summary>
        public void UpdateDataDefinition()
        {
            if (DataDefinition == null)
            {
            }
            else
            {
                NumInputParameters = DataDefinition.InputLength;
                NumOutputValues = DataDefinition.OutputLength;

                int inputLength = DataDefinition.InputLength;
                MinValues = new double[inputLength];
                MaxValues = new double[inputLength];
                for (int i = 0; i < inputLength; ++i)
                {
                    InputElementDefinition def = DataDefinition.InputElementList[i];
                    string itemText = def.Title;
                    if (string.IsNullOrEmpty(itemText))
                        itemText = def.Name;
                    if (string.IsNullOrEmpty(itemText))
                        itemText = "Input Parameter " + i.ToString();
                    comboBoxSelection.Items.Add(itemText);
                    if (def.BoundsDefined)
                    {
                        MinValues[i] = def.MinimalValue;
                        MaxValues[i] = def.MaximalValue;
                    }
                    else if (def.DefaultValueDefined)
                    {
                        MinValues[i] = def.DefaultValue;
                        MaxValues[i] = def.DefaultValue;
                    }
                    else
                    {
                        MinValues[i] = 0.0;
                        MaxValues[i] = 0.0;
                    }
                }
                // SelectedParameterId = 0;
                UpdateSelectedParameterIdDependencies();
            }
        }



        #endregion CommonData


        #region Events

        /// <summary>Occurs when selected input parameter ID is changed.</summary>
        public event System.EventHandler<IndexChangeEventArgs> SelectedParameterIdChanged;

        /// <summary>Raises the <see cref="SelectedParameterIdChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldId">Old parameter Id.</param>
        /// <param name="newId">New parameter Id.</param>
        protected void OnSelectedParameterIdChanged(int oldId, int newId)
        {
            if (SelectedParameterIdChanged != null)
                SelectedParameterIdChanged(this, new IndexChangeEventArgs(oldId, newId));
        }


        /// <summary>Occurs when selected input parameter's minimal value is changed by the user
        /// (but only when it is changed through GUI).</summary>
        public event System.EventHandler<ValueChangeEventArgs> SelectedParameterMinChanged;

        /// <summary>Raises the <see cref="SelectedParameterMinChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldValue">Old minimal parameter value.</param>
        /// <param name="newValue">New minimal parameter value.</param>
        protected void OnSelectedParameterMinChanged(double oldValue, double newValue)
        {
            if (SelectedParameterMinChanged != null)
                SelectedParameterMinChanged(this, new ValueChangeEventArgs(oldValue, newValue));
        }


        /// <summary>Occurs when selected input parameter's maximal value is changed by the user
        /// (but only when it is changed through GUI).</summary>
        public event System.EventHandler<ValueChangeEventArgs> SelectedParameterMaxChanged;

        /// <summary>Raises the <see cref="SelectedParameterMaxChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldValue">Old maximal parameter value.</param>
        /// <param name="newValue">New maximal parameter value.</param>
        protected void OnSelectedParameterMaxChanged(double oldValue, double newValue)
        {
            if (SelectedParameterMaxChanged != null)
                SelectedParameterMaxChanged(this, new ValueChangeEventArgs(oldValue, newValue));
        }


        // public event SelectedIdEventHandler SelectedParameterIdChanged;  // old definition
        // public event ValueChangedEventHandler SelectedParameterMinChanged;  // old definition
        // public event ValueChangedEventHandler SelectedParameterMaxChanged;  // old definition


        #endregion Events


        #region Appearance

        /// <summary>String that appears in the label above parameter selector (title of parameter selection).
        /// <para>This can be used to change the title in the case there are more than one input parameters selected,
        /// or a customized title is desired.</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleParameterSelection
        {
            get { return lblSelectParameter.Text; }
            set { lblSelectParameter.Text = value; }
        }


        #endregion Appearance


        #region Data.Operation


        int _numInputParameters = 2;

        /// <summary>Gets number of input parameters.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
                    if (DataDefinition != null)
                    {
                        if (value != DataDefinition.InputLength)
                            throw new ArgumentException("Number of input parameters can not be set different as specified by definition data.");
                    }
                    _numInputParameters = value;
                }
                // GUI must be updated in any case, even if the value is not different from previous:
                txtParamNum.Minimum = 0;
                txtParamNum.Maximum = NumInputParameters - 1;
            }
        }

        int _numOutputValues = 2;

        /// <summary>Gets number of output values.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
                    _numOutputValues = value;
                    if (DataDefinition != null)
                    {
                        if (value != DataDefinition.OutputLength)
                            throw new ArgumentException("Number of output values can not be set different as specified by definition ddata.");
                    }
                }
            }
        }


        protected double[] _minValues;

        protected double[] _maxValues;

        /// <summary>Minimal values of input parameters, obtained from <see cref="DataDefinition"/>.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double[] MinValues
        {
            get { return _minValues; }
            protected set { _minValues = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double[] MaxValues
        {
            get { return _maxValues; }
            protected set { _maxValues = value; }
        }

        protected int _selectedParameterId = 0;


        /// <summary>Sequential number of the selected parameter as specified by the user (usually through GUI).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedParameterId
        {
            get { return _selectedParameterId; }
            protected set
            {
                if (value != _selectedParameterId)
                {
                    int oldValue = _selectedParameterId;
                    if (value >= NumInputParameters)
                        value = NumInputParameters - 1;
                    if (value < 0)
                        value = 0;
                    if (value != _selectedParameterId)
                    {
                        _selectedParameterId = value;
                        UpdateSelectedParameterIdDependencies();  // update GUI
                        OnSelectedParameterIdChanged(oldValue, value);
                    } else
                    {
                        // GUI update must be done in any case, e.g. to account for rolling back a change due to index out of range:
                        UpdateSelectedParameterIdDependencies(); 
                    }
                }
            }
        }

        /// <summary>Replacement for the <see cref="SelectedParameterId"/> setter that has a lower access level.</summary>
        /// <param name="id">Sequential number of the output valuee to be selected.</param>
        public void SetSelectedParameterId(int id)
        {
            SelectedParameterId = id;
        }

        /// <summary>Updates things that depend on the selected parameter's index.</summary>
        public void UpdateSelectedParameterIdDependencies()
        {
            comboBoxSelection.SelectedIndex = SelectedParameterId;
            txtParamNum.Value = SelectedParameterId;
            txtMinValue.Text = SelectedParameterMin.ToString();
            txtMaxValue.Text = SelectedParameterMax.ToString();
            if (SelectedParameterId >= 0 && SelectedParameterId < MinValues.Length)
                txtMinValue.Text = MinValues[SelectedParameterId].ToString();
            if (SelectedParameterId >= 0 && SelectedParameterId < MaxValues.Length)
                txtMaxValue.Text = MaxValues[SelectedParameterId].ToString();
        }


        /// <summary>Minimal value of the selected parameter.</summary>
        public double SelectedParameterMin
        {
            get
            {
                if (MinValues != null)
                {
                    if (SelectedParameterId >= 0 && SelectedParameterId < MinValues.Length)
                        return MinValues[SelectedParameterId];
                }
                return 0.0;
            }
            //protected set
            //{
            //    if (value != _selectedParameterMin)
            //    {
            //        double oldValue = _selectedParameterMin;
            //        _selectedParameterMin = value;
            //    }
            //}
        }


        /// <summary>Maximal value of the selected parameter as specified by the user.</summary>
        public double SelectedParameterMax
        {
            get
            {
                if (MaxValues != null)
                {
                    if (SelectedParameterId >= 0 && SelectedParameterId < MaxValues.Length)
                        return MaxValues[SelectedParameterId];
                }
                return 0.0;
            }
        }


        #endregion Data.Operation


        private void comboBoxSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedParameterId = comboBoxSelection.SelectedIndex;
        }

        private void txtParamNum_ValueChanged(object sender, EventArgs e)
        {
            this.SelectedParameterId = (int)txtParamNum.Value;
        }


        private double _previousMinValue;
        private double _newMinvalue;

        private void txtMinValue_TextChanged(object sender, EventArgs e)
        {
            
        }


        private double _previousMaxValue;
        private double _newMaxValue;


        private void txtMaxValue_TextChanged(object sender, EventArgs e)
        {
            
        }


        private void txtMinValue_Validated(object sender, EventArgs e)
        {
            double value = 0;
            if (MinValues.Length > SelectedParameterId)
            {
                value = MinValues[SelectedParameterId];
            }
            if (double.TryParse(txtMinValue.Text, out value))
            {
                if (MinValues.Length > SelectedParameterId)
                {
                    MinValues[SelectedParameterId] = value;
                }
            }
            else
                txtMinValue.Text = value.ToString();
            _newMinvalue = value;

            // Also raise the event:
            OnSelectedParameterMinChanged(_previousMinValue, _newMinvalue);
            _previousMinValue = _newMinvalue;
        }

        private void txtMaxValue_Validated(object sender, EventArgs e)
        {
            double value = 0;
            if (MaxValues.Length > SelectedParameterId)
            {
                value = MaxValues[SelectedParameterId];
            }
            if (double.TryParse(txtMaxValue.Text, out value))
            {
                if (MaxValues.Length > SelectedParameterId)
                {
                    MaxValues[SelectedParameterId] = value;
                }
            }
            else
                txtMaxValue.Text = value.ToString();
            _newMaxValue = value;

            // Also raise the event:
            OnSelectedParameterMaxChanged(_previousMaxValue, _newMaxValue);
            _previousMaxValue = _newMaxValue;
        }

        private void btnDecrease_Click(object sender, EventArgs e)
        {
            SelectedParameterId = SelectedParameterId - 1;
        }

        private void btnIncrease_Click(object sender, EventArgs e)
        {
            SelectedParameterId = SelectedParameterId + 1;
        }

        private void menuSummary_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Number of input parameters: " + NumInputParameters);
            sb.AppendLine("Number of output values: " + NumOutputValues);
            sb.AppendLine();
            sb.AppendLine("Selected input index: " + SelectedParameterId);
            sb.AppendLine();
            if (DataDefinition == null)
                sb.AppendLine("Data definitions are NOT provided.");
            else
                sb.AppendLine("Data definitions are provided.");
            if (MinValues != null)
                sb.AppendLine("Minimal values are provided, dimension: " + MinValues.Length);
            else
                sb.AppendLine("Minimal values are NOT provided.");
            if (MaxValues != null)
                sb.AppendLine("Maximal values are provided, dimension: " + MaxValues.Length);
            else
                sb.AppendLine("Maximal values are NOT provided.");
            if (MinValues != null && MaxValues != null)
            {
                int numMinMaxDefined = 0;
                for (int i = 0; i < MinValues.Length && i < MaxValues.Length; ++i)
                {
                    if (MinValues[i] < MaxValues[i] && MinValues[i] > double.MinValue && MaxValues[i] < double.MaxValue)
                    {
                        ++numMinMaxDefined;
                    }
                }
                sb.AppendLine("Number of regular minimum/maximum pairs defined: " + numMinMaxDefined);
            }
            sb.AppendLine();
            sb.AppendLine("Events: ");
            if (SelectedParameterIdChanged == null)
                sb.AppendLine("SelectedParameterIdChanged: no handlers.");
            else
            {
                int numHandlers = SelectedParameterIdChanged.GetInvocationList().Count();
                sb.AppendLine("SelectedParameterIdChanged: " + numHandlers + " handler(s).");
            }
            if (SelectedParameterMinChanged == null)
                sb.AppendLine("SelectedParameterMinChanged: no handlers.");
            else
            {
                int numHandlers = SelectedParameterMinChanged.GetInvocationList().Count();
                sb.AppendLine("SelectedParameterMinChanged: " + numHandlers + " handler(s).");
            }
            if (SelectedParameterMaxChanged == null)
                sb.AppendLine("SelectedParameterMaxChanged: no handlers.");
            else
            {
                int numHandlers = SelectedParameterMaxChanged.GetInvocationList().Count();
                sb.AppendLine("SelectedParameterMaxChanged: " + numHandlers + " handler(s).");
            }
            sb.AppendLine();

            FadingMessage msg = new FadingMessage("Input selector summary", sb.ToString(), 4000, false);
            msg.LaunchedInParallelThread = false;
            msg.Launch(false /* parallel */);
        }



    }
}
