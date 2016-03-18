// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

using System.Media;

using System.Windows.Forms;


namespace IG.Forms
{
    public partial class SoundPlayerControlSimple : UserControl
    {
        public SoundPlayerControlSimple()
        {
            InitializeComponent();
        }

        private void fileSelector1_FileSelected(object sender, EventArgs e)
        {
            Player.SoundLocation = fileSelector1.FilePath;
            Player.LoadAsync();
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
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
                        fileSelector1.FilePath = droppedFilePath;
                    }
                }

        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }


        System.Media.SoundPlayer _soundPlayer;

        private System.Media.SoundPlayer Player
        {
            get {
                if (_soundPlayer == null)
                    _soundPlayer = new System.Media.SoundPlayer();
                return _soundPlayer;
            }
        }


        private void btnPlay_Click(object sender, EventArgs e)
        {
            Player.Play();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void chkRepeat_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }




        //private void txtFile_DragEnter(object sender, DragEventArgs e)
        //{
        //    e.Effect = DragDropEffects.Copy;
        //}

        //private void txtFile_DragDrop(object sender, DragEventArgs e)
        //{
        //}




    }
}
