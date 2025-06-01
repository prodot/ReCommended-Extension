using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Nullable;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
[TestNetCore20]
public sealed class UseNullableHasValueTuplePatternFixTests : QuickFixTestBase<UseNullableHasValueTuplePatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\NullableQuickFixes";

    [Test]
    public void TestHasValue_TuplePattern() => DoNamedTest2();

    [Test]
    public void TestHasValue_TuplePattern_Parenthesized() => DoNamedTest2();
}