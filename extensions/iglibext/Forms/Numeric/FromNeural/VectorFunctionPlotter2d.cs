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
using IG.Gr3d;

using IG.Forms;

using ZedGraph;

using Color = System.Drawing.Color;
using DashStyle = System.Drawing.Drawing2D.DashStyle;


namespace IG.Forms
{

    /// <summary>Form for simple parametric tests (variation of selected parameter) performed on ANN models.</summary>
    /// $A Igor Jun13;
    public partial class VectorFunctionPlotter2d : UserControl  // , INeuralModelContainer
    {

        ///// <summary>Constructs the control, with ANN-based model specified.</summary>
        ///// <param name="neuralModel">ANN-based model, containing data definitions and trained neural network.</param>
        //public VectorFunctionPlotter2d(INeuralModel neuralModel): this()
        //{
        //    this.NeuralModel = NeuralModel;
        //}

        public VectorFunctionPlotter2d()
        {
            InitializeComponent();

            chkPlotImmediately.Checked = PlotImmediately;
            numNumPlotPoints1.Value = NumPlotPoints1;
            numNumPlotPoints2.Value = NumPlotPoints2;
        }



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
                    neuralInputSelector1.DataDefinition = value;
                    neuralInputSelector2.DataDefinition = value;

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
            set
            {
                if (value != _scalarFunction)
                {
                    _scalarFunction = value;
                    IVectorFunction vecFunc = null;

                    vecFunc = new VectorFunctionFromScalar(new IScalarFunction[] { value });

                    Function = vecFunc;
                }
            }
        }

        private IVectorFunction _function;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IVectorFunction Function
        {
            get { return _function; }
            set
            {
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
            try
            {
                if (Function == null)
                {
                    if (_scalarFunction != null)
                    {
                        Reporter.ReportError("Vector function to be plotted is not set (but scalar function is!).");
                    }
                    else
                        Reporter.ReportError("Vector function to be plotted is not set ()");
                    throw new InvalidOperationException("Function to be plotted is not specified, can not evaluate it.");
                }
                else
                    return Function.Value(parameters, whichOutput);
            }
            catch (Exception ex)
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
        //                neuralInputSelector1.SetNeuralModel(_neuralModel);
        //                neuralInputSelector2.SetNeuralModel(_neuralModel);
        //                neuralOutputValueSelector1.SetNeuralModel(_neuralModel);

        //                SelectedParameterId1 = 0;
        //                SelectedParameterId2 = 1;


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

        // From input parameter selectors:

        /// <summary>Minimal values of parameters.</summary>
        public double[] MinValues
        {
            get {
                return neuralInputSelector1.MinValues;
            }
        }

        /// <summary>Maximal values of parameters.</summary>
        public double[] MaxValues
        {
            get
            {
                return neuralInputSelector1.MaxValues;
            }
        }


        protected int _selectedParameterId1 = 0;

        /// <summary>Sequential number of the selected first parameter as specified by the user.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedParameterId1
        {
            get {
                return neuralInputSelector1.SelectedParameterId;
            }
            protected set { neuralInputSelector1.SetSelectedParameterId(value); }
        }

        
        /// <summary>Minimal value of the selected first parameter.</summary>
        public double SelectedParameterMin1
        {
            get
            {
                return neuralInputSelector1.SelectedParameterMin;
            }
        }

        /// <summary>Maximal value of the selected first parameter as specified by the user.</summary>
        public double SelectedParameterMax1
        {
            get
            {
                return neuralInputSelector1.SelectedParameterMax;
            }
        }


        protected int _selectedParameterId2 = 0;

        /// <summary>Sequential number of the selected second parameter as specified by the user.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedParameterId2
        {
            get
            {
                return neuralInputSelector2.SelectedParameterId;
            }
            protected set { neuralInputSelector2.SetSelectedParameterId(value); }
        }


        /// <summary>Minimal value of the selected second parameter.</summary>
        public double SelectedParameterMin2
        {
            get
            {
                return neuralInputSelector2.SelectedParameterMin;
            }
        }

        /// <summary>Maximal value of the selected second parameter as specified by the user.</summary>
        public double SelectedParameterMax2
        {
            get
            {
                return neuralInputSelector2.SelectedParameterMax;
            }
        }

        public static string SelectedParametersIntroStr = "Selected input parameters: ";

        protected void UpdateSeclectedParametersDependencies()
        {
            lblSelectedParameters.Text = SelectedParametersIntroStr + SelectedParameterId1.ToString() 
                + ", " + SelectedParameterId2.ToString() + ".";
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


        protected int _numPoints1 = 80;

        /// <summary>Number of plotting points in the 1st direction.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NumPlotPoints1
        {
            get { return _numPoints1; }
            set
            {
                if (value != _numPoints1)
                {
                    if (value < 2)
                        throw new ArgumentException("Number of plotting points should not be less than 2, specified: "
                            + value + ".");
                    _numPoints1 = value;
                    this.numNumPlotPoints1.Value = value;
                    if (PlotImmediately)
                    {
                        PlotParametricTest1d();
                        if (PlotImmediately2dAlso)
                            PlotParametricTest();
                    }
                }
            }
        }

        protected int _numPoints2 = 80;

        /// <summary>Number of plotting points in the 2nd direction.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NumPlotPoints2
        {
            get { return _numPoints2; }
            set
            {
                if (value != _numPoints2)
                {
                    if (value < 2)
                        throw new ArgumentException("Number of plotting points should not be less than 2, specified: "
                            + value + ".");
                    _numPoints2 = value;
                    this.numNumPlotPoints2.Value = value;
                    if (PlotImmediately)
                    {
                        PlotParametricTest1d();
                        if (PlotImmediately2dAlso)
                            PlotParametricTest();
                    }
                }
            }
        }



        /// <summary>Gets axis label text for the currently selected parameter.</summary>
        public string SelectedParameterLabelText
        {
            get
            {
                return UtilResponseForms.SelectedParameterLabelText(NeuralDataDefinition, SelectedParameterId1);
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


        #region Plotting_1D


        PlotterZedGraph _plotterZedgraph;

        /// <summary>Plotter that is used for plotting the graphs.</summary>
        public PlotterZedGraph Plotterzedgraph
        {
            get
            {

                if (_plotterZedgraph == null)
                    _plotterZedgraph = PlotterZedGraph.CreateDefault(ParmTestFGraph);
                return _plotterZedgraph;
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
        public void PlotParametricTest1d()
        {
            double minParam = SelectedParameterMin1;
            double maxParam = SelectedParameterMax1;
            int numPoints = 100;
            numPoints = (int)numNumPlotPoints1.Value;
            int whichInput = SelectedParameterId1;
            int whichOutput = SelectedOutputId;
            double step = (maxParam - minParam) / (double)(numPoints - 1);
            Plotterzedgraph.RemoveAllPlotObjects();

            ApplyPlotterSettingsDefault(Plotterzedgraph, SelectedParameterId1, NumInputParameters, SelectedOutputId, NumOutputValues);

            PlotZedgraphCurve plot = new PlotZedgraphCurve(Plotterzedgraph);

            ApplyPlotCurveSettingsDefault(plot, SelectedParameterId1, NumInputParameters, SelectedOutputId, NumOutputValues);

            IVector parameters = new Vector(NumInputParameters);
            VectorBase.CopyPlain(ParameterValues, parameters);
            for (int j = 0; j < numPoints; ++j)
            {
                double param = minParam + j * step;
                parameters[whichInput] = param;
                double value = CalculateOutput(parameters, whichOutput); 
                plot.AddPoint(param, value);
            }


            Plotterzedgraph.Update();
            Plotterzedgraph.ResetView();

            // win.ShowDialog();
        }




        ///// <summary>plots the selected outpt dependend on the selected parameter.</summary>
        //public void PlotParametricTestZedGraph()
        //{
        //    double minParam = SelectedParameterMin1;
        //    double maxParam = SelectedParameterMax1;
        //    int numPoints = 100;
        //    numPoints = (int) txtNumPoints1.Value;
        //    int whichInput = SelectedParameterId1;
        //    int whichOutput = SelectedOutputId;
        //    double step = (maxParam - minParam) / (double)(numPoints - 1);
        //    Plotterzedgraph.RemoveAllPlotObjects();

        //    ApplyPlotterSettingsDefault(Plotterzedgraph, SelectedParameterId1, NumInputParameters, SelectedOutputId, NumOutputValues);

        //    PlotZedgraphCurve plot = new PlotZedgraphCurve(Plotterzedgraph);

        //    ApplyPlotCurveSettingsDefault(plot, SelectedParameterId1, NumInputParameters, SelectedOutputId, NumOutputValues);

        //    IVector parameters = new Vector(NumInputParameters);
        //    VectorBase.CopyPlain(ParameterValues, parameters);
        //    for (int j = 0; j < numPoints; ++j)
        //    {
        //        double param = minParam + j * step;
        //        parameters[whichInput] = param;
        //        double value = TrainedNetwork.CalculateOutput(parameters, whichOutput);
        //        plot.AddPoint(param, value);
        //    }


        //    Plotterzedgraph.Update();
        //    Plotterzedgraph.ResetView();

        //    // win.ShowDialog();
        //}


        #endregion Plotting_1D


        #region Plotting


        bool _plotImmediately = false;

        /// <summary>If true then graph is pletted immediately when any parameter changes that affects it appearance.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool PlotImmediately
        {
            get { return _plotImmediately; }
            set
            {
                if (value != _plotImmediately)
                {
                    _plotImmediately = value;
                    chkPlotImmediately.Checked = value;
                    if (_plotImmediately)
                    {
                        PlotParametricTest1d();
                        PlotParametricTest();
                    }
                }
            }
        }


        private bool _plotImmediately2dAlso = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool PlotImmediately2dAlso
        {
            get { return _plotImmediately2dAlso; }
            set { _plotImmediately2dAlso = value; }
        }


        private bool _plotImmediatelyOnNumPointsValueChanged = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool PlotImmediatelyOnNumPointsValueChanged
        {
            get { return _plotImmediatelyOnNumPointsValueChanged; }
            set { _plotImmediatelyOnNumPointsValueChanged = value; }
        }




        protected ColorScale SurfaceColorScale;

        protected BoundingBox3d OriginalBounds;


        /// <summary>Applies default settings to surface plots.</summary>
        /// <param name="plot">Surface plot to which settings are applied.</param>
        public virtual void ApplySurfacePlotSettingsDefault(VtkSurfacePlotBase plot)
        {

            plot.PointsVisible = false;
            plot.LinesVisible = true;
            plot.SurfacesVisible = true;
            plot.LineColor = System.Drawing.Color.Blue;
            plot.LineWidth = 0;
            plot.SurfaceColorIsScaled = true;
            plot.SurfaceColor = System.Drawing.Color.LightGreen;
             plot.SurfaceColorScale = this.SurfaceColorScale;  // this is set in the plotting method
            plot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent
        }


        /// <summary>Applies default settings to plotter used to render 3D graphs.</summary>
        /// <param name="plotter">3D Plotter to which settings are applied.</param>
        public virtual void ApplySurfacePlottterSettingsDefault(VtkPlotter plotter, int whichInput1, int whichInput2, int whichOutput)
        {
            plotter.WindowSizeX = 1000;
            plotter.WindowSizeY = 800;
            plotter.BackGround = System.Drawing.Color.White;
            //plotter2D.DecorationHandler.SetActorScale(1, 1, 0.1);
            // plotter.DecorationHandler.
            plotter.DecorationHandler.CubeAxesFlyMode = VtkFlyMode.OuterEdges;

            string xAxisLabel = null;
            string yAxisLabel = null;
            string zAxisLabel = null;

            xAxisLabel = NeuralDataDefinition.InputElementList[whichInput1].Title;
            yAxisLabel = NeuralDataDefinition.InputElementList[whichInput2].Title;
            zAxisLabel = NeuralDataDefinition.OutputElementList[whichOutput].Title;
            if (string.IsNullOrEmpty(xAxisLabel))
                xAxisLabel = NeuralDataDefinition.InputElementList[whichInput1].Name;
            if (string.IsNullOrEmpty(yAxisLabel))
                yAxisLabel = NeuralDataDefinition.InputElementList[whichInput2].Name;
            if (string.IsNullOrEmpty(zAxisLabel))
                zAxisLabel = NeuralDataDefinition.OutputElementList[whichOutput].Name;
            if (string.IsNullOrEmpty(xAxisLabel))
                xAxisLabel = "Parameter No. " + whichInput1;
            if (string.IsNullOrEmpty(yAxisLabel))
                yAxisLabel = "Parameter No. " + whichInput2;
            if (string.IsNullOrEmpty(zAxisLabel))
                zAxisLabel = "Output No. " + whichOutput;
            if (chkScaled.Checked)
            {
                xAxisLabel += " - scaled";
                yAxisLabel += " - scaled";
                zAxisLabel += " - scaled";
            }
            plotter.DecorationHandler.CubeAxesXLabel = xAxisLabel;
            plotter.DecorationHandler.CubeAxesYLabel = yAxisLabel;
            plotter.DecorationHandler.CubeAxesZLabel = zAxisLabel;

            plotter.DecorationHandler.ShowCubeAxes = true;

            // Scalar bar (color legend) settings:
            plotter.DecorationHandler.ScalarBarTitle = "Color scale legend";
            plotter.DecorationHandler.ScalarBarNumberOfLabels = 6;
            plotter.DecorationHandler.LookUpTableMinRange = OriginalBounds.MinZ; // SurfaceColorScale.MinValue;
            plotter.DecorationHandler.LookUpTableMaxRange = OriginalBounds.MaxZ; // SurfaceColorScale.MaxValue;
            plotter.DecorationHandler.LookUpTableColorScale = SurfaceColorScale; // new ColorScale(minRange, maxRange, Color.Blue, Color.DarkRed, Color.Orange);
            plotter.DecorationHandler.ShowScalarBar = true;

        }


        /// <summary>plots the selected outpt dependend on the selected parameter.</summary>
        public void PlotParametricTest()
        {

            if (NeuralDataDefinition == null)
            {
                indicatorLight1.SetOff();
                indicatorLight1.BlinkError(3);
                // Reporter.ReportError("Can not perform plotting: definition data is not specified (null reference).");
            }
            else if (Function == null)
            {
                indicatorLight1.SetOff();
                indicatorLight1.BlinkError(3);
                // Reporter.ReportError("Can not perform plotting: function to be plotted is not specified (null reference).");
            }
            else
            {
                try
                {

                    indicatorLight1.SetBusy();

                    double minParam1 = SelectedParameterMin1;
                    double maxParam1 = SelectedParameterMax1;
                    int numPoints1 = 100;
                    numPoints1 = (int)numNumPlotPoints1.Value;
                    int whichInput1 = SelectedParameterId1;
                    double minParam2 = SelectedParameterMin2;
                    double maxParam2 = SelectedParameterMax2;
                    int numPoints2 = 100;
                    numPoints2 = (int)numNumPlotPoints2.Value;
                    int whichInput2 = SelectedParameterId2;
                    int whichOutput = SelectedOutputId;
                    double step1 = (maxParam1 - minParam1) / (double)(numPoints1 - 1);
                    double step2 = (maxParam2 - minParam2) / (double)(numPoints2 - 1);


                    // Create a mesh for the surface plot:
                    StructuredMesh2d3d mesh = new StructuredMesh2d3d(numPoints1, numPoints2);
                    // Vector of current parameters used in calculation of output value:
                    IVector parameters = new Vector(NumInputParameters);
                    VectorBase.CopyPlain(ParameterValues, parameters);
                    for (int i = 0; i < numPoints1; ++i)
                    {
                        double x = minParam1 + (double)i * step1; ;
                        for (int j = 0; j < numPoints2; ++j)
                        {
                            double y = minParam2 + (double)j * step2;
                            parameters[whichInput1] = x;
                            parameters[whichInput2] = y;
                            // double z = TrainedNetwork.CalculateOutput(parameters, whichOutput);
                            double z = CalculateOutput(parameters, whichOutput);
                            mesh[i, j] = new vec3(
                                x,
                                y,
                                z);
                        }
                    }

                    if (chkScaled.Checked)
                    {
                        // Scale the mesh (all coordinates to [0, 1]):
                        // Get bounds of the original mesh:
                        OriginalBounds = new BoundingBox3d();
                        OriginalBounds.Update(mesh.Coordinates);
                        // Set new bounds:
                        BoundingBox3d scaledBounds = new BoundingBox3d();
                        scaledBounds.Min = new Vector3d(0, 0, 0);
                        scaledBounds.Max = new Vector3d(1, 1, 1);
                        // Transform coordinates of the mesh:
                        Vector3d coord = new Vector3d(0, 0, 0);
                        for (int i = 0; i < mesh.Length; ++i)
                        {
                            coord.Vec = mesh.Coordinates[i];
                            IVector result = coord;
                            BoundingBox3d.Map(OriginalBounds, scaledBounds, coord, ref result);
                            mesh.Coordinates[i] = coord.Vec;
                        }
                        // Print original and scaled bounds to the console:
                        Console.WriteLine("Manual surface scaled." + Environment.NewLine
                            + "Original mesh bounds: " + Environment.NewLine
                            + "  " + OriginalBounds.ToString() + " " + Environment.NewLine
                            + "Scaled mesh bounds: " + Environment.NewLine
                            + "  " + scaledBounds.ToString() + " " + Environment.NewLine
                            + Environment.NewLine);
                    }

                    // Plot the mesh surface:
                    VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
                    VtkSurfacePlot plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object
                    plot.OutputLevel = 1;  // print to console what's going on

                    plot.ClearSurfaceDefinition();  // Surface is not defined by functions
                    plot.Mesh = mesh; // assign the scaled mesh to the plot

                    // Define the color scale:
                    this.SurfaceColorScale = new ColorScale(0, 1, Color.Blue, Color.Green, Color.Red);

                    // Apply plot and plotter settings:
                    ApplySurfacePlotSettingsDefault(plot);
                    ApplySurfacePlottterSettingsDefault(plotter, whichInput1, whichInput2, whichOutput);

                    plot.Create();
                    plotter.ResetCamera();

                    // Now show all the plots that were created by the plotter; method with the same name is 
                    // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):
                    plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

                    indicatorLight1.SetOk();
                }
                catch (Exception ex)
                {
                    indicatorLight1.SetOk();
                    indicatorLight1.BlinkError(3);
                    Reporter.ReportError("Plotting error.", ex);
                    indicatorLight1.SetOk();
                    throw;
                }
                finally
                {
                }
            }

        }  // PlotParametric


        ///// <summary>plots the selected outpt dependend on the selected parameter.</summary>
        //public void PlotParametricTestOld()
        //{
        //    double minParam1 = SelectedParameterMin1;
        //    double maxParam1 = SelectedParameterMax1;
        //    int numPoints1 = 100;
        //    numPoints1 = (int)txtNumPoints1.Value;
        //    int whichInput1 = SelectedParameterId1;
        //    double minParam2 = SelectedParameterMin2;
        //    double maxParam2 = SelectedParameterMax2;
        //    int numPoints2 = 100;
        //    numPoints2 = (int)txtNumPoints2.Value;
        //    int whichInput2 = SelectedParameterId2;
        //    int whichOutput = SelectedOutputId;
        //    double step1 = (maxParam1 - minParam1) / (double)(numPoints1 - 1);
        //    double step2 = (maxParam2 - minParam2) / (double)(numPoints2 - 1);


        //    // Create a mesh for the surface plot:
        //    StructuredMesh2d3d mesh = new StructuredMesh2d3d(numPoints1, numPoints2);
        //    // Vector of current parameters used in calculation of output value:
        //    IVector parameters = new Vector(NumInputParameters);
        //    VectorBase.CopyPlain(ParameterValues, parameters);
        //    for (int i = 0; i < numPoints1; ++i)
        //    {
        //        double x = minParam1 + (double)i * step1; ;
        //        for (int j = 0; j < numPoints2; ++j)
        //        {
        //            double y = minParam2 + (double)j * step2;
        //            parameters[whichInput1] = x;
        //            parameters[whichInput2] = y;
        //            double z = TrainedNetwork.CalculateOutput(parameters, whichOutput);
        //            mesh[i, j] = new vec3(
        //                x,
        //                y,
        //                z);
        //        }
        //    }

        //    if (chkScaled.Checked)
        //    {
        //        // Scale the mesh (all coordinates to [0, 1]):
        //        // Get bounds of the original mesh:
        //        OriginalBounds = new BoundingBox3d();
        //        OriginalBounds.Update(mesh.Coordinates);
        //        // Set new bounds:
        //        BoundingBox3d scaledBounds = new BoundingBox3d();
        //        scaledBounds.Min = new Vector3d(0, 0, 0);
        //        scaledBounds.Max = new Vector3d(1, 1, 1);
        //        // Transform coordinates of the mesh:
        //        Vector3d coord = new Vector3d(0, 0, 0);
        //        for (int i = 0; i < mesh.Length; ++i)
        //        {
        //            coord.Vec = mesh.Coordinates[i];
        //            IVector result = coord;
        //            BoundingBox3d.Map(OriginalBounds, scaledBounds, coord, ref result);
        //            mesh.Coordinates[i] = coord.Vec;
        //        }
        //        // Print original and scaled bounds to the console:
        //        Console.WriteLine("Manual surface scaled." + Environment.NewLine
        //            + "Original mesh bounds: " + Environment.NewLine
        //            + "  " + OriginalBounds.ToString() + " " + Environment.NewLine
        //            + "Scaled mesh bounds: " + Environment.NewLine
        //            + "  " + scaledBounds.ToString() + " " + Environment.NewLine
        //            + Environment.NewLine);
        //    }

        //    // Plot the mesh surface:
        //    VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
        //    VtkSurfacePlot plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object
        //    plot.OutputLevel = 1;  // print to console what's going on

        //    plot.ClearSurfaceDefinition();  // Surface is not defined by functions
        //    plot.Mesh = mesh; // assign the scaled mesh to the plot

        //    // Define the color scale:
        //    this.SurfaceColorScale = new ColorScale(0, 1, Color.Blue, Color.Green, Color.Red);

        //    // Apply plot and plotter settings:
        //    ApplySurfacePlotSettingsDefault(plot);
        //    ApplySurfacePlottterSettingsDefault(plotter, whichInput1, whichInput2, whichOutput);

        //    plot.Create();
        //    plotter.ResetCamera();
        //    // Now show all the plots that were created by the plotter; method with the same name is 
        //    // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):
        //    plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

        //}


        #endregion Plotting





        /// <summary>Executes when ID of the selected first input parameter changes; Plots the parametric test.</summary>
        /// <param name="sender">Control that generated the event.</param>
        /// <param name="e">Event arguments that contain the old and new value.</param>
        private void neuralInputSelector1_SelectedParameterIdChanged(object sender, IndexChangeEventArgs e)
        {
            if (neuralInputSelector1.SelectedParameterId > NumInputParameters - 2)
            {
                neuralInputSelector1.SetSelectedParameterId(NumInputParameters - 2);
            } else
            {
                if (neuralInputSelector2.SelectedParameterId <= neuralInputSelector1.SelectedParameterId)
                {
                    neuralInputSelector2.SetSelectedParameterId(neuralInputSelector1.SelectedParameterId + 1);
                } else
                {
                    UpdateSeclectedParametersDependencies();
                }
            }
            if (PlotImmediately)
            {
                PlotParametricTest1d();
                if (PlotImmediately2dAlso)
                    PlotParametricTest();
            }
            indicatorLight1.BlinkSpecial(Color.Orange, 2);
        }


        /// <summary>Executes when minimal value of the selected first parameter changes by user interaction; Plots the parametric test.</summary>
        /// <param name="sender">Control that generated the event.</param>
        /// <param name="e">Event arguments that contain the old and new value.</param>
        private void neuralInputSelector1_SelectedParameterMinChanged(object sender, ValueChangeEventArgs e)
        {
            if (PlotImmediately)
            {
                PlotParametricTest1d();
                if (PlotImmediately2dAlso)
                    PlotParametricTest();
            }
            indicatorLight1.BlinkSpecial(Color.LightBlue, 2);
        }

        /// <summary>Executes when maximal value of the selected first parameter changes by user interaction; Plots the parametric test.</summary>
        /// <param name="sender">Control that generated the event.</param>
        /// <param name="e">Event arguments that contain the old and new value.</param>
        private void neuralInputSelector1_SelectedParameterMaxChanged(object sender, ValueChangeEventArgs e)
        {
            if (PlotImmediately)
            {
                PlotParametricTest1d();
                if (PlotImmediately2dAlso)
                    PlotParametricTest();
            }
            indicatorLight1.BlinkSpecial(Color.LightBlue, 2);
        }

        /// <summary>Executes when ID of the selected second input parameter changes; Plots the parametric test.</summary>
        /// <param name="sender">Control that generated the event.</param>
        /// <param name="e">Event arguments that contain the old and new value.</param>
        private void neuralInputSelector2_SelectedParameterIdChanged(object sender, IndexChangeEventArgs e)
        {
            if (neuralInputSelector2.SelectedParameterId <= neuralInputSelector1.SelectedParameterId)
            {
                if (neuralInputSelector2.SelectedParameterId > 0)
                    neuralInputSelector1.SetSelectedParameterId(neuralInputSelector2.SelectedParameterId - 1);
                else
                    neuralInputSelector2.SetSelectedParameterId(neuralInputSelector1.SelectedParameterId + 1);
            } else
            {
                UpdateSeclectedParametersDependencies();
            }
            if (PlotImmediately)
            {
                PlotParametricTest1d();
                if (PlotImmediately2dAlso)
                    PlotParametricTest();
            }
            indicatorLight1.BlinkSpecial(Color.Orange, 2);
        }

        ///// <summary>Executes when ID of the selected second input parameter changes; Plots the parametric test.</summary>
        ///// <param name="oldId">Old ID of the selected output value (after change).</param>
        ///// <param name="newId">New ID of the selected output value (after changed).</param>
        //private void neuralInputSelector2_SelectedParameterIdChanged(int oldId, int newId)
        //{
        //    if (neuralInputSelector2.SelectedParameterId <= neuralInputSelector1.SelectedParameterId)
        //    {
        //        if (neuralInputSelector2.SelectedParameterId > 0)
        //            neuralInputSelector1.SetSelectedParameterId(neuralInputSelector2.SelectedParameterId - 1);
        //        else
        //            neuralInputSelector2.SetSelectedParameterId(neuralInputSelector1.SelectedParameterId + 1);
        //    } else
        //    {
        //        UpdateSeclectedParametersDependencies();
        //    }
        //    if (PlotImmediately)
        //    {
        //        PlotParametricTest1d();
        //        if (PlotImmediately2dAlso)
        //            PlotParametricTest();
        //    }
        //}

        /// <summary>Executes when minimal value of the selected second parameter changes by user interaction; Plots the parametric test.</summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments containing the old and new value.</param>
        private void neuralInputSelector2_SelectedParameterMinChanged(object sender, ValueChangeEventArgs e)
        {
            if (PlotImmediately)
            {
                PlotParametricTest1d();
                if (PlotImmediately2dAlso)
                    PlotParametricTest();
            }
            indicatorLight1.BlinkSpecial(Color.Magenta, 2);
        }


        /// <summary>Executes when maximal value of the selected second parameter changes by user interaction; Plots the parametric test.</summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments containing the old and new value.</param>
        private void neuralInputSelector2_SelectedParameterMaxChanged(object sender, ValueChangeEventArgs e)
        {
            if (PlotImmediately)
            {
                PlotParametricTest1d();
                if (PlotImmediately2dAlso)
                    PlotParametricTest();
            }
            indicatorLight1.BlinkSpecial(Color.Magenta, 2);
        }


        /// <summary>Executes when ID of the selected output value changes; Plots the parametric test.</summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Event arguments containing the old and new value.</param>
        private void neuralOutputValueSelector1_SelectedOutputIdChanged(object sender,IndexChangeEventArgs e)
        {
            if (PlotImmediately)
            {
                PlotParametricTest1d();
                if (PlotImmediately2dAlso)
                    PlotParametricTest();
            }
            indicatorLight1.BlinkSpecial(Color.Yellow, 2);
        }


        private void chkPlotImmediately_CheckedChanged(object sender, EventArgs e)
        {
            PlotImmediately = chkPlotImmediately.Checked;
        }



        private void numNumPlotPoints1_Validated(object sender, EventArgs e)
        {
            NumPlotPoints1 = (int)numNumPlotPoints1.Value;
        }


        private void numNumPlotPoints1_ValueChanged(object sender, EventArgs e)
        {
            if (PlotImmediatelyOnNumPointsValueChanged)
            {
                // Remark: If the line below is executed then graph will be redrawn on everry change of 
                // NumericUpDown value, even before focus goes out.
                NumPlotPoints1 = (int)numNumPlotPoints1.Value;
            }
        }


        private void numNumPlotPoints2_Validated(object sender, EventArgs e)
        {
            NumPlotPoints2 = (int)numNumPlotPoints2.Value;
        }


        private void numNumPlotPoints2_ValueChanged(object sender, EventArgs e)
        {
            if (PlotImmediatelyOnNumPointsValueChanged)
            {
                // Remark: If the line below is executed then graph will be redrawn on everry change of 
                // NumericUpDown value, even before focus goes out.
                NumPlotPoints2 = (int)numNumPlotPoints2.Value;
            }
        }


        /// <summary>Plots the parametric test.</summary>
        private void btnParmTestStart_Click(object sender, EventArgs e)
        {
            PlotParametricTest1d();
            PlotParametricTest();
        }



        private void btnIdentifyThread_Click(object sender, EventArgs e)
        {
            UtilForms.IdentifyCurrentThread(4000, true /* topLevel */);
        }

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

        private void btnTestFunction_Click(object sender, EventArgs e)
        {
            this.Function = new VectorFunctionExamples.RosenrockAndCircle();
            int numInputs = Function.NumParameters;
            int numOutputs = Function.NumValues;
            this.NeuralDataDefinition = InputOutputDataDefiniton.CreateDefault(numInputs, numOutputs);
        }



    } // class
}
