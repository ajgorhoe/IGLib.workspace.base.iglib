/*
 * 
 *  xProcs - Tool Collection
 * 
 *  Copyright 1995-2005  Stefan Böther,  xprocs@hotmail.de
 * 
 *  Thanks for Julian Bucknall (TurboPower) for his UK holidays !
 * 
 */

using System;
using System.Collections;

namespace xProcs.Common.Holiday
{
    
    /// <summary>
    /// UK Holiday Calculation
    /// </summary>
    public class GreatBritainHoliday : IHoliday
    {
        /// <summary>
        /// Returns a dictionary with the germany holidays
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public IDictionary GetHolidays(int year)
        {
            Hashtable result = new Hashtable();
            DateTime date;

            result.Add(new DateTime(year, 1, 1), "'New Year's Day");

            DateTime easter = DateUtility.Easter(year);

            result.Add(easter.AddDays(-2), "Good Friday");
            result.Add(easter, "Easter Sunday");
            result.Add(easter.AddDays(+1), "Easter Monday");

            date = new DateTime(year, 5, 1);
            while (date.DayOfWeek != DayOfWeek.Monday) 
                date = date.AddDays(+1);
            result.Add(date, "May Day Bank Holiday");

            date = new DateTime(year, 5, 31);
            while (date.DayOfWeek != DayOfWeek.Monday)
                date = date.AddDays(+1);
            result.Add(date, "May Bank Holiday");


            date = new DateTime(year, 8, 31);
            while (date.DayOfWeek != DayOfWeek.Monday)
                date = date.AddDays(+1);
            result.Add(date, "August Bank Holiday");


            date = new DateTime(year, 12, 25);
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(+1);
            result.Add(date, "Christmas Day");

            date = date.AddDays(+1);
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(+1);
            result.Add(date, "Boxing Day");

            return result;
        }


    }
}

