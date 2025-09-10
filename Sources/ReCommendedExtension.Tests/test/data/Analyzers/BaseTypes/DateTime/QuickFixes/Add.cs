using System;

namespace Test
{
    public class DateTimes
    {
        public void Add(DateTime dateTime, TimeSpan value)
        {
            var result = dateTime.Add{caret}(value);
        }
    }
}