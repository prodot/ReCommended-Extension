using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
public sealed class RemoveArgumentRangeFixTests : QuickFixTestBase<RedundantArgumentRangeHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    [Test]
    [TestNet60]
    public void RemoveArgumentRange_Middle() => DoNamedTest();

    [Test]
    public void RemoveArgumentRange_Last() => DoNamedTest();

    [Test]
    public void RemoveArgumentRange_Last_Named() => DoNamedTest();

    [Test]
    public void RemoveArgumentRange_OutOfOrder() => DoNamedTest();
}