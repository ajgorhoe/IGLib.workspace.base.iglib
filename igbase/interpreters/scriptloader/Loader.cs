// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using System.Diagnostics;

using IG.Lib;
using IG.Num;

namespace IG.Lib
{


    /// <summary>Base script loader class for IGLib.</summary>
    /// <seealso cref="ScriptLoaderBase"/>
    /// $A Igor Jul09;
    public class ScriptLoaderIGLib : ScriptLoaderBase, ILockable
    {


        /// <summary>Creates a new <see>ScriptLoaderIGLib</see>.</summary>
        public ScriptLoaderIGLib(): base()
        {

            AddReferencedAssemblies(
                "IGLib.dll",
                "MathNet.Numerics.dll"
                );
        }

        /// <summary>ScriptLoader used for testing.</summary>
        public class ScriptLoaderTest : ScriptLoaderIGLib, ILockable
        {


            public ScriptLoaderTest()
                : base()
            {
                AddReferencedAssemblies(
                    // "System.Xml.dll",
                    "System.Windows.Forms.dll"
                    );
            }

        }  // class ScriptLoaderTest

    }  // class ScriptLoaderIGLib


    /// <summary>Dynamically compiles and loads classes from files or from strings.</summary>
    /// $A Igor Jul09 Feb10;
    public abstract class ScriptLoaderBase: ILockable
    {

        #region Constructors_Initialization

        public ScriptLoaderBase()
        {
            InitializeScriptLoader();
            AddReferencedAssemblies(
                "System.dll"
                );
            AddDefaultReferencedAssemblies();
        }

        private bool _isScriptLoaderInitialized = false;

        /// <summary>Whether the current script loader is initialized or not.</summary>
        public bool IsScriptLoaderInitialized
        {
            get { return _isScriptLoaderInitialized; }
            protected set { _isScriptLoaderInitialized = value; }
        }

        /// <summary>Initializes the current script loader.</summary>
        public virtual void InitializeScriptLoader()
        {
            if (!IsScriptingInitialized)
                InitializeScripting();
            if (!IsScriptLoaderInitialized)
            {
                Type thisType = GetType();
                Type baseType = thisType.BaseType;
                Type baseBaseType = baseType.BaseType;
                Console.WriteLine();
                Console.WriteLine("Script loader initialized:");
                if (baseBaseType != null)
                {
                    Console.WriteLine("  " + thisType.FullName + " : " + baseType.Name + " : " 
                        + baseBaseType.Name + ".");
                } else
                {
                    Console.WriteLine("  " + thisType.FullName + " : " + baseType.Name + ".");
                }
                
                Console.WriteLine();
                IsScriptLoaderInitialized = true;
            }
        }


        private static bool _isScriptingInitialized = false;

        /// <summary>Whether scripting system is initialized or not.</summary>
        public static bool IsScriptingInitialized
        {
            get { return _isScriptingInitialized; }
            protected set { _isScriptingInitialized = value; }
        }

        /// <summary>Initializes the scripting system.</summary>
        public static void InitializeScripting()
        {
            if (!IsScriptingInitialized)
            {
                Console.WriteLine();
                Console.WriteLine("IGLib scripting system initialized (by Igor Grešovnik, 2009).");
                Console.WriteLine();
            }
        }


        #endregion Constructors_Initialization


        #region ILockable

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ILockable



        #region StaticUtilities


        /// <summary>Returns the loadable script object if the script object is actually loadable,
        /// or null if it is not meant as loadable.</summary>
        /// <param name="scriptObject">Script object to be returned if the object itself is loadable.</param>
        /// <remarks>This method filters out the controllable loadable scripts (i.e. the scripts 
        /// that implement the <see cref="ILoadableScriptC"/> interface) that are not loadable by property <see cref="ILoadableScriptC.IsLoadable"/>.</remarks>
        public static ILoadableScript GetLoadableScriptC(ILoadableScript scriptObject)
        {
            ILoadableScriptC sC = scriptObject as ILoadableScriptC;
            if (sC != null)
            {
                // Controls loading of the controllably loadable scripts:
                if (!sC.IsLoadable)
                    scriptObject = null;
            }
            return scriptObject;
        }


        /// <summary>Copies the specified file to byte array and returns it.</summary>
        /// TODO: move this to utils or something like that!
        public static byte[] LoadFileBytes(string filePath)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                buffer = new byte[(int)fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

        
        /// <summary>Creates and returns a new script obect of the specified type.
        /// <para>null is returned if the oject can not be created (but no exception is thrown).</para></summary>
        /// <param name="scriptClassFullName">Full name of the script class whose object is created.</param>
        /// <param name="outputLevel">Level of output to console.</param>
        public static ILoadableScript CreateScriptObject(string scriptClassFullName)
        {
            return CreateScriptObject(scriptClassFullName, 0);
        }

        /// <summary>Creates and returns a new script obect of the specified type.
        /// <para>null is returned if the oject can not be created (but no exception is thrown).</para></summary>
        /// <param name="scriptClassFullName">Full name of the script class whose object is created.</param>
        /// <param name="outputLevel">Level of output to console.</param>
        public static ILoadableScript CreateScriptObject(string scriptClassFullName, int outputLevel)
        {
            ILoadableScript script = null;
            try
            {
                // Try to find and instantiate the class in the process' entry assembly:
                Assembly assembly = Assembly.GetEntryAssembly();
                script = GetLoadableScriptC((ILoadableScript)assembly.CreateInstance(scriptClassFullName));
                if (script!=null && outputLevel>=1)
                    Console.WriteLine("Executing script " + script.GetType().FullName + " from entry assembly " + assembly.FullName + 
                        "..." + Environment.NewLine);
            }
            catch { }
            if (script == null)
            {
                // The specified class does not exist in the current assembly. Try to find it in all loaded assemblies:
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (script == null)
                        {
                            script = GetLoadableScriptC((ILoadableScript)assembly.CreateInstance(scriptClassFullName));
                            if (script!=null && outputLevel>=1)
                                Console.WriteLine("Executing script " + script.GetType().FullName + " from loaded assembly " + assembly.FullName + 
                                    "..." + Environment.NewLine);
                        }
                    }
                    catch{  }
                }
            }
            if (script == null)
            {
                // The specified class does not exist in the current assembly. Try to find it in all assemblies referenced
                // by the entry assembly:
                foreach (AssemblyName assemblyName in Assembly.GetEntryAssembly().GetReferencedAssemblies())
                {
                    try
                    {
                        if (script == null)
                        {
                            Assembly assembly = Assembly.Load(assemblyName);
                            script = GetLoadableScriptC((ILoadableScript)assembly.CreateInstance(scriptClassFullName));
                            if (script!=null && outputLevel>=1)
                                Console.WriteLine("Executing script " + script.GetType().FullName + " from directly referenced assembly " + assembly.FullName + 
                                    "..." + Environment.NewLine);
                        }
                    }
                    catch{  }
                }
            }
            if (script == null)
            {
                // The specified class does not exist in any assembly directly reeferenced by the entry assembly. 
                // Try to find it in all directly or indirectly referenced assemblies up to the first recursion level:
                foreach (AssemblyName referencedAssemblyName in Assembly.GetEntryAssembly().GetReferencedAssemblies())
                {
                    try
                    {
                        Assembly referencedAssembly = Assembly.Load(referencedAssemblyName);
                        foreach (AssemblyName assemblyName in referencedAssembly.GetReferencedAssemblies())
                        {
                            try
                            {
                                if (script == null)
                                {
                                    Assembly assembly = Assembly.Load(assemblyName);
                                    script = GetLoadableScriptC((ILoadableScript)assembly.CreateInstance(scriptClassFullName));
                                    if (script != null && outputLevel >= 1)
                                        Console.WriteLine("Executing script " + script.GetType().FullName + " from indirectly referenced assembly " + assembly.FullName +
                                            "..." + Environment.NewLine);
                                }
                            }
                            catch { }
                        }
                    }
                    catch {  }
                }
            }
            if (script == null)
            {
                // The specified class was not find, try to find it again in all loaded assemblies:
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) 
                {
                    try
                    {
                        if (script == null)
                        {
                            script = GetLoadableScriptC((ILoadableScript)assembly.CreateInstance(scriptClassFullName));
                            if (script != null && outputLevel >= 1)
                                Console.WriteLine("Executing script " + script.GetType().FullName + " from indirectly referenced assembly " + assembly.FullName +
                                    "..." + Environment.NewLine);
                        }
                    }
                    catch { }
                }
            }
            script = GetLoadableScriptC(script);
            return script;
        }


        #region DefaultAssemblies

        private static List<string> _defaultAssemblies;

        /// <summary>Gets the list of assemblies that are added to all newly created script loaders by default.</summary>
        protected static List<string> DefaultAssemblies
        {
            get
            {
                lock (Util.LockGlobal)
                {
                    if (_defaultAssemblies == null)
                        _defaultAssemblies = new List<string>();
                    return _defaultAssemblies;
                }
            }
        }

        /// <summary>Add the specified assemblies (just their names) to the list of assemblies 
        /// that are referenced by newly created script loaders by default.</summary>
        /// <param name="assemblies">Names of assemblies that are added to the list.</param>
        public static void AddDefaultAssemblies(params string[] assemblies)
        {
            if (assemblies != null)
            {
                int numAssemblies = assemblies.Length;
                if (numAssemblies > 0)
                {
                    lock (Util.LockGlobal)
                    {
                        List<string> assemblyList = DefaultAssemblies;
                        if (assemblyList != null)
                        {
                            for (int i = 0; i < numAssemblies; ++i)
                                assemblyList.Add(assemblies[i]);
                        }
                    }
                }
            }
        }

        /// <summary>Removes the specified assemblies from the list of default assemblies that are referenced
        /// by newly created script loaders.</summary>
        /// <param name="assemblies">names of assemblies that are removed from the list.</param>
        public static void RemoveDefaultAssemblies(params string[] assemblies)
        {
            if (assemblies != null)
            {
                int numAssemblies = assemblies.Length;
                if (numAssemblies > 0)
                {
                    lock (Util.LockGlobal)
                    {
                        List<string> assemblyList = DefaultAssemblies;
                        if (assemblyList != null)
                        {
                            for (int i = 0; i < numAssemblies; ++i)
                            {
                                if (assemblyList.Contains(assemblies[i]))
                                    assemblyList.Remove(assemblies[i]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Clears the list of assemblies that are referenced by default by newly created script loaders.</summary>
        public static void ClearDefaultAssemblies()
        {
            lock (Util.LockGlobal)
            {
                List<string> assemblyList = DefaultAssemblies;
                if (assemblyList != null)
                    assemblyList.Clear();
            }
        }

        /// <summary>Add assemblies that are referenced by default to the list of assemblies that are 
        /// referenced by the current script loader.
        /// <para>Assemblies that are referenced by default can be set by the <see cref="ScriptLoaderBase.AddDefaultAssemblies"/> method.</para></summary>
        protected void AddDefaultReferencedAssemblies()
        {
            List<string> assemblyList = DefaultAssemblies;
            if (assemblyList != null)
                if (assemblyList.Count > 0)
                {
                    this.AddReferencedAssemblies(assemblyList.ToArray());
                }
        }

        #endregion DefaultAssemblies


        private static SortedList<string, string> _usedClassFullNames;

        /// <summary>Stores used class names.</summary>
        protected static SortedList<string, string> UsedClassFullNames
        {
            get
            {
                if (_usedClassFullNames == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_usedClassFullNames == null)
                            _usedClassFullNames = new SortedList<string, string>();
                    }
                }
                return _usedClassFullNames;
            }
        }

        /// <summary>Returns a flag indicating whether the specified class full name has already been used or not.</summary>
        /// <param name="className">Fully qualified class name (namespace and class).</param>
        public static bool IsClassFullNameUsed(string className)
        { return UsedClassFullNames.ContainsKey(className); }

        private static ScriptLoaderBase _global;

        /// <summary>Global script loader.
        /// Getter always returns a non-null object.</summary>
        public static ScriptLoaderBase Global
        {
            get
            {
                if (_global == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_global == null)
                            _global = new ScriptLoaderIGLib();
                    }
                }
                return _global;
            }
            set
            {
                lock (Util.LockGlobal)
                {
                    _global = value;
                }
            }
        }


        #endregion Static


        #region Data

        private int _outputLevel = Util.OutputLevel;

        /// <summary>Definbes level of output on console that is generated by some operations.
        /// 0 means that no output will be written to the console.</summary>
        public int OutputLevel
        { get { return _outputLevel; } set { _outputLevel = value; } }

        private Logger _logger = new Logger();

        /// <summary>Gets the logger that is used to log errors and warnings.</summary>
        public virtual Logger Logger
        {
            get { lock (Lock) { return _logger; } }
        }

        /// <summary>Clears the logger.</summary>
        public virtual void ClearLogger()
        {
            lock (Lock)
            {
                Logger.Clear();
            }
        }


        
        /// <summary>Gets example code base (a template code for compiling) where names of classes, 
        /// methods etc. are properly set.</summary>
        public virtual string GetExampleCode()
        {
            return GetExampleCode(@"
      MessageBox.Show(""This is a testmessage.""); 
      Console.WriteLine(Environment.NewLine + ""Loadable object code executed. "" + Environment.NewLine );
");
        }


        /// <summary>Returns a complete example code that can be loaded and run.</summary>
        /// <pparam name="pureScript">Code that is inserted in the <see cref="LoadableScriptBase.RunThis"/> method.</pparam>
        public virtual string GetExampleCode(string pureScript)
        {
                return @"
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using IG.Lib;


namespace " + ClassNamespace + @"
{
  public class " + ClassName + ": LoadableScriptBase, " + ClassInterface + @"
  {
    public " + ClassName + @"()
    {
    }

    protected override void " + InitFunctionName + @"(string[] " + ArgumentsName + @")
    {  }

    protected override string " + RunFunctionName + @"(string[] " + ArgumentsName + @") 
    {
" + pureScript + @"  
      return null;
    }
    }
}
";
        }

        protected string _code = null;


        /// <summary>In the specified string, finds a name that follows the specified keyword, and returns it.
        /// Name must directly follow the keyword with only blank characters between them, and it end at the first
        /// character that is not alphanumeric and is also not contained in the specified string of allowed characters.</summary>
        /// <param name="code">String in which the specified name is searched for.</param>
        /// <param name="allowedCharacters">Eventual string arguments that contains characters that are allowed beside the alphanumeric
        /// characters.</param>
        /// <returns>Name that follows the specified keyword.</returns>
        /// <remarks>This functions can be used e.g. for searching for class names or namespace names in
        /// code blocks.
        /// The method is currently not implemented very efficiently because it uses access to characters of 
        /// a string through their within the string.</remarks>
        public static string FindNameAfterKeyword(string code, string keyword, string allowedCharacters)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(keyword))
                return null;
            int pos, posAfter;
            pos = code.IndexOf(keyword); // find the keyword
            if (pos < 0)
                return null;
            pos += keyword.Length;  // one after the keyword
            int codeLength = code.Length;
            while (pos < codeLength) // first non-blank after the keyword
            {
                char current = code[pos];
                if (!char.IsWhiteSpace(current))
                    break;
                ++pos;
            }
            if (pos >= codeLength)
                return null;
            // Find the first character after the name:
            posAfter = pos+1;
            while (posAfter < codeLength)
            {
                char current = code[posAfter];
                bool isPartOfName = false;
                if (char.IsLetterOrDigit(current))
                    isPartOfName = true;
                //else if (current=='_')
                //    partOfName = true;
                else 
                {
                    if (allowedCharacters!=null)
                        if (allowedCharacters.IndexOf(current) >= 0)
                            isPartOfName = true;
                }
                if (!isPartOfName)
                {
                    break;
                }
                if (allowedCharacters!=null)

                ++posAfter;
            }
            return code.Substring(pos, posAfter - pos);
        }

        /// <summary>Finds and returns the name of the first namespace that is defined in 
        /// the specified C# code block. A null string is returned if the name can not be found.</summary>
        /// <param name="code">Code block where namespace name is searched for.</param>
        public static string FindNamespace(string code)
        {
            return FindNameAfterKeyword(code, "namespace ", "._");
        }

        /// <summary>Finds and returns the name of the first class that is defined in 
        /// the specified C# code block. A null string is returned if the name can not be found.</summary>
        /// <param name="code">Code block where class name is searched for.</param>
        public static string FindClassName(string code)
        {
            return FindNameAfterKeyword(code, "class ","_");
        }


        /// <summary>Gets or sets code to be compiled.
        /// Get: If not assigned explicitly then CodeBase is taken.</summary>
        public virtual string Code
        {
            get 
            {
                lock (Lock)
                {
                    return _code;
                }
            }
            set {
                lock (Lock)
                {
                    _code = value;
                    IsCodeLoaded = true;
                    // Update dependencies:
                    IsCompiled = false;
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Script code is loaded.");
                        Console.WriteLine("Namespace used in the code:  \"" + FindNamespace(value) + "\"");
                        Console.WriteLine("Class name used in the code: \"" + FindClassName(value) + "\"");
                        Console.WriteLine();
                    }
                }
            }
        }

        private bool _nameCorrection = true;

        private string _languageName = "CSharp";
        private string _namespace = "IG.Script";
        private string _interface = "IG.Lib.ILoadableScript";
        private string _className = "TestClass";
        private string _appDomainName = "NewDomain";

        private string _runFunctionName = "RunThis";
        private string _initFunctionName = "InitializeThis";
        private string _argumentsName = "arguments";


        /// <summary>Specifies whether automatic checking and correction of the class and namespace 
        /// name in the loaded code is performed.
        /// If correction is switched on, it is performed before every compilation.
        /// Correction first checks if the class name has already been used, and if this is the case, it
        /// changes the name to the one that has not yet been used.
        /// It is then checked whether the namespace and class names used in the code are the same as
        /// those set on the current script loader. If not then names are replaced. At this stage, the assumed
        /// namespace class names are the first namespace and the first class found in the code block.</summary>
        public bool PerformNamesCorrection
        { get { lock (Lock) { return _nameCorrection; } } set { lock (Lock) { _nameCorrection = value; } } }

        /// <summary>Corrects the <see cref="ClassName"/> (if the current class name has already been used)
        /// and namespace and class name in the loaded scritp code (if they don't correspond to <see cref="ClassNamespace"/>
        /// and <see cref="ClassName"/>, respectively).</summary>
        public void CheckAndCorrectNames()
        {
            lock (Lock)
            {
                string originalClassFullName = ClassFullName;
                string originalClassName = ClassName;
                if (!string.IsNullOrEmpty(originalClassFullName) && IsClassFullNameUsed(originalClassFullName))
                {
                    int i = 1;
                    do
                    {
                        ClassName = originalClassName + string.Format("{0}", i);
                        ++i;
                    } while (IsClassFullNameUsed(ClassFullName));
                    Logger.LogWarning("Loadable script correction: class name " + originalClassName + 
                        " already used, replaced by " + ClassName);
                    if (OutputLevel > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Loadable script correction: class name " + originalClassName + 
                            " already used, replaced by " + ClassName);
                        Console.WriteLine();
                    }
                }
                string originalCode = Code;
                if (!string.IsNullOrEmpty(originalCode))
                {
                    string originalCodeNamespace = FindNamespace(originalCode);
                    string originalCodeClassName = FindClassName(originalCode);
                    string newCode = null;
                    if (!string.IsNullOrEmpty(originalCodeNamespace) && originalCodeNamespace != ClassNamespace)
                    {
                        newCode = originalCode.Replace(originalCodeNamespace, ClassNamespace);
                        originalCode = newCode;
                        Logger.LogWarning("Script code correction: namespace " + originalCodeNamespace + " replaced by " + ClassNamespace);
                        if (OutputLevel > 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Script code correction: namespace " + originalCodeNamespace + " replaced by " + ClassNamespace);
                            Console.WriteLine();
                        }
                    }
                    if (!string.IsNullOrEmpty(originalCodeClassName) && originalCodeClassName != ClassName)
                    {
                        newCode = originalCode.Replace(originalCodeClassName, ClassName);
                        originalCode = newCode;
                        Logger.LogWarning("Script code correction: namespace " + originalCodeClassName + " replaced by " + ClassName);
                        if (OutputLevel > 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Script code correction: namespace " + originalCodeClassName + " replaced by " + ClassName);
                            Console.WriteLine();
                        }
                    }
                    if (newCode != null)
                        Code = newCode;
                }
            }
        }


        /// <summary>Language used by compiler.</summary>
        public virtual string LanguageName
        {
            get { lock (Lock) { return _languageName; } }
            protected set 
            { 
                lock (Lock) 
                { 
                    _languageName = value; 
                    // Invalidate dependencies: 
                    IsCompiled = false;
                } 
            }
        }

        /// <summary>Namespace in which the compiled class is defined.</summary>
        public virtual string ClassNamespace
        {
            get { lock (Lock) { return _namespace; } }
            protected set 
            { 
                lock (Lock) 
                { 
                    _namespace = value; 
                    // Invalidate dependencies:
                    IsCompiled = false;
                } 
            }
        }

        /// <summary>Interface that is implemented by the compiled class.</summary>
        public virtual string ClassInterface
        {
            get { lock (Lock) { return _interface; } }
            // protected set { _interface = value; }
        }

        public virtual string ClassName
        {
            get { lock(Lock) { return _className; } }
            set {
                lock (Lock)
                {
                    string oldName = _className;
                    _className = value;
                    // Update dependencies: 
                    IsCompiled = false;
                    LibraryFilename = null;
                    // Check whether this class name has already been used:
                    if (UsedClassFullNames.ContainsKey(ClassFullName))
                    {
                        string fullName = ClassFullName;
                        _className = oldName;
                        // throw new ArgumentException("Class with this name has already been compiled. Class full name: " + fullName);
                    }
                }
            }
        }

        /// <summary>Name of the compiled class that contains loadable script.</summary>
        public virtual string ClassFullName
        {
            get { lock (Lock) { return ClassNamespace + "." + ClassName; } }
        }

        /// <summary>Name of the application domain in which the class is loaded.</summary>
        public virtual string AppDomainName
        {
            get { lock (Lock) { return _appDomainName; } }
            set
            {
                lock (Lock)
                {
                    _appDomainName = value;
                    // Invalidate dependencies:
                    IsCompiled = false;
                }
            }
        }

        /// <summary>Name of the method of dynamically loadable classes that contains the runnable script.</summary>
        public virtual string RunFunctionName
        {
            get { lock (Lock) { return _runFunctionName; } }
            protected set 
            {
                lock (Lock)
                {
                    _runFunctionName = value;
                    // Invalidate dependencies:
                    IsCompiled = false;
                }
            } 
        }

        /// <summary>Name of the initialization method of dynamically loadable classes.</summary>
        public virtual string InitFunctionName
        {
            get { lock (Lock) { return _initFunctionName; }  }
            protected set 
            {
                lock (Lock)
                {
                    _initFunctionName = value;
                    // Invalidate dependencies:
                    IsCompiled = false;
                }
            }
        }

        /// <summary>Agreed formal name of the arguments parameter. Agreement upon this name makes possible
        /// to execute scripts in which run arguments are referenced.</summary>
        public virtual string ArgumentsName
        {
            get
            { lock (Lock) { return _argumentsName; } }
            set { lock (Lock) { _argumentsName = value; } }
        }


        protected string _libraryFilename = null;  // will be defined through classname
        private string _libraryPath = null;
        protected string _originalLibraryPath = null;
        private string _libraryDirectory = null;


        /// <summary>Returns the directory containing the executable that started the current
        /// application.</summary>
        public virtual string GetExecutableDirectory()
        {
            return UtilSystem.GetExecutableDirectory();
        }

        public virtual string LibraryFilename
        {
            get { lock (Lock) {
                if (_libraryFilename == null && ClassName!=null)
                    _libraryFilename = ClassName + ".dll";
                return _libraryFilename; 
            } }
            set
            {
                lock (Lock)
                {
                    _libraryFilename = value;
                    // Update dependencies:
                    LibraryPath = null;
                }
            }
        }

        /// <summary>Returns directory where library will be compiled, which will be the
        /// directory of the executable that started the application.</summary>
        public virtual string LibraryDirectory
        {
            get 
            {
                lock (Lock)
                {
                    if (_libraryDirectory == null)
                    {
                        LibraryDirectory = GetExecutableDirectory();
                    }
                    return _libraryDirectory;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _libraryDirectory = value;
                    // Invalidate dependencies:
                    LibraryPath = null;
                }
            }
        }


        /// <summary>Path to the dll where code is compiled.
        /// We take the directory where executable is located.</summary>
        public virtual string LibraryPath
        {
            get 
            {
                lock (Lock)
                {
                    if (string.IsNullOrEmpty(_libraryPath))
                        LibraryPath = Path.Combine(LibraryDirectory, LibraryFilename);
                    return _libraryPath;
                }
            }
            protected set
            {
                lock (Lock)
                {
                    _libraryPath = value;
                    _originalLibraryPath = value;
                    // Invalidate dependencies: 
                    IsCompiled = false;
                }
            }
        }



        private bool _isCodeLoaded = false;
        private bool _isCompiled = false;
        private bool _isClassLoaded = false;
        private bool _isObjectPrepared = false;


        /// <summary>Whether scripting code to be compiled is loaded.</summary>
        public virtual bool IsCodeLoaded
        {
            get { lock (Lock) { return _isCodeLoaded; } } 
            protected set {
                lock (Lock)
                {
                    _isCodeLoaded = value;
                    // Update dependencies:
                    if (value == false)
                        IsCompiled = false;
                }
            } 
        }

        /// <summary>Whether the code is compiled.</summary>
        public virtual bool IsCompiled
        {
            get { lock (Lock) { return _isCompiled; } } 
            protected set {
                lock (Lock)
                {
                    _isCompiled = value;
                    // Update dependencies:
                    if (value == false)
                        IsClassLoaded = false;
                }
            } 
        }

        /// <summary>Whether the compiled class corresponding to the script is prepared.</summary>
        public virtual bool IsClassLoaded
        {
            get { lock (Lock) { return _isClassLoaded; } } 
            protected set 
            {
                lock (Lock)
                {
                    _isClassLoaded = value;
                    // Update dependencies:
                    if (value == false)
                        IsObjectPrepared = false;
                }
            } 
        }

        /// <summary>Whether an instance of the object is prepared to be used.</summary>
        public virtual bool IsObjectPrepared
        {
            get { lock (Lock) { return _isObjectPrepared; } }
            protected set 
            {
                lock (Lock)
                {
                    _isObjectPrepared = value;
                }
            }
        }




        #endregion Data


        #region Operation



        private CompilerResults _compilerResults;


        /// <summary>Results of compilation.</summary>
        public virtual CompilerResults CompilerResults
        { 
            get { lock (Lock) { return _compilerResults; } } 
            protected set { lock (Lock) { _compilerResults = value; }  } 
        }


        protected List<string> _referencedAssimblies = new List<string>();

        protected bool _referenceAllLoadedAssemblies = true;

        /// <summary>List of assemblies that are referenced when compiling the code.
        /// Contains names of files containing referenced assemblies.</summary>
        public virtual List<String> ReferencedAssemblies
        {
            get { return _referencedAssimblies; }
        }

        /// <summary>If true then all assemblies that are currently loaded by the application are also
        /// added to the referenced assemblies when script code is compiled.
        /// This makes sure that all that is necessary is actually referenced.</summary>
        public virtual bool ReferenceAllLoadedAssemblies
        {
            get { lock (Lock) { return _referenceAllLoadedAssemblies; } }
            set { lock (Lock) { _referenceAllLoadedAssemblies = true; } }
        }

        /// <summary>Adds an additional referenced assembly by name of the assembly file.
        /// Assmebly is only added if it is not yet contained in the list of referenced assemblies and if 
        /// the specified assembly file name is not null or empty string.</summary>
        /// <param name="assemblyFile">File name of the assembly that is added to referenced assemblies.</param>
        public virtual void AddReferencedAssembly(string assemblyFile)
        {
            lock (Lock)
            {
                if (!string.IsNullOrEmpty(assemblyFile))
                    if (!ReferencedAssemblies.Contains(assemblyFile))
                        ReferencedAssemblies.Add(assemblyFile);
            }
        }

        /// <summary>Adds the specified assemblies to the list of referenced assemblies.
        /// Assemblise are added only if not yet on the list and if their names are not null or empty strings.</summary>
        /// <param name="assemblyFiles">A table of file names of the assemblies to be added.</param>
        public virtual void AddReferencedAssemblies(params string[] assemblyFiles)
        {
            lock (Lock)
            {
                if (assemblyFiles != null)
                    for (int i = 0; i < assemblyFiles.Length; ++i)
                    {
                        AddReferencedAssembly(assemblyFiles[i]);
                    }
            }
        }

        /// <summary>Sets the list of referenced assemblies in such a way that it contains only
        /// the specified assemblies.</summary>
        /// <param name="assemblyFiles">Array of file names of the assemblies to be contained in the list.</param>
        public virtual void SetReferencedAssemblies(params string[] assemblyFiles)
        {
            lock (Lock)
            {
                ReferencedAssemblies.Clear();
                AddReferencedAssemblies(assemblyFiles);
            }
        }

        /// <summary>Removes the specified assembly from the list of referenced assemblies.
        /// If the specified assembly does not exist on the list or it is a null or empty string then nothing happens.</summary>
        /// <param name="assemblyFile">File name of the assembly to be removed.</param>
        public virtual void RemoveReferencedAssembly(string assemblyFile)
        {
            lock (Lock)
            {
                if (ReferencedAssemblies.Contains(assemblyFile))
                    ReferencedAssemblies.Remove(assemblyFile);
            }
        }

        /// <summary>Removes the specified assemblies from the list of referenced assemblies.</summary>
        /// <param name="assemblyFiles">Array of file names of the assemblies to be removed from the list.</param>
        public virtual void RemoveReferencedAssemblies(params string[] assemblyFiles)
        {
            lock (Lock)
            {
                if (assemblyFiles != null)
                    for (int i = 0; i < assemblyFiles.Length; ++i)
                    {
                        RemoveReferencedAssembly(assemblyFiles[i]);
                    }
            }
        }

        /// <summary>Removes all assemblies from tehe list of referenced assemblies.</summary>
        public virtual void RemoveAllReferencedAssemblies()
        {
            lock (Lock)
            {
                ReferencedAssemblies.Clear();
            }
        }


        /// <summary>Returns a file path that does not exist and is derived form the specified file path
        /// by adding a suffix composed of underscore and a number.</summary>
        /// <param name="originalPath">Original file path.</param>
        /// <remarks>Even if the file with original path does not exist, the method finds and returns a new path.</remarks>
        protected static string CreateNonexistentFilePath(string originalPath)
        {
                string newPath = null;
                if (string.IsNullOrEmpty(originalPath))
                    throw new ArgumentNullException("Original path not specified (null or empty string).");
                string directoryName = Path.GetDirectoryName(originalPath);
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalPath);
                string fileExtension = Path.GetExtension(originalPath);
                int i = 1;
                do
                {
                    newPath = Path.Combine(directoryName, fileNameWithoutExt + "_"
                        + string.Format("D3", i)
                        + fileExtension);
                } while (File.Exists(newPath));
                return newPath;
        }

        /// <summary>Modifies library path in such a way that it points to an non-existent file.</summary>
        protected void CreateNonexistentLibraryPath()
        {
            lock (Lock)
            {
                string originalPath = _originalLibraryPath;  // remember the original library path
                LibraryPath = CreateNonexistentFilePath(originalPath);  // new path is derived from original path
                _originalLibraryPath = originalPath;  // make sure that original path doesn't change.
            }
        }


        /// <summary>Adds referenced assemblies to the specified compiler parameters,
        /// according to current settings.</summary>
        /// <param name="compilerParameters">Compiler parameters on which referenced assemblies ar added.</param>
        protected virtual void AddReferencedAssemblies(System.CodeDom.Compiler.CompilerParameters compilerParameters)
        {
            if (compilerParameters == null)
                throw new ArgumentNullException("Compiler parameters object not specified (null reference).");

            // Add referenced assemblies to parameters:
            for (int i = 0; i < ReferencedAssemblies.Count; ++i)
            {
                string assemblyFile = ReferencedAssemblies[i];
                try
                {
                    try
                    {
                        string assemblyFullPath = Path.Combine(LibraryDirectory, assemblyFile);
                        if (File.Exists(assemblyFullPath))
                            assemblyFile = assemblyFullPath;
                    }
                    catch {  }
                    compilerParameters.ReferencedAssemblies.Add(assemblyFile);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("Could not reference the following assembly: " + assemblyFile + ".", ex);
                }
            }
            if (ReferenceAllLoadedAssemblies)
            {
                // Add all assemblies that are currently loaded but are not yet added:
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                if (OutputLevel >= 3)
                {
                    Console.WriteLine("List of assemblies that are refferenced by the current application domain: ");
                    if (assemblies == null)
                        Console.WriteLine("Array is null.");
                    else for (int i = 0; i < assemblies.Length; ++i)
                            Console.WriteLine("  " + assemblies[i]);
                }
                if (assemblies != null)
                {
                    SortedList<string, string> alreadyAddedAssemblies = new SortedList<string, string>();
                    foreach (string assemblyLocation in compilerParameters.ReferencedAssemblies)
                    {
                        alreadyAddedAssemblies.Add(Path.GetFileName(assemblyLocation), null);
                    }
                    for (int i = 0; i < assemblies.Length; ++i)
                    {
                        Assembly assembly = assemblies[i];
                        bool alreadyContained = false;
                        bool doAdd = (!assembly.IsDynamic);
                        if (alreadyAddedAssemblies.ContainsKey(Path.GetFileName(assembly.Location)))
                        {
                            doAdd = false;
                            alreadyContained = true;
                        }
                        if (doAdd)
                        {
                            compilerParameters.ReferencedAssemblies.Add(assembly.Location);
                            if (OutputLevel >= 2)
                            {
                                if (i == 0)
                                    Console.WriteLine();
                                Console.WriteLine("Loaded assembly added: " + Path.GetFileName(assembly.Location));
                                if (i == assemblies.Length - 1)
                                    Console.WriteLine();
                            }
                        }
                        else
                        {
                            if (OutputLevel >= 2)
                            {
                                if (i == 0)
                                    Console.WriteLine();
                                Console.WriteLine("NOT added: " + Path.GetFileName(assembly.Location));
                                if (alreadyContained)
                                    Console.WriteLine("  The above assembly is already contained in referenced assemblies.");
                                if (i == assemblies.Length - 1)
                                    Console.WriteLine();
                            }
                        }
                    }
                }
            }
        }  // AddReferencedAssemblies(...)

        /// <summary>Returns an array of assemblies (paths, sometimes only file names) that are currently 
        /// referenced by the compiler on the current script loader.</summary>
        public virtual string[] GetReferencedAssemblies()
        {
            System.CodeDom.Compiler.CompilerParameters compilerParameters = new System.CodeDom.Compiler.CompilerParameters();
            AddReferencedAssemblies(compilerParameters);
            List<string> assemblies = new List<string>();
            foreach (string assemblyLocation in compilerParameters.ReferencedAssemblies)
            {
                assemblies.Add(Path.GetFileName(assemblyLocation));
            }
            return assemblies.ToArray();
        }

        /// <summary>Compiles the code that is currently loaded by the current loader, and returns full name of the compiled
        /// script class.</summary>
        /// <returns>Full name of the class implementing teh <see cref="ILoadableScript"/> interface that has been compiled.</returns>
        public virtual string Compile()
        {
            lock (Lock)
            {
                if (IsCompiled)
                    return ClassFullName;
                if (PerformNamesCorrection)
                    CheckAndCorrectNames();  // eventully correct class name if it has already been used.
                if (IsClassFullNameUsed(ClassFullName))
                {
                    throw new InvalidOperationException("Class with this name has already been compiled. Class full name: " + ClassFullName);
                }
                bool compileSuccessful = false;
                if (string.IsNullOrEmpty(Code))
                    throw new InvalidOperationException("Can not compile the code, code not specified (null or empty string).");
                if (File.Exists(LibraryPath))
                {
                    try
                    {
                        File.Delete(LibraryPath);
                        if (OutputLevel > 0)
                        {
                            Console.WriteLine(Environment.NewLine + "Library file deleted: " + LibraryPath
                                + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning("File could not be deleted: \n  " + LibraryPath, ex);
                        LibraryPath = CreateNonexistentFilePath(LibraryPath);
                    }
                }

                CodeDomProvider codeProvider = CodeDomProvider.CreateProvider(LanguageName);
                System.CodeDom.Compiler.CompilerParameters compilerParameters = new CompilerParameters();

                compilerParameters.OutputAssembly = LibraryPath;

                AddReferencedAssemblies(compilerParameters);

                compilerParameters.WarningLevel = 3;

                // compilerParameters.CompilerOptions = "/target:library /optimize";

                compilerParameters.GenerateExecutable = false;
                compilerParameters.GenerateInMemory = false;


                //parameters.GenerateExecutable = true;
                //parameters.OutputAssembly = Output;

                CompilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, Code);

                if (CompilerResults.Errors.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("The following compiler errors occurred:");
                    int numErrors = 0, numWarnings = 0;
                    foreach (CompilerError ce in CompilerResults.Errors)
                    {
                        if (ce.IsWarning)
                        {
                            Logger.LogWarning("Compiler warning No. " + ce.ErrorNumber
                                + " at (" + ce.Line + ", " + ce.Column + ") in " + ce.FileName + ": "
                                + Environment.NewLine + "  " + ce.ErrorText);
                            ++numWarnings;
                            if (OutputLevel >= 2)
                            {
                                Console.WriteLine("Compiler ERROR No. " + ce.ErrorNumber
                                + " at (" + ce.Line + ", " + ce.Column + ") in " + ce.FileName + ": "
                                + Environment.NewLine + Environment.NewLine + "  " + ce.ErrorText);
                            }
                        }
                        else
                        {
                            Logger.LogError("Compiler ERROR No. " + ce.ErrorNumber
                                + " at (" + ce.Line + ", " + ce.Column + ") in " + ce.FileName + ": "
                                + Environment.NewLine + "  " + ce.ErrorText);
                            ++numErrors;
                            if (OutputLevel >= 1)
                            {
                                Console.WriteLine("Compiler ERROR No. " + ce.ErrorNumber
                                + " at (" + ce.Line + ", " + ce.Column + ") in " + ce.FileName + ": "
                                + Environment.NewLine + Environment.NewLine + "  " + ce.ErrorText);
                            }
                        }
                    }
                    if (numErrors > 0)
                    {
                        compileSuccessful = false;
                        throw new InvalidOperationException("Code could not be compiled. "
                            + Environment.NewLine + "  Compiler errors: " + Logger.GetErrorsReport());

                    }
                    else
                        compileSuccessful = true;
                } else
                {
                    compileSuccessful = true;
                }
                if (compileSuccessful)
                    UsedClassFullNames.Add(ClassFullName,null);

                System.Collections.Specialized.StringCollection compilerOutputs = CompilerResults.Output;
                foreach (string infoString in compilerOutputs)
                {
                    Logger.LogInfo("Compiler: " + infoString);
                }

                if (ApplicationDomain == null)
                {
                    AppDomainSetup ads = new AppDomainSetup();
                    ads.ShadowCopyFiles = "false";
                    // ads.ShadowCopyFiles();
                    // AppDomain.CurrentDomain.SetShadowCopyFiles();
                    System.Security.Policy.Evidence securityInfo = new System.Security.Policy.Evidence();
                    ApplicationDomain = AppDomain.CreateDomain(AppDomainName, securityInfo, ads);
                }


                byte[] rawAssembly = LoadFileBytes(this.LibraryPath);
                Assembly = ApplicationDomain.Load(rawAssembly, null);
                if (Assembly == null)
                    compileSuccessful = false;
                if (compileSuccessful)
                    IsCompiled = true;
                return ClassFullName;
            } // lock
        }  // Compile()


        private AppDomain _applicationDomain;
        private Assembly _assembly;
        private ILoadableScript _runnableObject;

        /// <summary>Application domain used by the current script loader for loading assemblies.</summary>
        protected virtual AppDomain ApplicationDomain
        {
            get { lock (Lock) { return _applicationDomain; } }
            set 
            { 
                lock (Lock) 
                { 
                    _applicationDomain = value; 
                    // Invalidate dependencies: 
                    Assembly = null;  // this will set IsCompiled to false.
                } 
            }
        }


        /// <summary>Unloads the application domain used by the current script loader.</summary>
        public virtual void UnloadApplicationDomain()
        {
            lock (Lock)
            {
                AppDomain.Unload(ApplicationDomain);
                ApplicationDomain = null;  // this will set IsCompiled to false.
            }
        }

        public virtual Assembly Assembly
        {
            get { lock (Lock) { return _assembly; } }
            protected set 
            { 
                lock (Lock) 
                {
                    _assembly = value; 
                    // Update dependencies:
                    if (value == null)
                        IsCompiled = false;
                    else
                        IsClassLoaded = true;
                } 
            }
        }


        /// <summary>Object of the compiled class that has beeen created.</summary>
        public virtual ILoadableScript CreatedObject
        {
            get { lock (Lock) { return _runnableObject; } }
            protected set 
            { 
                lock (Lock) 
                {
                    _runnableObject = value; 
                    // Update dependencies: 
                    if (value == null)
                        IsObjectPrepared = false;
                    else
                        IsObjectPrepared = true;
                } 
            }
        }

        
        /// <summary>Creates and returns an object of the class that has been last compiled.
        /// The created object is also stored such that it can later be accessed through
        /// the <see cref="CreatedObject"/> property.</summary>
        /// <param name="initializationArguments">Initialization arguments.</param>
        public virtual ILoadableScript CreateObject(string[] initializationArguments)
        {
            return CreateLoadableObject(initializationArguments, true);
        }

        /// <summary>Creates and returns an object of the class that has been last compiled.</summary>
        /// <param name="initializationArguments">Initialization arguments.</param>
        /// <param name="storeObject">If true then the created object is stored such that it can be accessed
        /// via <see cref="CreatedObject"/> property.</param>
        public virtual ILoadableScript CreateLoadableObject(string[] initializationArguments, bool storeObject)
        {
            if (!IsCompiled)
                Compile();
            ILoadableScript ret = CreateLoadableObject(initializationArguments, this.ClassFullName);
            if (storeObject)
                CreatedObject = ret;
            return ret;
        }

        /// <summary>Creates and returns an object of the specified loadable class.</summary>
        /// <param name="initializationArguments">Initialization arguments.</param>
        /// <param name="classFullName">Full name of the class that is instantiated.</param>
        public virtual ILoadableScript CreateLoadableObject(string[] initializationArguments, string classFullName)
        {
            object instance = Assembly.CreateInstance(classFullName);
            // ILoadableScript ReturnedString = GetLoadableScriptC((ILoadableScript)Assembly.CreateInstance(classFullName));
            ILoadableScript ret = GetLoadableScriptC(instance as ILoadableScript);
            if (ret != null)
            {
                ret.InitializationArguments = initializationArguments;
            }
            return ret;
        }

        /// <summary>Runs the object of the compiled loadable class (calls its Run() method) 
        /// that is currently loaded by the current loader.
        /// Compiles the code and creates the object if necessary.</summary>
        /// <param name="initializationAndRunArguments">Arguments used both for initialization and running of the object.</param>
        public virtual void Run(string[] initializationAndRunArguments)
        {
            Run(initializationAndRunArguments, initializationAndRunArguments);
        }

        /// <summary>Runs the object of the compiled loadable class (calls its Run() method) 
        /// that is currently loaded by the current loader.
        /// Compiles the code and creates the object if necessary.</summary>
        /// <param name="initializationArguments">Argumets used for initialization of the object.</param>
        /// <param name="runArguments">Arguments used to run the object.</param>
        public virtual void Run(string[] initializationArguments, string[] runArguments)
        {
            lock (Lock)
            {
                if (!IsClassLoaded)
                {
                    Compile();
                }

                if (!IsObjectPrepared)
                    CreatedObject = CreateObject(initializationArguments);

                CreatedObject.Run(runArguments);

                // UnloadApplicationDomain();
            }
        }  // Run()


        #region HighLevel

        // Create loadable script objects from string code blocks:

        /// <summary>Loads code form a string.</summary>
        /// <param name="code">String containing script code that is loaded.</param>
        /// <param name="className">Name of the class that is contained in the code and contains
        /// loadable script that can be executed.</param>
        public void LoadCode(string code, string className)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Code containing loadable script is not specified.");
            lock (Lock)
            {
                Code = code;
                if (string.IsNullOrEmpty(className))
                {
                    className = FindClassName(code);
                }
                if (string.IsNullOrEmpty(className))
                    throw new ArgumentException("Class name is not specified and could not be extracted form loadable script code.");
                ClassName = className;
            }
        }

        /// <summary>Loads code form a string. Class name is extracted from the code.</summary>
        /// <param name="code">String containing script code that is loaded.</param>
        public void LoadCode(string code)
        {
            LoadCode(code, null);
        }

        /// <summary>Creates and returns a loadable script object form code.</summary>
        /// <param name="code">Code that contains class definition and is dynamically compiled.</param>
        /// <param name="className">Name of the class containing loadable script code.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object.</param>
        public ILoadableScript CreateObjectFromCode(string code, string className, string[] initializationArguments)
        {
            lock (Lock)
            {
                LoadCode(code, className);
                return CreateObject(initializationArguments);
            }
        }

        /// <summary>Creates and returns a loadable script object form code.
        /// Class name is extracted from code.</summary>
        /// <param name="code">Code that contains class definition and is dynamically compiled.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object.</param>
        public ILoadableScript CreateObjectFromCode(string code, string[] initializationArguments)
        {
            lock (Lock)
            {
                LoadCode(code);
                return CreateObject(initializationArguments);
            }
        }


        /// <summary>Creates and runs a loadable script object form code.</summary>
        /// <param name="code">Code that contains class definition and is dynamically compiled.</param>
        /// <param name="className">Name of the class containing loadable script code.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object.</param>
        /// <param name="runArguments">Arguments passed to the execution method of the loadable script.</param>
        public string RunCode(string code, string className, 
            string[] initializationArguments, string [] runArguments)
        {
            lock (Lock)
            {
                LoadCode(code, className);
                return CreateObject(initializationArguments).Run(runArguments);
            }
        }

        /// <summary>Creates and returns a loadable script object form code.
        /// Class name is extracted from code.</summary>
        /// <param name="code">Code that contains class definition and is dynamically compiled.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object.</param>
        /// <param name="runArguments">Arguments passed to the execution mathod of the loadable script.</param>
        public string RunCode(string code, string[] initializationArguments, string[] runArguments)
        {
            lock (Lock)
            {
                LoadCode(code);
                return CreateObject(initializationArguments).Run(runArguments);
            }
        }

        /// <summary>Runs a loadable script object form code.</summary>
        /// <param name="code">Code that contains class definition and is dynamically compiled.</param>
        /// <param name="className">Name of the class containing loadable script code.</param>
        /// <param name="initializationAndRunArguments">Arguments used both for initialization of 
        /// the loadable script object and as parameters of the executable method..</param>
        public string RunCode(string code, string className, 
            string[] initializationAndRunArguments)
        {
            lock (Lock)
            {
                LoadCode(code, className);
                return CreateObject(initializationAndRunArguments).Run(initializationAndRunArguments);
            }
        }

        /// <summary>Creates and returns a loadable script object form code.
        /// Class name is extracted from code.</summary>
        /// <param name="code">Code that contains class definition and is dynamically compiled.</param>
        /// <param name="initializationAndRunArguments">Arguments used both for initialization of 
        /// the loadable script object and as parameters of the executable method..</param>
        public string RunCode(string code, string[] initializationAndRunArguments)
        {
            lock (Lock)
            {
                LoadCode(code);
                return CreateObject(initializationAndRunArguments).Run(initializationAndRunArguments);
            }
        }


        // Create loadable scripr objects form files:

        /// <summary>Returns contents of the specified file.</summary>
        /// <param name="inputFilePath">Path to the file.</param>
        /// <returns>Full contents of the file as string.</returns>
        public static string GetFileContents(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path is not defined (null or empty string).");
            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exist: " + filePath);
            string ret = null;
            using (StreamReader sr = new StreamReader(filePath))
            {
                ret = sr.ReadToEnd();
            }
            return ret;
        }

        /// <summary>Loads loadable script code form the specified file.</summary>
        /// <param name="inputFilePath">Path to the file that contains script code.</param>
        /// <param name="className">Name of the class that is contained in the code and 
        /// embeds loadable script that can be executed.</param>
        public void LoadFile(string filePath, string className)
        {
            string code = GetFileContents(filePath);
            LoadCode(code, className);
        }

        /// <summary>Loads loadable script code form a file. 
        /// Name of the class is extracted from the file contents.</summary>
        /// <param name="inputFilePath">Path to the file that contains script code.</param>
        public void LoadFile(string filePath)
        {
            string code = GetFileContents(filePath);
            LoadCode(code);
        }

        /// <summary>Creates and returns a loadable script object form a file containing its code.</summary>
        /// <param name="inputFilePath">Path to the file that contains script code.</param>
        /// <param name="className">Name of the class containing class definition for loadable script objects.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object.</param>
        public ILoadableScript CreateObjectFromFile(string filePath, string className, string[] initializationArguments)
        {
            lock (Lock)
            {
                string code = GetFileContents(filePath);
                LoadCode(code, className);
                return CreateObject(initializationArguments);
            }
        }

        /// <summary>Creates and returns a loadable script object form code.
        /// Class name is extracted from code.</summary>
        /// <param name="inputFilePath">Path to the file that contains script code.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object.</param>
        public ILoadableScript CreateObjectFromFile(string filePath, string[] initializationArguments)
        {
            lock (Lock)
            {
                string code = GetFileContents(filePath);
                LoadCode(code);
                return CreateObject(initializationArguments);
            }
        }


        /// <summary>Executes a loadable script form a file. The file must contain definition of the loadable 
        /// script class that is used to instantiate an object and execute it.</summary>
        /// <param name="inputFilePath">Path to the file that contains script code.</param>
        /// <param name="className">Name of the class containing loadable script code.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object.</param>
        /// <param name="runArguments">Arguments passed to the execution mathod of the loadable script.</param>
        public string RunFile(string filePath, string className,
            string[] initializationArguments, string[] runArguments)
        {
            lock (Lock)
            {
                string code = GetFileContents(filePath);
                LoadCode(code, className);
                return CreateObject(initializationArguments).Run(runArguments);
            }
        }

        /// <summary>Executes a loadable script form a file. The file must contain definition of the loadable 
        /// script class that is used to instantiate an object and execute it.
        /// Class name is extracted from code.</summary>
        /// <param name="inputFilePath">Path to the file that contains script code.</param>
        /// <param name="initializationArguments">Initialization arguments for the created object.</param>
        /// <param name="runArguments">Arguments passed to the execution mathod of the loadable script.</param>
        public string RunFile(string filePath, string[] initializationArguments, string[] runArguments)
        {
            lock (Lock)
            {
                string code = GetFileContents(filePath);
                LoadCode(code);
                return CreateObject(initializationArguments).Run(runArguments);
            }
        }

        /// <summary>Executes a loadable script form a file. The file must contain definition of the loadable 
        /// script class that is used to instantiate an object and execute it. 
        /// The same arguments are taken for initialization of the loadable script object and for execution
        /// of the script.</summary>
        /// <param name="inputFilePath">Path to the file that contains script code.</param>
        /// <param name="className">Name of the class containing loadable script code.</param>
        /// <param name="initializationAndRunArguments">Arguments used both for initialization of 
        /// the loadable script object and as parameters of the executable method.</param>
        public string RunFile(string filePath, string className,
            string[] initializationAndRunArguments)
        {
            lock (Lock)
            {
                string code = GetFileContents(filePath);
                LoadCode(code, className);
                return CreateObject(initializationAndRunArguments).Run(initializationAndRunArguments);
            }
        }

        /// <summary>Executes a loadable script form a file. The file must contain definition of the loadable 
        /// script class that is used to instantiate an object and execute it.
        /// The same arguments are taken for initialization of the loadable script object and for execution
        /// of the script.
        /// Class name is extracted from code.</summary>
        /// <param name="inputFilePath">Path to the file that contains script code.</param>
        /// <param name="className">Name of the class containing loadable script code.</param>
        /// <param name="initializationAndRunArguments">Arguments used both for initialization of 
        /// the loadable script object and as parameters of the executable method.</param>
        public string RunFile(string filePath, string[] initializationAndRunArguments)
        {
            lock (Lock)
            {
                string code = GetFileContents(filePath);
                LoadCode(code);
                return CreateObject(initializationAndRunArguments).Run(initializationAndRunArguments);
            }
        }

        #endregion HighLevel

        #endregion Operation


    }  // class ScriptLoaderBase

}