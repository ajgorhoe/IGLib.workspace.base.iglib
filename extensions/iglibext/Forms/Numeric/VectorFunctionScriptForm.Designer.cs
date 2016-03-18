// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class VectorFunctionScriptForm
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
            this.vectorFunctionScriptControl1 = new IG.Forms.VectorFunctionScriptControl();
            this.SuspendLayout();
            // 
            // vectorFunctionScriptControl1
            // 
            this.vectorFunctionScriptControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
        //    this.vectorFunctionScriptControl1.FunctionDefinitions = new string[] {
        //null,
        //null};
            this.vectorFunctionScriptControl1.FunctionDescription = "Test vector function.";
            this.vectorFunctionScriptControl1.FunctionName = "F";
            this.vectorFunctionScriptControl1.FunctionNames = new string[] {
        "f1",
        "f2"};
            this.vectorFunctionScriptControl1.InitialScalarFunctionDefinitionStrings = new string[] {
        "x1 * x1 + 2 * x2 * x2",
        " x1 * x2"};
            this.vectorFunctionScriptControl1.Location = new System.Drawing.Point(0, 0);
            this.vectorFunctionScriptControl1.Margin = new System.Windows.Forms.Padding(2);
            this.vectorFunctionScriptControl1.MinimumSize = new System.Drawing.Size(391, 343);
            this.vectorFunctionScriptControl1.Name = "vectorFunctionScriptControl1";
            this.vectorFunctionScriptControl1.NumParameters = 2;
            this.vectorFunctionScriptControl1.NumValues = 2;
            this.vectorFunctionScriptControl1.ParameterNames = new string[] {
        "x1",
        "x2"};
            this.vectorFunctionScriptControl1.Size = new System.Drawing.Size(511, 692);
            this.vectorFunctionScriptControl1.TabIndex = 0;
            // 
            // VectorFunctionScriptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(510, 693);
            this.Controls.Add(this.vectorFunctionScriptControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(526, 732);
            this.Name = "VectorFunctionScriptForm";
            this.Text = "VectorFunctionScriptForm";
            this.ResumeLayout(false);

        }

        #endregion

        private IG.Forms.VectorFunctionScriptControl vectorFunctionScriptControl1;
    }
}