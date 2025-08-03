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
    public partial class ResourceSelector : UserControl
    {
        public ResourceSelector()
        {
            InitializeComponent();
            try
            {
                this.Enabled = false;
                chkEmbedded.Checked = SelectFromEmbeddedResources;
                chkResx.Checked = SelectFromResxResources;
                chkIncludeResxFiles.Checked = IncludeResourceFiles;
                chkMultipleAssemblies.Checked = MultipleAssembliesAllowed;
                this.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine + "ERROR: Exception thrown in resource selector control when created." + Environment.NewLine
                    + "  Details: " + ex.Message + Environment.NewLine);
                throw;
            }
        }


        #region Data


        /// <summary>Returns reference to assembly selector.</summary>
        protected AssemblySelector GetAssemblySeclector()
        { return assemblySelector1; }

        /// <summary>Whether resources can be seleted from assembly files. Delegated to assembly selector
        /// (property <see cref="AssemblySelector.SelectFromFiles"/>).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SelectFromAssemblyFiles
        {
            get { return assemblySelector1.SelectFromFiles; }
            set { assemblySelector1.SelectFromFiles = value; }
        }

        /// <summary>Whether resources can be seleted from loaded assemblies. Delegated to assembly selector.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SelectFromLoadedAssemblies
        {
            get { return assemblySelector1.SelectFromLoadedAssemblies; }
            set { assemblySelector1.SelectFromLoadedAssemblies = value; }
        }

        /// <summary>Whether resources can be seleted from referenced assemblies. Delegated to assembly selector.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SelectFromReferencedAssemblies
        {
            get { return assemblySelector1.SelectFromReferencedAssemblies; }
            set { assemblySelector1.SelectFromReferencedAssemblies = value; }
        }


        private static string _initialTextComboShortName = "<< Select a resource! >>";

        /// <summary>Initial text written on the combo box for resource selection.</summary>
        public static string InitialTextComboResources
        { get { return _initialTextComboShortName; } }


        private static string _initialTextLabelFullName = "<< Resource is not selected. >>";

        /// <summary>Initial text written on the label that shows the long name of the resource.</summary>
        public static string InitialTextLabelFullName
        { get { return _initialTextLabelFullName; } }



        private string _selectedResource = null;

        public string SelectedResource
        {
            get { return _selectedResource; }
            private set
            {
                if (value != _selectedResource)
                {
                    _selectedResource = value;
                    if (value == null)
                    {
                        lblSelectedResource.Text = InitialTextLabelFullName;
                        comboResources.Text = InitialTextComboResources;
                        lblSelectedResource.Text = InitialTextLabelFullName;
                    } else
                    {
                        lblSelectedResource.Text = _selectedResource;
                        comboResources.Text = value;
                        lblSelectedResource.Text = value;
                    }
                    OnResourceSelected();
                }
            }
        }



        private bool _multipleAssembliesAllowed = false;

        /// <summary>Ehether mulltiple assemblies are allowed. 
        /// <para>If true then resouces can be selected from all assemblies that are eligible for selection in the 
        /// assembly sleector (type <see cref="AssemblySelector"/>)</para>.
        /// <para>If false then resouces can be selected only from the (single) selected assembly.</para>.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool MultipleAssembliesAllowed
        {
            get { return _multipleAssembliesAllowed; }
            set {
                if (value != _multipleAssembliesAllowed)
                {
                    _multipleAssembliesAllowed = value;
                    chkMultipleAssemblies.Checked = value;
                    // We need to refresh all resources lists if this flag changes:
                    RefreshResourcesLists();  // appropriate events will also be raised here, and other dependencies will be handled as necessary
                }
            }
        }

        
        private bool _selectFromEmbeddedResources = true;

        /// <summary>Indicates whether embedded resources (compoiled as files) are included in listing.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SelectFromEmbeddedResources
        {
            get { return _selectFromEmbeddedResources; }
            set
            {
                if (value != _selectFromEmbeddedResources)
                {
                    _selectFromEmbeddedResources = value;
                    chkEmbedded.Checked = _selectFromEmbeddedResources;
                    if (_selectFromEmbeddedResources)
                    {
                        RefreshEmbeddedResourcesList();
                    }
                    OnSelectionParametersChanged();
                }
            }
        }


        private bool _includeResourceFiles = true;

        /// <summary>Indicates whether resource files (generated from .resx files and compiled into .resources files
        /// in assemblies) are listed as embedded resources.
        /// <para>If false then only resources included in these files can be selected (provided that the 
        /// <see cref="SelectFromResxResources"/> propery is true) as resources, but not their containig files themselves.</para></summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IncludeResourceFiles
        {
            get { return _includeResourceFiles; }
            set
            {
                if (value != _includeResourceFiles)
                {
                    _includeResourceFiles = value;
                    chkIncludeResxFiles.Checked = _includeResourceFiles;
                    RefreshEmbeddedResourcesList();
                    OnSelectionParametersChanged();
                }
            }
        }

        private bool _selectFromResxResources = true;

        /// <summary>Indicates whether resources that are bundled in resource files are included in listing.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SelectFromResxResources
        {
            get { return _selectFromResxResources; }
            set
            {
                if (value != _selectFromResxResources)
                {
                    _selectFromResxResources = value;
                    chkEmbedded.Checked = _selectFromResxResources;
                    RefreshResxResourcesList();
                    OnSelectionParametersChanged();
                }
            }
        }


        private List<Assembly> _selectableAssemblies = new List<Assembly>();

        protected List<Assembly> SelectableAssemblies { get { return _selectableAssemblies; } }


        /// <summary>Refreshes the list of assemblies from which one can select resources.</summary>
        public void RefreshSelectableAssembliesList()
        {
            if (MultipleAssembliesAllowed)
                assemblySelector1.GetSelectableAssemblies(_selectableAssemblies, true  /* clearBefore */);
            else
            {
                _selectableAssemblies.Clear();
                if (assemblySelector1.SelectedAssembly != null)
                    _selectableAssemblies.Add(assemblySelector1.SelectedAssembly);
            }
        }


        


        private List<string> _embeddedResourcesList = new List<string>();

        /// <summary>List of all embedded resources one can select from.</summary>
        protected List<string> EmbeddedResourcesList { get { return _embeddedResourcesList; } }

        private List<string> _resxResourcesList = new List<string>();

        /// <summary>List of all resx-based resources one can select from.</summary>
        protected List<string> ResxResourcesList { get { return _resxResourcesList; } }


        private List<string> _selectableResourcesList = new List<string>();

        /// <summary>List of all referenced assemblies.</summary>
        protected List<string> SelectableResourcesList { get { return _resxResourcesList; } }

        /// <summary>Serves internally as auxiliary list to temporarily store acquired data before further processing.</summary>
        protected List<string> _auxResList = new List<string>();
            

        /// <summary>Refreshes all internal lists of resources eligible for selection.
        /// <para>This will also handle all dependencies and raise the appropriate events.</para>
        /// <para>This method is made public in order to enable users invoke refreshes just for any case, although in most scenarios this
        /// will not be necessary (one case this might be usable is where external circumstances change, e.g. assemblies get loaded or
        /// unloaded).</para></summary>
        public void RefreshResourcesLists()
        {
            RefreshEmbeddedResourcesList(true /* suppressRefreshSelectable */);
            RefreshResxResourcesList(true /* suppressRefreshSelectable */);
            RefreshSelectableResourcesList();  // only done once due to suppress parameter set to true in previous two calls; this will also handle dependencies and raise events
        }

        /// <summary>Refreshes the list of all selectable resources, and also updates the combo box where
        /// resources are selected and triggers the <see cref="SelectionParametersChanged"/> event.
        /// <para>The currently selected resource is updated if necessary (i.e., set to null if the new list of 
        /// selectable resources does not contain it any more), and the appropriate event is triggered (i.e., the 
        /// <see cref="SelectionParametersChanged"/> by call to <see cref="OnSelectionParametersChanged"/>).</para></summary>
        protected void RefreshSelectableResourcesList()
        {
            SelectableResourcesList.Clear();
            bool listContainsSelectedResource = false;
            string current = SelectedResource;
            for (int i = 0; i < EmbeddedResourcesList.Count; ++i)
            {
                string resId = EmbeddedResourcesList[i];
                if (current != null && current == resId)
                {
                    listContainsSelectedResource = true;
                }
                if (!SelectableResourcesList.Contains(resId))
                {
                    SelectableResourcesList.Add(resId);
                }
            }
            for (int i = 0; i < ResxResourcesList.Count; ++i)
            {
                string resId = ResxResourcesList[i];
                if (current != null && current == resId)
                {
                    listContainsSelectedResource = true;
                }
                if (!SelectableResourcesList.Contains(resId))
                {
                    SelectableResourcesList.Add(resId);
                }
            }
            // If the new list does not contain the currently selected resource Id, reset the selected resource Id to null:
            if (!listContainsSelectedResource && current != null)
                SelectedResource = null;
            // Update the combo box according to the new list of selectable resources:
            comboResources.Items.Clear();
            for (int i = 0; i < SelectableResourcesList.Count; ++i)
                comboResources.Items.Add(SelectableResourcesList[i]);
            comboResources.Text = InitialTextComboResources;
            // Call function that fires the apppropriate event (SelectionParametersChanged):
            OnSelectionParametersChanged();
        }

        /// <summary>Re-creates (refreshes) the list of selectable embedded resources.</summary>
        /// <param name="suppressRefreshSelectable">If true then the method does not refresh the list of all selectable
        /// resources (performed by the <see cref="RefreshSelectableResourcesList"/> method) - see method desctiption
        /// for what it does beside refreshing of the list. This is useful when this method is called in connection with 
        /// another partial refresh method, sinnce common list can be refreshed only once after all of these methods are
        /// called.</param>
        protected void RefreshEmbeddedResourcesList(bool suppressRefreshSelectable = false)
        {
            if (_selectableAssemblies.Count < 1)
                RefreshSelectableAssembliesList();
            EmbeddedResourcesList.Clear();
            foreach (Assembly asm in _selectableAssemblies)
            {
                // obtain the list of appropriate resources from the currently treated assembly:
                UtilSystem.GetAssemblyEmbeddedFileResources(asm, _auxResList, true /* clearBefore */,
                    IncludeResourceFiles /* whether resource files should also be listed as (embedded) resources */);
                foreach (string resIdWithoutAssembly in _auxResList)
                {
                    string resId = UtilSystem.GetAssemblyName(asm) + UtilSystem.AssemblySeparator + resIdWithoutAssembly;
                    if (!EmbeddedResourcesList.Contains(resId))
                        EmbeddedResourcesList.Add(resId);
                }
            }
            if (!suppressRefreshSelectable)
            {
                // Upon refresh of selectable embedded resourced, also refresh the list of all selectable resourced
                // (if not suppressed by the argument), which will also refresh the combo box:
                RefreshSelectableResourcesList();
            }
        }

        /// <summary>Re-creates (refreshes) the list of selectable resources that were included into the assembly through
        /// resource file(s) (.resx files compiled into assemblies as .resources files).</summary>
        /// <param name="suppressRefreshSelectable">If true then the method does not refresh the list of all selectable
        /// resources (performed by the <see cref="RefreshSelectableResourcesList"/> method) - see method desctiption
        /// for what it does beside refreshing of the list. This is useful when this method is called in connection with 
        /// another partial refresh method, sinnce common list can be refreshed only once after all of these methods are
        /// called.</param>
        protected void RefreshResxResourcesList(bool suppressRefreshSelectable = false)
        {
            if (_selectableAssemblies.Count < 1)
                RefreshSelectableAssembliesList();
            ResxResourcesList.Clear();
            foreach (Assembly asm in _selectableAssemblies)
            {
                // obtain the list of appropriate resources from the currently treated assembly:
                UtilSystem.GetAssemblyResxResources(asm, _auxResList, true /* clearBefore */);
                foreach (string resIdWithoutAssembly in _auxResList)
                {
                    string resId = UtilSystem.GetAssemblyName(asm) + UtilSystem.AssemblySeparator + resIdWithoutAssembly;
                    if (!ResxResourcesList.Contains(resId))
                        ResxResourcesList.Add(resId);
                }
            }
            if (!suppressRefreshSelectable)
            {
                // Upon refresh of selectable resx-included resourced, also refresh the list of all selectable resourced
                // (if not suppressed by the argument), which will also refresh the combo box:
                RefreshSelectableResourcesList();
            }
        }



        /// <summary>Returns an array of all selectable embedded resources (compiled into assemblies as embedded files).</summary>
        public string[] GetEmbeddedResourcesArray()
        { return EmbeddedResourcesList.ToArray(); }

        /// <summary>Stores to the specified list all embedded resources (i.e. resources that were compiled into assemblies 
        /// as embedded files) eligible for selection, and returns the list.
        /// <para>The method takes care that resource identifires stored are unique.</para></summary>
        /// <param name="resourceList">List where resources are stored.</param>
        /// <param name="clearBefore">Whether the list is cleared at the beginnning of operation.</param>
        /// <returns>The list where resource identifiers are stored. This enables one to put null list as argument.</returns>
        public IList<string> GetEmbeddedResources(IList<string> resourceList = null, bool clearBefore = true)
        {
            if (resourceList == null)
                resourceList = new List<string>();
            else
            {
                if (clearBefore)
                    resourceList.Clear();
            }
            int num = 0;
            if (EmbeddedResourcesList != null)
            {
                num = EmbeddedResourcesList.Count;
                for (int i = 0; i < num; ++i)
                {
                    string resId = EmbeddedResourcesList[i];
                    if (!resourceList.Contains(resId))
                        resourceList.Add(resId);
                }
            }
            return resourceList;
        }


        /// <summary>Returns an array of all selectable resx resources (those included through .resx files and compiled into .resources files).</summary>
        public string[] GetResxResourcesArray()
        { return ResxResourcesList.ToArray(); }

        /// <summary>Stores to the specified list all resx resources (i.e. resources that were included through .resx files and compiled into assemblies' 
        /// .resources files) eligible for selection, and returns the list.
        /// <para>The method takes care that resource identifires stored are unique.</para></summary>
        /// <param name="resourceList">List where resources are stored.</param>
        /// <param name="clearBefore">Whether the list is cleared at the beginnning of operation.</param>
        /// <returns>The list where resource identifiers are stored. This enables one to put null list as argument.</returns>
        public IList<string> GetResxResources(IList<string> resourceList = null, bool clearBefore = true)
        {
            if (resourceList == null)
                resourceList = new List<string>();
            else
            {
                if (clearBefore)
                    resourceList.Clear();
            }
            int num = 0;
            if (ResxResourcesList != null)
            {
                num = ResxResourcesList.Count;
                for (int i = 0; i < num; ++i)
                {
                    string resId = ResxResourcesList[i];
                    if (!resourceList.Contains(resId))
                        resourceList.Add(resId);
                }
            }
            return resourceList;
        }



        /// <summary>Stores allresources eligible to be selected in the current selector control (according to its state) 
        /// to the specified list, and returns the list.
        /// <para>The method takes care that resource IDs stored are unique.</para>
        /// <para>If a resource is currently selected then this selected resource is added first.</para>
        /// <para>This takes into account which kinds of resources can be selected.</para></summary>
        /// <param name="resourceList">List where resource IDs are stored.</param>
        /// <param name="clearBefore">Whether the list is cleared at the beginnning of operation.</param>
        /// <returns>The list where assemblies are stored. This enables one to put null list as argument.</returns>
        public IList<string> GetSelectableResources(IList<string> resourceList = null, bool clearBefore = true)
        {
            if (resourceList == null)
                resourceList = new List<string>();
            else
            {
                if (clearBefore)
                    resourceList.Clear();
            }
            if (SelectedResource != null)
            {
                if (!resourceList.Contains(SelectedResource))
                    resourceList.Add(SelectedResource);
            }
            if (SelectFromEmbeddedResources)
                GetEmbeddedResources(resourceList, false /* clearBefore */);
            if (SelectFromResxResources)
                GetResxResources(resourceList, false /* clearBefore */);
            return resourceList;
        }


        #endregion Data



        #region Events


        /// <summary>Event that is fired whenever the selected resource changes.</summary>
        public event EventHandler ResourceSelected;

        /// <summary>Called whenever the selected resource changes.</summary>
        protected virtual void OnResourceSelected()
        {
            if (ResourceSelected != null)
                ResourceSelected(this, new EventArgs());
        }


        /// <summary>Event that is fired whenever the selection parameters change (e.g., whether resources can be
        /// selected from multiple assemblies, or from the set of loaded assemblies, or from the set of assemblies referenced 
        /// by the current assembly, and if the king of resources changes).</summary>
        public event EventHandler SelectionParametersChanged;

        /// <summary>Must be called whenever the selection parameters change.</summary>
        protected virtual void OnSelectionParametersChanged()
        {
            if (SelectionParametersChanged != null)
                SelectionParametersChanged(this, new EventArgs());
        }



        #endregion Events



        private void chkMultipleAssemblies_CheckedChanged(object sender, EventArgs e)
        {
            MultipleAssembliesAllowed = chkMultipleAssemblies.Checked;
        }

        private void chkEmbedded_CheckedChanged(object sender, EventArgs e)
        {
            SelectFromEmbeddedResources = chkEmbedded.Checked;
        }

        private void chkIncludeResxFiles_CheckedChanged(object sender, EventArgs e)
        {
            IncludeResourceFiles = chkIncludeResxFiles.Checked;
        }

        private void chkResx_CheckedChanged(object sender, EventArgs e)
        {
            SelectFromResxResources = chkResx.Checked;
        }

        private void comboResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            string resId = SelectableResourcesList[comboResources.SelectedIndex];
            comboResources.Text = resId;
            SelectedResource = resId;
        }

        private void assemblySelector1_AssemblySelected(object sender, EventArgs e)
        {
            if (!MultipleAssembliesAllowed)
            {
                // If multiple assemblies are not allowed then changing the selected assembly will change the ser of 
                // reosurces one can select from. This is handled by refreshing the list of resources (which will also 
                // take care of nullifying the selected resource if the new set of eligible resources does not contain
                // the current selection):
                RefreshResourcesLists();
            }
        }

        private void assemblySelector1_SelectionParametersChanged(object sender, EventArgs e)
        {
            // Since the list of assemblies from which resources may be selected changes, we must
            // refresh the list of seelctable resources. Whether this will cause nullification of the 
            // eventually currently selected resource and whether this affect the set of selectable 
            // resources will all be handled by the call that refreshes the list of resources:
            RefreshResourcesLists();
        }

        private void comboResources_Click(object sender, EventArgs e)
        {
            comboResources.DroppedDown = true;
        }

    }
}
