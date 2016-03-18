// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Reflection;

using IG.Lib;

namespace IG.Forms
{
    public partial class AssemblyInfoControl : UserControl
    {
        public AssemblyInfoControl()
        {
            InitializeComponent();
            RefreshOutput();
        }


        #region Data



        private static string _initialTextOutpput =
@"<< Select an assembly to view its information! >>
<< You can drag & drop assembly files into this area. >>
<< Press ""Refresh"" if necessary. >>";

        /// <summary>Initial text written on the label that shows the long name of the resource.</summary>
        public static string InitialTextOutput
        { get { return _initialTextOutpput; } }

        private string _selectedResource = null;

        private string SelectedResource
        {
            get { return _selectedResource; }
            set
            {
                if (value != _selectedResource)
                {
                    _selectedResource = value;
                    if (value == null)
                    {
                        //lblSelectedResource.Text = InitialTextLabelFullName;
                        //comboResources.Text = InitialTextComboResources;
                    } else
                    {
                        //lblSelectedResource.Text = _selectedResource;
                        //comboResources.Text = null;
                    }
                }
            }
        }

        private bool _multipleAssembliesAllowed = false;

        /// <summary>Whether mulltiple assemblies are allowed.</summary>
        public bool MultipleAssembliesAllowed
        {
            get { return _multipleAssembliesAllowed; }
            set
            {
                if (value != _multipleAssembliesAllowed)
                {
                    _multipleAssembliesAllowed = value;
                    chkMultipleAssemblies.Checked = value;
                    RefreshOutput();
                }
            }
        }



        // int _originalScrolPosition = 0;

        /// <summary>Refreshed information about the selected assembly (of potential assemblied if multiple assemblies
        /// descriptions are allowed) on the output text control.</summary>
        public void RefreshOutput()
        {
            int _originalScrolPosition = txtOutput.SelectionStart;
            txtOutput.Clear();
            Assembly asm = assemblySelector1.SelectedAssembly;
            if (asm == null)
            {
                if (!MultipleAssembliesAllowed)
                    txtOutput.AppendText(InitialTextOutput + Environment.NewLine);
            } else
            {
                // In any case, write the selected assembly first, provided that one exists:
                txtOutput.AppendText("Selected assembly: " + Environment.NewLine + UtilSystem.GetAssemblyInfo(asm) + Environment.NewLine);
            }
            {
                if (MultipleAssembliesAllowed) // TODO: change condition to multiple 
                {
                    // We can print more than one assembly:
                    if (assemblySelector1.FileSelectedAssembly != null)
                        if (assemblySelector1.FileSelectedAssembly != assemblySelector1.SelectedAssembly)
                    {
                        txtOutput.AppendText("Assembly loaded from the file system: " + Environment.NewLine
                            + UtilSystem.GetAssemblyInfo(assemblySelector1.FileSelectedAssembly) + Environment.NewLine);
                    }
                    if (assemblySelector1.SelectFromLoadedAssemblies)
                    {
                        Assembly[] asmLoaded = assemblySelector1.GetLoadedAssembliesArray();
                        if (asmLoaded != null && asmLoaded.Length > 0)
                        {
                            txtOutput.AppendText("==========" + Environment.NewLine + "Loaded assemblies: " + Environment.NewLine + Environment.NewLine);
                            for (int i = 0; i < asmLoaded.Length; ++i)
                            {
                                txtOutput.AppendText("  Loaded assembly No. " + i.ToString() + ": " + Environment.NewLine 
                                    + UtilSystem.GetAssemblyInfo(asmLoaded[i]) + Environment.NewLine);
                            }
                        }
                    }
                    if (assemblySelector1.SelectFromReferencedAssemblies)
                    {
                        Assembly[] asmReferenced = assemblySelector1.GetReferencedAssembliesArray();
                        if (asmReferenced != null && asmReferenced.Length > 0)
                        {
                            txtOutput.AppendText("==========" + Environment.NewLine + "Referenced assemblies: " + Environment.NewLine + Environment.NewLine);
                            for (int i = 0; i < asmReferenced.Length; ++i)
                            {
                                txtOutput.AppendText("  Referenced assembly No. " + i.ToString() + ": " + Environment.NewLine 
                                    + UtilSystem.GetAssemblyInfo(asmReferenced[i]) + Environment.NewLine);
                            }
                        }
                    }
                }
            }
            txtOutput.SelectionStart = _originalScrolPosition;
            txtOutput.ScrollToCaret();
        }



        #endregion Data
        


        private void chkMultipleAssemblies_CheckedChanged(object sender, EventArgs e)
        {
            MultipleAssembliesAllowed = chkMultipleAssemblies.Checked;
        }

        private void btnRefreshOutput_Click(object sender, EventArgs e)
        {
            RefreshOutput();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
            assemblySelector1.SelectedAssembly = null;
            txtOutput.Text = InitialTextOutput;
        }

        private void assemblySelector1_AssemblySelected(object sender, EventArgs e)
        {
            this.RefreshOutput();
        }

        private void assemblySelector1_SelectionParametersChanged(object sender, EventArgs e)
        {
            this.RefreshOutput();
        }




        private void txtOutput_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;

            Console.WriteLine("DragEnter occurred.");

        }

        private void txtOutput_DragDrop(object sender, DragEventArgs e)
        {

            Console.WriteLine("Somethong was dropped.");
            try
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
                        string selectedPath = droppedFilePath;
                        assemblySelector1.SelectedAssembly = Assembly.LoadFile(droppedFilePath);
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }


    }
}
