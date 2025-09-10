using System;

namespace Test
{
    public class TimesOnly
    {
        public void AddMinutes(TimeOnly timeOnly, double value)
        {
            var result = timeOnly.AddMinutes(value, out {caret}_);
        }
    }
}