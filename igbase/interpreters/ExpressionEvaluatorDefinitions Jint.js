
        /*******************************************************************************/
        /*                                                                             */
        /*    BUILT-IN DEFINITIONS FOR JavaScript EXPRESSION EVALUATOR BASED ON Jint   */
        /*                                                                             */
        /*******************************************************************************/

// Jint INCOMPATIBILITIES:
// Checking whether something (say obj) is na array does not work in this way:
//     obj.constructor.toString().indexOf('Array')
//  obj.constructor for arrays gives just "function() { ... }" and does not contain the word array.
//
// USEFUL INCOMPATIBILITIES:
// Variable definitions work without the var keyword. Because of this, input correctios are not 
// necessary for making calcullator more comfortable.
// E.g., this will work (with all variables undefined): c2 = 1; c3 = 2; c4 = 3;
// 
// STRONGHOODS:
// The JSON object is provided. JSON.stringify() is useful for conversion of objects to strings.
// This is also useful for the print function.
// 
// 
// DRAWBACK:
// When evaluating single expressions such as var "aa = 11;", "undefined" is returned.
// Because of this, additional check and correction must be performed to provide a comfortable calculator.
// 
// Internal conversion to strings (obj.toString() method) does not wor well. For example, a table [1, 2]
// is converted to "1,2". Aditional conversion function must be provided in order to enable a more
// comfortabla calculator.



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






