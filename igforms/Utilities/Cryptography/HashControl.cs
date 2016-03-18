// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
// using System.Windows;
using System.Windows.Forms;
using System.IO;

using IG.Lib;
using IG.Crypto;

namespace IG.Forms
{
    public partial class HashControl : UserControl
    {
        public HashControl()
        {
            InitializeComponent();
            
            txtFile.ForeColor = FileOnForeColor;
            txtContents.BackColor = ContentsFileBackColor;
            txtContents.ForeColor = ContentsFileForeColor;
        }


        #region Data

        protected bool FileTextBoxInitialized = false;

        protected Color FileOnForeColor = Color.Black;

        protected Color FileOffForeColor = Color.DarkGray;

        protected bool ContentsTextBoxInitialized = false;

        protected Color ContentsFileBackColor = Color.LightGray;

        protected Color ContentsFileBinaryBackColor = Color.Wheat;

        protected Color ContentsTextBackColor = Color.White;

        protected Color ContentsFileForeColor = Color.Black;

        protected Color ContentsTextForeColor = Color.Blue;


        protected string _directoryPath;

        /// <summary>Directory where files are currently selected.</summary>
        public string DirectoryPath
        {
            get {
                return _directoryPath;
            }
            set {
                _directoryPath = value;
            }
        }

        protected string _filePath;

        /// <summary>Path to the file whose hashRet values are calculated.</summary>
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {

                if (string.IsNullOrEmpty(value))
                    _filePath = value;
                else
                {
                    if (value != _filePath)
                    {
                        if (File.Exists(value))
                        {
                            _filePath = value;
                            string dir = Path.GetDirectoryName(value);
                            if (!string.IsNullOrEmpty(dir))
                            {
                                if (Directory.Exists(dir))
                                    DirectoryPath = dir;
                            }
                        }
                    }
                }
            }
        }


        #endregion Data



        #region Operation


        #endregion Operation

        #region Operation.Gui


        public void ReportError(string errorMessage)
        {
            string text = "ERROR: " + Environment.NewLine + errorMessage;
            string caption = "Error";
            MessageBoxButtons button = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Error;
            DialogResult result = MessageBox.Show(text, caption, button, icon);
        }

        public void ReportWarning(string warningMessage)
        {
            string text = "WARNING: " + Environment.NewLine + warningMessage;
            string caption = "Warning";
            MessageBoxButtons button = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Warning;
            DialogResult result = MessageBox.Show(text, caption, button, icon);
        }

        public void ReportInfo(string message)
        {
            string text = message;
            string caption = "Info";
            MessageBoxButtons button = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            DialogResult result = MessageBox.Show(text, caption, button, icon);
        }

        /// <summary>Clears all text fields with calculated hashRet values.</summary>
        protected void ClearHashes()
        {
            txtMd5.Clear();
            txtSha1.Clear();
            txtSha1.Clear();
            txtSha256.Clear();
            txtSha512.Clear();
        }

        protected void CalculateHashes()
        {
            ClearHashes();

            txtVerify.Clear();

            if (rbFile.Checked)
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    ReportError("Can not calculate file hashes: file is not specified.");
                    return;
                } else if (!File.Exists(FilePath))
                {
                    ReportError("Can not calculate file hashes: file does not exist.");
                    return;
                }
            }
            if (chkMd5.Checked)
            {
                if (rbFile.Checked)
                    txtMd5.Text = UtilCrypto.GetFileHashMd5Hex(FilePath);
                else
                    txtMd5.Text = UtilCrypto.GetStringHashMd5Hex(txtContents.Text);
                if (txtMd5.Text != null && chkUpperCase.Checked)
                {
                    txtMd5.Text = txtMd5.Text.ToUpper();
                }
            }
            if (chkSha1.Checked)
            {
                if (rbFile.Checked)
                    txtSha1.Text = UtilCrypto.GetFileHashSha1Hex(FilePath);
                else
                    txtSha1.Text = UtilCrypto.GetStringHashSha1Hex(txtContents.Text);
                if (txtSha1.Text != null && chkUpperCase.Checked)
                {
                    txtSha1.Text = txtSha1.Text.ToUpper();
                }
            }
            if (chkSha256.Checked)
            {
                if (rbFile.Checked)
                    txtSha256.Text = UtilCrypto.GetFileHashSha256Hex(FilePath);
                else
                    txtSha256.Text = UtilCrypto.GetStringHashSha256Hex(txtContents.Text);
                if (txtSha256.Text != null && chkUpperCase.Checked)
                {
                    txtSha256.Text = txtSha256.Text.ToUpper();
                }
            }
            if (chkSha512.Checked)
            {
                if (rbFile.Checked)
                    txtSha512.Text = UtilCrypto.GetFileHashSha512Hex(FilePath);
                else
                    txtSha512.Text = UtilCrypto.GetStringHashSha512Hex(txtContents.Text);
                if (txtSha512.Text != null && chkUpperCase.Checked)
                {
                    txtSha512.Text = txtSha512.Text.ToUpper();
                }
            }
            if (chkGenerateFile.Checked)
            {
                SaveHashesToFile();
            }
        }

        protected void SaveHashesToFile()
        {
            if (rbFile.Checked)
            {
                string hashFilePath = FilePath + ConstCrypto.HashFileExtension;
                bool doSave = true;
                if (File.Exists(hashFilePath))
                {
                    doSave = false;
                    string text = "Warning: " + Environment.NewLine 
                        + "The file to save hash values into already exists." + Environment.NewLine
                        + "File path: " + hashFilePath + Environment.NewLine + Environment.NewLine
                        + "Do you want to overwrite the file?";
                    string caption = "Error";
                    MessageBoxButtons button = MessageBoxButtons.YesNoCancel;
                    MessageBoxIcon icon = MessageBoxIcon.Warning;
                    DialogResult result = MessageBox.Show(text, caption, button, icon);
                    if (result == DialogResult.Yes)
                    {
                        doSave = true;
                    }
                }
                bool saved = false;
                if (doSave)
                {
                    using (TextWriter writer = new StreamWriter(hashFilePath))
                    {
                        if (writer == null)
                        {
                            ReportError("Counld not open the hash values file for writing." 
                                + Environment.NewLine + Environment.NewLine
                                +  "File path: " + hashFilePath);
                        }
                        else
                        {
                            saved = true;
                            long fileLength = new FileInfo(FilePath).Length;
                            writer.WriteLine(Environment.NewLine
                                + "File:   " + Path.GetFileName(FilePath) + Environment.NewLine
                                + "Length: " + fileLength + Environment.NewLine + Environment.NewLine
                                + "Hash values: " + Environment.NewLine);
                            if (!string.IsNullOrEmpty(txtMd5.Text))
                                writer.WriteLine("  MD5:    " + Environment.NewLine + txtMd5.Text);
                            if (!string.IsNullOrEmpty(txtSha1.Text))
                                writer.WriteLine("  SHA1:    " + Environment.NewLine + txtSha1.Text);
                            if (!string.IsNullOrEmpty(txtSha256.Text))
                                writer.WriteLine("  SHA256:    " + Environment.NewLine + txtSha256.Text);
                            if (!string.IsNullOrEmpty(txtSha512.Text))
                                writer.WriteLine("  SHA512:    " + Environment.NewLine + txtSha512.Text);
                            writer.WriteLine("  ");
                        }
                    }
                    if (saved)
                    {
                        ReportInfo("Calculated hash values were saved to the following file: "
                            + Environment.NewLine + Environment.NewLine
                            + hashFilePath);
                    }
                }
            }
        }

        protected OpenFileDialog fileDialog;

        /// <summary>Browses for the file to be hashed.</summary>
        protected void BrowseFile()
        {
            if (fileDialog == null)
                fileDialog = new OpenFileDialog();
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            fileDialog.ReadOnlyChecked = true;
            if (!string.IsNullOrEmpty(DirectoryPath))
                fileDialog.InitialDirectory = DirectoryPath;
            else
                fileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            fileDialog.ShowDialog();

            string oldFilePath = txtFile.Text;
            string browsedFilePath = fileDialog.FileName;
            FilePath = browsedFilePath;
            txtFile.Text = FilePath;
            if (browsedFilePath != oldFilePath && rbFile.Checked)
            {
                GenerateFilePreview();
                CalculateHashes();
            }
        }

        protected int NumCharsFilePreview = 1000;

        /// <summary>Generates file preview in the text preview box if the <see cref="rbFile"/> is checked.</summary>
        protected void GenerateFilePreview()
        {
            if (rbFile.Checked)
            {
                txtContents.Clear();
                if (FilePath != null)
                {
                    if (File.Exists(FilePath))
                    {
                        using (TextReader reader = File.OpenText(FilePath))
                        {
                            char[] buffer = new char[NumCharsFilePreview];
                            reader.Read(buffer, 0, NumCharsFilePreview);
                            if (NumCharsFilePreview < new FileInfo(FilePath).Length)
                                txtContents.Text = new string(buffer) + Environment.NewLine + "... <<file contents are truncated here>>";
                            else
                                txtContents.Text = new string(buffer);
                            if (!UtilSystem.IsTextFile(FilePath))
                            {
                                txtContents.BackColor = ContentsFileBinaryBackColor;
                                txtContents.ForeColor = ContentsFileForeColor;
                            }
                            else
                            {
                                txtContents.BackColor = ContentsFileBackColor;
                                txtContents.ForeColor = ContentsFileForeColor;
                            }
                        }
                    }
                }
            }
        }

        protected void VerifyHash()
        {
            if (string.IsNullOrEmpty(txtVerify.Text))
            {
                ReportError("Can not verify the hash value: value is not specified." + Environment.NewLine + Environment.NewLine
                    + "Paste hash value into the appropriate text field!");
                return;
            }
            if (rbFile.Checked)
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    ReportError("Can not verify the file's hash value: file is not specified." + Environment.NewLine + Environment.NewLine
                        + "Select the file whose hash values are calculated and verified!");
                    return;
                }
                else if (!File.Exists(FilePath))
                {
                    ReportError("Can not verify the file's hash value: file does not exist." + Environment.NewLine
                        + "File path: " + FilePath + Environment.NewLine + Environment.NewLine
                        + "Select an existent file whose hash values are calculated and verified!");
                    return;
                } else
                {
                    HashType type = UtilCrypto.CheckFileHashSupportedTypesHex(FilePath, txtVerify.Text);
                    if (type == HashType.None)
                    {
                        ReportError("The verified hash is NOT a VALID HASH of any supported type for the specified file. " + Environment.NewLine
                            + "File path: " + FilePath + Environment.NewLine
                            + "Checked hash: " + txtVerify.Text + " ");
                    } else
                    {
                        ReportInfo("The verified value matches the " + type.ToString() + " hash of the specified file." + Environment.NewLine + Environment.NewLine
                            + "File path: " + FilePath + Environment.NewLine
                            + "Checked hash: " + txtVerify.Text + " ");
                    }
                }
            } else if (rbText.Checked)
            {
                HashType type = UtilCrypto.CheckStringHashSupportedTypesHex(txtContents.Text, txtVerify.Text);
                if (type == HashType.None)
                {
                    ReportError("The verified hash is NOT a VALID HASH of any supported type for the specified text. " 
                        + Environment.NewLine + Environment.NewLine
                        + "Verified text is contained in teh text box at the bottom. ");
                }
                else
                {
                    ReportInfo("The verified value matches the " + type.ToString() + " hash of the specified text."
                        + Environment.NewLine + Environment.NewLine
                        + "Verified text is contained in teh text box at the bottom. ");
                }
            }
        }

        #endregion Operation.Gui



        /// <summary>Requested hashs are generated and shown in the appropriate text boxes.</summary>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            CalculateHashes();
        }

        /// <summary>Hash values are cleared from the appropriate text fields.</summary>
        private void btnClearHashes_Click(object sender, EventArgs e)
        {
            ClearHashes();
        }


        /// <summary>Hash value is verified for the specified file or text.</summary>
        private void btnVerify_Click(object sender, EventArgs e)
        {
            VerifyHash();
        }

        private bool _skipFileValidationEvent = false;

        /// <summary>After file has been validated.</summary>
        private void txtFile_Validated(object sender, EventArgs e)
        {
            FileTextBoxInitialized = true;
            if (!_skipFileValidationEvent)
            {
                string oldFilePath = FilePath;
                if (txtFile.Text != FilePath)
                {
                    _skipFileValidationEvent = true;
                    try
                    {
                        FilePath = txtFile.Text;
                        if (rbFile.Checked)
                        {
                            GenerateFilePreview();
                            CalculateHashes();
                        }
                    }
                    finally
                    {
                        _skipFileValidationEvent = false;
                    }
                }
            }
        }



        /// <summary>Browses for the file whose hashRet values will be calculated.</summary>
        private void btnFileBrowse_Click(object sender, EventArgs e)
        {
            string oldFilePath = FilePath;
            BrowseFile();
            if (!string.IsNullOrEmpty(FilePath) && FilePath!=oldFilePath)
            {
                if (!rbFile.Checked)
                    rbFile.Checked = true;
            }
        }

        // When the hashRet algorithm type changes:
        
        private void chkMd5_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMd5.Checked)
                CalculateHashes();
            else
                txtMd5.Clear();
        }

        private void chkSha1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSha1.Checked)
                CalculateHashes();
            else
                txtSha1.Clear();
        }

        private void chkSha256_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSha256.Checked)
                CalculateHashes();
            else
                txtSha256.Clear();
        }

        private void chkSha512_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSha512.Checked)
                CalculateHashes();
            else
                txtSha512.Clear();
        }

        // Copying hashes to clipboard:

        private void btnMd5Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtMd5.Text);
        }

        private void btnSha1Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtSha1.Text);
        }

        private void btnSha256Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtSha256.Text);
        }

        private void bthSha512_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtSha512.Text);
        }

        // Pasting verified hashRet value from the clipboard:

        private void btnVerifyPaste_Click(object sender, EventArgs e)
        {
            txtVerify.Text = Clipboard.GetText();
        }


        protected int NumInsertTextInfo = 0;

        protected int MaxInsertTextInfo = 1;

        /// <summary>Handles things that need to be done when we switch between hashing the file or hashing 
        /// text from the textbox.</summary>
        private void rbFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFile.Checked)
            {
                txtContents.Clear();
                txtFile.ForeColor = FileOnForeColor;
                txtContents.BackColor = ContentsFileBackColor;
                txtContents.ForeColor = ContentsFileForeColor;
                grpTextPreview.Text = "File Contents Preview: ";
                txtContents.ReadOnly = true;
                if (!string.IsNullOrEmpty(FilePath))
                {
                    GenerateFilePreview();
                    CalculateHashes();
                }
            }
            else
            {
                txtFile.ForeColor = FileOffForeColor;
                txtContents.BackColor = ContentsTextBackColor;
                txtContents.ForeColor = ContentsTextForeColor;
                txtFile.ForeColor = FileOffForeColor; //  System.Drawing.Color.LightGray;
                grpTextPreview.Text = "Text whose hash value is calculated: ";
                txtContents.Text = null;
                ClearHashes();
                txtContents.ReadOnly = false;
                if (!TextBeingDropped && NumInsertTextInfo <MaxInsertTextInfo)
                {
                    ++NumInsertTextInfo;
                    string infoString = "Insert text whose hash values are generated into the larger text box!";
                    if (NumInsertTextInfo == MaxInsertTextInfo)
                        infoString += Environment.NewLine + Environment.NewLine + "This message will not be shown again during this session.";
                    ReportInfo(infoString);
                }
            }
        }


        /// <summary>Handles situation when one changes the setting whether to store file's hashRet values
        /// to a file. If the check box gets checked then hashes are immediately saved to a file.</summary>
        private void chkGenerateFile_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGenerateFile.Checked)
            {
                SaveHashesToFile();
            }
        }


        private void txtContents_Enter(object sender, EventArgs e)
        {
            if (!ContentsTextBoxInitialized)
            {
                // Clear auxiliary contents at the first suitable event:
                ContentsTextBoxInitialized = true;
                txtContents.Clear();
            }
        }

        private void txtContents_MouseEnter(object sender, EventArgs e)
        {
            // Clear auxiliary contents at the first suitable event:
            if (!ContentsTextBoxInitialized)
            {
                ContentsTextBoxInitialized = true;
                txtContents.Clear();
            }
        }

        private void txtContents_TextChanged(object sender, EventArgs e)
        {
            ContentsTextBoxInitialized = true;
        }


        /// <summary>After text is inserted, calculate appropriate hashes immediately.</summary>
        private void txtContents_Validated(object sender, EventArgs e)
        {
            ContentsTextBoxInitialized = true;
            if (rbText.Checked)
            {
                CalculateHashes();
            }
        }

        /// <summary>Recalculates hashes because the requested lower/upper case form has changed.</summary>
        private void chkUpperCase_CheckedChanged(object sender, EventArgs e)
        {
            CalculateHashes();
        }


        private void txtFile_Enter(object sender, EventArgs e)
        {
            if (!FileTextBoxInitialized)
            {
                FileTextBoxInitialized = true;
                txtFile.Clear();
            }
        }

        private void txtFile_MouseEnter(object sender, EventArgs e)
        {
            if (!FileTextBoxInitialized)
            {
                FileTextBoxInitialized = true;
                txtFile.Clear();
            }
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            FileTextBoxInitialized = true;
        }



        private void txtFile_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void txtFile_DragDrop(object sender, DragEventArgs e)
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
                        if (!rbFile.Checked)
                        {
                            rbFile.Checked = true;
                        }
                        txtFile.Text = droppedFilePath;
                        txtFile_Validated(sender, e);
                    }
                }
        }

        private void txtContents_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat)
                || e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
        }

        protected bool TextBeingDropped = false;

        private void txtContents_DragDrop(object sender, DragEventArgs e)
        {
            bool fileDropped = false;
            List<string> paths = null;
            DataObject data = (DataObject)e.Data;
            if (data.ContainsFileDropList())
            {
                // Check for 
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
                        fileDropped = true;
                        if (!rbFile.Checked)
                        {
                            rbFile.Checked = true;
                        }
                        txtFile.Text = paths[0];
                        txtFile_Validated(sender, e);
                    }
                }
            if (!fileDropped)
            {
                // Dropped data did not contain files, try to extract dropped text:
                if (e.Data.GetDataPresent(DataFormats.StringFormat))
                {
                    string droppedText = (string)e.Data.GetData(DataFormats.StringFormat);
                    if (!string.IsNullOrEmpty(droppedText))
                    {
                        try
                        {
                            TextBeingDropped = true;
                            txtContents.Text = droppedText;
                            if (!rbText.Checked)
                                rbText.Checked = true;  // switch from file to string mode
                            txtContents.Text = droppedText;
                            txtContents_Validated(sender, e);
                        }
                        catch { throw; }
                        finally { TextBeingDropped = false; }
                    }
                }
            }
        }


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


        private void txtFile_KeyDown(object sender, KeyEventArgs e)
        {
            Handle_CtrlA_CtrlBackspace(txtFile, sender, e);
        }


        private void txtContents_KeyDown(object sender, KeyEventArgs e)
        {
            Handle_CtrlA_CtrlBackspace(txtContents, sender, e);
        }


    }
}

