// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Reflection;

using IG.Lib;

namespace IG.Forms
{
    public partial class ResourceViewerControl : UserControl
    {
        public ResourceViewerControl()
        {
            InitializeComponent();
            RefreshOutput();
        }

        #region Data

        private static string _initialTextOutpput =
@"<< Select resources. >>";


        private List<string> _auxList = new List<string>();



        protected bool _isImagePreviewActive = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsImagePreviewActive
        {
            get { return _isImagePreviewActive; }
            protected set
            {
                _isImagePreviewActive = value;
                if (value != _isImagePreviewActive)
                {
                    IsSoundPreviewActive = false;
                }
            }
        }


        protected bool _isSoundPreviewActive = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsSoundPreviewActive
        {
            get { return _isSoundPreviewActive; }
            protected set {
                if (value != _isSoundPreviewActive)
                {
                    _isSoundPreviewActive = value;
                    if (value == true)
                    {
                        IsImagePreviewActive = false;
                    }
                }
            }
        }

        public bool IsAnyPreviewActive
        { get {
                foreach (IFileViewer viewer in Viewers)
                {
                    Control viewerControl = viewer as Control;
                    if (viewerControl != null)
                        if (viewerControl.Visible)
                            return true;
                }
                // return (IsImagePreviewActive || IsSoundPreviewActive  );
                return false;
            } }


        public void UpdatePreviewArea()
        {
            // TODO: Change visible fo false later!
            imageViewerControl1.Visible = true;
            soundPlayerControlSimple1.Visible = true;
            imageViewerControl1.Dock = DockStyle.None;
            soundPlayerControlSimple1.Dock = DockStyle.None;


            splitContainer1.Panel2Collapsed = false;
            if (IsAnyPreviewActive)
            {
                // If we will show a preview, take care that its panel is not collapsed on the split container 
                // that splits between text outpt (log) and the preview control:
                splitContainer1.Panel1Collapsed = false;
            } else
            {
                splitContainer1.Panel1Collapsed = false;
            }
            if (IsImagePreviewActive)
            {
                imageViewerControl1.Visible = true;
                imageViewerControl1.Dock = DockStyle.Fill;
            } else if (IsSoundPreviewActive)
            {
                soundPlayerControlSimple1.Visible = true;
                soundPlayerControlSimple1.Dock = DockStyle.Fill;
            } else
            {

            }
        }



        /// <summary>Refreshed information about the selected assembly (of potential assemblied if multiple assemblies
        /// descriptions are allowed) on the output text control.</summary>
        public void RefreshViewer()
        {


            RefreshOutput();
        }

        /// <summary>Refreshed information about the selected assembly (of potential assemblied if multiple assemblies
        /// descriptions are allowed) on the output text control.</summary>
        public void RefreshOutput()
        {
            txtOutput.Clear();
            if (resourceSelector1.SelectedResource != null)
            {
                txtOutput.AppendText(Environment.NewLine + "Selected resource: " + Environment.NewLine
                    + "  " + resourceSelector1.SelectedResource + Environment.NewLine + Environment.NewLine);
            }
            resourceSelector1.GetSelectableResources(_auxList, true  /* clearBefore */);
            if (_auxList.Count <1)
            {
                txtOutput.AppendText(Environment.NewLine + "List of selectable resources is empty." + Environment.NewLine + Environment.NewLine);
            } else
            {
                txtOutput.AppendText(Environment.NewLine + "List of selectable resources: " + Environment.NewLine);
                for (int i = 0; i < _auxList.Count; ++i)
                {
                    txtOutput.AppendText("  " + (i+1).ToString("00") + ": " + _auxList[i] + Environment.NewLine);
                }
            }

            txtOutput.SelectionStart = 0;

        }


        #endregion Data


        #region Viewers


        /// <summary>Returns the number of viewers installed.</summary>
        public int NumViewers
        {
            get { return Viewers.Count; }
        }


        protected IFileViewer _activeViewer = null;

        protected IFileViewer ActiveViewer
        {
            get { return _activeViewer; }
            set { _activeViewer = value; }
        }

        protected List<IFileViewer> _viewers = null; 

        protected List<IFileViewer> Viewers
        {
            get
            {
                if (_viewers == null)
                {
                    _viewers = new List<IFileViewer>();
                    if (ImageViewer != null)
                        AddViewer(ImageViewer, true /* atBeginning */);

                    if (FileViewer != null)
                        AddViewer(FileViewer, false /* atBeginning */);
                }
                return _viewers;
            }
        }

        /// <summary>Adds a new viewer to the list of viewers.</summary>
        /// <param name="viewer">Viewer to be added.</param>
        /// <param name="atBeginning">If true then viewer is added at the beginning, meanig it will have higher prority
        /// and will be used even if there are other viewers installed for the same kind of resources.</param>
        public void AddViewer(IFileViewer viewer, bool atBeginning = true)
        {
            if (viewer == null)
                throw new ArgumentException("Resource viewer not specified (null reference).");
            if (Viewers.Contains(viewer))
            {
                Viewers.Remove(viewer);
            }
            if (atBeginning)
            {
                Viewers.Insert(0, viewer);
            } else
            {
                Viewers.Add(viewer);
            }
            Control viewerControl = viewer as Control;
            if (viewerControl != null)
            {
                // If viewer is a control, make sure that it is also aded to the corresponding container control:
                if (!splitContainer1.Panel1.Controls.Contains(viewerControl))
                {
                    splitContainer1.Panel1.Controls.Add(viewerControl);
                }
            }
        }

        /// <summary>Makes all viewers invisible.</summary>
        protected void HideViewers()
        {
            ActiveViewer = null;
            foreach (Control control in splitContainer1.Panel1.Controls)
            {
                control.Visible = false;
            }
        }



        IFileViewer _imageViewer = null;

        /// <summary>Gets or set internal image viewer component (which is used if other viewers can not
        /// be used for viewing the current resource).</summary>
        protected IFileViewer ImageViewer
        {
            get {
                if (_imageViewer == null)
                    _imageViewer = imageViewerControl1;
                return _imageViewer;
            }
            set
            {
                if (value != _imageViewer)
                {
                    if (Viewers.Contains(_imageViewer))
                    {
                        Viewers.Remove(_imageViewer);
                    }
                    if (_imageViewer != null)
                        _imageViewer.Clear();
                    _imageViewer = value;
                    if (value != null)
                    {
                        AddViewer(_imageViewer, true /* atBeginning */);  
                    }
                }
            }
        }

        IFileViewer _fileViewer = null;

        /// <summary>Gets or set internal general file viewer component (which is used if other viewers can not
        /// be used for viewing the current resource).</summary>
        protected IFileViewer FileViewer
        {
            get {
                if (_fileViewer == null)
                    _fileViewer = fileViewerControl1;
                return _fileViewer;
            }
            set
            {
                if (value != _fileViewer)
                {
                    if (Viewers.Contains(_fileViewer))
                    {
                        Viewers.Remove(_fileViewer);
                    }
                    if (_fileViewer != null)
                        _fileViewer.Clear();
                    _fileViewer = value;
                    if (value != null)
                    {
                        AddViewer(_fileViewer, false /* atBeginning */); 
                    }
                }
            }
        }


        #endregion Viewers


        #region Viewing



        protected bool _isShownImmediately = true;

        /// <summary>Flag that specifies whether content is shown immediately after the viewed object
        /// is set.
        /// <para>If set to true and the viewer does not at all have such capability, this should NOT throw
        /// an exception (thus, user should check success by verifying if flag changed).</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsShownImmediately
        {
            get { return _isShownImmediately; }
            set
            {
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
            HideViewers();

            //for (int i = 0; i < Viewers.Count; ++i)
            //{
            //    Control viewerControl = Viewers[i] as Control;
            //    if (viewerControl != null)
            //        viewerControl.Visible = false;
            //}
            string shownResource = resourceSelector1.SelectedResource;
            string resourceAssemblyName = null;
            Assembly resourceAssembly = null;
            string resourceAddress = null;
            Stream resourceStream = null;
            if (!string.IsNullOrEmpty(shownResource))
            {
                string[] splitStrings = shownResource.Split(UtilSystem.AssemblySeparator);
                if (splitStrings != null)
                {
                    if (splitStrings.Length > 1)
                    {
                        resourceAssemblyName = splitStrings[0];
                        resourceAddress = splitStrings[splitStrings.Length - 1];
                    }
                    if (! string.IsNullOrEmpty(resourceAssemblyName) && ! string.IsNullOrEmpty(resourceAddress))
                    {
                        resourceAssembly = UtilSystem.GetAssemblyByName(resourceAssemblyName);
                        if (resourceAssembly != null)
                        {
                            resourceStream = resourceAssembly.GetManifestResourceStream(resourceAddress);
                        }
                    }
                }
                if (resourceStream == null)
                {
                    Console.WriteLine(Environment.NewLine + "ERROR: could not open resource: " 
                        + shownResource + Environment.NewLine);
                } else
                {

                    Console.WriteLine(Environment.NewLine + "Viewing resource "
                        + shownResource + " ..." + Environment.NewLine);
                    for (int i = 0; i < Viewers.Count; ++i)
                    {
                        IFileViewer viewer = Viewers[i];
                        Control viewerControl = viewer as Control;
                        if (viewer == null)
                        {
                            Console.WriteLine("Viewer " + i + ": null.");
                        } else
                        {
                            if (!viewer.IsEligibleForViewing(resourceStream))
                            {
                                Console.WriteLine("Viewer " + i + ": not eligible, type: " + viewer.GetType());
                            } else
                            {
                                if (viewerControl == null)
                                    Console.WriteLine("Viewer " + i + " used, NOT a control, type: " + viewer.GetType());
                                else
                                    Console.WriteLine("Viewer " + i + " used, type: " + viewer.GetType());
                                if (viewerControl != null)
                                {
                                    viewerControl.Visible = true;
                                    // if (! )
                                    viewerControl.Dock = DockStyle.Fill;
                                }

                                ActiveViewer = viewer;

                                // $$$$
                                if (true)
                                {
                                    if ((resourceStream) != null)
                                    {
                                        resourceStream.Close();
                                    }
                                          
                                    resourceStream = resourceAssembly.GetManifestResourceStream(resourceAddress);
                                }

                                viewer.ViewedStream = resourceStream;
                                viewer.ShowViewedItem();

                                break;  /* we have found a convenient viewer, don't continue */ 
                            }
                        }
                    }
                }
            }
        }



        #endregion Viewing




        private void resourceSelector1_ResourceSelected(object sender, EventArgs e)
        {
            RefreshOutput();

            ShowViewedItem();

            Console.WriteLine(Environment.NewLine + "Resource selected: " 
                + resourceSelector1.SelectedResource + Environment.NewLine); 

        }

        private void resourceSelector1_SelectionParametersChanged(object sender, EventArgs e)
        {
            RefreshOutput();

            Console.WriteLine(Environment.NewLine + "Resource selection parameters changed." + Environment.NewLine);

        }
    }
}
