﻿using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Elements;

[TestFixture]
[TestNetFramework4]
public sealed class FieldAvailabilityTests : CSharpContextActionAvailabilityTestBase<NotNull>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Elements\Field";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}