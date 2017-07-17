// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

// TESTING SCRIPT FILE: various examples.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

using IG.Num;
using IG.Lib;
using IG.Forms;

using IG.Plot2d;
using IG.Gr3d;

namespace IG.Script
{


    /// <summary>Internal script for running embedded applications.</summary>
    /// <remarks>
    /// <para>In the applications that have the command-line interpreter, embedded applications from this class can typically be
    /// run in the following way:</para>
    /// <para>  AppName Internal IG.Script.AppExtBase CommandName arg1 arg2 ...</para>
    /// <para>where AppName is the application name, IG.Script.AppBase is the full name of the script class that contains
    /// embedded applications, CommandName is name of the command thar launches embedded application, and arg1, arg2, etc.
    /// are command arguments for the embedded application.</para></remarks>
    /// <seealso cref="ScriptAppBase"/>
    /// $A Igor xx Feb03;
    public class AppExtBase : AppBase, ILoadableScript
    {

        public AppExtBase()
            : base()
        { }


        #region Commands


        /// <summary>Name of the command for the file system-related embedded applications.</summary>
        public const string ConstFormDemo = "FormDemo";
        public const string ConstHelpFormDemo =
@"Various windows forms-related demonstrational embedded applications. 
  Run with the '?' argument to see which applications are available.";


        #endregion Commands


        /// <summary>Adds commands to the internal interpreter.</summary>
        /// <param name="interpreter">Interpreter where commands are executed.</param>
        /// <param name="helpStrings">List containg help strings.</param>
        public override void Script_AddCommands(ICommandLineApplicationInterpreter interpreter, SortedList<string, string> helpStrings)
        {
            base.Script_AddCommands(interpreter, helpStrings);
            Script_AddCommand(interpreter, helpStrings, ConstFormDemo, AppFormDemo, ConstHelpFormDemo);
        }


        #region Actions


        #region Actions.FormDemos



        /// <summary>List of installed form demo command names.</summary>
        protected List<string> AppFormDemoNames = new List<string>();

        /// <summary>List of help strings corresponding to installed form demo commands.</summary>
        protected List<string> AppFormDemoHelpStrings = new List<string>();

        /// <summary>List of methods used to perform form demo commmands.</summary>
        protected List<CommandMethod> AppFormDemoMethods = new List<CommandMethod>();

        /// <summary>Adds a new form demonstration - related embedded application's command (added as 
        /// a sub-command of the base command named <see cref="ConstFormDemo"/>).</summary>
        /// <param name="AppName">Application name.</param>
        /// <param name="appMethod">Method used to perform the application.</param>
        /// <param name="appHelp">Eventual help string for the application.</param>
        protected void AddFormDemoCommand(string appName, CommandMethod appMethod, string appHelp)
        {
            lock (Lock)
            {
                AppFormDemoNames.Add(appName.ToLower());
                AppFormDemoHelpStrings.Add(appHelp);
                AppFormDemoMethods.Add(appMethod);
            }
        }



        #region Actions.FormDemos.TestDevelop


        public const string FormDemoTestDevelop = "TestDevelop";

        protected const string FormDemoHelpTestDevelop = FormDemoTestDevelop + @"This is only a test function used in development.
  This function is not intended for users.";


        /// <summary>Executes embedded application - a test function used in development.
        /// <para>This function is not intended for users and should be removed from the interpreter when it is not needed.</para></summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Full type name of form or control that was launched.</returns>
        protected virtual string FormDemoFunctionTestDevelop(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;

            //string className = args[0];
            //if (string.IsNullOrEmpty(className))
            //    throw new ArgumentException("Class name not specified (null or empty string).");
            //string classNameIgForms = IgFormsNamespace + "." + className;
            //Console.WriteLine(Environment.NewLine + Environment.NewLine
            //    + "Launching " + controlTypeDescriptor + ": " + className + "..." + Environment.NewLine);
            //bool blockUntilClosed = true;
            //if (numArgs > 1)
            //{
            //    Util.TryParseBoolean(args[1], ref blockUntilClosed);
            //}

            int whichTest = 0;

            if (whichTest == 0)
            {
                Console.WriteLine(Environment.NewLine + "######################" + Environment.NewLine + "Starting development tests..." + Environment.NewLine);

                Console.WriteLine("Defining scalar funciton DTO...");
                ScalarFunctionScriptController funcDto = new ScalarFunctionScriptController();
                funcDto.Name = "f";
                funcDto.ParameterNames = new string[] { "x1", "x2" };
                funcDto.ValueDefinitonString = "x1 * x1 + 2 * x2 * x2";
                funcDto.GradientDefinitionStrings = new string[] { "2 * x1", "4 * x2" };

                Console.WriteLine("Creating Scalar function by DTO...");
                IScalarFunction func = funcDto.GetFunction();

                Console.WriteLine("Evaluating scalar function...");
                for (int i = 0; i < 3; ++i)
                    for (int j = 0; j < 3; ++j)
                    {
                        IVector param = new Vector(new double[] { i, j });
                        double val = func.Value(param);
                        Console.WriteLine("  f(" + param.ToString() + ") = " + val);
                    }

                Console.WriteLine(Environment.NewLine + "Creating vector function from scalar function ...");
                IVectorFunction vecFunc = new VectorFunctionFromScalar(func);
                Console.WriteLine("Evaluating vector function...");
                try
                {
                    for (int i = 0; i < 3; ++i)
                        for (int j = 0; j < 3; ++j)
                        {
                            IVector param = new Vector(new double[] { i, j });
                            List<double> valueList = new List<double>();
                            vecFunc.Value(param, ref valueList);
                            IVector values = new Vector(valueList);
                            Console.WriteLine("  F(" + param.ToString() + ") = " + values);
                        }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception type: " + ex.GetType() + Environment.NewLine
                        + "  message: " + ex.Message + Environment.NewLine
                        + "  exception: " + ex);
                }

            }


            Console.WriteLine(Environment.NewLine + "... development tests finished." + Environment.NewLine);

            return null;
        }




        #endregion Actions.FormDemos.TestDevelop

        // OpenForm OpenForm


        #region Actions.FormDemos.Reporter



        public const string FormDemoLaunchInfo = "Info";

        public const string FormDemoLaunchWarning = "Warning";

        public const string FormDemoLaunchError = "Error";



        protected const string FormDemoHelpLaunchInfo =
            FormDemoLaunchInfo + " message : launches an informative message (by using the UtilForms reporter)." + "\n" + "  message: message text.";

        protected const string FormDemoHelpLaunchWarning =
            FormDemoLaunchWarning + " message : launches a warning message (by using the UtilForms reporter)." + "\n" + "  message: message text.";

        protected const string FormDemoHelpLaunchError =
            FormDemoLaunchError + " message : launches an error message (by using the UtilForms reporter)." + "\n" + "  message: message text.";



        /// <summary>Executes embedded application - launches an info message by the <see cref="IG.Forms.UtilForms.Reporter"/>.
        ///  Message to be shown must be passed as command argument.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Message that was launched.</returns>
        protected virtual string FormDemoFunctionLaunchInfo(string appName, string[] args)
        {
            return FormDemoFunctionLaunchReport(ReportType.Info, appName, args);
        }

        /// <summary>Executes embedded application - launches a warning message by the <see cref="IG.Forms.UtilForms.Reporter"/>.
        ///  Message to be shown must be passed as command argument.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Message that was launched.</returns>
        protected virtual string FormDemoFunctionLaunchWarning(string appName, string[] args)
        {
            return FormDemoFunctionLaunchReport(ReportType.Warning, appName, args);
        }

        /// <summary>Executes embedded application - launches an error message by the <see cref="IG.Forms.UtilForms.Reporter"/>.
        ///  Message to be shown must be passed as command argument.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Message that was launched.</returns>
        protected virtual string FormDemoFunctionLaunchError(string appName, string[] args)
        {
            return FormDemoFunctionLaunchReport(ReportType.Error, appName, args);
        }



        /// <summary>Executes embedded application - launches a message of particular kind by the <see cref="IG.Forms.UtilForms.Reporter"/>.
        ///  Message to be shown must be passed as command argument.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Message that was launched.</returns>
        protected virtual string FormDemoFunctionLaunchReport(ReportType type, string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs < 1)
            {
                throw new Exception("Message to be launched is not specified.");
            }
            string message = args[0];
            switch (type)
            {
                case ReportType.Info:
                    Console.WriteLine("Informative message is being launched...");
                    UtilForms.Reporter.ReportInfo(message);
                    break;
                case ReportType.Warning:
                    Console.WriteLine("Warning message is being launched...");
                    UtilForms.Reporter.ReportWarning(message);
                    break;
                case ReportType.Error:
                    Console.WriteLine("Error report is being launched...");
                    UtilForms.Reporter.ReportError(message);
                    break;
                default:
                    Console.WriteLine(Environment.NewLine + "ERROR: report type " + type.ToString() +
                        " is not supported by the current command." + Environment.NewLine);
                    break;
            }
            Console.WriteLine("... lauch done.");
            return message;
        }

        #endregion Actions.FormDemos.Reporter


        #region Actions.FormDemos.OpenForm

        public const string IgFormsNamespace = "IG.Forms";


        public const string FormDemoOpenForm = "OpenForm";

        public const string FormDemoOpenControl = "OpenControl";

        public const string FormDemoOpenFormOrControl = "OpenFormOrControl";

        protected const string FormDemoHelpOpenForm =
            FormDemoOpenForm + " formName <blocking> : launches the specified form. " + "\n"
            + "    formName: fully quallified name of the form class, or just a simple" + "\n"
            + "      simple name if class is in the IG.Lib namespace." + "\n"
            + "      blocking: if true then a blocking window is launched.";

        protected const string FormDemoHelpOpenControl =
            FormDemoOpenControl + " formName <blocking> : launches the specified control in a view container. " + "\n"
            + "    formName: fully quallified name of the control class, or just a simple" + "\n"
            + "      simple name if class is in the IG.Lib namespace." + "\n"
            + "      blocking: if true then a blocking window is launched.";

        protected const string FormDemoHelpOpenFormOrControl =
            FormDemoOpenFormOrControl + " formName <blocking> : launches the specified form, or control in a view container. " + "\n"
            + "    formName: fully quallified name of the form or control class, or just a simple" + "\n"
            + "      simple name if class is in the IG.Lib namespace." + "\n"
            + "      blocking: if true then a blocking window is launched.";



        /// <summary>Executes embedded application - opens the specified form. The form must either
        /// be specified with a fully qualified name of its class, or, if its class is in the <see cref="IG.Forms"/>
        /// namespace, with just a simple name of its class.</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Full type name of form that was launched.</returns>
        protected virtual string FormDemoFunctionOpenForm(string appName, string[] args)
        {
            return FormDemoFunctionOpenFormOrControl(true /* openForm */, false /* openControl */, appName, args);
        }


        /// <summary>Executes embedded application - launches the specified control. The control must either
        /// be specified with a fully qualified name of its class, or, if its class is in the <see cref="IG.Forms"/>
        /// namespace, with just a simple name of its class. The form is launched embedded in a container window 
        /// handled by the <see cref="ControlViewerForm"/> class.
        /// <para>Control class must have a argument-less contructor, construction with arguments is currently not supported.</para></summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Full type name of control that was launched.</returns>
        protected virtual string FormDemoFunctionOpenControl(string appName, string[] args)
        {
            return FormDemoFunctionOpenFormOrControl(false /* openForm */, true /* openControl */, appName, args);
        }

        /// <summary>Executes embedded application - opens the specified form or control. The form or control must either
        /// be specified with a fully qualified name of its class, or, if its class is in the <see cref="IG.Forms"/>
        /// namespace, with just a simple name of its class.
        /// <para>Form class must have a argument-less contructor, construction with arguments is currently not supported.</para></summary>
        /// <param name="openForm">If true then forms are searched for and launched.</param>
        /// <param name="openControl">If true then controls are searched for and launched in a container window handled by 
        /// the <see cref="ControlViewerForm"/> class.</param>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Full type name of form or control that was launched.</returns>
        protected virtual string FormDemoFunctionOpenFormOrControl(string appName, string[] args)
        {
            return FormDemoFunctionOpenFormOrControl(true /* openForm */, true /* openControl */, appName, args);
        }

        /// <summary>Executes embedded application - opens the specified form or control. The form or control must either
        /// be specified with a fully qualified name of its class, or, if its class is in the <see cref="IG.Forms"/>
        /// namespace, with just a simple name of its class.
        /// <para>Control or form class must have a argument-less conttructor, construction with arguments is currently not supported.</para></summary>
        /// <param name="openForm">If true then forms are searched for and launched.</param>
        /// <param name="openControl">If true then controls are searched for and launched in a container window handled by 
        /// the <see cref="ControlViewerForm"/> class.</param>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Full type name of form or control that was launched.</returns>
        protected virtual string FormDemoFunctionOpenFormOrControl(bool openForm, bool openControl, string appName, string[] args)
        {
            string controlTypeDescriptor = null;
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (openForm)
            {
                if (openControl)
                {
                    controlTypeDescriptor = "form or control";
                }
                else
                {
                    controlTypeDescriptor = "form";
                }
            }
            else
            {
                controlTypeDescriptor = "control";
            }
            if (numArgs < 1)
            {
                throw new Exception("Name of the " + controlTypeDescriptor + " to be launched is not specified.");
            }
            if (!openForm && !openControl)
                throw new ArgumentException("Invalid type flag: neither control nor form flag is specified.");
            string className = args[0];
            if (string.IsNullOrEmpty(className))
                throw new ArgumentException("Class name not specified (null or empty string).");
            string classNameIgForms = IgFormsNamespace + "." + className;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Launching " + controlTypeDescriptor + ": " + className + "..." + Environment.NewLine);
            bool blockUntilClosed = true;
            if (numArgs > 1)
            {
                Util.TryParseBoolean(args[1], ref blockUntilClosed);
            }
            Type controlObjectType = null;
            controlObjectType = UtilSystem.GetType(className, false /* ignoreCase */);
            if (controlObjectType == null)
            {
                controlObjectType = UtilSystem.GetType(classNameIgForms, false /* ignoreCase */);
            }
            if (controlObjectType == null)
                throw new ArgumentException("Could not locate a class named " + className + " or " + classNameIgForms + ".");
            if (!openControl && !controlObjectType.IsSubclassOf(typeof(System.Windows.Forms.Form)))
            {
                // We intend to open a windows form, but the object type is not a Form type:
                throw new InvalidCastException("The specified type " + controlObjectType.FullName
                    + " does not inherit from System.Windows.Forms.Form.");
            }
            if (!openForm && controlObjectType.IsSubclassOf(typeof(System.Windows.Forms.Form)))
            {
                // We intend to open a windows form, but the object type is not a Form type:
                throw new InvalidCastException("The specified type " + controlObjectType.FullName
                    + " inherits from System.Windows.Forms.Form, but should only inherit from Control.");
            }
            if (!controlObjectType.IsSubclassOf(typeof(System.Windows.Forms.Control)))
            {
                // Object type is not of a Control type (Form is also of this type, so we need no additional condition):
                throw new InvalidCastException("The specified type " + controlObjectType.FullName
                    + " does not inherit from System.Windows.Forms.Control.");
            }
            object controlObject = System.Activator.CreateInstance(controlObjectType);
            if (controlObject == null)
                throw new ArgumentException("Can not instantiate an object of type " + controlObjectType.FullName
                    + ". Possibly the class lacks accessible argument-less constructor.");
            Form formObject = controlObject as Form;
            if (formObject != null)
            {
               
                if (blockUntilClosed)
                    formObject.ShowDialog();
                else
                    formObject.Show();
            }
            else
            {
                Control viewedControl = controlObject as Control;
                if (viewedControl == null)
                    throw new InvalidCastException("Unexpected error: could not cast the object to type System.Windows.Forms.Control.");
                Form formContainer = new ControlViewerForm(viewedControl);
                if (formContainer == null)
                    throw new InvalidOperationException("Unexpected error: Could not launch a control viewer with the created control.");
                if (blockUntilClosed)
                    formContainer.ShowDialog();
                else
                    formContainer.Show();
            }
            Console.WriteLine(Environment.NewLine + "Launched " + controlTypeDescriptor + " of type " + controlObjectType + "."
                + Environment.NewLine);
            return controlObjectType.FullName;
        }



        // TODO: replace the FormDemoFunctionOpenFormOld(...) method!

        /// <summary>Executes embedded application - opens the specified form. The form must either
        /// be specified with a fully qualified name of its class, or, if its class is in the IG.Lib
        /// namespace, with just a simple name of its class (or more precisel, part of the name that
        /// comes after "IG.Lib.").</summary>
        /// <param name="appName">Name of the embedded application.</param>
        /// <param name="args">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        /// [Obsolete("Should be removed in some time.")]
        protected virtual string FormDemoFunctionOpenFormOld(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            if (numArgs < 1)
                throw new Exception("Name of the form to be launched is not specified.");
            string formClassName1 = args[0];
            string formClassName2 = "IG.Forms." + args[0];
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Opening form: " + formClassName1 + "..." + Environment.NewLine);
            bool blockUntilClosed = true;
            if (numArgs > 1)
            {
                Util.TryParseBoolean(args[1], ref blockUntilClosed);
            }
            Form form = null;
            Type formType = null;
            // Check for the form type in the executable assembly:
            formType = Assembly.GetEntryAssembly().GetType(formClassName1, false);
            if (formType == null)
                formType = Assembly.GetEntryAssembly().GetType(formClassName2, false);
            // Form type not found, try to locate it in the currently executing assembly:
            if (formType == null)
                formType = Assembly.GetExecutingAssembly().GetType(formClassName1, false);
            if (formType == null)
                formType = Assembly.GetExecutingAssembly().GetType(formClassName2, false);
            // The type with specified name was not find in entry or in currently executing 
            // assembly, check all referenced assemblies:
            if (formType == null)
            {
                Assembly[] assemblies = UtilSystem.GetLoadedAssemblies();
                int numAssemblies = 0;
                if (assemblies != null)
                    numAssemblies = assemblies.Length;
                for (int whichAsm = 0; whichAsm < numAssemblies && formType == null; ++whichAsm)
                {
                    Assembly asm = assemblies[whichAsm];
                    try
                    {
                        formType = asm.GetType(formClassName1, false);
                        if (formType == null)
                            formType = asm.GetType(formClassName2, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(Environment.NewLine + "Exception thrown when trying to locate the form type "
                            + formClassName1 + Environment.NewLine + "  in assembly \"" + asm.FullName + "\". Details: "
                            + Environment.NewLine + ex.Message);
                    }
                    if (formType != null)
                        break;
                }
                if (formType == null)
                {
                    assemblies = UtilSystem.GetReferencedAssemblies();
                    numAssemblies = 0;
                    if (assemblies != null)
                        numAssemblies = assemblies.Length;
                    for (int whichAsm = 0; whichAsm < numAssemblies && formType == null; ++whichAsm)
                    {
                        Assembly asm = assemblies[whichAsm];
                        try
                        {
                            formType = asm.GetType(formClassName1, false);
                        }
                        catch { }
                        try
                        {
                            if (formType == null)
                                formType = asm.GetType(formClassName2, false);
                        }
                        catch { }
                        if (formType != null)
                            break;
                    }
                }
            }
            if (formType == null)
                throw new ArgumentException("Form class " + formClassName1 + " could not be found in the executing and referenced assemblies.");
            else
            {
                form = (Form)Activator.CreateInstance(formType);
                if (form != null)
                {
                    if (blockUntilClosed)
                        form.ShowDialog();
                    else
                        form.Show();
                }
            }
            return null;
        }




        #endregion Actions.FormDemos.OpenForm


        #region Actions.FormDemos.FadingMessage

        public const string FormDemoFadingMessage = "FadingMessage";

        protected const string FormDemoHelpFadingMessage = FormDemoFadingMessage + " : Runs the fading message demo.";

        /// <summary>Executes embedded application - demonstration of fading messages.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionFadingMessage(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Runnning the fading message demo..." + Environment.NewLine);

            Console.WriteLine("Running 1st example ...");
            FadingMessage.Example();

            Console.WriteLine("Running 2nd example...");
            FadingMessage.ExampleInferior();

            Console.WriteLine(Environment.NewLine + "Fading message demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.FormDemos.FadingMessage


        #region Actions.FormDemos.BrowserSimple

        public const string FormDemoBrowserSimple = "Browser";

        protected const string FormDemoHelpBrowserSimple = FormDemoBrowserSimple + " : Runs a browser.";

        /// <summary>Executes embedded application - demonstration of fading messages.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionBrowserSimple(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Running the simple web browser demo..." + Environment.NewLine);

            BrowserSimpleForm browser = new BrowserSimpleForm();
            try
            {
                Application.Run(browser);
            }
            catch
            {
                browser.ShowDialog();
            }

            Console.WriteLine(Environment.NewLine + "Fading message demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.FormDemos.BrowserSimple


        #region Actions.FormDemos.WindowPositioning

        public const string FormDemoWindowPositioning = "WindowPositioning";

        protected const string FormDemoHelpWindowPositioning = FormDemoWindowPositioning + " : Runs the window positioning test.";

        /// <summary>Executes embedded application - window positioning test.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionWindowPositioning(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Running the window positioning test..." + Environment.NewLine);

            WindowPositionerForm testForm = new WindowPositionerForm();
            try
            {
                Application.Run(testForm);
            }
            catch
            {

            }

            Console.WriteLine(Environment.NewLine + "Window positioning test finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        // BrowserSimple BrowseerSimple

        #endregion Actions.FormDemos.WindowPositioning



        #region Actions.FormDemos.MessageBoxes

        public const string FormDemoMessageBoxLauncher = "MessageBoxLauncher";

        protected const string FormDemoHelpMessageBoxLauncher = FormDemoMessageBoxLauncher + " : Runs the window positioning test.";

        /// <summary>Executes embedded application - messaxe box launcher demo.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionMessageBoxLauncher(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Running the message box launcher demonstration..." + Environment.NewLine);

            MessageBoxLauncher testForm = new MessageBoxLauncher();
            try
            {
                Application.Run(testForm);
            }
            catch
            {

            }

            Console.WriteLine(Environment.NewLine + "Maeesge box launcher demo finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.FormDemos.MessageBoxes



        #region Actions.FormDemos.StopWatch

        public const string FormDemoStopWatch = "StopWatch";

        protected const string FormDemoHelpStopWatch = FormDemoStopWatch + "  <blocking> : Runs a StopWatch." + "\n"
            + "      blocking: if true then a blocking window is launched.";

        /// <summary>Executes embedded application - a stopwatch.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionStopWatch(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Running the stopwatch application..." + Environment.NewLine);
            bool blockUntilClosed = true;
            if (numArgs > 1)
            {
                Util.TryParseBoolean(args[1], ref blockUntilClosed);
            }
            TimerForm testForm = new TimerForm();
            try
            {
                if (blockUntilClosed)
                {
                    testForm.ShowDialog();
                    //Application.Run(testForm);
                }
                else
                {
                    testForm.Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown: " + ex.Message);
            }

            Console.WriteLine(Environment.NewLine + "Stopwatch application finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.FormDemos.StopWatch



        #region Actions.FormDemos.CookingTimer

        public const string FormDemoCookingTimer = "CookingTimer";

        protected const string FormDemoHelpCookingTimer = FormDemoStopWatch
            + "  <blocking> : Runs a cooking timer with countdown and alarm."
            + "      blocking: if true then a blocking window is launched.";

        /// <summary>Executes embedded application - a cooking timer with a countdown and alarm.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        /// <returns>Number of arguments passed.</returns>
        protected virtual string FormDemoFunctionCookingTimer(string appName, string[] args)
        {
            int numArgs = 0;
            if (args != null)
                numArgs = args.Length;
            Console.WriteLine(Environment.NewLine + Environment.NewLine
                + "Running the cooking timer application..." + Environment.NewLine);
            bool blockUntilClosed = true;
            if (numArgs > 1)
            {
                Util.TryParseBoolean(args[1], ref blockUntilClosed);
            }
            CookingTimerForm testForm = new CookingTimerForm();
            try
            {
                if (blockUntilClosed)
                {
                    // Application.Run(testForm);
                    testForm.ShowDialog();
                }
                else
                {
                    testForm.Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown: " + ex.Message);
            }

            Console.WriteLine(Environment.NewLine + "Cooking timer application finished." + Environment.NewLine);
            return numArgs.ToString();
        }

        #endregion Actions.FormDemos.CookingTimer



        protected bool _appFormDemoCommandsInitialized = false;

        /// <summary>Initializes commands for form demo related utilities (embedded applications).</summary>
        protected virtual void InitAppFormDemo()
        {

            lock (Lock)
            {
                if (_appFormDemoCommandsInitialized)
                    return;
                AddFormDemoCommand(FormDemoOpenForm + "Old", FormDemoFunctionOpenFormOld, FormDemoHelpOpenForm);

#if DEBUG
                AddFormDemoCommand(FormDemoTestDevelop, FormDemoFunctionTestDevelop, FormDemoHelpTestDevelop);
#endif

                AddFormDemoCommand(FormDemoLaunchInfo, FormDemoFunctionLaunchInfo, FormDemoHelpLaunchInfo);
                AddFormDemoCommand(FormDemoLaunchWarning, FormDemoFunctionLaunchWarning, FormDemoHelpLaunchWarning);
                AddFormDemoCommand(FormDemoLaunchError, FormDemoFunctionLaunchError, FormDemoHelpLaunchError);

                AddFormDemoCommand(FormDemoOpenForm, FormDemoFunctionOpenForm, FormDemoHelpOpenForm);
                AddFormDemoCommand(FormDemoOpenFormOrControl, FormDemoFunctionOpenFormOrControl, FormDemoHelpOpenFormOrControl);
                AddFormDemoCommand(FormDemoOpenControl, FormDemoFunctionOpenControl, FormDemoHelpOpenControl);

                AddFormDemoCommand(FormDemoFadingMessage, FormDemoFunctionFadingMessage, FormDemoHelpFadingMessage);
                AddFormDemoCommand(FormDemoBrowserSimple, FormDemoFunctionBrowserSimple, FormDemoHelpBrowserSimple);
                AddFormDemoCommand(FormDemoWindowPositioning, FormDemoFunctionWindowPositioning, FormDemoHelpWindowPositioning);
                AddFormDemoCommand(FormDemoMessageBoxLauncher, FormDemoFunctionMessageBoxLauncher, FormDemoHelpMessageBoxLauncher);
                AddFormDemoCommand(FormDemoStopWatch, FormDemoFunctionStopWatch, FormDemoHelpStopWatch);
                AddFormDemoCommand(FormDemoCookingTimer, FormDemoFunctionCookingTimer, FormDemoHelpCookingTimer);

                _appFormDemoCommandsInitialized = true;
            }
        }



        /// <summary>Runs a form demo - related utility (embedded application) according to arguments.</summary>
        /// <param name="AppArguments">Arguments. 0-th argument is the base command name, 1st argument is the embedded application name, and the rest
        /// are arguments that are used by the embedded application.</param>
        protected virtual string RunAppFormDemo(string[] args)
        {
            InitAppFormDemo();
            if (args == null)
                throw new ArgumentException("No arguments. Embedded application name (or '?' for help) should be specified (null argument array).");
            if (args.Length < 2)
                throw new ArgumentException("Test name (or '?' for help) should be specified (less than 2 arguments).");
            if (string.IsNullOrEmpty(args[1]))
                throw new ArgumentException("Test name (or '?' for help) not specified (null or empty string argument).");
            if (args != null)
                if (args.Length >= 2)
                    if (args[1] == "?")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Usage: " + args[0] + " ApplicationName arg1 arg2...");
                        Console.WriteLine(args[0] + " ApplicationName ? : prints help.");
                        Console.WriteLine();
                        Console.WriteLine("List of embedded applications: ");
                        for (int i = 0; i < AppFormDemoNames.Count; ++i)
                            Console.WriteLine("  " + AppFormDemoNames[i]);
                        Console.WriteLine();
                        return null;
                    }
            string testName = args[1];
            string[] appParams = new string[args.Length - 2];
            for (int i = 0; i < appParams.Length; ++i)
                appParams[i] = args[i + 2];
            int index = AppFormDemoNames.IndexOf(testName.ToLower());
            if (index < 0)
                throw new ArgumentException("Embedded application named " + testName + " is not found. Call with '?' for list of application names.");
            testName = AppFormDemoNames[index];
            string helpString = AppFormDemoHelpStrings[index];
            CommandMethod method = AppFormDemoMethods[index];
            if (appParams.Length >= 1)
                if (appParams[0] == "?")
                {
                    // Print help for the specified surface:
                    Console.WriteLine();
                    Console.WriteLine("Test " + testName + ": ");
                    if (string.IsNullOrEmpty(AppFormDemoHelpStrings[index]))
                        Console.WriteLine("  No help available.");
                    Console.WriteLine(AppFormDemoHelpStrings[index]);
                    Console.WriteLine();
                    return null;
                }
            try
            {
                return method(testName, appParams);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("ERROR: " + ex.Message);
                Console.WriteLine("Usage: " + Environment.NewLine + helpString + Environment.NewLine);
                throw;
            }
        }

        /// <summary>Runs one of the form demo - related embedded applications.</summary>
        /// <param name="arguments">Array containing the base command name, application command name and arguments.</param>
        public virtual string AppFormDemo(string[] arguments)
        {
            string ret = null;
            if (arguments == null)
                throw new ArgumentException("Commandline arguments not specified (null argument).");
            if (arguments.Length < 2)
                Console.WriteLine("Number of arguments should be at least 2 (at least base command name & application name).");
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Running form demo - related embedded application..."
                + Environment.NewLine +
                "=============================="
                + Environment.NewLine);
            //Console.WriteLine();
            //Script_PrintArguments("Application arguments: ", arguments);
            //Console.WriteLine();

            if (ret == null)
                ret = RunAppFormDemo(arguments);


            Console.WriteLine("==============================");
            Console.WriteLine("Form demo - related application  finished.");
            Console.WriteLine();
            return ret;
        }  // AppFormDemo


        #endregion Actions.FormDemos


        #region Actions.CryptoUtilities_Inherited


        public const string CryptoHashForm = "HashForm";
        public const string CryptoHashDirForm = "HashDirForm";
        public const string CryptoHashAllForm = "HashAllForm";

        protected const string CryptoHelpHashForm = CryptoHashForm +
@": Launches a GUI window for calculation of hash values for files and text.";

        protected const string CryptoHelpHashDirForm = CryptoHashDirForm +
@": Launches a GUI window for calculation of hash values for all files in a directory.";

        protected const string CryptoHelpHashAllForm = CryptoHashAllForm +
@": Launches a GUI window for calculation of hash values for files and text, or all files in a directory.";

        protected HashForm hashForm;

        /// <summary>Executes embedded application - launches a windows form for calculation of 
        /// various hash values of a file.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        protected virtual string CryptoFunctionHashForm(string appName, string[] args)
        {
            Console.WriteLine(Environment.NewLine + "Launching a form for calculation of hash values..." + Environment.NewLine);

            lock (Lock)
            {
                try
                {
                    UtilConsole.HideConsoleWindow();
                    if (hashForm == null)
                        hashForm = new HashForm();
                    hashForm.ShowDialog();
                }
                finally
                {
                    UtilConsole.ShowConsoleWindow();
                }
            }
            Console.WriteLine("Form closed.");
            return null;
        }


        protected HashDirForm hashDirForm;

        /// <summary>Executes embedded application - launches a windows form for calculation of 
        /// various hash values of all files in a directory.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fo the embedded application's command.</param>
        protected virtual string CryptoFunctionHashDirForm(string appName, string[] args)
        {
            Console.WriteLine(Environment.NewLine + "Launching a form for calculation of hash values of all files in a directory..." + Environment.NewLine);

            lock (Lock)
            {
                try
                {
                    UtilConsole.HideConsoleWindow();
                    if (hashDirForm == null)
                        hashDirForm = new HashDirForm();
                    hashForm.ShowDialog();
                }
                finally
                {
                    UtilConsole.ShowConsoleWindow();
                }
            }

            Console.WriteLine("Form closed.");
            return null;
        }


        protected HashAllForm hashAllForm;

        /// <summary>Executes embedded application - launches a windows form for calculation of 
        /// various hash values of a file, text, or all files in a drectory.</summary>
        /// <param name="AppName">Name of the embedded application.</param>
        /// <param name="AppArguments">Arguments fro the embedded application's command.</param>
        protected virtual string CryptoFunctionHashAllForm(string appName, string[] args)
        {
            Console.WriteLine(Environment.NewLine + "Launching a form for calculation of hash values of text, file, or all files in a directory ..." + Environment.NewLine);

            lock (Lock)
            {
                try
                {
                    UtilConsole.HideConsoleWindow();
                    if (hashAllForm == null)
                        hashAllForm = new HashAllForm();
                    hashAllForm.ShowDialog();
                }
                finally
                {
                    UtilConsole.ShowConsoleWindow();
                }
            }

            Console.WriteLine("Form closed.");
            return null;
        }




        /// <summary>Initializes commands for cryptography related utilities (embedded applications).
        /// <para>Here the method from the base class is overridden in order to add some additional utilities.</para></summary>
        protected override void InitAppCrypto()
        {

            lock (Lock)
            {
                if (_appCryptoCommandsInitialized)
                    return;
                AddCryptoCommand(CryptoHashForm, CryptoFunctionHashForm, CryptoHelpHashForm);
                AddCryptoCommand(CryptoHashDirForm, CryptoFunctionHashDirForm, CryptoHelpHashDirForm);
                AddCryptoCommand(CryptoHashAllForm, CryptoFunctionHashAllForm, CryptoHelpHashAllForm);


                base.InitAppCrypto();
                _appCryptoCommandsInitialized = true;
            }
        }

        #endregion Actions.CryptoUtilities_Inherited

        #endregion Actions


    }  // class AppExtBase

}