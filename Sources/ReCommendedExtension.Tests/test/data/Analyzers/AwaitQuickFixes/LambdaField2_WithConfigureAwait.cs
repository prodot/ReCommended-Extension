using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaFields
    {
        Func<Task<int>> LambdaField2_WithConfigureAwait = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await{caret} Task.FromResult(LocalFunction()).ConfigureAwait(false);

            int LocalFunction() => 3;
        };
    }
}