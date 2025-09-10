using System;
using System.Globalization;

namespace Test
{
    public class Doubles
    {
        public void Parse(string s, IFormatProvider provider)
        {
            var result = double.Parse(s, NumberStyles{caret}.Float | NumberStyles.AllowThousands, provider);
        }
    }
}