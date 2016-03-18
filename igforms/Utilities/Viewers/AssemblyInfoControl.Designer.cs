// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class AssemblyInfoControl
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
            this.assemblySelector1 = new IG.Forms.AssemblySelector();
            this.lblResources = new System.Windows.Forms.Label();
            this.chkMultipleAssemblies = new System.Windows.Forms.CheckBox();
            this.btnRefreshOutput = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
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
            this.assemblySelector1.SelectFromLoadedAssemblies = false;
            this.assemblySelector1.SelectFromReferencedAssemblies = false;
            this.assemblySelector1.Size = new System.Drawing.Size(657, 113);
            this.assemblySelector1.TabIndex = 0;
            this.assemblySelector1.AssemblySelected += new System.EventHandler(this.assemblySelector1_AssemblySelected);
            this.assemblySelector1.SelectionParametersChanged += new System.EventHandler(this.assemblySelector1_SelectionParametersChanged);
            // 
            // lblResources
            // 
            this.lblResources.AutoSize = true;
            this.lblResources.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblResources.Location = new System.Drawing.Point(0, 115);
            this.lblResources.Name = "lblResources";
            this.lblResources.Size = new System.Drawing.Size(130, 13);
            this.lblResources.TabIndex = 100;
            this.lblResources.Text = "Assembly Information:";
            // 
            // chkMultipleAssemblies
            // 
            this.chkMultipleAssemblies.AutoSize = true;
            this.chkMultipleAssemblies.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chkMultipleAssemblies.Location = new System.Drawing.Point(3, 131);
            this.chkMultipleAssemblies.Name = "chkMultipleAssemblies";
            this.chkMultipleAssemblies.Size = new System.Drawing.Size(148, 17);
            this.chkMultipleAssemblies.TabIndex = 1;
            this.chkMultipleAssemblies.Text = "Select multiple assemblies";
            this.chkMultipleAssemblies.UseVisualStyleBackColor = true;
            this.chkMultipleAssemblies.CheckedChanged += new System.EventHandler(this.chkMultipleAssemblies_CheckedChanged);
            // 
            // btnRefreshOutput
            // 
            this.btnRefreshOutput.Location = new System.Drawing.Point(157, 127);
            this.btnRefreshOutput.Name = "btnRefreshOutput";
            this.btnRefreshOutput.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshOutput.TabIndex = 2;
            this.btnRefreshOutput.Text = "Refresh";
            this.btnRefreshOutput.UseVisualStyleBackColor = true;
            this.btnRefreshOutput.Click += new System.EventHandler(this.btnRefreshOutput_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(238, 127);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.AllowDrop = true;
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(3, 156);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(657, 245);
            this.txtOutput.TabIndex = 100;
            this.txtOutput.TabStop = false;
            this.txtOutput.Text = "<< Select an assembly to view its information! >>\r\n<< Press \"Refresh\" if necessar" +
    "y. >>";
            this.txtOutput.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtOutput_DragDrop);
            this.txtOutput.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtOutput_DragEnter);
            // 
            // AssemblyInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnRefreshOutput);
            this.Controls.Add(this.chkMultipleAssemblies);
            this.Controls.Add(this.lblResources);
            this.Controls.Add(this.assemblySelector1);
            this.MinimumSize = new System.Drawing.Size(550, 240);
            this.Name = "AssemblyInfoControl";
            this.Size = new System.Drawing.Size(663, 404);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private AssemblySelector assemblySelector1;
        private System.Windows.Forms.Label lblResources;
        private System.Windows.Forms.CheckBox chkMultipleAssemblies;
        private System.Windows.Forms.Button btnRefreshOutput;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtOutput;
    }
}
