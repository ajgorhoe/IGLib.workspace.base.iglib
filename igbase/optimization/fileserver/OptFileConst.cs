// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Constants used in definition of optimization servers and clients
    /// working through file system.</summary>
    /// <remarks>WARNING:
    /// This module is taken from Dragonfly opt. server and adapted for purpose of some
    /// projects. If necessary to further develop, synchronize (and possibly merge) with
    /// Dragonfly, otherwise there will be problems with consistent development of both branches.</remarks>
    /// $A Igor Jul08 Nov09 Mar11;
    static public class OptFileConst
    {

        public const string DirPrefix = "Optimization_Folder";

        // FILE SYSTEM LOCKING:

        /// <summary>Name of the mutex for locking file system in file server operations.</summary>
        public const string LockFileMutexName = "Global\\IG.Lib.OptFileManager.LockFileMutex";

        // DATA EXCHANGE FILES

        /// <summary>Default file name of analysis input file in standard IGLib format.</summary>
        public const string AnInMathFileName = "aninput.dat";
        /// <summary>Default file name of analysis input file in JSON format.</summary>
        public const string AnInJsonFilename = "aninput.json";
        /// <summary>Default file name of analysis input file in XML format.</summary>
        public const string AnInXmlFileName = "aninput.xml";
        /// <summary>Default file name of analysis output file in standard IGLib format.</summary>
        public const string AnOutMathFilename = "anoutput.dat";
        /// <summary>Default file name of analysis output file in JSON format.</summary>
        public const string AnOutJsonFilename = "anoutput.json";
        /// <summary>Default file name of analysis output file in XML format.</summary>
        public const string AnOutXmlFilename = "anoutput.xml";

        // MESSAGE FILES:
        /// <summary>Default file name for analysis busy flag.</summary>
        public const string MsgAnBusyFilename = "anbusy.msg";
        /// <summary>Default file name for analysis input data ready flag.</summary>
        public const string MsgAnInputReadyFilename = "andataready.msg";
        /// <summary>Default file name for analysis results ready flag.</summary>
        public const string MsgAnResultsReadyFilename = "anresultsready.msg";

        /// <summary>Default file name for optimization busy flag.</summary>
        public const string MsgOptBusyFilename = "optbusy.msg";
        /// <summary>Default file name for optimization input data ready flag.</summary>
        public const string MsgOptDataReadyFilename = "optdataready.msg";
        /// <summary>Default file name for optimization resutlts ready flag.</summary>
        public const string MsgOptResultsReadyFilename = "optresultsready.msg";
            
        // INTERFACE WITH PROGRAM Inverse:
        /// <summary>Default file name for optimization command file for program Inverse (Inverse interface).</summary>
        public const string InvOptCommandFilename = "optimization.cm";
        /// <summary>Default file name for analysis command file for program Inverse (Inverse interface).</summary>
        public const string InvAnCommandFilename = "analysis.cm";

    }


}