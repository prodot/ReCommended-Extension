using System;
using System.Numerics;

namespace Test
{
    // classes

    internal class Class2 : IEquatable<Class2>
    {
        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal class Class2WithOperators : IEquatable<Class2WithOperators>
    {
        public bool Equals(Class2WithOperators? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();

        public static bool operator !=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
    }

    // structs

    internal struct Struct2 : IEquatable<Struct2>
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperators : IEquatable<Struct2WithOperators>
    {
        public bool Equals(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();

        public static bool operator !=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
    }

    // records

    internal record ClassRecord1;

    internal record ClassRecord2 : ClassRecord1;

    internal record struct StructRecord1;
}