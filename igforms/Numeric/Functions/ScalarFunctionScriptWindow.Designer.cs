// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Neural.Applications.Functions
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
            this.scalarFunctionScriptControl1 = new IG.Neural.Applications.Functions.ScalarFunctionScriptControl();
            this.SuspendLayout();
            // 
            // scalarFunctionScriptControl1
            // 
            this.scalarFunctionScriptControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scalarFunctionScriptControl1.Location = new System.Drawing.Point(0, 0);
            this.scalarFunctionScriptControl1.Name = "scalarFunctionScriptControl1";
            this.scalarFunctionScriptControl1.Size = new System.Drawing.Size(815, 547);
            this.scalarFunctionScriptControl1.TabIndex = 0;
            // 
            // ScalarFunctionScriptWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 547);
            this.Controls.Add(this.scalarFunctionScriptControl1);
            this.Name = "ScalarFunctionScriptWindow";
            this.Text = "ScalarFunctionScriptWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private ScalarFunctionScriptControl scalarFunctionScriptControl1;
    }
}