using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

namespace ReCommendedExtension.Tests.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[TestFixture]
[TestPackagesWithAnnotations]
public sealed class NotifyPropertyChangedInvocatorFromConstructorQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructor\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is NotifyPropertyChangedInvocatorFromConstructorWarning;

    [Test]
    public void TestNotifyPropertyChangedInvocatorFromConstructorAvailability() => DoNamedTest2();
}