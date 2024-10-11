using System;

namespace Test
{
    public class Strings
    {
        public void IndexOf(string text, char c)
        {
            var result31 = text.IndexOf(c) > -1;
            var result32 = text.IndexOf(c) != -1;
            var result33 = text.IndexOf(c) >= 0;

            var result41 = text.IndexOf(c) == -1;
            var result42 = text.IndexOf(c) < 0;
        }
    }
}