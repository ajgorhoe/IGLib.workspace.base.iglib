namespace IG.Gr3d
{
    partial class Graph3dManipulatorBasic
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
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnManipulate = new System.Windows.Forms.Button();
            this.btnResetCamera = new System.Windows.Forms.Button();
            this.btnTrackBallJoystick = new System.Windows.Forms.Button();
            this.btnCameraActor = new System.Windows.Forms.Button();
            this.btnSurfaceWireframe = new System.Windows.Forms.Button();
            this.btnCloseWin = new System.Windows.Forms.Button();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlControls.Controls.Add(this.btnManipulate);
            this.pnlControls.Controls.Add(this.btnResetCamera);
            this.pnlControls.Controls.Add(this.btnTrackBallJoystick);
            this.pnlControls.Controls.Add(this.btnCameraActor);
            this.pnlControls.Controls.Add(this.btnSurfaceWireframe);
            this.pnlControls.Controls.Add(this.btnCloseWin);
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.MinimumSize = new System.Drawing.Size(670, 30);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(670, 30);
            this.pnlControls.TabIndex = 3;
            // 
            // btnManipulate
            // 
            this.btnManipulate.ForeColor = System.Drawing.Color.Blue;
            this.btnManipulate.Location = new System.Drawing.Point(435, 4);
            this.btnManipulate.Name = "btnManipulate";
            this.btnManipulate.Size = new System.Drawing.Size(102, 23);
            this.btnManipulate.TabIndex = 5;
            this.btnManipulate.Text = " ⇗ ↺  Manipulate";
            this.btnManipulate.UseVisualStyleBackColor = true;
            this.btnManipulate.Click += new System.EventHandler(this.btnManipulate_Click);
            // 
            // btnResetCamera
            // 
            this.btnResetCamera.ForeColor = System.Drawing.Color.Blue;
            this.btnResetCamera.Location = new System.Drawing.Point(3, 4);
            this.btnResetCamera.Name = "btnResetCamera";
            this.btnResetCamera.Size = new System.Drawing.Size(102, 23);
            this.btnResetCamera.TabIndex = 1;
            this.btnResetCamera.Text = "Reset Camera (R)";
            this.btnResetCamera.UseVisualStyleBackColor = true;
            this.btnResetCamera.Click += new System.EventHandler(this.btnResetCamera_Click);
            // 
            // btnTrackBallJoystick
            // 
            this.btnTrackBallJoystick.Location = new System.Drawing.Point(111, 4);
            this.btnTrackBallJoystick.Name = "btnTrackBallJoystick";
            this.btnTrackBallJoystick.Size = new System.Drawing.Size(102, 23);
            this.btnTrackBallJoystick.TabIndex = 2;
            this.btnTrackBallJoystick.Text = "Trackball Mode";
            this.btnTrackBallJoystick.UseVisualStyleBackColor = true;
            this.btnTrackBallJoystick.Click += new System.EventHandler(this.btnTrackBallJoystick_Click);
            // 
            // btnCameraActor
            // 
            this.btnCameraActor.Location = new System.Drawing.Point(219, 4);
            this.btnCameraActor.Name = "btnCameraActor";
            this.btnCameraActor.Size = new System.Drawing.Size(102, 23);
            this.btnCameraActor.TabIndex = 3;
            this.btnCameraActor.Text = "Actor Mode";
            this.btnCameraActor.UseVisualStyleBackColor = true;
            this.btnCameraActor.Click += new System.EventHandler(this.btnCameraActor_Click);
            // 
            // btnSurfaceWireframe
            // 
            this.btnSurfaceWireframe.Location = new System.Drawing.Point(327, 4);
            this.btnSurfaceWireframe.Name = "btnSurfaceWireframe";
            this.btnSurfaceWireframe.Size = new System.Drawing.Size(102, 23);
            this.btnSurfaceWireframe.TabIndex = 4;
            this.btnSurfaceWireframe.Text = "Wireframe";
            this.btnSurfaceWireframe.UseVisualStyleBackColor = true;
            this.btnSurfaceWireframe.Click += new System.EventHandler(this.btnSurfaceWireframe_Click);
            // 
            // btnCloseWin
            // 
            this.btnCloseWin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseWin.ForeColor = System.Drawing.Color.Brown;
            this.btnCloseWin.Location = new System.Drawing.Point(565, 4);
            this.btnCloseWin.Name = "btnCloseWin";
            this.btnCloseWin.Size = new System.Drawing.Size(102, 23);
            this.btnCloseWin.TabIndex = 6;
            this.btnCloseWin.Text = "x Close Window";
            this.btnCloseWin.UseVisualStyleBackColor = true;
            this.btnCloseWin.Click += new System.EventHandler(this.btnCloseWin_Click);
            // 
            // Graph3dManipulatorBasic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlControls);
            this.MinimumSize = new System.Drawing.Size(670, 30);
            this.Name = "Graph3dManipulatorBasic";
            this.Size = new System.Drawing.Size(670, 30);
            this.pnlControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Button btnResetCamera;
        private System.Windows.Forms.Button btnTrackBallJoystick;
        private System.Windows.Forms.Button btnCameraActor;
        private System.Windows.Forms.Button btnSurfaceWireframe;
        private System.Windows.Forms.Button btnCloseWin;
        private System.Windows.Forms.Button btnManipulate;
    }
}
