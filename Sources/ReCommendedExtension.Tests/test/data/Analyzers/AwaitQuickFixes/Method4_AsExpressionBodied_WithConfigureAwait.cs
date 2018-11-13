using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        async Task Method4_AsExpressionBodied_WithConfigureAwait() => {caret}await Task.FromResult("one").ConfigureAwait(false);
    }
}