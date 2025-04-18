﻿using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.LocalSuppression;

namespace ReCommendedExtension.Tests.Analyzers.LocalSuppression;

[TestFixture]
public sealed class LocalSuppressionAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\LocalSuppression";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is LocalSuppressionWarning or NotResolvedError;

    [Test]
    public void TestLocalSuppression() => DoNamedTest2();
}