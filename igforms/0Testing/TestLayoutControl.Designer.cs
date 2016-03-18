namespace IG.Forms
{
    partial class TestLayoutControl
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
            this.btnTestLayoutControl = new System.Windows.Forms.Button();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnControls1 = new System.Windows.Forms.Button();
            this.btnControls2 = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.pnlOuter = new System.Windows.Forms.Panel();
            this.btnPnlOuter = new System.Windows.Forms.Button();
            this.brnPnlControls = new System.Windows.Forms.Button();
            this.pnlBtnOutepPanel = new System.Windows.Forms.Panel();
            this.pnlControls.SuspendLayout();
            this.pnlOuter.SuspendLayout();
            this.pnlBtnOutepPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTestLayoutControl
            // 
            this.btnTestLayoutControl.Location = new System.Drawing.Point(12, 3);
            this.btnTestLayoutControl.Name = "btnTestLayoutControl";
            this.btnTestLayoutControl.Size = new System.Drawing.Size(128, 23);
            this.btnTestLayoutControl.TabIndex = 0;
            this.btnTestLayoutControl.Text = "TestLayoutComtrol";
            this.btnTestLayoutControl.UseVisualStyleBackColor = true;
            // 
            // pnlControls
            // 
            this.pnlControls.AutoSize = true;
            this.pnlControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlControls.Controls.Add(this.btnZoomOut);
            this.pnlControls.Controls.Add(this.btnZoomIn);
            this.pnlControls.Controls.Add(this.btnControls2);
            this.pnlControls.Controls.Add(this.brnPnlControls);
            this.pnlControls.Controls.Add(this.btnControls1);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(737, 70);
            this.pnlControls.TabIndex = 1;
            // 
            // btnControls1
            // 
            this.btnControls1.Location = new System.Drawing.Point(3, 3);
            this.btnControls1.Name = "btnControls1";
            this.btnControls1.Size = new System.Drawing.Size(75, 23);
            this.btnControls1.TabIndex = 0;
            this.btnControls1.Text = "Controls 1";
            this.btnControls1.UseVisualStyleBackColor = true;
            // 
            // btnControls2
            // 
            this.btnControls2.Location = new System.Drawing.Point(3, 32);
            this.btnControls2.Name = "btnControls2";
            this.btnControls2.Size = new System.Drawing.Size(116, 35);
            this.btnControls2.TabIndex = 0;
            this.btnControls2.Text = "Controls 2";
            this.btnControls2.UseVisualStyleBackColor = true;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomIn.Location = new System.Drawing.Point(659, 3);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(75, 23);
            this.btnZoomIn.TabIndex = 1;
            this.btnZoomIn.Text = "Zoom In (+)";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoomOut.Location = new System.Drawing.Point(578, 3);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(75, 23);
            this.btnZoomOut.TabIndex = 1;
            this.btnZoomOut.Text = "Zoom Out (-)";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            // 
            // pnlOuter
            // 
            this.pnlOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOuter.AutoScroll = true;
            this.pnlOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOuter.Controls.Add(this.pnlBtnOutepPanel);
            this.pnlOuter.Controls.Add(this.pnlControls);
            this.pnlOuter.Location = new System.Drawing.Point(12, 32);
            this.pnlOuter.Name = "pnlOuter";
            this.pnlOuter.Size = new System.Drawing.Size(737, 387);
            this.pnlOuter.TabIndex = 2;
            this.pnlOuter.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlOuter_Paint);
            // 
            // btnPnlOuter
            // 
            this.btnPnlOuter.Location = new System.Drawing.Point(3, 3);
            this.btnPnlOuter.Name = "btnPnlOuter";
            this.btnPnlOuter.Size = new System.Drawing.Size(93, 23);
            this.btnPnlOuter.TabIndex = 2;
            this.btnPnlOuter.Text = "Outer Panel";
            this.btnPnlOuter.UseVisualStyleBackColor = true;
            // 
            // brnPnlControls
            // 
            this.brnPnlControls.Location = new System.Drawing.Point(84, 3);
            this.brnPnlControls.Name = "brnPnlControls";
            this.brnPnlControls.Size = new System.Drawing.Size(116, 23);
            this.brnPnlControls.TabIndex = 0;
            this.brnPnlControls.Text = "Controls panel";
            this.brnPnlControls.UseVisualStyleBackColor = true;
            // 
            // pnlBtnOutepPanel
            // 
            this.pnlBtnOutepPanel.AutoSize = true;
            this.pnlBtnOutepPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlBtnOutepPanel.Controls.Add(this.btnPnlOuter);
            this.pnlBtnOutepPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBtnOutepPanel.Location = new System.Drawing.Point(0, 70);
            this.pnlBtnOutepPanel.Name = "pnlBtnOutepPanel";
            this.pnlBtnOutepPanel.Size = new System.Drawing.Size(737, 29);
            this.pnlBtnOutepPanel.TabIndex = 3;
            this.pnlBtnOutepPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBtnOutepPanel_Paint);
            // 
            // TestLayoutControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnTestLayoutControl);
            this.Controls.Add(this.pnlOuter);
            this.MinimumSize = new System.Drawing.Size(20, 20);
            this.Name = "TestLayoutControl";
            this.Size = new System.Drawing.Size(790, 468);
            this.pnlControls.ResumeLayout(false);
            this.pnlOuter.ResumeLayout(false);
            this.pnlOuter.PerformLayout();
            this.pnlBtnOutepPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTestLayoutControl;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnControls2;
        private System.Windows.Forms.Button btnControls1;
        private System.Windows.Forms.Panel pnlOuter;
        private System.Windows.Forms.Button btnPnlOuter;
        private System.Windows.Forms.Button brnPnlControls;
        private System.Windows.Forms.Panel pnlBtnOutepPanel;
    }
}
