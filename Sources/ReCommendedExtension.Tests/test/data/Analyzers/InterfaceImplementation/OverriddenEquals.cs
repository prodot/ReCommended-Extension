﻿using System;

namespace Test
{
    internal struct Struct1
    {
        readonly int _value;

        public Struct1(int f)
        {
            _value = f;
        }

        public override int GetHashCode() => _value.GetHashCode();

        public override bool Equals(object other) => other is S otherS && _value == otherS._value;
    }

    internal struct Struct2
    {
        readonly int _value;

        public Struct2(int f)
        {
            _value = f;
        }
    }

    internal class Class1
    {
        readonly int _value;

        public Class1(int f)
        {
            _value = f;
        }

        public override int GetHashCode() => _value.GetHashCode();

        public override bool Equals(object other) => other is Class1 otherS && _value == otherS._value;
    }
}