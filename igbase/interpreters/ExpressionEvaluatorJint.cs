// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;

// using Microsoft.JScript;

using Jint.Native;
using Jint.Runtime;
using Jint;


        /***********************************************/
        /*                                             */
        /*    JavaScript INTERPRETER based oon Jint    */
        /*                                             */
        /***********************************************/


namespace IG.Lib
{


    /// <summary>JavaScript Evaluator with command-line interpreter, based on Jint.</summary>
    /// <remarks>
    /// <para>See also: https://github.com/sebastienros/jint/blob/master/README.md </para></remarks>
    /// $A Igor Oct15;
    public class ExpressionEvaluatorJint : ExpressionEvaluatorJs,
            ILockable, IIdentifiable, IRegisterable<ExpressionEvaluatorJs>
    {

        
        /// <summary>Initializes a new JavaScript evaluator. This includes compiling the JavaScript code where 
        /// evaluation is plugged in.</summary>
        public ExpressionEvaluatorJint() : base()
        { }


        #region Initialzation 

        /// <summary>Contains initializations for the particular class.</summary>
        /// <remarks>This method is called form base class' constructor.</remarks>
        protected override void InitExpressionEvaluator()
        {

            base.InitExpressionEvaluator();

            Language = "JavaScript";
            PackageName = "EvaluatorPackageJint";
            ClassName = "EvaluatorClassJs_" + Id.ToString();

            BaseDefinitions += _baseDefinitionsJs;

            _inputMark = "JSi> ";
            _definitionsMark = "JSi Def> ";
            _multilineMark = "JSi ml> ";
            // _resultMark = "      = ";

        }

        protected string BaseDefinitionsJint = @"


        //Checks if an object is an array or not.
        function isarray(obj) 
        {
        //returns true is it is an array
        if (JSON.stringify(obj)[0] == '[')
	        return true;
        else
	        return false;
        }
	 
        // Returns string representation of the argument.
        // String represeentation is obtained by JSON.stringify() 
        function str(obj) {
        return JSON.stringify(obj)
        }


";


        /// <summary>A set of pre-defined definitions that can be used in the evaluated code.</summary>
        public override string BaseDefinitions
        {
            get { return _baseDefinitionsJs; }
            protected set { base.BaseDefinitions = value; }
        }

        


        #endregion Initialization



        // Evaluation engine:


        Jint.Engine _jsEngine = null;

        /// <summary>JavaScript execution engine.</summary>
        Jint.Engine JsEngine
        {
            get
            {
                if (_jsEngine == null)
                {
                    lock (Lock)
                    {
                        if (_jsEngine == null)
                            _jsEngine = new Engine(
                                cfg => cfg.AllowClr()
                                    .AllowClr(typeof(IG.Lib.Util).Assembly)
                                //.AllowClr(typeof(IG.Gr.Poot2d).Assembly)
                                ).SetValue("writeline", new Action<object>(Console.WriteLine));
                        try
                        {
                            _jsEngine.Execute(BaseDefinitions);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(Environment.NewLine + "WARNING: " + Environment.NewLine
                                + "  Exception was hrown when interpreting basic JavaScript evaluator definitions. " + Environment.NewLine
                                + "  Message: " + ex.Message + Environment.NewLine);
                        }
                    }
                }
                return _jsEngine;
            }
        }



        #region BasicOperations


        /// <summary>Evaluates JavaScript code and returns result as object.</summary>
        /// <param name="code">JavaScript code to be evaluated.</param>
        /// <returns>Object that is result of evaluaton of code.</returns>
        public override object EvalToObject(string code)
        {
            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Executed by Jint..." + Environment.NewLine);

            JsValue val = JsEngine.Evaluate(code);  // execute a statement  // changed Execute => Evaluate
            //    .GetCompletionValue(); // get the latest statement completion value

            return val.ToObject(); // converts the value to .NET
        }


        /// <summary>Evaluates (interprets) JavaScript code and returns string result of evaluation.
        /// Code must be such that result of evaluation can be interpreted as string.</summary>
        /// <param name="code">Code that is evaluated.</param>
        /// <returns></returns>
        public override string EvalToString(string code)
        {
            JsValue val = JsEngine.Evaluate(code);  // execute a statement   // Changed Execute => Evaluate
            //     .GetCompletionValue(); // get the latest statement completion value
            string ret = val.ToString();
            return ret;
        }

        /// <summary>Executes the specified code and returns the result.
        /// Throws exception if errors occur when interpreting code.
        /// After execution, the code is appended to the complete code that has been executed up to this point.</summary>
        /// <param name="inputCode">Code that is exected by the JavaScript interpreter.</param>
        /// <returns>Result of code execution as string.</returns>
        public override string Execute(string inputCode)
        {
            if (string.IsNullOrEmpty(inputCode))
                return null;
            string result = "";
            // string codeToExecute = CompleteCode + GetRepairedCommand(inputCode) /* + ";" */ + Environment.NewLine;

            string codeToExecute = GetRepairedCommand(inputCode) /* + ";" */ + Environment.NewLine;

            // Complete input, evaluate it, print results, and reset the code:

            if (this.OutputLevel >= 1)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + "Code to execute: " + Environment.NewLine
                    + "============ Jint expr." + Environment.NewLine + codeToExecute + Environment.NewLine
                    + "------------" + Environment.NewLine + Environment.NewLine);
            }

            result = EvalToString(codeToExecute);
            // Execution OK, append the inserted code to the complete code:
            CompleteCode = codeToExecute;
            return result;
        }




        #endregion BasicOperations








    }  // ExpressionEvaluatorJint


}