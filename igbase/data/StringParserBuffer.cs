// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IG.Lib
{


    /// <summary>Interface for all implementations of read-only character buffer.
    /// Classes that implement this interface are used e.g. in string parsing and output data formatters.</summary>
    /// $A Igor Feb09;
    public interface ICharacterBufferReadOnly
    {


        #region HouseKeeping

        /// <summary>Current length of the buffer or object represented by the buffer.</summary>
        int Length { get; set; }

        /// <summary>Buffer current capacity.</summary>
        int Capacity { get; set; }

        /// <summary>Ensures that the buffer's current capacity is at least the specified number of bytes.</summary>
        /// <param name="minCapacity">Minimal capacity to be ensured.</param>
        /// <returns>Current capacity after the call.</returns>
        int EnsureCapacity(int minCapacity);

        #endregion HouseKeeping


        #region ReadingOperations

        /// <summary>Character with the specified index.</summary>
        char this[int index] { get; }

        #endregion ReadingOperations


        #region Storing_Retrieval

        /// <summary>Saves buffer contents to string and returns it.</summary>
        string ToString();

        /// <summary>Saves buffer contents to a text file.</summary>
        /// <param name="filePath">Path of the file where buffer is stored.</param>
        /// <param name="append">Whether the file is appended. 
        /// If true and if the file already exists then buffer contents are appended at the end of the file.</param>
        void Save(string filePath, bool append);


        /// <summary>Saves buffer contents to a text file.
        /// If the file already exists then it is overwritten.</summary>
        /// <param name="filePath">Path of the file where buffer is stored.</param>
        void Save(string filePath);


        #endregion Storing_Retrieval

    }  // interface ICharacterBufferReadOnly


    /// <summary>Interface for all implementations of read-write character buffer.
    /// Classes that implement this interface are used e.g. in string parsing and output data formatters.</summary>
    /// $A Igor Feb09;
    public interface ICharacterBuffer : ICharacterBufferReadOnly
    {


        /// <summary>Character with the specified index.</summary>
        new char this[int index] { get;  set; }


        #region WritingOperations

        /// <summary>Removes the specified section of the buffer.</summary>
        /// <param name="startIndex">The first index of the removed text.</param>
        /// <param name="length">Length of the removed section (number of characters removed).</param>
        void Delete(int startIndex, int length);

        /// <summary>Appends the specified string at the end of the buffer.</summary>
        void Append(string str);

        /// <summary>Appends the specified character at the end of the buffer.</summary>
        void Append(char ch);

        /// <summary>Appends the specified array of characters at the end of the buffer.</summary>
        void Append(char[] chArray);
        /// <summary>Inserts the specified list of characters at the end of the buffer.</summary>
        /// <param name="chList"></param>
        void Append(List<char> chList);

        /// <summary>Inserts the specified string at the specified position of the buffer.</summary>
        void Insert(int index, string str);

        /// <summary>Inserts the specified character at the specified position of the buffer.</summary>
        void Insert(int index, char ch);

        /// <summary>Inserts the specified array of characters at the specified position of the buffer.</summary>
        void Insert(int index, char[] chArray);

        /// <summary>Inserts the specified list of characters at the specified position of the buffer.</summary>
        void Insert(int index, List<char> chList);

        #endregion WritingOperations

    }  // interface ICharacterBuffer



    /// <summary>CharacterBufferInterface wrapper for StringBilder.</summary>
    /// $A Igor Feb09;
    public class CharacterBuffer : ICharacterBuffer, ICharacterBufferReadOnly
    {

        public CharacterBuffer()
        {
            _str = new StringBuilder();
        }

        public CharacterBuffer(String str)
        {
            this._str = new StringBuilder(str);
        }

        public CharacterBuffer(StringBuilder sb)
        {
            this._str = sb;
            if (sb != null)
                this._str = sb;
            else
                this._str = new StringBuilder();
        }

        protected StringBuilder _str;

        /// <summary>Character with the specified index.</summary>
        public char this[int index] { get { return _str[index]; } set { _str[index] = value; } }

        /// <summary>Current length of the buffer or object represented by the buffer.</summary>
        public int Length { get { return _str.Length; } set { _str.Length = value; } }

        /// <summary>Buffer current capacity.</summary>
        public int Capacity { get { return _str.Capacity; } set { _str.Capacity = value; } }

        /// <summary>Ensures that the buffer's current capacity is at least the specified number of bytes.</summary>
        /// <param name="Capacity">Minimal capacity to be ensured.</param>
        /// <returns>Current capacity after the call.</returns>
        public int EnsureCapacity(int Capacity)
        { return _str.EnsureCapacity(Capacity); }

        #region WritingOperations

        /// <summary>Removes the specified section of the buffer.</summary>
        /// <param name="startIndex">The first index of the removed text.</param>
        /// <param name="length">Length of the removed section (number of characters removed).</param>
        public void Delete(int startIndex, int length)
        {
            _str.Remove(startIndex, length);
        }

        /// <summary>Appends the specified string at the end of the buffer.</summary>
        public void Append(string str)
        {
            if (str!=null)
                _str.Append(str);
        }

        /// <summary>Appends the specified character at the end of the buffer.</summary>
        public void Append(char ch)
        {
            _str.Append(ch);
        }

        /// <summary>Appends the specified array of characters at the end of the buffer.</summary>
        public void Append(char[] chArray)
        {
            if (chArray!=null)
                _str.Append(chArray);
        }

        /// <summary>Inserts the specified list of characters at the end of the buffer.</summary>
        /// <param name="chList"></param>
        public void Append(List<char> chList)
        {
            if (chList!=null)
                _str.Append(chList.ToArray());
        }

        /// <summary>Inserts the specified string at the specified position of the buffer.</summary>
        public void Insert(int index, string str)
        {
            if (str!=null)
                _str.Insert(index, str);
        }

        /// <summary>Inserts the specified character at the specified position of the buffer.</summary>
        public void Insert(int index, char ch)
        {
            _str.Insert(index, ch);
        }

        /// <summary>Inserts the specified array of characters at the specified position of the buffer.</summary>
        public void Insert(int index, char[] chArray)
        {
            if (chArray!=null)
                _str.Insert(index, chArray);
        }

        /// <summary>Inserts the specified list of characters at the specified position of the buffer.</summary>
        public void Insert(int index, List<char> chList)
        {
            if (chList!=null)
                _str.Insert(index, chList.ToArray());
        }


        #endregion WritingOperations



        #region Storing_Retrieval

        /// <summary>Saves buffer contents to string and returns it.</summary>
        public override string ToString()
        {
            return _str.ToString();
        }

        /// <summary>Saves buffer contents to a text file.</summary>
        /// <param name="filePath">Path of the file where buffer is stored.</param>
        /// <param name="append">Whether the file is appended. 
        /// If true and if the file already exists then buffer contents are appended at the end of the file.</param>
        public void Save(string filePath, bool append)
        {
            using (StreamWriter outfile =
                new StreamWriter(filePath, append))
            {
                outfile.Write(_str.ToString());
            }
        }


        /// <summary>Saves buffer contents to a text file.
        /// If the file already exists then it is overwritten.</summary>
        /// <param name="filePath">Path of the file where buffer is stored.</param>
        public void Save(string filePath)
        {
            Save(filePath, false /* append */);
        }

        #endregion Storing_Retrieval


        #region Examples

        /// <summary>Compares speed of pure stringBuilder adnd of CharacterBuffer.</summary>
        public static void TestSpeed()
        {
            double size = 1000; // ize of String buffer
            double tBuilder;  // time spent when using StringBuilder directly 
            double tBuffer;  // time spent when using buffer
            char c;
            int ic;
            int WarmUpCount = 1000;
            int FinalCount = 1000000;

            StopWatch t = new StopWatch();

            StringBuilder sb = new StringBuilder();
            CharacterBuffer buffer = new CharacterBuffer(sb);

            ic = 0;
            for (int i = 0; i < size; ++i)
            {
                if (ic > 255)
                    ic = 0;
                c = (char)ic;
                ++ic;
                sb.Append(c);
            }

            Console.WriteLine("Testing speed of StringBuilder and a derived CharacterBuffer....");
            Console.WriteLine("  Buffer size: " + size);
            Console.WriteLine("  Warmup runs: " + WarmUpCount);
            Console.WriteLine("  Measured runs: " + FinalCount);
            Console.WriteLine("  Total char. reads: " + FinalCount * size);
            Console.WriteLine("Buffers created.");
            Console.WriteLine("    StringBuilder size: " + sb.Length);
            Console.WriteLine("    Buffer size: " + buffer.Length);
            Console.WriteLine();

            Console.WriteLine("Warming up StringBuilder...");

            t.Reset();
            t.Start();
            for (int i = 0; i < WarmUpCount; ++i)
            {
                for (int j = 0; j < size; ++j)
                    c = sb[j];
            }
            t.Stop();
            Console.WriteLine("... finished in " + t.Time + "s");
            Console.WriteLine("StringBuilder reads...");
            t.Reset();
            t.Start();
            for (int i = 0; i < FinalCount; ++i)
            {
                for (int j = 0; j < size; ++j)
                    c = sb[j];
            }
            t.Stop();
            tBuilder = t.Time;
            Console.WriteLine("... finished in " + t.Time + "s");



            Console.WriteLine("Warming up Buffer...");

            t.Reset();
            t.Start();
            for (int i = 0; i < WarmUpCount; ++i)
            {
                for (int j = 0; j < size; ++j)
                    c = buffer[j];
            }
            t.Stop();
            Console.WriteLine("... finished in " + t.Time + "s");


            Console.WriteLine("Buffer reads...");
            t.Reset();
            t.Start();
            for (int i = 0; i < FinalCount; ++i)
            {
                for (int j = 0; j < size; ++j)
                    c = buffer[j];
            }
            t.Stop();
            tBuffer = t.Time;
            Console.WriteLine("... finished in " + t.Time + "s");

            Console.WriteLine();
            Console.WriteLine("Stringbuilder:   t = " + tBuilder + " s, " +
                1.0e-6 * size * FinalCount / tBuilder + " Mega op. / second");
            Console.WriteLine("CharacterBuffer: t = " + tBuffer + " s, " +
                1.0e-6 * size * FinalCount / tBuffer + " Mega op. / second");
            Console.WriteLine();

        }

        #endregion Examples

    }  // class CharacterBuffer

    



}