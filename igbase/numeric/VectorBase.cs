// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

    // VECTORS; This module is based on teh MathNet external library.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using IG.Lib;

using Vector_MathNetNumerics = MathNet.Numerics.LinearAlgebra.Double.DenseVector;
using VectorBase_MathNetNumerics = MathNet.Numerics.LinearAlgebra.Generic.Vector<double>;

namespace IG.Num
{


    /// <summary>Generic Vector interface</summary>
    /// $A Igor Sep08;
    public interface IVector<T>
    {

        /// <summary>Gets the number of rows.</summary>
        int Length { get; }

        /// <summary>Gets or set the element indexed by <c>i</c> in the <c>Vector</c>.</summary>
        /// <param name="i">Element index (zero-based by agreement).</param>
        T this[int i] { get; set; }

        /// <summary>Copy all elements of this vector to an array.</summary>
        T[] ToArray();


        #region Operations

        #region Norms

        /// <summary>Gets Euclidean norm of the vector.</summary>
        double Norm
        {
            get;
        }

        /// <summary>Gets Euclidean norm of the vector.</summary>
        double Norm2
        {
            get;
        }

        /// <summary>Gets Euclidean norm of the vector.</summary>
        double NormEuclidean
        {
            get;
        }

        /// <summary>Squared Euclidean norm, sum of squared components.</summary>
        double SquaredNorm
        {
            get;
        }


        /// <summary>1-norm (Manhattan norm or Taxicab norm), sum of absolute values of components.</summary>
        double Norm1
        {
            get;
        }


        /// <summary>p-norm, p-th root of sum of absolute values of components raised to the power of p.</summary>
        double NormP(int p);


        /// <summary>Infinity-norm, maximum absolute value of any component.</summary>
        double NormInf
        {
            get;
        }

        #endregion Norms

        /// <summary>Normalizes the current vector.</summary>
        /// <returns></returns>
        void Normalize();


        /// <summary>Changes the sign of the current vector.</summary>
        /// <returns></returns>
        void Negate();


        #endregion Operations



    }  // interface IVector<T>



    /// <summary>Real vector intrface.</summary>
    /// $A Igor Sep08;
    public interface IVector : IVector<double>
    {

        ///// <summary>Returns a deep copy of the current object.</summary>
        //VectorBase GetCopyBase();

        ///// <summary>Creates and returns a new vector with the specified dimension, and of the same type as 
        ///// the current vector.</summary>
        ///// <param name="length">Dimension of the newly created vector.</param>
        ///// <returns>A newly created vector of the specified dimension and of the same type as the current vector.</returns>
        //VectorBase GetNewBase(int length);

        ///// <summary>Creates and returns a new vector with the same dimension and of the same type as 
        ///// the current vector.</summary>
        //VectorBase GetNewBase();

        ///// <summary>Creates and returns a new matrix with the specified dimensona, and of a type that is 
        ///// consistent with the type of the current vector.</summary>
        ///// <param name="rowCount">Number of rows of the returned matrix.</param>
        ///// <param name="columnCount">Number of rows of the returned matrix.</param>
        ///// <returns>A matrix with specified dimensions, whose type is consistent with the type of the current vector.</returns>
        //MatrixBase GetNewMatrixBase(int rowCount, int columnCount);
        
        ///// <summary>Creates and returns a new matrix of a type that is consistent with the type of the current
        ///// vector, and with both dimensions equal to the dimension of the current vector.</summary>
        //MatrixBase GetNewMatrixBase();


        /// <summary>Returns a deep copy of the current object.</summary>
        IVector GetCopy();

        /// <summary>Creates and returns a new vector with the specified dimension, and of the same type as 
        /// the current vector.</summary>
        /// <param name="length">Dimension of the newly created vector.</param>
        /// <returns>A newly created vector of the specified dimension and of the same type as the current vector.</returns>
        IVector GetNew(int length);

        /// <summary>Creates and returns a new vector with the same dimension and of the same type as 
        /// the current vector.</summary>
        IVector GetNew();

        /// <summary>Creates and returns a new matrix with the specified dimensona, and of a type that is 
        /// consistent with the type of the current vector.</summary>
        /// <param name="rowCount">Number of rows of the returned matrix.</param>
        /// <param name="columnCount">Number of rows of the returned matrix.</param>
        /// <returns>A matrix with specified dimensions, whose type is consistent with the type of the current vector.</returns>
        IMatrix GetNewMatrix(int rowCount, int columnCount);
        
        /// <summary>Creates and returns a new matrix of a type that is consistent with the type of the current
        /// vector, and with both dimensions equal to the dimension of the current vector.</summary>
        IMatrix GetNewMatrix();
        
        /// <summary>Sets all components of the current vector to the specified value.</summary>
        /// <param name="elementValue">Value to which vector elements are set.</param>
        void SetConstant(double elementValue);

        /// <summary>Sets the current vector to the specific unit vector (one component equals 1, others are 0).</summary>
        /// <param name="which">Specifies which unit vector is set (i.e., which component equals 1).</param>
        void SetUnit(int which);

        /// <summary>Sets the current vector such that it contains random elements on the interval (0,1].</summary>
        void SetRandom();

        /// <summary>Sets the current vector such that it contains random elements on the interval (0,1].</summary>
        /// <param name="rnd">Random generator used to generate vector elements.</param>
        void SetRandom(IRandomGenerator rnd);

        #region Operations

        /// <summary>Sets all components of the current vector to 0.</summary>
        void SetZero();

        /// <summary>Returns a vector that equals a normalized current vector.</summary>
        IVector Normalized();


        #endregion Operations

        /// <summary>Returns a string representation of this vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).</summary>
        string ToString();
        
        /// <summary>Returns a string representation of this vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).</summary>
        string ToStringMath();

        /// <summary>Returns a string representation of the current vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the vector.</summary>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        string ToString(string elementFormat);
        
        /// <summary>Returns a string representation of the current vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the vector.</summary>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        string ToStringMath(string elementFormat);
        
        /// <summary>Returns an integer valued hash function of the current vector object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionInt"/> method.</para></summary>
        /// <seealso cref="Util.GetHashFunctionInt"/>
        int GetHashFunctionInt();

        /// <summary>Returns a string valued hash function of the current vector object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionString"/> method.</para></summary>
        /// <remarks>The returned string is always on the same length, and is based on the <see cref="ToString"/> method.
        /// Therefore it is convenient for use in file or directory names that have one part related to a specific vector.</remarks>
        /// <seealso cref="Util.GetHashFunctionString"/>
        string GetHashFunctionString();

    }  // interface IVector


    /// <summary>Extension methods for vector classes.</summary>
    /// $A Igor Apr11; 
    public static class VectorExtensions
    {
        
        /// <summary>Returns a string representation of the current vector in a standard IGLib form.
        /// Extension method for IVector interface.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        public static string ToString(this IVector vec)
        {
            return VectorBase.ToString(vec);
        }
        
        /// <summary>Returns a string representation of the current vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Extension method for IVector interface.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        public static string ToStringMath(IVector vec)
        {
            return VectorBase.ToStringMath(vec);
        }

        /// <summary>Returns a string representation of the current vector in a standard IGLib form, with the specified  
        /// format for elements of the vector.
        /// Extension method for IVector interface.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public static string ToString(this IVector vec, string elementFormat)
        {
            return VectorBase.ToString(vec, elementFormat);
        }
        
        /// <summary>Returns a string representation of the current vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the vector.
        /// Extension method for IVector interface.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public static string ToStringMath(IVector vec, string elementFormat)
        {
            return VectorBase.ToStringMath(vec, elementFormat);
        }


    }  // class VectorExtensions


    /// <summary>Base class for real vectors.</summary>
    /// $A Igor Jan08 Jul10 Nov10;
    [Serializable]
    public abstract class VectorBase : IVector
    {

        #region Data

        /// <summary>Gets dimension of the vector.</summary>
        public abstract int Length
        { get; }


        /// <summary>Gets or sets the element indexed by <c>i</c> in the <c>Vector</c>.</summary>
        /// <param name="i">Element index, 0 - based.</param>
        public abstract double this[int i]
        { get; set; }


        /// <summary>Copies the current vector to an array. </summary>
        public virtual double[] ToArray()
        {
            double[] ret = new double[Length];
            int i;
            for (i = 0; i < Length; ++i)
                ret[i] = this[i];
            return ret;
        }


        /// <summary>Creates and returns a copy of the current vector.
        /// WARNING: This method should be overridden in the derived classes in such a way that the returned
        /// vector is of the same type as the current vector.</summary>
        public abstract VectorBase GetCopyBase();

        /// <summary>Creates and returns a new vector of the specified dimension in such a way that the type
        /// of the returned vector is the same as of the current vector.
        /// WARNING: This method should be overridden in all derived classes such that the returned vector type
        /// is the same as the type of the current vector.</summary>
        /// <param name="length">Dimension of the returned vector.</param>
        public abstract VectorBase GetNewBase(int length);

        /// <summary>Creates and returns a new vector with the same dimension and of the same type as 
        /// the current vector.</summary>
        public abstract VectorBase GetNewBase();

        /// <summary>Creates and returns a new matrix with the specified dimensona, and of a type that is 
        /// consistent with the type of the current vector.</summary>
        /// <param name="rowCount">Number of rows of the returned matrix.</param>
        /// <param name="columnCount">Number of rows of the returned matrix.</param>
        /// <returns>A matrix with specified dimensions, whose type is consistent with the type of the current vector.</returns>
        public abstract MatrixBase GetNewMatrixBase(int rowCount, int columnCount);


        /// <summary>Creates and returns a new matrix of a type that is consistent with the type of the current
        /// vector, and with both dimensions equal to the dimension of the current vector.</summary>
        public abstract MatrixBase GetNewMatrixBase();


        /// <summary>Returns a deep copy of the current object.</summary>
        public virtual IVector GetCopy()
        { return GetCopyBase(); }

        /// <summary>Creates and returns a new vector with the specified dimension, and of the same type as 
        /// the current vector.</summary>
        /// <param name="length">Dimension of the newly created vector.</param>
        /// <returns>A newly created vector of the specified dimension and of the same type as the current vector.</returns>
        public virtual IVector GetNew(int length)
        { return GetNewBase(length); }

        /// <summary>Creates and returns a new vector with the same dimension and of the same type as 
        /// the current vector.</summary>
        public virtual IVector GetNew()
        { return GetNewBase(); }

        /// <summary>Creates and returns a new matrix with the specified dimensona, and of a type that is 
        /// consistent with the type of the current vector.</summary>
        /// <param name="rowCount">Number of rows of the returned matrix.</param>
        /// <param name="columnCount">Number of rows of the returned matrix.</param>
        /// <returns>A matrix with specified dimensions, whose type is consistent with the type of the current vector.</returns>
        public virtual IMatrix GetNewMatrix(int rowCount, int columnCount)
        { return GetNewMatrixBase(rowCount, columnCount); }


        /// <summary>Creates and returns a new matrix of a type that is consistent with the type of the current
        /// vector, and with both dimensions equal to the dimension of the current vector.</summary>
        public virtual IMatrix GetNewMatrix()
        { return GetNewMatrixBase(); }


        #endregion Data


        #region Operations

        #region SetValue

        /// <summary>Sets all components of the current vector to 0.</summary>
        public virtual void SetZero()
        {
            for (int i = 0; i < this.Length; ++i)
                this[i] = 0.0;
        }


        /// <summary>Sets all components of the current vector to the specified value.</summary>
        /// <param name="elementValue">Value to which vector elements are set.</param>
        public virtual void SetConstant(double elementValue)
        {
            for (int i = 0; i < Length; ++i)
                this[i] = elementValue;
        }

        /// <summary>Sets the current vector to the specific unit vector (one component equals 1, others are 0).</summary>
        /// <param name="which">Specifies which unit vector is set (i.e., which component equals 1).</param>
        public virtual void SetUnit(int which)
        {
            for (int i = 0; i < Length; ++i)
                this[i] = 0.0;
            this[which] = 1.0;
        }

        /// <summary>Sets the current vector such that it contains random elements on the interval (0,1].</summary>
        public virtual void SetRandom()
        {
            SetRandom(this);
        }

        /// <summary>Sets the current vector such that it contains random elements on the interval (0,1].</summary>
        /// <param name="rnd">Random generator used to generate vector elements.</param>
        public virtual void SetRandom(IRandomGenerator rnd)
        {
            SetRandom(this, rnd);
         }

        #endregion SetValue

        #region Norms

        /// <summary>Gets Euclidean norm of the vector.</summary>
        public virtual double Norm
        {
            get 
            {
                double ret = 0.0;
                for (int i = 0; i < Length; ++i)
                    ret += this[i] * this[i];
                return Math.Sqrt(ret);
            }
        }

        /// <summary>Gets Euclidean norm of the vector.</summary>
        public virtual double Norm2
        {
            get 
            {
                return Norm;
            }
        }

        /// <summary>Gets Euclidean norm of the vector.</summary>
        public virtual double NormEuclidean
        {
            get 
            {
                return Norm;
            }
        }

        /// <summary>Squared Euclidean norm, sum of squared components.</summary>
        public virtual double SquaredNorm
        {
            get
            {
                double ret = 0.0;
                for (int i = 0; i < Length; ++i)
                    ret += this[i] * this[i];
                return ret;
            }
        }


        /// <summary>1-norm (Manhattan norm or Taxicab norm), sum of absolute values of components.</summary>
        public virtual double Norm1
        {
            get
            {
                double ret = 0.0;
                for (int i = 0; i < Length; ++i)
                    ret += Math.Abs(this[i]);
                return ret;
            }
        }


        /// <summary>p-norm, p-th root of sum of absolute values of components raised to the power of p.</summary>
        public virtual double NormP(int p)
        {
            double ret = 0.0;
            for (int i = 0; i < Length; ++i)
                ret += Math.Pow(Math.Abs(this[i]), p);
            return Math.Pow(ret, 1/(double) p);
        }


        /// <summary>Infinity-norm, maximum absolute value of any component.</summary>
        public virtual double NormInf
        {
            get
            {
                double ret = 0.0, a;
                for (int i = 0; i < Length; ++i)
                {
                    a = Math.Abs(this[i]);
                    if (a > ret)
                        ret = a;
        }
                return ret;
            }
            
        }


        /// <summary>Returns Euqlidean distance between the specified two vectors.
        /// <para>WARNING: This is a plain version of the method that does not perform any consistency checks.</para></summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        public static double DistancePlain(IVector a, IVector b)
        {
            double ret = 0;
            int dim = a.Length;
            for (int i = 0; i < dim; ++i)
            {
                double d = a[i] - b[i];
                ret += d * d;
            }
            return Math.Sqrt(ret);
        }

        /// <summary>Returns Euqlidean distance between the specified two vectors.
        /// <para>WARNING: Dimensions must match, otherwise exception is thrown..</para></summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        public static double Distance(IVector a, IVector b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Euclidean distance of two vectors: operand not specified (null reference).");
            else if (a.Length != b.Length)
                throw new ArgumentException("Dimension mismatch at vector distance calculation. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length + ".");
            double ret = 0;
            int dim = a.Length;
            for (int i = 0; i < dim; ++i)
            {
                double d = a[i] - b[i];
                ret += d * d;
            }
            return Math.Sqrt(ret);
        }


        /// <summary>Returns distance between the specified two vectors where vector elements are weighted by
        /// elements of another vector, which represent intervals of equal importance (thus the weighted sum is
        /// divided by their squares).
        /// <para>Dw=Sqrt(Sum(((a_i-b_i)/l_i)^2))</para>
        /// <para>WARNING: This is a plain version of the method that does not perform any consistency checks. It 
        /// assumes that all vector prarameters are allocated and of hte same dimension, and that all elements 
        /// of characteristic lengths vector are non-zero.</para></summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="characteristicLengths">Vector whose elements represent interval lenhth of equal importance in different coordinate direction. 
        /// Used for calculation of weighted sum (terms are divided by squares of vector elements).</param>
        public static double DistanceWeightedPlain(IVector a, IVector b, IVector characteristicLengths)
        {
            double ret = 0;
            int dim = a.Length;
            for (int i = 0; i < dim; ++i)
            {
                double l = characteristicLengths[i];
                if (l == 0)
                    throw new ArgumentException("Invalid vector of characteristic lengths in weighted distance calculation, element " + i + " equals zero.");
                double d = (a[i] - b[i]) / l;
                ret += d * d;
            }
            return Math.Sqrt(ret);
        }

        /// <summary>Returns distance between the specified two vectors where vector elements are weighted by
        /// elements of another vector, which represent intervals of equal importance (thus the weighted sum is
        /// divided by their squares).
        /// <para>Dw=Sqrt(Sum(((a_i-b_i)/l_i)^2))</para>
        /// <para>WARNING: Dimensions of vector arguments must match, otherwise exceptin is thrown. Also if any element
        /// of characteristic lengths vector is zero, an exception is thrown, too.</para></summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="characteristicLengths">Vector whose elements represent interval lenhth of equal importance in different coordinate direction. 
        /// Used for calculation of weighted sum (terms are divided by squares of vector elements).</param>
        public static double DistanceWeighted(IVector a, IVector b, IVector characteristicLengths)
        {
            if (a == null || b == null || characteristicLengths==null)
                throw new ArgumentNullException("Euclidean distance of two vectors: operand not specified (null reference).");
            else if (a.Length != b.Length || a.Length!=characteristicLengths.Length)
                throw new ArgumentException("Dimension mismatch at vector distance calculation. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length 
                    + ",  intervals vector dim.: " + characteristicLengths.Length + ".");
            double ret = 0;
            int dim = a.Length;
            if (characteristicLengths == null)
            {
                for (int i = 0; i < dim; ++i)
                {
                    double d = (a[i] - b[i]);
                    ret += d * d;
                }
            } else
            {
                for (int i = 0; i < dim; ++i)
                {
                    double l = characteristicLengths[i];
                    if (l == 0)
                        throw new ArgumentException("Invalid vector of characteristic lengths in weighted distance calculation, element " + i + " equals zero.");
                    double d = (a[i] - b[i]) / l;
                    ret += d * d;
                }
            }
            return Math.Sqrt(ret);
        }

        /// <summary>Returns weighted Euclidean norm of the specified vector.</summary>
        /// <param name="a">Vector whose weighter Euclidean norm is returned.</param>
        /// <param name="characteristicLengths">Vector whose elements represent interval lenhth of equal importance in different coordinate direction. 
        /// Used for calculation of weighted sum (terms are divided by squares of vector elements).</param>
        public static double NormWeightedPlain(IVector a, IVector characteristicLengths)
        {
            double ret = 0;
            int dim = a.Length;
            for (int i = 0; i < dim; ++i)
            {
                double l = characteristicLengths[i];
                if (l == 0)
                    throw new ArgumentException("Invalid vector of characteristic lengths in weighted norm calculation, element " + i + " equals zero.");
                double d = (a[i]) / l;
                ret += d * d;
            }
            return Math.Sqrt(ret);
        }
        
        /// <summary>Returns weighted Euclidean norm of the specified vector.</summary>
        /// <param name="a">Vector whose weighter Euclidean norm is returned.</param>
        /// <param name="characteristicLengths">Vector whose elements represent interval lengths of equal importance in different coordinate direction. 
        /// Used for calculation of weighted sum (terms are divided by squares of vector elements).</param>
        public static double NormWeighted(IVector a, IVector characteristicLengths)
        {
            if (a == null)
                throw new ArgumentException("Vector whose norm should be calculated is not specified (null reference).");
            else if (characteristicLengths == null)
                throw new ArgumentException("Vector of characteristic lengths is not specified (null reference).");
            int dim = a.Length;
            if (characteristicLengths.Length != a.Length)
                throw new ArgumentException("Dimension of vector of characteristic lengths " + characteristicLengths.Length
                    + " is different than dimension of the vector (" + dim + ").");
            double ret = 0;
            for (int i = 0; i < dim; ++i)
            {
                double l = characteristicLengths[i];
                if (l == 0)
                    throw new ArgumentException("Invalid vector of characteristic lengths in weighted norm calculation, element " + i + " equals zero.");
                double d = (a[i]) / l;
                ret += d * d;
            }
            return Math.Sqrt(ret);
        }

        #endregion Norms


        /// <summary>Returns a vector that equals a normalized current vector.</summary>
        public IVector Normalized()
        {
            double norm = Norm;
            if (MathNet.Numerics.Precision.AlmostEqual(0.0, norm))
                return this.GetCopyBase();
            else
            {
                IVector ret = this.GetCopyBase();
                Divide(this, norm, ret);
                return ret;
            }
        }

        /// <summary>Normalizes the current vector.</summary>
        /// <returns></returns>
        public void Normalize()
        {
            double norm = Norm;
            if (!MathNet.Numerics.Precision.AlmostEqual(0.0, norm))
                Divide(this, norm, this);
        }


        /// <summary>Changes the sign of the current vector.</summary>
        /// <returns></returns>
        public void Negate()
        {
            Negate(this, this);
        }

        #region Operations.Auxiliary

        /// <summary>Returns the hash code (hash function) of the current vector.</summary>
        /// <remarks>
        /// <para>This method calls the <see cref="VectorBase.GetHashCode"/> to calculate the 
        /// hash code, which is standard for all implementations of the <see cref="IVector"/> interface.</para>
        /// <para>Two vectors that have the same dimensions and equal elements will produce the same hash codes.</para>
        /// <para>Probability that two different vectors will produce the same hash code is small but it exists.</para>
        /// <para>Overrides the <see cref="object.GetHashCode"/> method.</para>
        /// </remarks>
        public override int GetHashCode()
        {
            return VectorBase.GetHashCode(this);
        }

        /// <summary>Returns a value indicating whether the specified object is equal to the current vector.
        /// <para>True is returned if the object is a non-null vector (i.e. it implements the <see cref="IVector"/>
        /// interface), and has the same dimension and equal elements as the current vector.</para></summary>
        /// <remarks>This method calls the <see cref="VectorBase.Equals"/> to obtain the returned value, which is
        /// standard for all implementations of the <see cref="IVector"/> interface.
        /// <para>Overrides the <see cref="object.Equals"/> method.</para></remarks>
        public override bool Equals(Object obj)
        {
            return VectorBase.Equals(this, obj as IVector);
        }

        /// <summary>Returns an integer valued hash function of the current vector object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionInt"/> method.</para></summary>
        /// <seealso cref="Util.GetHashFunctionInt"/>
        public int GetHashFunctionInt()
        {
            return Util.GetHashFunctionInt(this);
        }

        /// <summary>Returns a string valued hash function of the current vector object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionString"/> method.</para></summary>
        /// <remarks>The returned string is always on the same length, and is based on the <see cref="ToString"/> method.
        /// Therefore it is convenient for use in file or directory names that have one part related to a specific vector.</remarks>
        /// <seealso cref="Util.GetHashFunctionString"/>
        public string GetHashFunctionString()
        {
            return Util.GetHashFunctionString(this);
        }
        
        #endregion Operations.Auxiliary


        #endregion Operations


        #region StaticOperations

        private static VectorStore _matrixStore;

        /// <summary>Gets the matrix store for recycling auxiliary matrices.
        /// <para>Matrix store is created only once, on the first access.</para></summary>
        public static VectorStore VectorStore
        {
            get
            {
                if (_matrixStore == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_matrixStore == null)
                            _matrixStore = new VectorStore(0, false);
                    }
                }
                return _matrixStore;
            }
        }


        #region SetValueStatic

        /// <summary>Sets all components of the current vector to 0.</summary>
        /// <param name="vec">Vector whose components are set.</param>
        public static void SetZero(IVector vec)
        {
            if (vec == null)
                throw new ArgumentNullException("Vector to be set to zero is not specified (null argument).");
            for (int i = 0; i < vec.Length; ++i)
                vec[i] = 0.0;
        }


        /// <summary>Sets all components of the current vector to the specified value.</summary>
        /// <param name="vec">Vector whose components are set.</param>
        /// <param name="elementValue">Value to which vector elements are set.</param>
        public static void SetConstant(IVector vec, double elementValue)
        {
            if (vec == null)
                throw new ArgumentNullException("Vector to be set to zero is not specified (null argument).");
            for (int i = 0; i < vec.Length; ++i)
                vec[i] = elementValue;
        }

        /// <summary>Sets the current vector to the specific unit vector (one component equals 1, others are 0).</summary>
        /// <param name="vec">Vector whose components are set.</param>
        /// <param name="which">Specifies which unit vector is set (i.e., which component equals 1).</param>
        public static void SetUnit(IVector vec, int which)
        {
            if (vec == null)
                throw new ArgumentNullException("Vector to be set to zero is not specified (null argument).");
            for (int i = 0; i < vec.Length; ++i)
                vec[i] = 0.0;
            vec[which] = 1;
        }

        /// <summary>Sets the current vector such that it contains random elements on the interval (0,1].</summary>
        /// <param name="vec">Vector whose components are set.</param>
        public static void SetRandom(IVector vec)
        {
            if (vec == null)
                throw new ArgumentNullException("Vector to be set to zero is not specified (null argument).");
            IRandomGenerator rnd = new RandGeneratorThreadSafe(1111);
            for (int i = 0; i < vec.Length; ++i)
            {
                double element;
                do
                {
                    element = vec[i] = rnd.NextDouble();
                } while (element == 0.0);
            }
        }

        /// <summary>Sets the current vector such that it contains random elements on the interval (0,1].</summary>
        /// <param name="vec">Vector whose components are set.</param>
        /// <param name="rnd">Random generator used to generate vector elements.</param>
        public static void SetRandom(IVector vec, IRandomGenerator rnd)
        {
            if (vec == null)
                throw new ArgumentNullException("Vector to be set to zero is not specified (null argument).");
            if (rnd == null)
                throw new ArgumentNullException("Random number generator for generation of components is not specified (null reference).");
            for (int i = 0; i < vec.Length; ++i)
            {
                double element;
                do
                {
                    element = vec[i] = rnd.NextDouble();
                } while (element == 0.0);
            }
        }

        #endregion SetValueStatic
 

        /// <summary>Compares two vectors and returns -1 if the first vector is smaller than the second one,
        /// 0 if vectors are equal, and 1 if the first vector is greater.
        /// Vector that is null is considered smaller than a vector that is not null. Two null vectors are considered equal.
        /// Vector with smaller dimension is considered smaller than a vector with greater dimension.
        /// Vectors with equal dimensions ar compared by elements. The first element that is different decides
        /// which vector is considered greater.</summary>
        /// <param name="v1">First vector to be compared.</param>
        /// <param name="v2">Second vector to be compared.</param>
        /// <returns>-1 if the first vector is smaller than the second one, 0 if vectors are equal, and 1 if the second is greater.</returns>
        /// <remarks>This comparison does not have any mathematical meaning. It is just used for sotting of vectors in data structures.</remarks>
        public static int Compare(IVector v1, IVector v2)
        {
            if (v1 == null)
            {
                if (v2 == null)
                    return 0;
                else
                    return -1;
            } else if (v2 == null)
            {
                return 1;
            } 
            else if (v1.Length < v2.Length)
                return -1;
            else if (v1.Length > v2.Length)
                return 1;
            else
            {
                int dim = v1.Length;
                for (int i = 0; i < dim; ++i)
                {
                    double el1 = v1[i];
                    double el2 = v2[i];
                    if (el1 < el2)
                        return -1;
                    else if (el1 > el2)
                        return 1;
                }
                return 0;  // all elements are equal
            }
        }


        /// <summary>Resizes, if necessary, the specified vector according to the required dimension.
        /// If the vector is initially null then a new vector is created. If in this case a template vector is specified
        /// then the newly created vector will be of the same type as that template vector, because it is created by 
        /// the GetNew() method on that vector.
        /// If dimension of the initial vector does not match the required dim., then vector is resized. 
        /// If the specified vector dimension is less or equal to 0 then vector is resized with the same dimension as 
        /// that of the template vector. If in this case the template vector is null, an exception is thrown.
        /// WARNINGS:
        /// Components are NOT preserved and have in general undefined values after operation is performed.
        /// If vector and template are null then the type of nawly created vector is Vector.</summary>
        /// <param name="vec">Vector that is resized.</param>
        /// <param name = "template">Vector that is taken as template (for type of a newly created vector or for dimension if
        /// it is not specified).</param>
        /// <param name="dimenson">If greater than 0 then it specifies the dimension to which vector is resized.</param>
        public static void Resize(ref IVector vec, IVector template, int dimension)
        {
            if (dimension <= 0)
            {
                if (template == null)
                    throw new ArgumentNullException("Vector dimension after resize is not specified (argument less or equal to 0) and template is not specified.");
                else
                    dimension = template.Length;
            }
            if (vec == null)
            {
                if (template != null)
                    vec = template.GetNew(dimension);
                else
                    vec = new Vector(dimension);
            }
            else if (vec.Length != dimension)
            {
                vec = vec.GetNew(dimension);
            }
        }

        /// <summary>Resizes, if necessary, the specified vector according to the required dimension.
        /// If the vector is initially null then a new vector is created.
        /// If dimension of the initial vector does not match the required dimension, then vector is resized. 
        /// Components are NOT preserved and have in general undefined values after operation is performed.
        /// WARNING: 
        /// If the vector is initially null then the type of the newly created vector is Vector.</summary>
        /// <param name="vec">Vector that is resized.</param>
        /// <param name="dimenson">Dimension to which vector is resized (if less or equal to 0 then exception is thrown).</param>
        public static void Resize(ref IVector vec, int dimension)
        {
            if (dimension <= 0)
                throw new ArgumentNullException("Vector dimension after resize is not specified (argument less or equal to 0).");
            if (vec == null)
                vec = new Vector(dimension);
            else if (vec.Length != dimension)
                vec = vec.GetNew(dimension);
        }


        /// <summary>Resizes, if necessary, the specified vector according to the dimension of the specified template vector.
        /// If the vector is initially null then a new vector is created. In this case the newly created vector will 
        /// be of the same type as that template vector, because it is created by  the GetNew() method on that vector.
        /// If dimension of the initial vector does not match the dimension of the template vector, then vector is resized. 
        /// If the template vector is null, then an exception is thrown.
        /// WARNINGS:
        /// Components are NOT preserved and have in general undefined values after operation is performed.
        /// If vector and template are null then the type of newly created vector is Vector.</summary>
        /// <param name="vec">Vector that is resized.</param>
        /// <param name = "template">Vector that is taken as template (for type of a newly created vector or for dimension).</param>
        public static void Resize(ref IVector vec, IVector template)
        {
            int dimension;
            if (template == null)
                throw new ArgumentNullException("Template vector is not specified in vector resize.");
            else
                dimension = template.Length;
            if (vec == null)
            {
                vec = template.GetNew(dimension);
            }
            else if (vec.Length != dimension)
            {
                vec = vec.GetNew(dimension);
            }
        }




        /// <summary>Copies components of a vector to another vector.
        /// This is a plain version that does not perform any dimension checks.
        /// WARNING: dimensions of the copied vector and result vector must match.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void CopyPlain(IVector a, IVector result)
        {
            int dim = a.Length;
            int i;
            for (i = 0; i < dim; ++i)
                result[i] = a[i];
        }

        /// <summary>Copies components of a vector to another vector.
        /// WARNING: dimensions of the copied vector and result vector must match.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void Copy(IVector a, IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector to be copied is not specified (null reference).");
            int dim = a.Length;
            if (result == null)
                throw new ArgumentNullException("Result vector for copy operation is not specified (null reference).");
            else if (dim != result.Length )
                throw new ArgumentException("Dimension mismatch while copying a vector." + Environment.NewLine
                    + "  Original vector dim.: " + dim 
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < dim; ++i)
                result[i] = a[i];
        }

        /// <summary>Copies components of a vectr to another vector.
        /// Resulting vector is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void Copy(IVector a, ref IVector result)
        {
            if (a == null)
                result = null;
            else
            {
                int dim = a.Length;
                if (result == null)
                    result = a.GetNew(dim);
                else if (dim != result.Length)
                    result = a.GetNew(dim);
                int i;
                for (i = 0; i < dim; ++i)
                    result[i] = a[i];
            }
        }


        /// <summary>Copies components of a vector to another vector.
        /// This is a plain version that does not perform any dimension checks.
        /// WARNING: dimensions of the copied vector and result vector must match.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void CopyPlain(VectorBase_MathNetNumerics a, IVector result)
        {
            int dim = a.Count;
            int i;
            for (i = 0; i < a.Count; ++i)
                result[i] = a[i];
        }

        /// <summary>Copies components of a vector to another vector.
        /// WARNING: dimensions of the copied vector and result vector must match.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void Copy(VectorBase_MathNetNumerics a, IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector to be copied is not specified (null reference).");
            int dim = a.Count;
            if (result == null)
                throw new ArgumentNullException("Result vector for copy operation is not specified (null reference).");
            else if (dim != result.Length)
                throw new ArgumentException("Dimension mismatch while copying a vector." + Environment.NewLine
                    + "  Original vector dim.: " + dim 
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < dim; ++i)
                result[i] = a[i];
        }

        /// <summary>Copies components of a vectr to another vector.
        /// Resulting vector is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void Copy(VectorBase_MathNetNumerics a, ref IVector result)
        {
            if (a == null)
                result = null;
            else
            {
                int dim = a.Count;
                if (result == null)
                    result = new Vector(dim);
                else if (dim != result.Length)
                    result = new Vector(dim);
                int i;
                for (i = 0; i < dim; ++i)
                    result[i] = a[i];
            }
        }

        /// <summary>Copies components of a vector to another vector.
        /// This is a plain version that does not perform any dimension checks.
        /// WARNING: dimensions of the copied vector and result vector must match.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void CopyPlain(IVector a, Vector_MathNetNumerics result)
        {
            int dim = a.Length;
            int i;
            for (i = 0; i < dim; ++i)
                result[i] = a[i];
        }

        /// <summary>Copies components of a vector to another vector.
        /// WARNING: dimensions of the copied vector and result vector must match.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void Copy(IVector a, Vector_MathNetNumerics result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector to be copied is not specified (null reference).");
            int dim = a.Length;
            if (result == null)
                throw new ArgumentNullException("Result vector for copy operation is not specified (null reference).");
            else if (dim != result.Count)
                throw new ArgumentException("Dimension mismatch while copying a vector." + Environment.NewLine
                    + "  Original vector dim.: " + dim
                    + ", result dim.: " + result.Count + ".");
            int i;
            for (i = 0; i < dim; ++i)
                result[i] = a[i];
        }

        /// <summary>Copies components of a vectr to another vector.
        /// Resulting vector is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void Copy(IVector a, ref Vector_MathNetNumerics result)
        {
            if (a == null)
                result = null;
            else
            {
                int dim = a.Length;
                if (result == null)
                    result = new Vector_MathNetNumerics(dim); // a.GetNew(dim);
                else if (dim != result.Count)
                    result = new Vector_MathNetNumerics(dim); // a.GetNew(dim);
                int i;
                for (i = 0; i < dim; ++i)
                    result[i] = a[i];
            }
        }

        /// <summary>Stores a negative vector of the specified vector to another vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the copied vector and result vector must match.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where result is be stored. Dimensions must match dimensions of original.</param>
        public static void NegatePlain(IVector a, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = - a[i];
        }

        /// <summary>Stores a negative vector of the specified vector to another vector.
        /// WARNING: dimensions of the copied vector and result vector must match.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where result is be stored. Dimensions must match dimensions of original.</param>
        public static void Negate(IVector a, IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector to be negated is not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result vector for negation is not specified (null reference).");
            else if (a.Length != result.Length)
                throw new ArgumentException("Dimension mismatch while negating a vector." + Environment.NewLine
                    + "  Original vector dim.: " + a.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = -a[i];
        }

        /// <summary>Stores a negative vector of the specified vector to another vector.
        /// Resulting vector is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original vector.</param>
        /// <param name="result">Vector where result is be stored.</param>
        public static void Negate(IVector a, ref IVector result)
        {
            if (a == null)
                result = null;
            else
            {
                if (result == null)
                    result = a.GetNew(a.Length);
                else if (a.Length != result.Length)
                    result = a.GetNew(a.Length);
                int i;
                for (i = 0; i < a.Length; ++i)
                    result[i] = - a[i];
            }
        }


        /// <summary>Sums two vectors and stores the result in the specified result vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of vectors must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void AddPlain(IVector a, IVector b, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] + b[i];
        }

        /// <summary>Sums two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of vectors must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void Add(IVector a, IVector b, IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector summation: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector summation is not specified (null reference).");
            else if (a.Length != b.Length || result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector summation. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] + b[i];
        }



        /// <summary>Sums two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void Add(IVector a, IVector b, ref IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector summation: operand not specified (null reference).");
            else if (a.Length != b.Length)
                throw new ArgumentException("Dimension mismatch at vector summation. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length 
                    + ",  second operand dim.: " + b.Length + ".");
            else if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            int i;
            for (i = 0; i < a.Length; ++i)
                    result[i] = a[i] + b[i];
        }


        /// <summary>Subtracts two vectors (<paramref name="a"/> - <paramref name="b"/>) and stores the result in the specified result vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of vectors must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void SubtractPlain(IVector a, IVector b, IVector result)
        {
                int i;
                for (i = 0; i < a.Length; ++i)
                    result[i] = a[i] - b[i];
        }

        /// <summary>Subtracts two vectors (<paramref name="a"/> - <paramref name="b"/>) and stores the result in the specified result vector.
        /// WARNING: dimensions of vectors must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void Subtract(IVector a, IVector b, IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector subtraction: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector subtraction is not specified (null reference).");
            else if (a.Length != b.Length || result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector subtraction. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] - b[i];
        }

        /// <summary>Subtracts two vectors (<paramref name="a"/> - <paramref name="b"/>) and stores the result in the specified result vector.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void Subtract(IVector a, IVector b, ref IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector subtraction: operand not specified (null reference).");
            else if (a.Length != b.Length)
                throw new ArgumentException("Dimension mismatch at vector subtraction. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length 
                    + ",  second operand dim.: " + b.Length + ".");
            else if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            int i;
            for (i = 0; i < a.Length; ++i)
                    result[i] = a[i] - b[i];
        }


        #region ArrayOperations

        /// <summary>Addition of a scalar to all components of a vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void ArrayAddPlain(IVector a, double scal, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] + scal;
        }

        /// <summary>Addition of a scalar to all components of a vector.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void ArrayAdd(IVector a, double scal, IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector array summation: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector array summation is not specified (null reference).");
            else if (result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector summation. " + Environment.NewLine
                    + "  Vector operand dim.: " + a.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] + scal;
        }

        /// <summary>Addition of a scalar to all components of a vector.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void ArrayAdd(IVector a, double scal, ref IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector array summation: operand not specified (null reference).");
            if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] + scal;
        }

        /// <summary>Subtraction of a scalar from all components of a vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void ArraySubtractPlain(IVector a, double scal, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] - scal;
        }

        /// <summary>Subtraction of a scalar from all components of a vector.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void ArraySubtract(IVector a, double scal, IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector array subtraction: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector array subtraction is not specified (null reference).");
            else if (result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector subtraction. " + Environment.NewLine
                    + "  Vector operand dim.: " + a.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] - scal;
        }

        /// <summary>Subtraction of a scalar to all components of a vector.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void ArraySubtract(IVector a, double scal, ref IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector array subtraction: operand not specified (null reference).");
            if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] - scal;
        }

        /// <summary>Multiplication of a vector by a scalar.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void MultiplyPlain(IVector a, double scal, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] * scal;
        }

        /// <summary>Multiplication of a vector by a scalar.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void Multiply(IVector a, double scal, IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector scalar multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector scalar multiplication is not specified (null reference).");
            else if (result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector scalar multiplication. " + Environment.NewLine
                    + "  Vector operand dim.: " + a.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] * scal;
        }

        /// <summary>Multiplication of a vector by a scalar.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void Multiply(IVector a, double scal, ref IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector scalar multiplication: operand not specified (null reference).");
            if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] * scal;
        }

        /// <summary>Multiplication of a vector by a scalar.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void ScalePlain(IVector a, double scal, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] * scal;
        }

        /// <summary>Multiplication of a vector by a scalar.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void Scale(IVector a, double scal, IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector scalar multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector scalar multiplication is not specified (null reference).");
            else if (result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector scalar multiplication. " + Environment.NewLine
                    + "  Vector operand dim.: " + a.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] * scal;
        }

        /// <summary>Multiplication of a vector by a scalar.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void Scale(IVector a, double scal, ref IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector scalar multiplication: operand not specified (null reference).");
            if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] * scal;
        }

        /// <summary>Division of a vector by a scalar.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void DividePlain(IVector a, double scal, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] / scal;
        }

        /// <summary>Division of a vector by a scalar.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void Divide(IVector a, double scal, IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector scalar division: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector scalar division is not specified (null reference).");
            else if (result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector scalar division. " + Environment.NewLine
                    + "  Vector operand dim.: " + a.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] / scal;
        }

        /// <summary>Division of a vector by a scalar.
        /// Vector operand must be defined (non-null).</summary>
        /// <param name="v1">First operand (vector).</param>
        /// <param name="d">Second operand (scalar).</param>
        /// <param name="result">Result.</param>
        public static void Divide(IVector a, double scal, ref IVector result)
        {
            if (a == null)
                throw new ArgumentNullException("Vector scalar division: operand not specified (null reference).");
            if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] / scal;
        }


        #endregion ArrayOperations


        #region Products

        /// <summary>Scalar product of teo vectors.
        /// This is a plain version of the method that does not perform any consistency checks.<summary>
        /// <param name="a">First operand (vector).</param>
        /// <param name="b">Second operand (vector).</param>
        /// <returns>Scalar product of operands.</returns>
        public static double ScalarProductPlain(IVector a, IVector b)
        {
            int i;
            double ret = 0.0;
            for (i = 0; i < a.Length; ++i)
                ret += a[i] * b[i];
            return ret;
        }

        /// <summary>Scalar product of teo vectors.<summary>
        /// <param name="a">First operand (vector).</param>
        /// <param name="b">Second operand (vector).</param>
        /// <returns>Scalar product of operands.</returns>
        public static double ScalarProduct(IVector a, IVector b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Scalar product of two vectors: argument not specified (null reference).");
            else if (a.Length != b.Length)
                throw new ArgumentException("Scalar product of two vectors: inconsistent vector dimensions. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length + ".");
            int i;
            double ret = 0.0;
            for (i = 0; i < a.Length; ++i)
                ret += a[i] * b[i];
            return ret;
        }



        /// <summary>Calculates dyadic product of two vectors and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored. Dimension must match dimensions of operands.</param>
        public static void DyadicProductPlain(IVector a, IVector b, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.Length; ++i)
            {
                for (j = 0; j < b.Length; ++j)
                {
                    result[i,j] = a[i] * b[j];
                }
            }
        }

        /// <summary>Calculates dyadic product of two vectors and stores the result in the specified result matrix.
        /// WARNING: dimensions must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored. Dimension must match dimensions of operands.</param>
        public static void DyadicProduct(IVector a, IVector b, IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Dyadic product of two vectors: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for dyadic product is not specified (null reference).");
            else if (result.RowCount != a.Length || result.ColumnCount != b.Length)
                throw new ArgumentException("Dimension mismatch in dyadic product. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j;
            for (i = 0; i < a.Length; ++i)
            {
                for (j = 0; j < b.Length; ++j)
                {
                    result[i, j] = a[i] * b[j];
                }
            }
        }

        /// <summary>Calculates dyadic product of two vectors and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where copy will be stored.</param>
        public static void DyadicProduct(IVector a, IVector b, ref IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Dyadic product of two vectors: operand not specified (null reference).");
            if (result == null)
                result = a.GetNewMatrix(a.Length, b.Length);
            else if (result.RowCount != a.Length || result.ColumnCount != b.Length)
                result = a.GetNewMatrix(a.Length, b.Length);
            int i, j;
            for (i = 0; i < a.Length; i++)
            {
                for (j = 0; j < b.Length; j++)
                {
                    result[i,j] = a[i] * b[j];
                }
            }
        }



        /// <summary>Calculates a vector product (cross product) of two vectors and stores the result in the specified result vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of all vectors must be 3, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void CrossProductPlain(IVector a, IVector b, IVector result)
        {
            result[0] = a[1] * b[2] - b[2] * a[1];
            result[1] = a[2] * b[0] - b[0] * a[2];
            result[2] = a[0] * b[1] - b[1] * a[0];
        }

        /// <summary>Calculates a vector product (cross product) of two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of all vectors must be 3, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void CrossProduct(IVector a, IVector b, IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector cross product: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector cross product is not specified (null reference).");
            else if (a.Length != b.Length || result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector cross product. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length
                    + ", result dim.: " + result.Length + ".");
            result[0] = a[1] * b[2] - b[2] * a[1];
            result[1] = a[2] * b[0] - b[0] * a[2];
            result[2] = a[0] * b[1] - b[1] * a[0];
        }

        /// <summary>Calculates a vector product (cross product) of two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void CrossProduct(IVector a, IVector b, ref IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector cross product: operand not specified (null reference).");
            else if (a.Length != b.Length)
                throw new ArgumentException("Dimension mismatch at vector cross product. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length + ".");
            if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            result[0] = a[1] * b[2] - b[2] * a[1];
            result[1] = a[2] * b[0] - b[0] * a[2];
            result[2] = a[0] * b[1] - b[1] * a[0];
        }


        /// <summary>Calculates a vector product (cross product) of two vectors and stores the result in the specified result vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of all vectors must be 3, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void VectorProductPlain(IVector a, IVector b, IVector result)
        {
            result[0] = a[1] * b[2] - b[2] * a[1];
            result[1] = a[2] * b[0] - b[0] * a[2];
            result[2] = a[0] * b[1] - b[1] * a[0];
        }

        /// <summary>Calculates a vector product (cross product) of two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of all vectors must be 3, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void VectorProduct(IVector a, IVector b, IVector result)
        {
            CrossProduct(a, b, result);
        }

        /// <summary>Calculates a vector product (cross product) of two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void VectorProduct(IVector a, IVector b, ref IVector result)
        {
            CrossProduct(a, b, ref result);
        }



        /// <summary>Calculates array product (element-by-element product) of two vectors and stores the result in the specified result vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of all vectors must be 3, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void ArrayProductPlain(IVector a, IVector b, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] * b[i];
        }

        /// <summary>Calculates array product (element-by-element product) of two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of all vectors must be 3, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void ArrayProduct(IVector a, IVector b, IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector array product: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector array product is not specified (null reference).");
            else if (a.Length != b.Length || result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector array product. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] * b[i];
        }

        /// <summary>Calculates array product (element-by-element product) of two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void ArrayProduct(IVector a, IVector b, ref IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector array product: operand not specified (null reference).");
            else if (a.Length != b.Length)
                throw new ArgumentException("Dimension mismatch at vector array product. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length + ".");
            if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            ArrayProductPlain(a, b, result);
        }


        /// <summary>Calculates array quotient (element-by-element division) of two vectors and stores the result in the specified result vector.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of all vectors must be 3, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void ArrayQuotientPlain(IVector a, IVector b, IVector result)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] / b[i];
        }

        /// <summary>Calculates array quotient (element-by-element division) of two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of all vectors must be 3, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where result is stored. Dimension must match dimensions of operands.</param>
        public static void ArrayQuotient(IVector a, IVector b, IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector array quotient: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for vector array quotient is not specified (null reference).");
            else if (a.Length != b.Length || result.Length != a.Length)
                throw new ArgumentException("Dimension mismatch at vector array quotient. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length
                    + ", result dim.: " + result.Length + ".");
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] / b[i];
        }

        /// <summary>Calculates array quotient (element-by-element division) of two vectors and stores the result in the specified result vector.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where copy will be stored.</param>
        public static void ArrayQuotient(IVector a, IVector b, ref IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Vector array quotient: operand not specified (null reference).");
            else if (a.Length != b.Length)
                throw new ArgumentException("Dimension mismatch at vector array quotient. " + Environment.NewLine
                    + "  First operand dim.: " + a.Length
                    + ",  second operand dim.: " + b.Length + ".");
            if (result == null)
                result = a.GetNew(a.Length);
            else if (result.Length != a.Length)
                result = a.GetNew(a.Length);
            int i;
            for (i = 0; i < a.Length; ++i)
                result[i] = a[i] / b[i];
        }


        #endregion Products


        #region Auxiliary

        /// <summary>Returns hash code of the specified vector.</summary>
        /// <param name="vec">Vector whose hath code is returned.</param>
        /// <remarks>This method should be used when overriding the GetHashCode() in  vector classes, 
        /// in order to unify equality check over different vector classes.</remarks>
        public static int GetHashCode(IVector vec)
        {
            if (vec == null)
                return int.MaxValue;
            int dim = vec.Length;
            if (dim < 1)
                return int.MaxValue-1+dim;
            int ret = 0;
            for (int i = 0; i < dim; i++)
            {
                ret ^= vec[i].GetHashCode(); ;
            }
            return ret;
        }

        /// <summary>Returns true if the specified vectors are equal, false if not.</summary>
        /// <param name="v1">The first of the two vectors that are checked for equality.</param>
        /// <param name="v2">The second of the two vectors that are checked for equality.</param>
        /// <remarks>
        /// <para>This method should be used when overriding the Equals() method in  vector classes, 
        /// in order to unify calculation of hash code over different vector classes.</para>
        /// <para>If both vectors are nulll or both have dimension less than 1 then vectors are considered equal.</para>
        /// <para>This method is consistent with the <see cref="VectorBase.Compare"/> method, i.e. it returns the 
        /// same value as the expression <see cref="VectorBase.Compare"/>(<paramref name="v1"/>, <paramref name="v2"/>==0).</para>
        /// </remarks>
        public static bool Equals(IVector v1, IVector v2)
        {
            if (v1 == null)
            {
                if (v2 == null)
                    return true;
                else
                    return false;
            }
            else
            {
                int dim = v1.Length;
                if (v2.Length != dim)
                    return false;
                else
                {
                    for (int i = 0; i < dim; ++i)
                    {
                        if (v1[i] != v2[i])
                            return false;
                    }
                    return true;
                }
            }
        }

        /// <summary>Returns an integer valued hash function of the specified vector object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionInt"/> method.</para></summary>
        /// <vec>Vector object whose hash function is calculated and returned.</vec>
        /// <seealso cref="Util.GetHashFunctionInt"/>
        public static int GetHashFunctionInt(IVector vec)
        {
            return Util.GetHashFunctionInt(vec);
        }

        /// <summary>Returns a string valued hash function of the specified vector object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionString"/> method.</para></summary>
        /// <vec>Vector object whose hash function is calculated and returned.</vec>
        /// <seealso cref="Util.GetHashFunctionString"/>
        public static string GetHashFunctionString(IVector vec)
        {
            return Util.GetHashFunctionString(vec);
        }


        #endregion Auxiliary


        #endregion StaticOperations


        #region Operators_Overloading


        /// <summary>Unary plus, returns the operand.</summary>
        public static VectorBase operator +(VectorBase v)
        {
            VectorBase ret = v.GetCopyBase();
            return ret;
        }

        /// <summary>Unary negation, returns the negative operand.</summary>
        public static VectorBase operator -(VectorBase v)
        {
            VectorBase ret = v.GetCopyBase();
            ret.Negate();
            return ret;
        }

        /// <summary>Vector addition.</summary>
        public static VectorBase operator +(VectorBase a, VectorBase b)
        {
            VectorBase ret = a.GetNewBase();
            Add(a, b, ret);
            return ret;
        }


        /// <summary>Vector subtraction.</summary>
        public static VectorBase operator -(VectorBase a, VectorBase b)
        {
            VectorBase ret = a.GetNewBase();
            Subtract(a, b, ret);
            return ret;
        }


        /// <summary>Scalar product of two vectors.</summary>
        public static double operator *(VectorBase a, VectorBase b)
        {
            return ScalarProduct(a, b);
        }

        /// <summary>Product of a vector by a scalar.</summary>
        public static VectorBase operator *(VectorBase a, double b)
        {
            VectorBase ret = a.GetNewBase();
            Multiply(a, b, ret);
            return ret;
        }

        /// <summary>Product of a vector by a scalar.</summary>
        public static VectorBase operator *(double a, VectorBase b)
        {
            VectorBase ret = b.GetNewBase();
            Multiply(b, a, ret);
            return ret;
        }

        /// <summary>Vector subtraction.</summary>
        public static VectorBase operator /(VectorBase a, double b)
        {
            VectorBase ret = a.GetNewBase();
            Divide(a, b, ret);
            return ret;
        }


        #endregion  // Operators_Overloading



        #region InputOutput


        #region StaticInputOutput


        /// <summary>Returns a string representation of the specified vector in a standard IGLib form.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        public static string ToString(IVector vec)
        {
            if (vec == null)
                return Util.NullRepresentationString;
            else
                return ToStringMath(vec);
        }

        /// <summary>Returns a string representation of the specified vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        public static string ToStringMath(IVector vec)
        {
            StringBuilder sb = new StringBuilder();
            if (vec == null)
                sb.Append("{}");
            else
            {
                int d = vec.Length - 1;
                sb.Append('{');
                for (int i = 0; i <= d; ++i)
                {
                    sb.Append(vec[i].ToString(null, CultureInfo.InvariantCulture));
                    if (i < d)
                        sb.Append(", ");
                }
                sb.Append("}");
            }
            return sb.ToString();
        }

        /// <summary>Returns a string representation of the specified vector in a standard IGLib form, with the specified  
        /// format for elements of the vector.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public static string ToString(IVector vec, string elementFormat)
        {
            if (vec == null)
                return Util.NullRepresentationString;
            return ToStringMath(vec, elementFormat);
        }

        /// <summary>Returns a string representation of the specified vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the vector.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public static string ToStringMath(IVector vec, string elementFormat)
        {
            StringBuilder sb = new StringBuilder();
            if (vec == null)
                sb.Append("{}");
            else
            {
                int d = vec.Length - 1;
                sb.Append('{');
                for (int i = 0; i <= d; ++i)
                {
                    sb.Append(vec[i].ToString(elementFormat, CultureInfo.InvariantCulture));
                    if (i < d)
                        sb.Append(", ");
                }
                sb.Append("}");
            }
            return sb.ToString();
        }


        /// <summary>Saves (serializes) the specified vector to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="vec">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file in which object is is saved.</param>
        public static void SaveJson(IVector vec, string filePath)
        {
            SaveJson(vec, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified vector to the specified JSON file.
        /// If the file already exists, contents either overwrites the file or is appended at the end, 
        /// dependent on the value of the append flag.</summary>
        /// <param name="vec">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file in which object is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveJson(IVector vec, string filePath, bool append)
        {
            VectorDtoBase dtoOriginal = new VectorDtoBase();
            dtoOriginal.CopyFrom(vec);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<VectorDtoBase>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) a vector from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which object is restored.</param>
        /// <param name="vecRestored">Object that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref IVector vecRestored)
        {
            ISerializer serializer = new SerializerJson();
            VectorDtoBase dtoRestored = serializer.DeserializeFile<VectorDtoBase>(filePath);
            dtoRestored.CopyTo(ref vecRestored);
        }


        /// <summary>Saves the specified vector to a CSV file.
        /// It the specified file already exists then it is overwritten.
        /// Constant <see cref="UtilStr.DefaultCsvSeparator"/> is used as separator.</summary>
        /// <param name="vec">Vector to be stored to a file.</param>
        /// <param name="filePath">Path of the file to which vector is stored.</param>
        /// <param name="separator">Separator used in the CSV file.</param>
        public static void SaveCsv(IVector vec, string filePath)
        {
            SaveCsv(vec, filePath, UtilStr.DefaultCsvSeparator /* separator */, false /* append */);
        }

        /// <summary>Saves the specified vector to a CSV file.
        /// It the specified file already exists then it is overwritten.</summary>
        /// <param name="vec">Vector to be stored to a file.</param>
        /// <param name="filePath">Path of the file to which vector is stored.</param>
        /// <param name="separator">Separator used in the CSV file.</param>
        public static void SaveCsv(IVector vec, string filePath, string separator)
        {
            SaveCsv(vec, filePath, separator, false /* append */);
        }

        /// <summary>Saves the specified vector to a CSV file.
        /// Constant <see cref="UtilStr.DefaultCsvSeparator"/> is used as separator in CSV.</summary>
        /// <param name="vec">Vector to be stored to a file.</param>
        /// <param name="filePath">Path of the file to which vector is stored.</param>
        /// <param name="append">Specifies whether the data is appended at the end of the file
        /// in the case that the ifle already exists.</param>
        public static void SaveCsv(IVector vec, string filePath, bool append)
        {
            SaveCsv(vec, filePath, UtilStr.DefaultCsvSeparator, append);
        }

        /// <summary>Saves the specified vector to a CSV file.</summary>
        /// <param name="vec">Vector to be stored to a file.</param>
        /// <param name="filePath">Path of the file to which vector is stored.</param>
        /// <param name="separator">Separator used in the CSV file.</param>
        /// <param name="append">Specifies whether the data is appended at the end of the file
        /// in the case that the ifle already exists.</param>
        public static void SaveCsv(IVector vec, string filePath, string separator, bool append)
        {
            int dimension = 0;
            if (vec != null)
                dimension = vec.Length;
            if (dimension < 0)
                dimension = 0;
            string[][] values = new string[1][];
            values[0] = new string[dimension];
            for (int i = 0; i < dimension; ++i)
                values[0][i] = vec[i].ToString();
            UtilStr.SaveCsv(filePath, values, separator, append);
        }

        /// <summary>Reads a vector from a CSV file.
        /// Constant <see cref="Str.DefaultCsvSeparator"/> is used as separator in CSV file.
        /// If there are no components then a null vector is returned by this method (no exceptions thrown).
        /// If there are more than one rows in the CSV file then vector is read from the first row.</summary>
        /// <param name="filePath">Path to the file that contains a vector in CSV format.</summary>
        /// <param name="filePath">Path to the file that vector is read form.</param>
        /// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        public static void LoadCsv(string filePath, ref IVector vecRestored)
        {
            LoadCsv(filePath, UtilStr.DefaultCsvSeparator, ref vecRestored);
        }

        /// <summary>Reads a vector written in CSV format from a file.
        /// If there are no components then a null vector is returned by this method (no exceptions thrown).
        /// If there are more than one rows in the CSV file then vector is read from the first row.</summary>
        /// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        /// <param name="separator">Separator that is used to separate values in a row in the CSV file.</param>
        /// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        public static void LoadCsv(string filePath, string separator, ref IVector vecRestored)
        {
            string[][] values = null;
            values = UtilStr.LoadCsv(filePath, separator);
            if (values == null)
                vecRestored = null;
            else if (values.Length==0)
                vecRestored = null;
            else
            {
                int dimension = values[0].Length;
                Vector.Resize(ref vecRestored, dimension);
                for (int i = 0; i < dimension; ++i)
                {
                    double comp = 0;
                    bool readCorrectly = double.TryParse(values[0][i], out comp);
                    if (readCorrectly)
                        vecRestored[i] = comp;
                    else
                    {
                        throw new FormatException("Vector coponenet No. " + i + " in a CSV file is not a number. "
                            + Environment.NewLine + "  Component representation in the file: " + values[0][i]);
                    }
                }
            }
        }


        /// <summary>Reads a vector from the specified row of a CSV file.
        /// Constant <see cref="UtilStr.DefaultCsvSeparator"/> is used as CSV separator.
        /// If the specified row does not exisist in the file then exception is thrown.</summary>
        /// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        /// <param name="rowNum">Number of the row from which the vector is read.</param>
        /// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        public static void LoadCsv(string filePath, int rowNum, ref IVector vecRestored)
        {
            LoadCsv(filePath, rowNum, UtilStr.DefaultCsvSeparator /* separator */, ref vecRestored);
        }

        /// <summary>Reads a vector from the specified row of a CSV file.
        /// If the specified row does not exisist in the file then exception is thrown.</summary>
        /// <param name="filePath">Path to the file that contains a vector in CSV format.</param>
        /// <param name="rowNum">Number of the row from which the vector is read.</param>
        /// <param name="separator">Separator that is used to separate values in a row in the CSV file.</param>
        /// <param name="vecRestored">Vector object where the read-in vector is stored.</param>
        public static void LoadCsv(string filePath, int rowNum, string separator, ref IVector vecRestored)
        {
            string[][] values = null;
            values = UtilStr.LoadCsv(filePath, separator);
            if (values == null)
                throw new FormatException("CSV file has no rows, can not read vector from row " + rowNum + ".");
            else if (values.Length < rowNum + 1)
                throw new FormatException("CSV file has only " + values.Length + " rows, can not read vector from row " + rowNum + ".");
            else
            {
                int dimension = values[rowNum].Length;
                Vector.Resize(ref vecRestored, dimension);
                for (int i = 0; i < dimension; ++i)
                {
                    int comp = 0;
                    bool readCorrectly = int.TryParse(values[rowNum][i], out comp);
                    if (readCorrectly)
                        vecRestored[i] = comp;
                    else
                    {
                        throw new FormatException("Vector coponenet No. " + i + ", element (" + rowNum + 
                            "," + i + ") of a CSV file, is not a number. "
                            + Environment.NewLine + "  Component representation in the file: " + values[rowNum][i]);
                    }
                }
            }
        }


        #endregion StaticInputOutput



        /// <summary>Returns a string representation of this vector in a standard IGLib form.</summary>
        public override string ToString()
        {
            return ToString(this);
        }

        /// <summary>Returns a string representation of this vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).</summary>
        public virtual string ToStringMath()
        {
            return ToStringMath(this);
        }


        /// <summary>Returns a string representation of the current vector in a standard IGLib form, with the specified  
        /// format for elements of the vector.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public virtual string ToString(string elementFormat)
        {
            return ToString(this, elementFormat);
        }

        /// <summary>Returns a string representation of the current vector in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the vector.</summary>
        /// <param name="vec">Vector whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public virtual string ToStringMath(string elementFormat)
        {
            return ToStringMath(this, elementFormat);
        }


        #endregion InputOutput


    }  // class VectorBase


    /// <summary>Vector store.
    /// <para>Stores Vector objects for reuse.</para>
    /// <para>Can be used for storage fo vectors with specific dimension (default) or for torage of any non-null vectors.</para></summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VectorStore<T> : ObjectStore<T>, ILockable
        where T : class, IVector
    {

        /// <summary>Constructs a new Vector store of unspecified dimensions.</summary>
        protected VectorStore()
            : base()
        { }

        /// <summary>Constructs a new Vector store for vectors with the specified dimensions.</summary>
        /// <param name="length">Dimension of stored vectors.</param>
        public VectorStore(int length)
            : this()
        {
            this._length = length;
        }

        /// <summary>Constructs a new Vector store for vectors with the specified dimensions.
        /// <para>If <param name="constrainDimensions"></param> is false then store can be used for vectors with any dimensions.</para></summary>
        /// <param name="length">Dimension of stored vectors.</param>
        public VectorStore(int length, bool constrainDimensions)
            : this(length)
        {
            this._constrainDimensions = constrainDimensions;
        }

        /// <summary>Ilf true then only vectors with matching dimensions are eligible for storing.
        /// <para>Default is true.</para></summary>
        protected bool _constrainDimensions = true;

        /// <summary>Ilf true then only vectors with matching dimensions are eligible for storing.</summary>
        public bool ConstrainDimensions
        {
            get { lock (Lock) { return _constrainDimensions; } }
            set
            {
                lock (Lock)
                {
                    bool checkEligible = false;
                    if (_constrainDimensions == false && value == true)
                        checkEligible = true;
                    _constrainDimensions = value;
                    if (checkEligible)
                        ClearIneligible();
                }
            }
        }

        protected int _length;

        /// <summary>Dimension for vectors to be stored.
        /// <para>If <see cref="ConstrainDimensions"/> is true then only vectors that match dimensions are eligible for storage.
        /// Otherwise, dimensions are only important for creation of new vectors.</para></summary>
        public int Length
        {
            get { lock (Lock) { return Length; } }
            set
            {
                lock (Lock)
                {
                    bool checkEligible = false;
                    if (value != _length && _constrainDimensions)
                        checkEligible = true;
                    _length = value;
                    if (checkEligible)
                        ClearIneligible();
                }
            }
        }

        
        /// <summary>Returns true if the specified Vector is eligible for storage in the current store, false if not.</summary>
        /// <param name="vec">Vector whose eligibility is checked.</param>
        public override bool IsEligible(T vec)
        {
            lock (Lock)
            {
                bool ret = true;
                if (_constrainDimensions)
                {
                    if (vec == null)
                        ret = false;
                    else if (vec.Length != _length)
                        ret = false;
                }
                return ret;
            }
        }
    }  // VectorStore<T>


    /// <summary>Vector store.
    /// <para>Stores Vector objects for reuse.</para>
    /// <para>Can be used for storage fo vectors with specific dimension (default) or for torage of any non-null vectors.</para></summary>
    public class VectorStore : VectorStore<Vector>, ILockable
    {

        /// <summary>Constructs a new Vector store of unspecified dimensions.</summary>
        protected VectorStore()
            : base()
        { }

        /// <summary>Constructs a new Vector store for vectors with the specified dimension.</summary>
        /// <param name="length">Dimension of stored vectors.</param>
        public VectorStore(int length)
            : base(length)
        { }


        /// <summary>Constructs a new Vector store.
        /// <para>If <param name="constrainDimensions"></param> is false then store can be used for vectors with any dimensions.</para></summary>
        /// <param name="length">Dimension of stored vectors.</param>
        public VectorStore(int length, bool constrainDimensions)
            : base(length, constrainDimensions)
        { }

        /// <summary>Returns a newly created object eligible for storage, or null if such an object
        /// can not be created. This method should not throw an exception.</summary>
        protected override Vector TryGetNew()
        {
            if (_length > 0)
            {
                return new Vector(_length);
            }
            else
                return null;
        }

    } // class VectorStore



}