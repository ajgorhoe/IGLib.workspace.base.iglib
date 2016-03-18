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

using IG.Lib;
using IG.Num;
using System.Security;

namespace IG.Forms  
{
    public partial class VectorFunctionScriptControl : UserControl
    {


        protected IG.Lib.IReporter Reporter { get { return UtilForms.Reporter; } }

        /// <summary>Constructs and initializes the vector function script control.</summary>
        public VectorFunctionScriptControl()
        {

            InitializeComponent();

            lblCurrentFunctionName.Text = ScalarFunctionNotDefinedString;
            this.CurrentScalarFunctionIndex = 0;

            this.SynchronizeData();

            this.HasUnsavedChanges = false;
            scalarFunctionScriptControl1.VectorFunctionControl = this;  // link child control 
            HasChildUnsavedChanges = false;
            lblFunctionName.Text = FunctionNameTitleNormal;
            scalarFunctionScriptControl1.HasUnsavedChanges = false;
        }



        #region Data

        private string _functionNameTitleNormal = "Function Name: ";

        private string _functionNameTitleModified = "Function Name: (*)";

        /// <summary>Normal text of the label that introduces function name input box.</summary>
        protected string FunctionNameTitleNormal
        {
            get { return _functionNameTitleNormal; }
            set { _functionNameTitleNormal = value; }
        }

        /// <summary>Normal text of the label that introduces function name input box.</summary>
        protected string FunctionNameTitleModified
        {
            get { return _functionNameTitleModified; }
            set { _functionNameTitleModified = value; }
        }


        private VectorFunctionScriptController _functionController;

        protected VectorFunctionScriptController FunctionController
        {
            get
            {
                if (_functionController == null)
                {
                    _functionController = new VectorFunctionScriptController(2, 2);
                }
                return _functionController;
            }
            set
            {
                if (!object.ReferenceEquals(value, _functionController))
                {
                    _functionController = value;

                    //this.FunctionName = value.Name;
                    //this.FunctionDescription = value.Description;
                    //this.Dimension = value.Dimension;
                    //this.ParameterNames = value.ParameterNames;
                    //this.ValueDefinition = value.ValueDefinitonString;
                    //this.GradientDefinitions = value.GradientDefinitionStrings;
                    //this.IsGradientDefined = value.IsGradientDefined;

                    CopyDataFromController();

                    this.HasUnsavedChanges = false;
                }
            }
        }

        /// <summary>Gets the function generated according to the function definition contained in this control
        /// and stored in <see cref="FunctionController"/>.</summary>
        /// <remarks>Function loaders from the <see cref="FunctionController"/>'s elements (scalar function script controllers) are 
        /// used to generate the function.</remarks>
        public IVectorFunction Function
        {
            get { return FunctionController.GetFunction(); }
        }

        private string[] _initialScalarFunctionDefinitionStrings;


        /// <summary>This property contains initial definitions of scalar functions (i.e.their values defined in terms of 
        /// parameters). If defined then these definitions are asigned in initialization.</summary>
        public virtual string[] InitialScalarFunctionDefinitionStrings
        {
            get {
                return _initialScalarFunctionDefinitionStrings;
                //return FunctionController.GetElements().Select(el => { return el.ValueDefinitonString; }).ToArray();
            }
            set
            {
                _initialScalarFunctionDefinitionStrings = value;
                SynchronizeData(true, true);
                ScalarFunctionScriptController[] functions = FunctionController.GetElements();
                for (int i = 0; i < functions.Length; ++i)
                {
                    ScalarFunctionScriptController func = functions[i];
                    if (func != null)
                    {
                        if (value == null)
                            func.ValueDefinitonString = null;
                        else if (value.Length <= i)
                            func.ValueDefinitonString = null;
                        else
                            func.ValueDefinitonString = value[i];

                    }
                }
            }
        }



        protected string ScalarFunctionNotDefinedString = "<< not defined >>";

        //protected string[] _initialScalarFunctionDefinitions = null;

        ///// <summary>This property contains initial definitions of scalar functions (i.e.their values defined in terms of 
        ///// parameters). If defined then these definitions are asigned in initialization.</summary>
        //public string[] InitialScalarFunctionDefinitionStrings
        //{
        //    get { return _initialScalarFunctionDefinitions; }
        //    set { _initialScalarFunctionDefinitions = value; }
        //}

        private bool _hasUnsavedChanges = false;

        /// <summary>Whether the current function definition has unsaved changes.</summary>
        public bool HasUnsavedChanges
        {
            get { return _hasUnsavedChanges; }
            set {
                if (value != _hasUnsavedChanges)
                {
                    _hasUnsavedChanges = value;
                    if (_hasUnsavedChanges)
                        lblFunctionName.Text = FunctionNameTitleModified;
                    else
                        lblFunctionName.Text = FunctionNameTitleNormal;
                }
            }
        }

        /// <summary>Indicates whether child control (of type <see cref="ScalarFunctionScriptControl"/>) has unsaved changes.</summary>
        public bool HasChildUnsavedChanges
        {
            get { return scalarFunctionScriptControl1.HasUnsavedChanges; }
            set { scalarFunctionScriptControl1.HasUnsavedChanges = value; }
        }


        /// <summary>Copies data from the current control to the internal function DTO.</summary>
        void CopyDataToController()
        {
            FunctionController.Name = txtFunctionName.Text;
            FunctionController.Description = txtFunctionDescription.Text;
            FunctionController.NumParameters = (int)numNumParameters.Value;
            FunctionController.NumValues = (int)numNumFunctions.Value;
            string[] strings = FunctionController.ConvertParameterNamesToArray(txtParameterNames.Text);
            FunctionController.ParameterNames = strings;
            strings = FunctionController.ConvertParameterNamesToArray(txtFunctionNames.Text);
            FunctionController.FunctionNames = strings;
        }


        /// <summary>Copies data from the internal DTO to the current control.</summary>
        public void CopyDataFromController()
        {
            txtFunctionName.Text = FunctionController.Name;
            txtFunctionDescription.Text = FunctionController.Description;
            numNumParameters.Value = FunctionController.NumParameters;
            numNumFunctions.Value = FunctionController.NumValues;
            txtParameterNames.Text = FunctionController.GetParametersString();
            txtFunctionNames.Text = FunctionController.GetFunctionsString();
        }


        /// <summary>Synchronizes data for the current vector function definition.
        /// <para>This method checks number of parameters and number of funcion values, then goes through all scalar funciton
        /// elemennts and checks if their data is allocated and consistent. It also takes care that data for the currently 
        /// edited scalar function is updated according to the true state.</para></summary>
        /// <param name="syncParameterNames">If true then parameterr names are also synchroniized.</param>
        /// <param name="syncFunctionNames">If true then function names are also synchronized.</param>
        public void SynchronizeData(bool syncParameterNames = false, bool syncFunctionNames = true)
        {
            FunctionController.SynchronizeData(syncParameterNames, syncFunctionNames);
            if (CurrentScalarFunctionIndex < 0)
                CurrentScalarFunctionIndex = 0;
            else if (CurrentScalarFunctionIndex >= NumValues)
                CurrentScalarFunctionIndex = NumValues - 1;
            this.lblCurrentFunctionName.Text = ScalarFunctionNotDefinedString;
            if (FunctionNames != null)
                if (FunctionNames.Length > CurrentScalarFunctionIndex)
                    this.lblCurrentFunctionName.Text = FunctionNames[CurrentScalarFunctionIndex];
            CopyDataFromController();
            CurrentScalarFunctionController = FunctionController[CurrentScalarFunctionIndex];
            ////// scalarFunctionScriptControl1.FunctionController = CurrentScalarFunctionController;
            ////// scalarFunctionScriptControl1.CopyDataFromController();
        }


        int _currentScalarFunctionIndex = 0;

        /// <summary>Gets or sets index of the current scalar funciton that is worked on.
        /// <para>Changing this property affect which function is edited in the scalar function definition frame.</para></summary>
        int CurrentScalarFunctionIndex
        {
            get {
                if (_currentScalarFunctionIndex < 0 || _currentScalarFunctionIndex >= NumValues)
                    throw new IndexOutOfRangeException("Scalar function (component) index " + _currentScalarFunctionIndex + " is out of range.");
                return _currentScalarFunctionIndex;
            }
            set {
                if (value != _currentScalarFunctionIndex)
                {
                    if (value < 0 || value >= NumValues)
                        throw new IndexOutOfRangeException("Scalar function index " + value + " is out of range, should be between 0 and "
                            + (NumValues - 1).ToString() + ".");

                    _currentScalarFunctionIndex = value;
                    ScalarFunctionScriptController scalarFunctionController = this.FunctionController[value];

                    if (scalarFunctionController == null)
                        scalarFunctionController = new ScalarFunctionScriptController(this.NumParameters);
                    if (scalarFunctionController.Dimension != this.NumParameters)
                        scalarFunctionController.Dimension = this.NumParameters;
                    if (FunctionController.IsNamesSynchronized
                            && !UtilStr.StringArraysEqual(scalarFunctionController.ParameterNames, ParameterNames))
                        scalarFunctionController.ParameterNames = this.ParameterNames;

                    // Line below will change, if necessary, the corresponding scalar function controler on the list of scalar function 
                    // controllers on the vector function controller, and it will also change scallar function controller in the
                    // GUI control for definition of scalar function:
                    this.CurrentScalarFunctionController = scalarFunctionController;
                    this.lblCurrentFunctionName.Text = ScalarFunctionNotDefinedString;
                    if (FunctionNames != null)
                        if (FunctionNames.Length > value)
                            this.lblCurrentFunctionName.Text = FunctionNames[value];
                    numCurrentFunction.Value = value;

                }
            }
        }


        ///// <summary>Sets scalar function controller on the scalar function definition (child) control, and this is done in such a way 
        ///// that the changes status (<see cref="HasUnsavedChanges"/> of the current control is not changes, and it is cleared
        ///// on the scalar function definition (child) control.)</summary>
        //protected void ScalarCntrolSetController()
        //{
        //    bool hasChanges = this.HasUnsavedChanges;


        //    this.HasUnsavedChanges = hasChanges;  // restore the flag
        //}

        ScalarFunctionScriptController _currentScalarFunctionController = null;

        /// <summary>Scalar function controller that represents the element of the vector function that is currently edited.</summary>
        protected ScalarFunctionScriptController CurrentScalarFunctionController
        {
            get { return _currentScalarFunctionController; }
            set
            {
                if (!object.ReferenceEquals(value, _currentScalarFunctionController))
                {
                    if (value != null)
                    {
                        if (value.Dimension != NumParameters)
                        {
                            DialogResult result1 = MessageBox.Show("Dimension of assigned current scalar function " + value.Dimension + Environment.NewLine
                                + "is different than the current number of parameters " + NumParameters + ". " + Environment.NewLine + Environment.NewLine 
                                + "  Do you want to adapt number of parameters of scalar function? " +  Environment.NewLine,
                                "Number of parameters does not match", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result1 != DialogResult.Yes)
                                value.Dimension = this.NumParameters;
                        }
                        if (!UtilStr.StringArraysEqual(value.ParameterNames, ParameterNames))
                        {
                            DialogResult result1 = MessageBox.Show("Parameter names of the assigned scalar function do not match." + Environment.NewLine
                                + "  Do you want to adapt parameter names of the scalar function? " +  Environment.NewLine,
                                "Parameter names do not match", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result1 != DialogResult.Yes)
                                value.ParameterNames = this.ParameterNames;
                        }
                    }
                    _currentScalarFunctionController = value;  // store the changes flag
                    bool hasChanges = this.HasUnsavedChanges;
                    scalarFunctionScriptControl1.FunctionController = value;  // restore the changes flag, to prevent false changes indicator
                    this.HasUnsavedChanges = hasChanges;
                    FunctionController[CurrentScalarFunctionIndex] = value;
                }
            }
        }




        /// <summary>Returns function signature created from the data.</summary>
        protected string GetFunctionSignature()
        {
            return FunctionName + " (" + UtilStr.GetParametersStringPlain(FunctionController.ParameterNames) + ") = ";
        }



        private bool _isLoaderConsistent = false;

        /// <summary>Whether function loader is in consistent state.
        /// <para>This method shoulld be elaborated or dropped.</para></summary>
        public bool IsLoaderConsistent
        {
            get { return _isLoaderConsistent; }
            protected set { _isLoaderConsistent = value; }
        }


        /// <summary>Name of the function.</summary>
        public string FunctionName
        {
            get { return FunctionController.Name; }
            set
            {
                if (value != FunctionName)
                {
                    FunctionController.Name = value;
                    txtFunctionName.Text = value;
                    // lblFunctionSignature.Text = GetFunctionSignature();

                    this.HasUnsavedChanges = true;
                }
            }
        }

        public string FunctionDescription
        {
            get { return FunctionController.Description; }
            set
            {
                if (value != FunctionDescription)
                {
                    FunctionController.Description = value;
                    txtFunctionDescription.Text = value;
                    // lblFunctionSignature.Text = GetFunctionSignature();

                    this.HasUnsavedChanges = true;

                }
            }
        }


        /// <summary>Gets or sets number of parameters of the represented vector function.</summary>
        public int NumParameters
        {
            get { return FunctionController.NumParameters; }
            set
            {
                if (value != NumParameters)
                {
                    if (value < 1)
                        throw new ArgumentException("Number of function parameters should be greater or equal to 1.");
                    FunctionController.NumParameters = value;
                    numNumParameters.Value = value;

                    //CopyDataFromController();
                    SynchronizeData();

                    this.HasUnsavedChanges = true;
                }
            }
        }

        /// <summary>Gets or sets number of values of the represented vector function (i.e., dimension of 
        /// its codomain).</summary>
        public int NumValues
        {
            get { return FunctionController.NumValues; }
            set
            {
                if (value != NumValues)
                {
                    if (value < 1)
                        throw new ArgumentException("Number of function elements should be greater or equal to 1.");
                    FunctionController.NumValues = value;
                    numNumFunctions.Value = value;
                    numCurrentFunction.Maximum = value;

                    CopyDataFromController();

                    this.HasUnsavedChanges = true;
                }
            }
        }


        /// <summary>Names of function parameters.</summary>
        public string[] ParameterNames
        {
            get { return FunctionController.ParameterNames; }
            set
            {
                if (!UtilStr.StringArraysEqual(value, ParameterNames))
                {
                    FunctionController.ParameterNames = value;

                    CopyDataFromController();  // refresh dependent definitions

                    this.HasUnsavedChanges = true;
                }
            }
        }
        

        /// <summary>Names of scalar function components that define individual values of the represented vector function.</summary>
        public string[] FunctionNames
        {
            get { return FunctionController.FunctionNames; }
            set
            {
                if (!UtilStr.StringArraysEqual(value, FunctionNames))
                {
                    FunctionController.FunctionNames = value;

                    CopyDataFromController();  // refresh dependent definitions

                    this.HasUnsavedChanges = true;
                }
            }
        }


        InputOutputDataDefiniton _dataDefinition = null;

        /// <summary>Definition of input and output data for the represented scalar function.</summary>
        InputOutputDataDefiniton DataDefinition
        {
            get { return _dataDefinition; }
            set { _dataDefinition = value; }
        }


        /// <summary>GUI component for seclecting files.</summary>
        public FileSelector FileSelector { get { return this.scalarFunctionScriptControl1.FileSelector; } }


        bool _prepareStandardDefinitions = true;

        bool PrepareStandardDefinitions
        {
            get { return _prepareStandardDefinitions; }
            set { _prepareStandardDefinitions = value; }
        }

        #endregion Data 



        public string CreateFunctonSummary()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(FunctionName).
                    Append(" ( ").
                    Append(UtilStr.GetParametersStringPlain(ParameterNames)).
                    Append(") = ").AppendLine()
                    .Append("{").Append(FunctionController.GetFunctionsString()).Append("} = ").AppendLine().AppendLine("{");

                for (int whichFunction = 0; whichFunction < NumValues; ++ whichFunction)
                {
                    if (FunctionController[whichFunction] == null)
                        sb.Append("null").AppendLine();
                    else
                        sb.Append(FunctionController[whichFunction].ValueDefinitonString);
                    if (whichFunction < NumValues - 1)
                        sb.AppendLine(" , ");
                    else
                        sb.AppendLine();
                }
                sb.AppendLine("}");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Reporter.ReportError("Exception was thrown when writing function description.", ex);
                return null;
            }
        }




        private void txtFunctionName_Validated(object sender, EventArgs e)
        {
            this.FunctionName = txtFunctionName.Text;
        }

        private void txtFunctionDescription_Validated(object sender, EventArgs e)
        {
            this.FunctionDescription = txtFunctionDescription.Text;
        }

        private void txtParameterNames_Validated(object sender, EventArgs e)
        {
            string[] strings = FunctionController.ConvertParameterNamesToArray(txtParameterNames.Text);
            ParameterNames = strings;
        }

        private void txtFunctionNames_Validated(object sender, EventArgs e)
        {
            string[] strings = FunctionController.ConvertParameterNamesToArray(txtFunctionNames.Text);
            FunctionNames = strings;
        }

        private void numNumParameters_Validated(object sender, EventArgs e)
        {
            this.NumParameters = (int)numNumParameters.Value;
        }

        private void numNumFunctions_Validated(object sender, EventArgs e)
        {
            this.NumValues = (int)numNumFunctions.Value;
        }



        private void numCurrentFunction_ValueChanged(object sender, EventArgs e)
        {
            if (numCurrentFunction.Value < 0)
                numCurrentFunction.Value = 0;
            else if (numCurrentFunction.Value >= NumValues)
                numCurrentFunction.Value = NumValues - 1;
            this.CurrentScalarFunctionIndex = (int)numCurrentFunction.Value;

        }


        private void numCurrentFunction_Validated(object sender, EventArgs e)
        {
            if (numCurrentFunction.Value < 0)
                numCurrentFunction.Value = 0;
            else if (numCurrentFunction.Value >= NumValues)
                numCurrentFunction.Value = NumValues - 1;
            // this.CurrentScalarFunctionIndex = (int)numCurrentFunction.Value;
        }


        // Buttons:

        private void btnCreateFunction_Click(object sender, EventArgs e)
        {
            IVectorFunction f = FunctionController.GetFunction();
            if (f == null)
            {
                Reporter.ReportError("Vector function could not be created from the current definition.");
            }
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {

            FadingMessage fm = new FadingMessage("Vector function summay", CreateFunctonSummary(), 4000, 0.25);

        }

        private void btnValueCalculator_Click(object sender, EventArgs e)
        {
            Reporter.ReportError("Vector function evaluator is not yet available.");
        }

        /// <summary>Launches 1D parametric plots of the function defined in this control.</summary>
        private void btnPlot1d_Click(object sender, EventArgs e)
        {
            try
            {
                VectorFunctionPlotter1dForm win = new VectorFunctionPlotter1dForm();
                win.MainControl.NeuralDataDefinition = FunctionController.GetDataDefinitionObject(true /* setBoundsAndDefaults */);
                win.MainControl.Function = this.Function;
                if (DataDefinition != null)
                    win.MainControl.NeuralDataDefinition = DataDefinition;
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                Reporter.ReportError("Exception was thrown in 1D function plotter.", ex);
            }
        }

        /// <summary>Launches 2D parametric plots of the function defined in this control.</summary>
        private void btnPlot2d_Click(object sender, EventArgs e)
        {
            try
            {
                VectorFunctionPlotter2dForm win = new VectorFunctionPlotter2dForm();
                win.MainControl.NeuralDataDefinition = FunctionController.GetDataDefinitionObject(true /* setBoundsAndDefaults */);
                win.MainControl.Function = this.Function;
                if (DataDefinition != null)
                    win.MainControl.NeuralDataDefinition = DataDefinition;
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                Reporter.ReportError("Exception was thrown in 2D function plotter.", ex);
            }
        }

        /// <summary>Saves the current fucntion definition to a JSON file.</summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            string filePath = null;
            FileSelector.AllowExistentFiles = true;
            FileSelector.AllowNonexistentFiles = true;
            FileSelector.Filter = "JSON Files (*.json)|*.json|All files|*";
            FileSelector.BrowseFile();
            filePath = FileSelector.FilePath;
            bool filePathValid = false;
            try
            {
                FileInfo info = new FileInfo(filePath);
                if (info != null)
                {
                    filePathValid = true;
                }
            }
            catch (Exception ex)
            {
                if (ex is SecurityException || ex is UnauthorizedAccessException)
                {
                    Reporter.ReportWarning("There is no access to the file: " + filePath + Environment.NewLine
                        + "  Details: " + ex.Message + Environment.NewLine + Environment.NewLine
                        + "Choose a different file path and try again.");
                }
                else if (ex is ArgumentException)
                {
                    Reporter.ReportWarning("The file path is invalid: " + filePath + Environment.NewLine
                        + "  Details: " + ex.Message + Environment.NewLine + Environment.NewLine
                        + "Choose a different file path and try again.");
                }
                else if (ex is ArgumentException)
                {
                    Reporter.ReportWarning("The file path is invalid, details unknown: " + filePath + Environment.NewLine
                        + Environment.NewLine
                        + "Choose a different file path and try again.");
                }
            }
            if (File.Exists(filePath))
            {
                DialogResult result1 = MessageBox.Show("The specified file already exists: " + Environment.NewLine
                    + "  " + filePath + Environment.NewLine + Environment.NewLine
                    + "Do you want to overwrite the file?",
                    "File Exists",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result1 != DialogResult.Yes)
                    filePathValid = false;
            }
            if (filePathValid)
            {
                try
                {


                    //ISerializer serializer = new SerializerJson();
                    //serializer.Serialize<ScalarFunctionScripController>(FunctionController, filePath, false /* append */);
                    //HasUnsavedChanges = false;


                    if (DataDefinition == null && FunctionController != null)
                    {
                        DataDefinition = FunctionController.GetDataDefinitionObject(true /* setBoundsAndDefaults */);
                    }

                    VectorFunctionScriptController.SaveJson(FunctionController, filePath, false,
                        DataDefinition /* save data definition in addition to pure function definition to the same file */);

                    Reporter.ReportInfo("Vector function was saved to the following file: "
                        + Environment.NewLine + filePath);


                }
                catch (Exception ex)
                {
                    Reporter.ReportError("Saving function definition to the file failed. " + Environment.NewLine
                        + "  Details: " + ex.Message + Environment.NewLine
                        + "  File path: " + filePath
                        + Environment.NewLine + Environment.NewLine
                        + "Choose a different file path and try again.");
                }
            }
        } // save to file


        /// <summary>Loads vector funcition definition from a file.</summary>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (this.HasUnsavedChanges)
            {
                DialogResult result1 = MessageBox.Show("The current function definition has unsaved changes! "
                    + Environment.NewLine + Environment.NewLine
                    + "Do you want to discard the changes and load a new definition?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result1 != DialogResult.Yes)
                    return;
            }
            string filePath = null;
            FileSelector.AllowExistentFiles = true;
            FileSelector.AllowNonexistentFiles = false;
            FileSelector.Filter = "JSON Files (*.json)|*.json|All files|*";
            FileSelector.BrowseFile();
            filePath = FileSelector.FilePath;
            if (!File.Exists(filePath))
            {
                Reporter.ReportError("Selected file does not exist: " + Environment.NewLine
                    + "  " + filePath + Environment.NewLine + Environment.NewLine
                    + "Select a valid file and try again.");
            }
            else
            {
                try
                {
                    VectorFunctionScriptController controller = null;
                    InputOutputDataDefiniton dataDef = null;
                    VectorFunctionScriptController.LoadJson(filePath, ref controller, ref dataDef);
                    FunctionController = controller;
                    DataDefinition = dataDef;

                    HasUnsavedChanges = false;
                }
                catch (Exception ex)
                {
                    Reporter.ReportError("Could not load function definition from the file. " + Environment.NewLine
                        + "  Details: " + ex.Message + Environment.NewLine
                        + "  File: " + filePath + Environment.NewLine + Environment.NewLine
                        + "Select a valid JSON file and try again.");
                }
            }

            // Finally, update the dependent information:
            if (CurrentScalarFunctionIndex >= NumValues)
                CurrentScalarFunctionIndex = NumValues - 1;
            else
                CurrentScalarFunctionController = FunctionController[CurrentScalarFunctionIndex];

        }  // load from file


    }  // class

}
