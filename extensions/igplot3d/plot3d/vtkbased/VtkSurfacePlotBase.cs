using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kitware.VTK;

using IG.Lib;
using IG.Num;

namespace IG.Gr3d
{


    /// <summary>Contains basic settings for various surface and contour plots.</para></summary>
    /// <remarks><para>This is used as base class for surface and contour plots.</para>
    /// <para>Beside its basic function, the class also contains methods for plotting basic graphic 
    /// primitives (points, lines, triangular and quadrilateral surface or wireframe patches, etc.).</para>
    /// <para>Plotting of basic primitives is added on this class for convenience, such that 
    /// minor custom graphics can simply be added on surface plots and contour plots. Aleernative
    /// and somewhat cleaner approach would be to include methods for plotting simple primitives
    /// in a derived class, but it is considered that letting surface and contour plot classes
    /// access this functionality is beneficial.</para>
    /// </remarks>
    /// $A Igor xx Oct11;
    public abstract class VtkSurfacePlotBase : VtkPlotBase
    {
        
        
        /// <summary>Constructor. Just calls base constructor with the same name.</summary>
        /// <param name="plotter">VTK plotter that is used for rendering graphics produeced by the 
        /// current plotting class.</param>
        public VtkSurfacePlotBase(VtkPlotter plotter): base(plotter)
        {  }


        #region Settings

        /// <summary>Default name of the field that contains node values for plotted grids.</summary>
        public static string DefaultValuesFieldName = "values"; 


        /// <summary>Invalidates things that are dependent on any settings.
        /// <para>This function should be called anywhere seddings are changed.</para></summary>
        protected virtual void InvalidateSettingDependencies()
        {
            CreateCustomPlot();
        }

        // Default settings:

        /// <summary>Default value of the flag indicating whether lines and contours will be shown.</summary>
        public static bool DefaultLinesVisible = true;

        /// <summary>Default line thickness for new objects of this class.</summary>
        public static double DefaultLineWidth = 2;

        /// <summary>Default value of the flag indicating whether lines are plotted in color scale, for new plot objects of this class.
        /// Value false means that constant color is used, true means that color scale is used.</summary>
        public static bool DefaultLineColorIsScaled = false;

        /// <summary>Default line color scale for new plot objects of this class.</summary>
        public static ColorScale DefaultLineColorScale = ColorScale.CreateGray(0,1);

        /// <summary>Default line color for new objects of this class.</summary>
        public static color DefaultLineColor = new color(0, 0, 1);


        /// <summary>Default value of the flag indicating whether surfaces will be shown.</summary>
        public static bool DefaultSurfacesVisible = true;

        /// <summary>Default value of the flag indicated whether surfaces are plotted in color scale, for new plot objects of this class.
        /// Value false means that constant color is used, true means that color scale is used.</summary>
        public static bool DefaultSurfaceColorIsScaled = true;

        /// <summary>Default surface color scale for new plot objects of this class.</summary>
        public static ColorScale DefaultSurfaceColorScale = ColorScale.Create(0, 1,
            System.Drawing.Color.DarkBlue, System.Drawing.Color.Green, System.Drawing.Color.Red, System.Drawing.Color.Yellow);

        /// <summary>Default surface color for new objects of this class.</summary>
        public static color DefaultSurfaceColor = new color(0.5, 0.75, 1.0);


        /// <summary>Default value of the flag indicating whether points will be shown.</summary>
        public static bool DefaultPointsVisible = true;

        /// <summary>Default point size for new objects of this class.</summary>
        public static double DefaultPointSize = 2;

        /// <summary>Default value of the flag indicated whether points are plotted in color scale, for new plot objects of this class.
        /// Value false means that constant color is used, true means that color scale is used.</summary>
        public static bool DefaultPointColorIsScaled = false;

        /// <summary>Default point color scale for new plot objects of this class.</summary>
        public static ColorScale DefaultPointColorScale = ColorScale.Create(-1, 1, new color(0.1, 0, 0.3), new color(1, 0.5, 0));

        /// <summary>Default point color for new objects of this class.</summary>
        public static color DefaultPointColor = new color(1, 0, 0);

        // Settings for plotting lines:

        private bool _linesVisible = DefaultLinesVisible;

        /// <summary>Whether line objects are shown in plots or not.</summary>
        public bool LinesVisible
        { get { return _linesVisible; } set { _linesVisible = value; InvalidateSettingDependencies(); } }

        private double _lineWidth = DefaultLineWidth;

        /// <summary>Line thickness used in plots.</summary>
        public double LineWidth
        { get { return _lineWidth; } set { _lineWidth = value; InvalidateSettingDependencies(); } }

        private bool _lineColorIsScaled = DefaultLineColorIsScaled;

        /// <summary>Whether a color scale is used for coloring lines (value true) or a fixed color is used (value false).</summary>
        public bool LineColorIsScaled
        { get { return _lineColorIsScaled; } set { _lineColorIsScaled = value; InvalidateSettingDependencies(); } }

        private ColorScale _lineColorScale = DefaultLineColorScale;

        /// <summary>Color scale used for plotting lines (when the flag <see cref="LineColorIsScaled"/> is true, 
        /// otherwise fixed <see cref="LineColor"/> is uesd).</summary>
        public ColorScale LineColorScale
        { get { return _lineColorScale; } set { if (value != null) _lineColorScale = value; InvalidateSettingDependencies(); } }

        private color _lineColor = DefaultLineColor;

        /// <summary>Color used for lines (when the flag <see cref="LineColorIsScaled"/> is false, 
        /// otherwise <see cref="LineColorScale"/> is uesd that depends on values assigned to line endpoints).</summary>
        public color LineColor
        { 
            get { return _lineColor; } 
            set { 
                _lineColor = value;
                // If opacity has been set separately then that setitng overrides color's opacity:
                if (_lineColorOpacity >= 0)
                    _lineColor.Opacity = _lineColorOpacity;
                InvalidateSettingDependencies(); 
            } 
        }

        private double _lineColorOpacity = -1; // negative value if not set!

        /// <summary>Opacity of lines.</summary>
        /// <remarks>If this property is not set separately then the opacity of <see cref="LineColor"/> is returned by getter.
        /// <para>Once this property is set, even when setitng a new value for <see cref="LineColor"/> property, the opacity on this property
        /// will be overridden by the value that has been set specially.</para>
        /// <para>This property is unset by setting it to a negative value.</para></remarks>
        public double LineColorOpacity
        { get { return LineColor.Opacity; }
            set 
            { 
                _lineColorOpacity = value;
                if (value >= 0)
                {
                    // Change opacity of the current line color:
                    color col = LineColor; col.Opacity = value; LineColor = col; InvalidateSettingDependencies();
                }
            } 
        }


        // Settings for plotting surfaces:

        private bool _surfacesVisible = DefaultSurfacesVisible;

        /// <summary>Whether surface objects are shown in plots or not.</summary>
        public bool SurfacesVisible
        { get { return _surfacesVisible; } set { _surfacesVisible = value; InvalidateSettingDependencies(); } }

        private bool _surfaceColorIsScaled = DefaultSurfaceColorIsScaled;

        /// <summary>Whether a color scale is used for coloring surfaces (value true) or a fixed color is used (value false).</summary>
        public bool SurfaceColorIsScaled
        { get { return _surfaceColorIsScaled; } set { _surfaceColorIsScaled = value; InvalidateSettingDependencies(); } }

        private ColorScale _surfaceColorScale = DefaultSurfaceColorScale;

        /// <summary>Color scale used for plotting surfaces (when the flag <see cref="SurfaceColorIsScaled"/> is true, 
        /// otherwise fixed <see cref="SurfaceColor"/> is uesd).</summary>
        public ColorScale SurfaceColorScale
        { get { return _surfaceColorScale; } set { if (value != null) _surfaceColorScale = value; InvalidateSettingDependencies(); } }

        private color _surfaceColor = DefaultSurfaceColor;

        /// <summary>Color used for surfaces (when the flag <see cref="SurfaceColorIsScaled"/> is false, 
        /// otherwise <see cref="SurfaceColorScale"/> is uesd that depends on values assigned to surface endpoints).</summary>
        public color SurfaceColor
        { 
            get { return _surfaceColor; } 
            set { 
                _surfaceColor = value; 
                // If opacity has been set separately then that setitng overrides color's opacity:
                if (_surfaceColorOpacity >= 0)
                    _surfaceColor.Opacity = _surfaceColorOpacity;
                InvalidateSettingDependencies(); 
            } 
        }

        private double _surfaceColorOpacity = -1; // negative value if not set!

        /// <summary>Opacity of surfaces.</summary>
        /// <remarks>If this property is not set separately then the opacity of <see cref="SurfaceColor"/> is returned by getter.
        /// <para>Once this property is set, even when setitng a new value for <see cref="SurfaceColor"/> property, the opacity on that property
        /// will be overridden by the value that has been set specially.</para>
        /// <para>This property is unset by setting it to a negative value.</para></remarks>
        public double SurfaceColorOpacity
        {  
            get { return SurfaceColor.Opacity; } 
            set 
            {
                _surfaceColorOpacity = value;
                if (value >= 0)
                {
                    // Change opacity of the current surface color:
                    color col = SurfaceColor; col.Opacity = value; SurfaceColor = col; 
                }
                InvalidateSettingDependencies();
            } 
        }



        // Settings for plotting points:

        private bool _pointsVisible = DefaultPointsVisible;

        /// <summary>Whether point objects are shown in plots or not.</summary>
        public bool PointsVisible
        { get { return _pointsVisible; } set { _pointsVisible = value; InvalidateSettingDependencies(); } }

        private double _pointSize = DefaultPointSize;

        /// <summary>Point size used in plots.</summary>
        public double PointSize
        { get { return _pointSize; } set { _pointSize = value; InvalidateSettingDependencies(); } }

        private bool _pointColorIsScaled = DefaultPointColorIsScaled;

        /// <summary>Whether a color scale is used for coloring points (value true) or a fixed color is used (value false).</summary>
        public bool PointColorIsScaled
        { get { return _pointColorIsScaled; } set { _pointColorIsScaled = value; InvalidateSettingDependencies(); } }

        private ColorScale _pointColorScale = DefaultPointColorScale;

        /// <summary>Color scale used for plotting points (when the flag <see cref="PointColorIsScaled"/> is true, 
        /// otherwise fixed <see cref="PointColor"/> is uesd).</summary>
        public ColorScale PointColorScale
        { get { return _pointColorScale; } set { if (value != null) _pointColorScale = value; InvalidateSettingDependencies(); } }

        private color _pointColor = DefaultPointColor;

        /// <summary>Color used for points (when the flag <see cref="PointColorIsScaled"/> is false, 
        /// otherwise <see cref="PointColorScale"/> is uesd that depends on values assigned to points).</summary>
        public color PointColor
        { 
            get { return _pointColor; } 
            set 
            { 
                _pointColor = value; 
                // If opacity has been set separately then that setitng overrides color's opacity:
                if (_pointColorOpacity >= 0)
                    _pointColor.Opacity = _pointColorOpacity;
                InvalidateSettingDependencies(); 
            } 
        }

        public double _pointColorOpacity = 1; // negative value if not set!

        /// <summary>Opacity of points.</summary>
        /// <remarks>If this property is not set separately then the opacity of <see cref="PointColor"/> is returned by getter.
        /// <para>Once this property is set, even when setitng a new value for <see cref="PointColor"/> property, the opacity on that property
        /// will be overridden by the value that has been set specially.</para>
        /// <para>This property is unset by setting it to a negative value.</para></remarks>
        public double PointColorOpacity
        { 
            get { return PointColor.Opacity; } 
            set 
            {
                _pointColorOpacity = value;
                if (value >= 0)
                {
                    // Change opacity of the current point color:
                    color col = PointColor; col.Opacity = value; PointColor = col; 
                }
                InvalidateSettingDependencies();
            } 
        }


        #endregion Settings

        #region CustomPlots

        /* Here there are utilities for creating cutstom plots.
          Custom plots consist of lines, filled polygons and points. 
        Any custom plot will be composed of these primitives.
          Each kind of basic primitives are accummulated on its own array, which is added to corresponding 
        actors that are in turn added to the array of actors of the current plot when the custom plot is 
        updated. When plotting primitives, standard settings are used, and if any setting changes then 
        custom plots will be automatically updated (actors created and added to renderers) such that for 
        the primitives constructd later the new settings will apply.
         */

        // TODO: add text primitives!

        /// <summary>Updates custom plots that have been accumulates since the last update (or since construction
        /// of the current plot object).
        /// <para>For all primitives that have been added since the last update, actors are created and added to
        /// the plot. In this way, the current plotting settings (such as surface colors, line thickness, etc.)
        /// will take effect, and for primitives that are eventually added later, settings valid at that time will apply.</para></summary>
        public void CreateCustomPlot()
        {
            if (_linePoints != null || _lineCells != null)
            {
                try
                {
                    vtkPolyData linePolyData = vtkPolyData.New();
                    AddDataset(linePolyData);
                    linePolyData.SetLines(_lineCells);
                    linePolyData.SetPoints(_linePoints);
                    // Prepare mapper and actor:
                    vtkPolyDataMapper lineMapper = vtkPolyDataMapper.New();
                    this.AddMapper(lineMapper);
                    lineMapper.SetInput(linePolyData);
                    vtkActor lineActor = vtkActor.New();
                    this.AddActor(lineActor);
                    lineActor.SetMapper(lineMapper);
                    vtkProperty lineProperties = lineActor.GetProperty();
                    lineProperties.SetColor(LineColor.R, LineColor.G, LineColor.B);
                    lineProperties.SetOpacity(LineColorOpacity);
                    lineProperties.SetLineWidth((float)LineWidth);
                }
                finally
                {
                    _linePoints = null;
                    _lineCells = null;
                    _linePointIndex = 0;
                }
            }
            if (_surfacePoints != null)
            {
                try
                {
                    vtkPolyData surfacePolydata = vtkPolyData.New();
                    this.AddDataset(surfacePolydata);
                    surfacePolydata.SetPolys(_surfaceCells);
                    surfacePolydata.SetPoints(_surfacePoints);
                    // Prepare mapper and actor:
                    vtkPolyDataMapper surfaceMapper = vtkPolyDataMapper.New();
                    this.AddMapper(surfaceMapper);
                    surfaceMapper.SetInput(surfacePolydata);
                    vtkActor surfaceActor = vtkActor.New();
                    this.AddActor(surfaceActor);
                    surfaceActor.SetMapper(surfaceMapper);
                    vtkProperty surfaceProperties = surfaceActor.GetProperty();
                    surfaceProperties.SetColor(SurfaceColor.R, SurfaceColor.G, SurfaceColor.B); 
                    surfaceProperties.SetOpacity(SurfaceColorOpacity);
                }
                finally
                {
                    _surfacePoints = null;
                    _surfaceCells = null;
                    _surfacePointIndex = 0;
                }
            }
            if (_pointPoints != null)
            {
                try
                {
                    vtkPolyData pointsPolyData = vtkPolyData.New();
                    this.AddDataset(pointsPolyData);
                    pointsPolyData.SetPolys(_pointCells);
                    pointsPolyData.SetPoints(_pointPoints);
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
                finally
                {
                    _pointPoints = null;
                    _pointCells = null;
                    _pointPointIndex = 0;
                }
            }
        }

        public void CreateCustomPlotAndShow()
        {
            CreateCustomPlot();
            ShowPlot();
        }

        // Graphic primitives:

        private int _linePointIndex;

        private vtkPoints _linePoints;

        vtkCellArray _lineCells;

        /// <summary>Adds a point with specified coordinates to the array of line endpoints and returns its index.
        /// Current point index is increased by one.</summary>
        /// <param name="x">X coordinate of the added point.</param>
        /// <param name="y">Y coordinate of the added point.</param>
        /// <param name="z">Z coordinate of the added point.</param>
        /// <returns>Index of the added pont on the list of points, which should be used when addressing 
        /// the point when assembling line cells.</returns>
        private int AddLinePoint(double x, double y, double z)
        { 
            if (_linePoints==null)
            {
                _linePointIndex = 0;
                _linePoints = vtkPoints.New();
            }
            int ret = _linePointIndex;
            _linePoints.InsertNextPoint(x, y, z);
            ++_linePointIndex;
            return ret;
        }

        /// <summary>Adds a line with specified endpoint coordinates to the array of line cells.</summary>
        /// <param name="x1">X coordinate of the first point.</param>
        /// <param name="y1">Y coordinate of the first point.</param>
        /// <param name="z1">Z coordinate of the first point.</param>
        /// <param name="x2">X coordinate of the second point.</param>
        /// <param name="y2">Y coordinate of the second point.</param>
        /// <param name="z2">Z coordinate of the second point.</param>
        protected void AddLineCell(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            lock (Lock)
            {
                if (_lineCells == null)
                    _lineCells = vtkCellArray.New();
                // Add two endpoints to the point array:
                int ind1 = AddLinePoint(x1, y1, z1);
                int ind2 = AddLinePoint(x2, y2, z2);
                // Create a new line and set IDs of its endpoints from the point array:
                vtkLine line = vtkLine.New();
                line.GetPointIds().SetId(0, ind1);
                line.GetPointIds().SetId(1, ind2);
                // Add the line to the array of cells:
                _lineCells.InsertNextCell(line);
            }
        }


        private int _surfacePointIndex;

        private vtkPoints _surfacePoints;

        vtkCellArray _surfaceCells;

        /// <summary>Adds a point with specified coordinates to the array of surface cell vertices and returns its index.
        /// Current point index is increased by one.</summary>
        /// <param name="x">X coordinate of the added point.</param>
        /// <param name="y">Y coordinate of the added point.</param>
        /// <param name="z">Z coordinate of the added point.</param>
        /// <returns>Index of the added pont on the array of points, which should be used when addressing 
        /// the point when assembling surface cells.</returns>
        private int AddSurfacePoint(double x, double y, double z)
        { 
            if (_surfacePoints==null)
            {
                _surfacePointIndex = 0;
                _surfacePoints = vtkPoints.New();
            }
            int ret = _surfacePointIndex;
            _surfacePoints.InsertNextPoint(x, y, z);
            ++_surfacePointIndex;
            return ret;
        }

        /// <summary>Adds a triangular surface cell with specified vertex coordinates to the array of surface cells.</summary>
        /// <param name="x1">X coordinate of the first point.</param>
        /// <param name="y1">Y coordinate of the first point.</param>
        /// <param name="z1">Z coordinate of the first point.</param>
        /// <param name="x2">X coordinate of the second point.</param>
        /// <param name="y2">Y coordinate of the second point.</param>
        /// <param name="z2">Z coordinate of the second point.</param>
        /// <param name="x3">X coordinate of the third point.</param>
        /// <param name="y3">Y coordinate of the third point.</param>
        /// <param name="z3">Z coordinate of the third point.</param>
        protected void AddSurfaceCell(
            double x1, double y1, double z1, 
            double x2, double y2, double z2,
            double x3, double y3, double z3)
        {
            lock (Lock)
            {
                if (_surfaceCells == null)
                    _surfaceCells = vtkCellArray.New();
                // Add vertices to the point array:
                int ind1 = AddSurfacePoint(x1, y1, z1);
                int ind2 = AddSurfacePoint(x2, y2, z2);
                int ind3 = AddSurfacePoint(x3, y3, z3);
                // Create a new triangle and set IDs of its endpoints from the point array:
                vtkTriangle triangle = vtkTriangle.New();
                triangle.GetPointIds().SetId(0, ind1);
                triangle.GetPointIds().SetId(1, ind2);
                triangle.GetPointIds().SetId(2, ind2);
                // Add the triangle to the array of cells:
                _surfaceCells.InsertNextCell(triangle);
            }
        }

        /// <summary>Adds a quadrilateral surface cell with specified vertex coordinates to the array of quadrilateral cells.</summary>
        /// <param name="x1">X coordinate of the first point.</param>
        /// <param name="y1">Y coordinate of the first point.</param>
        /// <param name="z1">Z coordinate of the first point.</param>
        /// <param name="x2">X coordinate of the second point.</param>
        /// <param name="y2">Y coordinate of the second point.</param>
        /// <param name="z2">Z coordinate of the second point.</param>
        /// <param name="x3">X coordinate of the third point.</param>
        /// <param name="y3">Y coordinate of the third point.</param>
        /// <param name="z3">Z coordinate of the third point.</param>
        /// <param name="x4">X coordinate of the third point.</param>
        /// <param name="y4">Y coordinate of the third point.</param>
        /// <param name="z4">Z coordinate of the third point.</param>
        protected void AddSurfaceCell(
            double x1, double y1, double z1, 
            double x2, double y2, double z2,
            double x3, double y3, double z3,
            double x4, double y4, double z4)
        {
            lock (Lock)
            {
                if (_surfaceCells == null)
                    _surfaceCells = vtkCellArray.New();
                // Add vertices to the point array:
                int ind1 = AddSurfacePoint(x1, y1, z1);
                int ind2 = AddSurfacePoint(x2, y2, z2);
                int ind3 = AddSurfacePoint(x3, y3, z3);
                int ind4 = AddSurfacePoint(x4, y4, z4);
                // Create a new triangle and set IDs of its endpoints from the point array:
                vtkTriangle triangle = vtkTriangle.New();
                triangle.GetPointIds().SetId(0, ind1);
                triangle.GetPointIds().SetId(1, ind2);
                triangle.GetPointIds().SetId(2, ind2);
                // Add the triangle to the array of cells:
                _surfaceCells.InsertNextCell(triangle);
            }
        }


        // TODO: Check whether points can be shown without adding cells!

        private int _pointPointIndex;

        private vtkPoints _pointPoints;

        vtkCellArray _pointCells;

        /// <summary>Adds a point with specified coordinates to the array of surface cell vertices and returns its index.
        /// Current point index is increased by one.</summary>
        /// <param name="x">X coordinate of the added point.</param>
        /// <param name="y">Y coordinate of the added point.</param>
        /// <param name="z">Z coordinate of the added point.</param>
        /// <returns>Index of the added pont on the array of points, which should be used when addressing 
        /// the point when assembling surface cells.</returns>
        private int AddPointPoint(double x, double y, double z)
        {
            if (_pointPoints == null)
            {
                _pointPointIndex = 0;
                _pointPoints = vtkPoints.New();
            }
            int ret = _pointPointIndex;
            _pointPoints.InsertNextPoint(x, y, z);
            ++_pointPointIndex;
            return ret;
        }


        /// <summary>Adds a triangular point cell with specified point coordinates to the array of point cells.
        /// A triangular cell is added with coordinates of all three vertices being the same.</summary>
        /// <param name="x1">X coordinate of the point.</param>
        /// <param name="y1">Y coordinate of the point.</param>
        /// <param name="z1">Z coordinate of the point.</param>
        protected void AddPointCell(
            double x1, double y1, double z1)
        {
            lock (Lock)
            {
                if (_pointCells == null)
                    _pointCells = vtkCellArray.New();
                // Add vertices to the point array:
                int ind1 = AddPointPoint(x1, y1, z1);
                int ind2 = AddPointPoint(x1, y1, z1);
                int ind3 = AddPointPoint(x1, y1, z1);
                // Create a new cell and set IDs of its endpoints from the point array:
                vtkTriangle triangle = vtkTriangle.New();
                triangle.GetPointIds().SetId(0, ind1);
                triangle.GetPointIds().SetId(1, ind2);
                triangle.GetPointIds().SetId(2, ind2);
                // Add the triangle to the array of cells:
                _pointCells.InsertNextCell(triangle);
            }
        }

        /// <summary>Adds a point with specified coordinates to the set of graphic primitives that 
        /// will be plotted when <see cref="CreateCustomPlot"/> is called.
        /// <para>Current settings are used to define appearance of the added primitive.</para></summary>
        /// <param name="x">X coordinate of the point.</param>
        /// <param name="y">Y coordinate of the point.</param>
        /// <param name="z">Z coordinate of the point.</param>
        public void AddPoint(double x, double y, double z)
        {
            AddPointCell(x, y, z);
        }

        /// <summary>Adds a point with specified coordinates to the set of graphic primitives that 
        /// will be plotted when <see cref="CreateCustomPlot"/> is called.
        /// <para>Current settings are used to define appearance of the added primitive.</para></summary>
        /// <param name="p">3D vector of point coordinates.</param>
        public void AddPoint(vec3 p)
        { AddPoint(p.x, p.y, p.z); }

        /// <summary>Adds a set of points with specified coordinates to the set of graphic primitives that 
        /// will be plotted when <see cref="CreateCustomPlot"/> is called.
        /// <para>Current settings are used to define appearance of the added primitives.</para></summary>
        /// <param name="points">3D vectors of coordinates of the individual points.</param>
        public void AddPoints(params vec3[] points)
        {
            lock (Lock)
            {
                if (points!=null)
                    for (int i = 0; i < points.Length; ++i)
                        AddPoint(points[i]);
            }
        }

        /// <summary>Adds a line with specified endpoint coordinates to the set of graphic primitives that 
        /// will be plotted when <see cref="CreateCustomPlot"/> is called.
        /// <para>Current settings are used to define appearance of the added primitive.</para></summary>
        /// <param name="x1">X coordinate of the first endpoint.</param>
        /// <param name="y1">Y coordinate of the first endpoint.</param>
        /// <param name="z1">Z coordinate of the first endpoint.</param>
        /// <param name="x2">X coordinate of the second endpoint.</param>
        /// <param name="y2">Y coordinate of the second endpoint.</param>
        /// <param name="z2">Z coordinate of the second endpoint.</param>
        public void AddLine(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            AddLineCell(x1, y1, z1, x2, y2, z2);
        }

        public void AddLine(vec3 p1, vec3 p2)
        {
            AddLine(p1.x, p1.y, p1.z, p2.x, p2.y, p2.z);
        }

        public void AddLines(params vec3[] points)
        {
            lock (Lock)
            {
                if (points!=null)
                    for (int i = 0; i < points.Length-1; ++i)
                        AddLine(points[i], points[i+1]);
            }
        }

        public void AddTriangle(double x1, double y1, double z1, double x2, double y2, double z2,
            double x3, double y3, double z3)
        {
            AddLine(x1, y1, z1, x2, y2, z2);
            AddLine(x2, y2, z2, x3, y3, z3);
            AddLine(x3, y3, z3, x1, y1, z1);
        }


        public void AddTriangle(vec3 p1, vec3 p2, vec3 p3)
        {
            AddTriangle(p1.x, p1.y, p1.z, p2.x, p2.y, p2.z, p3.x, p3.y, p3.z);
        }

        public void AddTriangularStrip(params vec3[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length - 2; ++i)
                {
                    AddTriangle(points[i], points[i + 1], points[i + 2]);
                }
            }
        }

        public void AddQuadrilateral(double x1, double y1, double z1, double x2, double y2, double z2,
            double x3, double y3, double z3, double x4, double y4, double z4)
        {
            AddLine(x1, y1, z1, x2, y2, z2);
            AddLine(x2, y2, z2, x3, y3, z3);
            AddLine(x3, y3, z3, x4, y4, z4);
            AddLine(x4, y4, z4, x1, y1, z1);
        }

        public void AddQuadrilateral(vec3 p1, vec3 p2, vec3 p3, vec3 p4)
        {
            AddQuadrilateral(p1.x, p1.y, p1.z, p2.x, p2.y, p2.z, p3.x, p3.y, p3.z, p4.x, p4.y, p4.z);
        }


        public void AddFilledTriangle(double x1, double y1, double z1, double x2, double y2, double z2,
            double x3, double y3, double z3)
        {
            AddSurfaceCell(x1, y1, z1, x2, y2, z2, x3, y3, z3);
        }

        public void AddFilledTriangle(vec3 p1, vec3 p2, vec3 p3)
        {
            AddFilledTriangle(p1.x, p1.y, p1.z, p2.x, p2.y, p2.z, p3.x, p3.y, p3.z);
        }

        public void AddFilledTriangularStrip(params vec3[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length - 2; ++i)
                {
                    AddFilledTriangle(points[i], points[i + 1], points[i + 2]);
                }
            }
        }

        public void AddBorderedTriangle(double x1, double y1, double z1, double x2, double y2, double z2,
            double x3, double y3, double z3)
        {
            AddFilledTriangle(x1, y1, z1, x2, y2, z2, x3, y3, z3);
            AddTriangle(x1, y1, z1, x2, y2, z2, x3, y3, z3);
        }

        public void AddBorderedTriangle(vec3 p1, vec3 p2, vec3 p3)
        {
            AddFilledTriangle(p1, p2, p3);
            AddTriangle(p1, p2, p3);
        }

        public void AddBorderedTriangularStrip(params vec3[] points)
        {
            AddFilledTriangularStrip(points);
            AddTriangularStrip(points);
        }



        public void AddFilledQuadrilateral(double x1, double y1, double z1, double x2, double y2, double z2,
            double x3, double y3, double z3, double x4, double y4, double z4)
        {
            AddSurfaceCell(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4);
        }

        public void AddFilledQuadrilateral(vec3 p1, vec3 p2, vec3 p3, vec3 p4)
        {
            AddFilledQuadrilateral(p1.x, p1.y, p1.z, p2.x, p2.y, p2.z, p3.x, p3.y, p3.z, p4.x, p4.y, p4.z);
        }

        public void AddBorderedQuadrilateral(double x1, double y1, double z1, double x2, double y2, double z2,
            double x3, double y3, double z3, double x4, double y4, double z4)
        {
            AddFilledQuadrilateral(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4);
            AddQuadrilateral(x1, y1, z1, x2, y2, z2, x3, y3, z3, x4, y4, z4);
        }

        public void AddBorderedQuadrilateral(vec3 p1, vec3 p2, vec3 p3, vec3 p4)
        {
            AddFilledQuadrilateral(p1, p2, p3, p4);
            AddQuadrilateral(p1, p2, p3, p4);
        }



        #endregion CustomPlots


    } // class VtkSurfacePlotBase
}