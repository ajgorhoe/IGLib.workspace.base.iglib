using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

using IG.Lib;
using System.IO;



/// <summary>Interface implemented by file viewers (usually GUI components).</summary>
namespace IG.Forms
{
    public interface IFileViewer
    {

        /// <summary>Gets or sets path to the file or URL of the file that is currently being viewed.
        /// <para>Whether it is actually shown depends on the <see cref="IsShownImmediately"/> flag).</para></summary>
        string ViewedFile
        { get; set; }

        /// <summary>Gets or sets the stream containing the file that is currently being viewed.
        /// <para>Whether it is actually shown depends on the <see cref="IsShownImmediately"/> flag).</para></summary>
        Stream ViewedStream
        { get; set; }

        /// <summary>Flag that specifies whether content is shown / played immediately after the viewed object
        /// is set.
        /// <para>If set to true and the viewer does not at all have such capability, this should NOT throw
        /// an exception (thus, user should check success by verifying if flag changed).</para></summary>
        bool IsShownImmediately { get; set; }

        /// <summary>Shows / plays the viewed contents.
        /// <para>Can have the role of refresh when <see cref="IsShownImmediately"/> is true, or is used to
        /// actually show the contents after assigning it.</para></summary>
        void ShowViewedItem();

        /// <summary>Clears the viewer and eventually releases any resources used.
        /// <para>Viewed entities are unset.</para></summary>
        void Clear();

        /// <summary>Whether the current viewer can view files from the file system.</summary>
        bool CanViewFiles
        { get; set; }

        /// <summary>Whether the current viewer can view web resources (files on the internet).</summary>
        bool CanViewUrls
        { get; set; }

        /// <summary>Whether the current viewer can read streams.</summary>
        bool CanViewStreams
        { get; set; }

        /// <summary>Rerurns a flag indicating whether the file (or web resource) at the specified location is eligible 
        /// for vieiwnig in the current viewer or not.</summary>
        /// <param name="fileLocation">Path to the file on a file system, or file URL.</param>
        /// <returns>True if the file can be viewed in the current viewer, false otherwise.</returns>
        bool IsEligibleForViewing(string fileLocation);

        /// <summary>Returnns a flag indicating whether the specified stream is eligible for vieiwnig or not.</summary>
        /// <param name="stream">Path to the file on a file system, or file URL.</param>
        /// <returns>True if the stream can be viewed in the current viewer, false otherwise.</returns>
        bool IsEligibleForViewing(Stream stream);

        /// <summary>Gets / sets a flag that specified whether the viewer's controls are visible or not.
        /// <para>Setting should have immediate effect and make controls visible / invisible.</para></summary>
        /// <remarks>This is important because viewers must be able to be used embedded in other controls, where 
        /// all behavior is handled by those controls and viewer just provides the viewing area.</remarks>
        bool IsControlsVisible
        { get; set; }

        /// <summary>Opens a browser to select the file shown.
        /// <para>Available only when <see cref="IsBrowsable"/> property is true.</para></summary>
        void BrowseFile();

        /// <summary>Gets or sets the flag specifying whether one can browse for the file to be opened (e.g. by
        /// using the file selection dialog).
        /// <para>If the viewer does not at all have browser capability then setting to true should not throw an 
        /// exception (therefore, the user should verify by getter if setting actually worked).</para>
        /// <para>Setter can be used to disable drag & drop (e.g. for security, or when used as embedded control
        /// and drag & drop would be disturbing). If the viewer does not at all have drag & drop capability, setting
        /// this flag to true should NOT throw an exception (thus, user should check if it was successful).</para></summary>
        bool IsBrowsable
        { get; set; }

        /// <summary>Whether the viewer provides drag & drop capability.
        /// <para>If the viewer does not at all have browser capability then setting to true should not throw an 
        /// exception (therefore, the user should verify by getter if setting actually worked).</para>
        /// <para>Setter can be used to disable drag & drop (e.g. for security, or when used as embedded control
        /// and drag & drop would be disturbing). If the viewer does not at all have drag & drop capability, setting
        /// this flag to true should NOT throw an exception (thus, user should check if it was successful).</para></summary>
        bool IsDragAndDrop
        { get; set; }


    }



}
