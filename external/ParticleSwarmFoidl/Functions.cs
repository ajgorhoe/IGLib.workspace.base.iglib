using System;

namespace ParticleSwarmDemo.FunctionMinimizing.Function
{
	/// <summary>
	/// R² -> R, x in [-3,3].
	/// </summary>
	public abstract class FunctionBase
	{
		public abstract double Function(double x, double y);
		//---------------------------------------------------------------------
		public double Function(double[] x)
		{
			return Function(x[0], x[1]);
		}
	}
	//-------------------------------------------------------------------------
	public class SphereFunction : FunctionBase
	{
		public override double Function(double x, double y)
		{
			return x * x + y * y;
		}
	}
	//-------------------------------------------------------------------------
	public class PeaksFunction : FunctionBase
	{
		public override double Function(double x, double y)
		{
			double z =
				3 * (1 - x) * (1 - x) *
				Math.Exp(-x * x) -
				(y + 1) * (y + 1) -
				10 * (x / 5 - x * x * x - y * y * y * y * y) *
				Math.Exp(-x * x - y * y) -
				1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
			return 1 - z;
		}
	}
	//-------------------------------------------------------------------------
	public class StepFunction : FunctionBase
	{
		public override double Function(double x, double y)
		{
			return Math.Abs(x) + Math.Abs(y);
		}
	}
	//-------------------------------------------------------------------------
	public class RosenbockFunction : FunctionBase
	{
		public override double Function(double x, double y)
		{
			return 100 * (x * x - y * y) * (x * x - y * y) + (1 - x) * (1 - x);
		}
	}
	//-------------------------------------------------------------------------
	public class RosenbockFunction2D : FunctionBase
	{
		public override double Function(double x, double y)
		{
			double a = 1, b = 100;
			return (a-x)*(a-x) + b * (y - x * x) * (y - x * x);
		}
	}
	//-------------------------------------------------------------------------
	public class SincFunction : FunctionBase
	{
		public override double Function(double x, double y)
		{
			double r = Math.Sqrt(x * x + y * y);
			double z = r == 0 ? 1 : Math.Sin(r) / r;

			return 1 - z;
		}
	}
	//-------------------------------------------------------------------------
	public class ManyPeaksFunction : FunctionBase
	{
		public override double Function(double x, double y)
		{
			// x, y in [-3,3] -> [0,1]:
			x = (x + 3) / 6d;
			y = (y + 3) / 6d;

			double z = 15 * x * y * (1 - x) * (1 - y) * Math.Sin(9 * Math.PI * y);
			z *= z;

			return 1 - z;
		}
	}
	//-------------------------------------------------------------------------
	public class HoleFunction : FunctionBase
	{
		public override double Function(double x, double y)
		{
			// x,y in [-3,3] -> [-5,5]:
			x = (x + 3) / 6d;	// [0,1]
			y = (y + 3) / 6d;	// [0,1]
			x = 10 * x - 5;
			y = 10 * y - 5;

			double z = Math.Cos(x) * Math.Cos(y);
			z *= z * z;
			return 1 - z;
		}
	}
	//-------------------------------------------------------------------------
	public class BumpsFunction : FunctionBase
	{
		public override double Function(double x, double y)
		{
			// x,y in [-3,3] -> [-5,5]:
			x = (x + 3) / 6d;	// [0,1]
			y = (y + 3) / 6d;	// [0,1]
			x = 10 * x - 5;
			y = 10 * y - 5;

			double z = Math.Cos(x) + Math.Cos(y);
			z = Math.Abs(z);
			return Math.Sqrt(z);
		}
	}
}