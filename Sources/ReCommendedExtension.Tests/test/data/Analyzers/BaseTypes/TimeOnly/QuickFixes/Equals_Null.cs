using System;

namespace Test
{
    public class TimesOnly
    {
        public void Equals(TimeOnly timeOnly)
        {
            var result = timeOnly.{caret}Equals(null);
        }
    }
}