using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateOnly;

[TestFixture]
[TestNet60]
public sealed class RemoveArgumentRangeFixTests : QuickFixTestBase<RemoveArgumentRangeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateOnly\QuickFixes";

    [Test]
    public void TestTryParseExact() => DoNamedTest2();
}