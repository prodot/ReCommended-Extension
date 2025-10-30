using System;

namespace Test
{
    public class Methods
    {
        public void BinaryOperator(int? nullable)
        {
            var result1 = nullable.GetValueOrDefault();
            var result2 = nullable.GetValueOrDefault(-1);
        }

        public void PropertyOfNullable(int? nullable)
        {
            var result1 = nullable.HasValue;

            var result2 = nullable.Value;
        }

        public void NoDetection(int? nullable, (int, bool)? nullableTuple)
        {
            var result11 = nameof(nullable.HasValue);
            var result12 = nameof(Nullable<int>.HasValue);

            var result21 = nameof(nullable.Value);
            var result22 = nameof(Nullable<int>.Value);
            var result23 = nullableTuple.Value;

            nullable.GetValueOrDefault();
            nullable.GetValueOrDefault(-1);
        }

        public void NoDetectionWithErrors(int? nullable)
        {
            nullable.HasValue = true;
            nullable.Value = 1;
        }
    }
}