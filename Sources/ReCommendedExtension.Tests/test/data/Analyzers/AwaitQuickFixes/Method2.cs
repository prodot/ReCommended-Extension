using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        async{caret} Task<int> Method2()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine();
            }

            return await Task.FromResult(LocalFunction());

            int LocalFunction() => 3;
        }
    }
}