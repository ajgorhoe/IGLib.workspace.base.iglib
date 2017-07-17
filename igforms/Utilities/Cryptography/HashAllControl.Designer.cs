// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class HashAllControl
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageHash = new System.Windows.Forms.TabPage();
            this.hashControl1 = new IG.Forms.HashControl();
            this.tabPageDir = new System.Windows.Forms.TabPage();
            this.hashDirControl1 = new IG.Forms.HashDirControl();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.hashFormAbout1 = new IG.Forms.HashAboutControl();
            this.tabControl1.SuspendLayout();
            this.tabPageHash.SuspendLayout();
            this.tabPageDir.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageHash);
            this.tabControl1.Controls.Add(this.tabPageDir);
            this.tabControl1.Controls.Add(this.tabPageAbout);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(805, 480);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyUp);
            // 
            // tabPageHash
            // 
            this.tabPageHash.Controls.Add(this.hashControl1);
            this.tabPageHash.Location = new System.Drawing.Point(4, 22);
            this.tabPageHash.Name = "tabPageHash";
            this.tabPageHash.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHash.Size = new System.Drawing.Size(797, 454);
            this.tabPageHash.TabIndex = 0;
            this.tabPageHash.Text = "File or Text Hashes";
            this.tabPageHash.UseVisualStyleBackColor = true;
            // 
            // hashControl1
            // 
            this.hashControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashControl1.DirectoryPath = null;
            this.hashControl1.FilePath = null;
            this.hashControl1.Location = new System.Drawing.Point(0, 0);
            this.hashControl1.MinimumSize = new System.Drawing.Size(780, 360);
            this.hashControl1.Name = "hashControl1";
            this.hashControl1.Size = new System.Drawing.Size(797, 454);
            this.hashControl1.TabIndex = 0;
            this.hashControl1.Load += new System.EventHandler(this.hashControl1_Load);
            // 
            // tabPageDir
            // 
            this.tabPageDir.Controls.Add(this.hashDirControl1);
            this.tabPageDir.Location = new System.Drawing.Point(4, 22);
            this.tabPageDir.Name = "tabPageDir";
            this.tabPageDir.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDir.Size = new System.Drawing.Size(797, 454);
            this.tabPageDir.TabIndex = 1;
            this.tabPageDir.Text = "Directory Hashes";
            this.tabPageDir.UseVisualStyleBackColor = true;
            // 
            // hashDirControl1
            // 
            this.hashDirControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashDirControl1.Location = new System.Drawing.Point(0, 0);
            this.hashDirControl1.MinimumSize = new System.Drawing.Size(780, 360);
            this.hashDirControl1.Name = "hashDirControl1";
            this.hashDirControl1.Size = new System.Drawing.Size(797, 455);
            this.hashDirControl1.TabIndex = 0;
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.hashFormAbout1);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAbout.Size = new System.Drawing.Size(797, 454);
            this.tabPageAbout.TabIndex = 2;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // hashFormAbout1
            // 
            this.hashFormAbout1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashFormAbout1.Location = new System.Drawing.Point(0, 0);
            this.hashFormAbout1.MinimumSize = new System.Drawing.Size(494, 320);
            this.hashFormAbout1.Name = "hashFormAbout1";
            this.hashFormAbout1.Size = new System.Drawing.Size(797, 455);
            this.hashFormAbout1.TabIndex = 0;
            // 
            // HashAllControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(800, 360);
            this.Name = "HashAllControl";
            this.Size = new System.Drawing.Size(805, 480);
            this.tabControl1.ResumeLayout(false);
            this.tabPageHash.ResumeLayout(false);
            this.tabPageDir.ResumeLayout(false);
            this.tabPageAbout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageHash;
        private System.Windows.Forms.TabPage tabPageDir;
        private System.Windows.Forms.TabPage tabPageAbout;
        private HashControl hashControl1;
        private IG.Forms.HashDirControl hashDirControl1;
        private IG.Forms.HashAboutControl hashFormAbout1;
    }
}
