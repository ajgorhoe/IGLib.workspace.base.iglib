// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class HashDirForm
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
            this.hashDirControl1 = new IG.Forms.HashDirControl();
            this.SuspendLayout();
            // 
            // hashDirControl1
            // 
            this.hashDirControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashDirControl1.Location = new System.Drawing.Point(0, 1);
            this.hashDirControl1.MinimumSize = new System.Drawing.Size(780, 360);
            this.hashDirControl1.Name = "hashDirControl1";
            this.hashDirControl1.Size = new System.Drawing.Size(824, 521);
            this.hashDirControl1.TabIndex = 0;
            // 
            // HashDirForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 522);
            this.Controls.Add(this.hashDirControl1);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.MinimumSize = new System.Drawing.Size(795, 506);
            this.Name = "HashDirForm";
            this.Text = "HashDirForm";
            this.ResumeLayout(false);

        }

        #endregion

        private IG.Forms.HashDirControl hashDirControl1;
    }
}