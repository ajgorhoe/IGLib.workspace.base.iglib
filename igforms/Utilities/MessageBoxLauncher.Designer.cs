// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{

    /// <summary>An exemple form that launches various message boxes.
    /// <para>User can set all parameters that affect function and appearance, and launc the message
    /// boxes accordingly.</para></summary>
    partial class MessageBoxLauncher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserSimpleForm));

            this.grpIcons = new System.Windows.Forms.GroupBox();
            this.rbQuestion = new System.Windows.Forms.RadioButton();
            this.rbInfo = new System.Windows.Forms.RadioButton();
            this.rbWarning = new System.Windows.Forms.RadioButton();
            this.rbError = new System.Windows.Forms.RadioButton();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbOKCancel = new System.Windows.Forms.RadioButton();
            this.rbOK = new System.Windows.Forms.RadioButton();
            this.rbYesNoCancel = new System.Windows.Forms.RadioButton();
            this.rbYesNo = new System.Windows.Forms.RadioButton();
            this.rbAbortRetryIgnore = new System.Windows.Forms.RadioButton();
            this.grTitleText = new System.Windows.Forms.GroupBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblMsgText = new System.Windows.Forms.Label();
            this.lblMsgTitle = new System.Windows.Forms.Label();
            this.btnLaunch = new System.Windows.Forms.Button();
            this.grpResult = new System.Windows.Forms.GroupBox();
            this.lblButtonClicked = new System.Windows.Forms.Label();
            this.lblResultDescription = new System.Windows.Forms.Label();
            this.grpFileOpen = new System.Windows.Forms.GroupBox();
            this.lblFileSize = new System.Windows.Forms.Label();
            this.lblFileOpenSizeTitle = new System.Windows.Forms.Label();
            this.txtFileOpen = new System.Windows.Forms.TextBox();
            this.btnFileOpenBrowse = new System.Windows.Forms.Button();
            this.lblCurrentDirTitle = new System.Windows.Forms.Label();
            this.lblCurrentDir = new System.Windows.Forms.Label();
            this.btnCurrentDirRefresh = new System.Windows.Forms.Button();
            this.grpDirOpen = new System.Windows.Forms.GroupBox();
            this.lblDirNumFiles = new System.Windows.Forms.Label();
            this.lblDirNumFilesTitle = new System.Windows.Forms.Label();
            this.txtDirOpen = new System.Windows.Forms.TextBox();
            this.btnDirOpenBrowse = new System.Windows.Forms.Button();
            this.grpIcons.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grTitleText.SuspendLayout();
            this.grpResult.SuspendLayout();
            this.grpFileOpen.SuspendLayout();
            this.grpDirOpen.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpIcons
            // 
            this.grpIcons.Controls.Add(this.rbQuestion);
            this.grpIcons.Controls.Add(this.rbInfo);
            this.grpIcons.Controls.Add(this.rbWarning);
            this.grpIcons.Controls.Add(this.rbError);
            this.grpIcons.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.grpIcons.Location = new System.Drawing.Point(12, 50);
            this.grpIcons.Name = "grpIcons";
            this.grpIcons.Size = new System.Drawing.Size(184, 139);
            this.grpIcons.TabIndex = 0;
            this.grpIcons.TabStop = false;
            this.grpIcons.Text = "MessageBox type (icon)";
            // 
            // rbQuestion
            // 
            this.rbQuestion.AutoSize = true;
            this.rbQuestion.Location = new System.Drawing.Point(7, 89);
            this.rbQuestion.Name = "rbQuestion";
            this.rbQuestion.Size = new System.Drawing.Size(75, 17);
            this.rbQuestion.TabIndex = 0;
            this.rbQuestion.TabStop = true;
            this.rbQuestion.Text = "Question";
            this.rbQuestion.UseVisualStyleBackColor = true;
            // 
            // rbInfo
            // 
            this.rbInfo.AutoSize = true;
            this.rbInfo.Location = new System.Drawing.Point(7, 66);
            this.rbInfo.Name = "rbInfo";
            this.rbInfo.Size = new System.Drawing.Size(47, 17);
            this.rbInfo.TabIndex = 0;
            this.rbInfo.TabStop = true;
            this.rbInfo.Text = "Info";
            this.rbInfo.UseVisualStyleBackColor = true;
            // 
            // rbWarning
            // 
            this.rbWarning.AutoSize = true;
            this.rbWarning.Location = new System.Drawing.Point(7, 43);
            this.rbWarning.Name = "rbWarning";
            this.rbWarning.Size = new System.Drawing.Size(72, 17);
            this.rbWarning.TabIndex = 0;
            this.rbWarning.TabStop = true;
            this.rbWarning.Text = "Warning";
            this.rbWarning.UseVisualStyleBackColor = true;
            // 
            // rbError
            // 
            this.rbError.AutoSize = true;
            this.rbError.Checked = true;
            this.rbError.Location = new System.Drawing.Point(7, 20);
            this.rbError.Name = "rbError";
            this.rbError.Size = new System.Drawing.Size(52, 17);
            this.rbError.TabIndex = 0;
            this.rbError.TabStop = true;
            this.rbError.Text = "Error";
            this.rbError.UseVisualStyleBackColor = true;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Wide Latin", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Teal;
            this.lblTitle.Location = new System.Drawing.Point(20, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(503, 29);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Message Box Launcher";
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbOKCancel);
            this.groupBox1.Controls.Add(this.rbOK);
            this.groupBox1.Controls.Add(this.rbYesNoCancel);
            this.groupBox1.Controls.Add(this.rbYesNo);
            this.groupBox1.Controls.Add(this.rbAbortRetryIgnore);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox1.Location = new System.Drawing.Point(226, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(235, 139);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Buttons shown";
            // 
            // rbOKCancel
            // 
            this.rbOKCancel.AutoSize = true;
            this.rbOKCancel.Location = new System.Drawing.Point(6, 112);
            this.rbOKCancel.Name = "rbOKCancel";
            this.rbOKCancel.Size = new System.Drawing.Size(89, 17);
            this.rbOKCancel.TabIndex = 0;
            this.rbOKCancel.TabStop = true;
            this.rbOKCancel.Text = "OK, Cancel";
            this.rbOKCancel.UseVisualStyleBackColor = true;
            // 
            // rbOK
            // 
            this.rbOK.AutoSize = true;
            this.rbOK.Location = new System.Drawing.Point(6, 89);
            this.rbOK.Name = "rbOK";
            this.rbOK.Size = new System.Drawing.Size(42, 17);
            this.rbOK.TabIndex = 0;
            this.rbOK.TabStop = true;
            this.rbOK.Text = "OK";
            this.rbOK.UseVisualStyleBackColor = true;
            // 
            // rbYesNoCancel
            // 
            this.rbYesNoCancel.AutoSize = true;
            this.rbYesNoCancel.Location = new System.Drawing.Point(6, 66);
            this.rbYesNoCancel.Name = "rbYesNoCancel";
            this.rbYesNoCancel.Size = new System.Drawing.Size(117, 17);
            this.rbYesNoCancel.TabIndex = 0;
            this.rbYesNoCancel.TabStop = true;
            this.rbYesNoCancel.Text = "Yes, No, Cancel";
            this.rbYesNoCancel.UseVisualStyleBackColor = true;
            // 
            // rbYesNo
            // 
            this.rbYesNo.AutoSize = true;
            this.rbYesNo.Checked = true;
            this.rbYesNo.Location = new System.Drawing.Point(6, 43);
            this.rbYesNo.Name = "rbYesNo";
            this.rbYesNo.Size = new System.Drawing.Size(70, 17);
            this.rbYesNo.TabIndex = 0;
            this.rbYesNo.TabStop = true;
            this.rbYesNo.Text = "Yes, No";
            this.rbYesNo.UseVisualStyleBackColor = true;
            // 
            // rbAbortRetryIgnore
            // 
            this.rbAbortRetryIgnore.AutoSize = true;
            this.rbAbortRetryIgnore.Location = new System.Drawing.Point(6, 20);
            this.rbAbortRetryIgnore.Name = "rbAbortRetryIgnore";
            this.rbAbortRetryIgnore.Size = new System.Drawing.Size(137, 17);
            this.rbAbortRetryIgnore.TabIndex = 0;
            this.rbAbortRetryIgnore.TabStop = true;
            this.rbAbortRetryIgnore.Text = "Abort, Retry, Ignore";
            this.rbAbortRetryIgnore.UseVisualStyleBackColor = true;
            // 
            // grTitleText
            // 
            this.grTitleText.Controls.Add(this.txtMessage);
            this.grTitleText.Controls.Add(this.txtTitle);
            this.grTitleText.Controls.Add(this.lblMsgText);
            this.grTitleText.Controls.Add(this.lblMsgTitle);
            this.grTitleText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.grTitleText.Location = new System.Drawing.Point(14, 199);
            this.grTitleText.Name = "grTitleText";
            this.grTitleText.Size = new System.Drawing.Size(509, 120);
            this.grTitleText.TabIndex = 3;
            this.grTitleText.TabStop = false;
            this.grTitleText.Text = "Title and text";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(17, 90);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(486, 20);
            this.txtMessage.TabIndex = 1;
            this.txtMessage.Text = "This is message box test.";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(17, 46);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(486, 20);
            this.txtTitle.TabIndex = 1;
            this.txtTitle.Text = "Messagebox Title";
            // 
            // lblMsgText
            // 
            this.lblMsgText.AutoSize = true;
            this.lblMsgText.Location = new System.Drawing.Point(11, 74);
            this.lblMsgText.Name = "lblMsgText";
            this.lblMsgText.Size = new System.Drawing.Size(90, 13);
            this.lblMsgText.TabIndex = 0;
            this.lblMsgText.Text = "Message Text:";
            // 
            // lblMsgTitle
            // 
            this.lblMsgTitle.AutoSize = true;
            this.lblMsgTitle.Location = new System.Drawing.Point(11, 27);
            this.lblMsgTitle.Name = "lblMsgTitle";
            this.lblMsgTitle.Size = new System.Drawing.Size(90, 13);
            this.lblMsgTitle.TabIndex = 0;
            this.lblMsgTitle.Text = "Message Title:";
            // 
            // btnLaunch
            // 
            this.btnLaunch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnLaunch.ForeColor = System.Drawing.Color.Blue;
            this.btnLaunch.Location = new System.Drawing.Point(14, 325);
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.Size = new System.Drawing.Size(173, 23);
            this.btnLaunch.TabIndex = 4;
            this.btnLaunch.Text = "Launch Message Box";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.btnLaunch_Click);
            // 
            // grpResult
            // 
            this.grpResult.Controls.Add(this.lblButtonClicked);
            this.grpResult.Controls.Add(this.lblResultDescription);
            this.grpResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.grpResult.Location = new System.Drawing.Point(16, 363);
            this.grpResult.Name = "grpResult";
            this.grpResult.Size = new System.Drawing.Size(507, 42);
            this.grpResult.TabIndex = 5;
            this.grpResult.TabStop = false;
            this.grpResult.Text = "Result:";
            // 
            // lblButtonClicked
            // 
            this.lblButtonClicked.AutoSize = true;
            this.lblButtonClicked.ForeColor = System.Drawing.Color.Blue;
            this.lblButtonClicked.Location = new System.Drawing.Point(213, 19);
            this.lblButtonClicked.Name = "lblButtonClicked";
            this.lblButtonClicked.Size = new System.Drawing.Size(131, 13);
            this.lblButtonClicked.TabIndex = 1;
            this.lblButtonClicked.Text = "< no button pressed >";
            // 
            // lblResultDescription
            // 
            this.lblResultDescription.AutoSize = true;
            this.lblResultDescription.Location = new System.Drawing.Point(14, 19);
            this.lblResultDescription.Name = "lblResultDescription";
            this.lblResultDescription.Size = new System.Drawing.Size(198, 13);
            this.lblResultDescription.TabIndex = 0;
            this.lblResultDescription.Text = "User clicked the following button:";
            // 
            // grpFileOpen
            // 
            this.grpFileOpen.Controls.Add(this.lblFileSize);
            this.grpFileOpen.Controls.Add(this.lblFileOpenSizeTitle);
            this.grpFileOpen.Controls.Add(this.txtFileOpen);
            this.grpFileOpen.Controls.Add(this.btnFileOpenBrowse);
            this.grpFileOpen.Location = new System.Drawing.Point(18, 461);
            this.grpFileOpen.Name = "grpFileOpen";
            this.grpFileOpen.Size = new System.Drawing.Size(505, 72);
            this.grpFileOpen.TabIndex = 6;
            this.grpFileOpen.TabStop = false;
            this.grpFileOpen.Text = "File Open Dialog Box";
            // 
            // lblFileSize
            // 
            this.lblFileSize.AutoSize = true;
            this.lblFileSize.Location = new System.Drawing.Point(109, 51);
            this.lblFileSize.Name = "lblFileSize";
            this.lblFileSize.Size = new System.Drawing.Size(90, 13);
            this.lblFileSize.TabIndex = 3;
            this.lblFileSize.Text = "< unknown size >";
            // 
            // lblFileOpenSizeTitle
            // 
            this.lblFileOpenSizeTitle.AutoSize = true;
            this.lblFileOpenSizeTitle.Location = new System.Drawing.Point(17, 51);
            this.lblFileOpenSizeTitle.Name = "lblFileOpenSizeTitle";
            this.lblFileOpenSizeTitle.Size = new System.Drawing.Size(86, 13);
            this.lblFileOpenSizeTitle.TabIndex = 2;
            this.lblFileOpenSizeTitle.Text = "File size in bytes:";
            // 
            // txtFileOpen
            // 
            this.txtFileOpen.Location = new System.Drawing.Point(53, 22);
            this.txtFileOpen.Name = "txtFileOpen";
            this.txtFileOpen.Size = new System.Drawing.Size(446, 20);
            this.txtFileOpen.TabIndex = 1;
            this.txtFileOpen.Text = "..\\..\\ExampleFile.txt";
            this.txtFileOpen.TextChanged += new System.EventHandler(this.txtFileOpen_TextChanged);
            // 
            // btnFileOpenBrowse
            // 
            this.btnFileOpenBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFileOpenBrowse.Location = new System.Drawing.Point(11, 19);
            this.btnFileOpenBrowse.Name = "btnFileOpenBrowse";
            this.btnFileOpenBrowse.Size = new System.Drawing.Size(37, 23);
            this.btnFileOpenBrowse.TabIndex = 0;
            this.btnFileOpenBrowse.Text = "...";
            this.btnFileOpenBrowse.UseVisualStyleBackColor = true;
            this.btnFileOpenBrowse.Click += new System.EventHandler(this.btnFileOpenBrowse_Click);
            // 
            // lblCurrentDirTitle
            // 
            this.lblCurrentDirTitle.AutoSize = true;
            this.lblCurrentDirTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCurrentDirTitle.Location = new System.Drawing.Point(23, 414);
            this.lblCurrentDirTitle.Name = "lblCurrentDirTitle";
            this.lblCurrentDirTitle.Size = new System.Drawing.Size(105, 13);
            this.lblCurrentDirTitle.TabIndex = 7;
            this.lblCurrentDirTitle.Text = "Current directory:";
            // 
            // lblCurrentDir
            // 
            this.lblCurrentDir.AutoSize = true;
            this.lblCurrentDir.Location = new System.Drawing.Point(28, 436);
            this.lblCurrentDir.Name = "lblCurrentDir";
            this.lblCurrentDir.Size = new System.Drawing.Size(75, 13);
            this.lblCurrentDir.TabIndex = 8;
            this.lblCurrentDir.Text = "< not known >";
            // 
            // btnCurrentDirRefresh
            // 
            this.btnCurrentDirRefresh.Location = new System.Drawing.Point(134, 409);
            this.btnCurrentDirRefresh.Name = "btnCurrentDirRefresh";
            this.btnCurrentDirRefresh.Size = new System.Drawing.Size(53, 23);
            this.btnCurrentDirRefresh.TabIndex = 9;
            this.btnCurrentDirRefresh.Text = "(refresh)";
            this.btnCurrentDirRefresh.UseVisualStyleBackColor = true;
            this.btnCurrentDirRefresh.Click += new System.EventHandler(this.btnCurrentDirRefresh_Click);
            // 
            // grpDirOpen
            // 
            this.grpDirOpen.Controls.Add(this.lblDirNumFiles);
            this.grpDirOpen.Controls.Add(this.lblDirNumFilesTitle);
            this.grpDirOpen.Controls.Add(this.txtDirOpen);
            this.grpDirOpen.Controls.Add(this.btnDirOpenBrowse);
            this.grpDirOpen.Location = new System.Drawing.Point(19, 539);
            this.grpDirOpen.Name = "grpDirOpen";
            this.grpDirOpen.Size = new System.Drawing.Size(505, 72);
            this.grpDirOpen.TabIndex = 6;
            this.grpDirOpen.TabStop = false;
            this.grpDirOpen.Text = "Directory Open Dialog Box";
            // 
            // lblDirNumFiles
            // 
            this.lblDirNumFiles.AutoSize = true;
            this.lblDirNumFiles.Location = new System.Drawing.Point(102, 51);
            this.lblDirNumFiles.Name = "lblDirNumFiles";
            this.lblDirNumFiles.Size = new System.Drawing.Size(107, 13);
            this.lblDirNumFiles.TabIndex = 3;
            this.lblDirNumFiles.Text = "< unknown number >";
            // 
            // lblDirNumFilesTitle
            // 
            this.lblDirNumFilesTitle.AutoSize = true;
            this.lblDirNumFilesTitle.Location = new System.Drawing.Point(17, 51);
            this.lblDirNumFilesTitle.Name = "lblDirNumFilesTitle";
            this.lblDirNumFilesTitle.Size = new System.Drawing.Size(80, 13);
            this.lblDirNumFilesTitle.TabIndex = 2;
            this.lblDirNumFilesTitle.Text = "Number of files:";
            // 
            // txtDirOpen
            // 
            this.txtDirOpen.Location = new System.Drawing.Point(53, 22);
            this.txtDirOpen.Name = "txtDirOpen";
            this.txtDirOpen.Size = new System.Drawing.Size(446, 20);
            this.txtDirOpen.TabIndex = 1;
            this.txtDirOpen.Text = "..\\..\\";
            this.txtDirOpen.TextChanged += new System.EventHandler(this.txtDirOpen_TextChanged);
            // 
            // btnDirOpenBrowse
            // 
            this.btnDirOpenBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnDirOpenBrowse.Location = new System.Drawing.Point(11, 19);
            this.btnDirOpenBrowse.Name = "btnDirOpenBrowse";
            this.btnDirOpenBrowse.Size = new System.Drawing.Size(37, 23);
            this.btnDirOpenBrowse.TabIndex = 0;
            this.btnDirOpenBrowse.Text = "...";
            this.btnDirOpenBrowse.UseVisualStyleBackColor = true;
            this.btnDirOpenBrowse.Click += new System.EventHandler(this.btnDirOpenBrowse_Click);
            // 
            // MessageBoxLauncher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 624);
            this.Controls.Add(this.btnCurrentDirRefresh);
            this.Controls.Add(this.lblCurrentDir);
            this.Controls.Add(this.lblCurrentDirTitle);
            this.Controls.Add(this.grpDirOpen);
            this.Controls.Add(this.grpFileOpen);
            this.Controls.Add(this.grpResult);
            this.Controls.Add(this.btnLaunch);
            this.Controls.Add(this.grTitleText);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.grpIcons);
            this.Name = "MessageBoxLauncher";
            this.Text = "Message Box Demo";
            this.Icon = IG.Forms.Properties.Resources.ig;
            this.Load += new System.EventHandler(this.MessageBoxLauncher_Load);
            this.grpIcons.ResumeLayout(false);
            this.grpIcons.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grTitleText.ResumeLayout(false);
            this.grTitleText.PerformLayout();
            this.grpResult.ResumeLayout(false);
            this.grpResult.PerformLayout();
            this.grpFileOpen.ResumeLayout(false);
            this.grpFileOpen.PerformLayout();
            this.grpDirOpen.ResumeLayout(false);
            this.grpDirOpen.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            
        }

        #endregion

        private System.Windows.Forms.GroupBox grpIcons;
        private System.Windows.Forms.RadioButton rbQuestion;
        private System.Windows.Forms.RadioButton rbInfo;
        private System.Windows.Forms.RadioButton rbWarning;
        private System.Windows.Forms.RadioButton rbError;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbOKCancel;
        private System.Windows.Forms.RadioButton rbOK;
        private System.Windows.Forms.RadioButton rbYesNoCancel;
        private System.Windows.Forms.RadioButton rbYesNo;
        private System.Windows.Forms.RadioButton rbAbortRetryIgnore;
        private System.Windows.Forms.GroupBox grTitleText;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblMsgText;
        private System.Windows.Forms.Label lblMsgTitle;
        private System.Windows.Forms.Button btnLaunch;
        private System.Windows.Forms.GroupBox grpResult;
        private System.Windows.Forms.Label lblButtonClicked;
        private System.Windows.Forms.Label lblResultDescription;
        private System.Windows.Forms.GroupBox grpFileOpen;
        private System.Windows.Forms.Label lblCurrentDirTitle;
        private System.Windows.Forms.Label lblCurrentDir;
        private System.Windows.Forms.Button btnCurrentDirRefresh;
        private System.Windows.Forms.Button btnFileOpenBrowse;
        private System.Windows.Forms.TextBox txtFileOpen;
        private System.Windows.Forms.Label lblFileSize;
        private System.Windows.Forms.Label lblFileOpenSizeTitle;
        private System.Windows.Forms.GroupBox grpDirOpen;
        private System.Windows.Forms.Label lblDirNumFiles;
        private System.Windows.Forms.Label lblDirNumFilesTitle;
        private System.Windows.Forms.TextBox txtDirOpen;
        private System.Windows.Forms.Button btnDirOpenBrowse;
    }
}