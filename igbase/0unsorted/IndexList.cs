// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Num;

namespace IG.Lib
{

    /// <summary>Index list, a sorted list of unique integer indices.
    /// Used for tasks such as filtering specified element from a list of elements or a general data set.</summary>
    /// $A Igor Dec08;
    public class IndexList : SortedUniqueItemList<int>
    {

        #region Construction

        /// <summary>Creates an empty index table with the default capacity.</summary>
        public IndexList()
            : base()
        { }

        /// <summary>Creates an empty index table with the specified initial capacity.</summary>
        /// <param name="initialCapacity">Initial capacity of the list.</param>
        public IndexList(int initialCapacity)
            : base(initialCapacity)
        {  }

        
        /// <summary>Creates an index table containing all indices from the specified table.</summary>
        /// <param name="items">Table fo indices that are added to the created index table.</param>
        public IndexList(params int[] items): base(items)
        {  }

        /// <summary>Creates an index table containing all indices from the specified list.</summary>
        /// <param name="items">List fo indices that are added to the created index table.</param>
        public IndexList(IList<int> items): base(items)
        {  }

        /// <summary>Creates an index table containing all indices from the specified collection.</summary>
        /// <param name="items">Collection fo indices that are added to the created index table.</param>
        public IndexList(ICollection<int> items): base(items)
        {  }


        #endregion Construction

        #region OperationStatic


        /// <summary>Creates and returns a random permutation of the specified length.</summary>
        /// <param name="length">Length of permutation, i.e. number of random indices contained in the generated list of indices.</param>
        public static IndexList CreateRandomPermutation(int length)
        {
            return CreateRandomPermutation(RandomGenerator.Global, length);
        }

        /// <summary>Creates and returns a random permutation of the specified length 
        /// by using the specified random generator.</summary>
        /// <param name="rand">Random numbers generator used to generate indices randomly. There is a version of the method without
        /// this argument, which uses the global random generator.</param>
        /// <param name="length">Length of permutation, i.e. number of random indices contained in the generated list of indices.</param>
        public static IndexList CreateRandomPermutation(IRandomGenerator rand, int length)
        {
            return CreateRandom(rand, length, 1 /* lowerBound */, length /* upperBound */);
        }


        /// <summary>Creates a random permutation of the specified length and stores them it the specified index list.</summary>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="upperBound">Upper bound.</param>
        /// <param name="indices">Index list to which the generated indices are stored.</param>
        public static void SetRandomPermutation(int length, int upperBound, ref IndexList indices)
        {
            SetRandomPermutation(RandomGenerator.Global, length, ref indices);
        }

        /// <summary>Creates a random permutation of the specified length by using the specified 
        /// random generator</summary>
        /// <param name="rand">Random numbers generator used to generate indices randomly. There is a version of the method without
        /// this argument, which uses the global random generator.</param>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="indices">Index list to which the generated indices are stored.</param>
        public static void SetRandomPermutation(IRandomGenerator rand, int length, ref IndexList indices)
        {
            SetRandom(rand, length, 1 /* lowerBound */, length /* upperBound */, ref indices);
        }



        /// <summary>Creates a prescribed number of unique random indices in the specified range.</summary>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="upperBound">Upper bound for generated indices (contained indices are lesser or equal to this value).</param>
        public static IndexList CreateRandom(int length, int upperBound)
        {
            return CreateRandom(RandomGenerator.Global, length, 0 /* lowerBound */, upperBound);
        }

        /// <summary>Creates and returns an index list with the specified number of unique random indices in the specified range .</summary>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="lowerBound">Lower bound for generated indices (contained indices are greater or equal to this value).</param>
        /// <param name="upperBound">Upper bound for generated indices (contained indices are lesser or equal to this value).</param>
        public static IndexList CreateRandom(int length, int lowerBound, int upperBound)
        {
            return CreateRandom(RandomGenerator.Global, length, lowerBound, upperBound);
        }

        /// <summary>Creates and returns an index list with the specified number of unique random indices in the specified range 
        /// by using the specified random generator.
        /// Lower bound of generated indices is 0.</summary>
        /// <param name="rand">Random numbers generator used to generate indices randomly. There is a version of the method without
        /// this argument, which uses the global random generator.</param>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="upperBound">Upper bound for generated indices (contained indices are lesser or equal to this value).</param>
        public static IndexList CreateRandom(IRandomGenerator rand, int length, int upperBound)
        {
            return CreateRandom(rand, length, 0 /* lowerBound */, upperBound);
        }

        /// <summary>Creates and returns an index list with the specified number of unique random indices in the specified range 
        /// by using the specified random generator.</summary>
        /// <param name="rand">Random numbers generator used to generate indices randomly. There is a version of the method without
        /// this argument, which uses the global random generator.</param>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="lowerBound">Lower bound for generated indices (contained indices are greater or equal to this value).</param>
        /// <param name="upperBound">Upper bound for generated indices (contained indices are lesser or equal to this value).</param>
        public static IndexList CreateRandom(IRandomGenerator rand, int length, int lowerBound, int upperBound)
        {
            IndexList ret = new IndexList(length);
            SetRandom(rand, length, lowerBound, upperBound, ref ret);
            //if (length > 1 + upperBound - lowerBound)
            //    throw new ArgumentException("Requested number of indices (" + length + ") is too large for generation of indices between "
            //        + lowerBound.ToString() + " and " + upperBound.ToString() + ".");
            //if (ReturnedString == null)
            //    ReturnedString = new IndexList(length);
            //else
            //    ReturnedString.Clear();
            //while (ReturnedString.Length < length)
            //{
            //    int newIndex = rand.NextInclusive(lowerBound, upperBound);
            //    while (ReturnedString.Contains(newIndex))
            //    {
            //        ++newIndex;
            //        if (newIndex > upperBound)
            //            newIndex = lowerBound;
            //    }
            //    ReturnedString.Add(newIndex);
            //}
            return ret;
        }



        /// <summary>Creates a prescribed number of unique random indices in the specified range.</summary>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="upperBound">Upper bound for generated indices (contained indices are lesser or equal to this value).</param>
        /// <param name="indices">Index list to which the generated indices are stored.</param>
        public static void SetRandom(int length, int upperBound, ref IndexList indices)
        {
            SetRandom(RandomGenerator.Global, length, 0 /* lowerBound */, upperBound, ref indices);
        }

        /// <summary>Creates a prescribed number of unique random indices in the specified range.</summary>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="lowerBound">Lower bound for generated indices (contained indices are greater or equal to this value).</param>
        /// <param name="upperBound">Upper bound for generated indices (contained indices are lesser or equal to this value).</param>
        /// <param name="indices">Index list to which the generated indices are stored.</param>
        public static void SetRandom(int length, int lowerBound, int upperBound, ref IndexList indices)
        {
            SetRandom(RandomGenerator.Global, length, lowerBound, upperBound, ref indices);
        }

        /// <summary>Creates a prescribed number of unique random indices in the specified range 
        /// by using the specified random generator and stores them in the specified index list.
        /// Lower bound of generated indices is 0.</summary>
        /// <param name="rand">Random numbers generator used to generate indices randomly. There is a version of the method without
        /// this argument, which uses the global random generator.</param>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="upperBound">Upper bound for generated indices (contained indices are lesser or equal to this value).</param>
        /// <param name="indices">Index list to which the generated indices are stored.</param>
       public static void SetRandom(IRandomGenerator rand, int length, int upperBound, ref IndexList indices)
        {
            SetRandom(rand, length, 0 /* lowerBound */, upperBound, ref indices);
        }

        /// <summary>Creates a prescribed number of unique random indices in the specified range 
        /// by using the specified random generator and stores them in the specified index list.</summary>
        /// <param name="rand">Random numbers generator used to generate indices randomly. There is a version of the method without
        /// this argument, which uses the global random generator.</param>
        /// <param name="length">Number of random indices contained in the generated list of indices.</param>
        /// <param name="lowerBound">Lower bound for generated indices (contained indices are greater or equal to this value).</param>
        /// <param name="upperBound">Upper bound for generated indices (contained indices are lesser or equal to this value).</param>
        /// <param name="indices">Index list to which the generated indices are stored.</param>
        public static void SetRandom(IRandomGenerator rand, int length, int lowerBound, int upperBound, ref IndexList indices)
        {
            // TODO: 
            // correct the algorithm for generation of random indices!
            // The current algorithm causes clustering because when indices are already clustered at some region (which may occur
            // randomly) then tehre is higher and higher probability that the next indices will be clustered with the current cluster.
            // corrected algorithm should somehow renumber all unoccupies indices and generate each new index for a unique consecutive 
            // position among unoccupied indices counted in this way. In order to finc correct gaps for newly created indices, a kind 
            // of modified bisection can be used (because for each index currently in the list, it can be calculated which would be
            // the number of the first unoccupied position after that index)

            if (length > 1 + upperBound - lowerBound)
                throw new ArgumentException("Requested number of indices (" + length + ") is too large for generation of indices between "
                    + lowerBound.ToString() + " and " + upperBound.ToString() + ".");
            if (indices == null)
                indices = new IndexList(length);
            else
                indices.Clear();
            while (indices.Length<length)
            {
                int newIndex = rand.NextInclusive(lowerBound, upperBound);
                while (indices.Contains(newIndex))
                {
                    ++newIndex;
                    if (newIndex > upperBound)
                        newIndex = lowerBound;
                }
                indices.Add(newIndex);
            }
         }

        #endregion OperationStatic

    }  // class IndexTable

}