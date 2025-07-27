using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void _Constructors(int year, int month, int day, Calendar calendar)
        {
            var result11 = new DateTime(year, month, day, 0, 0, 0);
            var result12 = new DateTime(year, month, day, 0, 0, 0, calendar);

            DateTime result21 = new(year, month, day, 0, 0, 0);
            DateTime result22 = new(year, month, day, 0, 0, 0, calendar);
        }
    }
}