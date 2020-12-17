//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using System.Threading;


//using IG.Lib;

//namespace IG.Plot2d
//{

//    /// <summary>NPlot graphs plotting object.</summary>
//    /// $A Igor Jun09;
//    public class GraphNPlot : GraphBase
//    {

//        /// <summary>Creates a new graph plotter based on a NPlot control.</summary>
//        /// <param name="control">Control used for plotting windows graphs.</param>
//        public GraphNPlot(PlotControlNPlot control)
//            : base("NPlot Graph", "NPlot Graph control.")
//        {
//        }


//        /// <summary>Creates a new graph plotter based on a NPlot control.</summary>
//        /// <param name="control">Control on which graphs are actually plotted.</param>
//        /// <param name="title">Title ofthe current plotter.</param>
//        /// <param name="description">Description of the current plotter.</param>
//        public GraphNPlot(PlotControlNPlot control, string title, string description): base(title, description)
//        {
//            this.Control = control;
//        }


//        PlotControlNPlot _control;

//        /// <summary>2D plot control where graphic items are plotted.</summary>
//        public PlotControlNPlot Control
//        {
//            get { return _control; }
//            protected set { _control = value; }
//        }




//    }

//}