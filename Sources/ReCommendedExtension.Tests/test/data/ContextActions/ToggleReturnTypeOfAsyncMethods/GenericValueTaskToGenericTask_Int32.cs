using System.Threading.Tasks;

namespace Test
{
    public class AsyncMethod
    {
        public async Value{caret}Task<int> Method(int value)
        {
            await Task.Yield();
            return value;
        }
    }
}