using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.CSharp.PropertiesExtender;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Await;

namespace ReCommendedExtension.Tests.Analyzers.Await;

[TestFixture]
[TestNetFramework45]
public sealed class AwaitAnalyzerTestsRedundantCapturedContext : CSharpAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Await";

    protected override bool UseHighlighting(IHighlighting highlighting)
        => highlighting is RedundantCapturedContextSuggestion or RedundantConfigureAwaitWarning; // to figure out which cases are supported by R#

    [Test]
    public void RedundantCapturedContext() => DoNamedTest();

    [Test]
    [TestNetCore21]
    public void RedundantCapturedContext_ValueTask() => DoNamedTest();

    [Test]
    [TestNetCore21]
    public void RedundantCapturedContext_ReturnValueTask() => DoNamedTest();

    [Test]
    [TestNetCore21]
    public void RedundantCapturedContext_ValueTask_ReturnTask() => DoNamedTest();

    [Test]
    [TestNetCore30]
    public void RedundantCapturedContext_IAsyncTypes() => DoNamedTest();

    [Test]
    [TestNetCore30]
    public void RedundantCapturedContext_LibraryMode()
        => ExecuteWithinSettingsTransaction(store =>
        {
            RunGuarded(() => store.SetValue<DaemonProjectSettings, ConfigureAwaitAnalysisMode>(
                s => s.ConfigureAwaitAnalysisMode,
                ConfigureAwaitAnalysisMode.Library));

            DoTestSolution("RedundantCapturedContext_LibraryMode.cs");
        });

    [Test]
    [TestNet80]
    public void RedundantCapturedContext_NET_8() => DoNamedTest();

    [Test]
    [TestNet80]
    public void RedundantCapturedContext_ValueTask_NET_8() => DoNamedTest();
}