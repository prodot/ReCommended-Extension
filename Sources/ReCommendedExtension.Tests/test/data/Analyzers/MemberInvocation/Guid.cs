using System;

namespace Test
{
    public class Methods
    {
        public void BinaryOperator(Guid guid, Guid value)
        {
            var result = guid.Equals(value);
        }

        public void NoDetection(Guid guid, Guid value)
        {
            guid.Equals(value);
        }
    }
}