using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.Await
{
    internal static class ValueTasks
    {
        public static ValueTask Delay(int millisecondsDelay) => throw new NotImplementedException();
        public static ValueTask<T> FromResult<T>(T result) => new ValueTask<T>(result);
    }

    public class AwaitForMethods
    {
        async Task Method()
        {
            await ValueTasks.Delay(10);
            await ValueTasks.Delay(20);
        }

        async Task Method2()
        {
            if (Environment.UserInteractive)
            {
                await ValueTasks.Delay(10);
            }

            await ValueTasks.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        }

        async Task Method3() => await ValueTasks.Delay(10);

        async Task<int> Method4()
        {
            if (Environment.UserInteractive)
            {
                return await ValueTasks.FromResult(3);
            }

            await ValueTasks.Delay(10);
            return await ValueTasks.FromResult(4);
        }

        async Task<int> Method5()
        {
            await ValueTasks.Delay(10);
            return await ValueTasks.FromResult(3);

            int LocalFunction() => 4;
        }

        async Task Method6() => await ValueTasks.FromResult(3);

        async Task<int> Method7() => await ValueTasks.FromResult(3);

        async Task<int> Method_AwaitNonLast()
        {
            await ValueTasks.Delay(10);
            await ValueTasks.Delay(20);

            return 3;
        }

        async Task Method2_AwaitNonLast()
        {
            using (new Process())
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);
            }
        }

        async Task<int> Method3_AwaitNonLast()
        {
            await ValueTasks.Delay(10);
            var result = await ValueTasks.FromResult(3);
            return result;
        }

        async Task Method_WithConfigureAwait()
        {
            await ValueTasks.Delay(10).ConfigureAwait(false);
        }

        async Task Method_WithConfigureAwait_AsExpressionBodied() => await ValueTasks.Delay(10).ConfigureAwait(false);

        async Task<int> Method_NestedInUsingScope()
        {
            using (new Process())
            {
                return await ValueTasks.FromResult(3);
            }
        }

        async Task<int> Method_NestedInUsingScope(int x)
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }
            }

            throw new NotImplementedException();
        }

        async Task<int> Method_UsingDeclaration()
        {
            using var p = new Process();

            return await ValueTasks.FromResult(35);
        }

        async Task<int> Method_UsingDeclaration(int x)
        {
            using var p = new Process();

            if (x > 2)
            {
                return await ValueTasks.FromResult(3);
            }

            throw new NotImplementedException();
        }

        async Task<int> Method_NestedInTryBlock()
        {
            try
            {
                return await ValueTasks.FromResult(3);
            }
            catch
            {
                throw;
            }
        }

        async Task<int> Method_NestedInTryBlock(int x)
        {
            try
            {
                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }
            }
            catch
            {
                throw;
            }

            throw new NotImplementedException();
        }
    }

    public class AwaitForLambdaVariables
    {
        void Method()
        {
            Func<Task> Method = async () =>
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);
            };

            Func<Task> Method2 = async () =>
            {
                if (Environment.UserInteractive)
                {
                    await ValueTasks.Delay(10);
                }

                await ValueTasks.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            };

            Func<Task> Method3 = async () => await ValueTasks.Delay(10);

            Func<Task<int>> Method4 = async () =>
            {
                if (Environment.UserInteractive)
                {
                    return await ValueTasks.FromResult(3);
                }

                await ValueTasks.Delay(10);
                return await ValueTasks.FromResult(4);
            };

            Func<Task<int>> Method5 = async () =>
            {
                await ValueTasks.Delay(10);
                return await ValueTasks.FromResult(3);

                int LocalFunction() => 4;
            };

            Func<Task> Method6 = async () => await ValueTasks.FromResult(3);

            Func<Task<int>> Method7 = async () => await ValueTasks.FromResult(3);

            Func<Task<int>> Method_AwaitNonLast = async () =>
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);

                return 3;
            };

            Func<Task> Method2_AwaitNonLast = async () =>
            {
                using (new Process())
                {
                    await ValueTasks.Delay(10);
                    await ValueTasks.Delay(20);
                }
            };

            Func<Task<int>> Method3_AwaitNonLast = async () =>
            {
                await ValueTasks.Delay(10);
                var result = await ValueTasks.FromResult(3);
                return result;
            };

            Func<Task> Method_WithConfigureAwait = async () => { await ValueTasks.Delay(10).ConfigureAwait(false); };

            Func<Task> Method_WithConfigureAwait_AsExpressionBodied = async () => await ValueTasks.Delay(10).ConfigureAwait(false);

            Func<Task<int>> Method_NestedInUsingScope = async () =>
            {
                using (new Process())
                {
                    return await ValueTasks.FromResult(3);
                }
            };

            Func<int, Task<int>> Method_NestedInUsingScope2 = async (int x) =>
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await ValueTasks.FromResult(3);
                    }
                }

                throw new NotImplementedException();
            };

            Func<Task<int>> Method_UsingDeclaration = async () =>
            {
                using var p = new Process();

                return await ValueTasks.FromResult(3);
            };

            Func<int, Task<int>> Method_UsingDeclaration2 = async (int x) =>
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }

                throw new NotImplementedException();
            };

            Func<Task<int>> Method_NestedInTryBlock = async () =>
            {
                try
                {
                    return await ValueTasks.FromResult(3);
                }
                catch
                {
                    throw;
                }
            };

            Func<int, Task<int>> Method_NestedInTryBlock2 = async (int x) =>
            {
                try
                {
                    if (x > 2)
                    {
                        return await ValueTasks.FromResult(3);
                    }
                }
                catch
                {
                    throw;
                }

                throw new NotImplementedException();
            };
        }
    }

    public class AwaitForLambdaFields
    {
        Func<Task> Method = async () =>
        {
            await ValueTasks.Delay(10);
            await ValueTasks.Delay(20);
        };

        Func<Task> Method2 = async () =>
        {
            if (Environment.UserInteractive)
            {
                await ValueTasks.Delay(10);
            }

            await ValueTasks.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        };

        Func<Task> Method3 = async () => await ValueTasks.Delay(10);

        Func<Task<int>> Method4 = async () =>
        {
            if (Environment.UserInteractive)
            {
                return await ValueTasks.FromResult(3);
            }

            await ValueTasks.Delay(10);
            return await ValueTasks.FromResult(4);
        };

        Func<Task<int>> Method5 = async () =>
        {
            await ValueTasks.Delay(10);
            return await ValueTasks.FromResult(3);

            int LocalFunction() => 4;
        };

        Func<Task> Method6 = async () => await ValueTasks.FromResult(3);

        Func<Task<int>> Method7 = async () => await ValueTasks.FromResult(3);

        Func<Task<int>> Method_AwaitNonLast = async () =>
        {
            await ValueTasks.Delay(10);
            await ValueTasks.Delay(20);

            return 3;
        };

        Func<Task> Method2_AwaitNonLast = async () =>
        {
            using (new Process())
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);
            }
        };

        Func<Task<int>> Method3_AwaitNonLast = async () =>
        {
            await ValueTasks.Delay(10);
            var result = await ValueTasks.FromResult(3);
            return result;
        };

        Func<Task> Method_WithConfigureAwait = async () => { await ValueTasks.Delay(10).ConfigureAwait(false); };

        Func<Task> Method_WithConfigureAwait_AsExpressionBodied = async () => await ValueTasks.Delay(10).ConfigureAwait(false);

        Func<Task<int>> Method_NestedInUsingScope = async () =>
        {
            using (new Process())
            {
                return await ValueTasks.FromResult(3);
            }
        };

        Func<int, Task<int>> Method_NestedInUsingScope2 = async (int x) =>
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }
            }

            throw new NotImplementedException();
        };

        Func<Task<int>> Method_UsingDeclaration = async () =>
        {
            using var p = new Process();

            return await ValueTasks.FromResult(3);
        };

        Func<int, Task<int>> Method_UsingDeclaration2 = async (int x) =>
        {
            using var p = new Process();

            if (x > 2)
            {
                return await ValueTasks.FromResult(3);
            }

            throw new NotImplementedException();
        };

        Func<Task<int>> Method_NestedInTryBlock = async () =>
        {
            try
            {
                return await ValueTasks.FromResult(3);
            }
            catch
            {
                throw;
            }
        };

        Func<int, Task<int>> Method_NestedInTryBlock2 = async (int x) =>
        {
            try
            {
                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }
            }
            catch
            {
                throw;
            }

            throw new NotImplementedException();
        };
    }

    public class AwaitForAnonymousMethodVariables
    {
        void Method()
        {
            Func<Task> Method = async delegate
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);
            };

            Func<Task> Method2 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    await ValueTasks.Delay(10);
                }

                await ValueTasks.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            };

            Func<Task> Method3 = async delegate { await ValueTasks.Delay(10); };

            Func<Task<int>> Method4 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    return await ValueTasks.FromResult(3);
                }

                await ValueTasks.Delay(10);
                return await ValueTasks.FromResult(4);
            };

            Func<Task<int>> Method5 = async delegate
            {
                await ValueTasks.Delay(10);
                return await ValueTasks.FromResult(3);

                int LocalFunction() => 4;
            };

            Func<Task> Method6 = async delegate { await ValueTasks.FromResult(3); };

            Func<Task<int>> Method7 = async delegate { return await ValueTasks.FromResult(3); };

            Func<Task<int>> Method_AwaitNonLast = async delegate
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);

                return 3;
            };

            Func<Task> Method2_AwaitNonLast = async delegate
            {
                using (new Process())
                {
                    await ValueTasks.Delay(10);
                    await ValueTasks.Delay(20);
                }
            };

            Func<Task<int>> Method3_AwaitNonLast = async delegate
            {
                await ValueTasks.Delay(10);
                var result = await ValueTasks.FromResult(3);
                return result;
            };

            Func<Task> Method_WithConfigureAwait = async delegate { await ValueTasks.Delay(10).ConfigureAwait(false); };

            Func<Task> Method_WithConfigureAwait_AsExpressionBodied = async delegate { await ValueTasks.Delay(10).ConfigureAwait(false); };

            Func<Task<int>> Method_NestedInUsingScope = async delegate
            {
                using (new Process())
                {
                    return await ValueTasks.FromResult(3);
                }
            };

            Func<int, Task<int>> Method_NestedInUsingScope2 = async delegate(int x)
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await ValueTasks.FromResult(3);
                    }
                }

                throw new NotImplementedException();
            };

            Func<Task<int>> Method_UsingDeclaration = async delegate
            {
                using var p = new Process();

                return await ValueTasks.FromResult(3);
            };

            Func<int, Task<int>> Method_UsingDeclaration2 = async delegate(int x)
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }

                throw new NotImplementedException();
            };

            Func<Task<int>> Method_NestedInTryBlock = async delegate
            {
                try
                {
                    return await ValueTasks.FromResult(3);
                }
                catch
                {
                    throw;
                }
            };

            Func<int, Task<int>> Method_NestedInTryBlock2 = async delegate(int x)
            {
                try
                {
                    if (x > 2)
                    {
                        return await ValueTasks.FromResult(3);
                    }
                }
                catch
                {
                    throw;
                }

                throw new NotImplementedException();
            };
        }
    }

    public class AwaitForAnonymousMethodFields
    {
        Func<Task> Method = async delegate
        {
            await ValueTasks.Delay(10);
            await ValueTasks.Delay(20);
        };

        Func<Task> Method2 = async delegate
        {
            if (Environment.UserInteractive)
            {
                await ValueTasks.Delay(10);
            }

            await ValueTasks.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        };

        Func<Task> Method3 = async delegate { await ValueTasks.Delay(10); };

        Func<Task<int>> Method4 = async delegate
        {
            if (Environment.UserInteractive)
            {
                return await ValueTasks.FromResult(3);
            }

            await ValueTasks.Delay(10);
            return await ValueTasks.FromResult(4);
        };

        Func<Task<int>> Method5 = async delegate
        {
            await ValueTasks.Delay(10);
            return await ValueTasks.FromResult(3);

            int LocalFunction() => 4;
        };

        Func<Task> Method6 = async delegate { await ValueTasks.FromResult(3); };

        Func<Task<int>> Method7 = async delegate { return await ValueTasks.FromResult(3); };

        Func<Task<int>> Method_AwaitNonLast = async delegate
        {
            await ValueTasks.Delay(10);
            await ValueTasks.Delay(20);

            return 3;
        };

        Func<Task> Method2_AwaitNonLast = async delegate
        {
            using (new Process())
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);
            }
        };

        Func<Task<int>> Method3_AwaitNonLast = async delegate
        {
            await ValueTasks.Delay(10);
            var result = await ValueTasks.FromResult(3);
            return result;
        };

        Func<Task> Method_WithConfigureAwait = async delegate { await ValueTasks.Delay(10).ConfigureAwait(false); };

        Func<Task> Method_WithConfigureAwait_AsExpressionBodied = async delegate { await ValueTasks.Delay(10).ConfigureAwait(false); };

        Func<Task<int>> Method_NestedInUsingScope = async delegate
        {
            using (new Process())
            {
                return await ValueTasks.FromResult(3);
            }
        };

        Func<int, Task<int>> Method_NestedInUsingScope2 = async delegate(int x)
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }
            }

            throw new NotImplementedException();
        };

        Func<Task<int>> Method_UsingDeclaration = async delegate
        {
            using var p = new Process();

            return await ValueTasks.FromResult(3);
        };

        Func<int, Task<int>> Method_UsingDeclaration2 = async delegate(int x)
        {
            using var p = new Process();

            if (x > 2)
            {
                return await ValueTasks.FromResult(3);
            }

            throw new NotImplementedException();
        };

        Func<Task<int>> Method_NestedInTryBlock = async delegate
        {
            try
            {
                return await ValueTasks.FromResult(3);
            }
            catch
            {
                throw;
            }
        };

        Func<int, Task<int>> Method_NestedInTryBlock2 = async delegate(int x)
        {
            try
            {
                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }
            }
            catch
            {
                throw;
            }

            throw new NotImplementedException();
        };
    }

    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async Task Method()
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);
            }

            async Task Method2()
            {
                if (Environment.UserInteractive)
                {
                    await ValueTasks.Delay(10);
                }

                await ValueTasks.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            }

            async Task Method3() => await ValueTasks.Delay(10);

            async Task<int> Method4()
            {
                if (Environment.UserInteractive)
                {
                    return await ValueTasks.FromResult(3);
                }

                await ValueTasks.Delay(10);
                return await ValueTasks.FromResult(4);
            }

            async Task<int> Method5()
            {
                await ValueTasks.Delay(10);
                return await ValueTasks.FromResult(3);

                int LocalFunction() => 4;
            }

            async Task Method6() => await ValueTasks.FromResult(3);

            async Task<int> Method7() => await ValueTasks.FromResult(3);

            async Task<int> Method_AwaitNonLast()
            {
                await ValueTasks.Delay(10);
                await ValueTasks.Delay(20);

                return 3;
            }

            async Task Method2_AwaitNonLast()
            {
                using (new Process())
                {
                    await ValueTasks.Delay(10);
                    await ValueTasks.Delay(20);
                }
            }

            async Task<int> Method3_AwaitNonLast()
            {
                await ValueTasks.Delay(10);
                var result = await ValueTasks.FromResult(3);
                return result;
            }

            async Task Method_WithConfigureAwait()
            {
                await ValueTasks.Delay(10).ConfigureAwait(false);
            }

            async Task Method_WithConfigureAwait_AsExpressionBodied() => await ValueTasks.Delay(10).ConfigureAwait(false);

            async Task<int> Method_NestedInUsingScope()
            {
                using (new Process())
                {
                    return await ValueTasks.FromResult(3);
                }
            }

            async Task<int> Method_NestedInUsingScope2(int x)
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await ValueTasks.FromResult(3);
                    }
                }

                throw new NotImplementedException();
            }

            async Task<int> Method_UsingDeclaration()
            {
                using var p = new Process();

                return await ValueTasks.FromResult(3);
            }

            async Task<int> Method_UsingDeclaration2(int x)
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await ValueTasks.FromResult(3);
                }

                throw new NotImplementedException();
            }

            async Task<int> Method_NestedInTryBlock()
            {
                try
                {
                    return await ValueTasks.FromResult(3);
                }
                catch
                {
                    throw;
                }
            }

            async Task<int> Method_NestedInTryBlock2(int x)
            {
                try
                {
                    if (x > 2)
                    {
                        return await ValueTasks.FromResult(3);
                    }
                }
                catch
                {
                    throw;
                }

                throw new NotImplementedException();
            }
        }
    }
}