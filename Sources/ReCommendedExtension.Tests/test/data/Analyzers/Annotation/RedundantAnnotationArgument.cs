using System;
using JetBrains.Annotations;

namespace Test
{
    [MustDisposeResource(true)]
    internal class Class : IDisposable
    {
        public void Dispose() { }
    }

    internal class ClassWithConstructor : IDisposable
    {
        [MustDisposeResource(true)]
        public ClassWithConstructor() { }

        public void Dispose() { }
    }

    [method: MustDisposeResource(true)]
    internal class ClassWithPrimaryConstructor() : IDisposable
    {
        public void Dispose() { }
    }

    [MustDisposeResource(true)]
    internal struct Struct : IDisposable
    {
        public void Dispose() { }
    }

    internal struct StructWithConstructor : IDisposable
    {
        [MustDisposeResource(true)]
        public StructWithConstructor() { }

        public void Dispose() { }
    }

    [method: MustDisposeResource(true)]
    internal struct StructWithPrimaryConstructor() : IDisposable
    {
        public void Dispose() { }
    }

    internal class Methods
    {
        [MustDisposeResource(true)]
        public IDisposable Method() => throw new NotImplementedException();

        public void LocalFunctions()
        {
            [MustDisposeResource(true)]
            IDisposable LocalFunction() => throw new NotImplementedException();
        }

        public void Parameters([MustDisposeResource(true)] out IDisposable parameter) => throw new NotImplementedException();
    }

    [MustDisposeResource(false)]
    internal class ClassWithFalse : IDisposable
    {
        public void Dispose() { }
    }
}