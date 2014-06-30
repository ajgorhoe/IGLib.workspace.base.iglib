
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
		if (obj.constructor.toString().indexOf(""Array"") == -1)
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






