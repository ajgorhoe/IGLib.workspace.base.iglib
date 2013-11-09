namespace IG.Forms
{
    partial class HashGeneratorControl
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
            this.grpHashGeneration = new System.Windows.Forms.GroupBox();
            this.chkMd5 = new System.Windows.Forms.CheckBox();
            this.chkSha1 = new System.Windows.Forms.CheckBox();
            this.chkSha256 = new System.Windows.Forms.CheckBox();
            this.chkSha512 = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnClearHashes = new System.Windows.Forms.Button();
            this.chkGenerateFile = new System.Windows.Forms.CheckBox();
            this.chkUpperCase = new System.Windows.Forms.CheckBox();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rbText = new System.Windows.Forms.RadioButton();
            this.btnFileBrowse = new System.Windows.Forms.Button();
            this.btnMd5Copy = new System.Windows.Forms.Button();
            this.btnSha1Copy = new System.Windows.Forms.Button();
            this.btnSha256Copy = new System.Windows.Forms.Button();
            this.bthSha512 = new System.Windows.Forms.Button();
            this.btnVerifyPaste = new System.Windows.Forms.Button();
            this.btnVerify = new System.Windows.Forms.Button();
            this.lvlVerifyMain = new System.Windows.Forms.Label();
            this.txtVerify = new System.Windows.Forms.TextBox();
            this.lblVerify = new System.Windows.Forms.Label();
            this.txtSha512 = new System.Windows.Forms.TextBox();
            this.lblSha512 = new System.Windows.Forms.Label();
            this.txtSha256 = new System.Windows.Forms.TextBox();
            this.lblSha256 = new System.Windows.Forms.Label();
            this.txtSha1 = new System.Windows.Forms.TextBox();
            this.lblSha1 = new System.Windows.Forms.Label();
            this.txtMd5 = new System.Windows.Forms.TextBox();
            this.lblMd5 = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.lblFile = new System.Windows.Forms.Label();
            this.grpTextPreview = new System.Windows.Forms.GroupBox();
            this.txtContents = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.grpHashGeneration.SuspendLayout();
            this.grpTextPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpHashGeneration
            // 
            this.grpHashGeneration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpHashGeneration.Controls.Add(this.chkMd5);
            this.grpHashGeneration.Controls.Add(this.chkSha1);
            this.grpHashGeneration.Controls.Add(this.chkSha256);
            this.grpHashGeneration.Controls.Add(this.chkSha512);
            this.grpHashGeneration.Controls.Add(this.btnGenerate);
            this.grpHashGeneration.Controls.Add(this.btnClearHashes);
            this.grpHashGeneration.Controls.Add(this.chkGenerateFile);
            this.grpHashGeneration.Controls.Add(this.chkUpperCase);
            this.grpHashGeneration.Controls.Add(this.rbFile);
            this.grpHashGeneration.Controls.Add(this.rbText);
            this.grpHashGeneration.Controls.Add(this.btnFileBrowse);
            this.grpHashGeneration.Controls.Add(this.btnMd5Copy);
            this.grpHashGeneration.Controls.Add(this.btnSha1Copy);
            this.grpHashGeneration.Controls.Add(this.btnSha256Copy);
            this.grpHashGeneration.Controls.Add(this.bthSha512);
            this.grpHashGeneration.Controls.Add(this.btnVerifyPaste);
            this.grpHashGeneration.Controls.Add(this.btnVerify);
            this.grpHashGeneration.Controls.Add(this.lvlVerifyMain);
            this.grpHashGeneration.Controls.Add(this.txtVerify);
            this.grpHashGeneration.Controls.Add(this.lblVerify);
            this.grpHashGeneration.Controls.Add(this.txtSha512);
            this.grpHashGeneration.Controls.Add(this.lblSha512);
            this.grpHashGeneration.Controls.Add(this.txtSha256);
            this.grpHashGeneration.Controls.Add(this.lblSha256);
            this.grpHashGeneration.Controls.Add(this.txtSha1);
            this.grpHashGeneration.Controls.Add(this.lblSha1);
            this.grpHashGeneration.Controls.Add(this.txtMd5);
            this.grpHashGeneration.Controls.Add(this.lblMd5);
            this.grpHashGeneration.Controls.Add(this.txtFile);
            this.grpHashGeneration.Controls.Add(this.lblFile);
            this.grpHashGeneration.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.grpHashGeneration.ForeColor = System.Drawing.Color.ForestGreen;
            this.grpHashGeneration.Location = new System.Drawing.Point(3, 3);
            this.grpHashGeneration.Name = "grpHashGeneration";
            this.grpHashGeneration.Size = new System.Drawing.Size(779, 262);
            this.grpHashGeneration.TabIndex = 0;
            this.grpHashGeneration.TabStop = false;
            this.grpHashGeneration.Text = "Hash Generation";
            // 
            // chkMd5
            // 
            this.chkMd5.AutoSize = true;
            this.chkMd5.Checked = true;
            this.chkMd5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMd5.Location = new System.Drawing.Point(81, 71);
            this.chkMd5.Name = "chkMd5";
            this.chkMd5.Size = new System.Drawing.Size(15, 14);
            this.chkMd5.TabIndex = 8;
            this.chkMd5.UseVisualStyleBackColor = true;
            this.chkMd5.CheckedChanged += new System.EventHandler(this.chkMd5_CheckedChanged);
            // 
            // chkSha1
            // 
            this.chkSha1.AutoSize = true;
            this.chkSha1.Checked = true;
            this.chkSha1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSha1.Location = new System.Drawing.Point(81, 95);
            this.chkSha1.Name = "chkSha1";
            this.chkSha1.Size = new System.Drawing.Size(15, 14);
            this.chkSha1.TabIndex = 9;
            this.chkSha1.UseVisualStyleBackColor = true;
            this.chkSha1.CheckedChanged += new System.EventHandler(this.chkSha1_CheckedChanged);
            // 
            // chkSha256
            // 
            this.chkSha256.AutoSize = true;
            this.chkSha256.Checked = true;
            this.chkSha256.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSha256.Location = new System.Drawing.Point(81, 122);
            this.chkSha256.Name = "chkSha256";
            this.chkSha256.Size = new System.Drawing.Size(15, 14);
            this.chkSha256.TabIndex = 10;
            this.chkSha256.UseVisualStyleBackColor = true;
            this.chkSha256.CheckedChanged += new System.EventHandler(this.chkSha256_CheckedChanged);
            // 
            // chkSha512
            // 
            this.chkSha512.AutoSize = true;
            this.chkSha512.Location = new System.Drawing.Point(81, 149);
            this.chkSha512.Name = "chkSha512";
            this.chkSha512.Size = new System.Drawing.Size(15, 14);
            this.chkSha512.TabIndex = 14;
            this.chkSha512.UseVisualStyleBackColor = true;
            this.chkSha512.CheckedChanged += new System.EventHandler(this.chkSha512_CheckedChanged);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnGenerate.ForeColor = System.Drawing.Color.Blue;
            this.btnGenerate.Location = new System.Drawing.Point(6, 175);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(155, 23);
            this.btnGenerate.TabIndex = 15;
            this.btnGenerate.Text = "Generate Hash Values";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnClearHashes
            // 
            this.btnClearHashes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnClearHashes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClearHashes.Location = new System.Drawing.Point(167, 175);
            this.btnClearHashes.Name = "btnClearHashes";
            this.btnClearHashes.Size = new System.Drawing.Size(61, 23);
            this.btnClearHashes.TabIndex = 15;
            this.btnClearHashes.Text = "Clear";
            this.btnClearHashes.UseVisualStyleBackColor = true;
            this.btnClearHashes.Click += new System.EventHandler(this.btnClearHashes_Click);
            // 
            // chkGenerateFile
            // 
            this.chkGenerateFile.AutoSize = true;
            this.chkGenerateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chkGenerateFile.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkGenerateFile.Location = new System.Drawing.Point(234, 181);
            this.chkGenerateFile.Name = "chkGenerateFile";
            this.chkGenerateFile.Size = new System.Drawing.Size(145, 17);
            this.chkGenerateFile.TabIndex = 9;
            this.chkGenerateFile.Text = "Save File Hashes to Files";
            this.chkGenerateFile.UseVisualStyleBackColor = true;
            this.chkGenerateFile.CheckedChanged += new System.EventHandler(this.chkGenerateFile_CheckedChanged);
            // 
            // chkUpperCase
            // 
            this.chkUpperCase.AutoSize = true;
            this.chkUpperCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.chkUpperCase.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkUpperCase.Location = new System.Drawing.Point(385, 181);
            this.chkUpperCase.Name = "chkUpperCase";
            this.chkUpperCase.Size = new System.Drawing.Size(82, 17);
            this.chkUpperCase.TabIndex = 9;
            this.chkUpperCase.Text = "Upper Case";
            this.chkUpperCase.UseVisualStyleBackColor = true;
            this.chkUpperCase.CheckedChanged += new System.EventHandler(this.chkUpperCase_CheckedChanged);
            // 
            // rbFile
            // 
            this.rbFile.AutoSize = true;
            this.rbFile.Checked = true;
            this.rbFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rbFile.ForeColor = System.Drawing.Color.Blue;
            this.rbFile.Location = new System.Drawing.Point(6, 20);
            this.rbFile.Name = "rbFile";
            this.rbFile.Size = new System.Drawing.Size(51, 17);
            this.rbFile.TabIndex = 0;
            this.rbFile.TabStop = true;
            this.rbFile.Text = "Files";
            this.rbFile.UseVisualStyleBackColor = true;
            this.rbFile.CheckedChanged += new System.EventHandler(this.rbFile_CheckedChanged);
            // 
            // rbText
            // 
            this.rbText.AutoSize = true;
            this.rbText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rbText.ForeColor = System.Drawing.Color.Blue;
            this.rbText.Location = new System.Drawing.Point(63, 20);
            this.rbText.Name = "rbText";
            this.rbText.Size = new System.Drawing.Size(50, 17);
            this.rbText.TabIndex = 1;
            this.rbText.TabStop = true;
            this.rbText.Text = "Text";
            this.rbText.UseVisualStyleBackColor = true;
            // 
            // btnFileBrowse
            // 
            this.btnFileBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnFileBrowse.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnFileBrowse.Location = new System.Drawing.Point(681, 40);
            this.btnFileBrowse.Name = "btnFileBrowse";
            this.btnFileBrowse.Size = new System.Drawing.Size(92, 23);
            this.btnFileBrowse.TabIndex = 4;
            this.btnFileBrowse.Text = "Browse";
            this.btnFileBrowse.UseVisualStyleBackColor = true;
            this.btnFileBrowse.Click += new System.EventHandler(this.btnFileBrowse_Click);
            // 
            // btnMd5Copy
            // 
            this.btnMd5Copy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMd5Copy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnMd5Copy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnMd5Copy.Location = new System.Drawing.Point(681, 67);
            this.btnMd5Copy.Name = "btnMd5Copy";
            this.btnMd5Copy.Size = new System.Drawing.Size(92, 23);
            this.btnMd5Copy.TabIndex = 7;
            this.btnMd5Copy.Text = "Copy MD5";
            this.btnMd5Copy.UseVisualStyleBackColor = true;
            this.btnMd5Copy.Click += new System.EventHandler(this.btnMd5Copy_Click);
            // 
            // btnSha1Copy
            // 
            this.btnSha1Copy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSha1Copy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSha1Copy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSha1Copy.Location = new System.Drawing.Point(681, 94);
            this.btnSha1Copy.Name = "btnSha1Copy";
            this.btnSha1Copy.Size = new System.Drawing.Size(92, 23);
            this.btnSha1Copy.TabIndex = 7;
            this.btnSha1Copy.Text = "Copy SHA-1";
            this.btnSha1Copy.UseVisualStyleBackColor = true;
            this.btnSha1Copy.Click += new System.EventHandler(this.btnSha1Copy_Click);
            // 
            // btnSha256Copy
            // 
            this.btnSha256Copy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSha256Copy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnSha256Copy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSha256Copy.Location = new System.Drawing.Point(681, 121);
            this.btnSha256Copy.Name = "btnSha256Copy";
            this.btnSha256Copy.Size = new System.Drawing.Size(92, 23);
            this.btnSha256Copy.TabIndex = 7;
            this.btnSha256Copy.Text = "Copy SHA-256";
            this.btnSha256Copy.UseVisualStyleBackColor = true;
            this.btnSha256Copy.Click += new System.EventHandler(this.btnSha256Copy_Click);
            // 
            // bthSha512
            // 
            this.bthSha512.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bthSha512.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.bthSha512.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bthSha512.Location = new System.Drawing.Point(681, 148);
            this.bthSha512.Name = "bthSha512";
            this.bthSha512.Size = new System.Drawing.Size(92, 23);
            this.bthSha512.TabIndex = 13;
            this.bthSha512.Text = "Copy SHA-512";
            this.bthSha512.UseVisualStyleBackColor = true;
            this.bthSha512.Click += new System.EventHandler(this.bthSha512_Click);
            // 
            // btnVerifyPaste
            // 
            this.btnVerifyPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVerifyPaste.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnVerifyPaste.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnVerifyPaste.Location = new System.Drawing.Point(681, 231);
            this.btnVerifyPaste.Name = "btnVerifyPaste";
            this.btnVerifyPaste.Size = new System.Drawing.Size(92, 23);
            this.btnVerifyPaste.TabIndex = 18;
            this.btnVerifyPaste.Text = "Paste Hash";
            this.btnVerifyPaste.UseVisualStyleBackColor = true;
            this.btnVerifyPaste.Click += new System.EventHandler(this.btnVerifyPaste_Click);
            // 
            // btnVerify
            // 
            this.btnVerify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVerify.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnVerify.ForeColor = System.Drawing.Color.DarkCyan;
            this.btnVerify.Location = new System.Drawing.Point(681, 205);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(92, 23);
            this.btnVerify.TabIndex = 18;
            this.btnVerify.Text = "Verify Hash";
            this.btnVerify.UseVisualStyleBackColor = true;
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // lvlVerifyMain
            // 
            this.lvlVerifyMain.AutoSize = true;
            this.lvlVerifyMain.ForeColor = System.Drawing.Color.DarkCyan;
            this.lvlVerifyMain.Location = new System.Drawing.Point(6, 208);
            this.lvlVerifyMain.Name = "lvlVerifyMain";
            this.lvlVerifyMain.Size = new System.Drawing.Size(448, 15);
            this.lvlVerifyMain.TabIndex = 19;
            this.lvlVerifyMain.Text = "Verify Hash Value for the Specified File or Text (any Supported Type):";
            // 
            // txtVerify
            // 
            this.txtVerify.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVerify.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtVerify.Location = new System.Drawing.Point(63, 231);
            this.txtVerify.Name = "txtVerify";
            this.txtVerify.Size = new System.Drawing.Size(612, 20);
            this.txtVerify.TabIndex = 17;
            // 
            // lblVerify
            // 
            this.lblVerify.AutoSize = true;
            this.lblVerify.ForeColor = System.Drawing.Color.DarkCyan;
            this.lblVerify.Location = new System.Drawing.Point(6, 231);
            this.lblVerify.Name = "lblVerify";
            this.lblVerify.Size = new System.Drawing.Size(44, 15);
            this.lblVerify.TabIndex = 16;
            this.lblVerify.Text = "Hash:";
            // 
            // txtSha512
            // 
            this.txtSha512.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSha512.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtSha512.Location = new System.Drawing.Point(102, 148);
            this.txtSha512.Name = "txtSha512";
            this.txtSha512.ReadOnly = true;
            this.txtSha512.Size = new System.Drawing.Size(573, 20);
            this.txtSha512.TabIndex = 12;
            // 
            // lblSha512
            // 
            this.lblSha512.AutoSize = true;
            this.lblSha512.ForeColor = System.Drawing.Color.DarkRed;
            this.lblSha512.Location = new System.Drawing.Point(6, 148);
            this.lblSha512.Name = "lblSha512";
            this.lblSha512.Size = new System.Drawing.Size(67, 15);
            this.lblSha512.TabIndex = 11;
            this.lblSha512.Text = "SHA-512:";
            // 
            // txtSha256
            // 
            this.txtSha256.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSha256.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtSha256.Location = new System.Drawing.Point(102, 121);
            this.txtSha256.Name = "txtSha256";
            this.txtSha256.ReadOnly = true;
            this.txtSha256.Size = new System.Drawing.Size(573, 20);
            this.txtSha256.TabIndex = 6;
            // 
            // lblSha256
            // 
            this.lblSha256.AutoSize = true;
            this.lblSha256.ForeColor = System.Drawing.Color.DarkRed;
            this.lblSha256.Location = new System.Drawing.Point(6, 121);
            this.lblSha256.Name = "lblSha256";
            this.lblSha256.Size = new System.Drawing.Size(67, 15);
            this.lblSha256.TabIndex = 5;
            this.lblSha256.Text = "SHA-256:";
            // 
            // txtSha1
            // 
            this.txtSha1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSha1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtSha1.Location = new System.Drawing.Point(102, 94);
            this.txtSha1.Name = "txtSha1";
            this.txtSha1.ReadOnly = true;
            this.txtSha1.Size = new System.Drawing.Size(573, 20);
            this.txtSha1.TabIndex = 6;
            // 
            // lblSha1
            // 
            this.lblSha1.AutoSize = true;
            this.lblSha1.ForeColor = System.Drawing.Color.DarkRed;
            this.lblSha1.Location = new System.Drawing.Point(6, 94);
            this.lblSha1.Name = "lblSha1";
            this.lblSha1.Size = new System.Drawing.Size(51, 15);
            this.lblSha1.TabIndex = 5;
            this.lblSha1.Text = "SHA-1:";
            // 
            // txtMd5
            // 
            this.txtMd5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMd5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtMd5.Location = new System.Drawing.Point(102, 67);
            this.txtMd5.Name = "txtMd5";
            this.txtMd5.ReadOnly = true;
            this.txtMd5.Size = new System.Drawing.Size(573, 20);
            this.txtMd5.TabIndex = 6;
            // 
            // lblMd5
            // 
            this.lblMd5.AutoSize = true;
            this.lblMd5.ForeColor = System.Drawing.Color.DarkRed;
            this.lblMd5.Location = new System.Drawing.Point(6, 67);
            this.lblMd5.Name = "lblMd5";
            this.lblMd5.Size = new System.Drawing.Size(41, 15);
            this.lblMd5.TabIndex = 5;
            this.lblMd5.Text = "MD5:";
            // 
            // txtFile
            // 
            this.txtFile.AllowDrop = true;
            this.txtFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtFile.ForeColor = System.Drawing.Color.Black;
            this.txtFile.Location = new System.Drawing.Point(63, 40);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(612, 20);
            this.txtFile.TabIndex = 3;
            this.txtFile.Text = "<< Drag files here or insert / copy file path >>";
            this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
            this.txtFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtFile_DragDrop);
            this.txtFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtFile_DragEnter);
            this.txtFile.Enter += new System.EventHandler(this.txtFile_Enter);
            this.txtFile.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFile_KeyDown);
            this.txtFile.MouseEnter += new System.EventHandler(this.txtFile_MouseEnter);
            this.txtFile.Validated += new System.EventHandler(this.txtFile_Validated);
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.ForeColor = System.Drawing.Color.Blue;
            this.lblFile.Location = new System.Drawing.Point(6, 40);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(35, 15);
            this.lblFile.TabIndex = 2;
            this.lblFile.Text = "File:";
            // 
            // grpTextPreview
            // 
            this.grpTextPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTextPreview.Controls.Add(this.txtContents);
            this.grpTextPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.grpTextPreview.ForeColor = System.Drawing.Color.ForestGreen;
            this.grpTextPreview.Location = new System.Drawing.Point(3, 271);
            this.grpTextPreview.Name = "grpTextPreview";
            this.grpTextPreview.Size = new System.Drawing.Size(779, 206);
            this.grpTextPreview.TabIndex = 1;
            this.grpTextPreview.TabStop = false;
            this.grpTextPreview.Text = "File Contents Preview: ";
            // 
            // txtContents
            // 
            this.txtContents.AllowDrop = true;
            this.txtContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContents.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtContents.Location = new System.Drawing.Point(6, 20);
            this.txtContents.Multiline = true;
            this.txtContents.Name = "txtContents";
            this.txtContents.ReadOnly = true;
            this.txtContents.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContents.Size = new System.Drawing.Size(767, 180);
            this.txtContents.TabIndex = 0;
            this.txtContents.Text = "<< Drag files or text here. >>";
            this.txtContents.TextChanged += new System.EventHandler(this.txtContents_TextChanged);
            this.txtContents.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtContents_DragDrop);
            this.txtContents.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtContents_DragEnter);
            this.txtContents.Enter += new System.EventHandler(this.txtContents_Enter);
            this.txtContents.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContents_KeyDown);
            this.txtContents.MouseEnter += new System.EventHandler(this.txtContents_MouseEnter);
            this.txtContents.Validated += new System.EventHandler(this.txtContents_Validated);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // HashGeneratorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpTextPreview);
            this.Controls.Add(this.grpHashGeneration);
            this.MinimumSize = new System.Drawing.Size(780, 360);
            this.Name = "HashGeneratorControl";
            this.Size = new System.Drawing.Size(785, 480);
            this.grpHashGeneration.ResumeLayout(false);
            this.grpHashGeneration.PerformLayout();
            this.grpTextPreview.ResumeLayout(false);
            this.grpTextPreview.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpHashGeneration;
        private System.Windows.Forms.CheckBox chkSha512;
        private System.Windows.Forms.Button bthSha512;
        private System.Windows.Forms.TextBox txtSha512;
        private System.Windows.Forms.Label lblSha512;
        private System.Windows.Forms.CheckBox chkSha256;
        private System.Windows.Forms.CheckBox chkSha1;
        private System.Windows.Forms.CheckBox chkMd5;
        private System.Windows.Forms.Button btnSha256Copy;
        private System.Windows.Forms.Button btnSha1Copy;
        private System.Windows.Forms.Button btnMd5Copy;
        private System.Windows.Forms.TextBox txtSha256;
        private System.Windows.Forms.Label lblSha256;
        private System.Windows.Forms.TextBox txtSha1;
        private System.Windows.Forms.Label lblSha1;
        private System.Windows.Forms.TextBox txtMd5;
        private System.Windows.Forms.Label lblMd5;
        private System.Windows.Forms.Button btnFileBrowse;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.RadioButton rbText;
        private System.Windows.Forms.RadioButton rbFile;
        private System.Windows.Forms.GroupBox grpTextPreview;
        private System.Windows.Forms.TextBox txtContents;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.CheckBox chkGenerateFile;
        private System.Windows.Forms.Label lvlVerifyMain;
        private System.Windows.Forms.Button btnVerify;
        private System.Windows.Forms.Button btnVerifyPaste;
        private System.Windows.Forms.TextBox txtVerify;
        private System.Windows.Forms.Label lblVerify;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnClearHashes;
        private System.Windows.Forms.CheckBox chkUpperCase;
    }
}
