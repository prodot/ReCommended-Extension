using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        [NotNull]
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