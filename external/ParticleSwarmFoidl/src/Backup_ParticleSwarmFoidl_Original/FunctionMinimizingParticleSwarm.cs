using System;
using gfoidl.ComputationalIntelligence.Particles;
using ParticleSwarmDemo.FunctionMinimizing.Function;

namespace ParticleSwarmDemo.FunctionMinimizing
{
	public sealed class FunctionMinimizingParticleSwarm : ParticleSwarm
	{
		public FunctionMinimizingParticleSwarm(
			FunctionBase function,
			int swarmSize)
		{
			// Create the swarm:
			this.InitSwarm(swarmSize, function);

			// Sort according to the cost of each particle:
			this.SortParticles();
		}
		//---------------------------------------------------------------------
		public void InitSwarm(int swarmSize, FunctionBase function)
		{
			// Create the array of particles:
			this.Particles = new FunctionMinimizingParticle[swarmSize];
			for (int i = 0; i < swarmSize; i++)
			{
				// Init with random position in [-3,3]:
				double x = _rnd.NextDouble() * 6 - 3;
				double y = _rnd.NextDouble() * 6 - 3;
				double[] particlePosition = { x, y };

				// Random initial velocity [-3,3]:
				double vx = _rnd.NextDouble() * 6 - 3;
				double vy = _rnd.NextDouble() * 6 - 3;
				double[] particleVelocity = { vx, vy };

				this[i] = new FunctionMinimizingParticle(
					function,
					this,
					particlePosition,
					particleVelocity);
			}
		}
	}
}