// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class SoundPlayerFormSimple
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
            this.soundPlayerControl = new IG.Forms.SoundPlayerControlSimple();
            this.SuspendLayout();
            // 
            // soundPlayerControl
            // 
            this.soundPlayerControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.soundPlayerControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.soundPlayerControl.Location = new System.Drawing.Point(1, -1);
            this.soundPlayerControl.MinimumSize = new System.Drawing.Size(300, 200);
            this.soundPlayerControl.Name = "soundPlayerControl";
            this.soundPlayerControl.Size = new System.Drawing.Size(767, 461);
            this.soundPlayerControl.TabIndex = 0;
            // 
            // SoundPlayerFormSimple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 460);
            this.Controls.Add(this.soundPlayerControl);
            this.Name = "SoundPlayerFormSimple";
            this.Text = "Sound preview (.wav)";
            this.Icon = IG.Forms.Properties.Resources.ig;
            this.ResumeLayout(false);

        }

        #endregion

        private SoundPlayerControlSimple soundPlayerControl;
    }
}