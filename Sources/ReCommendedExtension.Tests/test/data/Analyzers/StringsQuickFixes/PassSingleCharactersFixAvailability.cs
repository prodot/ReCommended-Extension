using System;

namespace Test
{
    public class Strings
    {
        public void Replace(string text)
        {
            var result11 = text.Replace("a", "b", StringComparison.Ordinal);
            var result12 = text.Replace(oldValue: "a", "b", StringComparison.Ordinal);
            var result13 = text.Replace("a", newValue: "b", StringComparison.Ordinal);
            var result14 = text.Replace(oldValue: "a", newValue: "b", StringComparison.Ordinal);

            var result21 = text.Replace("a", "b");
            var result22 = text.Replace(oldValue: "a", "b");
            var result23 = text.Replace("a", newValue: "b");
            var result24 = text.Replace(oldValue: "a", newValue: "b");
        }

        public void Split(string text, int count, StringSplitOptions options)
        {
            var result31 = text.Split(["a", "a"], options);
            var result32 = text.Split(new[] { "a", "a" }, options);

            var result41 = text.Split(["a", "a"], count, options);
            var result42 = text.Split(new[] { "a", "a" }, count, options);
        }
    }
}