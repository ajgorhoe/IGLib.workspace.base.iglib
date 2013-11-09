#define STOP
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ParticleSwarmDemo.FunctionMinimizing.Function;

namespace ParticleSwarmDemo.FunctionMinimizing
{
	public partial class Form1 : Form
	{
		private List<double[]> _solutions;
		private double[] _bestSolution;
		private ReaderWriterLockSlim _slimLock = new ReaderWriterLockSlim();
		private FunctionMinimizingParticleSwarm _swarm;
		private const int SWARM_SIZE = 20;
		private FunctionBase _function;
		//---------------------------------------------------------------------
		public Form1()
		{
			InitializeComponent();
			GetAvailableFunctions();
			_function = CreateFunction();
			SetImageOfFunction();

			cmbFunctions.SelectedIndexChanged += (sender, e) =>
				{
					_function = CreateFunction();
					SetImageOfFunction();
				};

			_swarm = new FunctionMinimizingParticleSwarm(_function, SWARM_SIZE);
			//_swarm.UseGlobalOptimum = true;
			_bestSolution = _swarm.CurrentBestPosition;
		}		
		//---------------------------------------------------------------------
		private void btnStart_Click(object sender, EventArgs e)
		{
			this.SetStatusText("Doing the evolution...");
			_solutions = new List<double[]>();
			_swarm = new FunctionMinimizingParticleSwarm(_function, SWARM_SIZE);
			_bestSolution = _swarm.CurrentBestPosition;

			ThreadPool.QueueUserWorkItem((o) =>
				{
					double minimum = double.MaxValue;
					double oldMinimum = double.MinValue;
					int iteration = 0;
					int countSame = 0;
					pictureBox1.Invalidate();

					// Wait for 25 iteration without any improvement:
#if STOP
					while (countSame < 25)
#else
					while (true)
#endif
					{
						Thread.Sleep(150);
						_swarm.Iteration();
						iteration++;
						minimum = _swarm.BestCost;

						if (Math.Abs(oldMinimum - minimum) < 0.001)
						{
							countSame++;
							oldMinimum = minimum;
						}
						else
						{
							countSame = 0;
							oldMinimum = minimum;
						}

						// Gather data for the animation:
						_slimLock.EnterWriteLock();
						try
						{
							double[] currentSolution = _swarm.CurrentBestPosition;
							_solutions.Add(currentSolution);
							_bestSolution = _swarm.BestPosition;
						}
						finally
						{
							_slimLock.ExitWriteLock();
						}

						pictureBox1.Invalidate();

						this.SetStatusText(string.Format(
							"Iteration: {0}. Minimum: {1:0.000}, " +
							"(x|y) = ({2:0.000}|{3:0.000})",
							iteration,
							_function.Function(_bestSolution),
							_bestSolution[0],
							_bestSolution[1]));
					}

					this.SetStatusText(string.Format(
						"Ready after {0} iterations. Minimum: {1:0.000}, " +
						"(x|y) = ({2:0.000}|{3:0.000})",
						iteration - 25,
						_function.Function(_bestSolution),
						_bestSolution[0],
						_bestSolution[1]));
				});
		}
		//---------------------------------------------------------------------
		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			int width = pictureBox1.Width;
			int height = pictureBox1.Height;

			_slimLock.EnterReadLock();
			try
			{
				// Draw the path of the solutions:
				if (_solutions != null && _solutions.Count > 1)
				{
					for (int i = 1; i < _solutions.Count; i++)
					{
						double[] start = Scale(_solutions[i - 1]);
						double[] end = Scale(_solutions[i]);

						// Line:
						g.DrawLine(
							Pens.Gray,
							(float)start[0], (float)start[1],
							(float)end[0], (float)end[1]);

						// Point:
						g.DrawEllipse(
							Pens.Red,
							(float)start[0] - 2, (float)start[1] - 2,
							4, 4);
					}
				}

				// Cross for the best solution found so far:
				if (_bestSolution != null)
				{
					double[] vector = Scale(_bestSolution);

					// x-Line:
					g.DrawLine(
						Pens.Black,
						(float)vector[0], 0,
						(float)vector[0], height);

					// y-Line:
					g.DrawLine(
						Pens.Black,
						0, (float)vector[1],
						width, (float)vector[1]);

					// Point for the best solution:
					g.FillEllipse(
						Brushes.Red,
						(float)vector[0] - 5, (float)vector[1] - 5,
						10, 10);
				}

				// Draw all particles on their current position:
				if (_swarm != null)
				{
					for (int i = 0; i < SWARM_SIZE; i++)
					{
						double[] vector = Scale(_swarm[i].Position);

						// Point a:
						g.FillEllipse(
							Brushes.Yellow,
							(float)vector[0] - 2, (float)vector[1] - 2,
							4, 4);
					}
				}
			}
			finally
			{
				_slimLock.ExitReadLock();
			}
		}
		//---------------------------------------------------------------------
		private double[] Scale(double[] vector)
		{
			int width = pictureBox1.Width;
			int height = pictureBox1.Height;

			// x,y in [-3,3]:
			double x = vector[0];
			double y = vector[1];

			// x,y in [0,1]:
			x = (x + 3) / 6d;
			y = (y + 3) / 6d;

			// x,y in [widht,height]:
			x *= width - 1;
			y *= height - 1;

			return new double[] { x, y };
		}
		//---------------------------------------------------------------------
		private void SetImageOfFunction()
		{
			int width = pictureBox1.Width;
			int height = pictureBox1.Height;

			// Evaluate the current function:
			double[][] data = new double[width][];
			for (int j = 0; j < data.Length; j++)
			{
				data[j] = new double[height];
				for (int i = 0; i < data[j].Length; i++)
				{
					// x, y in [0,1]:
					double x = (double)i / (double)(width - 1);
					double y = (double)j / (double)(height - 1);					

					// x,y in [-3,3]:
					x = 6 * x - 3;
					y = 6 * y - 3;

					data[j][i] = _function.Function(x, y);
				}
			}

			// Scale to [0,255] for producing a gray-scale image:
			this.Scale(data, 0, 255);

			// Create the image:
			Bitmap bmp = new Bitmap(width, height);
			for (int j = 0; j < width; j++)
				for (int i = 0; i < height; i++)
				{
					bmp.SetPixel(
						i, j,
						Color.FromArgb(
							(int)data[j][i],
							(int)data[j][i],
							(int)data[j][i]));
				}

			pictureBox1.Image = bmp;
		}
		//---------------------------------------------------------------------
		private void Scale(double[][] data, double start, double end)
		{
			double c = 0;
			double d = 0;

			// Max/Min:
			for (int i = 0; i < data.Length; i++)
				for (int j = 0; j < data[i].Length; j++)
				{
					// Minimum:
					if (data[i][j] < c)
						c = data[i][j];

					// Maximum:
					if (data[i][j] > d)
						d = data[i][j];
				}

			double a = start;
			double b = end;

			double m = (b - a) / (d - c);
			double o = (a * d - c * b) / (d - c);

			// Scale using a linear approcha:
			for (int i = 0; i < data.Length; i++)
				for (int j = 0; j < data[i].Length; j++)
					data[i][j] = m * data[i][j] + o;
		}
		//---------------------------------------------------------------------
		private void SetStatusText(string text)
		{
			if (this.InvokeRequired)
			{
				Action<string> a = s => SetStatusText(s);
				this.Invoke(a, text);
			}
			else
				statStatus.Text = text;
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Gets all classes that inherit from <see cref="FunctionBase"/>.
		/// </summary>
		private void GetAvailableFunctions()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			var query = from type in asm.GetTypes()
						where type.BaseType == typeof(FunctionBase)
						orderby type.Name
						select new
						{
							Name = type.Name,
							FullName = type.FullName
						};

			cmbFunctions.DisplayMember = "Name";
			cmbFunctions.ValueMember = "FullName";
			cmbFunctions.DataSource = query.ToList();
		}
		//---------------------------------------------------------------------
		private FunctionBase CreateFunction()
		{
			string fullName = cmbFunctions.SelectedValue.ToString();
			Type type = Type.GetType(fullName);
			object o = Activator.CreateInstance(type);
			return o as FunctionBase;
		}
	}
}