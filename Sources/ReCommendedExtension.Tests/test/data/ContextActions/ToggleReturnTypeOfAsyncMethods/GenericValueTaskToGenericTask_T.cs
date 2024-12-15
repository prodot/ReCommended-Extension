using System.Threading.Tasks;

namespace Test
{
    public class AsyncMethod<T>
    {
        public async Value{caret}Task<T> Method(T value)
        {
            await Task.Yield();
            return value;
        }
    }
}