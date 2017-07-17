// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class HashAboutControl
    {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnHelpHtml = new System.Windows.Forms.Button();
            this.pnlAbout = new System.Windows.Forms.Panel();
            this.lblAboutWeb = new System.Windows.Forms.Label();
            this.lblAboutContact = new System.Windows.Forms.Label();
            this.lblAbourContactIntro = new System.Windows.Forms.Label();
            this.lblAboutAuthors = new System.Windows.Forms.Label();
            this.lblAboutAuthorsIntro = new System.Windows.Forms.Label();
            this.lblAboutTitle = new System.Windows.Forms.Label();
            this.pbIGLib = new System.Windows.Forms.PictureBox();
            this.lblPoweredIGLib = new System.Windows.Forms.Label();
            this.pnlIGLib = new System.Windows.Forms.Panel();
            this.btnWeb = new System.Windows.Forms.Button();
            this.lblHomeBrowse = new System.Windows.Forms.Label();
            this.lblHelpBrowse = new System.Windows.Forms.Label();
            this.btnHelpPdf = new System.Windows.Forms.Button();
            this.pnlAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIGLib)).BeginInit();
            this.pnlIGLib.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Calibri", 20F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblTitle.ForeColor = System.Drawing.Color.Blue;
            this.lblTitle.Location = new System.Drawing.Point(10, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(231, 33);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "About the Software";
            // 
            // btnHelpHtml
            // 
            this.btnHelpHtml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelpHtml.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnHelpHtml.Location = new System.Drawing.Point(543, 50);
            this.btnHelpHtml.Name = "btnHelpHtml";
            this.btnHelpHtml.Size = new System.Drawing.Size(136, 34);
            this.btnHelpHtml.TabIndex = 1;
            this.btnHelpHtml.Text = "Help (HTML)";
            this.btnHelpHtml.UseVisualStyleBackColor = true;
            this.btnHelpHtml.Click += new System.EventHandler(this.btnHelpHtml_Click);
            // 
            // pnlAbout
            // 
            this.pnlAbout.BackColor = System.Drawing.Color.Transparent;
            this.pnlAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAbout.Controls.Add(this.lblAboutWeb);
            this.pnlAbout.Controls.Add(this.lblAboutContact);
            this.pnlAbout.Controls.Add(this.lblAbourContactIntro);
            this.pnlAbout.Controls.Add(this.lblAboutAuthors);
            this.pnlAbout.Controls.Add(this.lblAboutAuthorsIntro);
            this.pnlAbout.Controls.Add(this.lblAboutTitle);
            this.pnlAbout.Location = new System.Drawing.Point(16, 61);
            this.pnlAbout.Name = "pnlAbout";
            this.pnlAbout.Size = new System.Drawing.Size(478, 170);
            this.pnlAbout.TabIndex = 2;
            this.pnlAbout.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlAbout_Paint);
            // 
            // lblAboutWeb
            // 
            this.lblAboutWeb.AutoSize = true;
            this.lblAboutWeb.Font = new System.Drawing.Font("Times New Roman", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAboutWeb.ForeColor = System.Drawing.Color.Blue;
            this.lblAboutWeb.Location = new System.Drawing.Point(12, 141);
            this.lblAboutWeb.Name = "lblAboutWeb";
            this.lblAboutWeb.Size = new System.Drawing.Size(194, 15);
            this.lblAboutWeb.TabIndex = 1;
            this.lblAboutWeb.Text = "www2.arnes.si/~ljc3m2/igor/iglib/";
            this.lblAboutWeb.Click += new System.EventHandler(this.lblAboutWeb_Click);
            // 
            // lblAboutContact
            // 
            this.lblAboutContact.AutoSize = true;
            this.lblAboutContact.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.lblAboutContact.Location = new System.Drawing.Point(12, 123);
            this.lblAboutContact.Name = "lblAboutContact";
            this.lblAboutContact.Size = new System.Drawing.Size(134, 16);
            this.lblAboutContact.TabIndex = 1;
            this.lblAboutContact.Text = "gresovnik@gmail.com";
            // 
            // lblAbourContactIntro
            // 
            this.lblAbourContactIntro.AutoSize = true;
            this.lblAbourContactIntro.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAbourContactIntro.Location = new System.Drawing.Point(12, 104);
            this.lblAbourContactIntro.Name = "lblAbourContactIntro";
            this.lblAbourContactIntro.Size = new System.Drawing.Size(66, 19);
            this.lblAbourContactIntro.TabIndex = 1;
            this.lblAbourContactIntro.Text = "Contact:";
            // 
            // lblAboutAuthors
            // 
            this.lblAboutAuthors.AutoSize = true;
            this.lblAboutAuthors.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Italic);
            this.lblAboutAuthors.Location = new System.Drawing.Point(12, 63);
            this.lblAboutAuthors.Name = "lblAboutAuthors";
            this.lblAboutAuthors.Size = new System.Drawing.Size(93, 16);
            this.lblAboutAuthors.TabIndex = 1;
            this.lblAboutAuthors.Text = "Igor Grešovnik";
            // 
            // lblAboutAuthorsIntro
            // 
            this.lblAboutAuthorsIntro.AutoSize = true;
            this.lblAboutAuthorsIntro.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAboutAuthorsIntro.Location = new System.Drawing.Point(12, 44);
            this.lblAboutAuthorsIntro.Name = "lblAboutAuthorsIntro";
            this.lblAboutAuthorsIntro.Size = new System.Drawing.Size(59, 19);
            this.lblAboutAuthorsIntro.TabIndex = 1;
            this.lblAboutAuthorsIntro.Text = "Author:";
            // 
            // lblAboutTitle
            // 
            this.lblAboutTitle.AutoSize = true;
            this.lblAboutTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAboutTitle.Location = new System.Drawing.Point(3, 9);
            this.lblAboutTitle.Name = "lblAboutTitle";
            this.lblAboutTitle.Size = new System.Drawing.Size(215, 24);
            this.lblAboutTitle.TabIndex = 0;
            this.lblAboutTitle.Text = "HashForm, version 1.6.1";
            // 
            // pbIGLib
            // 
            this.pbIGLib.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbIGLib.Image = global::IG.Forms.Properties.Resources.iglib_small;
            this.pbIGLib.Location = new System.Drawing.Point(3, 5);
            this.pbIGLib.Name = "pbIGLib";
            this.pbIGLib.Size = new System.Drawing.Size(340, 259);
            this.pbIGLib.TabIndex = 3;
            this.pbIGLib.TabStop = false;
            // 
            // lblPoweredIGLib
            // 
            this.lblPoweredIGLib.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPoweredIGLib.AutoSize = true;
            this.lblPoweredIGLib.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(20)))), ((int)(((byte)(0)))));
            this.lblPoweredIGLib.Font = new System.Drawing.Font("Times New Roman", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoweredIGLib.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lblPoweredIGLib.Location = new System.Drawing.Point(12, 228);
            this.lblPoweredIGLib.Name = "lblPoweredIGLib";
            this.lblPoweredIGLib.Size = new System.Drawing.Size(172, 24);
            this.lblPoweredIGLib.TabIndex = 4;
            this.lblPoweredIGLib.Text = "Powered by IGLib.";
            // 
            // pnlIGLib
            // 
            this.pnlIGLib.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlIGLib.Controls.Add(this.pbIGLib);
            this.pnlIGLib.Controls.Add(this.lblPoweredIGLib);
            this.pnlIGLib.Location = new System.Drawing.Point(336, 241);
            this.pnlIGLib.Name = "pnlIGLib";
            this.pnlIGLib.Size = new System.Drawing.Size(346, 267);
            this.pnlIGLib.TabIndex = 5;
            // 
            // btnWeb
            // 
            this.btnWeb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWeb.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnWeb.Location = new System.Drawing.Point(543, 90);
            this.btnWeb.Name = "btnWeb";
            this.btnWeb.Size = new System.Drawing.Size(136, 34);
            this.btnWeb.TabIndex = 1;
            this.btnWeb.Text = "View Web Site";
            this.btnWeb.UseVisualStyleBackColor = true;
            this.btnWeb.Click += new System.EventHandler(this.btnWeb_Click);
            // 
            // lblHomeBrowse
            // 
            this.lblHomeBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHomeBrowse.AutoSize = true;
            this.lblHomeBrowse.Font = new System.Drawing.Font("Times New Roman", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblHomeBrowse.ForeColor = System.Drawing.Color.Blue;
            this.lblHomeBrowse.Location = new System.Drawing.Point(540, 147);
            this.lblHomeBrowse.Name = "lblHomeBrowse";
            this.lblHomeBrowse.Size = new System.Drawing.Size(132, 15);
            this.lblHomeBrowse.TabIndex = 7;
            this.lblHomeBrowse.Text = "Open Home in Browser";
            this.lblHomeBrowse.Click += new System.EventHandler(this.lblBrowseHome_Click);
            // 
            // lblHelpBrowse
            // 
            this.lblHelpBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHelpBrowse.AutoSize = true;
            this.lblHelpBrowse.Font = new System.Drawing.Font("Times New Roman", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblHelpBrowse.ForeColor = System.Drawing.Color.Blue;
            this.lblHelpBrowse.Location = new System.Drawing.Point(540, 129);
            this.lblHelpBrowse.Name = "lblHelpBrowse";
            this.lblHelpBrowse.Size = new System.Drawing.Size(127, 15);
            this.lblHelpBrowse.TabIndex = 8;
            this.lblHelpBrowse.Text = "Open Help in Browser";
            this.lblHelpBrowse.Click += new System.EventHandler(this.lblBrowseHelp_Click);
            // 
            // btnHelpPdf
            // 
            this.btnHelpPdf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelpPdf.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnHelpPdf.Location = new System.Drawing.Point(543, 10);
            this.btnHelpPdf.Name = "btnHelpPdf";
            this.btnHelpPdf.Size = new System.Drawing.Size(136, 34);
            this.btnHelpPdf.TabIndex = 10;
            this.btnHelpPdf.Text = "Help";
            this.btnHelpPdf.UseVisualStyleBackColor = true;
            this.btnHelpPdf.Click += new System.EventHandler(this.btnHelpPdf_Click);
            // 
            // HashAboutControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnHelpPdf);
            this.Controls.Add(this.lblHomeBrowse);
            this.Controls.Add(this.lblHelpBrowse);
            this.Controls.Add(this.btnWeb);
            this.Controls.Add(this.btnHelpHtml);
            this.Controls.Add(this.pnlIGLib);
            this.Controls.Add(this.pnlAbout);
            this.Controls.Add(this.lblTitle);
            this.MinimumSize = new System.Drawing.Size(494, 320);
            this.Name = "HashAboutControl";
            this.Size = new System.Drawing.Size(694, 520);
            this.pnlAbout.ResumeLayout(false);
            this.pnlAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIGLib)).EndInit();
            this.pnlIGLib.ResumeLayout(false);
            this.pnlIGLib.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnHelpHtml;
        private System.Windows.Forms.Panel pnlAbout;
        private System.Windows.Forms.Label lblAboutAuthors;
        private System.Windows.Forms.Label lblAboutAuthorsIntro;
        private System.Windows.Forms.Label lblAboutTitle;
        private System.Windows.Forms.Label lblAboutWeb;
        private System.Windows.Forms.Label lblAboutContact;
        private System.Windows.Forms.Label lblAbourContactIntro;
        private System.Windows.Forms.PictureBox pbIGLib;
        private System.Windows.Forms.Label lblPoweredIGLib;
        private System.Windows.Forms.Panel pnlIGLib;
        private System.Windows.Forms.Button btnWeb;
        private System.Windows.Forms.Label lblHomeBrowse;
        private System.Windows.Forms.Label lblHelpBrowse;
        private System.Windows.Forms.Button btnHelpPdf;
    }
}
