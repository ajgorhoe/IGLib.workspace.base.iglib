namespace IG.Gr3d
{
    partial class Graph3dManipulatorExtended
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
            this.graph3dManipulatorBasic1 = new IG.Gr3d.Graph3dManipulatorBasic();
            this.graph3dManipulator1 = new IG.Gr3d.Graph3dManipulator();
            this.SuspendLayout();
            // 
            // graph3dManipulatorBasic1
            // 
            this.graph3dManipulatorBasic1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph3dManipulatorBasic1.IsCloseWindoButtonVisible = true;
            this.graph3dManipulatorBasic1.IsManipulateButtonVisible = true;
            this.graph3dManipulatorBasic1.Location = new System.Drawing.Point(0, 100);
            this.graph3dManipulatorBasic1.MinimumSize = new System.Drawing.Size(676, 36);
            this.graph3dManipulatorBasic1.Name = "graph3dManipulatorBasic1";
            this.graph3dManipulatorBasic1.Size = new System.Drawing.Size(676, 36);
            this.graph3dManipulatorBasic1.TabIndex = 2;
            this.graph3dManipulatorBasic1.VtkContainer = null;
            this.graph3dManipulatorBasic1.VtkControl = null;
            // 
            // graph3dManipulator1
            // 
            this.graph3dManipulator1.IsCloseWindoButtonVisible = true;
            this.graph3dManipulator1.Location = new System.Drawing.Point(0, 0);
            this.graph3dManipulator1.MinimumSize = new System.Drawing.Size(524, 98);
            this.graph3dManipulator1.Name = "graph3dManipulator1";
            this.graph3dManipulator1.Size = new System.Drawing.Size(524, 98);
            this.graph3dManipulator1.TabIndex = 1;
            this.graph3dManipulator1.VtkContainer = null;
            this.graph3dManipulator1.VtkControl = null;
            // 
            // Graph3dManipulatorExtended
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.graph3dManipulatorBasic1);
            this.Controls.Add(this.graph3dManipulator1);
            this.MinimumSize = new System.Drawing.Size(676, 130);
            this.Name = "Graph3dManipulatorExtended";
            this.Size = new System.Drawing.Size(676, 130);
            this.ResumeLayout(false);

        }

        #endregion

        private Graph3dManipulator graph3dManipulator1;
        private Graph3dManipulatorBasic graph3dManipulatorBasic1;
    }
}
