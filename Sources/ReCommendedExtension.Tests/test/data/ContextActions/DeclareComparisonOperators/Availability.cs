using System;
using System.Numerics;

namespace Test
{
    // classes

    internal class Clas{off}s1 { }

    internal class Class{on}2 : IComparable<Class2>
    {
        public int CompareTo(Class2? other) => throw new NotImplementedException();
    }

    internal class Class2{on}WithOperators : IComparable<Class2WithOperators>
    {
        public int CompareTo(Class2WithOperators? other) => throw new NotImplementedException();

        public static bool operator <(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator <=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator >(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator >=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
    }

    internal class Class2WithSome{on}Operators : IComparable<Class2WithSomeOperators>
    {
        public int CompareTo(Class2WithSomeOperators? other) => throw new NotImplementedException();

        public static bool operator <(Class2WithSomeOperators? left, Class2WithSomeOperators? right) => throw new NotImplementedException();
        public static bool operator >(Class2WithSomeOperators? left, Class2WithSomeOperators? right) => throw new NotImplementedException();
    }

    internal class Class2{off}WithOperatorsAndInterface : 
        IEquatable<Class2WithOperatorsAndInterface>,
        IComparable<Class2WithOperatorsAndInterface>, 
        IEqualityOperators<Class2WithOperatorsAndInterface, Class2WithOperatorsAndInterface, bool>,
        IComparisonOperators<Class2WithOperatorsAndInterface, Class2WithOperatorsAndInterface, bool>
    {
        public bool Equals(Class2WithOperatorsAndInterface? other) => throw new NotImplementedException();
        public int CompareTo(Class2WithOperatorsAndInterface? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator !=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator <(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator >(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator <=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator >=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
    }

    internal class Class{off}3 : IComparable<Class2>
    {
        public int CompareTo(Class2? other) => throw new NotImplementedException();
    }

    internal class Class{off}6 : Class5
    {
        class Neste{on}d : IComparable<Nested>
        {
            public int CompareTo(Nested? other) => throw new NotImplementedException();
        }
    }

    // structs

    internal struct Stru{off}ct1 { }

    internal struct Struct{on}2 : IComparable<Struct2>
    {
        public int CompareTo(Struct2 other) => throw new NotImplementedException();
    }

    internal struct Struct2With{on}Operators : IComparable<Struct2WithOperators>
    {
        public int CompareTo(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator >(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator <(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator >=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator <=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
    }

    internal struct Struct2WithSome{on}Operators : IComparable<Struct2WithSomeOperators>
    {
        public int CompareTo(Struct2WithSomeOperators other) => throw new NotImplementedException();

        public static bool operator >(Struct2WithSomeOperators left, Struct2WithSomeOperators right) => throw new NotImplementedException();
        public static bool operator <(Struct2WithSomeOperators left, Struct2WithSomeOperators right) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperatorsAnd{off}Interface : 
        IEquatable<Struct2WithOperatorsAndInterface>,
        IComparable<Struct2WithOperatorsAndInterface>, 
        IEqualityOperators<Struct2WithOperatorsAndInterface, Struct2WithOperatorsAndInterface, bool>,
        IComparisonOperators<Struct2WithOperatorsAndInterface, Struct2WithOperatorsAndInterface, bool>
    {
        public bool Equals(Struct2WithOperatorsAndInterface other) => throw new NotImplementedException();
        public int CompareTo(Struct2WithOperatorsAndInterface other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator !=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator >(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator <(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator >=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator <=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
    }

    internal class Struct{off}3 : IComparable<Struct2>
    {
        public int CompareTo(Struct2 other) => throw new NotImplementedException();
    }

    // records

    internal record Class{on}Record : IComparable<ClassRecord>
    {
        public int CompareTo(ClassRecord? other) => throw new NotImplementedException();
    }

    internal record Class{off}Record2(int X) : ClassRecord;

    internal record Class{off}Record3(int X, int Y) : ClassRecord2(X);

    internal record ClassRecordWith{on}Operators : IComparable<ClassRecordWithOperators>
    {
        public int CompareTo(ClassRecordWithOperators? other) => throw new NotImplementedException();

        public static bool operator <(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator <=(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator >(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator >=(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
    }

    internal record ClassRecordWithSome{on}Operators : IComparable<ClassRecordWithSomeOperators>
    {
        public int CompareTo(ClassRecordWithSomeOperators? other) => throw new NotImplementedException();

        public static bool operator <(ClassRecordWithSomeOperators? left, ClassRecordWithSomeOperators? right) => throw new NotImplementedException();
        public static bool operator <=(ClassRecordWithSomeOperators? left, ClassRecordWithSomeOperators? right) => throw new NotImplementedException();
    }

    internal record ClassRecordWithOperators{off}AndInterface :
        IEqualityOperators<ClassRecordWithOperatorsAndInterface, ClassRecordWithOperatorsAndInterface, bool>,
        IComparable<ClassRecordWithOperatorsAndInterface>, 
        IComparisonOperators<ClassRecordWithOperatorsAndInterface, ClassRecordWithOperatorsAndInterface, bool>
    {
        public int CompareTo(ClassRecordWithOperatorsAndInterface? other) => throw new NotImplementedException();

        public static bool operator <(ClassRecordWithOperatorsAndInterface? left, ClassRecordWithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator <=(ClassRecordWithOperatorsAndInterface? left, ClassRecordWithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator >(ClassRecordWithOperatorsAndInterface? left, ClassRecordWithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator >=(ClassRecordWithOperatorsAndInterface? left, ClassRecordWithOperatorsAndInterface? right) => throw new NotImplementedException();
    }

    internal record struct Struct{on}Record : IComparable<StructRecord>
    {
        public int CompareTo(StructRecord other) => throw new NotImplementedException();
    }

    internal record StructRecordWit{on}hOperators : IComparable<StructRecordWithOperators>
    {
        public int CompareTo(StructRecordWithOperators other) => throw new NotImplementedException();

        public static bool operator <(StructRecordWithOperators left, StructRecordWithOperators right) => throw new NotImplementedException();
        public static bool operator <=(StructRecordWithOperators left, StructRecordWithOperators right) => throw new NotImplementedException();
        public static bool operator >(StructRecordWithOperators left, StructRecordWithOperators right) => throw new NotImplementedException();
        public static bool operator >=(StructRecordWithOperators left, StructRecordWithOperators right) => throw new NotImplementedException();
    }

    internal record StructRecordWith{off}OperatorsAndInterface :
        IEqualityOperators<StructRecordWithOperatorsAndInterface, StructRecordWithOperatorsAndInterface, bool>,
        IComparable<StructRecordWithOperatorsAndInterface>,
        IComparisonOperators<StructRecordWithOperatorsAndInterface, StructRecordWithOperatorsAndInterface, bool>
    {
        public int CompareTo(StructRecordWithOperatorsAndInterface other) => throw new NotImplementedException();

        public static bool operator <(StructRecordWithOperatorsAndInterface left, StructRecordWithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator <=(StructRecordWithOperatorsAndInterface left, StructRecordWithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator >(StructRecordWithOperatorsAndInterface left, StructRecordWithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator >=(StructRecordWithOperatorsAndInterface left, StructRecordWithOperatorsAndInterface right) => throw new NotImplementedException();
    }
}