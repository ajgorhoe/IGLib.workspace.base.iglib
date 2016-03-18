// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using IG.Lib;
using IG.Num;

namespace IG.Num
{

    /// <summary>Basic interface for point container classes.</summary>
    /// <typeparam name="PointType">Type of point objects that are embedded in point container.</typeparam>
    public interface IPointContainer<PointType>
    {

        /// <summary>Point that is enclosed by the current point container object.</summary>
        PointType Point
        {
            get;
        }

        /// <summary>Unique ID of the current point container (important also for testing and debugging).</summary>
        int Id
        {
            get;
        }

        
        /// <summary>Index of the current point in the original list of points where it can be accessed.</summary>
        int Index
        {
            get;
        }
        
        /// <summary>Stored distance to a reference point, which is used to increase performance of operations
        /// that perform comparison of point containers by the distance to some reference point.</summary>
        /// <remarks><para>Storing distance to a reference point can significantly speed up sorting operations
        /// where points are compared according to their distance to the specified reference point. Since CPU 
        /// time spent for calculation of the distance is proportional to the dimension of space where points
        /// are embedded, we can save time if we only calculate the distance to a reference point once and then
        /// in all comparisons use the already calculated distance. For example, when sorting N point, on average
        /// N*log_2(N) comparisons of point pairs are performed, each of which would need to calculate distances
        /// of both compared points from the specified reference point. If we calculate the distances in advance
        /// and use stored distances in sorting, distance calculation is performed only N times. If N=1000, this
        /// means about ten times less distance calculations and almost 10 times less time for sorting in higher
        /// space dimensions.</para></remarks>
        double StoredDistance
        {
            get;
            set;
        }

        
        /// <summary>returns vector coo-rdinates (or input parameters) of the point contained in the current container.</summary>
        IVector GetPointCoordinates();

        /// <summary>Returns vector of co-ordinates (or input parameters) of the specified point.</summary>
        /// <param name="point">Point whose co-ordinates are returned.</param>
        IVector GetPointCoordinates(PointType point);
        
        /// <summary>Gets the vector of output values of the point contained in the current container. 
        /// <pra>This method must be overridden in derived classes that use this functionality (not all point 
        /// containers use it).</pra></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        IVector GetPointOutputVector(PointType point);

        /// <summary>Gets the vector of output values of the point containet in the current container. 
        /// <pra>This method must be overridden in derived classes that use this functionality (not all point 
        /// containers use it).</pra></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        IVector GetPointOutputVector();


    }  // interface IPointContainer<PointType>



    ///// <summary>Container class that contains a single point plus all the data that are necessary for searching
    ///// and re-connecting operations on points.</summary>
    ///// <typeparam name="PointType">Type of the point enclosed in this container class.</typeparam>
    ///// <remarks>DEPRECATED!
    ///// <para>Instead of class MyPointContainer: PointContainer(PointType), use recursive generic definition 
    ///// class MyPointContainer: PointContainer(MyPointContainer, PointType)!!!</para></remarks>
    ///// $A Igor Sep08 May09 Dec11;
    //public abstract class PointContainer<PointType> : PointContainer<PointContainer<PointType>, PointType>,
    //        ILockable
    ////where PointType : class
    //{

    //    public PointContainer(PointType point, int index)
    //        : base(point, index)
    //    { }

    //    public PointContainer(PointType point)
    //        : this(point, 0)
    //    {
    //        this.Point = point;
    //    }

    //}  // abstract class PointContainer<PointType>


    /// <summary>Container class that contains a single point plus all the data that are necessary for searching
    /// and re-connecting operations on points.</summary>
    /// <typeparam name="PointContainerType">Type of point link that is used by point container.</typeparam>
    /// <typeparam name="PointType">Type of the point enclosed in this container class.</typeparam>
    /// $A Igor Sep08 May09 Dec11;
    public abstract class PointContainer<PointContainerType, PointType> :
            PointContainer<PointLink<PointContainerType, PointType>, PointContainerType, PointType>,
            IPointContainer<PointType>, ILockable
        where PointContainerType : PointContainer<PointLink<PointContainerType, PointType>, PointContainerType, PointType>
        //where PointType: class
    {


        public PointContainer(PointType point, int index): base(point, index)
        {  }

        public PointContainer(PointType point): this(point, 0)
        {
            this.Point = point;
        }

        #region Operation

        /// <summary>Creates and returns a new link object that points to the specified point (indirectly through point container).</summary>
        /// <param name="point">Pont at which the created link points.</param>
        public override PointLink<PointContainerType, PointType> CreateLink(PointContainerType point)
        {
            return new PointLink<PointContainerType, PointType>(point);
        }


        #endregion Operation


    }  // abstract class PointContainer<PointContainerType, PointType>


    
    /// <summary>Container class that contains a single point plus all the data that are necessary for searching
    /// and re-connecting operations on points.</summary>
    /// <typeparam name="PointLinkType">Type of point container used by the class.</typeparam>
    /// <typeparam name="PointContainerType">Type of point link that is used by point container.</typeparam>
    /// <typeparam name="PointType">Type of the point enclosed in this container class.</typeparam>
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
    public abstract class PointContainer<PointLinkType, PointContainerType, PointType>:
            IPointContainer<PointType>, ILockable
        where PointLinkType : PointLink<PointLinkType, PointContainerType, PointType>
        where PointContainerType: PointContainer<PointLinkType, PointContainerType, PointType>
        //where PointType: class
    {

        public PointContainer(PointType point, int index) 
        {
            this.Index = index;
            this.Point = point;
        }

        public PointContainer(PointType point) : this(point, 0)
        {  }


        #region ILockable

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ILockable

        #region Data

        private PointType _point;

        /// <summary>Point that is enclosed by the current point container object.</summary>
        public PointType Point
        {
            get { return _point; }
            protected set 
            {
                if (value == null)
                    throw new ArgumentException("The specified point is null.");
                _point = value; 
            }
        }

        private static int _nextPointId = 0;

        /// <summary>Returns the next point ID that can be used for new PointContainer objects.</summary>
        public static int NextPointId
        {
            get { ++_nextPointId; return _nextPointId - 1; }
        }

        private int _id = NextPointId;

        /// <summary>Unique ID of the current point container (important also for testing and debugging).</summary>
        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        private List<PointLinkType> _neighbors = new List<PointLinkType>();

        /// <summary>List of connected points ("neighbors").</summary>
        public List<PointLinkType> Neighbors
        {
            get { return _neighbors; }
        }

        protected int _index;

        /// <summary>Index of the current point in the original list of points where it can be accessed.</summary>
        public virtual int Index
        {
            get { return _index; }
            protected internal set { _index = value; }
        }

        protected double _storedDistance;

        /// <summary>Stored distance to a reference point, which is used to increase performance of operations
        /// that perform comparison of point containers by the distance to some reference point.</summary>
        /// <remarks><para>Storing distance to a reference point can significantly speed up sorting operations
        /// where points are compared according to their distance to the specified reference point. Since CPU 
        /// time spent for calculation of the distance is proportional to the dimension of space where points
        /// are embedded, we can save time if we only calculate the distance to a reference point once and then
        /// in all comparisons use the already calculated distance. For example, when sorting N point, on average
        /// N*log_2(N) comparisons of point pairs are performed, each of which would need to calculate distances
        /// of both compared points from the specified reference point. If we calculate the distances in advance
        /// and use stored distances in sorting, distance calculation is performed only N times. If N=1000, this
        /// means about ten times less distance calculations and almost 10 times less time for sorting in higher
        /// space dimensions.</para></remarks>
        public double StoredDistance
        {
            get { return _storedDistance; }
            set { _storedDistance = value; }
        }

        #endregion Data

        #region Operation


        /// <summary>returns vector coo-rdinates (or input parameters) of the point contained in the current container.</summary>
        public IVector GetPointCoordinates()
        {
            return GetPointCoordinates(Point);
        }

        /// <summary>Returns vector of co-ordinates (or input parameters) of the specified point.</summary>
        /// <param name="point">Point whose co-ordinates are returned.</param>
        public abstract IVector GetPointCoordinates(PointType point);


        /// <summary>Gets the vector of output values of the point contained in the current container. 
        /// <pra>This method must be overridden in derived classes that use this functionality (not all point 
        /// containers use it).</pra></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        public virtual IVector GetPointOutputVector(PointType point)
        {
            throw new InvalidOperationException("This type of point does not have output vector defined: " + typeof(PointType).FullName);
        }

        /// <summary>Gets the vector of output values of the point containet in the current container. 
        /// <pra>This method must be overridden in derived classes that use this functionality (not all point 
        /// containers use it).</pra></summary>
        /// <remarks>This functionality is used only in those types of points that have input parameters and output 
        /// values, such as approximation data points.</remarks>
        public virtual IVector GetPointOutputVector()
        {
            return GetPointOutputVector(Point); 
        }

        /// <summary>Creates and returns a new link object that points to the specified point (indirectly through point container).</summary>
        /// <param name="point">Pont at which the created link points.</param>
        public abstract PointLinkType CreateLink(PointContainerType point);

        #endregion Operation


    }  // anstract class PointContainer<PointLinkType, PointContainerType, PointType>





} 