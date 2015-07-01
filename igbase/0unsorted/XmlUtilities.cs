// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/



                  /***********************/
                  /*                     */
                  /*    XML Utilities    */
                  /*                     */
                  /***********************/



using System;
// using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace IG.Lib
{


    /// <summary>
    /// XML-based data class.
    /// Doc container is accessed through the Doc property, which is of class XmlData, an extension of XmlDocument.
    /// </summary>
    public class DataStore
    {

        // DATA CONTAINER OF THE DOCUMENT
        
        private XmlData _Data=null;
        private bool _DataChanged = false;



        /// <summary>Gets or sets the class' data storage.
        /// This is normally not accesst directly, but through s methods.</summary>
        public virtual XmlData Doc
        {
            get { return _Data; }
            set { _Data = value; }
        }

        /// <summary>Gets the indicator of whether data contents have been changed and not saved since loaded.</summary>
        public bool DataUnsaved
        {
            get { return _DataChanged; }
            protected set { _DataChanged = value; }
        }



        // DATA LOADING: 
        #region Data_Loading

        /// <summary>Loads data store'result XML Doc from a file.</summary>
        /// <param name="filepath">Name of the file from which XML is loaded.</param>
        /// <param name="forceoverwrite">If true then the file is loaded also if the data has been modified but not saved.</param>
        public void Load(string filepath,bool forceoverwrite)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentException("The XML file to load the data is not specified.");
            if (!File.Exists(filepath))
                throw new ArgumentException("The XML file to load the data does not wxist. File: "+filepath+".");
            if (!forceoverwrite && DataUnsaved)
                throw new Exception("Data has been modified but not saved. To overwrite the existing data, call DataStore.Load(...,true)!");
            _Data = new XmlData();
            try
            {  _Data.Load(filepath);  }
            catch (Exception ex)
            {
                throw new Exception("Data could not be loaded form aa file. Details: " + ex.Message);
            }
            // Re-set the data modification indicator:
            DataUnsaved = false;
        }

        /// <summary>Loads data store'result XML Doc from a file.
        /// An exception is thrown if the there is modified and unsaved data.</summary>
        /// <param name="filepath">Name of the file from which XML is loaded.</param>
        public void Load(string filepath)
        {
            Load(filepath, false /* throw exception if data is already loaded. */ );
        }


        /// <summary>Loads data store'result XML Doc from an XML string.</summary>
        /// <param name="str">XML string from which XML is loaded.</param>
        /// <param name="forceoverwrite">If true then the file is loaded also if the data has been modified but not saved.</param>
        public void LoadFromString(string str, bool loadifnotnull)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("The XML string to load the data is empty.");
            if (!loadifnotnull && _Data == null)
                throw new Exception("Data is already imported. To overwrite the existing data, call DataStore.Load(true)!");
            _Data = new XmlData();
            try
            {
                _Data.LoadXml(str);
            }
            catch (Exception ex)
            {
                throw new Exception("Data could not be loaded form the string. Details: " + ex.Message);
            }
            DataUnsaved = false;
        }

        /// <summary>Loads data store'result XML Doc from an XML string.
        /// An exception is thrown if the there is modified and unsaved data.</summary>
        /// <param name="str">XML string from which XML is loaded.</param>
        public void LoadFromString(string filename)
        {
            Load(filename, false /* throw exception if data is already loaded. */ );
        }


        /// <summary>Loads data store'result XML Doc from an input stream.</summary>
        /// <param name="inStream">Input stream from which XML is loaded.</param>
        /// <param name="forceoverwrite">If true then the file is loaded also if the data has been modified but not saved.</param>
        public void Load(Stream inStream, bool forceoverwrite)
        {
            if (inStream==null)
                throw new ArgumentException("The input stream to load the data is not specified.");
            if (!forceoverwrite && DataUnsaved)
                throw new Exception("Data has been modified but not saved. To overwrite the existing data, call DataStore.Load(...,true)!");
            _Data = new XmlData();
            try { _Data.Load(inStream); }
            catch (Exception ex) {
                throw new Exception("Data could not be loaded form an input stream. Details: " + ex.Message);
            };
            // Re-set the data modification indicator:
            DataUnsaved = false;
        }


        /// <summary>Loads data store'result XML Doc from an input stream.
        /// An exception is thrown if the there is modified and unsaved data.</summary>
        /// <param name="inStream">Input stream from which XML is loaded.</param>
        public void Load(Stream inStream)
        {
            Load(inStream, false /* throw exception if data is already loaded. */ );
        }


        /// <summary>Loads data store'result XML Doc from atext reader.</summary>
        /// <param name="txtReader">Text reader from which XML is loaded.</param>
        /// <param name="forceoverwrite">If true then the file is loaded also if the data has been modified but not saved.</param>
        public void Load(TextReader txtReader, bool forceoverwrite)
        {
            if (txtReader == null)
                throw new ArgumentException("The text reader to load the data from is not specified.");
            if (!forceoverwrite && DataUnsaved)
                throw new Exception("Data has been modified but not saved. To overwrite the existing data, call DataStore.Load(...,true)!");
            _Data = new XmlData();
            try { _Data.Load(txtReader); }
            catch (Exception ex) {
                throw new Exception("Data could not be loaded form an input stream. Details: " + ex.Message);
            };
            // Re-set the data modification indicator:
            DataUnsaved = false;
        }


        /// <summary>Loads data store'result XML Doc from atext reader.
        /// An exception is thrown if the there is modified and unsaved data.</summary>
        /// <param name="txtReader">Text reader from which XML is loaded.</param>
        public void Load(TextReader txtReader)
        {
            Load(txtReader, false /* throw exception if data is already loaded. */ );
        }




        #endregion Data_Loading   // Loading of the data


        // XML DATA MANIPULATION:
        #region XML_Manipulation

        /// <summary>Returns the root node of XML data.</summary>
        /// <returns>The root node of the document.</returns>
        public XmlNode GetRootNode()
        {
            if (Doc == null)
                throw new Exception("Data is not loaded.");
            return Doc.GetRootNode();
        }


        /// <summary>Returns a list of all _gridCoordinates of XML data that satisfy the specified XPath expression.</summary>
        /// <param name="xpath">The XPath expression used for selection of _gridCoordinates.</param>
        /// <returns>List of all _gridCoordinates that satisfy the XPath expression.</returns>
        public XmlNodeList GetNodes(string xpath)
        {
            if (Doc == null)
                throw new Exception("Data is not loaded.");
            return Doc.GetNodes(xpath);
        }

        /// <summary>Returns the first node in the current document that satisfy the specified XPath expression.</summary>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node in the sub-tree that satisfies the XPath expression.</returns>
        public XmlNode GetNode(string xpath)
        {
            if (Doc == null)
                throw new Exception("Data is not loaded.");
            return Doc.GetNode(xpath);
        }

        /// <summary>Returns an array of all element _gridCoordinates in the current XML document
        /// that satisfiy the specified XPath expression.</summary>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>

        // public XmlElement[] GetElements(string xpath)


        public XmlElement[] GetElements(string xpath)
        {
            if (Doc == null)
                throw new Exception("Data is not loaded.");
            return Doc.GetElements(xpath);
        }


        /// <summary>Returns the first ELEMENT node in the XML sub-tree whose root is basenode
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public XmlElement GetElement(string xpath)
        {
            if (Doc == null)
                throw new Exception("Data is not loaded.");
            return Doc.GetElement(xpath);
        }

        //XmlElement GetElement0(string xpath)
        //{
        //    try { return this.DocumentElement.SelectSingleNode(xpath) as XmlElement; }
        //    catch{}
        //    return null;
        //}

        //XmlElement GetElement(string xpath, string attributename, string attributevalue, 
        //    bool nameignorecase, bool valueignorecase)
        //{
        //    try
        //    {
        //        XmlElement element = GetElement(xpath);
        //        if (string.IsNullOrEmpty(attributename))
        //            return element;
        //        else
        //        {
        //            if (!ignorecase)
        //            {
        //                // case sensitive
        //            } else
        //            {
        //                // not case sensitive
        //            }
        //            string attr = element.GetAttribute(
        //                (attributename);
        //            if (string.IsNullOrEmpty(attr))
        //                return null;
        //            else
        //            {
        //                if (string.IsNullOrEmpty(attributevalue))
        //                {
        //                    if (string.IsNullOrEmpty(attr.Value))
        //                        return element;
        //                    else
        //                        return null;
        //                }
        //                else
        //                {
        //                    if (string.Compare(attributevalue,attr.Value,ignorecase))
        //                        return null;
        //                    else
        //                        return element;
        //                }
        //            }
        //        }
        //    }
        //    catch { }
        //    return null;
        //}







        // Exception handling - will be arranged through IErrorHandling interface.
        //private bool _CatchExceptions = true, _ReportErrors=true, _NullExceptions = true;

        ///// <summary>Gets or sets the exception catching flag.
        ///// If true then most of exceptions are caught internally.</summary>
        //public virtual bool CatchExceptions 
        //{ get { return _CatchExceptions; } set { _CatchExceptions = value; } }

        ///// <summary>Gets or sets the exception catching flag.
        ///// If true then exceptions are reported by class members.</summary>
        //public virtual bool ReportErrors
        //{ get { return _ReportErrors; } set { _ReportErrors = value; } }

        ///// <summary>Gets or sets the flag indicating whether null results throw exceptions.
        ///// If false then null results are returned without throwind exceptions.
        ///// 
        ///// </summary>
        //public virtual bool NullExceptions
        //{ get { return _NullExceptions; } set { _NullExceptions = value; } }

        #endregion  // XML_Manipulation


    }  // class DataStore


    // EXTENSION OF XmlDocument

    /// <summary>XmlDocument extended by additional functionality for managing complex data units.
    /// In applications, this is used for 
    /// </summary>
    public class XmlData : XmlDocument
    {

        /// <summary>Returns the root node of the document.</summary>
        /// <returns>The root node of the document.</returns>
        public XmlNode GetRootNode()
        {
            XmlNode ret = null;
            XmlNodeList childnodes = this.ChildNodes;
            if (childnodes == null)
                throw new NullReferenceException("The document contains no child elements.");
            int num, i;
            XmlNode child;
            num = childnodes.Count;
            i = 0;
            while (i < num && ret == null)
            {
                child = childnodes.Item(i);
                if (child.NodeType == XmlNodeType.Element)
                    ret = child;
            }
            return ret;
        }


        /// <summary>Returns a list of all _gridCoordinates in the current document that satisfy the specified XPath expression.</summary>
        /// <param name="xpath">The XPath expression used for selection of _gridCoordinates.</param>
        /// <returns>List of all _gridCoordinates that satisfy the XPath expression.</returns>
        public XmlNodeList GetNodes(string xpath)
        {  return Xml.GetNodes(this, xpath);  }

        /// <summary>Returns the first node in the current document that satisfy the specified XPath expression.</summary>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node in the sub-tree that satisfies the XPath expression.</returns>
        public XmlNode GetNode(string xpath)
        { return Xml.GetNode(this, xpath); }

        /// <summary>Returns an array of all element _gridCoordinates in the current XML document
        /// that satisfiy the specified XPath expression.</summary>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public XmlElement[] GetElements(string xpath)
        {
            return Xml.GetElements(this, xpath);
        }

        /// <summary>Returns the first ELEMENT node in the current XML document
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public XmlElement GetElement(string xpath)
        {
            return Xml.GetElement(this, xpath);
        }

        /// <summary>Returns the (first) text node of the first element node in the current document
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public XmlNode GetTextNode(string xpath)
        {
            return Xml.GetTextNode(this, xpath);
        }

        /// <summary>Returns value of the (first) text node of the first element node in current document
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public string GetValue(string xpath)
        {
            return Xml.GetValue(this, xpath);
        }



        //XmlElement GetElement0000(string xpath)
        //{
        //    try { return this.DocumentElement.SelectSingleNode(xpath) as XmlElement; }
        //    catch{}
        //    return null;
        //}

        //XmlElement GetElement(string xpath, string attributename, string attributevalue, 
        //    bool nameignorecase, bool valueignorecase)
        //{
        //    try
        //    {
        //        XmlElement element = GetElement(xpath);
        //        if (string.IsNullOrEmpty(attributename))
        //            return element;
        //        else
        //        {
        //            if (!ignorecase)
        //            {
        //                // case sensitive
        //            } else
        //            {
        //                // not case sensitive
        //            }
        //            string attr = element.GetAttribute(
        //                (attributename);
        //            if (string.IsNullOrEmpty(attr))
        //                return null;
        //            else
        //            {
        //                if (string.IsNullOrEmpty(attributevalue))
        //                {
        //                    if (string.IsNullOrEmpty(attr.Value))
        //                        return element;
        //                    else
        //                        return null;
        //                }
        //                else
        //                {
        //                    if (string.Compare(attributevalue,attr.Value,ignorecase))
        //                        return null;
        //                    else
        //                        return element;
        //                }
        //            }
        //        }
        //    }
        //    catch { }
        //    return null;
        //}



    }  // class DataStore 


        

    

    /// <summary>Supplemental basic XML node & document manipulation utilities.
    /// Contains some useful static methods for XML manipulation.</summary>
    public class Xml
    {

        /// <summary>Returns the root node of the document containtng the specified xml node.</summary>
        /// <param name="node">Node whose containing document is queried for the root node.</param>
        /// <returns>The root node of the document that contains the specified node.</returns>
        public static XmlNode RootNode(XmlNode node) 
        {
            XmlNode ret=null;
            if (node==null)
                throw new ArgumentException("Xml.RootNode(): node is not specified (null reference).");
            XmlDocument doc;
            doc = node as XmlDocument;  // try if the node is already an Xml document:
            if (doc==null)
                doc = node.OwnerDocument;
            if (doc == null)
                throw new NullReferenceException("Owner document of the node could not be determined.");
            XmlNodeList childnodes = doc.ChildNodes;
            if (childnodes == null)
                throw new NullReferenceException("The document has no child elements.");
            int num, i;
            XmlNode child;
            num = childnodes.Count;
            i = 0;
            while (i<num && ret == null)
            {
                child = childnodes.Item(i);
                if (child.NodeType == XmlNodeType.Element)
                    ret = child;
            }
            return ret;
        }

        /// <summary>Defines the index of the first child node as used in XPath expressions.</summary>
        public const int FirstChildIndex = 1;

        /// <summary>Returns the index of the node among siblings with the same name.</summary>
        /// <param name="node">The XML node whose index is returned.</param>
        /// <returns>Index of the XML node in a collection of its sibling _gridCoordinates with the same name.
        /// Counting starts from Xml.FirstChildIndex.
        /// If node does not have a name (or it has an empty name) then node'result index among all siblings is returned.</returns>
        public static int ChildIndex(XmlNode node)
        {
            if (node==null)
                throw new ArgumentException("Xml.ChildIndex(): node is not specified (null reference).");
                // return FirstChildIndex - 1;
            int ret = 0;
            XmlNode current=node;
            // Get node'result name or null if the node is of such a type that it doesn't have a name:
            string name = null;
            try { name = node.Name; }
            catch { }
            while(current!=null)
            {
                current=current.PreviousSibling;
                if (current != null) if (string.IsNullOrEmpty(name) // for a node without a name, get its index among all siblings
                                || current.Name == node.Name)
                    ++ret;
            }
            return ret + FirstChildIndex;
        }

        /// <summary>Returns an XPath expression that uniquely specifies location of an XML node relative to a given ancestor.</summary>
        /// <param name="node">The node whose XPath expressino is searched for.</param>
        /// <param name="basenode">The node'result ancestor relative to which the XPath is specified.
        /// If null then the absolute path within containing outer-most node (or document) is returned.
        /// If not null then basenode must be an ancestor of the node.</param>
        /// <returns></returns>
        public static string XPath(XmlNode node, XmlNode basenode)
        {
            try
            {
                if (node == null)
                    throw new ArgumentException("Xml.XPath(): The node whose XPath is queried is not specified (null reference).");
                if (node == basenode)
                    return "/";
                XmlNode
                    current = basenode,
                    parent = basenode.ParentNode;  // parent of the current node
                string ret = null;
                if (parent == null)
                    ret = "/";
                else
                {
                    ret = "";
                    if (current.NodeType == XmlNodeType.Attribute)
                    {
                        // If the node is an attribute then add @ before its name:
                        if (current == basenode)
                            throw new ArgumentException("An attribute node is specified as the base node in XPath query.");
                        ret = "/@" + current.Name;
                        // move one messagelevel higher:
                        current = parent;
                        parent = current.ParentNode;
                    }
                    if (current != null)
                    {
                        int index;
                        do
                        {
                            // Calculate index of the current node in the list of all its siblings with the same name and
                            // create the portion of xpath string corresponding to this node:
                            string name = null;
                            try { name = current.Name; }
                            catch { }
                            index = ChildIndex(current);
                            if (string.IsNullOrEmpty(name))
                            {
                                ret += "[" + index + "]";  // current element has no name (e.g. comment or tect node), add to 
                                // xpath only index of the element in square brackets, which will follow its parent name;
                            }
                            else
                            {
                                if (index == FirstChildIndex)  // we do not need to state index of the current element if it is the first sibling with specific name
                                    ret += "/" + current.Name;
                                else
                                    ret += "/" + current.Name + "[" + index + "]";
                            }
                            // move one messagelevel higher:
                            current = parent;
                            parent = current.ParentNode;

                        } while (parent != null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return null;
        }

        /// <summary>Returns a list of all _gridCoordinates in the XML sub-tree whose root is basenode
        /// that satisfy the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of _gridCoordinates.</param>
        /// <returns>List of all _gridCoordinates that satisfy the XPath expression.</returns>
        public static XmlNodeList GetNodes(XmlNode basenode, string xpath)
        {
            return basenode.SelectNodes(xpath);
        }

        /// <summary>Returns the first _gridCoordinates in the XML sub-tree whose root is basenode
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public static XmlNode GetNode(XmlNode basenode, string xpath)
        {
            return basenode.SelectSingleNode(xpath);
        }

        /// <summary>Returns an array of all elements _gridCoordinates in the XML sub-tree whose root is basenode
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public static XmlElement[] GetElements(XmlNode basenode, string xpath)
        {
            try 
            {
                XmlNodeList allnodes = basenode.SelectNodes(xpath);
                List<XmlElement> elementnodes = new List<XmlElement>();
                for (int i = 0; i < allnodes.Count; ++i)
                {
                    XmlElement node = allnodes.Item(i) as XmlElement;
                    // if (node.NodeType == XmlNodeType.Element)
                    if (node!=null)  // the given node is an XML element
                        elementnodes.Add(node) ;
                }
                if (elementnodes.Count < 1)
                    return null;
                else
                    return elementnodes.ToArray();
            }
            catch{}
            return null;
        }

        /// <summary>Returns the first ELEMENT node in the XML sub-tree whose root is basenode
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public static XmlElement GetElement(XmlNode basenode, string xpath)
        {
            try { return basenode.SelectSingleNode(xpath) as XmlElement; }
            catch{}
            return null;
        }

        /// <summary>Returns the (first) text node of the first ELEMENT node in the XML sub-tree whose root is basenode
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public static XmlNode GetTextNode(XmlNode basenode, string xpath)
        {
            XmlNode ret = null;
            try 
            { 
                XmlNode node = basenode.SelectSingleNode(xpath).FirstChild;
                if (node!=null) if (node.NodeType == XmlNodeType.Text)
                    ret = node;
            }
            catch{}
            return ret;
        }

        /// <summary>Returns value of the (first) text node of the first ELEMENT node in the XML sub-tree whose root is basenode
        /// that satisfies the specified XPath expression.</summary>
        /// <param name="basenode">Root node of the XML sub-tree in which _gridCoordinates are searched for.</param>
        /// <param name="xpath">The XPath expression used for selection of the node.</param>
        /// <returns>The first node that satisfies the XPath expression.</returns>
        public static string GetValue(XmlNode basenode, string xpath)
        {
            string ret = null;
            try 
            { 
                XmlNode node = basenode.SelectSingleNode(xpath).FirstChild;
                if (node!=null) if (node.NodeType == XmlNodeType.Text)
                    ret = node.Value;
            }
            catch{}
            return ret;
        }


        // Implementation of set & create methods:

        public static bool CreateContainer(XmlNode basenode, string xpath, string ContainerAttribute, string ContainerValue)
        {
            bool ret=false;


            if (basenode!=null)
                throw new Exception("Xml.CreateContainer(): This function is not implemented yet.");
            
            
            
            return ret;
        }

        public static bool CreateTextNode(XmlNode basenode, string xpath, 
            string ContainerAttribute, string ContainerValue, string NodeAttribute, string NodeValue)
        {
            bool ret = false;
            if (basenode!=null)
                throw new Exception("Xml.CreateTextNode(): This function is not implemented yet.");
            return ret;
        }

        public static bool SetValue(XmlNode basenode, string xpath, string value, bool createnodes,
                string ContainerAttribute, string ContainerValue, string NodeAttribute, string NodeValue)
        {
            bool ret = false;
            if (basenode!=null)
                throw new Exception("Xml.SetValue(): This function is not implemented yet.");

            return ret;
        }

        public static bool SetValue(XmlNode basenode, string xpath, string value, bool createnodes)
        {
            return SetValue(basenode, xpath, value, createnodes, null, null, null, null);
        }

        public static bool SetValue(XmlNode basenode, string xpath, string value)
        {
            return SetValue(basenode, xpath, value, false /* createnodes */ , null, null, null, null);
        }





        ////public static XmlNode GetFirstChildElementByName(XmlNode ParentNode, string Name)
        ////// Returns the first child element of ParentName with a specified Name.
        ////{
        ////    XmlNode ReturnedString = null;
        ////    if (ParentNode != null)
        ////    {
        ////        int i = 0, numchildren = 0;
        ////        if (ParentNode.ChildNodes != null) numchildren = ParentNode.ChildNodes.Count;
        ////        while (i < numchildren && ReturnedString == null)
        ////        {
        ////            XmlNode node = ParentNode.ChildNodes[i];
        ////            if (node.NodeType == XmlNodeType.Element && node.Name == Name)
        ////                ReturnedString = node;
        ////            ++i;
        ////        }
        ////    }
        ////    return ReturnedString;
        ////}

        ////public static XmlNode TextNodeOfFirstChildElementByName(XmlNode ParentNode, string Name)
        ////// Gets the text child of the first child element of ParentName with a specified Name.
        ////// Global is because value can be set only to text _gridCoordinates, and in this way we obtain the 
        ////// node of specific element where we can set the value.

        ////    // TODO: Dopolni tako, da bo vrnil prvo voyli[;e, katerega tip je tekst!
        ////{
        ////    XmlNode ReturnedString = null;
        ////    XmlNode element = GetFirstChildElementByName(ParentNode, Name);
        ////    if (element != null)
        ////    {
        ////        ReturnedString = element.FirstChild;
        ////        if (ReturnedString.NodeType != XmlNodeType.Text)
        ////            ReturnedString = null;
        ////    }
        ////    return ReturnedString;
        ////}



        //// TODO: use this function to CREATE an element!
        //public static XmlNode GetXmlElement00000(XmlNode Tree, string Path)
        //// Finds an XML tree node in the XML tree that is specified by the path string Path, and 
        //// returns it (or null if it douen't find a match)
        //// Example of Path: "/root/
        //{
        //    XmlNode ReturnedString = null, CurrentNode;
        //    try
        //    {
        //        // split the path to individual parts:
        //        string[] PathSegments = Path.Split(new char[] { '/' });
        //        CurrentNode = Tree;
        //        int iSegment = 0;
        //        string Segment, SegmentName;
        //        string[] SegmentSplit;
        //        while (iSegment < PathSegments.Length && CurrentNode != null)
        //        {
        //            Segment = PathSegments[iSegment];
        //            ++iSegment;
        //            if (Segment.Length > 0) // if the following segment is "" then current node is not changed.
        //            {
        //                SegmentSplit = Segment.Split(new char[] { '[', ']' });
        //                SegmentName = SegmentSplit[0];
        //                int WhichChild = 0;
        //                try
        //                {
        //                    if (SegmentSplit.Length > 1)
        //                    {
        //                        WhichChild = int.Parser(SegmentSplit[1]);
        //                    }
        //                }
        //                catch { }

        //                int numFound = 0; // number of _gridCoordinates with name that corresponds to SegmentName
        //                XmlNodeList Children = CurrentNode.ChildNodes;
        //                int iNode = 0;
        //                CurrentNode = null;
        //                while (iNode < Children.Count && numFound < WhichChild)
        //                {
        //                    XmlNode CurrentChild = Children[iNode];
        //                    if (CurrentChild.Name.CompareTo(SegmentName) == 0)
        //                    {
        //                        ++numFound;
        //                        if (numFound == WhichChild)
        //                            CurrentNode = CurrentChild;
        //                    }
        //                    ++iNode;
        //                }

        //            }
        //        }
        //        ReturnedString = CurrentNode;
        //    }
        //    catch (Exception ex)
        //    {
        //        ProgramBase.ReportError(ex);
        //    }
        //    return ReturnedString;
        //}   // GetXmlElement



    }   // class Xml



    // Reporter:
    //         public virtual IG.Lib.Reporter R { get { return Program.R; } }


    /// <summary>Base class for various utilities operating on XmlDocumnt.</summary>
    public class XmlUtilityBase
    // $A Igor Aug08 Apr09;
    {
        /// <summary>Reporter for this class.</summary>
        public virtual IG.Lib.IReporter R { get { return App.Rep; } }


        #region NameSpaceResolution


        private bool _hasDefaultNameSpace = false;


        public virtual bool HasDefaultNameSpace
        {
            get { return _hasDefaultNameSpace; }
            protected set { _hasDefaultNameSpace = value; }
        }

        public string DefaultNameSpace = null;

        public string DefaultNameSpacePrefix = "defaultnsprefix";



        private const string XmlNsAttribute = "xmlns";

        /// <summary>Gets the default namespace URI if defined, for the specified Xml element.</summary>
        /// <param name="element"></param>
        public static string GetDefaultNameSpaceUri(XmlElement element)
        {
            string ret = null;
            if (element != null)
            {
                try
                {
                    ret = element.GetAttribute(XmlNsAttribute);
                }
                catch { }
                if (ret == null)
                {
                    // The element does not directly contain the attribute defining the default namespace.
                    // Try with its ancestor _gridCoordinates:
                    try
                    {
                        XmlElement parent = element.ParentNode as XmlElement;
                        if (parent != null)
                            ret = GetDefaultNameSpaceUri(parent);
                    }
                    catch { }
                }
            }
            return ret;
        }

        /// <summary>Gets the default namespace URI of teh Xml document.</summary>
        /// <param name="doc">Xml document to which the default namespace might apply.</param>
        /// <returns>Tha default namespace URI that applies to the document, or null if there is no
        /// such namespace.</returns>
        public static string GetDefaultNameSpaceUri(XmlDocument doc)
        {
            string ret = null;
            if (doc != null)
            {
                ret = GetDefaultNameSpaceUri(doc.DocumentElement);

            }
            return ret;
        }


        /// <summary>Returns the default namespace URI that applies to the specified Xml node.</summary>
        /// <param name="node">Xml node to which namespace URI applies.</param>
        /// <returns>The defaulut namespace URI atht applies to the node, or null if there is no default namespace.</returns>
        public static string GetDefaultNameSpaceUri(XmlNode node)
        {
            if (node == null)
                return null;
            string ret = null;
            XmlElement el = node as XmlElement;
            if (el == null)
                el = node.ParentNode as XmlElement;
            if (el != null)
                ret = GetDefaultNameSpaceUri(el);
            return ret;
        }



        private static string GetNamespaceAttributeName(string prefix)
        {
            return (XmlNsAttribute + ":" + prefix);
        }

        /// <summary>Gets the namespace URI introduced by a particular attribute, if defined, 
        /// for the specified Xml element.</summary>
        /// <param name="element">Element for which rhe specific namespace URI is searched for.</param>
        /// <param name="NamespaceAttributeName">Attribute name that introduces that namespace.</param>
        private static string GetNameSpaceUri0(XmlElement element, string NamespaceAttributeName)
        {
            string ret = null;
            if (element != null)
            {
                try
                {
                    ret = element.GetAttribute(NamespaceAttributeName);
                }
                catch { }
                if (ret == null)
                {
                    // The element does not directly contain the attribute defining the default namespace.
                    // Try with its ancestor _gridCoordinates:
                    try
                    {
                        XmlElement parent = element.ParentNode as XmlElement;
                        if (parent != null)
                            ret = GetNameSpaceUri0(parent, NamespaceAttributeName);
                    }
                    catch { }
                }
            }
            return ret;
        }

        /// <summary>Returns the namespace URI associated with a specific prefix that applies to
        /// the specified Xml element.
        /// URI information is obtained from the corresponding attribute of the specified node and
        /// eventually its parent _gridCoordinates.</summary>
        /// <param name="element">Element for which namespace URI is searched for.</param>
        /// <param name="prefix">Prefix for which teh namespace is searched for. If null or empty string
        /// then a default namespace URI is searched for.</param>
        /// <returns>The namespace URI corresponding to the specified prefix at the level of a specified element,
        /// or null  if the particular namespace URI is not defined.</returns>
        private static string GetNameSpaceUri(XmlElement element, string prefix)
        {
            if (element == null)
                return null;
            else if (string.IsNullOrEmpty(prefix))
                return GetDefaultNameSpaceUri(element);
            else
                return GetNameSpaceUri0(element, GetNamespaceAttributeName(prefix));
        }


        /// <summary>Returns the namespace URI associated with a specific prefix that applies to
        /// the specified Xml document.</summary>
        /// <param name="doc">Xml document to which namespace URI applies.</param>
        /// <returns>The namespace URI if found, null otherwise.</returns>
        public static string GetNameSpaceUri(XmlDocument doc, string prefix)
        {
            string ret = null;
            if (doc != null)
            {
                ret = GetNameSpaceUri(doc.DocumentElement, prefix);

            }
            return ret;
        }

        /// <summary>Returns the namespace URI associated with a specific prefix that applies to
        /// the specified Xml document.</summary>
        /// <param name="node">Xml node to which namespace URI applies.</param>
        /// <returns>The namespace URI if found, null otherwise.</returns>
        public static string GetNameSpaceUri(XmlNode node, string prefix)
        {
            if (node == null)
                return null;
            string ret = null;
            XmlElement el = node as XmlElement;
            if (el == null)
                el = node.ParentNode as XmlElement;
            if (el != null)
                ret = GetNameSpaceUri(el, prefix);
            return ret;
        }





        #endregion  // NameSpaceResolution


        #region Initialization

        public virtual void SetDocument(XmlDataDocument doc)
        {
            ++R.Depth;
            if (R.TreatInfo) R.ReportInfo("XmlParser.Load()", "Started...");
            try
            {
                Doc = doc;
                FileName = null;
            }
            catch (Exception ex)
            {
                // R.ReportError(ex);
                throw ex;
            }
            finally
            {
                if (R.TreatInfo) R.ReportInfo("XmlParser.Load()", "Finished.");
                --R.Depth;
            }
        }

        public virtual void Load(string filename)
        {
            ++R.Depth;
            if (R.TreatInfo) R.ReportInfo("XmlParser.Load()", "Started...");
            try
            {
                XmlDocument doctmp = new XmlDocument();
                doctmp.Load(filename);
                Doc = doctmp;
                FileName = filename;
            }
            catch (Exception ex)
            {
                // R.ReportError(ex);
                throw ex;
            }
            finally
            {
                if (R.TreatInfo) R.ReportInfo("XmlParser.Load()", "Finished.");
                --R.Depth;
            }
        }

        public virtual void LoadXml(string docstr)
        {
            ++R.Depth;
            if (R.TreatInfo) R.ReportInfo("XmlParser.LoadXml()", "Started...");
            try
            {
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(docstr);
                Doc = doctmp;
                FileName = null;
            }
            catch (Exception ex)
            {
                // R.ReportError(ex);
                throw ex;
            }
            finally
            {
                if (R.TreatInfo) R.ReportInfo("XmlParser.LoadXml()", "Finished.");
                --R.Depth;
            }
        }

        string _filename = null;

        public string FileName
        {
            get { return _filename; }
            protected set { _filename = value; }
        }




        #endregion  // Initialization


        private XmlDocument _doc = null;

        /// <summary>Xml document that represents the message.</summary>
        public virtual XmlDocument Doc
        {
            get { return _doc; }
            set
            {
                _doc = value;
                if (_doc != null)
                {
                    Root = _doc.DocumentElement;
                }
                else
                    Root = null;
            }
        }



        private XmlNode _root = null;
        /// <summary>Root node of the current document.</summary>
        public XmlNode Root
        {
            get { return _root; }
            private set
            {
                XmlNode nd = value;
                if (value != null)
                {
                    // Only a node that are contained in the document can be set as Root:
                    if (!ContainedInDocument(value, _doc))
                    {
                        R.ReportError("XmlParser.Root.set", "Attempt to set the node that is not contained in the current document.");
                        value = null;
                    }
                }
                _root = value;
            }
        }



        /// <summary>Returns true if an Xml node is contained in the specified Xml document (false if any is null).</summary>
        protected bool ContainedInDocument(XmlNode node, XmlDocument doc)
        {
            if (node == null || doc == null)
                return false;
            XmlNode current = node, parent = node;
            while (parent != null)
            {
                parent = current.ParentNode;
                if (parent != null)
                    current = parent;
            }
            if (current == doc)
                return true;
            else
                return false;
        }

        /// <summary>Returns null if an XML node (first argument) is contained in the specified note (second argument).
        /// The node can be contained in another node at an arbitrary depth for the function to return true.
        /// If any of the _gridCoordinates is null then the function returns false.</summary>
        /// <param name="node">Node that might be contained in another node.</param>
        /// <param name="container">The node that might contain another node.</param>
        /// <returns>true if container contains node, false othwrwise.</returns>
        protected bool ContainedInNode(XmlNode node, XmlNode container)
        {
            if (node == null || container == null)
                return false;
            else if (node == container)
                return true;
            XmlNode current = node, parent = node;
            while (parent != null)
            {
                parent = current.ParentNode;
                if (parent != null)
                {
                    if (parent == container)
                        return true;
                    current = parent;
                }
            }
            return false;
        }


    }  // class XmlUtilityBase


    /// <summary>Base class for classes taht contain an Xml document that can be parsed.
    /// Provides comfortable utilities for transversing the document and for querying the value, name, and 
    /// attributes of the current node.</summary>
    public class XmlParser : XmlUtilityBase
    // $A Igor Aug08 Mar09;
    {


        /// <summary>Xml document that represents the message.</summary>
        public override XmlDocument Doc
        {
            get { return base.Doc; }
            set
            {
                base.Doc = value;
                Current = Previous = Root;
                marks.Clear();
            }
        }



        #region Querying


        private XmlNode _current = null;
        /// <summary>The current node on which all queries are performed.</summary>
        public XmlNode Current
        {
            get { return _current; }
            set
            {
                XmlNode nd = value;
                if (nd != null)
                {
                    // Only a node that are contained in the document can be set as Current:
                    if (!ContainedInDocument(nd, Doc))
                    {
                        R.ReportError("XmlParser.Current.set", "Attempt to set the node that is not contained in the current document.");
                        nd = null;
                    }
                }
                _current = nd;
            }
        }

        private XmlNode _previous = null;

        /// <summary>The node that was previously set to the current node.</summary>
        public XmlNode Previous
        {
            get { return _previous; }
            set
            {
                XmlNode nd = value;
                if (nd != null)
                {
                    // Only a node that are contained in the document can be set as Previous:
                    if (!ContainedInDocument(nd, Doc))
                    {
                        R.ReportError("XmlParser.Previous.set", "Attempt to set the node that is not contained in the current document.");
                        nd = null;
                    }
                }
                _previous = nd;
            }
        }


        XmlNode _parent = null;
        /// <summary>Parent of Current. If Current = null then Parent can still return something, 
        /// which happens in particular when StepIn was performed before.
        /// Parent can be set to a specific node. That node will be returned by Parent when the Current
        /// is null (otherwise, the parent of Current is still returned). This is useful when current becomes
        /// null, e.g. because of running out of index bounds.</summary>
        public XmlNode Parent
        {
            get
            {
                if (Current != null)
                    return Current.ParentNode;
                else
                    return _parent;
            }
            private set
            {
                XmlNode nd = value;
                if (nd != null)
                {
                    // Only a node that are contained in the document can be set as parent node:
                    if (!ContainedInDocument(nd, Doc))
                    {
                        R.ReportError("XmlParser.Parent.set", "Attempt to set1 the node that is not contained in the current document.");
                        nd = null;
                    }
                }
                _parent = nd;
            }
        }

        protected XmlElement ParentElement { get { return Parent as XmlElement; } }
        protected XmlElement CurrentElement { get { return (Current as XmlElement); } }
        protected XmlElement PreviousElement { get { return (Previous as XmlElement); } }

        /// <summary>Returns the value of the specified attribute of the current node.
        /// If the attribute does not exist then null is returned.</summary>
        public string Attribute(string key)
        {
            string ret = null;
            try
            {
                ret = CurrentElement.GetAttribute(key);
            }
            catch { }
            return ret;
        }

        /// <summary>Returns the name of the current node.</summary>
        public string Name
        {
            get
            {
                if (Current == null)
                    return null;
                else
                    return Current.LocalName;
            }
        }

        /// <summary>Returns the value of the current node.
        /// If the current node is an element then the value of its first text child element is returned.</summary>
        public string Value
        {
            get
            {
                try
                {
                    if (Current == null)
                        return null;
                    else
                    {
                        if (Current.NodeType == XmlNodeType.Element)
                        {
                            // TODO: this method may not be implemented well, please reconsider & correct!
                            // We find the first text child node of the current node and return its value:
                            XmlNode nd = Current.FirstChild;
                            while (nd != null && nd.NodeType != XmlNodeType.Text)
                                nd = nd.NextSibling;
                            if (nd != null)
                                return nd.Value;
                        }
                        else
                            return Current.Value;
                    }
                }
                catch { }
                return null;
            }
        }

        /// <summary>Returns the inner text of the current node.</summary>
        public string InnerText
        {
            get
            {
                try
                {
                    if (Current == null)
                        return null;
                    else
                        return Current.InnerText;
                }
                catch { }
                return null;
            }
        }


        #region Searches

        // Searches:
        // Functions that return results of different queries but do not change the current position:

        /// <summary>Returns the first node that satisfies a given XPath expression relative to the root node.
        /// It does not report any errors (just returns null in case of errors)</summary>
        /// <param name="path">XPath expression.</param>
        /// <returns>The first node that satisfies the path, or null if no such node is found.</returns>
        public XmlNode GetNode(string path)
        {
            XmlNode ret = null;
            try
            {
                ret = Root.SelectSingleNode(path);
            }
            catch
            {
            }
            return ret;
        }

        /// <summary>Returns the first node that satisfies a given XPath expression relative to the current node.
        /// It does not report any errors.</summary>
        /// <param name="path">XPath expression.</param>
        /// <returns>The first node that satisfies the path, or null if no such node is found.</returns>
        public XmlNode GetRelative(string path)
        {
            XmlNode ret = null;
            try
            {
                ret = Current.SelectSingleNode(path);
            }
            catch
            {
            }
            return ret;
        }


        // TODO: Dopolni tako, da bodo ekvivalentne zmogljivosti kot pri NextOrCurrentElement in NextElement!

        /// <summary>Finds the first sibling node of the starting node (or the current node itself) that satisfies the 
        /// specified conditions, and returns that node or null if such a node could not be found.
        /// This method does not report errors, it just returns null if the node satisfying conditions can not be found.</summary>
        /// <param name="StartingNode">Starting node at which the search is started.</param>
        /// <param name="NodeType">If not XmlNodeType.None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        /// <param name="IncludeCurrent">If true then the returned node can also be the current node
        /// (if it satisfies the conditions), otherwise the search starts at the next sibling of the current node.</param>
        /// <returns></returns>
        public static XmlNode GetNextNode(XmlNode StartingNode, XmlNodeType NodeType, string NodeName,
                string NodeValue, bool IncludeCurrent)
        {
            XmlNode ret = null;
            try
            {
                if (StartingNode != null)
                {
                    XmlNode cur = StartingNode;
                    bool stop = false;
                    if (!IncludeCurrent)
                        cur = cur.NextSibling;
                    while (!stop)
                    {
                        if (cur == null)
                            stop = true;
                        else
                        {
                            stop = true;
                            if (NodeType != XmlNodeType.None) // type should be considered
                                if (cur.NodeType != NodeType)
                                    stop = false;
                            if (NodeName != null)  // Node name should also be considered
                                if (cur.LocalName != NodeName)
                                    stop = false;
                            if (NodeValue != null)
                                if (cur.Value != NodeValue)
                                    stop = false;
                        }
                        if (!stop)
                            cur = cur.NextSibling;
                    }
                    ret = cur;
                }
            }
            catch { }
            return ret;
        }



        // Changing the current node to the next OR the current node that satisfies given contitions:

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode GetNextNode(XmlNodeType NodeType, string NodeName, string NodeValue)
        {
            return GetNextNode(Current  /* StartingNode */, NodeType, NodeName, NodeValue, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode GetNextNode(XmlNodeType NodeType, string NodeName)
        {
            return GetNextNode(Current  /* StartingNode */, NodeType, NodeName, null /* NodeValue */, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that 
        /// is of the specified type.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        public XmlNode GetNextNode(XmlNodeType NodeType)
        {
            return GetNextNode(Current  /* StartingNode */, NodeType, null /* NodeName */, null /* NodeValue */, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node. Node type is not important.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode GetNextNode(string NodeName, string NodeValue)
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.None /* NodeType */, NodeName, NodeValue, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node. Node type is not important.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode GetNextNode(string NodeName)
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.None /* NodeType */, NodeName, null /* NodeValue */, false /* IncludeCurrent */);
        }


        // Changing the current node to the next OR the current node that satisfies given contitions:

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode GetNextOrCurrentNode(XmlNodeType NodeType, string NodeName, string NodeValue)
        {
            return GetNextNode(Current  /* StartingNode */, NodeType, NodeName, NodeValue, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode GetNextOrCurrentNode(XmlNodeType NodeType, string NodeName)
        {
            return GetNextNode(Current  /* StartingNode */, NodeType, NodeName, null /* NodeValue */, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that 
        /// is of the specified type.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        public XmlNode GetNextOrCurrentNode(XmlNodeType NodeType)
        {
            return GetNextNode(Current  /* StartingNode */, NodeType, null /* NodeName */, null /* NodeValue */, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node. Node type is not important.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode GetNextOrCurrentNode(string NodeName, string NodeValue)
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.None /* NodeType */, NodeName, NodeValue, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node. Node type is not important.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode GetNextOrCurrentNode(string NodeName)
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.None /* NodeType */, NodeName, null /* NodeValue */, true /* IncludeCurrent */);
        }

        // These two groups of functions that only cosider element _gridCoordinates:

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode GetNextElement(string NodeName, string NodeValue)
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.Element /* NodeType */, NodeName, NodeValue, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode GetNextElement(string NodeName)
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.Element /* NodeType */, NodeName, null /* NodeValue */, false /* IncludeCurrent */);
        }


        /// <summary>Moves the current node to the first sibling element after the current node.</summary>
        public XmlNode GetNextElement()
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.Element /* NodeType */, null /* NodeName */, null /* NodeValue */, false /* IncludeCurrent */);
        }


        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode GetNextOrCurrentElement(string NodeName, string NodeValue)
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.Element /* NodeType */, NodeName, NodeValue, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode GetNextOrCurrentElement(string NodeName)
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.Element /* NodeType */, NodeName, null /* NodeValue */, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling element after the current node.</summary>
        public XmlNode GetNextOrCurrentElement()
        {
            return GetNextNode(Current  /* StartingNode */, XmlNodeType.Element /* NodeType */, null /* NodeName */, null /* NodeValue */, true /* IncludeCurrent */);
        }


        #endregion  // Searches

        #endregion Querying


        #region Traversing

        /// <summary>Position mark in the XML document</summary>
        public class Mark
        {
            public string name = null;
            public XmlNode Current = null, Previous = null, Parent = null;
        }



        private List<Mark> marks = new List<Mark>();

        /// <summary>Marks the current state of the XmlParser and sets mark name to name.
        /// Position is stored on a stack such that previous stored positions can be restored, either in a reverse way.</summary>
        /// <param name="name">Name assigned to the mark.</param>
        public void SetMark(string name)
        {
            XmlNode pos = null;
            try
            {
                Mark mark = new Mark();
                pos = mark.Current = Current;
                mark.Previous = Previous;
                mark.Parent = Parent;
                if (name == null)
                    name = "";
                mark.name = name;
                marks.Add(mark);
            }
            catch { }
            finally
            {
                if (pos == null)
                    R.ReportError("XmlParser.SetMark()", "Invalid current position, could not set a mark.");
            }
        }

        /// <summary>Marks the current state of the XmlParser. The mark set is not named.
        /// Position is stored on a stack such that previous stored positions can be restored, either in a reverse way.</summary>
        /// <param name="name">Name assigned to the mark.</param>
        public void SetMark()
        {
            SetMark(null);
        }
        /// <summary>Restores the parser state that is stored in the specified mark. The current node after operation
        /// is returned, or null if mark is not specified (i.e., it is null).</summary>
        /// <param name="mark">The mark containing the state that is restored.</param>
        /// <returns>The current node after operation.</returns>
        protected XmlNode GoToMark(Mark mark)
        {
            XmlNode ret = null;
            try
            {
                if (mark != null)
                {
                    MoveTo(mark.Current);
                    ret = Current;
                    Previous = mark.Previous;
                    Parent = mark.Parent;
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Restores the parser state to the state contained in the last mark. The current node after operation
        /// is returned, or null if there are no marks. 
        /// If required then the last mark is removed from the list.</summary>
        /// <param name="removemarks">If true then all marks from the specified one on (inclusively) are removed.</param>
        /// <returns>The current node after operation or null if there is no valid last mark.</returns>
        public XmlNode GoToMark(bool removemark)
        {
            XmlNode ret = null;
            try
            {
                Mark mark = null;
                if (marks.Count > 0)
                {
                    mark = marks[marks.Count - 1];  // marks.Last();
                    if (removemark)
                    {
                        marks.RemoveAt(marks.Count);
                    }
                    ret = GoToMark(mark);
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Restores the parser state to the state contained in the last mark with the specified name. 
        /// The current node after operation is returned, or null if there are no marks with such a name.
        /// If required then the targeted and all subsequent marks are removed from the list.</summary>
        /// <param name="name">Name of the mark whose state is restored.</param>
        /// <param name="removemarks">If true then all marks from the specified one on (inclusively) are removed.</param>
        /// <returns>The current node after operation or null if the specified mark has not been found.</returns>
        public XmlNode GoToMark(string name, bool removemarks)
        {
            XmlNode ret = null;
            if (string.IsNullOrEmpty(name))
                throw new Exception("XmlParser.GoToMark(name, ...): mark name is not specified.");
            try
            {
                Mark mark = null;
                if (marks.Count > 0)
                {
                    int which = marks.Count - 1;
                    // find the last mark with the specified name:
                    while (which >= 0 && mark == null)
                    {
                        Mark aux = marks[which];
                        if (aux != null) if (aux.name == name)
                                mark = aux;
                        if (mark == null)
                            --which;
                    }
                    // mark=marks.Last(m => m.name == name);
                    if (mark != null)
                    {
                        if (removemarks)
                            // Remove all marks from the specified one (inclusively):
                            marks.RemoveRange(which, marks.Count - which);
                        ret = GoToMark(mark);
                    }
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Restores the parser state to the state contained in the last mark. The current node after operation
        /// is returned, or null if there are no marks. 
        /// The last mark is LEFT on the list.</summary>
        /// <param name="removemarks">If true then all marks from the specified one on (inclusively) are removed.</param>
        /// <returns>The current node after operation or null if there is no valid last mark.</returns>
        public XmlNode GoToMark()
        {
            return GoToMark(false);
        }

        /// <summary>Restores the parser state to the state contained in the last mark with the specified name. 
        /// The current node after operation is returned, or null if there are no marks with such a name.</summary>
        /// <param name="name">Name of the mark whose state is restored.</param>
        /// <returns>The current node after operation or null if the specified mark has not been found.</returns>
        public XmlNode GoToMark(string name)
        {
            return GoToMark(name, false);
        }

        /// <summary>Restores the parser state to the state contained in the last mark, and 
        /// REMOVES that mark. 
        /// The current node after operation is returned, or null if there are no marks.</summary>
        /// <returns>The current node after operation or null if there is no valid last mark.</returns>
        public XmlNode BackToMark()
        {
            return GoToMark(true);
        }

        /// <summary>Restores the parser state to the state contained in the last mark with the specified name, and
        /// REMOVES all marks from that mark on (includively). 
        /// The current node after operation is returned, or null if there are no marks with such a name.</summary>
        /// <param name="name">Name of the mark whose state is restored.</param>
        /// <returns>The current node after operation or null if the specified mark has not been found.</returns>
        public XmlNode BackToMark(string name)
        {
            return GoToMark(name, true);
        }

        /// <summary>Removes the last mark and returns its current node.
        /// Position is not affected.</summary>
        /// <returns>The current node stored on the last mark, or null if no marks are defined.</returns>
        public XmlNode RemoveMark()
        {
            XmlNode ret = null;
            try
            {
                Mark mark = null;
                if (marks.Count > 0)
                {
                    mark = marks[marks.Count - 1];  // marks.Last();
                    marks.RemoveAt(marks.Count);
                    if (mark != null)
                        ret = mark.Current;
                }
            }
            catch { }
            return ret;
        }


        /// <summary>Removes all marks from the last mark with the specified name on the list and returns 
        /// the current node of the specified mark.
        /// Position is not affected.</summary>
        /// <returns>The current node stored on the last mark with the specified name, or null if such a mark is not found. </returns>
        public XmlNode RemoveMarks(string name)
        {
            XmlNode ret = null;
            if (string.IsNullOrEmpty(name))
                throw new Exception("XmlParser.RemoveMarks(name, ...): mark name is not specified.");
            try
            {
                Mark mark = null;
                if (marks.Count > 0)
                {
                    int which = marks.Count - 1;
                    // find the last mark with the specified name:
                    while (which >= 0 && mark == null)
                    {
                        Mark aux = marks[which];
                        if (aux != null) if (aux.name == name)
                                mark = aux;
                        if (mark == null)
                            --which;
                    }
                    // mark=marks.Last(m => m.name == name);
                    if (mark != null)
                    {
                        // Remove all marks from the specified one (inclusively):
                        marks.RemoveRange(which, marks.Count - which);
                        ret = mark.Current;
                    }
                }
            }
            catch { }
            return ret;
        }



        /// <summary>Moves the current to the previous node, if that node exists.
        /// Only one step backwards is enabled.</summary>
        public XmlNode Back()
        {
            XmlNode ret = null;
            try
            {
                if (Current != null && Previous != null)
                {
                    XmlNode prev = Current;
                    ret = Current = Previous;
                    Previous = prev;
                }
            }
            catch { }
            return ret;
        }


        /// <summary>Moves the current position to the specified node.</summary>
        public XmlNode MoveTo(XmlNode node)
        {
            XmlNode ret = null;
            try
            {
                if (node == null)
                    R.ReportError("XmlParser.MoveTo()", "Invalid node to jump to (null reference).");
                else if (!ContainedInDocument(node, Doc))
                {
                    node = null;
                    R.ReportError("XmlParser.MoveTo()", "Invalid node to jump to. Node name: " + node.Name);
                }
                else
                {
                    ret = node;
                    XmlNode prev = Current;
                    Current = ret;
                    Parent = Current.ParentNode;  // update parent information because the context may have been changed
                    if (prev != null && Current != prev)  // update previous only if the current node has changed and it was not null before
                        Previous = prev;
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Moves the current position to the first node that satisfies the absolute path 
        /// specified as an XPath string.</summary>
        /// <param name="path">XPath string that specifies the node position relative to the document root.</param>
        /// <returns>The node to which the current position has moved, or null if not successful.</returns>
        public XmlNode MoveTo(string path)
        {
            XmlNode ret = null;
            try
            {
                try
                {
                    ret = GetNode(path);
                }
                catch { }
                if (ret != null)
                    ret = MoveTo(ret);
            }
            catch
            {
            }
            return ret;
        }

        /// <summary>Moves the current position to the first node that satisfies the relative path 
        /// specified as an XPath string relative to the current node.</summary>
        /// <param name="path">XPath string that specifies the node position relative to the current node.</param>
        /// <returns>The node to which the current position has moved, or null if not successful.</returns>
        public XmlNode MoveRelative(string path)
        {
            XmlNode ret = null;
            try
            {
                try
                {
                    ret = GetRelative(path);
                }
                catch { }
                if (ret != null)
                    ret = MoveTo(ret);
            }
            catch
            {
            }
            return ret;
        }


        /// <summary>Sets the current node to the parent node of the current node and returns it.
        /// This also works if the Current node is null because the advancing fell out of range or because
        /// StepIn was executed but there were no child _gridCoordinates of the current node.</summary>
        /// <returns>The current node.</returns>
        public XmlNode GoToParent()
        {
            XmlNode ret = null;
            try
            {
                XmlNode newparent = Parent.ParentNode;
                XmlNode prev = Current;
                ret = Current = Parent;  // current becomes the current parent node;
                if (Current != null && prev != null) // update previous if current has changed and current was not null
                    Previous = prev;
                Parent = newparent;
            }
            catch { }
            return ret;
        }

        /// <summary>Sets the current node to the root node of the current document and returns it.</summary>
        /// <returns>The current node.</returns>
        public XmlNode GoToRoot()
        {
            XmlNode ret = null;
            try
            {
                XmlNode prev = Current;
                ret = Current = Root;  // current becomes the current parent node;
                if (Current != null && prev != null) // update previous if current has changed and current was not null
                    Previous = prev;
            }
            catch { }
            return ret;
        }

        /// <summary>Sets the current node to the root node of the current document and returns it.</summary>
        /// <returns>The current node.</returns>
        public XmlNode GoToDocument()
        {
            XmlNode ret = null;
            try
            {
                XmlNode prev = Current;
                ret = Current = Doc;  // current becomes the current parent node;
                if (Current != null && prev != null) // update previous if current has changed and current was not null
                    Previous = prev;
            }
            catch { }
            return ret;
        }

        /// <summary>Moves the current node to its first child node and returns the node.</summary>
        public XmlNode StepIn()
        {
            XmlNode ret = null;
            try
            {
                if (Current != null)
                {
                    _parent = Current;
                    XmlNode prev = Current;
                    ret = Current = Current.FirstChild;
                    if (Current != null && prev != null) // update previous if current has changed and current was not null
                        Previous = prev;
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Steps out of the current childnodes context and selects the next node of the parent as the current node.
        /// This also works if the Current node is null because the advancing fell out of range or because
        /// StepIn was executed but there were no child _gridCoordinates of the current node.</summary>
        /// <returns></returns>
        public XmlNode StepOut()
        {
            XmlNode ret = null;
            try
            {
                XmlNode newparent = Parent.ParentNode;
                XmlNode prev = Current;
                ret = Current = Parent;  // current becomes the current parent node;
                if (Current != null && prev != null) // update previous if current has changed and current was not null
                    Previous = prev;
                Parent = newparent;
            }
            catch { }
            return ret;
        }


        /// <summary>Sets the current node to the next sibling node, or to null if the next sibling node does not
        /// exist, and returns that node.</summary>
        public XmlNode NextNode()
        {
            XmlNode ret = null;
            try
            {
                if (Current != null)
                {
                    XmlNode prev = Current;
                    ret = Current = Current.NextSibling;  // current becomes the next sibling node of current;
                    if (Current != null && prev != null) // update previous if current has changed and current was not null
                        Previous = prev;
                }
            }
            catch { }
            return ret;
        }

        /// <summary>Moves the current node to the first sibling node of the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        /// <param name="IncludeCurrent">If true then the new current node can also be the current node
        /// (if it satisfies the conditions), otherwise the search starts at the next sibling of the current node.</param>
        /// <returns></returns>
        public XmlNode NextNodeConditional(XmlNodeType NodeType, string NodeName, string NodeValue, bool IncludeCurrent)
        {

            XmlNode ret = null;
            try
            {
                if (Current != null)
                {
                    XmlNode prev = Current;
                    // Move current to the node that is found, or set it to null if the node is not found
                    // (meaning that the node falls out of the child _gridCoordinates collection):
                    ret = Current = GetNextNode(Current /* StartingNode */ ,
                            NodeType, NodeName, NodeValue, IncludeCurrent);
                    if (Current != null && prev != null) // update previous if current has changed and current was not null
                        Previous = prev;
                }
            }
            catch { }
            return ret;

            //XmlNode ReturnedString = null;
            //try
            //{
            //    if (Current != null)
            //    {
            //        bool stop = false;
            //        if (!IncludeCurrent)
            //            NextNode();
            //        while (!stop)
            //        {
            //            if (Current == null)
            //                stop = true;
            //            else
            //            {
            //                stop = true;
            //                if (NodeType != XmlNodeType.None) // type should be considered
            //                    if (Current.NodeType != NodeType)
            //                        stop = false;
            //                if (NodeName != null)  // Node name should also be considered
            //                    if (Current.LocalName != NodeName)
            //                        stop = false;
            //                if (NodeValue != null)
            //                    if (Current.Value != NodeValue)
            //                        stop = false;
            //            }
            //            if (!stop)
            //                NextNode();
            //        }
            //        ReturnedString = Current;
            //    }
            //}
            //catch { }
            //return ReturnedString;
        }


        // Changing the current node to the next OR the current node that satisfies given contitions:

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode NextNode(XmlNodeType NodeType, string NodeName, string NodeValue)
        {
            return NextNodeConditional(NodeType, NodeName, NodeValue, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode NextNode(XmlNodeType NodeType, string NodeName)
        {
            return NextNodeConditional(NodeType, NodeName, null /* NodeValue */, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that 
        /// is of the specified type.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        public XmlNode NextNode(XmlNodeType NodeType)
        {
            return NextNodeConditional(NodeType, null /* NodeName */, null /* NodeValue */, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node. Node type is not important.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode NextNode(string NodeName, string NodeValue)
        {
            return NextNodeConditional(XmlNodeType.None /* NodeType */, NodeName, NodeValue, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node. Node type is not important.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode NextNode(string NodeName)
        {
            return NextNodeConditional(XmlNodeType.None /* NodeType */, NodeName, null /* NodeValue */, false /* IncludeCurrent */);
        }


        // Changing the current node to the next OR the current node that satisfies given contitions:

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode NextOrCurrentNode(XmlNodeType NodeType, string NodeName, string NodeValue)
        {
            return NextNodeConditional(NodeType, NodeName, NodeValue, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode NextOrCurrentNode(XmlNodeType NodeType, string NodeName)
        {
            return NextNodeConditional(NodeType, NodeName, null /* NodeValue */, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that 
        /// is of the specified type.</summary>
        /// <param name="NodeType">If not None then the current node must have this type.</param>
        public XmlNode NextOrCurrentNode(XmlNodeType NodeType)
        {
            return NextNodeConditional(NodeType, null /* NodeName */, null /* NodeValue */, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node. Node type is not important.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode NextOrCurrentNode(string NodeName, string NodeValue)
        {
            return NextNodeConditional(XmlNodeType.None /* NodeType */, NodeName, NodeValue, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node. Node type is not important.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode NextOrCurrentNode(string NodeName)
        {
            return NextNodeConditional(XmlNodeType.None /* NodeType */, NodeName, null /* NodeValue */, true /* IncludeCurrent */);
        }

        // These two groups of functions that only cosider element _gridCoordinates:

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode NextElement(string NodeName, string NodeValue)
        {
            return NextNodeConditional(XmlNodeType.Element /* NodeType */, NodeName, NodeValue, false /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode NextElement(string NodeName)
        {
            return NextNodeConditional(XmlNodeType.Element /* NodeType */, NodeName, null /* NodeValue */, false /* IncludeCurrent */);
        }


        /// <summary>Moves the current node to the first sibling element after the current node.</summary>
        public XmlNode NextElement()
        {
            return NextNodeConditional(XmlNodeType.Element /* NodeType */, null /* NodeName */, null /* NodeValue */, false /* IncludeCurrent */);
        }


        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        /// <param name="NodeValue">If not null then the current node muust have this value.</param>
        public XmlNode NextOrCurrentElement(string NodeName, string NodeValue)
        {
            return NextNodeConditional(XmlNodeType.Element /* NodeType */, NodeName, NodeValue, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling node after the current node that satisfies the specified conditions, 
        /// and returns the current node.</summary>
        /// <param name="NodeName">If not null then the current node must have this name.</param>
        public XmlNode NextOrCurrentElement(string NodeName)
        {
            return NextNodeConditional(XmlNodeType.Element /* NodeType */, NodeName, null /* NodeValue */, true /* IncludeCurrent */);
        }

        /// <summary>Moves the current node to the current or the first sibling element after the current node.</summary>
        public XmlNode NextOrCurrentElement()
        {
            return NextNodeConditional(XmlNodeType.Element /* NodeType */, null /* NodeName */, null /* NodeValue */, true /* IncludeCurrent */);
        }


        #endregion  // Traversing

        // Stack<XmlNode> nextchild = new Stack<XmlNode>();

    }  // class XmlParser


    /// <summary>Class that enables custom parsing and building of an Xml document.</summary>
    public class XmlBuilder : XmlParser
    // $A Aug08 Oct08 Mar09;
    {

        private XmlDocument NewXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.CreateXmlDeclaration("1.0", null, "yes");
            return doc;
        }

        private XmlDocument NewXmlDocument(string RootName)
        {
            XmlDocument doc = NewXmlDocument();
            doc.AppendChild(Doc.CreateElement(RootName));
            return doc;
        }

        /// <summary>Creates a new Xml document.</summary>
        public void NewDocument()
        {
            Doc = NewXmlDocument();
        }

        /// <summary>Creates a new Xml document with a specified name of hte root element.</summary>
        /// <param name="RootName">Name of the root element.</param>
        public void NewDocument(string RootName)
        {
            Doc = NewXmlDocument(RootName);
        }


        // Definition of the node that is currently treated:

        protected XmlNode _new = null;

        /// <summary>Returns the Xml node that is currently treaded.
        /// All modifications apply to this node.
        /// If a newly created node exists that has not been inserted into the document yet then this node is returned.
        /// Otherwise, the current node is returned.</summary>
        public XmlNode Treated
        {
            get
            {
                if (_new != null)
                    return _new;
                else
                    return Current;
            }
        }

        /// <summary>Returns Treated if this node is Xml element, or null if it is not or if treated is null.</summary>
        public XmlElement TreatedElement
        {
            get { return Treated as XmlElement; }
        }

        /// <summary>Gets or sets the newly created node (if one exists).
        /// When the newly created node is inserted into the document, it is not accessible any more through
        /// NewNode or Treated properties.
        /// If set to a node from another document, a deep copy is created and imported.
        /// If set to a node from the current document that is already included in the document, a deep copy is created and set.
        /// If set to a node from the current document that is not included in the document, a reference to the node is set.</summary>
        public XmlNode NewNode
        {
            get { return _new; }
            set
            {
                try
                {
                    if (_new != null)
                        throw new Exception("There already exists a new node.");
                    if (value != null)
                    {
                        if (value.OwnerDocument == Doc)
                        {
                            if (ContainedInDocument(value, Doc))
                                _new = value.CloneNode(true /* deep */);
                            else
                                _new = value;
                        }
                        else
                        {
                            _new = Doc.ImportNode(value, true /* deep copy */ );
                        }
                    }
                }
                catch (Exception ex)
                {
                    R.ReportError(ex);
                }
            }
        }

        /// <summary>Returns NewNode if it is an Xml Element, or null if it is not.</summary>
        public XmlElement NewElement
        { get { return NewNode as XmlElement; } }

        /// <summary>Discards the newly created node and returns it if it exists.</summary>
        /// <returns>The discarded Xml node or null if there was no newly created node.</returns>
        public XmlNode Discard()
        {
            XmlNode ret = NewNode;
            NewNode = null;
            return ret;
        }



        /// <summary>Sets a named attribute of the currently treated node to a specific value.
        /// This only works if the currently treated node is an XML element.
        /// If an attribute with a specified name dose not yet exist then it is created,
        /// otherwise old value is overwritten.</summary>
        /// <param name="name">Name of the attribute to be set.</param>
        /// <param name="value">Value of the attribute.</param>
        /// <returns>The node for which the attribute has been set, or null if the attribute could not be set.</returns>
        public XmlNode SetAttribute(string name, string value)
        {
            XmlNode ret = null;
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("The attribute name is not specified properly.");
                else if (TreatedElement == null)
                    throw new XmlException("The currently treated node is null or is not an element.");
                else
                {
                    TreatedElement.SetAttribute(name, value);
                    ret = TreatedElement;
                }
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
            }
            return ret;
        }

        /// <summary>Removes the attribute with a specified name from the currently treated XML node, and 
        /// returns  this node or null if the operation was not successful (e.g. if the treated node is not
        /// defined or if the attribute with a specified name does not exist).
        /// This function does not report any errors.</summary>
        /// <param name="name">Name of the attribute to be removed.</param>
        /// <returns>The node on which atttribute is removed (which is the currently treated node), or 
        /// null if the operation could not be performed (this function does not report any errors).</returns>
        public XmlNode RemoveAttribute(string name)
        {
            XmlNode ret = null;
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("The attribute name is not specified properly.");
                else if (TreatedElement == null)
                    throw new XmlException("The currently treated node is not specified or is not an element.");
                else
                {
                    TreatedElement.RemoveAttribute(name);
                    ret = TreatedElement;
                }
            }
            catch { }
            return ret;
        }


        /// <summary>Sets value of the currently treated XML node.
        /// If the currently treated node is a text node then value is set directly.
        /// If the currently treated node is an element then value is set on its first chilld text node
        /// if such a node exists. If the element has no children, a text child node is created.</summary>
        public XmlNode SetValue(string value)
        {
            XmlNode ret = null;
            try
            {
                ret = Treated;
                if (ret == null)
                    throw new XmlException("Could not set a value: the current node is null.");
                if (ret.NodeType == XmlNodeType.Element)
                {
                    XmlNode nd = null;
                    if (ret.HasChildNodes)
                    {
                        // try to find the first text child node and set value to that node
                        nd = ret.FirstChild;
                        while (nd != null && nd.NodeType != XmlNodeType.Text)
                            nd = nd.NextSibling;
                        if (nd != null)
                            nd.Value = value;
                    }
                    // no text child node was found, ve therefore just create one:
                    if (nd == null)
                    {
                        nd = ret.PrependChild(Doc.CreateTextNode(value));
                    }

                }
                else
                    Treated.Value = value;

                //{
                //     Treated.Value = value;
                //     ReturnedString = Current;
                //     // TODO:
                //     // Check if this implementation is sufficient. Possibly there would be a need to 
                //     // treat different node differently.
                // }
                // throw new NotImplementedException("Not yet implemented."); 
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
                ret = null;
            }
            return ret;
        }

        /// <summary>Sets inner text of the currently treated XML node.</summary>
        public XmlNode SetInnerText(string value)
        {
            XmlNode ret = null;
            try
            {
                ret = Treated;
                if (Treated == null)
                    throw new XmlException("Could not set a value: the current node is null.");
                Treated.InnerText = value;
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
            }
            return ret;
        }


        // Removal of _gridCoordinates:

        /// <summary>Removes the specified node from the current XML document and returns that node.
        /// null is returned if the node can not be removed. 
        /// CAUTION: 
        /// Removing the node may destroy information about previous node and thus render the Back() operation impossible.
        /// It may also affect marks, which is not checked by any function but the BackMark() following removal may fail.</summary>
        /// <param name="node">XML node to be removed.</param>
        public XmlNode RemoveNode(XmlNode node)
        {
            XmlNode ret = null;
            try
            {
                if (node == null)
                    throw new ArgumentNullException("Can not remove an XML node: node is null.");
                else
                {
                    if (node == Current)
                    {
                        // The node to be removed is the current node, which nead special treatment.
                        // We move to the next node and take care of Previous, then remove the node:
                        XmlNode prev = Previous;
                        // Move to the next node
                        NextNode();
                        if (!ContainedInNode(prev, node))
                            Previous = prev;
                        else
                        {
                            Previous = Current;
                            if (Previous == null && Current == null)
                            {
                                // It is possible here that both are null, in this case perform a step out.
                                MoveTo(Parent);
                            }
                        }
                        XmlNode parentnode = node.ParentNode;
                        parentnode.RemoveChild(node);
                        ret = node;
                    }
                    else if (ContainedInNode(Current, node))
                    {
                        // The node to be removed is not the current node, but it contains the current node.
                        // To remove such a node, the current node must be first set to that node
                        // and then the node can be removed.
                        MoveTo(node);
                        ret = RemoveNode(node);
                    }
                    else if (ContainedInNode(Previous, node))
                    {
                        // In this case the information on previous node willl be lost:
                        Previous = Current;
                        ret = RemoveNode(node);
                    }
                    else if (node != Current && node != Previous)
                    {
                        // There are no conflicts, the node can be simply deleted:
                        XmlNode parentnode = node.ParentNode;
                        parentnode.RemoveChild(node);
                        ret = node;
                    }
                    else
                    {
                        throw new Exception("The node could not be deleted due to an unknown reason. Node name: " + node.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
            }
            return ret;
        }

        /// <summary>Removes the current node and returns the removed node or null if the operation does not succeed.
        /// The current node is moved to the next node and may become null if the current node was 
        /// the last one in the given child list.</summary>
        public XmlNode RemoveCurrent()
        {
            return RemoveNode(Current);
        }


        // Creation of _gridCoordinates:

        /// <summary>Creates a new node and sets this node as treated node.
        /// If there already exist a newly created node then it must be unassigned first (e.g. by Discard()), 
        /// otherwise this method fails.
        /// The new node also gets unassigned when it is inserted into the document.</summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        XmlNode CreateNode(XmlNodeType type, string name, string value, string namespaceURI)
        {
            XmlNode ret = null;
            try
            {
                if (NewNode != null)
                    throw new Exception("There already exists the newly created node.");
                _new = Doc.CreateNode(type, name, namespaceURI);
                ret = _new;
                if (!string.IsNullOrEmpty(value))
                    _new.InnerText = value;
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
                ret = null;
            }
            return ret;
        }

        // Placement of the newly created node:

        // TODO: do some additional testig of placement routines.

        /// <summary>Inserts the newly created node before the current node.
        /// The new node is set to null if the operation is successful (otherwise it remains unchanged).</summary>
        /// <returns>The newly created node if operation is successful, null otherwise.</returns>
        XmlNode InsertBefore()
        {
            XmlNode ret = null;
            try
            {
                if (NewNode == null)
                    throw new Exception("There is no newly created Xml node." + Environment.NewLine +
                            "  Maybe the node has already been inserted into document or it has been discarded.");
                if (Current != null)
                {
                    ret = Current.ParentNode.InsertBefore(NewNode, Current);
                    if (ret != null)
                        NewNode = null;  // the new node has been inserted, set it to null
                }
                else if (Parent != null)
                {
                    // If Current is not defined but Parent is, append the node to the end of the parent's child
                    // _gridCoordinates. This is because the current has obviously run out of range of the children table.
                    ret = Parent.AppendChild(NewNode);
                    if (ret != null)
                        NewNode = null;  // the new node has been inserted, set it to null
                }
                else
                    throw new Exception("Could not insert the newly created node before the current node or at the end of the current context.");
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
                ret = null;
            }
            return ret;
        }


        /// <summary>Inserts the newly created node after the current node.
        /// The new node is set to null if the operation is successful (otherwise it remains unchanged).</summary>
        /// <returns>The newly created node if operation is successful, null otherwise.</returns>
        XmlNode InsertAfter()
        {
            XmlNode ret = null;
            try
            {
                if (NewNode == null)
                    throw new Exception("There is no newly created Xml node." + Environment.NewLine +
                            "  Maybe the node has already been inserted into document or it has been discarded.");
                if (Current != null)
                {
                    ret = Current.ParentNode.InsertAfter(NewNode, Current);
                    if (ret != null)
                        NewNode = null;  // the new node has been inserted, set it to null
                }
                else if (Parent != null)
                {
                    // If Current is not defined but Parent is, append the node to the end of the parent's child
                    // _gridCoordinates. This is because the current has obviously run out of range of the children table.
                    ret = Parent.AppendChild(NewNode);
                    if (ret != null)
                        NewNode = null;  // the new node has been inserted, set it to null
                }
                else
                    throw new Exception("Could not insert the newly created node after the current node or at the end of the current context.");
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
                ret = null;
            }
            return ret;
        }

        /// <summary>Appends the newly created node after the last child of the current node.
        /// The new node is set to null if the operation is successful (otherwise it remains unchanged).</summary>
        /// <returns>The newly created node if operation is successful, null otherwise.</returns>
        XmlNode AppendChild()
        {
            XmlNode ret = null;
            try
            {
                if (NewNode == null)
                    throw new Exception("There is no newly created Xml node." + Environment.NewLine +
                            "  Maybe the node has already been inserted into document or it has been discarded.");
                if (Current == null)
                    throw new Exception("The current node is not defined, can not insert the newly created node as its child.");
                if (Current != null)
                {
                    ret = Current.AppendChild(NewNode);
                    if (ret != null)
                        NewNode = null;  // the new node has been inserted, set it to null
                    else throw new Exception("Child insertion failed. Node name: " + Current.Name + ".");
                }
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
                ret = null;
            }
            return ret;
        }

        /// <summary>Inserts the newly created node before the first child of the current node.
        /// The new node is set to null if the operation is successful (otherwise it remains unchanged).</summary>
        /// <returns>The newly created node if operation is successful, null otherwise.</returns>
        XmlNode PrependChild()
        {
            XmlNode ret = null;
            try
            {
                if (NewNode == null)
                    throw new Exception("There is no newly created Xml node." + Environment.NewLine +
                            "  Maybe the node has already been inserted into document or it has been discarded.");
                if (Current == null)
                    throw new Exception("The current node is not defined, can not insert the newly created node as its child.");
                if (Current != null)
                {
                    ret = Current.PrependChild(NewNode);
                    if (ret != null)
                        NewNode = null;  // the new node has been inserted, set it to null
                    else throw new Exception("Child insertion failed. Node name: " + Current.Name + ".");
                }
            }
            catch (Exception ex)
            {
                R.ReportError(ex);
                ret = null;
            }
            return ret;
        }





    }  // class XmlBuilder







}   // namespace IG.Lib












// Validation utilities: deleted from the eFakturiranje source on Oct. 21, 2008.

////****************************
//// Validation of XML document
////****************************

//// Validation Error Count
//int ErrorsCount = 0, ErorsCountLast = 0;
//// Validation Error Message
//string ValidationErrorMessages = "", ValidationLastMessage = "";
//XmlValidatingReader reader1 = null;  // Reader for old-style validation (now we use just XmlReader)
//XmlReader reader = null;
//FileStream fs = null;
//StreamWriter sw = null;
//int countGlava = 0, countPostavke = 0, countPostavkeVrednosti = 0;
//bool reportToGUI = false, logInCallBack = true;


//private void XMLValidationCallBack(object sender,
//                                     System.Xml.Schema.ValidationEventArgs AppArguments)
//{
//    try
//    {
//        if (PadoVariables.PadoTraceSwitch.TraceVerbose) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//            EventLogEntryType.Information, "clsEfaServer.XMLValidationCallBack", "Started ...",
//            PadoEnums.enTraceMsgSource.Server));
//        ValidationErrorMessages += AppArguments.Message + "\r\d2";
//        ValidationLastMessage = AppArguments.Message;
//        ErrorsCount++;
//        if (logInCallBack)
//        {
//            if (reader != null)
//                sw.WriteLine("\r\d2  !!Error " + ErrorsCount.ToString() +
//                    "[cG=" + countGlava + ",cP=" + countPostavke + ",cPV=" + countPostavkeVrednosti +
//                    "]: " + ValidationLastMessage + "\r\d2");
//            else if (reader1 != null)
//                sw.WriteLine("\r\d2  !!Error " + ErrorsCount.ToString() +
//                    " [cG=" + countGlava + ",cP=" + countPostavke + ",cPV=" + countPostavkeVrednosti +
//                    "] (" + reader1.LineNumber + "," + reader1.LinePosition + ") : " + ValidationLastMessage + "\r\d2");
//        }
//    }
//    catch (Exception ex)
//    {
//        if (PadoVariables.PadoTraceSwitch.TraceError) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//            EventLogEntryType.Error, "clsEfaServer.XMLValidationCallBack", "Error (re-thrown): " + ex.Message,
//            PadoEnums.enTraceMsgSource.Server));
//        throw ex;
//    }
//    finally
//    {
//        if (PadoVariables.PadoTraceSwitch.TraceVerbose) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//            EventLogEntryType.Information, "clsEfaServer.XMLValidationCallBack", "Finished.", PadoEnums.enTraceMsgSource.Server));
//    }
//}




//public int ValidateXmlDoc(string strXMLDoc, string SchemaFile, bool dologging, string logfile)
//{
//    try
//    {
//        if (PadoVariables.PadoTraceSwitch.TraceInfo) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//            EventLogEntryType.Information, "clsEfaServer.ValidateXmlDoc", "Started ...",
//            PadoEnums.enTraceMsgSource.Server));
//        // Initialize global variables (used for connection with error handler):
//        ErrorsCount = 0; ValidationErrorMessages = "";
//        countGlava = 0; countPostavke = 0; countPostavkeVrednosti = 0;
//        // Declare local objects
//        XmlTextReader tr = new XmlTextReader(SchemaFile);
//        //System.IO.StreamReader sr = new System.IO.StreamReader("c:\\EfaPaket1.xsd");
//        // Text reader object 
//        if (PadoVariables.PadoTraceSwitch.TraceVerbose) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//            EventLogEntryType.Information, "clsEfaServer.ValidateXmlDoc", "Importing schema...", PadoEnums.enTraceMsgSource.Server));
//        System.Xml.Schema.XmlSchemaCollection xsc = new System.Xml.Schema.XmlSchemaCollection();
//        xsc.Add(null, tr);

//        if (PadoVariables.PadoTraceSwitch.TraceVerbose) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//            EventLogEntryType.Information, "clsEfaServer.ValidateXmlDoc", "Initializing reader...", PadoEnums.enTraceMsgSource.Server));
//        // XML validator object
//        reader1 = new XmlValidatingReader(strXMLDoc,
//                     XmlNodeType.Document, null);
//        reader1.Schemas.Add(xsc);

//        // Add validation event handler
//        reader1.ValidationType = ValidationType.Schema;
//        reader1.ValidationEventHandler +=
//                 new System.Xml.Schema.ValidationEventHandler(XMLValidationCallBack);

//        if (dologging)
//        {
//            if (PadoVariables.PadoTraceSwitch.TraceVerbose) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//                EventLogEntryType.Information, "clsEfaServer.ValidateXmlDoc", "Preparing log file...", PadoEnums.enTraceMsgSource.Server));
//            try
//            {
//                dologging = false;
//                fs = new FileStream(logfile, FileMode.Create, FileAccess.Write);
//                sw = new StreamWriter(fs);
//                if (sw != null)
//                    dologging = true;
//            }
//            catch (Exception ex)
//            {
//                if (PadoVariables.PadoTraceSwitch.TraceError) Trace.WriteLine(PadoFunctions.FormatTraceMsg(EventLogEntryType.Error,
//                   "clsEfaServer.ValidateXmlDoc", "Error in validating XML / preparing log file. Details: " + ex.Message, PadoEnums.enTraceMsgSource.Server));
//                if (reportToGUI)
//                    ReportError(ex);
//            }
//        }
//        if (dologging)
//            logInCallBack = true;
//        int count = 0;
//        if (PadoVariables.PadoTraceSwitch.TraceInfo) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//            EventLogEntryType.Information, "clsEfaServer.ValidateXmlDoc", "Entering validation loop...", PadoEnums.enTraceMsgSource.Server));
//        // Validate XML data
//        while (reader1.Read())
//        {
//            // reader1.Re
//            if (dologging)
//            {
//                ++count;
//                if (PadoVariables.PadoTraceSwitch.TraceVerbose) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//                    EventLogEntryType.Information, "clsEfaServer.ValidateXmlDoc", "Validation loop, it. " + count, PadoEnums.enTraceMsgSource.Server));


//                // Count the number of times a given element appears:
//                if (reader1.NodeType == XmlNodeType.Element && reader1.Name == "EfaGlava")
//                    ++countGlava;
//                if (reader1.NodeType == XmlNodeType.Element && reader1.Name == "EfaPostavke")
//                    ++countPostavke;
//                if (reader1.NodeType == XmlNodeType.Element && reader1.Name == "EfaPostavkeVrednosti")
//                    ++countPostavkeVrednosti;
//                sw.WriteLine(count + " (" + reader1.LineNumber + "," + reader1.LinePosition +
//                     ")[cG=" + countGlava + ",cP=" + countPostavke +
//                     ",cPV=" + countPostavkeVrednosti +
//                     "]: messagelevel " + reader1.Depth + " " + reader1.NodeType +
//                     " node, name = \"" + reader1.Name + "\", value = \"" + reader1.Value +
//                     "\", pos.: ");
//                if (ErrorsCount % 100000 > 0)
//                {
//                    _generationData.ServerLog += " E ";
//                }
//                if (ErrorsCount > ErorsCountLast)
//                {
//                    ErorsCountLast = ErrorsCount;
//                }
//            }
//            if (ErrorsCount > 0 && ErrorsCount >= _generationData.maxValidationErrors)
//            {
//                // If maximal number of errors is exceeded, just break the validation!
//                Exception e = new Exception("Number of validation errors reached the prescribed maximum - "
//                    + _generationData.maxValidationErrors + "\r\nValidation is interrupted.\d2");
//                throw (e);
//            }
//        }
//        reader1.Close();
//    }
//    catch (Exception e)
//    {
//        if (PadoVariables.PadoTraceSwitch.TraceError) Trace.WriteLine(PadoFunctions.FormatTraceMsg(EventLogEntryType.Error,
//           "clsEfaServer.ValidateXmlDoc", "Error in validating XML. Details: " + e.Message, PadoEnums.enTraceMsgSource.Server));
//        ++ErrorsCount;
//    }
//    finally
//    {
//        reportToGUI = false;  // This must be set in the calling environment if we want a GUI interaction
//        if (sw != null)
//            sw.Close();
//        if (fs != null)
//            fs.Close();
//        if (PadoVariables.PadoTraceSwitch.TraceInfo) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//            EventLogEntryType.Information, "clsEfaServer.ValidateXmlDoc", "Finished.", PadoEnums.enTraceMsgSource.Server));
//    }
//    return ErrorsCount;
//}




//public int ValidateXmlDoc(string strXMLDoc, string SchemaFile)
//// Validation of an XML document with respect to the schema file.
////TODO: Choose which validation method to use!
//// Old style validation using the XMLValidatingReader (drclared obsolete by MS,
//// but it works better than just XMLVReader:
//{
//    reportToGUI = false;
//    int InternalErrorsCount = 0;
//    bool dologging = false;
//    try
//    {
//        if (PadoVariables.PadoTraceSwitch.TraceInfo) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//             EventLogEntryType.Information, "clsEfaServer.GUI_ValidateXmlDoc", "Started...", PadoEnums.enTraceMsgSource.Server));
//        // establish whether logging should be done:
//        if (_generationData.validationLogFile.Length > 0)
//        {
//            // prepare a stream writer object used for logging:
//            fs = new FileStream(_generationData.validationLogFile, FileMode.Create, FileAccess.ReadWrite);
//            try
//            {
//                if (!fs.CanWrite)
//                {
//                    dologging = false;
//                    _generationData.ServerLog += "\nLog file can not be open for writing: " + _generationData.validationLogFile + "\r\d2";
//                }
//                else
//                {
//                    // sw = new StreamWriter(fs);
//                    dologging = true;
//                    _generationData.ServerLog += "Valiidation is logged to the file \"" +
//                        _generationData.validationLogFile + "\".";
//                }
//            }
//            catch (Exception ex)
//            {
//                if (PadoVariables.PadoTraceSwitch.TraceError) Trace.WriteLine(PadoFunctions.FormatTraceMsg(EventLogEntryType.Error,
//                   "clsEfaServer.ValidateXmlDoc", "Error in validating XML. Details: " + ex.Message, PadoEnums.enTraceMsgSource.Server));
//                ReportError(ex);
//            }
//            finally
//            {
//                fs.Close();
//                fs = null;
//            }
//        }
//        else
//            dologging = false;
//        // Perform validation:
//        InternalErrorsCount = ValidateXmlDoc(strXMLDoc, SchemaFile, dologging, _generationData.validationLogFile);
//        // Raise exception if XML validation fails
//        if (InternalErrorsCount > 0)
//        {
//            // TODO: Delete this?
//            throw new Exception(ValidationErrorMessages);
//        }
//    }
//    catch (Exception ex)
//    {
//        if (PadoVariables.PadoTraceSwitch.TraceError) Trace.WriteLine(PadoFunctions.FormatTraceMsg(EventLogEntryType.Error,
//           "clsEfaServer.GUI_ValidateXmlDoc", "Error in XML Validation. Details: " + ex.Message, PadoEnums.enTraceMsgSource.Server));
//        // ReportError(error);
//        ++InternalErrorsCount;
//    }
//    finally
//    {
//        if (PadoVariables.PadoTraceSwitch.TraceInfo) Trace.WriteLine(PadoFunctions.FormatTraceMsg(
//             EventLogEntryType.Information, "clsEfaServer.GUI_ValidateXmlDoc", "Finished.", PadoEnums.enTraceMsgSource.Server));
//    }
//    return InternalErrorsCount;
//}













