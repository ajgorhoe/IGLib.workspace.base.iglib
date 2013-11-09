using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace xProcs.Common.Holiday
{
    /// <summary>
    /// Factory for create holiday calculation classes
    /// </summary>
    public static class HolidayFactory
    {

        /// <summary>
        /// Returns an instance of a Holiday Calculator 
        /// </summary>
        /// <returns></returns>
        public static IHoliday CreateHolidayCalculator(RegionInfo region)
        {
            if (region.TwoLetterISORegionName == "DE")     
                return new GermanHoliday();

            else if (region.TwoLetterISORegionName == "US")
                return new UnitedStatesHoliday();

            else if (region.TwoLetterISORegionName == "GB")
                return new GreatBritainHoliday();

            else if (region.TwoLetterISORegionName == "CH")
                throw new NotImplementedException();

            else if (region.TwoLetterISORegionName == "RU")
                throw new NotImplementedException();

            else if (region.TwoLetterISORegionName == "CH")
                throw new NotImplementedException();

            else
                throw new NotImplementedException();
        }
    }
}
