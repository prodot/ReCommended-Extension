using System.Threading.Tasks;

namespace Tests
{
    public class IntentionalBlockingAttempts
    {
        ValueTask Method(ValueTask<int> valueTask)
        {
            var result = valueTask.GetAwaiter().GetResult();

            return new ValueTask();
        }

        void Method2(ValueTask<int> valueTask) => Method(valueTask).GetAwaiter().GetResult();
    }
}