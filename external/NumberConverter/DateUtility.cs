using System;
using System.Collections.Generic;
using System.Text;

namespace xProcs.Common
{
    /// <summary>
    /// Enum type for the months
    /// </summary>
    public enum Month
    {
        // Indicates no month.
        None     =  0,
        // Indicates January.
        January  =  1,
        // Indicates February.
        February =  2,
        // Indicates March.
        March    =  3,
        // Indicates April.
        April    =  4,
        // Indicates May.
        May      =  5,
        // Indicates June.
        June     =  6,
        // Indicates July.
        July     =  7,
        // Indicates August.
        August   =  8,
        // Indicates September.
        September=  9,
        // Indicates October.
        October  = 10,
        // Indicates November.
        November = 11,
        // Indicates December.
        December = 12
    }       

    /// <summary>
    /// Some methods for date calculation
    /// </summary>
    public static class DateUtility
    {
        /// <summary>
        /// Returns the first day of the year for a given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime BeginOfYear(DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        /// <summary>
        /// Returns the last day of the year for a given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfYear(DateTime date)
        {
            return new DateTime(date.Year, 12, 31);
        }

        /// <summary>
        /// Returns the first day of a month for a given date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime BeginOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Returns the last day of a month for a given date
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public static DateTime EndOfMonth(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            if (month == 12)
            {
                year++;
                month=1;
            }
            else
                month++;
            return new DateTime(year,month,1).AddDays(-1);
        }


        /// <summary>
        /// Returns the first day of a quarter
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime BeginOfQuarter(DateTime date)
        {
            int month = (((date.Month -1) / 3)*3)+1;
            return new DateTime(date.Year, month, 1);
        }

        /// <summary>
        /// Returns the last day of a quarter
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public static DateTime EndOfQuarter(DateTime date)
        {
            return BeginOfQuarter(BeginOfMonth(date).AddMonths(+3)).AddDays(-1);
        }

        /// <summary>
        /// Check if the date is in a leapyear
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public static bool LeapYear(DateTime date)
        {
            int year = date.Year;
            return (year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0));
        }

        /// <summary>
        /// Caclulates the weeknumber for a given date
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns></returns>
        public static int WeekOfYear(DateTime date)
        {            
            int[] t1 = new int[7] {-1,  0,  1,  2,  3, -3, -2};
            int[] t2 = new int[7] {-4,  2,  1,  0, -1, -2, -3};

            DateTime newYear = BeginOfYear(date);
            int doy1 = date.DayOfYear + t1[(int)newYear.DayOfWeek];
            int doy2 = date.DayOfYear + t2[(int)date.DayOfWeek];

            if (doy1 <= 0) 
                return WeekOfYear(newYear.AddDays(-1));
            else
                if (doy2 >= EndOfYear(newYear).DayOfYear )
                    return 1;
                else
                    return (doy1-1)/7 +1;
        }

        /// <summary>
        /// Determines how much days a month has
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>Number of days in the month</returns>
        public static int DaysInMonth(DateTime date)
        {
            short[] daysPerMonth = new short[12] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int month = date.Month;
            if (month == 2 && LeapYear(date))
                return daysPerMonth[month - 1] + 1;
            else
                return daysPerMonth[month - 1];
        }


        /// <summary>
        /// Return the duration between two dates in months
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int AgeInMonths(DateTime start, DateTime end)
        {
            return (end.Year-start.Year) * 12 + (end.Month-start.Month);
        }

        /// <summary>
        /// Return the duration between two dates in years
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int AgeInYears(DateTime start, DateTime end)
        {
            return AgeInMonths(start, end) / 12;
        }

        /// <summary>
        /// Return the duration until today in months
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int AgeInMonths(DateTime start)
        {
            return AgeInMonths(start, DateTime.Today);
        }

        /// <summary>
        /// Return the duration until today in years
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int AgeInYears(DateTime start)
        {
            return AgeInYears(start, DateTime.Today);
        }

        /// <summary>
        /// Calculate the easter holiday for a year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime Easter(int year)
        {
            int month;
            int day;

            int k1 = 24;
            int k2 = 5;

            int r1 = year % 19;
            int r2 = year % 4;
            int r3 = year % 7;
            int r4 = (19 * r1 + k1) % 30;
            int r5 = (2 * r2 + 4 * r3 + 6 * r4 + k2) % 7;
            int A  = 22 + r4 + r5;

            if (A < 32) 
            {
                month = 3;
                day = A;
            }
            else
            {
                A = r4 + r5 - 9;
                month = 4;
                if (A == 26)
                    day = 19;
                else if (A != 25) 
                    day = A;
                else if (r4 == 18  && r5 > 10)
                    day = 18;
                else
                    day = 25;
            }
            
            return new DateTime(year,month,day);
        }
    }
}
