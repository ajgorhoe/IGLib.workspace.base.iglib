// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class HashForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hashGeneratorControl1 = new IG.Forms.HashControl();
            this.SuspendLayout();
            // 
            // hashGeneratorControl1
            // 
            this.hashGeneratorControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashGeneratorControl1.DirectoryPath = null;
            this.hashGeneratorControl1.FilePath = null;
            this.hashGeneratorControl1.Location = new System.Drawing.Point(0, 0);
            this.hashGeneratorControl1.MinimumSize = new System.Drawing.Size(780, 360);
            this.hashGeneratorControl1.Name = "hashGeneratorControl1";
            this.hashGeneratorControl1.Size = new System.Drawing.Size(878, 522);
            this.hashGeneratorControl1.TabIndex = 0;
            // 
            // HashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 522);
            this.Controls.Add(this.hashGeneratorControl1);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.MinimumSize = new System.Drawing.Size(895, 506);
            this.Name = "HashForm";
            this.Text = "Hash Generator for Files and Text";
            this.ResumeLayout(false);

        }

        #endregion

        private HashControl hashGeneratorControl1;
    }
}