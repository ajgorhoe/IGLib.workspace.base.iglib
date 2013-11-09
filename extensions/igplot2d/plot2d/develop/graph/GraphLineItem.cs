// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// using ZedGraph;
using System.Drawing;

namespace IG.Plot2d
{

    /// <summary>Plottable item that consists of multiple line segments.</summary>
    /// $A Igor Jun09;
    public class GraphLineItem : GraphPlotItem
    {


        public GraphLineItem(GraphBase graph, string title, string description)
            : base(graph)
        {
            object lockObj;
            if (graph != null)
                lockObj = graph.Lock;
            else
                lockObj = new Object();
            int index = 0;
            lock (lockObj)
            {
                // Infer index of the current item: 
                if (graph != null)
                    index = graph.PlotItems.Count;
                this.Title = title;
                this.Description = Description;
                // Standard settings:
                //this.CurveData.Symbol.Type = ZedGraph.SymbolType.Circle;
                //this.CurveData.Symbol.Fill = new Fill(Color.White);
                //this.CurveData.Color = Color.Blue;

            }

        }


        void xx_to_delete()
        {
            //ZedGraph.GraphPane myPane = Graph.GraphPane;

            //ZedGraph.LineItem myCurve = new ZedGraph.LineItem("MyLineItem");

            // Generate a Black curve with triangle symbols, and "Energy" in the legend
            //myCurve = myPane.AddCurve("Energy",
            //    eList, Color.Black, SymbolType.Triangle);
            // Fill the symbols with white
            //myCurve.Symbol.Fill = new Fill(Color.White);
            //// Associate this curve with the Y2 axis
            //myCurve.IsY2Axis = true;
            //// Associate this curve with the second Y2 axis
            //myCurve.YAxisIndex = 1;

        }


        #region Data

        protected string _title;

        /// <summary>Curve title.</summary>
        public string Title {
            get { return _title; }
            set { _title = value; }
        }


        protected string _description;

        /// <summary>Curve description.</summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }



        //protected ZedGraph.LineItem _curveData = new ZedGraph.LineItem((LineItem) null);


        ///// <summary>Curve data (linear segments) containing the settings for the current curve.</summary>
        //protected ZedGraph.LineItem CurveData
        //{
        //    get { return _curveData; }
        //}



        //protected ZedGraph.LineItem _curve;

        //public ZedGraph.LineItem Curve
        //{
        //    get { return _curve; }
        //    protected set { 
        //        _curve = value;
        //        UpdateInGraph();
        //    }
        //}




        #endregion Data



        /// <summary>Updates the current item in the containing ZedGraph's control. 
        /// A ZedGraph item corresponding to the current item is updated according to 
        /// data contained in this item.</summary>
        public override void UpdateInGraph()
        {
            throw new NotImplementedException("GraphLineItem.UpdateInGraph not implemented.");
        }

        /// <summary>Detaches the current item from ZedGraph's graph control. 
        /// If ZedGraph control contains any items that are managed by the current item,
        /// these items  may be set invisible or be disposed. 
        /// References to related  representations in the Zedgraph control are deleted, 
        /// such that connection of the item with the  Zedgraph's graph object is broken.</summary>
        public override void DetachFromGraph()
        {
            throw new NotImplementedException("GraphLineItem.DetachFromGraph not implemented.");
        }

        #region Data





        #endregion Data



    }
}
