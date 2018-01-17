using System;

namespace Test
{
    public class ArgumentExceptionConstructorArgument
    {
        void _ArgumentException(int one)
        {
            throw new ArgumentException();

            throw new ArgumentException("one");
            throw new ArgumentException(nameof(one));
            throw new ArgumentException("some message");

            var innerException = null as Exception;
            throw new ArgumentException("one", innerException);
            throw new ArgumentException(nameof(one), innerException);
            throw new ArgumentException("some message", innerException);

            throw new ArgumentException("one", "some message");
            throw new ArgumentException(nameof(one), "some message");
            throw new ArgumentException("some message", "one");
            throw new ArgumentException("some message", nameof(one));

            throw new ArgumentException("one", "some message", innerException);
            throw new ArgumentException(nameof(one), "some message", innerException);
            throw new ArgumentException("some message", "one", innerException);
            throw new ArgumentException("some message", nameof(one), innerException);
        }

        void _ArgumentNullException(int one)
        {
            throw new ArgumentNullException();

            throw new ArgumentNullException("one");
            throw new ArgumentNullException(nameof(one));

            var innerException = null as Exception;
            throw new ArgumentNullException("one", innerException);
            throw new ArgumentNullException(nameof(one), innerException);
            throw new ArgumentNullException("some message", innerException);

            throw new ArgumentNullException("one", "some message");
            throw new ArgumentNullException(nameof(one), "some message");
            throw new ArgumentNullException("some message", "one");
            throw new ArgumentNullException("some message", nameof(one));
        }

        void _ArgumentOutOfRangeException(int one)
        {
            throw new ArgumentOutOfRangeException();

            throw new ArgumentOutOfRangeException("one");
            throw new ArgumentOutOfRangeException(nameof(one));

            var innerException = null as Exception;
            throw new ArgumentOutOfRangeException("one", innerException);
            throw new ArgumentOutOfRangeException(nameof(one), innerException);
            throw new ArgumentOutOfRangeException("some message", innerException);

            throw new ArgumentOutOfRangeException("one", "some message");
            throw new ArgumentOutOfRangeException(nameof(one), "some message");
            throw new ArgumentOutOfRangeException("some message", "one");
            throw new ArgumentOutOfRangeException("some message", nameof(one));

            throw new ArgumentOutOfRangeException("one", one, "some message");
            throw new ArgumentOutOfRangeException(nameof(one), one, "some message");
            throw new ArgumentOutOfRangeException("some message", one, "one");
            throw new ArgumentOutOfRangeException("some message", one, nameof(one));
        }
    }
}