// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using IG.Num;

namespace IG.Lib
{


    /// <summary>Memory representation of CSV data.</summary>
    /// <remarks>Currently, this is just an exact copy of the <see cref="StringTable"/> class.
    /// <para>Data access operations are thread safe.</para></remarks>
    /// $A Igor xx;
    public class CsvData : StringTable, ILockable
    {
        
        /// <summary>Constructs a new string table, a data structure compatible with CSV file format.</summary>
        /// <param name="readOnly">Whether the created object is read only or not.</param>
        public CsvData(bool readOnly): this()
        {
            this.IsReadOnly = readOnly;
        }

       /// <summary>Constructs a new string table, a data structure compatible with CSV file format.</summary>
        public CsvData()
        {
        }

    }

    /// <summary>2D tables of data represented by strings. Maps to CSV files.</summary>
    /// <remarks>Data access operations are thread safe.</remarks>
    /// $A Igor xx;
    public class StringTable: ILockable
    {

        /// <summary>Constructs a new string table, a data structure compatible with CSV file format.</summary>
        /// <param name="readOnly">Whether the created object is read only or not.</param>
        public StringTable(bool readOnly): this()
        {
            this.IsReadOnly = readOnly;
        }

       /// <summary>Constructs a new string table, a data structure compatible with CSV file format.</summary>
        public StringTable()
        {
        }


        #region ILockable

        protected readonly object _lock = new object();

        /// <summary>Object used for thread locking.</summary>
        public object Lock
        {
            get{ return _lock; }
        }

        #endregion ILockable

        /// <summary>Dafault output level for object of the <see cref="StringTable"/> and derived classes.</summary>
        public static int DefaultOutputLevel = 0;

        protected int _outputLevel = DefaultOutputLevel;

        /// <summary>Output level, specifies the level of descriptive output on console during the operation.</summary>
        public int OutputLevel
        {
            get { return _outputLevel; }
            set { _outputLevel = value; }
        }

        #region Data


        protected List<List<string>> _data = new List<List<string>>();

        /// <summary>Data behind the table - list of lists of strings.</summary>
        protected List<List<string>> Data
        {
            get { return _data; }
           //  set { _data = value; }
        }
        
        protected bool _isReadOnly = false;

        /// <summary>Whether or not data table is read only.</summary>
        public bool IsReadOnly
        {
            get { lock(Lock) { return _isReadOnly; } }
            set { lock(Lock) { _isReadOnly = value; } }
        }

        protected bool _isAutoExtend = true;

        /// <summary>Whether or not data storage automatically extends when a value is set on
        /// the position that is out of range.</summary>
        public bool IsAutoExtend {
            get { lock (Lock) { return _isAutoExtend; } }
            set { lock (Lock) { _isAutoExtend = value; } }
        }

        /// <summary>Index operator, gets or sets the specific element of the data table specified by row and column number.
        /// <para>Handling situation when row or column number is out of range:</para>
        /// <para>If <see cref="IsAutoExtend"/> then getter returns null and setter extends the table as needed.</para>
        /// <para>Otherwise, <see cref="IndexOutOfRangeException"/> exception is thrown.</para></summary>
        /// <param name="rowNumber">Row number of the element (zero-based).</param>
        /// <param name="columnNumber">Column number of the element (zero-based).</param>
        public string this[int rowNumber, int columnNumber]
        {
            get
            {
                lock (Lock)
                {
                    if (rowNumber < 0)
                        throw new IndexOutOfRangeException("Row number less thatn zero passed to indexer.");
                    int numRows = _data.Count;
                    if (rowNumber >= numRows)
                    {
                        if (_isAutoExtend)
                            return null;
                        else
                            throw new IndexOutOfRangeException("Row number " + rowNumber + " out of range, should be less than " + numRows);
                    }
                    List<string> row = _data[rowNumber];
                    if (row == null)
                    {
                        if (_isAutoExtend)
                        {
                            return null;
                        }
                        else
                            throw new IndexOutOfRangeException("Row No. " + rowNumber + " has no elements.");
                    }
                    if (columnNumber < 0)
                        throw new IndexOutOfRangeException("Column number less than zero passed to indexer.");
                    else if (columnNumber >= row.Count)
                    {
                        if (_isAutoExtend)
                            return null;
                        else
                            throw new IndexOutOfRangeException("Columnt number " + columnNumber + " out of range, should be less than " + row.Count);
                    }
                    else
                        return row[columnNumber];
                }
            }
            set
            {
                lock (Lock)
                {
                    if (rowNumber < 0)
                        throw new IndexOutOfRangeException("Row number less thatn zero passed to indexer.");
                    int numRows = _data.Count;
                    if (rowNumber >= numRows)
                    {
                        if (_isAutoExtend)
                        {
                            for (int i = numRows + 1; i <= rowNumber + 1; ++i)
                                _data.Add(new List<string>());
                            numRows = _data.Count;
                        }
                        else
                            throw new IndexOutOfRangeException("Row number " + rowNumber + " out of range, should be less than " + numRows);
                    }
                    List<string> row = _data[rowNumber];
                    if (row == null)
                    {
                        if (_isAutoExtend)
                        {
                            row = _data[rowNumber] = new List<string>();
                        }
                        else
                            throw new IndexOutOfRangeException("Row No. " + rowNumber + " has no elements.");
                    }
                    if (columnNumber < 0)
                        throw new IndexOutOfRangeException("Column number less than zero passed to indexer.");
                    else if (columnNumber >= row.Count)
                    {
                        int numColumns = row.Count;
                        if (_isAutoExtend)
                        {
                            for (int i = numColumns + 1; i <= columnNumber + 1; ++i)
                            {
                                row.Add(null);
                            }
                        }
                        else
                            throw new IndexOutOfRangeException("Columnt number " + columnNumber + " out of range, should be less than " + row.Count);
                    }
                    _data[rowNumber][columnNumber] = value;
                }
            }
        }

        /// <summary>Returns a flag telling whether the specified element is defined (it exists in the data table) or not.
        /// <para>If the specified element is null, true is returned. Use <see cref="IsNotNullOrEmpty"/> method to check also
        /// if the element is not null or empty string.</para></summary>
        /// <param name="rowNumber">Row number of the element checked.</param>
        /// <param name="columnNumber">Column number of the element checked.</param>
        public bool IsDefined(int rowNumber, int columnNumber)
        {
            lock (Lock)
            {
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return false;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return false;
                else if (columnNumber >= row.Count)
                    return false;
                else
                    return true;
            }
        }


        /// <summary>Returns a flag telling whether the specified element is defined (it exists in the data table)
        /// and is at the same time not null or empty string.</summary>
        /// <param name="rowNumber">Row number of the element checked.</param>
        /// <param name="columnNumber">Column number of the element checked.</param>
        public bool IsNotNullOrEmpty(int rowNumber, int columnNumber)
        {
            lock (Lock)
            {
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return false;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return false;
                else if (columnNumber >= row.Count)
                    return false;
                else if (string.IsNullOrEmpty(_data[rowNumber][columnNumber]))
                    return false;
                else
                    return true;
            }
        }

        /// <summary>Returns the specified element on the data table or null if that element is not defined,
        /// and notifies the caller through <paramref name="isDefined"/> whether the element is defined or not.</summary>
        /// <param name="rowNumber">Row number of the element checked.</param>
        /// <param name="columnNumber">Column number of the element checked.</param>
        /// <param name="isDefined">Specifies, on return, whether the specified element is defined (it exists in the data table).</param>
        public string GetElementOrNull(int rowNumber, int columnNumber, out bool isDefined)
        {
            lock (Lock)
            {
                isDefined = false;
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return null;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return null;
                else if (columnNumber >= row.Count)
                    return null;
                else
                {
                    isDefined = true;
                    return _data[rowNumber][columnNumber];
                }
            }
        }

        /// <summary>Returns the specified element on the data table or null if that element is not defined.
        /// <para>Use another overload to also notify the caller whether the element is defined or not.</para></summary>
        /// <param name="rowNumber">Row number of the element checked.</param>
        /// <param name="columnNumber">Column number of the element checked.</param>
        public string GetElementOrNull(int rowNumber, int columnNumber)
        {
            bool isDefined;
            return GetElementOrNull(rowNumber, columnNumber, out isDefined);
        }

        /// <summary>Returns index of the first non-empty row from the specified row on (inclusively),
        /// or -1 if there is no such row.</summary>
        /// <param name="startRow">Row where search starts.</param>
        public int FirstNonemptyRow(int startRow)
        {
            lock (Lock)
            {
                int numRows = _data.Count;
                for (int i = startRow; i < numRows; ++i)
                {
                    if (_data[i].Count > 0)
                        return i;
                }
                return -1;
            }
        }

        /// <summary>Returns index of the first non-empty row of the table, or -1 if there is no such row.</summary>
        public int FirstNonemptyRow()
        {
            return FirstNonemptyRow(0 /* startRow */);
        }

        /// <summary>Returns number of the first non-empty cell in the specified row,
        /// from the specified starting column on (inclusively), or -1 if there is no suuch column.</summary>
        /// <param name="rowNum">Index of row in which nonemnty cell is searched for.</param>
        /// <param name="startColumn">Starting column from which on (inclusively) a nonempty cell is searched.</param>
        public int FirstNonemptyColumn(int rowNum, int startColumn)
        {
            lock (Lock)
            {
                if (rowNum >= _data.Count)
                    return -1;
                int numColuns = _data[rowNum].Count;
                for (int i = startColumn; i < numColuns; ++i)
                {
                    if (!string.IsNullOrEmpty(_data[rowNum][i]))
                        return i;
                }
                return -1;
            }
        }

        /// <summary>Returns number of the first non-empty cell in the specified row.</summary>
        /// <param name="rowNum">Index of row in which nonemnty cell is searched for.</param>
        public int FirstNonemptyColumn(int rowNum)
        {
            return FirstNonemptyColumn(rowNum, 0 /* stattColumn */);
        }

        /// <summary>Clears the data table.</summary>
        public void Clear()
        {
            lock (Lock)
            {
                if (_isReadOnly)
                    throw new InvalidOperationException("Data table is read only, can not be cleared.");
                _data.Clear();
            }
        }

        /// <summary>Adds a new row at the end of the data table.</summary>
        /// <remarks>Throws exception if the data table is read only.</remarks>
        public void AddRow()
        {
            lock (Lock)
            {
                if (_isReadOnly)
                    throw new InvalidOperationException("Data table is read only, rows can not be added.");
                _data.Add(new List<string>());
            }
        }

        /// <summary>Adds a new element at the end of the specified row of te data table.</summary>
        /// <param name="rowNum">Sequential number of the row to which the element is added.</param>
        /// <param name="value">Value of the element that is added.</param>
        /// <remarks>Throws exception if the data table is read only, or if the specified row 
        /// does not exist and the data table is not extensible.</remarks>
        public void AddElement(int rowNum, string value)
        {
            lock (Lock)
            {
                if (_isReadOnly)
                    throw new InvalidOperationException("Data table is read only, elements can not be added.");
                if (NumRows <= rowNum)
                    SetNumRows(rowNum + 1);
                _data[rowNum].Add(value);
            }
        }


        /// <summary>Adds the specified elements at the end of the specified row of te data table.</summary>
        /// <param name="rowNum">Sequential number of the row to which the elements are added.</param>
        /// <param name="values">Array of values of the elements that are added.</param>
        /// <remarks>Throws exception if the data table is read only, or if the specified row 
        /// does not exist and the data table is not extensible.</remarks>
        public void AddElements(int rowNum, params string[] values)
        {
            if (values != null)
            {
                int num = values.Length;
                for (int i = 0; i < num; ++i)
                {
                    AddElement(rowNum, values[i]);
                }
            }
        }

        /// <summary>Change the number of rows in the data table to the specified number.</summary>
        /// <param name="numRows">New number of rows.</param>
        /// <remarks>Throws exception if the data table is read only.
        /// <para>If the current number of rows is smaller than the one specified, then new empty (but allocated) rows are added.
        /// If the number is greater then the redundant rows are removed.</para></remarks>
        public void SetNumRows(int numRows)
        {
            lock (Lock)
            {
                if (_isReadOnly)
                    throw new InvalidOperationException("Data table is read only, number of rows can not be set.");
                int numRowsOld = _data.Count;
                Util.ResizeList(ref _data, numRows, null);
                for (int i = numRowsOld; i < numRows; ++i)
                {
                    if (_data[i] == null)
                        _data[i] = new List<string>();
                }
            }
        }


        /// <summary>Changes the number of columns of the specified row to the specified number.</summary>
        /// <param name="rowNum">Row number where number of columns is changed.</param>
        /// <param name="numColumns">New numbef of columns in the specified row.</param>
        /// <remarks>Throws exception if the data table is read only.
        /// <para>If the current number of rows is smaller than the specified row number, then new empty (but allocated) rows are added.
        /// Cells that are eventually added are set to null.</para></remarks>
        public void SetNumColumns(int rowNum, int numColumns)
        {
            lock (Lock)
            {
                if (_isReadOnly)
                    throw new InvalidOperationException("Data table is read only, number of rows can not be set.");
                if (rowNum >= NumRows)
                {
                    SetNumRows(rowNum + 1);
                }
                List<string> row = _data[rowNum];
                Util.ResizeList(ref row, numColumns, null);
                _data[rowNum] = row;
            }
        }


        /// <summary>Clears the specified row in the data table.</summary>
        /// <param name="rowNum">Number of the row to be cleared.</param>
        /// <remarks>Throws exception if the data table is read only, or if the row number is out of range and
        /// the data table is not automatically extendable.</remarks>
        public void ClearRow(int rowNum)
        {
            lock (Lock)
            {
                if (_isReadOnly)
                    throw new InvalidOperationException("Data table is read only, rows can not be cleared.");
                if (rowNum >= NumRows)
                {
                    if (_isAutoExtend)
                        throw new InvalidOperationException("Row number " + rowNum + " is out of range.");
                } else
                {
                    _data[rowNum].Clear();
                }
            }
        }

        /// <summary>Gets or sets copy of the data table in form of 2D jagged array.</summary>
        public string[][] Table
        {
            get
            {
                lock (Lock)
                {
                    int numRows = _data.Count;
                    if (numRows < 1)
                        return null;
                    else
                    {
                        string[][] ret = new string[numRows][];
                        for (int i = 0; i < numRows; ++i)
                        {
                            if (_data[i] == null)
                                ret[i] = null;
                            else
                            {
                                int numColumns = _data[i].Count;
                                string[] row = new string[numColumns];
                                ret[i] = row;
                                for (int j = 0; j < numColumns; ++j)
                                    row[j] = _data[i][j];
                            }
                        }
                        return ret;
                    }
                }
            }
            set
            {
                lock (Lock)
                {
                    if (_isReadOnly)
                        throw new InvalidOperationException("Data table is read only.");
                    _data.Clear();
                    if (value != null)
                    {
                        int numRows = value.Length;
                        for (int i = 0; i < numRows; ++i)
                        {
                            string[] tableRow = value[i];
                            if (tableRow == null)
                                _data.Add(null);
                            else
                            {

                                int numColumns = tableRow.Length;
                                List<string> row = new List<string>();
                                _data.Add(row);
                                for (int j = 0; j < numColumns; ++j)
                                {
                                    row.Add(tableRow[j]);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Gets number of rows in the data table.</summary>
        public int NumRows
        {
            get
            {
                lock (Lock)
                {
                    return _data.Count;
                }
            }
        }

        /// <summary>Returns the maximal number of columns in any row.</summary>
        public int MaxNumColumns
        {
            get
            {
                lock (Lock)
                {
                    int ret = 0;
                    int numRows = _data.Count;
                    for (int i = 0; i < numRows; ++i)
                    {
                        int numColumns = 0;
                        if (_data[i] != null)
                            numColumns = _data[i].Count;
                        if (numColumns > ret)
                            ret = numColumns;
                    }
                    return ret;
                }
            }
        }

        /// <summary>Returns the number of elements (columns) of the specified row.</summary>
        /// <param name="rowNum">Specifies for which row number of columns is returned.</param>
        public int NumColumns(int rowNum)
        {
            lock (Lock)
            {
                if (rowNum >= _data.Count)
                {
                    if (_data.Count == 0)
                        throw new ArgumentException("Row number " + rowNum + " out of range, table contains no rows.");
                    else
                        throw new ArgumentException("Row number " + rowNum + " out of range, should be less than " + _data.Count);
                }
                return _data[rowNum].Count;
            }
        }

        /// <summary>Returns true if data table is rectangular (i.e. all rows have equal number of elements) or not.
        /// <para>Table without rows or with one row is considered rectangular.</para>
        /// <para>Table with all rows empty or null is considered rectangular.</para></summary>
        public bool IsRectangular
        {
            get
            {
                lock (Lock)
                {
                    if (_data.Count <= 0)
                        return true;
                    int numRows = _data.Count;
                    int numColumnsFirstRow = 0;
                    if (_data[0] != null)
                        numColumnsFirstRow = _data[0].Count;
                    for (int i = 1; i < numRows; ++i)
                    {
                        int numColumns = 0;
                        if (_data[i] != null)
                            numColumns = _data[i].Count;
                        if (numColumns != numColumnsFirstRow)
                            return false;
                    }
                    return true;
                }
            }
        }

        #endregion Data


        #region Data.Int

        /// <summary>Returns a flag specified whether the specified element of the data table exists and 
        /// represents an integer.
        /// <para>If the element does not exist then false is returned.</para></summary>
        /// <param name="rowNumber">Row number of the data table element.</param>
        /// <param name="columnNumber">Column number of the data table element.</param>
        public bool IsInt(int rowNumber, int columnNumber)
        {
            lock (Lock)
            {
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return false;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return false;
                else if (columnNumber >= row.Count)
                    return false;
                else
                {
                    string valueStr = _data[rowNumber][columnNumber];
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        int value;
                        bool isInt = int.TryParse(valueStr, out value);
                        return isInt;
                    }
                    else
                        return false;
                }
            }
        }

        /// <summary>Returns an integer value of the element at the specified position of the data table,
        /// if it is defined, or throws an exception.</summary>
        /// <param name="rowNumber">Row number of the data table element.</param>
        /// <param name="columnNumber">Column number of the data table element.</param>
        /// <returns>Integer value of the specified element, if the element exists and can be converted to an integer
        /// (otherwise, an exception is thrown).</returns>
        public int GetInt(int rowNumber, int columnNumber)
        {
            lock (Lock)
            {
                int value;
                bool isInt = TryGetInt(rowNumber, columnNumber, out value);
                if (isInt)
                {
                    return value;
                }
                else
                {
                    bool isElementDefined, isElementNotNullOrEmpty;
                    // The specified table position does not contain an integer, call another method to get
                    // more detailed error information:
                    GetIntSafe(rowNumber, columnNumber, out value, out isElementDefined, out isElementNotNullOrEmpty, out isInt);
                    if (!isElementDefined)
                        throw new IndexOutOfRangeException("Element at row " + rowNumber + " and column " + columnNumber + " is not defined.");
                    if (!isElementNotNullOrEmpty)
                        throw new InvalidDataException("Element at row " + rowNumber + " and column " + columnNumber + " is null or empty string.");
                    throw new InvalidDataException("Element at row " + rowNumber + " and column " + columnNumber + " does not represent an integer. "
                        + Environment.NewLine + "  Element value: " + this[rowNumber, columnNumber]);
                }
            }
        }

        /// <summary>Safely gets the integer value at the specified position of the data table, if it is defined,
        /// and notifies the caller whether it is defined.</summary>
        /// <param name="rowNumber">Row number of the data table element.</param>
        /// <param name="columnNumber">Column number of the data table element.</param>
        /// <param name="value">Holds on return the corresponding integer value of the specified element,
        /// if defined, or default integer value (i.e. 0) otherwise.</param>
        /// <returns>Flag indicating whether the elemet at the specified position actually represents an integer.</returns>
        public bool TryGetInt(int rowNumber, int columnNumber, out int value)
        {
            lock (Lock)
            {
                value = default(int);
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return false;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return false;
                else if (columnNumber >= row.Count)
                    return false;
                else
                {
                    string valueStr = _data[rowNumber][columnNumber];
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        bool isInt = int.TryParse(valueStr, out value);
                        return isInt;
                    }
                    else
                        return false;
                }
            }
        }

        /// <summary>Safely gets the integer value at the specified position of the data table, if it is defined,
        /// and notifies the caller on the status.</summary>
        /// <param name="rowNumber">Row number of the data table element.</param>
        /// <param name="columnNumber">Column number of the data table element.</param>
        /// <param name="value">Holds on return the corresponding integer value of the specified element,
        /// if defined, or default integer value (i.e. 0) otherwise.</param>
        /// <param name="isElementDefined">Notifies whether the specified element is defined or not.</param>
        /// <param name="isElementNotNullOrEmpty">Notifies whether the element at the specified position is 
        /// not null or empty string.</param>
        /// <param name="isInt">Notifies whether the elemet at the specified position actually represents an integer.</param>
        public void GetIntSafe(int rowNumber, int columnNumber, out int value, out bool isElementDefined,
            out bool isElementNotNullOrEmpty, out bool isInt)
        {
            lock (Lock)
            {
                isElementDefined = false;
                isElementNotNullOrEmpty = false;
                isInt = false;
                value = default(int);
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return;
                else if (columnNumber >= row.Count)
                    return;
                else
                {
                    isElementDefined = true;
                    string valueStr = _data[rowNumber][columnNumber];
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        isElementNotNullOrEmpty = true;
                        isInt = int.TryParse(valueStr, out value);
                    }
                }
            }
        }

        #endregion Data.Int


        #region Data.Double

        /// <summary>Returns a flag specified whether the specified element of the data table exists and 
        /// represents a number (of type double).
        /// <para>If the element does not exist then false is returned.</para></summary>
        /// <param name="rowNumber">Row number of the data table element.</param>
        /// <param name="columnNumber">Column number of the data table element.</param>
        public bool IsDouble(int rowNumber, int columnNumber)
        {
            lock (Lock)
            {
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return false;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return false;
                else if (columnNumber >= row.Count)
                    return false;
                else
                {
                    string valueStr = _data[rowNumber][columnNumber];
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        double value;
                        bool isDouble = double.TryParse(valueStr, out value);
                        return isDouble;
                    }
                    else
                        return false;
                }
            }
        }

        /// <summary>Returns a double value of the element at the specified position of the data table,
        /// if such element is defined and represents a number, or throws an exception otherwise.</summary>
        /// <param name="rowNumber">Row number of the data table element.</param>
        /// <param name="columnNumber">Column number of the data table element.</param>
        /// <returns>Double value of the specified element, if the element exists and can be converted to an double
        /// (otherwise, an exception is thrown).</returns>
        public double GetDouble(int rowNumber, int columnNumber)
        {
            lock (Lock)
            {
                double value;
                bool isDouble = TryGetDouble(rowNumber, columnNumber, out value);
                if (isDouble)
                {
                    return value;
                }
                else
                {
                    bool isElementDefined, isElementNotNullOrEmpty;
                    // The specified table position does not contain a double, call another method to get
                    // more detailed error information:
                    GetDoubleSafe(rowNumber, columnNumber, out value, out isElementDefined, out isElementNotNullOrEmpty, out isDouble);
                    if (!isElementDefined)
                        throw new IndexOutOfRangeException("Element at row " + rowNumber + " and column " + columnNumber + " is not defined.");
                    if (!isElementNotNullOrEmpty)
                        throw new InvalidDataException("Element at row " + rowNumber + " and column " + columnNumber + " is null or empty string.");
                    throw new InvalidDataException("Element at row " + rowNumber + " and column " + columnNumber + " does not represent a number. "
                        + Environment.NewLine + "  Element value: " + this[rowNumber, columnNumber]);
                }
            }
        }

        /// <summary>Safely gets the double value at the specified position of the data table, if it is defined,
        /// and notifies the caller whether it is defined.</summary>
        /// <param name="rowNumber">Row number of the data table element.</param>
        /// <param name="columnNumber">Column number of the data table element.</param>
        /// <param name="value">Holds on return the corresponding double value of the specified element,
        /// if defined, or default double value (i.e. 0) otherwise.</param>
        /// <returns>Flag indicating whether the elemet at the specified position actually represents a double.</returns>
        public bool TryGetDouble(int rowNumber, int columnNumber, out double value)
        {
            lock (Lock)
            {
                value = default(double);
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return false;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return false;
                else if (columnNumber >= row.Count)
                    return false;
                else
                {
                    string valueStr = _data[rowNumber][columnNumber];
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        bool isDouble = double.TryParse(valueStr, out value);
                        return isDouble;
                    }
                    else
                        return false;
                }
            }
        }

        /// <summary>Safely gets the numerical value (of type double) at the specified position of the data table, if it is defined,
        /// and notifies the caller about the status.</summary>
        /// <param name="rowNumber">Row number of the data table element.</param>
        /// <param name="columnNumber">Column number of the data table element.</param>
        /// <param name="value">Holds on return the corresponding double value of the specified element,
        /// if defined, or default double value (i.e. 0.0) otherwise.</param>
        /// <param name="isElementDefined">Notifies whether the specified element is defined or not.</param>
        /// <param name="isElementNotNullOrEmpty">Notifies whether the element at the specified position is 
        /// not null or empty string.</param>
        /// <param name="isDouble">Notifies whether the elemet at the specified position actually represents a number of type double.</param>
        public void GetDoubleSafe(int rowNumber, int columnNumber, out double value, out bool isElementDefined,
            out bool isElementNotNullOrEmpty, out bool isDouble)
        {
            lock (Lock)
            {
                isElementDefined = false;
                isElementNotNullOrEmpty = false;
                isDouble = false;
                value = default(double);
                if (rowNumber < 0)
                    throw new ArgumentException("Row number should be greater or equal to 0, specified: " + rowNumber + ".");
                if (columnNumber < 0)
                    throw new ArgumentException("Column number shoud be greater or equal to 0, specified: " + columnNumber + ".");
                if (rowNumber >= _data.Count)
                    return;
                List<string> row = _data[rowNumber];
                if (row == null)
                    return;
                else if (columnNumber >= row.Count)
                    return;
                else
                {
                    isElementDefined = true;
                    string valueStr = _data[rowNumber][columnNumber];
                    if (!string.IsNullOrEmpty(valueStr))
                    {
                        isElementNotNullOrEmpty = true;
                        isDouble = double.TryParse(valueStr, out value);
                    }
                }
            }
        }


        #endregion Data.Double


        #region OperationCsv

        /// <summary>The default separator in the CSV files - comma (",").</summary>
        public const string DefaultCsvSeparator = ",";

        protected string _csvSeparator = UtilCsv.CsvSeparator;

        /// <summary>Separator used in CSV files that this class loads data from or writes data to.
        /// <para>Property is used by ethods that deal with CSV files and do not have separator as.</para></summary>
        public string CsvSeparator
        {
            get { lock (Lock) { return _csvSeparator; } }
            set { lock (Lock) { _csvSeparator = value; } }
        }

        /// <summary>Loads the specified CSV file. Reads contents of the file into the data table of the current object.</summary>
        /// <param name="filePath">Path to the CSV file that is read and parsed.</param>
        /// <param name="separator">Separator that is used in the CSV file. If not specified (null or empty string) then Constant <see cref="UtilStr.DefaultCsvSeparator"/> is assumed.</param>
        public void LoadCsv(string filePath, string separator)
        {
            // TODO: optimize this! (without use of the jagged array - directly)
            lock (Lock)
            {
                if (string.IsNullOrEmpty(separator))
                    separator = this.CsvSeparator;
                string[][] tab = UtilCsv.LoadCsv(filePath, separator);
                this.Table = tab;
            }
        }

        
        /// <summary>Loads the specified CSV file. Reads contents of the file into the data table of the current object.
        /// <para>The value of the <see cref="CsvSeparator"/> property is used as separator.</para></summary>
        /// <param name="filePath">Path to the CSV file that is read and parsed.</param>
        public void LoadCsv(string filePath)
        {
            this.LoadCsv(filePath, this.CsvSeparator);
        }

        
        /// <summary>Saves the data of the current object to the specified CSV file.</summary>
        /// <param name="filePath">Path to the file into which data is written.</param>
        /// <param name="separator">Separator that is used in CSV format.</param>
        /// <param name="append">If true then the CSV string is appended to the existent file if 
        /// the file already exists. Otherwise, existend files are overwritten.</param>
        public void SaveCsv(string filePath, string separator, bool append)
        {
            lock (Lock)
            {
                string[][] data = this.Table;
                UtilCsv.SaveCsv(filePath, data, separator, append);
            }
        }


        /// <summary>Saves the data of the current object into a CSV file.
        /// If the file already exists then its contents are overwritten.</summary>
        /// <param name="filePath">Path to the file into which contents is written.</param>
        /// <param name="separator">Separator that is used in CSV format.</param>
        public void SaveCsv(string filePath, string separator)
        {
            SaveCsv(filePath, separator, false /* append */);
        }

        /// <summary>Saves the data of the current object into a CSV file.
        /// Constant <see cref="UtilStr.DefaultCsvSeparator"/> is assumed to be a separator for the CSV format.</summary>
        /// <param name="filePath">Path to the file into which contents is written.</param>
        /// <param name="values">A 2D jagged array of string cell values. Each outer element contains one row of values in CSV.</param>
        public void SaveCsv(string filePath, bool append)
        {
            SaveCsv(filePath, CsvSeparator /* separator */, append);
        }

        /// <summary>Saves the data of the current object values into a CSV file.
        /// If the file already exists then its contents are overwritten.
        /// Constant <see cref="UtilStr.DefaultCsvSeparator"/> is assumed to be a separator for the CSV format.</summary>
        /// <param name="filePath">Path to the file into which contents is written.</param>
        /// <param name="values">A 2D jagged array of string cell values. Each outer element contains one row of values in CSV.</param>
        public void SaveCsv(string filePath)
        {
            SaveCsv(filePath, CsvSeparator /* separator */, false /* append */);
        }

        #endregion OperationCsv


        #region Examples

        /// <summary>Creates a simple string table and saves it to a CSV file.</summary>
        /// <param name="filePath">Path to the file where CSV is stored.</param>
        public static void ExampleWriteCsv(string filePath)
        {
            StringTable csv = new StringTable();
            csv.AddRow();
            csv[0, 0] = "Generated CSV example.";
            csv[0, 6] = "This element (or cell) was inserted with gaps after its previous element.";
            csv[1, 0] = "Parameters:";
            csv.AddElements(2, null, null, "p1", "p2", "p3", "p4");
            for (int i=0; i<4; ++i)
            {
                for (int j=0; j<4; ++j)
                    csv[i, j+2] = RandomGenerator.Global.NextDouble().ToString();
            }
            csv.SaveCsv(filePath);
        }


        #endregion Examples

    }  // class StringTable 


}
