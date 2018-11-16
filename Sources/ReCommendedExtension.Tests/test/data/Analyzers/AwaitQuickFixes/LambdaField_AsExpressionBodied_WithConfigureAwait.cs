using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaFields
    {
        Func<Task> LambdaField_AsExpressionBodied_WithConfigureAwait = async () => await{caret} Task.Delay(10).ConfigureAwait(false);
    }
}