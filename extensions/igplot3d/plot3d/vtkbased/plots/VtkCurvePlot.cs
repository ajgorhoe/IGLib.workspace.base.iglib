using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kitware.VTK;

using IG.Lib;
using IG.Num;

namespace IG.Gr3d
{

    /// <summary>Handles 3D curve plots in VTK render window accessed through the <see cref="VtkPlotter"/> class.
    /// <para>Generated contour plots are based on unstructured 1D meshes embedded in 3D.</para></summary>
    /// $A Igor xx Nov11;
    public class VtkCurvePlot: VtkSurfacePlotBase
    {

        public VtkCurvePlot(VtkPlotter plotter)
            : base(plotter)
        { }

        public VtkCurvePlot(VtkPlotter plotter, UnstructuredMesh1d3d mesh)
            : this(plotter)
        {
            this.Mesh = mesh;
        }


        /// <summary>Default number of surface gridpoints in the first grid direction (for structured grids).</summary>
        public static int DefaultNumX = 400;
        

        private int _numX = DefaultNumX;

        /// <summary>Number of nodes of the plotted curve.
        /// <para>Used for construction of curve when the it is defined by functions.</para></summary>
        /// <remarks>Default value can be changed by setting <see cref="DefaultNumX"/>.</remarks>
        public int NumX
        { get { return _numX; }
            set
            {
                if (value > 1)
                    _numX = value;
                else
                    throw new ArgumentException("Number of curve nodes can not be set to value less than 2.");
            }
        }



        public static BoundingBox1d DefaultBoundingBoxReference = new BoundingBox1d(-1, 1);

        private BoundingBox1d _boundsReference = DefaultBoundingBoxReference;

        /// <summary>Bounds on reference parameters used for generation of the surface grid.</summary>
        public BoundingBox1d BoundsParameters
        {
            get {
                if (_boundsReference == null)
                    _boundsReference = new BoundingBox1d(-1, 1);
                return _boundsReference;
            }
            set { _boundsReference = value; }
        }

        private BoundingBox1d _boundsPointValues;

        /// <summary>Bounds on values assigned to surface nodes.</summary>
        public BoundingBox1d BoundsPointValues
        {
            get {
                if (_boundsPointValues == null)
                    _boundsPointValues = new BoundingBox1d();
                return _boundsPointValues;
            } 
            protected set
            {
                _boundsPointValues = value;
            }
        }

        /// <summary>Sets the bounds on reference coordinates used in generation of the surface grid.</summary>
        /// <param name="minX">Lower bound for the first coordinate.</param>
        /// <param name="maxX">Upper bound for the first coordinate.</param>
        /// <param name="minY">Lower bound for the second coordinate.</param>
        /// <param name="maxY">Upper bound for the second coordinate.</param>
        public void SetBoundsReference(double minX, double maxX)
        {
            BoundsParameters.Reset();
            BoundsParameters.Update(0, minX, maxX);
        }

        // private IFunc2d _surfaceFunction;

        /// <summary>Sets function of 1 variables whose graph is plotted in the XY plane.
        /// <para>The specified function represents explicit definition of the curve in 2 dimensions (positioned in the XY plane).</para></summary>
        public IRealFunction Function
        {
            set {
                FunctionX = Func.GetIdentity();
                FunctionY = value;
                FunctionZ = Func.GetConstant(0.0);

            }
        }

        private IRealFunction _functionX, _functionY, _functionZ;

        /// <summary>The first component of a 3D vector function of 1 parameter that acts as parametric
        /// definition of the plotted curve.</summary>
        public IRealFunction FunctionX
        { get { return _functionX; } 
            protected set { 
                _functionX = value; 
                if (_functionX!=null && _functionY!=null && _functionZ!=null) Mesh = null; 
            } 
        }

        /// <summary>The second component of a 3D vector function of 1 parameters that acts as parametric
        /// definition of the plotted curve.</summary>
        public IRealFunction FunctionY
        { get { return _functionY; } 
            protected set 
            { 
                _functionY = value;
                if (_functionX != null && _functionY != null && _functionZ != null) Mesh = null;  
            } 
        }

        /// <summary>The third component of a 3D vector function of 1 parameters that acts as parametric
        /// definition of the plotted curve.</summary>
        public IRealFunction FunctionZ
        { get { return _functionZ; } 
            protected set  { 
                _functionZ = value;
                if (_functionX != null && _functionY != null && _functionZ != null) Mesh = null;  
            } 
        }

        /// <summary>Sets the definition of parametric curve in 3D.</summary>
        /// <param name="fX">Function that specifies the first component of coordinate of a curve point dependent on 
        /// parameters of the parametrically defined curve.</param>
        /// <param name="fY">Function that specifies the second component of coordinate of a curcve point dependent on 
        /// parameters of the parametrically defined curve.</param>
        /// <param name="fZ">Function that specifies the third component of coordinate of a curve point dependent on 
        /// parameters of the parametrically defined curve.</param>
        public void SetCurveDefinition(IRealFunction fX, IRealFunction fY, IRealFunction fZ)
        { FunctionX = fX; FunctionY = fY; FunctionZ = fZ; }

        ///// <summary>Sets the definition of parametric curve in 3D.
        ///// <para>Components of the specified 3D vector functions of 1 variables define individual coordinates
        ///// on the curve dependent on curve parameter (as functions of one varuable).</para></summary>
        ///// <param name="functions">A 3D vector function of 1 variable. Each component (a 3D function) defines 
        ///// dependence of one cartesian coordinate on the curve parameter.</param>
        //public void SetCurveDefinition(IFunc3d1d functions)
        //{
        //    FunctionX = functions.Component1;
        //    FunctionY = functions.Component2;
        //    FunctionZ = functions.Component3;
        //}


        /// <summary>Sets explicit definition of a curve as graph of a function of one variable in the XY plane.</summary>
        /// <param name="funcY">Function that deines the dependence of Y coordinate of a point on the curve on X coordinate.</param>
        public void SetCurveDefinition(IRealFunction funcY)
        {
            FunctionX = Func.GetIdentity();
            FunctionY = funcY;
            FunctionZ = Func.GetConstant(0.0);
        }

        //private IFunc2d _funcAuxX, _funcAuxY;

        ///// <summary>Auxiliary function used to adapt definition of parametric curve to definition by function of 1 variables.</summary>
        //protected IRealFunction FuncAuxX
        //{ get { if (_funcAuxX == null) _funcAuxX = new Func2dX(); return _funcAuxX; } }

        ///// <summary>Auxiliary function used to adapt definition of parametric surface to definition by function of 2 variables.</summary>
        //protected IFunc2d FuncAuxY
        //{ get { if (_funcAuxY == null) _funcAuxY = new Func2dY(); return _funcAuxY; } }

        /// <summary>Removes any eventual definition of surface by functions (either parametric or explicit).</summary>
        public void ClearSurfaceDefinition()
        {
            FunctionX = null;
            FunctionY = null;
            FunctionZ = null;
        }

        /// <summary>Default function that specifies how values assigned to points on the plotted curve are generated.</summary>
        public static IFunc3d DefaultValueFunctionOfCoordinates = new Func3dZ();

        public IFunc3d _valueFunctionOfCoordinates = DefaultValueFunctionOfCoordinates;

        /// <summary>3D function that defines dependence of value assigned to points on the curve on coordinates of that point.
        /// <para>This function is used to assign values to points on the curve, which can then be used for plotting curves in color.</para></summary>
        public IFunc3d ValueFunctionOfCoordinates
        {
            get { return _valueFunctionOfCoordinates; } 
            set {
                if (value == null) 
                    value = DefaultValueFunctionOfCoordinates;
                else
                    _valueFunctionOfCoordinates = value;  
            } 
        }


        private UnstructuredMesh1d3d _mesh;

        public UnstructuredMesh1d3d Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }



        private string _valuesFieldName = DefaultValuesFieldName;

        /// <summary>Name of the scalar field on the mesh where values assigned to grid points are stored.
        /// <para>These values are assigned to the </para></summary>
        public string ValuesFieldName
        {
            get { return _valuesFieldName; }
            set { _valuesFieldName = value; }
        }


        /// <summary>Prepares the mesh for curve plot.</summary>
        protected virtual void PrepareMesh()
        {
            lock (Lock)
            {
                Field<double> values = null;
                if (Mesh == null)
                {
                    Mesh = new UnstructuredMesh1d3d(NumX);
                    // Generate coordinates of the curve grid:
                    if (FunctionX != null && FunctionY != null && FunctionZ != null)
                    {
                        // Surface generateed according to functions that specify coordinates:
                        Mesh.GenerateCoordinates(NumX,
                            BoundsParameters.Min[0], BoundsParameters.Max[0],
                            FunctionX, FunctionY, FunctionZ);
                    } else
                    {
                        // Functions are not specified, only a regular grid of nodes with grid direction
                       // parallel to the X coordinate direction is generated:
                        Mesh.GenerateCoordinates(NumX,
                            BoundsParameters.Min[0], BoundsParameters.Max[0]);
                    }
                    if (OutputLevel > 0)
                    {
                        Timer.Stop();
                        Console.WriteLine("Grid coordinates (" + NumX + ") calculated. Time: " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
                        Timer.Start();
                    }
                    // Generate values of the surface grid:
                    if (ValueFunctionOfCoordinates != null)
                    {
                        values = Mesh.GetScalarField(ValuesFieldName);
                        if (values == null)
                            values = Mesh.AddScalarField(ValuesFieldName, null);
                        for (int i = 0; i < values.Length; ++i)
                        {
                            values[i] = ValueFunctionOfCoordinates.Value(Mesh[i]);
                        }
                        if (OutputLevel > 0)
                        {
                            Timer.Stop();
                            Console.WriteLine("Values on surface calculated. Time: " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
                            Timer.Start();
                        }

                    }
                }
                if (Mesh != null)
                {
                    BoundsCoordinates.Reset();
                    Field<vec3>.UpdateBounds(Mesh, BoundsCoordinates);
                    if (values != null)
                    {
                        BoundsPointValues.Reset();
                        Field<double>.UpdateBounds(values, BoundsPointValues, 0 /* compoent index */);
                    }
                    if (OutputLevel > 0)
                    {
                        Timer.Stop();
                        Console.WriteLine("Bounds on coordinates and values calculated. Time: " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
                        Timer.Start();
                    }
                }
            } // lock
        }

        /// <summary>Creates the curve plot.</summary>
        public override void Create()
        {
            lock (Lock)
            {
                if (OutputLevel > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Preparing a curve plot...");
                    Timer.Reset();
                    Timer.Start();
                }
                // Prepare geometry - structured grid of coordinates and field holding values:
                PrepareMesh();

                UnstructuredMesh1d3d mesh = Mesh;
                if (mesh == null)
                    throw new InvalidOperationException("Curve geometry (1D grid of points in 3D) is not specified.");
                Field<double> values;
                values = Mesh.GetScalarField(ValuesFieldName);

                bool colorPointsScaled = PointsVisible && PointColorIsScaled && values != null;
                bool colorLinesScaled = LinesVisible && LineColorIsScaled && values != null;

                // Prepare auxiliary structures for VTK Polydata:
                // Collection of cell vertices:
                vtkPoints lineframePoints = vtkPoints.New();
                // Collection of lines:
                bool assembleLineFrame = (LinesVisible || PointsVisible);
                vtkCellArray lineFrameCells = null;
                if (assembleLineFrame)
                    lineFrameCells = vtkCellArray.New();

                // Collection of quadrilaterals:
                bool assembleSurfaceCells = (PointsVisible);
                vtkCellArray surfaceCells = null;
                if (assembleSurfaceCells)
                    surfaceCells = vtkCellArray.New();


                // Collection of point values:
                bool assemblePointValues = (LinesVisible || PointsVisible);
                vtkDoubleArray scalarPointValues = null;
                if (assemblePointValues)
                {
                    scalarPointValues = vtkDoubleArray.New();
                    scalarPointValues.SetNumberOfComponents(1);
                    string scalarPointValuesString = "Point values";
                    scalarPointValues.SetName(scalarPointValuesString);
                }

                // Arrays containing colors:
                // Colors for points:
                ColorScale colorScalePoints = PointColorScale;
                vtkUnsignedCharArray colorsPoints = null;
                if (colorPointsScaled)
                {
                    if (colorScalePoints == null)
                        throw new InvalidOperationException("Color scale for points is not specified (null reference) although it should be used.");
                    colorScalePoints.MinValue = BoundsPointValues.MinX;
                    colorScalePoints.MaxValue = BoundsPointValues.MaxX;
                    colorsPoints = vtkUnsignedCharArray.New();
                    colorsPoints.SetNumberOfComponents(3);
                    colorsPoints.SetName("Point colors");
                }

                // Colors for lines:
                ColorScale colorScaleLines = LineColorScale;
                vtkUnsignedCharArray colorsLinePoints = null;
                if (colorLinesScaled)
                {
                    if (colorScaleLines == null)
                        throw new InvalidOperationException("Color scale for points is not specified (null reference) although it should be used.");
                    colorScaleLines.MinValue = BoundsPointValues.MinX;
                    colorScaleLines.MaxValue = BoundsPointValues.MaxX;
                    colorsLinePoints = vtkUnsignedCharArray.New();
                    colorsLinePoints.SetNumberOfComponents(3);
                    colorsLinePoints.SetName("Line point colors");
                }


                double numi = Mesh.Length;
                color pointColor1, pointColor2;
                int firstPointId = 0;
                for (int i = 0; i < numi - 1; i++)
                {

                    // Current quadrilateral cell's vertices' coordinates:
                    vec3
                        v1 = mesh[i],
                        v2 = mesh[i + 1];
                    double
                        value1 = values[i],
                        value2 = values[i + 1],
                        x1 = v1.x,
                        y1 = v1.y,
                        z1 = v1.z,
                        x2 = v2.x,
                        y2 = v2.y,
                        z2 = v2.z;

                    if (assemblePointValues)
                    {
                        //Add scalar values corresponding to the 4 vertices:
                        //scalarPointValues.InsertNextTuple1(z1);
                        //scalarPointValues.InsertNextTuple1(z2);
                        //scalarPointValues.InsertNextTuple1(z3);
                        //scalarPointValues.InsertNextTuple1(z4);
                        scalarPointValues.InsertNextTuple1(value1);
                        scalarPointValues.InsertNextTuple1(value2);
                    }

                    // Prepare VTK arrays for colors in the case that color scales are used:
                    if (colorPointsScaled)
                    {
                        pointColor1 = colorScalePoints.GetColor(value1);
                        pointColor2 = colorScalePoints.GetColor(value2);
                        colorsPoints.InsertNextTuple3(pointColor1.IntR, pointColor1.IntG, pointColor1.IntB);
                        colorsPoints.InsertNextTuple3(pointColor2.IntR, pointColor2.IntG, pointColor2.IntB);
                    }
                    if (colorLinesScaled)
                    {
                        pointColor1 = colorScaleLines.GetColor(value1);
                        pointColor2 = colorScaleLines.GetColor(value2);
                        colorsLinePoints.InsertNextTuple3(pointColor1.IntR, pointColor1.IntG, pointColor1.IntB);
                        colorsLinePoints.InsertNextTuple3(pointColor2.IntR, pointColor2.IntG, pointColor2.IntB);
                    }
                    

                    //Add 2 corner points of the line cell to the collection of points:
                    lineframePoints.InsertNextPoint(x1, y1, z1);
                    lineframePoints.InsertNextPoint(x2, y2, z2);

                    if (assembleLineFrame)
                    {
                        //Create 2 lines for the wireframe (to outline the current cell):
                        vtkLine line1 = vtkLine.New();
                        line1.GetPointIds().SetId(0, firstPointId + 0);
                        line1.GetPointIds().SetId(1, firstPointId + 1);
                        //Add these lines to the cell array:
                        lineFrameCells.InsertNextCell(line1);
                    }


                    if (assembleSurfaceCells)
                    {
                        // Add surface cells:
                        // These surface cells are degenerate and are in fact lines; ew add them
                        // just because nodes of the curve can be shown in this way.
                        // Create a quad on the four points
                        vtkQuad quad = vtkQuad.New();
                        quad.GetPointIds().SetId(0, firstPointId + 0);
                        quad.GetPointIds().SetId(1, firstPointId + 1);
                        quad.GetPointIds().SetId(2, firstPointId + 1);
                        quad.GetPointIds().SetId(3, firstPointId + 0);
                        // Create a cell array to store the quad in
                        surfaceCells.InsertNextCell(quad);
                    }

                    firstPointId += 2;
                } // for loop over cells

                // TODO: check whether these objects should be included for disposal:
                // quad, lineFramePoints, lineFrameLines, colorsSurfacePoints, etc.

                if (OutputLevel > 0)
                {
                    Timer.Stop();
                    Console.WriteLine("Lineframe and surface cells assembled in " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
                    Timer.Start();
                }

                // Prepare actors for curve plot:
                if (LinesVisible)
                {
                    // Create actor for lines:
                    vtkPolyData lineFramePolyData = vtkPolyData.New();
                    AddDataset(lineFramePolyData);
                    lineFramePolyData.SetLines(lineFrameCells);
                    lineFramePolyData.SetPoints(lineframePoints);
                    if (colorLinesScaled)
                    {
                        //lineFramePolyData.GetCellData().SetScalars(colorsLines); // collors assigned to lines, lines will have uniform colors
                        //lineFramePolyData.GetPointData().SetScalars(scalarPointValues);  // only scalar values assigned, default coloring will be performed.
                        lineFramePolyData.GetPointData().SetScalars(colorsLinePoints);  // colors assigned to points, cmooth color transitions
                    }
                    // Prepare mapper and actor:
                    vtkPolyDataMapper lineFrameMapper = vtkPolyDataMapper.New();
                    this.AddMapper(lineFrameMapper);
                    lineFrameMapper.SetInput(lineFramePolyData);
                    vtkActor lineFrameActor = vtkActor.New();
                    this.AddActor(lineFrameActor);
                    lineFrameActor.SetMapper(lineFrameMapper);
                    vtkProperty lineFrameProperties = lineFrameActor.GetProperty();
                    lineFrameProperties.SetColor(LineColor.R, LineColor.G, LineColor.B);  // if point colors are set then this is overridden
                    lineFrameProperties.SetOpacity(LineColorOpacity);
                    lineFrameProperties.SetLineWidth((float)LineWidth);
                }


                if (PointsVisible)
                {

                    // Create actor for POINTS:
                    vtkPolyData pointsPolyData = vtkPolyData.New();
                    this.AddDataset(pointsPolyData);
                    pointsPolyData.SetPolys(surfaceCells);
                    pointsPolyData.SetPoints(lineframePoints);
                    if (colorPointsScaled)
                    {
                        //lineFramePolyData.GetCellData().SetScalars(colorsLines); // collors assigned to lines, lines will have uniform colors
                        //lineFramePolyData.GetPointData().SetScalars(scalarPointValues);  // only scalar values assigned, default coloring will be performed.
                        pointsPolyData.GetPointData().SetScalars(colorsPoints);  // colors assigned to points, cmooth color transitions
                    }
                    // Prepare mapper and actor:
                    vtkPolyDataMapper pointsMapper = vtkPolyDataMapper.New();
                    this.AddMapper(pointsMapper);
                    pointsMapper.SetInput(pointsPolyData);

                    vtkActor pointsActor = vtkActor.New();
                    this.AddActor(pointsActor);
                    pointsActor.SetMapper(pointsMapper);
                    vtkProperty pointsProperties = pointsActor.GetProperty();
                    pointsProperties.SetColor(PointColor.R, PointColor.G, PointColor.B);
                    pointsProperties.SetOpacity(PointColorOpacity);
                    pointsProperties.SetPointSize((float)(PointSize));
                    pointsProperties.SetRepresentationToPoints();
                }

                
                if (OutputLevel > 0)
                {
                    Timer.Stop();
                    Console.WriteLine("Final structures prepared in in " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
                    Timer.Start();
                }
            } // lock
        }  // CreatePlot()


    }  // class VtkSurfacePlot






}
