// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Num;

namespace IG.Plot2d
{
    
    /// <summary>Represents a plottable item contained in a graph.
    /// This excludes markup markups such as axes, legends, etc.</summary>
    /// $A Igor Jun09;
    public interface IGraphPlotItem
    {
    }


    /// <summary>Representation of a plottable item contained in a graph.
    /// This excludes markup markups such as axes, legends, etc.</summary>
    /// $A Igor Jun09;
    public abstract class GraphPlotItem : GraphItem
    {


        public GraphPlotItem(GraphBase graph)
            : base(graph)
        { }



        /// <summary>Gets index of the current plot item on the Graph.</summary>
        public int Index
        {
            get
            {
                if (Graph != null)
                    lock (Graph.Lock)
                    {
                        if (Graph.PlotItems != null)
                        {
                            for (int i = 0; i < Graph.PlotItems.Count; ++i)
                            {
                                GraphPlotItem item = Graph.PlotItems[i];
                                if (item == this)
                                    return i;
                            }
                        }
                    }
                return -1;
            }
        }

        #region Properties


        #endregion Properties


        #region Bounds

        protected double _minX, _maxX, _minY, _maxY;

        /// <summary>Minimal x co-ordinate of the plotted data.</summary>
        public double MinX
        {
            get { return _minX; }
            protected set { _minX = value; }
        }

        /// <summary>Maximal x co-ordinate of the plotted data.</summary>
        public double MaxX
        {
            get { return _maxX; }
            protected set { _maxX = value; }
        }

        /// <summary>Minimal y co-ordinate of the plotted data.</summary>
        public double MinY
        {
            get { return _minY; }
            protected set { _minY = value; }
        }

        /// <summary>Maximal y co-ordinate of the plotted data.</summary>
        public double MaxY
        {
            get { return _maxY; }
            protected set { _maxY = value; }
        }

        #endregion Bounds

    }

}
