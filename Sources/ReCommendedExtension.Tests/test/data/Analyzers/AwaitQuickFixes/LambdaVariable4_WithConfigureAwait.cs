using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaVariables
    {
        void Method()
        {
            Func<Task> LambdaVariable4_WithConfigureAwait = async () =>
            {
                Console.WriteLine();
                await{caret} Task.FromResult("one").ConfigureAwait(false);
            };
        }
    }
}