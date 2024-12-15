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

        public void IndexOf(string text)
        {
            var result1 = text.IndexOf("a");
            var result2 = text.IndexOf(value: "a");

            var result3 = text.IndexOf("a", StringComparison.OrdinalIgnoreCase);
            var result4 = text.IndexOf(value: "a", StringComparison.OrdinalIgnoreCase);
        }

        public void LastIndexOf(string text)
        {
            var result = text.IndexOf("a", StringComparison.Ordinal);
        }

        public void Split(string text, int count, StringSplitOptions options)
        {
            var result11 = text.Split("a");
            var result12 = text.Split("a", options);

            var result21 = text.Split("a", count);
            var result22 = text.Split("a", count, options);
        }

        public void Join(object?[] objectItems, int[] intItems, string?[] stringItems, ReadOnlySpan<object?> spanOfObjects, ReadOnlySpan<string?> spanOfStrings, int startIndex, int count)
        {
            var result1 = string.Join(",", objectItems);
            var result2 = string.Join(",", intItems);
            var result3 = string.Join(",", stringItems);
            var result4 = string.Join(",", spanOfObjects);
            var result5 = string.Join(",", spanOfStrings);
            var result6 = string.Join(",", stringItems, startIndex, count);
        }
    }
}