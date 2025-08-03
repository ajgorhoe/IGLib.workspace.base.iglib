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
 
    /// <summary>Extended control for manipulation of 3D VTK graphics. Contains the standard control plus
    /// additional buttons for access to some VTK functions.</summary>
    public partial class Graph3dManipulatorExtended : UserControl
    {
        public Graph3dManipulatorExtended()
        {
            InitializeComponent();
            this.graph3dManipulatorBasic1.IsManipulateButtonVisible = false;
            this.graph3dManipulator1.IsCloseWindoButtonVisible = false;
        }



        /// <summary>Costructs 3D graphics manipulator with the specified vtk container whose graph control is to be manipulated.</summary>
        /// <param name="vtkContainer">Container of the VTK control (must be of type implementing the <see cref="IVtkFormContainer"/> interface).</param>
        public Graph3dManipulatorExtended(IVtkFormContainer vtkContainer)
            : this()
        {
            this.VtkContainer = vtkContainer;
        }


        private IVtkFormContainer _vtkContainer;

        /// <summary>Gets the control of type <see cref="VtkControlBase"/> through which VTK functionality is controlled.</summary>
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
                    graph3dManipulator1.VtkContainer = value;
                    graph3dManipulatorBasic1.VtkContainer = value;
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
                    //// Update dependencies:
                    //
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




    }  // class Graph3dManipulatorExtended
}
