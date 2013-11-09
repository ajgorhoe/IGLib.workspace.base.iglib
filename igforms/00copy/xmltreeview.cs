using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading ;
using System.Text; 
using System.IO ;
using System.Xml;


//
// Read an XML Document and display the file as a Tree .
//
// Shripad Kulkarni 
// Date : May 15, 2002
// 

namespace IG.Forms
{

    /// <summary>
    /// Summary description for XMLTreeView.
    /// </summary>
    /// 
    public class XMLTreeView : System.Windows.Forms.Form
    {

        // Internal variables defining the behavior:
        protected System.Drawing.Color
            RootBackColor = Color.FromArgb(255, 180, 180),
            RootForeColor = Color.Black,
            ElementBackColor = Color.White,
            ElementForeColor = Color.DarkRed,
            TextBackColor = Color.FromArgb(255, 255, 180),
            TextForeColor = Color.Black,
            AttrNameBackColor = Color.White,
            AttrNameForeColor = Color.Red,
            AttrValBackColor = Color.White,
            AttrValForeColor = Color.Green,
            CommentBackColor = Color.White,
            CommentForeColor = Color.Gray,
            AttrContBackColor = Color.White,
            AttrContForeColor = Color.LightGray;
        protected int
            RootImageIndex = 0,
            ElementImageIndex = 1,
            TextImageIndex = 2,
            AttrNameImageIndex = 6,
            AttrValImageIndex = 7,
            CommentImageIndex = 8,
            AttrContImageIndex = 9;


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


        //Error Counters (for counting errors for which a message box is launched):
        int NumErr = 0, MaxErr = 3;




        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem FileMenuItem;

        private System.Windows.Forms.MenuItem FileCloseMenuItem;
        private System.Windows.Forms.MenuItem FileOpenMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private IContainer components;

        enum VIEW { TREE_VIEW = 0 } ;
        string XMLInputFile = null;
        string FileSize = "";
        string WorkingDir = Directory.GetCurrentDirectory();
        string OrigFormTitle = "";
        bool bFileLoaded = false;
        int CurrentView = (int)VIEW.TREE_VIEW;
        Object NodeTag = null;
        Thread t = null;
        TreeNode RootNode = null;
        Point ClickedPoint = new Point(0, 0);
        ArrayList TreeNodeArray = new ArrayList();
        ImageList tr_il = new ImageList();
        ImageList tb_il = new ImageList();

        Bitmap img_fileopen, img_exit, img_collapse, img_expand, img_about;

        private System.Windows.Forms.MenuItem HelpMenuItem;
        private System.Windows.Forms.MenuItem HelpAboutMenuItem;
        private System.Windows.Forms.ToolBar toolBar1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolBarButton Open;
        private System.Windows.Forms.ToolBarButton Exit;
        private System.Windows.Forms.ToolBarButton AboutBtn;
        private System.Windows.Forms.ToolBarButton SEPARATOR1;
        private System.Windows.Forms.ToolBarButton ExpandAll;
        private System.Windows.Forms.ToolBarButton CollapseAll;
        private System.Windows.Forms.ToolBarButton Stop;

        private System.Windows.Forms.ToolBarButton SEPARATOR2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Splitter splitter1;
        private Panel DispPanel;
        private MenuItem ToolsMenuItem;
        private MenuItem ExpandAllMenuItem;
        private MenuItem CollapseAllMenuItem;
        private MenuItem StopMenuItem;
        private System.Windows.Forms.ToolBarButton SEPARATOR3;
        delegate void MyDelegate();

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.FileMenuItem = new System.Windows.Forms.MenuItem();
            this.FileOpenMenuItem = new System.Windows.Forms.MenuItem();
            this.FileCloseMenuItem = new System.Windows.Forms.MenuItem();
            this.ToolsMenuItem = new System.Windows.Forms.MenuItem();
            this.ExpandAllMenuItem = new System.Windows.Forms.MenuItem();
            this.CollapseAllMenuItem = new System.Windows.Forms.MenuItem();
            this.StopMenuItem = new System.Windows.Forms.MenuItem();
            this.HelpMenuItem = new System.Windows.Forms.MenuItem();
            this.HelpAboutMenuItem = new System.Windows.Forms.MenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.Open = new System.Windows.Forms.ToolBarButton();
            this.SEPARATOR1 = new System.Windows.Forms.ToolBarButton();
            this.ExpandAll = new System.Windows.Forms.ToolBarButton();
            this.CollapseAll = new System.Windows.Forms.ToolBarButton();
            this.Stop = new System.Windows.Forms.ToolBarButton();
            this.SEPARATOR3 = new System.Windows.Forms.ToolBarButton();
            this.Exit = new System.Windows.Forms.ToolBarButton();
            this.AboutBtn = new System.Windows.Forms.ToolBarButton();
            this.SEPARATOR2 = new System.Windows.Forms.ToolBarButton();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.DispPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FileMenuItem,
            this.ToolsMenuItem,
            this.HelpMenuItem});
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.Index = 0;
            this.FileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FileOpenMenuItem,
            this.FileCloseMenuItem});
            this.FileMenuItem.Text = "&File";
            this.FileMenuItem.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.menuItem1_DrawItem);
            this.FileMenuItem.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.menuItem1_MeasureItem);
            // 
            // FileOpenMenuItem
            // 
            this.FileOpenMenuItem.Index = 0;
            this.FileOpenMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.FileOpenMenuItem.Text = "Open";
            this.FileOpenMenuItem.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.menuItem3_DrawItem);
            this.FileOpenMenuItem.Click += new System.EventHandler(this.menuItem3_Click);
            this.FileOpenMenuItem.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.menuItem3_MeasureItem);
            // 
            // FileCloseMenuItem
            // 
            this.FileCloseMenuItem.Index = 1;
            this.FileCloseMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
            this.FileCloseMenuItem.Text = "Close";
            this.FileCloseMenuItem.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.menuItem3_DrawItem);
            this.FileCloseMenuItem.Click += new System.EventHandler(this.menuItem2_Click);
            this.FileCloseMenuItem.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.menuItem3_MeasureItem);
            // 
            // ToolsMenuItem
            // 
            this.ToolsMenuItem.Index = 1;
            this.ToolsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ExpandAllMenuItem,
            this.CollapseAllMenuItem,
            this.StopMenuItem});
            this.ToolsMenuItem.Text = "T&ools";
            // 
            // ExpandAllMenuItem
            // 
            this.ExpandAllMenuItem.Index = 0;
            this.ExpandAllMenuItem.Text = "Expand all";
            // 
            // CollapseAllMenuItem
            // 
            this.CollapseAllMenuItem.Index = 1;
            this.CollapseAllMenuItem.Text = "Collapse all";
            // 
            // StopMenuItem
            // 
            this.StopMenuItem.Index = 2;
            this.StopMenuItem.Text = "Stop";
            // 
            // HelpMenuItem
            // 
            this.HelpMenuItem.Index = 2;
            this.HelpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.HelpAboutMenuItem});
            this.HelpMenuItem.Text = "&Help";
            this.HelpMenuItem.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.menuItem1_DrawItem);
            this.HelpMenuItem.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.menuItem1_MeasureItem);
            // 
            // HelpAboutMenuItem
            // 
            this.HelpAboutMenuItem.Index = 0;
            this.HelpAboutMenuItem.Text = "About";
            this.HelpAboutMenuItem.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.menuItem3_DrawItem);
            this.HelpAboutMenuItem.Click += new System.EventHandler(this.menuItem5_Click);
            this.HelpAboutMenuItem.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.menuItem3_MeasureItem);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "XML FILES| *.xml";
            this.openFileDialog1.InitialDirectory = "c:\\Users\\ajgorhoe\\cvis\\igcs\\XMLview\\0xml\\";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // toolBar1
            // 
            this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.Open,
            this.SEPARATOR1,
            this.ExpandAll,
            this.CollapseAll,
            this.Stop,
            this.SEPARATOR3,
            this.Exit,
            this.AboutBtn,
            this.SEPARATOR2});
            this.toolBar1.ButtonSize = new System.Drawing.Size(16, 16);
            this.toolBar1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Location = new System.Drawing.Point(10, 10);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(675, 28);
            this.toolBar1.TabIndex = 0;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // Open
            // 
            this.Open.ImageIndex = 0;
            this.Open.Name = "Open";
            this.Open.ToolTipText = "Open XML File";
            // 
            // SEPARATOR1
            // 
            this.SEPARATOR1.Name = "SEPARATOR1";
            this.SEPARATOR1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // ExpandAll
            // 
            this.ExpandAll.Enabled = false;
            this.ExpandAll.ImageIndex = 3;
            this.ExpandAll.Name = "ExpandAll";
            this.ExpandAll.ToolTipText = "Expand All Nodes";
            // 
            // CollapseAll
            // 
            this.CollapseAll.Enabled = false;
            this.CollapseAll.ImageIndex = 4;
            this.CollapseAll.Name = "CollapseAll";
            this.CollapseAll.ToolTipText = "Collapse All Nodes";
            // 
            // Stop
            // 
            this.Stop.Enabled = false;
            this.Stop.ImageIndex = 5;
            this.Stop.Name = "Stop";
            this.Stop.ToolTipText = "Stop";
            // 
            // SEPARATOR3
            // 
            this.SEPARATOR3.Name = "SEPARATOR3";
            this.SEPARATOR3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // Exit
            // 
            this.Exit.ImageIndex = 1;
            this.Exit.Name = "Exit";
            this.Exit.ToolTipText = "Exit Application";
            // 
            // AboutBtn
            // 
            this.AboutBtn.ImageIndex = 2;
            this.AboutBtn.Name = "AboutBtn";
            this.AboutBtn.ToolTipText = "Help About";
            // 
            // SEPARATOR2
            // 
            this.SEPARATOR2.Name = "SEPARATOR2";
            this.SEPARATOR2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.treeView1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.FullRowSelect = true;
            this.treeView1.HotTracking = true;
            this.treeView1.ItemHeight = 16;
            this.treeView1.Location = new System.Drawing.Point(10, 38);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(360, 516);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.ForeColor = System.Drawing.Color.Maroon;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(380, 38);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(305, 516);
            this.listBox1.TabIndex = 12;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(370, 38);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 516);
            this.splitter1.TabIndex = 11;
            this.splitter1.TabStop = false;
            // 
            // DispPanel
            // 
            this.DispPanel.Location = new System.Drawing.Point(425, -16);
            this.DispPanel.Name = "DispPanel";
            this.DispPanel.Size = new System.Drawing.Size(166, 108);
            this.DispPanel.TabIndex = 13;
            // 
            // XMLTreeView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(695, 564);
            this.Controls.Add(this.DispPanel);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolBar1);
            this.Menu = this.mainMenu1;
            this.Name = "XMLTreeView";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowInTaskbar = false;
            this.Text = "XML Viewer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        void Main()
        {
            Application.Run(new XMLTreeView());
        }



        private void Form1_Load(object sender, System.EventArgs e)
        {
            // Add images to the imageList for treeView
            try
            {
                tr_il.Images.Add(new Icon(WorkingDir + "\\ROOT.ICO"));		//ROOT		0
                tr_il.Images.Add(new Icon(WorkingDir + "\\ELEMENT.ICO"));	//ELEMENT	1
                tr_il.Images.Add(new Icon(WorkingDir + "\\EQUAL.ICO"));		//ATTRIBUTE	2
                treeView1.ImageList = tr_il;
            }
            catch { }

            // Add images to the imageList for Toolbar
            try
            {
                tb_il.Images.Add(new Bitmap(WorkingDir + "\\FileOpen.bmp"));		//ROOT		0
                tb_il.Images.Add(new Bitmap(WorkingDir + "\\exit.bmp"));			//ELEMENT	1
                tb_il.Images.Add(new Bitmap(WorkingDir + "\\about.bmp"));		//ATTRIBUTE	2
                tb_il.Images.Add(new Bitmap(WorkingDir + "\\ExpandTree.bmp"));	//ATTRIBUTE	3
                tb_il.Images.Add(new Bitmap(WorkingDir + "\\CollapseTree.bmp"));	//ATTRIBUTE	4
                tb_il.Images.Add(new Bitmap(WorkingDir + "\\Stop.bmp"));			//ATTRIBUTE	5
                toolBar1.ImageList = tb_il;
            }
            catch { }


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

            OrigFormTitle = this.Text;

        }


        private void menuItem3_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Initialize Buttons
            EnableDisableControls();

            // Initialize All the Arrays
            treeView1.Nodes.Clear();
            listBox1.Items.Clear();
            TreeNodeArray.Clear();

            bFileLoaded = false;

            XMLInputFile = openFileDialog1.FileName;
            this.Text = OrigFormTitle + " ..." + XMLInputFile;
            openFileDialog1.Dispose();

            // Get the filename and filesize
            FileInfo f = new FileInfo(XMLInputFile);
            FileSize = f.Length.ToString();

            // Begin thread to read input file and load into the ListBox
            Thread t = new Thread(new ThreadStart(PopulateList));
            t.Start();

            // Begin thread to read input file and populate the Tree
            Thread tt = new Thread(new ThreadStart(PopulateTree));
            tt.Start();
        }

        private void PopulateList()
        {
            // Load the File
            LoadFileIntoListBox();
        }

        private void PopulateTree()
        {
            // TreeView Nodes cannot be added in a thread , until the thread is marshalled
            // using an Invoke or beginInvoke call.
            // We create a delegate ( Funtion Pointer ) and invoke the thread using he delegate
            MyDelegate dlg_obj;
            dlg_obj = new MyDelegate(ParseFile);
            treeView1.Invoke(dlg_obj);
        }






        private void ParseFile()
        {
            // Use the XMLReader class to read the XML File and populate the
            // treeview
            try
            {
                XmlTextReader reader = null;
                reader = new XmlTextReader(XMLInputFile);
                reader.WhitespaceHandling = WhitespaceHandling.None;
                string readerName = "";
                bool start_node = false;
                int depth = 0;
                TreeNode WORKINGNODE = null;
                RootNode = null;
                TreeNode AttrNode = null;
                TreeNode newNode = null;
                bool bIsEmpty = false;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            readerName = reader.Name;
                            bIsEmpty = reader.IsEmptyElement;

                            if (!start_node)
                            {
                                // The element read is the root element.
                                start_node = true;
                                RootNode = this.treeView1.Nodes.Add(readerName);
                                AssociateTag(RootNode, reader.LineNumber);
                                RootNode.SelectedImageIndex = RootImageIndex;
                                RootNode.ImageIndex = RootImageIndex;
                                RootNode.BackColor = RootBackColor;
                                RootNode.ForeColor = RootForeColor;
                                continue;
                            }
                            depth = reader.Depth;

                            if (reader.IsStartElement() && depth == 1)
                            {
                                // Root element:
                                WORKINGNODE = RootNode.Nodes.Add(reader.Name);
                                AssociateTag(WORKINGNODE, reader.LineNumber);
                                // WORKINGNODE.BackColor = ElementBackColor;
                                // WORKINGNODE.ForeColor = ElementForeColor;
                            }
                            else
                            {
                                // Ordinary element (Element node is read):
                                TreeNode parent = WORKINGNODE;
                                WORKINGNODE = parent.Nodes.Add(reader.Name);
                                AssociateTag(WORKINGNODE, reader.LineNumber);
                                // WORKINGNODE.BackColor = ElementBackColor;
                                // WORKINGNODE.ForeColor = ElementForeColor;
                            }

                            WORKINGNODE.SelectedImageIndex = ElementImageIndex;
                            WORKINGNODE.ImageIndex = ElementImageIndex;
                            WORKINGNODE.BackColor = ElementBackColor;
                            WORKINGNODE.ForeColor = ElementForeColor;

                            // Containter node for holding the attributes:
                            TreeNode AttrContNode = WORKINGNODE.Nodes.Add(">> attributes");
                            AttrContNode.SelectedImageIndex = AttrContImageIndex;
                            AttrContNode.ImageIndex = AttrContImageIndex;
                            AttrContNode.BackColor = AttrContBackColor;
                            AttrContNode.ForeColor = AttrContForeColor;
                            AssociateTag(AttrContNode, reader.LineNumber);


                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                // Go through attributes
                                reader.MoveToAttribute(i);
                                string rValue = reader.Value.Replace("\r\n", " ");
                                AttrNode = AttrContNode.Nodes.Add(reader.Name);
                                //	AttrNode = WORKINGNODE.Nodes.Add(reader.Name +"="+rValue);
                                AssociateTag(AttrNode, reader.LineNumber);

                                AttrNode.SelectedImageIndex = AttrNameImageIndex;
                                AttrNode.ImageIndex = AttrNameImageIndex;
                                AttrNode.BackColor = AttrNameBackColor;
                                AttrNode.ForeColor = AttrNameForeColor;
                                TreeNode tmp = AttrNode.Nodes.Add(rValue);
                                tmp.SelectedImageIndex = AttrValImageIndex;
                                tmp.ImageIndex = AttrValImageIndex;
                                tmp.BackColor = AttrValBackColor;
                                tmp.ForeColor = AttrValForeColor;
                                AssociateTag(tmp, reader.LineNumber);

                                AttrNode.SelectedImageIndex = 2;
                                AttrNode.ImageIndex = 2;

                            }

                            if (bIsEmpty)
                                WORKINGNODE = WORKINGNODE.Parent;

                            break;
                        case XmlNodeType.Text:
                            {
                                string rValue = reader.Value.Replace("\r\n", " ");
                                newNode = WORKINGNODE.Nodes.Add(rValue);
                                AssociateTag(newNode, reader.LineNumber);
                                newNode.SelectedImageIndex = 2;
                                newNode.ImageIndex = 2;
                                newNode.BackColor = TextBackColor;
                                newNode.ForeColor = TextForeColor;
                            }
                            break;
                        case XmlNodeType.Comment:
                            {
                                TreeNode commentNode = null;
                                try
                                {
                                    if (WORKINGNODE != null)
                                    {
                                        string rValue;
                                        rValue= reader.Value.Replace("\r\n", " ");

                                        commentNode = WORKINGNODE.Nodes.Add(rValue);
                                        AssociateTag(commentNode, reader.LineNumber);
                                        commentNode.SelectedImageIndex = 15;
                                        commentNode.ImageIndex = 15;
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
                                            text+="XML documents contains comments before the root element.\n\r";
                                            text += "These comments will not be shown in the tree.\n\r\n\r";
                                            text += "Further messages of this type will be omitted.";
                                            new FadeMessage(text,4000);
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
                        case XmlNodeType.EndElement:
                            WORKINGNODE = WORKINGNODE.Parent;
                            break;
                    }
                }
                reader.Close();
                RootNode.Expand();

            }
            catch (Exception eee)
            {
                Console.WriteLine(eee.Message);
            }
        }






        private void treeView1_Click(object sender, System.EventArgs e)
        {
        }

        private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            if (!bFileLoaded) return;

            // The treenode is selected. Every node is tagged with LineNumber ( from input file ).
            // This allows us to jump to the line in the file.
            try
            {
                TreeNode tn;
                tn = (TreeNode)e.Node;
                Object ln = tn.Tag;
                int line = Convert.ToInt32(ln.ToString());
                MoveToLine(line);
            }
            catch { }
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
        //    Application.Exit();
        //}

        private void LoadFileIntoListBox()
        {
            // Load the xml file into a listbox.
            try
            {
                StreamReader sr = new StreamReader(XMLInputFile, Encoding.ASCII);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                while (sr.Peek() > -1)
                {
                    Thread.Sleep(5);
                    string str = sr.ReadLine();
                    listBox1.Items.Add(str);
                }
                sr.Close();
                bFileLoaded = true;
                listBox1.SetSelected(1, true);
            }
            catch (Exception ee)
            {
                Console.WriteLine("Error Reading File into ListBox " + ee.Message);
            }
        }

        private void MoveToLine(int ln)
        {
            // Select the input line from the file in the listbox
            try
            {
                listBox1.SetSelected(ln - 1, true);
            }
            catch { }
        }

        private void AssociateTag(TreeNode t, int l)
        {
            // Associate a line number Tag with every node in the tree
            NodeTag = new Object();
            NodeTag = l;
            t.Tag = NodeTag;
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
                "By Igor Grešovnik (2008), based on XMLTree by Shripad Kulkarni.";
            MessageBox.Show(text);
        }

        private void EnableDisableControls()
        {
            // Enable Disable Buttons

            switch (CurrentView)
            {
                case 0:	// TREE VIEW
                    {
                        ExpandAll.Enabled = true;
                        CollapseAll.Enabled = true;
                        Stop.Enabled = true;
                        treeView1.Visible = true;
                    }
                    break;
                case 1:	// REPORT VIEW
                    {
                        ExpandAll.Enabled = false;
                        CollapseAll.Enabled = false;
                        Stop.Enabled = false;
                    }
                    break;
            }
        }

        private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            // Invoke the function upon clicking the toolbar button
            int x = int.Parse(toolBar1.Buttons.IndexOf(e.Button).ToString());

            switch (toolBar1.Buttons.IndexOf(e.Button))
            {
                case 0:
                    openFileDialog1.ShowDialog(this);
                    break;
                case 1:
                    //separator
                    break;
                case 2:
                    TExpandAll();
                    break;
                case 3:
                    TCollapseAll();
                    break;
                case 4:
                    if (t != null && t.IsAlive)
                    {
                        t.Abort();
                        t = null;
                        EnableDisableControls();
                    }
                    break;
                case 5:
                    break;
                case 6:
                    WindowClose();
                    break;
                case 7:
                    ShowAboutBox();
                    break;
                case 8:	// separator
                    //separator
                    break;
                case 9:	// Go Back 
                    break;
                case 10:// separator
                    break;
                case 11:// Go Back 
                    CurrentView = (int)VIEW.TREE_VIEW;
                    EnableDisableControls();
                    break;
            }
        }

        private void TExpandAll()
        {
            t = new Thread(new ThreadStart(TE));
            t.Start();
        }

        private void TE()
        {
            // Expand all nodes in the tree
            treeView1.ExpandAll();
        }

        private void TCollapseAll()
        {
            // Collapse all nodes in the tree
            treeView1.CollapseAll();
        }

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

        private void Form1_Resize(object sender, System.EventArgs e)
        {
        }


    }



    public class FadingMessage
    {

        public FadingMessage(string Message, int ShowTime, double FadingTimePortion)
        { basFadingMessage(Message, ShowTime, FadingTimePortion); }
        public FadingMessage(string Message, int ShowTime)
        { basFadingMessage(Message, ShowTime, DefaultFadingTimePortion); }
        public FadingMessage(string Message)
        { basFadingMessage(Message, DefaultShowTime, DefaultFadingTimePortion); }

        string text;
        int showtime;
        double fadingportion;
        Thread msgthread = null, manipulationthread = null;
        private void basFadingMessage(string Message, int ShowTime, double FadingPortion)
        {
            text = Message + "\n\r\n\r>> Fading Message not yet implemented.\n\r";
            showtime = ShowTime;
            fadingportion = FadingPortion;
            manipulationthread = new Thread(new ThreadStart(ManipulationThreadFunc));
            manipulationthread.Start();
        }

        private void ManipulationThreadFunc()
        {
            try
            {
                msgthread = new Thread(new ThreadStart(MessageThreadFunc));
                msgthread.Start();
                Thread.Sleep(showtime);
                msgthread.Abort();
                // report that the thread has been aborted:
                //text = "Thread aborted.";
                //msgthread = new Thread(new ThreadStart(MessageThreadFunc));
                //msgthread.Start();
                //Thread.Sleep(showtime);
                //msgthread.Abort();
            }
            catch { }
        }
        [STAThread]
        private void MessageThreadFunc()
        {
            try
            {
                MessageBox.Show(text);
            }
            catch (Exception) { }
        }

        private static int defaultShowtime = 3000;
        private static double defaultFadingTimePortion = 0.3;
        public static int DefaultShowTime
        {
            set { if (value < 500) value = 500; defaultShowtime = value; }
            get { return defaultShowtime; }
        }
        public static double DefaultFadingTimePortion
        {
            set { if (value < 0) value = 0; if (value > 1) value = 1; defaultFadingTimePortion = value; }
            get { return defaultFadingTimePortion; }
        }


    }






}
