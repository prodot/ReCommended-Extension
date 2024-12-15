using System;

namespace Test
{
    public class Strings
    {
        public void SingleCharacters(string text)
        {
            var result = text.Replace(oldValue: "a", newValue: "b{caret}");
        }
    }
}