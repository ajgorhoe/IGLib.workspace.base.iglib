// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;
using IG.Num;

using Kitware.VTK;

using Color = System.Drawing.Color; // short name access to color without usig the whole namespace

namespace IG.Gr3d
{

    /// <summary>Base class for plotting classes that plot on VTK windows (class VtkPlotter).
    /// <para>Classes derived from this class produce pecific plots (such as surface or line plots)
    /// on the related VTK rendering window.</para>
    /// <para>Interaction with a VTK rendering window is managed by an <see cref="VtkPlotter"/></para> object.</summary>
    /// $A Igor xx Nov11;
    public abstract class VtkPlotBase: ILockable, IDisposable
    {

        #region Construction

        /// <summary>Prevent calling argument-less constructor in derived classes.</summary>
        private VtkPlotBase() 
        { }

        /// <summary>Constructor.</summary>
        /// <param name="plotter">VTK plotter that is used for rendering graphics produeced by the 
        /// current plotting class.</param>
        public VtkPlotBase(VtkPlotter plotter)
        {
            if (plotter == null)
                throw new ArgumentNullException("plotter", "The VTK plotter connected to the created plotting object is not specified.");
            this.Plotter = plotter;
            plotter.AddPlotObject(this);
        }

        #endregion Construction


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking


        #region Settings

        private int _outputLevel = VtkPlotter.DefaultOutputLevel;

        /// <summary>Level of output to the console for the current object.
        /// The defalult output level for newly created object is specified by <see cref="VtkPlotter"/>.<see cref="DefaultOutputLevel"/>.</summary>
        public int OutputLevel
        { get { return _outputLevel; } set { _outputLevel = value; } }

        private StopWatch _timer;

        /// <summary>Stopwatch that can be used to measure the time efficiency of actions.</summary>
        public StopWatch Timer
        { get { if (_timer == null) _timer = new StopWatch(); return _timer; } }


        /// <summary>Sets background color of the plotter that is used by the current plot object.
        /// <para>Task is delegated to the plotter.</para></summary>
        public color BackGround
        {
            get { return Plotter.BackGround; }
            set { Plotter.BackGround = value; }
        }

        #endregion Settings


        #region Data

        VtkPlotter _plotter;

        /// <summary>VTK plotter that is used for rendering graphics produeced by the 
        /// current plotting class, on a VTK rendering window.
        /// <para>Getter is not thread safe (for better efficiency).</para></summary>
        public VtkPlotter Plotter
        {
            get {
                return _plotter;
            }
            set
            {
                lock (Lock)
                {
                    if (value == null)
                        throw new InvalidOperationException("Invalid attempt to set VTK plotter object to null.");
                    _plotter = value;
                }
            }
        }

        //public BoundingBox3d _scaledBounds;

        //public BoundingBox3d ScaledBounds
        //{
        //    get
        //    {
        //        if (_scaledBounds == null)
        //        {
        //            lock (Lock)
        //            {
        //                if (_scaledBounds == null)
        //                    ScaledBounds = new BoundingBox3d();
        //            }
        //        }
        //        return _scaledBounds;
        //    }
        //    set { lock (Lock) { _scaledBounds = value; } }
        //}


        //public void AddScaledBounds(BoundingBox3d scaledBounds)
        //{
        //    if (scaledBounds == null)
        //        throw new ArgumentNullException("scaledBounds not specified (null reference).");
        //    lock (Lock)
        //    {
        //        ScaledBounds.Add(scaledBounds);
        //        //if (!ScaledBounds.Contains(scaledBounds))
        //        //{
        //        //    ScaledBounds.Add(scaledBounds);
        //        //}
        //    }
        //}

        #region Actors

        protected List<vtkActor> _actors;

        /// <summary>List of actors contained on the current class.
        /// <para>Setter is thread safe.</para>
        /// <para>Lazy evaluation - list object is automatically generated when first accessed.</para></summary>
        protected List<vtkActor> Actors
        {
            get
            {
                if (_actors == null)
                {
                    lock (Lock)
                    {
                        if (_actors == null)
                            Actors = new List<vtkActor>();
                    }
                }
                return _actors;
            }
            set { lock (Lock) { _actors = value; } }
        }


        /// <summary>Returns true if the specified VTK Actor is contained on (registered with) the current 
        /// <see cref="VtkPlotBase"/> object, or false otherwise.</summary>
        /// <param name="actor">Actor whose existent on the list is checked for.</param>
        /// <returns></returns>
        public bool ContainsActor(vtkActor actor)
        {
            if (actor == null)
                throw new ArgumentNullException("actor", "VTK actor not specified (null reference).");
            lock (Lock)
            {
                return Actors.Contains(actor);
            }
        }

        /// <summary>Adds the specified actor to the list of actors of the current
        /// VTK plotter.
        /// <para>If the object is already on the list of actors then it is not inserted again.</para></summary>
        /// <param name="actor">VTK Actor to be added on the currrent <see cref="VtkPlotter"/> object.</param>
        public void AddActor(vtkActor actor)
        {
            if (actor == null)
                throw new ArgumentNullException("actor", "VTK actor not specified (null reference).");
            lock (Lock)
            {
                Actors.Add(actor);
                Plotter.AddActor(actor);
                //if (!Actors.Contains(actor))
                //{
                //    Actors.Add(actor);
                //    Plotter.AddActor(actor);
                //}
            }
        }

        /// <summary>Adds the specified actors to the list of actors of the current
        /// VTK plotter.</summary>
        /// <param name="actors">Objects to be added to the list.</param>
        public void AddActors(params vtkActor[] actors)
        {
            if (actors != null)
                lock (Lock)
                {
                    for (int i = 0; i < actors.Length; ++i)
                    {
                        AddActor(actors[i]);
                    }
                }
        }

        /// <summary>Adds all actors from the current plot to the plotter that this plot is assigned to.</summary>
        public void AddActorsToPlotter()
        {
            lock (Lock)
            {
                if (this.Plotter != null)
                {
                    AddActorsToPlotter(this.Plotter);
                }
            }
        }

        /// <summary>Adds all actors from the current plot to the specified plotter.</summary>
        /// <param name="plotter">Plotter to which actors from the current plot are added.</param>
        public void AddActorsToPlotter(VtkPlotter plotter)
        {
            if (plotter == null)
                throw new ArgumentException("Plotter to add actors to is not specified (null reference).");
            lock(Lock)
            {
                if (Actors!=null)
                {
                    int numActors = Actors.Count;
                    for (int i = 0; i < numActors; ++i)
                    {
                        Plotter.AddActor(Actors[i]);
                    }
                }
            }
        }

        /// <summary>Removes the specified actor from the list of actors of the current
        /// VTK plotter, and disposes unmanaged resources used by that object.
        /// <para>If the specified object is not on the list of actors then nothing happens.</para></summary>
        /// <param name="actor">VTK Actor to be removed from the currrent <see cref="VtkPlotter"/> object.</param>
        public void RemoveActor(vtkActor actor)
        {
            if (actor == null)
                throw new ArgumentNullException("actor", "VTK actor not specified (null reference).");
            lock (Lock)
            {
                try
                {
                    if (Actors.Contains(actor))
                    {
                        Actors.Remove(actor);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    actor.Dispose();
                }
            }
        }

        /// <summary>Removes the specified actors from the list of actors of the current
        /// VTK plotter, and disposes unmanaged resources used by that objects.
        /// <para>If no objects are specified then nothing happens. Also for the specified objects that are null
        /// or are not on the list, nothing happens. If removing one of the objects throws an exception then the 
        /// remaining objects are removed without any disturbance.</para></summary>
        /// <param name="actors">Objects to be removed from the list.</param>
        public void RemoveActors(params vtkActor[] actors)
        {
            int numErrors = 0;
            if (actors != null)
            {
                lock (Lock)
                {
                    for (int i = 0; i < actors.Length; ++i)
                    {
                        try
                        {
                            RemoveActor(actors[i]);
                        }
                        catch (Exception)
                        {
                            ++numErrors;
                        }
                    }
                }
            }
            if (numErrors > 0)
                throw new Exception("The following number of exceptions occurred when removing actors: " + numErrors + ".");
        }

        /// <summary>Removes all actors that are currently contained on the list of actors of the current object.</summary>
        public void RemoveActors()
        {
            lock (Lock)
            {

                // TODO: This does not work, correct!

                RemoveActors(Actors.ToArray());
            }
        }

        /// <summary>Updates the specified bounding box in such a way that all actors from the current plot object
        /// fit into it.</summary>
        /// <param name="bounds">Bounds to be updated.</param>
        public void UpdateBoundsOnActors(IBoundingBox bounds)
        {
            lock (Lock)
            {
                foreach (vtkActor actor in Actors)
                {
                    if (actor != null)
                    {
                        double[] actorBounds = actor.GetBounds();
                        bounds.Update((int)0, actorBounds[0], actorBounds[1]);
                        bounds.Update((int)1, actorBounds[2], actorBounds[3]);
                        bounds.Update((int)2, actorBounds[4], actorBounds[5]);

                    }
                }
            }
        }

        /// <summary>Scales all actors on the current plot by the plotter's scaling method.</summary>
        public void ScaleActors()
        {
            lock (Lock)
            {
                VtkPlotter plotter = this.Plotter;
                if (plotter != null)
                {
                    foreach (vtkActor actor in Actors)
                    {
                        if (actor != null)
                        {
                            plotter.ScaleActorPlain(actor);
                        }
                    }
                }
            }
        }

        #endregion Actors


        #region Algorithms_Filters

        protected List<vtkAlgorithm> _algorithms;

        /// <summary>List of algorithms contained on the current class.
        /// <para>Setter is thread safe.</para>
        /// <para>Lazy evaluation - list object is automatically generated when first accessed.</para></summary>
        protected List<vtkAlgorithm> algorithms
        {
            get
            {
                if (_algorithms == null)
                {
                    lock (Lock)
                    {
                        if (_algorithms == null)
                            algorithms = new List<vtkAlgorithm>();
                    }
                }
                return _algorithms;
            }
            set { lock (Lock) { _algorithms = value; } }
        }

        /// <summary>Returns true if the specified VTK algorithm is contained on (registered with) the current 
        /// <see cref="VtkPlotter"/> object, or false otherwise.</summary>
        /// <param name="algorithm">object to be checked.</param>
        public bool ContainsAlgorithm(vtkAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm", "VTK algorithm not specified (null reference).");
            lock (Lock)
            {
                return algorithms.Contains(algorithm);
            }
        }

        /// <summary>Adds the specified algorithm to the list of algorithms of the current
        /// VTK plotter.
        /// <para>If the object is already on the list of algorithms then it is not inserted again.</para></summary>
        /// <param name="algorithm">VTK algorithm to be added on the currrent <see cref="VtkPlotter"/> object.</param>
        public void AddAlgorithm(vtkAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm", "VTK algorithm not specified (null reference).");
            lock (Lock)
            {
                if (!algorithms.Contains(algorithm))
                {
                    algorithms.Add(algorithm);
                }
            }
        }

        /// <summary>Adds the specified algorithms to the list of algorithms of the current
        /// VTK plotter.</summary>
        /// <param name="algorithms">Objects to be added to the list.</param>
        public void AddAlgorithms(params vtkAlgorithm[] algorithms)
        {
            if (algorithms != null)
                lock (Lock)
                {
                    for (int i = 0; i < algorithms.Length; ++i)
                    {
                        AddAlgorithm(algorithms[i]);
                    }
                }
        }

        /// <summary>Removes the specified algorithm from the list of algorithms of the current
        /// VTK plotter, and disposes unmanaged resources used by that object.
        /// <para>If the specified object is not on the list of algorithms then nothing happens.</para></summary>
        /// <param name="algorithm">VTK algorithm to be removed from the currrent <see cref="VtkPlotter"/> object.</param>
        public void RemoveAlgorithm(vtkAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm", "VTK algorithm not specified (null reference).");
            lock (Lock)
            {
                try
                {
                    if (algorithms.Contains(algorithm))
                    {
                        algorithms.Remove(algorithm);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    algorithm.Dispose();
                }
            }
        }

        /// <summary>Removes the specified algorithms from the list of algorithms of the current
        /// VTK plotter, and disposes unmanaged resources used by that objects.
        /// <para>If no objects are specified then nothing happens. Also for the specified objects that are null
        /// or are not on the list, nothing happens. If removing one of the objects throws an exception then the 
        /// remaining objects are removed without any disturbance.</para></summary>
        /// <param name="algorithms">Objects to be removed from the list.</param>
        public void RemoveAlgorithms(params vtkAlgorithm[] algorithms)
        {
            int numErrors = 0;
            if (algorithms != null)
            {
                lock (Lock)
                {
                    for (int i = 0; i < algorithms.Length; ++i)
                    {
                        try
                        {
                            RemoveAlgorithm(algorithms[i]);
                        }
                        catch (Exception)
                        {
                            ++numErrors;
                        }
                    }
                }
            }
            if (numErrors > 0)
                throw new Exception("The following number of exceptions occurred when removing algorithms: " + numErrors + ".");
        }


        #endregion Algorithms_Filters


        #region Mappers

        protected List<vtkMapper> _mappers;

        /// <summary>List of mappers contained on the current class.
        /// <para>Setter is thread safe.</para>
        /// <para>Lazy evaluation - list object is automatically generated when first accessed.</para></summary>
        protected List<vtkMapper> Mappers
        {
            get
            {
                if (_mappers == null)
                {
                    lock (Lock)
                    {
                        if (_mappers == null)
                            Mappers = new List<vtkMapper>();
                    }
                }
                return _mappers;
            }
            set { lock (Lock) { _mappers = value; } }
        }

        /// <summary>Returns true if the specified VTK mapper is contained on (registered with) the current 
        /// <see cref="VtkPlotter"/> object, or false otherwise.</summary>
        /// <param name="mapper">Mapper to be checked.</param>
        public bool ContainsMapper(vtkMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper", "VTK mapper not specified (null reference).");
            lock (Lock)
            {
                return Mappers.Contains(mapper);
            }
        }

        /// <summary>Adds the specified mapper to the list of mappers of the current
        /// VTK plotter.
        /// <para>If the object is already on the list of mappers then it is not inserted again.</para></summary>
        /// <param name="mapper">VTK mapper to be added on the currrent <see cref="VtkPlotter"/> object.</param>
        public void AddMapper(vtkMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper", "VTK mapper not specified (null reference).");
            lock (Lock)
            {
                if (!Mappers.Contains(mapper))
                {
                    Mappers.Add(mapper);
                }
            }
        }

        /// <summary>Adds the specified mappers to the list of mappers of the current
        /// VTK plotter.</summary>
        /// <param name="mappers">Objects to be added to the list.</param>
        public void AddMappers(params vtkMapper[] mappers)
        {
            if (mappers != null)
                lock (Lock)
                {
                    for (int i = 0; i < mappers.Length; ++i)
                    {
                        AddMapper(mappers[i]);
                    }
                }
        }

        /// <summary>Removes the specified mapper from the list of mappers of the current
        /// VTK plotter, and disposes unmanaged resources used by that object.
        /// <para>If the specified object is not on the list of mappers then nothing happens.</para></summary>
        /// <param name="mapper">VTK mapper to be removed from the currrent <see cref="VtkPlotter"/> object.</param>
        public void RemoveMapper(vtkMapper mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException("mapper", "VTK mapper not specified (null reference).");
            lock (Lock)
            {
                try
                {
                    if (Mappers.Contains(mapper))
                    {
                        Mappers.Remove(mapper);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    mapper.Dispose();
                }
            }
        }

        /// <summary>Removes the specified mappers from the list of mappers of the current
        /// VTK plotter, and disposes unmanaged resources used by that objects.
        /// <para>If no objects are specified then nothing happens. Also for the specified objects that are null
        /// or are not on the list, nothing happens. If removing one of the objects throws an exception then the 
        /// remaining objects are removed without any disturbance.</para></summary>
        /// <param name="mappers">Objects to be removed from the list.</param>
        public void RemoveMappers(params vtkMapper[] mappers)
        {
            int numErrors = 0;
            if (mappers != null)
            {
                lock (Lock)
                {
                    for (int i = 0; i < mappers.Length; ++i)
                    {
                        try
                        {
                            RemoveMapper(mappers[i]);
                        }
                        catch (Exception)
                        {
                            ++numErrors;
                        }
                    }
                }
            }
            if (numErrors > 0)
                throw new Exception("The following number of exceptions occurred when removing mappers: " + numErrors + ".");
        }


        #endregion Mappers


        #region DataSets

        // DataSets include vtkPolyData and similar objects

        protected List<vtkDataSet> _dataSets;

        /// <summary>List of datasets contained on the current class.
        /// <para>Setter is thread safe.</para>
        /// <para>Lazy evaluation - list object is automatically generated when first accessed.</para></summary>
        protected List<vtkDataSet> DataSets
        {
            get
            {
                if (_dataSets == null)
                {
                    lock (Lock)
                    {
                        if (_dataSets == null)
                            DataSets = new List<vtkDataSet>();
                    }
                }
                return _dataSets;
            }
            set { lock (Lock) { _dataSets = value; } }
        }

        /// <summary>Returns true if the specified VTK dataset is contained on (registered with) the current 
        /// <see cref="VtkPlotter"/> object, or false otherwise.</summary>
        /// <param name="dataset">dataset to be checked.</param>
        public bool ContainsDataset(vtkDataSet dataset)
        {
            if (dataset == null)
                throw new ArgumentNullException("dataset", "VTK dataset not specified (null reference).");
            lock (Lock)
            {
                return DataSets.Contains(dataset);
            }
        }

        /// <summary>Adds the specified dataset to the list of datasets of the current
        /// VTK plotter.
        /// <para>If the object is already on the list of datasets then it is not inserted again.</para></summary>
        /// <param name="dataset">VTK dataset to be added on the currrent <see cref="VtkPlotter"/> object.</param>
        public void AddDataset(vtkDataSet dataset)
        {
            if (dataset == null)
                throw new ArgumentNullException("dataset", "VTK dataset not specified (null reference).");
            lock (Lock)
            {
                if (!DataSets.Contains(dataset))
                {
                    DataSets.Add(dataset);
                }
            }
        }

        /// <summary>Adds the specified datasets to the list of datasets of the current
        /// VTK plotter.</summary>
        /// <param name="datasets">Objects to be added to the list.</param>
        public void AddDatasets(params vtkDataSet[] datasets)
        {
            if (datasets != null)
                lock (Lock)
                {
                    for (int i = 0; i < datasets.Length; ++i)
                    {
                        AddDataset(datasets[i]);
                    }
                }
        }

        /// <summary>Removes the specified dataset from the list of datasets of the current
        /// VTK plotter, and disposes unmanaged resources used by that object.
        /// <para>If the specified object is not on the list of datasets then nothing happens.</para></summary>
        /// <param name="dataset">VTK dataset to be removed from the currrent <see cref="VtkPlotter"/> object.</param>
        public void RemoveDataset(vtkDataSet dataset)
        {
            if (dataset == null)
                throw new ArgumentNullException("dataset", "VTK dataset not specified (null reference).");
            lock (Lock)
            {
                try
                {
                    if (DataSets.Contains(dataset))
                    {
                        DataSets.Remove(dataset);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    dataset.Dispose();
                }
            }
        }

        /// <summary>Removes the specified datasets from the list of datasets of the current
        /// VTK plotter, and disposes unmanaged resources used by that objects.
        /// <para>If no objects are specified then nothing happens. Also for the specified objects that are null
        /// or are not on the list, nothing happens. If removing one of the objects throws an exception then the 
        /// remaining objects are removed without any disturbance.</para></summary>
        /// <param name="datasets">Objects to be removed from the list.</param>
        public void RemoveDatasets(params vtkDataSet[] datasets)
        {
            int numErrors = 0;
            if (datasets != null)
            {
                lock (Lock)
                {
                    for (int i = 0; i < datasets.Length; ++i)
                    {
                        try
                        {
                            RemoveDataset(datasets[i]);
                        }
                        catch (Exception)
                        {
                            ++numErrors;
                        }
                    }
                }
            }
            if (numErrors > 0)
                throw new Exception("The following number of exceptions occurred when removing datasets: " + numErrors + ".");
        }


        #endregion DataSets


        #endregion Data


        #region Operaton 
        
        // TODO: Add routines for automatic calculation of the bounds!

        protected bool _autoUpdateBoundsCoordinates = true;

        /// <summary>Determines whether bounds on plotted geometry are automatically updated 
        /// when new primitives are added.</summary>
        public bool AutoUpdateBoundsCoordinates
        {
            get { return _autoUpdateBoundsCoordinates; }
            set { _autoUpdateBoundsCoordinates = value; }
        }

        private BoundingBox3d _boundsCoordinates;

        /// <summary>Bounds on coordinates of points that define the plots.
        /// everything on the plot should fit in these bounds.</summary>
        public BoundingBox3d BoundsCoordinates
        {
            get
            {
                if (_boundsCoordinates == null)
                    _boundsCoordinates = new BoundingBox3d();
                return _boundsCoordinates;
            }
            protected set
            {
                _boundsCoordinates = value;
            }
        }

        /// <summary>Updates the bounding box of coordinates (<see cref="BoundsCoordinates"/>), if the conditions are met, in such a way
        /// that the point with the specified coordinates fits witin it.
        /// <para>Conditions are met if the <paramref name="forced"/> flag is true or the <see cref="AutoUpdateBoundsCoordinates"/> flag is true.</para></summary>
        /// <param name="forced">Flag for forced update. If true then update is performed even if the auto updating flg
        /// (<see cref="AutoUpdateBoundsCoordinates"/>) is false.</param>
        /// <param name="x">X coordinate of the point to fit within the bounding box.</param>
        /// <param name="y">Y coordinate of the point to fit within the bounding box.</param>
        /// <param name="z">Z coordinate of the point to fit within the bounding box.</param>
        public void UpdateBoundsCoordinates(bool forced, double x, double y, double z)
        {
            if (_autoUpdateBoundsCoordinates || forced)
            {
                BoundingBox3d bounds = _boundsCoordinates;
                if (bounds == null)
                    bounds = BoundsCoordinates;
                bounds.Update(x, y, z);
            }
        }

        /// <summary>Updates the bounding box of coordinates (<see cref="BoundsCoordinates"/>), if 
        /// the <see cref="AutoUpdateBoundsCoordinates"/> flag is true, in such a way that the point with 
        /// the specified coordinates fits witin it.</summary>
        /// (<see cref="AutoUpdateBoundsCoordinates"/>) is false.</param>
        /// <param name="x">X coordinate of the point to fit within the bounding box.</param>
        /// <param name="y">Y coordinate of the point to fit within the bounding box.</param>
        /// <param name="z">Z coordinate of the point to fit within the bounding box.</param>
        public void UpdateBoundsCoordinates(double x, double y, double z)
        {
            if (_autoUpdateBoundsCoordinates)
            {
                BoundingBox3d bounds = _boundsCoordinates;
                if (bounds == null)
                    bounds = BoundsCoordinates;
                bounds.Update(x, y, z);
            }
        }

        /// <summary>Updates the bounding box of coordinates (<see cref="BoundsCoordinates"/>), if the conditions are met, in such a way
        /// that the point with the specified coordinates fits witin it.
        /// <para>Conditions are met if the <paramref name="forced"/> flag is true or the <see cref="AutoUpdateBoundsCoordinates"/> flag is true.</para></summary>
        /// <param name="forced">Flag for forced update. If true then update is performed even if the auto updating flg
        /// (<see cref="AutoUpdateBoundsCoordinates"/>) is false.</param>
        /// <param name="pointCoordinates">Coordinates of the point to fit within the bounding box.</param>
        public void UpdateBoundsCoordinates(bool forced, vec3 pointCoordinates)
        {
            if (_autoUpdateBoundsCoordinates || forced)
            {
                BoundingBox3d bounds = _boundsCoordinates;
                if (bounds == null)
                    bounds = BoundsCoordinates;
                bounds.Update(pointCoordinates);
            }
        }

        /// <summary>Updates the bounding box of coordinates (<see cref="BoundsCoordinates"/>), if 
        /// the <see cref="AutoUpdateBoundsCoordinates"/> flag is true, in such a way that the point with 
        /// the specified coordinates fits witin it.</summary>
        /// (<see cref="AutoUpdateBoundsCoordinates"/>) is false.</param>
        /// <param name="pointCoordinates">Coordinates of the point to fit within the bounding box.</param>
        public void UpdateBoundsCoordinates(vec3 pointCoordinates)
        {
            if (_autoUpdateBoundsCoordinates)
            {
                BoundingBox3d bounds = _boundsCoordinates;
                if (bounds == null)
                    bounds = BoundsCoordinates;
                bounds.Update(pointCoordinates);
            }
        }


        
        /// <summary>Creates the plot.</summary>
        public abstract void Create();

        /// <summary>Makes the associated plotter show the plots that are currently added.
        /// <para>Warning: this does not create a plot.</para></summary>
        public virtual void ShowPlot()
        {
            Plotter.ShowPlot();
        }


        /// <summary>Creates the plot and makes the associated plotter show it.</summary>
        public virtual void CreateAndShow()
        {
            Create();
            ShowPlot();
        }

        #endregion Operation


        #region IDisposable


        ~VtkPlotBase()
        {
            Dispose(false);
        }


        private bool disposed = false;

        /// <summary>Implementation of IDisposable interface.</summary>
        public void Dispose()
        {
            lock(Lock)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>Does the job of freeing resources. 
        /// <para></para>This method can be  eventually overridden in derived classes (if they use other 
        /// resources that must be freed - in addition to such resources of the current class).
        /// In the case of overriding this method, you should usually call the base.<see cref="Dispose"/>(<paramref name="disposing"/>).
        /// in the overriding method.</para></summary>
        /// <param name="disposing">Tells whether the method has been called form Dispose() method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free unmanaged objects resources:

                // Set large objects to null:

                disposed = true;
            }
        }

        #endregion IDisposable


        #region AuxiliaryFunctions


        /// <summary>2D function that returns the first parameter. Used as the first componnet of parametric 
        /// definition of surfce that is defined by a single function of 2 variables.</summary>
        public class Func2dX : Func2dBaseNoHessian
        {
            public Func2dX()
                : base()
            { }

            public override double Value(double x, double y)
            { return x; }

            public override void Gradient(double x, double y, out double gradx, out double grady)
            { gradx = 1; grady = 0; }
        }

        /// <summary>2D function that returns the second parameter. Used as the second componnet of parametric 
        /// definition of surfce that is defined by a single function of 2 variables.</summary>
        public class Func2dY : Func2dBaseNoHessian
        {
            public Func2dY()
                : base()
            { }

            public override double Value(double x, double y)
            { return y; }

            public override void Gradient(double x, double y, out double gradx, out double grady)
            { gradx = 0; grady = 1; }
        }

        /// <summary>Auxiliary 2D function that returns 0.</summary>
        public class Func2dZero : Func2dBaseNoHessian
        {
            public Func2dZero()
                : base()
            { }

            public override double Value(double x, double y)
            { return 0; }

            public override void Gradient(double x, double y, out double gradx, out double grady)
            { gradx = 0; grady = 0; }
        }


        /// <summary>3D function that returns the third component of its 3D vector argument. Used e.g. for coloring graphs
        /// by heights.</summary>
        public class Func3dZ : Func3dBaseNoHessian
        {
            public Func3dZ()
                : base()
            { }

            public override double Value(double x, double y, double z)
            { return z; }

            public override void Gradient(double x, double y, double z, out double gradx, out double grady, out double gradz)
            { gradx = 0; grady = 0; gradz = 1; }
        }

        /// <summary>3D function that returns 0. Used e.g. for coloring graphs by heights.</summary>
        public class Func3dZero : Func3dBaseNoHessian
        {
            public Func3dZero()
                : base()
            { }

            public override double Value(double x, double y, double z)
            { return 0; }

            public override void Gradient(double x, double y, double z, out double gradx, out double grady, out double gradz)
            { gradx = 0; grady = 0; gradz = 0; }
        }

        /// <summary>Function f(x,y)=x*y.</summary>
        public class ExampleFunc2dXY : Func2dBaseNoHessian
        {
            public override double Value(double x, double y)
            {
                return x * y;
            }
            public override void Gradient(double x, double y, out double gradx, out double grady)
            {
                gradx = y; grady = x;
            }
        }

        /// <summary>Function f(x,y)=x+y.</summary>
        public class ExampleFunc2dLinear : Func2dBaseNoHessian
        {
            public override double Value(double x, double y)
            {
                return x + y;
            }
            public override void Gradient(double x, double y, out double gradx, out double grady)
            {
                gradx = 1; grady = 1;
            }
        }


        /// <summary>Function f(x,y)=x*x+y*y.</summary>
        public class ExampleFunc2dSquare : Func2dBaseNoHessian
        {
            public override double Value(double x, double y)
            {
                return x * x + y * y;
            }
            public override void Gradient(double x, double y, out double gradx, out double grady)
            {
                gradx = 2 * x; grady = 2 * y;
            }
        }

        #endregion AuxiliaryFunctions


        #region Examples

        #region ExampleCustomFunctionComparison

        /// <summary>First uxiliary function used in examples.</summary>
        private static IFunc2d func1 = new ExampleFunc2dShifted(new ExampleFunc2dSquare(),  0.5);
        
        /// <summary>Second uxiliary function used in examples.</summary>
        private static IFunc2d func2 = new ExampleFunc2dXY();

        /// <summary>Function defined as some other function shifted for the specified value.</summary>
        protected class ExampleFunc2dShifted : Func2dBaseNoGradient, IFunc2d
        {
            public ExampleFunc2dShifted(IFunc2d funcOriginal, double shift): base() 
            {
                if (funcOriginal == null)
                    throw new ArgumentException("Function not specified.");
                this.FuncOriginal = funcOriginal;
                this.Shift = shift;
            }

            public double Shift;

            public IFunc2d FuncOriginal;

            public override double Value(double x, double y)
            {
                return FuncOriginal.Value(x, y) - Shift;
            }

        }

        /// <summary>Difference between the two auxiliary functions.</summary>
        private class ExampleFuncDiff : Func2dBaseNoHessian
        {
            public override double Value(double x, double y)
            {
                return func2.Value(x, y) - func1.Value(x, y);
            }

            public override void Gradient(double x, double y, out double gradx, out double grady)
            {
                double dx1, dy1, dx2, dy2;
                func1.Gradient(x, y, out dx1, out dy1);
                func2.Gradient(x, y, out dx2, out dy2);
                gradx = dx2 - dx1;
                grady = dy2 - dy1;
            }
        }


        /// <summary>3D function of coordinates that returns difference between <see cref="func2"/> and <see cref="func1"/>
        /// at the first two coordinates.
        /// <para>Used for coloring surfaces and contours according to defference between two functions.</para></summary>
        public class ExampleValueFunctionDiff21 : Func3dBaseNoGradient
        {
            public ExampleValueFunctionDiff21()
                : base()
            { }

            public override double Value(double x, double y, double z)
            { 
                return Math.Abs(func2.Value(x, y) - func1.Value(x, y)); 
            }

        }


        /// <summary>Custom comparison of two surfaces in 3D defined as functions of two variables.
        /// <para>On the main surface, wireframe is shown, surface is transparent and its color varies according to 
        /// difference with the other surface.</para>
        /// <para>The second surface is a wireframe and is very transparent.</para></summary>
        public static void ExampleCustomSurfaceComparison()
        {
            double minX = -1, maxX = 1, minY = -1, maxY = 1;
            int numX = 30, numY = 30;

            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots

            bool leadingSurface = true;
            bool leadingContours = true;

            if (leadingSurface)
            {
                // Create surface plot of the first function graph:
                VtkSurfacePlot plot1Surf = new VtkSurfacePlot(plotter);
                plot1Surf.OutputLevel = 1;  // print to console what's going on
                // Create the first surface graph by the plot object; adjust the setting first:
                plot1Surf.SetSurfaceDefinition(func1);  // function of 2 variables that defines the surface
                plot1Surf.SetBoundsParameters(minX, maxX, minY, maxY);
                plot1Surf.NumX = numX; plot1Surf.NumY = numY;
                plot1Surf.PointsVisible = true; plot1Surf.PointSize = 4; plot1Surf.PointColor = Color.Orange;
                plot1Surf.SurfacesVisible = true; plot1Surf.SurfaceColorIsScaled = true;
                plot1Surf.ValueFunctionOfCoordinates = new ExampleValueFunctionDiff21(); // difference between two functions
                plot1Surf.SurfaceColor = Color.Green; plot1Surf.SurfaceColorOpacity = 1;
                plot1Surf.SurfaceColorIsScaled = true;
                plot1Surf.SurfaceColorScale = ColorScale.CreateRainbow(0, 1); plot1Surf.SurfaceColorOpacity = 0.4;

                plot1Surf.SurfaceColor = Color.Red;

                plot1Surf.LinesVisible = true; plot1Surf.LineWidth = 2; plot1Surf.LineColorIsScaled = false;
                plot1Surf.LineColor = Color.LightGray;
                plot1Surf.Create();
                
            }

            if (leadingContours)
            {
                // Create contour plot for the first function graph:
                VtkContourPlot plot1Cont = new VtkContourPlot(plotter);
                plot1Cont.OutputLevel = 1;  // print to console what's going on
                // Create the first surface graph by the plot object; adjust the setting first:
                plot1Cont.SetSurfaceDefinition(func1);  // function of 2 variables that defines the surface
                plot1Cont.SetBoundsParameters(minX, maxX, minY, maxY);
                plot1Cont.NumX = numX; plot1Cont.NumY = numY;
                plot1Cont.PointsVisible = true; plot1Cont.PointSize = 4; plot1Cont.PointColor = Color.Orange;

                plot1Cont.NumContours = 20;

                plot1Cont.SurfacesVisible = false; plot1Cont.SurfaceColorIsScaled = true;
                plot1Cont.ValueFunctionOfCoordinates = new ExampleValueFunctionDiff21(); // difference between two functions
                plot1Cont.SurfaceColor = Color.Green; plot1Cont.SurfaceColorOpacity = 1;
                plot1Cont.SurfaceColorIsScaled = true;
                plot1Cont.SurfaceColorScale = ColorScale.Create(0, 1, Color.Blue, Color.Yellow); plot1Cont.SurfaceColorOpacity = 0.6;
                plot1Cont.LinesVisible = true; plot1Cont.LineWidth = 2; plot1Cont.LineColorIsScaled = true;
                plot1Cont.LineColor = Color.LightGray;
                plot1Cont.LineColorScale = ColorScale.Create(0, 1, Color.Blue, Color.Yellow);
                plot1Cont.Create();
            }

            // Create lineframe and solid surface for the second function graph:
            VtkSurfacePlot plot2 = new VtkSurfacePlot(plotter);
            plot2.OutputLevel = 1;  // print to console what's going on
            // Create the first surface graph by the plot object; adjust the setting first:
            plot2.SetSurfaceDefinition(func2);  // function of 2 variables that defines the surface
            plot2.SetBoundsParameters(minX, maxX, minY, maxY);
            plot2.NumX = numX; plot2.NumY = numY;
            plot2.PointsVisible = false; plot2.PointSize = 4; plot2.PointColor = Color.Green;
            plot2.SurfacesVisible = true; plot2.SurfaceColorIsScaled = false;
            plot2.SurfaceColor = Color.LightGray; plot2.SurfaceColorOpacity = 0.5;
            plot2.LinesVisible = true; plot2.LineWidth = 1; plot2.LineColorIsScaled = false;
            plot2.LineColor = Color.Brown;

            //VtkDecorationHandler decorator = plotter.DecorationHandler;
            //decorator.Bounds.ExpandOrShrinkInterval(3);
            //decorator.UpdateBounds();

            plot2.CreateAndShow();
            //plotter.ShowPlot();
            

        }

        #endregion ExampleCustomFunctionComparison


        #region CurvePlots

        /// <summary>Sine function with the specified frequency factor and phase.
        /// Used in the <see cref="ExampleLissajous"/>.</summary>
        public class ExampleSineFunctionForLissajous : RealFunction
        {
            public ExampleSineFunctionForLissajous(double frequencyFactor, double phase)
            {
                this.FrequencyFactor = frequencyFactor; this.Phase = phase;
            }

            public ExampleSineFunctionForLissajous(double frequencyFactor) : this(frequencyFactor, 0) { }

            double FrequencyFactor = 1, Phase = 0;

            public override double Value(double x)
            {
                return Math.Sin((double)FrequencyFactor * x + Phase);
            }
        }

        
        /// <summary>Example curve plot. Plots a curve whose projection on the XY plane is a Lissajous
        /// curve, but which also oscillates in the Z direction.</summary>
        public static void ExampleCurvePlotLissajous()
        {
            ExampleCurvePlotLissajous(4, 5);
        }

        /// <summary>Example curve plot. Plots a curve whose projection on the XY plane is a Lissajous
        /// curve, but which also oscillates in the Z direction.</summary>
        /// <param name="a">First parameter of the plotted Lissajous curve.</param>
        /// <param name="b">Second parameter of the plotted Lissajous curve.</param>
        /// <remarks>Lissajous curves are well known from fugures created on oscilloscopes when the apparatus
        /// is connected to two oscillating voltages with different frequences (in ratios that can be ) </remarks>
        public static void ExampleCurvePlotLissajous(int a, int b)
        {
            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkCurvePlot plot = new VtkCurvePlot(plotter);  // plot object that will create curve plots on the plotter object
            plot.OutputLevel = 1;  // print to console what's going on
            // Create the first curve graph by the plot object; adjust the setting first:
            plot.SetCurveDefinition(new ExampleSineFunctionForLissajous(a),
                new ExampleSineFunctionForLissajous(b),
                new ExampleSineFunctionForLissajous(a+b)
                );  // function of 1 variable that define the curve
            plot.SetBoundsReference(0, 2 * Math.PI);
            plot.NumX = 400;
            plot.PointsVisible = true; plot.PointColorIsScaled = false;
            plot.PointSize = 6; plot.PointColor = Color.Blue;  plot.PointColorScale = ColorScale.CreateBlueYellow(0, 1);
            plot.LinesVisible = true; plot.LineColorIsScaled = true;
            plot.LineWidth = 4; plot.LineColor = Color.Red;  plot.LineColorScale = ColorScale.CreateRainbow(0, 1);

            // Create plot of the first surface according to settings:
            plot.Create();

            // Show the created blot:
            plotter.ShowPlot();

        }  // ExampleCurvePlotLissajous(...)


        /// <summary>Functions for all 3 co-ordinates of parametric curve definition of a p-q torus knot .
        /// Used in the <see cref="ExampleCurvePlotTorusKnot"/>.</summary>
        /// <remarks>See http://en.wikipedia.org/wiki/Torus_knot </remarks>
        protected class ExampleFunctionTorusKnot : RealFunction
        {


            /// <summary>Constructs one of the coordinate functions in parametric definition of the torus knot curve.</summary>
            /// <param name="whichCoordinate">Selects the co-ordinate. Must be 0 for X coordinate, 1 for Y coordinate, or 2
            /// for Z coordinate.</param>
            /// <param name="p">The p-parameter. p and q must be coprimes.</param>
            /// <param name="q">The q-parameter of the p-q torus knot. p and q must be coprimes.</param>
            public ExampleFunctionTorusKnot(int whichCoordinate, int p, int q)
            {
                this.WhichCoordinate = whichCoordinate;
                this.P = p;
                this.Q = q;
            }

            protected int WhichCoordinate;

            /// <summary>The first parameter.</summary>
            protected int P = 3;

            /// <summary>The second parameter.</summary>
            protected int Q = -7;


            /// <summary>Returns function value at the specified parameter.</summary>
            /// <param name="fi">Parameter.</param>
            public override double Value(double fi)
            {
                double r = Math.Cos(Q * fi) + 2.0;
                if (WhichCoordinate == 0)
                    return r * Math.Cos((double) P * fi);
                else if (WhichCoordinate == 1)
                    return r * Math.Sin((double) P * fi);
                else if (WhichCoordinate==2)
                    return - Math.Sin((double) Q * fi);
                throw new ArgumentException("Coordinate selector must be 0 (X), 1(Y) or 2 (Z).");
            }

        }

        
        /// <summary>Example curve plot. Plots a curve whose projection on the XY plane is a Lissajous
        /// curve, but which also oscillates in the Z direction.</summary>
        public static void ExampleCurvePlotTorusKnot()
        {
            ExampleCurvePlotTorusKnot(3, -7);
        }

        /// <summary>Example curve plot. Plots a two-parameters torus knot. Parameters must be coprimes.</summary>
        /// <param name="p">First parameter of the torus knot.</param>
        /// <param name="q">Second parameter of the torus knot.</param>
        /// <remarks>Lissajous curves are well known from fugures created on oscilloscopes when the apparatus
        /// is connected to two oscillating voltages with different frequences (in ratios that can be ) </remarks>
        public static void ExampleCurvePlotTorusKnot(int p, int q)
        {
            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkCurvePlot plot = new VtkCurvePlot(plotter);  // plot object that will create curve plots on the plotter object
            plot.OutputLevel = 1;  // print to console what's going on
            // Create the first curve graph by the plot object; adjust the setting first:
            plot.SetCurveDefinition(new ExampleFunctionTorusKnot(0, p, q),
                new ExampleFunctionTorusKnot(1, p, q),
                new ExampleFunctionTorusKnot(2, p, q)
                );  // functions of 1 variable that define the curve
            plot.SetBoundsReference(0, 2 * Math.PI);
            plot.NumX = 400;
            plot.PointsVisible = true; plot.PointColorIsScaled = false;
            plot.PointSize = 6; plot.PointColor = Color.Blue;  plot.PointColorScale = ColorScale.CreateBlueYellow(0, 1);
            plot.LinesVisible = true; plot.LineColorIsScaled = true;
            plot.LineWidth = 4; plot.LineColor = Color.Red;  plot.LineColorScale = ColorScale.CreateRainbow(0, 1);

            // Create plot of the first surface according to settings:
            plot.Create();

            // Show the created blot:
            plotter.ShowPlot();

        }  // ExampleCurvePlotTorusKnot(...)


        #endregion CurvePlots


        /// <summary>Example that demonstrates how various plotter decorations can be set up.</summary>
        public static void ExamplePlotterDecoration()
        {

            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkSurfacePlot plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object

            plot.OutputLevel = 1;  // print to console what's going on
            // Create the first surface graph by the plot object; adjust the setting first:
            plot.SetSurfaceDefinition(new ExampleFunc2dXY());  // function of 2 variables that defines the surface
            plot.SetBoundsParameters(-2, 2, -1, 1);
            plot.NumX = plot.NumY = 30;
            plot.PointSize = 4; plot.LineWidth = 2; plot.PointsVisible = false;
            plot.SurfaceColorIsScaled = true;
            plot.LineColorIsScaled = false;

            //plot.BackGround = Color.LightGray;
            //plot.PointsVisible = false; plot.SurfacesVisible = false; plot.LinesVisible = true;
            //plot.PointSize = 8;

            // Create plot of the first surface according to settings:
            plot.Create();

            plotter.BackGround = Color.LightGray;

            // Add some decorations:

            plotter.DecorationHandler.ShowLegendBox = true;

            plotter.DecorationHandler.CubeAxesFlyMode = VtkFlyMode.OuterEdges;
            plotter.DecorationHandler.CubeAxesXLabel = "X axis";
            plotter.DecorationHandler.CubeAxesYLabel = "Y axis";
            plotter.DecorationHandler.CubeAxesZLabel = "Z axis";

            plotter.DecorationHandler.ShowCubeAxes = true;

            double minRange = -5;
            double maxRange = 5;
            BoundingBox1d bounds = plot.BoundsPointValues;
            if (bounds != null)
            {
                minRange = bounds.MinX;
                maxRange = bounds.MaxX;
            }

            plotter.DecorationHandler.ScalarBarTitle = "Color scale legend";
            plotter.DecorationHandler.ScalarBarNumberOfLabels = 6;
            plotter.DecorationHandler.LookUpTableMinRange = minRange;
            plotter.DecorationHandler.LookUpTableMaxRange = maxRange;
            plotter.DecorationHandler.LookUpTableColorScale = new ColorScale(minRange, maxRange, Color.Blue, Color.DarkRed, Color.Orange);
            plotter.DecorationHandler.ShowScalarBar = true;

            plotter.DecorationHandler.LegendBoxTitles = new string[] { "Legend box title No. 1", "Legend box title No. 2", "Legend box title No. 3" };
            plotter.DecorationHandler.ShowLegendBox = true;

            // Scaling of graphics:
            plotter.IsScaled = false;

            plotter.ShowPlot();
        }


        /// <summary>Example of surface plots.</summary>
        public static void ExampleSurfacePlot()
        {

            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkSurfacePlot plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object
            
            plot.OutputLevel = 1;  // print to console what's going on
            // Create the first surface graph by the plot object; adjust the setting first:
            plot.SetSurfaceDefinition(new ExampleFunc2dXY());  // function of 2 variables that defines the surface
            plot.SetBoundsParameters(-1, 1, -1, 1);
            plot.NumX = plot.NumY = 30;
            plot.PointSize = 4; plot.LineWidth = 2; plot.PointsVisible = false;
            plot.SurfaceColorIsScaled = true;
            plot.LineColorIsScaled = false;

            //plot.BackGround = Color.LightGray;
            //plot.PointsVisible = false; plot.SurfacesVisible = false; plot.LinesVisible = true;
            //plot.PointSize = 8;

            // Create plot of the first surface according to settings:
            plot.Create();

            // Then, create teh second surface graph by the same plot object; change some settings first:
            plot.SetSurfaceDefinition(new ExampleFunc2dLinear()); // another function of 2 variables for the second graph
            plot.PointsVisible = true;
            plot.SurfaceColorIsScaled = false;
            plot.SurfaceColor = System.Drawing.Color.LightGreen;
            plot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent
            // Create the second plot:
            plot.Create();
            plotter.ResetCamera();
            // No show all the plots that were created by the plotter; method with the same name is 
            // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):
            plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

            // Then create yet another surface plot with the same plot object (changing some settings again):
            plot.SetSurfaceDefinition(new ExampleFunc2dSquare()); // function of 2 variables whose graph will be plotted:
            plot.LineColorIsScaled = false;
            plot.PointSize = 2.0;
            plot.SurfaceColor = System.Drawing.Color.Orange;
            plot.SurfaceColorOpacity = 0.5;

            
            // Now create the third surface plot and immediately show it by the plotter:
            plot.CreateAndShow();

            // Now remove from the plotter everything plotted by now by the first plot object:
            plot.RemoveActors();  
            // Then create yet another surface plot by the same plot object and show it:
            IFunc2d func = new ExampleFunc2dSquare();
            plot.SetSurfaceDefinition(func);  // functino of 2 variables that defines the surface
            plot.SurfaceColorIsScaled = false;
            plot.SurfaceColor = System.Drawing.Color.DarkViolet; // this will automatically set opacity to 1 (non-transparent)
            plot.SurfaceColorOpacity = 0.6;
            plot.CreateAndShow();

            // Finally, add a custom plot; we create a set of random points on the plotted surface and a set of 
            // perturbed points that deviate from these points in Z direction by a random distance. We plot lines between
            // points on the surface and perturbed points.
            plot.LineColor = new color(1, 0, 0);
            plot.LineWidth = 4;
            plot.PointColor = new color(1, 1, 0);
            plot.PointSize = 6;
            int numPoints = 300;
            double averageDeviation = 0.3;
            IRandomGenerator rand = new RandomGenerator();
            for (int i = 0; i < numPoints; ++i)
            {
                double x = 2 * (0.5 - rand.NextDouble());
                double y = 2 * (0.5 - rand.NextDouble());
                double z = func.Value(x, y);
                double zError = z + 2.0 * averageDeviation * (0.5 - rand.NextDouble());
                plot.AddPoint(x, y, z);
                plot.AddPoint(x, y, zError);
                plot.AddLine(x, y, z, x, y, zError);
            }
            
            plot.CreateCustomPlot();
            plot.ShowPlot();


        }  // ExampleSurfacePlot()


        /// <summary>Example of surface plots where graph is scaled automaticallly.</summary>
        public static void ExampleSurfacePlotScaled()
        {
            ExampleSurfacePlotScaled(30 /* numX */, 30 /* numY */);
        }


        /// <summary>Example of surface plots where graph is scaled automatically.</summary>
        /// <param name="numX">Number of plotting points in x direction.</param>
        /// <param name="numY">Number of plotting points in Y direction.</param>
        public static void ExampleSurfacePlotScaled(int numX, int numY)
        {
            if (numX < 2)
                numX = 30;
            if (numY < 2)
                numY = 30;

            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Test of surface polot with manually composed grid and automatic scaling." + Environment.NewLine);


            // Manually prepare the mesh for the surface to be plotted:
            double
                fromX = -1,
                toX = 1,
                fromY = -1,
                toY = 1;
            double stepX = (toX - fromX) / (double)(numX - 1);
            double stepY = (toY - fromY) / (double)(numX - 1);
            Func2dBaseNoHessian func;
            // func = new ExampleFunc2dXY();
            func = new ExampleFunc2dSquare();
            StructuredMesh2d3d mesh = new StructuredMesh2d3d(numX, numY);
            for (int iX = 0; iX < numX; ++iX)
            {
                double x = fromX + (double)iX * stepX; // X coordinate of the mesh point
                for (int iY = 0; iY < numY; ++iY)
                {
                    double y = fromY + (double)iY * stepY;  // Y coordinate of the mesh point
                    double z = func.Value(x, y);  // Z coordinate of the mesh point - by function of 2 variables

                    // Set coordinates of the mesh point, stretch in x and z direction:
                    mesh[iX, iY] = new vec3(
                        2 * x,
                        y,
                        3 * z);
                }
            }

            // Prepare plotter and plot for the first surface:
            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkSurfacePlot plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object
            plot.OutputLevel = 1;  // print to console what's going on

            plot.ClearSurfaceDefinition();
            // Assign mesh to the surface plot:
            plot.Mesh = mesh;

            Console.WriteLine("Manual surface defined." + Environment.NewLine
                + "Mesh: " + Environment.NewLine
                + "  NumX = " + mesh.Dim1 + " " + Environment.NewLine
                + "  NumY = " + mesh.Dim1 + " " + Environment.NewLine
                + "Plot: " + Environment.NewLine
                + "  NumX = " + plot.NumX + " " + Environment.NewLine
                + "  NumY = " + plot.NumY + " " + Environment.NewLine
                + Environment.NewLine);

            plot.PointsVisible = true;
            plot.SurfaceColorIsScaled = true;
            plot.SurfaceColor = System.Drawing.Color.LightGreen;
            plot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent

            plot.Create();

            plotter.ResetCamera();
            // No show all the plots that were created by the plotter; method with the same name is 
            // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):
            plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

            // Create the second plot by scaling the first one in physical coordinates (i.e., by 
            // scaling the mesh of the surface plot):

            // First, scale the mesh:
            // Get bounds of the original mesh:
            BoundingBox3d originalBounds = new BoundingBox3d();
            originalBounds.Update(mesh.Coordinates);


            // Define new bounds:
            BoundingBox3d scaledBounds = new BoundingBox3d();
            scaledBounds.Min = new Vector3d(-5, -5, -3);
            scaledBounds.Max = new Vector3d(5, 5, 3);

            //Vector3d coord = new Vector3d(0, 0, 0);
            //for (int i = 0; i < mesh.Length; ++i)
            //{
            //    coord.Vec = mesh.Coordinates[i];
            //    IVector result = coord;
            //    BoundingBox3d.Map(originalBounds, scaledBounds, coord, ref result);
            //    mesh.Coordinates[i] = coord.Vec;
            //}
            //// Print original and scaled bounds to the console:
            //Console.WriteLine("Manual surface scaled." + Environment.NewLine
            //    + "Original mesh bounds: " + Environment.NewLine
            //    + "  " + originalBounds.ToString() + " " + Environment.NewLine
            //    + "Scaled mesh bounds: " + Environment.NewLine
            //    + "  " + scaledBounds.ToString() + " " + Environment.NewLine
            //    + Environment.NewLine);

            // Create a new plotter and plot the scaled mesh:
            plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            // switch on scaling to the specified bounds:
            plotter.IsScaled = true;
            plotter.SetBoundsScaled(scaledBounds);
            // Create and show the plot:
            plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object
            plot.OutputLevel = 1;  // print to console what's going on
            plot.ClearSurfaceDefinition();
            plot.Mesh = mesh; // assign the scaled mesh to the plot
            plot.PointsVisible = true;
            plot.SurfaceColorIsScaled = true;
            plot.SurfaceColor = System.Drawing.Color.LightGreen;
            plot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent

            plot.Create();
            plotter.ResetCamera();
            // No show all the plots that were created by the plotter; method with the same name is 
            // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):
            plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window


        }  // ExampleSurfacePlot()


        /// <summary>Example of surface plots where mesh is generated manually. Also shows scaling of graph.</summary>
        public static void ExampleSurfacePlotManualScaled()
        {
            ExampleSurfacePlotManualScaled(30 /* numX */, 30 /* numY */);
        }

        /// <summary>Example of surface plots where mesh is generated manually. Also shows scaling of graph.</summary>
        /// <param name="numX">Number of plotting points in x direction.</param>
        /// <param name="numY">Number of plotting points in Y direction.</param>
        public static void ExampleSurfacePlotManualScaled(int numX, int numY)
        {
            if (numX < 2)
                numX = 30;
            if (numY < 2)
                numY = 30;

            Console.WriteLine(Environment.NewLine+Environment.NewLine
                + "Test of surface polot with manually composed grid and manual scaling." + Environment.NewLine);

            // Manually prepare the mesh for the surface to be plotted:
            double
                fromX = -1,
                toX = 1,
                fromY = -1,
                toY = 1;
            double stepX = (toX-fromX)/(double)(numX-1);
            double stepY = (toY - fromY) / (double)(numX - 1);
            Func2dBaseNoHessian func;
            // func = new ExampleFunc2dXY();
            func = new ExampleFunc2dSquare();
            StructuredMesh2d3d mesh = new StructuredMesh2d3d(numX, numY);
            for (int iX = 0; iX < numX; ++iX)
            {
                double x = fromX + (double)iX * stepX; // X coordinate of the mesh point
                for (int iY = 0; iY < numY; ++iY)
                {
                    double y = fromY + (double)iY * stepY;  // Y coordinate of the mesh point
                    double z = func.Value(x, y);  // Z coordinate of the mesh point - by function of 2 variables

                    // Set coordinates of the mesh point, stretch in x and z direction:
                    mesh[iX, iY] = new vec3(
                        2 * x,
                        y, 
                        3 * z);
                }
            }


            // Prepare plotter and plot for the first surface:
            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkSurfacePlot plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object
            plot.OutputLevel = 1;  // print to console what's going on

            plot.ClearSurfaceDefinition();
            // Assign mesh to the surface plot:
            plot.Mesh = mesh;

            Console.WriteLine("Manual surface defined." + Environment.NewLine
                + "Mesh: " + Environment.NewLine
                + "  NumX = " + mesh.Dim1 + " " + Environment.NewLine
                + "  NumY = " + mesh.Dim1 + " " + Environment.NewLine
                + "Plot: " + Environment.NewLine
                + "  NumX = " + plot.NumX + " " + Environment.NewLine
                + "  NumY = " + plot.NumY + " " + Environment.NewLine
                + Environment.NewLine);
            
            plot.PointsVisible = true;
            plot.SurfaceColorIsScaled = true;
            plot.SurfaceColor = System.Drawing.Color.LightGreen;
            plot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent
            
            plot.Create();
            
            plotter.ResetCamera();
            // No show all the plots that were created by the plotter; method with the same name is 
            // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):
            plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

            // Create the second plot by scaling the first one in physical coordinates (i.e., by 
            // scaling the mesh of the surface plot):

            // First, scale the mesh:
            // Get bounds of the original mesh:
            BoundingBox3d originalBounds = new BoundingBox3d();
            originalBounds.Update(mesh.Coordinates);
            // Set new bounds:
            BoundingBox3d scaledBounds = new BoundingBox3d();
            scaledBounds.Min = new Vector3d(0, 0, 0);
            scaledBounds.Max = new Vector3d(1, 1, 1);
            // Transform coordinates of the mesh:
            Vector3d coord = new Vector3d(0,0,0);
            for (int i = 0; i < mesh.Length; ++i)
            {
                coord.Vec = mesh.Coordinates[i];
                IVector result = coord;
                BoundingBox3d.Map(originalBounds, scaledBounds, coord, ref result);
                mesh.Coordinates[i] = coord.Vec;
            }
            // Print original and scaled bounds to the console:
            Console.WriteLine("Manual surface scaled." + Environment.NewLine
                + "Original mesh bounds: " + Environment.NewLine
                + "  " + originalBounds.ToString() + " " + Environment.NewLine
                + "Scaled mesh bounds: " + Environment.NewLine
                + "  " + scaledBounds.ToString() + " " + Environment.NewLine
                + Environment.NewLine);

            // After mesh is scaled, create a new plotter and plot the scaled mesh:
            plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object
            plot.OutputLevel = 1;  // print to console what's going on
            plot.ClearSurfaceDefinition();
            plot.Mesh = mesh; // assign the scaled mesh to the plot
            plot.PointsVisible = true;
            plot.SurfaceColorIsScaled = true;
            plot.SurfaceColor = System.Drawing.Color.LightGreen;
            plot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent

            plot.Create();
            plotter.ResetCamera();
            // No show all the plots that were created by the plotter; method with the same name is 
            // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):
            plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window


        }  // ExampleSurfacePlot()


        /// <summary>Examples of contour plots.</summary>
        public static void ExampleContourPlot()
        {

            IFunc2d func = new ExampleFunc2dXY();
            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            VtkContourPlot plot = new VtkContourPlot(plotter);  // plot object that will create contour plots on the plotter object
            plot.OutputLevel = 1;  // print to console what's going on
            BoundingBox2d paramBounds = new BoundingBox2d(-1, 1, -1, 1);
            // Create the first surface graph by the plot object; adjust the setting first:
            func = new ExampleFunc2dXY();
            plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            plot.SetBoundsParameters(paramBounds);
            plot.NumX = plot.NumY = 30;
            plot.NumContours = 20;
            plot.LinesVisible = true;
            plot.SurfacesVisible = false;
            plot.PointSize = 4; plot.LineWidth = 2; plot.PointsVisible = false; 
            plot.SurfaceColorIsScaled = true;
            plot.LineColorIsScaled = false;
            // Create plot of the first surface according to settings:
            plot.Create();

            // Now show all the plots that were created by the plotter; method with the same name is 
            // usually run on the plotter object, but we can run int on the plot object as wall (it will delegate it to plotter):
            
            //plot.ShowPlot();  // since plotter is set to have standalone rendering window, this will open a new window

            // Then, create the surface graph to combine it with contours; change some settings first:
            VtkSurfacePlot surfacePlot = new VtkSurfacePlot(plotter);  // plot object that will create contour plots on the plotter object
            surfacePlot.SetSurfaceDefinition(func); // another function of 2 variables for the secont graph
            surfacePlot.SetBoundsParameters(paramBounds);
            surfacePlot.NumX = surfacePlot.NumY = 8;
            surfacePlot.PointsVisible = true;
            surfacePlot.SurfacesVisible = false;
            surfacePlot.LinesVisible = true;
            surfacePlot.SurfaceColorIsScaled = false;
            surfacePlot.SurfaceColor = System.Drawing.Color.LightGreen;
            surfacePlot.SurfaceColorOpacity = 0.7;  // make this surface semitransparent
            // Create the second plot:
            surfacePlot.CreateAndShow();

        }


        /// <summary>Copies bounds on parameters from the function definition of parametric surface
        /// to a plot object for plotting surfaces.</summary>
        public static void CopyBounds(Func3d2dExamples.ParametricSurface func, VtkSurfacePlot plot)
        {
            if (func == null) throw new ArgumentException("Function definition of parametric surface is null.");
            if (plot == null) throw new ArgumentException("Plot object is null.");
            plot.SetBoundsParameters(func.MinX, func.MaxX, func.MinY, func.MaxY);
            plot.NumX = func.NumX;
            plot.NumY = func.NumY;
        }


        /// <summary>Examples of various parametric surface plots.</summary>
        public static void ExampleParametricSurfacePlots()
        {
            // TODO: To implement:
            // Weierstrass surface
            // Costa's minimal surface
            // Roman surface
            // Boy's surface
            // Super ellipsoid, super torroid
            // Random hills
            // Cross-cap
            // Dini's surface
            // Cinquefoil knot
            // trefoil knot 
            // Figure-eight knot 


            VtkPlotter plotter = new VtkPlotter();  // plotter object that handles rendering of plots
            Func3d2dExamples.ParametricSurface func = new Func3d2dExamples.Paraboloid(2, 1, 1);

            VtkSurfacePlot plot = new VtkSurfacePlot(plotter);  // plot object that will create surface plots on the plotter object
            plot.SetSurfaceDefinition(func);
            plot.OutputLevel = 1;  // print to console what's going on
            plot.SetBoundsParameters(-1, 1, -1, 1);
            plot.NumX = 20;  plot.NumY = 20;
            plot.PointSize = 4; plot.LineWidth = 1; plot.PointsVisible = false;
            plot.SurfaceColorIsScaled = false;
            plot.LineColorIsScaled = false;
            plot.SurfaceColor = System.Drawing.Color.LightCyan;
            plot.LineColor = System.Drawing.Color.LightGreen;
            plot.SurfaceColorOpacity = 0.8;

            plot.LineColor = System.Drawing.Color.Red; plot.LineWidth = 2;
            plot.SurfaceColor = System.Drawing.Color.Cyan;
            plot.SurfaceColorOpacity = 0.7;

            //// Cylinder surface
            //func = new Func3d2dExamples.CylinderParametric(1, 1, 5);
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Ellipsoid surface
            //plot.RemoveActors();
            //func = new Func3d2dExamples.EllipsoidParametric(2, 1, 0.5);
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Enneper surface:
            //plot.RemoveActors();
            //func = new Func3d2dExamples.EnneperSurface();
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Umbilic torus, aingle-edged 3d surface:
            //plot.RemoveActors();
            //func = new Func3d2dExamples.UmbilicTorus();
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            // Klein's bottle of interesting and demonstrative shape:
            plot.RemoveActors();
            func = new Func3d2dExamples.KleinBottle();
            plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            CopyBounds(func, plot);
            plot.CreateAndShow();

            //// Klein's bottle where topology is well seen:
            //plot.RemoveActors();
            //func = new Func3d2dExamples.KleinBottle1();
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Klein's bottle where topology is well seen:
            //plot.RemoveActors();
            //func = new Func3d2dExamples.KleinBottle2(0.2);
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Möbius strip:
            //plot.RemoveActors();
            //func = new Func3d2dExamples.MobiusStrip();
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Snailshell-shaped surface
            //plot.RemoveActors();
            //// WARNING: Function of this class must be re-checked.
            //func = new Func3d2dExamples.SnailConicSpiral_ToCheck(
            //    1.0 /* a */,
            //    1.0 /* b */,
            //    0.1 /* c */,
            //    2 /* n */);
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Snailshell-shaped surface
            //plot.RemoveActors();
            //func = new Func3d2dExamples.SnailShell1Streched();
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Two interlocked torus surfaces:
            //plot.RemoveActors();
            //func = new Func3d2dExamples.TorusHorizontal();
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.Create();
            //func = new Func3d2dExamples.TorusVertical();
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.SurfaceColor = System.Drawing.Color.YellowGreen;
            //plot.Create();
            //plot.ShowPlot();

            //// Torus (toroid surface):
            //plot.RemoveActors();
            //func = new Func3d2dExamples.Torus(0.5, 1);
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// One-sheeted hyperboloid:
            //plot.RemoveActors();
            //func = new Func3d2dExamples.HyperboloidParametricPlus(1, 2);
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.CreateAndShow();

            //// Two-sheeted hyperboloid:
            //plot.RemoveActors();
            //func = new Func3d2dExamples.HyperboloidTwosheetedUpperParametric(1, 1);
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.Create();
            //func = new Func3d2dExamples.HyperboloidTwosheetedLowerParametric(1, 1);
            //plot.SetSurfaceDefinition(func);  // function of 2 variables that defines the surface
            //CopyBounds(func, plot);
            //plot.Create();
            //plot.ShowPlot();


        }  // ExampleParametricSurfacePlots()


        #endregion Examples


    } // abstract class VtkPlotBase


}