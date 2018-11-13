using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForAnonymousMethodFields
    {
        Func<Task> AnonymousMethodField4_WithConfigureAwait = async delegate
        {
            Console.WriteLine();
            await{caret} Task.FromResult("one").ConfigureAwait(false);
        };
    }
}