// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class VectorFunctionScriptControl
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
            IG.Num.ScalarFunctionScriptController scalarFunctionScriptController1 = new IG.Num.ScalarFunctionScriptController();
            this.pnlVector = new System.Windows.Forms.Panel();
            this.pnlFunctionDescription = new System.Windows.Forms.Panel();
            this.txtFunctionDescription = new System.Windows.Forms.TextBox();
            this.lblFunctionDescription = new System.Windows.Forms.Label();
            this.pnlFunctionName = new System.Windows.Forms.Panel();
            this.txtFunctionName = new System.Windows.Forms.TextBox();
            this.lblFunctionName = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCreateFunction = new System.Windows.Forms.Button();
            this.btnValueCalculator = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSummary = new System.Windows.Forms.Button();
            this.pnlPlotButtons = new System.Windows.Forms.Panel();
            this.btnPlot1d = new System.Windows.Forms.Button();
            this.btnPlot2d = new System.Windows.Forms.Button();
            this.pnlDimensions = new System.Windows.Forms.Panel();
            this.numNumParameters = new System.Windows.Forms.NumericUpDown();
            this.numNumFunctions = new System.Windows.Forms.NumericUpDown();
            this.numCurrentFunction = new System.Windows.Forms.NumericUpDown();
            this.lblCurrentFunctionName = new System.Windows.Forms.Label();
            this.lblNumParameters = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCurrentFunction = new System.Windows.Forms.Label();
            this.lblCurrentFunctionLabelTitle = new System.Windows.Forms.Label();
            this.pnlParameterNames = new System.Windows.Forms.Panel();
            this.txtParameterNames = new System.Windows.Forms.TextBox();
            this.lblParameterNames = new System.Windows.Forms.Label();
            this.pnlFunctionNames = new System.Windows.Forms.Panel();
            this.txtFunctionNames = new System.Windows.Forms.TextBox();
            this.lblFunctionNames = new System.Windows.Forms.Label();
            this.pnlOuter = new System.Windows.Forms.Panel();
            this.scalarFunctionScriptControl1 = new IG.Forms.ScalarFunctionScriptControl();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlVector.SuspendLayout();
            this.pnlFunctionDescription.SuspendLayout();
            this.pnlFunctionName.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.pnlPlotButtons.SuspendLayout();
            this.pnlDimensions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumFunctions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrentFunction)).BeginInit();
            this.pnlParameterNames.SuspendLayout();
            this.pnlFunctionNames.SuspendLayout();
            this.pnlOuter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlVector
            // 
            this.pnlVector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlVector.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlVector.Controls.Add(this.pnlFunctionDescription);
            this.pnlVector.Controls.Add(this.pnlFunctionName);
            this.pnlVector.Controls.Add(this.pnlButtons);
            this.pnlVector.Controls.Add(this.pnlDimensions);
            this.pnlVector.Controls.Add(this.pnlParameterNames);
            this.pnlVector.Controls.Add(this.pnlFunctionNames);
            this.pnlVector.Location = new System.Drawing.Point(2, 34);
            this.pnlVector.Margin = new System.Windows.Forms.Padding(2);
            this.pnlVector.MinimumSize = new System.Drawing.Size(270, 138);
            this.pnlVector.Name = "pnlVector";
            this.pnlVector.Size = new System.Drawing.Size(496, 246);
            this.pnlVector.TabIndex = 0;
            // 
            // pnlFunctionDescription
            // 
            this.pnlFunctionDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFunctionDescription.Controls.Add(this.txtFunctionDescription);
            this.pnlFunctionDescription.Controls.Add(this.lblFunctionDescription);
            this.pnlFunctionDescription.Location = new System.Drawing.Point(248, 3);
            this.pnlFunctionDescription.Name = "pnlFunctionDescription";
            this.pnlFunctionDescription.Size = new System.Drawing.Size(242, 42);
            this.pnlFunctionDescription.TabIndex = 15;
            // 
            // txtFunctionDescription
            // 
            this.txtFunctionDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFunctionDescription.Location = new System.Drawing.Point(3, 19);
            this.txtFunctionDescription.Name = "txtFunctionDescription";
            this.txtFunctionDescription.Size = new System.Drawing.Size(236, 20);
            this.txtFunctionDescription.TabIndex = 4;
            this.txtFunctionDescription.Text = "Test vector function.";
            this.txtFunctionDescription.Validated += new System.EventHandler(this.txtFunctionDescription_Validated);
            // 
            // lblFunctionDescription
            // 
            this.lblFunctionDescription.AutoSize = true;
            this.lblFunctionDescription.Location = new System.Drawing.Point(3, 3);
            this.lblFunctionDescription.Name = "lblFunctionDescription";
            this.lblFunctionDescription.Size = new System.Drawing.Size(107, 13);
            this.lblFunctionDescription.TabIndex = 1;
            this.lblFunctionDescription.Text = "Function Description:";
            // 
            // pnlFunctionName
            // 
            this.pnlFunctionName.Controls.Add(this.txtFunctionName);
            this.pnlFunctionName.Controls.Add(this.lblFunctionName);
            this.pnlFunctionName.Location = new System.Drawing.Point(7, 3);
            this.pnlFunctionName.Name = "pnlFunctionName";
            this.pnlFunctionName.Size = new System.Drawing.Size(235, 42);
            this.pnlFunctionName.TabIndex = 14;
            // 
            // txtFunctionName
            // 
            this.txtFunctionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFunctionName.Location = new System.Drawing.Point(3, 19);
            this.txtFunctionName.Name = "txtFunctionName";
            this.txtFunctionName.Size = new System.Drawing.Size(229, 20);
            this.txtFunctionName.TabIndex = 2;
            this.txtFunctionName.Text = "F";
            this.txtFunctionName.Validated += new System.EventHandler(this.txtFunctionName_Validated);
            // 
            // lblFunctionName
            // 
            this.lblFunctionName.AutoSize = true;
            this.lblFunctionName.Location = new System.Drawing.Point(3, 3);
            this.lblFunctionName.Name = "lblFunctionName";
            this.lblFunctionName.Size = new System.Drawing.Size(82, 13);
            this.lblFunctionName.TabIndex = 0;
            this.lblFunctionName.Text = "Function Name:";
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
            this.pnlButtons.Location = new System.Drawing.Point(327, 50);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(2);
            this.pnlButtons.MinimumSize = new System.Drawing.Size(120, 134);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(163, 148);
            this.pnlButtons.TabIndex = 13;
            // 
            // btnCreateFunction
            // 
            this.btnCreateFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateFunction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCreateFunction.Location = new System.Drawing.Point(7, 3);
            this.btnCreateFunction.Name = "btnCreateFunction";
            this.btnCreateFunction.Size = new System.Drawing.Size(153, 23);
            this.btnCreateFunction.TabIndex = 20;
            this.btnCreateFunction.Text = "Create vector function";
            this.btnCreateFunction.UseVisualStyleBackColor = true;
            this.btnCreateFunction.Click += new System.EventHandler(this.btnCreateFunction_Click);
            // 
            // btnValueCalculator
            // 
            this.btnValueCalculator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValueCalculator.Location = new System.Drawing.Point(7, 61);
            this.btnValueCalculator.Name = "btnValueCalculator";
            this.btnValueCalculator.Size = new System.Drawing.Size(153, 23);
            this.btnValueCalculator.TabIndex = 30;
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
            this.btnLoad.TabIndex = 50;
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
            this.btnSave.TabIndex = 45;
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
            this.btnSummary.TabIndex = 25;
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
            this.btnPlot1d.TabIndex = 35;
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
            this.btnPlot2d.TabIndex = 40;
            this.btnPlot2d.Text = "2D Plots";
            this.btnPlot2d.UseVisualStyleBackColor = true;
            this.btnPlot2d.Click += new System.EventHandler(this.btnPlot2d_Click);
            // 
            // pnlDimensions
            // 
            this.pnlDimensions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDimensions.Controls.Add(this.numNumParameters);
            this.pnlDimensions.Controls.Add(this.numNumFunctions);
            this.pnlDimensions.Controls.Add(this.numCurrentFunction);
            this.pnlDimensions.Controls.Add(this.lblCurrentFunctionName);
            this.pnlDimensions.Controls.Add(this.lblNumParameters);
            this.pnlDimensions.Controls.Add(this.label1);
            this.pnlDimensions.Controls.Add(this.lblCurrentFunction);
            this.pnlDimensions.Controls.Add(this.lblCurrentFunctionLabelTitle);
            this.pnlDimensions.Location = new System.Drawing.Point(2, 175);
            this.pnlDimensions.Margin = new System.Windows.Forms.Padding(2);
            this.pnlDimensions.MinimumSize = new System.Drawing.Size(270, 63);
            this.pnlDimensions.Name = "pnlDimensions";
            this.pnlDimensions.Size = new System.Drawing.Size(321, 65);
            this.pnlDimensions.TabIndex = 10;
            // 
            // numNumParameters
            // 
            this.numNumParameters.Location = new System.Drawing.Point(97, 2);
            this.numNumParameters.Margin = new System.Windows.Forms.Padding(2);
            this.numNumParameters.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numNumParameters.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumParameters.Name = "numNumParameters";
            this.numNumParameters.Size = new System.Drawing.Size(55, 20);
            this.numNumParameters.TabIndex = 10;
            this.numNumParameters.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numNumParameters.Validated += new System.EventHandler(this.numNumParameters_Validated);
            // 
            // numNumFunctions
            // 
            this.numNumFunctions.Location = new System.Drawing.Point(244, 2);
            this.numNumFunctions.Margin = new System.Windows.Forms.Padding(2);
            this.numNumFunctions.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numNumFunctions.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumFunctions.Name = "numNumFunctions";
            this.numNumFunctions.Size = new System.Drawing.Size(56, 20);
            this.numNumFunctions.TabIndex = 12;
            this.numNumFunctions.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numNumFunctions.Validated += new System.EventHandler(this.numNumFunctions_Validated);
            // 
            // numCurrentFunction
            // 
            this.numCurrentFunction.ForeColor = System.Drawing.Color.Blue;
            this.numCurrentFunction.Location = new System.Drawing.Point(5, 41);
            this.numCurrentFunction.Margin = new System.Windows.Forms.Padding(2);
            this.numCurrentFunction.Name = "numCurrentFunction";
            this.numCurrentFunction.Size = new System.Drawing.Size(55, 20);
            this.numCurrentFunction.TabIndex = 14;
            this.numCurrentFunction.ValueChanged += new System.EventHandler(this.numCurrentFunction_ValueChanged);
            this.numCurrentFunction.Validated += new System.EventHandler(this.numCurrentFunction_Validated);
            // 
            // lblCurrentFunctionName
            // 
            this.lblCurrentFunctionName.AutoSize = true;
            this.lblCurrentFunctionName.ForeColor = System.Drawing.Color.Blue;
            this.lblCurrentFunctionName.Location = new System.Drawing.Point(94, 43);
            this.lblCurrentFunctionName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCurrentFunctionName.Name = "lblCurrentFunctionName";
            this.lblCurrentFunctionName.Size = new System.Drawing.Size(90, 13);
            this.lblCurrentFunctionName.TabIndex = 8;
            this.lblCurrentFunctionName.Text = "<< not defined >>";
            // 
            // lblNumParameters
            // 
            this.lblNumParameters.AutoSize = true;
            this.lblNumParameters.Location = new System.Drawing.Point(2, 4);
            this.lblNumParameters.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumParameters.Name = "lblNumParameters";
            this.lblNumParameters.Size = new System.Drawing.Size(91, 13);
            this.lblNumParameters.TabIndex = 5;
            this.lblNumParameters.Text = "Num. Parameters:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(156, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Num. Functions:";
            // 
            // lblCurrentFunction
            // 
            this.lblCurrentFunction.AutoSize = true;
            this.lblCurrentFunction.Location = new System.Drawing.Point(2, 24);
            this.lblCurrentFunction.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCurrentFunction.Name = "lblCurrentFunction";
            this.lblCurrentFunction.Size = new System.Drawing.Size(51, 13);
            this.lblCurrentFunction.TabIndex = 2;
            this.lblCurrentFunction.Text = "Function:";
            // 
            // lblCurrentFunctionLabelTitle
            // 
            this.lblCurrentFunctionLabelTitle.AutoSize = true;
            this.lblCurrentFunctionLabelTitle.Location = new System.Drawing.Point(94, 24);
            this.lblCurrentFunctionLabelTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCurrentFunctionLabelTitle.Name = "lblCurrentFunctionLabelTitle";
            this.lblCurrentFunctionLabelTitle.Size = new System.Drawing.Size(82, 13);
            this.lblCurrentFunctionLabelTitle.TabIndex = 7;
            this.lblCurrentFunctionLabelTitle.Text = "Function Name:";
            // 
            // pnlParameterNames
            // 
            this.pnlParameterNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlParameterNames.Controls.Add(this.txtParameterNames);
            this.pnlParameterNames.Controls.Add(this.lblParameterNames);
            this.pnlParameterNames.Location = new System.Drawing.Point(2, 50);
            this.pnlParameterNames.Margin = new System.Windows.Forms.Padding(2);
            this.pnlParameterNames.MinimumSize = new System.Drawing.Size(270, 32);
            this.pnlParameterNames.Name = "pnlParameterNames";
            this.pnlParameterNames.Size = new System.Drawing.Size(321, 57);
            this.pnlParameterNames.TabIndex = 11;
            // 
            // txtParameterNames
            // 
            this.txtParameterNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParameterNames.Location = new System.Drawing.Point(5, 18);
            this.txtParameterNames.Margin = new System.Windows.Forms.Padding(2);
            this.txtParameterNames.Multiline = true;
            this.txtParameterNames.Name = "txtParameterNames";
            this.txtParameterNames.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtParameterNames.Size = new System.Drawing.Size(316, 37);
            this.txtParameterNames.TabIndex = 6;
            this.txtParameterNames.Text = "x1, x2";
            this.txtParameterNames.Validated += new System.EventHandler(this.txtParameterNames_Validated);
            // 
            // lblParameterNames
            // 
            this.lblParameterNames.AutoSize = true;
            this.lblParameterNames.Location = new System.Drawing.Point(2, 3);
            this.lblParameterNames.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblParameterNames.Name = "lblParameterNames";
            this.lblParameterNames.Size = new System.Drawing.Size(94, 13);
            this.lblParameterNames.TabIndex = 8;
            this.lblParameterNames.Text = "Parameter Names:";
            // 
            // pnlFunctionNames
            // 
            this.pnlFunctionNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFunctionNames.Controls.Add(this.txtFunctionNames);
            this.pnlFunctionNames.Controls.Add(this.lblFunctionNames);
            this.pnlFunctionNames.Location = new System.Drawing.Point(2, 111);
            this.pnlFunctionNames.Margin = new System.Windows.Forms.Padding(2);
            this.pnlFunctionNames.MinimumSize = new System.Drawing.Size(270, 32);
            this.pnlFunctionNames.Name = "pnlFunctionNames";
            this.pnlFunctionNames.Size = new System.Drawing.Size(321, 60);
            this.pnlFunctionNames.TabIndex = 12;
            // 
            // txtFunctionNames
            // 
            this.txtFunctionNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFunctionNames.Location = new System.Drawing.Point(2, 18);
            this.txtFunctionNames.Margin = new System.Windows.Forms.Padding(2);
            this.txtFunctionNames.Multiline = true;
            this.txtFunctionNames.Name = "txtFunctionNames";
            this.txtFunctionNames.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFunctionNames.Size = new System.Drawing.Size(319, 40);
            this.txtFunctionNames.TabIndex = 8;
            this.txtFunctionNames.Text = "f1, f2";
            this.txtFunctionNames.Validated += new System.EventHandler(this.txtFunctionNames_Validated);
            // 
            // lblFunctionNames
            // 
            this.lblFunctionNames.AutoSize = true;
            this.lblFunctionNames.Location = new System.Drawing.Point(2, 3);
            this.lblFunctionNames.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFunctionNames.Name = "lblFunctionNames";
            this.lblFunctionNames.Size = new System.Drawing.Size(87, 13);
            this.lblFunctionNames.TabIndex = 9;
            this.lblFunctionNames.Text = "Function Names:";
            // 
            // pnlOuter
            // 
            this.pnlOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOuter.Controls.Add(this.scalarFunctionScriptControl1);
            this.pnlOuter.Controls.Add(this.lblTitle);
            this.pnlOuter.Controls.Add(this.pnlVector);
            this.pnlOuter.Location = new System.Drawing.Point(2, 2);
            this.pnlOuter.Margin = new System.Windows.Forms.Padding(2);
            this.pnlOuter.Name = "pnlOuter";
            this.pnlOuter.Size = new System.Drawing.Size(500, 689);
            this.pnlOuter.TabIndex = 1;
            // 
            // scalarFunctionScriptControl1
            // 
            this.scalarFunctionScriptControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scalarFunctionScriptControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.scalarFunctionScriptControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scalarFunctionScriptControl1.Dimension = 2;
            scalarFunctionScriptController1.Description = null;
            scalarFunctionScriptController1.Dimension = 2;
            scalarFunctionScriptController1.GradientDefinitionStrings = new string[] {
        "2 * x1",
        "4 * x2"};
            scalarFunctionScriptController1.Name = "f";
            scalarFunctionScriptController1.ParameterNames = new string[] {
        "x1",
        "x2"};
            scalarFunctionScriptController1.ValueDefinitonString = "x1 * x1 + 2 * x2 * x2";
            this.scalarFunctionScriptControl1.FunctionController = scalarFunctionScriptController1;
            this.scalarFunctionScriptControl1.FunctionDescription = null;
            this.scalarFunctionScriptControl1.FunctionName = "f";
            this.scalarFunctionScriptControl1.GradientDefinitions = new string[] {
        "2 * x1",
        "4 * x2"};
            this.scalarFunctionScriptControl1.HasUnsavedChanges = false;
            this.scalarFunctionScriptControl1.IsGradientDefined = false;
            this.scalarFunctionScriptControl1.Location = new System.Drawing.Point(3, 285);
            this.scalarFunctionScriptControl1.MinimumSize = new System.Drawing.Size(450, 400);
            this.scalarFunctionScriptControl1.Name = "scalarFunctionScriptControl1";
            this.scalarFunctionScriptControl1.ParameterNames = new string[] {
        "x1",
        "x2"};
            this.scalarFunctionScriptControl1.Size = new System.Drawing.Size(495, 400);
            this.scalarFunctionScriptControl1.TabIndex = 60;
            this.scalarFunctionScriptControl1.ValueDefinition = "x1 * x1 + 2 * x2 * x2";
            this.scalarFunctionScriptControl1.VectorFunctionControl = null;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.Location = new System.Drawing.Point(3, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(223, 22);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Vector Function Definition";
            // 
            // VectorFunctionScriptControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlOuter);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(504, 693);
            this.Name = "VectorFunctionScriptControl";
            this.Size = new System.Drawing.Size(504, 693);
            this.pnlVector.ResumeLayout(false);
            this.pnlFunctionDescription.ResumeLayout(false);
            this.pnlFunctionDescription.PerformLayout();
            this.pnlFunctionName.ResumeLayout(false);
            this.pnlFunctionName.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlPlotButtons.ResumeLayout(false);
            this.pnlDimensions.ResumeLayout(false);
            this.pnlDimensions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumFunctions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrentFunction)).EndInit();
            this.pnlParameterNames.ResumeLayout(false);
            this.pnlParameterNames.PerformLayout();
            this.pnlFunctionNames.ResumeLayout(false);
            this.pnlFunctionNames.PerformLayout();
            this.pnlOuter.ResumeLayout(false);
            this.pnlOuter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlVector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCurrentFunction;
        private System.Windows.Forms.Panel pnlOuter;
        private System.Windows.Forms.NumericUpDown numCurrentFunction;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.NumericUpDown numNumFunctions;
        private System.Windows.Forms.Label lblCurrentFunctionLabelTitle;
        private System.Windows.Forms.NumericUpDown numNumParameters;
        private System.Windows.Forms.Label lblNumParameters;
        private System.Windows.Forms.Label lblParameterNames;
        private System.Windows.Forms.Panel pnlDimensions;
        private System.Windows.Forms.Label lblCurrentFunctionName;
        private System.Windows.Forms.Label lblFunctionNames;
        private System.Windows.Forms.Panel pnlFunctionNames;
        private System.Windows.Forms.TextBox txtFunctionNames;
        private System.Windows.Forms.Panel pnlParameterNames;
        private System.Windows.Forms.TextBox txtParameterNames;
        private IG.Forms.ScalarFunctionScriptControl scalarFunctionScriptControl1;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnCreateFunction;
        private System.Windows.Forms.Button btnValueCalculator;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSummary;
        private System.Windows.Forms.Panel pnlPlotButtons;
        private System.Windows.Forms.Button btnPlot1d;
        private System.Windows.Forms.Button btnPlot2d;
        private System.Windows.Forms.Panel pnlFunctionDescription;
        private System.Windows.Forms.TextBox txtFunctionDescription;
        private System.Windows.Forms.Label lblFunctionDescription;
        private System.Windows.Forms.Panel pnlFunctionName;
        private System.Windows.Forms.TextBox txtFunctionName;
        private System.Windows.Forms.Label lblFunctionName;
    }
}
