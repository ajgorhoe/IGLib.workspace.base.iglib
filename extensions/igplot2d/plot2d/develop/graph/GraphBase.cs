// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ICollection	= System.Collections.ICollection;
using System.Collections;

//using ZedGraph;


using IG.Lib;
using System.Windows.Forms;

namespace IG.Plot2d
{

    /// <summary>Interface for graph plotting objects.</summary>
    /// $A Igor Jun09;
    public interface IGraph : ILockable
    {
    }


    /// <summary>Graph data & control class.</summary>
    /// $A Igor Jun09;
    public abstract class GraphBase : IGraph, 
        ILockable
    {
        protected string _title;
        protected string _description;
        // protected ICollection types;

        #region Construction 


        /// <summary>Creates a GraphBase object with the specified title and description.</summary>
        /// <param name="title">Name (title) of the current plotter.</param>
        /// <param name="description">Description of the graph object.</param>
        public GraphBase(string title, string description /*, ICollection types */)
        {
            Init(title, description /* , types */ );
        }

        /// <summary>Takes care of initialization.</summary>
        /// <param name="title">Title of the current graph class.</param>
        /// <param name="description"></param>
        private void Init(string title, string description /* , ICollection types */)
        {
            this._description = description;
            this._title = title;


        }

        
        #endregion Construction 


        #region ILockable

        object _lock = new object();

        /// <summary>Object used for thread locking of the current object.</summary>
        public object Lock
        {
            get { return _lock; }
        }

        #endregion ILockable

        #region Data

        /// <summary>Graph title.</summary>
        public virtual string Title { 
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>Graph description.</summary>
        public virtual string Description 
        { 
            get { return _description; }
            set { _description = value; }
        }

        // public virtual ICollection Types { get { return types; } }

        //protected ZedGraphControl _graphControl;

        ///// <summary>
        ///// The control the graph pane is in.
        ///// </summary>
        //public ZedGraphControl GraphControl
        //{
        //    get { return _graphControl; }
        //    protected set { _graphControl = value; }
        //}

        #endregion Data

        #region Graph Data


        // List<GraphAxisItem> _axes = new List<GraphAxisItem>();

        List<GraphPlotItem> _plotItems = new List<GraphPlotItem>();

        /// <summary>Gets a list of axes maintained in the graph.</summary>
        //public List<GraphAxisItem> Axes
        //{ get { return _axes; } }

        /// <summary>Gets a list of plot items (such as lines or bar charts) contained
        /// in the graph.</summary>
        public List<GraphPlotItem> PlotItems
        { get { return _plotItems; } }


        #region Graph Data Form Manipulation

        Form _containingForm = null;

        public Form ContainingForm
        {
            protected internal set {
                lock(Lock)
                {
                    _containingForm = value;
                    _containingControl = value;
                }
            }
            get
            {
                return _containingForm;
            }
        }

        Control _containingControl;

        public Control ContainingControl
        {
            protected internal set
            {
                lock (Lock)
                {
                    _containingControl = value;
                    _containingForm = value.FindForm();
                }
            }
            get
            {
                return _containingControl;
            }
        }

        protected Thread _windowThread;

        /// <summary>Thread in which new window is opened.</summary>
        public Thread WindowThread
        { 
            get { return _windowThread; }
            protected set { _windowThread = value;  }
        }


        ///// <summary>Opens a new modal window containing the graph control that renders the current graph.</summary>
        //public void OpenModalWindow()
        //{
        //    OpenWindowCurrentThread();
        //}

        ///// <summary>Creates and shows in a new thread a window containing the graph control that renders 
        ///// the current graph.</summary>
        //public void OpenWindow()
        //{
        //    lock (Lock)
        //    {
        //        if (ContainingControl != null)
        //            DetachGraphWindow();
        //        WindowThread = new Thread(OpenWindowCurrentThread);
        //        WindowThread.IsBackground = true;
        //        WindowThread.Start();
        //    }
        //}

        /// <summary>Opens a new modal window containing the graph control that renders the current graph.</summary>
        //protected void OpenWindowCurrentThread()
        //{
        //        try
        //        {
        //            GraphWindow win = new GraphWindow();
        //            GraphControl = win.GraphControl;
        //            ContainingControl = win;
        //            ContainingForm = win;
        //            win.ShowDialog();
        //        }
        //        catch (Exception ex)
        //        {
        //            App.Rep.ReportError(ex, "Could nor open a 2D graph window.");
        //        }
        //}



        /// <summary>Removes the top-level graph window.</summary>
        public void DetachGraphWindow()
        {
            //TODO: implement!
        }

        #endregion Graph Data Form Manipulation

        #endregion Graph Data


        #region Testing

        public static void Example()
        {
        }

        #endregion Testing


    } // class DemoBase

}  