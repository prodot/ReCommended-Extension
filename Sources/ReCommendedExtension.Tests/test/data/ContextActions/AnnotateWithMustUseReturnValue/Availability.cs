using System.Threading.Tasks;

namespace Test
{
    internal class Availability
    {
        string Meth{on}od() => null;

        void Method{off}2() => null;

        async Task Method{off}3() => await Task.Yield();

        string Proper{off}ty => null;
    }
}