// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class DirectorySelector
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
            this.btnFileBrowse = new System.Windows.Forms.Button();
            this.lblFile = new System.Windows.Forms.Label();
            this.openDirectoryDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.comboBoxFile = new System.Windows.Forms.ComboBox();
            this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuClearHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAllowNonexistent = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAllowExistent = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExpandEnvironment = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbsolutePaths = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRelativePaths = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowPath = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPathRepresentation = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInputPath = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInputTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.menuUseFileDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFileBrowse
            // 
            this.btnFileBrowse.AllowDrop = true;
            this.btnFileBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFileBrowse.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnFileBrowse.Location = new System.Drawing.Point(279, 1);
            this.btnFileBrowse.Name = "btnFileBrowse";
            this.btnFileBrowse.Size = new System.Drawing.Size(67, 23);
            this.btnFileBrowse.TabIndex = 1;
            this.btnFileBrowse.Text = "Browse";
            this.btnFileBrowse.UseVisualStyleBackColor = true;
            this.btnFileBrowse.Click += new System.EventHandler(this.btnFileBrowse_Click);
            this.btnFileBrowse.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileSelector_DragDrop);
            this.btnFileBrowse.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileSelector_DragEnter);
            // 
            // lblFile
            // 
            this.lblFile.AllowDrop = true;
            this.lblFile.AutoSize = true;
            this.lblFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblFile.ForeColor = System.Drawing.Color.Blue;
            this.lblFile.Location = new System.Drawing.Point(3, 6);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(62, 13);
            this.lblFile.TabIndex = 100;
            this.lblFile.Text = "Directory:";
            this.lblFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileSelector_DragDrop);
            this.lblFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileSelector_DragEnter);
            // 
            // comboBoxFile
            // 
            this.comboBoxFile.AllowDrop = true;
            this.comboBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxFile.FormattingEnabled = true;
            this.comboBoxFile.Location = new System.Drawing.Point(71, 2);
            this.comboBoxFile.Name = "comboBoxFile";
            this.comboBoxFile.Size = new System.Drawing.Size(202, 21);
            this.comboBoxFile.TabIndex = 101;
            this.comboBoxFile.SelectedIndexChanged += new System.EventHandler(this.comboBoxFile_SelectedIndexChanged);
            this.comboBoxFile.TextChanged += new System.EventHandler(this.comboBoxFile_TextChanged);
            this.comboBoxFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileSelector_DragDrop);
            this.comboBoxFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileSelector_DragEnter);
            this.comboBoxFile.Enter += new System.EventHandler(this.comboBoxFile_Enter);
            this.comboBoxFile.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxFile_KeyUp);
            this.comboBoxFile.MouseEnter += new System.EventHandler(this.comboBoxFile_MouseEnter);
            this.comboBoxFile.Validated += new System.EventHandler(this.comboBoxFile_Validated);
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuClearHistory,
            this.menuAllowNonexistent,
            this.menuAllowExistent,
            this.menuExpandEnvironment,
            this.menuUseFileDialog,
            this.menuAbsolutePaths,
            this.menuRelativePaths,
            this.menuShowPath,
            this.menuInputPath});
            this.contextMenuMain.Name = "contextMenuMain";
            this.contextMenuMain.Size = new System.Drawing.Size(257, 224);
            this.contextMenuMain.Opened += new System.EventHandler(this.contextMenuMain_Opened);
            // 
            // menuClearHistory
            // 
            this.menuClearHistory.Name = "menuClearHistory";
            this.menuClearHistory.Size = new System.Drawing.Size(256, 22);
            this.menuClearHistory.Text = "Clear History";
            this.menuClearHistory.Click += new System.EventHandler(this.menuClearHistory_Click);
            // 
            // menuAllowNonexistent
            // 
            this.menuAllowNonexistent.Checked = true;
            this.menuAllowNonexistent.CheckOnClick = true;
            this.menuAllowNonexistent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuAllowNonexistent.Name = "menuAllowNonexistent";
            this.menuAllowNonexistent.Size = new System.Drawing.Size(256, 22);
            this.menuAllowNonexistent.Text = "Allow Non-existent";
            this.menuAllowNonexistent.CheckedChanged += new System.EventHandler(this.menuAllowNonexistent_CheckedChanged);
            this.menuAllowNonexistent.Click += new System.EventHandler(this.menuAllowNonexistent_Click);
            // 
            // menuAllowExistent
            // 
            this.menuAllowExistent.Checked = true;
            this.menuAllowExistent.CheckOnClick = true;
            this.menuAllowExistent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuAllowExistent.Name = "menuAllowExistent";
            this.menuAllowExistent.Size = new System.Drawing.Size(256, 22);
            this.menuAllowExistent.Text = "Allow Existent";
            this.menuAllowExistent.CheckedChanged += new System.EventHandler(this.menuAllowExistent_CheckedChanged);
            this.menuAllowExistent.Click += new System.EventHandler(this.menuAllowExistent_Click);
            // 
            // menuExpandEnvironment
            // 
            this.menuExpandEnvironment.Checked = true;
            this.menuExpandEnvironment.CheckOnClick = true;
            this.menuExpandEnvironment.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuExpandEnvironment.Name = "menuExpandEnvironment";
            this.menuExpandEnvironment.Size = new System.Drawing.Size(256, 22);
            this.menuExpandEnvironment.Text = "Expand Environment Variables";
            this.menuExpandEnvironment.CheckedChanged += new System.EventHandler(this.menuExpandEnvironment_CheckedChanged);
            this.menuExpandEnvironment.Click += new System.EventHandler(this.menuExpandEnvironment_Click);
            // 
            // menuAbsolutePaths
            // 
            this.menuAbsolutePaths.CheckOnClick = true;
            this.menuAbsolutePaths.Name = "menuAbsolutePaths";
            this.menuAbsolutePaths.Size = new System.Drawing.Size(256, 22);
            this.menuAbsolutePaths.Text = "Absolute Paths";
            this.menuAbsolutePaths.CheckedChanged += new System.EventHandler(this.menuAbsolutePaths_CheckedChanged);
            this.menuAbsolutePaths.Click += new System.EventHandler(this.menuAbsolutePaths_Click);
            // 
            // menuRelativePaths
            // 
            this.menuRelativePaths.CheckOnClick = true;
            this.menuRelativePaths.Name = "menuRelativePaths";
            this.menuRelativePaths.Size = new System.Drawing.Size(256, 22);
            this.menuRelativePaths.Text = "Relative Paths";
            this.menuRelativePaths.CheckedChanged += new System.EventHandler(this.menuRelativePaths_CheckedChanged);
            this.menuRelativePaths.Click += new System.EventHandler(this.menuRelativePaths_Click);
            // 
            // menuShowPath
            // 
            this.menuShowPath.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuPathRepresentation});
            this.menuShowPath.Name = "menuShowPath";
            this.menuShowPath.Size = new System.Drawing.Size(256, 22);
            this.menuShowPath.Text = "Show Selected Path (As Presented)";
            // 
            // menuPathRepresentation
            // 
            this.menuPathRepresentation.Name = "menuPathRepresentation";
            this.menuPathRepresentation.Size = new System.Drawing.Size(136, 22);
            this.menuPathRepresentation.Text = "<< Path >>";
            this.menuPathRepresentation.Click += new System.EventHandler(this.menuPathRepresentation_Click);
            // 
            // menuInputPath
            // 
            this.menuInputPath.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuInputTextBox});
            this.menuInputPath.Name = "menuInputPath";
            this.menuInputPath.Size = new System.Drawing.Size(256, 22);
            this.menuInputPath.Text = "Input Selected File (Original Form)";
            // 
            // menuInputTextBox
            // 
            this.menuInputTextBox.Name = "menuInputTextBox";
            this.menuInputTextBox.Size = new System.Drawing.Size(400, 23);
            this.menuInputTextBox.Text = "<< Not selected. >>";
            this.menuInputTextBox.Leave += new System.EventHandler(this.menuInputTextBox_Validated);
            this.menuInputTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.menuInputTextBox_KeyUp);
            this.menuInputTextBox.Click += new System.EventHandler(this.menuInputTextBox_Click);
            // 
            // menuUseFileDialog
            // 
            this.menuUseFileDialog.Checked = true;
            this.menuUseFileDialog.CheckOnClick = true;
            this.menuUseFileDialog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuUseFileDialog.Name = "menuUseFileDialog";
            this.menuUseFileDialog.Size = new System.Drawing.Size(256, 22);
            this.menuUseFileDialog.Text = "Use File Dialog";
            this.menuUseFileDialog.CheckedChanged += new System.EventHandler(this.menuUseFileDialog_CheckedChanged);
            this.menuUseFileDialog.Click += new System.EventHandler(this.menuUseFileDialog_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // DirectorySelector
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ContextMenuStrip = this.contextMenuMain;
            this.Controls.Add(this.comboBoxFile);
            this.Controls.Add(this.btnFileBrowse);
            this.Controls.Add(this.lblFile);
            this.MinimumSize = new System.Drawing.Size(200, 15);
            this.Name = "DirectorySelector";
            this.Size = new System.Drawing.Size(349, 25);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FileSelector_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileSelector_DragEnter);
            this.contextMenuMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnFileBrowse;
        private System.Windows.Forms.Label lblFile;
        // private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.FolderBrowserDialog openDirectoryDialog1;
        private System.Windows.Forms.ComboBox comboBoxFile;
        private System.Windows.Forms.ContextMenuStrip contextMenuMain;
        private System.Windows.Forms.ToolStripMenuItem menuClearHistory;
        private System.Windows.Forms.ToolStripMenuItem menuAllowNonexistent;
        private System.Windows.Forms.ToolStripMenuItem menuAllowExistent;
        private System.Windows.Forms.ToolStripMenuItem menuExpandEnvironment;
        private System.Windows.Forms.ToolStripMenuItem menuAbsolutePaths;
        private System.Windows.Forms.ToolStripMenuItem menuRelativePaths;
        private System.Windows.Forms.ToolStripMenuItem menuShowPath;
        private System.Windows.Forms.ToolStripMenuItem menuPathRepresentation;
        private System.Windows.Forms.ToolStripMenuItem menuInputPath;
        private System.Windows.Forms.ToolStripTextBox menuInputTextBox;
        private System.Windows.Forms.ToolStripMenuItem menuUseFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}
