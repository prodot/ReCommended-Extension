﻿using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types;

[TestFixture]
[TestNetFramework4]
public sealed class TimeSpanNegativeExecuteTests : CSharpContextActionExecuteTestBase<TimeSpanNegative>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\TimeSpanNegative";

    [Test]
    public void TestExecute() => DoNamedTest2();
}