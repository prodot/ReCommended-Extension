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

    public class MultipleConsumption
    {
        async ValueTask Awaiting()
        {
            var vt = Test.Delay();

            await vt;
            await vt;
        }

        async ValueTask CallingAsTask(ValueTask vt)
        {
            await vt.AsTask();
            await vt.AsTask();
        }

        async ValueTask AwaitingAndCallingAsTask(ValueTask vt)
        {
            await vt;
            await vt.AsTask();
        }

        async ValueTask PassingAsArgument()
        {
            var vt = Test.Delay();

            await CallingAsTask(vt);
            await AwaitingAndCallingAsTask(vt);
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
                return;
            }

            await vt;
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
                    goto case 0;

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
            }
        }

        async ValueTask Loop_Condition(ValueTask<int> vt)
        {
            while (await vt > 4)
            {
                Console.WriteLine();
            }
        }
    }
}