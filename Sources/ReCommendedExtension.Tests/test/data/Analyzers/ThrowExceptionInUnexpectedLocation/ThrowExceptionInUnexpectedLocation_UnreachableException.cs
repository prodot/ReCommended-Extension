using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
                throw new UnreachableException();
            }
        }

        int Property2 => throw new UnreachableException();

        int Property3
        {
            get => throw new UnreachableException();
        }

        string this[int index]
        {
            get
            {
                throw new UnreachableException();
            }
        }

        string this[long index] => throw new UnreachableException();

        string this[short index]
        {
            get => throw new UnreachableException();
        }

        event EventHandler Event
        {
            add
            {
                throw new UnreachableException();
            }
            remove
            {
                throw new UnreachableException();
            }
        }

        event EventHandler Event2
        {
            add => throw new UnreachableException();
            remove => throw new UnreachableException();
        }

        public override bool Equals(object obj)
        {
            throw new UnreachableException();
        }

        public bool Equals(ThrowExceptionInUnexpectedLocation other) => throw new UnreachableException();

        public override int GetHashCode()
        {
            throw new UnreachableException();
        }

        public bool Equals(int x, int y) => throw new UnreachableException();

        public int GetHashCode(int obj)
        {
            throw new UnreachableException();
        }

        public bool Equals(object x, object y) => throw new UnreachableException();

        public int GetHashCode(object obj)
        {
            throw new UnreachableException();
        }

        public override string ToString() => throw new UnreachableException();

        static ThrowExceptionInUnexpectedLocation() => throw new UnreachableException();

        ~ThrowExceptionInUnexpectedLocation() => throw new UnreachableException();

        public void Dispose() => throw new UnreachableException();

        public ValueTask DisposeAsync() => throw new UnreachableException();

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Property == 1)
                {
                    throw new UnreachableException();
                }
            }
            else
            {
                if (Property == 2)
                {
                    throw new UnreachableException();
                }
            }

            throw new UnreachableException();
        }

        public static bool operator ==(ThrowExceptionInUnexpectedLocation x, ThrowExceptionInUnexpectedLocation y)
            => throw new UnreachableException();

        public static bool operator !=(ThrowExceptionInUnexpectedLocation x, ThrowExceptionInUnexpectedLocation y)
        {
            throw new UnreachableException();
        }

        public static implicit operator DateTime(ThrowExceptionInUnexpectedLocation x)
        {
            throw new UnreachableException();
        }

        public static implicit operator Guid(ThrowExceptionInUnexpectedLocation x) => throw new UnreachableException();
    }

    public record Record(string S, EventHandler H)
    {
        string Property4 { get; } = S ?? throw new UnreachableException();

        string Property5 { get; set; } = S ?? throw new UnreachableException();

        event EventHandler E = H ?? throw new UnreachableException();

        static string F() => null;

        string Property6 { get; } = F() ?? throw new UnreachableException();

        string Property7 { get; set; } = F() ?? throw new UnreachableException();

        static string Property8 { get; } = F() ?? throw new UnreachableException();

        static string Property9 { get; set; } = F() ?? throw new UnreachableException();

        string field1 = S ?? throw new UnreachableException()

        static string field2 = F() ?? throw new UnreachableException();

        static EventHandler E2() => null;

        static event EventHandler Event = E2() ?? throw new UnreachableException();
    }

    public record Record2(string S, EventHandler H)
    {
        Lazy<string> Property4 { get; } = new(() => S ?? throw new UnreachableException());

        Lazy<string> Property5 { get; set; } = new(() => S ?? throw new UnreachableException());

        event EventHandler E = new Lazy<EventHandler>(() => H ?? throw new UnreachableException()).Value;

        static string F() => null;

        Lazy<string> Property6 { get; } = new(() => F() ?? throw new UnreachableException());

        Lazy<string> Property7 { get; set; } = new(() => F() ?? throw new UnreachableException());

        static Lazy<string> Property8 { get; } = new(() => F() ?? throw new UnreachableException());

        static Lazy<string> Property9 { get; set; } = new(() => F() ?? throw new UnreachableException());

        Lazy<string> field1 = new(() => S ?? throw new UnreachableException());

        static Lazy<string> field2 = new(() => F() ?? throw new UnreachableException());

        static EventHandler E2() => null;

        static event EventHandler Event = new Lazy<EventHandler>(() => E2() ?? throw new UnreachableException()).Value;
    }
}