using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ParticleSwarmFoidlNew;

namespace ParticleSwarmDemo.FunctionMinimizing
{
	static class Program
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main()
		{
#if NET6_0_OR_GREATER
			ApplicationConfiguration.Initialize();
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#endif
            Application.Run(new Form1());
		}
	}
}
