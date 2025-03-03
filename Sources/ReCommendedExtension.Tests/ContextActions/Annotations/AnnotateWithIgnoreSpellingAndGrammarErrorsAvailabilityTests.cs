using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80(ANNOTATIONS_PACKAGE)]
public sealed class AnnotateWithIgnoreSpellingAndGrammarErrorsAvailabilityTests
    : CSharpContextActionAvailabilityTestBase<AnnotateWithIgnoreSpellingAndGrammarErrors>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithIgnoreSpellingAndGrammarErrors";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}