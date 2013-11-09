#region About
// Course material: C# for numerical methods
// Copyright (c) 2009-2010, Igor Grešovnik
#endregion About


using ZedGraph;

namespace IG.Gr
{

    /// <summary>Window form containing a Zedgraph control for plotting graphs.</summary>
    /// $A Igor Jun09 Nov09;
	partial class ZedGraphWindow
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
            this.GraphControl = new ZedGraph.ZedGraphControl();
            this.grbox1 = new System.Windows.Forms.GroupBox();
            this.chkBlackBg = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grbox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GraphControl
            // 
            this.GraphControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GraphControl.EditButtons = System.Windows.Forms.MouseButtons.Left;
            this.GraphControl.IsAutoScrollRange = true;
            this.GraphControl.Location = new System.Drawing.Point(0, 0);
            this.GraphControl.Name = "GraphControl";
            this.GraphControl.PanModifierKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)));
            this.GraphControl.ScrollGrace = 0D;
            this.GraphControl.ScrollMaxX = 0D;
            this.GraphControl.ScrollMaxY = 0D;
            this.GraphControl.ScrollMaxY2 = 0D;
            this.GraphControl.ScrollMinX = 0D;
            this.GraphControl.ScrollMinY = 0D;
            this.GraphControl.ScrollMinY2 = 0D;
            this.GraphControl.Size = new System.Drawing.Size(698, 636);
            this.GraphControl.TabIndex = 0;
            // 
            // grbox1
            // 
            this.grbox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbox1.Controls.Add(this.chkBlackBg);
            this.grbox1.Controls.Add(this.btnCancel);
            this.grbox1.Location = new System.Drawing.Point(0, 642);
            this.grbox1.Name = "grbox1";
            this.grbox1.Size = new System.Drawing.Size(698, 45);
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
            this.chkBlackBg.CheckedChanged += new System.EventHandler(this.chkBlackBg_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(605, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Close window";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.GraphControl);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(698, 636);
            this.panel1.TabIndex = 2;
            // 
            // ZedGraphWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 690);
            this.Controls.Add(this.grbox1);
            this.Controls.Add(this.panel1);
            this.Name = "ZedGraphWindow";
            this.Text = "ZedGraph Plotting Window";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.grbox1.ResumeLayout(false);
            this.grbox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion


        protected ZedGraph.ZedGraphControl _graphControl;

        /// <summary>Zedgraph control that is used for plotting.</summary>
        public ZedGraph.ZedGraphControl GraphControl
        { 
            get { return _graphControl; }
            protected set { _graphControl = value; }
        }


        private System.Windows.Forms.GroupBox grbox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkBlackBg;
	}
}

