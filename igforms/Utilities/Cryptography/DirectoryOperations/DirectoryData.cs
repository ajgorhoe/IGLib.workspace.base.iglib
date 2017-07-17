// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/


// BASIC DIRECTORY DATA CLASSES


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;


namespace IG.Lib
{



    /// <summary>Basic directory data class.
    /// <para>Forms a group with the corresponding file data class <see cref="FileData"/>,
    /// and derives from <see cref="FileData{DirectoryDataType, FileDataType}"/>.</para></summary>
    public class DirectoryData: DirectoryData<DirectoryData, FileData>
    {

        void Test()
        {
            Files = new List<FileData>();
        }

    }


    /// <summary>Generic base class for file data classes (derived from <see cref="FileDataBaseObsolete"/>) that form a group with 
    /// corresponding directory data class.</summary>
    /// <typeparam name="DirectoryDataType">Specific type used for directory data (derives fom <see cref="DirectoryDataBase"/>).</typeparam>
    /// <typeparam name="FileDataType">Specific type for file data (derives from <see cref="FileDataBaseObsolete"/>).</typeparam>
    public class DirectoryData<DirectoryDataType, FileDataType> : FileOrDirectoryDataBase
        where DirectoryDataType : DirectoryData<DirectoryDataType, FileDataType>, new()
        where FileDataType : FileData<DirectoryDataType, FileDataType>, new()
    {


        #region Properties.ContainedItems

        private bool _hasAllFiles = false;

        /// <summary>Indicates whether the current directory data object tontains information for all files contained in the represented directory.
        /// <para>If false then the <see cref="Files"/> may contain only part (or none) of files contained inn the directory.</para>
        /// <para>Information provided by this properrty is indicative and may be used by various tools working on information about directory structure.</para></summary>
        public bool HasAllFiles
        {
            get { return _hasAllFiles; }
            protected set { _hasAllFiles = value; }
        }


        private bool _hasAllDirectories = false;

        /// <summary>Indicates whether the current directory data object tontains information for all files contained in the represented directory.
        /// <para>If false then the <see cref="Directories"/> may contain only part (or none) of files contained inn the directory.</para>
        /// <para>Information provided by this properrty is indicative and may be used by various tools working on information about directory structure.</para></summary>
        public bool HasAllDirectories
        {
            get { return _hasAllDirectories; }
            protected set { _hasAllDirectories = value; }
        }



        List<DirectoryDataType> _directories = null;

        /// <summary>Returns array of data objects for all directories contained in this directory for which information exists.
        /// <para>This does not necessarily provide info for all files contained in the directory represented by the current object, 
        /// but only those for which information is available.</para></summary>
        public List<DirectoryDataType> Directories
        {
            get
            {
                if (_directories == null)
                    _directories = new List<DirectoryDataType>();
                return _directories;
            }
            protected set { Directories = value; }
        }

        List<FileDataType> _files = null;

        /// <summary>Returns array of data objects for all files contained in this directory for which information exists.
        /// <para>This does not necessarily provide info for all files contained in the directory represented by the current object, 
        /// but only those for which information is available.</para></summary>
        public List<FileDataType> Files
        {
            get
            {
                if (_files == null)
                    _files = new List<FileDataType>();
                return _files;
            }
            set { _files = value; }
        }




        long? _sizePartial = null;

        /// <summary>Total size of all files directly contained in a directory.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public long? SizePartial { get { return _sizePartial; } set { _sizePartial = value; } }

        long? _sizeTotal = null;

        /// <summary>Total size of all files contained in a directory and all its subdirectories.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public long? SizeTotal { get { return _sizeTotal; } set { _sizeTotal = value; } }


        int? _numFilesPartial = null;

        /// <summary>Number of files directly contained in a directory.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public int? NumFilesPartial { get { return _numFilesPartial; } set { _numFilesPartial = value; } }


        int? numFilesTotal = null;

        /// <summary>Total number of all files contained in a directoryor its subdirectories.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public int? NumFilesTotal { get { return numFilesTotal; } set { numFilesTotal = value; } }


        int? _numDirectoriesPartial = null;

        /// <summary>Number of directories directly contained in a directory.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public int? NumDirectoriesPartial { get { return _numDirectoriesPartial; } set { _numDirectoriesPartial = value; } }

        int? _numDirectoriesTotal = null;

        /// <summary>Total number of all directories directly or indirectly contained in a directory, incliding neste subdirectories.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public int? NumDirectoriesTotal { get { return _numDirectoriesTotal; } set { _numDirectoriesTotal = value; } }




        #endregion Properties.ContainedItems


        #region Properties.Basic

        DirectoryInfo _info = null;

        /// <summary>System provided information about the file of type <see cref="FileInfo"/>.</summary>
        public virtual DirectoryInfo Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
            }
        }


        /// <summary>Sets information about current file from the specified <see cref="FileSystemInfo"/> object.</summary>
        /// <param name="info">File information object that is set.</param>
        /// <param name="updateDirectoryInformation">Indicates whether data on the current object should be updatedaccording to <paramref name="info"/>. 
        /// <param name="refreshBefore">Indicates whether Refresh() should be called on the <paramref name="info"/> parameter before other parts of operation.
        /// <para>Default is true.</para></param>
        /// copying data from the object.</param>
        public virtual void SetDirectoryInfo(DirectoryInfo info, bool updateDirectoryInformation = true, bool refreshBefore = false)
        {
            if (info != null)
            {
                if (refreshBefore)
                    info.Refresh();
                if (updateDirectoryInformation)
                {
                    DirectoryInfo parent = info.Parent;
                    if (parent != null)
                    {
                        if (parent.FullName != null)
                            ParentAbsolutePath = info.Parent.FullName;
                    }
                    base.SetFileOrDirectoryInfo(info);
                }
            }
        }


        #endregion Properties.Basic


        #region Properties.Auxiliary


        public DirectoryData ComparedDirectory { get; set; } = null;


        #endregion Properties.Auxiliary



    }  // class DirectoryData<DirectoryDataType, FileDataType>



}
