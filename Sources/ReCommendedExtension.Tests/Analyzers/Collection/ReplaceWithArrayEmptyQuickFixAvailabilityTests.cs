using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[TestNetFramework46]
public sealed class ReplaceWithArrayEmptyQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Collection\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseEmptyForArrayInitializationWarning or { IsError: true };

    [Test]
    public void TestEmptyArrayInitializationAvailability() => DoNamedTest2();
}