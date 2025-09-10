using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int16;

[TestFixture]
[TestNet70]
public sealed class UseExpressionResultAlternativeQuickFixTests : QuickFixTestBase<UseExpressionResultAlternativeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int16\QuickFixes";

    [Test]
    public void TestClamp_Alternative() => DoNamedTest2();
}