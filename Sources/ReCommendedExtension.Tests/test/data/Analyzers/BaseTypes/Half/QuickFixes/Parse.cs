using System;
using System.Globalization;

namespace Test
{
    public class Halves
    {
        public void Parse(string s, IFormatProvider provider)
        {
            var result = Half.Parse(s, NumberStyles{caret}.Float | NumberStyles.AllowThousands, provider);
        }
    }
}