// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class ScalarFunctionScriptControl
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDimension = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.numDimension = new System.Windows.Forms.NumericUpDown();
            this.pnlOuter = new System.Windows.Forms.Panel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCreateFunction = new System.Windows.Forms.Button();
            this.btnValueCalculator = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSummary = new System.Windows.Forms.Button();
            this.pnlPlotButtons = new System.Windows.Forms.Panel();
            this.btnPlot1d = new System.Windows.Forms.Button();
            this.btnPlot2d = new System.Windows.Forms.Button();
            this.pnlBasicAndTitle = new System.Windows.Forms.Panel();
            this.pnlBasic = new System.Windows.Forms.Panel();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.pnlGradients = new System.Windows.Forms.Panel();
            this.txtGradients = new System.Windows.Forms.TextBox();
            this.chkGradients = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlParameters = new System.Windows.Forms.Panel();
            this.txtParameterNames = new System.Windows.Forms.TextBox();
            this.lblParameterNames = new System.Windows.Forms.Label();
            this.pnlValues = new System.Windows.Forms.Panel();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblFunctionSignature = new System.Windows.Forms.Label();
            this.fileSelector1 = new IG.Forms.FileSelector();
            this.lblValue = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numDimension)).BeginInit();
            this.pnlOuter.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlPlotButtons.SuspendLayout();
            this.pnlBasicAndTitle.SuspendLayout();
            this.pnlBasic.SuspendLayout();
            this.pnlGradients.SuspendLayout();
            this.pnlParameters.SuspendLayout();
            this.pnlValues.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.Location = new System.Drawing.Point(3, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(217, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Scalar function definition";
            // 
            // lblDimension
            // 
            this.lblDimension.AutoSize = true;
            this.lblDimension.Location = new System.Drawing.Point(194, 3);
            this.lblDimension.Name = "lblDimension";
            this.lblDimension.Size = new System.Drawing.Size(59, 13);
            this.lblDimension.TabIndex = 1;
            this.lblDimension.Text = "Dimension:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 3);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(80, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Function name:";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtName.Location = new System.Drawing.Point(6, 22);
            this.txtName.MinimumSize = new System.Drawing.Size(76, 15);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(186, 20);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "f";
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // numDimension
            // 
            this.numDimension.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.numDimension.Location = new System.Drawing.Point(197, 23);
            this.numDimension.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDimension.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDimension.Name = "numDimension";
            this.numDimension.Size = new System.Drawing.Size(56, 20);
            this.numDimension.TabIndex = 304;
            this.numDimension.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numDimension.Validated += new System.EventHandler(this.numDimension_Validated);
            // 
            // pnlOuter
            // 
            this.pnlOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOuter.Controls.Add(this.pnlTop);
            this.pnlOuter.Controls.Add(this.pnlGradients);
            this.pnlOuter.Controls.Add(this.pnlParameters);
            this.pnlOuter.Controls.Add(this.pnlValues);
            this.pnlOuter.Location = new System.Drawing.Point(3, 3);
            this.pnlOuter.MinimumSize = new System.Drawing.Size(386, 337);
            this.pnlOuter.Name = "pnlOuter";
            this.pnlOuter.Size = new System.Drawing.Size(488, 384);
            this.pnlOuter.TabIndex = 4;
            // 
            // pnlTop
            // 
            this.pnlTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlTop.Controls.Add(this.pnlButtons);
            this.pnlTop.Controls.Add(this.pnlBasicAndTitle);
            this.pnlTop.Location = new System.Drawing.Point(3, 3);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(482, 158);
            this.pnlTop.TabIndex = 11;
            this.pnlTop.TabStop = true;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlButtons.Controls.Add(this.btnCreateFunction);
            this.pnlButtons.Controls.Add(this.btnValueCalculator);
            this.pnlButtons.Controls.Add(this.btnLoad);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnSummary);
            this.pnlButtons.Controls.Add(this.pnlPlotButtons);
            this.pnlButtons.Location = new System.Drawing.Point(316, 2);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(2);
            this.pnlButtons.MinimumSize = new System.Drawing.Size(120, 134);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(163, 148);
            this.pnlButtons.TabIndex = 10;
            this.pnlButtons.TabStop = true;
            // 
            // btnCreateFunction
            // 
            this.btnCreateFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateFunction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCreateFunction.Location = new System.Drawing.Point(7, 3);
            this.btnCreateFunction.Name = "btnCreateFunction";
            this.btnCreateFunction.Size = new System.Drawing.Size(153, 23);
            this.btnCreateFunction.TabIndex = 0;
            this.btnCreateFunction.Text = "Create scalar function";
            this.btnCreateFunction.UseVisualStyleBackColor = true;
            this.btnCreateFunction.Click += new System.EventHandler(this.btnCreateFunction_Click);
            // 
            // btnValueCalculator
            // 
            this.btnValueCalculator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValueCalculator.Location = new System.Drawing.Point(7, 61);
            this.btnValueCalculator.Name = "btnValueCalculator";
            this.btnValueCalculator.Size = new System.Drawing.Size(153, 23);
            this.btnValueCalculator.TabIndex = 2;
            this.btnValueCalculator.Text = "Value Calculator";
            this.btnValueCalculator.UseVisualStyleBackColor = true;
            this.btnValueCalculator.Click += new System.EventHandler(this.btnValueCalculator_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(96, 114);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(64, 23);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(7, 114);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(64, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSummary
            // 
            this.btnSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSummary.Location = new System.Drawing.Point(7, 32);
            this.btnSummary.Name = "btnSummary";
            this.btnSummary.Size = new System.Drawing.Size(153, 23);
            this.btnSummary.TabIndex = 1;
            this.btnSummary.Text = "Summary";
            this.btnSummary.UseVisualStyleBackColor = true;
            this.btnSummary.Click += new System.EventHandler(this.btnSummary_Click);
            // 
            // pnlPlotButtons
            // 
            this.pnlPlotButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPlotButtons.Controls.Add(this.btnPlot1d);
            this.pnlPlotButtons.Controls.Add(this.btnPlot2d);
            this.pnlPlotButtons.Location = new System.Drawing.Point(7, 87);
            this.pnlPlotButtons.Margin = new System.Windows.Forms.Padding(0);
            this.pnlPlotButtons.Name = "pnlPlotButtons";
            this.pnlPlotButtons.Size = new System.Drawing.Size(153, 23);
            this.pnlPlotButtons.TabIndex = 7;
            // 
            // btnPlot1d
            // 
            this.btnPlot1d.Location = new System.Drawing.Point(0, 0);
            this.btnPlot1d.Name = "btnPlot1d";
            this.btnPlot1d.Size = new System.Drawing.Size(64, 23);
            this.btnPlot1d.TabIndex = 0;
            this.btnPlot1d.Text = "1D Plots";
            this.btnPlot1d.UseVisualStyleBackColor = true;
            this.btnPlot1d.Click += new System.EventHandler(this.btnPlot1d_Click);
            // 
            // btnPlot2d
            // 
            this.btnPlot2d.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlot2d.Location = new System.Drawing.Point(89, 0);
            this.btnPlot2d.Name = "btnPlot2d";
            this.btnPlot2d.Size = new System.Drawing.Size(64, 23);
            this.btnPlot2d.TabIndex = 1;
            this.btnPlot2d.Text = "2D Plots";
            this.btnPlot2d.UseVisualStyleBackColor = true;
            this.btnPlot2d.Click += new System.EventHandler(this.btnPlot2d_Click);
            // 
            // pnlBasicAndTitle
            // 
            this.pnlBasicAndTitle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlBasicAndTitle.Controls.Add(this.pnlBasic);
            this.pnlBasicAndTitle.Controls.Add(this.lblTitle);
            this.pnlBasicAndTitle.Location = new System.Drawing.Point(3, 3);
            this.pnlBasicAndTitle.Name = "pnlBasicAndTitle";
            this.pnlBasicAndTitle.Size = new System.Drawing.Size(302, 140);
            this.pnlBasicAndTitle.TabIndex = 11;
            this.pnlBasicAndTitle.TabStop = true;
            // 
            // pnlBasic
            // 
            this.pnlBasic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBasic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlBasic.Controls.Add(this.lblName);
            this.pnlBasic.Controls.Add(this.lblDescription);
            this.pnlBasic.Controls.Add(this.txtName);
            this.pnlBasic.Controls.Add(this.txtDescription);
            this.pnlBasic.Controls.Add(this.lblDimension);
            this.pnlBasic.Controls.Add(this.numDimension);
            this.pnlBasic.Location = new System.Drawing.Point(2, 34);
            this.pnlBasic.Margin = new System.Windows.Forms.Padding(2);
            this.pnlBasic.MinimumSize = new System.Drawing.Size(257, 90);
            this.pnlBasic.Name = "pnlBasic";
            this.pnlBasic.Size = new System.Drawing.Size(298, 104);
            this.pnlBasic.TabIndex = 9;
            this.pnlBasic.TabStop = true;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(3, 45);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtDescription.Location = new System.Drawing.Point(2, 63);
            this.txtDescription.MinimumSize = new System.Drawing.Size(76, 25);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDescription.Size = new System.Drawing.Size(295, 38);
            this.txtDescription.TabIndex = 1;
            this.txtDescription.Text = "Sum of squares of arguments.";
            this.txtDescription.Validated += new System.EventHandler(this.txtDescription_Validated);
            // 
            // pnlGradients
            // 
            this.pnlGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGradients.AutoScroll = true;
            this.pnlGradients.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlGradients.Controls.Add(this.txtGradients);
            this.pnlGradients.Controls.Add(this.chkGradients);
            this.pnlGradients.Controls.Add(this.label1);
            this.pnlGradients.Location = new System.Drawing.Point(2, 298);
            this.pnlGradients.Margin = new System.Windows.Forms.Padding(2);
            this.pnlGradients.MinimumSize = new System.Drawing.Size(2, 53);
            this.pnlGradients.Name = "pnlGradients";
            this.pnlGradients.Size = new System.Drawing.Size(483, 84);
            this.pnlGradients.TabIndex = 8;
            // 
            // txtGradients
            // 
            this.txtGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGradients.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtGradients.Location = new System.Drawing.Point(3, 28);
            this.txtGradients.MinimumSize = new System.Drawing.Size(76, 25);
            this.txtGradients.Multiline = true;
            this.txtGradients.Name = "txtGradients";
            this.txtGradients.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtGradients.Size = new System.Drawing.Size(477, 53);
            this.txtGradients.TabIndex = 1;
            this.txtGradients.Text = "2 * x1;\r\n4 * x2 ";
            this.txtGradients.Validated += new System.EventHandler(this.txtGradients_Validated);
            // 
            // chkGradients
            // 
            this.chkGradients.AutoSize = true;
            this.chkGradients.Checked = true;
            this.chkGradients.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGradients.Location = new System.Drawing.Point(259, 4);
            this.chkGradients.Name = "chkGradients";
            this.chkGradients.Size = new System.Drawing.Size(109, 17);
            this.chkGradients.TabIndex = 0;
            this.chkGradients.Text = "Gradients defined";
            this.chkGradients.UseVisualStyleBackColor = true;
            this.chkGradients.CheckedChanged += new System.EventHandler(this.chkGradients_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(216, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Function gradients separated by semicolons:";
            // 
            // pnlParameters
            // 
            this.pnlParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlParameters.AutoScroll = true;
            this.pnlParameters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlParameters.Controls.Add(this.txtParameterNames);
            this.pnlParameters.Controls.Add(this.lblParameterNames);
            this.pnlParameters.Location = new System.Drawing.Point(2, 166);
            this.pnlParameters.Margin = new System.Windows.Forms.Padding(2);
            this.pnlParameters.MinimumSize = new System.Drawing.Size(2, 53);
            this.pnlParameters.Name = "pnlParameters";
            this.pnlParameters.Size = new System.Drawing.Size(483, 53);
            this.pnlParameters.TabIndex = 8;
            // 
            // txtParameterNames
            // 
            this.txtParameterNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParameterNames.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtParameterNames.Location = new System.Drawing.Point(3, 20);
            this.txtParameterNames.MinimumSize = new System.Drawing.Size(76, 25);
            this.txtParameterNames.Multiline = true;
            this.txtParameterNames.Name = "txtParameterNames";
            this.txtParameterNames.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtParameterNames.Size = new System.Drawing.Size(477, 31);
            this.txtParameterNames.TabIndex = 0;
            this.txtParameterNames.Text = "x1, x2\r\n";
            this.txtParameterNames.Validated += new System.EventHandler(this.txtParameterNames_Validated);
            // 
            // lblParameterNames
            // 
            this.lblParameterNames.AutoSize = true;
            this.lblParameterNames.Location = new System.Drawing.Point(4, 3);
            this.lblParameterNames.Name = "lblParameterNames";
            this.lblParameterNames.Size = new System.Drawing.Size(179, 13);
            this.lblParameterNames.TabIndex = 1;
            this.lblParameterNames.Text = "Comma separated parameter names:";
            // 
            // pnlValues
            // 
            this.pnlValues.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlValues.AutoScroll = true;
            this.pnlValues.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlValues.Controls.Add(this.txtValue);
            this.pnlValues.Controls.Add(this.lblFunctionSignature);
            this.pnlValues.Controls.Add(this.fileSelector1);
            this.pnlValues.Controls.Add(this.lblValue);
            this.pnlValues.Location = new System.Drawing.Point(2, 223);
            this.pnlValues.Margin = new System.Windows.Forms.Padding(2);
            this.pnlValues.MinimumSize = new System.Drawing.Size(2, 65);
            this.pnlValues.Name = "pnlValues";
            this.pnlValues.Size = new System.Drawing.Size(483, 73);
            this.pnlValues.TabIndex = 8;
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtValue.Location = new System.Drawing.Point(2, 37);
            this.txtValue.MinimumSize = new System.Drawing.Size(76, 25);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            this.txtValue.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtValue.Size = new System.Drawing.Size(478, 33);
            this.txtValue.TabIndex = 0;
            this.txtValue.Text = "x1 * x1 + 2 * x2 * x2";
            this.txtValue.Validated += new System.EventHandler(this.txtValue_Validated);
            // 
            // lblFunctionSignature
            // 
            this.lblFunctionSignature.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFunctionSignature.AutoSize = true;
            this.lblFunctionSignature.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblFunctionSignature.Location = new System.Drawing.Point(5, 20);
            this.lblFunctionSignature.Margin = new System.Windows.Forms.Padding(3);
            this.lblFunctionSignature.Name = "lblFunctionSignature";
            this.lblFunctionSignature.Size = new System.Drawing.Size(66, 14);
            this.lblFunctionSignature.TabIndex = 2;
            this.lblFunctionSignature.Text = " f (x1, x2) = ";
            // 
            // fileSelector1
            // 
            this.fileSelector1.AllowDrop = true;
            this.fileSelector1.AllowEnvironmentVariables = true;
            this.fileSelector1.AllowExistentFiles = true;
            this.fileSelector1.AllowNonexistentFiles = true;
            this.fileSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelector1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fileSelector1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileSelector1.ErrorBackground = System.Drawing.Color.Orange;
            this.fileSelector1.ErrorForeground = System.Drawing.Color.Red;
            this.fileSelector1.FilePath = null;
            this.fileSelector1.Filter = null;
            this.fileSelector1.IsBrowsable = true;
            this.fileSelector1.IsDragAndDrop = true;
            this.fileSelector1.Location = new System.Drawing.Point(253, 4);
            this.fileSelector1.Margin = new System.Windows.Forms.Padding(4);
            this.fileSelector1.MinimumSize = new System.Drawing.Size(200, 15);
            this.fileSelector1.Name = "fileSelector1";
            this.fileSelector1.NormalBackground = System.Drawing.SystemColors.Window;
            this.fileSelector1.NormalForeground = System.Drawing.Color.Black;
            this.fileSelector1.OriginalFilePath = null;
            this.fileSelector1.Size = new System.Drawing.Size(227, 30);
            this.fileSelector1.TabIndex = 7;
            this.fileSelector1.UseAbsolutePaths = false;
            this.fileSelector1.UseRelativePaths = false;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(3, 3);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(80, 13);
            this.lblValue.TabIndex = 1;
            this.lblValue.Text = "Function value:";
            // 
            // ScalarFunctionScriptControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.pnlOuter);
            this.MinimumSize = new System.Drawing.Size(494, 390);
            this.Name = "ScalarFunctionScriptControl";
            this.Size = new System.Drawing.Size(494, 390);
            ((System.ComponentModel.ISupportInitialize)(this.numDimension)).EndInit();
            this.pnlOuter.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.pnlPlotButtons.ResumeLayout(false);
            this.pnlBasicAndTitle.ResumeLayout(false);
            this.pnlBasicAndTitle.PerformLayout();
            this.pnlBasic.ResumeLayout(false);
            this.pnlBasic.PerformLayout();
            this.pnlGradients.ResumeLayout(false);
            this.pnlGradients.PerformLayout();
            this.pnlParameters.ResumeLayout(false);
            this.pnlParameters.PerformLayout();
            this.pnlValues.ResumeLayout(false);
            this.pnlValues.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDimension;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.NumericUpDown numDimension;
        private System.Windows.Forms.Panel pnlOuter;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtParameterNames;
        private System.Windows.Forms.Label lblParameterNames;
        private System.Windows.Forms.CheckBox chkGradients;
        private System.Windows.Forms.Button btnValueCalculator;
        private System.Windows.Forms.Button btnCreateFunction;
        private System.Windows.Forms.TextBox txtGradients;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFunctionSignature;
        private System.Windows.Forms.Button btnSummary;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnPlot1d;
        private System.Windows.Forms.Button btnPlot2d;
        private System.Windows.Forms.Button btnSave;
        private FileSelector fileSelector1;
        private System.Windows.Forms.Panel pnlParameters;
        private System.Windows.Forms.Panel pnlValues;
        private System.Windows.Forms.Panel pnlGradients;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Panel pnlPlotButtons;
        private System.Windows.Forms.Panel pnlBasic;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlBasicAndTitle;
        private System.Windows.Forms.Button btnLoad;
    }
}
