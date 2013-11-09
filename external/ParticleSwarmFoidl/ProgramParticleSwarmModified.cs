using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ParticleSwarmDemo.FunctionMinimizing
{
	static class ProgramParticleSwarm  // Name is different than in the original.
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
#if ISCLASSLIBRARY
        // If this is used in a class library project, main must be renamed:
        static void MainParticleSwarm()
#else
		static void Main()
#endif
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
