using System;

namespace Test
{
    public class FormatStrings
    {
        public void RedundantFormatSpecifier(Guid value)
        {
            var result1 = string.Format("{0:D}", value);
            var result2 = string.Format("{0:d}", value);
        }

        public void RedundantFormatSpecifier(Guid? value)
        {
            var result1 = string.Format("{0:D}", value);
            var result2 = string.Format("{0:d}", value);
        }

        public void NoDetection(Guid value)
        {
            var result = string.Format("{0:N}", value);
        }

        public void NoDetection(Guid? value)
        {
            var result = string.Format("{0:N}", value);
        }
    }
}