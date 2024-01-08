using System;
using System.Numerics;

namespace Test
{
    internal partial class EqualityClass
    {
        public static bool operator ==(EqualityClass? left, EqualityClass? right) => throw new NotImplementedException();
        public static bool operator !=(EqualityClass? left, EqualityClass? right) => throw new NotImplementedException();
    }

    internal partial class EqualityClass : IEquatable<EqualityClass>
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(EqualityClass? other) => throw new NotImplementedException();        
    }

    internal partial class ComparisonClass
    {
        public static bool operator ==(ComparisonClass? left, ComparisonClass? right) => throw new NotImplementedException();
        public static bool operator !=(ComparisonClass? left, ComparisonClass? right) => throw new NotImplementedException();
        public static bool operator <(ComparisonClass? left, ComparisonClass? right) => throw new NotImplementedException();
        public static bool operator >(ComparisonClass? left, ComparisonClass? right) => throw new NotImplementedException();
        public static bool operator <=(ComparisonClass? left, ComparisonClass? right) => throw new NotImplementedException();
        public static bool operator >=(ComparisonClass? left, ComparisonClass? right) => throw new NotImplementedException();
    }

    internal partial class ComparisonClass : IEquatable<ComparisonClass>, IComparable<ComparisonClass>
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(ComparisonClass? other) => throw new NotImplementedException();
        public int CompareTo(ComparisonClass? other) => throw new NotImplementedException();
    }
}