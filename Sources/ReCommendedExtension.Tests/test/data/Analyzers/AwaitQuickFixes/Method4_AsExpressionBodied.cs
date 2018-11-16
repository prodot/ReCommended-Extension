using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        async Task Method4_AsExpressionBodied() => {caret}await Task.FromResult("one");
    }
}