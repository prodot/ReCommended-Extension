using System;

namespace Test
{
    public class TimesOnly
    {
        public void Add(TimeOnly timeOnly, TimeSpan value)
        {
            var result = timeOnly.Add(value, out {caret}_);
        }
    }
}