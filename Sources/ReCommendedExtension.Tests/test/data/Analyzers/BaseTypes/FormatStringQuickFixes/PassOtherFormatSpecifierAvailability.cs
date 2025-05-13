using System;

namespace Test
{
    public class FormatStrings
    {
        public void Double(double number)
        {
            var result1 = string.Format("{0:R}", number);
            var result2 = string.Format("{0:R3}", number);
            var result3 = string.Format("{0:r}", number);
            var result4 = string.Format("{0:r3}", number);
        }

        public void Double(double? number)
        {
            var result1 = string.Format("{0:R}", number);
            var result2 = string.Format("{0:R3}", number);
            var result3 = string.Format("{0:r}", number);
            var result4 = string.Format("{0:r3}", number);
        }

        public void Single(float number)
        {
            var result1 = string.Format("{0:R}", number);
            var result2 = string.Format("{0:R3}", number);
            var result3 = string.Format("{0:r}", number);
            var result4 = string.Format("{0:r3}", number);
        }

        public void Single(float? number)
        {
            var result1 = string.Format("{0:R}", number);
            var result2 = string.Format("{0:R3}", number);
            var result3 = string.Format("{0:r}", number);
            var result4 = string.Format("{0:r3}", number);
        }
    }
}