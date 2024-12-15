using System;

namespace Test
{
    public class Strings
    {
        public void Remove(string text, int startIndex, int count)
        {
            var result1 = text.Remove(startIndex);
            var result2 = text.Remove(0, count);
        }
    }
}