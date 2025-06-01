using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Nullable;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
public sealed class UseNullableHasValueNotNullPatternFixTests : QuickFixTestBase<UseNullableHasValueNotNullPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\NullableQuickFixes";

    [Test]
    public void TestHasValue_NotNullPattern() => DoNamedTest2();

    [Test]
    public void TestHasValue_NotNullPattern_Parenthesized() => DoNamedTest2();
}