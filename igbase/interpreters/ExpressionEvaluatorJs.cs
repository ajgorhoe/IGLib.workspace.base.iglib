// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
//using Microsoft.JScript;

        /********************************/
        /*                              */
        /*    JavaScript INTERPRETER    */
        /*                              */
        /********************************/

/*
 * REMARK: For a better C# engine integrated with C#, look at:  http://javascriptdotnet.codeplex.com/
 *   This library needs some native DLLs!
 *   
 * 
 * 
 * 
 * */

namespace IG.Lib
{


    /// <summary>JavaScript Evaluator with command-line interpreter.</summary>
    /// $A Igor Aug10;
    public class ExpressionEvaluatorJs : ExpressionEvaluatorCompiled,
            ILockable, IIdentifiable, IRegisterable<ExpressionEvaluatorJs>
    {

        #region Initialization

        /// <summary>Initializes a new JavaScript evaluator. This includes compiling the JavaScript code where 
        /// evaluation is plugged in.</summary>
        public ExpressionEvaluatorJs(): base()
        { }

 
        /// <summary>Contains initializations for the particular class.</summary>
        /// <remarks>This method is called form base class' constructor.</remarks>
        protected override void InitExpressionEvaluator()
        {
            Register();
            Language = "JavaScript";
            PackageName = "EvaluatorPackageJs";
            ClassName = "EvaluatorClassJs_" + Id.ToString();

            // BaseDefinitions += _baseDefinitionsJs;

            _inputMark = "JS> ";
            _definitionsMark = "JS Def> ";
            _multilineMark = "JS ml> ";
            _resultMark = "      = ";

            
            _commandLineHead = @"

JavaScript Evaluator:

Insert JavaScript code! Twice <Enter> to evaluate, empty line to exit!
/m for multiline mode, /d for definitions, /e to evaluate, /q to exit!
End line with '\\' to input multiline code, ? for help!

";
            _commandLineStopNote = @"

Command-line JavaScript expression evaluator stopped.

";
            _helpCommandLineHeading = @"
Command-line JS Evaluator Help:
Insert valid JavaScript code or special commands one line after another!
Quit with '\q' .";

            CompileBase();  // compile the code where scripts are plugged in for evaluation
        }


    // WARNING:
    // When using Jint, it does not work correctly if the initiallization string is longer than the below string.
    // Therefore, do not add anything more to the initialization string, but try ro execure additional things
    // separately!

     protected string _baseDefinitionsJs = @"

     var const_pi = Math.PI; // pi, ratio between circle circumference and diameter
	 var const_e = Math.E; // basis of natural logarithm

     var Pi = Math.PI; // pi, ratio between circle circumference and diameter
     var E = Math.E; // basis of natural logarithm
     
     function deg(x) { return x * 180 / Math.PI; }
     function rad(x) { return x * Math.PI / 180; }

	 function sin(x) { return Math.sin(x); }  // sine
     function cos(x) { return Math.cos(x); }  // cosine
     function tan (x) { return Math.tan(x); } // tangent

     function asin(x) { return Math.asin(x); }  // arcsine
     function acos(x) { return Math.acos(x); }  // arccosine
     function atan(x) { return Math.atan(x); } // arctangent
     function atan2(y,x) { return Math.atan2(y,x); } // arctangent y/x

	 function round(x) { return Math.round(x); } // x rounded to the nearest integer
     function ceil(x) { return Math.ceil(x); } // x rounded upwards to the nearest integer
     function floor(x) { return Math.floor(x); } // x, rounded downwards to the nearest integer

     function pow(x,y) { return Math.pow(x,y); }  // x^y (x raised to the power of y)
     function sqrt(x) { return Math.sqrt(x); }  // square root pf x
     function sqr(x) { return x*x; }  // x^2, x square
     
	 function exp(x) { return Math.exp(x); }  // E^x
     function log(x) { return Math.log(x); }  // natural logarithm, inverse of exp
	 function log(x,b) { return Math.log(x)/Math.log(2); } // logarithm with base b of x
	 function log2(x) { return log(x,2); }  // base 2 logarithm of x
	 function log10(x) { return log(x,10); }  // base 10 logarithm of x

     function random() { return Math.random(); } // random number between 0 and 1
     


     // SPECIAL - do not include in the basic script because there will be problems
     // - the isarray works on some JavaScript engines but not on others, and functions
     // below are dependent on it.
    
	 // ARRAY UTILITIES: 

	 //Check if an object is an array or not.
	 function isarray(obj) {
		//returns true is it is an array
		if (obj.constructor.toString().indexOf('Array') == -1)
		return false;
		else
		return true;
	}

	// Returns sum of array members (or just the argument if it is not an array)
	function sum(obj)
	{
		if (isarray(obj))
		{
			var num = obj.length;
			var ret = 0.0;
			var i;
			for (i=0; i<num; ++i)
			{
				ret = ret + obj[i];
			}
			return ret;
		} else
			return obj;
	}
  

	// Returns average of array members (or just the argument if it is not an array)
	function average(obj)
	{
		if (isarray(obj))
		{
			var num = obj.length;
			var ret = 0.0;
			var i;
			for (i=0; i<num; ++i)
			{
				ret = ret + obj[i];
			}
			return ret/num;
		} else
			return obj;
	}




";  // _baseDefinitionsJs


    public string BaseDefinitionsJsAdditional = @"


	 // Returns minimum of function arguments. If some of the 
	 // arguments are arrays, their (recursive) minimum over 
	 // all elements is returned. Recursion goes to arbitrary levels.
	 function min() {
	     var ret = Infinity;
	     var i;
	     for (i = 0; i < arguments.length; i++) {
	         var arg = arguments[i];
	         if (!isarray(arg)) {
	             if (arg < ret)
	                 ret = arg;
	         } else {
	             var j;
	             for (j = 0; j < arg.length; j++) {
	                 var el = arg[j];
	                 if (!isarray(el)) {
	                     if (el < ret)
	                         ret = el;
	                 }
	                 else
	                     ret = min(ret, el);
	             }
	         }
	     }
	     return ret;
	 }

	 // Returns maximum of function arguments. If some of the 
	 // arguments are arrays, their (recursive) minimum over 
	 // all elements is returned. Recursion goes to arbitrary levels.
	 function max() {
	     var ret = -Infinity;
	     var i;
	     for (i = 0; i < arguments.length; i++) {
	         var arg = arguments[i];
	         if (!isarray(arg)) {
	             if (arg > ret)
	                 ret = arg;
	         } else {
	             var j;
	             for (j = 0; j < arg.length; j++) {
	                 var el = arg[j];
	                 if (!isarray(el)) {
	                     if (el > ret)
	                         ret = el;
	                 }
	                 else
	                     ret = max(ret, el);
	             }
	         }
	     }
	     return ret;
	 }


	 // Returns sum of function arguments. If some of the 
	 // arguments are arrays, their (recursive) sums of 
	 // elements are added. Recursion goes to arbitrary levels.
	 function sum() {
	     var ret = 0;
	     var i;
	     for (i = 0; i < arguments.length; i++) {
	         var arg = arguments[i];
	         if (!isarray(arg)) {
	             ret = ret + arg;
	         } else {
	             var j;
	             for (j = 0; j < arg.length; j++) {
	                 var el = arg[j];
	                 if (!isarray(el))
	                     ret = ret + el;
	                 else
	                     ret = ret + sum(el);
	             }
	         }
	     }
	     return ret;
	 }

	 // Returns number of elements of all arguments. If some of the 
	 // arguments are arrays, their  (recursive) sums of number of 
	 // elements are added. 1 is added for each nontable argument.
	 // Recursion goes to arbitrary levels.
	 function numelements() {
	     var ret = 0;
	     var i;
	     for (i = 0; i < arguments.length; i++) {
	         var arg = arguments[i];
	         if (!isarray(arg)) {
	             ret = ret + 1;
	         } else {
	             var j;
	             for (j = 0; j < arg.length; j++) {
	                 var el = arg[j];
	                 if (!isarray(el))
	                     ret = ret + 1;
	                 else
	                     ret = ret + numelements(el);
	             }
	         }
	     }
	     return ret;
	 }

	 // Returns average of function arguments. If some of the 
	 // arguments are arrays, their (recursive) elements are 
	 // all taken into account. Recursion goes to arbitrary levels.
	 function average() {
	     return sum(args) / numelements(args);
	 }


	 // Concatenates all arguments after the 1st one as string, and returns the resulting string.
	 // If some arguments are arrays then their elements are appended 
	 // recursively until arbitrary depth. If some nonarray argments (or elements
	 // of array arguments) are not strings then they are converted to strings
	 // by the str() function and then appended to the returned string.
	 // If the first element is true then spaces are inserter between individual strings.
	 function concatelements(spacesBetween) {
	     var ret = "";
	     var i;
	     for (i = 1; i < arguments.length; i++) {
	         var arg = arguments[i];
	         if (!isarray(arg)) {
	             if (spacesBetween)
	                 ret = ret + ' ';
	             if (typeof arg == typeof "")
	                 ret = ret + arg;
	             else
	                 ret = ret + str(arg)
	         } else {
	             var j;
	             for (j = 0; j < arg.length; j++) {
	                 var el = arg[j];
	                 if (!isarray(el)) {
	                     if (spacesBetween)
	                         ret = ret + ' ';
	                     if (typeof el == typeof '')
	                         ret = ret + el;
	                     else
	                         ret = ret + str(arg)
	                 }
	                 else
	                     ret = ret + concatStrings(spacesBetween, el);
	             }
	         }
	     }
	     return ret;
	 }



";  // _baseDefinitionsJs

     protected string _baseDefinitionsJsExtended = @"

	 // ARRAY UTILITIES: 

	 //Check if an object is an array or not.
	 function isarray(obj) {
		//returns true is it is an array
		if (obj.constructor.toString().indexOf(""Array"") == -1)
		return false;
		else
		return true;
	}



";  // _baseDefinitionsJsExtended


     /// <summary>A set of pre-defined definitions that can be used in the evaluated code.</summary>
     public override string BaseDefinitions
     {
         get { return _baseDefinitionsJs // definitions that should work with all interpreters.
             + _baseDefinitionsJsExtended; }
         protected set { base.BaseDefinitions = value; }
     }

        #endregion Initialization

        #region OperationData

        /// <summary>Container for compiled code that evaluates JavaScript expressions.</summary>
        protected override string ScriptBase
        {
            get
            {
                return  @"
package " + PackageName +
@"
{

   class XX {
   }

   class " + ClassName + @"
   {
      public function " + EvaluationFunctionName + @"(expr : String) : String 
      {  
         var _result_of_evaluation = eval(expr);
         return eval(_result_of_evaluation); 
      }

" + Definitions +
@"

   }
}";
            }
        }  // ScriptBase

        #endregion OperationData

        #region BasicOperations


        /// <summary>Repairs the specified command and returns the repaired command string.
        /// <para>Reparations serve for easier insertion of commands and for addition of syntactic cookies.</para></summary>
        /// <param name="command">Command to be repaired.</param>
        public override void RepairCommand(ref string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                // Repair variable declaration by adding var:
                if (command.Contains("="))
                {
                    string[] splitCommand = command.Split('=');
                    if (splitCommand != null)
                        if (splitCommand.Length == 2)
                        {
                            string firstPart = splitCommand[0];
                            if (!string.IsNullOrEmpty(firstPart) && !string.IsNullOrEmpty(splitCommand[1]))
                            {
                                // Exclude cases with operators "<=", ">=", "!=", "_=", etc:
                                char lastBeforeEquals = firstPart[firstPart.Length - 1];
                                if (lastBeforeEquals != '<' && lastBeforeEquals != '>'
                                    && lastBeforeEquals != '=' && lastBeforeEquals != '!')
                                {
                                    // Exclude ordinary variable and function definitions:
                                    if (!splitCommand[0].Contains("var") && !splitCommand[0].Contains("function"))
                                        command = "var " + command;
                                }
                            }
                        }
                }
                //if (command.Contains("("))
                //    if (command.Contains("{"))
                //    {
                //        if (!command.Contains("function"))
                //            command = "function " + command;
                //    }
                int length = command.Length;
                if (length > 0)
                {
                    string trimmed = command.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                        if (trimmed[trimmed.Length-1]!=';')
                            command = command + " ;";
                }
                if (command.Contains("\""))
                {
                }
            }
        }

        #endregion BasicOperations

        #region IRegistrable Implementation
        // Comment: this includes IIdentifiable implementation, for which _idProxy is used in addition.

        /// <summary>Static object that providees object register and generates IDs 
        /// for this class:</summary>
        private static ObjectRegister<ExpressionEvaluatorJs> _register =
            new ObjectRegister<ExpressionEvaluatorJs>(1 /* first ID */);

        /// <summary>Gets object register where the current object is registered.</summary>
        public ObjectRegister<ExpressionEvaluatorJs> ObjectRegister
        { get { return _register; } }

        /// <summary>Registers the current object.
        /// Subsequent calls (after the first one) have no effect.</summary>
        public void Register()
        { _register.Register(this); }

        /// <summary>Returns true if the current object is registered, false if not.</summary>
        /// <returns></returns>
        public bool IsRegistered()
        { return _register.IsRegistered(this); }

        /// <summary>Unregisters the current object if it is currently registered. 
        /// Can be performed several times, in this case only the first call may have effect.</summary>
        public void Unregister()
        { _register.Unregister(this.Id); }

        #endregion IRegistrableImplementation


        #region Global

        protected static ExpressionEvaluatorJs _evaluatorGlobal = null;

        private static object _globalLock = new object();

        /// <summary>Global JavaScript expression evaluator.
        /// Initialized on first use.</summary>
        /// <remarks>In many applications you will not need more than one evaluator and you 
        /// can just use this instance.</remarks>
        public static ExpressionEvaluatorJs Global
        {
            get 
            {
                lock (_globalLock)
                {
                    if (_evaluatorGlobal == null)
                        _evaluatorGlobal = new ExpressionEvaluatorJs();
                    return _evaluatorGlobal;
                }
            }
            protected set
            {
                lock (_globalLock)
                {
                    _evaluatorGlobal = value;
                }
            }
        }

        #endregion Global


        #region Examples

        /// <summary>Launches command-line JavaScript interpreter.</summary>
        public static void ExampleCommandLine()
        {
            // new EvaluatorJs().CommandLine();
            Global.CommandLine();
        }

        #endregion Examples


    }  // class EvaluatorJs
}

