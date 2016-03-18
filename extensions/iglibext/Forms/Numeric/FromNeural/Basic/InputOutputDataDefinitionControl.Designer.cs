namespace IG.Forms
{
    partial class InputOutputDataDefinitionControl
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
            this.lblMainTitle = new System.Windows.Forms.Label();
            this.inputDataDefinitionControl1 = new IG.Forms.InpuOrOutputtDataDefinitionControl();
            this.outputDataDefinitionControl2 = new IG.Forms.InpuOrOutputtDataDefinitionControl();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.numNumOutputValues = new System.Windows.Forms.NumericUpDown();
            this.numNumInputParameters = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnGenerateTestData = new System.Windows.Forms.Button();
            this.btnLoadFromFunction = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblNumOutputValues = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblNminputParameters = new System.Windows.Forms.Label();
            this.indicatorLight1 = new IG.Forms.IndicatorLight();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuSummary = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.fileSelector1 = new IG.Forms.FileSelector();
            this.pnlControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumOutputValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumInputParameters)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMainTitle
            // 
            this.lblMainTitle.AutoSize = true;
            this.lblMainTitle.Location = new System.Drawing.Point(3, 4);
            this.lblMainTitle.Name = "lblMainTitle";
            this.lblMainTitle.Size = new System.Drawing.Size(160, 13);
            this.lblMainTitle.TabIndex = 0;
            this.lblMainTitle.Text = "Input and Output Data Definition";
            // 
            // inputDataDefinitionControl1
            // 
            this.inputDataDefinitionControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputDataDefinitionControl1.DataDefinition = null;
            this.inputDataDefinitionControl1.HasUnsavedChanges = false;
            this.inputDataDefinitionControl1.InputLength = 0;
            this.inputDataDefinitionControl1.IsDimentionChangeAllowed = true;
            this.inputDataDefinitionControl1.IsInputData = true;
            this.inputDataDefinitionControl1.Location = new System.Drawing.Point(3, 3);
            this.inputDataDefinitionControl1.Name = "inputDataDefinitionControl1";
            this.inputDataDefinitionControl1.OutputLength = 0;
            this.inputDataDefinitionControl1.Size = new System.Drawing.Size(293, 335);
            this.inputDataDefinitionControl1.TabIndex = 1;
            this.inputDataDefinitionControl1.TitleInputData = "Input data:";
            this.inputDataDefinitionControl1.TitleOutputData = "Output data:";
            this.inputDataDefinitionControl1.UseLegalVariableNames = true;
            this.inputDataDefinitionControl1.NumElementsChanged += new System.EventHandler<IG.Forms.IndexChangeEventArgs>(this.inputDataDefinitionControl1_NumElementsChanged);
            this.inputDataDefinitionControl1.ElementDataChanged += new System.EventHandler(this.inputDataDefinitionControl1_DataChanged);
            // 
            // outputDataDefinitionControl2
            // 
            this.outputDataDefinitionControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputDataDefinitionControl2.DataDefinition = null;
            this.outputDataDefinitionControl2.HasUnsavedChanges = false;
            this.outputDataDefinitionControl2.InputLength = 0;
            this.outputDataDefinitionControl2.IsDimentionChangeAllowed = true;
            this.outputDataDefinitionControl2.IsInputData = true;
            this.outputDataDefinitionControl2.Location = new System.Drawing.Point(3, 3);
            this.outputDataDefinitionControl2.Name = "outputDataDefinitionControl2";
            this.outputDataDefinitionControl2.OutputLength = 0;
            this.outputDataDefinitionControl2.Size = new System.Drawing.Size(239, 335);
            this.outputDataDefinitionControl2.TabIndex = 2;
            this.outputDataDefinitionControl2.TitleInputData = "Input Data";
            this.outputDataDefinitionControl2.TitleOutputData = "Output Data";
            this.outputDataDefinitionControl2.UseLegalVariableNames = true;
            this.outputDataDefinitionControl2.NumElementsChanged += new System.EventHandler<IG.Forms.IndexChangeEventArgs>(this.outputDataDefinitionControl2_NumElementsChanged);
            this.outputDataDefinitionControl2.ElementDataChanged += new System.EventHandler(this.outputDataDefinitionControl2_DataChanged);
            // 
            // pnlControls
            // 
            this.pnlControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlControls.Controls.Add(this.txtName);
            this.pnlControls.Controls.Add(this.txtDescription);
            this.pnlControls.Controls.Add(this.numNumOutputValues);
            this.pnlControls.Controls.Add(this.numNumInputParameters);
            this.pnlControls.Controls.Add(this.btnSave);
            this.pnlControls.Controls.Add(this.btnGenerateTestData);
            this.pnlControls.Controls.Add(this.btnLoadFromFunction);
            this.pnlControls.Controls.Add(this.btnLoad);
            this.pnlControls.Controls.Add(this.lblNumOutputValues);
            this.pnlControls.Controls.Add(this.lblDescription);
            this.pnlControls.Controls.Add(this.lblName);
            this.pnlControls.Controls.Add(this.lblNminputParameters);
            this.pnlControls.Controls.Add(this.indicatorLight1);
            this.pnlControls.Location = new System.Drawing.Point(4, 20);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(553, 96);
            this.pnlControls.TabIndex = 3;
            // 
            // txtName
            // 
            this.txtName.ForeColor = System.Drawing.Color.Blue;
            this.txtName.Location = new System.Drawing.Point(162, 29);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(146, 20);
            this.txtName.TabIndex = 3;
            this.txtName.Text = "<< Name >>";
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(72, 55);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(236, 38);
            this.txtDescription.TabIndex = 3;
            this.txtDescription.Text = "<< Descriptiion >>";
            this.txtDescription.Validated += new System.EventHandler(this.txtDescription_Validated);
            // 
            // numNumOutputValues
            // 
            this.numNumOutputValues.ForeColor = System.Drawing.Color.Blue;
            this.numNumOutputValues.Location = new System.Drawing.Point(101, 29);
            this.numNumOutputValues.Name = "numNumOutputValues";
            this.numNumOutputValues.Size = new System.Drawing.Size(55, 20);
            this.numNumOutputValues.TabIndex = 1;
            this.numNumOutputValues.Validated += new System.EventHandler(this.numNumOutputValues_Validated);
            // 
            // numNumInputParameters
            // 
            this.numNumInputParameters.ForeColor = System.Drawing.Color.Blue;
            this.numNumInputParameters.Location = new System.Drawing.Point(101, 3);
            this.numNumInputParameters.Name = "numNumInputParameters";
            this.numNumInputParameters.Size = new System.Drawing.Size(55, 20);
            this.numNumInputParameters.TabIndex = 1;
            this.numNumInputParameters.Validated += new System.EventHandler(this.numNumInputParameters_Validated);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(435, 32);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(115, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnGenerateTestData
            // 
            this.btnGenerateTestData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateTestData.Location = new System.Drawing.Point(314, 32);
            this.btnGenerateTestData.Name = "btnGenerateTestData";
            this.btnGenerateTestData.Size = new System.Drawing.Size(115, 23);
            this.btnGenerateTestData.TabIndex = 0;
            this.btnGenerateTestData.Text = "Generate Template Data";
            this.btnGenerateTestData.UseVisualStyleBackColor = true;
            this.btnGenerateTestData.Click += new System.EventHandler(this.btnGenerateTestData_Click);
            // 
            // btnLoadFromFunction
            // 
            this.btnLoadFromFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadFromFunction.Location = new System.Drawing.Point(314, 3);
            this.btnLoadFromFunction.Name = "btnLoadFromFunction";
            this.btnLoadFromFunction.Size = new System.Drawing.Size(115, 23);
            this.btnLoadFromFunction.TabIndex = 0;
            this.btnLoadFromFunction.Text = "Load From Function";
            this.btnLoadFromFunction.UseVisualStyleBackColor = true;
            this.btnLoadFromFunction.Click += new System.EventHandler(this.btnLoadFromFunction_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(435, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(115, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lblNumOutputValues
            // 
            this.lblNumOutputValues.AutoSize = true;
            this.lblNumOutputValues.Location = new System.Drawing.Point(3, 31);
            this.lblNumOutputValues.Name = "lblNumOutputValues";
            this.lblNumOutputValues.Size = new System.Drawing.Size(92, 13);
            this.lblNumOutputValues.TabIndex = 2;
            this.lblNumOutputValues.Text = "Output dimension:";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(3, 55);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Description:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(162, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Name:";
            // 
            // lblNminputParameters
            // 
            this.lblNminputParameters.AutoSize = true;
            this.lblNminputParameters.Location = new System.Drawing.Point(3, 5);
            this.lblNminputParameters.Name = "lblNminputParameters";
            this.lblNminputParameters.Size = new System.Drawing.Size(84, 13);
            this.lblNminputParameters.TabIndex = 2;
            this.lblNminputParameters.Text = "Input dimension:";
            // 
            // indicatorLight1
            // 
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
            this.indicatorLight1.Location = new System.Drawing.Point(314, 55);
            this.indicatorLight1.MarginLabel = 2;
            this.indicatorLight1.MarginOut = 2;
            this.indicatorLight1.Name = "indicatorLight1";
            this.indicatorLight1.PaddingLabel = 2;
            this.indicatorLight1.PaddingOut = 2;
            this.indicatorLight1.Size = new System.Drawing.Size(22, 30);
            this.indicatorLight1.TabIndex = 8;
            this.indicatorLight1.ThrowOnInvalidSwitch = false;
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
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 122);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.inputDataDefinitionControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.fileSelector1);
            this.splitContainer1.Panel2.Controls.Add(this.outputDataDefinitionControl2);
            this.splitContainer1.Size = new System.Drawing.Size(554, 341);
            this.splitContainer1.SplitterDistance = 299;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 5;
            // 
            // fileSelector1
            // 
            this.fileSelector1.AllowDrop = true;
            this.fileSelector1.AllowEnvironmentVariables = true;
            this.fileSelector1.AllowExistentFiles = true;
            this.fileSelector1.AllowNonexistentFiles = true;
            this.fileSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileSelector1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fileSelector1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileSelector1.ErrorBackground = System.Drawing.Color.Orange;
            this.fileSelector1.ErrorForeground = System.Drawing.Color.Red;
            this.fileSelector1.FilePath = null;
            this.fileSelector1.Filter = null;
            this.fileSelector1.IsBrowsable = true;
            this.fileSelector1.IsDragAndDrop = true;
            this.fileSelector1.Location = new System.Drawing.Point(8, 262);
            this.fileSelector1.Margin = new System.Windows.Forms.Padding(4);
            this.fileSelector1.MinimumSize = new System.Drawing.Size(200, 15);
            this.fileSelector1.Name = "fileSelector1";
            this.fileSelector1.NormalBackground = System.Drawing.SystemColors.Window;
            this.fileSelector1.NormalForeground = System.Drawing.Color.Black;
            this.fileSelector1.OriginalFilePath = null;
            this.fileSelector1.Size = new System.Drawing.Size(227, 30);
            this.fileSelector1.TabIndex = 8;
            this.fileSelector1.UseAbsolutePaths = false;
            this.fileSelector1.UseRelativePaths = false;
            this.fileSelector1.Visible = false;
            // 
            // InputOutputDataDefinitionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.lblMainTitle);
            this.MinimumSize = new System.Drawing.Size(560, 466);
            this.Name = "InputOutputDataDefinitionControl";
            this.Size = new System.Drawing.Size(560, 466);
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumOutputValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumInputParameters)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMainTitle;
        private IG.Forms.InpuOrOutputtDataDefinitionControl inputDataDefinitionControl1;
        private IG.Forms.InpuOrOutputtDataDefinitionControl outputDataDefinitionControl2;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoadFromFunction;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuSummary;
        private System.Windows.Forms.Label lblNumOutputValues;
        private System.Windows.Forms.Label lblNminputParameters;
        private System.Windows.Forms.NumericUpDown numNumOutputValues;
        private System.Windows.Forms.NumericUpDown numNumInputParameters;
        private System.Windows.Forms.Button btnGenerateTestData;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblName;
        private FileSelector fileSelector1;
        private IndicatorLight indicatorLight1;
    }
}
