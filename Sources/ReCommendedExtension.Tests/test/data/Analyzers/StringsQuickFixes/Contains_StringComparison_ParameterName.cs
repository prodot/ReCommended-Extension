using System;

namespace Test
{
    public class Strings
    {
        public void SingleCharacter(string text)
        {
            var result = text.Contains(value: "a{caret}", StringComparison.OrdinalIgnoreCase);
        }
    }
}