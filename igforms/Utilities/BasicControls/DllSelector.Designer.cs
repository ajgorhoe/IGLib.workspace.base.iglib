namespace IG.Forms
{
    partial class DllSelector
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
            this.fileSelector1 = new IG.Forms.FileSelector();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblSelectedAssembly = new System.Windows.Forms.Label();
            this.txtSelectedAssembly = new System.Windows.Forms.TextBox();
            this.comboLoadedAssemblies = new System.Windows.Forms.ComboBox();
            this.comboReferencedAssemblies = new System.Windows.Forms.ComboBox();
            this.chkFiles = new System.Windows.Forms.CheckBox();
            this.chkLoaded = new System.Windows.Forms.CheckBox();
            this.chkReferenced = new System.Windows.Forms.CheckBox();
            this.lblAssemblyFullNameTitle = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fileSelector1
            // 
            this.fileSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelector1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileSelector1.FilePath = null;
            this.fileSelector1.Location = new System.Drawing.Point(107, 56);
            this.fileSelector1.MinimumSize = new System.Drawing.Size(160, 27);
            this.fileSelector1.Name = "fileSelector1";
            this.fileSelector1.Size = new System.Drawing.Size(465, 27);
            this.fileSelector1.TabIndex = 2;
            this.fileSelector1.FileSelected += new System.EventHandler(this.fileSelector1_FileSelected);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(3, 27);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(85, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh Lists";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblSelectedAssembly
            // 
            this.lblSelectedAssembly.AutoSize = true;
            this.lblSelectedAssembly.Location = new System.Drawing.Point(3, 6);
            this.lblSelectedAssembly.Name = "lblSelectedAssembly";
            this.lblSelectedAssembly.Size = new System.Drawing.Size(98, 13);
            this.lblSelectedAssembly.TabIndex = 4;
            this.lblSelectedAssembly.Text = "Selected assembly:";
            // 
            // txtSelectedAssembly
            // 
            this.txtSelectedAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelectedAssembly.Location = new System.Drawing.Point(107, 3);
            this.txtSelectedAssembly.Name = "txtSelectedAssembly";
            this.txtSelectedAssembly.Size = new System.Drawing.Size(465, 20);
            this.txtSelectedAssembly.TabIndex = 5;
            this.txtSelectedAssembly.Text = "<< Type assembly name or select in lists below. >>";
            this.txtSelectedAssembly.TextChanged += new System.EventHandler(this.txtSelectedAssembly_TextChanged);
            // 
            // comboLoadedAssemblies
            // 
            this.comboLoadedAssemblies.FormattingEnabled = true;
            this.comboLoadedAssemblies.Location = new System.Drawing.Point(162, 29);
            this.comboLoadedAssemblies.Name = "comboLoadedAssemblies";
            this.comboLoadedAssemblies.Size = new System.Drawing.Size(158, 21);
            this.comboLoadedAssemblies.TabIndex = 6;
            this.comboLoadedAssemblies.Text = "<< Loaded >>";
            this.comboLoadedAssemblies.SelectedIndexChanged += new System.EventHandler(this.comboLoadedAssemblies_SelectedIndexChanged);
            // 
            // comboReferencedAssemblies
            // 
            this.comboReferencedAssemblies.FormattingEnabled = true;
            this.comboReferencedAssemblies.Location = new System.Drawing.Point(413, 29);
            this.comboReferencedAssemblies.Name = "comboReferencedAssemblies";
            this.comboReferencedAssemblies.Size = new System.Drawing.Size(139, 21);
            this.comboReferencedAssemblies.TabIndex = 6;
            this.comboReferencedAssemblies.Text = "<< Referenced >>";
            this.comboReferencedAssemblies.SelectedIndexChanged += new System.EventHandler(this.comboReferencedAssemblies_SelectedIndexChanged);
            // 
            // chkFiles
            // 
            this.chkFiles.AutoSize = true;
            this.chkFiles.Checked = true;
            this.chkFiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFiles.Location = new System.Drawing.Point(6, 63);
            this.chkFiles.Name = "chkFiles";
            this.chkFiles.Size = new System.Drawing.Size(82, 17);
            this.chkFiles.TabIndex = 7;
            this.chkFiles.Text = "Include files";
            this.chkFiles.UseVisualStyleBackColor = true;
            this.chkFiles.CheckedChanged += new System.EventHandler(this.chkFiles_CheckedChanged);
            // 
            // chkLoaded
            // 
            this.chkLoaded.AutoSize = true;
            this.chkLoaded.Checked = true;
            this.chkLoaded.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLoaded.Location = new System.Drawing.Point(94, 31);
            this.chkLoaded.Name = "chkLoaded";
            this.chkLoaded.Size = new System.Drawing.Size(62, 17);
            this.chkLoaded.TabIndex = 8;
            this.chkLoaded.Text = "Loaded";
            this.chkLoaded.UseVisualStyleBackColor = true;
            // 
            // chkReferenced
            // 
            this.chkReferenced.AutoSize = true;
            this.chkReferenced.Checked = true;
            this.chkReferenced.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReferenced.Location = new System.Drawing.Point(332, 31);
            this.chkReferenced.Name = "chkReferenced";
            this.chkReferenced.Size = new System.Drawing.Size(82, 17);
            this.chkReferenced.TabIndex = 8;
            this.chkReferenced.Text = "Referenced";
            this.chkReferenced.UseVisualStyleBackColor = true;
            // 
            // lblAssemblyFullNameTitle
            // 
            this.lblAssemblyFullNameTitle.AutoSize = true;
            this.lblAssemblyFullNameTitle.Location = new System.Drawing.Point(3, 92);
            this.lblAssemblyFullNameTitle.Name = "lblAssemblyFullNameTitle";
            this.lblAssemblyFullNameTitle.Size = new System.Drawing.Size(55, 13);
            this.lblAssemblyFullNameTitle.TabIndex = 9;
            this.lblAssemblyFullNameTitle.Text = "Full name:";
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(64, 92);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(145, 13);
            this.lblFullName.TabIndex = 10;
            this.lblFullName.Text = "<< Assembly not selected  >>";
            // 
            // DllSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblFullName);
            this.Controls.Add(this.lblAssemblyFullNameTitle);
            this.Controls.Add(this.chkReferenced);
            this.Controls.Add(this.chkLoaded);
            this.Controls.Add(this.chkFiles);
            this.Controls.Add(this.comboReferencedAssemblies);
            this.Controls.Add(this.comboLoadedAssemblies);
            this.Controls.Add(this.txtSelectedAssembly);
            this.Controls.Add(this.lblSelectedAssembly);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.fileSelector1);
            this.MinimumSize = new System.Drawing.Size(550, 0);
            this.Name = "DllSelector";
            this.Size = new System.Drawing.Size(575, 115);
            this.Load += new System.EventHandler(this.DllSelector_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FileSelector fileSelector1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblSelectedAssembly;
        private System.Windows.Forms.TextBox txtSelectedAssembly;
        private System.Windows.Forms.ComboBox comboLoadedAssemblies;
        private System.Windows.Forms.ComboBox comboReferencedAssemblies;
        private System.Windows.Forms.CheckBox chkFiles;
        private System.Windows.Forms.CheckBox chkLoaded;
        private System.Windows.Forms.CheckBox chkReferenced;
        private System.Windows.Forms.Label lblAssemblyFullNameTitle;
        private System.Windows.Forms.Label lblFullName;
    }
}
