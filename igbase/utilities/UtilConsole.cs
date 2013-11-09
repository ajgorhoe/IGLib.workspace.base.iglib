﻿// Copyright (c) Igor Grešovnik (2009), IGLib license; http://www2.arnes.si/~ljc3m2/igor/ioptlib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IG.Num;

using System.Runtime.InteropServices;


namespace IG.Lib
{

    /// <summary>Utilities for reading from a console.
    /// Just an alias for UtilConsole.</summary>
    /// $A Igor Feb10;
    public class Cons : UtilConsole
    {  }


    /// <summary>Utilities for reading from a console.</summary>
    /// $A Igor Feb10;
    public class UtilConsole
    {

        #region SystemConsoleManipulation

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        protected const int SW_HIDE = 0;
        protected const int SW_SHOW = 5;

        /// <summary>Hides the console window.</summary>
        public static void HideConsoleWindow()
        {
            try
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Environment.NewLine
                    + "Could not hide the console window. " + Environment.NewLine
                    + "Reason: " + ex.Message + Environment.NewLine);
            }
        }
        
        /// <summary>Shows the console window.</summary>
        public static void ShowConsoleWindow()
        {
            try
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_SHOW);
            }
            catch(Exception ex)
            {
                Console.WriteLine(Environment.NewLine
                    + "Could not show the console window. " + Environment.NewLine
                    + "Reason: " + ex.Message + Environment.NewLine);
            }
        }

        
        #endregion SystemConsoleManipulation

        #region ReadingValues

        // Reading of different various types of numerical values from the console:

        /// <summary>Reads an integer from a console and assigns it to a variable.
        /// User can input a non-integer to see current content, or insert an empty string to leave the old content.</summary>
        /// <param name="value">Variable to which the inserted value is assigned.</param>
        /// <returns>true if a new value has been assigned, false otherwise.</returns>
        public static bool Read(ref int value)
        {
            bool ret = false;
            string str = null;
            int i = 0;
            do
            {
                ++i;
                str = Console.ReadLine();
                if (string.IsNullOrEmpty(str))
                {
                    // Keep the old value and print it
                    Console.WriteLine("  = " + value.ToString());
                }
                else
                {
                    try
                    {
                        value = int.Parse(str);
                        ret = true;  // value has been changed
                        str = ""; // continue if successfully parsed
                    }
                    catch
                    {
                        if (str == "?")
                        {
                            Console.WriteLine();
                            Console.WriteLine("Insert an integer,");
                            Console.WriteLine("  \"?\" for help,");
                            Console.WriteLine("  non-numeric string to show current value,");
                            Console.WriteLine("  <Enter> to keep the old value.");
                            Console.WriteLine();
                        }
                        // Inserted string is not a valid representation of the output type,
                        // print the old value and request a new one:
                        if (i > 1)
                            Console.WriteLine("Insert an integer, \"?\" for help.");
                        Console.WriteLine("  Current value: " + value.ToString());
                        Console.Write("  New value:     ");
                    }
                }
            } while (!string.IsNullOrEmpty(str));
            return ret;
        }  // Read (ref int)

        /// <summary>Reads an integer (of type long) from a console and assigns it to a variable.
        /// User can input a non-integer to see current content, or insert an empty string to leave the old content.</summary>
        /// <param name="value">Variable to which the inserted value is assigned.</param>
        /// <returns>true if a new value has been assigned, false otherwise.</returns>
        public static bool Read(ref long value)
        {
            bool ret = false;
            string str = null;
            int i = 0;
            do
            {
                ++i;
                str = Console.ReadLine();
                if (string.IsNullOrEmpty(str))
                {
                    // Keep the old value and print it
                    Console.WriteLine("  = " + value.ToString());
                }
                else
                {
                    try
                    {
                        value = long.Parse(str);
                        ret = true;  // value has been changed
                        str = ""; // continue if successfully parsed
                    }
                    catch
                    {
                        if (str == "?")
                        {
                            Console.WriteLine();
                            Console.WriteLine("Insert an integer (long),");
                            Console.WriteLine("  \"?\" for help,");
                            Console.WriteLine("  non-numeric string to show current value,");
                            Console.WriteLine("  <Enter> to keep the old value.");
                            Console.WriteLine();
                        }
                        // Inserted string is not a valid representation of the output type,
                        // print the old value and request a new one:
                        if (i > 1)
                            Console.WriteLine("Insert an integer (long), \"?\" for help.");
                        Console.WriteLine("  Current value: " + value.ToString());
                        Console.Write("  New value:     ");
                    }
                }
            } while (!string.IsNullOrEmpty(str));
            return ret;
        }  // Read (ref long)

        /// <summary>Reads a floating point number (type double) from a console and assigns it to a variable.
        /// User can input a non-integer to see current content, or insert an empty string to leave the old content.</summary>
        /// <param name="value">Variable to which the inserted value is assigned.</param>
        /// <returns>true if a new value has been assigned, false otherwise.</returns>
        public static bool Read(ref double value)
        {
            bool ret = false;
            string str = null;
            int i = 0;
            do
            {
                ++i;
                str = Console.ReadLine();
                if (string.IsNullOrEmpty(str))
                {
                    // Keep the old value and print it
                    Console.WriteLine("  = " + value.ToString());
                }
                else
                {
                    try
                    {
                        value = double.Parse(str);
                        ret = true;  // value has been changed
                        str = ""; // continue if successfully parsed
                    }
                    catch
                    {
                        if (str == "?")
                        {
                            Console.WriteLine();
                            Console.WriteLine("Insert a number (double precision),");
                            Console.WriteLine("  \"?\" for help,");
                            Console.WriteLine("  non-numeric string to show current value,");
                            Console.WriteLine("  <Enter> to keep the old value.");
                            Console.WriteLine();
                        }
                        // Inserted string is not a valid representation of the output type,
                        // print the old value and request a new one:
                        if (i > 1)
                            Console.WriteLine("Insert a number (double precision), \"?\" for help.");
                        Console.WriteLine("  Current value: " + value.ToString());
                        Console.Write("  New value:     ");
                    }
                }
            } while (!string.IsNullOrEmpty(str));
            return ret;
        }  // Read (ref double)

        /// <summary>Reads a floating point number (type float) from a console and assigns it to a variable.
        /// User can input a non-integer to see current content, or insert an empty string to leave the old content.</summary>
        /// <param name="value">Variable to which the inserted value is assigned.</param>
        /// <returns>true if a new value has been assigned, false otherwise.</returns>
        public static bool Read(ref float value)
        {
            bool ret = false;
            string str = null;
            int i = 0;
            do
            {
                ++i;
                str = Console.ReadLine();
                if (string.IsNullOrEmpty(str))
                {
                    // Keep the old value and print it
                    Console.WriteLine("  = " + value.ToString());
                }
                else
                {
                    try
                    {
                        value = float.Parse(str);
                        ret = true;  // value has been changed
                        str = ""; // continue if successfully parsed
                    }
                    catch
                    {
                        if (str == "?")
                        {
                            Console.WriteLine();
                            Console.WriteLine("Insert a number (single precision),");
                            Console.WriteLine("  \"?\" for help,");
                            Console.WriteLine("  non-numeric string to show current value,");
                            Console.WriteLine("  <Enter> to keep the old value.");
                            Console.WriteLine();
                        }
                        // Inserted string is not a valid representation of the output type,
                        // print the old value and request a new one:
                        if (i > 1)
                            Console.WriteLine("Insert a number (single precision), \"?\" for help.");
                        Console.WriteLine("  Current value: " + value.ToString());
                        Console.Write("  New value:     ");
                    }
                }
            } while (!string.IsNullOrEmpty(str));
            return ret;
        }  // Read (ref float)


        /// <summary>Reads a boolean from a console and assigns it to a variable.
        /// User can input a non-boolean to see current content, or insert an empty string to leave the old content.
        /// Eligible input to assign a new boolean value (strings are not case sensitive!):
        /// </summary>
        /// <param name="value">Variable to which the inserted value is assigned.</param>
        /// <returns>true if a new value has been assigned, false otherwise.</returns>
        public static bool Read(ref bool value)
        {
            bool ret = false;
            string str = null;
            int i = 0;
            do
            {
                ++i;
                str = Console.ReadLine();
                if (string.IsNullOrEmpty(str))
                {
                    // Keep the old value and print it
                    Console.WriteLine("  = " + value.ToString());
                }
                else
                {
                    try
                    {
                        try
                        {
                            value = bool.Parse(str);
                            ret = true;  // value has been changed
                            str = ""; // continue if successfully parsed
                        }
                        catch (Exception)
                        {
                            if (string.IsNullOrEmpty(str))
                                throw;
                            str = str.ToLower();
                            if (str == "0")
                                value = false;
                            else if (str == "1")
                                value = true;
                            else if (str == "false")
                                value = false;
                            else if (str == "true")
                                value = true;
                            else if (str == "no")
                                value = false;
                            else if (str == "yes")
                                value = true;
                            else if (str == "n")
                                value = false;
                            else if (str == "y")
                                value = true;
                            else throw; 
                            str = null;
                        }
                    }
                    catch
                    {
                        if (str == "?")
                        {
                            Console.WriteLine();
                            Console.WriteLine("Insert a boolean value,");
                            Console.WriteLine("  \"?\" for help,");
                            Console.WriteLine("  non-boolean string to show current value,");
                            Console.WriteLine("  <Enter> to keep the old value.");
                            Console.WriteLine("  Legal input: '0', '1', 'false', 'true', 'y', 'n', 'yes', 'no'.");
                            Console.WriteLine();
                        }
                        // Inserted string is not a valid representation of the output type,
                        // print the old value and request a new one:
                        if (i > 1)
                            Console.WriteLine("Insert a boolean, \"?\" for help.");
                        Console.WriteLine("  Current value: " + value.ToString());
                        Console.Write("  New value:     ");
                    }
                }
            } while (!string.IsNullOrEmpty(str));
            return ret;
        }  // Read (ref bool)


        /// <summary>Reads a string from a console and assigns it to a variable.
        /// User can input a ? to see current content, or insert an empty string to leave the old content.</summary>
        /// <param name="value">Variable to which the inserted value is assigned.</param>
        /// <returns>true if a new value has been assigned, false otherwise.</returns>
        public static bool Read(ref string value)
        {
            bool modified = false;
            string str = Console.ReadLine();
            if (str == "\\")
            {
                Console.WriteLine("  Enter literal string: ");
                str = Console.ReadLine();

            }
            else if (str == "\\" || str == "?" || str == "" || str == null)
            {
                if (str != "\\" /* && str != "?" */)
                {
                    Console.WriteLine("Current value: ");
                    Console.WriteLine("  \"" + value + "");
                }
                Console.WriteLine();
                Console.WriteLine("\"\\\", \"?\" or empty string inserted, choose one of the options.");
                Console.WriteLine("  <Enter>: keep the old value.");
                Console.WriteLine("  \"n\": null string.");
                Console.WriteLine("  \"e\": empty string.");
                Console.WriteLine("  \"\\\": enter literal string again.");
                Console.WriteLine("  Any other string: change value to that string.");
                str = Console.ReadLine();
                if (str == "" || str == null)
                {
                    // Keep the old value:
                    Console.WriteLine(" = \"{0}\"", value);
                }
                else if (str == "n")
                {
                    // null string
                    value = null;
                    modified = true;
                    Console.WriteLine(" = null");
                }
                else if (str == "e")
                {
                    // empty string
                    value = "";
                    modified = true;
                    Console.WriteLine(" = \"\"");
                }
                else if (value == "\\")
                {
                    Console.WriteLine("Insert literal string value again: ");
                    str = Console.ReadLine();
                    value = str;
                    modified = true;
                }
                else
                {
                    value = str;
                    modified = true;
                }
            }
            else
            {
                value = str;
                modified = true;
            }
            return modified;
        }


        protected const string _defaultVectorName = "vec";

        /// <summary>Reads a vector from a console and assigns it to the specified vector variable.</summary>
        /// <param name="vec">Vector variable to read-in the vector.</param>
        /// <returns>True if the value of <paramref name="vec"/> has been modified by the reading operation, false if not.</returns>
        public static bool Read(ref IVector vec)
        {
            return Read(ref vec, _defaultVectorName);
        }

        /// <summary>Reads a vector from a console and assigns it to the specified vector variable.</summary>
        /// <param name="vec">Vector variable to read-in the vector.</param>
        /// <param name="vecName">Name of the vector to be read in (used just in writing help strings to console).
        /// If null or empty string then default name is taken.</param>
        /// <returns>True if the value of <paramref name="vec"/> has been modified by the reading operation, false if not.</returns>
        public static bool Read(ref IVector vec, string vecName)
        {
            if (string.IsNullOrEmpty(vecName))
                vecName = _defaultVectorName;
            bool modified = false;
            bool leaveOldValue = false;
            Console.WriteLine("Leave old vector value (0/1)? ");
            UtilConsole.Read(ref leaveOldValue);
            if (!leaveOldValue)
            {
                int dim = 0;
                if (vec != null)
                    dim = vec.Length;
                Console.WriteLine("Vector dimension: ");
                UtilConsole.Read(ref dim);
                while (dim < 0)
                {
                    Console.WriteLine("Can not be less than 0.");
                    Console.Write("Vector dimension: ");
                    UtilConsole.Read(ref dim);
                }
                if (dim<=0)
                {
                    if (vec!=null)
                    {
                        modified = true;
                        vec = null;
                    }
                } else if (dim != vec.Length)
                {
                    Vector.Resize(ref vec, dim);
                    modified = true;
                }
                if (vec!=null)
                {
                    bool changeElements = true;
                    if (!modified)
                    {
                        Console.WriteLine("Insert vector elements (0/1)? ");
                        UtilConsole.Read(ref changeElements);
                    }
                    if (changeElements)
                    {
                        for (int i = 0; i < vec.Length; ++i)
                        {
                            double element = vec[i];
                            Console.Write(" " + vecName + "[" + i + "] = ");
                            UtilConsole.Read(ref element);
                            if (element != vec[i])
                            {
                                vec[i] = element;
                                modified = true;
                            }
                        }
                    }
                }
            }
            return modified;
        }

        protected const string _defaultMatrixName = "vec";

        /// <summary>Reads a matrix from console and assigns it to the specified matrix variable.</summary>
        /// <param name="mat">Matrix variable to hold the read-in matrix.</param>
        /// <returns>True if the value of <paramref name="mat"/> has been modified by the reading operation, false if not.</returns>
        public static bool Read(ref IMatrix mat)
        {
            return Read(ref mat, _defaultMatrixName);
        }

        /// <summary>Reads a matrix from console and assigns it to the specified matrix variable.</summary>
        /// <param name="mat">Matrix variable to hold the read-in matrix.</param>
        /// <param name="matName">Name of the matrix to be read in (used just in writing help strings to console).
        /// If null or empty string then default name is taken.</param>
        /// <returns>True if the value of <paramref name="mat"/> has been modified by the reading operation, false if not.</returns>
        public static bool Read(ref IMatrix mat, string matName)
        {
            if (string.IsNullOrEmpty(matName))
                matName = _defaultMatrixName;
            bool modified = false;
            bool leaveOldValue = false;
            Console.WriteLine("Leave old matrix value (0/1)? ");
            UtilConsole.Read(ref leaveOldValue);
            if (!leaveOldValue)
            {
                int dim1 = 0;
                int dim2 = 0;
                if (mat != null)
                {
                    dim1 = mat.RowCount;
                    dim2 = mat.ColumnCount;
                }
                Console.WriteLine("Number of matrix rows: ");
                UtilConsole.Read(ref dim1);
                Console.WriteLine("Number of matrix columns: ");
                UtilConsole.Read(ref dim2);
                while (dim1 < 0)
                {
                    Console.WriteLine("Number of rows can not be less than 0.");
                    Console.Write("Number of rows: ");
                    UtilConsole.Read(ref dim1);
                }
                while (dim2 < 0)
                {
                    Console.WriteLine("Number of columns can not be less than 0.");
                    Console.Write("Number of columns: ");
                    UtilConsole.Read(ref dim2);
                }
                if (dim1<=0 || dim2<=0)
                {
                    if (mat!=null)
                    {
                        modified = true;
                        mat = null;
                    }
                } else if (dim1 != mat.RowCount || dim2 != mat.ColumnCount)
                {
                    Matrix.Resize(ref mat, dim1, dim2);
                    modified = true;
                }
                if (mat!=null)
                {
                    bool changeElements = true;
                    if (!modified)
                    {
                        Console.WriteLine("Insert matrix elements (0/1)? ");
                        UtilConsole.Read(ref changeElements);
                    }
                    if (changeElements)
                    {
                        for (int i = 0; i < mat.RowCount; ++i)
                            for (int j=0; j < mat.ColumnCount; ++j)
                            {
                                double element = mat[i,j];
                                Console.Write(" " + matName + "[" + i + ", " + j +  "] = ");
                                UtilConsole.Read(ref element);
                                if (element != mat[i,j])
                                {
                                    mat[i,j] = element;
                                    modified = true;
                                }
                            }
                    }
                }
            }
            return modified;
        }

        /// <summary>Reads a password from console, masking the input as specified.</summary>
        /// <param name="value">Output parameter where the inserted password is stored.</param>
        /// <param name="printchar">Character that is output to the concole with every character input by the user.</param>
        /// <param name="printrandom">If thrue then random characters are output to console when password characters are typed in.</param>
        /// <returns>True if password has been read, false if not (i.e. empty string was inserted).</returns>
        public static bool ReadPwd(ref string value, string printchar, bool printrandom)
        {
            bool ret = false;
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    password += info.KeyChar;
                    if (printrandom)
                        Console.Write(UtilStr.RandomChar());
                    else
                        Console.Write(printchar);
                    info = Console.ReadKey(true);
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                    }
                    info = Console.ReadKey(true);
                }
            }
            Console.WriteLine();
            if (!string.IsNullOrEmpty(password))
            {
                value = password;
                ret = true;
            }
            return ret;
        }  //  ReadPwd(ref string, string, bool)

        /// <summary>Reads a password from console, masking the input as specified.</summary>
        /// <param name="value">Output parameter where the inserted password is stored.</param>
        /// <param name="printchar">Character that is output to the concole with every character input by the user.</param>
        /// <returns>True if password has been read, false if not (i.e. empty string was inserted).</returns>
        public static bool ReadPwd(ref string value, string printchar)
        {
            return ReadPwd(ref value, printchar, false /* printrandom */ );
        }

        /// <summary>Reads a password from console, masking the input as specified.</summary>
        /// <param name="value">Output parameter where the inserted password is stored.</param>
        /// <param name="printrandom">If true then a random character (alphabitic or numeric) is prineted for 
        /// each character typed in. Otherwise, * is printed.</param>
        /// <returns>True if password has been read, false if not (i.e. empty string was inserted).</returns>
        public static bool ReadPwd(ref string value, bool printrandom)
        {
            return ReadPwd(ref value, "*" /* printchar */ , printrandom);
        }

        /// <summary>Reads a password from console, masking the input by * characters.</summary>
        /// <param name="value">Output parameter where the inserted password is stored.</param>
        /// <returns>True if password has been read, false if not (i.e. empty string was inserted).</returns>
        public static bool ReadPwd(ref string value)
        {
            return ReadPwd(ref value, "*", false);
        }

        #endregion ReadingValues


        public static void Examples()
        {
            Console.WriteLine("Random string: ");
            Console.WriteLine(UtilStr.RandomString(600, CharType.Numeric, false));
            Console.WriteLine(); Console.WriteLine();

            long num = 22;
            Console.Write("Insert an integer: "); UtilConsole.Read(ref num);
            Console.WriteLine("Inserted number is: " + num.ToString());
            Console.WriteLine(); Console.WriteLine();

            string pwd = "abc";
            Console.WriteLine(); Console.WriteLine();
            Console.Write("Password: ");
            UtilConsole.ReadPwd(ref pwd, true);
            Console.WriteLine(); Console.WriteLine();
            Console.WriteLine("Password inserted: \"" + pwd + "\".");
        }


    }  // class UtilConsole
}

