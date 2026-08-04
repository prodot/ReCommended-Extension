using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.XamlBindingWithoutMode;

namespace ReCommendedExtension.Tests.Analyzers.XamlBindingWithoutMode;

[TestFixture]
public sealed class XamlBindingWithoutModeTests : XamlAnalyzerTests
{
    protected override string RelativeTestDataPath => @"Analyzers\XamlBindingWithoutMode";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is XamlBindingWithoutModeWarning;

    [Test]
    public void XamlBindingWithoutMode() => DoNamedTest();
}