using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

using IG.Lib;
using IG.Gr;
using IG.Num;

using Kitware.VTK;

namespace IG.Gr3d
{

    //TODO: 
    /* The following questions need to be answered:
     
     * How to scale graphics in x, y, and z directions in such a way that labels on scale
     axes are not affected? This is emported for graphs of mathematical functions where the
     aspect ratios of the bounding boxes are large.
     
     * How to position the axes relatively to the bounding box. Maybe the axes will have to
     be manually shifted.
     
     ** Contour plots:
     * How to plot line contours
         see http://www.vtk.org/Wiki/VTK/Examples/Cxx/PolyData/PolyDataIsoLines
           This may be the right answer.
         see http://www.vtk.org/Wiki/VTK/Examples/Cxx/Visualization/LabelContours
           contours lines with labels
         see http://www.vtk.org/doc/release/5.6/html/a00379.html (vtkContourFilter)
         see http://www.vtk.org/doc/release/5.6/html/a00177.html - vtkBandedPolyDataContourFilter class
           this may also be the right filter if you use the GenerateContourEdgesOn() method (maybe proprty in C#).
         see http://www.vtk.org/Wiki/VTK/Examples/Cxx/VisualizationAlgorithms/FilledContours;
            this example is for filled contours, but you may get a clue gow to use it for line 
             contours (e.g. by using very narrow range of values for clipping, then creating an outline)
     
     * How to plot filled contours with discrete colors (instead of smooth transition)
         see http://www.vtk.org/doc/release/5.6/html/a00302.html
         see http://www.vtk.org/Wiki/VTK/Examples/Cxx/VisualizationAlgorithms/FilledContours - with vtkClipPolyData
         see http://www.vtk.org/doc/release/5.6/html/a00177.html - vtkBandedPolyDataContourFilter class
         see http://www.vtk.org/Wiki/VTK/Examples/Cxx/VisualizationAlgorithms/BandedPolyDataContourFilter
         see http://www.vtk.org/doc/release/5.6/html/c2_vtk_e_0.html#c2_vtk_e_vtkBandedPolyDataContourFilter
         see http://www.vtk.org/Wiki/VTK/Examples/Cxx/Filtering/ContoursFromPolyData
           this example is not directly related, but it can hive some useful ideas. Just use the default
           sphere source instead of data from a file.
         
     * How to plot volumetric contors on bodies based on point values
         see http://www.vtk.org/doc/release/5.6/html/a00379.html (vtkContourFilter)
         see http://www.vtk.org/VTK/help/examplecode.html, the secod example.
         see http://www.vtk.org/Wiki/VTK/Examples/Cxx/ImplicitFunctions/SampleFunction
           in this case, an implicit function is furst sampled to get point values, then contourfilter is used
         see http://www.vtk.org/Wiki/VTK/Examples/Cxx/Modelling/ExtractLargestIsosurface
           this is useful for more advanced data visualization
         Steps will probably be as follows:
           1. assign scalar values to Polydata's points
           2. apply ContourFilter to PolyData
      
     * How to assign user defined colors to contours? 
         Currently, we can achieve that contour colors are based on point values, but
         mapping from values to colors can not be affected.
         One solution would be to add a separate actor for each contour and set color of 
         the actor by setting the mode on Mapper that does not take into account point values
         (e.g. vtkPolydataMapper.SetScalarModeToUseCellFieldData or something, provided that 
         cell field data is not provided, since in this case color specified in actor properties is used.)
     
     * How to plot filled surface contours on bodies' surfaces based on point values
     
     * How to plot line contours on bodies' surfaces based on point values
     
     * How to handle special effects, e.g. replacing lines with pipes, points with balls, etc., in a generic 
          way that is applicable to the overall graphical system
      
     * How to display different types of legends, e.g. for contour colors, or for point sizes
     
     * Is it possible to use different line styles>
      
     */


    /// <summary>Tests of 3D graphics enabled by ActiViz (VTK wrapper library).</summary>
    /// $A Tako78 Sep11; Igor Oct11;
    public class TestVtkGraphicBase
    {

        public static void Run()
        {

            // High level 3D graphic routines:

            //VtkPlotBase.ExampleCurvePlotLissajous(); return;

            //VtkPlotBase.ExampleSurfacePlot(); return;

            //VtkPlotBase.ExampleContourPlot(); return;

            //VtkPlotBase.ExampleCustomSurfaceComparison(); return;

            //VtkPlotBase.ExampleParametricSurfacePlots(); return;

           


            // LOW LEVEL vtk TESTS:

            //ExampleStructuredGrid(20, 20, 3);

            //ExampleAxisHendler();

            //ExampleQuadCells(20, 20);

            //ExampleCellsGridContours(20, 20, 20 /* numContours */);

            ExampleStructuredGridVolumeContours(20, 20, 20, 4 /* numContours */);

            //ColorBar();
            //Legend();

            //Test1();
            //Test2();

        }  // Run()

        public static void Test1()
        {
            vtkTable table = new vtkTable();
            vtkFloatArray arrX = new vtkFloatArray();
            arrX.SetName("X Axis");
            //table.AddColumn(arrX);

            vtkFloatArray arrC = new vtkFloatArray();
            arrC.SetName("Cosine");
            //table.AddColumn(arrC);

            vtkFloatArray arrS = new vtkFloatArray();
            arrS.SetName("Sine");
            //table.AddColumn(arrS);

            int numPoints = 69;
            float inc = (float)7.5 / (numPoints - 1);
            //table.SetNumberOfRows(numPoints);
            arrX.SetNumberOfTuples(numPoints);
            arrC.SetNumberOfTuples(numPoints);
            arrS.SetNumberOfTuples(numPoints);

            for (int i = 0; i < numPoints; ++i)
            {
                arrX.SetTuple1(i, i * inc);
                arrC.SetTuple1(i, Math.Cos(i * inc) + 0.0);
                arrS.SetTuple1(i, Math.Sin(i * inc) + 0.0);

                //table.SetValue(i, 0, i * inc);
                //table.SetValue(i, 1, cos(i * inc));
                //table.SetValue(i, 2, sin(i * inc));
            }

            table.AddColumn(arrX);
            table.AddColumn(arrC);
            table.AddColumn(arrS);

            vtkContextView view = new vtkContextView();
            view.GetRenderer().SetBackground(1.0, 1.0, 1.0);

            vtkChartXY chart = new vtkChartXY();
            view.GetScene().AddItem(chart);
            vtkPlot line = chart.AddPlot(1);

            line.SetInput(table, 0, 1);
            line.SetInput(table, 0, 1);
            line.SetColor(0, 255, 0, 255);
            line.SetWidth(1);
            line = chart.AddPlot(1);

            line.SetInput(table, 0, 2);
            line.SetInput(table, 0, 2);
            line.SetColor(255, 0, 0, 255);
            line.SetWidth(5);
            line.GetPen().SetLineType(2);

            view.GetInteractor().Initialize();
            view.GetInteractor().Start();
        }

        public static void Test2()
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();
 
            vtkRenderer renderer = vtkRenderer.New();
 
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(sphereSource.GetOutputPort());
            

 
            vtkActor sphereActor = vtkActor.New();
            sphereActor.SetMapper(mapper);
 
            vtkCubeAxesActor cubeAxesActor = vtkCubeAxesActor.New();

            double[] bounds = new double[6];
            bounds = sphereSource.GetOutput().GetBounds();
            cubeAxesActor.SetBounds(bounds[0], bounds[1], bounds[2], bounds[3], bounds[4], bounds[5]);
            cubeAxesActor.SetCamera(renderer.GetActiveCamera());
 
            renderer.AddActor(cubeAxesActor);
            renderer.AddActor(sphereActor);
            renderer.ResetCamera();
 
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);
 
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);
 
            renderWindow.Render();
            renderWindowInteractor.Start();
        }

        public static void ColorBar()
        {
            // Create a sphere for some geometry
            vtkSphereSource sphere = vtkSphereSource.New();
            sphere.SetCenter(0,0,0);
            sphere.SetRadius(1);
            sphere.Update();

            // Create scalar data to associate with the vertices of the sphere
            int numPts = Convert.ToInt32(sphere.GetOutput().GetPoints().GetNumberOfPoints());          //GetOutput().GetPoints().GetNumberOfPoints();
            vtkFloatArray scalars =vtkFloatArray.New();
            scalars.SetNumberOfValues( numPts );
            for( int i = 0; i < numPts; ++i )
                {
                scalars.SetValue(i,(float)i/numPts);
                }
            vtkPolyData poly =vtkPolyData.New();
            poly.DeepCopy(sphere.GetOutput());
            poly.GetPointData().SetScalars(scalars);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(poly);
            mapper.ScalarVisibilityOn();
            mapper.SetScalarModeToUsePointData();
            mapper.SetColorModeToMapScalars();
 
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
 
            vtkScalarBarActor scalarBar = vtkScalarBarActor.New();
            //scalarBar.SetLookupTable(mapper.GetLookupTable());
            scalarBar.SetTitle("Title");
            scalarBar.SetNumberOfLabels(6);
            scalarBar.SetVisibility(1);
            
            //
            //
            //bool vertical = false;
            //if (vertical == true)
            //{
            //    scalarBar.SetDisplayPosition(10, 10);
            //    scalarBar.SetHeight(0);
            //    //scalarBar.SetMaximumHeightInPixels(500);
            //    //scalarBar.SetMaximumWidthInPixels(100);
            //    scalarBar.SetOrientationToVertical();
            //}
            //else
            //{

            //    scalarBar.SetDisplayPosition(10, 10);
            //    scalarBar.SetWidth(0.5);

            //    scalarBar.SetPosition(10, 10);
            //    scalarBar.SetHeight(0.15);
            //    //scalarBar.SetMaximumHeightInPixels(50);
            //    //scalarBar.SetOrientationToHorizontal();
            //    scalarBar.SetOrientation(0);
            //}
            
            

            double minLT = 5;
            double maxLT = 20;

            // Create a lookup table to share between the mapper and the scalarbar
            ColorScale scale = ColorScale.CreateBlueRedYellow(0.5, 2);
            double minCS = scale.MinValue;
            double maxCS = scale.MaxValue;
             
            vtkLookupTable hueLut =vtkLookupTable.New();
            int num = 10;
            hueLut.SetNumberOfTableValues(num);
            hueLut.SetNumberOfColors(num);
             hueLut.SetTableRange(minLT, maxLT);
             double halfstep = 0.5 * (maxLT - minLT) / (double)num;
             for (int i = 0; i < num; ++i)
            {
                double valueLT = halfstep + minLT + (double)i * (maxLT - minLT) / (double)num;
                double valueCS = minCS + (valueLT- minLT) * (maxCS - minCS) / (maxLT - minLT);
                Console.WriteLine(i + ": value on LT: " + valueLT + ", CS: " + valueCS);
                 //Console.WriteLine("Value on CS: " + valueCS);
                color col = scale.GetColor(valueCS);
                hueLut.SetTableValue(i, col.R, col.G, col.B, col.Opacity);
            }
            hueLut.SetValueRange (minLT, maxLT);
            
            hueLut.Build();
            

            //hueLut.SetTableRange (0, 0.5);
            //hueLut.SetHueRange (0, 0.5);
            //hueLut.SetSaturationRange (1, 1);
            
            



            //hueLut.SetNumberOfColors(10);

            mapper.SetLookupTable(hueLut);
            scalarBar.SetLookupTable(hueLut);

            // Create a renderer and render window
            vtkRenderer renderer = vtkRenderer.New();
 
            renderer.GradientBackgroundOn();
            renderer.SetBackground(1,1,1);
            renderer.SetBackground2(0,0,0);
 
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);

            // Create an interactor
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);
 
            // Add the actors to the scene
            renderer.AddActor(actor);



            vtkScalarBarActor scalarBar1 = vtkScalarBarActor.New();
            vtkLookupTable lookupTable1 = vtkLookupTable.New();
            int num1 = 10;
            // _scalarBarLookupTable.SetNumberOfTableValues(num);
            lookupTable1.SetNumberOfColors(num1);
            double min1 = 0;
            double max1 = 1;
            lookupTable1.SetTableRange(min1, max1);
            lookupTable1.SetHueRange(0, 1);
            lookupTable1.SetSaturationRange(0.5, 1);
            lookupTable1.SetAlpha(1);
            lookupTable1.SetValueRange(0, 1);

            scalarBar1.SetLookupTable(lookupTable1);


            renderer.AddActor2D(scalarBar);
 
            // Render the scene (lights and cameras are created automatically)
            renderWindow.Render();
            renderWindowInteractor.Start();
        }

        public static void Legend()
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.SetCenter(0.0, 0.0, 0.0);
            sphereSource.SetRadius(5.0);
            sphereSource.SetPhiResolution(40);
            sphereSource.SetThetaResolution(40);
            sphereSource.Update();

            vtkPolyData polydata = sphereSource.GetOutput();
            
            // Create a mapper
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(polydata);
 
            // Create an actor
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            // A renderer and render window
            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);

            // An interactor
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);

            vtkLegendScaleActor legendScale = vtkLegendScaleActor.New();
            vtkLegendBoxActor legendBox = vtkLegendBoxActor.New();
 
            //LEGENDBOX
            
            legendBox.SetNumberOfEntries(2);
            //legendBox.SetEntry(0, null, "LEGEND 1", color);
            vtkSphereSource legendSphereSource = vtkSphereSource.New();
            vtkPolyData legendSphere = legendSphereSource.GetOutput();
            

            vtkPlaneSource legendPlaneSource = vtkPlaneSource.New();
            vtkPolyData legendPlane = legendPlaneSource.GetOutput();

            vtkTextProperty text = vtkTextProperty.New();
            text.SetColor(0, 0, 1);
            text.SetFontSize(10);
            legendBox.SetEntryTextProperty(text);

            //Entry 1
            legendBox.SetEntrySymbol(0, legendSphere);
            legendBox.SetEntryString(0, "Label-1");
            

            //legendBox.SetHeight(0.1);
            //legendBox.SetWidth(0.2);
            //legendBox.SetEntryColor(0, 1,0,0);
            //legendBox.SetBox(0); // 0-normal 1-filled
            //legendBox.SetBorder(1); // 0-not visible 1-visible

            //Entry 2
            legendBox.SetEntrySymbol(1, legendPlane);
            legendBox.SetEntryString(1, "Label-2");
            //legendBox.SetEntryColor(1, 0,1,0);
            //legendBox.SetBox(0); // 0-normal 1-filled
            //legendBox.SetBorder(1); // 0-not visible 1-visible
            

            

            //LEGENDSCALE
            
            // Add the actors to the scene
            //renderer.AddActor(actor);
            //renderer.AddActor(legendScale);
            renderer.AddActor2D(legendBox);
            renderer.SetBackground(0, 0, 0); // Background color white

            // Render an image (lights and cameras are created automatically)
            renderWindow.Render();

            // Begin mouse interaction
            renderWindowInteractor.Start();
 
        }
     
        /// <summary>Example function of 2 variables to generate a surface plot.
        /// In the future, ScalarFunction class will be used for this purpose.</summary>
        private static double f(double x, double y)
        {
            return x * y;
        }
        
        // Number of cells in different directions:
        private static int numi = 6;
        private static int numj = 6;
        private static int numk = 2;

        // Minimal and maximal values in different directions:
        private static double minX = -1;
        private static double maxX = 1;
        private static double minY = -1;
        private static double maxY = 1;
        private static double minZ = -1;
        private static double maxZ = 1;
        private static double minValue = 0;
        private static double maxValue = 0;

        /// <summary>Returns x coordinate corresponding to the specific index.</summary>
        private static double GetX(int i)
        {
            return minX + (maxX - minX) * (double)i / (double) (numi - 1);
        }

        /// <summary>Returns y coordinate corresponding to the specific index.</summary>
        private static double GetY(int j)
        {
            return minY + (maxY - minY) * (double)j / (double) (numj - 1);
        }

        /// <summary>Returns z coordinate corresponding to the specific index.</summary>
        private static double GetZ(int k)
        {
            return minZ + (maxZ - minZ) * (double) k / (double) (numk - 1);
        }

        /// <summary>StopWatch for measuring time.</summary>
        static StopWatch timer = new StopWatch();


        /// <summary>Example of plotting parametric curves in 3D space.</summary>
        /// <param name="numT">Number fo divisions.</param>
        /// <remarks><para>See also:</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/PolyData/RibbonFilter</para>
        /// <para></para>
        /// </remarks>
        public static void ExampleParametricCurve(int numT)
        {
        }



        private static double f3d(double x, double y, double z)
        {
            double aux = (x + y + z);
            double ret = aux * aux;

            ret = x * x + 0.5 * y * y + 0.2 * z * z;

            return ret;
        }


        // TODO: see what is wrong with this definition of a vtkImplicitFunction-based class!

        /// <summary>An <see cref="vtkImplicitFunction"/> class that represents a custom function
        /// of 3 variables used for plotting contours.</summary>
        protected class CustomImplicitFunction  : vtkImplicitFunction
        {

            const int maxWriteVal = 5, maxWriteGrad = 5;
            int numWriteVal = 0, numWriteGrad = 0;

            public CustomImplicitFunction()
            {
                Console.WriteLine("\n\n\n  INSIDE CustomImplicitFunction Constructor. \n\n\n");
            }

            public override double EvaluateFunction(IntPtr param)
            {
                if (numWriteVal < maxWriteVal)
                {
                    Console.WriteLine("Evaluation of value called...");
                }
                double[] xx = new double[3];
                Marshal.Copy(param, xx, 0, 3);  // copy values from IntPtr to double[]
                double x = xx[0], y = xx[1], z = xx[2];
                double ret = x * x + 0.5 * y * y + 0.2 * z * z;
                if (numWriteVal < maxWriteVal)
                {
                    Console.WriteLine("  x = " + x + ", y = " + y + ", z = " + z + ", value = " + ret);
                    ++numWriteVal;
                }
                return ret;
            }

            public override void EvaluateGradient(IntPtr param, IntPtr grad)
            {
                if (numWriteGrad < maxWriteGrad)
                {
                    Console.WriteLine("Evaluation of value called...");
                }
                double[] xx = new double[3];
                double[] gg = new double[3];
                Marshal.Copy(param, xx, 0, 3);  // copy values from IntPtr to double[]
                double x = xx[0], y = xx[1], z = xx[2];
                double dx = 0, dy = 0, dz = 0;
                
                dx = 2 * x;
                dy = y;
                dz = 0.4 * z;

                gg[0] = dx; gg[1] = dy; gg[2] = dz;
                Marshal.Copy(gg, 0, grad, 3);  // copy values from double[] to IntPtr
                if (numWriteGrad < maxWriteGrad)
                {
                    Console.WriteLine("  x = " + x + ", y = " + y + ", z = " + z + ", grad = {" + dx + ", " + dy + ", " + dz + "}");
                    ++numWriteGrad;
                }
            }


            ///// <summary>This is alternative way of defining the <see cref="EvaluateGradient"/> function
            ///// - with unsafe code and direct access to pointers.</summary>
            //public override void EvaluateGradient
            //    (IntPtr param, IntPtr grad)
            //{
            //    unsafe
            //    {
            //        double* xPtr = (double*)param.ToPointer();
            //        double x = *(xPtr + 0);
            //        double y = *(xPtr + 1);
            //        double z = *(xPtr + 2);
            //        double derx = 1;
            //        double dery = 1;
            //        double derz = 1;
            //        double* gradPtr = (double*)grad.ToPointer();
            //        gradPtr[0] = derx;
            //        gradPtr[1] = dery;
            //        gradPtr[2] = derz;
            //    }
            //}
        }


        /// <summary>Example that demonstrates generaton of contours on a 3D structured grid.</summary>
        /// <remarks><para>See also:</para><para>http://www.vtk.org/Wiki/VTK/Examples/Cxx#vtkStructuredGrid</para></remarks>
        public static void ExampleStructuredGridVolumeContours()
        {
            ExampleStructuredGridVolumeContours(6, 6, 4, 6);
        }

        /// <summary>Example that demonstrates generaton of contours on a 3D structured grid.</summary>
        /// <param name="sizex">Number of points in x direction.</param>
        /// <param name="sizey">Number of points in y direction.</param>
        /// <param name="sizez">Number of points in z direction.</param>
        /// <remarks><para>See also:</para>
        /// <para>http://www.vtk.org/VTK/help/examplecode.html - generating contours of implicit function of 3 variables,
        /// probably useful only when you actually have an implicit function.
        /// http://www.vtk.org/Wiki/VTK/Examples/Cxx/ImplicitFunctions/SampleFunction - sample implicit function, 1 contour</para>
        /// <para></para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx#vtkStructuredGrid</para>
        /// <para>See Delaunay3D triangularization.
        /// http://public.kitware.com/cgi-bin/viewcvs.cgi/*checkout*/Examples/Modelling/Cxx/Delaunay3D.cxx?root=VTK&content-type=text/plain
        /// http://www.vtk.org/doc/release/5.6/html/a00464.html - Delaunay3D class reference
        /// http://www.vtk.org/Wiki/VTK/Examples/Cxx/Modelling/Delaunay3D - Delaunay triangularization in 3D
        /// - this is used to convert structured grid or any other form containing points to a 3d mesh of tetrahetrons. </para>
        /// <para>See vtkContourGrid 
        /// http://nullege.com/codes/show/src%40p%40y%40pyvisi-HEAD%40trunk%40examples%40renderers%40vtk%40isosurfacePlot.py/25/vtk.vtkContourGrid/python 
        ///   - Python example - triangulate and generate contour.
        /// http://www.vtk.org/doc/release/5.6/html/a00380.html - vtkContourGrid class reference.</para>
        /// <para></para>
        /// <para>See vtkBandedPolyDataContourFilter 
        /// http://www.vtk.org/doc/release/5.6/html/a00177.html - vtkBandedPolyDataContourFilter class reference. This does not
        /// generate inner contur surfaces, but only outer contours on 3D body surface.
        /// http://www.vtk.org/Wiki/VTK/Examples/Cxx/VisualizationAlgorithms/BandedPolyDataContourFilter - filled grids on surface
        /// </para>
        /// <para></para>
        /// </remarks>
        public static void ExampleStructuredGridVolumeContours(int sizex, int sizey, int sizez, int numContours)
        {
            numi = 10;
            numj = 10;
            numk = 2;

            if (sizex > 1)
                numi = sizex;
            if (sizey > 1)
                numj = sizey;
            if (sizez > 0)
                numk = sizez;


            Console.WriteLine();
            Console.WriteLine("Example: building surface graph by a structured grid, dimensions: " + sizex + "x"
                + sizey + "x" + sizez + ".");
            Console.WriteLine("Perparing the graph actors...");
            Console.WriteLine("Preparing points...");
            timer.Reset();
            timer.Start();


            BoundingBox pointBounds = new BoundingBox(4);
            // Get bounding box first:

            for (int k = 0; k < numk; k++)
            {
                for (int j = 0; j < numj; j++)
                {
                    for (int i = 0; i < numi; i++)
                    {
                        double 
                            x, y, z, // coordinates of grid points
                            xRef, yRef, zRef; // reference coordinates used for evaluetion of 3D functions
                        xRef = x = GetX(i);
                        yRef = y = GetY(j);
                        z = f(x, y) + 2 * ((double)k / (double)numk);
                        zRef = GetZ(k);
                        double value = f3d(xRef, yRef, zRef);
                        pointBounds.Update(x, y, z, value);
                    }
                }
            }
            minZ = pointBounds.GetMin(2);
            maxZ = pointBounds.GetMax(2);
            minValue = pointBounds.GetMin(3);
            maxValue = pointBounds.GetMax(3);

            Console.WriteLine("Range of values (bounding box containing all geometric objects):");
            Console.WriteLine(pointBounds.ToString());



            // create grid
            vtkStructuredGrid structuredGrid = vtkStructuredGrid.New();
            vtkStructuredGrid structuredGridContours = vtkStructuredGrid.New();
            vtkContourGrid contourGrid = vtkContourGrid.New();
            vtkPoints structuredPoints = vtkPoints.New();
            // Point Colors:
            vtkUnsignedCharArray colorsPoints = vtkUnsignedCharArray.New();
            colorsPoints.SetNumberOfComponents(3);
            colorsPoints.SetName("Point colors.");

            // Create array of point values:
            vtkDoubleArray scalarPointValues = vtkDoubleArray.New();
            scalarPointValues.SetNumberOfComponents(1);
            string scalarPointValuesString = "Point values";
            scalarPointValues.SetName(scalarPointValuesString);


            // Color scale for wireframe
            IColorScale pointColorScale;
            pointColorScale = ColorScale.Create(minZ, maxZ, Color.Blue, Color.Red, Color.Yellow);
            pointColorScale = ColorScale.Create(minZ, maxZ, Color.Green, Color.Yellow);

            for (int k = 0; k < numk; k++)
            {
                for (int j = 0; j < numj; j++)
                {
                    for (int i = 0; i < numi; i++)
                    {
                        double
                            x, y, z, // coordinates of grid points
                            xRef, yRef, zRef; // reference coordinates used for evaluetion of 3D functions
                        xRef = x = GetX(i);
                        yRef = y = GetY(j);
                        z = f(x, y) + 2 * ((double)k / (double)numk);
                        zRef = GetZ(k);
                        double value = f3d(xRef, yRef, zRef);
                        
                        structuredPoints.InsertNextPoint(x, y, z);

                        scalarPointValues.InsertNextTuple1(value);

                        //color pointColor = color.ScaleBlueRedYellow(minZ, maxZ, z);
                        color pointColor = pointColorScale.GetColor(z);
                        colorsPoints.InsertNextTuple3(pointColor.IntR, pointColor.IntG, pointColor.IntB);
                    }
                }
            }

            timer.Stop();
            Console.WriteLine("Preparation of points of the structured grid done in " + timer.Time + " s.");
            timer.Start();

            Console.WriteLine("Bounding box: ");
            Console.WriteLine("" + pointBounds);

            // Specify the dimensions of the structured grid
            structuredGrid.SetDimensions(numi, numj, numk);
            structuredGrid.SetPoints(structuredPoints);

            // auxiliary objects:
            vtkDataSetMapper datasetMapper;
            vtkPolyDataMapper mapper;
            vtkProperty properties;

            //// contour
            //vtkContourFilter contourFilter = vtkContourFilter.New();
            //contourFilter.SetInputConnection(structuredGrid.GetProducerPort());
            //contourFilter.Update();

            //mapper = vtkPolyDataMapper.New();
            //mapper.SetInputConnection(contourFilter.GetOutputPort());

            //vtkActor actorContour = vtkActor.New();
            //actorContour.SetMapper(mapper);
            //properties = actorContour.GetProperty();
            //properties.SetColor(1, 0, 0);
            //properties.SetRepresentationToWireframe();


            // Assign colors to points of the grid:
            // Remark: COMMENT THE LINE BELOW IN ORDER TO REMOVE SMOOTH COLOR TRANSITIONS!
            structuredGrid.GetPointData().SetScalars(colorsPoints);

            // Actor that represents the structured grid without filtering:
            datasetMapper = vtkDataSetMapper.New();
            datasetMapper.SetInputConnection(structuredGrid.GetProducerPort());
            vtkActor actorPlain = vtkActor.New();
            actorPlain.SetMapper(datasetMapper);
            properties = actorPlain.GetProperty();
            properties.SetColor(0.0, 0.5, 1);
            properties.SetOpacity(0.2);

            // Actor that represents points of the structured grid. Does not work if the 3rd dimension is 1.
            vtkStructuredGridGeometryFilter geometryFilterPoints = vtkStructuredGridGeometryFilter.New();
            geometryFilterPoints.SetInputConnection(structuredGrid.GetProducerPort());
            geometryFilterPoints.Update();

            mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(geometryFilterPoints.GetOutputPort());
            vtkActor actorPoints = vtkActor.New();
            actorPoints.SetMapper(mapper);
            properties = actorPoints.GetProperty();
            properties.SetColor(0.0, 1.0, 0.0);
            properties.SetRepresentationToPoints();
            properties.SetPointSize(2);


            // Actor that represents lines of the structured grid. Does not work if the 3rd dimension is >1.
            vtkStructuredGridGeometryFilter geometryFilterLines = vtkStructuredGridGeometryFilter.New();
            geometryFilterLines.SetInputConnection(structuredGrid.GetProducerPort());
            geometryFilterLines.Update();

            mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(geometryFilterLines.GetOutputPort());
            vtkActor actorLines = vtkActor.New();
            actorLines.SetMapper(mapper);
            properties = actorLines.GetProperty();
            //properties.SetColor(0.0, 1.0, 0.0);
            properties.SetRepresentationToSurface();
            //properties.SetRepresentationToWireframe();
            //properties.EdgeVisibilityOn();
            //properties.SetEdgeVisibility(10);
            //properties.SetEdgeColor(1, 0, 0);
            properties.SetLineWidth(2);

            // Actor that represents outline of the structured grid.
            vtkStructuredGridOutlineFilter geometryFilterOutline = vtkStructuredGridOutlineFilter.New();
            geometryFilterOutline.SetInputConnection(structuredGrid.GetProducerPort());
            geometryFilterOutline.Update();

            mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(geometryFilterOutline.GetOutputPort());
            vtkActor actorOutline = vtkActor.New();
            actorOutline.SetMapper(mapper);
            properties = actorOutline.GetProperty();
            properties.SetColor(1.0, 0.0, 0.0);
            properties.SetLineWidth(2);

            // Actor that represents structured grid cells.
            vtkShrinkFilter shrinkFilter = vtkShrinkFilter.New();
            shrinkFilter.SetInputConnection(structuredGrid.GetProducerPort());
            shrinkFilter.SetShrinkFactor(1);



            datasetMapper = vtkDataSetMapper.New();
            datasetMapper.SetInputConnection(shrinkFilter.GetOutputPort());
            vtkActor actorShrink = vtkActor.New();
            actorShrink.SetMapper(datasetMapper);
            properties = actorShrink.GetProperty();
            properties.SetRepresentationToWireframe();
            properties.SetLineWidth(1.5f);
            properties.SetColor(1, 0, 0);



            // VOLUMETRIC CONTOURS:

            //Prepare grid for used for contour plots:
            structuredGridContours.SetDimensions(numi, numj, numk);
            structuredGridContours.SetPoints(structuredPoints);
            // Set values on basis of which contours are plot:
            structuredGridContours.GetPointData().SetScalars(scalarPointValues);


            // Convert structured grid to polydata:
            vtkGeometryFilter geometryFilter = vtkGeometryFilter.New();
            geometryFilter.SetInput(structuredGridContours);
            geometryFilter.Update(); 
            vtkPolyData contourPolyData = geometryFilter.GetOutput();



            // Prepare data for contours:
            // Assign point values (1 value per each point):
            contourPolyData.GetPointData().SetScalars(scalarPointValues);
            Console.WriteLine();
            // Check range:
            double[] valuesRange;
            valuesRange = contourPolyData.GetPointData().GetScalars().GetRange();
            Console.WriteLine("Range of values: from " + minValue + " to " + maxValue + ".");
            Console.WriteLine("Range obtaines form vtkPolyData (converted from vtkStructuredGrid): ");
            if (valuesRange == null)
                Console.WriteLine("null");
            else
            {
                Console.WriteLine("  length: " + valuesRange.Length);
                Console.Write("  Components: ");
                for (int i = 0; i < valuesRange.Length; ++i)
                    Console.Write(valuesRange[i] + " ");
                Console.WriteLine();
            }

            // Create banded contour filter and generate contour values:
            vtkBandedPolyDataContourFilter contourFilter = vtkBandedPolyDataContourFilter.New();
            //vtkContourFilter contourFilter = vtkContourFilter.New();
            contourFilter.SetInputConnection(contourPolyData.GetProducerPort());

            // Generate numContours equally spaced contours (individual contours may be added by SetValue()):
            contourFilter.GenerateValues(numContours, valuesRange[0], valuesRange[1]);

            //// Manually set contour values:
            //contourFilter.SetValue(0, 0.1);
            //contourFilter.SetValue(1, 0.2);
            //contourFilter.SetValue(2, 0.3);
            //contourFilter.SetValue(3, 0.4);
            //contourFilter.SetValue(4, 0.5);
            //contourFilter.SetValue(5, 0.6);
            //contourFilter.SetValue(6, 0.7);
            //contourFilter.SetValue(7, 0.8);
            //contourFilter.SetValue(8, 0.85);
            //contourFilter.SetValue(9, 0.87);
            //contourFilter.SetValue(10, 0.89);
            //contourFilter.SetValue(11, 0.92);


            contourFilter.GenerateContourEdgesOn();  // Must be enabled if we want to make line contours (not only filled)
            contourFilter.Update();

            // Color the contours
            // This colors contours obtained on output port 1 by just setting point values that are 
            // obtained from the output port 0 (here points are obtained through inputconnection from 
            // vtkPolyData object surfaceContourPolyData, which is cource of geometry and topology)
            contourFilter.GetOutput(1).GetPointData().SetScalars(contourFilter.GetOutput().GetPointData().GetScalars());

            //contourFilter.GetOutput(1).GetPointData().SetScalars(colorsPoints);

            // Make sure the mapper uses the new colors
            //bf.GetOutput(0).GetPointData().SetActiveScalars(scalarPointValuesString);

            // TODO: Figure out how to apply custom colors to contours! Now the contour filter uses
            // point data (it could also use cell data) for contours and colors them according to
            // scalar values assigned to points, but we do not specify which colors it uses for which 
            // contours (or point values, respectively)


            // Prepare mapper and actor for solid contours:
            vtkPolyDataMapper surfaceContourMapper = vtkPolyDataMapper.New();
            surfaceContourMapper.SetInputConnection(contourFilter.GetOutputPort(0));  // SWITCH BETWEEN FILLED AND LINE CONTOURS
            surfaceContourMapper.SetScalarRange(valuesRange[0], valuesRange[1]);

            // TODO: Figure out how different settings below influens coloring of filled contours!


            surfaceContourMapper.SetScalarModeToUsePointData();

            //surfaceContourMapper.SetScalarModeToUsePointFieldData();

            //surfaceContourMapper.SetScalarModeToUseFieldData();

            //surfaceContourMapper.SetScalarModeToUseCellData();

            //surfaceContourMapper.SetScalarModeToUseCellFieldData();

            vtkActor surfaceContourActor = vtkActor.New();
            surfaceContourActor.SetMapper(surfaceContourMapper);
            vtkProperty surfaceContourProperties = surfaceContourActor.GetProperty();
            surfaceContourProperties.SetColor(0, 0, 1);
            surfaceContourProperties.SetLineWidth(3);
            surfaceContourProperties.SetOpacity(0.5);


            // Prepare mapper and actor for contour lines:
            vtkPolyDataMapper contourLinesMapper = vtkPolyDataMapper.New();
            contourLinesMapper.SetInputConnection(contourFilter.GetOutputPort(1));  // SWITCH BETWEEN FILLED AND LINE CONTOURS
            vtkActor contourLinesActor = vtkActor.New();
            contourLinesActor.SetMapper(contourLinesMapper);
            vtkProperty contourLinesProperties = contourLinesActor.GetProperty();
            contourLinesProperties.SetColor(0.0, 0.0, 1.0);
            contourLinesProperties.SetLineWidth(3);


            //// CONTOURS DIRECTLY FROM STRUCTUREDGRID INSTEAD OF POLYDATA BY USING vtkContourGrid:

            //// Prepare data for contours:
            //// Assign point values (1 value per each point):
            //structuredGridContours.GetPointData().SetScalars(scalarPointValues);
            //Console.WriteLine();
            //// Check range:
            //valuesRange = structuredGridContours.GetPointData().GetScalars().GetRange();
            //Console.WriteLine("RANGE of z values: from " + minZ + " to " + maxZ + ".");
            //Console.WriteLine("Range obtaines form vtkStructuredGrid: ");
            //if (valuesRange == null)
            //    Console.WriteLine("null");
            //else
            //{
            //    Console.WriteLine("  length: " + valuesRange.Length);
            //    Console.Write("  Components: ");
            //    for (int i = 0; i < valuesRange.Length; ++i)
            //        Console.Write(valuesRange[i] + " ");
            //    Console.WriteLine();
            //}

            //// Create banded contour filter and generate contour values:
            //vtkContourGrid contourGridFilter = vtkContourGrid.New();
            //contourGridFilter.SetInputConnection(structuredGridContours.GetProducerPort());
            ////contourGridFilter.SetInput(structuredGridContours);

            //// Generate numContours equally spaced contours (individual contours may be added by SetValue()):
            //contourGridFilter.GenerateValues(numContours, valuesRange[0], valuesRange[1]);

            ////// Manually set contour values:
            ////contourFilter.SetValue(0, 0.1);
            ////contourFilter.SetValue(1, 0.2);
            ////contourFilter.SetValue(2, 0.3);
            ////contourFilter.SetValue(3, 0.4);
            ////contourFilter.SetValue(4, 0.5);
            ////contourFilter.SetValue(5, 0.6);
            ////contourFilter.SetValue(6, 0.7);
            ////contourFilter.SetValue(7, 0.8);
            ////contourFilter.SetValue(8, 0.85);
            ////contourFilter.SetValue(9, 0.87);
            ////contourFilter.SetValue(10, 0.89);
            ////contourFilter.SetValue(11, 0.92);


            //// contourGridFilter.GenerateContourEdgesOn();  // Must be enabled if we want to make line contours (not only filled)
            //contourGridFilter.Update();

            //// Color the contours
            //// This colors contours obtained on output port 1 by just setting point values that are 
            //// obtained from the output port 0 (here points are obtained through inputconnection from 
            //// vtkStructuredGrid object surfaceContourPolyData, which is cource of geometry and topology)
            //// contourGridFilter.GetOutput(1).GetPointData().SetScalars(contourGridFilter.GetOutput().GetPointData().GetScalars());

            ////contourGridFilter.GetOutput(1).GetPointData().SetScalars(colorsPoints);

            //// Make sure the mapper uses the new colors
            ////contourGridFilter.GetOutput(0).GetPointData().SetActiveScalars(scalarPointValuesString);

            //// TODO: Figure out how to apply custom colors to contours! Now the contour filter uses
            //// point data (it could also use cell data) for contours and colors them according to
            //// scalar values assigned to points, but we do not specify which colors it uses for which 
            //// contours (or point values, respectively)


            //// Prepare mapper and actor for solid contours:
            //vtkPolyDataMapper gridContourMapper = vtkPolyDataMapper.New();
            //gridContourMapper.SetInputConnection(contourGridFilter.GetOutputPort(0));  // SWITCH BETWEEN FILLED AND LINE CONTOURS
            //gridContourMapper.SetScalarRange(valuesRange[0], valuesRange[1]);

            //// TODO: Figure out how different settings below influens coloring of filled contours!


            //gridContourMapper.SetScalarModeToUsePointData();

            ////surfaceContourMapper.SetScalarModeToUsePointFieldData();

            ////surfaceContourMapper.SetScalarModeToUseFieldData();

            ////surfaceContourMapper.SetScalarModeToUseCellData();

            ////surfaceContourMapper.SetScalarModeToUseCellFieldData();

            //vtkActor surfaceGridContourActor = vtkActor.New();
            //surfaceGridContourActor.SetMapper(gridContourMapper);
            //vtkProperty surfaceGridContourProperties = surfaceGridContourActor.GetProperty();
            //surfaceGridContourProperties.SetColor(0, 0, 1);
            //surfaceGridContourProperties.SetLineWidth(3);


            //// Prepare mapper and actor for contour lines:
            //vtkPolyDataMapper contourGridLinesMapper = vtkPolyDataMapper.New();
            //contourGridLinesMapper.SetInputConnection(contourGridFilter.GetOutputPort(1));  // SWITCH BETWEEN FILLED AND LINE CONTOURS
            //vtkActor contourGridLinesActor = vtkActor.New();
            //contourGridLinesActor.SetMapper(contourGridLinesMapper);
            //vtkProperty contourGridLinesProperties = contourGridLinesActor.GetProperty();
            //contourGridLinesProperties.SetColor(0.0, 0.0, 1.0);
            //contourGridLinesProperties.SetLineWidth(3);


            // VOLUMETRIC CONTOURS USING DELAUNAY TRIAGULARIZATION IN 3D
            

            // Convert structured grid to polydata:
            vtkGeometryFilter geometryFilter1 = vtkGeometryFilter.New();
            geometryFilter1.SetInput(structuredGridContours);
            geometryFilter1.Update(); 
            vtkPolyData contourPolyDataDelaunay = geometryFilter.GetOutput();
             
            // Clean the polydata. This will remove duplicate points that may be
            // present in the input data.
            vtkCleanPolyData cleaner = vtkCleanPolyData.New();
            cleaner.SetInputConnection (contourPolyDataDelaunay.GetProducerPort());

            // Generate a tetrahedral mesh from the input points. By default, the generated 
            // volume is the convex hull of the points.
            vtkDelaunay3D delaunay3D = vtkDelaunay3D.New();
            //delaunay3D.SetOffset(2.5);
            // delaunay3D.SetTolerance(1.0e-50);
            //delaunay3D.SetAlpha(0.0);
            delaunay3D.SetInputConnection(cleaner.GetOutputPort());

            // Actor for showing Delaunay results:
            vtkDataSetMapper delaunayMapper =
            vtkDataSetMapper.New();
            delaunayMapper.SetInputConnection(delaunay3D.GetOutputPort());
            vtkActor delaunayActor = vtkActor.New();
            delaunayActor.SetMapper(delaunayMapper);
            delaunayActor.GetProperty().SetColor(1,0,0);

            // set up a contour filter (type vtkContourGrid)
            vtkContourGrid delaunayContourGridFilter = vtkContourGrid.New();
            delaunayContourGridFilter.SetInput(delaunay3D.GetOutput());
            delaunayContourGridFilter.GenerateValues(numContours, minValue, maxValue);
  
            // SUBDIVIDE MESH CREATED BY CONTOUR FILTER:
            // http://www.vtk.org/Wiki/VTK/Examples/Cxx/Meshes/Subdivision

            vtkPolyDataAlgorithm subDivisionFilter;
            int numSubdivisions = 2;  // This defines how the contour will be smoothed!
            //vtkLinearSubdivisionFilter f1 = vtkLinearSubdivisionFilter.New();
            //f1.SetNumberOfSubdivisions(numSubdivisions); subDivisionFilter = f1;
            //vtkButterflySubdivisionFilter f2 = vtkButterflySubdivisionFilter.New();
            //f2.SetNumberOfSubdivisions(numSubdivisions); subDivisionFilter = f2;
            vtkLoopSubdivisionFilter f3 = vtkLoopSubdivisionFilter.New();
            f3.SetNumberOfSubdivisions(numSubdivisions); subDivisionFilter = f3;

            subDivisionFilter.SetInputConnection(delaunayContourGridFilter.GetOutputPort());
            subDivisionFilter.Update();
            vtkPolyDataMapper delaunayContoursSubdividedMapper = vtkPolyDataMapper.New();
            delaunayContoursSubdividedMapper.SetInputConnection(subDivisionFilter.GetOutputPort());
            vtkActor delaunayContoursSubdividedActor = vtkActor.New();
            // Set up actor:
            delaunayContoursSubdividedActor.SetMapper(delaunayContoursSubdividedMapper);
            vtkProperty delaunayContourSubdividedProperties = delaunayContoursSubdividedActor.GetProperty();
            delaunayContourSubdividedProperties.SetOpacity(1);


            // DEFINE SMOOTHING FILTER:
            vtkSmoothPolyDataFilter smoothFilter = vtkSmoothPolyDataFilter.New();
            // Convert structured grid to polydata:
            vtkGeometryFilter geometryFilterForSmoothing = vtkGeometryFilter.New();
            geometryFilterForSmoothing.SetInput(delaunayContourGridFilter.GetOutput());
                // (delaunayContourGridFilter.GetOutputPort());
            geometryFilterForSmoothing.Update();
            vtkPolyData smoothedPolyData = geometryFilter.GetOutput();

            smoothFilter.SetInputConnection(delaunayContourGridFilter.GetOutputPort());

            // Set up the mapper:
            // Either apply smoothing or not:
            // Uncomment two lines below in order to apply smoothing!
            //vtkPolyDataMapper delaunayContoursMapper = vtkPolyDataMapper.New();
            //delaunayContoursMapper.SetInput(smoothedPolyData);

            // Uncomment two lines below if you do not want to apply smoothing (in this case, comment two lines above)!
            vtkDataSetMapper delaunayContoursMapper = vtkDataSetMapper.New();
            delaunayContoursMapper.SetInput(delaunayContourGridFilter.GetOutput());

            delaunayContoursMapper.ScalarVisibilityOn();
  
            // Set up the actor
            vtkActor delaunayContoursActor = vtkActor.New();
            delaunayContoursActor.SetMapper(delaunayContoursMapper);
            vtkProperty delaunayContourProperties = delaunayContoursActor.GetProperty();
            delaunayContourProperties.SetOpacity(1);
  


            // CONTOURS FROM SAMPLING IMPLICIT FUNCTION:

            // create the quadric function definition
            vtkQuadric quadric = vtkQuadric.New();  // vtkQuadric is a bult-in implicit function.
            quadric.SetCoefficients(.5,1,.2,0,.1,0,0,.2,0,0);
            // sample the quadric function
            vtkSampleFunction sample = vtkSampleFunction.New();
            sample.SetSampleDimensions(sizex, sizey, sizez);
            sample.SetModelBounds(minX, maxX, minY, maxY, minZ, maxZ);
            
            //// Try to use own implicit function:
            //CustomImplicitFunction myFunc = null;
            //try
            //{
            //    myFunc = new CustomImplicitFunction();  // user defined implicit function.
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Exception: " + ex);
            //}
            //try
            //{
            //    Console.WriteLine();
            //    double val;
            //    val = myFunc.FunctionValue(1, 1, 3);
            //    Console.WriteLine("Value of custom functon at {1, 1, 2}: " + val);
            //    Console.WriteLine();
            //}
            //catch (Exception ex) {
            //    Console.WriteLine("Exception: " + ex);
            //}
            //vtkImplicitFunction customFunction = myFunc;
            //sample.SetImplicitFunction(customFunction);
            
            sample.SetImplicitFunction(quadric);

            // Create five surfaces F(x,y,z) = constant between range specified
            vtkContourFilter contours = vtkContourFilter.New();
            
            contours.SetInput(sample.GetOutput());
            contours.GenerateValues(numContours, 0.0, 1.2);

            // map the contours to graphical primitives
            vtkPolyDataMapper contMapper = vtkPolyDataMapper.New();
            contMapper.SetInput(contours.GetOutput());
            contMapper.SetScalarRange(0.0, 1.2);

            // create an actor for the contours
            vtkActor contActor = vtkActor.New();
            contActor.SetMapper(contMapper);
            vtkProperty contProps = contActor.GetProperty();
            contProps.SetOpacity(0.8);


            timer.Stop();
            Console.WriteLine("Preparation of actors done in " + timer.TotalTime + " s, ");
            Console.WriteLine("  without preparation of poinst: " + timer.Time + " s.");
            timer.Start();

            // Visualiize the grid:

            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);
            renderer.SetBackground(0.08, 0.08, 0.15);


            renderer.AddActor(actorPlain);

            //renderer.AddActor(actorPoints);

            //renderer.AddActor(actorLines);

            //renderer.AddActor(actorOutline);

            //renderer.AddActor(actorShrink);


            // CONTOURS:

            //renderer.AddActor(surfaceContourActor);

            //renderer.AddActor(contourLinesActor);



            // Contours form DELAUNAY triangularization of structured grid data (through PolyData):

            //renderer.AddActor(delaunayActor);

            //renderer.AddActor(delaunayContoursActor);

            //renderer.AddActor(delaunayContoursSubdividedActor);


            // Contours created directly from a structured grid, rather than from polydata converted from grid
            
            //renderer.AddActor(surfaceGridContourActor);

            //renderer.AddActor(contourGridLinesActor);

            // Contours from sampled IMPLICIT FUNCTION: 

            renderer.AddActor(contActor); 


            // Add cube axes to the renderer:
            vtkCubeAxesActor cubeAxesActor = vtkCubeAxesActor.New();

            VtkDecorationHandler axesHandler = new VtkDecorationHandler(renderWindow, renderer);
            axesHandler.CubeAxesColor = System.Drawing.Color.Yellow;
            axesHandler.CubeAxesFlyMode = VtkFlyMode.Static;


            renderWindow.Render();
            renderWindow.SetSize(1000, 700);



            timer.Stop();
            Console.WriteLine("Preparation for rendering done in " + timer.Time + "s.");
            Console.WriteLine("Total preparation time: " + timer.TotalTime + " s.");
            Console.WriteLine();
            Console.WriteLine();

            renderWindowInteractor.Start();
        } // ExampleStructuredGridVolumeContours(...)





        /// <summary>ExamExample of plotting contours on surfaces in 3D (graphs of functions of 2 variables 
        /// or parametric surfaces) by using graphic primitives (cells) connected to polydata.
        /// Default division numbers of the shown graph are taken.</summary>
        /// <param name="sizex">Number of points in x direction.</param>
        /// <param name="sizey">Number of points in y direction.</param>
        /// <remarks><para>See also:</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/LongLine</para>
        /// </remarks>
        public static void ExampleCellsGridContours()
        { ExampleCellsGridContours(15, 15, 8); }

        /// <summary>Example of plotting contours on surfaces in 3D (graphs of functions of 2 variables 
        /// or parametric surfaces) by using graphic primitives (cells) connected to polydata.</summary>
        /// <param name="sizex">Number of points in x direction.</param>
        /// <param name="sizey">Number of points in y direction.</param>
        /// <param name="numContours">Number of contours to be plotted.</param>
        /// <remarks><para>See also:</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/Quad - quadric cells</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/ColoredLines - for adding line cells</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/VisualizationAlgorithms/BandedPolyDataContourFilter - discrete consours</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/PolyData/PolyDataIsoLines - for contours</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/VisualizationAlgorithms/FilledContours - this is more complex, approach taken here is simpler.</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/Visualization/LabelContours - labeling contours</para>
        /// </remarks>
        public static void ExampleCellsGridContours(int sizex, int sizey, int numContours)
        {


            numi = 20;
            numj = 20;
            numk = 1;

            if (sizex > 1)
                numi = sizex;
            if (sizey > 1)
                numj = sizey;

            minX = -1;
            maxX = 1;
            minY = -1;
            maxY = 1;

            Console.WriteLine();
            Console.WriteLine("Example: building surface graph by PolyData containing all cells, dimensions: " + numi + "x"
                + numj + "x" + numk + ".");
            Console.WriteLine("Perparing the graph actors...");
            Console.WriteLine("Preparing points...");
            timer.Reset();
            timer.Start();


            //bool drawLineframe = true;
            //bool drawQuads = true;
            //bool useColors = true;


            //Determine the bounding box of calculated points that will be used as 
            //Cell vertices:
            BoundingBox pointBounds = new BoundingBox(3);
            for (int j = 0; j < numj - 1; j++)
            {
                for (int i = 0; i < numi - 1; i++)
                {
                    double
                        x1 = GetX(i),
                        y1 = GetY(j),
                        z1 = f(x1, y1),
                        x2 = GetX(i + 1),
                        y2 = GetY(j),
                        z2 = f(x2, y2),
                        x3 = GetX(i + 1),
                        y3 = GetY(j + 1),
                        z3 = f(x3, y3),
                        x4 = GetX(i),
                        y4 = GetY(j + 1),
                        z4 = f(x4, y4);
                    pointBounds.Update(x1, y1, z1);
                    pointBounds.Update(x2, y2, z2);
                    pointBounds.Update(x3, y3, z3);
                    pointBounds.Update(x4, y4, z4);
                }
            }
            double minZ = pointBounds.GetMin(2);
            double maxZ = pointBounds.GetMax(2);

            Console.WriteLine("Calculation of bounding box for calculated points finished in " + timer.Time + " s.");
            Console.WriteLine("    Bounds on z: " + minZ + ", " + maxZ + ".");

            // Data for wireframe:
            vtkPoints lineFramePoints = vtkPoints.New();
            vtkCellArray lineFrameLines = vtkCellArray.New();
            vtkUnsignedCharArray colorsPoints = vtkUnsignedCharArray.New();
            colorsPoints.SetNumberOfComponents(3);
            colorsPoints.SetName("Point colors");
            vtkUnsignedCharArray colorsLines = vtkUnsignedCharArray.New();
            colorsLines.SetNumberOfComponents(3);
            colorsLines.SetName("Line colors");

            // Data for surfaces:
            vtkPoints surfaceContourPoints = vtkPoints.New();
            vtkCellArray surfaceContourCells = vtkCellArray.New();
            vtkUnsignedCharArray colorsSurfacePoints = vtkUnsignedCharArray.New();
            colorsSurfacePoints.SetNumberOfComponents(3);
            colorsSurfacePoints.SetName("Surface point colors");
            vtkUnsignedCharArray colorsSurfaces = vtkUnsignedCharArray.New();
            colorsSurfaces.SetNumberOfComponents(3);
            colorsSurfaces.SetName("Surface colors");

            // Create array of point values:
            vtkDoubleArray scalarPointValues = vtkDoubleArray.New();
            scalarPointValues.SetNumberOfComponents(1);
            string scalarPointValuesString = "Point values";
            scalarPointValues.SetName(scalarPointValuesString);


            // Color scale for points:
            IColorScale pointScale = ColorScale.Create(minZ, maxZ, Color.Blue, Color.Red, Color.Yellow);

            // Colors for surfaces:
            System.Drawing.Color lowColor = System.Drawing.Color.DarkCyan;
            System.Drawing.Color highColor = System.Drawing.Color.Yellow;
            ColorScale surfacePointScale = ColorScale.Create(minZ, maxZ, lowColor, highColor);

            int firstPointId = 0;
            for (int j = 0; j < numj - 1; j++)
            {
                for (int i = 0; i < numi - 1; i++)
                {

                    // Vertices' coordinates:
                    double
                        x1 = GetX(i),
                        y1 = GetY(j),
                        z1 = f(x1, y1),
                        x2 = GetX(i + 1),
                        y2 = GetY(j),
                        z2 = f(x2, y2),
                        x3 = GetX(i + 1),
                        y3 = GetY(j + 1),
                        z3 = f(x3, y3),
                        x4 = GetX(i),
                        y4 = GetY(j + 1),
                        z4 = f(x4, y4);

                    //Add scalar values corresponding to the 4 vertices:
                    scalarPointValues.InsertNextTuple1(z1);
                    scalarPointValues.InsertNextTuple1(z2);
                    scalarPointValues.InsertNextTuple1(z3);
                    scalarPointValues.InsertNextTuple1(z4);

                    color pointColor1 = pointScale.GetColor(z1);
                    color pointColor2 = pointScale.GetColor(z2);
                    color pointColor3 = pointScale.GetColor(z3);
                    color pointColor4 = pointScale.GetColor(z4);

                    colorsPoints.InsertNextTuple3(pointColor1.IntR, pointColor1.IntG, pointColor1.IntB);
                    colorsPoints.InsertNextTuple3(pointColor2.IntR, pointColor2.IntG, pointColor2.IntB);
                    colorsPoints.InsertNextTuple3(pointColor3.IntR, pointColor3.IntG, pointColor3.IntB);
                    colorsPoints.InsertNextTuple3(pointColor4.IntR, pointColor4.IntG, pointColor4.IntB);

                    color lineColor1 = color.Average(pointColor1, pointColor2);
                    color lineColor2 = color.Average(pointColor2, pointColor3);
                    color lineColor3 = color.Average(pointColor3, pointColor4);
                    color lineColor4 = color.Average(pointColor4, pointColor1);
                    colorsLines.InsertNextTuple3(lineColor1.IntR, lineColor1.IntG, lineColor1.IntB);
                    colorsLines.InsertNextTuple3(lineColor2.IntR, lineColor2.IntG, lineColor2.IntB);
                    colorsLines.InsertNextTuple3(lineColor3.IntR, lineColor3.IntG, lineColor3.IntB);
                    colorsLines.InsertNextTuple3(lineColor4.IntR, lineColor4.IntG, lineColor4.IntB);


                    //color surfacePointColor1 = color.Scale(lowColor, highColor, minZ, maxZ, z1);
                    //color surfacePointColor2 = color.Scale(lowColor, highColor, minZ, maxZ, z2);
                    //color surfacePointColor3 = color.Scale(lowColor, highColor, minZ, maxZ, z3);
                    //color surfacePointColor4 = color.Scale(lowColor, highColor, minZ, maxZ, z4);

                    color surfacePointColor1 = surfacePointScale.GetColor(z1);
                    color surfacePointColor2 = surfacePointScale.GetColor(z2);
                    color surfacePointColor3 = surfacePointScale.GetColor(z3);
                    color surfacePointColor4 = surfacePointScale.GetColor(z4);


                    colorsSurfacePoints.InsertNextTuple3(surfacePointColor1.IntR, surfacePointColor1.IntG, surfacePointColor1.IntB);
                    colorsSurfacePoints.InsertNextTuple3(surfacePointColor2.IntR, surfacePointColor2.IntG, surfacePointColor2.IntB);
                    colorsSurfacePoints.InsertNextTuple3(surfacePointColor3.IntR, surfacePointColor3.IntG, surfacePointColor3.IntB);
                    colorsSurfacePoints.InsertNextTuple3(surfacePointColor4.IntR, surfacePointColor4.IntG, surfacePointColor4.IntB);

                    color surfaceColor = color.Average(surfacePointColor1, surfacePointColor2, surfacePointColor3, surfacePointColor4);
                    colorsSurfaces.InsertNextTuple3(surfaceColor.IntR, surfaceColor.IntG, surfaceColor.IntB);



                    //Add lines for the wireframe:
                    //Add 4 corner points of the cell to the list of points:
                    lineFramePoints.InsertNextPoint(x1, y1, z1);
                    lineFramePoints.InsertNextPoint(x2, y2, z2);
                    lineFramePoints.InsertNextPoint(x3, y3, z3);
                    lineFramePoints.InsertNextPoint(x4, y4, z4);
                    //Create 4 lines to outline the current cell:
                    vtkLine line1 = vtkLine.New();
                    line1.GetPointIds().SetId(0, firstPointId + 0);
                    line1.GetPointIds().SetId(1, firstPointId + 1);
                    vtkLine line2 = vtkLine.New();
                    line2.GetPointIds().SetId(0, firstPointId + 1);
                    line2.GetPointIds().SetId(1, firstPointId + 2);
                    vtkLine line3 = vtkLine.New();
                    line3.GetPointIds().SetId(0, firstPointId + 2);
                    line3.GetPointIds().SetId(1, firstPointId + 3);
                    vtkLine line4 = vtkLine.New();
                    line4.GetPointIds().SetId(0, firstPointId + 3);
                    line4.GetPointIds().SetId(1, firstPointId + 0);
                    //Add these lines to the cell array:
                    lineFrameLines.InsertNextCell(line1);
                    lineFrameLines.InsertNextCell(line2);
                    lineFrameLines.InsertNextCell(line3);
                    lineFrameLines.InsertNextCell(line4);


                    // Add surface cells:
                    // Create a quad on the four points
                    vtkQuad quad = vtkQuad.New();
                    quad.GetPointIds().SetId(0, firstPointId + 0);
                    quad.GetPointIds().SetId(1, firstPointId + 1);
                    quad.GetPointIds().SetId(2, firstPointId + 2);
                    quad.GetPointIds().SetId(3, firstPointId + 3);
                    // Create a cell array to store the quad in
                    surfaceContourCells.InsertNextCell(quad);


                    firstPointId += 4;
                }
            }


            timer.Stop();
            Console.WriteLine("Preparation of points of the structured grid done in " + timer.Time + " s.");
            timer.Start();

            // Create actor for WIRE FRAME:
            vtkPolyData lineFramePolyData = vtkPolyData.New();
            lineFramePolyData.SetLines(lineFrameLines);
            lineFramePolyData.SetPoints(lineFramePoints);
            //lineFramePolyData.GetCellData().SetScalars(colorsLines); // collors assigned to lines, lines will have uniform colors
            lineFramePolyData.GetPointData().SetScalars(colorsPoints);  // colors assigned to points, cmooth color transitions
            //lineFramePolyData.GetPointData().SetScalars(scalarPointValues);  // only scalar values assigned, default coloring will be performed.
            // Prepare mapper and actor:
            vtkPolyDataMapper lineFrameMapper = vtkPolyDataMapper.New();
            lineFrameMapper.SetInput(lineFramePolyData);
            
            vtkActor lineFrameActor = vtkActor.New();
            lineFrameActor.SetMapper(lineFrameMapper);
            vtkProperty lineFrameProperties = lineFrameActor.GetProperty();
            lineFrameProperties.SetColor(1.0, 0.0, 0.0);
            lineFrameProperties.SetLineWidth(2);


            // Create actor for POINTS:
            vtkPolyData pointsPolyData = vtkPolyData.New();
            pointsPolyData.SetPolys(surfaceContourCells);
            pointsPolyData.SetPoints(lineFramePoints);
            //lineFramePolyData.GetCellData().SetScalars(colorsLines); // collors assigned to lines, lines will have uniform colors
            //pointsPolyData.GetPointData().SetScalars(colorsPoints);  // colors assigned to points, cmooth color transitions
            //lineFramePolyData.GetPointData().SetScalars(scalarPointValues);  // only scalar values assigned, default coloring will be performed.
            // Prepare mapper and actor:
            vtkPolyDataMapper pointsFrameMapper = vtkPolyDataMapper.New();
            pointsFrameMapper.SetInput(pointsPolyData);
            
            vtkActor pointsActor = vtkActor.New();
            pointsActor.SetMapper(pointsFrameMapper);
            vtkProperty pointsProperties = pointsActor.GetProperty();
            pointsProperties.SetColor(1.0, 1.0, 0.0);
            pointsProperties.SetPointSize(4.0f);
            // pointsProperties.SetLineWidth(2);
            pointsProperties.SetRepresentationToPoints();
            


            // Prepare geometry and topology for CONTOURS - create PolyData from points and cells:
            vtkPolyData surfaceContourPolyData = vtkPolyData.New();
            surfaceContourPolyData.SetPolys(surfaceContourCells);
            surfaceContourPolyData.SetPoints(lineFramePoints);


            // Prepare mapper and actor for SOLID SURFACE (used just for control):
            // Copy the created PolyData (containing geometry and topology) for use with surface actor:
            vtkPolyData surfacePolydata = vtkPolyData.New();
            surfacePolydata.SetPolys(surfaceContourCells);
            surfacePolydata.SetPoints(lineFramePoints);

            //surfacePolydata.DeepCopy(surfaceContourPolyData);
            // Assign color values either to surface cells (then cells will have uniform color)
            // or to vertice points (then color within cells will smoothly change).
            // Point values override cell values. Color values are 3-tupples of type double where components must be integers
            // in the range 0 to 255.
            surfacePolydata.GetPointData().SetScalars(colorsSurfacePoints);  // colors assigned to points, cmooth color transitions
            //surfacePolydata.GetCellData().SetScalars(colorsSurfaces); // collors assigned to cells, surfaces will have uniform colors
            // Prepare mapper and actor:
            vtkPolyDataMapper surfaceMapper = vtkPolyDataMapper.New();
            surfaceMapper.SetInput(surfacePolydata);
            vtkActor surfaceActor = vtkActor.New();
            surfaceActor.SetMapper(surfaceMapper);
            vtkProperty surfaceProperties = surfaceActor.GetProperty();
            surfaceProperties.SetColor(0.0, 0.0, 1.0);



            // Prepare data for contours:
            // Assign point values (1 value per each point):
            surfaceContourPolyData.GetPointData().SetScalars(scalarPointValues);
            Console.WriteLine();
            // Check range:
            double[] valuesRange;
            valuesRange = surfaceContourPolyData.GetPointData().GetScalars().GetRange();
            Console.WriteLine("RANGE of z values: from " + minZ + " to " + maxZ + ".");
            Console.WriteLine("Range obtaines form vtkPolyData: ");
            if (valuesRange == null)
                Console.WriteLine("null");
            else
            {
                Console.WriteLine("  length: " + valuesRange.Length);
                Console.Write("  Components: ");
                for (int i = 0; i < valuesRange.Length; ++i)
                    Console.Write(valuesRange[i] + " ");
                Console.WriteLine();
            }

            // Create banded contour filter and generate contour values:
            vtkBandedPolyDataContourFilter contourFilter = vtkBandedPolyDataContourFilter.New();
            contourFilter.SetInputConnection(surfaceContourPolyData.GetProducerPort());

            // Generate numContours equally spaced contours (individual contours may be added by SetValue()):
            contourFilter.GenerateValues(numContours, valuesRange[0], valuesRange[1]);

            //// Manually set contour values:
            //contourFilter.SetValue(0, 0.1);
            //contourFilter.SetValue(1, 0.2);
            //contourFilter.SetValue(2, 0.3);
            //contourFilter.SetValue(3, 0.4);
            //contourFilter.SetValue(4, 0.5);
            //contourFilter.SetValue(5, 0.6);
            //contourFilter.SetValue(6, 0.7);
            //contourFilter.SetValue(7, 0.8);
            //contourFilter.SetValue(8, 0.85);
            //contourFilter.SetValue(9, 0.87);
            //contourFilter.SetValue(10, 0.89);
            //contourFilter.SetValue(11, 0.92);


            contourFilter.GenerateContourEdgesOn();  // Must be enabled if we want to make line contours (not only filled)
            contourFilter.Update();

            // Color the contours
            // This colors contours obtained on output port 1 by just setting point values that are 
            // obtained from the output port 0 (here points are obtained through inputconnection from 
            // vtkPolyData object surfaceContourPolyData, which is cource of geometry and topology)
            contourFilter.GetOutput(1).GetPointData().SetScalars(contourFilter.GetOutput().GetPointData().GetScalars());

            //contourFilter.GetOutput(1).GetPointData().SetScalars(colorsPoints);

            // Make sure the mapper uses the new colors
            //bf.GetOutput(0).GetPointData().SetActiveScalars(scalarPointValuesString);

            // TODO: Figure out how to apply custom colors to contours! Now the contour filter uses
            // point data (it could also use cell data) for contours and colors them according to
            // scalar values assigned to points, but we do not specify which colors it uses for which 
            // contours (or point values, respectively)


            // Prepare mapper and actor for solid contours:
            vtkPolyDataMapper surfaceContourMapper = vtkPolyDataMapper.New();
            surfaceContourMapper.SetInputConnection(contourFilter.GetOutputPort(0));  // SWITCH BETWEEN FILLED AND LINE CONTOURS
            surfaceContourMapper.SetScalarRange(valuesRange[0], valuesRange[1]);

            // TODO: Figure out how different settings below influens coloring of filled contours!


            surfaceContourMapper.SetScalarModeToUsePointData();

            //surfaceContourMapper.SetScalarModeToUsePointFieldData();

            //surfaceContourMapper.SetScalarModeToUseFieldData();

            //surfaceContourMapper.SetScalarModeToUseCellData();

            //surfaceContourMapper.SetScalarModeToUseCellFieldData();

            vtkActor surfaceContourActor = vtkActor.New();
            surfaceContourActor.SetMapper(surfaceContourMapper);
            vtkProperty surfaceContourProperties = surfaceContourActor.GetProperty();
            surfaceContourProperties.SetColor(0, 0, 1);
            surfaceContourProperties.SetLineWidth(3);


            // Prepare mapper and actor for contour lines:
            vtkPolyDataMapper contourLinesMapper = vtkPolyDataMapper.New();
            contourLinesMapper.SetInputConnection(contourFilter.GetOutputPort(1));  // SWITCH BETWEEN FILLED AND LINE CONTOURS
            vtkActor contourLinesActor = vtkActor.New();
            contourLinesActor.SetMapper(contourLinesMapper);
            vtkProperty contourLinesProperties = contourLinesActor.GetProperty();
            contourLinesProperties.SetColor(0.0, 0.0, 1.0);
            surfaceContourProperties.SetColor(0, 1, 1);
            surfaceContourProperties.SetLineWidth(3);



            timer.Stop();
            Console.WriteLine("Preparation of actors done in " + timer.TotalTime + " s, ");
            Console.WriteLine("  without preparation of poinst: " + timer.Time + " s.");
            timer.Start();

            // Visualize the constructed graphic objects:

            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);
            renderer.SetBackground(0.05, 0.05, 0.15);



            //vtkSampleFunction sample = vtkSampleFunction.New();
            //sample.GetOutput();

            // Add actors to the renderer:

            //renderer.AddActor(surfaceActor);

            renderer.AddActor(lineFrameActor);

            renderer.AddActor(pointsActor);

            //renderer.AddActor(surfaceContourActor);

            renderer.AddActor(contourLinesActor);



            //Use AxesHandler class to add axes to the graph:
            VtkDecorationHandler axisHandler = new VtkDecorationHandler(renderWindow, renderer);
            axisHandler.CubeAxesXLabel = "MODIFIED X LABEL.";
            axisHandler.CubeAxesColor = new color(System.Drawing.Color.Orange);
            axisHandler.CubeAxesFlyMode = VtkFlyMode.OuterEdges;
            //axisHandler.ShowAxes = false;


            renderWindow.Render();
            renderWindow.SetSize(1000, 700);

            timer.Stop();
            Console.WriteLine("Preparation for rendering done in " + timer.Time + "s.");
            Console.WriteLine("Total preparation time: " + timer.TotalTime + " s.");
            Console.WriteLine();
            Console.WriteLine();

            renderWindowInteractor.Start();


        }  // ExampleCellsGridContours





        /// <summary>Example of efficient plotting surfaces in 3D (graphs of functions of 2 variables 
        /// or parametric surfaces) by using graphic primitives (cells).
        /// Default division numbers of the shown graph are taken.</summary>
        /// <param name="sizex">Number of points in x direction.</param>
        /// <param name="sizey">Number of points in y direction.</param>
        /// <remarks><para>See also:</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/Quad - quadric cells</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/LongLine</para>
        /// </remarks>
        public static void ExampleCellsGridEfficient()
        { ExampleCellsGridEfficient(40, 40); }

        /// <summary>Example of efficient plotting surfaces in 3D (graphs of functions of 2 variables 
        /// or parametric surfaces) by using graphic primitives (cells).</summary>
        /// <param name="sizex">Number of points in x direction.</param>
        /// <param name="sizey">Number of points in y direction.</param>
        /// <remarks><para>See also:</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/ColoredLines</para>
        /// <para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/LongLine</para>
        /// </remarks>
        private static void ExampleCellsGridEfficient(int sizex, int sizey)
        {


            numi = 20;
            numj = 20;
            numk = 1;

            if (sizex > 1)
                numi = sizex;
            if (sizey > 1)
                numj = sizey;

            minX = -1;
            maxX = 1;
            minY = -1;
            maxY = 1;

            Console.WriteLine();
            Console.WriteLine("Example: building surface graph by PolyData containing all cells, dimensions: " + numi + "x"
                + numj + "x" + numk + ".");
            Console.WriteLine("Perparing the graph actors...");
            Console.WriteLine("Preparing points...");
            timer.Reset();
            timer.Start();


            //bool drawLineframe = true;
            //bool drawQuads = true;
            //bool useColors = true;


            //Determine the bounding boc of calculated points that will be used as 
            //Cell vertices:
            BoundingBox valueBounds = new BoundingBox(3);
            for (int j = 0; j < numj-1; j++)
            {
                for (int i = 0; i < numi-1; i++)
                {
                    double
                        x1 = GetX(i),
                        y1 = GetY(j),
                        z1 = f(x1, y1),
                        x2 = GetX(i + 1),
                        y2 = GetY(j),
                        z2 = f(x2, y2),
                        x3 = GetX(i + 1),
                        y3 = GetY(j + 1),
                        z3 = f(x3, y3),
                        x4 = GetX(i),
                        y4 = GetY(j + 1),
                        z4 = f(x4, y4);
                    valueBounds.Update(x1, y1, z1);
                    valueBounds.Update(x2, y2, z2);
                    valueBounds.Update(x3, y3, z3);
                }
            }
            double minZ = valueBounds.GetMin(2);
            double maxZ = valueBounds.GetMax(2);

            Console.WriteLine("Calculation of bounding box for calculated points finished in " + timer.Time + " s.");
            Console.WriteLine("    Bounds on z: " + minZ + ", " + maxZ + ".");

            // Data for wireframe:
            vtkPoints lineFramePoints = vtkPoints.New();
            vtkCellArray lineFrameLines = vtkCellArray.New();
            vtkUnsignedCharArray colorsPoints = vtkUnsignedCharArray.New();
            colorsPoints.SetNumberOfComponents(3);
            colorsPoints.SetName("Point colors.");
            vtkUnsignedCharArray colorsLines = vtkUnsignedCharArray.New();
            colorsLines.SetNumberOfComponents(3);
            colorsLines.SetName("Line colors.");

            // Data for surfaces:
            vtkPoints surfacePoints = vtkPoints.New();
            vtkCellArray surfaceCells = vtkCellArray.New();
            vtkUnsignedCharArray colorsSurfacePoints = vtkUnsignedCharArray.New();
            colorsSurfacePoints.SetNumberOfComponents(3);
            colorsSurfacePoints.SetName("Surface point colors.");
            vtkUnsignedCharArray colorsSurfaces = vtkUnsignedCharArray.New();
            colorsSurfaces.SetNumberOfComponents(3);
            colorsSurfaces.SetName("Surface colors.");

            // Color scale for wireframe
            System.Drawing.Color lowColor = System.Drawing.Color.Brown;
            System.Drawing.Color highColor = System.Drawing.Color.White;
            IColorScale pointColorScale = ColorScale.Create(minZ, maxZ, Color.Brown, Color.White);

            // Color scale for surfaces:
            IColorScale surfacePointScale = ColorScale.Create(minZ, maxZ, Color.Yellow, Color.DarkRed, Color.LawnGreen, Color.DarkBlue);


            int firstPointId = 0;
            for (int j = 0; j < numj-1; j++)
            {
                for (int i = 0; i < numi-1; i++)
                {
                    double
                        x1 = GetX(i),
                        y1 = GetY(j),
                        z1 = f(x1, y1),
                        x2 = GetX(i + 1),
                        y2 = GetY(j),
                        z2 = f(x2, y2),
                        x3 = GetX(i + 1),
                        y3 = GetY(j + 1),
                        z3 = f(x3, y3),
                        x4 = GetX(i),
                        y4 = GetY(j + 1),
                        z4 = f(x4, y4);


                    // Colors for wireframe:
                    color pointColor1 = pointColorScale.GetColor(z1);
                    color pointColor2 = pointColorScale.GetColor(z2);
                    color pointColor3 = pointColorScale.GetColor(z3);
                    color pointColor4 = pointColorScale.GetColor(z4);

                    colorsPoints.InsertNextTuple3(pointColor1.IntR, pointColor1.IntG, pointColor1.IntB);
                    colorsPoints.InsertNextTuple3(pointColor2.IntR, pointColor2.IntG, pointColor2.IntB);
                    colorsPoints.InsertNextTuple3(pointColor3.IntR, pointColor3.IntG, pointColor3.IntB);
                    colorsPoints.InsertNextTuple3(pointColor4.IntR, pointColor4.IntG, pointColor4.IntB);

                    color lineColor1 = color.Average(pointColor1, pointColor2);
                    color lineColor2 = color.Average(pointColor2, pointColor3);
                    color lineColor3 = color.Average(pointColor3, pointColor4);
                    color lineColor4 = color.Average(pointColor4, pointColor1);
                    colorsLines.InsertNextTuple3(lineColor1.IntR, lineColor1.IntG, lineColor1.IntB);
                    colorsLines.InsertNextTuple3(lineColor2.IntR, lineColor2.IntG, lineColor2.IntB);
                    colorsLines.InsertNextTuple3(lineColor3.IntR, lineColor3.IntG, lineColor3.IntB);
                    colorsLines.InsertNextTuple3(lineColor4.IntR, lineColor4.IntG, lineColor4.IntB);

                    // Colors for surfaces:
                    color surfacePointColor1 = surfacePointScale.GetColor(z1);
                    color surfacePointColor2 = surfacePointScale.GetColor(z2);
                    color surfacePointColor3 = surfacePointScale.GetColor(z3);
                    color surfacePointColor4 = surfacePointScale.GetColor(z4);
                    colorsSurfacePoints.InsertNextTuple3(surfacePointColor1.IntR, surfacePointColor1.IntG, surfacePointColor1.IntB);
                    colorsSurfacePoints.InsertNextTuple3(surfacePointColor2.IntR, surfacePointColor2.IntG, surfacePointColor2.IntB);
                    colorsSurfacePoints.InsertNextTuple3(surfacePointColor3.IntR, surfacePointColor3.IntG, surfacePointColor3.IntB);
                    colorsSurfacePoints.InsertNextTuple3(surfacePointColor4.IntR, surfacePointColor4.IntG, surfacePointColor4.IntB);
                    color surfaceColor = color.Average(surfacePointColor1, surfacePointColor2, surfacePointColor3, surfacePointColor4);
                    colorsSurfaces.InsertNextTuple3(surfaceColor.IntR, surfaceColor.IntG, surfaceColor.IntB);



                    //Add lines for the wireframe:
                    //Add 4 corner points of the cell to the list of points:
                    lineFramePoints.InsertNextPoint(x1, y1, z1);
                    lineFramePoints.InsertNextPoint(x2, y2, z2);
                    lineFramePoints.InsertNextPoint(x3, y3, z3);
                    lineFramePoints.InsertNextPoint(x4, y4, z4);
                    //Create 4 lines to outline the current cell:
                    vtkLine line1 = vtkLine.New();
                    line1.GetPointIds().SetId(0, firstPointId + 0);
                    line1.GetPointIds().SetId(1, firstPointId + 1);
                    vtkLine line2 = vtkLine.New();
                    line2.GetPointIds().SetId(0, firstPointId + 1);
                    line2.GetPointIds().SetId(1, firstPointId + 2);
                    vtkLine line3 = vtkLine.New();
                    line3.GetPointIds().SetId(0, firstPointId + 2);
                    line3.GetPointIds().SetId(1, firstPointId + 3);
                    vtkLine line4 = vtkLine.New();
                    line4.GetPointIds().SetId(0, firstPointId + 3);
                    line4.GetPointIds().SetId(1, firstPointId + 0);
                    //Add these lines to the cell array:
                    lineFrameLines.InsertNextCell(line1);
                    lineFrameLines.InsertNextCell(line2);
                    lineFrameLines.InsertNextCell(line3);
                    lineFrameLines.InsertNextCell(line4);


                    // Add surface cells:
                    // Create a quad on the four points
                    vtkQuad quad = vtkQuad.New();
                    quad.GetPointIds().SetId(0, firstPointId + 0);
                    quad.GetPointIds().SetId(1, firstPointId + 1);
                    quad.GetPointIds().SetId(2, firstPointId + 2);
                    quad.GetPointIds().SetId(3, firstPointId + 3);
                    // Create a cell array to store the quad in
                    surfaceCells.InsertNextCell(quad);


                    firstPointId += 4;
                }
            }


            timer.Stop();
            Console.WriteLine("Preparation of points of the structured grid done in " + timer.Time + " s.");
            timer.Start();

            // Create actors for wire frame and grid of surface cells:
            // Prepare geometry and topology - dataset of type vtkPolyData:
            vtkPolyData lineFramePolyData = vtkPolyData.New();
            lineFramePolyData.SetLines(lineFrameLines);
            lineFramePolyData.SetPoints(lineFramePoints);
            // Assign color values either to surface cells (then cells will have uniform color)
            // or to vertice points (then color within cells will smoothly change).
            // Point values override cell values. Color values are 3-tupples of type double where components must be integers
            // in the range 0 to 255.
            //lineFramePolyData.GetCellData().SetScalars(colorsLines); // collors assigned to lines, lines will have uniform colors
            lineFramePolyData.GetPointData().SetScalars(colorsPoints);  // colors assigned to points, cmooth color transitions
            // Prepare mapper and actor:
            vtkPolyDataMapper lineFrameMapper = vtkPolyDataMapper.New();
            lineFrameMapper.SetInput(lineFramePolyData);
            vtkActor lineFrameActor = vtkActor.New();
            lineFrameActor.SetMapper(lineFrameMapper);
            vtkProperty lineFrameProperties = lineFrameActor.GetProperty();
            lineFrameProperties.SetColor(1.0, 0.0, 0.0);
            lineFrameProperties.SetLineWidth(2);


            // Create actor for surface cells:
            vtkPolyData surfacePolyData = vtkPolyData.New();
            surfacePolyData.SetPolys(surfaceCells);
            surfacePolyData.SetPoints(lineFramePoints);
            //surfacePolyData.GetCellData().SetScalars(colorsSurfaces); // collors assigned to cells, surfaces will have uniform colors
            surfacePolyData.GetPointData().SetScalars(colorsSurfacePoints);  // colors assigned to points, cmooth color transitions
            // Prepare mapper and actor:
            vtkPolyDataMapper surfaceMapper = vtkPolyDataMapper.New();
            surfaceMapper.SetInput(surfacePolyData);
            vtkActor surfaceActor = vtkActor.New();
            surfaceActor.SetMapper(surfaceMapper);
            vtkProperty surfaceProperties = surfaceActor.GetProperty();
            surfaceProperties.SetColor(0.0, 0.0, 1.0);
            

            timer.Stop();
            Console.WriteLine("Preparation of actors done in " + timer.TotalTime + " s, ");
            Console.WriteLine("  without preparation of poinst: " + timer.Time + " s.");
            timer.Start();

            // Visualize the constructed graphic objects:

            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);


            // Add actors to the renderer:

            //renderer.AddActor(lineFrameActor);

            renderer.AddActor(surfaceActor);


            //Use AxesHandler class to add axes to the graph:
            VtkDecorationHandler axisHandler = new VtkDecorationHandler(renderWindow, renderer);
            axisHandler.CubeAxesXLabel = "MODIFIED X LABEL.";
            axisHandler.CubeAxesColor = new color(System.Drawing.Color.Orange);
            axisHandler.CubeAxesFlyMode = VtkFlyMode.OuterEdges;
            //axisHandler.ShowAxes = false;


            renderWindow.Render();
            renderWindow.SetSize(1000, 700);

            timer.Stop();
            Console.WriteLine("Preparation for rendering done in " + timer.Time + "s.");
            Console.WriteLine("Total preparation time: " + timer.TotalTime + " s.");
            Console.WriteLine();
            Console.WriteLine();

            renderWindowInteractor.Start();

        }  // ExampleCellsGridEfficient




        /// <summary>Example that demonstrates the ability to use simple primitives for plotting surfaces in 3D
        /// (graphs of functions of 2 variables or parametric surfaces).</summary>
        /// <remarks><para>See also:</para><para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/Quad</para></remarks>
        public static void ExampleQuadCells()
        {
            ExampleQuadCells(8, 8);
        }

        /// <summary>Example that demonstrates the ability to use simple primitives for plotting surfaces in 3D
        /// (graphs of functions of 2 variables or parametric surfaces).
        /// </summary>
        /// <param name="sizex">Number of points in x direction.</param>
        /// <param name="sizey">Number of points in y direction.</param>
        /// <remarks><para>See also:</para><para>http://www.vtk.org/Wiki/VTK/Examples/Cxx/GeometricObjects/Quad</para></remarks>
        public static void ExampleQuadCells(int sizex, int sizey)
        {

            numi = 20;
            numj = 20;
            numk = 1;

            if (sizex > 1)
                numi = sizex;
            if (sizey > 1)
                numj = sizey;

            minX = -1;
            maxX = 1;
            minY = -1;
            maxY = 1;


            Console.WriteLine();
            Console.WriteLine("Example: building surface graph by quad cells, dimensions: " + sizex + "x" 
                + sizey + "x" + 1 + ".");
            Console.WriteLine("Perparing the graph actors...");
            Console.WriteLine("Preparing points...");
            timer.Reset();
            timer.Start();


            // auxiliary objects for rendering the graphics:
            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);

            

            // auxiliary objects:
            vtkDataSetMapper datasetMapper;
            vtkPolyDataMapper mapper;
            vtkProperty properties;

            for (int j = 0; j < numj; j++)
            {
                for (int i = 0; i < numi; i++)
                {
                    double 
                        x1 = GetX(i),
                        y1 = GetY(j),
                        z1 = f(x1,y1),
                        x2 = GetX(i+1),
                        y2 = GetY(j),
                        z2 = f(x2,y2),
                        x3 = GetX(i+1),
                        y3 = GetY(j + 1),
                        z3 = f(x3,y3),
                        x4 = GetX(i),
                        y4 = GetY(j + 1),
                        z4 = f(x4,y4);


                    // create structured grid with 4 points
                    vtkStructuredGrid structuredGrid = vtkStructuredGrid.New();
                    vtkPoints structuredPoints = vtkPoints.New();

                    structuredPoints.InsertNextPoint(x1, y1, z1);
                    structuredPoints.InsertNextPoint(x2, y2, z2);
                    structuredPoints.InsertNextPoint(x4, y4, z4);
                    structuredPoints.InsertNextPoint(x3, y3, z3);

                    // Specify the dimensions of the grid
                    structuredGrid.SetDimensions(2, 2, 1);
                    structuredGrid.SetPoints(structuredPoints);


                    // Actor that represents surfaces (structured grid is used):
                    vtkShrinkFilter shrinkFilter = vtkShrinkFilter.New();
                    shrinkFilter.SetInputConnection(structuredGrid.GetProducerPort());
                    shrinkFilter.SetShrinkFactor(1);
                    datasetMapper = vtkDataSetMapper.New();
                    datasetMapper.SetInputConnection(shrinkFilter.GetOutputPort());
                    vtkActor actorSurfaceStructuredGrid = vtkActor.New();
                    actorSurfaceStructuredGrid.SetMapper(datasetMapper);
                    properties = actorSurfaceStructuredGrid.GetProperty();
                    properties.SetColor(0, 1, 1);
                    properties.SetRepresentationToSurface();


                    // Actor that represents lines (structured grid is used):
                    datasetMapper = vtkDataSetMapper.New();
                    datasetMapper.SetInputConnection(structuredGrid.GetProducerPort());
                    vtkActor actorLinesStructuredGrid = vtkActor.New();
                    actorLinesStructuredGrid.SetMapper(datasetMapper);
                    properties = actorLinesStructuredGrid.GetProperty();
                    properties.SetColor(1, 0, 0);
                    properties.SetLineWidth(2);
                    properties.SetRepresentationToWireframe();


                    // Actors that use vtkQuad:

                    // Add the points to a vtkPoints object
                    vtkPoints points = vtkPoints.New();
                    points.InsertNextPoint(x1, y1, z1);
                    points.InsertNextPoint(x2, y2, z2);
                    points.InsertNextPoint(x3, y3, z3);
                    points.InsertNextPoint(x4, y4, z4);

                    // Create a quad on the four points
                    vtkQuad quad = vtkQuad.New();
                    quad.GetPointIds().SetId(0,0);
                    quad.GetPointIds().SetId(1,1);
                    quad.GetPointIds().SetId(2,2);
                    quad.GetPointIds().SetId(3,3);
 
                    // Create a cell array to store the quad in
                    vtkCellArray quads = vtkCellArray.New();
                    quads.InsertNextCell(quad);
 
                    // Create a polydata to store everything in
                    vtkPolyData polydata = vtkPolyData.New();
 
                    // Add the points and quads to the dataset
                    polydata.SetPoints(points);
                    polydata.SetPolys(quads);
 
                    // Setup actor and mapper for wireframe cell:
                    vtkPolyDataMapper mapperPolyData = vtkPolyDataMapper.New();
                    mapperPolyData.SetInput(polydata);
                    vtkActor actorLinesQuad = vtkActor.New();
                    actorLinesQuad.SetMapper(mapperPolyData);
                    properties = actorLinesQuad.GetProperty();
                    properties.SetColor(1, 0, 1);
                    properties.SetRepresentationToWireframe();
  
                    // Setup actor and mapper for surface cell:
                    vtkPolyDataMapper mapperPolyData1 = vtkPolyDataMapper.New();
                    mapperPolyData1.SetInput(polydata);
                    vtkActor actorSurfacesQuad = vtkActor.New();
                    actorSurfacesQuad.SetMapper(mapperPolyData1);
                    properties = actorSurfacesQuad.GetProperty();
                    properties.SetColor(0, 1, 0.3);
                    properties.SetRepresentationToSurface();



                    //renderer.AddActor(actorSurfaceStructuredGrid);

                    //renderer.AddActor(actorLinesStructuredGrid);

                    renderer.AddActor(actorLinesQuad);

                    //renderer.AddActor(actorSurfacesQuad);

                }
                
            }


            timer.Stop();
            Console.WriteLine("Preparation of graphic objectsdone in " + timer.Time + " s.");
            timer.Start();

            renderer.SetBackground(0.4, 0.4, 0.4);

            IBoundingBox bounds;
            // bounds = new BoundingBox(3);
            bounds = null;
            //bounds = UtilVtk.CreateBounds(renderer);
            UtilVtk.UpdateBounds(ref bounds, renderer);

            //Use AxesHandler class to add axes to the graph:
            VtkDecorationHandler axisHandler = new VtkDecorationHandler(renderWindow, renderer);
            axisHandler.CubeAxesXLabel = "MODIFIED X LABEL.";
            axisHandler.CubeAxesColor = new color(System.Drawing.Color.Orange);
            axisHandler.CubeAxesFlyMode = VtkFlyMode.OuterEdges;
            axisHandler.SetActorScale(1, 1, 0.1);
            //axisHandler.ShowAxes = false;
            renderer.ResetCamera();

            timer.Stop();
            Console.WriteLine("Preparation of actors done in " + timer.TotalTime + " s, ");
            Console.WriteLine("  without preparation of geometry: " + timer.Time + " s.");
            timer.Start();


            
            renderWindow.Render();
            renderWindow.SetSize(1000, 700);
            
            renderWindowInteractor.Start();

        }  // class ExampleQuadCells



        
        /// <summary>Example that demonstrates the ability to use structured grids for plotting surfaces in 3D
        /// (graphs of functions of 2 variables or parametric surfaces).</summary>
        /// <remarks><para>See also:</para><para>http://www.vtk.org/Wiki/VTK/Examples/Cxx#vtkStructuredGrid</para></remarks>
        public static void ExampleStructuredGrid()
        {
            ExampleStructuredGrid(6, 6, 2);
        }

        public static void ExampleAxisHendler()
        {
            vtkStructuredGrid structuredGrid = vtkStructuredGrid.New();
            vtkPoints structuredPoints = vtkPoints.New();
            vtkPolyDataMapper mapperPoints = vtkPolyDataMapper.New();
            vtkProperty properties;
            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renderWindow = vtkRenderWindow.New();

            renderWindow.AddRenderer(renderer);
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);
            renderer.SetBackground(0.3, 0.5, 0.3);

            for (int k = 0; k < 10; k++)
            {
                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        double x, y, z;
                        x = GetX(i);
                        y = GetY(j);
                        z = f(x, y) + ((double)k / (double)numk);
                        structuredPoints.InsertNextPoint(x, y, z);
                    }
                }
            }

            // Specify the dimensions of the grid
            structuredGrid.SetDimensions(10, 10, 10);
            structuredGrid.SetPoints(structuredPoints);

            // Actor that represents points of the structured grid. Does not work if the 3rd dimension is 1.
            vtkStructuredGridGeometryFilter geometryFilterPoints = vtkStructuredGridGeometryFilter.New();
            geometryFilterPoints.SetInputConnection(structuredGrid.GetProducerPort());
            geometryFilterPoints.Update();

            mapperPoints.SetInputConnection(geometryFilterPoints.GetOutputPort());
            vtkActor actorPoints = vtkActor.New();
            actorPoints.SetMapper(mapperPoints);
            
            double[] boundActorPoints1 = actorPoints.GetBounds();
            actorPoints.SetScale(1, 1, 0.1);
            double[] boundActorPoints2 = actorPoints.GetBounds();
            
            properties = actorPoints.GetProperty();
            properties.SetColor(0.0, 1.0, 0.0);
            properties.SetRepresentationToPoints();
            properties.SetPointSize(6);
            

            vtkCubeAxesActor cubeAxesActor = vtkCubeAxesActor.New();


            IG.Gr3d.UtilVtk.SetBounds(cubeAxesActor, UtilVtk.CreateBounds(actorPoints));
            cubeAxesActor.SetMapper(mapperPoints);
            cubeAxesActor.SetCamera(renderer.GetActiveCamera());
            properties = cubeAxesActor.GetProperty();
            cubeAxesActor.SetFlyMode(4);

            double[] boundAxisActor1 = cubeAxesActor.GetBounds();
            cubeAxesActor.SetScale(10.0, 1.0, 1.0);
            //cubeAxesActor.SetOrigin(10, 1, 1);
            double[] boundAxisActor2 = cubeAxesActor.GetBounds();
            

            properties.SetColor(1, 1, 0);
            renderer.AddActor(cubeAxesActor);
            renderer.AddActor(actorPoints);

            renderWindow.SetSize(1000, 700);
            renderWindow.SetPosition(200, 100);

            renderWindow.Render();

            try
            {

                renderWindowInteractor.Initialize();
                renderWindowInteractor.Start();

            }
            catch
            {

            }
            finally
            {

                // renderWindowInteractor.TerminateApp();
                renderWindowInteractor.Dispose();
                renderWindow.FinalizeWrapper();
                renderWindow.Dispose();

                bool renderAgain = true;
                if (renderAgain)
                {
                    // Demonstration of how to render contents once more in another window:
                    vtkRenderWindow win1 = vtkRenderWindow.New();
                    win1.AddRenderer(renderer);
                    vtkRenderWindowInteractor interactor1 = vtkRenderWindowInteractor.New();
                    interactor1.SetRenderWindow(win1);  //renderWindow);
                    interactor1.Initialize();
                    interactor1.Start();

                    win1.FinalizeWrapper();
                }

            }
        }


        /// <summary>Example that demonstrates the ability to use structured grids for plotting surfaces in 3D
        /// (graphs of functions of 2 variables or parametric surfaces).</summary>
        /// <param name="sizex">Number of points in x direction.</param>
        /// <param name="sizey">Number of points in y direction.</param>
        /// <param name="sizez">Number of points in z direction.</param>
        /// <remarks><para>See also:</para><para>http://www.vtk.org/Wiki/VTK/Examples/Cxx#vtkStructuredGrid</para></remarks>
        public static void ExampleStructuredGrid(int sizex, int sizey, int sizez)
        {
            

            numi = 10;
            numj = 10;
            numk = 2;

            if (sizex > 1)
                numi = sizex;
            if (sizey > 1)
                numj = sizey;
            if (sizez > 0)
                numk = sizez;


            Console.WriteLine();
            Console.WriteLine("Example: building surface graph by a structured grid, dimensions: " + sizex + "x" 
                + sizey + "x" + sizez + ".");
            Console.WriteLine("Perparing the graph actors...");
            Console.WriteLine("Preparing points...");
            timer.Reset();
            timer.Start();


            // create grid
            vtkStructuredGrid structuredGrid = vtkStructuredGrid.New();
            vtkContourGrid contourGrid = vtkContourGrid.New();
            vtkPoints structuredPoints = vtkPoints.New();
            // Point Colors:
            vtkUnsignedCharArray colorsPoints = vtkUnsignedCharArray.New();
            colorsPoints.SetNumberOfComponents(3);
            colorsPoints.SetName("Point colors.");

            BoundingBox valueBounds = new BoundingBox(3);
            // Get bounding box first:

            for (int k = 0; k < numk; k++)
            {
                for (int j = 0; j < numj; j++)
                {
                    for (int i = 0; i < numi; i++)
                    {
                        double x, y, z;
                        x = GetX(i);
                        y = GetY(j);
                        z = f(x, y) + ((double)k / (double)numk);
                        valueBounds.Update(x, y, z);
                    }
                }
            }
            minZ = valueBounds.GetMin(2);
            maxZ = valueBounds.GetMax(2);


            // Color scale for points of the grid:
            IColorScale pointColorScale;
            //pointColorScale = ColorScale.Create(minZ, maxZ, Color.DarkBlue, Color.Violet, Color.DarkGreen, Color.Magenta, Color.Red, Color.Yellow);
            //pointColorScale = ColorScale.Create(minZ, maxZ, Color.DarkBlue, Color.Green, Color.Red, Color.Yellow);
            pointColorScale = ColorScale.Create(minZ, maxZ, Color.Blue, Color.Red, Color.Yellow);

            // Color scale for surfaces:

            for (int k = 0; k < numk; k++)
            {
                for (int j = 0; j < numj; j++)
                {
                    for (int i = 0; i < numi; i++)
                    {
                        double x, y, z;
                        x = GetX(i);
                        y = GetY(j);
                        z = f(x, y) + ((double)k/(double)numk);
                        structuredPoints.InsertNextPoint(x, y, z);

                        //color pointColor = color.ScaleBlueRedYellow(minZ, maxZ, z);
                        color pointColor = pointColorScale.GetColor(z);
                        colorsPoints.InsertNextTuple3(pointColor.IntR, pointColor.IntG, pointColor.IntB);
                    }
                }
            }

            timer.Stop();
            Console.WriteLine("Preparation of points of the structured grid done in " + timer.Time + " s.");
            timer.Start();

            // Specify the dimensions of the grid
            structuredGrid.SetDimensions(numi, numj, numk);
            structuredGrid.SetPoints(structuredPoints);



            // auxiliary objects:
            vtkDataSetMapper datasetMapper;
            vtkPolyDataMapper mapper;
            vtkProperty properties;

            // contour
            vtkContourFilter contourFilter = vtkContourFilter.New();
            contourFilter.SetInputConnection(structuredGrid.GetProducerPort());
            contourFilter.Update();

            mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(contourFilter.GetOutputPort());

            vtkActor actorContour = vtkActor.New();
            actorContour.SetMapper(mapper);
            properties = actorContour.GetProperty();
            properties.SetColor(1, 0, 0);
            properties.SetRepresentationToWireframe();


            // Assign colors to points of the grid:
            // Remark: COMMENT THE LINE BELOW IN ORDER TO REMOVE SMOOTH COLOR TRANSITIONS!
            structuredGrid.GetPointData().SetScalars(colorsPoints);

            // Actor that represents the structured grid without filtering:
            datasetMapper = vtkDataSetMapper.New();
            datasetMapper.SetInputConnection(structuredGrid.GetProducerPort());
            vtkActor actorPlain = vtkActor.New();
            actorPlain.SetMapper(datasetMapper);
            properties = actorPlain.GetProperty();
            properties.SetColor(0.0, 0.5, 1);
            properties.SetOpacity(0.6);

            // Actor that represents points of the structured grid. Does not work if the 3rd dimension is 1.
            vtkStructuredGridGeometryFilter geometryFilterPoints = vtkStructuredGridGeometryFilter.New();
            geometryFilterPoints.SetInputConnection(structuredGrid.GetProducerPort());
            geometryFilterPoints.Update();
            
            //mapper = vtkPolyDataMapper.New();
            //mapper.SetInputConnection(geometryFilterPoints.GetOutputPort());
            //vtkActor actorPoints = vtkActor.New();
            //actorPoints.SetMapper(mapper);

            vtkPolyDataMapper mapperPoints = vtkPolyDataMapper.New();
            mapperPoints.SetInputConnection(geometryFilterPoints.GetOutputPort());
            vtkActor actorPoints = vtkActor.New();
            actorPoints.SetMapper(mapperPoints);

            properties = actorPoints.GetProperty();
            properties.SetColor(0.0, 1.0, 0.0);
            properties.SetRepresentationToPoints();
            properties.SetPointSize(6);


            // Actor that represents lines of the structured grid. Does not work if the 3rd dimension is >1.
            vtkStructuredGridGeometryFilter geometryFilterLines = vtkStructuredGridGeometryFilter.New();
            geometryFilterLines.SetInputConnection(structuredGrid.GetProducerPort());
            geometryFilterLines.Update();

            mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(geometryFilterLines.GetOutputPort());
            vtkActor actorLines = vtkActor.New();
            actorLines.SetMapper(mapper);
            properties = actorLines.GetProperty();
            //properties.SetColor(0.0, 1.0, 0.0);
            //properties.SetRepresentationToSurface();
            properties.SetRepresentationToWireframe();
            //properties.EdgeVisibilityOn();
            //properties.SetEdgeVisibility(10);
            //properties.SetEdgeColor(1, 0, 0);
            properties.SetLineWidth(2);

            // Actor that represents outline of the structured grid.
            vtkStructuredGridOutlineFilter geometryFilterOutline = vtkStructuredGridOutlineFilter.New();
            geometryFilterOutline.SetInputConnection(structuredGrid.GetProducerPort());
            geometryFilterOutline.Update();

            mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(geometryFilterOutline.GetOutputPort());
            vtkActor actorOutline = vtkActor.New();
            actorOutline.SetMapper(mapper);
            properties = actorOutline.GetProperty();
            properties.SetColor(1.0, 0.0, 0.0);
            properties.SetLineWidth(2);

            // Actor that represents structured grid cells.
            vtkShrinkFilter shrinkFilter = vtkShrinkFilter.New();
            shrinkFilter.SetInputConnection(structuredGrid.GetProducerPort());
            shrinkFilter.SetShrinkFactor(1);



            datasetMapper = vtkDataSetMapper.New();
            datasetMapper.SetInputConnection(shrinkFilter.GetOutputPort());
            vtkActor actorShrink = vtkActor.New();
            actorShrink.SetMapper(datasetMapper);
            properties = actorShrink.GetProperty();
            properties.SetRepresentationToWireframe();
            properties.SetLineWidth(1.5f);
            properties.SetColor(1, 0, 0);

            timer.Stop();
            Console.WriteLine("Preparation of actors done in " + timer.TotalTime + " s, ");
            Console.WriteLine("  without preparation of poinst: " + timer.Time + " s.");
            timer.Start();

            // Visualiize the grid:
            
            vtkRenderer renderer = vtkRenderer.New();
            vtkRenderWindow renderWindow = vtkRenderWindow.New();
            renderWindow.AddRenderer(renderer);
            vtkRenderWindowInteractor renderWindowInteractor = vtkRenderWindowInteractor.New();
            renderWindowInteractor.SetRenderWindow(renderWindow);

            //renderer.AddActor(actorContour);


            renderer.AddActor(actorPlain);

            renderer.AddActor(actorPoints);

            renderer.AddActor(actorLines);

            renderer.AddActor(actorOutline);

            renderer.AddActor(actorShrink);

            renderer.SetBackground(0.3, 0.5, 0.3);
            
            // Add cube axes to the renderer:
            //vtkCubeAxesActor cubeAxesActor = vtkCubeAxesActor.New();
            //// Get current bounds and set bounds on the axes actor:
            //cubeAxesActor.SetMapper(mapperPoints);
            //IG.Gr3d.UtilVtk.SetBounds(cubeAxesActor, UtilVtk.CreateBounds(actorPoints));

            //actorLines.SetScale(1, 1, 5);
            //actorPlain.SetScale(1, 1, 5);
            actorPoints.SetScale(1, 1, 1);
            //actorOutline.SetScale(1, 1, 5);
            //actorShrink.SetScale(1, 1, 5);
            
            // Add cube axes to the renderer:
            vtkCubeAxesActor cubeAxesActor = vtkCubeAxesActor.New();              
            cubeAxesActor.SetMapper(mapperPoints);
            // Get current bounds and set bounds on the axes actor:
            IG.Gr3d.UtilVtk.SetBounds(cubeAxesActor, UtilVtk.CreateBounds(actorPoints));
            vtkAxisActor2D axesActor2D = vtkAxisActor2D.New();
            vtkAxesActor axesActor = vtkAxesActor.New();
            vtkAxes axes = vtkAxes.New();
            
            
            //cubeAxesActor.SetBounds(
            //    structuredGrid.GetBounds()[0],
            //    structuredGrid.GetBounds()[1],
            //    structuredGrid.GetBounds()[2],
            //    structuredGrid.GetBounds()[3],
            //    structuredGrid.GetBounds()[4],
            //    structuredGrid.GetBounds()[5]
            //    );
            
            cubeAxesActor.SetCamera(renderer.GetActiveCamera());
            properties = cubeAxesActor.GetProperty();
            
            double[] boundxyz = cubeAxesActor.GetBounds();
            double[] ttx = cubeAxesActor.GetXRange();
            double[] tty = cubeAxesActor.GetYRange();
            double[] ttz = cubeAxesActor.GetZRange();
            
            //'fly' mode (STATIC is default): 'STATIC' constructs axes from all edges of the bounding box.  
            //'CLOSEST_TRIAD' consists of the three axes x-y-z forming a triad that lies closest to the 
            //specified camera. 'FURTHEST_TRIAD' consists of the three axes x-y-z forming a triad that lies 
            //furthest from the specified camera. 'OUTER_EDGES' is constructed from edges that are on the 
            //"exterior" of the bounding box, exterior as dete5rmined from examining outer edges of the bounding 
            //box in projection (display) space.
            // 0 - STATIC 
            // 1 - CLOSEST_TRIAD
            // 2 - FURTHEST_TRIAD
            // 3 - OUTER_EDGES 
            // 4 - STATIC
            // TODO: find the curresponding enumerator, if it exists!
            cubeAxesActor.SetFlyMode(4);
            // Put 10^ sign

            cubeAxesActor.SetScale(1, 1, 0.1);
            //properties.SetLineWidth(3.0);
            properties.SetColor(1,1,0);
            renderer.AddActor(cubeAxesActor);


            renderWindow.SetSize(1000, 700);
            renderWindow.SetPosition(200, 100);

            renderWindow.Render();

            timer.Stop();
            Console.WriteLine("Preparation for rendering done in " + timer.Time + "s.");
            Console.WriteLine("Total preparation time: " + timer.TotalTime + " s.");
            Console.WriteLine();
            Console.WriteLine();

            try
            {

                renderWindowInteractor.Initialize();
                renderWindowInteractor.Start();

            }
            catch
            {

            }
            finally
            {

                // renderWindowInteractor.TerminateApp();
                renderWindowInteractor.Dispose();
                renderWindow.FinalizeWrapper();
                renderWindow.Dispose();

                bool renderAgain = true;
                if (renderAgain)
                {
                    // Demonstration of how to render contents once more in another window:
                    vtkRenderWindow win1 = vtkRenderWindow.New();
                    win1.AddRenderer(renderer);
                    vtkRenderWindowInteractor interactor1 = vtkRenderWindowInteractor.New();
                    interactor1.SetRenderWindow(win1);  //renderWindow);
                    interactor1.Initialize();
                    interactor1.Start();

                    win1.FinalizeWrapper();
                }

            }

        }


        // TODO: remove these variables.
        static vtkMath math;
        static vtkPoints points;
        static vtkPolyData profile;
        static vtkDelaunay2D del;
        static vtkPolyDataMapper mapMesh;
        static vtkActor meshActor;
        static vtkExtractEdges extract;
        static vtkTubeFilter tubes;
        static vtkPolyDataMapper mapEdges;
        static vtkActor edgeActor;
        static vtkSphereSource ball;
        static vtkGlyph3D balls;
        static vtkPolyDataMapper mapBalls;
        static vtkActor ballActor;
        static vtkRenderer ren1;
        static vtkRenderWindow renWin;
        static vtkRenderWindowInteractor iren;

        public static ColorScale CreateGreenRedYellow(double minValue, double maxValue)
        {
            return new ColorScale(minValue, maxValue,
                System.Drawing.Color.Green, System.Drawing.Color.Red, System.Drawing.Color.Yellow);
        }


    }


    public class ExampleValueFunctionDiff3D : Func3dBaseNoGradient
    //public class ExampleValueFunctionDiff3D : Func2dBase, IFunc2d
    {
        public ExampleValueFunctionDiff3D()
            : base()
        { }

        public ExampleValueFunctionDiff3D(IFunc2d originalFunction, IFunc2d approximatedFunction)
        {
            this.OriginalFunction = originalFunction;
            this.ApproximatedFunction = approximatedFunction;
        }


        #region Data

        IFunc2d _oapproximatedFunction;

        public IFunc2d ApproximatedFunction
        {
            get { return _oapproximatedFunction; }
            protected set
            {
                //this.ValueDefined = false; this.GradientDefined = false; this.HessianDefined = false;
                //if (value != null)
                //{
                //    //this.ValueDefined = value.ValueDefined;
                //    //this.GradientDefined = value.GradientDefined;
                //    //this.HessianDefined = value.HessianDefined;
                //}
                _oapproximatedFunction = value;
            }
        }

        IFunc2d _originalFunction;

        public IFunc2d OriginalFunction
        {
            get { return _originalFunction; }
            protected set
            {
                //this.ValueDefined = false; this.GradientDefined = false; this.HessianDefined = false;
                //if (value != null)
                //{
                //    //this.ValueDefined = value.ValueDefined;
                //    //this.GradientDefined = value.GradientDefined;
                //    //this.HessianDefined = value.HessianDefined;
                //}
                _originalFunction = value;
            }
        }


        #endregion


        public override double Value(double x, double y, double z)
        {
            double diff = 0;
            vec2 xy = new vec2();
            xy[0] = x;
            xy[1] = y;

            diff = Math.Abs(_originalFunction.Value(xy) - _oapproximatedFunction.Value(xy));

            return diff;
        }

    }
}
