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
                Action action = () => throw new AnyException(); // allowed
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

    public record Record(string S, EventHandler H)
    {
        string Property4 { get; } = S ?? throw new ArgumentNullException(); // allowed

        string Property5 { get; set; } = S ?? throw new ArgumentNullException(); // allowed

        event EventHandler E = H ?? throw new ArgumentNullException(); // allowed

        static string F() => null;

        string Property6 { get; } = F() ?? throw new ArgumentNullException(); // allowed

        string Property7 { get; set; } = F() ?? throw new ArgumentNullException(); // allowed

        static string Property8 { get; } = F() ?? throw new ArgumentNullException(); // not allowed

        static string Property9 { get; set; } = F() ?? throw new ArgumentNullException(); // not allowed

        string field1 = S ?? throw new ArgumentNullException(); // allowed

        static string field2 = F() ?? throw new ArgumentNullException(); // not allowed

        static EventHandler E2() => null;

        static event EventHandler Event = E2() ?? throw new ArgumentNullException(); // not allowed
    }

    public record Record2(string S, EventHandler H)
    {
        Lazy<string> Property4 { get; } = new(() => S ?? throw new ArgumentNullException()); // not allowed

        Lazy<string> Property5 { get; set; } = new(() => S ?? throw new ArgumentNullException()); // not allowed

        event EventHandler E = new Lazy<EventHandler>(() => H ?? throw new ArgumentNullException()).Value; // not allowed

        static string F() => null;

        Lazy<string> Property6 { get; } = new(() => F() ?? throw new ArgumentNullException()); // not allowed

        Lazy<string> Property7 { get; set; } = new(() => F() ?? throw new ArgumentNullException()); // not allowed

        static Lazy<string> Property8 { get; } = new(() => F() ?? throw new ArgumentNullException()); // not allowed

        static Lazy<string> Property9 { get; set; } = new(() => F() ?? throw new ArgumentNullException()); // not allowed

        Lazy<string> field1 = new(() => S ?? throw new ArgumentNullException()); // not allowed

        static Lazy<string> field2 = new(() => F() ?? throw new ArgumentNullException()); // not allowed

        static EventHandler E2() => null;

        static event EventHandler Event = new Lazy<EventHandler>(() => E2() ?? throw new ArgumentNullException()).Value; // not allowed
    }
}