using System;

namespace Test
{
    public class Strings
    {
        public void Contains(string text, object someObject)
        {
            var result1 = text.Contains("");
            var result2 = text.Contains("", StringComparison.OrdinalIgnoreCase);

            var result3 = someObject.ToString().Contains(value: "", StringComparison.OrdinalIgnoreCase);
        }

        public void EndsWith(string text, object someObject)
        {
            var result1 = text.EndsWith("");
            var result2 = text.EndsWith("", StringComparison.OrdinalIgnoreCase);

            var result3 = someObject.ToString().EndsWith(value: "", StringComparison.OrdinalIgnoreCase);
        }
    }
}