// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Principal;
using System.Security.AccessControl;

namespace IG.Lib
{

    /// <summary>Lockable object, has a Lock property that returns object on which
    /// lock must be performed in order to lock the object.</summary>
    public interface ILockable
    {
        object Lock { get; }
    }

    /// <summary>General utilities.</summary>
    /// $A Igor Apr10;
    public class Util
    {


        #region ThreadSynchronization

        private static object _lockGlobal = new object();

        /// <summary>Global, process-level locking object. 
        /// <para>This object can be used for synchronization of any static methods.</para>
        /// <para>Warning: Do not use this lock for locking long lasting operations, since this can result in deadlocks.</para></summary>
        public static object LockGlobal
        { get { return _lockGlobal; } }


        /// <summary>Check whether the specified mutex has been abandoned, and returns true
        /// if it has been (otherwise, false is returned).
        /// <para>After the call, mutex is no longer in abandoned state (WaitOne() will not throw an exception)
        /// if it has been before the call.</para>
        /// <para>Call does not block.</para></summary>
        /// <param name="m">Mutex that is checked, must not be null.</param>
        /// <returns>true if mutex has been abandoned, false otherwise.</returns>
        public static bool MutexCheckAbandoned(Mutex m)
        {
            bool ret = false;
            if (m == null)
                throw new ArgumentException("Mutex to be checked is not specified (null argument).");
            bool acquired = false;
            try
            {
                acquired = m.WaitOne(0);
                if (acquired)
                {
                    try
                    {
                        m.ReleaseMutex();
                    }
                    catch { }
                }
            }
            catch
            {
                ret = true;
                try
                {
                    m.ReleaseMutex();
                }
                catch { }
            }
            return ret;
        }

        /// <summary>Name of the global mutex.</summary>
        public const string MutexGlobalName = "Global\\IG.Lib.Utils.MutexGlobal.R2D2_by_Igor_Gresovnik";

        protected static volatile Mutex _mutexGlobal;

        /// <summary>Mutex for system-wide exclusive locks.</summary>
        public static Mutex MutexGlobal
        {
            get
            {
                if (_mutexGlobal == null)
                {
                    lock (LockGlobal)
                    {
                        if (_mutexGlobal == null)
                        {
                            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                            MutexSecurity mutexsecurity = new MutexSecurity();
                            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.FullControl, AccessControlType.Allow));
                            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.ChangePermissions, AccessControlType.Deny));
                            mutexsecurity.AddAccessRule(new MutexAccessRule(sid, MutexRights.Delete, AccessControlType.Deny));
                            bool createdNew;
                            //_mutexGlobal = new Mutex(false, MutexGlobalName);
                            _mutexGlobal = new Mutex(false, MutexGlobalName,
                                out createdNew, mutexsecurity);
                        }
                    }
                }
                return _mutexGlobal;
            }
        }

        /// <summary>Check whether the global mutex (property <see cref="MutexGlobal"/>) has been abandoned, 
        /// and returns true if it has been (otherwise, false is returned).
        /// <para>After the call, mutex is no longer in abandoned state (WaitOne() will not throw an exception)
        /// if it has been before the call.</para>
        /// <para>Call does not block.</para></summary>
        /// <returns>true if mutex has been abandoned, false otherwise.</returns>
        public static bool MutexGlobalCheckAbandoned()
        {
            return MutexCheckAbandoned(MutexGlobal);
        }

        /// <summary>Suspends execution of the current thread for the specified time (in seconds).</summary>
        /// <param name="sleepTimeInSeconds">Sleeping time in seconds. If less than 0 then thread sleeps indefinitely.</param>
        public static void SleepSeconds(double sleepTimeInSeconds)
        {
            const int largeNumberOfMilliseconds = 100000;
            if (sleepTimeInSeconds >= 0)
            {
                int timeMs = (int) Math.Ceiling(sleepTimeInSeconds*1000.0);
                Thread.Sleep(timeMs);
            } else
            {
                if (OutputLevel >= 0)
                {
                    Console.WriteLine(
                        Environment.NewLine + Environment.NewLine
                        + "WARNING: Sleeping indefinitely in thread Np. " + Thread.CurrentThread.ManagedThreadId
                        + Environment.NewLine + Environment.NewLine);
                }

                while (true)
                    Thread.Sleep(largeNumberOfMilliseconds);
            }
        }

        #endregion ThreadSynchronization

        #region OutputLevelGlobal

        private static volatile int _outputLevel = 0;

        /// <summary>Serves as default output level for new objects of many classes that include the output
        /// level property (usually named "OutputLevel"). Such a property defines how much information about
        /// operation of the object is ouput to the console.</summary>
        /// <remarks>
        /// <para>General guidlines for use of the output level property in classes:</para>
        /// <para>The property usually defineds the quantity of output produced by an object of a class
        /// that implements this property. It is not strictly prescribed what certain values of the property 
        /// mean. By loose agreement, any negative value means unspecified output level (property not yet initialized),
        /// 0 means that no output is produced, 1 means only the most important information is ouptut and higher 
        /// values mean that more detailed information about operation is output to the console.</para>
        /// <para>For example application, see e.g. the IG.Gr.PlotterZedGraph in the 2D plotting library that uses IGLib.</para>
        /// </remarks>
        public static int OutputLevel
        {
            get { lock (LockGlobal) { return _outputLevel; } }
            set { lock (LockGlobal) { _outputLevel = value; } }
        }

        #endregion OutputLevelGlobal


        #region HashFunctions

        /// <summary>Standard string representation of null values of objects (often used when overriding 
        /// <see cref="object.ToString"/> method).</summary>
        public const string NullRepresentationString = "null";


        private static volatile int _maxLengthIntToString;

        /// <summary>Returns maximal length of string representation of integer value of type <see cref="int"/></summary>
        protected internal static int MaxLengthIntToString
        {
            get {
                if (_maxLengthIntToString == 0)
                {
                    lock (LockGlobal)
                    {
                        if (_maxLengthIntToString == 0)
                            _maxLengthIntToString = int.MinValue.ToString().Length;
                    }
                }
                return _maxLengthIntToString;
            }
        }

        /// <summary>Returns an integer hash function of the specified object.
        /// <para>Returned integer is always positive.</para>
        /// <para>This hash function is bound to the <see cref="object.ToString"/> method of the specified object,
        /// which means that it returns the same value for any two objects that have the same string
        /// representation.</para></summary>
        /// <param name="obj">Object whose hash function is returned.</param>
        /// <remarks><para>This hash function is calculated in such a way that <see cref="object.ToString"/>() is
        /// called first on <paramref name="obj"/> in order to obtain object's string representation (or, if the object is
        /// null, the <see cref="Util.NullRepresentationString"/> is taken), and then the <see cref="string.GetHashCode"/> 
        /// is called on the obtained string and its value returned.</para></remarks>
        public static int GetHashFunctionInt(object obj)
        {
            if (obj == null)
                return Util.NullRepresentationString.GetHashCode();
            else
            {
                string stringrep = obj.ToString();
                if (stringrep == null)
                    throw new InvalidOperationException("String representation of non-null object whose hash function is calculated is null.");
                int ret = stringrep.GetHashCode();
                if (ret < 0)
                {
                    ret = -ret;
                }
                return ret;
            }
        }


        /// <summary>Returns a string-valued hash function of the specified object.
        /// <para>This hash function is bound to the <see cref="object.ToString"/> method of the specified object,
        /// which means that it returns the same value for any two objects that have the same string
        /// representation.</para></summary>
        /// <param name="obj">Object whose string-valued hash function is returned.</param>
        /// <remarks><para>This hash function is calculated in such a way that <see cref="object.ToString"/>() is
        /// called first on <paramref name="obj"/> in order to obtain object's string representation (or, if the object is
        /// null, the <see cref="Util.NullRepresentationString"/> is taken), and then the <see cref="string.GetHashCode"/> 
        /// is called on the obtained string and its value returned.</para></remarks>
        /// <seealso cref="Util."/>
        public static string GetHashFunctionString(Object obj)
        {
            int maxLength = MaxLengthIntToString;
            char[] generatedCode = GetHashFunctionInt(obj).ToString().ToCharArray();
            int length = 0;
            if (generatedCode!=null)
                length = generatedCode.Length;
            if (length > maxLength)
            {
                if (OutputLevel >= 1)
                {
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine("WARNING: length of hash code " + length + " is greater than the maximum (" + maxLength + ".");
                    Console.WriteLine(Environment.NewLine);
                }
                maxLength = length;
            }
            char[] returnedCode = new char[maxLength];
            for (int i = 1; i <= maxLength; i++)
            {
                // Copy character representations of digits of generated hash code 
                // to the (equilength) returned code, starting form the last one:
                if (i <= length)
                {
                    // Copy character from teh generated code:
                    returnedCode[maxLength - i] = generatedCode[length - i];
                }
                else
                {
                    // No characters left in the generated code, fill remaining most significant
                    // characters with '0':
                    returnedCode[maxLength - i] = '0';
                }
            }
            return new string(returnedCode);
        }


        #endregion HashFunctions


        #region MultiDimensionalTables

        /// <summary>Returns the index of the element in the onedimensional list that corresponds
        /// to the specified indices of the multidimensional table of specified dimensions.</summary>
        /// <param name="indices">Indices of the element in the multidimensional table of training results.</param>
        /// <param name="tableDimensions">Dimensions of the multidimensional table.</param>
        /// <returns>One dimensional index that corresponds to the specified multidimensional indices
        /// of the element of the multidimensional table.</returns>
        public static int GetIndex(int[] tableDimensions, params int[] indices)
        {
            bool zeroDimensionalIndices = false;
            int numIndices = 0;
            if (indices == null)
                zeroDimensionalIndices = true;
            else
            {
                numIndices = indices.Length;
                if (numIndices < 1)
                    zeroDimensionalIndices = true;
            }
            if (zeroDimensionalIndices)
            {
                if (tableDimensions != null)
                {
                    if (tableDimensions.Length > 0)
                    {
                        throw new ArgumentException("Indices are 0 dimensional while the table has "
                            + tableDimensions.Length + " dimensions.");
                    }
                }
                return 0;
            }
            else
            {
                if (tableDimensions == null)
                    throw new ArgumentException("Table dimensions are not defined (null argument).");
                if (tableDimensions.Length != numIndices)
                    throw new ArgumentException("Number of indices " + numIndices + " is different than number of dimensions " + tableDimensions.Length + ".");
                int index = 0;
                int numElementsPerIndex = 1;  
                for (int whichIndex = numIndices - 1; whichIndex >= 0; --whichIndex)
                {
                    index += numElementsPerIndex * indices[whichIndex];
                    numElementsPerIndex *= tableDimensions[whichIndex];
                }
                return index;
            }
        }

        /// <summary>Calculates and stores the multidimensional indices of an element of the
        /// multidimensional table of the specified dimensions, which correspond to the specified 
        /// onedimensional index (index within 1D table containing all elements of the multidimensional 
        /// table, ordered according to the normal convention - earlier indices run slower).</summary>
        /// <param name="tableDimensions">Dimensions of the multidimensional table.</param>
        /// <param name="oneDimensionalIndex">One dimensional index that defines the position of the 
        /// element in the list of elements.</param>
        /// <param name="tableIndices">Variable where multidimensional indices of the element are stored.</param>
        public static void GetIndices(int[] tableDimensions, int oneDimensionalIndex, ref int[] tableIndices)
        {
            int numDimensions = 0;
            if (tableDimensions != null)
                numDimensions = tableDimensions.Length;
            if (numDimensions == 0)
                tableIndices = null;
            else
            {
                if (tableIndices == null)
                    tableIndices = new int[numDimensions];
                else if (tableIndices.Length != numDimensions)
                    tableIndices = new int[numDimensions];
                int numElements = 1;
                for (int whichDimension = 0; whichDimension < numDimensions; ++whichDimension)
                {
                    numElements *= tableDimensions[whichDimension];
                }
                int numElementsPerIndex = numElements;
                int numRemainingElements = numElements;
                for (int whichDimension = 0; whichDimension < numDimensions; ++whichDimension)
                {
                    numElementsPerIndex /= tableDimensions[whichDimension];
                    int currentIndex = numRemainingElements / numElementsPerIndex;
                    numRemainingElements -= currentIndex * numElementsPerIndex;
                    tableIndices[whichDimension] = currentIndex;
                }
            }
        }



        /// <summary>Returns the index of the element in an onedimensional list that corresponds
        /// to the specified indices of the multidimensional table of specified dimensions.</summary>
        /// <param name="indices">Indices of the element in the multidimensional table of training results.</param>
        /// <param name="tableDimensions">Dimensions of the multidimensional table.</param>
        /// <returns>One dimensional index that corresponds to the specified multidimensional indices
        /// of the element of the multidimensional table.</returns>
        public static int GetIndex(List<int> tableDimensions, params int[] indices)
        {
            if (tableDimensions == null)
                return GetIndex((int[])null, indices);
            return GetIndex(tableDimensions.ToArray(), indices);
        }
        
        /// <summary>Calculates and stores the multidimensional indices of an element of the
        /// multidimensional table of the specified dimensions, which correspond to the specified 
        /// onedimensional index (index in the 1D table containing all elements of the multidimensional 
        /// table, ordered according to the normal convention - earlier indices run slower).</summary>
        /// <param name="tableDimensions">Dimensions of the multidimensional table.</param>
        /// <param name="oneDimensionalIndex">One dimensional index that defines the position of the 
        /// element in the list of elements.</param>
        /// <param name="tableIndices">Variable where multidimensional indices of the element are stored.</param>
        public static void GetIndices(List<int> tableDimensions, int oneDimensionalIndex, ref int[] tableIndices)
        {
            if (tableDimensions == null)
                GetIndices((int[])null, oneDimensionalIndex, ref tableIndices);
            else
                GetIndices(tableDimensions.ToArray(), oneDimensionalIndex, ref tableIndices);
        }

        #endregion MultiDimensionalTables


        #region ListResize

        /// <summary>Allocates or re-allocates (resizes) the specified list in such a way that it
        /// contains the specified number of elements after operation.</summary>
        /// <typeparam name="T">Type of the list element.</typeparam>
        /// <param name="list">List to be allocated. </param>
        /// <param name="count">Number of elements list will contain after operation.</param>
        /// <param name="defaultElement">Elements to be added to the list if there are currently 
        /// too few elements.</param>
        /// <param name="reduceCapacity">If true then capacity is reduced if the current list's
        /// capacity exceeds the specified number  of elements.</param>
        /// $A Igor Apr10;
        public static void ResizeList<T>(ref List<T> list, int count, T defaultElement,
                bool reduceCapacity)
        {
            if (list == null)
                list = new List<T>(count);
            else
            {
                if (list.Capacity < count)
                    list.Capacity = count;
            }
            int currentCount = list.Count;
            if (currentCount < count)
            {
                for (int i = currentCount + 1; i <= count; ++i)
                    list.Add(defaultElement);
            }
            else if (currentCount > count)
            {
                list.RemoveRange(count, currentCount - count);
            }
            if (reduceCapacity && list.Capacity > count)
                list.Capacity = count;
        }


        /// <summary>Allocates or re-allocates (resizes) the specified list in such a way that it
        /// contains the specified number of elements after operation.
        /// If new size is smaller than the original size of the list then its capacity is not
        /// reduced.</summary>
        /// <typeparam name="T">Type of the list element.</typeparam>
        /// <param name="list">List to be allocated. </param>
        /// <param name="count">Number of elements list will contain after operation.</param>
        /// <param name="defaultElement">Elements to be added to the list if there are currently 
        /// too few elements.</param>
        /// $A Igor Apr10;
        public static void ResizeList<T>(ref List<T> list, int count, T defaultElement)
        {
            ResizeList<T>(ref list, count, defaultElement, false  /* reduceCapacity */ );
        }


        /// <summary>Allocates or re-allocates (resizes) the specified list in such a way that it
        /// contains the specified number of elements after operation. 
        /// If list must be enlarged then null elements are inserted to new places.
        /// List must contain elements of some reference type!</summary>
        /// <typeparam name="T">Type of the list element, must be a reference type.</typeparam>
        /// <param name="list">List to be allocated. </param>
        /// <param name="count">Number of elements list will contain after operation.</param>
        /// <param name="reduceCapacity">If true then capacity is reduced if the current list's
        /// capacity exceeds the specified number  of elements.</param>
        /// $A Igor Apr10;
        public static void ResizeListRefType<T>(ref List<T> list, int count,
                bool reduceCapacity) where T : class
        {
            ResizeList<T>(ref list, count, (T)null /* defaultElement */,
                reduceCapacity);
        }

        /// <summary>Allocates or re-allocates (resizes) the specified list in such a way that it
        /// contains the specified number of elements after operation. 
        /// If list must be enlarged then null elements are inserted to new places.
        /// List must contain elements of some reference type!</summary>
        /// <typeparam name="T">Type of the list element, must be a reference type.</typeparam>
        /// <param name="list">List to be allocated. </param>
        /// <param name="count">Number of elements list will contain after operation.</param>
        /// capacity exceeds the specified number  of elements.</param>
        /// $A Igor Apr10;
        public static void ResizeListRefType<T>(ref List<T> list, int count) where T : class
        {
            ResizeList<T>(ref list, count, (T)null /* defaultElement */,
                false /* reduceCapacity */);
        }

        /// <summary>Copies all elements of the specified list to a target list.
        /// After operation, target list contains all elements of the source list (only references are copied for objects)
        /// in the same order. 
        /// If the original list is null then target list can either be allocated (if it was allocated before the call) or not.
        /// Target list is allocated or re-allocated as necessary.</summary>
        /// <typeparam name="T">Type of elements contained in the list.</typeparam>
        /// <param name="original">Original list.</param>
        /// <param name="target">List that elements of the original list are copied to.</param>
        public static void CopyList<T>(List<T> original, ref List<T> target)
        {
            if (original == null)
            {
                if (target != null)
                    target.Clear();
            }
            else
            {
                int numEl = original.Count;
                if (target == null)
                    target = new List<T>(original.Capacity);
                int numElTarget = target.Count;
                if (numElTarget > numEl)
                {
                    target.RemoveRange(numEl, numElTarget - numEl);
                    numElTarget = target.Count;
                }
                for (int i = 0; i < numEl; ++i)
                {
                    if (i < numElTarget)
                        target[i] = original[i];
                    else
                        target.Add(original[i]);
                }
            }
        }

        /// <summary>Copies all elements of the specified list to a target table.
        /// After operation, target table contains all elements of the source list (only references are copied for objects)
        /// in the same order. 
        /// If the original list is null then target table will also become null.
        /// Target table is allocated or re-allocated as necessary.</summary>
        /// <typeparam name="T">Type of elements contained in the list.</typeparam>
        /// <param name="original">Original list.</param>
        /// <param name="target">Table that elements of the original list are copied to.</param>
        public static void CopyList<T>(List<T> original, ref T[] target)
        {
            if (original == null)
            {
                if (target != null)
                    target = null;
            }
            else
            {
                int numEl = original.Count;
                if (target == null)
                    target = new T[numEl];
                else if (target.Count() != numEl)
                    target = new T[numEl];
                for (int i = 0; i < numEl; ++i)
                {
                    target[i] = original[i];
                }
            }
        }

        #endregion ListResize


        #region ListSorted

        /// <summary>Checks whether the specified list is sorted according to the specified comparer,
        /// and returns true if the list is sorted and false if it is not.</summary>
        /// <typeparam name="T">Type of elements of the list.</typeparam>
        /// <param name="list">List to be checked for sorting.</param>
        /// <param name="comparer">Comparer according to which sorting is verified.</param>
        /// <returns>True if the specified list is sorted, false if not.</returns>
        /// <exception cref="ArgumentNullException">If list or comparer is null.</exception>
        public static bool IsListSorted<T>(List<T> list, IComparer<T> comparer)
        {
            if (list == null)
                throw new ArgumentNullException("list","List to be checked for sorting is not specified (null reference).");
            if (comparer == null)
                throw new ArgumentNullException("comparer", "Comparer is not specified (null reference).");
            for (int i = 0; i < list.Count - 1; ++i)
            {
                if (comparer.Compare(list[i], list[i - 1]) > 0)
                    return false;
            }
            return true;
        }

        /// <summary>Inserts the specified element to the appropriate position in a sorted list, in such a way that the list
        /// remains sorted.
        /// <para>Duplicate elements are allowed.</para></summary>
        /// <typeparam name="T">Type of elements of the list.</typeparam>
        /// <param name="sortedList">Sorted list.</param>
        /// <param name="insertedElement">Element to be inserted.</param>
        /// <param name="comparer">Comparer according to which the list is sorted.</param>
        /// <exception cref="ArgumentNullException">If list or comparer is null.</exception>
        public static void InsertSortedList<T>(List<T> sortedList, T insertedElement, IComparer<T> comparer)
        {
            if (sortedList == null)
                throw new ArgumentNullException("sortedList", "List where element should be inserted is not specified (null reference).");
            if (comparer == null)
                throw new ArgumentNullException("comparer", "Comparer is not specified (null reference).");
            else
            {
                int index = sortedList.BinarySearch(insertedElement, comparer);
                if (index < 0)
                {
                    // Element not yet in the list
                    sortedList.Insert(~index, insertedElement);
                } else
                {
                    // Element already in the list:
                    sortedList.Insert(index, insertedElement);
                }
            }
        }

        /// <summary>Inserts the specified element to the appropriate position in a sorted list, in such a way that the list
        /// remains and no duplicates are inserted.
        /// <para>If the list already contains the element that is equal (in the sense of comparer) than the inserted element
        /// then this method has no effect.</para></summary>
        /// <typeparam name="T">Type of elements of the list.</typeparam>
        /// <param name="sortedList">Sorted list.</param>
        /// <param name="insertedElement">Element to be inserted.</param>
        /// <param name="comparer">Comparer according to which the list is sorted.</param>
        /// <exception cref="ArgumentNullException">If list or comparer is null.</exception>
        public static void InsertSortedListUnique<T>(List<T> sortedList, T insertedElement, IComparer<T> comparer)
        {
            if (sortedList == null)
                throw new ArgumentNullException("sortedList", "List where element should be inserted is not specified (null reference).");
            if (comparer == null)
                throw new ArgumentNullException("comparer", "Comparer is not specified (null reference).");
            else
            {
                int index = sortedList.BinarySearch(insertedElement, comparer);
                if (index < 0)
                {
                    // Element not yet in the list
                    sortedList.Insert(~index, insertedElement);
                }
            }
        }


        /// <summary>Checks whether the specified list is sorted according to the specified comparison function,
        /// and returns true if the list is sorted and false if it is not.</summary>
        /// <typeparam name="T">Type of elements of the list.</typeparam>
        /// <param name="list">List to be checked for sorting.</param>
        /// <param name="comparison">Comparison method (delegate) according to which sorting is verified.</param>
        /// <returns>True if the specified list is sorted, false if not.</returns>
        /// <exception cref="ArgumentNullException">If list or comparer is null.</exception>
        public static bool IsListSorted<T>(List<T> list, Comparison<T> comparison)
        {
            if (list == null)
                throw new ArgumentNullException("list", "List to be checked for sorting is not specified (null reference).");
            if (comparison == null)
                throw new ArgumentNullException("comparison", "Comparison method (delegate) is not specified (null reference).");
            for (int i = 0; i < list.Count - 1; ++i)
            {
                if (comparison(list[i], list[i - 1]) > 0)
                    return false;
            }
            return true;
        }


        #region ListSorted_ToImplement

        // TODO: Uncomment and implement!

        /// <summary>Searches a sorted list in the specified range for the specified element, and returns 
        /// its index if the element is found, or a negative complement of the index before the first element 
        /// that is greater than the specified element (or binary complement of one greater than the last index if no element is greater).</summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="sortedList">List that is searched for the element. List must be sorted according to the specified comparison function.</param>
        /// <param name="searchedElement">Element that is searched for.</param>
        /// <param name="from">Index of the first element in the searched range.</param>
        /// <param name="to">Index of the last element in the searched range.</param>
        /// <param name="comparison">Comparison function (delegate) according to which the list is sorted.</param>
        /// <returns>Index of the searched element in the list, if there exist an element (within the search range)
        /// that is equal to the search element according to the comparison delegate, or a binary complement of the 
        /// negative index where the element should be inserted in order not tho spoil sorting.</returns>
        /// <exception cref="ArgumentNullException">When list or comparison delegate is null.</exception>
        public static int BinarySearchSortedListFromTo<T>(List<T> sortedList, T searchedElement, int from, int to, Comparison<T> comparison)
        {
            if (sortedList == null)
                throw new ArgumentNullException("list", "List to be checked for sorting is not specified (null reference).");
            if (comparison == null)
                throw new ArgumentNullException("comparison", "Comparison method (delegate) is nos specified (null reference).");
            if (from < 0 || from >= sortedList.Count)
                throw new IndexOutOfRangeException("Index of the first element (" + from + ")is out of range, should be between 0 and " + (sortedList.Count - 1) + ".");
            if (to < 0 || to >= sortedList.Count)
                throw new IndexOutOfRangeException("Index of the last element (" + to + ")is out of range, should be between 0 and " + (sortedList.Count - 1) + ".");
            int comparisonResult = comparison(searchedElement, sortedList[from]);
            if (comparisonResult == 0)
                return from;
            else if (comparisonResult<0)
                return ~from;
            comparisonResult = comparison(searchedElement, sortedList[to]);
            if (comparisonResult == 0)
                return to;
            else if (comparisonResult > 0)
                return ~(to+1);
            else
            {
                while (to - from > 1)
                {
                    int trial = (from + to) / 2;
                    comparisonResult = comparison(searchedElement, sortedList[trial]);
                    if (comparisonResult==0)
                        return trial;
                    else if (comparisonResult<0)
                        to = trial;
                    else
                        from = trial;
                }
                return ~(to);
            }
        }


        /// <summary>Searches a sorted list for the specified element, and returns 
        /// its index if the element is found, or a negative complement of the index before the first element 
        /// that is greater than the specified element (or binsry complement of number of elements if no element is greater).</summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="sortedList">List that is searched for the element. List must be sorted according to the specified comparison function.</param>
        /// <param name="searchedElement">Element that is searched for.</param>
        /// <param name="from">Index of the first element in the searched range.</param>
        /// <param name="to">Index of the last element in the searched range.</param>
        /// <param name="comparison">Comparison function (delegate) according to which the list is sorted.</param>
        /// <returns>Index of the searched element in the list, if there exist an element (within the search range)
        /// that is equal to the search element according to the comparison delegate, or a binary complement of the 
        /// negative index where the element should be inserted in order not tho spoil sorting.</returns>
        /// <exception cref="ArgumentNullException">When list or comparison delegate is null.</exception>
        public static int BinarySearchSortedList<T>(List<T> sortedList, T searchedElement, Comparison<T> comparison)
        {
            if (sortedList == null)
                throw new ArgumentNullException("list", "List to be checked for sorting is not specified (null reference).");
            if (comparison == null)
                throw new ArgumentNullException("comparison", "Comparison method (delegate) is not specified (null reference).");
            return BinarySearchSortedListFromTo(sortedList, searchedElement, 0, sortedList.Count - 1, comparison);
        }



        /// <summary>Inserts the specified element to the appropriate position in a sorted list, in such a way that the list
        /// remains sorted.
        /// <para>Duplicate elements are allowed.</para></summary>
        /// <typeparam name="T">Type of elements of the list.</typeparam>
        /// <param name="sortedList">Sorted list.</param>
        /// <param name="insertedElement">Element to be inserted.</param>
        /// <param name="comparison">Comparison function (delegate) according to which the list is sorted.</param>
        /// <exception cref="ArgumentNullException">If list or comparer is null.</exception>
        public static void InsertSortedList<T>(List<T> sortedList, T insertedElement, Comparison<T> comparison)
        {
            if (sortedList == null)
                throw new ArgumentNullException("sortedList", "List where element should be inserted is not specified (null reference).");
            if (comparison == null)
                throw new ArgumentNullException("comparer", "Comparer is not specified (null reference).");
            else
            {
                int index = BinarySearchSortedList(sortedList, insertedElement, comparison);
                if (index < 0)
                {
                    // Element not yet in the list
                    sortedList.Insert(~index, insertedElement);
                }
                else
                {
                    // Element already in the list:
                    sortedList.Insert(index, insertedElement);
                }
            }
        }

        /// <summary>Inserts the specified element to the appropriate position in a sorted list, in such a way that the list
        /// remains and no duplicates are inserted.
        /// <para>If the list already contains the element that is equal (in the sense of comparer) than the inserted element
        /// then this method has no effect.</para></summary>
        /// <typeparam name="T">Type of elements of the list.</typeparam>
        /// <param name="sortedList">Sorted list.</param>
        /// <param name="insertedElement">Element to be inserted.</param>
        /// <param name="comparison">Comparison function (delegate) according to which the list is sorted.</param>
        /// <exception cref="ArgumentNullException">If list or comparer is null.</exception>
        public static void InsertSortedListUnique<T>(List<T> sortedList, T insertedElement, Comparison<T> comparison)
        {
            if (sortedList == null)
                throw new ArgumentNullException("sortedList", "List where element should be inserted is not specified (null reference).");
            if (comparison == null)
                throw new ArgumentNullException("comparer", "Comparer is not specified (null reference).");
            else
            {
                int index = BinarySearchSortedList(sortedList, insertedElement, comparison);
                if (index < 0)
                {
                    // Element not yet in the list
                    sortedList.Insert(~index, insertedElement);
                }
            }
        }





        #endregion ListSorted_ToImplement



        #endregion ListSorted


        #region CollectionToString

        /// <summary>Returns a string representing the specified collection of objects.
        /// Each object is printeed by its ToString() method.
        /// Works on all collections, including lists and arrays.</summary>
        /// <param name="list">Collection to be converted to srting.</param>
        /// <param name="addNewlines">If true then a newline is added before each element printed.</param>
        /// <param name="numIndent">Number of spaces aded before each element.</param>
        public static string CollectionToString(System.Collections.ICollection list, bool addNewlines,
            int numIndent)
        {
            if (list == null)
                return "<null list>";
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('{');
                int count = list.Count;
                int i = 0;
                foreach (object obj in list)
                {
                    if (addNewlines)
                        sb.AppendLine();
                    if (numIndent > 0)
                        sb.Append(' ', numIndent);
                    sb.Append(obj.ToString());
                    if (i + 1 < count)
                        sb.Append(", ");
                    ++i;
                }
                if (addNewlines)
                    sb.AppendLine();
                sb.Append('}');
                return sb.ToString();
            }
        }

        /// <summary>Returns a string representing the specified collection of objects.
        /// Each object is printeed by its ToString() method.
        /// A  newline and two spaces are added before each element printed.
        /// Works on all collections, including lists and arrays.</summary>
        /// <param name="list">Collection to be converted to srting.</param>
        public static string CollectionToString(System.Collections.ICollection list)
        {
            return CollectionToString(list, true, 2);
        }



        /// <summary>Returns a string representing the specified list in long form.
        /// Count property (i.e. number of elements in collection) is also printed.
        /// Works on all collections, including lists and arrays.</summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">List to be converted to srting.</param>
        /// <param name="addNewlines">If true then a newline is added before each element printed.</param>
        /// <param name="numIndent">Number of spaces aded before each element.</param>
        public static string CollectionToStringLong(System.Collections.ICollection collection, bool addNewlines,
            int numIndent)
        {
            string prefix = null;
            if (collection != null)
            {
                prefix = "[Count: " + collection.Count + "] ";
            }
            return prefix + CollectionToString(collection, addNewlines, numIndent);
        }

        /// <summary>Returns a string representing the specified list in long form.
        /// Count property (i.e. number of elements in collection) is also printed.
        /// Works on all collections, including lists and arrays.
        /// A  newline and two spaces are added before each element printed.</summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">List to be converted to srting.</param>
        public static string CollectionToStringLong(System.Collections.ICollection collection)
        {
            return CollectionToStringLong(collection, true, 2);
        }


        /// <summary>Returns a string representing the specified generic list in short form (without count and capacity).</summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">List to be converted to srting.</param>
        /// <param name="addNewlines">If true then a newline is added before each element printed.</param>
        /// <param name="numIndent">Number of spaces aded before each element.</param>
        public static string ListToString<T>(List<T> list, bool addNewlines,
            int numIndent)
        {
            return ListToString(list, addNewlines, numIndent);
        }

        /// <summary>Returns a string representing the specified generic list in long form.
        /// Count and Capacity properties are also printed.
        /// A  newline and two spaces are added before each element printed.</summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">List to be converted to srting.</param>
        public static string ListToString<T>(List<T> list)
        {
            return ListToString(list, true, 2);
        }



        /// <summary>Returns a string representing the specified generic list in long form.
        /// Count and Capacity properties are also printed.</summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">List to be converted to srting.</param>
        /// <param name="addNewlines">If true then a newline is added before each element printed.</param>
        /// <param name="numIndent">Number of spaces aded before each element.</param>
        public static string ListToStringLong<T>(List<T> list, bool addNewlines,
            int numIndent)
        {
            string prefix = null;
            if (list != null)
            {
                prefix = "[Count: " + list.Count + ", Capacity: " + list.Count() + "] ";
            }
            return prefix + ListToString(list, addNewlines, numIndent);
        }

        /// <summary>Returns a string representing the specified generic list in long form.
        /// Count and Capacity properties are also printed.
        /// A  newline and two spaces are added before each element printed.</summary>
        /// <typeparam name="T">Type of list elements.</typeparam>
        /// <param name="list">List to be converted to srting.</param>
        public static string ListToStringLong<T>(List<T> list)
        {
            return ListToStringLong(list, true, 2);
        }


        #endregion CollectionToString


        #region StringParse


        /// <summary>Tries to parse a string representation of a boolean.</summary>
        /// <param name="str">String that is converted to boolean.</param>
        /// <param name="parsedValue">Boolean value parsed from the specified string.</param>
        /// <returns>true if string was successfully converted to boolean, false if not 
        /// (in this case <paramref name="parsedValue"/> retains its previous value).</returns>
        public static bool TryParseBoolean(string str, ref bool parsedValue)
        {
            bool parsed = false;
            try
            {
                parsedValue = ParseBoolean(str);
                parsed = true;
            }
            catch (Exception)
            { }
            return parsed;
        }

        /// <summary>Converts the specified string to a boolean value, if possible, and returns it.
        /// If conversion is not possible then exception is thrown.
        /// Recognized representations of true: "true", "1", "yes", "y" (case insensitive).
        /// Recognized representations of false: "false", "0", "no", "n" (case insensitive).</summary>
        /// <param name="str">String representation of boolean to beparsed.</param>
        /// <returns>Boolean value represented by the specified string.</returns>
        /// <exception cref="System.ArgumentNullException">When the string is null.</exception>
        /// <exception cref="System.FormatException">When the string can not represent a boolean value.</exception>
        public static bool ParseBoolean(string str)
        {
            bool value;
            try
            {
                value = bool.Parse(str);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(str))
                    throw;
                str = str.ToLower();
                if (str == "0")
                    value = false;
                else if (str == "1")
                    value = true;
                else if (str == "false")
                    value = false;
                else if (str == "true")
                    value = true;
                else if (str == "no")
                    value = false;
                else if (str == "yes")
                    value = true;
                else if (str == "n")
                    value = false;
                else if (str == "y")
                    value = true;
                else throw;

            }
            return value;
        }


        /// <summary>Tries to parse a string representation of a <see cref="ThreadPriority"/> enum.</summary>
        /// <param name="str">String that is converted to  a <see cref="ThreadPriority"/> value.</param>
        /// <param name="parsedValue">Boolean value parsed from the specified string.</param>
        /// <returns>true if string was successfully converted to <see cref="ThreadPriority"/>, false if not 
        /// (in this case <paramref name="parsedValue"/> retains its previous value).</returns>
        /// <seealso cref="Util.ParseThreadPriority"/>
        public static bool TryParseThreadPriority(string str, ref ThreadPriority parsedValue)
        {
            bool parsed = false;
            try
            {
                parsedValue = ParseThreadPriority(str);
                parsed = true;
            }
            catch (Exception)
            { }
            return parsed;
        }

        /// <summary>Converts the specified string to a <see cref="ThreadPriority"/> enum value, 
        /// if possible, and returns it. If conversion is not possible then exception is thrown.
        /// <para>Recognized representations (not case sensitive):</para>
        /// <para><see cref="ThreadPriority.Lowest"/>: "0", "lowest", "idle"</para>
        /// <para><see cref="ThreadPriority.BelowNormal"/>: "1", "belownormal", "low"</para>
        /// <para><see cref="ThreadPriority.Normal"/>: "2", "normal"</para>
        /// <para><see cref="ThreadPriority.AboveNormal"/>: "3", "abovenormal", "high"</para>
        /// <para><see cref="ThreadPriority."/>: "4", "Highest", "realtime"</para>
        /// </summary>
        /// <param name="str">String representation of a <see cref="ThreadPriority"/> value to be parsed.</param>
        /// <returns>The <see cref="ThreadPriority"/> value represented by the specified string.</returns>
        /// <exception cref="System.ArgumentNullException">When the string is null.</exception>
        /// <exception cref="System.FormatException">When the string can not represent a boolean value.</exception>
        public static ThreadPriority ParseThreadPriority(string str)
        {
            ThreadPriority value;
            try
            {
                value = (ThreadPriority) Enum.Parse(typeof(ThreadPriority), str, true /* ignoreCase */);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(str))
                    throw;
                int intVal = 0;
                bool parsed = false;
                parsed = int.TryParse(str, out intVal);
                if (parsed)
                {
                    // Strig represents an integer, convert it to the returned type:
                    if (intVal <= (int)ThreadPriority.Lowest)
                        value = ThreadPriority.Lowest;
                    else if (intVal >= (int)ThreadPriority.Highest)
                        value = ThreadPriority.Highest;
                    else
                        value = (ThreadPriority)intVal;
                    return value;
                }
                str = str.ToLower();
                if (str == "lowest" || str == "idle")
                    value = ThreadPriority.Lowest;
                else if (str == "belownormal" || str == "low")
                    value = ThreadPriority.BelowNormal;
                else if (str == "normal")
                    value = ThreadPriority.Normal;
                else if (str == "abovenormal" || str == "high")
                    value = ThreadPriority.AboveNormal;
                else if (str == "highest" || str == "realtime")
                    value = ThreadPriority.Highest;
               else throw;
            }
            return value;
        }


        #endregion StringParse


        #region Examples

        public static void ExampleList()
        {
            Console.WriteLine("\nExample Util.ExampleList(): \n");

            List<double> list = null;
            Console.WriteLine("\nInitial list: \n  " + ListToStringLong(list));
            int size;
            double defaultelement;
            size = 5;
            defaultelement = 9.9999;
            ResizeList(ref list, size, defaultelement);
            Console.WriteLine("\nAfter resize to " + size + ", default element " + defaultelement + ": \n  "
                + ListToStringLong(list));
            size = 10;
            defaultelement = 8.8888;
            ResizeList(ref list, size, defaultelement);
            Console.WriteLine("\nAfter resize to " + size + ", default element " + defaultelement + ": \n  "
                + ListToStringLong(list));
            Console.WriteLine("\nReduction, without capacity reduction: ");
            size = 7;
            defaultelement = 3.3333;
            ResizeList(ref list, size, defaultelement);
            Console.WriteLine("After resize to " + size + ", default element " + defaultelement + ": \n  "
                + ListToStringLong(list));
            Console.WriteLine("\nReduction, WITH capacity reduction: ");
            size = 4;
            defaultelement = 2.2222;
            ResizeList(ref list, size, defaultelement, true);
            Console.WriteLine("After resize to " + size + ", default element " + defaultelement + ": \n  "
                + ListToStringLong(list));
            Console.WriteLine("\nEnlarge again: ");
            size = 6;
            defaultelement = 6.6666;
            ResizeList(ref list, size, defaultelement, true);
            Console.WriteLine("After resize to " + size + ", default element " + defaultelement + ": \n  "
                + ListToStringLong(list));

        }

        #endregion Examples


        #region IGLib

        public const string IGLibUrl = "http://www2.arnes.si/~ljc3m2/igor/iglib/";

        public const string IGLibCodeDocumentationUrl = "http://www2.arnes.si/~fgreso/code_documentation/generated/iglib/html/index.html";

        public const string IGLibAuthor = "Igor Grešovnik";

        #endregion IGLib

    }  // class Utils



    /// <summary>Class representing a key-value pair where sorting can be performed both with respect to key
    /// and with respect to value.</summary>
    /// <typeparam name="Tkey">Type of the key, must implement IComparable interface.</typeparam>
    /// <typeparam name="Tvalue">Type of the value, must implement IComparable interface.</typeparam>
    /// $A Igor Apr10;
    public class KeyValueSortable<Tkey, Tvalue>
        where Tkey : IComparable
        where Tvalue : IComparable
    {
        public KeyValueSortable(Tkey key, Tvalue value, int orderParameter)
            : this(key, value)
        {
            this._orderParameter = orderParameter;
        }

        public KeyValueSortable(Tkey key, Tvalue value)
        {
            this._key = key;
            this._value = value;
            ++_counter;
            this._orderParameter = _counter;
        }

        protected static int _counter = 0;

        protected Tkey _key;
        protected Tvalue _value;
        int _orderParameter;

        /// <summary>Returns the key.</summary>
        public Tkey Key { get { return _key; } }

        /// <summary>Returns the value.</summary>
        public Tvalue Value { get { return _value; } }

        /// <summary>Order prameter that enable additional sorting when other fields are equal.</summary>
        public int OrderParameter { get { return _orderParameter; } }

        /// <summary>Comparison of keys.</summary>
        public static IComparer<KeyValueSortable<Tkey, Tvalue>> CompareKey =
                new ComparerKeyBase(false /* twoStage */, false /* strict */);

        /// <summary>Comparison of keys and then values (if keys are equal).</summary>
        public static IComparer<KeyValueSortable<Tkey, Tvalue>> CompareKeyValue =
                new ComparerKeyBase(true /* twoStage */, false /* strict */);

        /// <summary>Comparison of keys and then values (if keys are equal) and finally the (possibly unique)
        /// ordering parameter that enables strict ordering of objects with the same key and value.</summary>
        public static IComparer<KeyValueSortable<Tkey, Tvalue>> CompareKeyValueStrict =
                new ComparerKeyBase(true /* twoStage */, true /* strict */);


        /// <summary>Comparison of values.</summary>
        public static IComparer<KeyValueSortable<Tkey, Tvalue>> CompareValue =
                new ComparerValueBase(false /* twoStage */, false /* strict */);

        /// <summary>Comparison of values and then keys (if values are equal).</summary>
        public static IComparer<KeyValueSortable<Tkey, Tvalue>> CompareValueKey =
                new ComparerValueBase(true /* twoStage */, false /* strict */);

        /// <summary>Comparison of values and then keys (if keys are equal) and finally the (possibly unique)
        /// ordering parameter that enables strict ordering of objects with the same value and key.</summary>
        public static IComparer<KeyValueSortable<Tkey, Tvalue>> CompareValueKeyStrict =
                new ComparerValueBase(true /* twoStage */, true /* strict */);



        /// <summary>Base class for different IComparer classes.</summary>
        protected abstract class ComparerBase : IComparer<KeyValueSortable<Tkey, Tvalue>>
        {
            protected ComparerBase(bool twoStage, bool strict)
            {
                this._twoStage = twoStage;
                this._strict = Strict;
            }

            /// <summary>Extracts the object used in the first level of comparison from the argument.</summary>
            protected abstract IComparable GetFirstStageCompared(KeyValueSortable<Tkey, Tvalue> keyValue);

            /// <summary>Extracts the object used in the second level of comparison from the argument.</summary>
            protected abstract IComparable GetSecondStageCompared(KeyValueSortable<Tkey, Tvalue> keyValue);

            protected bool _twoStage = false;
            protected bool _strict = false;

            public bool TwoStage
            { get { return _twoStage; } }

            public bool Strict
            { get { return _strict; } }

            public int Compare(KeyValueSortable<Tkey, Tvalue> keyValue1, KeyValueSortable<Tkey, Tvalue> keyValue2)
            {
                if (keyValue1 == null)
                {
                    if (keyValue2 != null)
                        return -1;
                    else
                        return 0;
                }
                else if (keyValue2 == null)
                    return 1;
                IComparable obj1 = GetFirstStageCompared(keyValue1);
                IComparable obj2 = GetFirstStageCompared(keyValue2);
                int ret;
                if (obj1 == null)
                {
                    if (obj2 != null)
                        return -1;
                    else
                        return 0;
                }
                else if (obj2 == null)
                    return 1;
                else
                {
                    ret = obj1.CompareTo(obj2);
                    if (ret != 0)
                        return ret;
                }
                if (TwoStage)
                {
                    obj1 = GetSecondStageCompared(keyValue1);
                    obj2 = GetSecondStageCompared(keyValue2);
                    if (obj1 == null)
                    {
                        if (obj2 != null)
                            return -1;
                        else
                            return 0;
                    }
                    else if (obj2 == null)
                        return 1;
                    else
                    {
                        ret = obj1.CompareTo(obj2);
                        if (ret != 0)
                            return ret;
                    }
                }
                if (Strict)
                {
                    if (keyValue1.OrderParameter < keyValue2.OrderParameter)
                        return -1;
                    else if (keyValue1.OrderParameter > keyValue2.OrderParameter)
                        return 1;
                }
                return 0;
            }  // Compare(...,...)
        }  // class CompareBase


        /// <summary>IComparer that compares the key first and then eventually the value and 
        /// finally the additional ordering parameter, dependent on constructor parameters.</summary>
        protected class ComparerKeyBase : ComparerBase
        {
            public ComparerKeyBase(bool twoStage, bool strict)
                : base(twoStage, strict)
            { }

            /// <summary>Extracts the object used in the first level of comparison from the argument.</summary>
            protected override IComparable GetFirstStageCompared(KeyValueSortable<Tkey, Tvalue> keyValue)
            { return keyValue.Key; }

            /// <summary>Extracts the object used in the second level of comparison from the argument.</summary>
            protected override IComparable GetSecondStageCompared(KeyValueSortable<Tkey, Tvalue> keyValue)
            { return keyValue.Value; }
        }

        /// <summary>IComparer that compares the key first and then eventually the value and 
        /// finally the additional ordering parameter, dependent on constructor parameters.</summary>
        protected class ComparerValueBase : ComparerBase
        {
            public ComparerValueBase(bool twoStage, bool strict)
                : base(twoStage, strict)
            { }

            /// <summary>Extracts the object used in the first level of comparison from the argument.</summary>
            protected override IComparable GetFirstStageCompared(KeyValueSortable<Tkey, Tvalue> keyValue)
            { return keyValue.Value; }

            /// <summary>Extracts the object used in the second level of comparison from the argument.</summary>
            protected override IComparable GetSecondStageCompared(KeyValueSortable<Tkey, Tvalue> keyValue)
            { return keyValue.Key; }
        }

        void xxxx_to_delete()
        {
            StringBuilder sb = new StringBuilder();
            sb.Length = 10;
            sb.EnsureCapacity(10);
        }





    }  // class KeyValueSortable<Tkey, Tvalue>


}
