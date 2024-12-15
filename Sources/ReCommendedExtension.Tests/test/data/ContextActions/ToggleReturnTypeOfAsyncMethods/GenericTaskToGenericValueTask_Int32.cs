using System.Threading.Tasks;

namespace Test
{
    public class AsyncMethod
    {
        public async Ta{caret}sk<int> Method(int value)
        {
            await Task.Yield();
            return value;
        }
    }
}