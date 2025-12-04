using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ValueTask;

namespace ReCommendedExtension.Tests.Analyzers.ValueTask;

[TestFixture]
[TestNetCore30]
public sealed class ValueTaskQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\ValueTask\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is IntentionalBlockingAttemptWarning;

    [Test]
    public void TestIntentionalBlockingAttemptsAvailability() => DoNamedTest2();
}