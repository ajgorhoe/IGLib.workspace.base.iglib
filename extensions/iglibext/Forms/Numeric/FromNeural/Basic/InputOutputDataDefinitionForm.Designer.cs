namespace IG.Forms
{
    partial class InputOutputDataDefinitionForm
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
            this.inputOutputDataDefinitionCntrol1 = new IG.Forms.InputOutputDataDefinitionControl();
            this.SuspendLayout();
            // 
            // testControl1
            // 
            this.inputOutputDataDefinitionCntrol1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputOutputDataDefinitionCntrol1.Location = new System.Drawing.Point(0, 0);
            // this.inputOutputDataDefinitionCntrol1.Name = "testControl1";
            this.inputOutputDataDefinitionCntrol1.Size = new System.Drawing.Size(1065, 789);
            this.inputOutputDataDefinitionCntrol1.TabIndex = 0;
            // 
            // InputOutputDataDefinitionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 789);
            this.Controls.Add(this.inputOutputDataDefinitionCntrol1);
            this.Name = "InputOutputDataDefinitionForm";
            this.Text = "InputOutputDataDefinitionForm";
            this.ResumeLayout(false);

        }

        #endregion

        private IG.Forms.InputOutputDataDefinitionControl inputOutputDataDefinitionCntrol1;
    }
}