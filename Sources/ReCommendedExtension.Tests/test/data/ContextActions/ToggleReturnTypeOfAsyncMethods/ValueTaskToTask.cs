using System.Threading.Tasks;

namespace Test
{
    public class AsyncMethod
    {
        public async Value{caret}Task Method()
        {
            await Task.Yield();
        }
    }
}