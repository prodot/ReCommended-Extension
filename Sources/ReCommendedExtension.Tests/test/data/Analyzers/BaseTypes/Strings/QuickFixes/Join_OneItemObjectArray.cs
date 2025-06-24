using System;

namespace Test
{
    public class Strings
    {
        public void Join(string s, object objectItem)
        {
            var result = string.Join{caret}(s, (object?[])[objectItem]);
        }
    }
}