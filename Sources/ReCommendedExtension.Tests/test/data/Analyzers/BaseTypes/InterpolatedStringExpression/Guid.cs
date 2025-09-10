using System;

namespace Test
{
    public class InterpolatedStringItems
    {
        public void RedundantFormatSpecifier(Guid value)
        {
            var result1 = $"{value:D}";
            var result2 = $"{value:d}";
        }

        public void RedundantFormatSpecifier(Guid? value)
        {
            var result1 = $"{value:D}";
            var result2 = $"{value:d}";
        }

        public void NoDetection(Guid value)
        {
            var result = $"{value:N}";
        }

        public void NoDetection(Guid? value)
        {
            var result = $"{value:N}";
        }
    }
}