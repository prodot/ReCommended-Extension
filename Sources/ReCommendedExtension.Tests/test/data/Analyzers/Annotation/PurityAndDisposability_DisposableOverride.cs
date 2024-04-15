using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace NonDisposable
{
    internal class Parent
    {
        public virtual object Method() => throw new NotImplementedException();
    }

    internal class ChildAnnotated : Parent
    {
        [MustDisposeResource]
        public override IDisposable Method() => throw new NotImplementedException();
    }

    internal class ChildAnnotatedWithFalse : Parent
    {
        [MustDisposeResource(false)]
        public override IDisposable Method() => throw new NotImplementedException();
    }

    internal class ChildNotAnnotated : Parent
    {
        public override IDisposable Method() => throw new NotImplementedException();
    }
}