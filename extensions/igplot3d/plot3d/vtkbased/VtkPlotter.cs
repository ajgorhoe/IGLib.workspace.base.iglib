// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using IG.Lib;
using IG.Num;

using Kitware.VTK;


namespace IG.Gr3d
{

    /// <summary>Plotter class that uses a <see cref="vtkRenderWindow"/> object for plotting 3D graphics.</summary>
    /// $A Igor xx Oct11;
    public class VtkPlotter : ILockable, IDisposable
    {

        #region Constructors

        public VtkPlotter() {  }

        /// <summary>Constructor.
        /// <para>If a VTK window used for plotting is not specified (i.e., it is null) then a new window will be opened.</para></summary>
        /// <param name="window">VTK window to be used for plotting.</param>
        public VtkPlotter(vtkRenderWindow window)
        {
            //if (window == null)
            //    throw new ArgumentException("VTK window used for rendering 3D graphics is not specified.");
            this.Window = window;
        }

        /// <summary>Constructor.</summary>
        /// <param name="formContainer">VTK form container to be used for access to VTK render window for plotting.</param>
        public VtkPlotter(IVtkFormContainer formContainer)
        {
            //if (formContainer == null)
            //    throw new ArgumentException("VTK window used for rendering 3D graphics is not specified.");
            this.FormContainer = FormContainer;
        }

        /// <summary>Constructor.</summary>
        /// <param name="formContainer">VTK control used for plotting.</param>
        public VtkPlotter(Kitware.VTK.RenderWindowControl renderControl)
        {
            //if (renderControl == null)
            //    throw new ArgumentException("VTK window used for rendering 3D graphics is not specified.");
            this.RenderControl = RenderControl;
        }

        #endregion Constructors


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking


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


        private bool _isWindowStandalone = true;

        private int _windowSizeX = 600, _windowSizey = 600;

        private int _windowPositionX = 100, _windowPositionY = 50;

        /// <summary>Whether a stand-alone window is launched to render the scene.</summary>
        public bool IsWindowStandalone
        { get { return _isWindowStandalone; } set { _isWindowStandalone = value; } }

        /// <summary>Window size in x direction (used when a stand-alone window is launched for rendering).</summary>
        public int WindowSizeX
        { get { return _windowSizeX; } set { _windowSizeX = value; } }

        /// <summary>Window size in y direction (used when a stand-alone window is launched for rendering).</summary>
        public int WindowSizeY
        { get { return _windowSizey; } set { _windowSizey = value; } }

        /// <summary>Window position in x direction (used when a stand-alone window is launched for rendering).</summary>
        public int WindowPositionX
        { get { return _windowPositionX; } set { _windowPositionX = value; } }

        /// <summary>Window position in y direction (used when a stand-alone window is launched for rendering).</summary>
        public int WindowPositionY
        { get { return _windowPositionY; } set { _windowPositionY = value; } }



        /// <summary>Default background color for new windows.</summary>
        public static color DefaultBackground = new color(1,1,1);

        color _backGround = DefaultBackground;

        /// <summary>Background color for the current plotter.
        /// <para>If plotter is attached to a window that already has a renderer then background color of that renderer is set.
        /// If a renderer is created anew, this background color is assumed.</para></summary>
        public color BackGround
        {
            get { return _backGround; }
            set {
                _backGround = value;
                if (Renderer != null)
                    Renderer.SetBackground(value.R, value.G, value.B);
            }
        }

        /// <summary> Resets the camera position and shows complete plots. </summary>
        /// $A Tako78 Oct12;
        public void ResetCamera()
        {
            if (Renderer != null)
                Renderer.ResetCamera();
        }

        #endregion Settings


        #region Data


        private vtkRenderWindow _window;

        /// <summary>VTK window used for rendering 3D graphics.
        /// Warning: setter should only be used in constructors.</summary>
        protected vtkRenderWindow Window
        {
            get
            {
                if (_window == null)
                {
                    if (RenderControl != null)
                    {
                        _window = RenderControl.RenderWindow;
                    }
                    if (_window == null)
                    {
                        // Last resort: could not get a VTK render window through a container form 
                        // (either existent or created anew), create a plain VTK window:
                        IsWindowStandalone = true;
                        vtkRenderWindow win = vtkRenderWindow.New();
                        win.SetSize(WindowSizeX, WindowSizeY);
                        win.SetPosition(WindowPositionX, WindowPositionY);
                        if (_activeRenderer != null)
                        {
                            win.AddRenderer(_activeRenderer);
                        }
                        else
                        {
                            _activeRenderer = vtkRenderer.New();
                            win.AddRenderer(_activeRenderer);
                        }
                        Window = win;
                    }
                    //else
                    //    throw new InvalidOperationException("VTK window is not specified although it is not specified as stand alone.");
                }

                return _window;
            }
            set {
                if (value != _window)
                {
                    _window = value;
                    if (_decorationHandler != null)
                        _decorationHandler.ChangeWindow(value);
                    else if (_window!=null)
                        _decorationHandler = new VtkDecorationHandler(_window);
                }
            }
        }


        /// <summary>Sets the VTK render window.
        /// <para>Use only in exceptional cases!</para></summary>
        /// <param name="win">Window to be set.</param>
        public void SetWindow(vtkRenderWindow win)
        {
            this.Window = win;
        }

        #region WindowsControl

        // Definitions for standalone Windows form:

        /// <summary>Default value for flag indicating whether the <see cref="VtkForm"/> is used for standalone VTK windows.</summary>
        public static bool DefaultIsAllowedVtkForm = false;

        /// <summary>Default value for flag indicating whether the <see cref="VtkFormPlain"/> is used for stand-alone VTK windows.</summary>
        public static bool DefaultIsAllowedVtkFormPlain = true;
                           

        /// <summary>Modifies or retrieves a boolean value indicating whether any VTK form can be used 
        /// by plotters of the current type for standalone VTK windows by default.</summary>
        public static bool DefaultIsAllowedAnyVtkForm
        {
            get { return (DefaultIsAllowedVtkForm || DefaultIsAllowedVtkFormPlain); }
            set
            {
                bool nowAllowed = DefaultIsAllowedAnyVtkForm;
                if (value == true)
                {
                    if (!nowAllowed)
                    {
                        DefaultIsAllowedVtkForm = true;
                    }
                }
                else
                {
                    DefaultIsAllowedVtkForm = false;
                    DefaultIsAllowedVtkFormPlain = false;
                }
            }
        }

        /// <summary>Modifies or retrieves a boolean value indicating whether any VTK form can be used 
        /// for standalone VTK windows by the current plotter.</summary>
        public bool IsAllowedAnyVtkForm
        {
            get { return (IsAllowedVtkForm || IsAllowedVtkFormPlain); }
            set
            {
                bool nowAllowed = IsAllowedAnyVtkForm;
                if (value == true)
                {
                    if (!nowAllowed)
                    {
                        IsAllowedVtkForm = true;
                    }
                } else
                {
                    IsAllowedVtkForm = false;
                    IsAllowedVtkFormPlain = false;
                }
            }
        }

        protected bool _isAllowedVtkForm = DefaultIsAllowedVtkForm;

        /// <summary>Whether the <see cref="VtkForm"/> is used for standalone VTK windows.</summary>
        public bool IsAllowedVtkForm
        {
            get { return _isAllowedVtkForm; }
            set
            {
                _isAllowedVtkForm = value;
                // Update dependencies:
                if (value == true)
                    IsAllowedVtkFormPlain = false;
            }
        }

        protected bool _isAllowedVtkFormPlain = DefaultIsAllowedVtkFormPlain;

        /// <summary>Whether the <see cref="VtkFormPlain"/> is used for stand-alone VTK windows.</summary>
        public bool IsAllowedVtkFormPlain
        {
            get { return _isAllowedVtkFormPlain; }
            set
            {
                _isAllowedVtkFormPlain = value;
                // Update dependencies:
                if (value == true)
                    IsAllowedVtkForm = false;
            }
        }

        /// <summary>Tries to create and return a valid VTK Form container (of type <see cref="IVtkFormContainer"/>) that
        /// actually contains a VTK renderer control of type <see cref="Kitware.VTK.RenderWindowControl"/>.</summary>
        /// <remarks>
        /// <para>Which type of forms are attempted to create is specified by properties like <see cref="IsAllowedVtkForm"/>
        /// and <see cref="IsAllowedVtkFormPlain"/>.</para>
        /// <para>In some settings of conditional compilation, classes that should contain a proper VTK renderer control
        /// actually don't contain it. Therefore, this must be verified and a non-null object is returned only if it
        /// actually contains the proper VTK render control.</para></remarks>
        public IVtkFormContainer CreateVtkFormContainer()
        {
            IVtkFormContainer ret = null;
            if (IsAllowedAnyVtkForm)
            {
                if (ret == null && IsAllowedVtkForm)
                {
                    ret = new VtkForm();
                    if (ret.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl == null)
                        ret = null;
                }
                if (ret == null && IsAllowedVtkFormPlain)
                {
                    ret = new VtkFormPlain();
                    if (ret.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl == null)
                        ret = null;
                }
            }
            return ret;
        }


        private IVtkFormContainer _formContainer;

        /// <summary>Form that eventually contains VTK render control (of type <see cref="Kitware.VTK.RenderWindowControl"/>).</summary>
        protected IVtkFormContainer FormContainer
        {
            get
            {
                return _formContainer;
            }
            set
            {
                if (value != _formContainer)
                {
                    //if (value != null)
                    //{
                    //    if (value.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl == null)
                    //    {
                    //        // The IVtkFormContainer provided does not vontain the property VtkRenderWindowControl
                    //        // that would be of correct type, don't set the value.
                    //        value = null;
                    //    }
                    //}
                    _formContainer = value;
                    // Invalidate dependencies:
                    RenderControl = null;
                }
            }
        }

        private Kitware.VTK.RenderWindowControl _renderControl;

        /// <summary>VTK control of type <see cref="Kitware.VTK.RenderWindowControl"/> that can be used for
        /// rendering VTK graphics.</summary>
        protected Kitware.VTK.RenderWindowControl RenderControl
        {
            get {
                if (_renderControl == null)
                {
                    if (FormContainer != null)
                    {
                        if (FormContainer.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl != null)
                        {
                            _renderControl = FormContainer.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl;
                        }
                    }
                }
                return _renderControl;
            }
            set
            {
                if (value != _renderControl)
                {
                    _renderControl = value;
                    // Invalidate dependencies:
                    if (value!=null)
                    {
                        if (FormContainer != null)
                        {
                            if (FormContainer.VtkRenderWindowControl != value)
                            {
                                FormContainer = null;
                                IsWindowStandalone = false;
                            }
                        }
                    }
                    Window = null;
                    Renderer = null;
                }
            }
        }

        #endregion WindowsControl


        public VtkDecorationHandler _decorationHandler;

        /// <summary>Decoration handler that handles graph axes, title, labels, etc.</summary>
        public VtkDecorationHandler DecorationHandler
        {
            get {
                lock (Lock)
                {
                    if (_decorationHandler == null)
                    {
                        // vtkRenderWindow win = Window;
                        if (_decorationHandler == null)
                        {
                            //_decorationHandler = new VtkDecorationHandler(this.Window);
                            //_decorationHandler.Plotter = this;

                            _decorationHandler = new VtkDecorationHandler((VtkPlotter) this);
                            //_decorationHandler.ChangeWindow(this.Window);
                            
                        }
                    } 
                    return _decorationHandler;
                }
            }
            protected set {
                lock (Lock)
                {
                    _decorationHandler = value;
                }
            }
        }


        #region Data.PlotObjects

        protected List<VtkPlotBase> _plotObjects;

        /// <summary>List of plotting objects contained on the current class.
        /// <para>Setter is thread safe.</para>
        /// <para>Lazy evaluation - list object is automatically generated when first accessed.</para></summary>
        protected List<VtkPlotBase> PlottingObjects
        {
            get
            {
                if (_plotObjects == null)
                {
                    lock (Lock)
                    {
                        if (_plotObjects == null)
                            PlottingObjects = new List<VtkPlotBase>();
                    }
                }
                return _plotObjects;
            }
            set { lock (Lock) { _plotObjects = value; } }
        }

        /// <summary>Returns true if the specified VTK plotting object is contained on (registered with) the current 
        /// <see cref="VtkPlotter"/> object, or false otherwise.</summary>
        /// <param name="plotObject">Plotting object to be checked.</param>
        public bool ContainsPlotObject(VtkPlotBase plotObject)
        {
            if (plotObject == null)
                throw new ArgumentNullException("plotObject", "VTK plotting object not specified (null reference).");
            lock(Lock)
            {
                return PlottingObjects.Contains(plotObject);
            }
        }

        /// <summary>Adds the specified plotting object to the list of plotting objects of the current
        /// VTK plotter.
        /// <para>If the object is already on the list of plotting objects then it is not inserted again.</para></summary>
        /// <param name="plotObject">VTK plotting object to be added on the currrent <see cref="VtkPlotter"/> object.</param>
        public void AddPlotObject(VtkPlotBase plotObject)
        {
            if (plotObject == null)
                throw new ArgumentNullException("plotObject", "VTK plotting object not specified (null reference).");
            lock (Lock)
            {
                if (!PlottingObjects.Contains(plotObject))
                {
                    PlottingObjects.Add(plotObject);
                }
            }
        }

        /// <summary>Adds the specified plotting objects to the list of plotting objects of the current
        /// VTK plotter.</summary>
        /// <param name="plotObjects">Objects to be added to the list.</param>
        public void AddPlotObjects(params VtkPlotBase[] plotObjects)
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

        /// <summary>Removes the specified plotting object from the list of plotting objects of the current
        /// VTK plotter, and disposes unmanaged resources used by that object.
        /// <para>If the specified object is not on the list of plotting objects then nothing happens.</para></summary>
        /// <param name="plotObject">VTK plotting object to be removed from the currrent <see cref="VtkPlotter"/> object.</param>
        public void RemovePlotObject(VtkPlotBase plotObject)
        {
            if (plotObject == null)
                throw new ArgumentNullException("plotObject", "VTK plotting object not specified (null reference).");
            lock (Lock)
            {
                try
                {
                    if (PlottingObjects.Contains(plotObject))
                    {
                        PlottingObjects.Remove(plotObject);
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
        /// VTK plotter, and disposes unmanaged resources used by that objects.
        /// <para>If no objects are specified then nothing happens. Also for the specified objects that are null
        /// or are not on the list, nothing happens. If removing one of the objects throws an exception then the 
        /// remaining objects are removed without any disturbance.</para></summary>
        /// <param name="plotObjects">Objects to be removed from the list.</param>
        public void RemovePlotObjects(params VtkPlotBase[] plotObjects)
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


        #region Data.Renderers

        // Renderers that are used on the current VTK plotter's rendering window

        /// <summary>Returns the number of renderers that are currently attached to the VTK window that is 
        /// associated with the current VTK plotter object.</summary>
        /// <remarks>If htere is currently no VTK rendering windows associated with the plotter, there still can be an
        /// active renderer that has been allocated for adding actors, and in this case 1 is returned.</remarks>
        public int NumRenderers
        {
            get { lock (Lock) { 
                if (_window == null) 
                {
                    if (_activeRenderer!=null)
                        return 1;  // there is no window associated, but there is an active renderer allocated
                    else
                        return 0;
                } else return UtilVtk.GetNumRenderers(_window); 
            } 
            }
        }

        private int _activeRendererIndex = 0;

        /// <summary>Gets or sets the index of the active renderer.
        /// This is the renderer on which mathod such as <see cref="AddActor"/> operate.</summary>
        public int ActiveRendererIndex
        {
            get { lock(Lock) { return _activeRendererIndex; } }
            set 
            {
                lock(Lock)
                {
                    if (value<0)
                        throw new IndexOutOfRangeException("Attempt to set active renderer index less than 0.");
                    else if (value>=NumRenderers)
                        throw new IndexOutOfRangeException("Attempt to set active renderer index greater or equal to the number of renderers ("
                            + NumRenderers + ").");
                    _activeRendererIndex = value;
                    // Invalidate dependencies:
                    _activeRenderer = null;
                }
            }
        }

        private vtkRenderer _activeRenderer;
        
        /// <summary>Gets the active renderer of the current VTK plotter.</summary>
        protected vtkRenderer Renderer
        {
            get
            {
                lock (Lock)
                {
                    if (_activeRenderer == null)
                    {
                        // Note: Creating or adding a renderer may not create a new window!
                        // Check how many renderers there are on a window (if any):
                        int numRenderers = 0;
                        if (_window != null)
                            numRenderers = UtilVtk.GetNumRenderers(_window);
                        if (numRenderers < ActiveRendererIndex + 1)
                        {
                            // Now window is currently associated or it does not have enough renderers:
                            vtkRenderer ren = vtkRenderer.New();
                            ren.SetBackground(BackGround.R, BackGround.G, BackGround.B);
                            if (_window != null)
                            {
                                _window.AddRenderer(ren);
                                int index = UtilVtk.GetNumRenderers(_window)-1;
                                ActiveRendererIndex = index;
                            }
                            _activeRenderer = ren;
                        } else
                        {
                            _activeRenderer = UtilVtk.GetRenderers(Window)[ActiveRendererIndex];
                            // Active renderer was obtained from the existing window, therefore we adapt 
                            // background color from background color of that renderer:
                            if (_activeRenderer != null)
                            {
                                double[] colorComponents = _activeRenderer.GetBackground();
                                _backGround = new color(colorComponents[0], colorComponents[1], colorComponents[2]);
                            }
                        }
                        
                        if (_activeRenderer == null)
                        {
                            if (NumRenderers == 0)
                                throw new InvalidOperationException("Could not get an active renderer. There are no renderers initialized.");
                            else 
                                throw new InvalidOperationException("Could not get an active renderer. VTK plotter might be in inconsistent state.");
                        }
                    }
                    return _activeRenderer;
                }
            }
            set
            {
                lock (Lock) {
                    if (value != _activeRenderer)
                    {
                        _activeRenderer = value;
                        // Invalidate dependencies:
                    }
                }
            }
        }

        /// <summary>Sets the plotter's renderer.
        /// <para>This should be used only exceptionally. Access may be degraded to protected in the future and method accessed
        /// through accessor class.</para></summary>
        /// <param name="renderer">New renderer (can be null).</param>
        public void setRenderer(vtkRenderer renderer)
        {
            this._activeRenderer = renderer;
        }

        /// <summary>Returns index of the specified renderer on the VTK window of the current plotter,
        /// or -1 if the specified renderer is not contaied in the window.
        /// <para>This method can be used for setting active renderer through its reference (use the returned
        /// index in <see cref="ActiveRendererIndex"/>() since there is no direct method to set active 
        /// renderer by its reference.</para></summary>
        /// <param name="renderer">Renderer whose index is returned.</param>
        public int GetRendererIndex(vtkRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer", "Renderer not specified (null reference).");
            lock (Lock)
            {
                List<vtkRenderer> renderers = UtilVtk.GetRenderers(Window);
                for (int i = 0; i < renderers.Count; ++i)
                {
                    if (renderers[i] == renderer)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>Adds the specified renderer to the VTK window of the current plotter object, and returns its
        /// index.
        /// <para>If the specified renderer already exist on the plotter object, then only its index is returned.</para>
        /// <para>If specified so by the <paramref name="setActive"/> parameter then the specified renderer also
        /// becomes the active renderer active after adding.</para></summary>
        /// <param name="renderer">Renderer to be added.</param>
        /// <param name="setActive">If true then the specified renderer is set as active renderer.</param>
        public int AddRenderer(vtkRenderer renderer, bool setActive)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer", "Renderer to be added is not specified (null reference).");
            lock (Lock)
            {
                if (!UtilVtk.ContainsRenderer(Window, renderer))
                {
                    Window.AddRenderer(renderer);
                    int index = UtilVtk.GetNumRenderers(Window)-1;
                    if (setActive)
                        ActiveRendererIndex = index;
                    return index;
                } else
                {
                    int index = GetRendererIndex(renderer);
                    if (setActive)
                        ActiveRendererIndex = index;
                    return index;
                }
            }
        }
        
        #endregion Data.Renderers


        #region Data.Actors

        /// <summary>Adds the specified actor to the active renderer fo the current VTK plotter.</summary>
        /// <param name="actor"></param>
        public void AddActor(vtkActor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException("actor", "VTK actor to be added is not specified (null reference).");
            }
            lock (Lock)
            {
                vtkRenderer ren = this.Renderer;
                if (ren == null)
                {
                    throw new InvalidOperationException("Can not add a VTK actor to the plotter, active renderer is not defined.");
                }
                else
                {
                    //ren.AddActor(actor);
                    if (!UtilVtk.ContainsActor(ren, actor))
                    {
                        ren.AddActor(actor);
                    }
                    else
                    {
                        ; // ren.AddActor(actor);
                    }
                }
            }
        }

        /// <summary>Clears the active renderer (removes all actors on it).</summary>
        public void RemoveAllActors()
        {
            lock (Lock)
            {
                vtkRenderer ren = Renderer;
                if (ren != null)
                {
                    UtilVtk.RemoveAllActors(ren);
                    // ren.Clear();
                }
                if (Window != null)
                {
                    List<vtkRenderer> renderers = UtilVtk.GetRenderers(Window);
                    if (renderers != null)
                    {
                        for (int i = 0; i < renderers.Count; ++i)
                        {
                            vtkRenderer currentRenderer = renderers[i];
                            if (currentRenderer != null)
                            {
                                UtilVtk.RemoveAllActors(currentRenderer);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>Removes the specified actor form the active renderer (if it exists on that renderer) and from any 
        /// other renderer on which it is included (if not found on the active renderer) Nothing happens if the 
        /// actor is not found on any renderer.</summary>
        /// <param name="actor">Actor to be removed.</param>
        public void RemoveActor(vtkActor actor)
        {
            lock (Lock)
            {
                bool removed = false;
                vtkRenderer ren = Renderer;
                if (ren != null)
                {
                    if (UtilVtk.ContainsActor(ren, actor))
                    {
                        ren.RemoveActor(actor);
                        removed = true;
                    }
                }
                if (!removed && Window!=null)
                {
                    List<vtkRenderer> renderers = UtilVtk.GetRenderers(Window);
                    if (renderers != null)
                    {
                        for (int i = 0; i < renderers.Count; ++i)
                        {
                            vtkRenderer currentRenderer = renderers[i];
                            if (UtilVtk.ContainsActor(currentRenderer, actor))
                            {
                                currentRenderer.RemoveActor(actor);
                                removed = true;
                                return;
                            }
                        }
                    }
                }
            }
        }



        /// <summary>Adds the specified 2D actor to the active renderer fo the current VTK plotter.</summary>
        /// <param name="actor">A 2D actor to be added.</param>
        public void AddActor2D(vtkActor2D actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException("actor", "VTK 2D actor to be added is not specified (null reference).");
            }
            lock (Lock)
            {
                vtkRenderer ren = this.Renderer;
                if (ren == null)
                {
                    throw new InvalidOperationException("Can not add a VTK 2D actor to the plotter, active renderer is not defined.");
                }
                else
                {
                    //ren.AddActor2D(actor);
                    if (!UtilVtk.ContainsActor2D(ren, actor))
                    {
                        ren.AddActor2D(actor);
                    }
                    else
                    {
                        ; // ren.AddActor2D(actor);
                    }
                }
            }
        }

        /// <summary>Clears the active renderer of 2D actors (removes all 2D actors on it).</summary>
        public void RemoveAllActor2Ds()
        {
            lock (Lock)
            {
                vtkRenderer ren = Renderer;
                if (ren != null)
                {
                    UtilVtk.RemoveAllActors2D(ren);
                    // ren.Clear();
                }
                if (Window != null)
                {
                    List<vtkRenderer> renderers = UtilVtk.GetRenderers(Window);
                    if (renderers != null)
                    {
                        for (int i = 0; i < renderers.Count; ++i)
                        {
                            vtkRenderer currentRenderer = renderers[i];
                            if (currentRenderer != null)
                            {
                                UtilVtk.RemoveAllActors2D(currentRenderer);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>Removes the specified 2D actor form the active renderer (if it exists on that renderer) and from any 
        /// other renderer on which it is included (if not found on the active renderer). Nothing happens if the 
        /// actor is not found on any renderer.</summary>
        /// <param name="actor">The 2D actor to be removed.</param>
        public void RemoveActor2D(vtkActor2D actor)
        {
            lock (Lock)
            {
                bool removed = false;
                vtkRenderer ren = Renderer;
                if (ren != null)
                {
                    if (UtilVtk.ContainsActor2D(ren, actor))
                    {
                        ren.RemoveActor2D(actor);
                        removed = true;
                    }
                }
                if (!removed && Window != null)
                {
                    List<vtkRenderer> renderers = UtilVtk.GetRenderers(Window);
                    if (renderers != null)
                    {
                        for (int i = 0; i < renderers.Count; ++i)
                        {
                            vtkRenderer currentRenderer = renderers[i];
                            if (UtilVtk.ContainsActor2D(currentRenderer, actor))
                            {
                                currentRenderer.RemoveActor2D(actor);
                                removed = true;
                                return;
                            }
                        }
                    }
                }
            }
        }


        #endregion Data.Actors


        #endregion Data


        #region Operation


        private BoundingBox3d _boundsCoordinates;

        /// <summary>Bounds on coordinates of points that define the plots.
        /// Everything on the plot should fit in these bounds.</summary>
        public BoundingBox3d BoundsCoordinates
        {
            get
            {
                if (_boundsCoordinates == null)
                    _boundsCoordinates = new BoundingBox3d();
                return _boundsCoordinates;
            }
            protected set
            {
                _boundsCoordinates = value;
            }
        }

        /// <summary>Recalculates bounds on co-ordinates according to all plots that are contained
        /// in the current plotter.</summary>
        public void CalculateBoundsFromPlotsBounds()
        {
            lock (Lock)
            {
                BoundsCoordinates.Reset();
                for (int i = 0; i < PlottingObjects.Count; ++i)
                {
                    VtkPlotBase plot = PlottingObjects[i];
                    if (plot != null)
                        if (plot.BoundsCoordinates != null)
                        {
                            BoundsCoordinates.Update(plot.BoundsCoordinates);
                        }
                }
                
            }
        }


        /// <summary>Recalculates bounds on co-ordinates according to all plots that are contained
        /// in the current plotter.</summary>
        public void CalculateBoundsFromPlotsActors()
        {
            lock (Lock)
            {
                BoundsCoordinates.Reset();
                for (int i = 0; i < PlottingObjects.Count; ++i)
                {
                    VtkPlotBase plot = PlottingObjects[i];
                    plot.UpdateBoundsOnActors(BoundsCoordinates);
                }

            }
        }




        protected bool _skipNullWindowCheck = false;

        protected void ShowPlotEventhandler()
        {
            if (_window == null)
            {
                if (FormContainer != null)
                {
                    _window = FormContainer.GetVtkRenderWindow();
                }
            }
            if (_window != null)
            {
                ShowPlot();
            }
        }

        /// <summary>Basic things that must be done when showing the plot.</summary>
        public void ShowPlotWithoutRender()
        {

            CalculateBoundsFromPlotsActors();
            DecorationHandler.SetBounds(this.BoundsCoordinates);

            if (this.IsScaled)
            {
                // Scaling is switched on, scale all actors on the plot before showing:
                this.ScaleActors();
                this.ScaleDecorations();
            }

        }


        /// <summary>Adda a test graphics (a simple surface plot) to the specified plotter.</summary>
        /// <param name="plotter">Plotter where test graphics is added.</param>
        public static void ExampleAddTestGraph(VtkPlotter plotter)
        {
            VtkContourPlot plot = new VtkContourPlot(plotter);  // plot object that will create contour plots on the plotter object

            // Form the test graph: 
            IFunc2d func = new VtkPlotBase.ExampleFunc2dXY();

            plot.OutputLevel = 1;  // print to console what's going on
            BoundingBox2d paramBounds = new BoundingBox2d(-1, 1, -5, 5);

            // Create the first surface graph by the plot object; adjust the setting first:
            func = new VtkPlotBase.ExampleFunc2dXY();
            plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            plot.SetBoundsParameters(paramBounds);
            plot.NumX = plot.NumY = 30;
            plot.NumContours = 20;
            plot.LinesVisible = true;
            plot.SurfacesVisible = true;
            plot.SurfaceColor = System.Drawing.Color.LightCoral;
            plot.SurfaceColorOpacity = 0.4;
            plot.PointSize = 4; plot.LineWidth = 2; plot.PointsVisible = false;
            plot.SurfaceColorIsScaled = false;
            plot.LineColorIsScaled = false;
            plot.Create();
        }


        /// <summary>Default value of the flag that specifies whether VTK form containers are launched as modal forms.</summary>
        public const bool DefaultFormContainerModal = true;

        bool _formContainerModal = DefaultFormContainerModal;

        /// <summary>Whether or not VTK form containers are launched as modal forms.</summary>
        bool FormContainerModal
        {
            get { lock (Lock) { return _formContainerModal; } }
            set { lock (Lock) { _formContainerModal = value; } }
        }


        /// <summary>Default value of the flag that specifies whether or not form containers that contain VTK graphics are launched in parallel thread.</summary>
        public static bool DefaultFormContainerParallel = false;

        bool _formContainerParallel = DefaultFormContainerParallel;

        /// <summary>Whether or not form containers that contain VTK graphics are launched in parallel thread.</summary>
        bool FormContainerParallel
        {
            get { lock (Lock) { return _formContainerParallel; } }
            set { lock (Lock) { _formContainerParallel = value; } }
        }

        /// <summary>Launches the form container.</summary>
        [STAThread]
        protected void ShowFormContainerInThread()
        {

            // CREATE A NEW VTK FORM and prepare it for showing the graphics
            // (add the proper event handlers, etc.): 

            // if (FormContainer == null)
            FormContainer = CreateVtkFormContainer();
            System.Windows.Forms.Form form = FormContainer as System.Windows.Forms.Form;

            if (FormContainer != null)
            {
                VtkControlBase vtkControl = FormContainer.VtkControl;
                if (vtkControl != null)
                {
                    
                    // Add the event handler that will render plotter's graphics:
                    vtkControl.LoadVtkGraphics += (obj, eventArgs) =>
                    {
                        this.RenderControl = vtkControl.VtkRenderWindowControl as RenderWindowControl;
#if !VTKFORMSDESIGN
                        this.SetWindow(vtkControl.VtkRenderWindow);
                        this.Renderer = vtkControl.VtkRenderer;
#endif

                        //this.RemoveAllActors();

                        // REMARK:
                        // It is important to ensure that renderer is set properly. The renderer is set anove explicitly, 
                        // however it may be sufficient to set renderer to null wherever the VTK render window, VTK control,
                        // of VTK form container are set:
                        //this.setRenderer(vtkControl.VtkRenderer);

                        if (this._window == null || this.Renderer == null)
                        {
                            if (OutputLevel >= 1)
                            {
                                if (this._window == null)
                                {
                                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                                        + "ERROR: VTK render window can not be acquired (initialization problem?)."
                                        + Environment.NewLine);
                                }
                                if (this.Renderer == null)
                                {
                                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                                        + "ERROR: VTK renderer can not be acquired (initialization problem?)."
                                        + Environment.NewLine);
                                }
                            }
                        }
                        else
                        {
                            if (DecorationHandler != null)
                            {
                                DecorationHandler.AddActorsToPlotter(this);
                            }
                            for (int i = 0; i < PlottingObjects.Count; ++i)
                            {
                                VtkPlotBase plot1 = PlottingObjects[i];
                                if (plot1 != null)
                                {
                                    //plot1.Create();
                                    plot1.AddActorsToPlotter(this);
                                }
                            }

                            //this.RemoveAllActors();

                            this.ShowPlotWithoutRender();

                            // vtkRenderer renderer = vtkControl.VtkRenderer;
                            if (Renderer == null)
                            {
                                if (OutputLevel >= 1)
                                {
                                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                                        + "ERROR: VTK renderer could not be acquired while VTK rendering window was."
                                        + Environment.NewLine);
                                }
                            }
                            else
                            {
                                Renderer.Render();
                            }
                        }
                    };

                    // Add event handler that will unset the VRK render window on form's close event:
                    if (form != null)
                    {
                        form.FormClosing += (obj, eventArgs) =>
                        {
                            if (OutputLevel >= 1)
                            {
                                Console.WriteLine(Environment.NewLine + Environment.NewLine
                                    + "CLOSING VTK form container, VTK render window will be set to null."
                                    + Environment.NewLine);
                            }
                            this.Renderer = null;
                            this._window = null;
                            this.RenderControl = null;
                        };
                    }

                    if (!FormContainerParallel)
                        _windowCreated = true;

                    // UNCOMMENT the two lines below IF this code is moved to ShowPlot()!
                    // _windowCreated = true;
                    // ShowFormContainer();
                    
                }
            }


            // LAUNCH THE VTK FORM:

            // System.Windows.Forms.Form form = FormContainer as System.Windows.Forms.Form;
            if (form == null)
            {
                if (OutputLevel >= 1)
                {
                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                        + "ERROR: Can not obtain a form to show VTK graphics with." + Environment.NewLine);
                }
            } else
            {
                if (FormContainerModal)
                {
                    form.ShowDialog();
                    //Application.Run(form);
                } else
                {
                    form.Show();
                }
            }
        }

        /// <summary>Shows the form container.
        /// <para>Dependent on the value of the <see cref="FormContainerParallel"/> property, the form container may
        /// be shown in a parallel thread, which means that other form containers can be shown in parallel before the 
        /// current one is closed.</para></summary>
        protected void ShowFormContainer()
        {
            if (FormContainerParallel)
            {
                if (OutputLevel>=1)
                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                        + "Launching the VTK form in a parallel thread..." + Environment.NewLine);
                Thread t = new Thread(ShowFormContainerInThread);
                t.Start();
            } else
            {
                ShowFormContainerInThread();
            }

        }

        /// <summary>Whether the VTK window has been created, used for internal communication between some methods.</summary>
        protected bool _windowCreated = false;

        /// <summary>Shows the plot in the rendering window and allows user interactor for rotating, zooming, etc.</summary>
        public void ShowPlot()
        {
            //IsAllowedAnyVtkForm = false;
            bool addTestGraph = false;
            
            if (_window == null && IsAllowedAnyVtkForm)
            {
                if (addTestGraph)
                {
                    ExampleAddTestGraph(this);
                }

                ShowFormContainer(); 
                if (FormContainerParallel)
                    _windowCreated = true; // set to true here only since the window creation part is moved to another method

            }



            if (!_windowCreated)
            {
                if (DecorationHandler != null)
                {
                    DecorationHandler.AddActorsToPlotter(this);
                }
                ShowPlotWithoutRender();

                if (IsWindowStandalone)
                {
                    Window.SetSize(WindowSizeX, WindowSizeY);
                    Window.SetPosition(WindowPositionX, WindowPositionY);

                    Window.Render();
                    vtkRenderWindowInteractor windowInteractor = vtkRenderWindowInteractor.New();
                    windowInteractor.SetRenderWindow(Window);
                    windowInteractor.Initialize();

                    windowInteractor.Start();
                    Window.FinalizeWrapper();
                    //windowInteractor.TerminateApp();
                    windowInteractor.Dispose();
                    Window.Dispose();
                    Window = null;
                }
                else
                {
                    throw new NotImplementedException("Rendering VTK graphics in embedded windows is not yet implemented.");
                }



            }
        }

        #endregion Operation


        #region Operation.ScaleGraphs

        protected bool _isScaled = false;

        /// <summary>Specifies whether coordinates of graphic objects are scaled when plottting.</summary>
        public bool IsScaled
        {
            get { lock (Lock) { return _isScaled; } }
            set { lock (Lock) { _isScaled = value; } }
        }


        private BoundingBox3d _boundsScaled;

        /// <summary>Bounds on scaled coordinates graphic objects.
        /// Everything on the scaled plot should fit in these bounds.
        /// <para>Getter always returns an allocated bounding box. By default, all coordinate bounds
        /// are set to [0, 1].</para></summary>
        public BoundingBox3d BoundsScaled
        {
            get
            {
                if (_boundsScaled == null)
                    _boundsScaled = new BoundingBox3d(0,1,0,1,0,1);
                return _boundsScaled;
            }
            protected set
            {
                _boundsScaled = value;
            }
        }

        /// <summary>Sets the bounds of scaled graphs.</summary>
        /// <param name="bounds">Bounds of scaled graph to be set. Bounds are just copied to the internal data structure.</param>
        public void SetBoundsScaled(IBoundingBox bounds)
        {
            BoundsScaled.Reset();
            BoundsScaled.Update(bounds);
        }

        private Vector3d vecOriginal = new Vector3d(0);

        private Vector3d vecScaled = new Vector3d(0);

        /// <summary>Scales coordinates from the physical (original) coordinate system to the scaled graphical coordinates.</summary>
        /// <param name="original">Vector of physical (original) coordinates.</param>
        /// <param name="scaled">Vector of graphical coordinates.</param>
        public void ScaleCoordinatesPlain(IVector original, ref IVector scaled)
        {
            BoundingBox.Map(BoundsCoordinates, BoundsScaled, original, ref scaled);
        }


        /// <summary>Scaling factors - mapping parameters used when scaling VTK actors.</summary>
        protected vec3 ScalingFactors = new vec3();

        /// <summary>Scaling translations - mapping parameters used when scaling VTK actors.</summary>
        protected vec3 ScalingTranslations = new vec3();

        /// <summary>Calculates parameters for performing mapping on VTK actors.</summary>
        public void CalculateScalingParameters()
        {

            ScalingFactors.x = (BoundsScaled.MaxX - BoundsScaled.MinX) / (BoundsCoordinates.MaxX - BoundsCoordinates.MinX);
            ScalingFactors.y = (BoundsScaled.MaxY - BoundsScaled.MinY) / (BoundsCoordinates.MaxY - BoundsCoordinates.MinY);
            ScalingFactors.z = (BoundsScaled.MaxZ - BoundsScaled.MinZ) / (BoundsCoordinates.MaxZ - BoundsCoordinates.MinZ);

            ScalingTranslations.x = BoundsScaled.MinX - BoundsCoordinates.MinX * ScalingFactors.x;
            ScalingTranslations.y = BoundsScaled.MinY - BoundsCoordinates.MinY * ScalingFactors.y;
            ScalingTranslations.z = BoundsScaled.MinZ - BoundsCoordinates.MinZ * ScalingFactors.z;
        }


        /// <summary>Performs scaling on the specified VTK actor.
        /// <para>Also calculates parameters of mapping before scaling is applied.</para>
        /// <para>Scaling is performed regardless of the value of the scaling flad (the <see cref="IsScaled"/> property).</para></summary>
        /// <param name="actor">Actor whose coordinates are mapped (scaled).</param>
        public void ScaleActor(vtkActor actor)
        {
            CalculateScalingParameters();
            ScaleActorPlain(actor);
        }

        /// <summary>Performs scaling on the specified VTK actor.
        /// <para>Parameters of mapping must be calculated before this method is called. This is 
        /// done by the <see cref="CalculateScalingParameters"/> method.</para>
        /// <para>Scaling is performed regardless of the value of the scaling flad (the <see cref="IsScaled"/> property).</para></summary>
        /// <param name="actor">Actor whose coordinates are mapped (scaled).</param>
        public void ScaleActorPlain(vtkActor actor)
        {
            actor.SetScale(ScalingFactors.x, ScalingFactors.y, ScalingFactors.z);
            actor.SetPosition(ScalingTranslations.x, ScalingTranslations.y, ScalingTranslations.z);
        }

        /// <summary>Scales all actors on all plots of the bounding box.</summary>
        public void ScaleActors()
        {
            lock(Lock)
            {
                CalculateScalingParameters();
                for (int i = 0; i < PlottingObjects.Count; ++i)
                {
                    VtkPlotBase plot = PlottingObjects[i];
                    plot.ScaleActors();
                }
            }
        }




        /// <summary>Scales decorations, including the axes, according to scaling defined on this plotter.
        /// <para>Scaling is performed regardless of the value of the scaling flad (the <see cref="IsScaled"/> property).</para></summary>
        public void ScaleDecorations()
        {
            lock (Lock)
            {
                CalculateScalingParameters();

                DecorationHandler.SetBounds(this.BoundsScaled);

                //vtkCubeAxesActor actor = DecorationHandler.CubeAxesActor;
                //ScaleActor(actor);
                //VtkDecorationHandler.SetCubeAxesActorBounds(actor, BoundsScaled);
            }
        }

        #endregion Operation.ScaleGraphs


        #region IDisposable


        ~VtkPlotter()
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


        /// <summary>Class that is used to access the <see cref="VtkPlotter"/>'s window in classes that are not derived 
        /// from that class. Access can be granted by either inheriting from this class or by uding this class
        /// as proxy class.
        /// <para>This class is immutable in the sense that plotter can be set only when constructed.</para>
        /// <para>Warning: One should avoid accessing the plotter's window where this is not absolutely necessary.</para></summary>
        public sealed class Accessor
        {

            private Accessor() {  }

            public Accessor(VtkPlotter plotter)
            { this.Plotter = plotter; }


            VtkPlotter _plotter;

            public VtkPlotter Plotter
            {
                get { return _plotter; }
                private set {
                    if (value == null)
                        throw new ArgumentException("Plotter is not specified on its accesor (null reference).");
                    _plotter = value; }
            }

            // private vtkRenderWindow _window;

            /// <summary>VTK window used for rendering 3D graphics.
            /// <para>Warning: settes should only be used in constructors.</para></summary>
            public vtkRenderWindow Window
            {
                get { return Plotter.Window; }
                set { Plotter.Window = value; }
            }


            /// <summary>VTK renderer used by the current plotter for rendering 3D graphics.
            /// <para>Warning: setters should only be used in constructors.</para></summary>
            public vtkRenderer Renderer
            {
                get { return Plotter.Renderer; }
                set { Plotter.Renderer = value; }
            }

            ///// <summary>VTK window used for rendering 3D graphics, DIRECT ACCESS through field (bypassing property).
            ///// <para>Warning: setters should only be used in constructors.</para></summary>
            //public vtkRenderWindow _window
            //{
            //    get { return Plotter._window; }
            //    set { Plotter._window = value; }
            //}

        } // class Accessor
         

    } // class VtkPlotter



}