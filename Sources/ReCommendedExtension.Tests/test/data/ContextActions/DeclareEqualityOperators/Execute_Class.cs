using System;

namespace Test
{
    internal class Class2With{caret}Operators : IEquatable<Class2WithOperators>
    {
        public bool Equals(Class2WithOperators? other) => throw new NotImplementedException();

        public static bool operator ==(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();

        public static bool operator !=(Class2WithOperators? left, Class2WithOperators? right) => throw new NotImplementedException();
    }
}