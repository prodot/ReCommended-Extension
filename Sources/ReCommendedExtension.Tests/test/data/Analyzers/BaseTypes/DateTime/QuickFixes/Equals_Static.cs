using System;

namespace Test
{
    public class DateTimes
    {
        public void Equals(DateTime dateTime, DateTime value)
        {
            var result = !DateTime.Equals{caret}(dateTime, value);
        }
    }
}