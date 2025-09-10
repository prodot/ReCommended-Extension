﻿using System;

namespace Test
{
    internal struct Struct1 : IEquatable<Struct1>
    {
        readonly int _value;

        public Struct1(int f)
        {
            _value = f;
        }

        public bool Equals(Struct1 other) => _value == other._value;
    }

    internal struct Struct2 : IEquatable<Struct2>
    {
        readonly int _value;

        public Struct2(int f)
        {
            _value = f;
        }

        public bool Equals(Struct2 other) => _value == other._value;

        public override bool Equals(object obj) => obj is Struct2 objS && Equals(objS);

        public override int GetHashCode() => _value.GetHashCode();
    }

    internal class Class1 : IEquatable<Class1>
    {
        readonly int _value;

        public Class1(int f)
        {
            _value = f;
        }

        public bool Equals(Class1? other) => _value == other?._value;
    }

    internal class Class2 : IEquatable<Class2>
    {
        readonly int _value;

        public Class2(int f)
        {
            _value = f;
        }

        public bool Equals(Class2? other) => _value == other?._value;

        public override int GetHashCode() => _value.GetHashCode();

        public override bool Equals(object other) => other is Class2 otherS && Equals(otherS);
    }
}