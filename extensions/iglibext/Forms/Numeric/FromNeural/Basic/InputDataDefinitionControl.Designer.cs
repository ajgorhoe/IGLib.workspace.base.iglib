// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

namespace IG.Forms
{
    partial class InpuOrOutputtDataDefinitionControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grpOuter = new System.Windows.Forms.GroupBox();
            this.btnResetCenter = new System.Windows.Forms.Button();
            this.btnResetDefault = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.columnId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDefault = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnNumPoints = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuControl = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indicatorLight1 = new IG.Forms.IndicatorLight();
            this.grpOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpOuter
            // 
            this.grpOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOuter.Controls.Add(this.btnResetCenter);
            this.grpOuter.Controls.Add(this.btnResetDefault);
            this.grpOuter.Controls.Add(this.indicatorLight1);
            this.grpOuter.Controls.Add(this.dataGridView1);
            this.grpOuter.Location = new System.Drawing.Point(2, 2);
            this.grpOuter.Margin = new System.Windows.Forms.Padding(2);
            this.grpOuter.Name = "grpOuter";
            this.grpOuter.Padding = new System.Windows.Forms.Padding(2);
            this.grpOuter.Size = new System.Drawing.Size(712, 533);
            this.grpOuter.TabIndex = 7;
            this.grpOuter.TabStop = false;
            this.grpOuter.Text = "Input Parameters";
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
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnId,
            this.columnName,
            this.columnMin,
            this.columnMax,
            this.columnDefault,
            this.columnNumPoints,
            this.columnTitle,
            this.columnDescription});
            this.dataGridView1.Location = new System.Drawing.Point(4, 44);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(705, 486);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGridInputParam_CellValueChanged);
            this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dGridInputParam_MouseClick);
            // 
            // columnId
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            this.columnId.DefaultCellStyle = dataGridViewCellStyle4;
            this.columnId.Frozen = true;
            this.columnId.HeaderText = "ID";
            this.columnId.MinimumWidth = 20;
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.ToolTipText = "Sequential number of the input data element.";
            this.columnId.Width = 46;
            // 
            // columnName
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            this.columnName.DefaultCellStyle = dataGridViewCellStyle5;
            this.columnName.Frozen = true;
            this.columnName.HeaderText = "Name";
            this.columnName.MinimumWidth = 40;
            this.columnName.Name = "columnName";
            this.columnName.ToolTipText = "Input data element\'s variable name (should obey variable name conventions).";
            this.columnName.Width = 70;
            // 
            // columnMin
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.columnMin.DefaultCellStyle = dataGridViewCellStyle6;
            this.columnMin.HeaderText = "Min";
            this.columnMin.MinimumWidth = 40;
            this.columnMin.Name = "columnMin";
            this.columnMin.ToolTipText = "Minimal value of the input data element.";
            this.columnMin.Width = 68;
            // 
            // columnMax
            // 
            this.columnMax.HeaderText = "Max";
            this.columnMax.MinimumWidth = 40;
            this.columnMax.Name = "columnMax";
            this.columnMax.ToolTipText = "Maximal value of the input data element.";
            this.columnMax.Width = 68;
            // 
            // columnDefault
            // 
            this.columnDefault.HeaderText = "Default";
            this.columnDefault.MinimumWidth = 40;
            this.columnDefault.Name = "columnDefault";
            this.columnDefault.ToolTipText = "Default value of the input data element.";
            this.columnDefault.Width = 68;
            // 
            // columnNumPoints
            // 
            this.columnNumPoints.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnNumPoints.HeaderText = "Num. Pt.";
            this.columnNumPoints.MinimumWidth = 20;
            this.columnNumPoints.Name = "columnNumPoints";
            this.columnNumPoints.ToolTipText = "Number of grid (or sampling) points for structured sampling and other purposes.";
            this.columnNumPoints.Width = 73;
            // 
            // columnTitle
            // 
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ToolTipText = "Title (display name) of the input data element. Should be short and distinguishin" +
    "g but does not need to obey variable name rules.";
            // 
            // columnDescription
            // 
            this.columnDescription.HeaderText = "Description";
            this.columnDescription.Name = "columnDescription";
            this.columnDescription.ToolTipText = "Free form description of the input data element.";
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
            // indicatorLight1
            // 
            this.indicatorLight1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.indicatorLight1.AutoSize = true;
            this.indicatorLight1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.indicatorLight1.BlinkIntervalMilliSeconds = 200;
            this.indicatorLight1.BorderLabel = false;
            this.indicatorLight1.BorderOut = false;
            this.indicatorLight1.ColorLabel = System.Drawing.Color.Black;
            this.indicatorLight1.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.indicatorLight1.HasBusy = true;
            this.indicatorLight1.HasError = true;
            this.indicatorLight1.HasOff = true;
            this.indicatorLight1.HasOk = true;
            this.indicatorLight1.IsBlinking = false;
            this.indicatorLight1.LabelFont = null;
            this.indicatorLight1.LabelText = null;
            this.indicatorLight1.Location = new System.Drawing.Point(685, 10);
            this.indicatorLight1.MarginLabel = 2;
            this.indicatorLight1.MarginOut = 2;
            this.indicatorLight1.Name = "indicatorLight1";
            this.indicatorLight1.PaddingLabel = 2;
            this.indicatorLight1.PaddingOut = 2;
            this.indicatorLight1.Size = new System.Drawing.Size(22, 30);
            this.indicatorLight1.TabIndex = 7;
            this.indicatorLight1.ThrowOnInvalidSwitch = false;
            // 
            // InputDataDefinitionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpOuter);
            this.Name = "InputDataDefinitionControl";
            this.Size = new System.Drawing.Size(717, 536);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.InputDataDefinitionControl_MouseClick);
            this.grpOuter.ResumeLayout(false);
            this.grpOuter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpOuter;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnResetDefault;
        private System.Windows.Forms.Button btnResetCenter;
        private System.Windows.Forms.ContextMenuStrip contextMenuControl;
        private System.Windows.Forms.ToolStripMenuItem resetToDefaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToCenterToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMin;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnMax;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDefault;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnNumPoints;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDescription;
        private IndicatorLight indicatorLight1;
    }
}
