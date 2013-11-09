namespace IG.Gr3d
{
    partial class Graph3dManipulator
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
            this.btnClose = new System.Windows.Forms.Button();
            this.txtTheta = new System.Windows.Forms.TextBox();
            this.txtFi = new System.Windows.Forms.TextBox();
            this.txtR = new System.Windows.Forms.TextBox();
            this.txtViewAngle = new System.Windows.Forms.TextBox();
            this.lblTheta = new System.Windows.Forms.Label();
            this.lblFi = new System.Windows.Forms.Label();
            this.lblR = new System.Windows.Forms.Label();
            this.lblViewAngle = new System.Windows.Forms.Label();
            this.btnUpLeft = new System.Windows.Forms.Button();
            this.btnRollClockwise = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnRollCounterClockwise = new System.Windows.Forms.Button();
            this.btnUpRight = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDownRight = new System.Windows.Forms.Button();
            this.btnDownLeft = new System.Windows.Forms.Button();
            this.lblRotationStep = new System.Windows.Forms.Label();
            this.txtRotationStep = new System.Windows.Forms.TextBox();
            this.lblZoomFactor = new System.Windows.Forms.Label();
            this.txtZoomFactor = new System.Windows.Forms.TextBox();
            this.lblRoll = new System.Windows.Forms.Label();
            this.txtRoll = new System.Windows.Forms.TextBox();
            this.pnl3dManipulator = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnPositioner = new System.Windows.Forms.Button();
            this.pnl3dManipulator.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(438, 71);
            this.btnClose.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(83, 23);
            this.btnClose.TabIndex = 21;
            this.btnClose.Text = "Close Window";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtTheta
            // 
            this.txtTheta.Location = new System.Drawing.Point(340, 49);
            this.txtTheta.Name = "txtTheta";
            this.txtTheta.Size = new System.Drawing.Size(92, 20);
            this.txtTheta.TabIndex = 5;
            this.txtTheta.Click += new System.EventHandler(this.txtTheta_Enter);
            this.txtTheta.Enter += new System.EventHandler(this.txtTheta_Enter);
            this.txtTheta.Validated += new System.EventHandler(this.txtTheta_Validated);
            // 
            // txtFi
            // 
            this.txtFi.Location = new System.Drawing.Point(340, 26);
            this.txtFi.Name = "txtFi";
            this.txtFi.Size = new System.Drawing.Size(92, 20);
            this.txtFi.TabIndex = 4;
            this.txtFi.Click += new System.EventHandler(this.txtFi_Enter);
            this.txtFi.Enter += new System.EventHandler(this.txtFi_Enter);
            this.txtFi.Validated += new System.EventHandler(this.txtFi_Validated);
            // 
            // txtR
            // 
            this.txtR.Location = new System.Drawing.Point(340, 3);
            this.txtR.Name = "txtR";
            this.txtR.Size = new System.Drawing.Size(92, 20);
            this.txtR.TabIndex = 3;
            this.txtR.Click += new System.EventHandler(this.txtR_Enter);
            this.txtR.Enter += new System.EventHandler(this.txtR_Enter);
            this.txtR.Validated += new System.EventHandler(this.txtR_Validated);
            // 
            // txtViewAngle
            // 
            this.txtViewAngle.Location = new System.Drawing.Point(438, 26);
            this.txtViewAngle.Name = "txtViewAngle";
            this.txtViewAngle.Size = new System.Drawing.Size(83, 20);
            this.txtViewAngle.TabIndex = 7;
            this.txtViewAngle.Click += new System.EventHandler(this.txtViewAngle_Enter);
            this.txtViewAngle.Enter += new System.EventHandler(this.txtViewAngle_Enter);
            this.txtViewAngle.Validated += new System.EventHandler(this.txtViewAngle_Validated);
            // 
            // lblTheta
            // 
            this.lblTheta.AutoSize = true;
            this.lblTheta.Location = new System.Drawing.Point(318, 52);
            this.lblTheta.Name = "lblTheta";
            this.lblTheta.Size = new System.Drawing.Size(16, 13);
            this.lblTheta.TabIndex = 16;
            this.lblTheta.Text = "θ:";
            // 
            // lblFi
            // 
            this.lblFi.AutoSize = true;
            this.lblFi.Location = new System.Drawing.Point(316, 29);
            this.lblFi.Name = "lblFi";
            this.lblFi.Size = new System.Drawing.Size(18, 13);
            this.lblFi.TabIndex = 17;
            this.lblFi.Text = "φ:";
            // 
            // lblR
            // 
            this.lblR.AutoSize = true;
            this.lblR.Location = new System.Drawing.Point(321, 6);
            this.lblR.Name = "lblR";
            this.lblR.Size = new System.Drawing.Size(13, 13);
            this.lblR.TabIndex = 19;
            this.lblR.Text = "r:";
            // 
            // lblViewAngle
            // 
            this.lblViewAngle.AutoSize = true;
            this.lblViewAngle.Location = new System.Drawing.Point(435, 6);
            this.lblViewAngle.Name = "lblViewAngle";
            this.lblViewAngle.Size = new System.Drawing.Size(62, 13);
            this.lblViewAngle.TabIndex = 18;
            this.lblViewAngle.Text = "View angle:";
            // 
            // btnUpLeft
            // 
            this.btnUpLeft.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpLeft.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnUpLeft.Location = new System.Drawing.Point(3, 3);
            this.btnUpLeft.Name = "btnUpLeft";
            this.btnUpLeft.Size = new System.Drawing.Size(34, 23);
            this.btnUpLeft.TabIndex = 8;
            this.btnUpLeft.Text = "⇖";
            this.btnUpLeft.UseVisualStyleBackColor = true;
            this.btnUpLeft.Click += new System.EventHandler(this.btnUpLeft_Click);
            // 
            // btnRollClockwise
            // 
            this.btnRollClockwise.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRollClockwise.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnRollClockwise.Location = new System.Drawing.Point(123, 26);
            this.btnRollClockwise.Name = "btnRollClockwise";
            this.btnRollClockwise.Size = new System.Drawing.Size(39, 23);
            this.btnRollClockwise.TabIndex = 17;
            this.btnRollClockwise.Text = "↻";
            this.btnRollClockwise.UseVisualStyleBackColor = true;
            this.btnRollClockwise.Click += new System.EventHandler(this.btnRollClockwise_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZoomOut.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnZoomOut.Location = new System.Drawing.Point(168, 26);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(39, 23);
            this.btnZoomOut.TabIndex = 19;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnZoomIn.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnZoomIn.Location = new System.Drawing.Point(168, 3);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(39, 23);
            this.btnZoomIn.TabIndex = 18;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnRollCounterClockwise
            // 
            this.btnRollCounterClockwise.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRollCounterClockwise.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnRollCounterClockwise.Location = new System.Drawing.Point(123, 3);
            this.btnRollCounterClockwise.Name = "btnRollCounterClockwise";
            this.btnRollCounterClockwise.Size = new System.Drawing.Size(39, 23);
            this.btnRollCounterClockwise.TabIndex = 16;
            this.btnRollCounterClockwise.Text = "↺";
            this.btnRollCounterClockwise.UseVisualStyleBackColor = true;
            this.btnRollCounterClockwise.Click += new System.EventHandler(this.btnRollCounterClockwise_Click);
            // 
            // btnUpRight
            // 
            this.btnUpRight.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpRight.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnUpRight.Location = new System.Drawing.Point(83, 3);
            this.btnUpRight.Name = "btnUpRight";
            this.btnUpRight.Size = new System.Drawing.Size(34, 23);
            this.btnUpRight.TabIndex = 10;
            this.btnUpRight.Text = "⇗";
            this.btnUpRight.UseVisualStyleBackColor = true;
            this.btnUpRight.Click += new System.EventHandler(this.btnUpRight_Click);
            // 
            // btnDown
            // 
            this.btnDown.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDown.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnDown.Location = new System.Drawing.Point(43, 49);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(34, 23);
            this.btnDown.TabIndex = 14;
            this.btnDown.Text = "⇓";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnUp.Location = new System.Drawing.Point(43, 3);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(34, 23);
            this.btnUp.TabIndex = 9;
            this.btnUp.Text = "⇑";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnRight
            // 
            this.btnRight.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRight.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnRight.Location = new System.Drawing.Point(60, 26);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(36, 23);
            this.btnRight.TabIndex = 12;
            this.btnRight.Text = "⇒";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLeft.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnLeft.Location = new System.Drawing.Point(25, 26);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(36, 23);
            this.btnLeft.TabIndex = 11;
            this.btnLeft.Text = "⇐";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnDownRight
            // 
            this.btnDownRight.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownRight.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnDownRight.Location = new System.Drawing.Point(83, 49);
            this.btnDownRight.Name = "btnDownRight";
            this.btnDownRight.Size = new System.Drawing.Size(34, 23);
            this.btnDownRight.TabIndex = 15;
            this.btnDownRight.Text = "⇘";
            this.btnDownRight.UseVisualStyleBackColor = true;
            this.btnDownRight.Click += new System.EventHandler(this.btnDownRight_Click);
            // 
            // btnDownLeft
            // 
            this.btnDownLeft.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownLeft.ForeColor = System.Drawing.Color.RoyalBlue;
            this.btnDownLeft.Location = new System.Drawing.Point(3, 49);
            this.btnDownLeft.Name = "btnDownLeft";
            this.btnDownLeft.Size = new System.Drawing.Size(34, 23);
            this.btnDownLeft.TabIndex = 13;
            this.btnDownLeft.Text = "⇙";
            this.btnDownLeft.UseVisualStyleBackColor = true;
            this.btnDownLeft.Click += new System.EventHandler(this.btnDownLeft_Click);
            // 
            // lblRotationStep
            // 
            this.lblRotationStep.AutoSize = true;
            this.lblRotationStep.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblRotationStep.Location = new System.Drawing.Point(219, 6);
            this.lblRotationStep.Name = "lblRotationStep";
            this.lblRotationStep.Size = new System.Drawing.Size(73, 13);
            this.lblRotationStep.TabIndex = 18;
            this.lblRotationStep.Text = "Rotation step:";
            // 
            // txtRotationStep
            // 
            this.txtRotationStep.Location = new System.Drawing.Point(222, 22);
            this.txtRotationStep.Name = "txtRotationStep";
            this.txtRotationStep.Size = new System.Drawing.Size(70, 20);
            this.txtRotationStep.TabIndex = 1;
            this.txtRotationStep.Click += new System.EventHandler(this.txtRotationStep_Enter);
            this.txtRotationStep.Enter += new System.EventHandler(this.txtRotationStep_Enter);
            this.txtRotationStep.Validated += new System.EventHandler(this.txtRotationStep_Validated);
            // 
            // lblZoomFactor
            // 
            this.lblZoomFactor.AutoSize = true;
            this.lblZoomFactor.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblZoomFactor.Location = new System.Drawing.Point(219, 45);
            this.lblZoomFactor.Name = "lblZoomFactor";
            this.lblZoomFactor.Size = new System.Drawing.Size(67, 13);
            this.lblZoomFactor.TabIndex = 18;
            this.lblZoomFactor.Text = "Zoom factor:";
            // 
            // txtZoomFactor
            // 
            this.txtZoomFactor.Location = new System.Drawing.Point(222, 61);
            this.txtZoomFactor.Name = "txtZoomFactor";
            this.txtZoomFactor.Size = new System.Drawing.Size(70, 20);
            this.txtZoomFactor.TabIndex = 2;
            this.txtZoomFactor.Click += new System.EventHandler(this.txtZoomFactor_Enter);
            this.txtZoomFactor.Enter += new System.EventHandler(this.txtZoomFactor_Enter);
            this.txtZoomFactor.Validated += new System.EventHandler(this.txtZoomFactor_Validated);
            // 
            // lblRoll
            // 
            this.lblRoll.AutoSize = true;
            this.lblRoll.Location = new System.Drawing.Point(306, 74);
            this.lblRoll.Name = "lblRoll";
            this.lblRoll.Size = new System.Drawing.Size(28, 13);
            this.lblRoll.TabIndex = 18;
            this.lblRoll.Text = "Roll:";
            // 
            // txtRoll
            // 
            this.txtRoll.Location = new System.Drawing.Point(340, 71);
            this.txtRoll.Name = "txtRoll";
            this.txtRoll.Size = new System.Drawing.Size(92, 20);
            this.txtRoll.TabIndex = 6;
            this.txtRoll.Click += new System.EventHandler(this.txtRoll_Enter);
            this.txtRoll.Enter += new System.EventHandler(this.txtRoll_Enter);
            this.txtRoll.Validated += new System.EventHandler(this.txtRoll_Validated);
            // 
            // pnl3dManipulator
            // 
            this.pnl3dManipulator.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl3dManipulator.Controls.Add(this.btnPositioner);
            this.pnl3dManipulator.Controls.Add(this.btnLeft);
            this.pnl3dManipulator.Controls.Add(this.txtTheta);
            this.pnl3dManipulator.Controls.Add(this.btnRefresh);
            this.pnl3dManipulator.Controls.Add(this.btnClose);
            this.pnl3dManipulator.Controls.Add(this.txtFi);
            this.pnl3dManipulator.Controls.Add(this.btnDownLeft);
            this.pnl3dManipulator.Controls.Add(this.txtR);
            this.pnl3dManipulator.Controls.Add(this.btnDownRight);
            this.pnl3dManipulator.Controls.Add(this.txtZoomFactor);
            this.pnl3dManipulator.Controls.Add(this.btnRight);
            this.pnl3dManipulator.Controls.Add(this.txtRotationStep);
            this.pnl3dManipulator.Controls.Add(this.btnUp);
            this.pnl3dManipulator.Controls.Add(this.txtRoll);
            this.pnl3dManipulator.Controls.Add(this.btnDown);
            this.pnl3dManipulator.Controls.Add(this.txtViewAngle);
            this.pnl3dManipulator.Controls.Add(this.btnUpRight);
            this.pnl3dManipulator.Controls.Add(this.lblTheta);
            this.pnl3dManipulator.Controls.Add(this.btnRollCounterClockwise);
            this.pnl3dManipulator.Controls.Add(this.lblZoomFactor);
            this.pnl3dManipulator.Controls.Add(this.btnZoomIn);
            this.pnl3dManipulator.Controls.Add(this.lblFi);
            this.pnl3dManipulator.Controls.Add(this.btnZoomOut);
            this.pnl3dManipulator.Controls.Add(this.lblRotationStep);
            this.pnl3dManipulator.Controls.Add(this.btnRollClockwise);
            this.pnl3dManipulator.Controls.Add(this.lblR);
            this.pnl3dManipulator.Controls.Add(this.btnUpLeft);
            this.pnl3dManipulator.Controls.Add(this.lblRoll);
            this.pnl3dManipulator.Controls.Add(this.lblViewAngle);
            this.pnl3dManipulator.Location = new System.Drawing.Point(0, 0);
            this.pnl3dManipulator.MinimumSize = new System.Drawing.Size(524, 97);
            this.pnl3dManipulator.Name = "pnl3dManipulator";
            this.pnl3dManipulator.Size = new System.Drawing.Size(524, 97);
            this.pnl3dManipulator.TabIndex = 24;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(123, 50);
            this.btnRefresh.MinimumSize = new System.Drawing.Size(75, 23);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(84, 23);
            this.btnRefresh.TabIndex = 20;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnPositioner
            // 
            this.btnPositioner.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnPositioner.Location = new System.Drawing.Point(438, 47);
            this.btnPositioner.Name = "btnPositioner";
            this.btnPositioner.Size = new System.Drawing.Size(31, 18);
            this.btnPositioner.TabIndex = 22;
            this.btnPositioner.Text = "❏⇔ ";
            this.btnPositioner.UseVisualStyleBackColor = true;
            this.btnPositioner.Click += new System.EventHandler(this.btnPositioner_Click);
            // 
            // Graph3dManipulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnl3dManipulator);
            this.MinimumSize = new System.Drawing.Size(524, 98);
            this.Name = "Graph3dManipulator";
            this.Size = new System.Drawing.Size(524, 98);
            this.Load += new System.EventHandler(this.Graph3dManipulator_Load);
            this.pnl3dManipulator.ResumeLayout(false);
            this.pnl3dManipulator.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtTheta;
        private System.Windows.Forms.TextBox txtFi;
        private System.Windows.Forms.TextBox txtR;
        private System.Windows.Forms.TextBox txtViewAngle;
        private System.Windows.Forms.Label lblTheta;
        private System.Windows.Forms.Label lblFi;
        private System.Windows.Forms.Label lblR;
        private System.Windows.Forms.Label lblViewAngle;
        private System.Windows.Forms.Button btnUpLeft;
        private System.Windows.Forms.Button btnRollClockwise;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnRollCounterClockwise;
        private System.Windows.Forms.Button btnUpRight;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDownRight;
        private System.Windows.Forms.Button btnDownLeft;
        private System.Windows.Forms.Label lblRotationStep;
        private System.Windows.Forms.TextBox txtRotationStep;
        private System.Windows.Forms.Label lblZoomFactor;
        private System.Windows.Forms.TextBox txtZoomFactor;
        private System.Windows.Forms.Label lblRoll;
        private System.Windows.Forms.TextBox txtRoll;
        private System.Windows.Forms.Panel pnl3dManipulator;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnPositioner;
    }
}
