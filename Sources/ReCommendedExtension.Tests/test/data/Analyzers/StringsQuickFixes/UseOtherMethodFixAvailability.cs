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

            var result31 = text.IndexOf(s) > -1;
            var result32 = text.IndexOf(s) != -1;
            var result33 = text.IndexOf(s) >= 0;
            var result34 = text.IndexOf(s) == -1;
            var result35 = text.IndexOf(s) < 0;
        }
    }
}