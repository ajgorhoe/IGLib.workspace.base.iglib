// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class ResourceSelector
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
            this.lblResources = new System.Windows.Forms.Label();
            this.chkMultipleAssemblies = new System.Windows.Forms.CheckBox();
            this.chkEmbedded = new System.Windows.Forms.CheckBox();
            this.chkResx = new System.Windows.Forms.CheckBox();
            this.comboResources = new System.Windows.Forms.ComboBox();
            this.lblSelectedResource = new System.Windows.Forms.Label();
            this.chkIncludeResxFiles = new System.Windows.Forms.CheckBox();
            this.assemblySelector1 = new IG.Forms.AssemblySelector();
            this.SuspendLayout();
            // 
            // lblResources
            // 
            this.lblResources.AutoSize = true;
            this.lblResources.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblResources.Location = new System.Drawing.Point(0, 115);
            this.lblResources.Name = "lblResources";
            this.lblResources.Size = new System.Drawing.Size(90, 17);
            this.lblResources.TabIndex = 100;
            this.lblResources.Text = "Resources:";
            // 
            // chkMultipleAssemblies
            // 
            this.chkMultipleAssemblies.AutoSize = true;
            this.chkMultipleAssemblies.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chkMultipleAssemblies.Location = new System.Drawing.Point(3, 131);
            this.chkMultipleAssemblies.Name = "chkMultipleAssemblies";
            this.chkMultipleAssemblies.Size = new System.Drawing.Size(236, 21);
            this.chkMultipleAssemblies.TabIndex = 2;
            this.chkMultipleAssemblies.Text = "Choose from multiple assemblies";
            this.chkMultipleAssemblies.UseVisualStyleBackColor = true;
            this.chkMultipleAssemblies.CheckedChanged += new System.EventHandler(this.chkMultipleAssemblies_CheckedChanged);
            // 
            // chkEmbedded
            // 
            this.chkEmbedded.AutoSize = true;
            this.chkEmbedded.Checked = true;
            this.chkEmbedded.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEmbedded.ForeColor = System.Drawing.Color.Blue;
            this.chkEmbedded.Location = new System.Drawing.Point(3, 154);
            this.chkEmbedded.Name = "chkEmbedded";
            this.chkEmbedded.Size = new System.Drawing.Size(248, 19);
            this.chkEmbedded.TabIndex = 3;
            this.chkEmbedded.Text = "Embedded resources (stand-alone files)";
            this.chkEmbedded.UseVisualStyleBackColor = true;
            this.chkEmbedded.CheckedChanged += new System.EventHandler(this.chkEmbedded_CheckedChanged);
            // 
            // chkResx
            // 
            this.chkResx.AutoSize = true;
            this.chkResx.Checked = true;
            this.chkResx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkResx.ForeColor = System.Drawing.Color.Blue;
            this.chkResx.Location = new System.Drawing.Point(3, 177);
            this.chkResx.Name = "chkResx";
            this.chkResx.Size = new System.Drawing.Size(112, 19);
            this.chkResx.TabIndex = 4;
            this.chkResx.Text = "From .resx files";
            this.chkResx.UseVisualStyleBackColor = true;
            this.chkResx.CheckedChanged += new System.EventHandler(this.chkResx_CheckedChanged);
            // 
            // comboResources
            // 
            this.comboResources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboResources.FormattingEnabled = true;
            this.comboResources.Location = new System.Drawing.Point(3, 200);
            this.comboResources.Name = "comboResources";
            this.comboResources.Size = new System.Drawing.Size(585, 21);
            this.comboResources.TabIndex = 1;
            this.comboResources.Text = "<< Select a resource! >>";
            this.comboResources.SelectedIndexChanged += new System.EventHandler(this.comboResources_SelectedIndexChanged);
            this.comboResources.Click += new System.EventHandler(this.comboResources_Click);
            // 
            // lblSelectedResource
            // 
            this.lblSelectedResource.AutoSize = true;
            this.lblSelectedResource.Location = new System.Drawing.Point(3, 224);
            this.lblSelectedResource.Name = "lblSelectedResource";
            this.lblSelectedResource.Size = new System.Drawing.Size(181, 15);
            this.lblSelectedResource.TabIndex = 7;
            this.lblSelectedResource.Text = "<< Resource is not selected.  >>";
            // 
            // chkIncludeResxFiles
            // 
            this.chkIncludeResxFiles.AutoSize = true;
            this.chkIncludeResxFiles.Location = new System.Drawing.Point(220, 154);
            this.chkIncludeResxFiles.Name = "chkIncludeResxFiles";
            this.chkIncludeResxFiles.Size = new System.Drawing.Size(145, 19);
            this.chkIncludeResxFiles.TabIndex = 101;
            this.chkIncludeResxFiles.Text = "Include resource files";
            this.chkIncludeResxFiles.UseVisualStyleBackColor = true;
            this.chkIncludeResxFiles.CheckedChanged += new System.EventHandler(this.chkIncludeResxFiles_CheckedChanged);
            // 
            // assemblySelector1
            // 
            this.assemblySelector1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.assemblySelector1.Location = new System.Drawing.Point(3, 3);
            this.assemblySelector1.MinimumSize = new System.Drawing.Size(550, 0);
            this.assemblySelector1.Name = "assemblySelector1";
            this.assemblySelector1.SelectedAssembly = null;
            this.assemblySelector1.SelectFromFiles = false;
            this.assemblySelector1.SelectFromLoadedAssemblies = true;
            this.assemblySelector1.SelectFromReferencedAssemblies = false;
            this.assemblySelector1.Size = new System.Drawing.Size(585, 109);
            this.assemblySelector1.TabIndex = 10;
            this.assemblySelector1.AssemblySelected += new System.EventHandler(this.assemblySelector1_AssemblySelected);
            this.assemblySelector1.SelectionParametersChanged += new System.EventHandler(this.assemblySelector1_SelectionParametersChanged);
            // 
            // ResourceSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.chkIncludeResxFiles);
            this.Controls.Add(this.lblSelectedResource);
            this.Controls.Add(this.comboResources);
            this.Controls.Add(this.chkResx);
            this.Controls.Add(this.chkEmbedded);
            this.Controls.Add(this.chkMultipleAssemblies);
            this.Controls.Add(this.lblResources);
            this.Controls.Add(this.assemblySelector1);
            this.MinimumSize = new System.Drawing.Size(550, 240);
            this.Name = "ResourceSelector";
            this.Size = new System.Drawing.Size(591, 244);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private AssemblySelector assemblySelector1;
        private System.Windows.Forms.Label lblResources;
        private System.Windows.Forms.CheckBox chkMultipleAssemblies;
        private System.Windows.Forms.CheckBox chkEmbedded;
        private System.Windows.Forms.CheckBox chkResx;
        private System.Windows.Forms.ComboBox comboResources;
        private System.Windows.Forms.Label lblSelectedResource;
        private System.Windows.Forms.CheckBox chkIncludeResxFiles;
    }
}
