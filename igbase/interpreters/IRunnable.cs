using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IG.Lib
{

    /// <summary>Implemented by objects that have the Run(args) method, or "Main" metthod of execution.</summary>
    interface IRunnable
    {

        /// <summary>Runs the application.
        /// <para>This method should be overridden in each derived class.</para></summary>
        /// <param name="args">Arguments of the application.</param>
        void Run(string[] args);

    }

}
