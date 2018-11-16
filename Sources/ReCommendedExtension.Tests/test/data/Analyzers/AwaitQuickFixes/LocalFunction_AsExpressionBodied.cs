using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLocalFunctions
    {
        void Method()
        {
            async Task LocalFunction_AsExpressionBodied() => await{caret} Task.Delay(10);
        }
    }
}