using System;

namespace Test
{
    public class Strings
    {
        public void Comparison(string text, char c)
        {
            var result32 = text.Index{caret}Of(c, StringComparison.CurrentCulture) != -1;
        }
    }
}