using System.Threading.Tasks;

namespace Test
{
    public class AsyncMethod
    {
        public async Ta{caret}sk<T> Method<T>(T value)
        {
            await Task.Yield();
            return value;
        }
    }
}