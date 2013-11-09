// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IG.Lib;
using IG.Forms;

namespace IG.Gr3d
{

    /// <summary>Windows form for presenting VTK graphics.
    /// <para>Contains some additional buttons beside just the VTK renderer control.</para></summary>
    /// <remarks>This is a toplevel form that only contains a control of type <see cref="VtkControlWin"/>, which 
    /// handles all VTK related stuff.</remarks>
    /// $A Igor xx May13;
    public partial class VtkForm : Form, IVtkFormContainer, 
        IG.Forms.IClosableFromContainedForm
    {

        public VtkForm()
        {
            InitializeComponent();

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



    }
}
