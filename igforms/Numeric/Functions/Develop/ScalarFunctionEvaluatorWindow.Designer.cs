﻿// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{


    partial class ScalarFunctionEvaluatorWindow
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
            this.scalarFunctionEvaluatorControl1 = new IG.Forms.ScalarFunctionEvaluatorControl();
            this.SuspendLayout();
            // 
            // scalarFunctionEvaluatorControl1
            // 
            this.scalarFunctionEvaluatorControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scalarFunctionEvaluatorControl1.Function = null;
            this.scalarFunctionEvaluatorControl1.Location = new System.Drawing.Point(0, 0);
            this.scalarFunctionEvaluatorControl1.Name = "scalarFunctionEvaluatorControl1";
            this.scalarFunctionEvaluatorControl1.NumParameters = 0;
            this.scalarFunctionEvaluatorControl1.ParameterNames = null;
            this.scalarFunctionEvaluatorControl1.ParameterValues = null;
            this.scalarFunctionEvaluatorControl1.Size = new System.Drawing.Size(560, 535);
            this.scalarFunctionEvaluatorControl1.TabIndex = 0;
            // 
            // ScalarFunctionEvaluatorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 544);
            this.Controls.Add(this.scalarFunctionEvaluatorControl1);
            this.Name = "ScalarFunctionEvaluatorWindow";
            this.Text = "Scalar Function Evaluator";
            this.ResumeLayout(false);

        }

        #endregion

        private ScalarFunctionEvaluatorControl scalarFunctionEvaluatorControl1;

    }
}