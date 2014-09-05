// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class ScalarFunctionScriptControl
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
            this.lblDimension = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.numDimension = new System.Windows.Forms.NumericUpDown();
            this.pnlOuter = new System.Windows.Forms.Panel();
            this.chkGradients = new System.Windows.Forms.CheckBox();
            this.btnSummary = new System.Windows.Forms.Button();
            this.btnValueCalculator = new System.Windows.Forms.Button();
            this.btnCreateFunction = new System.Windows.Forms.Button();
            this.txtGradients = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.txtParameterNames = new System.Windows.Forms.TextBox();
            this.lblParameterNames = new System.Windows.Forms.Label();
            this.txtFunctionSignature = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numDimension)).BeginInit();
            this.pnlOuter.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.Location = new System.Drawing.Point(3, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(217, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Scalar function definition";
            // 
            // lblDimension
            // 
            this.lblDimension.AutoSize = true;
            this.lblDimension.Location = new System.Drawing.Point(226, 38);
            this.lblDimension.Name = "lblDimension";
            this.lblDimension.Size = new System.Drawing.Size(59, 13);
            this.lblDimension.TabIndex = 1;
            this.lblDimension.Text = "Dimension:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(4, 38);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(80, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Function name:";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtName.Location = new System.Drawing.Point(12, 57);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(203, 20);
            this.txtName.TabIndex = 2;
            this.txtName.Text = "f";
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // numDimension
            // 
            this.numDimension.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.numDimension.Location = new System.Drawing.Point(229, 57);
            this.numDimension.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDimension.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDimension.Name = "numDimension";
            this.numDimension.Size = new System.Drawing.Size(56, 20);
            this.numDimension.TabIndex = 3;
            this.numDimension.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numDimension.Validated += new System.EventHandler(this.numDimension_Validated);
            // 
            // pnlOuter
            // 
            this.pnlOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlOuter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlOuter.Controls.Add(this.chkGradients);
            this.pnlOuter.Controls.Add(this.btnSummary);
            this.pnlOuter.Controls.Add(this.btnValueCalculator);
            this.pnlOuter.Controls.Add(this.btnCreateFunction);
            this.pnlOuter.Controls.Add(this.lblTitle);
            this.pnlOuter.Controls.Add(this.numDimension);
            this.pnlOuter.Controls.Add(this.lblDimension);
            this.pnlOuter.Controls.Add(this.txtGradients);
            this.pnlOuter.Controls.Add(this.txtValue);
            this.pnlOuter.Controls.Add(this.label1);
            this.pnlOuter.Controls.Add(this.lblValue);
            this.pnlOuter.Controls.Add(this.txtParameterNames);
            this.pnlOuter.Controls.Add(this.lblParameterNames);
            this.pnlOuter.Controls.Add(this.txtFunctionSignature);
            this.pnlOuter.Controls.Add(this.txtName);
            this.pnlOuter.Controls.Add(this.lblName);
            this.pnlOuter.Location = new System.Drawing.Point(3, 3);
            this.pnlOuter.Name = "pnlOuter";
            this.pnlOuter.Size = new System.Drawing.Size(444, 429);
            this.pnlOuter.TabIndex = 4;
            // 
            // chkGradients
            // 
            this.chkGradients.AutoSize = true;
            this.chkGradients.Checked = true;
            this.chkGradients.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGradients.Location = new System.Drawing.Point(106, 233);
            this.chkGradients.Name = "chkGradients";
            this.chkGradients.Size = new System.Drawing.Size(109, 17);
            this.chkGradients.TabIndex = 5;
            this.chkGradients.Text = "Gradients defined";
            this.chkGradients.UseVisualStyleBackColor = true;
            this.chkGradients.CheckedChanged += new System.EventHandler(this.chkGradients_CheckedChanged);
            // 
            // btnSummary
            // 
            this.btnSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSummary.Location = new System.Drawing.Point(307, 154);
            this.btnSummary.Name = "btnSummary";
            this.btnSummary.Size = new System.Drawing.Size(132, 23);
            this.btnSummary.TabIndex = 4;
            this.btnSummary.Text = "Summary";
            this.btnSummary.UseVisualStyleBackColor = true;
            this.btnSummary.Click += new System.EventHandler(this.btnSummary_Click);
            // 
            // btnValueCalculator
            // 
            this.btnValueCalculator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValueCalculator.Location = new System.Drawing.Point(307, 68);
            this.btnValueCalculator.Name = "btnValueCalculator";
            this.btnValueCalculator.Size = new System.Drawing.Size(132, 23);
            this.btnValueCalculator.TabIndex = 4;
            this.btnValueCalculator.Text = "Value Calculator";
            this.btnValueCalculator.UseVisualStyleBackColor = true;
            this.btnValueCalculator.Click += new System.EventHandler(this.btnValueCalculator_Click);
            // 
            // btnCreateFunction
            // 
            this.btnCreateFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateFunction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCreateFunction.Location = new System.Drawing.Point(307, 21);
            this.btnCreateFunction.Name = "btnCreateFunction";
            this.btnCreateFunction.Size = new System.Drawing.Size(132, 41);
            this.btnCreateFunction.TabIndex = 4;
            this.btnCreateFunction.Text = "Create scalar function";
            this.btnCreateFunction.UseVisualStyleBackColor = true;
            this.btnCreateFunction.Click += new System.EventHandler(this.btnCreateFunction_Click);
            // 
            // txtGradients
            // 
            this.txtGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGradients.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtGradients.Location = new System.Drawing.Point(6, 274);
            this.txtGradients.Multiline = true;
            this.txtGradients.Name = "txtGradients";
            this.txtGradients.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGradients.Size = new System.Drawing.Size(433, 150);
            this.txtGradients.TabIndex = 2;
            this.txtGradients.Text = "2 * x1 \r\n4 * x2 ";
            this.txtGradients.Validated += new System.EventHandler(this.txtGradients_Validated);
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtValue.Location = new System.Drawing.Point(3, 178);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            this.txtValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtValue.Size = new System.Drawing.Size(436, 53);
            this.txtValue.TabIndex = 2;
            this.txtValue.Text = "x1 * x1 + 2 * x2 * x2";
            this.txtValue.Validated += new System.EventHandler(this.txtValue_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 234);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Function gradients:";
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(3, 143);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(80, 13);
            this.lblValue.TabIndex = 1;
            this.lblValue.Text = "Function value:";
            // 
            // txtParameterNames
            // 
            this.txtParameterNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParameterNames.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtParameterNames.Location = new System.Drawing.Point(7, 94);
            this.txtParameterNames.Multiline = true;
            this.txtParameterNames.Name = "txtParameterNames";
            this.txtParameterNames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtParameterNames.Size = new System.Drawing.Size(432, 39);
            this.txtParameterNames.TabIndex = 2;
            this.txtParameterNames.Text = "x1, x2\r\n";
            this.txtParameterNames.Validated += new System.EventHandler(this.txtParameterNames_Validated);
            // 
            // lblParameterNames
            // 
            this.lblParameterNames.AutoSize = true;
            this.lblParameterNames.Location = new System.Drawing.Point(4, 78);
            this.lblParameterNames.Name = "lblParameterNames";
            this.lblParameterNames.Size = new System.Drawing.Size(92, 13);
            this.lblParameterNames.TabIndex = 1;
            this.lblParameterNames.Text = "Parameter names:";
            // 
            // txtFunctionSignature
            // 
            this.txtFunctionSignature.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFunctionSignature.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtFunctionSignature.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtFunctionSignature.Location = new System.Drawing.Point(3, 159);
            this.txtFunctionSignature.Name = "txtFunctionSignature";
            this.txtFunctionSignature.ReadOnly = true;
            this.txtFunctionSignature.Size = new System.Drawing.Size(253, 13);
            this.txtFunctionSignature.TabIndex = 2;
            this.txtFunctionSignature.Text = " f (x1, x2) = ";
            // 
            // ScalarFunctionScriptControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlOuter);
            this.MinimumSize = new System.Drawing.Size(450, 350);
            this.Name = "ScalarFunctionScriptControl";
            this.Size = new System.Drawing.Size(450, 435);
            ((System.ComponentModel.ISupportInitialize)(this.numDimension)).EndInit();
            this.pnlOuter.ResumeLayout(false);
            this.pnlOuter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDimension;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.NumericUpDown numDimension;
        private System.Windows.Forms.Panel pnlOuter;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtParameterNames;
        private System.Windows.Forms.Label lblParameterNames;
        private System.Windows.Forms.CheckBox chkGradients;
        private System.Windows.Forms.Button btnValueCalculator;
        private System.Windows.Forms.Button btnCreateFunction;
        private System.Windows.Forms.TextBox txtGradients;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFunctionSignature;
        private System.Windows.Forms.Button btnSummary;
    }
}
