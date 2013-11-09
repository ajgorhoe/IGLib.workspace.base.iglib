using System;
using System.Collections;
using System.Text;

namespace xProcs.Common.Holiday
{
    public class SwissHoliday: IHoliday
    {

        protected DateTime AddNational(int year, Hashtable result)
        {
            result.Add(new DateTime(year, 1, 1), "Neujahr");

            DateTime date = new DateTime(year, 3, 1);
            while (date.DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(+1);
            result.Add(date, "Tag der Kranken");


            DateTime easter = DateUtility.Easter(year);

            result.Add(easter.AddDays(-2), "Karfreitag");
            result.Add(easter, "Ostern");
            result.Add(easter.AddDays(+1), "Ostermontag");


            result.Add(easter.AddDays(+39), "Auffahrt");
            result.Add(easter.AddDays(+49), "Pfingsten");
            result.Add(easter.AddDays(+50), "Pfingstmontag");

            date = new DateTime(year, 5, 8);
            while (date.DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(+1);
            result.Add(date, "Muttertag");

            result.Add(new DateTime(year, 8, 1), "Nationalfeiertag");


            date = new DateTime(year, 9, 15);
            while (date.DayOfWeek != DayOfWeek.Sunday)
                date = date.AddDays(+1);
            result.Add(date, "Eidg. Bettag");

            result.Add(new DateTime(year, 12, 25), "Weihnachten");
            result.Add(new DateTime(year, 12, 26), "Stefanstag");

            return easter;
        }

        /// <summary>
        /// Returns a dictionary with the germany holidays
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public IDictionary GetHolidays(int year)
        {
            Hashtable result = new Hashtable();

            AddNational(year, result);
            
            return result;
        }

    }

    /// <summary>
    /// Appenzell Innerrhoden
    /// </summary>
    public class SwissAIHolidays : SwissHoliday, IHoliday
    {
        /// <summary>
        /// Returns a dictionary with the germany holidays
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public new IDictionary GetHolidays(int year)
        {
            Hashtable result = new Hashtable();
            DateTime easter = base.AddNational(year, result);
            result.Add(easter.AddDays(60), "Fronleichnam");
            result.Add(new DateTime(year, 8,15), "Mariä Himmelfahrt");
            return result;
        }
    }

    /// <summary>
    /// Aargau
    /// </summary>
    public class SwissAGHolidays : SwissHoliday, IHoliday
    {
        /// <summary>
        /// Returns a dictionary with the germany holidays
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public new IDictionary GetHolidays(int year)
        {
            Hashtable result = new Hashtable();
            base.AddNational(year, result);
            return result;
        }
    }

    /// <summary>
    /// Appenzell Ausserrhoden
    /// </summary>
    public class SwissARHolidays : SwissHoliday, IHoliday
    {
        /// <summary>
        /// Returns a dictionary with the germany holidays
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public new IDictionary GetHolidays(int year)
        {
            Hashtable result = new Hashtable();
            base.AddNational(year, result);
            return result;
        }
    }

    /// <summary>
    /// Bern
    /// </summary>
    public class SwissBEHolidays : SwissHoliday, IHoliday
    {
        /// <summary>
        /// Returns a dictionary with the germany holidays
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public new IDictionary GetHolidays(int year)
        {
            Hashtable result = new Hashtable();
            base.AddNational(year, result);
            result.Add(new DateTime(year, 1, 2), "Berchtoldstag");
            return result;
        }
    }

    
/*

procedure SwissBEHolidays; { Bern }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do
    Add('Berchtoldstag +', EncodeDate(Year, 1, 2));
end;

procedure SwissBLHolidays; { Baselland }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do
    Add('Tag der Arbeit +',    EncodeDate(Year, 5, 1));
end;

procedure SwissBSHolidays; { Baselstadt }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do
    Add('Tag der Arbeit +',    EncodeDate(Year, 5, 1));
end;

procedure SwissFRHolidays;  { Freiburg }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Berchtoldstag +',     EncodeDate(Year, 1, 2));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Tag der Arbeit +',    EncodeDate(Year, 5, 1));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissGEHolidays; { Genf }
var
  aDate : TDateTime;
begin
  if Nationals then SwissHolidays(Sender, Year, Items);

  with Items do begin
    { 5. September (10 Tage vor Eidg. Bettag (3. Sonntag)) }
    aDate := EncodeDate(Year, 9, 15);
    while DayOfWeek(aDate) <> 1 do aDate := aDate + 1;
    Add('Genfer Bettag +', aDate - 10);
    Add('Aufstand +',                 EncodeDate(Year, 12, 12));
    Add('Genfer Wiederherstellung +', EncodeDate(Year, 12, 31));
  end;
end;

procedure SwissGLHolidays;  { Glarus }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Berchtoldstag +', EncodeDate(Year, 1, 2));
    Add('Allerheiligen +', EncodeDate(Year, 11, 1));
  end;
end;

procedure SwissGRHolidays;  { Graubünden }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do
    Add('Berchtoldstag +', EncodeDate(Year, 1, 2));
end;

procedure SwissJUHolidays;  { Jura }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Berchtoldstag +',     EncodeDate(Year, 1, 2));
    Add('Tag der Arbeit +',    EncodeDate(Year, 5, 1));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
  end;
end;

procedure SwissLUHolidays;  { Luzern }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Berchtoldstag +', EncodeDate(Year, 1, 2));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissNEHolidays;  { Neuenburg }
var
  aDate,
  aEaster: TDateTime;
begin
  if Nationals then SwissHolidays(Sender, Year, Items);

  with Items do begin
    Add('Berchtoldstag +',              EncodeDate(Year, 1, 2));
    Add('Neuenburger Unabhängigkeit +', EncodeDate(Year, 3, 1));

    { 16. September (1 Tag nach Eidg. Bettag (3. Sonntag)) }
    aDate := EncodeDate(Year, 9, 15);
    while DayOfWeek(aDate) <> 1 do aDate := aDate + 1;
    Add('Bettagmontag +', aDate + 1);
  end;
end;

procedure SwissNWHolidays;  { Nidwalden }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Josefstag +', EncodeDate(Year, 3, 19));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissOWHolidays;  { Obwalden }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Josefstag +',         EncodeDate(Year, 3, 19));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissSGHolidays;  { St. Gallen }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do
    Add('Allerheiligen +', EncodeDate(Year, 11, 1));
end;

procedure SwissSHHolidays;  { Shaffhausen }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do
    Add('Berchtoldstag +',     EncodeDate(Year, 1, 2));
end;

procedure SwissSOHolidays;   { Solothurn }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Berchtoldstag +',     EncodeDate(Year, 1, 2));
    Add('Tag der Arbeit +',    EncodeDate(Year, 5, 1));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
  end;
end;

procedure SwissSZHolidays;  { Schwyz }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Drei Könige +',       EncodeDate(Year, 1, 6));
    Add('Josefstag +',         EncodeDate(Year, 3, 19));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissTGHolidays;  { Thurgau }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Berchtoldstag +',     EncodeDate(Year, 1, 2));
    Add('Tag der Arbeit +',    EncodeDate(Year, 5, 1));
  end;
end;

procedure SwissTIHolidays;  { Tessin }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Drei Könige +',       EncodeDate(Year, 1, 6));
    Add('Josefstag +',         EncodeDate(Year, 3, 19));
    Add('Tag der Arbeit +',    EncodeDate(Year, 5, 1));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Peter und Paul +',    EncodeDate(Year, 6, 29));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissURHolidays; { Uri }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Drei Könige +',       EncodeDate(Year, 1, 6));
    Add('Josefstag +',         EncodeDate(Year, 3, 19));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissVDHolidays;  { Waadtland }
var
  aDate : TDateTime;
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Berchtoldstag +',     EncodeDate(Year, 1, 2));
    Add('Waadtländer Unabhängigkeit +', EncodeDate(Year, 1, 24));

    { 16. September (1 Tag nach Eidg. Bettag (3. Sonntag)) }
    aDate := EncodeDate(Year, 9, 15);
    while DayOfWeek(aDate) <> 1 do aDate := aDate + 1;
    Add('Bettagmontag +', aDate + 1);
  end;
end;

procedure SwissVSHolidays; { Wallis }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Josefstag +',         EncodeDate(Year, 3, 19));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissZGHolidays; { Zug }
begin
  if Nationals then SwissHolidays(Sender, Year, Items);
  with Items do begin
    Add('Berchtoldstag +',     EncodeDate(Year, 1, 2));
    Add('Fronleichnam +',      TxHoliday(Sender).Fronleichnam(Year));
    Add('Mariä Himmelfahrt +', EncodeDate(Year, 8, 15));
    Add('Allerheiligen +',     EncodeDate(Year, 11, 1));
    Add('Mariä Empfängnis +',  EncodeDate(Year, 12, 8));
  end;
end;

procedure SwissZHHolidays; { Zürich }
var
  aDate,
  aEaster: TDateTime;
begin
  if Nationals then SwissHolidays(Sender, Year, Items);

  with Items do begin
    Add('Berchtoldstag +', EncodeDate(Year, 1, 2));

    { 15. April (3. Sonntag, wenn Karwoche: 2. / wenn Osterwoche: 4.}
    aDate := EncodeDate(Year, 4, 15);
    while DayOfWeek(aDate) <> 2 do aDate := aDate + 1;
    aEaster := TxHoliday(Sender).Easter(Year);
    if aDate + 6 = aEaster then { Karwoche }
      aDate := aDate - 7
    else if aDate - 1 = aEaster then { Osterwoche }
      aDate := aDate + 7;
    Add('Zürcher Sechseläuten +', aDate);
    Add('Tag der Arbeit +', EncodeDate(Year, 5, 1));

    { 9. September (Montag vor Eidg. Bettag (3. Sonntag)) }
    aDate := EncodeDate(Year, 9, 15);
    while DayOfWeek(aDate) <> 1 do aDate := aDate + 1;
    Add('Zürcher Knabenschiessen +', aDate - 6);
  end;
end; */

}
