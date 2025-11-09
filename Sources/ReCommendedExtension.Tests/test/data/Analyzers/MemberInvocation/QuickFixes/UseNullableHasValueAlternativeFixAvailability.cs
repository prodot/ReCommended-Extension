using System;

namespace Test
{
    public class Methods
    {
        public void PropertyOfNullable(int? nullable, ValueTuple<int>? nullableTupleLength1, (int, bool)? nullableTuple)
        {
            var result1 = nullable.HasValue;
            var result2 = nullableTupleLength1.HasValue;
            var result3 = nullableTuple.HasValue;
        }
    }
}