// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace IG.Lib
{


    /// <summary>Sorted list of unique items.
    /// It is guaranteed that at all times the list of containing items is sorted.</summary>
    /// <typeparam name="Type">Type of items stored in the list.</typeparam>
    /// $A Igor Dec08 Aug09;
    public class SortedUniqueItemList<Type>: ILockable
    {

        #region Constructors

        /// <summary>Creates an empty sorted list of items with the default capacity.</summary>
        public SortedUniqueItemList()
        { _list = new List<Type>(); }

        /// <summary>Creates an empty sorted list of items with the specified initial capacity.</summary>
        /// <param name="initialCapacity">Initial capacity of the list.</param>
        public SortedUniqueItemList(int initialCapacity)
        { _list = new List<Type>(initialCapacity); }


        /// <summary>Creates a sorted list of items containing all items from the specified table.</summary>
        /// <param name="items">Table fo items that are added to the created list.</param>
        public SortedUniqueItemList(params Type[] items)
        {
            if (items == null)
            {
                _list = new List<Type>();
            } else
            {
                _list = new List<Type>(items.Length);
                this.Add(items);
            }
        }

        /// <summary>Creates a sorted list of items containing all items from the specified list.</summary>
        /// <param name="items">List fo items that are added to the created list.</param>
        public SortedUniqueItemList(IList<Type> items)
        {
            if (items == null)
            {
                _list = new List<Type>();
            } else
            {
                _list = new List<Type>(items.Count);
                this.Add(items);
            }
        }

        /// <summary>Creates a sorted list of items containing all items from the specified collection.</summary>
        /// <param name="items">Collection fo items that are added to the created list.</param>
        public SortedUniqueItemList(ICollection<Type> items)
        {
            if (items == null)
            {
                _list = new List<Type>();
            } else
            {
                _list = new List<Type>(items.Count);
                this.Add(items);
            }
        }

        #endregion Constructors

        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        #endregion ThreadLocking

        #region Data

        private List<Type> _list;

        private int _capacityOverhead = 0;

        private bool _performDownSizing = false;

        /// <summary>Gets the specified element.</summary>
        /// <param name="index">Index of the element.</param>
        public Type this[int index]
        {
            get { return _list[index]; }
            // private set { _list[index] = value; }
        }

        /// <summary>Creates and returns an array that cotains elements of the current list, 
        /// in the actual order.</summary>
        public Type[] ToArray()
        {
            return _list.ToArray();
        }

        /// <summary>Gets the current number of elements in the list.</summary>
        public int Length
        { get { return _list.Count; } }

        /// <summary>Gets the current number of elements in the list.</summary>
        /// <remarks>Setter is protected.</remarks>
        public int Capacity
        { get { return _list.Capacity; }  protected set { _list.Capacity = value; }  }


        /// <summary>Overhead in capacity. If differnt than 0 then when current capacity is filled, adding operations
        /// will perform resizing in such a way that this number of elements are unoccupied. In this way, the 
        /// frequency of resizes is reduced. Also, removing operations can perform downsizing if the number of free
        /// elements after removal is greater than CapacityOverhead (dependent on value of the PerformDownSizing flag).</summary>
        public int CapacityOverhead
        {
            get { return _capacityOverhead; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Attempt to set capacity overhead to a negative number ("
                        + value + ").");
                if (value < 2)
                    value = 2;
                _capacityOverhead = value;
            }
        }


        /// <summary>Flag specifying whether capacity can be reduced on removal operations which would generate large
        /// excess of list capacity with respect to the number of actual elements contained in the list.</summary>
        /// <remarks>If the flag is true then downsizing will occur when the capacity exceeds number of items by
        /// twice the CapacityOverhead. Downsizing does not Occur if CapacityOverhead is 0.</remarks>
        public bool PerformDownSizing
        {
            get { return _performDownSizing; }
            protected set
            { }
        }

        private IComparer<Type> _comparer;

        /// <summary>Gets or sets the comparer.
        /// If a new comparer is set then re-sorting of the list is performed.
        /// Setter is thread safe, but getter is not.</summary>
        public IComparer<Type> Comparer
        {
            get { return _comparer; }
            set
            {
                lock (Lock)
                {
                    bool re_sort = false;
                    if (value != _comparer)
                        re_sort = true;
                    _comparer = value;
                    if (re_sort)
                        Sort();
                }
            }
        }

        #endregion Data

        #region Operations

        
        /// <summary>Removes all elements from the current list.</summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>Sorts the internal list of items.
        /// NOT thread safe.</summary>
        protected void Sort()
        {
            // Remark: List.Sort itself checks whether the specified IComparer argument
            // is null and if it is, the default comparer is used.
            _list.Sort(this.Comparer);
        }

        /// <summary>Returns true if the current list contains the specified item, or false otherwise.</summary>
        /// <param name="item">Item whose presence is checked.</param>
        public bool Contains(Type item)
        {
            return (IndexOf(item) >= 0);
        }

        /// <summary>Returns index of the specified item on the list, if it exists, otherwise a negative number is returned.
        /// NOT thread safe.</summary>
        /// <param name="item">Item that is searched for.</param>
        /// <returns>Index of the searched item on the list, if it is contained in the list.
        /// Otherwise, a negative number is returned that is the bitwise complement of the 
        /// index of the next element that is larger than item or, if there is no larger element, 
        /// the bitwise complement of list length (number of elements). In this case, the bitwise
        /// compliment of the returned number can be used as index at which the item must be
        /// inserted in order to preseerve sorting of the list.</returns>
        /// <remarks>When the returned value is negative (the item is not contained in the list), 
        /// then its negative value can be directly used to predict on which place the specified
        /// item would be inserted in the list.</remarks>
        public int IndexOf(Type item)
        {
            // Remark: List.BinarySearch itself checks whether the specified IComparer argument
            // is null and if it is, the default comparer is used.
            return _list.BinarySearch(item, this.Comparer);
        }


        /// <summary>Adds the specified element to the list if it is not yet contained in it.
        /// If the item is already contained in the list then an exception is thrown.
        /// NOT thread safe.</summary>
        /// <param name="item">Item to be added to the list. For reference types, a reference is added.</param>
        /// <exception cref="ArgumentException">Throws when an item equal to the specified item 
        /// (according to the specified comparison operator) already exists on the list.</exception>
        /// <remarks>Use the Add() method for safe addition (does not throw an exception if the specified item
        /// is already contained in the list).</remarks>
        public void AddChecked(Type item)
        {
            int itemIndex = ~(IndexOf(item));
            if (itemIndex >= 0)
                _list.Insert(itemIndex, item);
            else
                throw new ArgumentException("Can not add the specified item because it alreadty exists in the list.");
        }

        /// <summary>Adds the specified item to the list if it does not yet exist.
        /// NOT thread safe.</summary>
        /// <param name="item">Item to be added to the list.</param>
        /// <returns>A non-negative index of the inserted item if the item was not contained in the list before the operation,
        /// and a negative number if the item already existed in the list.</returns>
        public int Add(Type item)
        {
            int itemIndex = ~(IndexOf(item));
            if (itemIndex >= 0)
                _list.Insert(itemIndex, item);
            return itemIndex;
        }


        /// <summary>Temoves the item from the list that is equal to the specified item.
        /// NOT thread safe.</summary>
        /// <param name="item">Item to be removed.</param>
        /// <exception cref="InvalidOperationException">Throws when the specified item to be removed does not exist on the list.</exception>
        /// <remarks>Use the Remove() method for safe removal (does not throw exception ehen the specified item does not exist).</remarks>
        public void RemoveChecked(Type item)
        {
            int itemIndex = IndexOf(item);
            if (itemIndex >= 0)
            {
                _list.RemoveAt(itemIndex);
            } else
                throw new InvalidOperationException("Can not remove the specified item because it does not exist in the list.");
        }

        /// <summary>Removes the specified item from the list if it exists.
        /// NOT thread safe.
        /// The object is locked when operation is performed.</summary>
        /// <param name="item">Item to be removed, found by comparision operations, 
        /// takes into account that list is sorted.</param>
        /// <returns>Index of the removed item if the item existed in the list, 
        /// or a negative number if the item was not found on the list.</returns>
        public int Remove(Type item)
        {
            int itemIndex = IndexOf(item);
            if (itemIndex >= 0)
                _list.RemoveAt(itemIndex);
            if (PerformDownSizing)
            {
                if (_list.Capacity > _list.Count + 1 + 2 * CapacityOverhead)
                    _list.Capacity = _list.Count + 1 + CapacityOverhead / 2;
            }
            return itemIndex;
        }


        /// <summary>Adds the specified table of items to the current list.
        /// Items are added one by one, which is not the most efficient.
        /// The object is locked when operation is performed.</summary>
        /// <param name="items">Table containg items to be added.</param>
        public void Add(Type[] items)
        {
            lock (Lock)
            {
                if (items != null)
                {
                    for (int i = 0; i < items.Length; ++i)
                    {
                        this.Add(items[i]);
                    }
                }
            }
        }


        /// <summary>Adds the specified list of items to the current list.
        /// Items are added one by one, which is not the most efficient.</summary>
        /// <param name="items">List containg items to be added.</param>
        public void Add(IList<Type> items)
        {
            lock (Lock)
            {
                if (items != null)
                {
                    for (int i = 0; i < items.Count; ++i)
                    {
                        this.Add(items[i]);
                    }
                }
            }
        }


        /// <summary>Adds the specified collection of items to the current list.
        /// Items are added one by one, which is not the most efficient.
        /// The object is locked when operation is performed.</summary>
        /// <param name="items">Collection containg items to be added.</param>
        public void Add(ICollection<Type> items)
        {
            lock (Lock)
            {
                if (items != null)
                {
                    foreach (Type item in items)
                    {
                        this.Add(item);
                    }
                }
            }
        }


        #endregion Operations

        #region Misc


        /// <summary>Returns string representation of this sorted list.</summary>
        /// <returns></returns>
        public override string ToString()
        {
            lock(Lock)
            {
                string ret = Util.ListToStringLong<Type>(_list, true, 2);
                if (ret.Length < 200)
                    ret = Util.ListToString<Type>(_list);
                return ret;
            }
        }


        #endregion Misc

    }  // class SortedItemList<Type>
}
