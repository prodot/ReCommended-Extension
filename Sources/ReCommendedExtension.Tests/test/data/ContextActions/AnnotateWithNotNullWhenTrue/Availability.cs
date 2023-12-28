using System;
using System.Diagnostics.CodeAnalysis;

namespace Test
{
    public class Class1 : IEquatable<Class1>
    {
        public bool Equals(Class1? oth{on}er) => throw new NotImplementedException();
    }

    public class Class2
    {
        public bool Equals(Class2? othe{on}r) => throw new NotImplementedException();
    }

    public abstract class Class3 : IEquatable<Class3>
    {
        public abstract bool Equals(Class3? othe{on}r);
    }

    public class Class4 : Class3
    {
        public override bool Equals(Class3? oth{off}er) => throw new NotImplementedException();
    }

    public struct Struct1 : IEquatable<Struct1>
    {
        public bool Equals(Struct1 oth{off}er) => throw new NotImplementedException();
    }

    public struct Struct2
    {
        public bool Equals(Struct2 ot{off}her) => throw new NotImplementedException();
    }

    public sealed record ClassRecord1
    {
        public bool Equals(ClassRecord1? oth{off}er) => throw new NotImplementedException();
    }

    public record struct StructRecord1
    {
        public bool Equals(StructRecord1 ot{off}her) => throw new NotImplementedException();
    }
}

namespace Test_Annotated
{
    public class Class1 : IEquatable<Class1>
    {
        public bool Equals([NotNullWhen(true)] Class1? ot{off}her) => throw new NotImplementedException();
    }

    public class Class2
    {
        public bool Equals([NotNullWhen(true)] Class2? ot{off}her) => throw new NotImplementedException();
    }

    public abstract class Class3 : IEquatable<Class3>
    {
        public abstract bool Equals([NotNullWhen(true)] Class3? oth{off}er);
    }

    public class Class4 : Class3
    {
        public override bool Equals([NotNullWhen(true)] Class3? ot{off}her) => throw new NotImplementedException();
    }

    public struct Struct1 : IEquatable<Struct1>
    {
        public bool Equals(Struct1 ot{off}her) => throw new NotImplementedException();
    }

    public struct Struct2
    {
        public bool Equals(Struct2 ot{off}her) => throw new NotImplementedException();
    }

    public sealed record ClassRecord1
    {
        public bool Equals([NotNullWhen(true)] ClassRecord1? oth{off}er) => throw new NotImplementedException();
    }

    public record struct StructRecord1
    {
        public bool Equals(StructRecord1 ot{off}her) => throw new NotImplementedException();
    }
}