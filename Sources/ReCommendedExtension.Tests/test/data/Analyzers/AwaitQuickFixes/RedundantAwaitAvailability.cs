using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        async Task Method()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        }

        async Task Method_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        async Task Method_AsExpressionBodied() => await Task.Delay(10);

        async Task Method_AsExpressionBodied_WithConfigureAwait() => await Task.Delay(10).ConfigureAwait(false);

        async Task<int> Method2()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Task.FromResult(LocalFunction());

            int LocalFunction() => 3;
        }

        async Task<int> Method2_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Task.FromResult(LocalFunction()).ConfigureAwait(false);

            int LocalFunction() => 3;
        }

        async Task<int> Method2_AsExpressionBodied() => await Task.FromResult(3);

        async Task<int> Method2_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult(3).ConfigureAwait(false);

        async Task Method4()
        {
            Console.WriteLine();
            await Task.FromResult("one");
        }

        async Task Method4_WithConfigureAwait()
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        }

        async Task Method4_AsExpressionBodied() => await Task.FromResult("one");

        async Task Method4_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult("one").ConfigureAwait(false);
    }

    public class AwaitForLambdaVariables
    {
        void Method()
        {
            Func<Task> Method = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Task.Delay(10);
            };

            Func<Task> Method_WithConfigureAwait = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Task.Delay(10).ConfigureAwait(false);
            };

            Func<Task> Method_AsExpressionBodied = async () => await Task.Delay(10);

            Func<Task> Method_AsExpressionBodied_WithConfigureAwait = async () => await Task.Delay(10).ConfigureAwait(false);

            Func<Task<int>> Method2 = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Task.FromResult(LocalFunction());

                int LocalFunction() => 3;
            };

            Func<Task<int>> Method2_WithConfigureAwait = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Task.FromResult(LocalFunction()).ConfigureAwait(false);

                int LocalFunction() => 3;
            };

            Func<Task<int>> Method2_AsExpressionBodied = async () => await Task.FromResult(3);

            Func<Task<int>> Method2_AsExpressionBodied_WithConfigureAwait = async () => await Task.FromResult(3).ConfigureAwait(false);

            Func<Task> Method4 = async () =>
            {
                Console.WriteLine();
                await Task.FromResult("one");
            };

            Func<Task> Method4_WithConfigureAwait = async () =>
            {
                Console.WriteLine();
                await Task.FromResult("one").ConfigureAwait(false);
            };

            Func<Task> Method4_AsExpressionBodied = async () => await Task.FromResult("one");

            Func<Task> Method4_AsExpressionBodied_WithConfigureAwait = async () => await Task.FromResult("one").ConfigureAwait(false);
        }
    }

    public class AwaitForLambdaFields
    {
        Func<Task> Method = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        };

        Func<Task> Method_WithConfigureAwait = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        };

        Func<Task> Method_AsExpressionBodied = async () => await Task.Delay(10);

        Func<Task> Method_AsExpressionBodied_WithConfigureAwait = async () => await Task.Delay(10).ConfigureAwait(false);

        Func<Task<int>> Method2 = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Task.FromResult(LocalFunction());

            int LocalFunction() => 3;
        };

        Func<Task<int>> Method2_WithConfigureAwait = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Task.FromResult(LocalFunction()).ConfigureAwait(false);

            int LocalFunction() => 3;
        };

        Func<Task<int>> Method2_AsExpressionBodied = async () => await Task.FromResult(3);

        Func<Task<int>> Method2_AsExpressionBodied_WithConfigureAwait = async () => await Task.FromResult(3).ConfigureAwait(false);

        Func<Task> Method4 = async () =>
        {
            Console.WriteLine();
            await Task.FromResult("one");
        };

        Func<Task> Method4_WithConfigureAwait = async () =>
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        };

        Func<Task> Method4_AsExpressionBodied = async () => await Task.FromResult("one");

        Func<Task> Method4_AsExpressionBodied_WithConfigureAwait = async () => await Task.FromResult("one").ConfigureAwait(false);
    }

    public class AwaitForAnonymousMethodVariables
    {
        void Method()
        {
            Func<Task> Method = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Task.Delay(10);
            };

            Func<Task> Method_WithConfigureAwait = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Task.Delay(10).ConfigureAwait(false);
            };

            Func<Task<int>> Method2 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Task.FromResult(LocalFunction());

                int LocalFunction() => 3;
            };

            Func<Task<int>> Method2_WithConfigureAwait = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Task.FromResult(LocalFunction()).ConfigureAwait(false);

                int LocalFunction() => 3;
            };

            Func<Task> Method4 = async delegate
            {
                Console.WriteLine();
                await Task.FromResult("one");
            };

            Func<Task> Method4_WithConfigureAwait = async delegate
            {
                Console.WriteLine();
                await Task.FromResult("one").ConfigureAwait(false);
            };
        }
    }

    public class AwaitForAnonymousMethodFields
    {
        Func<Task> Method = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10);
        };

        Func<Task> Method_WithConfigureAwait = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await Task.Delay(10).ConfigureAwait(false);
        };

        Func<Task<int>> Method2 = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Task.FromResult(LocalFunction());

            int LocalFunction() => 3;
        };

        Func<Task<int>> Method2_WithConfigureAwait = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Task.FromResult(LocalFunction()).ConfigureAwait(false);

            int LocalFunction() => 3;
        };

        Func<Task> Method4 = async delegate
        {
            Console.WriteLine();
            await Task.FromResult("one");
        };

        Func<Task> Method4_WithConfigureAwait = async delegate
        {
            Console.WriteLine();
            await Task.FromResult("one").ConfigureAwait(false);
        };
    }

    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async Task Method()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Task.Delay(10);
            }

            async Task Method_WithConfigureAwait()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await Task.Delay(10).ConfigureAwait(false);
            }

            async Task Method_AsExpressionBodied() => await Task.Delay(10);

            async Task Method_AsExpressionBodied_WithConfigureAwait() => await Task.Delay(10).ConfigureAwait(false);

            async Task<int> Method2()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Task.FromResult(LocalFunction());

                int LocalFunction() => 3;
            }

            async Task<int> Method2_WithConfigureAwait()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await Task.FromResult(LocalFunction()).ConfigureAwait(false);

                int LocalFunction() => 3;
            }

            async Task<int> Method2_AsExpressionBodied() => await Task.FromResult(3);

            async Task<int> Method2_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult(3).ConfigureAwait(false);

            async Task Method4()
            {
                Console.WriteLine();
                await Task.FromResult("one");
            }

            async Task Method4_WithConfigureAwait()
            {
                Console.WriteLine();
                await Task.FromResult("one").ConfigureAwait(false);
            }

            async Task Method4_AsExpressionBodied() => await Task.FromResult("one");

            async Task Method4_AsExpressionBodied_WithConfigureAwait() => await Task.FromResult("one").ConfigureAwait(false);
        }
    }
}