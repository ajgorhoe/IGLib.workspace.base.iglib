namespace IG.Forms
{
    partial class TestLayoutForm
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
            this.testLayoutControl1 = new IG.Forms.TestLayoutControl();
            this.SuspendLayout();
            // 
            // testLayoutControl1
            // 
            this.testLayoutControl1.AutoSize = true;
            this.testLayoutControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.testLayoutControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.testLayoutControl1.Location = new System.Drawing.Point(12, 12);
            this.testLayoutControl1.MinimumSize = new System.Drawing.Size(20, 20);
            this.testLayoutControl1.Name = "testLayoutControl1";
            this.testLayoutControl1.Size = new System.Drawing.Size(145, 216);
            this.testLayoutControl1.TabIndex = 0;
            // 
            // TestLayoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 522);
            this.Controls.Add(this.testLayoutControl1);
            this.Name = "TestLayoutForm";
            this.Text = "TestLayoutForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TestLayoutControl testLayoutControl1;
    }
}