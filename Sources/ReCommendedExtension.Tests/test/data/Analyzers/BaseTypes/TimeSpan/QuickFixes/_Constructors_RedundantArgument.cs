using System;

namespace Test
{
    public class TimeSpans
    {
        public void _Constructors(int hours, int minutes, int seconds)
        {
            var result = new TimeSpan({caret}0, hours, minutes, seconds);
        }
    }
}