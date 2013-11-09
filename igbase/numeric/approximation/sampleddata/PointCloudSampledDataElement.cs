using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Cloud of points where each point is represented by the <see cref="IVector"/> object, contains a list
    /// of containers of vector objects that include point coordinates.</summary>
    /// <typeparam name="PointContainerType">Type of point link that is used by point container.</typeparam>
    /// <typeparam name="PointType">Type of objects that include point coordinates.</typeparam>
    /// $A Igor Sep08 May09 Dec11;
    public class PointCloudSampledDataElement : PointCloud<PointContainerSampledDataElement, SampledDataElement>, ILockable
    {

        /// <summary>Constructs a cloud of ponts where points are of vector type.</summary>
        public PointCloudSampledDataElement()
            : base()
        { }

        /// <summary>Constructs a cloud of vector points containing the specified points.</summary>
        /// <param name="points">Points that are included in the created point cloud.</param>
        public PointCloudSampledDataElement(params SampledDataElement[] points)
            : this()
        { AddPoints(points); }

        /// <summary>Constructs a cloud of vector points containing the points embedded in the specified point containers.
        /// <para>All points are taken from their point containers and embedded in newly created point containers before
        /// adding to the created cloud of points.</para></summary>
        /// <param name="points">Points embedded in point containers that are included in the created point cloud.</param>
        public PointCloudSampledDataElement(params IPointContainer<SampledDataElement>[] points)
            : this()
        { AddPoints(points); }

        /// <summary>Creates and returns a new point with specified coordinates.
        /// <para>For this method to work, the delegate <see cref="CreateOutputValues"/> must be defined, which
        /// creates the output values correspoinding to specific input parameters.</para></summary>
        /// <param name="coordinates">Coordinates of the created point. A copy of this vector should always be created
        /// to hold coordinates within the point, because the caller is allowed to modify coordinates on the vector.</param>
        public override SampledDataElement CreatePointFromCoordinates(IVector coordinates)
        {
            if (CreateOutputValues == null)
                throw new InvalidOperationException("Can not create a training element from input vector coordinates. "
                    + Environment.NewLine + "  Delegate for creating output values from input parameters is not specified.");
            return new SampledDataElement(coordinates, CreateOutputValues(coordinates));
        }

        /// <summary>This delegate is used to create output values for a newly created training elements from the specified 
        /// input parameters.
        /// <para>This delegate is used in the <see cref="CreateOutputValues"/> property of its containing class.</para></summary>
        /// <param name="inputParameters">Input parameters of the neural training element.</param>
        /// <returns>The generated output values corresponding to the specified input parameters.</returns>
        public delegate IVector CreateOutputValuesDelegate(IVector inputParameters);

        CreateOutputValuesDelegate _createOutputValues;

        /// <summary>Creates and returns output values (usually for a newly created training element) correspoonding to
        /// the specified input parameters.</summary>
        CreateOutputValuesDelegate CreateOutputValues
        {
            get { return _createOutputValues; }
            set { _createOutputValues = value; }
        }



        /// <summary>Creates and returns a new point container that wraps the specified point.</summary>
        /// <param name="point">Point to be wrapped.</param>
        public override PointContainerSampledDataElement CreatePointContainer(SampledDataElement point)
        {
            return new PointContainerSampledDataElement(point);
        }


        /// <summary>Creates and returnws a distance comparer of hte default type for the current 
        /// type of point of clouds.</summary>
        public override DistanceComparer<PointContainerSampledDataElement, SampledDataElement> CreateDefaultDistanceComparer
            (PointContainerSampledDataElement referencePoint)
        {
            return new DistanceComparerSampledDataElement(referencePoint);
        }

    }  // class PointCloudTrainingElement


    /// <summary>Container class that contains a single vector point plus all the data that are necessary for searching
    /// and re-connecting operations on points.</summary>
    /// <typeparam name="PointType">Type of the vector enclosed in this container class to represent a point in space.</typeparam>
    /// $A Igor Sep08 May09 Dec11;
    public class PointContainerSampledDataElement : PointContainer<PointContainerSampledDataElement, SampledDataElement>,
        IPointContainer<SampledDataElement>, ILockable
    {

        public PointContainerSampledDataElement(SampledDataElement point, int index)
            : base(point, index)
        { }

        public PointContainerSampledDataElement(SampledDataElement point)
            : this(point, 0)
        {
            this.Point = point;
        }



        /// <summary>Returns vector of co-ordinates (or input parameters) of the specified point.</summary>
        /// <param name="point">Point whose co-ordinates are returned.</param>
        public override IVector GetPointCoordinates(SampledDataElement point)
        {
            return point.InputParameters;
        }


        /// <summary>Gets the vector of output values of the point containet in the current container. 
        /// <pra>This method must be overridden in derived classes that use this functionality (not all point 
        /// containers use it).</pra></summary>
        /// <remarks>This functionality is used only in those types of points that have also output 
        /// values defined, which is true for <see cref="SampledDataElement"/>.</remarks>
        public override IVector GetPointOutputVector(SampledDataElement point)
        {
            return point.OutputValues;
        }

    }  // class PointContainerVector

    /// <summary>Distance comparer for point clouds where point type is <see cref="SampledDataElement"/>.</summary>
    public class DistanceComparerSampledDataElement : DistanceComparer<PointContainerSampledDataElement, SampledDataElement>,
        IDistanceComparer<PointContainerSampledDataElement, SampledDataElement>,
        IComparer<PointContainerSampledDataElement>, ILockable
    {


        /// <summary>Constructs a new training element comparer according to the distance to the reference point.
        /// <para>Default methods for distance calculation is used.</para></summary>
        /// <param name="referencePoint">Reference points.</param>
        public DistanceComparerSampledDataElement(PointContainerSampledDataElement referencePoint) :
            this(referencePoint, null /* distanceFunction */, null /* lengthScales */)
        { }

        /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="SampledDataElement"/>).</summary>
        /// <param name="referencePoint">Reference point. Training elements are compared by their distance to this point.</param>
        /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
        /// <param name="immutable">If true then a copy of the reference point is stored internally rather than just 
        /// its reference, so it can not be changed.</param>
        public DistanceComparerSampledDataElement(PointContainerSampledDataElement referencePoint, DistanceDelegate<SampledDataElement> distanceFunction) :
            this(referencePoint, distanceFunction, null /* lengthScales */)
        { }

        /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="SampledDataElement"/>).</summary>
        /// <param name="referencePoint">Reference point. Training elements are compared by their distance to this point.</param>
        /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
        /// <param name="lengthScales">Vector of legth scales that defines how different co-ordinates are scaled by
        /// when calculating distances (this applies to the default length calculation function, ).</param>
        /// <param name="immutable">If true then a copy of the reference point is stored internally rather than just 
        /// its reference, so it can not be changed.</param>
        public DistanceComparerSampledDataElement(PointContainerSampledDataElement referencePoint, DistanceDelegate<SampledDataElement> distanceFunction,
                IVector lengthScales)
            : base(referencePoint, distanceFunction, lengthScales)
        { }


        /// <summary>Gets vector of coordinates of the specified point and stores it in the specified variable.
        /// <para>Where the type of point object itself contains vector of coordinates, only reference is
        /// stored. The obtained vector should therefore not be modified in any way.</para></summary>
        /// <param name="pt">Point for which vector of coordinates is obtained.</param>
        /// <param name="coord">Vector variable where extracted vector of coordinates is stored.</param>
        public override void GetPointCoordinates(SampledDataElement pt, ref IVector coord)
        {
            coord = pt.InputParameters;
        }

    }  // class DistanceComparerTrainingElement


}
