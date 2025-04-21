using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.Xaml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.XamlBindingWithoutMode;

namespace ReCommendedExtension.Tests.Analyzers.XamlBindingWithoutMode;

[TestFixture]
public sealed class XamlBindingWithoutMode : XamlHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\XamlBindingWithoutMode";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is XamlBindingWithoutModeWarning
            || highlighting.IsError() && highlighting is not XamlBindingWithoutContextNotResolvedHighlighting;

    [Test]
    public void TestXamlBindingWithoutMode() => DoNamedTest2();
}