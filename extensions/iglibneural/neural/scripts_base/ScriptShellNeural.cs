// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


using IG.Lib;
using IG.Num;
using IG.Neural;





namespace IG.Script
{


    /// <summary>Base class for loadable scripts that are used as custom applications 
    /// that inclued neural networks approximations and optimization.</summary>
    /// <remarks><para>This is a concrete class. It implements abstract methods from <see cref="LoadableScriptShellNeuralBase"/>,
    /// in sime cases providing logical default implementations, but in some cases these implementations just throw
    /// exception.</para>
    /// <para>Sometimes you might check if you have implemented everything as necessary in the derived class.
    /// The trick is simply to derive from <see cref="LoadableScriptShellNeuralBase"/> instead of this class, and check 
    /// by compiler which abstract methods remained unimplemented in your class.</para></remarks>
    /// $A Igor xx Feb12;
    public class LoadableScriptShellNeural : LoadableScriptShellNeuralBase,
        ILoadableScript
    {

        #region Initialization

        public LoadableScriptShellNeural()
            : base()
        { }


        /// <summary>Inializes the current script object.</summary>
        /// <param name="arguments">Initialization arguments.
        /// The first argument must be the working directory path.</param>
        protected override void InitializeThis(string[] arguments)
        {
            base.InitializeThis(arguments);  // this sets the working directory to the first argument
        }

        #endregion Initialization


        #region Dummy_Actions


        /// <summary>Throws <see cref="NotImplementedException"/>.</summary>
        protected override string RunThis(string[] arguments)
        {
            Console.WriteLine();
            Console.WriteLine("Class name: " + this.GetType().Name);
            Console.WriteLine("*********** SCRIPT EXECUTION... ***********");
            Script_PrintArguments("Run arguments: ", arguments);
            // Script_CommandHelp(arguments);

            if (arguments.Length < 2)
            {
                Console.WriteLine();
                Console.WriteLine("Command name is not specified!!");
                Console.WriteLine("Usage: command templateDirectory commandName ...");
                Script_CommandHelp(new string[0]);
            }
            else
            {
                string command = arguments[1];
                string[] args = new string[arguments.Length - 2];
                for (int i = 0; i < args.Length; ++i)
                    args[i] = arguments[i + 2];
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
                    return Script_Run(command, args);
                }
            }

            Console.WriteLine("*********** EXECUTION FINISHED. ***********");
            Console.WriteLine();
            return null;

            //throw new NotImplementedException("The run method should be implemented in the derived class.");
        }


        #endregion Dummy_Actions


        #region Dummy_Sim

        public override IResponseEvaluatorVectorSimple Simulator
        {
            get
            {
                throw new NotImplementedException("Simulator property not implemented.");
            }
            protected set
            {
                throw new NotImplementedException("Simulator property not implemented.");
            }
        }

        #endregion Dummy_Sim


        #region Dummy_Optimization

        /// <summary>Throws <see cref="NotImplementedException"/>.</summary>
        public override int NumOptimizationParameters
        {
            get
            {
                throw new NotImplementedException("Number of optimization parameters should be implemented in the derived class.");
            }
            protected set
            {
                throw new NotImplementedException("Number of optimization parameters should be implemented in the derived class.");
            }
        }

        /// <summary>Throws <see cref="NotImplementedException"/>.</summary>
        public override int NumOptimizationConstraints
        {
            get
            {
                throw new NotImplementedException("Number of optimization constraints should be implemented in the derived class.");
            }
            protected set
            {
                throw new NotImplementedException("Number of optimization constraints should be implemented in the derived class.");
            }
        }

        /// <summary>Dummy analysis, jsut throws the  exception.</summary>
        /// <param name="anRes"></param><see cref="NotImplementedException"/>
        public override void Analyse(Num.IAnalysisResults anRes)
        {
            throw new NotImplementedException("The analysis method is not implemeted by this class (" + this.GetType().Name + ").");
        }


        #endregion Dummy_Optimization


        #region Dummy_Transformations


        #region Transformations


        /// <summary>Transforms the specified vector of simulation input parameters to the vector of neural 
        /// input parameters and stores the vector to the specified variable.</summary>
        /// <param name="original">Vector to be transformed.</param>
        /// <param name="result">Vector where result of transformation is stored.</param>
        public override void TransfSimulationToNeuralInput(IVector original, ref IVector result)
        {
            lock (Lock)
            {
                throw new NotImplementedException("Transformation from simulation to neural input is not defined.");
            }
        }

        /// <summary>Transforms the specified vector of neural input parameters to the vector of simulation 
        /// input parameters and stores the vector to the specified variable.</summary>
        /// <param name="original">Vector to be transformed.</param>
        /// <param name="result">Vector where result of transformation is stored.</param>
        public override void TransfNeuralToSimulationInput(IVector original, ref IVector result)
        {
            throw new NotImplementedException("Transformation from neural to simulation input is not defined.");
        }

        /// <summary>Transforms the specified vector of simulation output values (results) to the vector of neural 
        /// output values and stores the vector to the specified variable.</summary>
        /// <param name="original">Vector to be transformed.</param>
        /// <param name="result">Vector where result of transformation is stored.</param>
        public override void TransfSimulationToNeuralOutput(IVector original, ref IVector result)
        {
            throw new NotImplementedException("Transformation from simulation to neural output is not defined.");
        }

        /// <summary>Transforms the specified vector of neural output values to the vector of simulation 
        /// output values (results) and stores the vector to the specified variable.</summary>
        /// <param name="original">Vector to be transformed.</param>
        /// <param name="result">Vector where result of transformation is stored.</param>
        public override void TransfNeuralToSimulationOutput(IVector original, ref IVector result)
        {
            throw new NotImplementedException("Transformation from neural to simulation input is not defined.");
        }


        #endregion Transformations



        #endregion Dummy_Transformations


        #region StoredScriptSettings

        /// <summary>In methods of this class you will find all the settings that apply to this script.</summary>
        /// <remarks>Before custom application script is archived, settings should be moved </remarks>
        /// $A Igor Feb12;
        protected new class StoredScriptSettings : ApplicationCommandlineBase
        {

            /// <summary>Creates interpreter - not implemented, just throws <see cref="NotImplementedException"/>.</summary>
            protected override CommandLineApplicationInterpreter CreateInterpreter() { throw new NotImplementedException("Creation of commandline interpreter is not implemented in this class."); }
            public override void TestMain(string[] args) { throw new NotImplementedException(""); }

            /// <summary>Test command method.</summary>
            /// <param name="args">Arguments of the command.</param>
            public void TestMain_Basic(string[] args)
            {
                // Store script settings in this method!
            }
        }

        #endregion StoredScriptSettings

    }

}