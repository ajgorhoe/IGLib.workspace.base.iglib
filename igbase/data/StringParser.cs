// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IG.Lib
{


    /// <summary>Parsig data from character arrays.
    /// Provides various parsing and writing utilities on character buffers.
    /// $A Igor Feb09;</summary>
    public class StringParser: IStringParser
    {


        // TODO: remove this example class after the globalization concepts ar used in the code!
        class LocalizationExample
        {
            static System.Globalization.NumberFormatInfo ni = null;
            public LocalizationExample()
            {
                System.Globalization.CultureInfo ci =
                   System.Globalization.CultureInfo.InstalledUICulture;
                ni = (System.Globalization.NumberFormatInfo)
                   ci.NumberFormat.Clone();
                ni.NumberDecimalSeparator = ".";
            }
            public void someMethod()
            {
                double someNumber;
                string myNumber = "123.4";
                try
                {
                    someNumber = double.Parse(myNumber, ni);
                }
                catch (Exception)
                {
                }
            }
        }


        #region Construction

        public StringParser()
        {
            Buffer = new CharacterBuffer();
        }

        public StringParser(String str)
        {
            this.Buffer = new CharacterBuffer(str);
        }

        public StringParser(StringBuilder sb)
        {
            this.Buffer = new CharacterBuffer(sb);
        }

         protected ICharacterBuffer _buffer; // StringBuilder _str;

        protected int _pos = 0;

        protected ICharacterBuffer Buffer
        { 
            get { return _buffer; }
            set { _buffer = value; }
        }

        #endregion Construction


        #region ThreadLocking

        private object _mainLock = new object();

        /// <summary>This object's central lock object to be used by other object.
        /// Do not use this object for locking in class' methods, for this you should use 
        /// InternalLock.</summary>
        public object Lock { get { return _mainLock; } }

        private object _internalLock = new object();

        /// <summary>Used internally for locking access to internal fields.</summary>
        protected object InternalLock { get { return _internalLock; } }

        //private object waitlock = new object();

        ///// <summary>Must be used only for locking waiting the Waiting() block (since it is potentially time consuming).</summary>
        //protected object WaitLock { get { return waitlock; } }

        #endregion ThreadLocking


        #region BasicOperation

        /// <summary>Current position.</summary>
        public int Position
        {  
            get { return _pos; }
            set { _pos = value; }
        }

        /// <summary>Current length of the buffer.</summary>
        public int Length
        {
            get { return Buffer.Length; }

        }

        /// <summary>Ensures that the buffer has at least the specified capacity.</summary>
        /// <param name="capacity"></param>
        public void EnsureCapacity(int capacity)
        {
            lock (_internalLock)
            {
                if (capacity > Buffer.Capacity)
                    Buffer.Capacity = capacity;
            }
        }

        #endregion BasicOperation


        #region OperationData

        protected List<char> _spaces;

        protected List<char> _openBrackets;

        protected List<char> _closedBrackets;

        public virtual Boolean UseThousandSeparator 
        {
            protected set {  }
            get { return false; }
        }

        protected char _decimalPoint = '.';

        protected char _thousandSeparator = ',';

        char[] _expC = { 'e', 'E', 'g', 'G' };
        String[] _expS = {"*10^"};

        protected List<char> _exponentChars = null;   // new List<char>();

        protected List<String> _exponentStrings = null; // new List<string>();

        #endregion OperationData


        #region CharacterHandling

        private bool _useEscapeSequences = false;

        private bool _useCharacterReferences = false;

        /// <summary>Returns the escape character of the current string parser.</summary>
        public virtual char EscapeCharacter
        {
            get { return _escapeChar; }
            protected set { _escapeChar = value; }
        }

        /// <summary>Whether or not escape characters are used.
        /// Evaluates to true only if the appropriate flag is set and also any escape seuences 
        /// are defined (setter just sets the flag).
        /// With escape characters, a combination of the escape character and another character
        /// substitutes a special character.</summary>
        public bool UseEscapeSeuences 
        {
            get {
                if (_useEscapeSequences)
                    if (_escapeSequences != null)
                        if (_escapeSequences.Count > 0)
                            return true;
            return false;
            }
            set { _useEscapeSequences = value; }
        }


        /// <summary>Whether or not character references are used.
        /// Evaluates to true only if the appropriate flag is set and also any character replacements 
        /// are defined (setter just sets the flag).
        /// With escape characters, a combination of the escape character and another character
        /// substitutes a special character.</summary>
        public bool UseCharacterReferences
        {
            get { return _useCharacterReferences; }
            set { _useCharacterReferences = value; }
        }

        public bool UseCharacterReplacement
        { get { return (UseEscapeSeuences || UseCharacterReferences); } }

        protected char _escapeChar = '\\';
        protected List<KeyValueSortable<char, char>> _escapeSequences;
        protected List<KeyValueSortable<char, char>> _inverseEscapeSequences;

        private List<KeyValueSortable<string, char>> _characterReferences;
        private List<KeyValueSortable<string, char>> _inverseCharacterReferences;



        /// <summary>Initializes data structures used for escape sequences.</summary>
        protected void initEscapeSequences()
        {
            if (_escapeSequences == null)
            {
                _escapeSequences = new List<KeyValueSortable<char, char>>();
                _inverseEscapeSequences = new List<KeyValueSortable<char, char>>();
            }
        }

        /// <summary>Adds a new escape sequence definition to the parser.</summary>
        public void AddEscapeSequence(char code, char character)
        {
            lock(_internalLock)
            {
                initEscapeSequences();
                KeyValueSortable<char, char> item = new KeyValueSortable<char, char>(code, character);
                int itemIndex = _escapeSequences.BinarySearch(item, KeyValueSortable<char, char>.CompareKeyValue);
                if (itemIndex < 0)
                {
                    // This escape sequence has not been added yet, do it now:
                    _escapeSequences.Add(item);
                    _escapeSequences.Sort(KeyValueSortable<char, char>.CompareKeyValueStrict);
                    _inverseEscapeSequences.Add(item);
                    _inverseEscapeSequences.Sort(KeyValueSortable<char, char>.CompareValueKeyStrict);
                }
            }
        }

        /// <summary>Removes the specified escape sequence from the parser.</summary>
        public void RemoveEscapeSequence(char code, char character)
        {
            lock (_internalLock)
            {
                initEscapeSequences();
                KeyValueSortable<char, char> item = new KeyValueSortable<char, char>(code, character);
                int itemIndex = _escapeSequences.BinarySearch(item, KeyValueSortable<char, char>.CompareKeyValue);
                int itemIndexInverse = _inverseEscapeSequences.BinarySearch(item, KeyValueSortable<char, char>.CompareValueKey);
                if (itemIndex >= 0 && itemIndexInverse >= 0)
                {
                    _escapeSequences.RemoveAt(itemIndex);
                    _escapeSequences.RemoveAt(itemIndexInverse);
                }
            }
        }


        /// <summary>Initializes data structures used for character references.</summary>
        protected void initCharacterReferences()
        {
            if (_characterReferences == null)
            {
                _characterReferences = new List<KeyValueSortable<string, char>>();
                _inverseCharacterReferences = new List<KeyValueSortable<string, char>>();
            }
        }

        /// <summary>Adds a new character reference definition to the parser.</summary>
        public void AddCharacterReferences(string code, char character)
        {
            lock (_internalLock)
            {
                initCharacterReferences();
                KeyValueSortable<string, char> item = new KeyValueSortable<string, char>(code, character);
                int itemIndex = _characterReferences.BinarySearch(item, KeyValueSortable<string, char>.CompareKeyValue);
                if (itemIndex < 0)
                {
                    // This escape sequence has not been added yet, do it now:
                    _characterReferences.Add(item);
                    _characterReferences.Sort(KeyValueSortable<string, char>.CompareKeyValueStrict);
                    _inverseCharacterReferences.Add(item);
                    _inverseCharacterReferences.Sort(KeyValueSortable<string, char>.CompareValueKeyStrict);
                }
            }
        }

        /// <summary>Removes the specified character reference from the parser.</summary>
        public void RemoveEscapeSequence(string code, char character)
        {
            lock (_internalLock)
            {
                initCharacterReferences();
                KeyValueSortable<string, char> item = new KeyValueSortable<string, char>(code, character);
                int itemIndex = _characterReferences.BinarySearch(item, KeyValueSortable<string, char>.CompareKeyValue);
                int itemIndexInverse = _inverseCharacterReferences.BinarySearch(item, KeyValueSortable<string, char>.CompareValueKey);
                if (itemIndex >= 0 && itemIndexInverse >= 0)
                {
                    _characterReferences.RemoveAt(itemIndex);
                    _characterReferences.RemoveAt(itemIndexInverse);
                }
            }
        }


        /// <summary>Gets a character represented by a character reference at the specified position.
        /// If character references are not defined on the current parser.
        /// WARNING: Does not check consistency of input parameters!</summary>
        /// <param name="from">Position at which character reference is obtained.</param>
        /// <param name="to">Last position until which the character reference can last.</param>
        /// <param name="value">Character represented by the character reference.</param>
        /// <returns>Position right after the character reference, or -1 if not found.</returns>
        protected int getCharFromReferenceNoCheck(int from, int to, ref char value)
        {
            if (Length >= from+1)
            {
                foreach (KeyValueSortable<string, char> def in _characterReferences)
                {
                    if (def != null)
                    {
                        string chRef = def.Key;
                        if (chRef != null)
                        {
                            int refLength = chRef.Length;
                            if (from + refLength <= Length)
                            {
                                bool isThisRef = true;
                                for (int i = 0; i < refLength; ++i)
                                {
                                    if (chRef[i] != Buffer[from + i])
                                    {
                                        isThisRef = false;
                                        break;
                                    }
                                }
                                if (isThisRef)
                                {
                                    value = def.Value;
                                    return from+refLength;
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>Gets a character represented by an escape sequence at the specified position.
        /// If character references are not defined on the current parser.
        /// Does not check input parameters, tries to convert an escape sequence to character even
        /// if internal parameters specify that there are no escape sequences.
        /// TODO: implement this method!</summary>
        /// <param name="from">Position at which character reference is obtained.</param>
        /// <param name="to">Last position until which the character reference can last.</param>
        /// <param name="value">Character represented by the character reference.</param>
        /// <returns>Position right after the character reference, or -1 if not found.</returns>
        protected int getCharFromEscapeNoCheck(int from, int to, ref char value)
        {
            if (Buffer[from] == EscapeCharacter && _escapeSequences!=null && from+2 <= Length)
            {
                foreach (KeyValueSortable<char, char> def in _escapeSequences)
                {
                    if (def!=null)
                        if (def.Key == Buffer[from + 1])
                        {
                            value = def.Value;
                            return from + 2;
                        }
                }
            }
            return -1;
        }

        /// <summary>Returns position rightr after the character that resides at position from and the value of that character
        /// as output argument. If the character can not be obtained (e.g. because of overflow, etc.), -1 is returned.</summary>
        /// <param name="from">Position at which character is seen.</param>
        /// <param name="to">Position until which (inclusively) character is parsed in case that 
        /// character references or escape sequences are used.</param>
        /// <param name="useEscape">Whether to use character replacement (escape sequences and character references).
        /// Whether the flag actually has effect depends also on internal settings of the string parser.</param>
        /// <param name="value">Returns value of the character at the specified position (if a valid value can be obtained).</param>
        /// <returns>Position of the first character after the obtained character.</returns>
        public int getChar(int from, int to, bool useEscape, ref char value)
        {
            lock(_internalLock)
            {
                int length = Length;
                if (from < 0 || from > length || to < from)
                {
                    return -1;
                }
                if (!useEscape || (!UseCharacterReplacement))
                {
                    value = Buffer[from];
                    return from+1;
                } else
                {
                    if (UseCharacterReferences)
                    {
                        int ret = getCharFromReferenceNoCheck(from, to, ref value);
                        if (ret >= 0)
                            return ret;
                    }
                    if (UseEscapeSeuences)
                    {
                        int ret = getCharFromEscapeNoCheck(from, to, ref value);
                        if (ret >= 0)
                            return ret;
                    }
                    value = Buffer[from];
                    return from + 1;
                }
            }
        }




        public int getPosAfterExponent(int startPos)
        {
            lock (_internalLock)
            {
                if (_exponentChars != null)
                {
                    foreach (char ch in _exponentChars)
                    {
                        if (ch == Buffer[_pos])
                            return _pos + 1;
                    }
                }
                if (_exponentStrings != null)
                {
                    foreach (String str in _exponentStrings)
                    {
                        // TODO: implement this!!
                    }
                }
                return -1;
            }
        }

        /// <summary>Whether the specified character represents a space for the current parser.</summary>
        public bool isSpace(char c)
        {
            lock (_internalLock)
            {
                if (_spaces == null)
                    return false;
                else return (_spaces.IndexOf(c) >= 0);
            }
        }

        /// <summary>Whether the specified character represents an open bracket for the current parser.</summary>
        public bool isOpenBracket(char c)
        {
            lock (_internalLock)
            {
                if (_openBrackets == null)
                    return false;
                else return (_openBrackets.IndexOf(c) >= 0);
            }
        }

        /// <summary>Whether the specified character represents a closed bracket for the current parser.</summary>
        protected bool isCloseBracket(char c)
        {
            lock (_internalLock)
            {
                if (_closedBrackets == null)
                    return false;
                else return (_closedBrackets.IndexOf(c) >= 0);
            }
        }


        /// <summary>Index of the specified character among open or closed brackets, or 0 if the 
        /// character does not represent neither open nor closed bracket.</summary>
        protected int bracketIndex(char c)
        {
            int ret = -1;
            lock (_internalLock)
            {
                if (_openBrackets != null)
                    ret = _openBrackets.IndexOf(c);
                if (ret < 0 && _closedBrackets != null)
                    ret = _closedBrackets.IndexOf(c);
            }
            return ret;
        }

        #endregion CharacterHandling

#region Global

        public static StringParser _global = null;

        public static StringParser Global
        {
            get
            {
                if (_global==null)
                    _global = new StringParser();
                return _global;
            }
        }

        public void Set(String str)
        {
            lock (_internalLock)
            {
                Buffer = new CharacterBuffer(str);
            }
        }

        public void Set(StringBuilder sb)
        {
            lock (_internalLock)
            {
                Buffer = new CharacterBuffer(sb);
            }
        }

#endregion Global






    }  // class StringParser









}  // namespace IG.Lib
















