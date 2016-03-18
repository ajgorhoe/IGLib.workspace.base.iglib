using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;
using IG.Num;

using Kitware.VTK;


namespace IG.Gr3d
{

    /// <summary><para>Fly modes specifies how axes are placed on the bounding box containing graphics objects.</para>
    /// <para>1 - CLOSEST_TRIAD </para>
    /// <para>2 - FURTHEST_TRIAD </para>
    /// <para>3 - OUTER_EDGES </para>
    /// <para>4 - STATIC </para>
    /// </summary>
    /// $A Igor Oct11;
    public enum VtkFlyMode
    {
        /// <summary>Axes are not shown</summary>
        // None = 0,
        /// <summary>The CLOSEST_TRIAD fly mode, consists of the three axes x-y-z forming a triad that lies closest to the 
        //specified camera</summary>
        ClosestTriad = 1,
        /// <summary>The FURTHEST_TRIAD fly mode consists of the three axes x-y-z forming a triad that lies 
        ///furthest from the specified camera</summary>
        FurthestTriad = 2,
        /// <summary>The OUTER_EDGES fly mode is constructed from edges that are on the 
        /// exterior of the bounding box, exterior as determined from examining outer edges of the bounding 
        /// box in projection (display) space.</summary>
        OuterEdges = 3,
        /// <summary>The STATIC fly mode, constructs axes from all edges of the bounding box.</summary>
        Static = 4
    }



    /// <summary>Manipulates axes, labels, grids, and other decorations of graphhics plotted in VTK windows.</summary>
    /// <remarks>Currently, all the decorations are put on a single renderer that is one of the renderers
    /// that are already used. 
    /// Initially the idea is that a separate renderer would be used for axes and other decorations generated
    /// and handled by this class. Since we don't currently know how one can combine multiple renderers in the same
    /// window and show contents of all of them together with the same viewing projection (is this possible at all?),
    /// we use the first renderer that is provided.</remarks>
    /// $A Igor Oct11;
    public class VtkDecorationHandler: ILockable
    {

        #region Construction

        private VtkDecorationHandler()
        { }

        /// <summary>Constructor.
        /// Since no Actors or Renderers are specified, updating the containing bounding box will take into
        /// account all Actors of all Renderers that are currently attached to the window.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        public VtkDecorationHandler(VtkPlotter plotter) :
            this(plotter, true /* updateImmediately */, (vtkRenderer[])null /* renderers */, null /* actors2D */, null /* actors */)
        {  }

        /// <summary>Constructor.
        /// Since no Actors or Renderers are specified, updating the containing bounding box will take into
        /// account all Actors of all Renderers that are currently attached to the window.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        public VtkDecorationHandler(VtkPlotter plotter, bool updateImmediately) :
            this(plotter, updateImmediately, (vtkRenderer[])null /* renderers */, null /* actors2D */, null /* actors */)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(VtkPlotter plotter, params vtkActor[] actors) :
            this(plotter, true /* updateImmediately */, (vtkRenderer[])null /* renderers */, null /* actors2D */, actors)
        {  }


        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(VtkPlotter plotter, bool updateImmediately, params vtkActor[] actors) :
            this(plotter, updateImmediately, (vtkRenderer[])null /* renderers */, null /* actors2D */, actors)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Renderers.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(VtkPlotter plotter, params vtkRenderer[] renderers):
            this(plotter, true /* updateImmediately */, renderers, null /* actors2D */, null /* actors */)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Renderers.</summary>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(VtkPlotter plotter, bool updateImmediately, params vtkRenderer[] renderers):
            this(plotter, updateImmediately, renderers, null /* actors2D */, null /* actors */)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A (single) renderer that is a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(VtkPlotter plotter, vtkRenderer renderer, params vtkActor[] actors) :
            this(plotter, true /* updateImmediately */, new vtkRenderer[] { renderer } /* renderers */, null /* actors2D */, actors)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        /// <param name="renderers">A (single) renderer that is a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(VtkPlotter plotter, bool updateImmediately, vtkRenderer renderer, params vtkActor[] actors) :
            this(plotter, updateImmediately, new vtkRenderer[] { renderer } /* renderers */, null /* actors2D */, actors)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Renderers and Actors.
        /// Decorations are updated immediately when the object is constructed. If this is not desired,
        /// use the constructor where you can set the updating flag and set the corresponding argument to false.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(VtkPlotter plotter, vtkRenderer[] renderers, params vtkActor[] actors):
            this(plotter, true /* updateImmediately */, renderers, null /* actors2D */, actors)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors2D.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(VtkPlotter plotter, params vtkActor2D[] actors2D) :
            this(plotter, true /* updateImmediately */, (vtkRenderer[])null /* renderers */, actors2D, null /* actors */)
        { }


        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors2D.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(VtkPlotter plotter, bool updateImmediately, params vtkActor2D[] actors2D) :
            this(plotter, updateImmediately, (vtkRenderer[])null /* renderers */, actors2D, null /* actors */)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified renderers and Actors2D.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(VtkPlotter plotter, vtkRenderer[] renderers, params vtkActor2D[] actors2D) :
            this(plotter, true /* updateImmediately */, renderers, actors2D, null /* actors */)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified actors and Actors2D.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(VtkPlotter plotter, vtkActor2D[] actors2D, params vtkActor[] actors) :
            this(plotter, true /* updateImmediately */, (vtkRenderer[])null /* renderers */, actors2D, actors)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specifiedrenderers, Actors and Actors2D.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(VtkPlotter plotter, vtkRenderer[] renderers, vtkActor2D[] actors2D, params vtkActor[] actors) :
            this(plotter, true /* updateImmediately */, renderers, actors2D, actors)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors and Actors2D.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(VtkPlotter plotter, bool updateImmediately, vtkActor2D[] actors2D, params vtkActor[] actors) :
            this(plotter, updateImmediately, (vtkRenderer[])null /* renderers */, actors2D, actors)
        { }


        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Renderers and Actors.</summary>
        /// <param name="plotter">VTK plotter for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(VtkPlotter plotter, bool updateImmediately, vtkRenderer[] renderers, vtkActor2D[] actors2D, params vtkActor[] actors)
        {
            this.UpdateWhenConstructed = updateImmediately;
            this.Plotter = plotter;
            AddRenderers(renderers);
            AddActors(actors);
            AddActors2D(actors2D);
            if (UpdateWhenConstructed)
                Update();
        }



#if !NeverDefinedPreprocessorVariable

        #region Construction.ByVtkRenderWindow


        /// <summary>Constructor.
        /// Since no Actors or Renderers are specified, updating the containing bounding box will take into
        /// account all Actors of all Renderers that are currently attached to the window.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        public VtkDecorationHandler(vtkRenderWindow win) :
            this(win, true /* updateImmediately */, (vtkRenderer[])null /* renderers */, null /* actors2D */, null /* actors */)
        {  }

        /// <summary>Constructor.
        /// Since no Actors or Renderers are specified, updating the containing bounding box will take into
        /// account all Actors of all Renderers that are currently attached to the window.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        public VtkDecorationHandler(vtkRenderWindow win, bool updateImmediately) :
            this(win, updateImmediately, (vtkRenderer[])null /* renderers */, null /* actors2D */, null /* actors */)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(vtkRenderWindow win, params vtkActor[] actors) :
            this(win, true /* updateImmediately */, (vtkRenderer[])null /* renderers */, null /* actors2D */, actors)
        {  }


        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(vtkRenderWindow win, bool updateImmediately, params vtkActor[] actors) :
            this(win, updateImmediately, (vtkRenderer[])null /* renderers */, null /* actors2D */, actors)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Renderers.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(vtkRenderWindow win, params vtkRenderer[] renderers):
            this(win, true /* updateImmediately */, renderers, null /* actors2D */, null /* actors */)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Renderers.</summary>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(vtkRenderWindow win, bool updateImmediately, params vtkRenderer[] renderers):
            this(win, updateImmediately, renderers, null /* actors2D */, null /* actors */)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A (single) renderer that is a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(vtkRenderWindow win, vtkRenderer renderer, params vtkActor[] actors) :
            this(win, true /* updateImmediately */, new vtkRenderer[] { renderer } /* renderers */, null /* actors2D */, actors)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        /// <param name="renderers">A (single) renderer that is a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(vtkRenderWindow win, bool updateImmediately, vtkRenderer renderer, params vtkActor[] actors) :
            this(win, updateImmediately, new vtkRenderer[] { renderer } /* renderers */, null /* actors2D */, actors)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Renderers and Actors.
        /// Decorations are updated immediately when the object is constructed. If this is not desired,
        /// use the constructor where you can set the updating flag and set the corresponding argument to false.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(vtkRenderWindow win, vtkRenderer[] renderers, params vtkActor[] actors):
            this(win, true /* updateImmediately */, renderers, null /* actors2D */, actors)
        {  }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors2D.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(vtkRenderWindow win, params vtkActor2D[] actors2D) :
            this(win, true /* updateImmediately */, (vtkRenderer[])null /* renderers */, actors2D, null /* actors */)
        { }


        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors2D.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(vtkRenderWindow win, bool updateImmediately, params vtkActor2D[] actors2D) :
            this(win, updateImmediately, (vtkRenderer[])null /* renderers */, actors2D, null /* actors */)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified renderers and Actors2D.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(vtkRenderWindow win, vtkRenderer[] renderers, params vtkActor2D[] actors2D) :
            this(win, true /* updateImmediately */, renderers, actors2D, null /* actors */)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified actors and Actors2D.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(vtkRenderWindow win, vtkActor2D[] actors2D, params vtkActor[] actors) :
            this(win, true /* updateImmediately */, (vtkRenderer[])null /* renderers */, actors2D, actors)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specifiedrenderers, Actors and Actors2D.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(vtkRenderWindow win, vtkRenderer[] renderers, vtkActor2D[] actors2D, params vtkActor[] actors) :
            this(win, true /* updateImmediately */, renderers, actors2D, actors)
        { }

        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Actors and Actors2D.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.</param>
        /// <param name="actors2D">A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public VtkDecorationHandler(vtkRenderWindow win, bool updateImmediately, vtkActor2D[] actors2D, params vtkActor[] actors) :
            this(win, updateImmediately, (vtkRenderer[])null /* renderers */, actors2D, actors)
        { }


        /// <summary>Constructor.
        /// Updating the containing bounding box will take into account all specified Renderers and Actors.</summary>
        /// <param name="win">VTK window for which axes and other decoration entities are manipulated.</param>
        /// <param name="updateImmediately">Whether decorations are updated immediately when constructed.
        /// If false then <see cref="Update"/>() must be called if one wants decorations to be displayed.</param>
        /// <param name="renderers">A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        /// <param name="actors">A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled.</param>
        public VtkDecorationHandler(vtkRenderWindow win, bool updateImmediately, vtkRenderer[] renderers, vtkActor2D[] actors2D, params vtkActor[] actors)
        {
            this.UpdateWhenConstructed = updateImmediately;
            this.Window = win;
            AddRenderers(renderers);
            AddActors(actors);
            AddActors2D(actors2D);
            if (UpdateWhenConstructed)
                Update();
        }

        #endregion Construction.ByVtkRenderWindow

#endif // if !NeverDefinedPreprocessorVariable


        #endregion Construction


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking


        #region Data

        protected VtkPlotter _plotter;

        public VtkPlotter Plotter
        {
            get
            {
                lock (Lock)
                {
                    return _plotter;
                }
            }
            set
            {
                if (value != _plotter)
                {
                    // Update dependencies:
                    {
                        PlotterAccessor = null;
                        Window = null;
                    }
                    _plotter = value;
                }
            }
        }


        private VtkPlotter.Accessor _plotterAccessor;


        /// <summary>Accessor object that grants access to some protected fields and properties of the
        /// VTK plotter (property <see cref="Plotter"/>).</summary>
        protected VtkPlotter.Accessor PlotterAccessor
        {
            get { lock (Lock) {
                if (_plotterAccessor == null)
                {
                    if (Plotter != null)
                        _plotterAccessor = new VtkPlotter.Accessor(Plotter);
                }
                return _plotterAccessor;
            } }
            private set { lock (Lock) { _plotterAccessor = value; } }
        }


        private IBoundingBox _bounds;

        private vtkRenderWindow _window;

        /// <summary>VTK window in which plotting is performed.</summary>
        public vtkRenderWindow Window
        {
            get { lock (Lock) {
                if (_window == null)
                {
                    if (PlotterAccessor != null)
                        _window = PlotterAccessor.Window;
                }
                return _window; 
            } }
            protected set {
                lock (Lock)
                {
                    //if (value == null)
                    //    throw new InvalidOperationException("VTK window may not be set to null.");
                    if (value != _window)
                    {
                        Actors.Clear();
                        Renderers.Clear();
                    }
                    _window = value;
                }
            }
        }

        /// <summary>Changes the VTK render window of the current decoration handler.</summary>
        /// <param name="win">Window that becomes this object's new rendering window.</param>
        public void ChangeWindow(vtkRenderWindow win)
        {
            if (win == null)
                _window = null;
            else
                Window = win;
        }

        private List<vtkActor> _actors = new List<vtkActor>();

        /// <summary>A list of Actors that are a part of the graphic scene for which axes
        /// and other decorations are handled. Mainly used for updating the bounding box.
        /// WARNING: 
        /// Whenever possible, this list should not be accessed directly. Use (create them, if necessary)
        /// other methods that perform specific operations on the lisr.</summary>
        protected List<vtkActor> Actors
        {
            get { return _actors; }
        }

        /// <summary>Adds the specified rendereds to the list of actors that are contained in the scene
        /// for which axes and other decoration entities are manipulated.
        /// Uniqueness is not guaranteed, i.e. the same actor can be added several times.</summary>
        /// <param name="actors">Actors to be added to the list.</param>
        public void AddActors(params vtkActor[] actors)
        {
            if (actors != null)
                lock (Lock)
                {
                    {
                        // Invalidate dependencies:
                        IsBoundsUpdated = false;
                        foreach (vtkActor currentActor in actors)
                        {
                            if (currentActor != null)
                                Actors.Add(currentActor);
                        }
                    }
                }
        }


        /// <summary>Removes the specified actors from the list of actors for which axes and other 
        /// decorations are handled.
        /// If some actor is included in the list multiple times then it is removed until it is no longer contained.</summary>
        /// <param name="actors">Actors to be removed from the list.</param>
        public void RemoveActors(params vtkActor[] actors)
        {
            if (actors != null)
            {
                lock (Lock)
                {
                    // Invalidate dependencies:
                    IsBoundsUpdated = false;
                    foreach (vtkActor currentActor in actors)
                    {
                        if (currentActor != null)
                        {
                            while (Actors.Contains(currentActor))
                            {
                                Actors.Remove(currentActor);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Returns true if the specified actor is internal (auxiliary) actor of the current object, false otherwise.
        /// In such a case the actor will not take part in some operations, e.g. in determining the bounding 
        /// box of the graphic scene.</summary>
        /// <param name="actor">Actor for which we querry whether it is an internal actor of the current helper object.</param>
        protected bool IsHandlersInternal(vtkActor actor)
        {
            if (actor==null)
                return false;
            if (actor == _cubeAxesActor)
                return true;
            // $$ HERE YOU SHOULD ADD OTHER AUXILIARY ACTORS WHEN DEFINED.
            return false;
        }


        private List<vtkActor2D> _actors2D = new List<vtkActor2D>();

        /// <summary>A list of Actors2D that are a part of the graphic scene for which axes
        /// and other decorations are handled. Mainly used for updating the bounding box.
        /// WARNING: 
        /// Whenever possible, this list should not be accessed directly. Use (create them, if necessary)
        /// other methods that perform specific operations on the lisr.</summary>
        /// $A Igor Oct11, Tako78 Dec13;
        protected List<vtkActor2D> Actors2D
        {
            get { return _actors2D; }
        }

        /// <summary>Adds the specified rendereds to the list of actors2D that are contained in the scene
        /// for which axes and other decoration entities are manipulated.
        /// Uniqueness is not guaranteed, i.e. the same actor can be added several times.</summary>
        /// <param name="actors">Actors2D to be added to the list.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public void AddActors2D(params vtkActor2D[] actors2D)
        {
            if (actors2D != null)
                lock (Lock)
                {
                    {
                        // Invalidate dependencies:
                        IsBoundsUpdated = false;
                        foreach (vtkActor2D currentActor2D in actors2D)
                        {
                            if (currentActor2D != null)
                                Actors2D.Add(currentActor2D);
                        }
                    }
                }
        }


        /// <summary>Removes the specified actors2D from the list of actors2D for which axes and other 
        /// decorations are handled.
        /// If some actor is included in the list multiple times then it is removed until it is no longer contained.</summary>
        /// <param name="actors">Actors2D to be removed from the list.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public void RemoveActors2D(params vtkActor2D[] actors2D)
        {
            if (actors2D != null)
            {
                lock (Lock)
                {
                    // Invalidate dependencies:
                    IsBoundsUpdated = false;
                    foreach (vtkActor2D currentActor2D in actors2D)
                    {
                        if (currentActor2D != null)
                        {
                            while (Actors2D.Contains(currentActor2D))
                            {
                                Actors2D.Remove(currentActor2D);
                            }
                        }
                    }
                }
            }
        }


        private List<vtkRenderer> _renderers = new List<vtkRenderer>();

        /// <summary>A list of Renderers that are a part of the graphic scene for which axes
        /// and other decorations are handled. Mainly used for updating the bounding box.
        /// WARNING: 
        /// Whenever possible, this list should not be accessed directly. Use (create them, if necessary)
        /// other methods that perform specific operations on the lisr.</summary>
        protected List<vtkRenderer> Renderers
        {
            get { return _renderers; }
        }

        /// <summary>Adds the specified rendereds to the list of renderers that are contained in the scene
        /// for which axes and other decoration entities are manipulated.
        /// Uniqueness is not guaranteed, i.e. the same actor can be added several times.</summary>
        /// <param name="renderers">Renderers that are added to the list.</param>
        public void AddRenderers(params vtkRenderer[] renderers)
        {
            if (renderers != null)
            {
                lock (Lock)
                {
                    // Invalidate dependencies:
                    IsBoundsUpdated = false;
                    foreach (vtkRenderer currentRenderer in renderers)
                    {
                        if (currentRenderer != null)
                            Renderers.Add(currentRenderer);
                    }
                }
            }
        }

        /// <summary>Removes the specified renderers from the list of renderers for which axes and other 
        /// decorations are handled.
        /// If some renderer is included in the list multiple times then it is removed until it is no longer contained.</summary>
        /// <param name="renderers">Rendeerrs to be removed from the list.</param>
        public void RemoveRenderers(params vtkRenderer[] renderers)
        {
            if (renderers != null)
            {
                lock (Lock)
                {
                    // Invalidate dependencies:
                    IsBoundsUpdated = false;
                    foreach (vtkRenderer currentRenderer in renderers)
                    {
                        if (currentRenderer != null)
                        {
                            while (Renderers.Contains(currentRenderer))
                            {
                                Renderers.Remove(currentRenderer);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>Returns true if the specified renderer is internal (auxiliary) renderer of the current object, false otherwise.
        /// In such a case the renderer will not take part in some operations, e.g. in determining the bounding 
        /// box of the graphic scene.</summary>
        /// <param name="renderer">Renderer for which we querry whether it is an internal renderer of the current helper object.</param>
        protected bool IsHandlersInternal(vtkRenderer renderer)
        {
            // Remark:
            // Currently, decorations are rendered by one of the renderers that contain graphics.
            // Therefore, the renderer used for decorations is not considered internal renderer, and will
            // be taken into account e.g. for bounds calculation (since this may be one of the usual 
            // renderers contained in the list of renderers).
            return false;
            //if (renderer==null)
            //    return false;
            //if (renderer == _decorationRenderer)
            //    return true;
            //return false;
        }

        private bool _isBoundsUpdated = false;

        /// <summary>Whether bounds are updated or not, according to internal actors and renderers.</summary>
        public bool IsBoundsUpdated
        {
            get { return _isBoundsUpdated; }
            private set { _isBoundsUpdated = value; }
        }

        /// <summary>Bounds of the graphic scene for which axes and other decorations are handled.</summary>
        protected IBoundingBox Bounds
        {
            get { lock (Lock) 
            {
                if (_bounds == null)
                    _bounds = new BoundingBox3d();
                return _bounds; 
            } 
            }
            set { lock (Lock) { _bounds = value; } }
        }

        /// <summary>Resets the bounds.</summary>
        public void ResetBounds()
        {
            lock (Lock)
            {
                Bounds.Reset();
                IsBoundsUpdated = false;
            }
        }


        /// <summary>Updates the bounds according to eventual internal bounds (defined by actors and renderers
        /// included in the current decorator).
        /// <para>This also updates positioning and sizing of included decorations such that cube axes.</para></summary>
        protected void UpdateBounds()
        {
            UpdateBounds(null);
        }

        /// <summary>Updates the bounds according to eventual internal bounds (defined by actors and renderers
        /// included in the current decorator), and additionally also according to the specified bounds.
        /// <para>This also updates positioning and sizing of included decorations such that cube axes.</para></summary>
        /// <param name="bounds">Specified bounds according to which the bounds are
        /// also decorated (beside the internal bounds).</param>
        protected void UpdateBounds(IBoundingBox bounds)
        {
            lock (Lock)
            {
                if (Bounds != null)
                {
                    Bounds.Update(bounds);
                    IsBoundsUpdated = true;
                }
                if (!IsBoundsUpdated) 
                    UpdateBoundsInternal();
                
                SetCubeAxesActorBounds(CubeAxesActor, Bounds);

                // $$ Add updates for other 3D decoration elements here!

            }
        }

        
        /// <summary>Sets the bounds for actors that to eventual internal bounds.</summary>
        /// <param name="bounds">Bounding box to which the bounds are set.</param>
        public void SetBounds()
        {
            SetBounds(null);
        }

        /// <summary>Sets the bounds for actors that must fill the bounding box aroung all graphic objects,
        /// to the specifid values.</summary>
        /// <param name="bounds">Bounding box to which the bounds are set.</param>
        public void SetBounds(IBoundingBox bounds)
        {
            lock (Lock)
            {
                if (Bounds != null)
                {
                    Bounds.Reset();
                    UpdateBounds(bounds);
                }
            }
        }

        /// <summary>Sets position and sizing of the specified cube axes actor according to the specified bounds.</summary>
        /// <param name="actor">Actor whose bounds are updated.</param>
        /// <param name="newBounds">New bounds of the actor.</param>
        public static void SetCubeAxesActorBounds(vtkCubeAxesActor actor, IBoundingBox newBounds)
        {
            double xMin = -1, xMax = 1, yMin = -1, yMax = 1, zMin = -1, zMax = 1;
            if (newBounds.IsMinDefined(0))
                xMin = newBounds.GetMin(0);
            if (newBounds.IsMaxDefined(0))
                xMax = newBounds.GetMax(0);
            if (newBounds.IsMinDefined(1))
                yMin = newBounds.GetMin(1);
            if (newBounds.IsMaxDefined(1))
                yMax = newBounds.GetMax(1);
            if (newBounds.IsMinDefined(2))
                zMin = newBounds.GetMin(2);
            if (newBounds.IsMaxDefined(2))
                zMax = newBounds.GetMax(2);
            if (actor != null)
                actor.SetBounds(xMin, xMax, yMin, yMax, zMin, zMax);
        }

        /// <summary>Updates the bounding box <see cref="Bounds"/> in such a way that it contains
        /// all actors from the internal list of actors and all renderers from the internal list of renderers.</summary>
        protected void UpdateBoundsInternal()
        {
            lock (Lock)
            {
                IBoundingBox boundsAux = Bounds;
                //boundsAux.Reset();
                foreach (vtkRenderer currentRenderer in Renderers)
                {
                    if (currentRenderer != null && !IsHandlersInternal(currentRenderer))
                        UtilVtk.UpdateBounds(ref boundsAux, currentRenderer);
                }
                foreach (vtkActor currentActor in Actors)
                {
                    if (currentActor != null && !IsHandlersInternal(currentActor))
                        UtilVtk.UpdateBounds(ref boundsAux, currentActor);
                }
                Bounds = boundsAux;
                IsBoundsUpdated = true;
            }
        }


        protected vtkRenderer _decorationRenderer;

        /// <summary>Gets the renderer used for decoration entities (axes, etc.).
        /// Currently, no special renderer is created for tgis purpose, but one is obtained either
        /// from the list of renderers included in the 3D graphics scene, or one is obtained form 
        /// the VTK window attached to this object.
        /// Lazy evaluation. Renderer is obtained only when first needed.</summary>
        public vtkRenderer DecorationRenderer
        {
            get 
            {
                lock (Lock)
                {
                    if (_decorationRenderer == null)
                    {
                        if (Renderers.Count > 0)
                            _decorationRenderer = Renderers[0];
                        else
                            _decorationRenderer = UtilVtk.GetFirstRenderer(Window);
                    }
                    return _decorationRenderer;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    if (value != null && value != _decorationRenderer)
                    {
                        vtkCubeAxesActor actor = CubeAxesActor;
                        if (actor != null)
                        {
                            value.AddActor(actor);
                            
                        }
                        vtkScalarBarActor actorScalarBar = ScalarBarActor;
                        if (actorScalarBar != null)
                        {
                            value.AddActor2D(actorScalarBar);

                        }
                        vtkLegendBoxActor actorLegendBox = LegendBoxActor;
                        if (actorLegendBox != null)
                        {
                            value.AddActor2D(actorLegendBox);
                        }


                        // $$ HERE YOU SHOULD ADD ACTORS FOR OTHER GRAPH DECORATIONS!
                    }
                    _decorationRenderer = value;
                }
            }
        }


        #endregion Data


        #region Labels


        vtkLegendBoxActor _legendBoxActor;

        public vtkLegendBoxActor LegendBoxActor
        {
            get
            {
                if (_legendBoxActor == null)
                {
                    LegendBoxActor = vtkLegendBoxActor.New();
                }
                return _legendBoxActor;
            }
            protected set
            {
                lock (Lock)
                {
                    if (value != null && value != _legendBoxActor)
                    {
                        if (ShowLegendBox)
                            value.SetVisibility(1);
                        else
                            value.SetVisibility(0);
                        vtkProperty2D prop = value.GetProperty();
                        
                        value.SetNumberOfEntries(LegendBoxNumEntries);
                        for (int i = 0; i < LegendBoxNumEntries; i++)
                        {
                            //vtkSphereSource legendSphereSource = vtkSphereSource.New();
                            //vtkPolyData legendSphere = legendSphereSource.GetOutput();

                            value.SetEntryString(i, LegendBoxTitles[i] );
                            value.SetEntrySymbol(i, Symbol);
                            
                        }
                        
                        value.SetEntryColor(0, 1, 1, 1);
                        
                        // $$ UPDATE OTHER PROPERTIES IF ANY ARE ADDED!

                        //// Add actor to the decoration renderer:
                        //vtkRenderer renderer = DecorationRenderer;
                        //if (renderer != null)
                        //{
                        //    if (!UtilVtk.ContainsActor2D(renderer, value))
                        //        renderer.AddActor2D(value);
                        //    //value.SetCamera(renderer.GetActiveCamera());
                        //}
                    }
                    _legendBoxActor = value;
                }
            }
        }


        private bool _showLegendBox = false;

        /// <summary>Whether legend-box is shown or not.
        /// Default is false.</summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public bool ShowLegendBox
        {
            get { return _showLegendBox; }
            set
            {
                lock (Lock)
                {
                    _showLegendBox = value;
                    // Update dependencies:
                    vtkLegendBoxActor legendBoxActor = _legendBoxActor;
                    if (legendBoxActor != null)
                    {
                        if (value == true)
                            legendBoxActor.SetVisibility(1);
                        else
                            legendBoxActor.SetVisibility(0);
                    }
                }
            }
        }
        
        private int _legendBoxNumEntries = 1;

        /// <summary>Number of labels in legend-box.
        /// Default 1.</summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public int LegendBoxNumEntries
        {
            get { return _legendBoxNumEntries; }
            set
            {
                lock (Lock)
                {
                    _legendBoxNumEntries = value;
                    // Update dependencies:
                    vtkLegendBoxActor legendBoxActor = _legendBoxActor;
                    if (legendBoxActor != null)
                    {
                        legendBoxActor.SetNumberOfEntries(_legendBoxNumEntries);
                    }
                }
            }
        }

        private string[] _legendBoxTitles = new string [1] {"Title 1"};

        /// <summary>A table of Titles for LegendBox
        /// Default is "Title 1"</summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public string[] LegendBoxTitles
        {
            get { return _legendBoxTitles; }
            set
            {
                lock (Lock)
                {
                    _legendBoxTitles = value;
                    // Update dependencies:
                    vtkLegendBoxActor legendBoxActor = _legendBoxActor;
                    if (legendBoxActor != null)
                    {
                        for (int i = 0; i < LegendBoxNumEntries; i++)
                        {
                            legendBoxActor.SetEntryString(i, _legendBoxTitles[i]);
                        } 
                    }
                }
            }
        }

        
        private vtkPolyData _symbol;
        
        /// <summary>A table of symbols for LegendBox
        /// Default is "Sphere symbol"</summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public vtkPolyData Symbol
        {
            get
            {
                if (_symbol == null)
                {
                    Symbol = vtkPolyData.New();
                }
                return _symbol;
            }
            set
            {
                lock (Lock)
                {
                    if (value != null && value != _symbol)
                    {
                        vtkSphereSource sphere = vtkSphereSource.New();                      
                        value = sphere.GetOutput();
                        
                    }
                    _symbol = value;
                }
            }
        }

        #endregion


        #region Decorations.ScalarBar


        vtkScalarBarActor _scalarBarActor;

        public vtkScalarBarActor ScalarBarActor
        {
            get
            {
                if (_scalarBarActor == null)
                {
                    ScalarBarActor = vtkScalarBarActor.New();
                }
                if (_scalarBarActor!=null)
                {
                    if (ShowScalarBar)
                        _scalarBarActor.SetVisibility(1);
                    else
                        _scalarBarActor.SetVisibility(0);
                }
                return _scalarBarActor;
            }
            protected set
            {
                lock (Lock)
                {
                    if (value != null && value != _scalarBarActor)
                    {
                        // Set axes actor's properties a set internally on the object:
                        //value.SetFlyMode((int)CubeAxesFlyMode);
                        if (ShowScalarBar)
                            value.SetVisibility(1);
                        else
                            value.SetVisibility(0);
                        
                        vtkProperty2D prop2D = value.GetProperty();

                        value.SetTitle(ScalarBarTitle);
                        value.SetNumberOfLabels(ScalarBarNumberOfLabels);
                        value.SetOrientation(ScalarBarOrientation);
                        value.SetPosition(ScalarBarXPosition, ScalarBarYPosition);
                        value.SetHeight(ScalarBarHeight);
                        value.SetWidth(ScalarBarWidth);
                        value.SetLookupTable(ScalarBarLookupTable);

                        // $$ UPDATE OTHER PROPERTIES IF ANY ARE ADDED!

                        ////// Add actor to the decoration renderer:
                        //vtkRenderer renderer = DecorationRenderer;
                        //if (renderer != null)
                        //{
                        //    if (!UtilVtk.ContainsActor2D(renderer, value))
                        //        renderer.AddActor2D(value);

                        //    //value.SetCamera(renderer.GetActiveCamera());
                        //}

                    }
                    _scalarBarActor = value;
                }
            }
        }


        private vtkLookupTable _scalarBarLookupTable;


        public vtkLookupTable ScalarBarLookupTable
        {
            get {
                if (_scalarBarLookupTable == null)
                {
                    ScalarBarLookupTable = vtkLookupTable.New();
                }
                return _scalarBarLookupTable; 
            }
            set
            {
                lock (Lock)
                {
                    if (value != null && value != _scalarBarLookupTable)
                    {
                        value.SetNumberOfColors(LookUpTableNumColors);
                        value.SetTableRange(LookUpTableMinRange, LookUpTableMaxRange);
                        value.SetValueRange(LookUpTableMinValue, LookUpTableMaxValue);
                        value.SetNumberOfTableValues(LookUpTableNumTableValues);
                        value.SetSaturationRange(LookUpTableMinSaturation, LookUpTableMaxSaturation);
                        value.SetHueRange(LookUpTableMinHue, LookUpTableMaxHue);
                        value.SetAlpha(LookUpTableAlpha);
                        if ((LookUpTableMinRange == 0) && (LookUpTableMaxRange == 0))
                            UtilVtk.LookUpTableRange(LookUpTableColorScale, LookUpTableNumTableValues, ref value);
                        else
                            UtilVtk.LookUpTableRange(LookUpTableColorScale, LookUpTableNumTableValues, LookUpTableMinRange,
                            LookUpTableMaxRange, ref value);
                        //for (int i = 0; i < LookUpTableNumTableValues; i++)
                        //{
                        //    double val = LookUpTableMinRange + (i * ((LookUpTableMaxRange - LookUpTableMinRange) / LookUpTableNumTableValues));
                        //    color col = LookUpTableColorScale.GetColor(val);
                        //    value.SetTableValue(i, col.R, col.G, col.B, col.Opacity);
                        //}
                    }
                    _scalarBarLookupTable = value;
                }
            }
        }


        #region LookUpTable


        private ColorScale _lookUpTableColorScale; //= ColorScale.CreateRainbow(min, max);

        /// <summary>Color scale for lookuptable.
        /// Default is rainbow. </summary>
        /// $A Igor Oct11, Tako78 Dec22;
        public ColorScale LookUpTableColorScale
        {
            get
            {
                if (_lookUpTableColorScale == null)
                    LookUpTableColorScale = ColorScale.CreateRainbow(min, max);
                return _lookUpTableColorScale;
            }
            set
            {
                lock (Lock)
                {
                    _lookUpTableColorScale = value;
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                    {
                        if ((LookUpTableMinRange == 0) && (LookUpTableMaxRange == 0))
                            UtilVtk.LookUpTableRange(LookUpTableColorScale, LookUpTableNumTableValues, ref lookUpTable);
                        else
                            UtilVtk.LookUpTableRange(LookUpTableColorScale, LookUpTableNumTableValues, LookUpTableMinRange,
                            LookUpTableMaxRange, ref lookUpTable);
                        //for (int i = 0; i < LookUpTableNumTableValues; i++)
                        //{
                        //    double val = LookUpTableMinRange + (i * ((LookUpTableMaxRange - LookUpTableMinRange) / LookUpTableNumTableValues));
                        //    color col = LookUpTableColorScale.GetColor(val);
                        //    lookUpTable.SetTableValue(i, col.R, col.G, col.B, col.Opacity);
                        //}
                    }
                    
                }
            }
        }

        private int _lookUpTableNumTableValues = 100;

        /// <summary>Number of table values.
        /// Default is 100. </summary>
        /// $A Igor Oct11, Tako78 Dec22;
        public int LookUpTableNumTableValues
        {
            get { return _lookUpTableNumTableValues; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableNumTableValues = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetNumberOfTableValues(_lookUpTableNumTableValues);
                }
            }
        }
        
        private double _lookUpTableAlpha = 1;

        /// <summary>Alpha value for lookuptable.
        /// Default is 1. </summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public double LookUpTableAlpha
        {
            get { return _lookUpTableAlpha; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableAlpha = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetAlpha(_lookUpTableAlpha);
                }
            }
        }

        private double _lookUpTableMinHue = 0;

        /// <summary>Minimum hue value for lookuptable.
        /// Default is 0. </summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public double LookUpTableMinHue
        {
            get { return _lookUpTableMinHue; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableMinHue = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetHueRange(_lookUpTableMinHue, _lookUpTableMaxHue);
                }
            }
        }

        private double _lookUpTableMaxHue = 1;

        /// <summary>Minimum hue value for lookuptable.
        /// Default is 1. </summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public double LookUpTableMaxHue
        {
            get { return _lookUpTableMaxHue; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableMaxHue = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetHueRange(_lookUpTableMinHue, _lookUpTableMaxHue);
                }
            }
        }

        private double _lookUpTableMinSaturation = 0;

        /// <summary>Minimum saturation value for lookuptable.
        /// Default is 0. </summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public double LookUpTableMinSaturation
        {
            get { return _lookUpTableMinSaturation; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableMinSaturation = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetSaturationRange(_lookUpTableMinSaturation, _lookUpTableMaxSaturation);
                }
            }
        }
        
        private double _lookUpTableMaxSaturation = 1;

        /// <summary>Maximum saturation value for lookuptable.
        /// Default is 1. </summary>
        /// $A Igor Oct11, Tako78 Dec23;
        public double LookUpTableMaxSaturation
        {
            get { return _lookUpTableMaxSaturation; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableMaxSaturation = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetSaturationRange(_lookUpTableMinSaturation, _lookUpTableMaxSaturation);
                }
            }
        }


        static double min = 0;
        static double max = 1;
        private double _lookUpTableMinValue;

        /// <summary>Minimum value for lookuptable.
        /// Default is 0. </summary>
        /// $A Igor Oct11, Tako78 Dec22;
        public double LookUpTableMinValue
        {
            get { return _lookUpTableMinValue; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableMinValue = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetValueRange(_lookUpTableMinValue, _lookUpTableMaxValue);
                }
            }
        }

        private double _lookUpTableMaxValue;

        /// <summary>Maximum value for lookuptable.
        /// Default is 1. </summary>
        /// $A Igor Oct11, Tako78 Dec22;
        public double LookUpTableMaxValue
        {
            get { return _lookUpTableMaxValue; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableMaxValue = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetValueRange(_lookUpTableMinValue, _lookUpTableMaxValue);
                }
            }
        }

        private double _lookUpTableMinRange;

        /// <summary>Minimum range for lookuptable.
        /// Default is 0. </summary>
        /// $A Igor Oct11, Tako78 Dec22;
        public double LookUpTableMinRange
        {
            get { return _lookUpTableMinRange; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableMinRange = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetTableRange(_lookUpTableMinRange, _lookUpTableMaxRange);
                }
            }
        }

        private double _lookUpTableMaxRange;

        /// <summary>Maximum range for lookuptable.
        /// Default is 1. </summary>
        /// $A Igor Oct11, Tako78 Dec22;
        public double LookUpTableMaxRange
        {
            get { return _lookUpTableMaxRange; }
            set
            {
                lock (Lock)
                {
                    _lookUpTableMaxRange = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)

                        lookUpTable.SetTableRange(_lookUpTableMinRange, _lookUpTableMaxRange);
                }
            }
        }

        private int _lookupTableNumColors = 100;

        /// <summary>Number fo collors in lookuptable.
        /// Default is 100. </summary>
        /// $A Igor Oct11, Tako78 Dec22;
        public int LookUpTableNumColors
        {
            get { return _lookupTableNumColors; }
            set
            {
                lock (Lock)
                {
                    _lookupTableNumColors = value;
                    // Update dependencies:
                    vtkLookupTable lookUpTable = _scalarBarLookupTable;
                    if (lookUpTable != null)
                        lookUpTable.SetNumberOfColors(_lookupTableNumColors);
                }
            }
        }


        #endregion


        private bool _showSclarBar = false;

        /// <summary>Whether scalar-bar is shown or not.
        /// Default is true.</summary>
        /// $A Igor Oct11, Tako78 Dec13;
        public bool ShowScalarBar
        {
            get { return _showSclarBar; }
            set
            {
                lock (Lock)
                {
                    _showSclarBar = value;
                    // Update dependencies:
                    vtkScalarBarActor actor2D = _scalarBarActor;
                    if (actor2D != null)
                    {
                        if (value == true)
                            actor2D.SetVisibility(1);
                        else
                            actor2D.SetVisibility(0);
                    }
                }
            }
        }

        private string _scalarBarTitle = "Values";

        /// <summary>Title for scalar bar.</summary>
        /// $A Igor Oct11, Tako78 Dec13;
        public string ScalarBarTitle
        {
            get { return _scalarBarTitle; }
            set
            {
                lock (Lock)
                {
                    _scalarBarTitle = value;
                    // Update dependencies:
                    vtkScalarBarActor actor2D = _scalarBarActor;
                    if (actor2D != null)
                        actor2D.SetTitle(_scalarBarTitle);
                }
            }
        }

        private int _scalarBarNumberOfLabels;

        /// <summary>Numer of labels on the scalar bar.</summary>
        /// $A Igor Oct11, Tako78 Dec13;
        public int ScalarBarNumberOfLabels
        {
            get { return _scalarBarNumberOfLabels; }
            set
            {
                lock (Lock)
                {
                    _scalarBarNumberOfLabels = value;
                    // Update dependencies:
                    vtkScalarBarActor actor2D = _scalarBarActor;
                    if (actor2D != null)
                        actor2D.SetNumberOfLabels(_scalarBarNumberOfLabels);
                }
            }
        }

        private int _scalarBarOrientation;

        /// <summary>Orientation of the scalar bar.
        /// 0 - Horizontal; 1 - Vertical</summary>
        /// $A Igor Oct11, Tako78 Dec13;
        public int ScalarBarOrientation
        {
            get { return _scalarBarOrientation; }
            set
            {
                lock (Lock)
                {
                    _scalarBarOrientation = value;
                    // Update dependencies:
                    vtkScalarBarActor actor2D = _scalarBarActor;
                    if (actor2D != null)
                    {
                        actor2D.SetOrientation(_scalarBarOrientation);
                    }
                }
            }
        }


        private double _scalarBarXPosition = 0.25;
        private double _scalarBarYPosition = 0.01;

        /// <summary>Position X of the scalar bar.</summary>
        /// $A Igor Oct11, Tako78 Dec14;
        public double ScalarBarXPosition
        {
            get { return _scalarBarXPosition; }
            set
            {
                lock (Lock)
                {
                    _scalarBarXPosition = value;
                    // Update dependencies:
                    vtkScalarBarActor actor2D = _scalarBarActor;
                    if (actor2D != null)
                    {
                        actor2D.SetPosition(_scalarBarXPosition, _scalarBarYPosition);
                    }
                }
            }
        }

        /// <summary>Position Y of the scalar bar.</summary>
        /// $A Igor Oct11, Tako78 Dec14;
        public double ScalarBarYPosition
        {
            get { return _scalarBarYPosition; }
            set
            {
                lock (Lock)
                {
                    _scalarBarYPosition = value;
                    // Update dependencies:
                    vtkScalarBarActor actor2D = _scalarBarActor;
                    if (actor2D != null)
                    {
                        actor2D.SetPosition(_scalarBarXPosition, _scalarBarYPosition);
                    }
                }
            }
        }

        private double _scalarBarHeight = 0.1;

        /// <summary>Hight of the scalar bar.</summary>
        /// $A Igor Oct11, Tako78 Dec13;
        public double ScalarBarHeight
        {
            get { return _scalarBarHeight; }
            set
            {
                lock (Lock)
                {
                    _scalarBarHeight = value;
                    // Update dependencies:
                    vtkScalarBarActor actor2D = _scalarBarActor;
                    if (actor2D != null)
                        actor2D.SetHeight(_scalarBarHeight);
                }
            }
        }

        private double _scalarBarWidth = 0.5;

        /// <summary>Width of the scalar bar.</summary>
        /// $A Igor Oct11, Tako78 Dec13;
        public double ScalarBarWidth
        {
            get { return _scalarBarWidth; }
            set
            {
                lock (Lock)
                {
                    _scalarBarWidth = value;
                    // Update dependencies:
                    vtkScalarBarActor actor2D = _scalarBarActor;
                    if (actor2D != null)
                        actor2D.SetWidth(_scalarBarWidth);
                }
            }
        }


        #endregion


        #region Decorations.CubeAxes

        protected vtkCubeAxesActor _cubeAxesActor;

        /// <summary>Actor that is used for manipulating cube axes.
        /// <para>Warning:</para>
        /// The returned actor can eventually be used to set properties of the axes that are not 
        /// supported by the current handler object. However, it is recommended to avoid such use 
        /// whenever possible because such properties may be overridden unpredictedly by the current 
        /// helper object.</summary>
        public vtkCubeAxesActor CubeAxesActor
        {
            get
            {
                if (_cubeAxesActor == null)
                {
                    CubeAxesActor = vtkCubeAxesActor.New();
                }
                return _cubeAxesActor;
            }
            protected set
            {
                lock (Lock)
                {
                    if (value != null && value != _cubeAxesActor)
                    {
                        // Set axes actor's properties a set internally on the object:
                        value.SetFlyMode((int)CubeAxesFlyMode);
                        if (ShowCubeAxes)
                            value.SetVisibility(1);
                        else
                            value.SetVisibility(0);
                        vtkProperty prop = value.GetProperty();
                        prop.SetColor(CubeAxesColor.R, CubeAxesColor.G, CubeAxesColor.B);
                        prop.SetOpacity(CubeAxesColor.Opacity);
                        prop.SetLineWidth((float)CubeAxesWidth);
                        value.SetXTitle(CubeAxesXLabel);
                        value.SetYTitle(CubeAxesYLabel);
                        value.SetZTitle(CubeAxesZLabel);
                        value.SetScale(_actorXScale, _actorYScale, _actorZScale);

                        // $$ UPDATE OTHER PROPERTIES IF ANY ARE ADDED!
                        
                        //// Add actor to the decoration renderer:
                        //vtkRenderer renderer = DecorationRenderer;
                        //if (renderer != null)
                        //{
                        //    if (!UtilVtk.ContainsActor(renderer, value))
                        //        renderer.AddActor(value);
                        //    value.SetCamera(renderer.GetActiveCamera());
                        //}

                    }
                    _cubeAxesActor = value;
                }
            }
        }

        /// <summary>Gets the Axes' properties, through wihich the caller can set additional
        /// properties that are not enabled by the current helper object.
        /// <para>Warning:</para>
        /// Use this only exceptionally.
        /// Changes made on the returned object may be overridden unpredictedly by the current helper object.</summary>
        public vtkProperty CubeAxesProperties
        {
            get
            {
                if (CubeAxesActor != null)
                {
                    vtkProperty prop = CubeAxesActor.GetProperty();
                    return prop;
                }
                return null;
            }
        }

        private string _cubeAxesXLabel;

        /// <summary>Label for X axis.</summary>
        public string CubeAxesXLabel
        {
            get { return _cubeAxesXLabel; }
            set
            {
                lock (Lock)
                {
                    _cubeAxesXLabel = value;
                    // Update dependencies:
                    vtkCubeAxesActor actor = _cubeAxesActor;
                    if (actor != null)
                        actor.SetXTitle(_cubeAxesXLabel);
                }
            }
        }

        private string _cubeAxesYLabel;

        /// <summary>Label for Y axis.</summary>
        public string CubeAxesYLabel
        {
            get { return _cubeAxesYLabel; }
            set
            {
                lock (Lock)
                {
                    _cubeAxesYLabel = value;
                    // Update dependencies:
                    vtkCubeAxesActor actor = _cubeAxesActor;
                    if (actor != null)
                        actor.SetYTitle(_cubeAxesYLabel);
                }
            }
        }

        private string _cubeAxesZLabel;

        /// <summary>Label for Z axis.</summary>
        public string CubeAxesZLabel
        {
            get { return _cubeAxesZLabel; }
            set
            {
                lock (Lock)
                {
                    _cubeAxesZLabel = value;
                    // Update dependencies:
                    vtkCubeAxesActor actor = _cubeAxesActor;
                    if (actor != null)
                        actor.SetZTitle(_cubeAxesZLabel);
                }
            }
        }

        public void SetCubeAxesLabels(string xLabel, string yLable, string zLabel)
        {
            CubeAxesXLabel = xLabel;
            CubeAxesYLabel = yLable;
            CubeAxesZLabel = zLabel;
        }

        private double _actorXScale;
        private double _actorYScale;
        private double _actorZScale;

        public void SetActorScale(double X, double Y, double Z)
        {
            _actorXScale = X;
            _actorYScale = Y;
            _actorZScale = Z;

            vtkCubeAxesActor actor = _cubeAxesActor;
            if (actor != null)
                actor.SetScale(_actorXScale, _actorYScale, _actorZScale);
        }

        private VtkFlyMode _cubeAxesFlyMode = VtkFlyMode.Static;

        /// <summary>Positioning mode ("fly mode") of the axes.
        /// Default is <see cref="VtkFlyMode.Static"/>.</summary>
        public VtkFlyMode CubeAxesFlyMode
        {
            get { return _cubeAxesFlyMode; }
            set
            {
                lock (Lock)
                {
                    _cubeAxesFlyMode = value;
                    // Update dependencies:
                    vtkCubeAxesActor actor = _cubeAxesActor;
                    if (actor != null)
                        actor.SetFlyMode((int)_cubeAxesFlyMode);
                }
            }
        }

        private bool _showCubeAxes = true;

        /// <summary>Whether axes are shown or not.
        /// Default is true.</summary>
        public bool ShowCubeAxes
        {
            get { return _showCubeAxes; }
            set
            {
                lock (Lock)
                {
                    _showCubeAxes = value;
                    // Update dependencies:
                    vtkCubeAxesActor actor = _cubeAxesActor;
                    if (actor != null)
                    {
                        if (value)
                            actor.SetVisibility(1);
                        else
                            actor.SetVisibility(0);
                    }
                }
            }
        }

        private color _cubeAxesColor = new color(System.Drawing.Color.Gray);

        /// <summary>Gets or sets axes color.</summary>
        public color CubeAxesColor
        {
            get { return _cubeAxesColor; }
            set
            {
                lock (Lock)
                {
                    _cubeAxesColor = value;
                    // Update dependencies:
                    vtkCubeAxesActor actor = _cubeAxesActor;
                    if (actor != null)
                    {
                        vtkProperty prop = actor.GetProperty();
                        prop.SetColor(value.R, value.G, value.B);
                        prop.SetOpacity(value.Opacity);
                    }
                }
            }
        }

        private double _cubeAxesWidth = 1;

        /// <summary>Gets or sets width of the axes lines.</summary>
        public double CubeAxesWidth
        {
            get { return _cubeAxesWidth; }
            set
            {
                lock (Lock)
                {
                    _cubeAxesWidth = value;
                    // Update dependencies:
                    vtkCubeAxesActor actor = _cubeAxesActor;
                    if (actor != null)
                    {
                        vtkProperty prop = actor.GetProperty();
                        prop.SetLineWidth((float)value);
                    }
                }

            }
        }

        #endregion Decorations.CubeAxes


        #region Operation

        private bool _updateWhenConstructed = true;

        /// <summary>Whether decorations to be drawn are added to the VTK window's renderer immediately 
        /// after the current decoration handler object is constructed.</summary>
        public bool UpdateWhenConstructed
        {
            get { return _updateWhenConstructed; }
            set { _updateWhenConstructed = value; }
        }


        
        /// <summary>Adds all actors from the current decoration handler to the plotter that it is assigned to.</summary>
        public void AddActorsToPlotter()
        {
            lock (Lock)
            {
                if (this.Plotter != null)
                    AddActorsToPlotter(this.Plotter);
            }
        }

        /// <summary>Adds all actors from the current plot to the specified plotter.</summary>
        /// <param name="plotter">Plotter to which actors from the current plot are added.</param>
        public void AddActorsToPlotter(VtkPlotter plotter)
        {
            if (plotter == null)
                throw new ArgumentException("Plotter to add decoration actors to is not specified (null reference).");
            lock (Lock)
            {
                if (plotter != null)
                {
                    VtkPlotter.Accessor accessor = new VtkPlotter.Accessor(plotter);
                    vtkRenderer renderer = accessor.Renderer;
                    if (renderer!=null)
                        AddActorsToRenderer(renderer);

                    //if (ShowScalarBar)
                    //{
                    //    vtkLookupTable tmp = ScalarBarLookupTable;
                    //    vtkScalarBarActor scalarBarActor = this.ScalarBarActor;
                    //    if (scalarBarActor != null)
                    //    {
                    //        plotter.AddActor2D(scalarBarActor);
                    //    }
                    //}
                    //if (ShowLegendBox)
                    //{
                    //    vtkLegendBoxActor legendActor = this.LegendBoxActor;
                    //    if (legendActor != null)
                    //    {
                    //        plotter.AddActor2D(legendActor);
                    //    }
                    //}
                    //if (ShowCubeAxes)
                    //{
                    //    vtkCubeAxesActor axesActor = this.CubeAxesActor;
                    //    if (CubeAxesActor != null)
                    //    {
                    //        plotter.AddActor(axesActor);
                    //        if (renderer!=null)
                    //            axesActor.SetCamera(renderer.GetActiveCamera());
                    //    }
                    //}
                    //// $$ ADD OTHER DECORATION ACTORS IF ANY ARE ADDED!

                }
            }
        }

        
        /// <summary>Adds all actors from the current decoraton handler to its current renderer.</summary>
        public void AddActorsToRenderer()
        {
            vtkRenderer renderer = DecorationRenderer;
            if (renderer!=null)
                AddActorsToRenderer(renderer);
        }

        /// <summary>Adds all actors from the current decoration handler to the specified rendeerr.</summary>
        /// <param name="renderer">VTK renderer to which actors from the current plot are added.</param>
        public void AddActorsToRenderer(vtkRenderer renderer)
        {
            lock (Lock)
            {
                if (renderer == null)
                    throw new ArgumentException("Plotter to add decoration actors to is not specified (null reference).");
                if (renderer != null)
                {
                    if (ShowScalarBar)
                    {
                        //vtkScalarBarActor sba;
                        vtkLookupTable tmp = ScalarBarLookupTable;
                        vtkScalarBarActor scalarBarActor = this.ScalarBarActor;
                        if (scalarBarActor != null)
                        {
                            renderer.AddActor2D(scalarBarActor);
                        }
                    }
                    if (ShowLegendBox)
                    {
                        vtkLegendBoxActor legendActor = this.LegendBoxActor;
                        if (legendActor != null)
                        {
                            renderer.AddActor2D(legendActor);
                        }
                    }
                    if (ShowCubeAxes)
                    {
                        vtkCubeAxesActor axesActor = this.CubeAxesActor;
                        if (CubeAxesActor != null)
                        {
                            renderer.AddActor(axesActor);
                            axesActor.SetCamera(renderer.GetActiveCamera());
                        }
                    }

                    // $$ ADD OTHER DECORATION ACTORS IF ANY ARE ADDED!

                }
            }
        }
        /// <summary>Updates decorations.</summary>
        public virtual void Update()
        {
            if (!IsBoundsUpdated)
                UpdateBoundsInternal();

            if (_decorationRenderer != null)
                AddActorsToRenderer();
            else if (Plotter != null)
                AddActorsToPlotter();
            else if (DecorationRenderer != null)
                AddActorsToPlotter();

            //// Statement below will create axes actor and insert it on the renderer used for 
            //// rendering decorations:
            //vtkLookupTable tmp = ScalarBarLookupTable;
            //vtkScalarBarActor scalarBarActor = ScalarBarActor;
            //vtkLegendBoxActor legendBoxActor = LegendBoxActor;
            //vtkCubeAxesActor axesAxtor = CubeAxesActor;

        }

        #endregion Operation

    }  // class VtkAxisHandler

}
