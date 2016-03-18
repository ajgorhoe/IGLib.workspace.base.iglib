namespace IG.Forms
{
    partial class DialogFormDemoControl
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
            this.dialogControl1 = new IG.Forms.DialogControl();
            this.lblMainTitle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.txtText = new System.Windows.Forms.TextBox();
            this.lblButtons = new System.Windows.Forms.Label();
            this.textBotxtButtons = new System.Windows.Forms.TextBox();
            this.grpResults = new System.Windows.Forms.GroupBox();
            this.lblResultText = new System.Windows.Forms.Label();
            this.lblResultButton = new System.Windows.Forms.Label();
            this.lblResultTitleText = new System.Windows.Forms.Label();
            this.lblResultTitleButton = new System.Windows.Forms.Label();
            this.grpDialogControl = new System.Windows.Forms.GroupBox();
            this.flowDialogControlAndResults = new System.Windows.Forms.FlowLayoutPanel();
            this.btnLaunchDialog = new System.Windows.Forms.Button();
            this.btnRefreshDialog = new System.Windows.Forms.Button();
            this.grpSettings = new System.Windows.Forms.GroupBox();
            this.lblTextBoxHeight = new System.Windows.Forms.Label();
            this.lblTextBoxWidth = new System.Windows.Forms.Label();
            this.numTextBoxHeight = new System.Windows.Forms.NumericUpDown();
            this.numTextBoxWidth = new System.Windows.Forms.NumericUpDown();
            this.chkTextPassword = new System.Windows.Forms.CheckBox();
            this.chkTextImmediate = new System.Windows.Forms.CheckBox();
            this.chkTextMultiLine = new System.Windows.Forms.CheckBox();
            this.chkTextSetExceptions = new System.Windows.Forms.CheckBox();
            this.chkTextSettable = new System.Windows.Forms.CheckBox();
            this.chkTextEditable = new System.Windows.Forms.CheckBox();
            this.chkTextCausesVisible = new System.Windows.Forms.CheckBox();
            this.chkTextVisible = new System.Windows.Forms.CheckBox();
            this.indicatorSettingsApplied = new IG.Forms.IndicatorLight();
            this.chkImmediateText = new System.Windows.Forms.CheckBox();
            this.grpEvents = new System.Windows.Forms.GroupBox();
            this.indicatorButtonPressed = new IG.Forms.IndicatorLight();
            this.indicatorButtonResultChanged = new IG.Forms.IndicatorLight();
            this.indicatorTextChanged = new IG.Forms.IndicatorLight();
            this.btnRefreshText = new System.Windows.Forms.Button();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.grpResults.SuspendLayout();
            this.grpDialogControl.SuspendLayout();
            this.flowDialogControlAndResults.SuspendLayout();
            this.grpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTextBoxHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTextBoxWidth)).BeginInit();
            this.grpEvents.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // dialogControl1
            // 
            this.dialogControl1.AutoSize = true;
            this.dialogControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.dialogControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dialogControl1.Buttons = new string[] {
        "OK",
        "Cancel"};
            this.dialogControl1.ControlText = "";
            this.dialogControl1.IsTextChangedEventOnValidationOnly = true;
            this.dialogControl1.IsTextEditable = true;
            this.dialogControl1.IsTextMultiLine = true;
            this.dialogControl1.IsTextPassword = false;
            this.dialogControl1.IsTextSettable = true;
            this.dialogControl1.IsTextSetThrows = true;
            this.dialogControl1.IsTextVisible = true;
            this.dialogControl1.Location = new System.Drawing.Point(7, 19);
            this.dialogControl1.Message = null;
            this.dialogControl1.Name = "dialogControl1";
            this.dialogControl1.Size = new System.Drawing.Size(315, 162);
            this.dialogControl1.TabIndex = 0;
            this.dialogControl1.TextBoxHeight = 35;
            this.dialogControl1.TextBoxWidth = 298;
            this.dialogControl1.IsTextCausesVisible = true;
            this.dialogControl1.Title = null;
            this.dialogControl1.ZoomFactor = 1.2D;
            // 
            // lblMainTitle
            // 
            this.lblMainTitle.AutoSize = true;
            this.lblMainTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblMainTitle.ForeColor = System.Drawing.Color.Blue;
            this.lblMainTitle.Location = new System.Drawing.Point(3, 0);
            this.lblMainTitle.Name = "lblMainTitle";
            this.lblMainTitle.Size = new System.Drawing.Size(344, 24);
            this.lblMainTitle.TabIndex = 1;
            this.lblMainTitle.Text = "Dialog Box with Editable Text Demo";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(4, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(84, 13);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Dialog Box Title:";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(3, 40);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(301, 20);
            this.txtTitle.TabIndex = 3;
            this.txtTitle.Text = "Title";
            this.txtTitle.TextChanged += new System.EventHandler(this.txtTitle_TextChanged);
            this.txtTitle.Validated += new System.EventHandler(this.txtTitle_Validated);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(3, 79);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessage.Size = new System.Drawing.Size(301, 95);
            this.txtMessage.TabIndex = 3;
            this.txtMessage.Text = "This is dialog message.\r\nThis is another line of the message.";
            this.txtMessage.TextChanged += new System.EventHandler(this.txtMessage_TextChanged);
            this.txtMessage.Validated += new System.EventHandler(this.txtMessage_Validated);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(4, 63);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(107, 13);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "Dialog Box Message:";
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(4, 177);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(85, 13);
            this.lblText.TabIndex = 2;
            this.lblText.Text = "Dialog Box Text:";
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(3, 193);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtText.Size = new System.Drawing.Size(301, 124);
            this.txtText.TabIndex = 3;
            this.txtText.Text = "This is dialog text.\r\nThis is another line of text.";
            this.txtText.TextChanged += new System.EventHandler(this.txtText_TextChanged);
            this.txtText.Validated += new System.EventHandler(this.txtText_Validated);
            // 
            // lblButtons
            // 
            this.lblButtons.AutoSize = true;
            this.lblButtons.Location = new System.Drawing.Point(329, 24);
            this.lblButtons.Name = "lblButtons";
            this.lblButtons.Size = new System.Drawing.Size(100, 13);
            this.lblButtons.TabIndex = 2;
            this.lblButtons.Text = "Dialog Box Buttons:";
            // 
            // textBotxtButtons
            // 
            this.textBotxtButtons.Location = new System.Drawing.Point(332, 40);
            this.textBotxtButtons.Multiline = true;
            this.textBotxtButtons.Name = "textBotxtButtons";
            this.textBotxtButtons.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBotxtButtons.Size = new System.Drawing.Size(188, 134);
            this.textBotxtButtons.TabIndex = 3;
            this.textBotxtButtons.Text = "Button 1\r\nCancel";
            this.textBotxtButtons.TextChanged += new System.EventHandler(this.textBotxtButtons_TextChanged);
            this.textBotxtButtons.Validated += new System.EventHandler(this.textBotxtButtons_Validated);
            // 
            // grpResults
            // 
            this.grpResults.AutoSize = true;
            this.grpResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpResults.Controls.Add(this.lblResultText);
            this.grpResults.Controls.Add(this.lblResultButton);
            this.grpResults.Controls.Add(this.lblResultTitleText);
            this.grpResults.Controls.Add(this.lblResultTitleButton);
            this.grpResults.ForeColor = System.Drawing.Color.Blue;
            this.grpResults.Location = new System.Drawing.Point(337, 3);
            this.grpResults.Name = "grpResults";
            this.grpResults.Size = new System.Drawing.Size(252, 79);
            this.grpResults.TabIndex = 4;
            this.grpResults.TabStop = false;
            this.grpResults.Text = "Dialog Results:";
            // 
            // lblResultText
            // 
            this.lblResultText.AutoSize = true;
            this.lblResultText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblResultText.ForeColor = System.Drawing.Color.Blue;
            this.lblResultText.Location = new System.Drawing.Point(6, 48);
            this.lblResultText.Name = "lblResultText";
            this.lblResultText.Size = new System.Drawing.Size(92, 15);
            this.lblResultText.TabIndex = 2;
            this.lblResultText.Text = "<< not defined >>";
            // 
            // lblResultButton
            // 
            this.lblResultButton.AutoSize = true;
            this.lblResultButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblResultButton.ForeColor = System.Drawing.Color.Blue;
            this.lblResultButton.Location = new System.Drawing.Point(154, 16);
            this.lblResultButton.Name = "lblResultButton";
            this.lblResultButton.Size = new System.Drawing.Size(92, 15);
            this.lblResultButton.TabIndex = 2;
            this.lblResultButton.Text = "<< not defined >>";
            // 
            // lblResultTitleText
            // 
            this.lblResultTitleText.AutoSize = true;
            this.lblResultTitleText.ForeColor = System.Drawing.Color.Black;
            this.lblResultTitleText.Location = new System.Drawing.Point(6, 35);
            this.lblResultTitleText.Name = "lblResultTitleText";
            this.lblResultTitleText.Size = new System.Drawing.Size(132, 13);
            this.lblResultTitleText.TabIndex = 2;
            this.lblResultTitleText.Text = "Dialog Box Resulting Text:";
            // 
            // lblResultTitleButton
            // 
            this.lblResultTitleButton.AutoSize = true;
            this.lblResultTitleButton.ForeColor = System.Drawing.Color.Black;
            this.lblResultTitleButton.Location = new System.Drawing.Point(6, 16);
            this.lblResultTitleButton.Name = "lblResultTitleButton";
            this.lblResultTitleButton.Size = new System.Drawing.Size(142, 13);
            this.lblResultTitleButton.TabIndex = 2;
            this.lblResultTitleButton.Text = "Dialog Box Resulting Button:";
            // 
            // grpDialogControl
            // 
            this.grpDialogControl.AutoSize = true;
            this.grpDialogControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpDialogControl.Controls.Add(this.dialogControl1);
            this.grpDialogControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.grpDialogControl.Location = new System.Drawing.Point(3, 3);
            this.grpDialogControl.Name = "grpDialogControl";
            this.grpDialogControl.Size = new System.Drawing.Size(328, 200);
            this.grpDialogControl.TabIndex = 5;
            this.grpDialogControl.TabStop = false;
            this.grpDialogControl.Text = "Embedded Dialog Control:";
            // 
            // flowDialogControlAndResults
            // 
            this.flowDialogControlAndResults.AutoSize = true;
            this.flowDialogControlAndResults.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowDialogControlAndResults.Controls.Add(this.grpDialogControl);
            this.flowDialogControlAndResults.Controls.Add(this.grpResults);
            this.flowDialogControlAndResults.Location = new System.Drawing.Point(3, 323);
            this.flowDialogControlAndResults.Name = "flowDialogControlAndResults";
            this.flowDialogControlAndResults.Size = new System.Drawing.Size(592, 206);
            this.flowDialogControlAndResults.TabIndex = 6;
            // 
            // btnLaunchDialog
            // 
            this.btnLaunchDialog.ForeColor = System.Drawing.Color.Blue;
            this.btnLaunchDialog.Location = new System.Drawing.Point(310, 236);
            this.btnLaunchDialog.Name = "btnLaunchDialog";
            this.btnLaunchDialog.Size = new System.Drawing.Size(210, 23);
            this.btnLaunchDialog.TabIndex = 7;
            this.btnLaunchDialog.Text = "Launch a New Dialog";
            this.btnLaunchDialog.UseVisualStyleBackColor = true;
            this.btnLaunchDialog.Click += new System.EventHandler(this.btnLaunchDialog_Click);
            // 
            // btnRefreshDialog
            // 
            this.btnRefreshDialog.ForeColor = System.Drawing.Color.Blue;
            this.btnRefreshDialog.Location = new System.Drawing.Point(310, 265);
            this.btnRefreshDialog.Name = "btnRefreshDialog";
            this.btnRefreshDialog.Size = new System.Drawing.Size(210, 23);
            this.btnRefreshDialog.TabIndex = 7;
            this.btnRefreshDialog.Text = "Refresh Dialog";
            this.btnRefreshDialog.UseVisualStyleBackColor = true;
            this.btnRefreshDialog.Click += new System.EventHandler(this.btnRefreshDialog_Click);
            // 
            // grpSettings
            // 
            this.grpSettings.Controls.Add(this.lblTextBoxHeight);
            this.grpSettings.Controls.Add(this.lblTextBoxWidth);
            this.grpSettings.Controls.Add(this.numTextBoxHeight);
            this.grpSettings.Controls.Add(this.numTextBoxWidth);
            this.grpSettings.Controls.Add(this.chkTextPassword);
            this.grpSettings.Controls.Add(this.chkTextImmediate);
            this.grpSettings.Controls.Add(this.chkTextMultiLine);
            this.grpSettings.Controls.Add(this.chkTextSetExceptions);
            this.grpSettings.Controls.Add(this.chkTextSettable);
            this.grpSettings.Controls.Add(this.chkTextEditable);
            this.grpSettings.Controls.Add(this.chkTextCausesVisible);
            this.grpSettings.Controls.Add(this.chkTextVisible);
            this.grpSettings.Location = new System.Drawing.Point(526, 24);
            this.grpSettings.Name = "grpSettings";
            this.grpSettings.Size = new System.Drawing.Size(161, 293);
            this.grpSettings.TabIndex = 8;
            this.grpSettings.TabStop = false;
            this.grpSettings.Text = "Settings";
            // 
            // lblTextBoxHeight
            // 
            this.lblTextBoxHeight.AutoSize = true;
            this.lblTextBoxHeight.Location = new System.Drawing.Point(3, 251);
            this.lblTextBoxHeight.Name = "lblTextBoxHeight";
            this.lblTextBoxHeight.Size = new System.Drawing.Size(86, 13);
            this.lblTextBoxHeight.TabIndex = 2;
            this.lblTextBoxHeight.Text = "Text Box Height:";
            // 
            // lblTextBoxWidth
            // 
            this.lblTextBoxWidth.AutoSize = true;
            this.lblTextBoxWidth.Location = new System.Drawing.Point(6, 212);
            this.lblTextBoxWidth.Name = "lblTextBoxWidth";
            this.lblTextBoxWidth.Size = new System.Drawing.Size(83, 13);
            this.lblTextBoxWidth.TabIndex = 2;
            this.lblTextBoxWidth.Text = "Text Box Width:";
            // 
            // numTextBoxHeight
            // 
            this.numTextBoxHeight.Location = new System.Drawing.Point(6, 267);
            this.numTextBoxHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numTextBoxHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTextBoxHeight.Name = "numTextBoxHeight";
            this.numTextBoxHeight.Size = new System.Drawing.Size(70, 20);
            this.numTextBoxHeight.TabIndex = 1;
            this.numTextBoxHeight.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numTextBoxHeight.Validated += new System.EventHandler(this.numTextBoxHeight_Validated);
            // 
            // numTextBoxWidth
            // 
            this.numTextBoxWidth.Location = new System.Drawing.Point(6, 228);
            this.numTextBoxWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numTextBoxWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTextBoxWidth.Name = "numTextBoxWidth";
            this.numTextBoxWidth.Size = new System.Drawing.Size(70, 20);
            this.numTextBoxWidth.TabIndex = 1;
            this.numTextBoxWidth.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numTextBoxWidth.Validated += new System.EventHandler(this.numTextBoxWidth_Validated);
            // 
            // chkTextPassword
            // 
            this.chkTextPassword.AutoSize = true;
            this.chkTextPassword.Location = new System.Drawing.Point(6, 180);
            this.chkTextPassword.Name = "chkTextPassword";
            this.chkTextPassword.Size = new System.Drawing.Size(99, 17);
            this.chkTextPassword.TabIndex = 0;
            this.chkTextPassword.Text = "Password Input";
            this.chkTextPassword.UseVisualStyleBackColor = true;
            this.chkTextPassword.CheckedChanged += new System.EventHandler(this.chkTextPassword_CheckedChanged);
            // 
            // chkTextImmediate
            // 
            this.chkTextImmediate.AutoSize = true;
            this.chkTextImmediate.Checked = true;
            this.chkTextImmediate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTextImmediate.Location = new System.Drawing.Point(6, 157);
            this.chkTextImmediate.Name = "chkTextImmediate";
            this.chkTextImmediate.Size = new System.Drawing.Size(117, 17);
            this.chkTextImmediate.TabIndex = 0;
            this.chkTextImmediate.Text = "Text Ch. Immediate";
            this.chkTextImmediate.UseVisualStyleBackColor = true;
            this.chkTextImmediate.CheckedChanged += new System.EventHandler(this.chkTextImmediate_CheckedChanged);
            // 
            // chkTextMultiLine
            // 
            this.chkTextMultiLine.AutoSize = true;
            this.chkTextMultiLine.Checked = true;
            this.chkTextMultiLine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTextMultiLine.Location = new System.Drawing.Point(6, 134);
            this.chkTextMultiLine.Name = "chkTextMultiLine";
            this.chkTextMultiLine.Size = new System.Drawing.Size(106, 17);
            this.chkTextMultiLine.TabIndex = 0;
            this.chkTextMultiLine.Text = "Text Is Multi Line";
            this.chkTextMultiLine.UseVisualStyleBackColor = true;
            this.chkTextMultiLine.CheckedChanged += new System.EventHandler(this.chkTextMultiLine_CheckedChanged);
            // 
            // chkTextSetExceptions
            // 
            this.chkTextSetExceptions.AutoSize = true;
            this.chkTextSetExceptions.Location = new System.Drawing.Point(6, 111);
            this.chkTextSetExceptions.Name = "chkTextSetExceptions";
            this.chkTextSetExceptions.Size = new System.Drawing.Size(141, 17);
            this.chkTextSetExceptions.TabIndex = 0;
            this.chkTextSetExceptions.Text = "Exception on Invalid Set";
            this.chkTextSetExceptions.UseVisualStyleBackColor = true;
            this.chkTextSetExceptions.CheckedChanged += new System.EventHandler(this.chkTextSetExceptions_CheckedChanged);
            // 
            // chkTextSettable
            // 
            this.chkTextSettable.AutoSize = true;
            this.chkTextSettable.Checked = true;
            this.chkTextSettable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTextSettable.Location = new System.Drawing.Point(6, 88);
            this.chkTextSettable.Name = "chkTextSettable";
            this.chkTextSettable.Size = new System.Drawing.Size(103, 17);
            this.chkTextSettable.TabIndex = 0;
            this.chkTextSettable.Text = "Text Can be Set";
            this.chkTextSettable.UseVisualStyleBackColor = true;
            this.chkTextSettable.CheckedChanged += new System.EventHandler(this.chkTextSettable_CheckedChanged);
            // 
            // chkTextEditable
            // 
            this.chkTextEditable.AutoSize = true;
            this.chkTextEditable.Checked = true;
            this.chkTextEditable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTextEditable.Location = new System.Drawing.Point(6, 65);
            this.chkTextEditable.Name = "chkTextEditable";
            this.chkTextEditable.Size = new System.Drawing.Size(99, 17);
            this.chkTextEditable.TabIndex = 0;
            this.chkTextEditable.Text = "Text Is Editable";
            this.chkTextEditable.UseVisualStyleBackColor = true;
            this.chkTextEditable.CheckedChanged += new System.EventHandler(this.chkTextEditable_CheckedChanged);
            // 
            // chkTextCausesVisible
            // 
            this.chkTextCausesVisible.AutoSize = true;
            this.chkTextCausesVisible.Location = new System.Drawing.Point(6, 42);
            this.chkTextCausesVisible.Name = "chkTextCausesVisible";
            this.chkTextCausesVisible.Size = new System.Drawing.Size(118, 17);
            this.chkTextCausesVisible.TabIndex = 0;
            this.chkTextCausesVisible.Text = "Text Causes Visible";
            this.chkTextCausesVisible.UseVisualStyleBackColor = true;
            this.chkTextCausesVisible.CheckedChanged += new System.EventHandler(this.chkTextCausesVisible_CheckedChanged);
            // 
            // chkTextVisible
            // 
            this.chkTextVisible.AutoSize = true;
            this.chkTextVisible.Checked = true;
            this.chkTextVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTextVisible.Location = new System.Drawing.Point(6, 19);
            this.chkTextVisible.Name = "chkTextVisible";
            this.chkTextVisible.Size = new System.Drawing.Size(91, 17);
            this.chkTextVisible.TabIndex = 0;
            this.chkTextVisible.Text = "Text Is Visible";
            this.chkTextVisible.UseVisualStyleBackColor = true;
            this.chkTextVisible.CheckedChanged += new System.EventHandler(this.chkTextVisible_CheckedChanged);
            // 
            // indicatorSettingsApplied
            // 
            this.indicatorSettingsApplied.AutoSize = true;
            this.indicatorSettingsApplied.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.indicatorSettingsApplied.BlinkIntervalMilliSeconds = 50;
            this.indicatorSettingsApplied.BorderLabel = false;
            this.indicatorSettingsApplied.BorderOut = false;
            this.indicatorSettingsApplied.ColorLabel = System.Drawing.Color.Black;
            this.indicatorSettingsApplied.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.indicatorSettingsApplied.HasBusy = true;
            this.indicatorSettingsApplied.HasError = true;
            this.indicatorSettingsApplied.HasOff = true;
            this.indicatorSettingsApplied.HasOk = true;
            this.indicatorSettingsApplied.IsBlinking = false;
            this.indicatorSettingsApplied.LabelFont = null;
            this.indicatorSettingsApplied.LabelText = "Settings Aplied";
            this.indicatorSettingsApplied.Location = new System.Drawing.Point(0, 19);
            this.indicatorSettingsApplied.MarginLabel = 2;
            this.indicatorSettingsApplied.MarginOut = 2;
            this.indicatorSettingsApplied.Name = "indicatorSettingsApplied";
            this.indicatorSettingsApplied.PaddingLabel = 2;
            this.indicatorSettingsApplied.PaddingOut = 2;
            this.indicatorSettingsApplied.Size = new System.Drawing.Size(107, 30);
            this.indicatorSettingsApplied.TabIndex = 0;
            this.indicatorSettingsApplied.ThrowOnInvalidSwitch = false;
            // 
            // chkImmediateText
            // 
            this.chkImmediateText.AutoSize = true;
            this.chkImmediateText.Location = new System.Drawing.Point(310, 193);
            this.chkImmediateText.Name = "chkImmediateText";
            this.chkImmediateText.Size = new System.Drawing.Size(145, 17);
            this.chkImmediateText.TabIndex = 0;
            this.chkImmediateText.Text = "Change Text Immediately";
            this.chkImmediateText.UseVisualStyleBackColor = true;
            this.chkImmediateText.CheckedChanged += new System.EventHandler(this.chkImmediateText_CheckedChanged);
            // 
            // grpEvents
            // 
            this.grpEvents.Controls.Add(this.indicatorButtonPressed);
            this.grpEvents.Controls.Add(this.indicatorButtonResultChanged);
            this.grpEvents.Controls.Add(this.indicatorTextChanged);
            this.grpEvents.Location = new System.Drawing.Point(693, 24);
            this.grpEvents.Name = "grpEvents";
            this.grpEvents.Size = new System.Drawing.Size(173, 128);
            this.grpEvents.TabIndex = 9;
            this.grpEvents.TabStop = false;
            this.grpEvents.Text = "Events (for Window Dialog):";
            // 
            // indicatorButtonPressed
            // 
            this.indicatorButtonPressed.AutoSize = true;
            this.indicatorButtonPressed.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.indicatorButtonPressed.BlinkIntervalMilliSeconds = 200;
            this.indicatorButtonPressed.BorderLabel = false;
            this.indicatorButtonPressed.BorderOut = false;
            this.indicatorButtonPressed.ColorLabel = System.Drawing.Color.Black;
            this.indicatorButtonPressed.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.indicatorButtonPressed.HasBusy = true;
            this.indicatorButtonPressed.HasError = true;
            this.indicatorButtonPressed.HasOff = true;
            this.indicatorButtonPressed.HasOk = true;
            this.indicatorButtonPressed.IsBlinking = false;
            this.indicatorButtonPressed.LabelFont = null;
            this.indicatorButtonPressed.LabelText = "Button Pressed";
            this.indicatorButtonPressed.Location = new System.Drawing.Point(6, 88);
            this.indicatorButtonPressed.MarginLabel = 2;
            this.indicatorButtonPressed.MarginOut = 2;
            this.indicatorButtonPressed.Name = "indicatorButtonPressed";
            this.indicatorButtonPressed.PaddingLabel = 2;
            this.indicatorButtonPressed.PaddingOut = 2;
            this.indicatorButtonPressed.Size = new System.Drawing.Size(109, 30);
            this.indicatorButtonPressed.TabIndex = 0;
            this.indicatorButtonPressed.ThrowOnInvalidSwitch = false;
            // 
            // indicatorButtonResultChanged
            // 
            this.indicatorButtonResultChanged.AutoSize = true;
            this.indicatorButtonResultChanged.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.indicatorButtonResultChanged.BlinkIntervalMilliSeconds = 200;
            this.indicatorButtonResultChanged.BorderLabel = false;
            this.indicatorButtonResultChanged.BorderOut = false;
            this.indicatorButtonResultChanged.ColorLabel = System.Drawing.Color.Black;
            this.indicatorButtonResultChanged.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.indicatorButtonResultChanged.HasBusy = true;
            this.indicatorButtonResultChanged.HasError = true;
            this.indicatorButtonResultChanged.HasOff = true;
            this.indicatorButtonResultChanged.HasOk = true;
            this.indicatorButtonResultChanged.IsBlinking = false;
            this.indicatorButtonResultChanged.LabelFont = null;
            this.indicatorButtonResultChanged.LabelText = "Button Result Changed";
            this.indicatorButtonResultChanged.Location = new System.Drawing.Point(6, 52);
            this.indicatorButtonResultChanged.MarginLabel = 2;
            this.indicatorButtonResultChanged.MarginOut = 2;
            this.indicatorButtonResultChanged.Name = "indicatorButtonResultChanged";
            this.indicatorButtonResultChanged.PaddingLabel = 2;
            this.indicatorButtonResultChanged.PaddingOut = 2;
            this.indicatorButtonResultChanged.Size = new System.Drawing.Size(147, 30);
            this.indicatorButtonResultChanged.TabIndex = 0;
            this.indicatorButtonResultChanged.ThrowOnInvalidSwitch = false;
            // 
            // indicatorTextChanged
            // 
            this.indicatorTextChanged.AutoSize = true;
            this.indicatorTextChanged.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.indicatorTextChanged.BlinkIntervalMilliSeconds = 50;
            this.indicatorTextChanged.BorderLabel = false;
            this.indicatorTextChanged.BorderOut = false;
            this.indicatorTextChanged.ColorLabel = System.Drawing.Color.Black;
            this.indicatorTextChanged.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.indicatorTextChanged.HasBusy = true;
            this.indicatorTextChanged.HasError = true;
            this.indicatorTextChanged.HasOff = true;
            this.indicatorTextChanged.HasOk = true;
            this.indicatorTextChanged.IsBlinking = false;
            this.indicatorTextChanged.LabelFont = null;
            this.indicatorTextChanged.LabelText = "Text Changed";
            this.indicatorTextChanged.Location = new System.Drawing.Point(6, 19);
            this.indicatorTextChanged.MarginLabel = 2;
            this.indicatorTextChanged.MarginOut = 2;
            this.indicatorTextChanged.Name = "indicatorTextChanged";
            this.indicatorTextChanged.PaddingLabel = 2;
            this.indicatorTextChanged.PaddingOut = 2;
            this.indicatorTextChanged.Size = new System.Drawing.Size(104, 30);
            this.indicatorTextChanged.TabIndex = 0;
            this.indicatorTextChanged.ThrowOnInvalidSwitch = false;
            // 
            // btnRefreshText
            // 
            this.btnRefreshText.ForeColor = System.Drawing.Color.Blue;
            this.btnRefreshText.Location = new System.Drawing.Point(310, 294);
            this.btnRefreshText.Name = "btnRefreshText";
            this.btnRefreshText.Size = new System.Drawing.Size(210, 23);
            this.btnRefreshText.TabIndex = 7;
            this.btnRefreshText.Text = "Refresh Text Result";
            this.btnRefreshText.UseVisualStyleBackColor = true;
            this.btnRefreshText.Click += new System.EventHandler(this.btnRefreshText_Click);
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.indicatorSettingsApplied);
            this.grpActions.Location = new System.Drawing.Point(693, 223);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(173, 100);
            this.grpActions.TabIndex = 10;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // DialogFormDemoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpEvents);
            this.Controls.Add(this.chkImmediateText);
            this.Controls.Add(this.grpSettings);
            this.Controls.Add(this.btnRefreshText);
            this.Controls.Add(this.btnRefreshDialog);
            this.Controls.Add(this.btnLaunchDialog);
            this.Controls.Add(this.flowDialogControlAndResults);
            this.Controls.Add(this.textBotxtButtons);
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lblButtons);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblMainTitle);
            this.MinimumSize = new System.Drawing.Size(880, 586);
            this.Name = "DialogFormDemoControl";
            this.Size = new System.Drawing.Size(880, 586);
            this.grpResults.ResumeLayout(false);
            this.grpResults.PerformLayout();
            this.grpDialogControl.ResumeLayout(false);
            this.grpDialogControl.PerformLayout();
            this.flowDialogControlAndResults.ResumeLayout(false);
            this.flowDialogControlAndResults.PerformLayout();
            this.grpSettings.ResumeLayout(false);
            this.grpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTextBoxHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTextBoxWidth)).EndInit();
            this.grpEvents.ResumeLayout(false);
            this.grpEvents.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpActions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DialogControl dialogControl1;
        private DialogForm dialogForm1;
        private System.Windows.Forms.Label lblMainTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.Label lblButtons;
        private System.Windows.Forms.TextBox textBotxtButtons;
        private System.Windows.Forms.GroupBox grpResults;
        private System.Windows.Forms.Label lblResultText;
        private System.Windows.Forms.Label lblResultButton;
        private System.Windows.Forms.Label lblResultTitleText;
        private System.Windows.Forms.Label lblResultTitleButton;
        private System.Windows.Forms.GroupBox grpDialogControl;
        private System.Windows.Forms.FlowLayoutPanel flowDialogControlAndResults;
        private System.Windows.Forms.Button btnLaunchDialog;
        private System.Windows.Forms.Button btnRefreshDialog;
        private System.Windows.Forms.GroupBox grpSettings;
        private System.Windows.Forms.CheckBox chkTextEditable;
        private System.Windows.Forms.CheckBox chkTextMultiLine;
        private System.Windows.Forms.CheckBox chkTextSettable;
        private System.Windows.Forms.CheckBox chkImmediateText;
        private System.Windows.Forms.NumericUpDown numTextBoxHeight;
        private System.Windows.Forms.NumericUpDown numTextBoxWidth;
        private System.Windows.Forms.Label lblTextBoxWidth;
        private System.Windows.Forms.GroupBox grpEvents;
        private System.Windows.Forms.Label lblTextBoxHeight;
        private IndicatorLight indicatorButtonResultChanged;
        private IndicatorLight indicatorTextChanged;
        private IndicatorLight indicatorButtonPressed;
        private System.Windows.Forms.Button btnRefreshText;
        private System.Windows.Forms.CheckBox chkTextSetExceptions;
        private IndicatorLight indicatorSettingsApplied;
        private System.Windows.Forms.CheckBox chkTextVisible;
        private System.Windows.Forms.CheckBox chkTextImmediate;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.CheckBox chkTextCausesVisible;
        private System.Windows.Forms.CheckBox chkTextPassword;
    }
}
