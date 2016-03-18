// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class IndicatorLightTestForm
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
            this.indicatorLightTestControl1 = new IG.Forms.IndicatorLightTestControl();
            this.SuspendLayout();
            // 
            // indicatorLightTestControl1
            // 
            this.indicatorLightTestControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.indicatorLightTestControl1.Location = new System.Drawing.Point(6, 12);
            this.indicatorLightTestControl1.MinimumSize = new System.Drawing.Size(558, 270);
            this.indicatorLightTestControl1.Name = "indicatorLightTestControl1";
            this.indicatorLightTestControl1.Size = new System.Drawing.Size(558, 350);
            this.indicatorLightTestControl1.TabIndex = 0;
            // 
            // IndicatorLightTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 411);
            this.Controls.Add(this.indicatorLightTestControl1);
            this.MinimumSize = new System.Drawing.Size(610, 450);
            this.Name = "IndicatorLightTestForm";
            this.Text = "Indicator Light Test";
            this.ResumeLayout(false);

        }

        #endregion

        private IG.Forms.IndicatorLightTestControl indicatorLightTestControl1;
    }
}