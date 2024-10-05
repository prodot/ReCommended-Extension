using System.Threading.Tasks;

namespace Test
{
    public class AsyncMethod
    {
        public async Ta{off}sk TaskMethod()
        {
            await Task.Yield();
        }

        public async Task{off}<int> TaskMethod(int value)
        {
            await Task.Yield();
            return value;
        }

        public async Value{off}Task ValueTaskMethod()
        {
            await Task.Yield();
        }

        public async Value{off}Task<int> ValueTaskMethod(int value)
        {
            await Task.Yield();
            return value;
        }

        public void LocalFunctions()
        {
            public async Ta{off}sk TaskMethod()
            {
                await Task.Yield();
            }

            public async Task{off}<int> TaskMethod(int value)
            {
                await Task.Yield();
                return value;
            }

            public async Value{off}Task ValueTaskMethod()
            {
                await Task.Yield();
            }

            public async Value{off}Task<int> ValueTaskMethod(int value)
            {
                await Task.Yield();
                return value;
            }
        }
    }
}