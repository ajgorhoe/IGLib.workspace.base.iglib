// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class TestTestControl
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
            this.fileSelector2 = new IG.Forms.FileSelector();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParameterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnChangedFont = new System.Windows.Forms.Button();
            this.btnOriginal = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.chkIriginal = new System.Windows.Forms.CheckBox();
            this.txtOriginal = new System.Windows.Forms.TextBox();
            this.numOriginal = new System.Windows.Forms.NumericUpDown();
            this.comboOriginal = new System.Windows.Forms.ComboBox();
            this.pnlOriginal = new System.Windows.Forms.Panel();
            this.lblOriginal = new System.Windows.Forms.Label();
            this.pnlFileSelectors = new System.Windows.Forms.Panel();
            this.lblFileSelector1 = new System.Windows.Forms.Label();
            this.lblFileSelector2 = new System.Windows.Forms.Label();
            this.lblFileSelectorsTitle = new System.Windows.Forms.Label();
            this.directorySelector1 = new IG.Forms.DirectorySelector();
            this.lblDirectorySelector = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOriginal)).BeginInit();
            this.pnlOriginal.SuspendLayout();
            this.pnlFileSelectors.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileSelector1
            // 
            this.fileSelector1.AllowDrop = true;
            this.fileSelector1.AllowEnvironmentVariables = true;
            this.fileSelector1.AllowExistentFiles = true;
            this.fileSelector1.AllowNonexistentFiles = true;
            this.fileSelector1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileSelector1.ErrorBackground = System.Drawing.Color.Orange;
            this.fileSelector1.ErrorForeground = System.Drawing.Color.Red;
            this.fileSelector1.FilePath = "d:\\users\\igor\\ighome\\bin\\browse.bat";
            this.fileSelector1.IsBrowsable = true;
            this.fileSelector1.IsDragAndDrop = true;
            this.fileSelector1.Location = new System.Drawing.Point(3, 63);
            this.fileSelector1.MinimumSize = new System.Drawing.Size(200, 15);
            this.fileSelector1.Name = "fileSelector1";
            this.fileSelector1.NormalBackground = System.Drawing.SystemColors.Window;
            this.fileSelector1.NormalForeground = System.Drawing.Color.Black;
            this.fileSelector1.OriginalFilePath = "%IGHOME%\\bin\\browse.bat";
            this.fileSelector1.Size = new System.Drawing.Size(616, 25);
            this.fileSelector1.TabIndex = 15;
            this.fileSelector1.UseAbsolutePaths = false;
            this.fileSelector1.UseRelativePaths = false;
            // 
            // fileSelector2
            // 
            this.fileSelector2.AllowDrop = true;
            this.fileSelector2.AllowEnvironmentVariables = true;
            this.fileSelector2.AllowExistentFiles = true;
            this.fileSelector2.AllowNonexistentFiles = false;
            this.fileSelector2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileSelector2.ErrorBackground = System.Drawing.Color.Orange;
            this.fileSelector2.ErrorForeground = System.Drawing.Color.Red;
            this.fileSelector2.FilePath = "d:\\users\\igor\\ighome\\bin\\browse.bat";
            this.fileSelector2.IsBrowsable = true;
            this.fileSelector2.IsDragAndDrop = true;
            this.fileSelector2.Location = new System.Drawing.Point(3, 19);
            this.fileSelector2.MinimumSize = new System.Drawing.Size(200, 15);
            this.fileSelector2.Name = "fileSelector2";
            this.fileSelector2.NormalBackground = System.Drawing.SystemColors.Window;
            this.fileSelector2.NormalForeground = System.Drawing.Color.Black;
            this.fileSelector2.OriginalFilePath = "d:\\users\\igor\\ighome\\bin\\browse.bat";
            this.fileSelector2.Size = new System.Drawing.Size(616, 25);
            this.fileSelector2.TabIndex = 16;
            this.fileSelector2.UseAbsolutePaths = false;
            this.fileSelector2.UseRelativePaths = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.ParameterName,
            this.Value});
            this.dataGridView1.Location = new System.Drawing.Point(2, 380);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(758, 128);
            this.dataGridView1.TabIndex = 5;
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            // 
            // ParameterName
            // 
            this.ParameterName.HeaderText = "Parameter Name";
            this.ParameterName.Name = "ParameterName";
            // 
            // Value
            // 
            this.Value.HeaderText = "Parameter Value";
            this.Value.Name = "Value";
            // 
            // btnChangedFont
            // 
            this.btnChangedFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangedFont.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
            this.btnChangedFont.Location = new System.Drawing.Point(640, 3);
            this.btnChangedFont.Name = "btnChangedFont";
            this.btnChangedFont.Size = new System.Drawing.Size(120, 41);
            this.btnChangedFont.TabIndex = 7;
            this.btnChangedFont.Text = "EnlargedFont";
            this.btnChangedFont.UseVisualStyleBackColor = true;
            // 
            // btnOriginal
            // 
            this.btnOriginal.Location = new System.Drawing.Point(3, 39);
            this.btnOriginal.Name = "btnOriginal";
            this.btnOriginal.Size = new System.Drawing.Size(75, 23);
            this.btnOriginal.TabIndex = 9;
            this.btnOriginal.Text = "Original Button";
            this.btnOriginal.UseVisualStyleBackColor = true;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(244, 13);
            this.lblTitle.TabIndex = 10;
            this.lblTitle.Text = "This control contains controls with default settings.";
            // 
            // chkIriginal
            // 
            this.chkIriginal.AutoSize = true;
            this.chkIriginal.Checked = true;
            this.chkIriginal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIriginal.Location = new System.Drawing.Point(3, 16);
            this.chkIriginal.Name = "chkIriginal";
            this.chkIriginal.Size = new System.Drawing.Size(112, 17);
            this.chkIriginal.TabIndex = 11;
            this.chkIriginal.Text = "Original Checkbox";
            this.chkIriginal.UseVisualStyleBackColor = true;
            // 
            // txtOriginal
            // 
            this.txtOriginal.Location = new System.Drawing.Point(3, 68);
            this.txtOriginal.Name = "txtOriginal";
            this.txtOriginal.Size = new System.Drawing.Size(100, 20);
            this.txtOriginal.TabIndex = 12;
            this.txtOriginal.Text = "Original Text Box";
            // 
            // numOriginal
            // 
            this.numOriginal.Location = new System.Drawing.Point(3, 121);
            this.numOriginal.Name = "numOriginal";
            this.numOriginal.Size = new System.Drawing.Size(120, 20);
            this.numOriginal.TabIndex = 13;
            this.numOriginal.Value = new decimal(new int[] {
            22,
            0,
            0,
            0});
            // 
            // comboOriginal
            // 
            this.comboOriginal.FormattingEnabled = true;
            this.comboOriginal.Items.AddRange(new object[] {
            "Item 1",
            "Item 2",
            "Item 3",
            "Item 4",
            "Item 5"});
            this.comboOriginal.Location = new System.Drawing.Point(3, 94);
            this.comboOriginal.Name = "comboOriginal";
            this.comboOriginal.Size = new System.Drawing.Size(121, 21);
            this.comboOriginal.TabIndex = 14;
            this.comboOriginal.Text = "Original Combo Box";
            // 
            // pnlOriginal
            // 
            this.pnlOriginal.AutoSize = true;
            this.pnlOriginal.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOriginal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOriginal.Controls.Add(this.lblOriginal);
            this.pnlOriginal.Controls.Add(this.comboOriginal);
            this.pnlOriginal.Controls.Add(this.chkIriginal);
            this.pnlOriginal.Controls.Add(this.numOriginal);
            this.pnlOriginal.Controls.Add(this.btnOriginal);
            this.pnlOriginal.Controls.Add(this.txtOriginal);
            this.pnlOriginal.Location = new System.Drawing.Point(2, 13);
            this.pnlOriginal.Margin = new System.Windows.Forms.Padding(0);
            this.pnlOriginal.Name = "pnlOriginal";
            this.pnlOriginal.Size = new System.Drawing.Size(129, 146);
            this.pnlOriginal.TabIndex = 15;
            // 
            // lblOriginal
            // 
            this.lblOriginal.AutoSize = true;
            this.lblOriginal.Location = new System.Drawing.Point(3, 0);
            this.lblOriginal.Name = "lblOriginal";
            this.lblOriginal.Size = new System.Drawing.Size(71, 13);
            this.lblOriginal.TabIndex = 12;
            this.lblOriginal.Text = "Original Label";
            // 
            // pnlFileSelectors
            // 
            this.pnlFileSelectors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFileSelectors.Controls.Add(this.directorySelector1);
            this.pnlFileSelectors.Controls.Add(this.lblDirectorySelector);
            this.pnlFileSelectors.Controls.Add(this.lblFileSelector1);
            this.pnlFileSelectors.Controls.Add(this.lblFileSelector2);
            this.pnlFileSelectors.Controls.Add(this.fileSelector1);
            this.pnlFileSelectors.Controls.Add(this.fileSelector2);
            this.pnlFileSelectors.Location = new System.Drawing.Point(138, 50);
            this.pnlFileSelectors.Name = "pnlFileSelectors";
            this.pnlFileSelectors.Size = new System.Drawing.Size(622, 177);
            this.pnlFileSelectors.TabIndex = 17;
            // 
            // lblFileSelector1
            // 
            this.lblFileSelector1.AutoSize = true;
            this.lblFileSelector1.Location = new System.Drawing.Point(3, 3);
            this.lblFileSelector1.Name = "lblFileSelector1";
            this.lblFileSelector1.Size = new System.Drawing.Size(100, 13);
            this.lblFileSelector1.TabIndex = 18;
            this.lblFileSelector1.Text = "Default file selector:";
            // 
            // lblFileSelector2
            // 
            this.lblFileSelector2.AutoSize = true;
            this.lblFileSelector2.Location = new System.Drawing.Point(3, 47);
            this.lblFileSelector2.Name = "lblFileSelector2";
            this.lblFileSelector2.Size = new System.Drawing.Size(377, 13);
            this.lblFileSelector2.TabIndex = 18;
            this.lblFileSelector2.Text = "Another file selector - allow nonexistent, relative paths, environment expansion:" +
    "";
            // 
            // lblFileSelectorsTitle
            // 
            this.lblFileSelectorsTitle.AutoSize = true;
            this.lblFileSelectorsTitle.ForeColor = System.Drawing.Color.Blue;
            this.lblFileSelectorsTitle.Location = new System.Drawing.Point(3, 2);
            this.lblFileSelectorsTitle.Name = "lblFileSelectorsTitle";
            this.lblFileSelectorsTitle.Size = new System.Drawing.Size(136, 13);
            this.lblFileSelectorsTitle.TabIndex = 17;
            this.lblFileSelectorsTitle.Text = "File and Directory Selectors";
            // 
            // directorySelector1
            // 
            this.directorySelector1.AllowDrop = true;
            this.directorySelector1.AllowEnvironmentVariables = true;
            this.directorySelector1.AllowExistentDirectories = true;
            this.directorySelector1.AllowNonexistentDirectories = true;
            this.directorySelector1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.directorySelector1.ErrorBackground = System.Drawing.Color.Orange;
            this.directorySelector1.ErrorForeground = System.Drawing.Color.Red;
            this.directorySelector1.DirectoryPath = "d:\\users\\igor\\ighome\\bin\\browse.bat";
            this.directorySelector1.IsBrowsable = true;
            this.directorySelector1.IsDragAndDrop = true;
            this.directorySelector1.Location = new System.Drawing.Point(3, 112);
            this.directorySelector1.MinimumSize = new System.Drawing.Size(200, 15);
            this.directorySelector1.Name = "directorySelector1";
            this.directorySelector1.NormalBackground = System.Drawing.SystemColors.Window;
            this.directorySelector1.NormalForeground = System.Drawing.Color.Black;
            this.directorySelector1.OriginalDirectoryPath = "%IGHOME%\\bin\\browse.bat";
            this.directorySelector1.Size = new System.Drawing.Size(616, 25);
            this.directorySelector1.TabIndex = 19;
            this.directorySelector1.UseAbsolutePaths = false;
            this.directorySelector1.UseRelativePaths = false;
            // 
            // lblDirectorySelector
            // 
            this.lblDirectorySelector.AutoSize = true;
            this.lblDirectorySelector.Location = new System.Drawing.Point(3, 96);
            this.lblDirectorySelector.Name = "lblDirectorySelector";
            this.lblDirectorySelector.Size = new System.Drawing.Size(95, 13);
            this.lblDirectorySelector.TabIndex = 18;
            this.lblDirectorySelector.Text = "Directory Delector:";
            // 
            // TestTestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlFileSelectors);
            this.Controls.Add(this.pnlOriginal);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnChangedFont);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TestTestControl";
            this.Size = new System.Drawing.Size(763, 510);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOriginal)).EndInit();
            this.pnlOriginal.ResumeLayout(false);
            this.pnlOriginal.PerformLayout();
            this.pnlFileSelectors.ResumeLayout(false);
            this.pnlFileSelectors.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnChangedFont;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Button btnOriginal;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.CheckBox chkIriginal;
        private System.Windows.Forms.TextBox txtOriginal;
        private System.Windows.Forms.NumericUpDown numOriginal;
        private System.Windows.Forms.ComboBox comboOriginal;
        private System.Windows.Forms.Panel pnlOriginal;
        private System.Windows.Forms.Label lblOriginal;
        private IG.Forms.FileSelector fileSelector1;
        private System.Windows.Forms.Panel pnlFileSelectors;
        private System.Windows.Forms.Label lblFileSelector2;
        private System.Windows.Forms.Label lblFileSelector1;
        private System.Windows.Forms.Label lblFileSelectorsTitle;
        private IG.Forms.FileSelector fileSelector2;
        private DirectorySelector directorySelector1;
        private System.Windows.Forms.Label lblDirectorySelector;

        // private System.Windows.Forms.DataGridViewTextBoxColumn Name;

        public System.Drawing.Font DatagridFont
        {
            get { return this.dataGridView1.Font; }
            set { throw new System.Exception("Can not set the datagrid view font."); }
        }
    }
}
