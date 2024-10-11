using System;

namespace Test
{
    public class Strings
    {
        public void Contains(string text)
        {
            var result1 = text.Contains("a");
            var result2 = text.Contains(value: "a");

            var result3 = text.Contains("a", StringComparison.OrdinalIgnoreCase);
            var result4 = text.Contains(value: "a", StringComparison.OrdinalIgnoreCase);
        }
    }
}