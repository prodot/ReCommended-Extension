using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        async{caret} Task<int> Method2_WithConfigureAwait()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Task.FromResult(LocalFunction()).ConfigureAwait(false);

            int LocalFunction() => 3;
        }
    }
}