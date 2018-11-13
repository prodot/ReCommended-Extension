using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForAnonymousMethodVariables
    {
        void Method()
        {
            Func<Task<int>> AnonymousMethodVariable2 = async delegate
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                return await{caret} Task.FromResult(LocalFunction());

                int LocalFunction() => 3;
            };
        }
    }
}