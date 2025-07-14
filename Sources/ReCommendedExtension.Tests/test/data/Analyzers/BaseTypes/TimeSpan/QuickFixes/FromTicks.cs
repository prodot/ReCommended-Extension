using System;

namespace Test
{
    public class TimeSpans
    {
        public void FromTicks()
        {
            var result = -TimeSpan.From{caret}Ticks(long.MaxValue);
        }
    }
}