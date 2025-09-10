using System.Collections.Generic;

namespace Test
{
    public class C(int i)
    {
        void Detection(bool f, C[] array)
        {
            var v1 = f ? new C[] { new(1), new(2) } : array;

            var v2 = array ?? new C[] { new(1), new(2) };

            var v3 = f switch
            {
                true => new C[] { new(1), new(2) },
                _ => array,
            };
        }

        void NoDetection(bool f)
        {
            var v1 = f ? new C[] { new(1), new(2) } : [new(1), new(2)];

            var v3 = f switch
            {
                true => new C[] { new(1), new(2) },
                _ => [new(1), new(2)],
            };

            var v4 = new C[] { new(1), new(2) };
        }
    }
}