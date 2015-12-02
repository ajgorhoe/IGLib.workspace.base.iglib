// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Forms
{

    /// <summary>Test application for forms.
    /// Usage: AppTestForms.Init().</summary>
    public class AppTestForms : App
    {

        protected AppTestForms()
            : this("IGLib Forms Test Program", 0, 0)
        {
        }

        protected AppTestForms(string name, int version, int subversion) :
                base(name, version, subversion)
        {  }

        protected override void BeforeInitialization()
        {
            base.BeforeInitialization();
            // Basic data:
            Expires = false;

            Name = "IGLib Forms Test Program";
            Version = 1;
            SubVersion = 2;
            Release = "debug";
            Expires = false;
            base.BeforeInitialization();
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
            
            // Reporter:
            ReporterForms rep = ReporterForms.Global;
            rep.UseConsole = true;
            Reporter = rep;
        }

        protected override void AfterInitialization()
        {
            base.AfterInitialization();
        }


        /// <summary>Launches initialization notice.</summary>
        public override void LaunchInitNotice()
        {
            int indent = 8;
            int padLeft = 2;
            int padRight = 2;
            int padTop = 1;
            int padBottom = 1;

            Console.WriteLine();
            Console.WriteLine(
                DecorationFrameDoubleDashed(Notice(), indent, padLeft, padRight, padTop, padBottom));
            Console.WriteLine();
            // Reporter.ReportWarning(InitNotice());
        }



        #region global


        /// <summary>Initializes global application data for the current class of application.</summary>
        protected static void Init()
        {
            lock (lockGlobal)
            {
                if (!InitializedGlobal)
                {
                    App.InitApp();
                    Global = new AppTestForms();
                    Global.LaunchInitNotice();
                }
            }
        }

        #endregion


    }  // class AppTestForms

}
