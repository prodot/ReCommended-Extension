using System.Collections.Generic;

namespace Test
{
    public struct Struct
    {
        public Struct()
        {
            A = 2;
            B = 3;
        }

        public int A { get; }

        public int B { get; }
    }

    public class ComplexClass
    {
        public IList<Struct> Structs { get; init; }
    }

    class Foo
    {
        void Method()
        {
            var x = new ComplexClass
            {
                Structs = new Struct[] { new(), new() },
            };
        }
    }
}