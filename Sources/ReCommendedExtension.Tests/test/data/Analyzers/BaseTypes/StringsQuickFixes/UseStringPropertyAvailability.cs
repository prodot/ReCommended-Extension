using System;

namespace Test
{
    public class Strings
    {
        public void LastIndexOf(string text, object someObject)
        {
            var result1 = text.LastIndexOf("");
            var result2 = text.LastIndexOf("", StringComparison.OrdinalIgnoreCase);

            var result3 = someObject.ToString().LastIndexOf("", StringComparison.OrdinalIgnoreCase);
        }
    }
}