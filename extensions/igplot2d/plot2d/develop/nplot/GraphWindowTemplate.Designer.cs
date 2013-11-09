// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

namespace IG.Plot2d
{

    /// <summary>Template for stand-alone graph windows.</summary>
    /// $A Igor Sept09;
    partial class GraphWindowTemplate
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

        private bool _resumeLayoutInInitializeComponent = true;

        /// <summary>Specifies whether layout of the window should be resumed in the InitializeComponents() method.
        /// Default is true.
        /// This flag can be set to false in the derived classes in order to delay the point in time where layout is resumed.
        /// In such a way the layout can be resumed in the overrideen InitCompoinent() method rather than in that method
        /// defined in this class (which is called from the original method).</summary>
        protected bool ResumeLayoutInInitializeComponent
        {
            get { return _resumeLayoutInInitializeComponent; }
            set { _resumeLayoutInInitializeComponent = value; }
        }


        /// <summary>Overridden ResumeLayout() that enables switching on or off the original ResumeLayout()
        /// dependent on the value of the ResumeLayoutInInitializeComponent flag.</summary>
        protected new void ResumeLayout()
        {
            if (ResumeLayoutInInitializeComponent)
            {
                System.Console.WriteLine("GraphWindowTemplate: ResumeLayout() will be called.");
                base.ResumeLayout();
            }
            else
            {
                System.Console.WriteLine("GraphWindowTemplate: ResumeLayout() will NOT be called (flag set to false).");
            }
        }


        /// <summary>Required method for Designer support - do not modify.
        /// the contents of this method with the code editor.</summary>
        protected virtual void InitializeComponent()
        {
            this.grpControls = new System.Windows.Forms.GroupBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlGraph = new System.Windows.Forms.Panel();
            this.grpControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpControls
            // 
            this.grpControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpControls.Controls.Add(this.btnClose);
            this.grpControls.Location = new System.Drawing.Point(0, 572);
            this.grpControls.Name = "grpControls";
            this.grpControls.Size = new System.Drawing.Size(688, 40);
            this.grpControls.TabIndex = 0;
            this.grpControls.TabStop = false;
            this.grpControls.Text = "Control Area";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(592, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close Window";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pnlGraph
            // 
            this.pnlGraph.Location = new System.Drawing.Point(0, -1);
            this.pnlGraph.Name = "pnlGraph";
            this.pnlGraph.Size = new System.Drawing.Size(688, 578);
            this.pnlGraph.TabIndex = 1;
            // 
            // GraphWindowTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 611);
            this.Controls.Add(this.grpControls);
            this.Controls.Add(this.pnlGraph);
            this.Name = "GraphWindowTemplate";
            this.Text = "GraphWindowTemplate";
            this.grpControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.GroupBox grpControls;
        protected System.Windows.Forms.Button btnClose;
        protected System.Windows.Forms.Panel pnlGraph;
    }
}