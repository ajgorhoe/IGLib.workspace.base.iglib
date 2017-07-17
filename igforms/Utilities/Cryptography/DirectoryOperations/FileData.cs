// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/


// BASIC FILE DATA CLASSES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace IG.Lib
{



    /// <summary>Basic file data class.
    /// <para>Forms a group with the corresponding directory data class <see cref="DirectoryData"/>,
    /// and derives from <see cref="FileData{DirectoryDataType, FileDataType}"/>.</para></summary>
    public class FileData : FileData<DirectoryData, FileData>
    {  }


    /// <summary>Generic base class for file data classes (derived from <see cref="FileDataBaseObsolete"/>) that form a group with 
    /// corresponding directory data class.</summary>
    /// <typeparam name="DirectoryDataType">Specific type used for directory data (derives fom <see cref="DirectoryDataBase"/>).</typeparam>
    /// <typeparam name="FileDataType">Specific type for file data (derives from <see cref="FileDataBaseObsolete"/>).</typeparam>
    public class FileData<DirectoryDataType, FileDataType> : FileOrDirectoryDataBase
        where DirectoryDataType : DirectoryData<DirectoryDataType, FileDataType>, new()
        where FileDataType : FileData<DirectoryDataType, FileDataType>, new()
    {


        #region Properties.Basic


        long? _length = null;

        /// <summary>File length.
        /// <para>This property is nullable, null meaning that there is no information.</para></summary>
        public long? Length { get { return _length; } set { _length = value; } }



        FileInfo _info = null;

        /// <summary>System provided information about the file, of type <see cref="FileInfo"/>.</summary>
        public virtual FileInfo Info
        {
            get
            {
                return _info;
            }
            protected set
            {
                if (value != null && value != _info)
                {
                    _info = value;
                    SetFileInfo(value);
                }
                else
                {
                    _info = value;
                }
            }
        }

        /// <summary>Sets information about current file from the specified <see cref="FileSystemInfo"/> object.</summary>
        /// <param name="info">File information object that is set.</param>
        /// <param name="updateFileData">Indicates whether data on the current object should be updatedaccording to <paramref name="info"/>. 
        /// <param name="refreshBefore">Indicates whether Refresh() should be called on the <paramref name="info"/> parameter before other parts of operation.
        /// <para>Default is true.</para></param>
        /// copying data from the object.</param>
        public virtual void SetFileInfo(FileInfo info, bool updateFileInformation = true, bool refreshBefore = false)
        {
            if (info != null)
            {
                if (refreshBefore)
                    info.Refresh();
                if (updateFileInformation)
                {
                    if (info.DirectoryName != null)
                        ParentAbsolutePath = info.DirectoryName;
                    base.SetFileOrDirectoryInfo(info);
                    this.Length = info.Length;
                }
            }
        }


        #endregion Properties.Basic



        public FileDataType ComparedFile { get; set; } = null;



    }  // class FileData<DirectoryDataType, FileDataType>









}
