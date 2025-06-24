using System;

namespace Test
{
    public class Nullables
    {
        public void Value(int? nullable)
        {
            var result = nullable.Value;
        }

        public void Value<T>(T? nullable) where T : struct
        {
            var result = nullable.Value;
        }
    }
}