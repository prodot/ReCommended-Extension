using System;
using System.Threading.Tasks;

namespace Tests
{
    public static class Test
    {
        public static async ValueTask Delay() => await Task.Delay(10);

        public static async ValueTask<T> Calc<T>(T x)
        {
            await Task.Delay(10);
            return x;
        }
    }

    public class SingleConsumption
    {
        async ValueTask Awaiting()
        {
            var vt = Test.Delay();

            await vt;
        }

        async ValueTask CallingAsTask(ValueTask vt)
        {
            await vt.AsTask();
        }

        async ValueTask PassingAsArgument()
        {
            var vt = Test.Delay();

            await CallingAsTask(vt);
        }

        void WellKnownInvocations(ValueTask vt, ValueTask other)
        {
            if (vt.IsCanceled || !vt.IsFaulted || vt.IsCompleted)
            {
                return;
            }

            if (vt.IsCompletedSuccessfully)
            {
                Console.WriteLine(vt.ToString());
            }

            if (vt == other) { }

            if (vt.Equals(other)) { }
            if (!vt.Equals(other)) { }

            Console.WriteLine(vt.GetHashCode());
        }

        async ValueTask IfElse(int x, ValueTask vt)
        {
            if (x > 3)
            {
                await vt;
            }
            else
            {
                await vt.ConfigureAwait(false);
            }
        }

        async ValueTask ConditionalExpression(int x, ValueTask<int> vt)
        {
            return x > 3 ? await vt : await vt.ConfigureAwait(false);
        }

        async ValueTask Switch(int x, ValueTask vt)
        {
            switch (x)
            {
                case 0:
                    await vt;
                    break;

                case 1:
                case 2:
                    await vt;
                    break;

                case 3:
                    await vt;
                    return;

                case 4:
                    await vt;
                    throw new InvalidOperationException();
            }
        }

        async ValueTask Loop(ValueTask vt, int[] a)
        {
            foreach (var i in a)
            {
                await vt.AsTask();
                break;
            }
        }
    }
}