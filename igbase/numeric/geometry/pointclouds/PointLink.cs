
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using IG.Lib;
using IG.Num;

namespace IG.Num
{



    /// <summary>Contains a pointer (link) to another point container.</summary>
    /// <typeparam name="PointContainerType">Type of point link that is used by point container.</typeparam>
    /// <typeparam name="PointType">Type of the point class object that is enclosed in the pointed-to container.</typeparam>
    /// $A  Sep08 May09 Dec11;
    public class PointLink<PointContainerType, PointType> : 
            PointLink<PointLink<PointContainerType, PointType>, PointContainerType, PointType>,
            ILockable
        where PointContainerType : PointContainer<PointLink<PointContainerType, PointType>, PointContainerType, PointType>
        //where PointType : class
    {

        public PointLink(PointContainerType point)
            : base(point)
        {  }

    }


    /// <summary>Contains a pointer (link) to another point container.</summary>
    /// <typeparam name="PointLinkType">Type of point container used by the class.</typeparam>
    /// <typeparam name="PointContainerType">Type of point link that is used by point container.</typeparam>
    /// <typeparam name="PointType">Type of the point class object that is enclosed in the pointed-to container.</typeparam>
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
    public class PointLink<PointLinkType, PointContainerType, PointType> : ILockable
        where PointLinkType: PointLink<PointLinkType, PointContainerType, PointType>
        where PointContainerType : PointContainer<PointLinkType, PointContainerType, PointType>
        //where PointType: class
    {

        public PointLink(PointContainerType point)
        {
            this.Point = point;
        }

        #region ILockable

        protected internal object _lock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _lock; } }

        #endregion ILockable

        protected internal PointContainerType _point;

        /// <summary>Gets or sets the point that is contained in the current container.</summary>
        public PointContainerType Point
        {
            get { return _point; }
            set {
                if (value == null)
                    throw new ArgumentException("Point to be linked is not specified (null argument).");
                _point = value; 
            }
        }

        protected double _distanceFromPoint;


        /// <summary>Distance to the point containing the current link in its neighbor list.
        /// <remarks>This kdistance must be calculated by algorithms that set, create or change the link.
        /// Otherwise, distance on all links of pont cloud can also be updated by the method </remarks></summary>
        public double DistanceFromPoint
        {
            get { return _distanceFromPoint; }
            set { _distanceFromPoint = value; }
        }


    }


}