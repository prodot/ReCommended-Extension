using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaVariables
    {
        void Method()
        {
            Func<Task> LambdaVariable = async () =>
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine();
                }

                await{caret} Task.Delay(10);}
            };
        }
    }
}