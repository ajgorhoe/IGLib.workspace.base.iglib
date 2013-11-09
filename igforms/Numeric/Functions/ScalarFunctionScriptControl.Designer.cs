// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Neural.Applications.Functions
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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.pnlOuter = new System.Windows.Forms.Panel();
            this.lblParameterNames = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtValueMath = new System.Windows.Forms.TextBox();
            this.txtGradients = new System.Windows.Forms.TextBox();
            this.btnCreateFunction = new System.Windows.Forms.Button();
            this.btnValueCalculator = new System.Windows.Forms.Button();
            this.chkGradients = new System.Windows.Forms.CheckBox();
            this.btnSummary = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
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
            this.lblDimension.Location = new System.Drawing.Point(226, 31);
            this.lblDimension.Name = "lblDimension";
            this.lblDimension.Size = new System.Drawing.Size(59, 13);
            this.lblDimension.TabIndex = 1;
            this.lblDimension.Text = "Dimension:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(4, 31);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(80, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Function name:";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtName.Location = new System.Drawing.Point(12, 50);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(203, 20);
            this.txtName.TabIndex = 2;
            this.txtName.Text = "f";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.numericUpDown1.Location = new System.Drawing.Point(229, 50);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(56, 20);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
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
            this.pnlOuter.Controls.Add(this.numericUpDown1);
            this.pnlOuter.Controls.Add(this.lblDimension);
            this.pnlOuter.Controls.Add(this.txtGradients);
            this.pnlOuter.Controls.Add(this.textBox2);
            this.pnlOuter.Controls.Add(this.label1);
            this.pnlOuter.Controls.Add(this.lblValue);
            this.pnlOuter.Controls.Add(this.textBox1);
            this.pnlOuter.Controls.Add(this.lblParameterNames);
            this.pnlOuter.Controls.Add(this.txtValueMath);
            this.pnlOuter.Controls.Add(this.txtName);
            this.pnlOuter.Controls.Add(this.lblName);
            this.pnlOuter.Location = new System.Drawing.Point(3, 3);
            this.pnlOuter.Name = "pnlOuter";
            this.pnlOuter.Size = new System.Drawing.Size(691, 510);
            this.pnlOuter.TabIndex = 4;
            // 
            // lblParameterNames
            // 
            this.lblParameterNames.AutoSize = true;
            this.lblParameterNames.Location = new System.Drawing.Point(4, 71);
            this.lblParameterNames.Name = "lblParameterNames";
            this.lblParameterNames.Size = new System.Drawing.Size(92, 13);
            this.lblParameterNames.TabIndex = 1;
            this.lblParameterNames.Text = "Parameter names:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBox1.Location = new System.Drawing.Point(7, 87);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(634, 39);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "x1, x2\r\n";
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(3, 129);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(80, 13);
            this.lblValue.TabIndex = 1;
            this.lblValue.Text = "Function value:";
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBox2.Location = new System.Drawing.Point(3, 171);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(634, 53);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = "x1 * x1 + 2 * x2 * x2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Function gradients:";
            // 
            // txtValueMath
            // 
            this.txtValueMath.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtValueMath.Location = new System.Drawing.Point(3, 145);
            this.txtValueMath.Name = "txtValueMath";
            this.txtValueMath.ReadOnly = true;
            this.txtValueMath.Size = new System.Drawing.Size(72, 20);
            this.txtValueMath.TabIndex = 2;
            this.txtValueMath.Text = " f (x1, x2) = ";
            // 
            // txtGradients
            // 
            this.txtGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGradients.Font = new System.Drawing.Font("Times New Roman", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtGradients.Location = new System.Drawing.Point(-1, 243);
            this.txtGradients.Multiline = true;
            this.txtGradients.Name = "txtGradients";
            this.txtGradients.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGradients.Size = new System.Drawing.Size(634, 262);
            this.txtGradients.TabIndex = 2;
            this.txtGradients.Text = "2 * x1 \r\n4 * x2 ";
            // 
            // btnCreateFunction
            // 
            this.btnCreateFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateFunction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCreateFunction.Location = new System.Drawing.Point(509, 3);
            this.btnCreateFunction.Name = "btnCreateFunction";
            this.btnCreateFunction.Size = new System.Drawing.Size(132, 41);
            this.btnCreateFunction.TabIndex = 4;
            this.btnCreateFunction.Text = "Create scalar function";
            this.btnCreateFunction.UseVisualStyleBackColor = true;
            // 
            // btnValueCalculator
            // 
            this.btnValueCalculator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnValueCalculator.Location = new System.Drawing.Point(509, 47);
            this.btnValueCalculator.Name = "btnValueCalculator";
            this.btnValueCalculator.Size = new System.Drawing.Size(132, 23);
            this.btnValueCalculator.TabIndex = 4;
            this.btnValueCalculator.Text = "Value Calculator";
            this.btnValueCalculator.UseVisualStyleBackColor = true;
            // 
            // chkGradients
            // 
            this.chkGradients.AutoSize = true;
            this.chkGradients.Checked = true;
            this.chkGradients.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGradients.Location = new System.Drawing.Point(106, 226);
            this.chkGradients.Name = "chkGradients";
            this.chkGradients.Size = new System.Drawing.Size(109, 17);
            this.chkGradients.TabIndex = 5;
            this.chkGradients.Text = "Gradients defined";
            this.chkGradients.UseVisualStyleBackColor = true;
            // 
            // btnSummary
            // 
            this.btnSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSummary.Location = new System.Drawing.Point(509, 142);
            this.btnSummary.Name = "btnSummary";
            this.btnSummary.Size = new System.Drawing.Size(132, 23);
            this.btnSummary.TabIndex = 4;
            this.btnSummary.Text = "Summary";
            this.btnSummary.UseVisualStyleBackColor = true;
            // 
            // ScalarFunctionScriptControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlOuter);
            this.Name = "ScalarFunctionScriptControl";
            this.Size = new System.Drawing.Size(697, 516);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.pnlOuter.ResumeLayout(false);
            this.pnlOuter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDimension;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Panel pnlOuter;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblParameterNames;
        private System.Windows.Forms.CheckBox chkGradients;
        private System.Windows.Forms.Button btnValueCalculator;
        private System.Windows.Forms.Button btnCreateFunction;
        private System.Windows.Forms.TextBox txtGradients;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtValueMath;
        private System.Windows.Forms.Button btnSummary;
    }
}
