using System;

namespace Test
{
    internal class UnthrowableException
    {
        void Method()
        {
            throw new Exception();
        }

        void Method2() => throw new Exception();

        string x;

        void Method3(string value) => x = value ?? throw new Exception();
    }
}