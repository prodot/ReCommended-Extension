using JetBrains.Application.Settings;
using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InterfaceImplementation;

namespace ReCommendedExtension.Tests.Analyzers.InterfaceImplementation;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
public sealed class InterfaceImplementationAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\InterfaceImplementation";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is ImplementOperatorsSuggestion;

    [Test]
    [TestNet60]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestEquatableTypes_NET_6() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void TestEquatableTypes_CS10() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestEquatableTypes() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestComparableTypes() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void TestComparableTypes_CS10() => DoNamedTest2();

    [Test]
    [TestNet60]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestComparableTypes_NET_6() => DoNamedTest2();
}