namespace IG.Forms
{
    partial class DialogControl
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
            this.components = new System.ComponentModel.Container();
            this.pnlOuter = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlTitle = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlMessage = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pnlText = new System.Windows.Forms.FlowLayoutPanel();
            this.txtText = new System.Windows.Forms.TextBox();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.menuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuCopyMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopyTitleAndMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuText = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopyText = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoomInTextBox = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoomOutTextBox = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoomInText = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoomOutText = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTextVisible = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowPasswordShortly = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSummary = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlOuter.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlMessage.SuspendLayout();
            this.pnlText.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlOuter
            // 
            this.pnlOuter.AutoSize = true;
            this.pnlOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOuter.Controls.Add(this.pnlTitle);
            this.pnlOuter.Controls.Add(this.pnlMessage);
            this.pnlOuter.Controls.Add(this.pnlText);
            this.pnlOuter.Controls.Add(this.pnlButtons);
            this.pnlOuter.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlOuter.Location = new System.Drawing.Point(0, 0);
            this.pnlOuter.Name = "pnlOuter";
            this.pnlOuter.Size = new System.Drawing.Size(289, 229);
            this.pnlOuter.TabIndex = 0;
            // 
            // pnlTitle
            // 
            this.pnlTitle.AutoSize = true;
            this.pnlTitle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Location = new System.Drawing.Point(3, 3);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(55, 32);
            this.pnlTitle.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.Blue;
            this.lblTitle.Location = new System.Drawing.Point(6, 6);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(43, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title";
            // 
            // pnlMessage
            // 
            this.pnlMessage.AutoSize = true;
            this.pnlMessage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMessage.Controls.Add(this.lblMessage);
            this.pnlMessage.Location = new System.Drawing.Point(3, 41);
            this.pnlMessage.Name = "pnlMessage";
            this.pnlMessage.Size = new System.Drawing.Size(62, 25);
            this.pnlMessage.TabIndex = 0;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblMessage.Location = new System.Drawing.Point(6, 6);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(6);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Message";
            // 
            // pnlText
            // 
            this.pnlText.AutoSize = true;
            this.pnlText.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlText.Controls.Add(this.txtText);
            this.pnlText.Location = new System.Drawing.Point(3, 72);
            this.pnlText.Name = "pnlText";
            this.pnlText.Size = new System.Drawing.Size(283, 113);
            this.pnlText.TabIndex = 0;
            // 
            // txtText
            // 
            this.txtText.AllowDrop = true;
            this.txtText.Location = new System.Drawing.Point(3, 3);
            this.txtText.MinimumSize = new System.Drawing.Size(90, 35);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(277, 107);
            this.txtText.TabIndex = 0;
            this.txtText.TextChanged += new System.EventHandler(this.txtText_TextChanged);
            this.txtText.Validated += new System.EventHandler(this.txtText_Validated);
            // 
            // pnlButtons
            // 
            this.pnlButtons.AutoSize = true;
            this.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlButtons.Controls.Add(this.btnOk);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnClose);
            this.pnlButtons.Location = new System.Drawing.Point(3, 191);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(261, 35);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.AutoSize = true;
            this.btnOk.Location = new System.Drawing.Point(6, 6);
            this.btnOk.Margin = new System.Windows.Forms.Padding(6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Visible = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.Location = new System.Drawing.Point(93, 6);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.Location = new System.Drawing.Point(180, 6);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCopyMessage,
            this.menuClose,
            this.menuText,
            this.menuTextVisible,
            this.menuShowPasswordShortly,
            this.menuSummary});
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(197, 136);
            // 
            // menuCopyMessage
            // 
            this.menuCopyMessage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCopyTitleAndMessage});
            this.menuCopyMessage.Name = "menuCopyMessage";
            this.menuCopyMessage.Size = new System.Drawing.Size(196, 22);
            this.menuCopyMessage.Text = "Copy Message";
            this.menuCopyMessage.Click += new System.EventHandler(this.menuCopyMessage_Click);
            // 
            // menuCopyTitleAndMessage
            // 
            this.menuCopyTitleAndMessage.Name = "menuCopyTitleAndMessage";
            this.menuCopyTitleAndMessage.Size = new System.Drawing.Size(197, 22);
            this.menuCopyTitleAndMessage.Text = "CopyTitle and Message";
            this.menuCopyTitleAndMessage.Click += new System.EventHandler(this.menuCopyTitleAndMessage_Click);
            // 
            // menuClose
            // 
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(196, 22);
            this.menuClose.Text = "Close (Crl+Right-Click)";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // menuText
            // 
            this.menuText.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCopyText,
            this.menuZoomInTextBox,
            this.menuZoomOutTextBox,
            this.menuZoomInText,
            this.menuZoomOutText});
            this.menuText.Name = "menuText";
            this.menuText.Size = new System.Drawing.Size(196, 22);
            this.menuText.Text = "Text";
            // 
            // menuCopyText
            // 
            this.menuCopyText.Name = "menuCopyText";
            this.menuCopyText.Size = new System.Drawing.Size(196, 22);
            this.menuCopyText.Text = "Copy Text to Clipboard";
            this.menuCopyText.Click += new System.EventHandler(this.menuCopyText_Click);
            // 
            // menuZoomInTextBox
            // 
            this.menuZoomInTextBox.Name = "menuZoomInTextBox";
            this.menuZoomInTextBox.Size = new System.Drawing.Size(196, 22);
            this.menuZoomInTextBox.Text = "Zoom In Text box";
            this.menuZoomInTextBox.Click += new System.EventHandler(this.menuZoomInTextBox_Click);
            // 
            // menuZoomOutTextBox
            // 
            this.menuZoomOutTextBox.Name = "menuZoomOutTextBox";
            this.menuZoomOutTextBox.Size = new System.Drawing.Size(196, 22);
            this.menuZoomOutTextBox.Text = "Zoom Out Text Box";
            this.menuZoomOutTextBox.Click += new System.EventHandler(this.menuZoomOutTextBox_Click);
            // 
            // menuZoomInText
            // 
            this.menuZoomInText.Name = "menuZoomInText";
            this.menuZoomInText.Size = new System.Drawing.Size(196, 22);
            this.menuZoomInText.Text = "Zoom In Text";
            this.menuZoomInText.Click += new System.EventHandler(this.menuZoomInText_Click);
            // 
            // menuZoomOutText
            // 
            this.menuZoomOutText.Name = "menuZoomOutText";
            this.menuZoomOutText.Size = new System.Drawing.Size(196, 22);
            this.menuZoomOutText.Text = "Zoom Out Text";
            this.menuZoomOutText.Click += new System.EventHandler(this.menuZoomOutText_Click);
            // 
            // menuTextVisible
            // 
            this.menuTextVisible.Checked = true;
            this.menuTextVisible.CheckOnClick = true;
            this.menuTextVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuTextVisible.Name = "menuTextVisible";
            this.menuTextVisible.Size = new System.Drawing.Size(196, 22);
            this.menuTextVisible.Text = "Text Visible";
            this.menuTextVisible.CheckedChanged += new System.EventHandler(this.menuTextVisible_CheckedChanged);
            // 
            // menuShowPasswordShortly
            // 
            this.menuShowPasswordShortly.Name = "menuShowPasswordShortly";
            this.menuShowPasswordShortly.Size = new System.Drawing.Size(196, 22);
            this.menuShowPasswordShortly.Text = "Show Password Shortly";
            this.menuShowPasswordShortly.Click += new System.EventHandler(this.menuShowPasswordShortly_Click);
            // 
            // menuSummary
            // 
            this.menuSummary.Name = "menuSummary";
            this.menuSummary.Size = new System.Drawing.Size(196, 22);
            this.menuSummary.Text = "Summary";
            this.menuSummary.Click += new System.EventHandler(this.menuSummary_Click);
            // 
            // DialogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ContextMenuStrip = this.menuMain;
            this.Controls.Add(this.pnlOuter);
            this.Name = "DialogControl";
            this.Size = new System.Drawing.Size(292, 232);
            this.pnlOuter.ResumeLayout(false);
            this.pnlOuter.PerformLayout();
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlMessage.ResumeLayout(false);
            this.pnlMessage.PerformLayout();
            this.pnlText.ResumeLayout(false);
            this.pnlText.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel pnlOuter;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.FlowLayoutPanel pnlText;
        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private System.Windows.Forms.FlowLayoutPanel pnlTitle;
        private System.Windows.Forms.FlowLayoutPanel pnlMessage;
        private System.Windows.Forms.ContextMenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem menuCopyMessage;
        private System.Windows.Forms.ToolStripMenuItem menuClose;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolStripMenuItem menuCopyTitleAndMessage;
        private System.Windows.Forms.ToolStripMenuItem menuText;
        private System.Windows.Forms.ToolStripMenuItem menuCopyText;
        private System.Windows.Forms.ToolStripMenuItem menuZoomInTextBox;
        private System.Windows.Forms.ToolStripMenuItem menuZoomOutTextBox;
        private System.Windows.Forms.ToolStripMenuItem menuZoomInText;
        private System.Windows.Forms.ToolStripMenuItem menuZoomOutText;
        private System.Windows.Forms.ToolStripMenuItem menuTextVisible;
        private System.Windows.Forms.ToolStripMenuItem menuSummary;
        private System.Windows.Forms.ToolStripMenuItem menuShowPasswordShortly;
    }
}
