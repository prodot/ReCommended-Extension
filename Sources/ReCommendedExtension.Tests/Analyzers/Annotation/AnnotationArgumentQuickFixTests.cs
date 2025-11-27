using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80(ANNOTATIONS_PACKAGE)]
public sealed class AnnotationArgumentQuickFixTests : QuickFixTestBase<RedundantAnnotationArgumentSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation\QuickFixes";

    [Test]
    public void TestRedundantAnnotationArgument() => DoNamedTest2();
}