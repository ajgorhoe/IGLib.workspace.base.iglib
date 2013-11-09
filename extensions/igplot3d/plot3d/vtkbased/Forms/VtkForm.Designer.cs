// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Gr3d
{
    partial class VtkForm
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
            this.vtkControlWin1 = new IG.Gr3d.VtkControlWin();
            this.SuspendLayout();
            // 
            // vtkControlWin1
            // 
            this.vtkControlWin1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vtkControlWin1.Location = new System.Drawing.Point(0, 0);
            this.vtkControlWin1.Name = "vtkControlWin1";
            this.vtkControlWin1.Size = new System.Drawing.Size(730, 571);
            this.vtkControlWin1.TabIndex = 0;
            // 
            // VtkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 571);
            this.Controls.Add(this.vtkControlWin1);
            this.Name = "VtkForm";
            this.Text = "3D Graphics (IGLib, based on VTK / Activiz)";
            this.ResumeLayout(false);

        }

        #endregion

        private VtkControlWin vtkControlWin1;

    }
}