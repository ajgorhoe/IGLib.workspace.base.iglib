// Copyright (c) Igor Grešovnik (2009 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Security.Cryptography;

namespace IG.Crypto
{

    
    /// <summary>Enumerator for selecting the hashing algorithm.</summary>
    [Flags]
    public enum HashType : int
    {
        /// <summary>No hashing algorithm specified.</summary>
        None = 0,
        /// <summary>Specifies the MD5 hashing algorithm.</summary>
        MD5 = 1,
        /// <summary>Specifies the SHA1 hashing algorithm.</summary>
        SHA1 = 2,
        /// <summary>Specifies the SHA256 hashing algorithm.</summary>
        SHA256 = 4,
        /// <summary>Specifies the SHA512 hashing algorithm.</summary>
        SHA512 = 8
    }
 

    /// <summary>Basic Cryptographic utilities.</summary>
    /// $A Igor Apr10;
    public static class UtilCrypto
    {


        /// <summary>Contains constants for cryptographic operations from shell functions.</summary>
        public static class ConstCrypto
        {

            /// <summary>File extension for files containing the calculated hash codes.</summary>
            public const string HashFileExtension = ".chk";

            /// <summary>Default hash type.</summary>
            public const HashType DefaultHashType = HashType.MD5;

            /// <summary>Hash is verified rather than created.</summary>
            public const string ArgCheck = "-c";
            /// <summary>Hash is verified rather than created.</summary>
            public const string ArgCheck1 = "/c";

            /// <summary>Specification of hash type follows.</summary>
            public const string ArgHashType = "-t";
            /// <summary>Specification of hash type follows.</summary>
            public const string ArgHashType1 = "/t";

            /// <summary>Specification of output file follows.</summary>
            public const string ArgOutputFile = "-o";
            /// <summary>Specification of output file follows.</summary>
            public const string ArgOutputFile1 = "/o";

            /// <summary>Specification of string whose hash is calculated follows.</summary>
            public const string ArgString = "-s";
            /// <summary>Specification of string whose hash is calculated follows.</summary>
            public const string ArgString1 = "/s";

            /// <summary>Specification of hash value follows (only with -c, for a single input file).</summary>
            public const string ArgHashValue = "-h";
            /// <summary>Specification of hash value follows (only with -c, for a single input file).</summary>
            public const string ArgHashValue1 = "/h";

        }




        #region HashGeneral


        /// <summary>Returns length of the hash value, in bytes, for the specified hash algorithm.
        /// <para>-1 is returned if the length is not known.</para></summary>
        /// <param name="hashType">Type of the hashing algorithm.</param>
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

        /// <summary>Returns length of the HEXADECIMAL hash string for the specified hash algorithm.
        /// <para>-1 is returned if the length is not known.</para></summary>
        /// <param name="hashType">Type of the hashing algorithm.</param>
        public static int GetHashLengthHex(HashType hashType)
        {
            int length = GetHashLengthBytes(hashType);
            if (length <= 0)
                return length;
            else
                return 2 * length;
        }

        /// <summary>Returns the appropriate hash algorithm according to the specified hash type, or null 
        /// if the type is not recognized or the method is not implemented for that type.</summary>
        /// <param name="hashType">Specification of the hash algorthm type.</param>
        public static HashAlgorithm CreateHashAlgorithm(HashType hashType)
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


        public static HashType GetHashType(string typeString)
        {
            typeString = typeString.ToUpper();
            try
            {
                HashType type = (HashType)Enum.Parse(typeof(HashType), typeString);
                return type;
            }
            catch { }
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
            return HashType.None;
        }
        

        /// <summary>Returns standard string representation of the specified hash type.
        /// <para>Does the same as <see cref="HashTypeToString"/>.</para></summary>
        /// <param name="hashType">Hash type whose string representation is returned.</param>
        public static string GetHashType(HashType hashType)
        {
            return HashTypeToString(hashType);
        }


        /// <summary>Returns standard string representation of the specified hash type.</summary>
        /// <param name="hashType">Hash type whose string representation is returned.</param>
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


        /// <summary>Parses the file containing hash values of one or more files, and adds the parsed
        /// pairs {hash, filePath} to the specified list.
        /// <para>File must be in the standard format where each line contains a hash value and the path to 
        /// the corresponding file separated from hash value by one or more spaces.</para>
        /// <para>List is allocated if necessary. Eventual existent pairs on the list are not affected.</para></summary>
        /// <param name="filePath">Path to the file that is parsed.</param>
        /// <param name="hashList">List to which which parsed pairs {hash, filePath} are added in form of arrays of 2 strings.</param>
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
                    // Find the end of the hash string:
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


        /// <summary>Parses the string containing hash values of one or more files, and adds the parsed
        /// pairs {hash, filePath} to the specified list.
        /// <para>String must be in the standard format where each line contains a hash value and the path to 
        /// the corresponding file separated from hash value by one or more spaces.</para>
        /// <para>List is allocated if necessary. Eventual existent pairs on the list are not affected.</para></summary>
        /// <param name="str">String that is parsed.</param>
        /// <param name="hashList">List to which which parsed pairs {hash, filePath} are added in form of arrays of 2 strings.</param>
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
                    // Find the end of the hash string:
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

        #endregion HashGeneral


        #region HashString

        /// <summary>Computes and returns the hash string of the specified kind of the specified string.
        /// <para>null is returned if the specified hash type is not known or implemented.</para></summary>
        /// <param name="stringToHash">String whose hash string is calculated.</param>
        /// <param name="hashType">Specifies the type of the hashing algorithm to be used.</param>
        public static string GetStringHash(string stringToHash, HashType hashType)
        {
            HashAlgorithm crypt = CreateHashAlgorithm(hashType);
            if (crypt == null)
                return null;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(stringToHash), 0, Encoding.UTF8.GetByteCount(stringToHash));
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>Computes and returns the SHA1 hash string of the specified string.</summary>
        /// <param name="stringToHash">String whose cryptographic hash is calculated.</param>
        public static string GetStringHashMd5(string stringToHash)
        {
            HashAlgorithm crypt = new MD5CryptoServiceProvider();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(stringToHash), 0, Encoding.UTF8.GetByteCount(stringToHash));

            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>Computes and returns the SHA1 hash string of the specified string.</summary>
        /// <param name="stringToHash">String whose cryptographic hash is calculated.</param>
        public static string GetStringHashSha1(string stringToHash)
        {
            HashAlgorithm crypt = new SHA1Managed();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(stringToHash), 0, Encoding.UTF8.GetByteCount(stringToHash));
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>Computes and returns the SHA256 hash string of the specified string.</summary>
        /// <param name="stringToHash">String whose cryptographic hash is calculated.</param>
        public static string GetStringHashSha256(string stringToHash)
        {
            HashAlgorithm crypt = new SHA256Managed();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(stringToHash), 0, Encoding.UTF8.GetByteCount(stringToHash));
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>Computes and returns the SHA512 hash string of the specified string.</summary>
        /// <param name="stringToHash">String whose cryptographic hash is calculated.</param>
        public static string GetStringHashSha512(string stringToHash)
        {
            HashAlgorithm crypt = new SHA512Managed();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(stringToHash), 0, Encoding.UTF8.GetByteCount(stringToHash));
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        #endregion HashString


        #region CeckHashString

        /// <summary>Chechs the specified type of hash value of a string.
        /// <para>Returns true if the hash value matches the hash value of the string, and false otherwise.</para>
        /// </summary>
        /// <param name="stringToCheck">String whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose correctness is checked.</param>
        /// <param name="hashType">Type of the hash value that is checked.</param>
        /// <returns>True if the specified hash <paramref name="hashValue"/> actually matches the hash value
        /// of the type <paramref name="hashType"/> of the verified string <paramref name="stringToCheck"/>, or
        /// false otherwise.</returns>
        public static bool CheckStringHash(string stringToCheck, string hashValue, HashType hashType)
        {
            if (string.IsNullOrEmpty(hashValue))
                return false;
            hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetStringHash(stringToCheck, hashType);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }

        /// <summary>Chechs all supported types of hash value of a string.
        /// <para>Returns the hash type if the hash value matches the hash value of that type of the specified string, 
        /// or <see cref="HashType.None"/> if the specified hash value doesn't match the hash value of any
        /// supported type of the string.</para>
        /// </summary>
        /// <param name="stringToCheck">String whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose match with the specified string is checked.</param>
        /// <returns>Type of the hash value that matches the specified hash value of the specified string, 
        /// or <see cref="HashType.None"/> if the specified hash value doesn't match the hash value of any
        /// supported type of the specified string.</returns>
        public static HashType CheckStringHashSupportedTypes(string stringToCheck, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue))
                return HashType.None;
            var values = Enum.GetValues(typeof(HashType)).Cast<HashType>();
            hashValue = hashValue.Trim();
            int hashLength = hashValue.Length;
            if (hashLength < 1)
                return HashType.None;
            foreach (HashType hashType in values)
            {
                int expectedLength = GetHashLengthHex(hashType);
                if (!(expectedLength > 0 && expectedLength != hashLength)) // don't check hash type if length does not match
                {
                    if (CheckStringHash(stringToCheck, hashValue, hashType))
                        return hashType;
                }
            }
            return HashType.None;
        }

        /// <summary>Checks whether the specified MD5 hash value matches the actual hash value
        /// of the specified string.
        /// <para>Returns true if the specified hash value matches the actual hash value of 
        /// the string, and false otherwise.</para></summary>
        /// <param name="stringToCheck">String whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose validity is checked for the specified string.</param>
        /// <returns>True if the specified hash value matches the actual value, false otherwise.</returns>
        public static bool CheckStringHashMd5(string stringToCheck, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue))
                return false;
            hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetStringHashMd5(stringToCheck);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }

        /// <summary>Checks whether the specified SHA-1 hash value matches the actual hash value
        /// of the specified string.
        /// <para>Returns true if the specified hash value matches the actual hash value of 
        /// the string, and false otherwise.</para></summary>
        /// <param name="stringToCheck">String whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose validity is checked for the specified string.</param>
        /// <returns>True if the specified hash value matches the actual value, false otherwise.</returns>
        public static bool CheckStringHashSha1(string stringToCheck, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue))
                return false;
            hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetStringHashSha1(stringToCheck);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }

        /// <summary>Checks whether the specified SHA-256 hash value matches the actual hash value
        /// of the specified string.
        /// <para>Returns true if the specified hash value matches the actual hash value of 
        /// the string, and false otherwise.</para></summary>
        /// <param name="stringToCheck">String whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose validity is checked for the specified string.</param>
        /// <returns>True if the specified hash value matches the actual value, false otherwise.</returns>
        public static bool CheckStringHashSha256(string stringToCheck, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue))
                return false;
            hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetStringHashSha256(stringToCheck);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }

        /// <summary>Checks whether the specified SHA-512 hash value matches the actual hash value
        /// of the specified string.
        /// <para>Returns true if the specified hash value matches the actual hash value of 
        /// the string, and false otherwise.</para></summary>
        /// <param name="stringToCheck">String whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose validity is checked for the specified string.</param>
        /// <returns>True if the specified hash value matches the actual value, false otherwise.</returns>
        public static bool CheckStringHashSha512(string stringToCheck, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue))
                return false;
            hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetStringHashSha512(stringToCheck);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }


        #endregion CheckHashString



        #region HashFile

        
        /// <summary>Computes and returns the hash string of specified type of the specified stream.</summary>
        /// <param name="filePath">Path to the file whose contents' cryptographic hash is calculated.</param>
        /// <param name="hashType">Specifies the type of the hashing algorithm to be used.</param>
        public static string GetFileHash(string filePath, HashType hashType)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHash(stream, hashType);
            }
            return hashString;
        }

        /// <summary>Computes and returns the MD5 hash string of the specified stream.</summary>
        /// <param name="filePath">Path to the file whose contents' cryptographic hash is calculated.</param>
        public static string GetFileHashMd5(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashMd5(stream);
            }
            return hashString;
        }

        /// <summary>Computes and returns the SHA1 hash string of the specified stream.</summary>
        /// <param name="filePath">Path to the file whose contents' cryptographic hash is calculated.</param>
        public static string GetFileHashSha1(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashSha1(stream);
            }
            return hashString;
        }

        /// <summary>Computes and returns the SHA256 hash string of the specified stream.</summary>
        /// <param name="filePath">Path to the file whose contents' cryptographic hash is calculated.</param>
        public static string GetFileHashSha256(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashSha256(stream);
            }
            return hashString;
        }

        /// <summary>Computes and returns the SHA512 hash string of the specified stream.</summary>
        /// <param name="filePath">Path to the file whose contents' cryptographic hash is calculated.</param>
        public static string GetFileHashSha512(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            string hashString = null;
            using (FileStream stream = File.OpenRead(filePath))
            {
                hashString = GetHashSha512(stream);
            }
            return hashString;
        }



        /// <summary>Computes and returns the hash string of specified type of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hash is calculated.</param>
        public static string GetHash(Stream stream, HashType hashType)
        {
            if (stream == null)
                return null;
            HashAlgorithm crypt = CreateHashAlgorithm(hashType);
            if (crypt == null)
                return null;
            byte[] crypto = crypt.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>Computes and returns the MD5 hash string of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hash is calculated.</param>
        public static string GetHashMd5(Stream stream)
        {
            if (stream == null)
                return null;
            HashAlgorithm crypt = new MD5CryptoServiceProvider();
            byte[] crypto = crypt.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>Computes and returns the SHA1 hash string of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hash is calculated.</param>
        public static string GetHashSha1(Stream stream)
        {
            if (stream == null)
                return null;
            HashAlgorithm crypt = new SHA1Managed();
            byte[] crypto = crypt.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>Computes and returns the SHA256 hash string of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hash is calculated.</param>
        public static string GetHashSha256(Stream stream)
        {
            if (stream == null)
                return null;
            HashAlgorithm crypt = new SHA256Managed();
            byte[] crypto = crypt.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        /// <summary>Computes and returns the SHA512 hash string of the specified stream.</summary>
        /// <param name="stream">Stream whose contents' cryptographic hash is calculated.</param>
        public static string GetHashSha512(Stream stream)
        {
            if (stream == null)
                return null;
            HashAlgorithm crypt = new SHA512Managed();
            byte[] crypto = crypt.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte bit in crypto)
            {
                sb.Append(bit.ToString("x2"));
            }
            return sb.ToString();
        }


        #endregion HashFile



        #region CeckHashFile

        /// <summary>Chechs the specified type of hash value of a file.
        /// <para>Returns true if the specified hash value matches the hash value of the file, and false otherwise.</para>
        /// </summary>
        /// <param name="filePath">Path to the file whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose correctness is checked.</param>
        /// <param name="hashType">Type of the hash value that is checked.</param>
        /// <returns>True if the specified hash <paramref name="hashValue"/> actually matches the hash value
        /// of the type <paramref name="hashType"/> of the verified file <paramref name="filePath"/>, or
        /// false otherwise.</returns>
        public static bool CheckFileHash(string filePath, string hashValue, HashType hashType)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(hashValue))
                return false;
            else
            {
                string actualHash = GetFileHash(filePath, hashType);
                if (actualHash != null)
                    actualHash = actualHash.ToUpper();
                return hashValue.Trim().ToUpper() == actualHash;
            }
        }


        /// <summary>Chechs all supported types of hash value of a file.
        /// <para>Returns the hash type if the hash value matches the hash value of that type of the specified file, 
        /// or <see cref="HashType.None"/> if the specified hash value doesn't match the hash value of any
        /// supported type of the specified file.</para>
        /// </summary>
        /// <param name="filePath">Path to the file whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose match with the specified file is checked.</param>
        /// <returns>Type of the hash value that matches the specified hash value of the specified file, 
        /// or <see cref="HashType.None"/> if the specified hash value doesn't match the hash value of any
        /// supported type of the specified file.</returns>
        public static HashType CheckFileHashSupportedTypes(string filePath, string hashValue)
        {
            if (string.IsNullOrEmpty(hashValue) || string.IsNullOrEmpty(filePath))
                return HashType.None;
            var values = Enum.GetValues(typeof(HashType)).Cast<HashType>();
            hashValue = hashValue.Trim();
            int hashLength = hashValue.Length;
            if (hashLength < 1)
                return HashType.None;
            foreach (HashType hashType in values)
            {
                int expectedLength = GetHashLengthHex(hashType);
                if (!(expectedLength > 0 && expectedLength != hashLength)) // don't check hash type if length does not match
                {
                    if (CheckFileHash(filePath, hashValue, hashType))
                        return hashType;
                }
            }
            return HashType.None;
        }

        /// <summary>Checks whether the specified MD5 hash value matches the actual hash value
        /// of the specified file.
        /// <para>Returns true if the specified hash value matches the actual hash value of 
        /// the file, and false otherwise.</para></summary>
        /// <param name="filePath">Path to the file whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose validity is checked for the specified file.</param>
        /// <returns>True if the specified file hash value matches the actual value, false otherwise.</returns>
        public static bool CheckFileHashMd5(string filePath, string hashValue)
        {
            if (hashValue != null)
                hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetFileHashMd5(filePath);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }

        /// <summary>Checks whether the specified SHA-1 hash value matches the actual hash value
        /// of the specified file.
        /// <para>Returns true if the specified hash value matches the actual hash value of 
        /// the file, and false otherwise.</para></summary>
        /// <param name="filePath">Path to the file whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose validity is checked for the specified file.</param>
        /// <returns>True if the specified file hash value matches the actual value, false otherwise.</returns>
        public static bool CheckFileHashSha1(string filePath, string hashValue)
        {
            if (hashValue != null)
                hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetFileHashSha1(filePath);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }

        /// <summary>Checks whether the specified SHA-256 hash value matches the actual hash value
        /// of the specified file.
        /// <para>Returns true if the specified hash value matches the actual hash value of 
        /// the file, and false otherwise.</para></summary>
        /// <param name="filePath">Path to the file whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose validity is checked for the specified file.</param>
        /// <returns>True if the specified file hash value matches the actual value, false otherwise.</returns>
        public static bool CheckFileHashSha256(string filePath, string hashValue)
        {
            if (hashValue != null)
                hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetFileHashSha256(filePath);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }

        /// <summary>Checks whether the specified SHA-512 hash value matches the actual hash value
        /// of the specified file.
        /// <para>Returns true if the specified hash value matches the actual hash value of 
        /// the file, and false otherwise.</para></summary>
        /// <param name="filePath">Path to the file whose hash value is checked.</param>
        /// <param name="hashValue">Supposed hash value whose validity is checked for the specified file.</param>
        /// <returns>True if the specified file hash value matches the actual value, false otherwise.</returns>
        public static bool CheckFileHashSha512(string filePath, string hashValue)
        {
            if (hashValue != null)
                hashValue = hashValue.Trim().ToUpper();
            string actualHash = GetFileHashSha512(filePath);
            if (actualHash != null)
                actualHash = actualHash.ToUpper();
            return hashValue == actualHash;
        }


        #endregion CeckHashFile


    }  // class UtilCrypto



}


