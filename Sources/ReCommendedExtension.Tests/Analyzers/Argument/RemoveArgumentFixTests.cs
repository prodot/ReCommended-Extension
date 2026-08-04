using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Argument;

namespace ReCommendedExtension.Tests.Analyzers.Argument;

[TestFixture]
public sealed class RemoveArgumentFixTests : QuickFixTestBase<RedundantArgumentHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Argument\QuickFixes";

    [Test]
    public void RemoveArgument_First() => DoNamedTest();

    [Test]
    [TestNet70]
    public void RemoveArgument_Middle() => DoNamedTest();

    [Test]
    public void RemoveArgument_Last() => DoNamedTest();

    [Test]
    public void RemoveArgument_Single() => DoNamedTest();

    [Test]
    public void RemoveArgument_OutOfOrder() => DoNamedTest();
}