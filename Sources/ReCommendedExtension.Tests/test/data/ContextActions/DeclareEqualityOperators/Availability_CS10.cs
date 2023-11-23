using System;
using System.Numerics;

namespace Test
{
    // classes

    internal class Clas{off}s1 { }

    internal class Cl{off}ass2 : IEquatable<Class2>
    {
        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal class Class2WithO{off}perators : IEquatable<Class2WithOperators>
    {
        public bool Equals(Class2WithOperators? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();

        public static bool operator !=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
    }

    internal class Class2WithOperatorsAnd{off}Interface : IEquatable<Class2WithOperatorsAndInterface>, IEqualityOperators<Class2WithOperatorsAndInterface, Class2WithOperatorsAndInterface, bool>
    {
        public bool Equals(Class2WithOperatorsAndInterface? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();

        public static bool operator !=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
    }

    internal class Cla{off}ss3 : IEquatable<Class2>
    {
        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal class Clas{off}s6 : Class5
    {
        record Nest{off}ed;
    }

    // structs

    internal struct Struc{off}t1 { }

    internal struct Stru{off}ct2 : IEquatable<Struct2>
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    internal struct Struct2WithOpe{off}rators : IEquatable<Struct2WithOperators>
    {
        public bool Equals(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();

        public static bool operator !=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperatorsAndIn{off}terface : IEquatable<Struct2WithOperatorsAndInterface>, IEqualityOperators<Struct2WithOperatorsAndInterface, Struct2WithOperatorsAndInterface, bool>
    {
        public bool Equals(Struct2WithOperatorsAndInterface other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();

        public static bool operator !=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
    }

    internal class Struct{off}3 : IEquatable<Struct2>
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    // records

    internal record ClassReco{off}rd1;

    internal record ClassRecord1WithOpera{off}tors : IEqualityOperators<ClassRecord1WithOperators, ClassRecord1WithOperators, bool>;

    internal record ClassRec{off}ord2 : ClassRecord1;

    internal record ClassRec{off}ord3(int X) : ClassRecord1;

    internal record ClassRec{off}ord4(int X, int Y) : ClassRecord3(X);

    internal record struct StructRec{off}ord1;

    internal record struct StructRecord1WithOper{off}ators : IEqualityOperators<StructRecord1WithOperators, StructRecord1WithOperators, bool>;
}