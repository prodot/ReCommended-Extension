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

        public void IndexOf(string text, object someObject)
        {
            var result1 = text.IndexOf("");
            var result2 = text.IndexOf("", StringComparison.OrdinalIgnoreCase);

            var result3 = someObject.ToString().IndexOf("", StringComparison.OrdinalIgnoreCase);
        }

        public void LastIndexOf(string text, char c)
        {
            var result = text.LastIndexOf(c, 0);
        }

        public void Remove(string text)
        {
            var result = text.Remove(0);
        }
    }
}