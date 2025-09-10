using System;

namespace Test
{
    public class Guids
    {
        public void ExpressionResult(Guid guid)
        {
            var result = guid.Equals(null);
        }

        public void Operator(Guid guid, Guid g)
        {
            var result = guid.Equals(g);
        }

        public void NoDetection(Guid guid, Guid g, Guid? otherGuid)
        {
            var result = guid.Equals(otherGuid);

            guid.Equals(null);

            guid.Equals(g);
        }
    }
}