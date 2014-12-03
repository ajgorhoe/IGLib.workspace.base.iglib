// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Lib;

namespace IG.Num
{

    /// <summary>Vector function containing lineer basis in a n-dimensional vector space.
    /// Composed of n+1 functions: 1, x1, ..., xn.</summary>
    /// $A Igor xx Apr10;
    public class LinearBasis : VectorFunctionBaseComponentWise, IVectorFunction, 
            ILockable
    {

        private LinearBasis() { }

        /// <summary>Constructs a vector function containing linear basis for the specified dimension.
        /// Composed of n+1 functions: 1, x1, ..., xn.</summary>
        /// <param name="dimension">Space dimension.</param>
        public LinearBasis(int dimension)
        {
            NumParameters = dimension;
            NumValues = 1 + dimension;
            Name = "LinearBasis" + dimension + "D";
            Description = "Linear basis in " + dimension + " dimensions.";
        }

        #region Evaluation

        #region ComponentWise

        /// <summary>Calculates and returns the particular component of the vector
        /// function value.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts. If the function does not support component-wise evaluation
        /// then results will be stored to this structure and returned from it. This makes reuse
        /// possible - when different components are evaluated subsequently with the same parameters,
        /// results are calculated only for the first time.</param>
        /// <param name="which">Specifies which function to evaluate.</param>
        public override double Value(IVectorFunctionResults evaluationData, int which)
        {
            if (which == 0)
                return 1;
            else
                return evaluationData.GetParameter(which-1);
        }

        /// <summary>Calculates and returns the particular component of the vector
        /// function's derivative.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts.
        /// Not used in this class, can be null.</param>
        /// <param name="which">Specifies which function to take.</param>
        /// <param name="component">Specifies which compoonent of the gradient should be returned.</param>
        public override double Derivative(IVectorFunctionResults evaluationData, int which,
            int component)
        {
            if (component + 1 == which)
                return 1;
            else
                return 0;
        }

        /// <summary>Calculates and returns the particular component of the vector
        /// function's second derivative (Hessian), which is always 0 in this case.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts. 
        /// Not used in this class, can be null.</param>
        /// <param name="which">Specifies which function to take.</param>
        /// <param name="rowNum">Specifies which row of the Hessian (matrix of second derivatives) should 
        /// be returned.</param>
        /// <param name="columnNum">Specifies which column of the Hessian (matrix of second derivatives) should 
        /// be returned.</param>
        public override double SecondDerivative(IVectorFunctionResults evaluationData, int which,
            int rowNum, int columnNum)
        {
            return 0;
        }

        #endregion ComponentWise

        #endregion Evaluation


        /// <summary>Examples for this class.</summary>
        public static void Example()
        {
            Console.WriteLine(Environment.NewLine + "Basis functions tests:");
            int dim = 4;
            IVectorFunction basis = new LinearBasisSafer(dim);
            Vector param = new Vector(dim);
            param[0] = 0.001;
            param[1] = 1.002;
            param[2] = 2.003;
            param[3] = 3.004;
            VectorFunctionResults res = new VectorFunctionResults();
            res.SetParametersReference(param);
            res.ReqValues = res.ReqGradients = true;
            res.ReqHessians = true;
            basis.Evaluate(res);
            Console.WriteLine("Evaluation of linear basis functions in " + dim + " dimensions: ");
            Console.WriteLine(res.ToString());
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(Environment.NewLine + "Basis functions tests:");
            dim = 4;
            basis = new QuadraticBasisSafer(dim);
            param = new Vector(dim);
            param[0] = 0.001;
            param[1] = 1.002;
            param[2] = 2.003;
            param[3] = 3.004;
            res = new VectorFunctionResults();
            res.SetParametersReference(param);
            res.ReqValues = res.ReqGradients = true;
            res.ReqHessians = true;
            basis.Evaluate(res);
            Console.WriteLine("Evaluation of linear basis functions in " + dim + " dimensions: ");
            Console.WriteLine(res.ToString());
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine(Environment.NewLine + "Basis functions tests:");
            dim = 2;
            basis = new QuadraticBasisSafer(dim);
            param = new Vector(dim);
            param[0] = 0.001;
            param[1] = 1.002;
            res = new VectorFunctionResults();
            res.SetParametersReference(param);
            res.ReqValues = res.ReqGradients = true;
            res.ReqHessians = true;
            basis.Evaluate(res);
            Console.WriteLine("Evaluation of linear basis functions in " + dim + " dimensions: ");
            Console.WriteLine(res.ToString());
            Console.WriteLine();
            Console.WriteLine();
        }

    }  // class LinearBasis


    /// <summary>The same as LinearBasis, just that it has more meaningful error reporting 
    /// (but is therefore slower).</summary>
    /// $A Igor xx Apr10;
    public class LinearBasisSafer : LinearBasis
    {

        public LinearBasisSafer(int dimension)
            : base(dimension)
        {
        }

        #region Evaluation

        #region ComponentWise

        /// <summary>Calculates and returns the particular component of the vector
        /// function value.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts. If the function does not support component-wise evaluation
        /// then results will be stored to this structure and returned from it. This makes reuse
        /// possible - when different components are evaluated subsequently with the same parameters,
        /// results are calculated only for the first time.</param>
        /// <param name="which">Specifies which function to evaluate.</param>
        public override double Value(IVectorFunctionResults evaluationData, int which)
        {
            if (evaluationData == null)
                throw new ArgumentException("Linear basis component: Evaluation data holding parameters is not specified.");
            if (evaluationData.Parameters == null)
                throw new ArgumentException("Linear basis component: Evaluation data does not contain parameters.");
            if (which < 0 || which >= NumValues)
                throw new ArgumentException("Linear basis component: function index out of range."
                    + Environment.NewLine + "  Index: " + which + ", should be between 0 and "
                    + (NumValues - 1) + "in " + NumParameters + "dimensions.");
            if (which == 0)
                return 1;
            else
                return evaluationData.GetParameter(which - 1);
        }

        /// <summary>Calculates and returns the particular component of the vector
        /// function's derivative.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts.
        /// Not used in this class, can be null.</param>
        /// <param name="which">Specifies which function to take.</param>
        /// <param name="component">Specifies which compoonent of the gradient should be returned.</param>
        public override double Derivative(IVectorFunctionResults evaluationData, int which,
            int component)
        {
            if (evaluationData == null)
                throw new ArgumentException("Linear basis component: Evaluation data holding parameters is not specified.");
            if (evaluationData.Parameters == null)
                throw new ArgumentException("Linear basis component: Evaluation data does not contain parameters.");
            if (which < 0 || which >= NumValues)
                throw new ArgumentException("Linear basis derivative component: function index out of range."
                    + Environment.NewLine + "  Index: " + which + ", should be between 0 and "
                    + (NumValues - 1) + "in " + NumParameters + "dimensions.");
            if (component < 0 || component >= NumParameters)
                throw new ArgumentException("Linear basis derivative component: variable index out of range."
                    + Environment.NewLine + "  Index: " + component + ", should be between 0 and "
                    + (NumParameters - 1) + " in " + NumParameters + " dimensions.");
            if (component + 1 == which)
                return 1;
            else
                return 0;
        }

        /// <summary>Calculates and returns the particular component of the vector
        /// function's second derivative (Hessian), which is always 0 in this case.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts. 
        /// Not used in this class, can be null.</param>
        /// <param name="which">Specifies which function to take.</param>
        /// <param name="rowNum">Specifies which row of the Hessian (matrix of second derivatives) should 
        /// be returned.</param>
        /// <param name="columnNum">Specifies which column of the Hessian (matrix of second derivatives) should 
        /// be returned.</param>
        public override double SecondDerivative(IVectorFunctionResults evaluationData, int which,
            int rowNum, int columnNum)
        {
            if (evaluationData == null)
                throw new ArgumentException("Linear basis component: Evaluation data holding parameters is not specified.");
            if (evaluationData.Parameters == null)
                throw new ArgumentException("Linear basis component: Evaluation data does not contain parameters.");
            if (which < 0 || which >= NumValues)
                throw new ArgumentException("Linear basis second derivative component: function index out of range."
                    + Environment.NewLine + "  Index: " + which + ", should be between 0 and "
                    + (NumValues - 1) + "in " + NumParameters + "dimensions.");
            if (rowNum < 0 || rowNum >= NumParameters || columnNum<0 || columnNum>=NumParameters)
                throw new ArgumentException("Linear basis second derivative component: variable indices out of range."
                    + Environment.NewLine + "  Indices: [" + rowNum + ", " + columnNum
                    + "], should be between 0 and " + (NumParameters - 1) + " in " + NumParameters
                    + " dimensions.");
            return 0;
        }

        #endregion ComponentWise

        #endregion Evaluation


        /// <summary>Example for this class, just runs examples form the LinearBasis class.</summary>
        public new static void Example()
        {
            LinearBasis.Example();
        }


    }  // class LinearBasisSafer



    /// <summary>Vector function containing quadratic basis in a n-dimensional vector space.
    /// Composed of (n+1)*(n+2)/2 functions: 1, x_1, ..., x_n, 0.5*x_1^2, 0.5*x_2^2, ..., 0.5*x_n^2,
    /// x_1*x_2, x_1*x_3, ..., x_1*x_n, ..., x_2*x_3, ..., x_2*x_n, ..., ..., x_n-1*x_n.</summary>
    /// $A Igor xx Apr10;
    public class QuadraticBasis : VectorFunctionBaseComponentWise, IVectorFunction,
            ILockable
    {

        private QuadraticBasis() {  }

        /// <summary>Constructs a vector function containing quadratic basis for the specified dimension.
        /// Composed of (n+1)*(n+2)/2 functions: 1, x_1, ..., x_n, 0.5*x_1^2, 0.5*x_2^2, ..., 0.5*x_n^2,
        /// x_1*x_2, x_1*x_3, ..., x_1*x_n, ..., x_2*x_3, ..., x_2*x_n, ..., ..., x_n-1*x_n.</summary>
        /// <param name="dimension">Space dimension.</param>
        public QuadraticBasis(int dimension)
        {
            Name = "QuadraticBasis" + dimension + "D";
            Description = "Quadratic basis in " + dimension + " dimensions.";
            int numFunc = (dimension+1) * (dimension+2) / 2;
            NumParameters = dimension;
            NumValues = numFunc;
            // Compose lookup table of indices that enable quick querying of basis
            // function definitions:
            functionDefinitions = new int[numFunc][];
            quatraticTerms = new int[numFunc, numFunc];  // locations of quadratic terms
            int[] indices;
            int which = 0;
            functionDefinitions[which] = new int[0];
            // linear terms:
            for (int i = 0; i<dimension; ++i)
            {
                ++ which;
                indices = new int[1];
                indices[0] = i;
                functionDefinitions[which] = indices;
            }
            // pure quadratic terms:
            for (int i = 0; i<dimension; ++i)
            {
                ++ which;
                indices = new int[2];
                indices[0] = i;
                indices[1] = i;
                functionDefinitions[which] = indices;
                quatraticTerms[i, i] = which;
            }
            // mixed terms:
            for (int i = 0; i<dimension-1; ++i)
                for (int j=i+1; j<dimension; ++j)
                {
                   ++ which;
                    indices = new int[2];
                    indices[0] = i;
                    indices[1] = j;
                    functionDefinitions[which] = indices;
                    quatraticTerms[i, j] = quatraticTerms[i, j] = which;
                }
            if (which!=numFunc-1)
                throw new InvalidOperationException("Quadratic basis functions: failed to initialize, wrong functions count.");
        }

        /// <summary>Definitions of basis functions. Each definition is an array of at most 2 indices
        /// specifying the product of which variables (zero-base) is a specific function.
        /// At index 0, length of array is 0. At indices 1 through n+1 it is 1 (linear terms),
        /// and at remaining indices it is 2.</summary>
        protected int[][] functionDefinitions;

        /// <summary>2D table of indices specifying for each quadratic term which function it 
        /// corresponds to (since up to quadratic terms this is trivial).</summary>
        protected int[,] quatraticTerms;

        /// <summary>Returns a table of indices defining the specified function of the quadratic basis.
        /// Table contains up to two indices of variables whose product represents the specific basis
        /// function. [] (empty array) means constant term, [1] means x1, [2] means x2,
        /// [1,1] means x1^2, [2,2] means x2^2, [1,2] means x1*x2, [3,5] means x4*x5, etc.</summary>
        /// <param name="which"></param>
        /// <returns></returns>
        public int[] GetFunctionDefinition(int which)
        {
            if (which < 0 || which >= NumValues)
                throw new IndexOutOfRangeException("Quadratic basis: function index ("
                    + which + ") out of range, should be between 0 and " + (NumValues-1) + "." );
            return functionDefinitions[which];
        }

        /// <summary>Returns data about the specific function of this vector function containing
        /// quadratic basis.
        /// Examples: 
        ///     numTerms = 0: constant (1)
        ///     numTerms = 1: linear terms, firstVariableIndex is index of the variable.
        ///     numTerms = 2: quadratic terms, firstVariableIndex and secondVariableIndex are indices
        ///         of variables whose product gives this basis function.</summary>
        /// <param name="which">Specifies the function whose definition is queried.</param>
        /// <param name="numTerms">Returns number of terms in the product that defines the function
        /// (0 for constant, 1 for linear terms, 2 for quadratic terms)</param>
        /// <param name="firstVariableIndex">Returns index of the first variable in the product. If
        /// 0 then the specific function is constant.</param>
        /// <param name="secondVariableIndex">Returns index of the second variable in the product.
        /// If 0 then the specific function is linear or constant.</param>
        public void GetFunctionDefinition(int which, out int numTerms, out int firstVariableIndex,
                out int secondVariableIndex)
        {
            int[] def = functionDefinitions[which];
            int num = def.Length;
            firstVariableIndex = 0;
            secondVariableIndex = 0;
            numTerms = num;
            if (num >= 1)
                firstVariableIndex = def[0];
            if (num == 2)
                secondVariableIndex = def[1];
            if (num > 2)
                throw new IndexOutOfRangeException("Quadratic base functions: function " + which
                    + " has more than 2 product terms.");
        }

        
        /// <summary>Returns index of the constant function in this quadratic basis.</summary>
        int GetConstantTermIndex()
        {
            return 0;
        }

        /// <summary>Returns index of the specified linear function in this quadratic basis.</summary>
        /// <param name="varIndex">Specifies which linear term to return.</param>
        int GetLinearTermIndex(int varIndex)
        {
            return varIndex + 1;
        }

        /// <returns>Returns index of the specified quadratic function in this quadratic basis.
        /// Order of indices specifying the term is not important.</summary>
        /// <param name="varIndex1">Index of the first variable in the product.</param>
        /// <param name="varIndex2">Index of the second variable of the product.</param>
        int GetQuadraticTermIndex(int varIndex1, int varIndex2)
        {
            return quatraticTerms[varIndex1, varIndex2];
        }


        #region Evaluation

        #region ComponentWise

        /// <summary>Calculates and returns the particular component of the vector
        /// function value.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts. If the function does not support component-wise evaluation
        /// then results will be stored to this structure and returned from it. This makes reuse
        /// possible - when different components are evaluated subsequently with the same parameters,
        /// results are calculated only for the first time.</param>
        /// <param name="which">Specifies which function to evaluate.</param>
        public override double Value(IVectorFunctionResults evaluationData, int which)
        {
            int[] funcDef = GetFunctionDefinition(which);
            IVector param = evaluationData.Parameters;
            if (funcDef.Length == 0)
                return 1;
            else if (funcDef.Length == 1)
                return param[funcDef[0]];
            else
            {
                if (funcDef[0]==funcDef[1])
                    return 0.5*param[funcDef[0]] * param[funcDef[1]];  // pure quadratic terms have factor 1/2
                else 
                    return param[funcDef[0]] * param[funcDef[1]];
            }
        }

        /// <summary>Calculates and returns the particular component of the vector
        /// function's derivative.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts.
        /// Not used in this class, can be null.</param>
        /// <param name="which">Specifies which function to take.</param>
        /// <param name="component">Specifies which compoonent of the gradient should be returned.</param>
        public override double Derivative(IVectorFunctionResults evaluationData, int which,
            int component)
        {
            IVector param = evaluationData.Parameters;
            int[] funcDef = GetFunctionDefinition(which);
            if (funcDef.Length == 0)
                return 0;
            else if (funcDef.Length == 1) // linear term
            {
                if (funcDef[0] == component)
                    return 1;
                else
                    return 0;
            } else
            {
                if (funcDef[0]==funcDef[1])  // pure quadratic term
                {
                    if (component == funcDef[0])
                        return param[component];  // pure quadratic terms have factor 1/2
                    else
                        return 0;
                } else  // mixed quadratic term
                {
                    if (component == funcDef[0])
                        return param[funcDef[1]];
                    else if (component == funcDef[1])
                        return param[funcDef[0]];
                    else
                        return 0;
                }
            }
        }

        /// <summary>Calculates and returns the particular component of the vector
        /// function's second derivative (Hessian).</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts. 
        /// Not used in this class, can be null.</param>
        /// <param name="which">Specifies which function to take.</param>
        /// <param name="rowNum">Specifies which row of the Hessian (matrix of second derivatives) should 
        /// be returned.</param>
        /// <param name="columnNum">Specifies which column of the Hessian (matrix of second derivatives) should 
        /// be returned.</param>
        public override double SecondDerivative(IVectorFunctionResults evaluationData, int which,
            int rowNum, int columnNum)
        {
            IVector param = evaluationData.Parameters;
            int[] funcDef = GetFunctionDefinition(which);
            if (funcDef.Length < 2)
                return 0;
            else
            {
                if (rowNum==funcDef[0] && columnNum == funcDef[1]
                    || rowNum==funcDef[1] && columnNum ==  funcDef[0])
                {
                    return 1;  // pure quadratic terms have factor 1/2
                } else
                    return 0;
            }
        }

        #endregion ComponentWise


        #endregion Evaluation


        /// <summary>Example for this class, just runs examples form the LinearBasis class.</summary>
        public static void Example()
        {
            LinearBasis.Example();
        }


    }  // class QuadraticBasis



    /// <summary>The same as QuadraticBasis, just that it has more meaningful exceptions.</summary>
    /// $A Igor xx Apr10;
    public class QuadraticBasisSafer : QuadraticBasis
    {

        public QuadraticBasisSafer(int dimension): base(dimension)
        {
        }

        #region Evaluation

        #region ComponentWise

        /// <summary>Calculates and returns the particular component of the vector
        /// function value.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts. If the function does not support component-wise evaluation
        /// then results will be stored to this structure and returned from it. This makes reuse
        /// possible - when different components are evaluated subsequently with the same parameters,
        /// results are calculated only for the first time.</param>
        /// <param name="which">Specifies which function to evaluate.</param>
        public override double Value(IVectorFunctionResults evaluationData, int which)
        {
            if (evaluationData == null)
                throw new ArgumentException("Linear basis component: Evaluation data holding parameters is not specified.");
            if (evaluationData.Parameters == null)
                throw new ArgumentException("Linear basis component: Evaluation data does not contain parameters.");
            if (which < 0 || which >= NumValues)
                throw new ArgumentException("Linear basis component: function index out of range."
                    + Environment.NewLine + "  Index: " + which + ", should be between 0 and "
                    + (NumValues - 1) + "in " + NumParameters + "dimensions.");

            int[] funcDef = GetFunctionDefinition(which);
            IVector param = evaluationData.Parameters;
            if (funcDef.Length == 0)
                return 1;
            else if (funcDef.Length == 1)
                return param[funcDef[0]];
            else
            {
                if (funcDef[0] == funcDef[1])
                    return 0.5 * param[funcDef[0]] * param[funcDef[1]];  // pure quadratic terms have factor 1/2
                else
                    return param[funcDef[0]] * param[funcDef[1]];
            }
        }

        /// <summary>Calculates and returns the particular component of the vector
        /// function's derivative.</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts.
        /// Not used in this class, can be null.</param>
        /// <param name="which">Specifies which function to take.</param>
        /// <param name="component">Specifies which compoonent of the gradient should be returned.</param>
        public override double Derivative(IVectorFunctionResults evaluationData, int which,
            int component)
        {
            if (evaluationData == null)
                throw new ArgumentException("Linear basis component: Evaluation data holding parameters is not specified.");
            if (evaluationData.Parameters == null)
                throw new ArgumentException("Linear basis component: Evaluation data does not contain parameters.");
            if (which < 0 || which >= NumValues)
                throw new ArgumentException("Linear basis derivative component: function index out of range."
                    + Environment.NewLine + "  Index: " + which + ", should be between 0 and "
                    + (NumValues - 1) + "in " + NumParameters + "dimensions.");
            if (component < 0 || component >= NumParameters)
                throw new ArgumentException("Linear basis derivative component: variable index out of range."
                    + Environment.NewLine + "  Index: " + component + ", should be between 0 and "
                    + (NumParameters - 1) + " in " + NumParameters + " dimensions.");
            IVector param = evaluationData.Parameters;
            int[] funcDef = GetFunctionDefinition(which);
            if (funcDef.Length == 0)
                return 0;
            else if (funcDef.Length == 1) // linear term
            {
                if (funcDef[0] == component)
                {
                    Console.WriteLine("Derivative of function " + which + ", component " 
                        + component + " = 1.");
                    return 1;
                } else
                    return 0;
            }
            else
            {
                if (funcDef[0] == funcDef[1])  // pure quadratic term
                {
                    if (component == funcDef[0])
                        return param[component];  // pure quadratic terms have factor 1/2
                    else
                        return 0;
                }
                else  // mixed quadratic term
                {
                    if (component == funcDef[0])
                        return param[funcDef[1]];
                    else if (component == funcDef[1])
                        return param[funcDef[0]];
                    else
                        return 0;
                }
            }
        }

        /// <summary>Calculates and returns the particular component of the vector
        /// function's second derivative (Hessian).</summary>
        /// <param name="evaluationData">Evaluation data that contains function parameters and
        /// can store function resuts. 
        /// Not used in this class, can be null.</param>
        /// <param name="which">Specifies which function to take.</param>
        /// <param name="rowNum">Specifies which row of the Hessian (matrix of second derivatives) should 
        /// be returned.</param>
        /// <param name="columnNum">Specifies which column of the Hessian (matrix of second derivatives) should 
        /// be returned.</param>
        public override double SecondDerivative(IVectorFunctionResults evaluationData, int which,
            int rowNum, int columnNum)
        {
            if (evaluationData == null)
                throw new ArgumentException("Linear basis component: Evaluation data holding parameters is not specified.");
            if (evaluationData.Parameters == null)
                throw new ArgumentException("Linear basis component: Evaluation data does not contain parameters.");
            if (which < 0 || which >= NumValues)
                throw new ArgumentException("Linear basis second derivative component: function index out of range."
                    + Environment.NewLine + "  Index: " + which + ", should be between 0 and "
                    + (NumValues - 1) + "in " + NumParameters + "dimensions.");
            if (rowNum < 0 || rowNum >= NumParameters || columnNum<0 || columnNum>=NumParameters)
                throw new ArgumentException("Linear basis second derivative component: variable indices out of range."
                    + Environment.NewLine + "  Indices: [" + rowNum + ", " + columnNum
                    + "], should be between 0 and " + (NumParameters - 1) + " in " + NumParameters
                    + " dimensions.");
            IVector param = evaluationData.Parameters;
            int[] funcDef = GetFunctionDefinition(which);
            if (funcDef.Length < 2)
                return 0;
            else
            {
                if (rowNum == funcDef[0] && columnNum == funcDef[1]
                    || rowNum == funcDef[1] && columnNum == funcDef[0])
                {
                    return 1;  // pure quadratic terms have factor 1/2
                }
                else
                    return 0;
            }
        }

        #endregion ComponentWise


        #endregion Evaluation


        /// <summary>Example for this class, just runs examples form the LinearBasis class.</summary>
        public static new void Example()
        {
            LinearBasis.Example();
        }


    }  // class QuadraticBasisSafer


}
