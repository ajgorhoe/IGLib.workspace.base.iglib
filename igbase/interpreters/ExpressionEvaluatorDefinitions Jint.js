
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






