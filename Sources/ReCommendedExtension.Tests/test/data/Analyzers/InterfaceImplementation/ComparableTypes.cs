using System;
using System.Numerics;

namespace Test
{
    // classes

    internal class Class1 { }

    internal class Class2 : IComparable<Class2> // implement IComparisonOperators<Class2, Class2, bool> interface
    {
        public int CompareTo(Class2? other) => throw new NotImplementedException();
    }

    internal class Class2WithOperators : IComparable<Class2WithOperators> // declare IComparisonOperators<Class2WithOperators, Class2WithOperators, bool> interface (operators available)
    {
        public int CompareTo(Class2WithOperators? other) => throw new NotImplementedException();

        public static bool operator <(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator <=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator >(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator >=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
    }

    internal class Class2WithSomeOperators : IComparable<Class2WithSomeOperators> // declare IComparisonOperators<Class2WithSomeOperators, Class2WithSomeOperators, bool> interface (operators partially available)
    {
        public int CompareTo(Class2WithSomeOperators? other) => throw new NotImplementedException();

        public static bool operator <(Class2WithSomeOperators? left, Class2WithSomeOperators? right) => throw new NotImplementedException();
        public static bool operator >(Class2WithSomeOperators? left, Class2WithSomeOperators? right) => throw new NotImplementedException();
    }

    internal class Class2WithOperatorsAndInterface : 
        IEquatable<Class2WithOperatorsAndInterface>,
        IComparable<Class2WithOperatorsAndInterface>, 
        IComparisonOperators<Class2WithOperatorsAndInterface, Class2WithOperatorsAndInterface, bool>
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Class2WithOperatorsAndInterface? other) => throw new NotImplementedException();
        public int CompareTo(Class2WithOperatorsAndInterface? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator !=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator <(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator >(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator <=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
        public static bool operator >=(Class2WithOperatorsAndInterface? left, Class2WithOperatorsAndInterface? right) => throw new NotImplementedException();
    }

    internal class Class3 : IComparable<Class2>
    {
        public int CompareTo(Class2? other) => throw new NotImplementedException();
    }

    internal class Class6
    {
        class Nested : IComparable<Nested>
        {
            public int CompareTo(Nested? other) => throw new NotImplementedException();
        }
    }

    // structs

    internal struct Struct1 { }

    internal struct Struct2 : IComparable<Struct2> // implement IComparisonOperators<Struct2, Struct2, bool> interface
    {
        public int CompareTo(Struct2 other) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperators : IComparable<Struct2WithOperators> // declare IComparisonOperators<Struct2WithOperators, Struct2WithOperators, bool> interface (operators available)
    {
        public int CompareTo(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator >(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator <(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator >=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator <=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
    }

    internal struct Struct2WithSomeOperators : IComparable<Struct2WithSomeOperators> // declare IComparisonOperators<Struct2WithSomeOperators, Struct2WithSomeOperators, bool> interface (operators partially available)
    {
        public int CompareTo(Struct2WithSomeOperators other) => throw new NotImplementedException();

        public static bool operator >(Struct2WithSomeOperators left, Struct2WithSomeOperators right) => throw new NotImplementedException();
        public static bool operator <(Struct2WithSomeOperators left, Struct2WithSomeOperators right) => throw new NotImplementedException();
    }

    internal struct Struct2WithOperatorsAndInterface : 
        IEquatable<Struct2WithOperatorsAndInterface>,
        IComparable<Struct2WithOperatorsAndInterface>, 
        IComparisonOperators<Struct2WithOperatorsAndInterface, Struct2WithOperatorsAndInterface, bool>
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Struct2WithOperatorsAndInterface other) => throw new NotImplementedException();
        public int CompareTo(Struct2WithOperatorsAndInterface other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator !=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator >(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator <(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator >=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
        public static bool operator <=(Struct2WithOperatorsAndInterface left, Struct2WithOperatorsAndInterface right) => throw new NotImplementedException();
    }

    internal class Struct3 : IComparable<Struct2>
    {
        public int CompareTo(Struct2 other) => throw new NotImplementedException();
    }

    // records

    internal record ClassRecord : IComparable<ClassRecord> // implement IComparisonOperators<ClassRecord, ClassRecord, bool> interface
    {
        public int CompareTo(ClassRecord? other) => throw new NotImplementedException();
    }

    internal record ClassRecordWithOperators : IComparable<ClassRecordWithOperators> // declare IComparisonOperators<ClassRecordWithOperators, ClassRecordWithOperators, bool> interface (operators available)
    {
        public int CompareTo(ClassRecordWithOperators? other) => throw new NotImplementedException();

        public static bool operator <(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator <=(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator >(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator >=(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
    }

    internal record ClassRecordWithSomeOperators : IComparable<ClassRecordWithSomeOperators> // declare IComparisonOperators<ClassRecordWithSomeOperators, ClassRecordWithSomeOperators, bool> interface (operators partially available)
    {
        public int CompareTo(ClassRecordWithSomeOperators? other) => throw new NotImplementedException();

        public static bool operator <(ClassRecordWithSomeOperators? left, ClassRecordWithSomeOperators? right) => throw new NotImplementedException();
        public static bool operator <=(ClassRecordWithSomeOperators? left, ClassRecordWithSomeOperators? right) => throw new NotImplementedException();
    }

    internal record ClassRecordWithOperatorsAndInterface :
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

    internal record struct StructRecord : IComparable<StructRecord> // implement IComparisonOperators<StructRecord, StructRecord, bool> interface
    {
        public int CompareTo(StructRecord other) => throw new NotImplementedException();
    }

    internal record StructRecordWithOperators : IComparable<StructRecordWithOperators> // declare IComparisonOperators<StructRecordWithOperators, StructRecordWithOperators, bool> interface (operators available)
    {
        public int CompareTo(StructRecordWithOperators other) => throw new NotImplementedException();

        public static bool operator <(StructRecordWithOperators left, StructRecordWithOperators right) => throw new NotImplementedException();
        public static bool operator <=(StructRecordWithOperators left, StructRecordWithOperators right) => throw new NotImplementedException();
        public static bool operator >(StructRecordWithOperators left, StructRecordWithOperators right) => throw new NotImplementedException();
        public static bool operator >=(StructRecordWithOperators left, StructRecordWithOperators right) => throw new NotImplementedException();
    }

    internal record StructRecordWithOperatorsAndInterface :
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