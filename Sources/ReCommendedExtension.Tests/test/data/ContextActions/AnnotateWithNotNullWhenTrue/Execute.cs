using System;

namespace Test
{
    public class Class1 : IEquatable<Class1>
    {
        public bool Equals(Class1? ot{caret}her) => throw new NotImplementedException();
    }
}