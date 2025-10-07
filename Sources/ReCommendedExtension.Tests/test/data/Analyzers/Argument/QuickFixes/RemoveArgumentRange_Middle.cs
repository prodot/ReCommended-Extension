using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgumentRange(string s, string format, out DateOnly r)
        {
            var result = DateOnly.TryParseExact(s, format, null,{caret} DateTimeStyles.None, out r);
        }
    }
}