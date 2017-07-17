// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/



            /*******************************/
            /*                             */
            /*    ProgramBase Utilities    */
            /*                             */
            /*******************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


namespace IG.Lib
{


    public class ModuleTest : Module
    {

        protected ModuleTest()
            : this("IGLib Forms Test Program", 0, 0)
        {  }

        protected ModuleTest(string name, int version, int subversion) :
                base(name, version, subversion)
        {  }


        protected override void ModuleInitializationBefore()
        {
            Name = "IGLib Test Module";
            Version = 1;
            SubVersion = 2;
            Release = "debug";
            Expires = false;
            base.ModuleInitializationBefore();
            Email = "inverse@gmail.com";  // program'result contact e-mail
            WebPage = "www2.arnes.si/~ljc3m2/igor/ioptlib/";  // program'result web page
            Phone = null;  // program'result contact telephone
            AuthorFirstName = "Igor";  // principal author'result first name
            AuthorMidName = null; // principal author'result middle name
            AuthorSecondName = "Grešovnik";  // principal author'result second name
            AuthorAddress1 = "Črneče 147";  // principal author'result address, first line
            AuthorAddress2 = "2370 Dravograd"; // principal author'result address, second line
            AuthorAddress3 = "Slovenia"; // principal author'result address, third line
            AuthorAddress4 = null;
            Authors = null;
        }

        protected override void ModuleInitializationAfter()
        {
            base.ModuleInitializationAfter();
        }


        #region global


        protected static new object _lock = new Object();

        protected static ModuleTest _moduleTest = null;

        /// <summary>Initializes global application data for the current class of application.</summary>
        public static Module Get()
        {
            lock (_lock)
            {
                if (_moduleTest == null)
                {
                    _moduleTest = new ModuleTest();
                }
                return _moduleTest;
            }
        }

        #endregion

    }  // class ModuleTest

    public class AppTest : ApplicationBase
    {

        protected AppTest()
            : base("IGLib Test Program", 0, 0)
        {  }


        /// <summary>Things performed before initialization of the application.</summary>
        protected override void ModuleInitializationBefore()
        {
            base.ModuleInitializationBefore();
            Expires = false;
        }

        protected override void ModuleInitializationAfter()
        {
            base.ModuleInitializationAfter();
            LaunchInitNotice();
            AddModule(ModuleTest.Get());
        }


        #region global


        /// <summary>Launches initialization notice.</summary>
        public override void LaunchInitNotice()
        {
            int indent = 8;
            int padLeft = 2;
            int padRight = 2;
            int padTop = 0;
            int padBottom = 0;

            Console.WriteLine();
            Console.WriteLine(
                DecorationFrameDashed(NoticeShort(), indent, padLeft, padRight, padTop, padBottom));
            Console.WriteLine();
            // Reporter.ReportWarning(InitNotice());
        }

        /// <summary>Initializes global application data for the current class of application.</summary>
        public static void Init()
        {
            lock (lockGlobal)
            {
                if (!InitializedGlobal)
                {
                    ApplicationBase.InitApp();
                    Global = new AppTest();
                    Global.LaunchInitNotice();
                }
            }
        }

        #endregion

    }  // class AppTest


    /// <summary>Base class for all application classes. 
    /// Provides some basic functionality such as keeping information about the application, managing application
    /// directories and basic files, etc.
    /// </summary>
    /// $A Igor Oct08;
    public class ApplicationBase : Module, ILockable
    {

        #region Initialization


        /// <summary>Constructs a new application object.</summary>
        /// <param name="programName">Full name of the program.</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        public ApplicationBase(string programName, int version, int subVersion) :
            this(programName, DefaultApplicationCodeName, version, subVersion, 
                    DefaultSubSubVersion, DefaultRelease) { }


        /// <summary>Constructs a new application object.</summary>
        /// <param name="programName">Full name of the program.</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        /// 
        public ApplicationBase(string programName, int version, int subVersion, string release) :
            this(programName, DefaultApplicationCodeName, version, subVersion, 
                    DefaultSubSubVersion, release) { }

        /// <summary>Initializes the global data for the current program.</summary>
        /// <param name="programName">Full name of the program.</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        /// <param name="subSubVersion">Sub-subversion of the program. 
        /// A negative number means that this messagelevel of versioning is not used.</param>
        /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        /// 
        public ApplicationBase(string programName, int version, int subVersion, int subSubVersion, string release) :
            this(programName, DefaultApplicationCodeName, version, subVersion, subSubVersion, release) { }

        /// <summary>Initializes the global data for the current program.</summary>
        /// <param name="programName">Full name of the program.</param>
        /// <param name="codeName">Short program codename, appropriate for use in directory names.
        /// If not specified then it is automatically formed from the full name</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        /// 
        public ApplicationBase(string programName, string codeName, int version, int subVersion, string release) :
            this(programName, codeName, version, subVersion, DefaultSubSubVersion, release) { }

        /// <summary>Constructs a new application object.</summary>
        /// <param name="programName">Full name of the program.</param>
        /// <param name="codeName">Short program codename, appropriate for use in directory names.
        /// If not specified then it is automatically formed from the full name.</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        /// <param name="subSubVersion">Sub-subversion of the program. 
        /// A negative number means that this messagelevel of versioning is not used.</param>
        /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        public ApplicationBase(string programName, string codeName, int version, int subVersion,
                    int subSubVersion, string release): base(programName, codeName, version, subVersion,
                    subSubVersion, release)
        {
            ModuleInitialization(programName, codeName, version, subVersion, subSubVersion, release);
        }


        ///// <summary>Constructs a new application object.</summary>
        ///// <param name="programName">Full name of the program.</param>
        ///// <param name="codeName">Short program codename, appropriate for use in directory names.
        ///// If not specified then it is automatically formed from the full name.</param>
        ///// <param name="version">Version of the program.</param>
        ///// <param name="subVersion">Sub-version of the program.</param>
        ///// <param name="subSubVersion">Sub-subversion of the program. 
        ///// A negative number means that this messagelevel of versioning is not used.</param>
        ///// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        //protected virtual void InitProgram(string programName, string codeName, int version, int subVersion,
        //                           int subSubVersion, string release)
        //{
        //    ModuleInitializationCore(programName, codeName, version, subVersion,
        //                           subSubVersion, release);
        //}


        /// <summary>Pre-initialization stage.</summary>
        protected override void ModuleInitializationBefore()
        {
            base.ModuleInitializationBefore();
            IsModule = false;
            IsApplication = true;
        }


        #endregion Initialization


        #region CommandlineArguments

        protected string[] _commandLineArguments;

        /// <summary>Command-line arguments.</summary>
        public string[] CommandlineArguments
        {
            get { lock (Lock) { return _commandLineArguments; }  }
            protected set
            {
                lock (Lock)
                {
                    _commandLineArguments = value;
                    // Update dependencies:
                    NumCommandlineArguments = 0;
                    if (value != null)
                        NumCommandlineArguments = value.Length;
                }
            }
        }

        /// <summary>Sets command-line arguments of the application.</summary>
        /// <param name="args">Command-line arguments to be set.</param>
        public void SetCommandlineArguments(string[] args)
        {
            CommandlineArguments = args;
        }

        public int _numCommandlineArguments;

        /// <summary>Number of command-line arguments.</summary>
        public int NumCommandlineArguments
        {
            get { lock (Lock) { return _numCommandlineArguments; } }
            private set { _numCommandlineArguments = value; }
        }

        /// <summary>Parses command-line arguments and eventually sets various internal variables 
        /// according to their values.
        /// <para>This method should be overrideden in derived classes and executed somewhere in the application 
        /// (possibly in the initialization part).</para>
        /// <para>Base class' <see cref="ParseCommandlineArguments"/> should be called in the overridden methods.</para></summary>
        /// <param name="args">Command line arguments.</param>
        public virtual void ParseCommandlineArguments(string[] args)
        {
            this.CommandlineArguments = args;
        }

        #endregion CommandlineArguments


        #region IRunnable

        /// <summary>Runs the application.
        /// <para>This method should be overridden in each derived class.</para></summary>
        /// <param name="args">Arguments of the application.</param>
        public virtual void Run(string[] args)
        {
            throw new ArgumentException("This application can not be run, the Run(args) method is not implemented.");
        }

        #endregion IRunnable

        #region Actions

        protected List<Module> _modules = new List<Module>();


        protected int FindModuleIndex(Module m)
        {
            return _modules.FindIndex(
                delegate(Module compared) {
                return (m==compared);
                });
        }

        /// <summary>Adds the specified module to the application's list of modules.</summary>
        /// <param name="m">Module to be added.</param>
        public void AddModule(Module m)
        {
            lock(Lock)
            {
                int index = FindModuleIndex(m);
                if (index < 0)
                    _modules.Add(m);
            }
        }

        /// <summary>Removes the specified module to the application's list of modules.</summary>
        /// <param name="m">Module to be removed.</param>
        public void RemoveModule(Module m)
        {
            lock(Lock)
            {
                int index = FindModuleIndex(m);
                if (index >= 0)
                    _modules.Remove(m);
            }
        }


        #endregion Actions

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            if (_modules!=null)
                if (_modules.Count > 0)
                {
                    sb.AppendLine("Installed modules: ");
                    foreach (Module m in _modules)
                    {
                        if (m != null)
                            sb.AppendLine("  " + m.ToStringShort());
                    }
                }
            return sb.ToString();
        }

        #region Global


        /// <summary>Global application data lock.</summary>
        protected static object lockGlobal = new Object();

        protected static ApplicationBase _global = new ApplicationBase(DefaultApplicationName, DefaultVersion, DefaultSubVersion);

        /// <summary>Gets the global instance of the App class, representing the current program.</summary>
        public static ApplicationBase Global
        // $A Igor Oct08;
        {
            get
            {
                return _global;
            }
            protected set 
            {
                lock (lockGlobal)
                {
                    _global = value;
                }
            }
        }


        protected static bool _initializedGlobal = false;

        /// <summary>Gets a true/false value telling whether the global application data is initialized or not.</summary>
        public static bool InitializedGlobal
        // $A Igor Oct08;
        {
            get
            {
                return _initializedGlobal;
            }
            protected set
            {
                _initializedGlobal = true;
            }
        }  // property Name

        /// <summary>Application's reporter.</summary>
        public static IReporter Rep
        {
            get { return Global.Reporter; }
            set
            {
                lock (lockGlobal)
                {
                    Global.Reporter = value;
                }
            }
        }


        /// <summary>Initializes the global application data.
        /// This should be called in derived class' Init().</summary>
        protected static void InitApp()
        {
            lock (lockGlobal)
            {
                InitializedGlobal = true;
            }
        }

        #endregion Global



    }  // class ApplicationBase



    ///// <summary>
    ///// General module (or library) management class, a base class for specific module classes. 
    ///// Provides some basic functionality such as keeping information about the module, managing module
    ///// directories and basic files, etc.
    ///// Global module object is not implemented (in contrary to global program object), but it should be
    ///// implemented in speciffic module classes derived from this one.
    ///// </summary>
    ///// $A Igor Jul08;
    //public class Module : ModuleBase, ILockable
    //{


    //    #region Initialization

    //    /// <param name="moduleName">Full name of the module.</param>
    //    /// <param name="version">Version of the program.</param>
    //    /// <param name="subVersion">Sub-version of the program.</param>
    //    public Module(string moduleName, int version, int subVersion) :
    //        this(moduleName, DefaultApplicationCodeName, version, subVersion, 
    //                DefaultSubSubVersion, DefaultRelease) { }


    //    /// <param name="moduleName">Full name of the module.</param>
    //    /// <param name="version">Version of the program.</param>
    //    /// <param name="subVersion">Sub-version of the program.</param>
    //    /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
    //    /// 
    //    public Module(string moduleName, int version, int subVersion, string release) :
    //        this(moduleName, DefaultApplicationCodeName, version, subVersion, 
    //                DefaultSubSubVersion, release) { }

    //    /// <summary>Initializes the global data for the current module.</summary>
    //    /// <param name="moduleName">Full name of the program.</param>
    //    /// <param name="version">Version of the program.</param>
    //    /// <param name="subVersion">Sub-version of the program.</param>
    //    /// <param name="subSubVersion">Sub-subversion of the program. 
    //    /// A negative number means that this messagelevel of versioning is not used.</param>
    //    /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
    //    /// 
    //    public Module(string moduleName, int version, int subVersion, int subSubVersion, string release) :
    //        this(moduleName, DefaultApplicationCodeName, version, subVersion, subSubVersion, release) { }

    //    /// <param name="moduleName">Full name of the module.</param>
    //    /// <param name="codeName">Short program codename, appropriate for use in directory names.
    //    /// If not specified then it is automatically formed from the full name</param>
    //    /// <param name="version">Version of the program.</param>
    //    /// <param name="subVersion">Sub-version of the program.</param>
    //    /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
    //    public Module(string moduleName, string codeName, int version, int subVersion, string release) :
    //        this(moduleName, codeName, version, subVersion, DefaultSubSubVersion, release) { }

    //    /// <param name="moduleName">Full name of the module.</param>
    //    /// <param name="codeName">Short program codename, appropriate for use in directory names.
    //    /// If not specified then it is automatically formed from the full name.</param>
    //    /// <param name="version">Version of the program.</param>
    //    /// <param name="subVersion">Sub-version of the program.</param>
    //    /// <param name="subSubVersion">Sub-subversion of the program. 
    //    /// A negative number means that this messagelevel of versioning is not used.</param>
    //    /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
    //    public Module(string moduleName, string codeName, int version, int subVersion,
    //                int subSubVersion, string release)
    //    {
    //        ModuleInitialization(moduleName, codeName, version, subVersion, subSubVersion, release);  
    //    }


    //    ///// <param name="moduleName">Full name of the module.</param>
    //    ///// <param name="codeName">Short program codename, appropriate for use in directory names.
    //    ///// If not specified then it is automatically formed from the full name.</param>
    //    ///// <param name="version">Version of the program.</param>
    //    ///// <param name="subVersion">Sub-version of the program.</param>
    //    ///// <param name="subSubVersion">Sub-subversion of the program. 
    //    ///// A negative number means that this messagelevel of versioning is not used.</param>
    //    ///// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
    //    //void InitModule(string moduleName, string codeName, int version, int subVersion,
    //    //                           int subSubVersion, string release)
    //    //{
    //    //    ModuleInitialization(moduleName, codeName, version, subVersion,
    //    //                           subSubVersion, release);
    //    //}

    //    /// <summary>Pre-initialization stage.</summary>
    //    protected override void ModuleInitializationBefore()
    //    {
    //        base.ModuleInitializationBefore();
    //        IsModule = true;
    //        IsApplication = false;
    //    }


    //    // programName programName programName programName 

    //    #endregion Initialization


    //}  //  class Module





    /// <summary>
    /// General module (or library) management class, a base class for specific module classes. 
    /// Provides some basic functionality such as keeping information about the module, managing module
    /// directories and basic files, etc.
    /// Global module object is not implemented (in contrary to global program object), but it should be
    /// implemented in speciffic module classes derived from this one.
    /// </summary>
    /// $A Igor Jul08;
    public class Module : ILockable
    // Global clas provides basic application-messagelevel functionality.
    {
        
        /* Default creation date - update this manually with the current date when compiling! */
        protected const int  
            DD = 06,
            MM = 08,
            YYYY = 2016;

        #region Initialization

        /// <param name="moduleName">Full name of the module.</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        public Module(string moduleName, int version, int subVersion) :
            this(moduleName, DefaultApplicationCodeName, version, subVersion, 
                    DefaultSubSubVersion, DefaultRelease) { }


        /// <param name="moduleName">Full name of the module.</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        /// 
        public Module(string moduleName, int version, int subVersion, string release) :
            this(moduleName, DefaultApplicationCodeName, version, subVersion, 
                    DefaultSubSubVersion, release) { }

        /// <summary>Initializes the global data for the current module.</summary>
        /// <param name="moduleName">Full name of the program.</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        /// <param name="subSubVersion">Sub-subversion of the program. 
        /// A negative number means that this messagelevel of versioning is not used.</param>
        /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        /// 
        public Module(string moduleName, int version, int subVersion, int subSubVersion, string release) :
            this(moduleName, DefaultApplicationCodeName, version, subVersion, subSubVersion, release) { }

        /// <param name="moduleName">Full name of the module.</param>
        /// <param name="codeName">Short program codename, appropriate for use in directory names.
        /// If not specified then it is automatically formed from the full name</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        public Module(string moduleName, string codeName, int version, int subVersion, string release) :
            this(moduleName, codeName, version, subVersion, DefaultSubSubVersion, release) { }

        /// <param name="moduleName">Full name of the module.</param>
        /// <param name="codeName">Short program codename, appropriate for use in directory names.
        /// If not specified then it is automatically formed from the full name.</param>
        /// <param name="version">Version of the program.</param>
        /// <param name="subVersion">Sub-version of the program.</param>
        /// <param name="subSubVersion">Sub-subversion of the program. 
        /// A negative number means that this messagelevel of versioning is not used.</param>
        /// <param name="release">Lifecycle stage of the program version (alpha, beta, release, etc.)</param>
        public Module(string moduleName, string codeName, int version, int subVersion,
                    int subSubVersion, string release)
        {
            ModuleInitialization(moduleName, codeName, version, subVersion, subSubVersion, release);
        }




        #endregion Initialization



        #region ILockable


        protected object _lock = new object();

        /// <summary>Object's lock (implementation of ILockable).</summary>
        public object Lock
        { get { return _lock; } }

        #endregion ILockable


        #region Constants_and_Variables

        private static string _SoftwareRoot = null;  // stores the location of the software root directory

        // Constants:
        protected string
            _IGHomeEnv = null,  // environment variable assigned to hold the software root directory
            _IGHomeIdFile = "ighome.did",  // name of the software root identification file
            _IGHomeIdFileString = "/IGHOME/";



        protected string[] _IGHomeEnvList = new string[] { 
            "IGHOME", "IGHOME01", "IGHOME02", "IGHOME03", "IGHOME04", "IGHOME05", 
            "IGHOME_spare_01", "IGHOME_spare_02", "IGHOME_spare_03", "IGHOME_spare_04", "IGHOME_spare_05"
        };


        /// <summary>Environment variable that holds the software root directory.</summary>
        public string IGHomeEnv
        {
            get { return _IGHomeEnv; }
            protected set { _IGHomeEnv = value; }
        }

        /// <summary>Possible names for environment variable that holds the software root directory.</summary>
        public string[] IGHomeEnvList
        {
            get { return _IGHomeEnvList; }
            protected set { _IGHomeEnvList = value; }
        }

        /// <summary>Name of the software root identification file
        /// This file must be contained immediately in the software root directory.
        /// Its contents is checked in order to see whether a certain directory is actually the software root
        /// directory.</summary>
        public virtual string IGHomeIdFile
        {
            get { return _IGHomeIdFile; }
            protected set { _IGHomeIdFile = value; }
        }

        /// <summary>Name of the software root identification file
        /// This file must be contained immediately in the software root directory.
        /// Its contents is checked in order to see whether a certain directory is actually the software root
        /// directory.</summary>
        public virtual string IGHomeIdFileString
        {
            get { return _IGHomeIdFileString; }
            protected set { _IGHomeIdFileString = value; }
        }


        public XmlDocument Data=null; // program data



        // Internal variables:

        protected bool
            _isModule = false,
            _isApplication = false;


        protected string
            _Name = "IG's Test Program",
            _CodeName = null,  // name used in directory names; created automatically from name if not specified
            _Release = "Test",  // release mark (aest, debug, pre-alpha alpha, beta, pre-release, release-candidate, release (or gold))
            _User = null;  // name of the user that runs the program
        protected int 
            _version = 0, 
            _SubVersion = 0, 
            _SubSubVersion = 0  /* <0 means unspecified. */;
        protected int 
            _NumVersionDigits=2; // mnimal number of digits used in version string for each versioning messagelevel
        protected bool
            _useSubVersion = true,  // whether subversion number is used
            _useSubSubVersion = false; // whether sub-subversion number is used
        protected DateTime
            _StartTime = DateTime.Now,
            _CreationTime = new DateTime(YYYY, MM, DD);
            // _ExpireWarnTime,
            // _ExpireTime;
        protected TimeSpan
            _ValidityPeriod=new TimeSpan(100*366,0,0,0,0),
            _ExpireWarnPeriod = new TimeSpan(60,0,0,0,0);


        protected string 
            _Directory = null,  // directory containing common information for the program or module
            _VersionDirectory = null,  // directory containing data for specific version of the program
            _UserDirectory = null,     // directory containing user data for the current version of the program;
                        // Inside Version directory
            _SessionDirectory = null;


        #endregion // Constants_and_Variables


        #region Initialization

        // Initialization units:

 
        /// <summary>A method called before other initialization parts of the module.
        /// <para>This method is called before <see cref="ModuleInitializationCore"/> and before <see cref="ModuleInitializationAfter"/>.</para>
        /// <para>The method should not be called explicitly in derived classes. Instead, it is called from <see cref="ModuleInitialization"/>.</para>
        /// <para>Override this method in derived classes.</para></summary>
        /// $A Igor Oct08;
        protected virtual void ModuleInitializationBefore()
        {
            StartTime = DateTime.Now;
            IsModule = true;
            IsApplication = false;

            //if (!ModuleInitializationBeforeCalled)
            //{
            //    ModuleInitializationBeforeCalled = true;
            //    // In derived classes, override this method to set such things as IGHomeEnv or IGHomeIdFile;
            //    // Global makes possible to derive new ProgramBase-like classes with somewhat different bahavior.
            //}
        }


        /// <summary> A method called after other initializaton parts of the module.
        /// <para>This method is called after <see cref="ModuleInitializationBefore"/> and before <see cref="ModuleInitializationCore"/>.</para>
        /// <para>The method should not be called explicitly in derived classes. Instead, it is called from <see cref="ModuleInitialization"/>.</para>
        /// <para>Override this method in derived classes.</para></summary>
        protected virtual void ModuleInitializationAfter()
        // $A Igor Oct08;
        {
            //if (!ModuleInitializationAfterCalled)
            //{
            //    ModuleInitializationAfterCalled = true;
            //    // In derived classes, override this method to change initializers' behavior;
            //    // Global makes possible to derive new ProgramBase-like classes with somewhat different bahavior.
            //}
        }


        /// <summary>Performs the core initialization step of the program or module.
        /// <para>This method is called after <see cref="ModuleInitializationBefore"/> and before <see cref="ModuleInitializationAfter"/>.</para>
        /// <para>The method should not be called explicitly in derived classes. Instead, it is called from <see cref="ModuleInitialization"/>.</para>
        /// <para>Override this method in derived classes.</para></summary>
        /// <param name="name">Name of the program or module.</param>
        /// <param name="codeName">Code name (short name, shoud not contain spaces).</param>
        /// <param name="version">Version numver.</param>
        /// <param name="subVersion">Subversion number.</param>
        /// <param name="subSubVersion">Sub-subversion number.</param>
        /// <param name="release">Release description (e.g. "pre-release", "alpha", "beta", "experimental")</param>
        /// $A Igor Oct08;
        protected virtual void ModuleInitializationCore(string name, string codeName, int version, int subVersion, 
                                   int subSubVersion, string release)
        {
            if (!String.IsNullOrEmpty(name)) Name = name;
            if (!String.IsNullOrEmpty(codeName)) CodeName = codeName;
            if (version>=0) Version = version;
            if (subVersion>=0) SubVersion = subVersion;
            if (subSubVersion>0) SubSubVersion = subSubVersion;
            if (release!=null) if (release.Length>0) Release = release;
        }

        private bool ModuleInitializationBeforeCalled = false;
        private bool ModuleInitializationCoreCalled = false;
        private bool ModuleInitializationAfterCalled = false;


        /// <summary>Performs complete initialization of the program or module, including the pre-initialization, core
        /// initializattion and post-initialization steps.</summary>
        /// <param name="moduleName">Name of the program or module.</param>
        /// <param name="codeName">Code name (short name, shoud not contain spaces).</param>
        /// <param name="version">Version numver.</param>
        /// <param name="subVersion">Subversion number.</param>
        /// <param name="subSubVersion">Sub-subversion number.</param>
        /// <param name="release">Release description (e.g. "pre-release", "alpha", "beta", "experimental")</param>
        /// $A Igor Oct08;
        protected void ModuleInitialization(string moduleName, string codeName, int version, int subVersion,
                                   int subSubVersion, string release)
        {
            if (!ModuleInitializationBeforeCalled)
            {
                ModuleInitializationBefore();
                ModuleInitializationBeforeCalled = true;
            }
            if (!ModuleInitializationCoreCalled)
            {
                ModuleInitializationCore(moduleName, codeName, version, subVersion, subSubVersion, release);
                ModuleInitializationCoreCalled = true;
            }
            if (!ModuleInitializationAfterCalled)
            {
                ModuleInitializationAfter();
                ModuleInitializationAfterCalled = true;
            }
        }


        #endregion   // Initialization
        
            
        #region Constants

        public const string 
            DefaultModuleName = "Test Library",
            DefaultApplicationName = "Test Application",
            DefaultModuleCodeName = null,
            DefaultApplicationCodeName = null,
            DefaultRelease = "final";

        public const int
            DefaultVersion = 0,
            DefaultSubVersion = 0,
            DefaultSubSubVersion = 0;



        #endregion Constants


        #region Properties

        public String
            Email = null,  // program'result contact e-mail
            WebPage = null,  // program'result web page
            Phone = null,  // program'result contact telephone
            AuthorFirstName = null,  // principal author'result first name
            AuthorMidName = null, // principal author'result middle name
            AuthorSecondName = null,  // principal author'result second name
            AuthorAddress1 = null,  // principal author'result address, first line
            AuthorAddress2 = null, // principal author'result address, second line
            AuthorAddress3 = null, // principal author'result address, third line
            AuthorAddress4 = null;
        public String[] Authors = null;


        /// <summary>Gets a treue/false value telling whether the global program data is initialized or not.</summary>
        public string AuthorFullName
        {
            get
            {
                string ret = "";
                if (AuthorFirstName!=null) if (AuthorFirstName.Length>0)
                    ret += AuthorFirstName;
                if (AuthorMidName != null) if (AuthorMidName.Length > 0)
                    ret += " " + AuthorMidName;
                if (AuthorSecondName != null) if (AuthorSecondName.Length > 0)
                    ret += " " + AuthorSecondName;
                return ret;
            }
        }

        /// <summary>Gets complete developer's address (combination of individual address lines).</summary>
        public string AuthorAddress
        {
            get 
            {
                string ret = "";
                if (!String.IsNullOrEmpty(AuthorAddress1))
                {
                    if (!String.IsNullOrEmpty(ret))
                        ret += Environment.NewLine;
                    ret += AuthorAddress1;
                }
                if (!String.IsNullOrEmpty(AuthorAddress2))
                {
                    if (!String.IsNullOrEmpty(ret))
                        ret += Environment.NewLine;
                    ret += AuthorAddress2;
                }
                if (!String.IsNullOrEmpty(AuthorAddress3))
                {
                    if (!String.IsNullOrEmpty(ret))
                        ret += Environment.NewLine;
                    ret += AuthorAddress3;
                }
                if (!String.IsNullOrEmpty(AuthorAddress4))
                {
                    if (!String.IsNullOrEmpty(ret))
                        ret += Environment.NewLine;
                    ret += AuthorAddress4;
                }
                return ret;
            }
        }

        string astr=null;
        private void Dummy()
        // Dummy function whose role is to eliminate compiler warning messages.
        {
            astr = Email;
            astr = WebPage;
            astr = Phone;
            if (Authors!=null) if (Authors.Length>0) 
                astr = Authors[0];
        }


        /// <summary>Whether the current object represents an application.</summary>
        public virtual bool IsApplication
        {
            get { return _isApplication; }
            protected set { _isApplication = value; }
        }

        /// <summary>Whether the current object represents a module (not an application).</summary>
        public virtual bool IsModule
        {
            get { return _isModule; }
            protected set { _isModule = value; }
        }
        

        /// <summary>Gets or sets the full name of the program.</summary>
        public virtual string Name
        // $A Igor Oct08;
        {
            get { return _Name; }
            set
            {
                _Name = value;
                /* Reset dependent internal variables: */
                CodeName = null;  // this will re-set s dependent variables
            }
        }


        /// <summary>Converts a module or program name to a valid code name.</summary>
        /// <param name="Name">Full name of the module or program.</param>
        /// <returns>Corresponding code name.</returns>
        protected virtual string ToCodeName(string Name)
        {
            string ret = Name ;
            if (ret != null) if (ret.Length > 0)
                {
                    ret = ret.ToLower().Replace(" ", "_");
                    for (int i = 0; i < ret.Length; ++i)
                    {
                        if (!(char.IsLetterOrDigit(ret[i]) || ret[i] == '_'))
                        {
                            ret = ret.Remove(i, 1).Insert(i, "_");  // .Insert(i, "_"));
                        }
                    }
                }
            return ret;
        }

        /// <summary>Gets or sets code name of the program.</summary>
        /// <summary>Code name is used in program standard directory names.</summary>
        /// <summary>If not specified, it is automatically derived from program full name.</summary>
        /// <summary>Get must not be called unles wither CodeName or Name has been set.</summary>
        public string CodeName
        // $A Igor Oct08;
        {
            get
            {
                if (_CodeName == null && !String.IsNullOrEmpty(Name))
                {
                    // Internal CodeName is not defined, therefore it is derived form Name
                    string ret = ToCodeName(Name);
                    _CodeName = ret;  // call the setter, in this way dependencies are also updated
                }
                if (_CodeName == null)
                    throw new ApplicationException("Can not retrieve the program's code name.");
                return _CodeName;
            }
            set
            {
                _CodeName = value;
                /* Reset dependent internal variables: */
                BaseDirectory = null;
                VersionDirectory = null;
                UserDirectory = null;
                SessionDirectory = null;
            }
        }  // property CodeName

        /// <summary>Gets or sets the main version number of the program.</summary>
        public int Version
        // $A Igor Oct08;
        {
            get { return _version; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Version number must not be less than 0.");
                _version = value;
                /* Reset dependent internal variables: */
                VersionDirectory = null;
                UserDirectory = null;
                SessionDirectory = null;
            }
        }  // property Name

        /// <summary>Gets or sets the subversion number of the program.</summary>
        /// <summary>Gets or sets the main version of the program.</summary>
        /// $A Igor Oct08;
        public int SubVersion
        {
            get { return _SubVersion; }
            set 
            {
                if (value < 0)
                    throw new ArgumentException("Subversion number must not be less than 0.");
                _SubVersion = value;
                /* Reset dependent internal variables: */
                VersionDirectory = null;
                UserDirectory = null;
                SessionDirectory = null;
            }
         }  // property SubVersion

        /// <summary>Gets or sets the sub-subversion number of the program.</summary>
        /// <summary>Negative number means that this messagelevel of versioning is not used.</summary>
        public int SubSubVersion
        // $A Igor Oct08;
        {
            get { return _SubSubVersion; }
            set 
            {
                if (value < 0)
                    throw new ArgumentException("Sub-subversion number must not be less than 0.");
                _SubSubVersion = value; 
            } // directories don't depend on sub-subversion in the current arrangement
        }  // property SubSubVersion

        /// <summary>Gets or sets the release specification of the program (e.g. "alpha", "beta", "release"...).</summary>
        public string Release
        // $A Igor Oct08;
        {
            get { return _Release; }
            set { _Release = value; } // directories don't depend on release spec. in the current arrangement
        }  // property Release

        /// <summary>Gets The starting time of the program (time when program data was initialized).</summary>
        public virtual DateTime StartTime  // virtual enables to make setter public when necessary.
        // $A Igor Oct08;
        {
            get 
            {
                if (_StartTime == null)
                    _StartTime = DateTime.Now;
                return _StartTime; 
            }
            private set
            {
                _StartTime = value;
                // Reset dependencies (use properties to update cascaded dependencies)
                SessionDirectory = null;
            }
        }


        /// <summary>Calculates program creation time of the current executable (the entry assembly) and returns it.</summary>
        /// <returns>Creation time of the current executable.</returns>
        public DateTime CalculateCreationTime()
        // $A Igor Oct08;
        {
            string AssemblyPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            FileInfo af = new FileInfo(AssemblyPath);
            return af.LastWriteTime;
        }


        /// <summary>Calculates creation time of the assembly containing the specified class and returns it.</summary>
        /// <param name="type">Class whose assembly'result creation time is returned.</param>
        /// <returns>Creation time of the assembly containing the specified class.</returns>
        public DateTime CalculateCreationTime(Type type)
        // $A Igor Oct08;
        {
            string AssemblyPath = System.Reflection.Assembly.GetAssembly(type).Location;
            FileInfo af = new FileInfo(AssemblyPath);
            return af.LastWriteTime;
        }



        /// <summary>Gets or sets craation time of the program.</summary>
        /// <remarks>Get throws an exception if the creation time has not been set before.</remarks>
        /// <remarks>Setting to some value is normally automatically performed by a class constructur</remarks>
        /// <remarks>and it is valid to override this later.</remarks>
        public DateTime CreationTime
        // $A Igor Oct08;
        {
            get 
            {
                if (_CreationTime == null)
                    throw new ApplicationException("Program creation time is not specified.");
                return _CreationTime; 
            }
            protected set { _CreationTime = value;  }
        }  // property CreationTime


        public bool _expires = true;

        /// <summary>Gets or sets the flag telling whether the program expires.</summary>
        public virtual bool Expires
        {
            get { return _expires; }
            set { _expires = value; }
        }


        /// <summary>Gets or sets the validity period length for the program.</summary>
        /// <summary>Validity period is time that elapses between creation and expiration of the program.</summary>
        public TimeSpan ValidityPeriod
        // $A Igor Oct08;
        {
            get 
            {
                if (_ValidityPeriod == null)
                    throw new ApplicationException("Pragram validity period is not specified.");
                return _ValidityPeriod; 
            }
            set { _ValidityPeriod = value; }
        }

        /// <summary>Gets or sets the length of the expiration warning period.
        /// This is the period of time between expiration of the software and time when
        /// the software actually stops working. If not set, 0 is taken.</summary>
        private TimeSpan ExpireWarnPeriod
        // $A Igor Oct08;
        {
            get
            {
                if (_ExpireWarnPeriod == null)
                    _ExpireWarnPeriod = new TimeSpan(0, 0, 0);
                return _ExpireWarnPeriod;
            }
            set { _ExpireWarnPeriod = value; }
        }

        /// <summary>Gets or sets the expiration time of the program.
        /// Property is automatically updated if the validity period is changed.</summary>
        public DateTime ExpireTime
        // $A Igor Oct08;
        {
            get { return CreationTime+ValidityPeriod;  }
            set { ValidityPeriod = value - CreationTime; }
        }  // property ExpireWarnTime

        /// <summary>Gets or sets the time when program will stop functioning completely.</summary>
        /// <summary>Global property is automatically updated if the validity period is changed.</summary>
        public DateTime ExpireFinalTime
        // $A Igor Oct08;
        {
            get  { return CreationTime+ValidityPeriod+ExpireWarnPeriod; }
            set  { ExpireWarnPeriod = value - ExpireTime; }
        }  // property ExpireTime

        /// <summary>Gets the indicatin whether the program has expired.</summary>
        public bool HasExpired
        // $A Igor Oct08;
        {
            get
            {
                if (!Expires)
                    return false;
                else if (ExpireTime == null || StartTime == null)
                    return false;
                else
                    return (StartTime > ExpireTime);
            }
        }

        /// <summary>Gets the indicatin whether the program has stopped being functional because of expiration.
        /// This happens after final expiration time has reach, which follows a certain period after the
        /// regular expiration time.</summary>
        public bool HasExpiredFinal
        // $A Igor Oct08;
        {
            get 
            {
                if (!Expires)
                    return false;
                else if (ExpireFinalTime == null || StartTime == null)
                    return false;
                else
                    return (StartTime > ExpireFinalTime);
            }
        }

        /// <summary>Gets the number of days until the expiration of the program or module 
        /// (0 or negative if already expired).</summary>
        public int DaysTillExpire
        // $A Igor Oct08;
        {
            get
            {
                if (!Expires)
                    return 100000;
                else if (ExpireTime == null || StartTime == null)
                    return 0;
                else
                    return (ExpireTime-StartTime).Days;
            }
        }

        /// <summary>Gets the number of days until the final expiration of the program or module,
        /// when the software will stop functioning (0 or negative if already expired).</summary>
        public int DaysTillExpireFinal
        // $A Igor Oct08;
        {
            get
            {
                if (!Expires)
                    return 100000;
                else if (ExpireFinalTime == null || StartTime == null)
                    return 0;
                else
                    return (ExpireFinalTime-StartTime).Days;
            }
        }



        /// <summary>Number of digits that are output in version strings.</summary>
        public int NumVersionDigits
        {
            get { return _NumVersionDigits; }
            set
            {
                _NumVersionDigits = value;
                if (_NumVersionDigits < 1)
                    _NumVersionDigits = 1;
            }
        }

        /// <summary>Whether or not subversion number is used.</summary>
        public bool UseSubVersion
        {
            get { return _useSubVersion; }
            set
            {
                _useSubVersion = value;
            }
        }

        /// <summary>Whether or not sub-subversion number is used.</summary>
        public bool UseSubSubVersion
        {
            get { return _useSubSubVersion; }
            set
            {
                _useSubSubVersion = value;
            }
        }

        /// <summary>Gets the string that represents program version.</summary>
        public string VersionString
        {
            get
            {
                string ret = "";
                string sub;
                if (true)
                {
                    sub = Version.ToString();
                    while (sub.Length < NumVersionDigits)
                        sub = "0" + sub;
                    ret += sub;
                }
                if (UseSubVersion)  // subversion number is used in versioning
                {
                    sub = SubVersion.ToString();
                    while (sub.Length < NumVersionDigits)
                        sub = "0" + sub;
                    ret += ("." + sub);
                }
                if (UseSubSubVersion)  // sub-subversion number is also used in versioning
                {
                    sub = SubSubVersion.ToString();
                    while (sub.Length < NumVersionDigits)
                        sub = "0" + sub;
                    ret += ("." + sub);
                }
                return ret;
            }
        }

        /// <summary>Gets the string representation of program version to be used in directory and file names.
        /// Sub-subversion is not included in the string.</summary>
        private string DirectoryVersionString
        {
            get
            {
                string ret = "";
                string sub = Version.ToString();
                while (sub.Length < NumVersionDigits)
                    sub = "0" + sub;
                ret += sub;
                if (ret.Length < 1)
                    ret = "";
                if (SubVersion >= 0)  // subversion number is used in versioning
                {
                    sub = SubVersion.ToString();
                    while (sub.Length < NumVersionDigits)
                        sub = "0" + sub;
                    ret += "." + sub;
                }
                // Sub-sub version is not used in version part of directory names.
                return ret;
            }
        }


        /// <summary>Gets name of the user of the program.</summary>
        public string User
        // Gets the user that runs the current program.
        // $A Igor Oct08;
        {
            get
            {
                if (_User != null)
                    if (_User.Length > 0)
                        return _User;  // user already specified
                // User is not yet determined, retrieve it from environment:
                string ret = null;
                bool userset = false;
                ret = Environment.UserName;
                if (ret!=null) if (ret.Length > 0)
                    userset = true;
                if (!userset)
                    throw new ApplicationException("Current user could not be determined.");
                User=ret;  // call property setter to update dependent variables.
                return ret;
            }
            internal set
            {
                _User = value;
                /* Reset dependent internal variables: */
                UserDirectory=null;
                SessionDirectory = null;
            }
        }

        protected string _modulesDir = "modules";

        protected string _applicationsDir = "applications";

        /// <summary>Gets the directory containing individual module or application directories.</summary>
        protected virtual String SoftwareParentDirectory
        {
            get 
            {
                if (IsApplication)
                {
                    return Path.Combine(SoftwareRoot, _applicationsDir);
                } else
                    return Path.Combine(SoftwareRoot, _modulesDir);
            }
        }

        /// <summary>Gets or sets the program directory (containing all version directories, user directories, etc.).
        /// Setting of program directory should normally be left to the system.</summary>
        public virtual string BaseDirectory
        {
            get 
            { 
                if (_Directory==null)
                {
                    _Directory = Path.Combine(SoftwareParentDirectory, CodeName);
                    if (_Directory==null)
                        throw new ApplicationException("Program directory path could not be determined.");
                    else
                    {
                        // ProgramBase directory has been determined, create the directory if it des not exist:
                        if (!Directory.Exists(_Directory))
                        {
                            Directory.CreateDirectory(_Directory);
                            if (!Directory.Exists(_Directory))
                            {
                                string dir = _Directory;
                                _Directory = null;
                                throw new ApplicationException("Program directory could not be created. Path: "
                                        + dir + ".");
                            }
                        }
                    }
                }
                return _Directory;
            }
            set 
            { 
                _Directory = value;
                // reset dependent things:
                VersionDirectory = null;
                UserDirectory = null;
                SessionDirectory = null;
            }
        }

        


        /// <summary>Gets or sets the program'result specific version directory.
        /// Setting of this directory should normally be left to the system.</summary>
        public virtual string VersionDirectory
        {
            get 
            {
                if (_VersionDirectory == null)
                {
                    string parentdir=BaseDirectory;
                    _VersionDirectory=Path.Combine(parentdir,"ver_" + DirectoryVersionString);
                    if (_VersionDirectory==null)
                        throw new ApplicationException("Program version directory can not be determined.");
                    else
                    {
                        // Create the directory if it does not yet exist:
                        if (!Directory.Exists(_VersionDirectory))
                        {
                            Directory.CreateDirectory(_VersionDirectory);
                            if (!Directory.Exists(_VersionDirectory))
                            {
                                string dir =_VersionDirectory;
                                _VersionDirectory = null;
                                throw new ApplicationException("Program version directory could not be created. Path: "
                                    + dir);
                            }
                        }
                    }
                }
                return _VersionDirectory;
            }
            set { 
                _VersionDirectory = value;
                // Reset dependent things:
                UserDirectory = null;
                SessionDirectory = null;
            }
        }

        /// <summary>Gets or sets the spedcific user'result directory for the current version of the progrm.
        /// Setting of this directory should normally be left to the system.</summary>
        public virtual string UserDirectory
        {
            get 
            {
                if (_UserDirectory == null)
                {
                    string parentdir = VersionDirectory;
                    _UserDirectory = Path.Combine(parentdir, "user_" + ToCodeName(User));
                    if (_UserDirectory == null)
                        throw new ApplicationException("Program user directory can not be determined.");
                    else
                    {
                        // Create the directory if it does not yet exist:
                        if (!Directory.Exists(_UserDirectory))
                        {
                            Directory.CreateDirectory(_UserDirectory);
                            if (!Directory.Exists(_UserDirectory))
                            {
                                string dir = _UserDirectory;
                                _UserDirectory = null;
                                throw new ApplicationException("Program user directory could not be created. Path: "
                                    + dir);
                            }
                        }
                    }
                }
                return _UserDirectory;
            }
            set 
            { 
                _UserDirectory = value;
                // Reset dependent things:
                SessionDirectory = null;
            }
        }

        /// <summary>Gets or sets the current session'result directory.
        /// Setting of this directory should normally be left to the system.</summary>
        public virtual string SessionDirectory
        {
            get 
            {
                if (_SessionDirectory == null)
                {
                    string parentdir = UserDirectory;
                    string uniquestr = null;
                    // TODO: serialize retrieval of the uniquestr in such a way that no two sessions
                    // can obtain the same uniquestr (cross-program serialization where parent directory can
                    // be used)
                    uniquestr = DateTime.Now.Ticks.ToString();
                    _SessionDirectory = Path.Combine(parentdir, "session_" + uniquestr);
                    if (_SessionDirectory == null)
                        throw new ApplicationException("Program session directory can not be determined.");
                    else
                    {
                        // Create the directory if it does not yet exist:
                        if (!Directory.Exists(_SessionDirectory))
                        {
                            Directory.CreateDirectory(_SessionDirectory);
                            if (!Directory.Exists(_SessionDirectory))
                            {
                                string dir = _SessionDirectory;
                                _SessionDirectory = null;
                                throw new ApplicationException("Program session directory could not be created. Path: "
                                    + dir);
                            }
                        }
                    }
                }
                return _SessionDirectory;
            }
            set { _SessionDirectory = value; }
        }


        /// <summary>Creates the software root direcroty with a specified path. The directory must not yet exist.
        /// The identification file is created in the directory such that its validity can be verified later.</summary>
        /// <param name="rootpath">Path of the software root directory.</param>
        internal virtual bool CreateSoftwareRoot(string rootpath)
        // Creates the software rood directory (usually named IGHOME) at the path rootpath and returns true if successful.
        // If the directory already wxists then na exception is thrown.
        // 
        {
            bool ret = false;
            try
            {
                bool direxists = false;
                try
                {
                    if (Directory.Exists(rootpath))
                        direxists = true;
                }
                catch (Exception) { }
                if (direxists)
                    throw new Exception("Directory already exists.");
                else
                {
                    Directory.CreateDirectory(rootpath);
                    if (Directory.Exists(rootpath))
                    {
                        string IdPath = Path.Combine(rootpath, IGHomeIdFile);
                        using (StreamWriter sw = new StreamWriter(IdPath))
                        {
                            DateTime t=DateTime.Now;
                            sw.WriteLine(); sw.WriteLine();
                            sw.WriteLine("/IGHOME/ {"
                                + t.Day + "."
                                + t.Month + "."
                                + t.Year
                                + "}");
                            sw.WriteLine();
                            sw.WriteLine("* {");
                            sw.WriteLine("This file identifies that its containing directory is the software home");
                            sw.WriteLine("directory for programs designed by Igor Gresovnik or for programs that");
                            sw.WriteLine("use Igor's utilities.");
                            sw.WriteLine("");
                            sw.WriteLine("This file should only be present in the root software home, usually named");
                            sw.WriteLine("ighome. It helps programs to determine the ighome directory when the $IGHOME");
                            sw.WriteLine("system variable is not set.");
                            sw.WriteLine("");
                            sw.WriteLine("Please do not change the contents of this file or delete the file.");
                            sw.WriteLine("");
                            sw.WriteLine("$A Igor jan02;");
                            sw.WriteLine("}");
                            sw.WriteLine();
                        }
                        if (File.Exists(IdPath))
                            ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in creating the IGHOME directory. Details: " 
                    + ex.Message + " Directory: " + rootpath + "." );
            }
            return ret;
        }

        /// <summary>Verifies whether the specified directory is a valid software root directory.
        /// Global is verified by the standard identtification file that was put into the directory at its creation.</summary>
        /// <param name="rootpath">Path of the software root directory to be checked.</param>
        public virtual bool CheckSoftwareRootValidity(string rootpath)
        // Checks the validity of the software root directory and returns true if rootpath is a valid
        // software home directory, or false otherwise.
        // $A Igor oct08;
        {
            bool ret = false;
            try
            {
                if (!String.IsNullOrEmpty(rootpath)) if (Directory.Exists(rootpath))
                {
                    string rootIdFile = Path.Combine(rootpath,IGHomeIdFile);
                    if (File.Exists(rootIdFile))
                    {
                        try
                        {
                            using (StreamReader sr = new StreamReader(rootIdFile,new ASCIIEncoding()))
                            {
                                string contents = sr.ReadToEnd();
                                if (contents.Contains(IGHomeIdFileString))
                                {
                                    ret=true;
                                }
                            }
                        }
                        catch(Exception)
                        {
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error in testing valididy of IGHOME directory. Details: " 
                    + ex.Message + " Directory: " + rootpath + "." );
            }

            return ret;
        }



        /// <summary>Gets or sets the software root directory.
        /// Setting it should normally be left to the system.</summary>
        public virtual string SoftwareRoot   // should it be static?
        // Returns the program base directory
        {
            get
            {
                bool defined = false, changed=false;
                try
                {
                    if (_SoftwareRoot != null)
                        if (_SoftwareRoot.Length > 0)
                            if (Directory.Exists(_SoftwareRoot))
                                defined = true;
                    if (!defined)
                    {
                        // Software root is not yet defined, define it:
                        changed = true;
                        string dirpath;


                        foreach (string envVar in IGHomeEnvList)
                        {
                            if (!defined && !String.IsNullOrEmpty(envVar))
                            {
                                dirpath = Environment.GetEnvironmentVariable(envVar);
                                if (CheckSoftwareRootValidity(dirpath))
                                {
                                    _SoftwareRoot = dirpath;
                                    IGHomeEnv = envVar;
                                    defined = true;
                                }
                            }
                        }
                        if (!defined)
                        {
                            // We could not find a valid software root directory, try to create it:
                            foreach (string envVar in IGHomeEnvList)
                            {
                                if (!defined && !String.IsNullOrEmpty(envVar))
                                {
                                    dirpath = Environment.GetEnvironmentVariable(envVar);
                                    if (!Directory.Exists(dirpath))
                                    {
                                        CreateSoftwareRoot(dirpath);
                                        if (CheckSoftwareRootValidity(dirpath))
                                        {
                                            _SoftwareRoot = dirpath;
                                            IGHomeEnv = envVar;
                                            defined = true;
                                        } else
                                            throw new ApplicationException("Software root get: The software root directory is not valid after creation: "
                                            + dirpath + ".");
                                    }
                                    else
                                    {
                                        // Directory exists but it does not appear to be a valid IGHOME directory:
                                        throw new ApplicationException("Software root get: The Directory does not appear to be a valid software root directory: "
                                            + dirpath + ".");
                                    }

                                }
                            }
                        }
                    }

                    if (!defined)
                        throw new ApplicationException("Software root get: The path of the software root directory can not be determined.");
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Software root get: Directory can not be determined. Details: " + ex.Message);
                }
                finally
                {
                    if (changed)
                    {
                        // Reset dependent properties:
                        BaseDirectory = null;
                        UserDirectory = null; 
                        SessionDirectory = null;
                    }
                }
                return _SoftwareRoot;
            }  // // SoftwareRoot get
            protected set
            {
                bool defined = false, changed = true;
                try
                {
                    _SoftwareRoot = null;
                    if (value != null)
                    {
                        string dirpath = value;
                        if (CheckSoftwareRootValidity(dirpath))
                        {
                            _SoftwareRoot = dirpath;
                            defined = true;
                        }
                        else if (!Directory.Exists(dirpath))
                        {
                            CreateSoftwareRoot(dirpath);
                            if (CheckSoftwareRootValidity(dirpath))
                            {
                                _SoftwareRoot = dirpath;
                                defined = true;
                            }
                            else
                                throw new ApplicationException("Set Software root: The software root directory is not valid after creation: "
                                + dirpath + ".");
                        }
                        else
                        {
                            // Directory exists but it does not appear to be a valid software root directory:
                            throw new ApplicationException("Set Software root: The Directory does not appear to be a valid software root directory: "
                                + dirpath + ".");
                        }
                    }
                    if (!defined)
                        throw new ApplicationException("The path of the IGHOME directory can not be determined.");
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Software root set: Directory can not be set. Detail: " + ex.Message);
                }
                finally
                {
                    if (changed)
                    {
                        // Reset dependent properties:
                        BaseDirectory = null;
                        VersionDirectory = null;
                        UserDirectory = null;
                        SessionDirectory = null;
                    }
                }
            } // SoftwareRoot set
        }  // SoftwareRoot property

        #endregion  // Properties


        #region Actions

        /// <summary>Retrurns a short string containing very basic information about the application or module.</summary>
        public string ToStringShort()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(" ("); sb.Append(CodeName); sb.Append(") ");
            sb.Append(VersionString);
            sb.Append(" ");
            sb.Append(Release);
            return sb.ToString();
        }

        /// <summary>Returns a string containing basic data of the module or application.</summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsApplication)
                sb.AppendLine("Application data: ");
            else
                sb.AppendLine("Module data: ");
            sb.AppendLine("Name: " + Name);
            sb.AppendLine("Code name: " + CodeName);
            sb.AppendLine("Version: " + Version);
            sb.AppendLine("Subversion: " + SubSubVersion);
            sb.AppendLine("Sub-subversion: " + SubSubVersion);
            sb.AppendLine("Release: " + Release);
            sb.AppendLine("E-mail: " + Email);
            sb.AppendLine("Web page: " + WebPage);
            sb.AppendLine("Author's name: " + AuthorFirstName);
            sb.AppendLine("Author's middle names: " + AuthorMidName);
            sb.AppendLine("Author's second name: " + AuthorSecondName);
            sb.AppendLine("Author's full name: " + AuthorFullName);
            string addrStr = AuthorAddress;
            if (!string.IsNullOrEmpty(AuthorAddress))
                sb.AppendLine("Author's address: " + Environment.NewLine + addrStr);
            else
                sb.AppendLine("Author's address is not specified.");
            sb.AppendLine("Other authors: " + Authors);
            sb.AppendLine("Environment variable for software root: " + IGHomeEnv);
            sb.AppendLine("Software root directory: " + SoftwareRoot);
            sb.AppendLine("Software parent directory: " + SoftwareParentDirectory);
            if (IsApplication)
                sb.Append("Application directory: " + BaseDirectory);
            else
                sb.AppendLine("Module directory: " + BaseDirectory);
            sb.AppendLine("User: " + User);
            sb.AppendLine("User directory: " + UserDirectory);
            sb.AppendLine("Session directory: " + SessionDirectory);
            sb.AppendLine("Start time: " + StartTime);
            sb.AppendLine("Creation time: " + CreationTime);
            if (!Expires)
                sb.AppendLine("Software does not expire.");
            else
            {
                sb.AppendLine("Software expires.");
                if (HasExpired)
                {
                    if (HasExpiredFinal)
                        sb.AppendLine("Expired completely.");
                    else
                        sb.AppendLine("Already expired, still working.");
                } else
                {
                    sb.AppendLine("Not yet expired.");
                    sb.AppendLine("Days till expiration: " + DaysTillExpire);
                }
                sb.AppendLine("Expiration time: " + ExpireTime);
                sb.AppendLine("Final expiration time: " + ExpireFinalTime);
                sb.AppendLine("Expiration period: " + ExpireWarnPeriod);
            }
            return sb.ToString();
        }


        /// <summary>Returns a list of text lines introducing the module or application in
        /// a condensed way.</summary>
        public virtual List<String> NoticeShort()
        {
            return Notice(false /* useCodename */, true /* printVersion */, true /* printRelease */,
                false /* printAuthor */, false /* printAddress */, 
                false /* printWebPage */, false /* printEmail */);
        }

        /// <summary>Returns a list of text lines introducing the module or application in.</summary>
         public virtual List<String> Notice()
        {
            return Notice(false /* useCodename */, true /* printVersion */, true /* printRelease */,
                true /* printAuthor */, true /* printAddress */, 
                true /* printWebPage */, true /* printEmail */);
        }


        /// <summary>Returns a list of text lines introducing the module or application.</summary>
        /// <param name="useCodename">If true then code name is used instead of full module/application name.</param>
        /// <param name="printVersion">Whether version information is printed.</param>
        /// <param name="printRelease">Whether release information is printed.</param>
        /// <param name="printAuthor">Whether software's author is printed.</param>
        /// <param name="printAuthorAddress">Whether author's address is printed.</param>
        /// <param name="printWebPage">Whether software's web page is printed.</param>
        /// <param name="printEmail">Whether software's e-mail address is printed.</param>
        /// <returns>A list of lines that constitute the notice with information about the software.
        /// There are no leading or ending empty lines.</returns>
        public virtual List<String> Notice(bool useCodename, bool printVersion, bool printRelease,
            bool printAuthor, bool printAuthorAddress, bool printWebPage, bool printEmail)
        {
            List<string> ret = new List<string>();
            string line;
            line = Name;
            if (printVersion && !string.IsNullOrEmpty(VersionString))
                line += " v. " + VersionString;
            if (printRelease && !string.IsNullOrEmpty(Release))
                line += " " + Release;
            ret.Add(line);
            if (printAuthor)
            {
                line = AuthorFullName;
                if (!string.IsNullOrEmpty(line))
                {
                    ret.Add("");
                    ret.Add("  Developed by: ");
                    ret.Add("    " + line);
                    if (printAuthorAddress)
                    {
                        if (!string.IsNullOrEmpty(AuthorAddress1))
                            ret.Add("    " + AuthorAddress1);
                        if (!string.IsNullOrEmpty(AuthorAddress2))
                            ret.Add("    " + AuthorAddress2);
                        if (!string.IsNullOrEmpty(AuthorAddress3))
                            ret.Add("    " + AuthorAddress3);
                        if (!string.IsNullOrEmpty(AuthorAddress4))
                            ret.Add("    " + AuthorAddress4);
                    }
                }
            }
            if (  (printWebPage && !string.IsNullOrEmpty(WebPage)) 
                || (printEmail && !string.IsNullOrEmpty(Email))  )
            {
                ret.Add("");
                if (printWebPage && !string.IsNullOrEmpty(WebPage))
                    ret.Add(WebPage);
                if (printEmail && !string.IsNullOrEmpty(Email))
                    ret.Add(Email);
            }
            return ret;
        }

        /// <summary>Maximal length of any line in the specified list.</summary>
        protected static int MaxLength(List<string> lines)
        {
            int ret = 0;
            if (lines != null)
                for (int i = 0; i < lines.Count; ++i)
                {
                    if (lines[i] != null)
                        if (lines[i].Length > ret)
                            ret = lines[i].Length;
                }
            return ret;
        }

        /// <summary>Returns a string containing the specified number of specified (equal) characters.</summary>
        /// <param name="ch">Character repeated in the string.</param>
        /// <param name="num">Number of characters in the string.</param>
        protected static string MultiCharacter(char ch, int num)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ch, num);
            return sb.ToString();
        }



        /// <summary>Returns a string containing the specified text lines in a single frame
        /// made of asterisks.</summary>
        /// <param name="lines">Lines to be embedded in the frame.</param>
        /// <param name="indent">Indentation of the frame from the left-hand side.</param>
        /// <param name="padLeft">Left padding (number of spaces inside the frame).</param>
        /// <param name="padRight">Right padding.</param>
        /// <param name="padTop">Top padding.</param>
        /// <param name="padBottom">Bottom padding.</param>
        public static string DecorationFrameAsterisk(List<string> lines, int indent, int padLeft, int padRight,
            int padTop, int padBottom)
        {
            return DecorationFrame(lines, indent, padLeft, padRight,
                padTop, padBottom, '*', 1);
        }

        /// <summary>Returns a string containing the specified text lines in a double frame
        /// made of asterisks.</summary>
        /// <param name="lines">Lines to be embedded in the frame.</param>
        /// <param name="indent">Indentation of the frame from the left-hand side.</param>
        /// <param name="padLeft">Left padding (number of spaces inside the frame).</param>
        /// <param name="padRight">Right padding.</param>
        /// <param name="padTop">Top padding.</param>
        /// <param name="padBottom">Bottom padding.</param>
        public static string DecorationFrameDoubleAsterisk(List<string> lines, int indent, int padLeft, int padRight,
            int padTop, int padBottom)
        {
            return DecorationFrame(lines, indent, padLeft, padRight,
                padTop, padBottom, '*', 2);
        }


        /// <summary>Returns a string containing the specified text lines in a frame
        /// made of one or more layers of equal characters.</summary>
        /// <param name="lines">Lines to be embedded in the frame.</param>
        /// <param name="indent">Indentation of the frame from the left-hand side.</param>
        /// <param name="padLeft">Left padding (number of spaces inside the frame).</param>
        /// <param name="padRight">Right padding.</param>
        /// <param name="padTop">Top padding.</param>
        /// <param name="padBottom">Bottom padding.</param>
        /// <param name="thickness">Thickness of the frame in characters.</param>
        /// <param name="frameChar">Character that constitutes the frame.</param>
        public static string DecorationFrame(List<string> lines, int indent, int padLeft, int padRight,
            int padTop, int padBottom, char frameChar, int thickness)
        {
            int maxLineLength = MaxLength(lines); // maximal length of any line in the list
            int totalLineLength = 2 * thickness + padLeft + padRight + maxLineLength; // total length of frame line
            int lineLength;
            // Prepare some strings for filling out the frame:
            string strIndent = MultiCharacter(' ', indent);
            string strFrame = MultiCharacter(frameChar, thickness);
            string strFillFrame = MultiCharacter(frameChar, maxLineLength + padLeft + padRight);
            string strFillSpaces = MultiCharacter(' ', maxLineLength + padLeft + padRight);
            string strPadLeft = MultiCharacter(' ', padLeft);
            string strPadRight = MultiCharacter(' ', padRight);
            // "Draw" the frame:
            StringBuilder sb = new StringBuilder();
            // Top border of the frame:
            for (int i = 0; i < thickness; ++i)
            {
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillFrame); sb.AppendLine(strFrame);
            }
            // Top padding:
            for (int i = 0; i < padTop; ++i)
            {
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillSpaces); sb.AppendLine(strFrame);
            }
            // Print contents:
            for (int i = 0; i < lines.Count; ++i)
            {
                lineLength = 0;
                if (lines[i] != null)
                    lineLength = lines[i].Length;
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strPadLeft);
                if (lines[i]!=null)
                    sb.Append(lines[i]);
                sb.Append(' ',maxLineLength-lineLength); // fill spaces up to max. line length
                sb.Append(strPadRight);
                sb.AppendLine(strFrame);
            }
            // Bottom padding:
            for (int i = 0; i < padBottom; ++i)
            {
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillSpaces); sb.AppendLine(strFrame);
            }
            // Bottom border of the frame:
            for (int i = 0; i < thickness; ++i)
            {
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillFrame); sb.AppendLine(strFrame);
            }
            return sb.ToString();
        }


        /// <summary>Returns a string containing the specified text lines in a frame
        /// made of a single layers of dashes ('-') and vertical bars ('|').</summary>
        /// <param name="lines">Lines to be embedded in the frame.</param>
        /// <param name="indent">Indentation of the frame from the left-hand side.</param>
        /// <param name="padLeft">Left padding (number of spaces inside the frame).</param>
        /// <param name="padRight">Right padding.</param>
        /// <param name="padTop">Top padding.</param>
        /// <param name="padBottom">Bottom padding.</param>
        public static string DecorationFrameDashed(List<string> lines, int indent, int padLeft, int padRight,
            int padTop, int padBottom)
        {
            int thickness = 1;
            int maxLineLength = MaxLength(lines); // maximal length of any line in the list
            int totalLineLength = 2 * thickness + padLeft + padRight + maxLineLength; // total length of frame line
            int lineLength;
            // Prepare some strings for filling out the frame:
            string strIndent = MultiCharacter(' ', indent);
            string strFrame = MultiCharacter('|', thickness);
            string strFillFrame = MultiCharacter('-', maxLineLength + padLeft + padRight);
            string strFillSpaces = MultiCharacter(' ', maxLineLength + padLeft + padRight);
            string strPadLeft = MultiCharacter(' ', padLeft);
            string strPadRight = MultiCharacter(' ', padRight);
            // "Draw" the frame:
            StringBuilder sb = new StringBuilder();
            // Top border of the frame:
            sb.Append(strIndent); sb.Append("-"); sb.Append(strFillFrame); sb.AppendLine("-");
            // Top padding:
            for (int i = 0; i < padTop; ++i)
            {
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillSpaces); sb.AppendLine(strFrame);
            }
            // Print contents:
            for (int i = 0; i < lines.Count; ++i)
            {
                lineLength = 0;
                if (lines[i] != null)
                    lineLength = lines[i].Length;
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strPadLeft);
                if (lines[i] != null)
                    sb.Append(lines[i]);
                sb.Append(' ', maxLineLength - lineLength); // fill spaces up to max. line length
                sb.Append(strPadRight);
                sb.AppendLine(strFrame);
            }
            // Bottom padding:
            for (int i = 0; i < padBottom; ++i)
            {
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillSpaces); sb.AppendLine(strFrame);
            }
            // Bottom border of the frame:
            sb.Append(strIndent); sb.Append("-"); sb.Append(strFillFrame); sb.AppendLine("-");
            return sb.ToString();
        }


        /// <summary>Returns a string containing the specified text lines in a frame
        /// made of a double layers of dashes ('-') and vertical bars ('|').</summary>
        /// <param name="lines">Lines to be embedded in the frame.</param>
        /// <param name="indent">Indentation of the frame from the left-hand side.</param>
        /// <param name="padLeft">Left padding (number of spaces inside the frame).</param>
        /// <param name="padRight">Right padding.</param>
        /// <param name="padTop">Top padding.</param>
        /// <param name="padBottom">Bottom padding.</param>
        public static string DecorationFrameDoubleDashed(List<string> lines, int indent, int padLeft, int padRight,
            int padTop, int padBottom)
        {
            int thickness = 2;
            int maxLineLength = MaxLength(lines); // maximal length of any line in the list
            int totalLineLength = 2 * thickness + padLeft + padRight + maxLineLength; // total length of frame line
            int lineLength;
            // Prepare some strings for filling out the frame:
            string strIndent = MultiCharacter(' ', indent);
            string strFrame = MultiCharacter('|', thickness);
            string strFillFrame = MultiCharacter('-', maxLineLength + padLeft + padRight);
            string strFillSpaces = MultiCharacter(' ', maxLineLength + padLeft + padRight);
            string strPadLeft = MultiCharacter(' ', padLeft);
            string strPadRight = MultiCharacter(' ', padRight);
            // "Draw" the frame:
            StringBuilder sb = new StringBuilder();
            // Top border of the frame:
            sb.Append(strIndent); sb.Append("--"); sb.Append(strFillFrame); sb.AppendLine("--");
            sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillFrame); sb.AppendLine(strFrame);
            // Top padding:
            for (int i = 0; i < padTop; ++i)
            {
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillSpaces); sb.AppendLine(strFrame);
            }
            // Print contents:
            for (int i = 0; i < lines.Count; ++i)
            {
                lineLength = 0;
                if (lines[i] != null)
                    lineLength = lines[i].Length;
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strPadLeft);
                if (lines[i] != null)
                    sb.Append(lines[i]);
                sb.Append(' ', maxLineLength - lineLength); // fill spaces up to max. line length
                sb.Append(strPadRight);
                sb.AppendLine(strFrame);
            }
            // Bottom padding:
            for (int i = 0; i < padBottom; ++i)
            {
                sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillSpaces); sb.AppendLine(strFrame);
            }
            // Bottom border of the frame:
            sb.Append(strIndent); sb.Append(strFrame); sb.Append(strFillFrame); sb.AppendLine(strFrame);
            sb.Append(strIndent); sb.Append("--"); sb.Append(strFillFrame); sb.AppendLine("--");
            return sb.ToString();
        }


        /// <summary>Launches initialization notice.</summary>
        // TODO: supplement reporters to enble printing without decorarion, then
        // use this in the following function.
        public virtual void LaunchInitNotice()
        {
            int indent = 8;
            int padLeft = 2;
            int padRight = 2;
            int padTop = 1;
            int padBottom = 1;

            Console.WriteLine();
            Console.WriteLine(
                DecorationFrameDoubleAsterisk(Notice(),indent,padLeft,padRight,padTop,padBottom) );
            Console.WriteLine();
            // Reporter.ReportWarning(InitNotice());
        }

        #endregion Actios


        #region Reporting

        protected IReporter _reporter = DefaultReporter;

        /// <summary>
        /// 
        /// </summary>
        public IReporter Reporter
        {
            get { return _reporter; }
            set {
                lock (Lock)
                {
                    if (_reporter == null)
                    {
                        string msg = "Reporter not specified (null reference). ";
                        if (IsApplication)
                            msg += " Application: " + Name;
                        else
                            msg += "Module: " + Name;
                        throw new ArgumentException(msg);
                    }
                    _reporter = value;
                }
            }
        }


        protected static IReporter _defaultReporter = null;

        /// <summary>Returns the curent global Application reporter.</summary>
        public static IReporter DefaultReporter
        {
            get
            {
                if (_defaultReporter == null)
                    _defaultReporter = ReporterConsole.Global;
                return _defaultReporter;
            }
        }




        #endregion Reporting



        #region Static.Utilities

        #region SpecificApplications.Ann.DynamicDll


        private static int
            AnnDD = 15,
            AnnMM = 8,
            AnnYY = 2014;

        private static DateTime ? _annDllLimit =  null;


        private static DateTime AnnDllTestLimit
        {
            get {
                lock(Util.LockGlobal)
                {
                    if (_annDllLimit == null)
                    {
                        _annDllLimit = new DateTime(AnnYY, AnnMM, AnnDD, 22, 00, 00);
                    }
                    return _annDllLimit.Value;
                }
            }
        }

        private static bool ? _isAnnDllTestMode = null; 

        /// <summary>Whether dynamic testing of software is on (helps in agile software development).</summary>
        private static bool IsAnnDllTestMode
        {
            get {
                lock(Util.LockGlobal)
                {
                    if (_isAnnDllTestMode == null)
                        _isAnnDllTestMode = (
                            UtilSystem.IsUserIgor || UtilSystem.IsUserTadej
                            || DateTime.Now < AnnDllTestLimit);
                }
                return _isAnnDllTestMode.Value;
            }
        }

        /// <summary>Appends list of assemblies for dynamic testing of software.</summary>
        private static void AnnAdd()
        {
            if (IsAnnDllTestMode)
            {
                ScriptLoaderBase.AddDefaultAssemblies(
                    "IGLibExt.dll",
                    "IGLibShellExt.dll",
                    "IGLibNeural.dll",
                    "IGLibNeuralExt.dll"
                    );
            }
        }

        /// <summary>Appends list of assemblies for dynamic testing of software in the 
        /// ExtShellApp project.</summary>
        public static void AddDefaultAssembliesExtShellApp()
        {
            ScriptLoaderBase.AddDefaultAssemblies(
                
                "IGLibPlot2d.dll",
                "IGLibPlot3d.dll",
                "IGLibForms.dll",
                //"IGLibReporterMsg.dll",
                //"IGLibOpt.dll",
                "IGLibNeuralExtForms.dll",
                "System.Windows.Forms.dll",
                "System.Drawing.dll",
                "Kitware.VTK.dll",
                "ZedGraph.dll"
                );
            AnnAdd();
        }
        
        // Adding default assemblies for specific applicaton contexts:

        /// <summary>Appends list of assemblies for dynamic testing of software in the 
        /// ExtNeuralApp project.</summary>
        public static void AddDefaultAssembliesExtNeuralApp()
        {
            AddDefaultAssembliesExtShellApp();
        }


        /// <summary>Appends list of assemblies for dynamic testing of software in the 
        /// ExtShellAppExt project.</summary>
        public static void AddDefaultAssembliesExtShellAppExt()
        {
            AddDefaultAssembliesExtShellApp();
        }

        #endregion SpecificApplications.Ann.DynamicDll


        #region SpecificApplications.Ann.Loader

        private static int _numLoadableScriptWarnings = 0;
        private const int _maxLoadableScriptWarnings = 1;

        /// <summary>Basic procedure for calculation of the flag <see cref="LoadableScriptShellIsLoadable"/>.</summary>
        private static bool LoadableScriptShellIsLoadableDefault
        {
            get
            {
                bool ret = false;
                if (UtilSystem.IsUserIgor || UtilSystem.IsUserTadej)
                {
                    ret = true;
                    if (ret==true && (UtilSystem.IsUserIgor || UtilSystem.IsUserTadej))
                    {
                        if (_numLoadableScriptWarnings < _maxLoadableScriptWarnings)
                        {
                            Console.WriteLine(Environment.NewLine + Environment.NewLine
                                + "WARNING: " + Environment.NewLine
                                + "Run in test mode. Loader may behave differently otherwise."
                                + Environment.NewLine + Environment.NewLine);
                            ++_numLoadableScriptWarnings;
                        }
                    }
                }
                return ret;
            }
        }


        /// <summary>Basic procedure for calculation of the flag <see cref="LoadableScriptShellIsRunnable"/>.</summary>
        private static bool LoadableScriptShellIsRunnableDefault
        {
            get
            {
                return LoadableScriptShellIsLoadableDefault;
            }
        }



        /// <summary>Whether shell scripts are dynamically loadable in the current application.</summary>
        protected internal virtual bool LoadableScriptShellIsLoadable
        {
            get
            {
                bool ret = false;
                ret = LoadableScriptShellIsLoadableDefault;
                return ret;
            }
        }

        /// <summary>Whether dynamically loaded shell scripts are runnable in the current application.</summary>
        protected internal virtual bool LoadableScriptShellIsRunnable
        {
            get
            {
                return LoadableScriptShellIsRunnableDefault;
            }
        }


        /// <summary>Whether shell scripts are dynamically loadable.</summary>
        protected internal static bool LoadableScriptShellIsLoadableS
        {
            get
            {
                if (ApplicationBase.Global != null)
                    return ApplicationBase.Global.LoadableScriptShellIsLoadable;
                else
                    return LoadableScriptShellIsLoadableDefault;
            }
        }

        /// <summary>Whether dynamically loaded shell scripts are runnable.</summary>
        protected internal static bool LoadableScriptShellIsRunnableS
        {
            get 
            {
                if (ApplicationBase.Global != null)
                    return ApplicationBase.Global.LoadableScriptShellIsRunnable;
                else
                    return LoadableScriptShellIsRunnableDefault;
            }
        }

        #endregion SpecificApplications.Ann.Loader



        #endregion Static.Utilities


    }  // class ModuleBase



}  // namespace IG.Lib
