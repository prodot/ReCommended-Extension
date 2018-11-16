using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaFields
    {
        Func<Task> LambdaField_WithConfigureAwait = async () =>
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await{caret} Task.Delay(10).ConfigureAwait(false);
        };
    }
}