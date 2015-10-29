
        /******************************************************************/
        /*                                                                */
        /*    BUILT-IN DEFINITIONS FOR JavaScript EXPRESSION EVALUATOR    */
        /*                                                                */
        /******************************************************************/


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
     
	 // ARRAY UTILITIES: 

	 //Check if an object is an array or not.
	 function isarray(obj) {
		//returns true is it is an array
		if (obj.constructor.toString().indexOf('Array') == -1)
		return false;
		else
		return true;
	}


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

	 // Returns minimum of function arguments. If some of the 
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







