using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async Task LocalFunction4_WithConfigureAwait()
            {
                Console.WriteLine();
                await{caret} Task.FromResult("one").ConfigureAwait(false);
            }
        }
    }
}