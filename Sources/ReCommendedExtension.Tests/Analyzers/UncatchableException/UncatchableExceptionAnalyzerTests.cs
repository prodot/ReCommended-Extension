using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.UncatchableException;

namespace ReCommendedExtension.Tests.Analyzers.UncatchableException;

[TestFixture]
public sealed class UncatchableExceptionAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\UncatchableException";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UncatchableExceptionWarning or NotResolvedError;

    [Test]
    public void TestUncatchableException() => DoNamedTest2();
}