
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

    /// <summary>Class for approximation file server.</summary>
    /// $A Igor Apr11;
    public class ApproximationFileServerNeural  // : ApproximationFileServerBase
    {
        private ApproximationFileServerNeural() {  }

        public ApproximationFileServerNeural(string workingDirectoryPath)
        // : base(workingDirectoryPath)
        {
            this.Directory = workingDirectoryPath;
            this.NeuralFileManager = new NeuraApproximationFileManager(workingDirectoryPath);
        }


        #region Data


        private string _directory;

        /// <summary>Gets or sets working directory of the current object.</summary>
        public virtual string Directory
        {
            get { return _directory; }
            protected set { 
                _directory = Path.GetFullPath(value); 
            }
        }

        private NeuraApproximationFileManager _neuralFileManager;
        
        public NeuraApproximationFileManager NeuralFileManager
        {
            get { return _neuralFileManager; }
            set { _neuralFileManager = value; }
        }


        #endregion Data


        #region Operation 

 
        /// <summary>Clears all messages in the neural network approximator's file client/server directory.</summary>
        public virtual void ClearMessages()
        {
            NeuralFileManager.ClearMessages();
        }


        /// <summary>Performs server-side calculation of the approximation.
        /// Reads input from standard location, calculates output and writes it to the standard location.</summary>
        public virtual void ServerCalculateApproximation()
        {
            NeuralFileManager.ServerCalculateApproximation();
        }


        /// <summary>Calculates the neural network based approximated values.</summary>
        /// <param name="input">Vector of input parameters.</param>
        /// <param name="output">Vector of output parameters.</param>
        public virtual void ClientCalculateApproximation(IVector input, ref IVector output)
        {
            NeuralFileManager.ClientCalculateApproximation(input, ref output);
        }

        
        /// <summary>Performs client-side test calculation of neural network based approximation where
        /// input parameters are read from a specified JSON file, and calculated output values are written 
        /// to the specified file.</summary>
        /// <param name="inputFilePath">Path to the JSON file where input parameters are read from.
        /// The file pointed at must exist.</param>
        /// <param name="outputFilePath">Path of a file where the calculated approximated values are written to.
        /// It can be null or empty string, in this case parameters are not written to a file (but they are still 
        /// output on console).</param>
        public virtual void ClientTestCalculateApproximation(string inputFilePath, string outputFilePath)
        {
            NeuralFileManager.ClientTestCalculateApproximation(inputFilePath, outputFilePath);
        }
        

        #endregion Operation

    }


    /// <summary>Class for mapping file server.</summary>
    /// $A tako78 Jul.21
    public class MappingApproximationFileServerNeural : ApproximationFileServerNeural
    {

        public MappingApproximationFileServerNeural(string workingDirectoryPath)
            : base(workingDirectoryPath)
        {
            this.Directory = workingDirectoryPath;
            this.MappingFileManager = new MappingApproximationFileManager(workingDirectoryPath);
        }


        #region Data


        private MappingApproximationFileManager _mappingFileManager;

        public MappingApproximationFileManager MappingFileManager
        {
            get { return _mappingFileManager; }
            set { _mappingFileManager = value; }
        }


        #endregion Data


        #region Operation


        /// <summary>Clears all messages in the nmapping approximator's file client/server directory.</summary>
        /// $A tako78 Jul.21
        public virtual void ClearMessages()
        {
            MappingFileManager.ClearMessages();
        }


        /// <summary>Performs server-side mapping and calculation of the approximation.
        /// Reads reduced input from standard location, mapps inputs, calculates output, mapps outputs 
        /// and writes it to the standard location.</summary>
        /// $A tako78 Jul.21
        public virtual void ServerCalculateMappingApproximation()
        {
            MappingFileManager.ServerCalculateMappingApproximation();
        }


        /// <summary>Performs client-side test calculation of neural network based approximation where
        /// input parameters are read from a specified function JSON file with reduced input parameters, 
        /// copies to specified JSON file with total inputs and calculated output values are written 
        /// to the specified file.</summary>
        /// <param name="inputFilePath">Path to the JSON file where input parameters are read from.
        /// The file pointed at must exist.</param>
        /// <param name="outputFilePath">Path of a file where the calculated approximated values are written to.
        /// It can be null or empty string, in this case parameters are not written to a file (but they are still 
        /// output on console).</param>
        /// $A tako78 Jul.21
        public virtual void ClientTestCalculateMappingApproximation(string inputFilePath, string outputFilePath)
        {
            MappingFileManager.ClientTestCalculateMappingApproximation(inputFilePath, outputFilePath);
        }


        #endregion

    } // class MappingApproximationFileServerNeural
}
