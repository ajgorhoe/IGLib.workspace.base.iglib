
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Lib;
using IG.Num;


namespace IG.Neural
{

    /// <summary>Commands for application's interpreter for neural networks.</summary>
    /// $A Igor Apr11;
    public class NeuralApplicationCommandsTadej: NeuralAllpicationCommands
    {

        // FOR Tadej:
        // Če hočeš dodati kakšne ukaze v aplikacijo, narediš to tako, kot je pri CmdExampleTadej(...):
        // Definiraš metodo, ki izvede ukaz na podoben način kot CmdExampleTadej, in jo inštaliraš v 
        // InstallCommands()

        /// <summary>Installs Tadej's commands for neural networks on the applications's command interpreter.</summary>
        public static new void InstallCommands(ICommandLineApplicationInterpreter app)
        {
            if (app==null)
                throw new ArgumentNullException("Application command-line interpreter not specified (null reference).");
            NeuralAllpicationCommands.InstallCommands(app);

            app.AddCommand("ExampleTadej", CmdExampleTadej);
        }


        /// <summary>Command.
        /// Example command for Tadej.</summary>
        /// <param name="interpreter">Interpreter on which commad is run.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Format of the file if test was performed, null otherwise.</returns>
        public static string CmdExampleTadej(ICommandLineApplicationInterpreter interpreter, 
            string cmdName, string[] args)
        {
            if (args != null)
                if (args.Length > 0)
                    if (args[0] == "?")
                    {
                        string executableName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        Console.WriteLine();
                        Console.WriteLine(executableName + " " + cmdName + " : Does nothing.");
                        Console.WriteLine();
                        return null;
                    }
            Console.WriteLine();
            Console.WriteLine("This is a test comand. It does nothing.");
            Console.WriteLine();
            return null;
        }

    }

}