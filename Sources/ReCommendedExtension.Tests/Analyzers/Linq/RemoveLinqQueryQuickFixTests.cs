using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

[TestFixture]
public sealed class RemoveLinqQueryQuickFixTests : QuickFixTestBase<RedundantLinqQueryHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Linq\QuickFixes";

    protected override bool AllowHighlightingOverlap => true;

    [Test]
    public void NoOpQuery() => DoNamedTest();

    [Test]
    public void NoOpQuery_Parenthesized() => DoNamedTest();
}