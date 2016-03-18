// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class AssemblyInfoForm
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
            this.assemblyInfoControl1 = new IG.Forms.AssemblyInfoControl();
            this.SuspendLayout();
            // 
            // resourceViewerControl1
            // 
            this.assemblyInfoControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assemblyInfoControl1.Location = new System.Drawing.Point(0, 0);
            this.assemblyInfoControl1.Name = "resourceViewerControl1";
            this.assemblyInfoControl1.Size = new System.Drawing.Size(1168, 670);
            this.assemblyInfoControl1.TabIndex = 0;
            // 
            // ResourceViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 670);
            this.Controls.Add(this.assemblyInfoControl1);
            this.Name = "ResourceViewerForm";
            this.Text = "Application Resourced";
            this.Icon = IG.Forms.Properties.Resources.ig;
            this.ResumeLayout(false);

        }

        #endregion

        private AssemblyInfoControl assemblyInfoControl1;
    }
}