using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(Guid guid)
        {
            var result = guid.Equals(null);
        }

        public void NoDetection(Guid guid, object obj)
        {
            var result = guid.Equals(obj);

            guid.Equals(null);
        }
    }
}