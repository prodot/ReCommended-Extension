using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Nullable;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp70)]
public sealed class UseNullableHasValueObjectPatternFixTests : QuickFixTestBase<UseNullableHasValueObjectPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Nullable\QuickFixes";

    [Test]
    public void TestHasValue_ObjectPattern() => DoNamedTest2();

    [Test]
    public void TestHasValue_ObjectPattern_Parenthesized() => DoNamedTest2();
}