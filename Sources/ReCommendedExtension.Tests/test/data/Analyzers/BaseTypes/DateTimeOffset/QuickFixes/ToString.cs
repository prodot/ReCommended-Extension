using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void ToString(DateTimeOffset dateTimeOffset)
        {
            var result = dateTimeOffset.ToString("{caret}");
        }
    }
}