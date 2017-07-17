using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kitware.VTK;

using IG.Lib;
using IG.Num;

namespace IG.Gr3d
{

    /// <summary>Handles surface plots in VTK render window accessed through the <see cref="VtkPlotter"/> class.
    /// <para>Generated surface plots are based on structured 2D meshes embedded in 3D.</para></summary>
    /// $A Igor xx Oct11;
    public class VtkSurfacePlot: VtkSurfacePlotBase
    {

        public VtkSurfacePlot(VtkPlotter plotter)
            : base(plotter)
        { }

        public VtkSurfacePlot(VtkPlotter plotter, StructuredMesh2d3d mesh)
            : this(plotter)
        {
            this.Mesh = mesh;
        }

        #region Data

        /// <summary>Default number of surface gridpoints in the first grid direction (for structured grids).</summary>
        public static int DefaultNumX = 20;
        
        /// <summary>Default numberof surface gridpoints in the second direction (for structured grids).</summary>
        public static int DefaultNumY = 20;


        private int _numX = DefaultNumX, _numY = DefaultNumY;

        private bool _randomPoints = false;

        /// <summary>Number of gridpoints of the plotted surface in the first grid direction.
        /// <para>Used for construction of surface grid when the surface is defined by functions.</para></summary>
        /// <remarks>Default value can be changed by setting <see cref="DefaultNumX"/>.</remarks>
        public int NumX
        { get { return _numX; }
            set
            {
                if (value > 1)
                    _numX = value;
                else
                    throw new ArgumentException("Number of gridpoints in the X direction can not be set to value less than 2.");
            }
        }

        /// <summary>Number of gridpoints of the plotted surface in the second grid direction.
        /// <para>Used for construction of surface grid when the surface is defined by functions.</para></summary>
        /// <remarks>Default value can be changed by setting <see cref="DefaultNumY"/>.</remarks>
        public int NumY
        { get { return _numY; }
            set
            {
                if (value > 1)
                    _numY = value;
                else
                    throw new ArgumentException("Number of gridpoints in the Y direction can not be set to value less than 2.");
            }
        }

        /// <summary> Selected points are distributed randomly on the surface. </summary>
        /// 
        public bool RandomPoints
        {
            get { return _randomPoints; }
            set { _randomPoints = value; }
        }

        public static BoundingBox2d DefaultBoundsParameters = new BoundingBox2d(-1, 1, -1, 1);

        private BoundingBox2d _boundsParameters;

        /// <summary>Bounds on reference parameters used for generation of the surface grid.</summary>
        public BoundingBox2d BoundsParameters
        {
            get {
                if (_boundsParameters == null)
                    _boundsParameters = new BoundingBox2d(DefaultBoundsParameters);
                return _boundsParameters;
            }
            set { _boundsParameters = value; }
        }

        /// <summary>Sets the bounds on reference coordinates used in generation of the surface grid.</summary>
        /// <param name="minX">Lower bound for the first coordinate.</param>
        /// <param name="maxX">Upper bound for the first coordinate.</param>
        /// <param name="minY">Lower bound for the second coordinate.</param>
        /// <param name="maxY">Upper bound for the second coordinate.</param>
        public void SetBoundsParameters(double minX, double maxX, double minY, double maxY)
        {
            lock (Lock)
            {
                BoundsParameters.Reset();
                BoundsParameters.Update(0, minX, maxX);
                BoundsParameters.Update(1, minY, maxY);
            }
        }

        /// <summary>Sets the bounds on reference coordinates used in generation of the surface grid.</summary>
        /// <param name="bounds">New bounds on parameters.</param>
        public void SetBoundsParameters(IBoundingBox bounds)
        {
            lock (Lock)
            {
                if (bounds != null)
                {
                    if (bounds.Dimension != 2)
                        throw new ArgumentException("Dimension of bounding box different than 2.");
                    BoundsParameters.Reset();
                    BoundsParameters.Update(bounds);
                }
            }
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

        // private IFunc2d _surfaceFunction;

        /// <summary>Sets function of 2 variables whose graph is plotted.
        /// <para>The specified function represents explicit definition of surface.</para></summary>
        public IFunc2d Function
        {
            set {
                FunctionZ = value;
                FunctionX = FuncAuxX;
                FunctionY = FuncAuxY;
            }
        }

        private IFunc2d _functionX, _functionY, _functionZ;

        /// <summary>The first component of a 3D vector function of 2 parameters that acts as parametric
        /// definition of the plotted surface.</summary>
        public IFunc2d FunctionX
        { get { return _functionX; } 
            protected set { 
                _functionX = value; 
                if (_functionX!=null && _functionY!=null && _functionZ!=null) Mesh = null; 
            } 
        }

        /// <summary>The second component of a 3D vector function of 2 parameters that acts as parametric
        /// definition of the plotted surface.</summary>
        public IFunc2d FunctionY
        { get { return _functionY; } 
            protected set 
            { 
                _functionY = value;
                if (_functionX != null && _functionY != null && _functionZ != null) Mesh = null;  
            } 
        }

        /// <summary>The third component of a scalar function of 2 parameters that acts as parametric
        /// definition of the plotted surface.</summary>
        public IFunc2d FunctionZ
        { get { return _functionZ; } 
            protected set  { 
                _functionZ = value;
                if (_functionX != null && _functionY != null && _functionZ != null) Mesh = null;  
            } 
        }

        /// <summary>Sets the definition of parametric surface in 3D.</summary>
        /// <param name="fX">Function that specifies the first component of coordinate of a surface point dependent on 
        /// parameters of the parametrically defined surface.</param>
        /// <param name="fY">Function that specifies the second component of coordinate of a surface point dependent on 
        /// parameters of the parametrically defined surface.</param>
        /// <param name="fZ">Function that specifies the third component of coordinate of a surface point dependent on 
        /// parameters of the parametrically defined surface.</param>
        public void SetSurfaceDefinition(IFunc2d fX, IFunc2d fY, IFunc2d fZ)
        { FunctionX = fX; FunctionY = fY; FunctionZ = fZ; }

        /// <summary>Sets the definition of parametric surface in 3D.
        /// <para>Components of the specified 3D vector functions of 2D variables define indivitual coordinates
        /// on the surface dependent on the two parameters of the parametric surface (as 2D scalar functions).</para></summary>
        /// <param name="functions">A 3D vector function of 2 variables. Each component (a 2D scalar function) defines 
        /// dependence of one cartesian coordinate on two surface parameters.</param>
        public void SetSurfaceDefinition(IFunc3d2d functions)
        {
            FunctionX = functions.Component1;
            FunctionY = functions.Component2;
            FunctionZ = functions.Component3;
        }


        /// <summary>Sets explicit definition of a surface as graph of a function of two variables.</summary>
        /// <param name="funcZ">Function that deines the dependence of Z coordinate of a point on the surface on X and Y coordinates.</param>
        public void SetSurfaceDefinition(IFunc2d funcZ)
        {
            FunctionX = FuncAuxX;
            FunctionY = FuncAuxY;
            FunctionZ = funcZ;
        }

        private IFunc2d _funcAuxX, _funcAuxY;

        /// <summary>Auxiliary function used to adapt definition of parametric surface to definition by function of 2 variables.</summary>
        protected IFunc2d FuncAuxX
        { get { if (_funcAuxX == null) _funcAuxX = new Func2dX(); return _funcAuxX; } }

        /// <summary>Auxiliary function used to adapt definition of parametric surface to definition by function of 2 variables.</summary>
        protected IFunc2d FuncAuxY
        { get { if (_funcAuxY == null) _funcAuxY = new Func2dY(); return _funcAuxY; } }

        /// <summary>Removes any eventual definition of surface by functions (either parametric or explicit).</summary>
        public void ClearSurfaceDefinition()
        {
            FunctionX = null;
            FunctionY = null;
            FunctionZ = null;
        }

        /// <summary>Default function that specifies how values assigned to points on the plotted surface are generated.</summary>
        public static IFunc3d DefaultValueFunctionOfCoordinates = new Func3dZ();

        public IFunc3d _valueFunctionOfCoordinates = DefaultValueFunctionOfCoordinates;

        /// <summary>3F function that defines dependence of value assigned to points on the surface on coordinates of that point.
        /// <para>This function is used to assign values to points on the surface, which can then be used for plotting contours
        /// on the surface or for coloring surface.</para></summary>
        public IFunc3d ValueFunctionOfCoordinates
        {
            get { return _valueFunctionOfCoordinates; } 
            set {
                if (value == null) 
                    value = new Func3dZero();
                else
                    _valueFunctionOfCoordinates = value;  
            } 
        }

        private vec3[] RandomMesh;

        private StructuredMesh2d3d _mesh;

        public StructuredMesh2d3d Mesh
        {
            get { return _mesh; }
            set { 
                _mesh = value;
                if (value != null)
                {
                    // Update dependencies:
                    _numX = value.Dim1;
                    _numY = value.Dim2;
                }
            }
        }


        private string _valuesFieldName = DefaultValuesFieldName;

        /// <summary>Name of the scalar field on the mesh where values assigned to grid points are stored.
        /// <para>These values are assigned to the </para></summary>
        public string ValuesFieldName
        {
            get { return _valuesFieldName; }
            set { _valuesFieldName = value; }
        }

        #endregion Data

        #region ForContourPlots

        /// <summary>Whether the current oject is intended for creation of contour plots.</summary>
        /// <remarks>This flags enables surface and contour plotting classes to share most functionality.</remarks>
        protected bool _isContourPlot = false;

        public static int DefaultNumContours = 20;

        /// <summary>Number of contours.</summary>
        protected int _numContours = DefaultNumContours;

        #endregion ForContourPlots

        #region Operation

        protected virtual void PrepareRandomMesh()
        {
            lock (Lock)
            {
                if (RandomMesh == null)
                {
                    int totalTrainingPoints = NumX * NumY;
                    RandomMesh = new vec3[totalTrainingPoints];
                    //vec3[] temRandomMesh = new vec3[totalTrainingPoints];

                    for (int i = 0; i < totalTrainingPoints; i++)
                    {
                        IVector randomPoint = null;
                        BoundsParameters.GetRandomPoint(ref randomPoint);

                        RandomMesh[i][0] = randomPoint[0];
                        RandomMesh[i][1] = randomPoint[1];
                        RandomMesh[i][2] = 0.0;
                    }

                    if (FunctionX != null && FunctionY != null && FunctionZ != null)
                    {
                        for (int i = 0; i < totalTrainingPoints; i++)
                        {
                            vec3 tmpRandomPoint;
                            IVector randomPoint = new Vector(3);


                            tmpRandomPoint = RandomMesh[i];

                            RandomMesh[i].x = FunctionX.Value(tmpRandomPoint.x, tmpRandomPoint.y);
                            RandomMesh[i].y = FunctionY.Value(tmpRandomPoint.x, tmpRandomPoint.y);
                            RandomMesh[i].z = FunctionZ.Value(tmpRandomPoint.x, tmpRandomPoint.y);
                        }
                    }
                }
            }
        }

        /// <summary>Prepares the mesh for surface plot.</summary>
        protected virtual void PrepareMesh()
        {
            lock (Lock)
            {
                StructuredField2d<double> values = null;
                if (Mesh == null)
                {
                    Mesh = new StructuredMesh2d3d(NumX, NumY);
                    // Generate coordinates of the surface grid:
                    if (FunctionX != null && FunctionY != null && FunctionZ != null)
                    {
                        // Surface generateed according to functions that specify coordinates:
                        Mesh.GenerateCoordinates(
                            BoundsParameters.Min[0], BoundsParameters.Max[0],
                            BoundsParameters.Min[1], BoundsParameters.Max[1],
                            FunctionX, FunctionY, FunctionZ);
                    }
                    else
                    {
                        // Functions are not specified, only a regular surface grid with grid directions parallel to the first
                        // two coordinates is generated:
                        Mesh.GenerateCoordinates(
                            BoundsParameters.Min[0], BoundsParameters.Max[0],
                            BoundsParameters.Min[1], BoundsParameters.Max[1]);
                    }
                    if (OutputLevel > 0)
                    {
                        Timer.Stop();
                        Console.WriteLine("Grid coordinates (" + NumX + "x" + NumY + ") calculated. Time: " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
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
                    values = Mesh.GetScalarField(ValuesFieldName);
                    if (values == null)
                    {
                        values = Mesh.AddScalarField(ValuesFieldName, null);
                        if (ValueFunctionOfCoordinates != null)
                        {
                            for (int i = 0; i < values.Length; ++i)
                            {
                                values[i] = ValueFunctionOfCoordinates.Value(Mesh[i]);
                            }
                        } else
                        {
                            for (int i = 0; i < values.Length; ++i)
                            {
                                values[i] = Mesh[i].z;
                            }
                        }
                        if (OutputLevel > 0)
                        {
                            Timer.Stop();
                            Console.WriteLine("Values on surface calculated. Time: " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
                            Timer.Start();
                        }
                    }

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

        /// <summary>Creates the surface plot.</summary>
        public override void Create()
        {
            lock (Lock)
            {
                if (OutputLevel > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Preparing a surface plot...");

                    Timer.Reset();
                    Timer.Start();
                }
                StructuredMesh2d3d mesh;
                PrepareMesh();
                if (!_randomPoints)
                {
                    // Prepare geometry - structured grid of coordinates and field holding values:
                    PrepareMesh();
                    mesh = Mesh;
                }
                else
                {
                    PrepareRandomMesh();
                    Mesh.Coordinates = RandomMesh;
                    mesh = Mesh;
                }
               
                //mesh = Mesh;
                if (mesh == null)
                    throw new InvalidOperationException("Surface geometry (2D structured grid of points in 3D) is not specified.");
                StructuredField2d<double> values;
                values = Mesh.GetScalarField(ValuesFieldName);

                bool colorPointsScaled = PointsVisible && PointColorIsScaled && values != null;
                bool colorLinesScaled = LinesVisible && LineColorIsScaled && values != null;
                bool colorSurfacesScaled = SurfacesVisible && SurfaceColorIsScaled && values != null;

                // Prepare auxiliary structures for VTK Polydata:
                // Collection of cell vertices:
                vtkPoints lineFramePoints = vtkPoints.New();
                // Collection of lines:
                bool assembleLineFrame = (LinesVisible && !_isContourPlot);
                vtkCellArray lineFrameLines = null;
                if (assembleLineFrame)
                    lineFrameLines = vtkCellArray.New();
                // Collection of quadrilaterals:
                bool assembleSurfaceCells = (SurfacesVisible || PointsVisible) 
                    || (_isContourPlot && (SurfacesVisible || LinesVisible));
                vtkCellArray surfaceCells = null;
                if (assembleSurfaceCells)
                    surfaceCells = vtkCellArray.New();
                // Collection of point values:
                bool assemblePointValues = _isContourPlot && (SurfacesVisible || LinesVisible);
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

                // Colors for surfaces:
                ColorScale colorScaleSurfaces = SurfaceColorScale;
                vtkUnsignedCharArray colorsSurfacePoints = null;
                if (colorSurfacesScaled)
                {
                    if (colorScaleSurfaces == null)
                        throw new InvalidOperationException("Color scale for surfaces is not specified (null reference) although it should be used.");
                    colorScaleSurfaces.MinValue = BoundsPointValues.MinX;
                    colorScaleSurfaces.MaxValue = BoundsPointValues.MaxX;
                    colorsSurfacePoints = vtkUnsignedCharArray.New();
                    colorsSurfacePoints.SetNumberOfComponents(3);
                    colorsSurfacePoints.SetName("Surface point colors");
                }


                double numi = Mesh.Dim1;
                double numj = Mesh.Dim2;
                color pointColor1, pointColor2, pointColor3, pointColor4;
                int firstPointId = 0;
                for (int j = 0; j < numj - 1; j++)
                {
                    for (int i = 0; i < numi - 1; i++)
                    {

                        // Current quadrilateral cell's vertices' coordinates:
                        vec3
                            v1 = mesh[i, j],
                            v2 = mesh[i + 1, j],
                            v3 = mesh[i + 1, j + 1],
                            v4 = mesh[i, j + 1];
                        double
                            value1 = values[i, j],
                            value2 = values[i + 1, j],
                            value3 = values[i + 1, j + 1],
                            value4 = values[i, j + 1],
                            x1 = v1.x,
                            y1 = v1.y,
                            z1 = v1.z,
                            x2 = v2.x,
                            y2 = v2.y,
                            z2 = v2.z,
                            x3 = v3.x,
                            y3 = v3.y,
                            z3 = v3.z,
                            x4 = v4.x,
                            y4 = v4.y,
                            z4 = v4.z;

                        if (assemblePointValues)
                        {
                            //Add scalar values corresponding to the 4 vertices:
                            //scalarPointValues.InsertNextTuple1(z1);
                            //scalarPointValues.InsertNextTuple1(z2);
                            //scalarPointValues.InsertNextTuple1(z3);
                            //scalarPointValues.InsertNextTuple1(z4);
                            scalarPointValues.InsertNextTuple1(value1);
                            scalarPointValues.InsertNextTuple1(value2);
                            scalarPointValues.InsertNextTuple1(value3);
                            scalarPointValues.InsertNextTuple1(value4);
                        }

                        // Prepare VTK arrays for colors in the case that color scales are used:
                        if (colorPointsScaled)
                        {
                            pointColor1 = colorScalePoints.GetColor(value1);
                            pointColor2 = colorScalePoints.GetColor(value2);
                            pointColor3 = colorScalePoints.GetColor(value3);
                            pointColor4 = colorScalePoints.GetColor(value4);
                            colorsPoints.InsertNextTuple3(pointColor1.IntR, pointColor1.IntG, pointColor1.IntB);
                            colorsPoints.InsertNextTuple3(pointColor2.IntR, pointColor2.IntG, pointColor2.IntB);
                            colorsPoints.InsertNextTuple3(pointColor3.IntR, pointColor3.IntG, pointColor3.IntB);
                            colorsPoints.InsertNextTuple3(pointColor4.IntR, pointColor4.IntG, pointColor4.IntB);
                        }
                        if (colorLinesScaled)
                        {
                            pointColor1 = colorScaleLines.GetColor(value1);
                            pointColor2 = colorScaleLines.GetColor(value2);
                            pointColor3 = colorScaleLines.GetColor(value3);
                            pointColor4 = colorScaleLines.GetColor(value4);
                            colorsLinePoints.InsertNextTuple3(pointColor1.IntR, pointColor1.IntG, pointColor1.IntB);
                            colorsLinePoints.InsertNextTuple3(pointColor2.IntR, pointColor2.IntG, pointColor2.IntB);
                            colorsLinePoints.InsertNextTuple3(pointColor3.IntR, pointColor3.IntG, pointColor3.IntB);
                            colorsLinePoints.InsertNextTuple3(pointColor4.IntR, pointColor4.IntG, pointColor4.IntB);
                        }
                        if (colorSurfacesScaled)
                        {
                            pointColor1 = colorScaleSurfaces.GetColor(value1);
                            pointColor2 = colorScaleSurfaces.GetColor(value2);
                            pointColor3 = colorScaleSurfaces.GetColor(value3);
                            pointColor4 = colorScaleSurfaces.GetColor(value4);
                            colorsSurfacePoints.InsertNextTuple3(pointColor1.IntR, pointColor1.IntG, pointColor1.IntB);
                            colorsSurfacePoints.InsertNextTuple3(pointColor2.IntR, pointColor2.IntG, pointColor2.IntB);
                            colorsSurfacePoints.InsertNextTuple3(pointColor3.IntR, pointColor3.IntG, pointColor3.IntB);
                            colorsSurfacePoints.InsertNextTuple3(pointColor4.IntR, pointColor4.IntG, pointColor4.IntB);
                        }

                        ////Add scalar values corresponding to the 4 vertices:
                        //scalarPointValues.InsertNextTuple1(z1);
                        //scalarPointValues.InsertNextTuple1(z2);
                        //scalarPointValues.InsertNextTuple1(z3);
                        //scalarPointValues.InsertNextTuple1(z4);

                        //color lineColor1 = color.Average(pointColor1, pointColor2);
                        //color lineColor2 = color.Average(pointColor2, pointColor3);
                        //color lineColor3 = color.Average(pointColor3, pointColor4);
                        //color lineColor4 = color.Average(pointColor4, pointColor1);
                        //colorsLines.InsertNextTuple3(lineColor1.IntR, lineColor1.IntG, lineColor1.IntB);
                        //colorsLines.InsertNextTuple3(lineColor2.IntR, lineColor2.IntG, lineColor2.IntB);
                        //colorsLines.InsertNextTuple3(lineColor3.IntR, lineColor3.IntG, lineColor3.IntB);
                        //colorsLines.InsertNextTuple3(lineColor4.IntR, lineColor4.IntG, lineColor4.IntB);

                        //color surfacePointColor1 = surfacePointScale.GetColor(z1);
                        //color surfacePointColor2 = surfacePointScale.GetColor(z2);
                        //color surfacePointColor3 = surfacePointScale.GetColor(z3);
                        //color surfacePointColor4 = surfacePointScale.GetColor(z4);

                        //colorsSurfacePoints.InsertNextTuple3(surfacePointColor1.IntR, surfacePointColor1.IntG, surfacePointColor1.IntB);
                        //colorsSurfacePoints.InsertNextTuple3(surfacePointColor2.IntR, surfacePointColor2.IntG, surfacePointColor2.IntB);
                        //colorsSurfacePoints.InsertNextTuple3(surfacePointColor3.IntR, surfacePointColor3.IntG, surfacePointColor3.IntB);
                        //colorsSurfacePoints.InsertNextTuple3(surfacePointColor4.IntR, surfacePointColor4.IntG, surfacePointColor4.IntB);

                        //color surfaceColor = color.Average(surfacePointColor1, surfacePointColor2, surfacePointColor3, surfacePointColor4);
                        //colorsSurfaces.InsertNextTuple3(surfaceColor.IntR, surfaceColor.IntG, surfaceColor.IntB);


                        //Add 4 corner points of the cell to the collection of points:
                        lineFramePoints.InsertNextPoint(x1, y1, z1);
                        lineFramePoints.InsertNextPoint(x2, y2, z2);
                        lineFramePoints.InsertNextPoint(x3, y3, z3);
                        lineFramePoints.InsertNextPoint(x4, y4, z4);

                        if (assembleLineFrame)
                        {
                            //Create 4 lines for the wireframe (to outline the current cell):
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
                        }

                        if (assembleSurfaceCells)
                        {
                            // Add surface cells:
                            // Create a quad on the four points
                            vtkQuad quad = vtkQuad.New();
                            quad.GetPointIds().SetId(0, firstPointId + 0);
                            quad.GetPointIds().SetId(1, firstPointId + 1);
                            quad.GetPointIds().SetId(2, firstPointId + 2);
                            quad.GetPointIds().SetId(3, firstPointId + 3);
                            // Create a cell array to store the quad in
                            surfaceCells.InsertNextCell(quad);
                        }


                        firstPointId += 4;
                    }
                } // for loop over cells

                // TODO: check whether these objects should be included for disposal:
                // quad, lineFramePoints, lineFrameLines, colorsSurfacePoints, etc.

                if (OutputLevel > 0)
                {
                    Timer.Stop();
                    Console.WriteLine("Lineframe and surface cells assembled in " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
                    Timer.Start();
                }

                // Prepare actors:
                if (!_isContourPlot)
                {
                    // Surface plots
                    // Create actor for WIRE FRAME:
                    if (LinesVisible)
                    {
                        vtkPolyData lineFramePolyData = vtkPolyData.New();
                        AddDataset(lineFramePolyData);
                        lineFramePolyData.SetLines(lineFrameLines);
                        lineFramePolyData.SetPoints(lineFramePoints);
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

                    // Create an actor for surfaces:
                    if (SurfacesVisible)
                    {

                        // Prepare mapper and actor for SOLID SURFACE (used just for control):
                        // Copy the created PolyData (containing geometry and topology) for use with surface actor:
                        vtkPolyData surfacePolydata = vtkPolyData.New();
                        this.AddDataset(surfacePolydata);
                        surfacePolydata.SetPolys(surfaceCells);
                        surfacePolydata.SetPoints(lineFramePoints);

                        if (colorSurfacesScaled)
                        {
                            //surfacePolydata.GetCellData().SetScalars(colorsSurfaces); // colors assigned to cells, surfaces will have uniform colors
                            surfacePolydata.GetPointData().SetScalars(colorsSurfacePoints);  // colors assigned to points, cmooth color transitions
                        }
                        // Prepare mapper and actor:
                        vtkPolyDataMapper surfaceMapper = vtkPolyDataMapper.New();
                        this.AddMapper(surfaceMapper);
                        surfaceMapper.SetInput(surfacePolydata);
                        vtkActor surfaceActor = vtkActor.New();

                        this.AddActor(surfaceActor);
                        surfaceActor.SetMapper(surfaceMapper);
                        vtkProperty surfaceProperties = surfaceActor.GetProperty();
                        surfaceProperties.SetColor(SurfaceColor.R, SurfaceColor.G, SurfaceColor.B); // if point colors are set then this is overridden
                        surfaceProperties.SetOpacity(SurfaceColorOpacity);
                    }

                    if (PointsVisible)
                    {

                        // Create actor for POINTS:
                        vtkPolyData pointsPolyData = vtkPolyData.New();
                        this.AddDataset(pointsPolyData);
                        pointsPolyData.SetPolys(surfaceCells);
                        pointsPolyData.SetPoints(lineFramePoints);
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
                } else  // _isContourPlot = true
                {
                    // Contour plots: 
                    if (LinesVisible || SurfacesVisible)
                    {

                        // Prepare geometry and topology for contours - create PolyData from points and cells:
                        vtkPolyData surfaceContourPolyData = vtkPolyData.New();
                        this.AddDataset(surfaceContourPolyData);
                        surfaceContourPolyData.SetPolys(surfaceCells); //surfaceContourCells);
                        surfaceContourPolyData.SetPoints(lineFramePoints);

                        // Prepare data for contours:
                        // Assign point values (1 value per each point):
                        surfaceContourPolyData.GetPointData().SetScalars(scalarPointValues);
                        double minVal = BoundsPointValues.MinX; double maxVal = BoundsPointValues.MaxX;
                        if (OutputLevel > 0)
                        {
                            // Check range:
                            Console.WriteLine();
                            double[] valuesRange;
                            valuesRange = surfaceContourPolyData.GetPointData().GetScalars().GetRange();
                            Console.WriteLine("RANGE of point values: from " + minVal + " to " + maxVal + ".");
                            Console.WriteLine("Range obtained form vtkPolyData: ");
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
                        }

                        // Create banded contour filter and generate contour values:
                        vtkBandedPolyDataContourFilter contourFilter = vtkBandedPolyDataContourFilter.New();
                        AddAlgorithm(contourFilter);
                        contourFilter.SetInputConnection(surfaceContourPolyData.GetProducerPort());

                        // Generate numContours equally spaced contours (individual contours may be added by SetValue()):
                        contourFilter.GenerateValues(_numContours, minVal, maxVal);
                        contourFilter.GenerateContourEdgesOn();  // Must be enabled if we want to make line contours (not only filled)
                        contourFilter.Update();

                        // Color the contours
                        // This colors contours obtained on output port 1 by just setting point values that are 
                        // obtained from the output port 0 (here points are obtained through input connection from 
                        // vtkPolyData object surfaceContourPolyData, which is cource of geometry and topology)
                        contourFilter.GetOutput(1).GetPointData().SetScalars(contourFilter.GetOutput().GetPointData().GetScalars());

                        //contourFilter.GetOutput(1).GetPointData().SetScalars(colorsPoints);

                        // Make sure the mapper will use the new colors
                        //bf.GetOutput(0).GetPointData().SetActiveScalars(scalarPointValuesString);

                        // TODO: Figure out how to apply custom colors to contours! Now the contour filter uses
                        // point data (it could also use cell data) for contours and colors them according to
                        // scalar values assigned to points, but we do not specify which colors it uses for which 
                        // contours (or point values, respectively)

                        if (LinesVisible)
                        {
                            // Line contours:
                            // Prepare mapper and actor for contour lines:
                            vtkPolyDataMapper contourLinesMapper = vtkPolyDataMapper.New();
                            AddMapper(contourLinesMapper);
                            contourLinesMapper.SetInputConnection(contourFilter.GetOutputPort(1));  // Switch between filled and line contours
                            vtkActor contourLinesActor = vtkActor.New();

                            AddActor(contourLinesActor);
                            contourLinesActor.SetMapper(contourLinesMapper);
                            vtkProperty contourLinesProperties = contourLinesActor.GetProperty();
                            contourLinesProperties.SetColor(0.0, 0.0, 1.0);
                            contourLinesProperties.SetColor(0, 1, 1);
                            contourLinesProperties.SetLineWidth(3);
                            
                        } // if (LinesVisible)
                        if (SurfacesVisible)
                        {
                            // Surface contours:
                            // Prepare mapper and actor for solid contours:
                            vtkPolyDataMapper surfaceContourMapper = vtkPolyDataMapper.New();
                            AddMapper(surfaceContourMapper);
                            AddMapper(surfaceContourMapper);
                            surfaceContourMapper.SetInputConnection(contourFilter.GetOutputPort(0));  // Switch between filled and line contours
                            surfaceContourMapper.SetScalarRange(minVal, maxVal);

                            surfaceContourMapper.SetScalarModeToUsePointData();

                            // TODO: Figure out how different settings below influence coloring of filled contours!
                            //surfaceContourMapper.SetScalarModeToUsePointFieldData();
                            //surfaceContourMapper.SetScalarModeToUseFieldData();
                            //surfaceContourMapper.SetScalarModeToUseCellData();
                            //surfaceContourMapper.SetScalarModeToUseCellFieldData();

                            vtkActor surfaceContourActor = vtkActor.New();

                            AddActor(surfaceContourActor);
                            AddActor(surfaceContourActor);
                            surfaceContourActor.SetMapper(surfaceContourMapper);
                            vtkProperty surfaceContourProperties = surfaceContourActor.GetProperty();
                            surfaceContourProperties.SetColor(0, 0, 1);
                            surfaceContourProperties.SetLineWidth(3);
                        } // if (SurfacesVisible)
                    }  // if (LinesVisible || SurfacesVisible)
                }           

                if (OutputLevel > 0)
                {
                    Timer.Stop();
                    Console.WriteLine("Final structures prepared in in " + Timer.Time + " s, total: " + Timer.TotalTime + " s.");
                    Timer.Start();
                }
            } // lock
        }  // CreatePlot()


        #endregion Operation

    }  // class VtkSurfacePlot



}
