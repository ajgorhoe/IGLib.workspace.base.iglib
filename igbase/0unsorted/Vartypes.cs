// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/



            /************************************/
            /*                                  */
            /*    INTERPRETER VARIABLE TYPES    */
            /*                                  */
            /************************************/


// REMARK: THIS IS CURRENTLY NOT USED AND WILL BE RESTRUCTURED! IT MAY EVEN BE REMOVED.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace IG.Lib
{


    // BASE CLASS FOR VARIABLE OBJECTS:

    /// <summary>DO NOT USE! The base class implementing the IVarType interface.</summary>
    class VarTypeBase : IVarType  // base class from which s variable classes are constructed. I
    {

        // IMPLEMENTATION OF ITypeName:

        public virtual string TypeName
        {
            get 
            {
                throw new NotSupportedException("Property TypeName is not available in the base class VarTypeBase.");
            }
        }

        public virtual string[] TypeNames
        {
            get 
            {
                throw new NotSupportedException("Property TypeNames is not available in the base class VarTypeBase."
                + " Calling object type ID: " + TypeName + ".");
            }
        }

        // Implementation of IXmlConvertable:

        public virtual object FromXmlElement(XmlElement source, ref object initial)
        {
            //TODO:
            // Implement this methods by using ony the TypeName property and IStringParsable interface!!! 
            throw new NotSupportedException ("Method FromXmlElement() is not available in the base class VarTypeBase."
                + " Calling object type ID: " + TypeName + ".");
        }

        public virtual XmlElement ToXmlElement(object source, XmlDocument doc, ref XmlElement initial)
        {
            //TODO:
            // Implement this methods by using ony the TypeName property and IStringParsable interface!!! 
            throw new NotSupportedException("Method FromXmlElement() is not available in the base class VarTypeBase."
                + " Calling object type ID: " + TypeName + ".");
        }

        // Implementation of IStringParsable:
        public string ConvertToString(object source, string format)
        {
            throw new NotSupportedException("Method ParseFromString() is not available in the base class VarTypeBase."
                + " Calling object type ID: " + TypeName + ".");
        }

        public object ParseFromString(string source,string format, int position, out int next)
        {
            throw new NotSupportedException("Method ParseFromString() is not available in the base class VarTypeBase."
                + " Calling object type ID: " + TypeName + ".");
        }

    }


    // BASE CLASS FOR SCALAR VARIABLES:

    class CounterVar : VarTypeBase
    {

        // STORAGE: 
        long _Value = 0;

        /// <summary>Gets or sets </summary>
        public long Value
        { get { return _Value; } set { _Value = value; } }


        // TODO: MAKE COMPLETE IMPLEMENTATION OF THIS CLASS!

        const string _TypeName = "counter";
        string [] _TypeNames = {_TypeName, "cont"} ;


        public override string TypeName
        {
            get { return _TypeName; }
        }

        public override string [] TypeNames
        {
            get { return _TypeNames; }
        }

        

    }


    /// <summary>Scalar variable.</summary>
    class ScalarVar : VarTypeBase
    {

        // STORAGE: 
        double _Value=0;

        /// <summary>Gets or sets </summary>
        public double Value
        { get { return _Value; }  set { _Value = value; } }

        // TODO: MAKE COMPLETE IMPLEMENTATION OF THIS CLASS!

        private static string _TypeName = "scalar";
        private static string[] _TypeNames = { _TypeName, "scal" };

        public override string TypeName
        { get { return _TypeName; } }

        public override string [] TypeNames
        { get { return _TypeNames; } }



    }


    // INTERFACES THAT MUST BE IMPLEMENTED IN THE VARIABLE TYPES:

    /// <summary>Classes that provide separate type names.
    /// Classes used e.g. in interpreter systems to represent types of variables, implement this interface.</summary>
    interface ITypeName
    {
        /// <summary>Gets the standard name of the type.</summary>
        string TypeName { get; }

        /// <summary></summary>
        string[] TypeNames { get; }
    }

    /// <summary>Defines classes whose instances can be converted to Xml elements.
    /// Conversion is different from that used in serialization.</summary>
    interface IFromXml : ITypeName
    {
        /// <summary>Creates a new object and initializes it from an XML element.</summary>
        /// <param name="source">XML elmeent containing contents of the object.</param>
        /// <param name="initial">Eventual existing object that can be used as storage storing the returnet object.
        /// The function is not obliged to use this object.</param>
        /// <returns>Object created and/or initialized from XML.</returns>
        object FromXmlElement(XmlElement source, ref object initial);
    }

    /// <summary>Defines classes whose instances can be created from Xml elements.
    /// Conversion is different from that used in serialization.</summary>
    interface IToXml : ITypeName
    {
        /// <summary>Creates and returns XML representation of an object.</summary>
        /// <param name="source">Object to be converted to XML.</param>
        /// <param name="doc">XML document used for eventual creation of an XML element.</param>
        /// <param name="initial">XML element on which contents of source can be stored.
        /// Function is not obliged to use it (e.g. it can always return a newly created object - this is left to implementation).</param>
        /// <returns>XML element, an XML representation of the object.</returns>
        XmlElement ToXmlElement(object source, XmlDocument doc, ref XmlElement initial);
    }


    /// <summary>Defines classes that can be converted to/from Xml elements.</summary>
    interface IXmlConvertable : IFromXml, IToXml { }


    /// <summary>Defines classes that can be converted to strings and whose values can be parsed from strings.
    /// This is used e.g. in interpreters and in variable storage systems.</summary>
    interface IStringParsable
    {
        string ConvertToString(object source, string format);
        object ParseFromString(string source,string format, int position, out int next);
    }

    /// <summary>Defines classes that can represent variables of different types.
    /// Used e.g. in interpreters and variable storing systems.</summary>
    interface IVarType : IXmlConvertable, IStringParsable
    {
    }
    
    
    
    
    class vartypes
    {
    }





}
