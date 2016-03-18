// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class ImageViewerControl
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
            this.fileSelector1 = new IG.Forms.FileSelector();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.comboSizeMode = new System.Windows.Forms.ComboBox();
            this.lblSzeMode = new System.Windows.Forms.Label();
            this.pnlPicture = new System.Windows.Forms.Panel();
            this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDragDrop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBrowse = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowControls = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlOuter = new System.Windows.Forms.Panel();
            this.pnlControls = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlOptions.SuspendLayout();
            this.pnlPicture.SuspendLayout();
            this.contextMenuMain.SuspendLayout();
            this.pnlOuter.SuspendLayout();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileSelector1
            // 
            this.fileSelector1.AllowDrop = true;
            this.fileSelector1.AllowNonexistentFiles = true;
            this.fileSelector1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileSelector1.Dock = System.Windows.Forms.DockStyle.Top;
            this.fileSelector1.FilePath = null;
            this.fileSelector1.IsBrowsable = true;
            this.fileSelector1.IsDragAndDrop = true;
            this.fileSelector1.Location = new System.Drawing.Point(0, 0);
            this.fileSelector1.MinimumSize = new System.Drawing.Size(200, 15);
            this.fileSelector1.Name = "fileSelector1";
            this.fileSelector1.Size = new System.Drawing.Size(480, 25);
            this.fileSelector1.TabIndex = 3;
            this.fileSelector1.FileSelected += fileSelector1_FileSelected; // new System.EventHandler(fileSelector1_FileSelected);
            // 
            // pictureBox1
            // 
            this.pictureBox1.AllowDrop = true;
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(4, 4);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(472, 257);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragDrop);
            this.pictureBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragEnter);
            // 
            // pnlOptions
            // 
            this.pnlOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOptions.Controls.Add(this.comboSizeMode);
            this.pnlOptions.Controls.Add(this.lblSzeMode);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOptions.Location = new System.Drawing.Point(0, 25);
            this.pnlOptions.Margin = new System.Windows.Forms.Padding(4);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(480, 31);
            this.pnlOptions.TabIndex = 2;
            // 
            // comboSizeMode
            // 
            this.comboSizeMode.FormattingEnabled = true;
            this.comboSizeMode.Location = new System.Drawing.Point(76, 3);
            this.comboSizeMode.Name = "comboSizeMode";
            this.comboSizeMode.Size = new System.Drawing.Size(159, 21);
            this.comboSizeMode.TabIndex = 3;
            this.comboSizeMode.Text = "CenterImage";
            this.comboSizeMode.SelectedIndexChanged += new System.EventHandler(this.comboSizeMode_SelectedIndexChanged);
            this.comboSizeMode.Click += new System.EventHandler(this.comboSizeMode_Click);
            // 
            // lblSzeMode
            // 
            this.lblSzeMode.AutoSize = true;
            this.lblSzeMode.Location = new System.Drawing.Point(3, 6);
            this.lblSzeMode.Name = "lblSzeMode";
            this.lblSzeMode.Size = new System.Drawing.Size(67, 13);
            this.lblSzeMode.TabIndex = 2;
            this.lblSzeMode.Text = "Sizing mode:";
            // 
            // pnlPicture
            // 
            this.pnlPicture.AutoScroll = true;
            this.pnlPicture.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlPicture.BackColor = System.Drawing.Color.Wheat;
            this.pnlPicture.Controls.Add(this.pictureBox1);
            this.pnlPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPicture.Location = new System.Drawing.Point(0, 56);
            this.pnlPicture.Margin = new System.Windows.Forms.Padding(0);
            this.pnlPicture.MinimumSize = new System.Drawing.Size(20, 20);
            this.pnlPicture.Name = "pnlPicture";
            this.pnlPicture.Padding = new System.Windows.Forms.Padding(4);
            this.pnlPicture.Size = new System.Drawing.Size(480, 265);
            this.pnlPicture.TabIndex = 3;
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOptions,
            this.menuClearHistory});
            this.contextMenuMain.Name = "contextMenuMain";
            this.contextMenuMain.Size = new System.Drawing.Size(143, 48);
            // 
            // menuOptions
            // 
            this.menuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDragDrop,
            this.menuBrowse,
            this.menuShowControls});
            this.menuOptions.Name = "menuOptions";
            this.menuOptions.Size = new System.Drawing.Size(142, 22);
            this.menuOptions.Text = "Options";
            // 
            // menuDragDrop
            // 
            this.menuDragDrop.Checked = true;
            this.menuDragDrop.CheckOnClick = true;
            this.menuDragDrop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuDragDrop.Name = "menuDragDrop";
            this.menuDragDrop.Size = new System.Drawing.Size(152, 22);
            this.menuDragDrop.Text = "Drag and Drop";
            this.menuDragDrop.Click += new System.EventHandler(this.menuDragDrop_Click);
            // 
            // menuBrowse
            // 
            this.menuBrowse.Checked = true;
            this.menuBrowse.CheckOnClick = true;
            this.menuBrowse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuBrowse.Name = "menuBrowse";
            this.menuBrowse.Size = new System.Drawing.Size(152, 22);
            this.menuBrowse.Text = "File Browsing";
            this.menuBrowse.Click += new System.EventHandler(this.menuBrowse_Click);
            // 
            // menuShowControls
            // 
            this.menuShowControls.Checked = true;
            this.menuShowControls.CheckOnClick = true;
            this.menuShowControls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuShowControls.Name = "menuShowControls";
            this.menuShowControls.Size = new System.Drawing.Size(152, 22);
            this.menuShowControls.Text = "Show Controls";
            this.menuShowControls.Click += new System.EventHandler(this.showControlsToolStripMenuItem_Click);
            // 
            // menuClearHistory
            // 
            this.menuClearHistory.Name = "menuClearHistory";
            this.menuClearHistory.Size = new System.Drawing.Size(142, 22);
            this.menuClearHistory.Text = "Clear History";
            this.menuClearHistory.Click += new System.EventHandler(this.menuClearHistory_Click);
            // 
            // pnlOuter
            // 
            this.pnlOuter.AllowDrop = true;
            this.pnlOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOuter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOuter.Controls.Add(this.pnlPicture);
            this.pnlOuter.Controls.Add(this.pnlControls);
            this.pnlOuter.Location = new System.Drawing.Point(5, 5);
            this.pnlOuter.Name = "pnlOuter";
            this.pnlOuter.Size = new System.Drawing.Size(482, 323);
            this.pnlOuter.TabIndex = 7;
            this.pnlOuter.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragDrop);
            this.pnlOuter.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragEnter);
            // 
            // pnlControls
            // 
            this.pnlControls.AutoSize = true;
            this.pnlControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlControls.Controls.Add(this.pnlOptions);
            this.pnlControls.Controls.Add(this.fileSelector1);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(480, 56);
            this.pnlControls.TabIndex = 8;
            // 
            // ImageViewerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ContextMenuStrip = this.contextMenuMain;
            this.Controls.Add(this.pnlOuter);
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "ImageViewerControl";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(492, 333);
            this.Load += new System.EventHandler(this.ImageViewerControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.pnlPicture.ResumeLayout(false);
            this.contextMenuMain.ResumeLayout(false);
            this.pnlOuter.ResumeLayout(false);
            this.pnlOuter.PerformLayout();
            this.pnlControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private IG.Forms.FileSelector fileSelector1;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Label lblSzeMode;
        private System.Windows.Forms.ComboBox comboSizeMode;
        private System.Windows.Forms.Panel pnlPicture;
        private System.Windows.Forms.ContextMenuStrip contextMenuMain;
        private System.Windows.Forms.Panel pnlOuter;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.ToolStripMenuItem menuClearHistory;
        private System.Windows.Forms.ToolStripMenuItem menuOptions;
        private System.Windows.Forms.ToolStripMenuItem menuDragDrop;
        private System.Windows.Forms.ToolStripMenuItem menuBrowse;
        private System.Windows.Forms.ToolStripMenuItem menuShowControls;
    }
}
