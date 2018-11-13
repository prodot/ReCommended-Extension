using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async Task LocalFunction4()
            {
                Console.WriteLine();
                await{caret} Task.FromResult("one");
            }
        }
    }
}