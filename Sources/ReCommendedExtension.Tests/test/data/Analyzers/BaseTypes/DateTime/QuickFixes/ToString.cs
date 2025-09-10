using System;

namespace Test
{
    public class DateTimes
    {
        public void ToString(DateTime dateTime)
        {
            var result = dateTime.ToString("{caret}");
        }
    }
}