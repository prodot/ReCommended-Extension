using System.Threading.Tasks;

namespace Test
{
    public class AsyncMethod
    {
        public async Ta{on}sk Task{off}Method()
        {
            await Task.Yield();
        }

        public async Task{on}<int> TaskMethod(int value)
        {
            await Task.Yield();
            return value;
        }

        public async Value{on}Task ValueTaskMethod()
        {
            await Task.Yield();
        }

        public async Value{on}Task<int> ValueTaskMethod(int value)
        {
            await Task.Yield();
            return value;
        }

        public void LocalFunctions()
        {
            public async Ta{on}sk TaskMethod()
            {
                await Task.Yield();
            }

            public async Task{on}<int> TaskMethod(int value)
            {
                await Task.Yield();
                return value;
            }

            public async Value{on}Task ValueTaskMethod()
            {
                await Task.Yield();
            }

            public async Value{on}Task<int> ValueTaskMethod(int value)
            {
                await Task.Yield();
                return value;
            }
        }
    }

    public class NonAsyncMethod
    {
        public Ta{off}sk TaskMethod()
        {
            return Task.CompletedTask;
        }

        public Task{off}<int> TaskMethod(int value)
        {
            return Task.FromResult(value);
        }

        public Value{off}Task ValueTaskMethod()
        {
            return ValueTask.CompletedTask;
        }

        public Value{off}Task<int> ValueTaskMethod(int value)
        {
            return ValueTask.FromResult(value);
        }

        public void LocalFunctions()
        {
            public Ta{off}sk TaskMethod()
            {
                return Task.CompletedTask;
            }

            public Task{off}<int> TaskMethod(int value)
            {
                return Task.FromResult(value);
            }

            public Value{off}Task ValueTaskMethod()
            {
                return ValueTask.CompletedTask;
            }

            public Value{off}Task<int> ValueTaskMethod(int value)
            {
                return ValueTask.FromResult(value);
            }
        }
    }
}