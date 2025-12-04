using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
public sealed class RemoveLinqQueryQuickFixTests : QuickFixTestBase<RedundantLinqQueryHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Linq\QuickFixes";

    [Test]
    public void TestNoOpQuery() => DoNamedTest2();

    [Test]
    public void TestNoOpQuery_Parenthesized() => DoNamedTest2();
}