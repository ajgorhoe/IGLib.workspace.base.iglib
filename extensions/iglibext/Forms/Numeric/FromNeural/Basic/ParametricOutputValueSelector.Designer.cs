// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class OutputValueSelector
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
            this.lblParamNum = new System.Windows.Forms.Label();
            this.lblSelectParameter = new System.Windows.Forms.Label();
            this.txtValueNum = new System.Windows.Forms.NumericUpDown();
            this.comboBoxSelection = new System.Windows.Forms.ComboBox();
            this.btnDecrease = new System.Windows.Forms.Button();
            this.btnIncrease = new System.Windows.Forms.Button();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuSummary = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.txtValueNum)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblParamNum
            // 
            this.lblParamNum.AutoSize = true;
            this.lblParamNum.Location = new System.Drawing.Point(3, 20);
            this.lblParamNum.Name = "lblParamNum";
            this.lblParamNum.Size = new System.Drawing.Size(24, 13);
            this.lblParamNum.TabIndex = 100;
            this.lblParamNum.Text = "No.";
            // 
            // lblSelectParameter
            // 
            this.lblSelectParameter.AutoSize = true;
            this.lblSelectParameter.Location = new System.Drawing.Point(3, 2);
            this.lblSelectParameter.Name = "lblSelectParameter";
            this.lblSelectParameter.Size = new System.Drawing.Size(85, 13);
            this.lblSelectParameter.TabIndex = 100;
            this.lblSelectParameter.Text = "Selected output:";
            // 
            // txtValueNum
            // 
            this.txtValueNum.Location = new System.Drawing.Point(33, 18);
            this.txtValueNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtValueNum.Name = "txtValueNum";
            this.txtValueNum.Size = new System.Drawing.Size(52, 20);
            this.txtValueNum.TabIndex = 1;
            this.txtValueNum.ValueChanged += new System.EventHandler(this.txtParamNum_ValueChanged);
            // 
            // comboBoxSelection
            // 
            this.comboBoxSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSelection.FormattingEnabled = true;
            this.comboBoxSelection.Location = new System.Drawing.Point(91, 17);
            this.comboBoxSelection.Name = "comboBoxSelection";
            this.comboBoxSelection.Size = new System.Drawing.Size(141, 21);
            this.comboBoxSelection.TabIndex = 2;
            this.comboBoxSelection.TabStop = false;
            this.comboBoxSelection.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelection_SelectedIndexChanged);
            // 
            // btnDecrease
            // 
            this.btnDecrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDecrease.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnDecrease.Location = new System.Drawing.Point(238, 15);
            this.btnDecrease.Name = "btnDecrease";
            this.btnDecrease.Size = new System.Drawing.Size(28, 23);
            this.btnDecrease.TabIndex = 3;
            this.btnDecrease.TabStop = false;
            this.btnDecrease.Text = "<";
            this.btnDecrease.UseVisualStyleBackColor = true;
            this.btnDecrease.Click += new System.EventHandler(this.btnDecrease_Click);
            // 
            // btnIncrease
            // 
            this.btnIncrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIncrease.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnIncrease.Location = new System.Drawing.Point(272, 15);
            this.btnIncrease.Name = "btnIncrease";
            this.btnIncrease.Size = new System.Drawing.Size(28, 23);
            this.btnIncrease.TabIndex = 4;
            this.btnIncrease.TabStop = false;
            this.btnIncrease.Text = ">";
            this.btnIncrease.UseVisualStyleBackColor = true;
            this.btnIncrease.Click += new System.EventHandler(this.btnIncrease_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMain.Controls.Add(this.comboBoxSelection);
            this.pnlMain.Controls.Add(this.txtValueNum);
            this.pnlMain.Controls.Add(this.btnIncrease);
            this.pnlMain.Controls.Add(this.btnDecrease);
            this.pnlMain.Controls.Add(this.lblSelectParameter);
            this.pnlMain.Controls.Add(this.lblParamNum);
            this.pnlMain.Location = new System.Drawing.Point(3, 3);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(302, 45);
            this.pnlMain.TabIndex = 13;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSummary});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(126, 26);
            // 
            // menuSummary
            // 
            this.menuSummary.Name = "menuSummary";
            this.menuSummary.Size = new System.Drawing.Size(125, 22);
            this.menuSummary.Text = "Summary";
            this.menuSummary.Click += new System.EventHandler(this.menuSummary_Click);
            // 
            // OutputValueSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.pnlMain);
            this.MinimumSize = new System.Drawing.Size(268, 51);
            this.Name = "OutputValueSelector";
            this.Size = new System.Drawing.Size(308, 51);
            ((System.ComponentModel.ISupportInitialize)(this.txtValueNum)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblParamNum;
        private System.Windows.Forms.Label lblSelectParameter;
        private System.Windows.Forms.NumericUpDown txtValueNum;
        private System.Windows.Forms.ComboBox comboBoxSelection;
        private System.Windows.Forms.Button btnDecrease;
        private System.Windows.Forms.Button btnIncrease;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuSummary;
    }
}
