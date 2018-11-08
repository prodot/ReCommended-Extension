namespace ReCommendedExtension.Tests.test.data.Analyzers.InternalConstructor
{
    public class NonAbtractPublicClass()
    {
        internal NonAbtractPublicClass() { }
    }

    internal class NonAbtractInternalClass()
    {
        internal NonAbtractInternalClass() { }
    }

    public abstract class PublicClass
    {
        public PublicClass() { }

        protected PublicClass(int x) { }

        protected internal PublicClass(bool x) { }

        internal PublicClass(string x) { }

        private protected PublicClass(long x) { }

        private PublicClass(byte x) { }

        PublicClass(short x) { }
    }

    internal abstract class InternalClass
    {
        public InternalClass() { }

        protected InternalClass(int x) { }

        protected internal InternalClass(bool x) { }

        internal InternalClass(string x) { }

        private protected InternalClass(long x) { }

        private InternalClass(byte x) { }

        InternalClass(short x) { }
    }
}