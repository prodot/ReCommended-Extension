using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.Await
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

    public class AwaitForMethods
    {
        async ValueTask Method()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Test.Delay();
        }

        async ValueTask Method_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Test.Delay().ConfigureAwait(false);
        }

        async ValueTask Method_AsExpressionBodied() => await Test.Delay();

        async ValueTask Method_AsExpressionBodied_WithConfigureAwait() => await Test.Delay().ConfigureAwait(false);

        async ValueTask<int> Method2()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Test.Calc(LocalFunction());

            int LocalFunction() => 3;
        }

        async ValueTask<int> Method2_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Test.Calc(LocalFunction()).ConfigureAwait(false);

            int LocalFunction() => 3;
        }

        async ValueTask<int> Method2_AsExpressionBodied() => await Test.Calc(3);

        async ValueTask<int> Method2_AsExpressionBodied_WithConfigureAwait() => await Test.Calc(3).ConfigureAwait(false);

        async ValueTask Method4()
        {
            Console.WriteLine();
            await Test.Calc("one");
        }

        async ValueTask Method4_WithConfigureAwait()
        {
            Console.WriteLine();
            await Test.Calc("one").ConfigureAwait(false);
        }

        async ValueTask Method4_AsExpressionBodied() => await Test.Calc("one");

        async ValueTask Method4_AsExpressionBodied_WithConfigureAwait() => await Test.Calc("one").ConfigureAwait(false);

        async ValueTask<bool> Method_MultipleAwaits()
        {
            return await Test.Calc(await Test.Calc(false));
        }

        async ValueTask<bool> MethodA_MultipleAwaits_AsExpressionBodied() => await Test.Calc(await Test.Calc(false));

        async ValueTask Method_AwaitNonLast()
        {
            await Test.Delay();
            Console.WriteLine();
        }

        async ValueTask<object> Method_Covariant()
        {
            return await Test.Calc("one");
        }

        async ValueTask<object> Method_Covariant_AsExpressionBodied() => await Test.Calc("one");

        async ValueTask<IDictionary<int, IList<string>>> Method_Covariant_MoreComplex()
        {
            return await Test.Calc(new Dictionary<int, IList<string>>());
        }

        async ValueTask<IDictionary<int, IList<string>>> Method_Covariant_MoreComplex_AsExpressionBodied()
            => await Test.Calc(new Dictionary<int, IList<string>>());

        async ValueTask Method_AwaitNonValueTask()
        {
            await Task.Delay(10);
        }

        async ValueTask Method_AwaitNonValueTask_AsExpressionBodied() => await Task.Delay(10).ConfigureAwait(false);

        async ValueTask Method_UsingVar()
        {
            using var p = new Process();

            await Test.Calc(35);
        }
    }

    public class AwaitForLambdaVariables
    {
        void Method()
        {
            Func<ValueTask> Method = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Test.Delay();
            };

            Func<ValueTask> Method_WithConfigureAwait = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Test.Delay().ConfigureAwait(false);
            };

            Func<ValueTask> Method_AsExpressionBodied = async () => await Test.Delay();

            Func<ValueTask> Method_AsExpressionBodied_WithConfigureAwait = async () => await Test.Delay().ConfigureAwait(false);

            Func<ValueTask<int>> Method2 = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Test.Calc(LocalFunction());

                int LocalFunction() => 3;
            };

            Func<ValueTask<int>> Method2_WithConfigureAwait = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Test.Calc(LocalFunction()).ConfigureAwait(false);

                int LocalFunction() => 3;
            };

            Func<ValueTask<int>> Method2_AsExpressionBodied = async () => await Test.Calc(3);

            Func<ValueTask<int>> Method2_AsExpressionBodied_WithConfigureAwait = async () => await Test.Calc(3).ConfigureAwait(false);

            Func<ValueTask> Method4 = async () =>
            {
                Console.WriteLine();
                await Test.Calc("one");
            };

            Func<ValueTask> Method4_WithConfigureAwait = async () =>
            {
                Console.WriteLine();
                await Test.Calc("one").ConfigureAwait(false);
            };

            Func<ValueTask> Method4_AsExpressionBodied = async () => await Test.Calc("one");

            Func<ValueTask> Method4_AsExpressionBodied_WithConfigureAwait = async () => await Test.Calc("one").ConfigureAwait(false);

            Func<ValueTask<bool>> Method_MultipleAwaits = async () => { return await Test.Calc(await Test.Calc(false)); };

            Func<ValueTask<bool>> MethodA_MultipleAwaits_AsExpressionBodied = async () => await Test.Calc(await Test.Calc(false));

            Func<ValueTask> Method_AwaitNonLast = async () =>
            {
                await Test.Delay();
                Console.WriteLine();
            };

            Func<ValueTask<object>> Method_Covariant = async () => { return await Test.Calc("one"); };

            Func<ValueTask<object>> Method_Covariant_AsExpressionBodied = async () => await Test.Calc("one");

            Func<ValueTask<IDictionary<int, IList<string>>>> Method_Covariant_MoreComplex = async () =>
            {
                return await Test.Calc(new Dictionary<int, IList<string>>());
            };

            Func<ValueTask<IDictionary<int, IList<string>>>> Method_Covariant_MoreComplex_AsExpressionBodied = async ()
                => await Test.Calc(new Dictionary<int, IList<string>>());

            Func<ValueTask> Method_AwaitNonValueTask = async () => { await Task.Delay(10); };

            Func<ValueTask> Method_AwaitNonValueTask_AsExpressionBodied = async () => await Task.Delay(10);

            Func<ValueTask> Method_UsingVar = async () =>
            {
                using var p = new Process();

                await Test.Calc(35);
            };
        }
    }

    public class AwaitForLambdaFields
    {
        Func<ValueTask> Method = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Test.Delay();
        };

        Func<ValueTask> Method_WithConfigureAwait = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Test.Delay().ConfigureAwait(false);
        };

        Func<ValueTask> Method_AsExpressionBodied = async () => await Test.Delay();

        Func<ValueTask> Method_AsExpressionBodied_WithConfigureAwait = async () => await Test.Delay().ConfigureAwait(false);

        Func<ValueTask<int>> Method2 = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Test.Calc(LocalFunction());

            int LocalFunction() => 3;
        };

        Func<ValueTask<int>> Method2_WithConfigureAwait = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Test.Calc(LocalFunction()).ConfigureAwait(false);

            int LocalFunction() => 3;
        };

        Func<ValueTask<int>> Method2_AsExpressionBodied = async () => await Test.Calc(3);

        Func<ValueTask<int>> Method2_AsExpressionBodied_WithConfigureAwait = async () => await Test.Calc(3).ConfigureAwait(false);

        Func<ValueTask> Method4 = async () =>
        {
            Console.WriteLine();
            await Test.Calc("one");
        };

        Func<ValueTask> Method4_WithConfigureAwait = async () =>
        {
            Console.WriteLine();
            await Test.Calc("one").ConfigureAwait(false);
        };

        Func<ValueTask> Method4_AsExpressionBodied = async () => await Test.Calc("one");

        Func<ValueTask> Method4_AsExpressionBodied_WithConfigureAwait = async () => await Test.Calc("one").ConfigureAwait(false);

        Func<ValueTask<bool>> Method_MultipleAwaits = async () => { return await Test.Calc(await Test.Calc(false)); };

        Func<ValueTask<bool>> MethodA_MultipleAwaits_AsExpressionBodied = async () => await Test.Calc(await Test.Calc(false));

        Func<ValueTask> Method_AwaitNonLast = async () =>
        {
            await Test.Delay();
            Console.WriteLine();
        };

        Func<ValueTask<object>> Method_Covariant = async () => { return await Test.Calc("one"); };

        Func<ValueTask<object>> Method_Covariant_AsExpressionBodied = async () => await Test.Calc("one");

        Func<ValueTask<IDictionary<int, IList<string>>>> Method_Covariant_MoreComplex = async () =>
        {
            return await Test.Calc(new Dictionary<int, IList<string>>());
        };

        Func<ValueTask<IDictionary<int, IList<string>>>> Method_Covariant_MoreComplex_AsExpressionBodied = async ()
            => await Test.Calc(new Dictionary<int, IList<string>>());

        Func<ValueTask> Method_AwaitNonValueTask = async () => { await Task.Delay(10); };

        Func<ValueTask> Method_AwaitNonValueTask_AsExpressionBodied = async () => await Task.Delay(10);

        Func<ValueTask> Method_UsingVar = async () =>
        {
            using var p = new Process();

            await Test.Calc(35);
        };
    }

    public class AwaitForAnonymousMethodVariables
    {
        void Method()
        {
            Func<ValueTask> Method = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Test.Delay();
            };

            Func<ValueTask> Method_WithConfigureAwait = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Test.Delay().ConfigureAwait(false);
            };

            Func<ValueTask<int>> Method2 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Test.Calc(LocalFunction());

                int LocalFunction() => 3;
            };

            Func<ValueTask<int>> Method2_WithConfigureAwait = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Test.Calc(LocalFunction()).ConfigureAwait(false);

                int LocalFunction() => 3;
            };

            Func<ValueTask> Method4 = async delegate
            {
                Console.WriteLine();
                await Test.Calc("one");
            };

            Func<ValueTask> Method4_WithConfigureAwait = async delegate
            {
                Console.WriteLine();
                await Test.Calc("one").ConfigureAwait(false);
            };

            Func<ValueTask<bool>> Method_MultipleAwaits = async delegate { return await Test.Calc(await Test.Calc(false)); };

            Func<ValueTask> Method_AwaitNonLast = async delegate
            {
                await Test.Delay();
                Console.WriteLine();
            };

            Func<ValueTask<object>> Method_Covariant = async delegate { return await Test.Calc("one"); };

            Func<ValueTask<IDictionary<int, IList<string>>>> Method_Covariant_MoreComplex = async delegate
            {
                return await Test.Calc(new Dictionary<int, IList<string>>());
            };

            Func<ValueTask> Method_AwaitNonValueTask = async delegate { await Task.Delay(10); };

            Func<ValueTask> Method_UsingVar = async delegate
            {
                using var p = new Process();

                await Test.Calc(35);
            };
        }
    }

    public class AwaitForAnonymousMethodFields
    {
        Func<ValueTask> Method = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Test.Delay();
        };

        Func<ValueTask> Method_WithConfigureAwait = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Test.Delay().ConfigureAwait(false);
        };

        Func<ValueTask<int>> Method2 = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Test.Calc(LocalFunction());

            int LocalFunction() => 3;
        };

        Func<ValueTask<int>> Method2_WithConfigureAwait = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Test.Calc(LocalFunction()).ConfigureAwait(false);

            int LocalFunction() => 3;
        };

        Func<ValueTask> Method4 = async delegate
        {
            Console.WriteLine();
            await Test.Calc("one");
        };

        Func<ValueTask> Method4_WithConfigureAwait = async delegate
        {
            Console.WriteLine();
            await Test.Calc("one").ConfigureAwait(false);
        };

        Func<ValueTask<bool>> Method_MultipleAwaits = async delegate { return await Test.Calc(await Test.Calc(false)); };

        Func<ValueTask> Method_AwaitNonLast = async delegate
        {
            await Test.Delay();
            Console.WriteLine();
        };

        Func<ValueTask<object>> Method_Covariant = async delegate { return await Test.Calc("one"); };

        Func<ValueTask<IDictionary<int, IList<string>>>> Method_Covariant_MoreComplex = async delegate
        {
            return await Test.Calc(new Dictionary<int, IList<string>>());
        };

        Func<ValueTask> Method_AwaitNonValueTask = async delegate { await Task.Delay(10); };

        Func<ValueTask> Method_UsingVar = async delegate
        {
            using var p = new Process();

            await Test.Calc(35);
        };
    }

    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async ValueTask Method()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Test.Delay();
            }

            async ValueTask Method_WithConfigureAwait()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Test.Delay().ConfigureAwait(false);
            }

            async ValueTask Method_AsExpressionBodied() => await Test.Delay();

            async ValueTask Method_AsExpressionBodied_WithConfigureAwait() => await Test.Delay().ConfigureAwait(false);

            async ValueTask<int> Method2()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Test.Calc(LocalFunction());

                int LocalFunction() => 3;
            }

            async ValueTask<int> Method2_WithConfigureAwait()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Test.Calc(LocalFunction()).ConfigureAwait(false);

                int LocalFunction() => 3;
            }

            async ValueTask<int> Method2_AsExpressionBodied() => await Test.Calc(3);

            async ValueTask<int> Method2_AsExpressionBodied_WithConfigureAwait() => await Test.Calc(3).ConfigureAwait(false);

            async ValueTask Method4()
            {
                Console.WriteLine();
                await Test.Calc("one");
            }

            async ValueTask Method4_WithConfigureAwait()
            {
                Console.WriteLine();
                await Test.Calc("one").ConfigureAwait(false);
            }

            async ValueTask Method4_AsExpressionBodied() => await Test.Calc("one");

            async ValueTask Method4_AsExpressionBodied_WithConfigureAwait() => await Test.Calc("one").ConfigureAwait(false);

            async ValueTask<bool> Method_MultipleAwaits()
            {
                return await Test.Calc(await Test.Calc(false));
            }

            async ValueTask<bool> MethodA_MultipleAwaits_AsExpressionBodied() => await Test.Calc(await Test.Calc(false));

            async ValueTask Method_AwaitNonLast()
            {
                await Test.Delay();
                Console.WriteLine();
            }

            async ValueTask<object> Method_Covariant()
            {
                return await Test.Calc("one");
            }

            async ValueTask<object> Method_Covariant_AsExpressionBodied() => await Test.Calc("one");

            async ValueTask<IDictionary<int, IList<string>>> Method_Covariant_MoreComplex()
            {
                return await Test.Calc(new Dictionary<int, IList<string>>());
            }

            async ValueTask<IDictionary<int, IList<string>>> Method_Covariant_MoreComplex_AsExpressionBodied()
                => await Test.Calc(new Dictionary<int, IList<string>>());

            async ValueTask Method_AwaitNonValueTask()
            {
                await Task.Delay(10);
            }

            async ValueTask Method_AwaitNonValueTask_AsExpressionBodied() => await Task.Delay(10);

            async ValueTask Method_UsingVar()
            {
                using var p = new Process();

                await Test.Calc(35);
            }
        }
    }
}