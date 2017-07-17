using System.Drawing;
using System.Windows.Forms;


namespace IG.Forms
{

    partial class XmlTreeViewControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlTreeViewControl));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.treeViewXML = new System.Windows.Forms.TreeView();
            this.NodeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextMenu_CopyNodeText = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_CopyFullPathExactly = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_CopyFullPath = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ContextMenu_EditMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ContextMenu_Expand = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandSpecified = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Expand1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Expand2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Expand3 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Expand4 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_Expand5 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtLeast = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtLeastSpecified = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtLeast1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtLeast2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtLeast3 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtLeast4 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtLeast5 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtMost = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtMostSpecified = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtMost1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtMost2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtMost3 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtMost4 = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenu_ExpandAtMost5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Collapse = new System.Windows.Forms.ToolStripMenuItem();
            this.NodeImages = new System.Windows.Forms.ImageList(this.components);
            this.listBoxXML = new System.Windows.Forms.ListBox();
            this.DispPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.richTextBoxXML = new System.Windows.Forms.RichTextBox();
            this.TopPnl = new System.Windows.Forms.Panel();
            this.ControlPnl = new System.Windows.Forms.Panel();
            this.CollAllBtn = new System.Windows.Forms.Button();
            this.TopPnlHideBtn = new System.Windows.Forms.Button();
            this.LevelLbl = new System.Windows.Forms.Label();
            this.ExpAllBtn = new System.Windows.Forms.Button();
            this.LevelNumUpDown = new System.Windows.Forms.NumericUpDown();
            this.ExpBtn = new System.Windows.Forms.Button();
            this.ExpMaxBtn = new System.Windows.Forms.Button();
            this.ExpMinBtn = new System.Windows.Forms.Button();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFile_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFile_Close = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFile_CloseDocument = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTools_Controls = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTools_TestXPath = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuTools_ExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTools_CollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuTools_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuGelp = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelp_About = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomPnl = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusType = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusPath = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusError = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.chkShowText = new System.Windows.Forms.CheckBox();
            this.chkShowList = new System.Windows.Forms.CheckBox();
            this.NodeContextMenu.SuspendLayout();
            this.DispPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.TopPnl.SuspendLayout();
            this.ControlPnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LevelNumUpDown)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.BottomPnl.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "XML FILES| *.xml";
            this.openFileDialog1.InitialDirectory = "c:\\Users\\ajgorhoe\\cvis\\igcs\\XMLview\\0xml\\";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // treeViewXML
            // 
            this.treeViewXML.BackColor = System.Drawing.Color.WhiteSmoke;
            this.treeViewXML.ContextMenuStrip = this.NodeContextMenu;
            this.treeViewXML.Cursor = System.Windows.Forms.Cursors.Hand;
            this.treeViewXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewXML.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewXML.FullRowSelect = true;
            this.treeViewXML.HotTracking = true;
            this.treeViewXML.ImageIndex = 0;
            this.treeViewXML.ImageList = this.NodeImages;
            this.treeViewXML.ItemHeight = 16;
            this.treeViewXML.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.treeViewXML.Location = new System.Drawing.Point(0, 0);
            this.treeViewXML.Name = "treeViewXML";
            this.treeViewXML.SelectedImageIndex = 0;
            this.treeViewXML.ShowNodeToolTips = true;
            this.treeViewXML.Size = new System.Drawing.Size(335, 444);
            this.treeViewXML.TabIndex = 3;
            this.toolTip1.SetToolTip(this.treeViewXML, "This region shows the tree view of the XML document\r\nafter the document is loaded" +
        ".");
            this.treeViewXML.NodeMouseHover += new System.Windows.Forms.TreeNodeMouseHoverEventHandler(this.treeViewXML_NodeMouseHover);
            this.treeViewXML.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewXML_AfterSelect);
            this.treeViewXML.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewXML_NodeMouseClick);
            // 
            // NodeContextMenu
            // 
            this.NodeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextMenu_CopyNodeText,
            this.ContextMenu_CopyFullPathExactly,
            this.ContextMenu_CopyFullPath,
            this.toolStripSeparator2,
            this.toolStripSeparator3,
            this.ContextMenu_EditMenu,
            this.toolStripSeparator4,
            this.ContextMenu_Expand,
            this.ContextMenu_ExpandAtLeast,
            this.ContextMenu_ExpandAtMost,
            this.toolStripSeparator1,
            this.Collapse});
            this.NodeContextMenu.Name = "contextMenuStrip1";
            this.NodeContextMenu.Size = new System.Drawing.Size(191, 204);
            this.NodeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.NodeContextMenu_Opening);
            // 
            // ContextMenu_CopyNodeText
            // 
            this.ContextMenu_CopyNodeText.Name = "ContextMenu_CopyNodeText";
            this.ContextMenu_CopyNodeText.Size = new System.Drawing.Size(190, 22);
            this.ContextMenu_CopyNodeText.Text = "Copy Node Text";
            this.ContextMenu_CopyNodeText.Click += new System.EventHandler(this.ContextMenu_CopyNodeText_Click);
            // 
            // ContextMenu_CopyFullPathExactly
            // 
            this.ContextMenu_CopyFullPathExactly.Name = "ContextMenu_CopyFullPathExactly";
            this.ContextMenu_CopyFullPathExactly.Size = new System.Drawing.Size(190, 22);
            this.ContextMenu_CopyFullPathExactly.Text = "Copy Full Path Exactly";
            this.ContextMenu_CopyFullPathExactly.Click += new System.EventHandler(this.ContextMenu_CopyFullPathExactly_Click);
            // 
            // ContextMenu_CopyFullPath
            // 
            this.ContextMenu_CopyFullPath.Name = "ContextMenu_CopyFullPath";
            this.ContextMenu_CopyFullPath.Size = new System.Drawing.Size(190, 22);
            this.ContextMenu_CopyFullPath.Text = "Copy Path (Attribute)";
            this.ContextMenu_CopyFullPath.Click += new System.EventHandler(this.ContextMenu_CopyFullPath_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(187, 6);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(187, 6);
            // 
            // ContextMenu_EditMenu
            // 
            this.ContextMenu_EditMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextMenu_Edit,
            this.ContextMenu_Cut,
            this.ContextMenu_Copy,
            this.ContextMenu_Paste,
            this.ContextMenu_Delete});
            this.ContextMenu_EditMenu.Name = "ContextMenu_EditMenu";
            this.ContextMenu_EditMenu.Size = new System.Drawing.Size(190, 22);
            this.ContextMenu_EditMenu.Text = "Edit";
            this.ContextMenu_EditMenu.Click += new System.EventHandler(this.ContextMenu_Edit_Click);
            // 
            // ContextMenu_Edit
            // 
            this.ContextMenu_Edit.Name = "ContextMenu_Edit";
            this.ContextMenu_Edit.Size = new System.Drawing.Size(107, 22);
            this.ContextMenu_Edit.Text = "Edit";
            this.ContextMenu_Edit.Click += new System.EventHandler(this.ContextMenu_Edit_Click);
            // 
            // ContextMenu_Cut
            // 
            this.ContextMenu_Cut.Name = "ContextMenu_Cut";
            this.ContextMenu_Cut.Size = new System.Drawing.Size(107, 22);
            this.ContextMenu_Cut.Text = "Cut";
            this.ContextMenu_Cut.Click += new System.EventHandler(this.ContextMenu_Cut_Click);
            // 
            // ContextMenu_Copy
            // 
            this.ContextMenu_Copy.Name = "ContextMenu_Copy";
            this.ContextMenu_Copy.Size = new System.Drawing.Size(107, 22);
            this.ContextMenu_Copy.Text = "Copy";
            this.ContextMenu_Copy.Click += new System.EventHandler(this.ContextMenu_Copy_Click);
            // 
            // ContextMenu_Paste
            // 
            this.ContextMenu_Paste.Name = "ContextMenu_Paste";
            this.ContextMenu_Paste.Size = new System.Drawing.Size(107, 22);
            this.ContextMenu_Paste.Text = "Paste";
            this.ContextMenu_Paste.Click += new System.EventHandler(this.ContextMenu_Paste_Click);
            // 
            // ContextMenu_Delete
            // 
            this.ContextMenu_Delete.Name = "ContextMenu_Delete";
            this.ContextMenu_Delete.Size = new System.Drawing.Size(107, 22);
            this.ContextMenu_Delete.Text = "Delete";
            this.ContextMenu_Delete.Click += new System.EventHandler(this.ContextMenu_Delete_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(187, 6);
            // 
            // ContextMenu_Expand
            // 
            this.ContextMenu_Expand.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextMenu_ExpandAll,
            this.ContextMenu_ExpandSpecified,
            this.ContextMenu_Expand1,
            this.ContextMenu_Expand2,
            this.ContextMenu_Expand3,
            this.ContextMenu_Expand4,
            this.ContextMenu_Expand5});
            this.ContextMenu_Expand.Name = "ContextMenu_Expand";
            this.ContextMenu_Expand.Size = new System.Drawing.Size(190, 22);
            this.ContextMenu_Expand.Text = "Expand";
            // 
            // ContextMenu_ExpandAll
            // 
            this.ContextMenu_ExpandAll.Name = "ContextMenu_ExpandAll";
            this.ContextMenu_ExpandAll.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAll.Text = "All Levels";
            this.ContextMenu_ExpandAll.Click += new System.EventHandler(this.ContextMenu_ExpandAll_Click);
            // 
            // ContextMenu_ExpandSpecified
            // 
            this.ContextMenu_ExpandSpecified.Name = "ContextMenu_ExpandSpecified";
            this.ContextMenu_ExpandSpecified.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandSpecified.Text = "Specified Number of Levels";
            this.ContextMenu_ExpandSpecified.Click += new System.EventHandler(this.ContextMenu_ExpandSpecified_Click);
            // 
            // ContextMenu_Expand1
            // 
            this.ContextMenu_Expand1.Name = "ContextMenu_Expand1";
            this.ContextMenu_Expand1.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_Expand1.Text = "1 Level";
            this.ContextMenu_Expand1.Click += new System.EventHandler(this.ContextMenu_Expand1_Click);
            // 
            // ContextMenu_Expand2
            // 
            this.ContextMenu_Expand2.Name = "ContextMenu_Expand2";
            this.ContextMenu_Expand2.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_Expand2.Text = "2 Levels";
            this.ContextMenu_Expand2.Click += new System.EventHandler(this.ContextMenu_Expand2_Click);
            // 
            // ContextMenu_Expand3
            // 
            this.ContextMenu_Expand3.Name = "ContextMenu_Expand3";
            this.ContextMenu_Expand3.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_Expand3.Text = "3 Levels";
            this.ContextMenu_Expand3.Click += new System.EventHandler(this.ContextMenu_Expand3_Click);
            // 
            // ContextMenu_Expand4
            // 
            this.ContextMenu_Expand4.Name = "ContextMenu_Expand4";
            this.ContextMenu_Expand4.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_Expand4.Text = "4 Levels";
            this.ContextMenu_Expand4.Click += new System.EventHandler(this.ContextMenu_Expand4_Click);
            // 
            // ContextMenu_Expand5
            // 
            this.ContextMenu_Expand5.Name = "ContextMenu_Expand5";
            this.ContextMenu_Expand5.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_Expand5.Text = "5 Levels";
            this.ContextMenu_Expand5.Click += new System.EventHandler(this.ContextMenu_Expand5_Click);
            // 
            // ContextMenu_ExpandAtLeast
            // 
            this.ContextMenu_ExpandAtLeast.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextMenu_ExpandAtLeastSpecified,
            this.ContextMenu_ExpandAtLeast1,
            this.ContextMenu_ExpandAtLeast2,
            this.ContextMenu_ExpandAtLeast3,
            this.ContextMenu_ExpandAtLeast4,
            this.ContextMenu_ExpandAtLeast5});
            this.ContextMenu_ExpandAtLeast.Name = "ContextMenu_ExpandAtLeast";
            this.ContextMenu_ExpandAtLeast.Size = new System.Drawing.Size(190, 22);
            this.ContextMenu_ExpandAtLeast.Text = "Expand at Least";
            // 
            // ContextMenu_ExpandAtLeastSpecified
            // 
            this.ContextMenu_ExpandAtLeastSpecified.Name = "ContextMenu_ExpandAtLeastSpecified";
            this.ContextMenu_ExpandAtLeastSpecified.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtLeastSpecified.Text = "Specified Number of Levels";
            this.ContextMenu_ExpandAtLeastSpecified.Click += new System.EventHandler(this.ContextMenu_ExpandAtLeastSpecified_Click);
            // 
            // ContextMenu_ExpandAtLeast1
            // 
            this.ContextMenu_ExpandAtLeast1.Name = "ContextMenu_ExpandAtLeast1";
            this.ContextMenu_ExpandAtLeast1.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtLeast1.Text = "1 Level";
            this.ContextMenu_ExpandAtLeast1.Click += new System.EventHandler(this.ContextMenu_ExpandAtLeast1_Click);
            // 
            // ContextMenu_ExpandAtLeast2
            // 
            this.ContextMenu_ExpandAtLeast2.Name = "ContextMenu_ExpandAtLeast2";
            this.ContextMenu_ExpandAtLeast2.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtLeast2.Text = "2 Levels";
            this.ContextMenu_ExpandAtLeast2.Click += new System.EventHandler(this.ContextMenu_ExpandAtLeast2_Click);
            // 
            // ContextMenu_ExpandAtLeast3
            // 
            this.ContextMenu_ExpandAtLeast3.Name = "ContextMenu_ExpandAtLeast3";
            this.ContextMenu_ExpandAtLeast3.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtLeast3.Text = "3 Levels";
            this.ContextMenu_ExpandAtLeast3.Click += new System.EventHandler(this.ContextMenu_ExpandAtLeast3_Click);
            // 
            // ContextMenu_ExpandAtLeast4
            // 
            this.ContextMenu_ExpandAtLeast4.Name = "ContextMenu_ExpandAtLeast4";
            this.ContextMenu_ExpandAtLeast4.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtLeast4.Text = "4 Levels";
            this.ContextMenu_ExpandAtLeast4.Click += new System.EventHandler(this.ContextMenu_ExpandAtLeast4_Click);
            // 
            // ContextMenu_ExpandAtLeast5
            // 
            this.ContextMenu_ExpandAtLeast5.Name = "ContextMenu_ExpandAtLeast5";
            this.ContextMenu_ExpandAtLeast5.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtLeast5.Text = "5 Levels";
            this.ContextMenu_ExpandAtLeast5.Click += new System.EventHandler(this.ContextMenu_ExpandAtLeast5_Click);
            // 
            // ContextMenu_ExpandAtMost
            // 
            this.ContextMenu_ExpandAtMost.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextMenu_ExpandAtMostSpecified,
            this.ContextMenu_ExpandAtMost1,
            this.ContextMenu_ExpandAtMost2,
            this.ContextMenu_ExpandAtMost3,
            this.ContextMenu_ExpandAtMost4,
            this.ContextMenu_ExpandAtMost5});
            this.ContextMenu_ExpandAtMost.Name = "ContextMenu_ExpandAtMost";
            this.ContextMenu_ExpandAtMost.Size = new System.Drawing.Size(190, 22);
            this.ContextMenu_ExpandAtMost.Text = "Expand at Most";
            // 
            // ContextMenu_ExpandAtMostSpecified
            // 
            this.ContextMenu_ExpandAtMostSpecified.Name = "ContextMenu_ExpandAtMostSpecified";
            this.ContextMenu_ExpandAtMostSpecified.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtMostSpecified.Text = "Specified Number of Levels";
            this.ContextMenu_ExpandAtMostSpecified.Click += new System.EventHandler(this.ContextMenu_ExpandAtMostSpecified_Click);
            // 
            // ContextMenu_ExpandAtMost1
            // 
            this.ContextMenu_ExpandAtMost1.Name = "ContextMenu_ExpandAtMost1";
            this.ContextMenu_ExpandAtMost1.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtMost1.Text = "1 Level";
            this.ContextMenu_ExpandAtMost1.Click += new System.EventHandler(this.ContextMenu_ExpandAtMost1_Click);
            // 
            // ContextMenu_ExpandAtMost2
            // 
            this.ContextMenu_ExpandAtMost2.Name = "ContextMenu_ExpandAtMost2";
            this.ContextMenu_ExpandAtMost2.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtMost2.Text = "2 Levels";
            this.ContextMenu_ExpandAtMost2.Click += new System.EventHandler(this.ContextMenu_ExpandAtMost2_Click);
            // 
            // ContextMenu_ExpandAtMost3
            // 
            this.ContextMenu_ExpandAtMost3.Name = "ContextMenu_ExpandAtMost3";
            this.ContextMenu_ExpandAtMost3.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtMost3.Text = "3 Levels";
            this.ContextMenu_ExpandAtMost3.Click += new System.EventHandler(this.ContextMenu_ExpandAtMost3_Click);
            // 
            // ContextMenu_ExpandAtMost4
            // 
            this.ContextMenu_ExpandAtMost4.Name = "ContextMenu_ExpandAtMost4";
            this.ContextMenu_ExpandAtMost4.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtMost4.Text = "4 Levels";
            this.ContextMenu_ExpandAtMost4.Click += new System.EventHandler(this.ContextMenu_ExpandAtMost4_Click);
            // 
            // ContextMenu_ExpandAtMost5
            // 
            this.ContextMenu_ExpandAtMost5.Name = "ContextMenu_ExpandAtMost5";
            this.ContextMenu_ExpandAtMost5.Size = new System.Drawing.Size(218, 22);
            this.ContextMenu_ExpandAtMost5.Text = "5 Levels";
            this.ContextMenu_ExpandAtMost5.Click += new System.EventHandler(this.ContextMenu_ExpandAtMost5_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // Collapse
            // 
            this.Collapse.Name = "Collapse";
            this.Collapse.Size = new System.Drawing.Size(190, 22);
            this.Collapse.Text = "Collapse";
            this.Collapse.Click += new System.EventHandler(this.ContextMenu_Collapse_Click);
            // 
            // NodeImages
            // 
            this.NodeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("NodeImages.ImageStream")));
            this.NodeImages.TransparentColor = System.Drawing.Color.Transparent;
            this.NodeImages.Images.SetKeyName(0, "xml_none");
            this.NodeImages.Images.SetKeyName(1, "xml_root");
            this.NodeImages.Images.SetKeyName(2, "xml_element");
            this.NodeImages.Images.SetKeyName(3, "xml_text");
            this.NodeImages.Images.SetKeyName(4, "xml_attribute_name");
            this.NodeImages.Images.SetKeyName(5, "xml_attribute_value");
            this.NodeImages.Images.SetKeyName(6, "xml_comment");
            // 
            // listBoxXML
            // 
            this.listBoxXML.BackColor = System.Drawing.Color.WhiteSmoke;
            this.listBoxXML.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxXML.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxXML.ForeColor = System.Drawing.Color.Maroon;
            this.listBoxXML.HorizontalScrollbar = true;
            this.listBoxXML.ItemHeight = 16;
            this.listBoxXML.Location = new System.Drawing.Point(0, 0);
            this.listBoxXML.Name = "listBoxXML";
            this.listBoxXML.Size = new System.Drawing.Size(384, 209);
            this.listBoxXML.TabIndex = 12;
            this.toolTip1.SetToolTip(this.listBoxXML, "This region shows a textual representation of the XML document\r\nafter the documen" +
        "t is loaded.");
            // 
            // DispPanel
            // 
            this.DispPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DispPanel.Controls.Add(this.splitContainer1);
            this.DispPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispPanel.Location = new System.Drawing.Point(4, 94);
            this.DispPanel.Name = "DispPanel";
            this.DispPanel.Padding = new System.Windows.Forms.Padding(5);
            this.DispPanel.Size = new System.Drawing.Size(748, 456);
            this.DispPanel.TabIndex = 13;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(5, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewXML);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(736, 444);
            this.splitContainer1.SplitterDistance = 335;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 0;
            // 
            // richTextBoxXML
            // 
            this.richTextBoxXML.BackColor = System.Drawing.Color.WhiteSmoke;
            this.richTextBoxXML.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxXML.HideSelection = false;
            this.richTextBoxXML.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxXML.Name = "richTextBoxXML";
            this.richTextBoxXML.Size = new System.Drawing.Size(384, 225);
            this.richTextBoxXML.TabIndex = 13;
            this.richTextBoxXML.Text = "<< Drag & drop XML files above. >>";
            this.richTextBoxXML.WordWrap = false;
            this.richTextBoxXML.TextChanged += new System.EventHandler(this.richTextBoxXML_TextChanged);
            // 
            // TopPnl
            // 
            this.TopPnl.AutoSize = true;
            this.TopPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TopPnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TopPnl.Controls.Add(this.ControlPnl);
            this.TopPnl.Controls.Add(this.MenuStrip);
            this.TopPnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPnl.Location = new System.Drawing.Point(4, 4);
            this.TopPnl.Name = "TopPnl";
            this.TopPnl.Padding = new System.Windows.Forms.Padding(5);
            this.TopPnl.Size = new System.Drawing.Size(748, 90);
            this.TopPnl.TabIndex = 14;
            // 
            // ControlPnl
            // 
            this.ControlPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ControlPnl.Controls.Add(this.chkShowList);
            this.ControlPnl.Controls.Add(this.chkShowText);
            this.ControlPnl.Controls.Add(this.CollAllBtn);
            this.ControlPnl.Controls.Add(this.TopPnlHideBtn);
            this.ControlPnl.Controls.Add(this.LevelLbl);
            this.ControlPnl.Controls.Add(this.ExpAllBtn);
            this.ControlPnl.Controls.Add(this.LevelNumUpDown);
            this.ControlPnl.Controls.Add(this.ExpBtn);
            this.ControlPnl.Controls.Add(this.ExpMaxBtn);
            this.ControlPnl.Controls.Add(this.ExpMinBtn);
            this.ControlPnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.ControlPnl.Location = new System.Drawing.Point(5, 29);
            this.ControlPnl.Name = "ControlPnl";
            this.ControlPnl.Size = new System.Drawing.Size(736, 54);
            this.ControlPnl.TabIndex = 4;
            // 
            // CollAllBtn
            // 
            this.CollAllBtn.Location = new System.Drawing.Point(5, 29);
            this.CollAllBtn.Name = "CollAllBtn";
            this.CollAllBtn.Size = new System.Drawing.Size(70, 23);
            this.CollAllBtn.TabIndex = 0;
            this.CollAllBtn.Text = "Collapse all";
            this.toolTip1.SetToolTip(this.CollAllBtn, "Hides all nodes in the tree view such that \r\nonly the top-most node is visible.");
            this.CollAllBtn.UseVisualStyleBackColor = true;
            this.CollAllBtn.Click += new System.EventHandler(this.CollAllBtn_Click);
            // 
            // TopPnlHideBtn
            // 
            this.TopPnlHideBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TopPnlHideBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TopPnlHideBtn.ForeColor = System.Drawing.Color.Crimson;
            this.TopPnlHideBtn.Location = new System.Drawing.Point(709, 4);
            this.TopPnlHideBtn.Name = "TopPnlHideBtn";
            this.TopPnlHideBtn.Size = new System.Drawing.Size(23, 23);
            this.TopPnlHideBtn.TabIndex = 0;
            this.TopPnlHideBtn.Text = "x";
            this.TopPnlHideBtn.UseVisualStyleBackColor = true;
            this.TopPnlHideBtn.Click += new System.EventHandler(this.TopPnlHideBtn_Click);
            // 
            // LevelLbl
            // 
            this.LevelLbl.AutoSize = true;
            this.LevelLbl.Location = new System.Drawing.Point(258, 14);
            this.LevelLbl.Name = "LevelLbl";
            this.LevelLbl.Size = new System.Drawing.Size(41, 13);
            this.LevelLbl.TabIndex = 2;
            this.LevelLbl.Text = "Levels:";
            // 
            // ExpAllBtn
            // 
            this.ExpAllBtn.Location = new System.Drawing.Point(6, 4);
            this.ExpAllBtn.Name = "ExpAllBtn";
            this.ExpAllBtn.Size = new System.Drawing.Size(70, 23);
            this.ExpAllBtn.TabIndex = 0;
            this.ExpAllBtn.Text = "Expand All";
            this.toolTip1.SetToolTip(this.ExpAllBtn, "Expands all nodes in the tree view such that all nodes can be visualised, \r\n even" +
        "tually by scrolling to the node location.");
            this.ExpAllBtn.UseVisualStyleBackColor = true;
            this.ExpAllBtn.Click += new System.EventHandler(this.ExpAllBtn_Click);
            // 
            // LevelNumUpDown
            // 
            this.LevelNumUpDown.Location = new System.Drawing.Point(261, 32);
            this.LevelNumUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.LevelNumUpDown.Name = "LevelNumUpDown";
            this.LevelNumUpDown.Size = new System.Drawing.Size(38, 20);
            this.LevelNumUpDown.TabIndex = 1;
            this.toolTip1.SetToolTip(this.LevelNumUpDown, "Determines the number of levels referred to by three buttons\r\non the left-hand si" +
        "de of this control.");
            this.LevelNumUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // ExpBtn
            // 
            this.ExpBtn.Location = new System.Drawing.Point(189, 4);
            this.ExpBtn.Name = "ExpBtn";
            this.ExpBtn.Size = new System.Drawing.Size(54, 48);
            this.ExpBtn.TabIndex = 0;
            this.ExpBtn.Text = "Expand exactly";
            this.toolTip1.SetToolTip(this.ExpBtn, resources.GetString("ExpBtn.ToolTip"));
            this.ExpBtn.UseVisualStyleBackColor = true;
            this.ExpBtn.Click += new System.EventHandler(this.ExpBtn_Click);
            // 
            // ExpMaxBtn
            // 
            this.ExpMaxBtn.Location = new System.Drawing.Point(81, 29);
            this.ExpMaxBtn.Name = "ExpMaxBtn";
            this.ExpMaxBtn.Size = new System.Drawing.Size(102, 23);
            this.ExpMaxBtn.TabIndex = 0;
            this.ExpMaxBtn.Text = "Expand At Most";
            this.toolTip1.SetToolTip(this.ExpMaxBtn, resources.GetString("ExpMaxBtn.ToolTip"));
            this.ExpMaxBtn.UseVisualStyleBackColor = true;
            this.ExpMaxBtn.Click += new System.EventHandler(this.ExpMaxBtn_Click);
            // 
            // ExpMinBtn
            // 
            this.ExpMinBtn.Location = new System.Drawing.Point(81, 4);
            this.ExpMinBtn.Name = "ExpMinBtn";
            this.ExpMinBtn.Size = new System.Drawing.Size(102, 23);
            this.ExpMinBtn.TabIndex = 0;
            this.ExpMinBtn.Text = "Expand At Least";
            this.toolTip1.SetToolTip(this.ExpMinBtn, resources.GetString("ExpMinBtn.ToolTip"));
            this.ExpMinBtn.UseVisualStyleBackColor = true;
            this.ExpMinBtn.Click += new System.EventHandler(this.ExpMinBtn_Click);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.editToolStripMenuItem,
            this.MenuTools,
            this.MenuGelp});
            this.MenuStrip.Location = new System.Drawing.Point(5, 5);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(736, 24);
            this.MenuStrip.TabIndex = 3;
            this.MenuStrip.Text = "menuStrip1";
            this.MenuStrip.Enter += new System.EventHandler(this.MenuStrip_Enter);
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile_Open,
            this.MenuFile_Close,
            this.MenuFile_CloseDocument});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "&File";
            this.MenuFile.ToolTipText = "Closes the current document, but not the form.";
            // 
            // MenuFile_Open
            // 
            this.MenuFile_Open.Name = "MenuFile_Open";
            this.MenuFile_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuFile_Open.Size = new System.Drawing.Size(162, 22);
            this.MenuFile_Open.Text = "&Open";
            this.MenuFile_Open.ToolTipText = "Loads an XML document from a file.";
            this.MenuFile_Open.Click += new System.EventHandler(this.MenuFile_Open_Click);
            // 
            // MenuFile_Close
            // 
            this.MenuFile_Close.Name = "MenuFile_Close";
            this.MenuFile_Close.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.MenuFile_Close.Size = new System.Drawing.Size(162, 22);
            this.MenuFile_Close.Text = "&Close";
            this.MenuFile_Close.ToolTipText = "Closes the XML Viewer.";
            this.MenuFile_Close.Click += new System.EventHandler(this.MenuFile_Close_Click);
            // 
            // MenuFile_CloseDocument
            // 
            this.MenuFile_CloseDocument.Name = "MenuFile_CloseDocument";
            this.MenuFile_CloseDocument.Size = new System.Drawing.Size(162, 22);
            this.MenuFile_CloseDocument.Text = "Close Document";
            this.MenuFile_CloseDocument.Click += new System.EventHandler(this.MenuFile_CloseDocument_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editElementToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // editElementToolStripMenuItem
            // 
            this.editElementToolStripMenuItem.Name = "editElementToolStripMenuItem";
            this.editElementToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.editElementToolStripMenuItem.Text = "Edit Element";
            this.editElementToolStripMenuItem.ToolTipText = "Not implemented yet.\r\nEdits the selected element.";
            this.editElementToolStripMenuItem.Click += new System.EventHandler(this.MenuTools_Edit_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.ToolTipText = "Not implemented yet.\r\nCopies the XML tree structure of the selected node to the c" +
    "lipboard\r\nand deletes it from the loaded XML document.";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.MenuTools_Cut_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.ToolTipText = "Not implemented yet. \r\nCopies the XML tree structure of the selected node to the " +
    "clipboard.";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.MenuTools_Copy_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.ToolTipText = "Not implemented yet. \r\nPastes the copied XML node structure inside the selected e" +
    "lement.";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.MenuTools_Paste_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.deleteToolStripMenuItem.Text = "Delete ";
            // 
            // MenuTools
            // 
            this.MenuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuTools_Controls,
            this.MenuTools_TestXPath,
            this.toolStripSeparator5,
            this.MenuTools_ExpandAll,
            this.MenuTools_CollapseAll,
            this.toolStripSeparator6,
            this.MenuTools_Stop});
            this.MenuTools.Name = "MenuTools";
            this.MenuTools.Size = new System.Drawing.Size(48, 20);
            this.MenuTools.Text = "T&ools";
            // 
            // MenuTools_Controls
            // 
            this.MenuTools_Controls.Checked = true;
            this.MenuTools_Controls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuTools_Controls.Name = "MenuTools_Controls";
            this.MenuTools_Controls.Size = new System.Drawing.Size(210, 22);
            this.MenuTools_Controls.Text = "Top &Control Panel";
            this.MenuTools_Controls.ToolTipText = resources.GetString("MenuTools_Controls.ToolTipText");
            this.MenuTools_Controls.Click += new System.EventHandler(this.MenuTools_Controls_Click);
            // 
            // MenuTools_TestXPath
            // 
            this.MenuTools_TestXPath.Name = "MenuTools_TestXPath";
            this.MenuTools_TestXPath.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.MenuTools_TestXPath.Size = new System.Drawing.Size(210, 22);
            this.MenuTools_TestXPath.Text = "&Test XPath";
            this.MenuTools_TestXPath.ToolTipText = "Opens the XPath testing window.\r\nPath of the selected node is set as the initial " +
    "path.";
            this.MenuTools_TestXPath.Click += new System.EventHandler(this.MenuTools_TestXPath_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(207, 6);
            // 
            // MenuTools_ExpandAll
            // 
            this.MenuTools_ExpandAll.Name = "MenuTools_ExpandAll";
            this.MenuTools_ExpandAll.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.MenuTools_ExpandAll.Size = new System.Drawing.Size(210, 22);
            this.MenuTools_ExpandAll.Text = "&Expand All";
            this.MenuTools_ExpandAll.ToolTipText = "Expands all nodes in the tree view such that all nodes can be visualised, \r\n even" +
    "tually by scrolling to the node location.";
            this.MenuTools_ExpandAll.Click += new System.EventHandler(this.MenuTools_ExpandAll_Click);
            // 
            // MenuTools_CollapseAll
            // 
            this.MenuTools_CollapseAll.Name = "MenuTools_CollapseAll";
            this.MenuTools_CollapseAll.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.MenuTools_CollapseAll.Size = new System.Drawing.Size(210, 22);
            this.MenuTools_CollapseAll.Text = "&Collapse All";
            this.MenuTools_CollapseAll.ToolTipText = "Hides all nodes in the tree view such that \r\nonly the top-most node is visible.";
            this.MenuTools_CollapseAll.Click += new System.EventHandler(this.MenuTools_CollapseAll_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(207, 6);
            // 
            // MenuTools_Stop
            // 
            this.MenuTools_Stop.Name = "MenuTools_Stop";
            this.MenuTools_Stop.Size = new System.Drawing.Size(210, 22);
            this.MenuTools_Stop.Text = "Stop";
            this.MenuTools_Stop.Click += new System.EventHandler(this.MenuTools_Stop_Click);
            // 
            // MenuGelp
            // 
            this.MenuGelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuHelp_About});
            this.MenuGelp.Name = "MenuGelp";
            this.MenuGelp.Size = new System.Drawing.Size(44, 20);
            this.MenuGelp.Text = "&Help";
            // 
            // MenuHelp_About
            // 
            this.MenuHelp_About.Name = "MenuHelp_About";
            this.MenuHelp_About.Size = new System.Drawing.Size(274, 22);
            this.MenuHelp_About.Text = "Shows information about this control.";
            this.MenuHelp_About.Click += new System.EventHandler(this.MenuHelp_About_Click);
            // 
            // BottomPnl
            // 
            this.BottomPnl.Controls.Add(this.statusStrip1);
            this.BottomPnl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPnl.Location = new System.Drawing.Point(4, 550);
            this.BottomPnl.Name = "BottomPnl";
            this.BottomPnl.Padding = new System.Windows.Forms.Padding(5);
            this.BottomPnl.Size = new System.Drawing.Size(748, 34);
            this.BottomPnl.TabIndex = 14;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusType,
            this.StatusPath,
            this.StatusError,
            this.StatusStatus});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(5, 7);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(738, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "<<< Initialized >>>";
            // 
            // StatusType
            // 
            this.StatusType.Name = "StatusType";
            this.StatusType.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusType.Size = new System.Drawing.Size(79, 17);
            this.StatusType.Text = "<< Type >>";
            this.StatusType.ToolTipText = "This shows the type of the selected XML node in the tree view.";
            // 
            // StatusPath
            // 
            this.StatusPath.Name = "StatusPath";
            this.StatusPath.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusPath.Size = new System.Drawing.Size(77, 17);
            this.StatusPath.Text = "<< Path >>";
            this.StatusPath.ToolTipText = "This shows the path of the last selected XML node in the tree view.";
            // 
            // StatusError
            // 
            this.StatusError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.StatusError.Name = "StatusError";
            this.StatusError.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusError.Size = new System.Drawing.Size(34, 17);
            this.StatusError.Text = " OK";
            this.StatusError.ToolTipText = "This shows eventual error and warning messages.";
            this.StatusError.Click += new System.EventHandler(this.StatusError_Click);
            // 
            // StatusStatus
            // 
            this.StatusStatus.ForeColor = System.Drawing.Color.Blue;
            this.StatusStatus.Name = "StatusStatus";
            this.StatusStatus.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.StatusStatus.Size = new System.Drawing.Size(68, 17);
            this.StatusStatus.Text = "Initialized.";
            this.StatusStatus.Click += new System.EventHandler(this.StatusStatus_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 800;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.richTextBoxXML);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listBoxXML);
            this.splitContainer2.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel2_Paint);
            this.splitContainer2.Size = new System.Drawing.Size(384, 438);
            this.splitContainer2.SplitterDistance = 225;
            this.splitContainer2.TabIndex = 14;
            // 
            // chkShowText
            // 
            this.chkShowText.AutoSize = true;
            this.chkShowText.Checked = true;
            this.chkShowText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowText.Location = new System.Drawing.Point(323, 14);
            this.chkShowText.Name = "chkShowText";
            this.chkShowText.Size = new System.Drawing.Size(73, 17);
            this.chkShowText.TabIndex = 3;
            this.chkShowText.Text = "Show text";
            this.chkShowText.UseVisualStyleBackColor = true;
            this.chkShowText.CheckedChanged += new System.EventHandler(this.chkShowText_CheckedChanged);
            // 
            // chkShowList
            // 
            this.chkShowList.AutoSize = true;
            this.chkShowList.Checked = true;
            this.chkShowList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowList.Location = new System.Drawing.Point(323, 35);
            this.chkShowList.Name = "chkShowList";
            this.chkShowList.Size = new System.Drawing.Size(68, 17);
            this.chkShowList.TabIndex = 4;
            this.chkShowList.Text = "Show list";
            this.chkShowList.UseVisualStyleBackColor = true;
            this.chkShowList.CheckedChanged += new System.EventHandler(this.chkShowList_CheckedChanged);
            // 
            // XmlTreeViewControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DispPanel);
            this.Controls.Add(this.BottomPnl);
            this.Controls.Add(this.TopPnl);
            this.Name = "XmlTreeViewControl";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Size = new System.Drawing.Size(756, 588);
            this.Load += new System.EventHandler(this.XMLTreeView_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.XmlTreeViewControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.XmlTreeViewControl_DragEnter);
            this.NodeContextMenu.ResumeLayout(false);
            this.DispPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.TopPnl.ResumeLayout(false);
            this.TopPnl.PerformLayout();
            this.ControlPnl.ResumeLayout(false);
            this.ControlPnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LevelNumUpDown)).EndInit();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.BottomPnl.ResumeLayout(false);
            this.BottomPnl.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion




        //private System.Windows.Forms.Button ButtonShowTemporary;





        #region CONTROLS_DEFINITIONS



        Bitmap img_fileopen, img_exit, img_collapse, img_expand, img_about;
        private System.Windows.Forms.TreeView treeViewXML;
        private System.Windows.Forms.ListBox listBoxXML;
        private Panel DispPanel;
        private Panel TopPnl;
        private Panel BottomPnl;
        private Button ExpAllBtn;
        private Button CollAllBtn;
        private Label LevelLbl;
        private NumericUpDown LevelNumUpDown;
        private Button ExpMinBtn;
        private Button ExpBtn;
        private Button ExpMaxBtn;
        private Button TopPnlHideBtn;
        private MenuStrip MenuStrip;
        private ToolStripMenuItem ContextMenu_CopyFullPath;
        private ToolStripMenuItem ContextMenu_CopyFullPathExactly;
        private ToolStripMenuItem ContextMenu_CopyNodeText;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem ContextMenu_EditMenu;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem ContextMenu_Expand;
        private ToolStripMenuItem ContextMenu_ExpandSpecified;
        private ToolStripMenuItem ContextMenu_Expand1;
        private ToolStripMenuItem ContextMenu_Expand2;
        private ToolStripMenuItem ContextMenu_Expand3;
        private ToolStripMenuItem ContextMenu_Expand4;
        private ToolStripMenuItem ContextMenu_Expand5;
        private ToolStripMenuItem ContextMenu_ExpandAtLeast;
        private ToolStripMenuItem ContextMenu_ExpandAtLeastSpecified;
        private ToolStripMenuItem ContextMenu_ExpandAtLeast1;
        private ToolStripMenuItem ContextMenu_ExpandAtLeast2;
        private ToolStripMenuItem ContextMenu_ExpandAtLeast3;
        private ToolStripMenuItem ContextMenu_ExpandAtLeast4;
        private ToolStripMenuItem ContextMenu_ExpandAtLeast5;
        private ToolStripMenuItem ContextMenu_ExpandAtMost;
        private ToolStripMenuItem ContextMenu_ExpandAtMostSpecified;
        private ToolStripMenuItem ContextMenu_ExpandAtMost1;
        private ToolStripMenuItem ContextMenu_ExpandAtMost2;
        private ToolStripMenuItem ContextMenu_ExpandAtMost3;
        private ToolStripMenuItem ContextMenu_ExpandAtMost4;
        private ToolStripMenuItem ContextMenu_ExpandAtMost5;
        private ToolStripMenuItem Collapse;
        private ContextMenuStrip NodeContextMenu;
        private ToolStripMenuItem MenuFile;
        private ToolStripMenuItem MenuTools;
        private ToolStripMenuItem MenuGelp;
        private ToolStripMenuItem MenuFile_Open;
        private ToolStripMenuItem MenuFile_Close;
        private ToolStripMenuItem MenuTools_ExpandAll;
        private ToolStripMenuItem MenuTools_CollapseAll;
        private ToolStripMenuItem MenuTools_Stop;
        private ToolStripMenuItem MenuHelp_About;
        private ToolStripMenuItem MenuTools_Controls;
        private Panel ControlPnl;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel StatusPath;
        private ToolStripStatusLabel StatusType;
        private ToolStripStatusLabel StatusError;
        private SplitContainer splitContainer1;
        delegate void MyDelegate();






        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        //private IContainer components;


        //Error Counters (for counting errors for which a message box is launched):
        int NumErr = 0, MaxErr = 3;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem editElementToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ImageList NodeImages;
        private ToolStripMenuItem MenuTools_TestXPath;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        private ToolTip toolTip1;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem MenuFile_CloseDocument;
        private ToolStripMenuItem ContextMenu_Edit;
        private ToolStripMenuItem ContextMenu_Cut;
        private ToolStripMenuItem ContextMenu_Copy;
        private ToolStripMenuItem ContextMenu_Paste;
        private ToolStripMenuItem ContextMenu_Delete;
        private RichTextBox richTextBoxXML;
        private ToolStripStatusLabel StatusStatus;
        private ToolStripMenuItem ContextMenu_ExpandAll;












        #endregion    // CONTROLS_DEFINITIONS

        private SplitContainer splitContainer2;
        private CheckBox chkShowList;
        private CheckBox chkShowText;
    }
}
