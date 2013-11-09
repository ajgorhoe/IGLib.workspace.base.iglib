using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Kitware.VTK;

using IG.Lib;
using IG.Num;

namespace IG.Gr3d
{

    /// <summary>Plots contours on surfaces in 3D.</summary>
    /// <remarks>This class can either be used to plot contours on a surface, with arbitrary definition of values,
    /// or contours projected on XY plane.</remarks>
    /// $A Igor xx Nov11;
    public class VtkContourPlot : VtkSurfacePlot
    {

        
        public VtkContourPlot(VtkPlotter plotter)
            : this(plotter, null)
        { }

        public VtkContourPlot(VtkPlotter plotter, StructuredMesh2d3d mesh)
            : base(plotter, mesh)
        {
            this.IsContourPlot = true;
            this.LinesVisible = true;
            this.SurfacesVisible = false;
            this.PointsVisible = false;

            this.NumContours = DefaultNumContours;
        }

        /// <summary>Whether the current object is intended for contour plots or not (in this case it is intended for
        /// surface plots).</summary>
        public bool IsContourPlot
        {
            get { return _isContourPlot; }
            protected set { _isContourPlot = value; }
        }

        #region Data


        /// <summary>Number of contours plotted in contour plots. 
        /// <para>When set, the number must be greater than zero.</para></summary>
        public int NumContours
        {
            get { return _numContours; }
            set { if (value < 1) throw new ArgumentException("Number of contours can not be les than 1."); _numContours = value; }
        }

        #endregion Data


    } // class VtkContourPlot


}
