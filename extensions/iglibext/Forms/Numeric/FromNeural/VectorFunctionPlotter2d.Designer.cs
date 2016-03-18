// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class VectorFunctionPlotter2d
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
            this.pnlGraph = new System.Windows.Forms.Panel();
            this.btnTestFunction = new System.Windows.Forms.Button();
            this.chkPlotImmediately = new System.Windows.Forms.CheckBox();
            this.btnIdentifyThread = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();
            this.indicatorLight1 = new IG.Forms.IndicatorLight();
            this.chkScaled = new System.Windows.Forms.CheckBox();
            this.lblTitleParametric2d = new System.Windows.Forms.Label();
            this.neuralInputSelector2 = new IG.Forms.InputParameterSelectorMinMax();
            this.neuralInputSelector1 = new IG.Forms.InputParameterSelectorMinMax();
            this.btnParamTestStart = new System.Windows.Forms.Button();
            this.neuralOutputValueSelector1 = new IG.Forms.OutputValueSelector();
            this.numNumPlotPoints2 = new System.Windows.Forms.NumericUpDown();
            this.numNumPlotPoints1 = new System.Windows.Forms.NumericUpDown();
            this.lblNumPointsCross = new System.Windows.Forms.Label();
            this.lblSelectedParameters = new System.Windows.Forms.Label();
            this.lblNumPoints = new System.Windows.Forms.Label();
            this.ParmTestFGraph = new ZedGraph.ZedGraphControl();
            this.neuralInputControl1 = new IG.Forms.InputParametersControl();
            this.pnlGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumPlotPoints2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumPlotPoints1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlGraph
            // 
            this.pnlGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGraph.Controls.Add(this.btnTestFunction);
            this.pnlGraph.Controls.Add(this.chkPlotImmediately);
            this.pnlGraph.Controls.Add(this.btnIdentifyThread);
            this.pnlGraph.Controls.Add(this.btnCheck);
            this.pnlGraph.Controls.Add(this.indicatorLight1);
            this.pnlGraph.Controls.Add(this.chkScaled);
            this.pnlGraph.Controls.Add(this.lblTitleParametric2d);
            this.pnlGraph.Controls.Add(this.neuralInputSelector2);
            this.pnlGraph.Controls.Add(this.neuralInputSelector1);
            this.pnlGraph.Controls.Add(this.btnParamTestStart);
            this.pnlGraph.Controls.Add(this.neuralOutputValueSelector1);
            this.pnlGraph.Controls.Add(this.numNumPlotPoints2);
            this.pnlGraph.Controls.Add(this.numNumPlotPoints1);
            this.pnlGraph.Controls.Add(this.lblNumPointsCross);
            this.pnlGraph.Controls.Add(this.lblSelectedParameters);
            this.pnlGraph.Controls.Add(this.lblNumPoints);
            this.pnlGraph.Controls.Add(this.ParmTestFGraph);
            this.pnlGraph.Location = new System.Drawing.Point(363, 3);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Size = new System.Drawing.Size(611, 476);
            this.pnlGraph.TabIndex = 1;
            // 
            // btnTestFunction
            // 
            this.btnTestFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestFunction.Location = new System.Drawing.Point(462, 410);
            this.btnTestFunction.Name = "btnTestFunction";
            this.btnTestFunction.Size = new System.Drawing.Size(145, 23);
            this.btnTestFunction.TabIndex = 50;
            this.btnTestFunction.Text = "Gentrate Test Function";
            this.btnTestFunction.UseVisualStyleBackColor = true;
            this.btnTestFunction.Click += new System.EventHandler(this.btnTestFunction_Click);
            // 
            // chkPlotImmediately
            // 
            this.chkPlotImmediately.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPlotImmediately.AutoSize = true;
            this.chkPlotImmediately.Location = new System.Drawing.Point(502, 329);
            this.chkPlotImmediately.Margin = new System.Windows.Forms.Padding(2);
            this.chkPlotImmediately.Name = "chkPlotImmediately";
            this.chkPlotImmediately.Size = new System.Drawing.Size(102, 17);
            this.chkPlotImmediately.TabIndex = 20;
            this.chkPlotImmediately.Text = "Plot Immediately";
            this.chkPlotImmediately.UseVisualStyleBackColor = true;
            this.chkPlotImmediately.CheckedChanged += new System.EventHandler(this.chkPlotImmediately_CheckedChanged);
            // 
            // btnIdentifyThread
            // 
            this.btnIdentifyThread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIdentifyThread.Location = new System.Drawing.Point(462, 351);
            this.btnIdentifyThread.Name = "btnIdentifyThread";
            this.btnIdentifyThread.Size = new System.Drawing.Size(142, 23);
            this.btnIdentifyThread.TabIndex = 30;
            this.btnIdentifyThread.Text = "Identify Thread";
            this.btnIdentifyThread.UseVisualStyleBackColor = true;
            this.btnIdentifyThread.Click += new System.EventHandler(this.btnIdentifyThread_Click);
            // 
            // btnCheck
            // 
            this.btnCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheck.Location = new System.Drawing.Point(462, 380);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(145, 23);
            this.btnCheck.TabIndex = 40;
            this.btnCheck.Text = "Summary";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // indicatorLight1
            // 
            this.indicatorLight1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.indicatorLight1.AutoSize = true;
            this.indicatorLight1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.indicatorLight1.BlinkIntervalMilliSeconds = 100;
            this.indicatorLight1.BorderLabel = false;
            this.indicatorLight1.BorderOut = true;
            this.indicatorLight1.ColorLabel = System.Drawing.Color.Black;
            this.indicatorLight1.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.indicatorLight1.HasBusy = true;
            this.indicatorLight1.HasError = true;
            this.indicatorLight1.HasOff = true;
            this.indicatorLight1.HasOk = true;
            this.indicatorLight1.IsBlinking = false;
            this.indicatorLight1.LabelFont = null;
            this.indicatorLight1.LabelText = null;
            this.indicatorLight1.Location = new System.Drawing.Point(577, 297);
            this.indicatorLight1.Margin = new System.Windows.Forms.Padding(4);
            this.indicatorLight1.MarginLabel = 0;
            this.indicatorLight1.MarginOut = 0;
            this.indicatorLight1.Name = "indicatorLight1";
            this.indicatorLight1.PaddingLabel = 0;
            this.indicatorLight1.PaddingOut = 0;
            this.indicatorLight1.Size = new System.Drawing.Size(18, 26);
            this.indicatorLight1.TabIndex = 35;
            this.indicatorLight1.ThrowOnInvalidSwitch = false;
            // 
            // chkScaled
            // 
            this.chkScaled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkScaled.AutoSize = true;
            this.chkScaled.Checked = true;
            this.chkScaled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkScaled.Location = new System.Drawing.Point(5, 169);
            this.chkScaled.Name = "chkScaled";
            this.chkScaled.Size = new System.Drawing.Size(127, 17);
            this.chkScaled.TabIndex = 6;
            this.chkScaled.Text = "Scale to [0, 1] ranges";
            this.chkScaled.UseVisualStyleBackColor = true;
            // 
            // lblTitleParametric2d
            // 
            this.lblTitleParametric2d.AutoSize = true;
            this.lblTitleParametric2d.Font = new System.Drawing.Font("Modern No. 20", 27.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleParametric2d.Location = new System.Drawing.Point(3, 6);
            this.lblTitleParametric2d.Name = "lblTitleParametric2d";
            this.lblTitleParametric2d.Size = new System.Drawing.Size(346, 38);
            this.lblTitleParametric2d.TabIndex = 2;
            this.lblTitleParametric2d.Text = "2D Parametric Tests";
            // 
            // neuralInputSelector2
            // 
            this.neuralInputSelector2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.neuralInputSelector2.DataDefinition = null;
            this.neuralInputSelector2.Location = new System.Drawing.Point(4, 309);
            this.neuralInputSelector2.Margin = new System.Windows.Forms.Padding(4);
            this.neuralInputSelector2.MinimumSize = new System.Drawing.Size(268, 86);
            this.neuralInputSelector2.Name = "neuralInputSelector2";
            this.neuralInputSelector2.NumInputParameters = 2;
            this.neuralInputSelector2.NumOutputValues = 2;
            this.neuralInputSelector2.Size = new System.Drawing.Size(384, 93);
            this.neuralInputSelector2.TabIndex = 10;
            this.neuralInputSelector2.TitleParameterSelection = "2nd parameter to be varied:";
            this.neuralInputSelector2.SelectedParameterIdChanged += new System.EventHandler<IG.Forms.IndexChangeEventArgs>(this.neuralInputSelector2_SelectedParameterIdChanged);
            this.neuralInputSelector2.SelectedParameterMinChanged += new System.EventHandler<IG.Forms.ValueChangeEventArgs>(this.neuralInputSelector2_SelectedParameterMinChanged);
            this.neuralInputSelector2.SelectedParameterMaxChanged += new System.EventHandler<IG.Forms.ValueChangeEventArgs>(this.neuralInputSelector2_SelectedParameterMaxChanged);
            // 
            // neuralInputSelector1
            // 
            this.neuralInputSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.neuralInputSelector1.DataDefinition = null;
            this.neuralInputSelector1.Location = new System.Drawing.Point(4, 209);
            this.neuralInputSelector1.Margin = new System.Windows.Forms.Padding(4);
            this.neuralInputSelector1.MinimumSize = new System.Drawing.Size(268, 86);
            this.neuralInputSelector1.Name = "neuralInputSelector1";
            this.neuralInputSelector1.NumInputParameters = 2;
            this.neuralInputSelector1.NumOutputValues = 2;
            this.neuralInputSelector1.Size = new System.Drawing.Size(384, 92);
            this.neuralInputSelector1.TabIndex = 8;
            this.neuralInputSelector1.TitleParameterSelection = "1st parameter to be varied:";
            this.neuralInputSelector1.SelectedParameterIdChanged += new System.EventHandler<IG.Forms.IndexChangeEventArgs>(this.neuralInputSelector1_SelectedParameterIdChanged);
            this.neuralInputSelector1.SelectedParameterMinChanged += new System.EventHandler<IG.Forms.ValueChangeEventArgs>(this.neuralInputSelector1_SelectedParameterMinChanged);
            this.neuralInputSelector1.SelectedParameterMaxChanged += new System.EventHandler<IG.Forms.ValueChangeEventArgs>(this.neuralInputSelector1_SelectedParameterMaxChanged);
            // 
            // btnParamTestStart
            // 
            this.btnParamTestStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnParamTestStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnParamTestStart.Location = new System.Drawing.Point(462, 438);
            this.btnParamTestStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnParamTestStart.Name = "btnParamTestStart";
            this.btnParamTestStart.Size = new System.Drawing.Size(146, 35);
            this.btnParamTestStart.TabIndex = 15;
            this.btnParamTestStart.Text = "Perform Parametric Test";
            this.btnParamTestStart.UseVisualStyleBackColor = true;
            this.btnParamTestStart.Click += new System.EventHandler(this.btnParmTestStart_Click);
            // 
            // neuralOutputValueSelector1
            // 
            this.neuralOutputValueSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.neuralOutputValueSelector1.DataDefinition = null;
            this.neuralOutputValueSelector1.Location = new System.Drawing.Point(4, 410);
            this.neuralOutputValueSelector1.Margin = new System.Windows.Forms.Padding(4);
            this.neuralOutputValueSelector1.MinimumSize = new System.Drawing.Size(268, 51);
            this.neuralOutputValueSelector1.Name = "neuralOutputValueSelector1";
            this.neuralOutputValueSelector1.NumInputParameters = 2;
            this.neuralOutputValueSelector1.NumOutputValues = 2;
            this.neuralOutputValueSelector1.Size = new System.Drawing.Size(384, 60);
            this.neuralOutputValueSelector1.TabIndex = 12;
            this.neuralOutputValueSelector1.SelectedOutputIdChanged += new System.EventHandler<IG.Forms.IndexChangeEventArgs>(this.neuralOutputValueSelector1_SelectedOutputIdChanged);
            // 
            // numNumPlotPoints2
            // 
            this.numNumPlotPoints2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numNumPlotPoints2.Location = new System.Drawing.Point(80, 144);
            this.numNumPlotPoints2.Margin = new System.Windows.Forms.Padding(2);
            this.numNumPlotPoints2.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numNumPlotPoints2.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numNumPlotPoints2.Name = "numNumPlotPoints2";
            this.numNumPlotPoints2.Size = new System.Drawing.Size(55, 20);
            this.numNumPlotPoints2.TabIndex = 4;
            this.numNumPlotPoints2.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.numNumPlotPoints2.ValueChanged += new System.EventHandler(this.numNumPlotPoints2_ValueChanged);
            this.numNumPlotPoints2.Validated += new System.EventHandler(this.numNumPlotPoints2_Validated);
            // 
            // numNumPlotPoints1
            // 
            this.numNumPlotPoints1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numNumPlotPoints1.Location = new System.Drawing.Point(5, 144);
            this.numNumPlotPoints1.Margin = new System.Windows.Forms.Padding(2);
            this.numNumPlotPoints1.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numNumPlotPoints1.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numNumPlotPoints1.Name = "numNumPlotPoints1";
            this.numNumPlotPoints1.Size = new System.Drawing.Size(55, 20);
            this.numNumPlotPoints1.TabIndex = 2;
            this.numNumPlotPoints1.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.numNumPlotPoints1.ValueChanged += new System.EventHandler(this.numNumPlotPoints1_ValueChanged);
            this.numNumPlotPoints1.Validated += new System.EventHandler(this.numNumPlotPoints1_Validated);
            // 
            // lblNumPointsCross
            // 
            this.lblNumPointsCross.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNumPointsCross.AutoSize = true;
            this.lblNumPointsCross.Location = new System.Drawing.Point(64, 151);
            this.lblNumPointsCross.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumPointsCross.Name = "lblNumPointsCross";
            this.lblNumPointsCross.Size = new System.Drawing.Size(12, 13);
            this.lblNumPointsCross.TabIndex = 27;
            this.lblNumPointsCross.Text = "x";
            // 
            // lblSelectedParameters
            // 
            this.lblSelectedParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectedParameters.AutoSize = true;
            this.lblSelectedParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSelectedParameters.Location = new System.Drawing.Point(2, 189);
            this.lblSelectedParameters.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelectedParameters.Name = "lblSelectedParameters";
            this.lblSelectedParameters.Size = new System.Drawing.Size(157, 16);
            this.lblSelectedParameters.TabIndex = 27;
            this.lblSelectedParameters.Text = "Selected parameters:";
            // 
            // lblNumPoints
            // 
            this.lblNumPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNumPoints.AutoSize = true;
            this.lblNumPoints.Location = new System.Drawing.Point(2, 129);
            this.lblNumPoints.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumPoints.Name = "lblNumPoints";
            this.lblNumPoints.Size = new System.Drawing.Size(207, 13);
            this.lblNumPoints.TabIndex = 27;
            this.lblNumPoints.Text = "Number of points on the graph (resolution):";
            // 
            // ParmTestFGraph
            // 
            this.ParmTestFGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ParmTestFGraph.Location = new System.Drawing.Point(232, 6);
            this.ParmTestFGraph.Margin = new System.Windows.Forms.Padding(6);
            this.ParmTestFGraph.Name = "ParmTestFGraph";
            this.ParmTestFGraph.ScrollGrace = 0D;
            this.ParmTestFGraph.ScrollMaxX = 0D;
            this.ParmTestFGraph.ScrollMaxY = 0D;
            this.ParmTestFGraph.ScrollMaxY2 = 0D;
            this.ParmTestFGraph.ScrollMinX = 0D;
            this.ParmTestFGraph.ScrollMinY = 0D;
            this.ParmTestFGraph.ScrollMinY2 = 0D;
            this.ParmTestFGraph.Size = new System.Drawing.Size(372, 183);
            this.ParmTestFGraph.TabIndex = 26;
            // 
            // neuralInputControl1
            // 
            this.neuralInputControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.neuralInputControl1.Location = new System.Drawing.Point(3, 3);
            this.neuralInputControl1.Margin = new System.Windows.Forms.Padding(4);
            this.neuralInputControl1.Name = "neuralInputControl1";
            this.neuralInputControl1.DataDefinition = null;
            this.neuralInputControl1.Size = new System.Drawing.Size(353, 477);
            this.neuralInputControl1.TabIndex = 0;
            // 
            // VectorFunctionPlotter2d
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlGraph);
            this.Controls.Add(this.neuralInputControl1);
            this.Name = "VectorFunctionPlotter2d";
            this.Size = new System.Drawing.Size(978, 483);
            this.pnlGraph.ResumeLayout(false);
            this.pnlGraph.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumPlotPoints2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumPlotPoints1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // $$ private NeuralParametricTestSelection parametricTestSelection1;
        private InputParametersControl neuralInputControl1;
        private System.Windows.Forms.Panel pnlGraph;
        private System.Windows.Forms.NumericUpDown numNumPlotPoints1;
        private System.Windows.Forms.Label lblNumPoints;
        private ZedGraph.ZedGraphControl ParmTestFGraph;
        private System.Windows.Forms.Button btnParamTestStart;
        private IG.Forms.OutputValueSelector neuralOutputValueSelector1;
        private IG.Forms.InputParameterSelectorMinMax neuralInputSelector1;
        private System.Windows.Forms.Label lblTitleParametric2d;
        private IG.Forms.InputParameterSelectorMinMax neuralInputSelector2;
        private System.Windows.Forms.NumericUpDown numNumPlotPoints2;
        private System.Windows.Forms.Label lblNumPointsCross;
        private System.Windows.Forms.Label lblSelectedParameters;
        private System.Windows.Forms.CheckBox chkScaled;

        private IG.Forms.IndicatorLight indicatorLight1;
        private System.Windows.Forms.CheckBox chkPlotImmediately;
        private System.Windows.Forms.Button btnIdentifyThread;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Button btnTestFunction;
    }
}
