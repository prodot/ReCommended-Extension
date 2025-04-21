﻿using System.Collections.Generic;

namespace Test
{
    public class Class
    {
        public int A { get; set; }

        public int B { get; set; }
    }

    public struct Struct
    {
        public int A { get; set; }

        public int B { get; set; }
    }

    public class ComplexClass
    {
        public IList<Class> Classes { get; set; }
        public IList<Struct> Structs { get; set; }
    }

    class Foo
    {
        void Method()
        {
            var x = new ComplexClass
            {
                Classes = new Class[] { new() { A = 1, B = 2 }, new() { A = 1, B = 2 } },
                Structs = new Struct[] { new() { A = 1, B = 2 }, new() { A = 1, B = 2 } },
            };
        }
    }
}