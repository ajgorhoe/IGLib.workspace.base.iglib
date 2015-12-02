// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;



namespace IG.Forms
{

    [Obsolete("Out of use, replaced by BrowserSimpleWindow.")]
    public class BrowserFormOld : Form
    {

        public BrowserFormOld()
        {
            InitializeComponent();
        }
        

        void ReportError(Exception ex)
        {
            UtilForms.ReportError(ex);
        }




        private void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.Text = webBrowser.DocumentTitle.ToString();
        }

        private void AddressBar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                webBrowser.Navigate(txtAddressBar.Text);
            }
        }

        private void webBrowser1_Navigated(object sender,
           WebBrowserNavigatedEventArgs e)
        {
            txtAddressBar.Text = webBrowser.Url.ToString();
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            buttonBack.Enabled = false;
            buttonForward.Enabled = false;
            buttonStop.Enabled = false;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            webBrowser.GoBack();
            txtAddressBar.Text = webBrowser.Url.ToString();
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {
            webBrowser.GoForward();
            txtAddressBar.Text = webBrowser.Url.ToString();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            webBrowser.Stop();
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            try
            {
                webBrowser.GoHome();
                txtAddressBar.Text = webBrowser.Url.ToString();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            webBrowser.Refresh();
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            webBrowser.Navigate(txtAddressBar.Text);
        }

        private void webBrowser1_CanGoBackChanged(object sender, EventArgs e)
        {
            if (webBrowser.CanGoBack == true)
            {
                buttonBack.Enabled = true;
            }
            else
            {
                buttonBack.Enabled = false;
            }
        }

        private void webBrowser1_CanGoForwardChanged(object sender, EventArgs e)
        {
            if (webBrowser.CanGoForward == true)
            {
                buttonForward.Enabled = true;
            }
            else
            {
                buttonForward.Enabled = false;
            }
        }

        private void webBrowser1_Navigating(object sender,
           WebBrowserNavigatingEventArgs e)
        {
            buttonStop.Enabled = true;
        }

        private void webBrowser1_DocumentCompleted(object sender,
           WebBrowserDocumentCompletedEventArgs e)
        {
            buttonStop.Enabled = false;
            txtAddressBar.Text = webBrowser.Url.ToString();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserFormOld));
            this.txtAddressBar = new System.Windows.Forms.TextBox();
            this.buttonSubmit = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonForward = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFile_OpenURL = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFile_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFile_Close = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFile_CloseDocument = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTools_Controls = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuTools_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuGelp = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelp_About = new System.Windows.Forms.ToolStripMenuItem();
            this.ControlPnl = new System.Windows.Forms.Panel();
            this.TopPnlHideBtn = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusType = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusError = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.MenuStrip.SuspendLayout();
            this.ControlPnl.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddressBar
            // 
            this.txtAddressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAddressBar.Location = new System.Drawing.Point(12, 33);
            this.txtAddressBar.Name = "AddressBar";
            this.txtAddressBar.Size = new System.Drawing.Size(608, 20);
            this.txtAddressBar.TabIndex = 1;
            this.txtAddressBar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AddressBar_KeyPress);
            // 
            // buttonSubmit
            // 
            this.buttonSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSubmit.Location = new System.Drawing.Point(628, 28);
            this.buttonSubmit.Name = "buttonSubmit";
            this.buttonSubmit.Size = new System.Drawing.Size(54, 29);
            this.buttonSubmit.TabIndex = 2;
            this.buttonSubmit.Text = "Submit";
            this.buttonSubmit.UseVisualStyleBackColor = true;
            this.buttonSubmit.Click += new System.EventHandler(this.buttonSubmit_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.ForeColor = System.Drawing.Color.Purple;
            this.buttonRefresh.Location = new System.Drawing.Point(224, 4);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 3;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.ForeColor = System.Drawing.Color.Purple;
            this.buttonHome.Location = new System.Drawing.Point(163, 4);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(55, 23);
            this.buttonHome.TabIndex = 4;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonStop.ForeColor = System.Drawing.Color.Red;
            this.buttonStop.Location = new System.Drawing.Point(108, 4);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(35, 23);
            this.buttonStop.TabIndex = 5;
            this.buttonStop.Text = "X";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonForward
            // 
            this.buttonForward.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonForward.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.buttonForward.Location = new System.Drawing.Point(60, 3);
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.Size = new System.Drawing.Size(42, 23);
            this.buttonForward.TabIndex = 6;
            this.buttonForward.Text = "=>";
            this.buttonForward.UseVisualStyleBackColor = true;
            this.buttonForward.Click += new System.EventHandler(this.buttonForward_Click);
            // 
            // buttonBack
            // 
            this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.buttonBack.Location = new System.Drawing.Point(12, 4);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(42, 23);
            this.buttonBack.TabIndex = 7;
            this.buttonBack.Text = "<=";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuTools,
            this.MenuGelp});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(686, 24);
            this.MenuStrip.TabIndex = 8;
            this.MenuStrip.Text = "menuStrip1";
            this.MenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuStrip_ItemClicked);
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile_OpenURL,
            this.MenuFile_Open,
            this.printPreviewToolStripMenuItem,
            this.printToolStripMenuItem,
            this.MenuFile_Close,
            this.MenuFile_CloseDocument});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "&File";
            this.MenuFile.ToolTipText = "Closes the current document, but not the form.";
            // 
            // MenuFile_OpenURL
            // 
            this.MenuFile_OpenURL.Name = "MenuFile_OpenURL";
            this.MenuFile_OpenURL.Size = new System.Drawing.Size(167, 22);
            this.MenuFile_OpenURL.Text = "Open &URL";
            this.MenuFile_OpenURL.ToolTipText = "Loads an XML document from a file.";
            // 
            // MenuFile_Open
            // 
            this.MenuFile_Open.Name = "MenuFile_Open";
            this.MenuFile_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuFile_Open.Size = new System.Drawing.Size(167, 22);
            this.MenuFile_Open.Text = "Open File";
            this.MenuFile_Open.ToolTipText = "Loads an XML document from a file.";
            // 
            // printPreviewToolStripMenuItem
            // 
            this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
            this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.printPreviewToolStripMenuItem.Text = "Print Pre&view";
            // 
            // printToolStripMenuItem
            // 
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.printToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.printToolStripMenuItem.Text = "&Print";
            // 
            // MenuFile_Close
            // 
            this.MenuFile_Close.Name = "MenuFile_Close";
            this.MenuFile_Close.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.MenuFile_Close.Size = new System.Drawing.Size(167, 22);
            this.MenuFile_Close.Text = "&Close";
            this.MenuFile_Close.ToolTipText = "Closes the XML Viewer.";
            // 
            // MenuFile_CloseDocument
            // 
            this.MenuFile_CloseDocument.Name = "MenuFile_CloseDocument";
            this.MenuFile_CloseDocument.Size = new System.Drawing.Size(167, 22);
            this.MenuFile_CloseDocument.Text = "Close Document";
            // 
            // MenuTools
            // 
            this.MenuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuTools_Controls,
            this.toolStripSeparator5,
            this.toolStripSeparator6,
            this.MenuTools_Stop});
            this.MenuTools.Name = "MenuTools";
            this.MenuTools.Size = new System.Drawing.Size(48, 20);
            this.MenuTools.Text = "T&ools";
            // 
            // MenuTools_Controls
            // 
            this.MenuTools_Controls.Checked = true;
            this.MenuTools_Controls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuTools_Controls.Name = "MenuTools_Controls";
            this.MenuTools_Controls.Size = new System.Drawing.Size(170, 22);
            this.MenuTools_Controls.Text = "Top &Control Panel";
            this.MenuTools_Controls.ToolTipText = resources.GetString("MenuTools_Controls.ToolTipText");
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(167, 6);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(167, 6);
            // 
            // MenuTools_Stop
            // 
            this.MenuTools_Stop.Name = "MenuTools_Stop";
            this.MenuTools_Stop.Size = new System.Drawing.Size(170, 22);
            this.MenuTools_Stop.Text = "Stop";
            // 
            // MenuGelp
            // 
            this.MenuGelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuHelp_About});
            this.MenuGelp.Name = "MenuGelp";
            this.MenuGelp.Size = new System.Drawing.Size(44, 20);
            this.MenuGelp.Text = "&Help";
            // 
            // MenuHelp_About
            // 
            this.MenuHelp_About.Name = "MenuHelp_About";
            this.MenuHelp_About.Size = new System.Drawing.Size(170, 22);
            this.MenuHelp_About.Text = "About this control";
            // 
            // ControlPnl
            // 
            this.ControlPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ControlPnl.Controls.Add(this.buttonBack);
            this.ControlPnl.Controls.Add(this.buttonForward);
            this.ControlPnl.Controls.Add(this.TopPnlHideBtn);
            this.ControlPnl.Controls.Add(this.buttonSubmit);
            this.ControlPnl.Controls.Add(this.txtAddressBar);
            this.ControlPnl.Controls.Add(this.buttonRefresh);
            this.ControlPnl.Controls.Add(this.buttonStop);
            this.ControlPnl.Controls.Add(this.buttonHome);
            this.ControlPnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.ControlPnl.Location = new System.Drawing.Point(0, 24);
            this.ControlPnl.Name = "ControlPnl";
            this.ControlPnl.Size = new System.Drawing.Size(686, 62);
            this.ControlPnl.TabIndex = 9;
            // 
            // TopPnlHideBtn
            // 
            this.TopPnlHideBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TopPnlHideBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TopPnlHideBtn.ForeColor = System.Drawing.Color.Crimson;
            this.TopPnlHideBtn.Location = new System.Drawing.Point(659, 4);
            this.TopPnlHideBtn.Name = "TopPnlHideBtn";
            this.TopPnlHideBtn.Size = new System.Drawing.Size(23, 23);
            this.TopPnlHideBtn.TabIndex = 0;
            this.TopPnlHideBtn.Text = "x";
            this.TopPnlHideBtn.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusType,
            this.StatusPath,
            this.StatusError,
            this.StatusStatus});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 757);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(686, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "<<< Initialized >>>";
            // 
            // StatusType
            // 
            this.StatusType.Name = "StatusType";
            this.StatusType.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusType.Size = new System.Drawing.Size(79, 17);
            this.StatusType.Text = "<< Type >>";
            this.StatusType.ToolTipText = "This shows the type of the selected XML node in the tree view.";
            // 
            // StatusPath
            // 
            this.StatusPath.Name = "StatusPath";
            this.StatusPath.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusPath.Size = new System.Drawing.Size(77, 17);
            this.StatusPath.Text = "<< Path >>";
            this.StatusPath.ToolTipText = "This shows the path of the last selected XML node in the tree view.";
            // 
            // StatusError
            // 
            this.StatusError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.StatusError.Name = "StatusError";
            this.StatusError.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusError.Size = new System.Drawing.Size(34, 17);
            this.StatusError.Text = " OK";
            this.StatusError.ToolTipText = "This shows eventual error and warning messages.";
            // 
            // StatusStatus
            // 
            this.StatusStatus.ForeColor = System.Drawing.Color.Blue;
            this.StatusStatus.Name = "StatusStatus";
            this.StatusStatus.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusStatus.Size = new System.Drawing.Size(68, 17);
            this.StatusStatus.Text = "Initialized.";
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Location = new System.Drawing.Point(40, 423);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(297, 293);
            this.tabControl.TabIndex = 4;
            this.tabControl.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(289, 267);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // webBrowser1
            // 
            this.webBrowser.Location = new System.Drawing.Point(334, 102);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser1";
            this.webBrowser.Size = new System.Drawing.Size(277, 315);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 779);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ControlPnl);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowserForm";
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ControlPnl.ResumeLayout(false);
            this.ControlPnl.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.TextBox txtAddressBar;
        private System.Windows.Forms.Button buttonSubmit;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonForward;
        private MenuStrip MenuStrip;
        private ToolStripMenuItem MenuFile;
        private ToolStripMenuItem MenuFile_Open;
        private ToolStripMenuItem MenuFile_Close;
        private ToolStripMenuItem MenuFile_CloseDocument;
        private ToolStripMenuItem MenuTools;
        private ToolStripMenuItem MenuTools_Controls;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem MenuTools_Stop;
        private ToolStripMenuItem MenuGelp;
        private ToolStripMenuItem MenuHelp_About;
        private Panel ControlPnl;
        private Button TopPnlHideBtn;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel StatusType;
        private ToolStripStatusLabel StatusPath;
        private ToolStripStatusLabel StatusError;
        private ToolStripStatusLabel StatusStatus;
        private ToolStripMenuItem MenuFile_OpenURL;
        private ToolStripMenuItem printPreviewToolStripMenuItem;
        private ToolStripMenuItem printToolStripMenuItem;
        private TabControl tabControl;
        private TabPage tabPage1;
        private WebBrowser webBrowser;
        private System.Windows.Forms.Button buttonBack;

        [STAThread]
        static public void BrowserMain()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BrowserFormOld());
        }

        private void MenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }



        public static void Example()
        {
            BrowserFormOld.BrowserMain();
        }


    }  // class BrowserForm


} // namespace IG.Forms