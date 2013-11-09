// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IG.Gr3d
{

    /// <summary>Windows form for presenting VTK graphics.
    /// <para>Contains some additional buttons beside just the VTK renderer control.</para></summary>
    /// <remarks>This is a toplevel form that only contains a control of type <see cref="VtkControlWin"/>, which 
    /// handles all VTK related stuff.</remarks>
    /// $A Igor May13;
    public partial class VtkFormPlain : Form, IVtkFormContainer, 
        IG.Forms.IClosableFromContainedForm
    {
        public VtkFormPlain()
        {
            InitializeComponent();

            //this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VtkFormPlain_KeyUp);

            //this.vtkControlWin1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VtkFormPlain_KeyUp);

            //this.VtkControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VtkFormPlain_KeyUp);

            //this.VtkControl.VtkRenderWindowControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.VtkFormPlain_KeyUp);

            btnManipulatorControls.Visible = IsManipulatorControlsButtonVisible;


            // Reset state of the Vtk control since automatically generated designer's code overrides the settings:
            this.VtkControl.InitializeState();
            // this.VtkControl.InitializeVtkRendering();

        }

        public static bool DefaultIsManipulatorControlsButtonVisible = true;

        protected bool _isManipulatorControlsButtonVisible = DefaultIsManipulatorControlsButtonVisible;

        /// <summary>Specifies whether the button that launches manipulator controls is visible or not.
        /// <para>If true, a small button sows in the bottom-right corner of the VTK plotting area.</para></summary>
        public bool IsManipulatorControlsButtonVisible
        {
            get {
                return _isManipulatorControlsButtonVisible;
            }
            set {
                _isManipulatorControlsButtonVisible = value;
                btnManipulatorControls.Visible = value;
            }
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
                    _vtkControl = vtkControlWin1.VtkControl;
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

        #region Manipulator


        protected Graph3dManipulatorWindowExtended _manipulatorWindow;

        /// <summary>Window that shows information about the software.
        /// <para>Help is also accessible through that window.</para></summary>
        public Graph3dManipulatorWindowExtended ManipulatorWindow
        {
            get
            {
                if (_manipulatorWindow == null)
                {
                    _manipulatorWindow = new Graph3dManipulatorWindowExtended(this.VtkControl);
                }
                return _manipulatorWindow;
            }
        }


        public void ShowManipulator()
        {
            _manipulatorWindow = new Graph3dManipulatorWindowExtended(this.VtkControl);
            _manipulatorWindow.Show();
        }

        /// <summary>Used to handle key up events that launch the manipulator window that has controls for 
        /// rotating 3D graphics, zooming, and others.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VtkFormPlain_HelpEventHandlers(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                ShowManipulator();
            }
        }



        /// <summary>Key up event.</summary>
        private void VtkFormPlain_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Key up event occurred, key = " + e.KeyCode);
            this.VtkFormPlain_HelpEventHandlers(sender, e);
        }


        /// <summary>Opens window with manipulator controls for manipulation of graphics (rotation, etc.).</summary>
        private void btnManipulatorControls_Click(object sender, EventArgs e)
        {
            ShowManipulator();
        }


        #endregion Manipulator



    }  // Class VtkFormPlain
}
