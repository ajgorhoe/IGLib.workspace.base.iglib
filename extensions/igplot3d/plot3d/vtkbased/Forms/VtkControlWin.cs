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
using IG.Forms;

namespace IG.Gr3d
{

    /// <summary>Control for presenting VTK graphics, intended for use in stand-alone windows.</summary>
    /// <remarks><para>This control contains the <see cref="VtkControlBase"/> control with sme additional controls, e.g.
    /// for closing the containing toplevel window.</para></remarks>
    /// $A Igor xx May13;
    public partial class VtkControlWin : UserControl, IVtkFormContainer
    {
        public VtkControlWin()
        {
            InitializeComponent();

            graph3dManipulatorBasic1.VtkContainer = this;

            //graph3dManipulatorBasic1.IsManipulateButtonVisible = false;
            
            // Reset state of the Vtk control since automatically generated designer's code overrides the settings:
            this.VtkControl.InitializeState();
            // this.VtkControl.InitializeVtkRendering();

        }



        #region IVtkFormContainer

        private VtkControlBase _vtkControl;

        /// <summary>Gets the control of type <see cref="VtkControlBase"/> through which VTK functionality is controlled.</summary>
        public VtkControlBase VtkControl
        {
            get
            {
                if (_vtkControl == null)
                {
                    _vtkControl = vtkControlBase1.VtkControl;
                }
                return _vtkControl;
            }
        }

#if (!VTKFORMSDESIGN)

        private Kitware.VTK.RenderWindowControl _vtkRenderWindowControl;
        
        /// <summary>VTK's control that is actually used for rendering graphics and through which VTK can be interacted directly.
        /// <para>Instead of accessing this control directly, you should normally access it through the <see cref="VtkControlBase.VtkControl"/> property
        /// of the <see cref="VtkControl"/> property.</para></summary>
        public Kitware.VTK.RenderWindowControl VtkRenderWindowControl
        {
            get
            {
                if (_vtkRenderWindowControl == null)
                {
                    if (VtkControl != null)
                        _vtkRenderWindowControl = VtkControl.VtkRenderWindowControl;

                }
                return _vtkRenderWindowControl;
            }
        }

#else // if (!VTKFORMSDESIGN)

        System.Windows.Forms.Control _vtkRenderWindowControl;


        /// <summary>VTK's control that is actually used for rendering graphics and through which VTK can be interacted directly.
        /// <para>Instead of accessing this control directly, you should normally access it through the <see cref="VtkControlBase.VtkControl"/> property
        /// of the <see cref="VtkControl"/> property.</para></summary>
        public System.Windows.Forms.Control VtkRenderWindowControl
        {
            get
            {
                if (_vtkRenderWindowControl == null)
                {
                    if (VtkControl != null)
                        _vtkRenderWindowControl = VtkControl.VtkRenderWindowControl;

                }
                return _vtkRenderWindowControl;
            }
        }

#endif  // (!VTKFORMSDESIGN)


        #endregion IVtkFormContainer


        //#region Operation.Constants

        //public const string StrToJoystickMode = Graph3dManipulatorBasic.StrToJoystickMode; // "Joystick Mode";

        //public const string StrToTrackballMode = Graph3dManipulatorBasic.StrToTrackballMode; // "Trackball Mode";

        //public const string StrToCameraMode = Graph3dManipulatorBasic.StrToCameraMode; // "Camera Mode";

        //public const string StrToActorsMode = Graph3dManipulatorBasic.StrToActorsMode; // "Actors Mode";

        //public const string StrToWireframe = Graph3dManipulatorBasic.StrToWireframe; // "Wireframe";

        //public const string StrToSurfaces = Graph3dManipulatorBasic.StrToSurfaces; // "Surfaces";

        //#endregion Operation.Constants


        #region Buttons

        ///// <summary>Closes the toplevel form if it implements the <see cref="IClosableFromContainedForm"/> interface.</summary>
        //private void btnCloseWin_Click(object sender, EventArgs e)
        //{
        //    Form topLevelForm = this.TopLevelControl as Form;
        //    if (topLevelForm != null && topLevelForm is IG.Forms.IClosableFromContainedForm)
        //    {
        //        topLevelForm.Close();
        //    } else
        //    {
        //        // Form's top level parent can not be closed, blink the button:
        //        UtilForms.BlinkForm(btnCloseWin);
        //    }
        //}

        //private void btnResetCamera_Click(object sender, EventArgs e)
        //{
        //    VtkControl.ResetCamera();
        //}

        //private void btnTrackBallJoystick_Click(object sender, EventArgs e)
        //{
        //    bool isJoistick = VtkControl.IsJoystickMode;
        //    VtkControl.IsJoystickMode = !isJoistick;
        //    if (VtkControl.IsJoystickMode)
        //    {
        //        btnTrackBallJoystick.Text = StrToTrackballMode;
        //    }
        //    else
        //    {
        //        btnTrackBallJoystick.Text = StrToJoystickMode;
        //    }
        //}


        //private void btnCameraActor_Click(object sender, EventArgs e)
        //{
        //    bool isCamera = VtkControl.IsCameraMode;
        //    VtkControl.IsCameraMode = !isCamera;
        //    if (VtkControl.IsCameraMode)
        //    {
        //        btnCameraActor.Text = StrToActorsMode;
        //    }
        //    else
        //    {
        //        btnCameraActor.Text = StrToCameraMode;
        //    }
        //}

        //private void btnSurfaceWireframe_Click(object sender, EventArgs e)
        //{
        //    bool isWireframe = VtkControl.IsWireframeMode;
        //    if (!isWireframe)
        //        VtkControl.IsWireframeMode = true;
        //    else
        //        VtkControl.IsSurfaceMode = true;
        //    if (!VtkControl.IsWireframeMode)
        //    {
        //        btnSurfaceWireframe.Text = StrToWireframe;
        //    }
        //    else
        //    {
        //        btnSurfaceWireframe.Text = StrToSurfaces;
        //    }
        //}


        //Graph3dManipulatorWindow _graphControls;

        //Graph3dManipulatorWindow GraphControls
        //{
        //    get
        //    {
        //        bool create = false;
        //        if (_graphControls == null)
        //            create = true;
        //        else if (_graphControls.IsDisposed)
        //            create = true;
        //        if (create)TCCCTT
        //        {
        //            _graphControls = new Graph3dManipulatorWindow(this.VtkControl);
        //        }
        //        if (_graphControls != null)
        //        {
        //            Point screenPosition = btnManipulate.Parent.PointToScreen(btnManipulate.Location);
        //            _graphControls.Location = screenPosition;
        //        }
        //        return _graphControls;
        //    }
        //}

        //private void btnManipulate_Click(object sender, EventArgs e)
        //{
        //    GraphControls.Show();
        //}


        #endregion Buttons


    }
}
