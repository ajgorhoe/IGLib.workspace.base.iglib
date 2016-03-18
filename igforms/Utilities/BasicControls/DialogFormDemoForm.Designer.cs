namespace IG.Forms
{
    partial class DialogFormDemoForm
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
            this.messageBoxDemoControl1 = new IG.Forms.DialogFormDemoControl();
            this.SuspendLayout();
            // 
            // messageBoxDemoControl1
            // 
            this.messageBoxDemoControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageBoxDemoControl1.Location = new System.Drawing.Point(0, 0);
            this.messageBoxDemoControl1.Name = "messageBoxDemoControl1";
            this.messageBoxDemoControl1.Size = new System.Drawing.Size(901, 733);
            this.messageBoxDemoControl1.TabIndex = 0;
            // 
            // DialogFormDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 733);
            this.Controls.Add(this.messageBoxDemoControl1);
            this.MinimumSize = new System.Drawing.Size(916, 772);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.Name = "DialogFormDemoForm";
            this.Text = "MessageBoxDemoForm";
            this.ResumeLayout(false);

        }

        #endregion

        private IG.Forms.DialogFormDemoControl messageBoxDemoControl1;
    }
}