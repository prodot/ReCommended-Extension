using System;

namespace Test
{
    public class DateTimeOffsets
    {
        public void TryParse(string s, out DateTimeOffset result)
        {
            var result_ = DateTimeOffset.TryParse(s, null{caret}, out result);
        }
    }
}