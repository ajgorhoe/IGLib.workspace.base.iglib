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

using IG.Gr;

namespace IG.Script
{

    /// <summary>Base class for script classes with 2D graphics examples.</summary>
    /// $A Igor xx Jul12;
    public class ScriptGraphics2dBase : LoadableScriptBase, ILoadableScript
    {

        #region Standard_TestScripts

        public ScriptGraphics2dBase()
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

        /// <summary>Name of the 2d graphs tests.</summary>
        public const string ConstGraph = "Graph";
        public const string ConstHelpGraph = "Various 2D graphs. Run with ? argument to see which tests are available.";


        #endregion Commands

        /// <summary>Adds commands to the internal interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);
            Script_AddCommand(interpreter, helpStrings, ConstMyTest, TestMyTest, ConstHelpMyTest);
            Script_AddCommand(interpreter, helpStrings, ConstCustom, TestCustom, ConstHelpCustom);
            Script_AddCommand(interpreter, helpStrings, ConstGraph, TestGraph, ConstHelpGraph);
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


        #region Actions.Graph

        /// <summary>List of installed 2D graph test names.</summary>
        protected List<string> TestGraphNames = new List<string>();

        /// <summary>List of help strings corresponding to installed 2D graph tests.</summary>
        protected List<string> TestGraphHelpStrings = new List<string>();

        /// <summary>List of methods used to perform 2D graph tests.</summary>
        protected List<CommandMethod> TestGraphMethods = new List<CommandMethod>();

        /// <summary>Adds a new command for a 2D graph test.</summary>
        /// <param name="testName">Test name.</param>
        /// <param name="surfaceMethod">Method used to perform the test.</param>
        /// <param name="surfaceHelp">Eventual help string for the test.</param>
        protected void AddGraphCommand(string testName, CommandMethod surfaceMethod, string surfaceHelp)
        {
            lock (Lock)
            {
                TestGraphNames.Add(testName.ToLower());
                TestGraphHelpStrings.Add(surfaceHelp);
                TestGraphMethods.Add(surfaceMethod);
            }
        }


        #region Actions.Graph.SinePlots

        private const string GraphSinePlots = "SinePlots";

        private const string GraphHelpSinePlots = GraphSinePlots + " <nCurves> <nPoints> : Plot of sine curves with different phases.";

        /// <summary>Demonstration of plotting sine curves with different phases.</summary>
        private string GraphFunctionSinePlots(string surfaceName, string[] args)
        {
            int nCurves = 5;
            int nPoints = 200;
            if (args.Length >= 1)
                nCurves = int.Parse(args[0]);
            if (args.Length >= 2)
                nPoints = int.Parse(args[1]);

            PlotterZedGraph.ExempleSinePlots(nCurves, nPoints);
            return null;
        }

        #endregion Actions.Graph.SinePlots


        #region Actions.Graph.CurvePlotLissajous

        private const string GraphCurvePlotLissajous = "CurvePlotLissajous";

        private const string GraphHelpCurvePlotLissajous = GraphCurvePlotLissajous + " <nX> <nY> : Plot of Lissajous curves in 2D.";

        /// <summary>Demonstration of plotting 2d parametric curves (Lissajous curves).</summary>
        private string GraphFunctionCurvePlotLissajous(string surfaceName, string[] args)
        {
            int nX = 5;
            int nY = 4;
            if (args.Length >= 1)
                nX = int.Parse(args[0]);
            if (args.Length >= 2)
                nY = int.Parse(args[1]);

            PlotterZedGraph.ExampleLissajous(nX, nY);
            return null;
        }

        #endregion Actions.Graph.CurvePlotLissajous


        #region Actions.Graph.Decorations

        private const string GraphDecorations = "Decorations";

        private const string GraphHelpDecorations = GraphDecorations + " : Decoration styles on 2D graphs.";

        /// <summary>Demonstration of different decoration styles for 2D graphs.</summary>
        private string GraphFunctionDecorations(string surfaceName, string[] args)
        {
            PlotterZedGraph.ExampleDecorations();
            return null;
        }

        #endregion Actions.Graph.Decorations


        #region Actions.Graph.CurveStylesWithSave

        private const string GraphCurveStylesWithSave = "CurveStylesWithSave";

        private const string GraphHelpCurveStylesWithSave = GraphCurveStylesWithSave + " <filePath.bmp> : Curve styles and saving of 2D graphs.";

        /// <summary>Demonstration of different curve styles and saving of 2D graphs.</summary>
        private string GraphFunctionCurveStylesWithSave(string surfaceName, string[] args)
        {
            string filePath = "%temp%/test.bmp";
            PlotterZedGraph.ExampleCurveStylesWithSave(filePath);
            return null;
        }

        #endregion Actions.Graph.CurveStylesWithSave



        private bool _graphCommandsInitialized = false;

        /// <summary>Initializes commands for 3d graphic tests.</summary>
        protected virtual void InitTestGraph()
        {

            lock (Lock)
            {
                if (_graphCommandsInitialized)
                    return;
                AddGraphCommand(GraphCurvePlotLissajous, GraphFunctionCurvePlotLissajous, GraphHelpCurvePlotLissajous);
                AddGraphCommand(GraphSinePlots, GraphFunctionSinePlots, GraphHelpSinePlots);
                AddGraphCommand(GraphDecorations, GraphFunctionDecorations, GraphHelpDecorations);
                AddGraphCommand(GraphCurveStylesWithSave, GraphFunctionCurveStylesWithSave, GraphHelpCurveStylesWithSave);

                _graphCommandsInitialized = true;
            }
        }


        /// <summary>Runs demonstration of surface plots according to arguments.</summary>
        protected virtual string RunTestGraph(string[] args)
        {
            InitTestGraph();
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
                        for (int i = 0; i < TestGraphNames.Count; ++i)
                            Console.WriteLine("  " + TestGraphNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] surfaceParams = new string[args.Length - 2];
            for (int i = 0; i < surfaceParams.Length; ++i)
                surfaceParams[i] = args[i + 2];
            int index = TestGraphNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Test named " + testName + " is not found. Call with '?' for list of test names.");
            testName = TestGraphNames[index];
            string helpString = TestGraphHelpStrings[index];
            CommandMethod method = TestGraphMethods[index];
            if (surfaceParams.Length >= 1)
                if (surfaceParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(TestGraphHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(TestGraphHelpStrings[index]);
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
        }

        /// <summary>Demonstration of various 3D plots.</summary>
        public virtual string TestGraph(string[] arguments)
        {
            string ret = null;
            Console.WriteLine();
            Console.WriteLine("2D graph plotting example.");
            Console.WriteLine("==============================");
            Script_PrintArguments("Script arguments: ", arguments);
            Console.WriteLine();


            if (ret == null)
                ret = RunTestGraph(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("2D graph plotting finished.");
            Console.WriteLine();
            return ret;
        }  // TestFormats


        #endregion Actions.Graph



        #endregion Actions

    }  // script

}

