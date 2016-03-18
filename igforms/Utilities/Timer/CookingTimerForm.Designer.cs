// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class CookingTimerForm
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
            this.cookingStopwatchControl1 = new IG.Forms.CookingTimerControl();
            this.SuspendLayout();
            // 
            // cookingStopwatchControl1
            // 
            this.cookingStopwatchControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cookingStopwatchControl1.AutoSize = true;
            this.cookingStopwatchControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cookingStopwatchControl1.Location = new System.Drawing.Point(0, 4);
            this.cookingStopwatchControl1.Name = "cookingStopwatchControl1";
            this.cookingStopwatchControl1.Size = new System.Drawing.Size(874, 472);
            this.cookingStopwatchControl1.TabIndex = 0;
            // 
            // CookingTimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(877, 477);
            this.Controls.Add(this.cookingStopwatchControl1);
            this.IsMdiContainer = true;
            this.Name = "CookingTimerForm";
            this.Text = "Cooking timer";
            this.Icon = IG.Forms.Properties.Resources.ig;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CookingTimerControl cookingStopwatchControl1;
    }
}