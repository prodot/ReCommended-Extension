using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async Task<int> LocalFunction2_WithConfigureAwait()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await{caret} Task.FromResult(LocalFunction()).ConfigureAwait(false);

                int LocalFunction() => 3;
            }
        }
    }
}