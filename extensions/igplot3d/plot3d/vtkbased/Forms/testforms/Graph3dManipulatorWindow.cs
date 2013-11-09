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

    /// <summary>A form for manipulation of 3D graphics.</summary>
    /// $A Igor xx;
    public partial class Graph3dManipulatorWindow : Form, IG.Forms.IClosableFromContainedForm
        
    {
        public Graph3dManipulatorWindow()
        {
            InitializeComponent();
        }

        public Graph3dManipulatorWindow(IVtkFormContainer vtkContainer)
            : this()
        {
            this.VtkContainer = vtkContainer; ;
        }


        private IVtkFormContainer _vtkContainer;

        /// <summary>Gets the control of type <see cref="VtkControlBase"/> through which VTK functionality is controlled.</summary>
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
                    this.graph3DManipulator1.VtkContainer = value;
                    //graph3dManipulator1.VtkContainer = value;
                }
            }
        }


        private VtkControlBase _vtkControl;

        /// <summary>Gets the control of type <see cref="VtkControlBase"/> through which VTK functionality is controlled.</summary>
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





        //I3dGraphicsControl GraphControl
        //{
        //    get { return graph3DManipulator1.GraphControl; }
        //    set { this.graph3DManipulator1.GraphControl = value; }
        //}

        private void graph3DManipulator1_Load(object sender, EventArgs e)
        {

        }

    }
}
