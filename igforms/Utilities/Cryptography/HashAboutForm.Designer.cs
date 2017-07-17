// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class HashAboutForm
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
            this.hashFormAbout1 = new IG.Forms.HashAboutControl();
            this.SuspendLayout();
            // 
            // hashFormAbout1
            // 
            this.hashFormAbout1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashFormAbout1.Location = new System.Drawing.Point(0, -1);
            this.hashFormAbout1.MinimumSize = new System.Drawing.Size(494, 320);
            this.hashFormAbout1.Name = "hashFormAbout1";
            this.hashFormAbout1.Size = new System.Drawing.Size(736, 454);
            this.hashFormAbout1.TabIndex = 0;
            // 
            // HashFormAboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 451);
            this.Controls.Add(this.hashFormAbout1);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.MinimumSize = new System.Drawing.Size(722, 485);
            this.Name = "HashFormAboutForm";
            this.Text = "HashFormAboutForm";
            this.ResumeLayout(false);

        }

        #endregion

        private IG.Forms.HashAboutControl hashFormAbout1;
    }
}