// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IG.Plot2d
{

    /// <summary>Interface for all items that can be put on a graph (including axes, labels, etc.).</summary>
    /// $A Igor Jun09;
    public interface IGraphItem
    {
        /// <summary>Graph that contains this axis.</summary>
        GraphBase Graph
        { get; }


        /// <summary>Updates the current item in the containing control.</summary>
        void UpdateInGraph();

        /// <summary>Detaches the current item from the graph. </summary>
        void DetachFromGraph();
    }


    /// <summary>Base lass for all items that can be put on a graph (including axes, labels, etc.).</summary>
    /// $A Igor Jun09;
    public abstract class GraphItem
    {

        
        private GraphItem() { }

        public GraphItem(GraphBase graph)
        {
            this.Graph = graph;
        }

        #region Data

        GraphBase _graph;

        /// <summary>Graph that contains this axis.</summary>
        public GraphBase Graph
        {
            get { return _graph; }
            protected set { _graph = value; }
        }

        #endregion Data

        #region Operation

        /// <summary>Updates the current item in the containing control.</summary>
        public abstract void UpdateInGraph();

        /// <summary>Detaches the current item from the graph. </summary>
        public abstract void DetachFromGraph();

        #endregion Operation


    }
}
