using System;

using IG.Lib;

namespace IG.App
{


    /// <summary>Application class for the IGLib's shell application.</summary>
    public class ApplicationIgShellBase : IG.Lib.ApplicationBase
    {

        public ApplicationIgShellBase()
            : base("IGLib Shell", 1 /* version */, 6 /* subversion */, "Demo" /* release */)
        {
           
        }

        

        /// <summary>Entry method of the application.</summary>
        /// <param name="args">List of arguments of the entry method.</param>
        public override void Run(string[] args)
        {

            int numArgs = 0;
            if (args != null)
            {
                numArgs = args.Length;
            }
            if (numArgs == 0)
            {
                Console.WriteLine(Environment.NewLine
                    + "No arguments were specified." + Environment.NewLine + Environment.NewLine
                    + "Run the application by typing: " + Environment.NewLine
                    + "  " + UtilSystem.GetCurrentProcessExecutableName() + " command <arg1> <arg2> ..." + Environment.NewLine
                    + "or" + Environment.NewLine
                    + "  " + UtilSystem.GetCurrentProcessExecutableName() + " ?" + Environment.NewLine
                    + "for list of commands or " + Environment.NewLine
                    + "  " + UtilSystem.GetCurrentProcessExecutableName() + " Interactive" + Environment.NewLine
                    + "for interactive mode or " + Environment.NewLine
                    + "  " + UtilSystem.GetCurrentProcessExecutableName() + " Run fileName" + Environment.NewLine
                    + "for running the specified command file."
                    + Environment.NewLine + Environment.NewLine
                    + "See the *.cmd files for example commmands." + Environment.NewLine);
                return;
            }
            try
            {
                //// Enable visuall styles - e.g. such things that Crtl-A means "Select all".
                //// $$$$$$$ Warning: line below was commented!
                //System.Windows.Forms.Application.EnableVisualStyles();
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + "Exception was thrown: " + Environment.NewLine
                    + "  " + ex.Message + Environment.NewLine);
            }



            IG.Script.AppBase application = new IG.Script.AppBase();


            //IgShellApp.Init();

            //application.Script_Run(AppArguments);
            application.Run(args);

        }


        protected override void ModuleInitializationBefore()
        {
            base.ModuleInitializationBefore();
            Expires = false;
            AuthorFirstName = "Igor Grešovnik";
            AuthorAddress1 = "Črneče 147";
            AuthorAddress2 = "2370 Dravograd";
            AuthorAddress3 = "Slovenia";
        }

        protected override void ModuleInitializationAfter()
        {
            base.ModuleInitializationAfter();
            LaunchInitNotice();
            // AddModule(ModuleTest.Get());
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

            //Console.WriteLine(
            //    DecorationFrameDoubleDashed(
            //    Notice(), indent, padLeft, padRight, padTop, padBottom)
            //    );
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
                    IG.Lib.ApplicationBase.InitApp();
                    Global = new ApplicationIgShellBase();  // IgShellApp();
                }
            }
        }

        #endregion

    }  // class ExtShellApp

}
