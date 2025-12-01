using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InternalConstructor;

namespace ReCommendedExtension.Tests.Analyzers.InternalConstructor;

[TestFixture]
public sealed class InternalConstructorAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\InternalConstructor";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is InternalConstructorVisibilitySuggestion;

    [Test]
    public void TestInternalConstructor() => DoNamedTest2();
}