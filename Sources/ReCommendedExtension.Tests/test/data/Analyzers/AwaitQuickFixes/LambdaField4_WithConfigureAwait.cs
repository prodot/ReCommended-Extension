using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaFields
    {
        Func<Task> LambdaField4_WithConfigureAwait = async () =>
        {
            Console.WriteLine();
            await{caret} Task.FromResult("one").ConfigureAwait(false);
        };
    }
}