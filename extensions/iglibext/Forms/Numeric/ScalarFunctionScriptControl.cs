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
using IG.Num;
using IG.Script;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Security;

namespace IG.Forms
{

    /// <summary>Control for definition of scalar functions by user defined expressions (through a script loader).</summary>
    /// $A Igor Jun14;
    public partial class ScalarFunctionScriptControl : UserControl 
    {

        /// <summary>Reporter used for internal reporting.</summary>
        protected IG.Lib.IReporter Reporter { get { return UtilForms.Reporter; } }


        /// <summary>Constructs the scalar function script control.</summary>
        public ScalarFunctionScriptControl(): base()
        {
            InitializeComponent();
            _bgDefined = txtValue.BackColor;
            lblName.Text = FunctionNameTitleNormal;

            // Console.WriteLine("Scalar function script control initalized, state: " + Environment.NewLine + CreateFunctonSummary());

            this.HasUnsavedChanges = false;
        }


        /// <summary>Constructs the scalar function script control.</summary>
        /// <param name="setTestDefinitions">If true then control is filled with test definitions, such that the represented
        /// function can be immediately plotted, saved, etc. 
        /// <para>Beside for debugging, this is convenient for testing by users who are not used to use the control.</para></param>
        public ScalarFunctionScriptControl(bool setTestDefinitions) 
        {
            if (setTestDefinitions)
                SetTestDefinitions();

            this.HasUnsavedChanges = false;
        }



        /// <summary>Setts a test definition.</summary>
        public void SetTestDefinitions()
        {
            FunctionName = "f";
            FunctionDescription = "Simple scalar function.";
            ParameterNames = new string[] { "x1", "x2" };
            ValueDefinition = "x1 * x1 + 2 * x2 * x2";
            GradientDefinitions = new string[] { "2 * x1", "4 * x2" };
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


        private ScalarFunctionScriptController _functionController;

        /// <summary>Cotroller that holds the definition data of the scalar function entered by the current control, and also
        /// holds mechanism for compiling and loading scalar function from these definitions.</summary>
        public ScalarFunctionScriptController FunctionController
        {
            get
            {
                if (_functionController == null)
                {
                    _functionController = new ScalarFunctionScriptController();
                }
                return _functionController;
            }
            set
            {
                if (!object.ReferenceEquals(value, _functionController))
                {
                    _functionController = value;

                    this.FunctionName = value.Name;
                    this.FunctionDescription = value.Description;
                    this.Dimension = value.Dimension;
                    this.ParameterNames = value.ParameterNames;
                    this.ValueDefinition = value.ValueDefinitonString;
                    this.GradientDefinitions = value.GradientDefinitionStrings;
                    this.IsGradientDefined = value.IsGradientDefined;

                    CopyDataFromController();

                    this.HasUnsavedChanges = false;
                }
            }
        }


        /// <summary>Scalar function loader that is responsible for creation of scalar function objects from data.</summary>
        public ScalarFunctionLoader FunctionLoader
        {
            get
            {
                return FunctionController.GetFunctionLoader();
                //if (_functionLoader == null)
                //{
                //    _functionLoader = new ScalarFunctionLoader();
                //}
                //return _functionLoader;
            }
            protected set
            {
                if (!object.ReferenceEquals(value, FunctionLoader))
                {

                    // TODO:
                    // Consider whether setitng the function loader could be avoided, and everything can be 
                    // handled by the DTO's function loader that is automatically provded by the DTO in a 
                    // lazy evaluation style.

                    if (value == null)
                        throw new ArgumentException("Scalar function loader can not be null.");
                    IsLoaderConsistent = false;
                }
            }
        }


        /// <summary>Gets the function generated by the function loader.</summary>
        /// <remarks>Function loader from the <see cref="FunctionController"/> is used.</remarks>
        public LoadableScalarFunctionBase Function
        {
            get { return FunctionController.GetFunction(); }
        }


        private VectorFunctionScriptControl _parentVectorFunctionControl = null;

        /// <summary>The parent vector function script control. When this control is embedded within the vector function script control,
        /// this property should be set such that communication back to parent is enabled (in particular, communication about unsaved
        /// changes).</summary>
        public VectorFunctionScriptControl VectorFunctionControl
        {
            get { return _parentVectorFunctionControl; }
            set { _parentVectorFunctionControl = value; }
        }


        /// <summary>Get or sets a flag indicating whether the parent control has unsaved changes.
        /// <para>The flag may only be set to false.</para>
        /// <para>If there is no parent control (such as <see cref="VectorFunctionScriptControl"/>) then getter returns false and setter has no effect.</para></summary>
        /// <param name="set">If true then the "Has unsaved changes" is set on the parent. Otherwise, only the value indicating whether
        /// the parent has unsaved changes is returned. Default value is false.</param>
        /// <returns>Flag indicating whether parent control had unsaved changes before this method was called.</returns>
        public bool HasParentUnsavedChanges(bool set = false)
        {
            bool ret = false;
            if (VectorFunctionControl != null)
                ret = VectorFunctionControl.HasUnsavedChanges;
            if (set)
            {
                if (VectorFunctionControl != null)
                    VectorFunctionControl.HasUnsavedChanges = true;
            }
            return ret;
        }

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
                    {
                        lblName.Text = FunctionNameTitleModified;
                        HasParentUnsavedChanges(true);
                    }
                    else
                        lblName.Text = FunctionNameTitleNormal;
                }
            }
        }


        /// <summary>Copies data from the current control to the internal function DTO.</summary>
        void CopyDataToController()
        {
            FunctionController.Name = txtName.Text;
            FunctionController.Description = txtDescription.Text;
            FunctionController.Dimension = (int)numDimension.Value;

            string[] strings = FunctionController.ConvertParameterNamesToArray(txtParameterNames.Text);
            FunctionController.ParameterNames = strings;

            FunctionController.ValueDefinitonString = txtValue.Text;
            if (IsGradientDefined)
            {
                FunctionController.SetGradientDefinitionSingleString(txtGradients.Text);
            } else
            {
                FunctionController.SetGradientDefinitionSingleString(null);
            }
        }


        /// <summary>Copies data from the internal DTO to the current control.</summary>
        public void CopyDataFromController()
        {
            if (FunctionController != null)
            txtName.Text = FunctionController.Name;
            txtDescription.Text = FunctionController.Description;
            txtParameterNames.Text = FunctionController.GetParametersString();
            numDimension.Value = FunctionController.Dimension;
            txtValue.Text = FunctionController.ValueDefinitonString;
            if (FunctionController.IsGradientDefined)
            {
                txtGradients.Text = FunctionController.GetGradientDefinitionSingleString();
                this.IsGradientDefined = true;
            } else
            {
                // txtGradients.Text = null;
                this.IsGradientDefined = false;
            }
            lblFunctionSignature.Text = GetFunctionSignature();
        }



        /// <summary>Returns function signature created from the data.</summary>
        protected string GetFunctionSignature()
        {
            return FunctionName + " (" + UtilStr.GetParametersStringPlain(FunctionController.ParameterNames) + ") = ";
        }



        /// <summary>Resets function definition to simply "0". Gradients are redefined accordingly.</summary>
        public void ResetFunctionDefinition()
        {
            int numParam = Dimension;
            ValueDefinition = "0";
            string[] grads = new string[numParam];
            for (int i = 0; i < numParam; ++i)
            {
                grads[i] = "0";
            }
            GradientDefinitions = grads;
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
                if (value != FunctionName && !(string.IsNullOrEmpty(value) && string.IsNullOrEmpty(FunctionName)))
                {
                    FunctionController.Name = value;
                    txtName.Text = value;
                    lblFunctionSignature.Text = GetFunctionSignature();

                    this.HasUnsavedChanges = true;
                }
            }
        }

        public string FunctionDescription
        {
            get { return FunctionController.Description; }
            set
            {
                if (value != FunctionDescription && !(string.IsNullOrEmpty(value) && string.IsNullOrEmpty(FunctionDescription)))
                {
                    FunctionController.Description = value;
                    txtDescription.Text = value;
                    lblFunctionSignature.Text = GetFunctionSignature();

                    this.HasUnsavedChanges = true;

                }
            }
        }


        /// <summary>Gets or sets function dimension.</summary>
        public int Dimension
        {
            get { return FunctionController.Dimension; }
            set {
                if (value != Dimension)
                {
                    if (value < 1)
                        throw new ArgumentException("Number of function parameters should be greater or equal to 1.");
                    FunctionController.Dimension = value;
                    numDimension.Value = value;

                    CopyDataFromController();

                    this.HasUnsavedChanges = true;
                }
            }
        }

        // protected string[] _parameterNames;

        /// <summary>Names of function parameters.</summary>
        public string[] ParameterNames
        {
            get { return FunctionController.ParameterNames; }
            set
            {
                if (!UtilStr.StringArraysEqual(value, ParameterNames) &&
                    !((value == null || value.Length == 0) && (ParameterNames == null || ParameterNames.Length == 0)))
                {
                    FunctionController.ParameterNames = value;

                    CopyDataFromController();  // refresh dependent definitions

                    this.HasUnsavedChanges = true;
                }
            }
        }


        /// <summary>String that defines function value.</summary>
        public string ValueDefinition
        {
            get { return FunctionController.ValueDefinitonString; }
            set {
                if (value != ValueDefinition && !(string.IsNullOrEmpty(value) && string.IsNullOrEmpty(ValueDefinition)))
                {
                    FunctionController.ValueDefinitonString = value;

                    this.txtValue.Text = FunctionController.ValueDefinitonString;

                    this.HasUnsavedChanges = true;
                }
            }
        }

        protected bool _isGradientDefined = false;

        Color _bgDefined = Color.White;

        Color _bgUndefined = Color.LightGray;

        public bool IsGradientDefined
        {
            get { return _isGradientDefined; }
            set {
                if (value != _isGradientDefined)
                {
                    _isGradientDefined = value;
                    chkGradients.Checked = value;
                    if (value == true)
                        txtGradients.BackColor = _bgDefined;
                    else
                        txtGradients.BackColor = _bgUndefined;


                    this.HasUnsavedChanges = true;

                }
            }
        }




        /// <summary>String that defines function gradients.</summary>
        public string[] GradientDefinitions
        {
            get { return FunctionController.GradientDefinitionStrings; }
            set
            {
                if (!UtilStr.StringArraysEqual(value, GradientDefinitions) && 
                    !((value == null || value.Length == 0) && (GradientDefinitions == null || GradientDefinitions.Length == 0)))
                {
                    FunctionController.GradientDefinitionStrings = value;

                    this.IsGradientDefined = FunctionController.IsGradientDefined;

                    txtGradients.Text = FunctionController.GetGradientDefinitionSingleString();


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
        public FileSelector FileSelector { get { return this.fileSelector1; } }


        #endregion Data


        public string CreateFunctonSummary()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(FunctionName).
                    Append(" ( ").
                    Append(UtilStr.GetParametersStringPlain(ParameterNames)).
                    Append(") = ").AppendLine().Append("\"" + ValueDefinition + "\"").AppendLine();
                if (IsGradientDefined)
                {
                    sb.Append("grad ").Append(FunctionName).Append(" ( ").
                        Append(UtilStr.GetParametersStringPlain(ParameterNames)).
                        Append(") = ").AppendLine();
                    try
                    {
                        for (int i = 0; i < GradientDefinitions.Length; ++i)
                        {
                            sb.AppendLine("\"" + GradientDefinitions[i] + "\"");
                        }
                    }
                    catch (Exception ex)
                    {
                        Reporter.ReportError("Exception was thrown when writing gradient definiton.", ex);
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Reporter.ReportError("Exception was thrown when writing function description.", ex);
                return null;
            }
        }

        
        //async Task<int> ExampleMethodIntAsync()
        //{
        //    //await Task.Yield();
        //    return 22333;
        //}

        //async Task ExampleMethodAsync()
        //{
        //    // await ExampleMethodIntAsync();
        //    // The following line simulates a task-returning asynchronous process.
        //    // await Task.Delay(10);

        //    Thread.Sleep(20);
        //}


        private void btnSummary_Click(object sender, EventArgs e)
        {

            FadingMessage fm = new FadingMessage("Scalar function summay", CreateFunctonSummary(), 4000, 0.25);

        }


        private void txtName_Validated(object sender, EventArgs e)
        {
            FunctionName = txtName.Text;
        }


        private void txtDescription_Validated(object sender, EventArgs e)
        {
            FunctionDescription = txtDescription.Text;
        }


        private void txtParameterNames_Validated(object sender, EventArgs e)
        {
            string[] strings = FunctionController.ConvertParameterNamesToArray(txtParameterNames.Text);
            ParameterNames = strings;
        }


        private void numDimension_Validated(object sender, EventArgs e)
        {
            //Reporter.ReportInfo("numDimension_Validated: Dimension = " + numDimension.Value);

            Dimension = (int) numDimension.Value;
        }


        private void txtValue_Validated(object sender, EventArgs e)
        {
            //Reporter.ReportInfo("txtValue_Validated: value = " + txtValue.Text);

            ValueDefinition = txtValue.Text;
        }

        private void txtGradients_Validated(object sender, EventArgs e)
        {

            string[] strings = FunctionController.ConvertFunctionDefinitionsToArray(txtGradients.Text);

            //string msg = "txtGradients_Validated: new gradient definitions: " + Environment.NewLine + "text: " + txtParameterNames.Text +
            //    Environment.NewLine;
            //for (int i = 0; i < strings.Length; ++i)
            //    msg += "  No. " + i.ToString() + ": " + strings[i] + Environment.NewLine;
            //Reporter.ReportInfo(msg);

            GradientDefinitions = strings;
        }

        private void chkGradients_CheckedChanged(object sender, EventArgs e)
        {
            //Reporter.ReportInfo("chkGradients_CheckedChanged: Checked = " + chkGradients.Checked);

            IsGradientDefined = chkGradients.Checked;
        }


        /// <summary>Creates a function i advance (before it is eeded for any other operation).
        /// Error message is launched if creation is not successful.</summary>
        private void btnCreateFunction_Click(object sender, EventArgs e)
        {
            //CreateScalarFunction();
            IScalarFunction f = FunctionController.GetFunction();
            if (f == null)
            {
                Reporter.ReportError("Function could not be created from current definition.");
            }
        }



        /// <summary>Launches value calculator where user can calculate functio values with the function 
        /// that was defined in the current control.</summary>
        private void btnValueCalculator_Click(object sender, EventArgs e)
        {

            // new FadingMessage("This is to confirm NEW changes.", 4000);


            // CreateScalarFunction();
            IScalarFunction f = FunctionController.GetFunction();
            if (f == null)
            {
                Reporter.ReportError("Function could not be created from current definition.");
            }
            else
            {
                double[] parameterValues = new double[Dimension];
                for (int i = 0; i < Dimension; ++i)
                {
                    parameterValues[i] = 0;
                }
                if (false && Dimension == 2)
                {
                    double[] param = new double[2];
                    for (double i = 1; i <= 3; ++i)
                        for (int j = 1; j <= 3; ++j)
                        {
                            param[0] = i;
                            param[1] = j;
                            Console.WriteLine(FunctionName + "(" + i + ", " + j + "} =  " + Function.Value(new Vector(param)));
                        }
                }
                ScalarFunctionEvaluatorForm win = new ScalarFunctionEvaluatorForm(this.Function, ParameterNames, parameterValues);
                if (true)
                {
                    // For a test, create a vector function from the current scalar funciton and evaluate the fnction as 
                    // vector function with a one dimensional return value.
                    win.MainControl.TreatScalarAsVectorFunction = true;
                }
                win.MainControl.FunctionDefinition = this.CreateFunctonSummary();
                win.Show();
            }
        }



        /// <summary>Launches 1D parametric plots of the function defined in this control.</summary>
        private void btnPlot1d_Click(object sender, EventArgs e)
        {
            try
            {
                VectorFunctionPlotter1dForm win = new VectorFunctionPlotter1dForm();
                win.MainControl.NeuralDataDefinition = FunctionController.GetDataDefinitionObject(true /* setBoundsAndDefaults */);
                win.MainControl.ScalarFunction = this.Function;
                if (DataDefinition != null)
                    win.MainControl.NeuralDataDefinition = DataDefinition;
                win.ShowDialog();
            }
            catch(Exception ex)
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
                win.MainControl.ScalarFunction = this.Function;
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
            fileSelector1.AllowExistentFiles = true;
            fileSelector1.AllowNonexistentFiles = true;
            fileSelector1.Filter = "JSON Files (*.json)|*.json|All files|*";
            fileSelector1.BrowseFile();
            filePath = fileSelector1.FilePath;
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
                    Reporter.ReportWarning("There is no write access to the file: " + filePath + Environment.NewLine
                        + "  Details: " + ex.Message + Environment.NewLine + Environment.NewLine
                        + "Choose a different file path and try again.");
                } else if (ex is ArgumentException)
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
                try {
                    if (DataDefinition == null && FunctionController != null)
                    {
                        DataDefinition = FunctionController.GetDataDefinitionObject(true /* setBoundsAndDefaults */);
                    }

                    ScalarFunctionScriptController.SaveJson(FunctionController, filePath, false, 
                        DataDefinition /* save data definition in addition to pure function definition to the same file */);

                    this.HasUnsavedChanges = false;

                    Reporter.ReportInfo("Scalar function was saved to the following file: "
                        + Environment.NewLine + filePath);


                }
                catch(Exception ex)
                {
                    Reporter.ReportError("Saving function definition to the file failed. " + Environment.NewLine 
                        + "  Details: " + ex.Message + Environment.NewLine 
                        + "  File path: " + filePath
                        + Environment.NewLine + Environment.NewLine
                        + "Choose a different file path and try again.");
                }
            }
        }  // save to file



        /// <summary>Loads the current function definition from a JSON file.</summary>
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
            fileSelector1.AllowExistentFiles = true;
            fileSelector1.AllowNonexistentFiles = false;
            fileSelector1.Filter = "JSON Files (*.json)|*.json|All files|*";
            fileSelector1.BrowseFile();
            filePath = fileSelector1.FilePath;
            if (!File.Exists(filePath))
            {
                Reporter.ReportError("Selected file does not exist: " + Environment.NewLine
                    + "  " + filePath + Environment.NewLine + Environment.NewLine
                    + "Select a valid file and try again.");
            } else
            {
                try
                {
                    ScalarFunctionScriptController controller = null;
                    InputOutputDataDefiniton dataDef = null;
                    ScalarFunctionScriptController.LoadJson(filePath, ref controller, ref dataDef);
                    FunctionController = controller;
                    DataDefinition = dataDef;

                    HasUnsavedChanges = false;
                    HasParentUnsavedChanges(true);
                }
                catch(Exception ex)
                {
                    Reporter.ReportError("Could not load function definition from the file. " + Environment.NewLine
                        + "  Details: " + ex.Message + Environment.NewLine
                        + "  File: " + filePath + Environment.NewLine + Environment.NewLine
                        + "Select a valid JSON file and try again.");
                }
            }
        }  // load from file



    } // class 


    }
