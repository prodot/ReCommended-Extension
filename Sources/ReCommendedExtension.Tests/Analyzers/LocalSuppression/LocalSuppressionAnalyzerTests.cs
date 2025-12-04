using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.LocalSuppression;

namespace ReCommendedExtension.Tests.Analyzers.LocalSuppression;

[TestFixture]
public sealed class LocalSuppressionAnalyzerTests : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\LocalSuppression";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is LocalSuppressionWarning;

    [Test]
    public void TestLocalSuppression() => DoNamedTest2();
}