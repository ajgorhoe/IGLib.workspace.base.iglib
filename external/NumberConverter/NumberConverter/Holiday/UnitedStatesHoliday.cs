/*
 * 
 *  xProcs - Tool Collection
 * 
 *  Copyright 1995-2005  Stefan Böther,  xprocs@hotmail.de
 * 
 *  Thanks for Rob Minter for the United States Holidays !
 * 
 */


using System;
using System.Collections;

namespace xProcs.Common.Holiday
{
    /// <summary>
    /// United States Holiday Calculation
    /// </summary>
    public class UnitedStatesHoliday : IHoliday
    {
        /// <summary>
        /// Returns a dictionary with the germany holidays
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public IDictionary GetHolidays(int year)
        {
            Hashtable result = new Hashtable();
            
            // fixed 
            result.Add(new DateTime(year,  1, 1), "New Year's Day");
            result.Add(new DateTime(year,  1, 15), "Martin Luther King Jr. Day");
            result.Add(new DateTime(year,  2,  2), "Groundhog Day");
            result.Add(new DateTime(year,  2, 14), "Valentine Day");
            result.Add(new DateTime(year,  2, 12), "Lincoln's Birthday");
            result.Add(new DateTime(year,  2, 22), "Washington's Birthday");
            result.Add(new DateTime(year,  3, 17), "Saint Patrick's Day");
            result.Add(new DateTime(year,  6, 14), "Flag Day");
            result.Add(new DateTime(year,  7,  4), "Independence Day");
            result.Add(new DateTime(year, 10, 24), "United Nations Day");
            result.Add(new DateTime(year, 10, 31), "Halloween");
            result.Add(new DateTime(year, 11, 11), "Veterans Day");
            result.Add(new DateTime(year, 12, 25), "Christmas");

            int index;

            //  Feb movable holidays 
            index = 16 - (FirstDay(year,2)- 1);
            if (index < 15) index += 7;
            result.Add(new DateTime(year, 2, index), "Washington's Birthday (observed)" );

            // May movable holidays 
            index = 21 - (FirstDay(year,5)- 1);
            result.Add(new DateTime(year, 5, index), "Armed Forces Day" );

            index = 30 - (FirstDay(year,5)- 1);
            if (index < 25) index += 7;
            result.Add(new DateTime(year, 5, index), "Memorial Day");

            index = 15 - (FirstDay(year, 5) - 1);
            if (index > 14) index -= 7;
            result.Add(new DateTime(year, 5, index), "Mother's Day");

            // Jun movable holidays 
            index = 22 - (FirstDay(year,6)- 1);
            if (index > 21) index -= 7;
            result.Add(new DateTime(year, 6, index), "Father's Day");

            // Sep movable holidays 
            index = 9 - (FirstDay(year,9) - 1);
            if (index > 7) index -= 7;
            result.Add(new DateTime(year, 9, index), "Labor Day");

            // Oct movable holidays 
            index = 16 - (FirstDay(year,10)- 1);
            if (index > 12) index -= 7;
            result.Add(new DateTime(year, 10, index), "Columbus Day");

            // Nov movable holidays 
            index = 26 - (FirstDay(year, 11) - 1);
            if (index < 22) index += 7;
            result.Add(new DateTime(year, 11, index), "Thanksgiving");

            DateTime easter = DateUtility.Easter(year);

            result.Add(easter.AddDays(-2),  "Good Friday" );
            result.Add(easter,              "Easter" );
            result.Add(easter.AddDays(-7),  "Palm Sunday" );
            result.Add(easter.AddDays(-46), "Ash WednesDay" );
            
            return result;
        }

        private int FirstDay(int year, int month)
        {
            return (int) new DateTime(year,month,1).DayOfWeek;
        }

        


    }
}


/*
 *   with Items do begin
    { fixed holidays }
    Add('New Year''s Day', EncodeDate(Year, 1, 1));
    Add('Martin Luther King Jr. Day', EncodeDate(Year, 1, 15));
    Add('Groundhog Day', EncodeDate(Year, 2, 2));
    Add('Valentine Day', EncodeDate(Year, 2, 14));
    Add('Lincoln''s Birthday', EncodeDate(Year, 2, 12));
    Add('Washington''s Birthday', EncodeDate(Year, 2, 22));
    Add('Saint Patrick''s Day', EncodeDate(Year, 3, 17));
    Add('Flag Day', EncodeDate(Year, 6, 14));
    Add('Independence Day', EncodeDate(Year, 7, 4));
    Add('United Nations Day', EncodeDate(Year, 10, 24));
    Add('Halloween', EncodeDate(Year, 10, 31));
    Add('Veterans Day', EncodeDate(Year, 11, 11));
    Add('Christmas', EncodeDate(Year, 12, 25));

    { Feb movable holidays }
    FirstDay := DayOfWeek(EncodeDate(Year, 2, 1));
    Idx := 16 - (FirstDay - 1);
    if Idx < 15 then Inc(Idx, 7);
    Add('Washington''s Birthday (observed)', EncodeDate(Year, 2, Idx));

    { May movable holidays }
    FirstDay := DayOfWeek(EncodeDate(Year, 5, 1));
    Idx := 21 - (FirstDay - 1);
    Add('Armed Forces Day', EncodeDate(Year, 5, Idx));

    Idx := 30 - (FirstDay - 1);
    if Idx < 25 then Inc(Idx, 7);
    Add('Memorial Day', EncodeDate(Year, 5, Idx));

    Idx := 15 - (FirstDay - 1);
    if Idx > 14 then Dec(Idx, 7);
    Add('Mother''s Day', EncodeDate(Year, 5, Idx));

    { Jun movable holidays }
    FirstDay := DayOfWeek(EncodeDate(Year, 6, 1));
    Idx := 22 - (FirstDay - 1);
    if Idx > 21 then Dec(Idx, 7);
    Add('Father''s Day', EncodeDate(Year, 6, Idx));

    { Sep movable holidays }
    FirstDay := DayOfWeek(EncodeDate(Year, 9, 1));
    Idx := 9 - (FirstDay - 1);
    if Idx > 7 then Dec(Idx, 7);
    Add('Labor Day', EncodeDate(Year, 9, Idx));

    { Oct movable holidays }
    FirstDay := DayOfWeek(EncodeDate(Year, 10, 1));
    Idx := 16 - (FirstDay - 1);
    if Idx > 12 then Dec(Idx, 7);
    Add('Columbus Day', EncodeDate(Year, 10, Idx));

    { Nov movable holidays }
    FirstDay := DayOfWeek(EncodeDate(Year, 11, 1));
    Idx := 26 - (FirstDay - 1);
    if Idx < 22 then Inc(Idx, 7);
    Add('Thanksgiving', EncodeDate(Year, 11, Idx));

    aDate:=TxHoliday(Sender).Easter(Year);
    
  end;

*/