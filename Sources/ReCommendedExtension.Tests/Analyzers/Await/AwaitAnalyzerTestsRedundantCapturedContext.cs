﻿using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.CSharp.PropertiesExtender;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Await;

namespace ReCommendedExtension.Tests.Analyzers.Await;

[TestFixture]
[TestNetFramework45]
public sealed class AwaitAnalyzerTestsRedundantCapturedContext : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\Await";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantCapturedContextSuggestion
            or RedundantConfigureAwaitWarning // to figure out which cases are supported by R#
            or NotResolvedError;

    [Test]
    public void TestRedundantCapturedContext() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestRedundantCapturedContext_ValueTask() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestRedundantCapturedContext_ReturnValueTask() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestRedundantCapturedContext_ValueTask_ReturnTask() => DoNamedTest2();

    [Test]
    [TestNetCore30]
    public void TestRedundantCapturedContext_IAsyncTypes() => DoNamedTest2();

    [Test]
    [TestNetCore30]
    public void TestRedundantCapturedContext_LibraryMode()
        => ExecuteWithinSettingsTransaction(
            store =>
            {
                RunGuarded(
                    () => store.SetValue<DaemonProjectSettings, ConfigureAwaitAnalysisMode>(
                        s => s.ConfigureAwaitAnalysisMode,
                        ConfigureAwaitAnalysisMode.Library));

                DoTestSolution("RedundantCapturedContext_LibraryMode.cs");
            });

    [Test]
    [TestNet80]
    public void TestRedundantCapturedContext_NET_8() => DoNamedTest2();

    [Test]
    [TestNet80]
    public void TestRedundantCapturedContext_ValueTask_NET_8() => DoNamedTest2();
}