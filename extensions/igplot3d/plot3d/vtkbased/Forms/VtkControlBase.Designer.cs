// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Gr3d
{
    partial class VtkControlBase
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


                    
        // this.vtkReplacementPanel = new System.Windows.Forms.Panel();
        // this.vtkReplacementPanel = new Kitware.VTK.RenderWindowControl();


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vtkReplacementPanel = new System.Windows.Forms.Panel();
            this.txtReplacementNotification = new System.Windows.Forms.TextBox();
            this.vtkReplacementPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // vtkReplacementPanel
            // 
            this.vtkReplacementPanel.BackColor = System.Drawing.Color.PaleTurquoise;
            this.vtkReplacementPanel.Controls.Add(this.txtReplacementNotification);
            this.vtkReplacementPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vtkReplacementPanel.Location = new System.Drawing.Point(0, 0);
            this.vtkReplacementPanel.Name = "vtkReplacementPanel";
            this.vtkReplacementPanel.Size = new System.Drawing.Size(400, 408);
            this.vtkReplacementPanel.TabIndex = 0;
            // 
            // txtReplacementNotification
            // 
            this.txtReplacementNotification.Location = new System.Drawing.Point(14, 12);
            this.txtReplacementNotification.Name = "txtReplacementNotification";
            this.txtReplacementNotification.Size = new System.Drawing.Size(242, 20);
            this.txtReplacementNotification.TabIndex = 0;
            this.txtReplacementNotification.Text = "This is a VTK window replacement panel.";
            // 
            // VtkControlBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vtkReplacementPanel);
            this.Name = "VtkControlBase";
            this.Size = new System.Drawing.Size(400, 408);
            this.Load += new System.EventHandler(this.VtkControlBase_Load);
            //this.LoadVtkGraphics += new System.EventHandler(this.VtkControlBase_LoadVtkGraphics);
            this.vtkReplacementPanel.ResumeLayout(false);
            this.vtkReplacementPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel vtkReplacementPanel;
        // private Kitware.VTK.RenderWindowControl vtkReplacementPanel;
        private System.Windows.Forms.TextBox txtReplacementNotification;
    }
}
