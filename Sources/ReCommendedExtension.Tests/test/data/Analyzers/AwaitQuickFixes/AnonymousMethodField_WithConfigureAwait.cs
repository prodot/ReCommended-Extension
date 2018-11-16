using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForAnonymousMethodFields
    {
        Func<Task> AnonymousMethodField_WithConfigureAwait = async delegate
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            await{caret} Task.Delay(10).ConfigureAwait(false);
        };
    }
}