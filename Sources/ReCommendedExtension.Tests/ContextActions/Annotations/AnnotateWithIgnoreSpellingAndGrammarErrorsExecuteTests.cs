using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[TestNet80(ANNOTATIONS_PACKAGE)]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotateWithIgnoreSpellingAndGrammarErrorsExecuteTests
    : CSharpContextActionExecuteTestBase<AnnotateWithIgnoreSpellingAndGrammarErrors>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithIgnoreSpellingAndGrammarErrors";

    [Test]
    public void TestExecute() => DoNamedTest2();
}