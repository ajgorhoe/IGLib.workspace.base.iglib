// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class FontSelectorSimple
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        //private System.ComponentModel.IContainer components = null;

        ///// <summary> 
        ///// Clean up any resources being used.
        ///// </summary>
        ///// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Component Designer generated code 

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpenDialog = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.lblFontDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpenDialog
            // 
            this.btnOpenDialog.Location = new System.Drawing.Point(0, 0);
            this.btnOpenDialog.Name = "btnOpenDialog";
            this.btnOpenDialog.Size = new System.Drawing.Size(27, 23);
            this.btnOpenDialog.TabIndex = 1;
            this.btnOpenDialog.Text = "...";
            this.btnOpenDialog.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOpenDialog.UseVisualStyleBackColor = true;
            this.btnOpenDialog.Click += new System.EventHandler(this.btnOpenDialog_Click);
            // 
            // lblFontDescription
            // 
            this.lblFontDescription.AutoSize = true;
            this.lblFontDescription.Location = new System.Drawing.Point(33, 4);
            this.lblFontDescription.Name = "lblFontDescription";
            this.lblFontDescription.Size = new System.Drawing.Size(99, 15);
            this.lblFontDescription.TabIndex = 100;
            this.lblFontDescription.Text = "<  selected font >";
            this.lblFontDescription.Click += new System.EventHandler(this.lblFontDescription_Click);
            // 
            // FontSelectorSimple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.lblFontDescription);
            this.Controls.Add(this.btnOpenDialog);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximumSize = new System.Drawing.Size(1200, 0);
            this.MinimumSize = new System.Drawing.Size(150, 15);
            this.Name = "FontSelectorSimple";
            this.Size = new System.Drawing.Size(240, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOpenDialog;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Label lblFontDescription;
    }
} 
