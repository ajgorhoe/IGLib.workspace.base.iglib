// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Gr3d
{
    partial class VtkControlWin
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
            this.vtkControlBase1 = new IG.Gr3d.VtkControlBase();
            this.graph3dManipulatorBasic1 = new IG.Gr3d.Graph3dManipulatorBasic();
            this.SuspendLayout();
            // 
            // vtkControlBase1
            // 
            this.vtkControlBase1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vtkControlBase1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.vtkControlBase1.CameraRoll = 0D;
            this.vtkControlBase1.CameraViewAngle = 30D;
            this.vtkControlBase1.IsActorsMode = false;
            this.vtkControlBase1.IsCameraMode = true;
            this.vtkControlBase1.IsJoystickMode = false;
            this.vtkControlBase1.IsSurfaceMode = false;
            this.vtkControlBase1.IsTrackballMode = true;
            this.vtkControlBase1.IsWireframeMode = false;
            this.vtkControlBase1.Location = new System.Drawing.Point(3, 3);
            this.vtkControlBase1.Name = "vtkControlBase1";
            this.vtkControlBase1.RotationStep = 2D;
            this.vtkControlBase1.Size = new System.Drawing.Size(699, 524);
            this.vtkControlBase1.TabIndex = 0;
            this.vtkControlBase1.TabStop = false;
            this.vtkControlBase1.VtkAddTestActors = true;
            this.vtkControlBase1.VtkAddTestActorsIGLib = true;
            this.vtkControlBase1.ZoomFactor = 1.2D;
            // 
            // graph3dManipulatorBasic1
            // 
            this.graph3dManipulatorBasic1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph3dManipulatorBasic1.IsCloseWindoButtonVisible = true;
            this.graph3dManipulatorBasic1.IsManipulateButtonVisible = true;
            this.graph3dManipulatorBasic1.Location = new System.Drawing.Point(3, 524);
            this.graph3dManipulatorBasic1.MinimumSize = new System.Drawing.Size(670, 30);
            this.graph3dManipulatorBasic1.Name = "graph3dManipulatorBasic1";
            this.graph3dManipulatorBasic1.Size = new System.Drawing.Size(699, 30);
            this.graph3dManipulatorBasic1.TabIndex = 1;
            this.graph3dManipulatorBasic1.VtkContainer = null;
            this.graph3dManipulatorBasic1.VtkControl = null;
            // 
            // VtkControlWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graph3dManipulatorBasic1);
            this.Controls.Add(this.vtkControlBase1);
            this.Name = "VtkControlWin";
            this.Size = new System.Drawing.Size(705, 557);
            this.ResumeLayout(false);

        }

        #endregion

        private IG.Gr3d.VtkControlBase vtkControlBase1; // System.Windows.Forms.Panel vtkControlBase1; //
        private Graph3dManipulatorBasic graph3dManipulatorBasic1;


    }
}
