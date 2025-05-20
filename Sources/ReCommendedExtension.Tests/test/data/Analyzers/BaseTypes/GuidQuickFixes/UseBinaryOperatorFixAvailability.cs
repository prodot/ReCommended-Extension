using System;

namespace Test
{
    public class Guids
    {
        public void Operator(Guid guid, Guid g)
        {
            var result = guid.Equals(g);
        }
    }
}