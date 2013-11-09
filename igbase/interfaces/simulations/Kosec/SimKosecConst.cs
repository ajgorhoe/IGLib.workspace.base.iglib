// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{


    /// <summary>Constants for (optimization) interface with the simulation code of Gregor Kosec.</summary>
    /// $A Igor Aug11;
    public static class SimKosecConst
    {

        #region Specific

        public const string ExecutableNameConvectionObstacles = "CavityProblem.exe";

        #endregion Specific



        #region OperationData

       /// <summary>Default separator used in CSV files.</summary>
        public const string CsvSeparatorDefault = ";";

       /// <summary>Comment introduction string (for 1 line comments).</summary>
        public const string CommentLineString = "-------- ";

        /// <summary>Default name for template for input file. In optimization, 
        /// this file is read, modified according to parameters, and stored as input 
        /// file before simulation is run.</summary>
        public const string TemplateInputFileNameDefault = "template.par";

        /// <summary>Default name of the simulation input file.</summary>
        public const string InputFileNameDefault = "default.par";

        /// <summary>Default name of executable.</summary>
        public const string ExecutableFilenameDefault = "CavityProblem.exe";

        /// <summary>Default name of the base directory for output files.</summary>
        public const string OutputDirectortyBaseDefault = "RawData";

        /// <summary>Name of the file where optimization results are found.</summary>
        public const string OptimizationOutputFileName = "optim.csv";

 
        #endregion OperationData



    }
}
