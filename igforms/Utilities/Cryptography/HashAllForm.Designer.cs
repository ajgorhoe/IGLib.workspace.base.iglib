// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class HashAllForm
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
            this.hashAllControl1 = new IG.Forms.HashAllControl();
            this.SuspendLayout();
            // 
            // hashAllControl1
            // 
            this.hashAllControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashAllControl1.Location = new System.Drawing.Point(1, 0);
            this.hashAllControl1.MinimumSize = new System.Drawing.Size(800, 360);
            this.hashAllControl1.Name = "hashAllControl1";
            this.hashAllControl1.Size = new System.Drawing.Size(825, 542);
            this.hashAllControl1.TabIndex = 0;
            // 
            // HashAllForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 542);
            this.Controls.Add(this.hashAllControl1);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.MinimumSize = new System.Drawing.Size(795, 526);
            this.Name = "HashAllForm";
            this.Text = "HashAllForm";
            this.ResumeLayout(false);

        }

        #endregion

        private HashAllControl hashAllControl1;
    }
}