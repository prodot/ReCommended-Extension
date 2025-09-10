using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void Equals(DateTimeOffset dateTimeOffset, DateTimeOffset other)
        {
            var result = !DateTimeOffset.Equals{caret}(dateTimeOffset, value);
        }
    }
}