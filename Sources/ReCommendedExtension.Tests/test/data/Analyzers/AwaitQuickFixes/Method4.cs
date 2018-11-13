using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForMethods
    {
        async Task Method4()
        {
            Console.WriteLine();
            {caret}await Task.FromResult("one");}
        }
    }
}