// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

// CLASSES FOR DATA TRANSFER OBJECTS (DTO) THAT FACILITATE SERIALIZATION OF MATRIX OBJECTS.

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using IG.Num;
using System.Collections.Generic;

namespace IG.Lib
{


    #region Matrices

    /// <summary>Base class for various matrix DTO (Data Transfer Objects) for matrixs.
    /// Used to store a state of a matrix.</summary>
    /// <typeparam name="MatrixType">Type parameter specifying the specific matrix type for which concrete DTO
    /// is designed.</typeparam>
    /// $A Igor Jun09;
    public abstract class MatrixDtoBase<MatrixType> : SerializationDtoBase<MatrixType, IMatrix>
        where MatrixType : class, IMatrix
    {

        #region Construction

        /// <summary>Default constructor, sets IsNull to true.</summary>
        public MatrixDtoBase()
            : base()
        { }

        /// <summary>Constructor, prepares the current DTO for storing a matrix of the specified dimension.</summary>
        /// <param name="rowCnt">First dimension (number of rows) of a matrix that is stored in the current DTO.</param>
        /// <param name="columnCnt">Second dimension (number of columns) of a matrix that is stored in the current DTO.</param>
        public MatrixDtoBase(int rowCnt, int columnCnt)
            : this()
        {
            this._rowCount = rowCnt;
            this._columnCount = columnCnt;
            AllocateComponents(rowCnt, columnCnt);
        }

        #endregion Construction

        #region Data

        protected int _rowCount;
        protected int _columnCount;

        protected double[][] _elements;

        /// <summary>Allocates the array that stores matrix elements.</summary>
        /// <param name="rowCnt">Number of rows of the matrix.</param>
        /// <param name="columnCnt">Number of columns of the matrix.</param>
        protected virtual void AllocateComponents(int rowCnt, int columnCnt)
        {
            this._rowCount = rowCnt;
            this._columnCount = columnCnt;
            if (rowCnt <= 0 || columnCnt <= 0)
                this.SetNull(true);
            else
                this.SetNull(false);
            int dim1 = 0;
            int dim2 = 0;
            if (_elements != null)
            {
                dim1 = _elements.Length;
                if (_elements[0] != null)
                    dim2 = _elements[0].Length;
            }
            if (dim1 != rowCnt || dim2 != columnCnt)
            {
                if (rowCnt <= 0 || columnCnt <= 0)
                {
                    _elements = null;
                }
                else
                {
                    _elements = new double[rowCnt][];
                    for (int i = 0; i < rowCnt; ++i)
                        _elements[i] = new double[columnCnt];
                }
            }
        }

        /// <summary>Number of rows of the matrix.</summary>
        public virtual int RowCount
        {
            get { return _rowCount; }
            set
            {
                // Make elements consistent with the new dimension:
                AllocateComponents(value, _columnCount);
                _rowCount = value;
            }
        }

        /// <summary>Number of columns of the matrix.</summary>
        public virtual int ColumnCount
        {
            get { return _columnCount; }
            set
            {
                // Make elements consistent with the new dimension:
                AllocateComponents(_rowCount, value);
                _columnCount = value;
            }
        }

        /// <summary>Matrix elements.</summary>
        public double[][] Components
        {
            get { return _elements; }
            set
            {
                _elements = value;
                // Make dimension consistent with new array of elements:
                if (value == null)
                {
                    _rowCount = 0;
                    _columnCount = 0;
                }
                else
                {
                    _rowCount = value.Length;
                    if (value[0] != null)
                        _columnCount = value[0].Length;
                    else
                        _columnCount = 0;
                }
            }
        }

        #endregion Data


        #region Operation

        /// <summary>Creates and returns a new matrix of the specified dimension.</summary>
        /// <param name="rowCnt">Number of rows of the matrix.</param>
        /// <param name="columnCnt">Number of columns of the matrix.</param>
        public abstract MatrixType CreateMatrix(int rowCnt, int columnCnt);

        /// <summary>Creates and returns a new matrix of the specified type and dimension.</summary>
        public override MatrixType CreateObject()
        {
            return CreateMatrix(this.RowCount, this.ColumnCount);
        }

        /// <summary>Copies data to the current DTO from a matrix object.</summary>
        /// <param name="mat">Matrix object from which data is copied.</param>
        protected override void CopyFromPlain(IMatrix mat)
        {
            if (mat == null)
            {
                _rowCount = _columnCount = 0;
                SetNull(true);
            }
            else
            {
                AllocateComponents(mat.RowCount, mat.ColumnCount);
                for (int i = 0; i < mat.RowCount; ++i)
                    for (int j = 0; j < mat.ColumnCount; ++j)
                        this._elements[i][j] = mat[i, j];
            }
        }

        /// <summary>Copies data from the current DTO to a matrix object.</summary>
        /// <param name="mat">Matrix object that data is copied to.</param>
        protected override void CopyToPlain(ref IMatrix mat)
        {
            if (GetNull())
                mat = null;
            else
            {
                if (mat == null)
                    mat = CreateObject();
                else if (mat.RowCount != this.RowCount || mat.ColumnCount != this.ColumnCount)
                    mat = CreateObject();
                for (int i = 0; i < RowCount; ++i)
                    for (int j = 0; j < ColumnCount; ++j)
                        mat[i, j] = this.Components[i][j];

            }
        }

        #endregion Operation

        #region Misc

        ///// <summary>Creates and returns a string representation of the curren matrix DTO.</summary>
        //public override string ToString()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.AppendLine("MatrixDTO, tpe = " + typeof(MatrixType).ToString() + ": ");
        //    sb.AppendLine("  Dimensions: " + RowCount + "*" + ColumnCount + "  ");
        //    sb.AppendLine("  Components: ");
        //    if (Components == null)
        //        sb.AppendLine("  null");
        //    else
        //    {
        //        sb.Append("{");
        //        for (int i = 0; i < RowCount; ++i)
        //        {
        //            if (Components[i] == null)
        //                sb.Append("  null");
        //            else
        //            {
        //                sb.Append("  {");
        //                for (int j = 0; j < ColumnCount; ++j)
        //                {
        //                    sb.Append(Components[i][j].ToString());
        //                    if (j == ColumnCount - 1)
        //                        sb.Append(", ");
        //                }
        //                sb.Append("}");
        //            }
        //            if (i == RowCount - 1) // last row, no comma
        //                sb.AppendLine(" ");
        //            else
        //                sb.AppendLine(", ");
        //        }
        //        sb.AppendLine("}");
        //    }
        //    return sb.ToString();
        //}

        #endregion Misc


    }  // abstract class MatrixDtoBase

    /// <summary>DTO (data transfer object) for matrix interface (IMatrix).</summary>
    /// $A Igor Jun09;
    public class MatrixDtoBase : MatrixDtoBase<IMatrix>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a matrix object of any matrix type</summary>
        public MatrixDtoBase()
            : base()
        { }

        /// <summary>Creates a DTO for storing a matrix object of any matrix type, with specified dimensions.</summary>
        /// <param name="rowCnt">First dimension of the matrix (number of rows).</param>
        /// <param name="columnCnt">Second dimension of the matrix (number of columns).</param>
        public MatrixDtoBase(int rowCnt, int columnCnt)
            : base(rowCnt, columnCnt)
        { }

        #endregion Construction

        /// <summary>Creates and returns a new matrix cast to the interface type IMatrix.</summary>
        /// <param name="rowCnt">First dimension of the matrix (number of rows).</param>
        /// <param name="columnCnt">Second dimension of the matrix (number of columns).</param>
        public override IMatrix CreateMatrix(int rowCnt, int columnCnt)
        {
            return new Matrix(rowCnt, columnCnt);
        }

    } // class MatrixDtoBase


    /// <summary>Data Transfer Object (DTO) for matrixs of type IG.Num.Matrix.
    /// Used to store, transfer, serialize and deserialize objects of type IMatrix.</summary>
    /// $A Igor Aug09;
    public class MatrixDto : MatrixDtoBase<Matrix>
    {

        #region Construction

        /// <summary>Creates a DTO for storing state of a matrix object of any matrix type</summary>
        public MatrixDto()
            : base()
        { }


        /// <summary>Creates a DTO for storing a matrix object of the Matrix type, with specified dimension.</summary>
        /// <param name="rowCnt">First dimension of the matrix (number of rows).</param>
        /// <param name="columnCnt">Second dimension of the matrix (number of columns).</param>
        public MatrixDto(int rowCnt, int columnCnt)
            : base(rowCnt, columnCnt)
        { }

        #endregion Construction

        /// <summary>Creates and returns a new matrix of the specified dimension.</summary>
        /// <param name="rowCnt">First dimension of the matrix (number of rows).</param>
        /// <param name="columnCnt">Second dimension of the matrix (number of columns).</param>
        public override Matrix CreateMatrix(int rowCnt, int columnCnt)
        {
            return new Matrix(rowCnt, columnCnt);
        }

    }

    #endregion Matrices

}