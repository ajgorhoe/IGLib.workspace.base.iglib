// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using IG.Lib;
using IG.Num;


namespace IG.Num
{




    /// <summary>Cloud of points, contains a list of containers of objects that include point coordinates.</summary>
    /// <typeparam name="PointContainerType">Type of point link that is used by point container.</typeparam>
    /// <typeparam name="PointType">Type of objects that include point coordinates.</typeparam>
    /// $A Igor Sep08 May09 Dec11;
    public abstract class PointCloud<PointContainerType, PointType> :
            PointCloud<PointLink<PointContainerType, PointType>, PointContainerType, PointType>,
            ILockable
        where PointContainerType: PointContainer<PointLink<PointContainerType, PointType>, PointContainerType, PointType>
        //where PointType: class
    {
        public PointCloud()
            : base()
        {  }


        /// <summary>Constructs a cloud of points containing the specified points.</summary>
        /// <param name="points">Points that are included in the created point cloud.</param>
        public PointCloud(params PointType[] points)
            : this()
        { AddPoints(points); }

        /// <summary>Constructs a cloud of points containing the points embedded in the specified point containers.
        /// <para>All points are taken from their point containers and embedded in newly created point containers before
        /// adding to the created cloud of points.</para></summary>
        /// <param name="points">Points embedded in point containers that are included in the created point cloud.</param>
        public PointCloud(params IPointContainer<PointType>[] points)
            : this()
        { AddPoints(points); }

    }


    /// <summary>Cloud of points, contains a list of containers of objects that include point coordinates.</summary>
    /// <typeparam name="PointLinkType">Type of point container used by the class.</typeparam>
    /// <typeparam name="PointContainerType">Type of point link that is used by point container.</typeparam>
    /// <typeparam name="PointType">Type of objects that include point coordinates.</typeparam>
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
    public abstract class PointCloud<PointLinkType, PointContainerType, PointType> : ILockable
        where PointLinkType : PointLink<PointLinkType, PointContainerType, PointType>
        where PointContainerType: PointContainer<PointLinkType, PointContainerType, PointType>
        //where PointType: class
    {

        /// <summary>Constructs an empty cloud of points.</summary>
        public PointCloud()
        {  }

        /// <summary>Constructs a cloud of points containing the specified points.</summary>
        /// <param name="points">Points that are included in the created point cloud.</param>
        public PointCloud(params PointType[] points): this()
        { AddPoints(points); }

        /// <summary>Constructs a cloud of points containing the points embedded in the specified point containers.
        /// <para>All points are taken from their point containers and embedded in newly created point containers before
        /// adding to the created cloud of points.</para></summary>
        /// <param name="points">Points embedded in point containers that are included in the created point cloud.</param>
        public PointCloud(params IPointContainer<PointType>[] points): this()
        { AddPoints(points); }


        
        #region ILockable

        protected internal object _lock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _lock; } }

        #endregion ILockable


        #region  Data

        protected List<PointContainerType> _points = new List<PointContainerType>();

        /// <summary>List of points contained on the current class.</summary>
        public List<PointContainerType> Points
        {
            get { return _points; }
        }

        /// <summary>Gets the current number of points on the list.</summary>
        public int NumPoints
        {
            get { return _points.Count; }
        }

        /// <summary>Returns the specified point identified by its position (index) within list of points.</summary>
        /// <param name="which">Point index in the list.</param>
        public PointContainerType GetPoint(int which)
        {
            return _points[which];
        }

        /// <summary>Returns the specified point identified by its position (index) within list of points.</summary>
        /// <param name="which">Point index in the list.</param>
        public PointContainerType this[int which]
        {
            get { return _points[which]; }
            protected set {
                if (which < 0 || which >= _points.Count)
                    throw new IndexOutOfRangeException("Point index " + which + " is out of range, should be between 0 and " 
                        + (_points.Count-1) + ".");
                _points[which] = value;
            }
        }

        /// <summary>Sets the points to be contained in the current point cloud.
        /// <para>Each point is embedded in a newly created point container.</para></summary>
        /// <param name="points">Points to be contained in the current point cloud.</param>
        public void SetPoints(params PointType[] points)
        {
            _points.Clear();
            foreach (PointType currentPoint in points)
            {
                _points.Add(CreatePointContainer(currentPoint));
            }
        }

        /// <summary>Adds the specified points to the current point cloud.
        /// <para>Each added point is embedded in a newly created point container.</para></summary>
        /// <param name="points">Points embedded in point containers to be contained in the current point cloud.</param>
        public void AddPoints(params PointType[] points)
        {
            foreach (PointType currentPoint in points)
            {
                _points.Add(CreatePointContainer(currentPoint));
            }
        }

        /// <summary>Sets the points to be contained in the current point cloud to the specified set of points
        /// that are embedded in point containers.
        /// <para>Each of the specified points is taken from its current container embedded in a newly created point container.</para></summary>
        /// <param name="points">Points to be contained in the current point cloud.</param>
        public void SetPoints(params IPointContainer<PointType>[] points)
        {
            _points.Clear();
            foreach (IPointContainer<PointType> currentPoint in points)
            {
                _points.Add(CreatePointContainer(currentPoint.Point));
            }
        }

        /// <summary>Adds the specified points that are embedded in point containers to the current point cloud.
        /// <para>Each of the specified points is taken from its current container embedded in a newly created point container.</para></summary>
        /// <param name="points">Points embedded in point containers to be contained in the current point cloud.</param>
        public void AddPoints(params IPointContainer<PointType>[] points)
        {
            foreach (IPointContainer<PointType> currentPoint in points)
            {
                _points.Add(CreatePointContainer(currentPoint.Point));
            }
        }


        #endregion Data


        #region Operation 

        public DistanceComparer<PointContainerType, PointType> _distanceComparer;

        /// <summary>Distance comparer of the current point cloud object.
        /// <para>Getter has lazy evaluation (The default distance comparer is created the first time when needed
        /// by the <see cref="CreateDefaultDistanceComparer"/> mathod if distance comparer has not ben set before).</para></summary>
        public DistanceComparer<PointContainerType, PointType> DistanceComparer
        {
            get {
                if (_distanceComparer == null)
                {
                    lock (Lock)
                    {
                        _distanceComparer = CreateDefaultDistanceComparer();
                    }
                }
                return _distanceComparer; 
            }
            set { _distanceComparer = value; }
        }


        /// <summary>Creates and returnws a distance comparer of hte default type for the current 
        /// type of point of clouds.</summary>
         public abstract DistanceComparer<PointContainerType, PointType> CreateDefaultDistanceComparer(PointContainerType point);

        /// <summary>Creates and returnws a distance comparer of hte default type for the current 
        /// type of point of clouds.</summary>
        public virtual DistanceComparer<PointContainerType, PointType> CreateDefaultDistanceComparer()
        {
            PointContainerType point = null;
            if (_points!=null)
                if (_points.Count > 0)
                {
                    point = _points[0];
                }
            return CreateDefaultDistanceComparer(point);
        }


        /// <summary>Creates and returns a new point with specified coordinates.</summary>
        /// <param name="coordinates">Coordinates of the created point. A copy of this vector should always be created
        /// to hold coordinates within the point, because the caller is allowed to modify coordinates on the vector.</param>
        public abstract PointType CreatePointFromCoordinates(IVector coordinates);
 
        /// <summary>Creates and returns a new point container that wraps the specified point.</summary>
        /// <param name="point">Point to be wrapped.</param>
        public abstract PointContainerType CreatePointContainer(PointType point);

        /// <summary>Creates and returns a new point container that wraps a newly created point with the specified coordinates.
        /// <param name="coordinates">Coordinates of the embedded point type. A copy of this vector is normally created
        /// to hold coordinates within the point, because the caller is allowed to modify coordinates on the vector.</param>
        public virtual PointContainerType CreatePointContainerFromCoordinates(IVector coordinates)
        {
            return CreatePointContainer(CreatePointFromCoordinates(coordinates));
        }

        /// <summary>Creates and returns a copy of list of points, containing references to points (point containers,
        /// in fact) contained in the current cloud of points.</summary>
        /// <returns></returns>
        public List<PointContainerType> CreatePointsCopy()
        {
            lock(Lock)
            {
                List<PointContainerType> ret = new List<PointContainerType>();
                int num = this.NumPoints;
                for (int i=0; i<num; ++i)
                    ret.Add(_points[i]);
                return ret;
            }
        }
        
        #endregion Operation


        #region UtilitiesRandomPoints

        private IRandomGenerator _rand;

        /// <summary>Random generator used by the current cloud of points.</summary>
        public IRandomGenerator Rand
        {
            get 
            {
                if (_rand == null)
                    lock (Lock)
                    {
                        if (_rand==null)
                        _rand = RandomGenerator.Global;
                    }
                return _rand;
            }
            set
            {
                _rand = value;
            }
        }


        protected IVector _auxVector;


        /// <summary>Creates and returns a new point with random co-ordinates that fall within the specified 
        /// bounds (inclusively).
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="bounds">Bounds (inclusive) on co-ordinates of the generated points.</param>
        public PointType CreateRandomPoint(IBoundingBox bounds)
        {
            if (bounds == null)
                throw new ArgumentException("Dimension of auxiliary vector is different than dimension of bounds.");
            lock (Lock)
            {
                int spaceDimension = bounds.Dimension;
                bool allocate = false;
                if (_auxVector == null)
                    allocate = true;
                else if (_auxVector.Length != spaceDimension)
                    allocate = true;
                if (allocate)
                    _auxVector = new Vector(spaceDimension);
                for (int i = 0; i < spaceDimension; ++i)
                {
                    if (!bounds.IsMinDefined(i))
                        throw new Exception("Lower bound is not defined for point co-ordinate No. " + i + ".");
                    if (!bounds.IsMaxDefined(i))
                        throw new Exception("Upper bound is not defined for point co-ordinate No. " + i + ".");
                    _auxVector[i] = Rand.NextDoubleInclusive(bounds.GetMin(i), bounds.GetMax(i));
                }
                return CreatePointFromCoordinates(_auxVector);
            }
        }

        /// <summary>Creates and returns a new point with random co-ordinates that fall within the specified 
        /// bounds (inclusively).
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="spaceDimension">Dimension of generated point.</param>
        /// <param name="minCoordinateValues">Minimal value of co-ordinates (inclusive).</param>
        /// <param name="maxCoordinateValues">Maximal value of co-ordinates (inclusive).</param>
        public PointType CreateRandomPoint(int spaceDimension, double minCoordinateValues, double maxCoordinateValues)
        {
            lock (Lock)
            {
                bool allocate = false;
                if (_auxVector == null)
                    allocate = true;
                else if (_auxVector.Length != spaceDimension)
                    allocate = true;
                if (allocate)
                    _auxVector = new Vector(spaceDimension);
                for (int i = 0; i < spaceDimension; ++i)
                {
                    _auxVector[i] = Rand.NextDoubleInclusive(minCoordinateValues, maxCoordinateValues);
                }
                return CreatePointFromCoordinates(_auxVector);
            }
        }

        /// <summary>Creates and returns a new point with random co-ordinates that fall between 0 and 1 (inclusively).
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="spaceDimension">Dimension of generated point.</param>
        public PointType CreateRandomPoint(int spaceDimension)
        {
            return CreateRandomPoint(spaceDimension, 0 /* minCoordinateValues */, 1  /* maxCoordinateValues */);
        }

        /// <summary>Creates and returns a new point container with random co-ordinates that fall within the specified 
        /// bounds (inclusively).
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="bounds">Bounds (inclusive) on co-ordinates of the generated points.</param>
        public PointContainerType CreateRandomPointContainer(IBoundingBox bounds)
        {
            return CreatePointContainer(CreateRandomPoint(bounds));
        }

        /// <summary>Creates and returns a new point container with random co-ordinates that fall within the specified 
        /// bounds (inclusively).
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="spaceDimension">Dimension of generated point.</param>
        /// <param name="minCoordinateValues">Minimal value of co-ordinates (inclusive).</param>
        /// <param name="maxCoordinateValues">Maximal value of co-ordinates (inclusive).</param>
        public PointContainerType CreateRandomPointContainer(int spaceDimension, double minCoordinateValues, double maxCoordinateValues)
        {
            return CreatePointContainer(CreateRandomPoint(spaceDimension, minCoordinateValues, maxCoordinateValues));
        }

        /// <summary>Creates and returns a new point container with random co-ordinates that fall between 0 and 1 (inclusively).
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="spaceDimension">Dimension of generated point.</param>
        public PointContainerType CreateRandomPointContainer(int spaceDimension)
        {
            return CreatePointContainer(CreateRandomPoint(spaceDimension));
        }


        /// <summary>Generates a specified number of random points with random co-ordinates that fall within the specified 
        /// bounds (inclusively), and incorporates them into the current point cloud. Eventual existent points are removed
        /// from the cloud.
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="bounds">Bounds (inclusive) on co-ordinates of the generated points.</param>
        public void GenerateRandomCloud(int numPoints, IBoundingBox bounds)
        {
            lock (Lock)
            {
                Points.Clear();
                for (int i = 0; i < numPoints; ++i)
                    Points.Add(CreateRandomPointContainer(bounds));
            }
        }

        /// <summary>Generates a specified number of random points with random co-ordinates that fall within the specified 
        /// bounds (inclusively), and incorporates them into the current point cloud. Eventual existent points are removed
        /// from the cloud.
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="spaceDimension">Dimension of generated point.</param>
        /// <param name="minCoordinateValues">Minimal value of co-ordinates (inclusive).</param>
        /// <param name="maxCoordinateValues">Maximal value of co-ordinates (inclusive).</param>
        public void GenerateRandomCloud(int numPoints, int spaceDimension, double minCoordinateValues, double maxCoordinateValues)
        {
            lock (Lock)
            {
                Points.Clear();
                for (int i = 0; i < numPoints; ++i)
                    Points.Add(CreateRandomPointContainer(spaceDimension ,minCoordinateValues, maxCoordinateValues));
            }
        }

        /// <summary>Generates a specified number of random points with random co-ordinates that fall between 0 and 1 (inclusively), 
        /// and incorporates them into the current point cloud. Eventual existent points are removed from the cloud.
        /// <para>Object's internal random generator is used to create random co-ordinates. This random
        /// generator can be set through the <see cref="Rand"/> property.</para></summary>
        /// <param name="spaceDimension">Dimension of generated point.</param>
        public void GenerateRandomCloud(int numPoints, int spaceDimension)
        {
            lock (Lock)
            {
                Points.Clear();
                for (int i = 0; i < numPoints; ++i)
                    Points.Add(CreateRandomPointContainer(spaceDimension));
            }
        }


        #endregion UtilitiesRandomPoints


        #region UtilitiesInspection

        /// <summary>Iterates through all points contained in the current cloud, and calculates and
        /// updates their distances to the containing point by the specified distance comparer.</summary>
        public void UpdateNeighborDistances(IDistanceComparer<PointContainerType, PointType> distanceComparer)
        {
            lock (Lock)
            {
                int numPoints = _points.Count;
                for (int i = 0; i < numPoints; ++i)
                {
                    PointContainerType point = _points[i];
                    List<PointLinkType> neighbors = point.Neighbors;
                    int numNeighbors = neighbors.Count;
                    for (int k = 0; k < numNeighbors; ++k)
                    {
                        PointLinkType link = neighbors[k];
                        if (link != null)
                        {
                            PointContainerType neighbor = link.Point;
                            if (neighbor != null)
                            {
                                link.DistanceFromPoint = distanceComparer.Distance(point, neighbor);
                            }
                        }
                    }

                }
            }
        }


        /// <summary>Calculates a number of statistics on the distances of the first specified number 
        /// of closest neighbors to each point, and stores them to the provided storage.
        /// <para>It is assumed htat distances to the master points are calculated and stored on neighbors (links),
        /// and that neighbors are sorted accorgding to ascending distances from their master point.</para>
        /// <para>Objects to store data are re-allocated if necessary.</para></summary>
        /// <param name="numClosestPoints">Number of closest neighbors for which statistics are calculated.</param>
        /// <param name="numSpecimens">Numbers of actual samples for each k-th closest neighbor. I.e., the k-th 
        /// element will store the number of points for which the k-th closest neighbor is actually defined.</param>
        /// <param name="minDistances">Minimal distances. The k-th element stores minimal distance of the k-th
        /// closest neighbor from its master over all points in the cloud.</param>
        /// <param name="maxDistances">Maximal distances. The k-th element stores maximal distance of the k-th
        /// closest neighbor from its master over all points in the cloud.</param>
        /// <param name="averageDistances">Average distances. The k-th element stores average distance of the k-th
        /// closest neighbor from its master over all points in the cloud.</param>
        public void GetNeighborDistanceStatistics(int numClosestPoints, ref int[] numSpecimens,
            ref IVector minDistances, ref IVector maxDistances, ref IVector averageDistances)
        {
            IVector standardDeviations = null, averageAbsoluteDeviations = null;
            GetNeighborDistanceStatistics(numClosestPoints, ref numSpecimens,
                        ref minDistances, ref maxDistances, ref averageDistances,
                        ref standardDeviations, ref averageAbsoluteDeviations, false /* calculateDeviations */);
        }


        /// <summary>Calculates a number of statistics on the distances of the first specified number 
        /// of closest neighbors to each point, and stores them to the provided storage.
        /// <para>It is assumed htat distances to the master points are calculated and stored on neighbors (links),
        /// and that neighbors are sorted accorgding to ascending distances from their master point.</para>
        /// <para>Objects to store data are re-allocated if necessary.</para></summary>
        /// <param name="numClosestPoints">Number of closest neighbors for which statistics are calculated.</param>
        /// <param name="numSpecimens">Numbers of actual samples for each k-th closest neighbor. I.e., the k-th 
        /// element will store the number of points for which the k-th closest neighbor is actually defined.</param>
        /// <param name="minDistances">Minimal distances. The k-th element stores minimal distance of the k-th
        /// closest neighbor from its master over all points in the cloud.</param>
        /// <param name="maxDistances">Maximal distances. The k-th element stores maximal distance of the k-th
        /// closest neighbor from its master over all points in the cloud.</param>
        /// <param name="averageDistances">Average distances. The k-th element stores average distance of the k-th
        /// closest neighbor from its master over all points in the cloud.</param>
        /// <param name="standardDeviations">Standard deviations of distances. The k-th element stores standard deviation of
        /// distance of the k-th closest neighbor from its master over all points in the cloud.</param>
        /// <param name="averageAbsoluteDeviations">Average absolute deviations of distances. The k-th element stores average 
        /// absolute deviation of distance of the k-th closest neighbor from its master over all points in the cloud.</param>
        /// <param name="calculateDeviations">Specifies whether the standard deviation and average absolute deviations 
        /// of distances are also calculated.</param>
        public void GetNeighborDistanceStatistics(int numClosestPoints, ref int[] numSpecimens,
            ref IVector minDistances, ref IVector maxDistances, ref IVector averageDistances, 
            ref IVector standardDeviations, ref IVector averageAbsoluteDeviations, bool calculateDeviations)
        {
            lock (Lock)
            {
                if (numSpecimens == null)
                    numSpecimens = new int[numClosestPoints];
                if (numSpecimens.Length != numClosestPoints)
                    numSpecimens = new int[numClosestPoints];
                if (minDistances == null)
                    minDistances = new Vector(numClosestPoints);
                if (minDistances.Length != numClosestPoints)
                    minDistances = new Vector(numClosestPoints);
                if (maxDistances == null)
                    maxDistances = new Vector(numClosestPoints);
                if (maxDistances.Length != numClosestPoints)
                    maxDistances = new Vector(numClosestPoints);
                if (averageDistances == null)
                    averageDistances = new Vector(numClosestPoints);
                if (averageDistances.Length != numClosestPoints)
                    averageDistances = new Vector(numClosestPoints);
                if (calculateDeviations)
                {
                    if (standardDeviations == null)
                        standardDeviations = new Vector(numClosestPoints);
                    if (standardDeviations.Length != numClosestPoints)
                        standardDeviations = new Vector(numClosestPoints);
                    if (averageAbsoluteDeviations == null)
                        averageAbsoluteDeviations = new Vector(numClosestPoints);
                    if (averageAbsoluteDeviations.Length != numClosestPoints)
                        averageAbsoluteDeviations = new Vector(numClosestPoints);
                }
                for (int k = 0; k < numClosestPoints; ++k)
                {
                    numSpecimens[k] = 0;
                    minDistances[k] = double.MaxValue;
                    maxDistances[k] = double.MinValue;
                    averageDistances[k] = 0;
                    if (calculateDeviations)
                    {
                        standardDeviations[k] = 0;
                        averageAbsoluteDeviations[k] = 0;
                    }
                }
                int numPoints = _points.Count;
                for (int i = 0; i < numPoints; ++i)
                {
                    // Iterate over points in the cloud:
                    PointContainerType point = GetPoint(i);
                    List<PointLinkType> neighbors = point.Neighbors;
                    int numNeighbors = neighbors.Count;
                    for (int k = 0; k < numNeighbors && k<numClosestPoints; ++k)
                    {
                        PointLinkType link = neighbors[k];
                        if (link != null)
                        {
                            PointContainerType neighbor = link.Point;
                            if (neighbor != null)
                            {
                                // the k-th neighbor of i-th point is defined, therefore we use its recorded
                                // distance to the point in statistics:
                                ++ numSpecimens[k];
                                double distance = link.DistanceFromPoint;
                                if (distance < minDistances[k])
                                    minDistances[k] = distance;
                                if (distance > maxDistances[k])
                                    maxDistances[k] = distance;
                                averageDistances[k] += distance;
                            }
                        }
                    }  // over all neighbors of the specific point
                }  // over all points in the cloud
                // calculate averages - divide from number of individuals: 
                for (int k = 0; k < numClosestPoints; ++k)
                {
                    if (numSpecimens[k] != 0)
                        averageDistances[k] /= numSpecimens[k];
                }
                if (calculateDeviations)
                {
                    // Repeat the loop to calculate stndard deviations:

                    for (int i = 0; i < numPoints; ++i)
                    {
                        // Iterate over points in the cloud:
                        PointContainerType point = GetPoint(i);
                        List<PointLinkType> neighbors = point.Neighbors;
                        int numNeighbors = neighbors.Count;
                        for (int k = 0; k < numNeighbors && k < numClosestPoints; ++k)
                        {
                            PointLinkType link = neighbors[k];
                            if (link != null)
                            {
                                PointContainerType neighbor = link.Point;
                                if (neighbor != null)
                                {
                                    // the k-th neighbor of i-th point is defined, therefore we use its recorded
                                    // distance to the point in calculation of standard deviation:
                                    double distance = link.DistanceFromPoint;
                                    double deviation = distance - averageDistances[k];
                                    standardDeviations[k] += deviation * deviation;
                                    averageAbsoluteDeviations[k] += Math.Abs(deviation);
                                }
                            }
                        }  // over all neighbors of the specific point
                    }  // over all points in the cloud
                    for (int k = 0; k < numClosestPoints; ++k)
                    {
                        if (numSpecimens[k] != 0)
                        {
                            standardDeviations[k] = Math.Sqrt(standardDeviations[k]/numSpecimens[k]);
                            averageAbsoluteDeviations[k] = averageAbsoluteDeviations[k] / numSpecimens[k];
                        }
                    }
                }
            }
        }


        protected void PrintNeighborDistanceStatistics(int[] numSpecimens,
            IVector minDistances, IVector maxDistances, IVector averageDistances,
            IVector standardDeviations, IVector averageAbsoluteDeviations, bool calculateDeviations)
        {
            Console.WriteLine("Neighbor points distance statistics: ");
            Console.WriteLine("Distance of neighbors to correspoinding points are worked.");
            Console.WriteLine("Printed quantities: ");
            Console.WriteLine("  - sequential number of neighbor of the considered points");
            Console.WriteLine("  - number of specimens found for this sequential number");
            Console.WriteLine("  - minimal distance for neighbor with the current sequential number");
            Console.WriteLine("  - maximal distance for this neighbor");
            Console.WriteLine("  - average distance for this neighbor");
            if (calculateDeviations)
            {
                Console.WriteLine("  - standard deviation of distances for this neighbor");
                Console.WriteLine("  - average absolute deviation from average for this neighbor");
            }
            Console.WriteLine();
            Console.WriteLine("Distance statistics: ");
            for (int i = 0; i < numSpecimens.Length; ++i)
            {
                Console.Write("{0,5}: {1,6} spec., dist. {2,8:#.###E0} - {3,-8:#.###E0}, av.: {4,-8:#.###E0}",
                    i, numSpecimens[i], minDistances[i], maxDistances[i], averageDistances[i]);
                //if (calculateDeviations)
                //{
                //    Console.WriteLine();
                //    Console.Write("                     std. dev.: {0,-8:#.###E0}, av. abs. dev.: {0,-8:#.###E0}",
                //        standardDeviations[i], averageAbsoluteDeviations[i]);
                //}
                Console.WriteLine();
            }
            if (calculateDeviations)
            {
                Console.WriteLine("Averages and deviations:");
                for (int i = 0; i < numSpecimens.Length; ++i)
                {
                    Console.Write("{0,5}: av.: {1,-8:#.###E0},  std. dev.: {2,-8:#.###E0}, av. abs. dev.: {3,-8:#.###E0}",
                        i, averageDistances[i], standardDeviations[i], averageAbsoluteDeviations[i]);
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }


        protected void PrintNeighborDistanceStatistics(int[] numSpecimens,
            IVector minDistances, IVector maxDistances, IVector averageDistances)
        {
            PrintNeighborDistanceStatistics(numSpecimens, minDistances, maxDistances, averageDistances,
                null /* standardDeviations */, null /* averageAbsoluteDeviations */, false /* calculateDeviations */);
        }

        
        /// <summary>Calculates and prints a number of statistics on the distances of the first specified number 
        /// of closest neighbors to each point, and stores them to the provided storage.
        /// <para>It is assumed htat distances to the master points are calculated and stored on neighbors (links),
        /// and that neighbors are sorted accorgding to ascending distances from their master point.</para>
        /// <para>Objects to store data are re-allocated if necessary.</para></summary>
        /// <param name="numClosestPoints">Number of closest neighbors for which statistics are calculated.</param>
        /// <param name="calculateDeviations">Specifies whether the standard deviation and average absolute deviations 
        /// of distances are also calculated.</param>
        /// <param name="distanceComparer">Distance comparer used to calculate distances and to compare points.</param>
        public void PrintNeighborDistanceStatistics(int numClosestPoints, bool calculateDeviations, 
            IDistanceComparer<PointContainerType, PointType> distanceComparer)
        {
            if (distanceComparer == null)
                throw new ArgumentException("Distance comparer is not specified (null argument).");
            StopWatch1 t = new StopWatch1();
            Console.WriteLine();
            Console.WriteLine("Calculating statistics for " + numClosestPoints + " closest neighbors...");
            t.Start();
            UpdateNeighborDistances(distanceComparer);
            t.Stop();
            Console.WriteLine("Neighbor distances updated in " + t.Time + "s.");
            int[] numSpecimens = null;
            IVector 
                minDistances = null, 
                maxDistances = null, 
                averageDistances = null, 
                standardDeviations = null, 
                averageAbsoluteDeviations = null;
             t.Start();
            GetNeighborDistanceStatistics(numClosestPoints, ref numSpecimens,
                ref minDistances, ref maxDistances, ref averageDistances,
                ref standardDeviations, ref averageAbsoluteDeviations, calculateDeviations);
            t.Stop();
            Console.WriteLine("Statistics calculated in " + t.Time + "s.");
            PrintNeighborDistanceStatistics(numSpecimens,
                minDistances, maxDistances, averageDistances,
                standardDeviations, averageAbsoluteDeviations, calculateDeviations);
        }



        #endregion UtilitiesInspection



        #region UtilitiesPrintClosestPoints

        // PRINTING STATISTICS on CLOSEST POINTS to a set of reference points:

        // TODO: Transfer also the TestClosestPoints(..., NeuralTrainingElement[] points , ...) methods 
        // (already copied below) that also print statistics on output values!

        // TODO: Test these mathods and polish them!

        /* 
         * Methods to implement on comparer:
         * GetNeuralInputRelative()  => comparer.GetInputRelative
         * 
         * */

        /// <summary>For each point in the specified array, the training points are sorted according to the 
        /// distance to this point, and data for the specified number of closest points are written.
        /// <para></para>
        /// <para>Result of this test can give some rough feeling about filling of space (but very rough 
        /// because anisotropy can not be detected in this way).</para></summary>
        /// <param name="numClosestPoints">Number of closest points that are written.</param>
        /// <param name="includeVerificationPoints">Whether verification points are included or not.</param>
        /// <param name="printByComponents">If true then results are also printed by components.</param>
        /// <param name="referencePoints">Points that are checked for closest data points contained in the specified 
        /// list of points.</param>
        /// <param name="comparerInput">Comparer object that calculates distance between points and compares
        /// compares points according to their distance to a reference point.</param>
        /// /// <param name="trainingPoints">Cloud of points that cover specific region of space and for which 
        /// one wants to check how well the space is covered by these points.</param>
        /// $A Igor xx Jan12;
        public static void TestClosestPoints(int numClosestPoints, 
            bool printByComponents, PointContainerType[] referencePoints,
            IDistanceComparer<PointContainerType, PointType> comparerInput, List<PointContainerType> trainingPoints)
        {
            TestClosestPoints(numClosestPoints, /* includeVerificationPoints, */
                printByComponents, false /* printIndividualPointsCom */ , referencePoints, comparerInput, trainingPoints);
        }


        /// <summary>For each point in the specified array, the training points are sorted according to the 
        /// distance to this point, and data for the specified number of closest points are written.
        /// <para>For individual points, differences are not printed by components.</para>
        /// <para>Result of this test can give some rough feeling about filling of space (but very rough 
        /// because anisotropy can not be detected in this way).</para></summary>
        /// <param name="numClosestPoints">Number of closest points that are written.</param>
        /// <param name="includeVerificationPoints">Whether verification points are included or not.</param>
        /// <param name="printByComponents">If true then results are also printed by components.</param>
        /// <param name="printIndividualPointsComp">If true then individual components of differences are printed for
        /// individual points (otherwise, they are only printed in statistics).</param>
        /// <param name="referencePoints">Points that are checked for closest data points contained in the specified 
        /// list of points.</param>
        /// <param name="comparerInput">Comparer object that calculates distance between points and compares
        /// compares points according to their distance to a reference point.</param>
        /// <param name="trainingPoints">Cloud of points that cover specific region of space and for which 
        /// one wants to check how well the space is covered by these points.</param>
        /// $A Igor xx Jan12;
        public static void TestClosestPoints(int numClosestPoints,  
            bool printByComponents, bool printIndividualPointsComp, 
            PointContainerType[] referencePoints,
            IDistanceComparer<PointContainerType, PointType> comparerInput, 
            List<PointContainerType> trainingPoints 
            /* PointCloud<PointLinkType, PointContainerType, PointType> trainingPoints */
                )
        {

            // TODO: clean and test this method!

            Console.WriteLine();
            Console.WriteLine("Checking closest trining data points for the specified set of points...");
            if (numClosestPoints < 1)
                throw new ArgumentException("Number of closest points to be written for each chosen points should be at least 1.");
            if (referencePoints == null)
                throw new ArgumentException("No points to be checked are specified.");
            int numPoints = referencePoints.Length; // points.NumPoints;
            if (numPoints < 1)
                throw new ArgumentException("The number of specified points to be checked is less than 1.");


            //NeuralTrainingSet.ComparerInputDistance comparerInput = new NeuralTrainingSet.ComparerInputDistance
            //    (points[0], InputDistance);
            //IDistanceComparer<PointContainerType, PointType> comparerInput = points.DistanceComparer;


            // List<NeuralTrainingElement> trainingElements = GetTrainingElements(includeVerificationPoints);
            List<PointContainerType> trainingElements = trainingPoints.GetRange(0, trainingPoints.Count);

            int NumNeuralParameters = referencePoints[0].GetPointCoordinates().Length;

            if (trainingElements.Count < numClosestPoints)
                numClosestPoints = trainingElements.Count;
            StopWatch1 t = new StopWatch1();
            IVector currentRelative = new Vector(NumNeuralParameters);       // current point in relative coordinates
            IVector closestRelative = new Vector(NumNeuralParameters);  // i-th closest point in relative coordinates
            IVector differenceRelative = new Vector(NumNeuralParameters);    // difference of the above

            PointContainerType currentPoint = null;
            double[] minDistances = new double[numClosestPoints], maxDistances = new double[numClosestPoints],
                averageDistances = new double[numClosestPoints];

            IVector[] minAbsDif = null, maxAbsDif = null, averageAbsDif = null;
            if (printByComponents)
            {
                minAbsDif = new IVector[numClosestPoints];
                maxAbsDif = new IVector[numClosestPoints];
                averageAbsDif = new IVector[numClosestPoints];
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    minAbsDif[i] = new Vector(NumNeuralParameters);
                    maxAbsDif[i] = new Vector(NumNeuralParameters);
                    averageAbsDif[i] = new Vector(NumNeuralParameters);
                    for (int k = 0; k < NumNeuralParameters; ++k)
                    {
                        minAbsDif[i][k] = double.MaxValue;
                        maxAbsDif[i][k] = double.MinValue;
                        averageAbsDif[i][k] = 0;
                    }
                }
            }
            for (int i = 0; i < numClosestPoints; ++i)
            {
                minDistances[i] = double.MaxValue;
                maxDistances[i] = double.MinValue;
                averageDistances[i] = 0.0;
            }
            for (int iPoint = 0; iPoint < numPoints; ++iPoint)
            {
                currentPoint = referencePoints[iPoint];
                comparerInput.GetRelativeCoordinates(currentPoint.GetPointCoordinates(), ref currentRelative);
                t.Start();
                // Sort training elemets according to their distance to the current point:
                comparerInput.ReferencePoint = currentPoint;
                trainingElements.Sort(comparerInput);
                t.Stop();
                Console.WriteLine();
                Console.WriteLine("Point No. " + iPoint + " (calculated in "
                    + t.Time.ToString("0.#####") + " s):");
                Console.WriteLine("Coordinates: ");
                Console.WriteLine("  " + currentPoint.GetPointCoordinates().ToString("#.####e0"));
                Console.WriteLine("  Relative coordinates: ");
                Console.WriteLine("  " + currentRelative.ToString("0.####"));
                if (printByComponents)
                {
                    if (printIndividualPointsComp)
                    {
                        Console.WriteLine();
                        Console.WriteLine("The first " + numClosestPoints + " closest points from the training set with ");
                        Console.WriteLine("differences (component-wise) expressed in relative coordinates:");
                    }
                    for (int i = 0; i < numClosestPoints; ++i)
                    {
                        PointContainerType element = trainingElements[i];
                        double inputDistance = comparerInput.Distance(element, currentPoint);
                        comparerInput.GetRelativeCoordinates(element.GetPointCoordinates(), ref closestRelative);
                        Vector.Subtract(closestRelative, currentRelative, ref differenceRelative);
                        if (printIndividualPointsComp)
                            Console.WriteLine("{0,4}.: {1}", i, differenceRelative.ToString("0.####"));
                        // Calculate statistics on indivitual components, for i-th closest point:
                        for (int k = 0; k < differenceRelative.Length; ++k)
                        {
                            double dif = Math.Abs(differenceRelative[k]);  // here we take absolute values of difference components
                            differenceRelative[k] = dif;
                            if (dif < minAbsDif[i][k])
                                minAbsDif[i][k] = dif;
                            if (dif > maxAbsDif[i][k])
                                maxAbsDif[i][k] = dif;
                            averageAbsDif[i][k] += dif / (double)numPoints;
                        }
                    }  // work over specified number of closest points to the current specified point
                }
                Console.WriteLine("The first " + numClosestPoints + " closest points from the training set with disances: ");
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    PointContainerType element = trainingElements[i];
                    double inputDistance = comparerInput.Distance(element, currentPoint);
                    Console.WriteLine("{0,4}: d = {1,-10:0.0000e0}, el. index: {2,-4}", i, inputDistance, element.Index);
                    if (inputDistance < minDistances[i])
                        minDistances[i] = inputDistance;
                    if (inputDistance > maxDistances[i])
                        maxDistances[i] = inputDistance;
                    averageDistances[i] += inputDistance / (double)numPoints;
                }  // work over specified number of closest points to the current specified point
            }  // iterate over specified points
            if (printByComponents)
            {
                Console.WriteLine();
                Console.WriteLine("Statistics on closest points absolute differences in componets (relative coordinates):");
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    string formatString = "0.00e0";
                    Console.WriteLine("*{0} -th closest point: ", i);
                    Console.WriteLine("  Minimal absolute difference of relative components:");
                    Console.WriteLine("    " + minAbsDif[i].ToString(formatString));
                    Console.WriteLine("  Maximal absolute difference of relative components:");
                    Console.WriteLine("    " + maxAbsDif[i].ToString(formatString));
                    Console.WriteLine("  Average absolute difference of relative components:");
                    Console.WriteLine("    " + averageAbsDif[i].ToString(formatString));
                }  // work over specified number of closest points to the current specified point
            }
            Console.WriteLine();
            Console.WriteLine("Statistics on closest points distances: ");
            Console.WriteLine("{0,10} {1,-10:0.0000E0}   {2,10:0.0000E0}    {3,-10:0.0000E0}",
                "No. ", "min. d", "max. d", "average");
            for (int i = 0; i < numClosestPoints; ++i)
            {
                Console.WriteLine("{0,8}.: {1,-10:0.0000E0} - {2,10:0.0000E0};   {3,-10:0.0000E0}",
                    i, minDistances[i], maxDistances[i], averageDistances[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Total time for all calculations ({0} test points, {1} closest points): {2:0.####} s",
                numPoints, numClosestPoints, t.TotalTime);
            Console.WriteLine();
        }



        #region PrintClosestPointsWithOutputVectors

        // This reigion contains utilities for printing closest points in the case where the point type has defined
        // vectors of output values (such as, for ecample, approximation data points).



        ///// <summary>For each point (training element) in the specified array, the training points are 
        ///// sorted according to the distance to this point, and data for the specified number of closest points are written.
        ///// <para>For individual points, differences are not printed by components.</para>
        ///// <para>Result of this test can give some rough feeling about filling of space (but very rough 
        ///// because anisotropy can not be detected in this way).</para></summary>
        ///// <param name="numClosestPoints">Number of closest points that are written.</param>
        ///// <param name="includeVerificationPoints">Whether verification points are included or not.</param>
        ///// <param name="printByComponents">If true then results are also printed by components.</param>
        ///// <param name="points">Points that are checked for closest trainnig data points.</param>
        ///// <param name="comparer">Comparer object that calculates distance between points and compares
        ///// compares points according to their distance to a reference point.</param>
        ///// $A Igor xx Jan12;
        //public void TestClosestPointsWithOutputs(int numClosestPoints, bool includeVerificationPoints,
        //    bool printByComponents, PointCloud<PointLinkType, PointContainerType, PointType> points,
        //    IDistanceComparer<PointContainerType, PointType> comparer)
        //{
        //    TestClosestPointsWithOutputs(numClosestPoints, includeVerificationPoints,
        //        printByComponents, false /* printIndividualPointsComp */, points, comparer);
        //}


        /// <summary>For each point (training element) in the specified array, the training points are 
        /// sorted according to the distance to this point, and data for the specified number of closest points are written.
        /// <para></para>
        /// <para>Result of this test can give some rough feeling about filling of space (but very rough 
        /// because anisotropy can not be detected in this way).</para></summary>
        /// <param name="numClosestPoints">Number of closest points that are written.</param>
        /// <param name="includeVerificationPoints">Whether verification points are included or not.</param>
        /// <param name="printByComponents">If true then results are also printed by components.</param>
        /// <param name="printIndividualPoints">If true then individual components of differences are printed for
        /// <param name="points">Points that are checked for closest trainnig data points.</param>
        /// <param name="comparerInput">Comparer object that calculates distance between points and compares
        /// compares points according to their distance to a reference point.</param>
        /// <param name="trainingPoints">Cloud of points that cover specific region of space and for which 
        /// one wants to check how well the space is covered by these points.</param>
        /// $A Igor xx Jan12;
        public void TestClosestPointsWithOutputs(int numClosestPoints, /* bool includeVerificationPoints, */
            bool printByComponents, bool printIndividualPointsComp,
            // PointCloud<PointLinkType, PointContainerType, PointType> points,
            // IDistanceComparer<PointContainerType, PointType> comparerInput
            PointContainerType[] points,
            IDistanceComparer<PointContainerType, PointType> comparerInput,
            List<PointContainerType> trainingPoints
            )
        {

            // TODO: clean this method!

            Console.WriteLine();
            Console.WriteLine("Checking closest trining data points for the specified set of training data points...");
            if (numClosestPoints < 1)
                throw new ArgumentException("Number of closest points to be written for each chosen points should be at least 1.");
            if (points == null)
                throw new ArgumentException("No points to be checked are specified.");
            int numPoints = points.Length;
            if (numPoints < 1)
                throw new ArgumentException("The number of specified points to be checked is less than 1.");
            
            //NeuralTrainingSet.ComparerInputDistance comparerInput = new NeuralTrainingSet.ComparerInputDistance
            //    (points[0].InputParameters, InputDistance);
            //NeuralTrainingSet.ComparerOutputDistance comparerOutput = new NeuralTrainingSet.ComparerOutputDistance
            //    (points[0].OutputValues, OutputDistance);


            List<PointContainerType> trainingElements = trainingPoints.GetRange(0, trainingPoints.Count);

            int NumNeuralParameters = trainingElements[0].GetPointCoordinates().Length;
            int NumNeuralOutputs = trainingElements[0].GetPointOutputVector().Length;

            if (trainingElements.Count < numClosestPoints + 1)
                numClosestPoints = trainingElements.Count - 1;
            StopWatch1 t = new StopWatch1();

            IVector currentRelative = new Vector(NumNeuralParameters);       // current point in relative coordinates
            IVector closestRelative = new Vector(NumNeuralParameters);  // i-th closest point in relative coordinates
            IVector differenceRelative = new Vector(NumNeuralParameters);    // difference of the above
            // Vectors of output values in relative coordinates:
            IVector currentOutRelative = new Vector(NumNeuralParameters);       // current point in relative coordinates
            IVector closestOutRelative = new Vector(NumNeuralParameters);  // i-th closest point in relative coordinates
            IVector differenceOutRelative = new Vector(NumNeuralParameters);    // difference of the above

            IVector currentPoint = null;
            IVector currentOutput = null;
            double[] minDistances = new double[numClosestPoints], maxDistances = new double[numClosestPoints],
                averageDistances = new double[numClosestPoints];
            double[] minOutDistances = new double[numClosestPoints], maxOutDistances = new double[numClosestPoints],
                averageOutDistances = new double[numClosestPoints];
            IVector[] minAbsDif = null, maxAbsDif = null, averageAbsDif = null;
            IVector[] minAbsOutDif = null, maxAbsOutDif = null, averageAbsOutDif = null;
            for (int i = 0; i < numClosestPoints; ++i)
            {
                minDistances[i] = double.MaxValue;
                maxDistances[i] = double.MinValue;
                averageDistances[i] = 0.0;
                minOutDistances[i] = double.MaxValue;
                maxOutDistances[i] = double.MinValue;
                averageOutDistances[i] = 0.0;
            }
            if (printByComponents)
            {
                minAbsDif = new IVector[numClosestPoints];
                maxAbsDif = new IVector[numClosestPoints];
                averageAbsDif = new IVector[numClosestPoints];
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    minAbsDif[i] = new Vector(NumNeuralParameters);
                    maxAbsDif[i] = new Vector(NumNeuralParameters);
                    averageAbsDif[i] = new Vector(NumNeuralParameters);
                    for (int k = 0; k < NumNeuralParameters; ++k)
                    {
                        minAbsDif[i][k] = double.MaxValue;
                        maxAbsDif[i][k] = double.MinValue;
                        averageAbsDif[i][k] = 0;
                    }
                }
                minAbsOutDif = new IVector[numClosestPoints];
                maxAbsOutDif = new IVector[numClosestPoints];
                averageAbsOutDif = new IVector[numClosestPoints];
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    minAbsOutDif[i] = new Vector(NumNeuralOutputs);
                    maxAbsOutDif[i] = new Vector(NumNeuralOutputs);
                    averageAbsOutDif[i] = new Vector(NumNeuralOutputs);
                    for (int k = 0; k < NumNeuralOutputs; ++k)
                    {
                        minAbsOutDif[i][k] = double.MaxValue;
                        maxAbsOutDif[i][k] = double.MinValue;
                        averageAbsOutDif[i][k] = 0;
                    }
                }
            }
            for (int iPoint = 0; iPoint < numPoints; ++iPoint)
            {
                //NeuralTrainingElement currentElement = points[iPoint];
                PointContainerType currentElement = points[iPoint];
                currentPoint = currentElement.GetPointCoordinates(); // InputParameters;
                currentOutput = points[iPoint].GetPointOutputVector(); //  OutputValues;
                comparerInput.GetRelativeCoordinates(currentPoint, ref currentRelative);
                comparerInput.GetOutputRelativeCoordinates(currentOutput, ref currentOutRelative);
                t.Start();
                // Sort training elemets according to their distance to the current point:
                comparerInput.ReferencePoint = currentElement; // currentPoint;
                trainingElements.Sort(comparerInput);
                t.Stop();
                Console.WriteLine();
                Console.WriteLine("Point No. " + iPoint + " (element index " + currentElement.Index
                    + ", calculated in " + t.Time.ToString("0.#####") + " s):");
                Console.WriteLine("Coordinates: ");
                Console.WriteLine("  " + currentPoint.ToString("#.####e0"));
                Console.WriteLine("  Relative coordinates: ");
                Console.WriteLine("  " + currentRelative.ToString("0.####"));
                if (printByComponents)
                {
                    if (printIndividualPointsComp)
                    {
                        Console.WriteLine();
                        Console.WriteLine("The first " + numClosestPoints + " closest points from the training set with ");
                        Console.WriteLine("differences (component-wise) expressed in relative coordinates:");
                    }
                    for (int i = 0; i < numClosestPoints; ++i)
                    {
                        //NeuralTrainingElement element;
                        PointContainerType element;
                        if (trainingElements[0] == currentElement)
                            element = trainingElements[i + 1];  // the first element must be skipped because it is the same as reference element
                        else
                            element = trainingElements[i];
                        //double inputDistance = comparerInput.Distance(element, currentPoint);
                        // GetNeuralInputRelative(element.InputParameters, ref closestRelative);
                        comparerInput.GetRelativeCoordinates(element.GetPointCoordinates(), ref closestRelative);
                        Vector.Subtract(closestRelative, currentRelative, ref differenceRelative);
                        // Vector of differences in relative coordinates for output vector:
                        // GetNeuralOutputRelative(element.OutputValues, ref closestOutRelative);
                        comparerInput.GetOutputRelativeCoordinates(element.GetPointOutputVector(), ref closestOutRelative);
                        Vector.Subtract(closestOutRelative, currentOutRelative, ref differenceOutRelative);
                        if (printIndividualPointsComp)
                        {
                            Console.WriteLine("{0,4}.: {1}", i, differenceRelative.ToString("0.####"));
                            Console.WriteLine("{0,10} {1}", "out:", differenceOutRelative.ToString("0.####"));
                        }
                        // Calculate statistics on indivitual components, for i-th closest point:
                        for (int k = 0; k < differenceRelative.Length; ++k)
                        {
                            double dif = Math.Abs(differenceRelative[k]);  // here we take absolute values of difference components
                            differenceRelative[k] = dif;
                            if (dif < minAbsDif[i][k])
                                minAbsDif[i][k] = dif;
                            if (dif > maxAbsDif[i][k])
                                maxAbsDif[i][k] = dif;
                            averageAbsDif[i][k] += dif / (double)numPoints;
                        }
                        // Calculate statistics on indivitual components, for i-th closest point, for OUTPUT:
                        for (int k = 0; k < differenceOutRelative.Length; ++k)
                        {
                            double dif = Math.Abs(differenceOutRelative[k]);
                            differenceOutRelative[k] = dif;
                            if (dif < minAbsOutDif[i][k])
                                minAbsOutDif[i][k] = dif;
                            if (dif > maxAbsOutDif[i][k])
                                maxAbsOutDif[i][k] = dif;
                            averageAbsOutDif[i][k] += dif / (double)numPoints;
                        }
                    }  // work over specified number of closest points to the current specified point
                }
                Console.WriteLine("The first " + numClosestPoints + " closest points from the training set with disances: ");
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    //NeuralTrainingElement element;
                    PointContainerType element;
                    if (trainingElements[0] == currentElement)
                        element = trainingElements[i + 1];  // the first element must be skipped because it is the same as reference element
                    else
                        element = trainingElements[i];
                    //double inputDistance = comparerInput.Distance(element, currentPoint);
                    //double outputDistance = comparerOutput.Distance(element, currentOutput);
                    double inputDistance = comparerInput.Distance(element, currentElement);
                    double outputDistance = comparerInput.OutputDistance(element, currentElement);
                    
                    Console.WriteLine("{0,4}: d = {1,-10:0.0000e0}, out. d = {2,-8:0.0000e0}, el. index: {3,-4}",
                        i, inputDistance, outputDistance, element.Index);
                    if (inputDistance < minDistances[i])
                        minDistances[i] = inputDistance;
                    if (inputDistance > maxDistances[i])
                        maxDistances[i] = inputDistance;
                    averageDistances[i] += inputDistance / (double)numPoints;
                    if (outputDistance < minOutDistances[i])
                        minOutDistances[i] = outputDistance;
                    if (outputDistance > maxOutDistances[i])
                        maxOutDistances[i] = outputDistance;
                    averageOutDistances[i] += outputDistance / (double)numPoints;
                }  // work over specified number of closest points to the current specified point
            }  // iterate over specified points
            if (printByComponents)
            {
                Console.WriteLine();
                Console.WriteLine("Statistics on closest points absolute differences in componets (relative coordinates):");
                for (int i = 0; i < numClosestPoints; ++i)
                {
                    string formatString = "0.00e0";
                    Console.WriteLine("*{0} -th closest point: ", i);
                    Console.WriteLine("  Minimal absolute difference of relative components:");
                    Console.WriteLine("    " + minAbsDif[i].ToString(formatString));
                    Console.WriteLine("  Maximal absolute difference of relative components:");
                    Console.WriteLine("    " + maxAbsDif[i].ToString(formatString));
                    Console.WriteLine("  Average absolute difference of relative components:");
                    Console.WriteLine("    " + averageAbsDif[i].ToString(formatString));
                    Console.WriteLine("  OUTPUT:");
                    Console.WriteLine("  Minimal absolute difference of relative components:");
                    Console.WriteLine("    " + minAbsOutDif[i].ToString(formatString));
                    Console.WriteLine("  Maximal absolute difference of relative components:");
                    Console.WriteLine("    " + maxAbsOutDif[i].ToString(formatString));
                    Console.WriteLine("  Average absolute difference of relative components:");
                    Console.WriteLine("    " + averageAbsOutDif[i].ToString(formatString));
                }  // work over specified number of closest points to the current specified point
            }
            Console.WriteLine();
            Console.WriteLine("Statistics on closest points distances: ");
            Console.WriteLine("{0,10} {1,-10:0.0000E0}   {2,10:0.0000E0}    {3,-10:0.0000E0}",
                "No. ", "min. d", "max. d", "average");
            for (int i = 0; i < numClosestPoints; ++i)
            {
                Console.WriteLine("{0,8}.: {1,-10:0.0000E0} - {2,10:0.0000E0};   {3,-10:0.0000E0}",
                    i, minDistances[i], maxDistances[i], averageDistances[i]);
            }
            Console.WriteLine("Statistics on closest points OUTPUT distances: ");
            Console.WriteLine("{0,10} {1,-10:0.0000E0}   {2,10:0.0000E0}    {3,-10:0.0000E0}",
                "No. ", "min. d", "max. d", "average");
            for (int i = 0; i < numClosestPoints; ++i)
            {
                Console.WriteLine("{0,8}.: {1,-10:0.0000E0} - {2,10:0.0000E0};   {3,-10:0.0000E0}",
                    i, minOutDistances[i], maxOutDistances[i], averageOutDistances[i]);
            }
            Console.WriteLine();
            Console.WriteLine("Total time for all calculations ({0} test points, {1} closest points): {2:0.####} s",
                numPoints, numClosestPoints, t.TotalTime);
            Console.WriteLine();
        }  // TestClosestPoints(...)




        #endregion PrintClosestPointsWithOutputVectors


        #endregion UtilitiesClosestPoints


        #region Algorithms


        /// <summary>Finds the specified number fo closest points to each point contained in the current cloud, and updates
        /// the neighbor lists of points such that they contain links to these neighbors.</summary>
        /// <param name="comparer">Comparer used to compare two points by distance to a reference point.
        /// <para>Used for sorting a list of all points according to their distance with the currently processed point.</para></param>
        /// <param name="numClosestPoints">Number of closest points to be identified for each point.</param>
        /// <remarks>This method is a dummy approach, it is slow and has time efficiency of N^2*log2(N). However,
        /// the method finds exact closest points for all points in the cloud.</remarks>
        public void FindClosestPointsDummy(IDistanceComparer<PointContainerType, PointType> comparer,
            int numClosestPoints)
        {
            FindClosestPointsDummy(comparer, numClosestPoints, true /* storeDistancesForSorting */);
        }


        /// <summary>Finds the specified number fo closest points to each point contained in the current cloud, and updates
        /// the neighbor lists of points such that they contain links to these neighbors.</summary>
        /// <param name="comparer">Comparer used to compare two points by distance to a reference point.
        /// <para>Used for sorting a list of all points according to their distance with the currently processed point.</para></param>
        /// <param name="numClosestPoints">Number of closest points to be identified for each point.</param>
        /// <param name="accelerateSortingByStoringDistances">If true then quicker sorting approach that stores the distances 
        /// to the reference point and compares points according to these values, is used.</param>
        /// <remarks>This method is a dummy approach, it is slow and has time efficiency of N^2*log2(N). However,
        /// the method finds exact closest points for all points in the cloud.</remarks>
        public void FindClosestPointsDummy(IDistanceComparer<PointContainerType, PointType> comparer,
            int numClosestPoints, bool accelerateSortingByStoringDistances)
        {
            lock (Lock)
            {
                List<PointContainerType> sorted = CreatePointsCopy();
                int numPoints = this.NumPoints;
                if(numClosestPoints > numPoints - 1)
                    throw new ArgumentException("Number of closest points to find (" + numPoints + ") is larger than numbe of points in the cloud minus 1.");
                for (int i = 0; i < numPoints; ++i)
                {
                    // For each point from the curretn point cloud, sort a copy of point list according to the
                    // distance with the current point, and obtain closest neighbors directly from sorted list:
                    PointContainerType currentPoint = this._points[i];
                    lock (currentPoint.Lock)
                    {
                        // Define reference point for comparison of points according do distances to this point: 
                        comparer.ReferencePoint = currentPoint;
                        // Sort point according to distance to the reference point:
                        if (accelerateSortingByStoringDistances) 
                        {

                            foreach (PointContainerType pt in sorted)
                                pt.StoredDistance = comparer.Distance(pt);  // calculate and store distance to reference point
                            sorted.Sort(comparer.StoredDistanceComparer);  // sort according to the stored distance
                            // comparer.SortUsingStoredDistance(sorted);  // alternative way
                        }
                        else  
                        {
                            comparer.Sort(sorted);
                            // alternative way sorted.Sort(comparer);
                        }
                        List<PointLinkType> neighbors = currentPoint.Neighbors;
                        // Copy the first closest points from the sorted list to the list of neighbors of the current point:
                        for (int k = 0; k < numClosestPoints; ++k)
                        {
                            if (neighbors.Count < k + 1)
                                neighbors.Add(currentPoint.CreateLink(sorted[k + 1]));
                            else
                                neighbors[k]._point = sorted[k + 1];
                            // Also set the distance form the neighbor point to its master point on the link:
                            neighbors[k].DistanceFromPoint = comparer.Distance(currentPoint, neighbors[k].Point);
                        }
                        int count = neighbors.Count;
                        if (count > numClosestPoints)
                        {
                            neighbors.RemoveRange(numClosestPoints, count - numClosestPoints);
                        }
                    }
                }
            }
        } // FindClosestPointListDummy(...)


        /// <summary>Finds closest points by initializing with random neighbors and then gradually 
        /// improving connections to neighbors.</summary>
        /// <param name="comparer">Distance comparer used to sort points according to the distance 
        /// to a reference point.</param>
        /// <param name="numClosestPoints">Number of closest points to detect.</param>
        public void FindClosestPointsGraph(IDistanceComparer<PointContainerType, PointType> comparer,
            int numClosestPoints)
        {
            int numPoints = this.NumPoints;
            // Praparation stage, prepare lists of neighbors by random selection of points:
            for (int i = 0; i < NumPoints; ++i)
            {
                PointContainerType point = GetPoint(i);
                List<PointLinkType> neighborList = point.Neighbors;
                Util.ResizeList<PointLinkType>(ref neighborList, numClosestPoints, null);

                
            
            }

        }



        #endregion Algorithms


        #region Examples
        
        /// <summary>Creates rabndomly an example cloud of points, calculates the specified number of closest neighbors 
        /// by the dummy method for each point in an example cloud, and prints statistics.
        /// <para>Sorting is accellerated by storing distances to reference points and using them in comparison.</para></summary>
        /// <param name="numPoints">Number of points in the created random cloud.</param>
        /// <param name="spaceDimension">Dimension of space in which points are embedded.</param>
        /// <param name="numClosestPoints">Number of closest points to each point.</param>
        /// <returns>The pooint cloud of vectors that is generated and for which nearest neighbors are found.</returns>
        public static PointCloudVector ExampleClosestPointsDummy
            (int numPoints, int spaceDimension, int numClosestPoints)
        {
            return ExampleClosestPointsDummy(numPoints, spaceDimension, numClosestPoints, true /* storeDistancesForSorting */);
        }

        /// <summary>Creates randomly an example cloud of points, calculates the specified number of closest neighbors 
        /// by the dummy method for each point in an example cloud, and prints statistics.</summary>
        /// <param name="numPoints">Number of points in the created random cloud.</param>
        /// <param name="spaceDimension">Dimension of space in which points are embedded.</param>
        /// <param name="numClosestPoints">Number of closest points to each point.</param>
        /// <param name="accelerateSortingByStoringDistances">If true then sorting is accelerated by first calculating and storing 
        /// distances on point containers and used the pre-calculated distances in comparison.</param>
        public static PointCloudVector ExampleClosestPointsDummy
            (int numPoints, int spaceDimension, int numClosestPoints, bool accelerateSortingByStoringDistances)
        {
            Console.WriteLine();
            Console.WriteLine("Example: dummy closest point generation.");
            Console.WriteLine("  Number of points: " + numPoints);
            Console.WriteLine("  Space dimension: " + spaceDimension);
            Console.WriteLine("  Number of closest points to identify: " + numClosestPoints);
            Console.WriteLine();
            Console.WriteLine("Calculating...");
            Console.WriteLine();
            PointCloudVector pointCloud = new PointCloudVector();
            pointCloud.GenerateRandomCloud(numPoints, spaceDimension);

            StopWatch1 t = new StopWatch1();
            t.Start();
            //DistanceComparer<PointContainerVector, IVector> comparer = new DistanceComparer<PointContainerVector, IVector>(
            //    new PointContainerVector(new Vector(spaceDimension)), Vector.Distance);

            IDistanceComparer<PointContainerVector, IVector> comparer =
                new DistanceComparerVector(new PointContainerVector(new Vector(spaceDimension)), Vector.Distance);
            pointCloud.FindClosestPointsDummy(comparer, numClosestPoints, accelerateSortingByStoringDistances);
            t.Stop();
            Console.WriteLine("... closest points calculated in " + t.Time + "s");
            Console.WriteLine();

            //// Print a couple of point coordinates
            //int numPrinted = 15;
            //int numPrintedNeighbors = 5;
            //if (numPrinted > pointCloud.NumPoints)
            //    numPrinted = pointCloud.NumPoints;
            //Console.WriteLine();
            //Console.WriteLine("Coordinates of the first " + numPrinted + " points: ");
            //string formatString = "#.####e0";
            //for (int i = 0; i < numPrinted && i < pointCloud.NumPoints; ++i)
            //{
            //    Console.WriteLine("  " + i + ": " + pointCloud.Points[i].GetPointCoordinates().ToString(formatString));
            //}
            //Console.WriteLine();
            //Console.WriteLine("Distances of the first " + numPrintedNeighbors + " neighbors for the first " + numPrinted + " points:");
            //for (int i = 0; i < numPrinted; ++i)
            //{
            //    Console.WriteLine("Point " + i + ", neighbor distances:");
            //    Console.Write("  ");
            //    // Iterate over points in the cloud:
            //    PointContainerVector point = pointCloud.GetPoint(i);
            //    List<PointLink<PointContainerVector, IVector>> neighbors = point.Neighbors;
            //    int numNeighbors = neighbors.Count;
            //    for (int k = 0; k < numNeighbors && k < numPrintedNeighbors; ++k)
            //    {
            //        PointLink<PointContainerVector, IVector> link = neighbors[k];
            //        if (link != null)
            //        {
            //            PointContainerVector neighbor = link.Point;
            //            if (neighbor != null)
            //            {
            //                // the k-th neighbor of i-th point is defined, therefore we use its recorded
            //                // distance to the point in statistics:
            //                double distance = link.DistanceFromPoint;
            //                Console.Write("{0}:{1:#.###e0} ", k, distance);
            //            }
            //        }
            //    }  // over all neighbors of the specific point
            //    Console.WriteLine();
            //}
            //Console.WriteLine();

            Console.WriteLine("Results:");
            pointCloud.PrintNeighborDistanceStatistics(numClosestPoints, true, comparer);

            return pointCloud;
        }

        #endregion Examples


    }  // class PointCloud<PointType, PointContainerType>


}