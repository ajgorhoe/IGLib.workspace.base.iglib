using System;
using System.Collections;

namespace xProcs.Common.Holiday
{
    /// <summary>
    /// German Holiday Calculation
    /// </summary>
    public class GermanHoliday : IHoliday
    {
        /// <summary>
        /// Returns a dictionary with the germany holidays
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public IDictionary GetHolidays(int year)
        {
            Hashtable result = new Hashtable();
    
            result.Add(new DateTime(year,1,1), "Neujahr");

            DateTime easter = DateUtility.Easter(year);
            
            result.Add(easter.AddDays(-2), "Karfreitag");
            result.Add(easter, "Ostern");
            result.Add(easter.AddDays(+1), "Ostermontag" );

            result.Add(new DateTime(year,5,1), "Maifeiertag" );

            result.Add(easter.AddDays(+39), "Christi Himmelfahrt");
            result.Add(easter.AddDays(+49), "Pfingsten");
            result.Add(easter.AddDays(+50), "Pfingstmontag");

            if (year >= 1990)
                result.Add(new DateTime(year,10,3), "Tag der Deutschen Einheit");
            if (year <= 1990)
                result.Add(new DateTime(year,6,17), "Tag der Deutschen Einheit");

            result.Add(new DateTime(year,12,25), "1. Weihnachtstag");
            result.Add(new DateTime(year,12,26), "2. Weihnachtstag");

            return result;
        }

       
    }
}


