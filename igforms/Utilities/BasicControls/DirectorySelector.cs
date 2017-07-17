// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using IG.Lib;

namespace IG.Forms
{
    public partial class DirectorySelector : UserControl
    {
        public DirectorySelector()
        {
            InitializeComponent();

            if (UseRelativePaths)
                UseAbsolutePaths = false;
            else if (UseAbsolutePaths)
                UseRelativePaths = false;
            if (!AllowExistentDirectories && !AllowNonexistentDirectories)
                AllowExistentDirectories = true;

            NormalBackground = comboBoxFile.BackColor;

            // openDirectoryDialog1.CheckFileExists = !AllowNonexistentFiles;

            menuAllowNonexistent.Checked = AllowNonexistentDirectories;
            menuAllowExistent.Checked = AllowExistentDirectories;
            menuExpandEnvironment.Checked = AllowEnvironmentVariables;
            menuRelativePaths.Checked = UseRelativePaths;
            menuAbsolutePaths.Checked = UseAbsolutePaths;
            menuUseFileDialog.Checked = UseFileDialog;

            menuPathRepresentation.Text = DirectoryPath;
            menuInputTextBox.Text = OriginalDirectoryPath;

            UpdateErrorState();


        }




        #region Data 




        private bool _fileTextBoxInitialized = false;

        protected bool FileTextBoxInitialized { get { return _fileTextBoxInitialized; }
            set { _fileTextBoxInitialized = value; } }
        

        protected string _containingDirectoryPath;

        /// <summary>Directory where files are currently selected.</summary>
        public string InitialDirectoryPath
        {
            get
            {
                return _containingDirectoryPath;
            }
            protected set
            {
                if (value != _containingDirectoryPath && value != null)
                {
                    _containingDirectoryPath = value;
                }
            }
        }


        private bool _allowEnvironmentVariables = true;

        /// <summary>If true then environment variables can be used in paths.</summary>
        public bool AllowEnvironmentVariables
        {
            get { return _allowEnvironmentVariables; }
            set {
                if (value != _allowEnvironmentVariables)
                {
                    _allowEnvironmentVariables = value;
                    menuExpandEnvironment.Checked = AllowEnvironmentVariables;
                    UpdateErrorState();
                }
            }
        }

        private bool _allowExistentDirectories = true;

        /// <summary>If true then existent files are allowed to be selected. 
        /// <para>It might be impossible to insert such a path through the file dialog (launched by the "Browse" button), but one
        /// can insert it in the text box.</para></summary>
        public bool AllowExistentDirectories
        {
            get { return _allowExistentDirectories; }
            set
            {
                if (value != _allowExistentDirectories)
                {
                    _allowExistentDirectories = value;
                    if (!_allowExistentDirectories)
                    {
                        AllowNonexistentDirectories = true;
                    }
                    menuAllowExistent.Checked = AllowExistentDirectories;
                    UpdateErrorState();
                }
            }
        }

        private bool _allowNonexistentDirectories = true;

        /// <summary>If true then non-existend files are allowed to be selected. 
        /// <para>It might be impossible to insert such a path through the file dialog (launched by the "Browse" button), but one
        /// can insert it in the text box.</para></summary>
        public bool AllowNonexistentDirectories
        {
            get { return _allowNonexistentDirectories; }
            set {
                if (value != _allowNonexistentDirectories)
                {
                    _allowNonexistentDirectories = value;
                    if (!_allowNonexistentDirectories)
                    {
                        AllowExistentDirectories = true;
                    }
                    menuAllowNonexistent.Checked = AllowNonexistentDirectories;

                    // openDirectoryDialog1.CheckFileExists = !AllowNonexistentFiles;

                    UpdateErrorState();
                }
            }
        }

        private bool _useRelativePaths = false;

        /// <summary>If true then relative paths are used in representation of the selseted file path.
        /// <para>This property is mutually exclusive with <see cref="UseAbsolutePaths"/> (only one can be true,
        /// but both can be false).</para>
        /// </summary>
        public bool UseRelativePaths
        {
            get { return _useRelativePaths; }
            set {
                if (value != _useRelativePaths)
                {
                    _useRelativePaths = value;
                    if (_useRelativePaths)
                        _useAbsolutePaths = false;
                    UpdateErrorState();
                }
            }
        }

        private bool _useAbsolutePaths = false;

        /// <summary>If true then absolute paths are used in representation of the selseted file path.
        /// <para>This property is mutually exclusive with <see cref="UseRelativePaths"/> (only one can be true,
        /// but both can be false).</para>
        /// </summary>
        public bool UseAbsolutePaths
        {
            get { return _useAbsolutePaths; }
            set {
                if (value != _useAbsolutePaths)
                {
                    _useAbsolutePaths = value;
                    if (_useAbsolutePaths)
                        _useRelativePaths = false;
                    UpdateErrorState();
                }
            }
        }

        private bool _useFileDialog = true;

        /// <summary>If true then an <see cref="OpenFileDialog"/> is used to browse for directory, rather 
        /// than <see cref="Directorydialog"/> bool UseFileDialog
        bool UseFileDialog
        {
            get { return _useFileDialog; }
            set {
                if (value != _useFileDialog)
                {
                    _useFileDialog = value;
                    menuUseFileDialog.Checked = _useFileDialog;
                }
            }
        }


        protected string _directoryPath;

        /// <summary>Path to the file that is currently selected.
        /// <para>Getter returns the output representation of the path, with eventual environment variables
        /// expansion (dependent on <see cref="AllowEnvironmentVariables"/> properties) and in appropriate form
        /// (relative or absolute path, according to <see cref="UseRelativePaths"/> and <see cref="UseAbsolutePaths"/> properties).</para>
        /// <para>Original path ca be retrieved by the <see cref="OriginalDirectoryPath"/> property.</para></summary>
        public string DirectoryPath
        {
            get
            {
                return GetFilePathRepresentation(_directoryPath);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _directoryPath = value;
                else
                {
                    if (value != _directoryPath)
                    {
                        //if (AllowNonexistentFiles || File.Exists(value))
                        //{
                            _directoryPath = value;
                            this.comboBoxFile.Text = value;
                            if (!this.comboBoxFile.Items.Contains(OriginalDirectoryPath))
                            {
                                if (!string.IsNullOrEmpty(DirectoryPath))
                                {
                                    this.comboBoxFile.Items.Insert(0, OriginalDirectoryPath);
                                }
                            } else
                            {
                                // Move the selected file to the first position:
                                this.comboBoxFile.Items.Remove(OriginalDirectoryPath);
                                this.comboBoxFile.Items.Insert(0, OriginalDirectoryPath);
                            }
                        //}
                        try
                        {
                            bool directorySet = false;
                            string dir = Path.GetDirectoryName(value);
                            if (!string.IsNullOrEmpty(dir))
                            {
                                if (Directory.Exists(dir))
                                {
                                    InitialDirectoryPath = dir;
                                    directorySet = true;
                                }
                            }
                            if (!directorySet)
                            {
                                dir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(value));
                                if (!string.IsNullOrEmpty(dir))
                                {
                                    if (Directory.Exists(dir))
                                    {
                                        InitialDirectoryPath = dir;
                                        directorySet = true;
                                    }
                                }
                            }
                        }
                        catch { }
                        comboBoxFile.Text = OriginalDirectoryPath;
                        menuPathRepresentation.Text = DirectoryPath;
                        menuInputTextBox.Text = OriginalDirectoryPath;
                        UpdateErrorState();
                        OnDirectorySelected();
                    }
                }
            }
        }

        /// <summary>Gets the original file path representation (as set by user), without any post processing
        /// (like environment variables expansion or conversion to relative or absolute path).
        /// <para>Setter just calls the setter of the <see cref="DirectoryPath"/> property.</para></summary>
        public string OriginalDirectoryPath
        {
            get { return _directoryPath; }
            set { DirectoryPath = value; }
        }

        /// <summary>Returns the presentational form of the specified directory path, with eventual environment
        /// variables expanded and with conversion to the appropriate form (relative or absolute).</summary>
        /// <param name="directoryPath">Directory path whose presentational form is returned.</param>
        /// <remarks>Presentational form depends on the following properties:
        /// <pra>  - <see cref="AllowEnvironmentVariables"/>: If true then environment variable references are expanded.
        /// <para>In Unix-like systems, enviironment variables are referenced as $varName, and on Windows with %varName%.</para></pra>
        /// <para>  - <see cref="UseAbsolutePaths"/>: If true then paths are presented as absolute paths (independend of the current working directory).</para>
        /// <para>  - <see cref="UseRelativePaths"/>: If true then paths are represented relative to the current working directory.</para></remarks>
        public string GetFilePathRepresentation(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                return null;
            else
            {
                if (AllowEnvironmentVariables)
                    directoryPath = Environment.ExpandEnvironmentVariables(directoryPath);
                if (UseRelativePaths)
                    directoryPath = UtilSystem.GetRelativePath(Directory.GetCurrentDirectory(), directoryPath);
                if (UseAbsolutePaths)
                    directoryPath = UtilSystem.GetAbsolutePath(directoryPath);
                if (File.Exists(directoryPath))
                    return null;  // we can't allow existent file paths
                if (Directory.Exists(directoryPath))
                {
                    if (!AllowExistentDirectories)
                        return null;

                } else
                {
                    if (!AllowNonexistentDirectories)
                        return null;
                }
            }
            return directoryPath;
        }

        /// <summary>Returns true if the current selection is invalid, false if it is valid.
        /// <para>In the case of invalid selection, <see cref="OriginalDirectoryPath"/> may return a non-null path
        /// but <see cref="DirectoryPath"/> does not.</para></summary>
        public virtual bool IsErrorState
        {
            get
            {
                return (!string.IsNullOrEmpty(OriginalDirectoryPath) 
                    && string.IsNullOrEmpty(DirectoryPath));
            }
        }

        /// <summary>Updates controls according to error state.</summary>
        protected virtual void UpdateErrorState()
        {
            if (IsErrorState)
            {
                comboBoxFile.BackColor = ErrorBackground;
                btnFileBrowse.ForeColor = ErrorForeground;
            } else
            {
                comboBoxFile.BackColor = NormalBackground;
                btnFileBrowse.ForeColor = NormalForeground;
            }
        }

        private Color _errorBackground = Color.Orange;

        private Color _errorForeground = Color.Red;

        private Color _normalBackground = Color.White;

        private Color _normalforeground = Color.Black;


        /// <summary>Background color of selection's text in case of error.</summary>
        public virtual Color ErrorBackground
        { get { return _errorBackground; } set { _errorBackground = value; } }

        /// <summary>Foreground color of button in case of error.</summary>
        public virtual Color ErrorForeground
        { get { return _errorForeground; } set { _errorForeground = value; } }

        /// <summary>Background color of selection's text in case of NO error.</summary>
        public virtual Color NormalBackground
        { get { return _normalBackground; } set { _normalBackground = value; } }

        /// <summary>Foreground color of button in case of NO error.</summary>
        public virtual Color NormalForeground
        { get { return _normalforeground; } set { _normalforeground = value; } }


        #endregion Data


        #region Actions


        /// <summary>Browses for the file to be hashed.</summary>
        public void BrowseDirectory()
        {
            if (UseFileDialog)
            {
                // Use OpennFileDialog instead of FolderBrowserDialog:



                OpenFileDialog fileDialog = openFileDialog1;
                if (fileDialog == null)
                    fileDialog = new OpenFileDialog();


                // To allow directory selection in file dialog, we must set properties as follows:
                fileDialog.ValidateNames = false;
                fileDialog.CheckFileExists = false;
                if (AllowNonexistentDirectories)
                    fileDialog.CheckPathExists = false;
                else
                    fileDialog.CheckPathExists = true;


                // Default file name, this will be later stripped off:
                fileDialog.FileName = "Folder_Selection.";


                if (!string.IsNullOrEmpty(InitialDirectoryPath))
                    fileDialog.InitialDirectory = InitialDirectoryPath;
                else
                    fileDialog.InitialDirectory = Directory.GetCurrentDirectory();

                //if (!string.IsNullOrEmpty(FilePath))
                //    fileDialog.FileName = Path.GetFileName(FilePath);

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string browsedDirectoryPath = fileDialog.FileName;

                    browsedDirectoryPath = Path.GetDirectoryName(browsedDirectoryPath);


                    if (!AllowExistentDirectories && Directory.Exists(browsedDirectoryPath))
                    {
                        DialogResult result1 = MessageBox.Show("The selected directory already exists! "
                        + Environment.NewLine + Environment.NewLine + "Directory path: "
                        + Environment.NewLine + "  " + browsedDirectoryPath + Environment.NewLine + Environment.NewLine
                        + "Existent directories are not allowed." + Environment.NewLine + Environment.NewLine
                        + "Will you select a new directory? ",
                        "Directory exists",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result1 == DialogResult.Yes)
                            BrowseDirectory();  // browse directory again
                    }
                    else
                    {
                        DirectoryPath = browsedDirectoryPath;
                    }
                }




            }
            else
            {

                FolderBrowserDialog directoryDialog = this.openDirectoryDialog1;
                if (directoryDialog == null)
                    directoryDialog = new FolderBrowserDialog();


                //fileDialog.CheckFileExists = true;
                //fileDialog.CheckPathExists = true;
                //fileDialog.ReadOnlyChecked = true;

                directoryDialog.ShowNewFolderButton = true;

                if (!string.IsNullOrEmpty(DirectoryPath) && Directory.Exists(DirectoryPath))
                    directoryDialog.SelectedPath = DirectoryPath;
                else if (!string.IsNullOrEmpty(InitialDirectoryPath))
                    directoryDialog.SelectedPath = InitialDirectoryPath;
                else
                    directoryDialog.SelectedPath = Directory.GetCurrentDirectory();

                directoryDialog.ShowDialog();
                string browsedFilePath = directoryDialog.SelectedPath;

                //string oldFilePath = comboBoxFile.Text;
                DirectoryPath = browsedFilePath;

            }

        }




        public void ClearHistory()
        {
            comboBoxFile.Items.Clear();
        }

        bool _isBrowsable = true;

        /// <summary>Gets or sets the flag specifying whether one can browse for the file to be opened (e.g. by
        /// using the file selection dialog).
        /// <para>If the viewer does not at all have browser capability then setting to true should NOT throw an 
        /// exception (therefore, the user should verify by getter if setting actually worked).</para>
        /// <para>Setter can be used to disable browsing (e.g. for security, or when used as embedded control
        /// and browsing would be disturbing).</para></summary>
        public bool IsBrowsable
        {
            get { return _isBrowsable; }
            set {
                if (value != _isBrowsable)
                {
                    _isBrowsable = value;
                    btnFileBrowse.Visible = _isBrowsable;
                    btnFileBrowse.Enabled = _isBrowsable;
                    comboBoxFile.Enabled = _isBrowsable;
                }
            }
        }

        bool _isDragAndDrop = true;

        /// <summary>Whether the selector provides drag & drop capability.
        /// <para>Setter can be used to disable drag & drop (e.g. for security, or when used with an embedded control
        /// and drag & drop would be disturbing).</para></summary>
        public bool IsDragAndDrop
        {
            get { return _isDragAndDrop; }
            set
            {
                _isDragAndDrop = value;
            }
        }


#endregion Actions



#region Events


        /// <summary>Event that is fired whenever the selected directory changes.</summary>
        public event EventHandler DirectorySelected;

        /// <summary>Called whenever the selected directory changes.</summary>
        protected virtual void OnDirectorySelected()
        {
            if (DirectorySelected != null)
                DirectorySelected(this, new EventArgs());
        }

        /// <summary>Event that is fired whenever directory containing the selected file changes.</summary>
        public event EventHandler DirectoryChanged;

        /// <summary>Called whenever the directory containing the selected file changes.</summary>
        protected virtual void OnDirectoryChanged()
        {
            if (DirectoryChanged != null)
                DirectoryChanged(this, new EventArgs());
        }




#endregion Events


#region EventHandlers


        /// <summary>Browses for the file whose hash values will be calculated.</summary>
        private void btnFileBrowse_Click(object sender, EventArgs e)
        {
            string oldFilePath = DirectoryPath;
            BrowseDirectory();
            if (!string.IsNullOrEmpty(DirectoryPath) && DirectoryPath != oldFilePath)
            {

                //if (!rbFile.Checked)
                //    rbFile.Checked = true;
            }
        }




        //private void txtFile_DragEnter(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Copy;
        //}

        //private void txtFile_DragDrop(object sender, DragEventArgs e)
        //{
        //    List<string> paths = null;
        //    DataObject data = (DataObject)e.Data;
        //    if (data.ContainsFileDropList())
        //    {
        //        string[] rawFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
        //        if (rawFiles != null)
        //        {
        //            paths = new List<string>();
        //            foreach (string path in rawFiles)
        //            {
        //                // paths.AddRange(File.ReadAllLines(path));
        //                paths.Add(path);
        //            }
        //        }
        //    }
        //    if (paths != null)
        //        if (paths.Count > 0)
        //        {
        //            string droppedFilePath = paths[0];
        //            if (!string.IsNullOrEmpty(droppedFilePath))
        //            {
        //                //if (!rbFile.Checked)
        //                //{
        //                //    rbFile.Checked = true;
        //                //}
        //                txtFile.Text = droppedFilePath;
        //                txtFile_Validated(sender, e);
        //            }
        //        }
        //}


        //private void txtFile_Enter(object sender, EventArgs e)
        //{
        //    if (!FileTextBoxInitialized)
        //    {
        //        FileTextBoxInitialized = true;
        //        txtFile.Clear();
        //    }
        //}

        //private void txtFile_MouseEnter(object sender, EventArgs e)
        //{
        //    if (!FileTextBoxInitialized)
        //    {
        //        FileTextBoxInitialized = true;
        //        txtFile.Clear();
        //    }
        //}

        //private void txtFile_TextChanged(object sender, EventArgs e)
        //{
        //    FileTextBoxInitialized = true;
        //}





        private bool _skipFileValidationEvent = false;


        /// <summary>Handle the Ctrl-A (select all) and Ctrl-Backspace events for the specified text box.</summary>
        /// <param name="textBox">Text box for wich the event is handled.</param>
        protected void Handle_CtrlA_CtrlBackspace(TextBox textBox, object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.A)
            {
                textBox.SelectAll();
            }
            else if (e.Control & e.KeyCode == Keys.Back)
            {
                SendKeys.SendWait("^+{LEFT}{BACKSPACE}");
            }
        }


        //private void txtFile_KeyDown(object sender, KeyEventArgs e)
        //{
        //    Handle_CtrlA_CtrlBackspace(txtFile, sender, e);
        //}



#endregion EventHandlers

        private void txtFile_Click(object sender, EventArgs e)
        {

        }

        
        private void comboBoxFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFile.SelectedIndex < comboBoxFile.Items.Count)
            {
                DirectoryPath = (string) comboBoxFile.Items[comboBoxFile.SelectedIndex];
            }
        }

        private void comboBoxFile_Enter(object sender, EventArgs e)
        {
            if (!FileTextBoxInitialized)
            {
                FileTextBoxInitialized = true;
                comboBoxFile.Text = null; //.Clear();
            }

        }

        private void comboBoxFile_Validated(object sender, EventArgs e)
        {
            FileTextBoxInitialized = true;
            if (!_skipFileValidationEvent)
            {
                string oldFilePath = DirectoryPath;
                if (comboBoxFile.Text != OriginalDirectoryPath)
                {
                    _skipFileValidationEvent = true;
                    try
                    {
                        DirectoryPath = comboBoxFile.Text;
                    }
                    finally
                    {
                        _skipFileValidationEvent = false;
                    }
                }
            }
        }

        private void comboBoxFile_KeyUp(object sender, KeyEventArgs e)
        {
            if (!FileTextBoxInitialized)
            {
                FileTextBoxInitialized = true;
                comboBoxFile.Text = null;
            }
            if (e.KeyCode == Keys.Enter)
            {
                comboBoxFile_Validated(sender, e);
                comboBoxFile.Focus();
            }
        }

        private void comboBoxFile_MouseEnter(object sender, EventArgs e)
        {
            if (!FileTextBoxInitialized)
            {
                FileTextBoxInitialized = true;
                comboBoxFile.Text=null;
            }
        }

        private void comboBoxFile_TextChanged(object sender, EventArgs e)
        {
            FileTextBoxInitialized = true;
        }



        private void FileSelector_DragEnter(object sender, DragEventArgs e)
        {
            if (IsDragAndDrop)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void FileSelector_DragDrop(object sender, DragEventArgs e)
        {
            if (IsDragAndDrop)
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
                            comboBoxFile.Text = droppedFilePath;
                            comboBoxFile_Validated(sender, e);
                        }
                    }
            }
        }

        private void menuClearHistory_Click(object sender, EventArgs e)
        {
            this.ClearHistory();
        }


        private void menuAllowNonexistent_CheckedChanged(object sender, EventArgs e)
        {
            AllowNonexistentDirectories = menuAllowNonexistent.Checked;
        }

        private void menuAllowExistent_CheckedChanged(object sender, EventArgs e)
        {
            AllowExistentDirectories = menuAllowExistent.Checked;
        }

        private void menuExpandEnvironment_CheckedChanged(object sender, EventArgs e)
        {
            AllowEnvironmentVariables = menuExpandEnvironment.Checked;
        }

        private void menuAbsolutePaths_CheckedChanged(object sender, EventArgs e)
        {
            UseAbsolutePaths = menuAbsolutePaths.Checked;
        }

        private void menuRelativePaths_CheckedChanged(object sender, EventArgs e)
        {
            UseRelativePaths = menuRelativePaths.Checked;
        }


        private void menuPathRepresentation_Click(object sender, EventArgs e)
        {
            string message = "";
            if (String.IsNullOrEmpty(DirectoryPath))
                message = Environment.NewLine + "File has not yet been selected." + Environment.NewLine;
            else
            {
                message = Environment.NewLine + "The following file has been selected: " + Environment.NewLine
                    + Environment.NewLine + "\"" + DirectoryPath + "\"" + Environment.NewLine + Environment.NewLine;
                if (DirectoryPath != OriginalDirectoryPath)
                    message += Environment.NewLine + "Original file path (as entered by the user: )" + Environment.NewLine
                        + Environment.NewLine + "\"" + OriginalDirectoryPath + "\"" + Environment.NewLine + Environment.NewLine;
                if (!string.IsNullOrEmpty(DirectoryPath))
                    message += Environment.NewLine + "Initial search directory path: " + Environment.NewLine
                        + Environment.NewLine + "\"" + InitialDirectoryPath + "\"" + Environment.NewLine + Environment.NewLine;
            }
            message += Environment.NewLine + "<< Ctrl-Right Click to close this message, Drag to move. >>";
            FadingMessage fm = new FadingMessage("Selected Path", message, 4000, 0.3, true /* launchImmediately */);
            fm.TopMost = false;
        }

        private void menuInputTextBox_Click(object sender, EventArgs e)
        {
            // menuInputTextBox.SelectAll();
        }

        private void menuInputTextBox_Validated(object sender, EventArgs e)
        {
            DirectoryPath = menuInputTextBox.Text;
        }

        private void menuInputTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                menuInputTextBox_Validated(sender, e);
                comboBoxFile.Focus();
            }
        }

        /// <summary>Reopens the context menu at its previous position.</summary>
        private void menuAllowNonexistent_Click(object sender, EventArgs e)
        {
            //contextMenuMain.Show(Cursor.Position);
            contextMenuMain.Show(_lasContectMenuPosition);
        }

        



        /// <summary>Reopens the context menu at its previous position.</summary>
        private void menuAllowExistent_Click(object sender, EventArgs e)
        {
            contextMenuMain.Show(_lasContectMenuPosition);
        }

        /// <summary>Reopens the context menu at its previous position.</summary>
        private void menuExpandEnvironment_Click(object sender, EventArgs e)
        {
            contextMenuMain.Show(_lasContectMenuPosition);
        }


        private void menuUseFileDialog_Click(object sender, EventArgs e)
        {
            contextMenuMain.Show(_lasContectMenuPosition);
        }

        private void menuUseFileDialog_CheckedChanged(object sender, EventArgs e)
        {
            UseFileDialog = menuUseFileDialog.Checked;
        }


        /// <summary>Reopens the context menu at its previous position.</summary>
        private void menuAbsolutePaths_Click(object sender, EventArgs e)
        {
            contextMenuMain.Show(_lasContectMenuPosition);
        }

        /// <summary>Reopens the context menu at its previous position.</summary>
        private void menuRelativePaths_Click(object sender, EventArgs e)
        {
            contextMenuMain.Show(_lasContectMenuPosition);
        }


        Point _lasContectMenuPosition = new Point(0, 0);  // = new Point(0, 0);

        /// <summary>Stores the position where the menu item has last opened.</summary>
        private void contextMenuMain_Opened(object sender, EventArgs e)
        {
            _lasContectMenuPosition = contextMenuMain.PointToScreen(new Point(0,0));
                // menuClearHistory.PointToScreen(new Point(0, 0));
        }


    } // class
}
