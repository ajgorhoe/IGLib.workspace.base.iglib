using System;
using System.Collections;

namespace xProcs.Common.Holiday
{
    /// <summary>
    /// Interface for holiday calculators
    /// </summary>
    public interface IHoliday
    {
        /// <summary>
        /// Returns a dictionary with the holidays for a country in a year
        /// the key in the dictionary is a DateTime object and the value is 
        /// a string with the description of that holiday
        /// </summary>
        /// <returns></returns>
        IDictionary GetHolidays(int year);
    }
}
