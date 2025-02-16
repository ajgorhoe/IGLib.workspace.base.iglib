using System;

namespace gfoidl.ComputationalIntelligence.Particles
{
	/// <summary>
	/// Provides a basic implementation of the parcticle swarm optimization.
	/// </summary>
	public abstract class ParticleSwarm
	{
		#region Felder
		protected static Random _rnd = new Random();
		#endregion
		//---------------------------------------------------------------------
		#region Eigenschaften
		/// <summary>
		/// The particles of this swarm.
		/// </summary>
		public Particle[] Particles { get; set; }
		//---------------------------------------------------------------------
		/// <summary>
		/// Indexer.
		/// </summary>
		public Particle this[int index]
		{
			get { return this.Particles[index]; }
			set { this.Particles[index] = value; }
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// The number of particles in the swarm.
		/// </summary>
		public int SwarmSize
		{
			get
			{
				if (this.Particles == null)
					return 0;

				return this.Particles.Length;
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// The cost of the current best particle of the swarm.
		/// </summary>
		public virtual double Cost { get { return this[0].Cost; } }
		//---------------------------------------------------------------------
		/// <summary>
		/// The cost of the best particle so far.
		/// </summary>
		public double BestCost { get; protected set; }
		//---------------------------------------------------------------------
		/// <summary>
		/// The current best of position of any particle in the swarm.
		/// </summary>
		/// <remarks>gBest.</remarks>
		public double[] CurrentBestPosition { get { return this[0].Position; } }
		//---------------------------------------------------------------------
		/// <summary>
		/// The best position of the swarm so far.
		/// </summary>
		public double[] BestPosition { get; protected set; }
		//---------------------------------------------------------------------
		private double _tendencyToOwnBest = 1;
		/// <summary>
		/// Accelerationcoefficient c1. Default value: 1
		/// </summary>
		public double TendencyToOwnBest
		{
			get { return _tendencyToOwnBest; }
			set { _tendencyToOwnBest = value; }
		}
		//---------------------------------------------------------------------
		private double _tendencyToGlobalBest = 1;
		/// <summary>
		/// Accelerationcoefficient c2. Default value: 1
		/// </summary>
		public double TendencyToGlobalBest
		{
			get { return _tendencyToGlobalBest; }
			set { _tendencyToGlobalBest = value; }
		}
		//---------------------------------------------------------------------
		private double _momentum = 1.05;
		/// <summary>
		/// Momentum. Default value: 1.05
		public double Momentum
		{
			get { return _momentum; }
			set { _momentum = value; }
		}
		//---------------------------------------------------------------------
		private double _percentMaximumVelocityOfSearchSpace = 1;
		/// <summary>
		/// Factor for maximum allowed velocity. 
		/// vmax = k * xmax mit k in [0.1, 1].
		/// Defaults to 1.
		/// </summary>
		public double PercentMaximumVelocityOfSearchSpace
		{
			get { return _percentMaximumVelocityOfSearchSpace; }
			set { _percentMaximumVelocityOfSearchSpace = value; }
		}
		//---------------------------------------------------------------------
		private bool _useGlobalOptimum = false;
		/// <summary>
		/// Determines wheter to use the global optimum or the current best
		/// solution for updating the particles. Default: false
		/// </summary>
		/// <remarks>
		/// This can be an improvement on speed (converges faster) but 
		/// the possibility to get stucked in a local optimum raises.
		/// <para>
		/// The global versiön can be used to get a rough solution and the 
		/// refine with the current best version.
		/// </para>
		/// </remarks>
		public bool UseGlobalOptimum
		{
			get { return _useGlobalOptimum; }
			set { _useGlobalOptimum = value; }
		}
		#endregion
		//---------------------------------------------------------------------
		#region Konstruktor
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ParticleSwarm()
		{
			this.BestCost = double.MaxValue;
		}
		#endregion
		//---------------------------------------------------------------------
		#region Methoden
		/// <summary>
		/// Sorts the particles according their cost.
		/// </summary>
		public void SortParticles()
		{
			Array.Sort(this.Particles);
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Does one iteration of the algorithm.
		/// </summary>
		public virtual void Iteration()
		{
			// Foreach particle calculate the cost and update the history:
			var localParticles = this.Particles;	// Range-Check-Elimination
			for (int i = 0; i < localParticles.Length; i++)
			{
				localParticles[i].CalculateCost();
				localParticles[i].UpdateHistory();
			}

			// Sort according to fitness:
			this.SortParticles();

			// Update history of the swarm:
			if (this.Cost < this.BestCost)
			{
				this.BestCost = this.Cost;
				this.BestPosition = this[0].BestPosition;
			}

			// Determine new velocity and position of the particles in
			// the swarm:
			for (int i = 0; i < localParticles.Length; i++)
				if (_useGlobalOptimum)
					localParticles[i].UpdateVelocityAndPosition(BestPosition);
				else
					localParticles[i].UpdateVelocityAndPosition(this[0].Position);
		}
		#endregion
	}
}