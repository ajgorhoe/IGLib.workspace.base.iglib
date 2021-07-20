// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading ;
using System.Text; 
using System.IO ;
using System.Xml;
using IG.Forms;


// $$$$Excluded

//
// Read an XML Document and display the file as a Tree.
//
// Shripad Kulkarni 
// Date : May 15, 2002
// 

namespace IG.Forms11
{

    /// <summary>
    /// Summary description for XMLTreeView.
    /// </summary>
    /// 
    public class XMLTreeView : System.Windows.Forms.Form
    {

        #region DEFINITIONS

        protected class NodeData
        // Class used for storing node data in TreeView tags.
        {
            public int LineNumber = 0;
            public XmlNodeType NodeType = XmlNodeType.None;
            public bool IsEmptyElement = true;
            public int Depth = -1;
            public XmlNode Node=null;
            public string
                NodeName = null,
                NodeValue = null;
        }

        public enum XMLSourceType: byte 
        {
            TextFile,
            String,
            XMLNode
        };

        
        Cursor CursorDefault = Cursors.Default;
        Color
            ActiveBackColor = Color.White,
            InActiveBackColor = Color.WhiteSmoke;

        
        public bool
            Editable = true,
            ShowText=true;

        protected XmlDocument Doc = null;
        protected XMLTestPath PathTester = null;
        


        // Internal variables defining the behavior:
        public XMLSourceType SourceType = XMLSourceType.TextFile;

        internal string
            AttributeContainerLabel = "> Attr.",
            TitleBase = "XMLViewer";

        protected System.Drawing.Color
            RootBackColor = Color.FromArgb(255, 180, 180),
            RootForeColor = Color.Black,
            ElementBackColor = Color.White,
            ElementForeColor = Color.Red,
            TextBackColor = Color.FromArgb(255, 255, 180),
            TextForeColor = Color.Black,
            AttrNameBackColor = Color.White,
            AttrNameForeColor = Color.Blue,
            AttrValBackColor = Color.White,
            AttrValForeColor = Color.Green,
            CommentBackColor = Color.White,
            CommentForeColor = Color.Gray,
            AttrContBackColor = Color.White,
            AttrContForeColor = Color.LightGray;

        protected string
            NoneImageKey = "xml_none",
            RootImageKey = "xml_root",
            ElementImageKey = "xml_element",
            TextImageKey = "xml_text",
            AttrNameImageKey = "xml_attribute_name",
            AttrValImageKey = "xml_attribute_value",
            CommentImageKey = "xml_comment";


        private bool haschanged = false;
        public bool HasChanged
        {

            // Document.pa

            get { return haschanged; }
            internal set { haschanged = value; }
        }

        private bool docloaded = false;
        // Indicaion that a document is loaded and ready for user manipulation.
        public bool DocumentLoaded { 
            get { return docloaded; }
            internal set 
            { 
                docloaded = value;
                if (docloaded)
                {
                    toolTip1.SetToolTip(treeViewXML, "");
                    toolTip1.SetToolTip(listBoxXML, "");
                    try
                    {
                        treeViewXML.Nodes[0].EnsureVisible();
                        listBoxXML.TopIndex = 0;
                    }
                    catch { }
                }
                else
                {
                    toolTip1.SetToolTip(treeViewXML, "This panel shows the tree view of the XML document \r\nafter the document is loaded.");
                    toolTip1.SetToolTip(listBoxXML, "This panel shows a textual representation of the XML document\r\nafter the document is loaded.");
                    toolTip1.SetToolTip(richTextBoxXML, "This panel shows a textual representation of the XML document\r\nafter the document is loaded.");
                    treeViewXML.Nodes.Clear();
                    listBoxXML.Items.Clear();
                    richTextBoxXML.Clear();
                }
            }
        }

        // Properties defining whether the subnodes are expanded or not, and whether 
        // attribute values are shown as subnodes or together with attribute names.
        private bool expattr = true,
            attsubnode = true;

        public bool ExpAttr
        {
            set { expattr = value; }
            get { return expattr; }
        }
        public bool AttrSubnode
        {
            set { attsubnode = value; }
            get { return attsubnode; }
        }

        internal bool 
            DrawDocumentNode = true;  // If true then the document node is an outer-most node drawn.
                                      // In this way,comments before or after the root node can be included.
        internal string DocumentLabel = "XML Document";

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

        #endregion DEFINITIONS



        enum VIEW { TREE_VIEW = 0 } ;
        string XMLInputFile = null;
        string FileSize = "";
        string WorkingDir = Directory.GetCurrentDirectory();
        //bool FileLoaded = false;
        int CurrentView = (int)VIEW.TREE_VIEW;
        Thread t = null;
        TreeNode RootNode = null;
        Point ClickedPoint = new Point(0, 0);
        ArrayList TreeNodeArray = new ArrayList();



        #region ERROR_REPORTING

        private void ReportError0(string str)
        // Launches an error report, string is the error message.
        {
            FadingMessage fm = new FadingMessage();
            fm.MsgTitle = "Error in XMLTreeView";
            fm.MsgText = str;
            fm.BackGroundColor = Color.Orange;
            // $$$$Excluded: fm.ShowThread(str, 4000);
        }

        private void ReportError(string str)
        // Launches an error report, string is the error message.
        {
            StatusError.Text = " Error: " + str;
            ReportError0(str);
        }

        private void ReportError(Exception ex)
        {
            ReportError(ex.Message);
        }
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private IContainer components;

        #endregion  // ERROR_REPORTING


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

        #endregion    // CONTROLS_DEFINITIONS


        public XMLTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XMLTreeView));
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
            this.NodeContextMenu.SuspendLayout();
            this.DispPanel.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.TopPnl.SuspendLayout();
            this.ControlPnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LevelNumUpDown)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.BottomPnl.SuspendLayout();
            this.statusStrip1.SuspendLayout();
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
            this.treeViewXML.Size = new System.Drawing.Size(381, 564);
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
            this.listBoxXML.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listBoxXML.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxXML.ForeColor = System.Drawing.Color.Maroon;
            this.listBoxXML.HorizontalScrollbar = true;
            this.listBoxXML.ItemHeight = 16;
            this.listBoxXML.Location = new System.Drawing.Point(0, 320);
            this.listBoxXML.Name = "listBoxXML";
            this.listBoxXML.Size = new System.Drawing.Size(440, 244);
            this.listBoxXML.TabIndex = 12;
            this.toolTip1.SetToolTip(this.listBoxXML, "This region shows a textual representation of the XML document\r\nafter the documen" +
                    "t is loaded.");
            // 
            // DispPanel
            // 
            this.DispPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DispPanel.Controls.Add(this.splitContainer1);
            this.DispPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispPanel.Location = new System.Drawing.Point(10, 100);
            this.DispPanel.Name = "DispPanel";
            this.DispPanel.Padding = new System.Windows.Forms.Padding(5);
            this.DispPanel.Size = new System.Drawing.Size(843, 576);
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
            this.splitContainer1.Panel2.Controls.Add(this.richTextBoxXML);
            this.splitContainer1.Panel2.Controls.Add(this.listBoxXML);
            this.splitContainer1.Size = new System.Drawing.Size(831, 564);
            this.splitContainer1.SplitterDistance = 381;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 0;
            // 
            // richTextBoxXML
            // 
            this.richTextBoxXML.BackColor = System.Drawing.Color.WhiteSmoke;
            this.richTextBoxXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxXML.HideSelection = false;
            this.richTextBoxXML.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxXML.Name = "richTextBoxXML";
            this.richTextBoxXML.Size = new System.Drawing.Size(440, 320);
            this.richTextBoxXML.TabIndex = 13;
            this.richTextBoxXML.Text = "";
            this.richTextBoxXML.WordWrap = false;
            // 
            // TopPnl
            // 
            this.TopPnl.AutoSize = true;
            this.TopPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TopPnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TopPnl.Controls.Add(this.ControlPnl);
            this.TopPnl.Controls.Add(this.MenuStrip);
            this.TopPnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPnl.Location = new System.Drawing.Point(10, 10);
            this.TopPnl.Name = "TopPnl";
            this.TopPnl.Padding = new System.Windows.Forms.Padding(5);
            this.TopPnl.Size = new System.Drawing.Size(843, 90);
            this.TopPnl.TabIndex = 14;
            // 
            // ControlPnl
            // 
            this.ControlPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
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
            this.ControlPnl.Size = new System.Drawing.Size(831, 54);
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
            this.TopPnlHideBtn.Location = new System.Drawing.Point(804, 4);
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
            this.MenuStrip.Size = new System.Drawing.Size(831, 24);
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
            this.BottomPnl.Location = new System.Drawing.Point(10, 676);
            this.BottomPnl.Name = "BottomPnl";
            this.BottomPnl.Padding = new System.Windows.Forms.Padding(5);
            this.BottomPnl.Size = new System.Drawing.Size(843, 34);
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
            this.statusStrip1.Size = new System.Drawing.Size(833, 22);
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
            // XMLTreeView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(863, 720);
            this.Controls.Add(this.DispPanel);
            this.Controls.Add(this.BottomPnl);
            this.Controls.Add(this.TopPnl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "XMLTreeView";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowInTaskbar = false;
            this.Text = "XML Viewer";
            this.Load += new System.EventHandler(this.XMLTreeView_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XMLTreeView_Closing);
            this.NodeContextMenu.ResumeLayout(false);
            this.DispPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion  // Windows Form Designer generated code



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        void Main0()
        {
            Application.Run(new XMLTreeView());
        }

        //For getting resources, either use the 
        //  System.ComponentModel.ComponentResourceManager class, 
        //      http://msdn.microsoft.com/en-us/library/system.componentmodel.componentresourcemanager.aspx
        // or the System.Resources.ResourceManager class
        //      http://msdn.microsoft.com/en-us/library/system.resources.resourcemanager.aspx
        //http://msdn.microsoft.com/en-us/library/system.resources.aspx



        #region INITIALIZATION

        System.ComponentModel.ComponentResourceManager resources = null;

        private void XMLTreeView_Load(object sender, System.EventArgs e)
        {
            try
            {
                CursorDefault = this.Cursor;
                StatusError.Text = "";
                richTextBoxXML.WordWrap = false;  // If this is not set then navigation does not work properly.
                listBoxXML.Visible = false;
                if (!ShowText)
                {
                    listBoxXML.Visible = false;
                    richTextBoxXML.Visible = false;
                }
                // Add images to the imageList for treeView
                resources = new System.ComponentModel.ComponentResourceManager(typeof(XMLTreeView));

                // MessageBox.Show("Resources: " + resources.ToString());

                //Icon icon_root = (Icon)resources.GetObject(RootImageKey);
                //Icon icon_element = (Icon)resources.GetObject(ElementImageKey);
                //Icon icon_text = (Icon)resources.GetObject(TextImageKey);
                //Icon icon_attribute_name = (Icon)resources.GetObject(AttrNameImageKey);
                //Icon icon_attribute_value = (Icon)resources.GetObject(AttrValImageKey);
                //Icon icon_comment = (Icon)resources.GetObject(CommentImageKey);

                //NodeImages.Images.Add(RootImageKey, icon_text);
                //NodeImages.Images.Add(ElementImageKey, icon_element);
                //NodeImages.Images.Add(TextImageKey, icon_text);
                //NodeImages.Images.Add(AttrNameImageKey, icon_attribute_name);
                //NodeImages.Images.Add(AttrValImageKey, icon_attribute_value);
                //NodeImages.Images.Add(CommentImageKey, icon_comment);
            }
            catch (Exception ex) { //
                MessageBox.Show(ex.Message); 
            }



            // Add images to the imageList for the MenuBar
            try
            {
                img_fileopen = new Bitmap("FileOpen.bmp");
                img_exit = new Bitmap("exit.bmp");
                img_expand = new Bitmap("ExpandTree.bmp");
                img_collapse = new Bitmap("CollapseTree.bmp");
                img_about = new Bitmap("about.bmp");
            }
            catch { }

            this.Text = TitleBase;

        }

        #endregion   // INITIALIZATION


        #region DOCUMENT_LOADING

        /*
        Notification System:
        Used when several operations are performed, possibly within different threads, and
        we must do something before any of them starts and after all of them are completed.
          Procedure:
          Before we start any operation, we call ResetNotifications(). Before we start any
        operations in parallel threads, we call LockNotifications(), and after we launch all
        operations, we call UnlockNotifications(). Locks can be nested, but must be strictly
        called in pairs (take care that they will be executed for sure).
         Before starting each operation, call NotifyStarted(), and after operation is
       completed, call NotifyFinished(). For operations that are started in 
         The FinalizeNotifications() should contain the code that must be executed after all
       operations are completed. Either the last UnlockNotifications() or the last 
       NotifyFinished() will call this function. FinalizeNotifications() sets the lock
       that will not have a corresponding unlock, such that the system is locked until the
       ResetNotifications() is called that prepares the system for another group of operations
       for which we neet notification when all are completed.
        */


        int numLoadsToPerform = 0, countLock=0;
        
        private void FinalizeNotifications()
        // Performas all the actions that should be performed when operations watched
        // by the notification system are completed.
        {
            try
            {
                // A lock without corresponding unlock is called. This is to lock the system before the
                // next ResetNotifications() is called.
                LockNotifications();
                DocumentLoaded = true;
                this.Cursor = CursorDefault;
                if (treeViewXML != null)
                    treeViewXML.BackColor = ActiveBackColor;
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ResetNotifications()
        // resets the notification system and performs initialization actions that must be
        // performed before the watched operations are started.
        {
            countLock=0;
            numLoadsToPerform = 0;
            // Initialization
            DocumentLoaded=false;
            this.Cursor = Cursors.WaitCursor;
            try 
            { 
                treeViewXML.Nodes.Clear();
                if (richTextBoxXML != null)
                {
                    richTextBoxXML.Clear();
                    richTextBoxXML.BackColor = InActiveBackColor;
                    richTextBoxXML.Refresh();
                }
                if (listBoxXML!=null)
                    listBoxXML.Items.Clear();
            }  catch {}
        }

        void LockNotifications()
        // locks the notification system; when the system is locked, the 
        // NotifyFinished can not set DocumentLoaded to true. Instead,
        // the UnlockNotifications will do that.
        {
            ++countLock;
        }

        void UnlockNotifications()
        // Decreases the lock counter by 1. If the counter hits zero and all the considered
        // tasks have finished, the finalization is performed.
        {
            --countLock;
            if (countLock==0 && numLoadsToPerform == 0)
                FinalizeNotifications();
        }

        private void NotifyStarted()
        // Notifies that another operation has been started.
        {
            ++numLoadsToPerform;
        }

        private void NotifyFinished()
        // Notifies the class that one operation has finished. If lock counter is on 0 then
        // also the finalization is performed.
        {
            --numLoadsToPerform;
            if (countLock==0 && numLoadsToPerform == 0)
                FinalizeNotifications();
        }



        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        // Performs all loading operations necessary in order to load an XML document to this consform from a file.
        // When feasible, demanding operations are performed in separate threads.
        {
            bool NotificationsUsed = false;
            try
            {
                // Initialize Buttons
                EnableDisableControls();

                // Initialize All the Arrays
                treeViewXML.Nodes.Clear();
                listBoxXML.Items.Clear();
                TreeNodeArray.Clear();
                //FileLoaded = false;
                XMLInputFile = openFileDialog1.FileName;
                this.Text = TitleBase + " ..." + XMLInputFile;
                openFileDialog1.Dispose();

                // Get the filename and filesize
                FileInfo f = new FileInfo(XMLInputFile);
                FileSize = f.Length.ToString();


                // Prepare notification system that will call FinalizeNotifications()
                NotificationsUsed = true;
                ResetNotifications();
                LockNotifications();

                // Begin thread to load file contents in XML representation Doc:
                Thread docthread = new Thread(new ThreadStart(LoadFileToDoc));
                docthread.Start();

                // Begin thread to read input file and load it into the ListBox
                Thread textboxthread = new Thread(new ThreadStart(LoadFileToListBox));
                textboxthread.Start();

                // Begin thread to read input file and load it into the ListBox
                Thread listboxthread = new Thread(new ThreadStart(LoadFileToTextBox));
                listboxthread.Start();                
                
                // Begin thread to read input file and populate the Tree
                Thread treeviewthread = new Thread(new ThreadStart(ParseFileToTreeView));
                treeviewthread.Start();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
            finally
            {
                if (NotificationsUsed)
                    UnlockNotifications();
            }
        }



        private void LoadFileToListBox()
        // Load line by line the XML file specified by XMLInputFile to listBoxXML.
        {
            // Load the xml file into a listbox.
            try
            {

                if (this.InvokeRequired)
                {
                    // Delegate the method when called consform a thread not owning the consform.
                    VoidDelegate fref = new VoidDelegate(LoadFileToListBox);
                    this.Invoke(fref);
                }
                else
                {
                    try
                    {
                        NotifyStarted();     // Notify that this operation has started
                        StreamReader sr = new StreamReader(XMLInputFile, Encoding.ASCII);
                        sr.BaseStream.Seek(0, SeekOrigin.Begin);
                        while (sr.Peek() > -1)
                        {
                            // Thread.Sleep(5);
                            string str = sr.ReadLine();
                            listBoxXML.Items.Add(str);
                        }
                        sr.Close();
                        //FileLoaded = true;
                        listBoxXML.SetSelected(1, true);
                    }
                    catch (Exception ex)
                    {
                        ReportError(ex);
                    }
                    finally
                    {
                        NotifyFinished();        // Notify that this operation has finished:
                    }
                }
            }
            catch (Exception ee)
            {
                ReportError(ee);
            }
        }


        private void LoadFileToTextBox()
        // Load the XML file specified by XMLInputFile to richTextBoxXML.
        {
            // Load the xml file into a listbox.
            try
            {

                if (this.InvokeRequired)
                {
                    // Delegate the method when called consform a thread not owning the consform.
                    VoidDelegate fref = new VoidDelegate(LoadFileToTextBox);
                    this.Invoke(fref);
                }
                else
                {
                    try
                    {
                        NotifyStarted();     // Notify that this operation has started
                        richTextBoxXML.Clear();
                        using (TextReader tr = new StreamReader(XMLInputFile)  )
                        {
                            richTextBoxXML.AppendText(tr.ReadToEnd());
                        }
                    }
                    catch (Exception ex)
                    {
                        ReportError(ex);
                    }
                    finally
                    {
                        NotifyFinished();        // Notify that this operation has finished:
                    }
                }
            }
            catch (Exception ee)
            {
                ReportError(ee);
            }
        }



        private void MoveToLine(int ln)
        {
            // Select the input line from the file in the listbox
            try
            {
                if (richTextBoxXML.Visible)
                {
                    int linestart = richTextBoxXML.GetFirstCharIndexFromLine(ln-1);
                    int linelength = richTextBoxXML.Lines[ln-1].Length;
                    // richTextBoxXML.Select( linestart, linelength );
                    richTextBoxXML.SelectionStart = linestart;
                    richTextBoxXML.SelectionLength = linelength;
                    richTextBoxXML.ScrollToCaret();
                    //StatusError.Text = richTextBoxXML.Lines[ln - 1];
                }


                if (listBoxXML.Visible)
                    if (listBoxXML.Items.Count>=ln)
                    listBoxXML.SetSelected(ln - 1, true);
            }
            catch (Exception ex) { ReportError(ex); }
        }


        private void LoadFileToDoc()
        // Loads an document from the file specified by XMLInputFile and stores it in Doc. It does
        // not take care of binding between the XML tree and the data; This must be done by a 
        // a separate function (either ParseDocumentToTreeView() or )
        {
            try
            {
                NotifyStarted();  // Mark the beginning of operation
                Doc = new XmlDocument();
                Doc.Load(XMLInputFile);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
            finally
            {
                NotifyFinished();  // Mark completion of operation
            }
        }

        private void UpdateDocumentInTreeView()
        // Updates document information of the XML document Doc in the treeViewXML. This control
        // must at the function call contain an XML tree corresponding to the TreeView.
        // WARNING: 
        // This operation may not be started before the tree view and the document are entirely loaded.
        {
            try
            {
                NotifyStarted();  // Mark the beginning of operation

                ReportError("Updating the tree view with node information from the XML document is not yet implemented.");
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
            finally
            {
                NotifyFinished();  // Mark completion of operation
            }
        }

        private void ParseDocumentToTreeView()
        // Parses the XML document stored in Doc and stores as a tree in treeViewXML.
        {
            try
            {
                NotifyStarted();  // Mark the beginning of operation

                ReportError("Parsing an XML document to the tree view is not implemented yet.");
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
            finally
            {
                NotifyFinished();  // Mark completion of operation
            }
        }


        private void ParseFileToTreeView()
        // Read the XML file (specified by the variable XMLInputFile) by the XMLTextReader class
        // and create the corresponding tree in the treeViewXML control.
        // The generated GUI tree _gridCoordinates have information about line numbers and XML node types, but
        // do not contain references to complete XML _gridCoordinates because this can not be achieved 
        {
            if (this.InvokeRequired)
            {
                // Delegate the method when called consform a thread not owning the consform.
                VoidDelegate fref = new VoidDelegate(ParseFileToTreeView);
                this.Invoke(fref);
            }
            else
            {
                try
                {

                    NotifyStarted();  // Mark the beginning of operation


                    XmlReader reader = null;
                    XmlTextReader textreader = null; ;
                    switch (SourceType)
                    {
                        case XMLSourceType.TextFile:
                            textreader = new XmlTextReader(XMLInputFile);
                            textreader.WhitespaceHandling = WhitespaceHandling.None;
                            reader = (XmlReader)textreader;
                            break;
                        default:
                            textreader = new XmlTextReader(XMLInputFile);
                            textreader.WhitespaceHandling = WhitespaceHandling.None;
                            reader = (XmlReader)textreader;
                            break;
                    }

                    //XmlReader reader;
                    //reader = XmlReader.Create(textreader);
                    //reader = XmlReader.Create(textreader);
                    //XmlReader reader = null;
                    //reader = XmlReader.Create(XMLInputFile);

                    string readerName = "";
                    bool RootNodeWorked = false;
                    int depth = 0;
                    TreeNode CurrentNode = null;
                    RootNode = null;
                    TreeNode AttrNode = null;
                    TreeNode newNode = null;
                    bool bIsEmpty = false;
                    bool continuereading = true;
                    if (DrawDocumentNode)
                    {
                        // Insert the document node that will contain all s _gridCoordinates.
                        // RootNodeWorked = true;
                        TreeNode DocumentNode = this.treeViewXML.Nodes.Add(DocumentLabel);
                        CurrentNode = DocumentNode;
                        //RootNode.SelectedImageKey = RootImageKey;
                        //RootNode.ImageKey = RootImageKey;
                        DocumentNode.BackColor = CommentBackColor;
                        DocumentNode.ForeColor = CommentForeColor;
                    }
                    while (continuereading)
                    {
                        switch (SourceType)
                        {
                            case XMLSourceType.TextFile:
                                continuereading = textreader.Read();
                                break;
                            default:
                                continuereading = textreader.Read();
                                break;
                        }
                        reader = textreader;
                        if (continuereading)
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    {
                                        readerName = reader.Name;
                                        bIsEmpty = reader.IsEmptyElement;
                                        if (!RootNodeWorked)
                                        {
                                            // The element read is the root element.
                                            RootNodeWorked = true;
                                            if (DrawDocumentNode)
                                                RootNode = CurrentNode.Nodes.Add(readerName);
                                            else
                                                RootNode = this.treeViewXML.Nodes.Add(readerName);
                                            //$$ AssociateTag(RootNode, reader.LineNumber);
                                            AssociateTag(RootNode, reader);
                                            RootNode.SelectedImageKey = RootImageKey;
                                            RootNode.ImageKey = RootImageKey;
                                            RootNode.BackColor = RootBackColor;
                                            RootNode.ForeColor = RootForeColor;
                                            continue;
                                        }
                                        depth = reader.Depth;
                                        if (reader.IsStartElement() && depth == 1)
                                        {
                                            // Root element:
                                            CurrentNode = RootNode.Nodes.Add(reader.Name);
                                            //$$ AssociateTag(CurrentNode, reader.LineNumber);
                                            AssociateTag(CurrentNode, reader);
                                            // CurrentNode.BackColor = ElementBackColor;
                                            // CurrentNode.ForeColor = ElementForeColor;
                                        }
                                        else
                                        {
                                            // Ordinary element (Element node is read):
                                            TreeNode parent = CurrentNode;
                                            CurrentNode = parent.Nodes.Add(reader.Name);
                                            //$$ AssociateTag(CurrentNode, reader.LineNumber);
                                            AssociateTag(CurrentNode, reader);
                                            // CurrentNode.BackColor = ElementBackColor;
                                            // CurrentNode.ForeColor = ElementForeColor;
                                        }

                                        CurrentNode.SelectedImageKey = ElementImageKey;
                                        CurrentNode.ImageKey = ElementImageKey;
                                        CurrentNode.BackColor = ElementBackColor;
                                        CurrentNode.ForeColor = ElementForeColor;

                                        // Containter node for holding the attributes:
                                        TreeNode AttrContNode = CurrentNode.Nodes.Add(AttributeContainerLabel);
                                        //AttrContNode.SelectedImageKey = "";
                                        //AttrContNode.ImageKey = "";
                                        AttrContNode.BackColor = AttrContBackColor;
                                        AttrContNode.ForeColor = AttrContForeColor;
                                        //$$ AssociateTag(AttrContNode, reader.LineNumber);
                                        AssociateTag(AttrContNode, reader);

                                        for (int i = 0; i < reader.AttributeCount; i++)
                                        {
                                            // Go through attributes
                                            reader.MoveToAttribute(i);
                                            string rValue = reader.Value.Replace("\r\n", " ");
                                            AttrNode = AttrContNode.Nodes.Add("@" + reader.Name);
                                            //	AttrNode = CurrentNode.Nodes.Add(reader.Name +"="+rValue);
                                            //$$ AssociateTag(AttrNode, reader.LineNumber);
                                            AssociateTag(AttrNode, reader);

                                            AttrNode.SelectedImageKey = AttrNameImageKey;
                                            AttrNode.ImageKey = AttrNameImageKey;
                                            AttrNode.BackColor = AttrNameBackColor;
                                            AttrNode.ForeColor = AttrNameForeColor;
                                            TreeNode tmp = AttrNode.Nodes.Add(rValue);
                                            tmp.SelectedImageKey = AttrValImageKey;
                                            tmp.ImageKey = AttrValImageKey;
                                            tmp.BackColor = AttrValBackColor;
                                            tmp.ForeColor = AttrValForeColor;
                                            //$$ AssociateTag(tmp, reader.LineNumber);
                                            AssociateTag(tmp, reader);

                                            //AttrNode.SelectedImageKey = 2;
                                            //AttrNode.ImageKey = 2;
                                        }

                                        if (bIsEmpty)
                                            CurrentNode = CurrentNode.Parent;
                                    }
                                    break;
                                //case XmlNodeType.Text:
                                //    {
                                //        string rValue = reader.Value.Replace("\r\d2", " ");
                                //        newNode = CurrentNode.Nodes.Add(rValue);
                                //        //$$ AssociateTag(newNode, reader.LineNumber);
                                //        AssociateTag(newNode, reader);
                                //        newNode.SelectedImageKey = TextImageKey;
                                //        newNode.ImageKey = TextImageKey;
                                //        newNode.BackColor = TextBackColor;
                                //        newNode.ForeColor = TextForeColor;
                                //    }
                                //    break;
                                case XmlNodeType.Comment:
                                    {
                                        TreeNode commentNode = null;
                                        try
                                        {
                                            if (CurrentNode != null)
                                            {
                                                string rValue;
                                                rValue = reader.Value.Replace("\r\n", " ");

                                                commentNode = CurrentNode.Nodes.Add(rValue);
                                                //$$ AssociateTag(commentNode, reader.LineNumber);
                                                AssociateTag(commentNode, reader);
                                                commentNode.SelectedImageKey = CommentImageKey;
                                                commentNode.ImageKey = CommentImageKey;
                                                commentNode.BackColor = CommentBackColor;
                                                commentNode.ForeColor = CommentForeColor;
                                            }
                                            else
                                            {
                                                //void method()
                                                //{
                                                //    MessageBox.Show("Test method.");
                                                //}
                                                ++NumErr;
                                                if (NumErr == 1)
                                                {
                                                    // Method()
                                                    string text = null;
                                                    text += "XML documents contains comments before the root element.\n\r";
                                                    text += "These comments will not be shown in the tree.\n\r\n\r";
                                                    text += "Further messages of this type will be omitted.";
                                                    new FadingMessage(text, 4000);
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            ++NumErr;
                                            if (NumErr <= MaxErr)
                                            {
                                                string text = null;
                                                text += "Error in processing a comment node.";
                                                if (NumErr == MaxErr)
                                                    text += "\n\nFurther error reports will be omitted (max. number reached).";

                                                MessageBox.Show(text);
                                            }
                                        }
                                    }
                                    break;
                                case XmlNodeType.Text:
                                    {
                                        string rValue = reader.Value.Replace("\r\n", " ");
                                        newNode = CurrentNode.Nodes.Add(rValue);
                                        //$$ AssociateTag(newNode, reader.LineNumber);
                                        AssociateTag(newNode, reader);
                                        newNode.SelectedImageKey = TextImageKey;
                                        newNode.ImageKey = TextImageKey;
                                        newNode.BackColor = TextBackColor;
                                        newNode.ForeColor = TextForeColor;
                                    }
                                    break;
                                case XmlNodeType.EndElement:
                                    CurrentNode = CurrentNode.Parent;
                                    break;
                            }
                        }
                    }
                    reader.Close();
                    RootNode.ExpandAll();

                }
                catch (Exception eee)
                {
                    Console.WriteLine(eee.Message);
                }
                finally
                {
                    NotifyFinished();  // Mark completion
                }


            }

        }    // ParseFileToTreeView 


        #endregion DOCUMENT_LOADING



        #region NODE_UTILITIES

        bool isAttributeContainter(TreeNode node)
        // Returns true if the node is an artificial node that groups element attributes.
        {
            bool ret = false;  // Indicates that the node is an artificial container for grouping attribute _gridCoordinates 
            NodeData data = node.Tag as NodeData;
            int length = 0, textlength = 0 ;
            string text=node.Text;
            if (AttributeContainerLabel!=null)
                length = AttributeContainerLabel.Length;
            if (text != null)
                textlength = text.Length;
            if (textlength>=length)
                if (text.Substring(0, length) == AttributeContainerLabel)
                {
                    if (data == null)
                        ret = true;
                    else if (data.NodeType == XmlNodeType.Element)
                        ret = true;
                }
            return ret;
        }
        
        string getTreeNodePath(TreeNode node)
        {
            string ret="/";
            XmlNodeType type = XmlNodeType.Element;
            try
            {
                if (node.Tag != null)
                {
                    NodeData data = (NodeData)node.Tag;
                    type = data.NodeType;
                    if (type == XmlNodeType.Comment)
                        return getTreeNodePath(node.Parent);
                    if (type == XmlNodeType.Text)
                        return getTreeNodePath(node.Parent);
                    if (type == XmlNodeType.Attribute && node.Text[0] != '@')
                        return getTreeNodePath(node.Parent);
                }
                ret="/" + node.FullPath.Replace('\\', '/').Replace("/" + AttributeContainerLabel, "" );
                if (DrawDocumentNode)
                    ret = ret.Replace("/" + DocumentLabel, "");
            }
            catch (Exception ex) { ReportError(ex); }
            return ret;
        }

        string getTreeNodeTypeName(TreeNode node)
        {
            string ret = null;
            try
            {
                NodeData data = node.Tag as NodeData;
                if (data != null)
                    ret = data.NodeType.ToString();
                else
                    ret = null;
            }
            catch(Exception ex)
            {
                ReportError(ex);
            }
            return ret;
        }

        string getTreeNodeName(TreeNode node)
        {
            string ret = null;
            try
            {
                ret = node.Text;
                NodeData data = node.Tag as NodeData;
                if (data != null)
                    ret = data.NodeName.ToString();
            }
            catch(Exception ex)
            {
                ReportError(ex);
            }
            return ret;
        }


        string getTreeNodeText(TreeNode node)
        {
            return node.Text;
        }

        int getTreeNodeLineNumber(TreeNode node)
        {
            NodeData data = node.Tag as NodeData;
            return data.LineNumber;
        }

        XmlNode FindXmlNode(TreeNode TNode)
        // Returns the XML node of the loaded document that corresponds to the specified node in the tree.
        // $A Igor sep08;
        {
            XmlNode ret = null;
            try
            {
                //string TType = null;
                //if (TNode!=null)
                //    if (TNode.Tag != null)
                //    {
                //        NodeData data = TNode.Tag as NodeData;
                //        if (data != null)
                //            TType = data.NodeType;
                //    }
                if (TNode==null)
                    return null;
                else if (TNode.Level == 0) 
                {
                    // Caes where the node does not have a parent:
                    if (TNode.Text.CompareTo(DocumentLabel) == 0 &&
                        getTreeNodeTypeName(TNode).CompareTo("Element") != 0)
                    {
                        ret = Doc;
                    } else
                    {
                        // The tree node represents the root node of the document:
                        if (Doc.ChildNodes != null)
                        {
                            ret=null;
                            int i = 0;
                            while (ret == null && i < Doc.ChildNodes.Count)
                            {
                                XmlNode nd = Doc.ChildNodes[i];
                                if (nd.NodeType == XmlNodeType.Element)
                                {
                                    if (nd.Name.CompareTo(TNode.Text) == 0)
                                        ret=nd;
                                    else
                                        throw new Exception("Names of the root node in the document and tree view do not match.\n");
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<TreeNode> TList = new List<TreeNode>();
                    TreeNode TCurrent = TNode;
                    XmlNode XCurrent = null;
                    // Put the node and all its parents into an array; find the appropriate type of node first:
                    while ( TCurrent!=null 
                         && getTreeNodeTypeName(TCurrent).CompareTo("Element") != 0
                         && getTreeNodeTypeName(TCurrent).CompareTo("Text") != 0 
                         && getTreeNodeTypeName(TCurrent).CompareTo("Comment") != 0)
                    {
                        TCurrent=TCurrent.Parent;
                    }

                    // Compose the list of node'result predecessors, starting with the node and ending with the root:
                    while (TCurrent != null)
                    {
                        if (TCurrent.Text.CompareTo(DocumentLabel) == 0 &&
                            getTreeNodeTypeName(TCurrent).CompareTo("Element") != 0)
                        {
                            TCurrent = null;
                        }
                        else
                        {
                            TList.Add(TCurrent);
                            TCurrent = TCurrent.Parent;
                        }
                    }
                    if (TList.Count==0)
                        throw new Exception("Can not determine the complete line of tree node ancestors.\n");
                    else
                    {
                        int indCur = TList.Count-1;
                        // Find the equivalent root _gridCoordinates in the tree view and XML document:
                        TCurrent = TList[indCur];
                        XCurrent = null;
                        if (Doc!=null)
                            if (Doc.HasChildNodes)
                            {
                                int childIndex = 0;
                                while (XCurrent == null && childIndex < Doc.ChildNodes.Count)
                                {
                                    XmlNode n=Doc.ChildNodes[childIndex];
                                    if (n.NodeType == XmlNodeType.Element)
                                    {
                                        if (n.Name.CompareTo(getTreeNodeName(TCurrent)) == 0)
                                            XCurrent=n;
                                        else
                                        {
                                            throw new Exception ("Name of the root element (\"" + n.Name +
                                                "\") is different from the corresponding name in the tree view (\"" +
                                                getTreeNodeName(TCurrent) + "\")." );
                                        }
                                    }
                                    ++childIndex;
                                }
                            }
                        
                        

                        while (indCur >= 0)
                        {
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }

            ReportError("This method is not yet implemented: FindXmlNode.");

            return ret;
        }

        // TreeNode T; XmlNode N; TreeNode T; XmlNode N;

        private void treeViewXML_Click(object sender, System.EventArgs e)
        {
        }


        private void ExpandNodes(TreeNode node, int numlevels, bool atleast, bool atmost)
        // Expands numlevels levels of node. numlevels==0 means all levels.
        // messagelevel must be 0 when called.
        // If atleast==true then all subnodes until messagelevel numlevels are expanded, and if some _gridCoordinates beyond
        // this messagelevel have laready been expanded then they are left in this state. 
        // If atmost==true then all _gridCoordinates beyond the specified messagelevel are collapsed, and expanded remain only
        // those that have already been expanded before the function call.
        // If both atleast and atmost are true then exactly the specified number of _gridCoordinates are expanded.
        // $A Igor aug08; 
        {
            ExpandNodes(node, numlevels, 0, atleast, atmost);
        }

        private void ExpandNodesExactly(TreeNode node, int numlevels)
        // Expands exactly numlevels levels of node.
        {
            ExpandNodes(node, numlevels, true /* atleast */, true /* atmost */);
        }

        private void ExpandNodesAtLeast(TreeNode node, int numlevels)
        // Expands at least numlevels levels of node.
        {
            ExpandNodes(node, numlevels, true /* atleast */, false /* atmost */);
        }

        private void ExpandNodesAtMost(TreeNode node, int numlevels)
        // Expands at most numlevels levels of node.
        {
            ExpandNodes(node, numlevels, false /* atleast */, true /* atmost */);
        }


        private void ExpandNodes(TreeNode node, int numlevels, int level, bool atleast, bool atmost)
        // Does the job for ExpandNodes(). Since this function is called recursively, the current messagelevel must
        // be an input paerameter (which is not necessary for the overloaded function).
        // $A Igor aug08; 
        {
            ++level;
            if (level < numlevels || numlevels==0)
            {
                if (atleast || atmost && node.IsExpanded)
                {
                    node.Expand();
                    TreeNodeCollection nodes = node.Nodes;
                    foreach (TreeNode childnode in nodes)
                    {
                        ExpandNodes(childnode, numlevels, level, atleast, atmost);
                    }
                }
            }
            else
            {
                if (atmost)
                {
                    node.Collapse();
                }
            }
        }



        #endregion  //  NODE_UTILITIES


        #region NODE_EDIT

        // ALL Edit actions that change the loaded XML document must be placed here.


        void EditNode(TreeNode node)
        {
            if (!DocumentLoaded)
                ReportError0("Can not edit a node: No document loaded.");
            else if (!Editable)
                ReportError0("Can not edit a node: the document is read-only.");
            else if (node==null)
                ReportError0("Can not edit a node: No node has been selected.");
            {
                this.HasChanged = true;
                ReportError0("This function is not yet implemeted.");
            }
        }

        void CutNode(TreeNode node)
        {
            if (!DocumentLoaded)
                ReportError0("Can not cut a node to the clipboard: No document loaded.");
            else if (!Editable)
                ReportError0("Can not cut a node to the clipboard: the document is read-only.");
            else if (node == null)
                ReportError0("Can not cut a node to the clipboard: No node has been selected.");
            {
                this.HasChanged = true;
                ReportError0("This function is not yet implemeted.");
            }
        }

        void CopyNode(TreeNode node)
        {
            if (!DocumentLoaded)
                ReportError0("Can not copy a node to the clipboard: No document loaded.");
            else if (!Editable)
                ReportError0("Can not copy a node to the clipboard: the document is read-only.");
            else if (node == null)
                ReportError0("Can not copy a node to the clipboard: No node has been selected.");
            {
                this.HasChanged = true;
                ReportError0("This function is not yet implemeted.");
            }
        }

        void PasteNode(TreeNode node)
        {
            if (!DocumentLoaded)
                ReportError0("Can not paste a tree at the node location: No document loaded.");
            else if (!Editable)
                ReportError0("Can not paste a tree at the node location: the document is read-only.");
            else if (node == null)
                ReportError0("Can not paste a tree at the node location: No node has been selected.");
            {
                this.HasChanged = true;
                ReportError0("This function is not yet implemeted.");
            }
        }

        void DeleteNode(TreeNode node)
        {
            if (!DocumentLoaded)
                ReportError0("Can not delete a node: No document loaded.");
            else if (!Editable)
                ReportError0("Can not delete a node: the document is read-only.");
            else if (node == null)
                ReportError0("Can not delete a node: No node has been selected.");
            {
                this.HasChanged = true;
                ReportError0("This function is not yet implemeted.");
            }
        }


        #endregion   // NODE_EDIT




        private void treeViewXML_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        // Selects the node when clicked.
        // This takes care that when any item in a context menu is clicked, the selected node is
        // already set to the node on which right mouse button is clicked (and not the node that was
        // selrcted before that).
        {
            TreeNode tn;
            tn = (TreeNode)e.Node;
            tn.TreeView.SelectedNode = tn;
        }


        private void treeViewXML_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            //if (!FileLoaded) return;
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
                TreeNode tn;
                tn = (TreeNode)e.Node;
                NodeData data = tn.Tag as NodeData;
                if (data != null)
                {

                    // string path = "/" + treeXml.SelectedNode.FullPath;
                    // string path = "/" + tn.FullPath.Replace('\\', '/').Replace("/" + AttributeContainerLabel, "" );
                    string path = getTreeNodePath(tn);

                    // show the selected node type in the status bar
                    StatusType.Text = " " + getTreeNodeTypeName(tn) + ":  ";

                    // show the path in the status bar
                    StatusPath.Text = "  " + path;

                    // The treenode is selected. Every node is tagged with a NodeData that includes LineNumber (from input file).
                    // This allows us to jump to the line in the file:
                    if (ShowText)
                    {
                        if (tn != null)
                        {
                            int Line = data.LineNumber;
                            if (Line > 0)
                                MoveToLine(Line);
                        }
                        //Object ln = tn.Tag;
                        //int line = Convert.ToInt32(ln.ToString());
                        //MoveToLine(line);
                    }
                    if (PathTester != null)
                    {
                        // If a pathtester has been open from this window then selection of a node causes
                        // automatic testing of its XPath in that window.
                        PathTester.txtTestExpression.Text = path;
                        PathTester.btnTest_Click(this, null);
                    }
                }
            }
            catch (Exception ex) { ReportError(ex); }
        }

        private void menuItem2_Click(object sender, System.EventArgs e)
        {
            WindowClose();
        }

        private void WindowClose()
        {
            this.Dispose();
        }

        //private void AppExit()
        //{
        //    Application.ExitTB();
        //}


        private void AssociateTag(TreeNode t, XmlReader reader)
        {
            NodeData nodedata = new NodeData() ;
            nodedata.LineNumber = 0;
            XmlTextReader textreader = reader as XmlTextReader;
            if (textreader!=null)
                nodedata.LineNumber=textreader.LineNumber;
            nodedata.NodeType=reader.NodeType;
            nodedata.Depth=reader.Depth;
            nodedata.IsEmptyElement = reader.IsEmptyElement;
            nodedata.NodeName = reader.Name;
            nodedata.NodeValue = reader.Value;
            t.Tag = nodedata;


            // Associate a line number Tag with every node in the tree
            //Object NodeTag1 = new Object();
            //NodeTag1 = l;
            //t.Tag = NodeTag1;

        }

        private void menuItem5_Click(object sender, System.EventArgs e)
        {
            // AboutBtn Box is clicked
            ShowAboutBox();
        }

        private void ShowAboutBox()
        {
            string text=null;
            text += "XMLTreeView:\n\rA form for browsing XML documents.\n\r\n\r" +
                "By Igor Grešovnik (2008).";
            MessageBox.Show(text);
        }

        private void EnableDisableControls()
        {
            // Enable Disable Buttons

            switch (CurrentView)
            {
                case 0:	// TREE VIEW
                    {
                        //ExpandAllTB.Enabled = true;
                        //CollapseAllTB.Enabled = true;
                        
                        // StopTB.Enabled = true;
                        treeViewXML.Visible = true;
                    }
                    break;
                case 1:	// REPORT VIEW
                    {
                        // ExpandAllTB.Enabled = false;
                        //CollapseAllTB.Enabled = false;
                        // StopTB.Enabled = false;
                    }
                    break;
            }
        }

 
        private void ThreadMessage(string message)
        // Launches a message that fades out in a new thread
        {
            new FadingMessage(message, 3000,0.5);
        }

        private void TExpandAll()
        {
            t = new Thread(new ThreadStart(ExpandAll));
            t.Start();
        }

        private void ExpandAll()
        {
            // Expand all _gridCoordinates in the tree
            treeViewXML.ExpandAll();
        }

        private void TCollapseAll()
        {
            // Collapse all _gridCoordinates in the tree
            treeViewXML.CollapseAll();
        }


        //private void Expand()
        //{
        //    ThreadMessage("Expanding exactly a specified messagelevel of _gridCoordinates is not implemented yet.");
        //}
        
        //private void ExpandMin()
        //{
        //    ThreadMessage("Expanding all _gridCoordinates until a specified messagelevel is not implemented yet.");
        //}

        //private void ExpandMax()
        //{
        //    ThreadMessage("Collapsing all _gridCoordinates above a specified messagelevel is not implemented yet.");
        //}


        void Clear()
        {
            TreeNodeArray.Clear();
        }

        private void menuItem1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            //Owner Draw Menu Items

            //Get the bounding rectangle
            Rectangle rc = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 5, e.Bounds.Height - 1);
            //Fill the rectangle
            e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), rc);
            //Unbox the menu item
            MenuItem s = (MenuItem)sender;
            string s1 = s.Text;
            //Set the stringformat object
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(s1, new Font("Ariel", 10), new SolidBrush(Color.Black), rc, sf);
            Console.WriteLine(e.State.ToString());

            //Check if the object is selected. 
            if (e.State == (DrawItemState.NoAccelerator | DrawItemState.Selected) ||
                e.State == (DrawItemState.NoAccelerator | DrawItemState.HotLight))
            {
                // Draw selected menu item
                e.Graphics.FillRectangle(new SolidBrush(Color.LightYellow), rc);
                e.Graphics.DrawString(s1, new Font("Veranda", 10, FontStyle.Bold), new SolidBrush(Color.Red), rc, sf);
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), rc);
            }
            e.DrawFocusRectangle();
            e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black), 1), rc);
        }

        private void menuItem1_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
        {
            // Set the menu item height
            e.ItemWidth = 75;
        }

        private void menuItem3_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
        {
            // Set the sub menu item height and width
            e.ItemWidth = 95;
            e.ItemHeight = 25;
        }

        private void menuItem3_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        // Custom drawing of 
        {
            //Owner Draw Sub Menu Items

            Rectangle rc = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
            e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), rc);
            MenuItem s = (MenuItem)sender;
            string s1 = s.Text;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;

            Image useImage = null;

            if (s1 == "Open")
            {
                useImage = img_fileopen;
            }
            if (s1 == "ExpandAll")
            {
                if (CurrentView == (int)VIEW.TREE_VIEW)
                    s.Enabled = true;
                else
                    s.Enabled = false;
                useImage = img_expand;
            }
            if (s1 == "Exit")
            {
                useImage = img_exit;
            }
            if (s1 == "CollapseAll")
            {
                if (CurrentView == (int)VIEW.TREE_VIEW)
                    s.Enabled = true;
                else
                    s.Enabled = false;
                useImage = img_collapse;
            }
            if (s1 == "AboutBtn")
            {
                useImage = img_about;
            }
            Rectangle rcText = rc;
            rcText.Width -= 5;
            e.Graphics.DrawString(s1, new Font("Veranda", 10), new SolidBrush(Color.Blue), rcText, sf);
            e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.LightGray)), rc);

            if (e.State == (DrawItemState.NoAccelerator | DrawItemState.Selected))
            {
                Rectangle rc1 = rc;
                rc1.X = rc1.X + useImage.Width + 5;
                rc1.Width = rc.Width - 25;
                rc1.Height = rc.Height - 2;
                e.Graphics.FillRectangle(new SolidBrush(Color.LightYellow), rc);
                e.Graphics.DrawString(s1, new Font("Veranda", 10, FontStyle.Bold | FontStyle.Underline), new SolidBrush(Color.Red), rcText, sf);
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black), 3), rc);
                e.DrawFocusRectangle();
            }

            if (useImage != null)
            {
                SizeF sz = useImage.PhysicalDimension;
                e.Graphics.DrawImage(useImage, e.Bounds.X + 5, (e.Bounds.Bottom + e.Bounds.Top) / 2 - sz.Height / 2);
            }
        }


        private void ExpAllBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
                TExpandAll();
            }
            catch (Exception ex) { ReportError(ex); }
        }

        private void CollAllBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
                TCollapseAll();
            }
            catch (Exception ex) { ReportError(ex); }
        }

        private void ExpandAllMenuItem_Click(object sender, EventArgs e)
        {
            TExpandAll();
        }

        private void CollapseAllMenuItem_Click(object sender, EventArgs e)
        {
            TCollapseAll();
        }

        private void ExpMinBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
                int numlevels = (int)LevelNumUpDown.Value;
                TreeNodeCollection nodes = treeViewXML.Nodes;
                foreach (TreeNode node in nodes)
                {
                    ExpandNodesAtLeast(node, numlevels);
                }
            }
            catch (Exception ex) { ReportError(ex); }
        }

        private void ExpMaxBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
                int numlevels = (int)LevelNumUpDown.Value;
                TreeNodeCollection nodes = treeViewXML.Nodes;
                foreach (TreeNode node in nodes)
                {
                    ExpandNodesAtMost(node, numlevels);
                }
            }
            catch (Exception ex) { ReportError(ex); }
        }

        private void ControlPanelSwitchMenuItem_Click(object sender, EventArgs e)
        // Switch top control panel on or off
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {
                if (item.Checked)
                {
                    item.Checked = false;
                    HideControlPnl();
                } else
                {
                    item.Checked = true;
                    ShowControlPnl();
                }
            }

        }

        private void TopPnlHideBtn_Click(object sender, EventArgs e)
        {
            MenuTools_Controls.Checked = false;
            HideControlPnl();
        }

        void ShowControlPnl()
        {
            ControlPnl.Visible = true;
        }

        void HideControlPnl()
        {
            ControlPnl.Visible = false;
        }

        private void ExpBtn_Click(object sender, EventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
                int numlevels = (int)LevelNumUpDown.Value;
                TreeNodeCollection nodes = treeViewXML.Nodes;
                foreach (TreeNode node in nodes)
                {
                    ExpandNodesExactly(node, numlevels);
                }
            }
            catch (Exception ex) { ReportError(ex); }
        }


        #region MENU_MAIN

        private void MenuFile_Open_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
        }

        private void MenuFile_Close_Click(object sender, EventArgs e)
        {
            WindowClose();
        }

        private void MenuFile_CloseDocument_Click(object sender, EventArgs e)
        {
            DocumentLoaded = false;
        }

        private void MenuTools_Edit_Click(object sender, EventArgs e)
        // Launches an editor fot the selected node.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                EditNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void MenuTools_Cut_Click(object sender, EventArgs e)
        // Cuts the selected node and stores its copy in clipboard.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                CutNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void MenuTools_Copy_Click(object sender, EventArgs e)
        // Copies the selected node to clipboard.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                CopyNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void MenuTools_Paste_Click(object sender, EventArgs e)
        // Pastes the clipboard content at the selected node.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                PasteNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }


        private void PathTester_Close()
        // This method will be passed through a delegate to the XMLTestPath consform to 
        // be executed when the consform is closed (in order to close the connection)
        {
            PathTester = null;
        }

        private void MenuTools_TestXPath_Click(object sender, EventArgs e)
        // Launches a panel for testing the XPath expressions.
        {
            try
            {
                bool DoOpen = true;
                if (PathTester!=null)
                {
                    // One path tester is already open for this window, test if it is active and
                    // then ask if the user really wants to open another one:
                    try
                    {
                        PathTester.Activate();
                        //$$$$Exclude: UtilForms.BlinkForm(PathTester);
                        DialogResult res = MessageBox.Show("An XPath tester is already open.\r\nOpen another PathTester window?", "Opening confirmation", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (res == DialogResult.No)
                            DoOpen = false;
                    }
                    // If pointer to another XPath tester references an non-existing consform then
                    // DoOpen will remain true and the consform will be launched.
                    catch { }
                }
                if (DoOpen)
                {
                    if (treeViewXML != null)
                    {
                        string path = getTreeNodePath(treeViewXML.SelectedNode);
                        XMLTestPath frm = new XMLTestPath(path);
                        TreeNode node = treeViewXML.SelectedNode;
                        frm.Doc = Doc;
                        if (node != null)
                            frm.InitialPath = getTreeNodePath(node);
                        PathTester = frm; // register the path tester
                        frm.CloseDelegate = new XMLTestPath.MyDelegate(PathTester_Close);
                        frm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Open an xml document prior to starting a test.", "No data loaded.");
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }


        private void MenuTools_Controls_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                if (item.Checked)
                {
                    item.Checked = false;
                    HideControlPnl();
                }
                else
                {
                    item.Checked = true;
                    ShowControlPnl();
                }
            }
        }


        private void MenuTools_ExpandAll_Click(object sender, EventArgs e)
        {
            TExpandAll();
        }

        private void MenuTools_CollapseAll_Click(object sender, EventArgs e)
        {
            TCollapseAll();
        }

        private void MenuTools_Stop_Click(object sender, EventArgs e)
        {

        }

        private void MenuHelp_About_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }

        #endregion     // MENU_MAIN




        #region MENU_CONTEXT_NODE

        private void ContextMenu_CopyFullPath_Click(object sender, EventArgs e)
        {
            string tmp = null;
            TreeNode tn = treeViewXML.SelectedNode;
            try
            {
                tmp = getTreeNodePath(tn);
                ////tmp = tn.FullPath;
                ////tmp = tmp.Replace("#document", "/");

                // check to see if there is an attribute present;
                // you'l likely query the attribute; this can
                // get hosed if the user selects this option and
                // has not clicked on an attribute'result value
                int pos = 0;
                pos = tmp.LastIndexOf('@');
                pos = pos - 1;

                if (pos != 0)
                {
                    tmp = tmp.Remove(pos, 1);
                    tmp = tmp.Insert(pos, "[");

                    int posSlash = treeViewXML.SelectedNode.FullPath.LastIndexOf('/');

                    if (posSlash < pos)
                    {
                        tmp += "='KeyValueHere']";
                    }
                    else
                    {
                        tmp = tmp.Remove(posSlash - 8, 1);
                        tmp = tmp.Insert(posSlash - 8, "='");
                        tmp += "']";
                    }
                }
            }
            catch
            {
                // if it fails, just select the selected node'result
                // full path
                tmp = getTreeNodePath(tn);

            }
            finally
            {
                // put it in the clip board
                Clipboard.SetDataObject(tmp, true);
            }
            new FadingMessage(tmp, "< Path (attr.) >", 800, 0.2);
        }


        private void ContextMenu_CopyFullPathExactly_Click(object sender, EventArgs e)
        {
            string tmp = null;
            TreeNode tn = treeViewXML.SelectedNode;
            try
            {
                tmp = getTreeNodePath(tn);
            }
            catch
            {
            }
            finally
            {
                // put it in the clip board
                Clipboard.SetDataObject(tmp, true);
            }
            new FadingMessage(tmp, "< Path (exact) >", 800, 0.2);
        }


        private void ContextMenu_CopyNodeText_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                string tmp = tn.Text;
                Clipboard.SetDataObject(tmp, true);
                new FadingMessage(tmp, "< Node text >", 800, 0.2);
            }
            catch { }
        }

        private void ContextMenu_Edit_Click(object sender, EventArgs e)
        // Launches an editor for the node for which the context menu was launched.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                EditNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }


        private void ContextMenu_Cut_Click(object sender, EventArgs e)
        // Cuts the node and copies it to the clipboard.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                EditNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_Copy_Click(object sender, EventArgs e)
        // Copies the node to clipboard.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                EditNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_Paste_Click(object sender, EventArgs e)
        // Pastes clipboard content at the node.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                EditNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_Delete_Click(object sender, EventArgs e)
        // Deletes the node.
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                EditNode(tn);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }


        private void ContextMenu_ExpandAll_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                tn.ExpandAll();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandSpecified_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = (int) LevelNumUpDown.Value;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesExactly(tn,numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_Expand1_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 1;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesExactly(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_Expand2_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 2;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesExactly(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_Expand3_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 3;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesExactly(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_Expand4_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 4;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesExactly(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_Expand5_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 5;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesExactly(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }



        private void ContextMenu_ExpandAtLeastSpecified_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = (int) LevelNumUpDown.Value;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtLeast(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }


        private void ContextMenu_ExpandAtLeast1_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 1;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtLeast(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtLeast2_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 2;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtLeast(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtLeast3_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 3;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtLeast(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtLeast4_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 4;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtLeast(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtLeast5_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 5;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtLeast(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
         }



        private void ContextMenu_ExpandAtMostSpecified_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = (int) LevelNumUpDown.Value;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtMost(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtMost1_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 1;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtMost(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtMost2_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 2;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtMost(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtMost3_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 3;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtMost(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtMost4_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 4;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtMost(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ContextMenu_ExpandAtMost5_Click(object sender, EventArgs e)
        {
            try
            {
                int numlevels = 5;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesAtMost(tn, numlevels);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }


        private void ContextMenu_Collapse_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode tn = treeViewXML.SelectedNode;
                tn.Collapse();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }


        #endregion   // MENU_CONTEXT_NODE

        private void treeViewXML_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            try
            {
                TreeNode node = e.Node;
                if (node != null && docloaded)
                {
                    node.ToolTipText = "...";
                    NodeData data = node.Tag as NodeData;
                    if (!isAttributeContainter(node) )
                    {
                        string nodedescription = "";
                        if (data!=null)
                            nodedescription += /* "Node type: "*/  data.NodeType.ToString() + "\r\n";
                        string path = getTreeNodePath(node);
                        if (path != null)
                            nodedescription += "At '" + path + "'\r\n" ;
                        nodedescription+=  /* "Node value (possibly shortened and decorateed):\r\d2'" */ 
                            "\r\n\"" + node.Text + "\"";
                        node.ToolTipText=nodedescription;
                        // new FadeMessage(nodedescription,2000);
                    }
                }
            }
            catch(Exception ex)
            {
                this.StatusError.Text = "Error (MouseHover): "+ex.Message;
            }
        }

        private void XMLTreeView_Closing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (PathTester != null)
                    PathTester.Close();
            }
            catch { }
        }

        private void StatusError_Click(object sender, EventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
            }
            catch { }
        }

        private void StatusStatus_Click(object sender, EventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
            }
            catch { }
        }

        private void MenuStrip_Enter(object sender, EventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
            }
            catch { }
        }

        private void NodeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                StatusStatus.Text = StatusError.Text = "";  // reset contents of status lines
            }
            catch { }
        }




        public static void Example()
        {
            //  XML TreeView Tests:
            Application.Run(new XMLTreeView());
            XMLTreeView tv;
            Environment.Exit(0);
            tv = new XMLTreeView();
            tv.ShowInTaskbar = true;     // Override the default setting
            tv.ShowDialog();

            XMLTreeView tv1;
            tv1 = new XMLTreeView();
            tv1.ShowDialog();

            tv.ShowDialog();
        } // Example()



    }









}
