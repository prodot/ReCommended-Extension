using System;

namespace Test
{
    internal class Struct2With{caret}Operators : IEquatable<Struct2WithOperators>
    {
        public bool Equals(Struct2WithOperators other) => throw new NotImplementedException();

        public static bool operator ==(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();

        public static bool operator !=(Struct2WithOperators left, Struct2WithOperators right) => throw new NotImplementedException();
    }
}