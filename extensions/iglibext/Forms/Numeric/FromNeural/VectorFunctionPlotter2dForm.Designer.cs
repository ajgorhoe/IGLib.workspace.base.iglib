﻿namespace IG.Forms
{
    partial class VectorFunctionPlotter2dForm
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
            this.neuralParametricTest1 = new IG.Forms.VectorFunctionPlotter2d();
            this.SuspendLayout();
            // 
            // neuralParametricTest1
            // 
            this.neuralParametricTest1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.neuralParametricTest1.Function = null;
            this.neuralParametricTest1.Location = new System.Drawing.Point(0, 0);
            this.neuralParametricTest1.Name = "neuralParametricTest1";
            this.neuralParametricTest1.NeuralDataDefinition = null;
            this.neuralParametricTest1.Size = new System.Drawing.Size(1125, 644);
            this.neuralParametricTest1.TabIndex = 0;
            this.neuralParametricTest1.Load += new System.EventHandler(this.neuralParametricTest1_Load);
            // 
            // VectorFunctionPlotter1dForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1177, 666);
            this.Controls.Add(this.neuralParametricTest1);
            this.Name = "VectorFunctionPlotter1dForm";
            this.Text = "ParametricForm";
            this.ResumeLayout(false);

        }

        #endregion

        private IG.Forms.VectorFunctionPlotter2d neuralParametricTest1;
    }
}