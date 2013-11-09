
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


    /// <summary>Base class for interfaces with interpreters.</summary>
    /// $A Igor Jul09;
    public class InterfaceInterpreterBase: ILockable
    {

        #region Construction

        public InterfaceInterpreterBase(): 
            this("test_command_fiel_name.cm")
        { this.CommandFileName = DefaultCommandFileName; }

        public InterfaceInterpreterBase(string commandFileName): 
            this(Directory.GetCurrentDirectory(), commandFileName)
        {  }

        /// <summary>Constructor.</summary>
        /// <param name="workingDirctory">Directory whre command file is written to and working directory
        /// for interpreter's application. Relative or absolute path.</param>
        /// <param name="commandFileName">Name of the command file to which commands are written and which is 
        /// interpreted by the interpreter.</param>
        public InterfaceInterpreterBase(string workingDirctory, string commandFileName): 
            this(workingDirctory, workingDirctory, commandFileName)
        {  }

        /// <summary>Constructor.</summary>
        /// <param name="commandDirectory">Directory whre command file is written to. Relative or absolute path.</param>
        /// <param name="startDirectory">Directory in which interpreter application is started. Relative or absolute path.</param>
        /// <param name="commandFileName">Name of the command file to which commands are written and which is 
        /// interpreted by the interpreter.</param>
        public InterfaceInterpreterBase(string commandDirectory, string startDirectory, string commandFileName)
        {
            this.CommandDirectory = commandDirectory;
            this.StartingDirectory = startDirectory;
            this.CommandFileName = commandFileName;
            this.CommandIntroduction = "";
            this.CommandArgumentBlockBegin = "(" ;
            this.CommandArgumentBlockEnd = ")";
            this.VariableReferenceBegin = "$";
            this.VariableReferenceEnd = "";
            this.CommandArgumentSeparator = ", " ;
            this.CommandArgumentsInNewLines = false;
            this.CodeBlockBegin = Environment.NewLine + "{" + Environment.NewLine;
            this.CodeBlockEnd = Environment.NewLine + "}" + Environment.NewLine;
            this.IndentationString = "  ";
            this.CommentBegin = "/* ";
            this.CommentEnd = " */ ";

        }


        #endregion Construction


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking

        #region Data

        private StringBuilder _commandBuilder = new StringBuilder();

        private string _interpreterCommand;

        private string _commandDirectory;

        private string _startingDirectory;

        private string _commandFileName;

        // SYNTAX:

        private string _commandIndroduction = Environment.NewLine;
        private string _commandArgumentBlockBegin = " ( " + Environment.NewLine + "  ";
        private string _commandArgumentBlockEnd = " ) " + Environment.NewLine;
        private string _commandArgumentSeparator = ", " ;
        private bool _commandArgumentsInNewLines = true;
        private int _commandArgumentCount = 0;
        private bool _isWithinArgumentBlock = false;
        private string _codeBlockBegin = Environment.NewLine + "{" + Environment.NewLine;
        private string _codeBlockEnd = Environment.NewLine + "}" + Environment.NewLine;
        private string _variableReferenceBegin = "$";
        private string _variableReferenceEnd = "";
        private string _indentationString = "  ";
        private int _indentationLevel = 0;
        private string _commentBegin = "/* ";
        private string _commentEnd = " */ ";

        /// <summary>Command builder.</summary>
        public virtual StringBuilder CommandBuilder
        {
            get { lock (Lock) { return _commandBuilder; } }
            protected set
            {
                lock (Lock)
                {
                    if (value == null)
                        throw new ArgumentNullException("New command builde is not specified.");
                    _commandBuilder = value;
                }
            }
        }

        /// <summary>Gets current state of commands as stirng.</summary>
        public virtual string CommandFileString
        {
            get { lock (Lock) { return CommandBuilder.ToString(); } }
        }


        /// <summary>Gets or sets the command that invokes the interpreter.
        /// This may be a full path to the executable, or only executable name if the
        /// executable is located in a directory included in path.</summary>
        public virtual string InterpreterCommand
        {
            get { lock(Lock) { return _interpreterCommand; } }
            protected set
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("Interperter invocation command is not specified.");
                    _interpreterCommand = value;
                }
            }
        }


        /// <summary>Gets or sets the directory where command file will be located.</summary>
        public virtual string CommandDirectory
        {
            get { lock (Lock) { return _commandDirectory; } }
            protected set
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("Command directory is not specified.");
                    string fullPath = Path.GetFullPath(value);
                    _commandDirectory = value;
                }
            }
        }


        /// <summary>Gets the default command file name for the current interpreter.</summary>
        public virtual string DefaultCommandFileName
        {
            get { return "interpreter_test_command_file.cm"; }
        }

        /// <summary>Gets or sets the command file name where interpreter commands will be written.</summary>
        public virtual string CommandFileName
        {
            get { lock(Lock) { return _commandFileName; } }
            protected set
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("Interperter's command file name is not specified.");
                    _commandFileName = value;
                }
            }
        }

        /// <summary>Gets path to command file.</summary>
        public virtual string CommandFilePath
        {
            get {
                lock (Lock)
                {
                    string path = CommandDirectory;
                    if (string.IsNullOrEmpty(path))
                        throw new InvalidDataException("Command directory not specified.");
                    return Path.Combine(path, CommandFileName);
                }
            }
        }

        /// <summary>Gets or sets the command that invokes the interpreter.</summary>
        public virtual string StartingDirectory
        {
            get { lock(Lock) { return _startingDirectory; } }
            protected set 
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentException("Starting directory for interpreter application is not specified.");
                    string fullPath = Path.GetFullPath(value);
                    _startingDirectory = fullPath;
                }
            }
        }


        /// <summary>String that introduces a new command (and is written before the command name in the 
        /// command file). Usually a newline.</summary>
        public virtual string CommandIntroduction
        {
            get { lock (Lock) { return _commandIndroduction; } }
            protected set { lock (Lock) { _commandIndroduction = value; } }
        }


        /// <summary>String that begins command arguments block.</summary>
        public virtual string CommandArgumentBlockBegin
        {
            get { lock(Lock) { return _commandArgumentBlockBegin; } }
            protected set { lock (Lock) { _commandArgumentBlockBegin = value; } }
        }

        /// <summary>String that ends command argument block.</summary>
        public virtual string CommandArgumentBlockEnd
        {
            get { lock(Lock) { return _commandArgumentBlockEnd; } }
            protected set { lock (Lock) { _commandArgumentBlockEnd = value; } }
        }

        /// <summary>String that begins command arguments block.</summary>
        public virtual string CommandArgumentSeparator
        {
            get { lock (Lock) { return _commandArgumentSeparator; } }
            protected set { lock (Lock) { _commandArgumentSeparator = value; } }
        }

        /// <summary>Whether command arguments are listed in separate lines or not.</summary>
        public virtual bool CommandArgumentsInNewLines
        {
            get { lock (Lock) { return _commandArgumentsInNewLines; } }
            set { lock (Lock) { _commandArgumentsInNewLines = value; } }
        }


        /// <summary>The number of current argument.</summary>
        public virtual int CommandArgumentCount
        {
            get { lock (Lock) { return _commandArgumentCount; } }
            protected set { lock (Lock) { _commandArgumentCount = value; } }
        }

        /// <summary>The number of current argument.</summary>
        public virtual bool IsWithinArgumentBlock
        {
            get { lock (Lock) { return _isWithinArgumentBlock; } }
            protected set { lock (Lock) { _isWithinArgumentBlock = value; } }
        }

        /// <summary>String that begins a code block in interpreted files.</summary>
        public virtual string CodeBlockBegin
        {
            get { lock (Lock) { return _codeBlockBegin; } }
            protected set { lock (Lock) { _codeBlockBegin = value; } }
        }

        /// <summary>String that ends a code block in interpreted files.</summary>
        public virtual string CodeBlockEnd
        {
            get { lock (Lock) { return _codeBlockEnd; } }
            protected set { lock (Lock) { _codeBlockEnd = value; } }
        }

        /// <summary>String that begins a code block in interpreted files.</summary>
        public virtual string VariableReferenceBegin
        {
            get { lock (Lock) { return _variableReferenceBegin; } }
            protected set { lock (Lock) { _variableReferenceBegin = value; } }
        }

        /// <summary>String that ends a code block in interpreted files.</summary>
        public virtual string VariableReferenceEnd
        {
            get { lock (Lock) { return _variableReferenceEnd; } }
            protected set { lock (Lock) { _variableReferenceEnd = value; } }
        }

        /// <summary>String used for indentation of code.</summary>
        public virtual string IndentationString
        {
            get { lock (Lock) { return _indentationString; } }
            set { lock (Lock) { _indentationString = value; } }
        }

        /// <summary>Indentation level - how many indentation strings are added before each new row.</summary>
        public virtual int IndentationLevel
        {
            get { lock (Lock) { return _indentationLevel; } }
            protected set { lock (Lock) { _indentationLevel = value; } }
        }

        /// <summary>String that begins a comment.</summary>
        public virtual string CommentBegin
        {
            get { lock (Lock) { return _commentBegin; } }
            protected set { lock (Lock) { _commentBegin = value; } }
        }

        /// <summary>String that ends a comment.</summary>
        public virtual string CommentEnd
        {
            get { lock (Lock) { return _commentEnd; } }
            protected set { lock (Lock) { _commentEnd = value; } }
        }


        #endregion Data


        #region Actions


        #region CommandComposition

        public virtual void ResetCommands()
        {
            CommandBuilder.Length = 0;
        }

        /// <summary>Appends a literal string to the contents of the command file plus a newline character.</summary>
        /// <param name="str">String to be appended.</param>
        public virtual void Append(string str)
        {
            lock (Lock)
            {
                CommandBuilder.Append(str);
            }
        }

        /// <summary>Appends a literal string to the contents of the command file.</summary>
        /// <param name="str">String to be appended.</param>
        public virtual void AppendLine(string str)
        {
            this.Append(str + Environment.NewLine);
        }

        public virtual void AppendLine()
        {
            this.Append(Environment.NewLine);
        }
        
        /// <summary>Appends indentation to the contents of the command file.</summary>
        public virtual void AppendIndent()
        {
            lock(Lock)
            {
                for (int i=0; i<IndentationLevel; ++i)
                    Append(IndentationString);
            }
        }
        
        /// <summary>Appends a vomment to the contents of the command file.</summary>
        /// <param name="str">Comment text.</param>
        /// <param name="withNewlines">Whether comment is appended in a separate line.</param>
        public virtual void AppendComment(string str, bool withNewlines)
        {
            lock(Lock)
            {
                if (withNewlines)
                {
                    AppendLine();
                    AppendIndent();
                }
                Append(CommentBegin);
                Append(str);
                Append(CommentEnd);
                if (withNewlines)
                {
                    AppendLine();
                }
            }
        }

        /// <summary>Starts a new code block in the interpreter command file contents.</summary>
        public virtual void StartCodeBlock()
        {
            lock (Lock)
            {
                AppendLine();
                AppendIndent();
                Append(CodeBlockBegin);
                AppendLine();
                ++IndentationLevel;
            }
        }

        /// <summary>Ends a code block in the interpreter command file contents.</summary>
        public virtual void EndCodeBlock()
        {
            lock (Lock)
            {
                AppendLine();
                AppendIndent();
                Append(CodeBlockEnd);
                AppendLine();
                --IndentationLevel;
            }
        }

        /// <summary>Starts a new command in the interpreter command file contents.</summary>
        /// <param name="commandName">Name of the command.</param>
        public virtual void StartCommand(string commandName)
        {
            lock (Lock)
            {
                try
                {
                    CommandArgumentCount = 0;
                    AppendLine();
                    AppendIndent();
                    Append(CommandIntroduction);
                    Append(commandName);
                    if (CommandArgumentsInNewLines)
                    {
                        AppendLine();
                        AppendIndent();
                    }
                    Append(CommandArgumentBlockBegin);
                    if (CommandArgumentsInNewLines)
                        AppendLine();
                }
                catch { throw; }
                finally
                {
                    ++IndentationLevel;
                    IsWithinArgumentBlock = true;
                }
            }
        }

        /// <summary>Ends (finalizes) a command in the interpreter command file contents.</summary>
        public virtual void EndCommand()
        {
            lock (Lock)
            {
                try
                {
                    CommandArgumentCount = 0;
                    if (CommandArgumentsInNewLines)
                    {
                        AppendLine();
                        AppendIndent();
                    }
                    Append(CommandArgumentBlockEnd);
                    if (CommandArgumentsInNewLines)
                        AppendLine();
                    CommandArgumentCount = 0;
                }
                catch { throw; }
                finally
                {
                    --IndentationLevel;
                    IsWithinArgumentBlock = false;
                }
            }
        }

        /// <summary>Appends a command line separator to the interpreter command file contents,
        /// if necessary (i.e. if any arguments have already been written to the command file contents).</summary>
        public virtual void AppendCommandArgumentSeparatorIfNecessary()
        {
            lock (Lock)
            {
                if (CommandArgumentCount > 0 && IsWithinArgumentBlock)
                {
                    Append(CommandArgumentSeparator);
                    if (CommandArgumentsInNewLines)
                    {
                        AppendLine();
                        AppendIndent();
                    }
                }
            }
        }

        /// <summary>Appends an integer value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public virtual void AppendValue(int value)
        {
            Append(value.ToString());
        }

        /// <summary>Appends a double value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public virtual void AppendValue(double value)
        {
            Append(value.ToString());
        }

        /// <summary>Appends a boolean value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public virtual void AppendValue(bool value)
        {
            Append(value.ToString());
        }

        /// <summary>Appends a string value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public virtual void AppendValue(string value)
        {
            Append("\"" + value.ToString() + "\"");
        }

        /// <summary>Appends a vector value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public virtual void AppendValue(IVector value)
        {
            throw new InvalidOperationException("Appending a vector value to the command file is not yet implemented.");
            // Append(value.ToStringMath());
        }

        /// <summary>Appends a matrix value to the interpreter command file contents.</summary>
        /// <param name="value">Value to be appended.</param>
        public virtual void AppendValue(IMatrix value)
        {
            throw new InvalidOperationException("Appending a matrix value to the command file is not yet implemented.");
            // Append(value.ToStringMath());
        }


        /// <summary>Appends a variable reference to the contents of the interpreter command file.</summary>
        /// <param name="value">Name of the variable whose reference is appended.</param>
        protected virtual void AppendCommandVariableReference(string variableName)
        {
            lock (Lock)
            {
                Append(VariableReferenceBegin);
                Append(variableName);
                Append(VariableReferenceEnd);
            }
        }

        /// <summary>Appends a variable reference to the current argument block.</summary>
        /// <param name="value">Name of the variable whose reference is appended.</param>
        public virtual void AppendCommandArgumentVariableReference(string variableName)
        {
            lock (Lock)
            {
                AppendCommandArgumentSeparatorIfNecessary();
                AppendCommandVariableReference(variableName);
            }
        }

        /// <summary>Appends an integer argument to the command argument block of the interpreter command file.</summary>
        /// <param name="value">Value of the argument that is appended.</param>
        public virtual void AppendCommandArgument(int value)
        {
            lock (Lock)
            {
                if (!IsWithinArgumentBlock)
                    throw new ArgumentException("Can not append a command argument: not within argument block.");
                AppendCommandArgumentSeparatorIfNecessary();
                AppendValue(value);
            }
        }


        /// <summary>Appends a double argument to the command argument block of the interpreter command file.</summary>
        /// <param name="value">Value of the argument that is appended.</param>
        public virtual void AppendCommandArgument(double value)
        {
            lock (Lock)
            {
                if (!IsWithinArgumentBlock)
                    throw new ArgumentException("Can not append a command argument: not within argument block.");
                AppendCommandArgumentSeparatorIfNecessary();
                AppendValue(value);
            }
        }

        /// <summary>Appends a boolean argument to the command argument block of the interpreter command file.</summary>
        /// <param name="value">Value of the argument that is appended.</param>
        public virtual void AppendCommandArgument(bool value)
        {
            lock (Lock)
            {
                if (!IsWithinArgumentBlock)
                    throw new ArgumentException("Can not append a command argument: not within argument block.");
                AppendCommandArgumentSeparatorIfNecessary();
                AppendValue(value);
            }
        }

        /// <summary>Appends a string argument to the command argument block of the interpreter command file.</summary>
        /// <param name="value">Value of the argument that is appended.</param>
        public virtual void AppendCommandArgument(string value)
        {
            lock (Lock)
            {
                if (!IsWithinArgumentBlock)
                    throw new ArgumentException("Can not append a command argument: not within argument block.");
                AppendCommandArgumentSeparatorIfNecessary();
                AppendValue(value);
            }
        }

        /// <summary>Appends a plain string argument (without decoration such as double quotes) to the command argument block of the interpreter command file.</summary>
        /// <param name="value">Value of the argument that is appended.</param>
        public virtual void AppendCommandArgumentPlain(string value)
        {
            lock (Lock)
            {
                if (!IsWithinArgumentBlock)
                    throw new ArgumentException("Can not append a command argument: not within argument block.");
                AppendCommandArgumentSeparatorIfNecessary();
                Append(value);
            }
        }

        /// <summary>Appends a vector argument to the command argument block of the interpreter command file.</summary>
        /// <param name="value">Value of the argument that is appended.</param>
        public virtual void AppendCommandArgument(IVector value)
        {
            lock (Lock)
            {
                if (!IsWithinArgumentBlock)
                    throw new ArgumentException("Can not append a command argument: not within argument block.");
                AppendCommandArgumentSeparatorIfNecessary();
                AppendValue(value);
            }
        }

        /// <summary>Appends a matrix argument to the command argument block of the interpreter command file.</summary>
        /// <param name="value">Value of the argument that is appended.</param>
        public virtual void AppendCommandArgument(IMatrix value)
        {
            lock (Lock)
            {
                if (!IsWithinArgumentBlock)
                    throw new ArgumentException("Can not append a command argument: not within argument block.");
                AppendCommandArgumentSeparatorIfNecessary();
                AppendValue(value);
            }
        }

        #endregion CommandComposition


        /// <summary>Writes the current contents of the interpreter command file to the physical file.
        /// After this method is called, ResetCommands() should be called if the object will be further used.</summary>
        /// <param name="appendContents">Whether file contents are appended.</param>
        protected virtual void WriteCommandFile(bool appendContents)
        {
            lock (Lock)
            {
                using (StreamWriter sw = new StreamWriter(CommandFilePath, appendContents))
                {
                    sw.WriteLine(CommandFileString);
                }
            }
        }

        /// <summary>Writes the current contents of the interpreter command file to the physical file.
        /// If the file exists then it is overwritten.
        /// After this method is called, ResetCommands() should be called if the object will be further used.</summary>
        public virtual void WriteCommandFile()
        {
            WriteCommandFile(false);
        }

        /// <summary>Appends the current contents of the interpreter command file to the physical file.
        /// After this method is called, ResetCommands() should be called if the object will be further used.</summary>
        public virtual void WriteAppendToCommandFile()
        {
            WriteCommandFile(true);
        }

        /// <summary>Runs the interpreter command file.</summary>
        protected virtual void RunCommandFile()
        {
            lock(Lock)
            {
                bool easierWay = false;
                if (easierWay)
                {
                    if (string.IsNullOrEmpty(InterpreterCommand))
                        throw new InvalidDataException("Interpreter command is not specified.");
                    if (string.IsNullOrEmpty(CommandFilePath))
                        throw new InvalidDataException("Command file is not specified.");
                    System.Diagnostics.Process process;
                    process = System.Diagnostics.Process.Start(InterpreterCommand + " " + CommandFilePath);
                }
                else
                {
                    //Create a process:
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = InterpreterCommand; // path and file name of command to run
                    process.StartInfo.Arguments = CommandFilePath; // parameters to pass to program
                    process.StartInfo.UseShellExecute = false;  // whether to use OS shell to start the process
                    process.StartInfo.CreateNoWindow = false;
                    // process.StartInfo.RedirectStandardOutput = true; // in this way you can read output stream by string strOutput = pProcess.StandardOutput.ReadToEnd();
                    // process.StartInfo.WorkingDirectory = ".\"; //Optional
                    //Start the process
                    process.Start();
                    ////Get program output
                    //string strOutput = process.StandardOutput.ReadToEnd();
                    //Wait for process to finish
                    process.WaitForExit();
                }
            }
        }

        /// <summary>Writes interpreter command file and runs it.
        /// After this method is called, ResetCommands() should be called if the object will be further used.</summary>
        public virtual void WriteAndRunCommandFile()
        {
            lock (Lock)
            {
                WriteCommandFile();
                RunCommandFile();
            }
        }

        #endregion Actions


    }  // class InterfaceInterpreterBase



}

