using System.Threading.Tasks;

namespace Tests
{
    public class IntentionalBlockingAttempts
    {
        ValueTask Method(ValueTask<int> valueTask)
        {
            var result = valueTask.Get{caret}Awaiter().GetResult();

            return new ValueTask();
        }
    }
}