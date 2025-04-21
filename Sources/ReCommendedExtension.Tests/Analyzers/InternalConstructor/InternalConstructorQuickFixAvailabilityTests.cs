﻿using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InternalConstructor;

namespace ReCommendedExtension.Tests.Analyzers.InternalConstructor;

[TestFixture]
public sealed class InternalConstructorQuickFixAvailabilityTests : QuickFixAvailabilityTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\InternalConstructorQuickFixes";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is InternalConstructorVisibilitySuggestion || highlighting.IsError();

    [Test]
    public void TestInternalConstructorAvailability() => DoNamedTest2();
}