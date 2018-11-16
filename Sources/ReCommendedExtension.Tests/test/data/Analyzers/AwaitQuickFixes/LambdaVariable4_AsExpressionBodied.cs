using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaVariables
    {
        void Method()
        {
            Func<Task> LambdaVariable4_AsExpressionBodied = async () => await{caret} Task.FromResult("one");
        }
    }
}