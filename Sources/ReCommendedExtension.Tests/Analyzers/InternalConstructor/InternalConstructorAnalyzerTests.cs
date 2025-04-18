using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InternalConstructor;

namespace ReCommendedExtension.Tests.Analyzers.InternalConstructor;

[TestFixture]
public sealed class InternalConstructorAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\InternalConstructor";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is InternalConstructorVisibilitySuggestion or NotResolvedError;

    [Test]
    public void TestInternalConstructor() => DoNamedTest2();
}