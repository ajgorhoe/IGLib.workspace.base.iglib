// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class IndicatorLight
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
            this.flowOuter = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlLight = new System.Windows.Forms.Panel();
            this.lblText = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.flowOuter.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowOuter
            // 
            this.flowOuter.AutoSize = true;
            this.flowOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowOuter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowOuter.Controls.Add(this.pnlLight);
            this.flowOuter.Controls.Add(this.lblText);
            this.flowOuter.Location = new System.Drawing.Point(3, 3);
            this.flowOuter.Margin = new System.Windows.Forms.Padding(2);
            this.flowOuter.Name = "flowOuter";
            this.flowOuter.Padding = new System.Windows.Forms.Padding(2);
            this.flowOuter.Size = new System.Drawing.Size(95, 29);
            this.flowOuter.TabIndex = 0;
            // 
            // pnlLight
            // 
            this.pnlLight.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlLight.Location = new System.Drawing.Point(2, 2);
            this.pnlLight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlLight.Name = "pnlLight";
            this.pnlLight.Size = new System.Drawing.Size(13, 21);
            this.pnlLight.TabIndex = 1;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblText.Location = new System.Drawing.Point(17, 4);
            this.lblText.Margin = new System.Windows.Forms.Padding(2);
            this.lblText.Name = "lblText";
            this.lblText.Padding = new System.Windows.Forms.Padding(2);
            this.lblText.Size = new System.Drawing.Size(72, 19);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Control Light";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // IndicatorLight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowOuter);
            this.Name = "IndicatorLight";
            this.Size = new System.Drawing.Size(100, 34);
            this.flowOuter.ResumeLayout(false);
            this.flowOuter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowOuter;
        private System.Windows.Forms.Panel pnlLight;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Timer timer1;
    }
}
