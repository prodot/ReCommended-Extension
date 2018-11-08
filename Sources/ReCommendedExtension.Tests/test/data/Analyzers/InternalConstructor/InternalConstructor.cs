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

        public abstract class NestedPublicClass
        {
            public NestedPublicClass() { }

            protected NestedPublicClass(int x) { }

            protected internal NestedPublicClass(bool x) { }

            internal NestedPublicClass(string x) { }

            private protected NestedPublicClass(long x) { }

            private NestedPublicClass(byte x) { }

            NestedPublicClass(short x) { }
        }

        protected abstract class NestedProtectedClass
        {
            public NestedProtectedClass() { }

            protected NestedProtectedClass(int x) { }

            protected internal NestedProtectedClass(bool x) { }

            internal NestedProtectedClass(string x) { }

            private protected NestedProtectedClass(long x) { }

            private NestedProtectedClass(byte x) { }

            NestedProtectedClass(short x) { }
        }

        protected internal abstract class NestedProtectedInternalClass
        {
            public NestedProtectedInternalClass() { }

            protected NestedProtectedInternalClass(int x) { }

            protected internal NestedProtectedInternalClass(bool x) { }

            internal NestedProtectedInternalClass(string x) { }

            private protected NestedProtectedInternalClass(long x) { }

            private NestedProtectedInternalClass(byte x) { }

            NestedProtectedInternalClass(short x) { }
        }

        internal abstract class NestedInternalClass
        {
            public NestedInternalClass() { }

            protected NestedInternalClass(int x) { }

            protected internal NestedInternalClass(bool x) { }

            internal NestedInternalClass(string x) { }

            private protected NestedInternalClass(long x) { }

            private NestedInternalClass(byte x) { }

            NestedInternalClass(short x) { }
        }

        private protected abstract class NestedPrivateProtectedClass
        {
            public NestedPrivateProtectedClass() { }

            protected NestedPrivateProtectedClass(int x) { }

            protected internal NestedPrivateProtectedClass(bool x) { }

            internal NestedPrivateProtectedClass(string x) { }

            private protected NestedPrivateProtectedClass(long x) { }

            private NestedPrivateProtectedClass(byte x) { }

            NestedPrivateProtectedClass(short x) { }
        }

        abstract class NestedPrivateClass
        {
            public NestedPrivateClass() { }

            protected NestedPrivateClass(int x) { }

            protected internal NestedPrivateClass(bool x) { }

            internal NestedPrivateClass(string x) { }

            private protected NestedPrivateClass(long x) { }

            private NestedPrivateClass(byte x) { }

            NestedPrivateClass(short x) { }
        }
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

        public abstract class NestedPublicClass
        {
            public NestedPublicClass() { }

            protected NestedPublicClass(int x) { }

            protected internal NestedPublicClass(bool x) { }

            internal NestedPublicClass(string x) { }

            private protected NestedPublicClass(long x) { }

            private NestedPublicClass(byte x) { }

            NestedPublicClass(short x) { }
        }

        protected abstract class NestedProtectedClass
        {
            public NestedProtectedClass() { }

            protected NestedProtectedClass(int x) { }

            protected internal NestedProtectedClass(bool x) { }

            internal NestedProtectedClass(string x) { }

            private protected NestedProtectedClass(long x) { }

            private NestedProtectedClass(byte x) { }

            NestedProtectedClass(short x) { }
        }

        protected internal abstract class NestedProtectedInternalClass
        {
            public NestedProtectedInternalClass() { }

            protected NestedProtectedInternalClass(int x) { }

            protected internal NestedProtectedInternalClass(bool x) { }

            internal NestedProtectedInternalClass(string x) { }

            private protected NestedProtectedInternalClass(long x) { }

            private NestedProtectedInternalClass(byte x) { }

            NestedProtectedInternalClass(short x) { }
        }

        internal abstract class NestedInternalClass
        {
            public NestedInternalClass() { }

            protected NestedInternalClass(int x) { }

            protected internal NestedInternalClass(bool x) { }

            internal NestedInternalClass(string x) { }

            private protected NestedInternalClass(long x) { }

            private NestedInternalClass(byte x) { }

            NestedInternalClass(short x) { }
        }

        private protected abstract class NestedPrivateProtectedClass
        {
            public NestedPrivateProtectedClass() { }

            protected NestedPrivateProtectedClass(int x) { }

            protected internal NestedPrivateProtectedClass(bool x) { }

            internal NestedPrivateProtectedClass(string x) { }

            private protected NestedPrivateProtectedClass(long x) { }

            private NestedPrivateProtectedClass(byte x) { }

            NestedPrivateProtectedClass(short x) { }
        }

        abstract class NestedPrivateClass
        {
            public NestedPrivateClass() { }

            protected NestedPrivateClass(int x) { }

            protected internal NestedPrivateClass(bool x) { }

            internal NestedPrivateClass(string x) { }

            private protected NestedPrivateClass(long x) { }

            private NestedPrivateClass(byte x) { }

            NestedPrivateClass(short x) { }
        }
    }
}