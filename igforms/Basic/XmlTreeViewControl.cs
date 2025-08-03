using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.Collections;
using System.IO;

#if NETFRAMEWORK
using MenuItem = System.Windows.Forms.MenuItem;
#else
using MenuItem = System.Windows.Forms.ToolStripMenuItem;
#endif

namespace IG.Forms
{
    public partial class XmlTreeViewControl : UserControl
    {
        public XmlTreeViewControl()
        {
            InitializeComponent();

            richTextBoxXML.Text = InitialTextRichTextBoxXml;
        }



        #region DEFINITIONS

        protected class NodeData
        // Class used for storing node data in TreeView tags.
        {
            public int LineNumber = 0;
            public XmlNodeType NodeType = XmlNodeType.None;
            public bool IsEmptyElement = true;
            public int Depth = -1;
            public XmlNode Node = null;
            public string
                NodeName = null,
                NodeValue = null;
        }

        public enum XMLSourceType : byte
        {
            TextFile,
            String,
            XMLNode
        };


        private string _InitialTextRichTextBoxXML = "<< Drag & drop XML files above. >>";

        /// <summary>Text that is initially written </summary>
        public string InitialTextRichTextBoxXml
        {
            get { return _InitialTextRichTextBoxXML; }
        }

        Cursor CursorDefault = Cursors.Default;
        Color
            ActiveBackColor = Color.White,
            InActiveBackColor = Color.WhiteSmoke;


        public bool
            Editable = true,
            ShowText = true;

        protected XmlDocument Doc = null;
        protected XMLTestPathForm PathTester = null;



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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool HasChanged
        {

            // Document.pa

            get { return haschanged; }
            internal set { haschanged = value; }
        }

        private bool docloaded = false;
        // Indicaion that a document is loaded and ready for user manipulation.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool DocumentLoaded
        {
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
                    // richTextBoxXML.Clear();
                    richTextBoxXML.Text = InitialTextRichTextBoxXml;
                }
            }
        }

        // Properties defining whether the subnodes are expanded or not, and whether 
        // attribute values are shown as subnodes or together with attribute names.
        private bool expattr = true,
            attsubnode = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ExpAttr
        {
            set { expattr = value; }
            get { return expattr; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AttrSubnode
        {
            set { attsubnode = value; }
            get { return attsubnode; }
        }

        internal bool
            DrawDocumentNode = true;  // If true then the document node is an outer-most node drawn.
                                      // In this way,comments before or after the root node can be included.
        internal string DocumentLabel = "XML Document";


        //// TODO: DELETE BELOW:
        ////Error Counters (for counting errors for which a message box is launched):
        //int NumErr = 0, MaxErr = 3;
        //private ToolStripMenuItem editToolStripMenuItem;
        //private ToolStripMenuItem editElementToolStripMenuItem;
        //private ToolStripMenuItem cutToolStripMenuItem;
        //private ToolStripMenuItem copyToolStripMenuItem;
        //private ToolStripMenuItem pasteToolStripMenuItem;
        //private ImageList NodeImages;
        //private ToolStripMenuItem MenuTools_TestXPath;
        //private ToolStripSeparator toolStripSeparator5;
        //private ToolStripSeparator toolStripSeparator6;
        //private ToolTip toolTip1;
        //private ToolStripMenuItem deleteToolStripMenuItem;
        //private ToolStripMenuItem MenuFile_CloseDocument;
        //private ToolStripMenuItem ContextMenu_Edit;
        //private ToolStripMenuItem ContextMenu_Cut;
        //private ToolStripMenuItem ContextMenu_Copy;
        //private ToolStripMenuItem ContextMenu_Paste;
        //private ToolStripMenuItem ContextMenu_Delete;
        //private RichTextBox richTextBoxXML;
        //private ToolStripStatusLabel StatusStatus;
        //private ToolStripMenuItem ContextMenu_ExpandAll;


        #endregion DEFINITIONS



        enum VIEW { TREE_VIEW = 0 };
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
            fm.ShowTime = 4000;
            // fm.ShowThread(str, 4000);
            fm.Launch(true /* parallel thread */);
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
        //private System.Windows.Forms.OpenFileDialog openFileDialog1;
        //private IContainer components;

        #endregion  ERROR_REPORTING









        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        void Main0()
        {
            Application.Run(new XmlTreeViewForm());
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
                // $$$$$$ listBoxXML.Visible = false;
                if (!ShowText)
                {
                    listBoxXML.Visible = false;
                    richTextBoxXML.Visible = false;
                }
                // Add images to the imageList for treeView
                resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlTreeViewForm));

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


        int numLoadsToPerform = 0, countLock = 0;

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
            countLock = 0;
            numLoadsToPerform = 0;
            // Initialization
            DocumentLoaded = false;
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
                if (listBoxXML != null)
                    listBoxXML.Items.Clear();
            }
            catch { }
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
            if (countLock == 0 && numLoadsToPerform == 0)
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
            if (countLock == 0 && numLoadsToPerform == 0)
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
                        using (TextReader tr = new StreamReader(XMLInputFile))
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
                    int linestart = richTextBoxXML.GetFirstCharIndexFromLine(ln - 1);
                    int linelength = richTextBoxXML.Lines[ln - 1].Length;
                    // richTextBoxXML.Select( linestart, linelength );
                    richTextBoxXML.SelectionStart = linestart;
                    richTextBoxXML.SelectionLength = linelength;
                    richTextBoxXML.ScrollToCaret();
                    //StatusError.Text = richTextBoxXML.Lines[ln - 1];
                }


                if (listBoxXML.Visible)
                    if (listBoxXML.Items.Count >= ln)
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
            int length = 0, textlength = 0;
            string text = node.Text;
            if (AttributeContainerLabel != null)
                length = AttributeContainerLabel.Length;
            if (text != null)
                textlength = text.Length;
            if (textlength >= length)
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
            string ret = "/";
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
                ret = "/" + node.FullPath.Replace('\\', '/').Replace("/" + AttributeContainerLabel, "");
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                if (TNode == null)
                    return null;
                else if (TNode.Level == 0)
                {
                    // Caes where the node does not have a parent:
                    if (TNode.Text.CompareTo(DocumentLabel) == 0 &&
                        getTreeNodeTypeName(TNode).CompareTo("Element") != 0)
                    {
                        ret = Doc;
                    }
                    else
                    {
                        // The tree node represents the root node of the document:
                        if (Doc.ChildNodes != null)
                        {
                            ret = null;
                            int i = 0;
                            while (ret == null && i < Doc.ChildNodes.Count)
                            {
                                XmlNode nd = Doc.ChildNodes[i];
                                if (nd.NodeType == XmlNodeType.Element)
                                {
                                    if (nd.Name.CompareTo(TNode.Text) == 0)
                                        ret = nd;
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
                    while (TCurrent != null
                         && getTreeNodeTypeName(TCurrent).CompareTo("Element") != 0
                         && getTreeNodeTypeName(TCurrent).CompareTo("Text") != 0
                         && getTreeNodeTypeName(TCurrent).CompareTo("Comment") != 0)
                    {
                        TCurrent = TCurrent.Parent;
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
                    if (TList.Count == 0)
                        throw new Exception("Can not determine the complete line of tree node ancestors.\n");
                    else
                    {
                        int indCur = TList.Count - 1;
                        // Find the equivalent root _gridCoordinates in the tree view and XML document:
                        TCurrent = TList[indCur];
                        XCurrent = null;
                        if (Doc != null)
                            if (Doc.HasChildNodes)
                            {
                                int childIndex = 0;
                                while (XCurrent == null && childIndex < Doc.ChildNodes.Count)
                                {
                                    XmlNode n = Doc.ChildNodes[childIndex];
                                    if (n.NodeType == XmlNodeType.Element)
                                    {
                                        if (n.Name.CompareTo(getTreeNodeName(TCurrent)) == 0)
                                            XCurrent = n;
                                        else
                                        {
                                            throw new Exception("Name of the root element (\"" + n.Name +
                                                "\") is different from the corresponding name in the tree view (\"" +
                                                getTreeNodeName(TCurrent) + "\").");
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
            if (level < numlevels || numlevels == 0)
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
            else if (node == null)
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
            NodeData nodedata = new NodeData();
            nodedata.LineNumber = 0;
            XmlTextReader textreader = reader as XmlTextReader;
            if (textreader != null)
                nodedata.LineNumber = textreader.LineNumber;
            nodedata.NodeType = reader.NodeType;
            nodedata.Depth = reader.Depth;
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
            string text = null;
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
            new FadingMessage(message, 3000, 0.5);
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
                }
                else
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
                if (PathTester != null)
                {
                    // One path tester is already open for this window, test if it is active and
                    // then ask if the user really wants to open another one:
                    try
                    {
                        PathTester.Activate();
                        UtilForms.BlinkControl(PathTester);
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
                        XMLTestPathForm frm = new XMLTestPathForm(path);
                        TreeNode node = treeViewXML.SelectedNode;
                        frm.Doc = Doc;
                        if (node != null)
                            frm.InitialPath = getTreeNodePath(node);
                        PathTester = frm; // register the path tester
                        frm.CloseDelegate = new XMLTestPathForm.MyDelegate(PathTester_Close);
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
                int numlevels = (int)LevelNumUpDown.Value;
                TreeNode tn = treeViewXML.SelectedNode;
                ExpandNodesExactly(tn, numlevels);
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

        private void richTextBoxXML_TextChanged(object sender, EventArgs e)
        {

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

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }


        /// <summary>State of this check box specified whether the text representation of XML is visible 
        /// or not (in this case, the appropriate panel collapses).</summary>
        private void chkShowText_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowText.Checked)
            {
                splitContainer2.Panel1Collapsed = false;
            } else
            {
                splitContainer2.Panel2Collapsed = false;
                splitContainer2.Panel1Collapsed = true;
                if (!chkShowList.Checked)
                {
                    chkShowList.Checked = true;
                }
            }
        }

        /// <summary>State of this check box specified whether the list representation of XML is visible 
        /// or not (in this case, the appropriate panel collapses).</summary>
        private void chkShowList_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowList.Checked)
            {
                splitContainer2.Panel2Collapsed = false;
            } else
            {
                splitContainer2.Panel1Collapsed = false;
                splitContainer2.Panel2Collapsed = true;
                if (!chkShowText.Checked)
                {
                    chkShowText.Checked = true;
                }
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
                int numlevels = (int)LevelNumUpDown.Value;
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
                int numlevels = (int)LevelNumUpDown.Value;
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
                    if (!isAttributeContainter(node))
                    {
                        string nodedescription = "";
                        if (data != null)
                            nodedescription += /* "Node type: "*/  data.NodeType.ToString() + "\r\n";
                        string path = getTreeNodePath(node);
                        if (path != null)
                            nodedescription += "At '" + path + "'\r\n";
                        nodedescription +=  /* "Node value (possibly shortened and decorateed):\r\d2'" */
                            "\r\n\"" + node.Text + "\"";
                        node.ToolTipText = nodedescription;
                        // new FadeMessage(nodedescription,2000);
                    }
                }
            }
            catch (Exception ex)
            {
                this.StatusError.Text = "Error (MouseHover): " + ex.Message;
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


        private void XmlTreeViewControl_DragDrop(object sender, DragEventArgs e)
        {
            List<string> paths = null;
            DataObject data = (DataObject)e.Data;
            if (data.ContainsFileDropList())
            {
                string[] rawFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (rawFiles != null)
                {
                    paths = new List<string>();
                    foreach (string path in rawFiles)
                    {
                        // paths.AddRange(File.ReadAllLines(path));
                        paths.Add(path);
                    }
                }
            }
            if (paths != null)
                if (paths.Count > 0)
                {
                    string droppedFilePath = paths[0];
                    if (!string.IsNullOrEmpty(droppedFilePath))
                    {
                        //txtFile.Text = droppedFilePath;
                        //txtFile_Validated(sender, e);
                        openFileDialog1.FileName = droppedFilePath;
                        openFileDialog1_FileOk(this, null);
                    }
                }
        }

        private void XmlTreeViewControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }



        public static void Example()
        {
            //  XML TreeView Tests:
            Application.Run(new XmlTreeViewForm());
            XmlTreeViewForm tv;
            Environment.Exit(0);
            tv = new XmlTreeViewForm();
            tv.ShowInTaskbar = true;     // Override the default setting
            tv.ShowDialog();

            XmlTreeViewForm tv1;
            tv1 = new XmlTreeViewForm();
            tv1.ShowDialog();

            tv.ShowDialog();
        } // Example()



        




        #region NOTES

        // THE FOLLOWING WAS REMOVED from the ..Designer.cs:



        // Doubly - commmented lines of the code below:

        //// this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        //this.ClientSize = new System.Drawing.Size(863, 720);
        //this.Controls.Add(this.DispPanel);
        //this.Controls.Add(this.BottomPnl);
        //this.Controls.Add(this.TopPnl);
        //// this.Icon = IG.Forms.Properties.Resources.ig;
        ////this.MainMenuStrip = this.MenuStrip;
        //this.Name = "XMLTreeView";
        //this.Padding = new System.Windows.Forms.Padding(10);
        ////this.ShowInTaskbar = false;
        //this.Text = "XML Viewer";
        //this.Load += new System.EventHandler(this.XMLTreeView_Load);
        ////this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XMLTreeView_Closing);
        //this.NodeContextMenu.ResumeLayout(false);
        //this.DispPanel.ResumeLayout(false);
        //this.splitContainer1.Panel1.ResumeLayout(false);
        //this.splitContainer1.Panel2.ResumeLayout(false);
        //this.splitContainer1.ResumeLayout(false);
        //this.TopPnl.ResumeLayout(false);
        //this.TopPnl.PerformLayout();
        //this.ControlPnl.ResumeLayout(false);
        //this.ControlPnl.PerformLayout();
        //((System.ComponentModel.ISupportInitialize)(this.LevelNumUpDown)).EndInit();
        //this.MenuStrip.ResumeLayout(false);
        //this.MenuStrip.PerformLayout();
        //this.BottomPnl.ResumeLayout(false);
        //this.BottomPnl.PerformLayout();
        //this.statusStrip1.ResumeLayout(false);
        //this.statusStrip1.PerformLayout();


        #endregion NOTES


    }
}
