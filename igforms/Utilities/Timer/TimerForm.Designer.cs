// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class TimerForm
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
            this.stopWatchControl2 = new IG.Forms.TimerControl();
            this.SuspendLayout();
            // 
            // stopWatchControl2
            // 
            this.stopWatchControl2.AutoSize = true;
            this.stopWatchControl2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stopWatchControl2.CdShowHoursWhenZero = true;
            this.stopWatchControl2.CdShowMilliSeconds = false;
            this.stopWatchControl2.CdStartText = "Start";
            this.stopWatchControl2.CdStopText = "Pause";
            this.stopWatchControl2.ColorBgOk = System.Drawing.Color.Green;
            this.stopWatchControl2.ColorBgWarning = System.Drawing.Color.Red;
            this.stopWatchControl2.Location = new System.Drawing.Point(12, 12);
            this.stopWatchControl2.MilliSecondsSizeRatio = 0.5D;
            this.stopWatchControl2.Name = "stopWatchControl2";
            //this.stopWatchControl2.PlaySystemSoundOnCountdownFinished = true;
            this.stopWatchControl2.Size = new System.Drawing.Size(615, 262);
            this.stopWatchControl2.SwShowHoursWhenZero = true;
            this.stopWatchControl2.SwShowMilliSeconds = true;
            this.stopWatchControl2.SwStartText = "Start";
            this.stopWatchControl2.SwStopText = "Pause";
            this.stopWatchControl2.TabIndex = 0;
            // 
            // TimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(652, 307);
            this.Controls.Add(this.stopWatchControl2);
            this.Name = "TimerForm";
            this.Text = "Handy Timer";
            this.Icon = IG.Forms.Properties.Resources.ig;
            this.Load += new System.EventHandler(this.StopWatchForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private TimerControl stopWatchControl2;
    }
}