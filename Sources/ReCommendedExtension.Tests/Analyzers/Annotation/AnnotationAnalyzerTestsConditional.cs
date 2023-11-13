using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
public sealed class AnnotationAnalyzerTestsConditional : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is ConditionalAnnotationHint;

    [Test]
    public void TestConditional() => DoNamedTest2();
}