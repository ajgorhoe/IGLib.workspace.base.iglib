
// INTERFACE WITH OPTIMIZATION SHELL

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

using IG.Lib;
using IG.Num;

namespace IG.Lib
{


    /// <summary>Interface for Inverse Interpreter.</summary>
    /// <remarks>This interface is used for testing, training and demonstration purposes.</remarks>
    /// $A Igor Jul09;
    public class InterfaceInverse : InterfaceInterpreterBase, ILockable
    {


        #region Construction

        public InterfaceInverse(): 
            this("test_command_fiel_name.cm")
        { this.CommandFileName = DefaultCommandFileName; }

        public InterfaceInverse(string commandFileName): 
            this(Directory.GetCurrentDirectory(), commandFileName)
        {  }

        /// <summary>Constructor.</summary>
        /// <param name="workingDirctory">Directory whre command file is written to and working directory
        /// for interpreter's application. Relative or absolute path.</param>
        /// <param name="commandFileName">Name of the command file to which commands are written and which is 
        /// interpreted by the interpreter.</param>
        public InterfaceInverse(string workingDirctory, string commandFileName): 
            this(workingDirctory, workingDirctory, commandFileName)
        {  }

        /// <summary>Constructor.</summary>
        /// <param name="commandDirectory">Directory whre command file is written to. Relative or absolute path.</param>
        /// <param name="startDirectory">Directory in which interpreter application is started. Relative or absolute path.</param>
        /// <param name="commandFileName">Name of the command file to which commands are written and which is 
        /// interpreted by the interpreter.</param>
        public InterfaceInverse(string commandDirectory, string startDirectory, string commandFileName)
        {
            this.CommandDirectory = commandDirectory;
            this.StartingDirectory = startDirectory;
            this.CommandFileName = commandFileName;
            this.CommandIntroduction = "";
            this.CommandArgumentBlockBegin = "{";
            this.CommandArgumentBlockEnd = "}";
            this.CommandArgumentSeparator = ", ";
            this.CommandArgumentsInNewLines = false;
            this.CodeBlockBegin = Environment.NewLine + "{" + Environment.NewLine;
            this.CodeBlockEnd = Environment.NewLine + "}" + Environment.NewLine;
            this.VariableReferenceBegin = "#";
            this.VariableReferenceEnd = "";
            this.IndentationString = "  ";
            this.CommentBegin = "*{ ";
            this.CommentEnd = " } ";

        }


        #endregion Construction

        #region Data

        #region Constants

        /// <summary>String that introduces the analysis block.</summary>
        public virtual string AnalysisBlockName
        { get { return "analysis"; } }

        /// <summary>Command name - interpret.</summary>
        public virtual string CmdNameInterpret
        { get { return "interpret"; } }

        /// <summary>Command name - analyse.</summary>
        public virtual string CmdNameInteractive
        { get { return "interactive"; } }

        /// <summary>Command name - analyse.</summary>
        public virtual string CmdNameWrite
        { get { return "write"; } }

        /// <summary>Command name - analyse.</summary>
        public virtual string CmdNameDWrite
        { get { return "dwrite"; } }

        /// <summary>Command name - setting an integer variable.</summary>
        public virtual string CmdNameSetVarInt
        { get { return "setcounter"; } }

        /// <summary>Command name - setting a double variable.</summary>
        public virtual string CmdNameSetVarDouble
        { get { return "setscalar"; } }

        /// <summary>Command name - setting a boolean variable.</summary>
        public virtual string CmdNameSetVarBoolean
        { get { return "setcounter"; } }

        /// <summary>Command name - setting a string variable.</summary>
        public virtual string CmdNameSetVarString
        { get { return "setstring"; } }

        /// <summary>Command name - setting a vector variable.</summary>
        public virtual string CmdNameSetVarVector
        { get { return "setvector"; } }

        /// <summary>Command name - setting a matrix variable.</summary>
        public virtual string CmdNameSetVarMatrix
        { get { return "setmatrix"; } }

        // STANDARD VARIABLES:

        public virtual string VarNameParamMom
        { get { return "parammom"; } }

        public virtual string VarNameCalcObjective
        { get { return "calcobj"; } }

        public virtual string VarNameCalcConstraints
        { get { return "calcconstr"; } }

        public virtual string VarNameCalcGradObjective
        { get { return "calcgradobj"; } }

        public virtual string VarNameCalcGradConstraints
        { get { return "calcgradconstr"; } }

        public virtual string VarNameObjectiveMom
        { get { return "objectivemom"; } }

        public virtual string VarNameConstraintsMom
        { get { return "constraintmom"; } }

        public virtual string VarNameGradObjectiveMom
        { get { return "gradobjectivemom"; } }

        public virtual string VarNameGradConstraintMom
        { get { return "gradconstraintmom"; } }

        
        // OPTIMIZATION RELATED COMMANDS:

        /// <summary>Command name - analyse.</summary>
        public virtual string CmdNameFileAnalysis
        { get { return "fileanalysis"; } }

        /// <summary>Standard analysis input file name.</summary>
        public virtual string AnalysisInputFileNameStandard
        { get { return OptFileConst.AnInMathFileName; } }

        /// <summary>Standard analysis output file name.</summary>
        public virtual string AnalysisOutputFileNameStandard
        { get { return OptFileConst.AnOutMathFilename; } }

        /// <summary>Command name - analyse.</summary>
        public virtual string CmdNameAnalyse
        { get { return "analyse"; } }

        /// <summary>Command name - taban1d.</summary>
        public virtual string CmdNameTab1d
        { get { return "taban1d"; } }

        /// <summary>Command name - taban2d.</summary>
        public virtual string CmdNameTab2d
        { get { return "taban2d"; } }

        /// <summary>Command name - taban2d.</summary>
        public virtual string CmdNameMinSimp
        { get { return "minsimp"; } }

        /// <summary>Command name - taban2d.</summary>
        public virtual string CmdNameNlpSimp
        { get { return "nlpsimp"; } }

        /// <summary>Command name - taban2d.</summary>
        public virtual string CmdNameNlpSimpBoundConstr
        { get { return "nlpsimpbound0 "; } }

        #endregion Constants

        #endregion Data


        #region Actions


        /// <summary>Appends a boolean value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public override void AppendValue(bool value)
        {
            if (value == true)
                Append("1");
            else
                Append("0");
        }

        /// <summary>Appends a string value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public override void AppendValue(string value)
        {
            Append("\"" + value.ToString() + "\"");
        }

        /// <summary>Appends a vector value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public override void AppendValue(IVector value)
        {
            AppendLine(value.Length.ToString() + " " );
            Append("{ ");
            for (int i = 0; i < value.Length; ++i)
            {
                Append(value[i].ToString() + " ");
            }
            Append("} ");
        }

        /// <summary>Appends a matrix value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public override void AppendValue(IMatrix value)
        {
            AppendLine(value.RowCount.ToString() + " " + value.ColumnCount.ToString() + " ");
            AppendLine("{ ");
            for (int i = 0; i < value.RowCount; ++i)
            {
                AppendIndent();
                for (int j = 0; j < value.ColumnCount; ++j)
                {
                    Append(value[i, j].ToString() + " ");
                }
                AppendLine();
            }
            AppendLine("} ");
        }




        #endregion Actions


        #region SpecificCommands

        /// <summary>Appends beginning of analysis block to the interpreter file contents.</summary>
        public virtual void StartAnalysisBlock()
        {
            AppendLine();
            AppendIndent();
            Append(AnalysisBlockName);
            AppendLine();
            StartCodeBlock();
        }

        /// <summary>Appends end of analysis block to the interpreter file contents.</summary>
        public virtual void EndAnalysisBlock()
        {
            AppendLine();
            EndCodeBlock();
            AppendLine();
        }

        /// <summary>Starts interactive interpreter.</summary>
        /// <param name="message">Message to the user that is written before starting 
        /// an interactive interpreter.</param>
        public virtual void Interactive(string message)
        {
            StartCommand(CmdNameInteractive);
            AppendCommandArgument(message);
            EndCommand();
        }

        /// <summary>Interprets a file.</summary>
        /// <param name="message">Path to the file that will be interpreted.</param>
        public virtual void Interpret(string filePath)
        {
            StartCommand(CmdNameInterpret);
            AppendCommandArgument(filePath);
            EndCommand();
        }

        #region VariableHandling

        /// <summary>Command - sets an integer interpreter variable.</summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public virtual void SetVariable(string variableName, int value)
        {
            StartCommand(CmdNameSetVarInt);
            AppendCommandArgumentSeparatorIfNecessary();
            Append(variableName);
            AppendCommandArgument(value);
            EndCommand();
        }

        /// <summary>Command - sets a double interpreter variable.</summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public virtual void SetVariable(string variableName, double value)
        {
            StartCommand(CmdNameSetVarDouble);
            AppendCommandArgumentSeparatorIfNecessary();
            Append(variableName);
            AppendCommandArgument(value);
            EndCommand();
        }

        /// <summary>Command - sets a boolean interpreter variable.</summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public virtual void SetVariable(string variableName, bool value)
        {
            StartCommand(CmdNameSetVarBoolean);
            AppendCommandArgumentSeparatorIfNecessary();
            Append(variableName);
            AppendCommandArgument(value);
            EndCommand();
        }

        /// <summary>Command - sets a string interpreter variable.</summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public virtual void SetVariable(string variableName, string value)
        {
            StartCommand(CmdNameSetVarString);
            AppendCommandArgumentSeparatorIfNecessary();
            Append(variableName);
            AppendCommandArgument(value);
            EndCommand();
        }

        /// <summary>Command - sets a vector interpreter variable.</summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public virtual void SetVariable(string variableName, IVector value)
        {
            StartCommand(CmdNameSetVarVector);
            AppendCommandArgumentSeparatorIfNecessary();
            Append(variableName);
            AppendCommandArgument(value);
            EndCommand();
        }

        /// <summary>Command - sets a matrix interpreter variable.</summary>
        /// <param name="variableName">Name of the variable.</param>
        /// <param name="value">Value of the variable.</param>
        public virtual void SetVariable(string variableName, IMatrix value)
        {
            StartCommand(CmdNameSetVarMatrix);
            AppendCommandArgumentSeparatorIfNecessary();
            Append(variableName);
            AppendCommandArgument(value);
            EndCommand();
        }

        
        #region PredefinedVAriables

        /// <summary>Sets the current parameters.</summary>
        /// <param name="parameters">Vector containing parameter values.</param>
        public virtual void SetParamMom(IVector parameters)
        {
            SetVariable(VarNameParamMom, parameters);
        }

        /// <summary>Sets the flag for calculation of objective function.</summary>
        /// <param name="flagValue">Value of the flag that is assigned.</param>
        public virtual void SetCalculateObjective(bool flagValue)
        {
            SetVariable(VarNameCalcObjective, flagValue);
        }

        /// <summary>Sets the flag for calculation of constraint functions.</summary>
        /// <param name="flagValue">Value of the flag that is assigned.</param>
        public virtual void SetCalculateConstraints(bool flagValue)
        {
            SetVariable(VarNameCalcConstraints, flagValue);
        }

        /// <summary>Sets the flag for calculation of objective function gradient.</summary>
        /// <param name="flagValue">Value of the flag that is assigned.</param>
        public virtual void SetCalculateGradObjective(bool flagValue)
        {
            SetVariable(VarNameCalcGradObjective, flagValue);
        }

        /// <summary>Sets the flag for calculation of constraint function gradients.</summary>
        /// <param name="flagValue">Value of the flag that is assigned.</param>
        public virtual void SetCalculateGradConstraints(bool flagValue)
        {
            SetVariable(VarNameCalcGradConstraints, flagValue);
        }

        #endregion PredefinedVariables

        #endregion VariableHandling


        /// <summary>Command - runs an external analysis program that exchanges intoermation through files.
        /// Before running the program, it writes analysis input file, and afterwards it reads analysis output.
        /// This command is usually put into anaysis block.</summary>
        /// <param name="analysisCommand">Command that needs to be executed to run external direct analysis
        /// (together with command-line arguments).</param>
        /// <param name="analysisInputPath">Path to the analysis input file (relative paths are NOT converted!).</param>
        /// <param name="analysisOutputPath">Path to the analysis output file (relative paths are NOT converted!).</param>
        public virtual void FileAnalysis(string analysisCommand, string analysisInputPath, string analysisOutputPath)
        {
            StartCommand(CmdNameFileAnalysis);
            AppendCommandArgument(analysisCommand);
            AppendCommandArgument(analysisInputPath);
            AppendCommandArgument(analysisOutputPath);
            EndCommand();
        }

        /// <summary>Command - runs an external analysis program that exchanges intoermation through files.
        /// Before running the program, it writes analysis input file, and afterwards it reads analysis output.
        /// This command is usually put into anaysis block.</summary>
        /// <param name="analysisCommand">Command that needs to be executed to run external direct analysis.</param>
        /// <param name="workingDirectoryPath">Working directory where files are exchanged.
        /// Relative paths are NOT converted to absolute paths before use.</param>
        /// <param name="analysisInputFileName">Name of the analysis input file.</param>
        /// <param name="analysisOutputFileName">Name the analysis output file.</param>
        public virtual void FileAnalysis(string analysisCommand, string workingDirectoryPath,
            string analysisInputFileName, string analysisOutputFileName)
        {
            string analysisInputPath = Path.Combine(workingDirectoryPath, Path.GetFileName(analysisInputFileName));
            string analysisOutputPath = Path.Combine(workingDirectoryPath, Path.GetFileName(analysisOutputFileName));
            FileAnalysis(analysisCommand, analysisInputPath, analysisOutputPath);
        }


        /// <summary>Command - runs an external analysis program that exchanges intoermation through files.
        /// Before running the program, it writes analysis input file, and afterwards it reads analysis output.
        /// This command is usually put into anaysis block.
        /// IMPORTANT: 
        /// It is assumed that analysis program is called as a shell with built-in command-line interpreter,
        /// such that command is invoked by executable name followed by interpreter command followed by
        /// working directory path.
        /// It is alsoassumed that analysis input and output files are exchanged in the working directory.</summary>
        /// <param name="analysisExecutable">Name of the executable.</param>
        /// <param name="analysisCommandName">Name of the interpreter command that invokes direct analysis
        /// that exchanges data through files.</param>
        /// <param name="workingDirectory">Working directory path (relative paths are NOT converted!).</param>
        /// <param name="analysisInputFileName">Name of the analysis input file.</param>
        /// <param name="analysisOutputFileName">Name of the analysis output file.</param>
        public virtual void FileAnalysisStandard(string analysisExecutable, string analysisCommandName,
            string workingDirectory, string analysisInputFileName, string analysisOutputFileName)
        {
            string analysisCommand = analysisExecutable + " " + analysisCommandName + " " 
                + workingDirectory;
            string analysisInputPath = Path.Combine(workingDirectory, Path.GetFileName(analysisInputFileName));
            string analysisOutputPath = Path.Combine(workingDirectory, Path.GetFileName(analysisOutputFileName));
            FileAnalysis(analysisCommand, analysisInputPath, analysisOutputPath);
        }


        /// <summary>Command - runs an external analysis program that exchanges intoermation through files.
        /// Before running the program, it writes analysis input file, and afterwards it reads analysis output.
        /// This command is usually put into anaysis block.
        /// IMPORTANT: 
        /// It is assumed that analysis program is called as a shell with built-in command-line interpreter,
        /// such that command is invoked by executable name followed by interpreter command followed by
        /// working directory path.
        /// It is assumed that analysis input and output files are exchanged in the working directory,
        /// and their names are standard names used by file analysis servers.</summary>
        /// <param name="analysisExecutable">Name of the executable.</param>
        /// <param name="analysisCommandName">Name of the interpreter command that invokes direct analysis
        /// that exchanges data through files.</param>
        /// <param name="workingDirectory">Working directory path (relative paths are NOT converted!).</param>
        public virtual void FileAnalysisStandard(string analysisExecutable, string analysisCommandName,
            string workingDirectory)
        {
            FileAnalysisStandard(analysisExecutable, analysisCommandName,
                workingDirectory, AnalysisInputFileNameStandard, AnalysisOutputFileNameStandard);
        }


        /// <summary>Command - runs a direct analysis at the specified parameters.</summary>
        /// <param name="param">Vector of parameters.</param>
        public virtual void Analyse(IVector param)
        {
            StartCommand(CmdNameAnalyse);
            AppendCommandArgument(param);
            EndCommand();
        }

        /// <summary>Command - runs a direct analysis at the specified parameters, with specified calculation flags.</summary>
        /// <param name="param">Vector of parameters.</param>
        public virtual void Analyse(IVector param, bool calcobj, bool calcconstr, bool calcgradobj, bool calcgradconstr)
        {
            StartCommand(CmdNameAnalyse);
            AppendCommandArgument(param);
            AppendCommandArgument(calcobj);
            AppendCommandArgument(calcconstr);
            AppendCommandArgument(calcgradobj);
            AppendCommandArgument(calcgradconstr);
            EndCommand();
        }

        /// <summary>Runs an 1D table of analyses.</summary>
        /// <param name="pont0">Starting point of the table in the parameter space.</param>
        /// <param name="point1">End point of the table in the parameter space.</param>
        /// <param name="numPoints">Number of analysis points.</param>
        /// <param name="centered">Flag for a centered table. 
        /// If true then the table is centered around the starting point point0.
        /// If table is centered with geometrically growing intervals then the interval lengths first 
        /// fall from point1 reflected over point0 until point0, and then grow from point0 to point1.</param>
        /// <param name="factor">Factor of interval length growth. If 0 or 1 then intervals between table 
        /// points are uniform. If it is greater than 1 then intervals grow in such a way that each 
        /// successive interval length is the previous length multiplied by factor. If it is smaller 
        /// than 1 then factors fall in the same way.</param>
        /// <param name="scaling">Additional scaling factor by which intervals are multiplied. 
        /// The factor can be used e.g. if we want the table extend a bit over some special point 
        /// of interest which we set as endpoint. Regardless of its size, the table remains to be 
        /// centered (if centered is non-zero) or starting in point0.</param>
        public virtual void TabAn1d(IVector pont0, IVector point1, int numPoints, bool centered,
            double factor, double scaling)
        {
            StartCommand(CmdNameTab1d);
            AppendCommandArgument(pont0);
            AppendCommandArgument(point1);
            AppendCommandArgument(numPoints);
            AppendCommandArgument(centered);
            AppendCommandArgument(factor);
            AppendCommandArgument(scaling);
            EndCommand();
        }


        /// <summary>Runs an 1D table of analyses.</summary>
        /// <param name="pont0">Starting point of the table in the parameter space.</param>
        /// <param name="point1">End point of the table in the parameter space.</param>
        /// <param name="numPoints">Number of analysis points.</param>
        /// <param name="centered">Flag for a centered table. 
        /// If true then the table is centered around the starting point point0.
        /// If table is centered with geometrically growing intervals then the interval lengths first 
        /// fall from point1 reflected over point0 until point0, and then grow from point0 to point1.</param>
        /// <param name="factor">Factor of interval length growth. If 0 or 1 then intervals between table 
        /// points are uniform. If it is greater than 1 then intervals grow in such a way that each 
        /// successive interval length is the previous length multiplied by factor. If it is smaller 
        /// than 1 then factors fall in the same way.</param>
        /// <param name="scaling">Additional scaling factor by which intervals are multiplied. 
        /// The factor can be used e.g. if we want the table extend a bit over some special point 
        /// of interest which we set as endpoint. Regardless of its size, the table remains to be 
        /// centered (if centered is non-zero) or starting in point0.</param>
        /// <param name="printTab">Whether data is also printed in table form.</param>
        /// <param name="printParam">Whether a table of parameters in sampled points is printed
        /// together with the corresponding table indices and factors defining relative position with 
        /// respect to point0 and point1.</param>
        /// <param name="printList">If true then data is also printed in list form.</param>
        /// <param name="printObj">Whether objective function value is printed.</param>
        /// <param name="printConstr">Whethe constraint function value is printed.</param>
        /// <param name="printGradobj">Whether gradient of the objective function is printed.</param>
        /// <param name="printGradconstr">Ehether Gradients of constraint functions are printed.</param>
        public virtual void TabAn1d(IVector pont0, IVector point1, int numPoints, bool centered, 
            double factor, double scaling,
            bool printTab, bool printParam, bool printList,
            bool printObj, bool printConstr, bool printGradobj, bool printGradconstr)
        {
            StartCommand(CmdNameTab1d);
            AppendCommandArgument(pont0);
            AppendCommandArgument(point1);
            AppendCommandArgument(numPoints);
            AppendCommandArgument(centered);
            AppendCommandArgument(factor);
            AppendCommandArgument(scaling);
            AppendCommandArgument(printTab);
            AppendCommandArgument(printParam);
            AppendCommandArgument(printList);
            AppendCommandArgument(printObj);
            AppendCommandArgument(printConstr);
            AppendCommandArgument(printGradobj);
            AppendCommandArgument(printGradconstr);
            EndCommand();
        }


        // /// <summary>Runs a 2D table of analyses.</summary>
        /// <param name="pont0">Starting point of the table in the parameter space.</param>
        /// <param name="point1">The first end point of the table, defines the first table 
        /// direction together with point0.</param>
        /// <param name="point2">The second end point of the table, defines the second table 
        /// direction together with point0.</param>
        /// <param name="numPoints1">Number of analysis points (divisions) in the first direction.</param>
        /// <param name="centered1">Flag for a centered table in the first direction. If true then 
        /// the table is centered around the starting point point0. If table is centered with 
        /// geometrically growing intervals then the interval lengths first fall from point1 
        /// reflected over point0 until point0, and then grow from point0 to point1. </param>
        /// <param name="factor1">Factor of interval length growth in the first direction. 
        /// If 0 or 1 then intervals between table points are uniform. If it is greater than 1 
        /// then intervals grow in such a way that each successive interval length is the previous
        /// length multiplied by factor. If it is smaller than 1 then factors fall in the same way.</param>
        /// <param name="scaling1">Additional scaling factor by which intervals are multiplied in 
        /// the first direction. The factor can be used e.g. if we want the table extend a bit over 
        /// some special point of interest which we set as endpoint. Regardless of its size, the 
        /// table remains to be centered (if centered is non-zero) or starting in point0.</param>
        /// <param name="numPoints2">Number of analysis points (divisions) in the second direction.</param>
        /// <param name="centered2">Flag for a centered table in the second direction.</param>
        /// <param name="factor2">Factor of interval length growth in the second direction.</param>
        /// <param name="scaling2">Additional scaling factor by which intervals are multiplied 
        /// in the second direction.</param>
        public virtual void TabAn2d(IVector pont0, IVector point1, IVector point2,
            int numPoints1, bool centered1, double factor1, double scaling1,
            int numPoints2, bool centered2, double factor2, double scaling2)
        {
            StartCommand(CmdNameTab1d);
            AppendCommandArgument(pont0);
            AppendCommandArgument(point1);
            AppendCommandArgument(numPoints1);
            AppendCommandArgument(centered1);
            AppendCommandArgument(factor1);
            AppendCommandArgument(scaling1);
            AppendCommandArgument(numPoints2);
            AppendCommandArgument(centered2);
            AppendCommandArgument(factor2);
            AppendCommandArgument(scaling2);
            EndCommand();
        }

        // /// <summary>Runs a 2D table of analyses.</summary>
        /// <param name="pont0">Starting point of the table in the parameter space.</param>
        /// <param name="point1">The first end point of the table, defines the first table 
        /// direction together with point0.</param>
        /// <param name="point2">The second end point of the table, defines the second table 
        /// direction together with point0.</param>
        /// <param name="numPoints1">Number of analysis points (divisions) in the first direction.</param>
        /// <param name="centered1">Flag for a centered table in the first direction. If true then 
        /// the table is centered around the starting point point0. If table is centered with 
        /// geometrically growing intervals then the interval lengths first fall from point1 
        /// reflected over point0 until point0, and then grow from point0 to point1. </param>
        /// <param name="factor1">Factor of interval length growth in the first direction. 
        /// If 0 or 1 then intervals between table points are uniform. If it is greater than 1 
        /// then intervals grow in such a way that each successive interval length is the previous
        /// length multiplied by factor. If it is smaller than 1 then factors fall in the same way.</param>
        /// <param name="scaling1">Additional scaling factor by which intervals are multiplied in 
        /// the first direction. The factor can be used e.g. if we want the table extend a bit over 
        /// some special point of interest which we set as endpoint. Regardless of its size, the 
        /// table remains to be centered (if centered is non-zero) or starting in point0.</param>
        /// <param name="numPoints2">Number of analysis points (divisions) in the second direction.</param>
        /// <param name="centered2">Flag for a centered table in the second direction.</param>
        /// <param name="factor2">Factor of interval length growth in the second direction.</param>
        /// <param name="scaling2">Additional scaling factor by which intervals are multiplied 
        /// in the second direction.</param>
        /// <param name="printTab">Whether data is also printed in table form.</param>
        /// <param name="printParam">Whether a table of parameters in sampled points is printed
        /// together with the corresponding table indices and factors defining relative position with 
        /// respect to point0 and point1.</param>
        /// <param name="printList">If true then data is also printed in list form.</param>
        /// <param name="printObj">Whether objective function value is printed.</param>
        /// <param name="printConstr">Whethe constraint function value is printed.</param>
        /// <param name="printGradobj">Whether gradient of the objective function is printed.</param>
        /// <param name="printGradconstr">Ehether Gradients of constraint functions are printed.</param>
        public virtual void TabAn2d(IVector pont0, IVector point1, IVector point2, 
            int numPoints1, bool centered1, double factor1, double scaling1,
            int numPoints2, bool centered2, double factor2, double scaling2,
            bool printTab, bool printParam, bool printList,
            bool printObj, bool printConstr, bool printGradobj, bool printGradconstr)
        {
            StartCommand(CmdNameTab1d);
            AppendCommandArgument(pont0);
            AppendCommandArgument(point1);
            AppendCommandArgument(numPoints1);
            AppendCommandArgument(centered1);
            AppendCommandArgument(factor1);
            AppendCommandArgument(scaling1);
            AppendCommandArgument(numPoints2);
            AppendCommandArgument(centered2);
            AppendCommandArgument(factor2);
            AppendCommandArgument(scaling2);
            AppendCommandArgument(printTab);
            AppendCommandArgument(printParam);
            AppendCommandArgument(printList);
            AppendCommandArgument(printObj);
            AppendCommandArgument(printConstr);
            AppendCommandArgument(printGradobj);
            AppendCommandArgument(printGradconstr);
            EndCommand();
        }

        /// <summary>Runs the unconstaint nonlinear (modified Nelder-Mead) simplex minimization algorithm.</summary>
        /// <param name="initialGuess">Initial guess.</param>
        /// <param name="stepSizes">Vector step to construct the initial simplex.</param>
        /// <param name="maxIt">Maximal number of iterations.</param>
        /// <param name="tolX">Tolerance on parameter values.</param>
        /// <param name="tolF">Tolerance on function values.</param>
        /// <param name="printLevel">Level of output produced: 
        ///   1 - data about arguments and optimization results are printed.
        ///   2 – basic information about iterations and more detailed information about results are also printed.
        ///   3 – simplex (co-ordinates of apices and values of the objective function) is also printed during 
        ///   iterations.
        ///   4 – Complete results are printed, included values of the constraint functions.
        ///   5 – at the end, all results of all analyses are also printed. Sets of results in all simplices over al iterations are also printed in the list form readable by Mathematica.
        /// </param>
        public virtual void MinSimplex(IVector initialGuess, IVector stepSizes, int maxIt, IVector tolX, double tolF, int printLevel)
        {
            StartCommand(CmdNameMinSimp);
            AppendCommandArgument(tolX);
            AppendCommandArgument(tolF);
            AppendCommandArgument(maxIt);
            AppendCommandArgument(printLevel);
            AppendCommandArgument(initialGuess);
            AppendCommandArgument(stepSizes);
            EndCommand();
        }

        /// <summary>Runs the constraint nonlinear (modified Nelder-Mead) simplex minimization algorithm.</summary>
        /// <param name="numConstraints">Number of constraint functions.</param>
        /// <param name="initialGuess">Initial guess.</param>
        /// <param name="stepSizes">Vector step to construct the initial simplex.</param>
        /// <param name="maxIt">Maximal number of iterations.</param>
        /// <param name="tolX">Tolerance on parameter values.</param>
        /// <param name="tolF">Tolerance on function values.</param>
        /// <param name="tolConstr">Tolerance for constraint residuum (scalar argument; if it is 0 then none of 
        /// the constraints may be violated in the solution).</param>
        /// <param name="printLevel">Level of output produced: 
        ///   1 - data about arguments and optimization results are printed.
        ///   2 – basic information about iterations and more detailed information about results are also printed.
        ///   3 – simplex (co-ordinates of apices and values of the objective function) is also printed during 
        ///   iterations.
        ///   4 – Complete results are printed, included values of the constraint functions.
        ///   5 – at the end, all results of all analyses are also printed. Sets of results in all simplices over al iterations are also printed in the list form readable by Mathematica.
        /// </param>
        public virtual void NlpSimplex(int numConstraints, IVector initialGuess, IVector stepSizes,
            int maxIt, IVector tolX, double tolF, double tolConstr, int printLevel)
        {
            StartCommand(CmdNameNlpSimp);
            AppendCommandArgument(numConstraints);
            AppendCommandArgument(tolX);
            AppendCommandArgument(tolF);
            AppendCommandArgument(tolConstr);
            AppendCommandArgument(maxIt);
            AppendCommandArgument(printLevel);
            AppendCommandArgument(initialGuess);
            AppendCommandArgument(stepSizes);
            EndCommand();
        }

        /// <summary>Runs the constraint nonlinear (modified Nelder-Mead) simplex minimization algorithm.</summary>
        /// <param name="numConstraints">Number of constraint functions.</param>
        /// <param name="initialGuess">Initial guess.</param>
        /// <param name="stepSizes">Vector step to construct the initial simplex.</param>
        /// <param name="maxIt">Maximal number of iterations.</param>
        /// <param name="tolX">Tolerance on parameter values.</param>
        /// <param name="tolF">Tolerance on function values.</param>
        /// <param name="tolConstr">Tolerance for constraint residuum (scalar argument; if it is 0 then none of 
        /// the constraints may be violated in the solution).</param>
        /// <param name="lowerBounds">Vector of lower bounds on prameters.</param>
        /// <param name="upperBounds">Vector of upper bounds on parameters.</param>
        /// <param name="bigNumber">Large positive value which is used for deciding whether components of lower and 
        /// upper Bounds.</param>
        /// <param name="printLevel">Level of output produced: 
        ///   1 - data about arguments and optimization results are printed.
        ///   2 – basic information about iterations and more detailed information about results are also printed.
        ///   3 – simplex (co-ordinates of apices and values of the objective function) is also printed during 
        ///   iterations.
        ///   4 – Complete results are printed, included values of the constraint functions.
        ///   5 – at the end, all results of all analyses are also printed. Sets of results in all simplices over al iterations are also printed in the list form readable by Mathematica.
        /// </param>
        /// <remarks>Bound constraints are specified by vector arguments lowbounds and upbounds, whose components 
        /// specify lower and upper bounds, respectively, for individual components of the parameter vector.
        /// If for some index the specified lower bound is larger than the corresponding upper bound then it is 
        /// understood that no bounds are defined for this component of the parameter vector.
        /// If absolute value of some component of either lower or upper bound is greater than bignum, 
        /// then it is also assumed that the corresponding bound is not defined (which allows to define for a given 
        /// component of the parameter vector only lower or only upper bound). If there are components of the 
        /// parameter vector for which only lower or only upper bound is defined, then the large positive number 
        /// bignum must be specified such that components of lower or upper bound vectors whose absolute vlue is 
        /// larger than bignum are not taken into account.
        /// bignum can be set to 0. In this case, the default value is taken, but this value can not fit the 
        /// actual problem that is solved.
        /// If lowbounds and upbounds are not specified then the normal nonlinear constraint simplex algorithm 
        /// is performed.</remarks>
        public virtual void NlpSimplexBoundConstr(int numConstraints, IVector initialGuess, IVector stepSizes,
            int maxIt, IVector tolX, double tolF, double tolConstr, 
            IVector lowerBounds, IVector upperBounds, double bigNumber, int printLevel)
        {
            StartCommand(CmdNameNlpSimpBoundConstr);
            AppendCommandArgument(numConstraints);
            AppendCommandArgument(tolX);
            AppendCommandArgument(tolF);
            AppendCommandArgument(tolConstr);
            AppendCommandArgument(maxIt);
            AppendCommandArgument(printLevel);
            AppendCommandArgument(initialGuess);
            AppendCommandArgument(stepSizes);
            if (lowerBounds != null)
            {
                AppendCommandArgument(lowerBounds);
                if (upperBounds != null)
                {
                    AppendCommandArgument(upperBounds);
                    if (bigNumber != 0)
                        AppendCommandArgument(bigNumber);
                }
            }
            EndCommand();
        }



        #endregion SpecificCommands




    }  // class InterfaceInverse



}

