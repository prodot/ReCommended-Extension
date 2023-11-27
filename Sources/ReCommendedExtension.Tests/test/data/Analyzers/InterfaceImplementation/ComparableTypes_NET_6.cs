using System;
using System.Numerics;

namespace Test
{
    // classes

    internal class Class1 { }

    internal class Class2 : IComparable<Class2>
    {
        public int CompareTo(Class2? other) => throw new NotImplementedException();
    }

    // structs

    internal struct Struct1 { }

    internal struct Struct2 : IComparable<Struct2>
    {
        public int CompareTo(Struct2 other) => throw new NotImplementedException();
    }

    // records

    internal record ClassRecord : IComparable<ClassRecord>
    {
        public int CompareTo(ClassRecord? other) => throw new NotImplementedException();
    }

    internal record struct StructRecord : IComparable<StructRecord>
    {
        public int CompareTo(StructRecord other) => throw new NotImplementedException();
    }
}