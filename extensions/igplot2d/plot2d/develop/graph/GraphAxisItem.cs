// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

// using ZedGraph;

namespace IG.Forms
{



    ///// <summary>Contains data and methods for manipulating a single axis item
    ///// of a graph.
    ///// Axis items manipulate axis labels and axis ticks.</summary>
    ///// $A Ikgor Jul09;
    //public class GraphAxisItem : GraphItem
    //{

    //    #region Initialization


    //    //public GraphAxisItem(GraphBase graph) : base(graph)
    //    //{
    //    //    // Standard settings:
    //    //    // Enable the axis display
    //    //    AxisData.IsVisible = true;
    //    //    // Color:
    //    //    AxisData.Color = System.Drawing.Color.Blue;
    //    //    // Make the axis scale blue
    //    //    AxisData.Scale.FontSpec.FontColor = System.Drawing.Color.Blue;
    //    //    AxisData.Title.FontSpec.FontColor = System.Drawing.Color.Blue;
    //    //    // turn off the opposite tics so e.g. the Y2 tics don't show up on the Y axis and vice versa
    //    //    AxisData.MajorTic.IsOpposite = false;
    //    //    AxisData.MinorTic.IsOpposite = false;
    //    //    // Display the axis grid lines
    //    //    AxisData.MajorGrid.IsVisible = false;
    //    //    // Align the axis labels so they are flush to the axis
    //    //    AxisData.Scale.Align = AlignP.Inside;
    //    //    //AxisData.Scale.Min = 1.5;
    //    //    //AxisData.Scale.Max = 3;

    //    //}


    //    #endregion Initialization




    //    #region Data


    //    //protected ZedGraph.Axis _axisData = new ZedGraph.XAxis();

    //    ///// <summary>Axis data containing the settings for the current axis.</summary>
    //    //protected ZedGraph.Axis AxisData
    //    //{
    //    //    get { return _axisData; }
    //    //}


    //    /// <summary>Gets index of the current axis on the Graph.</summary>
    //    public int Index
    //    {
    //        get {
    //            if (Graph != null)
    //            lock(Graph.Lock)
    //            {
    //                if (Graph.Axes != null)
    //                {
    //                    for (int i = 0; i < Graph.Axes.Count; ++i)
    //                    {
    //                        GraphAxisItem axis = Graph.Axes[i];
    //                        if (axis == this)
    //                            return i;
    //                    }
    //                }
    //            }
    //            return -1;
    //        }
    //    }

    //    //protected ZedGraph.Axis _axis;

    //    ///// <summary>ZedGraph axes controlled by the current axis.</summary>
    //    //public ZedGraph.Axis Axis
    //    //{ 
    //    //    get { return _axis; }
    //    //    protected set { _axis = value; }
    //    //}

    //    /// <summary>Resets all data that associate this axis with any pre-definied axis.</summary>
    //    protected void ResetAxisAssociations()
    //    {
    //        _isXaxis = false;
    //        _isX2axis = false;
    //        _isYaxis = false;
    //        _isY2axis = false;
    //        _axisIndex = -1;
    //        Axis = null;
    //    }

        


    //    protected bool _isXaxis;

    //    /// <summary>Whether the current axis represents an X axis of the ZegGraph control.</summary>
    //    public bool IsXAxis
    //    {
    //        get { return _isXaxis; }
    //        protected internal set {
    //            ResetAxisAssociations();
    //            _isXaxis = value;
    //        }
    //    }

    //    protected bool _isX2axis;

    //    /// <summary>Whether the current axis represents an X2 axis of the ZegGraph control.</summary>
    //    public bool IsX2Axis
    //    {
    //        get { return _isX2axis; }
    //        protected internal set
    //        {
    //            ResetAxisAssociations();
    //            _isX2axis = value;

    //        }
    //    }

    //    protected bool _isYaxis;

    //    /// <summary>Whether the current axis represents an left-hand side Y axis of the ZegGraph control.</summary>
    //    public bool IsYAxis
    //    {
    //        get { return _isYaxis; }
    //        protected internal set
    //        {
    //            ResetAxisAssociations();
    //            _isYaxis = value;
    //        }
    //    }

    //    protected bool _isY2axis;

    //    /// <summary>Whether the current axis represents an right-hand side Y axis of the ZegGraph control.</summary>
    //    public bool IsY2Axis
    //    {
    //        get { return _isY2axis; }
    //        protected internal set
    //        {
    //            ResetAxisAssociations();
    //            _isY2axis = value;
    //        }
    //    }

    //    protected int _axisIndex=-1;

    //    /// <summary>Index of the current axis in the ZedGraph control.</summary>
    //    protected internal int AxisIndex
    //    {
    //        get { return _axisIndex; }
    //        protected set {
    //            ResetAxisAssociations();
    //            _axisIndex = value;
    //        }
    //    }

    //    double _min;

    //    /// <summary>Gets or sets minimal value on the scale.</summary>
    //    public double Min 
    //    {
    //        get {
    //            if (Axis != null)
    //                return  Axis.Scale.Min;
    //            return _min;
    //        }
    //        set
    //        {
    //            _min = value;
    //            if (Axis != null)
    //                Axis.Scale.Min = value;
    //        }
    //    }

    //    double _max;

    //    /// <summary>Gets or sets maximal value on the scale.
    //    /// Get tries to return ZedGraph axis' maximum, and set tries to set the ZedGrapg axis max.
    //    /// Set also stores the value to internal variable from where Zedgraph's axis value is set
    //    /// on  update.</summary>
    //    public double Max
    //    {
    //        get
    //        {
    //            if (Axis != null)
    //                return Axis.Scale.Max;
    //            return _max;
    //        }
    //        set
    //        {
    //            _max = value;
    //            if (Axis != null)
    //                Axis.Scale.Max = value;
    //        }
    //    }


    //    // Setting that will affect axis visual representation:


    //    /// <summary>Whether axis is visible on the graph.</summary>
    //    public bool IsVisible
    //    {
    //        get 
    //        { return AxisData.IsVisible; }
    //        set 
    //        { 
    //            AxisData.IsVisible = value;
    //            UpdateInGraph();
    //        }
    //    }


    //    /// <summary>Scale font color for the current axis.</summary>
    //    public System.Drawing.Color ScaleFontColor
    //    {
    //        get { return AxisData.Scale.FontSpec.FontColor; }
    //        set {
    //            AxisData.Scale.FontSpec.FontColor = value;
    //            UpdateInGraph();
    //        }
    //    }

    //    /// <summary>Title font color for the current axis.</summary>
    //    public System.Drawing.Color TitleFontColor
    //    {
    //        get { return AxisData.Title.FontSpec.FontColor; }
    //        set {
    //            AxisData.Title.FontSpec.FontColor = value;
    //            UpdateInGraph();
    //        }
    //    }



    //    #endregion Data

    //    // Specification of which axis is represented in ZedGraph control by this item.
    //    // ZedGraph control knows the following axis items:
    //    //  XAxis - a single axis in x direction
    //    // 


    //    #region Operation


    //    ///// <summary>Updates the current item in the containing ZedGraph's control. 
    //    ///// A ZedGraph item corresponding to the current item is updated according to 
    //    ///// data contained in this item.</summary>
    //    //public override void UpdateInGraph()
    //    //{
    //    //    if (Axis != null)
    //    //    {
    //    //        // Check whether associated axis from ZedGraph control corresponds to settings:
    //    //        GraphPane graphPane = Graph.GraphPane;
    //    //        if (graphPane == null)
    //    //            Axis = null;
    //    //        else
    //    //        {
    //    //            if (IsXAxis && Axis != graphPane.XAxis)
    //    //                Axis = null;
    //    //            if (IsX2Axis && Axis != graphPane.X2Axis)
    //    //                Axis = null;
    //    //            if (IsYAxis && Axis != graphPane.YAxis)
    //    //                Axis = null;
    //    //            if (IsY2Axis && Axis != graphPane.Y2Axis)
    //    //                Axis = null;
    //    //        }
    //    //    }
    //    //    if (Axis == null && Graph !=null)
    //    //    {
    //    //        GraphPane graphPane = Graph.GraphPane;
    //    //        if (graphPane != null)
    //    //        {
    //    //            if (IsXAxis)
    //    //                Axis = graphPane.XAxis;
    //    //            if (IsX2Axis)
    //    //                Axis = graphPane.X2Axis;
    //    //            if (IsYAxis)
    //    //                Axis = graphPane.YAxis;
    //    //            if (IsY2Axis)
    //    //                Axis = graphPane.Y2Axis;
    //    //        }
    //    //    }
    //    //    if (Axis != null)
    //    //    {
    //    //        Axis.IsVisible = AxisData.IsVisible;
    //    //        Axis.Color = AxisData.Color;
    //    //        Axis.Scale.FontSpec.FontColor = AxisData.Scale.FontSpec.FontColor;
    //    //        Axis.Title.FontSpec.FontColor = AxisData.Title.FontSpec.FontColor;
    //    //        Axis.MajorTic.IsOpposite = AxisData.MajorTic.IsOpposite;
    //    //        Axis.MinorTic.IsOpposite = AxisData.MinorTic.IsOpposite;
    //    //        Axis.MinorGrid.IsVisible = AxisData.MinorGrid.IsVisible;
    //    //        Axis.Scale.Align = AxisData.Scale.Align;
    //    //    }
    //    //    // TODO: consider whether thread operation issues be cared about (check 
    //    //    // Graph.ContainingControl.InvokeRequired, etc.)
    //    //}

    //    /// <summary>Detaches the current item from ZedGraph's graph control. 
    //    /// If ZedGraph control contains any items that are managed by the current item,
    //    /// these items  may be set invisible or be disposed. 
    //    /// References to related  representations in the Zedgraph control are deleted, 
    //    /// such that connection of the item with the  Zedgraph's graph object is broken.</summary>
    //    //public override void DetachFromGraph()
    //    //{
    //    //    // TODO: implemnt operations on ZedGraphControl if necessary!
    //    //    // Maybe this is not necessary and we can just leave axes in the control when
    //    //    // detaching.

    //    //    Axis = null;
    //    //    AxisIndex = -1;
    //    //    IsXAxis = false;
    //    //    IsX2Axis = false;
    //    //    IsYAxis = false;
    //    //    IsY2Axis = false;
    //    //}


    //    #endregion Operation


    //}



}
