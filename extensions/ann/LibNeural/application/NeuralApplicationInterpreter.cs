// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

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

        /// <summary></summary>
        public NeuralApplicationInterpreter(): this(false)
        {  }

        /// <summary></summary>
        /// <param name="caseSensitive"></param>
        public NeuralApplicationInterpreter(bool caseSensitive)
            : base(caseSensitive)
        {
            NeuralApplicationCommandsTadej.InstallCommands(this); // ( as ICommandLineApplication);

            this.LoadableScriptInterpreter.ScriptLoader.AddReferencedAssemblies(
                "System.Windows.Forms.dll"
            );
        }

        #region Commands


        /// <summary>Execution method that prints some information about the application.</summary>
        /// <param name="commandThread">Command thread that is being executed.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="AppArguments">Command arguments.</param>
        protected override string CmdAbout(CommandThread commandThread, string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        return base.CmdAbout(commandThread, cmdName, args);
                    }
            string retBase = base.CmdAbout(commandThread, cmdName, args);
            return "This is a prototypic shell application, written by Igor Grešovnik and others."
                    + Environment.NewLine + retBase;
        }


        #endregion Commands

    }


}