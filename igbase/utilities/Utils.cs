// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Collections.Concurrent;

namespace IG.Lib
{

    ///// <summary>Lockable object, has a Lock property that returns object on which
    ///// lock must be performed in order to lock the object.</summary>
    //public interface ILockable
    //{
    //    object Lock { get; }
    //}

    /// <summary>General utilities.</summary>
    /// $A Igor Apr10 Jun15;
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
                int timeMs = (int)Math.Ceiling(sleepTimeInSeconds * 1000.0);
                Thread.Sleep(timeMs);
            }
            else
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
            get
            {
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
        /// <seealso cref="Util"/>
        public static string GetHashFunctionString(Object obj)
        {
            int maxLength = MaxLengthIntToString;
            char[] generatedCode = GetHashFunctionInt(obj).ToString().ToCharArray();
            int length = 0;
            if (generatedCode != null)
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


        #region EnumManipulation

        /// <summary>Returns IEnumerable containing all values of the enumeration whose type is specified as type parameter.</summary>
        /// <typeparam name="enumType">Type of enumeration whose all possible values are returned.</typeparam>
        public static List<enumType> GetEnumValues<enumType>()
        {
            IEnumerable<enumType> values = Enum.GetValues(typeof(enumType)).Cast<enumType>();
            List<enumType> ret = new List<enumType>();
            foreach (enumType val in values)
            {
                if (!ret.Contains(val))
                    ret.Add(val); 
            }
            return ret;
        }

        /// <summary>Returns a stirng that contains information about the specified enumeration type.
        /// <para>Enumeration name and its symbolic values, together with the corresponding integer values, are 
        /// contained in the returned string.</para>
        /// <para>Warning: thic function is slow because a number of exceptions are caught until the right
        /// integer type is picked, because conversion is performed through boxing, therefore exact match must be
        /// obtained.</para></summary>
        /// <typeparam name="enumType">Enumeration type whose information in string form is returned.</typeparam>
        public static string EnumValuesToString<enumType>()
            where enumType:struct
        {
            StringBuilder sb = new StringBuilder();
            enumType a = default(enumType);
            sb.AppendLine("Enumeration " + a.GetType().ToString() + ": ");
            object o;
            foreach (enumType val in GetEnumValues<enumType>())
            {
                o = val;  // box val in order to convert it to long
                long longVal = 0;
                bool converted = false;
                if (!converted)
                {
                    try
                    {
                        longVal = (long)o;
                        converted = true;
                    } catch(Exception) { }
                }
                if (!converted)
                {
                    try
                    {
                        longVal = (int)o;
                        converted = true;
                    }
                    catch (Exception) { }
                }
                if (!converted)
                {
                    try
                    {
                        longVal = (uint)o;
                        converted = true;
                    }
                    catch (Exception) { }
                }
                if (!converted)
                {
                    try
                    {
                        longVal = (short)o;
                        converted = true;
                    }
                    catch (Exception) { }
                }
                if (!converted)
                {
                    try
                    {
                        longVal = (ushort)o;
                        converted = true;
                    }
                    catch (Exception) { }
                }
                if (!converted)
                {
                    try
                    {
                        longVal = (byte)o;
                        converted = true;
                    }
                    catch (Exception) { }
                }
                if (!converted)
                {
                    try
                    {
                        longVal = (sbyte)o;
                        converted = true;
                    }
                    catch (Exception) { }
                }
                if (converted)
                    sb.AppendLine("  " + val.ToString() + " = " +  longVal);
                else
                { 
                    //  ! converted
                    ulong ulongVal = 0;
                    if (!converted)
                    {
                        try
                        {
                            ulongVal = (ulong)o;
                            converted = true;
                        }
                        catch (Exception) { }
                    }
                    if (converted)
                    {
                        sb.AppendLine("  " + val.ToString() + " = " + ulongVal);
                    }
                }
                if (!converted)
                {
                    sb.AppendLine("  " + val.ToString() + ": could not convert to integer.");
                }
            }
            return sb.ToString();
        }

        #endregion EnumManipulation


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
                throw new ArgumentNullException("list", "List to be checked for sorting is not specified (null reference).");
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
            else if (comparisonResult < 0)
                return ~from;
            comparisonResult = comparison(searchedElement, sortedList[to]);
            if (comparisonResult == 0)
                return to;
            else if (comparisonResult > 0)
                return ~(to + 1);
            else
            {
                while (to - from > 1)
                {
                    int trial = (from + to) / 2;
                    comparisonResult = comparison(searchedElement, sortedList[trial]);
                    if (comparisonResult == 0)
                        return trial;
                    else if (comparisonResult < 0)
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


        #region Arrays_Collections


        
        /// <summary>Creates and returns a stirng representation of a list of items.
        /// <para>Each item in the list is represented by calling its own ToString() method.</para></summary>
        /// <typeparam name="T">Type of elements of the listt.</typeparam>
        /// <param name="elementList">List whose string representation is returned.</param>
        public static string ToString<T>(IList<T> elementList)
        {
            return ToString(elementList, false /* newLines */, 0 /* numIndent */);
        }


        /// <summary>Creates and returns a stirng representation of a list of items.
        /// <para>Each item in the list is represented by calling its own ToString() method.</para></summary>
        /// <typeparam name="T">Type of elements of the listt.</typeparam>
        /// <param name="elementList">List whose string representation is returned.</param>
        /// <param name="newLines">If true then representation of each element is positioned in its ownl line.</param>
        /// <param name="numIndent">Indentation. If greater than 0 then this number of spaces is inserted before each 
        /// line of the returned string (including in the beginning of the returned string, which also applies
        /// if <paramref name="newLines"/> is false).</param>
        /// <returns></returns>
        public static string ToString<T>(IList<T> elementList, bool newLines, int numIndent = 0)
        {
            StringBuilder sb = new StringBuilder();
            if (numIndent < 0)
                throw new ArgumentException("Indentation can not be negative.");
            //else if (numIndent > 0)
            //{
            //    for (int i = 0; i < numIndent; ++i)
            //        sb.Append(" ");
            //    sb.Append(' ', numIndent);
            //    indent = sb.ToString();
            //    sb.Clear();
            //}
            if (elementList == null)
            {
                if (numIndent > 0 )
                    sb.Append(' ', numIndent); 
                sb.Append("null");
            }
            else
            {
                int length = elementList.Count;
                if (numIndent > 0)
                    sb.Append(' ', numIndent); 
                sb.Append('{');
                if (newLines)
                {
                    sb.AppendLine();
                    if (numIndent > 0)
                        sb.Append(' ', numIndent); 
                }
                for (int i = 0; i < length; ++i)
                {
                    if (newLines)
                        sb.Append("  ");
                    sb.Append(elementList[i].ToString());
                    if (i < length - 1)
                    {
                        if (newLines)
                            sb.Append(",");
                        else
                            sb.Append(", ");
                    }
                    if (newLines)
                    {
                        sb.AppendLine();
                        if (numIndent > 0)
                            sb.Append(' ', numIndent); 
                    }
                }
            }
            sb.Append('}');
            if (newLines)
                sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>Returns true if the specified enumerables (collections) are equal, false otherwise.
        /// <para>Enumerables are considered equal if they are both null, or they are of the same size and all
        /// elements are equal.</para></summary>
        /// <typeparam name="T">Type of elements of the enumerables.</typeparam>
        /// <param name="a">First enumerable to be compared.</param>
        /// <param name="b">Second enumerable to be compared.</param>
        /// <returns>True if the two enumerables are of equal lengths and have equal adjacent elements or are both null; false otherwise.</returns>
        public static bool AreEqual<T>(IEnumerable<T> a, IEnumerable<T> b)
            where T : IComparable<T>
        {
            if (a == null)
                return (b == null);
            else if (b == null)
                return false;
            else
            {
                IEnumerator<T> enuma = a.GetEnumerator();
                IEnumerator<T> enumb = b.GetEnumerator();
                while (true)
                {
                    bool isNexta = enuma.MoveNext();
                    bool isNextb = enumb.MoveNext();
                    if (!isNexta)
                    {
                        if (isNextb)
                            return false;  // incompatible element count
                        else
                            return true; // all elements were checked and there were no disagreements
                    }
                    else if (!isNextb)
                        return false;  // incompatible element count
                    else
                    {
                        if (!enuma.Current.Equals(enumb.Current))
                            return false;  // unequal elements detected
                    }
                }
            }
        }

        /// <summary>Returns true if the specified collections are equal, false otherwise.
        /// <para>Collection are considered equal if they are both null, or they are of the same size and all
        /// elements are equal.</para></summary>
        /// <remarks>There is also a method for comparing variables of <see cref="IEnumerable{T}"/> interface which can be used
        /// in all places where this method is used. A special method for collections was created for efficiency reasons,
        /// because the <see cref="IList{T}"/> interface implements the Count property, thus collections of unequal sizes
        /// can be immediately detected as unequal by comparing their size, and one does not need to iterate over elements.</remarks>
        /// <typeparam name="T">Type of elements of the collections.</typeparam>
        /// <param name="a">First collection to be compared.</param>
        /// <param name="b">Second collection to be compared.</param>
        /// <returns>True if the two collection are of equal lengths and have equal adjacent elements or are both null; false otherwise.</returns>
        public static bool AreEqual<T>(IList<T> a, IList<T> b)
            where T : IComparable<T>
        {
            if (a == null)
                return (b == null);
            else if (b == null)
                return false;
            else if (a.Count != b.Count)  // unequal size. This is the difference with comparing IEnumerable<T>.
                return false;
            else
            {
                if (a == null)
                    return (b == null);
                else if (b == null)
                    return false;
                else
                {
                    int la = a.Count, lb = b.Count;
                    if (la != lb)
                        return false;
                    else
                    {
                        for (int i = 0; i < la; ++i)
                            if (!a[i].Equals(b[i]))
                                return false;
                        return true;
                    }
                }
            }
        }




        /// <summary>Concatenates an arbitrary number of arrays or lists of the specified type, and returns the result.</summary>
        /// <typeparam name="T">Type of array elements.</typeparam>
        /// <param name="arrays">An arbitrary-length list of array or list parameters to be concatenated.</param>
        /// <returns>An array that contains, in order of appearance of the listed list/array parameters, all elements of
        /// those lists/arrays.</returns>
        public static T[] Concatenate<T>(params IList<T>[] arrays)
        {
            var result = new T[arrays.Sum(a => a.Count)];
            int offset = 0;
            for (int whichArray = 0; whichArray < arrays.Length; whichArray++)
            {
                arrays[whichArray].CopyTo(result, offset);
                offset += arrays[whichArray].Count;
            }
            return result;
        }




        #endregion Arrays_Collections



        #region ByteArrays.ConversionAndSize

        /// <summary>Returns size of a value of some specific value type, in bytes.</summary> 
        /// <remarks>See also: http://stackoverflow.com/questions/16519200/size-of-struct-with-generic-type-fields </remarks>
        /// <param name="val">Value whose size is returned.</param>
        public static int SizeOf<T>(T? val) where T : struct
        {
            if (val == null) throw new ArgumentNullException("obj");
            return SizeOf(typeof(T?));
        }

        /// <summary>Returns size of a value of some specific value type, in bytes.</summary> 
        /// <remarks>See also: http://stackoverflow.com/questions/16519200/size-of-struct-with-generic-type-fields </remarks>
        /// <param name="val">Value whose size is returned.</param>
        public static int SizeOf<T>(T val)
        {
            if (val == null) throw new ArgumentNullException("obj");
            return SizeOf(val.GetType());
        }

        private static readonly ConcurrentDictionary<Type, int>
        _cache = new ConcurrentDictionary<Type, int>();

        /// <summary>Returns size of a value of some specific value type, in bytes.</summary> 
        /// <remarks>See also: http://stackoverflow.com/questions/16519200/size-of-struct-with-generic-type-fields </remarks>
        /// <param name="t">Type of the value whose size is to be returned.</param>
        public static int SizeOf(Type t)
        {
            if (t == null) throw new ArgumentNullException("t");

            return _cache.GetOrAdd(t, t2 =>
            {
                var dm = new DynamicMethod("$", typeof(int), Type.EmptyTypes);
                ILGenerator il = dm.GetILGenerator();
                il.Emit(OpCodes.Sizeof, t2);
                il.Emit(OpCodes.Ret);

                var func = (Func<int>)dm.CreateDelegate(typeof(Func<int>));
                return func();
            });
        }

        /// <summary>Returns size of a value of type bool, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(bool val) { return sizeof(bool); }

        /// <summary>Returns size of a value of type char, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(char val) { return sizeof(char); }

        /// <summary>Returns size of a value of type sbyte, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(sbyte val) { return sizeof(sbyte); }

        /// <summary>Returns size of a value of type byte, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(byte val) { return sizeof(byte); }

        /// <summary>Returns size of a value of type short, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(short val) { return sizeof(short); }

        /// <summary>Returns size of a value of type ushort, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(ushort val) { return sizeof(ushort); }

        /// <summary>Returns size of a value of type int, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(int val) { return sizeof(int); }

        /// <summary>Returns size of a value of type uint, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(uint val) { return sizeof(uint); }

        /// <summary>Returns size of a value of type long, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(long val) { return sizeof(long); }

        /// <summary>Returns size of a value of type ulong, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(ulong val) { return sizeof(ulong); }

        /// <summary>Returns size of a value of type float, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(float val) { return sizeof(float); }

        /// <summary>Returns size of a value of type double, in bytes.</summary> <param name="val">Value whose size is returned.</param>
        public static int SizeOf(double val) { return sizeof(double); }

        ///// <summary>Returns size of a value of some value type, in bytes.</summary> 
        ///// <remarks>This works only for simlmple value types (char, byte, int, long, uint, ulong, float, double...).</remarks>
        ///// <param name="val">Value whose size is returned.</param>
        //public static int SizeOf<T>(T val)
        //{
        //    Type type = typeof(T);
        //    if (type.IsEnum)
        //    {
        //        return Marshal.SizeOf(Enum.GetUnderlyingType(type));
        //    }
        //    if (type.IsValueType)
        //    {
        //        return Marshal.SizeOf(val);
        //    }
        //    if (type == typeof(string))
        //    {
        //        return Encoding.Default.GetByteCount(val.ToString());
        //    }
        //    throw new InvalidOperationException("Can not determine the size of object of type " + type.ToString() + ".");
        //}




        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(bool val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also 
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(bool val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + "."); 
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (val == false)
                bytes[startIndex] = 0;
            else
                bytes[startIndex] = 1;
            //if (!BitConverter.IsLittleEndian)
            //{
            //    for (int i = 0; i < size; ++i)
            //        bytes[startIndex + i] = (byte)(val >> (8 * i));
            //}
            //else
            //{
            //    for (int i = 0; i < size; ++i)
            //        bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            //}
        }


        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(char val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(char val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }


        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(byte val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(byte val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }

        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(sbyte val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(sbyte val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }

        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(Int16 val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(Int16 val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }


        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(UInt16 val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(UInt16 val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }

        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(Int32 val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(Int32 val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }

        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(UInt32 val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(UInt32 val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }

        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(Int64 val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(Int64 val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }

        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(UInt64 val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(UInt64 val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            if (!BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + i] = (byte)(val >> (8 * i));
            }
            else
            {
                for (int i = 0; i < size; ++i)
                    bytes[startIndex + size - i - 1] = (byte)(val >> (8 * i));
            }
        }

        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(float val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(float val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            byte[] ret = BitConverter.GetBytes(val);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(ret);
            for (int i = 0; i < size; ++i)
                bytes[startIndex + i] = ret[i];
        }


        /// <summary>Converts a value to byte array.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored. Allocated/reallocated if null or the current size does not 
        /// precisely match the required size.</param>
        public static void ToByteArray(double val, ref byte[] bytes)
        {
            int size = SizeOf(val);
            if (bytes == null || bytes.Length != size)
                bytes = new byte[size];
            ToByteArray(val, bytes, 0 /* startIndex */);
        }

        /// <summary>Converts a value to sequence of bytes and stores these bytes int the specified byte array at the specified position.</summary>
        /// <remarks>Bytes are stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given values produce the same byte arrays on all machines, regardless of endianness.</remarks>
        /// <param name="val">Value to be converted to byte array.</param>
        /// <param name="bytes">Byte array where converted vlaue is stored.</param>
        /// <param name="startIndex">Index where bytes are stored in the provided byte array.</param>
        public static void ToByteArray(double val, byte[] bytes, int startIndex = 0)
        {
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null)
                throw new ArgumentException("Array of bytes to store the value is not specified (null reference).");
            if (bytes.Length < startIndex + size)
            {
                throw new ArgumentException("Array of bytes to store the value is too small (" + bytes.Length +
                    ", should be at least " + (startIndex + size) + "); starting index: " + startIndex + ".");
            }
            byte[] ret = BitConverter.GetBytes(val);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(ret);
            for (int i = 0; i < size; ++i)
                bytes[startIndex + i] = ret[i];
        }



        private static byte[] _auxBytes = null;


        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out bool val, bool preciseLength, int startIndex = 0)
        {
            val = false;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size + startIndex)
                    _auxBytes = new byte[size + startIndex];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToBoolean(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out bool val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }

        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out char val, bool preciseLength, int startIndex = 0)
        {
            val = (char) 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToChar(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out char val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }


        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out byte val, bool preciseLength, int startIndex = 0)
        {
            val = (byte)0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = (_auxBytes[0]);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out byte val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }


        ///// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        ///// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        ///// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        ///// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        ///// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        ///// <param name="val">Variable where the extracted value is stored.</param>
        ///// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        ///// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        ///// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        //public static void FromByteArray(byte[] bytes, out sbyte val, bool preciseLength, int startIndex = 0)
        //{
        //    val = (sbyte)0;
        //    int size = SizeOf(val);
        //    if (startIndex < 0)
        //        throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
        //    if (bytes == null || bytes.Length < size + startIndex)
        //        throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
        //    if (preciseLength && bytes.Length != size + startIndex)
        //        throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

        //    lock (LockGlobal)
        //    {
        //        if (_auxBytes == null || _auxBytes.Length < size)
        //            _auxBytes = new byte[size];
        //        if (!BitConverter.IsLittleEndian)
        //        {
        //            for (int i = 0; i < size; ++i)
        //                _auxBytes[i] = bytes[startIndex + i];
        //        }
        //        else
        //        {
        //            for (int i = 0; i < size; ++i)
        //                _auxBytes[i] = bytes[startIndex + size - i - 1];
        //        }
        //        throw new NotImplementedException("This method is not yet implemennted.");
        //        val = (sbyte) BitConverter.ToInt16(_auxBytes[0]); // BitConverter.ToUInt16 (_auxBytes, 0); ;
        //    }
        //}

        ///// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        ///// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        ///// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        ///// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        ///// <param name="bytes">Bytes array where value to be extracted is stored.
        ///// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        ///// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        ///// <param name="val">Variable where the extracted value is stored.</param>
        ///// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        //public static void FromByteArray(byte[] bytes, out sbyte val, int startIndex = 0)
        //{
        //    FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        //}



        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out Int16 val, bool preciseLength, int startIndex = 0)
        {
            val = 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToInt16(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out Int16 val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }

        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out UInt16 val, bool preciseLength, int startIndex = 0)
        {
            val = 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToUInt16(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out UInt16 val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }

        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out Int32 val, bool preciseLength, int startIndex = 0)
        {
            val = 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");
            
            lock(LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToInt32(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out Int32 val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }

        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out UInt32 val, bool preciseLength, int startIndex = 0)
        {
            val = 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");
            
            lock(LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToUInt32(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out UInt32 val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }

        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out Int64 val, bool preciseLength, int startIndex = 0)
        {
            val = 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToInt64(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out Int64 val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }

        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out UInt64 val, bool preciseLength, int startIndex = 0)
        {
            val = 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (!BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToUInt64(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out UInt64 val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }

        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out float val, bool preciseLength, int startIndex = 0)
        {
            val = 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                }
                else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToSingle(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out float val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }

        /// <summary>Extracts the value stored in a byte array in big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.</param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="preciseLength">If true then the <paramref name="bytes"/> array must have precisely the right length to
        /// store the value at the specified index (<paramref name="startIndex"/>), otherwise the array can also be larger.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out double val, bool preciseLength, int startIndex = 0)
        {
            val = 0;
            int size = SizeOf(val);
            if (startIndex < 0)
                throw new ArgumentException("Starting index should be greater or equal to 0. Provided: " + startIndex + ".");
            if (bytes == null || bytes.Length < size + startIndex)
                throw new ArgumentException("Byte array is too short (" + bytes.Length + " bytes, should be at least " + (startIndex + size) + ").");
            if (preciseLength && bytes.Length != size + startIndex)
                throw new ArgumentException("Byte array is of incorrect size (" + bytes.Length + " bytes, should be " + (startIndex + size) + ").");

            lock (LockGlobal)
            {
                if (_auxBytes == null || _auxBytes.Length < size)
                    _auxBytes = new byte[size];
                if (BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + i];
                } else
                {
                    for (int i = 0; i < size; ++i)
                        _auxBytes[i] = bytes[startIndex + size - i - 1];
                }
                val = BitConverter.ToDouble(_auxBytes, 0);
            }
        }

        /// <summary>Extracts the value stored in a byte array in the big-endian order.</summary>
        /// <remarks>Bytes must be stored in big-endian order ("network byte order") where most significant byte comes first. 
        /// This is compatible with the <see cref="Util.ToHexString(byte[], string)"/> method and also
        /// guarantees that given bytes produce the same value on all machines, regardless of endianness.</remarks>
        /// <param name="bytes">Bytes array where value to be extracted is stored.
        /// <para>Length of the byte array can be larger than the smallest possible (with respect to <paramref name="startIndex"/> and
        /// value size). If precise length is required, use the method with 2 arguments when starting index is 0, or with 4 arguments otherwise.</para></param>
        /// <param name="val">Variable where the extracted value is stored.</param>
        /// <param name="startIndex">Index of the byte array where the stored value begins (this allows to store other values).</param>
        public static void FromByteArray(byte[] bytes, out double val, int startIndex = 0)
        {
            FromByteArray(bytes, out val, false /* preciseLength */, startIndex);
        }


        #endregion ByteArrays.ConversionAndSize



        #region HexadecimalStrings

        /// <summary>Returns a byte array that is represented by a hexadecimal string.</summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] FromHexString(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return null;
            int length = hex.Length;
            int numBytes = 0;
            bool withSeparator = false;  // whether a separator is used in the string
            char separator = '0';
            if (length >= 3)
            {
                separator = hex[2];
                if (!char.IsLetterOrDigit(separator))
                    withSeparator = true;
            }
            if (withSeparator)
            {
                numBytes = (length + 1) / 3;
                if ((length + 1) % 3 != 0)
                    throw new ArgumentException("Hexadecimal string with separators is of invalid length " + length + ".");
            } else
            { 
                numBytes = length / 2;
                if ((length) % 2 != 0)
                    throw new ArgumentException("Hexadecimal string (without separators) is of invalid length " + length + ".");
            }
            byte[] bytes = new byte[numBytes];
            int whichHex = 0;
            for (int i = 0; i < numBytes; ++i)
            {
                char c1 = hex[whichHex];
                char c2 = hex[whichHex + 1];
                bytes[i] = (byte) (16 * HexCharToInt(c1) + HexCharToInt(c2));
                if (withSeparator)
                    whichHex += 3;
                else
                    whichHex += 2;
                // arr[i] = (byte)((HexCharToInt(hex[i << 1]) << 4) + (HexCharToInt(hex[(i << 1) + 1])));
            }

            return bytes;
        }

        /// <summary>Returns value of the specified hexadecimal character (e.g. 9 for '9', 10 for 'a' or 'A', 15 for 'f' or 'F').</summary>
        /// <param name="hex">Hexadecimal character whose integer value is returned.</param>
        /// <returns></returns>
        public static int HexCharToInt(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        
        /// <summary>Returns a hexadecimal string representation of the specified byte array using lower case letters for digits above 9.</summary>
        /// <param name="bytes">Array of bytes whose hexadecial representation is to be returned.</param>
        /// <param name="separator">If not null or empty string then this string is inserted between hexadecimal digits.
        /// <para>If specified then it must be a single character string, and may not be a digit or a letter.</para></param>
        public static string ToHexString(byte[] bytes, string separator = null)
        {
            return ToHexString(bytes, false /* upperCase */, separator);
        }

        /// <summary>Returns a hexadecimal string representation of the specified byte array.</summary>
        /// <param name="bytes">Array of bytes whose hexadecial representation is to be returned.</param>
        /// <param name="upperCase">Whether digits greater than 9 should be represented by upper case letters (default is false).</param>
        /// <param name="separator">If not null or empty string then this string is inserted between hexadecimal digits.
        /// <para>If specified then it must be a single character string, and may not be a digit or a letter.</para></param>
        public static string ToHexString(byte[] bytes, bool upperCase, string separator = null)
        {
            int numBytes = 0;
            if (bytes != null)
                numBytes = bytes.Length;
            bool withSeparator = (!string.IsNullOrEmpty(separator));
            if (withSeparator)
            {
                if (separator.Length > 1)
                    throw new ArgumentException("Separator length must be 1 for generating hexadecimal representations of byte arrays.");
                char first = separator[0];
                if (char.IsLetterOrDigit(first))
                    throw new ArgumentException("Invalid separator " + separator + ", may not be a digit or a letter.");
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < numBytes; ++i)
            {
                if (upperCase)
                    sb.Append(bytes[i].ToString("X2"));
                else
                    sb.Append(bytes[i].ToString("x2"));
                if (withSeparator && i < numBytes - 1)
                    sb.Append(separator);
            }
            return sb.ToString();
        }

        /// <summary>Returns true if the two specified hexadecimal strings represent the same sequence of bytes (or the same number),
        /// and false otherwise.
        /// <para>If any string is null or its length is 0 then false is returned.</para></summary>
        /// <param name="hexString1">The first hexadecimal sequence to be compared.</param>
        /// <param name="hexString2">The second hexadecimal sequence to be compared.</param>
        public static bool AreHexStringsEqual(string hexString1, string hexString2)
        {
            bool ret = false;
            byte[] b1 = FromHexString(hexString1);
            byte[] b2 = FromHexString(hexString2);
            if (b1 != null && b2 != null)
                if (b1.Length == b2.Length)
                {
                    int length = b1.Length;
                    for (int i = 0; i < length; ++i)
                        if (b1[i] != b2[i])
                            return false;
                    ret = true;
                }
            return ret;

            //if (hexString1 != null)
            //    hexString1 = hexString1.ToUpper();
            //if (hexString2 != null)
            //    hexString2 = hexString2.ToUpper();
            //return hexString1 == hexString2;
        }

        #endregion HexadecimalStrings



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
        /// <param name="collection">Collection to be converted to srting.</param>
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
        /// <param name="collection">Collection to be converted to srting.</param>
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
            // return ListToString(list, addNewlines, numIndent);
            return ToString(list, addNewlines, numIndent);
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


        #region ToString

        // Globalization:

        /// <summary>Converts obect of the specified type to its string representation, where
        /// numbers are converted in ivariant culture (ignoring any localization settings).
        /// <para>This method can be used to avoid problems with differen local settinggs when
        /// transfering numerical values through text files.</para></summary>
        /// <typeparam name="ObjectType">Type of the object to be converted to string.</typeparam>
        /// <param name="obj">Object to be converted.</param>
        public static string ObjectToString<ObjectType>(ObjectType obj)
        {
            return ObjectToString<ObjectType>(obj, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>Converts obect of the specified type to its string representation, where
        /// numbers are converted in ivariant culture (ignoring any localization settings).
        /// <para>This method can be used to avoid problems with differen local settinggs when
        /// transfering numerical values through text files.</para></summary>
        /// <typeparam name="ObjectType">Type of the object to be converted to string.</typeparam>
        /// <param name="obj">Object to be converted.</param>
        /// <param name="cultureInfo">Culture info used in conversion.</param>
        public static string ObjectToString<ObjectType>(ObjectType obj, System.Globalization.CultureInfo cultureInfo)
        {
            if (IsNumeric(obj))
            {
                object expression = obj;
                return Convert.ToString(expression, cultureInfo);
            }
            else
            {
                return obj.ToString();
            }
        }


        /// <summary>Returns a flag indicating whether the specified object is of numeric type (such as int, float, double, etc.).
        /// <para>When called on an arbitrary object, the correct type parameter will be inferred, and
        /// we can get the desired information if </para></summary>
        /// <typeparam name="ObjectType">Type of the object that is queried.</typeparam>
        /// <param name="obj">Object for which we query whether it represents a numerical value.</param>
        public static bool IsNumeric<ObjectType>(ObjectType obj)
        {
            if (Equals(obj, null))
            {
                return false;
            }
            Type objType = typeof(ObjectType);
            if (objType.IsPrimitive)
            {
                return (objType != typeof(bool) &&
                    objType != typeof(char) &&
                    objType != typeof(IntPtr) &&
                    objType != typeof(UIntPtr)) ||
                    objType == typeof(decimal);
            }
            return false;
        }


        /// <summary>Returns true if the specified expression or object is of numeric type (such as int, float, double, etc.),
        /// and false otherwise.</summary>
        /// <param name="expression">Expression that is checked for being of numeric type.</param>
        [Obsolete("Replaced by IsNumeric<ObjectType>.")]
        protected static bool IsNumericOld(object expression)
        {
            if (expression == null)
                return false;
            double number;
            return Double.TryParse(
                Convert.ToString(expression, System.Globalization.CultureInfo.InvariantCulture),
                System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo,
                out number);
        }



        /// <summary>Test conversion to strings with invariant culture info.</summary>
        public static void TestToString()
        {
            Console.WriteLine(Environment.NewLine + "Test of conversion of numbers to string with invariant culture:" + Environment.NewLine);
            Console.WriteLine("Default (straightfrward) conversion: ");
            double d = 1.234e-6;
            Console.WriteLine("Through Console.WriteLine: " + d);
            Console.WriteLine("Through ToString(): " + d.ToString());
            Console.WriteLine("Through generic Util.ToString(): " +
                Util.ObjectToString<double>(d));
            Console.WriteLine("Through ToString() with NonGeneric Util.ToString(): " +
                Util.ObjectToString(d));
            object o = d;
            Console.WriteLine("Through NonGeneric Util.ToString(), cast to object: " +
                Util.ObjectToString(o));
            Console.WriteLine("Through NonGeneric Util.ToString(), cast to object and back to double: " +
                Util.ObjectToString((double)o));
            Console.WriteLine("Test of number to string conversion finished.");
        }


        #endregion ToString


        #region StringParse



        /// <summary>Tries to parse a string representation of an object of the specified type and return 
        /// it through output argument. Invariant culture is used in parsing.</summary>
        /// <typeparam name="ReturnType">Type of the object whose value is tried to be parsed from the string.</typeparam>
        /// <param name="strValue">String that is converted to obect of the specified type.</param>
        /// <param name="parsedValue">Value (of the specified type)vthat is obtained from the parsed string.</param>
        /// <returns>true if string was successfully converted to the object of the specified type, false if not 
        /// (in this case <paramref name="parsedValue"/> retains its previous value).</returns>
        public static bool TryParse<ReturnType>(string strValue, ref ReturnType parsedValue)
        {
            return TryParse<ReturnType>(strValue, ref parsedValue, System.Globalization.CultureInfo.InvariantCulture);
        }


        /// <summary>Converts a string to the object of the specified type and returns the entity, by using the 
        /// invariant culture.
        /// <para>This works for simple types, for complex types deserialization must be used.</para></summary>
        /// <typeparam name="ReturnType">Type of the entity to be returned, can be int.</typeparam>
        /// <param name="strValue">String to be converted to other type.</param>
        /// <returns>Object of the specified type converted form a string.</returns>
        public static ReturnType Parse<ReturnType>(string strValue)
        {
            return Parse<ReturnType>(strValue, System.Globalization.CultureInfo.InvariantCulture);
        }


        /// <summary>Converts a string to the entity of the specified type and returns that entity, by using invariant culture.
        /// <para>This works for simple types, for complex types deserialization must be used.</para></summary>
        /// <param name="strValue">String to be converted to other type.</param>
        /// <param name="propertyType">Type of the entity to be parsed from a string.</param>
        /// <returns>Object of the specified type converted form a string.</returns>
        public static object Parse(string strValue, Type propertyType)
        {
            return Parse(strValue, propertyType, System.Globalization.CultureInfo.InvariantCulture);
        }


        /// <summary>Tries to parse a string representation of an object of the specified type and return it through output argument.</summary>
        /// <typeparam name="ReturnType">Type of the object whose value is tried to be parsed from the string.</typeparam>
        /// <param name="strValue">String that is converted to obect of the specified type.</param>
        /// <param name="parsedValue">Value (of the specified type)vthat is obtained from the parsed string.</param>
        /// <param name="cultureInfo">Culture info used in conversion.</param>
        /// <returns>true if string was successfully converted to the object of the specified type, false if not 
        /// (in this case <paramref name="parsedValue"/> retains its previous value).</returns>
        public static bool TryParse<ReturnType>(string strValue, ref ReturnType parsedValue, System.Globalization.CultureInfo cultureInfo)
        {
            bool parsed = false;
            try
            {
                parsedValue = (ReturnType)Parse<ReturnType>(strValue, cultureInfo);
                parsed = true;
            }
            catch (Exception)
            { }
            return parsed;
        }


        /// <summary>Converts a string to the object of the specified type and returns the entity, by using the 
        /// specified culture info.
        /// <para>This works for simple types, for complex types deserialization must be used.</para></summary>
        /// <typeparam name="ReturnType">Type of the entity to be returned, can be int.</typeparam>
        /// <param name="strValue">String to be converted to other type.</param>
        /// <param name="cultureInfo">Culture info used in conversion.</param>
        /// <returns>Object of the specified type converted form a string.</returns>
        public static ReturnType Parse<ReturnType>(string strValue, System.Globalization.CultureInfo cultureInfo)
        {
            return (ReturnType)Parse(strValue, typeof(ReturnType), cultureInfo);
        }


        /// <summary>Converts a string to the entity of the specified type and returns that entity.
        /// <para>This works for simple types, for complex types deserialization must be used.</para></summary>
        /// <param name="strValue">String to be converted to other type.</param>
        /// <param name="propertyType">Type of the entity to be parsed from a string.</param>
        /// <param name="cultureInfo">Culture info used in conversion.</param>
        /// <returns>Object of the specified type converted form a string.</returns>
        public static object Parse(string strValue, Type propertyType, System.Globalization.CultureInfo cultureInfo)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            if (underlyingType == null)
                return Convert.ChangeType(strValue, propertyType, cultureInfo);
            return String.IsNullOrEmpty(strValue)
              ? null
              : Convert.ChangeType(strValue, underlyingType, cultureInfo);
        }



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
            catch (Exception)
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
        /// <para><see cref="ThreadPriority.Highest"/>: "4", "Highest", "realtime"</para>
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
                value = (ThreadPriority)Enum.Parse(typeof(ThreadPriority), str, true /* ignoreCase */);
            }
            catch (Exception )
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


        #region XML


        /// <summary>Returns a reformatted XML string, eventually in a more human readable form.</summary>
        /// <param name="xmlString">String containing the XML to be returned in a reformatted form.</param>
        /// <param name="indentCahrs">String used for indentation (default is string containing two space characters). 
        /// Default is two space characters. If null or empty srting then no indentation id used.</param>
        /// <param name="newlineChars">Character used for newlines. If null then <see cref="Environment.NewLine"/> is used.</param>
        public static string XmlToString(string xmlString, string indentCahrs = "  ", string newlineChars = null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            return XmlToString(doc, indentCahrs, newlineChars);
        }


        /// <summary>Converts the specified XML document to string, eventually with human readable indentation
        /// and newlines added. The stirng representation is returned.</summary>
        /// <param name="doc">XML documennt to be converted to a string.</param>
        /// <param name="indentCahrs">String used for indentation (default is string containing two space characters). 
        /// Default is two space characters. If null or empty srting then no indentation id used.</param>
        /// <param name="newlineChars">Character used for newlines. If null then <see cref="Environment.NewLine"/> is used.</param>
        public static string XmlToString(XmlDocument doc, string indentCahrs = "  ", string newlineChars = null)
        {
            if (newlineChars == null)
                newlineChars = Environment.NewLine;
            NewLineHandling newLineHandling = NewLineHandling.Replace;
            StringBuilder sb = new StringBuilder();
            bool indent = false;
            if (!string.IsNullOrEmpty(indentCahrs))
                indent = true;
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = indent,
                IndentChars = "  ",
                NewLineChars = newlineChars,
                NewLineHandling = newLineHandling
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        #endregion XML


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
