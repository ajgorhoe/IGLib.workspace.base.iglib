// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;

using ZedGraph;

using Color = System.Drawing.Color;

namespace IG.Gr
{

    /// <summary>Plotter class that uses a <see cref="ZedGraphControl"/> object for plotting ordinary 2D graphs.</summary>
    /// $A Igor Jun09;
    public class PlotterZedGraph : ILockable, IDisposable
    {

        #region Constructors

        private PlotterZedGraph() { }

        public PlotterZedGraph(ZedGraphControl plotWindow) 
        {
            this.Window = plotWindow;
        }

        #endregion Constructors


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking


        #region Data.Basic

                
        private ZedGraphControl _window;

        /// <summary>Zedgraph control used for plotting graphs.
        /// Warning: settes should only be used in constructors.</summary>
        public ZedGraphControl Window
        {
            get {
                lock (Lock)
                {
                    if (_window == null)
                    {
                        throw new NotImplementedException("Not yet implemented: can not handle stuations where YedGraph control is not specified.");
                    }
                    return _window;
                }
            }
            set {
                lock (Lock)
                {
                    if (value != _window)
                    {
                        _window = value;
                        // Invalidate dependencies:
                        GraphPane = null;
                        if (value != null)
                            GraphPane = value.GraphPane;
                    }
                }
            }
        }

        GraphPane _pane;

        /// <summary>Gets a reference to the GraphPane instance in the current ZedGraphControl.</summary>
        public GraphPane GraphPane
        {
            get {
                lock (Lock)
                {
                    if (_pane == null)
                    {
                        if (Window == null)
                            throw new InvalidOperationException("Can not get the graph pane, since Window is not specified (null reference).");
                        _pane = Window.GraphPane;
                    }
                    return _pane;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _pane = value;
                }
            }
        }

        #endregion Data.Basic
        

        #region Data.PlotObjects


        // BOUNDS:


        BoundingBox2d _boundsCoordinates;

        /// <summary>Bounds of the current plot.
        /// <para>If <see cref="AutoUpdateBoundsCoordinates"/> == true then the bounds are updated automatically 
        /// when new points are added.</para></summary>
        /// <remarks>These bounds are usually used just for information, but they can also be used to adjust
        /// the bounds on scales.</remarks>
        public BoundingBox2d BoundsCoordinates
        {
            get
            {
                lock (Lock)
                {
                    if (_boundsCoordinates == null)
                    {
                        _boundsCoordinates = new BoundingBox2d();
                    }
                    return _boundsCoordinates;
                }
            }
        }

        /// <summary>Recalculates bounds for coordinates on the graphic objects currently contained in the graph.
        /// <para>This recalculates, if necessary, the bounds of plots that are added on the current plotter
        /// (list <see cref="PlotObjects"/>).</para></summary>
        protected void CalculateBoundsCoordinates()
        {
            lock (Lock)
            {
                BoundsCoordinates.Reset();
                for (int i = 0; i < _plotObjects.Count; ++i)
                {
                    PlotZedGraphBase plt = _plotObjects[i];
                    if (!plt.AutoUpdateBoundsCoordinates)
                        plt.CalculateBoundsCoordinates();
                    BoundsCoordinates.Update(plt.BoundsCoordinates);
                }
            }
        }


        protected List<PlotZedGraphBase> _plotObjects;

        /// <summary>List of plotting objects contained on the current class.
        /// <para>Setter is thread safe.</para>
        /// <para>Lazy evaluation - list object is automatically generated when first accessed.</para></summary>
        protected List<PlotZedGraphBase> PlotObjects
        {
            get
            {
                if (_plotObjects == null)
                {
                    lock (Lock)
                    {
                        if (_plotObjects == null)
                            PlotObjects = new List<PlotZedGraphBase>();
                    }
                }
                return _plotObjects;
            }
            set { lock (Lock) { _plotObjects = value; } }
        }

        /// <summary>Returns true if the specified plotting object is contained on (registered with) the current 
        /// <see cref="PlotterZedGraph"/> object, or false otherwise.</summary>
        /// <param name="plotObject">Plotting object to be checked.</param>
        public bool ContainsPlotObject(PlotZedGraphBase plotObject)
        {
            if (plotObject == null)
                throw new ArgumentNullException("plotObject", "Plotting object not specified (null reference).");
            lock(Lock)
            {
                return PlotObjects.Contains(plotObject);
            }
        }

        /// <summary>Adds the specified plotting object to the list of plotting objects of the current
        /// plotter.
        /// <para>If the object is already on the list of plotting objects then it is not inserted again.</para></summary>
        /// <param name="plotObject">Zedgraph plotting object to be added on the currrent <see cref="PlotterZedGraph"/> object.</param>
        public void AddPlotObject(PlotZedGraphBase plotObject)
        {
            if (plotObject == null)
                throw new ArgumentNullException("plotObject", "Plotting object not specified (null reference).");
            lock (Lock)
            {
                if (!PlotObjects.Contains(plotObject))
                {
                    PlotObjects.Add(plotObject);
                }
            }
        }

        /// <summary>Adds the specified plotting objects to the list of plotting objects of the current
        /// plotter.</summary>
        /// <param name="plotObjects">Objects to be added to the list.</param>
        public void AddPlotObjects(params PlotZedGraphBase[] plotObjects)
        {
            if (plotObjects!=null)
                lock (Lock)
                {
                    for (int i = 0; i < plotObjects.Length; ++i)
                    {
                        AddPlotObject(plotObjects[i]);
                    }
                }
        }

        /// <summary>Removes all plotting objects from the list of plotting objects of the current polotter.</summary>
        public void RemoveAllPlotObjects()
        {
            lock (Lock)
            {
                RemovePlotObjects(PlotObjects.ToArray());
                //PlotZedGraphBase[] plObjects = PlotObjects.ToArray();
                //foreach (PlotZedGraphBase plotObject in plObjects)
                //{
                //    PlotObjects.Remove(plotObject);
                //}
            }
        }

        /// <summary>Removes the specified plotting object from the list of plotting objects of the current
        /// plotter, and disposes unmanaged resources used by that object.
        /// <para>If the specified object is not on the list of plotting objects then nothing happens.</para></summary>
        /// <param name="plotObject">Zedgraph plotting object to be removed from the currrent <see cref="PlotterZedGraph"/> object.</param>
        public void RemovePlotObject(PlotZedGraphBase plotObject)
        {
            if (plotObject == null)
                throw new ArgumentNullException("plotObject", "Plotting object not specified (null reference).");
            lock (Lock)
            {
                try
                {
                    if (PlotObjects.Contains(plotObject))
                    {
                        plotObject.RemoveFromGraphPane();
                        PlotObjects.Remove(plotObject);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    plotObject.Dispose();
                }
            }
        }


        /// <summary>Removes the specified plotting objects from the list of plotting objects of the current
        /// plotter, and disposes unmanaged resources used by that objects.
        /// <para>If no objects are specified then nothing happens. Also for the specified objects that are null
        /// or are not on the list, nothing happens. If removing one of the objects throws an exception then the 
        /// remaining objects are removed without any disturbance.</para></summary>
        /// <param name="plotObjects">Objects to be removed from the list.</param>
        public void RemovePlotObjects(params PlotZedGraphBase[] plotObjects)
        {
            int numErrors = 0;
            if (plotObjects != null)
            {
                lock (Lock)
                {
                    for (int i = 0; i < plotObjects.Length; ++i)
                    {
                        try
                        {
                            RemovePlotObject(plotObjects[i]);
                        }
                        catch (Exception)
                        {
                            ++numErrors;
                        }
                    }
                }
            }
            if (numErrors > 0)
                throw new Exception("The following number of exceptions occurred when removing plotting objects: " + numErrors + ".");
        }

        #endregion Data.PlotObjects
        

        #region Settings

        protected static int _defaultOutputLevel = -1;

        /// <summary>Gets or sets the default level of output for this class.
        /// <para>When accessed for the first time, the current value of <see cref="Util.OutputLevel"/> is returned.</para>
        /// <para>If set to less than 0 then the first subsequent set access will return the current the current value of <see cref="Util.OutputLevel"/>.</para></summary>
        public static int DefaultOutputLevel
        {
            get
            {
                if (_defaultOutputLevel < 0)
                    _defaultOutputLevel = Util.OutputLevel;
                return _defaultOutputLevel;
            }
            set { _defaultOutputLevel = value; }
        }

        private int _outputLevel = DefaultOutputLevel;

        /// <summary>Level of output to the console for the current object.</summary>
        public int OutputLevel
        { get { return _outputLevel; } set { _outputLevel = value; } }

        /// <summary>Default first background color for new windows.</summary>
        public static color DefaultBackgroundColor1 = new color(1, 1, 1);

        /// <summary>Default second background color for new windows.</summary>
        public static color DefaultBackgroundColor2 = new color(1, 1, 1);

        /// <summary>Background color for the current plotter.
        /// <para>Setter sets both colors of the gradient to the specified color, making the background color uniform.</para>
        /// <para>Getter returns the first color of the two colors that define background color gradient.</para></summary>
        public color Background
        {
            get { return _background1; }
            set
            {
                _background1 = value;
                _background2 = value;
                // TODO: update dependencies!
            }
        }

        color _background1 = DefaultBackgroundColor1;

        /// <summary>The first background color of possibly two colors that define color gradient 
        /// of the plotter window's background.</summary>
        public color Background1
        {
            get { return _background1; }
            set
            {
                _background1 = value;
                // TODO: update dependencies!
            }
        }
        
        color _background2 = DefaultBackgroundColor2;

        /// <summary>The second background color of possibly two colors that define color gradient 
        /// of the plotter window's background.</summary>
        public color Background2
        {
            get { return _background2; }
            set
            {
                _background2 = value;
                // TODO: update dependencies!
            }
        }
        
        /// <summary>Default angle of the background color gradient.</summary>
        public static double DefaultBackgroundGradientAngle = 45.0;

        double _backgroundGradientAngle = DefaultBackgroundGradientAngle;

        /// <summary>Angle of the background color gradient.</summary>
        public double BackgroundGradientAngle
        {
            get { return _backgroundGradientAngle; }
            set { _backgroundGradientAngle = value; }
        }

        bool _showPlotTip = false;

        bool ShowPlotTip
        {
            get { return _showPlotTip; }
            set { _showPlotTip = value; }
        }

        /// <summary>
        /// Default flag for showing point coordinates in tooltips.</summary>
        public static bool DefaultIsShowPointValues = true;

        protected bool _isShowPointValues = DefaultIsShowPointValues;

        /// <summary>Whether a tooltip with point coordinates is displayed when a mouse hovers over points.</summary>
        public bool IsShowPointValues
        {
            get { lock (Lock) { return _isShowPointValues; } }
            set { lock (Lock) { _isShowPointValues = value; } }
        }

        /// <summary>Default precision for point values displayed in tooltips.</summary>
        public int DefaultPointValuePrecision = 4;

        protected int _pointValuesPrecision = 4;

        /// <summary>Precision of point values that are displayed in tooltips.</summary>
        public int PointValuePrecision
        { 
            get { lock(Lock) { return _pointValuesPrecision; } }
            set { lock(Lock) { _pointValuesPrecision = value; } }
        } 



        #endregion Settings


        #region Settings.Decoration

        /// <summary>Sets graph bounds to the specified values.</summary>
        /// <param name="minX">Lower bound in X coordinate.</param>
        /// <param name="maxX">Upper bound in X coordinate.</param>
        /// <param name="minY">Lower bound in Y coordinate.</param>
        /// <param name="maxY">Upper bound in Y coordinate.</param>
        public void SetBounds(double minX, double maxX, double minY, double maxY)
        {
            XAxisScale.Min = minX;
            XAxisScale.Max = maxX;
            YAxisScale.Min = minY;
            YAxisScale.Max = maxY;
        }

        /// <summary>Sets graph bounds to the specified values.</summary>
        /// <param name="bounds">Bounds to which graph bounds are specified. Must be different than null and
        /// of dimension 2.</param>
        public void SetBounds(IBoundingBox bounds)
        {
            if (bounds == null)
                throw new ArgumentException("Bounds are not specified (null argument).");
            if (bounds.Dimension != 2)
                throw new ArgumentException("Dimension of bounds must be 2. Actual dimension: " + bounds.Dimension + ".");
            SetBounds(bounds.Min[0], bounds.Max[0], bounds.Min[1], bounds.Max[1]);
        }

        /// <summary>Sets graph bounds for the second axes to the specified values.</summary>
        /// <param name="minX">Lower bound in X coordinate.</param>
        /// <param name="maxX">Upper bound in X coordinate.</param>
        /// <param name="minY">Lower bound in Y coordinate.</param>
        /// <param name="maxY">Upper bound in Y coordinate.</param>
        public void SetBounds2(double minX, double maxX, double minY, double maxY)
        {
            X2AxisScale.Min = minX;
            X2AxisScale.Max = maxX;
            Y2AxisScale.Min = minY;
            Y2AxisScale.Max = maxY;
        }

        /// <summary>Sets graph bounds for the second axes to the specified values.</summary>
        /// <param name="bounds">Bounds to which graph bounds are specified. Must be different than null and
        /// of dimension 2.</param>
        public void SetBounds2(IBoundingBox bounds)
        {
            if (bounds == null)
                throw new ArgumentException("Bounds are not specified (null argument).");
            if (bounds.Dimension != 2)
                throw new ArgumentException("Dimension of bounds must be 2. Actual dimension: " + bounds.Dimension + ".");
            SetBounds2(bounds.Min[0], bounds.Max[0], bounds.Min[1], bounds.Max[1]);
        }

        /// <summary>Default graph title used for new plotter objects when not specified explicitly.</summary>
        public static string DefaultTitle = "Dependency";

        protected string _title = DefaultTitle;

        /// <summary>Graph title.</summary>
        public virtual string Title
        {
            get { lock (Lock) { return _title; } }
            set { lock (Lock) { _title = value; } }
        }

        /// <summary>Default font for graph title.</summary>
        public static FontSpec DefaultTitleFont = new FontSpec("Times New Roman", 16 /* size */, 
            Color.Blue, true /* bold */, true /* italic */, false /* underline */);

        /// <summary>Default font for axis labels.</summary>
        public static FontSpec DefaultAxisLabelFont = new FontSpec("Courier New", 12 /* size */,
            Color.Blue, true /* bold */, false /* italic */, false /* underline */);

        public static FontSpec DefaultAxisScaleFont = new FontSpec("Courier New", 10 /* size */,
            Color.Black, false /* bold */, false /* italic */, false /* underline */);

        /// <summary>Whether scale is visible by default.</summary>
        public static bool DefaultScaleIsVisible = true;

        /// <summary>Whether the second axes are visible by default.</summary>
        public static bool DefaultAxes2IsVisible = false;

        /// <summary>Whether scale on the second axes is visible by default.</summary>
        public static bool DefaultScale2IsVisible = false;

        /// <summary>Default value of the flag specifying whether zero lines are shown.</summary>
        public static bool DefaultZeroLine = true;

        /// <summary>Default value of the flag indicating whether major grid lines are shown.</summary>
        public static bool DefaultMajorGridIsVisible = true;

        /// <summary>Default value of the flag indicating whether minor grid lines are shown.</summary>
        public static bool DefaultMinorGridIsVisible = false;

        /// <summary>Default value of the flag indicating whether major grid lines for the second axes are shown.</summary>
        public static bool DefaultMajorGrid2IsVisible = true;

        /// <summary>Default value of the flag indicating whether minor grid lines for the second axes are shown.</summary>
        public static bool DefaultMinorGrid2IsVisible = false;


        // GRAPH TITLE:

        protected FontSpec _titleFont;

        /// <summary>Font used for graph title.</summary>
        public virtual FontSpec TitleFont
        {
            get
            {
                lock (Lock)
                {
                    if (_titleFont == null)
                    {
                        if (_pane != null)
                        {
                            _titleFont = new FontSpec(GraphPane.Title.FontSpec);
                            _titleFont.Border.IsVisible = false;
                        } else
                            _titleFont = DefaultTitleFont;
                    }
                    return _titleFont;
                }
            }
            set { lock (Lock) { _titleFont = value; } }
        }

        // X AXIS:

        bool _xAxisIsVisible = true;

        /// <summary>Whether X axis is visible.</summary>
        bool XAxisIsVisible
        {
            get { lock (Lock) { return _xAxisIsVisible; } }
            set { lock (Lock) { _xAxisIsVisible = value; if (value == true) XAxisScaleIsVisible = true; } }
        }

        /// <summary>Gets the X axis of the graph.</summary>
        public virtual XAxis XAxis
        {
            get { return GraphPane.XAxis; }
        }

        /// <summary>Default label to for X axis for new plot objects.</summary>
        public static string DefaultXAxisLabel = "X";

        protected string _xAxisLabel = DefaultXAxisLabel;

        /// <summary>Label for X axis.</summary>
        public string XAxisLabel
        {
            get { lock (Lock) { return _xAxisLabel; } }
            set { lock (Lock) { _xAxisLabel = value; } }
        }

        protected FontSpec _xAxisLabelFont;

        /// <summary>Font used for X axis label.</summary>
        public virtual FontSpec XAxisLabelFont
        {
            get
            {
                lock (Lock)
                {
                    if (_xAxisLabelFont == null)
                    {
                        if (_pane != null)
                            _xAxisLabelFont = new FontSpec(GraphPane.XAxis.Title.FontSpec);
                        else
                            _xAxisLabelFont = DefaultAxisLabelFont;
                    }
                    return _xAxisLabelFont;
                }
            }
            set { lock (Lock) { _xAxisLabelFont = value; } }
        }

        protected bool _xAxisIsZeroLine = DefaultZeroLine;

        /// <summary>Whether zero line on X axis is plotted or not.</summary>
        bool XAxisIsZeroLine
        {
            get { lock (Lock) { return _xAxisIsZeroLine; } }
            set { lock (Lock) { _xAxisIsZeroLine = value; } }
        }

        bool _xAxisScaleIsVisible = DefaultScaleIsVisible;

        /// <summary>Whether scale is shown for X axis.</summary>
        bool XAxisScaleIsVisible
        {
            get { lock (Lock) { return _xAxisScaleIsVisible; } }
            set { lock (Lock) { _xAxisScaleIsVisible = value; } }
        }

        /// <summary>Scale for X axis.</summary>
        Scale XAxisScale
        {
            get { lock (Lock) { return XAxis.Scale; } }
        }

        protected FontSpec _xAxisScaleFont;

        /// <summary>Settings for font on X scale.</summary>
        public FontSpec XAxisScaleFont
        {
            get
            {
                lock (Lock)
                {
                    if (_xAxisScaleFont == null)
                    {
                        if (_pane != null)
                            _xAxisScaleFont = new FontSpec(GraphPane.XAxis.Scale.FontSpec);
                        else
                            _xAxisScaleFont = DefaultAxisScaleFont;
                    }
                    return _xAxisScaleFont;
                }
            }
            set { lock (Lock) { _xAxisScaleFont = value; } }
        }

        protected bool _xAxisMajorTicAccessed = false;

        /// <summary>Properties for major tixs on X axis.</summary>
        public MajorTic XAxisMajorTic
        {
            get
            {
                lock (Lock)
                {
                    MajorTic ret = XAxis.MajorTic;
                    if (!_xAxisMajorTicAccessed)
                    {
                        _xAxisMajorTicAccessed = true;
                        ret.IsOpposite = false;
                        ret.IsInside = true;
                        ret.IsOutside = false;
                    }
                    return ret;
                }
            }
        }

        protected bool _xAxisMinorTicAccessed = false;

        /// <summary>Properties for minor tixs on X axis.</summary>
        public MinorTic XAxisMinorTic
        {
            get
            {
                lock (Lock)
                {
                    MinorTic ret = XAxis.MinorTic;
                    if (!_xAxisMinorTicAccessed)
                    {
                        _xAxisMinorTicAccessed = true;
                        ret.IsOpposite = false;
                        ret.IsInside = true;
                        ret.IsOutside = false;
                    }
                    return ret;
                }
            }
        }

        bool _xAxisMajorGridIsVisible = DefaultMajorGridIsVisible;

        /// <summary>Whether major grid lines for X axis are shown.</summary>
        bool XAxisMajorGridIsVisible
        {
            get { lock (Lock) { return _xAxisMajorGridIsVisible; } }
            set { lock (Lock) { _xAxisMajorGridIsVisible = value; } }
        }

        /// <summary>Major grid lines properties for X axis.</summary>
        MajorGrid XAxisMajorGrid
        { get { return XAxis.MajorGrid; } }

        bool _xAxisMinorGridIsVisible = DefaultMinorGridIsVisible;

        /// <summary>Whether minor grid lines for X axis are shown.</summary>
        bool XAxisMinorGridIsVisible
        {
            get { lock (Lock) { return _xAxisMinorGridIsVisible; } }
            set { lock (Lock) { _xAxisMinorGridIsVisible = value; } }
        }

        /// <summary>Minor grid lines properties for X axis.</summary>
        MinorGrid XAxisMinorGrid
        { get { return XAxis.MinorGrid; } }


        // Y AXIS:

        bool _yAxisIsVisible = true;

        /// <summary>Whether Y axis is visible.</summary>
        bool YAxisIsVisible
        {
            get { lock (Lock) { return _yAxisIsVisible; } }
            set { lock (Lock) { _yAxisIsVisible = value; if (value == true) YAxisScaleIsVisible = true; } }
        }

        /// <summary>Gets the Y axis of the graph.</summary>
        public virtual YAxis YAxis
        {
            get { return GraphPane.YAxis; }
        }

        /// <summary>Default label for Y axis for new plot objects.</summary>
        public static string DefaultYAxisLabel = "Y";

        protected string _yAxisLabel = DefaultYAxisLabel;

        /// <summary>Label for Y axis.</summary>
        public string YAxisLabel
        {
            get { lock (Lock) { return _yAxisLabel; } }
            set { lock (Lock) { _yAxisLabel = value; } }
        }

        protected FontSpec _yAxisLabelFont;

        /// <summary>Font used for Y axis label.</summary>
        public virtual FontSpec YAxisLabelFont
        {
            get
            {
                lock (Lock)
                {
                    if (_yAxisLabelFont == null)
                    {
                        if (_pane != null)
                            _yAxisLabelFont = new FontSpec(GraphPane.YAxis.Title.FontSpec);
                        else
                            _yAxisLabelFont = DefaultAxisLabelFont;
                    }
                    return _yAxisLabelFont;
                }
            }
            set { lock (Lock) { _yAxisLabelFont = value; } }
        }

        protected bool _yAxisIsZeroLine = DefaultZeroLine;

        /// <summary>Whether zero line on Y axis is plotted or not.</summary>
        bool YAxisIsZeroLine
        {
            get { lock (Lock) { return _yAxisIsZeroLine; } }
            set { lock (Lock) { _yAxisIsZeroLine = value; } }
        }

        bool _yAxisScaleIsVisible = DefaultScaleIsVisible;

        /// <summary>Whether scale is shown for Y axis.</summary>
        bool YAxisScaleIsVisible
        {
            get { lock (Lock) { return _yAxisScaleIsVisible; } }
            set { lock (Lock) { _yAxisScaleIsVisible = value; } }
        }

        /// <summary>Scale for Y axis.</summary>
        Scale YAxisScale
        {
            get { lock (Lock) { return YAxis.Scale; } }
        }

        protected FontSpec _yAxisScaleFont;

        /// <summary>Settings for font on Y scale.</summary>
        public FontSpec YAxisScaleFont
        {
            get
            {
                lock (Lock)
                {
                    if (_yAxisScaleFont == null)
                    {
                        if (_pane != null)
                            _yAxisScaleFont = new FontSpec(GraphPane.YAxis.Scale.FontSpec);
                        else
                            _yAxisScaleFont = DefaultAxisScaleFont;
                    }
                    return _yAxisScaleFont;
                }
            }
            set { lock (Lock) { _yAxisScaleFont = value; } }
        }

        protected bool _yAxisMajorTicAccessed = false;

        /// <summary>Properties for major tixs on Y axis.</summary>
        public MajorTic YAxisMajorTic
        {
            get
            {
                lock (Lock)
                {
                    MajorTic ret = YAxis.MajorTic;
                    if (!_yAxisMajorTicAccessed)
                    {
                        _yAxisMajorTicAccessed = true;
                        ret.IsOpposite = false;
                        ret.IsInside = true;
                        ret.IsOutside = false;
                    }
                    return ret;
                }
            }
        }

        protected bool _yAxisMinorTicAccessed = false;

        /// <summary>Properties for minor tixs on X axis.</summary>
        public MinorTic YAxisMinorTic
        {
            get
            {
                lock (Lock)
                {
                    MinorTic ret = YAxis.MinorTic;
                    if (!_yAxisMinorTicAccessed)
                    {
                        _yAxisMinorTicAccessed = true;
                        ret.IsOpposite = false;
                        ret.IsInside = true;
                        ret.IsOutside = false;
                    }
                    return ret;
                }
            }
        }


        bool _yAxisMajorGridIsVisible = DefaultMajorGridIsVisible;

        /// <summary>Whether major grid lines for Y axis are shown.</summary>
        bool YAxisMajorGridIsVisible
        {
            get { lock (Lock) { return _yAxisMajorGridIsVisible; } }
            set { lock (Lock) { _yAxisMajorGridIsVisible = value; } }
        }

        /// <summary>Major grid lines properties for Y axis.</summary>
        MajorGrid YAxisMajorGrid
        { get { return YAxis.MajorGrid; } }

        bool _yAxisMinorGridIsVisible = DefaultMinorGridIsVisible;

        /// <summary>Whether minor grid lines for Y axis are shown.</summary>
        bool YAxisMinorGridIsVisible
        {
            get { lock (Lock) { return _yAxisMinorGridIsVisible; } }
            set { lock (Lock) { _yAxisMinorGridIsVisible = value; } }
        }

        /// <summary>Minor grid lines properties for Y axis.</summary>
        MinorGrid YAxisMinorGrid
        { get { return YAxis.MinorGrid; } }



        // SECOND (ALTERNATIVE) X AXIS:

        bool _x2AxisIsVisible = DefaultAxes2IsVisible;

        /// <summary>Whether X2 axis is visible.</summary>
        bool X2AxisIsVisible
        {
            get { lock (Lock) { return _x2AxisIsVisible; } }
            set { lock (Lock) { _x2AxisIsVisible = value; if (value == true) X2AxisScaleIsVisible = true; } }
        }

        /// <summary>Gets the X2 axis of the graph.</summary>
        public virtual X2Axis X2Axis
        {
            get { return GraphPane.X2Axis; }
        }


        /// <summary>Default label to for the second X axis for new plot objects.</summary>
        public static string DefaultX2AxisLabel = "X2";

        protected string _x2AxisLabel = DefaultX2AxisLabel;

        /// <summary>Label for the second X axis.</summary>
        public string X2AxisLabel
        {
            get { lock (Lock) { return _x2AxisLabel; } }
            set { lock (Lock) { _x2AxisLabel = value; } }
        }

        protected FontSpec _x2AxisLabelFont;

        /// <summary>Font used for the second X axis label.</summary>
        public virtual FontSpec X2AxisLabelFont
        {
            get
            {
                lock (Lock)
                {
                    if (_x2AxisLabelFont == null)
                    {
                        if (_pane != null)
                            _x2AxisLabelFont = new FontSpec(GraphPane.X2Axis.Title.FontSpec);
                        else
                            _x2AxisLabelFont = DefaultAxisLabelFont;
                    }
                    return _x2AxisLabelFont;
                }
            }
            set { lock (Lock) { _x2AxisLabelFont = value; } }
        }

        protected bool _x2AxisIsZeroLine = false;

        /// <summary>Whether zero line on X2 axis is plotted or not.</summary>
        bool X2AxisIsZeroLine
        {
            get { lock (Lock) { return _x2AxisIsZeroLine; } }
            set { lock (Lock) { _x2AxisIsZeroLine = value; } }
        }

        bool _x2AxisScaleIsVisible = DefaultScale2IsVisible;

        /// <summary>Whether scale is shown for X axis.</summary>
        bool X2AxisScaleIsVisible
        {
            get { lock (Lock) { return _x2AxisScaleIsVisible; } }
            set { lock (Lock) { _x2AxisScaleIsVisible = value; } }
        }

        /// <summary>Scale for X axis.</summary>
        Scale X2AxisScale
        {
            get { lock (Lock) { return X2Axis.Scale; } }
        }

        protected FontSpec _x2AxisScaleFont;

        /// <summary>Settings for font on X scale.</summary>
        public FontSpec X2AxisScaleFont
        {
            get
            {
                lock (Lock)
                {
                    if (_x2AxisScaleFont == null)
                    {
                        if (_pane != null)
                            _x2AxisScaleFont = new FontSpec(GraphPane.X2Axis.Scale.FontSpec);
                        else
                            _x2AxisScaleFont = DefaultAxisScaleFont;
                    }
                    return _x2AxisScaleFont;
                }
            }
            set { lock (Lock) { _x2AxisScaleFont = value; } }
        }

        protected bool _x2AxisMajorTicAccessed = false;

        /// <summary>Properties for major tixs on X2 axis.</summary>
        public MajorTic X2AxisMajorTic
        {
            get
            {
                lock (Lock)
                {
                    MajorTic ret = X2Axis.MajorTic;
                    if (!_x2AxisMajorTicAccessed)
                    {
                        _x2AxisMajorTicAccessed = true;
                        ret.IsOpposite = false;
                        ret.IsInside = true;
                        ret.IsOutside = false;
                    }
                    return ret;
                }
            }
        }

        protected bool _x2AxisMinorTicAccessed = false;

        /// <summary>Properties for minor tixs on X2 axis.</summary>
        public MinorTic X2AxisMinorTic
        {
            get
            {
                lock (Lock)
                {
                    MinorTic ret = X2Axis.MinorTic;
                    if (!_x2AxisMinorTicAccessed)
                    {
                        _x2AxisMinorTicAccessed = true;
                        ret.IsOpposite = false;
                        ret.IsInside = true;
                        ret.IsOutside = false;
                    }
                    return ret;
                }
            }
        }

        bool _x2AxisMajorGridIsVisible = DefaultMajorGrid2IsVisible;

        /// <summary>Whether major grid lines for X2 axis are shown.</summary>
        bool X2AxisMajorGridIsVisible
        {
            get { lock (Lock) { return _x2AxisMajorGridIsVisible; } }
            set { lock (Lock) { _x2AxisMajorGridIsVisible = value; } }
        }

        /// <summary>Major grid lines properties for X2 axis.</summary>
        MajorGrid X2AxisMajorGrid
        { get { return X2Axis.MajorGrid; } }

        bool _x2AxisMinorGridIsVisible = DefaultMinorGrid2IsVisible;

        /// <summary>Whether minor grid lines for X2 axis are shown.</summary>
        bool X2AxisMinorGridIsVisible
        {
            get { lock (Lock) { return _x2AxisMinorGridIsVisible; } }
            set { lock (Lock) { _x2AxisMinorGridIsVisible = value; } }
        }

        /// <summary>Minor grid lines properties for X2 axis.</summary>
        MinorGrid X2AxisMinorGrid
        { get { return X2Axis.MinorGrid; } }



        // SECOND (ALTERNATIVE) Y AXIS:

        bool _y2AxisIsVisible = DefaultAxes2IsVisible;

        /// <summary>Whether Y2 axis is visible.</summary>
        public bool Y2AxisIsVisible
        {
            get { lock (Lock) { return _y2AxisIsVisible; } }
            set { lock (Lock) { _y2AxisIsVisible = value; if (value == true) Y2AxisScaleIsVisible = true; } }
        }

        /// <summary>Gets the Y2 axis of the graph.</summary>
        public virtual Y2Axis Y2Axis
        {
            get { return GraphPane.Y2Axis; }
        }

        /// <summary>Default label for the second Y axis for new plot objects.</summary>
        public static string DefaultY2AxisLabel = "Y2";

        protected string _y2AxisLabel = DefaultY2AxisLabel;

        /// <summary>Label for the second Y axis.</summary>
        public string Y2AxisLabel
        {
            get { lock (Lock) { return _y2AxisLabel; } }
            set { lock (Lock) { _y2AxisLabel = value; } }
        }

        protected FontSpec _y2AxisLabelFont;

        /// <summary>Font used for the second Y axis label.</summary>
        public virtual FontSpec Y2AxisLabelFont
        {
            get
            {
                lock (Lock)
                {
                    if (_y2AxisLabelFont == null)
                    {
                        if (_pane != null)
                            _y2AxisLabelFont = new FontSpec(GraphPane.Y2Axis.Title.FontSpec);
                        else
                            _y2AxisLabelFont = DefaultAxisLabelFont;
                    }
                    return _y2AxisLabelFont;
                }
            }
            set { lock (Lock) { _y2AxisLabelFont = value; } }
        }

        protected bool _y2AxisIsZeroLine = false;

        /// <summary>Whether zero line on Y2 axis is plotted or not.</summary>
        bool Y2AxisIsZeroLine
        {
            get { lock (Lock) { return _y2AxisIsZeroLine; } }
            set { lock (Lock) { _y2AxisIsZeroLine = value; } }
        }

        bool _y2AxisScaleIsVisible = DefaultScale2IsVisible;

        /// <summary>Whether scale is shown for Y2 axis.</summary>
        bool Y2AxisScaleIsVisible
        {
            get { lock (Lock) { return _y2AxisScaleIsVisible; } }
            set { lock (Lock) { _y2AxisScaleIsVisible = value; } }
        }

        /// <summary>Scale for Y2 axis.</summary>
        Scale Y2AxisScale
        {
            get { lock (Lock) { return Y2Axis.Scale; } }
        }

        protected FontSpec _y2AxisScaleFont;

        /// <summary>Settings for font on Y2 scale.</summary>
        public FontSpec Y2AxisScaleFont
        {
            get
            {
                lock (Lock)
                {
                    if (_y2AxisScaleFont == null)
                    {
                        if (_pane != null)
                            _y2AxisScaleFont = new FontSpec(GraphPane.Y2Axis.Scale.FontSpec);
                        else
                            _y2AxisScaleFont = DefaultAxisScaleFont;
                    }
                    return _y2AxisScaleFont;
                }
            }
            set { lock (Lock) { _y2AxisScaleFont = value; } }
        }

        protected bool _y2AxisMajorTicAccessed = false;

        /// <summary>Properties for major tixs on Y2 axis.</summary>
        public MajorTic Y2AxisMajorTic
        {
            get
            {
                lock (Lock)
                {
                    MajorTic ret = Y2Axis.MajorTic;
                    if (!_y2AxisMajorTicAccessed)
                    {
                        _y2AxisMajorTicAccessed = true;
                        ret.IsOpposite = false;
                        ret.IsInside = true;
                        ret.IsOutside = false;
                    }
                    return ret;
                }
            }
        }

        protected bool _y2AxisMinorTicAccessed = false;

        /// <summary>Properties for minor tixs on Y2 axis.</summary>
        public MinorTic Y2AxisMinorTic
        {
            get
            {
                lock (Lock)
                {
                    MinorTic ret = Y2Axis.MinorTic;
                    if (!_y2AxisMinorTicAccessed)
                    {
                        _y2AxisMinorTicAccessed = true;
                        ret.IsOpposite = false;
                        ret.IsInside = true;
                        ret.IsOutside = false;
                    }
                    return ret;
                }
            }
        }

        bool _y2AxisMajorGridIsVisible = DefaultMajorGrid2IsVisible;

        /// <summary>Whether major grid lines for Y2 axis are shown.</summary>
        bool Y2AxisMajorGridIsVisible
        {
            get { lock (Lock) { return _y2AxisMajorGridIsVisible; } }
            set { lock (Lock) { _y2AxisMajorGridIsVisible = value; } }
        }

        /// <summary>Major grid lines properties for Y2 axis.</summary>
        MajorGrid Y2AxisMajorGrid
        { get { return Y2Axis.MajorGrid; } }

        bool _y2AxisMinorGridIsVisible = DefaultMinorGrid2IsVisible;

        /// <summary>Whether minor grid lines for Y2 axis are shown.</summary>
        bool Y2AxisMinorGridIsVisible
        {
            get { lock (Lock) { return _y2AxisMinorGridIsVisible; } }
            set { lock (Lock) { _y2AxisMinorGridIsVisible = value; } }
        }

        /// <summary>Minor grid lines properties for Y2 axis.</summary>
        MinorGrid Y2AxisMinorGrid
        { get { return Y2Axis.MinorGrid; } }


        #endregion Settings.Decoration


        #region Operation

        /// <summary>Updates the current plotter's settings such that they are reflected on the graph.</summary>
        public void Update()
        {
            lock (Lock)
            {

                // Define various graph decorations:


                //TitleFont = new FontSpec("Times New Roman", 30 /* size */,
                //    Color.Blue, true /* bold */, true /* italic */, false /* underline */);
                //TitleFont.Border.IsVisible = false;
                

                // Set the titles and axis labels:
                GraphPane.Title.Text = Title;  GraphPane.Title.FontSpec = TitleFont;
                //Pane.XAxis.Title.Text = XAxisLabel;   Pane.XAxis.Title.FontSpec = XAxisLabelFont;
                //Pane.YAxis.Title.Text = YAxisLabel;   Pane.YAxis.Title.FontSpec = YAxisLabelFont;
                //Pane.X2Axis.Title.Text = X2AxisLabel; Pane.X2Axis.Title.FontSpec = X2AxisLabelFont;
                //Pane.Y2Axis.Title.Text = Y2AxisLabel; Pane.Y2Axis.Title.FontSpec = Y2AxisLabelFont;

                // Settings for X axis:
                XAxis.IsVisible = XAxisIsVisible;
                XAxis.Title.Text = XAxisLabel; 
                XAxis.Title.FontSpec = XAxisLabelFont;

                XAxisMajorGrid.IsVisible = XAxisMajorGridIsVisible;
                XAxisMajorGrid.IsZeroLine = XAxisIsZeroLine;

                XAxisMinorGrid.IsVisible = XAxisMinorGridIsVisible;

                XAxisScale.IsVisible = XAxisScaleIsVisible;
                XAxisScale.FontSpec = XAxisScaleFont;

                // Just call the tic getters in order to perform some initialization
                MajorTic MaTiX = XAxisMajorTic;
                MinorTic MaMiX = XAxisMinorTic;


                // Settings for Y axis:
                YAxis.IsVisible = YAxisIsVisible;
                YAxis.Title.Text = YAxisLabel;
                YAxis.Title.FontSpec = YAxisLabelFont;

                YAxisMajorGrid.IsVisible = YAxisMajorGridIsVisible;
                YAxisMajorGrid.IsZeroLine = YAxisIsZeroLine;

                YAxisMinorGrid.IsVisible = YAxisMinorGridIsVisible;

                YAxisScale.IsVisible = YAxisScaleIsVisible;
                YAxisScale.FontSpec = YAxisScaleFont;

                // Just call the tic getters in order to perform some initialization
                MajorTic MaTiY = YAxisMajorTic;
                MinorTic MaMiY = YAxisMinorTic;


                // Settings for second (alternative) X axis:
                X2Axis.IsVisible = X2AxisIsVisible;
                X2Axis.Title.Text = X2AxisLabel;
                X2Axis.Title.FontSpec = X2AxisLabelFont;

                X2AxisMajorGrid.IsVisible = X2AxisMajorGridIsVisible;
                X2AxisMajorGrid.IsZeroLine = X2AxisIsZeroLine;

                X2AxisMinorGrid.IsVisible = X2AxisMinorGridIsVisible;

                X2AxisScale.IsVisible = X2AxisScaleIsVisible;
                X2AxisScale.FontSpec = X2AxisScaleFont;

                // Just call the tic getters in order to perform some initialization
                MajorTic MaTiX2 = X2AxisMajorTic;
                MinorTic MaMiX2 = X2AxisMinorTic;


                // Settings for Y axis:
                Y2Axis.IsVisible = Y2AxisIsVisible;
                Y2Axis.Title.Text = Y2AxisLabel;
                Y2Axis.Title.FontSpec = Y2AxisLabelFont;

                Y2AxisMajorGrid.IsVisible = Y2AxisMajorGridIsVisible;
                Y2AxisMajorGrid.IsZeroLine = Y2AxisIsZeroLine;

                Y2AxisMinorGrid.IsVisible = Y2AxisMinorGridIsVisible;

                Y2AxisScale.IsVisible = Y2AxisScaleIsVisible;
                Y2AxisScale.FontSpec = Y2AxisScaleFont;

                // Just call the tic getters in order to perform some initialization
                MajorTic MaTiY2 = Y2AxisMajorTic;
                MinorTic MaMiY2 = Y2AxisMinorTic;

                Y2AxisScale.IsVisible = Y2AxisScaleIsVisible;

                


                //// Show the x axis grid
                //Pane.XAxis.MajorGrid.IsVisible = true;

                ////Pane.XAxis.MinorGrid.IsVisible = true;
                ////Pane.XAxis.MinorGrid.DashOff = 0.2F;

                //// Make the Y axis scale red
                //Pane.YAxis.Scale.FontSpec.FontColor = System.Drawing.Color.Red;
                //Pane.YAxis.Title.FontSpec.FontColor = System.Drawing.Color.Red;
                //// turn off the opposite tics so the Y tics don't show up on the Y2 axis
                //Pane.YAxis.MajorTic.IsOpposite = false;
                //Pane.YAxis.MinorTic.IsOpposite = false;
                //// Don't display the Y zero line
                //Pane.YAxis.MajorGrid.IsZeroLine = false;
                //// Align the Y axis labels so they are flush to the axis
                //Pane.YAxis.Scale.Align = AlignP.Inside;
                //// Manually set the axis range
                //Pane.YAxis.Scale.Min = -30;
                //Pane.YAxis.Scale.Max = 30;

                //// Enable the Y2 axis display
                //Pane.Y2Axis.IsVisible = true;
                //// Make the Y2 axis scale blue
                //Pane.Y2Axis.Scale.FontSpec.FontColor = System.Drawing.Color.Blue;
                //Pane.Y2Axis.Title.FontSpec.FontColor = System.Drawing.Color.Blue;
                //// turn off the opposite tics so the Y2 tics don't show up on the Y axis
                //Pane.Y2Axis.MajorTic.IsOpposite = false;
                //Pane.Y2Axis.MinorTic.IsOpposite = false;

                //Pane.YAxis.MajorTic.IsOpposite = false;
                //Pane.YAxis.MinorTic.IsOpposite = false;

                //Pane.Y2Axis.Scale.Min = -40;
                //Pane.Y2Axis.Scale.Max = -15;


                //// Display the Y2 axis grid lines
                //Pane.Y2Axis.MajorGrid.IsVisible = true;
                //// Align the Y2 axis labels so they are flush to the axis
                //Pane.Y2Axis.Scale.Align = AlignP.Inside;


                // Fill the axis background with a gradient
                GraphPane.Chart.Fill = new Fill(Background1, Background2, (float) BackgroundGradientAngle);

                if (ShowPlotTip)
                {
                    // Add a text box with instructions
                    TextObj text = new TextObj(
                        "Zoom: left mouse & drag\nPan: middle mouse & drag\nContext Menu: right mouse",
                        0.05f, 0.95f, CoordType.ChartFraction, AlignH.Left, AlignV.Bottom);
                    text.FontSpec.StringAlignment = System.Drawing.StringAlignment.Near;
                    GraphPane.GraphObjList.Add(text);
                }

                // Enable scrollbars if needed
                Window.IsShowHScrollBar = true;
                Window.IsShowVScrollBar = true;
                Window.IsAutoScrollRange = true;
                Window.IsScrollY2 = true;

                // Show tooltips when the mouse hovers over a point
                Window.IsShowPointValues = true;
                Window.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);

                for (int i = 0; i < PlotObjects.Count; ++i)
                {
                    PlotZedGraphBase plot = PlotObjects[i];
                    plot.Update();
                }


            } // lock

        }  // Update()


        /// <summary>Resets the view (scale and position of the graph) in such a way that all plotted 
        /// objects are visible.</summary>
        public void ResetView()
        {
            Window.RestoreScale(Window.GraphPane);
        }

        /// <summary>Displays customized tooltips when the mouse hovers over a point (event handler).</summary>
        private string MyPointValueHandler(ZedGraphControl control, GraphPane pane,
                        CurveItem curve, int iPt)
        {
            // Get the PointPair that is under the mouse
            PointPair pt = curve[iPt];
            string precisionString = "f" + PointValuePrecision;
            return curve.Label.Text + ": (" + pt.X.ToString(precisionString) + ", " + pt.Y.ToString(precisionString) + " )";
        }



        #endregion Operation


        #region IDisposable


        ~PlotterZedGraph()
        {
            Dispose(false);
        }


        private bool disposed = false;

        /// <summary>Implementation of IDisposable interface.</summary>
        public void Dispose()
        {
            lock(Lock)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>Does the job of freeing resources. 
        /// <para></para>This method can be  eventually overridden in derived classes (if they use other 
        /// resources that must be freed - in addition to such resources of the current class).
        /// In the case of overriding this method, you should usually call the base.<see cref="Dispose"/>(<paramref name="disposing"/>).
        /// in the overriding method.</para></summary>
        /// <param name="disposing">Tells whether the method has been called form Dispose() method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free unmanaged objects resources:

                // Set large objects to null:

                disposed = true;
            }
        }

        #endregion IDisposable



        #region Static

        /// <summary>Creates a top-level window including a ZedGraph control (type <see cref="ZedGraphControl"/>)
        /// and returns it such that the ZedGraph control can be accessed and window opened.</summary>
        /// <returns>A toplevel window containg a ZedGraph control.</returns>
        public static ZedGraphWindow CreateWindow()
        {
            return new ZedGraphWindow();
        }

        public static PlotterZedGraph CreateDefault(ZedGraphControl window)
        {
            PlotterZedGraph ret = new PlotterZedGraph(window);
            // Settings for title:
            ret.TitleFont.Family = "Times";
            ret.TitleFont.FontColor = Color.Blue;
            ret.TitleFont.IsBold = true;
            ret.TitleFont.IsItalic = true;
            // Settings for axis labels:
            ret.XAxisLabelFont.Family = "Courier";
            ret.TitleFont.FontColor = Color.Black;
            ret.TitleFont.IsBold = true;
            ret.TitleFont.IsItalic = false;
            ret.YAxisLabelFont = new FontSpec(ret.XAxisLabelFont);
            ret.X2AxisLabelFont = new FontSpec(ret.XAxisLabelFont);
            ret.Y2AxisLabelFont = new FontSpec(ret.X2AxisLabelFont);
            return ret;
        }


        #endregion Static



        #region Examples


        /// <summary>Example that demonstrates how decorations (titles, labels, scales, etc.) can be sadjusted..</summary>
        public static void ExampleDecorations()
        {

            IRealFunction f1 = Func.GetExp();
            IRealFunction f2 = Func.GetCubic(1, 2, 2, 0);
            int
                numPoints = 400,
                from = -1, to = 9;

            ZedGraphWindow win = new ZedGraphWindow();
            PlotterZedGraph plotter = PlotterZedGraph.CreateDefault(win.GraphControl);
            PlotZedgraphCurve plot = new PlotZedgraphCurve(plotter);
            plot.SetCurveDefiniton(f1);
            plot.MinParam = from;
            plot.MaxParam = to;
            plot.NumPoints = numPoints;
            plot.PointsVisible = false;
            plot.LineColor = Color.Red;
            plot.LineWidth = 2;
            plot.LineSmoothing = true;
            plot.LegendString = "Exp";


            PlotZedgraphCurve plot1 = new PlotZedgraphCurve(plotter);
            plot1.SetCurveDefiniton(f2);
            plot1.MinParam = from;
            plot1.MaxParam = to;
            plot1.NumPoints = numPoints;
            plot1.PointsVisible = false;
            plot1.LineColor = Color.LightGreen;
            plot1.LineWidth = 2;
            plot1.LineSmoothing = true;
            // Important: This plot will be assigned to the second Y axis, which will be activated 
            // on the plotter (due to very different ranges of both functions):
            plot1.IsY2Axis = true;
            

            // Adjust some settings on the plotter:
            // First, make the second Y axes (Y2) visible since the second plot is associated with that axes!
            plotter.Y2AxisIsVisible = true;
            //plotter.Y2AxisScaleIsVisible = false; // the above statement automaticall sets this to true, if you don't want scale to be shown then uncomment this line

            // Show grid for the second axis rather than the main one (main is default) - but show the minor grid and not the major one (which is unusual):
            plotter.YAxisMajorGridIsVisible = false; plotter.YAxisMinorGridIsVisible = false;
            plotter.Y2AxisMinorGridIsVisible = true; 
            plotter.Y2AxisMajorGridIsVisible = false; 

            // Asjust some additional things:
            plotter.XAxisScaleFont.FontColor = System.Drawing.Color.Purple;
            plotter.XAxisScaleFont.IsUnderline = true;
            plotter.YAxisScaleFont.FontColor = Color.Red;
            plotter.XAxisMajorTic.IsOutside = true; plotter.XAxisMajorTic.IsOpposite = true; plotter.XAxisMajorTic.IsCrossInside = true;

            plotter.Y2AxisScaleFont.IsDropShadow = true; plotter.Y2AxisScaleFont.DropShadowColor = Color.LightGray; plotter.Y2AxisScaleFont.DropShadowOffset = 0.2f; 
            plotter.Y2AxisScaleFont.FontColor = Color.Green;
            plotter.YAxisLabelFont.FontColor = Color.Orange; plotter.YAxisLabelFont.Angle = 160;

            plotter.Title = "Exponential and cubic function";
            plotter.XAxisLabel = "x";
            plotter.YAxisLabel = "Exponential";
            plotter.Y2AxisLabel = "Cubic";

            plotter.Update();
            win.ShowDialog();
        }



        /*
         * Sample code for saving clipboard to EMF: 
        public override void Copy()
        {
            // Example code from http://forums.ni.com/t5/Measurement-Studio-for-NET/Graphs-to-clipboard-as-vector-graphics/td-p/537105:
            Graphics g = _scatterGraph.CreateGraphics();
            IntPtr hdc = g.GetHdc();
            Metafile metaFile = new Metafile(hdc, EmfType.EmfOnly);
            g.ReleaseHdc(hdc);
            g.Dispose();
            Graphics gMeta = Graphics.FromImage(metaFile);
            ComponentDrawArgs cda = new ComponentDrawArgs(gMeta, _scatterGraph.Bounds);
            _scatterGraph.Draw(cda);
            gMeta.Dispose();
            // This function can be found from my previous post.
            ClipboardMetafileHelper.PutEnhMetafileOnClipboard(this.Handle, metaFile);
        }
         * */


        /// <summary>Tests plotting with ZedGraph. If file path is specified then graph contents is saved to the specified file.</summary>
        /// <param name="fileName">Name of the file where graph is saved (as a bitmap). If the file already exists then it is not
        /// overwritten.</param>
        /// <remarks>For saving graphics as metafile (i.e. in a vector graphics format), see the below:
        /// <para>Extend Zedgraph to produce SVG: http://stackoverflow.com/questions/2501302/extend-zedgraph-to-produce-svg</para>
        /// <para>ZedGraph C# Graph Data Export to CSV Using a Custom Context Menu: http://www.smallguru.com/2009/06/zedgraph-csharp-graph-data-export-to-cs/ </para>
        /// <para> How to save an image as EMF? http://stackoverflow.com/questions/152729/gdi-c-how-to-save-an-image-as-emf </para>
        /// <para> Graphs to clipboard as vector graphics http://forums.ni.com/t5/Measurement-Studio-for-NET/Graphs-to-clipboard-as-vector-graphics/td-p/537105 </para>
        /// </remarks>
        public static void ExampleCurveStylesWithSave(string filePath)
        {

            int numPoints = 200;
            double minX = 0;
            double maxX = 1.5 * 2.0 * Math.PI;
            //double minY = -1.2;
            //double maxY = 1.2;
            ZedGraphWindow win = new ZedGraphWindow();
            PlotterZedGraph plotter = PlotterZedGraph.CreateDefault(win.GraphControl);

            double step = (maxX - minX) / (double)(numPoints - 1);
            double phase, phase0 = 2 * Math.PI * (double)1 / (double)3;

            // Add a plot of sine curve to the plotter:
            PlotZedgraphCurve plot = new PlotZedgraphCurve(plotter);
            plot.LineColor = Color.Blue;
            plot.PointFillColor = Color.White;
            phase = 0 * phase0;
            plot.LegendString = "sin(x-2*Pi*" + phase + ")";
            for (int j=0; j<numPoints; ++j)
            {
                double x = minX + j * step;
                double y = Math.Sin(x-phase);
                plot.AddPoint(x, y);
            }

            // Add another plot of the sine curve, with different phase, to the plotter:
            plot = new PlotZedgraphCurve(plotter);
            plot.LineColor = Color.Green;
            plot.PointFillColor = Color.Yellow;
            phase = 1 * phase0;
            plot.LegendString = "sin(x-2*Pi*" + phase + ")";
            for (int j=0; j<numPoints/3; ++j)
            {
                double x = minX + j * step;
                double y = Math.Sin(x-phase);
                plot.AddPoint(x, y);
            }
            plot.LineSmoothing = true;
            plot.LineWidth = 0.2;
            plot.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            plot.LineColor = Color.Red;
            plot.PointSize = 6;
            plot.PointTypeSymbol = SymbolType.Star;
            plot.PointBorderColor = Color.Black;
            plot.PointFillColor = Color.Yellow;


            // Add yet another plot of the sine curve, with different phase, to the plotter:
            plot = new PlotZedgraphCurve(plotter);
            plot.LineColor = Color.Green;
            plot.PointFillColor = Color.Yellow;
            phase = 2 * phase0;
            plot.LegendString = "sin(x-2*Pi*" + phase + ")";
            for (int j=0; j<numPoints; ++j)
            {
                double x = minX + j * step;
                double y = Math.Sin(x-phase);
                plot.AddPoint(x, y);
            }
            plot.LineSmoothing = true;
            plot.LineWidth = 4;
            plot.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            plot.LineColor = Color.Purple;
            plot.LineStepType = StepType.ForwardStep;
            plot.PointsVisible = false;


            // Set graph bounds before update, such that saved image will be centered and scaled properly:
            plotter.XAxisScale.Min = -1;
            plotter.XAxisScale.Max = 10;
            plotter.YAxisScale.Min = -1.2;
            plotter.YAxisScale.Max = 1.2;
            // plotter.SetBounds(-1, 10, -1.2, 1.2); // shorter way

            // Update the plotter, this will cause plots to be actually rendered:
            plotter.Update();

            if (!string.IsNullOrEmpty(filePath))
            {
                // Save the plot to a file:
                Console.WriteLine();
                Console.WriteLine("Saving plot as a EMF file...");
                Console.WriteLine("File path: " + filePath);
                if (File.Exists(filePath))
                {
                    Console.WriteLine();
                    Console.WriteLine("File already exists, graph is NOT saved!");
                    Console.WriteLine();
                }
                else
                {
                    plotter.Window.GraphPane.ZoomStack.Clear();
                    // Save window:
                    plotter.Window.MasterPane.GetImage().Save(filePath);
                    // win.GraphControl.SaveAsEmf();  // this opens a file dialog box
                    Console.WriteLine("... saving to a file done.");
                    Console.WriteLine();
                }
            }

            // Open a modal window with plots:
            win.ShowDialog();
            //win.ShowDialog();

        }

        /// <summary>Sine function with the specified frequency factor and phase.
        /// Used in the <see cref="ExampleLissajous"/>.</summary>
        protected class ExampleSineFunctionForLissajous: RealFunction
        {
            public ExampleSineFunctionForLissajous(double frequencyFactor, double phase)
            {
                this.FrequencyFactor = frequencyFactor; this.Phase = phase;
            }

            public ExampleSineFunctionForLissajous(double frequencyFactor) : this(frequencyFactor, 0) { }

            double FrequencyFactor = 1, Phase = 0;

            public override double Value(double x)
            {
                return  Math.Sin((double)FrequencyFactor * x + Phase);
            }

        }


        /// <summary>Plots a specific Lissayous curve.</summary>
        public static void ExampleLissajous()
        {
            ExampleLissajous(5, 4);
        }

        /// <summary>Plots a Lissayous curve with the specified integer ratios of frequencies and phase difference zero.</summary>
        /// <param name="a">Frequency factor for function defining X coordinate.</param>
        /// <param name="b">Frequency factor for function defining Y coordinate.</param>
        public static void ExampleLissajous(int a, int b)
        {
            ZedGraphWindow win = new ZedGraphWindow();
            PlotterZedGraph plotter = PlotterZedGraph.CreateDefault(win.GraphControl);
            PlotZedgraphCurve plot = new PlotZedgraphCurve(plotter);
            plot.SetCurveDefiniton(
                new ExampleSineFunctionForLissajous(a),
                new ExampleSineFunctionForLissajous(b)
                );
            plot.MinParam = 0;
            plot.MaxParam = 2 * Math.PI;
            plot.NumPoints = 400;
            plot.PointsVisible = false;
            plot.LineColor = Color.Red;
            plot.LineWidth = 3;
            plot.LineSmoothing = true;

            plotter.Update();
            win.ShowDialog();
        }

        /// <summary>Plots a set of sine surves with different (equally spaced) phases, in different colors on the same graph.</summary>
        public static void ExempleSinePlots()
        {
            ExempleSinePlots(12 /* numCurves */, 200 /* numPoints */);
        }

        /// <summary>Plots a set of sine surves with different (equally spaced) phases, in different colors on the same graph.</summary>
        /// <param name="numCurves">Number of curves to be plotted.</param>
        /// <param name="numPoints">Number of points used to plot each curve.</param>
        public static void ExempleSinePlots(int numCurves, int numPoints)
        {
            double minX = 0;
            double maxX = 5 * 2 * Math.PI;
            //double minY = -1.2;
            //double maxY = 1.2;
            ZedGraphWindow win = new ZedGraphWindow();
            PlotterZedGraph plotter = PlotterZedGraph.CreateDefault(win.GraphControl);

            ColorScale scale = ColorScale.CreateRainbow(0, numCurves);
            double step = (maxX - minX)/(double)(numPoints - 1);
            for (int i = 0; i < numCurves; ++i)
            {
                PlotZedgraphCurve plot = new PlotZedgraphCurve(plotter);
                plot.PointFillColor = scale.GetColor(i);
                plot.LineColor = scale.GetColor(i);
                plot.LegendString = "sin(x-2*Pi*" + i + "/" + numCurves + ")";
                
                double phase = 2 * Math.PI * (double) i / (double) numCurves;

                for (int j=0; j<numPoints; ++j)
                {
                    double x = minX + j * step;
                    double y = Math.Sin(x-phase);
                    plot.AddPoint(x, y);
                }
            }
            plotter.Update();
            win.ShowDialog();
        }


        #endregion Examples



    }  // class PlotterZedGraph

}
