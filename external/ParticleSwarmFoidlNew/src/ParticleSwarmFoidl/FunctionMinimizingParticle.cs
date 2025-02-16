using gfoidl.ComputationalIntelligence.Particles;
using ParticleSwarmDemo.FunctionMinimizing.Function;


namespace ParticleSwarmDemo.FunctionMinimizing
{
	public sealed class FunctionMinimizingParticle : Particle
	{
		private FunctionBase _function;
		//---------------------------------------------------------------------
		public FunctionMinimizingParticle(
			FunctionBase function,
            gfoidl.ComputationalIntelligence.Particles.ParticleSwarm swarm,
			double[] position,
			double[] velocity)
		{
			_function = function;
			this.Swarm = swarm;
			this.Position = position;
			this.Velocity = velocity;

			this.CalculateCost();
		}
		//---------------------------------------------------------------------
		public override void CalculateCost()
		{
			this.Cost = _function.Function(this.Position);
		}
        
	}
}