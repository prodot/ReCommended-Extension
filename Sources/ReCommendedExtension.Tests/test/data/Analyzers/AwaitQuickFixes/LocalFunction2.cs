using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async Task<int> LocalFunction2()
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await{caret} Task.FromResult(LocalFunction());

                int LocalFunction() => 3;
            }
        }
    }
}