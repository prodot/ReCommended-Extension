using System;

namespace Test
{
    public class DateTimes
    {
        public void Equals(DateTime dateTime)
        {
            var result = dateTime.{caret}Equals(null);
        }
    }
}