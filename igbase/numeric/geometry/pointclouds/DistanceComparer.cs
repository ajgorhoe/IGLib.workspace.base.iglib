// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using IG.Lib;
using IG.Num;


namespace IG.Num
{

    public delegate double DistanceDelegate<in PointType>(PointType pt1, PointType pt2);


    /// <summary>Interface used for distance comparers in point clouds.
    /// <remarks>Distance comparers are able to calculate distances between contained points or
    /// between point containers, and to compare point containers according to their distance with 
    /// a reference point.</remarks></summary>
    /// <typeparam name="PointType">Type of the point objects that have a point position in space,
    /// and whose containers of type PointContainer{PointType} are compared by the current class.</typeparam>
    /// <typeparam name="PointContainerType">Type of point container that is used to wrap points of the point type.</typeparam>
    public interface IDistanceComparer<PointContainerType, PointType> :
            IComparer<PointContainerType>
        where PointContainerType : IPointContainer<PointType>
    {
        /// <summary>Reference point.
        /// <para>Points are compared with respect to the distance to this point.</para></summary>
        PointContainerType ReferencePoint
        {
            get;
            set;
        }
        
        /// <summary>Gets vector of coordinates of the specified point and stores it in the specified variable.
        /// <para>Where the type of point object itself contains vector of coordinates, only reference is
        /// stored. The obtained vector should therefore not be modified in any way.</para></summary>
        /// <param name="pt">Point for which vector of coordinates is obtained.</param>
        /// <param name="coord">Vector variable where extracted vector of coordinates is stored.</param>
        void GetPointCoordinates(PointType pt, ref IVector coord);

        /// <summary>Sets the vector of length scales that are used for calculation of distances.
        /// <para>Property has protected setter.</para></summary>
        IVector LengthScales
        {
            set;
        }

        /// <summary>Sets the vector of minimal values for point coordinates. Needed for calculation of relative coordinate vector.</summary>
        IVector Min
        {
            set;
        }

        /// <summary>Sets the vector of maximal values point coordinates. Needed for calculation of relative coordinate vector.</summary>
        IVector Max
        {
            set;
        }

        /// <summary>Sets parameters that affect default calculation of distance and relative point coordinates.</summary>
        /// <param name="min">Vector of minimal coordinates.</param>
        /// <param name="max">Vector of maximal coordinates.</param>
        /// <param name="lengthScales">Vector of length scales.</param>
        void SetCoordinateScales(IVector min, IVector max, IVector lengthScales);


        /// <summary>Converts the specified coordinates to relative coorsinates where compponents run from 0 to 1
        /// if original coordinates run from minimal to maximal value.
        /// <para>Minimal coordinate values are stored in <see cref="Min"/> and maximal values are stored in <see cref="Max"/>.</para></summary>
        /// <param name="coord">Vector of original coordinates.</param>
        /// <param name="relativeCoortinates">Vector where converted relative coordinates are stored.</param>
        /// <remarks>Calculation of relative coordinates does not rely on the vector of length scales, but only on vectors
        /// of minimal and maximal coordinates.</remarks>
        void GetRelativeCoordinates(IVector coord, ref IVector relativeCoortinates);
        

        #region OutputVectors


        /// <summary>Sets the vector of OUTPUT vector's length scales that are used for calculation of distances and relative coordinates.
        /// <para>Property has protected setter.</para></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        IVector OutputLengthScales
        { set; }

        
        /// <summary>Sets the vector of minimal values for point OUTPUT values. Needed for calculation of relative coordinate vector.
        /// <para>Property has protected setter.</para></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        IVector OutputMin
        { set; }

        
        /// <summary>Sets the vector of maximal values point OUTPUT values. Needed for calculation of relative coordinate vector.
        /// <para>Property has protected setter.</para></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        IVector OutputMax
        { set; }
        
        /// <summary>Sets parameters that affect default calculation of output distance and output relative coordinates.</summary>
        /// <param name="min">Vector of minimal output coordinates.</param>
        /// <param name="max">Vector of maximal output coordinates.</param>
        /// <param name="lengthScales">Vector of output length scales.</param>
        void SetOutputScales(IVector min, IVector max, IVector lengthScales);
        
        /// <summary>Converts the specified OUTPUT coordinates to relative coordinates where compponents run from 0 to 1
        /// if original coordinates run from minimal to maximal value.
        /// <para>Minimal OUTPUT coordinate values are stored in <see cref="OutputMin"/> and maximal values are stored in <see cref="OutputMax"/>.</para></summary>
        /// <param name="coord">Vector of original coordinates.</param>
        /// <param name="relativeCoortinates">Vector where converted relative OUTPUT coordinates are stored.</param>
        /// <remarks>Calculation of relative coordinates does not rely on the vector of length scales, but only on vectors
        /// of minimal and maximal coordinates.
        /// <para>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</para></remarks>
        void GetOutputRelativeCoordinates(IVector coord, ref IVector relativeCoortinates);
                
        /// <summary>Returns output distance between two point containers.
        /// <para>If vector of output scales (property <see cref="OutputLengthScales"/>) is specified
        /// then weighted Euclidean distance between output vectors of the specified containers is returned 
        /// (differences in components are divided by components of vector output scaling lengths before being squared).</para>
        /// <para>If vector of output length scales is not specified then usual Euclidean distance between coordinates
        /// of the two specified points is returned.</para></summary>
        /// <param name="pt1">The first poiint container for distance calculation.</param>
        /// <param name="pt2">The second point container for distance calculation.</param>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.
        /// <para>While definition of input distance (between point co-ordinates) can be defined by a delegate,
        /// definition of output distance is currenyly fixed, can not be modified (except by setting the vector of
        /// output length scales that affects the definition).</para></remarks>
        double OutputDistance(PointContainerType pt1, PointContainerType pt2);


        ///// <summary>Returns output distance between two output vectors.
        ///// <para>If vector of output scales (property <see cref="OutputLengthScales"/>) is specified
        ///// then weighted Euclidean distance between coordinates of the specified point is returned 
        ///// (differences in coordinates are divided by components of vector of scales before being squared).</para>
        ///// <para>If vector of output length scales is not specified then usual Euclidean distance between coordinates
        ///// of the two specified points is returned.</para></summary>
        ///// <param name="outVec1">The first output vector for output distance calculation.</param>
        ///// <param name="outVec2">The second output vector for output distance calculation.</param>
        ///// <remarks>This functionality is used only in those types of points that have input parameters and output 
        ///// values, such as approximation data points.
        ///// <para>While definition of input distance (between point co-ordinates) can be defined by a delegate,
        ///// definition of output distance is currenyly fixed, can not be modified (except by setting the vector of
        ///// output length scales that affects the definition).</para></remarks>
        //public virtual double OutputDistance(IVector outVec1, IVector outVec2);


        #endregion OutputVectors


        /// <summary>Returns distance between the specified two points.</summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="pt2">The second point.</param>
        /// <remarks>Distance is defined by the DistanceFunction property, which is a delegate of type <see cref="DistanceDelegate{PointType}"/>.</remarks>
        double Distance(PointType pt1, PointType pt2);
        
        /// <summary>Returns distance between the specified two points enclosed in PointContainer{PointType} objects,
        /// as defined by the distance calculation delegate (property DistanceFunction) of the current object.</summary>
        /// <param name="boxedPt1">The first point.</param>
        /// <param name="boxedPt2">The second point.</param>
        double Distance(PointContainerType boxedPt1, PointContainerType boxedPt2);
        
        /// <summary>Returns distance between the specified point encolosed in an PointContainer object and
        /// a non-enclosed point, as defined by the distance calculation delegate (property DistanceFunction) 
        /// of the current object.</summary>
        /// <param name="boxedPt1">The first training element.</param>
        /// <param name="pt2">The second training element.</param>
        double Distance(PointContainerType boxedPt1, PointType pt2);

        /// <summary>Returns distance between the specified point and the second point encolosed in an PointContainer{PointType} object,
        /// as defined by the distance calculation delegate (property DistanceFunction) of the current object.</summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="boxedPt2">The second point enclosed in PointContainer{PointType} object.</param>
        double Distance(PointType pt1, PointContainerType boxedPt2);

        /// <summary>Returns distance between the specified point and the  reference point (property ReferencePoint) 
        /// of the current object (enclosed in PointContainer{PointType} object), as defined by the distance calculation
        /// delegate  of the current comparer object (property DistanceFunction).</summary>
        /// <param name="pt">Vector whose distance to the reference point is returned.</param>
        double Distance(PointType pt);

        /// <summary>Returns distance between the specified point (enclosed in PointContainer{PointType} pbject) and the  
        /// reference point (property <see cref="ReferencePoint"/>)  of the current object (property <see cref="ReferencePoint"/>), as 
        /// defined by the distance calculation delegate  of the current comparer object (property DistanceFunction).</summary>
        /// <param name="boxedPt">Pont enclosed in an PointContainer{PointType} whose distance to the reference point is returned.</param>
        double Distance(PointContainerType boxedPt);

        /// <summary>Sorts a list of point containers with the current comparer.</summary>
        /// <param name="list">List of point containers to be sorted.</param>
        void Sort(List<PointContainerType> list);
        
        /// <summary>Gets comparer that compares two point containers of type PointContainerType{}
        /// by the stored distance stored on those points.</summary>
        IComparer<PointContainerType> StoredDistanceComparer
        {
            get;
        }
                
        /// <summary>Sorts a list of point containers with the current comparer.</summary>
        /// <param name="list">List of point containers to be sorted.</param>
        void SortUsingStoredDistance(List<PointContainerType> list);
        
    }

    /// <summary>Class that is used to calculate and compare distances between point containers.</summary>
    /// <typeparam name="PointType">Type of the point objects that have a point position in space,
    /// and whose containers of type PointContainer{PointType} are compared by the current class.</typeparam>
    /// <typeparam name="PointContainerType">Type of point container that is used to wrap points of the point type.</typeparam>
    /// <remarks>This class as a part of group of classes that have been created in 2008 in order to support 
    /// different operations and algorithms on losely connected clouds of points that are embedded in space 
    /// of arbitrary dimension. The primary intended application was in optimization algorithms based on successive
    /// approximations of response and on restricted step prototype algorithms, developed by the author.
    /// The scope is much broader, however, because these classes can support closest neighbors algorithms,
    /// various graph algorithms based on points in space, detection of clusters, etc.
    /// <para>In 2009, major refactoring has been performed where a more generic structure has been introduced, 
    /// allowing implementation of generic algorithm methods that can be used with different representations 
    /// of points.</para>
    /// <para>In 2011, the classes weer migrated from sandbox environment to the prduction IGLib library
    /// and simplified a bit. The intention is to use the classes in the artificial neural network-based approximation
    /// modules developed for COBIK and the University of Nova Gorica. The intention is, however, that the eventual
    /// algorithms developed within this scope are implemented in a generic way, such that they can serve
    /// multiple purposes.</para></remarks>
    /// $A Igor Sep08 May09 Dec11;
    public abstract class DistanceComparer<PointContainerType, PointType>: 
        IDistanceComparer<PointContainerType, PointType>,
        IComparer<PointContainerType>, ILockable
        // where PointLinkType : PointLink<PointLinkType, PointContainerType, PointType>
        where PointContainerType: IPointContainer<PointType>
        //where PointType: class
    {

            private DistanceComparer() { }  // disable argumentless constructor

            /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="IVector"/>).</summary>
            /// <param name="referencePoint">Reference point. Training elements are compared by their distance to this point.</param>
            /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
            public DistanceComparer(PointContainerType referencePoint, DistanceDelegate<PointType> distanceFunction):
                this(referencePoint, distanceFunction, null /* lengthScales */)
            {  }

        /// <summary>Constructs a new comparer according to input distance to a reference poiont (type <see cref="IVector"/>).</summary>
        /// <param name="referencePoint">Reference point. Training elements are compared by their distance to this point.</param>
        /// <param name="distanceFunction">Delegate used for calculation of distance between two points.</param>
        /// <param name="lengthScales">Length scales, used for scaling lengths along coo-rdinate axes.</param>
        public DistanceComparer(PointContainerType referencePoint, DistanceDelegate<PointType> distanceFunction,
                IVector lengthScales)
            {
                if (referencePoint == null)
                    throw new ArgumentException("The reference point for comparison by distance to a reference point is not specified (null argument).");
                if (distanceFunction == null)
                    this.DistanceFunction = DefaultDistanceFunction;
                else
                    this.DistanceFunction = distanceFunction;
                this.ReferencePoint = referencePoint;
                this.LengthScales = lengthScales;
            }


        #region ILockable

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ILockable


        protected PointContainerType _referencePoint;


        /// <summary>Reference point.
        /// <para>Points are compared with respect to the distance to this point.</para></summary>
        public PointContainerType ReferencePoint
        {
            get { return _referencePoint; }
            set { _referencePoint = value; }
        }

        protected DistanceDelegate<PointType> _distanceFunction;

        /// <summary>Delegate that calculates distance between two vectors.
        /// <para>If set to nul then default distance function (<see cref="DefaultDistanceFunction"/>) is used for 
        /// distance calculation.</para></summary>
        protected DistanceDelegate<PointType> DistanceFunction
        {
            get { return _distanceFunction; }
            set {
                if (value == null)
                    _distanceFunction = DefaultDistanceFunction;
                else
                    _distanceFunction = value;
            }
        }

        // protected System.Comparison<PointContainerType> ComparisonFunction;

        /// <summary>Gets vector of coordinates of the specified point and stores it in the specified variable.
        /// <para>Where the type of point object itself contains vector of coordinates, only reference is
        /// stored. The obtained vector should therefore not be modified in any way.</para></summary>
        /// <param name="pt">Point for which vector of coordinates is obtained.</param>
        /// <param name="coord">Vector variable where extracted vector of coordinates is stored.</param>
        public abstract void GetPointCoordinates(PointType pt, ref IVector coord);
        

        protected IVector _lengthScales = null;

        /// <summary>Sets the vector of length scales that are used for calculation of distances.
        /// <para>Property has protected setter.</para></summary>
        public IVector LengthScales
        {
            protected get { return _lengthScales; }
            set { _lengthScales = value; }
        }

        protected IVector _min = null;


        /// <summary>Sets the vector of minimal values for point coordinates. Needed for calculation of relative coordinate vector.
        /// <para>Property has protected setter.</para></summary>
        public IVector Min
        {
            protected get { return _min; }
            set { _min = value; }
        }

        protected IVector _max = null;

        /// <summary>Sets the vector of maximal values point coordinates. Needed for calculation of relative coordinate vector.
        /// <para>Property has protected setter.</para></summary>
        public IVector Max
        {
            protected get { return _max; }
            set { _max = value; }
        }

        /// <summary>Sets parameters that affect default calculation of distance and relative point coordinates.</summary>
        /// <param name="min">Vector of minimal coordinates.</param>
        /// <param name="max">Vector of maximal coordinates.</param>
        /// <param name="lengthScales">Vector of length scales.</param>
        public void SetCoordinateScales(IVector min, IVector max, IVector lengthScales)
        {
            Min = min;
            Max = max;
            LengthScales = lengthScales;
        }

        /// <summary>Converts the specified coordinates to relative coordinates where compponents run from 0 to 1
        /// if original coordinates run from minimal to maximal value.
        /// <para>Minimal coordinate values are stored in <see cref="Min"/> and maximal values are stored in <see cref="Max"/>.</para></summary>
        /// <param name="coord">Vector of original coordinates.</param>
        /// <param name="relativeCoortinates">Vector where converted relative coordinates are stored.</param>
        /// <remarks>Calculation of relative coordinates does not rely on the vector of length scales, but only on vectors
        /// of minimal and maximal coordinates.</remarks>
        public void GetRelativeCoordinates(IVector coord, ref IVector relativeCoortinates)
        {
            if (coord == null)
                throw new ArgumentException("The vector of coordinates is not specified.");
            if (_min == null || _max == null)
                throw new InvalidDataException("Vectors of minimal and maximal coordinates are not specified.");
            int dim = _min.Length;
            if (_max.Length != dim)
                throw new InvalidDataException("Dimensions of minimal and maximal co-ordinate vectors do not match.");
            if (coord.Length != dim)
                throw new InvalidDataException("Dimension of vector of co-ordinates to be converted does not match dimensions of minimal and maximal coordinates ("
                    + dim + ").");
            bool allocate = false;
            if (relativeCoortinates == null)
                allocate = true;
            else if (relativeCoortinates.Length != dim)
                allocate = true;
            if (allocate)
                relativeCoortinates = new Vector(dim);
            for (int i = 0; i < dim; ++i)
            {
                double min = _min[i];
                double max = _max[i];
                double scalingLength = max - min;
                if (scalingLength == 0)
                    scalingLength = 1;
                relativeCoortinates[i] = (coord[i] - min) / scalingLength;
            }
        }


        #region OutputVectors


        protected IVector _outputLengthScales = null;

        /// <summary>Sets the vector of OUTPUT vector's length scales that are used for calculation of distances and relative coordinates.
        /// <para>Property has protected setter.</para></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        public IVector OutputLengthScales
        {
            protected get { return _outputLengthScales; }
            set { _outputLengthScales = value; }
        }

        protected IVector _outputMin = null;

        /// <summary>Sets the vector of minimal values for point OUTPUT values. Needed for calculation of relative coordinate vector.
        /// <para>Property has protected setter.</para></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        public IVector OutputMin
        {
            protected get { return _outputMin; }
            set { _outputMin = value; }
        }

        protected IVector _outputMax = null;

        /// <summary>Sets the vector of maximal values point OUTPUT values. Needed for calculation of relative coordinate vector.
        /// <para>Property has protected setter.</para></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        public IVector OutputMax
        {
            protected get { return _outputMax; }
            set { _outputMax = value; }
        }

        /// <summary>Sets parameters that affect default calculation of output distance and output relative coordinates.</summary>
        /// <param name="min">Vector of minimal output coordinates.</param>
        /// <param name="max">Vector of maximal output coordinates.</param>
        /// <param name="lengthScales">Vector of output length scales.</param>
        public void SetOutputScales(IVector min, IVector max, IVector lengthScales)
        {
            OutputMin = min;
            OutputMax = max;
            OutputLengthScales = lengthScales;
        }


        /// <summary>Converts the specified OUTPUT coordinates to relative coordinates where compponents run from 0 to 1
        /// if original coordinates run from minimal to maximal value.
        /// <para>Minimal OUTPUT coordinate values are stored in <see cref="OutputMin"/> and maximal values are stored in <see cref="OutputMax"/>.</para></summary>
        /// <param name="coord">Vector of original coordinates.</param>
        /// <param name="relativeCoortinates">Vector where converted relative OUTPUT coordinates are stored.</param>
        /// <remarks>Calculation of relative coordinates does not rely on the vector of length scales, but only on vectors
        /// of minimal and maximal coordinates.
        /// <para>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</para></remarks>
        public void GetOutputRelativeCoordinates(IVector coord, ref IVector relativeCoortinates)
        {
            if (coord == null)
                throw new ArgumentException("The vector of output coordinates is not specified.");
            if (_outputMin == null || _outputMax == null)
                throw new InvalidDataException("Vectors of minimal and maximal output coordinates are not specified.");
            int dim = _outputMin.Length;
            if (_outputMax.Length != dim)
                throw new InvalidDataException("Dimensions of minimal and maximal output co-ordinate vectors do not match.");
            if (coord.Length != dim)
                throw new InvalidDataException("Dimension of vector of output co-ordinates to be converted does not match dimensions of minimal and maximal output coordinates ("
                    + dim + ").");
            bool allocate = false;
            if (relativeCoortinates == null)
                allocate = true;
            else if (relativeCoortinates.Length != dim)
                allocate = true;
            if (allocate)
                relativeCoortinates = new Vector(dim);
            for (int i = 0; i < dim; ++i)
            {
                double min = _outputMin[i];
                double max = _outputMax[i];
                double scalingLength = max - min;
                if (scalingLength == 0)
                    scalingLength = 1;
                relativeCoortinates[i] = (coord[i] - min) / scalingLength;
            }
        }

        
        /// <summary>Returns output distance between two point containers.
        /// <para>If vector of output scales (property <see cref="OutputLengthScales"/>) is specified
        /// then weighted Euclidean distance between output vectors of the specified containers is returned 
        /// (differences in components are divided by components of vector output scaling lengths before being squared).</para>
        /// <para>If vector of output length scales is not specified then usual Euclidean distance between coordinates
        /// of the two specified points is returned.</para></summary>
        /// <param name="pt1">The first poiint container for distance calculation.</param>
        /// <param name="pt2">The second point container for distance calculation.</param>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.
        /// <para>While definition of input distance (between point co-ordinates) can be defined by a delegate,
        /// definition of output distance is currenyly fixed, can not be modified (except by setting the vector of
        /// output length scales that affects the definition).</para></remarks>
        public virtual double OutputDistance(PointContainerType pt1, PointContainerType pt2)
        {
            return OutputDistance(pt1.GetPointOutputVector(), pt2.GetPointOutputVector());
        }


        /// <summary>Returns output distance between two output vectors.
        /// <para>If vector of output scales (property <see cref="OutputLengthScales"/>) is specified
        /// then weighted Euclidean distance between coordinates of the specified point is returned 
        /// (differences in coordinates are divided by components of vector of scales before being squared).</para>
        /// <para>If vector of output length scales is not specified then usual Euclidean distance between coordinates
        /// of the two specified points is returned.</para></summary>
        /// <param name="outVec1">The first output vector for output distance calculation.</param>
        /// <param name="outVec2">The second output vector for output distance calculation.</param>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.
        /// <para>While definition of input distance (between point co-ordinates) can be defined by a delegate,
        /// definition of output distance is currenyly fixed, can not be modified (except by setting the vector of
        /// output length scales that affects the definition).</para></remarks>
        protected virtual double OutputDistance(IVector outVec1, IVector outVec2)
        {
            if (outVec1 == null || outVec2 == null)
            {
                throw new ArgumentException("Undefined output vector(s) for output distance calculation.");
            }
            else if (outVec1.Length != outVec2.Length)
                throw new ArgumentException("Vectors of outputs for output distance calculation have unequal length ("
                    + outVec1.Length + " vs. " + outVec2.Length);
            if (_outputLengthScales == null)
            {
                return Vector.DistancePlain(outVec1, outVec2);
            }
            else
            {
                if (_outputLengthScales.Length != outVec1.Length)
                    throw new InvalidDataException("Vector of length scales is not of correct dimension, "
                        + _outputLengthScales.Length + " instead of " + outVec1.Length);
                return Vector.DistanceWeightedPlain(outVec1, outVec2, _outputLengthScales);
            }
        }


        #endregion OutputVectors




        /// <summary>Default distance funciton for the current class, used when distance calculation delegate is not specified.
        /// <para>If vector of length scales (property <see cref="LengthScales"/>) is specified
        /// then weighted Euclidean distance between coordinates of the specified point is returned 
        /// (differences in coordinates are divided by components of this vector before being squared).</para>
        /// <para>Iv vector of length scales is not specified then Euclidean distance between coordinates
        /// of the two specified points is returned.</para></summary>
        /// <param name="pt1">The first poiint for distance calculation.</param>
        /// <param name="pt2">The second point for distance calculation.</param>
        protected virtual double DefaultDistanceFunction(PointType pt1, PointType pt2)
        {
            IVector coord1 = null, coord2 = null;
            GetPointCoordinates(pt1, ref coord1);
            GetPointCoordinates(pt2, ref coord2);
            if (coord1 == null || coord2 == null)
            {
                throw new ArgumentException("Undefined vector(s) of point coordinates in distance calculation.");
            }
            else if (coord1.Length != coord2.Length)
                throw new ArgumentException("Vectors of point coordinates for distance calculation have unequal length ("
                    + coord1.Length + " vs. " + coord2.Length);
            if (_lengthScales == null)
            {
                return Vector.DistancePlain(coord1, coord2);
            }
            else
            {
                if (_lengthScales.Length != coord1.Length)
                    throw new InvalidDataException("Vector of length scales is not of correct dimension, " 
                        + _lengthScales.Length + " instead of " + coord1.Length);
                return Vector.DistanceWeightedPlain(coord1, coord2, _lengthScales);
            }
        }

        /* REMARKS: IComparer<PointType> is not implemented because both interfaces - 
         * IComparer<PointType> and IComparer<PointContainerType> - can not be explicitly
         * referenced at the same time (because PointContainerType coud in princible be
         * the same type as PointType, since recursive type parameters are allowed)
         */

        /// <summary>Compares two points according to the current comparer definition.</summary>
        /// <param name="pt1">the first point to be compared.</param>
        /// <param name="pt2">The second point to be compared.</param>
        /// <returns>A negative value if the first point is smaller, positive value if it is greater, and 0 if it is 
        /// equal to the second point according to comparison criterion implemented by the current comparer object.</returns>
        public virtual int Compare(PointContainerType pt1, PointContainerType pt2)
        {
            if (DistanceFunction==null)
                throw new ArgumentException("Can not compare two points, distance function is not defined.");
            if (ReferencePoint==null)
            throw new ArgumentException("Can not compare two points, reference point is not defined.");
            double d1 = DistanceFunction(pt1.Point, ReferencePoint.Point);
            double d2 = DistanceFunction(pt2.Point, ReferencePoint.Point);
            return d1<d2?-1:(d1>d2?1:0);
            // return Comparer<double>.Default.Compare(d1, d2);
        }

        /// <summary>Returns distance between the specified two points.</summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="pt2">The second point.</param>
        /// <remarks>Distance is defined by the <see cref="DistanceFunction"/> property, which is a delegate of type <see cref="DistanceDelegate{PointType}"/>.</remarks>
        public virtual double Distance(PointType pt1, PointType pt2)
        {
            if (DistanceFunction == null)
                throw new InvalidOperationException("Distance bunction is not specified, can not calculate the distance between two points.");
            else
                return DistanceFunction(pt1, pt2);
        }


        /// <summary>Returns distance between the specified two points enclosed in PointContainer{} objects,
        /// as defined by the distance calculation delegate (property <see cref="DistanceFunction"/>) of the current object.</summary>
        /// <param name="boxedPt1">The first point.</param>
        /// <param name="boxedPt2">The second point.</param>
        public double Distance(PointContainerType boxedPt1, PointContainerType boxedPt2)
        {
            if (boxedPt1 == null || boxedPt2 == null)
                throw new ArgumentException("One of the points whose input distance is calculated is not specified (null reference).");
            return Distance(boxedPt1.Point, boxedPt2.Point);
        }

        /// <summary>Returns distance between the specified point encolosed in an PointContainer{...} object and
        /// a non-enclosed point, as defined by the distance calculation delegate (property <see cref="DistanceFunction"/>) 
        /// of the current object.</summary>
        /// <param name="boxedPt1">The first training element.</param>
        /// <param name="pt2">The second training element.</param>
        public double Distance(PointContainerType boxedPt1, PointType pt2)
        {
            if (boxedPt1 == null)
                throw new ArgumentException("The enclosed point whose distance to the specified pont is calculated is not specified (null reference).");
            else if (pt2 == null)
                throw new ArgumentException("The second point whose distance to the specified pont is calculated is not specified (null reference).");
            return Distance(boxedPt1.Point, pt2);
        }

        /// <summary>Returns distance between the specified point and the second point encolosed in an PointContainer{...} object,
        /// as defined by the distance calculation delegate (property <see cref="DistanceFunction"/>) of the current object.</summary>
        /// <param name="pt1">The first point.</param>
        /// <param name="boxedPt2">The second point enclosed in PointContainer{PointType} object.</param>
        public double Distance(PointType pt1, PointContainerType boxedPt2)
        {
            if (pt1 == null)
                throw new ArgumentException("The first point whose distance to the specified pont is calculated is not specified (null reference).");
            else if (boxedPt2 == null)
                throw new ArgumentException("The enclosed second point whose distance to the specified pont is calculated is not specified (null reference).");
            return Distance(pt1, boxedPt2.Point);
        }

        /// <summary>Returns distance between the specified point and the  reference point (property <see cref="ReferencePoint"/>) 
        /// of the current object (enclosed in PointContainer{PointType} object), as defined by the distance calculation
        /// delegate  of the current comparer object (property <see cref="DistanceFunction"/>).</summary>
        /// <param name="pt">Vector whose distance to the reference point is returned.</param>
        public double Distance(PointType pt)
        {
            return Distance(pt, ReferencePoint.Point);
        }


        /// <summary>Returns distance between the specified point (enclosed in PointContainer{PointType} object) and the  
        /// reference point (property <see cref="ReferencePoint"/>)  of the current object (property <see cref="ReferencePoint"/>), as 
        /// defined by the distance calculation delegate  of the current comparer object (property <see cref="DistanceFunction"/>).</summary>
        /// <param name="boxedPt">Point enclosed in an PointContainer{T} object whose distance to the reference point is returned.</param>
        public double Distance(PointContainerType boxedPt)
        {
            return Distance(boxedPt.Point, ReferencePoint.Point);
        }

        /// <summary>Sorts a list of point containers with the current comparer.</summary>
        /// <param name="list">List of point containers to be sorted.</param>
        public void Sort(List<PointContainerType> list)
        {
            if (list!=null)
                list.Sort(this);
        }

        /// <summary>Compares two point containers according to their  StoredDistance property.</summary>
        protected class StoredDistanceComparerClass : IComparer<PointContainerType>
        {
            public StoredDistanceComparerClass() { }
            
            public int Compare(PointContainerType pt1, PointContainerType pt2)
            {
                double l1 = pt1.StoredDistance;
                double l2 = pt2.StoredDistance;
                return (l1 < l2) ? -1 : (l1 > l2 ? 1 : 0);
            }
        }

        private IComparer<PointContainerType> _storedDistanceComparer;

        /// <summary>Gets comparer that compares two point containers of type <typeparamref name="PointContainerType"/>
        /// by the stored distance stored on those points.</summary>
        public IComparer<PointContainerType> StoredDistanceComparer
        {
            get
            {
                if (_storedDistanceComparer == null)
                    _storedDistanceComparer = new StoredDistanceComparerClass();
                return _storedDistanceComparer;
            }
        }


        /// <summary>Sorts a list of point containers with the current comparer.</summary>
        /// <param name="list">List of point containers to be sorted.</param>
        public void SortUsingStoredDistance(List<PointContainerType> list)
        {
            if (list != null)
            {
                foreach (PointContainerType pt in list)
                    pt.StoredDistance = this.Distance(pt);  // calculate and store distance to reference point
                list.Sort(StoredDistanceComparer);  // sort according to the stored distance
            }
        }


    }  // class DistanceComparer<PointType>

}