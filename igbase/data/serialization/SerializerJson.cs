// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using IG.Num;
using System.Collections.Generic;

namespace IG.Lib
{

    //TODO: take a look at http://www.lesnikowski.com/blog/index.php/json-net-formatter/ 

    /// <summary>Helper class for JSON serialization and deserialization.
    /// This class enables formatting of the generated JSON with propper indentation,
    /// which is not the case with its base class.
    /// JSON Serializer has several limitations, such as:
    ///   - cyclic references are not supported.
    ///   - rectangular arrays are not supported. Jagged arrays must be used.</summary>
    /// $A Igor jun09;
    public class SerializerJson: SerializerJsonBase
    {
        public SerializerJson()
            : this(true /* formatted */)
        { }

        public SerializerJson(bool formatted)
            : base()
        { this.Formatted = formatted; }

        #region Global

        private static SerializerJson _global = new SerializerJson();

        /// <summary>Gets a global helper object for JSON serialization.</summary>
        public static new SerializerJson Global
        {
            get
            {
                return _global;
            }
        }

        #endregion Global

        private bool _formatted;

        public bool Formatted
        { get { return _formatted; } set { _formatted = value; } }

        private JsonFormatter _formatter;

        public JsonFormatter Formatter
        {
            get
            {
                if (_formatter == null)
                {
                    _formatter = new JsonFormatter();
                    _formatter.NumIndent = 2;  // indent by 2 characters by default
                }
                return _formatter;
            }
            set { _formatter = value; }
        }

        #region Serialization

        /// <summary>Serializes the specified object and outputs it to a stream.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="serializationStream">Stream on which the serialized object is output.</param>
        public override void Serialize<T>(T serializedObject, Stream serializationStream)
        {
            if (!Formatted)
                base.Serialize<T>(serializedObject, serializationStream);
            else
            {
                // We want to format serialized JSON text. First we create an unformatted string representation
                // and then format it.
                string str;
                using (MemoryStream ms = new MemoryStream())
                {
                    base.Serialize<T>(serializedObject, ms);
                    str = Encoding.Default.GetString(ms.ToArray());
                    // ms.Dispose();  - using instead.
                }
                string outStr = Formatter.Format(str);
                using (StreamWriter sw = new StreamWriter(serializationStream))
                {
                    sw.Write(outStr);
                }
            }
        }

        #endregion Serialization

    }  // class SerializerJSon



    /// <summary>Helper class for JSON serialization and deserialization.
    /// This class does not enable formatting of the generated JSON (no indentation etc.).</summary>
    /// $A Igor jun09;
    public class SerializerJsonBase: SerializerBase, ISerializer
    {

        /// <summary>Creates a JSON serialization class.</summary>
        public SerializerJsonBase()
        {
        }

        #region Global 

        private static SerializerJsonBase _global = new SerializerJsonBase();

        /// <summary>Gets a global helper object for JSON serialization.</summary>
        public static SerializerJsonBase Global
        {
            get 
            {
                return _global;
            }
        }

        #endregion Global


        
        #region Serialization

        /// <summary>Serializes the specified object and outputs it to a stream.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="serializationStream">Stream on which the serialized object is output.</param>
        public override void Serialize<T>(T serializedObject, Stream serializationStream)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer =
                new System.Runtime.Serialization.Json.DataContractJsonSerializer(serializedObject.GetType());
            serializer.WriteObject(serializationStream, serializedObject);
        }

        ///// <summary>Serializes the specified object and outputs it to a file.
        ///// If the specified file exists then it is overwritten.</summary>
        ///// <typeparam name="T">Type of the object to be serialized.</typeparam>
        ///// <param name="serializedObject">Object to be serialized.</param>
        ///// <param name="filePath">Path to the file that serialized object is written to.</param>
        //public void Serialize<T>(T serializedObject, string filePath)
        //{
        //    Serialize<T>(serializedObject, filePath, false /* append */);
        //}

        ///// <summary>Serializes the specified object and outputs it to a file.</summary>
        ///// <typeparam name="T">Type of the object to be serialized.</typeparam>
        ///// <param name="serializedObject">Object to be serialized.</param>
        ///// <param name="filePath">Path to the file that serialized object is written to.</param>
        ///// <param name="append">If true then the generated contents is appended to a file.</param>
        //public void Serialize<T>(T serializedObject, string filePath, bool append)
        //{
        //    string directory = Path.GetDirectoryName(filePath);
        //    if (!Directory.Exists(directory))
        //    {
        //        throw new ArgumentException("Directory containing the specified file does not exist. Path: " + filePath);
        //    }
        //    FileMode mode;
        //    if (append)
        //        mode=FileMode.Append;
        //    else
        //        mode = FileMode.Create;
        //    using (FileStream fs = new FileStream(filePath, mode, FileAccess.Write))
        //    {
        //        Serialize<T>(serializedObject, fs);
        //    }
        //}

        ///// <summary>Serializes the specified object to a string and returns it.</summary>
        ///// <typeparam name="T">Type of the object to be serialized.</typeparam>
        ///// <param name="obj">Objejct to be serialized.</param>
        ///// <returns>String containing the serialized object.</returns>
        //public string Serialize<T>(T obj)
        //{
        //    string ret;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        Serialize(obj, ms);
        //        ret = Encoding.Default.GetString(ms.ToArray());
        //        // ms.Dispose();  - using instead.
        //    }
        //    return ret;
        //}

        #endregion Serialization


        #region Deserialization

        /// <summary>Deserializes an object from the specified stream containing JSON representation of the object.
        /// Deserialized object is instantiated and returned.</summary>
        /// <typeparam name="T">Type of the deserialized object.</typeparam>
        /// <param name="deserializationStream">Stream from which object is deserialized.</param>
        /// <returns>Object that is deserialized from the stream.</returns>
        public override T Deserialize<T>(Stream deserializationStream)
        {
            Type type;
            T obj = Activator.CreateInstance<T>();
            type = obj.GetType();
            // type = typeof(T);
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = 
                new System.Runtime.Serialization.Json.DataContractJsonSerializer(type);
            obj = (T) serializer.ReadObject(deserializationStream);
            return obj;
        }

        ///// <summary>Deserializes an object from JSON - serialized string representation and returns it.</summary>
        ///// <typeparam name="T">Type of the deserialized object.</typeparam>
        ///// <param name="strSerialized">String containing the serialized object.</param>
        ///// <returns>Object instantiated form the serialiyed representation.</returns>
        //public T DeserializeString<T>(string strSerialized)
        //{
        //    T obj;
        //    using (MemoryStream deserializationStream = new MemoryStream(Encoding.Unicode.GetBytes(strSerialized)))
        //    {
        //        obj = Deserialize<T>(deserializationStream);
        //    }
        //    return obj;
        //}

        ///// <summary>Deserializes an object from JSON - serialized file and returns it.</summary>
        ///// <typeparam name="T">Type of the deserialized object.</typeparam>
        ///// <param name="filePath">Path to thefile. File must contain the appropriately serialized 
        ///// object of the correct type.</param>
        ///// <returns>Object deserialized from the file.</returns>
        //public T DeserialzeFile<T>(string filePath)
        //{
        //    T obj;
        //    using (FileStream ms = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //    {
        //        obj = Deserialize<T>(ms);
        //    }
        //    return obj;
        //}


        #endregion Deserialization


        //#region StaticMethods

        //// REMARK: for similar classes, just implement a new Global property and copy static methods!

        //#region StaticSerialization


        //#endregion StaticSerialization


        ///// <summary>Serializes the specified object and outputs it to a stream.</summary>
        ///// <typeparam name="T">Type of the object to be serialized.</typeparam>
        ///// <param name="serializedObject">Object to be serialized.</param>
        ///// <param name="serializationStream">Stream on which the serialized object is output.</param>
        //public static void SSerialize<T>(T serializedObject, Stream serializationStream)
        //{ 
        //    Global.Serialize<T>(serializedObject, serializationStream); 
        //}

        ///// <summary>Serializes the specified object and outputs it to a file.
        ///// If the specified file exists then it is overwritten.</summary>
        ///// <typeparam name="T">Type of the object to be serialized.</typeparam>
        ///// <param name="serializedObject">Object to be serialized.</param>
        ///// <param name="filePath">Path to the file that serialized object is written to.</param>
        //public void SSerialize<T>(T serializedObject, string filePath)
        //{
        //    Global.Serialize<T>(serializedObject, filePath);
        //}

        ///// <summary>Serializes the specified object and outputs it to a file.</summary>
        ///// <typeparam name="T">Type of the object to be serialized.</typeparam>
        ///// <param name="serializedObject">Object to be serialized.</param>
        ///// <param name="filePath">Path to the file that serialized object is written to.</param>
        ///// <param name="append">If true then the generated contents is appended to a file.</param>
        //public void SSerialize<T>(T serializedObject, string filePath, bool append)
        //{
        //    Global.Serialize<T>(serializedObject, filePath, append);
        //}

        ///// <summary>Serializes the specified object to a string and returns it.</summary>
        ///// <typeparam name="T">Type of the object to be serialized.</typeparam>
        ///// <param name="obj">Objejct to be serialized.</param>
        ///// <returns>String containing the serialized object.</returns>
        //public string SSerialize<T>(T obj)
        //{
        //    return Global.Serialize<T>(obj);
        //}



        //#region StaticDeserialization


        ///// <summary>Deserializes an object from the specified stream containing JSON representation of the object.
        ///// Deserialized object is instantiated and returned.</summary>
        ///// <typeparam name="T">Type of the deserialized object.</typeparam>
        ///// <param name="deserializationStream">Stream from which object is deserialized.</param>
        ///// <returns>Object that is deserialized from the stream.</returns>
        //public static T Deserialize<T>(Stream deserializationStream)
        //{
        //    return Global.Deserialize<T>(deserializationStream);
        //}

        ///// <summary>Deserializes an object from JSON - serialized string representation and returns it.</summary>
        ///// <typeparam name="T">Type of the deserialized object.</typeparam>
        ///// <param name="strSerialized">String containing the serialized object.</param>
        ///// <returns>Object instantiated form the serialiyed representation.</returns>
        //public static T SDeserializeString<T>(string strSerialized)
        //{
        //    return Global.DeserializeString<T>(strSerialized);
        //}

        ///// <summary>Deserializes an object from JSON - serialized file and returns it.</summary>
        ///// <typeparam name="T">Type of the deserialized object.</typeparam>
        ///// <param name="filePath">Path to thefile. File must contain the appropriately serialized 
        ///// object of the correct type.</param>
        ///// <returns>Object deserialized from the file.</returns>
        //public static T SDeserialzeFile<T>(string filePath)
        //{
        //    return Global.DeserialzeFile<T>(filePath);
        //}


        //#endregion StaticDeserialization

        //#endregion StaticMethods


    }  // class SerializerJSON


    /// <summary>Formatting of JSON strings into human readable form.</summary>
    /// $A Igor jun09;
    public class JsonFormatter
    {

        private int _numIndent;

        /// <summary>Number of spaces used for indentation.</summary>
        public int NumIndent
        { get { return _numIndent; } set { _numIndent = value; } }

        /// <summary>Returns a formatted JSON string.</summary>
        /// <param name="jsonString">Original unformatted string.</param>
        /// <returns></returns>
        public string Format(string jsonString)
        {
            JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategyContext strategyContext =
                new JsonPrettyPrinterPlus.JsonPrettyPrinterInternals.JsonPPStrategyContext();
            strategyContext.SpacesPerIndent = NumIndent;
            JsonPrettyPrinterPlus.JsonPrettyPrinter pp = new JsonPrettyPrinterPlus.JsonPrettyPrinter(
                    strategyContext);
            string converted = pp.PrettyPrint(jsonString);
            return converted;
        }

        /// <summary>Reads a JSON string from a file, formats it, and writes it to the specified file
        /// (which can be the same as the original one).</summary>
        /// <param name="pathOriginal">Path to the original file.</param>
        /// <param name="pathConverted">Path to the converted file.</param>
        void FormatFile(string pathOriginal, string pathConverted)
        {
            if (!File.Exists(pathOriginal))
                throw new ArgumentException("File containing original JSON does not exist.");
            string jsonString = File.ReadAllText(pathOriginal);
            string converted = Format(jsonString);
            File.WriteAllText(pathConverted, converted);
        }

    }  // class JsonFormatter




    ///// <summary>String extensions necessary for the JsonFormatter class.</summary>
    //public static class StringExtensions
    //{
    //    public static string Repeat(this string str, int count)
    //    {
    //        return new StringBuilder().Insert(0, str, count).ToString();
    //    }

    //    public static bool IsEscaped(this string str, int index)
    //    {
    //        bool escaped = false;
    //        while (index > 0 && str[--index] == '\\') escaped = !escaped;
    //        return escaped;
    //    }

    //    public static bool IsEscaped(this StringBuilder str, int index)
    //    {
    //        return str.ToString().IsEscaped(index);
    //    }
    //}


    ///// <summary>Formatter for JSON strings. Adds newlines and indentation.</summary>
    //public class JsonFormatter2
    //{
    //    #region class members
    //    const string Space = " ";
    //    const int DefaultIndent = 0;
    //    public string Indent = "  ";
    //    static readonly string NewLine = Environment.NewLine;
    //    #endregion

    //    private enum JsonContextType
    //    {
    //        Object, Array
    //    }

    //    void BuildIndents(int indents, StringBuilder output)
    //    {
    //        indents += DefaultIndent;
    //        for (; indents > 0; indents--)
    //            output.Append(Indent);
    //    }


    //    bool inDoubleString = false;
    //    bool inSingleString = false;
    //    bool inVariableAssignment = false;
    //    char prevChar = '\0';

    //    Stack<JsonContextType> context = new Stack<JsonContextType>();

    //    bool InString()
    //    {
    //        return inDoubleString || inSingleString;
    //    }

    //    public string PrettyPrint(string input)
    //    {
    //        var output = new StringBuilder(input.Length * 2);
    //        char c;

    //        for (int i = 0; i < input.Length; i++)
    //        {
    //            c = input[i];

    //            switch (c)
    //            {
    //                case '{':
    //                    if (!InString())
    //                    {
    //                        if (inVariableAssignment || (context.Count > 0 && context.Peek() != JsonContextType.Array))
    //                        {
    //                            output.Append(NewLine);
    //                            BuildIndents(context.Count, output);
    //                        }
    //                        output.Append(c);
    //                        context.Push(JsonContextType.Object);
    //                        output.Append(NewLine);
    //                        BuildIndents(context.Count, output);
    //                    }
    //                    else
    //                        output.Append(c);

    //                    break;

    //                case '}':
    //                    if (!InString())
    //                    {
    //                        output.Append(NewLine);
    //                        context.Pop();
    //                        BuildIndents(context.Count, output);
    //                        output.Append(c);
    //                    }
    //                    else
    //                        output.Append(c);

    //                    break;

    //                case '[':
    //                    output.Append(c);

    //                    if (!InString())
    //                        context.Push(JsonContextType.Array);

    //                    break;

    //                case ']':
    //                    if (!InString())
    //                    {
    //                        output.Append(c);
    //                        context.Pop();
    //                    }
    //                    else
    //                        output.Append(c);

    //                    break;

    //                case '=':
    //                    output.Append(c);
    //                    break;

    //                case ',':
    //                    output.Append(c);

    //                    if (!InString() && context.Peek() != JsonContextType.Array)
    //                    {
    //                        BuildIndents(context.Count, output);
    //                        output.Append(NewLine);
    //                        BuildIndents(context.Count, output);
    //                        inVariableAssignment = false;
    //                    }

    //                    break;

    //                case '\'':
    //                    if (!inDoubleString && prevChar != '\\')
    //                        inSingleString = !inSingleString;

    //                    output.Append(c);
    //                    break;

    //                case ':':
    //                    if (!InString())
    //                    {
    //                        inVariableAssignment = true;
    //                        output.Append(Space);
    //                        output.Append(c);
    //                        output.Append(Space);
    //                    }
    //                    else
    //                        output.Append(c);

    //                    break;

    //                case '"':
    //                    if (!inSingleString && prevChar != '\\')
    //                        inDoubleString = !inDoubleString;

    //                    output.Append(c);
    //                    break;
    //                case ' ':
    //                    if (InString())
    //                        output.Append(c);
    //                    break;

    //                default:
    //                    output.Append(c);
    //                    break;
    //            }
    //            prevChar = c;
    //        }

    //        return output.ToString();
    //    }
    //}


    ///// <summary>Formatter for JSON strings. Adds newlines and indentation.</summary>
    //public class JsonFormatter1
    //{

    //    private string _indent = "  ";

    //    public string Indent
    //    { get { return _indent; } set { _indent = value; } }

    //    public string PrettyPrint(string input)
    //    {
    //        var output = new StringBuilder(input.Length * 2);
    //        char? quote = null;
    //        int depth = 0;

    //        for (int i = 0; i < input.Length; ++i)
    //        {
    //            char ch = input[i];

    //            switch (ch)
    //            {
    //                case '{':
    //                case '[':
    //                    output.Append(ch);
    //                    if (!quote.HasValue)
    //                    {
    //                        output.AppendLine();
    //                        output.Append(Indent.Repeat(++depth));
    //                    }
    //                    break;
    //                case '}':
    //                case ']':
    //                    if (quote.HasValue)
    //                        output.Append(ch);
    //                    else
    //                    {
    //                        output.AppendLine();
    //                        output.Append(Indent.Repeat(--depth));
    //                        output.Append(ch);
    //                    }
    //                    break;
    //                case '"':
    //                case '\'':
    //                    output.Append(ch);
    //                    if (quote.HasValue)
    //                    {
    //                        if (!output.IsEscaped(i))
    //                            quote = null;
    //                    }
    //                    else quote = ch;
    //                    break;
    //                case ',':
    //                    output.Append(ch);
    //                    if (!quote.HasValue)
    //                    {
    //                        output.AppendLine();
    //                        output.Append(Indent.Repeat(depth));
    //                    }
    //                    break;
    //                case ':':
    //                    if (quote.HasValue) output.Append(ch);
    //                    else output.Append(" : ");
    //                    break;
    //                default:
    //                    if (quote.HasValue || !char.IsWhiteSpace(ch))
    //                        output.Append(ch);
    //                    break;
    //            }
    //        }

    //        return output.ToString();
    //    }



    //}   // Class JSonFormatter





}