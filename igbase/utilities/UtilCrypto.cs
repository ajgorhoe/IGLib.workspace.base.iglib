// Copyright (c) Igor Grešovnik (2009 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using System.Security.Cryptography;

using IG.Lib;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;

namespace IG.Crypto
{


    // See:
    // Writing secure code - MSDN: https://msdn.microsoft.com/en-us/security/aa570401.aspx 
    // https://www.owasp.org/index.php/Guide_to_Cryptography#Algorithm_Selection  


    /// <summary>Basic Cryptographic utilities.</summary>
    /// <remarks>
    /// <para>Warnings: </para>
    /// <para>  - Functions of this class must be fixed (may not change over time) in order not to break any security instruments.</para>
    /// <para>  - Functions can only be renamd (always by refactoring tools that change all calls to functions).</para>
    /// <para>  - For IGLib functions (IGLib specific supplements of standards), these should only be used through standard shell applications.
    /// In such a way security tools can not be broken, as shell functions are more guaranteed to behave constantly over time.</para>
    /// <para></para><para>See also: </para>
    /// <para> - Cryptographic hashRet function: http://en.wikipedia.org/wiki/Cryptographic_hash_function </para>
    /// <para>  - Password verification: http://en.wikipedia.org/wiki/Cryptographic_hash_function#Password_verification </para>
    /// <para>  - Storing passwords: http://www.aspheute.com/english/20040105.asp </para>
    /// <para>   - How to encrypt user passwords: http://www.jasypt.org/howtoencryptuserpasswords.html </para>
    /// <para>  - Salt: https://en.wikipedia.org/wiki/Salt_(cryptography) </para>
    /// <para>  - Key stretching: http://en.wikipedia.org/wiki/Key_stretching </para>
    /// </remarks>
    /// $A Igor Apr10 Jun15;
    public static class UtilCrypto
    {

        private static int _outputLevel = 1;

        /// <summary>Output level for static methods of the current utility class.
        /// <para>Warning: This field can be removed at any time.</para></summary>
        public static int OutputLevel { get { return _outputLevel; } }  /* for debugging purposes */

        private static object _lockStatic = null;

        /// <summary>Locking object for static methods and properties of this class.</summary>
        public static object LockStatic
        {
            get
            {
                if (_lockStatic == null)
                {
                    lock(Util.LockGlobal)
                    {
                        if (_lockStatic == null)
                            _lockStatic = new object();
                    }
                }
                return _lockStatic;
            }
        }


        /// <summary>Gets the character encoding that is used for calculating hashes of strings.</summary>
        /// <remarks>
        /// <para>About thread safety (Enncoding classes should be stateless):</para>
        /// <para> http://stackoverflow.com/questions/3024384/thread-safety-and-system-text-encoding-in-c-sharp </para>
        /// </remarks>
        public static Encoding StringEncoding
        {
            //TODO: check whether it is thread safe to use encoding object returned in this way!
            // You can check the following: http://stackoverflow.com/questions/3024384/thread-safety-and-system-text-encoding-in-c-sharp

            get { return Encoding.UTF8; }
        }



        #region Passwords


        #region Passwords.General


        private static PasswordAlgorithmType[] _passwordAlgorithmTypes = null;

        /// <summary>Returns all password algorithm types contained in the enumeration <see cref="PasswordAlgorithmType"/></summary>
        public static PasswordAlgorithmType[] GetPasswordAlgorithmTypes()
        {
            if (_passwordAlgorithmTypes == null)
            {
                List<PasswordAlgorithmType> allTypes = new List<PasswordAlgorithmType>();
                lock (Util.LockGlobal)
                {
                    if (_passwordAlgorithmTypes == null)
                    {
                        IEnumerable<PasswordAlgorithmType> tmpTypes = Util.GetEnumValues<PasswordAlgorithmType>();
                        foreach (PasswordAlgorithmType specType in tmpTypes)
                            allTypes.Add(specType);
                        _passwordAlgorithmTypes = allTypes.ToArray();
                    }
                }
            }
            return _passwordAlgorithmTypes;
        }


        /// <summary>Returns a <see cref="PasswordAlgorithmType"/> value corresponding to the specified string representation.</summary>
        /// <param name="flagString">String that represents the <see cref="PasswordAlgorithmType"/> value.</param>
        public static PasswordAlgorithmType GetPasswordAlgorithmType(string typeString)
        {
            if (typeString != null)
                typeString = typeString.ToUpper();
            try
            {
                PasswordAlgorithmType type = (PasswordAlgorithmType)Enum.Parse(typeof(PasswordAlgorithmType), typeString);
                return type;
            }
            catch { }
            if (string.IsNullOrEmpty(typeString))
                return PasswordAlgorithmType.Default;
            typeString = typeString.ToUpper();
            if (typeString == "NONE")
                return PasswordAlgorithmType.None;
            if (typeString == "RFC")
                return PasswordAlgorithmType.Rfc2898;
            if (typeString == "RFC2898")
                return PasswordAlgorithmType.Rfc2898;
            if (typeString == "DERIVEBYTES")
                return PasswordAlgorithmType.DeriveBytes;
            throw new ArgumentException("Did not recognize the following password algorithm type: " + typeString);
            // return PasswordAlgorithmType.None;
        }


        /// <summary>Returns the password generation algorithm type, i.e. an enumeration of type <see cref="PasswordAlgorithmType"/>,
        /// for the specified password generation algorithm object.</summary>
        /// <param name="algorithmObject">Password generaton algorithm object for which type enumeration is returned.</param>
        /// <returns>The type enumeration corresponding to the specified algorithm, or 0 (None) if the algorithm is null or not
        /// contained in the enumeration.</returns>
        public static PasswordAlgorithmType GetPasswordAlgorithmType(PasswordAlgorithmBase algorithmObject)
        {
            if (algorithmObject == null)
                return PasswordAlgorithmType.None;
            if (algorithmObject is PasswordAlgorithmRfc2898)
                return PasswordAlgorithmType.Rfc2898;
            if (algorithmObject is PasswordAlgorithmDeriveBytes)
                return PasswordAlgorithmType.DeriveBytes;
            return PasswordAlgorithmType.None;
        }


        /// <summary>Returns standard string representation of the specified password generation algoithm.</summary>
        /// <param name="">Password algorithm type whose string representation is returned.</param>
        public static string PasswordAlgorithmTypeToString(PasswordAlgorithmType passwordAlgorithmType)
        {
            switch (passwordAlgorithmType)
            {
                case PasswordAlgorithmType.None:
                    return "None";
                case PasswordAlgorithmType.Rfc2898:
                    return "Rfc2898";
                case PasswordAlgorithmType.DeriveBytes:
                    return "DeriveBytes";
                default:
                    return passwordAlgorithmType.ToString();
            }
        }


        private static PasswordAlgorithmBase _passwordAlgorithmNone = null;

        /// <summary>Returns password algorithm that does not change the password provided as initialization
        /// parameters.
        /// <para>The same object is returned by each call.</para></summary>
        private static PasswordAlgorithmBase GetPasswordAlgorithmNoneSingleton()
        {
            if (_passwordAlgorithmNone == null)
            {
                lock(Util.LockGlobal)
                {
                    if (_passwordAlgorithmNone == null)
                        _passwordAlgorithmNone = new PasswordAlgorithmNone();
                }
            }
            return _passwordAlgorithmNone;
        }

        /// <summary>Returns the appropriate password generation algorithm according to the specified algorithm type, or null 
        /// if the type is not recognized or the method is not implemented for that type.</summary>
        /// <param name="flag">Specification of the password generaion algorithm type.</param>
        public static IG.Crypto.PasswordAlgorithmBase GetPasswordAlgorithm(PasswordAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case PasswordAlgorithmType.Rfc2898:
                    return new PasswordAlgorithmRfc2898();
                case PasswordAlgorithmType.DeriveBytes:
                    return new PasswordAlgorithmDeriveBytes();
                default:
                    return GetPasswordAlgorithmNoneSingleton();
            }
        }

        /// <summary>Returns true if the specified password generation algorithm corresponds the type specification, false otherwise.
        /// <para>If the specified algorithm is null or the specified type is unknown then false is returned.</para></summary>
        /// <param name="flag">Specification of the password derivation nalgorithm type.</param>
        /// <param name="algorithm">Algorithm that is checked for type correctness.</param>
        public static bool IsCorrectPasswordAlgorithm(DeriveBytes algorithm, PasswordAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case PasswordAlgorithmType.None:
                    return true;  // type not specified, all types are valid.
                case PasswordAlgorithmType.Rfc2898:
                    return (algorithm as Rfc2898DeriveBytes != null);
                case PasswordAlgorithmType.DeriveBytes:
                    return (algorithm as PasswordDeriveBytes != null);
                default:
                    return false;
            }
        }


        #endregion Passwords.General


        /// <summary>Creates and returns a random password consisting of only allowed characters from the specified string.</summary>
        /// <remarks>Cryptographically secure random numbers are used to select characters for the password from 
        /// the array of allowed characters.</remarks>
        /// <param name="PasswordLength">Length of the generated password.</param>
        /// <param name="allowedChars">String containing allowed characters of the password.
        /// <para>By default (if parameter is a null or empty string), these are digits and lower- and upper- case English letters.</para></param>
        /// <returns>A random  password of specified length.</returns>
        public static string CreateRandomPassword(int PasswordLength, string allowedChars = null)
        {
            if (string.IsNullOrEmpty(allowedChars))
                allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ23456789";
            Byte[] randomBytes = new Byte[PasswordLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            char[] chars = new char[PasswordLength];
            int allowedCharCount = allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = allowedChars[(int)randomBytes[i] % allowedCharCount];
            }
            return new string(chars);

            // return UtilStr.RandomString(PasswordLength, allowedChars, null /* randomGenerator */);
        }


        private static RNGCryptoServiceProvider _rng = null;

        /// <summary>Object used to generate random bytes.
        /// <remarks>Object is created on demand (layzy evaluation). Ony one object is used through appllication lifetime.</remarks></summary>
        private static RNGCryptoServiceProvider Rng
        {
            get {
                if (_rng == null)
                {
                    lock(LockStatic)
                    {
                        if (_rng == null)
                            _rng = new RNGCryptoServiceProvider();
                    }
                }
                return _rng;
            }
        }

        /// <summary>Fills the specified byte array with a cryptographically strong sequence of random bytes.</summary>
        /// <param name="byteArray">Byte array that is filled with random bytes.</param>
        public static void GetRandomBytes(byte[] byteArray)
        {
            lock (LockStatic)
            {
                Rng.GetBytes(byteArray);
            }
        }

        /// <summary>Fills the specified byte array with a specified number of cryptographically strong random bytes.
        /// <para>Byte array is created anew if its size does not equal the specified number of bytes.</para></summary>
        /// <param name="byteArray">Byte array that is filled with random bytes. Reallocated if necessary.</param>
        /// <param name="numBytes">Number of bytes to be filled into the array.</param>
        public static void GetRandomBytes(ref byte[] byteArray, int numBytes)
        {
            if (byteArray == null)
            {
                if (numBytes > 0)
                    byteArray = new byte[numBytes];
            } else if (byteArray.Length != numBytes)
                byteArray = new byte[numBytes];
            if (numBytes > 0) 
                Rng.GetBytes(byteArray);
        }

        /// <summary>Creates and returns an array of cryptographically strong random bytes of the specified size.</summary>
        /// <param name="numBytes">Number of bytes to be filled into the array.</param>
        public static byte[] GetRandomBytes(int numBytes)
        {
            if (numBytes <= 0)
                throw new ArgumentException("Length of a random byte array shoulld be at least 1.");
            byte[] ret = new byte[numBytes];
            GetRandomBytes(ret);
            return ret;
        }

        #endregion Passwords



        #region Hash

        #region Hash.General

        /// <summary>Returns length of the hashRet value, in bytes, for the specified hashRet algorithm.
        /// <para>-1 is returned if the length is not known.</para></summary>
        /// <param name="HashAlgorithmType">Type of the hashing algorithm.</param>
        public static int GetHashLengthBytes(HashType hashType)
        {
            switch (hashType)
            {
                case HashType.MD5:
                    return 16;
                case HashType.SHA1:
                    return 20;
                case HashType.SHA256:
                    return 32;
                case HashType.SHA512:
                    return 64;
            }
            return -1;
        }

        /// <summary>Returns length of the HEXADECIMAL hashRet string for the specified hashRet algorithm.
        /// <para>-1 is returned if the length is not known.</para></summary>
        /// <param name="HashAlgorithmType">Type of the hashing algorithm.</param>
        public static int GetHashLengthHex(HashType hashType)
        {
            int length = GetHashLengthBytes(hashType);
            if (length <= 0)
                return length;
            else
                return 2 * length;
        }

        private static HashType[] _hashTypes = null;

        /// <summary>Returns all hashing algorithm types contained in the enumeration <see cref="HashType"/></summary>
        public static HashType[] GetHashTypes()
        {
            if (_hashTypes == null)
            {
                List<HashType> allTypes = new List<HashType>();
                lock (Util.LockGlobal)
                {
                    if (_hashTypes == null)
                    {
                        IEnumerable<HashType> tmpTypes = Util.GetEnumValues<HashType>();
                        foreach (HashType specType in tmpTypes)
                            allTypes.Add(specType);
                        _hashTypes = allTypes.ToArray();
                    }
                }
            }
            return _hashTypes;
        }


        /// <summary>Returns a <see cref="HashType"/> value corresponding to the specified string representation.</summary>
        /// <param name="flagString">String that represents the hashRet type.</param>
        public static HashType GetHashType(string typeString)
        {
            if (typeString != null)
                typeString = typeString.ToUpper();
            try
            {
                HashType type = (HashType)Enum.Parse(typeof(HashType), typeString);
                return type;
            }
            catch { }
            if (string.IsNullOrEmpty(typeString))
                return HashType.Default;
            //flagString = flagString.ToUpper();
            if (typeString == "NONE")
                return HashType.None;
            if (typeString == "MD5")
                return HashType.MD5;
            if (typeString == "SHA1" || typeString == "SHA-1")
                return HashType.SHA1;
            if (typeString == "SHA256" || typeString == "SHA-256")
                return HashType.SHA256;
            if (typeString == "SHA512" || typeString == "SHA-512")
                return HashType.SHA512;
            if (typeString == "SHA512" || typeString == "SHA-512")
                return HashType.SHA512;
            if (typeString == "DEFAULT")
                return HashType.Default;
            return HashType.None;
        }


        /// <summary>Returns the hashing algorithm type, i.e. an enumeration of type <see cref="HashType"/>,
        /// for the specified hashing algorithm object.</summary>
        /// <param name="algorithmObject">Hashing algorithm object for which type enumeration is returned.</param>
        /// <returns>The type enumeration corresponding to the specified algorithm, or 0 (None) if the algorithm is null or not
        /// contained in the enumeration.</returns>
        public static HashType GetHashType(HashAlgorithm algorithmObject)
        {
            if (algorithmObject == null)
                return HashType.None;
            if (algorithmObject is System.Security.Cryptography.MD5)
                return HashType.MD5;
            if (algorithmObject is System.Security.Cryptography.SHA1)
                return HashType.SHA1;
            if (algorithmObject is System.Security.Cryptography.SHA256)
                return HashType.SHA256;
            if (algorithmObject is System.Security.Cryptography.SHA512)
                return HashType.SHA512;
            return HashType.None;
        }


        /// <summary>Returns standard string representation of the specified hashRet type.</summary>
        /// <param name="HashAlgorithmType">Hash type whose string representation is returned.</param>
        public static string HashTypeToString(HashType hashType)
        {
            switch (hashType)
            {
                case HashType.None:
                    return "None";
                case HashType.MD5:
                    return "MD5";
                case HashType.SHA1:
                    return "SHA-1";
                case HashType.SHA256:
                    return "SHA-256";
                case HashType.SHA512:
                    return "SHA-512";
                default:
                    return hashType.ToString();
            }
        }


        /// <summary>Returns the appropriate hashRet algorithm according to the specified hashRet type, or null 
        /// if the type is not recognized or the method is not implemented for that type.</summary>
        /// <param name="HashAlgorithmType">Specification of the hashRet algorithm type.</param>
        public static HashAlgorithm GetHashAlgorithm(HashType hashType)
        {
            switch (hashType)
            {
                case HashType.MD5:
                    return new MD5CryptoServiceProvider();
                case HashType.SHA1:
                    return new SHA1Managed();
                case HashType.SHA256:
                    return new SHA256Managed();
                case HashType.SHA512:
                    return new SHA512Managed();
                default:
                    return null;
            }
        }

        /// <summary>Returns true if the specified cryptographic hashRet algorithm corresponds the type specification, false otherwise.
        /// <para>If the specified algorithm is null or the specified type is unknown then false is returned.</para></summary>
        /// <param name="flag">Specification of the cryptographic hashRet algorithm type.</param>
        /// <param name="algorithm">Algorithm that is checked for type correctness.</param>
        public static bool IsCorrectHashAlgorithm(HashAlgorithm algorithm, HashType algorithmType)
        {
            switch (algorithmType)
            {
                case HashType.None:
                    return true;  // type not specified, all types are valid.
                case HashType.MD5:
                    return (algorithm as MD5CryptoServiceProvider != null);
                case HashType.SHA1:
                    return (algorithm as SHA1Managed != null);
                case HashType.SHA256:
                    return (algorithm as SHA256Managed != null);
                case HashType.SHA512:
                    return (algorithm as SHA512Managed != null);
                default:
                    return false;
            }
        }



        /// <summary>Parses the file containing hashRet values of one or more files, and adds the parsed
        /// pairs {hashRet, inputFilePath} to the specified list.
        /// <para>File must be in the standard format where each line contains a hashRet value and the path to 
        /// the corresponding file separated from hashRet value by one or more spaces.</para>
        /// <para>List is allocated if necessary. Eventual existent pairs on the list are not affected.</para></summary>
        /// <param name="inputFilePath">Path to the file that is parsed.</param>
        /// <param name="hashList">List to which which parsed pairs {hashRet, inputFilePath} are added in form of arrays of 2 strings.</param>
        /// <remarks><para>Example contents of the file: </para>
        /// <para>595f44fec1e92a71d3e9e77456ba80d1  filetohashA.txt</para>
        /// <para>71f920fa275127a7b60fa4d4d41432a3  filetohashB.txt</para>
        /// <para>43c191bf6d6c3f263a8cd0efd4a058ab  filetohashC.txt</para>
        ///</remarks>
        public static void ParseHashFile(string filePath, ref List<string[]> hashList)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path not specified (null or empty string).");
            if (!File.Exists(filePath))
                throw new ArgumentException("File containing hashes does not exist. " + Environment.NewLine
                    + "  File path: " + filePath);
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    char[] chars = line.ToArray();
                    int length = chars.Length;
                    int firstSpace = -1;
                    int firstNonSpace = -1;
                    int pos = 0;
                    // Find the end of the hashRet string:
                    while (pos < length && firstSpace < 0)
                    {
                        if (char.IsWhiteSpace(chars[pos]))
                            firstSpace = pos;
                        ++pos;
                    }
                    if (firstSpace > 0)
                    {
                        while (pos < length && firstNonSpace < 0)
                        {
                            if (!char.IsWhiteSpace(chars[pos]))
                                firstNonSpace = pos;
                            ++pos;
                        }
                    }
                    if (firstSpace > 0 && firstNonSpace > firstSpace)
                    {
                        string hash = new string(chars, 0, firstSpace);
                        string hashedFile = new string(chars, firstNonSpace, length - firstNonSpace);
                        if (hashList == null)
                            hashList = new List<string[]>();
                        hashList.Add(new string[] { hash, hashedFile });
                    }
                }
            }
        }


        /// <summary>Parses the string containing hashRet values of one or more files, and adds the parsed
        /// pairs {hashRet, inputFilePath} to the specified list.
        /// <para>String must be in the standard format where each line contains a hashRet value and the path to 
        /// the corresponding file separated from hashRet value by one or more spaces.</para>
        /// <para>List is allocated if necessary. Eventual existent pairs on the list are not affected.</para></summary>
        /// <param name="str">String that is parsed.</param>
        /// <param name="hashList">List to which which parsed pairs {hashRet, inputFilePath} are added in form of arrays of 2 strings.</param>
        /// <remarks><para>Example contents of the string: </para>
        /// <para>595f44fec1e92a71d3e9e77456ba80d1  filetohashA.txt</para>
        /// <para>71f920fa275127a7b60fa4d4d41432a3  filetohashB.txt</para>
        /// <para>43c191bf6d6c3f263a8cd0efd4a058ab  filetohashC.txt</para>
        ///</remarks>
        public static void ParseHashFileString(string str, ref List<string[]> hashList)
        {
            if (string.IsNullOrEmpty(str))
                return;
            using (StringReader reader = new StringReader(str))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    char[] chars = line.ToArray();
                    int length = chars.Length;
                    int firstSpace = -1;
                    int firstNonSpace = -1;
                    int pos = 0;
                    // Find the end of the hashRet string:
                    while (pos < length && firstSpace < 0)
                    {
                        if (char.IsWhiteSpace(chars[pos]))
                            firstSpace = pos;
                        ++pos;
                    }
                    if (firstSpace > 0)
                    {
                        while (pos < length && firstNonSpace < 0)
                        {
                            if (!char.IsWhiteSpace(chars[pos]))
                                firstNonSpace = pos;
                            ++pos;
                        }
                    }
                    if (firstSpace > 0 && firstNonSpace > firstSpace)
                    {
                        string hash = new string(chars, 0, firstSpace);
                        string hashedFile = new string(chars, firstNonSpace, length - firstNonSpace);
                        if (hashList == null)
                            hashList = new List<string[]>();
                        hashList.Add(new string[] { hash, hashedFile });
                    }
                }
            }
        }


        #endregion Hash.General


        #region Hash.ByteArray


        /// <summary>Computes and returns the hashRet (in form of byte array) of the specified kind of the specified byte array.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="bytesToHash">Byte array whose hashRet string is calculated.</param>
        /// <param name="HashAlgorithmType">Specifies the type of the hashing algorithm to be used.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by teh method. Its type must correspond the specified type.</param>
        public static byte[] GetHashBytes(byte[] bytesToHash, HashType hashType, HashAlgorithm cryptoAlgorithm = null)
        {
            if (cryptoAlgorithm == null)
                cryptoAlgorithm = GetHashAlgorithm(hashType);
            if (!IsCorrectHashAlgorithm(cryptoAlgorithm, hashType))
                throw new ArgumentException("The specified cryptographic hash algorithm does not correspond type specification: "
                    + HashTypeToString(hashType) + ".");
            if (cryptoAlgorithm == null)
                throw new InvalidOperationException("Can not get hashing algorithm for the following hash type: "
                    + hashType.ToString() + ".");
            if (bytesToHash == null)
                throw new ArgumentException("Array of bytes to be hashed is not specified (null arguments).");
            int length = bytesToHash.Length;
            //if (length < 1)
            //    throw new ArgumentException("Array of bytes to be hashed has zero length.");
            byte[] resultBytes = cryptoAlgorithm.ComputeHash(bytesToHash, 0, length);
            return resultBytes;
        }

        /// <summary>Computes and returns the hashRet string of the specified kind of the specified byte array.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="bytesToHash">Byte array whose hashRet string is calculated.</param>
        /// <param name="HashAlgorithmType">Specifies the type of the hashing algorithm to be used.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by teh method. Its type must correspond the specified type.</param>
        public static string GetHashHex(byte[] bytesToHash, HashType hashType, HashAlgorithm cryptoAlgorithm = null)
        {
            return Util.ToHexString(GetHashBytes(bytesToHash, hashType, cryptoAlgorithm));
        }


        /// <summary>Computes and returns the MD5 hashRet string of the specified array of bytes.</summary>
        /// <param name="bytesToHash">Byte array whose hashRet string is calculated.</param>
        public static string GetHashMd5Hex(byte[] bytesToHash)
        {
            return GetHashHex(bytesToHash, HashType.MD5);
        }

        /// <summary>Computes and returns the SHA1 hashRet string of the specified array of bytes.</summary>
        /// <param name="bytesToHash">Byte array whose hashRet string is calculated.</param>
        public static string GetHashSha1Hex(byte[] bytesToHash)
        {
            return GetHashHex(bytesToHash, HashType.SHA1);
        }

        /// <summary>Computes and returns the SHA256 hashRet string of the specified array of bytes.</summary>
        /// <param name="bytesToHash">Byte array whose hashRet string is calculated.</param>
        public static string GetHashSha256Hex(byte[] bytesToHash)
        {
            return GetHashHex(bytesToHash, HashType.SHA256);
        }

        /// <summary>Computes and returns the SHA512 hashRet string of the specified array of bytes.</summary>
        /// <param name="bytesToHash">Byte array whose hashRet string is calculated.</param>
        public static string GetHashSha512Hex(byte[] bytesToHash)
        {
            return GetHashHex(bytesToHash, HashType.SHA512);
        }


        #endregion Hash.ByteArray


        #region Hash.CeckByteArray

        /// <summary>Chechs the specified type of hashRet value of a byte array.
        /// <para>Returns true if the hashRet value matches the hashRet value of the byte array, and false otherwise.</para>
        /// </summary>
        /// <param name="bytesToCheck">String whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose correctness is checked.</param>
        /// <param name="HashAlgorithmType">Type of the hashRet value that is checked.</param>
        /// <returns>True if the specified hashRet <paramref name="HashValue"/> actually matches the hashRet value
        /// of the type <paramref name="HashAlgorithmType"/> of the verified string <paramref name="bytesToCheck"/>, or
        /// false otherwise.</returns>
        public static bool CheckHashHex(byte[] bytesToCheck, string hashValue, HashType hashAlgorithmType)
        {
            if (string.IsNullOrEmpty(hashValue))
                return false;
            hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetHashHex(bytesToCheck, hashAlgorithmType);
            return Util.AreHexStringsEqual(hashValue, actualHash);
        }


        /// <summary>Chechs all supported types of hashRet value of a string.
        /// <para>Returns the hashRet type if the hashRet value matches the hashRet value of that type of the specified string, 
        /// or <see cref="HashType.None"/> if the specified hashRet value doesn't match the hashRet value of any
        /// supported type of the string.</para>
        /// </summary>
        /// <param name="bytesToCheck">String whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose match with the specified string is checked.</param>
        /// <returns>Type of the hashRet value that matches the specified hashRet value of the specified string, 
        /// or <see cref="HashType.None"/> if the specified hashRet value doesn't match the hashRet value of any
        /// supported type of the specified string.</returns>
        public static HashType CheckHashSupportedTypesHex(byte[] bytesToCheck, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue))
                return HashType.None;
            hashValue = hashValue.Trim();
            var values = Enum.GetValues(typeof(HashType)).Cast<HashType>();
            foreach (HashType hashType in values)
            {
                string actualHahsh = GetHashHex(bytesToCheck, hashType);
                if (Util.AreHexStringsEqual(hashValue, actualHahsh))
                    return hashType;
            }
            return HashType.None;
        }


        #endregion Hash.CheckByteArray


        #region Hash.String


        /// <summary>Computes and returns the hashRet string of the specified kind of the specified string.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose hashRet string is calculated.</param>
        /// <param name="HashAlgorithmType">Specifies the type of the hashing algorithm to be used.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        public static byte[] GetStringHashBytes(string stringToHash, HashType hashType = HashType.Default, HashAlgorithm cryptoAlgorithm = null)
        {
            if (cryptoAlgorithm == null)
                cryptoAlgorithm = GetHashAlgorithm(hashType);
            if (!IsCorrectHashAlgorithm(cryptoAlgorithm, hashType))
                throw new ArgumentException("The specified cryptographic hash algorithm does not correspond type specification: "
                    + HashTypeToString(hashType) + ".");
            byte[] stringBytes = StringEncoding.GetBytes(stringToHash);
            int numStringBytes = StringEncoding.GetByteCount(stringToHash);
            byte[] resultBytes = cryptoAlgorithm.ComputeHash(stringBytes, 0, numStringBytes);
            return resultBytes;
        }

        /// <summary>Computes and returns the hashRet string of the specified kind of the specified string.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose hashRet string is calculated.</param>
        /// <param name="HashAlgorithmType">Specifies the type of the hashing algorithm to be used.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        public static string GetStringHashHex(string stringToHash, HashType hashType = HashType.Default, HashAlgorithm cryptoAlgorithm = null)
        {
            return Util.ToHexString(GetStringHashBytes(stringToHash, hashType, cryptoAlgorithm));
        }


        /// <summary>Computes and returns the MD5 hashRet string of the specified string.</summary>
        /// <param name="stringToHash">String whose cryptographic hashRet is calculated.</param>
        public static string GetStringHashMd5Hex(string stringToHash)
        {
            return GetStringHashHex(stringToHash, HashType.MD5);

            //HashAlgorithm crypt = new MD5CryptoServiceProvider();
            //byte[] crypto = crypt.ComputeHash(HashEncoding.GetBytes(stringToHash), 0, HashEncoding.GetByteCount(stringToHash));
            //StringBuilder sb = new StringBuilder();
            //foreach (byte bit in crypto)
            //{
            //    sb.Append(bit.ToString("x2"));
            //}
            //return sb.ToString();
        }


        /// <summary>Computes and returns the SHA1 hashRet string of the specified string.</summary>
        /// <param name="stringToHash">String whose cryptographic hashRet is calculated.</param>
        public static string GetStringHashSha1Hex(string stringToHash)
        {
            return GetStringHashHex(stringToHash, HashType.SHA1);

            //HashAlgorithm crypt = new SHA1Managed();
            //byte[] crypto = crypt.ComputeHash(HashEncoding.GetBytes(stringToHash), 0, HashEncoding.GetByteCount(stringToHash));
            //StringBuilder sb = new StringBuilder();
            //foreach (byte bit in crypto)
            //{
            //    sb.Append(bit.ToString("x2"));
            //}
            //return sb.ToString();
        }


        /// <summary>Computes and returns the SHA256 hashRet string of the specified string.</summary>
        /// <param name="stringToHash">String whose cryptographic hashRet is calculated.</param>
        public static string GetStringHashSha256Hex(string stringToHash)
        {
            return GetStringHashHex(stringToHash, HashType.SHA256);

            //HashAlgorithm crypt = new SHA256Managed();
            //byte[] crypto = crypt.ComputeHash(HashEncoding.GetBytes(stringToHash), 0, HashEncoding.GetByteCount(stringToHash));
            //StringBuilder sb = new StringBuilder();
            //foreach (byte bit in crypto)
            //{
            //    sb.Append(bit.ToString("x2"));
            //}
            //return sb.ToString();
        }


        /// <summary>Computes and returns the SHA512 hashRet string of the specified string.</summary>
        /// <param name="stringToHash">String whose cryptographic hashRet is calculated.</param>
        public static string GetStringHashSha512Hex(string stringToHash)
        {
            return GetStringHashHex(stringToHash, HashType.SHA512);

            //HashAlgorithm crypt = new SHA512Managed();
            //byte[] crypto = crypt.ComputeHash(HashEncoding.GetBytes(stringToHash), 0, HashEncoding.GetByteCount(stringToHash));
            //StringBuilder sb = new StringBuilder();
            //foreach (byte bit in crypto)
            //{
            //    sb.Append(bit.ToString("x2"));
            //}
            //return sb.ToString();
        }


        /// <summary>Computes and returns the salted hashRet (in form of byte array) of the specified kind of the specified string, with the specified
        /// number of repetitions of the salted hashing algoriithm.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose salted hashRet is to be computed.</param>
        /// <param name="HashAlgorithmType">Type of hashing algorithm used.</param>
        /// <param name="salt">Salt string.</param>
        /// <param name="numIterations">Optional number of iterations. If hreater than 0 then hashing algorithm is
        /// reapplied to the salted result of the previous computation for the specified number of time.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        /// <returns>The salted hashRet of the specified string, with specified hashRet, and specified number of 
        /// repetitions of the salted hashRet calculation.</returns>
        public static byte[] GetStringSaltedHashBytes(string stringToHash, HashType hashType,
            string salt, int numIterations = 0, HashAlgorithm cryptoAlgorithm = null)
        {
            if (numIterations < 0)
                throw new ArgumentException("Invalid number of iterations of salted hash function: " +
                    numIterations + ", should be greater or equal to 0.");
            if (cryptoAlgorithm == null)
                cryptoAlgorithm = GetHashAlgorithm(hashType);
            if (!IsCorrectHashAlgorithm(cryptoAlgorithm, hashType))
                throw new ArgumentException("The specified cryptographic hash algorithm does not correspond type specification: "
                    + HashTypeToString(hashType) + ".");
            byte[] stringBytes = StringEncoding.GetBytes(stringToHash);
            int numStringBytes = StringEncoding.GetByteCount(stringToHash);
            byte[] saltBytes = StringEncoding.GetBytes(salt);
            int numSaltBytes = StringEncoding.GetByteCount(salt);
            byte[] hashedBytes = new byte[numStringBytes + numSaltBytes];
            Array.Copy(stringBytes, 0, hashedBytes, 0, numStringBytes);
            Array.Copy(saltBytes, 0, hashedBytes, numStringBytes, numSaltBytes);
            byte[] resultBytes = cryptoAlgorithm.ComputeHash(hashedBytes, 0, numStringBytes + numSaltBytes);
            int numResultBytes = resultBytes.Length;

            //Console.WriteLine(Environment.NewLine + "GetStringSaltedHash: " + Environment.NewLine
            //    + "  String: " + stringToHash + Environment.NewLine
            //    + "  Salt:   " + salt + Environment.NewLine
            //    + "    It. 0: " + ByteArrayToString(resultBytes));

            if (numIterations > 0)
            {
                hashedBytes = new byte[numResultBytes + numSaltBytes];
                for (int i = 0; i < numIterations; ++i)
                {
                    Array.Copy(resultBytes, 0, hashedBytes, 0, numResultBytes);
                    Array.Copy(saltBytes, 0, hashedBytes, numResultBytes, numSaltBytes);
                    resultBytes = cryptoAlgorithm.ComputeHash(hashedBytes, 0, numResultBytes + numSaltBytes);

                    //Console.WriteLine("    It. " + (i+1) + ": " + ByteArrayToString(resultBytes));

                }
            }
            return resultBytes;
        }

        /// <summary>Computes and returns the salted hashRet of the specified kind of the specified string, with the specified
        /// number of repetitions of the salted hashing algoriithm.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose salted hashRet is to be computed.</param>
        /// <param name="HashAlgorithmType">Type of hashing algorithm used.</param>
        /// <param name="salt">Salt string.</param>
        /// <param name="numIterations">Optional number of iterations. If hreater than 0 then hashing algorithm is
        /// reapplied to the salted result of the previous computation for the specified number of time.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        /// <returns>The salted hashRet of the specified string, with specified hashRet, and specified number of 
        /// repetitions of the salted hashRet calculation.</returns>
        public static string GetStringSaltedHash(string stringToHash, HashType hashType,
            string salt, int numIterations = 0, HashAlgorithm cryptoAlgorithm = null)
        {
            return Util.ToHexString(GetStringSaltedHashBytes(stringToHash, hashType,
                salt, numIterations, cryptoAlgorithm));
        }


        /// <summary>Computes and returns the MD5 salted hashRet of the specified string, with the specified
        /// number of repetitions of the salted hashing algoriithm.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose salted hashRet is to be computed.</param>
        /// <param name="salt">Salt string.</param>
        /// <param name="numIterations">Optional number of iterations. If hreater than 0 then hashing algorithm is
        /// reapplied to the salted result of the previous computation for the specified number of time.</param>
        /// <returns>The salted hashRet of the specified string, with specified hashRet, and specified number of 
        /// repetitions of the salted hashRet calculation.</returns>
        public static string GetStringSaltedHashMd5Hex(string stringToHash,
            string salt, int numIterations = 0)
        {
            return GetStringSaltedHash(stringToHash, HashType.MD5 /* HashAlgorithmType */,
                salt, numIterations);
        }

        /// <summary>Computes and returns the SHA1 salted hashRet of the specified string, with the specified
        /// number of repetitions of the salted hashing algoriithm.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose salted hashRet is to be computed.</param>
        /// <param name="salt">Salt string.</param>
        /// <param name="numIterations">Optional number of iterations. If hreater than 0 then hashing algorithm is
        /// reapplied to the salted result of the previous computation for the specified number of time.</param>
        /// <returns>The salted hashRet of the specified string, with specified hashRet, and specified number of 
        /// repetitions of the salted hashRet calculation.</returns>
        public static string GetStringSaltedHashSha1Hex(string stringToHash,
            string salt, int numIterations = 0)
        {
            return GetStringSaltedHash(stringToHash, HashType.SHA1 /* HashAlgorithmType */,
                salt, numIterations);
        }

        /// <summary>Computes and returns the SHA-256 salted hashRet of the specified string, with the specified
        /// number of repetitions of the salted hashing algoriithm.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose salted hashRet is to be computed.</param>
        /// <param name="salt">Salt string.</param>
        /// <param name="numIterations">Optional number of iterations. If hreater than 0 then hashing algorithm is
        /// reapplied to the salted result of the previous computation for the specified number of time.</param>
        /// <returns>The salted hashRet of the specified string, with specified hashRet, and specified number of 
        /// repetitions of the salted hashRet calculation.</returns>
        public static string GetStringSaltedHashSha256Hex(string stringToHash,
            string salt, int numIterations = 0)
        {
            return GetStringSaltedHash(stringToHash, HashType.SHA256 /* HashAlgorithmType */,
                salt, numIterations);
        }

        /// <summary>Computes and returns the SHA-512 salted hashRet of the specified string, with the specified
        /// number of repetitions of the salted hashing algoriithm.
        /// <para>Exception (<see cref="InvalidOperationException"/>) is thrown if the specified hashRet type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose salted hashRet is to be computed.</param>
        /// <param name="salt">Salt string.</param>
        /// <param name="numIterations">Optional number of iterations. If hreater than 0 then hashing algorithm is
        /// reapplied to the salted result of the previous computation for the specified number of time.</param>
        /// <returns>The salted hashRet of the specified string, with specified hashRet, and specified number of 
        /// repetitions of the salted hashRet calculation.</returns>
        public static string GetStringSaltedHashSha512Hex(string stringToHash,
            string salt, int numIterations = 0)
        {
            return GetStringSaltedHash(stringToHash, HashType.SHA512 /* HashAlgorithmType */,
                salt, numIterations);
        }




        #endregion Hash.String


        #region Hash.CeckString

        /// <summary>Chechs the specified type of hashRet value of a string.
        /// <para>Returns true if the hashRet value matches the hashRet value of the string, and false otherwise.</para>
        /// </summary>
        /// <param name="bytesToCheck">String whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose correctness is checked.</param>
        /// <param name="HashAlgorithmType">Type of the hashRet value that is checked.</param>
        /// <returns>True if the specified hashRet <paramref name="HashValue"/> actually matches the hashRet value
        /// of the type <paramref name="HashAlgorithmType"/> of the verified string <paramref name="bytesToCheck"/>, or
        /// false otherwise.</returns>
        public static bool CheckStringHashHex(string stringToCheck, string hashValue, HashType hashAlgorithmType)
        {
            if (string.IsNullOrEmpty(hashValue))
                return false;
            hashValue = hashValue.Trim();  // .ToUpper();
            string actualHash = GetStringHashHex(stringToCheck, hashAlgorithmType);
            return Util.AreHexStringsEqual(hashValue, actualHash);
        }

        /// <summary>Chechs all supported types of hashRet value of a string.
        /// <para>Returns the hashRet type if the hashRet value matches the hashRet value of that type of the specified string, 
        /// or <see cref="HashType.None"/> if the specified hashRet value doesn't match the hashRet value of any
        /// supported type of the string.</para>
        /// </summary>
        /// <param name="bytesToCheck">String whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose match with the specified string is checked.</param>
        /// <returns>Type of the hashRet value that matches the specified hashRet value of the specified string, 
        /// or <see cref="HashType.None"/> if the specified hashRet value doesn't match the hashRet value of any
        /// supported type of the specified string.</returns>
        public static HashType CheckStringHashSupportedTypesHex(string stringToCheck, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue))
                return HashType.None;
            hashValue = hashValue.Trim();
            var values = Enum.GetValues(typeof(HashType)).Cast<HashType>();
            foreach (HashType hashType in values)
            {
                string actualHahsh = GetStringHashHex(stringToCheck, hashType);
                if (Util.AreHexStringsEqual(hashValue, actualHahsh))
                    return hashType;
            }
            return HashType.None;

            //if (string.IsNullOrEmpty(HashValue))
            //    return HashType.None;
            //var values = Enum.GetValues(typeof(HashType)).Cast<HashType>();
            //HashValue = HashValue.Trim();
            //int hashLength = HashValue.Length;
            //if (hashLength < 1)
            //    return HashType.None;
            //foreach (HashType hashType in values)
            //{
            //    int expectedLength = GetHashLengthHex(hashType);
            //    if (!(expectedLength > 0 && expectedLength != hashLength)) // don't check hashRet type if length does not match
            //    {
            //        if (CheckStringHashHex(stringToCheck, HashValue, hashType))
            //            return hashType;
            //    }
            //}
            //return HashType.None;
        }

        /// <summary>Checks whether the specified MD5 hashRet value matches the actual hashRet value
        /// of the specified string.
        /// <para>Returns true if the specified hashRet value matches the actual hashRet value of 
        /// the string, and false otherwise.</para></summary>
        /// <param name="bytesToCheck">String whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose validity is checked for the specified string.</param>
        /// <returns>True if the specified hashRet value matches the actual value, false otherwise.</returns>
        public static bool CheckStringHashMd5Hex(string stringToCheck, string hashValue)
        {
            return CheckStringHashHex(stringToCheck, hashValue, HashType.MD5);
        }

        /// <summary>Checks whether the specified SHA-1 hashRet value matches the actual hashRet value
        /// of the specified string.
        /// <para>Returns true if the specified hashRet value matches the actual hashRet value of 
        /// the string, and false otherwise.</para></summary>
        /// <param name="bytesToCheck">String whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose validity is checked for the specified string.</param>
        /// <returns>True if the specified hashRet value matches the actual value, false otherwise.</returns>
        public static bool CheckStringHashSha1Hex(string stringToCheck, string hashValue)
        {
            return CheckStringHashHex(stringToCheck, hashValue, HashType.SHA1);
        }

        /// <summary>Checks whether the specified SHA-256 hashRet value matches the actual hashRet value
        /// of the specified string.
        /// <para>Returns true if the specified hashRet value matches the actual hashRet value of 
        /// the string, and false otherwise.</para></summary>
        /// <param name="bytesToCheck">String whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose validity is checked for the specified string.</param>
        /// <returns>True if the specified hashRet value matches the actual value, false otherwise.</returns>
        public static bool CheckStringHashSha256Hex(string stringToCheck, string hashValue)
        {
            return CheckStringHashHex(stringToCheck, hashValue, HashType.SHA256);
        }

        /// <summary>Checks whether the specified SHA-512 hashRet value matches the actual hashRet value
        /// of the specified string.
        /// <para>Returns true if the specified hashRet value matches the actual hashRet value of 
        /// the string, and false otherwise.</para></summary>
        /// <param name="bytesToCheck">String whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose validity is checked for the specified string.</param>
        /// <returns>True if the specified hashRet value matches the actual value, false otherwise.</returns>
        public static bool CheckStringHashSha512Hex(string stringToCheck, string hashValue)
        {
            return CheckStringHashHex(stringToCheck, hashValue, HashType.SHA512);
        }


        #endregion Hash.CheckString


        #region Hash.File


        /// <summary>Computes and returns the hashRet (in form of byte array) of specified type of the specified stream.</summary>
        /// <param name="inputFilePath">Path to the file whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="HashAlgorithmType">Specifies the type of the hashing algorithm to be used.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by teh method. Its type must correspond the specified type.</param>
        public static byte[] GetFileHashBytes(string filePath, HashType hashType, HashAlgorithm cryptoAlgorithm = null)
        {
            if (!File.Exists(filePath))
                return null;
            byte[] hashBytes = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashBytes = GetHashBytes(stream, hashType, cryptoAlgorithm);
            }
            return hashBytes;
        }

        /// <summary>Computes and returns the hashRet string of specified type of the specified stream.</summary>
        /// <param name="inputFilePath">Path to the file whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="HashAlgorithmType">Specifies the type of the hashing algorithm to be used.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by teh method. Its type must correspond the specified type.</param>
        public static string GetFileHashHex(string filePath, HashType hashType, HashAlgorithm cryptoAlgorithm = null)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashHex(stream, hashType, cryptoAlgorithm);
            }
            return hashString;
        }

        /// <summary>Computes and returns the MD5 hashRet string of the specified stream.</summary>
        /// <param name="inputFilePath">Path to the file whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by teh method. Its type must correspond the specified type.</param>
        public static string GetFileHashMd5Hex(string filePath, HashAlgorithm cryptoAlgorithm = null)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashMd5Hex(stream, cryptoAlgorithm);
            }
            return hashString;
        }

        /// <summary>Computes and returns the SHA1 hashRet string of the specified stream.</summary>
        /// <param name="inputFilePath">Path to the file whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by teh method. Its type must correspond the specified type.</param>
        public static string GetFileHashSha1Hex(string filePath, HashAlgorithm cryptoAlgorithm = null)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashSha1Hex(stream, cryptoAlgorithm);
            }
            return hashString;
        }

        /// <summary>Computes and returns the SHA256 hashRet string of the specified stream.</summary>
        /// <param name="inputFilePath">Path to the file whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by teh method. Its type must correspond the specified type.</param>
        public static string GetFileHashSha256Hex(string filePath, HashAlgorithm cryptoAlgorithm = null)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashSha256Hex(stream, cryptoAlgorithm);
            }
            return hashString;
        }

        /// <summary>Computes and returns the SHA512 hashRet string of the specified stream.</summary>
        /// <param name="inputFilePath">Path to the file whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by teh method. Its type must correspond the specified type.</param>
        public static string GetFileHashSha512Hex(string filePath, HashAlgorithm cryptoAlgorithm = null)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashSha512Hex(stream, cryptoAlgorithm);
            }
            return hashString;
        }



        /// <summary>Computes and returns the hashRet (in form of byte array) of specified type of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="HashAlgorithmType">Type of the hashRet algorithm used.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        public static byte[] GetHashBytes(Stream stream, HashType hashType, HashAlgorithm cryptoAlgorithm = null)
        {
            if (stream == null)
                return null;
            if (cryptoAlgorithm == null)
                cryptoAlgorithm = GetHashAlgorithm(hashType);
            if (!IsCorrectHashAlgorithm(cryptoAlgorithm, hashType))
                throw new ArgumentException("The specified cryptographic hash algorithm does not correspond type specification: "
                    + HashTypeToString(hashType) + ".");
            byte[] hashBytes = cryptoAlgorithm.ComputeHash(stream);
            return hashBytes;

        }

        /// <summary>Computes and returns the hexadecimal hashRet string of specified type of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="HashAlgorithmType">Type of the hashRet algorithm used.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        public static string GetHashHex(Stream stream, HashType hashType, HashAlgorithm cryptoAlgorithm = null)
        {
            return Util.ToHexString(GetHashBytes(stream, hashType, cryptoAlgorithm));

        }


        /// <summary>Computes and returns the MD5 hashRet string of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        public static string GetHashMd5Hex(Stream stream, HashAlgorithm cryptoAlgorithm = null)
        {
            return GetHashHex(stream, HashType.MD5, cryptoAlgorithm);

            //if (stream == null)
            //    return null;
            //HashAlgorithm crypt = new MD5CryptoServiceProvider();
            //byte[] crypto = crypt.ComputeHash(stream);
            //StringBuilder sb = new StringBuilder();
            //foreach (byte bit in crypto)
            //{
            //    sb.Append(bit.ToString("x2"));
            //}
            //return sb.ToString();
        }


        /// <summary>Computes and returns the SHA1 hashRet string of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        public static string GetHashSha1Hex(Stream stream, HashAlgorithm cryptoAlgorithm = null)
        {
            return GetHashHex(stream, HashType.SHA1, cryptoAlgorithm);

            //if (stream == null)
            //    return null;
            //HashAlgorithm crypt = new SHA1Managed();
            //byte[] crypto = crypt.ComputeHash(stream);
            //StringBuilder sb = new StringBuilder();
            //foreach (byte bit in crypto)
            //{
            //    sb.Append(bit.ToString("x2"));
            //}
            //return sb.ToString();
        }

        /// <summary>Computes and returns the SHA256 hashRet string of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        public static string GetHashSha256Hex(Stream stream, HashAlgorithm cryptoAlgorithm = null)
        {
            return GetHashHex(stream, HashType.SHA256, cryptoAlgorithm);

            //if (stream == null)
            //    return null;
            //HashAlgorithm crypt = new SHA256Managed();
            //byte[] crypto = crypt.ComputeHash(stream);
            //StringBuilder sb = new StringBuilder();
            //foreach (byte bit in crypto)
            //{
            //    sb.Append(bit.ToString("x2"));
            //}
            //return sb.ToString();
        }


        /// <summary>Computes and returns the SHA512 hashRet string of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hashRet is calculated.</param>
        /// <param name="cryptoAlgorithm">Algorithm objedt used to perform the job. 
        /// If not specified then it is created by the method. Its type must correspond the specified type.</param>
        public static string GetHashSha512Hex(Stream stream, HashAlgorithm cryptoAlgorithm = null)
        {
            return GetHashHex(stream, HashType.SHA512, cryptoAlgorithm);

            //if (stream == null)
            //    return null;
            //HashAlgorithm crypt = new SHA512Managed();
            //byte[] crypto = crypt.ComputeHash(stream);
            //StringBuilder sb = new StringBuilder();
            //foreach (byte bit in crypto)
            //{
            //    sb.Append(bit.ToString("x2"));
            //}
            //return sb.ToString();
        }


        #endregion Hash.File


        #region Hash.CeckHashFile

        /// <summary>Chechs the specified type of hashRet value of a file.
        /// <para>Returns true if the specified hashRet value matches the hashRet value of the file, and false otherwise.</para>
        /// </summary>
        /// <param name="inputFilePath">Path to the file whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose correctness is checked.</param>
        /// <param name="HashAlgorithmType">Type of the hashRet value that is checked.</param>
        /// <returns>True if the specified hashRet <paramref name="HashValue"/> actually matches the hashRet value
        /// of the type <paramref name="HashAlgorithmType"/> of the verified file <paramref name="inputFilePath"/>, or
        /// false otherwise.</returns>
        public static bool CheckFileHashHex(string filePath, string hashValue, HashType hashType, HashAlgorithm cryptoAlgorithm = null)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(hashValue))
                return false;
            else
            {
                hashValue = hashValue.Trim();
                string actualHash = GetFileHashHex(filePath, hashType, cryptoAlgorithm);
                return Util.AreHexStringsEqual(hashValue, actualHash);
            }
        }


        /// <summary>Chechs all supported types of hashRet value of a file.
        /// <para>Returns the hashRet type if the hashRet value matches the hashRet value of that type of the specified file, 
        /// or <see cref="HashType.None"/> if the specified hashRet value doesn't match the hashRet value of any
        /// supported type of the specified file.</para>
        /// </summary>
        /// <param name="inputFilePath">Path to the file whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose match with the specified file is checked.</param>
        /// <returns>Type of the hashRet value that matches the specified hashRet value of the specified file, 
        /// or <see cref="HashType.None"/> if the specified hashRet value doesn't match the hashRet value of any
        /// supported type of the specified file.</returns>
        public static HashType CheckFileHashSupportedTypesHex(string filePath, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue))
                return HashType.None;
            hashValue = hashValue.Trim();
            var values = Enum.GetValues(typeof(HashType)).Cast<HashType>();
            foreach (HashType hashType in values)
            {
                string actualHahsh = GetFileHashHex(filePath, hashType);
                if (Util.AreHexStringsEqual(hashValue, actualHahsh))
                    return hashType;
            }
            return HashType.None;
        }

        /// <summary>Checks whether the specified MD5 hashRet value matches the actual hashRet value
        /// of the specified file.
        /// <para>Returns true if the specified hashRet value matches the actual hashRet value of 
        /// the file, and false otherwise.</para></summary>
        /// <param name="inputFilePath">Path to the file whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose validity is checked for the specified file.</param>
        /// <returns>True if the specified file hashRet value matches the actual value, false otherwise.</returns>
        public static bool CheckFileHashMd5Hex(string filePath, string hashValue)
        {
            return CheckFileHashHex(filePath, hashValue, HashType.MD5); 
        }

        /// <summary>Checks whether the specified SHA-1 hashRet value matches the actual hashRet value
        /// of the specified file.
        /// <para>Returns true if the specified hashRet value matches the actual hashRet value of 
        /// the file, and false otherwise.</para></summary>
        /// <param name="inputFilePath">Path to the file whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose validity is checked for the specified file.</param>
        /// <returns>True if the specified file hashRet value matches the actual value, false otherwise.</returns>
        public static bool CheckFileHashSha1Hex(string filePath, string hashValue)
        {
            return CheckFileHashHex(filePath, hashValue, HashType.SHA1); 
        }

        /// <summary>Checks whether the specified SHA-256 hashRet value matches the actual hashRet value
        /// of the specified file.
        /// <para>Returns true if the specified hashRet value matches the actual hashRet value of 
        /// the file, and false otherwise.</para></summary>
        /// <param name="inputFilePath">Path to the file whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose validity is checked for the specified file.</param>
        /// <returns>True if the specified file hashRet value matches the actual value, false otherwise.</returns>
        public static bool CheckFileHashSha256Hex(string filePath, string hashValue)
        {
            return CheckFileHashHex(filePath, hashValue, HashType.SHA256); 
        }

        /// <summary>Checks whether the specified SHA-512 hashRet value matches the actual hashRet value
        /// of the specified file.
        /// <para>Returns true if the specified hashRet value matches the actual hashRet value of 
        /// the file, and false otherwise.</para></summary>
        /// <param name="inputFilePath">Path to the file whose hashRet value is checked.</param>
        /// <param name="HashValue">Supposed hashRet value whose validity is checked for the specified file.</param>
        /// <returns>True if the specified file hashRet value matches the actual value, false otherwise.</returns>
        public static bool CheckFileHashSha512Hex(string filePath, string hashValue)
        {
            return CheckFileHashHex(filePath, hashValue, HashType.SHA512); 

        }


        #endregion Hash.CeckHashFile


        #endregion Hash



        #region SymmetricEncryption

        // WARNING: Problems with padding 
        // It is important how to close the CryptoStream and when to execute FlushFinalBlock() on it.
        // SymmetricAlgorithm can by itself correctly handling padding and truncates padding bytes when decrypting,
        // but only when this is done correctly. Specifically, Close() or FlushFinalBlock() cause finalization where 
        // the last (possibly incomplete) block is processed and padding is added in a proper way.

        // See: 
        // Symmetric Encryption - very good article:http://www.moi.vonos.net/programming/symmetric-encryption/ 
        // Short introduction to proper symmetric encryption: https://blogcybersapiens.wordpress.com/2014/07/01/symmetric-encryptiondecryption-in-c/ 
        // Symmetric algorithms, detailed technical explanations http://dotnetcodr.com/2013/11/04/symmetric-encryption-algorithms-in-net-cryptography-part-1/  
        // MSDN: Encrypting data: - FOR STRINGS written to STREAM https://msdn.microsoft.com/en-us/library/as0w18af(v=vs.110).aspx 
        // MSDN: SymmetricAlgorithmClass: https://msdn.microsoft.com/en-us/library/system.security.cryptography.symmetricalgorithm(v=vs.80).aspx
        // MSDN: Creating a Cryptographic Scheme: https://msdn.microsoft.com/en-us/library/vstudio/0cwc0x23(v=vs.100).aspx 
        // MSDN: Encrypt/decrypt using TrippleDes: https://msdn.microsoft.com/en-us/library/System.Security.Cryptography.TripleDESCryptoServiceProvider(v=vs.80).aspx
        // Encrypt and Decrypt a string: http://stackoverflow.com/questions/202011/encrypt-and-decrypt-a-string/10366194#10366194 
        // Is saving IV and salt witth encrypted data safe? http://stackoverflow.com/questions/13901529/symmetric-encryption-aes-is-saving-the-iv-and-salt-alongside-the-encrypted-da 
        // Padding article: http://www.di-mgt.com.au/cryptopad.html 
        // PaddingMode Enumeration - MSDN: https://msdn.microsoft.com/en-us/library/system.security.cryptography.paddingmode(v=vs.110).aspx
        // CipherMode enumeration (for mode of encryption algs), MSDN: https://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode%28v=vs.110%29.aspx
        // Creating crypto- STRONG passwords and initialization vectors with salt: https://msdn.microsoft.com/en-us/library/system.security.cryptography.rfc2898derivebytes%28v=vs.110%29.aspx
        // STRONG passwords - diff. between faster PasswordDeriveBytes and strong Rfc2898DeriveBytes: http://stackoverflow.com/questions/1356899/passwordderivebytes-vs-rfc2898derivebytes-obsolete-but-way-faster 

        // TODO: Check the tutorials below: 
        // Cryptography model, MSDN - check subtitles: https://msdn.microsoft.com/en-us/library/0ss79b2x%28v=vs.110%29.aspx 
        // 
        // C# AES 256 bits Encryption Library with Salt - STRONG passwords - CodeProject: http://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt 
        //     - ima veliko pomanjkljivosti ('salt' bi moral biti random itd.), res samo za orientacijo 
        // Simple encrypting and decrypting data in C# - CodeProject - STRONG (?) password using PasswordDeriveBytes: http://www.codeproject.com/Articles/5719/Simple-encrypting-and-decrypting-data-in-C 
        //     - zelo poenostavljen, tudi lahko služi le za osnovni koncept, kako se uporabi .NET orodja. 

        // EncryptStringToBytes / DecryptStringFromBytes,  MSDN:  https://msdn.microsoft.com/en-us/library/system.security.cryptography.rijndaelmanaged%28v=vs.110%29.aspx 


        #region SymmetricEncryption.Constants

        // Constants that determine behavior of some functions. These constants should be kept fixed over time.

        //private static const bool _useLasgestSymmetricEncryptionKeys = true;

        ///// <summary>If true then symmetric keys are always forced to use the largest possible keys.</summary>
        //public static bool UseLasgestSymmetricEncryptionKeys
        //{
        //    get { return _useLasgestSymmetricEncryptionKeys; }
        //}

        #endregion SymmetricEncryption.Constants



        #region SymmetricEncryption.General


        private static SymmetricAlgorithmType[] _symmetricTypes = null;

        /// <summary>Returns all symmetric encryption algorithm types contained in the enumeration <see cref="SymmetricAlgorithmType"/></summary>
        public static SymmetricAlgorithmType[] GetSymmetricAlgorithmTypes()
        {
            if (_symmetricTypes == null)
            {
                List<SymmetricAlgorithmType> allTypes = new List<SymmetricAlgorithmType>();
                lock (Util.LockGlobal)
                {
                    if (_symmetricTypes == null)
                    {
                        IEnumerable<SymmetricAlgorithmType> tmpTypes = Util.GetEnumValues<SymmetricAlgorithmType>();
                        foreach (SymmetricAlgorithmType specType in tmpTypes)
                            allTypes.Add(specType);
                        _symmetricTypes = allTypes.ToArray();
                    }
                }
            }
            return _symmetricTypes;
        }

        /// <summary>Returns the <see cref="SymmetricAlgorithmType"/> value corresponding to the specified string.
        /// <para>This function enables selection of algorithms with strings that can have arbitrary capitalization
        /// of their letters, and are thus friendly for user insertion.</para></summary>
        /// <param name="flagString">String that represents the symmetric encryption algorithm type.</param> 
        public static SymmetricAlgorithmType GetSymmetricAlgorithmType(string typeString)
        {
            try
            {
                SymmetricAlgorithmType type = (SymmetricAlgorithmType)Enum.Parse(typeof(SymmetricAlgorithmType), typeString);
                return type;
            }
            catch { }
            if (string.IsNullOrEmpty(typeString))
                return SymmetricAlgorithmType.Default;
            if (typeString != null)
                typeString = typeString.ToUpper();
            if (typeString == "NONE")
                return SymmetricAlgorithmType.None;
            if (typeString == "RIJNDAEL")
                return SymmetricAlgorithmType.Rijndael;
            if (typeString == "RD")
                return SymmetricAlgorithmType.Rijndael;
            if (typeString == "AES")
                return SymmetricAlgorithmType.AES;
            if (typeString == "TRIPLEDES")
                return SymmetricAlgorithmType.TripleDES;
            if (typeString == "TD")
                return SymmetricAlgorithmType.TripleDES;
            if (typeString == "DES")
                return SymmetricAlgorithmType.DES;
            if (typeString == "RC2")
                return SymmetricAlgorithmType.RC2;
            if (typeString == "DEFAULT")
                return SymmetricAlgorithmType.Default;
            throw new ArgumentException("Did not recognize the following symmetric algorithm type: " + typeString);
            // return SymmetricAlgorithmType.None;
        }

        /// <summary>Returns the symmetric encryption algorithm type, i.e. an enumeration of type <see cref="SymmetricAlgorithmType"/>,
        /// for the specified symmetric encryption algorithm object.</summary>
        /// <param name="algorithmObject">Symmetric algorithm object for which type enumeration is returned.</param>
        /// <returns>The type enumeration corresponding to the specified algorithm, or 0 (None) if the algorithm is null or not
        /// contained in the enumeration.</returns>
        public static SymmetricAlgorithmType GetSymmetricAlgorithmType(SymmetricAlgorithm algorithmObject)
        {
            if (algorithmObject == null)
                return SymmetricAlgorithmType.None;
            if (algorithmObject is System.Security.Cryptography.Rijndael)
                return SymmetricAlgorithmType.Rijndael;
            if (algorithmObject is System.Security.Cryptography.Aes)
                return SymmetricAlgorithmType.AES;
            if (algorithmObject is System.Security.Cryptography.TripleDES)
                return SymmetricAlgorithmType.TripleDES;
            if (algorithmObject is System.Security.Cryptography.DES)
                return SymmetricAlgorithmType.DES;
            if (algorithmObject is System.Security.Cryptography.RC2)
                return SymmetricAlgorithmType.RC2;
            return SymmetricAlgorithmType.None;
        }

        /// <summary>Returns standard string representation of the specified symmetric encryption algorithm type.</summary>
        /// <param name="flag">Symmetric encryption algorithm type whose string representation is returned.</param>
        public static string SymmetricAlgorithmTypeToString(SymmetricAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case SymmetricAlgorithmType.None:
                    return "None";
                case SymmetricAlgorithmType.Rijndael:
                    return "Rijndael";
                case SymmetricAlgorithmType.AES:
                    return "AES";
                case SymmetricAlgorithmType.TripleDES:
                    return "TripleDES";
                case SymmetricAlgorithmType.DES:
                    return "DES";
                case SymmetricAlgorithmType.RC2:
                    return "RC2";
                default:
                    return algorithmType.ToString();
            }
        }

        /// <summary>Returns the appropriate symmetric encryption algorithm according to the specified hashRet type, or null 
        /// if the type is not recognized or the method is not implemented for that type.</summary>
        /// <param name="flag">Specification of the symmetric encryption algorithm type.</param>
        public static SymmetricAlgorithm GetSymmetricEncryptionAlgorithm(SymmetricAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case SymmetricAlgorithmType.Rijndael:
                    return new System.Security.Cryptography.RijndaelManaged();
                case SymmetricAlgorithmType.AES:
                    return new System.Security.Cryptography.AesManaged();
                case SymmetricAlgorithmType.TripleDES:
                    return new System.Security.Cryptography.TripleDESCryptoServiceProvider();
                case SymmetricAlgorithmType.DES:
                    return new System.Security.Cryptography.DESCryptoServiceProvider();
                case SymmetricAlgorithmType.RC2:
                    return new System.Security.Cryptography.RC2CryptoServiceProvider();
                default:
                    return null;
            }
        }

        /// <summary>Returns true if the specified symmetric encryption algorithm corresponds the type specification, false otherwise.
        /// <para>If the specified algorithm is null or the specified type is unknown then false is returned.</para></summary>
        /// <param name="flag">Specification of the symmetric encryption algorithm type.</param>
        /// <param name="algorithm">Algorithm that is checked for type correctness.</param>
        public static bool IsCorrectSymmetricEncryptionAlgorithm(SymmetricAlgorithm algorithm, SymmetricAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case SymmetricAlgorithmType.None:
                    return true;  // type not specified, all types are valid.
                case SymmetricAlgorithmType.Rijndael:
                    return (algorithm as RijndaelManaged != null);
                case SymmetricAlgorithmType.TripleDES:
                    return (algorithm as TripleDESCryptoServiceProvider != null);
                case SymmetricAlgorithmType.AES:
                    return (algorithm as TripleDESCryptoServiceProvider != null);
                case SymmetricAlgorithmType.DES:
                    return (algorithm as DESCryptoServiceProvider != null);
                case SymmetricAlgorithmType.RC2:
                    return (algorithm as RC2CryptoServiceProvider != null);
                default:
                    return false;
            }
        }


        /// <summary>For the specified symmetric encryption algorithm, this function takes the specified byte array and checks if the 
        /// array is of valid size. If yes then the unmodified initialization array is returned, otherwise the modified array that fits the required
        /// block size is returned (i.e. has integer number of blocks).
        /// <para>Bytes that are missing to the last full block are filled cyclically with the original bytes from teh beginning.</para></summary>
        /// <param name="byteArray">Original array provided for the algorithm with specified block size.</param>
        /// <param name="blockSize">Block size of the algorithm for which array is repaired, in bytes.
        /// <para>Warning: Algorithms specify block size in bits, not in bytes.</para></param>
        /// <param name="truncateIfLarger">If true then the byte array is truncated to the block size if it is larger than it.
        /// <para>Warning: this can only be done with the key and initialization vector, but not with data!!</para></param>
        /// <returns>An array that is of valid size for the algorithm. 
        /// <para>If the original initialization vector size matches the required one, the vector is unmodified.</para>
        /// <para>If the original vector is not a multiple of block size, it is padded by its initial bytes until the desired length.</para>
        /// </returns>
        public static byte[] PadToAlgorithmBlocksizeCyclic(byte[] byteArray, int blockSize, bool truncateIfLarger = false)
        {
            if (byteArray == null)
                throw new ArgumentException("Array of bytes not specified for valid byte array padding (null reference).");
            int providedSize = byteArray.Length;
            if (providedSize < 1)
                throw new ArgumentException("Array of bytes not specified properly for byte array padding (size too  small).");
            if (truncateIfLarger && providedSize > blockSize)
            {
                // Truncate the array if it is largerr than block size and the corresponding flag is set:
                byte[] ret = new byte[blockSize];
                for (int i = 0; i < blockSize; ++i)
                    ret[i] = byteArray[i];
                return ret;
            }

            int numBlocks = providedSize / blockSize;
            int requiredSize = numBlocks * blockSize;
            if (requiredSize < providedSize)
                requiredSize += blockSize;
            if (providedSize == requiredSize)
                return byteArray;
            else
            {
                byte[] ret = new byte[requiredSize];
                for (int i = 0; i < requiredSize; ++i)
                    ret[i] = byteArray[i % providedSize];
                return ret;
            }
        }

        /// <summary>For the specified symmetric encryption algorithm, this function takes the specified byte array and checks if the 
        /// array is of valid size. If yes then the unmodified initialization array is returned, otherwise the modified array that fits the required
        /// block size is returned (i.e. has integer number of blocks).
        /// <para>Bytes that are missing to the last full block are filled cyclically with the original bytes from teh beginning.</para></summary>
        /// <param name="byteArray">Original array provided for the algorithm with specified block size.</param>
        /// <param name="blockSize">Block size of the algorithm for which array is repaired.</param>
        /// <param name="truncateIfLarger">If true then the byte array is truncated to the block size if it is larger than it.
        /// <para>Warning: this can only be done with the key and initialization vector, but not with data!!</para></param>
        /// <returns>An array that is of valid size for the algorithm. 
        /// <para>If the original initialization vector size matches the required one, the vector is unmodified.</para>
        /// <para>If the original vector is not a multiple of block size, it is padded by its initial bytes until the desired length.</para>
        /// </returns>
        public static byte[] PadToAlgorithmBlocksizeZero(byte[] byteArray, int blockSize, bool truncateIfLarger = false)
        {
            if (byteArray == null)
                throw new ArgumentException("Array of bytes not specified for byte array padding (null reference).");
            int providedSize = byteArray.Length;
            if (providedSize < 1)
                throw new ArgumentException("Array of bytes not specified properly for byte array padding extraction (size too  small).");
            if (truncateIfLarger && providedSize > blockSize)
            {
                // Truncate the array if it is largerr than block size and the corresponding flag is set:
                byte[] ret = new byte[blockSize];
                for (int i = 0; i < blockSize; ++i)
                    ret[i] = byteArray[i];
                return ret;
            }
            int numBlocks = providedSize / blockSize;
            int requiredSize = numBlocks * blockSize;
            if (requiredSize < providedSize)
                requiredSize += blockSize;
            if (providedSize == requiredSize)
                return byteArray;
            else
            {
                byte[] ret = new byte[requiredSize];
                for (int i = 0; i < providedSize; ++i)
                    ret[i] = byteArray[i];
                for (int i = providedSize; i < requiredSize; ++i)
                    ret[i] = 0;
                return ret;
            }
        }

        /// <summary>For the specified symmetric encryption algorithm, this function takes the specified byte array and checks if the 
        /// array is of valid size. If yes then the unmodified array is returned, otherwise the modified array that fits the required
        /// block size is returned. The array size must be mltiple of the block size.
        /// <para>Bytes that are missing to the last full block are filled cyclically with the original bytes from teh beginning.</para></summary>
        /// <param name="byteArray">Original byte array provided for the algorithm.</param>
        /// <param name="algorithm">Symmetric algorithm that is used for encryption/decryption. Object is used to query and 
        /// eventually set the block sizes.</param>
        /// <param name="truncateIfLarger">If true then the byte array is truncated to the block size if it is larger than it.
        /// <para>Warning: this can only be done with the key and initialization vector, but not with data!!</para></param>
        /// <returns>An array of bytes that is of valid size for the algorithm. 
        /// <para>If the original initialization vector size matches the required one, the array is unmodified.</para>
        /// <para>If the original array is not a multiple of block size, it is padded by its initial bytes until the desired length.</para>
        /// </returns>
        public static byte[] PadToSymmetricAlgorithmBlocksizeCyclic(byte[] byteArray, SymmetricAlgorithm algorithm,
            // bool useLargestBlock = false, 
            bool truncateIfLarger = false)
        {
            if (byteArray == null)
                throw new ArgumentException("Symmetric encryption initialization vector not specified for byte array padding (null reference).");
            int providedSize = byteArray.Length;
            if (providedSize < 1)
                throw new ArgumentException("Symmetric encryption initialization vector not specified properly for byte array padding (size too  small).");
            if (algorithm == null)
                throw new ArgumentException("Symmetric encryption algorithm not specified for byte array padding (null reference).");
            int blockSize = algorithm.BlockSize / 8;
            return PadToAlgorithmBlocksizeCyclic(byteArray, blockSize, truncateIfLarger);
        }

        /// <summary>For the specified symmetric encryption algorithm, this function takes the specified byte array and checks if the 
        /// array is of valid size. If yes then the unmodified array is returned, otherwise the modified array that fits the required
        /// block size is returned. The array size must be mltiple of the block size.
        /// <para>Bytes that are missing to the last full block are filled cyclically with the original bytes from teh beginning.</para></summary>
        /// <param name="byteArray">Original byte array provided for the algorithm.</param>
        /// <param name="algorithm">Symmetric algorithm that is used for encryption/decryption. Object is used to query and 
        /// eventually set the block sizes.</param>
        /// <param name="truncateIfLarger">If true then the byte array is truncated to the block size if it is larger than it.
        /// <para>Warning: this can only be done with the key and initialization vector, but not with data!!</para></param>
        /// <returns>An array of bytes that is of valid size for the algorithm. 
        /// <para>If the original initialization vector size matches the required one, the array is unmodified.</para>
        /// <para>If the original array is not a multiple of block size, it is padded by its initial bytes until the desired length.</para>
        /// </returns>
        public static byte[] PadToSymmetricAlgorithmBlocksizeZero(byte[] byteArray, SymmetricAlgorithm algorithm,
            // bool useLargestBlock = false, 
            bool truncateIfLarger = false)
        {
            if (byteArray == null)
                throw new ArgumentException("Symmetric encryption initialization vector not specified for byte array padding (null reference).");
            int providedSize = byteArray.Length;
            if (providedSize < 1)
                throw new ArgumentException("Symmetric encryption initialization vector not specified properly for byte array padding (size too  small).");
            if (algorithm == null)
                throw new ArgumentException("Symmetric encryption algorithm not specified for byte array padding (null reference).");
            int blockSize = algorithm.BlockSize / 8;
            return PadToAlgorithmBlocksizeZero(byteArray, blockSize, truncateIfLarger);
        }



        /// <summary>For the specified symmetric encryption algorithm, this function takes the specified key and checks if the 
        /// key is of valid size. If yes then the unmodified key is returned, otherwise the modified key that fits the required
        /// size is returned.</summary>
        /// <param name="key">Original key provided for the algorithm.</param>
        /// <param name="algorithm">Symmetric algorithm that is used for encryption/decryption. Object is used to query and 
        /// eventually set the key sizes.</param>
        /// <returns>A key that is of valid size for the algorithm. 
        /// <para>If the original key size matches the required one, the key is unmodified.</para>
        /// <para>If the original key is larger than the required one, the first bytes of the provided key are used in the returned key.</para>
        /// <para>If the provided key is smaller than the erquired key size then bytes in the returned key are cyclically repeated
        /// bytes from the original key.</para></returns>
        public static byte[] GetValidSymmetricAlgorithmKey(byte[] key, SymmetricAlgorithm algorithm)
        {
            if (key == null)
                throw new ArgumentException("Symmetric encryption key not specified for valid key extraction (null reference).");
            int providedSize = key.Length;
            if (providedSize < 1)
                throw new ArgumentException("Symmetric encryption key not specified properly for valid key extraction (size too  small).");
            if (algorithm == null)
                throw new ArgumentException("Symmetric encryption algorithm not specified for valid key extraction (null reference).");
            int requiredSize = algorithm.KeySize / 8;
            if (providedSize == requiredSize)
                return key;
            else
            {
                return PadToAlgorithmBlocksizeCyclic(key, requiredSize, true /* truncateIfLarger */);
            }
        }

        /// <summary>For the specified symmetric encryption algorithm, this function takes the specified initialization vector and checks if the 
        /// vector is of valid size. If yes then the unmodified IV is returned, otherwise the modified IV that fits the required
        /// size is returned.</summary>
        /// <param name="IV">Original initialization vector provided for the algorithm.</param>
        /// <param name="algorithm">Symmetric algorithm that is used for encryption/decryption. Object is used to query and 
        /// eventually set the initialization vector sizes (i.e., the block sizes).</param>
        /// <returns>An initialization vector that is of valid size for the algorithm (i.e. is of the same size as block size). 
        /// <para>If the original key size matches the required one, the IV is unmodified.</para>
        /// <para>If the original IV is larger than the required one, the first bytes of the provided IV are used in the returned IV.</para>
        /// <para>If the provided key is smaller than the erquired IV size (i.e. block size) then bytes in the returned IV are cyclically repeated
        /// bytes from the original IV.</para></returns>
        public static byte[] GetValidSymmetricAlgorithmIV(byte[] IV, SymmetricAlgorithm algorithm)
        {
            if (IV == null)
                throw new ArgumentException("Symmetric encryption initialization vector not specified for valid key extraction (null reference).");
            int providedSize = IV.Length;
            if (providedSize < 1)
                throw new ArgumentException("Symmetric encryption initialization vector not specified properly for valid key extraction (size too  small).");
            if (algorithm == null)
                throw new ArgumentException("Symmetric encryption algorithm not specified for valid key extraction (null reference).");
            int requiredSize = algorithm.BlockSize / 8;
            if (providedSize == requiredSize)
                return IV;
            else
            {
                return PadToAlgorithmBlocksizeCyclic(IV, requiredSize, true /* truncateIfLarger */);
            }
        }



        /// <summary>Returns the largest possible key siye, in bits, for the spcified symmetric encryption algorithm.</summary>
        /// <param name="algorithm">Algorithm whose largest possible key size is returned.</param>
        public static int GetLargestSymmetricAlgorithmKeySize(SymmetricAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentException("Symmetric encryption algorithm not specified for getting the largest key size (null reference).");
            KeySizes[] keySizes = algorithm.LegalKeySizes;
            int maxSize = 0;
            foreach (KeySizes keySize in keySizes)
            {
                int size = keySize.MaxSize;
                if (size > maxSize)
                    maxSize = size;
            }
            return maxSize;
        }

        /// <summary>On the specified symmetric encryption algorythm, sets the largest possible key size, in bits, for the spcified symmetric encryption algorithm.</summary>
        /// <param name="algorithm">Algorithm whose largest possible key size is returned.</param>
        /// <returns>Actual key size on the algorithm after the operation (should be the largest possible key).</returns>
        private static int SetLargestSymmetricAlgorithmKeySize(SymmetricAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentException("Symmetric encryption algorithm not specified for setting the largest key size (null reference).");
            int currentSize = algorithm.KeySize;
            int largestSize = GetLargestSymmetricAlgorithmKeySize(algorithm);
            if (currentSize != largestSize)
                algorithm.KeySize = largestSize;
            return algorithm.KeySize;
        }

        /// <summary>Returns the largest possible block siye, in bits, for the spcified symmetric encryption algorithm.</summary>
        /// <param name="algorithm">Algorithm whose largest possible block size is returned.</param>
        public static int GetLargestSymmetricAlgorithmBlockSize(SymmetricAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentException("Symmetric encryption algorithm not specified for getting the largest block size (null reference).");
            KeySizes[] blockSizes = algorithm.LegalBlockSizes;
            int maxSize = 0;
            foreach (KeySizes blockSize in blockSizes)
            {
                int size = blockSize.MaxSize;
                if (size > maxSize)
                    maxSize = size;
            }
            return maxSize;
        }

        /// <summary>On the specified symmetric encryption algorithm, sets the largest possible block size, in bits, for the 
        /// spcified symmetric encryption algorithm.</summary>
        /// <param name="algorithm">Algorithm whose largest possible block size is set.</param>
        /// <returns>Actual block size on the algorithm after the operation (should be the largest possible block size).</returns>
        private static int SetLargestSymmetricAlgorithmBlockSize(SymmetricAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentException("Symmetric encryption algorithm not specified for setting the largest block size (null reference).");
            int currentSize = algorithm.BlockSize;
            int largestSize = GetLargestSymmetricAlgorithmBlockSize(algorithm);
            if (currentSize != largestSize)
                algorithm.BlockSize = largestSize;
            return algorithm.BlockSize;
        }


        #endregion SymmetricEncryption.General



        #region SymmetricEncryption.Iglib



        #endregion SymmetricEncryption.Iglib



        #region SymmetricEncryption.Basic




        /// <summary>Prepares the symmetric algorithm, initialization vector, and key before encryption or decryption takes place.
        /// <para>Creates the algorithm if necessary, changes algorithm parameters (such as key size) when applicable,
        /// and checks and corrects the key and initialization vector, if necessary.</para></summary>
        /// <param name="key">Key used. Method checks that the key is of correct size and corrects it when necessary.</param>
        /// <param name="IV">Initialization vector used. Method checks that it is consistent with the algorithm's block size 
        /// and corrects it if it is not.</param>
        /// <param name="flag">Algorithm type. If the algorithm is not specified then it is created anew according 
        /// to this parameter.</param>
        /// <param name="algorithm">Algorithm used for encryption. If null it is created. If specified then it
        /// is checked that it corresponds to the type specified by <typeparamref name="flag"/>.</param>
        /// <param name="useLargestKey">If true then algorithm parameters are set (if necessary) in such a way that the 
        /// largest hey length supported by the algorithm is used.</param>
        /// <param name="useLargestBlock">If true then algorithm parameters are set (if necessary) in such a way that the 
        /// largest hey length supported by the algorithm is used.</param>
        public static void PrepareSymmetricAllgorithmBasic(byte[] passwordBytes, byte[] passwordSalt,
            ref byte[] key, ref byte[] IV, SymmetricAlgorithmType algorithmType,
            ref SymmetricAlgorithm algorithm, PasswordAlgorithmType pwdType = PasswordAlgorithmType.Default,
                int pwdNumIterations = 1000, bool useLargestKey = false, bool useLargestBlock = false)
        {
            if (passwordBytes == null)
                throw  new ArgumentNullException("Password not specified (null reference).");
            if (passwordBytes.Length < ConstCrypto.MinPasswordLength)
                throw new ArgumentException("Password length is " + passwordBytes.Length + " bytes,  shoulld be at least " 
                    + ConstCrypto.MinPasswordLength + ".");
            if (passwordSalt == null)
                throw new ArgumentNullException("Password salt is not specified (null reference).");
            if (passwordSalt.Length < ConstCrypto.MinPasswordSaltLength)
                throw new ArgumentException("Password salt length is " + passwordSalt.Length + " bytes,  shoulld be at least " 
                    + ConstCrypto.MinPasswordSaltLength + ".");
            if (pwdType == PasswordAlgorithmType.None)
                throw new ArgumentException("Password algorithm type " + UtilCrypto.PasswordAlgorithmTypeToString(pwdType) + " is not allowed in this context.");

            // Create a symmetric algorithm object if necessary (such that key and block size kan be obtained):
            if (algorithm == null)
            {
                algorithm = GetSymmetricEncryptionAlgorithm(algorithmType);
            }
            if (pwdType != PasswordAlgorithmType.None)
            {
                // Generate the key and initialization vector from password and salt:
                PasswordAlgorithmBase pwdAlg = UtilCrypto.GetPasswordAlgorithm(pwdType);
                pwdAlg.Init(passwordBytes, passwordSalt, pwdNumIterations);
                pwdAlg.GetBytes(algorithm.KeySize, ref key);
                pwdAlg.GetBytes(algorithm.BlockSize, ref IV);
            }
            // Call the basic method for encryption parameters preparation (such as selection of algorithm, padding, etc.):
            PrepareSymmetricAllgorithmPlain(ref key, ref IV, algorithmType, ref algorithm, useLargestKey, useLargestBlock);
        }



        /// <summary>Encrypts the specified byte array by a symmetric encryption algorithm and returns encrypted bytes.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="data">Data to be encrypted.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="SaltBytes">Salt that is prepended the data before encryption.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static byte[] EncryptBasic(byte[] data, byte[] passwordBytes, byte[] passwordSalt, 
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None, SymmetricAlgorithm algorithm = null, 
            byte[] saltBytes = null, PasswordAlgorithmType pwdType = PasswordAlgorithmType.Default, 
            int pwdNumIterations = 1000, bool useLargestKey = false, bool useLargestBlock = false)
        {
                        
            if (saltBytes == null)
                throw new ArgumentNullException("Salt is not specified (null reference).");
            if (saltBytes.Length < ConstCrypto.MinPasswordSaltLength)
                throw new ArgumentException("Password salt length is " + saltBytes.Length + " bytes,  shoulld be at least "
                    + ConstCrypto.MinPasswordSaltLength + ".");
            byte[] key = null, IV = null;
            PrepareSymmetricAllgorithmBasic(passwordBytes, passwordSalt, ref key, ref IV, algorithmType,
                ref algorithm, pwdType, pwdNumIterations, useLargestKey, useLargestBlock);
            return EncryptPlain(data, key, IV, algorithmType, algorithm, saltBytes, useLargestKey, useLargestBlock);
        }

        /// <summary>Decrypts the specified byte array by a symmetric algorithm and returns decrypted data.
        /// <para>If the algorithm is not specified then it is provided by the system, but in this case both the 
        /// key and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="data">Data to be decrypted.</param>
        /// <param name="key">Secret key used to decrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="numSaltBytes">Length of salt, which must be accounted for when decrypting.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Decrypted data as byte array.</returns>
        public static byte[] DecryptBasic(byte[] data, byte[] passwordBytes, byte[] passwordSalt,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None,
            SymmetricAlgorithm algorithm = null, int numSaltBytes = 0, PasswordAlgorithmType pwdType = PasswordAlgorithmType.Default, 
            int pwdNumIterations = 1000, bool useLargestKey = false, bool useLargestBlock = false)
        {
            byte[] key = null, IV = null;
            PrepareSymmetricAllgorithmBasic(passwordBytes, passwordSalt, ref key, ref IV, algorithmType,
                ref algorithm, pwdType, pwdNumIterations, useLargestKey, useLargestBlock);

            return DecryptPlain(data, key, IV, algorithmType, algorithm, numSaltBytes, useLargestKey, useLargestBlock);
        }



        /// <summary>Encrypts the specified string, using the specified symmetric algorithm, key, and initialization vector,
        /// and returns the ToBase64 string representation of the encrypted bytes.</summary>
        /// <param name="stringToEncrypt">String that is encrypted.</param>
        /// <param name="key">Secret key used in encryption.</param>
        /// <param name="initializationVector">Initialization vector used in encryption. Necessary for stronger encryption 
        /// (othervise repeating input blocks would cause repeating output blocks).</param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Agorithm used for encryption. Must be consistend with <param name="flag", or null.</param>
        /// <param name="SaltBytes">Salt that is prepended the data before encryption.</param>
        /// <param name="useLargestKey">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible key for this kind of algorithm.</param>
        /// <param name="useLargestBlock">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible block for this kind of algorithm.</param>
        /// <returns>Base64 string representation of the encrypted string bytes.</returns>
        public static string EncryptStringBasic(string stringToEncrypt, byte[] passwordBytes, byte[] passwordSalt,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.Default, SymmetricAlgorithm algorithm = null,
            byte[] saltBytes = null, PasswordAlgorithmType pwdType = PasswordAlgorithmType.Default, int pwdNumIterations = 1000,
            bool useLargestKey = false, bool useLargestBlock = false)
        {
            if (saltBytes == null)
                throw new ArgumentNullException("Salt is not specified (null reference).");
            if (saltBytes.Length < ConstCrypto.MinPasswordSaltLength)
                throw new ArgumentException("Password salt length is " + saltBytes.Length + " bytes,  shoulld be at least "
                    + ConstCrypto.MinPasswordSaltLength + ".");
            byte[] key = null, IV = null;
            PrepareSymmetricAllgorithmBasic(passwordBytes, passwordSalt, ref key, ref IV, algorithmType,
                ref algorithm, pwdType, pwdNumIterations, useLargestKey, useLargestBlock);
            return EncryptStringPlain(stringToEncrypt, key, IV, algorithmType, algorithm,
                saltBytes, useLargestKey, useLargestBlock);
        }


        /// <summary>Derypts the specified string from encryted byte array, using the specified symmetric algorithm, key, and initialization vector.</summary>
        /// <param name="cipherText">Base64 string representation of the byte array containing the encrypted original string.</param>
        /// <param name="key">Secret key used in encryption.</param>
        /// <param name="initializationVector">Initialization vector used in encryption. Necessary for stronger encryption 
        /// (othervise repeating input blocks would cause repeating output blocks).</param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Agorithm used for encryption. Must be consistend with <param name="flag", or null.</param>
        /// <param name="numSaltBytes">Length of salt, which must be accounted for when decrypting.</param>
        /// <param name="useLargestKey">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible key for this kind of algorithm.</param>
        /// <param name="useLargestBlock">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible block for this kind of algorithm.</param>
        /// <returns>Original string, provided that input arguments are correct.</returns>
        public static string DecryptStringBasic(string cipherText, byte[] passwordBytes, byte[] passwordSalt,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.Default, SymmetricAlgorithm algorithm = null,
            int numSaltBytes = 0, PasswordAlgorithmType pwdType = PasswordAlgorithmType.Default, int pwdNumIterations = 1000, 
            bool useLargestKey = false, bool useLargestBlock = false)
        {
            byte[] key = null, IV = null;
            PrepareSymmetricAllgorithmBasic(passwordBytes, passwordSalt, ref key, ref IV, algorithmType,
                ref algorithm, pwdType, pwdNumIterations, useLargestKey, useLargestBlock);
            return DecryptStringPlain(cipherText, key, IV,
                algorithmType, algorithm, numSaltBytes, useLargestKey, useLargestBlock);

        }






        /// <summary>Encrypts the data from the specified input file by a symmetric encryption algorithm and writes encrypted data to
        /// the specified output file.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="inputFilePath">Path to the input file from which data is encrypted. The file must be exist and  be readable.</param>
        /// <param name="deletedFilePath">Path to the output file to which encrypted data is written. The file must be writable and will be
        /// overwritten if it already exists.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="bufferSize">Size of the intermediate buffer through which we read from input and write to output stream.
        /// Buffer makes possible to work on very large streams efficiently.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static void EncryptFileBasic(string inputFilePath, string outputFilePath, byte[] passwordBytes, byte[] passwordSalt,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.Default, SymmetricAlgorithm algorithm = null,
            byte[] saltBytes = null, int bufferSize = ConstCrypto.FileBufferSize, 
            PasswordAlgorithmType pwdType = PasswordAlgorithmType.Default, int pwdNumIterations = 1000,
            bool useLargestKey = false, bool useLargestBlock = false)
        {
            if (saltBytes == null)
                throw new ArgumentNullException("Salt is not specified (null reference).");
            if (saltBytes.Length < ConstCrypto.MinPasswordSaltLength)
                throw new ArgumentException("Password salt length is " + saltBytes.Length + " bytes,  shoulld be at least "
                    + ConstCrypto.MinPasswordSaltLength + ".");
            byte[] key = null, IV = null;
            PrepareSymmetricAllgorithmBasic(passwordBytes, passwordSalt, ref key, ref IV, algorithmType,
                ref algorithm, pwdType, pwdNumIterations, useLargestKey, useLargestBlock);
            EncryptFilePlain(inputFilePath, outputFilePath, key, IV, algorithmType, algorithm,
                saltBytes, bufferSize, useLargestKey, useLargestBlock);
        }

        /// <summary>Decrypts the data from the specified input file by a symmetric encryption algorithm and writes decrypted data to
        /// the specified output file.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="inputFilePath">Path to the input file from which data is encrypted. The file must be exist and  be readable.</param>
        /// <param name="deletedFilePath">Path to the output file to which encrypted data is written. The file must be writable and will be
        /// overwritten if it already exists.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="bufferSize">Size of the intermediate buffer through which we read from input and write to output stream.
        /// Buffer makes possible to work on very large streams efficiently.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static void DecryptFileBasic(string inputFilePath, string outputFilePath, byte[] passwordBytes, byte[] passwordSalt,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.Default, SymmetricAlgorithm algorithm = null,
            int numSaltBytes = 0, int bufferSize = ConstCrypto.FileBufferSize, 
            PasswordAlgorithmType pwdType = PasswordAlgorithmType.Default, int pwdNumIterations = 1000, 
            bool useLargestKey = false, bool useLargestBlock = false)
        {
            byte[] key = null, IV = null;
            PrepareSymmetricAllgorithmBasic(passwordBytes, passwordSalt, ref key, ref IV, algorithmType,
                ref algorithm, pwdType, pwdNumIterations, useLargestKey, useLargestBlock);
            
            DecryptFilePlain(inputFilePath, outputFilePath, key, IV, algorithmType, algorithm,
                numSaltBytes, bufferSize, useLargestKey, useLargestBlock);
        }








        #endregion SymmetricEncryption.Basic




        #region SymmetricEncryption.Plain


        /// <summary>Prepares the symmetric algorithm, initialization vector, and key before encryption or decryption takes place.
        /// <para>Creates the algorithm if necessary, changes algorithm parameters (such as key size) when applicable,
        /// and checks and corrects the key and initialization vector, if necessary.</para></summary>
        /// <param name="key">Key used. Method checks that the key is of correct size and corrects it when necessary.</param>
        /// <param name="IV">Initialization vector used. Method checks that it is consistent with the algorithm's block size 
        /// and corrects it if it is not.</param>
        /// <param name="flag">Algorithm type. If the algorithm is not specified then it is created anew according 
        /// to this parameter.</param>
        /// <param name="algorithm">Algorithm used for encryption. If null it is created. If specified then it
        /// is checked that it corresponds to the type specified by <typeparamref name="flag"/>.</param>
        /// <param name="useLargestKey">If true then algorithm parameters are set (if necessary) in such a way that the 
        /// largest hey length supported by the algorithm is used.</param>
        /// <param name="useLargestBlock">If true then algorithm parameters are set (if necessary) in such a way that the 
        /// largest hey length supported by the algorithm is used.</param>
        public static void PrepareSymmetricAllgorithmPlain(ref byte[] key, ref byte[] IV, SymmetricAlgorithmType algorithmType,
            ref SymmetricAlgorithm algorithm, bool useLargestKey = false, bool useLargestBlock = false)
        {

            if (algorithm == null)
            {
                if (key == null)
                    throw new ArgumentException("Key for symmetric encryption algorithm not specified (null reference) while algorithm object is also not specified to generate and store the key.");
                if (IV == null)
                    throw new ArgumentException("Initialization vector for symmetric encryption algorithm not specified (null reference) while algorithm object is also not specified to generate and store the key.");
                algorithm = GetSymmetricEncryptionAlgorithm(algorithmType);
                if (algorithm == null)
                    throw new ArgumentException("Symmetric encryption algorithm could not be created with type: "
                        + SymmetricAlgorithmTypeToString(algorithmType) + ".");
            }
            else
            {
                if (!IsCorrectSymmetricEncryptionAlgorithm(algorithm, algorithmType))
                    throw new ArgumentException("The provided symmetric encryption algorithm is of wrong type "
                        + GetSymmetricAlgorithmType(algorithm) + ", should be of type " + SymmetricAlgorithmTypeToString(algorithmType) + ".");
                if (key == null && algorithm.Key == null)
                    throw new ArgumentException("The key is not secified and algorithm object also does not contain it.");
                if (IV == null && algorithm.IV == null)
                    throw new ArgumentException("The initialization vector is not secified and algorithm object also does not contain it.");
            }
            int originalAlgorithmKeySize = algorithm.KeySize;
            int originalKeysize = key.Length * 8;
            if (useLargestKey)
                SetLargestSymmetricAlgorithmKeySize(algorithm);
            if (useLargestBlock)
                SetLargestSymmetricAlgorithmBlockSize(algorithm);

            if (key == null)
            {
                //algorithm.GenerateKey();
                //key = algorithm.Key;
                key = algorithm.Key;
            }
            else
            {
                key = GetValidSymmetricAlgorithmKey(key, algorithm);
                algorithm.Key = key;
            }
            if (IV == null)
            {
                //algorithm.GenerateIV();
                //IV = algorithm.IV;
                IV = algorithm.IV;
            }
            else
            {
                IV = GetValidSymmetricAlgorithmIV(IV, algorithm);
                algorithm.IV = IV;
            }

            //if (OutputLevel >= 2)
            //{
            //    int maximalAlgoritmKeysize = GetLargestSymmetricAlgorithmKeySize(algorithm);
            //    Console.WriteLine(Environment.NewLine + "    Alg. " + SymmetricAlgorithmTypeToString(flag)
            //        + ":  key length = " + algorithm.KeySize + " b; " + ((double)algorithm.KeySize / 8.0)
            //        + ", actual: " + key.Length
            //        + ", original alg. key size in b: " + originalAlgorithmKeySize + ", key: " + originalKeysize + ", max.: " + maximalAlgoritmKeysize);
            //}
        }



        /// <summary>Encrypts the specified byte array by a symmetric encryption algorithm and writes encrypted data to
        /// the specified output stream that must be open for writing.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="data">Data to be encrypted.</param>
        /// <param name="outputStream">Stream to which decrypted data is written. The stream must be open for writing and should
        /// be closed by the caller of this method.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="SaltBytes">Salt that is prepended the data before encryption.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static void EncryptPlain(byte[] data, Stream outputStream, byte[] key, byte[] IV,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None, SymmetricAlgorithm algorithm = null,
            byte[] saltBytes = null, bool useLargestKey = false, bool useLargestBlock = false)
        {
            // Repair eventual mismatches in algorithm parameters, take into account function argumennts:
            UtilCrypto.PrepareSymmetricAllgorithmPlain(ref key, ref IV, algorithmType, ref algorithm, useLargestKey, useLargestBlock);
            // Create a CryptoStream through which we will write unencrypted data (output, e.g. encrypted bytes,
            // will be written to the underlying outputStream):
            using (CryptoStream cryptoStream = new CryptoStream(outputStream,
               algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {


                if (saltBytes != null) if (saltBytes.Length > 0)
                    {
                        // Write the salt on cryptoStream:
                        cryptoStream.Write(saltBytes, 0, saltBytes.Length);
                    }

                // Write the data and make it do the encryption 
                cryptoStream.Write(data, 0, data.Length);

                // This will tell it that writing is finished and as there is no more data coming in, 
                // we can finalize the encryption (e.g. apply the padding as necessary, etc.) 
                cryptoStream.FlushFinalBlock();

                // cryptoStream.Close();  // stream will be closed by using block
            }

        }

        /// <summary>Decrypts the specified byte array by a symmetric algorithm and writes decrypted data to the specified
        /// output stream.
        /// <para>If the algorithm is not specified then it is provided by the system, but in this case both the 
        /// key and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="data">Data to be decrypted.</param>
        /// <param name="outputStream">Stream to which decrypted data is written. The stream must be open for writing and should
        /// be closed by the caller of this method.</param>
        /// <param name="key">Secret key used to decrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="numSaltBytes">Length of salt, which must be accounted for when decrypting.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Decrypted data as byte array.</returns>
        public static void DecryptPlain(byte[] data, Stream outputStream, byte[] key, byte[] IV,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None, SymmetricAlgorithm algorithm = null,
            int numSaltBytes = 0, bool useLargestKey = false, bool useLargestBlock = false)
        {
            // Repair eventual mismatches in algorithm parameters, take into account function argumennts:
            UtilCrypto.PrepareSymmetricAllgorithmPlain(ref key, ref IV, algorithmType, ref algorithm, false, false);


            if (numSaltBytes <= 0)
            {
                if (numSaltBytes < 0)
                    throw new ArgumentException("Salt length can not be less than 0.");
                // Create a CryptoStream on which we write unencrypted data; output will be written in the MemoryStream:
                using (CryptoStream cryptoStream = new CryptoStream(outputStream,
                    algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                {

                    // Write the data to be decrtpted:
                    cryptoStream.Write(data, 0, data.Length);

                    // This will cause finalization, including removing any padding when necessary (padded
                    // bytes are automatically truncated, as information on how many bytes were padded can be
                    // unambiguously derived from the last bytes):
                    cryptoStream.FlushFinalBlock();  // close() would have similar effect

                    // cryptoStream.Close(); // closed automatically by using block
                }
            }
            else
            {
                // We must discard the bytes that belong to salt. For this, we will first decrypt complete data with salt
                // to a temporary memory stream, and then transcribe only data without salt to the output stream:
                int bufferSize = 1024;
                using (MemoryStream tempStream = new MemoryStream())
                {

                    // Create a CryptoStream through which we will write decrypted data (output, e.g. encrypted bytes,
                    // will be written to the underlying outputStream):
                    using (CryptoStream cryptoStream = new CryptoStream(tempStream,
                       algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);

                        // This will tell it that writing is finished and as there is no more data coming in, 
                        // we can finalize the encryption (e.g. apply the padding as necessary, etc.) 
                        cryptoStream.FlushFinalBlock();

                        // Now, from the decrypted file stream, read everything except the salt and write it on the output stream:
                        tempStream.Flush();
                        tempStream.Seek(numSaltBytes, SeekOrigin.Begin);

                        byte[] buffer = new byte[bufferSize];
                        int numRead;
                        do
                        {
                            // Read the  next block of data from the input stream: 
                            numRead = tempStream.Read(buffer, 0, bufferSize);

                            // Encrypt data onto the output stream: 
                            outputStream.Write(buffer, 0, numRead);

                        } while (numRead != 0);

                    }
                }

            }
        }


        /// <summary>Encrypts the specified byte array by a symmetric encryption algorithm and returns encrypted bytes.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="data">Data to be encrypted.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="SaltBytes">Salt that is prepended the data before encryption.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static byte[] EncryptPlain(byte[] data, byte[] key, byte[] IV, SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None,
            SymmetricAlgorithm algorithm = null, byte[] saltBytes = null, bool useLargestKey = false, bool useLargestBlock = false)
        {

            //// Repair eventual mismatches in algorithm parameters, take into account function argumennts:
            //UtilCrypto.PrepareSymmetricAllgorithm(ref key, ref IV, flag, ref algorithm, useLargestKey, useLargestBlock);

            byte[] encryptedData = null;
            // Create a MemoryStream on which bytes to be encrypted are written:
            using (MemoryStream memStream = new MemoryStream())
            {

                EncryptPlain(data, memStream, key, IV, algorithmType, algorithm, saltBytes, useLargestKey, useLargestBlock);

                // Get the encrypted data from the MemoryStream:
                encryptedData = memStream.ToArray();

            }
            return encryptedData;
        }

        /// <summary>Decrypts the specified byte array by a symmetric algorithm and returns decrypted data.
        /// <para>If the algorithm is not specified then it is provided by the system, but in this case both the 
        /// key and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="data">Data to be decrypted.</param>
        /// <param name="key">Secret key used to decrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="numSaltBytes">Length of salt, which must be accounted for when decrypting.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Decrypted data as byte array.</returns>
        public static byte[] DecryptPlain(byte[] data, byte[] key, byte[] IV, SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None,
            SymmetricAlgorithm algorithm = null, int numSaltBytes = 0, bool useLargestKey = false, bool useLargestBlock = false)
        {

            byte[] decryptedData = null;
            // Create a MemoryStream that is going to accept the decrypted bytes 
            using (MemoryStream memStream = new MemoryStream())
            {

                DecryptPlain(data, memStream, key, IV, algorithmType, algorithm,
                    numSaltBytes, useLargestKey, useLargestBlock);

                // Get the decrypted data from the MemoryStream beefore it is destroyed:
                // Warning: using GetBuffer() here would not be the right way. 
                decryptedData = memStream.ToArray();
            }
            return decryptedData;
        }



        /// <summary>Encrypts the specified string, using the specified symmetric algorithm, key, and initialization vector,
        /// and returns the Base64 string representation of the encrypted bytes.</summary>
        /// <param name="stringToEncrypt">String that is encrypted.</param>
        /// <param name="key">Secret key used in encryption.</param>
        /// <param name="initializationVector">Initialization vector used in encryption. Necessary for stronger encryption 
        /// (othervise repeating input blocks would cause repeating output blocks).</param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Agorithm used for encryption. Must be consistend with <param name="flag", or null.</param>
        /// <param name="SaltBytes">Salt that is prepended the data before encryption.</param>
        /// <param name="useLargestKey">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible key for this kind of algorithm.</param>
        /// <param name="useLargestBlock">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible block for this kind of algorithm.</param>
        /// <returns>Base64 string representation of the encrypted string bytes.</returns>
        public static string EncryptStringPlain(string stringToEncrypt, byte[] key, byte[] initializationVector,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.Default, SymmetricAlgorithm algorithm = null,
            byte[] saltBytes = null, bool useLargestKey = false, bool useLargestBlock = false)
        {
            return Convert.ToBase64String(
                EncryptStringToBytesPlain(stringToEncrypt, key, initializationVector, algorithmType, algorithm,
                saltBytes, useLargestKey, useLargestBlock)
                );
        }


        /// <summary>Derypts the specified string from encryted byte array, using the specified symmetric algorithm, key, and initialization vector.</summary>
        /// <param name="cipherText">Base64 string representation of the byte array containing the encrypted original string.</param>
        /// <param name="key">Secret key used in encryption.</param>
        /// <param name="initializationVector">Initialization vector used in encryption. Necessary for stronger encryption 
        /// (othervise repeating input blocks would cause repeating output blocks).</param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Agorithm used for encryption. Must be consistend with <param name="flag", or null.</param>
        /// <param name="numSaltBytes">Length of salt, which must be accounted for when decrypting.</param>
        /// <param name="useLargestKey">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible key for this kind of algorithm.</param>
        /// <param name="useLargestBlock">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible block for this kind of algorithm.</param>
        /// <returns>Original string, provided that input arguments are correct.</returns>
        public static string DecryptStringPlain(string cipherText, byte[] key, byte[] initializationVector,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.Default, SymmetricAlgorithm algorithm = null,
            int numSaltBytes = 0, bool useLargestKey = false, bool useLargestBlock = false)
        {
            return DecryptStringFromBytesPlain(Convert.FromBase64String(cipherText), key, initializationVector,
                algorithmType, null /* algorithm */, numSaltBytes, useLargestKey, useLargestBlock);
        }


        /// <summary>Encrypts the specified string to a byte array, using the specified symmetric algorithm, key, and initialization vector.</summary>
        /// <param name="stringToEncrypt">String that is encrypted.</param>
        /// <param name="key">Secret key used in encryption.</param>
        /// <param name="initializationVector">Initialization vector used in encryption. Necessary for stronger encryption 
        /// (othervise repeating input blocks would cause repeating output blocks).</param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Agorithm used for encryption. Must be consistend with <param name="flag", or null.</param>
        /// <param name="SaltBytes">Salt that is prepended the data before encryption.</param>
        /// <param name="useLargestKey">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible key for this kind of algorithm.</param>
        /// <param name="useLargestBlock">If true then the algorithm will be modified in such a way that it uses the largest 
        /// possible block for this kind of algorithm.</param>
        /// <returns>Array of bytes containing encrypted string.</returns>
        public static byte[] EncryptStringToBytesPlain(string stringToEncrypt, byte[] key, byte[] initializationVector,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.Default, SymmetricAlgorithm algorithm = null, 
            byte[] saltBytes = null, bool useLargestKey = false, bool useLargestBlock = false)
        {

            return EncryptPlain(StringEncoding.GetBytes(stringToEncrypt), key, initializationVector, algorithmType,
                algorithm, saltBytes, useLargestKey, useLargestBlock);

            //return EncryptStringToBytesPlain_OLD(stringToEncrypt, key, initializationVector,
            //    flag);  // , useLargestKey);

            //return EncryptStringToBytes_Msdn1_ToDeleteLater_OLD(stringToEncrypt, key, initializationVector,
            //    flag);  // , uselargestKey);

        }


        /// <summary>Derypts the specified string from encryted byte array, using the specified symmetric algorithm, key, and initialization vector.</summary>
        /// <param name="cipherText">Byte array containing the encrypted original string.</param>
        /// <param name="key">Secret key used in encryption.</param>
        /// <param name="initializationVector">Initialization vector used in encryption. Necessary for stronger encryption 
        /// (othervise repeating input blocks would cause repeating output blocks).</param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Agorithm used for encryption. Must be consistend with <param name="flag", or null.</param>
        /// <param name="numSaltBytes">Length of salt, which must be accounted for when decrypting.</param>
        /// <param name="useLargestKey">If true then the algorithm is modified (if necessary) in 
        /// such a way that the largest possible key size is used.</param>
        /// <param name="useLargestBlock">If true then the algorithm is modified in such a way 
        /// that the largest possible block size is used.</param>
        /// <returns>Original string, provided that input arguments are correct.</returns>
        public static string DecryptStringFromBytesPlain(byte[] cipherText, byte[] key, byte[] initializationVector,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.Default, SymmetricAlgorithm algorithm = null,
            int numSaltBytes = 0, bool useLargestKey = false, bool useLargestBlock = false)
        {

            return StringEncoding.GetString(
                DecryptPlain(cipherText, key, initializationVector, algorithmType, algorithm, numSaltBytes,
                    useLargestKey, useLargestBlock));

            //return DecryptStringFromBytesPlain_OLD(cipherText, key, initializationVector,
            //    flag, useLargestKey, useLargestBlock);


            //return DecryptStringFromBytes_Msdn1_ToDeleteLater_OLD(cipherText, key, initializationVector,
            //    flag, useLargestKey, useLargestBlock);

        }






        /// <summary>Encrypts the data from the specified input stream by a symmetric encryption algorithm and writes encrypted data to
        /// the specified output stream.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="inputStream">Input stream from which data is encrypted. The stream must be open for reading and should 
        /// be closed by the caller of this method.</param>
        /// <param name="outputStream">Stream to which encrypted data is written. The stream must be open for writing and should
        /// be closed by the caller of this method.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="bufferSize">Size of the intermediate buffer through which we read from input and write to output stream.
        /// Buffer makes possible to work on very large streams efficiently.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static void EncryptPlain(Stream inputStream, Stream outputStream, byte[] key, byte[] IV,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None, SymmetricAlgorithm algorithm = null, 
            byte[] saltBytes = null, int bufferSize = ConstCrypto.FileBufferSize, bool useLargestKey = false, bool useLargestBlock = false)
        {
            // Repair eventual mismatches in algorithm parameters, take into account function argumennts:
            UtilCrypto.PrepareSymmetricAllgorithmPlain(ref key, ref IV, algorithmType, ref algorithm, useLargestKey, useLargestBlock);
            // Create a CryptoStream through which we will write encrypted data (output, e.g. encrypted bytes,
            // will be written to the underlying outputStream):
            using (CryptoStream cryptoStream = new CryptoStream(outputStream,
               algorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {

                if (saltBytes != null)
                    if (saltBytes.Length > 0)
                    {
                        // First, write the salt to the output stream to prepend it to the data from input stream
                        // before encryption:
                        cryptoStream.Write(saltBytes, 0, saltBytes.Length);
                    }


                // Create a buffer and process the input file in blocks. In this way we can process large amounts of streamed 
                // data or large files:
                byte[] buffer = new byte[bufferSize];
                int numRead;
                do
                {
                    // Read the  next block of data from the input stream: 
                    numRead = inputStream.Read(buffer, 0, bufferSize);

                    // Encrypt data onto the output stream: 
                    cryptoStream.Write(buffer, 0, numRead);

                } while (numRead != 0);


                // This will tell it that writing is finished and as there is no more data coming in, 
                // we can finalize the encryption (e.g. apply the padding as necessary, etc.) 
                cryptoStream.FlushFinalBlock();

                // cryptoStream.Close();  // stream will be closed by using block
            }
        }

        /// <summary>Decrypts the data from the specified input stream by a symmetric encryption algorithm and writes decrypted data to
        /// the specified output stream.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="inputStream">Input stream from which data is decrypted. The stream must be open for reading and should 
        /// be closed by the caller of this method.</param>
        /// <param name="outputStream">Stream to which decrypted data is written. The stream must be open for writing and should
        /// be closed by the caller of this method.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="bufferSize">Size of the intermediate buffer through which we read from input and write to output stream.
        /// Buffer makes possible to work on very large streams efficiently.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static void DecryptPlain(Stream inputStream, Stream outputStream, byte[] key, byte[] IV,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None, SymmetricAlgorithm algorithm = null,
            int numSaltBytes = 0, int bufferSize = ConstCrypto.FileBufferSize, bool useLargestKey = false, bool useLargestBlock = false)
        {
            // Repair eventual mismatches in algorithm parameters, take into account function argumennts:
            UtilCrypto.PrepareSymmetricAllgorithmPlain(ref key, ref IV, algorithmType, ref algorithm, useLargestKey, useLargestBlock);


            if (numSaltBytes <= 0)
            {
                if (numSaltBytes < 0)
                    throw new ArgumentException("Salt length less than 0 specified.");

                {
                    // Create a CryptoStream through which we will write decrypted data (output, e.g. encrypted bytes,
                    // will be written to the underlying outputStream):
                    using (CryptoStream cryptoStream = new CryptoStream(outputStream,
                       algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                    {

                        // Create a buffer and process the input file in blocks. In this way we can process large amounts of streamed 
                        // data or large files:
                        byte[] buffer = new byte[bufferSize];
                        int numRead;
                        do
                        {
                            // Read the  next block of data from the input stream: 
                            numRead = inputStream.Read(buffer, 0, bufferSize);

                            // Encrypt data onto the output stream: 
                            cryptoStream.Write(buffer, 0, numRead);

                        } while (numRead != 0);


                        // This will tell it that writing is finished and as there is no more data coming in, 
                        // we can finalize the encryption (e.g. apply the padding as necessary, etc.) 
                        cryptoStream.FlushFinalBlock();

                    }
                }

            } else
            {
                using (TempFileStream tempStream = new TempFileStream(FileAccess.ReadWrite, FileShare.Read, bufferSize))
                {


                    // Create a CryptoStream through which we will write decrypted data (output, e.g. encrypted bytes,
                    // will be written to the underlying outputStream):
                    using (CryptoStream cryptoStream = new CryptoStream(tempStream,
                       algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                    {

                        // Create a buffer and process the input file in blocks. In this way we can process large amounts of streamed 
                        // data or large files:
                        byte[] buffer = new byte[bufferSize];
                        int numRead;
                        do
                        {
                            // Read the  next block of data from the input stream: 
                            numRead = inputStream.Read(buffer, 0, bufferSize);

                            // Encrypt data onto the output stream: 
                            cryptoStream.Write(buffer, 0, numRead);

                        } while (numRead != 0);


                        // This will tell it that writing is finished and as there is no more data coming in, 
                        // we can finalize the encryption (e.g. apply the padding as necessary, etc.) 
                        cryptoStream.FlushFinalBlock();


                        // Now, from the decrypted file stream, read everything except the salt and write it on the output stream:
                        tempStream.Flush();
                        tempStream.Seek(numSaltBytes, SeekOrigin.Begin);

                        //byte[] buffer = new byte[bufferSize];
                        //int numRead;
                        do
                        {
                            // Read the  next block of data from the input stream: 
                            numRead = tempStream.Read(buffer, 0, bufferSize);

                            // Encrypt data onto the output stream: 
                            outputStream.Write(buffer, 0, numRead);

                        } while (numRead != 0);

                    }
                }
            }
            
        }



        /// <summary>Encrypts the data from the specified input file by a symmetric encryption algorithm and writes encrypted data to
        /// the specified output file.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="inputFilePath">Path to the input file from which data is encrypted. The file must be exist and  be readable.</param>
        /// <param name="deletedFilePath">Path to the output file to which encrypted data is written. The file must be writable and will be
        /// overwritten if it already exists.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="bufferSize">Size of the intermediate buffer through which we read from input and write to output stream.
        /// Buffer makes possible to work on very large streams efficiently.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static void EncryptFilePlain(string inputFilePath, string outputFilePath, byte[] key, byte[] IV,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None, SymmetricAlgorithm algorithm = null,
            byte[] saltBytes = null, int bufferSize = ConstCrypto.FileBufferSize, bool useLargestKey = false, bool useLargestBlock = false)
        {
            // First we open the file streams 
            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                if (inputStream == null)
                    throw new InvalidOperationException("Can not open the input file for reading. " + Environment.NewLine
                        + "  File path: " + inputFilePath + ".");
                using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    if (outputStream == null)
                        throw new InvalidOperationException("Can not open the output file for writiing. " + Environment.NewLine
                            + "  File path: " + outputFilePath + ".");
                    EncryptPlain(inputStream, outputStream, key, IV, algorithmType, algorithm,
                                saltBytes, bufferSize, useLargestKey, useLargestBlock);
                }
            }
        }

        /// <summary>Decrypts the data from the specified input file by a symmetric encryption algorithm and writes decrypted data to
        /// the specified output file.
        /// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        /// and the initialization vector must be specified.</para>
        /// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        /// and retrieved from the algorithm after the call.</para></summary>
        /// <param name="inputFilePath">Path to the input file from which data is encrypted. The file must be exist and  be readable.</param>
        /// <param name="deletedFilePath">Path to the output file to which encrypted data is written. The file must be writable and will be
        /// overwritten if it already exists.</param>
        /// <param name="key">Secret key used to encrypt the data.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        /// advised that the vecor is randomly generated for each session or each encryption.
        /// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        /// <param name="flag">Type of the algorithm used.</param>
        /// <param name="algorithm">Algoritgm used. 
        /// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        /// to the specified type.</para>
        /// <para>If not specified then it is generated anew.</para></param>
        /// <param name="bufferSize">Size of the intermediate buffer through which we read from input and write to output stream.
        /// Buffer makes possible to work on very large streams efficiently.</param>
        /// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible key size.</param>
        /// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        /// possible block size.</param>
        /// <returns>Encrypted data as byte array.</returns>
        public static void DecryptFilePlain(string inputFilePath, string outputFilePath, byte[] key, byte[] IV,
            SymmetricAlgorithmType algorithmType = SymmetricAlgorithmType.None, SymmetricAlgorithm algorithm = null,
            int numSaltBytes = 0, int bufferSize = ConstCrypto.FileBufferSize, bool useLargestKey = false, bool useLargestBlock = false)
        {
            // First we open the file streams 
            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                if (inputStream == null)
                    throw new InvalidOperationException("Can not open the input file for reading. " + Environment.NewLine
                        + "  File path: " + inputFilePath + ".");
                using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    if (outputStream == null)
                        throw new InvalidOperationException("Can not open the output file for writiing. " + Environment.NewLine
                            + "  File path: " + outputFilePath + ".");
                    DecryptPlain(inputStream, outputStream, key, IV, algorithmType, algorithm,
                                numSaltBytes, bufferSize, useLargestKey, useLargestBlock);
                }
            }
        }












        #region Obsolete_TO_DELETE_LATER


        ///// <para>Warning: </para>
        ///// <para>This performs a plain encryption. If length of the encrypted data is not consistent with the block size of the
        ///// encryption algorithm then the missing bytes are added and filled with 0s. In such cases, encrypted-decrypted data will 
        ///// not be completely the same as the original, but will have some zero bytes added and it will not be possible to infer this
        ///// from data itself (i.e., the lenngth of the original string must be known).</para>
        ///// <para>See also:  https://msdn.microsoft.com/en-us/library/System.Security.Cryptography.TripleDESCryptoServiceProvider(v=vs.80).aspx 
        /////    - in this example, just replace WriteLine with Write and ReadLine with ReadToEnd ! </para></remarks>
        ///// <param name="data"></param>
        ///// <param name="key"></param>
        ///// <param name="IV"></param>
        ///// <param name="flag"></param>
        ///// <param name="useLargestKey"></param>
        ///// <param name="useLargestBlock"></param>
        ///// <returns></returns>
        //[Obsolete("Replaced by other method.")]
        //public static byte[] EncryptStringToBytesPlain_OLD(string data, byte[] key, byte[] IV, SymmetricAlgorithmType flag,
        //    bool useLargestKey = false, bool useLargestBlock = false)
        //{
        //    // Convert the passed string to a byte array.
        //    byte[] toEncrypt = SymmetricEncryptionEncoding.GetBytes(data);
        //    // new UTF8Encoding().GetBytes(data); // new ASCIIEncoding().GetBytes(data);
        //    return EncryptPlain_OLD(toEncrypt, key, IV, flag, null, useLargestKey, useLargestBlock);
        //}


        //[Obsolete("Replaced by another method.")]
        //public static string DecryptStringFromBytesPlain_OLD(byte[] data, byte[] key, byte[] IV, SymmetricAlgorithmType flag,
        //    bool useLargestKey = false, bool useLargestBlock = false)
        //{
        //    return SymmetricEncryptionEncoding.GetString(DecryptPlain_OLD(data, key, IV, flag, null, useLargestKey, useLargestBlock));
        //    // new UTF8Encoding().GetString(DecryptPlain(data, key, IV, flag, useLargestKey, useLargestBlock)); // ASCIIEncoding().GetString(DecryptPlain(data, key, IV, flag, useLargestKey, useLargestBlock));
        //}



        ///// <summary>Encrypt the specified byte array by a symmetric encryption algorithm and returns encrypted bytes.
        ///// <para>If the algorithm is not specified then it is provided  by the system, but in this case both the key 
        ///// and the initialization vector must be specified.</para>
        ///// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        ///// and retrieved from the algorithm after the call.</para></summary>
        ///// <param name="data">Data to be encrypted.</param>
        ///// <param name="key">Secret key used to encrypt the data.
        ///// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        ///// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        ///// advised that the vecor is randomly generated for each session or each encryption.
        ///// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        ///// <param name="flag">Type of the algorithm used.</param>
        ///// <param name="algorithm">Algoritgm used. 
        ///// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        ///// to the specified type.</para>
        ///// <para>If not specified then it is generated anew.</para></param>
        ///// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        ///// possible key size.</param>
        ///// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        ///// possible block size.</param>
        ///// <returns>Encrypted data as byte array.</returns>
        //[Obsolete("Replaced by another method.")]
        //public static byte[] EncryptPlain_OLD(byte[] data, byte[] key, byte[] IV, SymmetricAlgorithmType flag,
        //    SymmetricAlgorithm algorithm = null, bool useLargestKey = false, bool useLargestBlock = false)
        //{
        //    //try
        //    //{

        //    PrepareSymmetricAllgorithm(ref key, ref IV, flag, ref algorithm, useLargestKey, useLargestBlock);

        //    //data = PadToSymmetricAlgorithmBlocksizeZero(data, algorithm, false /* useLargestBlock */);


        //    // Create a MemoryStream.
        //    MemoryStream mStream = new MemoryStream();
        //    // Create a CryptoStream using the MemoryStream 
        //    // and the passed key and initialization vector (IV).
        //    CryptoStream cStream = new CryptoStream(mStream,
        //        /* new TripleDESCryptoServiceProvider()*/ algorithm.CreateEncryptor(key, IV),
        //        CryptoStreamMode.Write);

        //    //// Convert the passed string to a byte array.
        //    //byte[] toEncrypt = new ASCIIEncoding().GetBytes(data);


        //    byte[] toEncrypt = data;


        //    // $$$$
        //    // toEncrypt = PadToSymmetricAlgorithmBlocksizeZero(toEncrypt, cryptoAlgorithm, false /* useLargestBlock */);


        //    int originalByteLength = toEncrypt.Length;

        //    // Write the byte array to the crypto stream and flush it.
        //    cStream.Write(toEncrypt, 0, toEncrypt.Length);
        //    cStream.FlushFinalBlock();

        //    // Get an array of bytes from the 
        //    // MemoryStream that holds the 
        //    // encrypted data.
        //    byte[] ReturnedString = mStream.ToArray();

        //    // Close the streams.
        //    cStream.Close();
        //    mStream.Close();

        //    // Return the encrypted buffer.

        //    // TODO: try what happens if this is uncommented!

        //    // $$
        //    //if (ReturnedString.Length > originalByteLength)
        //    //{
        //    //    // The encrypted array may be larger than the original one, which can be due
        //    //    // to padding in order to match algorithm's block size. In this case, we truncate
        //    //    // the redundand bytes:
        //    //    byte[] ret1 = new byte[originalByteLength];
        //    //    Array.Copy(ReturnedString, ret1, originalByteLength);
        //    //    return ret1;
        //    //}

        //    return ReturnedString;

        //    //}
        //    //catch (CryptographicException e)
        //    //{
        //    //    Console.WriteLine("Symmetric encryption/decryption: Cryptographic error occurred: {0}", e.Message);
        //    //    throw;
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Console.WriteLine("Symmetric encryption/decryption: Error occurred: {0}", e.Message);
        //    //    throw;
        //    //}
        //    //finally { }
        //}



        ///// <summary>Decrypts the specified byte array by a symmetric algorithm and returns decrypted data.
        ///// <para>If the algorithm is not specified then it is provided by the system, but in this case both the 
        ///// key and the initialization vector must be specified.</para>
        ///// <para>If algorithm is specified then the key and initialization vector can be generated by the algorithm
        ///// and retrieved from the algorithm after the call.</para></summary>
        ///// <param name="data">Data to be decrypted.</param>
        ///// <param name="key">Secret key used to decrypt the data.
        ///// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        ///// <param name="IV">Initialization vector. It can be public and stored together with encrypted data, but it is
        ///// advised that the vecor is randomly generated for each session or each encryption.
        ///// <para>If not specified but algorithm is specified, it is randomly generated and stored by the algorithm.</para></param>
        ///// <param name="flag">Type of the algorithm used.</param>
        ///// <param name="algorithm">Algoritgm used. 
        ///// <para>If specified then key and IV can be generated and stored by the algorithm. Algorithm type must correspond 
        ///// to the specified type.</para>
        ///// <para>If not specified then it is generated anew.</para></param>
        ///// <param name="useLargestKey">If true then algorithm is eventually modified in such a way that it uses the largest
        ///// possible key size.</param>
        ///// <param name="useLargestBlock">If true then algorithm is eventually modified in such a way that it uses the largest
        ///// possible block size.</param>
        ///// <returns>Decrypted data as byte array.</returns>
        //[Obsolete("Replaced by another method.")]
        //public static byte[] DecryptPlain_OLD(byte[] data, byte[] key, byte[] IV, SymmetricAlgorithmType flag,
        //    SymmetricAlgorithm algorithm,
        //    bool useLargestKey = false, bool useLargestBlock = false)
        //{
        //    //try
        //    //{


        //    PrepareSymmetricAllgorithm(ref key, ref IV, flag, ref algorithm, useLargestKey, useLargestBlock);


        //    //data = PadToSymmetricAlgorithmBlocksizeZero(data, algorithm, false /* useLargestBlock */);


        //    // Create a new MemoryStream using the passed 
        //    // array of encrypted data.
        //    MemoryStream msDecrypt = new MemoryStream(data);

        //    // Create a CryptoStream using the MemoryStream 
        //    // and the passed key and initialization vector (IV).


        //    CryptoStream csDecrypt = new CryptoStream(msDecrypt,
        //        algorithm.CreateDecryptor(key, IV),
        //        CryptoStreamMode.Read);

        //    // Create buffer to hold the decrypted data.
        //    byte[] fromEncrypt = new byte[data.Length];

        //    // Read the decrypted data out of the crypto stream
        //    // and place it into the temporary buffer.
        //    csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

        //    return fromEncrypt;

        //    //////Convert the buffer into a string and return it.
        //    ////return new ASCIIEncoding().GetString(fromEncrypt);
        //    //}
        //    //catch (CryptographicException e)
        //    //{
        //    //    Console.WriteLine("Symmetric encryption/decryption: Cryptographic error occurred: {0}", e.Message);
        //    //    throw;
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    Console.WriteLine("Symmetric encryption/decryption: Error occurred: {0}", e.Message);
        //    //    throw;
        //    //}
        //    //finally {  }
        //}


        #endregion Obsolete_TO_DELETE_LATER




        //#region FromMsdn1
        //// https://msdn.microsoft.com/en-us/library/f9df14hc(v=vs.110).aspx

        ///// <summary>Encrypts the specified string to a byte array, using the specified symmetric algorithm, key, and initialization vector.</summary>
        ///// <param name="stringToEncrypt">String that is encrypted.</param>
        ///// <param name="key">Secret key used in encryption.</param>
        ///// <param name="initializationVector">Initialization vector used in encryption. Necessary for stronger encryption 
        ///// (othervise repeating input blocks would cause repeating output blocks).</param>
        ///// <param name="flag">Type of the algorithm used.</param>
        ///// <param name="useLargestKey">If true then the algorithm is modified in such a way that the largest possible key is uesd.</param>
        ///// <param name="useLargestBlock">If true then the algorithm will be modified in such a way that it uses the largest 
        ///// possible block for this kind of algorithm.</param>
        ///// <returns>Array of bytes containing encrypted string.</returns>
        //public static byte[] EncryptStringToBytes_Msdn1_ToDeleteLater_OLD(string stringToEncrypt, byte[] key, byte[] initializationVector,
        //    SymmetricAlgorithmType flag = SymmetricAlgorithmType.Default, bool useLargestKey = false,
        //    bool useLargestBlock = false)
        //{
        //    // Check arguments. 
        //    if (string.IsNullOrEmpty(stringToEncrypt))
        //        throw new ArgumentNullException("String to encrypt is not specified properly (null or empty string).");
        //    if (key == null || key.Length <= 0)
        //        throw new ArgumentNullException("Symmetric encryption (" + SymmetricAlgorithmTypeToString(flag)
        //            + ") key is not specified properly (null or empty array).");
        //    if (initializationVector == null || initializationVector.Length <= 0)
        //        throw new ArgumentNullException("Symmetric encryption (" + SymmetricAlgorithmTypeToString(flag)
        //            + ") initialization vector is not specified properly (null or empty array).");
        //    byte[] encrypted;
        //    // Create an RijndaelManaged object 
        //    // with the specified key and IV. 

        //    //using (
        //    SymmetricAlgorithm cryptoAlgorithm = GetSymmetricEncryptionAlgorithm(flag);
        //    //)
        //    {
        //        int originalAlgorithmKeySize = cryptoAlgorithm.KeySize;
        //        int originalKeysize = key.Length * 8;
        //        int maximalAlgoritmKeysize = GetLargestSymmetricAlgorithmKeySize(cryptoAlgorithm);

        //        if (useLargestKey)
        //            SetLargestSymmetricAlgorithmKeySize(cryptoAlgorithm);
        //        if (useLargestBlock)
        //            SetLargestSymmetricAlgorithmBlockSize(cryptoAlgorithm);

        //        key = GetValidSymmetricAlgorithmKey(key, cryptoAlgorithm);
        //        initializationVector = GetValidSymmetricAlgorithmIV(initializationVector, cryptoAlgorithm);

        //        Console.WriteLine(Environment.NewLine + "    Alg. " + SymmetricAlgorithmTypeToString(flag)
        //            + ":  key length = " + cryptoAlgorithm.KeySize + " b; " + ((double)cryptoAlgorithm.KeySize / 8.0)
        //            + ", actual: " + key.Length
        //            + ", original alg. (b): " + originalAlgorithmKeySize + ", key: " + originalKeysize + ", max.: " + maximalAlgoritmKeysize);


        //        // 
        //        //cryptoAlgorithm.Key = key;
        //        //cryptoAlgorithm.IV = initializationVector;

        //        // Create a encryptor to perform the stream transform.
        //        ICryptoTransform encryptor = cryptoAlgorithm.CreateEncryptor(cryptoAlgorithm.Key, cryptoAlgorithm.IV);

        //        // Create the streams used for encryption. 
        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                {

        //                    //Write all data to the stream.
        //                    swEncrypt.Write(stringToEncrypt);
        //                    csEncrypt.FlushFinalBlock();
        //                }
        //                encrypted = msEncrypt.ToArray();
        //            }
        //        }
        //    }

        //    // Return the encrypted bytes from the memory stream. 
        //    return encrypted;
        //}


        ///// <summary>Derypts the specified string from encryted byte array, using the specified symmetric algorithm, key, and initialization vector.</summary>
        ///// <param name="cipherText">Byte array containing the encrypted original string.</param>
        ///// <param name="key">Secret key used in encryption.</param>
        ///// <param name="initializationVector">Initialization vector used in encryption. Necessary for stronger encryption 
        ///// (othervise repeating input blocks would cause repeating output blocks).</param>
        ///// <param name="flag">Type of the algorithm used.</param>
        ///// <param name="useLargestKey">If true then the algorithm is modified in such a way that the largest possible key is uesd.</param>
        ///// <param name="useLargestBlock">If true then the algorithm will be modified in such a way that it uses the largest 
        ///// possible block for this kind of algorithm.</param>
        ///// <returns>Original string, provided that input arguments are correct.</returns>
        //public static string DecryptStringFromBytes_Msdn1_ToDeleteLater_OLD(byte[] cipherText, byte[] key, byte[] initializationVector,
        //    SymmetricAlgorithmType flag = SymmetricAlgorithmType.Default, bool useLargestKey = false,
        //    bool useLargestBlock = false)
        //{
        //    // Check arguments. 
        //    if (cipherText == null || cipherText.Length <= 0)
        //        throw new ArgumentNullException("Cypher text to decrypt is not specified properly (null or empty array).");
        //    if (key == null || key.Length <= 0)
        //        throw new ArgumentNullException("Symmetric encryption (" + SymmetricAlgorithmTypeToString(flag)
        //            + ") key is not specified properly (null or empty array).");
        //    if (initializationVector == null || initializationVector.Length <= 0)
        //        throw new ArgumentNullException("Symmetric encryption (" + SymmetricAlgorithmTypeToString(flag)
        //            + ") initialization vector is not specified properly (null or empty array).");

        //    // Declare the string used to hold 
        //    // the decrypted text. 
        //    string decryptedString = null;

        //    // Create an RijndaelManaged object 
        //    // with the specified key and IV. 

        //    // using (
        //    SymmetricAlgorithm cryptoAlgorithm = GetSymmetricEncryptionAlgorithm(flag);
        //    //)
        //    {
        //        int originalAlgorithmKeySize = cryptoAlgorithm.KeySize;
        //        int originalKeysize = key.Length * 8;
        //        int maximalAlgoritmKeysize = GetLargestSymmetricAlgorithmKeySize(cryptoAlgorithm);


        //        if (useLargestKey)
        //            SetLargestSymmetricAlgorithmKeySize(cryptoAlgorithm);
        //        if (useLargestBlock)
        //            SetLargestSymmetricAlgorithmBlockSize(cryptoAlgorithm);

        //        key = GetValidSymmetricAlgorithmKey(key, cryptoAlgorithm);
        //        initializationVector = GetValidSymmetricAlgorithmIV(initializationVector, cryptoAlgorithm);

        //        Console.WriteLine(Environment.NewLine + "    Alg. " + SymmetricAlgorithmTypeToString(flag)
        //            + ":  key length = " + cryptoAlgorithm.KeySize + " b; " + ((double)cryptoAlgorithm.KeySize / 8.0)
        //            + ", actual: " + key.Length
        //            + ", original alg. (b): " + originalAlgorithmKeySize + ", key: " + originalKeysize + ", max.: " + maximalAlgoritmKeysize);


        //        //cryptoAlgorithm.Key = key;
        //        //cryptoAlgorithm.IV = initializationVector;

        //        // Create a decrytor to perform the stream transform.
        //        ICryptoTransform decryptor = cryptoAlgorithm.CreateDecryptor(cryptoAlgorithm.Key, cryptoAlgorithm.IV);

        //        // Create the streams used for decryption. 
        //        using (MemoryStream msDecrypt = new MemoryStream(cipherText))
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                {

        //                    // Read the decrypted bytes from the decrypting stream 
        //                    // and place them in a string.

        //                    decryptedString = srDecrypt.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //    return decryptedString;
        //}

        //#endregion FromMsdn1




        //#region FromMsdn2
        //// https://msdn.microsoft.com/en-us/library/System.Security.Cryptography.TripleDESCryptoServiceProvider(v=vs.80).aspx



        //[Obsolete("Replaced bt another method.")]
        //public static byte[] EncryptTextToMemory_Msdn2_ToDeleteLater_OLD(string data, byte[] key, byte[] IV, SymmetricAlgorithmType flag,
        //    bool useLargestKey = false, bool useLargestBlock = false)
        //{
        //    try
        //    {
        //        // Create a MemoryStream.
        //        MemoryStream mStream = new MemoryStream();

        //        SymmetricAlgorithm cryptoAlgorithm = GetSymmetricEncryptionAlgorithm(flag);  // new TripleDESCryptoServiceProvider();

        //        int originalAlgorithmKeySize = cryptoAlgorithm.KeySize;
        //        int originalKeysize = key.Length * 8;
        //        int maximalAlgoritmKeysize = GetLargestSymmetricAlgorithmKeySize(cryptoAlgorithm);

        //        if (useLargestKey)
        //            SetLargestSymmetricAlgorithmKeySize(cryptoAlgorithm);
        //        if (useLargestBlock)
        //            SetLargestSymmetricAlgorithmBlockSize(cryptoAlgorithm);

        //        key = GetValidSymmetricAlgorithmKey(key, cryptoAlgorithm);
        //        IV = GetValidSymmetricAlgorithmIV(IV, cryptoAlgorithm);


        //        Console.WriteLine(Environment.NewLine + "    Alg. " + SymmetricAlgorithmTypeToString(flag)
        //            + ":  key length = " + cryptoAlgorithm.KeySize + " b; " + ((double)cryptoAlgorithm.KeySize / 8.0)
        //            + ", actual: " + key.Length
        //            + ", original alg. (b): " + originalAlgorithmKeySize + ", key: " + originalKeysize + ", max.: " + maximalAlgoritmKeysize);

        //        // Create a CryptoStream using the MemoryStream 
        //        // and the passed key and initialization vector (IV).
        //        CryptoStream cStream = new CryptoStream(mStream,
        //            /* new TripleDESCryptoServiceProvider()*/ cryptoAlgorithm.CreateEncryptor(key, IV),
        //            CryptoStreamMode.Write);

        //        // Convert the passed string to a byte array.
        //        byte[] toEncrypt = new ASCIIEncoding().GetBytes(data);

        //        // $$$$
        //        // toEncrypt = PadToSymmetricAlgorithmBlocksizeZero(toEncrypt, cryptoAlgorithm, false /* useLargestBlock */);


        //        int originalByteLength = toEncrypt.Length;

        //        // Write the byte array to the crypto stream and flush it.
        //        cStream.Write(toEncrypt, 0, toEncrypt.Length);
        //        cStream.FlushFinalBlock();

        //        // Get an array of bytes from the 
        //        // MemoryStream that holds the 
        //        // encrypted data.
        //        byte[] ReturnedString = mStream.ToArray();

        //        // Close the streams.
        //        cStream.Close();
        //        mStream.Close();

        //        // Return the encrypted buffer.

        //        // TODO: try what happens if this is uncommented!

        //        // $$
        //        //if (ReturnedString.Length > originalByteLength)
        //        //{
        //        //    // The encrypted array may be larger than the original one, which can be due
        //        //    // to padding in order to match algorithm's block size. In this case, we truncate
        //        //    // the redundand bytes:
        //        //    byte[] ret1 = new byte[originalByteLength];
        //        //    Array.Copy(ReturnedString, ret1, originalByteLength);
        //        //    return ret1;
        //        //}

        //        return ReturnedString;
        //    }
        //    catch (CryptographicException e)
        //    {
        //        Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
        //        throw;
        //        // return null;
        //    }

        //}

        //[Obsolete("Replaced bt another method.")]
        //public static string DecryptTextFromMemory_Msdn2_ToDeleteLater_OLD(byte[] data, byte[] key, byte[] IV, SymmetricAlgorithmType flag,
        //    bool useLargestKey = false, bool useLargestBlock = false)
        //{
        //    try
        //    {
        //        // Create a new MemoryStream using the passed 
        //        // array of encrypted data.
        //        MemoryStream msDecrypt = new MemoryStream(data);

        //        // Create a CryptoStream using the MemoryStream 
        //        // and the passed key and initialization vector (IV).


        //        SymmetricAlgorithm cryptoAlgorithm = GetSymmetricEncryptionAlgorithm(flag);  // new TripleDESCryptoServiceProvider();



        //        int originalAlgorithmKeySize = cryptoAlgorithm.KeySize;
        //        int originalKeysize = key.Length * 8;
        //        int maximalAlgoritmKeysize = GetLargestSymmetricAlgorithmKeySize(cryptoAlgorithm);


        //        if (useLargestKey)
        //            SetLargestSymmetricAlgorithmKeySize(cryptoAlgorithm);
        //        if (useLargestBlock)
        //            SetLargestSymmetricAlgorithmBlockSize(cryptoAlgorithm);

        //        key = GetValidSymmetricAlgorithmKey(key, cryptoAlgorithm);
        //        IV = GetValidSymmetricAlgorithmIV(IV, cryptoAlgorithm);

        //        data = PadToSymmetricAlgorithmBlocksizeZero(data, cryptoAlgorithm, false /* useLargestBlock */);

        //        Console.WriteLine(Environment.NewLine + "    Alg. " + SymmetricAlgorithmTypeToString(flag)
        //            + ":  key length = " + cryptoAlgorithm.KeySize + " b; " + ((double)cryptoAlgorithm.KeySize / 8.0)
        //            + ", actual: " + key.Length
        //            + ", original alg. (b): " + originalAlgorithmKeySize + ", key: " + originalKeysize + ", max.: " + maximalAlgoritmKeysize);




        //        CryptoStream csDecrypt = new CryptoStream(msDecrypt,
        //            cryptoAlgorithm.CreateDecryptor(key, IV),
        //            CryptoStreamMode.Read);

        //        // Create buffer to hold the decrypted data.
        //        byte[] fromEncrypt = new byte[data.Length];

        //        // Read the decrypted data out of the crypto stream
        //        // and place it into the temporary buffer.
        //        csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

        //        //Convert the buffer into a string and return it.
        //        return new ASCIIEncoding().GetString(fromEncrypt);
        //    }
        //    finally { }
        //    //catch (CryptographicException e)
        //    //{
        //    //    Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
        //    //    throw;
        //    //    //return null;
        //    //}
        //}



        //#endregion FromMsdn2




        //#region FromStackOverflow
        //// http://stackoverflow.com/questions/202011/encrypt-and-decrypt-a-string/10366194#10366194


        //private static byte[] _saltSymmetricEncryption = Encoding.ASCII.GetBytes("o6806642kbM7c5");

        ///// <summary>
        ///// Encrypt the given string using AES.  The string can be decrypted using 
        ///// DecryptStringc().  The sharedSecret parameters must match.
        ///// </summary>
        ///// <param name="plainText">The text to encrypt.</param>
        ///// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        //public static string EncryptStringAES_original_ToCheckAndDeleteLater(string plainText, string sharedSecret)
        //{
        //    if (string.IsNullOrEmpty(plainText))
        //        throw new ArgumentNullException("plainText");
        //    if (string.IsNullOrEmpty(sharedSecret))
        //        throw new ArgumentNullException("sharedSecret");

        //    string outStr = null;                       // Encrypted string to return
        //    RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

        //    try
        //    {
        //        // generate the key from the shared secret and the salt
        //        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _saltSymmetricEncryption);

        //        // Create a RijndaelManaged object
        //        aesAlg = new RijndaelManaged();
        //        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

        //        // Create a decryptor to perform the stream transform.
        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        // Create the streams used for encryption.
        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            // prepend the IV
        //            msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
        //            msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                {
        //                    //Write all data to the stream.
        //                    swEncrypt.Write(plainText);
        //                }
        //            }
        //            outStr = Convert.ToBase64String(msEncrypt.ToArray());
        //        }
        //    }
        //    finally
        //    {
        //        // Clear the RijndaelManaged object.
        //        if (aesAlg != null)
        //            aesAlg.Clear();
        //    }

        //    // Return the encrypted bytes from the memory stream.
        //    return outStr;
        //}

        ///// <summary>
        ///// Decrypt the given string.  Assumes the string was encrypted using 
        ///// EncryptStringAES(), using an identical sharedSecret.
        ///// </summary>
        ///// <param name="cipherText">The text to decrypt.</param>
        ///// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        //public static string DecryptStringAES_original_ToCheckAndDeleteLater(string cipherText, string sharedSecret)
        //{
        //    if (string.IsNullOrEmpty(cipherText))
        //        throw new ArgumentNullException("cipherText");
        //    if (string.IsNullOrEmpty(sharedSecret))
        //        throw new ArgumentNullException("sharedSecret");

        //    // Declare the RijndaelManaged object
        //    // used to decrypt the data.
        //    RijndaelManaged aesAlg = null;

        //    // Declare the string used to hold
        //    // the decrypted text.
        //    string plaintext = null;

        //    try
        //    {
        //        // generate the key from the shared secret and the salt
        //        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _saltSymmetricEncryption);

        //        // Create the streams used for decryption.                
        //        byte[] bytes = Convert.FromBase64String(cipherText);
        //        using (MemoryStream msDecrypt = new MemoryStream(bytes))
        //        {
        //            // Create a RijndaelManaged object
        //            // with the specified key and IV.
        //            aesAlg = new RijndaelManaged();
        //            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
        //            // Get the initialization vector from the encrypted stream
        //            aesAlg.IV = ReadByteArray(msDecrypt);
        //            // Create a decrytor to perform the stream transform.
        //            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))

        //                    // Read the decrypted bytes from the decrypting stream
        //                    // and place them in a string.
        //                    plaintext = srDecrypt.ReadToEnd();
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        // Clear the RijndaelManaged object.
        //        if (aesAlg != null)
        //            aesAlg.Clear();
        //    }

        //    return plaintext;
        //}

        //private static byte[] ReadByteArray(Stream s)
        //{
        //    byte[] rawLength = new byte[sizeof(int)];
        //    if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
        //    {
        //        throw new SystemException("Stream did not contain properly formatted byte array");
        //    }

        //    byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
        //    if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
        //    {
        //        throw new SystemException("Did not read byte array properly");
        //    }

        //    return buffer;
        //}


        //#endregion FromStackOverflow










        #endregion SymmetricEncryption.Plain







        #endregion SymmetrixEncryption



        #region AsymmetricEncryption

        // See:

        // How to create a self-signed certificats:
        // .NET does not have sufficient tools! 
        // See this answer for the whole procedure in .NET (last answer is cleaned code) - BUT USING Bouncy Castle, which is Java!: http://stackoverflow.com/questions/22230745/generate-self-signed-certificate-on-the-fly 
        // By using Windows API (COM object) OR PURE MANAGED external libraries 
        //     (Mno.Security; CLR Security extensions library (Ms-LPL!!); PluralSight.Crypto library(komercialna?)):  http://stackoverflow.com/questions/13806299/how-to-create-a-self-signed-certificate-using-c
        // Maybe an option is Moono Security (MIT X11 license), but its doc sais it should NEVER be used directly by any application (e.g. referencing 
        //     the assembly from a project):  http://www.mono-project.com/archived/cryptography/#assembly-monosecuritywin32 
        // By using Windows API (CertCreateSelfSignCertificate): http://blogs.msdn.com/b/dcook/archive/2008/11/25/creating-a-self-signed-certificate-in-c.aspx 


        // X509Certificate2 Class - see the 1st example for encrypt/decrypt using a certificate - msdn: https://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509certificate2%28v=vs.110%29.aspx 

        // Key exchange with RSA - RSAPKCS1KeyExchangeFormatter Class -msdn: https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsapkcs1keyexchangeformatter%28v=vs.110%29.aspx 

        // Introduction to asymmetric encryption in .NET cryptography: http://dotnetcodr.com/2013/11/11/introduction-to-asymmetric-encryption-in-net-cryptography/

        // RSAParameters - kratek opis RSA algoritma s parametri - msdn: https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsaparameters%28v=vs.110%29.aspx 
        // X.509 Certificates in .NET - CodeProject: http://www.codeproject.com/Articles/9075/X-Certificates-in-NET 

        // RSA Private Key Encryption - CodeProject: http://www.codeproject.com/Articles/38739/RSA-Private-Key-Encryption

        // (ni dober) Cryptography : Asymmetric Encryption by using Asymmetric Algorithm Classes - CodeProject: http://www.codeproject.com/Articles/448719/Cryptography-Asymmetric-Encryption-by-using-Asymme
        // Combining Symmetric and Asymmetric Encryption: http://www.codeproject.com/Articles/8648/Combining-Symmetric-and-Asymmetric-Encryption

        // RSA Algorithm With C# - z opisi algoritmov: http://www.c-sharpcorner.com/UploadFile/75a48f/rsa-algorithm-with-C-Sharp2/ 
        //  Več na temo enkripcije na C# Corner (z opisom algoritmov): http://www.c-sharpcorner.com/1/60/cryptography-C-Sharp.aspx 
        // Kratek pregled, C# Corner: http://www.c-sharpcorner.com/UploadFile/gsparamasivam/CryptEncryption11282005061028AM/CryptEncryption.aspx 
        //   Več razumljivih člannkov o enkripciji na C# corner: http://www.c-sharpcorner.com/1/60/cryptography-C-Sharp.aspx 

        // Dealing with asymmetric keys and certificates: 
        // How to assign a private key to a certificate (from command line or code): http://vunvulearadu.blogspot.com/2013/10/how-to-assign-private-key-to.html 
        // Creating Self Signed Certificates with PowerShell (export also described): http://windowsitpro.com/blog/creating-self-signed-certificates-powershell 
        // How to find Certificates on Windows: https://technet.microsoft.com/en-us/library/cc772317.aspx 
        // Export a Certificate with the Private Key: https://technet.microsoft.com/en-us/library/cc754329.aspx 
        //    Remark: not alll cerrtificates allow exporting private key. If disalowed, this can not be done through Windows GUI, but other tools do exist 



        #region AsymmetricEncryption.Constants

        // Constants that determine behavior of some functions. These constants should be kept fixed over time.

        //private static const bool _useLasgestSymmetricEncryptionKeys = true;

        ///// <summary>If true then symmetric keys are always forced to use the largest possible keys.</summary>
        //public static bool UseLasgestSymmetricEncryptionKeys
        //{
        //    get { return _useLasgestSymmetricEncryptionKeys; }
        //}

        #endregion AsymmetricEncryption.Constants



        #region AsymmetricEncryption.General



        private static AsymmetricAlgorithmType[] _asymmetricTypes = null;

        /// <summary>Returns all asymmetric encryption algorithm types contained in the enumeration <see cref="AsymAlgorithmType"/></summary>
        public static AsymmetricAlgorithmType[] GetAsymmetricAlgorithmTypes()
        {
            if (_asymmetricTypes == null)
            {
                List<AsymmetricAlgorithmType> allTypes = new List<AsymmetricAlgorithmType>();
                lock (Util.LockGlobal)
                {
                    if (_asymmetricTypes == null)
                    {
                        IEnumerable<AsymmetricAlgorithmType> tmpTypes = Util.GetEnumValues<AsymmetricAlgorithmType>();
                        foreach (AsymmetricAlgorithmType specType in tmpTypes)
                            allTypes.Add(specType);
                        _asymmetricTypes = allTypes.ToArray();
                    }
                }
            }
            return _asymmetricTypes;
        }

        /// <summary>Returns the <see cref="AsymmetricAlgorithmType"/> value corresponding to the specified string.</summary>
        /// <param name="typeString">String that represents the asymmetric encryption algorithm type.</param>  
        public static AsymmetricAlgorithmType GetAsymmetricAlgorithmType(string typeString)
        {
            try
            {
                AsymmetricAlgorithmType type = (AsymmetricAlgorithmType)Enum.Parse(typeof(AsymmetricAlgorithmType), typeString);
                return type;
            }
            catch { }
            if (typeString != null)
                typeString = typeString.ToUpper();
            if (string.IsNullOrEmpty(typeString))
                return AsymmetricAlgorithmType.Default;
            if (typeString == "NONE")
                return AsymmetricAlgorithmType.None;
            if (typeString == "RSA")
                return AsymmetricAlgorithmType.RSA;
            if (typeString == "DSA")
                return AsymmetricAlgorithmType.DSA;
            if (typeString == "DEFAULT")
                return AsymmetricAlgorithmType.Default;
            throw new ArgumentException("Did not recognize the following asymmetric algorithm type: " + typeString);
            //return AsymAlgorithmType.None;
        }


        /// <summary>Returns the asymmetric encryption algorithm type, i.e. an enumeration of type <see cref="SymmetricAlgorithmType"/>,
        /// for the specified asymmetric encryption algorithm object.</summary>
        /// <param name="algorithmObject">Asymmetric algorithm object for which type enumeration is returned.</param>
        /// <returns>The type enumeration corresponding to the specified algorithm, or 0 (None) if the algorithm is null or not
        /// contained in the enumeration.</returns>
        public static AsymmetricAlgorithmType GetAsymmetricAlgorithmType(AsymmetricAlgorithm algorithmObject)
        {
            if (algorithmObject == null)
                return AsymmetricAlgorithmType.None;
            if (algorithmObject is System.Security.Cryptography.RSA)
                return AsymmetricAlgorithmType.RSA;
            if (algorithmObject is System.Security.Cryptography.DSA)
                return AsymmetricAlgorithmType.DSA;
            return AsymmetricAlgorithmType.None;
        }



        /// <summary>Returns standard string representation of the specified asymmetric encryption algorithm type.</summary>
        /// <param name="flag">Asymmetric encryption algorithm type whose string representation is returned.</param>
        public static string AsymmetricAlgorithmTypeToString(AsymmetricAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case AsymmetricAlgorithmType.None:
                    return "None";
                case AsymmetricAlgorithmType.RSA:
                    return "RSA";
                case AsymmetricAlgorithmType.DSA:
                    return "DSA";
                default:
                    return algorithmType.ToString();
            }
        }

        /// <summary>Returns the appropriate asymmetric algorithm according to the specified hashRet type, or null 
        /// if the type is not recognized or the method is not implemented for that type.</summary>
        /// <param name="flag">Specification of the asymmetric encryption algorithm type.</param>
        public static AsymmetricAlgorithm GetAsymmetricEncryptionAlgorithm(AsymmetricAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case AsymmetricAlgorithmType.RSA:
                    return new System.Security.Cryptography.RSACryptoServiceProvider();
                case AsymmetricAlgorithmType.DSA:
                    return new System.Security.Cryptography.DSACryptoServiceProvider();
                default:
                    return null;
            }
        }

        /// <summary>Returns true if the specified asymmetric encryption algorithm corresponds the type specification, false otherwise.
        /// <para>If the specified algorithm is null or the specified type is unknown then false is returned.</para></summary>
        /// <param name="flag">Specification of the asymmetric encryption algorithm type.</param>
        /// <param name="algorithm">Algorithm that is checked for type correctness.</param>
        public static bool IsCorrectAsymmetricEncryptionAlgorithm(AsymmetricAlgorithm algorithm, AsymmetricAlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case AsymmetricAlgorithmType.None:
                    return true;  // type not specified, all types are valid.
                case AsymmetricAlgorithmType.RSA:
                    return (algorithm as RSACryptoServiceProvider != null);
                case AsymmetricAlgorithmType.DSA:
                    return (algorithm as DSACryptoServiceProvider != null);
                default:
                    return false;
            }
        }


        /// <summary>Returns the largest possible key size, in bits, for the spcified symmetric encryption algorithm.</summary>
        /// <param name="algorithm">Algorithm whose largest possible key size is returned.</param>
        public static int GetLargestAsymmetricAlgorithmKeySize(AsymmetricAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentException("Asymmetric encryption algorithm not specified for getting the largest key size (null reference).");
            KeySizes[] keySizes = algorithm.LegalKeySizes;
            int maxSize = 0;
            foreach (KeySizes keySize in keySizes)
            {
                int size = keySize.MaxSize;
                if (size > maxSize)
                    maxSize = size;
            }
            return maxSize;
        }

        /// <summary>On the specified asymmetric encryption algorithm, sets the largest possible key size, in bits, for the corresponding asymmetric encryption algorithm.</summary>
        /// <param name="algorithm">Algorithm whose largest possible key size is returned.</param>
        /// <returns>Actual key size on the algorithm after the operation (should be the largest possible key).</returns>
        private static int SetLargestAsymmetricAlgorithmKeySize(AsymmetricAlgorithm algorithm)
        {
            if (algorithm == null)
                throw new ArgumentException("Asymmetric encryption algorithm not specified for setting the largest key size (null reference).");
            int currentSize = algorithm.KeySize;
            int largestSize = GetLargestAsymmetricAlgorithmKeySize(algorithm);
            if (currentSize != largestSize)
                algorithm.KeySize = largestSize;
            return algorithm.KeySize;
        }

        /// <summary>For the specified asymmetric encryption algorithm, this function takes the specified key and checks if the 
        /// key is of valid size. If yes then the unmodified key is returned, otherwise the modified key that fits the required
        /// size is returned.</summary>
        /// <param name="key">Original key provided for the algorithm.</param>
        /// <param name="algorithm">Asymmetric algorithm that is used for encryption/decryption. Object is used to query and 
        /// eventually set the key sizes.</param>
        /// <param name="useLargestKey">If true then the specified algorithm is first modified in such a way that it uses
        /// the maximal possible key length for the specific kind of algorithm.</param>
        /// <returns>A key that is of valid size for the algorithm. 
        /// <para>If the original key size matches the required one, the key is unmodified.</para>
        /// <para>If the original key is larger than the required one, the first bytes of the provided key are used in the returned key.</para>
        /// <para>If the provided key is smaller than the erquired key size then bytes in the returned key are cyclically repeated
        /// bytes from the original key.</para></returns>
        public static byte[] GetValidAsymmetricAlgorithmKey(byte[] key, AsymmetricAlgorithm algorithm, bool useLargestKey = false)
        {
            if (key == null)
                throw new ArgumentException("Asymmetric encryption key not specified for valid key extraction (null reference).");
            if (useLargestKey)
                SetLargestAsymmetricAlgorithmKeySize(algorithm);
            int providedSize = key.Length;
            if (providedSize < 1)
                throw new ArgumentException("Asymmetric encryption key not specified properly for valid key extraction (size too  small).");
            if (algorithm == null)
                throw new ArgumentException("Asymmetric encryption algorithm not specified for valid key extraction (null reference).");
            int requiredSize = algorithm.KeySize / 8;
            if (providedSize == requiredSize)
                return key;
            else
            {
                byte[] ret = new byte[requiredSize];
                for (int i = 0; i < requiredSize; ++i)
                    ret[i] = key[i % providedSize];
                return ret;
            }
        }


        /// <summary>Returns the value of the CspProviderFlags enum corresponding to the specified string.</summary>
        /// <param name="flagString">String representation of the flag value of type <see cref="CspProviderFlags"/></param>
        /// <returns><see cref="CspProviderFlags"/> value corresponding to string parameter.</returns>
        public static CspProviderFlags GetCspProviderFlags(string flagString)
        {
            try
            {
                CspProviderFlags val = (CspProviderFlags)Enum.Parse(typeof(CspProviderFlags), flagString);
                return val;
            }
            catch { }
            if (flagString != null)
                flagString = flagString.ToUpper();
            if (string.IsNullOrEmpty(flagString))
                return CspProviderFlags.NoFlags;
            if (flagString == "NONE" || flagString == "NOFLAGS")
                return CspProviderFlags.NoFlags;
            if (flagString == "CreateEphemeralKey".ToUpper())
                return CspProviderFlags.CreateEphemeralKey;
            if (flagString == "NoPrompt".ToUpper())
                return CspProviderFlags.NoPrompt;
            if (flagString == "UseArchivableKey".ToUpper())
                return CspProviderFlags.UseArchivableKey;
            if (flagString == "UseDefaultKeyContainer".ToUpper())
                return CspProviderFlags.UseDefaultKeyContainer;
            if (flagString == "UseExistingKey".ToUpper())
                return CspProviderFlags.UseExistingKey;
            if (flagString == "UseMachineKeyStore".ToUpper())
                return CspProviderFlags.UseMachineKeyStore;
            if (flagString == "UseNonExportableKey".ToUpper())
                return CspProviderFlags.UseNonExportableKey;
            if (flagString == "UseUserProtectedKey".ToUpper())
                return CspProviderFlags.UseUserProtectedKey;
            throw new ArgumentException("Did not recognize the following CSP flag: " + flagString);
            //return AsymAlgorithmType.None;

        }


        /// <summary>Returns standard string representation of the <see cref="CspProviderFlags"/> enumeration.</summary>
        /// <param name="flag">Asymmetric encryption algorithm type whose string representation is returned.</param>
        public static string CspProviderFlagsToString(CspProviderFlags flag)
        {
            switch (flag)
            {
                //case CspProviderFlags.NoFlags:
                //    return "NoFlags";
                default:
                    return flag.ToString();
            }
        }


        /// <summary>Creates and returns a string that presents the key information contained in the 
        /// argument <paramref name="keyInfo"/>.</summary>
        /// <param name="keyInfo">Information about the key pair that is returned in string form.</param>
        /// <param name="numIndent">Indentation.</param>
        public static string ToString(CspKeyContainerInfo keyInfo, int numIndent = 0)
        {
            StringBuilder sb = new StringBuilder();
            if (numIndent < 0)
                throw new ArgumentException("Indentation can not be less than 0.");
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Key container name:    " + keyInfo.KeyContainerName);
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Unique container name: " + keyInfo.UniqueKeyContainerName);
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("KeyNumber: " + keyInfo.KeyNumber);
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Machine key store: " + keyInfo.MachineKeyStore);
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Accessible:  " + keyInfo.Accessible);
            try
            {
                if (numIndent > 0) sb.Append(' ', numIndent);
                bool exportable = keyInfo.Exportable;
                sb.AppendLine("Exportable:  " + keyInfo.Exportable);
            }
            catch (Exception ex)
            {
                sb.AppendLine("Exportable unknown: " + ex.Message);
            }
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Protected:   " + keyInfo.Protected);
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Removable:   " + keyInfo.Removable);
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Provider name: " + keyInfo.ProviderName);
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Provider type: " + keyInfo.ProviderType);
            if (numIndent > 0) sb.Append(' ', numIndent);
            sb.AppendLine("Randomly generated: " + keyInfo.RandomlyGenerated);
            return sb.ToString();
        }

        /// <summary>Creates and returns a string containing information about the specified RSA algorithm.</summary>
        /// <param name="alg">Algorithm for which information is returned.</param>
        public static string ToString(RSACryptoServiceProvider alg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Asymmetric RSA algorithm: ");
            sb.AppendLine("  Key size: " + alg.KeySize);
            if (alg.PublicOnly)
                sb.AppendLine("  Only public key.");
            else
                sb.AppendLine("  Public and private key.");
            sb.AppendLine("  Persistent: " + alg.PersistKeyInCsp);
            sb.AppendLine("  Key exchange algorithm: " + alg.KeyExchangeAlgorithm);
            sb.AppendLine("  Signature algorithm: " + alg.SignatureAlgorithm);
            sb.AppendLine("  Legal key sizes: " + Util.ToString<KeySizes>(alg.LegalKeySizes));
            sb.AppendLine("  Public key hash:  " + GetAsymmetricalgorithmHash(alg));
            // sb.AppendLine("  Private key hashRet: " + GetAsymmetricalgorithmHash(alg,true /* includePrivate */));

            if (alg.CspKeyContainerInfo == null)
                sb.AppendLine("  There is no information about the key pair.");
            else
            {
                CspKeyContainerInfo keyInfo = alg.CspKeyContainerInfo;
                if (!alg.PersistKeyInCsp)
                {
                    Console.WriteLine(Environment.NewLine + "  Key is NOT persisted in CSP!");
                }
                sb.AppendLine("  Keys: ");
                sb.Append(UtilCrypto.ToString(keyInfo, 3));
            }
            return sb.ToString();
        }

        /// <summary>Creates and returns a string containing information about the specified DSA algorithm.</summary>
        /// <param name="alg">Algorithm for which information is returned.</param>
        public static string ToString(DSACryptoServiceProvider alg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Asymmetric DSA algorithm: ");
            sb.AppendLine("  Key size: " + alg.KeySize);
            sb.Append("  Legal sizes: " + Util.CollectionToString(alg.LegalKeySizes));
            if (alg.PublicOnly)
                sb.AppendLine("Only public key is present.");
            else
                sb.AppendLine("Public and private key.");
            sb.AppendLine("  Persistent:" + alg.PersistKeyInCsp);
            sb.AppendLine("  Key exchange algorithm: " + alg.KeyExchangeAlgorithm);
            sb.AppendLine("  Signature algorithm: " + alg.SignatureAlgorithm);
            if (alg.CspKeyContainerInfo == null)
                sb.AppendLine("  There is no information about the key pair.");
            else
            {
                CspKeyContainerInfo keyInfo = alg.CspKeyContainerInfo;
                sb.AppendLine("  Keys: ");
                sb.Append(UtilCrypto.ToString(keyInfo, 3));
            }
            return sb.ToString();
        }


        /// <summary>Creates and returns a string containing information about the specified RSA algorithm.</summary>
        /// <param name="alg">Algorithm for which information is returned.</param>
        public static string ToString(AsymmetricAlgorithm alg)
        {
            RSACryptoServiceProvider rsaCp = alg as RSACryptoServiceProvider;
            if (rsaCp != null)
                return ToString(rsaCp);
            DSACryptoServiceProvider dsaCp = alg as DSACryptoServiceProvider;
            if (dsaCp != null)
                return ToString(dsaCp);
            StringBuilder sb = new StringBuilder();
            sb.Append("Asymmetric key: ");
            sb.AppendLine("  Key size: " + alg.KeySize);
            sb.AppendLine("  Key exchange algorithm: " + alg.KeyExchangeAlgorithm);
            sb.AppendLine("  Signature algorithm: " + alg.SignatureAlgorithm);
            return sb.ToString();
        }

                
        /// <summary>Returns a readable string representation of the specified certificate.</summary>
        /// <remarks>To obtain a detailed access to certificate data, the object is first used to create
        /// a more advanced <see cref="X509Certificate2"/> type of object (which has a contructor
        /// that takes its base class) and then string representation is created from thi object,
        /// which offers better access to internal data.</remarks>
        /// <param name="cert">Certificate whose string representaion is returned.</param>
        public static string ToString(X509Certificate cert)
        {
            return ToString(new X509Certificate2(cert));
        }

        /// <summary>Returns a readable string representation of the specified certificate.</summary>
        /// <param name="cert">Certificate whose string representaion is returned.</param>
        public static string ToString(X509Certificate2 cert)
        {
            if (cert == null)
                throw new ArgumentException("Certificate not specified (null reference).");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("X509 v.3 Certificate: ");

            sb.AppendLine("  Has private key: " + cert.HasPrivateKey);
            sb.AppendLine("  Simple Name: " + cert.GetNameInfo(X509NameType.SimpleName, true));
            sb.AppendLine("  Valid: " + cert.NotBefore + " - " + cert.NotAfter);
            sb.AppendLine("  Content Type: " + X509Certificate2.GetCertContentType(cert.RawData));
            sb.AppendLine("  Version: " + cert.Version);
            sb.AppendLine("  Friendly Name: " + cert.FriendlyName);
            sb.AppendLine("  Subject: " + cert.Subject);
            sb.AppendLine("  Subject Name: " + cert.SubjectName);
            sb.AppendLine("  Serial Number: " + cert.SerialNumber);
            sb.AppendLine("  Thumbprint: " + cert.Thumbprint);
            sb.AppendLine("  Issuer: " + cert.Issuer);
            sb.AppendLine("  Issuer name: " + cert.IssuerName);
            sb.AppendLine("  Certificate Verified: " + cert.Verify());
            sb.AppendLine("  Signature Algorithm: " + cert.SignatureAlgorithm.FriendlyName);
            try
            {
                sb.AppendLine("  Private Key: " + cert.PrivateKey.ToXmlString(true));
            }
            catch (Exception ex) { sb.AppendLine("  Could not access the private key: " + ex.Message); }
            sb.AppendLine("  Public Key: " + cert.PublicKey.Key.ToXmlString(false));
            sb.AppendLine("  Certificate Archived: " + cert.Archived);
            sb.AppendLine("  Length of Raw Data: " + cert.RawData.Length);  // + Environment.NewLine + cert.ToString(true));
            // X509Certificate2UI.DisplayCertificate(cert);


            return sb.ToString();
        }


        /// <summary>Returns a hash value for the specified Asymmetric algorithm, which depends on algorithm keys.
        /// <para>The hash vlaue can be used to verify that two algorithm object used actually have the same keys.
        /// This is especially useful when encryption is performed on one machine and decryption is performed on
        /// another.</para></summary>
        /// <remarks>See also: RSAParameters", 
        /// https://msdn.microsoft.com/en-us/library/system.security.cryptography.rsaparameters%28v=vs.110%29.aspx </remarks>
        /// <param name="algorithm">Algorithm for which keys' hashRet is returned.</param>
        /// <param name="includePrivate">Whether private parameters of the asymmetric key contained in the algorithm are taken into account while hassing.
        /// <para>Default is false and it is highly recommended to use only false for this parameter.</para>
        /// <para>Usually using only public parameters should be enough, because it is very unlikely that any key
        /// pair would have the same public parameters but different private parameters.</para></param>
        /// <param name="skipPrivateIfNotExportable">If true then private parameters are not taken into 
        /// account when they are not exportable to XML, even when <paramref name="includePrivate"/> = true.
        /// If false then exception is thrown in the case that <paramref name="includePrivate"/> is true but private
        /// parameters of the keys are not exportable to XML.
        /// <para>Default is false.</para></param>
        /// <param name="hashType">Type of the hashing algorithm. Default is <see cref="HashType.SHA1"/> and it is recommended 
        /// that this is not changed.</param>
        public static string GetAsymmetricalgorithmHash(AsymmetricAlgorithm algorithm, bool includePrivate = false,
            bool skipPrivateIfNotExportable = false, HashType hashType = HashType.SHA1)
        {
            if (algorithm == null)
                throw new ArgumentException("Asymmetric algorithm not specified (null reference).");
            RSA rsa = algorithm as RSA;
            if (rsa != null)
            {
                RSAParameters rsaParams;
                if (includePrivate)
                    rsaParams = rsa.ExportParameters(true);
                else
                    rsaParams = rsa.ExportParameters(false);
                // First, hashRet the private parameters:
                byte[] hashRet = GetHashBytes(rsaParams.Modulus, hashType);
                byte[] hashExponent = GetHashBytes(rsaParams.Exponent, hashType);
                int len = hashRet.Length;
                for (int i = 0; i < len; ++i)
                    hashRet[i] ^= hashExponent[i];
                if (includePrivate)
                {
                    byte[] hashPrivateExponent = GetHashBytes(rsaParams.D, hashType);
                    for (int i = 0; i < len; ++i)
                        hashRet[i] ^= hashPrivateExponent[i];
                }
                return Util.ToHexString(hashRet);
            }
            string xmlString = null;
            if (!includePrivate)
            {
                xmlString = algorithm.ToXmlString(includePrivate);
            }
            else
            {
                try
                {
                    xmlString = algorithm.ToXmlString(includePrivate);
                }
                catch (Exception)
                {
                    xmlString = algorithm.ToXmlString(false);
                }
            }
            return UtilCrypto.GetStringHashHex(xmlString, hashType);
        }


        /// <summary>Returns the maximal size of a byte array that can be encrypted by using the specified 
        /// asymmetric algoritm.</summary>
        /// <param name="alg">Asymmetric algorithm that would be used for encryption.</param>
        /// <returns></returns>
        public static int GetMaximalAsymmetricTextLength(AsymmetricAlgorithm alg)
        {
            if (alg == null)
                return 0;
            else
            {
                int keySize = alg.KeySize;
                if (IsUsed_fOAEP)
                    return ((keySize - 384) / 8) + 7;
                else
                    return ((keySize - 384) / 8) + 37;
            }
        }


        /// <summary>Tests whether the specified asymmetric algorithm(s) correctly encrypts/decrypts the specified string.
        /// <para>Returns true if the test passes and false if not.</para>
        /// <para>The specified string must be short enough.</para></summary>
        /// <param name="testString">String to be encrypted and decrypted.
        /// <para>Must be short enough in order to be suitable for asymmetric encryption / decryption.</para>
        /// <para>If null or empty string then false is returned.</para> </param>
        /// <param name="algEnc">Asymmetric algorithm object used for encryption (and also for decryption if 
        /// the second algorithm object is not specified).
        /// <para>If null then false is returned.</para></param>
        /// <param name="algDec">Asymmetric algorithm used for decryption.
        /// <para>If null then the first algorithm object is used for both encryption and decryption.</para>
        /// <para>Possibility of having two algorithm objects makes possible to test situations when one object contains
        /// only the public key and a different object contains the private key.</para></param>
        /// <returns>True if the secified asymmetric algorithms correctly encrypt/decrypt the specified string, 
        /// false otherwise.</returns>
        public static bool AsymEncryptionDecryptionTest(string testString, AsymmetricAlgorithm algEnc, AsymmetricAlgorithm algDec = null)
        {
            string errorString = null;
            return AsymEncryptionDecryptionTest(testString, out errorString, algEnc, algDec);
        }

        /// <summary>Tests whether the specified asymmetric algorithm(s) correctly encrypts/decrypts the specified string.
        /// <para>Returns true if the test passes and false if not.</para>
        /// <para>The specified string must be short enough.</para></summary>
        /// <param name="testString">String to be encrypted and decrypted.
        /// <para>Must be short enough in order to be suitable for asymmetric encryption / decryption.</para>
        /// <para>If null or empty string then false is returned.</para> </param>
        /// <param name="errorString">String variable where error message is stored, in case that exception is thrown
        /// during encryption/decryption. If no exception is thrown then the variable attains value null.</param>
        /// <param name="algEnc">Asymmetric algorithm object used for encryption (and also for decryption if 
        /// the second algorithm object is not specified).
        /// <para>If null then false is returned.</para></param>
        /// <param name="algDec">Asymmetric algorithm used for decryption.
        /// <para>If null then the first algorithm object is used for both encryption and decryption.</para>
        /// <para>Possibility of having two algorithm objects makes possible to test situations when one object contains
        /// only the public key and a different object contains the private key.</para></param>
        /// <returns>True if the secified asymmetric algorithms correctly encrypt/decrypt the specified string, 
        /// false otherwise.</returns>
        public static bool AsymEncryptionDecryptionTest(string testString, out string errorString, AsymmetricAlgorithm algEnc, 
            AsymmetricAlgorithm algDec = null)
        {
            errorString = null;
            if (algEnc == null)
                return false;
            if (string.IsNullOrEmpty(testString))
                return false;
            bool ret = false;
            if (algDec == null)
                algDec = algEnc;  // the same algorithm obect used for encryption and decryption
            if (algEnc != null && algDec != null)
            {
                try
                {
                    string encrypted = EncryptStringAsymShort(testString, algEnc);
                    string decrypted = DecryptStringAsymShort(encrypted, algDec);
                    if (decrypted == testString)
                        ret = true;  // encryption/decryption performed correctly
                }
                catch (Exception ex)
                {
                    errorString = ex.Message;
                    return false;
                }
            }
            return ret;
        }


        /// <summary>Tests whether the specified asymmetric algorithm(s) correctly encrypts/decrypts a random byte
        /// array of the specified size.
        /// <para>Returns true if the test passes (encrypted and then decrypted bytes equal to original) and false if not.</para>
        /// <para>The specified length must me smaller and equal to the value returned by <see cref="GetMaximalAsymmetricTextLength"/>
        /// for the test to pass (beside the encryption/decryption capability and correctness of operaton).</para></summary>
        /// <param name="testString">Length of the random byte array to be encrypted and decrypted.
        /// <para>Must be short enough in order to be suitable for asymmetric encryption / decryption. Maximal length 
        /// can be obtained by the <see cref="GetMaximalAsymmetricTextLength"/> method.</para>
        /// <para>If null or empty string then false is returned.</para> </param>
        /// <param name="algEnc">Asymmetric algorithm object used for encryption (and also for decryption if 
        /// the second algorithm object is not specified).
        /// <para>If null then false is returned.</para></param>
        /// <param name="algDec">Asymmetric algorithm used for decryption.
        /// <para>If null then the first algorithm object is used for both encryption and decryption.</para>
        /// <para>Possibility of having two algorithm objects makes possible to test situations when one object contains
        /// only the public key and a different object contains the private key.</para></param>
        /// <returns>True if the secified asymmetric algorithms correctly encrypt/decrypt a random byte array
        /// of the specified length, false otherwise.</returns>
        public static bool AsymEncryptionDecryptionTest(int length, AsymmetricAlgorithm algEnc, 
            AsymmetricAlgorithm algDec = null)
        {
            string errorString;
            return AsymEncryptionDecryptionTest(length, out errorString, algEnc, algDec);
        }


        /// <summary>Tests whether the specified asymmetric algorithm(s) correctly encrypts/decrypts a random byte
        /// array of the specified size.
        /// <para>Returns true if the test passes (encrypted and then decrypted bytes equal to original) and false if not.</para>
        /// <para>The specified length must me smaller and equal to the value returned by <see cref="GetMaximalAsymmetricTextLength"/>
        /// for the test to pass (beside the encryption/decryption capability and correctness of operaton).</para></summary>
        /// <param name="testString">Length of the random byte array to be encrypted and decrypted.
        /// <para>Must be short enough in order to be suitable for asymmetric encryption / decryption. Maximal length 
        /// can be obtained by the <see cref="GetMaximalAsymmetricTextLength"/> method.</para>
        /// <para>If null or empty string then false is returned.</para> </param>
        /// <param name="errorString">String variable where error message is stored, in case that exception is thrown
        /// during encryption/decryption. If no exception is thrown then the variable attains value null.</param>
        /// <param name="algEnc">Asymmetric algorithm object used for encryption (and also for decryption if 
        /// the second algorithm object is not specified).
        /// <para>If null then false is returned.</para></param>
        /// <param name="algDec">Asymmetric algorithm used for decryption.
        /// <para>If null then the first algorithm object is used for both encryption and decryption.</para>
        /// <para>Possibility of having two algorithm objects makes possible to test situations when one object contains
        /// only the public key and a different object contains the private key.</para></param>
        /// <returns>True if the secified asymmetric algorithms correctly encrypt/decrypt a random byte array
        /// of the specified length, false otherwise.</returns>
        public static bool AsymEncryptionDecryptionTest(int length, out string errorString, AsymmetricAlgorithm algEnc, 
            AsymmetricAlgorithm algDec = null) 
        {
            errorString = null;
            if (algEnc == null)
                return false;
            if (length <= 0)
                return false;
            bool ret = false;
            if (algDec == null)
                algDec = algEnc;  // the same algorithm obect used for encryption and decryption
            if (algEnc != null && algDec != null)
            {
                try
                {
                    byte[] original = null;
                    GetRandomBytes(ref original, length);
                    byte[] encrypted = EncryptAsymShort(original, algEnc);
                    byte[] decrypted = DecryptAsymShort(encrypted, algEnc);
                    ret = Util.AreEqual(original, encrypted);
                }
                catch (Exception ex)
                {
                    errorString = ex.Message;
                    return false;
                }
            }
            return ret;

        }





        /// <summary>Creates (if necessary) and returns a new asymmetric algorithm whose keys are created or obtained 
        /// through arguments.</summary>
        /// <param name="algorithmType">Type of the asymmetric algorithm that is returned.</param>
        /// <param name="containerName">Name of the key container where the keys persist. If not specified then
        /// keys are not persistent.</param>
        /// <param name="xmlString">If not null, this string must contain properly formed XML form which 
        /// the keys are read.</param>
        /// <param name="flags">Flags of type <see cref="CspProviderFlags"/> used in crypto service parameters
        /// when algorithm object is created. If the value is <see cref="CspProviderFlags.NoFlags"/> then
        /// flags are not used.</param>
        public static AsymmetricAlgorithm GetAsymmetricAlgorithm(AsymmetricAlgorithmType algorithmType,
            string containerName = null, string xmlString = null, CspProviderFlags flags = CspProviderFlags.NoFlags)
        {
            CspParameters cspParams = null;
            // Create a new instance of CspParameters.  Pass 
            // 13 to specify a DSA container or 1 to specify 
            // an RSA container.  The default is 1.
            switch (algorithmType)
            {
                // For value of argument in CspParameters constructor, see https://msdn.microsoft.com/en-us/library/1dh4wac4%28v=vs.110%29.aspx
                // For RSA it is 1 (default), for DSA it is 13.
                case AsymmetricAlgorithmType.RSA:
                    cspParams = new CspParameters(1);
                    break;
                case AsymmetricAlgorithmType.DSA:
                    cspParams = new CspParameters(13);
                    break;
                default:
                    throw new NotImplementedException("Getting asymmetric algorithm with persisted key is not implemented for algorithm type "
                        + UtilCrypto.AsymmetricAlgorithmTypeToString(algorithmType));
            }

            // Specify the container name using the passed variable.
            if (flags != CspProviderFlags.NoFlags)
                cspParams.Flags |= flags;

            cspParams.KeyContainerName = containerName;
            AsymmetricAlgorithm algorithm = null;
            switch (algorithmType)
            {
                // Create a new instance of asymmetric algorithm to generate a new key pair. Pass the CspParameters class 
                // to persist the key in the container.  The PersistKeyInCsp property is true by  default, allowing the 
                // key to be persisted. 
                case AsymmetricAlgorithmType.RSA:
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams);
                    if (string.IsNullOrEmpty(containerName))
                        rsa.PersistKeyInCsp = false;
                    if (!string.IsNullOrEmpty(xmlString))
                        rsa.FromXmlString(xmlString);
                    algorithm = rsa;
                    break;
                case AsymmetricAlgorithmType.DSA:
                    DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(cspParams);
                    if (string.IsNullOrEmpty(containerName))
                        dsa.PersistKeyInCsp = false;
                    if (!string.IsNullOrEmpty(xmlString))
                        dsa.FromXmlString(xmlString);
                    algorithm = dsa;

                    break;
                case AsymmetricAlgorithmType.None:
                    throw new ArgumentException("Asymmetric algorithm type is not specified.");
                //break;
                default:
                    throw new NotImplementedException("Getting asymmetric algorithm is not implemented for algorithm type "
                        + UtilCrypto.AsymmetricAlgorithmTypeToString(algorithmType));
            }
            return algorithm;
        }



        #endregion AsymmetricEncryption.General



        /// <summary>Deletes the asymmetric algorithm keys from the specified key container.</summary>
        /// <param name="flag">Type of the asymmetric algorithm for which the keys are deleted.</param>
        /// <param name="containerName">Name of the container from which keys are deleted.</param>
        public static void DeleteKeyInCsp(AsymmetricAlgorithmType algorithmType, string containerName)
        {

            CspParameters cspParams = null;
            switch (algorithmType)
            {
                // For value of argument in CspParameters constructor, see https://msdn.microsoft.com/en-us/library/1dh4wac4%28v=vs.110%29.aspx
                // For RSA it is 1 (default), for DSA it is 13.
                case AsymmetricAlgorithmType.RSA:
                    // Create new CSP (crypto service provider) parameters:
                    cspParams = new CspParameters(1);
                    // Specify the container name.
                    cspParams.KeyContainerName = containerName;
                    {
                        // Create the algorithm object with these parameters:
                        RSACryptoServiceProvider alg = new RSACryptoServiceProvider(cspParams);
                        //Explicitly set the PersistKeyInCsp property to false to delete the key entry in the container.
                        alg.PersistKeyInCsp = false;
                        //Call Clear to release resources and delete the key from the container.
                        alg.Clear();
                    }
                    //Indicate that the key was persisted.
                    Console.WriteLine("The RSA key was deleted from the container \"{0}\".", containerName);
                    break;
                case AsymmetricAlgorithmType.DSA:
                    // Create new CSP (crypto service provider) parameters:
                    cspParams = new CspParameters(1);
                    // Specify the container name.
                    cspParams.KeyContainerName = containerName;
                    {
                        // Create the algorithm object with these parameters:
                        DSACryptoServiceProvider alg = new DSACryptoServiceProvider(cspParams);
                        //Explicitly set the PersistKeyInCsp property to false to delete the key entry in the container.
                        alg.PersistKeyInCsp = false;
                        //Call Clear to release resources and delete the key from the container.
                        alg.Clear();
                    }
                    //Indicate that the key was persisted.
                    Console.WriteLine("The RSA key was deleted from the container \"{0}\".", containerName);
                    break;
                default:
                    throw new NotImplementedException("Deleting keys from container is not implemented for asymmetric algorithm type: "
                        + UtilCrypto.AsymmetricAlgorithmTypeToString(algorithmType));
            }
        }


        private static bool _isUsed_fOAEP1 = false;

        /// <summary>Whether to perform direct RSA encryption using OAEP (optimal asymmetric encryption padding - 
        /// only available on computers running MS Windows XP or later).</summary>
        /// <remarks>To do: <para>Check whether this works in Mono.</para></remarks>
        public static bool IsUsed_fOAEP
        { get { return _isUsed_fOAEP1; }  }




        public static void PrepareAymmetricAllgorithmBasic(AsymmetricAlgorithmType algorithmType,
            ref AsymmetricAlgorithm algorithm, string containerName = null, string xmlString = null, 
            CspProviderFlags flags = CspProviderFlags.NoFlags, bool useLargestKey = false)
        {
            if (algorithm == null)
            {
                algorithm = GetAsymmetricAlgorithm(algorithmType, containerName, xmlString, flags);
            }
            else
            {
                if (algorithmType != AsymmetricAlgorithmType.None)
                {
                    if (!IsCorrectAsymmetricEncryptionAlgorithm(algorithm, algorithmType))
                        throw new ArgumentException("Provided asymmetric algorithm is not of correct type: " + Environment.NewLine
                            + "  " + UtilCrypto.AsymmetricAlgorithmTypeToString(GetAsymmetricAlgorithmType(algorithm))
                            + ", should be: " + UtilCrypto.AsymmetricAlgorithmTypeToString(algorithmType) + ".");
                }
            }
            if (algorithm == null)
                throw new InvalidOperationException("Could not obtain the asymmetric algorithm object.");

        }



       // [Obsolete("Plain Asymmetric algorithms should not be used to encrypt/decrypt files.")]
       // public static void EncryptFileAsymShort(string inputFilePath, string outputFilePath, AsymmetricAlgorithm algorithm,
       //     AsymmetricAlgorithmType algorithmType = AsymmetricAlgorithmType.None)
       // {
       //     if (string.IsNullOrEmpty(inputFilePath))
       //         throw new ArgumentException("Input file path not specified (null or empty string)");
       //      if (string.IsNullOrEmpty(outputFilePath))
       //         throw new ArgumentException("Output file path not specified (null or empty string)");
       //      if (!File.Exists(inputFilePath))
       //          throw new ArgumentException("Input file does not exist: " + inputFilePath);
       //      byte[] inputBytes = File.ReadAllBytes(inputFilePath);
       //      byte[] outputBytes = EncryptAsymShort(inputBytes, algorithm, algorithmType);
       //      File.WriteAllBytes(outputFilePath, outputBytes);
       //}

       // [Obsolete("Plain Asymmetric algorithms should not be used to encrypt/decrypt files.")]
       // public static void DecryptFileAsymShort(string inputFilePath, string outputFilePath, AsymmetricAlgorithm algorithm,
       //     AsymmetricAlgorithmType algorithmType = AsymmetricAlgorithmType.None)
       // {
       //     if (string.IsNullOrEmpty(inputFilePath))
       //         throw new ArgumentException("Input file path not specified (null or empty string)");
       //     if (string.IsNullOrEmpty(outputFilePath))
       //         throw new ArgumentException("Output file path not specified (null or empty string)");
       //     if (!File.Exists(inputFilePath))
       //         throw new ArgumentException("Input file does not exist: " + inputFilePath);
       //     byte[] inputBytes = File.ReadAllBytes(inputFilePath);
       //     byte[] outputBytes = DecryptAsymShort(inputBytes, algorithm, algorithmType);
       //     File.WriteAllBytes(outputFilePath, outputBytes);
       // }


        public static string EncryptStringAsymShort(string stringToEncrypt, AsymmetricAlgorithm algorithm,
            AsymmetricAlgorithmType algorithmType = AsymmetricAlgorithmType.None)
        {
            return Convert.ToBase64String(
                EncryptAsymShort(StringEncoding.GetBytes(stringToEncrypt), algorithm, algorithmType)  );
        }

        public static string DecryptStringAsymShort(string cipherText, AsymmetricAlgorithm algorithm,
            AsymmetricAlgorithmType algorithmType = AsymmetricAlgorithmType.None)
        {
            return StringEncoding.GetString(
                DecryptAsymShort(Convert.FromBase64String(cipherText) , algorithm, algorithmType) );
        }


        public static byte[] EncryptAsymShort(byte[] originalBytes, AsymmetricAlgorithm algorithm,
            AsymmetricAlgorithmType algorithmType = AsymmetricAlgorithmType.None)
        {
            byte[] ret = null;
            if (algorithm == null)
                throw new ArgumentException("Asymmetric algorithm is not specified (null reference).");
            if (algorithmType == AsymmetricAlgorithmType.None)
            {
                algorithmType = GetAsymmetricAlgorithmType(algorithm);
                if (algorithmType == AsymmetricAlgorithmType.None)
                    throw new InvalidOperationException("Can not specify the type of the asymmetric algorithm object.");
            }
            switch (algorithmType)
            {
                case AsymmetricAlgorithmType.RSA:
                    {
                        RSACryptoServiceProvider alg = algorithm as RSACryptoServiceProvider;
                        if (alg != null)
                            ret = alg.Encrypt(originalBytes, IsUsed_fOAEP);
                        break;
                    }
                //case AsymAlgorithmType.DSA:
                //    {
                //        DSACryptoServiceProvider alg = algorithm as DSACryptoServiceProvider;
                //        if (alg != null)
                //            retInternal = alg.Encrypt(originalBytes, IsUsed_fOAEP);
                //        break;
                //    }
                default:
                    throw new NotImplementedException("Asymmetric encryption not implemented for algorithm of type "
                        + AsymmetricAlgorithmTypeToString(algorithmType));
            }
            if (ret == null)
                throw new InvalidOperationException("Could not perform encryption by using an asymmetric algorithm of type "
                        + AsymmetricAlgorithmTypeToString(algorithmType));
            return ret;
        }


        public static byte[] DecryptAsymShort(byte[] originalBytes, AsymmetricAlgorithm algorithm,
            AsymmetricAlgorithmType algorithmType = AsymmetricAlgorithmType.None)
        {
            byte[] ret = null;
            if (algorithm == null)
                throw new ArgumentException("Asymmetric algorithm is not specified (null reference).");
            if (algorithmType == AsymmetricAlgorithmType.None)
            {
                algorithmType = GetAsymmetricAlgorithmType(algorithm);
                if (algorithmType == AsymmetricAlgorithmType.None)
                    throw new InvalidOperationException("Can not specify the type of the asymmetric algorithm object.");
            }
            switch (algorithmType)
            {
                case AsymmetricAlgorithmType.RSA:
                    {
                        RSACryptoServiceProvider alg = algorithm as RSACryptoServiceProvider;
                        if (alg != null)
                        {
                            ret = alg.Decrypt(originalBytes, IsUsed_fOAEP);
                        }
                    }
                    break;
                //case AsymAlgorithmType.DSA:
                //    {
                //        DSACryptoServiceProvider alg = algorithm as DSACryptoServiceProvider;
                //        if (alg != null)
                //            retInternal = alg.Encrypt(originalBytes, IsUsed_fOAEP);
                //        break;
                //    }
                default:
                    throw new NotImplementedException("Asymmetric decryption not implemented for algorithm of type "
                        + AsymmetricAlgorithmTypeToString(algorithmType));
            }
            if (ret == null)
                throw new InvalidOperationException("Could not perform decryption by using an asymmetric algorithm of type "
                        + AsymmetricAlgorithmTypeToString(algorithmType));
            return ret;
        }



        public static byte[] EncryptAsymShort(byte[] originalBytes, AsymmetricAlgorithmType algorithmType = AsymmetricAlgorithmType.None, 
            AsymmetricAlgorithm algorithm = null, string containerName = null, string xmlString = null, 
            CspProviderFlags flags = CspProviderFlags.NoFlags, bool useLargestKey = false)
        {
            byte[] ret = null;
            PrepareAymmetricAllgorithmBasic(algorithmType, ref algorithm, containerName, xmlString, flags, useLargestKey);
            if (algorithmType == AsymmetricAlgorithmType.None)
                algorithmType = GetAsymmetricAlgorithmType(algorithm);
            ret = EncryptAsymShort(originalBytes, algorithm, algorithmType);
            return ret;
        }


        public static byte[] DecryptAsymShort(byte[] originalBytes, AsymmetricAlgorithmType algorithmType = AsymmetricAlgorithmType.None, 
            AsymmetricAlgorithm algorithm = null, string containerName = null, string xmlString = null, 
            CspProviderFlags flags = CspProviderFlags.NoFlags, bool useLargestKey = false)
        {
            byte[] ret = null;
            PrepareAymmetricAllgorithmBasic(algorithmType, ref algorithm, containerName, xmlString, flags, useLargestKey);
            if (algorithmType == AsymmetricAlgorithmType.None)
                algorithmType = GetAsymmetricAlgorithmType(algorithm);
            ret = DecryptAsymShort(originalBytes, algorithm, algorithmType);
            return ret;
        }





        #endregion AsymmetricEncryption



        #region IGLibCrypto

        // In development. This contains special cryptographic operations for IGLib, i.e. everything that are not standard procedures.



        #region IGLibCrypto.Constants

        /// <summary>An array of standard IGLib salt suffices.</summary>
        /// <remarks>
        /// <para>Warnings: </para>
        /// <para>  - Hard coding salts is generally not OK, it is important that salts are random and salts can be 
        /// stored together with enxrypted data. </para>
        /// <para>  - This array should not be modifiable.</para>
        /// <para>  - Elements of this array may not change over time. Only new elements can be added.</para>
        /// </remarks>
        private static string[] _igLibLongSaltSuffices = new string[] {
            // IGLib salt string No. 0:
            "0586297593_hc94kglh9kfjhtgk04vnbtheli9f84_IGLib_by_Igor_Gresovnik_k8ktkg9mgrn48vg94f9bkjd8_79384659674",
            // IGLib salt string No. 1:
            "74073659573_l9hfnci85vj7fl58f799aj5kla8s9_IGLib_by_Igor_Gresovnik_b098jdk87jflg274kglmng02_73608109847"
        };

        /// <summary>Returns the specified standard IGLib salt string.</summary>
        /// <remarks><para>There are several uses of salt strings:</para>
        /// <para>  - they are appended or prepended to passwords before hashing.</para>
        /// </remarks>
        /// <param name="whichSalt">Index of the standard IGLib salt string.</param>
        /// <returns>The standard IGLib salt string corresponding to the specified index.</returns>
        public static string IgGetSaltSuffix(int whichSalt)
        {
            if (whichSalt >= _igLibLongSaltSuffices.Length || whichSalt < 0)
                throw new IndexOutOfRangeException("Index of IGLib standard salt is out of range.");
            else return _igLibLongSaltSuffices[whichSalt];
        }

        /// <summary>Returns the number of standard IGLib salt strings.</summary>
        /// <returns>The number of standard IGLib salt strings.</returns>
        /// <seealso cref="GetIGLibSalt"/>
        public static int IgGetNumSaltSuffices()
        { return _igLibLongSaltSuffices.Length; }




        #endregion IGLibCrypto.Constants


        #endregion IGLibCrypto






        #region QuickTest


        //#region QuickTest.GenerateSelfSignedCertificate

        //// Generation of Root CA certificate and then a self-signed certificate: http://stackoverflow.com/questions/22230745/generate-self-signed-certificate-on-the-fly 

        //private void button_Click(object sender, EventArgs e)
        //{
        //    AsymmetricKeyParameter myCAprivateKey = null;
        //    //generate a root CA cert and obtain the privateKey
        //    X509Certificate2 MyRootCAcert = GenerateCACertificate("CN=MYTESTCA", ref myCAprivateKey);
        //    //add CA cert to store
        //    addCertToStore(MyRootCAcert, StoreName.Root, StoreLocation.LocalMachine);

        //    //generate cert based on the CA cert privateKey
        //    X509Certificate2 MyCert = GenerateSelfSignedCertificate("CN=127.0.01", "CN=MYTESTCA", myCAprivateKey);
        //    //add cert to store
        //    addCertToStore(MyCert, StoreName.My, StoreLocation.LocalMachine);

        //    MessageBox.Show("Done!");
        //}


        //public static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, string issuerName, AsymmetricKeyParameter issuerPrivKey)
        //{
        //    const int keyStrength = 2048;

        //    // Generating Random Numbers
        //    CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
        //    SecureRandom random = new SecureRandom(randomGenerator);

        //    // The Certificate Generator
        //    X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();

        //    // Serial Number
        //    BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
        //    certificateGenerator.SetSerialNumber(serialNumber);

        //    // Signature Algorithm
        //    const string signatureAlgorithm = "SHA256WithRSA";
        //    certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

        //    // Issuer and Subject Name
        //    X509Name subjectDN = new X509Name(subjectName);
        //    X509Name issuerDN = new X509Name(issuerName);
        //    certificateGenerator.SetIssuerDN(issuerDN);
        //    certificateGenerator.SetSubjectDN(subjectDN);

        //    // Valid For
        //    DateTime notBefore = DateTime.UtcNow.Date;
        //    DateTime notAfter = notBefore.AddYears(2);

        //    certificateGenerator.SetNotBefore(notBefore);
        //    certificateGenerator.SetNotAfter(notAfter);

        //    // Subject Public Key
        //    AsymmetricCipherKeyPair subjectKeyPair;
        //    var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
        //    var keyPairGenerator = new RsaKeyPairGenerator();
        //    keyPairGenerator.Init(keyGenerationParameters);
        //    subjectKeyPair = keyPairGenerator.GenerateKeyPair();

        //    certificateGenerator.SetPublicKey(subjectKeyPair.Public);

        //    // Generating the Certificate
        //    AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;

        //    // selfsign certificate
        //    Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(issuerPrivKey, random);

        //    // correcponding private key
        //    PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);


        //    // merge into X509Certificate2
        //    X509Certificate2 x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.GetEncoded());

        //    Asn1Sequence seq = (Asn1Sequence)Asn1Object.FromByteArray(info.PrivateKey.GetDerEncoded());
        //    if (seq.Count != 9)
        //    {
        //        //throw new PemException("malformed sequence in RSA private key");
        //    }

        //    RsaPrivateKeyStructure rsa = new RsaPrivateKeyStructure(seq);
        //    RsaPrivateCrtKeyParameters rsaparams = new RsaPrivateCrtKeyParameters(
        //        rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2, rsa.Coefficient);

        //    x509.PrivateKey = DotNetUtilities.ToRSA(rsaparams);
        //    return x509;

        //}

        //public static X509Certificate2 GenerateCACertificate(string subjectName, ref AsymmetricKeyParameter CaPrivateKey)
        //{
        //    const int keyStrength = 2048;

        //    // Generating Random Numbers
        //    CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
        //    SecureRandom random = new SecureRandom(randomGenerator);

        //    // The Certificate Generator
        //    X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();

        //    // Serial Number
        //    BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
        //    certificateGenerator.SetSerialNumber(serialNumber);

        //    // Signature Algorithm
        //    const string signatureAlgorithm = "SHA256WithRSA";
        //    certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

        //    // Issuer and Subject Name
        //    X509Name subjectDN = new X509Name(subjectName);
        //    X509Name issuerDN = subjectDN;
        //    certificateGenerator.SetIssuerDN(issuerDN);
        //    certificateGenerator.SetSubjectDN(subjectDN);

        //    // Valid For
        //    DateTime notBefore = DateTime.UtcNow.Date;
        //    DateTime notAfter = notBefore.AddYears(2);

        //    certificateGenerator.SetNotBefore(notBefore);
        //    certificateGenerator.SetNotAfter(notAfter);

        //    // Subject Public Key
        //    AsymmetricCipherKeyPair subjectKeyPair;
        //    KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
        //    RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
        //    keyPairGenerator.Init(keyGenerationParameters);
        //    subjectKeyPair = keyPairGenerator.GenerateKeyPair();

        //    certificateGenerator.SetPublicKey(subjectKeyPair.Public);

        //    // Generating the Certificate
        //    AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;

        //    // selfsign certificate
        //    Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(issuerKeyPair.Private, random);
        //    X509Certificate2 x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.GetEncoded());

        //    CaPrivateKey = issuerKeyPair.Private;

        //    return x509;
        //    //return issuerKeyPair.Private;

        //}

        //public static bool addCertToStore(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, System.Security.Cryptography.X509Certificates.StoreName st, System.Security.Cryptography.X509Certificates.StoreLocation sl)
        //{
        //    bool bRet = false;

        //    try
        //    {
        //        X509Store store = new X509Store(st, sl);
        //        store.Open(OpenFlags.ReadWrite);
        //        store.Add(cert);

        //        store.Close();
        //    }
        //    catch
        //    {

        //    }

        //    return bRet;
        //}


        //#endregion QuickTest.GenerateSelfSignedCertificate


        #endregion QuickTest


















    }  // class UtilCrypto



}


