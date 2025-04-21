using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.Await
{
    public class AwaitForMethods
    {
        async ValueTask Method()
        {
            await Task.Delay(10);
            await Task.Delay(20);
        }

        async ValueTask Method2()
        {
            if (Environment.UserInteractive)
            {
                await Task.Delay(10);
            }

            await Task.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        }

        async ValueTask Method3() => await Task.Delay(10);

        async ValueTask<int> Method4()
        {
            if (Environment.UserInteractive)
            {
                return await Task.FromResult(3);
            }

            await Task.Delay(10);
            return await Task.FromResult(4);
        }

        async ValueTask<int> Method5()
        {
            await Task.Delay(10);
            return await Task.FromResult(3);

            int LocalFunction() => 4;
        }

        async ValueTask Method6() => await Task.FromResult(3);

        async ValueTask<int> Method7() => await Task.FromResult(3);

        async ValueTask<int> Method_AwaitNonLast()
        {
            await Task.Delay(10);
            await Task.Delay(20);

            return 3;
        }

        async ValueTask Method2_AwaitNonLast()
        {
            using (new Process())
            {
                await Task.Delay(10);
                await Task.Delay(20);
            }
        }

        async ValueTask<int> Method3_AwaitNonLast()
        {
            await Task.Delay(10);
            var result = await Task.FromResult(3);
            return result;
        }

        async ValueTask Method_AwaitNonTask()
        {
            await Task.Delay(10);
            await Task.Yield();
        }

        async ValueTask Method_WithConfigureAwait()
        {
            await Task.Delay(10).ConfigureAwait(false);
        }

        async ValueTask Method_WithConfigureAwait_AsExpressionBodied() => await Task.Delay(10).ConfigureAwait(false);

        async ValueTask<int> Method_NestedInUsingScope()
        {
            using (new Process())
            {
                return await Task.FromResult(3);
            }
        }

        async ValueTask<int> Method_NestedInUsingScope(int x)
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }

            throw new NotImplementedException();
        }

        async ValueTask<int> Method_UsingDeclaration()
        {
            using var p = new Process();

            return await Task.FromResult(3);
        }

        async ValueTask<int> Method_UsingDeclaration(int x)
        {
            using var p = new Process();

            if (x > 2)
            {
                return await Task.FromResult(3);
            }

            throw new NotImplementedException();
        }

        async ValueTask<int> Method_NestedInTryBlock()
        {
            try
            {
                return await Task.FromResult(3);
            }
            catch
            {
                throw;
            }
        }

        async ValueTask<int> Method_NestedInTryBlock(int x)
        {
            try
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
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
            Func<ValueTask> Method = async () =>
            {
                await Task.Delay(10);
                await Task.Delay(20);
            };

            Func<ValueTask> Method2 = async () =>
            {
                if (Environment.UserInteractive)
                {
                    await Task.Delay(10);
                }

                await Task.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            };

            Func<ValueTask> Method3 = async () => await Task.Delay(10);

            Func<ValueTask<int>> Method4 = async () =>
            {
                if (Environment.UserInteractive)
                {
                    return await Task.FromResult(3);
                }

                await Task.Delay(10);
                return await Task.FromResult(4);
            };

            Func<ValueTask<int>> Method5 = async () =>
            {
                await Task.Delay(10);
                return await Task.FromResult(3);

                int LocalFunction() => 4;
            };

            Func<ValueTask> Method6 = async () => await Task.FromResult(3);

            Func<ValueTask<int>> Method7 = async () => await Task.FromResult(3);

            Func<ValueTask<int>> Method_AwaitNonLast = async () =>
            {
                await Task.Delay(10);
                await Task.Delay(20);

                return 3;
            };

            Func<ValueTask> Method2_AwaitNonLast = async () =>
            {
                using (new Process())
                {
                    await Task.Delay(10);
                    await Task.Delay(20);
                }
            };

            Func<ValueTask<int>> Method3_AwaitNonLast = async () =>
            {
                await Task.Delay(10);
                var result = await Task.FromResult(3);
                return result;
            };

            Func<ValueTask> Method_AwaitNonTask = async () =>
            {
                await Task.Delay(10);
                await Task.Yield();
            };

            Func<ValueTask> Method_WithConfigureAwait = async () => { await Task.Delay(10).ConfigureAwait(false); };

            Func<ValueTask> Method_WithConfigureAwait_AsExpressionBodied = async () => await Task.Delay(10).ConfigureAwait(false);

            Func<ValueTask<int>> Method_NestedInUsingScope = async () =>
            {
                using (new Process())
                {
                    return await Task.FromResult(3);
                }
            };

            Func<int, ValueTask<int>> Method_NestedInUsingScope2 = async (int x) =>
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }

                throw new NotImplementedException();
            };

            Func<ValueTask<int>> Method_UsingDeclaration = async () =>
            {
                using var p = new Process();

                return await Task.FromResult(3);
            };

            Func<int, ValueTask<int>> Method_UsingDeclaration2 = async (int x) =>
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await Task.FromResult(3);
                }

                throw new NotImplementedException();
            };

            Func<ValueTask<int>> Method_NestedInTryBlock = async () =>
            {
                try
                {
                    return await Task.FromResult(3);
                }
                catch
                {
                    throw;
                }
            };

            Func<int, ValueTask<int>> Method_NestedInTryBlock2 = async (int x) =>
            {
                try
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
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
        Func<ValueTask> Method = async () =>
        {
            await Task.Delay(10);
            await Task.Delay(20);
        };

        Func<ValueTask> Method2 = async () =>
        {
            if (Environment.UserInteractive)
            {
                await Task.Delay(10);
            }

            await Task.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        };

        Func<ValueTask> Method3 = async () => await Task.Delay(10);

        Func<ValueTask<int>> Method4 = async () =>
        {
            if (Environment.UserInteractive)
            {
                return await Task.FromResult(3);
            }

            await Task.Delay(10);
            return await Task.FromResult(4);
        };

        Func<ValueTask<int>> Method5 = async () =>
        {
            await Task.Delay(10);
            return await Task.FromResult(3);

            int LocalFunction() => 4;
        };

        Func<ValueTask> Method6 = async () => await Task.FromResult(3);

        Func<ValueTask<int>> Method7 = async () => await Task.FromResult(3);

        Func<ValueTask<int>> Method_AwaitNonLast = async () =>
        {
            await Task.Delay(10);
            await Task.Delay(20);

            return 3;
        };

        Func<ValueTask> Method2_AwaitNonLast = async () =>
        {
            using (new Process())
            {
                await Task.Delay(10);
                await Task.Delay(20);
            }
        };

        Func<ValueTask<int>> Method3_AwaitNonLast = async () =>
        {
            await Task.Delay(10);
            var result = await Task.FromResult(3);
            return result;
        };

        Func<ValueTask> Method_AwaitNonTask = async () =>
        {
            await Task.Delay(10);
            await Task.Yield();
        };

        Func<ValueTask> Method_WithConfigureAwait = async () => { await Task.Delay(10).ConfigureAwait(false); };

        Func<ValueTask> Method_WithConfigureAwait_AsExpressionBodied = async () => await Task.Delay(10).ConfigureAwait(false);

        Func<ValueTask<int>> Method_NestedInUsingScope = async () =>
        {
            using (new Process())
            {
                return await Task.FromResult(3);
            }
        };

        Func<int, ValueTask<int>> Method_NestedInUsingScope2 = async (int x) =>
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }

            throw new NotImplementedException();
        };

        Func<ValueTask<int>> Method_UsingDeclaration = async () =>
        {
            using var p = new Process();

            return await Task.FromResult(3);
        };

        Func<int, ValueTask<int>> Method_UsingDeclaration2 = async (int x) =>
        {
            using var p = new Process();

            if (x > 2)
            {
                return await Task.FromResult(3);
            }

            throw new NotImplementedException();
        };

        Func<ValueTask<int>> Method_NestedInTryBlock = async () =>
        {
            try
            {
                return await Task.FromResult(3);
            }
            catch
            {
                throw;
            }
        };

        Func<int, ValueTask<int>> Method_NestedInTryBlock2 = async (int x) =>
        {
            try
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
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
            Func<ValueTask> Method = async delegate
            {
                await Task.Delay(10);
                await Task.Delay(20);
            };

            Func<ValueTask> Method2 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    await Task.Delay(10);
                }

                await Task.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            };

            Func<ValueTask> Method3 = async delegate { await Task.Delay(10); };

            Func<ValueTask<int>> Method4 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    return await Task.FromResult(3);
                }

                await Task.Delay(10);
                return await Task.FromResult(4);
            };

            Func<ValueTask<int>> Method5 = async delegate
            {
                await Task.Delay(10);
                return await Task.FromResult(3);

                int LocalFunction() => 4;
            };

            Func<ValueTask> Method6 = async delegate { await Task.FromResult(3); };

            Func<ValueTask<int>> Method7 = async delegate { return await Task.FromResult(3); };

            Func<ValueTask<int>> Method_AwaitNonLast = async delegate
            {
                await Task.Delay(10);
                await Task.Delay(20);

                return 3;
            };

            Func<ValueTask> Method2_AwaitNonLast = async delegate
            {
                using (new Process())
                {
                    await Task.Delay(10);
                    await Task.Delay(20);
                }
            };

            Func<ValueTask<int>> Method3_AwaitNonLast = async delegate
            {
                await Task.Delay(10);
                var result = await Task.FromResult(3);
                return result;
            };

            Func<ValueTask> Method_AwaitNonTask = async delegate
            {
                await Task.Delay(10);
                await Task.Yield();
            };

            Func<ValueTask> Method_WithConfigureAwait = async delegate { await Task.Delay(10).ConfigureAwait(false); };

            Func<ValueTask> Method_WithConfigureAwait_AsExpressionBodied = async delegate { await Task.Delay(10).ConfigureAwait(false); };

            Func<ValueTask<int>> Method_NestedInUsingScope = async delegate
            {
                using (new Process())
                {
                    return await Task.FromResult(3);
                }
            };

            Func<int, ValueTask<int>> Method_NestedInUsingScope2 = async delegate(int x)
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }

                throw new NotImplementedException();
            };

            Func<ValueTask<int>> Method_UsingDeclaration = async delegate
            {
                using var p = new Process();

                return await Task.FromResult(3);
            };

            Func<int, ValueTask<int>> Method_UsingDeclaration2 = async delegate(int x)
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await Task.FromResult(3);
                }

                throw new NotImplementedException();
            };

            Func<ValueTask<int>> Method_NestedInTryBlock = async delegate
            {
                try
                {
                    return await Task.FromResult(3);
                }
                catch
                {
                    throw;
                }
            };

            Func<int, ValueTask<int>> Method_NestedInTryBlock2 = async delegate(int x)
            {
                try
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
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
        Func<ValueTask> Method = async delegate
        {
            await Task.Delay(10);
            await Task.Delay(20);
        };

        Func<ValueTask> Method2 = async delegate
        {
            if (Environment.UserInteractive)
            {
                await Task.Delay(10);
            }

            await Task.Delay(20);

            int LocalFunction()
            {
                return 4;
            }
        };

        Func<ValueTask> Method3 = async delegate { await Task.Delay(10); };

        Func<ValueTask<int>> Method4 = async delegate
        {
            if (Environment.UserInteractive)
            {
                return await Task.FromResult(3);
            }

            await Task.Delay(10);
            return await Task.FromResult(4);
        };

        Func<ValueTask<int>> Method5 = async delegate
        {
            await Task.Delay(10);
            return await Task.FromResult(3);

            int LocalFunction() => 4;
        };

        Func<ValueTask> Method6 = async delegate { await Task.FromResult(3); };

        Func<ValueTask<int>> Method7 = async delegate { return await Task.FromResult(3); };

        Func<ValueTask<int>> Method_AwaitNonLast = async delegate
        {
            await Task.Delay(10);
            await Task.Delay(20);

            return 3;
        };

        Func<ValueTask> Method2_AwaitNonLast = async delegate
        {
            using (new Process())
            {
                await Task.Delay(10);
                await Task.Delay(20);
            }
        };

        Func<ValueTask<int>> Method3_AwaitNonLast = async delegate
        {
            await Task.Delay(10);
            var result = await Task.FromResult(3);
            return result;
        };

        Func<ValueTask> Method_AwaitNonTask = async delegate
        {
            await Task.Delay(10);
            await Task.Yield();
        };

        Func<ValueTask> Method_WithConfigureAwait = async delegate { await Task.Delay(10).ConfigureAwait(false); };

        Func<ValueTask> Method_WithConfigureAwait_AsExpressionBodied = async delegate { await Task.Delay(10).ConfigureAwait(false); };

        Func<ValueTask<int>> Method_NestedInUsingScope = async delegate
        {
            using (new Process())
            {
                return await Task.FromResult(3);
            }
        };

        Func<int, ValueTask<int>> Method_NestedInUsingScope2 = async delegate(int x)
        {
            using (new Process())
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
                }
            }

            throw new NotImplementedException();
        };

        Func<ValueTask<int>> Method_UsingDeclaration = async delegate
        {
            using var p = new Process();

            return await Task.FromResult(3);
        };

        Func<int, ValueTask<int>> Method_UsingDeclaration2 = async delegate(int x)
        {
            using var p = new Process();

            if (x > 2)
            {
                return await Task.FromResult(3);
            }

            throw new NotImplementedException();
        };

        Func<ValueTask<int>> Method_NestedInTryBlock = async delegate
        {
            try
            {
                return await Task.FromResult(3);
            }
            catch
            {
                throw;
            }
        };

        Func<int, ValueTask<int>> Method_NestedInTryBlock2 = async delegate(int x)
        {
            try
            {
                if (x > 2)
                {
                    return await Task.FromResult(3);
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
            async ValueTask Method()
            {
                await Task.Delay(10);
                await Task.Delay(20);
            }

            async ValueTask Method2()
            {
                if (Environment.UserInteractive)
                {
                    await Task.Delay(10);
                }

                await Task.Delay(20);

                int LocalFunction()
                {
                    return 4;
                }
            }

            async ValueTask Method3() => await Task.Delay(10);

            async ValueTask<int> Method4()
            {
                if (Environment.UserInteractive)
                {
                    return await Task.FromResult(3);
                }

                await Task.Delay(10);
                return await Task.FromResult(4);
            }

            async ValueTask<int> Method5()
            {
                await Task.Delay(10);
                return await Task.FromResult(3);

                int LocalFunction() => 4;
            }

            async ValueTask Method6() => await Task.FromResult(3);

            async ValueTask<int> Method7() => await Task.FromResult(3);

            async ValueTask<int> Method_AwaitNonLast()
            {
                await Task.Delay(10);
                await Task.Delay(20);

                return 3;
            }

            async ValueTask Method2_AwaitNonLast()
            {
                using (new Process())
                {
                    await Task.Delay(10);
                    await Task.Delay(20);
                }
            }

            async ValueTask<int> Method3_AwaitNonLast()
            {
                await Task.Delay(10);
                var result = await Task.FromResult(3);
                return result;
            }

            async ValueTask Method_AwaitNonTask()
            {
                await Task.Delay(10);
                await Task.Yield();
            }

            async ValueTask Method_WithConfigureAwait()
            {
                await Task.Delay(10).ConfigureAwait(false);
            }

            async ValueTask Method_WithConfigureAwait_AsExpressionBodied() => await Task.Delay(10).ConfigureAwait(false);

            async ValueTask<int> Method_NestedInUsingScope()
            {
                using (new Process())
                {
                    return await Task.FromResult(3);
                }
            }

            async ValueTask<int> Method_NestedInUsingScope2(int x)
            {
                using (new Process())
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
                    }
                }

                throw new NotImplementedException();
            }

            async ValueTask<int> Method_UsingDeclaration()
            {
                using var p = new Process();

                return await Task.FromResult(3);
            }

            async ValueTask<int> Method_UsingDeclaration2(int x)
            {
                using var p = new Process();

                if (x > 2)
                {
                    return await Task.FromResult(3);
                }

                throw new NotImplementedException();
            }

            async ValueTask<int> Method_NestedInTryBlock()
            {
                try
                {
                    return await Task.FromResult(3);
                }
                catch
                {
                    throw;
                }
            }

            async ValueTask<int> Method_NestedInTryBlock2(int x)
            {
                try
                {
                    if (x > 2)
                    {
                        return await Task.FromResult(3);
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