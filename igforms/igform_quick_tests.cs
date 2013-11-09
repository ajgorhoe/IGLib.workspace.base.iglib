// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

using System.Threading;
using System.Text;
using System.IO;
using System.Xml;

using IG.Forms;


//using System.ComponentModel;
//using System.Data;
//using System.Windows.Forms;
//



            /**************************************************************/
            /*                                                            */
            /*   QUICK COPY-PASTE TESTING OF CODE FROM EXTERNAL SOURCES   */
            /*                                                            */
            /**************************************************************/


namespace IG.Forms.Test
{

    public class IgFormsQickTests
    {
        [STAThread]
        public static void Main()
        {

            ColorConverterForm.Main0(); // Converts colors from RGB to Hue-Saturation-Brightness

            //MessageBoxBuilder.Main0(null);

            // MessageBox.Show("This is a default Main from the class IgFormsQickTests, \d2\d2  RENAME it to use a different maain.");
        }
    }


    #region TEST_CLASSES



    public class ColorConverterForm : System.Windows.Forms.Form {
        private Color theColor;

        public ColorConverterForm() {
            InitializeComponent();
            tbRed.Text = "0";
            tbGreen.Text = "0";
            tbBlue.Text = "0";
        }
        [STAThread]
        public static void Main0()
        {
            Application.EnableVisualStyles();
            Application.Run(new ColorConverterForm());
        }

        private void parseRGB() {
            int r, g, b;
            try
            {
                r = Int16.Parse(tbRed.Text);
                r = Math.Min(r, 255);
                g = Int16.Parse(tbGreen.Text);
                g = Math.Min(g, 255);
                b = Int16.Parse(tbBlue.Text);
                b = Math.Min(b, 255);
                theColor = Color.FromArgb(r, g, b);
                tbHue.Text = theColor.GetHue().ToString();
                tbSat.Text = theColor.GetSaturation().ToString();
                tbBright.Text = theColor.GetBrightness().ToString();
                this.Invalidate();
            }
            catch { }
        }

        private void tbRed_TextChanged(object sender, EventArgs e) {
            parseRGB();
        }

        private void tbGreen_TextChanged(object sender, EventArgs e) {
            parseRGB();
        }

        private void tbBlue_TextChanged(object sender, EventArgs e) {
            parseRGB();
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;

            SolidBrush backBrush = new SolidBrush(System.Drawing.SystemColors.Control);
            g.FillRectangle(backBrush, this.ClientRectangle);
            backBrush.Dispose();

            Rectangle r = new Rectangle(10, 80, 100, 40);
            SolidBrush b = new SolidBrush(theColor);
            g.SetClip(this.ClientRectangle);
            g.FillRectangle(b, r);
            b.Dispose();
        }
        private void InitializeComponent() {
            this.tbRed = new System.Windows.Forms.TextBox();
            this.tbGreen = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbBlue = new System.Windows.Forms.TextBox();
            this.tbHue = new System.Windows.Forms.TextBox();
            this.tbSat = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbBright = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbRed
            // 
            this.tbRed.Location = new System.Drawing.Point(45, 10);
            this.tbRed.Name = "tbRed";
            this.tbRed.Size = new System.Drawing.Size(47, 20);
            this.tbRed.TabIndex = 1;
            this.tbRed.TextChanged += new System.EventHandler(this.tbRed_TextChanged);
            // 
            // tbGreen
            // 
            this.tbGreen.Location = new System.Drawing.Point(149, 10);
            this.tbGreen.Name = "tbGreen";
            this.tbGreen.Size = new System.Drawing.Size(47, 20);
            this.tbGreen.TabIndex = 2;
            this.tbGreen.TextChanged += new System.EventHandler(this.tbGreen_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 14);
            this.label1.TabIndex = 8;
            this.label1.Text = "Red";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(106, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 14);
            this.label2.TabIndex = 9;
            this.label2.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(212, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 14);
            this.label3.TabIndex = 10;
            this.label3.Text = "Blue";
            // 
            // tbBlue
            // 
            this.tbBlue.Location = new System.Drawing.Point(246, 10);
            this.tbBlue.Name = "tbBlue";
            this.tbBlue.Size = new System.Drawing.Size(47, 20);
            this.tbBlue.TabIndex = 3;
            this.tbBlue.TextChanged += new System.EventHandler(this.tbBlue_TextChanged);
            // 
            // tbHue
            // 
            this.tbHue.Location = new System.Drawing.Point(45, 44);
            this.tbHue.Name = "tbHue";
            this.tbHue.ReadOnly = true;
            this.tbHue.Size = new System.Drawing.Size(47, 20);
            this.tbHue.TabIndex = 4;
            // 
            // tbSat
            // 
            this.tbSat.Location = new System.Drawing.Point(149, 44);
            this.tbSat.Name = "tbSat";
            this.tbSat.ReadOnly = true;
            this.tbSat.Size = new System.Drawing.Size(47, 20);
            this.tbSat.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 14);
            this.label4.TabIndex = 11;
            this.label4.Text = "Hue";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(121, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 14);
            this.label5.TabIndex = 12;
            this.label5.Text = "Sat";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(205, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 14);
            this.label6.TabIndex = 13;
            this.label6.Text = "Bright";
            // 
            // tbBright
            // 
            this.tbBright.Location = new System.Drawing.Point(246, 42);
            this.tbBright.Name = "tbBright";
            this.tbBright.ReadOnly = true;
            this.tbBright.Size = new System.Drawing.Size(47, 20);
            this.tbBright.TabIndex = 6;
            // 
            // AutoScaleBaseSize
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(431, 217);
            this.Controls.Add(this.tbBright);
            this.Controls.Add(this.tbBlue);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSat);
            this.Controls.Add(this.tbHue);
            this.Controls.Add(this.tbGreen);
            this.Controls.Add(this.tbRed);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.TextBox tbRed;
        private System.Windows.Forms.TextBox tbGreen;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbBlue;
        private System.Windows.Forms.TextBox tbHue;
        private System.Windows.Forms.TextBox tbSat;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbBright;

    }      //  class ColorConverterForm


    public class MessageBoxBuilder : System.Windows.Forms.Form
    {


        [STAThreadAttribute]
        public static void Main0(string[] args)
        {
            Application.Run(new MessageBoxBuilder());
        }

        public MessageBoxBuilder()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            InitForm();
        }

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtCaption;
        private System.Windows.Forms.GroupBox grpCaption;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.GroupBox grpMessage;

        private MessageBoxButtons btnStyle;
        private MessageBoxIcon iconStyle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.CheckBox cbnVisualCSharp;
        private System.Windows.Forms.CheckBox cbnVisualCPP;
        private System.Windows.Forms.CheckBox cbnVisualBasic;
        private System.Windows.Forms.CheckBox cbnUseMFC;
        private System.Windows.Forms.GroupBox grpButtons;
        private System.Windows.Forms.GroupBox grpDefault;
        private System.Windows.Forms.Button btnDefault3;
        private System.Windows.Forms.Button btnDefault2;
        private System.Windows.Forms.Button btnDefault1;
        private System.Windows.Forms.ComboBox cboButtons;
        private System.Windows.Forms.GroupBox grpSend;
        private System.Windows.Forms.RadioButton rdoFile;
        private System.Windows.Forms.RadioButton rdoClipboard;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox cbnDeclareIt;
        private System.Windows.Forms.CheckBox cbnBuildSwitch;
        private System.Windows.Forms.Label lblVariable;
        private System.Windows.Forms.CheckBox cbnUseReturnVar;
        private System.Windows.Forms.TextBox txtVariable;
        private System.Windows.Forms.GroupBox grpCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.GroupBox grpSample;
        private System.Windows.Forms.Button btnSample4;
        private System.Windows.Forms.Button btnSample2;
        private System.Windows.Forms.Button btnSample5;
        private System.Windows.Forms.Button btnSample3;
        private System.Windows.Forms.Button btnSample1;
        private System.Windows.Forms.Label lblSampleText;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox cbnDefault3;
        private System.Windows.Forms.CheckBox cbnDefault2;
        private System.Windows.Forms.CheckBox cbnDefault1;
        private MessageBoxDefaultButton btnDefault;

            /// <summary>
            ///    Required method for Designer support - do not modify
            ///    the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
        {
          this.components = new System.ComponentModel.Container();
          //resources = new System.Resources.ResourceManager(typeof(MainForm));
          this.btnSample4 = new System.Windows.Forms.Button();
          this.btnSample5 = new System.Windows.Forms.Button();
          this.txtCaption = new System.Windows.Forms.TextBox();
          this.btnSample1 = new System.Windows.Forms.Button();
          this.btnSample2 = new System.Windows.Forms.Button();
          this.btnSample3 = new System.Windows.Forms.Button();
          this.cbnBuildSwitch = new System.Windows.Forms.CheckBox();
          this.pbIcon = new System.Windows.Forms.PictureBox();
          this.grpSend = new System.Windows.Forms.GroupBox();
          this.rdoFile = new System.Windows.Forms.RadioButton();
          this.rdoClipboard = new System.Windows.Forms.RadioButton();
          this.cbnVisualCSharp = new System.Windows.Forms.CheckBox();
          this.cbnDefault2 = new System.Windows.Forms.CheckBox();
          this.cbnDefault3 = new System.Windows.Forms.CheckBox();
          this.lblSampleText = new System.Windows.Forms.Label();
          this.grpCaption = new System.Windows.Forms.GroupBox();
          this.btnCancel = new System.Windows.Forms.Button();
          this.grpCode = new System.Windows.Forms.GroupBox();
          this.txtCode = new System.Windows.Forms.TextBox();
          this.pictureBox1 = new System.Windows.Forms.PictureBox();
          this.btnOK = new System.Windows.Forms.Button();
          this.listBox1 = new System.Windows.Forms.ListBox();
          this.grpMessage = new System.Windows.Forms.GroupBox();
          this.txtMessage = new System.Windows.Forms.TextBox();
          this.cbnUseMFC = new System.Windows.Forms.CheckBox();
          this.cbnDeclareIt = new System.Windows.Forms.CheckBox();
          this.btnDefault3 = new System.Windows.Forms.Button();
          this.lblCaption = new System.Windows.Forms.Label();
          this.lblVariable = new System.Windows.Forms.Label();
          this.btnPreview = new System.Windows.Forms.Button();
          this.cbnVisualCPP = new System.Windows.Forms.CheckBox();
          this.cbnDefault1 = new System.Windows.Forms.CheckBox();
          this.btnDefault1 = new System.Windows.Forms.Button();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this.btnDefault2 = new System.Windows.Forms.Button();
          this.cbnUseReturnVar = new System.Windows.Forms.CheckBox();
          this.txtVariable = new System.Windows.Forms.TextBox();
          this.grpDefault = new System.Windows.Forms.GroupBox();
          this.grpButtons = new System.Windows.Forms.GroupBox();
          this.cboButtons = new System.Windows.Forms.ComboBox();
          this.grpOptions = new System.Windows.Forms.GroupBox();
          this.cbnVisualBasic = new System.Windows.Forms.CheckBox();
          this.grpSample = new System.Windows.Forms.GroupBox();
          this.imageList1 = new System.Windows.Forms.ImageList(this.components);
          this.grpSend.SuspendLayout();
          this.grpCaption.SuspendLayout();
          this.grpCode.SuspendLayout();
          this.grpMessage.SuspendLayout();
          this.groupBox1.SuspendLayout();
          this.grpDefault.SuspendLayout();
          this.grpButtons.SuspendLayout();
          this.grpOptions.SuspendLayout();
          this.grpSample.SuspendLayout();
          this.SuspendLayout();
          // 
          // btnSample4
          // 
          this.btnSample4.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
          this.btnSample4.Location = new System.Drawing.Point(168, 154);
          this.btnSample4.Name = "btnSample4";
          this.btnSample4.Size = new System.Drawing.Size(57, 18);
          this.btnSample4.TabIndex = 4;
          this.btnSample4.Text = "button5";
          this.btnSample4.Visible = false;
          // 
          // btnSample5
          // 
          this.btnSample5.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
          this.btnSample5.Location = new System.Drawing.Point(205, 154);
          this.btnSample5.Name = "btnSample5";
          this.btnSample5.Size = new System.Drawing.Size(57, 18);
          this.btnSample5.TabIndex = 4;
          this.btnSample5.Text = "button3";
          this.btnSample5.Visible = false;
          // 
          // txtCaption
          // 
          this.txtCaption.Location = new System.Drawing.Point(9, 24);
          this.txtCaption.Name = "txtCaption";
          this.txtCaption.Size = new System.Drawing.Size(317, 20);
          this.txtCaption.TabIndex = 0;
          this.txtCaption.Text = "";
          this.txtCaption.TextChanged += new System.EventHandler(this.txtCaption_TextChanged);
          // 
          // btnSample1
          // 
          this.btnSample1.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
          this.btnSample1.Location = new System.Drawing.Point(56, 154);
          this.btnSample1.Name = "btnSample1";
          this.btnSample1.Size = new System.Drawing.Size(56, 18);
          this.btnSample1.TabIndex = 4;
          this.btnSample1.Text = "button1";
          this.btnSample1.Visible = false;
          // 
          // btnSample2
          // 
          this.btnSample2.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
          this.btnSample2.Location = new System.Drawing.Point(93, 154);
          this.btnSample2.Name = "btnSample2";
          this.btnSample2.Size = new System.Drawing.Size(57, 18);
          this.btnSample2.TabIndex = 4;
          this.btnSample2.Text = "button4";
          this.btnSample2.Visible = false;
          // 
          // btnSample3
          // 
          this.btnSample3.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
          this.btnSample3.Location = new System.Drawing.Point(131, 154);
          this.btnSample3.Name = "btnSample3";
          this.btnSample3.Size = new System.Drawing.Size(56, 18);
          this.btnSample3.TabIndex = 4;
          this.btnSample3.Text = "OK";
          // 
          // cbnBuildSwitch
          // 
          this.cbnBuildSwitch.Enabled = false;
          this.cbnBuildSwitch.Location = new System.Drawing.Point(22, 104);
          this.cbnBuildSwitch.Name = "cbnBuildSwitch";
          this.cbnBuildSwitch.Size = new System.Drawing.Size(175, 16);
          this.cbnBuildSwitch.TabIndex = 4;
          this.cbnBuildSwitch.Text = "Build Switch/Conditional";
          this.cbnBuildSwitch.CheckedChanged += new System.EventHandler(this.OnMessageBoxChanged);
          // 
          // pbIcon
          // 
          //his.pbIcon.Image = ((System.Drawing.Bitmap)(resources.GetObject("pbIcon.Image")));
          this.pbIcon.Location = new System.Drawing.Point(18, 75);
          this.pbIcon.Name = "pbIcon";
          this.pbIcon.Size = new System.Drawing.Size(38, 37);
          this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
          this.pbIcon.TabIndex = 2;
          this.pbIcon.TabStop = false;
          this.pbIcon.Visible = false;
          // 
          // grpSend
          // 
          this.grpSend.Controls.AddRange(new System.Windows.Forms.Control[] {
                                              this.rdoFile,
                                              this.rdoClipboard});
          this.grpSend.Location = new System.Drawing.Point(544, 264);
          this.grpSend.Name = "grpSend";
          this.grpSend.Size = new System.Drawing.Size(208, 56);
          this.grpSend.TabIndex = 9;
          this.grpSend.TabStop = false;
          this.grpSend.Text = "Send To";
          // 
          // rdoFile
          // 
          this.rdoFile.Location = new System.Drawing.Point(141, 19);
          this.rdoFile.Name = "rdoFile";
          this.rdoFile.Size = new System.Drawing.Size(59, 24);
          this.rdoFile.TabIndex = 1;
          this.rdoFile.Text = "File";
          // 
          // rdoClipboard
          // 
          this.rdoClipboard.Checked = true;
          this.rdoClipboard.Location = new System.Drawing.Point(17, 19);
          this.rdoClipboard.Name = "rdoClipboard";
          this.rdoClipboard.Size = new System.Drawing.Size(94, 24);
          this.rdoClipboard.TabIndex = 0;
          this.rdoClipboard.TabStop = true;
          this.rdoClipboard.Text = "Clipboard";
          // 
          // cbnVisualCSharp
          // 
          this.cbnVisualCSharp.AutoCheck = false;
          this.cbnVisualCSharp.Checked = true;
          this.cbnVisualCSharp.CheckState = System.Windows.Forms.CheckState.Checked;
          this.cbnVisualCSharp.Location = new System.Drawing.Point(22, 16);
          this.cbnVisualCSharp.Name = "cbnVisualCSharp";
          this.cbnVisualCSharp.Size = new System.Drawing.Size(102, 15);
          this.cbnVisualCSharp.TabIndex = 14;
          this.cbnVisualCSharp.Text = "Visual C#";
          this.cbnVisualCSharp.Click += new System.EventHandler(this.OnLanguageChanged);
          // 
          // cbnDefault2
          // 
          this.cbnDefault2.AutoCheck = false;
          this.cbnDefault2.Location = new System.Drawing.Point(34, 75);
          this.cbnDefault2.Name = "cbnDefault2";
          this.cbnDefault2.Size = new System.Drawing.Size(17, 18);
          this.cbnDefault2.TabIndex = 1;
          this.cbnDefault2.Visible = false;
          this.cbnDefault2.Click += new System.EventHandler(this.btnDefault2_Clicked);
          // 
          // cbnDefault3
          // 
          this.cbnDefault3.AutoCheck = false;
          this.cbnDefault3.Location = new System.Drawing.Point(34, 112);
          this.cbnDefault3.Name = "cbnDefault3";
          this.cbnDefault3.Size = new System.Drawing.Size(17, 17);
          this.cbnDefault3.TabIndex = 2;
          this.cbnDefault3.Visible = false;
          this.cbnDefault3.Click += new System.EventHandler(this.btnDefault3_Clicked);
          // 
          // lblSampleText
          // 
          this.lblSampleText.Location = new System.Drawing.Point(75, 56);
          this.lblSampleText.Name = "lblSampleText";
          this.lblSampleText.Size = new System.Drawing.Size(197, 94);
          this.lblSampleText.TabIndex = 3;
          this.lblSampleText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // grpCaption
          // 
          this.grpCaption.Controls.AddRange(new System.Windows.Forms.Control[] {
                                               this.txtCaption});
          this.grpCaption.Location = new System.Drawing.Point(18, 56);
          this.grpCaption.Name = "grpCaption";
          this.grpCaption.Size = new System.Drawing.Size(340, 65);
          this.grpCaption.TabIndex = 6;
          this.grpCaption.TabStop = false;
          this.grpCaption.Text = "Caption";
          // 
          // btnCancel
          // 
          this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this.btnCancel.Location = new System.Drawing.Point(592, 504);
          this.btnCancel.Name = "btnCancel";
          this.btnCancel.Size = new System.Drawing.Size(103, 28);
          this.btnCancel.TabIndex = 8;
          this.btnCancel.Text = "Cancel";
          this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
          // 
          // grpCode
          // 
          this.grpCode.Controls.AddRange(new System.Windows.Forms.Control[] {
                                              this.txtCode});
          this.grpCode.Location = new System.Drawing.Point(232, 328);
          this.grpCode.Name = "grpCode";
          this.grpCode.Size = new System.Drawing.Size(520, 169);
          this.grpCode.TabIndex = 4;
          this.grpCode.TabStop = false;
          this.grpCode.Text = "Generated Code";
          // 
          // txtCode
          // 
          this.txtCode.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
          this.txtCode.Location = new System.Drawing.Point(5, 22);
          this.txtCode.Multiline = true;
          this.txtCode.Name = "txtCode";
          this.txtCode.ReadOnly = true;
          this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
          this.txtCode.Size = new System.Drawing.Size(491, 137);
          this.txtCode.TabIndex = 0;
          this.txtCode.Text = "MessageBox.Show();";
          // 
          // pictureBox1
          // 
          this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
          this.pictureBox1.Location = new System.Drawing.Point(9, 22);
          this.pictureBox1.Name = "pictureBox1";
          this.pictureBox1.Size = new System.Drawing.Size(281, 159);
          this.pictureBox1.TabIndex = 0;
          this.pictureBox1.TabStop = false;
          // 
          // btnOK
          // 
          this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
          this.btnOK.Location = new System.Drawing.Point(456, 504);
          this.btnOK.Name = "btnOK";
          this.btnOK.Size = new System.Drawing.Size(103, 28);
          this.btnOK.TabIndex = 7;
          this.btnOK.Text = "OK";
          this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
          // 
          // listBoxXML
          // 
          this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
          this.listBox1.Items.AddRange(new object[] {
                                  "None",
                                  "Information",
                                  "Question",
                                  "Exclamation",
                                  "Error"});
          this.listBox1.Location = new System.Drawing.Point(12, 25);
          this.listBox1.Name = "listBox1";
          this.listBox1.Size = new System.Drawing.Size(188, 154);
          this.listBox1.TabIndex = 0;
          this.listBox1.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox1_MeasureItem);
          this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.OnListBox1DrawItem);
          // 
          // grpMessage
          // 
          this.grpMessage.Controls.AddRange(new System.Windows.Forms.Control[] {
                                               this.txtMessage});
          this.grpMessage.Location = new System.Drawing.Point(360, 0);
          this.grpMessage.Name = "grpMessage";
          this.grpMessage.Size = new System.Drawing.Size(392, 119);
          this.grpMessage.TabIndex = 5;
          this.grpMessage.TabStop = false;
          this.grpMessage.Text = "Message";
          // 
          // txtMessage
          // 
          this.txtMessage.Location = new System.Drawing.Point(12, 20);
          this.txtMessage.Multiline = true;
          this.txtMessage.Name = "txtMessage";
          this.txtMessage.Size = new System.Drawing.Size(372, 89);
          this.txtMessage.TabIndex = 0;
          this.txtMessage.Text = "";
          this.txtMessage.TextChanged += new System.EventHandler(this.OnMessageBoxChanged);
          // 
          // cbnUseMFC
          // 
          this.cbnUseMFC.Enabled = false;
          this.cbnUseMFC.Location = new System.Drawing.Point(128, 36);
          this.cbnUseMFC.Name = "cbnUseMFC";
          this.cbnUseMFC.Size = new System.Drawing.Size(95, 14);
          this.cbnUseMFC.TabIndex = 17;
          this.cbnUseMFC.Text = "Use MFC";
          this.cbnUseMFC.CheckedChanged += new System.EventHandler(this.OnMessageBoxChanged);
          // 
          // cbnDeclareIt
          // 
          this.cbnDeclareIt.Enabled = false;
          this.cbnDeclareIt.Location = new System.Drawing.Point(22, 77);
          this.cbnDeclareIt.Name = "cbnDeclareIt";
          this.cbnDeclareIt.Size = new System.Drawing.Size(95, 17);
          this.cbnDeclareIt.TabIndex = 5;
          this.cbnDeclareIt.Text = "Declare It?";
          this.cbnDeclareIt.CheckedChanged += new System.EventHandler(this.OnMessageBoxChanged);
          // 
          // btnDefault3
          // 
          this.btnDefault3.Location = new System.Drawing.Point(77, 109);
          this.btnDefault3.Name = "btnDefault3";
          this.btnDefault3.Size = new System.Drawing.Size(68, 24);
          this.btnDefault3.TabIndex = 5;
          this.btnDefault3.Text = "button3";
          this.btnDefault3.Visible = false;
          this.btnDefault3.Click += new System.EventHandler(this.btnDefault3_OnClick);
          // 
          // lblCaption
          // 
          this.lblCaption.BackColor = System.Drawing.SystemColors.ActiveCaption;
          this.lblCaption.ForeColor = System.Drawing.SystemColors.HighlightText;
          this.lblCaption.Location = new System.Drawing.Point(9, 22);
          this.lblCaption.Name = "lblCaption";
          this.lblCaption.Size = new System.Drawing.Size(279, 19);
          this.lblCaption.TabIndex = 1;
          this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // lblVariable
          // 
          this.lblVariable.Enabled = false;
          this.lblVariable.Location = new System.Drawing.Point(7, 50);
          this.lblVariable.Name = "lblVariable";
          this.lblVariable.Size = new System.Drawing.Size(89, 21);
          this.lblVariable.TabIndex = 3;
          this.lblVariable.Text = "Variable Name";
          this.lblVariable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
          // 
          // btnPreview
          // 
          this.btnPreview.Location = new System.Drawing.Point(328, 504);
          this.btnPreview.Name = "btnPreview";
          this.btnPreview.Size = new System.Drawing.Size(103, 28);
          this.btnPreview.TabIndex = 10;
          this.btnPreview.Text = "Test";
          this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
          // 
          // cbnVisualCPP
          // 
          this.cbnVisualCPP.AutoCheck = false;
          this.cbnVisualCPP.Location = new System.Drawing.Point(128, 16);
          this.cbnVisualCPP.Name = "cbnVisualCPP";
          this.cbnVisualCPP.Size = new System.Drawing.Size(102, 15);
          this.cbnVisualCPP.TabIndex = 15;
          this.cbnVisualCPP.Text = "Visual C++";
          this.cbnVisualCPP.Click += new System.EventHandler(this.OnLanguageChanged);
          // 
          // cbnDefault1
          // 
          this.cbnDefault1.AutoCheck = false;
          this.cbnDefault1.Location = new System.Drawing.Point(34, 37);
          this.cbnDefault1.Name = "cbnDefault1";
          this.cbnDefault1.Size = new System.Drawing.Size(17, 17);
          this.cbnDefault1.TabIndex = 0;
          this.cbnDefault1.Click += new System.EventHandler(this.btnDefault1_Clicked);
          // 
          // btnDefault1
          // 
          this.btnDefault1.Location = new System.Drawing.Point(77, 34);
          this.btnDefault1.Name = "btnDefault1";
          this.btnDefault1.Size = new System.Drawing.Size(68, 25);
          this.btnDefault1.TabIndex = 3;
          this.btnDefault1.Text = "OK";
          this.btnDefault1.Click += new System.EventHandler(this.btnDefault1_OnClick);
          // 
          // groupBox1
          // 
          this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                              this.listBox1});
          this.groupBox1.Location = new System.Drawing.Point(8, 336);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(214, 187);
          this.groupBox1.TabIndex = 12;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "Message Box Icon";
          // 
          // btnDefault2
          // 
          this.btnDefault2.Location = new System.Drawing.Point(77, 71);
          this.btnDefault2.Name = "btnDefault2";
          this.btnDefault2.Size = new System.Drawing.Size(68, 24);
          this.btnDefault2.TabIndex = 4;
          this.btnDefault2.Text = "button2";
          this.btnDefault2.Visible = false;
          this.btnDefault2.Click += new System.EventHandler(this.btnDefault2_OnClick);
          // 
          // cbnUseReturnVar
          // 
          this.cbnUseReturnVar.Location = new System.Drawing.Point(22, 25);
          this.cbnUseReturnVar.Name = "cbnUseReturnVar";
          this.cbnUseReturnVar.Size = new System.Drawing.Size(197, 17);
          this.cbnUseReturnVar.TabIndex = 2;
          this.cbnUseReturnVar.Text = "Use return variable";
          this.cbnUseReturnVar.CheckedChanged += new System.EventHandler(this.cbnUseReturnVar_OnCheckChanged);
          // 
          // txtVariable
          // 
          this.txtVariable.Enabled = false;
          this.txtVariable.Location = new System.Drawing.Point(104, 50);
          this.txtVariable.Name = "txtVariable";
          this.txtVariable.Size = new System.Drawing.Size(90, 20);
          this.txtVariable.TabIndex = 1;
          this.txtVariable.Text = "mbResult";
          this.txtVariable.TextChanged += new System.EventHandler(this.OnMessageBoxChanged);
          // 
          // grpDefault
          // 
          this.grpDefault.Controls.AddRange(new System.Windows.Forms.Control[] {
                                               this.btnDefault3,
                                               this.btnDefault2,
                                               this.btnDefault1,
                                               this.cbnDefault3,
                                               this.cbnDefault2,
                                               this.cbnDefault1});
          this.grpDefault.Enabled = false;
          this.grpDefault.Location = new System.Drawing.Point(11, 59);
          this.grpDefault.Name = "grpDefault";
          this.grpDefault.Size = new System.Drawing.Size(185, 141);
          this.grpDefault.TabIndex = 2;
          this.grpDefault.TabStop = false;
          this.grpDefault.Text = "Select Default Button";
          // 
          // grpButtons
          // 
          this.grpButtons.Controls.AddRange(new System.Windows.Forms.Control[] {
                                               this.grpDefault,
                                               this.cboButtons});
          this.grpButtons.Location = new System.Drawing.Point(8, 128);
          this.grpButtons.Name = "grpButtons";
          this.grpButtons.Size = new System.Drawing.Size(207, 205);
          this.grpButtons.TabIndex = 1;
          this.grpButtons.TabStop = false;
          this.grpButtons.Text = "Buttons";
          // 
          // cboButtons
          // 
          this.cboButtons.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.cboButtons.DropDownWidth = 174;
          this.cboButtons.Items.AddRange(new object[] {
                                  "OK",
                                  "OK/Cancel",
                                  "Retry/Cancel",
                                  "Abort/Retry/Ignore",
                                  "Yes/No",
                                  "Yes/No/Cancel"});
          this.cboButtons.Location = new System.Drawing.Point(11, 25);
          this.cboButtons.Name = "cboButtons";
          this.cboButtons.Size = new System.Drawing.Size(185, 21);
          this.cboButtons.TabIndex = 0;
          this.cboButtons.SelectedIndexChanged += new System.EventHandler(this.cboButtons_OnSelectedIndexChanged);
          // 
          // grpOptions
          // 
          this.grpOptions.Controls.AddRange(new System.Windows.Forms.Control[] {
                                               this.cbnDeclareIt,
                                               this.cbnBuildSwitch,
                                               this.lblVariable,
                                               this.cbnUseReturnVar,
                                               this.txtVariable});
          this.grpOptions.Location = new System.Drawing.Point(544, 128);
          this.grpOptions.Name = "grpOptions";
          this.grpOptions.Size = new System.Drawing.Size(208, 131);
          this.grpOptions.TabIndex = 0;
          this.grpOptions.TabStop = false;
          this.grpOptions.Text = "Options";
          // 
          // cbnVisualBasic
          // 
          this.cbnVisualBasic.AutoCheck = false;
          this.cbnVisualBasic.Location = new System.Drawing.Point(234, 16);
          this.cbnVisualBasic.Name = "cbnVisualBasic";
          this.cbnVisualBasic.Size = new System.Drawing.Size(102, 15);
          this.cbnVisualBasic.TabIndex = 16;
          this.cbnVisualBasic.Text = "Visual Basic";
          this.cbnVisualBasic.Click += new System.EventHandler(this.OnLanguageChanged);
          // 
          // grpSample
          // 
          this.grpSample.Controls.AddRange(new System.Windows.Forms.Control[] {
                                              this.btnSample4,
                                              this.btnSample2,
                                              this.btnSample5,
                                              this.btnSample3,
                                              this.btnSample1,
                                              this.lblSampleText,
                                              this.pbIcon,
                                              this.lblCaption,
                                              this.pictureBox1});
          this.grpSample.Location = new System.Drawing.Point(232, 128);
          this.grpSample.Name = "grpSample";
          this.grpSample.Size = new System.Drawing.Size(299, 196);
          this.grpSample.TabIndex = 13;
          this.grpSample.TabStop = false;
          this.grpSample.Text = "Preview";
          // 
          // imageList1
          // 
          this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
          this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
          this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
          // 
          // MainForm
          // 
          this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
          this.ClientSize = new System.Drawing.Size(760, 541);
          this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                          this.cbnUseMFC,
                                          this.cbnVisualBasic,
                                          this.cbnVisualCPP,
                                          this.cbnVisualCSharp,
                                          this.groupBox1,
                                          this.btnPreview,
                                          this.btnCancel,
                                          this.btnOK,
                                          this.grpCaption,
                                          this.grpMessage,
                                          this.grpButtons,
                                          this.grpSend,
                                          this.grpOptions,
                                          this.grpCode,
                                          this.grpSample});
          this.MaximizeBox = false;
          this.Name = "MainForm";
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
          this.Text = "Message Box Builder";
          this.grpSend.ResumeLayout(false);
          this.grpCaption.ResumeLayout(false);
          this.grpCode.ResumeLayout(false);
          this.grpMessage.ResumeLayout(false);
          this.groupBox1.ResumeLayout(false);
          this.grpDefault.ResumeLayout(false);
          this.grpButtons.ResumeLayout(false);
          this.grpOptions.ResumeLayout(false);
          this.grpSample.ResumeLayout(false);
          this.ResumeLayout(false);

        }


        int SelectedIcon = 0;
        private void InitForm ()
        {
          cboButtons.SelectedIndex = 0;
          listBox1.SelectedIndex = SelectedIcon;
          txtCode.BackColor = Color.White;
          imageList1.Images.Add (System.Drawing.SystemIcons.Information);
          imageList1.Images.Add (System.Drawing.SystemIcons.Question);
          imageList1.Images.Add (System.Drawing.SystemIcons.Exclamation);
          imageList1.Images.Add (System.Drawing.SystemIcons.Error);
        }

        protected void btnCancel_Click (object sender, System.EventArgs e)
        {
          this.Close();
        }

        protected void btnOK_Click (object sender, System.EventArgs e)
        {      
          if (rdoClipboard.Checked)
          {
    // Clone the generated code to the clipboard.
            Clipboard.SetDataObject (txtCode.Text, true);
          }
          else
          {
    // Write the generated code to a file.
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.OverwritePrompt = true;
            if (dlg.ShowDialog() == DialogResult.Cancel)
              return;
            string strName=dlg.FileName;
            FileStream strm = null;
            StreamWriter writer = null;
            while (true)
            {
              try
              {
                strm = new FileStream(strName,FileMode.Create,FileAccess.Write);
              }
              catch (Exception)
              {
                DialogResult result = MessageBox.Show("Cannot open file. Press Ignore to exit without saving.","Warning", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch(result)
                {
                  case DialogResult.Abort:
                    return;
                  case DialogResult.Retry:
                    continue;
                  case DialogResult.Ignore:
                    this.Close();
                    return;
                }            
              }
              try
              {
                writer = new StreamWriter(strm);
                writer.WriteLine(txtCode.Text);
                writer.Flush();
              }
              catch
              {
                DialogResult result = MessageBox.Show("Could not write to file. Press Ignore to exit anyway","Write failed", MessageBoxButtons.AbortRetryIgnore);
                switch(result)
                {
                  case DialogResult.Abort:
                    writer.Close();
                    strm.Close();
                    return;
                  case DialogResult.Retry:
                    writer.Close();
                    strm.Close();
                    continue;
                  case DialogResult.Ignore:
                    break;
                }            
              }
              writer.Close();
              strm.Close();
              break;
            }
          }
          this.Close();
        }
        private void btnDefault1_Clicked(object sender, System.EventArgs e)
        {
          cbnDefault1.Checked ^= true;
          cbnDefault2.Checked = false;
          cbnDefault3.Checked = false;
          OnMessageBoxChanged (sender, e);
        }

        private void btnDefault2_Clicked(object sender, System.EventArgs e)
        {
          cbnDefault1.Checked = false;
          cbnDefault2.Checked ^= true;
          cbnDefault3.Checked = false;
          OnMessageBoxChanged (sender, e);
        }

        private void btnDefault3_Clicked(object sender, System.EventArgs e)
        {
          cbnDefault1.Checked = false;
          cbnDefault2.Checked = false;
          cbnDefault3.Checked ^= true;
          OnMessageBoxChanged (sender, e);
        }

        private void OnMessageBoxChanged(object sender, System.EventArgs e)
        {
          txtCode.Text = BuildMessageBox();  
          txtCode.SelectionStart = 0;
          txtCode.SelectionLength = 0;
          lblSampleText.Text = txtMessage.Text;
        }

        protected void txtCaption_TextChanged (object sender, System.EventArgs e)
        {
          lblCaption.Text = txtCaption.Text;
          OnMessageBoxChanged (sender, e);
        }

        protected void txtMessage_TextChanged (object sender, System.EventArgs e)
        {
          lblSampleText.Text = txtMessage.Text;
          OnMessageBoxChanged (sender, e);
        }

        protected void cboButtons_OnSelectedIndexChanged (object sender, System.EventArgs e)
        {
          grpDefault.Enabled=true;
          int index=cboButtons.SelectedIndex;
          switch (index)
          {
            case 0:      // OK
              btnDefault1.Visible = false;
              btnDefault2.Visible = false;
              btnDefault3.Visible = false;
              cbnDefault1.Checked = false;
              cbnDefault2.Checked = false;
              cbnDefault3.Checked = false;
              cbnDefault1.Visible = false;
              cbnDefault2.Visible = false;
              cbnDefault3.Visible = false;
              this.pbIcon.Visible = false;
              btnSample1.Visible = false;
              btnSample2.Visible = false;
              btnSample3.Visible = true;
              btnSample4.Visible = false;
              btnSample5.Visible = false;
              btnSample3.Text = "OK";
              break;
            case 1:    // OK/Cancel
              cbnDefault1.Checked = false;
              cbnDefault2.Checked = false;
              cbnDefault3.Checked = false;
              btnDefault1.Text = "OK";
              btnDefault2.Text = "Cancel";
              btnDefault1.Visible = true;
              btnDefault2.Visible = true;
              btnDefault3.Visible = false;
              cbnDefault1.Visible = true;
              cbnDefault2.Visible = true;
              cbnDefault3.Visible = false;
              txtCode.Text = BuildMessageBox();  
              btnSample1.Visible = false;
              btnSample2.Visible = true;
              btnSample3.Visible = false;
              btnSample4.Visible = true;
              btnSample5.Visible = false;
              btnSample2.Text = "OK";
              btnSample4.Text = "Cancel";
              break;
            case 2:      // Retry/Cancel
              cbnDefault1.Checked = false;
              cbnDefault2.Checked = false;
              cbnDefault3.Checked = false;
              btnDefault1.Text = "Retry";
              btnDefault2.Text = "Cancel";
              btnDefault1.Visible = true;
              btnDefault2.Visible = true;
              btnDefault3.Visible = false;
              cbnDefault1.Visible = true;
              cbnDefault2.Visible = true;
              cbnDefault3.Visible = false;
              txtCode.Text=BuildMessageBox();  
              btnSample1.Visible = false;
              btnSample2.Visible = true;
              btnSample3.Visible = false;
              btnSample4.Visible = true;
              btnSample5.Visible = false;
              btnSample2.Text = "Retry";
              btnSample4.Text = "Cancel";
              break;
            case 3:      // Abort/Retry/Ignore
              cbnDefault1.Checked = false;
              cbnDefault2.Checked = false;
              cbnDefault3.Checked = false;
              btnDefault1.Text = "Abort";
              btnDefault2.Text = "Retry";
              btnDefault3.Text = "Cancel";
              btnDefault1.Visible = true;
              btnDefault2.Visible = true;
              btnDefault3.Visible = true;
              cbnDefault1.Visible = true;
              cbnDefault2.Visible = true;
              cbnDefault3.Visible = true;
              txtCode.Text=BuildMessageBox();
              btnSample1.Visible = true;
              btnSample2.Visible = false;
              btnSample3.Visible = true;
              btnSample4.Visible = false;
              btnSample5.Visible = true;
              btnSample1.Text = "Abort";
              btnSample3.Text = "Retry";
              btnSample5.Text = "Ignore";
              break;
            case 4:      // Yes/No
              cbnDefault1.Checked = false;
              cbnDefault2.Checked = false;
              cbnDefault3.Checked = false;
              btnDefault1.Text = "Yes";
              btnDefault2.Text = "No";
              btnDefault1.Visible = true;
              btnDefault2.Visible = true;
              btnDefault3.Visible = false;
              cbnDefault1.Visible = true;
              cbnDefault2.Visible = true;
              cbnDefault3.Visible = false;
              txtCode.Text=BuildMessageBox();  
              btnSample1.Visible = false;
              btnSample2.Visible = true;
              btnSample3.Visible = false;
              btnSample4.Visible = true;
              btnSample5.Visible = false;
              btnSample2.Text = "Yes";
              btnSample4.Text = "No";
              break;
            case 5:    // Yes/No/Cancel
              cbnDefault1.Checked = false;
              cbnDefault2.Checked = false;
              cbnDefault3.Checked = false;
              btnDefault1.Text = "Yes";
              btnDefault2.Text = "No";
              btnDefault3.Text = "Cancel";
              btnDefault1.Visible = true;
              btnDefault2.Visible = true;
              btnDefault3.Visible = true;
              cbnDefault1.Visible = true;
              cbnDefault2.Visible = true;
              cbnDefault3.Visible = true;
              txtCode.Text=BuildMessageBox();  
              btnSample1.Visible = true;
              btnSample2.Visible = false;
              btnSample3.Visible = true;
              btnSample4.Visible = false;
              btnSample5.Visible = true;
              btnSample1.Text = "Yes";
              btnSample3.Text = "No";
              btnSample5.Text = "Cancel";
              break;
          }
          string str = BuildMessageBox();  
          OnMessageBoxChanged (sender, e);
        }
        private string BuildMessageBox()
        {
          string result = "";
          if (cbnVisualBasic.Checked)
          {
            result = BuildForVisualBasic();
          }
          else if (cbnVisualCPP.Checked)
            result = BuildForCPlusPlus();
          else
            result = BuildForCSharp();
          return (result);
        }
        private string BuildForCPlusPlus()
        {
          string strButtons = "";
          string result = "";
          string strSwitch="";
          if (cbnUseMFC.Checked)
            result = "AfxMessageBox(\"";
          else
            result = "MessageBox (NULL, \"";
          string [] lines = txtMessage.Lines;
          for (int x = 0; x < lines.Length; ++x)
          {
            result += lines[x];
            if (x < (lines.Length - 1))
              result += "\\n";
          }
          result+="\",\"";
          result+=txtCaption.Text+"\"";
          switch(cboButtons.SelectedIndex)
          {
            default:
              break;
            case 0:
              strSwitch = "\tcase IDOK:";
              strSwitch += "\r\n\t\tbreak;";
              break;
            case 1:      // OK/Cancel
              strButtons = "MB_OKCANCEL";
              btnStyle=MessageBoxButtons.OKCancel;
              strSwitch ="\tcase IDOK:";
              strSwitch+="\r\n\t\tbreak;";
              strSwitch+="\r\n\tcase IDCANCEL:";
              strSwitch+="\r\n\t\tbreak;";
              break;
            case 2:      //Retry/Cancel
              strButtons = "MB_RETRYCANCEL";
              btnStyle=MessageBoxButtons.RetryCancel;
              strSwitch ="\tcase IDRETRY:";
              strSwitch+="\r\n\t\tbreak;";
              strSwitch+="\r\n\tcase IDCANCEL:";
              strSwitch+="\r\n\t\tbreak;";
              break;
            case 3://Abort/Retry/Ignore
              strButtons="MB_ABORTRETRYIGNORE";
              btnStyle=MessageBoxButtons.AbortRetryIgnore;
              strSwitch ="\tcase IDABORT:";
              strSwitch+="\r\n\t\tbreak;";
              strSwitch+="\r\n\tcase IDRETRY:";
              strSwitch+="\r\n\t\tbreak;";
              strSwitch+="\r\n\tcase IDIGNORE:";
              strSwitch+="\r\n\t\tbreak;";
              break;
            case 4:    // Yes/No
              strButtons = "MB_YESNO";
              btnStyle = MessageBoxButtons.YesNo;
              strSwitch ="\tcase IDYES:";
              strSwitch+="\r\n\t\tbreak;";
              strSwitch+="\r\n\tcase IDNO:";
              strSwitch+="\r\n\t\tbreak;";
              break;
            case 5://Yes/No/Cancel
              strButtons = "MB_YESNOCANCEL";
              btnStyle=MessageBoxButtons.YesNoCancel;
              strSwitch ="\tcase IDYES:";
              strSwitch+="\r\n\t\tbreak;";
              strSwitch+="\r\n\tcase IDNO:";
              strSwitch+="\r\n\t\tbreak;";
              strSwitch+="\r\n\tcase IDCANCEL:";
              strSwitch+="\r\n\t\tbreak;";
              break;          
          }
          if ((listBox1.SelectedIndex > 0) && (strButtons != ""))
            strButtons += " | ";
          switch (listBox1.SelectedIndex)
          {
            case 0:
              this.pbIcon.Visible = false;
              break;
            case 1:
              strButtons += "MB_ICONINFORMATION";
              iconStyle = MessageBoxIcon.Information;
              this.pbIcon.Visible = true;
              this.pbIcon.Image = ((System.Drawing.Bitmap)(imageList1.Images[0]));
              break;
            case 2:
              strButtons += "MB_ICONQUESTION";
              iconStyle = MessageBoxIcon.Question;
              this.pbIcon.Visible = true;
              this.pbIcon.Image = ((System.Drawing.Bitmap)(imageList1.Images[1]));
              break;
            case 3:
              strButtons += "MB_ICONEXCLAMATION";
              iconStyle = MessageBoxIcon.Exclamation;
              this.pbIcon.Visible = true;
              this.pbIcon.Image = ((System.Drawing.Bitmap)(imageList1.Images[2]));
              break;
            case 4:
              strButtons += "MB_ICONERROR";
              iconStyle = MessageBoxIcon.Error;
              this.pbIcon.Visible = true;
              this.pbIcon.Image = ((System.Drawing.Bitmap)(imageList1.Images[3]));
              break;
          }
          int iDefButton = 0;
          if (cbnDefault1.Checked)
            iDefButton = 1;
          if (cbnDefault2.Checked)
            iDefButton = 2;
          if (cbnDefault3.Checked)
            iDefButton = 3;
          if (iDefButton > 0)
          {
            if (strButtons != "")
              strButtons += " | ";
            strButtons += "MB_DEFBUTTON";
            strButtons += iDefButton.ToString();
          }
          switch (iDefButton)
          {
            case 1:
              btnDefault = MessageBoxDefaultButton.Button1;
              break;
            case 2:
              btnDefault = MessageBoxDefaultButton.Button2;
              break;
            case 3:
              btnDefault = MessageBoxDefaultButton.Button3;
              break;
          }
          if (strButtons != "")
          {
            result += ", ";
            result += strButtons;
          }
          result += ");";
          if (cbnUseReturnVar.Checked)
          {
            if (cbnDeclareIt.Checked)
              result = "int " + txtVariable.Text + " = " + result;
            else
              result = txtVariable.Text + " = " + result;
            if (cbnBuildSwitch.Checked)
            {          
              result += "\r\n";
              result += "switch(" + txtVariable.Text + ")";
              result += "\r\n{";
              result += "\r\n";          
              result += strSwitch;
              result += "\r\n}";
            }
          }
          return (result);
        }
        private string BuildForVisualBasic()
        {
          string strIcon = "";
          string strButtons = "";
          string strDefButton = "";
          string strSwitch = "";
          string result = "";
          if (cbnUseReturnVar.Checked)
          {
            if (cbnDeclareIt.Checked)
              result="dim " + txtVariable.Text + " as int32\r\n";
            result += txtVariable.Text + " = ";
          }
          result += "MessageBox.Show(\"";
          string [] lines = txtMessage.Lines;
          string strUnion;
          if (cbnVisualBasic.Checked)
          {
            strUnion = " ";
          }
          else
          {
            strUnion = "\\n";
          }
          for (int x = 0; x < lines.Length; ++x)
          {
            result += lines[x];
            if (x < (lines.Length - 1))
              result += strUnion;
          }
          result += "\", \"";
          result += txtCaption.Text + "\"";

          ButtonsAreUs (ref strButtons, ref strSwitch, ref strIcon, ref strDefButton);
          if (strButtons.Length > 0)
          {
            result += ", ";
            result += strButtons;
          }
          if (strIcon.Length > 0)
          {
            result += ", ";
            result += strIcon;
          }
          if (strDefButton.Length > 0)
          {
            result += ", ";
            result += strDefButton;
          }
          result += ")";
          if (cbnUseReturnVar.Checked && cbnBuildSwitch.Checked)
          {
            result += "\r\n";
            strSwitch = "";
            strSwitch = "Select " + txtVariable.Text + "\r\n";
            switch(cboButtons.SelectedIndex)
            {
              case 0:    //OK
                strSwitch += "\tCase DialogResult.OK\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                break;
              case 1:    //OK/Cancel
                strSwitch += "\tCase DialogResult.OK\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                strSwitch += "\tCase DialogResult.Cancel\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                break;
              case 2:    //Retry/Cancel
                strSwitch += "\tCase DialogResult.Retry\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                strSwitch += "\tCase DialogResult.Cancel\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                break;
              case 3:    //Abort/Retry/Ignore
                strSwitch += "\tCase DialogResult.Abort\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                strSwitch += "\tCase DialogResult.Retry\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                strSwitch += "\tCase DialogResult.Ignore\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                break;
              case 4://Yes/No
                strSwitch += "\tCase DialogResult.Yes\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                strSwitch += "\tCase DialogResult.No\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                break;
              case 5://Yes/No/Cancel
                strSwitch += "\tCase DialogResult.Yes\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                strSwitch += "\tCase DialogResult.No\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                strSwitch += "\tCase DialogResult.Cancel\r\n";
                strSwitch += "\t\t' Add your code here\r\n";
                break;          
            }
            strSwitch += "End Select\r\n";
            result += strSwitch;
          }
          return (result);
        }
        private string BuildForVisualBasicOld()
        {
          string strIcon = "";
          string strButtons = "";
          string strDefButton = "";
          string strSwitch = "";
          string result = "";
          if (cbnUseReturnVar.Checked)
          {
            if (cbnDeclareIt.Checked)
              result="dim " + txtVariable.Text + " as int32\r\n";
            result += txtVariable.Text + " = ";
          }
          result += "MessageBox.Show(\"";
          string [] lines = txtMessage.Lines;
          string strUnion;
          if (cbnVisualBasic.Checked)
          {
            strUnion = " ";
          }
          else
          {
            strUnion = "\\n";
          }
          for (int x = 0; x < lines.Length; ++x)
          {
            result += lines[x];
            if (x < (lines.Length - 1))
              result += strUnion;
          }
          result += "\", \"";
          result += txtCaption.Text + "\"";

          ButtonsAreUs (ref strButtons, ref strSwitch, ref strIcon, ref strDefButton);
          if (strButtons.Length > 0)
          {
            result += ", ";
            result += strButtons;
          }
          if (strIcon.Length > 0)
          {
            result += ", ";
            result += strIcon;
          }
          if (strDefButton.Length > 0)
          {
            result += ", ";
            result += strDefButton;
          }
          result += ")";
          if (cbnUseReturnVar.Checked && cbnBuildSwitch.Checked)
          {
            result += "\r\n";
            strSwitch = "";
            switch(cboButtons.SelectedIndex)
            {
              case 0:    //OK
                strSwitch = "If " + txtVariable.Text + " = dialogresult.OK Then\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "End If";
                break;
              case 1:    //OK/Cancel
                strSwitch = "If " + txtVariable.Text + " = DialogResult.OK Then\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "ElseIf " + txtVariable.Text + " = DialogResult.Cancel\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "End If";
                break;
              case 2:    //Retry/Cancel
                strSwitch = "If " + txtVariable.Text + " = DialogResult.Retry Then\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "ElseIf " + txtVariable.Text + " = DialogResult.Cancel\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "End If";
                break;
              case 3:    //Abort/Retry/Ignore
                strSwitch = "If " + txtVariable.Text + " = DialogResult.Abort Then\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "ElseIf " + txtVariable.Text + " = DialogResult.Retry\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "ElseIf " + txtVariable.Text + " = DialogResult.Ignore\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "End If";
                break;
              case 4://Yes/No
                strSwitch = "If " + txtVariable.Text + " = DialogResult.Yes Then\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "ElseIf " + txtVariable.Text + " = DialogResult.No\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "End If";
                break;
              case 5://Yes/No/Cancel
                strSwitch = "If " + txtVariable.Text + " = DialogResult.Yes Then\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "ElseIf " + txtVariable.Text + " = DialogResult.No\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "ElseIf " + txtVariable.Text + " = DialogResult.Cancel\r\n";
                strSwitch += "\t' Add your code here\r\n";
                strSwitch += "End If";
                break;          
            }
            result += strSwitch;
          }
          return (result);
        }
        private string BuildForCSharp()
        {
          string strIcon = "";
          string strButtons = "";
          string strDefButton = "";
          string result = "";
          result="MessageBox.Show(\"";

          string [] lines = txtMessage.Lines;
          for (int x = 0; x < lines.Length; ++x)
          {
            result += lines[x];
            if (x < (lines.Length - 1))
              result += "\\n";
          }
          string strSwitch="";

          result+="\",\"";
          result+=txtCaption.Text+"\"";
          ButtonsAreUs (ref strButtons, ref strSwitch, ref strIcon, ref strDefButton);
          if (strButtons.Length > 0)
          {
            result += ", ";
            result += strButtons;
          }
          if (strIcon.Length > 0)
          {
            result += ", ";
            result += strIcon;
          }
          if (strDefButton.Length > 0)
          {
            result += ", ";
            result += strDefButton;
          }

          result+=");";

          if (cbnUseReturnVar.Checked)
          {
            if (cbnDeclareIt.Checked)
              result = "DialogResult " + txtVariable.Text + " = " + result;
            else
              result = txtVariable.Text + " = " + result;
            if (cbnBuildSwitch.Checked)
            {          
              result += "\r\n";
              result += "switch (" + txtVariable.Text + ")";
              result += "\r\n{";
              result += "\r\n";          
              result += strSwitch;
              result += "\r\n}";
            }
          }
          return (result);
        }

        private void ButtonsAreUs (ref string strButtons, ref string strSwitch, ref string strIcon, ref string strDefButton)
        {
          if (cboButtons.SelectedIndex!=-1)
          {
            switch(cboButtons.SelectedIndex)
            {
              case 0:      //OK
                strSwitch ="\tcase DialogResult.OK:";
                strSwitch+="\r\n\t\tbreak;";
                break;
              case 1:      //OK/Cancel
                strButtons = "MessageBoxButtons.OKCancel";
                btnStyle = MessageBoxButtons.OKCancel;
                strSwitch = "\tcase DialogResult.OK :";
                strSwitch += "\r\n\t\tbreak;";
                strSwitch += "\r\n\tcase DialogResult.Cancel :";
                strSwitch += "\r\n\t\tbreak;";
                break;
              case 2:      //Retry/Cancel
                strButtons = "MessageBoxButtons.RetryCancel";
                btnStyle=MessageBoxButtons.RetryCancel;
                strSwitch ="\tcase DialogResult.Retry :";
                strSwitch+="\r\n\t\tbreak;";
                strSwitch+="\r\n\tcase DialogResult.Cancel :";
                strSwitch+="\r\n\t\tbreak;";
                break;
              case 3:      //Abort/Retry/Ignore
                strButtons="MessageBoxButtons.AbortRetryIgnore";            
                btnStyle=MessageBoxButtons.AbortRetryIgnore;
                strSwitch ="\tcase DialogResult.Abort :";
                strSwitch+="\r\n\t\tbreak;";
                strSwitch+="\r\n\tcase DialogResult.Retry :";
                strSwitch+="\r\n\t\tbreak;";
                strSwitch+="\r\n\tcase DialogResult.Ignore :";
                strSwitch+="\r\n\t\tbreak;";
                break;
              case 4:      //Yes/No
                strButtons = "MessageBoxButtons.YesNo";
                btnStyle = MessageBoxButtons.YesNo;
                strSwitch ="\tcase DialogResult.Yes :";
                strSwitch+="\r\n\t\tbreak;";
                strSwitch+="\r\n\tcase DialogResult.No :";
                strSwitch+="\r\n\t\tbreak;";
                break;
              case 5:      //Yes/No/Cancel
                strButtons = "MessageBoxButtons.YesNoCancel";
                btnStyle=MessageBoxButtons.YesNoCancel;
                strSwitch ="\tcase DialogResult.Yes :";
                strSwitch+="\r\n\t\tbreak;";
                strSwitch+="\r\n\tcase DialogResult.No :";
                strSwitch+="\r\n\t\tbreak;";
                strSwitch+="\r\n\tcase DialogResult.Cancel :";
                strSwitch+="\r\n\t\tbreak;";
                break;          
            }      

            if (cbnDefault1.Checked)
            {
              strDefButton = "MessageBoxDefaultButton.Button1";
              btnDefault = MessageBoxDefaultButton.Button1;
            }
            else if (cbnDefault2.Checked)
            {
              strDefButton = "MessageBoxDefaultButton.Button2";
              btnDefault = MessageBoxDefaultButton.Button2;
            }
            else if (cbnDefault3.Checked)
            {
              strDefButton = "MessageBoxDefaultButton.Button3";
              btnDefault = MessageBoxDefaultButton.Button3;
            }
          }
          switch (listBox1.SelectedIndex)
          {
            case 0:
              this.pbIcon.Visible = false;
              break;
            case 1:
              strIcon = "MessageBoxIcon.Information";        
              iconStyle = MessageBoxIcon.Information;
              this.pbIcon.Visible = true;
              this.pbIcon.Image = ((System.Drawing.Bitmap)(imageList1.Images[0]));
              break;
            case 2:
              strIcon = "MessageBoxIcon.Question";        
              iconStyle = MessageBoxIcon.Question;
              this.pbIcon.Visible = true;
              this.pbIcon.Image = ((System.Drawing.Bitmap)(imageList1.Images[1]));
              break;
            case 3:
              strIcon = "MessageBoxIcon.Exclamation";        
              iconStyle = MessageBoxIcon.Exclamation;
              this.pbIcon.Visible = true;
              this.pbIcon.Image = ((System.Drawing.Bitmap)(imageList1.Images[2]));
              break;
            case 4:
              strIcon = "MessageBoxIcon.Error";
              iconStyle = MessageBoxIcon.Error;
              this.pbIcon.Visible = true;
              this.pbIcon.Image = ((System.Drawing.Bitmap)(imageList1.Images[3]));
              break;
          }
    //
    // Be sure there are no empty parameters
          if ((strDefButton != "") && (strIcon == ""))
          {
            iconStyle = MessageBoxIcon.None;
            strIcon = "MessageBoxIcon.None";
          }
          if ((strIcon != "") && (strButtons == ""))
          {
            strButtons = "MessageBoxButtons.OK";
            btnStyle = MessageBoxButtons.OK;
          }
        }

        protected void btnPreview_Click (object sender, System.EventArgs e)
        {
          OnMessageBoxChanged (sender, e);
          string str = txtMessage.Text.ToString();
          MessageBox.Show(txtMessage.Text,txtCaption.Text,btnStyle, iconStyle, btnDefault);
        }

        protected void cbnUseReturnVar_CheckedChanged (object sender, System.EventArgs e)
        {
          txtVariable.Enabled=cbnUseReturnVar.Checked;  
          cbnBuildSwitch.Enabled=cbnUseReturnVar.Checked;
          lblVariable.Enabled=cbnUseReturnVar.Checked;
          cbnDeclareIt.Enabled = cbnUseReturnVar.Checked;
          OnMessageBoxChanged (sender, e);
        }

        private void OnListBox1DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
          StringFormat format = new StringFormat ();
          format.LineAlignment = StringAlignment.Center;
          Rectangle rc = e.Bounds;
          Brush brush;
          bool bSelected = (e.State & DrawItemState.Selected) != 0;
          string str = listBox1.GetItemText(listBox1.Items[e.Index]);
          int Left = e.Bounds.Left;
          int Top = e.Bounds.Top + 3;
          int Height = 20;
          int Width = 20;
          if (e.Index > 0)
            imageList1.Draw (e.Graphics, Left, Top, Height, Width, e.Index - 1);
          rc.X += Width + 3;
          if (bSelected)
          {
            e.Graphics.FillRectangle (Brushes.DarkBlue, rc);
            brush = Brushes.White;
          }
          else
          {
            e.Graphics.FillRectangle (Brushes.White, rc);
            brush = Brushes.Black;
          }
          e.Graphics.DrawString (str, listBox1.Font, brush, rc, format);
          OnMessageBoxChanged (sender, e);
        }

        private void listBox1_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
        {
          e.ItemHeight = 24;
          e.ItemWidth = listBox1.Width;
        }

        private void OnLanguageChanged(object sender, System.EventArgs e)
        {
          CheckBox cbn = (CheckBox) sender;
          if (cbn == cbnVisualBasic)
          {
            cbnVisualBasic.Checked = true;
            cbnVisualCPP.Checked = false;
            cbnUseMFC.Checked = false;
            cbnUseMFC.Enabled = false;
            cbnVisualCSharp.Checked = false;
          }
          else if (cbn == cbnVisualCSharp)
          {
            cbnVisualBasic.Checked = false;
            cbnVisualCPP.Checked = false;
            cbnUseMFC.Checked = false;
            cbnUseMFC.Enabled = false;
            cbnVisualCSharp.Checked = true;
          }
          else if (cbn == cbnVisualCPP)
          {
            cbnVisualBasic.Checked = false;
            cbnVisualCPP.Checked = true;
            cbnUseMFC.Enabled = true;
            cbnVisualCSharp.Checked = false;
          }
          OnMessageBoxChanged (sender, e);
        }

        private void cbnUseReturnVar_OnCheckChanged(object sender, System.EventArgs e)
        {
          cbnDeclareIt.Enabled = cbnUseReturnVar.Checked;
          cbnBuildSwitch.Enabled = cbnUseReturnVar.Checked;
          lblVariable.Enabled = cbnUseReturnVar.Checked;
          txtVariable.Enabled = cbnUseReturnVar.Checked;
          OnMessageBoxChanged (sender, e);
        }

        private void btnDefault1_OnClick(object sender, System.EventArgs e)
        {
          cbnDefault1.Checked ^= true;
          cbnDefault2.Checked = false;
          cbnDefault3.Checked = false;
          OnMessageBoxChanged (sender, e);
        }

        private void btnDefault2_OnClick(object sender, System.EventArgs e)
        {
          cbnDefault1.Checked = false;
          cbnDefault2.Checked ^= true;
          cbnDefault3.Checked = false;
          OnMessageBoxChanged (sender, e);
        }

        private void btnDefault3_OnClick(object sender, System.EventArgs e)
        {
          cbnDefault1.Checked = false;
          cbnDefault2.Checked = false;
          cbnDefault3.Checked ^= true;
          OnMessageBoxChanged (sender, e);
        }


    }  // class MessageBoxBuilder

    #endregion   // TEST_CLASSES



} // nmespace IG.Forms.Test



 







