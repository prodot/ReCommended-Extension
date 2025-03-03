using System;

namespace Test
{
    internal record ClassRecord(int X);

    internal record ClassRecordWith{caret}Operators(int X, int Y) : ClassRecord(X), IComparable<ClassRecordWithOperators>
    {
        public int CompareTo(ClassRecordWithOperators? other) => throw new NotImplementedException();

        public static bool operator <(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator <=(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator >(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
        public static bool operator >=(ClassRecordWithOperators? left, ClassRecordWithOperators? right) => throw new NotImplementedException();
    }
}