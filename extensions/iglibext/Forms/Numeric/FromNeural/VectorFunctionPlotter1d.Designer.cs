// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class VectorFunctionPlotter1d
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
            this.indicatorLight1 = new IG.Forms.IndicatorLight();
            this.chkPlotImmediately = new System.Windows.Forms.CheckBox();
            this.btnIdentifyThread = new System.Windows.Forms.Button();
            this.btnTestFunction = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();
            this.neuralInputParameterSelector1 = new IG.Forms.InputParameterSelectorMinMax();
            this.btnParamTestStart = new System.Windows.Forms.Button();
            this.neuralOutputValueSelector1 = new IG.Forms.OutputValueSelector();
            this.numNumPlotPoints = new System.Windows.Forms.NumericUpDown();
            this.lblNumPointsLine = new System.Windows.Forms.Label();
            this.ParmTestFGraph = new ZedGraph.ZedGraphControl();
            this.neuralInputControl1 = new IG.Forms.InputParametersControl();
            this.pnlGraph.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumPlotPoints)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlGraph
            // 
            this.pnlGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGraph.Controls.Add(this.indicatorLight1);
            this.pnlGraph.Controls.Add(this.chkPlotImmediately);
            this.pnlGraph.Controls.Add(this.btnIdentifyThread);
            this.pnlGraph.Controls.Add(this.btnTestFunction);
            this.pnlGraph.Controls.Add(this.btnCheck);
            this.pnlGraph.Controls.Add(this.neuralInputParameterSelector1);
            this.pnlGraph.Controls.Add(this.btnParamTestStart);
            this.pnlGraph.Controls.Add(this.neuralOutputValueSelector1);
            this.pnlGraph.Controls.Add(this.numNumPlotPoints);
            this.pnlGraph.Controls.Add(this.lblNumPointsLine);
            this.pnlGraph.Controls.Add(this.ParmTestFGraph);
            this.pnlGraph.Location = new System.Drawing.Point(322, 3);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Size = new System.Drawing.Size(532, 413);
            this.pnlGraph.TabIndex = 1;
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
            this.indicatorLight1.Location = new System.Drawing.Point(506, 210);
            this.indicatorLight1.MarginLabel = 0;
            this.indicatorLight1.MarginOut = 0;
            this.indicatorLight1.Name = "indicatorLight1";
            this.indicatorLight1.PaddingLabel = 0;
            this.indicatorLight1.PaddingOut = 0;
            this.indicatorLight1.Size = new System.Drawing.Size(18, 26);
            this.indicatorLight1.TabIndex = 35;
            this.indicatorLight1.ThrowOnInvalidSwitch = false;
            // 
            // chkPlotImmediately
            // 
            this.chkPlotImmediately.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPlotImmediately.AutoSize = true;
            this.chkPlotImmediately.Location = new System.Drawing.Point(390, 211);
            this.chkPlotImmediately.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkPlotImmediately.Name = "chkPlotImmediately";
            this.chkPlotImmediately.Size = new System.Drawing.Size(102, 17);
            this.chkPlotImmediately.TabIndex = 8;
            this.chkPlotImmediately.Text = "Plot Immediately";
            this.chkPlotImmediately.UseVisualStyleBackColor = true;
            this.chkPlotImmediately.CheckedChanged += new System.EventHandler(this.chkPlotImmediately_CheckedChanged);
            // 
            // btnIdentifyThread
            // 
            this.btnIdentifyThread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIdentifyThread.Location = new System.Drawing.Point(394, 253);
            this.btnIdentifyThread.Name = "btnIdentifyThread";
            this.btnIdentifyThread.Size = new System.Drawing.Size(132, 23);
            this.btnIdentifyThread.TabIndex = 13;
            this.btnIdentifyThread.Text = "Identify Thread";
            this.btnIdentifyThread.UseVisualStyleBackColor = true;
            this.btnIdentifyThread.Click += new System.EventHandler(this.btnIdentifyThread_Click);
            // 
            // btnTestFunction
            // 
            this.btnTestFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestFunction.Location = new System.Drawing.Point(394, 346);
            this.btnTestFunction.Name = "btnTestFunction";
            this.btnTestFunction.Size = new System.Drawing.Size(133, 23);
            this.btnTestFunction.TabIndex = 10;
            this.btnTestFunction.Text = "Gentrate Test Function";
            this.btnTestFunction.UseVisualStyleBackColor = true;
            this.btnTestFunction.Click += new System.EventHandler(this.btnTestFunction_Click);
            // 
            // btnCheck
            // 
            this.btnCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheck.Location = new System.Drawing.Point(394, 282);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(133, 23);
            this.btnCheck.TabIndex = 12;
            this.btnCheck.Text = "Summary";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // neuralInputParameterSelector1
            // 
            this.neuralInputParameterSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.neuralInputParameterSelector1.DataDefinition = null;
            this.neuralInputParameterSelector1.Location = new System.Drawing.Point(6, 253);
            this.neuralInputParameterSelector1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.neuralInputParameterSelector1.MinimumSize = new System.Drawing.Size(201, 70);
            this.neuralInputParameterSelector1.Name = "neuralInputParameterSelector1";
            this.neuralInputParameterSelector1.NumInputParameters = 2;
            this.neuralInputParameterSelector1.NumOutputValues = 2;
            this.neuralInputParameterSelector1.Size = new System.Drawing.Size(268, 93);
            this.neuralInputParameterSelector1.TabIndex = 2;
            this.neuralInputParameterSelector1.TitleParameterSelection = "Parameter to be varied:";
            this.neuralInputParameterSelector1.SelectedParameterIdChanged += new System.EventHandler<IG.Forms.IndexChangeEventArgs>(this.neuralInputParameterSelector1_SelectedParameterIdChanged);
            this.neuralInputParameterSelector1.SelectedParameterMinChanged += new System.EventHandler<IG.Forms.ValueChangeEventArgs>(this.neuralInputParameterSelector1_SelectedParameterMinChanged);
            this.neuralInputParameterSelector1.SelectedParameterMaxChanged += new System.EventHandler<IG.Forms.ValueChangeEventArgs>(this.neuralInputParameterSelector1_SelectedParameterMaxChanged);
            // 
            // btnParamTestStart
            // 
            this.btnParamTestStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnParamTestStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnParamTestStart.Location = new System.Drawing.Point(394, 374);
            this.btnParamTestStart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnParamTestStart.Name = "btnParamTestStart";
            this.btnParamTestStart.Size = new System.Drawing.Size(134, 35);
            this.btnParamTestStart.TabIndex = 4;
            this.btnParamTestStart.Text = "Perform Plot";
            this.btnParamTestStart.UseVisualStyleBackColor = true;
            this.btnParamTestStart.Click += new System.EventHandler(this.btnParmTestStart_Click);
            // 
            // neuralOutputValueSelector1
            // 
            this.neuralOutputValueSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.neuralOutputValueSelector1.DataDefinition = null;
            this.neuralOutputValueSelector1.Location = new System.Drawing.Point(6, 348);
            this.neuralOutputValueSelector1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.neuralOutputValueSelector1.MinimumSize = new System.Drawing.Size(201, 41);
            this.neuralOutputValueSelector1.Name = "neuralOutputValueSelector1";
            this.neuralOutputValueSelector1.NumInputParameters = 2;
            this.neuralOutputValueSelector1.NumOutputValues = 2;
            this.neuralOutputValueSelector1.Size = new System.Drawing.Size(268, 60);
            this.neuralOutputValueSelector1.TabIndex = 3;
            this.neuralOutputValueSelector1.SelectedOutputIdChanged += new System.EventHandler<IG.Forms.IndexChangeEventArgs>(this.neuralOutputValueSelector1_SelectedOutputIdChanged);
            // 
            // numNumPlotPoints
            // 
            this.numNumPlotPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numNumPlotPoints.Location = new System.Drawing.Point(6, 228);
            this.numNumPlotPoints.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numNumPlotPoints.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numNumPlotPoints.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numNumPlotPoints.Name = "numNumPlotPoints";
            this.numNumPlotPoints.Size = new System.Drawing.Size(55, 20);
            this.numNumPlotPoints.TabIndex = 1;
            this.numNumPlotPoints.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numNumPlotPoints.ValueChanged += new System.EventHandler(this.numNumPlotPoints_ValueChanged);
            this.numNumPlotPoints.Validated += new System.EventHandler(this.numNumPlotPoints_Validated);
            // 
            // lblNumPointsLine
            // 
            this.lblNumPointsLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNumPointsLine.AutoSize = true;
            this.lblNumPointsLine.Location = new System.Drawing.Point(3, 213);
            this.lblNumPointsLine.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumPointsLine.Name = "lblNumPointsLine";
            this.lblNumPointsLine.Size = new System.Drawing.Size(214, 13);
            this.lblNumPointsLine.TabIndex = 27;
            this.lblNumPointsLine.Text = "Number of Points on the Curve (Resolution):";
            // 
            // ParmTestFGraph
            // 
            this.ParmTestFGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ParmTestFGraph.Location = new System.Drawing.Point(6, 6);
            this.ParmTestFGraph.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ParmTestFGraph.Name = "ParmTestFGraph";
            this.ParmTestFGraph.ScrollGrace = 0D;
            this.ParmTestFGraph.ScrollMaxX = 0D;
            this.ParmTestFGraph.ScrollMaxY = 0D;
            this.ParmTestFGraph.ScrollMaxY2 = 0D;
            this.ParmTestFGraph.ScrollMinX = 0D;
            this.ParmTestFGraph.ScrollMinY = 0D;
            this.ParmTestFGraph.ScrollMinY2 = 0D;
            this.ParmTestFGraph.Size = new System.Drawing.Size(518, 196);
            this.ParmTestFGraph.TabIndex = 26;
            // 
            // neuralInputControl1
            // 
            this.neuralInputControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.neuralInputControl1.Location = new System.Drawing.Point(3, 3);
            this.neuralInputControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.neuralInputControl1.Name = "neuralInputControl1";
            this.neuralInputControl1.DataDefinition = null;
            this.neuralInputControl1.Size = new System.Drawing.Size(313, 413);
            this.neuralInputControl1.TabIndex = 0;
            // 
            // VectorFunctionPlotter1d
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlGraph);
            this.Controls.Add(this.neuralInputControl1);
            this.Name = "VectorFunctionPlotter1d";
            this.Size = new System.Drawing.Size(857, 419);
            this.pnlGraph.ResumeLayout(false);
            this.pnlGraph.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumPlotPoints)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // $$ private NeuralParametricTestSelection parametricTestSelection1;
        private InputParametersControl neuralInputControl1;
        private System.Windows.Forms.Panel pnlGraph;
        private System.Windows.Forms.NumericUpDown numNumPlotPoints;
        private System.Windows.Forms.Label lblNumPointsLine;
        private ZedGraph.ZedGraphControl ParmTestFGraph;
        private System.Windows.Forms.Button btnParamTestStart;
        private Forms.OutputValueSelector neuralOutputValueSelector1;
        private InputParameterSelectorMinMax neuralInputParameterSelector1;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Button btnIdentifyThread;
        private System.Windows.Forms.Button btnTestFunction;
        private System.Windows.Forms.CheckBox chkPlotImmediately;
        private IndicatorLight indicatorLight1;
    }
}
