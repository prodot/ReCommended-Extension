using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForAnonymousMethodFields
    {
        Func<Task> AnonymousMethodField4 = async delegate
        {
            Console.WriteLine();
            await{caret} Task.FromResult("one");
        };
    }
}