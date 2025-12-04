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
    public void TestRemoveArgument_First() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRemoveArgument_Middle() => DoNamedTest2();

    [Test]
    public void TestRemoveArgument_Last() => DoNamedTest2();

    [Test]
    public void TestRemoveArgument_Single() => DoNamedTest2();

    [Test]
    public void TestRemoveArgument_OutOfOrder() => DoNamedTest2();
}