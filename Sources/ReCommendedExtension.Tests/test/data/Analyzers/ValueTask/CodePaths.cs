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

    public class CodePaths
    {
        async ValueTask OnePath(ValueTask vt)
        {
            await vt;
            await vt;
        }

        async ValueTask TwoPaths(ValueTask vt, int x)
        {
            if (x > 3)
            {
                await vt;
            }
            else
            {
                await vt;
                await vt;
            }

            await vt;
        }

        async ValueTask TwoPaths_TwoExits(ValueTask vt, int x)
        {
            if (x > 3)
            {
                await vt;
            }
            else
            {
                await vt;
                await vt;
                return;
            }

            await vt;
        }

        async ValueTask TwoPaths_TwoExits_Throws(ValueTask vt, int x)
        {
            if (x > 3)
            {
                await vt;
            }
            else
            {
                await vt;
                await vt;
                throw new NotSupportedException();
            }

            await vt;
        }

        async ValueTask ThreePaths(ValueTask vt, int x)
        {
            switch (x)
            {
                case 0:
                    await vt;
                    break;

                case 1:
                case 2:
                    await vt;
                    await vt;
                    break;

                default:
                    await vt;
                    await vt;
                    await vt;
                    break;
            }
        }

        async ValueTask ThreePaths_Complex(ValueTask vt, ValueTask<int> x, ValueTask<int> y)
        {
            switch (await x)
            {
                case 0 when (await y) > 2:
                    await vt;
                    return;

                case 1 when (await y) > 5:
                    await vt;
                    break;

                default:
                    await vt;
                    throw new NotSupportedException();
            }

            await vt;
        }

        async ValueTask Loop(ValueTask vt)
        {
            for (var i = 0; i < 10; i++)
            {
                await vt;
            }
        }

        async ValueTask Loop2(ValueTask vt)
        {
            for (var i = 0, j = 1; i < 10 && j < 11; i++, j++)
            {
                await vt;
            }
        }

        async ValueTask Loop_Forever(ValueTask vt)
        {
            for (;;)
            {
                await vt;
            }
        }

        async ValueTask Loop_WithBreak(ValueTask vt, int[] a)
        {
            foreach (var i in a)
            {
                if (i = 3)
                {
                    await vt;
                    break;
                }
            }
        }

        async ValueTask Loop_WithContinue(ValueTask vt, int[] a)
        {
            foreach (var i in a)
            {
                if (i != 3)
                {
                    continue;
                }

                await vt;
                return;
            }
        }

        async ValueTask Loop_InCondition(ValueTask<int> vt)
        {
            while (await vt > 3)
            {
                Console.WriteLine();
            }
        }

        async ValueTask Loop_InCondition_OneIteration(ValueTask<int> vt)
        {
            while (await vt > 3)
            {
                break;
            }
        }

        async ValueTask Captures(ValueTask vt)
        {
            await LocalFunction();
            await LocalFunction();

            async ValueTask LocalFunction()
            {
                await vt;
            }
        }
    }
}