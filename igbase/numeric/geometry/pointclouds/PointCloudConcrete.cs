// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// CONCRETE CLASSES

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using IG.Lib;
using IG.Num;

namespace IG.Num
{

    #region Vector


    /// <summary>Cloud of points where each poinr is represented by the <see cref="IVector"/> object, contains a list
    /// of containers of vector objects that include point coordinates.</summary>
    /// $A Igor Sep08 May09 Dec11;
    public class PointCloudVector : PointCloud<PointContainerVector, IVector>, ILockable
    {

        /// <summary>Constructs a cloud of ponts where points are of vector type.</summary>
        public PointCloudVector()
            : base()
        { }
        
        /// <summary>Constructs a cloud of vector points containing the specified points.</summary>
        /// <param name="points">Points that are included in the created point cloud.</param>
        public PointCloudVector(params IVector[] points)
            : this()
        { AddPoints(points); }

        /// <summary>Constructs a cloud of vector points containing the points embedded in the specified point containers.
        /// <para>All points are taken from their point containers and embedded in newly created point containers before
        /// adding to the created cloud of points.</para></summary>
        /// <param name="points">Points embedded in point containers that are included in the created point cloud.</param>
        public PointCloudVector(params IPointContainer<IVector>[] points)
            : this()
        { AddPoints(points); }

        /// <summary>Creates and returns a new point with specified coordinates.</summary>
        /// <param name="coordinates">Coordinates of the created point. A copy of this vector should always be created
        /// to hold coordinates within the point, because the caller is allowed to modify coordinates on the vector.</param>
        public override IVector CreatePointFromCoordinates(IVector coordinates)
        {
            return new Vector(coordinates);
        }

        /// <summary>Creates and returns a new point container that wraps the specified point.</summary>
        /// <param name="point">Point to be wrapped.</param>
        public override PointContainerVector CreatePointContainer(IVector point)
        {
            return new PointContainerVector(point);
        }


        /// <summary>Creates and returnws a distance comparer of hte default type for the current 
        /// type of point of clouds.</summary>
        public override DistanceComparer<PointContainerVector, IVector> CreateDefaultDistanceComparer(PointContainerVector point)
        {
            return new DistanceComparerVector(point);
        }


    }  // class PointCloudVector


    /// <summary>Container class that contains a single vector point plus all the data that are necessary for searching
    /// and re-connecting operations on points.</summary>
    /// $A Igor Sep08 May09 Dec11;
    public class PointContainerVector : PointContainer<PointContainerVector, IVector>,
        IPointContainer<IVector>, ILockable
    {

        public PointContainerVector(IVector point, int index)
            : base(point, index)
        { }

        public PointContainerVector(IVector point)
            : this(point, 0)
        {
            this.Point = point;
        }


        /// <summary>Returns vector of co-ordinates (or input parameters) of the specified point.</summary>
        /// <param name="point">Point whose co-ordinates are returned.</param>
        public override IVector GetPointCoordinates(IVector point)
        {
            return point;
        }


    }  // class PointContainerVector


    public class DistanceComparerVector : DistanceComparer<PointContainerVector, IVector>,
        IDistanceComparer<PointContainerVector, IVector>,
        IComparer<PointContainerVector>, ILockable
    {

        /// <summary>Constructs a new vector comparer according to the distance to the reference point.
        /// <para>Default methods for distance calculation is used.</para></summary>
        /// <param name="referencePoint">Reference points.</param>
        public DistanceComparerVector(PointContainerVector referencePoint) : 
            this(referencePoint, null /* distanceFunction */, null /* lengthScales */)
        {  }


        /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="IVector"/>).</summary>
        /// <param name="referencePoint">Reference point. Training elements are compared by their distance to this point.</param>
        /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
        public DistanceComparerVector(PointContainerVector referencePoint, DistanceDelegate<IVector> distanceFunction) :
            this(referencePoint, distanceFunction, null /* lengthScales */)
        { }

        /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="IVector"/>).</summary>
        /// <param name="referencePoint">Reference point. Training elements are compared by their distance to this point.</param>
        /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
        /// <param name="lengthScales">Vector of legth scales that defines how different co-ordinates are scaled by
        /// when calculating distances (this applies to the default length calculation function, ).</param>
        public DistanceComparerVector(PointContainerVector referencePoint, DistanceDelegate<IVector> distanceFunction,
                IVector lengthScales)
            : base(referencePoint, distanceFunction, lengthScales)
        { }


        /// <summary>Gets vector of coordinates of the specified point and stores it in the specified variable.
        /// <para>Where the type of point object itself contains vector of coordinates, only reference is
        /// stored. The obtained vector should therefore not be modified in any way.</para></summary>
        /// <param name="pt">Point for which vector of coordinates is obtained.</param>
        /// <param name="coord">Vector variable where extracted vector of coordinates is stored.</param>
        public override void GetPointCoordinates(IVector pt, ref IVector coord)
        {
            coord = pt;
        }

    }  // class DistanceComparerVector

    #endregion Vector

    

}
