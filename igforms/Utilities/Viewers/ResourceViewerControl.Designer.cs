// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class ResourceViewerControl
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
            this.components = new System.ComponentModel.Container();
            this.resourceSelector1 = new IG.Forms.ResourceSelector();
            this.grpPreview = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.soundPlayerControlSimple1 = new IG.Forms.SoundPlayerControlSimple();
            this.imageViewerControl1 = new IG.Forms.ImageViewerControl();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnShowInExternalViewer = new System.Windows.Forms.Button();
            this.fileViewerControl1 = new IG.Forms.FileViewerControl();
            this.grpPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // resourceSelector1
            // 
            this.resourceSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resourceSelector1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resourceSelector1.IncludeResourceFiles = true;
            this.resourceSelector1.Location = new System.Drawing.Point(3, 3);
            this.resourceSelector1.MinimumSize = new System.Drawing.Size(550, 240);
            this.resourceSelector1.MultipleAssembliesAllowed = false;
            this.resourceSelector1.Name = "resourceSelector1";
            this.resourceSelector1.SelectFromEmbeddedResources = false;
            this.resourceSelector1.SelectFromResxResources = true;
            this.resourceSelector1.Size = new System.Drawing.Size(1053, 240);
            this.resourceSelector1.TabIndex = 0;
            this.resourceSelector1.ResourceSelected += new System.EventHandler(this.resourceSelector1_ResourceSelected);
            this.resourceSelector1.SelectionParametersChanged += new System.EventHandler(this.resourceSelector1_SelectionParametersChanged);
            // 
            // grpPreview
            // 
            this.grpPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPreview.Controls.Add(this.splitContainer1);
            this.grpPreview.Location = new System.Drawing.Point(3, 249);
            this.grpPreview.Name = "grpPreview";
            this.grpPreview.Padding = new System.Windows.Forms.Padding(0);
            this.grpPreview.Size = new System.Drawing.Size(1053, 455);
            this.grpPreview.TabIndex = 1;
            this.grpPreview.TabStop = false;
            this.grpPreview.Text = "Resource Preview";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 16);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fileViewerControl1);
            this.splitContainer1.Panel1.Controls.Add(this.soundPlayerControlSimple1);
            this.splitContainer1.Panel1.Controls.Add(this.imageViewerControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtOutput);
            this.splitContainer1.Size = new System.Drawing.Size(1047, 436);
            this.splitContainer1.SplitterDistance = 601;
            this.splitContainer1.TabIndex = 2;
            // 
            // soundPlayerControlSimple1
            // 
            this.soundPlayerControlSimple1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.soundPlayerControlSimple1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.soundPlayerControlSimple1.Location = new System.Drawing.Point(42, 43);
            this.soundPlayerControlSimple1.MinimumSize = new System.Drawing.Size(300, 200);
            this.soundPlayerControlSimple1.Name = "soundPlayerControlSimple1";
            this.soundPlayerControlSimple1.Size = new System.Drawing.Size(300, 200);
            this.soundPlayerControlSimple1.TabIndex = 1;
            // 
            // imageViewerControl1
            // 
            this.imageViewerControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.imageViewerControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageViewerControl1.CanViewFiles = true;
            this.imageViewerControl1.CanViewStreams = true;
            this.imageViewerControl1.CanViewUrls = true;
            this.imageViewerControl1.IsBrowsable = true;
            this.imageViewerControl1.IsControlsVisible = true;
            this.imageViewerControl1.IsDragAndDrop = true;
            this.imageViewerControl1.IsShownImmediately = true;
            this.imageViewerControl1.Location = new System.Drawing.Point(0, 0);
            this.imageViewerControl1.MinimumSize = new System.Drawing.Size(300, 200);
            this.imageViewerControl1.Name = "imageViewerControl1";
            this.imageViewerControl1.Padding = new System.Windows.Forms.Padding(2);
            this.imageViewerControl1.Size = new System.Drawing.Size(300, 200);
            this.imageViewerControl1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imageViewerControl1.TabIndex = 0;
            this.imageViewerControl1.ViewedFile = null;
            this.imageViewerControl1.ViewedStream = null;
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(3, 3);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(436, 430);
            this.txtOutput.TabIndex = 2;
            this.txtOutput.Text = "<< Select resources. >>";
            // 
            // btnShowInExternalViewer
            // 
            this.btnShowInExternalViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowInExternalViewer.Location = new System.Drawing.Point(891, 176);
            this.btnShowInExternalViewer.Name = "btnShowInExternalViewer";
            this.btnShowInExternalViewer.Size = new System.Drawing.Size(159, 23);
            this.btnShowInExternalViewer.TabIndex = 2;
            this.btnShowInExternalViewer.Text = "Launch external viewer";
            this.btnShowInExternalViewer.UseVisualStyleBackColor = true;
            // 
            // fileViewerControl1
            // 
            this.fileViewerControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fileViewerControl1.CanViewFiles = true;
            this.fileViewerControl1.CanViewStreams = true;
            this.fileViewerControl1.CanViewUrls = true;
            this.fileViewerControl1.IsBrowsable = true;
            this.fileViewerControl1.IsControlsVisible = true;
            this.fileViewerControl1.IsDragAndDrop = true;
            this.fileViewerControl1.IsShownImmediately = true;
            this.fileViewerControl1.Location = new System.Drawing.Point(109, 84);
            this.fileViewerControl1.MinimumSize = new System.Drawing.Size(300, 200);
            this.fileViewerControl1.Name = "fileViewerControl1";
            this.fileViewerControl1.Padding = new System.Windows.Forms.Padding(2);
            this.fileViewerControl1.Size = new System.Drawing.Size(300, 200);
            this.fileViewerControl1.TabIndex = 2;
            this.fileViewerControl1.ViewedFile = null;
            this.fileViewerControl1.ViewedStream = null;
            // 
            // ResourceViewerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnShowInExternalViewer);
            this.Controls.Add(this.grpPreview);
            this.Controls.Add(this.resourceSelector1);
            this.Name = "ResourceViewerControl";
            this.Size = new System.Drawing.Size(1059, 707);
            this.grpPreview.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ResourceSelector resourceSelector1;
        private System.Windows.Forms.GroupBox grpPreview;
        private ImageViewerControl imageViewerControl1;
        private SoundPlayerControlSimple soundPlayerControlSimple1;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnShowInExternalViewer;
        private FileViewerControl fileViewerControl1;
    }
}
