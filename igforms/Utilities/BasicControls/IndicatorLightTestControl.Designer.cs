// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class IndicatorLightTestControl
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
            this.indicatorLight1 = new IG.Forms.IndicatorLight();
            this.fontSelectorSimple1 = new IG.Forms.FontSelectorSimple();
            this.chkThrowOnInvalid = new System.Windows.Forms.CheckBox();
            this.chkOuterBorder = new System.Windows.Forms.CheckBox();
            this.chkLabelBorder = new System.Windows.Forms.CheckBox();
            this.chkHasOff = new System.Windows.Forms.CheckBox();
            this.chkHasOk = new System.Windows.Forms.CheckBox();
            this.chkHasBusy = new System.Windows.Forms.CheckBox();
            this.chkHasError = new System.Windows.Forms.CheckBox();
            this.grpProperties = new System.Windows.Forms.GroupBox();
            this.comboDirection = new System.Windows.Forms.ComboBox();
            this.lblFlowDirection = new System.Windows.Forms.Label();
            this.txtLabelText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblBlinkTime = new System.Windows.Forms.Label();
            this.btnLabelColor = new System.Windows.Forms.Button();
            this.numLabelPadding = new System.Windows.Forms.NumericUpDown();
            this.numBlinkTime = new System.Windows.Forms.NumericUpDown();
            this.numLabelMargin = new System.Windows.Forms.NumericUpDown();
            this.numOuterPadding = new System.Windows.Forms.NumericUpDown();
            this.numOuterMargin = new System.Windows.Forms.NumericUpDown();
            this.lblLabelMargin = new System.Windows.Forms.Label();
            this.lblOuterMargin = new System.Windows.Forms.Label();
            this.grpState = new System.Windows.Forms.GroupBox();
            this.btnBlinkError2x = new System.Windows.Forms.Button();
            this.btnBlinkYellow3 = new System.Windows.Forms.Button();
            this.btnBlinkTwice = new System.Windows.Forms.Button();
            this.btnBlinkOnce = new System.Windows.Forms.Button();
            this.chkBlinking = new System.Windows.Forms.CheckBox();
            this.chkError = new System.Windows.Forms.CheckBox();
            this.chkBusy = new System.Windows.Forms.CheckBox();
            this.chkOk = new System.Windows.Forms.CheckBox();
            this.chkOff = new System.Windows.Forms.CheckBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.lblFont = new System.Windows.Forms.Label();
            this.lblLabelFont = new System.Windows.Forms.Label();
            this.pnlIndicatorContainerInner = new System.Windows.Forms.Panel();
            this.pnlIndicatorContainerOuter = new System.Windows.Forms.Panel();
            this.grpProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelPadding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlinkTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOuterPadding)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOuterMargin)).BeginInit();
            this.grpState.SuspendLayout();
            this.pnlIndicatorContainerInner.SuspendLayout();
            this.pnlIndicatorContainerOuter.SuspendLayout();
            this.SuspendLayout();
            // 
            // indicatorLight1
            // 
            this.indicatorLight1.AutoSize = true;
            this.indicatorLight1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.indicatorLight1.BlinkIntervalMilliSeconds = 500;
            this.indicatorLight1.BorderLabel = false;
            this.indicatorLight1.BorderOut = false;
            this.indicatorLight1.ColorLabel = System.Drawing.Color.Black;
            this.indicatorLight1.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.indicatorLight1.HasBusy = true;
            this.indicatorLight1.HasError = true;
            this.indicatorLight1.HasOff = true;
            this.indicatorLight1.HasOk = true;
            this.indicatorLight1.IsBlinking = false;
            this.indicatorLight1.LabelFont = null;
            this.indicatorLight1.LabelText = "Indicator";
            this.indicatorLight1.Location = new System.Drawing.Point(4, 7);
            this.indicatorLight1.Margin = new System.Windows.Forms.Padding(5);
            this.indicatorLight1.MarginLabel = 2;
            this.indicatorLight1.MarginOut = 2;
            this.indicatorLight1.Name = "indicatorLight1";
            this.indicatorLight1.PaddingLabel = 2;
            this.indicatorLight1.PaddingOut = 2;
            this.indicatorLight1.Size = new System.Drawing.Size(94, 36);
            this.indicatorLight1.TabIndex = 100;
            this.indicatorLight1.TabStop = false;
            this.indicatorLight1.ThrowOnInvalidSwitch = false;
            // 
            // fontSelectorSimple1
            // 
            this.fontSelectorSimple1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fontSelectorSimple1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fontSelectorSimple1.InitialFontLabelText = "< Click to select font! >";
            this.fontSelectorSimple1.Location = new System.Drawing.Point(5, 400);
            this.fontSelectorSimple1.Margin = new System.Windows.Forms.Padding(1);
            this.fontSelectorSimple1.MaximumSize = new System.Drawing.Size(1600, 0);
            this.fontSelectorSimple1.MinimumSize = new System.Drawing.Size(200, 18);
            this.fontSelectorSimple1.Name = "fontSelectorSimple1";
            this.fontSelectorSimple1.SelectedFont = new System.Drawing.Font("Times New Roman", 10F);
            this.fontSelectorSimple1.Size = new System.Drawing.Size(730, 18);
            this.fontSelectorSimple1.TabIndex = 72;
            this.fontSelectorSimple1.FontSelected += new IG.Forms.FontEventHandler(this.fontSelectorSimple1_FontSelected);
            // 
            // chkThrowOnInvalid
            // 
            this.chkThrowOnInvalid.AutoSize = true;
            this.chkThrowOnInvalid.Location = new System.Drawing.Point(8, 23);
            this.chkThrowOnInvalid.Margin = new System.Windows.Forms.Padding(4);
            this.chkThrowOnInvalid.Name = "chkThrowOnInvalid";
            this.chkThrowOnInvalid.Size = new System.Drawing.Size(163, 20);
            this.chkThrowOnInvalid.TabIndex = 30;
            this.chkThrowOnInvalid.Text = "Throw on Invalid switch";
            this.chkThrowOnInvalid.UseVisualStyleBackColor = true;
            this.chkThrowOnInvalid.CheckedChanged += new System.EventHandler(this.chkThrowOnInvalid_CheckedChanged);
            // 
            // chkOuterBorder
            // 
            this.chkOuterBorder.AutoSize = true;
            this.chkOuterBorder.Location = new System.Drawing.Point(8, 163);
            this.chkOuterBorder.Margin = new System.Windows.Forms.Padding(4);
            this.chkOuterBorder.Name = "chkOuterBorder";
            this.chkOuterBorder.Size = new System.Drawing.Size(102, 20);
            this.chkOuterBorder.TabIndex = 40;
            this.chkOuterBorder.Text = "Outer border";
            this.chkOuterBorder.UseVisualStyleBackColor = true;
            this.chkOuterBorder.CheckedChanged += new System.EventHandler(this.chkOuterBorder_CheckedChanged);
            // 
            // chkLabelBorder
            // 
            this.chkLabelBorder.AutoSize = true;
            this.chkLabelBorder.Location = new System.Drawing.Point(8, 191);
            this.chkLabelBorder.Margin = new System.Windows.Forms.Padding(4);
            this.chkLabelBorder.Name = "chkLabelBorder";
            this.chkLabelBorder.Size = new System.Drawing.Size(104, 20);
            this.chkLabelBorder.TabIndex = 42;
            this.chkLabelBorder.Text = "Label border";
            this.chkLabelBorder.UseVisualStyleBackColor = true;
            this.chkLabelBorder.CheckedChanged += new System.EventHandler(this.chkLabelBorder_CheckedChanged);
            // 
            // chkHasOff
            // 
            this.chkHasOff.AutoSize = true;
            this.chkHasOff.Location = new System.Drawing.Point(8, 51);
            this.chkHasOff.Margin = new System.Windows.Forms.Padding(4);
            this.chkHasOff.Name = "chkHasOff";
            this.chkHasOff.Size = new System.Drawing.Size(105, 20);
            this.chkHasOff.TabIndex = 32;
            this.chkHasOff.Text = "Has Off State";
            this.chkHasOff.UseVisualStyleBackColor = true;
            this.chkHasOff.CheckedChanged += new System.EventHandler(this.chkHasOff_CheckedChanged);
            // 
            // chkHasOk
            // 
            this.chkHasOk.AutoSize = true;
            this.chkHasOk.Location = new System.Drawing.Point(8, 79);
            this.chkHasOk.Margin = new System.Windows.Forms.Padding(4);
            this.chkHasOk.Name = "chkHasOk";
            this.chkHasOk.Size = new System.Drawing.Size(105, 20);
            this.chkHasOk.TabIndex = 34;
            this.chkHasOk.Text = "Has OK state";
            this.chkHasOk.UseVisualStyleBackColor = true;
            this.chkHasOk.CheckedChanged += new System.EventHandler(this.chkHasOk_CheckedChanged);
            // 
            // chkHasBusy
            // 
            this.chkHasBusy.AutoSize = true;
            this.chkHasBusy.Location = new System.Drawing.Point(8, 107);
            this.chkHasBusy.Margin = new System.Windows.Forms.Padding(4);
            this.chkHasBusy.Name = "chkHasBusy";
            this.chkHasBusy.Size = new System.Drawing.Size(119, 20);
            this.chkHasBusy.TabIndex = 36;
            this.chkHasBusy.Text = "Has Busy State";
            this.chkHasBusy.UseVisualStyleBackColor = true;
            this.chkHasBusy.CheckedChanged += new System.EventHandler(this.chkHasBusy_CheckedChanged);
            // 
            // chkHasError
            // 
            this.chkHasError.AutoSize = true;
            this.chkHasError.Location = new System.Drawing.Point(8, 135);
            this.chkHasError.Margin = new System.Windows.Forms.Padding(4);
            this.chkHasError.Name = "chkHasError";
            this.chkHasError.Size = new System.Drawing.Size(116, 20);
            this.chkHasError.TabIndex = 38;
            this.chkHasError.Text = "Has Error state";
            this.chkHasError.UseVisualStyleBackColor = true;
            this.chkHasError.CheckedChanged += new System.EventHandler(this.chkHasError_CheckedChanged);
            // 
            // grpProperties
            // 
            this.grpProperties.Controls.Add(this.comboDirection);
            this.grpProperties.Controls.Add(this.lblFlowDirection);
            this.grpProperties.Controls.Add(this.txtLabelText);
            this.grpProperties.Controls.Add(this.label1);
            this.grpProperties.Controls.Add(this.lblBlinkTime);
            this.grpProperties.Controls.Add(this.btnLabelColor);
            this.grpProperties.Controls.Add(this.numLabelPadding);
            this.grpProperties.Controls.Add(this.numBlinkTime);
            this.grpProperties.Controls.Add(this.numLabelMargin);
            this.grpProperties.Controls.Add(this.numOuterPadding);
            this.grpProperties.Controls.Add(this.numOuterMargin);
            this.grpProperties.Controls.Add(this.lblLabelMargin);
            this.grpProperties.Controls.Add(this.lblOuterMargin);
            this.grpProperties.Controls.Add(this.chkOuterBorder);
            this.grpProperties.Controls.Add(this.chkLabelBorder);
            this.grpProperties.Controls.Add(this.chkHasError);
            this.grpProperties.Controls.Add(this.chkThrowOnInvalid);
            this.grpProperties.Controls.Add(this.chkHasBusy);
            this.grpProperties.Controls.Add(this.chkHasOk);
            this.grpProperties.Controls.Add(this.chkHasOff);
            this.grpProperties.Location = new System.Drawing.Point(379, 15);
            this.grpProperties.Margin = new System.Windows.Forms.Padding(4);
            this.grpProperties.Name = "grpProperties";
            this.grpProperties.Padding = new System.Windows.Forms.Padding(4);
            this.grpProperties.Size = new System.Drawing.Size(356, 358);
            this.grpProperties.TabIndex = 2;
            this.grpProperties.TabStop = false;
            this.grpProperties.Text = "Indicator properties";
            // 
            // comboDirection
            // 
            this.comboDirection.FormattingEnabled = true;
            this.comboDirection.Items.AddRange(new object[] {
            "LeftToright",
            "TopDown",
            "RightToLeft",
            "BottomUp"});
            this.comboDirection.Location = new System.Drawing.Point(8, 325);
            this.comboDirection.Margin = new System.Windows.Forms.Padding(4);
            this.comboDirection.Name = "comboDirection";
            this.comboDirection.Size = new System.Drawing.Size(339, 24);
            this.comboDirection.TabIndex = 54;
            this.comboDirection.SelectedIndexChanged += new System.EventHandler(this.comboDirection_SelectedIndexChanged);
            this.comboDirection.Click += new System.EventHandler(this.comboDirection_Click);
            // 
            // lblFlowDirection
            // 
            this.lblFlowDirection.AutoSize = true;
            this.lblFlowDirection.Location = new System.Drawing.Point(8, 303);
            this.lblFlowDirection.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFlowDirection.Name = "lblFlowDirection";
            this.lblFlowDirection.Size = new System.Drawing.Size(93, 16);
            this.lblFlowDirection.TabIndex = 104;
            this.lblFlowDirection.Text = "Flow direction:";
            // 
            // txtLabelText
            // 
            this.txtLabelText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLabelText.Location = new System.Drawing.Point(8, 239);
            this.txtLabelText.Margin = new System.Windows.Forms.Padding(4);
            this.txtLabelText.Name = "txtLabelText";
            this.txtLabelText.Size = new System.Drawing.Size(339, 22);
            this.txtLabelText.TabIndex = 50;
            this.txtLabelText.Text = "Indicator";
            this.txtLabelText.TextChanged += new System.EventHandler(this.txtLabelText_TextChanged);
            this.txtLabelText.Validated += new System.EventHandler(this.txtLabelText_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 219);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 16);
            this.label1.TabIndex = 102;
            this.label1.Text = "Label text:";
            // 
            // lblBlinkTime
            // 
            this.lblBlinkTime.AutoSize = true;
            this.lblBlinkTime.Location = new System.Drawing.Point(145, 160);
            this.lblBlinkTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBlinkTime.Name = "lblBlinkTime";
            this.lblBlinkTime.Size = new System.Drawing.Size(86, 16);
            this.lblBlinkTime.TabIndex = 110;
            this.lblBlinkTime.Text = "Blink time (s):";
            // 
            // btnLabelColor
            // 
            this.btnLabelColor.Location = new System.Drawing.Point(8, 271);
            this.btnLabelColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnLabelColor.Name = "btnLabelColor";
            this.btnLabelColor.Size = new System.Drawing.Size(155, 28);
            this.btnLabelColor.TabIndex = 52;
            this.btnLabelColor.Text = "Set Label Color";
            this.btnLabelColor.UseVisualStyleBackColor = true;
            this.btnLabelColor.Click += new System.EventHandler(this.btnLabelColor_Click);
            // 
            // numLabelPadding
            // 
            this.numLabelPadding.Location = new System.Drawing.Point(220, 130);
            this.numLabelPadding.Margin = new System.Windows.Forms.Padding(4);
            this.numLabelPadding.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numLabelPadding.Name = "numLabelPadding";
            this.numLabelPadding.Size = new System.Drawing.Size(63, 22);
            this.numLabelPadding.TabIndex = 66;
            this.numLabelPadding.ValueChanged += new System.EventHandler(this.numLabelPadding_ValueChanged);
            // 
            // numBlinkTime
            // 
            this.numBlinkTime.DecimalPlaces = 1;
            this.numBlinkTime.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numBlinkTime.Location = new System.Drawing.Point(149, 180);
            this.numBlinkTime.Margin = new System.Windows.Forms.Padding(4);
            this.numBlinkTime.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numBlinkTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numBlinkTime.Name = "numBlinkTime";
            this.numBlinkTime.Size = new System.Drawing.Size(63, 22);
            this.numBlinkTime.TabIndex = 68;
            this.numBlinkTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numBlinkTime.ValueChanged += new System.EventHandler(this.numBlinkTime_ValueChanged);
            // 
            // numLabelMargin
            // 
            this.numLabelMargin.Location = new System.Drawing.Point(149, 130);
            this.numLabelMargin.Margin = new System.Windows.Forms.Padding(4);
            this.numLabelMargin.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numLabelMargin.Name = "numLabelMargin";
            this.numLabelMargin.Size = new System.Drawing.Size(63, 22);
            this.numLabelMargin.TabIndex = 64;
            this.numLabelMargin.ValueChanged += new System.EventHandler(this.numLabelMargin_ValueChanged);
            // 
            // numOuterPadding
            // 
            this.numOuterPadding.Location = new System.Drawing.Point(220, 80);
            this.numOuterPadding.Margin = new System.Windows.Forms.Padding(4);
            this.numOuterPadding.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numOuterPadding.Name = "numOuterPadding";
            this.numOuterPadding.Size = new System.Drawing.Size(63, 22);
            this.numOuterPadding.TabIndex = 62;
            this.numOuterPadding.ValueChanged += new System.EventHandler(this.numOuterPadding_ValueChanged);
            // 
            // numOuterMargin
            // 
            this.numOuterMargin.Location = new System.Drawing.Point(149, 80);
            this.numOuterMargin.Margin = new System.Windows.Forms.Padding(4);
            this.numOuterMargin.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numOuterMargin.Name = "numOuterMargin";
            this.numOuterMargin.Size = new System.Drawing.Size(63, 22);
            this.numOuterMargin.TabIndex = 60;
            this.numOuterMargin.ValueChanged += new System.EventHandler(this.numOuterMargin_ValueChanged);
            // 
            // lblLabelMargin
            // 
            this.lblLabelMargin.AutoSize = true;
            this.lblLabelMargin.Location = new System.Drawing.Point(145, 111);
            this.lblLabelMargin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLabelMargin.Name = "lblLabelMargin";
            this.lblLabelMargin.Size = new System.Drawing.Size(147, 16);
            this.lblLabelMargin.TabIndex = 108;
            this.lblLabelMargin.Text = "Label Margin / Padding";
            // 
            // lblOuterMargin
            // 
            this.lblOuterMargin.AutoSize = true;
            this.lblOuterMargin.Location = new System.Drawing.Point(145, 54);
            this.lblOuterMargin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOuterMargin.Name = "lblOuterMargin";
            this.lblOuterMargin.Size = new System.Drawing.Size(147, 16);
            this.lblOuterMargin.TabIndex = 106;
            this.lblOuterMargin.Text = "Outer Margin / padding:";
            // 
            // grpState
            // 
            this.grpState.Controls.Add(this.btnBlinkError2x);
            this.grpState.Controls.Add(this.btnBlinkYellow3);
            this.grpState.Controls.Add(this.btnBlinkTwice);
            this.grpState.Controls.Add(this.btnBlinkOnce);
            this.grpState.Controls.Add(this.chkBlinking);
            this.grpState.Controls.Add(this.chkError);
            this.grpState.Controls.Add(this.chkBusy);
            this.grpState.Controls.Add(this.chkOk);
            this.grpState.Controls.Add(this.chkOff);
            this.grpState.Location = new System.Drawing.Point(5, 188);
            this.grpState.Margin = new System.Windows.Forms.Padding(4);
            this.grpState.Name = "grpState";
            this.grpState.Padding = new System.Windows.Forms.Padding(4);
            this.grpState.Size = new System.Drawing.Size(352, 165);
            this.grpState.TabIndex = 3;
            this.grpState.TabStop = false;
            this.grpState.Text = "groupBox1";
            // 
            // btnBlinkError2x
            // 
            this.btnBlinkError2x.Location = new System.Drawing.Point(189, 123);
            this.btnBlinkError2x.Margin = new System.Windows.Forms.Padding(4);
            this.btnBlinkError2x.Name = "btnBlinkError2x";
            this.btnBlinkError2x.Size = new System.Drawing.Size(155, 28);
            this.btnBlinkError2x.TabIndex = 16;
            this.btnBlinkError2x.Text = "Blink Error 2x";
            this.btnBlinkError2x.UseVisualStyleBackColor = true;
            this.btnBlinkError2x.Click += new System.EventHandler(this.btnBlinkError2x_Click);
            // 
            // btnBlinkYellow3
            // 
            this.btnBlinkYellow3.Location = new System.Drawing.Point(189, 87);
            this.btnBlinkYellow3.Margin = new System.Windows.Forms.Padding(4);
            this.btnBlinkYellow3.Name = "btnBlinkYellow3";
            this.btnBlinkYellow3.Size = new System.Drawing.Size(155, 28);
            this.btnBlinkYellow3.TabIndex = 15;
            this.btnBlinkYellow3.Text = "Blink Yellow 3x";
            this.btnBlinkYellow3.UseVisualStyleBackColor = true;
            this.btnBlinkYellow3.Click += new System.EventHandler(this.btnBlinkYellow3_Click);
            // 
            // btnBlinkTwice
            // 
            this.btnBlinkTwice.Location = new System.Drawing.Point(189, 52);
            this.btnBlinkTwice.Margin = new System.Windows.Forms.Padding(4);
            this.btnBlinkTwice.Name = "btnBlinkTwice";
            this.btnBlinkTwice.Size = new System.Drawing.Size(155, 28);
            this.btnBlinkTwice.TabIndex = 14;
            this.btnBlinkTwice.Text = "Blink twice";
            this.btnBlinkTwice.UseVisualStyleBackColor = true;
            this.btnBlinkTwice.Click += new System.EventHandler(this.btnBlinkTwice_Click);
            // 
            // btnBlinkOnce
            // 
            this.btnBlinkOnce.Location = new System.Drawing.Point(189, 16);
            this.btnBlinkOnce.Margin = new System.Windows.Forms.Padding(4);
            this.btnBlinkOnce.Name = "btnBlinkOnce";
            this.btnBlinkOnce.Size = new System.Drawing.Size(155, 28);
            this.btnBlinkOnce.TabIndex = 12;
            this.btnBlinkOnce.Text = "Blink once";
            this.btnBlinkOnce.UseVisualStyleBackColor = true;
            this.btnBlinkOnce.Click += new System.EventHandler(this.btnBlinkOnce_Click);
            // 
            // chkBlinking
            // 
            this.chkBlinking.AutoSize = true;
            this.chkBlinking.Location = new System.Drawing.Point(8, 135);
            this.chkBlinking.Margin = new System.Windows.Forms.Padding(4);
            this.chkBlinking.Name = "chkBlinking";
            this.chkBlinking.Size = new System.Drawing.Size(74, 20);
            this.chkBlinking.TabIndex = 10;
            this.chkBlinking.Text = "Blinking";
            this.chkBlinking.UseVisualStyleBackColor = true;
            this.chkBlinking.CheckedChanged += new System.EventHandler(this.chkBlinking_CheckedChanged);
            // 
            // chkError
            // 
            this.chkError.AutoSize = true;
            this.chkError.Location = new System.Drawing.Point(8, 107);
            this.chkError.Margin = new System.Windows.Forms.Padding(4);
            this.chkError.Name = "chkError";
            this.chkError.Size = new System.Drawing.Size(56, 20);
            this.chkError.TabIndex = 8;
            this.chkError.Text = "Error";
            this.chkError.UseVisualStyleBackColor = true;
            this.chkError.CheckedChanged += new System.EventHandler(this.chkError_CheckedChanged);
            // 
            // chkBusy
            // 
            this.chkBusy.AutoSize = true;
            this.chkBusy.Location = new System.Drawing.Point(8, 79);
            this.chkBusy.Margin = new System.Windows.Forms.Padding(4);
            this.chkBusy.Name = "chkBusy";
            this.chkBusy.Size = new System.Drawing.Size(57, 20);
            this.chkBusy.TabIndex = 6;
            this.chkBusy.Text = "Busy";
            this.chkBusy.UseVisualStyleBackColor = true;
            this.chkBusy.CheckedChanged += new System.EventHandler(this.chkBusy_CheckedChanged);
            // 
            // chkOk
            // 
            this.chkOk.AutoSize = true;
            this.chkOk.Location = new System.Drawing.Point(8, 51);
            this.chkOk.Margin = new System.Windows.Forms.Padding(4);
            this.chkOk.Name = "chkOk";
            this.chkOk.Size = new System.Drawing.Size(45, 20);
            this.chkOk.TabIndex = 4;
            this.chkOk.Text = "OK";
            this.chkOk.UseVisualStyleBackColor = true;
            this.chkOk.CheckedChanged += new System.EventHandler(this.chkOk_CheckedChanged);
            // 
            // chkOff
            // 
            this.chkOff.AutoSize = true;
            this.chkOff.Location = new System.Drawing.Point(8, 23);
            this.chkOff.Margin = new System.Windows.Forms.Padding(4);
            this.chkOff.Name = "chkOff";
            this.chkOff.Size = new System.Drawing.Size(43, 20);
            this.chkOff.TabIndex = 2;
            this.chkOff.Text = "Off";
            this.chkOff.UseVisualStyleBackColor = true;
            this.chkOff.CheckedChanged += new System.EventHandler(this.chkOff_CheckedChanged);
            // 
            // lblFont
            // 
            this.lblFont.AutoSize = true;
            this.lblFont.Location = new System.Drawing.Point(3, 248);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(57, 13);
            this.lblFont.TabIndex = 7;
            this.lblFont.Text = "Label font:";
            // 
            // lblLabelFont
            // 
            this.lblLabelFont.AutoSize = true;
            this.lblLabelFont.Location = new System.Drawing.Point(4, 383);
            this.lblLabelFont.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLabelFont.Name = "lblLabelFont";
            this.lblLabelFont.Size = new System.Drawing.Size(69, 16);
            this.lblLabelFont.TabIndex = 4;
            this.lblLabelFont.Text = "Label font:";
            // 
            // pnlIndicatorContainerInner
            // 
            this.pnlIndicatorContainerInner.AutoSize = true;
            this.pnlIndicatorContainerInner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlIndicatorContainerInner.BackColor = System.Drawing.SystemColors.Control;
            this.pnlIndicatorContainerInner.Controls.Add(this.indicatorLight1);
            this.pnlIndicatorContainerInner.Location = new System.Drawing.Point(31, 28);
            this.pnlIndicatorContainerInner.Margin = new System.Windows.Forms.Padding(0);
            this.pnlIndicatorContainerInner.Name = "pnlIndicatorContainerInner";
            this.pnlIndicatorContainerInner.Size = new System.Drawing.Size(103, 48);
            this.pnlIndicatorContainerInner.TabIndex = 5;
            // 
            // pnlIndicatorContainerOuter
            // 
            this.pnlIndicatorContainerOuter.AutoSize = true;
            this.pnlIndicatorContainerOuter.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlIndicatorContainerOuter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlIndicatorContainerOuter.Controls.Add(this.pnlIndicatorContainerInner);
            this.pnlIndicatorContainerOuter.Location = new System.Drawing.Point(5, 5);
            this.pnlIndicatorContainerOuter.Margin = new System.Windows.Forms.Padding(5);
            this.pnlIndicatorContainerOuter.Name = "pnlIndicatorContainerOuter";
            this.pnlIndicatorContainerOuter.Padding = new System.Windows.Forms.Padding(27, 25, 27, 25);
            this.pnlIndicatorContainerOuter.Size = new System.Drawing.Size(173, 105);
            this.pnlIndicatorContainerOuter.TabIndex = 6;
            // 
            // IndicatorLightTestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlIndicatorContainerOuter);
            this.Controls.Add(this.lblLabelFont);
            this.Controls.Add(this.fontSelectorSimple1);
            this.Controls.Add(this.grpProperties);
            this.Controls.Add(this.grpState);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(744, 448);
            this.Name = "IndicatorLightTestControl";
            this.Size = new System.Drawing.Size(744, 448);
            this.grpProperties.ResumeLayout(false);
            this.grpProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelPadding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlinkTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLabelMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOuterPadding)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOuterMargin)).EndInit();
            this.grpState.ResumeLayout(false);
            this.grpState.PerformLayout();
            this.pnlIndicatorContainerInner.ResumeLayout(false);
            this.pnlIndicatorContainerInner.PerformLayout();
            this.pnlIndicatorContainerOuter.ResumeLayout(false);
            this.pnlIndicatorContainerOuter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private IG.Forms.IndicatorLight indicatorLight1;
        private System.Windows.Forms.CheckBox chkThrowOnInvalid;
        private System.Windows.Forms.CheckBox chkOuterBorder;
        private System.Windows.Forms.CheckBox chkLabelBorder;
        private System.Windows.Forms.CheckBox chkHasOff;
        private System.Windows.Forms.CheckBox chkHasOk;
        private System.Windows.Forms.CheckBox chkHasBusy;
        private System.Windows.Forms.CheckBox chkHasError;
        private System.Windows.Forms.GroupBox grpProperties;
        private System.Windows.Forms.TextBox txtLabelText;
        private System.Windows.Forms.NumericUpDown numOuterPadding;
        private System.Windows.Forms.NumericUpDown numOuterMargin;
        private System.Windows.Forms.Label lblLabelMargin;
        private System.Windows.Forms.Label lblOuterMargin;
        private System.Windows.Forms.NumericUpDown numLabelPadding;
        private System.Windows.Forms.NumericUpDown numLabelMargin;
        private System.Windows.Forms.Button btnLabelColor;
        private System.Windows.Forms.GroupBox grpState;
        private System.Windows.Forms.CheckBox chkOff;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox chkBlinking;
        private System.Windows.Forms.CheckBox chkError;
        private System.Windows.Forms.CheckBox chkBusy;
        private System.Windows.Forms.CheckBox chkOk;
        private System.Windows.Forms.Button btnBlinkTwice;
        private System.Windows.Forms.Button btnBlinkOnce;
        private System.Windows.Forms.Label lblBlinkTime;
        private System.Windows.Forms.NumericUpDown numBlinkTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFont;
        private FontSelectorSimple fontSelectorSimple1;
        private System.Windows.Forms.Label lblLabelFont;
        private System.Windows.Forms.Button btnBlinkYellow3;
        private System.Windows.Forms.Button btnBlinkError2x;
        private System.Windows.Forms.ComboBox comboDirection;
        private System.Windows.Forms.Label lblFlowDirection;
        private System.Windows.Forms.Panel pnlIndicatorContainerInner;
        private System.Windows.Forms.Panel pnlIndicatorContainerOuter;
    }
}
