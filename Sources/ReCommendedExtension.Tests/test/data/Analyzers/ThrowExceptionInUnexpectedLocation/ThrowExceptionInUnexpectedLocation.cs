using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnyException = System.InvalidTimeZoneException;

namespace Test
{
    public class ThrowExceptionInUnexpectedLocation : IEquatable<ThrowExceptionInUnexpectedLocation>,
        IEqualityComparer<int>,
        IEqualityComparer,
        IDisposable, IAsyncDisposable
    {
        int Property
        {
            get
            {
                // not allowed
                throw new AnyException();
                throw new ArgumentException();
                throw new KeyNotFoundException();

                // allowed
                throw new InvalidOperationException();
                throw new ObjectDisposedException(""); // inherits InvalidOperationException
                throw new NotSupportedException();

                void LocalFunction() => throw new AnyException(); // allowed
                Action action => () => throw new AnyException(); // allowed
            }
        }

        int Property2 => throw new AnyException(); // not allowed

        int Property3
        {
            get => throw new AnyException(); // not allowed
        }

        string this[int index]
        {
            get
            {
                // not allowed
                throw new AnyException();

                // allowed
                throw new ArgumentException();
                throw new ArgumentNullException(); // inherits ArgumentException
                throw new KeyNotFoundException();
                throw new InvalidOperationException();
                throw new NotSupportedException();
            }
        }

        string this[long index] => throw new AnyException(); // not allowed

        string this[short index]
        {
            get => throw new AnyException(); // not allowed
        }

        event EventHandler Event
        {
            add
            {
                // not allowed
                throw new AnyException();

                // allowed
                throw new ArgumentException();
                throw new InvalidOperationException();
                throw new NotSupportedException();
            }
            remove
            {
                // not allowed
                throw new AnyException();

                // allowed
                throw new ArgumentException();
                throw new InvalidOperationException();
                throw new NotSupportedException();
            }
        }

        event EventHandler Event2
        {
            add => throw new AnyException(); // not allowed
            remove => throw new AnyException(); // not allowed
        }

        public override bool Equals(object obj)
        {
            // not allowed
            throw new AnyException();
        }

        public bool Equals(ThrowExceptionInUnexpectedLocation other) => throw new AnyException(); // not allowed

        public override int GetHashCode()
        {
            // not allowed
            throw new AnyException();
        }

        public bool Equals(int x, int y) => throw new AnyException(); // not allowed

        public int GetHashCode(int obj)
        {
            // not allowed
            throw new AnyException();

            // allowed
            throw new ArgumentException();
        }

        public bool Equals(object x, object y) => throw new AnyException(); // not allowed

        public int GetHashCode(object obj)
        {
            // not allowed
            throw new AnyException();

            // allowed
            throw new ArgumentException();
        }

        public override string ToString() => throw new AnyException(); // not allowed

        static ThrowExceptionInUnexpectedLocation() => throw new AnyException(); // not allowed

        ~ThrowExceptionInUnexpectedLocation() => throw new AnyException(); // not allowed

        public void Dispose() => throw new AnyException(); // not allowed

        public ValueTask DisposeAsync() => throw new AnyException(); // not allowed

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Property == 1)
                {
                    // allowed
                    throw new AnyException();
                }
            }
            else
            {
                if (Property == 2)
                {
                    // not allowed
                    throw new AnyException();
                }
            }

            // not allowed
            throw new AnyException();
        }

        public static bool operator ==(ThrowExceptionInUnexpectedLocation x, ThrowExceptionInUnexpectedLocation y)
            => throw new AnyException(); // not allowed

        public static bool operator !=(ThrowExceptionInUnexpectedLocation x, ThrowExceptionInUnexpectedLocation y)
        {
            // not allowed
            throw new AnyException();
        }

        public static implicit operator DateTime(ThrowExceptionInUnexpectedLocation x)
        {
            // not allowed
            throw new AnyException();
        }

        public static implicit operator Guid(ThrowExceptionInUnexpectedLocation x) => throw new AnyException(); // not allowed
    }
}