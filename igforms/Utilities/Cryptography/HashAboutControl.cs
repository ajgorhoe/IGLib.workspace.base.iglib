// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;


using IG.Lib;
using IG.Forms;

namespace IG.Forms
{
    public partial class HashAboutControl : UserControl
    {
        public HashAboutControl()
        {
            InitializeComponent();
            this.lblAboutWeb.Text = DefaultHomePageShort;
        }

        

        #region Operation

        const string DefaultHomePageShort = "www2.arnes.si/~ljc3m2/igor/software/IGLibShellApp/HashForm.html#hashform";

        const string DefaultDocumentLocation = "http://www2.arnes.si/~ljc3m2/igor/software/IGLibShellApp/versions/1.6.1/doc/";

        const string DefaultHelpFileNameWithoutExtension = "HashForm_help";

        const string DefaultHelpFilenamePdf = DefaultHelpFileNameWithoutExtension + ".pdf";

        const string DefaultHelpFilenameHtml = DefaultHelpFileNameWithoutExtension + ".html";

        const string DefaultHelpLocation = DefaultDocumentLocation + DefaultHelpFilenameHtml;

        const string DefaultHelpLocationPdf = DefaultDocumentLocation + DefaultHelpFilenamePdf;



        protected string _homePageShort;

        /// <summary>Short version of the application's home page.</summary>
        public virtual string HomePageShort
        {
            get
            {
                if (_homePageShort == null)
                    _homePageShort = DefaultHomePageShort;
                return _homePageShort;
            }
            protected set { 
                _homePageShort = value;
                this.lblAboutWeb.Text = DefaultHomePageShort;
            }
        }

        public virtual string HomePage
        {
            get { return "http://" + HomePageShort; }
        }

        protected string _helpLocation;

        /// <summary>Location of the help file that can be displayed in a web browser.</summary>
        public virtual string HelpLocation
        {
            get
            {
                if (string.IsNullOrEmpty(_helpLocation))
                {
                    _helpLocation = DefaultHelpLocation;
                    string trialLocation = GetHelpLocationHtmlInExecutableDirectory();
                    if (!string.IsNullOrEmpty(trialLocation))
                        if (File.Exists(trialLocation))
                        {
                            _helpLocation = trialLocation;
                        }
                }
                return _helpLocation;
            }
        }



        protected string _helpLocationPdf;

        /// <summary>Location of the help file that can be displayed in a web browser.</summary>
        public virtual string HelpLocationPdf
        {
            get
            {
                if (string.IsNullOrEmpty(_helpLocationPdf))
                {
                    _helpLocationPdf = DefaultHelpLocationPdf;
                    string trialLocation = GetHelpLocationPdfInExecutableDirectory();
                    if (!string.IsNullOrEmpty(trialLocation))
                        if (File.Exists(trialLocation))
                        {
                            _helpLocationPdf = trialLocation;
                        }
                }
                return _helpLocationPdf;
            }
        }

        public virtual string GetHelpLocationHtmlInExecutableDirectory()
        {
            string location = null;
            string executablePath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(executablePath))
            {
                string testLocation = Path.Combine(executablePath, DefaultHelpFilenameHtml);
                if (File.Exists(testLocation))
                    location = testLocation;
            }
            return location;
        }

        public virtual string GetHelpLocationPdfInExecutableDirectory()
        {
            string location = null;
            string executablePath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(executablePath))
            {
                string testLocation = Path.Combine(executablePath, DefaultHelpFilenamePdf);
                if (File.Exists(testLocation))
                    location = testLocation;
            }
            return location;
        }

        protected BrowserSimpleForm _browser;

        protected BrowserSimpleForm Browser
        {
            get
            {
                bool openNew = false;
                if (_browser == null)
                    openNew = true;
                else if (_browser.IsDisposed)
                    openNew = true;
                if (openNew)
                    _browser = new BrowserSimpleForm();
                return _browser;
            }
        }

        public string DefaultApplicationName = "HashForm";

        protected string _applicationName;

        public string ApplicationName
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationName))
                {
                    _applicationName = DefaultApplicationName;
                }
                return _applicationName;
            }
        }

        protected int _fadingMessageShowTimeMs = 1000;

        public void HelpBrowse()
        {
            FadingMessage msg = new FadingMessage("Help", "Opening help in browser...", _fadingMessageShowTimeMs);
            msg.LaunchedInParallelThread = false;
            msg.LaunchedAtMouseCursor = true;
            System.Diagnostics.Process.Start(HelpLocation);
        }

        public void HelpPdf()
        {
            FadingMessage msg = new FadingMessage("Help", "Opening help in PDF...", _fadingMessageShowTimeMs);
            msg.LaunchedInParallelThread = false;
            msg.LaunchedAtMouseCursor = true;
            System.Diagnostics.Process.Start(HelpLocationPdf);
        }

        public void WebPageBrowse()
        {
            FadingMessage msg = new FadingMessage("Software web page", "Opening software web page in browser...", _fadingMessageShowTimeMs);
            msg.LaunchedInParallelThread = false;
            msg.LaunchedAtMouseCursor = true;
            System.Diagnostics.Process.Start(HomePage);
        }

        public void HelpShow()
        {
            
            try
            {
                Browser.OpenLocation(HelpLocation);
                Browser.ShowDialog();
            }
            catch (Exception)
            {
            }

            //            // Open software web page:
            //            System.Diagnostics.Process.Start(lblAboutWeb.Text);

            //            //Creating a fading message in this thread, which must be canceled explicitly (e.g. by pressing mouse button 3:)
            //            string msgtitle = "Info:";
            //            string msgtext = 
            //@"Sorry, help is not yet available.";
            //            int showTime = 4000;
            //            FadingMessage fm = new FadingMessage(msgtitle, msgtext, showTime);
        }

        public void WebPageShow()
        {
            try
            {
                Browser.OpenLocation(HomePage);
                Browser.ShowDialog();
            }
            catch (Exception)
            {
            }
        }


        #endregion Operation



        private void lblAboutWeb_Click(object sender, EventArgs e)
        {
            // Open software web page:
            System.Diagnostics.Process.Start(lblAboutWeb.Text);
            // BrowserForm.BrowserMain();
        }

        private void btnHelpPdf_Click(object sender, EventArgs e)
        {
            HelpPdf();
        }

        private void btnHelpHtml_Click(object sender, EventArgs e)
        {
            HelpShow();
        }

        private void btnWeb_Click(object sender, EventArgs e)
        {
            WebPageShow();
        }

        /// <summary>Triggered on each repaint of the panel. can be attemted to make background transparent.</summary>
        /// <remarks>Does not work. Color.FromArgb(25, Color.White); gets the closest to doing something.</remarks>
        private void pnlAbout_Paint(object sender, PaintEventArgs e)
        {
            //Console.WriteLine(Environment.NewLine + Environment.NewLine + "PNL REDRAW, BACK COLOR: Transparent" + Environment.NewLine + Environment.NewLine);
            //// ((Control)sender).BackColor = Color.Transparent;
            //// this.pnlAbout.BackColor = Color.Transparent;
            // The lline below dows something, but does not cause transparency:
            // this.pnlAbout.BackColor = Color.FromArgb(25, Color.White);
        }

        private void lblBrowseHelp_Click(object sender, EventArgs e)
        {
            HelpBrowse();
        }

        private void lblHelpPdf_Click(object sender, EventArgs e)
        {
            HelpPdf();
        }

        private void lblBrowseHome_Click(object sender, EventArgs e)
        {
            WebPageBrowse();
        }

    }
}
