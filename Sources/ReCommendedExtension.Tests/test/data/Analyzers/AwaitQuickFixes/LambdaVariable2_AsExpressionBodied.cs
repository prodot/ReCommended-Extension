﻿using System;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.AwaitQuickFixes
{
    public class AwaitForLambdaVariables
    {
        void Method()
        {
            Func<Task<int>> LambdaVariable2_AsExpressionBodied = async () => await{caret} Task.FromResult(3);
        }
    }
}