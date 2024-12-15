using System;

namespace Test
{
    public class Strings
    {
        public void Join(string?[] stringItems)
        {
            var result = string.Join(",{caret}", stringItems);
        }
    }
}