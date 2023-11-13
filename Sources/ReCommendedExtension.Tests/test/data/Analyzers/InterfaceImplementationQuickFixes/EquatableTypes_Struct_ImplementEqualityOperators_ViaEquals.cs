using System;

namespace Test
{
    internal struct Struct{caret}2 : IEquatable<Struct2>
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }
}