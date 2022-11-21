using System.Threading.Tasks;

namespace Test
{
    internal class Availability
    {
        string Meth{on}od() => null;

        void MethodWithLocalFunction()
        {
            string Local{off}Function() => null;
        }

        void Method{off}2() => null;

        async Task Method{on}3() => await Task.Yield();

        string Proper{off}ty => null;
    }
}