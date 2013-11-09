// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Gr3d
{
    partial class VtkFormPlain
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
            this.vtkControlWin1 = new IG.Gr3d.VtkControlBase();
            this.btnManipulatorControls = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // vtkControlWin1
            // 
            this.vtkControlWin1.CameraRoll = 0D;
            this.vtkControlWin1.CameraViewAngle = 30D;
            this.vtkControlWin1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vtkControlWin1.IsActorsMode = false;
            this.vtkControlWin1.IsCameraMode = true;
            this.vtkControlWin1.IsJoystickMode = false;
            this.vtkControlWin1.IsSurfaceMode = false;
            this.vtkControlWin1.IsTrackballMode = true;
            this.vtkControlWin1.IsWireframeMode = false;
            this.vtkControlWin1.Location = new System.Drawing.Point(0, 0);
            this.vtkControlWin1.Name = "vtkControlWin1";
            this.vtkControlWin1.RotationStep = 2D;
            this.vtkControlWin1.Size = new System.Drawing.Size(709, 564);
            this.vtkControlWin1.TabIndex = 0;
            this.vtkControlWin1.VtkAddTestActors = true;
            this.vtkControlWin1.VtkAddTestActorsIGLib = true;
            this.vtkControlWin1.ZoomFactor = 1.2D;
            // 
            // btnControls
            // 
            this.btnManipulatorControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManipulatorControls.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnManipulatorControls.ForeColor = System.Drawing.Color.Blue;
            this.btnManipulatorControls.Location = new System.Drawing.Point(671, 542);
            this.btnManipulatorControls.Name = "btnControls";
            this.btnManipulatorControls.Size = new System.Drawing.Size(38, 22);
            this.btnManipulatorControls.TabIndex = 1;
            this.btnManipulatorControls.Text = "⇗ ↺";
            this.btnManipulatorControls.UseVisualStyleBackColor = true;
            this.btnManipulatorControls.Click += new System.EventHandler(this.btnManipulatorControls_Click);
            // 
            // VtkFormPlain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 564);
            this.Controls.Add(this.btnManipulatorControls);
            this.Controls.Add(this.vtkControlWin1);
            this.Name = "VtkFormPlain";
            this.Text = "3D Graphics (IGLib, based on VTK / Activiz)";
            this.ResumeLayout(false);

        }

        #endregion

        private VtkControlBase vtkControlWin1;
        private System.Windows.Forms.Button btnManipulatorControls; // System.Windows.Forms.Panel vtkControlWin1; // 
    }
}