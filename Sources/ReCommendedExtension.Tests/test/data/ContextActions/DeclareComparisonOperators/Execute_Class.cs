using System;

namespace Test
{
    internal class Class2{caret}WithOperators : IComparable<Class2WithOperators>
    {
        public int CompareTo(Class2WithOperators? other) => throw new NotImplementedException();

        public static bool operator <(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator <=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator >(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
        public static bool operator >=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
    }
}