// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace IG.Lib
{

    /// <summary>Contains a number of important constants used in Html.</summary>
    /// $A Igor feb12;
    public static class HtmlConst
    {

        #region Attributes

        /// <summary>The class attribute. Specifies one or more classnames for an element (refers to a class in a style sheet).</summary>
        public const string AttributeClass = "class";

        /// <summary>The id attribute. Specifies a unique id for an element.</summary>
        public const string AttributeId = "id";

        /// <summary>The style attribute. Specifies an inline CSS style for an element.</summary>
        public const string AttributeStyle = "style";

        /// <summary>The style attribute. Specifies extra information about an element.</summary>
        public const string AttributeTitle = "title";

        /// <summary>The text direction attribute. Specifies text direction for the content in an element.</summary>
        public const string AttributeTextDirection = "dir";

        /// <summary>The language attribute. Specifies the language of the element's content.</summary>
        public const string AttributeLanguage = "lang";

        /// <summary>The xml language attribute. Specifies the language of the element's content (for XHTML documents).</summary>
        public const string AttributeLanguageXml = "xml:lang";

        /// <summary>The shortcut key attribute. Specifies a shortcut key to activate/focus an element.</summary>
        public const string AttributeShortcutKey = "accesskey";

        /// <summary>The tab index attribute. Specifies the tabbing order of an element.</summary>
        public const string AttributeTabIndex = "tabindex";

        #endregion Attributes

    }

    /// <summary>Generates and composes a HTML document.</summary>
    /// <remarks>Each object of this class can be used for generation of only one HTML document.</remarks>
    /// $A Igor xx Feb12;
    public class HtmlWriter: IDisposable
    {

        #region Construction

        /// <summary>Prevent argument-less constructor.</summary>
        private HtmlWriter() { }
        
        /// <summary>Creates a new HTML generator and connests it with the specified file.</summary>
        /// <param name="filePath">Path to the HTML file that is created.</param>
        public HtmlWriter(string filePath): this(filePath, null)
        {  }

        
        /// <summary>Creates a new HTML generator and connests it with the specified file.</summary>
        /// <param name="filePath">Path to the HTML file that is created.</param>
        /// <param name="cssPath">Path (usually relative) to the CSS (Cascading Style Sheets) file.</param>
        public HtmlWriter(string filePath, string cssPath)
        {
            this.FilePath = filePath;
            this.CssPath = cssPath;
        }


        #endregion Construction

        #region Constants

        /// <summary>The class attribute. Specifies one or more classnames for an element (refers to a class in a style sheet).</summary>
        public string ConstAttributeClass { get { return HtmlConst.AttributeClass; } }

        /// <summary>The id attribute. Specifies a unique id for an element.</summary>
        public string ConstAttributeId { get { return HtmlConst.AttributeId; } }

        /// <summary>The style attribute. Specifies an inline CSS style for an element.</summary>
        public string ConstAttributeStyle { get { return HtmlConst.AttributeStyle; } }

        /// <summary>The style attribute. Specifies extra information about an element.</summary>
        public string ConstAttributeTitle { get { return HtmlConst.AttributeTitle; } }

        /// <summary>The text direction attribute. Specifies text direction for the content in an element.</summary>
        public string ConstAttributeTextDirection { get { return HtmlConst.AttributeTextDirection; } }

        /// <summary>The language attribute. Specifies the language of the element's content.</summary>
        public string ConstAttributeLanguage { get { return HtmlConst.AttributeLanguage; } }

        /// <summary>The xml language attribute. Specifies the language of the element's content (for XHTML documents).</summary>
        public string ConstAttributeLanguageXml { get { return HtmlConst.AttributeLanguageXml; } }

        /// <summary>The shortcut key attribute. Specifies a shortcut key to activate/focus an element.</summary>
        public string ConstAttributeShortcutKey { get { return HtmlConst.AttributeShortcutKey; } }

        /// <summary>The tab index attribute. Specifies the tabbing order of an element.</summary>
        public string ConstAttributeTabIndex { get { return HtmlConst.AttributeTabIndex; } }

        #endregion Constants


        protected bool _headWritten = false;

        protected bool _tailWritten = false;

        protected string _filePath;

        /// <summary>Path to file containing the generator.</summary>
        public string FilePath
        {
            get { return _filePath; }
            protected set { _filePath = value; }
        }

        #region DocumentLookAndProperties

        /// <summary>Number of spaces used in each level of indentation)</summary>
        public int IndentSpaces = 2;

        /// <summary>Current level of indentation.</summary>
        protected int IndentLevel = 0;

        /// <summary>Returns indentation string for the specified indentation level.</summary>
        /// <param name="level">Level of indentation for which indentation string is returned.</param>
        protected string GetIndent(int level)
        {
            if (level<=0)
                return "";
            StringBuilder sb = new StringBuilder();
            if (IndentSpaces<0)
                IndentSpaces = 0;
            int numSpaces = IndentSpaces * level;
            for (int i=0; i<numSpaces; ++i)
                sb.Append(" ");
            return sb.ToString();
        }

        /// <summary>Returns indentation string for the current indentation level.
        /// <para>Level is increased or decreased automatically with respect to what is done with the document.</para></summary>
        public string GetIndent()
        { return GetIndent(IndentLevel); }

        /// <summary>Returns current indentation level of the document.</summary>
        public int GetIndentLevel()
        { return IndentLevel; }

        protected string _cssPath;

        /// <summary>Path to the associated CSS file (Cascaded Style Sheets).
        /// <para>Must be set before the document head is written, otherwise it will have no effect.</para></summary>
        public string CssPath
        {
            get { return _cssPath; }
            set { _cssPath = value; }
        }

        protected string _styleString;

        /// <summary>CSS string with internal style definitions.</summary>
        public string DocumentStyle
        {
            get { return _styleString; }
            set { _styleString = value; }
        }

        protected string _documentTitle = "Generated HTML Document";

        public virtual string DocumentTitle
        {
            get { return _documentTitle; }
            set { _documentTitle = value; }
        }

        protected string _documentAuthor = "IGLIb (by Igor Gresovnik)";
        
        public virtual string DocumentAuthor
        {
            get { return _documentAuthor; }
            set { _documentAuthor = value; }
        }

        protected string _documentDescription = "This document was programatically generated by IGLib (Investigative Generic Library), http://www2.arnes.si/~ljc3m2/igor/iglib/";

        public virtual string DocumentDescription
        {
            get { return _documentDescription; }
            set { _documentDescription = value; }
        }

        protected List<string> _documentComments;

        /// <summary>Adds another commet to the document that will be written in the document head.
        /// <para>Must be called before the document head is written, otherwise the comment will not be written.</para></summary>
        /// <param name="commentText">Text of the comment to be added.</param>
        public void AddDocumentComment(string commentText)
        {
            if (!string.IsNullOrEmpty(commentText))
            {
                if (_documentComments == null)
                    _documentComments = new List<string>();
                _documentComments.Add(commentText);
            }
        }

        /// <summary>Returns an array of comments of the document that will be written in the document head.</summary>
        public string[] GetDocumentComments()
        {
            string[] ret = null;
            if (_documentComments != null)
                ret = _documentComments.ToArray();
            return ret;
        }


        protected List<string> _keywords;

        /// <summary>Adds a keyword to the document (keywords are written as meta tag).
        /// <para>Must be called before the document head is written, otherwise the keyword will not be written.</para></summary>
        /// <param name="keyword">Keyword to be added to the document.</param>
        public void AddSingleKeyWord(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                if (_keywords == null)
                    _keywords = new List<string>();
                _keywords.Add(keyword);
            }
        }

        /// <summary>Adds the specified keywords to the document (keywords are written as meta tag).
        /// <para>Must be called before the document head is written, otherwise the keyword will not be written.</para></summary>
        /// <param name="keywords">Keywords to be added.</param>
        public void AddKeywords(params string[] keywords)
        {
            if (keywords != null)
                for (int i = 0; i < keywords.Length; ++i)
                    AddSingleKeyWord(keywords[i]);
        }

        /// <summary>Returns an array of current keywords on the document.</summary>
        public string[] GetKeywords()
        {
            string[] ret = null;
            if (_keywords != null)
                ret = _keywords.ToArray();
            return ret;
        }


        #endregion DocumentLookAndProperties


        protected TextWriter _writer;

        /// <summary>Text writer used for writing on the document.
        /// <para>Lazy evaluation, automatically opened when first needed.</para></summary>
        public TextWriter Writer
        {
            get {
                if (_writer == null)
                {
                    if (_tailWritten)
                        throw new InvalidOperationException("This document has already been finished and can not be re-opened.");
                    if (!_headWritten)
                        _writer = new StreamWriter(FilePath); // Open a new document
                    else
                        _writer = new  StreamWriter(FilePath, true /*append */);
                    if (!_lockBeginDocument && !_headWritten)
                        BeginDocument();
                }
                return _writer;
            }
        }

        #region DocumentHead

        /// <summary>Closes the text writer used for assembling the HTML document.</summary>
        public void CloseWriter()
        {
            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }
        }

        /// <summary>Adds the title meta tag to the document.
        /// <para>Must be called within the method for writing document head.</para></summary>
        protected virtual void WriteDocumentTitle()
        {
            if (_headWritten)
                throw new InvalidOperationException("Document title can not be written because the document head has already been written.");
            AppendPlainText(GetIndent() + "<title>" + DocumentTitle + "</title>" + Environment.NewLine); 
        }

        /// <summary>Adds the style tag to the document.
        /// <para>Must be called within the method for writing document head.</para></summary>
        protected virtual void WriteDocumentStyle()
        {
            if (_headWritten)
                throw new InvalidOperationException("Document style can not be written because the document head has already been written.");
            AppendPlainText(GetIndent() + "<style>" + Environment.NewLine + DocumentStyle 
                + GetIndent() + "</style>" + Environment.NewLine); 
        }

        protected virtual void WriteCssPath()
        {
            if (_headWritten)
                throw new InvalidOperationException("Document CSS path can not be written because the document head has already been written.");
            if (!string.IsNullOrEmpty(CssPath))
            {
                AppendPlainText(GetIndent() + "<link rel=\"stylesheet\" type=\"text/css\" href=\"" 
                    + CssPath + "\" />" + Environment.NewLine);
            }
        }

        /// <summary>Writes document comments.
        /// <para>Must be called within the method for writing document head.</para></summary>
        protected virtual void WriteDocumentComments()
        {
            if (_headWritten)
                throw new InvalidOperationException("Document comments can not be written because the document head has already been written.");
            string[] commentStrings = GetDocumentComments();
            if (commentStrings!=null)
                if (commentStrings.Length > 0)
                {
                    for (int i = 0; i < commentStrings.Length; ++i)
                    {
                        AppendPlainText(GetIndent() + "<!-- " + commentStrings.Length + " -->");
                    }
                }
        }


        /// <summary>Wirtes the meta tag with specified pairs of field names and corresponding values.
        /// <para>Must be called within the method for writing document head.</para><</summary>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will comprise the meta tag.</param>
        protected virtual void WriteMetaTagGeneral(params string[] attributeNameValuePairs)
        {
            if (_headWritten)
                throw new InvalidOperationException("Meta tags can not be written because the document head has already been written.");
            if (IsAttributesDefined(attributeNameValuePairs))
            {
                if (attributeNameValuePairs.Length % 2 != 0)
                        throw new ArgumentException("Number of provided fields and values is not even.");
                AppendPlainText(GetIndent() + "<meta");
                AppendAttributes(attributeNameValuePairs);
                AppendPlainText(" />" + Environment.NewLine);
            }
        }

        /// <summary>Adds a contenbt meta tag.
        /// <para>Must be called within the method for writing document head.</para></summary>
        protected virtual void WriteMetaContent()
        {
            WriteMetaTagGeneral("http-equiv", "content-type", "content", "text/html; charset=UTF-8");
        }

        /// <summary>Adds a meta tag with specified values of the name and content fields.
        /// <para>Must be called within the method for writing document head.</para></summary>
        /// <param name="name">Name of the meta tag.</param>
        /// <param name="content">Content of the meta tag.</param>
        protected virtual void WriteMetaTag(string name, string content)
        {
            if (!string.IsNullOrEmpty(name))
                WriteMetaTagGeneral("name", name, "content", content);
        }

        /// <summary>Writes the author meta tag to the document.
        /// <para>Must be called within the method for writing document head.</para></summary>
        protected virtual void WriteMetaAuthor()
        { WriteMetaTag("author", DocumentAuthor); }

        /// <summary>Adds the audescription meta tag to the document.
        /// <para>Must be called within the method for writing document head.</para></summary>
        protected virtual void WriteMetaDescription()
        { WriteMetaTag("description", DocumentDescription); }


        /// <summary>Adds the keywords meta tag to the document.
        /// <para>Must be called within the method for writing document head.</para></summary>
        protected virtual void WriteMetaKeywords()
        {
            string[] keywords = GetKeywords();
            StringBuilder sb = new StringBuilder();
            if (keywords!=null)
            {
                if (keywords.Length > 0)
                {
                    for (int i = 0; i < keywords.Length; ++i)
                    {
                        sb.Append(keywords[i]);
                        if (i < keywords.Length - 1)
                            sb.Append(", ");
                    }
                    WriteMetaTag("keywords", sb.ToString());
                }
            }
        }


        protected bool _lockBeginDocument = false;

        /// <summary>Begins the HTML document.
        /// <para>Adds HTML head if necessary.</para></summary>
        public void BeginDocument()
        {
            try
            {
                _lockBeginDocument = true;
                if (!_headWritten)
                {

                    AppendPlainText(
    @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"">
<html>
  <head>
");
                    ++IndentLevel; ++IndentLevel;

                    WriteMetaContent();
                    WriteDocumentTitle();
                    WriteMetaAuthor();
                    WriteMetaDescription();
                    WriteMetaKeywords();
                    WriteCssPath();
                    WriteDocumentComments();
                    WriteDocumentStyle();
                    AppendPlainText(
@"
  </head>
  <body>
" 
);
                }
            }
            finally
            {
                _lockBeginDocument = false;
                _headWritten = true;
            }
        }  // BeginDocument()

        #endregion DocumentHead


        /// <summary>Ends the HTML document. After this method is called, writing is not possible any more.</summary>
        public void EndDocument()
        {
            if (!_tailWritten)
            {
                _tailWritten = true;
                // Close all eventual open sections:
                while(_sectionLevel>0)
                {
                    EndSection();
                }
                AppendPlainText(
@"
  </body>
</html>"
);
            }
        }


        /// <summary>Adds plain text to HTML (without any markup, indentation or newlines, 
        /// unless contained in the text string).</summary>
        /// <param name="text">Text to be added.</param>
        public void AppendPlainText(string text)
        {
            Writer.Write(text);
        }

        /// <summary>Adds a line of plain text to HTML (without any markup, unless contained in the text string),
        /// indented and with newline fillowed.</summary>
        /// <param name="text">Text to be added.</param>
        public void AddPlainTextLine(string text)
        {
            Writer.Write(GetIndent() +  text + Environment.NewLine);
        }


        
        /// <summary>Adds the specified tag to the document.</summary>
        /// <param name="tagName">Tag name.</param>
        public void BeginTag(string tagName)
        {
            BeginTagWithText(tagName, null /* text */, null /* attributeNameValuePairs */);
        }

        /// <summary>Adds the specified tag to the document.</summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void BeginTag(string tagName, params string[] attributeNameValuePairs)
        {
            BeginTagWithText(tagName, null /* text */, attributeNameValuePairs);
        }

        /// <summary>Adds the specified tag containing the specified text to the document.</summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="text">Tag text.</param>
        public void BeginTagWithText(string tagName, string text)
        {
            BeginTagWithText(tagName, null /* text */, null /* attributeNameValuePairs */);
        }

        /// <summary>Adds the specified tag containing the specified text to the document.</summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="text">Tag text.</param>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void BeginTagWithText(string tagName, string text, params string[] attributeNameValuePairs)
        {
            TextWriter tw = Writer; // to perform initializations if not yet performed.
            AppendPlainText(GetIndent() + "<" + tagName);
            AppendAttributes(attributeNameValuePairs);
            AppendPlainText(">" + text + Environment.NewLine);
            ++IndentLevel;
        }


        /// <summary>Adds the specified tag containing the specified text to the document.</summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="text">Tag text.</param>
        /// <param name="withLineBreak">Whether line break is added or not.</param>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void EndTag(string tagName)
        {
            --IndentLevel;
            AppendPlainText(GetIndent() + "</" + tagName + ">" + Environment.NewLine);
        }



        int _sectionLevel = 0;

        /// <summary>Begins a new section.</summary>
        /// <param name="attributeNameValuePairs">Eventual attributes (in form of attribute name - value pairs).</param>
        public void BeginSection(params string[] attributeNameValuePairs)
        {
            //AppendPlainText(GetIndent() + "<section>" + Environment.NewLine);
            BeginTag("section", attributeNameValuePairs);
            ++_sectionLevel;
            // ++IndentLevel;
        }

        /// <summary>Ends a section.</summary>
        public void EndSection()
        {
            if (_sectionLevel < 1)
                throw new InvalidOperationException("Can not end section, nested section level is less than 1.");
            --_sectionLevel;
            EndTag("section");
            //--IndentLevel;
            //AppendPlainText(GetIndent() + "</section>" + Environment.NewLine);
        }


        /// <summary>Adds a newline to HTML.</summary>
        public void AddNewLine()
        {
            AppendPlainText(GetIndent() + "<br/>" + Environment.NewLine);
        }


        /// <summary>Returns true if the specified array of attribute name-value pairs defines any attributes, false otherwise.</summary>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values to be added to some tag.</param>
        /// <returns>true if the specified array of attribute name-value pairs means that any attributes are defined, false otherwise.</returns>
        protected bool IsAttributesDefined(string[] attributeNameValuePairs)
        {
            if (attributeNameValuePairs != null)
                if (attributeNameValuePairs.Length > 0)
                    return true;
            return false;
        }

        /// <summary>Writes definition of the specified attriutes of a HTML element to the HTML document.</summary>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the html element.</param>
        public void AppendAttributes(string[] attributeNameValuePairs)
        {
            if (IsAttributesDefined(attributeNameValuePairs))
            {
                if (attributeNameValuePairs.Length % 2 != 0)
                    throw new ArgumentException("Number of provided fields and values is not even.");
                AppendPlainText(" ");
                // AppendPlainText(GetIndent() + "<meta ");
                int which = 0;
                while (which < attributeNameValuePairs.Length)
                {
                    string fieldName = attributeNameValuePairs[which];
                    ++which;
                    if (string.IsNullOrEmpty(fieldName))
                        throw new ArgumentException("Attributes: name of the field No. " + (which % 2) + " is not specified. (null or empty string).");
                    string fieldValue = attributeNameValuePairs[which];
                    AppendPlainText(fieldName + "=\"" + fieldValue + "\"");
                    if (which < attributeNameValuePairs.Length - 1)
                        AppendPlainText(" ");
                    ++which;
                }
            }
        }

        /// <summary>Adds the specified tag containing the specified text to the document.</summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="text">Tag text.</param>
        /// <param name="withLineBreak">Whether line break is added or not.</param>
        public void AddTag(string tagName, string text, bool withLineBreak)
        {
            AddTag(tagName, text, withLineBreak, null /* attributeNameValuePairs */);
        }

        /// <summary>Adds the specified tag containing the specified text to the document.
        /// <para>Tag has no attributes. Line break is not added.</para></summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="text">Tag text.</param>
        public void AddTag(string tagName, string text)
        {
            AddTag(tagName, text, false /* withLineBreak */, null /* attributeNameValuePairs */);
        }

        /// <summary>Adds the specified tag containing the specified text to the document.
        /// <para>Line breakis not added.</para></summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="text">Tag text.</param>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void AddTag(string tagName, string text, params string[] attributeNameValuePairs)
        {
            AddTag(tagName, text, false /* withLineBreak */, attributeNameValuePairs);
        }


        /// <summary>Adds the specified tag containing the specified text to the document.</summary>
        /// <param name="tagName">Tag name.</param>
        /// <param name="text">Tag text.</param>
        /// <param name="withLineBreak">Whether line break is added or not.</param>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void AddTag(string tagName, string text, bool withLineBreak, params string[] attributeNameValuePairs)
        {
            TextWriter tw = Writer; // to perform initializations if not yet performed.
            string lineBreakString;
            if (withLineBreak)
                lineBreakString = "<br/>" + Environment.NewLine;
            else
                lineBreakString = "";
            AppendPlainText(GetIndent() + "<" + tagName);
            AppendAttributes(attributeNameValuePairs);
            AppendPlainText(">" + text);
            if (withLineBreak)
            {
                AppendPlainText("<br>" + Environment.NewLine + GetIndent());
            }
            AppendPlainText("</" + tagName + ">" + Environment.NewLine);
        }

        /// <summary>Adds a new paragraph with the specified text to the HTML document.</summary>
        /// <param name="paragraphText">Text of the paragraph.</param>
        /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void AddParagraph(string paragraphText, string[] attributeNameValuePairs)
        {
            AddTag("p", paragraphText, false /* withLineBreak */, attributeNameValuePairs);
        }

        /// <summary>Adds a new paragraph with the specified text to the HTML document.
        /// <para>The added HTML element has no attributes.</para></summary>
        /// <param name="paragraphText">Text of the paragraph.</param>
        public void AddParagraph(string paragraphText)
        {
            AddParagraph(paragraphText, null /* attributeNameValuePairs */);
        }

        /// <summary>Adds a new level 1 heading with the specified text to the HTML document.</summary>
        /// <param name="headingText">Text of the heading.</param>
       /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void AddHeading1(string headingText, params string[] attributeNameValuePairs)
        {
            AddTag("h1", headingText, false /* withLineBreak */, attributeNameValuePairs);
        }

        /// <summary>Adds a new level 1 heading with the specified text to the HTML document.
        /// <para>The added HTML element has no attributes.</para></summary>
        /// <param name="headingText">Text of the heading.</param>
        public void AddHeading1(string headingText)
        {
            AddHeading1(headingText, null /* attributeNameValuePairs */);
        }

        /// <summary>Adds a new level 2 heading with the specified text to the HTML document.</summary>
        /// <param name="headingText">Text of the heading.</param>
       /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void AddHeading2(string headingText, params string[] attributeNameValuePairs)
        {
            AddTag("h2", headingText, false /* withLineBreak */, attributeNameValuePairs);
        }

        /// <summary>Adds a new level 2 heading with the specified text to the HTML document.
        /// <para>The added HTML element has no attributes.</para></summary>
        /// <param name="headingText">Text of the heading.</param>
        public void AddHeading2(string headingText)
        {
            AddHeading2(headingText, null /* attributeNameValuePairs */);
        }

        /// <summary>Adds a new level 3 heading with the specified text to the HTML document.</summary>
        /// <param name="headingText">Text of the heading.</param>
       /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void AddHeading3(string headingText, params string[] attributeNameValuePairs)
        {
            AddTag("h3", headingText, false /* withLineBreak */, attributeNameValuePairs);
        }

        /// <summary>Adds a new level 3 heading with the specified text to the HTML document.
        /// <para>The added HTML element has no attributes.</para></summary>
        /// <param name="headingText">Text of the heading.</param>
        public void AddHeading3(string headingText)
        {
            AddHeading3(headingText, null /* attributeNameValuePairs */);
        }

        /// <summary>Adds a new level 4 heading with the specified text to the HTML document.</summary>
        /// <param name="headingText">Text of the heading.</param>
       /// <param name="attributeNameValuePairs">Pairs of attribute names and values that will be added to the tag.</param>
        public void AddHeading4(string headingText, params string[] attributeNameValuePairs)
        {
            AddTag("h4", headingText, false /* withLineBreak */, attributeNameValuePairs);
        }

        /// <summary>Adds a new level 4 heading with the specified text to the HTML document.
        /// <para>The added HTML element has no attributes.</para></summary>
        /// <param name="headingText">Text of the heading.</param>
        public void AddHeading4(string headingText)
        {
            AddHeading4(headingText, null /* attributeNameValuePairs */);;
        }

        

        /// <summary>Adds a linked image to the HTML document with specified custom size different than the
        /// original size.
        /// <para>It is possible to constraint height/width ratio to the original one, in this case the 
        /// original width and height must be specified.</para></summary>
        /// <param name="imageLink">Link to the image. Must not be null or empty string.</param>
        /// <param name="altText">Alternative text to be displayed when image is not shown.</param>
        /// <param name="captionText">Text below the image.</param>
        /// <param name="width">Width of the image in pixels. Takes effect when <param name="definsSize"=true.</param>
        /// <param name="height">Width of the image in pixels. Takes effect when <param name="definsSize"=true.</param>
        /// <param name="defineSize">Whether size (in pixels) of the displayed image is defined or not.</param>
        /// <param name="originalWidth">Original image width. Must be specified only if the ratio is constrained
        /// (i.e. if <paramref name="constrainRatio"/> = true)</param>
        /// <param name="originalHeight">Original image height. Must be specified only if the ratio is constrained
        /// (i.e. if <paramref name="constrainRatio"/> = true)</param>
        /// <param name="constrainRatio">If true then ratio between width and height is kept the same as in the original.
        /// In this case, original width and height must be specified, too.</param>
        /// <remarks><para>When ratio between image width and height is not constrained to the original ratio of the image
        /// (<paramref name="constrainRatio"/> = false) both width and height must be greater than 0 and they represent
        /// image dimensions in rendered HTML.</para>
        /// <para>When ratio between image width and height is constrained to the original ratio of the image
        /// (<paramref name="constrainRatio"/> = true), either the specified width or height may be less or equal to 0.
        /// In this case the remaining dimension is calculatd from the specified one in such a way that original ratio
        /// between width and height is preserved.</para>
        /// <para>When ratio between image width and height is constrained to the original ratio of the image
        /// (<paramref name="constrainRatio"/> = true) and both image width and height are specified, </para></remarks>
        public void AddImage(string imageLink, string altText, string captionText, int width, int height,
            int originalWidth, int originalHeight, bool constrainRatio)
        {
            if (width <= 0 || height <= 0)
            {
                if (constrainRatio)
                {
                    if (width <= 0 && height <= 0)
                        throw new ArgumentException("Both image width and height are less or equal to 0.");
                } else
                {
                    throw new ArgumentException("Image width or heighth is leee or equal than 0 while ratio is not constrained.");
                }
            }
            if (constrainRatio)
            {
                if (originalWidth <= 0 || originalHeight <= 0)
                    throw new ArgumentException("Original image width or height not specified while ratio between width and height is constrained.");
                double originalRatio = (double) originalWidth/(double) originalHeight;
                double ratio = 1.0;
                if (width>0 && height>0)
                    ratio = (double)width / (double)height;
                if (width<=0)
                    // Width not specified, calculate it from height:
                    width = (int)Math.Round((double) height * ratio);
                else if (height<=0)
                    // Height not specifid, calculate it form width:
                    height = (int)Math.Round((double)width / ratio);
                else if (ratio > originalRatio)
                    // Specified width is too large, reduce it in order to correspond to hwight:
                    width = (int)Math.Round((double) height * ratio);
                else if (ratio < originalRatio)
                    // Specified width is too large, reduce it in order to correspond to hwight:
                    height = (int)Math.Round((double)width / ratio);
            }
            AddImage(imageLink, altText, captionText, width, height, true /* defineSize */ );
        }


        /// <summary>Adds a linked image to the HTML document.</summary>
        /// <param name="imageLink">Link to the image. Must not be null or empty string.</param>
        /// <param name="altText">Alternative text to be displayed when image is not shown.</param>
        /// <param name="captionText">Text below the image.</param>
        /// <param name="width">Width of the image in pixels. Takes effect when <param name="definsSize"=true.</param>
        /// <param name="height">Width of the image in pixels. Takes effect when <param name="definsSize"=true.</param>
        /// <param name="defineSize">Whether size (in pixels) of the displayed image is defined or not.</param>
        public void AddImage(string imageLink, string altText, string captionText, int width, int height, bool defineSize)
        {
            if (string.IsNullOrEmpty(imageLink))
                return;
                //throw new ArgumentException("Image link is not specified (null or empty string).");
            string sizeString = null;
            if (defineSize)
                sizeString = " style=\" width: " + width + "px ; height: " + height + "px;\" " ;
            else
                sizeString = "";
            string altString = null;
            if (!string.IsNullOrEmpty(altText))
                altString = " alt=\"" + altText + "\" ";
            else
                altString = "";
            string srcString = " src=\"" + imageLink + "\" ";
            string tagName = "img";
            string lineBreakString = "<br/>" + Environment.NewLine;
            AppendPlainText(GetIndent() + "<" + tagName + sizeString + altString + srcString + "/>" + lineBreakString);
            AddImageCaption(captionText);
        }


        int _imageNum = 0;

        public string ImageCaption = "Figure";

        /// <summary>Adds Image caption to the current HTML document.</summary>
        /// <param name="captionText">Caption text.</param>
        protected void AddImageCaption(string captionText)
        {
            if (!string.IsNullOrEmpty(captionText))
            {
                ++_imageNum;
                AppendPlainText(GetIndent() + "&nbsp; ");
                if (ImageCaption != null)
                    AppendPlainText("<b>" + ImageCaption + " " + _imageNum + ": " + "</b>");
                AppendPlainText(captionText + "<br/>" + Environment.NewLine);
            }
        }


        #region IDisposable

        ~HtmlWriter()
        {
            Dispose(false);
        }


        private bool disposed = false;

        /// <summary>Implementation of IDisposable interface.</summary>
        public void Dispose()
        {
                Dispose(true);
                GC.SuppressFinalize(this);
        }

        /// <summary>Does the job of freeing resources. 
        /// <para></para>This method can be  eventually overridden in derived classes (if they use other 
        /// resources that must be freed - in addition to such resources of the current class).
        /// In the case of overriding this method, you should usually call the base.<see cref="Dispose"/>(<paramref name="disposing"/>).
        /// in the overriding method.</para></summary>
        /// <param name="disposing">Tells whether the method has been called form Dispose() method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free unmanaged objects resources:
                CloseWriter();

                // Set large objects to null:

                disposed = true;
            }
        }

        #endregion IDisposable


        #region Examples

        /// <summary>Writes an example HTML document.</summary>
        /// <param name="htmlDocumentPath">Path to the created HTML document.</param>
        /// <param name="styleSheetPath">Path to StyleSheet (CSS file).</param>
        /// <param name="imagePath1">Path to the first image included (can be null).</param>
        /// <param name="imagePath2">Path to the second image included (can be null).</param>
        public static void Example(string htmlDocumentPath, string styleSheetPath, string imagePath1, string imagePath2)
        {
            Console.WriteLine();
            Console.WriteLine("Testing HTML Generator...");
            Console.WriteLine("Generated document: "
                + Environment.NewLine + "  " + htmlDocumentPath);
            Console.WriteLine();

            // Create HTML generator (each one can be used only for one document), with specified
            // path to Cascading Style Sheet file:
            HtmlWriter html = new HtmlWriter(htmlDocumentPath, styleSheetPath);
            // Set some document properties, in this case title and keywords:
            html.DocumentTitle = "Example of generated report.";
            html.AddKeywords("Keyword 1", "Keyword 2", "Keyword 3");
            // Add some internal styling. This will override eventual settings from the CSS file.
            html.DocumentStyle = @"
        body
        {
            background-color:#b0d4ee;
        }
        h1
        {
            color:orange;
            text-align:center;
            font-style:italic;
            font-weight:bold;
        }
        p
        {
            font-family:""Times New Roman"";
            font-size:20px;
        }
";
            // Compose HTML body:
            // The first writing command will open file stream associated with the specified file
            // and write HTML head to the stream before writing is done.
            html.AddHeading1("Test HTML document - Heading1", html.ConstAttributeClass, "HClass");
            html.AddHeading2("Heading2", html.ConstAttributeClass, "MyClass", html.ConstAttributeId, "FirstH2");
            html.AddParagraph("This is a paragraph in the HTML document.");
            html.AddParagraph("A couple of newlines will follow...");
            html.AddNewLine();
            html.AddNewLine();
            html.AddParagraph("This is another paragraph.");
            html.AddImage(imagePath1, "shuttle", "Space shuttle above the Earth.", 50, 50, false);
            html.AddParagraph("This is a paragraph after the image.");
            html.AddParagraph("This is another paragraph after the image.");
            html.AddImage(imagePath2, "Alt. text to station", "Space station.", 400, 300, true);
            html.BeginSection();
            html.AddHeading1("New section");
            html.AddParagraph("This is a paragaph in the new section.");
            html.AddParagraph("This is a another paragraph in the new section.");
            html.BeginSection(html.ConstAttributeClass, "NestedSection");
            html.AddParagraph("This paragraph is in a nested section.");
            html.EndSection();
            html.EndSection();
            html.AddParagraph("This paragraph is out of the section.");
            html.AddPlainTextLine("End of the document.");
            html.EndDocument();
            html.CloseWriter();
            UtilSystem.OpenFileInDefaultBrowser(htmlDocumentPath);

        }


        #endregion Examples


    } // HtmlWriter

}
