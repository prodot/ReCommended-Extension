using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaFields
    {
        Func<Task> LambdaField4 = async () =>
        {
            Console.WriteLine();
            await{caret} Task.FromResult("one");
        };
    }
}