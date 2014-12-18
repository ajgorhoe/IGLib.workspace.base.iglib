
// NEURAL NETWORKS TRAINING DATA

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Num;
using IG.Neural;

namespace IG.Lib
{

    /// <summary>Command line application interpreter that contains some neural networks - related commands.</summary>
    public class NeuralApplicationInterpreter : CommandLineApplicationInterpreter
    {

        public NeuralApplicationInterpreter(): this(false)
        {  }

        public NeuralApplicationInterpreter(bool caseSensitive)
            : base(caseSensitive)
        {
            NeuralApplicationCommandsTadej.InstallCommands(this); // ( as ICommandLineApplication);

            this.LoadableScriptInterpreter.ScriptLoader.AddReferencedAssemblies(
                "System.Windows.Forms.dll"
            );
        }

        #region Commands


        ///// <summary>Execution method that prints some information about the application.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        protected override string CmdAbout(ICommandLineApplicationInterpreter interpreter,
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : prints some basic information about the current command interpreter.");
                        Console.WriteLine();
                        return null;
                    }
            if (args.Length > 0)
            {
                return base.CmdAbout(interpreter, cmdName, args);
            }
            {
                Console.WriteLine(Environment.NewLine + "This is a prototypic shell application, written by Igor Grešovnik and others."
                    + Environment.NewLine);
            }
            return null;
        }


        #endregion Commands

    }


}