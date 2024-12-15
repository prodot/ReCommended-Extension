using System.Threading.Tasks;

namespace Test
{
    public class AsyncMethod
    {
        public async Ta{caret}sk Method()
        {
            await Task.Yield();
        }
    }
}