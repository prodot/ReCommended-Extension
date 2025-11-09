using System;

namespace Test
{
    public class Properties
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