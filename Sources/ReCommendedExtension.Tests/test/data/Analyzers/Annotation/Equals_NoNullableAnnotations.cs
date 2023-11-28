using System;
using System.Diagnostics.CodeAnalysis;

namespace Test
{
    public class Class1 : IEquatable<Class1>
    {
        public bool Equals(Class1 other) => throw new NotImplementedException();
    }

    public class Class2
    {
        public bool Equals(Class2 other) => throw new NotImplementedException();
    }

    public abstract class Class3 : IEquatable<Class3>
    {
        public abstract bool Equals(Class3 other);
    }

    public class Class4 : Class3
    {
        public override bool Equals(Class3 other) => throw new NotImplementedException();
    }

    public struct Struct1 : IEquatable<Struct1>
    {
        public bool Equals(Struct1 other) => throw new NotImplementedException();
    }

    public struct Struct2
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    public sealed record ClassRecord1
    {
        public bool Equals(ClassRecord1 other) => throw new NotImplementedException();
    }

    public record struct StructRecord1
    {
        public bool Equals(StructRecord1 other) => throw new NotImplementedException();
    }
}