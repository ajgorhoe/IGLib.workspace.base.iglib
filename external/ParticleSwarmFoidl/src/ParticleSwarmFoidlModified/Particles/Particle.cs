using System;
using System.Linq;

namespace gfoidl.ComputationalIntelligence.Particles
{
	/// <summary>
	/// Provides a basic implementation of a paricle.
	/// </summary>
	public abstract class Particle : IComparable<Particle>
	{
		#region Felder
		protected static Random _rnd = new Random();
		protected double _bestCost = double.MaxValue;
		#endregion
		//---------------------------------------------------------------------
		#region Eigenschaften
		/// <summary>
		/// The cost for this particle. The lower the better.
		/// </summary>
		public double Cost { get; set; }
		//---------------------------------------------------------------------
		/// <summary>
		/// The current position of the particle.
		/// </summary>
		public virtual double[] Position { get; set; }
		//---------------------------------------------------------------------
		/// <summary>
		/// The best position of this particle so far.
		/// </summary>
		/// <remarks>pBest.</remarks>
		public double[] BestPosition { get; protected set; }
		//---------------------------------------------------------------------
		/// <summary>
		/// Indexer.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <exception cref="IndexOutOfRangeException">
		/// Der Index ist außerhalb der gültigen Grenzen.
		/// </exception>
		public double this[int index]
		{
			get { return this.Position[index]; }
			set { this.Position[index] = value; }
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// The velocity of the particle.
		/// </summary>
		public double[] Velocity { get; protected set; }
		//---------------------------------------------------------------------
		/// <summary>
		/// The particle swarm to whom this particle belongs.
		/// </summary>
		public ParticleSwarm Swarm { get; protected set; }
		#endregion
		//---------------------------------------------------------------------
		#region Methoden
		/// <summary>
		/// Calculates the cost for this particle.
		/// </summary>
		public abstract void CalculateCost();
		//---------------------------------------------------------------------
		/// <summary>
		/// Updates the history for this particle.
		/// </summary>
		public void UpdateHistory()
		{
			if (this.Cost < _bestCost)
			{
				_bestCost = this.Cost;
				this.BestPosition = this.GetArrayCopy();
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Updates the position (and velocity) of the particle.
		/// </summary>
		/// <param name="bestPositionOfSwarm">
		/// The current best position of the particle swarm.
		/// </param>
		public void UpdateVelocityAndPosition(double[] bestPositionOfSwarm)
		{
			if (this.BestPosition == null)
				this.UpdateHistory();

			// Determine maximum allowed velocity:
			double xmax = Math.Max(
				Math.Abs(this.Position.Min()),
				Math.Abs(this.Position.Max()));
			double vmax = this.Swarm.PercentMaximumVelocityOfSearchSpace * xmax;

			// Range-Check-Elimination: Therefore get a reference to the arrays
			// on the local stack -> improvement of speed:
			double[] localVelocity = this.Velocity;
			double[] localPosition = this.Position;
			double[] localBestPosition = this.BestPosition;

			for (int i = 0; i < localVelocity.Length; i++)
			{
				// Factors for calculating the velocity:
				double c1 = this.Swarm.TendencyToOwnBest;
				double r1 = _rnd.NextDouble();
				double c2 = this.Swarm.TendencyToGlobalBest;
				double r2 = _rnd.NextDouble();
				double m = this.Swarm.Momentum;

				// New velocity of the particle:
				double newVelocity =
					m * localVelocity[i] +
					c1 * r1 * (localBestPosition[i] - localPosition[i]) +
					c2 * r2 * (bestPositionOfSwarm[i] - localPosition[i]);

				// Limit the velocity to the maximum value:
				if (newVelocity > vmax)
					newVelocity = vmax;
				if (newVelocity < -vmax)
					newVelocity = -vmax;

				// Assign new velocity and calculate the new position:
				localVelocity[i] = newVelocity;
				localPosition[i] += localVelocity[i];
			}
		}
		#endregion
		//---------------------------------------------------------------------
		#region Private Methoden
		/// <summary>
		/// Gets a copy of the current solution vector.
		/// </summary>
		private double[] GetArrayCopy()
		{
			double[] tmp = new double[this.Position.Length];
			this.Position.CopyTo(tmp, 0);

			return tmp;
		}
		#endregion
		//---------------------------------------------------------------------
		#region IComparable<Particle> Member
		/// <summary>
		/// Compares to particles. Used for sorting.
		/// </summary>
		public int CompareTo(Particle other)
		{
			if (other == null)
				throw new ArgumentNullException("other");
			//-----------------------------------------------------------------
			if (this == other || this.Cost == other.Cost)
				return 0;

			if (this.Cost > other.Cost)
				return 1;
			else
				return -1;
		}
		#endregion
	}
}