using System;

namespace Test
{
    internal interface ISomeInterface { }

    // classes

    internal class Class10 { }
    internal class Class11 : ISomeInterface { }
    internal class Class12 : Class10, ISomeInterface { }

    internal class Class20 : object { }
    internal class Class21 : Object { }

    internal class Class30 : object, ISomeInterface { }
    internal class Class31 : Object, ISomeInterface { }

    internal class Class40(int x) : object, ISomeInterface { }
    internal class Class41(int x) : Object, ISomeInterface { }

    // records

    internal record Record10 { }
    internal record Record11 : ISomeInterface { }
    internal record Record12 : Record10, ISomeInterface { }

    internal record Record20 : object { }
    internal record Record21 : Object { }

    internal record Record30 : object, ISomeInterface { }
    internal record Record31 : Object, ISomeInterface { }

    internal record Record40(int X) : object, ISomeInterface { }
    internal record Record41(int X) : Object, ISomeInterface { }

    internal record class Record50 : object { }

    // other types

    internal record struct Struct1 { }
    internal struct Struct2 { }
    internal ref struct Struct3 { }

    // nested

    internal class Parent
    {
        // classes

        internal class Class10 { }
        internal class Class11 : ISomeInterface { }
        internal class Class12 : Class10, ISomeInterface { }

        internal class Class20 : object { }
        internal class Class21 : Object { }

        internal class Class30 : object, ISomeInterface { }
        internal class Class31 : Object, ISomeInterface { }

        internal class Class40(int x) : object, ISomeInterface { }
        internal class Class41(int x) : Object, ISomeInterface { }

        // records

        internal record Record10 { }
        internal record Record11 : ISomeInterface { }
        internal record Record12 : Record10, ISomeInterface { }

        internal record Record20 : object { }
        internal record Record21 : Object { }

        internal record Record30 : object, ISomeInterface { }
        internal record Record31 : Object, ISomeInterface { }

        internal record Record40(int X) : object, ISomeInterface { }
        internal record Record41(int X) : Object, ISomeInterface { }

        internal record class Record50 : object { }
    }
}