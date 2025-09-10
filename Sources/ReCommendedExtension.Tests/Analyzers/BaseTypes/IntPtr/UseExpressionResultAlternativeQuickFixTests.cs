using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.IntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet70]
public sealed class UseExpressionResultAlternativeQuickFixTests : QuickFixTestBase<UseExpressionResultAlternativeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\IntPtr\QuickFixes";

    [Test]
    public void TestClamp_Alternative() => DoNamedTest2();
}