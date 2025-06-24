using System;

namespace Test
{
    public class UInt64s
    {
        public void GetTypeCode(ulong number)
        {
            var result = number.Get{caret}TypeCode();
        }
    }
}