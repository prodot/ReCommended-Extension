﻿using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ValueTask;

namespace ReCommendedExtension.Tests.Analyzers.ValueTask;

[TestFixture]
[TestNetCore30]
public sealed class ValueTaskQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\ValueTaskQuickFixes";

    protected override bool HighlightingPredicate(
        IHighlighting highlighting,
        IPsiSourceFile psiSourceFile,
        IContextBoundSettingsStore boundSettingsStore)
        => highlighting is IntentionalBlockingAttemptWarning or NotResolvedError;

    [Test]
    public void TestIntentionalBlockingAttemptsAvailability() => DoNamedTest2();
}