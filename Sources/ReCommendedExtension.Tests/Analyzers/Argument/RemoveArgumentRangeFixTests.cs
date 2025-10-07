using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
public sealed class RemoveArgumentRangeFixTests : QuickFixTestBase<RemoveArgumentRangeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    [Test]
    [TestNet60]
    public void TestRemoveArgumentRange_Middle() => DoNamedTest2();

    [Test]
    public void TestRemoveArgumentRange_Last() => DoNamedTest2();
}