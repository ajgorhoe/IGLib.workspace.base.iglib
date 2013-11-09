using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
// using ZedGraph;


namespace IG.Forms
{
	partial class GraphWindowTemplate_Old
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.grbox1 = new System.Windows.Forms.GroupBox();
            this.chkBlackBg = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grbox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbox1
            // 
            this.grbox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grbox1.Controls.Add(this.chkBlackBg);
            this.grbox1.Controls.Add(this.btnClose);
            this.grbox1.Location = new System.Drawing.Point(0, 545);
            this.grbox1.Name = "grbox1";
            this.grbox1.Size = new System.Drawing.Size(671, 45);
            this.grbox1.TabIndex = 1;
            this.grbox1.TabStop = false;
            this.grbox1.Text = "Additional controls";
            // 
            // chkBlackBg
            // 
            this.chkBlackBg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkBlackBg.AutoSize = true;
            this.chkBlackBg.Location = new System.Drawing.Point(6, 22);
            this.chkBlackBg.Name = "chkBlackBg";
            this.chkBlackBg.Size = new System.Drawing.Size(107, 17);
            this.chkBlackBg.TabIndex = 1;
            this.chkBlackBg.Text = "Blue background";
            this.chkBlackBg.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(578, 16);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close window";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(671, 539);
            this.panel1.TabIndex = 2;
            // 
            // GraphWindowTemplate_Old
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 593);
            this.Controls.Add(this.grbox1);
            this.Controls.Add(this.panel1);
            this.Name = "GraphWindowTemplate_Old";
            this.Text = "Graph Plotting Window - Old!";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grbox1.ResumeLayout(false);
            this.grbox1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion


        ///// <summary>Zedgraph control that is used for plotting.</summary>
        //private ZedGraph.ZedGraphControl _graphControl;


        private System.Windows.Forms.GroupBox grbox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkBlackBg;
	}
}

