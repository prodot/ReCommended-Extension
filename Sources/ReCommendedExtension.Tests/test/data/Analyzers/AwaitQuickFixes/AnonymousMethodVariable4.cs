using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForAnonymousMethodVariables
    {
        void Method()
        {
            Func<Task> AnonymousMethodVariable4 = async delegate
            {
                Console.WriteLine();
                await{caret} Task.FromResult("one");
            };
        }
    }
}