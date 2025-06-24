using System;

namespace Test
{
    public class InterpolatedStringItems
    {
        public void Double(double number)
        {
            var result1 = $"{number:R}";
            var result2 = $"{number:R3}";
            var result3 = $"{number:r}";
            var result4 = $"{number:r3}";
        }

        public void Double(double? number)
        {
            var result1 = $"{number:R}";
            var result2 = $"{number:R3}";
            var result3 = $"{number:r}";
            var result4 = $"{number:r3}";
        }

        public void Single(float number)
        {
            var result1 = $"{number:R}";
            var result2 = $"{number:R3}";
            var result3 = $"{number:r}";
            var result4 = $"{number:r3}";
        }

        public void Single(float? number)
        {
            var result1 = $"{number:R}";
            var result2 = $"{number:R3}";
            var result3 = $"{number:r}";
            var result4 = $"{number:r3}";
        }
    }
}