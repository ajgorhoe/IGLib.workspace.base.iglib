// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;


namespace IG.Forms
{
    public partial class XMLTestPathForm : Form
    {

        internal XmlDocument Doc = new XmlDocument();  // the document opened for examination
        internal string InitialPath = null;

        public delegate void MyDelegate();
        public MyDelegate CloseDelegate = null;
        private GroupBox groupBoxButtons;

        


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XMLTestPathForm));
            this.gbxExpression = new System.Windows.Forms.GroupBox();
            this.optValueOnly = new System.Windows.Forms.RadioButton();
            this.optOuterXml = new System.Windows.Forms.RadioButton();
            this.optInnerXml = new System.Windows.Forms.RadioButton();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtTestExpression = new System.Windows.Forms.TextBox();
            this.gbxResults = new System.Windows.Forms.GroupBox();
            this.rtbResults = new System.Windows.Forms.RichTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBoxButtons = new System.Windows.Forms.GroupBox();
            this.gbxExpression.SuspendLayout();
            this.gbxResults.SuspendLayout();
            this.groupBoxButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxExpression
            // 
            this.gbxExpression.Controls.Add(this.optValueOnly);
            this.gbxExpression.Controls.Add(this.optOuterXml);
            this.gbxExpression.Controls.Add(this.optInnerXml);
            this.gbxExpression.Controls.Add(this.btnTest);
            this.gbxExpression.Controls.Add(this.txtTestExpression);
            this.gbxExpression.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbxExpression.Location = new System.Drawing.Point(0, 0);
            this.gbxExpression.Name = "gbxExpression";
            this.gbxExpression.Padding = new System.Windows.Forms.Padding(6, 10, 6, 6);
            this.gbxExpression.Size = new System.Drawing.Size(467, 76);
            this.gbxExpression.TabIndex = 0;
            this.gbxExpression.TabStop = false;
            this.gbxExpression.Text = "Expression";
            // 
            // optValueOnly
            // 
            this.optValueOnly.AutoSize = true;
            this.optValueOnly.Location = new System.Drawing.Point(272, 50);
            this.optValueOnly.Name = "optValueOnly";
            this.optValueOnly.Size = new System.Drawing.Size(76, 17);
            this.optValueOnly.TabIndex = 4;
            this.optValueOnly.Text = "Value Only";
            this.optValueOnly.UseVisualStyleBackColor = true;
            // 
            // optOuterXml
            // 
            this.optOuterXml.AutoSize = true;
            this.optOuterXml.Location = new System.Drawing.Point(190, 50);
            this.optOuterXml.Name = "optOuterXml";
            this.optOuterXml.Size = new System.Drawing.Size(76, 17);
            this.optOuterXml.TabIndex = 3;
            this.optOuterXml.Text = "Outer XML";
            this.optOuterXml.UseVisualStyleBackColor = true;
            // 
            // optInnerXml
            // 
            this.optInnerXml.AutoSize = true;
            this.optInnerXml.Checked = true;
            this.optInnerXml.Location = new System.Drawing.Point(109, 50);
            this.optInnerXml.Name = "optInnerXml";
            this.optInnerXml.Size = new System.Drawing.Size(74, 17);
            this.optInnerXml.TabIndex = 2;
            this.optInnerXml.TabStop = true;
            this.optInnerXml.Text = "Inner XML";
            this.optInnerXml.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(6, 47);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "&Test XPath";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // txtTestExpression
            // 
            this.txtTestExpression.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtTestExpression.Location = new System.Drawing.Point(6, 23);
            this.txtTestExpression.Name = "txtTestExpression";
            this.txtTestExpression.Size = new System.Drawing.Size(455, 20);
            this.txtTestExpression.TabIndex = 0;
            // 
            // gbxResults
            // 
            this.gbxResults.Controls.Add(this.rtbResults);
            this.gbxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxResults.Location = new System.Drawing.Point(0, 76);
            this.gbxResults.Name = "gbxResults";
            this.gbxResults.Padding = new System.Windows.Forms.Padding(10);
            this.gbxResults.Size = new System.Drawing.Size(467, 305);
            this.gbxResults.TabIndex = 1;
            this.gbxResults.TabStop = false;
            this.gbxResults.Text = "Results";
            // 
            // rtbResults
            // 
            this.rtbResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbResults.Location = new System.Drawing.Point(10, 23);
            this.rtbResults.Name = "rtbResults";
            this.rtbResults.ShortcutsEnabled = false;
            this.rtbResults.Size = new System.Drawing.Size(447, 272);
            this.rtbResults.TabIndex = 0;
            this.rtbResults.Text = "";
            this.rtbResults.UseWaitCursor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(394, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(67, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBoxButtons
            // 
            this.groupBoxButtons.Controls.Add(this.btnClose);
            this.groupBoxButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxButtons.Location = new System.Drawing.Point(0, 381);
            this.groupBoxButtons.Name = "groupBoxButtons";
            this.groupBoxButtons.Size = new System.Drawing.Size(467, 43);
            this.groupBoxButtons.TabIndex = 3;
            this.groupBoxButtons.TabStop = false;
            // 
            // XMLTestPath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 424);
            this.Controls.Add(this.gbxResults);
            this.Controls.Add(this.gbxExpression);
            this.Controls.Add(this.groupBoxButtons);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "XMLTestPath";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test XPath expressions";
            this.Load += new System.EventHandler(this.XMLTestPath_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XMLTestPath_Closing);
            this.gbxExpression.ResumeLayout(false);
            this.gbxExpression.PerformLayout();
            this.gbxResults.ResumeLayout(false);
            this.groupBoxButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxExpression;
        private System.Windows.Forms.Button btnTest;
        internal System.Windows.Forms.TextBox txtTestExpression;
        private System.Windows.Forms.GroupBox gbxResults;
        private System.Windows.Forms.RichTextBox rtbResults;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RadioButton optOuterXml;
        private System.Windows.Forms.RadioButton optInnerXml;
        private System.Windows.Forms.RadioButton optValueOnly;

        // Resource manager for this consform:
        System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(typeof(XMLTestPathForm));

        #region INITIALIZATION

        public XMLTestPathForm(string Path) { XMLTestPathBas(Path); }

        public XMLTestPathForm() { XMLTestPathBas(null); }

        private void XMLTestPathBas(string Path)
        // Function called from constructors
        {
            InitializeComponent();    

            // load the file upon init
            try
            {
                // Doc.Load(inputFilePath);
                string title = "Testing XPath";
                this.Text = title;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Open File Error");
                return;
            }
        }



        private void XMLTestPath_Load(object sender, EventArgs e)
        {
            try
            {
                if (InitialPath != null)
                {
                    txtTestExpression.Text = InitialPath;
                    btnTest_Click(null, null);
                }
                //Icon icon1 = null, icon2 = null, icon3 = null;
                //icon1 = ((Icon)(res.GetObject("$this.Icon")));
                //icon2 = ((Icon)(res.GetObject("xml_root")));
                //icon3 = ((Icon)(res.GetObject("xml_element")));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Open File Error"); }
        }

        #endregion   // INITIALIZATION


        /// <summary>
        /// Use XPath to search for the path/values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void btnTest_Click(object sender, EventArgs e)
        {
            rtbResults.Text = string.Empty;

            // get an xpath navigator   
            XPathNavigator navigator = Doc.CreateNavigator();

            // contain the results in a stringbuilder
            StringBuilder sb = new StringBuilder();


            try
            {
                // look for the path and use an iterator to capture the results
                XPathNodeIterator nodes = navigator.Select(txtTestExpression.Text);
                while (nodes.MoveNext())
                {
                    XPathNavigator node = nodes.Current;

                    // depending upon which radio button is checked,
                    // write the results to the string builder
                    if (optInnerXml.Checked == true)
                    {
                        sb.Append(node.InnerXml + Environment.NewLine);
                    }
                    else if (optOuterXml.Checked == true)
                    {
                        sb.Append(node.OuterXml + Environment.NewLine);
                    }
                    else
                    {
                        sb.Append(node.Value + Environment.NewLine);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "XPath Error");
            }

            // post any results to the results box
            rtbResults.Text = sb.ToString();
        }


        /// <summary>
        /// close the consform, not the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void XMLTestPath_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (CloseDelegate != null)
                    CloseDelegate();
            }
            catch { }
        }

    }
}
