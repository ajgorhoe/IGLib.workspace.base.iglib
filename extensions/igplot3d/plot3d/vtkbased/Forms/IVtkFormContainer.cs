using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IG.Gr3d
{
    

        
    /// <summary>This interface is implemented by the forms that contain the VTK control that can 
    /// render VTK graphics.</summary>
    /// $A Igor xx Jun13;
#if (!VTKFORMSDESIGN)
    public interface IVtkFormContainer : IVtkFormContainerGen<Kitware.VTK.RenderWindowControl>
#else
    public interface IVtkFormContainer: IVtkFormContainerGen<System.Windows.Forms.Control>
#endif
    {
    }




    /// <summary>This interface is implemented by the forms that contain the VTK control that can 
    /// render VTK graphics.</summary>
    /// $A Igor xx Jun13;
    public interface IVtkFormContainerGen<VtkControlType>
        where VtkControlType: System.Windows.Forms.Control
    {

        /// <summary>Base windows forms control through which the basic VTK functionality and all additional features can be
        /// accessed.</summary>
        VtkControlBase VtkControl
        {
            get;
        }

        /// <summary>VTK's control that is actually used for rendering graphics and through which VTK can be interacted directly.
        /// <para>Instead of accessing this control directly, you should normally access it through the <see cref="VtkControlBase.VtkControl"/> property
        /// on the <see cref="VtkControl"/> property.</para></summary>
        VtkControlType VtkRenderWindowControl
        {
            get;
        }

    }


}
