
// NEURAL NETWORKS APPROXIMATOR'S FILE SERVER

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Num;

using IG.Neural;

namespace IG.Neural
{


    //public abstract class AnalysisFileServerNeuralBase : ApproximationFileServerNeural
    //{

 
    //    #region construction

 

    //    /// <summary>Constructor, sets object's working directory to the specified path.</summary>
    //    /// <param name="workingDirectoryPath"></param>
    //    public AnalysisFileServerNeuralBase(string workingDirectoryPath): base(workingDirectoryPath)
    //    { this.Directory = workingDirectoryPath; }

    //    #endregion Construction


    //    #region Data

    //    private string _directory;

    //    ///// <summary>Gets or sets working directory of the current object.</summary>
    //    //public virtual string Directory
    //    //{
    //    //    get { return _directory; }
    //    //    protected set 
    //    //    { 
    //    //        _directory = Path.GetFullPath(value);
    //    //        ApproximationServer = new ApproximationFileServerNeural(Directory);
    //    //    }
    //    //}

    //    //private ApproximationFileServerNeural _approximationServer;

    //    //public ApproximationFileServerNeural ApproximationServer
    //    //{
    //    //    get { return _approximationServer; }
    //    //    protected set { _approximationServer = null; }
    //    //}

    //    #endregion Data

    //    #region Operation 

        
    //    //#region Specific_Approximation

    //    ///// <summary>Converts analysis input parameters to approximation input parameters.</summary>
    //    ///// <param name="anInput">Input parameters for direct analysis.</param>
    //    ///// <param name="approxInput">Input parameters for response approximation.</param>
    //    //protected abstract void AnalysisToApproximationInput(IVector anInput, ref IVector approxInput);
        
    //    ///// <summary>Converts approximation output to direct analysis results.</summary>
    //    ///// <param name="approxOuptut">Vector of approximated output values.</param>
    //    ///// <param name="anResults">Direct analysis results.</param>
    //    //protected abstract void ApproximationToAnalysisOutput(IVector approxOuptut, AnalysisResults anResults);

    //    //#endregion Specific_Approximation


    //    /// <summary>Client application for calculationg the approximation.
    //    /// Writes analysis input, runs the CalculateApproximation method, and reads the results.</summary>
    //    /// <param name="anRes">Analysis reuest object.</param>
    //    public virtual void CalculatAnalysisResultsClientApp(IAnalysisResults analysisRequest)
    //    {


    //        throw new NotImplementedException();
    //    }

    //    /// <summary>Performs calculation of the approximation.
    //    /// Reads input from standard location, calculates output and writes it to the standard location.</summary>
    //    public virtual void CalculateAnalysisResults()
    //    {


    //        throw new NotImplementedException();
    //    }

    //    /// <summary>Performs calculation of the approximation.</summary>
    //    /// <param name="anResults">Analysis results structure, contains analysis input parameters & calculation flagswhen 
    //    /// function is called, and analysis results after function returns.</param>
    //    public virtual void CalculateAnalysisResults(AnalysisResults anResults)
    //    {


    //        throw new NotImplementedException();
    //    }

    //    #endregion Operation

    //}



    /// <summary>Class containing direct analysis (in optimization) based on neural network optimization.</summary>
    public class AnalysisFileServerNeural : ApproximationFileServerNeural , // AnalysisFileServerNeuralBase
        IAnalysis
    {
        
 
        #region construction

        /// <summary>Constructor, sets object's working directory to the specified path.</summary>
        /// <param name="workingDirectoryPath"></param>
        public AnalysisFileServerNeural(string workingDirectoryPath): base(workingDirectoryPath)
        {
            OptimizationFileManager = new OptFileManager(workingDirectoryPath);
            OptimizationFileManager.Analysis = this;
        }

        #endregion Construction

        
        #region Data


        private OptFileManager _optFileManager;

        public OptFileManager OptimizationFileManager
        {
            get { return _optFileManager; }
            set { _optFileManager = value; }
        }
 

        #endregion Data

        #region IAnslysis

        /// <summary>Performs analysis - calculates requested results and writes them
        /// to the provided data structure.</summary>
        /// <param name="analysisData">Data structure where analysis request parameters are
        /// obtained and where analysis results are written.</param>
        public virtual void Analyse(IAnalysisResults analysisData)
        {
            IVector param = analysisData.Parameters;
            if (param==null)
                throw new ArgumentNullException("Analysis: Vector of parameters is null.");
            IVector approxInput = null;
            IVector approxOutput = null;
            AnalysisToApproximationInput(param, ref approxInput);
            NeuralFileManager.ClientWriteNeuralInput(approxInput);
            NeuralFileManager.ServerCalculateApproximation();
            NeuralFileManager.ClientReadNeuralOutput(ref approxOutput);
            ApproximationToAnalysisOutput(approxOutput, analysisData);

        }

        /// <summary>Number of parameters.</summary>
        public int NumParameters { get {return 0;}  set { } }

        /// <summary>Number of objective functions (normally 1 for this type, but can be 0).</summary>
        public int NumObjectives { get { return 0; } set { } }

        /// <summary>Number of constraints.</summary>
        public int NumConstraints { get { return 0; } set { } }

        /// <summary>Number of equality constraints.</summary>
        public int NumEqualityConstraints { get { return 0; } set { } }

        #endregion IAnalysis

        #region Specific_Analysis



        /// <summary>Converts analysis input parameters to approximation input parameters.</summary>
        /// <param name="anInput">Input parameters for direct analysis.</param>
        /// <param name="approximationInput">Input parameters for response approximation.</param>
        protected virtual void AnalysisToApproximationInput(IVector anInput, ref IVector approximationInput)
        {
            Vector.Copy(anInput, ref approximationInput);
        }

        /// <summary>Converts approximation output to direct analysis results.<br/>
        /// Sets objective function to sum of squares of approximated values.<br/>
        /// WARNING: <br/>
        /// This method should be overridden in derived classes.</summary>
        /// <param name="approxOuptut">Vector of approximated output values.</param>
        /// <param name="anResults">Direct analysis results.</param>
        protected virtual void ApproximationToAnalysisOutput(IVector approxOuptut, IAnalysisResults anResults)
        {
            if (approxOuptut == null)
                throw new ArgumentNullException("Vector of approximation output values is not specified (null reference).");
            if (anResults == null)
                throw new ArgumentNullException("Analysis results structure is not specified (null reference).");
            if (anResults.ReqObjective)
            {
                
                double objective = 0.0;
                for (int i = 0; i < approxOuptut.Length; ++i)
                    objective += approxOuptut[i] * approxOuptut[i];
                anResults.CalculatedObjective = true;
            }
        }

        #endregion Specific_Analysis


        #region Operation 

        
        /// <summary>Clears all messages in the neural network approximator's file client/server directory.</summary>
        public override void ClearMessages()
        {
            base.ClearMessages();
            NeuralFileManager.ClearMessages();
        }


        /// <summary>Performs server-side direct analysis.
        /// Reads analysis input from standard location, calculates output and writes it to the standard location.</summary>
        public virtual void ServerAnalyse()
        {
            this.OptimizationFileManager.ServerAnalyse();
        }

  
        /// <summary>Calculates analysis results by using the analysis server.</summary>
        /// <param name="inputParameters">Intput parameters for which approximation is calculated.</param>
        /// <param name="outputValues">Vector where approximation output values are stored.</param>
        public virtual void ClientCalculateAnalysisResults(AnalysisResults anRes)
        {
            this.OptimizationFileManager.ClientCalculateAnalysisResults(ref anRes);
        }

        /// <summary>Performs client-side test calculation of analysis response.</summary>
        /// <param name="inputFilePath">Path to the JSON file where input parameters are read from.
        /// The file pointed at must exist.</param>
        /// <param name="reqObjective">Flag indicating whether objective function must be calculated.</param>
        /// <param name="reqConstraints">Flag indicating whether constraint functions must be calculated.</param>
        /// <param name="reqGradObjective">Flag indicating whether objective function gradientmust be calculated.</param>
        /// <param name="reqGradOConstraints">Fleg indicating whether constraint function gradients must be calculated.</param>
        /// <param name="outputFilePath">Path of a file where the calculated analysis response in JSON is written to.
        /// It can be null or empty string, in this case response is not written to a file (but it is 
        /// output on console).</param>
        public virtual void ClientTestCalculateAnalysisResults(string inputFilePath,
            bool reqObjective, bool reqConstraints, bool reqObjectiveGradient, bool reqConstraintGradients,
            string outputFilePath)
        {
            this.OptimizationFileManager.ClientTestCalculateAnalysisResults(inputFilePath,
                reqObjective, reqConstraints, reqObjectiveGradient, reqConstraintGradients,
                outputFilePath);
        }



        #endregion Operation



    }  // AnalysisFileServerNeural





}