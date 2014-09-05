// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{


    partial class ScalarFunctionScriptWindow
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
            this.scalarFunctionScriptControl1 = new IG.Forms.ScalarFunctionScriptControl();
            this.SuspendLayout();
            // 
            // scalarFunctionScriptControl1
            // 
            this.scalarFunctionScriptControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scalarFunctionScriptControl1.Dimension = 2;
            this.scalarFunctionScriptControl1.FunctionName = "f";
            this.scalarFunctionScriptControl1.GradientDefinitions = new string[] {
        "2 * x1",
        "4 * x2 * Pi"};
            this.scalarFunctionScriptControl1.IsGradientDefined = false;
            this.scalarFunctionScriptControl1.Location = new System.Drawing.Point(0, 0);
            this.scalarFunctionScriptControl1.MinimumSize = new System.Drawing.Size(450, 350);
            this.scalarFunctionScriptControl1.Name = "scalarFunctionScriptControl1";
            this.scalarFunctionScriptControl1.ParameterNames = new string[] {
        "x1",
        "x2"};
            this.scalarFunctionScriptControl1.Size = new System.Drawing.Size(573, 547);
            this.scalarFunctionScriptControl1.TabIndex = 0;
            this.scalarFunctionScriptControl1.ValueDefinition = "x1 * x1 + 2 * x2 * x2";
            // 
            // ScalarFunctionScriptWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 547);
            this.Controls.Add(this.scalarFunctionScriptControl1);
            this.Name = "ScalarFunctionScriptWindow";
            this.Text = "ScalarFunctionScriptWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private ScalarFunctionScriptControl scalarFunctionScriptControl1;
    }
}