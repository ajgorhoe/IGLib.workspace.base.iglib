// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// EXAMPLE CLASS that hosts some VTK examples, e.g. those dependent on the VTKFORMSDESIGN compiler directive.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Text;

using IG.Num;
using IG.Lib;

using System.Drawing;

namespace IG.Gr3d
{

    /// <summary>Examples from VTK forms.
    /// <para>This class hosts some example methods (e.g. for application scripts) whose natural location
    /// would be somewhere else but are put into this project because they contain dependencies on the 
    /// VTKFORMSDESIGN compiler directive, which is used in the current project to enable normal function 
    /// of the GUI designer in the presence of the VTK form classes that are not compatible with GUI
    /// designer.</para></summary>
    public class VtkFormsExamples
    {


        static VtkPlotter _plotter;

        /// <summary>Plotter that is used for rendering VTK graphics.</summary>
        static VtkPlotter Plotter
        {
            get
            {
                if (_plotter == null)
                {
                    _plotter = new VtkPlotter();
                }
                return _plotter;
            }
            set
            {
                _plotter = value;
            }

        }


        /// <summary>Tests use of VTK controls.</summary>
        public static string Plot3dFunctionVtkControl(string surfaceName, string[] args)
        {
            VtkControlBase.DefaultVtkAddTestActorsIGLib = false;
            VtkControlBase.DefaultVtkAddTestActors = false;
            VtkControlBase.DefaultVtkTestText = "VTK Render Window's test text.";
            bool modal = true;
            bool testPlotter = true; // whether a plotter is created that plots some IGLib graphics to the form.

            Form form = null;
            IVtkFormContainer vtkFormContainer = null;

            string formType = null;
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs >= 1)
            {
                formType = args[0];
            }
            if (numArgs >= 2)
            {
                bool val = modal;
                if (Util.TryParseBoolean(args[1], ref val))
                    modal = val;
                else
                    Console.WriteLine("Invalid form of boolean argument 3 - modal: " + args[1]);
            }
            if (numArgs >= 3)
            {
                bool val = testPlotter;
                if (Util.TryParseBoolean(args[2], ref val))
                    testPlotter = val;
                else
                    Console.WriteLine("Invalid form of boolean argument 3 - testPlotter: " + args[2]);
            }
            if (numArgs >= 4)
            {
                bool val = VtkControlBase.DefaultVtkAddTestActorsIGLib;
                if (Util.TryParseBoolean(args[3], ref val))
                    VtkControlBase.DefaultVtkAddTestActorsIGLib = val;
                else
                    Console.WriteLine("Invalid form of boolean argument 4 - DefaultVtkAddTestActorsIGLib: " + args[3]);
            }
            if (numArgs >= 5)
            {
                bool val = VtkControlBase.DefaultVtkAddTestActors;
                if (Util.TryParseBoolean(args[4], ref val))
                    VtkControlBase.DefaultVtkAddTestActors = val;
                else
                    Console.WriteLine("Invalid form of boolean argument 5 - DefaultVtkAddTestActors: " + args[4]);
            }
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            catch { }

            if (formType != null) formType = formType.ToLower();
            if (formType == "plain" || formType == "vtkformplain")
            {
                VtkFormPlain formPlain = new VtkFormPlain();
                form = formPlain;
                vtkFormContainer = formPlain;
            }
            else if (formType == "vtk")
            {
                VtkForm formVtk = new VtkForm();
                form = formVtk;
                vtkFormContainer = formVtk;
            }
            else
            {
                VtkForm formVtk = new VtkForm();
                form = formVtk;
                vtkFormContainer = formVtk;
            }

            VtkControlBase vtkControl = null;
            if (vtkFormContainer != null)
                vtkControl = vtkFormContainer.VtkControl;


#if !VTKFORMSDESIGN

            if (testPlotter)
            {

                bool useInternalExample = false;

                if (vtkControl == null)
                {
                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                        + "ERROR: Can not get VTK control. Problems with initialization? "
                        + Environment.NewLine);
                }

                if (useInternalExample)
                {
                    // Add the event handler that plots a test graphics through IGLib plotter:
                    if (vtkControl != null)
                    {
                        vtkFormContainer.VtkControl.LoadVtkGraphics += (obj, eventArgs) =>
                        {
                            // This method on VtkControlBase plots a test graphics on VTK form container's embedded control:
                            VtkControlBase.ExampleExternalLoadVtkGraphics_SurfacePlots(vtkFormContainer);
                            vtkControl.VtkRenderer.Render();
                        };
                    }
                }

                else
                {
                    // Add the event handler that plots a test graphics through IGLib plotter:

                    if (vtkControl != null)
                    {


                        VtkPlotter plotter = Plotter;
                        IFunc2d func = new VtkPlotBase.ExampleFunc2dXY();

                        VtkContourPlot plot = new VtkContourPlot(plotter);  // plot object that will create contour plots on the plotter object
                        plot.OutputLevel = 1;  // print to console what's going on
                        BoundingBox2d paramBounds = new BoundingBox2d(-1, 1, -1, 1);

                        // Create the first surface graph by the plot object; adjust the setting first:
                        func = new VtkPlotBase.ExampleFunc2dXY();
                        plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
                        plot.SetBoundsParameters(paramBounds);
                        plot.NumX = plot.NumY = 30;
                        plot.NumContours = 20;
                        plot.LinesVisible = true;
                        plot.SurfacesVisible = true;
                        plot.SurfaceColor = Color.LightCoral;
                        plot.SurfaceColorOpacity = 0.4;
                        plot.PointSize = 4; plot.LineWidth = 2; plot.PointsVisible = false;
                        plot.SurfaceColorIsScaled = false;
                        plot.LineColorIsScaled = false;

                        // Create plot of the first surface according to settings:
                        plot.Create();

                        vtkFormContainer.VtkControl.LoadVtkGraphics += (obj, eventArgs) =>
                        {
                            // This method on VtkControlBase plots a test graphics on VTK form container's embedded control:

                            Kitware.VTK.vtkRenderWindow vtkWindow = vtkControl.VtkRenderWindow;
                            if (vtkWindow == null)
                            {
                                Console.WriteLine(Environment.NewLine + Environment.NewLine
                                    + "WARNING: Can not obtain VTK Window (external example method)." + Environment.NewLine);
                            }
                            else
                            {
                                plotter.SetWindow(vtkWindow);  // plotter object that handles rendering of plots

                                // !!!!!
                                plotter.setRenderer(vtkControl.VtkRenderer);

                                // Create plot of the first surface according to settings:
                                plot.Create();

                                // Now show all the plots that were created by the plotter; method with the same name is 
                                // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):

                                //plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

                                //// Then, create the surface graph to combine it with contours; change some settings first:
                                //VtkSurfacePlot surfacePlot = new VtkSurfacePlot(plotter);  // plot object that will create contour plots on the plotter object
                                //surfacePlot.SetSurfaceDefinition(func); // another function of 2 variables for the secont graph
                                //surfacePlot.SetBoundsParameters(paramBounds);
                                //surfacePlot.NumX = surfacePlot.NumY = 8;
                                //surfacePlot.PointsVisible = true;
                                //surfacePlot.SurfacesVisible = false;
                                //surfacePlot.LinesVisible = true;
                                //surfacePlot.SurfaceColorIsScaled = false;
                                //surfacePlot.SurfaceColor = System.Drawing.Color.LightGreen;
                                //surfacePlot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent

                                //// Create the second plot:
                                ////surfacePlot.CreateAndShow();


                                // plotter.ShowPlotWithoutRender();

                                vtkControl.VtkRenderer.Render();

                            }


                        };
                    }

                }  // ! useInternalExample


            }


            if (form != null)
            {
                if (modal)
                    form.ShowDialog();
                else
                    form.Show();
            }


            if (form != null)
            {
                if (modal)
                    form.ShowDialog();
                else
                    form.Show();
            }


            #endif  // !VTKFORMSDESIGN



            return null;
        }  // Plot3dFunctionVtkControl



    }  // class VtkFormsExamples
}
