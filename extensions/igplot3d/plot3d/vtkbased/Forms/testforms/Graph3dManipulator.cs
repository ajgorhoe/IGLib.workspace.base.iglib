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

    /// <summary>Basic control for manipulating 3D graphics.</summary>
    /// $A Igor xx;
    public partial class Graph3dManipulator : UserControl
    {
        public Graph3dManipulator()
        {
            InitializeComponent();
            UpdateTextFields();
        }

        /// <summary>Costructs 3D graphics manipulator with the specified container  of the VTK control to be manipulated.</summary>
        /// <param name="vtkContainer">VTK container that contains the manipulated control.</param>
        public Graph3dManipulator(IVtkFormContainer vtkContainer)
            : this()
        {
            this.VtkContainer = vtkContainer;
        }


        /// <summary>Specifies whether the button that closes the main form is visible.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsCloseWindoButtonVisible
        {
            get { return btnClose.Visible; }
            set { btnClose.Visible = value; }
        }

        /// <summary>Specifies whether the button that opens the positioner form is visible.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsPositionerButtonVisible
        {
            get { return btnPositioner.Visible; }
            set { btnPositioner.Visible = value; }
        }

        #region VTK

        private IVtkFormContainer _vtkContainer;

        /// <summary>VTK container of type <see cref="IVtkFormContainer"/> through which VTK functionality is controlled.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IVtkFormContainer VtkContainer
        {
            get { return this._vtkContainer; }
            set
            {
                if (value != this._vtkContainer)
                {
                    this._vtkContainer = value;
                    // Update dependencies:
                    VtkControl = null;
                    this.UpdateTextFields();
                }
            }
        }


        private VtkControlBase _vtkControl;

        /// <summary>Gets the control of type <see cref="VtkControlBase"/> through which VTK functionality is controlled.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public VtkControlBase VtkControl
        {
            get
            {
                if (_vtkControl == null)
                {
                    if (VtkContainer != null)
                        _vtkControl = VtkContainer.VtkControl;
                }
                return this._vtkControl;
            }
            set
            {
                if (value != _vtkControl)
                {
                    this._vtkControl = value;
                    if (Positioner != null)
                        Positioner.MasterWindow = ManipulatedForm;
                }
            }
        }


        /// <summary>Graph control that is manipulated by the current control.</summary>
        public I3dGraphicsControl GraphControl
        {
            get
            {
                return VtkControl;
            }
        }

        #endregion VTK

        
        #region Positioning

        bool _isPositioned = true;

        /// <summary>Whether the form containing the current manipulator control is positioned automatically
        /// relative to the form containing the manipulated control.</summary>
        bool IsPositioned
        {
            get {
                if (_isPositioned == true)
                {
                    // Control's form can not be positioned if the control is contained in the
                    // same form as the manipulated control:
                    if (ManipulatedForm == ParentForm && ParentForm != null)
                        IsPositioned = false;
                }
                return _isPositioned;
            }
            set {
                if (value!=_isPositioned)
                {
                    _isPositioned = value;
                    if (Positioner !=null)
                    {
                        if (value == true)
                            InitPositioner();
                        else
                        {
                            Positioner.UnStickFromMaster();
                            Positioner.StopRememberPosition();
                        }
                    }
                    if (value == true)
                        IsPositionerButtonVisible = true;
                    else
                        IsPositionerButtonVisible = false;
                }

            }
        }

        /// <summary>The form that contains the mainipulated graphic window.</summary>
        public Form ManipulatedForm
        {
            get {
                Form ret = null;
                if (VtkControl != null)
                {
                    ret = VtkControl.ParentForm;
                }
                return ret;
            }
        }

        public void InitPositioner()
        {
            WindowPositioner positioner = this.Positioner;
            if (positioner != null && IsPositioned)
            {
                positioner.MasterWeight = 1.0;
                positioner.WindowAlignment = new Alignment(AlignmentHorizontal.Left, AlignmentVertical.Top);
                positioner.WindowShiftRelative = new vec2(0, 0);
                positioner.ShiftAbsolute = new vec2(0, 5);
                positioner.MasterWindowAlignment = new Alignment(AlignmentHorizontal.Left, AlignmentVertical.Bottom);
                positioner.MasterWindowShiftRelative = new vec2(0, 0);
                positioner.Window = ParentForm;
                positioner.MasterWindow = ManipulatedForm;
                positioner.StickToMaster();
                positioner.RememberPosition();
            }
        }

        protected WindowPositioner _positioner;

        /// <summary>Window positioner.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WindowPositioner Positioner
        {
            get
            {
                if (_positioner == null)
                {
                    _positioner = new WindowPositioner();
                    InitPositioner();
                }
                return _positioner;
            }
            protected set
            {
                if (value != _positioner)
                {
                    _positioner = value;
                    if (value != null)
                        InitPositioner();
                }
            }
        }


        /// <summary>Method that is called in event handlers to initialize the window positioning system for the 
        /// form containing the current manipulator control.</summary>
        protected void PositionInitializeEventHandler()
        {
            // The following will cause the form containing the manipulator to be automatically positioned relative to the
            // form containing the manipulated graphics, if thse forms are not the same:
            if (IsPositioned)
            {
                WindowPositioner positioner = this.Positioner;
                if (positioner != null)
                    positioner.PositionWindow();
            }
            // Add event that will close the manipulator window when window containing manipulated graphics is closed:
            if (ManipulatedForm != null && ParentForm != null && ParentForm != ManipulatedForm)
            {
                ManipulatedForm.FormClosing += (objSender, eventArgs) =>
                {
                    try
                    {
                        ParentForm.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(Environment.NewLine + Environment.NewLine
                            + "ERROR occurred when closing manipulator control because of manipulated control closing: " + Environment.NewLine
                            + "  " + ex.Message + Environment.NewLine);
                    }
                };
            }
        }


        protected WindowPositionerForm _positionerForm;

        /// <summary>A GIU panel for setting window positioner properties.</summary>
        protected WindowPositionerForm PositionerForm
        {
            get
            {
                if (_positionerForm != null)
                    if (_positionerForm.IsDisposed)
                        _positionerForm = null;
                if (_positionerForm == null)
                {
                    _positionerForm = new WindowPositionerForm(Positioner, ParentForm, ManipulatedForm);
                }
                _positionerForm.Positioner = this.Positioner;
                _positionerForm.Window = this.ParentForm;
                _positionerForm.MasterWindow = this.ManipulatedForm;
                return _positionerForm;
            }
            set
            {
                _positionerForm = value;
            }
        }

        /// <summary>Method that launches the panel where the window positioner properties can be changed and tested.</summary>
        protected void LaunchPositionerGuiPanel()
        {
            if (IsPositioned)
            {
                if (ManipulatedForm != null && ParentForm != null && ParentForm != ManipulatedForm)
                {
                    Form posForm = PositionerForm;
                    if (posForm != null)
                    {
                        if (!posForm.Visible)
                        {
                            posForm.ShowDialog();
                        }
                    }
                }
            }
        }

        #endregion Positioning


        #region Operation

        protected bool _suspendEventHandlers = false;


        /// <summary>Updates control's text fields according to values on the manipulated control.</summary>
        public void UpdateTextFields()
        {
            try
            {
                _suspendEventHandlers = true;
                if (GraphControl != null)
                {
                    //if (_numUpdatesToGo < 0 || _numUpdatesToGo > 0)
                    //{
                    txtRotationStep.Text = GraphControl.RotationStep.ToString();
                    txtZoomFactor.Text = GraphControl.ZoomFactor.ToString();
                    txtViewAngle.Text = (Math.Round(100 * GraphControl.CameraViewAngle) / 100.0).ToString();
                    vec3 dir = GraphControl.CameraDirectionSpherical;
                    txtR.Text = (Math.Round(100 * dir.x) / 100.0).ToString();
                    txtFi.Text = (Math.Round(100 * dir.y) / 100.0).ToString();
                    txtTheta.Text = (Math.Round(100 * dir.z) / 100.0).ToString();
                    txtRoll.Text = (Math.Round(100 * GraphControl.CameraRoll) / 100.0).ToString();
                }
                //    if (_numUpdatesToGo > 0)
                //        --_numUpdatesToGo;
                //}
            }
            catch { throw; }
            finally {
                Util.SleepSeconds(0.05);
                _suspendEventHandlers = false;
            }
        }

        /// <summary>Closes the top level form if it implements the IClosableFromContainedForm interface.</summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Form topLevelForm = this.TopLevelControl as Form;
            if (topLevelForm != null && topLevelForm is IG.Forms.IClosableFromContainedForm)
            {
                topLevelForm.Close();
            }
            else
            {
                // Form's top level parent can not be closed, blink the button:
                UtilForms.BlinkControl(btnClose);
            }
        }


        /// <summary>Refreshes values in the text fields.</summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateTextFields();
        }

        private void btnUpLeft_Click(object sender, EventArgs e)
        {
            if (GraphControl!=null)
            {
                GraphControl.RotatePitch(GraphControl.RotationStep/Math.Sqrt(2));
                GraphControl.RotateAzimuth(-GraphControl.RotationStep / Math.Sqrt(2));
                UpdateTextFields();
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (GraphControl!=null)
            {
                GraphControl.RotatePitch(GraphControl.RotationStep);
                UpdateTextFields();
            }
        }

        private void btnUpRight_Click(object sender, EventArgs e)
        {
            if (GraphControl != null)
            {
                GraphControl.RotatePitch(GraphControl.RotationStep / Math.Sqrt(2));
                GraphControl.RotateAzimuth(GraphControl.RotationStep / Math.Sqrt(2));
                UpdateTextFields();
            }
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (GraphControl != null)
            {
                GraphControl.RotateAzimuth(-GraphControl.RotationStep);
                UpdateTextFields();
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (GraphControl != null)
            {
                GraphControl.RotateAzimuth(GraphControl.RotationStep);
                UpdateTextFields();
            }
        }

        private void btnDownLeft_Click(object sender, EventArgs e)
        {
            if (GraphControl != null)
            {
                GraphControl.RotatePitch(-GraphControl.RotationStep / Math.Sqrt(2));
                GraphControl.RotateAzimuth(-GraphControl.RotationStep / Math.Sqrt(2));
                UpdateTextFields();
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (GraphControl!=null)
            {
                GraphControl.RotatePitch(-GraphControl.RotationStep);
                UpdateTextFields();
            }
        }

        private void btnDownRight_Click(object sender, EventArgs e)
        {
            if (GraphControl != null)
            {
                GraphControl.RotatePitch(-GraphControl.RotationStep / Math.Sqrt(2));
                GraphControl.RotateAzimuth(GraphControl.RotationStep / Math.Sqrt(2));
                UpdateTextFields();
            }

        }

        private void btnRollCounterClockwise_Click(object sender, EventArgs e)
        {
            if (GraphControl!=null)
            {
                GraphControl.RotateRoll(-GraphControl.RotationStep);
                UpdateTextFields();
            }
        }

        private void btnRollClockwise_Click(object sender, EventArgs e)
        {
            if (GraphControl!=null)
            {
                GraphControl.RotateRoll(GraphControl.RotationStep);
                UpdateTextFields();
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if (GraphControl != null)
            {
                GraphControl.ChangeZoom(GraphControl.ZoomFactor);
                UpdateTextFields();
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (GraphControl != null)
            {
                GraphControl.ChangeZoom(1.0/GraphControl.ZoomFactor);
                UpdateTextFields();
            }
        }


        private void txtViewAngle_Validated(object sender, EventArgs e)
        {
            if (! _suspendEventHandlers && GraphControl != null)
            {
                bool parsed = false;
                double newValue = GraphControl.CameraViewAngle;
                parsed = double.TryParse(txtViewAngle.Text, out newValue);
                if (!parsed)
                    UtilForms.BlinkControl(txtViewAngle);
                else
                {
                    GraphControl.CameraViewAngle = newValue;
                    UpdateTextFields();
                }
            }
        }

        private void txtR_Validated(object sender, EventArgs e)
        {
            if (!_suspendEventHandlers && GraphControl != null)
            {
                vec3 dir = GraphControl.CameraDirectionSpherical;
                bool parsed = false;
                double newValue = dir.x;
                parsed = double.TryParse(txtR.Text, out newValue);
                if (!parsed)
                    UtilForms.BlinkControl(txtR);
                else
                {
                    dir.x = newValue;
                    GraphControl.CameraDirectionSpherical = dir;
                    UpdateTextFields();
                }
            }
        }

        private void txtFi_Validated(object sender, EventArgs e)
        {
            if (!_suspendEventHandlers && GraphControl != null)
            {
                vec3 dir = GraphControl.CameraDirectionSpherical;
                bool parsed = false;
                double newValue = dir.y;
                parsed = double.TryParse(txtFi.Text, out newValue);
                if (!parsed)
                    UtilForms.BlinkControl(txtFi);
                else
                {
                    dir.y = newValue;
                    GraphControl.CameraDirectionSpherical = dir;
                    UpdateTextFields();
                }
            }
        }

        private void txtTheta_Validated(object sender, EventArgs e)
        {
            if (!_suspendEventHandlers && GraphControl != null)
            {
                vec3 dir = GraphControl.CameraDirectionSpherical;
                bool parsed = false;
                double newValue = dir.z;
                parsed = double.TryParse(txtTheta.Text, out newValue);
                if (!parsed)
                    UtilForms.BlinkControl(txtTheta);
                else
                {
                    dir.z = newValue;
                    GraphControl.CameraDirectionSpherical = dir;
                    UpdateTextFields();
                }
            }
        }

        private void txtRoll_Validated(object sender, EventArgs e)
        {
            if (!_suspendEventHandlers && GraphControl != null)
            {
                bool parsed = false;
                double newValue = GraphControl.CameraRoll;
                parsed = double.TryParse(txtRoll.Text, out newValue);
                if (!parsed)
                    UtilForms.BlinkControl(txtRoll);
                else
                {
                    GraphControl.CameraRoll = newValue;
                    UpdateTextFields();
                }
            }
        }

        private void txtRotationStep_Validated(object sender, EventArgs e)
        {
            if (!_suspendEventHandlers && GraphControl != null)
            {
                bool parsed = false;
                double newValue = GraphControl.RotationStep;
                parsed = double.TryParse(txtRotationStep.Text, out newValue);
                if (!parsed)
                    UtilForms.BlinkControl(txtRotationStep);
                else
                {
                    GraphControl.RotationStep = newValue;
                    UpdateTextFields();
                }
            }
        }


        private void txtZoomFactor_Validated(object sender, EventArgs e)
        {
            if (!_suspendEventHandlers && GraphControl != null)
            {
                bool parsed = false;
                double newValue = GraphControl.ZoomFactor;
                parsed = double.TryParse(txtZoomFactor.Text, out newValue);
                if (!parsed)
                    UtilForms.BlinkControl(txtZoomFactor);
                else
                {
                    GraphControl.ZoomFactor = newValue;
                    UpdateTextFields();
                }
            }
        }


        // Select text in items that receive focus:

        private void txtRotationStep_Enter(object sender, EventArgs e)
        {
            txtRotationStep.SelectAll();
        }

        private void txtZoomFactor_Enter(object sender, EventArgs e)
        {
            txtZoomFactor.SelectAll();
        }

        private void txtR_Enter(object sender, EventArgs e)
        {
            txtR.SelectAll();
        }

        private void txtFi_Enter(object sender, EventArgs e)
        {
            txtFi.SelectAll();
        }

        private void txtTheta_Enter(object sender, EventArgs e)
        {
            txtTheta.SelectAll();
        }

        private void txtRoll_Enter(object sender, EventArgs e)
        {
            txtRoll.SelectAll();
        }

        private void txtViewAngle_Enter(object sender, EventArgs e)
        {
            txtViewAngle.SelectAll();
        }

        #endregion Operation



        private void Graph3dManipulator_Load(object sender, EventArgs e)
        {
            PositionInitializeEventHandler();
        }

        private void btnPositioner_Click(object sender, EventArgs e)
        {
            LaunchPositionerGuiPanel();
        }


    }  // class Graph3dManipulator

}
