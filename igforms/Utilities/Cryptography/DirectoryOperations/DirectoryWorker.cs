// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace IG.Lib
{


    public class DirectoryWorker : DirectoryWorker<DirectoryData, FileData>
    {  }


    /// <summary>Loops through directories and files and performs operations on them.
    /// <para>Informaton on worked directories and files is stored internally. In this way, objects of this class can be used to simply list
    /// <para></para>
    /// directory tree plus additional information on looped directories and files.</para>
    /// <para>Additional work can be performed by registeering events handlers for the provided events:</para>
    /// <para>  Before a file is worked </para>
    /// <para>  After a file is worked </para>
    /// <para>  Before a directory is worked </para>
    /// <para>  After a directory is worked </para>
    /// <para>Work can either be performed "online", simultaneously by traversing directory structure, or "offline", on data generated in the
    /// previous traversal of directory structure.</para>
    /// <para></para></summary>
    /// $A Igor Apr17;
    public class DirectoryWorker<DirectoryDataType, FileDataType>
        where DirectoryDataType: DirectoryData<DirectoryDataType, FileDataType>, new()
        where FileDataType: FileData<DirectoryDataType, FileDataType>, new()
    {

        #region Properties

        /// <summary>File filter used when listing files in directories and subdirectories.</summary>
        public string FileFilter { get; set; } = null;

        /// <summary>File exclude filter used when listing files in directories and subdirectories.</summary>
        public string FileExcludeFilter { get; set; } = null;

        /// <summary>Directory filter used when listing files in directories and subdirectories.</summary>
        public string DirectoryFilter { get; set; } = null;

        /// <summary>Directory exclude filter used when listing files in directories and subdirectories.</summary>
        public string DirectoryExcludeFilter { get; set; } = null;


        bool _isRelativePaths = false;

        /// <summary>Whether relative paths are used. Ig true then base path must be defined, and 
        /// it will be set to current directory if not set explicitly.</summary>
        public bool IsRelativePathsUsed
        {
            get { return _isRelativePaths; }
            set { _isRelativePaths = value; }
        }

        string _basePath = null;

        /// <summary>Base path for the data about files and directories stored on and worked by the current <see cref="DirectoryWorker{DirectoryDataType, FileDataType"/> 
        /// object.</summary>
        public virtual string BasePath
        {
            get
            {
                if (IsRelativePathsUsed && string.IsNullOrEmpty(_basePath))
                {
                    lock (this);
                    BasePath = Directory.GetCurrentDirectory();
                }
                return _basePath;
            }
            protected set
            {
                _basePath = value;
            }
        }


        /// <summary>Gets a value indicating whether base path is defined on the current <see cref="DirectoryWorker{DirectoryDataType, FileDataType"/>
        /// object.</summary>
        public bool IsBasePathDefined
        {
            get { return (!string.IsNullOrEmpty(_basePath)); }
        }


        #region PrimaryDirectories

        // PRIMARY DIRECTORIES WORKED ON:

        List<DirectoryDataType> _primaryDirectories = null;

        /// <summary>Primary directories that are initially set to be stored on and worked by the current <see cref="DirectoryWorker{DirectoryDataType, FileDataType"/> 
        /// object.</summary>
        public virtual List<DirectoryDataType> PrimaryDirectories
        {
            get {
                if (_primaryDirectories == null)
                {
                    lock(this)
                    {
                        if (_primaryDirectories == null)
                        {
                            _primaryDirectories = new List<DirectoryDataType>();
                        }
                    }
                }
                return _primaryDirectories;
           }
        }

        public int NumPrimaryDirectories { get { return _primaryDirectories == null ? 0 : PrimaryDirectories.Count; } }



        /// <summary>Adds the specified directories to the list of primary directories stored on and worked by the current <see cref="DirectoryWorker{DirectoryDataType, FileDataType"/> 
        /// object.</summary>
        /// <param name="directories">Directories that are added to the list of primary directories.</param>
        protected virtual void AddPrimaryDirectories(params DirectoryDataType[] directories)
        {
            if (directories != null)
            {
                foreach (DirectoryDataType dir in directories)
                {
                    PrimaryDirectories.Add(dir);
                }
            }
        }

        /// <summary>Adds directories witht the specified paths to the list of primary directoires that are listed and worked on by
        /// the current object.</summary>
        /// <param name="directoryPaths"></param>
        public void AddPrimaryDirectories(params string[] directoryPaths)
        {
            if (directoryPaths != null)
            {
                foreach (string path in directoryPaths)
                {
                    AddPrimaryDirectories(CreateDirectoryData(path));
                }
            }
        }


        /// <summary>Creates and returns a new directory data object for the specified path.</summary>
        /// <param name="path">Path of the directory for which directory data object should be created.</param>
        public DirectoryDataType CreateDirectoryData(string path)
        {
            DirectoryDataType ret = null;
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Can not create directory data: path not specified (null or empty string).");
            UtilSystem.GetAbsolutePath(path);
            DirectoryInfo info = new DirectoryInfo(path);
            if (info == null || !info.Exists)
                throw new ArgumentException("Can ot create directory data object: directory does not exist." + Environment.NewLine
                    + "  Specified directory path: " + path + (IsBasePathDefined? 
                    (Environment.NewLine + "  Base path: " + BasePath): "" ) );
            ret = new DirectoryDataType();
            if (IsBasePathDefined)
            {
                ret.BasePath = BasePath;
                if (IsRelativePathsUsed)
                    ret.RelativePath = UtilSystem.GetRelativePath(BasePath, info.FullName);
            }
            ret.SetDirectoryInfo(info, updateDirectoryInformation: true, refreshBefore: false);
            return ret;
        }



        /// <summary>Clears the list of primary directories stored on and worked by the current <see cref="DirectoryWorker{DirectoryDataType, FileDataType"/> 
        /// object.</summary>
        protected virtual void ClearPrimaryDirectories()
        {
            PrimaryDirectories.Clear();
        }

        public int NumWorkedDirectories
        { get { return PrimaryDirectories.Count; } }


        #endregion PrimarzDirectories


        #region PrimaryFiles

        // PRIMARY FILES WORKED ON:


        List<FileDataType> _primaryFiles = null;

        /// <summary>Primary files that are initially set to be stored on and worked by the current <see cref="DirectoryWorker{DirectoryDataType, FileDataType"/> 
        /// object.</summary>
        public virtual List<FileDataType> PrimaryFiles
        {
            get
            {
                if (_primaryFiles == null)
                {
                    lock (this)
                    {
                        if (_primaryFiles == null)
                        {
                            _primaryFiles = new List<FileDataType>();
                        }
                    }
                }
                return _primaryFiles;
            }
        }

        /// <summary>Number of primary files.</summary>
        public int NumPrimaryFiles { get { return _primaryFiles == null ? 0 : PrimaryFiles.Count; } }



        /// <summary>Adds the specified files to the list of primary files stored on and worked by the current <see cref="DirectoryWorker{DirectoryDataType, FileDataType"/> 
        /// object.</summary>
        /// <param name="files">Files that are added to the list of primary files.</param>
        protected virtual void AddPrimaryFiles(params FileDataType[] files)
        {
            if (files != null)
            {
                foreach (FileDataType file in files)
                {
                    PrimaryFiles.Add(file);
                }
            }
        }

        /// <summary>Adds files witht the specified paths to the list of primary files that are listed and worked on by
        /// the current object.</summary>
        /// <param name="filePaths"></param>
        public void AddPrimaryFiles(params string[] filePaths)
        {
            if (filePaths != null)
            {
                foreach (string path in filePaths)
                {
                    AddPrimaryFiles(CreateFileData(path));
                }
            }
        }


        /// <summary>Creates and returns a new file data object for the specified path.</summary>
        /// <param name="path">Path of the file for which file data object should be created.</param>
        public FileDataType CreateFileData(string path)
        {
            FileDataType ret = null;
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Can not create file data: path not specified (null or empty string).");
            UtilSystem.GetAbsolutePath(path);
            FileInfo info = new FileInfo(path);
            if (info == null || !info.Exists)
                throw new ArgumentException("Can ot create file data object: file does not exist." + Environment.NewLine
                    + "  Specified file path: " + path + (IsBasePathDefined ?
                    (Environment.NewLine + "  Base path: " + BasePath) : ""));
            ret = new FileDataType();
            if (IsBasePathDefined)
            {
                ret.BasePath = BasePath;
                if (IsRelativePathsUsed)
                    ret.RelativePath = UtilSystem.GetRelativePath(BasePath, info.FullName);
            }
            ret.SetFileInfo(info, updateFileInformation: true, refreshBefore: false);
            return ret;
        }



        /// <summary>Clears the list of primary files stored on and worked by the current <see cref="DirectoryWorker{DirectoryDataType, FileDataType"/> 
        /// object.</summary>
        protected virtual void ClearPrimaryFiles()
        {
            PrimaryDirectories.Clear();
        }

 

        //  Directory Directory Directory Directories Directories directory directory directories directories 


        #endregion PrimayFiles 


        #endregion Properties

        /// <summary>Returns string representation of the current object.</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Directory worker:");
            sb.AppendLine("  Relative paths: " + IsRelativePathsUsed);
            if (IsRelativePathsUsed)
            {
                if (string.IsNullOrEmpty(BasePath))
                    sb.AppendLine("    WARNING: Base path not specified.");
                else
                    sb.AppendLine("    Base path: " + BasePath);
            }
            else
            {
                if (!string.IsNullOrEmpty(BasePath))
                    sb.AppendLine("  Warning: base path defined:" + Environment.NewLine + "    " + BasePath);
            }
            return sb.ToString();
        }

    }




}
