using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.DelegateInvoke;

namespace ReCommendedExtension.Tests.Analyzers.DelegateInvoke;

[TestFixture]
public sealed class DelegateInvokeQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\DelegateInvokeQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantDelegateInvokeSuggestion;

    [Test]
    public void TestDelegateInvokeAvailability() => DoNamedTest2();
}