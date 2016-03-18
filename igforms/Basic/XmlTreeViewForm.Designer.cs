namespace IG.Forms
{
    partial class XmlTreeViewForm
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.xmlTreeViewControl1 = new IG.Forms.XmlTreeViewControl();
            this.SuspendLayout();
            // 
            // xmlTreeViewControl1
            // 
            this.xmlTreeViewControl1.AllowDrop = true;
            this.xmlTreeViewControl1.AttrSubnode = true;
            this.xmlTreeViewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlTreeViewControl1.ExpAttr = true;
            this.xmlTreeViewControl1.Location = new System.Drawing.Point(0, 0);
            this.xmlTreeViewControl1.Name = "xmlTreeViewControl1";
            this.xmlTreeViewControl1.Padding = new System.Windows.Forms.Padding(10);
            this.xmlTreeViewControl1.Size = new System.Drawing.Size(676, 528);
            this.xmlTreeViewControl1.TabIndex = 0;
            // 
            // XmlTreeViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 528);
            this.Controls.Add(this.xmlTreeViewControl1);
            this.Icon = global::IG.Forms.Properties.Resources.ig;
            this.Name = "XmlTreeViewForm";
            this.Text = "XML Viewer";
            this.ResumeLayout(false);

        }

        #endregion

        private XmlTreeViewControl xmlTreeViewControl1;
    }
}