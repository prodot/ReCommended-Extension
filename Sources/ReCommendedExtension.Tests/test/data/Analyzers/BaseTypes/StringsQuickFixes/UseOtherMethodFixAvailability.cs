using System;

namespace Test
{
    public class Strings
    {
        public void IndexOf(string text, char c, string s)
        {
            var result11 = text.IndexOf(c) > -1;
            var result12 = text.IndexOf(c) != -1;
            var result13 = text.IndexOf(c) >= 0;
            var result14 = text.IndexOf(c) == -1;
            var result15 = text.IndexOf(c) < 0;

            var result21 = text.IndexOf(c, StringComparison.CurrentCulture) > -1;
            var result22 = text.IndexOf(c, StringComparison.CurrentCulture) != -1;
            var result23 = text.IndexOf(c, StringComparison.CurrentCulture) >= 0;
            var result24 = text.IndexOf(c, StringComparison.OrdinalIgnoreCase) == -1;
            var result25 = text.IndexOf(c, StringComparison.OrdinalIgnoreCase) < 0;

            var result31 = text.IndexOf(s) == 0;
            var result32 = text.IndexOf(s) != 0;

            var result41 = text.IndexOf(s) > -1;
            var result42 = text.IndexOf(s) != -1;
            var result43 = text.IndexOf(s) >= 0;
            var result44 = text.IndexOf(s) == -1;
            var result45 = text.IndexOf(s) < 0;

            var result51 = text.IndexOf(s, StringComparison.CurrentCulture) == 0;
            var result52 = text.IndexOf(s, StringComparison.CurrentCulture) != 0;

            var result61 = text.IndexOf(s, StringComparison.CurrentCulture) > -1;
            var result62 = text.IndexOf(s, StringComparison.CurrentCulture) != -1;
            var result63 = text.IndexOf(s, StringComparison.CurrentCulture) >= 0;
            var result64 = text.IndexOf(s, StringComparison.OrdinalIgnoreCase) == -1;
            var result65 = text.IndexOf(s, StringComparison.OrdinalIgnoreCase) < 0;
        }

        public void IndexOfAny(string text, char c, int startIndex, int count)
        {
            var result1 = text.IndexOfAny([c]);
            var result2 = text.IndexOfAny([c], startIndex);
            var result3 = text.IndexOfAny([c], startIndex, count);
        }

        public void LastIndexOfAny(string text, char c, int startIndex, int count)
        {
            var result1 = text.LastIndexOfAny([c]);
            var result2 = text.LastIndexOfAny([c], startIndex);
            var result3 = text.LastIndexOfAny([c], startIndex, count);
        }
    }
}