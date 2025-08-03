// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using IG.Lib;
using System.IO;
using System.Net;

namespace IG.Forms
{
    public partial class ImageViewerControl : UserControl, IFileViewer
    {
        public ImageViewerControl()
        {
            InitializeComponent();
            comboSizeMode.Items.Clear();
            int selectedIndex = -1;
            for (int i = 0; i < _availableSizeModes.Count; ++i)
            {
                PictureBoxSizeMode mode = _availableSizeModes[i];
                comboSizeMode.Items.Add(mode);
                if (mode == SizeMode)
                    comboSizeMode.SelectedIndex = i;
            }
            if (selectedIndex >= 0)
            {
                comboSizeMode.SelectedIndex = 0;
                SizeMode = (PictureBoxSizeMode) comboSizeMode.Items[0];
            }
            comboSizeMode.Text = SizeMode.ToString();
        }

        #region Data

        protected List<PictureBoxSizeMode> _availableSizeModes = Util.GetEnumValues<PictureBoxSizeMode>();

        private PictureBoxSizeMode _sizeMode = PictureBoxSizeMode.CenterImage;

        /// <summary>Enumeraton of type <see cref="PictureBoxSizeMode"/> that specifies how the image is sized and positioned
        /// witin the <see cref="PictureBox"/> control.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                if (value != _sizeMode)
                {
                    _sizeMode = value;
                    pictureBox1.SizeMode = value;
                }
            }
        }

        #endregion Data


        #region IFileViewer

        // Auxiliary methods (may be moved to utility class):

        /// <summary>Chechks whether the specified URI is a valid HTTP URL.</summary>
        /// <param name="uriNamme">URL to be tested.</param>
        /// <returns>True iif the specified string is a valid HTTP URL, false otherwise.</returns>
        public static bool IsValidHttpUrl(string uriNamme)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(uriNamme, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }


        /// <summary>Gets an image from the specified URL.</summary>
        /// <param name="url">URL of the image.</param>
        /// <returns>Image object (of class <see cref="Image"/>) obtained from the specified URL.</returns>
        public static Image GetImageFromUrl(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    return Image.FromStream(stream);
                }
            }
        }



        protected string _viewedFile;

        /// <summary>Gets or sets path to the file or URL of the file that is currently being viewed.
        /// <para>Whether it is actually shown depends on the <see cref="IsShownImmediately"/> flag).</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ViewedFile
        {  get { return _viewedFile; }
            set {
                if (value != _viewedFile)
                {
                    _viewedFile = value;
                    if (string.IsNullOrEmpty(ViewedFile))
                    {

                    } else
                    {
                        _stream = null;
                        if (CanViewFiles && File.Exists(_viewedFile))
                        {
                            fileSelector1.FilePath = _viewedFile;
                            if (IsShownImmediately)
                                ShowViewedItem();
                        }
                        else if (CanViewUrls && IsValidHttpUrl(_viewedFile))
                        {
                            fileSelector1.FilePath = _viewedFile;
                            if (IsShownImmediately)
                                ShowViewedItem();
                        }
                        else
                        {
                            throw new ArgumentException("Can not view the following image file location: " + _viewedFile);
                        }
                    }
                }

                // fileSelector1.FilePath = value;

            } }


        bool _closeStreamAfterUse = false;

        protected Stream _stream;

        /// <summary>Gets or sets the stream containing the file that is currently being viewed.
        /// <para>Whether it is actually shown depends on the <see cref="IsShownImmediately"/> flag).</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Stream ViewedStream
        { get { return _stream; }
            set
            {
                if (!object.ReferenceEquals(_stream, value))
                {
                    if (_closeStreamAfterUse)
                    {
                        if (_stream != null)
                            _stream.Close();
                    }
                    if (!CanViewStreams)
                    {
                        throw new ArgumentException("The image viewer used can not view sreams.");
                    }
                    _stream = value;
                    if (_stream != null)
                    {
                        if (IsShownImmediately)
                            ShowViewedItem();
                    }
                }
            }
        }

        protected bool _isShownImmediately = true;

        /// <summary>Flag that specifies whether content is shown immediately after the viewed object
        /// is set.
        /// <para>If set to true and the viewer does not at all have such capability, this should NOT throw
        /// an exception (thus, user should check success by verifying if flag changed).</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsShownImmediately
        {
            get { return _isShownImmediately; }
            set {
                if (value != _isShownImmediately)
                {
                    _isShownImmediately = value;
                    if (_isShownImmediately)
                    {
                        ShowViewedItem();
                    }
                }
            }
        }

        /// <summary>Shows / plays the viewed contents.
        /// <para>Can have the role of refresh when <see cref="IsShownImmediately"/> is true, or is used to
        /// actually show the contents after assigning it.</para></summary>
        public void ShowViewedItem()
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            if (!string.IsNullOrEmpty(ViewedFile))
            {
                if (CanViewFiles && File.Exists(ViewedFile))
                {
                    Image image = Image.FromFile(ViewedFile);
                    this.pictureBox1.Image = image;
                } else if (CanViewUrls && IsValidHttpUrl(_viewedFile))
                {
                    Image image = GetImageFromUrl(_viewedFile);
                    pictureBox1.Image = image;
                }
                else
                {
                    throw new ArgumentException("Can not view image file: " + ViewedFile + ".");
                }
            } else if (ViewedStream != null)
            {
                Image image = Image.FromStream(ViewedStream);
                pictureBox1.Image = image;
            }
        }

        /// <summary>Clears the viewer and eventually releases any resources used.</summary>
        public void Clear()
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            ViewedFile = null; ViewedStream = null; }


        protected bool _canViewFiles = true;

        /// <summary>Whether the current viewer can view files from the file system.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CanViewFiles
        {  get  {  return _canViewFiles;  }
        set { _canViewFiles = value; } }


        protected bool _canViewUrls = true;

        /// <summary>Whether the current viewer can view web resources (files on the internet).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CanViewUrls
        {
            get { return _canViewUrls; }
            set { _canViewUrls = value; }
        }

        /// <summary>Whether the current viewer can read streams.</summary>
        protected bool _canViewStreams = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool CanViewStreams
        {
            get { return _canViewStreams; }
            set { _canViewStreams = value; }
        }



        /// <summary>Rerurns a flag indicating whether the file (or web resource) at the specified location is eligible 
        /// for vieiwnig in the current viewer or not.</summary>
        /// <param name="fileLocation">Path to the file on a file system, or file URL.</param>
        /// <returns>True if the file can be viewed in the current viewer, false otherwise.</returns>
        public bool IsEligibleForViewing(string fileLocation)
        {
            bool ret = false;
            if (CanViewFiles && File.Exists(fileLocation))
            {
                Image image = null;
                try
                {
                    image = Image.FromFile(fileLocation);
                    if (image != null)
                        ret = true;
                }
                catch { }
                finally { if (image != null) image.Dispose(); image = null; }
                if (ret)
                    return ret;
            }
            if (CanViewUrls && IsValidHttpUrl(fileLocation))
            {
                Image image = null;
                try
                {
                    image = GetImageFromUrl(fileLocation);
                    if (image != null)
                        ret = true;
                }
                catch { }
                finally { if (image != null) image.Dispose(); image = null; }
                if (ret)
                    return ret;
            }
            return ret;
        }

        /// <summary>Returnns a flag indicating whether the specified stream is eligible for vieiwnig or not.</summary>
        /// <param name="stream">Path to the file on a file system, or file URL.</param>
        /// <returns>True if the stream can be viewed in the current viewer, false otherwise.</returns>
        public bool IsEligibleForViewing(Stream stream)
        {
            bool ret = false;
            Image image = null;
            try
            {
                image = Image.FromStream(stream);
                if (image != null)
                    ret = true;
            }
            catch { }
            finally {
                if (image != null)
                    image.Dispose();
                image = null;
            }
            return ret;
        }


        bool _isControlsVisible = true;

        /// <summary>Updates dependencies on <see cref="IsControlsVisible"/>.</summary>
        protected void UpdateControlsVisible()
        {
            menuShowControls.Checked = IsControlsVisible;
            pnlControls.Visible = IsControlsVisible;
        }

        /// <summary>Gets / sets a flag that specified whether the viewer's controls are visible or not.
        /// <para>Setting should have immediate effect and make controls visible / invisible.</para></summary>
        /// <remarks>This is important because viewers must be able to be used embedded in other controls, where 
        /// all behavior is handled by those controls and viewer just provides the viewing area.</remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsControlsVisible
        { get { return _isControlsVisible; }
            set
            {
                if (value != _isControlsVisible)
                {
                    _isControlsVisible = value;
                    UpdateControlsVisible();
                }
            }
        }

        /// <summary>Opens a browser to select the file shown.
        /// <para>Available only when <see cref="IsBrowsable"/> property is true.</para></summary>
        public void BrowseFile()
        {
            if (IsBrowsable)
                fileSelector1.BrowseFile();
        }

        protected bool _isBrowsable = true;

        /// <summary>Updates dependencies on <see cref="IsBrowsable"/>.</summary>
        protected void UpdateBrowsable()
        {
            fileSelector1.IsBrowsable = IsBrowsable;
            menuBrowse.Checked = IsBrowsable;
        }

        /// <summary>Gets or sets the flag specifying whether one can browse for the file to be opened (e.g. by
        /// using the file selection dialog).
        /// <para>If the viewer does not at all have browser capability then setting to true should not throw an 
        /// exception (therefore, the user should verify by getter if setting actually worked).</para>
        /// <para>Setter can be used to disable drag & drop (e.g. for security, or when used as embedded control
        /// and drag & drop would be disturbing). If the viewer does not at all have drag & drop capability, setting
        /// this flag to true should NOT throw an exception (thus, user should check if it was successful).</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsBrowsable
        {
            get { return _isBrowsable; }
            set {
                if (value != _isBrowsable)
                {
                    _isBrowsable = value;
                    UpdateBrowsable();
                }
            } }

        protected bool _isDragAndDrop = true;

        /// <summary>Updates dependencies on <see cref="IsDragAndDrop"/>.</summary>
        protected void UpdateDragAndDrop()
        {
            fileSelector1.IsDragAndDrop = IsDragAndDrop;
            menuDragDrop.Checked = IsDragAndDrop;
        }

        /// <summary>Whether the viewer provides drag & drop capability.
        /// <para>If the viewer does not at all have browser capability then setting to true should not throw an 
        /// exception (therefore, the user should verify by getter if setting actually worked).</para>
        /// <para>Setter can be used to disable drag & drop (e.g. for security, or when used as embedded control
        /// and drag & drop would be disturbing). If the viewer does not at all have drag & drop capability, setting
        /// this flag to true should NOT throw an exception (thus, user should check if it was successful).</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsDragAndDrop
        {
            get { return _isDragAndDrop; }
            set {
                if (value != _isDragAndDrop)
                {
                    _isDragAndDrop = value;
                    UpdateDragAndDrop();
                }
            }
        }


        #endregion IFileViewer



        /// <summary>Triggered when file is selected by the embedded file selector control.</summary>
        private void fileSelector1_FileSelected(object sender, EventArgs e)
        {
            // pictureBox1.ImageLocation = fileSelector1.FilePath;
            //  pictureBox1.Image = Image.FromFile(fileSelector1.FilePath);
            this.ViewedFile = fileSelector1.FilePath;
        }


        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
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
                            // fileSelector1.FilePath = droppedFilePath;
                            this.ViewedFile = droppedFilePath;
                        }
                    }
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (IsDragAndDrop)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }


        private void comboSizeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            SizeMode = (PictureBoxSizeMode)comboSizeMode.Items[comboSizeMode.SelectedIndex];
        }


        private void menuShowControls_Click(object sender, EventArgs e)
        {
            if (IsControlsVisible)
                IsControlsVisible = false;
            else
                IsControlsVisible = true;
        }

        private void menuClearHistory_Click(object sender, EventArgs e)
        {
            fileSelector1.ClearHistory();
        }

        private void comboSizeMode_Click(object sender, EventArgs e)
        {
            comboSizeMode.DroppedDown = true;
        }

        private void menuDragDrop_Click(object sender, EventArgs e)
        {
            if (IsDragAndDrop)
                IsDragAndDrop = false;
            else
                IsDragAndDrop = true;
        }

        private void menuBrowse_Click(object sender, EventArgs e)
        {
            if (IsBrowsable)
                IsBrowsable = false;
            else
                IsBrowsable = true;
        }

        private void showControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsControlsVisible)
                IsControlsVisible = false;
            else
                IsControlsVisible = true;
        }

        /// <summary>Handles the load event of the current control.</summary>
        private void ImageViewerControl_Load(object sender, EventArgs e)
        {
            // On load, update controls' GUI state according to its properties:
            UpdateControlsVisible();
            UpdateBrowsable();
            UpdateDragAndDrop();
            pictureBox1.SizeMode = SizeMode;
        }
    }
}
