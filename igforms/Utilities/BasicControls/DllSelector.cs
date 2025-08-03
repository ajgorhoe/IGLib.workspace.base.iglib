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
    public partial class DllSelector : UserControl
    {
        public DllSelector()
        {
            InitializeComponent();
            txtSelectedAssembly.Text = InitialTextSelectedAssembly;
            comboReferencedAssemblies.Text = InitialTextReferencedList;
            comboLoadedAssemblies.Text = InitialTextLoadedList;
            lblFullName.Text = InitialTextLblFullName;
        }

        #region Constants 

        private static string _initialTextSelectedAssembly = "<< Type assembly name or select in lists below. >>";

        /// <summary>Initial text that is written in the textbox where loaded assembly is written.</summary>
        public static string InitialTextSelectedAssembly
        { get { return _initialTextSelectedAssembly; } set { _initialTextSelectedAssembly = value; } }


        private static string _initialTextLoadedList = "<< Loaded assemblies >>";

        /// <summary>Initial text that is written in the combobox with a list of loaded assemblies.</summary>
        public static string InitialTextLoadedList
        { get { return _initialTextLoadedList; } set { _initialTextLoadedList = value; } }


        private static string _initialTextReferencedList = "<< Referenced assemblies >>";

        /// <summary>Initial text that is written in the combobox with a list of referenced assemblies.</summary>
        public static string InitialTextReferencedList
        { get { return _initialTextReferencedList; } set { _initialTextReferencedList = value; } }


        private static string _initialTextLblFullName = "<< Assembly not selected. >>";

        /// <summary>Initial text that is written on the label that shows full name of the selected assembly.</summary>
        public static string InitialTextLblFullName
        { get { return _initialTextLblFullName; } set { _initialTextLblFullName = value; } }


        #endregion Constants


        #region Data


        Assembly _asembly;


        /// <summary>Selected assembly.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Assembly SelectedAssembly
        {
            get { return _asembly; }
            set {
                if (value != _asembly)
                {
                    _asembly = value;
                    if (_asembly == null)
                    {
                        txtSelectedAssembly.Text = InitialTextSelectedAssembly;
                        comboLoadedAssemblies.Text = InitialTextLoadedList;
                        comboReferencedAssemblies.Text = InitialTextReferencedList;
                        lblFullName.Text = InitialTextLblFullName;
                        fileSelector1.FilePath = null;
                    } else
                    {
                        txtSelectedAssembly.Text = _asembly.GetName().Name;
                        fileSelector1.FilePath = this.SelectedAssembly.Location;  //.Location;
                        lblFullName.Text = _asembly.FullName;

                    }
                    OnAssemblySelected();
                }
            }
        }


        private bool _selectFromFiles;

        /// <summary>Indicates whether assemblies can also be loaded from the file system. If false then assemblies
        /// can only be selected from the lists of loaded and / or referenced assemblies.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SelectFromFiles
        {
            get { return _selectFromFiles; }
            set
            {
                if (value != _selectFromFiles)
                {
                    _selectFromFiles = value;
                    chkFiles.Checked = _selectFromFiles;
                    if (_selectFromFiles)
                    {
                        fileSelector1.Enabled = true;
                    }
                    else
                    {
                        fileSelector1.Enabled = false;
                    }
                }
            }
        }


        private bool _selectFromLoadedAssemblies;

        /// <summary>Indicates whether assemblies can also be selected from the list of loaded assemblies.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SelectFromLoadedAssemblies
        {
            get { return _selectFromLoadedAssemblies; }
            set
            {
                if (value != _selectFromLoadedAssemblies)
                {
                    _selectFromLoadedAssemblies = value;
                    chkFiles.Checked = _selectFromLoadedAssemblies;
                    if (_selectFromLoadedAssemblies)
                    {
                        comboLoadedAssemblies.Enabled = true;
                    }
                    else
                    {
                        comboLoadedAssemblies.Enabled = false;
                    }
                }
            }
        }

        private bool _selectFromReferencedAssemblies;

        /// <summary>Indicates whether assemblies can also be selected from the list of referenced assemblies.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SelectFromReferencedAssemblies
        {
            get { return _selectFromReferencedAssemblies; }
            set
            {
                if (value != _selectFromReferencedAssemblies)
                {
                    _selectFromReferencedAssemblies = value;
                    chkFiles.Checked = _selectFromReferencedAssemblies;
                    if (_selectFromReferencedAssemblies)
                    {
                        comboReferencedAssemblies.Enabled = true;
                    }
                    else
                    {
                        comboReferencedAssemblies.Enabled = false;
                    }
                }
            }
        }


        private List<Assembly> _loadedAssembliesList = new List<Assembly>();

        /// <summary>List of all loaded assemblies.</summary>
        protected List<Assembly> LoadedAssembliesList { get { return _loadedAssembliesList; } }

        /// <summary>Returns an array of all loaded assemblies.</summary>
        public Assembly[] GetLoadedAssemblies()
        { return LoadedAssembliesList.ToArray(); }

        private List<Assembly> _referencedAssembliesList = new List<Assembly>();

        /// <summary>List of all referenced assemblies.</summary>
        protected List<Assembly> ReferencedAssembliesList { get { return _referencedAssembliesList; } }

        /// <summary>Returns an array of all referenced assemblies.</summary>
        public Assembly[] GetReferencedAssemblies()
        { return ReferencedAssembliesList.ToArray(); }


        /// <summary>Re-creates the list of loaded assemblies.</summary>
        public void RefreshLoadedAssembliesList()
        {
            UtilSystem.GetLoadedAssemblies(LoadedAssembliesList, true  /* clearBefore */);
            comboLoadedAssemblies.Items.Clear();
            for (int i = 0; i < LoadedAssembliesList.Count; ++i)
                comboLoadedAssemblies.Items.Add(LoadedAssembliesList[i]);
            comboLoadedAssemblies.Text = InitialTextLoadedList;
        }


        /// <summary>Re-creates the list of referenced assemblies.</summary>
        public void RefreshReferencedAssembliesList()
        {
            UtilSystem.GetReferencedAssemblies(LoadedAssembliesList, true  /* clearBefore */);
            for (int i = 0; i < ReferencedAssembliesList.Count; ++i)
                comboReferencedAssemblies.Items.Add(ReferencedAssembliesList[i]);
            comboReferencedAssemblies.Text = InitialTextReferencedList;
        }


        #endregion Data


        #region Events


        /// <summary>Event that is fired whenever the selected assembly changes.</summary>
        public event EventHandler AssemblySelected;

        /// <summary>Called whenever the selected assembly changes.</summary>
        protected virtual void OnAssemblySelected()
        {
            if (AssemblySelected != null)
                AssemblySelected(this, new EventArgs());
        }

        #endregion Events

        /// <summary>Executed when the current DLL selector control is loaded.</summary>
        private void DllSelector_Load(object sender, EventArgs e)
        {
            // Obtain the lists of referenced and loaded assemblies, if necessary:
            if (SelectFromLoadedAssemblies && LoadedAssembliesList.Count < 1)
                RefreshLoadedAssembliesList();
            if (SelectFromReferencedAssemblies && ReferencedAssembliesList.Count < 1)
                RefreshReferencedAssembliesList();
        }



        private void txtSelectedAssembly_TextChanged(object sender, EventArgs e)
        {
            string assemblyName = txtSelectedAssembly.Text;
            Assembly asm = UtilSystem.GetAssemblyByName(assemblyName, false /* caseSensitive */, true /* loadIfNecessary  */,
                false /* byFileName */);
            if (asm != null)
            {
                SelectedAssembly = asm;
                comboLoadedAssemblies.Text = InitialTextLoadedList;
                comboReferencedAssemblies.Text = InitialTextReferencedList;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (SelectFromLoadedAssemblies)
                RefreshLoadedAssembliesList();
            if (SelectFromReferencedAssemblies)
                RefreshReferencedAssembliesList();
        }

        private void comboLoadedAssemblies_SelectedIndexChanged(object sender, EventArgs e)
        {
            Assembly asm = comboLoadedAssemblies.Items[comboLoadedAssemblies.SelectedIndex] as Assembly;
            if (asm != null)
            {
                comboLoadedAssemblies.Text = asm.GetName().Name;
                comboReferencedAssemblies.Text = InitialTextReferencedList;
                SelectedAssembly = asm;
            }

        }

        private void comboReferencedAssemblies_SelectedIndexChanged(object sender, EventArgs e)
        {
            Assembly asm = comboReferencedAssemblies.Items[comboReferencedAssemblies.SelectedIndex] as Assembly;
            if (asm != null)
            {
                comboReferencedAssemblies.Text = asm.GetName().Name;
                comboLoadedAssemblies.Text = InitialTextLoadedList;
                SelectedAssembly = asm;
            }
        }

        private void fileSelector1_FileSelected(object sender, EventArgs e)
        {
            Assembly asm = Assembly.LoadFile(fileSelector1.FilePath);
            if (asm != null)
            {
                comboReferencedAssemblies.Text = InitialTextReferencedList;
                comboLoadedAssemblies.Text = InitialTextLoadedList;
                SelectedAssembly = asm;
            }
        }

        private void chkFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFiles.Checked)
                SelectFromFiles = true;
            else
                SelectFromFiles = false;
        }

    }
}
