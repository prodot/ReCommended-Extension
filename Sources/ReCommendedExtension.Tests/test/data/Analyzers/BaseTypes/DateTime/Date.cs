using System;
using static System.DateTime;

namespace Test
{
    public class DateTimes
    {
        public void DateTimeProperty()
        {
            var result1 = DateTime.Now.Date;
            var result2 = Now.Date;
        }

        public void NoDetection(DateTime dateTime)
        {
            var result11 = dateTime.Date;

            var result21 = nameof(DateTime.Now.Date);
            var result22 = nameof(Now.Date);

            dateTime.Date = dateTime;
        }
    }
}