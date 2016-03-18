using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IG.Lib;
using IG.Num;
using IG.Forms;

namespace IG.Gr3d
{
    
    /// <summary>Basic manipulation of VTK-based 3D hraphs.</summary>
    /// $A Igor May13;
    public partial class Graph3dManipulatorBasic : UserControl
    {
        public Graph3dManipulatorBasic()
        {
            InitializeComponent();
        }

        

        /// <summary>Costructs 3D graphics manipulator with the specified vtk container whose graph control is to be manipulated.</summary>
        /// <param name="vtkContainer">Container of the VTK control (must be of type implementing the <see cref="IVtkFormContainer"/> interface).</param>
        public Graph3dManipulatorBasic(IVtkFormContainer vtkContainer)
            : this()
        {
            this.VtkContainer = vtkContainer;

            if (VtkControl.IsJoystickMode)
                btnTrackBallJoystick.Text = StrTrackballMode;
            else
                btnTrackBallJoystick.Text = StrJoystickMode;
            if (VtkControl.IsCameraMode)
                btnCameraActor.Text = StrActorsMode;
            else
                btnCameraActor.Text = StrCameraMode;
            if (!VtkControl.IsWireframeMode)
                btnSurfaceWireframe.Text = StrWireframe;
            else
                btnSurfaceWireframe.Text = StrSurfaces;
        }


        /// <summary>Specifies whether the button that launchs the 3D manipulation controls is visible.</summary>
        public bool IsManipulateButtonVisible
        {
            get { return btnManipulate.Visible; }
            set { btnManipulate.Visible = value; }
        }

        /// <summary>Specifies whether the button that closes the main form is visible.</summary>
        public bool IsCloseWindoButtonVisible
        {
            get { return btnCloseWin.Visible; }
            set { btnCloseWin.Visible = value; }
        }

        private IVtkFormContainer _vtkContainer;

        /// <summary>Gets the control of type <see cref="VtkControlBase"/> through which VTK functionality is controlled.</summary>
        public IVtkFormContainer VtkContainer
        {
            get { return this._vtkContainer; }
            set {
                if (value != this._vtkContainer)
                {
                    this._vtkContainer = value;
                    // Update dependencies:
                    VtkControl = null;
                }
            }
        }


        private VtkControlBase _vtkControl;

        /// <summary>Gets the control of type <see cref="VtkControlBase"/> through which VTK functionality is controlled.</summary>
        public VtkControlBase VtkControl
        {
            get {
                if (_vtkControl == null)
                {
                    if (VtkContainer != null)
                        _vtkControl = VtkContainer.VtkControl;
                }
                return this._vtkControl; }
            set
            {
                if (value != _vtkControl)
                {
                    this._vtkControl = value;
                    //// Update dependencies:
                    //
                }
            }
        }


        /// <summary>Graph control that is manipulated by the current control.</summary>
        public I3dGraphicsControl GraphControl
        {
            get { return VtkControl; }
        }



        #region Operation.Constants

        public const string StrJoystickMode = "Trackball Mode";

        public const string StrTrackballMode = "Joystick Mode";

        public const string StrCameraMode = "Actors Mode";

        public const string StrActorsMode = "Camera Mode";

        public const string StrWireframe = "Surfaces";

        public const string StrSurfaces = "Wireframe";

        #endregion Operation.Constants


        /// <summary>Closes the toplevel form if it implements the <see cref="IClosableFromContainedForm"/> interface.</summary>
        private void btnCloseWin_Click(object sender, EventArgs e)
        {
            Form topLevelForm = this.TopLevelControl as Form;
            if (topLevelForm != null && topLevelForm is IG.Forms.IClosableFromContainedForm)
            {
                topLevelForm.Close();
            }
            else
            {
                // Form's top level parent can not be closed, blink the button:
                UtilForms.BlinkControl(btnCloseWin);
            }
        }

        private void btnResetCamera_Click(object sender, EventArgs e)
        {
            VtkControl.ResetCamera();
        }

        private void btnTrackBallJoystick_Click(object sender, EventArgs e)
        {

            bool isJoistick = VtkControl.IsJoystickMode;
            VtkControl.IsJoystickMode = !isJoistick;
            if (VtkControl.IsJoystickMode)
            {
                btnTrackBallJoystick.Text = StrTrackballMode;
            }
            else
            {
                btnTrackBallJoystick.Text = StrJoystickMode;
            }
        }

        private void btnCameraActor_Click(object sender, EventArgs e)
        {
            bool isCamera = VtkControl.IsCameraMode;
            VtkControl.IsCameraMode = !isCamera;
            if (VtkControl.IsCameraMode)
            {
                btnCameraActor.Text = StrActorsMode;
            }
            else
            {
                btnCameraActor.Text = StrCameraMode;
            }
        }

        private void btnSurfaceWireframe_Click(object sender, EventArgs e)
        {
            bool isWireframe = VtkControl.IsWireframeMode;
            if (!isWireframe)
                VtkControl.IsWireframeMode = true;
            else
                VtkControl.IsSurfaceMode = true;
            if (!VtkControl.IsWireframeMode)
            {
                btnSurfaceWireframe.Text = StrWireframe;
            }
            else
            {
                btnSurfaceWireframe.Text = StrSurfaces;
            }
        }

            
        Graph3dManipulatorWindow _graphControls;

        Graph3dManipulatorWindow GraphControls
        {
            get {
                bool create = false;
                if (_graphControls == null)
                    create = true;
                else if (_graphControls.IsDisposed)
                    create = true;
                if (create)
                {
                    _graphControls = new Graph3dManipulatorWindow(this.VtkControl);
                }
                if (_graphControls != null)
                {
                    Point screenPosition = btnManipulate.Parent.PointToScreen(btnManipulate.Location);
                    _graphControls.Location = screenPosition;
                }
                return _graphControls;
            }
        }

        private void btnManipulate_Click(object sender, EventArgs e)
        {
            GraphControls.Show();
        }


    } // class Graph3dManipulatorBasic
}
