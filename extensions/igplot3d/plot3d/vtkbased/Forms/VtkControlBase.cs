// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Kitware.VTK;

using IG.Lib;
using IG.Num;
using IG.Forms;

namespace IG.Gr3d
{


    /// <summary>Basic control encapsulating the VTK's render control of type <see cref="Kitware.VTK.RenderWindowControl"/>.</summary>
    /// <remarks>
    /// <para>This control solves the problem that controls containing the VTK's render control (<see cref="Kitware.VTK.RenderWindowControl"/>)
    /// can not be shown in designer because ActiViz provides only 64 bit version while Visual Studio's designer requires that 
    /// controls' implementations can be loaded on 32 bit environments (i.e. target platform can be either set to 32 bit or any platform).</para>
    /// <para></para>
    /// </remarks>
    /// $A Igor May13;
    public partial class VtkControlBase : UserControl, IVtkFormContainer,
        I3dGraphicsControl
    {
        public VtkControlBase()
        {
            InitializeComponent();

            // Reset state of the Vtk control since automatically generated designer's code overrides the settings:
            InitializeState();

            VtkReplaceControl();

            // Add the predefined event handler for loading VTK graphics (other handlers may be added externally):
            this.LoadVtkGraphics += new System.EventHandler(this.VtkControlBase_LoadVtkGraphics);
        }

        public void InitializeState()
        {
            this.RotationStep = 5.0;
            this.ZoomFactor = 1.2;
            
            CameraViewAngle = 30;
            CameraRoll = 0;
            CameraPosition = new vec3(0, 0, 1);
            CameraFocalPoint = new vec3(0, 0, 0);
            CameraViewUp = new vec3(0, 1, 0);
            CameraDirection = new vec3(1, 0, 0);

            this.VtkAddTestActors = DefaultVtkAddTestActors;
            this.VtkAddTestActorsIGLib = DefaultVtkAddTestActorsIGLib;
        }

        /// <summary>Intializes the VTK interactor mode.</summary>
        public void InitializeVtkRendering()
        {
            // Intialize the VTK interactor mode:
            IsJoystickMode = false;
            IsCameraMode = true;
            IsWireframeMode = false;
            IsSurfaceMode = false;
        }


        #region VtkEvents

        /// <summary>Event that occurs when graphics must be loaded in Vtk control.</summary>
        public event EventHandler LoadVtkGraphics;

        /// <summary>Raises the <see cref="LoadVtkGraphics"/> event.</summary>
        public void OnLoadVtkGraphics(EventArgs e)
        {
            if (LoadVtkGraphics != null)
                LoadVtkGraphics(this, e);
        }

        #endregion VtkEvents


        #region VtkAuxiliary


        public static bool DefaultVtkAddTestActors = false;

        private bool _vtkAddTestActors = DefaultVtkAddTestActors;

        /// <summary>Gets the flag indicated whether test actors are added to the VTK control.
        /// <para>Test actors are added for debugging purposes. If default value of the flag is true then all newly created controls
        /// of the current type will contain a pre-loaded test actors and it can be seen if the VTK renderer is properly
        /// installed on the control.</para>
        /// <para>Default value is defined by the static property <see cref="DefaultVtkAddTestActors"/></para></summary>
        public bool VtkAddTestActors
        {
            get { return _vtkAddTestActors; }
            set { _vtkAddTestActors = value; }
        }


        public static string DefaultVtkTestText = null;

        private string _vtkTestText = DefaultVtkTestText;

        /// <summary>Gets the eventual test text that is shown on  the VTK control.
        /// <para>Test text is added for debugging purposes. If default value is non-null then all newly created controls
        /// of the current type will contain a pre-loaded text through which it can be seen if the VTK renderer is properly
        /// installed on the controls.</para>
        /// <para>Default value is defined by the static property <see cref="DefaultVtkTestText"/></para></summary>
        protected string VtkTestText
        {
            get { return _vtkTestText; }
        }


        public static bool DefaultVtkAddTestActorsIGLib = false;

        private bool _vtkAddTestActorsIGLib = DefaultVtkAddTestActorsIGLib;

        /// <summary>Gets the flag indicated whether test actors created by IGLib plots are added to the VTK control.
        /// <para>Test actors are added for debugging purposes. If default value of the flag is true then all newly created controls
        /// of the current type will contain a pre-loaded test actors and it can be seen if the VTK renderer is properly
        /// installed on the control.</para>
        /// <para>Default value is defined by the static property <see cref="DefaultVtkAddTestActors"/></para></summary>
        public bool VtkAddTestActorsIGLib
        {
            get { return _vtkAddTestActorsIGLib; }
            set { _vtkAddTestActorsIGLib = value; }
        }


        #endregion VtkAuxiliary


        #region IVtkFormContainer


        public bool _isVtkInitialized = false;

        /// <summary>Whether the VTK window is initialized and ready to show graphics and accept commands.</summary>
        public bool IsVtkInitialized
        {
            get { return _isVtkInitialized; }
            protected set { _isVtkInitialized = value; }
        }


        /// <summary>Base windows forms control through which the basic VTK functionality and all additional features can be
        /// accessed.</summary>
        public VtkControlBase VtkControl
        {
            get { return this; }
        }


        #region I3dGraphicsControl.Common

        /// <summary>Angle step, in degrees, that is used in a single rotation operation.
        /// <para>Must be greater than 0.</para></summary>
        protected double _rotationStep = 5.0;

        public double RotationStep
        {
            get { return _rotationStep; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentException("Rotation step less or equal to 0 provided.");
                _rotationStep = value;
            }
        }

        /// <summary>Zoom step, in degrees, that is used in a single zoom operation.
        /// <para>Must be greater than 1.</para></summary>
        protected double _zoomStep = 1.2;

        public double ZoomFactor
        {
            get { return _zoomStep; }
            set {
                if (value<=1.0)
                    throw new ArgumentException("Zoom step less or equal to 1 provided.");
                _zoomStep = value;
            }
        }

        // COMMON:

        protected double _cameraViewAngle = 30;

        protected double _cameraRoll = 0;

        protected vec3 _cameraPosition = new vec3(0, 0, 1);

        protected vec3 _cameraFocalPoint = new vec3(0, 0, 0);

        protected vec3 _cameraViewUp = new vec3(0, 1, 0);

        protected vec3 _cameraDirection = new vec3(1, 0, 0);

        protected vec3 _cameraDirectionSpherical;


        /// <summary>Camera direction.
        /// <para>Getter obtains it as difference between the camera focal point and camera position.</para>
        /// <para>Setter sets the camera focal points in such a way that camera direction has the specified value.</para></summary>
        public vec3 CameraDirection
        {
            get { return CameraFocalPoint - CameraPosition; }
            set
            {
                CameraFocalPoint = CameraPosition + value;
            }
        }

        public const double smallNumber = 1.0e-8;

        /// <summary>Gets or sets camera direction in spherical coordinates.</summary>
        public vec3 CameraDirectionSpherical
        {
            get
            {
                vec3 dir = CameraDirection;
                double r = Math.Sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                double fi, theta;
                if (Math.Abs(dir.x) <= smallNumber)
                {
                    if (Math.Sign(dir.x) * Math.Sign(dir.y) < 0)
                        fi = -0.5 * Math.PI;
                    else
                        fi = 0.5 * Math.PI;
                }
                else
                {
                    fi = Math.Atan(dir.y / dir.x) * 180.0 / Math.PI;
                }
                if (Math.Abs(r) <= smallNumber)
                {
                    if (Math.Sign(dir.x) * Math.Sign(dir.y) < 0)
                        theta = Math.PI;
                    else
                        theta = 0;
                }
                else
                {
                    theta = Math.Acos(dir.z / r) * 180.0 / Math.PI;
                }
                vec3 ret = new vec3(
                    r,
                    fi,
                    theta
                    );
                return ret;
            }
            set
            {
                double r = value.x, fi = value.y * Math.PI / 180.0, theta = value.z * Math.PI / 180.0;
                vec3 dir = new vec3(
                    r * Math.Sin(theta) * Math.Cos(fi),
                    r * Math.Sin(theta) * Math.Sin(fi),
                    r * Math.Cos(theta));
                CameraDirection = dir;
            }
        }


        #endregion I3dGraphicsControl.Common

#if (!VTKFORMSDESIGN)


        #region VtkReplacement



        protected bool _vtkReplacementAccomplished = false;

        /// <summary>Replaces the dummy control (which can be manipulated by the Windows Forms Designer) by the VTK renderer
        /// control of type <see cref="Kitware.VTK.RenderWindowControl"/>.</summary>
        protected virtual void VtkReplaceControl()
        {
            if (!_vtkReplacementAccomplished)
            {
                this.SuspendLayout();
                try
                {
                    if (VtkRenderWindowControl == null)
                    {
                        // Create the VTK renderer control if not yet created:
                        //if (vtkReplacementPanel is Kitware.VTK.RenderWindowControl)
                        //{
                        //    VtkRenderWindowControl = vtkReplacementPanel as Kitware.VTK.RenderWindowControl;
                        //} else
                        //{
                        //    VtkRenderWindowControl = new Kitware.VTK.RenderWindowControl();
                        //}
                        VtkRenderWindowControl = new Kitware.VTK.RenderWindowControl();
                        _vtkRenderWindow = VtkRenderWindowControl.RenderWindow;
                        if (_vtkRenderWindow == null)
                        {
                            Console.WriteLine(Environment.NewLine + Environment.NewLine + 
                                "WARNING: VTK Render window is null!" + Environment.NewLine);
                        }
                    }
                    // this._renderWindowControl1.Load += new System.EventHandler(this.renderWindowControl1_Load);
                    this.Load += new System.EventHandler(this.renderWindowControl1_Load);

                    // Set the VTK-related settings on the control:
                    if (VtkAddTestActors)
                    {
                        VtkRenderWindowControl.AddTestActors = VtkAddTestActors;
                        VtkRenderWindowControl.TestText = VtkTestText;
                    }
                    VtkRenderWindowControl.Name = "renderWindowControl1";
                    // Set properties of the VTK renderer control in accordance with the replacement control
                    // that took its part in the designer and in initialization of components:

                    ////this.vtkReplacementPanel.BackColor = System.Drawing.Color.Wheat;
                    ////this.vtkReplacementPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                    ////this.vtkReplacementPanel.Location = new System.Drawing.Point(0, 0);
                    ////this.vtkReplacementPanel.Name = "vtkReplacementPanel";
                    ////this.vtkReplacementPanel.Size = new System.Drawing.Size(400, 408);
                    ////this.vtkReplacementPanel.TabIndex = 0;

                    VtkRenderWindowControl.BackColor = Color.White; // vtkReplacementPanel.BackColor;
                    VtkRenderWindowControl.Anchor = vtkReplacementPanel.Anchor;
                    VtkRenderWindowControl.Dock = vtkReplacementPanel.Dock;
                    VtkRenderWindowControl.Location = new System.Drawing.Point(
                        vtkReplacementPanel.Location.X, vtkReplacementPanel.Location.Y);
                    VtkRenderWindowControl.Size = new Size(
                        vtkReplacementPanel.Size.Width, vtkReplacementPanel.Size.Height);
                    VtkRenderWindowControl.TabIndex = vtkReplacementPanel.TabIndex;

                }
                catch { throw; }
                finally
                {
                    try
                    {
                        if (VtkRenderWindowControl == null)
                        {
                            // Notify that something went wrong:
                            vtkReplacementPanel.BackColor = Color.Red;
                        }
                        else
                        {
                            this.Controls.Remove(txtReplacementNotification);
                            this.Controls.Remove(vtkReplacementPanel);
                            this.Controls.Add(VtkRenderWindowControl);
                        }
                    }
                    catch {
                        Console.WriteLine(Environment.NewLine + Environment.NewLine +
                            "WARNING: Exception was trown in VtkReplaceControl(). 3D graphics may not show." + Environment.NewLine);
                        throw; }
                    finally
                    {
                        this.ResumeLayout();
                    }
                }
            }
            _vtkReplacementAccomplished = true;
        }

        private Kitware.VTK.RenderWindowControl _renderWindowControl1;

        /// <summary>Gets the VTK's rendering control that shows the VTK graphics on the form.</summary>
        public Kitware.VTK.RenderWindowControl VtkRenderWindowControl
        {
            get { return _renderWindowControl1; }
            protected set { 
                _renderWindowControl1 = value; 
                // Update dependencies:
                // _vtkRenderWindow = null;
            }
        }



        protected vtkRenderer _vtkRenderer;

        /// <summary>Gets the VTK renderer that renders 3D objects on the VTK's control <see cref="VtkRenderWindowControl"/>.</summary>
        public vtkRenderer VtkRenderer
        {
            get
            {
                if (_vtkRenderer == null)
                {
                    _vtkRenderer = this.GetVtkRenderer(); 
                    if (_vtkRenderer==null)
                    {
                        if (!this.IsVtkForm())
                        {
                            Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                "The current container is not a true VTK render container, VTK renderer could not be obtained." + Environment.NewLine);
                        }
                        else
                        {
                            if (this.IsVtkInitialized)
                            {
                                Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                    "VTK  renderer is not available, although the VTK control is initialized." + Environment.NewLine);
                            }
                            else
                            {
                                Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                    "VTK  renderer is not available or not initialized." + Environment.NewLine);
                            }
                        }
                    }
                }
                return _vtkRenderer;
            }
            protected set { _vtkRenderer = value; }
        }


        protected vtkCamera _vtkCamera;

        /// <summary>Gets the VTK camera that defines the viewing position, direction, zoom, etc.</summary>
        public vtkCamera VtkCamera
        {
            get 
            {
                if (_vtkCamera == null)
                {
                    _vtkCamera = this.GetVtkCamera();
                    if (_vtkCamera == null)
                    {
                        if (!this.IsVtkForm())
                        {
                            Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                "The current container is not a true VTK render container, VTK camera could not be obtained." + Environment.NewLine);
                        }
                        else
                        {
                            if (this.IsVtkInitialized)
                            {
                                Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                    "VTK  camera is not available, although the VTK control is initialized." + Environment.NewLine);
                            }
                            else
                            {
                                Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                    "VTK  camera is not available or is not initialized." + Environment.NewLine);
                            }
                        }
                    }


                    //vtkRenderer renderer = VtkRenderer;
                    //if (renderer != null)
                    //    _vtkCamera = VtkRenderer.GetActiveCamera();
                    //else
                    //{
                    //    Console.WriteLine("");
                    //}
                }
                return _vtkCamera; }
            set { _vtkCamera = value; }
        }

        vtkRenderWindow _vtkRenderWindow;

        /// <summary>VTK render window of hte current control.
        /// <para>Getter may return null if the window is not defined.</para></summary>
        public Kitware.VTK.vtkRenderWindow VtkRenderWindow
        {
            get {
                if (_vtkRenderWindow == null)
                {
                    _vtkRenderWindow = this.GetVtkRenderWindow();
                    if (_vtkRenderWindow == null)
                    {
                        if (!this.IsVtkForm())
                        {
                            Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                "The current container is not a true VTK render container, VTK render window could not be obtained." + Environment.NewLine);
                        }
                        else
                        {
                            if (this.IsVtkInitialized)
                            {
                                Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                    "VTK  render window is not available, although the VTK control is initialized." + Environment.NewLine);
                            }
                            else
                            {
                                Console.WriteLine(Environment.NewLine + Environment.NewLine +
                                    "VTK  render window is not available or is not initialized." + Environment.NewLine);
                            }
                        }
                    }

                    if (VtkRenderWindowControl != null)
                    {
                        _vtkRenderWindow = VtkRenderWindowControl.RenderWindow;
                        if (_vtkRenderWindow == null)
                        {
                            Console.WriteLine(Environment.NewLine + Environment.NewLine + "VTK render window is null on VTK control." + Environment.NewLine);
                        }
                        else
                        {
                            IsVtkInitialized = true;
                        }
                    }
                }
                return _vtkRenderWindow;
            }
            protected set
            {
                _vtkRenderWindow = value;
                // Update dependencies:
                VtkRenderer = null;
            }
        }



        /// <summary>Adds test surface plots to the specified VTK control.
        /// <para>This method can be called within an external event handler that is added 
        /// to the <see cref="LoadVtkGraphics"/> event.</para></summary>
        /// <param name="control">VTK control where graphics is added.</param>
        public virtual void VtkOnRenderWindowControlLoad()
        {
            Color backColor = VtkRenderWindowControl.BackColor;
            VtkRenderer.SetBackground((double)backColor.R / byte.MaxValue, (double)backColor.G / byte.MaxValue, (double)backColor.B / byte.MaxValue);
        }


        
        /// <summary>Adds test surface plots to the VTK control that is contained on the specified VTK container.
        /// <para>This method can be called within an external event handler that is added 
        /// to the <see cref="LoadVtkGraphics"/> event of the VTK control.</para></summary>
        /// <param name="control">VTK control where graphics is added.</param>
        /// <remarks>This method just calls the <see cref="ExampleExternalLoadVtkGraphics_SurfacePlots"/> method 
        /// with argument of type <see cref="VtkControlBase"/></remarks>
        public static void ExampleExternalLoadVtkGraphics_SurfacePlots(IVtkFormContainer vtkContainer)
        {
            VtkControlBase control = null;
            if (vtkContainer != null)
            {
                control = vtkContainer.VtkControl;
                if (control != null)
                    ExampleExternalLoadVtkGraphics_SurfacePlots(control);
            }
        }



        /// <summary>Adds test surface plots to the specified VTK control.
        /// <para>This method can be called within an external event handler that is added 
        /// to the <see cref="LoadVtkGraphics"/> event.</para></summary>
        /// <param name="control">VTK control where graphics is added.</param>
        /// <example>
        /// IVtkFormContainer vtkFormContainer;
        /// vtkFormContainer.VtkControl.LoadVtkGraphics += (obj, eventArgs) =>
        /// {
        ///     VtkControlBase.ExampleExternalLoadVtkGraphics_SurfacePlots(vtkFormContainer);
        /// };
        /// </example>
        public static void ExampleExternalLoadVtkGraphics_SurfacePlots(VtkControlBase control)
        {
            Kitware.VTK.vtkRenderWindow vtkWindow = control.VtkRenderWindow;
            if (vtkWindow == null)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine
                    + "WARNING: Can not obtain VTK Window (external example method)." + Environment.NewLine);
            }
            else
            {
                IFunc2d func = new VtkPlotBase.ExampleFunc2dXY();
                VtkPlotter plotter = new VtkPlotter(vtkWindow);  // plotter object that handles rendering of plots
                VtkContourPlot plot = new VtkContourPlot(plotter);  // plot object that will create contour plots on the plotter object
                plot.OutputLevel = 1;  // print to console what's going on
                BoundingBox2d paramBounds = new BoundingBox2d(-1, 1, -1, 1);
                // Create the first surface graph by the plot object; adjust the setting first:
                func = new VtkPlotBase.ExampleFunc2dXY();
                plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
                plot.SetBoundsParameters(paramBounds);
                plot.NumX = plot.NumY = 30;
                plot.NumContours = 20;
                plot.LinesVisible = true;
                plot.SurfacesVisible = true;
                plot.SurfaceColor = Color.LightCoral;
                plot.SurfaceColorOpacity = 0.4;
                plot.PointSize = 4; plot.LineWidth = 2; plot.PointsVisible = false;
                plot.SurfaceColorIsScaled = false;
                plot.LineColorIsScaled = false;
                // Create plot of the first surface according to settings:
                plot.Create();

                // Now show all the plots that were created by the plotter; method with the same name is 
                // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):

                //plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

                // Then, create the surface graph to combine it with contours; change some settings first:
                VtkSurfacePlot surfacePlot = new VtkSurfacePlot(plotter);  // plot object that will create contour plots on the plotter object
                surfacePlot.SetSurfaceDefinition(func); // another function of 2 variables for the secont graph
                surfacePlot.SetBoundsParameters(paramBounds);
                surfacePlot.NumX = surfacePlot.NumY = 8;
                surfacePlot.PointsVisible = true;
                surfacePlot.SurfacesVisible = false;
                surfacePlot.LinesVisible = true;
                surfacePlot.SurfaceColorIsScaled = false;
                surfacePlot.SurfaceColor = System.Drawing.Color.LightGreen;
                surfacePlot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent
                // Create the second plot:
                surfacePlot.Create();
                plotter.ShowPlotWithoutRender();
            }
        }


        /// <summary>Event handler that is called when VTK graphics is loaded.
        /// <para>This method actually executes loading of some test graphics in the case that the 
        /// appropriate flags are set.</para>
        /// <para>Flag <see cref="VtkAddTestActors"/> specified whether some VTK test graphics is added (this is graphics from the VTK Hello World example).</para>
        /// <para>Flag <see cref="VtkAddTestActorsIGLib"/> specified whether some IGLib test graphics is added (a parametric curve in 3D).</para></summary>
        private void VtkControlBase_LoadVtkGraphics(object sender, EventArgs e)
        {

            if (VtkAddTestActors)
            {

                // Create a simple sphere. A pipeline is created.
                vtkSphereSource sphere = vtkSphereSource.New();
                sphere.SetThetaResolution(8);
                sphere.SetPhiResolution(16);

                vtkShrinkPolyData shrink = vtkShrinkPolyData.New();
                shrink.SetInputConnection(sphere.GetOutputPort());
                shrink.SetShrinkFactor(0.9);

                vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
                mapper.SetInputConnection(shrink.GetOutputPort());

                // The actor links the data pipeline to the rendering subsystem
                vtkActor actor = vtkActor.New();
                actor.SetMapper(mapper);
                actor.GetProperty().SetColor(1, 0, 0);

                // Add the actors to the renderer, set the window size
                VtkRenderer.AddViewProp(actor);


                Console.WriteLine(Environment.NewLine + Environment.NewLine
                   + "VTK's test graphics added." + Environment.NewLine);
            }

            if (VtkAddTestActorsIGLib)
            {
                
                Kitware.VTK.vtkRenderWindow vtkWindow = VtkRenderWindow;
                if (vtkWindow == null)
                {
                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                        + "WARNING: Can not obtain VTK Window." + Environment.NewLine);
                }
                else
                {
                    int a = 4, b = 5;
                    VtkPlotter plotter = new VtkPlotter(vtkWindow);  // plotter object that handles rendering of plots
                    VtkCurvePlot plot = new VtkCurvePlot(plotter);  // plot object that will create curve plots on the plotter object
                    plot.OutputLevel = 1;  // print to console what's going on
                    // Create the first curve graph by the plot object; adjust the setting first:
                    plot.SetCurveDefinition(new VtkPlotBase.ExampleSineFunctionForLissajous(a),
                        new VtkPlotBase.ExampleSineFunctionForLissajous(b),
                        new VtkPlotBase.ExampleSineFunctionForLissajous(a + b)
                        );  // function of 1 variable that define the curve
                    plot.SetBoundsReference(0, 2 * Math.PI);
                    plot.NumX = 400;
                    plot.PointsVisible = true; plot.PointColorIsScaled = false;
                    plot.PointSize = 6; plot.PointColor = Color.Blue; plot.PointColorScale = ColorScale.CreateBlueYellow(0, 1);
                    plot.LinesVisible = true; plot.LineColorIsScaled = true;
                    plot.LineWidth = 4; plot.LineColor = Color.Red; plot.LineColorScale = ColorScale.CreateRainbow(0, 1);

                    // Create plot of the first surface according to settings:
                    plot.Create();

                    // Show the created blot:
                    plotter.ShowPlotWithoutRender();

                    Console.WriteLine(Environment.NewLine + Environment.NewLine
                        + "IGLib's test graphics added." + Environment.NewLine);
                }
            }
        }

        /// <summary>Event handler that is executed when the VTK control is loaded.</summary>
        protected virtual void renderWindowControl1_Load(object sender, EventArgs e)
        {
            // Create components of the rendering subsystem
            //
            //vtkRenderer VtkRenderer = renderWindowControl1.RenderWindow.GetRenderers().GetFirstRenderer();
            // vtkRenderWindow VtkRenderWindow = renderWindowControl1.RenderWindow;



            // IMPORTANT: This event is used when the form is used by a 3D plotter:
            // Raises event that occurs when VTK graphics mst be loaded.
            OnLoadVtkGraphics(e);

            // Also execute user defined things that are done when VTK's control is loaded:
            VtkOnRenderWindowControlLoad();

            InitializeVtkRendering();

            // renWin.SetSize(250, 250);
            // Set the camera:
            VtkRenderWindow.Render();
            VtkCamera.Zoom(1.1);

        }


        #endregion VtkReplacement


        #region I3dGraphicsControl

        public void RefreshGraph()
        {
            // VtkRenderer.Render();
            VtkRenderWindow.Render();
        }

        /// <summary>Changes <see cref="CameraViewAngle"/> by the factor defined by the specified factor.</summary>
        /// <param name="factor">Factor by which the view angle is increased (must be greater than 1).</param>
        public void ChangeZoom(double factor)
        {

            if (IsVtkInitialized)
            {
                VtkCamera.Zoom(factor);
                RefreshGraph();
            }
            _cameraViewAngle *= factor;
        }

        /// <summary>Rotates the camera in the fi direction.</summary>
        /// <param name="angleStepDegrees">Step, in degrees, by which the camera is rotated clockwise.</param>
        public void RotateAzimuth(double angleStepDegrees)
        {
            if (IsVtkInitialized)
            {
                VtkCamera.Azimuth(angleStepDegrees);
                RefreshGraph();
            }
            _cameraDirectionSpherical.y += angleStepDegrees;
        }

        /// <summary>Rotates the camera in the theta direction.</summary>
        /// <param name="angleStepDegrees">Step, in degrees, by which the camera is rotated.</param>
        public void RotatePitch(double angleStepDegrees)
        {
            if (IsVtkInitialized)
            {
                //VtkCamera.Pitch(angleStepDegrees);
                VtkCamera.Elevation(angleStepDegrees);
                RefreshGraph();
            }
            _cameraDirectionSpherical.z += angleStepDegrees;
        }


        /// <summary>Rotates the camera around the viewing direction.</summary>
        /// <param name="angleStepDegrees">Step, in degrees, by which the camera is rotated.</param>
        public void RotateRoll(double angleStepDegrees)
        {
            if (IsVtkInitialized)
            {
                VtkCamera.Roll(angleStepDegrees);
                RefreshGraph();
            }
            _cameraRoll += angleStepDegrees;
        }


        /// <summary>Viewing angle of the camera, in degrees (defines the zoom level).</summary>
        /// <param name="angleStepDegrees">Step, in degrees, by which the camera is rotated.</param>
        public double CameraViewAngle
        {
            get {
                if (IsVtkInitialized)
                {
                    _cameraViewAngle = VtkCamera.GetViewAngle();
                }
                return _cameraViewAngle;
            }
            set {
                if (IsVtkInitialized)
                {
                    VtkCamera.SetViewAngle(value);
                    RefreshGraph();
                }
                _cameraViewAngle = value;
            }
        }

        /// <summary>Roll of the camera (amount of rotation abount viewing direction)</summary>
        public double CameraRoll
        {
            get {
                if (IsVtkInitialized)
                {
                    _cameraRoll = VtkCamera.GetRoll();
                }
                return _cameraRoll;
            }
            set {
                if (IsVtkInitialized)
                {
                    VtkCamera.SetRoll(value);
                    RefreshGraph();
                }
                _cameraRoll = value;
            }
        }

        /// <summary>Gets or sets camera position.</summary>
        public vec3 CameraPosition
        {
            get {
                if (IsVtkInitialized)
                {
                    double[] coord = VtkCamera.GetPosition();
                    _cameraPosition.x = coord[0];
                    _cameraPosition.y = coord[1];
                    _cameraPosition.z = coord[2];
                }
                return _cameraPosition;
            }
            set
            {
                if (IsVtkInitialized)
                {
                    VtkCamera.SetPosition(value.x, value.y, value.z);
                    RefreshGraph();
                }
                _cameraPosition = value;
            }
        }


        /// <summary>Gets or sets camera focal point.</summary>
        public vec3 CameraFocalPoint
        {
            get
            {
                if (IsVtkInitialized)
                {
                    double[] coord = VtkCamera.GetFocalPoint();
                    _cameraFocalPoint.x = coord[0];
                    _cameraFocalPoint.y = coord[1];
                    _cameraFocalPoint.z = coord[2];
                }
                return _cameraFocalPoint;
            }
            set {
                if (IsVtkInitialized)
                {
                    VtkCamera.SetFocalPoint(value.x, value.y, value.z);
                    RefreshGraph();
                }
                _cameraFocalPoint = value;
            }
        }


        /// <summary>Gets or sets the camera viewing up position.</summary>
        public vec3 CameraViewUp
        {
            get
            {
                if (IsVtkInitialized)
                {
                    double[] coord = VtkCamera.GetViewUp();
                    _cameraViewUp.x = coord[0];
                    _cameraViewUp.y = coord[1];
                    _cameraViewUp.z = coord[2];
                }
                return _cameraViewUp;
            }
            set
            {
                if (IsVtkInitialized)
                {
                    VtkCamera.SetViewUp(value.x, value.y, value.z);
                    RefreshGraph();
                }
                _cameraViewUp = value;
           }
        }


        #endregion I3dGraphicsControl


#else // if (!VTKFORMSDESIGN)


        /// <summary>VTK's control that is actually used for rendering graphics and through which VTK can be interacted directly.
        /// <para>Instead of accessing this control directly, you should normally access it through the <see cref="VtkControlBase.VtkControl"/> property
        /// on the <see cref="VtkControl"/> property.</para></summary>
        public System.Windows.Forms.Control VtkRenderWindowControl
        {
            get { return this.vtkReplacementPanel; }
        }

        
        /// <summary>Dummy function for design mode. 
        /// <para>Original method replaces the dummy control (which can be manipulated by the Windows Forms Designer) by the VTK renderer
        /// control of type <see cref="Kitware.VTK.RenderWindowControl"/>.</para></summary>
        protected virtual void VtkReplaceControl()
        {
            IsVtkInitialized = true;
        }

        
        /// <summary>Event handler that is called when VTK graphics is loaded (this method must actually
        /// execute loading of graphics).</summary>
        private void VtkControlBase_LoadVtkGraphics(object sender, EventArgs e)
        {
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Event handler executed: VtkControlBase_LoadVtkGraphics(...)." + Environment.NewLine);
        }
        
        
        
        /// <summary>Adds test surface plots to the VTK control that is contained on the specified VTK container.
        /// <para>This method can be called within an external event handler that is added 
        /// to the <see cref="LoadVtkGraphics"/> event of the VTK control.</para></summary>
        /// <param name="control">VTK control where graphics is added.</param>
        /// <remarks>This method just calls the <see cref="ExampleExternalLoadVtkGraphics_SurfacePlots"/> method 
        /// with argument of type <see cref="VtkControlBase"/></remarks>
        public static void ExampleExternalLoadVtkGraphics_SurfacePlots(IVtkFormContainer vtkContainer)
        {
            Console.WriteLine(Environment.NewLine + Environment.NewLine 
                + "  Executied emulation of ExampleExternalLoadVtkGraphics_SurfacePlots(IVtkFormContainer)." 
                + Environment.NewLine);
        }



        /// <summary>Adds test surface plots to the specified VTK control.
        /// <para>This method can be called within an external event handler that is added 
        /// to the <see cref="LoadVtkGraphics"/> event.</para></summary>
        /// <param name="control">VTK control where graphics is added.</param>
        /// <example>
        /// IVtkFormContainer vtkFormContainer;
        /// vtkFormContainer.VtkControl.LoadVtkGraphics += (obj, eventArgs) =>
        /// {
        ///     VtkControlBase.ExampleExternalLoadVtkGraphics_SurfacePlots(vtkFormContainer);
        /// };
        /// </example>
        public static void ExampleExternalLoadVtkGraphics_SurfacePlots(VtkControlBase control)
        {
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "  Executied emulation of ExampleExternalLoadVtkGraphics_SurfacePlots(VtkControlBase)."
                + Environment.NewLine);
        }


        #region I3dGraphicsControl

        public void ChangeZoom(double factor)
        {
            _cameraViewAngle *= factor;
        }

        public void RotateAzimuth(double angleStepDegrees)
        {
            _cameraDirectionSpherical.y += angleStepDegrees;
        }

        public void RotatePitch(double angleStepDegrees)
        {
            _cameraDirectionSpherical.z += angleStepDegrees;
        }

        public void RotateRoll(double angleStepDegrees)
        {
            _cameraRoll += angleStepDegrees;
        }

        public double CameraViewAngle
        {
            get {
                return _cameraViewAngle;
            }
            set { 
                _cameraViewAngle = value;
            }
        }

        /// <summary>Roll of the camera (amount of rotation abount viewing direction)</summary>
        public double CameraRoll
        {
            get { 
                return _cameraRoll;
            }
            set { 
                _cameraRoll = value;
            }
        }

        /// <summary>Gets or sets camera position.</summary>
        public vec3 CameraPosition
        {
            get {
                return _cameraPosition;
            }
            set {
                _cameraPosition = value;
            }
        }


        /// <summary>Gets or sets camera focal point.</summary>
        public vec3 CameraFocalPoint
        {
            get
            {
                return _cameraFocalPoint;
            }
            set {
                _cameraFocalPoint = value;
            }
        }


        /// <summary>Gets or sets the camera viewing up position.</summary>
        public vec3 CameraViewUp
        {
            get
            {
                return _cameraViewUp;
            }
            set
            {
                _cameraViewUp = value;
           }
        }


        #endregion I3dGraphicsControl



#endif  // VTKFORMSDESIGN

        #endregion IVtkFormContainer


        #region Operation.Constants

        public const string KeysResetCamera = "R";

        public const string KeysJoystickMode = "J";

        public const string KeysTrackballMode = "T";

        public const string KeysCameraMode = "C";

        public const string KeysActorsMode = "A";

        public const string KeysWireframeMode = "W";

        public const string KeysSurfaceMode = "S";

        #endregion Operation.Constants


        #region Operation.Interaction


        /// <summary>Sends the specified sequence of keyboard keys to the embedded VTK control.
        /// <para>Meaning of keys: <see cref="UtilForms.GenerateKeyPress"/></para>
        /// <para>See also: http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx </para> </summary>
        /// <param name="keys">Specification of keys that are sent.</param>
        public void SendKeys(string keys)
        {
            UtilForms.GenerateKeyPress(this.VtkRenderWindowControl, keys);
        }

        /// <summary>Resets the camera in such a way that actors are centered in the visual field
        /// and all actors are visible.</summary>
        public void ResetCamera()
        {
            //this.VtkRenderer.ResetCamera();
            //SendKeys("ESC");
            SendKeys(KeysResetCamera);
        }


        private bool _isJoystickMode = true;

        /// <summary>Flag indicating whether the VTK window interactor is in joystick mode (i.e. constant rotation by 
        /// holding the mouse button, as opposed to the trackball mode where rotation is caused by dragging).</summary>
        public virtual bool IsJoystickMode
        {
            get { return _isJoystickMode; }
            set {
                _isJoystickMode = value;
                if (value == true)
                    SendKeys(KeysJoystickMode);
                else
                    SendKeys(KeysTrackballMode);
            }
        }

        /// <summary>Flag indicating whether the VTK window interactor is in trackball mode.
        /// <para>Bound to <see cref="IsJoystickMode"/> as its logical complement.</para></summary>
        public bool IsTrackballMode
        {
            get { return !IsJoystickMode; }
            set { IsJoystickMode = !value;
            }
        }


        private bool _isCameraMode = true;


        /// <summary>Flag indicating whether the embedded VTK control is in camera mode (i.e. the camera is moved, 
        /// as opposed to the actors mode where rotation and movement are performed on actors).</summary>
        public virtual bool IsCameraMode
        {
            get { return _isCameraMode; }
            set { _isCameraMode = value;
            if (value == true)
                SendKeys(KeysCameraMode);
            else
                SendKeys(KeysActorsMode);
            }
        }

        /// <summary>Flag indicating whether the embedded VTK control is in actors mode.
        /// <para>Bound to <see cref="IsCameraMode"/> as its logical complement.</para></summary>
        public virtual bool IsActorsMode
        {
            get { return !IsCameraMode; }
            set { IsCameraMode = !value; }
        }

        private bool _isWireframeMode = false;

        public bool IsWireframeMode
        {
            get { return _isWireframeMode; }
            set {
                _isWireframeMode = value;
                if (value == true)
                {
                    SendKeys(KeysWireframeMode);
                    _isSurfaceMode = false;
                }
            }
        }

        private bool _isSurfaceMode = false;

        public bool IsSurfaceMode
        {
            get { return _isSurfaceMode; }
            set {
                _isSurfaceMode = value;
                if (value == true)
                {
                    SendKeys(KeysSurfaceMode);
                    _isWireframeMode = false;
                }
            }
        }


        /// <summary>Handler for the Load event for the whole <see cref="VtkControlBase"/> form.</summary>
        private void VtkControlBase_Load(object sender, EventArgs e)
        {

        }



        #region Operation.Movement


        #endregion Operation.Movement


        #endregion Operation.Interaction



    }
}
