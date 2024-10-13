using System;

namespace Test
{
    public class Strings
    {
        public void Comparison(string text)
        {
            var result = text.IndexOf("a{caret}", StringComparison.CurrentCulture);
        }
    }
}