// Copyright (c) Igor Grešovnik, IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace IG.Forms
{
    public partial class MessageBoxLauncher : Form
    {
        public MessageBoxLauncher()
        {
            InitializeComponent();
        }

        private void MessageBoxLauncher_Load(object sender, EventArgs e)
        {
            try
            {
                lblCurrentDir.Text = Directory.GetCurrentDirectory();
                if (!File.Exists(txtFileOpen.Text))
                    txtFileOpen.BackColor = Color.Orange;
                if (!Directory.Exists(txtDirOpen.Text))
                {
                    txtDirOpen.BackColor = Color.Orange;
                }
            }
            catch {}
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text;
            string message = txtMessage.Text;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            if (rbError.Checked)
                icon = MessageBoxIcon.Error;
            else if (rbWarning.Checked)
                icon = MessageBoxIcon.Warning;
            else if (rbInfo.Checked)
                icon = MessageBoxIcon.Information;
            else if (rbWarning.Checked)
                icon = MessageBoxIcon.Warning;
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            if (rbAbortRetryIgnore.Checked)
                buttons = MessageBoxButtons.AbortRetryIgnore;
            else if (rbYesNo.Checked)
                buttons = MessageBoxButtons.YesNo;
            else if (rbYesNoCancel.Checked)
                buttons = MessageBoxButtons.YesNoCancel;
            else if (rbOK.Checked)
                buttons = MessageBoxButtons.OK;
            else if (rbOKCancel.Checked)
                buttons = MessageBoxButtons.OKCancel;
            DialogResult dlgResult = MessageBox.Show(message, title, buttons, icon);
            lblButtonClicked.Text = dlgResult.ToString();
        }

        private void btnCurrentDirRefresh_Click(object sender, EventArgs e)
        {
            lblCurrentDir.Text = Directory.GetCurrentDirectory();
        }

        private void btnFileOpenBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            string filePath = txtFileOpen.Text;
            if ((!string.IsNullOrEmpty(filePath)) && File.Exists(filePath))
            {
                dialog.InitialDirectory=Path.GetDirectoryName(filePath);
                dialog.FileName = Path.GetFileName(filePath);
            } else
            {
                dialog.InitialDirectory = Directory.GetCurrentDirectory();
                dialog.FileName = "ExampleFile.txt";
            }
            DialogResult res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                filePath = Path.GetFullPath(dialog.FileName);
                txtFileOpen.Text = filePath;
            }
        }

        private void txtFileOpen_TextChanged(object sender, EventArgs e)
        {
            string filePath = txtFileOpen.Text;
            if (File.Exists(filePath))
            {
                txtFileOpen.BackColor = Color.White;
                FileInfo fInfo = new FileInfo(filePath);
                lblFileSize.Text = fInfo.Length.ToString();
            }
            else
            {
                txtFileOpen.BackColor = Color.Orange;
                lblFileSize.Text = "< File does not exist. >";
            }
        }

        private void btnDirOpenBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string filePath = txtDirOpen.Text;
            if ((!string.IsNullOrEmpty(filePath)) && Directory.Exists(filePath))
            {
                dialog.SelectedPath = Path.GetDirectoryName(filePath);
            }
            else
            {
                dialog.SelectedPath = Directory.GetCurrentDirectory();
            }
            DialogResult res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                txtDirOpen.Text = Path.GetFullPath(dialog.SelectedPath);
            }
        }

        private void txtDirOpen_TextChanged(object sender, EventArgs e)
        {
            string filePath = Path.GetFullPath(txtDirOpen.Text);
            if (!string.IsNullOrEmpty(filePath) && Directory.Exists(filePath))
            {
                txtDirOpen.BackColor = Color.White;
                DirectoryInfo dirInfo = new DirectoryInfo(filePath);
                FileInfo[] containedFiles = dirInfo.GetFiles();
                if (containedFiles == null)
                    lblDirNumFiles.Text = "0";
                else
                    lblDirNumFiles.Text = containedFiles.Length.ToString();
            }
            else
            {
                txtDirOpen.BackColor = Color.Orange;
                lblDirNumFiles.Text = "< Dierectory does not exist. >";
            }
         }

    }
}
