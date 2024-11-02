using System;

namespace Test
{
    public class Strings
    {
        public void Split(string text, int count, StringSplitOptions options)
        {
            var result12 = text.Split(['a', 'b', 'a']);
            var result13 = text.Split(new[] { 'a', 'b', 'a' });

            var result21 = text.Split(['a', 'b', 'a'], count);
            var result22 = text.Split(new[] { 'a', 'b', 'a' }, count);

            var result31 = text.Split(['a', 'b', 'a'], options);
            var result32 = text.Split(new[] { 'a', 'b', 'a' }, options);

            var result41 = text.Split(['a', 'b', 'a'], count, options);
            var result42 = text.Split(new[] { 'a', 'b', 'a' }, count, options);

            var result51 = text.Split(["aa", "bb", "aa"], options);
            var result52 = text.Split(new[] { "aa", "bb", "aa" }, options);

            var result61 = text.Split(["aa", "bb", "aa"], count, options);
            var result62 = text.Split(new[] { "aa", "bb", "aa" }, count, options);
        }
    }
}