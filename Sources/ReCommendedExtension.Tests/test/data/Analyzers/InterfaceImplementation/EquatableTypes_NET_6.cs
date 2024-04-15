using System;

namespace Test
{
    internal interface IEquatableExtended<T> : IEquatable<T> { }

    internal class Class1 { }

    internal class Class2 : IEquatable<Class2>
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal class Class3 : IEquatable<Class2>
    {
        public bool Equals(Class2? other) => throw new NotImplementedException();
    }

    internal interface IEquatableOfClass4 : IEquatable<Class4> { }

    internal class Class4 : IEquatableOfClass4
    {
        public bool Equals(Class4? other) => throw new NotImplementedException();
    }

    internal class Class5 : IEquatableExtended<Class5>
    {
        public bool Equals(Class5? other) => throw new NotImplementedException();
    }

    internal class Class6 : Class5 { }

    internal struct Struct1 { }

    internal struct Struct2 : IEquatable<Struct2>
    {
        public override bool Equals(object? obj) => throw new NotImplementedException();
        public override int GetHashCode() => throw new NotImplementedException();

        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    internal class Struct3 : IEquatable<Struct2>
    {
        public bool Equals(Struct2 other) => throw new NotImplementedException();
    }

    internal interface IEquatableOfStruct4 : IEquatable<Struct4> { }

    internal class Struct4 : IEquatableOfStruct4
    {
        public bool Equals(Struct4 other) => throw new NotImplementedException();
    }

    internal class Struct5 : IEquatableExtended<Struct5>
    {
        public bool Equals(Struct5 other) => throw new NotImplementedException();
    }

    internal record ClassRecord1; // implements IEquatable<ClassRecord1>

    internal record ClassRecord2 : ClassRecord1; // implements IEquatable<ClassRecord2>

    internal record struct StructRecord1; // implements IEquatable<StructRecord1>
}