// Copyright (c) Igor Grešovnik (2009 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Security.Cryptography;

using IG.Lib;

namespace IG.Crypto
{




    /// <summary>Enumeration for selecting the hashing algorithm.</summary>
    /// <remarks><para>All names must be at most 20 characters in length (constant <see cref="ConstCrypto.MaxTypeStringLength"/>).</para>
    /// <para>Names can only contain ASCII characters and may not change once they are defined (as many functionality will
    /// rely on string representation of flag values rather than integer codes).</para>
    /// <para>Integer numbers of flags should not change once they are defined. New numbers must be defined sych that they
    /// can be combined by bitwise or (i.e. as flags). The "Default" value may eventually be changed (for which there must
    /// be very strong reasons) but must always point to some other value that is not "None".</para></remarks>
    /// $A Igor Apr10;
    [Flags]
    public enum HashType : int
    {
        /// <summary>No hashing algorithm specified.</summary>
        None = 0,
        /// <summary>Default hashing algorithm.</summary>
        Default = SHA512,
        /// <summary>Specifies the MD5 hashing algorithm.</summary>
        MD5 = 1,
        /// <summary>Specifies the SHA1 hashing algorithm.</summary>
        SHA1 = 2,
        /// <summary>Specifies the SHA256 hashing algorithm.</summary>
        SHA256 = 4,
        /// <summary>Specifies the SHA512 hashing algorithm.</summary>
        SHA512 = 8,
    }

    /// <summary>Enumeration for selecting the algorithm for generation of secret keys, initialization vectors,
    /// etc., on basis of provided passwords, keys, and salts.
    /// <para>The algorithm must be of type <see cref="System.Security.Cryptography.DeriveBytes"/></para></summary>
    /// <remarks>
    /// <para>All names must be at most 20 characters in length (constant <see cref="ConstCrypto.MaxTypeStringLength"/>).</para>
    /// <para>Names can only contain ASCII characters and may not change once they are defined (as many functionality will
    /// rely on string representation of flag values rather than integer codes).</para>
    /// <para>Integer numbers of flags should not change once they are defined. New numbers must be defined sych that they
    /// can be combined by bitwise or (i.e. as flags). The "Default" value may eventually be changed (for which there must
    /// be very strong reasons) but must always point to some other value that is not "None".</para>
    /// </remarks>
    /// $A Igor Apr10;
    [Flags]
    public enum PasswordAlgorithmType : int
    {
        /// <summary>No asymmetric algorithm specified.</summary>
        None = 0,
        /// <summary>Default key derivation algorithm.</summary>
        Default = Rfc2898,
        /// <summary>Default key derivation algorithm.</summary>
        Rfc = Rfc2898,
        /// <summary>Specifies the Rfc2898DeriveBytes algorithms.</summary>
        Rfc2898 = 1,
        /// <summary>Specifies the DeriveBytes algorithm.</summary>
        DeriveBytes = 2
    }

    /// <summary>Enumeration for selecting the symmetric encryption algorithm.</summary>
    /// <remarks>
    /// <para>All names must be at most 20 characters in length (constant <see cref="ConstCrypto.MaxTypeStringLength"/>).</para>
    /// <para>Names can only contain ASCII characters and may not change once they are defined (as many functionality will
    /// rely on string representation of flag values rather than integer codes).</para>
    /// <para>Integer numbers of flags should not change once they are defined. New numbers must be defined sych that they
    /// can be combined by bitwise or (i.e. as flags). The "Default" value may eventually be changed (for which there must
    /// be very strong reasons) but must always point to some other value that is not "None".</para>
    /// </remarks>
    /// $A Igor Apr10;
    [Flags]
    public enum SymmetricAlgorithmType : int
    {
        /// <summary>No hashing algorithm specified.</summary>
        None = 0,
        /// <summary>Default Symmetric algorithm.</summary>
        Default = Rijndael,
        /* Alias for Rijndael */
        RD = Rijndael,
        /// <summary>Alias for Rijndael (used in Advanced Encryption Standard - AES, only that AES has 
        /// block size limited to 128 bits and has some other limitations).</summary>
        Rijndael = 1,
        /// <summary>Specifies the AES symmetric encryption algorithm.</summary>
        AES = 2,
        /// <summary>Specifies the TripleDES symmetric encryption algorithm.</summary>
        TripleDES = 4,
        /// <summary>Specifies the DES symmetric encryption algorithm.</summary>
        DES = 8,
        /// <summary>Specifies the RC2 symmetric encryption algorithm.</summary>
        RC2 = 16
    }

    /// <summary>Enumeration for selecting the asymmetric cryptographic algorithm.</summary>
    /// <remarks>
    /// <para>All names must be at most 20 characters in length (constant <see cref="ConstCrypto.MaxTypeStringLength"/>).</para>
    /// <para>Names can only contain ASCII characters and may not change once they are defined (as many functionality will
    /// rely on string representation of flag values rather than integer codes).</para>
    /// <para>Integer numbers of flags should not change once they are defined. New numbers must be defined sych that they
    /// can be combined by bitwise or (i.e. as flags). The "Default" value may eventually be changed (for which there must
    /// be very strong reasons) but must always point to some other value that is not "None".</para>
    /// </remarks>
    /// $A Igor Apr10;
    [Flags]
    public enum AsymmetricAlgorithmType : int
    {
        /// <summary>No asymmetric algorithm specified.</summary>
        None = 0,
        /// <summary>Default asymmetric algorithm.</summary>
        Default = RSA,
        /// <summary>Specifies the RSA asymmetric algorithms.</summary>
        RSA = 1,
        /// <summary>Specifies the DSA asymmetric algorithm.</summary>
        DSA = 2
    }





    /// <summary>Flags for cryptographic operations.</summary>
    /// <remarks>These flags describe which cryptographic operations has been performed and some other details.
    /// <para>The flags are written e.g. to headers of encrypted data in order to know what data should be expected 
    /// further in the header. Sizes of further data are then specified by further information such as types of 
    /// algorithms used, key sizes, salt and initialization vecor lengths, etc.</para></remarks>
    /// $A Igor May15;
    [Flags]
    public enum CryptoFlags : int
    {
        None = 0,
        EncryptionError = -1,
        DecryptionError = -2,
        Error = EncryptionError | DecryptionError,
        Hashing = 1,
        KeyGeneration = 2,
        SymmetricEncryption = 4,
        AsymmetricEncryption = 8,
        Signing = 16,
        Public = 32,
        Private = 64
    }


    /// <summary>This class contains key parameters of the cryptographic operations performed on some item.
    /// <para>Class also handles some tasks that enable consisstent performance of operations, such as writing 
    /// and reading cryptographic heads of encrypted files and data.</para></summary>
    /// <remarks>This is the base class for </remarks>
    /// $A Igor May15;
    public class CryptoParameters
    {
        public CryptoParameters()
        { }

        /// <summary>Resets the state.</summary>
        public void Reset()
        {
            Flags = CryptoFlags.None;
        }

        #region Flags

        CryptoFlags _flags = CryptoFlags.None;

        /// <summary>Flags for cryptographic operations performed.
        /// <para>Warning: don't set the whole flags direcryl, only individual flags should be set!</para>CryptoFlags.</summary>
        public CryptoFlags Flags { get { return _flags; } private set { _flags = value; } }

        /// <summary>Returns true if ALL the specified flags are set, or false if some of them are unset.</summary>
        /// <param name="whichFlags">Specifies which flags are queried. A set of one or more values can be specified, 
        /// of which each one can be an orred combination of flags to be queried.</param>
        public bool IsFlagsSet(params CryptoFlags[] whichFlags)
        {
            CryptoFlags allFlags = 0;
            foreach (CryptoFlags flag in whichFlags)
            {
                allFlags |= flag;
            }
            return (Flags & allFlags) == allFlags;
        }

        /// <summary>Returns true if any of the specified flags is set, or false if none of them is set.</summary>
        /// <param name="whichFlags">Specifies which flags are queried. A set of one or more values can be specified, 
        /// of which each one can be an orred combination of flags to be queried.</param>
        public bool IsAnyFlagSet(params CryptoFlags[] whichFlags)
        {
            CryptoFlags allFlags = 0;
            foreach (CryptoFlags flag in whichFlags)
            {
                allFlags |= flag;
            }
            return (Flags & allFlags) != 0;
        }

        /// <summary>Sets the specified flags.</summary>
        /// <param name="whichFlags">Specifies which flags are set. A single flag can be specified such as 
        /// <see cref="CryptoFlags.Hashing"/>, or an orred combination of flags to be set.</param>
        public void SetFlags(params CryptoFlags[] whichFlags)
        {
            foreach (CryptoFlags flag in whichFlags)
            {
                Flags |= flag;
            }
        }

        /// <summary>Clears the specified flags.</summary>
        /// <param name="firstFlags">The same as <paramref name="whichFlags"/>. This argument is actually the first of 
        /// flags that are taken into account, ensuring that at least one value is specified in the call with 
        /// variable number of arguments.</param>
        /// <param name="whichFlags">Specifies which flags are cleared. A set of one or more values can be specified, 
        /// of which each one can be an orred combination of flags to be set.</param>
        public void ClearFlags(CryptoFlags firstFlags, params CryptoFlags[] whichFlags)
        {
            Flags &= ~firstFlags;
            foreach (CryptoFlags flag in whichFlags)
            {
                Flags &= (~flag);
            }
        }

        /// <summary>Clears all flags.</summary>
        public void ClearAllFlags()
        {
            Flags = CryptoFlags.None;
        }

        #endregion Flags






    } // class CryptoParameters







    /// <summary>Contains constants for cryptographic operations from shell functions.</summary>
    public static class ConstCrypto
    {

        /// <summary>Maximal length of string representing algorithm types.</summary>
        public const int MaxTypeStringLength = 20;

        /// <summary>Minimal password length for encryption.</summary>
        public const int MinPasswordLength = 8;

        /// <summary>Minimal salt length for key derivation.</summary>
        public const int MinPasswordSaltLength = 8;

        /// <summary>Minimal salt length for encryption.</summary>
        public const int MinSaltLength = 8;

        /// <summary>Default size of file buffer.</summary>
        public const int FileBufferSize = 1024;

        /// <summary>Default number of key generation algorithms when generating the keys.</summary>
        public const int DefaultNumPasswordIterations = 2000;

        /// <summary>Default password length.</summary>
        public const int DefaultPasswordLength = 12;

        /// <summary>Default salt length.</summary>
        public const int DefaultSaltLength = 64;

        /// <summary>Default key length.</summary>
        public const int DefaultKeyLength = 64;

        /// <summary>File extension for files containing the calculated hashRet codes.</summary>
        public const string HashFileExtension = ".chk";

        /// <summary>File extension for encrypted files.</summary>
        public const string FileExtensionEncrypted = ".ig_enc";

        /// <summary>File extension for encrypted files.</summary>
        public const string FileExtensionDecrypted = ".ig_dec";

        public const string FileSignatureEncrypted = "IGLibEnc";


        #region CommandLineOptions

        #region CommandlineOptions.Operation

        /// <summary>Verification (check) is performed rather than calculation (although this 
        /// may also include calculation).</summary>
        public const string ArgCheck = "-c";
        /// <summary>Verification (check) is performed rather than calculation (although this 
        /// may also include calculation).</summary>
        public const string ArgCheck1 = "/c";

        /// <summary>Specifies that decryption is performed, rather than encryption.</summary>
        public const string ArgDecrypt = "-decrypt";

        /// <summary>Specifies that decryption is performed, rather than encryption.</summary>
        public const string ArgDecrypt1 = "/decrypt";

        /// <summary>Specifies that encryption is performed, rather than decryption.</summary>
        public const string ArgEncrypt = "-encrypt";

        /// <summary>Specifies that encryption is performed, rather than decryption.</summary>
        public const string ArgEncrypt1 = "/encrypt";
        
        /// <summary>Prescribed or expected time of exection (e.g. in time measurements).</summary>
        public const string ArgTime = "-time";

        /// <summary>Prescribed or expected time of exection (e.g. in time measurements).</summary>
        public const string ArgTime1 = "/time";

        #endregion CommandlineOptions.Operation

        #region CommandlineOptions.Algorithms

        /// <summary>Specification of algorithm type follows.</summary>
        public const string ArgAlgorithmType = "-t";
        /// <summary>Specification of algorithm type follows.</summary>
        public const string ArgAlgorithmType1 = "/t";

        /// <summary>Specification of password or key generaton algorithm type follows.</summary>
        public const string ArgPasswordAlgorithmType = "-pwt";
        /// <summary>Specification of password or key generaton algorithm type follows.</summary>
        public const string ArgPasswordAlgorithmType1 = "/pwt";

        /// <summary>True or false follows, a flag specifying whether the largest key for an algorithm is used.</summary>
        public const string ArgLargestKey = "-largestkey";

        /// <summary>True or false follows, a flag specifying whether the largest key for an algorithm is used.</summary>
        public const string ArgLargestKey1 = "/largestkey";

        #endregion CommmandlineOptions.Algorithms


        #region CommandLineOptions.Asym



        /// <summary>Specification of asymmetric algorithm type follows.</summary>
        public const string ArgAsymAlgorithmType = "-at";
        /// <summary>Specification of asymmetric algorithm type follows.</summary>
        public const string ArgAsymAlgorithmType1 = "/at";


        /// <summary>Specification of assymmetric key container name.</summary>
        public const string ArgAsymKeyName = "-akn";
        /// <summary>Specification of assymmetric key container name.</summary>
        public const string ArgAsymKeyName1 = "/akn";

        /// <summary>Specification of assymmetric key xml file.</summary>
        public const string ArgAsymKeyXmlFile = "-akxml";
        /// <summary>Specification of assymmetric key xml file.</summary>
        public const string ArgAsymKeyXmlFile1 = "/akxml";

        /// <summary>Specification that assymmetric CSP (crypto service provider) flag follows.</summary>
        public const string ArgAsymKeyFlag = "-akf";
        /// <summary>Specification that assymmetric key CSP (crypto service provider) flag follows.</summary>
        public const string ArgAsymKeyFlag1 = "/akf";


        /// <summary>Specification that assymmetric private keys are be exported (or printed), too.</summary>
        public const string ArgAsymExportPrivateKey = "-exportprivatekey";
        /// <summary>Specification that assymmetric private keys are be exported, (or printed), too.</summary>
        public const string ArgAsymExportPrivateKey1 = "/exportprivatekey";

        /// <summary>Specification that certificate file path follows.</summary>
        public const string ArgCertificateFile = "-cf";
        /// <summary>Specification that certificate file path follows.</summary>
        public const string ArgCertificateFile1 = "/cf";

        /// <summary>Specification that certificate password (for access to private key) follows.</summary>
        public const string ArgCertificatePassword = "-cp";
        /// <summary>Specification that certificate password (for access to private key) follows.</summary>
        public const string ArgCertificatePassword1 = "/cp";

        /// <summary>Specifies that certificate password (for access to private key) should be obtained through 
        /// the user interface (normally, throough console).</summary>
        public const string ArgCertificatePasswordThroughUI = "-cpui";
        /// <summary>Specifies that certificate password (for access to private key) should be obtained through 
        /// the user interface (normally, throough console).</summary>
        public const string ArgCertificatePasswordThroughUI1 = "/cpui";


        #endregion CommandLineOptions.Asym




        #region CommandLineOptionsSalts_Passwords_IV

        /// <summary>Specification that number of iterations used in key generation algorithm follows.</summary>
        public const string ArgPasswordNumIterations = "-pwit";
        /// <summary>Specification that number of iterations used in key generation algorithm follows.</summary>
        public const string ArgPasswordNumIterations1 = "/pwit";

        /// <summary>Specification that key as hexadecimal byte sequence follows.</summary>
        public const string ArgKeyHexBytes = "-kx";
        /// <summary>Specification that key as hexadecimal byte sequence follows.</summary>
        public const string ArgKeyHexBytes1 = "/kx";

        /// <summary>Specification that key as base-64 encoded byte sequence follows.</summary>
        public const string ArgKeyBase64Bytes = "-k64";
        /// <summary>Specification that key as base-64 encoded byte sequence follows.</summary>
        public const string ArgKeyBase64Bytes1 = "/k64";

        /// <summary>Specification that length of the key follows.</summary>
        public const string ArgKeyLength = "-klen";
        /// <summary>Specification that length of the key follows.</summary>
        public const string ArgKeyLength1 = "/klen";

        /// <summary>Specification  that length of the password or key follows.</summary>
        public const string ArgPasswordLength = "-pwlen";
        /// <summary>Specification  that length of the password or key follows.</summary>
        public const string ArgPasswordLength1 = "/pwlen";

        /// <summary>Specification that password string follows.</summary>
        public const string ArgPasswordString = "-pw";
        /// <summary>Specification that password string follows.</summary>
        public const string ArgPasswordString1 = "/pw";

        /// <summary>Specification that password as hexadecimal byte sequence follows.</summary>
        public const string ArgPasswordHexBytes = "-pwx";
        /// <summary>Specification that password as hexadecimal byte sequence follows.</summary>
        public const string ArgPasswordHexBytes1 = "/pwx";

        /// <summary>Specification of password as base-64 encoded byte sequence follows.</summary>
        public const string ArgPasswordBase64Bytes = "-pw64";
        /// <summary>Specification of password as base-64 encoded byte sequence follows.</summary>
        public const string ArgPasswordBase64Bytes1 = "/pw64";

        /// <summary>Specification  of length of the salt follows.</summary>
        public const string ArgSaltLength = "-sllen";
        /// <summary>Specification  of length of the salt follows.</summary>
        public const string ArgSaltLength1 = "/sllen";

        /// <summary>Specification of password string follows.</summary>
        public const string ArgSaltString = "-sl";
        /// <summary>Specification of password string follows.</summary>
        public const string ArgSaltString1 = "/sl";

        /// <summary>Specification of password as hexadecimal byte sequence follows.</summary>
        public const string ArgSaltHexBytes = "-slx";
        /// <summary>Specification of password as hexadecimal byte sequence follows.</summary>
        public const string ArgSaltHexBytes1 = "/slx";

        /// <summary>Specification of password as base-64 encoded byte sequence follows.</summary>
        public const string ArgSaltBase64Bytes = "-sl64";
        /// <summary>Specification of password as base-64 encoded byte sequence follows.</summary>
        public const string ArgSaltBase64Bytes1 = "/sl64";

        /// <summary>Specification  of length of the initialization vector follows.</summary>
        public const string ArgIvLength = "-ivlen";
        /// <summary>Specification  of length of the initialization vector follows.</summary>
        public const string ArgIvLength1 = "/ivlen";

        /// <summary>Specification of initialization vector in form of string follows.</summary>
        public const string ArgIvString = "-iv";
        /// <summary>Specification of initialization vector in form of string follows.</summary>
        public const string ArgIvString1 = "/iv";

        /// <summary>Specification of initialization vector as hexadecimal byte sequence follows.</summary>
        public const string ArgIvHexBytes = "-ivx";
        /// <summary>Specification of initialization vector as hexadecimal byte sequence follows.</summary>
        public const string ArgIvHexBytes1 = "/ivx";

        /// <summary>Specification of initialization vector as base-64 encoded byte sequence follows.</summary>
        public const string ArgIvBase64Bytes = "-iv64";
        /// <summary>Specification of initialization vector as base-64 encoded byte sequence follows.</summary>
        public const string ArgIvBase64Bytes1 = "/iv64";


        #endregion CommandLineOptionsSalts_Passwords_IV

        #region CommandlineOptions.Strings_ByteArrays

        /// <summary>String output format (and possibly input, if not specified separately) is used.</summary>
        public const string ArgStringOutputFormat = "-sf";
        /// <summary>String output format (and possibly input, if not specified separately) is used.</summary>
        public const string ArgStringOutputFormat1 = "/bfx";

        /// <summary>Binary output format (and possibly input, if not specified separately) used is hexadecimal.</summary>
        public const string ArgBinaryOutputFormatHex = "-bfx";
        /// <summary>Binary output format (and possibly input, if not specified separately) used is hexadecimal.</summary>
        public const string ArgBinaryOutputFormatHex1 = "/bfx";

        /// <summary>Binary output format (and possibly input, if not specified separately) used is hexadecimal.</summary>
        public const string ArgBinaryOutputFormatBase64 = "-bf64";
        /// <summary>Binary output format (and possibly input, if not specified separately) used is hexadecimal.</summary>
        public const string ArgBinaryOutputFormatBase641 = "/bf64";

        /// <summary>Binary output format (and possibly input, if not specified separately) used is long integer.</summary>
        public const string ArgBinaryOutputFormatLongInt = "-bfi";
        /// <summary>Binary output format (and possibly input, if not specified separately) used is long integer.</summary>
        public const string ArgBinaryOutputFormatLongInt1 = "/bfi";

        /// <summary>String input format is used.</summary>
        public const string ArgStringInputFormat= "-sfi";
        /// <summary>String input format is used.</summary>
        public const string ArgStringInputFormat1 = "/sfi";

        /// <summary>Binary input format used is hexadecimal.</summary>
        public const string ArgBinaryInputFormatHex = "-bfix";
        /// <summary>Binary input format used is hexadecimal.</summary>
        public const string ArgBinaryInputFormatHex1 = "/bfix";

        /// <summary>Binary input format used is base-64 encoding.</summary>
        public const string ArgBinaryInputFormatBase64 = "-bfi64";
        /// <summary>Binary input format used is base-64 encoding.</summary>
        public const string ArgBinaryInputFormatBase641 = "/bfi64";

        /// <summary>Binary input format used is long int.</summary>
        public const string ArgBinaryInputFormatLongInt = "-bfii";
        /// <summary>Binary input format used is long int.</summary>
        public const string ArgBinaryInputFormatLongInt1 = "/bfii";

        /// <summary>Specification of string that is worked follows.</summary>
        public const string ArgString = "-s";
        /// <summary>Specification of string that is worked follows.</summary>
        public const string ArgString1 = "/s";

        /// <summary>Specification of hashRet value follows (only with -c, for a single input file).</summary>
        public const string ArgHashValue = "-h";
        /// <summary>Specification of hashRet value follows (only with -c, for a single input file).</summary>
        public const string ArgHashValue1 = "/h";

        #endregion CommandlineOptions.Strings_ByteArrays

        #region CommandlineOptions.Files

        /// <summary>Specification of output file follows.</summary>
        public const string ArgOutputFile = "-o";
        /// <summary>Specification of output file follows.</summary>
        public const string ArgOutputFile1 = "/o";


        /// <summary>If files are to be overwritten, permission is not asked for and overwriting is forced.</summary>
        public const string ArgForceOverwrite = "-yo";
        /// <summary>If files are to be overwritten, permission is not asked for and overwriting is forced..</summary>
        public const string ArgForceOverwrite1 = "/yo";

        /// <summary>If files are to be overwritten, this is skipped without asking whether to overwrite.</summary>
        public const string ArgSkipOverwrite = "-no";
        /// <summary>If files are to be overwritten, this is skipped without asking whether to overwrite.</summary>
        public const string ArgSkipOverwrite1 = "/no";



        /// <summary>If files are to be deleted, permission is not asked for and deletion is forced.</summary>
        public const string ArgForceDelete = "-yd";
        /// <summary>If files are to be deleted, permission is not asked for and deletion is forced.</summary>
        public const string ArgForceDelete1 = "/yd";

        /// <summary>If files are to be deleted, this is automatically skipped without asking whether to delete.</summary>
        public const string ArgSkipDelete = "-nd";
        /// <summary>If files are to be deleted, this is automatically skipped without asking whether to delete.</summary>
        public const string ArgSkipDelete1 = "/nd";


        /// <summary>Paths will be output as absolute paths.</summary>
        public const string ArgPathsAbsolute = "-pa";
        /// <summary>Paths will be output as absolute paths.</summary>
        public const string ArgPathsAbsolute1 = "/pa";

        /// <summary>Paths will be output as relative paths.</summary>
        public const string ArgPathsRelative = "-pr";
        /// <summary>Paths will be output as relative paths.</summary>
        public const string ArgPathsRelative1 = "/pr";

        /// <summary>Specification of a directory for recursive search of input files follows.</summary>
        public const string ArgRecursiveDirectory = "-rd";
        /// <summary>Specification of a directory for recursive search of input files follows.</summary>
        public const string ArgRecursiveDirectory1 = "/rd";

        /// <summary>Specification of a directory for recursive search of input files, sorted by levels, follows.</summary>
        public const string ArgRecursiveDirectoryByLevels = "-rdl";
        /// <summary>Specification of a directory for recursive search of input files, sorted by levels, follows.</summary>
        public const string ArgRecursiveDirectoryByLevels1 = "/rdl";

        /// <summary>Specification of a file search pattern for recursive directory search of input files follows.</summary>
        public const string ArgRecursiveDirectoryPattern = "-rp";
        /// <summary>Specification of a file search pattern for recursive directory search of input files follows.</summary>
        public const string ArgRecursiveDirectoryPattern1 = "/rp";

        /// <summary>Specification of a level of recursive directory search of input files follows.</summary>
        public const string ArgRecursiveDirectoryLevel = "-rl";
        /// <summary>Specification of a level of recursive directory search of input files follows.</summary>
        public const string ArgRecursiveDirectoryLevel1 = "/rl";


        // Cleaning options:

        /// <summary>Specifies that original files are deleted after being encrypted or decrypted.</summary>
        public const string ArgDeleteOriginal = "-delorig";

        /// <summary>Specifies that original files are deleted after being encrypted or decrypted.</summary>
        public const string ArgDeleteOriginal1 = "/delorig";

        /// <summary>Specifies that decrypted files (i.e., files with the <see cref="ConstCrypto.FileExtensionDecrypted"/> extension) are deleted.
        /// <para>In most cases, such files will only be deleted if either the corresponding 
        /// original or encrypted file exists.</para></summary>
        public const string ArgDeleteDecrypted = "-deldecrypted";

        /// <summary>Specifies that decrypted files (i.e., files with the <see cref="FileExtensionDecrypted"/> extension) are deleted.
        /// <para>In most cases, such files will only be deleted if either the corresponding 
        /// original or encrypted file exists.</para></summary>
        public const string ArgDeleteDecrypted1 = "/deldecrypted";

        /// <summary>Specifies that encrypted files (i.e., files with the <see cref="FileExtensionEncrypted"/> extension) are deleted.
        /// <para>In most cases, such files will only be deleted if the corresponding original file exists.</para></summary>
        public const string ArgDeleteEncrypted = "-delencrypted";

        /// <summary>Specifies that encrypted files (i.e., files with the <see cref="FileExtensionEncrypted"/> extension) are deleted.
        /// <para>In most cases, such files will only be deleted if either the corresponding original file exists.</para></summary>
        public const string ArgDeleteEncrypted1 = "/delencrypted";

        /// <summary>Specifies that all versions of a single file are deleted in the cleaning operation.</summary>
        public const string ArgDeleteAllVersions = "-delallversions";

        /// <summary>Specifies that all versions of a single file are deleted in the cleaning operation.</summary>
        public const string ArgDeleteAllVersions1 = "/delallversions";


        #endregion CommandlineOptions.Files

        #endregion CommandlineOptiions


    }  // class ConstCrypto


    /// <summary>Base class for algorithms that generate passwords, encryption initialization vectors, and
    /// salts from the specified keys/passwords and salts.</summary>
    /// <remarks>
    /// <para>Classes derived from this class also serve for unification of behavior of classes such as 
    /// <see cref="System.Security.Cryptography.PasswordDeriveBytes"/> and <see cref="System.Security.Cryptography.Rfc2898DeriveBytes"/></para>
    /// <para>Derived classes must have a default constructor and a constructor with a flag specifying whether
    /// initialization parameters are public. Initialization must be performed by one of the Init(...) functions.</para>
    /// <para>Any concrete password generator class must be first initialized with a key or password and a salt.</para>
    /// <para>After initialization, each consecutive call to GetBytes() creates an unique key that is derived 
    /// from the key and salt provided at initialization (except the <see cref="PasswordAlgorithmNone"/>
    /// class).</para>
    /// </remarks>
    public abstract class PasswordAlgorithmBase
    {

        protected PasswordAlgorithmBase(bool publicParameters = false)
        {
            this.IsParametersPublic = publicParameters;
            Reset();
        }

        /// <summary>Clears all data and resets the passwords/keys/salts generation algorithm.</summary>
        /// <param name="publicParameters">If true then parameters with which this object has been initialized can be queried.</param>
        public virtual void Init(bool publicParameters = false)
        {
            this.IsInitialized = false;
            this.IsParametersPublic = true;
        }

        /// <summary>Resets the password/key generator (Clears its parameters and sets the initialization 
        /// flag <see cref="IsInitialized"/> to false).</summary>
        public virtual void Reset()
        {
            IsInitialized = false;
            PasswordString = null;
            PasswordBytes = null;
            SaltString = null;
            SaltBytes = null;
            NumIterations = 0;
            ClearExternalAlgorithm();
        }

        public virtual void Init(string password, byte[] salt, int numIterations)
        {
            Reset();
            this.PasswordString = password;
            this.SaltBytes = salt;
            this.NumIterations = numIterations;
            InitializeExternalAlgorithm();
            this._isInitialized = true;
        }

        public virtual void Init(byte[] password, byte[] salt, int numIterations)
        {
            Reset();
            this.PasswordBytes = password;
            this.SaltBytes = salt;
            this.NumIterations = numIterations;
            InitializeExternalAlgorithm();
            this._isInitialized = true;
        }


        #region Data

        private bool _isPublicParameters = false;

        /// <summary>Flag that specifies whether initialization parameters of the algorithm can be queried or not.
        /// <para>Making parameters inaccessible prevents developer's errors where they store secret parameters
        /// in places they should not be stored.</para></summary>
        public bool IsParametersPublic { get { return _isPublicParameters; } protected set { _isPublicParameters = value; } }

        private bool _isInitialized = false;

        /// <summary>Flag that specifies whether the algorithm has been initialized.
        /// <para>This means that it has been provided with a key and a salt and is able do generate consecuttive passwords).</para></summary>
        public bool IsInitialized { get { return _isInitialized; } set { _isInitialized = value; } }


        private string _passwordString = null;

        /// <summary>Password, in form of string, to be used in generation of keys, initialization vectors, etc.</summary>
        protected string PasswordString { get { return _passwordString; } set { _passwordString = value; } }

        private byte[] _passwordBytes = null;

        /// <summary>Password, in form of byte array, to be used in generation of keys, initialization vectors, etc.</summary>
        protected byte[] PasswordBytes { get { return _passwordBytes; } set { _passwordBytes = value; } }

        private string _saltString = null;

        /// <summary>Salt, in form of byte array, to be used in generation of keys, initialization vectors, etc.</summary>
        protected string SaltString { get { return _saltString; } set { _saltString = value; } }

        private byte[] _saltBytes = null;

        /// <summary>Salt, in form of byte array, to be used in generation of keys, initialization vectors, etc.</summary>
        protected byte[] SaltBytes { get { return _saltBytes; } set { _saltBytes = value; } }

        protected int _numIterations = 0;

        /// <summary>Number of iteration used when generating keys.
        /// <para>Must be at least 1.</para></summary>
        protected int NumIterations
        { get { return _numIterations; } set { _numIterations = value; } }


        private byte[] _lastGeneratedBytes = null;

        /// <summary>Last value of the generated bytes.</summary>
        protected byte[] LastGeneratedBytes { get { return _lastGeneratedBytes; } set { _lastGeneratedBytes = value; } }

        protected int _numGenerations = 0;

        /// <summary>Number of generated keys (byte sequences) up to now.</summary>
        protected int NumGenerations
        { get { return _numGenerations; } set { _numGenerations = value; } }



        /// <summary>If the generator has public parameters (flag <see cref="IsParametersPublic"/>) then the
        /// password string is returned. Otherwise, an <see cref="InvalidOperationException"/> is thrown.</summary>
        /// <returns></returns>
        public string GetPasswordString()
        {
            if (!IsParametersPublic) throw new InvalidOperationException("The current key generator does not have initialization parameters public.");
            return PasswordString;
        }

        /// <summary>If the generator has public parameters (flag <see cref="IsParametersPublic"/>) then the
        /// password in form of a byte array is returned. Otherwise, an <see cref="InvalidOperationException"/> is thrown.</summary>
        public byte[] GetPasswordBytes()
        {
            if (!IsParametersPublic) throw new InvalidOperationException("The current key generator does not have initialization parameters public.");
            return PasswordBytes;
        }

        /// <summary>If the generator has public parameters (flag <see cref="IsParametersPublic"/>) then the
        /// salt string is returned. Otherwise, an <see cref="InvalidOperationException"/> is thrown.</summary>
        public string GetSaltString()
        {
            if (!IsParametersPublic) throw new InvalidOperationException("The current key generator does not have initialization parameters public.");
            return SaltString;
        }

        /// <summary>If the generator has public parameters (flag <see cref="IsParametersPublic"/>) then the
        /// salt in form of a byte array is returned. Otherwise, an <see cref="InvalidOperationException"/> is thrown.</summary>
        public byte[] GetSaltBytes()
        {
            if (!IsParametersPublic) throw new InvalidOperationException("The current key generator does not have initialization parameters public.");
            return SaltBytes;
        }

        /// <summary>If the generator has public parameters (flag <see cref="IsParametersPublic"/>) then the
        /// number of iterations is returned. Otherwise, an <see cref="InvalidOperationException"/> is thrown.</summary>
        public int GetNuIterations()
        {
            if (!IsParametersPublic) throw new InvalidOperationException("The current key generator does not have initialization parameters public.");
            return NumIterations;
        }


        /// <summary>If the generator has public parameters (flag <see cref="IsParametersPublic"/>) then the last generated key
        /// in form of a byte array is returned. Otherwise, an <see cref="InvalidOperationException"/> is thrown.</summary>
        public byte[] GetLastGeneratedBytes()
        {
            if (!IsParametersPublic) throw new InvalidOperationException("The current key generator does not have initialization parameters public.");
            return LastGeneratedBytes;
        }

        /// <summary>Returns the number of keys (byte sequences) generated until now.</summary>
        public int GetNuGenerations()
        {
            return NumGenerations;
        }

        #endregion Data


        #region Operation

        /// <summary>Initializes eventual external algorithm used as proxy object to perform operations.</summary>
        protected abstract void InitializeExternalAlgorithm();

        /// <summary>Clears eventual external algorithm used as proxy object to perform operations.</summary>
        protected abstract void ClearExternalAlgorithm();

        /// <summary>Returns the next generated key of the specified length.
        /// <para>In principle, the successive calls will return different and uncorrelated byte sequences.</para></summary>
        /// <param name="numBytes">Number of bytes in the returned array.</param>
        /// <param name="outputBytes">Reference to a byte array where generated bytes will be stored.</param>
        protected abstract void GetBytesInternal(int numBytes, ref byte[] outputBytes);

        /// <summary>Creates and stores the next generated key of the specified length.
        /// <para>In principle, the successive calls will return different and uncorrelated byte sequences.</para></summary>
        /// <param name="numBytes">Number of bytes in the returned array.</param>
        /// <param name="outputBytes">Reference to a byte array where generated bytes will be stored.</param>
        public void GetBytes(int numBytes, ref byte[] outputBytes)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Can not generate the next key, password generation object is not initialized.");
            // byte[] ReturnedString = null;
            GetBytesInternal(numBytes, ref outputBytes);
            // outputBytes = ReturnedString;
            LastGeneratedBytes = outputBytes;
            ++NumGenerations;
        }

        /// <summary>Creates and returns the next generated key of the specified length by calling <see cref="GetBytes(int, ref byte[])"/>.
        /// <para>In principle, the successive calls will return different and uncorrelated byte sequences.</para></summary>
        /// <param name="numBytes">Number of bytes in the returned array.</param>
        public byte[] GetBytes(int numBytes)
        {
            byte[] outputBytes = null;
            GetBytes(numBytes, ref outputBytes);
            return outputBytes;
        }

        #endregion Operation


    }  // class PasswordAlgorithmBase



    /// <summary>Key generation algorithm that uses a <see cref="Rfc2898DeriveBytes"/> object (PBKDF2 algorithm).</summary>
    public class PasswordAlgorithmRfc2898 : PasswordAlgorithmBase
    {

        public PasswordAlgorithmRfc2898()
            : base()
        { }

        public PasswordAlgorithmRfc2898(bool publicParameters)
            : base(publicParameters)
        { }

        #region Operation


        private Rfc2898DeriveBytes _externalAlgorithm;

        protected Rfc2898DeriveBytes ExternalAlgorithm { get { return _externalAlgorithm; } set { _externalAlgorithm = value; } }

        /// <summary>Initializes eventual external algorithm used as proxy object to perform operations.</summary>
        protected override void InitializeExternalAlgorithm()
        {
            if (PasswordBytes != null)
            {
                if (SaltBytes != null)
                    ExternalAlgorithm = new Rfc2898DeriveBytes(PasswordBytes, SaltBytes, NumIterations);
                else
                    throw new ArgumentException("Salt is not provided for key generation algorithm.");
            }
            else if (PasswordString != null)
            {
                if (SaltBytes != null)
                    ExternalAlgorithm = new Rfc2898DeriveBytes(PasswordString, SaltBytes, NumIterations);
                else
                    throw new ArgumentException("Salt is not provided for key generation algorithm.");
            }
            else
            {
                throw new ArgumentException("Password is not provided for key generation algorithm, neither as string nor as byte array.");
            }
        }

        /// <summary>Clears eventual external algorithm used as proxy object to perform operations.</summary>
        protected override void ClearExternalAlgorithm()
        {
            ExternalAlgorithm = null;
        }

        /// <summary>Returns the next generated key of the specified length.</summary>
        /// <param name="numBytes">Number of bytes in the returned array.</param>
        /// <param name="outputBytes">Reference to a byte array where generated bytes will be stored.</param>
        protected override void GetBytesInternal(int numBytes, ref byte[] outputBytes)
        {
            outputBytes = ExternalAlgorithm.GetBytes(numBytes);
        }

        #endregion Operation



    }  // class PasswordAlgorithmRfc2898




    /// <summary>Key generation algorithm that uses a <see cref="Rfc2898DeriveBytes"/> object (PBKDF2 algorithm).</summary>
    /// $A Igor Jun15;
    public class PasswordAlgorithmDeriveBytes : PasswordAlgorithmBase
    {

        public PasswordAlgorithmDeriveBytes()
            : base()
        { }

        public PasswordAlgorithmDeriveBytes(bool publicParameters)
            : base(publicParameters)
        { }

        private PasswordDeriveBytes _externalAlgorithm;


        /// <summary>External algoritgm that will do the job.</summary>
        protected PasswordDeriveBytes ExternalAlgorithm { get { return _externalAlgorithm; } set { _externalAlgorithm = value; } }


        #region Operation


        /// <summary>Initializes eventual external algorithm used as proxy object to perform operations.</summary>
        protected override void InitializeExternalAlgorithm()
        {
            if (PasswordBytes != null)
            {
                if (SaltBytes != null)
                    ExternalAlgorithm = new PasswordDeriveBytes(PasswordBytes, SaltBytes, "SHA256", NumIterations);
                else
                    throw new ArgumentException("Salt is not provided for key generation algorithm.");
            } else if (PasswordString != null)
            {
                if (SaltBytes != null)
                    ExternalAlgorithm = new PasswordDeriveBytes(PasswordString, SaltBytes, "SHA256", NumIterations);
                else
                    throw new ArgumentException("Salt is not provided for key generation algorithm.");
            } else
            {
                throw new ArgumentException("Password is not provided for key generation algorithm, neither as string nor as byte array.");
            }
        }

        /// <summary>Clears eventual external algorithm used as proxy object to perform operations.</summary>
        protected override void ClearExternalAlgorithm()
        {
            ExternalAlgorithm = null;
        }

        /// <summary>Returns the next generated key of the specified length.
        /// <para>WARNING: This algorithm is weaker than the <see cref="PasswordAlgorithmRfc2898"/>.</para></summary>
        protected override void GetBytesInternal(int numBytes, ref byte[] outputBytes)
        {
            outputBytes = ExternalAlgorithm.GetBytes(numBytes);
        }

        #endregion Operation

    }  // class PasswordAlgorithmDeriveBytes


    /// <summary>Key generation "algorithm" that generates key as an (eventually padded and truncated) copy of 
    /// its original bytes.</summary>
    /// $A Igor jun15;
    public class PasswordAlgorithmNone : PasswordAlgorithmBase
    {

        public PasswordAlgorithmNone()
            : base()
        { }

        public PasswordAlgorithmNone(bool publicParameters)
            : base(publicParameters)
        { }

        #region Operation


        /// <summary>Initializes eventual external algorithm used as proxy object to perform operations.</summary>
        protected override void InitializeExternalAlgorithm()
        { }

        /// <summary>Clears eventual external algorithm used as proxy object to perform operations.</summary>
        protected override void ClearExternalAlgorithm()
        { }

        /// <summary>Returns the next generated key of the specified length.
        /// <para>WARNING: with this algorithm, all successively generated keys are the same and are directly 
        /// correlated to the initial state of the output. Key generation initialization parameters are not taken into account.</para></summary>
        /// <remarks>For the current algorithm, this method simply performs eventual cyclic padding or truncation on the initial
        /// value of the <paramref name="outputBytes"/> parameter, using the <see cref="UtilCrypto.PadToAlgorithmBlocksizeCyclic(byte[], int, bool)"/> method.</remarks>
        /// <param name="numBytes">Required length of the key.</param>
        /// <param name="outputBytes">Byte array where the generated key is stored.</param>
        protected override void GetBytesInternal(int numBytes, ref byte[] outputBytes)
        {
            if (outputBytes == null)
                throw new ArgumentException("Output of key derivation algorithm not specified; This class can only replicate and correct the provided bytes.");
            else if (outputBytes.Length < 4)
                Console.WriteLine("Output key is too short: " + outputBytes.Length + " bytes, should be at least " + 4);
            outputBytes = UtilCrypto.PadToAlgorithmBlocksizeCyclic(outputBytes, numBytes, true /* truncateIfLarger */ );
        }

        #endregion Operation

    }  // class PasswordAlgorithmNone




}  // namespace IG.Crypto



