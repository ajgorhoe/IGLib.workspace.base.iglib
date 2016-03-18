// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/



/*********************************/
/*                               */
/*    EVENT RELATED UTILITIES    */
/*                               */
/*********************************/


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using System.Runtime.InteropServices;
using System.Drawing.Printing;
using System.Drawing.Imaging;



namespace IG.Forms
{



    #region EventArguments


    /// <summary>Event arguments for events bearing information about value that has changed.
    /// <para>Old and new values should normally be set via constructor.</para></summary>
    /// <typeparam name="ValueType">Type of the value that has changed.</typeparam>
    /// <remarks>Fields <see cref="New"/> and <see cref="Old"/> contain the new and the old value of the item
    /// whose value has changed. Either of them can be defined or not, and flags <see cref="NewDefined"/> and <see cref="OldDefined"/>
    /// specify that.</remarks>
    public class ValueChangeEventArgs<ValueType> : System.EventArgs
    {

        /// <summary>Default constructor, leaves old and new values unspecified.
        /// <para>To specify new and old values in constructor, use <see cref="ValuChangedEventArgs{ValueType}(ValueType, ValueType)"/>
        /// or <see cref="ValuChangedEventArgs{ValueType}(ValueType old, ValueType new, bool oldDefined, bool newDefined)"/></para></summary>
        public ValueChangeEventArgs() : base() { OldDefined = false; NewDefined = false; }

        /// <summary>Constructs event args with specific old and new value (<paramref name="oldValue"/> and <paramref name="newValue"/>).
        /// <para>Optional arguments <paramref name="oldValueDefined"/> and <paramref name="newValueDefined"/> can specify whether the
        /// values have actually been defined (default is true, so one can call constructor without these flags, implying that both
        /// old and new values are specified).</para></summary>
        /// <param name="oldValue">Old value of the item that has changed.</param>
        /// <param name="newValue"></param>
        /// <param name="oldValueDefined"></param>
        /// <param name="newValueDefined"></param>
        public ValueChangeEventArgs(ValueType oldValue, ValueType newValue, bool oldValueDefined = true, bool newValueDefined = true) : base()
        {
            this.Old = oldValue; this.New = newValue;
            this.OldDefined = oldValueDefined; this.NewDefined = newValueDefined;
        }

        /// <summary>Old value of the item that has changed.</summary>
        public ValueType Old { get; set; }

        /// <summary>Indicates whether the old value of the item is defined. 
        /// <para>It is possible that new or old value are not specified.</para></summary>
        public bool OldDefined { get; set; }

        /// <summary>New value of the item that has changed.</summary>
        public ValueType New { get; set; }

        /// <summary>Indicates whether the old value of the item is defined. 
        /// <para>It is possible that new or old value are not specified.</para></summary>
        public bool NewDefined { get; set; }

    }  // class ValuChangedEventArgs<ValueType>


    /// <summary>Event arguments for events bearing information about a value of type <see cref="double"/> that has changed.
    /// <para>Old and new values should normally be set via constructor.</para></summary>
    /// <remarks>Fields <see cref="New"/> and <see cref="Old"/> contain the new and the old value of the item
    /// whose value has changed. Either of them can be defined or not, and flags <see cref="NewDefined"/> and <see cref="OldDefined"/>
    /// specify that.
    /// <para>See also the generic base class, <see cref="ValueChangeEventArgs{ValueType}"/>.</para></remarks>
    public class ValueChangeEventArgs : ValueChangeEventArgs<double>
    {

        /// <summary>Constructs event arguments with old and new value unspecified.
        /// See <see cref="ValueChangeEventArgs{ValueType}ValueChangeEventArgs()"/>.
        /// <para></para></summary>
        ///public ValueChangeEventArgs() : base() { }

        /// <summary>Default constructor, leaves old and new values unspecified.
        /// <para>To specify new and old values in constructor, use <see cref="ValuChangedEventArgs(double, double)"/>
        /// or <see cref="ValuChangedEventArgs(double old, double new, bool oldDefined, bool newDefined)"/></para></summary>
        /// <remarks>See also <see cref="ValueChangeEventArgs{ValueType}()"/></remarks>
        public ValueChangeEventArgs() : base() { }

        /// <summary>Constructs event args with specific old and new value (<paramref name="oldValue"/> and <paramref name="newValue"/>).
        /// <para>Optional arguments <paramref name="oldValueDefined"/> and <paramref name="newValueDefined"/> can specify whether the
        /// values have actually been defined (default is true, so one can call constructor without these flags, implying that both
        /// old and new values are specified).</para>
        /// <para>See also <see cref="ValueChangeEventArgs{ValueType}()"/>.</para></summary>
        /// <param name="oldValue">Old value of the item that has changed.</param>
        /// <param name="newValue"></param>
        /// <param name="oldValueDefined"></param>
        /// <param name="newValueDefined"></param>
        public ValueChangeEventArgs(double oldValue, double newValue, bool oldValueDefined = true, bool newValueDefined = true) :
            base(oldValue, newValue, oldValueDefined, newValueDefined)
        { }

    }  // class ValueChangeEventArgs


    /// <summary>Event arguments for events bearing information about an index (of type <see cref="int"/>) that has changed.
    /// <para>Old and new values should normally be set via constructor.</para></summary>
    /// <remarks>Fields <see cref="New"/> and <see cref="Old"/> contain the new and the old value of the item
    /// whose value has changed. Either of them can be defined or not, and flags <see cref="NewDefined"/> and <see cref="OldDefined"/>
    /// specify that.
    /// <para>See also the generic base class, <see cref="ValueChangeEventArgs{ValueType}"/>.</para></remarks>
    public class IndexChangeEventArgs : ValueChangeEventArgs<int>
    {

        /// <summary>Constructs event arguments with old and new value unspecified.
        /// See <see cref="ValueChangeEventArgs{ValueType}ValueChangeEventArgs()"/>.
        /// <para></para></summary>
        ///public ValueChangeEventArgs() : base() { }

        /// <summary>Default constructor, leaves old and new values unspecified.
        /// <para>To specify new and old values in constructor, use <see cref="ValuChangedEventArgs(double, double)"/>
        /// or <see cref="ValuChangedEventArgs(int old, int new, bool oldDefined, bool newDefined)"/></para></summary>
        /// <remarks>See also <see cref="ValueChangeEventArgs{ValueType}()"/></remarks>
        public IndexChangeEventArgs() : base() { }

        /// <summary>Constructs event args with specific old and new value (<paramref name="oldValue"/> and <paramref name="newValue"/>).
        /// <para>Optional arguments <paramref name="oldValueDefined"/> and <paramref name="newValueDefined"/> can specify whether the
        /// values have actually been defined (default is true, so one can call constructor without these flags, implying that both
        /// old and new values are specified).</para>
        /// <para>See also <see cref="ValueChangeEventArgs{ValueType}()"/>.</para></summary>
        /// <param name="oldValue">Old value of the item that has changed.</param>
        /// <param name="newValue"></param>
        /// <param name="oldValueDefined"></param>
        /// <param name="newValueDefined"></param>
        public IndexChangeEventArgs(int oldValue, int newValue, bool oldValueDefined = true, bool newValueDefined = true) :
            base(oldValue, newValue, oldValueDefined, newValueDefined)
        { }

    }  // class ValueChangeEventArgs

    #endregion EventArguments


    #region EventHandlers

    /// <summary>Delegate for events that are triggered when ID of hte selected object changes.</summary>
    /// <param name="oldId">Old ID.</param>
    /// <param name="newId">New ID.</param>
    public delegate void SelectedIdEventHandler(int oldId, int newId);

    /// <summary>Delegate for events that are triggered when ID of hte selected object changes.</summary>
    /// <param name="oldId">Old value.</param>
    /// <param name="newId">New value.</param>
    public delegate void ValueChangedEventHandler(double oldValue, double newValue);


    #endregion EventHandlers

    


}
