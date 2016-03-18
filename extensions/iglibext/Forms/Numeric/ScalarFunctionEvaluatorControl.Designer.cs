// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class ScalarFunctionEvaluatorControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkTreatAsVector = new System.Windows.Forms.CheckBox();
            this.btnIdentifyThread = new System.Windows.Forms.Button();
            this.chkImmediateCalculation = new System.Windows.Forms.CheckBox();
            this.lblParameters = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnShowDefinition = new System.Windows.Forms.Button();
            this.btnResetDefault = new System.Windows.Forms.Button();
            this.dGridInputParam = new System.Windows.Forms.DataGridView();
            this.columnID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuControl = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGridInputParam)).BeginInit();
            this.contextMenuControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkTreatAsVector);
            this.groupBox1.Controls.Add(this.btnIdentifyThread);
            this.groupBox1.Controls.Add(this.chkImmediateCalculation);
            this.groupBox1.Controls.Add(this.lblParameters);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtValue);
            this.groupBox1.Controls.Add(this.btnCalculate);
            this.groupBox1.Controls.Add(this.btnShowDefinition);
            this.groupBox1.Controls.Add(this.btnResetDefault);
            this.groupBox1.Controls.Add(this.dGridInputParam);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.MinimumSize = new System.Drawing.Size(504, 227);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(504, 227);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input Parameters";
            // 
            // chkTreatAsVector
            // 
            this.chkTreatAsVector.AutoSize = true;
            this.chkTreatAsVector.Location = new System.Drawing.Point(163, 17);
            this.chkTreatAsVector.Margin = new System.Windows.Forms.Padding(2);
            this.chkTreatAsVector.Name = "chkTreatAsVector";
            this.chkTreatAsVector.Size = new System.Drawing.Size(116, 17);
            this.chkTreatAsVector.TabIndex = 2;
            this.chkTreatAsVector.Text = "As Vector Function";
            this.chkTreatAsVector.UseVisualStyleBackColor = true;
            this.chkTreatAsVector.CheckedChanged += new System.EventHandler(this.chkTreatAsVector_CheckedChanged);
            // 
            // btnIdentifyThread
            // 
            this.btnIdentifyThread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIdentifyThread.Location = new System.Drawing.Point(391, 13);
            this.btnIdentifyThread.Margin = new System.Windows.Forms.Padding(2);
            this.btnIdentifyThread.Name = "btnIdentifyThread";
            this.btnIdentifyThread.Size = new System.Drawing.Size(109, 23);
            this.btnIdentifyThread.TabIndex = 8;
            this.btnIdentifyThread.Text = "Identify Thread";
            this.btnIdentifyThread.UseVisualStyleBackColor = true;
            this.btnIdentifyThread.Click += new System.EventHandler(this.btnIdentifyThread_Click);
            // 
            // chkImmediateCalculation
            // 
            this.chkImmediateCalculation.AutoSize = true;
            this.chkImmediateCalculation.Checked = true;
            this.chkImmediateCalculation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImmediateCalculation.Location = new System.Drawing.Point(7, 17);
            this.chkImmediateCalculation.Margin = new System.Windows.Forms.Padding(2);
            this.chkImmediateCalculation.Name = "chkImmediateCalculation";
            this.chkImmediateCalculation.Size = new System.Drawing.Size(129, 17);
            this.chkImmediateCalculation.TabIndex = 1;
            this.chkImmediateCalculation.Text = "Immediate Calculation";
            this.chkImmediateCalculation.UseVisualStyleBackColor = true;
            this.chkImmediateCalculation.CheckedChanged += new System.EventHandler(this.chkImmediateCalculation_CheckedChanged);
            // 
            // lblParameters
            // 
            this.lblParameters.AutoSize = true;
            this.lblParameters.Location = new System.Drawing.Point(4, 92);
            this.lblParameters.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblParameters.Name = "lblParameters";
            this.lblParameters.Size = new System.Drawing.Size(92, 13);
            this.lblParameters.TabIndex = 9;
            this.lblParameters.Text = "Parameter values:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Function value:";
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.BackColor = System.Drawing.Color.White;
            this.txtValue.ForeColor = System.Drawing.Color.Blue;
            this.txtValue.Location = new System.Drawing.Point(7, 55);
            this.txtValue.Margin = new System.Windows.Forms.Padding(2);
            this.txtValue.Name = "txtValue";
            this.txtValue.ReadOnly = true;
            this.txtValue.Size = new System.Drawing.Size(347, 20);
            this.txtValue.TabIndex = 3;
            // 
            // btnCalculate
            // 
            this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalculate.Location = new System.Drawing.Point(358, 52);
            this.btnCalculate.Margin = new System.Windows.Forms.Padding(2);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(142, 23);
            this.btnCalculate.TabIndex = 4;
            this.btnCalculate.Text = "Calculate Function Value";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnShowDefinition
            // 
            this.btnShowDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowDefinition.Location = new System.Drawing.Point(297, 14);
            this.btnShowDefinition.Margin = new System.Windows.Forms.Padding(2);
            this.btnShowDefinition.Name = "btnShowDefinition";
            this.btnShowDefinition.Size = new System.Drawing.Size(90, 23);
            this.btnShowDefinition.TabIndex = 7;
            this.btnShowDefinition.Text = "Show def.";
            this.btnShowDefinition.UseVisualStyleBackColor = true;
            this.btnShowDefinition.Click += new System.EventHandler(this.btnShowDefinition_Click);
            // 
            // btnResetDefault
            // 
            this.btnResetDefault.Location = new System.Drawing.Point(124, 87);
            this.btnResetDefault.Margin = new System.Windows.Forms.Padding(2);
            this.btnResetDefault.Name = "btnResetDefault";
            this.btnResetDefault.Size = new System.Drawing.Size(110, 23);
            this.btnResetDefault.TabIndex = 5;
            this.btnResetDefault.Text = "Reset to Default";
            this.btnResetDefault.UseVisualStyleBackColor = true;
            this.btnResetDefault.Click += new System.EventHandler(this.btnResetDefault_Click);
            // 
            // dGridInputParam
            // 
            this.dGridInputParam.AllowUserToAddRows = false;
            this.dGridInputParam.AllowUserToDeleteRows = false;
            this.dGridInputParam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dGridInputParam.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dGridInputParam.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dGridInputParam.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dGridInputParam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGridInputParam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnID,
            this.columnName,
            this.columnValue});
            this.dGridInputParam.Location = new System.Drawing.Point(4, 122);
            this.dGridInputParam.Margin = new System.Windows.Forms.Padding(2);
            this.dGridInputParam.MinimumSize = new System.Drawing.Size(0, 100);
            this.dGridInputParam.Name = "dGridInputParam";
            this.dGridInputParam.RowHeadersVisible = false;
            this.dGridInputParam.RowTemplate.Height = 24;
            this.dGridInputParam.Size = new System.Drawing.Size(496, 102);
            this.dGridInputParam.TabIndex = 6;
            this.dGridInputParam.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGridInputParam_CellValueChanged);
            this.dGridInputParam.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dGridInputParam_MouseClick);
            // 
            // columnID
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.columnID.DefaultCellStyle = dataGridViewCellStyle1;
            this.columnID.Frozen = true;
            this.columnID.HeaderText = "ID";
            this.columnID.MinimumWidth = 20;
            this.columnID.Name = "columnID";
            this.columnID.ReadOnly = true;
            this.columnID.Width = 43;
            // 
            // columnName
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            this.columnName.DefaultCellStyle = dataGridViewCellStyle2;
            this.columnName.Frozen = true;
            this.columnName.HeaderText = "Name";
            this.columnName.MinimumWidth = 50;
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            this.columnName.Width = 60;
            // 
            // columnValue
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.columnValue.DefaultCellStyle = dataGridViewCellStyle3;
            this.columnValue.Frozen = true;
            this.columnValue.HeaderText = "Value";
            this.columnValue.MinimumWidth = 40;
            this.columnValue.Name = "columnValue";
            this.columnValue.Width = 59;
            // 
            // contextMenuControl
            // 
            this.contextMenuControl.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToDefaultToolStripMenuItem});
            this.contextMenuControl.Name = "contextMenuControl";
            this.contextMenuControl.Size = new System.Drawing.Size(158, 26);
            // 
            // resetToDefaultToolStripMenuItem
            // 
            this.resetToDefaultToolStripMenuItem.Name = "resetToDefaultToolStripMenuItem";
            this.resetToDefaultToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.resetToDefaultToolStripMenuItem.Text = "Reset to Default";
            this.resetToDefaultToolStripMenuItem.Click += new System.EventHandler(this.resetToDefaultToolStripMenuItem_Click);
            // 
            // ScalarFunctionEvaluatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(508, 230);
            this.Name = "ScalarFunctionEvaluatorControl";
            this.Size = new System.Drawing.Size(508, 230);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ScalarFunctionEvaluatorControl_MouseClick);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGridInputParam)).EndInit();
            this.contextMenuControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dGridInputParam;
        private System.Windows.Forms.Button btnResetDefault;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.ContextMenuStrip contextMenuControl;
        private System.Windows.Forms.ToolStripMenuItem resetToDefaultToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblParameters;
        private System.Windows.Forms.Button btnShowDefinition;
        private System.Windows.Forms.CheckBox chkImmediateCalculation;
        private System.Windows.Forms.CheckBox chkTreatAsVector;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnID;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnValue;
        private System.Windows.Forms.Button btnIdentifyThread;
    }
}
