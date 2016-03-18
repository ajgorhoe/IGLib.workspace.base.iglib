// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{ 
    partial class ControlViewerControl
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnCloseControls = new System.Windows.Forms.Button();
            this.lblControls = new System.Windows.Forms.Label();
            this.pnlControlContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnControlPosition = new System.Windows.Forms.Button();
            this.btnUnderConstruction_To_Delete = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlControls.SuspendLayout();
            this.pnlControlContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Controls.Add(this.btnUnderConstruction_To_Delete);
            this.flowLayoutPanel1.Controls.Add(this.pnlControls);
            this.flowLayoutPanel1.Controls.Add(this.pnlControlContainer);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(5, 2);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(484, 425);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // pnlControls
            // 
            this.pnlControls.BackColor = System.Drawing.SystemColors.Control;
            this.pnlControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlControls.Controls.Add(this.btnZoomOut);
            this.pnlControls.Controls.Add(this.btnZoomIn);
            this.pnlControls.Controls.Add(this.btnCloseControls);
            this.pnlControls.Controls.Add(this.lblControls);
            this.pnlControls.Location = new System.Drawing.Point(8, 37);
            this.pnlControls.Margin = new System.Windows.Forms.Padding(4);
            this.pnlControls.MinimumSize = new System.Drawing.Size(350, 70);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Padding = new System.Windows.Forms.Padding(4);
            this.pnlControls.Size = new System.Drawing.Size(466, 95);
            this.pnlControls.TabIndex = 2;
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnZoomOut.Location = new System.Drawing.Point(342, 10);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(34, 23);
            this.btnZoomOut.TabIndex = 2;
            this.btnZoomOut.Text = "-";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnZoomIn.Location = new System.Drawing.Point(382, 10);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(34, 23);
            this.btnZoomIn.TabIndex = 2;
            this.btnZoomIn.Text = "+";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnCloseControls
            // 
            this.btnCloseControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseControls.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCloseControls.ForeColor = System.Drawing.Color.Red;
            this.btnCloseControls.Location = new System.Drawing.Point(422, 7);
            this.btnCloseControls.Name = "btnCloseControls";
            this.btnCloseControls.Size = new System.Drawing.Size(35, 28);
            this.btnCloseControls.TabIndex = 1;
            this.btnCloseControls.Text = "x";
            this.btnCloseControls.UseVisualStyleBackColor = true;
            this.btnCloseControls.Click += new System.EventHandler(this.btnCloseControls_Click);
            // 
            // lblControls
            // 
            this.lblControls.AutoSize = true;
            this.lblControls.Location = new System.Drawing.Point(7, 4);
            this.lblControls.Name = "lblControls";
            this.lblControls.Size = new System.Drawing.Size(45, 13);
            this.lblControls.TabIndex = 0;
            this.lblControls.Text = "Controls";
            // 
            // pnlControlContainer
            // 
            this.pnlControlContainer.AutoSize = true;
            this.pnlControlContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlControlContainer.BackColor = System.Drawing.Color.Wheat;
            this.pnlControlContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlControlContainer.Controls.Add(this.btnTest);
            this.pnlControlContainer.Controls.Add(this.btnControlPosition);
            this.pnlControlContainer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlControlContainer.Location = new System.Drawing.Point(8, 140);
            this.pnlControlContainer.Margin = new System.Windows.Forms.Padding(4);
            this.pnlControlContainer.MinimumSize = new System.Drawing.Size(220, 60);
            this.pnlControlContainer.Name = "pnlControlContainer";
            this.pnlControlContainer.Padding = new System.Windows.Forms.Padding(4);
            this.pnlControlContainer.Size = new System.Drawing.Size(393, 275);
            this.pnlControlContainer.TabIndex = 1;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(7, 7);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test Button";
            this.btnTest.UseVisualStyleBackColor = true;
            // 
            // btnControlPosition
            // 
            this.btnControlPosition.Location = new System.Drawing.Point(7, 36);
            this.btnControlPosition.MinimumSize = new System.Drawing.Size(200, 50);
            this.btnControlPosition.Name = "btnControlPosition";
            this.btnControlPosition.Size = new System.Drawing.Size(377, 230);
            this.btnControlPosition.TabIndex = 0;
            this.btnControlPosition.Text = "Control Position (control not loaded)";
            this.btnControlPosition.UseVisualStyleBackColor = true;
            // 
            // btnUnderConstruction_To_Delete
            // 
            this.btnUnderConstruction_To_Delete.Location = new System.Drawing.Point(7, 7);
            this.btnUnderConstruction_To_Delete.Name = "btnUnderConstruction_To_Delete";
            this.btnUnderConstruction_To_Delete.Size = new System.Drawing.Size(212, 23);
            this.btnUnderConstruction_To_Delete.TabIndex = 3;
            this.btnUnderConstruction_To_Delete.Text = "This control is under construction.";
            this.btnUnderConstruction_To_Delete.UseVisualStyleBackColor = true;
            // 
            // ControlViewerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "ControlViewerControl";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(495, 433);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.pnlControlContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel pnlControlContainer;  // panel
        private System.Windows.Forms.Button btnControlPosition;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Label lblControls;
        private System.Windows.Forms.Button btnCloseControls;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnUnderConstruction_To_Delete;
    }
}
