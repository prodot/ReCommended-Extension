using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ConditionalInvocation;

namespace ReCommendedExtension.Tests.Analyzers.ConditionalInvocation;

[TestFixture]
public sealed class ConditionalInvocationAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\ConditionalInvocation";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is ConditionalInvocationHint;

    [Test]
    public void TestConditional() => DoNamedTest2();
}