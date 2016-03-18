// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class CookingTimerControl
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
            this.btnCookingSw = new System.Windows.Forms.Button();
            this.floTimer = new System.Windows.Forms.FlowLayoutPanel();
            this.btnTimerFlow = new System.Windows.Forms.Button();
            this.timerControl1 = new IG.Forms.TimerControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCookingControls = new System.Windows.Forms.Button();
            this.flowOuter = new System.Windows.Forms.FlowLayoutPanel();
            this.flowImage = new System.Windows.Forms.FlowLayoutPanel();
            this.btnImageFlow = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnOuterFlow = new System.Windows.Forms.Button();
            this.comboTimes = new System.Windows.Forms.ComboBox();
            this.floTimer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowOuter.SuspendLayout();
            this.flowImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCookingSw
            // 
            this.btnCookingSw.Location = new System.Drawing.Point(3, 3);
            this.btnCookingSw.Name = "btnCookingSw";
            this.btnCookingSw.Size = new System.Drawing.Size(120, 23);
            this.btnCookingSw.TabIndex = 0;
            this.btnCookingSw.Text = "Cooking Stopwatch";
            this.btnCookingSw.UseVisualStyleBackColor = true;
            this.btnCookingSw.Visible = false;
            // 
            // floTimer
            // 
            this.floTimer.AutoSize = true;
            this.floTimer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.floTimer.BackColor = System.Drawing.Color.DarkOrchid;
            this.floTimer.Controls.Add(this.btnTimerFlow);
            this.floTimer.Controls.Add(this.timerControl1);
            this.floTimer.Controls.Add(this.panel1);
            this.floTimer.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.floTimer.Location = new System.Drawing.Point(0, 0);
            this.floTimer.Margin = new System.Windows.Forms.Padding(0);
            this.floTimer.Name = "floTimer";
            this.floTimer.Size = new System.Drawing.Size(522, 509);
            this.floTimer.TabIndex = 1;
            // 
            // btnTimerFlow
            // 
            this.btnTimerFlow.Location = new System.Drawing.Point(3, 3);
            this.btnTimerFlow.Name = "btnTimerFlow";
            this.btnTimerFlow.Size = new System.Drawing.Size(114, 23);
            this.btnTimerFlow.TabIndex = 0;
            this.btnTimerFlow.Text = "Timer";
            this.btnTimerFlow.UseVisualStyleBackColor = true;
            this.btnTimerFlow.Visible = false;
            // 
            // timerControl1
            // 
            this.timerControl1.AutoSize = true;
            this.timerControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timerControl1.BackColor = System.Drawing.SystemColors.Control;
            this.timerControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timerControl1.CdShowHoursWhenZero = true;
            this.timerControl1.CdShowMilliSeconds = false;
            this.timerControl1.CdStartText = "Start";
            this.timerControl1.CdStopText = "Pause";
            this.timerControl1.ColorBgOk = System.Drawing.Color.Green;
            this.timerControl1.ColorBgWarning = System.Drawing.Color.Red;
            this.timerControl1.Location = new System.Drawing.Point(4, 33);
            this.timerControl1.Margin = new System.Windows.Forms.Padding(4);
            this.timerControl1.MilliSecondsSizeRatio = 0.5D;
            this.timerControl1.MinimumSize = new System.Drawing.Size(100, 100);
            this.timerControl1.Name = "timerControl1";
            this.timerControl1.Padding = new System.Windows.Forms.Padding(2);
            // this.timerControl1.PlaySystemSoundOnCountdownFinished = true;
            this.timerControl1.Size = new System.Drawing.Size(514, 232);
            this.timerControl1.SwShowHoursWhenZero = true;
            this.timerControl1.SwShowMilliSeconds = true;
            this.timerControl1.SwStartText = "Start";
            this.timerControl1.SwStopText = "Pause";
            this.timerControl1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboTimes);
            this.panel1.Controls.Add(this.btnCookingControls);
            this.panel1.Location = new System.Drawing.Point(3, 272);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(515, 234);
            this.panel1.TabIndex = 3;
            // 
            // btnCookingControls
            // 
            this.btnCookingControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCookingControls.Location = new System.Drawing.Point(398, 3);
            this.btnCookingControls.Name = "btnCookingControls";
            this.btnCookingControls.Size = new System.Drawing.Size(114, 23);
            this.btnCookingControls.TabIndex = 0;
            this.btnCookingControls.Text = "Controls";
            this.btnCookingControls.UseVisualStyleBackColor = true;
            this.btnCookingControls.Visible = false;
            // 
            // flowOuter
            // 
            this.flowOuter.AutoSize = true;
            this.flowOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowOuter.BackColor = System.Drawing.Color.LightSeaGreen;
            this.flowOuter.Controls.Add(this.floTimer);
            this.flowOuter.Controls.Add(this.flowImage);
            this.flowOuter.Controls.Add(this.btnOuterFlow);
            this.flowOuter.Location = new System.Drawing.Point(3, 32);
            this.flowOuter.Name = "flowOuter";
            this.flowOuter.Size = new System.Drawing.Size(939, 509);
            this.flowOuter.TabIndex = 1;
            // 
            // flowImage
            // 
            this.flowImage.AutoSize = true;
            this.flowImage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowImage.BackColor = System.Drawing.Color.DodgerBlue;
            this.flowImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowImage.Controls.Add(this.btnImageFlow);
            this.flowImage.Controls.Add(this.pictureBox1);
            this.flowImage.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowImage.Location = new System.Drawing.Point(522, 0);
            this.flowImage.Margin = new System.Windows.Forms.Padding(0);
            this.flowImage.Name = "flowImage";
            this.flowImage.Padding = new System.Windows.Forms.Padding(4);
            this.flowImage.Size = new System.Drawing.Size(346, 331);
            this.flowImage.TabIndex = 2;
            this.flowImage.Paint += new System.Windows.Forms.PaintEventHandler(this.flowImage_Paint);
            // 
            // btnImageFlow
            // 
            this.btnImageFlow.Location = new System.Drawing.Point(7, 7);
            this.btnImageFlow.Name = "btnImageFlow";
            this.btnImageFlow.Size = new System.Drawing.Size(65, 23);
            this.btnImageFlow.TabIndex = 0;
            this.btnImageFlow.Text = "Image";
            this.btnImageFlow.UseVisualStyleBackColor = true;
            this.btnImageFlow.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Image = global::IG.Forms.Properties.Resources.iglib_small;
            this.pictureBox1.Location = new System.Drawing.Point(7, 36);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(330, 286);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnOuterFlow
            // 
            this.btnOuterFlow.Location = new System.Drawing.Point(871, 3);
            this.btnOuterFlow.Name = "btnOuterFlow";
            this.btnOuterFlow.Size = new System.Drawing.Size(65, 23);
            this.btnOuterFlow.TabIndex = 0;
            this.btnOuterFlow.Text = "Outer";
            this.btnOuterFlow.UseVisualStyleBackColor = true;
            this.btnOuterFlow.Visible = false;
            // 
            // comboTimes
            // 
            this.comboTimes.FormattingEnabled = true;
            this.comboTimes.Items.AddRange(new object[] {
            "4 min - cooked egg, soft",
            "6 min - cooked egg, medium",
            "10 min - cooked egg, hard",
            "15 min - ",
            "20 min - ",
            "25 min - ",
            "30 min - ",
            "45 min - ",
            "1 h - ",
            "1 h 15 min - ",
            "1 h 30 min - ",
            "1 h 45 min - ",
            "2 h",
            "2 h 15 min - ",
            "2 h 30 min - "});
            this.comboTimes.Location = new System.Drawing.Point(3, 5);
            this.comboTimes.Name = "comboTimes";
            this.comboTimes.Size = new System.Drawing.Size(133, 21);
            this.comboTimes.TabIndex = 1;
            this.comboTimes.Text = "Select countdoun time!";
            this.comboTimes.Click += new System.EventHandler(this.comboTimes_Click);
            this.comboTimes.Leave += new System.EventHandler(this.comboTimes_Leave);
            // 
            // CookingTimerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowOuter);
            this.Controls.Add(this.btnCookingSw);
            this.Name = "CookingTimerControl";
            this.Size = new System.Drawing.Size(945, 544);
            this.floTimer.ResumeLayout(false);
            this.floTimer.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.flowOuter.ResumeLayout(false);
            this.flowOuter.PerformLayout();
            this.flowImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCookingSw;
        private System.Windows.Forms.FlowLayoutPanel floTimer;
        private System.Windows.Forms.Button btnTimerFlow;
        private System.Windows.Forms.FlowLayoutPanel flowOuter;
        private System.Windows.Forms.Button btnOuterFlow;
        private System.Windows.Forms.FlowLayoutPanel flowImage;
        private System.Windows.Forms.Button btnImageFlow;
        private System.Windows.Forms.PictureBox pictureBox1;
        private TimerControl timerControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCookingControls;
        private System.Windows.Forms.ComboBox comboTimes;
    }
}
