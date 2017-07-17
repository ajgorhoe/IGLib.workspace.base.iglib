// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class HashDirControl
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.notificationFrame = new IG.Forms.NotificationFrame();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnLaunchNotification = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(147, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Directory Hash";
            // 
            // notificationFrame
            // 
            this.notificationFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.notificationFrame.AutoScroll = true;
            this.notificationFrame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.notificationFrame.BackColor = System.Drawing.Color.Transparent;
            this.notificationFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.notificationFrame.Location = new System.Drawing.Point(500, 239);
            this.notificationFrame.MinimumSize = new System.Drawing.Size(100, 20);
            this.notificationFrame.Name = "notificationFrame";
            this.notificationFrame.Padding = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.notificationFrame.Size = new System.Drawing.Size(282, 238);
            this.notificationFrame.TabIndex = 1;
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(221, 3);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(280, 20);
            this.txtTitle.TabIndex = 2;
            this.txtTitle.Text = "Message title";
            this.txtTitle.TextChanged += new System.EventHandler(this.txtTitle_TextChanged);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(221, 29);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(280, 100);
            this.txtMessage.TabIndex = 2;
            this.txtMessage.Text = "Message:\r\nThis is notification\'s message.\r\nThis is another line.";
            this.txtMessage.TextChanged += new System.EventHandler(this.txtTitle_TextChanged);
            // 
            // btnLaunchNotification
            // 
            this.btnLaunchNotification.Location = new System.Drawing.Point(507, 29);
            this.btnLaunchNotification.Name = "btnLaunchNotification";
            this.btnLaunchNotification.Size = new System.Drawing.Size(75, 23);
            this.btnLaunchNotification.TabIndex = 3;
            this.btnLaunchNotification.Text = "Launch notification";
            this.btnLaunchNotification.UseVisualStyleBackColor = true;
            this.btnLaunchNotification.Click += new System.EventHandler(this.btnLaunchNotification_Click);
            // 
            // HashDirControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnLaunchNotification);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.notificationFrame);
            this.Controls.Add(this.lblTitle);
            this.MinimumSize = new System.Drawing.Size(780, 360);
            this.Name = "HashDirControl";
            this.Size = new System.Drawing.Size(785, 480);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private IG.Forms.NotificationFrame notificationFrame;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnLaunchNotification;
    }
}
