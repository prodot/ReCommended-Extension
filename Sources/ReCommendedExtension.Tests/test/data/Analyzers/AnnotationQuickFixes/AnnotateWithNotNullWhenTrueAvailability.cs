using System;

namespace Test
{
    public class Class1 : IEquatable<Class1>
    {
        public bool Equals(Class1? other) => throw new NotImplementedException();
    }
}