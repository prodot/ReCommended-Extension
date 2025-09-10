using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

namespace ReCommendedExtension.Tests.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[TestFixture]
[TestPackagesWithAnnotations]
public sealed class NotifyPropertyChangedInvocatorFromConstructorQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructor\QuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is NotifyPropertyChangedInvocatorFromConstructorWarning || highlighting.IsError();

    [Test]
    public void TestNotifyPropertyChangedInvocatorFromConstructorAvailability() => DoNamedTest2();
}