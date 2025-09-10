using System;

namespace Test
{
    public class DateTimes
    {
        public void TryParse(string s, out DateTime result)
        {
            var result_ = DateTime.TryParse(s, null{caret}, out result);
        }
    }
}