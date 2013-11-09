// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;
using IG.Num;

using ZedGraph;

using Color = System.Drawing.Color;
using DashStyle = System.Drawing.Drawing2D.DashStyle;

namespace IG.Gr
{

    /// <summary>Curve plots thst is shown on a <see cref="ZedGraphControl"/> object.</summary>
    /// <remarks>
    /// <para>Line and point settings can be specified such that look of the curve plotted is finely adjusted.</para>
    /// <para>Symbol to draw points can be specified as <see cref="SymbolType"/> enum or as integer, which is useful in 
    /// automatical settings when drawing multiple curves.</para></remarks>
    /// $A Igor Jun09 Nov11;
    public class PlotZedgraphCurve : PlotZedGraphBase, ILockable, IDisposable
    {


        /// <summary>Creates a curve plot.</summary>
        /// <param name="plotter">Plotter used for displaying the plot.</param>
        public PlotZedgraphCurve(PlotterZedGraph plotter): base(plotter)
        {
        }

        #region Data

        // FUNCTONS:

        protected IRealFunction _functionX, _functionY;

        /// <summary>The first component of a 2D function of 1 parameter that acts as parametric
        /// definition of the plotted curve.</summary>
        public IRealFunction FunctionX
        {
            get { return _functionX; }
            protected set
            {
                _functionX = value;
                if (_functionX != null && _functionY != null) ClearPoints();
            }
        }

        /// <summary>The second component of a 2D function of 1 parameter that acts as parametric
        /// definition of the plotted curve.</summary>
        public IRealFunction FunctionY
        {
            get { return _functionY; }
            protected set
            {
                _functionY = value;
                if (_functionX != null && _functionY != null) ClearPoints();
            }
        }

        /// <summary>The first component of a 2D function of 1 parameter that acts as parametric
        /// definition of the plotted curve.</summary>
        protected IRealFunction Function
        {
            set
            {
                FunctionX = IdentityFunction;
                FunctionY = value;
            }
        }

        private IRealFunction _identity;

        /// <summary>Gets identity function.</summary>
        protected IRealFunction IdentityFunction
        {
            get { lock (Lock) { if (_identity == null) _identity = Func.GetIdentity(); return _identity; } }
        }

        /// <summary>Sets definition of the parametric curve in 2D to be plotted.</summary>
        /// <param name="funcX">Function of parameter that defines X coordinates of points.</param>
        /// <param name="funcY">Function of parameter that defines Y coordinates of points.</param>
        public void SetCurveDefiniton(IRealFunction funcX, IRealFunction funcY)
        {
            FunctionX = funcX;
            FunctionY = funcY;
        }

        /// <summary>Sets explicit definition of curve in 2D to be plotted.</summary>
        /// <param name="func">Function that defines how Y coordinate depends on X coordinate of points.</param>
        public void SetCurveDefiniton(IRealFunction func)
        {
            FunctionX = IdentityFunction;
            FunctionY = func;
        }

        /// <summary>Removes any eventual definition of the plotted curve curve by functions (either parametric or explicit).</summary>
        public void ClearCurveDefinition()
        {
            FunctionX = null;
            FunctionY = null;
        }

        // Data for plotting functions

        /// <summary>Default number of points when plotting curves specified by functions.</summary>
        public static int DefaultNumPoints = 100;

        protected int _numPoints = DefaultNumPoints;

        /// <summary>Number of points in curve plots. Used when plotting curves specified by functions.</summary>
        public int NumPoints
        {
            get { lock (Lock) { return _numPoints; } }
            set { lock (Lock) { _numPoints = value; } }
        }

        /// <summary>Default lower bound on parameter.</summary>
        public static double DefaultMinParam = 0;

        /// <summary>Default upper bound on parameter.</summary>
        public static double DefaultMaxParam = 1;

        protected double _minParam = DefaultMinParam;

        protected double _maxParam = DefaultMaxParam;

        /// <summary>Lower bound on parameter when plotting a curve.</summary>
        public double MinParam
        {
            get { lock (Lock) { return _minParam; } }
            set { lock (Lock) { _minParam = value; } }
        }

        /// <summary>Upper bound on parameter when plotting a curve.</summary>
        public double MaxParam
        {
            get { lock (Lock) { return _maxParam; } }
            set { lock (Lock) { _maxParam = value; } }
        }


        /// <summary>Recalculates bounds for coordinates on the graphic objects currently contained in the graph.</summary>
        public override void CalculateBoundsCoordinates()
        {
            lock (Lock)
            {
                BoundsCoordinates.Reset();
                for (int i = 0; i < _points.Count; ++i)
                {
                    vec2 pt = _points[i];
                    BoundsCoordinates.Update(pt);
                }
            }
        }


        protected List<vec2> _points;

        /// <summary>List of points that define the curve to be plotted.</summary>
        protected List<vec2> Points
        {
            get
            {
                lock (Lock)
                {
                    if (_points == null)
                    {
                        _points = new List<vec2>();
                    }
                    return _points;
                }
            }
            set
            {
                lock (Lock)
                {
                    if (value!=_points)
                    {
                        _points = value;
                        if (AutoUpdateBoundsCoordinates)
                            CalculateBoundsCoordinates();
                    }
                }
            }
        }

        /// <summary>Clears the list of points that define the curve to be plotted.</summary>
        public void ClearPoints()
        {
            lock (Lock)
            {
                if (_points != null)
                    _points.Clear();
                //if (AutoUpdateBoundsCoordinates)
                    BoundsCoordinates.Reset();
            }
        }

        /// <summary>Adds a point with specified coordinates to the curve plotted on the current curve graph.</summary>
        /// <param name="xCoordinate">X coordinate of the added point.</param>
        /// <param name="yCoordinate">Y coordinate of the added point.</param>
        public void AddPoint(double xCoordinate, double yCoordinate)
        {
            AddPoint(new vec2(xCoordinate, yCoordinate));  // bouds updated in called function
        }

        /// <summary>Adds the specified point to the curve plotted on the current curve graph.</summary>
        /// <param name="point">Point to be added.</param>
        public void AddPoint(vec2 point)
        {
            lock (Lock)
            {
                Points.Add(point);
                if (AutoUpdateBoundsCoordinates)
                {
                    BoundsCoordinates.Update(point);
                }
            }
        }

        /// <summary>Adds the specified set of points to the curve plotted on the curve graph.</summary>
        /// <param name="points">Points to be added to the curve definition.</param>
        public void AddPoints(params vec2[] points)
        {
            lock (Lock)
            {
                if (points != null)
                {
                    for (int i = 0; i < points.Length; ++i)
                    {
                        AddPoint(points[i]);  // bouds updated in called function
                    }
                }
            }
        }

        /// <summary>Sets points of the curve plotted on the curve graph.</summary>
        /// <param name="points">Points to be added to the curve definition.
        /// <para>If any of the the elements of the array is null then it is ignored.</para></param>
        public void SetPoints(params vec2[] points)
        {
            lock (Lock)
            {
                ClearPoints();
                AddPoints(points);  // bouds updated in called function
            }
        }

        /// <summary>Adds the specified set of points to the curve plotted on the curve graph.</summary>
        /// <param name="points">Points to be added to the curve definition.
        /// <para>If any of the the elements of the array is null then it is ignored.</para></param>
        public void AddPoints(params Vector2d[] points)
        {
            lock (Lock)
            {
                if (points != null)
                {
                    for (int i = 0; i < points.Length; ++i)
                    {
                        Vector2d point = points[i];
                        if (point!=null)
                            AddPoint(points[i].Vec);  // bouds updated in called function
                    }
                }
            }
        }

        /// <summary>Sets points of the curve plotted on the curve graph.</summary>
        /// <param name="points">Points to be added to the curve definition.
        /// <para>If any of the the elements of the array is null then it is ignored.</para></param>
        public void SetPoints(params Vector2d[] points)
        {
            lock (Lock)
            {
                ClearPoints();
                AddPoints(points);  // bouds updated in called function
            }
        }

        /// <summary>Adds the specified set of points to the curve plotted on the curve graph.</summary>
        /// <param name="points">Points to be added to the curve definition.
        /// <para>If any of the the elements of the array is null then it is ignored.
        /// Otherwise, vectors representing points must be of dimension 2.</para></param>
        public void AddPoints(params IVector[] points)
        {
            lock (Lock)
            {
                if (points != null)
                {
                    for (int i = 0; i < points.Length; ++i)
                    {
                        IVector point = points[i];
                        if (point != null)
                        {
                            if (point.Length != 2)
                            {
                                throw new ArgumentException("Vector of point coordinates No. " + i + " is not of dimenson 2 but of dimension " + point.Length + ".");
                            }
                            AddPoint(new vec2(point[0], point[1]));  // bouds updated in called function
                        }
                    }
                }
            }
        }

        /// <summary>Sets points of the curve plotted on the curve graph.</summary>
        /// <param name="points">Points to be added to the curve definition.
        /// <para>If any of the the elements of the array is null then it is ignored.
        /// Otherwise, vectors representing points must be of dimension 2.</para></param>
        public void SetPoints(params IVector[] points)
        {
            lock (Lock)
            {
                ClearPoints();
                AddPoints(points);  // bouds updated in called function
            }
        }

        /// <summary>Adds the specified set of points to the curve plotted on the curve graph.</summary>
        /// <param name="xArray">Array of X coordinates of the points that define the curve.</param>
        /// <param name="yArray">Array of Y coordinates of the points that define the curve.</param>
        public void AddPoints(double[] xArray, double[] yArray)
        {
            lock (Lock)
            {
                if (xArray == null && yArray == null)
                {
                    Points=null;
                    return;
                } else
                {
                    if (xArray == null)
                        throw new ArgumentException("Array of X co-ordinates is not specified (null argument).");
                    if (xArray == null)
                        throw new ArgumentException("Array of Y co-ordinates is not specified (null argument).");
                    if (xArray.Length != yArray.Length)
                        throw new ArgumentException("Number of X coordinates (" + xArray.Length 
                            + ") is different than number ov Y coordinates (" + yArray.Length + ").");
                }
                List<vec2> points = this.Points;
                points.Clear();
                for (int i = 0; i < xArray.Length; ++i)
                {
                    vec2 point = new vec2(xArray[i], yArray[i]);
                    AddPoint(new vec2(xArray[i], yArray[i]));  // bouds updated in called function
                }

            }
        }
        
        /// <summary>Sets points of the curve plotted on the curve graph.</summary>
        /// <param name="xArray">Array of X coordinates of the points that define the curve.</param>
        /// <param name="yArray">Array of Y coordinates of the points that define the curve.</param>
        public void SetPoints(double[] xArray, double[] yArray)
        {
            lock (Lock)
            {
                ClearPoints();
                AddPoints(xArray, yArray);  // bouds updated in called function
            }
        }


        protected PointPairList _pointList = null;

        /// <summary>List of points used for ZedGraph representation of the curve.</summary>
        protected PointPairList PointList
        {
            get 
            {
                lock (Lock)
                {
                    if (_pointList == null)
                    {
                        _pointList = new PointPairList();
                    }
                    return _pointList;
                }
            }
            set
            {
                _pointList = value;
            }
        }

        #endregion Data


        #region Settings

        /// <summary>Default value of the flag specifying whether lines are shown or not.</summary>
        public static bool DefaultShowLines = true;

        public bool _linesVisible = DefaultShowLines;

        /// <summary>Specifies whether lines will be shown for the current curve or not.</summary>
        public bool LinesVisible
        {
            get { lock (Lock) { return _linesVisible; } }
            set { lock (Lock) { _linesVisible = value; } }
        }

        /// <summary>Default value of the flag specifying whether points are shown or not.</summary>
        public static bool DefaultShowPoints = true;

        public bool _pointsVisible = DefaultShowPoints;

        /// <summary>Specifies whether points will be shown for the current curve or not.</summary>
        public bool PointsVisible
        {
            get { lock (Lock) { return _pointsVisible; } }
            set { lock (Lock) { _pointsVisible = value; } }
        }

        /// <summary>Collectively sets the common color for line, point border and point fill.</summary>
        public color ColorAll
        {
            set
            {
                lock (Lock)
                {
                    LineColor = value; 
                    PointFillColor = value; 
                    PointBorderColor = value;
                }
            }
        }


        // SETTINGS FOR DRAWING LINES:

        /// <summary>Default color for curves.</summary>
        public static color DefaultLineColor = Color.Blue;

        protected color _lineColor = DefaultLineColor;

        /// <summary>Line color.</summary>
        public color LineColor
        {
            get { lock (Lock) { return _lineColor; } }
            set { lock (Lock) { _lineColor = value; } }
        }


        /// <summary>Default line width.</summary>
        public static double DefaultLineWidth = 2.0;

        protected double _lineWidth = DefaultLineWidth;

        /// <summary>Specifies the line width.</summary>
        public double LineWidth
        {
            get { lock (Lock) { return _lineWidth; } }
            set { lock (Lock) { _lineWidth = value; } }
        }

        /// <summary>Default line dash style.</summary>
        public static DashStyle DefaultLineDashStyle = DashStyle.Solid;

        protected DashStyle _lineDashStyle = DefaultLineDashStyle;

        /// <summary>Specifies the line dash style.</summary>
        public DashStyle LineDashStyle
        {
            get { lock (Lock) { return _lineDashStyle; } }
            set { lock (Lock) { _lineDashStyle = value; } }
        }

        /// <summary>Default line step type.</summary>
        public static StepType DefaultLineStepType = StepType.NonStep;

        protected StepType _lineStepType = DefaultLineStepType;

        /// <summary>Specifies the line step type in form of the Yedgraph's <see cref="StepType"/> enumerator.
        /// If different that <see cref="StepType.NonStep"/> then horizontal steps are plotted between values.</summary>
        public StepType LineStepType
        {
            get { lock (Lock) { return _lineStepType; } }
            set { lock (Lock) { _lineStepType = value; } }
        }


        /// <summary>Default smoothing flag.</summary>
        public static bool DefaultLineSmoothing = false;

        protected bool _lineSmoothing = DefaultLineSmoothing;

        /// <summary>Specifies whether lines are smoothed or not when drawing the curve.</summary>
        public bool LineSmoothing
        {
            get { lock (Lock) { return _lineSmoothing; } }
            set { lock (Lock) { _lineSmoothing = value; } }
        }

        /// <summary>Default level of smoothing when drawing lines.</summary>
        public static double DefaultLineSmoothness = 1.0;

        protected double _lineSmoothness = DefaultLineSmoothness;

        /// <summary>Specifies the level of smoothing when drawing the curve.
        /// <para>Must be between 0 (not smoothed) and 1 (fully smoothed).</para>
        /// <para>When set to a value greater than 0, the <see cref="LineSmoothing"/> flag is set to true.</para></summary>
        public double LineSmoothness
        {
            get { lock (Lock) { return _lineSmoothness; } }
            set { lock (Lock) { _lineSmoothness = value; if (value > 0) LineSmoothing = true; } }
        }


        // SETTINGS FOR DRAWING POINTS:


        /// <summary>Default point size.</summary>
        public static double DefaultPointSize = 4.0;

        protected double _pointSize = DefaultPointSize;

        /// <summary>Type of symbol used for drawing points, specified as int.</summary>
        public double PointSize
        {
            get { lock (Lock) { return _pointSize; } }
            set { lock (Lock) { _pointSize = value; } }
        }

        /// <summary>Default point type.</summary>
        public static int DefaultPointType = GetPointType(SymbolType.Circle);

        protected int _pointType = DefaultPointType;

        /// <summary>Type of symbol used for drawing points, specified as int.</summary>
        public int PointType
        {
            get { lock (Lock) { return _pointType; } }
            set { lock (Lock) { _pointType = value; } }
        }

        /// <summary>Type of symbol used for drawing points, specified as <see cref="SymbolType"/> enum.</summary>
        public SymbolType PointTypeSymbol
        {
            get { lock (Lock) { return GetSymbolType(PointType); } }
            set { lock (Lock) { _pointType = GetPointType(value); } }
        }

        /// <summary>Converts integer to <see cref="SymbolType"/> enum.</summary>
        /// <param name="typeId">Integer to be converted.</param>
        public static SymbolType GetSymbolType(int typeId)
        {
            if (typeId<0)
                return SymbolType.None;
            else if (typeId >= (int) SymbolType.None)
                return SymbolType.Default;
            else
                return (SymbolType) typeId;
        }

        /// <summary>Converts <see cref="SymbolType"/> enum to integer.</summary>
        /// <param name="typeId">Enumumerator value to be converted.</param>
        public static int GetPointType(SymbolType pointType)
        {
            if (pointType == SymbolType.None)
                return -1;
            return (int) pointType;
        }

        /// <summary>Collectively sets point border color (<see cref="PointBorderColor"/>) and point fill color (<see cref="PointFillColor"/>) 
        /// to the same value.</summary>
        public color PointColor
        {
            set { lock (Lock) { PointBorderColor = value;  PointFillColor = value; } }
        }

        /// <summary>Default color for point borders.</summary>
        public static color DefaultPointBorderColor = DefaultLineColor;

        protected color _pointBorderColor = DefaultPointBorderColor;

        /// <summary>Color for plotting points outline (border).</summary>
        public color PointBorderColor
        {
            get { lock (Lock) { return _pointBorderColor; } }
            set { lock (Lock) { _pointBorderColor = value; } }
        }

        /// <summary>Default point fill color.</summary>
        public static color DefaultPointFillColor = DefaultLineColor;

        protected color _pointFillColor = DefaultPointFillColor;

        /// <summary>Points fill color.</summary>
        public color PointFillColor
        {
            get { lock (Lock) { return _pointFillColor; } }
            set { lock (Lock) { _pointFillColor = value; } }
        }



        protected bool _isX2Axis = false;

        public bool IsX2Axis
        {
            get { lock (Lock) { return _isX2Axis; } }
            set { lock (Lock) { _isX2Axis = value; } }
        }

        protected bool _isY2Axis = false;

        public bool IsY2Axis
        {
            get { lock (Lock) { return _isY2Axis; } }
            set { lock (Lock) { _isY2Axis = value; } }
        }

        public bool Is2Axis
        {
            get { lock (Lock) { return IsX2Axis && IsY2Axis; } }
            set { lock (Lock) { IsX2Axis = value; IsY2Axis = value; } }
        }



        #endregion Settings


        #region Operation

        /// <summary>Creates data for the plot. Basically, this creates and updates the internal data
        /// structures used by the plot, while <see cref="Update"/> will also update the plot 
        /// in the window where it is shown.</summary>
        public override void CreateData()
        {
            if (FunctionX != null && FunctionY != null && NumPoints > 0)
            {
                double step = 0;
                if (NumPoints > 1)
                    step = (MaxParam - MinParam) / (double) (NumPoints-1);
                double t;  // parameter
                for (int i = 0; i < NumPoints; ++i)
                {
                    t = MinParam + (double)i * step;
                    AddPoint(FunctionX.Value(t), FunctionY.Value(t));
                }
            }
        }



        protected LineItem _curveItem;


        /// <summary>LineItem that is put to the GraphPane. Object can be removed from the pane through this
        /// property.</summary>
        public LineItem CurveItem
        {
            get { lock (Lock) { return _curveItem; } }
            protected set
            {
                lock (Lock)
                {
                    if (value != _curveItem)
                    {
                        if (_curveItem != null)
                        {
                            if (GraphPane != null)
                                GraphPane.CurveList.Remove(_curveItem);
                        }
                        _curveItem = value;
                    }
                }
            }
        }

        /// <summary>Updates the plot on the plotter (and consequently the related window of type <see cref="ZedGraphControl"/>).</summary>
        public override void Update()
        {
            lock (Lock)
            {
                base.Update();  // this calls the Create() method.

                // Get a reference to the GraphPane instance in the ZedGraphControl
                // GraphPane = Plotter.Window.GraphPane;

                // Make the list of points that define the plotted curve:
                PointPairList PointList1 = new PointPairList();

                // Copy point data to the list:
                for (int i = 0; i < Points.Count; ++i)
                {
                    vec2 point = Points[i];
                    PointList1.Add(point.x, point.y);
                }


                // Generate the curve with specified properties, with the specified legend string:
                LineItem myCurve = GraphPane.AddCurve(LegendString,
                    PointList1, LineColor, GetSymbolType(PointType));
                CurveItem = myCurve;

                myCurve.IsX2Axis = IsX2Axis;  // whether the second X axis is associated with the curve
                myCurve.IsY2Axis = IsY2Axis;  // whether the second Y axis is associated with the curve

                myCurve.IsVisible = true;  // $$
                myCurve.IsSelectable = true;  // $$ c

                myCurve.Line.IsAntiAlias = true;
                myCurve.Line.IsVisible = LinesVisible;
                myCurve.Line.Color = LineColor;
                myCurve.Line.Width = (float) LineWidth;
                myCurve.Line.Style = LineDashStyle;
                myCurve.Line.IsSmooth = LineSmoothing;
                myCurve.Line.SmoothTension = (float) LineSmoothness; 
                myCurve.Line.StepType = LineStepType;

                myCurve.Symbol.IsAntiAlias = true;
                myCurve.Symbol.IsVisible = PointsVisible;
                myCurve.Symbol.Size = (float) PointSize;
                myCurve.Symbol.Type = PointTypeSymbol;
                myCurve.Symbol.Border.Color = PointBorderColor;
                myCurve.Symbol.Fill = new Fill(PointFillColor);
                myCurve.Symbol.Border.Style = System.Drawing.Drawing2D.DashStyle.Solid;

            }
        }


        /// <summary>Removes all items of the current plot from the graph pane.
        /// <para>Must be overridden in derived classes.</para></summary>
        public override void RemoveFromGraphPane()
        {
            // This will remove eventual LineItem associated with the current plot:
            CurveItem = null;
        }

        #endregion Operation

    }


}
