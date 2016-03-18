// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class InputParameterSelectorMinMax
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
            this.txtParamNum = new System.Windows.Forms.NumericUpDown();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.comboBoxSelection = new System.Windows.Forms.ComboBox();
            this.lblMax = new System.Windows.Forms.Label();
            this.lblParamNum = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblSelectParameter = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnIncrease = new System.Windows.Forms.Button();
            this.btnDecrease = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuSummary = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.txtParamNum)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtParamNum
            // 
            this.txtParamNum.Location = new System.Drawing.Point(33, 18);
            this.txtParamNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtParamNum.Name = "txtParamNum";
            this.txtParamNum.Size = new System.Drawing.Size(58, 20);
            this.txtParamNum.TabIndex = 1;
            this.txtParamNum.ValueChanged += new System.EventHandler(this.txtParamNum_ValueChanged);
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(109, 57);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(100, 20);
            this.txtMaxValue.TabIndex = 3;
            this.txtMaxValue.TextChanged += new System.EventHandler(this.txtMaxValue_TextChanged);
            this.txtMaxValue.Validated += new System.EventHandler(this.txtMaxValue_Validated);
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(3, 57);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(100, 20);
            this.txtMinValue.TabIndex = 2;
            this.txtMinValue.TextChanged += new System.EventHandler(this.txtMinValue_TextChanged);
            this.txtMinValue.Validated += new System.EventHandler(this.txtMinValue_Validated);
            // 
            // comboBoxSelection
            // 
            this.comboBoxSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSelection.FormattingEnabled = true;
            this.comboBoxSelection.Location = new System.Drawing.Point(97, 18);
            this.comboBoxSelection.Name = "comboBoxSelection";
            this.comboBoxSelection.Size = new System.Drawing.Size(134, 21);
            this.comboBoxSelection.TabIndex = 4;
            this.comboBoxSelection.TabStop = false;
            this.comboBoxSelection.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelection_SelectedIndexChanged);
            // 
            // lblMax
            // 
            this.lblMax.AutoSize = true;
            this.lblMax.Location = new System.Drawing.Point(106, 42);
            this.lblMax.Name = "lblMax";
            this.lblMax.Size = new System.Drawing.Size(33, 13);
            this.lblMax.TabIndex = 6;
            this.lblMax.Text = "Max.:";
            // 
            // lblParamNum
            // 
            this.lblParamNum.AutoSize = true;
            this.lblParamNum.Location = new System.Drawing.Point(3, 20);
            this.lblParamNum.Name = "lblParamNum";
            this.lblParamNum.Size = new System.Drawing.Size(24, 13);
            this.lblParamNum.TabIndex = 5;
            this.lblParamNum.Text = "No.";
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(3, 41);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(30, 13);
            this.lblFrom.TabIndex = 8;
            this.lblFrom.Text = "Min.:";
            // 
            // lblSelectParameter
            // 
            this.lblSelectParameter.AutoSize = true;
            this.lblSelectParameter.Location = new System.Drawing.Point(3, 2);
            this.lblSelectParameter.Name = "lblSelectParameter";
            this.lblSelectParameter.Size = new System.Drawing.Size(117, 13);
            this.lblSelectParameter.TabIndex = 7;
            this.lblSelectParameter.Text = "Parameter to be varied:";
            // 
            // pnlMain
            // 
            this.pnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMain.Controls.Add(this.comboBoxSelection);
            this.pnlMain.Controls.Add(this.txtParamNum);
            this.pnlMain.Controls.Add(this.txtMaxValue);
            this.pnlMain.Controls.Add(this.txtMinValue);
            this.pnlMain.Controls.Add(this.btnIncrease);
            this.pnlMain.Controls.Add(this.btnDecrease);
            this.pnlMain.Controls.Add(this.lblSelectParameter);
            this.pnlMain.Controls.Add(this.lblFrom);
            this.pnlMain.Controls.Add(this.lblParamNum);
            this.pnlMain.Controls.Add(this.lblMax);
            this.pnlMain.Location = new System.Drawing.Point(3, 3);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(302, 83);
            this.pnlMain.TabIndex = 13;
            // 
            // btnIncrease
            // 
            this.btnIncrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIncrease.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnIncrease.Location = new System.Drawing.Point(271, 18);
            this.btnIncrease.Name = "btnIncrease";
            this.btnIncrease.Size = new System.Drawing.Size(28, 23);
            this.btnIncrease.TabIndex = 6;
            this.btnIncrease.TabStop = false;
            this.btnIncrease.Text = ">";
            this.btnIncrease.UseVisualStyleBackColor = true;
            this.btnIncrease.Click += new System.EventHandler(this.btnIncrease_Click);
            // 
            // btnDecrease
            // 
            this.btnDecrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDecrease.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnDecrease.Location = new System.Drawing.Point(237, 18);
            this.btnDecrease.Name = "btnDecrease";
            this.btnDecrease.Size = new System.Drawing.Size(28, 23);
            this.btnDecrease.TabIndex = 5;
            this.btnDecrease.TabStop = false;
            this.btnDecrease.Text = "<";
            this.btnDecrease.UseVisualStyleBackColor = true;
            this.btnDecrease.Click += new System.EventHandler(this.btnDecrease_Click);
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
            // InputParameterSelectorMinMax
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.pnlMain);
            this.MinimumSize = new System.Drawing.Size(268, 86);
            this.Name = "InputParameterSelectorMinMax";
            this.Size = new System.Drawing.Size(308, 89);
            ((System.ComponentModel.ISupportInitialize)(this.txtParamNum)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown txtParamNum;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.ComboBox comboBoxSelection;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.Label lblParamNum;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblSelectParameter;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnIncrease;
        private System.Windows.Forms.Button btnDecrease;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuSummary;
    }
}
