// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class ControlViewerForm
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
            this.controlViewerControl1 = new IG.Forms.ControlViewerControl();
            this.SuspendLayout();
            // 
            // controlViewerControl1
            // 
            this.controlViewerControl1.AutoScroll = true;
            this.controlViewerControl1.AutoSize = true;
            this.controlViewerControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controlViewerControl1.DisplayedControl = null;
            this.controlViewerControl1.Location = new System.Drawing.Point(0, 0);
            this.controlViewerControl1.Name = "controlViewerControl1";
            this.controlViewerControl1.Padding = new System.Windows.Forms.Padding(2);
            this.controlViewerControl1.ShowDummyContents = true;
            this.controlViewerControl1.Size = new System.Drawing.Size(495, 429);
            this.controlViewerControl1.TabIndex = 0;
            // 
            // ControlViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(494, 427);
            this.Controls.Add(this.controlViewerControl1);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "ControlViewerForm";
            this.Text = "ControlViewerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ControlViewerControl controlViewerControl1;
    }
}