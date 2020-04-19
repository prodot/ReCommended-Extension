using System;

namespace Test
{
    internal static class Class
    {
        static void Foo(string? x)
        {
            if (x != null)
            {
                Console.WriteLine(x{caret}!.Length);
            }
        }
    }
}