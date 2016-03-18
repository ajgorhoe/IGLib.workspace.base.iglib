// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Text;
using System;
using System.Web.Script.Serialization;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using IG.Num;

namespace IG.Lib
{

    /// <summary>Interface for helper classes that perform serialization/deserialization of objects.</summary>
    public interface ISerializer
    {

        #region Serialization

        /// <summary>Serializes the specified object and outputs it to a stream.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="serializationStream">Stream on which the serialized object is output.</param>
        void Serialize<T>(T serializedObject, Stream serializationStream);

        /// <summary>Serializes the specified object and outputs it to a file.
        /// If the specified file exists then it is overwritten.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="filePath">Path to the file that serialized object is written to.</param>
        void Serialize<T>(T serializedObject, string filePath);

        /// <summary>Serializes the specified object and outputs it to a file.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="filePath">Path to the file that serialized object is written to.</param>
        /// <param name="append">If true then the generated contents is appended to a file.</param>
        void Serialize<T>(T serializedObject, string filePath, bool append);

        /// <summary>Serializes the specified object to a string and returns it.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="obj">Objejct to be serialized.</param>
        /// <returns>String containing the serialized object.</returns>
        string Serialize<T>(T obj);

        #endregion Serialization


        #region Deserialization

        /// <summary>Deserializes an object from the specified stream containing JSON representation of the object.
        /// Deserialized object is instantiated and returned.</summary>
        /// <typeparam name="T">Type of the deserialized object.</typeparam>
        /// <param name="deserializationStream">Stream from which object is deserialized.</param>
        /// <returns>Object that is deserialized from the stream.</returns>
        T Deserialize<T>(Stream deserializationStream);

        /// <summary>Deserializes an object from JSON - serialized string representation and returns it.</summary>
        /// <typeparam name="T">Type of the deserialized object.</typeparam>
        /// <param name="strSerialized">String containing the serialized object.</param>
        /// <returns>Object instantiated form the serialiyed representation.</returns>
        T DeserializeString<T>(string strSerialized);

        /// <summary>Deserializes an object from JSON - serialized file and returns it.</summary>
        /// <typeparam name="T">Type of the deserialized object.</typeparam>
        /// <param name="filePath">Path to thefile. File must contain the appropriately serialized 
        /// object of the correct type.</param>
        /// <returns>Object deserialized from the file.</returns>
        T DeserializeFile<T>(string filePath);

        #endregion Deserialization


    }  // interface ISerializer




    /// <summary>Base class for serialization helper classes.</summary>
    /// $A Igor May09;
    public abstract class SerializerBase: ISerializer
    {
        public SerializerBase()
        {  }


        #region Serialization

        /// <summary>Serializes the specified object and outputs it to a stream.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="serializationStream">Stream on which the serialized object is output.</param>
        public abstract void Serialize<T>(T serializedObject, Stream serializationStream);

        /// <summary>Serializes the specified object and outputs it to a file.
        /// If the specified file exists then it is overwritten.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="filePath">Path to the file that serialized object is written to.</param>
        public void Serialize<T>(T serializedObject, string filePath)
        {
            Serialize<T>(serializedObject, filePath, false /* append */);
        }

        /// <summary>Serializes the specified object and outputs it to a file.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="filePath">Path to the file that serialized object is written to.</param>
        /// <param name="append">If true then the generated contents is appended to a file.</param>
        public void Serialize<T>(T serializedObject, string filePath, bool append)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                throw new ArgumentException("Directory containing the specified file does not exist. Path: " + filePath);
            }
            FileMode mode;
            if (append)
                mode = FileMode.Append;
            else
                mode = FileMode.Create;
            using (FileStream fs = new FileStream(filePath, mode, FileAccess.Write))
            {
                Serialize<T>(serializedObject, fs);
            }
        }

        /// <summary>Serializes the specified object to a string and returns it.</summary>
        /// <typeparam name="T">Type of the object to be serialized.</typeparam>
        /// <param name="obj">Objejct to be serialized.</param>
        /// <returns>String containing the serialized object.</returns>
        public string Serialize<T>(T obj)
        {
            string ret;
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(obj, ms);
                ret = Encoding.Default.GetString(ms.ToArray());
                // ms.Dispose();  - using instead.
            }
            return ret;
        }

        #endregion Serialization


        #region Deserialization

        /// <summary>Deserializes an object from the specified stream containing JSON representation of the object.
        /// Deserialized object is instantiated and returned.</summary>
        /// <typeparam name="T">Type of the deserialized object.</typeparam>
        /// <param name="deserializationStream">Stream from which object is deserialized.</param>
        /// <returns>Object that is deserialized from the stream.</returns>
        public abstract T Deserialize<T>(Stream deserializationStream);

        /// <summary>Deserializes an object from JSON - serialized string representation and returns it.</summary>
        /// <typeparam name="T">Type of the deserialized object.</typeparam>
        /// <param name="strSerialized">String containing the serialized object.</param>
        /// <returns>Object instantiated form the serialiyed representation.</returns>
        public T DeserializeString<T>(string strSerialized)
        {
            T obj;
            using (MemoryStream deserializationStream = new MemoryStream(Encoding.Unicode.GetBytes(strSerialized)))
            {
                obj = Deserialize<T>(deserializationStream);
            }
            return obj;
        }

        /// <summary>Deserializes an object from JSON - serialized file and returns it.</summary>
        /// <typeparam name="T">Type of the deserialized object.</typeparam>
        /// <param name="filePath">Path to thefile. File must contain the appropriately serialized 
        /// object of the correct type.</param>
        /// <returns>Object deserialized from the file.</returns>
        public T DeserializeFile<T>(string filePath)
        {
            T obj;
            using (FileStream ms = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                obj = Deserialize<T>(ms);
            }
            return obj;
        }

        #endregion Deserialization


        #region Tests


        /// <summary>Tests different serialization methods with different data.</summary>
        /// <param name="outputDirectory">Directory where test files are kept.</param>
        public static void TestSerializationAll(string outputDirectory)
        {
            string directoryPath = Path.GetFullPath(outputDirectory);
            if (!Directory.Exists(directoryPath))
                throw new ArgumentException("Directory of the specified file path does not exist. " + Environment.NewLine
                    + "  Path: " + directoryPath);

            SerializerBase.TestSerialization<SerializerBase.SerializationTestClass>(
                new SerializerJson(true),
                // new SerializerBase.TestClass("TestClass object", 2.33),
                SerializerBase.SerializationTestClass.CreateTestObject(),
                Path.Combine(directoryPath,"Serialized.json"),
                true /* firstStep */, true /* secondStep */);


            // TODO: finish example with serialization of analysis input and analysis results, 
            // arrange this to separate functions!


            AnalysisResults anres = AnalysisResults.CreateExample(3, 2, true, true, true, true, true, true);
            SerializerBase.TestSerializationDto<AnalysisRequestDto, AnalysisResults, AnalysisResults>(
                new SerializerJson(true),
                anres,
                Path.Combine(directoryPath, "AnalysisInput.json"),
                true /* firstStep */, true /* secondStep */);
            //AnalysisRequestDTO aninInputDTO = new AnalysisRequestDTO();
            //aninInputDTO.CopyFrom(anres);
            //SerializerBase.TestSerialization<AnalysisRequestDTO>(
            //    new SerializerJson(true),
            //    aninInputDTO,
            //    Path.Combine(directoryPath, "AnalysisInput.json"),
            //    true /* firstStep */, true /* secondStep */);

            //SerializerBase.TestSerializationDto<AnalysisResultsDTO, AnalysisResults, AnalysisResults>(
            //    new SerializerJson(true),
            //    anres,
            //    Path.Combine(directoryPath, "AnalysisOutput.json"),
            //    true /* firstStep */, true /* secondStep */);
            AnalysisResultsDto anOutputDTO = new AnalysisResultsDto();
            anOutputDTO.CopyFrom(anres);
            SerializerBase.TestSerialization<AnalysisResultsDto>(
                new SerializerJson(true),
                anOutputDTO,
                Path.Combine(directoryPath, "AnalysisOutput.json"),
                true /* firstStep */, true /* secondStep */);

            
        }  // TestSerialization


        /// <summary>Test serialization performed by the specified serialization helper class.
        /// This test does not perform only serialization/deserialization, but also performs
        /// copying of an object to and from the corresponding DTO (Data Transfer Object). It is
        /// actually DTO that is serialized and deserialized.
        /// This function creates a test object, copies it to the corresponding DTO (Data Transfer
        /// object) of the specified type, serializes it and stores it to a file, 
        /// then deserializes the DTO (instantiates a new object for the stored record), copies the
        /// contents of the DTO to a new object, creates a new object and copies data to it from the DTO,
        /// and serializes the DTO again and writes it into another file in the same directory,
        /// but with a modified name. 
        /// Both files can then be compared in order to see if something was lost during conversions or 
        /// serialization/deserialization.
        /// </summary>
        /// <typeparam name="TypeDto">Declared type of the DTO to that is used for storing and 
        /// serializing object contents (state).
        /// The type must implements implement a default constructor.</typeparam>
        /// <typeparam name="Type">Type of the object to be serialized/deserialized through corresponding DTO.</typeparam>
        /// <param name="serializer">Serialization helper object that is used for serialization.</param>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="filePath">Name of the file to which serialized object si written.
        /// Another file is created in the same directory for storing deserialized/serialized object.</param>
        /// <param name="firstStep">If false then the first step is not performed (serialization to a file).
        /// This is useful if we already have a file with serialized object, and we would like to change it 
        /// ans observe how changes are reflected in the second file where deserialized/serialized object
        /// is stored.</param>
        /// <param name="secondStep">If false then the second step is not (deserialization/serialization) performed.</param>
        public static void TestSerializationDto<TypeDto, Type>(ISerializer serializer,
            Type serializedObject, string filePath, bool firstStep, bool secondStep)
            where Type : class
            where TypeDto : SerializationDto<Type>, new()
        {
            TestSerializationDto<TypeDto, Type, Type>(serializer,
                serializedObject, filePath, firstStep, secondStep);
        }



        /// <summary>Test serialization performed by the specified serialization helper class.
        /// This test does not perform only serialization/deserialization, but also performs
        /// copying of an object to and from the corresponding DTO (Data Transfer Object). It is
        /// actually the DTO that is serialized and deserialized.
        /// This function creates a test object, copies it to the corresponding DTO (Data Transfer
        /// object) of the specified type, serializes it and stores it to a file, 
        /// then deserializes the DTO (instantiates a new object for the stored record), copies the
        /// contents of the DTO to a new object, creates a new object and copies data to it from the DTO,
        /// and serializes the DTO again and writes it into another file in the same directory,
        /// with a modified name. 
        /// Both files can then be compared in order to see if something was lost during conversions or 
        /// serialization/deserialization.
        /// IMPORTANT REMARK:
        /// In most cases one will not need this function, but can use the variant with only two
        /// type parameters (BaseType omitted).
        /// </summary>
        /// <typeparam name="TypeDto">Type of the DTO to that is used for storing and 
        /// serializing object contents (state).
        /// The type must implements implement a default constructor.</typeparam>
        /// <typeparam name="Type">Type of the object to be serialized/deserialized through corresponding DTO.</typeparam>
        /// <typeparam name="BaseType">Base type from which Type inherits. It can be the same as Type. Serializer is
        /// initialized throug Type and VaseType.</typeparam>
        /// <param name="serializer">Serialization helper object that is used for serialization.</param>
        /// <param name="serializedObject">Object to be serialized.</param>
        /// <param name="filePath">Name of the file to which serialized object is written.
        /// Another file is created in the same directory for storing deserialized/serialized object.</param>
        /// <paparam name="firstStep">If false then the first step is not performed (serialization to a file).
        /// This is useful if we already have a file with serialized object, and we would like to change it 
        /// ans observe how changes are reflected in the second file where deserialized/serialized object
        /// is stored.</paparam>
        /// <param name="firstStep">If false then the second step is not (deserialization/serialization) performed.</param>
        /// <param name="secondStep">If false then the second step is not (deserialization/serialization) performed.</param>
        public static void TestSerializationDto<TypeDto, Type, BaseType>(ISerializer serializer,
            Type serializedObject, string filePath, bool firstStep, bool secondStep)
            where BaseType : class
            where Type : class, BaseType
            where TypeDto : SerializationDtoBase<Type, BaseType>, new()
        {
            string path = Path.GetFullPath(filePath);
            string filenamepure = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
                throw new ArgumentException("Directory of the specified file path does not exist. " + Environment.NewLine
                    + "  Path: " + filePath);

            string pathOriginal = Path.Combine(directory,
                filenamepure + extension);
            string pathRestored = Path.Combine(directory,
                filenamepure + "_restored" + extension);

            Console.WriteLine();
            Console.WriteLine("Test of serialization/deserialization/serialization of an object to a file");
            Console.WriteLine("Serialization is performed through a DTO.");
            if (serializedObject!=null)
                Console.WriteLine("Object type: " + serializedObject.GetType());
            Console.WriteLine("Serializer:  " + serializer.GetType());

            if (firstStep)
            {
                if (serializedObject == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Object to be serialized is not specified, skipping serialization step.");
                    Console.WriteLine();
                } else
                {
                    Console.WriteLine("Copying object to DTO and writing serialized object to file: " + pathOriginal);
                    // Copy object to the corresponding DTO, serialize the DTO and store it to a file:
                    TypeDto dtoOriginal = new TypeDto();
                    dtoOriginal.CopyFrom(serializedObject);
                    serializer.Serialize<TypeDto>(dtoOriginal, pathOriginal);
                }
            }
            if (secondStep)
            {
                // Desserialize the DTO corresponding to an object, copy it to a new object and back to a new DTO,
                // and serialize it again to another file:
                // First, deserialize object's state from file to a corresponding DTO:
                Console.WriteLine("Restoring a corresponding DTO form the file (deserializing)...");
                TypeDto restoredDto = serializer.DeserializeFile<TypeDto>(pathOriginal);
                // Then copy DTO to object:
                Console.WriteLine("Copying restored DTO to a new (restored) object:");
                Type restoredObject = null;
                restoredDto.CopyTo(ref restoredObject);
                // Copying restored object to another DTO:
                Console.WriteLine("Copying restored object to another DTO:");
                TypeDto anotherDto = new TypeDto();
                anotherDto.CopyFrom(restoredObject);
                // Serialize another DTO to another file to enable comparison:
                Console.WriteLine("Serializing another DTO to another file: ");
                Console.WriteLine(" " + pathRestored);
                serializer.Serialize<TypeDto>(anotherDto, pathRestored);
            }
            Console.WriteLine();
        }



        /// <summary>Test serialization performed by the specified serialization helper class.
        /// This function creates a test object, serializes it ans stores it to a file, 
        /// then deserializes it (instantiates a new object for the stored record), 
        /// and serializes that object and writes it into another file in the same directory,
        /// with a modified name. Both files can then be compared in order to see if something was lost.</summary>
        /// <typeparam name="SerializationTestClass">Declared type of the object to be serialized.</typeparam>
        /// <param name="serializer">Serialization helper object that is used for serialization.</param>
        /// <param name="serObject">Object to be serialized.</param>
        /// <param name="filePath">Name of the file to which serialized object si written.
        /// Another file is created in the same directory for storing deserialized/serialized object.</param>
        public static void TestSerialization<SerializationTestClass>(ISerializer serializer,
            SerializationTestClass serObject, string filePath)
        {
            TestSerialization<SerializationTestClass>(serializer, serObject, filePath, 
                true /* firstStep */, true /* secondStep */ );
        }

        /// <summary>Test serialization performed by the specified serialization helper class.
        /// This function creates a test object, serializes it ans stores it to a file, 
        /// then deserializes it (instantiates a new object for the stored record), 
        /// and serializes that object and writes it into another file in the same directory,
        /// with a modified name. Both files can then be compared in order to see if something was lost.</summary>
        /// <typeparam name="TypeSer">Declared type of the object to be serialized.
        /// Usually, TypeSer will be a type that implements ISerializer.</typeparam>
        /// <param name="serializer">Serialization helper object that is used for serialization.</param>
        /// <param name="serObject">Object to be serialized.</param>
        /// <param name="filePath">Name of the file to which serialized object si written.
        /// Another file is created in the same directory for storing deserialized/serialized object.</param>
        /// <paparam name="firstStep">If false then the first step is not performed (serialization to a file).
        /// This is useful if we already have a file with serialized object, and we would like to change it 
        /// ans observe how changes are reflected in the second file where deserialized/serialized object
        /// is stored.</paparam>
        /// <param name="firstStep">If false then the second step is not (deserialization/serialization) performed.</param>
        /// <param name="secondStep">If false then the second step is not (deserialization/serialization) performed.</param>
        public static void TestSerialization<TypeSer>(ISerializer serializer, 
            TypeSer serObject, string filePath, bool firstStep, bool secondStep)
        {
            string path = Path.GetFullPath(filePath);
            string filenamepure = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
                throw new ArgumentException("Directory of the specified file path does not exist. " + Environment.NewLine
                    + "  Path: " + filePath);

            string pathOriginal = Path.Combine(directory,
                filenamepure + extension);
            string pathRestored = Path.Combine(directory,
                filenamepure + "_restored" + extension);

            Console.WriteLine();
            Console.WriteLine("Test of serialization/deserialization/serialization of an object to a file");
            Console.WriteLine("Object type: " + serObject.GetType());
            Console.WriteLine("Serializer:  " + serializer.GetType());

            if (firstStep)
            {
                // Serialize the object and store it to a file:
                Console.WriteLine("Writing serialized object to file: " + pathOriginal);
                serializer.Serialize<TypeSer>(serObject, pathOriginal);
            }
            if (secondStep)
            {
                // Desserialize the object from file:
                Console.WriteLine("Restoring object form the file (deserializing)...");
                TypeSer restoredObject = serializer.DeserializeFile<TypeSer>(pathOriginal);

                // Serialize the deserialized object to another file to enable comparison:
                Console.WriteLine("Serializing the restored object to the file: ");
                Console.WriteLine(pathRestored);
                serializer.Serialize<TypeSer>(restoredObject, pathRestored);
            }
            Console.WriteLine();
        }

        /// <summary>Test serialization performed by the specified serialization helper class.
        /// This function creates a test object, serializes it ans stores it to a file, 
        /// then deserializes it (instantiates a new object for the stored record), 
        /// and serializes that object and writes it into another file in the same directory,
        /// with a modified name. Both files can then be compared in order to see if something was lost.</summary>
        /// <param name="filePath">Name of the file to which serialized object si written.
        /// Another file is created in the same directory for storing deserialized/serialized object.</param>
        /// <param name="serObject">>Object to be serialized.</param>
        public static void TestSerializationJSON(string filePath, object serObject)
        {
            // Prepare a helper serialization object:
            ISerializer serializer = new SerializerJson();
            TestSerialization<object>(serializer,
                serObject, filePath);
        }

        /// <summary>Test serialization performed by the JSon serializer.
        /// This function creates a test object, serializes it ans stores it to a file, 
        /// then deserializes it (instantiates a new object for the stored record), 
        /// and serializes that object and writes it into another file in the same directory,
        /// with a modified name. Both files can then be compared in order to see if something was lost.</summary>
        /// <param name="filePath">Path of te basic file used for saving a serialized object.</param>
        public static void TestSerializationJSON(string filePath)
        {
            // Craate a test object to be serialized: 
            SerializationTestClass serObject = SerializationTestClass.CreateTestObject();
            // Prepare a helper serialization object:
            ISerializer serializer = new SerializerJson();
            TestSerialization<SerializationTestClass>(serializer,
                serObject, filePath);
        }




        /// <summary>Writes the type of the object passed as parameter in two ways:
        /// by using tpye parameter of the generic method and by using object's GetType() method.
        /// The object itself is not printed out.</summary>
        /// <typeparam name="T">Specified type of the object (as type parameter of the method).</typeparam>
        /// <param name="obj">Object whose type printed.</param>
        public static void TestType<T>(T obj) { TestType<T>(obj, false); }


        /// <summary>Writes the type of the object passed as parameter in two ways:
        /// by using tpye parameter of the generic method and by using object's GetType() method.</summary>
        /// <typeparam name="T">Specified type of the object (as type parameter of the method).</typeparam>
        /// <param name="obj">Object whose type printed.</param>
        /// <param name="printObject">If true then the object itself is also printed out.</param>
        public static void TestType<T>(T obj, bool printObject)
        {
            Console.WriteLine();
            Console.WriteLine("Type test: ");
            Console.WriteLine("Type of generic type parameter: " + typeof(T));
            Console.WriteLine("Actual type of object passed:   " + obj.GetType());
            if (printObject)
            {
                Console.WriteLine("Object: ");
                Console.WriteLine(obj.ToString());
            }
            Console.WriteLine();
            //int i=1;
            //int? iNullable = i;
            //double d = 2.2;
            //Double dBoxed = d;
            //double? dNullable = d;
            //Console.WriteLine();
            //Console.WriteLine("Types of variables of variaous basic types vie Gettype() method:");
            //Console.WriteLine("int:     " + i.GetType());
            //Console.WriteLine("int?:    " + iNullable.GetType());
            //Console.WriteLine("double:  " + d.GetType());
            //Console.WriteLine("Double:  " + dBoxed.GetType());
            //Console.WriteLine("double?: " + dNullable.GetType());
            Console.WriteLine();
        }

        #endregion Tests


        #region Examples

        /// <summary>Testing the type of the object within a generic method.</summary>
        public static void ExampleTestType()
        {
            SerializerBase.TestClass objTestClass = new SerializerJson.TestClass(
                "Testclass name", 44.444);
            SerializerBase.TestClassDerived objTestClassDerived = new SerializerBase.TestClassDerived(
                "TestClassDerivedName", "TestClassDerivedDescription", 5511.5);

            Console.WriteLine("Passing TestClass ojbect as TestClass: ");
            SerializerBase.TestType<SerializerBase.TestClass>(objTestClass);
            Console.WriteLine("Passing TestClassDerived ojbect as TestClass: ");
            SerializerBase.TestType<SerializerBase.TestClass>(objTestClassDerived);
        }

        #endregion Examples


        #region TestClasses

        /// <summary>Simple class using for testing serialization.</summary>
        public class TestClass
        {
            public TestClass() : this("TestClass", 1.23) { }
            public TestClass(string name, double value)
            { Name = name; Value = value; }
            private string _name;
            private double _value;
            private bool _flag = true;
            public string Name { get { return _name; } set { _name = value; } }
            public double Value { get { return _value; } set { _value = value; } }
            public bool Flag
            { get { return _flag; } set { _flag = value; } }

            private double[][] _numbers = new double[2][] { new double[3] {1.1, 1.2, 1.3}, null /* new double[3] {2.1, 2.2, 2.3} */ };
            
            [ScriptIgnore]  // for JSON serialize to ignore this, since it does not support rectangular arrays
            public double[][] Numbers
            {
                get { return _numbers; }
                // set { _numbers = value; }
            }

            public override string ToString()
            { return base.ToString() + ", Name: " + Name + ", Value: " + Value; }
        }

        /// <summary>Simple derived class using for testing serialization.</summary>
        public class TestClassDerived : TestClass
        {
            public TestClassDerived()
                : this("TestClassDerived", "This is a derived test class.", 3.33)
            { }
            public TestClassDerived(string name, string description, double value)
                : base(name, value)
            { Name = name; Value = value; Description = description; }
            private string _description;
            public string Description
            { get { return _description; } set { _description = value; } }

            public override string ToString()
            { return base.ToString() + ", Description: " + Description; }
        }



        /// <summary>Interface for members of arrays in classes for testing serialization.</summary>
        /// $A Igor Jun10;
        public interface ISerializationTestArrayMember
        {
            int Index1 { get; set; }
            int Index2 { get; set; }
            string Name { get; set; }
            // string ModifiedName { get; set; }
            long LongNumber { get; set; }
            double DoubleNumber { get; set; }
        }

        /// <summary>Interface for classes for testing serialization.</summary>
        /// $A Igor Jun10;
        public interface ISerializationTestClass
        {

            string ObjectName
            { get; set; }

            string ObjectNameModified
            { get; set; }

            double DNum
            { get; set; }

            SerializationTestArrayMember SingleElement
            { get; set; }

            SerializationTestArrayMember[] Array1D
            { get; set; }

            SerializationTestArrayMember[][] JaggedArray2D
            { get; set; }

            //[ScriptIgnore]  // for JSON serialize to ignore this, since it does not support rectangular arrays
            //SerializationTestArrayMember[,] RectangularArray2D
            //{ get; set; }

        }


        /// <summary>Member of arrays in test classes for testing serialization.</summary>
        /// $A Igor Jun10;
        public class SerializationTestArrayMember : ISerializationTestArrayMember
        {
            public SerializationTestArrayMember() :
                this(555, 555.55, "Object of type SerializationTestArrayMember")
            {
            }

            public SerializationTestArrayMember(long longNum, double doubleNum, string str)
            {
                this.LongNumber = longNum;
                this.DoubleNumber = doubleNum;
                this.Name = str;
            }

            private int _ind1;
            private int _ind2;
            private long _longNumber = 0;
            private double _doubleNumber = 0.0;
            private string _str = null;

            public virtual int Index1
            { get { return _ind1; } set { _ind1 = value; } }

            public virtual int Index2
            { get { return _ind2; } set { _ind2 = value; } }

            public virtual string Name
            { get { return _str; } set { _str = value; ModifiedName = value + "  \"\\\" ČŠŽĆĐčšžćđ - Modified"; } }

            protected string _modifiedName;

            protected virtual string ModifiedName
            { get { return _modifiedName; } set { _modifiedName = value; } }

            public virtual long LongNumber
            { get { return _longNumber; } set { _longNumber = value; } }

            public virtual double DoubleNumber
            { get { return _doubleNumber; } set { _doubleNumber = value; } }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("  SerializationTestArrayMember object:");
                sb.AppendLine("    [Index1, Index2] = [" + Index1 + ", " + Index2 + "]");
                sb.AppendLine("    Name = " + Name);
                sb.AppendLine("    ModifiedName = " + ModifiedName);
                sb.AppendLine("    LongNumber = " + LongNumber);
                sb.AppendLine("    DoubleNumber = " + DoubleNumber);
                return sb.ToString();
            }

        }


        /// <summary>Test class for testing serialization.</summary>
        /// $A Igor Jun10;
        public class SerializationTestClass : ISerializationTestClass
        {

            public SerializationTestClass() :
                this("Object of type SerializationTestClass")
            {
            }

            public SerializationTestClass(string objectName)
            {
                this.ObjectName = objectName;
            }

            private string _objectName;

            private string _objectNameModified;

            private double _dnum;

            private bool _bflag;

            public virtual string ObjectName
            { get { return _objectName; } set { _objectName = value; ObjectNameModified = value + "  \"\\\" ČŠŽĆĐčšžćđ - Modified"; } }

            public virtual string ObjectNameModified
            { get { return _objectNameModified; } set { _objectNameModified = value; } }

            public virtual double DNum
            { get { return _dnum; } set { _dnum = value; } }

            public virtual bool BFlag
            { get { return _bflag; } set { _bflag = value; } }

            private SerializationTestArrayMember _singleElement;

            private SerializationTestArrayMember[] _testArray;

            private SerializationTestArrayMember[][] _testJaggedArray2d;

            [ScriptIgnore]  // for JSON serialize to ignore this, since it does not support rectangular arrays
            private SerializationTestArrayMember[,] _rectangularArray2d;

            public virtual SerializationTestArrayMember SingleElement
            { get { return _singleElement; } set { _singleElement = value; } }

            public virtual SerializationTestArrayMember[] Array1D
            { get { return _testArray; } set { _testArray = value; } }

            public virtual SerializationTestArrayMember[][] JaggedArray2D
            { get { return _testJaggedArray2d; } set { _testJaggedArray2d = value; } }

            //[ScriptIgnore]  // for JSON serialize to ignore this, since it does not support rectangular arrays
            //public virtual SerializationTestArrayMember[,] RectangularArray2D
            //{ get { return _rectangularArray2d; } set { _rectangularArray2d = value; } }


            private double[][][] _array3D;

            /// <summary>Three dimensional array of doubles.</summary>
            public double[][][] ZArray3D
            {
                get { return _array3D; }
                set { _array3D = value; }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine("  SerializationTestClass object:");
                sb.AppendLine("    ObjectName = " + ObjectName);
                sb.AppendLine("    ObjectNameModified = " + ObjectNameModified);
                sb.AppendLine("    DNum = " + DNum);
                sb.AppendLine("");
                sb.AppendLine("SingleElement: ");
                if (SingleElement == null)
                    sb.AppendLine("  null");
                else
                    sb.AppendLine(SingleElement.ToString());
                sb.AppendLine("Array1D: ");
                if (Array1D == null)
                    sb.AppendLine("  null array.");
                else
                {
                    for (int j = 0; j < Array1D.Length; ++j)
                    {
                        sb.AppendLine("Array1D[" + j + "]:");
                        if (Array1D[j] == null)
                            sb.AppendLine("  null");
                        else
                            sb.AppendLine(Array1D[j].ToString());
                    }
                }
                sb.AppendLine("JaggedArray2D: ");
                if (JaggedArray2D == null)
                    sb.AppendLine(" null array.");
                else
                {
                    for (int i = 0; i < JaggedArray2D.Length; ++i)
                    {
                        SerializationTestArrayMember[] array = JaggedArray2D[i];
                        if (array == null)
                        {
                            sb.AppendLine("Row " + i + " is null.");
                            sb.AppendLine("  ----");
                        }
                        else
                        {
                            for (int j = 0; i < array.Length; ++j)
                            {
                                sb.AppendLine("Element [" + i + "][" + j + "]:");
                                if (array[j] == null)
                                {
                                    sb.AppendLine("  null element.");
                                }
                                else
                                    sb.AppendLine(array[j].ToString());
                            }
                        }
                    }
                }

                //sb.AppendLine("RectangularArray2D: ");
                //if (RectangularArray2D == null)
                //    sb.AppendLine(" null array.");
                //else
                //{
                //    for (int i = 0; i < RectangularArray2D.GetLength(0); ++i)
                //    {
                //        for (int j = 0; i < RectangularArray2D.GetLength(1); ++j)
                //        {
                //            sb.AppendLine("Element [" + i + ", " + j + "]:");
                //            if (RectangularArray2D[i, j] == null)
                //            {
                //                sb.AppendLine("  null element.");
                //            }
                //            else
                //                sb.AppendLine(RectangularArray2D[i, j].ToString());
                //        }
                //    }
                //}

                return sb.ToString();
            }

            
            /// <summary>Initializes the 3D array on test object.</summary>
            /// <param name="d1">First dimension.</param>
            /// <param name="d2">Second dimension.</param>
            /// <param name="d3">Third dimension.</param>
            protected virtual void InitTestArray3D(int d1, int d2, int d3)
            {
                ZArray3D = new double[d1][][];
                for (int i = 0; i < d1; ++i)
                {
                    ZArray3D[i] = new double[d2][];
                    for (int j = 0; j < d1; ++j)
                    {
                        ZArray3D[i][j] = new double[d3];
                        for (int k = 0; k < d3; ++k)
                            ZArray3D[i][j][k] = 1000 * i + j + (double)k / 1000;
                    }
                }
            }


            /// <summary>Initializes the test object.</summary>
            /// <param name="d1">First dimension for 2D arrays of objects.</param>
            /// <param name="d2">Second dimension for 2D arrays of objects.</param>
            /// <param name="dim3d1">First dimension for 3D plain array.</param>
            /// <param name="dim3d2">Second dimension for 3D plain arrray.</param>
            /// <param name="dim3d3">Third dimension for 3D plain array.</param>
            public virtual void InitTestObject(int d1, int d2, int dim3d1, int dim3d2, int dim3d3)
            {
                DNum = 22.333;

                SingleElement = new SerializationTestArrayMember(1, 1.0, "Single Element");
                SingleElement.Index1 = 0;
                SingleElement.Index2 = 0;

                Array1D = new SerializationTestArrayMember[d2];
                for (int j = 0; j < d2; ++j)
                {
                    Array1D[j] = new SerializationTestArrayMember(j, j * 0.1, "Array element [" + j + "]");
                    Array1D[j].Index1 = 0;
                    Array1D[j].Index2 = j;
                }

                JaggedArray2D = new SerializationTestArrayMember[d1][];
                for (int i = 0; i < d1; ++i)
                {
                    JaggedArray2D[i] = new SerializationTestArrayMember[d2];
                    for (int j = 0; j < d2; ++j)
                    {
                        JaggedArray2D[i][j] = new SerializationTestArrayMember(10 * i + j, i + 0.1 * j,
                            "Jagged array element [" + i + "][" + j + "]");
                        JaggedArray2D[i][j].Index1 = i;
                        JaggedArray2D[i][j].Index2 = j;
                    }
                }

                //RectangularArray2D = new SerializationTestArrayMember[d1, d2];
                //for (int i = 0; i < d1; ++i)
                //{
                //    for (int j = 0; j < d2; ++j)
                //    {
                //        RectangularArray2D[i, j] = new SerializationTestArrayMember(10 * i + j, i + 0.1 * j,
                //            "Rectangular array element [" + i + "][" + j + "]");
                //        RectangularArray2D[i, j].Index1 = i;
                //        RectangularArray2D[i, j].Index2 = j;
                //    }
                //}

                // TODO: initialize other fields, too!

                InitTestArray3D(dim3d1,dim3d2,dim3d3);


            }


            public static SerializationTestClass CreateTestObject()
            {
                return CreateTestObject(2, 3, 3, 4, 5);
            }

            /// <summary>Creates an object for testing serialization.</summary>
            /// <param name="d1">First dimension for 2D arrays of objects.</param>
            /// <param name="d2">Second dimension for 2D arrays of objects.</param>
            /// <param name="dim3d1">First dimension for 3D plain array.</param>
            /// <param name="dim3d2">Second dimension for 3D plain arrray.</param>
            /// <param name="dim3d3">Third dimension for 3D plain array.</param>
            /// <returns>Object of class SerializationTestClass that can be used to test serialization.</returns>
            public static SerializationTestClass CreateTestObject(int d1, int d2, int dim3d1, int dim3d2, int dim3d3)
            {
                SerializationTestClass obj = new SerializationTestClass("TestObjectToSerialize");
                obj.InitTestObject(d1, d2, dim3d1, dim3d2, dim3d3);
                return obj;
            }

        }  // SerializationTestClass


        #endregion TestClasses



    }  // class SerializerBase

}

