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

    /// <summary>A form for manipulating 3D graphics.</summary>
    /// $A Igor xx Jun13;
    public partial class Graph3dManipulatorWindowExtended : Form, IG.Forms.IClosableFromContainedForm
        
    {
        public Graph3dManipulatorWindowExtended()
        {
            InitializeComponent();
        }

        public Graph3dManipulatorWindowExtended(IVtkFormContainer vtkContainer)
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
                    graph3DManipulator1.VtkContainer = value;
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



        private void graph3DManipulator1_Load(object sender, EventArgs e)
        {

        }

    }
}
