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
using IG.Neural;
using IG.Gr;


using ZedGraph;

using Color = System.Drawing.Color;
using DashStyle = System.Drawing.Drawing2D.DashStyle;
using System.Threading;

namespace IG.Forms
{


    /// <summary>Form for training artificial neural networks.</summary>
    /// $A Igor Jul13;
    public partial class VectorFunctionPlotter1d : UserControl //, INeuralModelContainer
    {
        
        ///// <summary>Constructs the control, with ANN-based model specified.</summary>
        ///// <param name="neuralModel">ANN-based model, containing data definitions and trained neural network.</param>
        //public NeuralParametricTest(INeuralModel neuralModel): this()
        //{
        //    this.NeuralModel = NeuralModel;
        //}

        public VectorFunctionPlotter1d()
        {
            InitializeComponent();

            chkPlotImmediately.Checked = PlotImmediately;
            numNumPlotPoints.Value = NumPlotPoints;
        }

        //bool _isReady = false;

        //bool IsReady
        //{
        //    get { return _isReady; }
        //    set { _isReady = value; }
        //}

        /// <summary>Reorter used for launching info, warning and error reports.</summary>
        IReporter Reporter
        { get { return UtilForms.Reporter; } }

        #region CommonData


        private InputOutputDataDefiniton _neuralDataDefinition;


        /// <summary>Data about input and output quantities of the manipulated functions or response.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public InputOutputDataDefiniton NeuralDataDefinition
        {
            get
            {
                //if (_neuralDataDefinition == null)
                //{
                //    Reporter.ReportError("Input / output data definition is invalid (null reference).");
                //}
                return _neuralDataDefinition;
            }
            set
            {
                if (!object.ReferenceEquals(value, _neuralDataDefinition))
                {
                    _neuralDataDefinition = value;

                    neuralInputControl1.DataDefinition = value;
                    neuralOutputValueSelector1.DataDefinition = value;
                    neuralInputParameterSelector1.DataDefinition = value;

                    if (_function != null && _neuralDataDefinition != null)
                    {
                        _function.SetNumParameters(_neuralDataDefinition.InputLength);
                        _function.SetNumValues(_neuralDataDefinition.OutputLength);
                    }

                    indicatorLight1.SetOff();
                }
            }
        }



        private IScalarFunction _scalarFunction;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IScalarFunction ScalarFunction
        {
            set {
                if (value != _scalarFunction)
                {
                    _scalarFunction = value;
                    IVectorFunction vecFunc = null;

                    vecFunc = new VectorFunctionFromScalar(new IScalarFunction[] {  value });

                    Function = vecFunc; 
                }
            }
        }

        private IVectorFunction _function;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IVectorFunction Function
        {
            get { return _function; }
            set {
                if (!object.ReferenceEquals(value, _function))
                {
                    _function = value;

                    if (_function != null && _neuralDataDefinition != null)
                    {
                        _function.SetNumParameters(_neuralDataDefinition.InputLength);
                        _function.SetNumValues(_neuralDataDefinition.OutputLength);
                    }

                    indicatorLight1.SetOff();
                }
            }
        }
        

        #endregion CommonData


        /// <summary>Calculates the specified output value of the vector function at the specified parameters.</summary>
        /// <param name="parameters">Parameter values.</param>
        /// <param name="whichOutput">Specifies which output value (which element of the vector function) should be calculated.</param>
        /// <returns>The calculated vector function component.</returns>
        public virtual double CalculateOutput(IVector parameters, int whichOutput)
        {
            try {
                if (Function == null)
                {
                    if (_scalarFunction != null)
                    {
                        Reporter.ReportError("Vector function to be plotted is not set (but scalar function is!).");
                    } else
                        Reporter.ReportError("Vector function to be plotted is not set ()");
                    throw new InvalidOperationException("Function to be plotted is not specified, can not evaluate it.");
                } else
                    return Function.Value(parameters, whichOutput);
            }
            catch(Exception ex)
            {
                Reporter.ReportError("Error occurred when evaluating the vector function.", ex);
                throw;
            }
        }


        //#region INeuralModelContainer

        //// This region contains a standard definition for INeuralModelContainer and INeuralModel on forms.

        //protected INeuralModelContainer _neuralModelContainerControl;

        //INeuralModel _neuralModel;

        ///// <summary>Neural network - based model.
        ///// <para>Contains data definition and trained neural network approximator.</para></summary>
        //public INeuralModel NeuralModel
        //{
        //    get
        //    {
        //        if (_neuralModel == null)
        //        {
        //            // ANN model has not been set; try to find it on a parent control:
        //            if (_neuralModelContainerControl != null)
        //            {
        //                if (_neuralModelContainerControl.NeuralModel != null)
        //                    return _neuralModelContainerControl.NeuralModel;
        //                else
        //                    _neuralModelContainerControl = null;
        //            }
        //            // A parent control containing ANN model is not specified, try to fint it:
        //            Control parentControl = this.Parent;
        //            while (parentControl != null && _neuralModelContainerControl == null)
        //            {
        //                INeuralModelContainer modelContainer = parentControl as INeuralModelContainer;
        //                if (modelContainer != null)
        //                {
        //                    if (modelContainer.NeuralModel != null)
        //                    {
        //                        _neuralModelContainerControl = modelContainer;
        //                        return NeuralModel;
        //                    }
        //                }
        //                parentControl = parentControl.Parent;
        //            }
        //        }
        //        if (_neuralModel == null)
        //            throw new InvalidOperationException("ANN - based model is not defined.");
        //        return _neuralModel;
        //    }
        //    protected set
        //    {
        //        if (value != _neuralModel)
        //        {
        //            _neuralModelContainerControl = null;
        //            _neuralModel = value;
        //            // Control-specific stuff:
        //            if (_neuralModel != null)
        //            {
        //                neuralInputControl1.SetNeuralModel(_neuralModel);
        //                neuralInputParameterSelector1.SetNeuralModel(_neuralModel);
        //                neuralOutputValueSelector1.SetNeuralModel(_neuralModel);


        //            }
        //        }

        //    }
        //}

        ///// <summary>Sets the ANN-based model used by the current form.</summary>
        ///// <param name="model">ANN based model that is set.</param>
        ///// <remarks>Because of this dedicated method, the setter of the <see cref="NeuralModel"/> property
        ///// can be non-public.</remarks>
        //public void SetNeuralModel(INeuralModel model)
        //{ this.NeuralModel = model; }



        //#region INeuralModel

        ///// <summary>Traint artificial neural network.</summary>
        //public INeuralApproximator TrainedNetwork
        //{
        //    get { return NeuralModel.TrainedNetwork; }
        //}

        ///// <summary>Neural data definition.</summary>
        //public InputOutputDataDefiniton NeuralDataDefinition
        //{
        //    get { return NeuralModel.NeuralDataDefinition; }
        //}

        //#endregion INeuralModel


        //#endregion INeuralModelContainer




        #region Data


        /// <summary>Gets number of input parameters.</summary>
        public int NumInputParameters
        {
            get
            {
                if (NeuralDataDefinition == null)
                {
                    Reporter.ReportError("Input / output data definition is invalid (null reference)."
                        + Environment.NewLine + "  Number of parameters is set to 0.");
                    return 0;
                }
                return NeuralDataDefinition.InputLength;
            }
        }

        /// <summary>Gets number of output values.</summary>
        public int NumOutputValues
        {
            get
            {
                if (NeuralDataDefinition == null)
                {
                    Reporter.ReportError("Input / output data definition is invalid (null reference)."
                        + Environment.NewLine + "  Number of output values is set to 0.");
                    return 0;
                }
                return NeuralDataDefinition.OutputLength;
            }
        }


        // From input parameters form:

        /// <summary>Vector of current values of input parameters as defined by the DadaGridView.</summary>
        public IVector ParameterValues
        {
            get
            {
                return neuralInputControl1.Values;
            }
        }

        // From input parameter selector:

        /// <summary>Minimal values of parameters.</summary>
        public double[] MinValues
        {
            get {
                return neuralInputParameterSelector1.MinValues;
            }
        }

        /// <summary>Maximal values of parameters.</summary>
        public double[] MaxValues
        {
            get
            {
                return neuralInputParameterSelector1.MaxValues;
            }
        }

        protected int _selectedParameterId = 0;


        /// <summary>Sequential number of the selected parameter as specified by the user.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedParameterId
        {
            get {
                return neuralInputParameterSelector1.SelectedParameterId;
            }
            protected set { neuralInputParameterSelector1.SetSelectedParameterId(value); }
        }

        
        /// <summary>Minimal value of the selected parameter.</summary>
        public double SelectedParameterMin
        {
            get
            {
                return neuralInputParameterSelector1.SelectedParameterMin;
            }
        }

        /// <summary>Maximal value of the selected parameter as specified by the user.</summary>
        public double SelectedParameterMax
        {
            get
            {
                return neuralInputParameterSelector1.SelectedParameterMax;
            }
        }



        // From output value selector:

        protected int _selectedOutputId = 0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedOutputId
        {
            get { 
                // return _selectedOutputId; 
                return neuralOutputValueSelector1.SelectedOutputId;
            }
            protected set { 
                // _selectedOutputId = value; 
                neuralOutputValueSelector1.SetSelectedOutputId(value);
            }
        }

        // Auxiliary for plotting:


        protected int _numPoints = 50;

        /// <summary>Number of plotting points.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NumPlotPoints
        {
            get { return _numPoints; }
            set
            {
                if (value != _numPoints)
                {
                    if (value < 2)
                        throw new ArgumentException("Number of plotting points should not be less than 2, specified: " 
                            + value + ".");
                    _numPoints = value;
                    this.numNumPlotPoints.Value = NumPlotPoints;
                    if (PlotImmediately)
                        PlotParametricTest();
                }
            }
        }


        /// <summary>Gets axis label text for the currently selected parameter.</summary>
        public string SelectedParameterLabelText
        {
            get
            {
                return UtilResponseForms.SelectedParameterLabelText(NeuralDataDefinition, SelectedParameterId);
            }
        }

        /// <summary>Gets axis label text for the currently selected output value.</summary>
        public string SelectedOutputLabelText
        {
            get
            {
                return UtilResponseForms.SelectedOutputLabelText(NeuralDataDefinition, SelectedOutputId);
            }
        }

        
        #endregion Data




        #region Plotting


        bool _plotImmediately = false;

        /// <summary>If true then graph is pletted immediately when any parameter changes that affects it appearance.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool PlotImmediately
        {
            get { return _plotImmediately; }
            set {
                if (value != _plotImmediately)
                {
                    _plotImmediately = value;
                    chkPlotImmediately.Checked = value;
                    if (_plotImmediately)
                        PlotParametricTest();
                }
            }
        }


        private bool _plotImmediatelyOnNumPointsValueChanged = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool PlotImmediatelyOnNumPointsValueChanged
        {
            get { return _plotImmediatelyOnNumPointsValueChanged; }
            set { _plotImmediatelyOnNumPointsValueChanged = value; }
        }


        PlotterZedGraph _plotter;

        /// <summary>Plotter that is used for plotting the graphs.</summary>
        public PlotterZedGraph Plotter
        {
            get
            {

                if (_plotter == null)
                    _plotter = PlotterZedGraph.CreateDefault(ParmTestFGraph);
                return _plotter;
            }
        }


        /// <summary>Applies style and other settings to the plotter used to show the parametric graph.</summary>
        /// <param name="plotter">Plotter to which settings are applied.</param>
        /// <param name="whichInput">ID of the input to be plotted.</param>
        /// <param name="whichOutpput">ID of the output to be plotted.</param>
        public void ApplyPlotterSettingsDefault(PlotterZedGraph plotter, int whichInput, int numInputs, int whichOutput, int numOutpts)
        {
            plotter.IsShowPointValues = true;
            plotter.XAxisLabel = SelectedParameterLabelText;
            plotter.YAxisLabel = SelectedOutputLabelText;
            plotter.XAxisLabel = NeuralDataDefinition.InputElementList[whichInput].Title;
            plotter.YAxisLabel = NeuralDataDefinition.OutputElementList[whichOutput].Title;
        }


        /// <summary>Applies style and other settings to the plotter used to show the parametric graph.</summary>
        /// <param name="plotter">Plotter to which settings are applied.</param>
        /// <param name="whichInput">ID of the input to be plotted.</param>
        /// <param name="whichOutpput">ID of the output to be plotted.</param>
        public void ApplyPlotCurveSettingsDefault(PlotZedgraphCurve plot, int whichInput, int numInputs, int whichOutput, int numOutpts)
        {
            plot.PointFillColor = Color.White;
            plot.PointsVisible = true;
            plot.PointBorderColor = Color.Blue;
            plot.LineColor = Color.Red;
            plot.LineWidth = 2;

            // plot.LegendString = "sin(x-2*Pi*" + i + "/" + numCurves + ")";
        }



        /// <summary>plots the selected outpt dependend on the selected parameter.</summary>
        public void PlotParametricTest()
        {
            if (NeuralDataDefinition == null)
            {
                indicatorLight1.SetOff();
                indicatorLight1.BlinkError(3);
                // Reporter.ReportError("Can not perform plotting: definition data is not specified (null reference).");
            } else if (Function == null)
            {
                indicatorLight1.SetOff();
                indicatorLight1.BlinkError(3);
                // Reporter.ReportError("Can not perform plotting: function to be plotted is not specified (null reference).");
            } else
            {
                try
                {

                    indicatorLight1.SetBusy();

                    double minParam = SelectedParameterMin;
                    double maxParam = SelectedParameterMax;
                    int whichInput = SelectedParameterId;
                    int whichOutput = SelectedOutputId;
                    double step = (maxParam - minParam) / (double)(NumPlotPoints - 1);
                    Plotter.RemoveAllPlotObjects();

                    ApplyPlotterSettingsDefault(Plotter, SelectedParameterId, NumInputParameters, SelectedOutputId, NumOutputValues);

                    PlotZedgraphCurve plot = new PlotZedgraphCurve(Plotter);

                    ApplyPlotCurveSettingsDefault(plot, SelectedParameterId, NumInputParameters, SelectedOutputId, NumOutputValues);

                    IVector parameters = new Vector(NumInputParameters);
                    VectorBase.CopyPlain(ParameterValues, parameters);
                    for (int j = 0; j < NumPlotPoints; ++j)
                    {
                        double param = minParam + j * step;
                        parameters[whichInput] = param;
                        double value = CalculateOutput(parameters, whichOutput);
                        plot.AddPoint(param, value);
                    }

                    Plotter.Update();
                    Plotter.ResetView();

                    indicatorLight1.SetOk();

                }
                catch(Exception ex)
                {
                    indicatorLight1.SetOk();
                    indicatorLight1.BlinkError(3);
                    Reporter.ReportError("Plotting error.", ex);
                    throw;
                }
            }
            // win.ShowDialog();
        }


        #endregion Plotting

        //private void TestX()
        //{
        //    // Test difference between ordinary delegate variables and events:
        //    // Ordinary delegates can be invoked from outside of the defining class while 
        //    // events can be raised only from the defining class!
        //    if (neuralOutputValueSelector1.SelectedOutputIdChanged != null)
        //        neuralOutputValueSelector1.SelectedOutputIdChanged(0,1);
        //}


        /// <summary>Plots the parametric test.</summary>
        private void btnParmTestStart_Click(object sender, EventArgs e)
        {
            PlotParametricTest();
        }



        /// <summary>Executes when ID of the selected input parameter changes; Plots the parametric test.</summary>
        /// <param name="o">Control that raised the event.</param>
        /// <param name="e">Event arguments containing old and new value.</param>
        private void neuralInputParameterSelector1_SelectedParameterIdChanged(object o, IndexChangeEventArgs e)
        {
            if (PlotImmediately)
                PlotParametricTest();
            indicatorLight1.BlinkSpecial(Color.Orange, 2);
        }

        ///// <summary>Executes when ID of the selected input parameter changes; Plots the parametric test.</summary>
        ///// <param name="oldId">Old ID of the selected output value (after change).</param>
        ///// <param name="newId">New ID of the selected output value (after changed).</param>
        //private void neuralInputParameterSelector1_SelectedParameterIdChanged(int oldId, int newId)
        //{
        //    if (PlotImmediately)
        //        PlotParametricTest();
        //}

        /// <summary>Executes when minimal value of the selected parameter changes by user interaction; Plots the parametric test.</summary>
        /// <param name="sender">Control that generated the event.</param>
        /// <param name="e">Event arrguments containing old and new value.</param>
        private void neuralInputParameterSelector1_SelectedParameterMinChanged(object sender, IG.Forms.ValueChangeEventArgs e)
        {
            if (PlotImmediately)
                PlotParametricTest();
            indicatorLight1.BlinkSpecial(Color.LightBlue, 2);
        }


       /// <summary>Executes when maximal value of the selected parameter changes by user interaction; Plots the parametric test.</summary>
        /// <param name="sender">Control that raised the event.</param>
        /// <param name="e">Event arguments containing the old and new value.</param>
        private void neuralInputParameterSelector1_SelectedParameterMaxChanged(object sender, ValueChangeEventArgs e)
        {
            if (PlotImmediately)
                PlotParametricTest();
            indicatorLight1.BlinkSpecial(Color.LightBlue, 2);
        }


        /// <summary>Executes when ID of the selected output value changes; Plots the parametric test.</summary>
        /// <param name="sender">Control that generated the event.</param>
        /// <param name="e">Event arguments, contain old and new value of the index.</param>
        private void neuralOutputValueSelector1_SelectedOutputIdChanged(object sender, IndexChangeEventArgs e)
        {
            if (PlotImmediately)
                PlotParametricTest();
            indicatorLight1.BlinkSpecial(Color.Yellow, 2);
        }
        


        /// <summary>Generates a test fuction to be plotted.</summary>
        private void btnTestFunction_Click(object sender, EventArgs e)
        {
            this.Function = new VectorFunctionExamples.RosenrockAndCircle();
            int numInputs = Function.NumParameters;
            int numOutputs = Function.NumValues;
            this.NeuralDataDefinition = InputOutputDataDefiniton.CreateDefault(numInputs, numOutputs);
        }

        /// <summary>Reports state of data in a fading message.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine().AppendLine("Plotting information:").AppendLine();
            if (this.NeuralDataDefinition == null)
                sb.AppendLine("Data definition object is not defined.").AppendLine();
            else
                sb.AppendLine("Data definition:").AppendLine(NeuralDataDefinition.ToString()).AppendLine();
            if (this._scalarFunction != null)
                sb.AppendLine("Scalar function has been set as plotted function.").AppendLine();

            FadingMessage msg = new FadingMessage(null, sb.ToString(), 6000, 0.3, false);
            msg.Launch(false /* launchInParallelThread */);
                
            // Reporter.ReportInfo(sb.ToString());
        }

        /// <summary>Report identity information of the running thread in a fading message.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIdentifyThread_Click(object sender, EventArgs e)
        {
            UtilForms.IdentifyCurrentThread(4000, true /* topLevel */);
        }

        //private void chkPlotImmediately_Click(object sender, EventArgs e)
        //{
            
        //}

        private void chkPlotImmediately_CheckedChanged(object sender, EventArgs e)
        {
            PlotImmediately = chkPlotImmediately.Checked;
        }

        private void numNumPlotPoints_Validated(object sender, EventArgs e)
        {
            NumPlotPoints = (int) numNumPlotPoints.Value;
        }

        private void numNumPlotPoints_ValueChanged(object sender, EventArgs e)
        {
            if (PlotImmediatelyOnNumPointsValueChanged)
            {
                // Remark: If the line below is executed then graph will be redrawn on everry change of 
                // NumericUpDown value, even before focus goes out.
                NumPlotPoints = (int)numNumPlotPoints.Value;
            }
        }


    } // class
}
