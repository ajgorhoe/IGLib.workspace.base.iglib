// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// TESTING SCRIPT FILE: output formats for numbers.
// Original filename: ScriptExtFormats

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Text;

using IG.Num;
using IG.Lib;

using IG.Gr3d;
using System.Drawing;

namespace IG.Script
{

    /// <summary>Base class for script classes with graphics examples.</summary>
    /// $A Igor xx Jul12;
    public class ScriptGraphics3DBase : LoadableScriptBase, ILoadableScript
    {

        #region Standard_TestScripts

        public ScriptGraphics3DBase()
            : base()
        { }

        /// <summary>Initializes the current object.</summary>
        protected override void InitializeThis(string[] arguments)
        {
            Script_DefaultInitialize(arguments);
        }

        /// <summary>Runs action of the current object.</summary>
        /// <param name="arguments">Command-line arguments of the action.</param>
        protected override string RunThis(string[] arguments)
        {
            return Script_DefaultRun(arguments);
        }

        #endregion Standard_TestScripts

        #region Commands

        /// <summary>Name of the command that performs my custom test.</summary>
        public const string ConstMyTest = "MyTest";
        public const string ConstHelpMyTest = "Custom test function.";

        /// <summary>Name of the command for custom test.</summary>
        public const string ConstCustom = "Custom";
        public const string ConstHelpCustom = "Custom test.";

        /// <summary>Name of the command for VTK tests.</summary>
        public const string ConstVtkTest = "VtkTest";
        public const string ConstHelpVtkTest = "Various 3D tests using VTK. Run with ? argument to see which tests are available.";

        /// <summary>Name of the command for 3D plots tests.</summary>
        public const string ConstPlot3d = "Plot3d";
        public const string ConstHelpPlot3d = "Various higher level 3D plots. Run with ? argument to see which tests are available.";

        /// <summary>Name of the command for 3D surface examples.</summary>
        public const string ConstSurface3d = "Surface3D";
        public const string ConstHelpSurface3d = "Examples of 3D surfaces. Run with ? argument to see which surfaces are available.";

        #endregion Commands

        

        /// <summary>Adds commands to the internal interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);
            Script_AddCommand(interpreter, helpStrings, ConstMyTest, TestMyTest, ConstHelpMyTest);
            Script_AddCommand(interpreter, helpStrings, ConstCustom, TestCustom, ConstHelpCustom);
            Script_AddCommand(interpreter, helpStrings, ConstVtkTest, TestVtkTest, ConstHelpVtkTest);
            Script_AddCommand(interpreter, helpStrings, ConstPlot3d, TestPlot3d, ConstHelpPlot3d);
            Script_AddCommand(interpreter, helpStrings, ConstSurface3d, TestSurface3d, ConstHelpSurface3d);
        }


        #region Actions

        /// <summary>Test action.</summary>
        /// <param name="arguments">Array of command-line arguments.</param>
        public string TestMyTest(string[] arguments)
        {
            Console.WriteLine();
            Console.WriteLine("MY CUSTOM TEST.");
            Console.WriteLine("This script is alive.");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine("==== END of my custom test.");
            Console.WriteLine();
            return null;
        }

        /// <summary>Custom test.</summary>
        public virtual string TestCustom(string[] arguments)
        {
            string ret = null;
            Console.WriteLine();
            Console.WriteLine("CUSTOM TEST for 3D graphics scripts.");
            Console.WriteLine("==============================");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine();

            


            Console.WriteLine("==============================");
            Console.WriteLine("Custom test finished.");
            Console.WriteLine();
            return ret;
        }  // TestFormats


        #region Actions.VtkTests

        /// <summary>List of VTK test names.</summary>
        protected List<string> TestVtkNames = new List<string>();

        /// <summary>List of VTK tests' help strings.</summary>
        protected List<string> TestVtkHelpStrings = new List<string>();

        /// <summary>List of methods used to perform VTK tests.</summary>
        protected List<CommandMethod> TestVtkMethods = new List<CommandMethod>();

        /// <summary>Adds a new command testing VTK.</summary>
        /// <param name="testName">VTK test name.</param>
        /// <param name="surfaceMethod">Method used to perform the test.</param>
        /// <param name="surfaceHelp">Eventual help string for the test.</param>
        protected void AddTestVtkCommand(string testName, CommandMethod surfaceMethod, string surfaceHelp)
        {
            lock (Lock)
            {
                TestVtkNames.Add(testName.ToLower());
                TestVtkHelpStrings.Add(surfaceHelp);
                TestVtkMethods.Add(surfaceMethod);
            }
        }


        #region Actions.VtkTests.StructuredGrid

        public const string VtkStructuredGrid = "StructuredGrid";

        private const string VtkHelpStructuredGrid = VtkStructuredGrid + " <numX> <numY> <numZ> : Structured grid in 3D.";

        /// <summary>Demonstration of plotting structured grids.</summary>
        private string VtkFunctionStructuredGrid(string surfaceName, string[] args)
        {

            int sizeX = 20;
            int sizeY = 20;
            int sizeZ = 3;

            if (args.Length >= 1)
                sizeX = int.Parse(args[0]);
            if (args.Length >= 2)
                sizeY = int.Parse(args[1]);
            if (args.Length >= 3)
                sizeZ = int.Parse(args[2]);

            TestVtkGraphicBase.ExampleStructuredGrid(sizeX, sizeY, sizeZ);

            return null;
        }

        #endregion Actions.VtkTests.StructuredGrid


        #region Actions.VtkTests.QuadCells

        public const string VtkQuadCells = "QuadCells";

        private const string VtkHelpQuadCells = VtkQuadCells + " <numX> <numY> : Using simple primitives in 3D graphics.";

        /// <summary>Example that demonstrates the ability to use simple primitives for plotting surfaces in 3D
        /// (graphs of functions of 2 variables or parametric surfaces).</summary>
        private string VtkFunctionQuadCells(string surfaceName, string[] args)
        {

            int sizeX = 20;
            int sizeY = 20;

            if (args.Length >= 1)
                sizeX = int.Parse(args[0]);
            if (args.Length >= 2)
                sizeY = int.Parse(args[1]);

            TestVtkGraphicBase.ExampleQuadCells(sizeX, sizeY);

            return null;
        }

        #endregion Actions.VtkTests.QuadCells


        #region Actions.VtkTests.CellGridContours

        public const string VtkCellGridContours = "CellGridContours";

        private const string VtkHelpCellGridContours = VtkCellGridContours + " <numX> <numY> <numContours> : Cotours on 3D surfaces by using cells.";

        /// <summary>Example of plotting contours on surfaces in 3D (graphs of functions of 2 variables 
        /// or parametric surfaces) by using graphic primitives (cells) connected to polydata.</summary>
        private string VtkFunctionCellGridContours(string surfaceName, string[] args)
        {

            int sizeX = 20;
            int sizeY = 20;
            int numContours = 20;

            if (args.Length >= 1)
                sizeX = int.Parse(args[0]);
            if (args.Length >= 2)
                sizeY = int.Parse(args[1]);
            if (args.Length >= 3)
                numContours = int.Parse(args[2]);

            TestVtkGraphicBase.ExampleCellsGridContours(sizeX, sizeY, numContours);

            return null;
        }

        #endregion Actions.VtkTests.CellsGridContours


        #region Actions.VtkTests.StructuredGridVolumeContours

        public const string VtkStructuredGridVolumeContours = "StructuredGridVolumeContours";

        private const string VtkHelpStructuredGridVolumeContours = VtkStructuredGridVolumeContours + " <numX> <numY> <numZ> <numContours> : Using volume contorus with structured grid.";

        /// <summary>Example that demonstrates volume contours with structure grid.</summary>
        private string VtkFunctionStructuredGridVolumeContours(string surfaceName, string[] args)
        {

            int sizeX = 20;
            int sizeY = 20;
            int sizeZ = 20;
            int numContours = 4;

            if (args.Length >= 1)
                sizeX = int.Parse(args[0]);
            if (args.Length >= 2)
                sizeY = int.Parse(args[1]);
            if (args.Length >= 3)
                sizeZ = int.Parse(args[2]);
            if (args.Length >= 4)
                numContours = int.Parse(args[3]);

            TestVtkGraphicBase.ExampleStructuredGridVolumeContours(sizeX, sizeY, sizeZ, numContours);

            return null;
        }

        #endregion Actions.VtkTests.QuadCells


        private bool _TestVtkCommandInitialized = false;

        /// <summary>Initializes VTK test commands.</summary>
        protected virtual void InitTestVtk()
        {

            lock (Lock)
            {
                if (_TestVtkCommandInitialized)
                    return;
                AddTestVtkCommand(VtkStructuredGrid, VtkFunctionStructuredGrid, VtkHelpStructuredGrid);
                AddTestVtkCommand(VtkQuadCells, VtkFunctionQuadCells, VtkHelpQuadCells);
                AddTestVtkCommand(VtkCellGridContours, VtkFunctionCellGridContours, VtkHelpCellGridContours);
                AddTestVtkCommand(VtkStructuredGridVolumeContours, VtkFunctionStructuredGridVolumeContours, VtkHelpStructuredGridVolumeContours);

                _TestVtkCommandInitialized = true;
            }
        }


        /// <summary>Runs demonstration of surface plots according to arguments.</summary>
        protected virtual string RunTestVtk(string[] args)
        {
            InitTestVtk();
            if (args == null)
                throw new ArgumentException("No arguments. Test name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " TestName arg1 arg2...");
                        Console.WriteLine(args[0] + " TestName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of test names: ");
                        for (int i = 0; i < TestVtkNames.Count; ++i)
                            Console.WriteLine("  " + TestVtkNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string surfaceName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = TestVtkNames.IndexOf(surfaceName.ToLower());
            if (index < 0)
                throw new ArgumentException("Test named " + surfaceName + " is not found. Call with '?' for list of test names.");
            surfaceName = TestVtkNames[index];
            string helpString = TestVtkHelpStrings[index];
            CommandMethod method = TestVtkMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + surfaceName + ": ");
                    if (string.IsNullOrEmpty(TestVtkHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(TestVtkHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(surfaceName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs various VTK tests.</summary>
        public virtual string TestVtkTest(string[] arguments)
        {
            string ret = null;
            Console.WriteLine();
            Console.WriteLine("VTK test.");
            Console.WriteLine("==============================");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine();


            if (ret == null)
                ret = RunTestVtk(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("VTK test finished.");
            Console.WriteLine();
            return ret;
        }  // TestFormats


        #endregion Actions.VtkTests

        
        #region Actions.Plot3d

        /// <summary>List of installed 3D plotting test names.</summary>
        protected List<string> TestPlot3dNames = new List<string>();

        /// <summary>List of help strings corresponding to installed 3D plotting tests.</summary>
        protected List<string> TestPlot3dHelpStrings = new List<string>();

        /// <summary>List of methods used to perform 3D plotting tests.</summary>
        protected List<CommandMethod> TestPlot3dMethods = new List<CommandMethod>();

        /// <summary>Adds a new command for a 3D graphics test.</summary>
        /// <param name="testName">Test name.</param>
        /// <param name="surfaceMethod">Method used to perform the test.</param>
        /// <param name="surfaceHelp">Eventual help string for the test.</param>
        protected void AddPlot3dCommand(string testName, CommandMethod surfaceMethod, string surfaceHelp)
        {
            lock (Lock)
            {
                TestPlot3dNames.Add(testName.ToLower());
                TestPlot3dHelpStrings.Add(surfaceHelp);
                TestPlot3dMethods.Add(surfaceMethod);
            }
        }



        #region Actions.Plot3d.VtkControl

        public const string Plot3dVtkControl = "VtkControl";

        private const string Plot3dHelpVtkControl = Plot3dVtkControl + @" formType modal testPlotter testActorsIGLib testActorsVTK : 
  Tests VTK controls.
    formType: type of the form ('plain', 'vtk').
    modal: if true then form is launched as modal form
    testPlotter: if true then some test IGLib graphics is plotted on the form through plotter
    testActorsIGLib: if true then some test IGLib graphics is plotted on the form internally
    testActorsVTK: if true then some test VTK graphics is added internally ";

        /// <summary>Tests use of VTK controls.</summary>
        private string Plot3dFunctionVtkControl(string surfaceName, string[] args)
        {

            return IG.Gr3d.VtkFormsExamples.Plot3dFunctionVtkControl(surfaceName, args);

            //VtkControlBase.DefaultVtkAddTestActorsIGLib = false;
            //VtkControlBase.DefaultVtkAddTestActors = false;
            //VtkControlBase.DefaultVtkTestText = "VTK Render Window's test text.";
            //bool modal = true;
            //bool testPlotter = true; // whether a plotter is created that plots some IGLib graphics to the form.

            //Form form = null;
            //IVtkFormContainer vtkFormContainer = null;

            //string formType = null;
            //int NumAppArguments = 0;
            //if (AppArguments != null)
            //    NumAppArguments = AppArguments.Length;
            //if (NumAppArguments >= 1)
            //{
            //    formType = AppArguments[0];
            //}
            //if (NumAppArguments >= 2)
            //{
            //    bool val = modal;
            //    if (Util.TryParseBoolean(AppArguments[1], ref val))
            //        modal = val;
            //    else
            //        Console.WriteLine("Invalid form of boolean argument 3 - modal: " + AppArguments[1]);
            //}
            //if (NumAppArguments >= 3)
            //{
            //    bool val = testPlotter;
            //    if (Util.TryParseBoolean(AppArguments[2], ref val))
            //        testPlotter = val;
            //    else
            //        Console.WriteLine("Invalid form of boolean argument 3 - testPlotter: " + AppArguments[2]);
            //}
            //if (NumAppArguments >= 4)
            //{
            //    bool val = VtkControlBase.DefaultVtkAddTestActorsIGLib;
            //    if (Util.TryParseBoolean(AppArguments[3], ref val))
            //        VtkControlBase.DefaultVtkAddTestActorsIGLib = val;
            //    else
            //        Console.WriteLine("Invalid form of boolean argument 4 - DefaultVtkAddTestActorsIGLib: " + AppArguments[3]);
            //}
            //if (NumAppArguments >= 5)
            //{
            //    bool val = VtkControlBase.DefaultVtkAddTestActors;
            //    if (Util.TryParseBoolean(AppArguments[4], ref val))
            //        VtkControlBase.DefaultVtkAddTestActors = val;
            //    else
            //        Console.WriteLine("Invalid form of boolean argument 5 - DefaultVtkAddTestActors: " + AppArguments[4]);
            //}
            //try
            //{
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //}
            //catch { }

            //if (formType!=null) formType = formType.ToLower();
            //if (formType == "plain" || formType == "vtkformplain")
            //{
            //    VtkFormPlain formPlain = new VtkFormPlain();
            //    form = formPlain;
            //    vtkFormContainer = formPlain;
            //}
            //else if (formType == "vtk")
            //{
            //    VtkForm formVtk = new VtkForm();
            //    form = formVtk;
            //    vtkFormContainer = formVtk;
            //}
            //else
            //{
            //    VtkForm formVtk = new VtkForm();
            //    form = formVtk;
            //    vtkFormContainer = formVtk;
            //}

            //VtkControlBase vtkControl = null;
            //if (vtkFormContainer!=null)
            //    vtkControl = vtkFormContainer.VtkControl;

            //if (testPlotter)
            //{

            //    bool useInternalExample = false;

            //    if (vtkControl == null)
            //    {
            //        Console.WriteLine(Environment.NewLine + Environment.NewLine
            //            + "ERROR: Can not get VTK control. Problems with initialization? "
            //            + Environment.NewLine);
            //    }

            //    if (useInternalExample)
            //    {
            //        // Add the event handler that plots a test graphics through IGLib plotter:
            //        if (vtkControl != null)
            //        {
            //            vtkFormContainer.VtkControl.LoadVtkGraphics += (obj, eventArgs) =>
            //                {
            //                    // This method on VtkControlBase plots a test graphics on VTK form container's embedded control:
            //                    VtkControlBase.ExampleExternalLoadVtkGraphics_SurfacePlots(vtkFormContainer);
            //                    vtkControl.VtkRenderer.Render();
            //                };
            //        }
            //    }

            //    else
            //    {
            //        // Add the event handler that plots a test graphics through IGLib plotter:

            //        if (vtkControl!=null)
            //        {


            //            VtkPlotter plotter = this.Plotter;
            //            IFunc2d func = new VtkPlotBase.ExampleFunc2dXY();

            //            VtkContourPlot plot = new VtkContourPlot(plotter);  // plot object that will create contour plots on the plotter object
            //            plot.OutputLevel = 1;  // print to console what's going on
            //            BoundingBox2d paramBounds = new BoundingBox2d(-1, 1, -1, 1);

            //            // Create the first surface graph by the plot object; adjust the setting first:
            //            func = new VtkPlotBase.ExampleFunc2dXY();
            //            plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //            plot.SetBoundsParameters(paramBounds);
            //            plot.NumX = plot.NumY = 30;
            //            plot.NumContours = 20;
            //            plot.LinesVisible = true;
            //            plot.SurfacesVisible = true;
            //            plot.SurfaceColor = Color.LightCoral;
            //            plot.SurfaceColorOpacity = 0.4;
            //            plot.PointSize = 4; plot.LineWidth = 2; plot.PointsVisible = false;
            //            plot.SurfaceColorIsScaled = false;
            //            plot.LineColorIsScaled = false;

            //            // Create plot of the first surface according to settings:
            //            plot.Create();

            //            vtkFormContainer.VtkControl.LoadVtkGraphics += (obj, eventArgs) =>
            //                {
            //                    // This method on VtkControlBase plots a test graphics on VTK form container's embedded control:

            //                    Kitware.VTK.vtkRenderWindow vtkWindow = vtkControl.VtkRenderWindow;
            //                    if (vtkWindow == null)
            //                    {
            //                        Console.WriteLine(Environment.NewLine + Environment.NewLine
            //                            + "WARNING: Can not obtain VTK Window (external example method)." + Environment.NewLine);
            //                    }
            //                    else
            //                    {
            //                        plotter.SetWindow(vtkWindow);  // plotter object that handles rendering of plots

            //                        // !!!!!
            //                        plotter.setRenderer(vtkControl.VtkRenderer);

            //                        // Create plot of the first surface according to settings:
            //                        plot.Create();

            //                        // Now show all the plots that were created by the plotter; method with the same name is 
            //                        // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):

            //                        //plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

            //                        //// Then, create the surface graph to combine it with contours; change some settings first:
            //                        //VtkSurfacePlot surfacePlot = new VtkSurfacePlot(plotter);  // plot object that will create contour plots on the plotter object
            //                        //surfacePlot.SetSurfaceDefinition(func); // another function of 2 variables for the secont graph
            //                        //surfacePlot.SetBoundsParameters(paramBounds);
            //                        //surfacePlot.NumX = surfacePlot.NumY = 8;
            //                        //surfacePlot.PointsVisible = true;
            //                        //surfacePlot.SurfacesVisible = false;
            //                        //surfacePlot.LinesVisible = true;
            //                        //surfacePlot.SurfaceColorIsScaled = false;
            //                        //surfacePlot.SurfaceColor = System.Drawing.Color.LightGreen;
            //                        //surfacePlot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent
                                    
            //                        //// Create the second plot:
            //                        ////surfacePlot.CreateAndShow();


            //                        // plotter.ShowPlotWithoutRender();

            //                        vtkControl.VtkRenderer.Render();

            //                    }


            //                };
            //        }

            //    }  // ! useInternalExample


            //}

            //if (form != null)
            //{
            //    if (modal)
            //        form.ShowDialog();
            //    else
            //        form.Show();
            //}


            //if (form != null)
            //{
            //    if (modal)
            //        form.ShowDialog();
            //    else
            //        form.Show();
            //}

            //return null;



        }  // Plot3dFunctionVtkControl


        //#region Actions.Plot3d.VtkControl.Auxiliary

        //VtkPlotter _plotter;

        //VtkPlotter Plotter
        //{
        //    get
        //    {
        //        if (_plotter == null)
        //        {
        //            _plotter = new VtkPlotter();
        //        }
        //        return _plotter;
        //    }
        //    set
        //    {
        //        _plotter = value;
        //    }

        //}

        //#endregion Actions.Plot3d.VtkControl.Auxiliary


        #endregion Actions.Plot3d.VtkControl


        // CurvePlotLissajous CurvePlotLissajous CurvePlotLissajous

        #region Actions.Plot3d.CurvePlotLissajous

        public const string Plot3dCurvePlotLissajous = "CurvePlotLissajous";

        private const string Plot3dHelpCurvePlotLissajous = Plot3dCurvePlotLissajous + " : Plot of Lissajous-derived curves in 3D.";

        /// <summary>Demonstration of plotting 3d parametric curves.</summary>
        private string Plot3dFunctionCurvePlotLissajous(string surfaceName, string[] args)
        {
            VtkPlotBase.ExampleCurvePlotLissajous();
            return null;
        }

        #endregion Actions.Plot3d.CurvePlotLissajous


        #region Actions.Plot3d.SurfacePlot

        public const string Plot3dSurfacePlot = "SurfacePlot";

        private const string Plot3dHelpSurfacePlot = Plot3dSurfacePlot + " : Surface plot.";

        /// <summary>Demonstration of surface plots in 3D.</summary>
        private string Plot3dFunctionSurfacePlot(string surfaceName, string[] args)
        {
            VtkPlotBase.ExampleSurfacePlot();
            return null;
        }

        #endregion Actions.Plot3d.SurfacePlot


        #region Actions.Plot3d.SurfacePlotScaled

        public const string Plot3dSurfacePlotScaled = "SurfacePlotScaled";

        private const string Plot3dHelpSurfacePlotScaled = Plot3dSurfacePlotScaled + " <numX> <numY>: Surface plot with manual mesh, automatically scaled.";

        /// <summary>Demonstration of surface plots in 3D where mesh is manually composed. Automatic scaling of physical graph is also demonstrated.</summary>
        private string Plot3dFunctionSurfacePlotScaled(string surfaceName, string[] args)
        {
            int numX = 30, numY = 30;
            int numArguments = 0;
            if (args != null)
            {
                if (args.Length > 0)
                    numArguments = args.Length;
            }
            if (numArguments > 0)
            {
                Console.WriteLine("Command that invoked this metod: " + args[0]);
                if (numArguments >= 2)
                {
                    Console.WriteLine("Arg. 0 (numX): " + args[0]);
                    Console.WriteLine("Arg. 1: " + args[0]);
                    numX = int.Parse(args[0]);
                    numY = int.Parse(args[1]);
                }
            }
            VtkPlotBase.ExampleSurfacePlotScaled(numX, numY);
            return null;
        }

        #endregion Actions.Plot3d.SurfacePlotScaled



        #region Actions.Plot3d.SurfacePlotManualScaled

        public const string Plot3dSurfacePlotManualScaled = "SurfacePlotManualScaled";

        private const string Plot3dHelpSurfacePlotManualScaled = Plot3dSurfacePlotManualScaled + " <numX> <numY>: Surface plot with manual mesh, manually scaled.";

        /// <summary>Demonstration of surface plots in 3D where mesh is manually composed. Scaling of physical graph is also demonstrated.</summary>
        private string Plot3dFunctionSurfacePlotManualScaled(string surfaceName, string[] args)
        {
            int numX = 30, numY = 30;
            int numArguments = 0;
            if (args != null)
            {
                if (args.Length > 0)
                    numArguments = args.Length;
            }
            if (numArguments > 0)
            {
                Console.WriteLine("Command that invoked this metod: " + args[0]);
                if (numArguments >= 2)
                {
                    Console.WriteLine("Arg. 0 (numX): " + args[0]);
                    Console.WriteLine("Arg. 1: " + args[0]);
                    numX = int.Parse(args[0]);
                    numY = int.Parse(args[1]);
                }
            }

            VtkPlotBase.ExampleSurfacePlotManualScaled(numX, numY);
            return null;
        }

        #endregion Actions.Plot3d.SurfacePlotManualScaled



        #region Actions.Plot3d.ContourPlot

        public const string Plot3dContourPlot = "ContourPlot";

        private const string Plot3dHelpContourPlot = Plot3dContourPlot + " : Contour plot.";

        /// <summary>Demonstration contour plots in 3D.</summary>
        private string Plot3dFunctionContourPlot(string surfaceName, string[] args)
        {
            VtkPlotBase.ExampleContourPlot();
            return null;
        }

        #endregion Actions.Plot3d.ContourPlot
        

        #region Actions.Plot3d.SurfaceComparison

        public const string Plot3dSurfaceComparison = "SurfaceComparison";

        private const string Plot3dHelpSurfaceComparison = Plot3dSurfaceComparison + " : Comparison of 2 surfaces in 3D.";

        /// <summary>Demonstration of comparison of 2 surfaces in 3D.</summary>
        private string Plot3dFunctionSurfaceComparison(string surfaceName, string[] args)
        {
            VtkPlotBase.ExampleCustomSurfaceComparison();
            return null;
        }

        #endregion Actions.Plot3d.SurfaceComparison


        #region Actions.Plot3d.Decoration

        public const string Plot3dDecoration = "Decoration";

        private const string Plot3dHelpDecoration = Plot3dDecoration + " : Decorations demo.";

        /// <summary>Demonstration of surface plots in 3D.</summary>
        private string Plot3dFunctionDecoration(string surfaceName, string[] args)
        {
            VtkPlotBase.ExamplePlotterDecoration();
            return null;
        }

        #endregion Actions.Plot3d.Decoration


        private bool _plot3dCommandsInitialized = false;

        /// <summary>Initializes commands for 3d graphic tests.</summary>
        protected virtual void InitTestPlot3d()
        {

            lock (Lock)
            {
                if (_plot3dCommandsInitialized)
                    return;
                AddPlot3dCommand(Plot3dVtkControl, Plot3dFunctionVtkControl, Plot3dHelpVtkControl);
                AddPlot3dCommand(Plot3dCurvePlotLissajous, Plot3dFunctionCurvePlotLissajous, Plot3dHelpCurvePlotLissajous);
                AddPlot3dCommand(Plot3dSurfacePlot, Plot3dFunctionSurfacePlot, Plot3dHelpSurfacePlot);
                AddPlot3dCommand(Plot3dSurfacePlotScaled, Plot3dFunctionSurfacePlotScaled, Plot3dHelpSurfacePlotScaled);
                AddPlot3dCommand(Plot3dSurfacePlotManualScaled, Plot3dFunctionSurfacePlotManualScaled, Plot3dHelpSurfacePlotManualScaled);
                AddPlot3dCommand(Plot3dContourPlot, Plot3dFunctionContourPlot, Plot3dHelpContourPlot);
                AddPlot3dCommand(Plot3dSurfaceComparison, Plot3dFunctionSurfaceComparison, Plot3dHelpSurfaceComparison);
                AddPlot3dCommand(Plot3dDecoration, Plot3dFunctionDecoration, Plot3dHelpDecoration);

                _plot3dCommandsInitialized = true;
            }
        }


        /// <summary>Runs demonstration of surface plots according to arguments.</summary>
        protected virtual string RunTestPlot3d(string[] args)
        {
            InitTestPlot3d();
            if (args == null)
                throw new ArgumentException("No arguments. Test name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " TestName arg1 arg2...");
                        Console.WriteLine(args[0] + " TestName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of tests: ");
                        for (int i = 0; i < TestPlot3dNames.Count; ++i)
                            Console.WriteLine("  " + TestPlot3dNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = TestPlot3dNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Test named " + testName + " is not found. Call with '?' for list of test names.");
            testName = TestPlot3dNames[index];
            string helpString = TestPlot3dHelpStrings[index];
            CommandMethod method = TestPlot3dMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(TestPlot3dHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(TestPlot3dHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
            finally
            {
            }
        }

        /// <summary>Demonstration of various 3D plots.</summary>
        public virtual string TestPlot3d(string[] arguments)
        {
            string ret = null;
            Console.WriteLine();
            Console.WriteLine("3D plot example.");
            Console.WriteLine("==============================");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine();


            if (ret == null)
                ret = RunTestPlot3d(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("3D plot example finished.");
            Console.WriteLine();
            return ret;
        }  // TestFormats


        #endregion Actions.Plot3d


        #region Actions.SurfacePlots

        /// <summary>List of surface names.</summary>
        protected List<string> SurfaceNames = new List<string>();

        /// <summary>List of surface methods' help strings.</summary>
        protected List<string> SurfaceHelpStrings = new List<string>();

        /// <summary>List of methods used to plot surfaces.</summary>
        protected List<CommandMethod> SurfaceMethods = new List<CommandMethod>();

        /// <summary>Adds a new command for plotting the specified surface.</summary>
        /// <param name="surfaceName">Surface (and the corresponding plotting command) name.</param>
        /// <param name="surfaceMethod">Method used to plot the surface.</param>
        /// <param name="surfaceHelp">Eventual help string for the added surface.</param>
        protected void AddSurfaceCommand(string surfaceName, CommandMethod surfaceMethod, string surfaceHelp)
        {
            lock (Lock)
            {
                SurfaceNames.Add(surfaceName.ToLower());
                SurfaceHelpStrings.Add(surfaceHelp);
                SurfaceMethods.Add(surfaceMethod);
            }
        }

        /// <summary>Sets default properties of the specified plot.</summary>
        /// <param name="plot"></param>
        protected virtual void SetDefaultPlotProperties(VtkSurfacePlot plot)
        {
            // plot.SetSurfaceDefinition(func);
            plot.OutputLevel = 1;  // print to console what's going on
            plot.SetBoundsParameters(-1, 1, -1, 1);
            plot.NumX = 20; plot.NumY = 20;
            plot.PointSize = 4; plot.LineWidth = 1; plot.PointsVisible = false;
            plot.SurfaceColorIsScaled = false;
            plot.LineColorIsScaled = false;
            plot.SurfaceColor = System.Drawing.Color.LightCyan;
            plot.LineColor = System.Drawing.Color.LightGreen;
            plot.SurfaceColorOpacity = 0.8;
            plot.LineColor = System.Drawing.Color.Red; plot.LineWidth = 2;
            plot.SurfaceColor = System.Drawing.Color.Cyan;
            plot.SurfaceColorOpacity = 0.7;
        }


        /// <summary>Sets plotting resolution and parameter bounds on the specified plot according to arguments.
        /// <para>Only things that are specified by arguments are set.</para>
        /// <para>Arguments must follow in this order: NumX, NumY, fromX, toX, fromY, toY</para></summary>
        /// <param name="plot">Plot where resolution and bounds are set.</param>
        protected virtual void SetPlotBounds(VtkSurfacePlot plot, string[] args)
        {
            if (args.Length >= 1)
                plot.NumX = int.Parse(args[0]);
            if (args.Length >= 2)
                plot.NumY = int.Parse(args[1]);
            if (args.Length >= 3)
                plot.BoundsParameters.SetMin(0, double.Parse(args[2]));
            if (args.Length >= 4)
                plot.BoundsParameters.SetMax(0, double.Parse(args[3]));
            if (args.Length >= 5)
                plot.BoundsParameters.SetMin(1, double.Parse(args[4]));
            if (args.Length >= 6)
                plot.BoundsParameters.SetMax(1, double.Parse(args[5]));
        }


        #region Actions.SurfacePlots.KleinBottle

        public const string surfaceKleinBottle = "KleinBottle";

        private const string surfaceHelpKleinBottle = surfaceKleinBottle + " <numX> <numY> <formX> <toX> <fromY> <toY> : Klein's bottle";

        /// <summary>Plots the Klein's Bottle parametric surface.</summary>
        /// <seealso cref="VtkPlotBase.ExampleParametricSurfacePlots"/>
        private string SurfaceFunctionKleinBottle(string surfaceName, string[] args)
        {

            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkSurfacePlot plot = new VtkSurfacePlot(plotter);
            SetDefaultPlotProperties(plot);

            plot.SurfaceColorOpacity = 0.8;
            plot.LineColor = System.Drawing.Color.Red; plot.LineWidth = 2;
            plot.SurfaceColor = System.Drawing.Color.Cyan;
            plot.SurfaceColorOpacity = 0.7;
            Func3d2dExamples.ParametricSurface func = new Func3d2dExamples.KleinBottle();

            plot.SetSurfaceDefinition(func);
            VtkPlotBase.CopyBounds(func, plot);
            SetPlotBounds(plot, args);
            plot.CreateAndShow();
            return null;
        }

        #endregion Surface.KleinBottle


        #region Actions.SurfacePlots.TwoToruses

        public const string surfaceTwoToruses = "TwoToruses";

        private const string surfaceHelpTwoToruses = surfaceTwoToruses + " : Two interlocked toruses.";

        /// <summary>Plots two interlocked toruses.</summary>
        /// <seealso cref="VtkPlotBase.ExampleParametricSurfacePlots"/>
        private string SurfaceFunctionTwoToruses(string surfaceName, string[] args)
        {

            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkSurfacePlot plot = new VtkSurfacePlot(plotter);
            SetDefaultPlotProperties(plot);

            Func3d2dExamples.ParametricSurface func;
            func = new Func3d2dExamples.TorusHorizontal();
            plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            VtkPlotBase.CopyBounds(func, plot);
            plot.Create();
            func = new Func3d2dExamples.TorusVertical();
            plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            VtkPlotBase.CopyBounds(func, plot);
            plot.SurfaceColor = System.Drawing.Color.YellowGreen;
            plot.Create();
            plot.ShowPlot();

            return null;
        }

        #endregion Actions.SurfacePlots.TwoToruses


        #region Actions.SurfacePlots.SnailShell

        public const string surfaceSnailShell = "SnailShell";

        private const string surfaceHelpSnailShell = surfaceSnailShell + " <numX> <numY> <formX> <toX> <fromY> <toY> : Snail's shell surface";

        /// <summary>Plots the Snail shell parametric surface.</summary>
        /// <seealso cref="VtkPlotBase.ExampleParametricSurfacePlots"/>
        private string SurfaceFunctionSnailShell(string surfaceName, string[] args)
        {
            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkSurfacePlot plot = new VtkSurfacePlot(plotter);
            SetDefaultPlotProperties(plot);

            plot.SurfaceColorOpacity = 0.8;
            plot.LineColor = System.Drawing.Color.Red; plot.LineWidth = 2;
            plot.SurfaceColor = System.Drawing.Color.Cyan;
            plot.SurfaceColorOpacity = 0.7;
            Func3d2dExamples.ParametricSurface func = new Func3d2dExamples.KleinBottle();

            // Snailshell-shaped surface:
            func = new Func3d2dExamples.SnailShell1Streched();
            plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface

            plot.SetSurfaceDefinition(func);
            VtkPlotBase.CopyBounds(func, plot);
            SetPlotBounds(plot, args);
            plot.CreateAndShow();
            return null;
        }

        #endregion Actions.SurfacePlots.SnailShell


        private bool _surfaceCommandInitialized = false;

        /// <summary>Initializes surface plotting commands.</summary>
        protected virtual void InitSurfaceCommands()
        {

            lock (Lock)
            {
                if (_surfaceCommandInitialized)
                    return;
                AddSurfaceCommand(surfaceKleinBottle, SurfaceFunctionKleinBottle, surfaceHelpKleinBottle);
                AddSurfaceCommand(surfaceTwoToruses, SurfaceFunctionTwoToruses, surfaceHelpTwoToruses);
                AddSurfaceCommand(surfaceSnailShell, SurfaceFunctionSnailShell, surfaceHelpSnailShell);

                _surfaceCommandInitialized = true;
            }
        }


        /// <summary>Runs demonstration of surface plots according to arguments.</summary>
        protected virtual string Run3DSurfaceTest(string[] args)
        {
            InitSurfaceCommands();
            if (args == null)
                throw new ArgumentException("No arguments. Surface type (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Surface type (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Surface type (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " SurfaceType arg1 arg2...");
                        Console.WriteLine(args[0] + " SurfaceType ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of surfaces: ");
                        for (int i = 0; i < SurfaceNames.Count; ++i)
                            Console.WriteLine("  " + SurfaceNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string surfaceName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = SurfaceNames.IndexOf(surfaceName.ToLower());
            if (index < 0)
                throw new ArgumentException("Surface named " + surfaceName + " is not found. Call with '?' for list of surface names.");
            surfaceName = SurfaceNames[index];
            string helpString = SurfaceHelpStrings[index];
            CommandMethod method = SurfaceMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Plot of surface " + surfaceName + ": ");
                    if (string.IsNullOrEmpty(SurfaceHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(SurfaceHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(surfaceName, surfaceParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Demonstration of surface plots.</summary>
        public virtual string TestSurface3d(string[] arguments)
        {
            string ret = null;
            Console.WriteLine();
            Console.WriteLine("3D surfaces.");
            Console.WriteLine("==============================");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine();


            if (ret == null)
                ret = Run3DSurfaceTest(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("3D surface example finished.");
            Console.WriteLine();
            return ret;
        }  // TestFormats


        #endregion Actions.SurfacePlots


        #endregion Actions

    }  // script

}

