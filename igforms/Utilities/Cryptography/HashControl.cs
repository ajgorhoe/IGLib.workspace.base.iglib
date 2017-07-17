// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

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
using System.Threading;


namespace IG.Forms
{

    /// <summary>A control used to calculate various checksums of selected files or inserted text.</summary>
    /// <remarks><para>See:</para>
    /// <para>http://www2.arnes.si/~ljc3m2/igor/software/IGLibShellApp/HashForm.html#hashform </para></remarks>
    /// $A Igor Aug 08;
    public partial class HashControl : UserControl
    {
        public HashControl()
        {
            InitializeComponent();


            txtFile.ForeColor = FileOnForeColor;
            txtContents.BackColor = ContentsFileBackColor;
            txtContents.ForeColor = ContentsFileForeColor;

            backgroundWorker1.WorkerSupportsCancellation = true;

            indicatorLight1.SetOff();
            indicatorLight1.BlinkIntervalMilliSeconds = 100;
            btnCancelCalculation.Visible = false;

            txtContents.MaxLength = 0;
            CalculateAllCanceled = false;
            IsGroupCalculationPerformed = false;
            if (rbFile.Checked)
            {
                chkCalculateWhenTyping.Visible = false;
            } else
            {
                chkCalculateWhenTyping.Visible = true;
            }

        }



        /// <summary>Used to carry arguments and results to background jobs of calculating hash values.</summary>
        public class HashJobArgumentsAndResults
        {
            protected HashJobArgumentsAndResults() { }

            public HashJobArgumentsAndResults(HashType hashType, bool updateUI = true)
            {
                this.HashType = hashType;
                this.UpdateUI = updateUI;
                this.CalculatedHash = null;
                this.HashedFilePath = null;
                this.HashedText = null;
            }

            public HashType HashType { get; set; }

            public long HashedLength { get; set; }

            protected string _hashedFilePath = null;

            public string HashedFilePath
            {
                get { return _hashedFilePath; }
                set
                {
                    _hashedFilePath = value;
                    if (!string.IsNullOrEmpty(value) && HashedText == null)
                    {
                        if (File.Exists(value))
                            HashedLength = new FileInfo(value).Length;
                        else
                            HashedLength = 0;
                    }
                }
            }

            protected string _hashedText = null;

            public string HashedText {
                get { return _hashedText; }
                set
                {
                    _hashedText = value;
                    if (value != null)
                    {
                        HashedLength = value.Length;
                    }
                }
            }

            public bool UpdateUI { get; set; }

            public string CalculatedHash { get; set; }

        }  // class HashJobArgumentsAndResults


        protected int _fadingMessageShowTimeMs = 3000;

        protected int FadingMessageShowTimeMs
        {
            get { return _fadingMessageShowTimeMs; }
            set { _fadingMessageShowTimeMs = value; }
        }

        Color _fadingMessageInfoColor = FadingMessage.DefaultBackGroundColor;

        Color _fadingMessageWarningColor = Color.Orange;

        Color _fadingMessageErrorColor = Color.Red;

        protected void SetGeneralProperties(FadingMessage fmsg)
        {
            fmsg.BackGroundColor = FadingMessage.DefaultBackGroundColor;
            fmsg.LaunchedInParallelThread = false;
            fmsg.IsTopMostWindow = false;
            fmsg.LaunchedAtMouseCursor = true;
        }

        public void FadingMessageInfo(string message, int showTimeMs = 0)
        {
            if (UtilSystem.IsWindowsOs)
            {
                if (showTimeMs <= 0)
                    showTimeMs = FadingMessageShowTimeMs;
                FadingMessage msg = new FadingMessage("Info:", message, showTimeMs);
                SetGeneralProperties(msg);
                msg.BackGroundColor = _fadingMessageInfoColor;
            } else
            {

            }
        }

        public void FadingMessageWarning(string message, int showTimeMs = 0)
        {
            if (UtilSystem.IsWindowsOs)
            {
                if (showTimeMs <= 0)
                    showTimeMs = FadingMessageShowTimeMs;
                FadingMessage msg = new FadingMessage("Warning:", message, showTimeMs);
                SetGeneralProperties(msg);
                msg.BackGroundColor = _fadingMessageWarningColor;
            }
            else
            {

            }
        }

        public void FadingMessageError(string message, int showTimeMs = 0)
        {
            if (UtilSystem.IsWindowsOs)
            {
                if (showTimeMs <= 0)
                    showTimeMs = FadingMessageShowTimeMs;
                FadingMessage msg = new FadingMessage("Error:", message, showTimeMs);
                SetGeneralProperties(msg);
                msg.BackGroundColor = _fadingMessageErrorColor;
            } else
            {

            }
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

        /// <summary>Path to the file whose hash values are calculated.</summary>
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

        /// <summary>Clears all text fields with calculated hash values.</summary>
        protected void ClearHashes()
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.Abort();
            }
            RecalculateHashes = false;
            Application.DoEvents();

            indicatorLight1.SetOff();
            txtMd5.Clear();
            txtSha1.Clear();
            txtSha1.Clear();
            txtSha256.Clear();
            txtSha512.Clear();
        }

        protected void DisableActionButtons()
        {
            btnGenerate.Enabled = false;
            btnVerify.Enabled = false;
        }

        protected void EnableActionButtons()
        {
            btnGenerate.Enabled = true;
            btnVerify.Enabled = true;
        }



        /// <summary>Calculates (in background) the required hash (specified in Argument property event arguments) as <see cref="HashJobArgumentsAndResults"/>.
        /// </summary>
        /// <param name="sender">Originaror of event that runs this event handler.</param>
        /// <param name="e">Event arguments, contain the necessary information about how to calculate the hash.</param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e == null)
                throw new ArgumentException("Parameters of the hash calculation job were not provided.");
            HashJobArgumentsAndResults arg = e.Argument as HashJobArgumentsAndResults;
            LastArguments = arg;
            if (arg == null)
                throw new ArgumentException("Background calculation of hash value: job arguments not specified.");
            else
            {
                e.Result = arg;
                arg.CalculatedHash = null;
                switch (arg.HashType)
                {
                    case HashType.MD5:
                        {
                            if (arg.HashedFilePath != null)
                                arg.CalculatedHash = UtilCrypto.GetFileHashMd5Hex(FilePath);
                            else if (arg.HashedText != null)
                                arg.CalculatedHash = UtilCrypto.GetStringHashMd5Hex(arg.HashedText);
                            break;
                        }
                    case HashType.SHA1:
                        {
                            if (arg.HashedFilePath != null)
                                arg.CalculatedHash = UtilCrypto.GetFileHashSha1Hex(FilePath);
                            else if (arg.HashedText != null)
                                arg.CalculatedHash = UtilCrypto.GetStringHashSha1Hex(arg.HashedText);
                            break;
                        }
                    case HashType.SHA256:
                        {
                            if (arg.HashedFilePath != null)
                                arg.CalculatedHash = UtilCrypto.GetFileHashSha256Hex(FilePath);
                            else if (arg.HashedText != null)
                                arg.CalculatedHash = UtilCrypto.GetStringHashSha256Hex(arg.HashedText);
                            break;
                        }
                    case HashType.SHA512:
                        {
                            if (arg.HashedFilePath != null)
                                arg.CalculatedHash = UtilCrypto.GetFileHashSha512Hex(FilePath);
                            else if (arg.HashedText != null)
                                arg.CalculatedHash = UtilCrypto.GetStringHashSha512Hex(arg.HashedText);
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException("Calculation method is not defined for the following hash type: " + arg.HashType.ToString());
                        }
                }
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentException("Results of the hash calculation job were not provided.");
            if (!e.Cancelled)
            {
                HashJobArgumentsAndResults res = e.Result as HashJobArgumentsAndResults;
                LastResults = res;
                if (res != null)
                {
                    if (res.UpdateUI)
                    {
                        switch (res.HashType)
                        {
                            case HashType.MD5:
                                {
                                    if (res.CalculatedHash != null)
                                    {
                                        if (chkUpperCase.Checked)
                                        {
                                            txtMd5.Text = res.CalculatedHash.ToUpper();
                                        }
                                        else
                                        {
                                            txtMd5.Text = res.CalculatedHash;
                                        }
                                    }
                                    break;
                                }
                            case HashType.SHA1:
                                {
                                    if (res.CalculatedHash != null)
                                    {
                                        if (chkUpperCase.Checked)
                                        {
                                            txtSha1.Text = res.CalculatedHash.ToUpper();
                                        }
                                        else
                                        {
                                            txtSha1.Text = res.CalculatedHash;
                                        }
                                    }
                                    break;
                                }
                            case HashType.SHA256:
                                {
                                    if (res.CalculatedHash != null)
                                    {
                                        if (chkUpperCase.Checked)
                                        {
                                            txtSha256.Text = res.CalculatedHash.ToUpper();
                                        }
                                        else
                                        {
                                            txtSha256.Text = res.CalculatedHash;
                                        }
                                    }
                                    break;
                                }
                            case HashType.SHA512:
                                {
                                    if (res.CalculatedHash != null)
                                    {
                                        if (chkUpperCase.Checked)
                                        {
                                            txtSha512.Text = res.CalculatedHash.ToUpper();
                                        }
                                        else
                                        {
                                            txtSha512.Text = res.CalculatedHash;
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            EnableActionButtons();
            btnCancelCalculation.Visible = false;
            if (IsAllHashesCalculated)
            {
                indicatorLight1.SetOk();
                indicatorLight1.BlinkSpecial(Color.Yellow, 1);
            }
            else
            {
                indicatorLight1.SetOff();
                indicatorLight1.BlinkSpecial(Color.Orange, 1);
            }

        }


        protected void CalculateHashAsync(HashType hashType)
        {
            if (hashType == HashType.None)
            {
                ReportError("Can not calculate this the following type of hash: " + hashType.ToString());
            } else
            {
                HashJobArgumentsAndResults args = new HashJobArgumentsAndResults(hashType, true);
                if (rbFile.Checked)
                {
                    if (string.IsNullOrEmpty(FilePath))
                    {
                        ReportError("Can not calculate file hashes: file is not specified.");
                        CalculateAllCanceled = true;
                        indicatorLight1.SetError();
                        return;
                    }
                    else if (!File.Exists(FilePath))
                    {
                        ReportError("Can not calculate file hashes: file does not exist.");
                        CalculateAllCanceled = true;
                        indicatorLight1.SetError();
                        return;
                    } else if (!UtilSystem.IsReadableFile(FilePath))
                    {
                        ReportError("Can not calculate file hashes: file ia not readable.");
                        CalculateAllCanceled = true;
                        indicatorLight1.SetError();
                        return;
                    }
                    args.HashedFilePath = FilePath;
                } else
                {
                    args.HashedText = txtContents.Text;
                }
                args.HashedLength = GetFileOrTextLength();
                args.UpdateUI = true;
                DisableActionButtons();
                indicatorLight1.SetBusy();
                btnCancelCalculation.Visible = true;
                LoopWhileBusy();  // Just make sure that other jobs have finished
                backgroundWorker1.RunWorkerAsync(args);

                Application.DoEvents();

                //switch (hashType)
                //{
                //    case HashType.MD5:
                //        {
                //            txtMd5.Clear();
                //            string hash = null;
                //            if (rbFile.Checked)
                //                hash = UtilCrypto.GetFileHashMd5Hex(FilePath);
                //            else
                //                hash = UtilCrypto.GetStringHashMd5Hex(txtContents.Text);
                //            if (hash != null)
                //            {
                //                if (chkUpperCase.Checked)
                //                {
                //                    txtMd5.Text = hash.ToUpper();
                //                }
                //                else
                //                {
                //                    txtMd5.Text = hash;
                //                }
                //            }
                //            break;
                //        }
                //    case HashType.SHA1:
                //        {
                //            txtSha1.Clear();
                //            string hash = null;
                //            if (rbFile.Checked)
                //                hash = UtilCrypto.GetFileHashSha1Hex(FilePath);
                //            else
                //                hash = UtilCrypto.GetStringHashSha1Hex(txtContents.Text);
                //            if (hash != null)
                //            {
                //                if (chkUpperCase.Checked)
                //                {
                //                    txtSha1.Text = hash.ToUpper();
                //                }
                //                else
                //                {
                //                    txtSha1.Text = hash;
                //                }
                //            }
                //            break;
                //        }
                //    case HashType.SHA256:
                //        {
                //            txtSha256.Clear();
                //            string hash = null;
                //            if (rbFile.Checked)
                //                hash = UtilCrypto.GetFileHashSha256Hex(FilePath);
                //            else
                //                hash = UtilCrypto.GetStringHashSha256Hex(txtContents.Text);
                //            if (hash != null)
                //            {
                //                if (chkUpperCase.Checked)
                //                {
                //                    txtSha256.Text = hash.ToUpper();
                //                }
                //                else
                //                {
                //                    txtSha256.Text = hash;
                //                }
                //            }
                //            break;
                //        }
                //    case HashType.SHA512:
                //        {
                //            txtSha512.Clear();
                //            string hash = null;
                //            if (rbFile.Checked)
                //                hash = UtilCrypto.GetFileHashSha512Hex(FilePath);
                //            else
                //                hash = UtilCrypto.GetStringHashSha512Hex(txtContents.Text);
                //            if (hash != null)
                //            {
                //                if (chkUpperCase.Checked)
                //                {
                //                    txtSha512.Text = hash.ToUpper();
                //                }
                //                else
                //                {
                //                    txtSha512.Text = hash;
                //                }
                //            }
                //            break;
                //        }

                //}
            }
        }

        protected void LoopWhileBusy()
        {
            while (backgroundWorker1.IsBusy)
            {
                Application.DoEvents();
            }
        }

        protected bool IsAllHashesCalculated
        {
            get
            {
                bool allCalculated = true;
                int numChecked = 0;
                // int (chkMd5.Checked)
                if (chkMd5.Checked)
                {
                    ++numChecked;
                    if (string.IsNullOrEmpty(txtMd5.Text))
                        allCalculated = false;
                }
                if (chkSha1.Checked)
                {
                    ++numChecked;
                    if (string.IsNullOrEmpty(txtSha1.Text))
                        allCalculated = false;
                }
                if (chkSha256.Checked)
                {
                    ++numChecked;
                    if (string.IsNullOrEmpty(txtSha256.Text))
                        allCalculated = false;
                }
                if (chkSha512.Checked)
                {
                    ++numChecked;
                    if (string.IsNullOrEmpty(txtSha512.Text))
                        allCalculated = false;
                }
                return (allCalculated && numChecked > 0);

                //return (
                //    (!chkMd5.Checked || !string.IsNullOrEmpty(txtMd5.Text)) &&
                //    (!chkSha1.Checked || !string.IsNullOrEmpty(txtSha1.Text)) &&
                //    (!chkSha256.Checked || !string.IsNullOrEmpty(txtSha256.Text)) &&
                //    (!chkSha512.Checked || !string.IsNullOrEmpty(txtSha512.Text))
                //    );
            }
        }

        protected long GetFileOrTextLength()
        {
            long ret = 0;
            if (rbFile.Checked)
            {
                if (!string.IsNullOrEmpty(FilePath))
                    if (File.Exists(FilePath))
                        ret = new System.IO.FileInfo(FilePath).Length;
            } else
            {
                if (!string.IsNullOrEmpty(txtContents.Text))
                    ret = txtContents.Text.Length;
            }
            return ret;
        }

        protected void CancelHashCalculations()
        {
            CalculateAllCanceled = true;
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.Abort();
                FadingMessageWarning("Calculation aborted.");
            } else
            {
                FadingMessageInfo("Not canceled: " + Environment.NewLine + "Calculations were already finished.", FadingMessageShowTimeMs / 2);
            }
        }

        protected bool CalculateAllCanceled { get; set; }

        protected bool IsGroupCalculationPerformed { get; set; }

        protected bool RecalculateHashes { get; set; }

        protected HashJobArgumentsAndResults LastArguments { get; set; }

        protected HashJobArgumentsAndResults LastResults { get; set; }


        /// <summary>Re-calculates all hashes that are checked for calculation.</summary>
        protected void CalculateHashes()
        {
            if (backgroundWorker1.IsBusy)
            {
                if (IsGroupCalculationPerformed)
                {
                    CancelHashCalculations();
                    Application.DoEvents();
                }
            }
            try
            {
                int numCalculationsRun = 0;
                IsGroupCalculationPerformed = true;
                RecalculateHashes = false;
                CalculateAllCanceled = false;
                ClearHashes();

                txtVerify.Clear();

                //if (rbFile.Checked)
                //{
                //    if (string.IsNullOrEmpty(FilePath))
                //    {
                //        ReportError("Can not calculate file hashes: file is not specified.");
                //        return;
                //    } else if (!File.Exists(FilePath))
                //    {
                //        ReportError("Can not calculate file hashes: file does not exist.");
                //        return;
                //    } else if (!UtilSystem.IsReadableFile(FilePath))
                //    {
                //        ReportError("Can not calculate file hashes: file is not readable.");
                //        return;
                //    }
                //}
                LoopWhileBusy();
                if (chkMd5.Checked && !CalculateAllCanceled)
                {
                    CalculateHashAsync(HashType.MD5);
                    ++numCalculationsRun;
                    //if (rbFile.Checked)
                    //    txtMd5.Text = UtilCrypto.GetFileHashMd5Hex(FilePath);
                    //else
                    //    txtMd5.Text = UtilCrypto.GetStringHashMd5Hex(txtContents.Text);
                    //if (txtMd5.Text != null && chkUpperCase.Checked)
                    //{
                    //    txtMd5.Text = txtMd5.Text.ToUpper();
                    //}
                }
                LoopWhileBusy();
                if (chkSha1.Checked && !CalculateAllCanceled)
                {
                    CalculateHashAsync(HashType.SHA1);
                    ++numCalculationsRun;
                    //if (rbFile.Checked)
                    //    txtSha1.Text = UtilCrypto.GetFileHashSha1Hex(FilePath);
                    //else
                    //    txtSha1.Text = UtilCrypto.GetStringHashSha1Hex(txtContents.Text);
                    //if (txtSha1.Text != null && chkUpperCase.Checked)
                    //{
                    //    txtSha1.Text = txtSha1.Text.ToUpper();
                    //}
                }
                LoopWhileBusy();
                if (chkSha256.Checked && !CalculateAllCanceled)
                {
                    CalculateHashAsync(HashType.SHA256);
                    ++numCalculationsRun;
                    //if (rbFile.Checked)
                    //    txtSha256.Text = UtilCrypto.GetFileHashSha256Hex(FilePath);
                    //else
                    //    txtSha256.Text = UtilCrypto.GetStringHashSha256Hex(txtContents.Text);
                    //if (txtSha256.Text != null && chkUpperCase.Checked)
                    //{
                    //    txtSha256.Text = txtSha256.Text.ToUpper();
                    //}
                }
                LoopWhileBusy();
                if (chkSha512.Checked && !CalculateAllCanceled)
                {
                    CalculateHashAsync(HashType.SHA512);
                    ++numCalculationsRun;
                    //if (rbFile.Checked)
                    //    txtSha512.Text = UtilCrypto.GetFileHashSha512Hex(FilePath);
                    //else
                    //    txtSha512.Text = UtilCrypto.GetStringHashSha512Hex(txtContents.Text);
                    //if (txtSha512.Text != null && chkUpperCase.Checked)
                    //{
                    //    txtSha512.Text = txtSha512.Text.ToUpper();
                    //}
                }

                if (chkGenerateFile.Checked)
                {
                    SaveHashesToFile();
                }
                Application.DoEvents();
                if (numCalculationsRun > 0 && LastResults != null && object.ReferenceEquals(LastResults, LastArguments))
                {
                    long currentLength = GetFileOrTextLength();
                    if (currentLength != LastResults.HashedLength)
                    {
                        indicatorLight1.SetError();
                        if (!(rbText.Checked && chkCalculateWhenTyping.Checked))
                        {
                            FadingMessageError("Current length of calculated data (" + currentLength + ") is different from the length " + Environment.NewLine
                                + "when calculation started (" + LastResults.HashedLength + ").");
                        }
                        Application.DoEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                indicatorLight1.SetError();
                ReportError("Error occurred when calculating hash values (checksums): " + Environment.NewLine
                    + ex.Message);
            }
            finally
            {
                EnableActionButtons();
                if (CalculateAllCanceled)
                {
                    CalculateAllCanceled = false;
                    // indicatorLight1.SetOff();
                    indicatorLight1.BlinkError(4);
                }
                IsGroupCalculationPerformed = false;
                if (RecalculateHashes)
                {
                    // This occurs e.g. when hashes are calculated as one types and there were additional key strokes after calculation.
                    Application.DoEvents();
                    //Thread.Sleep(20);
                    //Application.DoEvents();
                    if (!IsGroupCalculationPerformed)
                    {
                        RecalculateHashes = false;
                        CalculateHashes();
                    }
                }

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
                                + "File path: " + hashFilePath);
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

            string providedHash = txtVerify.Text;
            if (string.IsNullOrEmpty(txtVerify.Text))
            {
                ReportError("Can not verify the hash value: value is not specified." + Environment.NewLine + Environment.NewLine
                    + "Paste hash value into the appropriate text field!");
                return;
            }
            else
            {
                try {

                    providedHash = providedHash.Trim();
                    HashType machedType = HashType.None;
                    // First, check hashes that were already calculated:
                    if (!string.IsNullOrEmpty(txtMd5.Text))
                        if (Util.AreHexStringsEqual(txtMd5.Text, providedHash))
                            machedType = HashType.MD5;
                    if (!string.IsNullOrEmpty(txtSha1.Text))
                        if (Util.AreHexStringsEqual(txtSha1.Text, providedHash))
                            machedType = HashType.SHA1;
                    if (!string.IsNullOrEmpty(txtSha256.Text))
                        if (Util.AreHexStringsEqual(txtSha256.Text, providedHash))
                            machedType = HashType.SHA256;
                    if (!string.IsNullOrEmpty(txtSha512.Text))
                        if (Util.AreHexStringsEqual(txtSha512.Text, providedHash))
                            machedType = HashType.SHA512;
                    if (machedType == HashType.None)
                    {
                        if (machedType == HashType.None && string.IsNullOrEmpty(txtMd5.Text))
                        {
                            CalculateHashAsync(HashType.MD5);
                            LoopWhileBusy();
                            if (!string.IsNullOrEmpty(txtMd5.Text))
                            {
                                if (Util.AreHexStringsEqual(txtMd5.Text, providedHash))
                                    machedType = HashType.MD5;
                            }
                        }
                        if (machedType == HashType.None && string.IsNullOrEmpty(txtSha1.Text))
                        {
                            CalculateHashAsync(HashType.SHA1);
                            LoopWhileBusy();
                            if (!string.IsNullOrEmpty(txtSha1.Text))
                            {
                                if (Util.AreHexStringsEqual(txtSha1.Text, providedHash))
                                    machedType = HashType.SHA1;
                            }
                        }
                        if (machedType == HashType.None && string.IsNullOrEmpty(txtSha256.Text))
                        {
                            CalculateHashAsync(HashType.SHA256);
                            LoopWhileBusy();
                            if (!string.IsNullOrEmpty(txtSha256.Text))
                            {
                                if (Util.AreHexStringsEqual(txtSha256.Text, providedHash))
                                    machedType = HashType.SHA256;
                            }
                        }
                        if (machedType == HashType.None && string.IsNullOrEmpty(txtSha512.Text))
                        {
                            CalculateHashAsync(HashType.SHA512);
                            LoopWhileBusy();
                            if (!string.IsNullOrEmpty(txtSha512.Text))
                            {
                                if (Util.AreHexStringsEqual(txtSha512.Text, providedHash))
                                    machedType = HashType.SHA512;
                            }
                        }
                    }
                    if (rbFile.Checked)
                    {
                        if (machedType == HashType.None)
                        {
                            ReportError("The verified hash is NOT a VALID HASH of any supported type for the specified file. " + Environment.NewLine
                                    + "File path: " + FilePath + Environment.NewLine
                                    + "Checked hash: " + txtVerify.Text + " ");
                        }
                        else
                        {
                            ReportInfo("The verified value matches the " + machedType.ToString() + " hash of the specified file." + Environment.NewLine + Environment.NewLine
                                + "File path: " + FilePath + Environment.NewLine
                                + "Checked hash: " + txtVerify.Text + " ");
                        }
                    }
                    else
                    {
                        if (machedType == HashType.None)
                        {
                            ReportError("The verified hash is NOT a VALID HASH of any supported type for the specified text. "
                                + Environment.NewLine + Environment.NewLine
                                + "Verified text is contained in teh text box at the bottom. ");
                        }
                        else
                        {
                            ReportInfo("The verified value matches the " + machedType.ToString() + " hash of the specified text."
                                + Environment.NewLine + Environment.NewLine
                                + "Verified text is contained in teh text box at the bottom. ");
                        }
                    }

                }
                catch (Exception ex)
                {
                    ReportError("Error occurred when trying to verify hash: " + Environment.NewLine + ex.Message);
                }

            }


            //if (string.IsNullOrEmpty(txtVerify.Text))
            //{
            //    ReportError("Can not verify the hash value: value is not specified." + Environment.NewLine + Environment.NewLine
            //        + "Paste hash value into the appropriate text field!");
            //    return;
            //}
            //else
            //{

            //    if (rbFile.Checked)
            //    {
            //        if (string.IsNullOrEmpty(FilePath))
            //        {
            //            ReportError("Can not verify the file's hash value: file is not specified." + Environment.NewLine + Environment.NewLine
            //                + "Select the file whose hash values are calculated and verified!");
            //            return;
            //        }
            //        else if (!File.Exists(FilePath))
            //        {
            //            ReportError("Can not verify the file's hash value: file does not exist." + Environment.NewLine
            //                + "File path: " + FilePath + Environment.NewLine + Environment.NewLine
            //                + "Select an existent file whose hash values are calculated and verified!");
            //            return;
            //        }
            //        else
            //        {
            //            HashType type = UtilCrypto.CheckFileHashSupportedTypesHex(FilePath, txtVerify.Text);
            //            if (type == HashType.None)
            //            {
            //                ReportError("The verified hash is NOT a VALID HASH of any supported type for the specified file. " + Environment.NewLine
            //                    + "File path: " + FilePath + Environment.NewLine
            //                    + "Checked hash: " + txtVerify.Text + " ");
            //            }
            //            else
            //            {
            //                ReportInfo("The verified value matches the " + type.ToString() + " hash of the specified file." + Environment.NewLine + Environment.NewLine
            //                    + "File path: " + FilePath + Environment.NewLine
            //                    + "Checked hash: " + txtVerify.Text + " ");
            //            }
            //        }
            //    }
            //    else if (rbText.Checked)
            //    {
            //        HashType type = UtilCrypto.CheckStringHashSupportedTypesHex(txtContents.Text, txtVerify.Text);
            //        if (type == HashType.None)
            //        {
            //            ReportError("The verified hash is NOT a VALID HASH of any supported type for the specified text. "
            //                + Environment.NewLine + Environment.NewLine
            //                + "Verified text is contained in teh text box at the bottom. ");
            //        }
            //        else
            //        {
            //            ReportInfo("The verified value matches the " + type.ToString() + " hash of the specified text."
            //                + Environment.NewLine + Environment.NewLine
            //                + "Verified text is contained in teh text box at the bottom. ");
            //        }
            //    }
            //}
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
                        string path = txtFile.Text;
                        FilePath = path;
                        if (rbFile.Checked)
                        {
                            if (!UtilSystem.IsReadableFile(path))
                            {
                                ReportError("Specified file does not exist or is not readable.");
                                indicatorLight1.SetError();
                                ClearHashes();
                            }
                            else
                            {
                                GenerateFilePreview();
                                CalculateHashes();
                            }
                        }
                    }
                    finally
                    {
                        _skipFileValidationEvent = false;
                    }
                }
            }
        }



        /// <summary>Browses for the file whose hash values will be calculated.</summary>
        private void btnFileBrowse_Click(object sender, EventArgs e)
        {
            string oldFilePath = FilePath;
            BrowseFile();
            if (!string.IsNullOrEmpty(FilePath) && FilePath != oldFilePath)
            {
                if (!rbFile.Checked)
                    rbFile.Checked = true;
            }
        }

        // When the hash algorithm type changes:

        private void chkMd5_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMd5.Checked)
            {
                if (string.IsNullOrEmpty(txtMd5.Text))
                    CalculateHashAsync(HashType.MD5);
            } else
            {
                //txtMd5.Clear();
                if (IsAllHashesCalculated)
                    indicatorLight1.SetOk();
            }
        }

        private void chkSha1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSha1.Checked)
            {
                if (string.IsNullOrEmpty(txtSha1.Text))
                    CalculateHashAsync(HashType.SHA1);
            } else
            {
                //txtSha1.Clear();
                if (IsAllHashesCalculated)
                    indicatorLight1.SetOk();
            }
        }

        private void chkSha256_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSha256.Checked)
            {
                if (string.IsNullOrEmpty(txtSha256.Text))
                    CalculateHashAsync(HashType.SHA256);
            } else
            {
                //txtSha256.Clear();
                if (IsAllHashesCalculated)
                    indicatorLight1.SetOk();
            }
        }

        private void chkSha512_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSha512.Checked)
            {
                if (string.IsNullOrEmpty(txtSha512.Text))
                    CalculateHashAsync(HashType.SHA512);
            } else
            {
                //txtSha512.Clear();
                if (IsAllHashesCalculated)
                    indicatorLight1.SetOk();
            }
        }

        // Copying hashes to clipboard:

        private void btnMd5Copy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMd5.Text))
            {
                CalculateHashAsync(HashType.MD5);
                LoopWhileBusy();
            }
            if (!string.IsNullOrEmpty(txtMd5.Text))
                Clipboard.SetText(txtMd5.Text);
            else
                ReportError("The MD5 hash is not available.");
        }

        private void btnSha1Copy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSha1.Text))
            {
                CalculateHashAsync(HashType.SHA1);
                LoopWhileBusy();
            }
            if (!string.IsNullOrEmpty(txtSha1.Text))
                Clipboard.SetText(txtSha1.Text);
            else
                ReportError("The SHA1 hash is not available.");
        }

        private void btnSha256Copy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSha256.Text))
            {
                CalculateHashAsync(HashType.SHA256);
                LoopWhileBusy();
            }
            if (!string.IsNullOrEmpty(txtSha256.Text))
                Clipboard.SetText(txtSha256.Text);
            else
                ReportError("The SHA256 hash is not available.");
        }

        private void bthSha512_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSha512.Text))
            {
                CalculateHashAsync(HashType.SHA512);
                LoopWhileBusy();
            }
            if (!string.IsNullOrEmpty(txtSha512.Text))
                Clipboard.SetText(txtSha512.Text);
            else
                ReportError("The SHA512 hash is not available.");
        }

        // Pasting verified hash value from the clipboard:

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
            RecalculateHashes = false;
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
                chkCalculateWhenTyping.Visible = false;
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
                if (NumInsertTextInfo < MaxInsertTextInfo) // && !TextBeingDropped)
                {
                    ++NumInsertTextInfo;
                    string infoString =
                          "Operation changed to text mode." + Environment.NewLine + Environment.NewLine
                        + "You can insert text whose hash values are calculated into the larger text box!";
                    if (NumInsertTextInfo == MaxInsertTextInfo)
                        infoString += Environment.NewLine + Environment.NewLine + "This message will not be shown again during this session.";
                    FadingMessageInfo(infoString);
                    // ReportInfo(infoString);
                }
                chkCalculateWhenTyping.Visible = true;
            }
        }


        /// <summary>Handles situation when one changes the setting whether to store file's hash values
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
            if (!rbFile.Checked)
            {
                // If we are calculating hashes of the text entered in this text box, then we must clear 
                // calculated hashes when text box re-gains focus.
                ClearHashes();
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

            if (chkCalculateWhenTyping.Checked && rbText.Checked)
            {
                if (IsGroupCalculationPerformed)
                {
                    RecalculateHashes = true;
                }
                else
                {
                    CalculateHashes();
                }
            }

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


        private void txtContents_KeyPress(object sender, KeyPressEventArgs e)
        {
        }


        /// <summary>Recalculates hashes because the requested lower/upper case form has changed.</summary>
        private void chkUpperCase_CheckedChanged(object sender, EventArgs e)
        {
            // CalculateHashes();
            foreach (TextBox tb in new TextBox[] { txtMd5, txtSha1, txtSha256, txtSha512 })
            {
                if (!string.IsNullOrEmpty(tb.Text))
                {
                    if (chkUpperCase.Checked)
                    {
                        tb.Text = tb.Text.ToUpper();
                    } else
                    {
                        tb.Text = tb.Text.ToLower();
                    }
                }
            }
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
                        FilePath = "";
                        txtFile.Text = "";
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
            // After drag/drop, put the containing form in front of others:
            _form.Activate();
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
                        // ClearHashes();
                        fileDropped = true;
                        FilePath = "";
                        txtFile.Text = "";
                        if (!rbFile.Checked)
                        {
                            rbFile.Checked = true;
                        }

                        txtFile.Text = droppedFilePath;
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
                        ClearHashes();
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


        protected HashAboutForm _aboutForm = new HashAboutForm();

        private void btnAbout_Click(object sender, EventArgs e)
        {
            try
            {
                if (_aboutForm == null)
                {
                    _aboutForm = new HashAboutForm();
                }
                _aboutForm.ShowDialog();


            }
            catch (Exception)
            {
                _aboutForm = null;
            }
        }

        private void btnCancelCalculation_Click(object sender, EventArgs e)
        {
            CancelHashCalculations();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            _aboutForm.MainControl.HelpPdf();
        }

        private void chkCalculateWhenTyping_CheckedChanged(object sender, EventArgs e)
        {

        }



        /// <summary>Intended for handling key presses such as F1 for help, etc.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HashControl_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpHandler(sender, e);
        }

        /// <summary>Handles key up events for the current form.</summary>
        /// <remarks>Form does not catch the KeyUp events. Therefore, the event handler code was moved to a public function, which
        /// can be called from a form, tab control, or other control that contains this control.</remarks>
        public void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                if (e.Control)
                {
                    btnAbout_Click(sender, e);
                }
                else
                    btnHelp_Click(sender, e);
            }
        }


        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnHelp_Click(sender, e);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAbout_Click(sender, e);
        }


        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }




        #region UserInteraction



        /// <summary>Sets mouse cursor shape on MouseUp event, according to the current status of modifier keys.</summary>
        void SetCursorMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>Sets mouse cursor shape on MouseDown event, according to the current status of modifier keys.</summary>
        void SetCursorMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!(ControlDown || ShiftDown || AltDown))
                {
                    Cursor.Current = Cursors.SizeAll;
                }
                if (ControlDown && !(ShiftDown || AltDown))
                {
                    Cursor.Current = Cursors.SizeNESW;
                }
                if (ShiftDown && !(ControlDown || AltDown))
                {
                    Cursor.Current = Cursors.PanNE;
                }
                if (AltDown && !(ControlDown || ShiftDown))
                {
                    Cursor.Current = Cursors.Hand;
                }
                if (ControlDown && ShiftDown && !AltDown)
                {
                    Cursor.Current = Cursors.NoMove2D;
                }
                if (ControlDown && AltDown && !ShiftDown)
                {
                    Cursor.Current = Cursors.NoMoveHoriz;
                }
                if (ShiftDown && AltDown && !ControlDown)
                {
                    Cursor.Current = Cursors.Cross;
                }
            }
        }

        ///// <summary>Sets mouse cursor shape on MouseMove event, according to the current status of modifier keys.</summary>
        //void SetCursorMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        if (!(ControlDown || ShiftDown || AltDown))
        //        {
        //            Cursor.Current = Cursors.SizeAll;
        //        }
        //        if (ControlDown && !(ShiftDown || AltDown))
        //        {
        //            Cursor.Current = Cursors.SizeNESW;
        //        }
        //        if (ShiftDown && !(ControlDown || AltDown))
        //        {
        //            Cursor.Current = Cursors.PanNE;
        //        }
        //        if (AltDown && !(ControlDown || ShiftDown))
        //        {
        //            Cursor.Current = Cursors.Hand;
        //        }
        //        if (ControlDown && ShiftDown && !AltDown)
        //        {
        //            Cursor.Current = Cursors.NoMove2D;
        //        }
        //        if (ControlDown && AltDown && !ShiftDown)
        //        {
        //            Cursor.Current = Cursors.NoMoveHoriz;
        //        }
        //        if (ShiftDown && AltDown && !ControlDown)
        //        {
        //            Cursor.Current = Cursors.Cross;
        //        }
        //    }
        //}



        Form _form { get { return this.ParentForm; } }

        int _mouseInitialLocalX;
        int _mouseInitialLocalY;
        int _mouseInitialX;
        int _mouseInitialY;

        int _formInitialLeft;
        int _formInitialTop;
        int _formInitialWidth;
        int _formInitialHeight;
        double _mouseInitialRelX;
        double _mouseInitialRelY;

        int _mousePreviousX;
        int _mousePreviousY;

        int _distMoveX;
        int _distMoveY;
        int _distSizeX;
        int _distSizeY;

        bool _isSizingOrMoving = false;

        /*
        int _distY;
        int _distControlX;
        int _distControlY;
        int _distShiftX;
        int _distShiftY;
        int _distAltX;
        int _distAltY;
        */



        /// <summary>Indicates whether Control key is currently pressed.</summary>
        bool ControlDown { get { return (ModifierKeys & Keys.Control) == Keys.Control; } }
        /// <summary>Indicates whether Shift key is currently pressed.</summary>
        bool ShiftDown { get { return (ModifierKeys & Keys.Shift) == Keys.Shift; } }
        /// <summary>Indicates whether Alt key is currently pressed.</summary>
        bool AltDown { get { return (ModifierKeys & Keys.Alt) == Keys.Alt; } }

        /// <summary>Gets X component of current mouse cursor position (global, distance from left edge of display).</summary>
        int MouseX { get { return Cursor.Position.X; } }
        /// <summary>Gets Y component of current mouse cursor position (global, distance from top edge of display).</summary>
        int MouseY { get { return Cursor.Position.Y; } }
        /// <summary>Mouse relative X position within the form (runs from -0.5 to 0.5).</summary>
        double MouseRelX { get { return (double)(MouseX - _form.Left) / (double)(_form.Width) - 0.5; } }
        /// <summary>Mouse relative Y position within the form (runs from -0.5 to 0.5).</summary>
        double MouseRelY { get { return (double)(MouseY - _form.Top) / (double)(_form.Height) - 0.5; } }



        void InitiateMoveOrSize(object sender, MouseEventArgs e)
        {
            if (ControlDown && !ShiftDown)
                Cursor.Current = Cursors.PanSE;
            else
                Cursor.Current = Cursors.SizeAll;
            _isSizingOrMoving = true;
            _mouseInitialLocalX = e.X;
            _mouseInitialLocalY = e.Y;
            _mouseInitialX = MouseX;
            _mouseInitialY = MouseY;
            _mousePreviousX = MouseX;
            _mousePreviousY = MouseY;
            _distMoveX = 0;
            _distMoveY = 0;
            _distSizeX = 0;
            _distSizeY = 0;
            _formInitialLeft = _form.Left;
            _formInitialTop = _form.Top;
            _formInitialWidth = _form.Width;
            _formInitialHeight = _form.Height;
            _mouseInitialRelX = MouseRelX;
            _mouseInitialRelY = MouseRelY;
        }

        //bool _isAltPressed = false;

        /// <summary>Common event handler for key down event, installed on all controls.</summary>
        void Common_KeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>Common event handler for key up event, installed on all controls.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Common_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpHandler(sender, e);
        }

        /// <summary>Handles mouse click events for the current control.
        /// Available on non-interactive controls of the current control.</summary>
        private void Common_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    bool ControlPressed = false;
                    try
                    {
                        Control SenderControl = sender as Control;
                        if (SenderControl != null)
                        {
                            if (Control.ModifierKeys == Keys.Control)
                                ControlPressed = true;
                        }
                    }
                    catch { }
                    if (ControlPressed)
                        ; //  CloseForm();
                    else
                        menuMain.Show(Cursor.Position);
                }
            }
            catch { }
        }

        /// <summary>Initiates dragging or resizing by mouse on MouseDown event.</summary>
        void Common_MouseDownOnInteractive(object sender, MouseEventArgs e)
        {
            if (AltDown)  //ShiftDown && ControlDown)
            {
                InitiateMoveOrSize(sender, e);
            }
        }


        ///// <summary>Initiates dragging or resizing by mouse on MouseDown event.</summary>
        //void Common_MouseDownModifierKeysInitiateMoveOrSize(object sender, MouseEventArgs e)
        //{

        //    InitiateMoveOrSize(sender, e);
        //}

        /// <summary>Enables dragging by mouse.</summary>
        private void Common_MouseDownOnNoninteractiveControls(object sender, MouseEventArgs e)
        {
            // SetCursorMouseDown(sender, e);
            if (e.Button == MouseButtons.Left)
            {
                InitiateMoveOrSize(sender, e);

                //FadingMessageWarning(
                //    "Form size (with, height): " + _formInitialWidth + ", " + _formInitialHeight + Environment.NewLine +
                //    "Form position (from left, from top): " + _formInitialLeft + ", " + _formInitialTop + Environment.NewLine +
                //    "Mouse local position (X, Y) :                          " + _mouseInitialLocalX + ", " + _mouseInitialLocalY + Environment.NewLine +
                //    "Mouse global position (X, Y) :                         " + _mouseInitialX + ", " + _mouseInitialY + Environment.NewLine +
                //    "Mouse relative centered position in form (relX, relY): " + _mouseInitialRelX + ", " + _mouseInitialRelY + Environment.NewLine
                //    );

            }

        }

        /// <summary>Enables dragging by mouse.</summary>
        private void Common_MouseUp(object sender, MouseEventArgs e)
        {
            SetCursorMouseUp(sender, e);
            if (e.Button == MouseButtons.Left)
            {
                _isSizingOrMoving = false;  // de-pressing left button stops resizing or moving
                Cursor.Current = Cursors.Default;
            }
        }

        

        /// <summary>Enables dragging by mouse.</summary>
        private void Common_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left && _isSizingOrMoving)
            {
                
                // SetCursorMouseMove(sender, e);
                int diffMouseX = MouseX - _mousePreviousX;
                int diffMouseY = MouseY - _mousePreviousY;
                _mousePreviousX = MouseX;
                _mousePreviousY = MouseY;
                if (AltDown)
                {
                    //// Prevent mouse cursor from moving:
                    //Cursor.Position = new Point(_mousePreviousX, _mousePreviousY);
                    // Change cursor shape until Alt key is released:
                    Cursor.Current = Cursors.Cross;
                } else
                {
                    if (ControlDown /* && !ShiftDown */)
                    {
                        // Resizing is performed
                        Cursor.Current = Cursors.PanSE;
                        _distSizeX += diffMouseX;
                        _distSizeY += diffMouseY;
                    }
                    else if (!ControlDown /* (ControlDown && ShiftDown) */ )
                    {
                        Cursor.Current = Cursors.SizeAll;
                        _distMoveX += diffMouseX;
                        _distMoveY += diffMouseY;
                    }
                }
                _form.Left = _formInitialLeft + _distMoveX;
                _form.Top = _formInitialTop + _distMoveY;
                _form.Width = _formInitialWidth + _distSizeX;
                _form.Height = _formInitialHeight + _distSizeY;
            } else {
                if (AltDown)
                    Cursor.Current = Cursors.Cross;
            }
        }



        /// <summary>This method is used to set  the common event handlers for the current control and its subcontrols.</summary>
        /// <param name="control">Control on which common event handlers are set.</param>
        void SetCommonEvents(Control control)
        {
            {

                try
                {
                    if (control != txtContents && control != txtFile)
                    {
                        control.AllowDrop = true;
                        control.DragEnter += txtContents_DragEnter;
                        control.DragDrop += txtContents_DragDrop;
                    }
                }
                catch { }

                try
                {
                    if (!(control is TextBox || control is Button || control is CheckBox || control is RadioButton))
                    {
                        // Non-interactive controls:
                        control.MouseClick += Common_MouseClick;
                        // Initiate moving or resizing window by click&drag inside (on panels and similar, not on interactive controls)
                        // This will allow dragging the window by mouse, no mather where mouse clicks:
                        control.MouseDown += new MouseEventHandler(this.Common_MouseDownOnNoninteractiveControls);
                    } else
                    {
                        // Interactive controls:
                        control.MouseDown += new MouseEventHandler(Common_MouseDownOnInteractive);
                    }
                    // MouseUp event stops positioning and sizing, and it must be added to each and every control because
                    // during resiying, control under mouse when depressing left button may be different than the one 
                    // that was under mouse when pressing the button (and initiating resize / move):
                    control.MouseMove += new MouseEventHandler(this.Common_MouseMove);
                    // MouseUp should be installed on all controls:
                    control.MouseUp += new MouseEventHandler(this.Common_MouseUp);
                    // KeyDown and KeyUp also installed on all controls:
                    control.KeyDown += new KeyEventHandler(this.Common_KeyDown);
                    control.KeyUp += new KeyEventHandler(this.Common_KeyUp);
            }
                catch { }  // catch (Exception e) { Exception ee = e; }
            }
        }






        #endregion UserInteraction

        private void HashControl_Load(object sender, EventArgs e)
        {
            try
            {
                // Set common event handlers recursively:
                UtilForms.RecursiveControlDelegate(this, new ControlDelegate(SetCommonEvents));

            }
            catch (Exception ex)
            {
                ReportError("Could not set common events for user interaction. Error: "
                    + Environment.NewLine + ex.Message);
            }

        }

        #region ContextMenu

        private void helpToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            btnHelp_Click(sender, e);
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            btnAbout_Click(sender, e);
        }


        #endregion ContextMenu
    }  // class HashControl


}

