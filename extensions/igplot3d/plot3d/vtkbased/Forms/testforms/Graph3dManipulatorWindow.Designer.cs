namespace IG.Gr3d
{
    partial class Graph3dManipulatorWindow
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
            this.graph3DManipulator1 = new IG.Gr3d.Graph3dManipulator();
            this.SuspendLayout();
            // 
            // graph3DManipulator1
            // 
            this.graph3DManipulator1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph3DManipulator1.IsCloseWindoButtonVisible = true;
            this.graph3DManipulator1.Location = new System.Drawing.Point(0, 1);
            this.graph3DManipulator1.MinimumSize = new System.Drawing.Size(405, 97);
            this.graph3DManipulator1.Name = "graph3DManipulator1";
            this.graph3DManipulator1.Size = new System.Drawing.Size(525, 97);
            this.graph3DManipulator1.TabIndex = 0;
            this.graph3DManipulator1.VtkContainer = null;
            this.graph3DManipulator1.VtkControl = null;
            this.graph3DManipulator1.Load += new System.EventHandler(this.graph3DManipulator1_Load);
            // 
            // Graph3dManipulatorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 98);
            this.Controls.Add(this.graph3DManipulator1);
            this.MinimumSize = new System.Drawing.Size(539, 136);
            this.Name = "Graph3dManipulatorWindow";
            this.Text = "3D Graphics Manipulator (IGLib)";
            this.ResumeLayout(false);

        }

        #endregion

        private Graph3dManipulator graph3DManipulator1;

        // private Graph3dManipulator1 graph3DManipulatorOriginal;
    }
}