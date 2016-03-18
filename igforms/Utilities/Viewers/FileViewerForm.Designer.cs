// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class FileViewerForm
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
            this.components = new System.ComponentModel.Container();
            this.imageViewerControl1 = new IG.Forms.FileViewerControl();
            this.SuspendLayout();
            // 
            // imageViewerControl1
            // 
            this.imageViewerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageViewerControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.imageViewerControl1.CanViewFiles = true;
            this.imageViewerControl1.CanViewStreams = true;
            this.imageViewerControl1.CanViewUrls = true;
            this.imageViewerControl1.IsBrowsable = true;
            this.imageViewerControl1.IsControlsVisible = true;
            this.imageViewerControl1.IsDragAndDrop = true;
            this.imageViewerControl1.IsShownImmediately = true;
            this.imageViewerControl1.Location = new System.Drawing.Point(1, -1);
            this.imageViewerControl1.MinimumSize = new System.Drawing.Size(300, 200);
            this.imageViewerControl1.Name = "imageViewerControl1";
            this.imageViewerControl1.Padding = new System.Windows.Forms.Padding(2);
            this.imageViewerControl1.Size = new System.Drawing.Size(767, 461);
            this.imageViewerControl1.TabIndex = 0;
            this.imageViewerControl1.ViewedFile = null;
            this.imageViewerControl1.ViewedStream = null;
            // 
            // FileViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 460);
            this.Controls.Add(this.imageViewerControl1);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.Name = "FileViewerForm";
            this.Text = "File Preview";
            this.ResumeLayout(false);

        }

        #endregion

        private FileViewerControl imageViewerControl1;
    }
}