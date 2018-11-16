using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        async{caret} Task<int> Method2_AsExpressionBodied() => await Task.FromResult(3);
    }
}