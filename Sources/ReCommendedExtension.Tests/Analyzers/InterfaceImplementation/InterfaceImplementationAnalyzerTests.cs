using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InterfaceImplementation;

namespace ReCommendedExtension.Tests.Analyzers.InterfaceImplementation;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
public sealed class InterfaceImplementationAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\InterfaceImplementation";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is ImplementOperatorsHighlighting;

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet60]
    public void EquatableTypes_NET_6() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet70]
    public void EquatableTypes_CS10() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void EquatableTypes() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void ComparableTypes() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [TestNet70]
    public void ComparableTypes_CS10() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet60]
    public void ComparableTypes_NET_6() => DoNamedTest();

    [Test]
    public void OverriddenEquals() => DoNamedTest();

    [Test]
    public void ImplementedEquals() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void PartialTypes() => DoNamedTest();
}