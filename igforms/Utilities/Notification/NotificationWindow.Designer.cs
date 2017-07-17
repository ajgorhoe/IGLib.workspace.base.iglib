// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms.Utilities.Notification
{
    partial class NotificationWindow
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
            this.notificationPanel1 = new IG.Forms.NotificationPanel();
            this.SuspendLayout();
            // 
            // notificationPanel1
            // 
            this.notificationPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.notificationPanel1.AutoScroll = true;
            this.notificationPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.notificationPanel1.BackColor = System.Drawing.Color.LightYellow;
            this.notificationPanel1.BackgroundColor = System.Drawing.Color.LightYellow;
            this.notificationPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.notificationPanel1.FadeColor = System.Drawing.Color.Gray;
            this.notificationPanel1.FadingPortion = 0.3D;
            this.notificationPanel1.FadingTime = 0.89999999999999991D;
            this.notificationPanel1.Location = new System.Drawing.Point(0, 0);
            this.notificationPanel1.Margin = new System.Windows.Forms.Padding(3, 12, 30, 6);
            this.notificationPanel1.MessageColor = System.Drawing.Color.Black;
            this.notificationPanel1.Name = "notificationPanel1";
            this.notificationPanel1.NonFadingTime = 2.1D;
            this.notificationPanel1.ShowTime = 3D;
            this.notificationPanel1.ShowTmeMilliseconds = 3000;
            this.notificationPanel1.Size = new System.Drawing.Size(327, 161);
            this.notificationPanel1.TabIndex = 0;
            this.notificationPanel1.TimerInterval = 0D;
            this.notificationPanel1.TimerIntervalMilliseconds = 0;
            this.notificationPanel1.Title = "Notification";
            this.notificationPanel1.TitleColor = System.Drawing.Color.Blue;
            this.notificationPanel1.Type = IG.Lib.ReportType.Info;
            // 
            // NotificationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(326, 161);
            this.Controls.Add(this.notificationPanel1);
            this.Name = "NotificationWindow";
            this.Text = "NotificationWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private NotificationPanel notificationPanel1;
    }
}