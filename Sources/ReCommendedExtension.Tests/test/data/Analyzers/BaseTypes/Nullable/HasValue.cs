using System;

namespace Test
{
    public class Nullables
    {
        public void HasValueAlternative(int? nullable)
        {
            var result = nullable.HasValue;
        }

        public void HasValueAlternative<T>(T? nullable) where T : struct
        {
            var result = nullable.HasValue;
        }

        public void NoDetection(int? nullable)
        {
            nullable.HasValue = true;

            (nullable.HasValue, var x) = (true, 1);

            var result1 = nameof(nullable.HasValue);
            var result2 = nameof(Nullable<int>.HasValue);
        }

        public void NoDetection<T>(T? nullable) where T : struct
        {
            nullable.HasValue = true;

            (nullable.HasValue, var x) = (true, 1);

            var result1 = nameof(nullable.HasValue);
            var result2 = nameof(Nullable<T>.HasValue);
        }
    }
}