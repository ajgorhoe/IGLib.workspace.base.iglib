namespace IG.Forms
{
    partial class DialogForm
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
            this.components = new System.ComponentModel.Container();
            this.messageControl1 = new IG.Forms.DialogControl();
            this.SuspendLayout();
            // 
            // messageControl1
            // 
            this.messageControl1.AutoSize = true;
            this.messageControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.messageControl1.Buttons = new string[] {
        "OK",
        "Cancel"};
            this.messageControl1.ControlText = "";
            this.messageControl1.IsTextChangedEventOnValidationOnly = false;
            this.messageControl1.IsTextEditable = true;
            this.messageControl1.IsTextMultiLine = true;
            this.messageControl1.IsTextPassword = false;
            this.messageControl1.IsTextSettable = true;
            this.messageControl1.IsTextSetThrows = true;
            this.messageControl1.IsTextVisible = true;
            this.messageControl1.Location = new System.Drawing.Point(0, 0);
            this.messageControl1.Message = "Message";
            this.messageControl1.Name = "messageControl1";
            this.messageControl1.Size = new System.Drawing.Size(335, 325);
            this.messageControl1.TabIndex = 0;
            this.messageControl1.TextBoxHeight = 200;
            this.messageControl1.TextBoxWidth = 320;
            this.messageControl1.IsTextCausesVisible = false;
            this.messageControl1.Title = "Title";
            this.messageControl1.ZoomFactor = 1.2D;
            // 
            // DialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(332, 319);
            this.Controls.Add(this.messageControl1);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.Name = "DialogForm";
            this.Text = "MessageBoxForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DialogControl messageControl1;
    }
}