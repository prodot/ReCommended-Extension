using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForAnonymousMethodVariables
    {
        void Method()
        {
            Func<Task> AnonymousMethodVariable4_WithConfigureAwait = async delegate
            {
                Console.WriteLine();
                await{caret} Task.FromResult("one").ConfigureAwait(false);
            };
        }
    }
}