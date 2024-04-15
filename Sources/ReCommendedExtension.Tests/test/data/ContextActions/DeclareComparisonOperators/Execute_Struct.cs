using System;

namespace Test
{
    internal struct Struct2With{caret}Operators : IComparable<Struct2WithOperators>
    {
        public int CompareTo(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator >(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator <(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator >=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
        public static bool operator <=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
    }
}