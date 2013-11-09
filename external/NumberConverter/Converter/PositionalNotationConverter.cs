/*
 * 
 *  xProcs - Tool Collection
 * 
 *  Copyright 1995-2005  Stefan Böther,  xprocs@hotmail.de
 * 
 *  Feel free to use this library into your own projects. A small eMail that you
 *  use it and with a suggestion for improving and from where you came would be nice :-)
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace xProcs.Common.Converter
{    
    /// <summary>
    /// Converter for dual positional notations like binary, octal, hexadecimal numbers 
    /// </summary>
    public sealed class PositionalNotationConverter : ConverterBase
    {
        private byte radix;
        private byte bits;
        private string digits;        

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="radix">Radix of the system</param>
        public PositionalNotationConverter(byte radix)
        {
            this.radix = radix;
            bits = (byte) Math.Round(Math.Log(radix,2));
            digits = "0123456789ABCDEF".Substring(0, radix);
        }

        public override string ToString(int value)
        {
            StringBuilder result = new StringBuilder(128 >> bits);
            do
            {
                result.Insert(0, digits[value % radix]);
                value = value >> bits;
            } while (value != 0);

            return result.ToString();
        }

        public override int ToInt(string number)
        {
            int result = 0;
            foreach (char digit in number.ToUpper())
            {
                result = result << bits;

                int value = digits.IndexOf(digit);
                if (value == -1)
                    throw new ArgumentOutOfRangeException("number", number);

                result = result + value;
            }
            return result;
        }

    }
}
