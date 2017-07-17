// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class NotificationFrame
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
            this.notificationPanel3 = new IG.Forms.NotificationPanel();
            this.notificationPanel4 = new IG.Forms.NotificationPanel();
            this.SuspendLayout();
            // 
            // notificationPanel3
            // 
            this.notificationPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.notificationPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.notificationPanel3.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.notificationPanel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.notificationPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.notificationPanel3.ErrorColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.notificationPanel3.ErrorTitle = "Error";
            this.notificationPanel3.FadeColor = System.Drawing.Color.Gray;
            this.notificationPanel3.FadingPortion = 0.3D;
            this.notificationPanel3.FadingTime = 0.89999999999999991D;
            this.notificationPanel3.InfoTitle = "Info";
            this.notificationPanel3.Location = new System.Drawing.Point(2, 2);
            this.notificationPanel3.Margin = new System.Windows.Forms.Padding(3, 12, 30, 6);
            this.notificationPanel3.Message = "XX Notification Message in Panel 1.\\nMessage can be multiline:\\n  Line1\\n  Line2\\" +
    "n...";
            this.notificationPanel3.MessageColor = System.Drawing.Color.Black;
            this.notificationPanel3.Name = "notificationPanel3";
            this.notificationPanel3.NonFadingTime = 2.1D;
            this.notificationPanel3.OtherBackgroundColor = System.Drawing.Color.LightYellow;
            this.notificationPanel3.OtherTitle = "Notification";
            this.notificationPanel3.ShowTime = 3D;
            this.notificationPanel3.ShowTmeMilliseconds = 3000;
            this.notificationPanel3.Size = new System.Drawing.Size(309, 128);
            this.notificationPanel3.TabIndex = 0;
            this.notificationPanel3.TimerInterval = 0D;
            this.notificationPanel3.TimerIntervalMilliseconds = 0;
            this.notificationPanel3.Title = "Warning";
            this.notificationPanel3.TitleColor = System.Drawing.Color.Blue;
            this.notificationPanel3.Type = IG.Lib.ReportType.Warning;
            this.notificationPanel3.WarningColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.notificationPanel3.WarningTitle = "Warning";
            // 
            // notificationPanel4
            // 
            this.notificationPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.notificationPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.notificationPanel4.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.notificationPanel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.notificationPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.notificationPanel4.ErrorColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.notificationPanel4.ErrorTitle = "Error";
            this.notificationPanel4.FadeColor = System.Drawing.Color.Gray;
            this.notificationPanel4.FadingPortion = 0.3D;
            this.notificationPanel4.FadingTime = 0.89999999999999991D;
            this.notificationPanel4.InfoTitle = "Info";
            this.notificationPanel4.Location = new System.Drawing.Point(2, 130);
            this.notificationPanel4.Margin = new System.Windows.Forms.Padding(3, 12, 30, 6);
            this.notificationPanel4.Message = "<< Message. >>";
            this.notificationPanel4.MessageColor = System.Drawing.Color.Black;
            this.notificationPanel4.Name = "notificationPanel4";
            this.notificationPanel4.NonFadingTime = 2.1D;
            this.notificationPanel4.OtherBackgroundColor = System.Drawing.Color.LightYellow;
            this.notificationPanel4.OtherTitle = "Notification";
            this.notificationPanel4.ShowTime = 3D;
            this.notificationPanel4.ShowTmeMilliseconds = 3000;
            this.notificationPanel4.Size = new System.Drawing.Size(309, 132);
            this.notificationPanel4.TabIndex = 1;
            this.notificationPanel4.TimerInterval = 0D;
            this.notificationPanel4.TimerIntervalMilliseconds = 0;
            this.notificationPanel4.Title = "Error";
            this.notificationPanel4.TitleColor = System.Drawing.Color.Blue;
            this.notificationPanel4.Type = IG.Lib.ReportType.Error;
            this.notificationPanel4.WarningColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(165)))), ((int)(((byte)(0)))));
            this.notificationPanel4.WarningTitle = "Warning";
            // 
            // NotificationFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.notificationPanel4);
            this.Controls.Add(this.notificationPanel3);
            this.Name = "NotificationFrame";
            this.Padding = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.Size = new System.Drawing.Size(313, 406);
            this.ResumeLayout(false);

        }

        #endregion

        private NotificationPanel notificationFrame1;
        private NotificationPanel notificationFrame2;
        private NotificationPanel notificationPanel1;
        private NotificationPanel notificationPanel2;
        private NotificationPanel notificationPanel3;
        private NotificationPanel notificationPanel4;
    }
}
