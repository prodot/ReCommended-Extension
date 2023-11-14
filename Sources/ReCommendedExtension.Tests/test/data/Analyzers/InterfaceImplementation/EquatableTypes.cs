using System;
using System.Numerics;

namespace Test
{
    // classes

    internal class Class1 { }

    internal class Class2 : IEquatable<Class2> // declare implementation of the IEqualityOperators<Class2, Class2, bool>
    {
        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal class Class2WithOperators : IEquatable<Class2WithOperators> // declare implementation of the IEqualityOperators<Class2WithOperators, Class2WithOperators, bool>
    {
        public bool Equals(Class2WithOperators? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();

        public static bool operator !=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
    }

    internal class Class2WithOperatorsAndInterface : IEquatable<Class2WithOperatorsAndInterface>, IEqualityOperators<Class2WithOperatorsAndInterface, Class2WithOperatorsAndInterface, bool>
    {
        public bool Equals(Class2WithOperatorsAndInterface? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();

        public static bool operator !=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
    }

    internal class Class3 : IEquatable<Class2>
    {
        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal class Class6 : Class5 { }

    // structs

    internal struct Struct1 { }

    internal struct Struct2 : IEquatable<Struct2> // declare implementation of the IEqualityOperators<Struct2, Struct2, bool>
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperators : IEquatable<Struct2WithOperators> // declare implementation of the IEqualityOperators<Struct2WithOperators, Struct2WithOperators, bool> (operators available)
    {
        public bool Equals(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();

        public static bool operator !=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperatorsAndInterface : IEquatable<Struct2WithOperatorsAndInterface>, IEqualityOperators<Struct2WithOperatorsAndInterface, Struct2WithOperatorsAndInterface, bool>
    {
        public bool Equals(Struct2WithOperatorsAndInterface other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();

        public static bool operator !=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
    }

    internal class Struct3 : IEquatable<Struct2>
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    // records

    internal record ClassRecord1; // implements IEquatable<ClassRecord1>: declare implementation of the IEqualityOperators<ClassRecord1, ClassRecord1, bool> (operators available)

    internal record ClassRecord1WithOperators : IEqualityOperators<ClassRecord1WithOperators, ClassRecord1WithOperators, bool>; // implements IEquatable<ClassRecord1>

    internal record ClassRecord2 : ClassRecord1; // implements IEquatable<ClassRecord2>: declare implementation of the IEqualityOperators<ClassRecord2, ClassRecord2, bool> (operators available)

    internal record struct StructRecord1; // implements IEquatable<StructRecord1>: declare implementation of the IEqualityOperators<StructRecord1, StructRecord1, bool> (operators available)

    internal record struct StructRecord1WithOperators : IEqualityOperators<StructRecord1WithOperators, StructRecord1WithOperators, bool>; // implements IEquatable<StructRecord1>
}