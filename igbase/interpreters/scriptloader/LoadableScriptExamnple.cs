// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Text;

using IG.Lib;

namespace IG.Script
{

    /// <summary>Example cls. for testing function of loadable scripts.</summary>
    public class LoadableScriptExample : LoadableScriptBase,
        ILoadableScript, ILockable
    {
        
        #region Initialization

        /// <summary>Creates the <see name="LoadableScriptExamnple"/> object.</summary>
        public LoadableScriptExample()
            : base()
        {  }

        protected void PrintArguments(string[] args)
        {
            Console.WriteLine("Arguments: ");
            if (args == null)
                Console.WriteLine("  null array.");
            else if (args.Length < 1)
                Console.WriteLine("  empty array (length 0).");
            else
            {
                for (int i=0; i<args.Length; ++i)
                {
                    if (args[i] == null)
                        Console.WriteLine("  Arg. " + i + ": null");
                    Console.WriteLine("  Arg. " + i + ": \"" + args[i] + "\"");
                }
            }
        }

        /// <summary>Inializes the current script object.</summary>
        /// <param name="arguments">Initialization arguments.
        /// The first argument must be the working directory path.</param>
        protected override void InitializeThis(string[] arguments)
        {
            Console.WriteLine(Environment.NewLine
                + "InitializeThis started..." + Environment.NewLine);
            PrintArguments(arguments);
            if (arguments != null)
            {
                //if (arguments.Length >= 1)
                //    OptimizationDirectory = arguments[0];
            }
            Console.WriteLine(Environment.NewLine
                + "InitializeThis finished." + Environment.NewLine);
        }

        #endregion Initialization


        /// <summary>Runs action of the current object.</summary>
        /// <param name="arguments">Command-line arguments of the action.</param>
        protected override string RunThis(string[] arguments)
        {
            string ret = null;
            Console.WriteLine(Environment.NewLine
                + "RinThis started..." + Environment.NewLine);
            PrintArguments(arguments);

            Console.WriteLine();
            Console.WriteLine("Class name: " + this.GetType().Name);
            Console.WriteLine("*********** SCRIPT EXECUTION... ***********");
            Script_PrintArguments("Run arguments: ", arguments);
            // Script_CommandHelp(arguments);

            if (arguments.Length < 1)
            {
                Console.WriteLine();
                Console.WriteLine("Command name is not specified!!");
                Console.WriteLine("Usage: command commandName ...");
                Script_CommandHelp(new string[0]);
            }
            else
            {
                string command = arguments[0];
                string[] args = new string[arguments.Length - 1];
                for (int i = 0; i < args.Length; ++i)
                    args[i] = arguments[i + 1];
                if (!Script_ContainsCommand(command))
                {
                    Console.WriteLine();
                    Console.WriteLine("Command " + command + " is not known.");
                    Console.WriteLine();
                    Console.WriteLine("Usage: command templateDirectory commandName ...");
                    Script_CommandHelp(new string[0]);
                }
                else
                {
                    ret = Script_Run(command, args);
                }
            }

            Console.WriteLine("*********** EXECUTION FINISHED. ***********");
            Console.WriteLine();

            Console.WriteLine(Environment.NewLine + "Result: " + Environment.NewLine + "  \"" + ret + "\"");
            Console.WriteLine(Environment.NewLine
                + "RunThis finished." + Environment.NewLine);
            return ret;
        }


    }

}
