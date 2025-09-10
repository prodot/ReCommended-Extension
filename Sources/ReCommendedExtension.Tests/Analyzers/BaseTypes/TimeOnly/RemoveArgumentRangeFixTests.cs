using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeOnly;

[TestFixture]
[TestNet60]
public sealed class RemoveArgumentRangeFixTests : QuickFixTestBase<RemoveArgumentRangeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeOnly\QuickFixes";

    [Test]
    public void TestTryParseExact() => DoNamedTest2();
}