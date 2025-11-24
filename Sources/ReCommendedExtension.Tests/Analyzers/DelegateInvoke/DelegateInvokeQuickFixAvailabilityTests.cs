using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.DelegateInvoke;

namespace ReCommendedExtension.Tests.Analyzers.DelegateInvoke;

[TestFixture]
[TestNetFramework4]
public sealed class DelegateInvokeQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\DelegateInvoke\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantDelegateInvokeHint or { IsError: true };

    [Test]
    public void TestDelegateInvokeAvailability() => DoNamedTest2();
}