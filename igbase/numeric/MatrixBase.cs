// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

    // MATRICES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;


//$$
//using Matrix_MathNet = MathNet.Numerics.LinearAlgebra.Matrix;

using Matrix_MathNetNumercs = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix;
using MatrixBase_MathNetNumerics = MathNet.Numerics.LinearAlgebra.Generic.Matrix<double>;

using IG.Lib;

namespace IG.Num
{



    //TODO: Define abstract class MatrixBase from which all matrix classes will inherit!

    /// <summary>Generic Matrix interface.</summary>
    /// $A Igor Sep08 Jan09;
    public interface IMatrix<ComponentType>
    {

        /// <summary>Gets the number of rows.</summary>
        int RowCount { get; }

        /// <summary>Gets the number of columns.</summary>
        int ColumnCount { get; }

        /// <summary>Gets total number of elements.
        /// <para>Warning: this is usually done by multiplying <see cref="RowCount"/> and <see cref="ColumnCount"/>,
        /// so it is not a priceless operation.</para></summary>
        int Count { get; }

        /// <summary>Gets or set the element indexed by <c>(i, j)</c> in the <c>Matrix</c>.</summary>
        /// <param name="i">Row index.</param>
        /// <param name="j">Column index.</param>
        ComponentType this[int i, int j] { get; set; }

        /// <summary>Gets or set the element indexed by the specified <c>flat index</c> in the <c>Matrix</c>.</summary>
        /// <param name="flatIndex">Flat (one dimensional) index that addresses matrix element.</param>
        ComponentType this[int flatIndex] { get; set; }

        /// <summary>Creates and returns a rectangular 2D array that contains a component-wise copy of the matrix.</summary>
        ComponentType[,] ToArray();

        /// <summary>Creates and returns a jagged 2D array that contains a component-wise copy of the matrix.</summary>
        ComponentType[][] ToJaggedArray();

        #region Operations

        /// <summary>Sets all components of the current matrix to 0.</summary>
        void SetZero();

        /// <summary>Negates the matrix.</summary>
        void Negate();

        /// <summary>Gets matrix trace (sum of diagonal terms).</summary>
        ComponentType Trace { get; }

        #region Norms

        /// <summary>Gets Forbenious (or Euclidean) norm of the matrix - square root of sum of squares of elements.</summary>
        double NormForbenius
        { get; }

        /// <summary>Gets Forbenious (or Euclidean) norm of the matrix - square root of sum of squares of elements.</summary>
        [Obsolete("Use NormForbenius instead!")]
        double NormEuclidean
        { get; }

        /// <summary>Gets Forbenious (or Euclidean) norm of the matrix - square root of sum of squares of elements.</summary>
        double Norm
        { get; }

        #endregion Norms

        #endregion Operations


    }  // interface IMatrix<T>


    /// <summary>Real matrix interface.</summary>
    /// $A Igor Sep08 Dec09;
    public interface IMatrix : IMatrix<double>
    {

        ///// <summary>Creates and returns a copy of the current matrix.</summary>
        ///// <returns>A new copy of the current matrix. 
        ///// The copy is supposed to be of the same type as the current matrix.</returns>
        //MatrixBase GetCopyBase();

        ///// <summary>Creates and returns a new matrix with the specified dimensions, and of the same type as 
        ///// the current matrix.</summary>
        ///// <param name="rowCount">Number fo rows of the newly created matrix.</param>
        ///// <param name="columnCount">Number of columns of the newly created matrix.</param>
        ///// <returns>A newly created matrix of the specified dimensions and of the same type as the current matrix.</returns>
        //MatrixBase GetNewBase(int rowCount, int columnCount);

        ///// <summary>Creates and returns a new matrix with the same dimensions and of the same type as 
        ///// the current matrix.</summary>
        //MatrixBase GetNewBase();

        ///// <summary>Creates and returns a new vector with the specified dimension, 
        ///// and of the type that is consistent with the type of the current vector.</summary>
        ///// <param name="length">Dimension of the newly created vector.</param>
        ///// <returns>A newly created vector of the specified dimension and of the same type as the current vector.</returns>
        //VectorBase GetNewVectorBase(int length);

        /// <summary>Creates and returns a copy of the current matrix.</summary>
        /// <returns>A new copy of the current matrix. 
        /// The copy is supposed to be of the same type as the current matrix.</returns>
        IMatrix GetCopy();

        /// <summary>Creates and returns a new matrix with the specified dimensions, and of the same type as 
        /// the current matrix.</summary>
        /// <param name="rowCount">Number fo rows of the newly created matrix.</param>
        /// <param name="columnCount">Number of columns of the newly created matrix.</param>
        /// <returns>A newly created matrix of the specified dimensions and of the same type as the current matrix.</returns>
        IMatrix GetNew(int rowCount, int columnCount);

        /// <summary>Creates and returns a new matrix with the same dimensions and of the same type as 
        /// the current matrix.</summary>
        IMatrix GetNew();

        /// <summary>Creates and returns a new vector with the specified dimension, 
        /// and of the type that is consistent with the type of the current vector.</summary>
        /// <param name="length">Dimension of the newly created vector.</param>
        /// <returns>A newly created vector of the specified dimension and of the same type as the current vector.</returns>
        IVector GetNewVector(int length);

        
        /// <summary>Sets all components of the current matrix to the specified value.</summary>
        /// <param name="elementValue">Value to which elements are set.</param>
        void SetConstant(double elementValue);

        /// <summary>Sets the current matrix to identity matrix.</summary>
        void SetIdentity();

        /// <summary>Sets the current matrix such that it contains random elements on the interval (0,1].</summary>
        void SetRandom();

        /// <summary>Sets the current matrix such that it contains random elements on the interval (0,1].</summary>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        void SetRandom(IRandomGenerator rnd);
        
        /// <summary>Sets the current matrix to the diagonal matrix with all diagonal elements 
        /// equal to the specified value.
        /// Matrix does not need to be a square matrix.</summary>
        /// <param name="diagonalElement">Value of diagonal elements.</param>
        void SetDiagonal(double diagonalElement);

        /// <summary>Sets the current matrix to the diagonal matrix with diagonal element specified by a vector.</summary>
        /// <param name="diagonal">Vector of diagonal elements.</param>
        void SetDiagonal(IVector diagonal);

        /// <summary>Returns true if the current matrix is a square matrix, and false if not.</summary>
        bool IsSquare();

        /// <summary>Returns true if the current matrix is symmetric, and false if not.
        /// If the matrix is not a square matrix then false is returned.</summary>
        bool IsSymmetric();

        /// <summary>Returns true if the specified matrix is symmetric within some tolerance, and false if not.
        /// If the matrix is null then false is returned.</summary>
        /// <param name="relativeRMSTolerance">Tolerance on the ratio between RMS of differences between out of diagonal terms
        /// and their transposes and between RMS of out of diagonal terms, below which matrix is considered symmetric.</param>
        bool IsSymmetric(double relativeRMSTolerance);

        
         
        /// <summary>Returns a readable an easily string form of a matrix, accuracy and padding can be set.</summary>
        /// <param name="mat">Matrix to be changed to a string.</param>
        /// <param name="accuracy">Accuracy of matrix elments representations.</param>
        /// <param name="padding">Paddind of matrix elements.</param>
        /// <returns>A readable string representation in tabular form.</returns>
        string ToStringReadable(int accuracy = 4, int padding = 8);


        /// <summary>Returns a string representation of this matrix with newlines inserted after each row.
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        string ToStringNewlines();

        /// <summary>Returns a string representation of this matrix with newlines inserted after each row, 
        /// with the specified format for elements of the matrix.
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        string ToStringNewlines(string elementFormat);

        /// <summary>Returns string representation of the current matrix in the standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        string ToStringMath();

        /// <summary>Returns string representation of the current matrix in the standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        string ToString();

        /// <summary>Returns a string representation of the current matrix in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the matrix.</summary>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        string ToString(string elementFormat);

        /// <summary>Returns a string representation of the current matrix in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the matrix.</summary>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        string ToStringMath(string elementFormat);
        
        /// <summary>Returns an integer valued hash function of the current matrix object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionInt"/> method.</para></summary>
        /// <seealso cref="Util.GetHashFunctionInt"/>
        int GetHashFunctionInt();
        
        /// <summary>Returns a string valued hash function of the current matrix object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionString"/> method.</para></summary>
        /// <remarks>The returned string is always on the same length, and is based on the <see cref="ToString"/> method.
        /// Therefore it is convenient for use in file or directory names that have one part related to a specific matrix.</remarks>
        /// <seealso cref="Util.GetHashFunctionString"/>
        string GetHashFunctionString();

    }  //  interface IMatrix


    /// <summary>Extension methods for IMatrix interface.</summary>
    /// $A Igor Apr11;
    public static class MatrixExtensions
    {

        #region ToString

        /// <summary>Easily readable string form of a matrix, accuracy and padding can be set.</summary>
        /// <param name="mat">Matrix to be changed to a string.</param>
        /// <param name="accuracy">Accuracy of matrix elments representations.</param>
        /// <param name="padding">Paddind of matrix elements.</param>
        /// <returns>A readable string representation in tabular form.</returns>
        public static string ToStringReadable(this IMatrix mat, int accuracy = 4, int padding = 8)
        {
            return MatrixBase.ToStringReadable(mat, accuracy, padding);
        }


        /// <summary>Returns string representation of the current matrix in the standard IGLib form. 
        /// Rows and elements are printed in comma separated lists in curly brackets.
        /// Extension method for IMatrix interface.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToStringMath(this IMatrix mat)
        {
            return MatrixBase.ToStringMath(mat);
        }

        /// <summary>Returns string representation of the current matrix in the standard IGLib form, with the specified  
        /// format for elements of the matrix. 
        /// Rows and elements are printed in comma separated lists in curly brackets.
        /// Extension method for IMatrix interface.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToString(this IMatrix mat, string elementFormat)
        {
            return MatrixBase.ToString(mat, elementFormat);
        }

        /// <summary>Returns string representation of the current matrix in the standard IGLib form, with the specified  
        /// format for elements of the matrix. 
        /// Rows and elements are printed in comma separated lists in curly brackets.
        /// Extension method for IMatrix interface.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToStringMath(this IMatrix mat, string elementFormat)
        {
            return MatrixBase.ToStringMath(mat, elementFormat);
        }


        /// <summary>Returns a string representation of the current matrix with newlines inserted after each row.
        /// Rows and elements are printed in comma separated lists in curly brackets.
        /// Extension nethod for IMatrix interface.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToStringNewlines(IMatrix mat)
        {
            return MatrixBase.ToStringNewlines(mat);
        }

        /// <summary>Returns a string representation of the current matrix with newlines inserted after each row, 
        /// with the specified format for elements of the matrix.
        /// Rows and elements are printed in comma separated lists in curly brackets.
        /// Extension nethod for IMatrix interface.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToStringNewlines(IMatrix mat, string elementFormat)
        {
            return MatrixBase.ToStringNewlines(mat, elementFormat);
        }

        
        /// <summary>Returns string representation of the current matrix in the standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Rows and elements are printed in comma separated lists in curly brackets.
        /// Extension nethod for IMatrix interface.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToStringMath1(IMatrix mat)
        {
            return MatrixBase.ToStringMath(mat);
        }

        #endregion ToString



    } // MatrixExtensions


    /// <summary>Base class for matrices.</summary>
    /// $A Igor Jan08 Jul10 Nov10;
    public abstract class MatrixBase: IMatrix
    {

        #region Data

        /// <summary>Gets the number of rows of the current matrix.</summary>
        public abstract int RowCount
        {
            get;
        }

        /// <summary>Gets the number of columns of the current matrix.</summary>
        public abstract int ColumnCount
        {
            get;
        }

        /// <summary>Gets total number of elements.
        /// <para>Warning: this is usually done by multiplying <see cref="RowCount"/> and <see cref="ColumnCount"/>,
        /// so it is not a priceless operation.</para></summary>
        public virtual int Count { get { return RowCount + ColumnCount; } }

        ///// <summary>Gets or sets the specified component of the current matrix.</summary>
        ///// <param name="i">Row number of the component.</param>
        ///// <param name="j">Column number of the component.</param>
        //public abstract double this[int i, int j]
        //{
        //    get;
        //    set;
        //}


        /// <summary>Creates and returns a copy of the current matrix.</summary>
        /// <returns>A new copy of the current matrix. 
        /// The copy is supposed to be of the same type as the current matrix.</returns>
        public abstract MatrixBase GetCopyBase();

        /// <summary>Creates and returns a new matrix with the specified dimensions, and of the same type as 
        /// the current matrix.</summary>
        /// <param name="rowCount">Number fo rows of the newly created matrix.</param>
        /// <param name="columnCount">Number of columns of the newly created matrix.</param>
        /// <returns>A newly created matrix of the specified dimensions and of the same type as the current matrix.</returns>
        public abstract MatrixBase GetNewBase(int rowCount, int ColumnCount);

                
        /// <summary>Creates and returns a new matrix with the same dimensions and of the same type as 
        /// the current matrix.</summary>
        public abstract MatrixBase GetNewBase();


        /// <summary>Creates and returns a new vector with the specified dimension, 
        /// and of the type that is consistent with the type of the current vector.</summary>
        /// <param name="length">Dimension of the newly created vector.</param>
        /// <returns>A newly created vector of the specified dimension and of the same type as the current vector.</returns>
        public abstract VectorBase GetNewVectorBase(int length);

        /// <summary>Creates and returns a copy of the current matrix.</summary>
        /// <returns>A new copy of the current matrix. 
        /// The copy is supposed to be of the same type as the current matrix.</returns>
        public virtual IMatrix GetCopy()
        { return GetCopyBase(); }

        /// <summary>Creates and returns a new matrix with the specified dimensions, and of the same type as 
        /// the current matrix.</summary>
        /// <param name="rowCount">Number fo rows of the newly created matrix.</param>
        /// <param name="columnCount">Number of columns of the newly created matrix.</param>
        /// <returns>A newly created matrix of the specified dimensions and of the same type as the current matrix.</returns>
        public virtual IMatrix GetNew(int rowCount, int columnCount)
        { return GetNewBase(rowCount, columnCount); }

        /// <summary>Creates and returns a new matrix with the same dimensions and of the same type as 
        /// the current matrix.</summary>
        public virtual IMatrix GetNew()
        { return GetNewBase(); }

        /// <summary>Creates and returns a new vector with the specified dimension, 
        /// and of the type that is consistent with the type of the current vector.</summary>
        /// <param name="length">Dimension of the newly created vector.</param>
        /// <returns>A newly created vector of the specified dimension and of the same type as the current vector.</returns>
        public virtual IVector GetNewVector(int length)
        { return GetNewVectorBase(length); }

        /// <summary>Creates and returns a rectangular 2D array that contains a component-wise copy of the matrix.</summary>
        public virtual double[,] ToArray()
        {
            double[,] ret = new double[RowCount, ColumnCount];
            for (int i = 0; i < RowCount; ++i)
                for (int j = 0; j < ColumnCount; ++j )
                {
                    ret[i, j] = this[i, j];
                }
            return ret;
        }

        /// <summary>Creates and returns a jagged 2D array that contains a component-wise copy of the matrix.</summary>
        public virtual double[][] ToJaggedArray()
        {
            double[][] ret = new double[RowCount][];
            for (int i = 0; i < RowCount; ++i)
            {
                ret[i] = new double[ColumnCount];
                for (int j = 0; j < ColumnCount; ++j)
                {
                    ret[i][j] = this[i, j];
                }
            }
            return ret;
        }

        /// <summary>Returns a new matrix that is transpose of the current matrix.</summary>
        public virtual IMatrix T
        {
            get
            {
                IMatrix ret = GetNewBase(ColumnCount, RowCount);
                Transpose(this, ret);
                return ret;
            }
        }

        #endregion Data


        #region Indices

        /// <summary>Gets or set the element indexed by a flat index in the <c>Matrix</c>.
        /// <remarks>This method just provides a mechanism of addressing elements by a single (flat) index in derived 
        /// classes where implementation adheres to addressing by a single index (e.g. where elements are
        /// stored in a two dimensional array). In other cases, this indexing operator will be overridden.</remarks></summary>
        /// <param name="flatIndex">Flat element index, i.e. one dimensional index of a matrix element
        /// when elements are expanded to one dimensional array.</param>
        public virtual double this[int flatIndex]
        {
            get
            {
                return this[flatIndex / ColumnCount, flatIndex % ColumnCount];
            }
            set 
            {
                this[flatIndex / ColumnCount, flatIndex % ColumnCount] = value;
            }
        }

        /// <summary>Gets or set the element indexed by <c>(i, j)</c> in the <c>Matrix</c>.
        /// <remarks>This method just provides a mechanism of addressing elements by two indices in derived 
        /// classes where implementation adheres to addressing by a single index (e.g. where elements are
        /// stored in a one dimensional array). In other cases, this indexing operator will be overridden.</remarks></summary>
        /// <param name="row">Row index of the element.</param>
        /// <param name="column">Column index of the element.</param>
        public virtual double this[int row, int column]
        {
            get
            {
                return this[row * ColumnCount + column];
            }
            set 
            {
                this[row * ColumnCount + column] = value;
            }
        }


        /// <summary>Returns flat index corresponding to the specified row and column indices of a matrix
        /// with specified dimensions.</summary>
        /// <param name="dim1">First index.</param>
        /// <param name="dim2">Second index.</param>
        /// <param name="row">Row number (0-based).</param>
        /// <param name="column">Coolumn number (0-based).</param>
        public static int Index(int dim1, int dim2, int row, int column)
        {
            return row * dim2 + column;
        }
        
        /// <summary>Calculates and returns flat index corresponding to the specified row and column indices of the 
        /// specified matrix.</summary>
        /// <param name="mat">Matrix for which flat index is calculated.</param>
        /// <param name="row">Row number (0-based).</param>
        /// <param name="column">Coolumn number (0-based).</param>
        public static int Index(IMatrix mat, int row, int column)
        {
            if (mat == null)
                throw new ArgumentException("Matrix where index is looked for is not specified (null reference).");
            return Index(mat.RowCount, mat.ColumnCount, row, column);
        }
       
        /// <summary>Calculates and returns flat index corresponding to the specified row and column indices of the 
        /// current matrix.</summary>
        /// <param name="row">Row number (0-based).</param>
        /// <param name="column">Coolumn number (0-based).</param>
        public virtual int Index(int row, int column)
        {
            return Index(this.RowCount, this.ColumnCount, row, column);
        }

        /// <summary>Returns (through output arguments) row and column indices corresponding to the
        /// specified flat index in a matrix with the specified dimensions.</summary>
        /// <param name="dim1">First dimension of the matrix (number of rows).</param>
        /// <param name="dim2">Second dimension of the matrix (number of columns).</param>
        /// <param name="flatIndex">Flat element index, i.e. one dimensional index of a matrix element
        /// when elements are expanded to one dimensional array.</param>
        /// <param name="row">Row index corresponding to the specified flat index.</param>
        /// <param name="column">Column index corresponding to the specified flat index.</param>
        public static void Indices(int dim1, int dim2, int flatIndex, out int row, out int column)
        {
            row = flatIndex / dim2;
            column = flatIndex % dim2;
        }

        /// <summary>Returns (through output arguments) row and column indices corresponding to the
        /// specified flat index in the specified matrix.</summary>
        /// <param name="mat">Matrix for which flat index is calculated.</param>
        /// <param name="flatIndex">Flat element index, i.e. one dimensional index of a matrix element
        /// obtained as if elements were expanded to one dimensional array.</param>
        /// <param name="row">Row index corresponding to the specified flat index.</param>
        /// <param name="column">Column index corresponding to the specified flat index.</param>
        public static void Indices(IMatrix mat, int flatIndex, out int row, out int column)
        {
            if (mat == null)
                throw new ArgumentException("Matrix where index is looked for is not specified (null reference).");
            Indices(mat.RowCount, mat.ColumnCount, flatIndex, out row, out column);
        }

        /// <summary>Returns (through output arguments) row and column indices corresponding to the
        /// specified flat index in the current matrix.</summary>
        /// <param name="flatIndex">Flat element index, i.e. one dimensional index of a matrix element
        /// obtained as if elements were expanded to one dimensional array.</param>
        /// <param name="row">Row index corresponding to the specified flat index.</param>
        /// <param name="column">Column index corresponding to the specified flat index.</param>
        public virtual void Indices(int flatIndex, out int row, out int column)
        {
            Indices(RowCount, ColumnCount, flatIndex, out row, out column);
        }


        #endregion Indices



        #region Operations

        #region SetValue

        /// <summary>Sets all components of the current matrix to 0.</summary>
        public virtual void SetZero()
        {
            for (int i = 0; i < RowCount; ++i)
                for (int j = 0; j < ColumnCount; ++j)
                    this[i, j] = 0.0;
        }

        /// <summary>Sets all components of the current matrix to the specified value.</summary>
        /// <param name="elementValue">Value to which elements are set.</param>
        public virtual void SetConstant(double elementValue)
        {
            for (int i = 0; i < RowCount; ++i)
                for (int j = 0; j < ColumnCount; ++j)
                    this[i, j] = elementValue;
        }

        /// <summary>Sets the current matrix to identity matrix.
        /// WARNING:
        /// Exception is thrown if a matrix is not square. For nonsquare matrices, use SetDiagonal(1.0)!</summary>
        public virtual void SetIdentity()
        {
            if (RowCount!=ColumnCount)
                throw new InvalidOperationException("Attempt to set a non-square matrix to identity matrix.");
            for (int i = 0; i < RowCount; ++i)
                for (int j = 0; j < ColumnCount; ++j)
                {
                    if (i == j)
                        this[i, j] = 1.0;
                    else
                        this[i, j] = 0.0;
                }
        }

        /// <summary>Sets the current matrix such that it contains random elements on the interval (0,1].</summary>
        public virtual void SetRandom()
        {
            SetRandom(this);
        }

        /// <summary>Sets the current matrix such that it contains random elements on the interval (0,1].</summary>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        public virtual void SetRandom(IRandomGenerator rnd)
        {
            SetRandom(this, rnd);
        }


        /// <summary>Sets the current matrix to the diagonal matrix with all diagonal elements 
        /// equal to the specified value.
        /// Matrix does not need to be a square matrix.</summary>
        /// <param name="diagonalElement">Value of diagonal elements.</param>
        public virtual void SetDiagonal(double diagonalElement)
        {
            for (int i = 0; i < RowCount; ++i)
                for (int j = 0; j < ColumnCount; ++j)
                {
                    if (i == j)
                        this[i, j] = diagonalElement;
                    else
                        this[i, j] = 0.0;
                }
        }

        /// <summary>Sets the current matrix to the diagonal matrix with diagonal element specified by a vector.</summary>
        /// <param name="diagonal">Vector of diagonal elements.</param>
        public virtual void SetDiagonal(IVector diagonal)
        {
            SetDiagonal(this, diagonal);
        }

        #endregion SetValue


        #region QueryType

        /// <summary>Returns true if the current matrix is a square matrix, and false if not.</summary>
        public virtual bool IsSquare()
        {
            if (RowCount == ColumnCount)
                return true;
            return false;
        }

        /// <summary>Returns true if the current matrix is symmetric, and false if not.
        /// If the matrix is not a square matrix then false is returned.</summary>
        public virtual bool IsSymmetric()
        {
            return IsSymmetric(this);
        }

        /// <summary>Returns true if the specified matrix is symmetric within some tolerance, and false if not.
        /// If the matrix is null then false is returned.</summary>
        /// <param name="relativeRMSTolerance">Tolerance on the ratio between RMS of differences between out of diagonal terms
        /// and their transposes and between RMS of out of diagonal terms, below which matrix is considered symmetric.</param>
        public virtual bool IsSymmetric(double relativeRMSTolerance = 0.0)
        {
            return IsSymmetric(this, relativeRMSTolerance);
        }


        #endregion QueryType


        /// <summary>Gets matrix trace (sum of diagonal terms).</summary>
        public virtual double Trace
        {
            get
            {
                if (RowCount != ColumnCount)
                    throw new InvalidOperationException("Can not calculate trace of a non-square matrix.");
                double ret = 0.0;
                for (int i = 0; i < RowCount; ++i)
                    ret += this[i, i];
                return ret;
            }
        }

        /// <summary>Negates the current matrix.</summary>
        public virtual void Negate()
        {
            int i, j;
            for (i = 0; i < this.RowCount; ++i)
                for (j = 0; j < this.ColumnCount; ++j)
                    this[i, j] = -this[j, i];
        }

        /// <summary>Transposes the current matrix.
        /// WARNING: this operation can only be done on square matrices!</summary>
        public virtual void Transpose()
        {
            int i, j;
            double element;
            if (RowCount != ColumnCount)
                throw new InvalidOperationException("Non-square matrix can not be transposed in-place.");
            for (i = 1; i < this.RowCount; ++i)
                for (j = 0; j < i; ++j)
                {
                    element = this[i, j];
                    this[i, j] = this[j, i];
                    this[j, i] = element;
                }
        }



        #region Norms


        /// <summary>Gets Forbenious (or Euclidean) norm of the matrix - square root of sum of squares of elements.</summary>
        public virtual double NormForbenius
        {
            get
            {
                double ret = 0.0;
                int i, j;
                for (i = 0; i < RowCount; ++i)
                    for (j = 0; j < ColumnCount; ++j)
                        ret += this[i, j] * this[i, j];
                return Math.Sqrt(ret);
            }
        }

        /// <summary>Gets Forbenious (or Euclidean) norm of the matrix - square root of sum of squares of elements.</summary>
        [Obsolete("Use NormForbenius instead!")]
        public virtual double NormEuclidean
        {  get{ return NormForbenius; } }

        /// <summary>Gets Forbenious (or Euclidean) norm of the matrix - square root of sum of squares of elements.</summary>
        public virtual double Norm
        { get { return NormForbenius; } }



        #endregion Norms



        #region Operations.Auxiliary


        /// <summary>Returns the hash code (hash function) of the current matrix.</summary>
        /// <remarks>
        /// <para>This method calls the <see cref="MatrixBase.GetHashCode"/> to calculate the 
        /// hash code, which is standard for all implementations of the <see cref="IMatrix"/> interface.</para>
        /// <para>Two matrices that have the same dimensions and equal elements will produce the same hash codes.</para>
        /// <para>Probability that two different matrices will produce the same hash code is small but it exists.</para>
        /// <para>Overrides the <see cref="object.GetHashCode"/> method.</para>
        /// </remarks>
        public override int GetHashCode()
        {
            return MatrixBase.GetHashCode(this);
        }

        /// <summary>Returns a value indicating whether the specified object is equal to the current matrix.
        /// <para>True is returned if the object is a non-null matrix (i.e. it implements the <see cref="IMatrix"/>
        /// interface), and has the same dimension and equal elements as the current matrix.</para></summary>
        /// <remarks>This method calls the <see cref="MatrixBase.Equals"/> to obtain the returned value, which is
        /// standard for all implementations of the <see cref="IMatrix"/> interface.
        /// <para>Overrides the <see cref="object.Equals"/> method.</para></remarks>
        public override bool Equals(Object obj)
        {
            return MatrixBase.Equals(this, obj as IMatrix);
        }

        /// <summary>Returns an integer valued hash function of the current matrix object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionInt"/> method.</para></summary>
        /// <seealso cref="Util.GetHashFunctionInt"/>
        public int GetHashFunctionInt()
        {
            return Util.GetHashFunctionInt(this);
        }

        /// <summary>Returns a string valued hash function of the current matrix object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionString"/> method.</para></summary>
        /// <remarks>The returned string is always on the same length, and is based on the <see cref="ToString"/> method.
        /// Therefore it is convenient for use in file or directory names that have one part related to a specific matrix.</remarks>
        /// <seealso cref="Util.GetHashFunctionString"/>
        public string GetHashFunctionString()
        {
            return Util.GetHashFunctionString(this);
        }

        #endregion Operations.Auxiliary



        #endregion Operations



        #region StaticOperations


        private static MatrixStore _matrixStore;

        /// <summary>Gets the matrix store for recycling auxiliary matrices.
        /// <para>Matrix store is created only once, on the first access.</para></summary>
        public static MatrixStore MatrixStore
        {
            get
            {
                if (_matrixStore == null)
                {
                    lock (Util.LockGlobal)
                    {
                        if (_matrixStore == null)
                            _matrixStore = new MatrixStore(0, 0, false);
                    }
                }
                return _matrixStore;
            }
        }


        #region SetValueStatic


        /// <summary>Sets all components of the specified matrix to 0.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetZero(IMatrix mat)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to zero is not specified (null argument).");
            for (int i = 0; i < mat.RowCount; ++i)
                for (int j = 0; j < mat.ColumnCount; ++j)
                    mat[i, j] = 0.0;
        }

        /// <summary>Sets all components of the specified matrix to the specified value.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="elementValue">Value to which elements are set.</param>
        public static void SetConstant(IMatrix mat, double elementValue)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to zero is not specified (null argument).");
            for (int i = 0; i < mat.RowCount; ++i)
                for (int j = 0; j < mat.ColumnCount; ++j)
                    mat[i, j] = elementValue;
        }

        /// <summary>Sets the specified matrix to identity matrix.
        /// WARNING: 
        /// Works only for square matrices (exception is thrown if matrix is not square). For nonsquare matrices, use SetDiagonal(1.0)!</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetIdentity(IMatrix mat)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to zero is not specified (null argument).");
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to identity not specified (null matrix).");
            if (mat.RowCount != mat.ColumnCount)
                throw new InvalidOperationException("Attempt to set a non-square matrix to identity matrix.");
            for (int i = 0; i < mat.RowCount; ++i)
                for (int j = 0; j < mat.ColumnCount; ++j)
                {
                    if (i == j)
                        mat[i, j] = 1.0;
                    else
                        mat[i, j] = 0.0;
                }
        }

        /// <summary>Sets the current matrix to the diagonal matrix with diagonal element specified by a vector.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="diagonal">Vector of diagonal elements.</param>
        public static void SetDiagonal(IMatrix mat, IVector diagonal)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to zero is not specified (null argument).");
            if (diagonal == null)
                throw new ArgumentNullException("Vector of diagonal matrix elements is not specified (null reference).");
            if (diagonal.Length < mat.RowCount && diagonal.Length < mat.ColumnCount || 
                diagonal.Length > mat.RowCount && diagonal.Length > mat.ColumnCount)
                throw new Exception("Vector of diagonal elements has incorrect dimension " + diagonal.Length
                    + " for a matrix with dimensions " + mat.RowCount + "x" + mat.ColumnCount + ".");
            for (int i = 0; i < mat.RowCount; ++i)
                for (int j = 0; j < mat.ColumnCount; ++j)
                {
                    if (i == j)
                        mat[i, j] = diagonal[i];
                    else
                        mat[i, j] = 0;
                }
        }

        /// <summary>Sets the current matrix to the diagonal matrix with all diagonal elements equal to the specified
        /// value.
        /// Matrix does not need to be a square matrix.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="diagonalElement">Value of diagonal elements.</param>
        public static void SetDiagonal(IMatrix mat, double diagonalElement)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to random matrix is not specified (null argument).");
            for (int i = 0; i < mat.RowCount; ++i)
                for (int j = 0; j < mat.ColumnCount; ++j)
                {
                    if (i == j)
                        mat[i, j] = diagonalElement;
                    else
                        mat[i, j] = 0.0;
                }
        }
        
        /// <summary>Sets the specified matrix such that it contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandom(IMatrix mat)
        {
            IRandomGenerator rnd = RandomGenerator.Global;  // new RandGeneratorThreadSafe(1111);
            SetRandom(mat, rnd);
        }

        /// <summary>Sets the current matrix such that it contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        public static void SetRandom(IMatrix mat, IRandomGenerator rnd)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to zero is not specified (null argument).");
            if (rnd == null)
                throw new ArgumentNullException("Random number generator for generation of components is not specified (null reference).");
            int d1 = mat.RowCount, d2 = mat.ColumnCount;
            double element;
            for (int i = 0; i < d1; ++i)
                for (int j = 0; j < d2; ++j)
                {
                    do
                    {
                        element = rnd.NextDouble();
                    } while (element == 0.0);
                    mat[i, j] = element;
                }
        }

        /// <summary>Sets the specified matrix such that it is symmetric and contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandomSymmetric(IMatrix mat)
        {
            IRandomGenerator rnd = RandomGenerator.Global; // new RandGeneratorThreadSafe(1111);
            SetRandomSymmetric(mat, rnd);
        }

        /// <summary>Sets the specified matrix such that it is symmetric and contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        public static void SetRandomSymmetric(IMatrix mat, IRandomGenerator rnd)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to symmetric random matrix is not specified (null argument).");
            if (rnd == null)
                throw new ArgumentNullException("Random number generator for generation of components is not specified (null reference).");
            int d1 = mat.RowCount, d2 = mat.ColumnCount;
            if (d1 != d2)
                throw new ArgumentException("Matrix to be set to random symmetric matrix is not square (dimensions: " 
                    + d1 + "x" + d2 + ")");
            double element;
            for (int i = 0; i < d1; ++i)
            {
                for (int j = 0; j <= i; ++j)
                {
                    do
                    {
                        element = rnd.NextDouble();
                    } while (element == 0.0);
                    mat[i, j] = element;
                    if (i != j)
                        mat[j, i] = element;
                }
            }
        }

        /// <summary>Sets the specified matrix such that it is antisymmetric and contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandomAntiSymmetric(IMatrix mat)
        {
            IRandomGenerator rnd = RandomGenerator.Global; // new RandGeneratorThreadSafe(1111);
            SetRandomAntiSymmetric(mat, rnd);
        }

        /// <summary>Sets the specified matrix such that it is antisymmetric and contains random elements on the interval (0,1].
        /// <para>Matrix will have zero elements on the diagonal.</para></summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        public static void SetRandomAntiSymmetric(IMatrix mat, IRandomGenerator rnd)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to antisymmetric random matrix is not specified (null argument).");
            if (rnd == null)
                throw new ArgumentNullException("Random number generator for generation of components is not specified (null reference).");
            int d1 = mat.RowCount, d2 = mat.ColumnCount;
            if (d1 != d2)
                throw new ArgumentException("Matrix to be set to random antisymmetric matrix is not square (dimensions: "
                    + d1 + "x" + d2 + ")");
            double element;
            for (int i = 0; i < d1; ++i)
            {
                mat[i, i] = 0.0;
                for (int j = 0; j < i; ++j)
                {
                    do
                    {
                        element = rnd.NextDouble();
                    } while (element == 0.0);
                    mat[i, j] = element;
                    mat[j, i] = -element;
                }
            }
        }

        /// <summary>Sets the specified matrix such that it is lower triangular and contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandomLowerTriangular(IMatrix mat)
        {
            IRandomGenerator rnd = RandomGenerator.Global; // new RandGeneratorThreadSafe(1111);
            SetRandomLowerTriangular(mat, rnd);
        }

        /// <summary>Sets the specified matrix such that it is lower triangular and contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        public static void SetRandomLowerTriangular(IMatrix mat, IRandomGenerator rnd)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to lower triangular random matrix is not specified (null argument).");
            if (rnd == null)
                throw new ArgumentNullException("Random number generator for generation of components is not specified (null reference).");
            int d1 = mat.RowCount, d2 = mat.ColumnCount;
            double element;
            for (int i = 0; i < d1; ++i)
            {
                for (int j = 0; j < d2; ++j)
                {
                    if (j > i)
                        mat[i, j] = 0.0;
                    else
                    {
                        do
                        {
                            element = rnd.NextDouble();
                        } while (element == 0.0);
                        mat[i, j] = element;
                    }
                }
            }
        }

        /// <summary>Sets the specified matrix such that it is upper triangular and contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandomUpperTriangular(IMatrix mat)
        {
            IRandomGenerator rnd = RandomGenerator.Global; // new RandGeneratorThreadSafe(1111);
            SetRandomUpperTriangular(mat, rnd);
        }

        /// <summary>Sets the specified matrix such that it is upper triangular and contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        public static void SetRandomUpperTriangular(IMatrix mat, IRandomGenerator rnd)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to lower triangular random matrix is not specified (null argument).");
            if (rnd == null)
                throw new ArgumentNullException("Random number generator for generation of components is not specified (null reference).");
            int d1 = mat.RowCount, d2 = mat.ColumnCount;
            double element;
            for (int i = 0; i < d1; ++i)
            {
                for (int j = 0; j < d2; ++j)
                {
                    if (j < i)
                        mat[i, j] = 0.0;
                    else
                    {
                        do
                        {
                            element = rnd.NextDouble();
                        } while (element == 0.0);
                        mat[i, j] = element;
                    }
                }
            }
        }

        /// <summary>Sets the specified matrix such that it is positive definite and contains random elements on the interval (0,1].</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandomSymmetricPositiveDefinite(IMatrix mat)
        {
            IRandomGenerator rnd = RandomGenerator.Global; // new RandGeneratorThreadSafe(1111);
            SetRandomSymmetricPositiveDefinite(mat, rnd);
        }

        /// <summary>Sets the specified matrix such that it is positive definite and contains random 
        /// elements.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        /// <remarks><para>This method is relativley slow because of multiplication of two matrices.
        /// For quicker method use <see cref="SetRandomPositiveDiagonallyDominantSymmetric"/>().</para>
        /// <para>Matrix is created in such a way that a random lower triangular matrix with positive diagonal elements
        /// is created first, then it is multiplied by its transpose.</para>
        /// <para>It seems that generation of positive definite matrices in this way is not stable when elements of the 
        /// lower triangular matrix are random on the interval [0,1). For this reason, 
        /// 1 is added to all diagonal elements of the lower triangular matrix.</para></remarks>
        public static void SetRandomSymmetricPositiveDefinite(IMatrix mat, IRandomGenerator rnd)
        {
            if (mat == null)
                throw new ArgumentException("Matrix to be set to random positive definite matrix is not specified (null argument).");
            int d1 = mat.RowCount, d2 = mat.ColumnCount;
            if (d1 != d2)
                throw new ArgumentException("Matrix to be set to random positive definite matrix is not square (dimensions: "
                    + d1 + "x" + d2 + ")");
            IMatrix L = null, LT = null;
            // Try to get auxiliary matrices from the matrix store:
            L = MatrixStore.TryGet();
            LT = MatrixStore.TryGet();
            if (L == LT && LT!=null)
                throw new ArgumentException("Matrix L and LT are the same.");
            // Resize if necessary:
            Resize(ref L, d1, d1);
            Resize(ref LT, d1, d1);
            // Set lower triangular matrix and its transpose:
            SetRandomLowerTriangular(L, rnd);
            for (int i = 0; i < d1; ++i) // ensure that diagonal elements are positive:
            {
                 L[i, i] += 1.0;
            }
            Transpose(L, ref LT);
            // Set matrix as product of lower and upper triangular matrix:
            Multiply(L, LT, mat);
            // Try to put auxiliary matrixes back to matrix store:
            MatrixStore.TryStore(L as Matrix);
            MatrixStore.TryStore(LT as Matrix);
        }


        /// <summary>Sets the specified matrix such that it is has random elements and is diagonally dominant with positive diagonal
        /// elements, i.e. any diagonal element is greater by absolute value than sum of absolute values of nondiagonal 
        /// elements in the corresponding column.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandomPositiveDiagonallyDominant(IMatrix mat, double dominancyFactor)
        {
            IRandomGenerator rnd = RandomGenerator.Global; 
            SetRandomPositiveDiagonallyDominant(mat, rnd, dominancyFactor);
        }

        /// <summary>Sets the specified matrix such that it is has random elements and is diagonally dominant with positive diagonal 
        /// elements, i.e. any diagonal element is greater by absolute value than sum of absolute values of nondiagonal 
        /// elements in the corresponding column.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        /// <param name="dominancyFactor">Factor such that any diagonal element is by absolute value at least by this
        /// factor greater than the sum of absolute values of nondiagonal elements in the corresponding column.</param>
        public static void SetRandomPositiveDiagonallyDominant(IMatrix mat, IRandomGenerator rnd, double dominancyFactor)
        {
            if (mat == null)
                throw new ArgumentNullException("Matrix to be set to lower triangular random matrix is not specified (null argument).");
            if (rnd == null)
                throw new ArgumentNullException("Random number generator for generation of components is not specified (null reference).");
            int d1 = mat.RowCount, d2 = mat.ColumnCount;
            double element;
            if (dominancyFactor < 0)
                dominancyFactor = -dominancyFactor;
            else if (dominancyFactor == 0)
                dominancyFactor = 1.0;
            if (dominancyFactor < 1.0)
                dominancyFactor = 1.0 / dominancyFactor;
            double outDiagonalFactor = 0.5 * (1.0 / ((double)Math.Max(d1, d2) * dominancyFactor));
            for (int i = 0; i < d1; ++i)
            {
                for (int j = 0; j < d2; ++j)
                {
                    if (i == j)
                    {
                        element = rnd.NextDouble();
                        if (element < 0.5)
                            element = 1.0 - element;
                        mat[i, i] = element;
                    } else
                    {
                        mat[i, j] = rnd.NextDouble() * outDiagonalFactor;
                        mat[j, i] = rnd.NextDouble() * outDiagonalFactor;
                    }
                }
            }
        }

        
        /// <summary>Sets the specified matrix such that it is has random elements and is diagonally dominant with positive diagonal
        /// elements, i.e. any diagonal element is greater by absolute value than sum of absolute values of nondiagonal 
        /// elements in the corresponding column.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandomPositiveDiagonallyDominant(IMatrix mat)
        {
            IRandomGenerator rnd = RandomGenerator.Global; 
            SetRandomPositiveDiagonallyDominant(mat, rnd, 100 /* dominancyFactor */);
        }

        /// <summary>Sets the specified matrix such that it is has random elements and is diagonally dominant with positive diagonal 
        /// elements, i.e. any diagonal element is greater by absolute value than sum of absolute values of nondiagonal 
        /// elements in the corresponding column.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        /// <param name="dominancyFactor">The average ratio between absolute value of any diagonal term and the sum of absolute values of 
        /// out of diagonal terms in the same column. Should be greater than 1.</param>
        public static void SetRandomPositiveDiagonallyDominant(IMatrix mat, IRandomGenerator rnd)
        {
            SetRandomPositiveDiagonallyDominant(mat, rnd, 100 /* dominancyFactor */ );
        }


        /// <summary>Sets the specified matrix such that it is has random elements and is symmetric diagonally dominant with positive diagonal
        /// elements, i.e. any diagonal element is greater by absolute value than sum of absolute values of nondiagonal 
        /// elements in the corresponding column.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <dominancyFactor>The average ratio between absolute value of any diagonal term and the sum of absolute values of 
        /// out of diagonal terms in the same column. Should be greater than 1.</dominancyFactor>
        public static void SetRandomPositiveDiagonallyDominantSymmetric(IMatrix mat, double dominancyFactor)
        {
            IRandomGenerator rnd = RandomGenerator.Global;
            SetRandomPositiveDiagonallyDominantSymmetric(mat, rnd, dominancyFactor);
        }

        /// <summary>Sets the specified matrix such that it is has random elements and is symmetric diagonally dominant with positive diagonal 
        /// elements, i.e. any diagonal element is greater by absolute value than sum of absolute values of nondiagonal 
        /// elements in the corresponding column.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        /// <param name="dominancyFactor">Factor such that any diagonal element is by absolute value at least by this
        /// factor greater than the sum of absolute values of nondiagonal elements in the corresponding column.</param>
        /// <dominancyFactor>The average ratio between absolute value of any diagonal term and the sum of absolute values of 
        /// out of diagonal terms in the same column. Should be greater than 1.</dominancyFactor>
        public static void SetRandomPositiveDiagonallyDominantSymmetric(IMatrix mat, IRandomGenerator rnd, double dominancyFactor)
        {
            SetRandomPositiveDiagonallyDominant(mat, rnd, dominancyFactor);
            SymmetricPart(mat, mat);
        }


        /// <summary>Sets the specified matrix such that it is has random elements and is symmetric diagonally dominant with positive diagonal
        /// elements, i.e. any diagonal element is greater by absolute value than sum of absolute values of nondiagonal 
        /// elements in the corresponding column.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        public static void SetRandomPositiveDiagonallyDominantSymmetric(IMatrix mat)
        {
            IRandomGenerator rnd = RandomGenerator.Global;
            SetRandomPositiveDiagonallyDominantSymmetric(mat, rnd, 100 /* dominancyFactor */);
        }

        /// <summary>Sets the specified matrix such that it is has random elements and is symmetric diagonally dominant with positive diagonal 
        /// elements, i.e. any diagonal element is greater by absolute value than sum of absolute values of nondiagonal 
        /// elements in the corresponding column.</summary>
        /// <param name="mat">Matrix whose components are set.</param>
        /// <param name="rnd">Random generator used to generate matrix elements.</param>
        /// <param name="dominancyFactor">Factor such that any diagonal element is by absolute value at least by this
        /// factor greater than the sum of absolute values of nondiagonal elements in the corresponding column.</param>
        public static void SetRandomPositiveDiagonallyDominantSymmetric(IMatrix mat, IRandomGenerator rnd)
        {
            SetRandomPositiveDiagonallyDominantSymmetric(mat, rnd, 100 /* dominancyFactor */ );
        }


        #endregion SetValueStatic


        #region QueryTypeStatic

        /// <summary>Returns true if the specified matrix is a square matrix, and false if not.
        /// If the matrix is null then false is returned.</summary>
        /// <param name="mat">Matrix that is tested for being square.</param>
        public static bool IsSquare(IMatrix mat)
        {
            if (mat != null)
                if (mat.RowCount == mat.ColumnCount)
                    return true;
            return false;
        }

        /// <summary>Returns true if the specified matrix is symmetric, and false if not.
        /// If the matrix is null then false is returned.</summary>
        /// <param name="mat">Matrix that is tested for being symmetric.</param>
        public static bool IsSymmetric(IMatrix mat)
        {
            if (IsSquare(mat))
            {
                for (int i = 0; i<mat.RowCount; ++i)
                    for (int j = 0; j < i; ++j)
                    {
                        if (mat[i,j]!=mat[j,i])
                            return false;
                    }
                return true;
            }
            return false;
        }

        /// <summary>Returns true if the specified matrix is symmetric within some tolerance, and false if not.
        /// If the matrix is null then false is returned.</summary>
        /// <param name="mat">Matrix that is tested for being symmetric.</param>
        /// <param name="relativeRMSTolerance">Tolerance on the ratio between RMS of differences between out of diagonal terms
        /// and their transposes and between RMS of out of diagonal terms, below which matrix is considered symmetric.</param>
        public static bool IsSymmetric(IMatrix mat, double relativeRMSTolerance)
        {
            double smallValue = 1.0e-60;
            if (IsSquare(mat))
            {
                if (relativeRMSTolerance<0)
                    relativeRMSTolerance = - relativeRMSTolerance;
                double sumSquares = 0.0, sumSquareDifs = 0.0;
                for (int i = 0; i < mat.RowCount; ++i)
                    for (int j = 0; j < i; ++j)
                    {
                        double m_ij = mat[i, j];
                        double m_ji = mat[j, i];
                        sumSquares += m_ij*m_ij + m_ji*m_ji;
                        sumSquareDifs += 2.0 * (m_ij - m_ji) * (m_ij - m_ji);
                    }
                if (Math.Sqrt(sumSquareDifs / (sumSquares + smallValue)) < relativeRMSTolerance)
                    return true;
            }
            return false;
        }

        #endregion QueryTypeStatic


        /// <summary>Compares two matrices and returns -1 if the first matrix is smaller than the second one,
        /// 0 if matrices are equal, and 1 if the first matrix is greater.
        /// Matrix that is null is considered smaller than a matrix that is not null. Two null matrices are considered equal.
        /// Matrix with smaller dimension is considered smaller than a matrix with greater dimension.
        /// Matrices with equal dimensions ar compared by elements. The first element that is different decides
        /// which matrix is considered greater.</summary>
        /// <param name="m1">First matrix to be compared.</param>
        /// <param name="m2">Second matrix to be compared.</param>
        /// <returns>-1 if the first matrix is smaller than the second one, 0 if matrices are equal, and 1 if the second is greater.</returns>
        /// <remarks>This comparison does not have any mathematical meaning, it is just used for sotting of matrices in data structures.</remarks>
        public static int Compare(IMatrix m1, IMatrix m2)
        {
            if (m1 == null)
            {
                if (m2 == null)
                    return 0;
                else
                    return -1;
            }
            else if (m2 == null)
            {
                return 1;
            }
            else if (m1.RowCount < m2.RowCount)
                return -1;
            else if (m1.RowCount > m2.RowCount)
                return 1;
            else if (m1.ColumnCount < m2.ColumnCount)
                return -1;
            else if (m1.ColumnCount > m2.ColumnCount)
                return 1;
            else
            {
                int dim1 = m1.RowCount;
                int dim2 = m1.ColumnCount;
                for (int i = 0; i < dim1; ++i)
                    for (int j=0; j<dim2; ++j)
                    {
                        double el1 = m1[i,j];
                        double el2 = m2[i,j];
                        if (el1 < el2)
                            return -1;
                        else if (el1 > el2)
                            return 1;
                    }
                return 0;  // all elements are equal
            }
        }



        /// <summary>Resizes, if necessary, the specified matrix according to the required dimensions.
        /// If the matrix is initially null then a new matrix is created. If in this case a template matrix is specified
        /// then the newly created matrix will be of the same type as that template matrix, because it is created by 
        /// the GetNew() method on that matrix.
        /// If dimensions of the initial matrix do not match the required dim., then matrix is resized. 
        /// If the specified matrix dimension is less or equal to 0 then matrix is resized with the same dimensions as 
        /// those of the template matirx. If in this case the template matrix is null, an exception is thrown.
        /// WARNINGS:
        /// Components are NOT preserved and have in general undefined values after operation is performed.
        /// If matrix and template are both null then the type of nawly created matrix is Matrix.</summary>
        /// <param name="mat">Matrix that is resized.</param>
        /// <param name = "template">Matrix that is taken as template (for type of a newly created matrix or for dimensions if
        /// they are not specified).</param>
        /// <param name="rowCount">If greater than 0 then it specifies the number of rows to which matrix is resized.</param>
        /// <param name="columnCount">If greater than 0 then it specifies the number of columns to which matrix is resized.</param>
        public static void Resize(ref IMatrix mat, IMatrix template, int rowCount, int columnCount)
        {
            if (rowCount <= 0 || columnCount <=0)
            {
                if (template == null)
                    throw new ArgumentNullException("Matrix dimensions after resize are not specified (arguments less or equal to 0) and template is not specified.");
                else
                {
                    rowCount = template.RowCount;
                    columnCount = template.ColumnCount;
                }
            }
            if (mat == null)
            {
                if (template != null)
                    mat = template.GetNew(rowCount, columnCount);
                else
                    mat = new Matrix(rowCount, columnCount);
            }
            else if (mat.RowCount != rowCount || mat.ColumnCount!=columnCount)
            {
                mat = mat.GetNew(rowCount, columnCount);
            }
        }

        /// <summary>Resizes, if necessary, the specified matrix according to the required dimensions.
        /// If the matrix is initially null then a new matrix (of type Matrix) is created.
        /// If dimensions of the initial matrix do not match the required dimensions, then matrix is resized. 
        /// Components are NOT preserved and have in general undefined values after operation is performed.
        /// WARNING: 
        /// If the matrix is initially null then the type of the newly created matrix is Matrix.</summary>
        /// <param name="mat">VMatrix that is resized.</param>
        /// <param name="rowCount">Dimension to which matrix is resized (if less than 1 then exception is thrown).</param>
        /// <param name="columnCount">Dimension to which matrix is resized (if less than 1 then exception is thrown).</param>
        public static void Resize(ref IMatrix mat, int rowCount, int columnCount)
        {
            if (rowCount <= 0 || columnCount <=0)
                throw new ArgumentNullException("Matrix dimensions after resize are not specified (argument less or equal to 0).");
            if (mat == null)
                mat = new Matrix(rowCount, columnCount);
            else if (mat.RowCount!=rowCount || mat.ColumnCount!=columnCount)
                mat = mat.GetNew(rowCount, columnCount);
        }


        /// <summary>Resizes, if necessary, the specified matrix according to the dimensions of the specified template matrix.
        /// If the matrix is initially null then a new matrix is created. In this case the newly created matrix will 
        /// be of the same type as that template matrix, because it is created by  the GetNew() method on that matrix.
        /// If dimensions of the initial matrix do not match the dimensions of the template matrix, then matrix is resized. 
        /// If the template matrix is null, then an exception is thrown.
        /// WARNINGS:
        /// Components are NOT preserved and have in general undefined values after operation is performed.
        /// If matrix and template are null then the type of newly created matrix is Matrix.</summary>
        /// <param name="mat">Matrix that is resized.</param>
        /// <param name = "template">Matrix that is taken as template (for type of a newly created matrix or for dimensions).</param>
        public static void Resize(ref IMatrix mat, IMatrix template)
        {
            int rowCount, columnCount;
            if (template == null)
                throw new ArgumentNullException("Template matrix is not specified in vector resize.");
            else
            {
                rowCount = template.RowCount;
                columnCount = template.ColumnCount;
            }
            if (mat == null)
            {
                mat = template.GetNew(rowCount, columnCount);
            } else if (mat.RowCount != rowCount || mat.ColumnCount!=columnCount)
            {
                mat = mat.GetNew(rowCount, columnCount);
            }
        }


        /// <summary>Copies components of a matrix to another matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the copied matrix and result storage must match.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void CopyPlain(IMatrix a, IMatrix result)
        {
            int d1 = a.RowCount;
            int d2 = a.ColumnCount;
            int i, j;
            for (i = 0; i < d1; ++i)
                for (j = 0; j < d2; ++j)
                    result[i, j] = a[i, j];
        }

        /// <summary>Copies components of a matrix to another matrix.
        /// WARNING: dimensions of the copied matrix and result storage must match.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void Copy(IMatrix a, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix to be copied is not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result matrix for copy operation is not specified (null reference).");
            else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                throw new ArgumentException("Dimension mismatch at copying matrices." + Environment.NewLine
                    + "  Original matrix: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int d1 = a.RowCount;
                int d2 = a.ColumnCount;
                int i, j;
                for (i = 0; i < d1; ++i)
                    for (j = 0; j < d2; ++j)
                        result[i, j] = a[i, j];
            }
        }

        /// <summary>Copies components of a matrix to another matrix.
        /// Resulting matrix is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy is stored.</param>
        public static void Copy(IMatrix a, ref IMatrix result)
        {
            if (a == null)
                result = null;
            else
            {
                int d1 = a.RowCount;
                int d2 = a.ColumnCount;
                if (result == null)
                    result = a.GetNew(d1, d2);
                else if (d1 != result.RowCount || d2 != result.ColumnCount)
                    result = a.GetNew(d1, d2);
                int i, j;
                for (i = 0; i < d1; ++i)
                    for (j = 0; j < d2; ++j)
                        result[i,j] = a[i, j];
            }
        }


        /// <summary>Copies components of a matrix to another matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the copied matrix and result storage must match.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void CopyPlain(MatrixBase_MathNetNumerics a, IMatrix result)
        {
            int d1 = a.RowCount;
            int d2 = a.ColumnCount;
            int i, j;
            for (i = 0; i < d1; ++i)
                for (j = 0; j < d2; ++j)
                    result[i, j] = a[i, j];
        }

        /// <summary>Copies components of a matrix to another matrix.
        /// WARNING: dimensions of the copied matrix and result storage must match.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void Copy(MatrixBase_MathNetNumerics a, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix to be copied is not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result matrix for copy operation is not specified (null reference).");
            else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                throw new ArgumentException("Dimension mismatch at copying matrices." + Environment.NewLine
                    + "  Original matrix: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int d1 = a.RowCount;
                int d2 = a.ColumnCount;
                int i, j;
                for (i = 0; i < d1; ++i)
                    for (j = 0; j < d2; ++j)
                        result[i, j] = a[i, j];
            }
        }

        /// <summary>Copies components of a matrix to another matrix.
        /// Resulting matrix is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy is stored.</param>
        public static void Copy(MatrixBase_MathNetNumerics a, ref IMatrix result)
        {
            if (a == null)
                result = null;
            else
            {
                int d1 = a.RowCount;
                int d2 = a.ColumnCount;
                if (result == null)
                    result = new Matrix(d1, d2);
                else if (d1 != result.RowCount || d2 != result.ColumnCount)
                    result = new Matrix(d1, d2);
                int i, j;
                for (i = 0; i < d1; ++i)
                    for (j = 0; j < d2; ++j)
                        result[i,j] = a[i, j];
            }
        }


        /// <summary>Copies components of a matrix to another matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the copied matrix and result storage must match.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void CopyPlain(IMatrix a, Matrix_MathNetNumercs result)
        {
            int d1 = a.RowCount;
            int d2 = a.ColumnCount;
            int i, j;
            for (i = 0; i < d1; ++i)
                for (j = 0; j < d2; ++j)
                    result[i, j] = a[i, j];
        }

        /// <summary>Copies components of a matrix to another matrix.
        /// WARNING: dimensions of the copied matrix and result storage must match.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy will be stored. Dimensions must match dimensions of original.</param>
        public static void Copy(IMatrix a, Matrix_MathNetNumercs result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix to be copied is not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result matrix for copy operation is not specified (null reference).");
            else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                throw new ArgumentException("Dimension mismatch at copying matrices." + Environment.NewLine
                    + "  Original matrix: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int d1 = a.RowCount;
                int d2 = a.ColumnCount;
                int i, j;
                for (i = 0; i < d1; ++i)
                    for (j = 0; j < d2; ++j)
                        result[i, j] = a[i, j];
            }
        }

        /// <summary>Copies components of a matrix to another matrix.
        /// Resulting matrix is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where copy is stored.</param>
        public static void Copy(IMatrix a, ref Matrix_MathNetNumercs result)
        {
            if (a == null)
                result = null;
            else
            {
                int d1 = a.RowCount;
                int d2 = a.ColumnCount;
                if (result == null)
                    result = new Matrix_MathNetNumercs(d1, d2);
                else if (d1 != result.RowCount || d2 != result.ColumnCount)
                    result = new Matrix_MathNetNumercs(d1, d2);
                int i, j;
                for (i = 0; i < d1; ++i)
                    for (j = 0; j < d2; ++j)
                        result[i, j] = a[i, j];
            }
        }


        /// <summary>Stores a negative matrix of the operand in another matrix.
        /// Can be done in-place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the operand and result storage must match.</summary>
        /// <param name="a">Operand.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operand.</param>
        public static void NegatePlain(IMatrix a, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = -a[i, j];
        }

        /// <summary>Stores a negative matrix of the operand in another matrix.
        /// Can be done in-place.
        /// WARNING: dimensions of the operand and result storage must match.</summary>
        /// <param name="a">Operand.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operand.</param>
        public static void Negate(IMatrix a, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix to be negated is not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result matrix for negation is not specified (null reference).");
            else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix negation." + Environment.NewLine
                    + "  Operand: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int i, j;
                for (i = 0; i < a.RowCount; ++i)
                    for (j = 0; j < a.ColumnCount; ++j)
                        result[i, j] = -a[i, j];
            }
        }

        /// <summary>Stores a negative matrix of the operand in another matrix.
        /// Can be done in-place.
        /// Resulting matrix is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where result of negation is be stored.</param>
        public static void Negate(IMatrix a, ref IMatrix result)
        {
            if (a == null)
                result = null;
            else
            {
                if (result == null)
                    result = a.GetNew(a.RowCount, a.ColumnCount);
                else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                    result = a.GetNew(a.RowCount, a.ColumnCount);
                int i, j;
                for (i = 0; i < a.RowCount; ++i)
                    for (j = 0; j < a.ColumnCount; ++j)
                        result[i, j] = -a[i, j];
            }
        }



        /// <summary>Stores transpose of the operand in another matrix.
        /// Can be done in-place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the operand and result storage must match.</summary>
        /// <param name="a">Operand.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operand.</param>
        public static void TransposePlain(IMatrix a, IMatrix result)
        {
            if (a == result)
            {
                // Perform transposition in-place:
                if (a.RowCount != a.ColumnCount)
                {
                    throw new ArgumentException("Matrix transpose can not be performed in-place for non-square matrices. "
                        + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
                }
                int i, j;
                double element;
                for (i = 0; i < result.RowCount; ++i)
                {
                    result[i, i] = a[i, i];
                    for (j = 0; j < i; ++j)
                    {
                        element = a[i, j];
                        a[i, j] = a[j, i];
                        a[j, i] = element;
                    }
                }
            } else
            {
                int i, j;
                for (i = 0; i < result.RowCount; ++i)
                    for (j = 0; j < result.ColumnCount; ++j)
                    {
                        result[i, j] = a[j, i];
                    }
            }
        }

        /// <summary>Stores transpose of the operand in another matrix.
        /// Can be done in-place.
        /// WARNING: dimensions of the operand and result storage must match.</summary>
        /// <param name="a">Operand.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operand.</param>
        public static void Transpose(IMatrix a, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix to be transposed is not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result matrix for transposition is not specified (null reference).");
            else if (a.RowCount != result.ColumnCount || a.ColumnCount != result.RowCount)
                throw new ArgumentException("Dimension mismatch at matrix transpose." + Environment.NewLine
                    + "  Operand: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                if (a == result)
                {
                    // Perform transposition in-place:
                    if (a.RowCount != a.ColumnCount)
                    {
                        throw new ArgumentException("Matrix transpose can not be performed in-place for non-square matrices. "
                            + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
                    }
                    int i, j;
                    double element;
                    for (i = 0; i < result.RowCount; ++i)
                    {
                        result[i, i] = a[i, i];
                        for (j = 0; j < i; ++j)
                        {
                            element = a[i, j];
                            a[i, j] = a[j, i];
                            a[j, i] = element;
                        }
                    }
                }
                else
                {
                    int i, j;
                    for (i = 0; i < result.RowCount; ++i)
                        for (j = 0; j < result.ColumnCount; ++j)
                        {
                            result[i, j] = a[j, i];
                        }
                }
            }
        }

        /// <summary>Stores transpose of the matrix operand in another matrix.
        /// Can be done in-place.
        /// Resulting matrix is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where result of negation is be stored.</param>
        public static void Transpose(IMatrix a, ref IMatrix result)
        {
            if (a == null)
                result = null;
            else
            {
                if (result == null)
                    result = a.GetNew(a.ColumnCount, a.RowCount);
                else if (a.RowCount != result.ColumnCount || a.ColumnCount != result.RowCount)
                    result = a.GetNew(a.ColumnCount, a.RowCount);
                if (a == result)
                {
                    // Perform transposition in-place:
                    if (a.RowCount != a.ColumnCount)
                    {
                        throw new ArgumentException("Matrix transpose can not be performed in-place for non-square matrices. "
                            + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
                    }
                    int i, j;
                    double element;
                    for (i = 0; i < result.RowCount; ++i)
                    {
                        result[i, i] = a[i, i];
                        for (j = 0; j < i; ++j)
                        {
                            element = a[i, j];
                            a[i, j] = a[j, i];
                            a[j, i] = element;
                        }
                    }
                }
                else
                {
                    int i, j;
                    for (i = 0; i < result.RowCount; ++i)
                        for (j = 0; j < result.ColumnCount; ++j)
                        {
                            result[i, j] = a[j, i];
                        }
                }
            }
        }


        /// <summary>Calculates symmetric and antisymmetric part of the specified matrix (symmetric part 
        /// is stored in the first result argument but is calculated last, such that it overrides antisymmetric 
        /// when both arguments point to the same matrix object).
        /// <para>Can be done in-place.</para>
        /// <para>When both symmetric and antisymmetric result arguments point to the same matrix, symmetric
        /// part is stored in the matrix.</para>
        /// <para>This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the operand and result storage must match.</para>
        /// <para>WARNING: If this <paramref name="antiSym"/> points to the same matrix object as <paramref name="sym"/> then symmetric part is
        /// stored in this matrix (because it overrides antisymmetric part).</para></summary>
        /// <param name="a">Matrix whose symmetric and antisymmetric part are calculated.</param>
        /// <param name="sym">Matrix where symmetric part of <paramref name="a"/> is stored.</param>
        /// <param name="antiSym">Matrix where antisymmetric part of <paramref name="a"/> is stored.
        /// <para>WARNING: If this argument points to the same matrix object as <paramref name="sym"/> then symmetric part is 
        /// stored in this matrix (because it overrides antisymmetric part).</para>
        /// </param>
        public static void SymmetrizePlain(IMatrix a, IMatrix sym, IMatrix antiSym)
        {
            int dim = a.RowCount;
            for (int i = 0; i < dim; ++i)
            {
                // Diagonal elements:
                antiSym[i, i] = 0;
                sym[i, i] = a[i, i];
                double elAntiSym, elSym;
                for (int j = 0; j < i; ++j)
                {
                    elAntiSym = 0.5 * (a[i, j] - a[j, i]);
                    elSym = 0.5 * (a[i, j] + a[j, i]);
                    antiSym[i, j] = elAntiSym;
                    antiSym[j, i] = -elAntiSym;
                    sym[i, j] = elSym;
                    sym[j, i] = elSym;
                }
            }
        }  // SymmetrizePlain(...)

        /// <summary>Calculates symmetric and antisymmetric part of the specified matrix (symmetric part 
        /// is stored in the first result argument but is calculated last, such that it overrides antisymmetric 
        /// when both arguments point to the same matrix object).
        /// <para>Can be done in-place.</para>
        /// <para>When both symmetric and antisymmetric result arguments point to the same matrix, exception is thrown.</para>
        /// <para>Dimensions of the operand and result storage must match, otherwise exception is thrown.</para></summary>
        /// <param name="a">Matrix whose symmetric and antisymmetric part are calculated.</param>
        /// <param name="sym">Matrix where symmetric part of <paramref name="a"/> is stored.</param>
        /// <param name="antiSym">Matrix where antisymmetric part of <paramref name="a"/> is stored.
        /// <para>WARNING: If this argument points to the same matrix object as <paramref name="sym"/> then exception is thrown.</para>
        /// </param>
        public static void Symmetrize(IMatrix a, IMatrix sym, IMatrix antiSym)
        {
            if (a == null)
                throw new ArgumentException("Matrix to be symmetrized is null.");
            if (sym == null)
                throw new ArgumentException("Symmetric result matrix is null.");
            else if (antiSym == null)
            {
                antiSym = sym;
            }
            if (sym == antiSym)
                throw new ArgumentException("Argument to store symmetric part points to the same matrix object as argument to store antisymmetric part.");
            if (a.RowCount != a.ColumnCount)
                throw new ArgumentException("Symmetrization can not be done for non-square matrices. "
                        + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
            if (sym.RowCount != a.RowCount || sym.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimensinos of symmetric result matrix do not match dimensions of the original.");
            if (antiSym.RowCount != a.RowCount || antiSym.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimensinos of antisymmetric result matrix do not match dimensions of the original.");
            int dim = a.RowCount;
            for (int i = 0; i < dim; ++i)
            {
                // Diagonal elements:
                antiSym[i, i] = 0;
                sym[i, i] = a[i, i];
                double elAntiSym, elSym;
                for (int j = 0; j < i; ++j)
                {
                    elAntiSym = 0.5 * (a[i, j] - a[j, i]);
                    elSym = 0.5 * (a[i, j] + a[j, i]);
                    antiSym[i, j] = elAntiSym;
                    antiSym[j, i] = -elAntiSym;
                    sym[i, j] = elSym;
                    sym[j, i] = elSym;
                }
            }
        }  // SymmetrizePlain(...)

        /// <summary>Stores transpose of the matrix operand in another matrix.
        /// Can be done in-place.
        /// Resulting matrix is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original matrix.</param>
        /// <param name="result">Matrix where result of negation is be stored.</param>
        public static void Symmetrize(IMatrix a, ref IMatrix sym, ref IMatrix antiSym)
        {
            if (a == null)
                throw new ArgumentException("Matrix to be symmetrized is null.");
            if (a.RowCount != a.ColumnCount)
                throw new ArgumentException("Symmetrization can not be done for non-square matrices. "
                        + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
            int dim = a.RowCount;
            bool allocSym = false, allocAntiSym = false;
            if (sym == null)
                allocSym = true;
            else if (sym.RowCount!=dim || sym.ColumnCount != dim)
                allocSym = true;
            if (antiSym == null)
                allocAntiSym = true;
            else
            {
                if (antiSym == sym)
                {
                    allocAntiSym = true;
                    antiSym = null;
                    if (sym == null)
                    {
                        // Not only that symmetric and antisymmetric storage variables point to the same matrix, but
                        // references to the same variables are passed via these arguments. Both parts can therefore
                        // not be decoupled by allocation, therefore exception is thrown.
                        throw new ArgumentException("The same variable was passed as argument for storing both symmetric and antisymmetric part of the matrix.");
                    }
                }
                if (antiSym.RowCount != dim || antiSym.ColumnCount != dim)
                    allocAntiSym = true;
            }
            if (allocSym)
                sym = a.GetNew();
            if (allocAntiSym)
                antiSym = a.GetNew();
            for (int i = 0; i < dim; ++i)
            {
                // Diagonal elements:
                antiSym[i, i] = 0;
                sym[i, i] = a[i, i];
                double elAntiSym, elSym;
                for (int j = 0; j < i; ++j)
                {
                    elAntiSym = 0.5 * (a[i, j] - a[j, i]);
                    elSym = 0.5 * (a[i, j] + a[j, i]);
                    antiSym[i, j] = elAntiSym;
                    antiSym[j, i] = -elAntiSym;
                    sym[i, j] = elSym;
                    sym[j, i] = elSym;
                }
            }
        }  // SymmetrizePlain(...)



        /// <summary>Stores symmetric part of a square matrix operand in another matrix.
        /// Symmetrization is performed by averaging of non-diagonal terms and their transposed terms.
        /// Can be done in-place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the operand and result storage must match.</summary>
        /// <param name="a">Operand, must be a square matrix.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operand.</param>
        public static void SymmetricPartPlain(IMatrix a, IMatrix result)
        {
            // Perform symmetrization in-place:
            if (a.RowCount != a.ColumnCount)
            {
                throw new ArgumentException("Matrix symmetrization can not be performed for non-square matrices. "
                    + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
            }
            int i, j;
            double element;
            for (i = 0; i < result.RowCount; ++i)
            {
                result[i, i] = a[i, i];
                for (j = 0; j < i; ++j)
                {
                    element = 0.5*(a[i, j]+a[j,i]);
                    result[i, j] = element;
                    result[j, i] = element;
                }
            }
        }  // SymmetricPartPlain

        /// <summary>Stores symmetric part of a square matrix operand in another matrix.
        /// Symmetrization is performed by averaging of non-diagonal terms and their transposed terms.
        /// Can be done in-place.
        /// WARNING: dimensions of the operand and result storage must match.</summary>
        /// <param name="a">Operand, must be a square matrix.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operand.</param>
        public static void SymmetricPart(IMatrix a, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix to be symmetrized is not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result matrix for symmetrization is not specified (null reference).");
            else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix symmetrization." + Environment.NewLine
                    + "  Operand: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else if (a.RowCount != a.ColumnCount)
            {
                throw new ArgumentException("Matrix symmetrization can not be performed in-place for non-square matrices. "
                    + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
            }
            int i, j;
            double element;
            for (i = 0; i < result.RowCount; ++i)
            {
                result[i, i] = a[i, i];
                for (j = 0; j < i; ++j)
                {
                    element = 0.5 * (a[i, j] + a[j, i]);
                    result[i, j] = element;
                    result[j, i] = element;
                }
            }
        }  // SymmetricPart

        /// <summary>Stores symmetric part of a square matrix operand in another matrix.
        /// Symmetrization is performed by averaging of non-diagonal terms and their transposed terms.
        /// Can be done in-place.
        /// Resulting matrix is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original matrix, must be a square matrix.</param>
        /// <param name="result">Matrix where result of negation is be stored.</param>
        public static void SymmetricPart(IMatrix a, ref IMatrix result)
        {
            if (a == null)
                result = null;
            else if (a.RowCount != a.ColumnCount)
            {
                throw new ArgumentException("Matrix symmetrization can not be performed in-place for non-square matrices. "
                    + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
            } else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            double element;
            for (i = 0; i < result.RowCount; ++i)
            {
                result[i, i] = a[i, i];
                for (j = 0; j < i; ++j)
                {
                    element = 0.5 * (a[i, j] + a[j, i]);
                    result[i, j] = element;
                    result[j, i] = element;
                }
            }
        }  // SymmetricPart

        /// <summary>Stores antisymmetric part of a square matrix operand in another matrix.
        /// Antisymmetrization is performed by subtracting non-diagonal terms and their transposed terms,
        /// division by 2 and storing result in one matrix element and its negative value in another element.
        /// Can be done in-place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of the operand and result storage must match.</summary>
        /// <param name="a">Operand; must be a square matrix.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operand.</param>
        public static void AntisymmetricPartPlain(IMatrix a, IMatrix result)
        {
            // Perform antisymmetrization in-place:
            int i, j, d1 = a.RowCount, d2 = a.ColumnCount;
            double element;
            if (d1 != d2)
            {
                throw new ArgumentException("Matrix antisymmetrization can not be performed for non-square matrices. "
                    + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
            }
            for (i = 0; i < d1; ++i)
            {
                result[i, i] = 0;
                for (j = 0; j < i; ++j)
                {
                    element = 0.5*(a[i, j]-a[j,i]);
                    result[i, j] = element;
                    result[j, i] = -element;
                }
            }
        }  // SymmetricPartPlain

        /// <summary>Stores antisymmetric part of a square matrix operand in another matrix.
        /// Antisymmetrization is performed by subtracting non-diagonal terms and their transposed terms,
        /// division by 2 and storing result in one matrix element and its negative value in another element.
        /// Can be done in-place.
        /// WARNING: dimensions of the operand and result storage must match.</summary>
        /// <param name="a">Operand, must be a square matrix.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operand.</param>
        public static void AntisymmetricPart(IMatrix a, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix to be antisymmetrized is not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result matrix for symmetrization is not specified (null reference).");
            else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix symmetrization." + Environment.NewLine
                    + "  Operand: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            // Perform antisymmetrization in-place:
            int i, j, d1 = a.RowCount, d2 = a.ColumnCount;
            double element;
            if (d1 != d2)
            {
                throw new ArgumentException("Matrix antisymmetrization can not be performed for non-square matrices. "
                    + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
            }
            for (i = 0; i < d1; ++i)
            {
                result[i, i] = 0;
                for (j = 0; j < i; ++j)
                {
                    element = 0.5 * (a[i, j] - a[j, i]);
                    result[i, j] = element;
                    result[j, i] = -element;
                }
            }
        }  // AntisymmetricPart

        /// <summary>Stores antisymmetric part of a square matrix operand in another matrix.
        /// Antisymmetrization is performed by subtracting non-diagonal terms and their transposed terms,
        /// division by 2 and storing result in one matrix element and its negative value in another element.
        /// Can be done in-place.
        /// Resulting matrix is allocated or reallocated if necessary.</summary>
        /// <param name="a">Original matrix, must be a square matrix.</param>
        /// <param name="result">Matrix where result of negation is be stored.</param>
        public static void AntisymmetricPart(IMatrix a, ref IMatrix result)
        {
            if (a == null)
            {
                result = null;
                return;
            } else if (a.RowCount != a.ColumnCount)
            {
                throw new ArgumentException("Matrix antisymmetrization can not be performed in-place for non-square matrices. "
                    + Environment.NewLine + "  Matrix dimensions: " + a.RowCount + "x" + a.ColumnCount + ".");
            }
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (a.RowCount != result.RowCount || a.ColumnCount != result.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            double element;
            for (i = 0; i < result.RowCount; ++i)
            {
                result[i, i] = 0;
                for (j = 0; j < i; ++j)
                {
                    element = 0.5 * (a[i, j] - a[j, i]);
                    result[i, j] = element;
                    result[j, i] = -element;
                }
            }
        }  // AntisymmetricPart



        /// <summary>Sums two matrices and stores the result in the specified result matrix.
        /// Operation can be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operands.</param>
        public static void AddPlain(IMatrix a, IMatrix b, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = a[i, j] + b[i, j];
        }


        /// <summary>Sums two matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored. Dimensions must match dimensions of operands.</param>
        public static void Add(IMatrix a, IMatrix b, IMatrix result)
        {
            if (a == null || b==null)
                throw new ArgumentNullException("Matrix summation: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix summation is not specified (null reference).");
            else if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount
                || result.RowCount!=a.RowCount || result.ColumnCount !=a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix summation. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int i, j;
                for (i = 0; i < a.RowCount; ++i)
                    for (j = 0; j < a.ColumnCount; ++j)
                        result[i, j] = a[i, j] + b[i,j];
            }
        }

        /// <summary>Sums two matrices and stores the result in the specified result matrix.
        /// Operation can be performed in place.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void Add(IMatrix a, IMatrix b, ref IMatrix result)
        {
            if (a == null || b==null)
                throw new ArgumentNullException("Matrix summation: operand not specified (null reference).");
            else if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix summation. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount,a.ColumnCount);
            else if (result.RowCount!=a.RowCount || result.ColumnCount !=a.ColumnCount)
                result = a.GetNew(a.RowCount,a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = a[i, j] + b[i, j];
        }

        /// <summary>Subtracts two matrices and stores the result in the specified result matrix.
        /// Operation can be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void SubtractPlain(IMatrix a, IMatrix b, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = a[i, j] - b[i, j];
        }

        /// <summary>Subtracts two matrices and stores the result in the specified result matrix.
        /// Operation can be performed in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void Subtract(IMatrix a, IMatrix b, IMatrix result)
        {
            if (a == null || b==null)
                throw new ArgumentNullException("Matrix subtraction: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix subtraction is not specified (null reference).");
            else if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount
                || result.RowCount!=a.RowCount || result.ColumnCount !=a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix subtraction. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int i, j;
                for (i = 0; i < a.RowCount; ++i)
                    for (j = 0; j < a.ColumnCount; ++j)
                        result[i, j] = a[i, j] - b[i,j];
            }
        }

        /// <summary>Subtracts two matrices and stores the result in the specified result matrix.
        /// Operation can be performed in place.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void Subtract(IMatrix a, IMatrix b, ref IMatrix result)
        {
            if (a == null || b==null)
                throw new ArgumentNullException("Matrix subtraction: operand not specified (null reference).");
            else if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix subtraction. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount,a.ColumnCount);
            else if (result.RowCount!=a.RowCount || result.ColumnCount !=b.ColumnCount)
                result = a.GetNew(a.RowCount,a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = a[i, j] - b[i,j];
        }


        #region ArrayOperations

        /// <summary>Element-by-element multiplication.
        /// This operation can be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void ArrayMultiplyPlain(IMatrix a, IMatrix b, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = a[i, j] * b[i, j];
        }

        /// <summary>Element-by-element multiplication.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void ArrayMultiply(IMatrix a, IMatrix b, IMatrix result)
        {
            if (a == null || b==null)
                throw new ArgumentNullException("Matrix array multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix array multiplication is not specified (null reference).");
            else if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount
                || result.RowCount!=a.RowCount || result.ColumnCount !=a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix array multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int i, j;
                for (i = 0; i < a.RowCount; ++i)
                    for (j = 0; j < a.ColumnCount; ++j)
                        result[i, j] = a[i, j] * b[i,j];
            }
        }

        /// <summary>Element-by-element multiplication.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void ArrayMultiply(IMatrix a, IMatrix b, ref IMatrix result)
        {
            if (a == null || b==null)
                throw new ArgumentNullException("Matrix array multiplication: operand not specified (null reference).");
            else if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix array multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount,a.ColumnCount);
            else if (result.RowCount!=a.RowCount || result.ColumnCount !=b.ColumnCount)
                result = a.GetNew(a.RowCount,a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = a[i, j] * b[i,j];
        }

        /// <summary>Element-by-element division.
        /// This operation can be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void ArrayDividePlain(IMatrix a, IMatrix b, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = a[i, j] / b[i, j];
        }

        /// <summary>Element-by-element division.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void ArrayDivide(IMatrix a, IMatrix b, IMatrix result)
        {
            if (a == null || b==null)
                throw new ArgumentNullException("Matrix array division: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix array division is not specified (null reference).");
            else if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount
                || result.RowCount!=a.RowCount || result.ColumnCount !=a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix array division. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int i, j;
                for (i = 0; i < a.RowCount; ++i)
                    for (j = 0; j < a.ColumnCount; ++j)
                        result[i, j] = a[i, j] / b[i,j];
            }
        }

        /// <summary>Element-by-element division.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void ArrayDivide(IMatrix a, IMatrix b, ref IMatrix result)
        {
            if (a == null || b==null)
                throw new ArgumentNullException("Matrix array division: operand not specified (null reference).");
            else if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix array division. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount,a.ColumnCount);
            else if (result.RowCount!=a.RowCount || result.ColumnCount !=b.ColumnCount)
                result = a.GetNew(a.RowCount,a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = a[i, j] / b[i,j];
        }

        /// <summary>Element-by-element raise to power.
        /// This operation can be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="power">Power to which elements of the matrix are raised.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void ArrayPowerPlain(IMatrix a, double power, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = Math.Pow(a[i, j], power);
        }

        /// <summary>Element-by-element raise to power.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="power">Power to which elements of the matrix are raised.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void ArrayPower(IMatrix a, double power, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix array raise to power: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix array raise to power is not specified (null reference).");
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix array raise to power. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int i, j;
                for (i = 0; i < a.RowCount; ++i)
                    for (j = 0; j < a.ColumnCount; ++j)
                        result[i, j] = Math.Pow(a[i, j], power);
            }
        }

        /// <summary>Element-by-element raise to power.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="power">Power to which elements of the matrix are raised.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void ArrayPower(IMatrix a, double power, ref IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix array raise to power: operand not specified (null reference).");
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = Math.Pow(a[i, j], power);
        }

        /// <summary>Element-by-element mapping of an arbitrary function.
        /// This operation can be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="mapping">Mapping applied to each element.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void ArrayMapPlain(IMatrix a, Converter<double, double> mapping, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = mapping(a[i,j]);
        }

        /// <summary>Element-by-element mapping of an arbitrary function.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="mapping">Mapping applied to each element.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void ArrayMap(IMatrix a, Converter<double, double> mapping, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix array raise to power: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix array raise to power is not specified (null reference).");
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix array raise to power. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else
            {
                int i, j;
                for (i = 0; i < a.RowCount; ++i)
                    for (j = 0; j < a.ColumnCount; ++j)
                        result[i, j] = mapping(a[i, j]);
            }
        }

        /// <summary>Element-by-element mapping of an arbitrary function.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then teh result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="mapping">Mapping applied to each element.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void ArrayMap(IMatrix a, Converter<double, double> mapping, ref IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix array raise to power: operand not specified (null reference).");
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                    result[i, j] = mapping(a[i, j]);
        }

        #endregion ArrayOperations


        #region MatrixProducts


        // MATRIX * MATRIX

        /// <summary>R=A*B. 
        /// Multiplies two matrices and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyPlain(IMatrix a, IMatrix b, IMatrix result)
        {
            int i, j, k;
            double element;
            if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < b.ColumnCount; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < a.ColumnCount; ++k)
                        element += a[i, k] * b[k, j];
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B. 
        /// Multiplies two matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void Multiply(IMatrix a, IMatrix b, IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix multiplication is not specified (null reference).");
            else if (a.ColumnCount != b.RowCount 
                || result.RowCount != a.RowCount || result.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k;
            double element;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < b.ColumnCount; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < a.ColumnCount; ++k)
                        element += a[i, k] * b[k, j];
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B. 
        /// Multiplies two matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void Multiply(IMatrix a, IMatrix b, ref IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            else if (a.ColumnCount != b.RowCount)
                throw new ArgumentException("Dimension mismatch at matrix  multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount, b.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != b.ColumnCount)
                result = a.GetNew(a.RowCount, b.ColumnCount);
            int i, j, k;
            double element;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < b.ColumnCount; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < a.ColumnCount; ++k)
                        element += a[i, k] * b[k, j];
                    result[i, j] = element;
                }
        }



        // TRANSPOSED MATRIX * MATRIX

        /// <summary>R=A^T*B
        /// Calculates product of transposed matrix and a matrix, and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspMatPlain(IMatrix a, IMatrix b, IMatrix result)
        {
            int i, j, k;
            double element;
            if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Transposed matrix by matrix multiplication can not be done in place.");
            for (i = 0; i < a.ColumnCount; ++i)  // first matrix transposed!
                for (j = 0; j < b.ColumnCount; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < a.RowCount; ++k)  // first matrix transposed!
                        element += a[k, i] * b[k, j];  // first matrix transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>R=A^T*B
        /// Multiplies two matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspMat(IMatrix a, IMatrix b, IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Transposed matrix by matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Transposed matrix by matrix  multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Transposed matrix by matrix multiplication: result storage for matrix multiplication is not specified (null reference).");
            else if (a.RowCount != b.RowCount  // first matrix transposed!
                || result.RowCount != a.ColumnCount || result.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Dimension mismatch at transposed matrix by matrix  multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k;
            double element;
            for (i = 0; i < a.ColumnCount; ++i)  // first matrix transposed!
                for (j = 0; j < b.ColumnCount; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < a.RowCount; ++k)  // first matrix transposed!
                        element += a[k, i] * b[k, j];  // first matrix transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>R=A^T*B
        /// Multiplies two matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyTranspMat(IMatrix a, IMatrix b, ref IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Transposed matrix by matrix  multiplication: operand not specified (null reference).");
            else if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Transposed matrix by matrix  multiplication can not be done in place.");
            else if (a.RowCount != b.RowCount)  // first matrix transposed!
                throw new ArgumentException("Dimension mismatch at transposed matrix by matrix  multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.ColumnCount, b.ColumnCount);  // first matrix transposed!
            else if (result.RowCount != a.ColumnCount || result.ColumnCount != b.ColumnCount)  // first matrix transposed!
                result = a.GetNew(a.ColumnCount, b.ColumnCount);  // first matrix transposed!
            int i, j, k;
            double element;
            for (i = 0; i < a.ColumnCount; ++i)  // first matrix transposed!
                for (j = 0; j < b.ColumnCount; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < a.RowCount; ++k)  // first matrix transposed!
                        element += a[k, i] * b[k, j];  // first matrix transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>Tests product A^T*B.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiplyTranspMat(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d2, d1); A.SetRandom();
            IMatrix B = new Matrix(d2, d3); B.SetRandom();
            IMatrix A_T = null; Matrix.Transpose(A, ref A_T);
            MultiplyTranspMat(A, B, ref result);
            Multiply(A_T, B, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }



        // MATRIX * TRANSPOSED MATRIX

        /// <summary>R=A*B^T. 
        /// Multiplies a matrix by transpose of another matrix and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyMatTranspPlain(IMatrix a, IMatrix b, IMatrix result)
        {
            int i, j, k;
            double element;
            if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Matrix by transposed matrix  multiplication can not be done in place.");
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < b.RowCount; ++j)  // second matrix transposed!
                {
                    element = 0.0;
                    for (k = 0; k < a.ColumnCount; ++k)
                        element += a[i, k] * b[j, k];  // second matrix transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B^T. 
        /// Multiplies a matrix by transpose of another matrix and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyMatTransp(IMatrix a, IMatrix b, IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrix by transposed matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Matrix by transposed matrix multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix by transposed matrix multiplication is not specified (null reference).");
            else if (a.ColumnCount != b.ColumnCount  // second matrix transposed!
                || result.RowCount != a.RowCount || result.ColumnCount != b.RowCount)
                throw new ArgumentException("Dimension mismatch at matrix by transposed matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k;
            double element;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < b.RowCount; ++j)  // second matrix transposed!
                {
                    element = 0.0;
                    for (k = 0; k < a.ColumnCount; ++k)
                        element += a[i, k] * b[j, k];  // second matrix transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B^T. 
        /// Multiplies a matrix by transpose of another matrix and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyMatTransp(IMatrix a, IMatrix b, ref IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrix by transposed matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Matrix by transposed matrix multiplication can not be done in place.");
            else if (a.ColumnCount != b.ColumnCount)  // second matrix transposed!
                throw new ArgumentException("Dimension mismatch at matrix by transposed matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount, b.RowCount);  // second matrix transposed!
            else if (result.RowCount != a.RowCount || result.ColumnCount != b.RowCount)  // second matrix transposed!
                result = a.GetNew(a.RowCount, b.RowCount);
            int i, j, k;
            double element;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < b.RowCount; ++j)  // second matrix transposed!
                {
                    element = 0.0;
                    for (k = 0; k < a.ColumnCount; ++k)
                        element += a[i, k] * b[j, k];  // second matrix transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>Tests product A*B^T.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiplyMatTransp(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d1, d2);  A.SetRandom();
            IMatrix B = new Matrix(d3, d2); B.SetRandom();
            IMatrix B_T = null; Matrix.Transpose(B, ref B_T);
            MultiplyMatTransp(A, B, ref result);
            Multiply(A, B_T, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }



        // TRANSPOSED MATRIX * TRANSPOSED MATRIX

        /// <summary>R=A^T*B^T. 
        /// Multiplies transposed matrix by another transposed matrix, and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspTranspPlain(IMatrix a, IMatrix b, IMatrix result)
        {
            int i, j, k;
            double element;
            if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Transposed matrix by transposed matrix multiplication can not be done in place.");
            for (i = 0; i < a.ColumnCount; ++i)  // both matrices transposed!
                for (j = 0; j < b.RowCount; ++j)  // both matrices transposed!
                {
                    element = 0.0;
                    for (k = 0; k < a.RowCount; ++k)  // both matrices transposed!
                        element += a[k, i] * b[j, k];  // both matrices transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>R=A^T*B^T. 
        /// Multiplies transposed matrix by another transposed matrix, and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspTransp(IMatrix a, IMatrix b, IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Transposed matrix by transposed matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Transposed matrix by transposed matrix multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for transposed matrix by transposed matrix multiplication is not specified (null reference).");
            else if (a.RowCount != b.ColumnCount  // both matrices transposed!
                || result.RowCount != a.ColumnCount || result.ColumnCount != b.RowCount)  // both matrices transposed!
                throw new ArgumentException("Dimension mismatch at transposed matrix by transposed matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k;
            double element;
            for (i = 0; i < a.ColumnCount; ++i)  // both matrices transposed!
                for (j = 0; j < b.RowCount; ++j)  // both matrices transposed!
                {
                    element = 0.0;
                    for (k = 0; k < a.RowCount; ++k)  // both matrices transposed!
                        element += a[k, i] * b[j, k];  // both matrices transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>R=A^T*B^T. 
        /// Multiplies transposed matrix by another transposed matrix, and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyTranspTransp(IMatrix a, IMatrix b, ref IMatrix result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Transposed matrix by transposed matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result)
                throw new ArgumentException("Result the same as operand. Transposed matrix by transposed matrix multiplication can not be done in place.");
            else if (a.RowCount != b.ColumnCount)  // both matrices transposed!
                throw new ArgumentException("Dimension mismatch at transposed matrix by transposed matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.ColumnCount, b.RowCount);  // both matrices transposed!
            else if (result.RowCount != a.ColumnCount || result.ColumnCount != b.RowCount)  // both matrices transposed!
                result = a.GetNew(a.ColumnCount, b.RowCount);  // both matrices transposed!
            int i, j, k;
            double element;
            for (i = 0; i < a.ColumnCount; ++i)  // both matrices transposed!
                for (j = 0; j < b.RowCount; ++j)  // both matrices transposed!
                {
                    element = 0.0;
                    for (k = 0; k < a.RowCount; ++k)  // both matrices transposed!
                        element += a[k, i] * b[j, k];  // both matrices transposed!
                    result[i, j] = element;
                }
        }

        /// <summary>Tests product A^T*B^T.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiplyTranspTransp(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d2, d1); A.SetRandom();
            IMatrix B = new Matrix(d3, d2); B.SetRandom();
            IMatrix A_T = null; Matrix.Transpose(A, ref A_T);
            IMatrix B_T = null; Matrix.Transpose(B, ref B_T);
            MultiplyTranspTransp(A, B, ref result);
            Multiply(A_T, B_T, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }


        // MATRIX * MATRIX * MATRIX

        /// <summary>R=A*B*C. 
        /// Multiplies three matrices and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyPlain(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            // Store dimensions of involved matrix factors:
            int 
                m = 
                a.RowCount,
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.ColumnCount;
            if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[l, j];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B*C. 
        /// Multiplies three matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void Multiply(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            if (a == null || b == null || c==null)
                throw new ArgumentNullException("Tripple matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result || c==result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for tripple matrix multiplication is not specified (null reference).");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.RowCount,
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.ColumnCount;
            if (a.ColumnCount!=n || c.RowCount!=o || result.RowCount!=m || result.ColumnCount!=p)
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[l, j];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B*C. 
        /// Multiplies three matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void Multiply(IMatrix a, IMatrix b, IMatrix c, ref IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix multiplication can not be done in place.");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.RowCount,
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.ColumnCount;
            if (a.ColumnCount != n || c.RowCount != o)
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount + ".");
            if (result == null)
                result=a.GetNew(m,p);
            else if (result.RowCount != m || result.ColumnCount != p)
                result=a.GetNew(m,p);
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[l, j];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>Tests product A*B*C.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiply3(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3, d4 = 5;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d1, d2); A.SetRandom();
            IMatrix B = new Matrix(d2, d3); B.SetRandom();
            IMatrix C = new Matrix(d3, d4); C.SetRandom();
            Multiply(A, B, C, ref result);
            IMatrix AB = null;
            Multiply(A, B, ref AB);
            Multiply(AB, C, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }


        // TRANSPOSED MATRIX * MATRIX * TRANSPOSED MATRIX

        /// <summary>R=A^T*B*C^T. 
        /// Multiplies three matrices (transposed first argument, second argument, and transposed third argument) 
        /// and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of all arguments must be consistent, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand, transpose of the first matrix factor.</param>
        /// <param name="b">Second operand, the second matrix factor.</param>
        /// <param name="c">Third operand, transpose of the third matrix factor.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must be consistent with dimensions of operands.</param>
        public static void MultiplyTranspMatTranspPlain(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix multiplication (A^T*B*C^T) can not be done in place.");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.ColumnCount,  // transpose!
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.RowCount;  // transpose!
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[j, l];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A^T*B*C^T. 
        /// Multiplies three matrices (transposed first argument, second argument, and transposed third argument) 
        /// and stores the result in the specified result matrix.
        /// WARNING: dimensions of all arguments must be consistent, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspMatTransp(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication (A^T*B*C^T): operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix (A^T*B*C^T) multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for tripple matrix multiplication (A^T*B*C^T) is not specified (null reference).");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.ColumnCount,  // transpose!
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.RowCount;  // transpose!
            if (a.RowCount != n || c.ColumnCount != o   // transposed a and b!
                || result.RowCount != m || result.ColumnCount != p)
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication (A^T*B*C^T). " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[j, l];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A^T*B*C^T. 
        /// Multiplies three matrices (transposed first argument, second argument, and transposed third argument) 
        /// and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must be consistent, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where result is stored. Allocated or reallocated if necessary.</param>
        public static void MultiplyTranspMatTransp(IMatrix a, IMatrix b, IMatrix c, ref IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication (A^T*B*C^T): operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix multiplication (A^T*B*C^T) can not be done in place.");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.ColumnCount,  // transpose!
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.RowCount;  // transpose!
            if (a.RowCount != n || c.ColumnCount != o)   // transposed a and b!
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication (A^T*B*C^T). " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount + ".");
            if (result == null)
                result = a.GetNew(m, p);
            else if (result.RowCount != m || result.ColumnCount != p)
                result = a.GetNew(m, p);
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[j, l];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>Tests product A^T*B*C^T.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiplyTranspMatTransp(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3, d4 = 5;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d2, d1); A.SetRandom();
            IMatrix B = new Matrix(d2, d3); B.SetRandom();
            IMatrix C = new Matrix(d4, d3); C.SetRandom();
            MultiplyTranspMatTransp(A, B, C, ref result);
            IMatrix A_T = null;  Matrix.Transpose(A, ref A_T);
            IMatrix C_T = null;  Matrix.Transpose(C, ref C_T);
            IMatrix prod = null;
            Multiply(A_T, B, ref prod);
            Multiply(prod, C_T, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }


        // TRANSPOSED MATRIX * MATRIX * MATRIX

        /// <summary>R=A^T*B*C. 
        /// Multiplies three matrices (transposed first argument, second argument, and third argument) 
        /// and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of all arguments must be consistent, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand, transpose of the first matrix factor.</param>
        /// <param name="b">Second operand, the second matrix factor.</param>
        /// <param name="c">Third operand, transpose of the third matrix factor.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must be consistent with dimensions of operands.</param>
        public static void MultiplyTranspMatMatPlain(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix multiplication (A^T*B*C) can not be done in place.");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.ColumnCount,  // transpose!
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.ColumnCount;  // transpose!
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[l, j];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A^T*B*C. 
        /// Multiplies three matrices (transposed first argument, second argument, and third argument) 
        /// and stores the result in the specified result matrix.
        /// WARNING: dimensions of all arguments must be consistent, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspMatMat(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication (A^T*B*C): operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix (A^T*B*C) multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for tripple matrix multiplication (A^T*B*C) is not specified (null reference).");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.ColumnCount,  // transpose!
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.ColumnCount;  // transpose!
            if (a.RowCount != n || c.RowCount != o   // transposed a and b!
                || result.RowCount != m || result.ColumnCount != p)
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication (A^T*B*C). " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[l, j];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A^T*B*C. 
        /// Multiplies three matrices (transposed first argument, second argument, and third argument) 
        /// and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must be consistent, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where result is stored. Allocated or reallocated if necessary.</param>
        public static void MultiplyTranspMatMat(IMatrix a, IMatrix b, IMatrix c, ref IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication (A^T*B*C): operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix multiplication (A^T*B*C) can not be done in place.");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.ColumnCount,  // transpose!
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.ColumnCount;  // transpose!
            if (a.RowCount != n || c.RowCount != o)   // transposed a and b!
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication (A^T*B*C). " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount + ".");
            if (result == null)
                result = a.GetNew(m, p);
            else if (result.RowCount != m || result.ColumnCount != p)
                result = a.GetNew(m, p);
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[l, j];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }


        /// <summary>Tests product A^T*B*C.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiplyTranspMatMat(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3, d4 = 5;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d2, d1); A.SetRandom();
            IMatrix B = new Matrix(d2, d3); B.SetRandom();
            IMatrix C = new Matrix(d3, d4); C.SetRandom();
            MultiplyTranspMatMat(A, B, C, ref result);
            IMatrix A_T = null; Matrix.Transpose(A, ref A_T);
            IMatrix prod = null;
            Multiply(A_T, B, ref prod);
            Multiply(prod, C, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }


        // MATRIX * MATRIX * TRANSPOSED MATRIX

        /// <summary>R=A*B*C^T. 
        /// Multiplies three matrices (first argumet, second argumet, and transposed third argument) 
        /// and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyMatMatTranspPlain(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            // Store dimensions of involved matrix factors:
            int
                m = a.RowCount,
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.RowCount;
            if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[j, l];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B*C^T. 
        /// Multiplies three matrices (first argumet, second argumet, and transposed third argument) 
        /// and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyMatMatTransp(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix-matrix-transpose multiplication: operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for tripple matrix-matrix-transpose multiplication is not specified (null reference).");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.RowCount,
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.RowCount;
            if (a.ColumnCount != n || c.ColumnCount != o || result.RowCount != m || result.ColumnCount != p)
                throw new ArgumentException("Dimension mismatch at tripple matrix-matrix-transpose multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[j, l];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B*C^T. 
        /// Multiplies three matrices (first argumet, second argumet, and transposed third argument) 
        /// and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyMatMatTransp(IMatrix a, IMatrix b, IMatrix c, ref IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix-matrix-transpose multiplication can not be done in place.");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.RowCount,
                n = b.RowCount,
                o = b.ColumnCount,
                p = c.RowCount;
            if (a.ColumnCount != n || c.ColumnCount != o)
                throw new ArgumentException("Dimension mismatch at tripple matrix-matrix-transpose multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount + ".");
            if (result == null)
                result = a.GetNew(m, p);
            else if (result.RowCount != m || result.ColumnCount != p)
                result = a.GetNew(m, p);
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[k, l] * c[j, l];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>Tests product A*B*C^T.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiplyMatMatTransp(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3, d4 = 5;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d1, d2); A.SetRandom();
            IMatrix B = new Matrix(d2, d3); B.SetRandom();
            IMatrix C = new Matrix(d4, d3); C.SetRandom();
            MultiplyMatMatTransp(A, B, C, ref result);
            IMatrix C_T = null;  Transpose(C, ref C_T);
            IMatrix AB = null; 
            Multiply(A, B, ref AB);
            Multiply(AB, C_T, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }

        // MATRIX * TRANSPOSED MATRIX * MATRIX

        /// <summary>R=A*B^T*C. 
        /// Multiplies three matrices (first argument, transposed second argument, and third argument) 
        /// and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyMatTranspMatPlain(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            // Store dimensions of involved matrix factors:
            int
                m =
                a.RowCount,
                n = b.ColumnCount,
                o = b.RowCount,
                p = c.ColumnCount;
            if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[l, k] * c[l, j];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B^T*C. 
        /// Multiplies three matrices (first argument, transposed second argument, and third argument) 
        /// and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyMatTranspMat(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for tripple matrix multiplication is not specified (null reference).");
            // Store dimensions of involved matrix factors:
            int
                m = a.RowCount,
                n = b.ColumnCount,
                o = b.RowCount,
                p = c.ColumnCount;
            if (a.ColumnCount != n || c.RowCount != o || result.RowCount != m || result.ColumnCount != p)
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[l, k] * c[l, j];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>R=A*B^T*C. 
        /// Multiplies three matrices (first argument, transposed second argument, and third argument) 
        /// and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyMatTranspMat(IMatrix a, IMatrix b, IMatrix c, ref IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication: operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix multiplication can not be done in place.");
            // Store dimensions of involved matrix factors:
            int
                m =
                a.RowCount,
                n = b.ColumnCount,
                o = b.RowCount,
                p = c.ColumnCount;
            if (a.ColumnCount != n || c.RowCount != o)
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount + ".");
            if (result == null)
                result = a.GetNew(m, p);
            else if (result.RowCount != m || result.ColumnCount != p)
                result = a.GetNew(m, p);
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[l, k] * c[l, j];
                        element += a[i, k] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>Tests product A*B^T*C.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiplyMatTranspMat(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3, d4 = 5;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d1, d2); A.SetRandom();
            IMatrix B = new Matrix(d3, d2); B.SetRandom();
            IMatrix C = new Matrix(d3, d4); C.SetRandom();
            MultiplyMatTranspMat(A, B, C, ref result);
            IMatrix B_T = null;  Transpose(B, ref B_T);
            IMatrix ABT = null;
            Multiply(A, B_T, ref ABT);
            Multiply(ABT, C, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }


        // TRANSPOSED MATRIX * TRANSPOSED MATRIX * TRANSPOSED MATRIX

        /// <summary>R=A^T*B^T*C^T. 
        /// Multiplies three matrices and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspTranspTranspPlain(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            // Store dimensions of involved matrix factors:
            int
                m = a.ColumnCount,
                n = b.ColumnCount,
                o = b.RowCount,
                p = c.RowCount;
            if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication (A^T*B^T*C^T) can not be done in place.");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[l, k] * c[j, l];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }


        /// <summary>R=A^T*B^T*C^T. 
        /// Multiplies three matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspTranspTransp(IMatrix a, IMatrix b, IMatrix c, IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication (A^T*B^T*C^T): operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Matrix multiplication (A^T*B^T*C^T) can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for tripple matrix multiplication (A^T*B^T*C^T) is not specified (null reference).");
            // Store dimensions of involved matrix factors:
            int
                m = a.ColumnCount,
                n = b.ColumnCount,
                o = b.RowCount,
                p = c.RowCount;
            if (a.RowCount != n || c.ColumnCount != o || result.RowCount != m || result.ColumnCount != p)
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication (A^T*B^T*C^T). " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[l, k] * c[j, l];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }


        /// <summary>R=A^T*B^T*C^T. 
        /// Multiplies three matrices and stores the result in the specified result matrix.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// WARNING: This operation can not be performed in place.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="c">Third operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyTranspTranspTransp(IMatrix a, IMatrix b, IMatrix c, ref IMatrix result)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Tripple matrix multiplication (A^T*B^T*C^T): operand not specified (null reference).");
            else if (a == result || b == result || c == result)
                throw new ArgumentException("Result the same as operand. Tripple matrix multiplication (A^T*B^T*C^T) can not be done in place.");
            // Store dimensions of involved matrix factors:
            int
                m = a.ColumnCount,
                n = b.ColumnCount,
                o = b.RowCount,
                p = c.RowCount;
            if (a.RowCount != n || c.ColumnCount != o)
                throw new ArgumentException("Dimension mismatch at tripple matrix multiplication (A^T*B^T*C^T). " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + b.RowCount + "x" + b.ColumnCount
                    + ",  third operand: " + c.RowCount + "x" + c.ColumnCount + ".");
            if (result == null)
                result = a.GetNew(m, p);
            else if (result.RowCount != m || result.ColumnCount != p)
                result = a.GetNew(m, p);
            int i, j, k, l;
            double element, element_bc;
            for (i = 0; i < m; ++i)
                for (j = 0; j < p; ++j)
                {
                    element = 0.0;
                    for (k = 0; k < n; ++k)
                    {
                        element_bc = 0.0;
                        for (l = 0; l < o; ++l)
                            element_bc += b[l, k] * c[j, l];
                        element += a[k, i] * element_bc;
                    }
                    result[i, j] = element;
                }
        }

        /// <summary>Tests product A^T*B^T*C^T.</summary>
        /// <param name="tolerance">Tolerance (on norm of the difference) for the test to pass</param>
        /// <param name="printReports">Specifies whether to print reports or not.</param>
        /// <returns>true if the test passes, false if not.</returns>
        private static bool TestMultiplyTranspTranspTransp(double tolerance, bool printReports)
        {
            int d1 = 2, d2 = 4, d3 = 3, d4 = 5;
            IMatrix result = null, referenceResult = null;
            IMatrix A = new Matrix(d2, d1); A.SetRandom();
            IMatrix B = new Matrix(d3, d2); B.SetRandom();
            IMatrix C = new Matrix(d4, d3); C.SetRandom();
            MultiplyTranspTranspTransp(A, B, C, ref result);
            IMatrix A_T = null; Transpose(A, ref A_T);
            IMatrix B_T = null; Transpose(B, ref B_T);
            IMatrix C_T = null; Transpose(C, ref C_T);
            IMatrix ATBT = null;
            Multiply(A_T, B_T, ref ATBT);
            Multiply(ATBT, C_T, ref referenceResult);
            bool ret = CheckTestResult(result, referenceResult, tolerance, printReports);
            return ret;
        }





        #endregion MatrixProducts


        #region MatrixVectorProducts

        // MATRIX * VECTOR

        /// <summary>R=A*b. 
        /// Multiplies a matrix with a vector and stores the result in the specified result vector.
        /// Operation can not be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyPlain(IMatrix a, IVector b, IVector result)
        {
            int i, k;
            if (b == result)
                throw new ArgumentException("Result the same as operand. Matrix-vector multiplication can not be done in place.");
            double element;
            for (i = 0; i < a.RowCount; ++i)
            {
                element = 0.0;
                for (k = 0; k < a.ColumnCount; ++k)
                    element += a[i, k] * b[k];
                result[i] = element;
            }
        }

        /// <summary>R=A*b. 
        /// Multiplies a matrix with a vector and stores the result in the specified result vector.
        /// Operation can not be performed in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void Multiply(IMatrix a, IVector b, IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrix by vector multiplication: operand not specified (null reference).");
            else if (b == result)
                throw new ArgumentException("Result the same as operand. Matrix-vector multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix multiplication is not specified (null reference).");
            else if (a.ColumnCount != b.Length
                || result.Length != a.RowCount)
                throw new ArgumentException("Dimension mismatch at matrix by vector multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand dim.: " + b.Length 
                    + ", result dim: " + result.Length + ".");
            int i, k;
            double element;
            for (i = 0; i < a.RowCount; ++i)
                {
                    element = 0.0;
                    for (k = 0; k < a.ColumnCount; ++k)
                        element += a[i, k] * b[k];
                    result[i] = element;
                }
        }

        /// <summary>R=A*b. 
        /// Multiplies matrix by scalar and stores the result in the specified result matrix.
        /// Operation can not be performed in place.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void Multiply(IMatrix a, IVector b, ref IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrix by vector multiplication: operand not specified (null reference).");
            else if (b == result)
                throw new ArgumentException("Result the same as operand. Matrix-vector multiplication can not be done in place.");
            else if (a.ColumnCount != b.Length)
                throw new ArgumentException("Dimension mismatch at matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand dim.: " + b.Length + ".");
            else if (result == null)
                result = a.GetNewVector(a.RowCount);
            else if (result.Length != a.RowCount)
                result = a.GetNewVector(a.RowCount);
            int i, k;
            double element;
            for (i = 0; i < a.RowCount; ++i)
            {
                element = 0.0;
                for (k = 0; k < a.ColumnCount; ++k)
                    element += a[i, k] * b[k];
                result[i] = element;
            }
        }

        // TRANSPOSED MATRIX * VECTOR

        /// <summary>R=A^T*b. 
        /// Multiplies a transposed matrix with a vector and stores the result in the specified result vector.
        /// Operation can not be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspVecPlain(IMatrix a, IVector b, IVector result)
        {
            int i, k;
            if (b == result)
                throw new ArgumentException("Result the same as operand. Matrix-vector multiplication can not be done in place.");
            double element;
            for (i = 0; i < a.ColumnCount; ++i)
            {
                element = 0.0;
                for (k = 0; k < a.RowCount; ++k)
                    element += a[k, i] * b[k];
                result[i] = element;
            }
        }

        /// <summary>R=A^T*b. 
        /// Multiplies a transposed matrix with a vector and stores the result in the specified result vector.
        /// Operation can not be performed in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Vector where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyTranspVec(IMatrix a, IVector b, IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrix by vector multiplication: operand not specified (null reference).");
            else if (b == result)
                throw new ArgumentException("Result the same as operand. Matrix-vector multiplication can not be done in place.");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix multiplication is not specified (null reference).");
            else if (a.RowCount != b.Length
                || result.Length != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix by vector multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand dim.: " + b.Length 
                    + ", result dim: " + result.Length + ".");
            int i, k;
            double element;
            for (i = 0; i < a.ColumnCount; ++i)
            {
                element = 0.0;
                for (k = 0; k < a.RowCount; ++k)
                    element += a[k, i] * b[k];
                result[i] = element;
            }
        }

        /// <summary>R=A^T*b. 
        /// Multiplies transposed matrix by scalar and stores the result in the specified result matrix.
        /// Operation can not be performed in place.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyTranspVec(IMatrix a, IVector b, ref IVector result)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Matrix by vector multiplication: operand not specified (null reference).");
            else if (b == result)
                throw new ArgumentException("Result the same as operand. Matrix-vector multiplication can not be done in place.");
            else if (a.ColumnCount != b.Length)
                throw new ArgumentException("Dimension mismatch at matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand dim.: " + b.Length + ".");
            else if (result == null)
                result = a.GetNewVector(a.ColumnCount);
            else if (result.Length != a.ColumnCount)
                result = a.GetNewVector(a.ColumnCount);
            int i, k;
            double element;
            for (i = 0; i < a.ColumnCount; ++i)
            {
                element = 0.0;
                for (k = 0; k < a.RowCount; ++k)
                    element += a[k, i] * b[k];
                result[i] = element;
            }
        }

        // TRANSPOSED VECTOR * MATRIX * VECTOR

        /// <summary>R=a^T*M*b. 
        /// Left-multiplies a matrix with a vector (transposed), right multiplies the result vith another vector, 
        /// and returns the final result.
        /// Can be performed in place.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <left>Left factor (vector as one row matrix).</left>
        /// <param name="a">Matrix factor.</param>
        /// <param name="right">Right factor (vector as one column matrix).</param>
        public static double MultiplyPlain(IVector left, IMatrix a, IVector right)
        {
            int i, j;
            double ret = 0.0;
            for (i = 0; i < a.RowCount; ++i)
            {
                double rightElement = 0.0;
                for (j = 0; j < a.ColumnCount; ++j)
                    rightElement += a[i, j] * right[j];
                ret += left[i]*rightElement;
            }
            return ret;
        }

        /// <summary>R=a^T*M*b. 
        /// Left-multiplies a matrix with a vector (transposed), right multiplies the result vith another vector, 
        /// and returns the final result.
        /// Can be performed in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <left>Left factor (vector as one row matrix).</left>
        /// <param name="a">Matrix factor.</param>
        /// <param name="right">Right factor (vector as one column matrix).</param>
        public static double Multiply(IVector left, IMatrix a, IVector right)
        {
            if (a == null || left==null || right == null)
                throw new ArgumentNullException("Vector-matrix-vector multiplication: operand not specified (null reference).");
            else if (a.RowCount!=left.Length ||  a.ColumnCount != right.Length)
                throw new ArgumentException("Dimension mismatch at matrix by vector multiplication. " + Environment.NewLine
                    + "  Left vector operand: " + left.Length
                    + ", Matrix operand: " + a.RowCount + "x" + a.ColumnCount
                    + ", Right vector operand: " + right.Length + ".");
            int i, j;
            double ret = 0.0;
            for (i = 0; i < a.RowCount; ++i)
            {
                double rightElement = 0.0;
                for (j = 0; j < a.ColumnCount; ++j)
                    rightElement += a[i, j] * right[j];
                ret += left[i] * rightElement;
            }
            return ret;
        }

        /// <summary>R=a^T*M*b. 
        /// Left-multiplies a matrix with a vector (transposed), right multiplies the result vith another vector, 
        /// and returns the final result.
        /// Can be performed in place.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <left>Left factor (vector as one row matrix).</left>
        /// <param name="a">Matrix factor.</param>
        /// <param name="right">Right factor (vector as one column matrix).</param>
        public static double Multiply(IVector left, IMatrix a, IVector right, ref IVector result)
        {
            if (a == null || left == null || right == null)
                throw new ArgumentNullException("Vector-matrix-vector multiplication: operand not specified (null reference).");
            else if (a.RowCount != left.Length || a.ColumnCount != right.Length)
                throw new ArgumentException("Dimension mismatch at matrix by vector multiplication. " + Environment.NewLine
                    + "  Left vector operand: " + left.Length
                    + ", Matrix operand: " + a.RowCount + "x" + a.ColumnCount
                    + ", Right vector operand: " + right.Length + ".");
            int i, j;
            double ret = 0.0;
            for (i = 0; i < a.RowCount; ++i)
            {
                double rightElement = 0.0;
                for (j = 0; j < a.ColumnCount; ++j)
                    rightElement += a[i, j] * right[j];
                ret += left[i] * rightElement;
            }
            return ret;
        }


        #endregion MatrixVectorProducts


        #region MatrixScalarProducts


        /// <summary>Multiplies matrix by scalar and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// This operation can be performed in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyPlain(IMatrix a, double b, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i,j]*b;
                }
        }

        /// <summary>Multiplies matrix by scalar and stores the result in the specified result matrix.
        /// This operation can be performed in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void Multiply(IMatrix a, double b, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix-scalar multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix-scalar multiplication is not specified (null reference).");
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix-scalar multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: scalar"
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i,j]*b;
                }
       }

        /// <summary>Multiplies matrix by scalar and stores the result in the specified result matrix.
        /// This operation can be performed in place.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void Multiply(IMatrix a, double b, ref IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix-scalar multiplication: operand not specified (null reference).");
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i,j]*b;
                }
        }




        /// <summary>Divides matrix by scalar and stores the result in the specified result matrix.
        /// This is a plain version of the method that does not perform any consistency checks.
        /// This operation can be performed in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void DividePlain(IMatrix a, double b, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * b;
                }
        }

        /// <summary>Divides matrix by scalar and stores the result in the specified result matrix.
        /// This operation can be performed in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void Divide(IMatrix a, double b, IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix-scalar multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix-scalar multiplication is not specified (null reference).");
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix-scalar multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: scalar"
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * b;
                }
        }

        /// <summary>Divides matrix by scalar and stores the result in the specified result matrix.
        /// This operation can be performed in place.
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void Divide(IMatrix a, double b, ref IMatrix result)
        {
            if (a == null)
                throw new ArgumentNullException("Matrix-scalar multiplication: operand not specified (null reference).");
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * b;
                }
        }

        #endregion MatrixScalarProducts


        #region DiagonalMatrixProducts


        // DIAGONAL * MATRIX

        /// <summary>Left-multiplies a matrix with a diagonal matrix, and stores the result in the specified result matrix.
        /// R=diag(d)*A
        /// This is a plain version of the method that does not perform any consistency checks.
        /// The operation can be done in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="diag">First operand (a vector representing a diagonal matrix).</param>
        /// <param name="a">First operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyDiagonalPlain(IVector diag, IMatrix a, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * diag[i];
                }
        }

        /// <summary>Right-multiplies a matrix with a diagonal matrix, and stores the result in the specified result matrix.
        /// R=diag(d)*A
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="diag">First operand (a vector representing a diagonal matrix).</param>
        /// <param name="a">First operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyDiagonal(IVector diag, IMatrix a, IMatrix result)
        {
            if (a == null || diag == null)
                throw new ArgumentNullException("Diagonal - matrix multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for diagonal - matrix multiplication is not specified (null reference).");
            else if (diag.Length != a.RowCount
                || result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at diagonal - matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + diag.Length
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * diag[i];
                }
        }

        /// <summary>Right-multiplies a matrix with a diagonal matrix, and stores the result in the specified result matrix.
        /// R=diag(d)*A
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="diag">First operand (a vector representing a diagonal matrix).</param>
        /// <param name="a">First operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyDiagonal(IVector diag, IMatrix a, ref IMatrix result)
        {
            if (a == null || diag == null)
                throw new ArgumentNullException("Diagonal - matrix multiplication: operand not specified (null reference).");
            else if (diag.Length != a.RowCount)
                throw new ArgumentException("Dimension mismatch at diagonal - matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + diag.Length
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * diag[i];
                }
        }



        // INVERSE DIAGONAL * MATRIX

        /// <summary>Left-multiplies a matrix with inverse of a diagonal matrix, and stores the result in the specified result matrix.
        /// R=diag(d)^-1*A
        /// This is a plain version of the method that does not perform any consistency checks.
        /// The operation can be done in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="diag">First operand (a vector representing a diagonal matrix).</param>
        /// <param name="a">Second operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyInverseDiagonalPlain(IVector diag, IMatrix a, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] / diag[i];
                }
        }

        /// <summary>Right-multiplies a matrix with inverse of a diagonal matrix, and stores the result in the specified result matrix.
        /// R=diag(d)^-1*A
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="diag">First operand (a vector representing a diagonal matrix).</param>
        /// <param name="a">First operand.</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyInverseDiagonal(IVector diag, IMatrix a, IMatrix result)
        {
            if (a == null || diag == null)
                throw new ArgumentNullException("Inverse diagonal - matrix multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for inverse diagonal - matrix multiplication is not specified (null reference).");
            else if (diag.Length != a.RowCount
                || result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at inverse diagonal - matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + diag.Length
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] / diag[i];
                }
        }

        /// <summary>Right-multiplies a matrix with inverse of a diagonal matrix, and stores the result in the specified result matrix.
        /// R=diag(d)^-1*A
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="diag">First operand (a vector representing a diagonal matrix).</param>
        /// <param name="a">First operand.</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyInverseDiagonal(IVector diag, IMatrix a, ref IMatrix result)
        {
            if (a == null || diag == null)
                throw new ArgumentNullException("Inverse diagonal - matrix multiplication: operand not specified (null reference).");
            else if (diag.Length != a.RowCount)
                throw new ArgumentException("Dimension mismatch at inverse diagonal - matrix multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + diag.Length
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] / diag[i];
                }
        }




        // MATRIX * DIAGONAL

        /// <summary>Right-multiplies a matrix with a diagonal matrix, and stores the result in the specified result matrix.
        /// R=A*diag(d)
        /// This is a plain version of the method that does not perform any consistency checks.
        /// The operation can be done in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="diag">Second operand (a vector representing a diagonal matrix).</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyDiagonalPlain(IMatrix a, IVector diag, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * diag[j];
                }
        }

        /// <summary>Right-multiplies a matrix with a diagonal matrix, and stores the result in the specified result matrix.
        /// R=A*diag(d)
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="diag">Second operand (a vector representing a diagonal matrix).</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyDiagonal(IMatrix a, IVector diag, IMatrix result)
        {
            if (a == null || diag == null)
                throw new ArgumentNullException("Matrix - diagonal multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix -diagonal multiplication is not specified (null reference).");
            else if (diag.Length!=a.ColumnCount
                || result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix - diagonal multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + diag.Length 
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * diag[j];
                }
        }

        /// <summary>Right-multiplies a matrix with a diagonal matrix, and stores the result in the specified result matrix.
        /// R=A*diag(d)
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="diag">Second operand (a vector representing a diagonal matrix).</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyDiagonal(IMatrix a, IVector diag, ref IMatrix result)
        {
            if (a == null || diag == null)
                throw new ArgumentNullException("Matrix - diagonal multiplication: operand not specified (null reference).");
            else if (diag.Length != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix - diagonal multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + diag.Length
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] * diag[j];
                }
        }



        // MATRIX * INVERSE DIAGONAL

        /// <summary>Right-multiplies a matrix with inverse of a diagonal matrix, and stores the result in the specified result matrix.
        /// R=A*diag(d)^-1
        /// This is a plain version of the method that does not perform any consistency checks.
        /// The operation can be done in place.
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="diag">Second operand (a vector representing a diagonal matrix).</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyInverseDiagonalPlain(IMatrix a, IVector diag, IMatrix result)
        {
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] / diag[j];
                }
        }

        /// <summary>Right-multiplies a matrix with inverse of a diagonal matrix, and stores the result in the specified result matrix.
        /// R=A*diag(d)^-1
        /// WARNING: dimensions of matrices must match, otherwise an exception is thrown.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="diag">Second operand (a vector representing a diagonal matrix).</param>
        /// <param name="result">Matrix where the result is stored. Dimensions must match dimensions of operands.</param>
        public static void MultiplyInverseDiagonal(IMatrix a, IVector diag, IMatrix result)
        {
            if (a == null || diag == null)
                throw new ArgumentNullException("Matrix - inverse diagonal multiplication: operand not specified (null reference).");
            else if (result == null)
                throw new ArgumentNullException("Result storage for matrix - inverse diagonal multiplication is not specified (null reference).");
            else if (diag.Length!=a.ColumnCount
                || result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix - inverse diagonal multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + diag.Length 
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] / diag[j];
                }
        }

        /// <summary>Right-multiplies a matrix with inverse of a diagonal matrix, and stores the result in the specified result matrix.
        /// R=A*diag(d)^-1
        /// WARNING: dimensions of operands must match, otherwise an exception is thrown.
        /// If dimensions of the result do not match then the result is re-allocated.</summary>
        /// <param name="a">First operand.</param>
        /// <param name="diag">Second operand (a vector representing a diagonal matrix).</param>
        /// <param name="result">Matrix where result is stored.</param>
        public static void MultiplyInverseDiagonal(IMatrix a, IVector diag, ref IMatrix result)
        {
            if (a == null || diag == null)
                throw new ArgumentNullException("Matrix - inverse diagonal multiplication: operand not specified (null reference).");
            else if (diag.Length != a.ColumnCount)
                throw new ArgumentException("Dimension mismatch at matrix - inverse diagonal multiplication. " + Environment.NewLine
                    + "  First operand: " + a.RowCount + "x" + a.ColumnCount
                    + ",  second operand: " + diag.Length
                    + ", result: " + result.RowCount + "x" + result.ColumnCount + ".");
            else if (result == null)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            else if (result.RowCount != a.RowCount || result.ColumnCount != a.ColumnCount)
                result = a.GetNew(a.RowCount, a.ColumnCount);
            int i, j;
            for (i = 0; i < a.RowCount; ++i)
                for (j = 0; j < a.ColumnCount; ++j)
                {
                    result[i, j] = a[i, j] / diag[j];
                }
        }



        #endregion DiagonalMatrixProducts


        #region Auxiliary

        /// <summary>Returns hash code of the specified matrix.</summary>
        /// <param name="mat">Matrix whose hath code is returned.</param>
        /// <remarks>This method should be used when overriding the GetHashCode() in  vector classes, 
        /// in order to unify calculation of hash code over different vector classes.</remarks>
        public static int GetHashCode(IMatrix mat)
        {
            if (mat == null)
                return int.MaxValue;
            int dim1 = mat.RowCount;
            int dim2 = mat.ColumnCount;
            if (dim1 < 1 || dim2 < 1)
            {
                return int.MaxValue - 1 - Math.Abs(dim1 * dim2) - Math.Abs(dim2);
            }
            int ret = 0;
            for (int i = 0; i < dim1; i++)
            {
                for (int j = 0; j < dim2; j++)
                {
                    ret ^= mat[i, j].GetHashCode(); ;
                }
            }
            return ret;
        }

        /// <summary>Returns true if the specified matrices are equal, false if not.</summary>
        /// <param name="m1">The first of the two matrices that are checked for equality.</param>
        /// <param name="m2">The second of the two matrices that are checked for equality.</param>
        /// <remarks>
        /// <para>This method should be used when overriding the Equals() method in  matrix classes, 
        /// in order to unify the equality check over different matrix classes.</para>
        /// <para>If both matrices are null or both have one dimension less than 1 and the other dimension the same 
        /// then then they considered equal.</para>
        /// <para>This method is consistent with the <see cref="MatrixBase.Compare"/> method, i.e. it returns the 
        /// same value as the expression <see cref="MatrixBase.Compare"/>(<paramref name="m1"/>, <paramref name="m2"/>==0).</para>
        /// </remarks>
        public static bool Equals(IMatrix m1, IMatrix m2)
        {
            if (m1 == null)
            {
                if (m2 == null)
                    return true;
                else
                    return false;
            }
            else
            {
                int dim1 = m1.RowCount;
                int dim2 = m1.ColumnCount;
                if (m2.RowCount != dim1 || m2.ColumnCount!=dim2)
                    return false;
                else
                {
                    for (int i = 0; i < dim1; i++)
                        for (int j=0; j<dim2; j++)
                    {
                        if (m1[i, j] != m2[i, j])
                            return false;
                    }
                    return true;
                }
            }
        }

        /// <summary>Returns an integer valued hash function of the specified matrix object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionInt"/> method.</para></summary>
        /// <mat>Matrix object whose hash function is calculated and returned.</vec>
        /// <seealso cref="Util.GetHashFunctionInt"/>
        public static int GetHashFunctionInt(IMatrix mat)
        {
            return Util.GetHashFunctionInt(mat);
        }

        /// <summary>Returns a string valued hash function of the specified matrix object.
        /// <para>The returned value is calculated by the <see cref="Util.GetHashFunctionString"/> method.</para></summary>
        /// <mat>Matrix object whose hash function is calculated and returned.</vec>
        /// <seealso cref="Util.GetHashFunctionString"/>
        public static string GetHashFunctionString(IMatrix mat)
        {
            return Util.GetHashFunctionString(mat);
        }


        #endregion Auxiliary


        #endregion StaticOperations



        #region InputOutput



        #region StaticInputOutput



        ///// <summary>Returns string representation of the specified matrix in the standard IGLib form. 
        ///// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        ///// <param name="mat">Matrix whose string representation is returned.</param>
        //public static string ToString(IMatrix mat)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (mat == null)
        //        sb.Append("{}");
        //    else
        //    {
        //        int d1 = mat.RowCount - 1;
        //        int d2 = mat.ColumnCount - 1;
        //        sb.Append('{');
        //        for (int i = 0; i <= d1; ++i)
        //        {
        //            sb.Append("{");
        //            for (int j = 0; j <= d2; ++j)
        //            {
        //                sb.Append(mat[i, j].ToString(null, CultureInfo.InvariantCulture));
        //                if (j < d2)
        //                    sb.Append(", ");
        //            }
        //            sb.Append("}");
        //            if (i < d1)
        //                sb.Append(", ");
        //        }
        //        sb.Append("}");
        //    }
        //    return sb.ToString();
        //}


        /// <summary>Returns a readable string form of a matrix, accuracy and padding can be set.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        /// <param name="accuracy">Accuracy of matrix elments representations.</param>
        /// <param name="padding">Padding of matrix elements.</param>
        /// <returns>A readable string representation in tabular form, with the specified accuracy and padding of elements.</returns>
        public static string ToStringReadable(IMatrix mat, int accuracy = 4, int padding = 8)
        {
            if (mat == null)
                return Util.NullRepresentationString;
            string accStr = "F" + accuracy.ToString();
            StringBuilder sb = new StringBuilder();

            // string s = "";
            for (int i = 0; i < mat.RowCount; ++i)
            {
                for (int j = 0; j < mat.ColumnCount; ++j)
                    sb.Append(mat[i, j].ToString(accStr).PadLeft(padding) + " ");
                sb.AppendLine();
            }
            return sb.ToString();
        }



        /// <summary>Returns a string representation of the specified matrix with newlines inserted after each row.
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToStringNewlines(IMatrix mat)
        {
            StringBuilder sb = new StringBuilder();
            if (mat == null)
                sb.Append("{}");
            else
            {
                int d1 = mat.RowCount - 1;
                int d2 = mat.ColumnCount - 1;
                sb.Append('{' + Environment.NewLine);
                for (int i = 0; i <= d1; ++i)
                {
                    sb.Append("  {");
                    for (int j = 0; j <= d2; ++j)
                    {
                        sb.Append(mat[i, j].ToString(null, CultureInfo.InvariantCulture));
                        if (j < d2)
                            sb.Append(", ");
                    }
                    sb.Append("}");
                    if (i < d1)
                        sb.Append(", " + Environment.NewLine);
                }
                sb.Append("}" + Environment.NewLine);
            }
            return sb.ToString();
        }


        /// <summary>Returns a string representation of the specified matrix with newlines inserted after each row, 
        /// with the specified format for elements of the matrix.
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public static string ToStringNewlines(IMatrix mat, string elementFormat)
        {
            StringBuilder sb = new StringBuilder();
            if (mat == null)
                sb.Append("{}");
            else
            {
                int d1 = mat.RowCount - 1;
                int d2 = mat.ColumnCount - 1;
                sb.Append('{' + Environment.NewLine);
                for (int i = 0; i <= d1; ++i)
                {
                    sb.Append("  {");
                    for (int j = 0; j <= d2; ++j)
                    {
                        sb.Append(mat[i, j].ToString(elementFormat, CultureInfo.InvariantCulture));
                        if (j < d2)
                            sb.Append(", ");
                    }
                    sb.Append("}");
                    if (i < d1)
                        sb.Append(", " + Environment.NewLine);
                }
                sb.Append("}" + Environment.NewLine);
            }
            return sb.ToString();
        }

        /// <summary>Returns string representation of the current matrix in the standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToString(IMatrix mat)
        {
            return ToStringMath(mat);
        }

        /// <summary>Returns string representation of the current matrix in the standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        public static string ToStringMath(IMatrix mat)
        {
            StringBuilder sb = new StringBuilder();
            if (mat == null)
                sb.Append("{}");
            else
            {
                int d1 = mat.RowCount - 1;
                int d2 = mat.ColumnCount - 1;
                sb.Append('{');
                for (int i = 0; i <= d1; ++i)
                {
                    sb.Append("{");
                    for (int j = 0; j <= d2; ++j)
                    {
                        sb.Append(mat[i, j].ToString(null, CultureInfo.InvariantCulture));
                        if (j < d2)
                            sb.Append(", ");
                    }
                    sb.Append("}");
                    if (i < d1)
                        sb.Append(", ");
                }
                sb.Append("}");
            }
            return sb.ToString();
        }

        /// <summary>Returns string representation of the current matrix in the standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the matrix.
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public static string ToString(IMatrix mat, string elementFormat)
        {
            return ToStringMath(mat, elementFormat);
        }

        /// <summary>Returns string representation of the current matrix in the standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the matrix.
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        /// <param name="mat">Matrix whose string representation is returned.</param>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public static string ToStringMath(IMatrix mat, string elementFormat)
        {
            StringBuilder sb = new StringBuilder();
            if (mat == null)
                sb.Append("{}");
            else
            {
                int d1 = mat.RowCount - 1;
                int d2 = mat.ColumnCount - 1;
                sb.Append('{');
                for (int i = 0; i <= d1; ++i)
                {
                    sb.Append("{");
                    for (int j = 0; j <= d2; ++j)
                    {
                        sb.Append(mat[i, j].ToString(elementFormat, CultureInfo.InvariantCulture));
                        if (j < d2)
                            sb.Append(", ");
                    }
                    sb.Append("}");
                    if (i < d1)
                        sb.Append(", ");
                }
                sb.Append("}");
            }
            return sb.ToString();
        }


        /// <summary>Saves (serializes) the specified matrix to the specified JSON file.
        /// File is owerwritten if it exists.</summary>
        /// <param name="mat">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file in which object is is saved.</param>
        public static void SaveJson(IMatrix mat, string filePath)
        {
            SaveJson(mat, filePath, false /* append */ );
        }

        /// <summary>Saves (serializes) the specified matrix to the specified JSON file.</summary>
        /// <param name="mat">Object that is saved to a file.</param>
        /// <param name="filePath">Path to the file in which object is is saved.</param>
        /// <param name="append">Specifies whether serialized data is appended at the end of the file
        /// in the case that the file already exists.</param>
        public static void SaveJson(IMatrix mat, string filePath, bool append)
        {
            MatrixDtoBase dtoOriginal = new MatrixDtoBase();
            dtoOriginal.CopyFrom(mat);
            ISerializer serializer = new SerializerJson();
            serializer.Serialize<MatrixDtoBase>(dtoOriginal, filePath, append);
        }

        /// <summary>Restores (deserializes) a matrix object from the specified file in JSON format.</summary>
        /// <param name="filePath">File from which object data is restored.</param>
        /// <param name="matRestored">Object that is restored by deserialization.</param>
        public static void LoadJson(string filePath, ref IMatrix matRestored)
        {
            ISerializer serializer = new SerializerJson();
            MatrixDtoBase dtoRestored = serializer.DeserializeFile<MatrixDtoBase>(filePath);
            dtoRestored.CopyTo(ref matRestored);
        }


        #endregion StaticInputOutput




        /// <summary>Returns string representation of the current matrix in the standard IGLib form. 
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        public override string ToString()
        {
            return ToString(this);
        }

        /// <summary>Returns a readable an easily string form of a matrix, accuracy and padding can be set.</summary>
        /// <param name="mat">Matrix to be changed to a string.</param>
        /// <param name="accuracy">Accuracy of matrix elments representations.</param>
        /// <param name="padding">Paddind of matrix elements.</param>
        /// <returns>A readable string representation in tabular form.</returns>
        public virtual string ToStringReadable(int accuracy = 4, int padding = 8)
        {
            return MatrixBase.ToStringReadable(this, accuracy, padding);
        }

        /// <summary>Returns a string representation of this matrix with newlines inserted after each row.
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        public virtual string ToStringNewlines()
        {
            return ToStringNewlines(this);
        }

        /// <summary>Returns a string representation of this matrix with newlines inserted after each row, 
        /// with the specified format for elements of the matrix.
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public virtual string ToStringNewlines(string elementFormat)
        {
            return ToStringNewlines(this, elementFormat);
        }


        /// <summary>Returns string representation of the current matrix in the standard IGLib form
        /// (Mathematica-like format but with C representation of numbers).
        /// Rows and elements are printed in comma separated lists in curly brackets.</summary>
        public virtual string ToStringMath()
        {
            return ToStringMath(this);
        }


        /// <summary>Returns a string representation of the current matrix in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the matrix.</summary>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public virtual string ToString(string elementFormat)
        {
            return ToString(this, elementFormat);
        }

        /// <summary>Returns a string representation of the current matrix in a standard IGLib form
        /// (Mathematica-like format but with C representation of numbers), with the specified  
        /// format for elements of the matrix.</summary>
        /// <param name="elementFormat">Format specification for printing individual element.</param>
        public virtual string ToStringMath(string elementFormat)
        {
            return ToStringMath(this, elementFormat);
        }
        

        #endregion InputOutput


        #region Operators_Overloading


        /// <summary>Unary plus, returns the operand.</summary>
        public static MatrixBase operator +(MatrixBase m)
        {
            if (m == null)
                return null;
            else
                return m.GetCopyBase();
        }

        /// <summary>Unary negation, returns the negative operand.</summary>
        public static MatrixBase operator -(MatrixBase m)
        {
            if (m == null)
                return null;
            else
            {
                MatrixBase ret = m.GetCopyBase();
                ret.Negate();
                return ret;
            }
        }

        /// <summary>Matrix addition.</summary>
        public static MatrixBase operator +(MatrixBase a, MatrixBase b)
        {
            MatrixBase ret = a.GetCopyBase();
            Add(a, b, ret);
            return ret;
        }


        /// <summary>Matrix subtraction.</summary>
        public static MatrixBase operator -(MatrixBase a, MatrixBase b)
        {
            MatrixBase ret = a.GetCopyBase();
            Subtract(a, b, ret);
            return ret;
        }



        /// <summary>Product of two matrices.</summary>
        public static MatrixBase operator *(MatrixBase a, MatrixBase b)
        {
            if (a == null)
                return null;
            else if (b == null)
                return null;
            MatrixBase ret = a.GetNewBase(a.RowCount, b.ColumnCount);
            Multiply(a, b, ret);
            return ret;
        }

        /// <summary>Product of a matrix and a vector.</summary>
        public static VectorBase operator *(MatrixBase a, VectorBase b)
        {
            VectorBase ret = b.GetNewBase();
            Multiply(a, b, ret);
            return ret;
        }

        /// <summary>Product of a matrix by a scalar.</summary>
        public static MatrixBase operator *(MatrixBase a, double b)
        {
            MatrixBase ret = a.GetNewBase();
            MatrixBase.Multiply(a, b, ret);
            return ret;
        }

        /// <summary>Product of a matrix by a scalar.</summary>
        public static MatrixBase operator *(double a, MatrixBase b)
        {
            MatrixBase ret = b.GetNewBase();
            Multiply(b, a, ret);
            return ret;
        }

        /// <summary>Matrix subtraction.</summary>
        public static MatrixBase operator /(MatrixBase a, double b)
        {
            MatrixBase ret = a.GetNewBase();
            Divide(a, b, ret);
            return ret;
        }



        #endregion  // Operators_Overloading


        #region Testing


        /// <summary>Checks whether the difference between matrix result of the tested operation and 
        /// some reference result is within the specified tolerance.</summary>
        /// <param name="result">Matrix that is the result of the tested operation.</param>
        /// <param name="referenceResult">Reference result obtained in a different way.</param>
        /// <param name="tolerance">Tolerance on norm of the difference. Must be greater than 0.
        /// If norm is less or equal to the tolerance then the test passes.</param>
        /// <param name="printReports">If true then short reports are printed to console.</param>
        /// <returns>true if norm of the difference between the result matrix and the reference result is less or
        /// equal to tolerance.</returns>
        protected static bool CheckTestResult(IMatrix result, IMatrix referenceResult, double tolerance, bool printReports)
        {
            if (tolerance<=0)
                throw new ArgumentException("Invalid tolerance " + tolerance + ", should not be less or equal to 0!");
            IMatrix diff = null;
            Subtract(result, referenceResult, ref diff);
            double normDiff = diff.NormForbenius;
            bool passed = true;
            if (normDiff <= tolerance)
            {
                if (printReports)
                    Console.WriteLine("Test PASSED. Norm of difference: " + normDiff);
            } else 
            {
                passed = false;
                if (printReports)
                {
                    Console.WriteLine("ERROR: Test NOT passed. Norm of difference: " + normDiff + ", tolerance: " + tolerance);
                    Console.WriteLine("Difference between the result of operation and reference result: ");
                    Console.WriteLine(diff.ToString());
                }
            }
            return passed;
        }

        /// <summary>Checks whether the difference between matrix result of the tested operation and 
        /// some reference result is within the specified tolerance.</summary>
        /// <param name="result">Matrix that is the result of the tested operation.</param>
        /// <param name="referenceResult">Reference result obtained in a different way.</param>
        /// <param name="tolerance">Tolerance on norm of the difference. Must be greater than 0.
        /// If norm is less or equal to the tolerance then the test passes.</param>
        /// <param name="printReports">If true then short reports are printed to console.</param>
        /// <returns>true if norm of the difference between the result matrix and the reference result is less or
        /// equal to tolerance.</returns>
        protected static bool CheckTestResult(IVector result, IVector referenceResult, double tolerance, bool printReports)
        {
            if (tolerance <= 0)
                throw new ArgumentException("Invalid tolerance " + tolerance + ", should not be less or equal to 0!");
            IVector diff = null;
            Vector.Subtract(result, referenceResult, ref diff);
            double normDiff = diff.Norm2;
            bool passed = true;
            if (normDiff <= tolerance)
            {
                if (printReports)
                    Console.WriteLine("Test PASSED. Norm of difference: " + normDiff);
            }
            else
            {
                passed = false;
                if (printReports)
                {
                    Console.WriteLine("ERROR: Test NOT passed. Norm of difference: " + normDiff + ", tolerance: " + tolerance);
                    Console.WriteLine("Difference between the result of operation and reference result: ");
                    Console.WriteLine(diff.ToString());
                }
            }
            return passed;
        }



        /// <summary>Tests various matrix and vector products with fixed tolerance of 1.0E-6.</summary>
        /// <param name="printReports">Specifies whether to print short reports to console or not.</param>
        /// <returns>True if all tests have passed, and false if there is an error.</returns>
        public static bool TestMatrixProducts(bool printReports)
        {
            return TestMatrixProducts(1.0e-6, printReports);
        }

        /// <summary>Tests various matrix and vector products, without printing reports.</summary>
        /// <param name="tolerance">Tolerance for difference between product and test expression below which any individual test passes.
        /// Must be greater than 0.</param>
       /// <returns>True if all tests have passed, and false if there is an error.</returns>
        public static bool TestMatrixProducts(double tolerance)
        {
            return TestMatrixProducts(tolerance, false);
        }

        /// <summary>Tests various matrix and vector products with fixed tolerance of 1.0E-6 and without printing reports.</summary>
        /// <returns>True if all tests have passed, and false if there is an error.</returns>
        public static bool TestMatrixProducts()
        {
            return TestMatrixProducts(1.0e-6, false);
        }

        /// <summary>Tests various matrix and vector products.</summary>
        /// <param name="tolerance">Tolerance for difference between product and test expression below which any individual test passes.
        /// Must be greater than 0.</param>
        /// <param name="printReports">Specifies whether to print short reports to console or not.</param>
        /// <returns>True if all tests have passed, and false if there is an error.</returns>
        public static bool TestMatrixProducts(double tolerance, bool printReports)
        {
            if (tolerance <= 0)
                throw new ArgumentException("Invalid tolerance " + tolerance + ", should not be less or equal to 0!");
            bool passed = true, passedThis;
            int numPassed = 0, numFailed = 0;
            if (printReports)
                Console.WriteLine(Environment.NewLine, "Tests of matrix products:");

            if (printReports)
                Console.WriteLine(Environment.NewLine + "MultiplyTranspMat: ");
            passedThis = TestMultiplyTranspMat(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports) 
                Console.WriteLine(Environment.NewLine + "MultiplyMatTransp: ");
            passedThis = TestMultiplyMatTransp(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports) 
                Console.WriteLine(Environment.NewLine + "MultiplyTranspTransp: ");
            passedThis = TestMultiplyTranspTransp(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports) 
                Console.WriteLine(Environment.NewLine + "Multiply3: ");
            passedThis = TestMultiply3(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports)
                Console.WriteLine(Environment.NewLine + "MultiplyTranspMatTransp: ");
            passedThis = TestMultiplyTranspMatTransp(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports)
                Console.WriteLine(Environment.NewLine + "MultiplyTranspMatMat: ");
            passedThis = TestMultiplyTranspMatMat(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports)
                Console.WriteLine(Environment.NewLine + "MultiplyMatMatTransp: ");
            passedThis = TestMultiplyMatMatTransp(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports)
                Console.WriteLine(Environment.NewLine + "MultiplyMatTranspMat: ");
            passedThis = TestMultiplyMatTranspMat(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports)
                Console.WriteLine(Environment.NewLine + "MultiplyTranspTranspTransp: ");
            passedThis = TestMultiplyTranspTranspTransp(tolerance, printReports);
            passed = passed && passedThis;
            if (passedThis) ++numPassed; else ++numFailed;

            if (printReports)
            {
                if (passed)
                    Console.WriteLine(Environment.NewLine + "All tests have passed." + Environment.NewLine);
                else
                    Console.WriteLine(Environment.NewLine + "WARNING: Some tests have NOT PASSED (" 
                        +  numFailed + " of " + (numPassed +  numFailed) + " failed)." + Environment.NewLine);
            }
            return passed;
        }

         
        /// <summary>A test method, just prints some output.</summary>
        public static void TestStaticMethodCommon()
        {
            Console.WriteLine("TestStaticMethodCommon from the MatrixBase class.");
            TestStaticMethodSpecific();
        }

        /// <summary>A test method, just prints some output.</summary>
        public static void TestStaticMethodSpecific()
        {
            Console.WriteLine("TestStaticMethod from MatrixBase.");
        }

        /// <summary>Performs test of converson between double indexing and flat indexing of matric elements,.
        /// Returns true if successful, false othwrwise.</summary>
        /// <param name="dim1">Number of rows.</param>
        /// <param name="dim2">Number of columns.</param>
        /// <returns>true if successful, false if not.</returns>
        public static bool TestIndices(int dim1 = 3, int dim2 = 4)
        {
            Console.WriteLine(Environment.NewLine + "Test of matrix index conversions ...");
            bool ret = true;
            IMatrix mat = new Matrix(3, 4);
            MatrixBase.SetRandom(mat);
            int iFlat = 0;
            for (int row = 0; row < mat.RowCount; ++row)
                for (int col = 0; col < mat.ColumnCount; ++col)
                {
                    double element = mat[row, col];
                    int flatIndex = MatrixBase.Index(mat, row, col);
                    double elementFlat = mat[flatIndex];
                    int rowCalc, colCalc;
                    MatrixBase.Indices(mat, flatIndex, out rowCalc, out colCalc);
                    Console.WriteLine(row + ", " + col + ": flat = " + flatIndex + ", restored = ("
                        + rowCalc + ", " + colCalc + "), el. = " + elementFlat);
                    if (flatIndex != iFlat)
                    {
                        ret = false;
                        Console.WriteLine("  Error (" + row + ", " + col + "): Calculated flat index (" + flatIndex 
                            + ") different from actual one (" + flatIndex + ").");
                    }
                    if (rowCalc != row || colCalc != col)
                    {
                        ret = false;
                        Console.WriteLine("  Error (" + row + ", " + col + "): Calculated indices from flat form (" 
                            + rowCalc + ", " + colCalc + ") don't match original indices.");
                    }
                    if (flatIndex != iFlat)
                    {
                        ret = false;
                        Console.WriteLine("  Error (" + row + ", " + col + "): Calculated flat index (" + flatIndex
                            + ") different from actual one (" + flatIndex + ").");
                    }
                    ++ iFlat;
                }
            Console.WriteLine("... test done " + (ret?"successfully.":" with ERRORS.") + Environment.NewLine);
            return ret;
        }


        #endregion Testing


    }  // abstract class MatrixBase


    /// <summary>Matrix store.
    /// <para>Stores matrix objects for reuse.</para>
    /// <para>Can be used for storage fo matrices with specific dimension (default) or for torage of any non-null matrices.</para></summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MatrixStore<T> : ObjectStore<T>, ILockable
        where T: class, IMatrix
    {

        /// <summary>Constructs a new matrix store of unspecified dimensions.</summary>
        protected MatrixStore(): base()
        {  }

        /// <summary>Constructs a new matrix store for matrices with the specified dimensions.</summary>
        /// <param name="rowCount">Number of rows of stored matrices.</param>
        /// <param name="columnCount">Number of columns of stored matrices.</param>
        public MatrixStore(int rowCount, int columnCount)
            : this()
        {
            this._rowCount = rowCount;
            this._columnCount = columnCount;
        }

        /// <summary>Constructs a new matrix store for matrices with the specified dimensions.
        /// <para>If <param name="constrainDimensions"></param> is false then store can be used for matrices with any dimensions.</para></summary>
        /// <param name="rowCount">Number of rows of stored matrices.</param>
        /// <param name="columnCount">Number of columns of stored matrices.</param>
        public MatrixStore(int rowCount, int columnCount, bool constrainDimensions)
            : this(rowCount, columnCount)
        {
            this._constrainDimensions = constrainDimensions;
        }

        /// <summary>Ilf true then only matrices with matching dimensions are eligible for storing.
        /// <para>Default is true.</para></summary>
        protected bool _constrainDimensions = true;

        /// <summary>Ilf true then only matrices with matching dimensions are eligible for storing.</summary>
        public bool ConstrainDimensions
        {
            get { lock (Lock) { return _constrainDimensions; } }
            set { lock (Lock) { 
                bool checkEligible = false;
                if (_constrainDimensions == false && value == true)
                    checkEligible = true;
                _constrainDimensions = value;
                if (checkEligible)
                    ClearIneligible();
            } }
        }

        protected int _rowCount, _columnCount;

        /// <summary>Number of rows for matrices to be stored.
        /// <para>If <see cref="ConstrainDimensions"/> is true then only matrices that match dimensions are eligible for storage.
        /// Otherwise, dimensions are only important for creation of new matrices.</para></summary>
        public int RowCount
        {
            get { lock (Lock) { return RowCount; } }
            set 
            {
                lock (Lock)
                {
                    bool checkEligible = false;
                    if (value != _rowCount && _constrainDimensions)
                        checkEligible = true;
                    _rowCount = value;
                    if (checkEligible)
                        ClearIneligible();
                }
            }
        }

        /// <summary>Number of columns for matrices to be stored.
        /// <para>If <see cref="ConstrainDimensions"/> is true then only matrices that match dimensions are eligible for storage.
        /// Otherwise, dimensions are only important for creation of new matrices.</para></summary>
        public int ColumnCount
        {
            get { lock (Lock) { return _columnCount; } }
            set
            {
                lock (Lock)
                {
                    bool checkEligible = false;
                    if (value != _columnCount && _constrainDimensions)
                        checkEligible = true;
                    _columnCount = value;
                    if (checkEligible)
                        ClearIneligible();
                }
            }
        }

        /// <summary>Returns true if the specified matrix is eligible for storage in the current store, false if not.</summary>
        /// <param name="mat">Matrix whose eligibility is checked.</param>
        public override bool IsEligible(T mat)
        {
            lock (Lock)
            {
                bool ret = true;
                if (_constrainDimensions)
                {
                    if (mat == null)
                        ret = false;
                    else if (mat.RowCount != _rowCount || mat.ColumnCount != _columnCount)
                        ret = false;
                }
                return ret;
            }
        }

    }  // MatrixStore<T>


    /// <summary>Matrix store.
    /// <para>Stores matrix objects for reuse.</para>
    /// <para>Can be used for storage fo matrices with specific dimension (default) or for torage of any non-null matrices.</para></summary>
    public class MatrixStore : MatrixStore<Matrix>, ILockable
    {

        /// <summary>Constructs a new matrix store of unspecified dimensions.</summary>
        protected MatrixStore()
            : base()
        { }

        /// <summary>Constructs a new matrix store for matrices with the specified dimensions.</summary>
        /// <param name="rowCount">Number of rows of stored matrices.</param>
        /// <param name="columnCount">Number of columns of stored matrices.</param>
        public MatrixStore(int rowCount, int columnCount)
            : base(rowCount, columnCount)
        {  }

        /// <summary>Constructs a new matrix store for matrices with the specified dimensions.
        /// <para>If <param name="constrainDimensions"></param> is false then store can be used for matrices with any dimensions.</para></summary>
        /// <param name="rowCount">Number of rows of stored matrices.</param>
        /// <param name="columnCount">Number of columns of stored matrices.</param>
        public MatrixStore(int rowCount, int columnCount, bool constrainDimensions)
            : base(rowCount, columnCount, constrainDimensions)
        {  }

        /// <summary>Returns a newly created object eligible for storage, or null if such an object
        /// can not be created. This method should not throw an exception.</summary>
        protected override Matrix TryGetNew()
        {
            if (_rowCount > 0 && _columnCount > 0)
            {
                return new Matrix(_rowCount, _columnCount);
            }
            else
                return null;
        }

    } // class MatrixStore


} 


