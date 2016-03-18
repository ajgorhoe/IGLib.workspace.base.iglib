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
using System.IO;
using System.Security;

namespace IG.Forms
{
    public partial class InputOutputDataDefinitionControl : UserControl
    {

        /// <summary>Reporter used for internal reporting.</summary>
        protected IG.Lib.IReporter Reporter { get { return UtilForms.Reporter; } }


        public InputOutputDataDefinitionControl()
        {
            InitializeComponent();

            inputDataDefinitionControl1.IsInputData = true; ;
            outputDataDefinitionControl2.IsInputData = false;

            lblMainTitle.Text = TitleWithChangedSign;
        }


        #region Data

        /// <summary>Gets control used for editing definitions of input data elements.</summary>
        public InpuOrOutputtDataDefinitionControl InputDataControl { get { return inputDataDefinitionControl1; } }

        /// <summary>Gets control used for editing definitions of output data elements.</summary>
        public InpuOrOutputtDataDefinitionControl OutputDataControl { get { return outputDataDefinitionControl2; } }

        protected InputOutputDataDefiniton _dataDefinition;


        /// <summary>Data about input and output quantities of the manipulated functions or response.</summary>
        public virtual InputOutputDataDefiniton DataDefinition
        {
            get
            {
                return _dataDefinition;
            }
            set
            {
                if (!object.ReferenceEquals(value, _dataDefinition))
                {
                    this.HasUnsavedChanges = false;
                    InputOutputDataDefiniton previous = _dataDefinition;
                    _dataDefinition = value;
                    // Update child controls before updating internal data:
                    inputDataDefinitionControl1.DataDefinition = _dataDefinition;
                    outputDataDefinitionControl2.DataDefinition = _dataDefinition;
                    // Now update internal data:
                    if (_dataDefinition != null)
                    {
                        this.InputLength = DataDefinition.InputLength;
                        this.OutputLength = DataDefinition.OutputLength;
                        this.DataName = DataDefinition.Name;
                        this.DataDescription = DataDefinition.Description;
                    }
                    OnDefinitionObjectChanged(previous, _dataDefinition);
                    indicatorLight1.SetOk();
                    this.HasUnsavedChanges = false;

                }
            }
        }


        protected int _inputLength;

        /// <summary>Number of input parameters or data elements.</summary>
        public virtual int InputLength
        {
            get { return _inputLength; }
            protected set
            {
                if (value != _inputLength)
                {
                    int previous = _inputLength;
                    _inputLength = value;
                    numNumInputParameters.Value = _inputLength;
                    if (_inputLength > 0 && DataDefinition == null)
                    {
                        // Create data definition but leave to child controls to add elements:
                        DataDefinition = new InputOutputDataDefiniton();
                    }
                    // Order of the two lines below is important!
                    inputDataDefinitionControl1.SetInputLength(_inputLength);
                    outputDataDefinitionControl2.SetInputLength(_inputLength);
                    OnInputLengthChanged(previous, _inputLength);
                }
            }
        }

        protected int _outputLength;

        /// <summary>Number of input parameters or data elements.</summary>
        public virtual int OutputLength
        {
            get { return _outputLength; }
            protected set
            {
                if (value != _outputLength)
                {
                    int previous = _outputLength;
                    _outputLength = value;
                    if (_outputLength > 0 && DataDefinition == null)
                    {
                        // Create data definition but leave to child controls to add elements:
                        DataDefinition = new InputOutputDataDefiniton();
                    }
                    numNumOutputValues.Value = _outputLength;
                    // Order of the two lines below is important!
                    outputDataDefinitionControl2.SetOutputLength(_outputLength);
                    inputDataDefinitionControl1.SetOutputLength(_outputLength);
                    OnOutputLengthChanged(previous, _outputLength);
                }
            }
        }


        protected string _name;

        /// <summary>Name of the data defined by <see cref="DataDefinition"/></summary>
        public virtual string DataName
        {
            get { return _name; }
            protected set
            {
                if (value != _name)
                {
                    string previous = _name;
                    _name = value;
                    txtName.Text = _name;
                }
            }
        }


        protected string _description;

        /// <summary>Description of the data defined by <see cref="DataDefinition"/></summary>
        public virtual string DataDescription
        {
            get { return _description; }
            protected set
            {
                if (value != _description)
                {
                    string previous = _description;
                    _description = value;
                    txtDescription.Text = _description;
                }
            }
        }



        #region Data.Behavior

        private string _title = "Input and Output Data Definition";

        /// <summary>Title that is shown on the top of control, in the <see cref="lblMainTitle"/> label.</summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>Actual title that is shown on the top of control, in the <see cref="lblMainTitle"/> label, includes
        /// eventual marks for unsaved changes.</summary>
        public string TitleWithChangedSign
        {
            get
            {
                if (HasUnsavedChanges)
                    return Title + " (*) ";
                else
                    return Title;
            }
        }

        private bool _hasUnsavedChanges = false;

        /// <summary>Whether the current function definition has unsaved changes.</summary>
        public bool HasUnsavedChanges
        {
            get { return _hasUnsavedChanges; }
            set
            {
                if (value != _hasUnsavedChanges)
                {
                    _hasUnsavedChanges = value;
                    // Update displayed title (with eventual unsaved sign):
                    lblMainTitle.Text = TitleWithChangedSign;
                    if (!_hasUnsavedChanges)
                    {
                        if (DataDefinition == null)
                            indicatorLight1.SetOff();
                        else
                            indicatorLight1.SetOk();
                    }
                    else
                    {
                        indicatorLight1.SetBusy();
                    }

                }
            }
        }

        private bool _isDimentionChangeAllowed = true;

        /// <summary>Indicates whetherr user can change dimensions (number of input/output data elements).</summary>
        public bool IsDimentionChangeAllowed
        {
            get { return _isDimentionChangeAllowed; }
            set
            {
                if (value != _isDimentionChangeAllowed)
                {
                    _isDimentionChangeAllowed = value;
                    inputDataDefinitionControl1.IsDimentionChangeAllowed = _isDimentionChangeAllowed;
                    outputDataDefinitionControl2.IsDimentionChangeAllowed = _isDimentionChangeAllowed;
                }
            }
        }


        private bool _useLegalVariableNames = true;

        /// <summary>Specifies whether legal variable names (according to rules in programming languages such as C++, C# or Java) should be enforced for naming input and utput elements.</summary>
        public bool UseLegalVariableNames
        {
            get { return _useLegalVariableNames; }
            set {
                if (value != _useLegalVariableNames)
                {
                    _useLegalVariableNames = value;
                    inputDataDefinitionControl1.UseLegalVariableNames = _useLegalVariableNames;
                    outputDataDefinitionControl2.UseLegalVariableNames = _useLegalVariableNames;
                }
            }
        }

        #endregion Data.Behavior



        #endregion Data


        #region Events


        /// <summary>Occurs when data definition object is changed (i.e., when object reference changes), meaning
        /// that whole data definition object is replaced.</summary>
        public event EventHandler<ValueChangeEventArgs<InputOutputDataDefiniton>> DefinitionObjectChanged;

        /// <summary>Raises the <see cref="DefinitionObjectChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldDataDefinition">Old data definition object.</param>
        /// <param name="newDataDefinition">New data definition object.</param>
        protected void OnDefinitionObjectChanged(InputOutputDataDefiniton oldDataDefinition, InputOutputDataDefiniton newDataDefinition)
        {
            if (DefinitionObjectChanged != null)
                DefinitionObjectChanged(this, new ValueChangeEventArgs<InputOutputDataDefiniton>(oldDataDefinition, newDataDefinition));
            indicatorLight1.BlinkSpecial(Color.LightBlue, 2);
        }

        /// <summary>Occurs when number of input parameters (<see cref="InputLength"/>) is changed.</summary>
        public event EventHandler<IndexChangeEventArgs> InputLengthChanged;

        /// <summary>Raises the <see cref="InputLengthChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldLength">Old length.</param>
        /// <param name="newLength">New length.</param>
        protected void OnInputLengthChanged(int oldLength, int newLength)
        {
            if (InputLengthChanged != null)
                InputLengthChanged(this, new IndexChangeEventArgs(oldLength, newLength));
            // indicatorLight1.BlinkSpecial(Color.Magenta, 3);
        }

        /// <summary>Occurs when number of output values (<see cref="OutputLength"/>) is changed.</summary>
        public event EventHandler<IndexChangeEventArgs> OutputLengthChanged;

        /// <summary>Raises the <see cref=OutputLengthChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        /// <param name="oldLength">Old length.</param>
        /// <param name="newLength">New length.</param>
        protected void OnOutputLengthChanged(int oldLength, int newLength)
        {
            if (OutputLengthChanged != null)
                OutputLengthChanged(this, new IndexChangeEventArgs(oldLength, newLength));
             //indicatorLight1.BlinkSpecial(Color.Magenta, 3);
        }


        /// <summary>Occurs when some input element property is changed.</summary>
        public event EventHandler InputElementDataChanged;

        /// <summary>Raises the <see cref="InputElementDataChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        protected void OnInputElementDataChanged(EventArgs e)
        {
            if (InputElementDataChanged != null)
                InputElementDataChanged(this, new EventArgs());
            //indicatorLight1.BlinkSpecial(Color.Yellow, 2);
        }

        /// <summary>Occurs when some output element property is changed.</summary>
        public event EventHandler OutputElementDataChanged;

        /// <summary>Raises the <see cref="OutputElementDataChanged"/> event. This method should be called preferrably, rather than the event itself.</summary>
        protected void OnOutputElementDataChanged(EventArgs e)
        {
            if (OutputElementDataChanged != null)
                OutputElementDataChanged(this, new EventArgs());
            //indicatorLight1.BlinkSpecial(Color.Yellow, 2);
        }

        #endregion Events


        public virtual string CreateSummary()
        {
            StringBuilder sb = new StringBuilder();
            if (DataDefinition == null)
                sb.AppendLine("Data definition object is not  specified (null reference).");
            else
                sb.AppendLine("Data: ").AppendLine().AppendLine(DataDefinition.ToString());
            sb.AppendLine().AppendLine("Events: ");
            if (DefinitionObjectChanged == null)
                sb.AppendLine("DefinitionObjectChanged: no handlers set.");
            else
            {
                int numHandlers = DefinitionObjectChanged.GetInvocationList().Count();
                sb.AppendLine("DefinitionObjectChanged: " + numHandlers + " handler(s).");
            }
            if (InputLengthChanged == null)
                sb.AppendLine("InputLengthChanged: no handlers set.");
            else
            {
                int numHandlers = InputLengthChanged.GetInvocationList().Count();
                sb.AppendLine("InputLengthChanged: " + numHandlers + " handler(s).");
            }
            if (OutputLengthChanged == null)
                sb.AppendLine("OutputLengthChanged : no handlers set.");
            else
            {
                int numHandlers = OutputLengthChanged.GetInvocationList().Count();
                sb.AppendLine("OutputLengthChanged : " + numHandlers + " handler(s).");
            }
            if (InputElementDataChanged == null)
                sb.AppendLine("InputElementDataChanged : no handlers set.");
            else
            {
                int numHandlers = InputElementDataChanged.GetInvocationList().Count();
                sb.AppendLine("InputElementDataChanged : " + numHandlers + " handler(s).");
            }
            if (OutputElementDataChanged == null)
                sb.AppendLine("OutputElementDataChanged : no handlers set.");
            else
            {
                int numHandlers = OutputElementDataChanged.GetInvocationList().Count();
                sb.AppendLine("OutputElementDataChanged : " + numHandlers + " handler(s).");
            }
            // Include summaries of child controls:
            sb.AppendLine();
            sb.AppendLine("Input data definition child control:");
            sb.AppendLine(inputDataDefinitionControl1.CreateSummary());
            sb.AppendLine();
            sb.AppendLine("Output data definition child control:");
            sb.AppendLine(outputDataDefinitionControl2.CreateSummary());

            return sb.ToString();
        }


        private void menuSummary_Click(object sender, EventArgs e)
        {
            FadingMessage fm = new FadingMessage("Scalar function summay", CreateSummary(), 4000, 0.25, false);
            fm.Launch(false /* inParallelThread */);
        }

        private void numNumInputParameters_Validated(object sender, EventArgs e)
        {
            InputLength = (int)numNumInputParameters.Value;
        }

        private void numNumOutputValues_Validated(object sender, EventArgs e)
        {
            OutputLength = (int)numNumOutputValues.Value;
        }


        private void txtName_Validated(object sender, EventArgs e)
        {
            this.DataName = txtName.Text;
        }

        private void txtDescription_Validated(object sender, EventArgs e)
        {
            this.DataDescription = txtDescription.Text;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (this.HasUnsavedChanges)
            {
                DialogResult result1 = MessageBox.Show("The current control has unsaved changes! "
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
            }
            else
            {
                InputOutputDataDefiniton dataDef = null;
                string message = null;
                try
                {
                    InputOutputDataDefiniton.LoadJson(filePath, ref dataDef);
                    if (dataDef != null)
                    {
                        bool doSetDefinitions = true;
                        if (dataDef.InputLength < 1 || dataDef.OutputLength < 1)
                        {
                            string msg = "Loaded data definition has no input or output elements. File: "
                                + Environment.NewLine + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine
                                + "File format may be incorrect." + Environment.NewLine;
                            msg += "You can choose another file and try again, or you can use the empty data." + Environment.NewLine + Environment.NewLine
                                + "Do you want to use the loaded empty data?";

                            DialogResult result1 = MessageBox.Show(msg, "Load failed",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result1 != DialogResult.Yes)
                            {
                                doSetDefinitions = false;
                            }
                        }
                        if (doSetDefinitions)
                        {
                            this.DataDefinition = dataDef;
                            HasUnsavedChanges = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    //Reporter.ReportError("Could not load definition data from the file. " + Environment.NewLine
                    //    + "  Details: " + ex.Message + Environment.NewLine
                    //    + "  File: " + filePath + Environment.NewLine + Environment.NewLine
                    //    + "Select a valid JSON file and try again.");
                }
                if (dataDef == null || message != null)
                {
                    string msg = "Could not load data definition from the following file: "
                        + Environment.NewLine + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine
                        + "File format may be incorrect." + Environment.NewLine;
                    if (message != null)
                    {
                        msg += "Error message: " + Environment.NewLine
                        + message + Environment.NewLine;
                    }
                    msg += Environment.NewLine + "You can choose another file and try again." + Environment.NewLine;
                    DialogResult result1 = MessageBox.Show(msg, "Load failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

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
                    //if (DataDefinition == null && FunctionController != null)
                    //{
                    //    DataDefinition = FunctionController.GetDataDefinitionObject(true /* setBoundsAndDefaults */);
                    //}

                    InputOutputDataDefiniton.SaveJson(DataDefinition, filePath);

                    this.HasUnsavedChanges = false;

                    Reporter.ReportInfo("Data definition was saved to the following file: "
                        + Environment.NewLine + filePath);
                }
                catch (Exception ex)
                {
                    Reporter.ReportError("Saving data definition to the file failed. " + Environment.NewLine
                        + "  Details: " + ex.Message + Environment.NewLine
                        + "  File path: " + filePath
                        + Environment.NewLine + Environment.NewLine
                        + "Choose a different file path and try again.");
                }
            }

        }

        private void btnGenerateTestData_Click(object sender, EventArgs e)
        {
            if (this.HasUnsavedChanges)
            {
                DialogResult result1 = MessageBox.Show("The current control has unsaved changes! "
                    + Environment.NewLine + Environment.NewLine
                    + "Do you want to discard the changes and create a new definition from a template?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result1 != DialogResult.Yes)
                    return;
            }
            DataDefinition = InputOutputDataDefiniton.CreateDefault(InputLength, OutputLength);
            if (InputLength < 1 || OutputLength < 1)
            {
                {
                    // Blink the appropriate controls whose number of elements is 0:
                    ControlManipulator manipulator = new ControlManipulator(); // (numNumInputParameters, numNumOutputValues);
                    if (this.InputLength < 1)
                        manipulator.AddControl(numNumInputParameters);
                    if (this.OutputLength < 1)
                        manipulator.AddControl(numNumOutputValues);
                    manipulator.Blink(3, 0.1 /* blink interval in seconds */);
                }
            }
            if (InputLength < 1 && OutputLength < 1)
            {

                Reporter.ReportWarning("No elements are added to data definition. " + Environment.NewLine + Environment.NewLine
                    + "Set numbers of input and output elments greater than 0.");
            }
        }

        private void btnLoadFromFunction_Click(object sender, EventArgs e)
        {
            if (this.HasUnsavedChanges)
            {
                DialogResult result1 = MessageBox.Show("The current control has unsaved changes! "
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
            }
            else
            {
                InputOutputDataDefiniton dataDef = null;
                string message = null;
                try
                {
                    // Try to load a vector function:
                    VectorFunctionScriptController controller = null;
                    VectorFunctionScriptController.LoadJson(filePath, ref controller, ref dataDef);
                    this.DataDefinition = dataDef;
                    HasUnsavedChanges = false;
                }
                catch (Exception ex)
                { message = ex.Message; }
                if (dataDef == null)
                {
                    try
                    {
                        // Could not load a vector function from the speecified file, try also with a scalar function:
                        ScalarFunctionScriptController controller = null;
                        ScalarFunctionScriptController.LoadJson(filePath, ref controller, ref dataDef);
                        this.DataDefinition = dataDef;
                        HasUnsavedChanges = false;
                    }
                    catch (Exception ex)
                    { if (message == null) message = ex.Message; }
                }
                if (dataDef == null)
                {
                    string msg = "Could not load data definition from the following vector or scalar function file: "
                        + Environment.NewLine + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine
                        + "File format may be incorrect or the function has no input / output data definition attached with it." + Environment.NewLine;
                    if (message != null)
                    {
                        msg += "Error message: " + Environment.NewLine
                        + message + Environment.NewLine;
                    }
                    msg += Environment.NewLine + "You can choose another file and try again." + Environment.NewLine;
                    DialogResult result1 = MessageBox.Show(msg, "Load from function file failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }


        }

        private void inputDataDefinitionControl1_NumElementsChanged(object sender, IndexChangeEventArgs e)
        {
            if (e.NewDefined)
            {
                if (e.New != inputDataDefinitionControl1.InputLength)
                    throw new ArgumentException("New number of input elements specified by event arguments (" + e.New 
                        + ") does not correspond to the appropriate number on child control( " + inputDataDefinitionControl1.InputLength + ").");
                this.InputLength = e.New;  // This will raise event by calling InInputChanged().
            } else
                throw new ArgumentException("Number of input elements changed in child control, but the new number is not specified in event arguments.");
            if (inputDataDefinitionControl1.HasUnsavedChanges)
                this.HasUnsavedChanges = true;
        }

        private void outputDataDefinitionControl2_NumElementsChanged(object sender, IndexChangeEventArgs e)
        {
            if (e.NewDefined)
            {
                if (e.New != outputDataDefinitionControl2.OutputLength)
                    throw new ArgumentException("New number of output elements specified by event arguments (" + e.New
                        + ") does not correspond to the appropriate number on child control( " + outputDataDefinitionControl2.OutputLength + ").");
                this.OutputLength = e.New;  // This will raise event by calling InInputChanged().
            } else
                throw new ArgumentException("Number of input output changed in child control, but the new number is not specified in event arguments.");
            if (outputDataDefinitionControl2.HasUnsavedChanges)
                this.HasUnsavedChanges = true;
        }

        private void inputDataDefinitionControl1_DataChanged(object sender, EventArgs e)
        {
            if (inputDataDefinitionControl1.HasUnsavedChanges)
            {
                this.HasUnsavedChanges = true;
                // also raise internal event:
                OnInputElementDataChanged(e);
            }
        }

        private void outputDataDefinitionControl2_DataChanged(object sender, EventArgs e)
        {
            if (outputDataDefinitionControl2.HasUnsavedChanges)
            {
                this.HasUnsavedChanges = true;
                // also raise internal event:
                OnOutputElementDataChanged(e);
            }
        }

    }
}
