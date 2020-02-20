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

            await{caret} Task.Delay(10);
        }
    }
}