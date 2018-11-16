using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        async Task Method_AsExpressionBodied() => await{caret} Task.Delay(10);
    }
}