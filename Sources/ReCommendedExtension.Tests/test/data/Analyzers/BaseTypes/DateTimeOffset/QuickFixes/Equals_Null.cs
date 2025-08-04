using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void Equals(DateTimeOffset dateTimeOffset)
        {
            var result = dateTimeOffset.{caret}Equals(null);
        }
    }
}