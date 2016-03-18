// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class InputParametersControl
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
            this.btnResetCenter = new System.Windows.Forms.Button();
            this.btnResetDefault = new System.Windows.Forms.Button();
            this.dGridInputParam = new System.Windows.Forms.DataGridView();
            this.contextMenuControl = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.groupBox1.Controls.Add(this.btnResetCenter);
            this.groupBox1.Controls.Add(this.btnResetDefault);
            this.groupBox1.Controls.Add(this.dGridInputParam);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(368, 410);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Input Parameters";
            // 
            // btnResetCenter
            // 
            this.btnResetCenter.Location = new System.Drawing.Point(107, 17);
            this.btnResetCenter.Margin = new System.Windows.Forms.Padding(2);
            this.btnResetCenter.Name = "btnResetCenter";
            this.btnResetCenter.Size = new System.Drawing.Size(99, 23);
            this.btnResetCenter.TabIndex = 6;
            this.btnResetCenter.Text = "Reset to Center";
            this.btnResetCenter.UseVisualStyleBackColor = true;
            this.btnResetCenter.Click += new System.EventHandler(this.btnResetCenter_Click);
            // 
            // btnResetDefault
            // 
            this.btnResetDefault.Location = new System.Drawing.Point(4, 17);
            this.btnResetDefault.Margin = new System.Windows.Forms.Padding(2);
            this.btnResetDefault.Name = "btnResetDefault";
            this.btnResetDefault.Size = new System.Drawing.Size(99, 23);
            this.btnResetDefault.TabIndex = 6;
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
            this.dGridInputParam.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dGridInputParam.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dGridInputParam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGridInputParam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnId,
            this.columnTitle,
            this.columnValue});
            this.dGridInputParam.Location = new System.Drawing.Point(4, 44);
            this.dGridInputParam.Margin = new System.Windows.Forms.Padding(2);
            this.dGridInputParam.Name = "dGridInputParam";
            this.dGridInputParam.RowHeadersVisible = false;
            this.dGridInputParam.RowTemplate.Height = 24;
            this.dGridInputParam.Size = new System.Drawing.Size(361, 363);
            this.dGridInputParam.TabIndex = 5;
            this.dGridInputParam.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGridInputParam_CellValueChanged);
            this.dGridInputParam.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dGridInputParam_MouseClick);
            // 
            // contextMenuControl
            // 
            this.contextMenuControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToDefaultToolStripMenuItem,
            this.resetToCenterToolStripMenuItem});
            this.contextMenuControl.Name = "contextMenuControl";
            this.contextMenuControl.Size = new System.Drawing.Size(158, 48);
            // 
            // resetToDefaultToolStripMenuItem
            // 
            this.resetToDefaultToolStripMenuItem.Name = "resetToDefaultToolStripMenuItem";
            this.resetToDefaultToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.resetToDefaultToolStripMenuItem.Text = "Reset to Default";
            this.resetToDefaultToolStripMenuItem.Click += new System.EventHandler(this.resetToDefaultToolStripMenuItem_Click);
            // 
            // resetToCenterToolStripMenuItem
            // 
            this.resetToCenterToolStripMenuItem.Name = "resetToCenterToolStripMenuItem";
            this.resetToCenterToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.resetToCenterToolStripMenuItem.Text = "Reset to Center";
            this.resetToCenterToolStripMenuItem.Click += new System.EventHandler(this.resetToCenterToolStripMenuItem_Click);
            // 
            // columnID
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            this.columnId.DefaultCellStyle = dataGridViewCellStyle1;
            this.columnId.Frozen = true;
            this.columnId.HeaderText = "ID";
            this.columnId.MinimumWidth = 20;
            this.columnId.Name = "columnID";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 46;
            // 
            // columnTitle
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            this.columnTitle.DefaultCellStyle = dataGridViewCellStyle2;
            this.columnTitle.Frozen = true;
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.MinimumWidth = 40;
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 70;
            // 
            // columnValue
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.columnValue.DefaultCellStyle = dataGridViewCellStyle3;
            this.columnValue.Frozen = true;
            this.columnValue.HeaderText = "Value";
            this.columnValue.MinimumWidth = 40;
            this.columnValue.Name = "columnValue";
            this.columnValue.Width = 68;
            // 
            // InputParametersControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "InputParametersControl";
            this.Size = new System.Drawing.Size(373, 413);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.InputParametersControl_MouseClick);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dGridInputParam)).EndInit();
            this.contextMenuControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dGridInputParam;
        private System.Windows.Forms.Button btnResetDefault;
        private System.Windows.Forms.Button btnResetCenter;
        private System.Windows.Forms.ContextMenuStrip contextMenuControl;
        private System.Windows.Forms.ToolStripMenuItem resetToDefaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToCenterToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnValue;
    }
}
