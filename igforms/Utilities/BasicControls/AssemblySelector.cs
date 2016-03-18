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
    public partial class AssemblySelector : UserControl
    {
        public AssemblySelector()
        {
            InitializeComponent();
            txtSelectedAssembly.Text = InitialTextSelectedAssembly;
            comboReferencedAssemblies.Text = InitialTextReferencedList;
            comboLoadedAssemblies.Text = InitialTextLoadedList;
            lblFullName.Text = InitialTextLblFullName;

            chkFiles.Checked = SelectFromFiles;     fileSelector1.Enabled = SelectFromFiles;
            chkLoaded.Checked = SelectFromLoadedAssemblies;   comboLoadedAssemblies.Enabled = SelectFromFiles;
            chkReferenced.Checked = SelectFromReferencedAssemblies;   comboReferencedAssemblies.Enabled = SelectFromFiles;
        }

        #region Constants 

        private static string _initialTextSelectedAssembly = "<< Type assembly name or select in lists below. >>";

        /// <summary>Initial text that is written in the textbox where loaded assembly is written.</summary>
        public static string InitialTextSelectedAssembly
        { get { return _initialTextSelectedAssembly; } set { _initialTextSelectedAssembly = value; } }


        private static string _initialTextLoadedList = "<< Loaded... >>";

        /// <summary>Initial text that is written in the combobox with a list of loaded assemblies.</summary>
        public static string InitialTextLoadedList
        { get { return _initialTextLoadedList; } set { _initialTextLoadedList = value; } }


        private static string _initialTextReferencedList = "<< Referenced... >>";

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
                        txtSelectedAssembly.Text = UtilSystem.GetAssemblyName(_asembly);  // _asembly.GetName().Name;
                        fileSelector1.FilePath = this.SelectedAssembly.Location;  //.Location;
                        lblFullName.Text = _asembly.FullName;

                    }
                    OnAssemblySelected();
                }
            }
        }


        Assembly _fileSelectedAssembly = null;

        /// <summary>The last assembly that was selected in the file sistem (no matter whether this is still the selected
        /// assembly or not).</summary>
        public Assembly FileSelectedAssembly
        {
            get { return _fileSelectedAssembly; }
            protected set { _fileSelectedAssembly = value; }
        }

        private bool _selectFromFiles;

        /// <summary>Indicates whether assemblies can also be loaded from the file system. If false then assemblies
        /// can only be selected from the lists of loaded and / or referenced assemblies.</summary>
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
                    OnSelectionParametersChanged();
                }
            }
        }


        private bool _selectFromLoadedAssemblies = true;

        /// <summary>Indicates whether assemblies can also be selected from the list of loaded assemblies.</summary>
        public bool SelectFromLoadedAssemblies
        {
            get { return _selectFromLoadedAssemblies; }
            set
            {
                if (value != _selectFromLoadedAssemblies)
                {
                    _selectFromLoadedAssemblies = value;
                    chkLoaded.Checked = _selectFromLoadedAssemblies;
                    if (_selectFromLoadedAssemblies)
                    {
                        comboLoadedAssemblies.Enabled = true;
                        if (LoadedAssembliesList.Count < 1)
                            RefreshLoadedAssembliesList();
                    }
                    else
                    {
                        comboLoadedAssemblies.Enabled = false;
                    }
                    OnSelectionParametersChanged();
                }
            }
        }

        private bool _selectFromReferencedAssemblies = false;

        /// <summary>Indicates whether assemblies can also be selected from the list of referenced assemblies.</summary>
        public bool SelectFromReferencedAssemblies
        {
            get { return _selectFromReferencedAssemblies; }
            set
            {
                if (value != _selectFromReferencedAssemblies)
                {
                    _selectFromReferencedAssemblies = value;
                    chkReferenced.Checked = _selectFromReferencedAssemblies;
                    if (_selectFromReferencedAssemblies)
                    {
                        comboReferencedAssemblies.Enabled = true;
                        if (ReferencedAssembliesList.Count < 1)
                            RefreshReferencedAssembliesList();
                    }
                    else
                    {
                        comboReferencedAssemblies.Enabled = false;
                    }
                    OnSelectionParametersChanged();
                }
            }
        }



        private List<Assembly> _loadedAssembliesList = new List<Assembly>();

        /// <summary>List of all loaded assemblies.</summary>
        protected List<Assembly> LoadedAssembliesList { get { return _loadedAssembliesList; } }

        private List<Assembly> _referencedAssembliesList = new List<Assembly>();

        /// <summary>List of all referenced assemblies.</summary>
        protected List<Assembly> ReferencedAssembliesList { get { return _referencedAssembliesList; } }

        /// <summary>Re-creates the list of loaded assemblies.</summary>
        protected void RefreshLoadedAssembliesList()
        {
            UtilSystem.GetLoadedAssemblies(LoadedAssembliesList, true  /* clearBefore */);
            comboLoadedAssemblies.Items.Clear();
            for (int i = 0; i < LoadedAssembliesList.Count; ++i)
                comboLoadedAssemblies.Items.Add(UtilSystem.GetAssemblyName(LoadedAssembliesList[i]));  //LoadedAssembliesList[i].GetName().Name);
            comboLoadedAssemblies.Text = InitialTextLoadedList;
            OnSelectionParametersChanged();
        }


        /// <summary>Re-creates the list of referenced assemblies.</summary>
        protected void RefreshReferencedAssembliesList()
        {
            UtilSystem.GetReferencedAssemblies(ReferencedAssembliesList, true  /* clearBefore */);
            for (int i = 0; i < ReferencedAssembliesList.Count; ++i)
                comboReferencedAssemblies.Items.Add(UtilSystem.GetAssemblyName(ReferencedAssembliesList[i]));    //ReferencedAssembliesList[i].GetName().Name);
            comboReferencedAssemblies.Text = InitialTextReferencedList;
            OnSelectionParametersChanged();
        }


        /// <summary>Returns an array of all loaded assemblies.</summary>
        public Assembly[] GetLoadedAssembliesArray()
        { return LoadedAssembliesList.ToArray(); }

        /// <summary>Stores all loaded assemblies to the specified list and returns the list.
        /// <para>The method takes care that assemblies stored are unique.</para></summary>
        /// <param name="assemblies">List where assemblies are stored.</param>
        /// <param name="clearBefore">Whether the list is cleared at the beginnning of operation.</param>
        /// <returns>The list where assemblies are stored. This enables one to put null list as argument.</returns>
        public IList<Assembly> GetLoadedAssemblies(IList<Assembly> assemblies = null, bool clearBefore = true)
        {
            if (assemblies == null)
                assemblies = new List<Assembly>();
            else
            {
                if (clearBefore)
                    assemblies.Clear();
            }
            int num = 0;
            if (LoadedAssembliesList != null)
            {
                num = LoadedAssembliesList.Count;
                for (int i = 0; i < num; ++i)
                {
                    if (!assemblies.Contains(LoadedAssembliesList[i]))
                        assemblies.Add(LoadedAssembliesList[i]);
                }
            }
            return assemblies;
        }

        /// <summary>Returns an array of all referenced assemblies.</summary>
        public Assembly[] GetReferencedAssembliesArray()
        { return ReferencedAssembliesList.ToArray(); }

        /// <summary>Stores all referenced assemblies to the specified list and returns the list.</summary>
        /// <param name="assemblies">List where assemblies are stored.</param>
        /// <param name="clearBefore">Whether the list is cleared at the beginnning of operation.</param>
        /// <returns>The list where assemblies are stored. This enables one to put null list as argument.</returns>
        public IList<Assembly> GetReferencedAssemblies(IList<Assembly> assemblies = null, bool clearBefore = true)
        {
            if (assemblies == null)
                assemblies = new List<Assembly>();
            else
            {
                if (clearBefore)
                    assemblies.Clear();
            }
            int num = 0;
            if (ReferencedAssembliesList != null)
            {
                num = ReferencedAssembliesList.Count;
                for (int i = 0; i < num; ++i)
                {
                    if (!assemblies.Contains(ReferencedAssembliesList[i]))
                        assemblies.Add(ReferencedAssembliesList[i]);
                }
            }
            return assemblies;
        }

        /// <summary>Stores all selectable assemblies to the specified list and returns the list.
        /// <para>The method takes care that assemblies stored are unique. If an assembly is currently selected then it is added
        /// to the list first.</para>
        /// <para>This takes into account which kinds of assemblies can be selected.</para></summary>
        /// <param name="assemblies">List where assemblies are stored.</param>
        /// <param name="clearBefore">Whether the list is cleared at the beginnning of operation.</param>
        /// <returns>The list where assemblies are stored. This enables one to put null list as argument.</returns>
        public IList<Assembly> GetSelectableAssemblies(IList<Assembly> assemblies = null, bool clearBefore = true)
        {
            if (assemblies == null)
                assemblies = new List<Assembly>();
            else
            {
                if (clearBefore)
                    assemblies.Clear();
            }
            if (SelectedAssembly != null)
            {
                if (!assemblies.Contains(SelectedAssembly))
                    assemblies.Add(SelectedAssembly);
            }
            if (SelectFromLoadedAssemblies)
                GetLoadedAssemblies(assemblies, false /* clearBefore */);
            if (SelectFromReferencedAssemblies)
                GetReferencedAssemblies(assemblies, false /* clearBefore */);
            return assemblies;
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


        /// <summary>Event that is fired whenever the selection parameters change (e.g., whether assemblies can be
        /// selected from the file system, or from the set of loaded assemblies, or from the set of assemblies referenced 
        /// by the current assembly).</summary>
        public event EventHandler SelectionParametersChanged;

        /// <summary>Must be called whenever the selection parameters change.</summary>
        protected virtual void OnSelectionParametersChanged()
        {
            if (SelectionParametersChanged != null)
                SelectionParametersChanged(this, new EventArgs());
        }

        #endregion Events


        /// <summary>Executed when the current DLL selector control is loaded.</summary>
        private void AssemblySelector_Load(object sender, EventArgs e)
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
            Assembly asm = LoadedAssembliesList[comboLoadedAssemblies.SelectedIndex]; 
            if (asm != null)
            {
                comboLoadedAssemblies.Text = UtilSystem.GetAssemblyName(asm); // asm.GetName().Name;
                comboReferencedAssemblies.Text = InitialTextReferencedList;
                SelectedAssembly = asm;
            }

        }

        private void comboReferencedAssemblies_SelectedIndexChanged(object sender, EventArgs e)
        {
            Assembly asm = ReferencedAssembliesList[comboReferencedAssemblies.SelectedIndex];
            if (asm != null)
            {
                comboReferencedAssemblies.Text = UtilSystem.GetAssemblyName(asm); // asm.GetName().Name;
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
                FileSelectedAssembly = asm;
                SelectedAssembly = asm;
            }
        }

        private void chkFiles_CheckedChanged(object sender, EventArgs e)
        {
            SelectFromFiles = chkFiles.Checked;
        }

        private void chkLoaded_CheckedChanged(object sender, EventArgs e)
        {
            SelectFromLoadedAssemblies = chkLoaded.Checked;
        }

        private void chkReferenced_CheckedChanged(object sender, EventArgs e)
        {
            SelectFromReferencedAssemblies = chkReferenced.Checked;
        }
    }
}
