using System;
using System.Numerics;

namespace Test
{
    // classes

    internal class Class1 { }

    internal class Class2 : IEquatable<Class2> // implement IEqualityOperators<Class2, Class2, bool> interface
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal class Class2WithOperators : IEquatable<Class2WithOperators> // declare IEqualityOperators<Class2WithOperators, Class2WithOperators, bool> interface (operators available)
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Class2WithOperators? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator !=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
    }

    internal class Class2WithSomeOperators : IEquatable<Class2WithSomeOperators> // declare IEqualityOperators<Class2WithSomeOperators, Class2WithSomeOperators, bool> interface (operators partially available)
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Class2WithSomeOperators? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithSomeOperators? left, Class2WithSomeOperators? right) => throw new NotImplementedException();
    }

    internal class Class2WithOperatorsAndInterface : IEquatable<Class2WithOperatorsAndInterface>, IEqualityOperators<Class2WithOperatorsAndInterface, Class2WithOperatorsAndInterface, bool>
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Class2WithOperatorsAndInterface? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator !=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
    }

    internal class Class3 : IEquatable<Class2>
    {
        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal class Class6 : Class5
    {
        class Nested : IEquatable<Nested>
        {
            public override bool Equals(object? obj) => throw new NotImplementedException();
            public override int GetHashCode() => throw new NotImplementedException();

            public bool Equals(Nested? other) => throw new NotImplementedException();
        }

        record Nested2;
    }

    // structs

    internal struct Struct1 { }

    internal struct Struct2 : IEquatable<Struct2> // implement IEqualityOperators<Struct2, Struct2, bool> interface
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperators : IEquatable<Struct2WithOperators> // declare IEqualityOperators<Struct2WithOperators, Struct2WithOperators, bool> (operators available)
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator !=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
    }

    internal struct Struct2WithSomeOperators : IEquatable<Struct2WithSomeOperators> // declare IEqualityOperators<Struct2WithSomeOperators, Struct2WithSomeOperators, bool> (operators partially available)
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithSomeOperators left, Struct2WithSomeOperators right) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperatorsAndInterface : IEquatable<Struct2WithOperatorsAndInterface>, IEqualityOperators<Struct2WithOperatorsAndInterface, Struct2WithOperatorsAndInterface, bool>
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Struct2WithOperatorsAndInterface other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator !=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
    }

    internal class Struct3 : IEquatable<Struct2>
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    // records

    internal record ClassRecord1; // declare IEqualityOperators<ClassRecord1, ClassRecord1, bool> (operators available)

    internal record ClassRecord1WithOperators : IEqualityOperators<ClassRecord1WithOperators, ClassRecord1WithOperators, bool>;

    internal record ClassRecord2 : ClassRecord1; // declare IEqualityOperators<ClassRecord2, ClassRecord2, bool> (operators available)

    internal record struct StructRecord1; // declare IEqualityOperators<StructRecord1, StructRecord1, bool> (operators available)

    internal record struct StructRecord1WithOperators : IEqualityOperators<StructRecord1WithOperators, StructRecord1WithOperators, bool>;
}