using System;

namespace Test
{
    public class Nullables
    {
        public void HasValue(int? nullable, ValueTuple<int>? tuple1, (int, bool)? tuple2, (int, bool, string)? tuple3, (int, int, int, int, int, int, int, int, int, int)? tuple10)
        {
            var result11 = nullable.HasValue;

            var result21 = tuple1.HasValue;
            var result22 = tuple2.HasValue;
            var result23 = tuple3.HasValue;

            var result31 = tuple10.HasValue;
        }

        public void HasValue<T>(T? nullable) where T : struct
        {
            var result = nullable.HasValue;
        }
    }
}