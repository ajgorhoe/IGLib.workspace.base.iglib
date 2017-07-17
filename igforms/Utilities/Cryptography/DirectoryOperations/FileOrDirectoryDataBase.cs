// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


using IG.Crypto;


namespace IG.Lib
{



    /// <summary>Base class for storing file- or directory- related data.
    /// <para>Main purpose is to serve as base class for <see cref="FileDataBaseObsolete"/> and <see cref="DirectoryDataBase"/></para>
    /// <para>This class provides definitions that are common for file and directory data classes.</para></summary>
    /// <remarks>
    /// <para>For this and related classes, basic path information consists of file or directory name, and either parent directory
    /// absolute path or base path plus parent directory relative path. In this way, path is provided in dcomposed way.</para>
    /// <para>Derived path information is always calculated on demand and is cached</para>
    /// </remarks>
    /// $A Igor Apr17;
    public abstract class FileOrDirectoryDataBase
    {

        #region Properties

 
        string _name = null;

        /// <summary>Name of the file or directory (without any path information) for which the current object contains data.</summary>
        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    if (!string.IsNullOrEmpty(AbsolutePath))
                        _name = Path.GetFileName(AbsolutePath);
                }
                return _name;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != Name)
                {
                    // Invalidate Depenndencies
                    {
                        AbsolutePath = null;
                        RelativePath = null;
                    }
                    _name = value;
                }
            }
        }

        string _parentAbsolutePath = null;

        /// <summary>Absolute path of the parent directory that contains the current file or directory..</summary>
        public virtual string ParentAbsolutePath
        {
            get
            {
                if (string.IsNullOrEmpty(_parentAbsolutePath))
                {
                    if (IsBasePathAbsolute && !string.IsNullOrEmpty(BasePath) && !string.IsNullOrEmpty(ParentRelativePath))
                        _parentAbsolutePath = Path.Combine(BasePath, ParentRelativePath);
                    else if (!string.IsNullOrEmpty(AbsolutePath))
                        _parentAbsolutePath = Path.GetDirectoryName(AbsolutePath);
                }
                return _parentAbsolutePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != ParentAbsolutePath)
                {
                    // Invalidate Depenndencies
                    {
                        AbsolutePath = null;
                        if (!string.IsNullOrEmpty(BasePath) && IsBasePathAbsolute)
                        {
                            ParentRelativePath = null;
                            RelativePath = null;
                        }
                    }
                }
                _parentAbsolutePath = value;
            }
        }

        bool _isBasePathAbsolute = true;

        /// <summary>Whether base path is an absolute path.</summary>
        public virtual bool IsBasePathAbsolute
        {
            get
            {
                if (string.IsNullOrEmpty(BasePath))
                    _isBasePathAbsolute = false;
                return _isBasePathAbsolute;
            }
            set
            {
                if (value != _isBasePathAbsolute)
                {
                    _isBasePathAbsolute = value;
                }
            }
        }

        string _basePath = null;

        /// <summary>Path (usually absolute) of the directory</summary>
        public virtual string BasePath
        {
            get { return _basePath; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != BasePath && !string.IsNullOrEmpty(BasePath))
                {
                    // Invalidate dependencies:
                    if (!string.IsNullOrEmpty(ParentRelativePath))
                    {
                        ParentAbsolutePath = null;
                        AbsolutePath = null;
                    }
                }
                _basePath = value;
            }
        }

        string _parentRelativePath;

        /// <summary>Relative path (with respect to base directory) of the parent directory that contains the current file or directory.</summary>
        public virtual string ParentRelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(_parentRelativePath))
                {
                    if ((!string.IsNullOrEmpty(BasePath) && IsBasePathAbsolute) && !string.IsNullOrEmpty(ParentAbsolutePath))
                    {
                        _parentRelativePath = UtilSystem.GetRelativePath(BasePath, ParentAbsolutePath);
                    }
                }
                return _parentRelativePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != ParentRelativePath && !string.IsNullOrEmpty(ParentRelativePath))
                {
                    // Invalidate dependencies:
                    RelativePath = null;
                    if (!string.IsNullOrEmpty(BasePath) && IsBasePathAbsolute)
                    {
                        ParentAbsolutePath = null;
                        AbsolutePath = null;
                    }

                }
                _parentRelativePath = value;
            }
        }

        #region Properties.Dependent

        string _absolutePath = null;

        /// <summary>Full (absolute) path of the current file.
        /// <para>This is dependent property.</para></summary>
        public virtual string AbsolutePath
        {
            get
            {
                if (string.IsNullOrEmpty(_absolutePath))
                {
                    if (!string.IsNullOrEmpty(ParentAbsolutePath) && !string.IsNullOrEmpty(Name))
                        _absolutePath = Path.Combine(ParentAbsolutePath, Name);
                }
                return _absolutePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != AbsolutePath)
                {
                    ParentAbsolutePath = Path.GetDirectoryName(value);
                    Name = Path.GetFileName(value);
                }
                _absolutePath = value;
            }
        }

        string _relativePath = null;

        /// <summary>Relative path of the current file or directory (includes file or directory name).
        /// <para>This is dependent property.</para></summary>
        public virtual string RelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(_relativePath))
                {
                    if (!string.IsNullOrEmpty(ParentRelativePath) && !string.IsNullOrEmpty(Name))
                        _relativePath = Path.Combine(ParentRelativePath, Name);
                }
                return _relativePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != RelativePath)
                {
                    ParentRelativePath = Path.GetDirectoryName(value);
                    Name = Path.GetFileName(value);
                }
                _relativePath = value;
            }
        }

        #endregion Properties.Dependent



        bool? _exists = false;

        /// <summary>Whether the file/directory exists. Dafault is false.
        /// <para>This property is nullable, null meaning that there is no information.</para>
        /// <para>File or directory data object can be manipulated if the corresponding file or directory does not existt
        /// (i.e., to store informaton about previous state of apart of file system). This flag is used to specify what
        /// is actually the case (whether file or directory actually exists) but may not be used in some scenarios.</para></summary>
        public virtual bool? Exists
        {
            get
            {
                return _exists;
            }
            set
            {
                _exists = value;
            }
        }



        DateTime? _creationTime;

        /// <summary>Creation time of file or directory.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public virtual DateTime? CreationTime { get { return _creationTime; } set { _creationTime = value; } }



        DateTime? _lastWriteTime;

        /// <summary>Creation time of file or directory.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public virtual DateTime? LastWriteTime { get { return _lastWriteTime; } set { _lastWriteTime = value; } }


        DirectoryData _parentBase;

        /// <summary><see cref="DirectoryDataBase"/> object that contains the current file or directory object.
        /// <para>This properties enables setting container object in uniform way for derived classes. In this way, container
        /// object that contains at least basic data about file or directory, can always be sett.</para></summary>
        public virtual DirectoryData ContainerBase
        {
            get { return _parentBase; }
            set { _parentBase = value; }
        }


        /// <summary>Sets information about current file or directory from the specified <see cref="FileSystemInfo"/> object.</summary>
        /// <param name="info">Object from which information is eventually copied.</param>
        protected void SetFileOrDirectoryInfo(FileSystemInfo info)
        {
            if (info != null)
            {
                if (info.FullName != null)
                    this.AbsolutePath = info.FullName;
                if (info.Name != null)
                    this.Name = info.Name;
                this.CreationTime = info.CreationTime;
                this.LastWriteTime = info.LastWriteTime;
            }
        }


        Dictionary<HashType, string> _hashes;


        protected Dictionary<HashType, string> Hashes
        {
            get
            {
                if (_hashes == null)
                {
                    lock(this)
                    {
                        if (_hashes == null)
                            _hashes = new Dictionary<Crypto.HashType, string>();
                    }
                }
                return _hashes;
            }
        }

        /// <summary>Returns a value indicating whether a hash (checksum) of the file or directory represented by the 
        /// current object of the specified type is available (contained on the current object).</summary>
        /// <param name="hashType">Type of hash for which information is queried.</param>
        public bool IsHashAvailable(HashType hashType)
        {
            return Hashes.ContainsKey(hashType);
        }

        /// <summary>Retrns hash (checksum) of the file or directory represented by the current object of the 
        /// specified type, if such hash is calculated, or null if such a has is not calculated.
        /// <para>Returned hash is in hexadecimal form.</para></summary>
        /// <param name="hashType">Type of hash to be returned. Must not be <see cref="HashType.None"/>.</param>
        public string GetHash(HashType hashType)
        {
            if (hashType == HashType.None)
                throw new ArgumentException("Can not get file or directory hash (checksum): type is not specified.");
            else
            {
                Dictionary<HashType, string> hashes = this.Hashes;
                if (hashes.ContainsKey(hashType))
                    return hashes[hashType];
                else
                    return null;
            }
        }


        /// <summary>Sets the hash value of the specified type (for the file or directory represented by the current object)
        /// to the specified value.</summary>
        /// <param name="hashType">Type of hash to be returned. Must not be <see cref="HashType.None"/>.</param>
        /// <param name="hashValue">Value to which hasj of the specified type is set. 
        /// <para>If null then hash of this type is removed from the object, if it exists on it.</para></param>
        public void SetHash(HashType hashType, string hashValue)
        {
            if (hashType == HashType.None)
                throw new ArgumentException("Can not set file or directory hash (checksum): type is not specified.");
            if (hashValue == null)
            {
                RemoveHash(hashType);
            } else
            {
                Hashes[hashType] = hashValue;
            }
        }


        /// <summary>Removes hash of the specified type from the current object.
        /// <para>If hash of the specified type does not exist on the current object, this method has no effect.</para></summary>
        /// <param name="hashType">Type of hash that is to be removed. Can be <see cref="HashType.None"/>.</param>
        public void RemoveHash(HashType hashType)
        {
            if (Hashes.ContainsKey(hashType))
                Hashes.Remove(hashType);
        }



        #endregion Properties

    }

}
